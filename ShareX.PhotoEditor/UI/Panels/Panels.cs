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
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromArgb(110, 62, 131, 242));
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

    public override void Refresh()
    {
        _list.Children.Clear();
        PhotoDocument? doc = Doc;
        if (doc == null) return;
        History history = doc.History;
        foreach (HistoryEntry snapshot in history.Snapshots) _list.Children.Add(CreateRow(doc, snapshot, -1, true));
        if (history.Snapshots.Count > 0) _list.Children.Add(new Border { Height = 1, Background = Brushes.Gray, Margin = new Thickness(0, 2) });
        for (int i = 0; i < history.Entries.Count; i++) _list.Children.Add(CreateRow(doc, history.Entries[i], i, false));
    }

    private Control CreateRow(PhotoDocument doc, HistoryEntry entry, int index, bool snapshot)
    {
        History history = doc.History;
        RadioButton source = new() { IsChecked = history.Source == entry, GroupName = "historySource", VerticalAlignment = VerticalAlignment.Center };
        ToolTip.SetTip(source, "Sets the source for the history brush");
        source.IsCheckedChanged += (_, _) =>
        {
            if (source.IsChecked == true) history.Source = entry;
        };
        TextBlock name = Text(entry.Name);
        if (!snapshot && index > history.Index) name.Opacity = 0.45;
        Border row = new()
        {
            Padding = new Thickness(4, 2),
            Background = !snapshot && index == history.Index ? SelectedBrush : Brushes.Transparent,
            Child = Row(source, snapshot ? ToolIcons.Create("snapshot", 14) : ToolIcons.Create("brush-tip", 8), name)
        };
        row.PointerPressed += (_, e) =>
        {
            if (e.Source is RadioButton) return;
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
    private readonly Slider _r = Slider255(), _g = Slider255(), _b = Slider255(), _h = new() { Minimum = 0, Maximum = 360 }, _s = Slider100(), _v = Slider100();
    private readonly TextBox _hex = new() { Width = 80 };
    private readonly Border _fg = new() { Width = 28, Height = 28, BorderBrush = Brushes.White, BorderThickness = new Thickness(1) };
    private readonly Border _bg = new() { Width = 28, Height = 28, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
    private readonly Border _ramp = new() { Height = 24, Cursor = new Cursor(StandardCursorType.Cross) };
    private bool _editBackground;
    private bool _updating;

    private static Slider Slider255() => new() { Minimum = 0, Maximum = 255 };
    private static Slider Slider100() => new() { Minimum = 0, Maximum = 100 };

    public ColorPanel(PhotoEditorWindow editor) : base(editor)
    {
        _fg.PointerPressed += (_, _) => { _editBackground = false; Refresh(); };
        _bg.PointerPressed += (_, _) => { _editBackground = true; Refresh(); };
        _fg.DoubleTapped += async (_, _) => { SKColor? c = await ColorPickerDialog.ShowAsync(Editor, Editor.Foreground, "Foreground Color", Editor); if (c is SKColor col) Editor.Foreground = col; };
        _bg.DoubleTapped += async (_, _) => { SKColor? c = await ColorPickerDialog.ShowAsync(Editor, Editor.Background, "Background Color", Editor); if (c is SKColor col) Editor.Background = col; };
        foreach (Slider s in new[] { _r, _g, _b }) s.ValueChanged += (_, _) => FromRgb();
        foreach (Slider s in new[] { _h, _s, _v }) s.ValueChanged += (_, _) => FromHsv();
        _hex.LostFocus += (_, _) => FromHex();
        _hex.KeyDown += (_, e) => { if (e.Key == Key.Enter) FromHex(); };

        LinearGradientBrush spectrum = new() { StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative) };
        for (int i = 0; i <= 6; i++)
        {
            SKColor c = ColorMath.FromHsv(i * 60, 1, 1);
            spectrum.GradientStops.Add(new Avalonia.Media.GradientStop(Color.FromRgb(c.Red, c.Green, c.Blue), i / 6.0));
        }
        _ramp.Background = spectrum;
        _ramp.PointerPressed += (_, e) => PickRamp(e.GetPosition(_ramp));
        _ramp.PointerMoved += (_, e) => { if (e.GetCurrentPoint(_ramp).Properties.IsLeftButtonPressed) PickRamp(e.GetPosition(_ramp)); };

        Grid grid = DialogBuilder.Form();
        DialogBuilder.AddRow(grid, "R", _r);
        DialogBuilder.AddRow(grid, "G", _g);
        DialogBuilder.AddRow(grid, "B", _b);
        DialogBuilder.AddRow(grid, "H", _h);
        DialogBuilder.AddRow(grid, "S", _s);
        DialogBuilder.AddRow(grid, "B", _v);
        DialogBuilder.AddRow(grid, "#", _hex);
        Canvas swatches = new() { Width = 48, Height = 44 };
        Canvas.SetLeft(_bg, 18);
        Canvas.SetTop(_bg, 14);
        swatches.Children.Add(_bg);
        swatches.Children.Add(_fg);
        StackPanel root = new() { Spacing = 6, Margin = new Thickness(8) };
        root.Children.Add(Row(swatches, Text("Click a swatch to edit it; double-click opens the Color Picker.", 11)));
        root.Children.Add(grid);
        root.Children.Add(_ramp);
        Content = new ScrollViewer { Content = root };
    }

    public override string Id => "color";

    public override string Title => "Color";

    private SKColor Current => _editBackground ? Editor.Background : Editor.Foreground;

    private void Set(SKColor color)
    {
        if (_editBackground) Editor.Background = color;
        else Editor.Foreground = color;
    }

    private void PickRamp(Point p)
    {
        float hue = (float)Math.Clamp(p.X / Math.Max(1, _ramp.Bounds.Width), 0, 1) * 360;
        Set(ColorMath.FromHsv(hue, 1, 1));
    }

    private void FromRgb()
    {
        if (_updating) return;
        Set(new SKColor((byte)_r.Value, (byte)_g.Value, (byte)_b.Value));
    }

    private void FromHsv()
    {
        if (_updating) return;
        Set(ColorMath.FromHsv((float)_h.Value, (float)_s.Value / 100, (float)_v.Value / 100));
    }

    private void FromHex()
    {
        if (!_updating && ColorMath.TryParseHex(_hex.Text, out SKColor c)) Set(c);
    }

    public override void Refresh()
    {
        _updating = true;
        SKColor c = Current;
        _fg.Background = DialogBuilder.ToBrush(Editor.Foreground);
        _bg.Background = DialogBuilder.ToBrush(Editor.Background);
        _fg.BorderBrush = _editBackground ? Brushes.Gray : Brushes.White;
        _bg.BorderBrush = _editBackground ? Brushes.White : Brushes.Gray;
        _r.Value = c.Red; _g.Value = c.Green; _b.Value = c.Blue;
        ColorMath.RgbToHsv(c.Red / 255f, c.Green / 255f, c.Blue / 255f, out float h, out float s, out float v);
        _h.Value = h; _s.Value = s * 100; _v.Value = v * 100;
        if (!_hex.IsFocused) _hex.Text = ColorMath.ToHex(c);
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
        WrapPanel wrap = new() { Margin = new Thickness(6) };
        foreach (AdjustmentKind kind in Enum.GetValues<AdjustmentKind>())
        {
            Button button = new() { Content = Text(AdjustmentSettings.GetName(kind), 11), Margin = new Thickness(2), Padding = new Thickness(6, 3) };
            ToolTip.SetTip(button, $"Add a {AdjustmentSettings.GetName(kind)} adjustment layer");
            AdjustmentKind captured = kind;
            button.Click += (_, _) => EditorCommands.NewAdjustmentLayer(Editor, captured);
            wrap.Children.Add(button);
        }
        StackPanel root = new();
        root.Children.Add(new TextBlock { Text = "Add an adjustment", Margin = new Thickness(8, 6, 8, 0), FontWeight = FontWeight.SemiBold });
        root.Children.Add(wrap);
        Content = new ScrollViewer { Content = root };
    }

    public override string Id => "adjustments";

    public override string Title => "Adjustments";

    public override void Refresh()
    {
    }
}

public sealed class ChannelsPanel : EditorPanel
{
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromArgb(110, 62, 131, 242));
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

    public override string Id => "channels";

    public override string Title => "Channels";

    public override void Refresh()
    {
        _list.Children.Clear();
        PhotoDocument? doc = Doc;
        if (doc == null) return;
        (ChannelView View, string Name, string Key)[] colorChannels = [(ChannelView.Composite, "RGB", "Ctrl+2"), (ChannelView.Red, "Red", "Ctrl+3"), (ChannelView.Green, "Green", "Ctrl+4"), (ChannelView.Blue, "Blue", "Ctrl+5")];
        foreach (var (view, name, key) in colorChannels)
        {
            bool selected = doc.ActiveChannel == null && doc.ChannelView == view;
            Border row = new() { Padding = new Thickness(6, 4), Background = selected ? SelectedBrush : Brushes.Transparent, Child = Row(ToolIcons.Create("eye", 14), Text(name), Text(key, 10)) };
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
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.FromArgb(110, 62, 131, 242));
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
    private readonly TextBlock _text = new() { FontFamily = new FontFamily("Cascadia Mono, Consolas, Courier New"), FontSize = 11, Margin = new Thickness(8) };

    public InfoPanel(PhotoEditorWindow editor) : base(editor)
    {
        Content = new ScrollViewer { Content = _text };
    }

    public override string Id => "info";

    public override string Title => "Info";

    public override void Refresh()
    {
        DocumentView? view = Editor.View;
        if (view?.Composite == null)
        {
            _text.Text = string.Empty;
            return;
        }
        PhotoDocument doc = view.Document;
        SKPoint p = Editor.Canvas.LastDocPoint;
        System.Text.StringBuilder sb = new();
        int x = (int)MathF.Floor(p.X), y = (int)MathF.Floor(p.Y);
        if (x >= 0 && y >= 0 && x < doc.Width && y < doc.Height)
        {
            SKColor c = ColorMath.ToColor(ImageOps.GetPixel(view.Composite, x, y));
            ColorMath.RgbToHsv(c.Red / 255f, c.Green / 255f, c.Blue / 255f, out float h, out float s, out float v);
            sb.AppendLine($"R: {c.Red,3}   H: {h,3:0}°");
            sb.AppendLine($"G: {c.Green,3}   S: {s * 100,3:0}%");
            sb.AppendLine($"B: {c.Blue,3}   B: {v * 100,3:0}%");
            sb.AppendLine($"A: {c.Alpha,3}   #{ColorMath.ToHex(c)}");
        }
        sb.AppendLine();
        sb.AppendLine($"X: {p.X,7:0.0}   Y: {p.Y,7:0.0}");
        SKRectI sel = doc.SelectionBounds;
        sb.AppendLine(doc.Selection != null ? $"W: {sel.Width,7}   H: {sel.Height,7}" : "W:     -     H:     -");
        for (int i = 0; i < doc.Samplers.Count; i++)
        {
            ColorSamplerPoint sampler = doc.Samplers[i];
            SKColor c = ImageOps.SampleAverage(view.Composite, (int)sampler.Position.X, (int)sampler.Position.Y, 1);
            sb.AppendLine($"#{i + 1}  R:{c.Red,3} G:{c.Green,3} B:{c.Blue,3}");
        }
        sb.AppendLine();
        sb.AppendLine($"Doc: {doc.Width} × {doc.Height} px @ {doc.Resolution:0} ppi");
        sb.AppendLine($"Layers: {doc.AllLayers.Count()}   History: {doc.History.Entries.Count}");
        _text.Text = sb.ToString();
    }
}
