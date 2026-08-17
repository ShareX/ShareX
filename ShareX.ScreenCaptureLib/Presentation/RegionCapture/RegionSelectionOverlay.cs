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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

/// <summary>
/// Event-driven region chrome. The frozen screenshot and annotations are rendered by the
/// shared editor workspace; this control only redraws when hover or selection state changes.
/// </summary>
public sealed class RegionSelectionOverlay : Control
{
    public static readonly StyledProperty<IBrush> AccentBrushProperty =
        AvaloniaProperty.Register<RegionSelectionOverlay, IBrush>(nameof(AccentBrush), Brushes.DodgerBlue);

    private static readonly IPen AntDashPen = new Pen(
        new SolidColorBrush(Color.FromArgb(230, 0, 0, 0)),
        1,
        new DashStyle([5, 5], 0));
    private static readonly IBrush HandleFill = Brushes.White;
    private static readonly IPen HandleBorder = new Pen(Brushes.Black, 1);

    private Rect _selectionRectangle;
    private Rect _hoverRectangle;
    private bool _showHandles;
    private byte _dimAlpha = 51;
    private IBrush _dimBrush = new SolidColorBrush(Color.FromArgb(51, 0, 0, 0));

    static RegionSelectionOverlay()
    {
        AffectsRender<RegionSelectionOverlay>(AccentBrushProperty);
    }

    public IBrush AccentBrush
    {
        get => GetValue(AccentBrushProperty);
        set => SetValue(AccentBrushProperty, value);
    }

    public Rect SelectionRectangle
    {
        get => _selectionRectangle;
        set
        {
            if (_selectionRectangle != value)
            {
                _selectionRectangle = value;
                InvalidateVisual();
            }
        }
    }

    public Rect HoverRectangle
    {
        get => _hoverRectangle;
        set
        {
            if (_hoverRectangle != value)
            {
                _hoverRectangle = value;
                InvalidateVisual();
            }
        }
    }

    public bool ShowHandles
    {
        get => _showHandles;
        set
        {
            if (_showHandles != value)
            {
                _showHandles = value;
                InvalidateVisual();
            }
        }
    }

    public byte DimAlpha
    {
        get => _dimAlpha;
        set
        {
            if (_dimAlpha != value)
            {
                _dimAlpha = value;
                _dimBrush = new SolidColorBrush(Color.FromArgb(value, 0, 0, 0));
                InvalidateVisual();
            }
        }
    }

    public Rect ActiveRectangle => IsValid(SelectionRectangle) ? SelectionRectangle : HoverRectangle;

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        Rect surface = new Rect(Bounds.Size);
        Rect active = Intersect(ActiveRectangle, surface);

        if (DimAlpha > 0)
        {
            if (IsValid(active))
            {
                DrawDimmedOutside(context, surface, active, _dimBrush);
            }
            else
            {
                context.DrawRectangle(_dimBrush, null, surface);
            }
        }

        if (!IsValid(active))
        {
            return;
        }

        Rect antRectangle = new(
            active.X + 0.5,
            active.Y + 0.5,
            Math.Max(0, active.Width - 1),
            Math.Max(0, active.Height - 1));
        DrawAntRectangle(context, antRectangle);

        if (ShowHandles && IsValid(SelectionRectangle))
        {
            foreach (Point point in GetHandlePoints(SelectionRectangle))
            {
                const double size = 8;
                Rect handle = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
                context.DrawRectangle(HandleFill, HandleBorder, handle);
            }
        }
    }

    private void DrawAntRectangle(DrawingContext context, Rect rectangle)
    {
        context.DrawRectangle(null, new Pen(AccentBrush, 1), rectangle);
        context.DrawLine(AntDashPen, rectangle.TopLeft, rectangle.TopRight);
        context.DrawLine(AntDashPen, rectangle.TopLeft, rectangle.BottomLeft);
        context.DrawLine(AntDashPen, rectangle.TopRight, rectangle.BottomRight);
        context.DrawLine(AntDashPen, rectangle.BottomLeft, rectangle.BottomRight);
    }

    public RegionResizeHandle HitTestHandle(Point point, double tolerance = 10)
    {
        if (!IsValid(SelectionRectangle))
        {
            return RegionResizeHandle.None;
        }

        Point[] points = GetHandlePoints(SelectionRectangle).ToArray();
        RegionResizeHandle[] handles =
        [
            RegionResizeHandle.TopLeft,
            RegionResizeHandle.Top,
            RegionResizeHandle.TopRight,
            RegionResizeHandle.Right,
            RegionResizeHandle.BottomRight,
            RegionResizeHandle.Bottom,
            RegionResizeHandle.BottomLeft,
            RegionResizeHandle.Left
        ];

        for (int i = 0; i < points.Length; i++)
        {
            if (Math.Abs(point.X - points[i].X) <= tolerance && Math.Abs(point.Y - points[i].Y) <= tolerance)
            {
                return handles[i];
            }
        }

        return RegionResizeHandle.None;
    }

    private static void DrawDimmedOutside(DrawingContext context, Rect surface, Rect clear, IBrush brush)
    {
        DrawIfValid(context, brush, new Rect(surface.Left, surface.Top, surface.Width, clear.Top - surface.Top));
        DrawIfValid(context, brush, new Rect(surface.Left, clear.Top, clear.Left - surface.Left, clear.Height));
        DrawIfValid(context, brush, new Rect(clear.Right, clear.Top, surface.Right - clear.Right, clear.Height));
        DrawIfValid(context, brush, new Rect(surface.Left, clear.Bottom, surface.Width, surface.Bottom - clear.Bottom));
    }

    private static void DrawIfValid(DrawingContext context, IBrush brush, Rect rectangle)
    {
        if (IsValid(rectangle))
        {
            context.DrawRectangle(brush, null, rectangle);
        }
    }

    private static IEnumerable<Point> GetHandlePoints(Rect rectangle)
    {
        yield return rectangle.TopLeft;
        yield return new Point(rectangle.Center.X, rectangle.Top);
        yield return rectangle.TopRight;
        yield return new Point(rectangle.Right, rectangle.Center.Y);
        yield return rectangle.BottomRight;
        yield return new Point(rectangle.Center.X, rectangle.Bottom);
        yield return rectangle.BottomLeft;
        yield return new Point(rectangle.Left, rectangle.Center.Y);
    }

    internal static Rect NormalizeAndClamp(Point first, Point second, Size bounds)
    {
        double left = Math.Clamp(Math.Min(first.X, second.X), 0, bounds.Width);
        double top = Math.Clamp(Math.Min(first.Y, second.Y), 0, bounds.Height);
        double right = Math.Clamp(Math.Max(first.X, second.X), 0, bounds.Width);
        double bottom = Math.Clamp(Math.Max(first.Y, second.Y), 0, bounds.Height);
        return new Rect(left, top, Math.Max(0, right - left), Math.Max(0, bottom - top));
    }

    internal static Rect Intersect(Rect rectangle, Rect bounds)
    {
        double left = Math.Max(rectangle.Left, bounds.Left);
        double top = Math.Max(rectangle.Top, bounds.Top);
        double right = Math.Min(rectangle.Right, bounds.Right);
        double bottom = Math.Min(rectangle.Bottom, bounds.Bottom);
        return right > left && bottom > top
            ? new Rect(left, top, right - left, bottom - top)
            : default;
    }

    internal static bool IsValid(Rect rectangle) => rectangle.Width > 0 && rectangle.Height > 0;
}

public enum RegionResizeHandle
{
    None,
    TopLeft,
    Top,
    TopRight,
    Right,
    BottomRight,
    Bottom,
    BottomLeft,
    Left
}
