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

namespace ShareX.PixelEditor.Core;

public enum ResizeMethod
{
    Nearest,
    Smooth,
    PixelArtScale
}

/// <summary>Image effects. Each returns a new buffer and limits itself to the mask when one is given.</summary>
public static class Effects
{
    public static PixelBuffer Map(PixelBuffer source, bool[]? mask, Func<uint, uint> map)
    {
        PixelBuffer result = source.Clone();
        Dictionary<uint, uint> cache = [];
        for (int i = 0; i < result.Pixels.Length; i++)
        {
            uint p = source.Pixels[i];
            if (p >> 24 == 0 || mask != null && !mask[i]) continue;
            if (!cache.TryGetValue(p, out uint mapped))
            {
                mapped = map(p);
                if (mapped >> 24 == 0) mapped = 0;
                cache[p] = mapped;
            }
            result.Pixels[i] = mapped;
        }
        return result;
    }

    public static uint Invert(uint c) => Argb.Make(Argb.A(c), 255 - Argb.R(c), 255 - Argb.G(c), 255 - Argb.B(c));

    public static uint Grayscale(uint c)
    {
        int l = Argb.Luma(c);
        return Argb.Make(Argb.A(c), l, l, l);
    }

    /// <summary>Hue shift in degrees, saturation and lightness in -100..100.</summary>
    public static uint HueSaturation(uint c, float hue, float saturation, float lightness)
    {
        (float h, float s, float l) = Argb.ToHsl(c);
        s = saturation >= 0 ? s + (1 - s) * saturation / 100f : s * (1 + saturation / 100f);
        l = lightness >= 0 ? l + (1 - l) * lightness / 100f : l * (1 + lightness / 100f);
        return Argb.FromHsl(h + hue, s, l, Argb.A(c));
    }

    /// <summary>Brightness and contrast in -100..100.</summary>
    public static uint BrightnessContrast(uint c, float brightness, float contrast)
    {
        float factor = contrast >= 0 ? 1 + contrast / 50f : 1 + contrast / 100f, offset = brightness * 1.275f;
        int Adjust(int v) => (int)MathF.Round((v - 127.5f) * factor + 127.5f + offset);
        return Argb.Make(Argb.A(c), Adjust(Argb.R(c)), Adjust(Argb.G(c)), Adjust(Argb.B(c)));
    }

    public static uint Posterize(uint c, int levels)
    {
        levels = Math.Clamp(levels, 2, 64);
        int Step(int v) => (int)MathF.Round(MathF.Round(v / 255f * (levels - 1)) / (levels - 1) * 255);
        return Argb.Make(Argb.A(c), Step(Argb.R(c)), Step(Argb.G(c)), Step(Argb.B(c)));
    }

    public static PixelBuffer ReplaceColor(PixelBuffer source, bool[]? mask, uint from, uint to, int tolerance)
    {
        PixelBuffer result = source.Clone();
        long limit = (long)tolerance * tolerance * 12;
        for (int i = 0; i < result.Pixels.Length; i++)
        {
            if (mask != null && !mask[i]) continue;
            uint p = source.Pixels[i];
            if (Argb.Distance(p, from) > limit) continue;
            result.Pixels[i] = to >> 24 == 0 ? 0 : to;
        }
        return result;
    }

    /// <summary>Makes every pixel fully opaque or fully transparent, removing soft edges.</summary>
    public static PixelBuffer AlphaThreshold(PixelBuffer source, bool[]? mask, int threshold)
    {
        PixelBuffer result = source.Clone();
        for (int i = 0; i < result.Pixels.Length; i++)
        {
            if (mask != null && !mask[i]) continue;
            uint p = source.Pixels[i];
            result.Pixels[i] = Argb.A(p) >= threshold && p >> 24 != 0 ? Argb.Opaque(p) : 0;
        }
        return result;
    }

    /// <summary>Draws a one pixel outline around (or just inside) the opaque shapes.</summary>
    public static PixelBuffer Outline(PixelBuffer source, bool[]? mask, uint color, bool inside, bool corners)
    {
        PixelBuffer result = source.Clone();
        int w = source.Width, h = source.Height;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int i = y * w + x;
                if (mask != null && !mask[i]) continue;
                bool solid = source.Pixels[i] >> 24 != 0;
                if (solid != inside) continue;
                bool edge = false;
                for (int oy = -1; oy <= 1 && !edge; oy++)
                {
                    for (int ox = -1; ox <= 1; ox++)
                    {
                        if (ox == 0 && oy == 0 || !corners && ox != 0 && oy != 0) continue;
                        bool neighbor = source.GetOrDefault(x + ox, y + oy) >> 24 != 0;
                        if (neighbor != solid)
                        {
                            edge = true;
                            break;
                        }
                    }
                }
                if (edge) result.Pixels[i] = color;
            }
        }
        return result;
    }

    public static PixelBuffer DropShadow(PixelBuffer source, bool[]? mask, uint color, int dx, int dy)
    {
        PixelBuffer result = source.Clone();
        int w = source.Width, h = source.Height;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int i = y * w + x;
                if (mask != null && !mask[i]) continue;
                if (source.Pixels[i] >> 24 == 255) continue;
                uint caster = source.GetOrDefault(x - dx, y - dy);
                if (caster >> 24 == 0) continue;
                result.Pixels[i] = Argb.Over(Argb.WithAlpha(color, Argb.A(color) * Argb.A(caster) / 255), source.Pixels[i]);
            }
        }
        return result;
    }

    /// <summary>Doubles the size while keeping edges crisp and rounding staircase corners (the EPX rule).</summary>
    public static PixelBuffer Scale2x(PixelBuffer source)
    {
        int w = source.Width, h = source.Height;
        PixelBuffer result = new(w * 2, h * 2);
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                uint p = source.Pixels[y * w + x];
                uint a = y > 0 ? source.Pixels[(y - 1) * w + x] : p, d = y < h - 1 ? source.Pixels[(y + 1) * w + x] : p;
                uint c = x > 0 ? source.Pixels[y * w + x - 1] : p, b = x < w - 1 ? source.Pixels[y * w + x + 1] : p;
                uint p1 = p, p2 = p, p3 = p, p4 = p;
                if (c == a && c != d && a != b) p1 = a;
                if (a == b && a != c && b != d) p2 = b;
                if (d == c && d != b && c != a) p3 = c;
                if (b == d && b != a && d != c) p4 = d;
                int o = y * 2 * result.Width + x * 2;
                result.Pixels[o] = p1;
                result.Pixels[o + 1] = p2;
                result.Pixels[o + result.Width] = p3;
                result.Pixels[o + result.Width + 1] = p4;
            }
        }
        return result;
    }

    public static PixelBuffer Resize(PixelBuffer source, int width, int height, ResizeMethod method)
    {
        if (width == source.Width && height == source.Height) return source.Clone();
        switch (method)
        {
            case ResizeMethod.Smooth:
                using (SKBitmap bitmap = source.ToBitmap())
                using (SKBitmap premul = new(new SKImageInfo(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Premul)))
                {
                    bitmap.CopyTo(premul);
                    using SKBitmap scaled = premul.Resize(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul), new SKSamplingOptions(SKCubicResampler.Mitchell));
                    if (scaled != null) return PixelBuffer.FromBitmap(scaled);
                }
                return source.ScaleNearest(width, height);
            case ResizeMethod.PixelArtScale:
                PixelBuffer current = source;
                while (current.Width * 2 <= width && current.Height * 2 <= height && current.Width < 4096 && current.Height < 4096) current = Scale2x(current);
                return current.Width == width && current.Height == height ? current : current.ScaleNearest(width, height);
            default:
                return source.ScaleNearest(width, height);
        }
    }

    /// <summary>
    /// Draws a transformed copy of a source image onto a buffer with nearest neighbor sampling: scaled to a destination
    /// rectangle, rotated around its center and optionally flipped. Reports the pixels that were covered.
    /// </summary>
    public static void DrawTransformed(PixelBuffer target, PixelBuffer source, bool[]? sourceMask, double x, double y, double width, double height, double angleDegrees, bool flipX, bool flipY, bool[]? covered)
    {
        if (width < 0.5 || height < 0.5) return;
        double cx = x + width / 2, cy = y + height / 2, radians = angleDegrees * Math.PI / 180, cos = Math.Cos(radians), sin = Math.Sin(radians);
        double hw = width / 2, hh = height / 2;
        double extentX = Math.Abs(hw * cos) + Math.Abs(hh * sin), extentY = Math.Abs(hw * sin) + Math.Abs(hh * cos);
        int left = (int)Math.Floor(cx - extentX), right = (int)Math.Ceiling(cx + extentX), top = (int)Math.Floor(cy - extentY), bottom = (int)Math.Ceiling(cy + extentY);
        for (int py = top; py < bottom; py++)
        {
            for (int px = left; px < right; px++)
            {
                int tx = px, ty = py;
                if ((uint)tx >= (uint)target.Width || (uint)ty >= (uint)target.Height) continue;
                // Inverse map the pixel center into the unrotated destination rectangle, then into the source.
                double dx = px + 0.5 - cx, dy = py + 0.5 - cy;
                double ux = dx * cos + dy * sin + hw, uy = -dx * sin + dy * cos + hh;
                if (ux < 0 || uy < 0 || ux >= width || uy >= height) continue;
                int sx = Math.Min(source.Width - 1, (int)(ux * source.Width / width)), sy = Math.Min(source.Height - 1, (int)(uy * source.Height / height));
                if (flipX) sx = source.Width - 1 - sx;
                if (flipY) sy = source.Height - 1 - sy;
                if (sourceMask != null && !sourceMask[sy * source.Width + sx]) continue;
                int i = ty * target.Width + tx;
                if (covered != null) covered[i] = true;
                uint c = source.Pixels[sy * source.Width + sx];
                if (c >> 24 != 0) target.Pixels[i] = Argb.Over(target.Pixels[i], c);
            }
        }
    }
}
