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

/// <summary>Pixel operations on raster layers shared by tools and menu commands.</summary>
public static class LayerOps
{
    /// <summary>
    /// Composites a per-pixel source onto a raster layer, weighted by an optional coverage mask and the selection.
    /// Masks are in document coordinates. Returns the changed document rectangle.
    /// </summary>
    public static unsafe SKRectI CompositeMasked(PhotoDocument doc, RasterLayer layer, Func<int, int, uint> source, PixelSurface? coverage,
        float opacity, LayerBlendMode mode, SKRectI? limit = null)
    {
        layer.EnsureCovers(doc.Bounds);
        PixelSurface surface = layer.GetWritableSurface();
        SKRectI area = limit ?? (coverage != null ? MaskOps.GetBounds(coverage) : doc.Bounds);
        if (doc.Selection != null) area.Intersect(MaskOps.GetBounds(doc.Selection));
        area.Intersect(doc.Bounds);
        if (area.IsEmpty) return area;
        uint* pixels = (uint*)surface.Bitmap.GetPixels();
        int stride = surface.RowBytes / 4;
        SKPointI offset = layer.Offset;
        PixelSurface? selection = doc.Selection;
        bool lockAlpha = layer.LockTransparency;
        Parallel.For(area.Top, area.Bottom, y =>
        {
            for (int x = area.Left; x < area.Right; x++)
            {
                float weight = opacity;
                if (coverage != null) weight *= MaskOps.Get(coverage, x, y) / 255f;
                if (selection != null) weight *= MaskOps.Get(selection, x, y) / 255f;
                if (weight <= 0) continue;
                uint s = source(x, y);
                ColorMath.Unpack(s, out float r, out float g, out float b, out float a);
                int lx = x - offset.X, ly = y - offset.Y;
                uint* p = pixels + ly * stride + lx;
                *p = BlendModes.CompositePixel(*p, r, g, b, a * weight, mode, lockAlpha);
            }
        });
        return area;
    }

    /// <summary>Erases layer pixels by mask coverage (and selection).</summary>
    public static unsafe SKRectI EraseMasked(PhotoDocument doc, RasterLayer layer, PixelSurface? coverage, float opacity)
    {
        layer.EnsureCovers(doc.Bounds);
        PixelSurface surface = layer.GetWritableSurface();
        SKRectI area = coverage != null ? MaskOps.GetBounds(coverage) : doc.SelectionBounds;
        area.Intersect(doc.Bounds);
        uint* pixels = (uint*)surface.Bitmap.GetPixels();
        int stride = surface.RowBytes / 4;
        for (int y = area.Top; y < area.Bottom; y++)
        {
            for (int x = area.Left; x < area.Right; x++)
            {
                float weight = opacity * (coverage != null ? MaskOps.Get(coverage, x, y) / 255f : 1) * doc.SelectionAt(x, y);
                if (weight <= 0) continue;
                uint* p = pixels + (y - layer.Offset.Y) * stride + (x - layer.Offset.X);
                *p = ImageOps.ScalePixel(*p, (int)((1 - weight) * 255));
            }
        }
        return area;
    }

    /// <summary>Converts a locked background layer into a normal layer (needed before erasing to transparency).</summary>
    public static void UnlockBackground(RasterLayer layer)
    {
        if (!layer.IsBackground) return;
        layer.IsBackground = false;
        layer.LockTransparency = false;
        layer.LockPosition = false;
        if (layer.Name == "Background") layer.Name = "Layer 0";
    }

    /// <summary>Renders a gradient into a document-sized bitmap.</summary>
    public static unsafe SKBitmap RenderGradient(GradientDefinition gradient, SKPoint start, SKPoint end, int width, int height, bool dither)
    {
        SKBitmap result = ImageOps.CreateBitmap(width, height);
        result.Erase(SKColors.Transparent);
        if (gradient.Type == GradientType.Diamond)
        {
            uint[] lut = gradient.BuildLut();
            uint* p = (uint*)result.GetPixels();
            int stride = result.RowBytes / 4;
            SKPoint axis = end - start;
            float length = Math.Max(0.5f, axis.Length);
            SKPoint u = new(axis.X / length, axis.Y / length);
            SKPoint v = new(-u.Y, u.X);
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = x + 0.5f - start.X, dy = y + 0.5f - start.Y;
                    float t = (MathF.Abs(dx * u.X + dy * u.Y) + MathF.Abs(dx * v.X + dy * v.Y)) / length;
                    p[y * stride + x] = lut[Math.Clamp((int)(t * 255), 0, 255)];
                }
            });
            return result;
        }
        using SKCanvas canvas = new(result);
        using SKShader shader = gradient.CreateShader(start, end);
        using SKPaint paint = new() { Shader = shader, IsDither = dither };
        canvas.DrawRect(0, 0, width, height, paint);
        return result;
    }

    /// <summary>Strokes the selection outline (Edit &gt; Stroke).</summary>
    public static SKRectI StrokeSelection(PhotoDocument doc, RasterLayer layer, float width, SKColor color, StrokePosition position, float opacity, LayerBlendMode mode)
    {
        if (doc.Selection == null) return SKRectI.Empty;
        PixelSurface outer = position == StrokePosition.Inside ? doc.Selection.Clone() : MaskOps.Morph(doc.Selection, position == StrokePosition.Center ? width / 2 : width);
        PixelSurface inner = position == StrokePosition.Outside ? doc.Selection.Clone() : MaskOps.Morph(doc.Selection, -(position == StrokePosition.Center ? width / 2 : width));
        PixelSurface? ring = MaskOps.Combine(outer, inner, SelectionMode.Subtract);
        inner.Dispose();
        if (ring == null) return SKRectI.Empty;
        MaskOps.Blur(ring, 0.75f);
        uint c = ColorMath.FromColor(color);
        PixelSurface? saved = doc.Selection;
        doc.SetSelectionLive(null);
        SKRectI changed = CompositeMasked(doc, layer, (_, _) => c, ring, opacity, mode);
        doc.SetSelectionLive(saved);
        ring.Dispose();
        return changed;
    }
}
