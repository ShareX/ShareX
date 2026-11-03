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
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI;

/// <summary>Builds the context-sensitive options bar for the active tool.</summary>
public sealed class OptionsBuilder
{
    private readonly IEditorHost _host;

    public OptionsBuilder(IEditorHost host)
    {
        _host = host;
        Root = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, VerticalAlignment = VerticalAlignment.Center };
    }

    public StackPanel Root { get; }

    public void Add(Control control) => Root.Children.Add(control);

    public TextBlock Label(string text, string? tooltip = null)
    {
        TextBlock block = new() { Text = text, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0, 0, 0) };
        if (tooltip != null) ToolTip.SetTip(block, tooltip);
        Add(block);
        return block;
    }

    public void Separator() => Add(new Border { Classes = { "vsep" } });

    /// <summary>
    /// Numeric field with a scrubby label (drag horizontally to change the value) and, for bounded ranges,
    /// a small dropdown slider like the reference options bar.
    /// </summary>
    public NumericUpDown Number(string label, double min, double max, Func<double> get, Action<double> set, string format = "0", string? suffix = null, double increment = 1, double width = 50, string? tooltip = null)
    {
        TextBlock? caption = string.IsNullOrEmpty(label) ? null : Label(label, tooltip);
        NumericUpDown box = new()
        {
            Minimum = (decimal)min,
            Maximum = (decimal)max,
            Value = (decimal)Math.Clamp(get(), min, max),
            Increment = (decimal)increment,
            FormatString = format,
            Width = width,
            ShowButtonSpinner = false,
            VerticalAlignment = VerticalAlignment.Center
        };
        if (tooltip != null) ToolTip.SetTip(box, tooltip);
        box.ValueChanged += (_, e) =>
        {
            if (e.NewValue is decimal value) set((double)value);
        };
        if (caption != null)
        {
            caption.Cursor = new Cursor(StandardCursorType.SizeWestEast);
            Point? dragStart = null;
            double startValue = 0;
            caption.PointerPressed += (_, e) =>
            {
                dragStart = e.GetPosition(caption);
                startValue = (double)(box.Value ?? 0);
                e.Pointer.Capture(caption);
                e.Handled = true;
            };
            caption.PointerMoved += (_, e) =>
            {
                if (dragStart is not Point start) return;
                double dx = e.GetPosition(caption).X - start.X;
                double step = (max - min) > 200 ? 1 : increment;
                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift)) step *= 10;
                box.Value = (decimal)Math.Clamp(startValue + Math.Round(dx / 2) * step, min, max);
            };
            caption.PointerReleased += (_, e) =>
            {
                dragStart = null;
                e.Pointer.Capture(null);
            };
        }
        // Suffix sits tight against the field.
        StackPanel field = new() { Orientation = Orientation.Horizontal, Spacing = 2, VerticalAlignment = VerticalAlignment.Center };
        field.Children.Add(box);
        if (suffix != null) field.Children.Add(new TextBlock { Text = suffix, Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center });
        if (max - min <= 5000)
        {
            Slider slider = new() { Minimum = min, Maximum = Math.Min(max, min + 5000), Value = (double)(box.Value ?? 0), Width = 180 };
            slider.ValueChanged += (_, e) => box.Value = (decimal)Math.Round(e.NewValue / increment) * (decimal)increment;
            box.ValueChanged += (_, e) => { if (e.NewValue is decimal v && Math.Abs(slider.Value - (double)v) > 1e-6) slider.Value = (double)v; };
            Button drop = new() { Classes = { "icon" }, Content = ToolIcons.Create("chevron-down", 9), Padding = new Thickness(2, 4), VerticalAlignment = VerticalAlignment.Center };
            drop.Flyout = new Flyout { Content = slider, Placement = PlacementMode.BottomEdgeAlignedRight };
            field.Children.Add(drop);
        }
        Add(field);
        return box;
    }

    /// <summary>Percentage option backed by a 0..1 value.</summary>
    public NumericUpDown Percent(string label, Func<float> get, Action<float> set, string? tooltip = null) =>
        Number(label, 0, 100, () => get() * 100, v => set((float)(v / 100)), "0", "%", 1, 40, tooltip);

    public ComboBox Combo<T>(string? label, IEnumerable<(string Text, T Value)> items, Func<T> get, Action<T> set, double width = 120, string? tooltip = null)
    {
        if (label != null) Label(label);
        List<(string Text, T Value)> list = items.ToList();
        ComboBox combo = new() { Width = width, VerticalAlignment = VerticalAlignment.Center };
        foreach (var item in list) combo.Items.Add(new ComboBoxItem { Content = item.Text });
        T current = get();
        combo.SelectedIndex = Math.Max(0, list.FindIndex(i => Equals(i.Value, current)));
        combo.SelectionChanged += (_, _) =>
        {
            if (combo.SelectedIndex >= 0 && combo.SelectedIndex < list.Count) set(list[combo.SelectedIndex].Value);
        };
        if (tooltip != null) ToolTip.SetTip(combo, tooltip);
        Add(combo);
        return combo;
    }

    public ComboBox EnumCombo<T>(string? label, Func<T> get, Action<T> set, double width = 120, Func<T, string>? text = null) where T : struct, Enum =>
        Combo(label, Enum.GetValues<T>().Select(v => (text?.Invoke(v) ?? SplitWords(v.ToString()), v)), get, set, width);

    public static string SplitWords(string text) =>
        string.Concat(text.Select((c, i) => i > 0 && char.IsUpper(c) && !char.IsUpper(text[i - 1]) ? " " + c : c.ToString()));

    public ComboBox BlendMode(Func<LayerBlendMode> get, Action<LayerBlendMode> set, bool paintModes = true) =>
        Combo("Mode:", (paintModes ? BlendModes.PaintModes : BlendModes.LayerModes).Select(m => (BlendModes.GetName(m), m)), get, set, 120);

    public CheckBox Check(string text, Func<bool> get, Action<bool> set, string? tooltip = null)
    {
        CheckBox box = new() { Content = text, IsChecked = get(), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0, 0, 0) };
        box.IsCheckedChanged += (_, _) => set(box.IsChecked == true);
        if (tooltip != null) ToolTip.SetTip(box, tooltip);
        Add(box);
        return box;
    }

    public Button Button(string text, Action click, string? tooltip = null)
    {
        Button button = new() { Content = text, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2, 0) };
        button.Click += (_, _) => click();
        if (tooltip != null) ToolTip.SetTip(button, tooltip);
        Add(button);
        return button;
    }

    public Button IconButton(string iconKey, string tooltip, Action click, bool active = false)
    {
        Button button = new()
        {
            Classes = { "icon" },
            Content = ToolIcons.Create(iconKey, 16),
            Width = 26,
            Height = 24,
            Padding = new Thickness(0),
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        button.Classes.Set("checked", active);
        ToolTip.SetTip(button, tooltip);
        button.Click += (_, _) => click();
        Add(button);
        return button;
    }

    public TextBox Text(string? label, Func<string> get, Action<string> set, double width = 120)
    {
        if (label != null) Label(label);
        TextBox box = new() { Text = get(), Width = width, VerticalAlignment = VerticalAlignment.Center };
        box.TextChanged += (_, _) => set(box.Text ?? string.Empty);
        Add(box);
        return box;
    }

    public Button ColorButton(string tooltip, Func<SKColor> get, Action<SKColor> set, bool allowNone = false, Func<bool>? isNone = null, Action? setNone = null)
    {
        Border swatch = new() { Width = 20, Height = 20, BorderBrush = new SolidColorBrush(Color.FromRgb(0x9A, 0x9A, 0x9A)), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(2) };
        void Refresh()
        {
            SKColor c = get();
            swatch.Background = isNone?.Invoke() == true
                ? new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                    GradientStops = { new Avalonia.Media.GradientStop(Colors.White, 0.44), new Avalonia.Media.GradientStop(Color.FromRgb(0xE0, 0x30, 0x30), 0.5), new Avalonia.Media.GradientStop(Colors.White, 0.56) }
                }
                : new SolidColorBrush(Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue));
        }
        Refresh();
        Button button = new() { Classes = { "icon" }, Content = swatch, Padding = new Thickness(2), VerticalAlignment = VerticalAlignment.Center };
        ToolTip.SetTip(button, tooltip);
        button.Click += async (_, _) =>
        {
            SKColor? picked = await ColorPickerDialog.ShowAsync(_host.Window, get(), tooltip, _host);
            if (picked is SKColor color)
            {
                set(color);
                Refresh();
            }
        };
        if (allowNone && setNone != null)
        {
            button.ContextMenu = new ContextMenu();
            MenuItem none = new() { Header = "No Color" };
            none.Click += (_, _) => { setNone(); Refresh(); };
            button.ContextMenu.Items.Add(none);
        }
        Add(button);
        return button;
    }

    /// <summary>New / Add / Subtract / Intersect selection mode buttons.</summary>
    public void SelectionMode(Func<SelectionMode> get, Action<SelectionMode> set)
    {
        StackPanel group = new() { Orientation = Orientation.Horizontal, Spacing = 0, VerticalAlignment = VerticalAlignment.Center };
        List<(Button Button, SelectionMode Mode)> buttons = [];
        void Refresh()
        {
            foreach (var (button, mode) in buttons) button.Classes.Set("checked", mode == get());
        }
        foreach (var (mode, icon, tip) in new[]
        {
            (Imaging.SelectionMode.Replace, "sel-new", "New selection"),
            (Imaging.SelectionMode.Add, "sel-add", "Add to selection (Shift)"),
            (Imaging.SelectionMode.Subtract, "sel-sub", "Subtract from selection (Alt)"),
            (Imaging.SelectionMode.Intersect, "sel-int", "Intersect with selection (Shift+Alt)")
        })
        {
            Button button = new() { Classes = { "icon" }, Content = ToolIcons.Create(icon, 16), Width = 26, Height = 24, Padding = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Center };
            ToolTip.SetTip(button, tip);
            SelectionMode captured = mode;
            button.Click += (_, _) => { set(captured); Refresh(); };
            buttons.Add((button, mode));
            group.Children.Add(button);
        }
        Refresh();
        Add(group);
    }

    /// <summary>Draws a small preview of a brush tip (soft or hard round, square, sampled).</summary>
    public static Control TipPreview(BrushPreset preset, double size)
    {
        double hardness = Math.Clamp(preset.Hardness, 0, 1);
        IBrush fill = preset.Shape == BrushTipShape.Sampled
            ? new SolidColorBrush(Color.FromRgb(0xC8, 0xC8, 0xC8))
            : new RadialGradientBrush
            {
                GradientStops =
                {
                    new Avalonia.Media.GradientStop(Color.FromRgb(0xE6, 0xE6, 0xE6), 0),
                    new Avalonia.Media.GradientStop(Color.FromRgb(0xE6, 0xE6, 0xE6), hardness * 0.95),
                    new Avalonia.Media.GradientStop(Color.FromArgb(0, 0xE6, 0xE6, 0xE6), 1)
                }
            };
        double diameter = Math.Clamp(size * (0.45 + Math.Min(preset.Size, 100) / 180.0), 6, size);
        Border tip = new()
        {
            Width = diameter,
            Height = diameter * Math.Clamp(preset.Roundness, 0.2, 1),
            Background = fill,
            CornerRadius = preset.Shape == BrushTipShape.Square ? new CornerRadius(1) : new CornerRadius(diameter / 2),
            RenderTransform = new RotateTransform(-preset.Angle),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        return new Border { Width = size, Height = size, Child = tip };
    }

    /// <summary>Brush preset picker with size, hardness and preset tips, shown as a dropdown.</summary>
    public void BrushPicker(Func<BrushPreset> get, Action<BrushPreset> set, Func<float> getSize, Action<float> setSize)
    {
        TextBlock sizeText = new() { Text = $"{getSize():0}", VerticalAlignment = VerticalAlignment.Center, FontSize = 11 };
        ContentControl tipHost = new() { Content = TipPreview(get(), 18) };
        StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 2 };
        StackPanel tipStack = new() { Spacing = -2 };
        tipStack.Children.Add(tipHost);
        tipStack.Children.Add(sizeText);
        content.Children.Add(tipStack);
        content.Children.Add(ToolIcons.Create("chevron-down", 9));
        Button button = new() { Classes = { "icon" }, Content = content, Padding = new Thickness(4, 0), Height = 30, VerticalAlignment = VerticalAlignment.Center };
        ToolTip.SetTip(button, "Click to open the Brush Preset picker");

        StackPanel panel = new() { Spacing = 8, Width = 270 };
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Size:", DialogBuilder.SliderNumber(1, 2500, getSize(), v =>
        {
            setSize((float)v);
            sizeText.Text = $"{v:0}";
            _host.InvalidateOverlay();
        }));
        DialogBuilder.AddRow(form, "Hardness:", DialogBuilder.SliderNumber(0, 100, get().Hardness * 100, v =>
        {
            get().Hardness = (float)(v / 100);
            tipHost.Content = TipPreview(get(), 18);
        }));
        panel.Children.Add(form);
        panel.Children.Add(new Border { Classes = { "hsep" }, Margin = new Thickness(0) });
        WrapPanel presets = new() { ItemWidth = 52, ItemHeight = 52 };
        foreach (BrushPreset preset in _host.BrushPresets)
        {
            StackPanel cell = new() { Spacing = 0 };
            cell.Children.Add(TipPreview(preset, 30));
            cell.Children.Add(new TextBlock { Text = $"{preset.Size:0}", FontSize = 10, HorizontalAlignment = HorizontalAlignment.Center, Classes = { "dim" } });
            Button presetButton = new() { Classes = { "icon" }, Content = cell, Width = 50, Height = 50, HorizontalContentAlignment = HorizontalAlignment.Center };
            ToolTip.SetTip(presetButton, preset.Name);
            presetButton.Click += (_, _) =>
            {
                set(preset.Clone());
                setSize(preset.Size);
                sizeText.Text = $"{preset.Size:0}";
                tipHost.Content = TipPreview(preset, 18);
                button.Flyout?.Hide();
                _host.RefreshOptions();
            };
            presets.Children.Add(presetButton);
        }
        panel.Children.Add(new ScrollViewer { Content = presets, MaxHeight = 240 });
        button.Flyout = new Flyout { Content = panel, Placement = PlacementMode.BottomEdgeAlignedLeft };
        Add(button);
    }
}
