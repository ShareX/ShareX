#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace ShareX;

public sealed class AboutLogoAnimationControl : Control
{
    private readonly DispatcherTimer _timer;
    private int _step = 10;
    private int _direction = 1;
    private double _hue;
    private bool _isPaused;

    public AboutLogoAnimationControl()
    {
        _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(50), DispatcherPriority.Render, OnTick);
    }

    public void Start()
    {
        _timer.Start();
        InvalidateVisual();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _isPaused = !_isPaused;
        TaskHelpers.PlayNotificationSoundAsync(NotificationSound.ActionCompleted);
        e.Handled = true;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        double width = Bounds.Width;
        double height = Bounds.Height;
        double halfWidth = width / 2;
        double halfHeight = height / 2;
        Pen pen = new(new SolidColorBrush(ColorFromHsv(_hue, 1, 0.9)), 2);

        for (int i = 0; i <= halfWidth; i += _step)
        {
            DrawRotatedLine(context, pen, new Point(i, halfHeight), new Point(halfWidth, halfHeight - i));
            DrawRotatedLine(context, pen, new Point(halfWidth, i), new Point(halfWidth + i, halfHeight));
            DrawRotatedLine(context, pen, new Point(width - i, halfHeight), new Point(halfWidth, halfHeight + i));
            DrawRotatedLine(context, pen, new Point(halfWidth, height - i), new Point(halfWidth - i, halfHeight));
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _timer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (!_isPaused)
        {
            if (_step + _direction > 35)
            {
                _direction = -1;
            }
            else if (_step + _direction < 3)
            {
                _direction = 1;
            }

            _step += _direction;
            _hue = (_hue + 3.6) % 360;
        }

        InvalidateVisual();
    }

    private void DrawRotatedLine(DrawingContext context, Pen pen, Point start, Point end)
    {
        Point center = new(Bounds.Width / 2, Bounds.Height / 2);
        context.DrawLine(pen, Rotate(start, center), Rotate(end, center));
    }

    private static Point Rotate(Point point, Point center)
    {
        const double angle = Math.PI / 4;
        double x = point.X - center.X;
        double y = point.Y - center.Y;
        return new Point(
            center.X + x * Math.Cos(angle) - y * Math.Sin(angle),
            center.Y + x * Math.Sin(angle) + y * Math.Cos(angle));
    }

    private static Color ColorFromHsv(double hue, double saturation, double value)
    {
        double chroma = value * saturation;
        double x = chroma * (1 - Math.Abs(hue / 60 % 2 - 1));
        double match = value - chroma;
        (double red, double green, double blue) = hue switch
        {
            < 60 => (chroma, x, 0d),
            < 120 => (x, chroma, 0d),
            < 180 => (0d, chroma, x),
            < 240 => (0d, x, chroma),
            < 300 => (x, 0d, chroma),
            _ => (chroma, 0d, x)
        };

        return Color.FromRgb(
            (byte)Math.Round((red + match) * 255),
            (byte)Math.Round((green + match) * 255),
            (byte)Math.Round((blue + match) * 255));
    }
}
