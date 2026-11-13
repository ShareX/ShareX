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

namespace ShareX.PixelEditor.UI.Panels;

/// <summary>A saturation and value square, or a vertical hue or alpha bar, that reports the picked position.</summary>
public sealed class ColorField : Control
{
    public enum FieldKind
    {
        SaturationValue,
        Hue,
        Alpha
    }

    private static readonly IBrush HueBrush = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
        GradientStops =
        {
            new GradientStop(Color.FromRgb(255, 0, 0), 0), new GradientStop(Color.FromRgb(255, 255, 0), 1 / 6.0), new GradientStop(Color.FromRgb(0, 255, 0), 2 / 6.0),
            new GradientStop(Color.FromRgb(0, 255, 255), 3 / 6.0), new GradientStop(Color.FromRgb(0, 0, 255), 4 / 6.0), new GradientStop(Color.FromRgb(255, 0, 255), 5 / 6.0), new GradientStop(Color.FromRgb(255, 0, 0), 1)
        }
    };

    private static readonly IBrush BlackFade = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
        GradientStops = { new GradientStop(Color.FromArgb(0, 0, 0, 0), 0), new GradientStop(Colors.Black, 1) }
    };

    private static readonly IBrush CheckerBrush = Ui.Checker(0xFFFFFFFF, 0xFFC8C8C8, 4);

    private readonly FieldKind _kind;

    public ColorField(FieldKind kind)
    {
        _kind = kind;
        ClipToBounds = true;
        Cursor = new Cursor(StandardCursorType.Cross);
    }

    /// <summary>Horizontal position 0..1 (saturation).</summary>
    public double X { get; set; }

    /// <summary>Vertical position 0..1 (1 - value, hue / 360 or 1 - alpha).</summary>
    public double Y { get; set; }

    /// <summary>The fully saturated color of the current hue, behind the square and the alpha bar.</summary>
    public Color BaseColor { get; set; } = Colors.Red;

    public event Action? Changed;

    public override void Render(DrawingContext context)
    {
        Rect bounds = new(Bounds.Size);
        switch (_kind)
        {
            case FieldKind.SaturationValue:
                context.FillRectangle(new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                    GradientStops = { new GradientStop(Colors.White, 0), new GradientStop(BaseColor, 1) }
                }, bounds);
                context.FillRectangle(BlackFade, bounds);
                Point p = new(Math.Round(X * bounds.Width), Math.Round(Y * bounds.Height));
                context.DrawEllipse(null, new Pen(Brushes.Black, 1), p, 5.5, 5.5);
                context.DrawEllipse(null, new Pen(Brushes.White, 1), p, 4.5, 4.5);
                return;
            case FieldKind.Hue:
                context.FillRectangle(HueBrush, bounds);
                break;
            default:
                context.FillRectangle(CheckerBrush, bounds);
                context.FillRectangle(new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                    GradientStops = { new GradientStop(BaseColor, 0), new GradientStop(Color.FromArgb(0, BaseColor.R, BaseColor.G, BaseColor.B), 1) }
                }, bounds);
                break;
        }
        double y = Math.Round(Y * (bounds.Height - 1)) + 0.5;
        context.DrawRectangle(null, new Pen(Brushes.Black, 1), new Rect(0.5, y - 2, bounds.Width - 1, 4));
        context.DrawRectangle(null, new Pen(Brushes.White, 1), new Rect(1.5, y - 1, bounds.Width - 3, 2));
    }

    private void Pick(Point position)
    {
        if (Bounds.Width <= 0 || Bounds.Height <= 0) return;
        X = Math.Clamp(position.X / Bounds.Width, 0, 1);
        Y = Math.Clamp(position.Y / Bounds.Height, 0, 1);
        InvalidateVisual();
        Changed?.Invoke();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        e.Pointer.Capture(this);
        Pick(e.GetPosition(this));
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (ReferenceEquals(e.Pointer.Captured, this)) Pick(e.GetPosition(this));
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Pointer.Capture(null);
    }
}

/// <summary>Picks the foreground or background color with a saturation square, hue and alpha bars and a hex field.</summary>
public sealed class ColorPanel : EditorPanel
{
    private readonly ColorField _square = new(ColorField.FieldKind.SaturationValue) { MinHeight = 60 };
    private readonly ColorField _hue = new(ColorField.FieldKind.Hue) { Width = 16 };
    private readonly ColorField _alpha = new(ColorField.FieldKind.Alpha) { Width = 16 };
    private readonly Border _foreground, _background;
    private readonly TextBox _hex = new() { Width = 78, MaxLength = 9 };
    private readonly TextBlock _values = new() { Classes = { "dim" }, FontSize = 11, VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
    private bool _editBackground, _updating;
    private float _h, _s, _v = 1;
    private int _a = 255;

    public ColorPanel(PixelEditorWindow editor) : base(editor)
    {
        _foreground = MakeTarget(false);
        _background = MakeTarget(true);
        _square.Changed += FieldChanged;
        _hue.Changed += FieldChanged;
        _alpha.Changed += FieldChanged;
        _hex.KeyDown += (_, e) =>
        {
            if (e.Key != Key.Enter) return;
            if (Argb.TryParseHex(_hex.Text, out uint color)) Apply(color);
            Editor.Canvas.Focus();
        };
        _hex.LostFocus += (_, _) =>
        {
            if (Argb.TryParseHex(_hex.Text, out uint color) && color != Current) Apply(color);
        };

        Grid fields = new() { ColumnDefinitions = new ColumnDefinitions("*,Auto,Auto"), ColumnSpacing = 6 };
        Grid.SetColumn(_hue, 1);
        Grid.SetColumn(_alpha, 2);
        fields.Children.Add(_square);
        fields.Children.Add(_hue);
        fields.Children.Add(_alpha);

        Button swap = Ui.IconButton(PixelEditorIcons.SwapColors, Strings.Toolbar_SwitchColors, () => Editor.Execute("color.swap"), 12);
        Button add = Ui.IconButton(PixelEditorIcons.Plus, Strings.Palette_AddColor, () => Editor.Execute("palette.addForeground"), 12);
        StackPanel row = new() { Orientation = Orientation.Horizontal, Spacing = 6, Margin = new Thickness(0, 6, 0, 0) };
        row.Children.Add(_foreground);
        row.Children.Add(_background);
        row.Children.Add(swap);
        row.Children.Add(new TextBlock { Text = "#", VerticalAlignment = VerticalAlignment.Center, Classes = { "dim" }, Margin = new Thickness(2, 0, -4, 0) });
        row.Children.Add(_hex);
        row.Children.Add(add);
        StackPanel bottom = new();
        bottom.Children.Add(row);
        _values.Margin = new Thickness(0, 5, 0, 0);
        bottom.Children.Add(_values);
        _recent.Margin = new Thickness(0, 6, 0, 0);
        bottom.Children.Add(_recent);

        DockPanel root = new() { Margin = new Thickness(8) };
        DockPanel.SetDock(bottom, Dock.Bottom);
        root.Children.Add(bottom);
        root.Children.Add(fields);
        Content = root;
    }

    private readonly WrapPanel _recent = new() { Orientation = Orientation.Horizontal };
    private string? _recentKey;

    public override string Id => "color";

    public override string Title => Strings.Panel_Color;

    private uint Current => _editBackground ? Editor.BackgroundColor : Editor.ForegroundColor;

    private Border MakeTarget(bool background)
    {
        Border fill = new();
        Border target = new()
        {
            Width = 26,
            Height = 22,
            BorderThickness = new Thickness(2),
            Background = Ui.Checker(0xFFFFFFFF, 0xFFC8C8C8, 4),
            Child = fill,
            Cursor = new Cursor(StandardCursorType.Hand)
        };
        ToolTip.SetTip(target, background ? Strings.Toolbar_BackgroundColor : Strings.Toolbar_ForegroundColor);
        target.PointerPressed += (_, _) =>
        {
            _editBackground = background;
            Refresh();
        };
        return target;
    }

    private void FieldChanged()
    {
        if (_updating) return;
        _h = (float)(_hue.Y * 360);
        if (_h >= 360) _h = 359.9f;
        _s = (float)_square.X;
        _v = (float)(1 - _square.Y);
        _a = (int)Math.Round((1 - _alpha.Y) * 255);
        uint color = Argb.FromHsv(_h, _s, _v, _a);
        _updating = true;
        if (_editBackground) Editor.BackgroundColor = color;
        else Editor.ForegroundColor = color;
        _updating = false;
        Show(color);
    }

    private void Apply(uint color)
    {
        if (_editBackground) Editor.BackgroundColor = color;
        else Editor.ForegroundColor = color;
    }

    public override void Refresh()
    {
        if (_updating) return;
        uint color = Current;
        // Keep the hue and saturation the user picked when the color is gray or black, where they are undefined.
        if (Argb.FromHsv(_h, _s, _v, _a) != color)
        {
            (float h, float s, float v) = Argb.ToHsv(color);
            if (s > 0 && v > 0) _h = h;
            if (v > 0) _s = s;
            _v = v;
            _a = Argb.A(color);
        }
        Show(color);
    }

    private void Show(uint color)
    {
        Color baseColor = Argb.ToAvalonia(Argb.FromHsv(_h, 1, 1));
        _square.BaseColor = baseColor;
        _square.X = _s;
        _square.Y = 1 - _v;
        _hue.Y = _h / 360;
        _alpha.BaseColor = Argb.ToAvalonia(Argb.Opaque(color));
        _alpha.Y = 1 - _a / 255.0;
        _square.InvalidateVisual();
        _hue.InvalidateVisual();
        _alpha.InvalidateVisual();
        ((Border)_foreground.Child!).Background = Ui.Brush(Editor.ForegroundColor);
        ((Border)_background.Child!).Background = Ui.Brush(Editor.BackgroundColor);
        _foreground.BorderBrush = _editBackground ? Ui.Gutter : Brushes.White;
        _background.BorderBrush = _editBackground ? Brushes.White : Ui.Gutter;
        if (!_hex.IsFocused) _hex.Text = Argb.ToHex(color);
        ShowRecent();
        _values.Text = $"R {Argb.R(color)}   G {Argb.G(color)}   B {Argb.B(color)}   A {Argb.A(color)}      H {_h:0}°   S {_s * 100:0}%   V {_v * 100:0}%";
    }

    /// <summary>Rebuilds the row of recently painted colors when it changed.</summary>
    private void ShowRecent()
    {
        List<uint> colors = Editor.Settings.RecentColors;
        string key = string.Join(",", colors);
        if (key == _recentKey) return;
        _recentKey = key;
        _recent.Children.Clear();
        _recent.IsVisible = colors.Count > 0;
        foreach (uint color in colors)
        {
            Border swatch = Ui.Swatch(color, 14);
            swatch.Margin = new Thickness(0, 0, 3, 3);
            swatch.Cursor = new Cursor(StandardCursorType.Hand);
            ToolTip.SetTip(swatch, "#" + Argb.ToHex(color) + Environment.NewLine + Strings.Color_RecentTip);
            swatch.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(swatch).Properties.IsRightButtonPressed) Editor.BackgroundColor = color;
                else Editor.ForegroundColor = color;
                e.Handled = true;
            };
            _recent.Children.Add(swatch);
        }
    }
}