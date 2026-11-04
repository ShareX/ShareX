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

/// <summary>Color-similarity region selection used by the magic wand, paint bucket and magic eraser.</summary>
public static class FloodFill
{
    /// <summary>
    /// Builds a mask of pixels similar to the seed color. Coverage falls off near the tolerance edge when anti-aliasing.
    /// </summary>
    public static unsafe PixelSurface Select(SKBitmap source, int seedX, int seedY, int tolerance, bool contiguous, bool antialias)
    {
        int width = source.Width, height = source.Height;
        PixelSurface mask = PixelSurface.CreateMask(width, height);
        if (seedX < 0 || seedY < 0 || seedX >= width || seedY >= height) return mask;
        uint* pixels = (uint*)source.GetPixels();
        int stride = source.RowBytes / 4;
        uint seed = pixels[seedY * stride + seedX];
        return SelectColor(source, seed, tolerance, contiguous ? (seedX, seedY) : null, antialias);
    }

    public static unsafe PixelSurface SelectColor(SKBitmap source, uint color, int tolerance, (int X, int Y)? seed, bool antialias)
    {
        int width = source.Width, height = source.Height;
        PixelSurface mask = PixelSurface.CreateMask(width, height);
        uint* pixels = (uint*)source.GetPixels();
        int stride = source.RowBytes / 4;
        byte* m = (byte*)mask.Bitmap.GetPixels();
        int maskStride = mask.RowBytes;
        uint seedColor = Unpremul(color);

        bool Match(int x, int y) => ColorMath.Distance(Unpremul(pixels[y * stride + x]), seedColor) <= tolerance;

        if (seed is (int sx, int sy))
        {
            // Scanline flood fill.
            bool[] visited = new bool[width * height];
            Stack<(int, int)> stack = new();
            stack.Push((sx, sy));
            while (stack.Count > 0)
            {
                var (x, y) = stack.Pop();
                if (visited[y * width + x] || !Match(x, y)) continue;
                int left = x, right = x;
                while (left > 0 && !visited[y * width + left - 1] && Match(left - 1, y)) left--;
                while (right < width - 1 && !visited[y * width + right + 1] && Match(right + 1, y)) right++;
                for (int i = left; i <= right; i++)
                {
                    visited[y * width + i] = true;
                    m[y * maskStride + i] = 255;
                    if (y > 0 && !visited[(y - 1) * width + i] && Match(i, y - 1)) stack.Push((i, y - 1));
                    if (y < height - 1 && !visited[(y + 1) * width + i] && Match(i, y + 1)) stack.Push((i, y + 1));
                }
            }
        }
        else
        {
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    if (Match(x, y)) m[y * maskStride + x] = 255;
                }
            });
        }

        if (antialias) AntialiasEdges(mask);
        return mask;
    }

    /// <summary>
    /// Softens stair-stepped edges of a binary mask with a small blur limited to the boundary.
    /// Straight horizontal and vertical edges are left crisp so pixel-aligned regions keep exact edges.
    /// </summary>
    public static unsafe void AntialiasEdges(PixelSurface mask)
    {
        int width = mask.Width, height = mask.Height, stride = mask.RowBytes;
        byte* m = (byte*)mask.Bitmap.GetPixels();
        byte[] copy = new byte[stride * height];
        fixed (byte* c = copy)
        {
            Buffer.MemoryCopy(m, c, copy.Length, copy.Length);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sum = 0, count = 0;
                    int different = 0, bit = 0;
                    byte center = c[y * stride + x];
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int ny = y + dy;
                        for (int dx = -1; dx <= 1; dx++, bit++)
                        {
                            int nx = x + dx;
                            if (ny < 0 || ny >= height || nx < 0 || nx >= width) continue;
                            byte v = c[ny * stride + nx];
                            if (v != center) different |= 1 << bit;
                            sum += v;
                            count++;
                        }
                    }
                    // Neighbor bits: 0 1 2 / 3 4 5 / 6 7 8 (row-major, 4 is the pixel itself).
                    // Straight edges, right-angle corners and lone diagonal contacts stay exactly as they are.
                    bool straight = (different & ~0b000000111) == 0 || (different & ~0b111000000) == 0 || (different & ~0b001001001) == 0 || (different & ~0b100100100) == 0
                        || different is 0b001001111 or 0b100100111 or 0b111001001 or 0b111100100;
                    if (different != 0 && !straight) m[y * stride + x] = (byte)((sum + center * count) / (count * 2));
                }
            }
        }
    }

    private static uint Unpremul(uint p)
    {
        uint a = p >> 24;
        if (a == 0) return 0;
        if (a == 255) return p;
        uint r = Math.Min(255, ((p >> 16) & 0xFF) * 255 / a);
        uint g = Math.Min(255, ((p >> 8) & 0xFF) * 255 / a);
        uint b = Math.Min(255, (p & 0xFF) * 255 / a);
        return (a << 24) | (r << 16) | (g << 8) | b;
    }

    /// <summary>Selects pixels whose color is within a fuzziness range of any of the sample colors (Select &gt; Color Range).</summary>
    public static unsafe PixelSurface ColorRange(SKBitmap source, IReadOnlyList<SKColor> samples, float fuzziness)
    {
        int width = source.Width, height = source.Height;
        PixelSurface mask = PixelSurface.CreateMask(width, height);
        uint* pixels = (uint*)source.GetPixels();
        int stride = source.RowBytes / 4;
        byte* m = (byte*)mask.Bitmap.GetPixels();
        int maskStride = mask.RowBytes;
        float range = Math.Max(1, fuzziness);
        Parallel.For(0, height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                ColorMath.Unpack(pixels[y * stride + x], out float r, out float g, out float b, out _);
                m[y * maskStride + x] = (byte)(ColorCoverage(r, g, b, samples, range) * 255);
            }
        });
        return mask;
    }

    /// <summary>How closely a color matches the nearest sample: 1 for a match, falling to 0 at the given distance in 0..255 steps.</summary>
    public static float ColorCoverage(float r, float g, float b, IReadOnlyList<SKColor> samples, float range)
    {
        float best = float.MaxValue;
        foreach (SKColor s in samples)
        {
            float dr = r * 255 - s.Red, dg = g * 255 - s.Green, db = b * 255 - s.Blue;
            float d = MathF.Sqrt((dr * dr + dg * dg + db * db) / 3);
            if (d < best) best = d;
        }
        return Math.Clamp(1 - best / range, 0, 1);
    }
}
