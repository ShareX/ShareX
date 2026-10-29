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

/// <summary>
/// Content-aware fill using multi-scale PatchMatch with expectation-maximization voting
/// (in the spirit of Wexler et al. and Barnes et al.). Works on premultiplied BGRA pixels.
/// </summary>
public static class Inpainting
{
    private sealed class Level
    {
        public int W, H;
        public float[] Img = [];   // 4 floats per pixel: a, r, g, b (premultiplied, 0..255)
        public bool[] Hole = [];
        public bool[] Valid = [];  // patch centered here lies fully in known pixels
        public bool[] SourceAllowed = [];
    }

    /// <summary>Fills masked pixels (mask &gt;= 128) of the bitmap. The mask is in bitmap coordinates.</summary>
    public static unsafe void Fill(SKBitmap bitmap, PixelSurface mask, int patchRadius = 3, Func<int, int, bool>? sourceFilter = null, IProgress<float>? progress = null)
    {
        SKRectI holeBounds = MaskOps.GetBounds(mask);
        holeBounds.Intersect(new SKRectI(0, 0, bitmap.Width, bitmap.Height));
        if (holeBounds.IsEmpty) return;

        int margin = Math.Max(48, Math.Max(holeBounds.Width, holeBounds.Height));
        SKRectI roi = holeBounds;
        roi.Inflate(margin, margin);
        roi.Intersect(new SKRectI(0, 0, bitmap.Width, bitmap.Height));

        Level level = new() { W = roi.Width, H = roi.Height };
        int n = level.W * level.H;
        level.Img = new float[n * 4];
        level.Hole = new bool[n];
        level.SourceAllowed = new bool[n];
        uint* pixels = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        byte* m = (byte*)mask.Bitmap.GetPixels();
        int holeCount = 0;
        for (int y = 0; y < level.H; y++)
        {
            for (int x = 0; x < level.W; x++)
            {
                int bx = x + roi.Left, by = y + roi.Top;
                int i = y * level.W + x;
                uint p = pixels[by * stride + bx];
                level.Img[i * 4] = p >> 24;
                level.Img[i * 4 + 1] = (p >> 16) & 0xFF;
                level.Img[i * 4 + 2] = (p >> 8) & 0xFF;
                level.Img[i * 4 + 3] = p & 0xFF;
                bool hole = m[by * mask.RowBytes + bx] >= 128;
                level.Hole[i] = hole;
                level.SourceAllowed[i] = !hole && (sourceFilter == null || sourceFilter(bx, by));
                if (hole) holeCount++;
            }
        }
        if (holeCount == 0) return;

        // Build the pyramid.
        List<Level> pyramid = [level];
        while (true)
        {
            Level last = pyramid[^1];
            if (last.W < 24 || last.H < 24 || pyramid.Count > 8) break;
            int holeSize = CountHoleExtent(last);
            if (holeSize <= patchRadius * 2 + 1) break;
            pyramid.Add(Downsample(last));
        }

        int levels = pyramid.Count;
        int[]? nnX = null, nnY = null;
        for (int li = levels - 1; li >= 0; li--)
        {
            Level current = pyramid[li];
            ComputeValid(current, patchRadius);
            if (li == levels - 1)
            {
                DiffuseFill(current);
            }
            else
            {
                Level coarse = pyramid[li + 1];
                UpsampleInto(coarse, current);
                if (nnX != null && nnY != null) UpsampleNnf(coarse, current, ref nnX, ref nnY);
            }

            int emIterations = li == 0 ? 3 : 5;
            for (int em = 0; em < emIterations; em++)
            {
                float[] distances = RunPatchMatch(current, patchRadius, ref nnX, ref nnY, li == 0 ? 3 : 4, li * 1000 + em);
                Vote(current, patchRadius, nnX!, nnY!, distances);
            }
            progress?.Report(1 - li / (float)levels);
        }

        // Write back.
        for (int y = 0; y < level.H; y++)
        {
            for (int x = 0; x < level.W; x++)
            {
                int i = y * level.W + x;
                if (!level.Hole[i]) continue;
                int bx = x + roi.Left, by = y + roi.Top;
                float coverage = m[by * mask.RowBytes + bx] / 255f;
                uint filled = PackPremul(level.Img, i);
                pixels[by * stride + bx] = coverage >= 1 ? filled : ColorMath.LerpPremul(pixels[by * stride + bx], filled, coverage);
            }
        }
    }

    private static uint PackPremul(float[] img, int i)
    {
        int a = Math.Clamp((int)(img[i * 4] + 0.5f), 0, 255);
        int r = Math.Clamp((int)(img[i * 4 + 1] + 0.5f), 0, a);
        int g = Math.Clamp((int)(img[i * 4 + 2] + 0.5f), 0, a);
        int b = Math.Clamp((int)(img[i * 4 + 3] + 0.5f), 0, a);
        return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
    }

    private static int CountHoleExtent(Level level)
    {
        int minX = int.MaxValue, minY = int.MaxValue, maxX = -1, maxY = -1;
        for (int y = 0; y < level.H; y++)
            for (int x = 0; x < level.W; x++)
                if (level.Hole[y * level.W + x])
                {
                    minX = Math.Min(minX, x); maxX = Math.Max(maxX, x);
                    minY = Math.Min(minY, y); maxY = Math.Max(maxY, y);
                }
        return maxX < 0 ? 0 : Math.Max(maxX - minX, maxY - minY) + 1;
    }

    private static Level Downsample(Level fine)
    {
        Level coarse = new() { W = (fine.W + 1) / 2, H = (fine.H + 1) / 2 };
        int n = coarse.W * coarse.H;
        coarse.Img = new float[n * 4];
        coarse.Hole = new bool[n];
        coarse.SourceAllowed = new bool[n];
        for (int y = 0; y < coarse.H; y++)
        {
            for (int x = 0; x < coarse.W; x++)
            {
                int ci = y * coarse.W + x;
                float a = 0, r = 0, g = 0, b = 0;
                int known = 0;
                bool anyHole = false, allAllowed = true;
                for (int dy = 0; dy < 2; dy++)
                {
                    for (int dx = 0; dx < 2; dx++)
                    {
                        int fx = Math.Min(x * 2 + dx, fine.W - 1), fy = Math.Min(y * 2 + dy, fine.H - 1);
                        int fi = fy * fine.W + fx;
                        if (fine.Hole[fi])
                        {
                            anyHole = true;
                            continue;
                        }
                        if (!fine.SourceAllowed[fi]) allAllowed = false;
                        a += fine.Img[fi * 4]; r += fine.Img[fi * 4 + 1]; g += fine.Img[fi * 4 + 2]; b += fine.Img[fi * 4 + 3];
                        known++;
                    }
                }
                coarse.Hole[ci] = anyHole;
                coarse.SourceAllowed[ci] = !anyHole && allAllowed;
                if (known > 0)
                {
                    coarse.Img[ci * 4] = a / known; coarse.Img[ci * 4 + 1] = r / known;
                    coarse.Img[ci * 4 + 2] = g / known; coarse.Img[ci * 4 + 3] = b / known;
                }
            }
        }
        return coarse;
    }

    private static void ComputeValid(Level level, int r)
    {
        int w = level.W, h = level.H;
        level.Valid = new bool[w * h];
        // A source patch is valid when every pixel in its window is an allowed known pixel.
        int[] integral = new int[(w + 1) * (h + 1)];
        for (int y = 0; y < h; y++)
        {
            int rowSum = 0;
            for (int x = 0; x < w; x++)
            {
                rowSum += level.SourceAllowed[y * w + x] ? 0 : 1;
                integral[(y + 1) * (w + 1) + x + 1] = integral[y * (w + 1) + x + 1] + rowSum;
            }
        }
        for (int y = r; y < h - r; y++)
        {
            for (int x = r; x < w - r; x++)
            {
                int x0 = x - r, y0 = y - r, x1 = x + r + 1, y1 = y + r + 1;
                int bad = integral[y1 * (w + 1) + x1] - integral[y0 * (w + 1) + x1] - integral[y1 * (w + 1) + x0] + integral[y0 * (w + 1) + x0];
                level.Valid[y * w + x] = bad == 0;
            }
        }
        if (!level.Valid.Any(v => v))
        {
            // Too little context for full patches: allow any known pixel as a center.
            for (int i = 0; i < level.Valid.Length; i++) level.Valid[i] = level.SourceAllowed[i];
        }
    }

    /// <summary>Onion-peel diffusion fill used to initialize the coarsest level.</summary>
    private static void DiffuseFill(Level level)
    {
        int w = level.W, h = level.H;
        bool[] known = level.Hole.Select(x => !x).ToArray();
        bool changed = true;
        while (changed)
        {
            changed = false;
            List<(int, float, float, float, float)> updates = [];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int i = y * w + x;
                    if (known[i]) continue;
                    float a = 0, r = 0, g = 0, b = 0;
                    int count = 0;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int nx = x + dx, ny = y + dy;
                            if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                            int ni = ny * w + nx;
                            if (!known[ni]) continue;
                            a += level.Img[ni * 4]; r += level.Img[ni * 4 + 1]; g += level.Img[ni * 4 + 2]; b += level.Img[ni * 4 + 3];
                            count++;
                        }
                    }
                    if (count > 0) updates.Add((i, a / count, r / count, g / count, b / count));
                }
            }
            foreach (var (i, a, r, g, b) in updates)
            {
                level.Img[i * 4] = a; level.Img[i * 4 + 1] = r; level.Img[i * 4 + 2] = g; level.Img[i * 4 + 3] = b;
                known[i] = true;
                changed = true;
            }
        }
    }

    private static void UpsampleInto(Level coarse, Level fine)
    {
        for (int y = 0; y < fine.H; y++)
        {
            for (int x = 0; x < fine.W; x++)
            {
                int fi = y * fine.W + x;
                if (!fine.Hole[fi]) continue;
                int ci = Math.Min(y / 2, coarse.H - 1) * coarse.W + Math.Min(x / 2, coarse.W - 1);
                for (int c = 0; c < 4; c++) fine.Img[fi * 4 + c] = coarse.Img[ci * 4 + c];
            }
        }
    }

    private static void UpsampleNnf(Level coarse, Level fine, ref int[] nnX, ref int[] nnY)
    {
        int[] fx = new int[fine.W * fine.H], fy = new int[fine.W * fine.H];
        for (int y = 0; y < fine.H; y++)
        {
            for (int x = 0; x < fine.W; x++)
            {
                int ci = Math.Min(y / 2, coarse.H - 1) * coarse.W + Math.Min(x / 2, coarse.W - 1);
                fx[y * fine.W + x] = Math.Clamp(nnX[ci] * 2 + (x & 1), 0, fine.W - 1);
                fy[y * fine.W + x] = Math.Clamp(nnY[ci] * 2 + (y & 1), 0, fine.H - 1);
            }
        }
        nnX = fx;
        nnY = fy;
    }

    private static bool NeedsPatch(Level level, int x, int y, int r)
    {
        for (int dy = -r; dy <= r; dy++)
        {
            int yy = y + dy;
            if (yy < 0 || yy >= level.H) continue;
            for (int dx = -r; dx <= r; dx++)
            {
                int xx = x + dx;
                if (xx < 0 || xx >= level.W) continue;
                if (level.Hole[yy * level.W + xx]) return true;
            }
        }
        return false;
    }

    private static float Distance(Level level, int px, int py, int sx, int sy, int r, float limit)
    {
        float sum = 0;
        int count = 0;
        float[] img = level.Img;
        int w = level.W, h = level.H;
        for (int dy = -r; dy <= r; dy++)
        {
            int ty = py + dy, uy = sy + dy;
            if (ty < 0 || ty >= h || uy < 0 || uy >= h) continue;
            for (int dx = -r; dx <= r; dx++)
            {
                int tx = px + dx, ux = sx + dx;
                if (tx < 0 || tx >= w || ux < 0 || ux >= w) continue;
                int ti = (ty * w + tx) * 4, ui = (uy * w + ux) * 4;
                float d0 = img[ti] - img[ui], d1 = img[ti + 1] - img[ui + 1], d2 = img[ti + 2] - img[ui + 2], d3 = img[ti + 3] - img[ui + 3];
                sum += d0 * d0 + d1 * d1 + d2 * d2 + d3 * d3;
                count++;
            }
            if (count > 0 && sum / count > limit) return float.MaxValue;
        }
        return count == 0 ? float.MaxValue : sum / count;
    }

    private static float[] RunPatchMatch(Level level, int r, ref int[]? nnX, ref int[]? nnY, int iterations, int seed)
    {
        int w = level.W, h = level.H;
        Random random = new(seed);
        List<int> validList = [];
        for (int i = 0; i < level.Valid.Length; i++) if (level.Valid[i]) validList.Add(i);
        if (validList.Count == 0) return new float[w * h];

        if (nnX == null || nnY == null || nnX.Length != w * h)
        {
            nnX = new int[w * h];
            nnY = new int[w * h];
            for (int i = 0; i < w * h; i++)
            {
                int s = validList[random.Next(validList.Count)];
                nnX[i] = s % w;
                nnY[i] = s / w;
            }
        }

        List<int> targets = [];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                if (NeedsPatch(level, x, y, r)) targets.Add(y * w + x);

        float[] dist = new float[w * h];
        foreach (int i in targets)
        {
            int x = i % w, y = i / w;
            if (!level.Valid[nnY[i] * w + nnX[i]])
            {
                int s = validList[random.Next(validList.Count)];
                nnX[i] = s % w;
                nnY[i] = s / w;
            }
            dist[i] = Distance(level, x, y, nnX[i], nnY[i], r, float.MaxValue);
        }

        int[] bx = nnX, by = nnY;
        bool[] isTarget = new bool[w * h];
        foreach (int t in targets) isTarget[t] = true;

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            bool reverse = iteration % 2 == 1;
            int step = reverse ? -1 : 1;
            for (int k = 0; k < targets.Count; k++)
            {
                int i = targets[reverse ? targets.Count - 1 - k : k];
                int x = i % w, y = i / w;

                void Try(int cx, int cy)
                {
                    if (cx < 0 || cy < 0 || cx >= w || cy >= h) return;
                    if (!level.Valid[cy * w + cx]) return;
                    if (cx == bx[i] && cy == by[i]) return;
                    float d = Distance(level, x, y, cx, cy, r, dist[i]);
                    if (d < dist[i])
                    {
                        dist[i] = d;
                        bx[i] = cx;
                        by[i] = cy;
                    }
                }

                // Propagation.
                int nx = x - step;
                if (nx >= 0 && nx < w && isTarget[y * w + nx]) Try(bx[y * w + nx] + step, by[y * w + nx]);
                int ny = y - step;
                if (ny >= 0 && ny < h && isTarget[ny * w + x]) Try(bx[ny * w + x], by[ny * w + x] + step);

                // Random search with exponentially decreasing radius.
                int radius = Math.Max(w, h);
                while (radius >= 1)
                {
                    Try(bx[i] + random.Next(-radius, radius + 1), by[i] + random.Next(-radius, radius + 1));
                    radius /= 2;
                }
            }
        }
        return dist;
    }

    private static void Vote(Level level, int r, int[] nnX, int[] nnY, float[] dist)
    {
        int w = level.W, h = level.H;
        // Similarity weights: patches that matched well contribute more.
        List<float> sample = [];
        for (int i = 0; i < dist.Length; i += 7) if (dist[i] > 0 && dist[i] < float.MaxValue) sample.Add(dist[i]);
        sample.Sort();
        float sigma = sample.Count > 0 ? Math.Max(1, sample[(int)(sample.Count * 0.75f)]) : 100;

        float[] acc = new float[w * h * 4];
        float[] weight = new float[w * h];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int i = y * w + x;
                if (!NeedsPatch(level, x, y, r)) continue;
                float wv = MathF.Exp(-dist[i] / (2 * sigma));
                if (float.IsNaN(wv) || wv <= 0) wv = 1e-4f;
                int sx = nnX[i], sy = nnY[i];
                for (int dy = -r; dy <= r; dy++)
                {
                    int ty = y + dy, uy = sy + dy;
                    if (ty < 0 || ty >= h || uy < 0 || uy >= h) continue;
                    for (int dx = -r; dx <= r; dx++)
                    {
                        int tx = x + dx, ux = sx + dx;
                        if (tx < 0 || tx >= w || ux < 0 || ux >= w) continue;
                        int ti = ty * w + tx;
                        if (!level.Hole[ti]) continue;
                        int ui = (uy * w + ux) * 4;
                        acc[ti * 4] += level.Img[ui] * wv;
                        acc[ti * 4 + 1] += level.Img[ui + 1] * wv;
                        acc[ti * 4 + 2] += level.Img[ui + 2] * wv;
                        acc[ti * 4 + 3] += level.Img[ui + 3] * wv;
                        weight[ti] += wv;
                    }
                }
            }
        }
        for (int i = 0; i < w * h; i++)
        {
            if (!level.Hole[i] || weight[i] <= 0) continue;
            for (int c = 0; c < 4; c++) level.Img[i * 4 + c] = acc[i * 4 + c] / weight[i];
        }
    }
}
