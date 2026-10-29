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

/// <summary>Poisson (gradient-domain) blending used by the healing brush, patch tool and content-aware move.</summary>
public static class PoissonBlend
{
    /// <summary>
    /// Blends <paramref name="source"/> into <paramref name="target"/> inside the mask so that the source texture
    /// is kept while colors and lighting match the target along the mask boundary.
    /// All buffers are premultiplied BGRA in the same coordinate space; mask is 0..255.
    /// </summary>
    public static unsafe void Blend(SKBitmap target, SKBitmap source, PixelSurface mask, SKRectI? limit = null)
    {
        SKRectI bounds = MaskOps.GetBounds(mask);
        if (limit is SKRectI l) bounds.Intersect(l);
        bounds.Inflate(2, 2);
        bounds.Intersect(new SKRectI(0, 0, target.Width, target.Height));
        if (bounds.IsEmpty) return;

        int w = bounds.Width, h = bounds.Height;
        float[][] diff = [new float[w * h], new float[w * h], new float[w * h], new float[w * h]];
        bool[] inside = new bool[w * h];
        uint* tp = (uint*)target.GetPixels();
        uint* sp = (uint*)source.GetPixels();
        int stride = target.RowBytes / 4;
        byte* m = (byte*)mask.Bitmap.GetPixels();

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int bx = x + bounds.Left, by = y + bounds.Top;
                int i = y * w + x;
                inside[i] = m[by * mask.RowBytes + bx] > 0 && x > 0 && y > 0 && x < w - 1 && y < h - 1;
                uint t = tp[by * stride + bx], s = sp[by * stride + bx];
                diff[0][i] = (t >> 24) - (float)(s >> 24);
                diff[1][i] = ((t >> 16) & 0xFF) - (float)((s >> 16) & 0xFF);
                diff[2][i] = ((t >> 8) & 0xFF) - (float)((s >> 8) & 0xFF);
                diff[3][i] = (t & 0xFF) - (float)(s & 0xFF);
            }
        }

        // Solve the Laplace equation for the difference with Dirichlet boundary values (membrane interpolation).
        for (int c = 0; c < 4; c++)
        {
            float[] d = diff[c];
            for (int i = 0; i < d.Length; i++) if (inside[i]) d[i] = 0;
            SolveLaplace(d, inside, w, h);
        }

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int i = y * w + x;
                int bx = x + bounds.Left, by = y + bounds.Top;
                byte coverage = m[by * mask.RowBytes + bx];
                if (coverage == 0) continue;
                uint s = sp[by * stride + bx];
                float a = (s >> 24) + diff[0][i];
                float r = ((s >> 16) & 0xFF) + diff[1][i];
                float g = ((s >> 8) & 0xFF) + diff[2][i];
                float b = (s & 0xFF) + diff[3][i];
                int A = Math.Clamp((int)(a + 0.5f), 0, 255);
                uint healed = ((uint)A << 24) | ((uint)Math.Clamp((int)(r + 0.5f), 0, A) << 16) | ((uint)Math.Clamp((int)(g + 0.5f), 0, A) << 8) | (uint)Math.Clamp((int)(b + 0.5f), 0, A);
                tp[by * stride + bx] = coverage >= 255 ? healed : ColorMath.LerpPremul(tp[by * stride + bx], healed, coverage / 255f);
            }
        }
    }

    /// <summary>Hierarchical successive over-relaxation solver for Laplace's equation inside a mask.</summary>
    public static void SolveLaplace(float[] values, bool[] inside, int w, int h)
    {
        if (w > 48 && h > 48)
        {
            // Solve a half resolution problem first to get a good initial guess.
            int cw = (w + 1) / 2, ch = (h + 1) / 2;
            float[] coarse = new float[cw * ch];
            bool[] coarseInside = new bool[cw * ch];
            for (int y = 0; y < ch; y++)
            {
                for (int x = 0; x < cw; x++)
                {
                    float sum = 0;
                    int count = 0;
                    bool allInside = true;
                    for (int dy = 0; dy < 2; dy++)
                    {
                        for (int dx = 0; dx < 2; dx++)
                        {
                            int fx = Math.Min(x * 2 + dx, w - 1), fy = Math.Min(y * 2 + dy, h - 1);
                            int fi = fy * w + fx;
                            if (inside[fi]) continue;
                            allInside = false;
                            sum += values[fi];
                            count++;
                        }
                    }
                    coarseInside[y * cw + x] = allInside && x > 0 && y > 0 && x < cw - 1 && y < ch - 1;
                    coarse[y * cw + x] = count > 0 ? sum / count : 0;
                }
            }
            SolveLaplace(coarse, coarseInside, cw, ch);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int i = y * w + x;
                    if (inside[i]) values[i] = coarse[Math.Min(y / 2, ch - 1) * cw + Math.Min(x / 2, cw - 1)];
                }
            }
        }

        List<int> cells = [];
        for (int i = 0; i < inside.Length; i++) if (inside[i]) cells.Add(i);
        const float omega = 1.85f;
        int iterations = Math.Clamp(Math.Max(w, h), 30, 300);
        for (int it = 0; it < iterations; it++)
        {
            float maxChange = 0;
            foreach (int i in cells)
            {
                float avg = (values[i - 1] + values[i + 1] + values[i - w] + values[i + w]) * 0.25f;
                float change = omega * (avg - values[i]);
                values[i] += change;
                if (MathF.Abs(change) > maxChange) maxChange = MathF.Abs(change);
            }
            if (maxChange < 0.05f) break;
        }
    }
}

/// <summary>Intelligent scissors (live-wire) for the magnetic lasso.</summary>
public sealed class LiveWire
{
    private readonly float[] _cost;
    private readonly float[] _gradient;

    public LiveWire(SKBitmap image)
    {
        Width = image.Width;
        Height = image.Height;
        _gradient = ComputeGradient(image, Width, Height);
        _cost = new float[Width * Height];
        float max = Math.Max(1e-3f, _gradient.Max());
        for (int i = 0; i < _cost.Length; i++) _cost[i] = 1 - _gradient[i] / max + 0.02f;
    }

    public int Width { get; }

    public int Height { get; }

    public float GradientAt(int x, int y) => x < 0 || y < 0 || x >= Width || y >= Height ? 0 : _gradient[y * Width + x];

    private static unsafe float[] ComputeGradient(SKBitmap image, int width, int height)
    {
        using SKBitmap smooth = Filters.GaussianBlur(image, 1.5f);
        float[] lum = new float[width * height];
        uint* p = (uint*)smooth.GetPixels();
        int stride = smooth.RowBytes / 4;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                ColorMath.Unpack(p[y * stride + x], out float r, out float g, out float b, out float a);
                lum[y * width + x] = ColorMath.Luminance(r, g, b) * a;
            }
        }
        float[] gradient = new float[width * height];
        float L(int x, int y) => lum[Math.Clamp(y, 0, height - 1) * width + Math.Clamp(x, 0, width - 1)];
        Parallel.For(0, height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                float gx = L(x + 1, y - 1) + 2 * L(x + 1, y) + L(x + 1, y + 1) - L(x - 1, y - 1) - 2 * L(x - 1, y) - L(x - 1, y + 1);
                float gy = L(x - 1, y + 1) + 2 * L(x, y + 1) + L(x + 1, y + 1) - L(x - 1, y - 1) - 2 * L(x, y - 1) - L(x + 1, y - 1);
                gradient[y * width + x] = MathF.Sqrt(gx * gx + gy * gy);
            }
        });
        return gradient;
    }

    /// <summary>Moves a point to the strongest edge within the given radius.</summary>
    public SKPointI Snap(SKPointI point, int radius)
    {
        SKPointI best = point;
        float bestValue = GradientAt(point.X, point.Y);
        for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                if (dx * dx + dy * dy > radius * radius) continue;
                float v = GradientAt(point.X + dx, point.Y + dy) - (dx * dx + dy * dy) * 0.0005f;
                if (v > bestValue)
                {
                    bestValue = v;
                    best = new SKPointI(point.X + dx, point.Y + dy);
                }
            }
        }
        return best;
    }

    /// <summary>Finds the minimum cost path between two points with Dijkstra's algorithm inside a search window.</summary>
    public List<SKPointI> FindPath(SKPointI start, SKPointI end, int margin = 24)
    {
        start = Clamp(start);
        end = Clamp(end);
        SKRectI window = new(Math.Min(start.X, end.X) - margin, Math.Min(start.Y, end.Y) - margin,
            Math.Max(start.X, end.X) + margin + 1, Math.Max(start.Y, end.Y) + margin + 1);
        window.Intersect(new SKRectI(0, 0, Width, Height));
        int w = window.Width, h = window.Height;
        float[] dist = new float[w * h];
        int[] previous = new int[w * h];
        Array.Fill(dist, float.MaxValue);
        Array.Fill(previous, -1);
        int Index(SKPointI p) => (p.Y - window.Top) * w + (p.X - window.Left);
        int startIndex = Index(start), endIndex = Index(end);
        PriorityQueue<int, float> queue = new();
        dist[startIndex] = 0;
        queue.Enqueue(startIndex, 0);
        while (queue.TryDequeue(out int current, out float d))
        {
            if (d > dist[current]) continue;
            if (current == endIndex) break;
            int cx = current % w, cy = current / w;
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = cx + dx, ny = cy + dy;
                    if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                    int ni = ny * w + nx;
                    float step = (dx != 0 && dy != 0 ? 1.4142f : 1f) * _cost[(ny + window.Top) * Width + nx + window.Left];
                    float nd = d + step;
                    if (nd < dist[ni])
                    {
                        dist[ni] = nd;
                        previous[ni] = current;
                        queue.Enqueue(ni, nd);
                    }
                }
            }
        }
        List<SKPointI> path = [];
        for (int i = endIndex; i >= 0; i = previous[i])
        {
            path.Add(new SKPointI(i % w + window.Left, i / w + window.Top));
            if (i == startIndex) break;
        }
        path.Reverse();
        return path;
    }

    private SKPointI Clamp(SKPointI p) => new(Math.Clamp(p.X, 0, Width - 1), Math.Clamp(p.Y, 0, Height - 1));
}

/// <summary>Automatic selection algorithms (object selection, select subject, quick selection) and red-eye removal.</summary>
public static class SmartSelection
{
    private struct Rgb
    {
        public float R, G, B;
        public Rgb(float r, float g, float b) { R = r; G = g; B = b; }
        public readonly float Dist(Rgb o) => (R - o.R) * (R - o.R) + (G - o.G) * (G - o.G) + (B - o.B) * (B - o.B);
    }

    private static unsafe Rgb[] ReadColors(SKBitmap image, out int w, out int h)
    {
        w = image.Width;
        h = image.Height;
        Rgb[] colors = new Rgb[w * h];
        uint* p = (uint*)image.GetPixels();
        int stride = image.RowBytes / 4;
        int width = w;
        Parallel.For(0, h, y =>
        {
            for (int x = 0; x < width; x++)
            {
                // Composite transparent pixels over white so they look like background.
                ColorMath.Unpack(p[y * stride + x], out float r, out float g, out float b, out float a);
                colors[y * width + x] = new Rgb((r * a + (1 - a)) * 255, (g * a + (1 - a)) * 255, (b * a + (1 - a)) * 255);
            }
        });
        return colors;
    }

    private static Rgb[] KMeans(List<Rgb> samples, int k, int seed)
    {
        if (samples.Count == 0) return [new Rgb(0, 0, 0)];
        Random random = new(seed);
        k = Math.Min(k, samples.Count);
        Rgb[] centers = new Rgb[k];
        for (int i = 0; i < k; i++) centers[i] = samples[random.Next(samples.Count)];
        int[] assign = new int[samples.Count];
        for (int iteration = 0; iteration < 8; iteration++)
        {
            for (int i = 0; i < samples.Count; i++)
            {
                float best = float.MaxValue;
                for (int c = 0; c < k; c++)
                {
                    float d = samples[i].Dist(centers[c]);
                    if (d < best) { best = d; assign[i] = c; }
                }
            }
            float[] sr = new float[k], sg = new float[k], sb = new float[k];
            int[] counts = new int[k];
            for (int i = 0; i < samples.Count; i++)
            {
                int c = assign[i];
                sr[c] += samples[i].R; sg[c] += samples[i].G; sb[c] += samples[i].B; counts[c]++;
            }
            for (int c = 0; c < k; c++)
            {
                if (counts[c] > 0) centers[c] = new Rgb(sr[c] / counts[c], sg[c] / counts[c], sb[c] / counts[c]);
            }
        }
        return centers;
    }

    private static float ModelDistance(Rgb color, Rgb[] model)
    {
        float best = float.MaxValue;
        foreach (Rgb c in model)
        {
            float d = color.Dist(c);
            if (d < best) best = d;
        }
        return best;
    }

    /// <summary>
    /// Selects the dominant object inside a rectangle by iteratively refining foreground/background color models,
    /// smoothing labels spatially and keeping the main connected component (a light-weight GrabCut).
    /// </summary>
    public static PixelSurface SelectObject(SKBitmap image, SKRectI rect)
    {
        Rgb[] colors = ReadColors(image, out int w, out int h);
        rect.Intersect(new SKRectI(0, 0, w, h));
        PixelSurface result = PixelSurface.CreateMask(w, h);
        if (rect.Width < 3 || rect.Height < 3) return result;

        int rw = rect.Width, rh = rect.Height;
        bool[] fg = new bool[rw * rh];
        int band = Math.Max(2, Math.Min(rw, rh) / 20);
        int innerX = rw / 6, innerY = rh / 6;
        Random random = new(5);

        List<Rgb> bgSamples = [];
        SKRectI outer = rect;
        outer.Inflate(band * 2, band * 2);
        outer.Intersect(new SKRectI(0, 0, w, h));
        for (int y = outer.Top; y < outer.Bottom; y++)
        {
            for (int x = outer.Left; x < outer.Right; x++)
            {
                bool inBorder = x < rect.Left + band || x >= rect.Right - band || y < rect.Top + band || y >= rect.Bottom - band;
                if (inBorder && random.Next(3) == 0) bgSamples.Add(colors[y * w + x]);
            }
        }
        List<Rgb> fgSamples = [];
        for (int y = innerY; y < rh - innerY; y++)
            for (int x = innerX; x < rw - innerX; x++)
                if (random.Next(3) == 0) fgSamples.Add(colors[(y + rect.Top) * w + x + rect.Left]);

        Rgb[] bgModel = KMeans(bgSamples, 6, 1);
        Rgb[] fgModel = KMeans(fgSamples.Count > 0 ? fgSamples : bgSamples, 6, 2);

        for (int iteration = 0; iteration < 5; iteration++)
        {
            Parallel.For(0, rh, y =>
            {
                for (int x = 0; x < rw; x++)
                {
                    bool border = x < band || y < band || x >= rw - band || y >= rh - band;
                    Rgb c = colors[(y + rect.Top) * w + x + rect.Left];
                    // Pixels close to the center get a small foreground prior.
                    float cx = (x - rw / 2f) / (rw / 2f), cy = (y - rh / 2f) / (rh / 2f);
                    float prior = 1 + (cx * cx + cy * cy) * 0.35f;
                    fg[y * rw + x] = !border && ModelDistance(c, fgModel) * prior < ModelDistance(c, bgModel);
                }
            });
            fg = MajorityFilter(fg, rw, rh, 2);
            if (iteration < 4)
            {
                List<Rgb> newFg = [], newBg = [.. bgSamples];
                for (int i = 0; i < fg.Length; i += 2)
                {
                    Rgb c = colors[(i / rw + rect.Top) * w + i % rw + rect.Left];
                    if (fg[i]) newFg.Add(c);
                    else if (random.Next(4) == 0) newBg.Add(c);
                }
                if (newFg.Count > 10) fgModel = KMeans(newFg, 6, iteration + 3);
                bgModel = KMeans(newBg, 6, iteration + 13);
            }
        }

        fg = KeepLargestComponent(fg, rw, rh);
        fg = FillHoles(fg, rw, rh);

        unsafe
        {
            byte* m = (byte*)result.Bitmap.GetPixels();
            for (int y = 0; y < rh; y++)
                for (int x = 0; x < rw; x++)
                    if (fg[y * rw + x]) m[(y + rect.Top) * result.RowBytes + x + rect.Left] = 255;
        }
        MaskOps.Blur(result, 1.5f);
        return result;
    }

    /// <summary>Selects the most prominent subject of the image.</summary>
    public static PixelSurface SelectSubject(SKBitmap image)
    {
        int inset = Math.Max(2, Math.Min(image.Width, image.Height) / 40);
        return SelectObject(image, new SKRectI(inset, inset, image.Width - inset, image.Height - inset));
    }

    private static bool[] MajorityFilter(bool[] labels, int w, int h, int radius)
    {
        int[] integral = new int[(w + 1) * (h + 1)];
        for (int y = 0; y < h; y++)
        {
            int row = 0;
            for (int x = 0; x < w; x++)
            {
                row += labels[y * w + x] ? 1 : 0;
                integral[(y + 1) * (w + 1) + x + 1] = integral[y * (w + 1) + x + 1] + row;
            }
        }
        bool[] result = new bool[labels.Length];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int x0 = Math.Max(0, x - radius), y0 = Math.Max(0, y - radius), x1 = Math.Min(w, x + radius + 1), y1 = Math.Min(h, y + radius + 1);
                int sum = integral[y1 * (w + 1) + x1] - integral[y0 * (w + 1) + x1] - integral[y1 * (w + 1) + x0] + integral[y0 * (w + 1) + x0];
                int area = (x1 - x0) * (y1 - y0);
                result[y * w + x] = sum * 2 > area;
            }
        }
        return result;
    }

    private static int[] Label(bool[] mask, int w, int h, bool value, out int count, out int[] sizes)
    {
        int[] labels = new int[mask.Length];
        Array.Fill(labels, -1);
        List<int> sizeList = [];
        Stack<int> stack = new();
        for (int start = 0; start < mask.Length; start++)
        {
            if (mask[start] != value || labels[start] >= 0) continue;
            int label = sizeList.Count;
            int size = 0;
            stack.Push(start);
            labels[start] = label;
            while (stack.Count > 0)
            {
                int i = stack.Pop();
                size++;
                int x = i % w, y = i / w;
                void Visit(int nx, int ny)
                {
                    if (nx < 0 || ny < 0 || nx >= w || ny >= h) return;
                    int ni = ny * w + nx;
                    if (mask[ni] != value || labels[ni] >= 0) return;
                    labels[ni] = label;
                    stack.Push(ni);
                }
                Visit(x - 1, y); Visit(x + 1, y); Visit(x, y - 1); Visit(x, y + 1);
            }
            sizeList.Add(size);
        }
        count = sizeList.Count;
        sizes = sizeList.ToArray();
        return labels;
    }

    private static bool[] KeepLargestComponent(bool[] fg, int w, int h)
    {
        int[] labels = Label(fg, w, h, true, out int count, out int[] sizes);
        if (count <= 1) return fg;
        int largest = Array.IndexOf(sizes, sizes.Max());
        bool[] result = new bool[fg.Length];
        // Keep components that are at least 15% of the largest one to allow multi-part objects.
        for (int i = 0; i < fg.Length; i++) result[i] = labels[i] >= 0 && (labels[i] == largest || sizes[labels[i]] > sizes[largest] * 0.15f);
        return result;
    }

    private static bool[] FillHoles(bool[] fg, int w, int h)
    {
        int[] labels = Label(fg, w, h, false, out int count, out _);
        bool[] touchesBorder = new bool[count];
        for (int x = 0; x < w; x++)
        {
            if (labels[x] >= 0) touchesBorder[labels[x]] = true;
            if (labels[(h - 1) * w + x] >= 0) touchesBorder[labels[(h - 1) * w + x]] = true;
        }
        for (int y = 0; y < h; y++)
        {
            if (labels[y * w] >= 0) touchesBorder[labels[y * w]] = true;
            if (labels[y * w + w - 1] >= 0) touchesBorder[labels[y * w + w - 1]] = true;
        }
        bool[] result = (bool[])fg.Clone();
        for (int i = 0; i < fg.Length; i++)
        {
            if (!fg[i] && labels[i] >= 0 && !touchesBorder[labels[i]]) result[i] = true;
        }
        return result;
    }

    /// <summary>
    /// Quick selection growth step: grows from brush seed pixels into similar colors, stopping at strong edges
    /// and within a distance limit of the stroke.
    /// </summary>
    public static PixelSurface QuickSelect(SKBitmap image, IReadOnlyList<SKPoint> strokePoints, float brushRadius, LiveWire edges)
    {
        Rgb[] colors = ReadColors(image, out int w, out int h);
        PixelSurface result = PixelSurface.CreateMask(w, h);
        if (strokePoints.Count == 0) return result;

        float maxDistance = brushRadius * 4 + 10;
        SKRect area = SKRect.Create(strokePoints[0].X, strokePoints[0].Y, 0, 0);
        foreach (SKPoint p in strokePoints) area = SKRect.Union(area, SKRect.Create(p.X, p.Y, 0, 0));
        area.Inflate(maxDistance, maxDistance);
        SKRectI window = SKRectI.Round(area);
        window.Intersect(new SKRectI(0, 0, w, h));

        List<Rgb> seeds = [];
        Queue<int> queue = new();
        bool[] visited = new bool[w * h];
        int r = (int)MathF.Ceiling(brushRadius);
        foreach (SKPoint p in strokePoints)
        {
            for (int dy = -r; dy <= r; dy++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    if (dx * dx + dy * dy > r * r) continue;
                    int x = (int)p.X + dx, y = (int)p.Y + dy;
                    if (x < 0 || y < 0 || x >= w || y >= h) continue;
                    int i = y * w + x;
                    if (visited[i]) continue;
                    visited[i] = true;
                    seeds.Add(colors[i]);
                    queue.Enqueue(i);
                }
            }
        }
        Rgb[] model = KMeans(seeds, Math.Min(4, seeds.Count), 3);
        float spread = 0;
        foreach (Rgb s in seeds) spread += ModelDistance(s, model);
        spread /= Math.Max(1, seeds.Count);
        float threshold = Math.Max(30 * 30, spread * 6);
        float maxGradient = 0;
        for (int y = window.Top; y < window.Bottom; y++)
            for (int x = window.Left; x < window.Right; x++)
                maxGradient = Math.Max(maxGradient, edges.GradientAt(x, y));
        float edgeLimit = maxGradient * 0.35f;

        unsafe
        {
            byte* m = (byte*)result.Bitmap.GetPixels();
            while (queue.Count > 0)
            {
                int i = queue.Dequeue();
                int x = i % w, y = i / w;
                m[y * result.RowBytes + x] = 255;
                void Visit(int nx, int ny)
                {
                    if (nx < window.Left || ny < window.Top || nx >= window.Right || ny >= window.Bottom) return;
                    int ni = ny * w + nx;
                    if (visited[ni]) return;
                    visited[ni] = true;
                    if (edges.GradientAt(nx, ny) > edgeLimit) return;
                    if (ModelDistance(colors[ni], model) > threshold) return;
                    queue.Enqueue(ni);
                }
                Visit(x - 1, y); Visit(x + 1, y); Visit(x, y - 1); Visit(x, y + 1);
            }
        }
        PixelSurface smooth = MaskOps.Smooth(result, 1);
        result.Dispose();
        return smooth;
    }

    /// <summary>Removes red-eye inside an area by desaturating and darkening strongly red pixels.</summary>
    public static unsafe void RemoveRedEye(SKBitmap image, SKPointI offset, SKRectI area, float pupilSize, float darken)
    {
        area.Offset(-offset.X, -offset.Y);
        area.Intersect(new SKRectI(0, 0, image.Width, image.Height));
        if (area.IsEmpty) return;
        uint* p = (uint*)image.GetPixels();
        int stride = image.RowBytes / 4;
        float cx = area.MidX, cy = area.MidY;
        float radius = Math.Max(1, Math.Min(area.Width, area.Height) / 2f) * Math.Clamp(pupilSize / 50f, 0.2f, 2f);
        for (int y = area.Top; y < area.Bottom; y++)
        {
            for (int x = area.Left; x < area.Right; x++)
            {
                uint px = p[y * stride + x];
                ColorMath.Unpack(px, out float r, out float g, out float b, out float a);
                float redness = r - (g + b) / 2;
                if (r < 0.25f || redness < 0.12f) continue;
                float dx = (x - cx) / radius, dy = (y - cy) / radius;
                float falloff = Math.Clamp(1.4f - MathF.Sqrt(dx * dx + dy * dy), 0, 1);
                float strength = Math.Clamp(redness * 3, 0, 1) * falloff;
                if (strength <= 0) continue;
                float target = (g + b) / 2 * (1 - darken / 100f);
                r += (target - r) * strength;
                g += (g * (1 - darken / 100f) - g) * strength;
                b += (b * (1 - darken / 100f) - b) * strength;
                p[y * stride + x] = ColorMath.PackUnpremul(r, g, b, a);
            }
        }
    }
}
