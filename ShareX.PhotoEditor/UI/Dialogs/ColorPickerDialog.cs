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
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Panels;

namespace ShareX.PhotoEditor.UI.Dialogs;

/// <summary>
/// Color picker with a two-dimensional field and a strip. The selected HSB, RGB or Lab component runs along the strip and the
/// other two span the field. Also shows CMYK and HEX values, a web-safe warning and opacity.
/// </summary>
public sealed class ColorPickerControl : UserControl
{
    private const int FieldSize = 256;
    private const int StripWidth = 20;
    private const int StripArrowWidth = 7;
    private const int FieldMarkerSize = 12;
    private const float LabAxisOffset = 128;
    private const float LabAxisRange = 255;
    private const int WebSteps = 5;

    private enum PickMode
    {
        Hue,
        Saturation,
        Brightness,
        Red,
        Green,
        Blue,
        LabL,
        LabA,
        LabB
    }

    private readonly Image _field = new() { Stretch = Stretch.None, IsHitTestVisible = false };
    private readonly Image _strip = new() { Stretch = Stretch.None, IsHitTestVisible = false };
    private readonly Ellipse _fieldMarker = new() { Width = FieldMarkerSize, Height = FieldMarkerSize, StrokeThickness = 1.5, IsHitTestVisible = false };
    private readonly Polygon _stripArrowLeft = StripArrow(new Point(0, 0), new Point(StripArrowWidth, 5), new Point(0, 10));
    private readonly Polygon _stripArrowRight = StripArrow(new Point(StripArrowWidth, 0), new Point(0, 5), new Point(StripArrowWidth, 10));
    private readonly Border _newSwatch = new() { Width = 70, Height = 34 };
    private readonly Border _webSwatch = new() { Width = 14, Height = 14, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1), Cursor = new Cursor(StandardCursorType.Hand) };
    private readonly StackPanel _webWarning = new() { Spacing = 3, VerticalAlignment = VerticalAlignment.Center };
    private readonly TextBox _hex = new() { Width = 80, HorizontalAlignment = HorizontalAlignment.Left };
    private readonly Slider _alpha = new() { Minimum = 0, Maximum = 100, VerticalAlignment = VerticalAlignment.Center };
    private readonly NumericUpDown _hBox, _sBox, _vBox, _rBox, _gBox, _bBox, _labLBox, _labABox, _labBBox, _cBox, _mBox, _yBox, _kBox, _aBox;
    private float _h, _s, _v, _a = 1;
    private PickMode _mode = PickMode.Hue;
    private bool _webOnly;
    private bool _updating;

    /// <summary>The pointer position while picking, so rounding and gamut clipping do not make the markers jump.</summary>
    private (float Strip, float X, float Y)? _picked;

    private (PickMode, float, bool)? _fieldKey;
    private (PickMode, float, float, bool)? _stripKey;

    public ColorPickerControl(SKColor initial, SKColor current, params Control[] actions)
    {
        Grid fieldHost = new() { Width = FieldSize, Height = FieldSize, ClipToBounds = true, Background = Brushes.Transparent, Cursor = new Cursor(StandardCursorType.Cross) };
        Canvas fieldOverlay = new() { IsHitTestVisible = false };
        fieldOverlay.Children.Add(_fieldMarker);
        fieldHost.Children.Add(_field);
        fieldHost.Children.Add(fieldOverlay);
        fieldHost.PointerPressed += (_, e) =>
        {
            e.Pointer.Capture(fieldHost);
            PickField(e.GetPosition(fieldHost));
        };
        fieldHost.PointerMoved += (_, e) =>
        {
            if (e.GetCurrentPoint(fieldHost).Properties.IsLeftButtonPressed) PickField(e.GetPosition(fieldHost));
        };

        Canvas stripHost = new() { Width = StripWidth + StripArrowWidth * 2, Height = FieldSize, Background = Brushes.Transparent, Cursor = new Cursor(StandardCursorType.SizeNorthSouth) };
        Canvas.SetLeft(_strip, StripArrowWidth);
        Canvas.SetLeft(_stripArrowRight, StripArrowWidth + StripWidth);
        stripHost.Children.Add(_strip);
        stripHost.Children.Add(_stripArrowLeft);
        stripHost.Children.Add(_stripArrowRight);
        stripHost.PointerPressed += (_, e) =>
        {
            e.Pointer.Capture(stripHost);
            PickStrip(e.GetPosition(stripHost).Y);
        };
        stripHost.PointerMoved += (_, e) =>
        {
            if (e.GetCurrentPoint(stripHost).Properties.IsLeftButtonPressed) PickStrip(e.GetPosition(stripHost).Y);
        };

        _hBox = Box(0, 360, v => { _h = (float)v; AfterBoxEdit(); });
        _sBox = Box(0, 100, v => { _s = (float)v / 100; AfterBoxEdit(); });
        _vBox = Box(0, 100, v => { _v = (float)v / 100; AfterBoxEdit(); });
        _rBox = Box(0, 255, _ => FromRgbBoxes());
        _gBox = Box(0, 255, _ => FromRgbBoxes());
        _bBox = Box(0, 255, _ => FromRgbBoxes());
        _labLBox = Box(0, 100, _ => FromLabBoxes());
        _labABox = Box(-128, 127, _ => FromLabBoxes());
        _labBBox = Box(-128, 127, _ => FromLabBoxes());
        _cBox = Box(0, 100, _ => FromCmykBoxes());
        _mBox = Box(0, 100, _ => FromCmykBoxes());
        _yBox = Box(0, 100, _ => FromCmykBoxes());
        _kBox = Box(0, 100, _ => FromCmykBoxes());
        _aBox = Box(0, 100, v => { _a = (float)v / 100; Refresh(); });
        _alpha.ValueChanged += (_, e) =>
        {
            if (_updating) return;
            _a = (float)e.NewValue / 100;
            Refresh();
        };
        _hex.LostFocus += (_, _) => ApplyHex();
        _hex.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Enter) ApplyHex();
        };

        CheckBox webOnly = new() { Content = Strings.ColorPickerDialog_OnlyWebColors };
        webOnly.IsCheckedChanged += (_, _) =>
        {
            _webOnly = webOnly.IsChecked == true;
            _picked = null;
            SnapToWeb();
            Refresh();
        };
        Grid opacity = new() { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,Auto"), ColumnSpacing = 6 };
        TextBlock opacityLabel = new() { Text = Strings.EditorCommands_OpacityColon, VerticalAlignment = VerticalAlignment.Center };
        TextBlock opacityUnit = new() { Text = "%", VerticalAlignment = VerticalAlignment.Center };
        _aBox.Width = 48;
        Grid.SetColumn(_alpha, 1);
        Grid.SetColumn(_aBox, 2);
        Grid.SetColumn(opacityUnit, 3);
        opacity.Children.Add(opacityLabel);
        opacity.Children.Add(_alpha);
        opacity.Children.Add(_aBox);
        opacity.Children.Add(opacityUnit);

        StackPanel pickers = new() { Orientation = Orientation.Horizontal, Spacing = 4 };
        pickers.Children.Add(new Border { BorderBrush = new SolidColorBrush(Avalonia.Media.Color.FromRgb(0x1A, 0x1A, 0x1A)), BorderThickness = new Thickness(1), Child = fieldHost });
        pickers.Children.Add(stripHost);
        StackPanel left = new() { Spacing = 8 };
        left.Children.Add(pickers);
        left.Children.Add(webOnly);
        left.Children.Add(opacity);

        StackPanel right = new() { Spacing = 12 };
        right.Children.Add(BuildPreview(current, actions));
        right.Children.Add(BuildFields());

        StackPanel root = new() { Orientation = Orientation.Horizontal, Spacing = 20 };
        root.Children.Add(left);
        root.Children.Add(right);
        Content = root;
        SetColor(initial);
    }

    private static Polygon StripArrow(params Point[] points) => new() { Points = points, Fill = Brushes.White, IsHitTestVisible = false };

    private NumericUpDown Box(double minimum, double maximum, Action<double> set)
    {
        NumericUpDown box = DialogBuilder.Number(minimum, maximum, 0);
        box.Width = 48;
        box.MinWidth = 48;
        box.ValueChanged += (_, e) =>
        {
            if (!_updating && e.NewValue is decimal value) set((double)value);
        };
        return box;
    }

    private Control BuildPreview(SKColor current, Control[] actions)
    {
        Border currentSwatch = new() { Width = 70, Height = 34, Background = DialogBuilder.ToBrush(current), Cursor = new Cursor(StandardCursorType.Hand) };
        ToolTip.SetTip(currentSwatch, Strings.ColorPickerDialog_RestoreCurrentTip);
        currentSwatch.PointerPressed += (_, _) => SetColor(current);

        StackPanel swatches = new() { HorizontalAlignment = HorizontalAlignment.Left };
        swatches.Children.Add(new TextBlock { Text = "new", HorizontalAlignment = HorizontalAlignment.Center, Classes = { "dim" } });
        swatches.Children.Add(new Border
        {
            BorderBrush = new SolidColorBrush(Avalonia.Media.Color.FromRgb(0x1A, 0x1A, 0x1A)),
            BorderThickness = new Thickness(1),
            Child = new StackPanel { Children = { _newSwatch, currentSwatch } }
        });
        swatches.Children.Add(new TextBlock { Text = "current", HorizontalAlignment = HorizontalAlignment.Center, Classes = { "dim" } });

        ToolTip.SetTip(_webWarning, Strings.ColorPickerDialog_NotWebColorTip);
        _webSwatch.PointerPressed += (_, _) =>
        {
            _picked = null;
            SKColor web = NearestWebColor(Color);
            SetRgb(web.Red / 255f, web.Green / 255f, web.Blue / 255f);
            Refresh();
        };
        _webWarning.Children.Add(new IconView(PhotoEditorIcons.WebColor, 16));
        _webWarning.Children.Add(_webSwatch);

        StackPanel actionPanel = new() { Spacing = 8, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(12, 0, 0, 0) };
        foreach (Control action in actions) actionPanel.Children.Add(action);
        Grid.SetColumn(_webWarning, 1);
        Grid.SetColumn(actionPanel, 3);
        Grid preview = new() { ColumnDefinitions = new ColumnDefinitions("Auto,Auto,*,Auto"), ColumnSpacing = 8 };
        preview.Children.Add(swatches);
        preview.Children.Add(_webWarning);
        preview.Children.Add(actionPanel);
        return preview;
    }

    private Control BuildFields()
    {
        Grid fields = new()
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,Auto,18,12,Auto,Auto,Auto"),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto,8,Auto,Auto,Auto,Auto"),
            RowSpacing = 4,
            ColumnSpacing = 3
        };

        void Add(Control control, int row, int column, int span = 1)
        {
            Grid.SetRow(control, row);
            Grid.SetColumn(control, column);
            Grid.SetColumnSpan(control, span);
            fields.Children.Add(control);
        }
        void Unit(string text, int row, int column) => Add(new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center }, row, column);
        void ModeField(string label, PickMode mode, NumericUpDown box, string unit, int row, int column)
        {
            RadioButton radio = new() { Content = label, GroupName = "ColorPickerMode", IsChecked = mode == _mode, MinWidth = 0, VerticalAlignment = VerticalAlignment.Center };
            radio.IsCheckedChanged += (_, _) =>
            {
                if (radio.IsChecked != true) return;
                _mode = mode;
                _picked = null;
                Refresh();
            };
            Add(radio, row, column);
            Add(box, row, column + 1);
            Unit(unit, row, column + 2);
        }
        void InkField(string label, NumericUpDown box, int row)
        {
            Add(new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 4, 0) }, row, 4);
            Add(box, row, 5);
            Unit("%", row, 6);
        }

        ModeField("H:", PickMode.Hue, _hBox, "°", 0, 0);
        ModeField("S:", PickMode.Saturation, _sBox, "%", 1, 0);
        ModeField("B:", PickMode.Brightness, _vBox, "%", 2, 0);
        ModeField("R:", PickMode.Red, _rBox, string.Empty, 4, 0);
        ModeField("G:", PickMode.Green, _gBox, string.Empty, 5, 0);
        ModeField("B:", PickMode.Blue, _bBox, string.Empty, 6, 0);
        ModeField("L:", PickMode.LabL, _labLBox, string.Empty, 0, 4);
        ModeField("a:", PickMode.LabA, _labABox, string.Empty, 1, 4);
        ModeField("b:", PickMode.LabB, _labBBox, string.Empty, 2, 4);
        InkField("C:", _cBox, 4);
        InkField("M:", _mBox, 5);
        InkField("Y:", _yBox, 6);
        InkField("K:", _kBox, 7);
        Add(new TextBlock { Text = "#", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 4, 0) }, 7, 0);
        Add(_hex, 7, 1, 3);
        return fields;
    }

    public SKColor Color => ColorMath.FromHsv(_h, _s, _v).WithAlpha((byte)MathF.Round(_a * 255));

    public event Action<SKColor>? ColorChanged;

    public void SetColor(SKColor color)
    {
        _picked = null;
        SetRgb(color.Red / 255f, color.Green / 255f, color.Blue / 255f);
        _a = color.Alpha / 255f;
        SnapToWeb();
        Refresh();
    }

    /// <summary>Stores a color, keeping the hue and saturation that black, white and grays cannot express.</summary>
    private void SetRgb(float r, float g, float b)
    {
        ColorMath.RgbToHsv(r, g, b, out float h, out float s, out float v);
        if (s > 0.001f) _h = h;
        if (v > 0.001f) _s = s;
        _v = v;
    }

    private void SnapToWeb()
    {
        if (!_webOnly) return;
        SKColor web = NearestWebColor(Color);
        SetRgb(web.Red / 255f, web.Green / 255f, web.Blue / 255f);
    }

    private static float SnapChannel(float value) => MathF.Round(value * WebSteps) / WebSteps;

    private static SKColor NearestWebColor(SKColor c)
    {
        static byte Snap(byte channel) => (byte)(MathF.Round(channel / 255f * WebSteps) * 255 / WebSteps);
        return new SKColor(Snap(c.Red), Snap(c.Green), Snap(c.Blue), c.Alpha);
    }

    /// <summary>The color at a strip position and field position, all in 0..1 with Y pointing up.</summary>
    private (float R, float G, float B) ColorAt(float strip, float x, float y)
    {
        float r, g, b;
        switch (_mode)
        {
            case PickMode.Hue:
                ColorMath.HsvToRgb(strip * 360, x, y, out r, out g, out b);
                break;
            case PickMode.Saturation:
                ColorMath.HsvToRgb(x * 360, strip, y, out r, out g, out b);
                break;
            case PickMode.Brightness:
                ColorMath.HsvToRgb(x * 360, y, strip, out r, out g, out b);
                break;
            case PickMode.Red:
                (r, g, b) = (strip, y, x);
                break;
            case PickMode.Green:
                (r, g, b) = (y, strip, x);
                break;
            case PickMode.Blue:
                (r, g, b) = (x, y, strip);
                break;
            case PickMode.LabL:
                ColorMath.LabToRgb(strip * 100, LabAxis(x), LabAxis(y), out r, out g, out b);
                break;
            case PickMode.LabA:
                ColorMath.LabToRgb(y * 100, LabAxis(strip), LabAxis(x), out r, out g, out b);
                break;
            default:
                ColorMath.LabToRgb(y * 100, LabAxis(x), LabAxis(strip), out r, out g, out b);
                break;
        }
        return _webOnly ? (SnapChannel(r), SnapChannel(g), SnapChannel(b)) : (r, g, b);
    }

    private static float LabAxis(float position) => position * LabAxisRange - LabAxisOffset;

    private static float LabPosition(float value) => (value + LabAxisOffset) / LabAxisRange;

    /// <summary>The inverse of <see cref="ColorAt"/> for the current color.</summary>
    private (float Strip, float X, float Y) CurrentPosition()
    {
        if (_picked is { } picked) return picked;
        SKColor c = Color;
        float r = c.Red / 255f, g = c.Green / 255f, b = c.Blue / 255f;
        ColorMath.RgbToLab(r, g, b, out float labL, out float labA, out float labB);
        return _mode switch
        {
            PickMode.Hue => (_h / 360, _s, _v),
            PickMode.Saturation => (_s, _h / 360, _v),
            PickMode.Brightness => (_v, _h / 360, _s),
            PickMode.Red => (r, b, g),
            PickMode.Green => (g, b, r),
            PickMode.Blue => (b, r, g),
            PickMode.LabL => (labL / 100, LabPosition(labA), LabPosition(labB)),
            PickMode.LabA => (LabPosition(labA), LabPosition(labB), labL / 100),
            _ => (LabPosition(labB), LabPosition(labA), labL / 100)
        };
    }

    private void PickField(Point p)
    {
        float x = (float)Math.Clamp(p.X / (FieldSize - 1), 0, 1);
        float y = (float)Math.Clamp(1 - p.Y / (FieldSize - 1), 0, 1);
        ApplyPicked((CurrentPosition().Strip, x, y));
    }

    private void PickStrip(double y)
    {
        (_, float fieldX, float fieldY) = CurrentPosition();
        ApplyPicked(((float)Math.Clamp(1 - y / (FieldSize - 1), 0, 1), fieldX, fieldY));
    }

    private void ApplyPicked((float Strip, float X, float Y) position)
    {
        _picked = position;
        (float strip, float x, float y) = position;
        if (_webOnly || _mode > PickMode.Brightness)
        {
            (float r, float g, float b) = ColorAt(strip, x, y);
            SetRgb(r, g, b);
        }
        else
        {
            // Hue, saturation and brightness are stored as they are, so hue survives on black and gray.
            (_h, _s, _v) = _mode switch
            {
                PickMode.Hue => (strip * 360, x, y),
                PickMode.Saturation => (x * 360, strip, y),
                _ => (x * 360, y, strip)
            };
        }
        Refresh();
    }

    private void AfterBoxEdit()
    {
        _picked = null;
        SnapToWeb();
        Refresh();
    }

    private void FromRgbBoxes()
    {
        SetRgb((float)(_rBox.Value ?? 0) / 255, (float)(_gBox.Value ?? 0) / 255, (float)(_bBox.Value ?? 0) / 255);
        AfterBoxEdit();
    }

    private void FromLabBoxes()
    {
        ColorMath.LabToRgb((float)(_labLBox.Value ?? 0), (float)(_labABox.Value ?? 0), (float)(_labBBox.Value ?? 0), out float r, out float g, out float b);
        SetRgb(r, g, b);
        _picked = null;
        SnapToWeb();
        // The typed values stay as entered; most Lab values have no exact RGB color.
        Refresh(_labLBox, _labABox, _labBBox);
    }

    private void FromCmykBoxes()
    {
        float keep = 1 - (float)(_kBox.Value ?? 0) / 100;
        SetRgb((1 - (float)(_cBox.Value ?? 0) / 100) * keep, (1 - (float)(_mBox.Value ?? 0) / 100) * keep, (1 - (float)(_yBox.Value ?? 0) / 100) * keep);
        _picked = null;
        SnapToWeb();
        // The typed values stay as entered; many ink combinations give the same RGB color.
        Refresh(_cBox, _mBox, _yBox, _kBox);
    }

    private void ApplyHex()
    {
        if (_updating) return;
        if (ColorMath.TryParseHex(_hex.Text, out SKColor parsed)) SetColor(parsed.WithAlpha(Color.Alpha));
    }

    private void Refresh(params NumericUpDown[] keep)
    {
        _updating = true;
        SKColor c = Color;
        (float strip, float x, float y) = CurrentPosition();
        RenderField(strip);
        RenderStrip(x, y);

        Canvas.SetLeft(_fieldMarker, x * (FieldSize - 1) - FieldMarkerSize / 2.0);
        Canvas.SetTop(_fieldMarker, (1 - y) * (FieldSize - 1) - FieldMarkerSize / 2.0);
        _fieldMarker.Stroke = ColorMath.Luminance(c) > 0.6f ? Brushes.Black : Brushes.White;
        double stripTop = (1 - strip) * (FieldSize - 1) - 5;
        Canvas.SetTop(_stripArrowLeft, stripTop);
        Canvas.SetTop(_stripArrowRight, stripTop);

        _newSwatch.Background = DialogBuilder.ToBrush(c);
        SKColor web = NearestWebColor(c);
        _webWarning.IsVisible = web != c;
        _webSwatch.Background = DialogBuilder.ToBrush(web.WithAlpha(255));

        void Show(NumericUpDown box, double value)
        {
            if (!keep.Contains(box)) box.Value = (decimal)Math.Round(value);
        }
        float r = c.Red / 255f, g = c.Green / 255f, b = c.Blue / 255f;
        Show(_hBox, _h);
        Show(_sBox, _s * 100);
        Show(_vBox, _v * 100);
        Show(_rBox, c.Red);
        Show(_gBox, c.Green);
        Show(_bBox, c.Blue);
        ColorMath.RgbToLab(r, g, b, out float labL, out float labA, out float labB);
        Show(_labLBox, labL);
        Show(_labABox, Math.Clamp(labA, -128, 127));
        Show(_labBBox, Math.Clamp(labB, -128, 127));
        float black = 1 - MathF.Max(r, MathF.Max(g, b));
        float ink = black < 1 ? 1 - black : 1;
        Show(_cBox, (ink - r) / ink * 100 * (black < 1 ? 1 : 0));
        Show(_mBox, (ink - g) / ink * 100 * (black < 1 ? 1 : 0));
        Show(_yBox, (ink - b) / ink * 100 * (black < 1 ? 1 : 0));
        Show(_kBox, black * 100);
        Show(_aBox, _a * 100);
        _alpha.Value = _a * 100;
        if (!_hex.IsFocused) _hex.Text = ColorMath.ToHex(c, c.Alpha < 255).ToLowerInvariant();
        _updating = false;
        ColorChanged?.Invoke(c);
    }

    private void RenderField(float strip)
    {
        (PickMode, float, bool) key = (_mode, strip, _webOnly);
        if (_fieldKey == key) return;
        _fieldKey = key;
        Show(_field, FieldSize, (px, py) => ColorAt(strip, px / (FieldSize - 1f), 1 - py / (FieldSize - 1f)));
    }

    private void RenderStrip(float x, float y)
    {
        // The hue strip is always the full spectrum.
        bool hue = _mode == PickMode.Hue;
        (PickMode, float, float, bool) key = (_mode, hue ? 0 : x, hue ? 0 : y, _webOnly);
        if (_stripKey == key) return;
        _stripKey = key;
        Show(_strip, StripWidth, (_, py) =>
        {
            float position = 1 - py / (FieldSize - 1f);
            return hue ? ColorAt(position, 1, 1) : ColorAt(position, x, y);
        });
    }

    private static unsafe void Show(Image image, int width, Func<int, int, (float R, float G, float B)> colorAt)
    {
        using SKBitmap bitmap = ImageOps.CreateBitmap(width, FieldSize);
        uint* pixels = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        Parallel.For(0, FieldSize, y =>
        {
            for (int x = 0; x < width; x++)
            {
                (float r, float g, float b) = colorAt(x, y);
                pixels[y * stride + x] = ColorMath.PackUnpremul(r, g, b, 1);
            }
        });
        WriteableBitmap? previous = image.Source as WriteableBitmap;
        image.Source = BitmapHelper.ToAvalonia(bitmap);
        previous?.Dispose();
    }
}

public static class ColorPickerDialog
{
    public static async Task<SKColor?> ShowAsync(Window owner, SKColor initial, string title, IEditorHost? host = null)
    {
        Button reset = ActionButton(Strings.ColorPickerDialog_Reset);
        Button add = ActionButton(Strings.ColorPickerDialog_AddToSwatches);
        add.IsVisible = host != null;
        ColorPickerControl picker = new(initial, initial, reset, add);
        reset.Click += (_, _) => picker.SetColor(initial);
        add.Click += (_, _) =>
        {
            host?.Settings.Swatches.Add("#" + ColorMath.ToHex(picker.Color));
            host?.RefreshPanels();
        };
        bool ok = await DialogBuilder.ShowAsync(owner, title.Length > 0 ? string.Format(Strings.ColorPickerDialog_TitleWith, title) : Strings.ColorPickerDialog_Title, picker, DialogBuilder.AutoWidth);
        return ok ? picker.Color : null;
    }

    private static Button ActionButton(string text) => new() { Classes = { "secondary" }, Content = text, HorizontalAlignment = HorizontalAlignment.Stretch };
}
