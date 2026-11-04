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

/// <summary>
/// Flattens a layer tree into pixels. Supports blend modes, opacity and fill, masks, clipping masks,
/// layer styles, adjustment layers, groups (isolated and pass-through) and artboards.
/// </summary>
public static class Compositor
{
    /// <summary>Renders the document into a full-size target bitmap, only touching the given area.</summary>
    public static void Render(PhotoDocument document, SKBitmap target, SKRectI area)
    {
        area.Intersect(document.Bounds);
        area.Intersect(new SKRectI(0, 0, target.Width, target.Height));
        if (area.IsEmpty) return;

        RenderContext context = new(document.Width, document.Height) { Area = area };
        using SKBitmap accumulator = ImageOps.CreateBitmap(area.Width, area.Height);
        accumulator.Erase(SKColors.Transparent);
        RenderChildren(document.Root.Children, accumulator, context);

        using SKCanvas canvas = new(target);
        canvas.Save();
        canvas.ClipRect(SKRect.Create(area.Left, area.Top, area.Width, area.Height));
        canvas.Clear(SKColors.Transparent);
        using SKPaint paint = new() { BlendMode = SKBlendMode.Src };
        canvas.DrawBitmap(accumulator, area.Left, area.Top, paint);
        canvas.Restore();
    }

    public static SKBitmap Flatten(PhotoDocument document)
    {
        SKBitmap result = ImageOps.CreateBitmap(document.Width, document.Height);
        result.Erase(SKColors.Transparent);
        Render(document, result, document.Bounds);
        return result;
    }

    /// <summary>Renders a set of layers (in bottom-to-top order) as if they were the only content of the document.</summary>
    public static SKBitmap RenderLayers(PhotoDocument document, IReadOnlyList<Layer> layers)
    {
        SKBitmap result = ImageOps.CreateBitmap(document.Width, document.Height);
        result.Erase(SKColors.Transparent);
        RenderContext context = new(document.Width, document.Height) { Area = document.Bounds };
        RenderChildren(layers, result, context);
        return result;
    }

    /// <summary>Renders one layer with its mask and style (ignoring visibility of the layer itself) at document size.</summary>
    public static SKBitmap RenderLayer(PhotoDocument document, Layer layer, bool applyMask = true)
    {
        RenderContext context = new(document.Width, document.Height) { Area = document.Bounds };
        return RenderLayerContent(layer, context, applyMask);
    }

    private static void RenderChildren(IReadOnlyList<Layer> children, SKBitmap accumulator, RenderContext context)
    {
        for (int i = 0; i < children.Count; i++)
        {
            Layer layer = children[i];

            List<Layer> clipped = [];
            int j = i + 1;
            while (j < children.Count && children[j].IsClipped && !layer.IsClipped)
            {
                clipped.Add(children[j]);
                j++;
            }

            if (!layer.Visible)
            {
                i = j - 1;
                continue;
            }

            if (layer is AdjustmentLayer adjustment)
            {
                ApplyAdjustment(adjustment, accumulator, context);
                RenderClippedRun(clipped, accumulator, context, null);
                i = j - 1;
                continue;
            }

            if (layer is GroupLayer group && group.BlendMode == LayerBlendMode.PassThrough && !group.IsArtboard && clipped.Count == 0)
            {
                RenderPassThrough(group, accumulator, context);
                continue;
            }

            if (clipped.Count == 0 && TryDrawRasterDirect(layer, accumulator, context)) continue;

            using SKBitmap content = RenderLayerContent(layer, context, true);
            if (clipped.Count > 0)
            {
                RenderClippedRun(clipped, content, context, content);
                i = j - 1;
            }
            Composite(accumulator, content, layer.BlendMode == LayerBlendMode.PassThrough ? LayerBlendMode.Normal : layer.BlendMode, layer.Opacity);
        }
    }

    /// <summary>
    /// Plain pixel layers (no style, mask or CPU-only blend mode) are drawn straight onto the accumulator,
    /// which avoids allocating and filling a document-sized bitmap per layer.
    /// </summary>
    private static bool TryDrawRasterDirect(Layer layer, SKBitmap accumulator, RenderContext context)
    {
        if (layer is not RasterLayer raster || layer.Style is { HasAny: true } || layer.Mask != null && layer.MaskEnabled) return false;
        if (layer.BlendMode == LayerBlendMode.Dissolve || BlendModes.ToSkia(layer.BlendMode) is not SKBlendMode blend) return false;
        float alpha = layer.Opacity * layer.FillOpacity;
        SKRectI area = context.Area;
        if (alpha <= 0 || !raster.SurfaceBounds.IntersectsWith(area)) return true;
        using SKCanvas canvas = new(accumulator);
        using SKPaint paint = new() { BlendMode = blend, Color = SKColors.White.WithAlpha((byte)Math.Round(Math.Clamp(alpha, 0, 1) * 255)) };
        canvas.DrawBitmap(raster.Surface.Bitmap, raster.Offset.X - area.Left, raster.Offset.Y - area.Top, paint);
        return true;
    }

    private static void RenderClippedRun(List<Layer> clipped, SKBitmap baseContent, RenderContext context, SKBitmap? clipSource)
    {
        if (clipSource == null)
        {
            // Layers clipped to an adjustment layer render normally.
            foreach (Layer layer in clipped.Where(l => l.Visible))
            {
                using SKBitmap c = RenderLayerContent(layer, context, true);
                Composite(baseContent, c, layer.BlendMode, layer.Opacity);
            }
            return;
        }

        using SKBitmap clipAlpha = clipSource.Copy();
        foreach (Layer layer in clipped.Where(l => l.Visible))
        {
            if (layer is AdjustmentLayer adjustment)
            {
                ApplyAdjustment(adjustment, baseContent, context);
                continue;
            }
            using SKBitmap content = RenderLayerContent(layer, context, true);
            using (SKCanvas canvas = new(content))
            using (SKPaint paint = new() { BlendMode = SKBlendMode.DstIn })
            {
                canvas.DrawBitmap(clipAlpha, 0, 0, paint);
            }
            BlendModes.BlendBitmaps(baseContent, content, layer.BlendMode, layer.Opacity,
                new SKRectI(0, 0, baseContent.Width, baseContent.Height), preserveAlpha: true);
        }
    }

    private static void RenderPassThrough(GroupLayer group, SKBitmap accumulator, RenderContext context)
    {
        bool needsBlend = group.Opacity < 1 || group.Mask != null && group.MaskEnabled;
        if (!needsBlend)
        {
            RenderChildren(group.Children, accumulator, context);
            return;
        }
        using SKBitmap result = accumulator.Copy();
        RenderChildren(group.Children, result, context);
        LerpInto(accumulator, result, (x, y) => group.Opacity * MaskWeight(group, context, x, y));
    }

    private static float MaskWeight(Layer layer, RenderContext context, int x, int y)
    {
        if (layer.Mask == null || !layer.MaskEnabled) return 1;
        return MaskOps.Get(layer.Mask, x + context.Area.Left, y + context.Area.Top) / 255f;
    }

    private static unsafe void LerpInto(SKBitmap destination, SKBitmap source, Func<int, int, float> weight)
    {
        uint* dst = (uint*)destination.GetPixels();
        uint* src = (uint*)source.GetPixels();
        int stride = destination.RowBytes / 4;
        int width = destination.Width;
        Parallel.For(0, destination.Height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                float w = weight(x, y);
                if (w <= 0) continue;
                dst[y * stride + x] = ColorMath.LerpPremul(dst[y * stride + x], src[y * stride + x], w);
            }
        });
    }

    private static void ApplyAdjustment(AdjustmentLayer layer, SKBitmap target, RenderContext context)
    {
        float opacity = layer.Opacity * layer.FillOpacity;
        bool masked = layer.Mask != null && layer.MaskEnabled;
        Func<int, int, float>? weight = opacity >= 1 && !masked ? null : (x, y) => opacity * MaskWeight(layer, context, x, y);
        if (layer.BlendMode == LayerBlendMode.Normal)
        {
            Adjustments.Apply(layer.Settings, target, new SKRectI(0, 0, target.Width, target.Height), weight);
            return;
        }
        // Blend the adjusted image back with the layer's blend mode.
        using SKBitmap adjusted = target.Copy();
        Adjustments.Apply(layer.Settings, adjusted, new SKRectI(0, 0, adjusted.Width, adjusted.Height));
        if (weight != null) ScaleAlpha(adjusted, weight);
        BlendModes.BlendBitmaps(target, adjusted, layer.BlendMode, 1, new SKRectI(0, 0, target.Width, target.Height), preserveAlpha: true);
    }

    private static unsafe void ScaleAlpha(SKBitmap bitmap, Func<int, int, float> weight)
    {
        uint* p = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                float w = weight(x, y);
                if (w < 1) p[y * stride + x] = ImageOps.ScalePixel(p[y * stride + x], (int)(Math.Clamp(w, 0, 1) * 255));
            }
        }
    }

    private static void Composite(SKBitmap destination, SKBitmap source, LayerBlendMode mode, float opacity)
    {
        if (opacity <= 0) return;
        SKBlendMode? skia = mode == LayerBlendMode.Dissolve ? null : BlendModes.ToSkia(mode);
        if (skia is SKBlendMode blend)
        {
            using SKCanvas canvas = new(destination);
            using SKPaint paint = new() { BlendMode = blend, Color = SKColors.White.WithAlpha((byte)Math.Round(opacity * 255)) };
            canvas.DrawBitmap(source, 0, 0, paint);
        }
        else
        {
            BlendModes.BlendBitmaps(destination, source, mode, opacity, new SKRectI(0, 0, destination.Width, destination.Height));
        }
    }

    /// <summary>Renders layer content into an area-sized bitmap including fill opacity, style and mask.</summary>
    private static SKBitmap RenderLayerContent(Layer layer, RenderContext context, bool applyMask)
    {
        SKRectI area = context.Area;
        LayerStyle? style = layer.Style is { HasAny: true } s ? s : null;
        int extent = style != null ? (int)MathF.Ceiling(style.Extent) : 0;
        SKRectI renderArea = area;
        renderArea.Inflate(extent, extent);

        SKBitmap content = ImageOps.CreateBitmap(renderArea.Width, renderArea.Height);
        content.Erase(SKColors.Transparent);
        using (SKCanvas canvas = new(content))
        {
            canvas.Translate(-renderArea.Left, -renderArea.Top);
            if (layer is GroupLayer group)
            {
                RenderGroupIsolated(group, content, context, renderArea);
            }
            else
            {
                layer.RenderContent(canvas, context);
            }
        }

        SKBitmap result;
        if (style != null)
        {
            result = ApplyStyle(content, style, layer.FillOpacity);
            content.Dispose();
            if (extent > 0)
            {
                SKBitmap cropped = ImageOps.Crop(result, SKRectI.Create(extent, extent, area.Width, area.Height));
                result.Dispose();
                result = cropped;
            }
        }
        else
        {
            result = content;
            if (layer.FillOpacity < 1 && layer is not GroupLayer)
            {
                ScaleAlpha(result, (_, _) => layer.FillOpacity);
            }
        }

        if (applyMask && layer.Mask != null && layer.MaskEnabled)
        {
            using SKCanvas canvas = new(result);
            using SKPaint paint = new() { BlendMode = SKBlendMode.DstIn };
            canvas.DrawBitmap(layer.Mask.Bitmap, -area.Left, -area.Top, paint);
        }
        return result;
    }

    private static void RenderGroupIsolated(GroupLayer group, SKBitmap target, RenderContext context, SKRectI renderArea)
    {
        RenderContext inner = new(context.DocumentWidth, context.DocumentHeight) { Area = renderArea, Sampling = context.Sampling };
        if (group.ArtboardBounds is SKRectI artboard)
        {
            using SKBitmap content = ImageOps.CreateBitmap(target.Width, target.Height);
            content.Erase(SKColors.Transparent);
            using (SKCanvas canvas = new(content))
            using (SKPaint background = new() { Color = group.ArtboardBackground })
            {
                canvas.Translate(-renderArea.Left, -renderArea.Top);
                canvas.DrawRect(SKRect.Create(artboard.Left, artboard.Top, artboard.Width, artboard.Height), background);
            }
            RenderChildren(group.Children, content, inner);
            using SKCanvas targetCanvas = new(target);
            targetCanvas.ClipRect(SKRect.Create(artboard.Left - renderArea.Left, artboard.Top - renderArea.Top, artboard.Width, artboard.Height));
            targetCanvas.DrawBitmap(content, 0, 0);
            return;
        }
        RenderChildren(group.Children, target, inner);
    }

    private static SKBitmap ApplyStyle(SKBitmap content, LayerStyle style, float fillOpacity)
    {
        int width = content.Width, height = content.Height;
        SKBitmap result = ImageOps.CreateBitmap(width, height);
        result.Erase(SKColors.Transparent);
        using SKCanvas canvas = new(result);

        if (style.OuterGlow && style.GlowSize > 0)
        {
            using SKImageFilter glow = SKImageFilter.CreateDropShadowOnly(0, 0, style.GlowSize / 2f, style.GlowSize / 2f,
                style.GlowColor.WithAlpha((byte)(style.GlowOpacity * 255)));
            using SKPaint paint = new() { ImageFilter = glow, BlendMode = SKBlendMode.Screen };
            canvas.DrawBitmap(content, 0, 0, paint);
        }

        if (style.DropShadow)
        {
            float radians = style.ShadowAngle * MathF.PI / 180;
            float dx = -MathF.Cos(radians) * style.ShadowDistance;
            float dy = MathF.Sin(radians) * style.ShadowDistance;
            using SKImageFilter shadow = SKImageFilter.CreateDropShadowOnly(dx, dy, style.ShadowSize / 2f, style.ShadowSize / 2f,
                style.ShadowColor.WithAlpha((byte)(style.ShadowOpacity * 255)));
            using SKPaint paint = new() { ImageFilter = shadow };
            canvas.DrawBitmap(content, 0, 0, paint);
        }

        using SKBitmap body = content.Copy();
        if (style.ColorOverlay)
        {
            using SKCanvas bodyCanvas = new(body);
            using SKPaint overlay = new() { Color = style.OverlayColor.WithAlpha((byte)(style.OverlayOpacity * 255)), BlendMode = SKBlendMode.SrcATop };
            bodyCanvas.DrawRect(0, 0, width, height, overlay);
        }
        if (style.InnerShadow)
        {
            // Shadow cast by the inverse of the shape, kept inside the shape.
            using SKBitmap inverse = ImageOps.CreateBitmap(width, height);
            inverse.Erase(SKColors.Black);
            using (SKCanvas inverseCanvas = new(inverse))
            using (SKPaint cut = new() { BlendMode = SKBlendMode.DstOut })
            {
                inverseCanvas.DrawBitmap(content, 0, 0, cut);
            }
            using SKImageFilter shadow = SKImageFilter.CreateDropShadowOnly(style.InnerShadowDistance, style.InnerShadowDistance,
                style.InnerShadowSize / 2f, style.InnerShadowSize / 2f, style.InnerShadowColor.WithAlpha((byte)(style.InnerShadowOpacity * 255)));
            using SKCanvas bodyCanvas = new(body);
            using SKPaint paint = new() { ImageFilter = shadow, BlendMode = SKBlendMode.SrcATop };
            bodyCanvas.DrawBitmap(inverse, 0, 0, paint);
        }
        using (SKPaint bodyPaint = new() { Color = SKColors.White.WithAlpha((byte)(fillOpacity * 255)) })
        {
            canvas.DrawBitmap(body, 0, 0, bodyPaint);
        }

        if (style.Stroke && style.StrokeSize > 0)
        {
            float size = style.StrokeSize;
            SKColor color = style.StrokeColor.WithAlpha((byte)(style.StrokeOpacity * 255));
            using SKColorFilter tint = SKColorFilter.CreateBlendMode(color, SKBlendMode.SrcIn);
            using SKBitmap stroke = ImageOps.CreateBitmap(width, height);
            stroke.Erase(SKColors.Transparent);
            using (SKCanvas strokeCanvas = new(stroke))
            {
                float outer = style.StrokePosition switch { StrokePosition.Outside => size, StrokePosition.Center => size / 2, _ => 0 };
                float inner = style.StrokePosition switch { StrokePosition.Inside => size, StrokePosition.Center => size / 2, _ => 0 };
                using (SKImageFilter dilate = SKImageFilter.CreateDilate(outer, outer))
                using (SKImageFilter colored = SKImageFilter.CreateColorFilter(tint, outer > 0 ? dilate : null))
                using (SKPaint paint = new() { ImageFilter = colored })
                {
                    strokeCanvas.DrawBitmap(content, 0, 0, paint);
                }
                using SKImageFilter erode = SKImageFilter.CreateErode(inner, inner);
                using SKPaint cut = new() { BlendMode = SKBlendMode.DstOut, ImageFilter = inner > 0 ? erode : null };
                strokeCanvas.DrawBitmap(content, 0, 0, cut);
            }
            canvas.DrawBitmap(stroke, 0, 0);
        }
        return result;
    }
}
