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

public enum SelectionMode
{
    Replace,
    Add,
    Subtract,
    Intersect
}

/// <summary>Operations on 8-bit masks (selections, layer masks and channels).</summary>
public static class MaskOps
{
    public static unsafe byte* Ptr(PixelSurface mask) => (byte*)mask.Pixels;

    /// <summary>Rasterizes a path into a new mask.</summary>
    public static PixelSurface FromPath(SKPath path, int width, int height, bool antialias = true)
    {
        PixelSurface mask = PixelSurface.CreateMask(width, height);
        using SKCanvas canvas = new(mask.Bitmap);
        using SKPaint paint = new() { Color = SKColors.White, IsAntialias = antialias, Style = SKPaintStyle.Fill };
        canvas.DrawPath(path, paint);
        return mask;
    }

    public static PixelSurface FromRect(SKRectI rect, int width, int height)
    {
        PixelSurface mask = PixelSurface.CreateMask(width, height);
        rect.Intersect(new SKRectI(0, 0, width, height));
        Span<byte> span = mask.GetSpan();
        for (int y = rect.Top; y < rect.Bottom; y++) span.Slice(y * mask.RowBytes + rect.Left, rect.Width).Fill(255);
        return mask;
    }

    /// <summary>Combines an existing selection with a new shape. Either input may be null (no selection).</summary>
    public static unsafe PixelSurface? Combine(PixelSurface? current, PixelSurface shape, SelectionMode mode)
    {
        if (mode == SelectionMode.Replace || current == null)
        {
            if (mode == SelectionMode.Subtract || mode == SelectionMode.Intersect && current == null) return mode == SelectionMode.Intersect ? null : current;
            return shape;
        }
        PixelSurface result = current.Clone();
        byte* r = Ptr(result);
        byte* s = Ptr(shape);
        int length = result.RowBytes * result.Height;
        for (int i = 0; i < length; i++)
        {
            int a = r[i], b = s[i];
            r[i] = mode switch
            {
                SelectionMode.Add => (byte)Math.Max(a, b),
                SelectionMode.Subtract => (byte)(a * (255 - b) / 255),
                SelectionMode.Intersect => (byte)(a * b / 255),
                _ => (byte)b
            };
        }
        return result;
    }

    public static unsafe void Invert(PixelSurface mask)
    {
        byte* p = Ptr(mask);
        int length = mask.RowBytes * mask.Height;
        for (int i = 0; i < length; i++) p[i] = (byte)(255 - p[i]);
    }

    public static unsafe bool IsEmpty(PixelSurface mask)
    {
        byte* p = Ptr(mask);
        int length = mask.RowBytes * mask.Height;
        for (int i = 0; i < length; i++) if (p[i] != 0) return false;
        return true;
    }

    public static SKRectI GetBounds(PixelSurface mask) => ImageOps.GetOpaqueBounds(mask.Bitmap);

    /// <summary>Gaussian-like feather using three box blur passes.</summary>
    public static unsafe void Blur(PixelSurface mask, float radius)
    {
        if (radius <= 0) return;
        int width = mask.Width, height = mask.Height, stride = mask.RowBytes;
        float sigma = radius / 2f;
        int[] boxes = BoxesForGauss(sigma, 3);
        byte[] temp = new byte[stride * height];
        byte* data = Ptr(mask);
        fixed (byte* t = temp)
        {
            foreach (int box in boxes)
            {
                int r = (box - 1) / 2;
                BoxBlurH(data, t, width, height, stride, r);
                BoxBlurV(t, data, width, height, stride, r);
            }
        }
    }

    public static int[] BoxesForGauss(float sigma, int n)
    {
        float wIdeal = MathF.Sqrt(12 * sigma * sigma / n + 1);
        int wl = (int)MathF.Floor(wIdeal);
        if (wl % 2 == 0) wl--;
        int wu = wl + 2;
        float mIdeal = (12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
        int m = (int)MathF.Round(mIdeal);
        int[] sizes = new int[n];
        for (int i = 0; i < n; i++) sizes[i] = Math.Max(1, i < m ? wl : wu);
        return sizes;
    }

    private static unsafe void BoxBlurH(byte* src, byte* dst, int width, int height, int stride, int r)
    {
        if (r <= 0)
        {
            Buffer.MemoryCopy(src, dst, (long)stride * height, (long)stride * height);
            return;
        }
        Parallel.For(0, height, y =>
        {
            byte* s = src + y * stride;
            byte* d = dst + y * stride;
            int sum = 0;
            int window = r * 2 + 1;
            for (int x = -r - 1; x < r; x++) sum += s[Math.Clamp(x, 0, width - 1)];
            for (int x = 0; x < width; x++)
            {
                sum += s[Math.Min(x + r, width - 1)] - s[Math.Max(x - r - 1, 0)];
                d[x] = (byte)((sum + window / 2) / window);
            }
        });
    }

    private static unsafe void BoxBlurV(byte* src, byte* dst, int width, int height, int stride, int r)
    {
        if (r <= 0)
        {
            Buffer.MemoryCopy(src, dst, (long)stride * height, (long)stride * height);
            return;
        }
        Parallel.For(0, width, x =>
        {
            int sum = 0;
            int window = r * 2 + 1;
            for (int y = -r - 1; y < r; y++) sum += src[Math.Clamp(y, 0, height - 1) * stride + x];
            for (int y = 0; y < height; y++)
            {
                sum += src[Math.Min(y + r, height - 1) * stride + x] - src[Math.Max(y - r - 1, 0) * stride + x];
                dst[y * stride + x] = (byte)((sum + window / 2) / window);
            }
        });
    }

    /// <summary>Exact squared Euclidean distance transform (Felzenszwalb and Huttenlocher).</summary>
    public static float[] DistanceTransform(PixelSurface mask, bool toSelected, byte threshold = 128)
    {
        int width = mask.Width, height = mask.Height;
        const float inf = 1e20f;
        float[] grid = new float[width * height];
        unsafe
        {
            byte* p = Ptr(mask);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool selected = p[y * mask.RowBytes + x] >= threshold;
                    grid[y * width + x] = selected == toSelected ? 0 : inf;
                }
            }
        }
        Parallel.For(0, width, () => (new float[height], new float[height], new int[height], new float[height + 1]), (x, _, buffers) =>
        {
            var (f, d, v, z) = buffers;
            for (int y = 0; y < height; y++) f[y] = grid[y * width + x];
            Transform1D(f, height, d, v, z);
            for (int y = 0; y < height; y++) grid[y * width + x] = d[y];
            return buffers;
        }, _ => { });
        Parallel.For(0, height, () => (new float[width], new float[width], new int[width], new float[width + 1]), (y, _, buffers) =>
        {
            var (f, d, v, z) = buffers;
            for (int x = 0; x < width; x++) f[x] = grid[y * width + x];
            Transform1D(f, width, d, v, z);
            for (int x = 0; x < width; x++) grid[y * width + x] = d[x];
            return buffers;
        }, _ => { });
        return grid;
    }

    private static void Transform1D(float[] f, int n, float[] d, int[] v, float[] z)
    {
        int k = 0;
        v[0] = 0;
        z[0] = float.NegativeInfinity;
        z[1] = float.PositiveInfinity;
        for (int q = 1; q < n; q++)
        {
            float s;
            while (true)
            {
                s = ((f[q] + q * q) - (f[v[k]] + v[k] * v[k])) / (2f * q - 2f * v[k]);
                if (s <= z[k] && k > 0) k--;
                else break;
            }
            if (s <= z[k])
            {
                v[k] = q;
                z[k + 1] = float.PositiveInfinity;
                continue;
            }
            k++;
            v[k] = q;
            z[k] = s;
            z[k + 1] = float.PositiveInfinity;
        }
        k = 0;
        for (int q = 0; q < n; q++)
        {
            while (z[k + 1] < q) k++;
            float diff = q - v[k];
            d[q] = diff * diff + f[v[k]];
        }
    }

    /// <summary>Grows (positive) or shrinks (negative) a selection by a pixel radius.</summary>
    public static PixelSurface Morph(PixelSurface mask, float radius)
    {
        if (MathF.Abs(radius) < 0.5f) return mask.Clone();
        bool grow = radius > 0;
        float r2 = radius * radius;
        float[] distance = DistanceTransform(mask, grow);
        PixelSurface result = PixelSurface.CreateMask(mask.Width, mask.Height);
        unsafe
        {
            byte* p = Ptr(result);
            int width = mask.Width;
            for (int y = 0; y < mask.Height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float d = distance[y * width + x];
                    bool selected = grow ? d <= r2 : d > r2;
                    p[y * result.RowBytes + x] = selected ? (byte)255 : (byte)0;
                }
            }
        }
        return result;
    }

    public static PixelSurface Border(PixelSurface mask, float width)
    {
        PixelSurface outer = Morph(mask, width / 2);
        PixelSurface inner = Morph(mask, -width / 2);
        PixelSurface? result = Combine(outer, inner, SelectionMode.Subtract);
        inner.Dispose();
        Blur(result!, 1);
        return result!;
    }

    public static unsafe PixelSurface Smooth(PixelSurface mask, float radius)
    {
        PixelSurface result = mask.Clone();
        Blur(result, radius * 2);
        byte* p = Ptr(result);
        int length = result.RowBytes * result.Height;
        for (int i = 0; i < length; i++) p[i] = p[i] >= 128 ? (byte)255 : (byte)0;
        return result;
    }

    /// <summary>Hardens a soft selection to a binary one at 50%.</summary>
    public static unsafe void Threshold(PixelSurface mask, byte level = 128)
    {
        byte* p = Ptr(mask);
        int length = mask.RowBytes * mask.Height;
        for (int i = 0; i < length; i++) p[i] = p[i] >= level ? (byte)255 : (byte)0;
    }

    /// <summary>Builds outline segments of the 50% selection boundary for marching ants. Coordinates are pixel corners.</summary>
    public static unsafe List<(SKPoint A, SKPoint B)> TraceOutline(PixelSurface mask, int maxSegments = 400_000)
    {
        int width = mask.Width, height = mask.Height, stride = mask.RowBytes;
        byte* p = Ptr(mask);
        bool Sel(int x, int y) => x >= 0 && y >= 0 && x < width && y < height && p[y * stride + x] >= 128;
        List<(SKPoint, SKPoint)> segments = [];

        // Horizontal edges between row y-1 and y, merged into runs.
        for (int y = 0; y <= height; y++)
        {
            int runStart = -1;
            for (int x = 0; x <= width; x++)
            {
                bool edge = x < width && Sel(x, y - 1) != Sel(x, y);
                if (edge && runStart < 0) runStart = x;
                else if (!edge && runStart >= 0)
                {
                    segments.Add((new SKPoint(runStart, y), new SKPoint(x, y)));
                    runStart = -1;
                }
            }
            if (segments.Count > maxSegments) return segments;
        }
        // Vertical edges between column x-1 and x.
        for (int x = 0; x <= width; x++)
        {
            int runStart = -1;
            for (int y = 0; y <= height; y++)
            {
                bool edge = y < height && Sel(x - 1, y) != Sel(x, y);
                if (edge && runStart < 0) runStart = y;
                else if (!edge && runStart >= 0)
                {
                    segments.Add((new SKPoint(x, runStart), new SKPoint(x, y)));
                    runStart = -1;
                }
            }
            if (segments.Count > maxSegments) return segments;
        }
        return segments;
    }

    /// <summary>Traces closed contours of the selection as polygons (used for "Make Work Path").</summary>
    public static unsafe List<List<SKPoint>> TraceContours(PixelSurface mask)
    {
        int width = mask.Width, height = mask.Height, stride = mask.RowBytes;
        byte* p = Ptr(mask);
        bool Sel(int x, int y) => x >= 0 && y >= 0 && x < width && y < height && p[y * stride + x] >= 128;

        // Directed boundary edges keep the selected pixel on the right; chaining them gives closed loops.
        Dictionary<long, List<(int X, int Y)>> outgoing = [];
        long Key(int x, int y) => ((long)y << 32) | (uint)x;
        void AddEdge(int x0, int y0, int x1, int y1)
        {
            long key = Key(x0, y0);
            if (!outgoing.TryGetValue(key, out List<(int, int)>? list)) outgoing[key] = list = [];
            list.Add((x1, y1));
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!Sel(x, y)) continue;
                if (!Sel(x, y - 1)) AddEdge(x, y, x + 1, y);
                if (!Sel(x + 1, y)) AddEdge(x + 1, y, x + 1, y + 1);
                if (!Sel(x, y + 1)) AddEdge(x + 1, y + 1, x, y + 1);
                if (!Sel(x - 1, y)) AddEdge(x, y + 1, x, y);
            }
        }

        List<List<SKPoint>> contours = [];
        while (outgoing.Count > 0)
        {
            long startKey = outgoing.Keys.First();
            int sx = (int)(startKey & 0xFFFFFFFF), sy = (int)(startKey >> 32);
            List<SKPoint> contour = [];
            int cx = sx, cy = sy;
            int guard = 0;
            while (guard++ < 10_000_000)
            {
                long key = Key(cx, cy);
                if (!outgoing.TryGetValue(key, out List<(int X, int Y)>? list) || list.Count == 0) break;
                var next = list[^1];
                list.RemoveAt(list.Count - 1);
                if (list.Count == 0) outgoing.Remove(key);
                contour.Add(new SKPoint(cx, cy));
                cx = next.X;
                cy = next.Y;
                if (cx == sx && cy == sy) break;
            }
            if (contour.Count >= 3)
            {
                // Remove collinear points.
                List<SKPoint> reduced = [];
                for (int i = 0; i < contour.Count; i++)
                {
                    SKPoint prev = contour[(i - 1 + contour.Count) % contour.Count];
                    SKPoint cur = contour[i];
                    SKPoint next = contour[(i + 1) % contour.Count];
                    if ((cur.X - prev.X) * (next.Y - cur.Y) - (cur.Y - prev.Y) * (next.X - cur.X) != 0) reduced.Add(cur);
                }
                if (reduced.Count >= 3) contours.Add(reduced);
            }
        }
        return contours;
    }

    /// <summary>Paints a stroke along the selection edge (Edit &gt; Stroke).</summary>
    public static unsafe SKPath ToPath(PixelSurface mask, float tolerance = 0.75f)
    {
        SKPath path = new() { FillType = SKPathFillType.EvenOdd };
        foreach (List<SKPoint> contour in TraceContours(mask))
        {
            List<SKPoint> simplified = VectorPath.Simplify(contour.Append(contour[0]).ToList(), tolerance);
            if (simplified.Count < 3) continue;
            path.MoveTo(simplified[0]);
            for (int i = 1; i < simplified.Count; i++) path.LineTo(simplified[i]);
            path.Close();
        }
        return path;
    }

    public static unsafe byte Get(PixelSurface mask, int x, int y)
    {
        if (x < 0 || y < 0 || x >= mask.Width || y >= mask.Height) return 0;
        return Ptr(mask)[y * mask.RowBytes + x];
    }

    /// <summary>Resizes a mask to new document dimensions with a transform.</summary>
    public static PixelSurface Transform(PixelSurface mask, SKMatrix matrix, int width, int height, bool antialias = true)
    {
        PixelSurface result = PixelSurface.CreateMask(width, height);
        using SKCanvas canvas = new(result.Bitmap);
        canvas.SetMatrix(matrix);
        using SKImage image = SKImage.FromBitmap(mask.Bitmap);
        using SKPaint paint = new() { IsAntialias = antialias };
        canvas.DrawImage(image, 0, 0, new SKSamplingOptions(SKFilterMode.Linear), paint);
        return result;
    }

    /// <summary>Expands the canvas of a mask, filling new areas with the given value.</summary>
    public static PixelSurface Reframe(PixelSurface mask, int width, int height, int offsetX, int offsetY, byte fill)
    {
        PixelSurface result = PixelSurface.CreateMask(width, height, fill);
        using SKCanvas canvas = new(result.Bitmap);
        using SKPaint paint = new() { BlendMode = SKBlendMode.Src };
        canvas.DrawBitmap(mask.Bitmap, offsetX, offsetY, paint);
        return result;
    }
}
