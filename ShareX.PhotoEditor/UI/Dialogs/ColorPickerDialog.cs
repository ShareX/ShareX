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

namespace ShareX.PhotoEditor.UI.Dialogs;

/// <summary>Color picker with a saturation/brightness field, hue strip, alpha and HEX, RGB, HSB and HSL fields.</summary>
public sealed class ColorPickerControl : UserControl
{
    private float _h, _s, _v, _a = 1;
    private readonly Border _field;
    private readonly Canvas _fieldMarker;
    private readonly Border _hueStrip;
    private readonly Border _hueMarker;
    private readonly Slider _alpha;
    private readonly Border _newSwatch;
    private readonly TextBox _hex;
    private readonly NumericUpDown _r, _g, _b, _hBox, _sBox, _vBox, _lBox, _hslS, _aBox;
    private bool _updating;

    public ColorPickerControl(SKColor initial, SKColor current)
    {
        _field = new Border { Width = 256, Height = 256, ClipToBounds = true, Cursor = new Cursor(StandardCursorType.Cross) };
        _fieldMarker = new Canvas { IsHitTestVisible = false };
        Grid fieldHost = new() { Width = 256, Height = 256 };
        Border white = new()
        {
            Background = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops = { new Avalonia.Media.GradientStop(Colors.White, 0), new Avalonia.Media.GradientStop(Avalonia.Media.Color.FromArgb(0, 255, 255, 255), 1) }
            },
            IsHitTestVisible = false
        };
        Border black = new()
        {
            Background = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                GradientStops = { new Avalonia.Media.GradientStop(Avalonia.Media.Color.FromArgb(0, 0, 0, 0), 0), new Avalonia.Media.GradientStop(Colors.Black, 1) }
            },
            IsHitTestVisible = false
        };
        fieldHost.Children.Add(_field);
        fieldHost.Children.Add(white);
        fieldHost.Children.Add(black);
        fieldHost.Children.Add(_fieldMarker);
        // Overlays are not hit-testable, so route pointer input through the host grid.
        fieldHost.PointerPressed += (_, e) => { e.Pointer.Capture(fieldHost); PickField(e.GetPosition(fieldHost)); };
        fieldHost.PointerMoved += (_, e) => { if (e.GetCurrentPoint(fieldHost).Properties.IsLeftButtonPressed) PickField(e.GetPosition(fieldHost)); };

        LinearGradientBrush spectrum = new() { StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative) };
        for (int i = 0; i <= 6; i++)
        {
            SKColor c = ColorMath.FromHsv(360 - i * 60, 1, 1);
            spectrum.GradientStops.Add(new Avalonia.Media.GradientStop(Avalonia.Media.Color.FromRgb(c.Red, c.Green, c.Blue), i / 6.0));
        }
        _hueStrip = new Border { Width = 22, Height = 256, Background = spectrum, Cursor = new Cursor(StandardCursorType.SizeNorthSouth) };
        _hueMarker = new Border { Height = 4, Width = 26, BorderBrush = Brushes.White, BorderThickness = new Thickness(1), Background = Brushes.Transparent, IsHitTestVisible = false };
        Canvas hueHost = new() { Width = 26, Height = 256 };
        Canvas.SetLeft(_hueStrip, 2);
        hueHost.Children.Add(_hueStrip);
        hueHost.Children.Add(_hueMarker);
        hueHost.PointerPressed += (_, e) => { e.Pointer.Capture(hueHost); PickHue(e.GetPosition(hueHost).Y); };
        hueHost.PointerMoved += (_, e) => { if (e.GetCurrentPoint(hueHost).Properties.IsLeftButtonPressed) PickHue(e.GetPosition(hueHost).Y); };

        _newSwatch = new Border { Width = 70, Height = 34 };
        Border currentSwatch = new() { Width = 70, Height = 34, Background = DialogBuilder.ToBrush(current), Cursor = new Cursor(StandardCursorType.Hand) };
        ToolTip.SetTip(currentSwatch, "Click to restore the current color");
        currentSwatch.PointerPressed += (_, _) => SetColor(current);

        _hex = new TextBox { Width = 90 };
        _hex.LostFocus += (_, _) => ApplyHex();
        _hex.KeyDown += (_, e) => { if (e.Key == Key.Enter) ApplyHex(); };

        NumericUpDown Box(double max, Action<double> set)
        {
            NumericUpDown box = DialogBuilder.Number(0, max, 0);
            box.Width = 72;
            box.MinWidth = 72;
            box.ValueChanged += (_, e) => { if (!_updating && e.NewValue is decimal d) set((double)d); };
            return box;
        }
        _hBox = Box(360, v => { _h = (float)v; Refresh(); });
        _sBox = Box(100, v => { _s = (float)v / 100; Refresh(); });
        _vBox = Box(100, v => { _v = (float)v / 100; Refresh(); });
        _r = Box(255, _ => FromRgbBoxes());
        _g = Box(255, _ => FromRgbBoxes());
        _b = Box(255, _ => FromRgbBoxes());
        _hslS = Box(100, _ => FromHslBoxes());
        _lBox = Box(100, _ => FromHslBoxes());
        _aBox = Box(100, v => { _a = (float)v / 100; Refresh(); });
        _alpha = new Slider { Minimum = 0, Maximum = 100, Width = 160 };
        _alpha.ValueChanged += (_, e) => { if (!_updating) { _a = (float)e.NewValue / 100; Refresh(); } };

        // Two aligned field columns: HSB and RGB on the left, HSL and alpha on the right.
        Grid fields = new() { ColumnDefinitions = new ColumnDefinitions("Auto,Auto,14,22,Auto,Auto,Auto"), RowSpacing = 6, ColumnSpacing = 4 };
        void Field(int row, int column, string label, NumericUpDown box, string unit)
        {
            while (fields.RowDefinitions.Count <= row) fields.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            box.Width = 48;
            TextBlock caption = new() { Text = label, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) };
            TextBlock suffix = new() { Text = unit, Classes = { "dim" }, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(caption, row); Grid.SetColumn(caption, column);
            Grid.SetRow(box, row); Grid.SetColumn(box, column + 1);
            Grid.SetRow(suffix, row); Grid.SetColumn(suffix, column + 2);
            fields.Children.Add(caption);
            fields.Children.Add(box);
            fields.Children.Add(suffix);
        }
        Field(0, 0, "H:", _hBox, "°");
        Field(1, 0, "S:", _sBox, "%");
        Field(2, 0, "B:", _vBox, "%");
        Field(4, 0, "R:", _r, string.Empty);
        Field(5, 0, "G:", _g, string.Empty);
        Field(6, 0, "B:", _b, string.Empty);
        Field(0, 4, "A:", _aBox, "%");
        Field(4, 4, "S:", _hslS, "%");
        Field(5, 4, "L:", _lBox, "%");
        fields.RowDefinitions.Add(new RowDefinition(new GridLength(8)));
        fields.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        TextBlock hash = new() { Text = "#", VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(hash, 8);
        Grid.SetRow(_hex, 8);
        Grid.SetColumn(_hex, 1);
        Grid.SetColumnSpan(_hex, 4);
        _hex.Width = 80;
        _hex.HorizontalAlignment = HorizontalAlignment.Left;
        fields.Children.Add(hash);
        fields.Children.Add(_hex);
        TextBlock hslHeader = new() { Text = "HSL", Classes = { "dim" }, FontSize = 10, VerticalAlignment = VerticalAlignment.Bottom };
        Grid.SetRow(hslHeader, 3);
        Grid.SetColumn(hslHeader, 4);
        fields.Children.Add(hslHeader);

        StackPanel swatches = new() { Spacing = 0, HorizontalAlignment = HorizontalAlignment.Left };
        swatches.Children.Add(new TextBlock { Text = "new", HorizontalAlignment = HorizontalAlignment.Center, FontSize = 11, Classes = { "dim" } });
        swatches.Children.Add(new Border { BorderBrush = new SolidColorBrush(Avalonia.Media.Color.FromRgb(0x1A, 0x1A, 0x1A)), BorderThickness = new Thickness(1), Child = new StackPanel { Children = { _newSwatch, currentSwatch } } });
        swatches.Children.Add(new TextBlock { Text = "current", HorizontalAlignment = HorizontalAlignment.Center, FontSize = 11, Classes = { "dim" } });

        StackPanel right = new() { Spacing = 10 };
        right.Children.Add(swatches);
        right.Children.Add(fields);

        StackPanel alphaRow = new() { Orientation = Orientation.Horizontal, Spacing = 8 };
        alphaRow.Children.Add(new TextBlock { Text = "Opacity", VerticalAlignment = VerticalAlignment.Center });
        alphaRow.Children.Add(_alpha);

        StackPanel left = new() { Spacing = 8 };
        StackPanel pickers = new() { Orientation = Orientation.Horizontal, Spacing = 10 };
        pickers.Children.Add(fieldHost);
        pickers.Children.Add(hueHost);
        left.Children.Add(pickers);
        left.Children.Add(alphaRow);

        StackPanel root = new() { Orientation = Orientation.Horizontal, Spacing = 16 };
        root.Children.Add(left);
        root.Children.Add(right);
        Content = root;
        SetColor(initial);
    }

    private static Control Unit(Control box, string unit)
    {
        StackPanel panel = new() { Orientation = Orientation.Horizontal, Spacing = 4 };
        panel.Children.Add(box);
        panel.Children.Add(new TextBlock { Text = unit, VerticalAlignment = VerticalAlignment.Center });
        return panel;
    }

    public SKColor Color
    {
        get
        {
            SKColor c = ColorMath.FromHsv(_h, _s, _v);
            return c.WithAlpha((byte)MathF.Round(_a * 255));
        }
    }

    public event Action<SKColor>? ColorChanged;

    public void SetColor(SKColor color)
    {
        ColorMath.RgbToHsv(color.Red / 255f, color.Green / 255f, color.Blue / 255f, out float h, out float s, out float v);
        if (s > 0.001f) _h = h;
        _s = s;
        _v = v;
        _a = color.Alpha / 255f;
        Refresh();
    }

    private void PickField(Point p)
    {
        _s = (float)Math.Clamp(p.X / 256, 0, 1);
        _v = (float)Math.Clamp(1 - p.Y / 256, 0, 1);
        Refresh();
    }

    private void PickHue(double y)
    {
        _h = (float)Math.Clamp(360 - y / 256 * 360, 0, 359.99);
        Refresh();
    }

    private void ApplyHex()
    {
        if (_updating) return;
        if (ColorMath.TryParseHex(_hex.Text, out SKColor parsed)) SetColor(parsed.WithAlpha((byte)(_a * 255)));
    }

    private void FromRgbBoxes()
    {
        SKColor c = new((byte)(_r.Value ?? 0), (byte)(_g.Value ?? 0), (byte)(_b.Value ?? 0), (byte)(_a * 255));
        SetColor(c);
    }

    private void FromHslBoxes()
    {
        ColorMath.HslToRgb(_h, (float)(_hslS.Value ?? 0) / 100, (float)(_lBox.Value ?? 0) / 100, out float r, out float g, out float b);
        ColorMath.RgbToHsv(r, g, b, out _, out _s, out _v);
        Refresh();
    }

    private void Refresh()
    {
        _updating = true;
        SKColor hue = ColorMath.FromHsv(_h, 1, 1);
        _field.Background = new SolidColorBrush(Avalonia.Media.Color.FromRgb(hue.Red, hue.Green, hue.Blue));
        _fieldMarker.Children.Clear();
        Avalonia.Controls.Shapes.Ellipse marker = new() { Width = 12, Height = 12, Stroke = _v > 0.5f ? Brushes.Black : Brushes.White, StrokeThickness = 1.5 };
        Canvas.SetLeft(marker, _s * 256 - 6);
        Canvas.SetTop(marker, (1 - _v) * 256 - 6);
        _fieldMarker.Children.Add(marker);
        Canvas.SetTop(_hueMarker, (1 - _h / 360) * 256 - 2);
        SKColor c = Color;
        _newSwatch.Background = DialogBuilder.ToBrush(c);
        _hBox.Value = (decimal)Math.Round(_h);
        _sBox.Value = (decimal)Math.Round(_s * 100);
        _vBox.Value = (decimal)Math.Round(_v * 100);
        _r.Value = c.Red;
        _g.Value = c.Green;
        _b.Value = c.Blue;
        ColorMath.RgbToHsl(c.Red / 255f, c.Green / 255f, c.Blue / 255f, out _, out float hs, out float hl);
        _hslS.Value = (decimal)Math.Round(hs * 100);
        _lBox.Value = (decimal)Math.Round(hl * 100);
        _aBox.Value = (decimal)Math.Round(_a * 100);
        _alpha.Value = _a * 100;
        if (!_hex.IsFocused) _hex.Text = ColorMath.ToHex(c, c.Alpha < 255);
        _updating = false;
        ColorChanged?.Invoke(c);
    }
}

public static class ColorPickerDialog
{
    public static async Task<SKColor?> ShowAsync(Window owner, SKColor initial, string title, IEditorHost? host = null)
    {
        ColorPickerControl picker = new(initial, initial);
        StackPanel content = new() { Spacing = 10 };
        content.Children.Add(picker);
        if (host != null)
        {
            // Swatches from the editor make it quick to reuse colors.
            WrapPanel swatches = new() { MaxWidth = 560 };
            foreach (string hex in host.Settings.Swatches.Take(40))
            {
                if (!ColorMath.TryParseHex(hex, out SKColor color)) continue;
                Border swatch = new() { Width = 16, Height = 16, Margin = new Thickness(1), Background = DialogBuilder.ToBrush(color), Cursor = new Cursor(StandardCursorType.Hand) };
                swatch.PointerPressed += (_, _) => picker.SetColor(color);
                swatches.Children.Add(swatch);
            }
            content.Children.Add(swatches);
        }
        Button add = new() { Content = "Add to Swatches" };
        add.Click += (_, _) =>
        {
            host?.Settings.Swatches.Add("#" + ColorMath.ToHex(picker.Color));
            host?.RefreshPanels();
        };
        bool ok = await DialogBuilder.ShowAsync(owner, title.Length > 0 ? $"Color Picker ({title})" : "Color Picker", content, 600, "OK", null, host != null ? [add] : null);
        return ok ? picker.Color : null;
    }
}
