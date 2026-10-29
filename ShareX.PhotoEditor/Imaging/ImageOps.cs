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

namespace ShareX.PhotoEditor.Imaging;

public enum ResampleMethod
{
    NearestNeighbor,
    Bilinear,
    Bicubic,
    BicubicSmoother,
    BicubicSharper,
    Lanczos
}

/// <summary>General bitmap operations shared by commands and tools.</summary>
public static class ImageOps
{
    public static SKBitmap CreateBitmap(int width, int height) =>
        new(new SKImageInfo(Math.Max(1, width), Math.Max(1, height), SKColorType.Bgra8888, SKAlphaType.Premul));

    public static SKSamplingOptions GetSampling(ResampleMethod method) => method switch
    {
        ResampleMethod.NearestNeighbor => new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None),
        ResampleMethod.Bilinear => new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear),
        ResampleMethod.BicubicSmoother => new SKSamplingOptions(SKCubicResampler.Mitchell),
        ResampleMethod.BicubicSharper => new SKSamplingOptions(new SKCubicResampler(0, 0.75f)),
        ResampleMethod.Lanczos => new SKSamplingOptions(SKCubicResampler.CatmullRom),
        _ => new SKSamplingOptions(SKCubicResampler.CatmullRom)
    };

    public static unsafe uint[] ToPremulArray(SKBitmap bitmap)
    {
        using SKBitmap converted = bitmap.ColorType == SKColorType.Bgra8888 && bitmap.AlphaType == SKAlphaType.Premul ? null! : Convert(bitmap);
        SKBitmap source = converted ?? bitmap;
        uint[] result = new uint[source.Width * source.Height];
        uint* pixels = (uint*)source.GetPixels();
        int stride = source.RowBytes / 4;
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++) result[y * source.Width + x] = pixels[y * stride + x];
        }
        return result;
    }

    public static SKBitmap Convert(SKBitmap source)
    {
        SKBitmap result = CreateBitmap(source.Width, source.Height);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }

    /// <summary>Returns the bounds of pixels with non-zero alpha, or empty.</summary>
    public static unsafe SKRectI GetOpaqueBounds(SKBitmap bitmap, int threshold = 0)
    {
        int width = bitmap.Width, height = bitmap.Height;
        int left = width, top = height, right = -1, bottom = -1;
        if (bitmap.ColorType == SKColorType.Alpha8)
        {
            byte* data = (byte*)bitmap.GetPixels();
            for (int y = 0; y < height; y++)
            {
                byte* row = data + y * bitmap.RowBytes;
                for (int x = 0; x < width; x++)
                {
                    if (row[x] > threshold)
                    {
                        if (x < left) left = x;
                        if (x > right) right = x;
                        if (y < top) top = y;
                        bottom = y;
                    }
                }
            }
        }
        else
        {
            uint* data = (uint*)bitmap.GetPixels();
            int stride = bitmap.RowBytes / 4;
            for (int y = 0; y < height; y++)
            {
                uint* row = data + y * stride;
                for (int x = 0; x < width; x++)
                {
                    if ((row[x] >> 24) > threshold)
                    {
                        if (x < left) left = x;
                        if (x > right) right = x;
                        if (y < top) top = y;
                        bottom = y;
                    }
                }
            }
        }
        return right < 0 ? SKRectI.Empty : new SKRectI(left, top, right + 1, bottom + 1);
    }

    public static SKBitmap Crop(SKBitmap source, SKRectI rect)
    {
        SKBitmap result = CreateBitmap(rect.Width, rect.Height);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(source, -rect.Left, -rect.Top);
        return result;
    }

    public static SKBitmap Resize(SKBitmap source, int width, int height, ResampleMethod method)
    {
        SKBitmap result = CreateBitmap(width, height);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);
        using SKImage image = SKImage.FromBitmap(source);
        SKSamplingOptions sampling = GetSampling(method);
        if (width < source.Width / 2 && method != ResampleMethod.NearestNeighbor)
        {
            sampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
        }
        canvas.DrawImage(image, SKRect.Create(source.Width, source.Height), SKRect.Create(width, height), sampling);
        return result;
    }

    /// <summary>Draws a bitmap through a transform into a new bitmap of the given size.</summary>
    public static SKBitmap Transform(SKBitmap source, SKMatrix matrix, int width, int height, ResampleMethod method = ResampleMethod.Bicubic)
    {
        SKBitmap result = CreateBitmap(width, height);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);
        canvas.SetMatrix(matrix);
        using SKImage image = SKImage.FromBitmap(source);
        using SKPaint paint = new() { IsAntialias = method != ResampleMethod.NearestNeighbor };
        canvas.DrawImage(image, 0, 0, GetSampling(method), paint);
        return result;
    }

    public static SKBitmap Flip(SKBitmap source, bool horizontal)
    {
        SKMatrix matrix = horizontal ? new SKMatrix(-1, 0, source.Width, 0, 1, 0, 0, 0, 1) : new SKMatrix(1, 0, 0, 0, -1, source.Height, 0, 0, 1);
        return Transform(source, matrix, source.Width, source.Height, ResampleMethod.NearestNeighbor);
    }

    /// <summary>
    /// Computes the projective matrix mapping the unit-aligned rectangle <paramref name="source"/> to an arbitrary quad.
    /// Quad order: top-left, top-right, bottom-right, bottom-left.
    /// </summary>
    public static SKMatrix RectToQuad(SKRect source, IReadOnlyList<SKPoint> quad)
    {
        SKMatrix unitToQuad = UnitSquareToQuad(quad[0], quad[1], quad[2], quad[3]);
        SKMatrix rectToUnit = SKMatrix.CreateScale(1 / Math.Max(1e-6f, source.Width), 1 / Math.Max(1e-6f, source.Height))
            .PreConcat(SKMatrix.CreateTranslation(-source.Left, -source.Top));
        return unitToQuad.PreConcat(rectToUnit);
    }

    /// <summary>Inverse of <see cref="RectToQuad"/>: maps a quad onto an axis aligned rectangle.</summary>
    public static SKMatrix QuadToRect(IReadOnlyList<SKPoint> quad, SKRect destination)
    {
        SKMatrix forward = RectToQuad(destination, quad);
        return forward.TryInvert(out SKMatrix inverse) ? inverse : SKMatrix.Identity;
    }

    private static SKMatrix UnitSquareToQuad(SKPoint p0, SKPoint p1, SKPoint p2, SKPoint p3)
    {
        // Heckbert's projective mapping from the unit square (0,0),(1,0),(1,1),(0,1).
        float x0 = p0.X, y0 = p0.Y, x1 = p1.X, y1 = p1.Y, x2 = p2.X, y2 = p2.Y, x3 = p3.X, y3 = p3.Y;
        float sx = x0 - x1 + x2 - x3;
        float sy = y0 - y1 + y2 - y3;
        float a, b, c, d, e, f, g, h;
        if (MathF.Abs(sx) < 1e-6f && MathF.Abs(sy) < 1e-6f)
        {
            a = x1 - x0; b = x2 - x1; c = x0;
            d = y1 - y0; e = y2 - y1; f = y0;
            g = 0; h = 0;
        }
        else
        {
            float dx1 = x1 - x2, dx2 = x3 - x2, dy1 = y1 - y2, dy2 = y3 - y2;
            float det = dx1 * dy2 - dx2 * dy1;
            if (MathF.Abs(det) < 1e-9f) det = 1e-9f;
            g = (sx * dy2 - dx2 * sy) / det;
            h = (dx1 * sy - sx * dy1) / det;
            a = x1 - x0 + g * x1; b = x3 - x0 + h * x3; c = x0;
            d = y1 - y0 + g * y1; e = y3 - y0 + h * y3; f = y0;
            return new SKMatrix(a, b, c, d, e, f, g, h, 1);
        }
        // Affine case: columns are (u, v) basis vectors where v maps to p3 - p0.
        return new SKMatrix(a, x3 - x0, c, d, y3 - y0, f, 0, 0, 1);
    }

    /// <summary>Copies pixels of a bitmap inside a mask into a new bitmap.</summary>
    public static unsafe SKBitmap ExtractMasked(SKBitmap source, SKPointI sourceOffset, PixelSurface mask, SKRectI documentRect)
    {
        SKBitmap result = CreateBitmap(documentRect.Width, documentRect.Height);
        result.Erase(SKColors.Transparent);
        uint* dst = (uint*)result.GetPixels();
        uint* src = (uint*)source.GetPixels();
        byte* m = (byte*)mask.Bitmap.GetPixels();
        int srcStride = source.RowBytes / 4, dstStride = result.RowBytes / 4, maskStride = mask.RowBytes;
        for (int y = 0; y < documentRect.Height; y++)
        {
            int dy = documentRect.Top + y;
            int sy = dy - sourceOffset.Y;
            if (dy < 0 || dy >= mask.Height || sy < 0 || sy >= source.Height) continue;
            for (int x = 0; x < documentRect.Width; x++)
            {
                int dx = documentRect.Left + x;
                int sx = dx - sourceOffset.X;
                if (dx < 0 || dx >= mask.Width || sx < 0 || sx >= source.Width) continue;
                byte coverage = m[dy * maskStride + dx];
                if (coverage == 0) continue;
                dst[y * dstStride + x] = ScalePixel(src[sy * srcStride + sx], coverage);
            }
        }
        return result;
    }

    public static uint ScalePixel(uint p, int coverage)
    {
        if (coverage >= 255) return p;
        uint a = (uint)(((p >> 24) * coverage + 127) / 255);
        uint r = (uint)((((p >> 16) & 0xFF) * coverage + 127) / 255);
        uint g = (uint)((((p >> 8) & 0xFF) * coverage + 127) / 255);
        uint b = (uint)(((p & 0xFF) * coverage + 127) / 255);
        return (a << 24) | (r << 16) | (g << 8) | b;
    }

    /// <summary>Erases pixels covered by the mask, proportionally to coverage.</summary>
    public static unsafe void EraseMasked(SKBitmap target, SKPointI targetOffset, PixelSurface mask, SKRectI documentRect)
    {
        uint* dst = (uint*)target.GetPixels();
        byte* m = (byte*)mask.Bitmap.GetPixels();
        int stride = target.RowBytes / 4, maskStride = mask.RowBytes;
        for (int dy = Math.Max(0, documentRect.Top); dy < Math.Min(mask.Height, documentRect.Bottom); dy++)
        {
            int ty = dy - targetOffset.Y;
            if (ty < 0 || ty >= target.Height) continue;
            for (int dx = Math.Max(0, documentRect.Left); dx < Math.Min(mask.Width, documentRect.Right); dx++)
            {
                int tx = dx - targetOffset.X;
                if (tx < 0 || tx >= target.Width) continue;
                byte coverage = m[dy * maskStride + dx];
                if (coverage == 0) continue;
                dst[ty * stride + tx] = ScalePixel(dst[ty * stride + tx], 255 - coverage);
            }
        }
    }

    /// <summary>Returns the premultiplied pixel at a position or transparent if outside.</summary>
    public static unsafe uint GetPixel(SKBitmap bitmap, int x, int y)
    {
        if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height) return 0;
        return ((uint*)bitmap.GetPixels())[y * (bitmap.RowBytes / 4) + x];
    }

    /// <summary>Averages a square sample area (premultiplied average, returned unpremultiplied).</summary>
    public static unsafe SKColor SampleAverage(SKBitmap bitmap, int cx, int cy, int size)
    {
        int half = size / 2;
        float r = 0, g = 0, b = 0, a = 0;
        int count = 0;
        for (int y = cy - half; y <= cy - half + size - 1; y++)
        {
            for (int x = cx - half; x <= cx - half + size - 1; x++)
            {
                if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height) continue;
                uint p = GetPixel(bitmap, x, y);
                a += p >> 24; r += (p >> 16) & 0xFF; g += (p >> 8) & 0xFF; b += p & 0xFF;
                count++;
            }
        }
        if (count == 0 || a <= 0) return SKColors.Transparent;
        return new SKColor((byte)Math.Clamp(r / a * 255, 0, 255), (byte)Math.Clamp(g / a * 255, 0, 255), (byte)Math.Clamp(b / a * 255, 0, 255), (byte)Math.Clamp(a / count, 0, 255));
    }

    public static SKBitmap Rotate(SKBitmap source, float degrees, out SKPoint offset, SKColor? background = null)
    {
        float radians = degrees * MathF.PI / 180;
        float cos = MathF.Abs(MathF.Cos(radians)), sin = MathF.Abs(MathF.Sin(radians));
        int width = (int)MathF.Ceiling(source.Width * cos + source.Height * sin - 0.001f);
        int height = (int)MathF.Ceiling(source.Width * sin + source.Height * cos - 0.001f);
        SKMatrix matrix = SKMatrix.CreateTranslation(-source.Width / 2f, -source.Height / 2f)
            .PostConcat(SKMatrix.CreateRotationDegrees(degrees))
            .PostConcat(SKMatrix.CreateTranslation(width / 2f, height / 2f));
        offset = new SKPoint((width - source.Width) / 2f, (height - source.Height) / 2f);
        SKBitmap result = CreateBitmap(width, height);
        using SKCanvas canvas = new(result);
        canvas.Clear(background ?? SKColors.Transparent);
        canvas.SetMatrix(matrix);
        using SKImage image = SKImage.FromBitmap(source);
        using SKPaint paint = new() { IsAntialias = true };
        canvas.DrawImage(image, 0, 0, new SKSamplingOptions(SKCubicResampler.CatmullRom), paint);
        return result;
    }

    /// <summary>Converts a BGRA bitmap to a grayscale mask using luminance (used for channels and masks).</summary>
    public static unsafe PixelSurface ToMask(SKBitmap source, bool useAlpha)
    {
        PixelSurface mask = PixelSurface.CreateMask(source.Width, source.Height);
        byte* dst = (byte*)mask.Bitmap.GetPixels();
        uint* src = (uint*)source.GetPixels();
        int stride = source.RowBytes / 4;
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                uint p = src[y * stride + x];
                if (useAlpha)
                {
                    dst[y * mask.RowBytes + x] = (byte)(p >> 24);
                }
                else
                {
                    ColorMath.Unpack(p, out float r, out float g, out float b, out float a);
                    dst[y * mask.RowBytes + x] = (byte)Math.Clamp(ColorMath.Luminance(r, g, b) * a * 255, 0, 255);
                }
            }
        }
        return mask;
    }

    /// <summary>Renders an A8 mask as an opaque grayscale bitmap.</summary>
    public static unsafe SKBitmap MaskToGray(PixelSurface mask)
    {
        SKBitmap result = CreateBitmap(mask.Width, mask.Height);
        uint* dst = (uint*)result.GetPixels();
        byte* src = (byte*)mask.Bitmap.GetPixels();
        int stride = result.RowBytes / 4;
        for (int y = 0; y < mask.Height; y++)
        {
            for (int x = 0; x < mask.Width; x++)
            {
                uint v = src[y * mask.RowBytes + x];
                dst[y * stride + x] = 0xFF000000u | (v << 16) | (v << 8) | v;
            }
        }
        return result;
    }
}
