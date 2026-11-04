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

namespace ShareX.PhotoEditor.Core;

public enum CanvasAnchor
{
    TopLeft, Top, TopRight,
    Left, Center, Right,
    BottomLeft, Bottom, BottomRight
}

/// <summary>Whole-document geometry operations: crop, resize, canvas size, rotate, flip and perspective crop.</summary>
public static class DocumentTransforms
{
    /// <summary>
    /// Maps every layer, mask and overlay item through a matrix into a canvas of a new size.
    /// Raster content outside the new canvas is discarded unless <paramref name="keepRasterOffsets"/> is set and
    /// the matrix is a pure translation.
    /// </summary>
    public static void ApplyMatrix(PhotoDocument doc, SKMatrix matrix, int width, int height, ResampleMethod resample, SKColor? backgroundFill = null, bool keepRasterOffsets = false)
    {
        bool translationOnly = matrix.ScaleX == 1 && matrix.ScaleY == 1 && matrix.SkewX == 0 && matrix.SkewY == 0 && matrix.Persp0 == 0 && matrix.Persp1 == 0 &&
            matrix.TransX == MathF.Round(matrix.TransX) && matrix.TransY == MathF.Round(matrix.TransY);
        foreach (Layer layer in doc.Root.Descendants().ToList())
        {
            TransformLayer(layer, matrix, width, height, resample, translationOnly && keepRasterOffsets, backgroundFill);
        }
        if (doc.Selection != null) doc.SetSelectionLive(MaskOps.Transform(doc.Selection, matrix, width, height));
        foreach (AlphaChannel channel in doc.Channels) channel.Mask = MaskOps.Transform(channel.Mask, matrix, width, height);
        foreach (NoteItem note in doc.Notes) note.Position = matrix.MapPoint(note.Position);
        foreach (CountItem count in doc.Counts) count.Position = matrix.MapPoint(count.Position);
        foreach (ColorSamplerPoint sampler in doc.Samplers) sampler.Position = matrix.MapPoint(sampler.Position);
        foreach (NamedPath path in doc.Paths) path.Path.Transform(matrix);
        doc.WorkPath?.Transform(matrix);
        if (translationOnly)
        {
            List<Guide> guides = doc.Guides.Select(g => g with { Position = g.Position + (g.Horizontal ? matrix.TransY : matrix.TransX) }).ToList();
            doc.Guides.Clear();
            doc.Guides.AddRange(guides);
            foreach (SliceItem slice in doc.Slices)
            {
                SKRectI b = slice.Bounds;
                b.Offset((int)matrix.TransX, (int)matrix.TransY);
                slice.Bounds = b;
            }
        }
        else
        {
            doc.Guides.Clear();
            doc.Slices.Clear();
        }
        doc.ResizeCanvas(width, height);
    }

    /// <summary>Transforms one layer (and its descendants) in place within the current canvas.</summary>
    public static void TransformLayerInPlace(Layer layer, SKMatrix matrix, int width, int height, ResampleMethod resample, SKRectI? knownOpaqueBounds = null)
    {
        TransformLayerContent(layer, matrix, width, height, resample, knownOpaqueBounds);
        if (layer is GroupLayer group)
        {
            foreach (Layer child in group.Descendants()) TransformLayerContent(child, matrix, width, height, resample);
        }
    }

    /// <summary>Largest raster layer (in pixels) a transform may create when it keeps content outside the canvas.</summary>
    private const long MaxExpandedPixels = 64_000_000;

    private static void TransformLayerContent(Layer layer, SKMatrix matrix, int width, int height, ResampleMethod resample, SKRectI? knownOpaqueBounds = null)
    {
        if (layer is RasterLayer { IsBackground: false } raster)
        {
            TransformRasterKeepingOverflow(raster, matrix, width, height, resample, knownOpaqueBounds);
            TransformLayerMask(layer, matrix, width, height);
            return;
        }
        TransformLayer(layer, matrix, width, height, resample, false, null);
    }

    /// <summary>
    /// Transforms a raster layer and keeps the parts that land outside the canvas so a later transform can bring them back.
    /// </summary>
    private static void TransformRasterKeepingOverflow(RasterLayer raster, SKMatrix matrix, int width, int height, ResampleMethod resample, SKRectI? knownOpaqueBounds)
    {
        SKRectI opaque = knownOpaqueBounds ?? raster.GetOpaqueBounds();
        SKMatrix full = SKMatrix.CreateTranslation(raster.Offset.X, raster.Offset.Y).PostConcat(matrix);
        SKRect mapped = matrix.MapRect(SKRect.Create(opaque.Left, opaque.Top, opaque.Width, opaque.Height));
        SKRectI target = opaque.IsEmpty ? new SKRectI(0, 0, width, height) : new SKRectI((int)MathF.Floor(mapped.Left) - 1, (int)MathF.Floor(mapped.Top) - 1, (int)MathF.Ceiling(mapped.Right) + 1, (int)MathF.Ceiling(mapped.Bottom) + 1);
        // Keep a bounded margin around the canvas so huge scale factors cannot exhaust memory.
        SKRectI limit = new(-width, -height, width * 2, height * 2);
        target.Intersect(limit);
        if (target.IsEmpty || (long)target.Width * target.Height > MaxExpandedPixels) target = new SKRectI(0, 0, width, height);
        SKBitmap result = ImageOps.CreateBitmap(target.Width, target.Height);
        using (SKCanvas canvas = new(result))
        {
            canvas.Clear(SKColors.Transparent);
            canvas.Translate(-target.Left, -target.Top);
            canvas.Concat(full);
            using SKImage image = SKImage.FromBitmap(raster.Surface.Bitmap);
            using SKPaint paint = new() { IsAntialias = resample != ResampleMethod.NearestNeighbor };
            canvas.DrawImage(image, 0, 0, ImageOps.GetSampling(resample), paint);
        }
        raster.Surface = new PixelSurface(result);
        raster.Offset = new SKPointI(target.Left, target.Top);
    }

    private static void TransformLayer(Layer layer, SKMatrix matrix, int width, int height, ResampleMethod resample, bool keepOffsets, SKColor? backgroundFill)
    {
        switch (layer)
        {
            case RasterLayer raster:
                if (keepOffsets)
                {
                    raster.Offset = new SKPointI(raster.Offset.X + (int)matrix.TransX, raster.Offset.Y + (int)matrix.TransY);
                    if (raster.IsBackground)
                    {
                        CropRasterToCanvas(raster, width, height, backgroundFill);
                    }
                }
                else
                {
                    SKMatrix full = SKMatrix.CreateTranslation(raster.Offset.X, raster.Offset.Y).PostConcat(matrix);
                    SKBitmap result = ImageOps.CreateBitmap(width, height);
                    using (SKCanvas canvas = new(result))
                    {
                        canvas.Clear(raster.IsBackground && backgroundFill is SKColor fill ? fill : SKColors.Transparent);
                        canvas.SetMatrix(full);
                        using SKImage image = SKImage.FromBitmap(raster.Surface.Bitmap);
                        using SKPaint paint = new() { IsAntialias = resample != ResampleMethod.NearestNeighbor };
                        canvas.DrawImage(image, 0, 0, ImageOps.GetSampling(resample), paint);
                    }
                    raster.Surface = new PixelSurface(result);
                    raster.Offset = default;
                }
                break;
            case TextLayer text:
            {
                SKRect block = text.GetBlockBounds();
                SKPoint c = new(block.MidX, block.MidY);
                if (!text.Transform.IsIdentity)
                {
                    SKMatrix saved = text.Transform;
                    text.Transform = SKMatrix.Identity;
                    block = text.GetBlockBounds();
                    c = new SKPoint(block.MidX, block.MidY);
                    text.Transform = saved;
                }
                text.Transform = text.Transform
                    .PostConcat(SKMatrix.CreateTranslation(c.X, c.Y))
                    .PostConcat(matrix)
                    .PostConcat(SKMatrix.CreateTranslation(-c.X, -c.Y));
                text.Invalidate();
                break;
            }
            case ShapeLayer shape:
                shape.Path.Transform(matrix);
                break;
            case SmartObjectLayer smart:
                smart.Transform = smart.Transform.PostConcat(matrix);
                break;
            case FrameLayer frame:
                frame.Frame = matrix.MapRect(frame.Frame);
                frame.ContentRect = matrix.MapRect(frame.ContentRect);
                break;
            case GroupLayer group when group.ArtboardBounds is SKRectI artboard:
                group.ArtboardBounds = SKRectI.Round(matrix.MapRect(SKRect.Create(artboard.Left, artboard.Top, artboard.Width, artboard.Height)));
                break;
        }
        TransformLayerMask(layer, matrix, width, height);
    }

    private static void TransformLayerMask(Layer layer, SKMatrix matrix, int width, int height)
    {
        if (layer.Mask != null)
        {
            byte fill = MaskOps.Get(layer.Mask, 0, 0);
            PixelSurface transformed = PixelSurface.CreateMask(width, height, fill);
            using (SKCanvas canvas = new(transformed.Bitmap))
            {
                // Clear the transformed footprint, then draw the mask into it.
                using SKPath footprint = new();
                footprint.AddRect(SKRect.Create(layer.Mask.Width, layer.Mask.Height));
                footprint.Transform(matrix);
                using SKPaint clear = new() { BlendMode = SKBlendMode.Clear };
                canvas.DrawPath(footprint, clear);
                canvas.SetMatrix(matrix);
                using SKImage image = SKImage.FromBitmap(layer.Mask.Bitmap);
                canvas.DrawImage(image, 0, 0, new SKSamplingOptions(SKFilterMode.Linear));
            }
            layer.Mask = transformed;
        }
    }

    private static void CropRasterToCanvas(RasterLayer raster, int width, int height, SKColor? fill)
    {
        SKBitmap result = ImageOps.CreateBitmap(width, height);
        using (SKCanvas canvas = new(result))
        {
            canvas.Clear(fill ?? SKColors.Transparent);
            canvas.DrawBitmap(raster.Surface.Bitmap, raster.Offset.X, raster.Offset.Y);
        }
        raster.Surface = new PixelSurface(result);
        raster.Offset = default;
    }

    /// <summary>Crops the document to a rectangle.</summary>
    public static void Crop(PhotoDocument doc, SKRectI rect, bool deleteCroppedPixels, SKColor background)
    {
        ApplyMatrix(doc, SKMatrix.CreateTranslation(-rect.Left, -rect.Top), rect.Width, rect.Height, ResampleMethod.NearestNeighbor, background, !deleteCroppedPixels);
    }

    /// <summary>Crops a rotated rectangle given its center, size and angle (degrees).</summary>
    public static void CropRotated(PhotoDocument doc, SKPoint center, SKSize size, float angle, bool deleteCroppedPixels, SKColor background, ResampleMethod resample)
    {
        int width = Math.Max(1, (int)MathF.Round(size.Width)), height = Math.Max(1, (int)MathF.Round(size.Height));
        if (MathF.Abs(angle) < 0.01f)
        {
            Crop(doc, SKRectI.Create((int)MathF.Round(center.X - size.Width / 2), (int)MathF.Round(center.Y - size.Height / 2), width, height), deleteCroppedPixels, background);
            return;
        }
        SKMatrix matrix = SKMatrix.CreateTranslation(-center.X, -center.Y)
            .PostConcat(SKMatrix.CreateRotationDegrees(-angle))
            .PostConcat(SKMatrix.CreateTranslation(width / 2f, height / 2f));
        ApplyMatrix(doc, matrix, width, height, resample, background);
    }

    /// <summary>Warps a quadrilateral (TL, TR, BR, BL) to a rectangle.</summary>
    public static void PerspectiveCrop(PhotoDocument doc, IReadOnlyList<SKPoint> quad, int width, int height, ResampleMethod resample)
    {
        SKMatrix matrix = ImageOps.QuadToRect(quad, SKRect.Create(width, height));
        ApplyMatrix(doc, matrix, width, height, resample);
    }

    public static void ResizeImage(PhotoDocument doc, int width, int height, ResampleMethod resample, float? resolution = null)
    {
        float sx = width / (float)doc.Width, sy = height / (float)doc.Height;
        SKMatrix matrix = SKMatrix.CreateScale(sx, sy);
        foreach (Layer layer in doc.Root.Descendants().ToList())
        {
            if (layer is RasterLayer raster)
            {
                int w = Math.Max(1, (int)MathF.Round(raster.Surface.Width * sx));
                int h = Math.Max(1, (int)MathF.Round(raster.Surface.Height * sy));
                raster.Surface = new PixelSurface(ImageOps.Resize(raster.Surface.Bitmap, w, h, resample));
                raster.Offset = new SKPointI((int)MathF.Round(raster.Offset.X * sx), (int)MathF.Round(raster.Offset.Y * sy));
                if (layer.Mask != null) layer.Mask = MaskOps.Transform(layer.Mask, matrix, width, height);
            }
            else if (layer is TextLayer text && text.Transform.IsIdentity && MathF.Abs(sx - sy) < 0.001f)
            {
                // Keep text editable and crisp by scaling the font instead of adding a transform.
                text.FontSize *= sx;
                text.Position = new SKPoint(text.Position.X * sx, text.Position.Y * sy);
                text.BoxSize = new SKSize(text.BoxSize.Width * sx, text.BoxSize.Height * sy);
                text.Invalidate();
                if (layer.Mask != null) layer.Mask = MaskOps.Transform(layer.Mask, matrix, width, height);
            }
            else
            {
                TransformLayer(layer, matrix, width, height, resample, false, null);
            }
        }
        if (doc.Selection != null) doc.SetSelectionLive(MaskOps.Transform(doc.Selection, matrix, width, height));
        foreach (AlphaChannel channel in doc.Channels) channel.Mask = MaskOps.Transform(channel.Mask, matrix, width, height);
        foreach (NoteItem note in doc.Notes) note.Position = matrix.MapPoint(note.Position);
        foreach (CountItem count in doc.Counts) count.Position = matrix.MapPoint(count.Position);
        foreach (ColorSamplerPoint sampler in doc.Samplers) sampler.Position = matrix.MapPoint(sampler.Position);
        foreach (NamedPath path in doc.Paths) path.Path.Transform(matrix);
        doc.WorkPath?.Transform(matrix);
        List<Guide> guides = doc.Guides.Select(g => g with { Position = g.Position * (g.Horizontal ? sy : sx) }).ToList();
        doc.Guides.Clear();
        doc.Guides.AddRange(guides);
        doc.Slices.Clear();
        doc.ResizeCanvas(width, height);
        if (resolution is float dpi) doc.Resolution = dpi;
    }

    public static SKPointI AnchorOffset(CanvasAnchor anchor, int oldWidth, int oldHeight, int newWidth, int newHeight)
    {
        int col = (int)anchor % 3, row = (int)anchor / 3;
        int dx = col switch { 0 => 0, 1 => (newWidth - oldWidth) / 2, _ => newWidth - oldWidth };
        int dy = row switch { 0 => 0, 1 => (newHeight - oldHeight) / 2, _ => newHeight - oldHeight };
        return new SKPointI(dx, dy);
    }

    /// <summary>Changes the canvas size without scaling content. New background areas use the extension color.</summary>
    public static void CanvasSize(PhotoDocument doc, int width, int height, CanvasAnchor anchor, SKColor extension)
    {
        SKPointI offset = AnchorOffset(anchor, doc.Width, doc.Height, width, height);
        ApplyMatrix(doc, SKMatrix.CreateTranslation(offset.X, offset.Y), width, height, ResampleMethod.NearestNeighbor, extension, keepRasterOffsets: true);
    }

    public static void Rotate(PhotoDocument doc, float degrees, SKColor background)
    {
        float radians = degrees * MathF.PI / 180;
        float cos = MathF.Abs(MathF.Cos(radians)), sin = MathF.Abs(MathF.Sin(radians));
        int width = (int)MathF.Round(doc.Width * cos + doc.Height * sin);
        int height = (int)MathF.Round(doc.Width * sin + doc.Height * cos);
        SKMatrix matrix = SKMatrix.CreateTranslation(-doc.Width / 2f, -doc.Height / 2f)
            .PostConcat(SKMatrix.CreateRotationDegrees(degrees))
            .PostConcat(SKMatrix.CreateTranslation(width / 2f, height / 2f));
        bool rightAngle = MathF.Abs(degrees % 90) < 0.001f;
        ApplyMatrix(doc, matrix, width, height, rightAngle ? ResampleMethod.NearestNeighbor : ResampleMethod.Bicubic, background);
    }

    public static void Flip(PhotoDocument doc, bool horizontal)
    {
        SKMatrix matrix = horizontal ? new SKMatrix(-1, 0, doc.Width, 0, 1, 0, 0, 0, 1) : new SKMatrix(1, 0, 0, 0, -1, doc.Height, 0, 0, 1);
        ApplyMatrix(doc, matrix, doc.Width, doc.Height, ResampleMethod.NearestNeighbor);
    }

    /// <summary>Bounds of all layer content (Image &gt; Reveal All).</summary>
    public static SKRectI ContentBounds(PhotoDocument doc)
    {
        SKRectI bounds = doc.Bounds;
        foreach (Layer layer in doc.Root.Descendants())
        {
            SKRect b = layer is RasterLayer raster ? raster.GetOpaqueBounds() : layer.GetContentBounds();
            if (!b.IsEmpty) bounds.Union(SKRectI.Round(b));
        }
        return bounds;
    }

    /// <summary>Bounds for Image &gt; Trim based on transparency or a corner color.</summary>
    public static unsafe SKRectI TrimBounds(SKBitmap composite, bool transparent, bool topLeft)
    {
        if (transparent) return ImageOps.GetOpaqueBounds(composite);
        uint reference = ImageOps.GetPixel(composite, topLeft ? 0 : composite.Width - 1, topLeft ? 0 : composite.Height - 1);
        uint* p = (uint*)composite.GetPixels();
        int stride = composite.RowBytes / 4;
        int left = composite.Width, top = composite.Height, right = -1, bottom = -1;
        for (int y = 0; y < composite.Height; y++)
        {
            for (int x = 0; x < composite.Width; x++)
            {
                if (ColorMath.Distance(p[y * stride + x], reference) <= 2) continue;
                left = Math.Min(left, x); right = Math.Max(right, x);
                top = Math.Min(top, y); bottom = Math.Max(bottom, y);
            }
        }
        return right < 0 ? SKRectI.Empty : new SKRectI(left, top, right + 1, bottom + 1);
    }
}
