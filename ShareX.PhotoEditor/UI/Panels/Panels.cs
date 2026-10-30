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
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Panels;

public sealed class HistoryPanel : EditorPanel
{
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B));
    private readonly StackPanel _list = new();

    public HistoryPanel(PhotoEditorWindow editor) : base(editor)
    {
        StackPanel footer = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 2, Margin = new Thickness(4) };
        footer.Children.Add(IconButton("new-document", "Create new document from current state", () => EditorCommands.NewDocumentFromState(Editor)));
        footer.Children.Add(IconButton("snapshot", "Create new snapshot", () =>
        {
            Doc?.History.CreateSnapshot();
            Refresh();
        }));
        footer.Children.Add(IconButton("trash", "Delete states after the current one (purge)", () =>
        {
            Doc?.History.Purge();
            Refresh();
        }));
        DockPanel root = new();
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(footer);
        root.Children.Add(new ScrollViewer { Content = _list, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled });
        Content = root;
    }

    public override string Id => "history";

    public override string Title => "History";

    /// <summary>Picks an icon for a history state from the name of the operation.</summary>
    private static string HistoryIcon(string name)
    {
        string n = name.ToLowerInvariant();
        if (n.Contains("open") || n.Contains("new document") || n.Contains("duplicate image")) return "new-document";
        if (n.Contains("brush") || n.Contains("pencil") || n.Contains("paint") || n.Contains("stamp") || n.Contains("healing") || n.Contains("smudge") || n.Contains("blur") || n.Contains("dodge") || n.Contains("burn") || n.Contains("sponge")) return "brush";
        if (n.Contains("eras")) return "eraser";
        if (n.Contains("select") || n.Contains("marquee") || n.Contains("lasso") || n.Contains("wand") || n.Contains("deselect")) return "rectangularmarquee";
        if (n.Contains("type") || n.Contains("text")) return "horizontaltype";
        if (n.Contains("move") || n.Contains("nudge") || n.Contains("align")) return "move";
        if (n.Contains("crop") || n.Contains("trim")) return "crop";
        if (n.Contains("transform") || n.Contains("rotate") || n.Contains("flip") || n.Contains("scale")) return "freetransform";
        if (n.Contains("gradient")) return "gradient";
        if (n.Contains("fill") || n.Contains("bucket")) return "paintbucket";
        if (n.Contains("layer") || n.Contains("merge") || n.Contains("flatten") || n.Contains("group")) return "new-layer";
        if (n.Contains("shape") || n.Contains("rectangle") || n.Contains("ellipse") || n.Contains("polygon")) return "rectangle";
        if (n.Contains("path") || n.Contains("pen") || n.Contains("anchor")) return "pen";
        return "adjust";
    }

    public override void Refresh()
    {
        _list.Children.Clear();
        PhotoDocument? doc = Doc;
        if (doc == null) return;
        History history = doc.History;
        foreach (HistoryEntry snapshot in history.Snapshots) _list.Children.Add(CreateRow(doc, snapshot, -1, true));
        if (history.Snapshots.Count > 0) _list.Children.Add(new Border { Height = 1, Background = Brushes.Gray, Margin = new Thickness(0, 2) });
        Control? current = null;
        for (int i = 0; i < history.Entries.Count; i++)
        {
            Control row = CreateRow(doc, history.Entries[i], i, false);
            if (i == history.Index) current = row;
            _list.Children.Add(row);
        }
        // Keep the current state visible, like the reference panel.
        if (current != null) Avalonia.Threading.Dispatcher.UIThread.Post(() => current.BringIntoView(), Avalonia.Threading.DispatcherPriority.Background);
    }

    private Control CreateRow(PhotoDocument doc, HistoryEntry entry, int index, bool snapshot)
    {
        History history = doc.History;
        // Left column: the history brush source marker, like the reference panel.
        bool isSource = history.Source == entry;
        Border source = new()
        {
            Width = 22,
            Height = 22,
            Background = Brushes.Transparent,
            Child = isSource ? ToolIcons.Create("historybrush", 14) : null,
            Cursor = new Cursor(StandardCursorType.Hand),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28)),
            BorderThickness = new Thickness(0, 0, 1, 0)
        };
        ToolTip.SetTip(source, "Set the source for the history brush");
        source.PointerPressed += (_, e) =>
        {
            history.Source = entry;
            e.Handled = true;
            Refresh();
        };
        TextBlock name = Text(entry.Name);
        name.Margin = new Thickness(4, 0, 0, 0);
        bool future = !snapshot && index > history.Index;
        if (future) name.Opacity = 0.45;
        Control icon = snapshot ? ToolIcons.Create("snapshot", 16) : ToolIcons.Create(HistoryIcon(entry.Name), 14);
        icon.Opacity = future ? 0.45 : 0.85;
        StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 6, VerticalAlignment = VerticalAlignment.Center };
        content.Children.Add(source);
        content.Children.Add(icon);
        content.Children.Add(name);
        Border row = new()
        {
            Height = snapshot ? 30 : 24,
            Background = !snapshot && index == history.Index ? SelectedBrush : Brushes.Transparent,
            Child = content
        };
        row.PointerPressed += (_, e) =>
        {
            if (snapshot) history.RestoreSnapshot(entry);
            else history.JumpTo(index);
            Editor.AfterHistoryChange();
        };
        if (snapshot)
        {
            ContextMenu menu = new();
            MenuItem rename = new() { Header = "Rename Snapshot..." };
            rename.Click += async (_, _) =>
            {
                string? text = await DialogBuilder.PromptAsync(Editor, "Rename Snapshot", "Name:", entry.Name);
                if (!string.IsNullOrWhiteSpace(text)) { entry.Name = text; Refresh(); }
            };
            MenuItem delete = new() { Header = "Delete Snapshot" };
            delete.Click += (_, _) => { history.DeleteSnapshot(entry); Refresh(); };
            menu.Items.Add(rename);
            menu.Items.Add(delete);
            row.ContextMenu = menu;
        }
        return row;
    }
}

public sealed class ColorPanel : EditorPanel
{
    private readonly Border _fg = new() { Width = 26, Height = 26, BorderThickness = new Thickness(1), Cursor = new Cursor(StandardCursorType.Hand) };
    private readonly Border _bg = new() { Width = 26, Height = 26, BorderThickness = new Thickness(1), Cursor = new Cursor(StandardCursorType.Hand) };
    private readonly Border _field = new() { ClipToBounds = true };
    private readonly Canvas _fieldMarker = new() { IsHitTestVisible = false };
    private readonly Border _hue = new() { Width = 14 };
    private readonly Canvas _hueMarker = new() { IsHitTestVisible = false };
    private readonly TextBox _hex = new() { Width = 64 };
    private readonly NumericUpDown _r = Channel(), _g = Channel(), _b = Channel();
    private float _h, _s, _v;
    private bool _editBackground;
    private bool _updating;

    private static NumericUpDown Channel() => new() { Minimum = 0, Maximum = 255, Increment = 1, FormatString = "0", Width = 40, ShowButtonSpinner = false, HorizontalContentAlignment = HorizontalAlignment.Right };

    public ColorPanel(PhotoEditorWindow editor) : base(editor)
    {
        _fg.PointerPressed += async (_, e) =>
        {
            if (e.ClickCount >= 2)
            {
                SKColor? c = await ColorPickerDialog.ShowAsync(Editor, Editor.Foreground, "Foreground Color", Editor);
                if (c is SKColor col) Editor.Foreground = col;
                return;
            }
            _editBackground = false;
            Refresh();
        };
        _bg.PointerPressed += async (_, e) =>
        {
            if (e.ClickCount >= 2)
            {
                SKColor? c = await ColorPickerDialog.ShowAsync(Editor, Editor.Background, "Background Color", Editor);
                if (c is SKColor col) Editor.Background = col;
                return;
            }
            _editBackground = true;
            Refresh();
        };
        ToolTip.SetTip(_fg, "Foreground color. Double-click opens the Color Picker.");
        ToolTip.SetTip(_bg, "Background color. Double-click opens the Color Picker.");

        // Saturation/brightness field: hue fill with white and black gradient overlays.
        Grid field = new() { Cursor = new Cursor(StandardCursorType.Cross), MinHeight = 90, ClipToBounds = true };
        field.Children.Add(_field);
        field.Children.Add(new Border
        {
            IsHitTestVisible = false,
            Background = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops = { new Avalonia.Media.GradientStop(Colors.White, 0), new Avalonia.Media.GradientStop(Color.FromArgb(0, 255, 255, 255), 1) }
            }
        });
        field.Children.Add(new Border
        {
            IsHitTestVisible = false,
            Background = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                GradientStops = { new Avalonia.Media.GradientStop(Color.FromArgb(0, 0, 0, 0), 0), new Avalonia.Media.GradientStop(Colors.Black, 1) }
            }
        });
        field.Children.Add(_fieldMarker);
        field.PointerPressed += (_, e) => { e.Pointer.Capture(field); PickField(e.GetPosition(field), field.Bounds.Size); };
        field.PointerMoved += (_, e) => { if (e.GetCurrentPoint(field).Properties.IsLeftButtonPressed) PickField(e.GetPosition(field), field.Bounds.Size); };
        field.SizeChanged += (_, _) => PlaceMarkers();

        LinearGradientBrush spectrum = new() { StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative) };
        for (int i = 0; i <= 6; i++)
        {
            SKColor c = ColorMath.FromHsv(360 - i * 60, 1, 1);
            spectrum.GradientStops.Add(new Avalonia.Media.GradientStop(Color.FromRgb(c.Red, c.Green, c.Blue), i / 6.0));
        }
        _hue.Background = spectrum;
        Grid hue = new() { Width = 14, Cursor = new Cursor(StandardCursorType.SizeNorthSouth) };
        hue.Children.Add(_hue);
        hue.Children.Add(_hueMarker);
        hue.PointerPressed += (_, e) => { e.Pointer.Capture(hue); PickHue(e.GetPosition(hue).Y, hue.Bounds.Height); };
        hue.PointerMoved += (_, e) => { if (e.GetCurrentPoint(hue).Properties.IsLeftButtonPressed) PickHue(e.GetPosition(hue).Y, hue.Bounds.Height); };

        Canvas swatches = new() { Width = 40, Height = 40, VerticalAlignment = VerticalAlignment.Top };
        Canvas.SetLeft(_bg, 13);
        Canvas.SetTop(_bg, 13);
        swatches.Children.Add(_bg);
        swatches.Children.Add(_fg);

        Grid picker = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"), ColumnSpacing = 8 };
        Grid.SetColumn(field, 1);
        Grid.SetColumn(hue, 2);
        picker.Children.Add(swatches);
        picker.Children.Add(field);
        picker.Children.Add(hue);

        foreach (NumericUpDown box in new[] { _r, _g, _b }) box.ValueChanged += (_, _) => FromRgb();
        _hex.LostFocus += (_, _) => FromHex();
        _hex.KeyDown += (_, e) => { if (e.Key == Key.Enter) FromHex(); };
        StackPanel values = new() { Orientation = Orientation.Horizontal, Spacing = 4, Margin = new Thickness(44, 6, 0, 0) };
        values.Children.Add(Dim("R"));
        values.Children.Add(_r);
        values.Children.Add(Dim("G"));
        values.Children.Add(_g);
        values.Children.Add(Dim("B"));
        values.Children.Add(_b);
        values.Children.Add(Dim("#"));
        values.Children.Add(_hex);

        DockPanel root = new() { Margin = new Thickness(8, 8, 10, 6) };
        DockPanel.SetDock(values, Dock.Bottom);
        root.Children.Add(values);
        root.Children.Add(picker);
        Content = root;
    }

    private static TextBlock Dim(string text) => new() { Text = text, Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0, 0, 0) };

    public override string Id => "color";

    public override string Title => "Color";

    private SKColor Current => _editBackground ? Editor.Background : Editor.Foreground;

    private void Set(SKColor color)
    {
        if (_editBackground) Editor.Background = color;
        else Editor.Foreground = color;
    }

    private void PickField(Point p, Size size)
    {
        _s = (float)Math.Clamp(p.X / Math.Max(1, size.Width), 0, 1);
        _v = (float)Math.Clamp(1 - p.Y / Math.Max(1, size.Height), 0, 1);
        Set(ColorMath.FromHsv(_h, _s, _v));
    }

    private void PickHue(double y, double height)
    {
        _h = (float)Math.Clamp(360 - y / Math.Max(1, height) * 360, 0, 359.9);
        Set(ColorMath.FromHsv(_h, _s, _v));
    }

    private void FromRgb()
    {
        if (_updating) return;
        Set(new SKColor((byte)(_r.Value ?? 0), (byte)(_g.Value ?? 0), (byte)(_b.Value ?? 0)));
    }

    private void FromHex()
    {
        if (!_updating && ColorMath.TryParseHex(_hex.Text, out SKColor c)) Set(c);
    }

    private void PlaceMarkers()
    {
        Size field = _field.Bounds.Size;
        _fieldMarker.Children.Clear();
        Avalonia.Controls.Shapes.Ellipse ring = new() { Width = 10, Height = 10, Stroke = _v > 0.55f && _s < 0.5f ? Brushes.Black : Brushes.White, StrokeThickness = 1.5 };
        Canvas.SetLeft(ring, _s * field.Width - 5);
        Canvas.SetTop(ring, (1 - _v) * field.Height - 5);
        _fieldMarker.Children.Add(ring);
        _hueMarker.Children.Clear();
        double y = (1 - _h / 360) * _hue.Bounds.Height;
        Avalonia.Controls.Shapes.Path left = new() { Data = Geometry.Parse("M0,0 L4,3 L0,6 Z"), Fill = Brushes.White };
        Canvas.SetLeft(left, -4);
        Canvas.SetTop(left, y - 3);
        Avalonia.Controls.Shapes.Path right = new() { Data = Geometry.Parse("M4,0 L0,3 L4,6 Z"), Fill = Brushes.White };
        Canvas.SetLeft(right, 14);
        Canvas.SetTop(right, y - 3);
        _hueMarker.Children.Add(left);
        _hueMarker.Children.Add(right);
    }

    public override void Refresh()
    {
        _updating = true;
        SKColor c = Current;
        _fg.Background = DialogBuilder.ToBrush(Editor.Foreground.WithAlpha(255));
        _bg.Background = DialogBuilder.ToBrush(Editor.Background.WithAlpha(255));
        IBrush active = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0)), inactive = new SolidColorBrush(Color.FromRgb(0x70, 0x70, 0x70));
        _fg.BorderBrush = _editBackground ? inactive : active;
        _bg.BorderBrush = _editBackground ? active : inactive;
        _bg.ZIndex = _editBackground ? 1 : 0;
        ColorMath.RgbToHsv(c.Red / 255f, c.Green / 255f, c.Blue / 255f, out float h, out float s, out float v);
        if (s > 0.001f && v > 0.001f) _h = h;
        _s = s;
        _v = v;
        SKColor pure = ColorMath.FromHsv(_h, 1, 1);
        _field.Background = new SolidColorBrush(Color.FromRgb(pure.Red, pure.Green, pure.Blue));
        _r.Value = c.Red;
        _g.Value = c.Green;
        _b.Value = c.Blue;
        if (!_hex.IsFocused) _hex.Text = ColorMath.ToHex(c);
        PlaceMarkers();
        _updating = false;
    }
}

public sealed class SwatchesPanel : EditorPanel
{
    private readonly WrapPanel _wrap = new() { Margin = new Thickness(6) };

    public static readonly string[] DefaultSwatches =
    [
        "#000000", "#404040", "#808080", "#C0C0C0", "#FFFFFF", "#FF0000", "#FF8000", "#FFFF00", "#80FF00", "#00FF00", "#00FF80", "#00FFFF",
        "#0080FF", "#0000FF", "#8000FF", "#FF00FF", "#FF0080", "#800000", "#804000", "#808000", "#408000", "#008000", "#008040", "#008080",
        "#004080", "#000080", "#400080", "#800080", "#800040", "#F4CCCC", "#FCE5CD", "#FFF2CC", "#D9EAD3", "#D0E0E3", "#CFE2F3", "#D9D2E9",
        "#EAD1DC", "#E06666", "#F6B26B", "#FFD966", "#93C47D", "#76A5AF", "#6FA8DC", "#8E7CC3", "#C27BA0"
    ];

    public SwatchesPanel(PhotoEditorWindow editor) : base(editor)
    {
        StackPanel footer = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(4) };
        footer.Children.Add(IconButton("plus", "Create new swatch of foreground color", () =>
        {
            Editor.Settings.Swatches.Add("#" + ColorMath.ToHex(Editor.Foreground));
            Refresh();
        }));
        DockPanel root = new();
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(footer);
        root.Children.Add(new ScrollViewer { Content = _wrap });
        Content = root;
    }

    public override string Id => "swatches";

    public override string Title => "Swatches";

    public override void Refresh()
    {
        _wrap.Children.Clear();
        if (Editor.Settings.Swatches.Count == 0) Editor.Settings.Swatches.AddRange(DefaultSwatches);
        foreach (string hex in Editor.Settings.Swatches.ToList())
        {
            if (!ColorMath.TryParseHex(hex, out SKColor color)) continue;
            Border swatch = new() { Width = 18, Height = 18, Margin = new Thickness(1), Background = DialogBuilder.ToBrush(color), BorderBrush = Brushes.Black, BorderThickness = new Thickness(0.5), Cursor = new Cursor(StandardCursorType.Hand) };
            ToolTip.SetTip(swatch, $"#{ColorMath.ToHex(color)} — click: foreground, Alt/Ctrl-click: background, right-click: delete");
            swatch.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(swatch).Properties.IsRightButtonPressed)
                {
                    Editor.Settings.Swatches.Remove(hex);
                    Refresh();
                }
                else if (e.KeyModifiers.HasFlag(KeyModifiers.Alt) || e.KeyModifiers.HasFlag(KeyModifiers.Control)) Editor.Background = color;
                else Editor.Foreground = color;
            };
            _wrap.Children.Add(swatch);
        }
    }
}

public sealed class AdjustmentsPanel : EditorPanel
{
    public AdjustmentsPanel(PhotoEditorWindow editor) : base(editor)
    {
        // Icon grid of adjustment presets, grouped like the reference panel.
        AdjustmentKind[][] rows =
        [
            [AdjustmentKind.BrightnessContrast, AdjustmentKind.Levels, AdjustmentKind.Curves, AdjustmentKind.Exposure],
            [AdjustmentKind.Vibrance, AdjustmentKind.HueSaturation, AdjustmentKind.ColorBalance, AdjustmentKind.BlackAndWhite, AdjustmentKind.PhotoFilter, AdjustmentKind.ChannelMixer],
            [AdjustmentKind.Invert, AdjustmentKind.Posterize, AdjustmentKind.Threshold, AdjustmentKind.GradientMap, AdjustmentKind.SelectiveColor, AdjustmentKind.Desaturate]
        ];
        TextBlock hint = new() { Text = "Add an adjustment", Margin = new Thickness(10, 8, 10, 4), Classes = { "dim" } };
        StackPanel root = new() { Spacing = 2 };
        root.Children.Add(hint);
        foreach (AdjustmentKind[] row in rows)
        {
            StackPanel line = new() { Orientation = Orientation.Horizontal, Spacing = 2, Margin = new Thickness(8, 0) };
            foreach (AdjustmentKind kind in row)
            {
                Button button = new()
                {
                    Classes = { "icon" },
                    Content = ToolIcons.Create("adj-" + kind.ToString().ToLowerInvariant(), 18),
                    Width = 30,
                    Height = 30,
                    Padding = new Thickness(0),
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                string name = AdjustmentSettings.GetName(kind);
                button.PointerEntered += (_, _) => hint.Text = name;
                button.PointerExited += (_, _) => hint.Text = "Add an adjustment";
                ToolTip.SetTip(button, $"Create a new {name} adjustment layer");
                AdjustmentKind captured = kind;
                button.Click += (_, _) => EditorCommands.NewAdjustmentLayer(Editor, captured);
                line.Children.Add(button);
            }
            root.Children.Add(line);
        }
        Content = new ScrollViewer { Content = root, HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled };
    }

    public override string Id => "adjustments";

    public override string Title => "Adjustments";

    public override void Refresh()
    {
    }
}

public sealed class ChannelsPanel : EditorPanel
{
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B));
    private readonly StackPanel _list = new();

    public ChannelsPanel(PhotoEditorWindow editor) : base(editor)
    {
        StackPanel footer = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 2, Margin = new Thickness(4) };
        footer.Children.Add(IconButton("selection", "Load channel as selection", () => EditorCommands.LoadChannelSelection(Editor)));
        footer.Children.Add(IconButton("mask", "Save selection as channel", () => EditorCommands.SaveSelectionToChannel(Editor)));
        footer.Children.Add(IconButton("new-layer", "Create new channel", () => EditorCommands.NewChannel(Editor)));
        footer.Children.Add(IconButton("trash", "Delete current channel", () => EditorCommands.DeleteChannel(Editor)));
        DockPanel root = new();
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(footer);
        root.Children.Add(new ScrollViewer { Content = _list });
        Content = root;
    }

    /// <summary>Grayscale thumbnail of one color channel (or the composite).</summary>
    private static unsafe SKBitmap ChannelThumbnail(SKBitmap source, ChannelView view)
    {
        SKBitmap result = ImageOps.CreateBitmap(source.Width, source.Height);
        uint* s = (uint*)source.GetPixels(), d = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                uint p = s[y * stride + x];
                if (view == ChannelView.Composite)
                {
                    d[y * stride + x] = p | 0xFF000000u;
                    continue;
                }
                uint v = view switch { ChannelView.Red => (p >> 16) & 0xFF, ChannelView.Green => (p >> 8) & 0xFF, _ => p & 0xFF };
                d[y * stride + x] = 0xFF000000u | (v << 16) | (v << 8) | v;
            }
        }
        return result;
    }

    public override string Id => "channels";

    public override string Title => "Channels";

    public override void Refresh()
    {
        _list.Children.Clear();
        PhotoDocument? doc = Doc;
        if (doc == null) return;
        (ChannelView View, string Name, string Key)[] colorChannels = [(ChannelView.Composite, "RGB", "Ctrl+2"), (ChannelView.Red, "Red", "Ctrl+3"), (ChannelView.Green, "Green", "Ctrl+4"), (ChannelView.Blue, "Blue", "Ctrl+5")];
        Editor.View?.Update(true);
        SKBitmap? composite = Editor.View?.Composite;
        using SKBitmap? small = composite != null ? ImageOps.Resize(composite, Math.Max(1, (int)(30f * composite.Width / Math.Max(composite.Width, composite.Height))), Math.Max(1, (int)(30f * composite.Height / Math.Max(composite.Width, composite.Height))), ResampleMethod.Bilinear) : null;
        foreach (var (view, name, key) in colorChannels)
        {
            // In composite view every color channel is active, like the reference panel.
            bool selected = doc.ActiveChannel == null && (doc.ChannelView == view || doc.ChannelView == ChannelView.Composite);
            Grid content = new() { ColumnDefinitions = new ColumnDefinitions("28,Auto,*,Auto") };
            Border eye = new() { Width = 28, BorderBrush = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24)), BorderThickness = new Thickness(0, 0, 1, 0), Child = doc.ChannelView == view || doc.ChannelView == ChannelView.Composite ? ToolIcons.Create("eye", 15) : null };
            content.Children.Add(eye);
            Control thumb = small != null ? new Border { Width = 32, Height = 32, BorderBrush = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A)), BorderThickness = new Thickness(1), Margin = new Thickness(6, 0, 0, 0), Child = new Image { Source = BitmapHelper.ToAvalonia(ChannelThumbnail(small, view)) } } : new Border();
            Grid.SetColumn(thumb, 1);
            content.Children.Add(thumb);
            TextBlock label = Text(name);
            label.Margin = new Thickness(8, 0, 0, 0);
            Grid.SetColumn(label, 2);
            content.Children.Add(label);
            TextBlock shortcut = new() { Text = key, Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0) };
            Grid.SetColumn(shortcut, 3);
            content.Children.Add(shortcut);
            Border row = new() { Height = 38, Background = selected ? SelectedBrush : Brushes.Transparent, BorderBrush = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24)), BorderThickness = new Thickness(0, 0, 0, 1), Child = content };
            row.PointerPressed += (_, _) =>
            {
                doc.ActiveChannel = null;
                EditorCommands.SetChannelView(Editor, view);
            };
            _list.Children.Add(row);
        }
        if (doc.QuickMask) _list.Children.Add(new Border { Padding = new Thickness(6, 4), Background = SelectedBrush, Child = Text("Quick Mask", 12, true) });
        foreach (AlphaChannel channel in doc.Channels)
        {
            CheckBox visible = new() { IsChecked = channel.Visible };
            ToolTip.SetTip(visible, "Show the channel as an overlay");
            visible.IsCheckedChanged += (_, _) =>
            {
                channel.Visible = visible.IsChecked == true;
                Editor.InvalidateCanvas();
            };
            Border row = new() { Padding = new Thickness(6, 4), Background = doc.ActiveChannel == channel ? SelectedBrush : Brushes.Transparent, Child = Row(visible, Text(channel.Name)) };
            row.PointerPressed += async (_, e) =>
            {
                if (e.Source is CheckBox) return;
                if (e.ClickCount >= 2)
                {
                    string? name = await DialogBuilder.PromptAsync(Editor, "Channel Options", "Name:", channel.Name);
                    if (!string.IsNullOrWhiteSpace(name)) { channel.Name = name; Editor.Commit("Channel Options"); }
                    return;
                }
                if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    doc.SetSelection(channel.Mask.Clone());
                    Editor.Commit("Load Selection");
                    return;
                }
                doc.ActiveChannel = doc.ActiveChannel == channel ? null : channel;
                if (doc.ActiveChannel != null) channel.Visible = true;
                Editor.RefreshPanels();
                Editor.InvalidateCanvas();
            };
            _list.Children.Add(row);
        }
    }
}

public sealed class PathsPanel : EditorPanel
{
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B));
    private readonly StackPanel _list = new();

    public PathsPanel(PhotoEditorWindow editor) : base(editor)
    {
        StackPanel footer = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 2, Margin = new Thickness(4) };
        footer.Children.Add(IconButton("path-fill", "Fill path with foreground color", () => EditorCommands.FillPath(Editor)));
        footer.Children.Add(IconButton("path-stroke", "Stroke path with brush", () => EditorCommands.StrokePath(Editor)));
        footer.Children.Add(IconButton("selection", "Load path as a selection", () => EditorCommands.PathToSelection(Editor)));
        footer.Children.Add(IconButton("pen", "Make work path from selection", () => EditorCommands.SelectionToPath(Editor)));
        footer.Children.Add(IconButton("mask", "Add a vector mask (shape layer from path)", () => EditorCommands.PathToShape(Editor)));
        footer.Children.Add(IconButton("new-layer", "Create new path", () => EditorCommands.NewPath(Editor)));
        footer.Children.Add(IconButton("trash", "Delete current path", () => EditorCommands.DeletePath(Editor)));
        DockPanel root = new();
        DockPanel.SetDock(footer, Dock.Bottom);
        root.Children.Add(footer);
        root.Children.Add(new ScrollViewer { Content = _list });
        Content = root;
    }

    public override string Id => "paths";

    public override string Title => "Paths";

    public override void Refresh()
    {
        _list.Children.Clear();
        PhotoDocument? doc = Doc;
        if (doc == null) return;
        if (doc.WorkPath != null)
        {
            bool selected = doc.ActivePath?.Path == doc.WorkPath;
            Border row = new() { Padding = new Thickness(6, 4), Background = selected ? SelectedBrush : Brushes.Transparent, Child = new TextBlock { Text = "Work Path", FontStyle = FontStyle.Italic } };
            row.PointerPressed += async (_, e) =>
            {
                if (e.ClickCount >= 2)
                {
                    string? name = await DialogBuilder.PromptAsync(Editor, "Save Path", "Name:", $"Path {doc.Paths.Count + 1}");
                    if (string.IsNullOrWhiteSpace(name)) return;
                    NamedPath saved = new(name, doc.WorkPath!);
                    doc.Paths.Add(saved);
                    doc.WorkPath = null;
                    doc.ActivePath = saved;
                    Editor.Commit("Save Path");
                    return;
                }
                doc.ActivePath = selected ? null : new NamedPath("Work Path", doc.WorkPath!);
                Editor.RefreshPanels();
                Editor.InvalidateOverlay();
            };
            _list.Children.Add(row);
        }
        foreach (NamedPath path in doc.Paths)
        {
            bool selected = doc.ActivePath == path;
            Border row = new() { Padding = new Thickness(6, 4), Background = selected ? SelectedBrush : Brushes.Transparent, Child = Text(path.Name) };
            row.PointerPressed += async (_, e) =>
            {
                if (e.ClickCount >= 2)
                {
                    string? name = await DialogBuilder.PromptAsync(Editor, "Rename Path", "Name:", path.Name);
                    if (!string.IsNullOrWhiteSpace(name)) { path.Name = name; Editor.Commit("Rename Path"); }
                    return;
                }
                if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    doc.ActivePath = path;
                    EditorCommands.PathToSelection(Editor);
                    return;
                }
                doc.ActivePath = selected ? null : path;
                Editor.RefreshPanels();
                Editor.InvalidateOverlay();
            };
            _list.Children.Add(row);
        }
        if (doc.ActiveLayer is ShapeLayer shape)
        {
            _list.Children.Add(new Border { Padding = new Thickness(6, 4), Child = new TextBlock { Text = $"{shape.Name} Shape Path", FontStyle = FontStyle.Italic, Opacity = 0.8 } });
        }
    }
}

public sealed class BrushesPanel : EditorPanel
{
    private readonly StackPanel _root = new() { Spacing = 6, Margin = new Thickness(8) };

    public BrushesPanel(PhotoEditorWindow editor) : base(editor)
    {
        Content = new ScrollViewer { Content = _root };
    }

    public override string Id => "brushes";

    public override string Title => "Brushes";

    public override void Refresh()
    {
        _root.Children.Clear();
        if (Editor.Tools.Active is not BrushToolBase tool)
        {
            _root.Children.Add(new TextBlock { Text = "Select a painting tool to edit its brush.", TextWrapping = TextWrapping.Wrap });
            ListBox presets = PresetList(null);
            _root.Children.Add(presets);
            return;
        }
        _root.Children.Add(Text(tool.Name, 12, true));
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Size", DialogBuilder.SliderNumber(1, 2500, tool.Size, v => { tool.Size = (float)v; Editor.RefreshOptions(); Editor.InvalidateOverlay(); }));
        DialogBuilder.AddRow(form, "Hardness", DialogBuilder.SliderNumber(0, 100, tool.Tip.Hardness * 100, v => tool.Tip.Hardness = (float)v / 100));
        DialogBuilder.AddRow(form, "Spacing", DialogBuilder.SliderNumber(1, 1000, tool.Tip.Spacing * 100, v => tool.Tip.Spacing = (float)v / 100));
        DialogBuilder.AddRow(form, "Angle", DialogBuilder.SliderNumber(-180, 180, tool.Tip.Angle, v => { tool.Tip.Angle = (float)v; Editor.InvalidateOverlay(); }));
        DialogBuilder.AddRow(form, "Roundness", DialogBuilder.SliderNumber(5, 100, tool.Tip.Roundness * 100, v => { tool.Tip.Roundness = (float)v / 100; Editor.InvalidateOverlay(); }));
        DialogBuilder.AddRow(form, "Size Jitter", DialogBuilder.SliderNumber(0, 100, tool.Tip.SizeJitter * 100, v => tool.Tip.SizeJitter = (float)v / 100));
        DialogBuilder.AddRow(form, "Scatter", DialogBuilder.SliderNumber(0, 1000, tool.Tip.Scatter * 100, v => tool.Tip.Scatter = (float)v / 100));
        _root.Children.Add(form);
        CheckBox square = new() { Content = "Square tip", IsChecked = tool.Tip.Shape == BrushTipShape.Square };
        square.IsCheckedChanged += (_, _) => tool.Tip.Shape = square.IsChecked == true ? BrushTipShape.Square : BrushTipShape.Round;
        _root.Children.Add(square);
        CheckBox pressureSize = new() { Content = "Pen pressure controls size", IsChecked = tool.Tip.PressureSize };
        pressureSize.IsCheckedChanged += (_, _) => tool.Tip.PressureSize = pressureSize.IsChecked == true;
        CheckBox pressureOpacity = new() { Content = "Pen pressure controls flow", IsChecked = tool.Tip.PressureOpacity };
        pressureOpacity.IsCheckedChanged += (_, _) => tool.Tip.PressureOpacity = pressureOpacity.IsChecked == true;
        _root.Children.Add(pressureSize);
        _root.Children.Add(pressureOpacity);
        Button save = new() { Content = "New Brush Preset..." };
        save.Click += async (_, _) =>
        {
            string? name = await DialogBuilder.PromptAsync(Editor, "Brush Name", "Name:", $"{tool.Tip.Name} {tool.Size:0}");
            if (string.IsNullOrWhiteSpace(name)) return;
            BrushPreset preset = tool.Tip.Clone();
            preset.Name = name;
            preset.Size = tool.Size;
            Editor.BrushPresets.Add(preset);
            Refresh();
        };
        _root.Children.Add(save);
        _root.Children.Add(PresetList(tool));
    }

    private ListBox PresetList(BrushToolBase? tool)
    {
        ListBox list = new() { MaxHeight = 260 };
        foreach (BrushPreset preset in Editor.BrushPresets) list.Items.Add(new ListBoxItem { Content = $"{preset.Name} ({preset.Size:0} px)", Tag = preset });
        list.SelectionChanged += (_, _) =>
        {
            if (tool == null || list.SelectedItem is not ListBoxItem { Tag: BrushPreset preset }) return;
            tool.Tip = preset.Clone();
            tool.Size = preset.Size;
            Editor.RefreshOptions();
            Refresh();
        };
        return list;
    }
}

public sealed class NavigatorPanel : EditorPanel
{
    private readonly Image _image = new() { Stretch = Stretch.Uniform };
    private readonly Canvas _overlay = new();
    private readonly Border _viewRect = new() { BorderBrush = Brushes.Red, BorderThickness = new Thickness(2), IsHitTestVisible = false };
    private readonly Slider _zoom = new() { Minimum = -6.6, Maximum = 5, Width = 160 };
    private readonly TextBlock _zoomText = new() { Width = 56, VerticalAlignment = VerticalAlignment.Center };
    private readonly Grid _host = new() { Height = 170 };
    private bool _updating;

    public NavigatorPanel(PhotoEditorWindow editor) : base(editor)
    {
        _overlay.Children.Add(_viewRect);
        _host.Children.Add(_image);
        _host.Children.Add(_overlay);
        _host.PointerPressed += (_, e) => PanTo(e.GetPosition(_image));
        _host.PointerMoved += (_, e) => { if (e.GetCurrentPoint(_host).Properties.IsLeftButtonPressed) PanTo(e.GetPosition(_image)); };
        _image.SizeChanged += (_, _) => UpdateRect();
        _zoom.ValueChanged += (_, e) =>
        {
            if (_updating || Editor.View == null) return;
            Editor.View.ZoomAt(Math.Pow(2, e.NewValue), new Point(Editor.View.Viewport.Width / 2, Editor.View.Viewport.Height / 2));
            Editor.InvalidateOverlay();
            UpdateRect();
        };
        StackPanel root = new() { Margin = new Thickness(6), Spacing = 6 };
        root.Children.Add(_host);
        root.Children.Add(Row(_zoomText, _zoom));
        Content = root;
    }

    public override string Id => "navigator";

    public override string Title => "Navigator";

    private void PanTo(Point p)
    {
        DocumentView? view = Editor.View;
        if (view == null || _image.Bounds.Width <= 0) return;
        double scale = Math.Min(_image.Bounds.Width / view.Document.Width, _image.Bounds.Height / view.Document.Height);
        double ox = (_image.Bounds.Width - view.Document.Width * scale) / 2, oy = (_image.Bounds.Height - view.Document.Height * scale) / 2;
        SKPoint target = new((float)((p.X - ox) / scale), (float)((p.Y - oy) / scale));
        Point screen = view.DocToScreen(target);
        view.Pan += new Point(view.Viewport.Width / 2, view.Viewport.Height / 2) - screen;
        Editor.InvalidateOverlay();
        UpdateRect();
    }

    public override void Refresh()
    {
        DocumentView? view = Editor.View;
        if (view?.Composite == null)
        {
            _image.Source = null;
            return;
        }
        float scale = Math.Min(240f / view.Document.Width, 170f / view.Document.Height);
        using SKBitmap thumb = ImageOps.Resize(view.Composite, Math.Max(1, (int)(view.Document.Width * scale)), Math.Max(1, (int)(view.Document.Height * scale)), ResampleMethod.Bilinear);
        _image.Source = BitmapHelper.ToAvalonia(thumb);
        UpdateRect();
    }

    public void UpdateRect()
    {
        DocumentView? view = Editor.View;
        if (view == null || _image.Bounds.Width <= 0) return;
        _updating = true;
        _zoom.Value = Math.Log2(view.Zoom);
        _zoomText.Text = $"{view.Zoom * 100:0.##}%";
        _updating = false;
        double scale = Math.Min(_image.Bounds.Width / view.Document.Width, _image.Bounds.Height / view.Document.Height);
        double ox = _image.Bounds.X + (_image.Bounds.Width - view.Document.Width * scale) / 2, oy = _image.Bounds.Y + (_image.Bounds.Height - view.Document.Height * scale) / 2;
        SKPoint a = view.ScreenToDoc(new Point(0, 0)), b = view.ScreenToDoc(new Point(view.Viewport.Width, view.Viewport.Height));
        Canvas.SetLeft(_viewRect, ox + Math.Min(a.X, b.X) * scale);
        Canvas.SetTop(_viewRect, oy + Math.Min(a.Y, b.Y) * scale);
        _viewRect.Width = Math.Abs(b.X - a.X) * scale;
        _viewRect.Height = Math.Abs(b.Y - a.Y) * scale;
    }
}

public sealed class InfoPanel : EditorPanel
{
    private static readonly IBrush DividerBrush = new SolidColorBrush(Color.FromRgb(0x47, 0x47, 0x47));

    private readonly Dictionary<string, TextBlock> _values = [];
    private readonly StackPanel _samplers = new() { Spacing = 2 };
    private readonly TextBlock _doc = new() { Classes = { "dim" }, FontSize = 11, TextWrapping = TextWrapping.Wrap };

    public InfoPanel(PhotoEditorWindow editor) : base(editor)
    {
        Grid quadrants = new() { ColumnDefinitions = new ColumnDefinitions("*,1,*"), RowDefinitions = new RowDefinitions("Auto,1,Auto") };
        AddCell(quadrants, 0, 0, "eyedropper", ["R", "G", "B", "A"]);
        AddCell(quadrants, 0, 2, null, ["H", "S", "Br", "#"]);
        AddCell(quadrants, 2, 0, "move", ["X", "Y"]);
        AddCell(quadrants, 2, 2, "rectangularmarquee", ["W", "H2"]);
        Border vertical = new() { Background = DividerBrush, Margin = new Thickness(0, 4) };
        Grid.SetColumn(vertical, 1);
        Grid.SetRowSpan(vertical, 3);
        Border horizontal = new() { Background = DividerBrush };
        Grid.SetRow(horizontal, 1);
        Grid.SetColumnSpan(horizontal, 3);
        quadrants.Children.Add(vertical);
        quadrants.Children.Add(horizontal);

        StackPanel root = new() { Margin = new Thickness(8, 6), Spacing = 8 };
        root.Children.Add(quadrants);
        root.Children.Add(_samplers);
        root.Children.Add(new Border { Height = 1, Background = DividerBrush });
        root.Children.Add(_doc);
        Content = new ScrollViewer { Content = root, HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled };
    }

    public override string Id => "info";

    public override string Title => "Info";

    private void AddCell(Grid parent, int row, int column, string? icon, string[] keys)
    {
        Grid cell = new() { ColumnDefinitions = new ColumnDefinitions("20,18,*"), Margin = new Thickness(column == 0 ? 0 : 8, 5, 4, 5) };
        for (int i = 0; i < keys.Length; i++)
        {
            cell.RowDefinitions.Add(new RowDefinition(new GridLength(17)));
            TextBlock label = new() { Text = keys[i].TrimEnd('2') switch { "Br" => "B", var k => k } + (keys[i] == "#" ? string.Empty : ":"), Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center };
            TextBlock value = new() { Text = string.Empty, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left };
            Grid.SetRow(label, i);
            Grid.SetColumn(label, 1);
            Grid.SetRow(value, i);
            Grid.SetColumn(value, 2);
            cell.Children.Add(label);
            cell.Children.Add(value);
            _values[keys[i]] = value;
        }
        if (icon != null)
        {
            Control image = ToolIcons.Create(icon, 14);
            image.VerticalAlignment = VerticalAlignment.Top;
            image.Margin = new Thickness(0, 1, 0, 0);
            image.Opacity = 0.8;
            Grid.SetRowSpan(image, keys.Length);
            cell.Children.Add(image);
        }
        Grid.SetRow(cell, row);
        Grid.SetColumn(cell, column);
        parent.Children.Add(cell);
    }

    private void Set(string key, string text) => _values[key].Text = text;

    public override void Refresh()
    {
        _samplers.Children.Clear();
        DocumentView? view = Editor.View;
        if (view?.Composite == null)
        {
            foreach (TextBlock value in _values.Values) value.Text = string.Empty;
            _doc.Text = string.Empty;
            return;
        }
        PhotoDocument doc = view.Document;
        SKPoint p = Editor.Canvas.LastDocPoint;
        int x = (int)MathF.Floor(p.X), y = (int)MathF.Floor(p.Y);
        if (x >= 0 && y >= 0 && x < doc.Width && y < doc.Height)
        {
            SKColor c = ColorMath.ToColor(ImageOps.GetPixel(view.Composite, x, y));
            ColorMath.RgbToHsv(c.Red / 255f, c.Green / 255f, c.Blue / 255f, out float h, out float s, out float v);
            Set("R", c.Red.ToString());
            Set("G", c.Green.ToString());
            Set("B", c.Blue.ToString());
            Set("A", c.Alpha.ToString());
            Set("H", $"{h:0}°");
            Set("S", $"{s * 100:0}%");
            Set("Br", $"{v * 100:0}%");
            Set("#", ColorMath.ToHex(c));
        }
        else
        {
            foreach (string key in new[] { "R", "G", "B", "A", "H", "S", "Br", "#" }) Set(key, string.Empty);
        }
        Set("X", $"{p.X:0.0}");
        Set("Y", $"{p.Y:0.0}");
        SKRectI sel = doc.SelectionBounds;
        Set("W", doc.Selection != null ? sel.Width.ToString() : string.Empty);
        Set("H2", doc.Selection != null ? sel.Height.ToString() : string.Empty);
        for (int i = 0; i < doc.Samplers.Count; i++)
        {
            ColorSamplerPoint sampler = doc.Samplers[i];
            SKColor c = ImageOps.SampleAverage(view.Composite, (int)sampler.Position.X, (int)sampler.Position.Y, 1);
            StackPanel row = new() { Orientation = Orientation.Horizontal, Spacing = 10 };
            row.Children.Add(new TextBlock { Text = $"#{i + 1}", Classes = { "dim" }, Width = 20 });
            row.Children.Add(new Border { Width = 12, Height = 12, Background = new SolidColorBrush(Color.FromRgb(c.Red, c.Green, c.Blue)), BorderBrush = DividerBrush, BorderThickness = new Thickness(1) });
            row.Children.Add(new TextBlock { Text = $"R: {c.Red}   G: {c.Green}   B: {c.Blue}" });
            _samplers.Children.Add(row);
        }
        _samplers.IsVisible = _samplers.Children.Count > 0;
        _doc.Text = $"Doc: {doc.Width} × {doc.Height} px @ {doc.Resolution:0} ppi\nLayers: {doc.AllLayers.Count()}   History states: {doc.History.Entries.Count}";
    }
}
