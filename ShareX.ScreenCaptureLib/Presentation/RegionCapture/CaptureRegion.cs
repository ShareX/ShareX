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

#nullable enable

using Avalonia;
using Avalonia.Media;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

/// <summary>The shape that a region capture tool selects.</summary>
public enum RegionShape
{
    Rectangle,
    Ellipse,
    Freehand
}

/// <summary>
/// A selected region in screenshot pixel coordinates. <see cref="Bounds"/> is the rectangle that is captured; an
/// ellipse is inscribed in it and a freehand region is the polygon through <see cref="Points"/>.
/// </summary>
internal readonly record struct CaptureRegion(RegionShape Shape, Rect Bounds, IReadOnlyList<Point>? Points = null)
{
    public static CaptureRegion FromRectangle(Rect bounds) => new(RegionShape.Rectangle, bounds);

    public bool IsRectangle => Shape == RegionShape.Rectangle;

    public bool Contains(Point point)
    {
        if (!Bounds.Contains(point))
        {
            return false;
        }

        switch (Shape)
        {
            case RegionShape.Ellipse:
                double radiusX = Bounds.Width / 2;
                double radiusY = Bounds.Height / 2;
                double dx = (point.X - Bounds.Center.X) / radiusX;
                double dy = (point.Y - Bounds.Center.Y) / radiusY;
                return dx * dx + dy * dy <= 1;
            case RegionShape.Freehand:
                return Points is { Count: >= 3 } points && PolygonContains(points, point);
            default:
                return true;
        }
    }

    /// <summary>Returns the region moved by the offset. A freehand region moves its points with its bounds.</summary>
    public CaptureRegion Offset(double dx, double dy)
    {
        if (dx == 0 && dy == 0)
        {
            return this;
        }

        Point[]? points = null;
        if (Points != null)
        {
            points = new Point[Points.Count];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Point(Points[i].X + dx, Points[i].Y + dy);
            }
        }

        return this with { Bounds = Bounds.Translate(new Vector(dx, dy)), Points = points };
    }

    /// <summary>Creates the outline of the region, or null for a freehand region with fewer than two points.</summary>
    public Geometry? CreateGeometry()
    {
        switch (Shape)
        {
            case RegionShape.Ellipse:
                return new EllipseGeometry(Bounds);
            case RegionShape.Freehand:
                if (Points is not { Count: >= 2 } points)
                {
                    return null;
                }

                StreamGeometry geometry = new();
                using (StreamGeometryContext context = geometry.Open())
                {
                    context.SetFillRule(FillRule.EvenOdd);
                    context.BeginFigure(points[0], true);
                    for (int i = 1; i < points.Count; i++)
                    {
                        context.LineTo(points[i]);
                    }

                    context.EndFigure(true);
                }

                return geometry;
            default:
                return new RectangleGeometry(Bounds);
        }
    }

    /// <summary>Creates the region's path moved by the offset, for clipping a captured image.</summary>
    public SKPath CreatePath(double offsetX, double offsetY)
    {
        SKPath path = new SKPath { FillType = SKPathFillType.EvenOdd };
        SKRect rectangle = SKRect.Create(
            (float)(Bounds.X + offsetX),
            (float)(Bounds.Y + offsetY),
            (float)Bounds.Width,
            (float)Bounds.Height);

        switch (Shape)
        {
            case RegionShape.Ellipse:
                path.AddOval(rectangle);
                break;
            case RegionShape.Freehand when Points is { Count: >= 3 } points:
                path.MoveTo((float)(points[0].X + offsetX), (float)(points[0].Y + offsetY));
                for (int i = 1; i < points.Count; i++)
                {
                    path.LineTo((float)(points[i].X + offsetX), (float)(points[i].Y + offsetY));
                }

                path.Close();
                break;
            default:
                path.AddRect(rectangle);
                break;
        }

        return path;
    }

    /// <summary>Returns the rectangle that bounds the points, or an empty rectangle without points.</summary>
    public static Rect GetBounds(IReadOnlyList<Point> points)
    {
        if (points.Count == 0)
        {
            return default;
        }

        double left = double.MaxValue;
        double top = double.MaxValue;
        double right = double.MinValue;
        double bottom = double.MinValue;

        foreach (Point point in points)
        {
            left = Math.Min(left, point.X);
            top = Math.Min(top, point.Y);
            right = Math.Max(right, point.X);
            bottom = Math.Max(bottom, point.Y);
        }

        return new Rect(left, top, right - left, bottom - top);
    }

    // Even-odd rule, matching the fill of the outline and of the captured image.
    private static bool PolygonContains(IReadOnlyList<Point> points, Point point)
    {
        bool inside = false;

        for (int i = 0, j = points.Count - 1; i < points.Count; j = i++)
        {
            Point a = points[i];
            Point b = points[j];

            if ((a.Y > point.Y) != (b.Y > point.Y) &&
                point.X < (b.X - a.X) * (point.Y - a.Y) / (b.Y - a.Y) + a.X)
            {
                inside = !inside;
            }
        }

        return inside;
    }
}
