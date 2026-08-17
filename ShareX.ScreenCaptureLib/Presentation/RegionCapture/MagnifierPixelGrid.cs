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
using Avalonia.VisualTree;
using System;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

/// <summary>Static pixel grid and center-cell outline for the region magnifier.</summary>
public sealed class MagnifierPixelGrid : Control
{
    public static readonly StyledProperty<int> PixelCountProperty =
        AvaloniaProperty.Register<MagnifierPixelGrid, int>(nameof(PixelCount), 15);

    private static readonly IBrush GridBrush = new SolidColorBrush(Color.FromArgb(75, 0, 0, 0));

    static MagnifierPixelGrid()
    {
        AffectsRender<MagnifierPixelGrid>(PixelCountProperty);
    }

    public int PixelCount
    {
        get => GetValue(PixelCountProperty);
        set => SetValue(PixelCountProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        int count = Math.Max(1, PixelCount);
        if (Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            return;
        }

        TopLevel topLevel = TopLevel.GetTopLevel(this);
        double scale = topLevel?.RenderScaling ?? 1;
        if (!double.IsFinite(scale) || scale <= 0)
        {
            scale = 1;
        }

        Point origin = topLevel != null ? this.TranslatePoint(default, topLevel) ?? default : default;
        double originPhysicalX = origin.X * scale;
        double originPhysicalY = origin.Y * scale;
        int left = SnapPhysical(originPhysicalX);
        int top = SnapPhysical(originPhysicalY);
        int right = SnapPhysical(originPhysicalX + Bounds.Width * scale);
        int bottom = SnapPhysical(originPhysicalY + Bounds.Height * scale);

        for (int index = 1; index < count; index++)
        {
            int x = SnapPhysical(originPhysicalX + index * Bounds.Width * scale / count);
            int y = SnapPhysical(originPhysicalY + index * Bounds.Height * scale / count);
            DrawPhysicalRectangle(context, GridBrush, x - 1, top, x, bottom, originPhysicalX, originPhysicalY, scale);
            DrawPhysicalRectangle(context, GridBrush, left, y - 1, right, y, originPhysicalX, originPhysicalY, scale);
        }

        int centerIndex = count / 2;
        int centerLeft = SnapPhysical(originPhysicalX + centerIndex * Bounds.Width * scale / count);
        int centerTop = SnapPhysical(originPhysicalY + centerIndex * Bounds.Height * scale / count);
        int centerRight = SnapPhysical(originPhysicalX + (centerIndex + 1) * Bounds.Width * scale / count);
        int centerBottom = SnapPhysical(originPhysicalY + (centerIndex + 1) * Bounds.Height * scale / count);

        // Grid lines occupy the physical pixel immediately before each cell boundary.
        // Start the center outline there too, matching the legacy magnifier while
        // keeping its already-correct right and bottom edges unchanged.
        centerLeft--;
        centerTop--;

        DrawPhysicalOutline(
            context,
            Brushes.Black,
            centerLeft,
            centerTop,
            centerRight,
            centerBottom,
            originPhysicalX,
            originPhysicalY,
            scale);

        DrawPhysicalOutline(
            context,
            Brushes.White,
            centerLeft + 1,
            centerTop + 1,
            centerRight - 1,
            centerBottom - 1,
            originPhysicalX,
            originPhysicalY,
            scale);
    }

    private static int SnapPhysical(double value) =>
        (int)Math.Round(value, MidpointRounding.AwayFromZero);

    private static void DrawPhysicalOutline(
        DrawingContext context,
        IBrush brush,
        int left,
        int top,
        int right,
        int bottom,
        double originPhysicalX,
        double originPhysicalY,
        double scale)
    {
        if (right <= left || bottom <= top)
        {
            return;
        }

        DrawPhysicalRectangle(context, brush, left, top, right, top + 1, originPhysicalX, originPhysicalY, scale);
        DrawPhysicalRectangle(context, brush, left, bottom - 1, right, bottom, originPhysicalX, originPhysicalY, scale);
        DrawPhysicalRectangle(context, brush, left, top + 1, left + 1, bottom - 1, originPhysicalX, originPhysicalY, scale);
        DrawPhysicalRectangle(context, brush, right - 1, top + 1, right, bottom - 1, originPhysicalX, originPhysicalY, scale);
    }

    private static void DrawPhysicalRectangle(
        DrawingContext context,
        IBrush brush,
        int left,
        int top,
        int right,
        int bottom,
        double originPhysicalX,
        double originPhysicalY,
        double scale)
    {
        if (right <= left || bottom <= top)
        {
            return;
        }

        Rect rectangle = new(
            (left - originPhysicalX) / scale,
            (top - originPhysicalY) / scale,
            (right - left) / scale,
            (bottom - top) / scale);
        context.DrawRectangle(brush, null, rectangle);
    }
}
