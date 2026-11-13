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
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PixelEditor.UI;

/// <summary>Small helpers for building the code-only interface.</summary>
public static class Ui
{
    public static readonly IBrush Chrome = new SolidColorBrush(Color.FromRgb(0x32, 0x32, 0x32));
    public static readonly IBrush Deep = new SolidColorBrush(Color.FromRgb(0x26, 0x26, 0x26));
    public static readonly IBrush Strip = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28));
    public static readonly IBrush Gutter = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E));
    public static readonly IBrush Divider = new SolidColorBrush(Color.FromRgb(0x47, 0x47, 0x47));
    public static readonly IBrush Hover = new SolidColorBrush(Color.FromRgb(0x3E, 0x3E, 0x3E));
    public static readonly IBrush RowSelected = new SolidColorBrush(Color.FromRgb(0x4B, 0x4B, 0x4B));
    public static readonly IBrush TextBrush = new SolidColorBrush(Color.FromRgb(0xD9, 0xD9, 0xD9));
    public static readonly IBrush DimBrush = new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E));

    public static IBrush Brush(uint color) => new SolidColorBrush(Argb.ToAvalonia(color));

    public static Button IconButton(string icon, string tip, Action click, double size = 16)
    {
        Button button = new() { Classes = { "icon" }, Content = new IconView(icon, size), Padding = new Thickness(5), Width = size + 12, Height = size + 10, HorizontalContentAlignment = HorizontalAlignment.Center };
        ToolTip.SetTip(button, tip);
        button.Click += (_, _) => click();
        return button;
    }

    public static TextBlock Text(string text, bool dim = false, double size = 12) =>
        new() { Text = text, FontSize = size, VerticalAlignment = VerticalAlignment.Center, Foreground = dim ? DimBrush : TextBrush };

    public static StackPanel Row(double spacing, params Control[] children)
    {
        StackPanel panel = new() { Orientation = Orientation.Horizontal, Spacing = spacing };
        foreach (Control child in children) panel.Children.Add(child);
        return panel;
    }

    /// <summary>A tiling checkerboard brush for transparent areas.</summary>
    public static IBrush Checker(uint color1, uint color2, double cell)
    {
        // Vector tiles stay sharp at every zoom level, unlike a scaled bitmap.
        GeometryGroup squares = new();
        squares.Children.Add(new RectangleGeometry(new Rect(1, 0, 1, 1)));
        squares.Children.Add(new RectangleGeometry(new Rect(0, 1, 1, 1)));
        DrawingGroup drawing = new();
        drawing.Children.Add(new GeometryDrawing { Brush = Brush(Argb.Opaque(color1)), Geometry = new RectangleGeometry(new Rect(0, 0, 2, 2)) });
        drawing.Children.Add(new GeometryDrawing { Brush = Brush(Argb.Opaque(color2)), Geometry = squares });
        return new DrawingBrush(drawing)
        {
            TileMode = TileMode.Tile,
            Stretch = Stretch.Fill,
            DestinationRect = new RelativeRect(0, 0, cell * 2, cell * 2, RelativeUnit.Absolute)
        };
    }

    /// <summary>Copies a buffer into an Avalonia bitmap (premultiplying), optionally reusing a bitmap of the same size.</summary>
    public static unsafe WriteableBitmap ToBitmap(PixelBuffer buffer, WriteableBitmap? reuse = null)
    {
        WriteableBitmap bitmap = reuse != null && reuse.PixelSize.Width == buffer.Width && reuse.PixelSize.Height == buffer.Height
            ? reuse
            : new WriteableBitmap(new PixelSize(buffer.Width, buffer.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        using ILockedFramebuffer frame = bitmap.Lock();
        for (int y = 0; y < buffer.Height; y++)
        {
            uint* row = (uint*)((byte*)frame.Address + y * frame.RowBytes);
            int source = y * buffer.Width;
            for (int x = 0; x < buffer.Width; x++) row[x] = Argb.Premultiply(buffer.Pixels[source + x]);
        }
        return bitmap;
    }

    /// <summary>A color swatch on a checkerboard, so transparent colors read as such.</summary>
    public static Border Swatch(uint color, double size)
    {
        return new Border
        {
            Width = size,
            Height = size,
            Background = Checker(0xFFFFFFFF, 0xFFC8C8C8, Math.Max(2, size / 4)),
            Child = new Border { Background = Brush(color) }
        };
    }

    public static async Task<uint?> PickColorAsync(Window owner, uint initial, string title)
    {
        SKColor? picked = await ColorPickerDialog.ShowAsync(owner, Argb.ToSK(initial), title);
        return picked is SKColor color ? Argb.FromSK(color) : null;
    }
}

/// <summary>Fills the options bar of the active tool.</summary>
public sealed class OptionsBuilder
{
    private readonly PixelEditorWindow _editor;

    public OptionsBuilder(PixelEditorWindow editor)
    {
        _editor = editor;
    }

    public StackPanel Root { get; } = new() { Orientation = Orientation.Horizontal, Spacing = 6, VerticalAlignment = VerticalAlignment.Center };

    public void Label(string text) => Root.Children.Add(new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center, Classes = { "dim" } });

    public void Separator() => Root.Children.Add(new Border { Classes = { "vsep" } });

    public void Number(string label, int min, int max, int value, Action<int> changed, string? suffix = null)
    {
        Root.Children.Add(new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center });
        NumericUpDown box = new()
        {
            Minimum = min,
            Maximum = max,
            Value = Math.Clamp(value, min, max),
            Increment = 1,
            FormatString = "0",
            Width = max >= 1000 || min < 0 ? 60 : 46,
            VerticalAlignment = VerticalAlignment.Center
        };
        box.ValueChanged += (_, e) =>
        {
            if (e.NewValue is decimal v)
            {
                changed((int)v);
                _editor.Canvas.InvalidateVisual();
            }
        };
        Root.Children.Add(box);
        if (suffix != null) Root.Children.Add(new TextBlock { Text = suffix, VerticalAlignment = VerticalAlignment.Center, Classes = { "dim" }, Margin = new Thickness(-3, 0, 2, 0) });
    }

    public void Check(string label, bool value, Action<bool> changed, string? tip = null)
    {
        CheckBox box = new() { Content = label, IsChecked = value, VerticalAlignment = VerticalAlignment.Center };
        if (tip != null) ToolTip.SetTip(box, tip);
        box.IsCheckedChanged += (_, _) => changed(box.IsChecked == true);
        Root.Children.Add(box);
    }

    public void Combo<T>(string label, IEnumerable<(string Text, T Value)> items, T value, Action<T> changed)
    {
        Root.Children.Add(new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center });
        ComboBox combo = DialogBuilder.Combo(items, value, changed);
        combo.MinWidth = 96;
        combo.VerticalAlignment = VerticalAlignment.Center;
        Root.Children.Add(combo);
    }

    /// <summary>A row of icon buttons of which one is active.</summary>
    public void Choice<T>(IEnumerable<(string Icon, string Tip, T Value)> items, T current, Action<T> changed)
    {
        List<(Button Button, T Value)> buttons = [];
        StackPanel group = new() { Orientation = Orientation.Horizontal, Spacing = 1, VerticalAlignment = VerticalAlignment.Center };
        foreach ((string icon, string tip, T value) in items)
        {
            Button button = new() { Classes = { "icon" }, Content = new IconView(icon, 16), Padding = new Thickness(4), Width = 26, Height = 24, HorizontalContentAlignment = HorizontalAlignment.Center };
            ToolTip.SetTip(button, tip);
            button.Classes.Set("checked", Equals(value, current));
            T captured = value;
            button.Click += (_, _) =>
            {
                changed(captured);
                foreach ((Button other, T otherValue) in buttons) other.Classes.Set("checked", Equals(otherValue, captured));
                _editor.Canvas.InvalidateVisual();
            };
            buttons.Add((button, value));
            group.Children.Add(button);
        }
        Root.Children.Add(group);
    }

    public Button Toggle(string icon, string tip, bool value, Action<bool> changed)
    {
        Button button = new() { Classes = { "icon" }, Content = new IconView(icon, 16), Padding = new Thickness(4), Width = 26, Height = 24, HorizontalContentAlignment = HorizontalAlignment.Center };
        ToolTip.SetTip(button, tip);
        button.Classes.Set("checked", value);
        button.Click += (_, _) =>
        {
            bool now = !button.Classes.Contains("checked");
            button.Classes.Set("checked", now);
            changed(now);
        };
        Root.Children.Add(button);
        return button;
    }

    public void Button(string text, Action click, bool primary = false)
    {
        Button button = new() { Content = text, VerticalAlignment = VerticalAlignment.Center };
        if (primary) button.BorderBrush = ShareX.PhotoEditor.UI.Theme.PhotoEditorTheme.AccentBorderBrush;
        button.Click += (_, _) => click();
        Root.Children.Add(button);
    }

    public void IconButton(string icon, string tip, Action click)
    {
        Button button = Ui.IconButton(icon, tip, click);
        button.VerticalAlignment = VerticalAlignment.Center;
        Root.Children.Add(button);
    }

    public void ColorButton(string label, uint color, Action<uint> changed)
    {
        Root.Children.Add(new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center });
        Border swatch = Ui.Swatch(color, 18);
        swatch.BorderBrush = Ui.Gutter;
        swatch.BorderThickness = new Thickness(1);
        swatch.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand);
        swatch.PointerPressed += async (_, _) =>
        {
            uint? picked = await Ui.PickColorAsync(_editor, color, label);
            if (picked is uint c) changed(c);
        };
        Root.Children.Add(swatch);
    }
}
