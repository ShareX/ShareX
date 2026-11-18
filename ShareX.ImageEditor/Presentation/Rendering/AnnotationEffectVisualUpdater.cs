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
using Avalonia.Media;
using ShareX.ImageEditor.Core.Annotations;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Rendering;

public static class AnnotationEffectVisualUpdater
{
    public static void UpdateEffectVisual(Control shape, SKBitmap? sourceBitmap, Rect? overrideBounds = null)
    {
        if (shape?.Tag is not BaseEffectAnnotation annotation || sourceBitmap == null)
        {
            return;
        }

        Rect bounds = overrideBounds ?? GetBounds(shape, annotation);
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        annotation.StartPoint = new SKPoint((float)bounds.X, (float)bounds.Y);
        annotation.EndPoint = new SKPoint((float)(bounds.X + bounds.Width), (float)(bounds.Y + bounds.Height));
        annotation.UpdateEffect(sourceBitmap);
        ApplyEffectClip(shape, annotation, bounds);
        ApplyEffectBrush(shape, annotation);
    }

    public static void ApplyEffectBrush(Control shape, BaseEffectAnnotation annotation)
    {
        if (shape is not Shape shapeControl || annotation.EffectBitmap == null)
        {
            return;
        }

        var brush = new ImageBrush(BitmapConversionHelpers.ToAvaloniBitmap(annotation.EffectBitmap))
        {
            Stretch = Stretch.Fill,
            SourceRect = new RelativeRect(0, 0, 1, 1, RelativeUnit.Relative)
        };

        if (annotation.EffectPixelBounds is SKRectI pixelBounds)
        {
            // Draw the bitmap 1:1 on the image pixel grid: stretching it over fractional bounds resamples it,
            // which blurs text under a highlight. Avalonia resamples untiled or offset tiles through an
            // intermediate image, so use one tile the size of the bitmap and shift it onto the pixel grid with
            // a transform. The tile covers the shape; mirroring it keeps filtering at the edges from pulling
            // in the opposite edge when zoomed out.
            SKRect bounds = annotation.GetBounds();
            brush.TileMode = TileMode.FlipXY;
            brush.DestinationRect = new RelativeRect(0, 0, pixelBounds.Width, pixelBounds.Height, RelativeUnit.Absolute);
            brush.Transform = new TranslateTransform(pixelBounds.Left - bounds.Left, pixelBounds.Top - bounds.Top);
        }

        shapeControl.Fill = brush;
    }

    private static Rect GetBounds(Control shape, BaseEffectAnnotation annotation)
    {
        double left = Canvas.GetLeft(shape);
        double top = Canvas.GetTop(shape);
        double width = shape.Width;
        double height = shape.Height;

        if (double.IsNaN(width) || width <= 0)
        {
            width = shape.Bounds.Width;
        }

        if (double.IsNaN(height) || height <= 0)
        {
            height = shape.Bounds.Height;
        }

        if (double.IsNaN(left) || double.IsNaN(top) || width <= 0 || height <= 0)
        {
            var annotationBounds = annotation.GetBounds();
            left = annotationBounds.Left;
            top = annotationBounds.Top;
            width = annotationBounds.Width;
            height = annotationBounds.Height;
        }

        return new Rect(left, top, width, height);
    }

    public static void ApplyEffectClip(Control shape, BaseEffectAnnotation annotation, Rect bounds)
    {
        if (annotation is MagnifyAnnotation magnifyAnnotation)
        {
            AnnotationVisualHelpers.ApplyMagnifyShape(shape, magnifyAnnotation, bounds.Width, bounds.Height);
        }
        else
        {
            shape.Clip = null;
        }
    }
}
