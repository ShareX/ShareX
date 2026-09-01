#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
    This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License.
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using DrawingColor = System.Drawing.Color;

namespace ShareX.HelpersLib;

public sealed class AdvancedColorPickerControl : Control
{
    private const double SliderWidth = 30;
    private const double Gap = 12;
    private WriteableBitmap? _boxBitmap;
    private WriteableBitmap? _sliderBitmap;
    private DrawingColor _selectedColor = DrawingColor.Red;
    private DrawStyle _drawStyle = DrawStyle.Hue;
    private bool _draggingSlider;

    public event EventHandler<DrawingColor>? ColorChanged;

    public DrawingColor SelectedColor
    {
        get => _selectedColor;
        set
        {
            if (_selectedColor.ToArgb() == value.ToArgb()) return;
            _selectedColor = value;
            RebuildBitmaps();
            InvalidateVisual();
        }
    }

    public DrawStyle DrawStyle
    {
        get => _drawStyle;
        set
        {
            if (_drawStyle == value) return;
            _drawStyle = value;
            RebuildBitmaps();
            InvalidateVisual();
        }
    }

    public AdvancedColorPickerControl()
    {
        MinWidth = 300;
        MinHeight = 260;
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double width = double.IsInfinity(availableSize.Width) ? 342 : availableSize.Width;
        double height = double.IsInfinity(availableSize.Height) ? 300 : availableSize.Height;
        return new Size(Math.Max(300, width), Math.Max(260, height));
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        RebuildBitmaps();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        (Rect box, Rect slider) = GetRegions();
        if (_boxBitmap == null || _sliderBitmap == null) RebuildBitmaps();

        if (_boxBitmap != null)
        {
            context.DrawImage(_boxBitmap, new Rect(_boxBitmap.Size), box);
        }
        if (_sliderBitmap != null)
        {
            context.DrawImage(_sliderBitmap, new Rect(_sliderBitmap.Size), slider);
        }

        Pen border = new(new SolidColorBrush(Color.FromRgb(95, 95, 95)), 1);
        context.DrawRectangle(null, border, box);
        context.DrawRectangle(null, border, slider);

        Point boxPoint = GetBoxPoint(box);
        context.DrawEllipse(null, new Pen(Brushes.Black, 3), boxPoint, 6, 6);
        context.DrawEllipse(null, new Pen(Brushes.White, 1.5), boxPoint, 6, 6);

        double sliderY = GetSliderY(slider);
        Rect marker = new(slider.X - 3, sliderY - 5, slider.Width + 6, 10);
        context.DrawRectangle(null, new Pen(Brushes.Black, 3), marker);
        context.DrawRectangle(null, new Pen(Brushes.White, 1), marker);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        Point point = e.GetPosition(this);
        (Rect box, Rect slider) = GetRegions();
        if (!box.Contains(point) && !slider.Contains(point)) return;
        _draggingSlider = slider.Contains(point);
        e.Pointer.Capture(this);
        UpdateFromPointer(point);
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.Pointer.Captured == this) UpdateFromPointer(e.GetPosition(this));
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Pointer.Captured != this) return;
        UpdateFromPointer(e.GetPosition(this));
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    private void UpdateFromPointer(Point point)
    {
        (Rect box, Rect slider) = GetRegions();
        double x = Clamp01((point.X - box.X) / Math.Max(1, box.Width - 1));
        double y = Clamp01((point.Y - box.Y) / Math.Max(1, box.Height - 1));
        double sy = Clamp01((point.Y - slider.Y) / Math.Max(1, slider.Height - 1));
        int alpha = _selectedColor.A;
        HSB hsb = _selectedColor;

        DrawingColor color = _drawStyle switch
        {
            DrawStyle.Hue when _draggingSlider => FromHsv(1 - sy, hsb.Saturation, hsb.Brightness, alpha),
            DrawStyle.Hue => FromHsv(hsb.Hue, x, 1 - y, alpha),
            DrawStyle.Saturation when _draggingSlider => FromHsv(hsb.Hue, 1 - sy, hsb.Brightness, alpha),
            DrawStyle.Saturation => FromHsv(x, hsb.Saturation, 1 - y, alpha),
            DrawStyle.Brightness when _draggingSlider => FromHsv(hsb.Hue, hsb.Saturation, 1 - sy, alpha),
            DrawStyle.Brightness => FromHsv(x, 1 - y, hsb.Brightness, alpha),
            DrawStyle.Red when _draggingSlider => DrawingColor.FromArgb(alpha, ToByte(1 - sy), _selectedColor.G, _selectedColor.B),
            DrawStyle.Red => DrawingColor.FromArgb(alpha, _selectedColor.R, ToByte(1 - y), ToByte(x)),
            DrawStyle.Green when _draggingSlider => DrawingColor.FromArgb(alpha, _selectedColor.R, ToByte(1 - sy), _selectedColor.B),
            DrawStyle.Green => DrawingColor.FromArgb(alpha, ToByte(1 - y), _selectedColor.G, ToByte(x)),
            DrawStyle.Blue when _draggingSlider => DrawingColor.FromArgb(alpha, _selectedColor.R, _selectedColor.G, ToByte(1 - sy)),
            _ => DrawingColor.FromArgb(alpha, ToByte(x), ToByte(1 - y), _selectedColor.B)
        };

        _selectedColor = color;
        RebuildBitmaps();
        InvalidateVisual();
        ColorChanged?.Invoke(this, color);
    }

    private (Rect Box, Rect Slider) GetRegions()
    {
        double boxWidth = Math.Max(1, Bounds.Width - SliderWidth - Gap);
        return (new Rect(0, 0, boxWidth, Bounds.Height), new Rect(boxWidth + Gap, 0, SliderWidth, Bounds.Height));
    }

    private Point GetBoxPoint(Rect box)
    {
        HSB hsb = _selectedColor;
        return _drawStyle switch
        {
            DrawStyle.Hue => new Point(box.X + hsb.Saturation * box.Width, box.Y + (1 - hsb.Brightness) * box.Height),
            DrawStyle.Saturation => new Point(box.X + hsb.Hue * box.Width, box.Y + (1 - hsb.Brightness) * box.Height),
            DrawStyle.Brightness => new Point(box.X + hsb.Hue * box.Width, box.Y + (1 - hsb.Saturation) * box.Height),
            DrawStyle.Red => new Point(box.X + _selectedColor.B / 255d * box.Width, box.Y + (1 - _selectedColor.G / 255d) * box.Height),
            DrawStyle.Green => new Point(box.X + _selectedColor.B / 255d * box.Width, box.Y + (1 - _selectedColor.R / 255d) * box.Height),
            _ => new Point(box.X + _selectedColor.R / 255d * box.Width, box.Y + (1 - _selectedColor.G / 255d) * box.Height)
        };
    }

    private double GetSliderY(Rect slider)
    {
        HSB hsb = _selectedColor;
        double value = _drawStyle switch
        {
            DrawStyle.Hue => hsb.Hue,
            DrawStyle.Saturation => hsb.Saturation,
            DrawStyle.Brightness => hsb.Brightness,
            DrawStyle.Red => _selectedColor.R / 255d,
            DrawStyle.Green => _selectedColor.G / 255d,
            _ => _selectedColor.B / 255d
        };
        return slider.Y + (1 - value) * slider.Height;
    }

    private void RebuildBitmaps()
    {
        (Rect box, Rect slider) = GetRegions();
        int boxWidth = Math.Max(1, (int)Math.Round(box.Width));
        int height = Math.Max(1, (int)Math.Round(box.Height));
        int sliderWidth = Math.Max(1, (int)Math.Round(slider.Width));
        _boxBitmap?.Dispose();
        _sliderBitmap?.Dispose();
        _boxBitmap = CreateBitmap(boxWidth, height, false);
        _sliderBitmap = CreateBitmap(sliderWidth, height, true);
    }

    private unsafe WriteableBitmap CreateBitmap(int width, int height, bool slider)
    {
        WriteableBitmap bitmap = new(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
        using ILockedFramebuffer buffer = bitmap.Lock();
        HSB selectedHsb = _selectedColor;

        for (int y = 0; y < height; y++)
        {
            uint* row = (uint*)((byte*)buffer.Address + y * buffer.RowBytes);
            double ny = y / (double)Math.Max(1, height - 1);
            for (int x = 0; x < width; x++)
            {
                double nx = x / (double)Math.Max(1, width - 1);
                DrawingColor color = GetPixelColor(nx, ny, slider, selectedHsb);
                row[x] = 0xFF000000u | ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;
            }
        }
        return bitmap;
    }

    private DrawingColor GetPixelColor(double x, double y, bool slider, HSB hsb)
    {
        if (slider)
        {
            return _drawStyle switch
            {
                DrawStyle.Hue => FromHsv(1 - y, 1, 1),
                DrawStyle.Saturation => FromHsv(hsb.Hue, 1 - y, hsb.Brightness),
                DrawStyle.Brightness => FromHsv(hsb.Hue, hsb.Saturation, 1 - y),
                DrawStyle.Red => DrawingColor.FromArgb(ToByte(1 - y), _selectedColor.G, _selectedColor.B),
                DrawStyle.Green => DrawingColor.FromArgb(_selectedColor.R, ToByte(1 - y), _selectedColor.B),
                _ => DrawingColor.FromArgb(_selectedColor.R, _selectedColor.G, ToByte(1 - y))
            };
        }

        return _drawStyle switch
        {
            DrawStyle.Hue => FromHsv(hsb.Hue, x, 1 - y),
            DrawStyle.Saturation => FromHsv(x, hsb.Saturation, 1 - y),
            DrawStyle.Brightness => FromHsv(x, 1 - y, hsb.Brightness),
            DrawStyle.Red => DrawingColor.FromArgb(_selectedColor.R, ToByte(1 - y), ToByte(x)),
            DrawStyle.Green => DrawingColor.FromArgb(ToByte(1 - y), _selectedColor.G, ToByte(x)),
            _ => DrawingColor.FromArgb(ToByte(x), ToByte(1 - y), _selectedColor.B)
        };
    }

    private static DrawingColor FromHsv(double hue, double saturation, double value, int alpha = 255)
    {
        hue = ((hue % 1) + 1) % 1;
        saturation = Clamp01(saturation);
        value = Clamp01(value);
        double scaled = hue * 6;
        int sector = (int)Math.Floor(scaled) % 6;
        double fraction = scaled - Math.Floor(scaled);
        double p = value * (1 - saturation);
        double q = value * (1 - fraction * saturation);
        double t = value * (1 - (1 - fraction) * saturation);
        (double r, double g, double b) = sector switch
        {
            0 => (value, t, p),
            1 => (q, value, p),
            2 => (p, value, t),
            3 => (p, q, value),
            4 => (t, p, value),
            _ => (value, p, q)
        };
        return DrawingColor.FromArgb(alpha, ToByte(r), ToByte(g), ToByte(b));
    }

    private static byte ToByte(double value) => (byte)Math.Clamp((int)Math.Round(Clamp01(value) * 255), 0, 255);
    private static double Clamp01(double value) => Math.Clamp(value, 0, 1);
}
