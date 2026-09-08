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
using Avalonia.Media;
using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class CropAnnotation
{
    private const int CropHandleZIndex = 7000;
    private const double CropHandleCornerHitSize = 40;
    private const double CropHandleEdgeHitLength = 40;
    private const double CropHandleEdgeHitThickness = 28;
    private const double CropHandleCornerArmLength = 24;
    private const double CropHandleCenterBarLength = 30;
    private const double CropHandleThickness = 5;
    private static readonly Color CropHandleFill = Color.FromRgb(255, 255, 255);
    private const double MinCropSize = 16;

    internal static Rect GetAutoCropBounds(SKBitmap? source, Size canvasSize)
    {
        const int tolerance = 10;
        var fullRect = new Rect(canvasSize);

        if (source == null || source.Width <= 0 || source.Height <= 0)
        {
            return fullRect;
        }

        SKRectI? bounds = ImageHelpers.FindContentBounds(source, source.GetPixel(0, 0), tolerance);
        if (bounds is not SKRectI rect || (rect.Width >= source.Width && rect.Height >= source.Height))
        {
            return fullRect;
        }

        double scaleX = canvasSize.Width / source.Width;
        double scaleY = canvasSize.Height / source.Height;
        return new Rect(rect.Left * scaleX, rect.Top * scaleY, rect.Width * scaleX, rect.Height * scaleY);
    }

    internal static Border CreateResizeHandle(Point center, string tag)
    {
        Cursor cursor = new Cursor(StandardCursorType.SizeNorthSouth);
        if (tag.Contains("TopLeft") || tag.Contains("BottomRight")) cursor = new Cursor(StandardCursorType.TopLeftCorner);
        else if (tag.Contains("TopRight") || tag.Contains("BottomLeft")) cursor = new Cursor(StandardCursorType.TopRightCorner);
        else if (tag.Contains("Top") || tag.Contains("Bottom")) cursor = new Cursor(StandardCursorType.SizeNorthSouth);
        else if (tag.Contains("Left") || tag.Contains("Right")) cursor = new Cursor(StandardCursorType.SizeWestEast);

        bool isCorner = tag.EndsWith("TopLeft", StringComparison.Ordinal) || tag.EndsWith("TopRight", StringComparison.Ordinal)
            || tag.EndsWith("BottomRight", StringComparison.Ordinal) || tag.EndsWith("BottomLeft", StringComparison.Ordinal);

        bool isHorizontalEdge = tag.Contains("Top", StringComparison.Ordinal) || tag.Contains("Bottom", StringComparison.Ordinal);
        double width = isCorner ? CropHandleCornerHitSize : (isHorizontalEdge ? CropHandleEdgeHitLength : CropHandleEdgeHitThickness);
        double height = isCorner ? CropHandleCornerHitSize : (isHorizontalEdge ? CropHandleEdgeHitThickness : CropHandleEdgeHitLength);
        Control visual;

        if (isCorner)
        {
            visual = CreateCropCornerLShape(tag);
        }
        else
        {
            visual = CreateCropEdgeBar(width, height);
        }

        var handle = new Border
        {
            Width = width,
            Height = height,
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Tag = tag,
            Cursor = cursor,
            Child = visual,
            ClipToBounds = false
        };
        handle.SetValue(Panel.ZIndexProperty, CropHandleZIndex);

        Canvas.SetLeft(handle, center.X - (width / 2.0));
        Canvas.SetTop(handle, center.Y - (height / 2.0));
        return handle;
    }

    /// <summary>
    /// Creates a corner bracket pointing into the crop area.
    /// </summary>
    private static Control CreateCropCornerLShape(string tag)
    {
        const double size = CropHandleCornerHitSize;
        const double arm = CropHandleCornerArmLength;
        const double w = CropHandleThickness;
        const double half = size / 2.0;

        // Center the bracket vertex in the hit area, with arms pointing into the crop.
        bool extendsLeft = tag.Contains("Right", StringComparison.Ordinal);
        bool extendsUp = tag.Contains("Bottom", StringComparison.Ordinal);
        double horizontalEnd = extendsLeft ? half - arm : half + arm;
        double verticalEnd = extendsUp ? half - arm : half + arm;

        var geometry = new StreamGeometry();
        using (StreamGeometryContext context = geometry.Open())
        {
            context.BeginFigure(new Point(horizontalEnd, half), false);
            context.LineTo(new Point(half, half));
            context.LineTo(new Point(half, verticalEnd));
        }

        return new global::Avalonia.Controls.Shapes.Path
        {
            Width = size,
            Height = size,
            Data = geometry,
            Stroke = new SolidColorBrush(CropHandleFill),
            StrokeThickness = w,
            StrokeLineCap = PenLineCap.Round,
            StrokeJoin = PenLineJoin.Round,
            Stretch = Stretch.None,
            IsHitTestVisible = false
        };
    }

    /// <summary>
    /// Creates a center resize node with a larger hit target.
    /// </summary>
    private static Control CreateCropEdgeBar(double width, double height)
    {
        bool isHorizontal = width >= height;
        double barWidth = isHorizontal ? CropHandleCenterBarLength : CropHandleThickness;
        double barHeight = isHorizontal ? CropHandleThickness : CropHandleCenterBarLength;

        var canvas = new Canvas
        {
            Width = width,
            Height = height,
            IsHitTestVisible = false,
            ClipToBounds = false
        };

        canvas.Children.Add(CreateCropHandleRect(
            (width - barWidth) / 2.0,
            (height - barHeight) / 2.0,
            barWidth,
            barHeight));

        return canvas;
    }

    private static Rectangle CreateCropHandleRect(double left, double top, double width, double height)
    {
        var rect = new Rectangle
        {
            Width = width,
            Height = height,
            Fill = new SolidColorBrush(CropHandleFill),
            RadiusX = CropHandleThickness / 2.0,
            RadiusY = CropHandleThickness / 2.0,
            IsHitTestVisible = false
        };

        Canvas.SetLeft(rect, left);
        Canvas.SetTop(rect, top);
        return rect;
    }

    internal static Rect ResizeBounds(string handleTag, Point dragStart, Point current, Rect originalRect, double canvasW, double canvasH)
    {
        double left = originalRect.Left;
        double top = originalRect.Top;
        double right = originalRect.Right;
        double bottom = originalRect.Bottom;
        double cx = ClampSafe(current.X, 0, canvasW);
        double cy = ClampSafe(current.Y, 0, canvasH);

        switch (handleTag)
        {
            case "Crop_TopLeft":
                left = ClampSafe(cx, 0, right - MinCropSize);
                top = ClampSafe(cy, 0, bottom - MinCropSize);
                break;
            case "Crop_TopCenter":
                top = ClampSafe(cy, 0, bottom - MinCropSize);
                break;
            case "Crop_TopRight":
                right = ClampSafe(cx, left + MinCropSize, canvasW);
                top = ClampSafe(cy, 0, bottom - MinCropSize);
                break;
            case "Crop_RightCenter":
                right = ClampSafe(cx, left + MinCropSize, canvasW);
                break;
            case "Crop_BottomRight":
                right = ClampSafe(cx, left + MinCropSize, canvasW);
                bottom = ClampSafe(cy, top + MinCropSize, canvasH);
                break;
            case "Crop_BottomCenter":
                bottom = ClampSafe(cy, top + MinCropSize, canvasH);
                break;
            case "Crop_BottomLeft":
                left = ClampSafe(cx, 0, right - MinCropSize);
                bottom = ClampSafe(cy, top + MinCropSize, canvasH);
                break;
            case "Crop_LeftCenter":
                left = ClampSafe(cx, 0, right - MinCropSize);
                break;
            case "Crop_Move":
                double deltaX = current.X - dragStart.X;
                double deltaY = current.Y - dragStart.Y;
                double maxLeft = Math.Max(0, canvasW - originalRect.Width);
                double maxTop = Math.Max(0, canvasH - originalRect.Height);
                double newLeft = ClampSafe(originalRect.Left + deltaX, 0, maxLeft);
                double newTop = ClampSafe(originalRect.Top + deltaY, 0, maxTop);
                return new Rect(newLeft, newTop, originalRect.Width, originalRect.Height);
            default:
                return originalRect;
        }

        left = ClampSafe(left, 0, canvasW - MinCropSize);
        top = ClampSafe(top, 0, canvasH - MinCropSize);
        right = ClampSafe(right, left + MinCropSize, canvasW);
        bottom = ClampSafe(bottom, top + MinCropSize, canvasH);

        return new Rect(left, top, Math.Max(MinCropSize, right - left), Math.Max(MinCropSize, bottom - top));
    }

    private static double ClampSafe(double value, double min, double max)
    {
        if (max < min) return min;
        return Math.Clamp(value, min, max);
    }
}
