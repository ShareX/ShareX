#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Commands;

namespace ShareX.PhotoEditor.UI.Dialogs;

public static class NewDocumentDialog
{
    public sealed record Result(string Name, int Width, int Height, float Resolution, SKColor Background);

    public static async Task<Result?> ShowAsync(Window owner, SKSizeI clipboard, SKColor backgroundColor)
    {
        string name = "Untitled";
        NumericUpDown width = DialogBuilder.Number(1, 30000, clipboard.Width > 0 ? clipboard.Width : 1920);
        NumericUpDown height = DialogBuilder.Number(1, 30000, clipboard.Height > 0 ? clipboard.Height : 1080);
        NumericUpDown resolution = DialogBuilder.Number(1, 10000, 72);
        int background = 0;
        TextBox nameBox = new() { Text = name };
        List<(string, SKSizeI)> presets =
        [
            ("Clipboard", clipboard), ("Default (1920 × 1080)", new(1920, 1080)), ("HD 1280 × 720", new(1280, 720)), ("4K UHD 3840 × 2160", new(3840, 2160)),
            ("Square 1080 × 1080", new(1080, 1080)), ("Web 1366 × 768", new(1366, 768)), ("Letter 2550 × 3300 (300 ppi)", new(2550, 3300)), ("A4 2480 × 3508 (300 ppi)", new(2480, 3508)),
            ("Icon 512 × 512", new(512, 512))
        ];
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Name:", nameBox);
        DialogBuilder.AddRow(form, "Preset:", DialogBuilder.Combo(presets.Where(p => p.Item2.Width > 0).Select(p => (p.Item1, p.Item2)), presets.First(p => p.Item2.Width > 0).Item2, v =>
        {
            width.Value = v.Width;
            height.Value = v.Height;
            resolution.Value = v.Width >= 2480 ? 300 : 72;
        }));
        DialogBuilder.AddRow(form, "Width (px):", width);
        DialogBuilder.AddRow(form, "Height (px):", height);
        DialogBuilder.AddRow(form, "Resolution (ppi):", resolution);
        DialogBuilder.AddRow(form, "Background:", DialogBuilder.Combo([("White", 0), ("Black", 1), ("Background Color", 2), ("Transparent", 3)], background, v => background = v));
        if (!await DialogBuilder.ShowAsync(owner, "New Document", form, 420, "Create")) return null;
        SKColor fill = background switch { 1 => SKColors.Black, 2 => backgroundColor.WithAlpha(255), 3 => SKColors.Transparent, _ => SKColors.White };
        return new Result(string.IsNullOrWhiteSpace(nameBox.Text) ? "Untitled" : nameBox.Text, (int)(width.Value ?? 1), (int)(height.Value ?? 1), (float)(resolution.Value ?? 72), fill);
    }
}

public static class ExportDialog
{
    public sealed record Result(string Extension, int Quality, int Width, int Height);

    public static async Task<Result?> ShowAsync(Window owner, PhotoDocument doc)
    {
        string extension = ".png";
        int quality = 90;
        NumericUpDown width = DialogBuilder.Number(1, 30000, doc.Width);
        NumericUpDown height = DialogBuilder.Number(1, 30000, doc.Height);
        bool updating = false;
        width.ValueChanged += (_, e) =>
        {
            if (updating || e.NewValue is not decimal w) return;
            updating = true;
            height.Value = Math.Max(1, Math.Round(w * doc.Height / doc.Width));
            updating = false;
        };
        height.ValueChanged += (_, e) =>
        {
            if (updating || e.NewValue is not decimal h) return;
            updating = true;
            width.Value = Math.Max(1, Math.Round(h * doc.Width / doc.Height));
            updating = false;
        };
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Format:", DialogBuilder.Combo([("PNG", ".png"), ("JPEG", ".jpg"), ("WebP", ".webp"), ("GIF", ".gif"), ("BMP", ".bmp"), ("TIFF", ".tif")], extension, v => extension = v));
        DialogBuilder.AddRow(form, "Quality:", DialogBuilder.SliderNumber(1, 100, quality, v => quality = (int)v));
        DialogBuilder.AddRow(form, "Width (px):", width);
        DialogBuilder.AddRow(form, "Height (px):", height);
        if (!await DialogBuilder.ShowAsync(owner, "Export As", form, 420, "Export...")) return null;
        return new Result(extension, quality, (int)(width.Value ?? doc.Width), (int)(height.Value ?? doc.Height));
    }
}

public static class FillDialog
{
    public sealed class Result
    {
        public SKColor Color { get; set; }
        public PatternDefinition? Pattern { get; set; }
        public bool History { get; set; }
        public bool ContentAware { get; set; }
        public LayerBlendMode Mode { get; set; } = LayerBlendMode.Normal;
        public float Opacity { get; set; } = 1;
        public bool PreserveTransparency { get; set; }
    }

    public static async Task<Result?> ShowAsync(PhotoEditorWindow owner)
    {
        Result result = new() { Color = owner.Foreground };
        int contents = 0;
        PatternDefinition pattern = owner.Pattern ?? owner.Patterns[0];
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Contents:", DialogBuilder.Combo([("Foreground Color", 0), ("Background Color", 1), ("Color...", 2), ("Content-Aware", 3), ("Pattern", 4), ("History", 5), ("Black", 6), ("50% Gray", 7), ("White", 8)], contents, v => contents = v));
        DialogBuilder.AddRow(form, "Pattern:", DialogBuilder.Combo(owner.Patterns.Select(p => (p.Name, p)), pattern, v => pattern = v));
        DialogBuilder.AddRow(form, "Mode:", DialogBuilder.Combo(BlendModes.PaintModes.Select(m => (BlendModes.GetName(m), m)), result.Mode, v => result.Mode = v));
        DialogBuilder.AddRow(form, "Opacity (%):", DialogBuilder.SliderNumber(0, 100, 100, v => result.Opacity = (float)v / 100));
        CheckBox preserve = new() { Content = "Preserve Transparency" };
        preserve.IsCheckedChanged += (_, _) => result.PreserveTransparency = preserve.IsChecked == true;
        DialogBuilder.AddFull(form, preserve);
        if (!await DialogBuilder.ShowAsync(owner, "Fill", form, 420)) return null;
        switch (contents)
        {
            case 1: result.Color = owner.Background; break;
            case 2:
                SKColor? picked = await ColorPickerDialog.ShowAsync(owner, owner.Foreground, "Fill Color", owner);
                if (picked == null) return null;
                result.Color = picked.Value;
                break;
            case 3: result.ContentAware = true; break;
            case 4: result.Pattern = pattern; break;
            case 5: result.History = true; break;
            case 6: result.Color = SKColors.Black; break;
            case 7: result.Color = new SKColor(128, 128, 128); break;
            case 8: result.Color = SKColors.White; break;
        }
        return result;
    }
}

public static class ImageSizeDialog
{
    public sealed record Result(int Width, int Height, float Resolution, ResampleMethod Method);

    public static async Task<Result?> ShowAsync(Window owner, int width, int height, float resolution, ResampleMethod method)
    {
        NumericUpDown w = DialogBuilder.Number(1, 30000, width), h = DialogBuilder.Number(1, 30000, height);
        NumericUpDown percent = DialogBuilder.Number(1, 10000, 100, 1, "0.##");
        NumericUpDown dpi = DialogBuilder.Number(1, 10000, resolution);
        bool constrain = true, updating = false;
        void Sync(Action action)
        {
            if (updating) return;
            updating = true;
            action();
            updating = false;
        }
        w.ValueChanged += (_, e) => Sync(() =>
        {
            decimal value = e.NewValue ?? width;
            if (constrain) h.Value = Math.Max(1, Math.Round(value * height / width));
            percent.Value = Math.Round(value * 100 / width, 2);
        });
        h.ValueChanged += (_, e) => Sync(() =>
        {
            decimal value = e.NewValue ?? height;
            if (constrain) w.Value = Math.Max(1, Math.Round(value * width / height));
        });
        percent.ValueChanged += (_, e) => Sync(() =>
        {
            decimal p = e.NewValue ?? 100;
            w.Value = Math.Max(1, Math.Round(width * p / 100));
            h.Value = Math.Max(1, Math.Round(height * p / 100));
        });
        CheckBox link = new() { Content = "Constrain proportions", IsChecked = true };
        link.IsCheckedChanged += (_, _) => constrain = link.IsChecked == true;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddFull(form, new TextBlock { Text = $"Current size: {width} × {height} px", Opacity = 0.8 });
        DialogBuilder.AddRow(form, "Width (px):", w);
        DialogBuilder.AddRow(form, "Height (px):", h);
        DialogBuilder.AddRow(form, "Percent:", percent);
        DialogBuilder.AddFull(form, link);
        DialogBuilder.AddRow(form, "Resolution (ppi):", dpi);
        DialogBuilder.AddRow(form, "Resample:", DialogBuilder.Combo(Enum.GetValues<ResampleMethod>().Select(m => (OptionsBuilder.SplitWords(m.ToString()), m)), method, v => method = v));
        if (!await DialogBuilder.ShowAsync(owner, "Image Size", form, 420)) return null;
        return new Result((int)(w.Value ?? width), (int)(h.Value ?? height), (float)(dpi.Value ?? (decimal)resolution), method);
    }
}

public static class CanvasSizeDialog
{
    public sealed record Result(int Width, int Height, CanvasAnchor Anchor, SKColor Extension);

    public static async Task<Result?> ShowAsync(PhotoEditorWindow owner, int width, int height, SKColor background)
    {
        NumericUpDown w = DialogBuilder.Number(1, 30000, width), h = DialogBuilder.Number(1, 30000, height);
        CheckBox relative = new() { Content = "Relative" };
        relative.IsCheckedChanged += (_, _) =>
        {
            bool r = relative.IsChecked == true;
            w.Minimum = r ? -30000 : 1;
            h.Minimum = r ? -30000 : 1;
            w.Value = r ? 0 : width;
            h.Value = r ? 0 : height;
        };
        CanvasAnchor anchor = CanvasAnchor.Center;
        UniformGrid anchors = new() { Rows = 3, Columns = 3, Width = 96, Height = 96, HorizontalAlignment = HorizontalAlignment.Left };
        void BuildAnchors()
        {
            anchors.Children.Clear();
            foreach (CanvasAnchor a in Enum.GetValues<CanvasAnchor>())
            {
                CanvasAnchor captured = a;
                Button b = new() { Content = a == anchor ? "●" : "", Margin = new Thickness(1), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                b.Click += (_, _) => { anchor = captured; BuildAnchors(); };
                anchors.Children.Add(b);
            }
        }
        BuildAnchors();
        SKColor extension = background;
        int extensionMode = 0;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddFull(form, new TextBlock { Text = $"Current size: {width} × {height} px", Opacity = 0.8 });
        DialogBuilder.AddRow(form, "Width (px):", w);
        DialogBuilder.AddRow(form, "Height (px):", h);
        DialogBuilder.AddFull(form, relative);
        DialogBuilder.AddRow(form, "Anchor:", anchors);
        DialogBuilder.AddRow(form, "Canvas extension color:", DialogBuilder.Combo([("Background", 0), ("Foreground", 1), ("White", 2), ("Black", 3), ("Gray", 4), ("Transparent", 5)], extensionMode, v => extensionMode = v));
        if (!await DialogBuilder.ShowAsync(owner, "Canvas Size", form, 440)) return null;
        int nw = (int)(w.Value ?? width), nh = (int)(h.Value ?? height);
        if (relative.IsChecked == true) { nw += width; nh += height; }
        extension = extensionMode switch { 1 => owner.Foreground, 2 => SKColors.White, 3 => SKColors.Black, 4 => new SKColor(128, 128, 128), 5 => SKColors.Transparent, _ => background };
        return new Result(Math.Max(1, nw), Math.Max(1, nh), anchor, extension);
    }
}

public static class LayerStyleDialog
{
    /// <summary>Edits the layer's style live; returns true when OK is pressed.</summary>
    public static async Task<bool> ShowAsync(PhotoEditorWindow owner, Layer layer)
    {
        LayerStyle style = layer.Style!;
        void Changed() => owner.InvalidateCanvas();
        Control ColorButton(Func<SKColor> get, Action<SKColor> set, string title)
        {
            Button button = new() { Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(get()) } };
            button.Click += async (_, _) =>
            {
                Window? window = TopLevel.GetTopLevel(button) as Window;
                if (window == null) return;
                SKColor? picked = await ColorPickerDialog.ShowAsync(window, get(), title, owner);
                if (picked is SKColor c)
                {
                    set(c);
                    button.Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(c) };
                    Changed();
                }
            };
            return button;
        }

        Grid blending = DialogBuilder.Form();
        DialogBuilder.AddRow(blending, "Blend Mode:", DialogBuilder.Combo(BlendModes.LayerModes.Select(m => (BlendModes.GetName(m), m)), layer.BlendMode, v => { layer.BlendMode = v; Changed(); }));
        DialogBuilder.AddRow(blending, "Opacity:", DialogBuilder.SliderNumber(0, 100, layer.Opacity * 100, v => { layer.Opacity = (float)v / 100; Changed(); }));
        DialogBuilder.AddRow(blending, "Fill Opacity:", DialogBuilder.SliderNumber(0, 100, layer.FillOpacity * 100, v => { layer.FillOpacity = (float)v / 100; Changed(); }));

        Grid shadow = DialogBuilder.Form();
        DialogBuilder.AddRow(shadow, "Color:", ColorButton(() => style.ShadowColor, c => style.ShadowColor = c, "Shadow Color"));
        DialogBuilder.AddRow(shadow, "Opacity:", DialogBuilder.SliderNumber(0, 100, style.ShadowOpacity * 100, v => { style.ShadowOpacity = (float)v / 100; Changed(); }));
        DialogBuilder.AddRow(shadow, "Angle:", DialogBuilder.SliderNumber(-180, 180, style.ShadowAngle, v => { style.ShadowAngle = (float)v; Changed(); }));
        DialogBuilder.AddRow(shadow, "Distance:", DialogBuilder.SliderNumber(0, 300, style.ShadowDistance, v => { style.ShadowDistance = (float)v; Changed(); }));
        DialogBuilder.AddRow(shadow, "Size:", DialogBuilder.SliderNumber(0, 250, style.ShadowSize, v => { style.ShadowSize = (float)v; Changed(); }));

        Grid inner = DialogBuilder.Form();
        DialogBuilder.AddRow(inner, "Color:", ColorButton(() => style.InnerShadowColor, c => style.InnerShadowColor = c, "Inner Shadow Color"));
        DialogBuilder.AddRow(inner, "Opacity:", DialogBuilder.SliderNumber(0, 100, style.InnerShadowOpacity * 100, v => { style.InnerShadowOpacity = (float)v / 100; Changed(); }));
        DialogBuilder.AddRow(inner, "Distance:", DialogBuilder.SliderNumber(0, 300, style.InnerShadowDistance, v => { style.InnerShadowDistance = (float)v; Changed(); }));
        DialogBuilder.AddRow(inner, "Size:", DialogBuilder.SliderNumber(0, 250, style.InnerShadowSize, v => { style.InnerShadowSize = (float)v; Changed(); }));

        Grid glow = DialogBuilder.Form();
        DialogBuilder.AddRow(glow, "Color:", ColorButton(() => style.GlowColor, c => style.GlowColor = c, "Glow Color"));
        DialogBuilder.AddRow(glow, "Opacity:", DialogBuilder.SliderNumber(0, 100, style.GlowOpacity * 100, v => { style.GlowOpacity = (float)v / 100; Changed(); }));
        DialogBuilder.AddRow(glow, "Size:", DialogBuilder.SliderNumber(0, 250, style.GlowSize, v => { style.GlowSize = (float)v; Changed(); }));

        Grid stroke = DialogBuilder.Form();
        DialogBuilder.AddRow(stroke, "Size:", DialogBuilder.SliderNumber(1, 250, style.StrokeSize, v => { style.StrokeSize = (float)v; Changed(); }));
        DialogBuilder.AddRow(stroke, "Position:", DialogBuilder.Combo(Enum.GetValues<StrokePosition>().Select(p => (p.ToString(), p)), style.StrokePosition, v => { style.StrokePosition = v; Changed(); }));
        DialogBuilder.AddRow(stroke, "Opacity:", DialogBuilder.SliderNumber(0, 100, style.StrokeOpacity * 100, v => { style.StrokeOpacity = (float)v / 100; Changed(); }));
        DialogBuilder.AddRow(stroke, "Color:", ColorButton(() => style.StrokeColor, c => style.StrokeColor = c, "Stroke Color"));

        Grid overlay = DialogBuilder.Form();
        DialogBuilder.AddRow(overlay, "Color:", ColorButton(() => style.OverlayColor, c => style.OverlayColor = c, "Overlay Color"));
        DialogBuilder.AddRow(overlay, "Opacity:", DialogBuilder.SliderNumber(0, 100, style.OverlayOpacity * 100, v => { style.OverlayOpacity = (float)v / 100; Changed(); }));

        // Reference-style layout: a list of styles with enable checkboxes on the left, settings on the right.
        (string Title, Func<bool>? Get, Action<bool>? Set, Grid Body)[] sections =
        [
            ("Blending Options", null, null, blending),
            ("Stroke", () => style.Stroke, v => style.Stroke = v, stroke),
            ("Inner Shadow", () => style.InnerShadow, v => style.InnerShadow = v, inner),
            ("Color Overlay", () => style.ColorOverlay, v => style.ColorOverlay = v, overlay),
            ("Outer Glow", () => style.OuterGlow, v => style.OuterGlow = v, glow),
            ("Drop Shadow", () => style.DropShadow, v => style.DropShadow = v, shadow)
        ];
        TextBlock detailTitle = new() { Classes = { "header" }, FontSize = 13, Margin = new Thickness(0, 0, 0, 10) };
        ContentControl detail = new();
        StackPanel list = new() { Spacing = 0 };
        List<Border> rows = [];
        void Select(int index)
        {
            for (int i = 0; i < rows.Count; i++) rows[i].Background = i == index ? new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B)) : Brushes.Transparent;
            detailTitle.Text = sections[index].Title;
            detail.Content = sections[index].Body;
        }
        for (int i = 0; i < sections.Length; i++)
        {
            int index = i;
            var section = sections[i];
            Control label;
            if (section.Get != null)
            {
                CheckBox check = new() { Content = section.Title, IsChecked = section.Get() };
                check.IsCheckedChanged += (_, _) => { section.Set!(check.IsChecked == true); Changed(); Select(index); };
                label = check;
            }
            else
            {
                label = new TextBlock { Text = section.Title, FontWeight = FontWeight.SemiBold, Margin = new Thickness(4, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            }
            Border row = new() { Child = label, Height = 30, Padding = new Thickness(8, 0), Cursor = new Cursor(StandardCursorType.Hand) };
            row.PointerPressed += (_, _) => Select(index);
            rows.Add(row);
            list.Children.Add(row);
        }
        Border sidebar = new()
        {
            Width = 180,
            Background = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E)),
            BorderThickness = new Thickness(1),
            Child = list,
            Padding = new Thickness(0, 4)
        };
        StackPanel detailPanel = new() { Margin = new Thickness(16, 4, 4, 4), MinWidth = 380 };
        detailPanel.Children.Add(detailTitle);
        detailPanel.Children.Add(detail);
        Grid layout = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*"), MinHeight = 340 };
        Grid.SetColumn(detailPanel, 1);
        layout.Children.Add(sidebar);
        layout.Children.Add(detailPanel);
        Select(0);
        return await DialogBuilder.ShowAsync(owner, "Layer Style", layout, 640);
    }
}

public static class PreferencesDialog
{
    public static async Task ShowAsync(PhotoEditorWindow owner)
    {
        EditorSettings s = owner.Settings;
        int history = s.HistoryStates, checker = s.CheckerSize, subdivisions = s.GridSubdivisions;
        float grid = s.GridSpacing;
        ResampleMethod resample = s.DefaultResample;
        string guideColor = s.GuideColor;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "History States:", DialogBuilder.SliderNumber(5, 1000, history, v => history = (int)v));
        DialogBuilder.AddRow(form, "Image Interpolation:", DialogBuilder.Combo(Enum.GetValues<ResampleMethod>().Select(m => (OptionsBuilder.SplitWords(m.ToString()), m)), resample, v => resample = v));
        DialogBuilder.AddRow(form, "Transparency Grid Size:", DialogBuilder.Combo([("Small", 4), ("Medium", 8), ("Large", 16)], checker, v => checker = v));
        DialogBuilder.AddRow(form, "Gridline Every (px):", DialogBuilder.SliderNumber(1, 2000, grid, v => grid = (float)v));
        DialogBuilder.AddRow(form, "Grid Subdivisions:", DialogBuilder.SliderNumber(1, 100, subdivisions, v => subdivisions = (int)v));
        TextBox guide = new() { Text = guideColor };
        DialogBuilder.AddRow(form, "Guide Color (hex):", guide);
        if (!await DialogBuilder.ShowAsync(owner, "Preferences", form, 460)) return;
        s.HistoryStates = history;
        s.CheckerSize = checker;
        s.GridSpacing = grid;
        s.GridSubdivisions = subdivisions;
        s.DefaultResample = resample;
        if (ColorMath.TryParseHex(guide.Text, out _)) s.GuideColor = guide.Text!;
        foreach (DocumentView view in owner.Views) view.Document.History.MaxStates = history;
        owner.SaveSettings();
        owner.InvalidateCanvas();
    }
}

public static class ShortcutsDialog
{
    public static async Task ShowAsync(PhotoEditorWindow owner)
    {
        Dictionary<string, string> edits = new(owner.Settings.Shortcuts);
        TextBox filter = new() { PlaceholderText = "Search commands" };
        StackPanel list = new() { Spacing = 2 };
        void Build()
        {
            list.Children.Clear();
            string term = filter.Text ?? string.Empty;
            foreach (EditorCommand command in CommandRegistry.Commands.Values.OrderBy(c => c.Id))
            {
                if (term.Length > 0 && !command.Text.Contains(term, StringComparison.OrdinalIgnoreCase) && !command.Id.Contains(term, StringComparison.OrdinalIgnoreCase)) continue;
                string current = edits.TryGetValue(command.Id, out string? custom) ? custom : command.DefaultGesture ?? string.Empty;
                TextBox gesture = new() { Text = current, Width = 200, PlaceholderText = "none" };
                string id = command.Id;
                gesture.KeyDown += (_, e) =>
                {
                    // Record the pressed key combination.
                    if (e.Key is Key.LeftCtrl or Key.RightCtrl or Key.LeftShift or Key.RightShift or Key.LeftAlt or Key.RightAlt) return;
                    if (e.Key == Key.Back && e.KeyModifiers == KeyModifiers.None) { gesture.Text = string.Empty; edits[id] = string.Empty; e.Handled = true; return; }
                    if (e.Key == Key.Tab) return;
                    gesture.Text = new KeyGesture(e.Key, e.KeyModifiers).ToString();
                    edits[id] = gesture.Text;
                    e.Handled = true;
                };
                Grid row = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto") };
                TextBlock name = new() { Text = $"{command.Text.Replace("...", string.Empty)}  ({command.Id})", VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
                Grid.SetColumn(gesture, 1);
                row.Children.Add(name);
                row.Children.Add(gesture);
                list.Children.Add(row);
            }
        }
        filter.TextChanged += (_, _) => Build();
        Build();
        Button reset = new() { Content = "Use Defaults" };
        reset.Click += (_, _) => { edits.Clear(); Build(); };
        DockPanel content = new() { Height = 520 };
        DockPanel.SetDock(filter, Dock.Top);
        content.Children.Add(filter);
        content.Children.Add(new ScrollViewer { Content = list, Margin = new Thickness(0, 8, 0, 0) });
        if (!await DialogBuilder.ShowAsync(owner, "Keyboard Shortcuts", content, 640, "OK", null, [reset])) return;
        // Store only gestures that differ from the defaults.
        owner.Settings.Shortcuts = edits.Where(e => CommandRegistry.Get(e.Key)?.DefaultGesture != e.Value).ToDictionary(e => e.Key, e => e.Value);
        owner.SaveSettings();
        owner.RebuildMenu();
    }
}

public static class ToolbarCustomizeDialog
{
    /// <summary>Lets the user reorder tools, regroup them and hide unused ones.</summary>
    public static async Task ShowAsync(PhotoEditorWindow owner)
    {
        List<List<ToolId>> groups = owner.Tools.Groups.Select(g => g.Tools.Select(t => t.Id).ToList()).ToList();
        List<ToolId> hidden = owner.Settings.HiddenTools.Select(n => Enum.TryParse(n, out ToolId id) ? id : (ToolId?)null).Where(i => i != null).Select(i => i!.Value).ToList();
        StackPanel toolbar = new() { Spacing = 4 };
        WrapPanel extra = new();
        (int Group, int Index)? selected = null;

        void Build()
        {
            toolbar.Children.Clear();
            extra.Children.Clear();
            for (int g = 0; g < groups.Count; g++)
            {
                WrapPanel row = new();
                for (int i = 0; i < groups[g].Count; i++)
                {
                    int gi = g, ii = i;
                    ToolBase tool = owner.Tools.Get(groups[g][i]);
                    bool isSelected = selected == (gi, ii);
                    Button b = new()
                    {
                        Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, Children = { ToolIcons.Create(tool.Id, 16), new TextBlock { Text = tool.Name.Replace(" Tool", string.Empty), FontSize = 11 } } },
                        Margin = new Thickness(1),
                        Background = isSelected ? new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B)) : Brushes.Transparent
                    };
                    b.Click += (_, _) => { selected = (gi, ii); Build(); };
                    row.Children.Add(b);
                }
                toolbar.Children.Add(new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(0, 0, 0, 1), Padding = new Thickness(2), Child = row });
            }
            foreach (ToolId id in hidden)
            {
                ToolId captured = id;
                Button b = new() { Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, Children = { ToolIcons.Create(id, 16), new TextBlock { Text = owner.Tools.Get(id).Name, FontSize = 11 } } }, Margin = new Thickness(1) };
                ToolTip.SetTip(b, "Click to restore to the toolbar");
                b.Click += (_, _) =>
                {
                    hidden.Remove(captured);
                    groups.Add([captured]);
                    Build();
                };
                extra.Children.Add(b);
            }
        }

        void Act(Action<int, int> action)
        {
            if (selected is not (int g, int i)) return;
            action(g, i);
            groups.RemoveAll(x => x.Count == 0);
            Build();
        }
        Button up = new() { Content = "Move Group Up" };
        up.Click += (_, _) => Act((g, i) => { if (g > 0) { (groups[g - 1], groups[g]) = (groups[g], groups[g - 1]); selected = (g - 1, i); } });
        Button down = new() { Content = "Move Group Down" };
        down.Click += (_, _) => Act((g, i) => { if (g < groups.Count - 1) { (groups[g + 1], groups[g]) = (groups[g], groups[g + 1]); selected = (g + 1, i); } });
        Button left = new() { Content = "Move Earlier in Group" };
        left.Click += (_, _) => Act((g, i) => { if (i > 0) { (groups[g][i - 1], groups[g][i]) = (groups[g][i], groups[g][i - 1]); selected = (g, i - 1); } });
        Button split = new() { Content = "Own Group" };
        split.Click += (_, _) => Act((g, i) => { ToolId id = groups[g][i]; groups[g].RemoveAt(i); groups.Insert(g + 1, [id]); selected = (g + 1, 0); });
        Button merge = new() { Content = "Merge With Group Above" };
        merge.Click += (_, _) => Act((g, i) => { if (g > 0) { ToolId id = groups[g][i]; groups[g].RemoveAt(i); groups[g - 1].Add(id); selected = null; } });
        Button hide = new() { Content = "Hide Tool" };
        hide.Click += (_, _) => Act((g, i) => { if (groups[g][i] != ToolId.Move) { hidden.Add(groups[g][i]); groups[g].RemoveAt(i); selected = null; } });
        Button restore = new() { Content = "Restore Defaults" };
        restore.Click += (_, _) =>
        {
            groups = owner.Tools.DefaultGroups.Select(g => g.Tools.Select(t => t.Id).ToList()).ToList();
            hidden.Clear();
            selected = null;
            Build();
        };
        Build();
        WrapPanel actions = new() { Children = { up, down, left, split, merge, hide, restore } };
        foreach (Control c in actions.Children) c.Margin = new Thickness(2);
        StackPanel content = new() { Spacing = 8 };
        content.Children.Add(new TextBlock { Text = "Toolbar (each row is a toolbar slot with its flyout tools). Select a tool, then use the buttons to rearrange.", TextWrapping = TextWrapping.Wrap });
        content.Children.Add(new ScrollViewer { Content = toolbar, Height = 360 });
        content.Children.Add(actions);
        content.Children.Add(new TextBlock { Text = "Extra Tools (hidden from the toolbar):" });
        content.Children.Add(extra);
        if (!await DialogBuilder.ShowAsync(owner, "Customize Toolbar", content, 760, "Done")) return;
        owner.Settings.Toolbar = groups.Select(g => new ToolbarGroupLayout { Tools = g.Select(t => t.ToString()).ToList() }).ToList();
        owner.Settings.HiddenTools = hidden.Select(t => t.ToString()).ToList();
        owner.RebuildToolbar();
        owner.SaveSettings();
    }
}
