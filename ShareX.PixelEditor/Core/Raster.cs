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

/// <summary>Integer rasterizers that report the pixels of lines, ellipses, polygons and fills.</summary>
public static class Raster
{
    public static void Line(int x0, int y0, int x1, int y1, Action<int, int> plot)
    {
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;
        while (true)
        {
            plot(x0, y0);
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy)
            {
                err += dy;
                x0 += sx;
            }
            if (e2 <= dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    public static List<SKPointI> LinePoints(int x0, int y0, int x1, int y1)
    {
        List<SKPointI> points = [];
        Line(x0, y0, x1, y1, (x, y) => points.Add(new SKPointI(x, y)));
        return points;
    }

    public static void Rect(int x0, int y0, int x1, int y1, bool filled, Action<int, int> plot)
    {
        if (x0 > x1) (x0, x1) = (x1, x0);
        if (y0 > y1) (y0, y1) = (y1, y0);
        if (filled)
        {
            for (int y = y0; y <= y1; y++)
                for (int x = x0; x <= x1; x++)
                    plot(x, y);
            return;
        }
        for (int x = x0; x <= x1; x++)
        {
            plot(x, y0);
            if (y1 != y0) plot(x, y1);
        }
        for (int y = y0 + 1; y < y1; y++)
        {
            plot(x0, y);
            if (x1 != x0) plot(x1, y);
        }
    }

    /// <summary>An ellipse that exactly fits the rectangle between two corner pixels, so even sizes stay symmetric.</summary>
    public static void Ellipse(int x0, int y0, int x1, int y1, bool filled, Action<int, int> plot)
    {
        if (x0 > x1) (x0, x1) = (x1, x0);
        if (y0 > y1) (y0, y1) = (y1, y0);
        long a = x1 - x0, b = y1 - y0, b1 = b & 1;
        if (a == 0 || b == 0)
        {
            Rect(x0, y0, x1, y1, true, plot);
            return;
        }
        double dx = 4 * (1 - a) * b * b, dy = 4 * (b1 + 1) * a * a;
        double err = dx + dy + b1 * a * a;
        y0 += (int)((b + 1) / 2);
        y1 = y0 - (int)b1;
        a *= 8 * a;
        b1 = 8 * b * b;

        void Row(int left, int right, int y)
        {
            if (filled)
            {
                for (int x = left; x <= right; x++) plot(x, y);
            }
            else
            {
                plot(left, y);
                if (right != left) plot(right, y);
            }
        }

        do
        {
            Row(x0, x1, y0);
            if (y1 != y0) Row(x0, x1, y1);
            double e2 = 2 * err;
            if (e2 <= dy)
            {
                y0++;
                y1--;
                err += dy += a;
            }
            if (e2 >= dx || 2 * err > dy)
            {
                x0++;
                x1--;
                err += dx += b1;
            }
        }
        while (x0 <= x1);

        while (y0 - y1 < b)
        {
            Row(x0 - 1, x1 + 1, y0++);
            Row(x0 - 1, x1 + 1, y1--);
        }
    }

    /// <summary>Fills a polygon with the even-odd rule, sampling at pixel centers, and includes its outline.</summary>
    public static void FillPolygon(IReadOnlyList<SKPointI> points, Action<int, int> plot)
    {
        if (points.Count == 0) return;
        if (points.Count < 3)
        {
            Line(points[0].X, points[0].Y, points[^1].X, points[^1].Y, plot);
            return;
        }
        int minY = points.Min(p => p.Y), maxY = points.Max(p => p.Y);
        List<double> crossings = [];
        for (int y = minY; y <= maxY; y++)
        {
            crossings.Clear();
            double center = y + 0.5;
            for (int i = 0; i < points.Count; i++)
            {
                SKPointI p = points[i], q = points[(i + 1) % points.Count];
                double py = p.Y + 0.5, qy = q.Y + 0.5;
                if (py == qy) continue;
                if (center < Math.Min(py, qy) || center >= Math.Max(py, qy)) continue;
                crossings.Add(p.X + 0.5 + (center - py) * (q.X - p.X) / (qy - py));
            }
            crossings.Sort();
            for (int i = 0; i + 1 < crossings.Count; i += 2)
            {
                int left = (int)Math.Ceiling(crossings[i] - 0.5), right = (int)Math.Floor(crossings[i + 1] - 0.5);
                for (int x = left; x <= right; x++) plot(x, y);
            }
        }
        for (int i = 0; i < points.Count; i++)
        {
            SKPointI p = points[i], q = points[(i + 1) % points.Count];
            Line(p.X, p.Y, q.X, q.Y, plot);
        }
    }

    /// <summary>
    /// Finds the region a fill covers: pixels whose color is within the tolerance of the start pixel. A contiguous fill
    /// spreads through neighbors (optionally diagonals too); otherwise every matching pixel of the buffer is taken.
    /// </summary>
    public static bool[] FloodRegion(PixelBuffer source, int startX, int startY, int tolerance, bool contiguous, bool diagonal, bool[]? clip = null)
    {
        int w = source.Width, h = source.Height;
        bool[] region = new bool[w * h];
        if (!source.Contains(startX, startY)) return region;
        uint target = source[startX, startY];
        // Tolerance 0..255 maps onto the squared distance metric of Argb.Distance (max 12 * 255^2).
        long limit = (long)tolerance * tolerance * 12;
        bool Match(int index) => tolerance == 0 ? source.Pixels[index] == target || (source.Pixels[index] >> 24 == 0 && target >> 24 == 0) : Argb.Distance(source.Pixels[index], target) <= limit;

        if (!contiguous)
        {
            for (int i = 0; i < region.Length; i++) region[i] = (clip == null || clip[i]) && Match(i);
            return region;
        }

        if (clip != null && !clip[startY * w + startX]) return region;
        Stack<(int X, int Y)> stack = new();
        stack.Push((startX, startY));
        while (stack.Count > 0)
        {
            (int x, int y) = stack.Pop();
            int row = y * w;
            if (region[row + x]) continue;
            int left = x, right = x;
            while (left > 0 && !region[row + left - 1] && (clip == null || clip[row + left - 1]) && Match(row + left - 1)) left--;
            while (right < w - 1 && !region[row + right + 1] && (clip == null || clip[row + right + 1]) && Match(row + right + 1)) right++;
            for (int i = left; i <= right; i++) region[row + i] = true;
            int from = diagonal ? Math.Max(0, left - 1) : left, to = diagonal ? Math.Min(w - 1, right + 1) : right;
            for (int ny = y - 1; ny <= y + 1; ny += 2)
            {
                if ((uint)ny >= (uint)h) continue;
                int nrow = ny * w;
                bool inRun = false;
                for (int i = from; i <= to; i++)
                {
                    bool ok = !region[nrow + i] && (clip == null || clip[nrow + i]) && Match(nrow + i);
                    if (ok && !inRun) stack.Push((i, ny));
                    inRun = ok;
                }
            }
        }
        return region;
    }

    /// <summary>Offsets of the pixels of a brush tip, relative to the pixel under the cursor.</summary>
    public static SKPointI[] BrushTip(int size, bool round)
    {
        size = Math.Clamp(size, 1, 256);
        List<SKPointI> points = [];
        int origin = -(size / 2);
        double radius = size / 2.0, center = size / 2.0;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (round && size > 2)
                {
                    double dx = x + 0.5 - center, dy = y + 0.5 - center;
                    if (dx * dx + dy * dy > radius * radius) continue;
                }
                points.Add(new SKPointI(origin + x, origin + y));
            }
        }
        return points.ToArray();
    }

    /// <summary>4x4 and 8x8 Bayer matrices for ordered dithering; values are 0..(n*n-1).</summary>
    public static readonly int[] Bayer4 = [0, 8, 2, 10, 12, 4, 14, 6, 3, 11, 1, 9, 15, 7, 13, 5];

    public static readonly int[] Bayer8 =
    [
        0, 32, 8, 40, 2, 34, 10, 42, 48, 16, 56, 24, 50, 18, 58, 26, 12, 44, 4, 36, 14, 46, 6, 38, 60, 28, 52, 20, 62, 30, 54, 22,
        3, 35, 11, 43, 1, 33, 9, 41, 51, 19, 59, 27, 49, 17, 57, 25, 15, 47, 7, 39, 13, 45, 5, 37, 63, 31, 55, 23, 61, 29, 53, 21
    ];

    /// <summary>Ordered dithering threshold in 0..1 for a pixel position.</summary>
    public static float BayerThreshold(int x, int y, bool large) =>
        large ? (Bayer8[(y & 7) * 8 + (x & 7)] + 0.5f) / 64f : (Bayer4[(y & 3) * 4 + (x & 3)] + 0.5f) / 16f;
}
