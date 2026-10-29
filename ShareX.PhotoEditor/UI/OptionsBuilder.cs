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
        Root = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, VerticalAlignment = VerticalAlignment.Center };
    }

    public StackPanel Root { get; }

    public void Add(Control control) => Root.Children.Add(control);

    public TextBlock Label(string text, string? tooltip = null)
    {
        TextBlock block = new() { Text = text, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2, 0, 0, 0) };
        if (tooltip != null) ToolTip.SetTip(block, tooltip);
        Add(block);
        return block;
    }

    public void Separator()
    {
        Add(new Border { Width = 1, Height = 22, Margin = new Thickness(4, 0), Background = new SolidColorBrush(Color.FromArgb(80, 128, 128, 128)) });
    }

    /// <summary>Numeric field with a scrubby label: drag the label horizontally to change the value.</summary>
    public NumericUpDown Number(string label, double min, double max, Func<double> get, Action<double> set, string format = "0", string? suffix = null, double increment = 1, double width = 78, string? tooltip = null)
    {
        TextBlock caption = Label(label, tooltip);
        caption.Cursor = new Cursor(StandardCursorType.SizeWestEast);
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
        Add(box);
        if (suffix != null) Label(suffix);
        return box;
    }

    /// <summary>Percentage option backed by a 0..1 value.</summary>
    public NumericUpDown Percent(string label, Func<float> get, Action<float> set, string? tooltip = null) =>
        Number(label, 0, 100, () => get() * 100, v => set((float)(v / 100)), "0", "%", 1, 64, tooltip);

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
        Combo("Mode:", (paintModes ? BlendModes.PaintModes : BlendModes.LayerModes).Select(m => (BlendModes.GetName(m), m)), get, set, 130);

    public CheckBox Check(string text, Func<bool> get, Action<bool> set, string? tooltip = null)
    {
        CheckBox box = new() { Content = text, IsChecked = get(), VerticalAlignment = VerticalAlignment.Center };
        box.IsCheckedChanged += (_, _) => set(box.IsChecked == true);
        if (tooltip != null) ToolTip.SetTip(box, tooltip);
        Add(box);
        return box;
    }

    public Button Button(string text, Action click, string? tooltip = null)
    {
        Button button = new() { Content = text, VerticalAlignment = VerticalAlignment.Center, Padding = new Thickness(8, 3) };
        button.Click += (_, _) => click();
        if (tooltip != null) ToolTip.SetTip(button, tooltip);
        Add(button);
        return button;
    }

    public Button IconButton(string iconKey, string tooltip, Action click, bool active = false)
    {
        Button button = new()
        {
            Content = ToolIcons.Create(iconKey, 16),
            Padding = new Thickness(4),
            VerticalAlignment = VerticalAlignment.Center,
            Background = active ? new SolidColorBrush(Color.FromArgb(90, 62, 131, 242)) : Brushes.Transparent
        };
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
        Border swatch = new() { Width = 26, Height = 16, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
        void Refresh()
        {
            SKColor c = get();
            swatch.Background = isNone?.Invoke() == true
                ? new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                    GradientStops = { new Avalonia.Media.GradientStop(Colors.White, 0.45), new Avalonia.Media.GradientStop(Colors.Red, 0.5), new Avalonia.Media.GradientStop(Colors.White, 0.55) }
                }
                : new SolidColorBrush(Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue));
        }
        Refresh();
        Button button = new() { Content = swatch, Padding = new Thickness(3), VerticalAlignment = VerticalAlignment.Center };
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
        StackPanel group = new() { Orientation = Orientation.Horizontal, Spacing = 1 };
        List<(Button Button, SelectionMode Mode)> buttons = [];
        void Refresh()
        {
            foreach (var (button, mode) in buttons)
                button.Background = mode == get() ? new SolidColorBrush(Color.FromArgb(90, 62, 131, 242)) : Brushes.Transparent;
        }
        foreach (var (mode, icon, tip) in new[]
        {
            (Imaging.SelectionMode.Replace, "sel-new", "New selection"),
            (Imaging.SelectionMode.Add, "sel-add", "Add to selection (Shift)"),
            (Imaging.SelectionMode.Subtract, "sel-sub", "Subtract from selection (Alt)"),
            (Imaging.SelectionMode.Intersect, "sel-int", "Intersect with selection (Shift+Alt)")
        })
        {
            Button button = new() { Content = ToolIcons.Create(icon, 16), Padding = new Thickness(3) };
            ToolTip.SetTip(button, tip);
            SelectionMode captured = mode;
            button.Click += (_, _) => { set(captured); Refresh(); };
            buttons.Add((button, mode));
            group.Children.Add(button);
        }
        Refresh();
        Add(group);
    }

    /// <summary>Brush preset picker with size and hardness, shown as a dropdown.</summary>
    public void BrushPicker(Func<BrushPreset> get, Action<BrushPreset> set, Func<float> getSize, Action<float> setSize)
    {
        TextBlock sizeText = new() { Text = $"{getSize():0}", VerticalAlignment = VerticalAlignment.Center, MinWidth = 24 };
        StackPanel content = new() { Orientation = Orientation.Horizontal, Spacing = 4 };
        content.Children.Add(ToolIcons.Create("brush-tip", 16));
        content.Children.Add(sizeText);
        Button button = new() { Content = content, Padding = new Thickness(4, 2), VerticalAlignment = VerticalAlignment.Center };
        ToolTip.SetTip(button, "Brush preset picker");

        StackPanel panel = new() { Spacing = 6, Margin = new Thickness(8), Width = 240 };
        Slider sizeSlider = new() { Minimum = 1, Maximum = 2500, Value = getSize() };
        TextBlock sizeLabel = new() { Text = $"Size: {getSize():0} px" };
        Slider hardSlider = new() { Minimum = 0, Maximum = 100, Value = get().Hardness * 100 };
        TextBlock hardLabel = new() { Text = $"Hardness: {get().Hardness * 100:0}%" };
        sizeSlider.ValueChanged += (_, e) =>
        {
            float v = (float)Math.Round(e.NewValue);
            setSize(v);
            sizeLabel.Text = $"Size: {v:0} px";
            sizeText.Text = $"{v:0}";
            _host.InvalidateOverlay();
        };
        hardSlider.ValueChanged += (_, e) =>
        {
            get().Hardness = (float)(e.NewValue / 100);
            hardLabel.Text = $"Hardness: {e.NewValue:0}%";
        };
        panel.Children.Add(sizeLabel);
        panel.Children.Add(sizeSlider);
        panel.Children.Add(hardLabel);
        panel.Children.Add(hardSlider);
        ListBox presets = new() { MaxHeight = 220 };
        foreach (BrushPreset preset in _host.BrushPresets)
        {
            presets.Items.Add(new ListBoxItem { Content = $"{preset.Name}  ({preset.Size:0})", Tag = preset });
        }
        presets.SelectionChanged += (_, _) =>
        {
            if (presets.SelectedItem is ListBoxItem { Tag: BrushPreset preset })
            {
                BrushPreset copy = preset.Clone();
                set(copy);
                setSize(preset.Size);
                sizeSlider.Value = preset.Size;
                hardSlider.Value = preset.Hardness * 100;
            }
        };
        panel.Children.Add(presets);
        button.Flyout = new Flyout { Content = panel, Placement = PlacementMode.Bottom };
        Add(button);
    }
}
