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

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

/// <summary>Static pixel grid and center-cell outline for the region magnifier.</summary>
public sealed class MagnifierPixelGrid : Control
{
    public static readonly StyledProperty<int> PixelCountProperty =
        AvaloniaProperty.Register<MagnifierPixelGrid, int>(nameof(PixelCount), 15);

    private static readonly IPen GridPen = new Pen(
        new SolidColorBrush(Color.FromArgb(75, 0, 0, 0)),
        1);
    private static readonly IPen CenterOuterPen = new Pen(Brushes.Black, 1);
    private static readonly IPen CenterInnerPen = new Pen(Brushes.White, 1);

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

        double cellWidth = Bounds.Width / count;
        double cellHeight = Bounds.Height / count;

        for (int index = 1; index < count; index++)
        {
            double x = index * cellWidth;
            double y = index * cellHeight;
            context.DrawLine(GridPen, new Point(x, 0), new Point(x, Bounds.Height));
            context.DrawLine(GridPen, new Point(0, y), new Point(Bounds.Width, y));
        }

        int centerIndex = count / 2;
        double centerLeft = centerIndex * cellWidth;
        double centerTop = centerIndex * cellHeight;
        Rect centerCell = new(
            centerLeft + 0.5,
            centerTop + 0.5,
            Math.Max(0, cellWidth - 1),
            Math.Max(0, cellHeight - 1));
        context.DrawRectangle(null, CenterOuterPen, centerCell);

        if (Math.Min(cellWidth, cellHeight) >= 6)
        {
            Rect innerCell = centerCell.Deflate(1);
            context.DrawRectangle(null, CenterInnerPen, innerCell);
        }
    }
}
