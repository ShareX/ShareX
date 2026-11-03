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
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Panels;

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

    private static NumericUpDown Channel() => new() { Minimum = 0, Maximum = 255, Increment = 1, FormatString = "0", Width = 40, ShowButtonSpinner = false };

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
