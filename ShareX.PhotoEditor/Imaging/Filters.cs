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

public sealed record FilterParameter(string Key, string Label, float Minimum, float Maximum, float Default, float Step = 1, string[]? Options = null);

public sealed class FilterContext
{
    public SKColor Foreground { get; init; } = SKColors.Black;
    public SKColor Background { get; init; } = SKColors.White;
    public int Seed { get; init; } = 1;
}

/// <summary>Describes a filter for the Filter menu and its generic parameter dialog.</summary>
public sealed class FilterDefinition
{
    public FilterDefinition(string category, string name, IReadOnlyList<FilterParameter> parameters, Func<SKBitmap, IReadOnlyDictionary<string, float>, FilterContext, SKBitmap> apply)
    {
        Category = category;
        Name = name;
        Parameters = parameters;
        Apply = apply;
    }

    public string Category { get; }
    public string Name { get; }
    public IReadOnlyList<FilterParameter> Parameters { get; }
    public Func<SKBitmap, IReadOnlyDictionary<string, float>, FilterContext, SKBitmap> Apply { get; }

    public Dictionary<string, float> Defaults => Parameters.ToDictionary(p => p.Key, p => p.Default);

    public string Key(IReadOnlyDictionary<string, float> values) => Name + ":" + string.Join(",", values.OrderBy(v => v.Key).Select(v => $"{v.Key}={v.Value}"));
}

/// <summary>Built-in pixel filters operating on premultiplied BGRA bitmaps. Every method returns a new bitmap.</summary>
public static class Filters
{
    public static IReadOnlyList<FilterDefinition> All { get; } = CreateDefinitions();

    private static float P(IReadOnlyDictionary<string, float> v, string key) => v.TryGetValue(key, out float value) ? value : 0;

    private static List<FilterDefinition> CreateDefinitions() =>
    [
        new("Blur", "Average", [], (b, v, c) => Average(b)),
        new("Blur", "Blur", [], (b, v, c) => Convolve(b, [1, 2, 1, 2, 4, 2, 1, 2, 1], 3, 16)),
        new("Blur", "Blur More", [], (b, v, c) => GaussianBlur(b, 2)),
        new("Blur", "Box Blur", [new("radius", "Radius", 1, 200, 5)], (b, v, c) => BoxBlur(b, (int)P(v, "radius"))),
        new("Blur", "Gaussian Blur", [new("radius", "Radius", 0.1f, 250, 4, 0.1f)], (b, v, c) => GaussianBlur(b, P(v, "radius"))),
        new("Blur", "Motion Blur", [new("angle", "Angle", -90, 90, 0), new("distance", "Distance", 1, 500, 20)], (b, v, c) => MotionBlur(b, P(v, "angle"), (int)P(v, "distance"))),
        new("Blur", "Radial Blur", [new("amount", "Amount", 1, 100, 10), new("method", "Method", 0, 1, 0, 1, ["Spin", "Zoom"])], (b, v, c) => RadialBlur(b, P(v, "amount"), P(v, "method") > 0)),
        new("Blur", "Surface Blur", [new("radius", "Radius", 1, 30, 5), new("threshold", "Threshold", 2, 255, 15)], (b, v, c) => SurfaceBlur(b, (int)P(v, "radius"), P(v, "threshold"))),

        new("Distort", "Pinch", [new("amount", "Amount", -100, 100, 50)], (b, v, c) => Pinch(b, P(v, "amount") / 100f)),
        new("Distort", "Polar Coordinates", [new("mode", "Mode", 0, 1, 0, 1, ["Rectangular to Polar", "Polar to Rectangular"])], (b, v, c) => Polar(b, P(v, "mode") < 1)),
        new("Distort", "Ripple", [new("amount", "Amount", -999, 999, 100), new("size", "Size", 0, 2, 1, 1, ["Small", "Medium", "Large"])], (b, v, c) => Wave(b, P(v, "amount") / 100f * 4, 8 + P(v, "size") * 12, 8 + P(v, "size") * 12)),
        new("Distort", "Spherize", [new("amount", "Amount", -100, 100, 100)], (b, v, c) => Spherize(b, P(v, "amount") / 100f)),
        new("Distort", "Twirl", [new("angle", "Angle", -999, 999, 50)], (b, v, c) => Twirl(b, P(v, "angle"))),
        new("Distort", "Wave", [new("amplitude", "Amplitude", 1, 200, 10), new("wavelength", "Wavelength", 2, 500, 60)], (b, v, c) => Wave(b, P(v, "amplitude"), P(v, "wavelength"), P(v, "wavelength") * 1.3f)),

        new("Noise", "Add Noise", [new("amount", "Amount", 0.1f, 400, 12.5f, 0.1f), new("gaussian", "Distribution", 0, 1, 1, 1, ["Uniform", "Gaussian"]), new("mono", "Monochromatic", 0, 1, 0, 1, ["Off", "On"])],
            (b, v, c) => AddNoise(b, P(v, "amount"), P(v, "gaussian") > 0, P(v, "mono") > 0, c.Seed)),
        new("Noise", "Despeckle", [], (b, v, c) => Despeckle(b)),
        new("Noise", "Dust & Scratches", [new("radius", "Radius", 1, 16, 1), new("threshold", "Threshold", 0, 255, 0)], (b, v, c) => DustAndScratches(b, (int)P(v, "radius"), P(v, "threshold"))),
        new("Noise", "Median", [new("radius", "Radius", 1, 30, 2)], (b, v, c) => Median(b, (int)P(v, "radius"))),
        new("Noise", "Reduce Noise", [new("strength", "Strength", 0, 10, 6), new("details", "Preserve Details", 0, 100, 60)], (b, v, c) => SurfaceBlur(b, (int)(1 + P(v, "strength") / 2), 40 - P(v, "details") * 0.35f + P(v, "strength") * 2)),

        new("Pixelate", "Crystallize", [new("size", "Cell Size", 3, 300, 10)], (b, v, c) => Crystallize(b, (int)P(v, "size"), c.Seed)),
        new("Pixelate", "Facet", [], (b, v, c) => Median(b, 2)),
        new("Pixelate", "Mosaic", [new("size", "Cell Size", 2, 200, 8)], (b, v, c) => Mosaic(b, (int)P(v, "size"))),
        new("Pixelate", "Pointillize", [new("size", "Cell Size", 3, 300, 8)], (b, v, c) => Pointillize(b, (int)P(v, "size"), c.Background, c.Seed)),

        new("Render", "Clouds", [], (b, v, c) => Clouds(b, c.Foreground, c.Background, c.Seed, false)),
        new("Render", "Difference Clouds", [], (b, v, c) => Clouds(b, c.Foreground, c.Background, c.Seed, true)),
        new("Render", "Fibers", [new("variance", "Variance", 1, 64, 16), new("strength", "Strength", 1, 64, 4)], (b, v, c) => Fibers(b, c.Foreground, c.Background, P(v, "variance"), P(v, "strength"), c.Seed)),
        new("Render", "Lighting Vignette", [new("amount", "Amount", 0, 100, 50), new("size", "Size", 10, 100, 60)], (b, v, c) => Vignette(b, P(v, "amount") / 100f, P(v, "size") / 100f)),

        new("Sharpen", "Sharpen", [], (b, v, c) => Convolve(b, [0, -1, 0, -1, 6, -1, 0, -1, 0], 3, 2)),
        new("Sharpen", "Sharpen Edges", [], (b, v, c) => UnsharpMask(b, 1, 0.6f, 12)),
        new("Sharpen", "Sharpen More", [], (b, v, c) => Convolve(b, [-1, -1, -1, -1, 12, -1, -1, -1, -1], 3, 4)),
        new("Sharpen", "Smart Sharpen", [new("amount", "Amount", 1, 500, 200), new("radius", "Radius", 0.1f, 64, 1, 0.1f), new("noise", "Reduce Noise", 0, 100, 10)],
            (b, v, c) => UnsharpMask(b, P(v, "radius"), P(v, "amount") / 100f, P(v, "noise") * 0.3f)),
        new("Sharpen", "Unsharp Mask", [new("amount", "Amount", 1, 500, 100), new("radius", "Radius", 0.1f, 250, 1, 0.1f), new("threshold", "Threshold", 0, 255, 0)],
            (b, v, c) => UnsharpMask(b, P(v, "radius"), P(v, "amount") / 100f, P(v, "threshold"))),

        new("Stylize", "Diffuse", [], (b, v, c) => Diffuse(b, 2, c.Seed)),
        new("Stylize", "Emboss", [new("angle", "Angle", -180, 180, 135), new("height", "Height", 1, 10, 3), new("amount", "Amount", 1, 500, 100)], (b, v, c) => Emboss(b, P(v, "angle"), P(v, "height"), P(v, "amount") / 100f)),
        new("Stylize", "Find Edges", [], (b, v, c) => FindEdges(b)),
        new("Stylize", "Oil Paint", [new("radius", "Radius", 1, 10, 4), new("levels", "Levels", 4, 64, 20)], (b, v, c) => OilPaint(b, (int)P(v, "radius"), (int)P(v, "levels"))),
        new("Stylize", "Solarize", [], (b, v, c) => MapPixels(b, (ref float r, ref float g, ref float bb) => { r = r > 0.5f ? 1 - r : r; g = g > 0.5f ? 1 - g : g; bb = bb > 0.5f ? 1 - bb : bb; })),
        new("Stylize", "Tiles", [new("count", "Number of Tiles", 1, 99, 10), new("offset", "Maximum Offset %", 1, 99, 10)], (b, v, c) => Tiles(b, (int)P(v, "count"), P(v, "offset") / 100f, c.Seed)),

        new("Other", "High Pass", [new("radius", "Radius", 0.1f, 250, 10, 0.1f)], (b, v, c) => HighPass(b, P(v, "radius"))),
        new("Other", "Maximum", [new("radius", "Radius", 1, 100, 1)], (b, v, c) => Morphology(b, (int)P(v, "radius"), true)),
        new("Other", "Minimum", [new("radius", "Radius", 1, 100, 1)], (b, v, c) => Morphology(b, (int)P(v, "radius"), false)),
        new("Other", "Offset", [new("x", "Horizontal", -2000, 2000, 50), new("y", "Vertical", -2000, 2000, 50)], (b, v, c) => Offset(b, (int)P(v, "x"), (int)P(v, "y")))
    ];

    public static SKBitmap Copy(SKBitmap source) => source.Copy();

    public static SKBitmap GaussianBlur(SKBitmap source, float radius)
    {
        SKBitmap result = ImageOps.CreateBitmap(source.Width, source.Height);
        result.Erase(SKColors.Transparent);
        using SKCanvas canvas = new(result);
        float sigma = Math.Max(0.01f, radius / 2f);
        using SKImageFilter blur = SKImageFilter.CreateBlur(sigma, sigma, SKShaderTileMode.Clamp);
        using SKPaint paint = new() { ImageFilter = blur };
        canvas.DrawBitmap(source, 0, 0, paint);
        return result;
    }

    public static unsafe SKBitmap BoxBlur(SKBitmap source, int radius)
    {
        SKBitmap result = source.Copy();
        int width = source.Width, height = source.Height;
        uint[] temp = new uint[width * height];
        uint* p = (uint*)result.GetPixels();
        int stride = result.RowBytes / 4;
        int window = radius * 2 + 1;
        Parallel.For(0, height, y =>
        {
            long a = 0, r = 0, g = 0, b = 0;
            for (int x = -radius; x <= radius; x++) Accumulate(p[y * stride + Math.Clamp(x, 0, width - 1)], ref a, ref r, ref g, ref b, 1);
            for (int x = 0; x < width; x++)
            {
                temp[y * width + x] = PackAverage(a, r, g, b, window);
                Accumulate(p[y * stride + Math.Min(x + radius + 1, width - 1)], ref a, ref r, ref g, ref b, 1);
                Accumulate(p[y * stride + Math.Max(x - radius, 0)], ref a, ref r, ref g, ref b, -1);
            }
        });
        Parallel.For(0, width, x =>
        {
            long a = 0, r = 0, g = 0, b = 0;
            for (int y = -radius; y <= radius; y++) Accumulate(temp[Math.Clamp(y, 0, height - 1) * width + x], ref a, ref r, ref g, ref b, 1);
            for (int y = 0; y < height; y++)
            {
                p[y * stride + x] = PackAverage(a, r, g, b, window);
                Accumulate(temp[Math.Min(y + radius + 1, height - 1) * width + x], ref a, ref r, ref g, ref b, 1);
                Accumulate(temp[Math.Max(y - radius, 0) * width + x], ref a, ref r, ref g, ref b, -1);
            }
        });
        return result;
    }

    private static void Accumulate(uint p, ref long a, ref long r, ref long g, ref long b, int sign)
    {
        a += sign * (p >> 24);
        r += sign * ((p >> 16) & 0xFF);
        g += sign * ((p >> 8) & 0xFF);
        b += sign * (p & 0xFF);
    }

    private static uint PackAverage(long a, long r, long g, long b, int count)
    {
        uint A = (uint)Math.Clamp(a / count, 0, 255);
        uint R = (uint)Math.Clamp(r / count, 0, A);
        uint G = (uint)Math.Clamp(g / count, 0, A);
        uint B = (uint)Math.Clamp(b / count, 0, A);
        return (A << 24) | (R << 16) | (G << 8) | B;
    }

    public static unsafe SKBitmap Average(SKBitmap source)
    {
        long a = 0, r = 0, g = 0, b = 0;
        uint* p = (uint*)source.GetPixels();
        int stride = source.RowBytes / 4;
        long count = 0;
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                Accumulate(p[y * stride + x], ref a, ref r, ref g, ref b, 1);
                count++;
            }
        }
        SKBitmap result = ImageOps.CreateBitmap(source.Width, source.Height);
        uint avg = PackAverage(a, r, g, b, (int)Math.Max(1, count));
        result.Erase(ColorMath.ToColor(avg));
        return result;
    }

    /// <summary>Generic convolution on premultiplied pixels.</summary>
    /// <param name="keepAlpha">Leaves alpha untouched and clamps colors independently; needed for kernels whose weights sum to zero.</param>
    public static unsafe SKBitmap Convolve(SKBitmap source, float[] kernel, int size, float divisor, float bias = 0, bool keepAlpha = false)
    {
        SKBitmap result = source.Copy();
        int width = source.Width, height = source.Height, half = size / 2;
        uint* src = (uint*)source.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        Parallel.For(0, height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                float a = 0, r = 0, g = 0, b = 0;
                for (int ky = 0; ky < size; ky++)
                {
                    int sy = Math.Clamp(y + ky - half, 0, height - 1);
                    for (int kx = 0; kx < size; kx++)
                    {
                        float k = kernel[ky * size + kx];
                        if (k == 0) continue;
                        uint p = src[sy * stride + Math.Clamp(x + kx - half, 0, width - 1)];
                        a += (p >> 24) * k; r += ((p >> 16) & 0xFF) * k; g += ((p >> 8) & 0xFF) * k; b += (p & 0xFF) * k;
                    }
                }
                uint A = keepAlpha ? src[y * stride + x] >> 24 : (uint)Math.Clamp(a / divisor + bias, 0, 255);
                uint max = keepAlpha ? 255 : A;
                dst[y * stride + x] = (A << 24) | ((uint)Math.Clamp(r / divisor + bias, 0, max) << 16) | ((uint)Math.Clamp(g / divisor + bias, 0, max) << 8) | (uint)Math.Clamp(b / divisor + bias, 0, max);
            }
        });
        return result;
    }

    public static SKBitmap MapPixels(SKBitmap source, Adjustments.PixelFunction function)
    {
        SKBitmap result = source.Copy();
        ApplyFunction(result, function);
        return result;
    }

    private static unsafe void ApplyFunction(SKBitmap bitmap, Adjustments.PixelFunction function)
    {
        uint* p = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        int width = bitmap.Width;
        Parallel.For(0, bitmap.Height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                uint px = p[y * stride + x];
                if ((px >> 24) == 0) continue;
                ColorMath.Unpack(px, out float r, out float g, out float b, out float a);
                function(ref r, ref g, ref b);
                p[y * stride + x] = ColorMath.PackUnpremul(r, g, b, a);
            }
        });
    }

    public static unsafe SKBitmap MotionBlur(SKBitmap source, float angle, int distance)
    {
        SKBitmap result = source.Copy();
        int width = source.Width, height = source.Height;
        uint* src = (uint*)source.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        float radians = angle * MathF.PI / 180;
        float dx = MathF.Cos(radians), dy = -MathF.Sin(radians);
        int samples = Math.Max(1, distance);
        Parallel.For(0, height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                long a = 0, r = 0, g = 0, b = 0;
                for (int i = 0; i < samples; i++)
                {
                    float t = i - (samples - 1) / 2f;
                    int sx = Math.Clamp((int)MathF.Round(x + dx * t), 0, width - 1);
                    int sy = Math.Clamp((int)MathF.Round(y + dy * t), 0, height - 1);
                    Accumulate(src[sy * stride + sx], ref a, ref r, ref g, ref b, 1);
                }
                dst[y * stride + x] = PackAverage(a, r, g, b, samples);
            }
        });
        return result;
    }

    public static unsafe SKBitmap RadialBlur(SKBitmap source, float amount, bool zoom)
    {
        SKBitmap result = source.Copy();
        int width = source.Width, height = source.Height;
        uint* src = (uint*)source.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        float cx = width / 2f, cy = height / 2f;
        const int samples = 24;
        Parallel.For(0, height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                long a = 0, r = 0, g = 0, b = 0;
                float ox = x - cx, oy = y - cy;
                for (int i = 0; i < samples; i++)
                {
                    float t = (i / (float)(samples - 1) - 0.5f) * amount / 100f;
                    float sx, sy;
                    if (zoom)
                    {
                        sx = cx + ox * (1 - t);
                        sy = cy + oy * (1 - t);
                    }
                    else
                    {
                        float angle = t * MathF.PI / 2;
                        float cos = MathF.Cos(angle), sin = MathF.Sin(angle);
                        sx = cx + ox * cos - oy * sin;
                        sy = cy + ox * sin + oy * cos;
                    }
                    Accumulate(src[Math.Clamp((int)sy, 0, height - 1) * stride + Math.Clamp((int)sx, 0, width - 1)], ref a, ref r, ref g, ref b, 1);
                }
                dst[y * stride + x] = PackAverage(a, r, g, b, samples);
            }
        });
        return result;
    }

    /// <summary>Edge-preserving blur that only averages neighbors within a color threshold.</summary>
    public static unsafe SKBitmap SurfaceBlur(SKBitmap source, int radius, float threshold)
    {
        SKBitmap result = source.Copy();
        int width = source.Width, height = source.Height;
        uint* src = (uint*)source.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        float t = Math.Max(1, threshold);
        Parallel.For(0, height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                uint center = src[y * stride + x];
                float wa = 0, a = 0, r = 0, g = 0, b = 0;
                for (int ky = -radius; ky <= radius; ky++)
                {
                    int sy = y + ky;
                    if (sy < 0 || sy >= height) continue;
                    for (int kx = -radius; kx <= radius; kx++)
                    {
                        int sx = x + kx;
                        if (sx < 0 || sx >= width) continue;
                        uint p = src[sy * stride + sx];
                        float w = Math.Max(0, 1 - ColorMath.Distance(p, center) / (2.5f * t));
                        if (w <= 0) continue;
                        wa += w;
                        a += (p >> 24) * w; r += ((p >> 16) & 0xFF) * w; g += ((p >> 8) & 0xFF) * w; b += (p & 0xFF) * w;
                    }
                }
                if (wa <= 0) continue;
                uint A = (uint)Math.Clamp(a / wa, 0, 255);
                dst[y * stride + x] = (A << 24) | ((uint)Math.Clamp(r / wa, 0, A) << 16) | ((uint)Math.Clamp(g / wa, 0, A) << 8) | (uint)Math.Clamp(b / wa, 0, A);
            }
        });
        return result;
    }

    public static SKBitmap UnsharpMask(SKBitmap source, float radius, float amount, float threshold)
    {
        using SKBitmap blurred = GaussianBlur(source, radius);
        return Combine(source, blurred, (o, bl) =>
        {
            float diff = o - bl;
            if (MathF.Abs(diff) * 255 < threshold) return o;
            return o + diff * amount;
        });
    }

    public static SKBitmap HighPass(SKBitmap source, float radius)
    {
        using SKBitmap blurred = GaussianBlur(source, radius);
        return Combine(source, blurred, (o, bl) => 0.5f + (o - bl));
    }

    private static unsafe SKBitmap Combine(SKBitmap a, SKBitmap b, Func<float, float, float> channel)
    {
        SKBitmap result = a.Copy();
        uint* pa = (uint*)a.GetPixels();
        uint* pb = (uint*)b.GetPixels();
        uint* pr = (uint*)result.GetPixels();
        int stride = a.RowBytes / 4;
        int width = a.Width;
        Parallel.For(0, a.Height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                int i = y * stride + x;
                if ((pa[i] >> 24) == 0) continue;
                ColorMath.Unpack(pa[i], out float r1, out float g1, out float b1, out float alpha);
                ColorMath.Unpack(pb[i], out float r2, out float g2, out float b2, out _);
                pr[i] = ColorMath.PackUnpremul(channel(r1, r2), channel(g1, g2), channel(b1, b2), alpha);
            }
        });
        return result;
    }

    public static unsafe SKBitmap AddNoise(SKBitmap source, float amount, bool gaussian, bool monochromatic, int seed)
    {
        SKBitmap result = source.Copy();
        uint* p = (uint*)result.GetPixels();
        int stride = result.RowBytes / 4;
        int width = result.Width;
        float scale = amount / 100f;
        Parallel.For(0, result.Height, y =>
        {
            Random random = new(seed * 7919 + y);
            float Next()
            {
                if (!gaussian) return (random.NextSingle() * 2 - 1);
                float u1 = 1 - random.NextSingle(), u2 = random.NextSingle();
                return MathF.Sqrt(-2 * MathF.Log(u1)) * MathF.Cos(2 * MathF.PI * u2) * 0.5f;
            }
            for (int x = 0; x < width; x++)
            {
                uint px = p[y * stride + x];
                if ((px >> 24) == 0) continue;
                ColorMath.Unpack(px, out float r, out float g, out float b, out float a);
                if (monochromatic)
                {
                    float n = Next() * scale;
                    r += n; g += n; b += n;
                }
                else
                {
                    r += Next() * scale; g += Next() * scale; b += Next() * scale;
                }
                p[y * stride + x] = ColorMath.PackUnpremul(r, g, b, a);
            }
        });
        return result;
    }

    public static unsafe SKBitmap Median(SKBitmap source, int radius)
    {
        SKBitmap result = source.Copy();
        int width = source.Width, height = source.Height;
        uint* src = (uint*)source.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        // Half width of the round window for every row offset.
        int[] half = new int[radius * 2 + 1];
        int count = 0;
        for (int ky = -radius; ky <= radius; ky++)
        {
            half[ky + radius] = (int)MathF.Floor(MathF.Sqrt(Math.Max(0, radius * radius + radius - ky * ky)));
            count += half[ky + radius] * 2 + 1;
        }
        int middle = count / 2;
        Parallel.For(0, height, y =>
        {
            int[] ha = new int[256], hr = new int[256], hg = new int[256], hb = new int[256];
            void Add(uint p, int sign)
            {
                ha[p >> 24] += sign; hr[(p >> 16) & 0xFF] += sign; hg[(p >> 8) & 0xFF] += sign; hb[p & 0xFF] += sign;
            }
            // Fill the window for x = 0, then slide it: each step removes one pixel and adds one per window row.
            for (int ky = -radius; ky <= radius; ky++)
            {
                uint* row = src + Math.Clamp(y + ky, 0, height - 1) * stride;
                int w = half[ky + radius];
                for (int kx = -w; kx <= w; kx++) Add(row[Math.Clamp(kx, 0, width - 1)], 1);
            }
            for (int x = 0; x < width; x++)
            {
                if (x > 0)
                {
                    for (int ky = -radius; ky <= radius; ky++)
                    {
                        uint* row = src + Math.Clamp(y + ky, 0, height - 1) * stride;
                        int w = half[ky + radius];
                        Add(row[Math.Clamp(x - 1 - w, 0, width - 1)], -1);
                        Add(row[Math.Clamp(x + w, 0, width - 1)], 1);
                    }
                }
                uint A = (uint)MedianOf(ha, middle);
                dst[y * stride + x] = (A << 24) | ((uint)Math.Min(MedianOf(hr, middle), (int)A) << 16) | ((uint)Math.Min(MedianOf(hg, middle), (int)A) << 8) | (uint)Math.Min(MedianOf(hb, middle), (int)A);
            }
        });
        return result;
    }

    private static int MedianOf(int[] histogram, int half)
    {
        int acc = 0;
        for (int i = 0; i < 256; i++)
        {
            acc += histogram[i];
            if (acc > half) return i;
        }
        return 255;
    }

    public static SKBitmap Despeckle(SKBitmap source)
    {
        using SKBitmap median = Median(source, 1);
        using SKBitmap edges = FindEdgesMagnitude(source);
        // Keep edges, smooth flat areas.
        return Blend(source, median, edges, inverse: true);
    }

    public static SKBitmap DustAndScratches(SKBitmap source, int radius, float threshold)
    {
        using SKBitmap median = Median(source, radius);
        return Combine(source, median, (o, m) => MathF.Abs(o - m) * 255 > threshold ? m : o);
    }

    private static unsafe SKBitmap FindEdgesMagnitude(SKBitmap source)
    {
        SKBitmap result = ImageOps.CreateBitmap(source.Width, source.Height);
        int width = source.Width, height = source.Height;
        uint* src = (uint*)source.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        float L(int x, int y)
        {
            ColorMath.Unpack(src[Math.Clamp(y, 0, height - 1) * stride + Math.Clamp(x, 0, width - 1)], out float r, out float g, out float b, out _);
            return ColorMath.Luminance(r, g, b);
        }
        Parallel.For(0, height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                float gx = -L(x - 1, y - 1) - 2 * L(x - 1, y) - L(x - 1, y + 1) + L(x + 1, y - 1) + 2 * L(x + 1, y) + L(x + 1, y + 1);
                float gy = -L(x - 1, y - 1) - 2 * L(x, y - 1) - L(x + 1, y - 1) + L(x - 1, y + 1) + 2 * L(x, y + 1) + L(x + 1, y + 1);
                uint m = (uint)Math.Clamp(MathF.Sqrt(gx * gx + gy * gy) * 255, 0, 255);
                dst[y * stride + x] = 0xFF000000 | (m << 16) | (m << 8) | m;
            }
        });
        return result;
    }

    private static unsafe SKBitmap Blend(SKBitmap a, SKBitmap b, SKBitmap weights, bool inverse)
    {
        SKBitmap result = a.Copy();
        uint* pa = (uint*)a.GetPixels(), pb = (uint*)b.GetPixels(), pw = (uint*)weights.GetPixels(), pr = (uint*)result.GetPixels();
        int stride = a.RowBytes / 4;
        for (int y = 0; y < a.Height; y++)
        {
            for (int x = 0; x < a.Width; x++)
            {
                int i = y * stride + x;
                float w = Math.Clamp((pw[i] & 0xFF) / 255f * 4, 0, 1);
                if (inverse) w = 1 - w;
                pr[i] = ColorMath.LerpPremul(pa[i], pb[i], w);
            }
        }
        return result;
    }

    public static SKBitmap FindEdges(SKBitmap source)
    {
        // The kernel sums to zero, so it must not run on the alpha channel (flat areas would become transparent).
        using SKBitmap opaque = source.Copy();
        ForceOpaque(opaque);
        using SKBitmap edges = Convolve(opaque, [-1, -1, -1, -1, 8, -1, -1, -1, -1], 3, 1, 0, keepAlpha: true);
        SKBitmap result = edges.Copy();
        ApplyFunction(result, (ref float r, ref float g, ref float b) => { r = 1 - r; g = 1 - g; b = 1 - b; });
        CopyAlpha(source, result);
        return result;
    }

    private static unsafe void ForceOpaque(SKBitmap bitmap)
    {
        uint* p = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        for (int y = 0; y < bitmap.Height; y++)
            for (int x = 0; x < bitmap.Width; x++)
                p[y * stride + x] |= 0xFF000000u;
    }

    private static unsafe void CopyAlpha(SKBitmap from, SKBitmap to)
    {
        uint* s = (uint*)from.GetPixels(), d = (uint*)to.GetPixels();
        int stride = from.RowBytes / 4;
        for (int y = 0; y < from.Height; y++)
        {
            for (int x = 0; x < from.Width; x++)
            {
                int i = y * stride + x;
                ColorMath.Unpack(d[i], out float r, out float g, out float b, out _);
                d[i] = ColorMath.PackUnpremul(r, g, b, (s[i] >> 24) / 255f);
            }
        }
    }

    public static unsafe SKBitmap Emboss(SKBitmap source, float angle, float height, float amount)
    {
        float radians = angle * MathF.PI / 180;
        int dx = (int)MathF.Round(MathF.Cos(radians) * height), dy = (int)MathF.Round(-MathF.Sin(radians) * height);
        SKBitmap result = source.Copy();
        int width = source.Width, h = source.Height;
        uint* src = (uint*)source.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        Parallel.For(0, h, y =>
        {
            for (int x = 0; x < width; x++)
            {
                ColorMath.Unpack(src[Math.Clamp(y - dy, 0, h - 1) * stride + Math.Clamp(x - dx, 0, width - 1)], out float r1, out float g1, out float b1, out _);
                ColorMath.Unpack(src[Math.Clamp(y + dy, 0, h - 1) * stride + Math.Clamp(x + dx, 0, width - 1)], out float r2, out float g2, out float b2, out _);
                float v = 0.5f + (ColorMath.Luminance(r2, g2, b2) - ColorMath.Luminance(r1, g1, b1)) * amount;
                ColorMath.Unpack(src[y * stride + x], out _, out _, out _, out float a);
                dst[y * stride + x] = ColorMath.PackUnpremul(v, v, v, a);
            }
        });
        return result;
    }

    public static unsafe SKBitmap Mosaic(SKBitmap source, int size)
    {
        SKBitmap result = source.Copy();
        uint* src = (uint*)source.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        int width = source.Width, height = source.Height;
        for (int by = 0; by < height; by += size)
        {
            for (int bx = 0; bx < width; bx += size)
            {
                long a = 0, r = 0, g = 0, b = 0;
                int count = 0;
                for (int y = by; y < Math.Min(by + size, height); y++)
                    for (int x = bx; x < Math.Min(bx + size, width); x++) { Accumulate(src[y * stride + x], ref a, ref r, ref g, ref b, 1); count++; }
                uint avg = PackAverage(a, r, g, b, count);
                for (int y = by; y < Math.Min(by + size, height); y++)
                    for (int x = bx; x < Math.Min(bx + size, width); x++) dst[y * stride + x] = avg;
            }
        }
        return result;
    }

    /// <summary>Voronoi cells with averaged colors.</summary>
    public static unsafe SKBitmap Crystallize(SKBitmap source, int size, int seed)
    {
        int width = source.Width, height = source.Height;
        int gw = width / size + 2, gh = height / size + 2;
        Random random = new(seed);
        SKPoint[] points = new SKPoint[gw * gh];
        for (int gy = 0; gy < gh; gy++)
            for (int gx = 0; gx < gw; gx++)
                points[gy * gw + gx] = new SKPoint((gx + random.NextSingle()) * size, (gy + random.NextSingle()) * size);

        int[] owner = new int[width * height];
        Parallel.For(0, height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                int cx = x / size, cy = y / size;
                float best = float.MaxValue;
                int bestIndex = 0;
                for (int oy = -1; oy <= 1; oy++)
                {
                    for (int ox = -1; ox <= 1; ox++)
                    {
                        int gx = cx + ox, gy = cy + oy;
                        if (gx < 0 || gy < 0 || gx >= gw || gy >= gh) continue;
                        SKPoint p = points[gy * gw + gx];
                        float d = (p.X - x) * (p.X - x) + (p.Y - y) * (p.Y - y);
                        if (d < best) { best = d; bestIndex = gy * gw + gx; }
                    }
                }
                owner[y * width + x] = bestIndex;
            }
        });
        long[] sa = new long[points.Length], sr = new long[points.Length], sg = new long[points.Length], sb = new long[points.Length];
        int[] counts = new int[points.Length];
        uint* src = (uint*)source.GetPixels();
        int stride = source.RowBytes / 4;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int o = owner[y * width + x];
                uint p = src[y * stride + x];
                sa[o] += p >> 24; sr[o] += (p >> 16) & 0xFF; sg[o] += (p >> 8) & 0xFF; sb[o] += p & 0xFF;
                counts[o]++;
            }
        }
        SKBitmap result = source.Copy();
        uint* dst = (uint*)result.GetPixels();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int o = owner[y * width + x];
                dst[y * stride + x] = PackAverage(sa[o], sr[o], sg[o], sb[o], Math.Max(1, counts[o]));
            }
        }
        return result;
    }

    public static SKBitmap Pointillize(SKBitmap source, int size, SKColor background, int seed)
    {
        using SKBitmap cells = Crystallize(source, size, seed);
        SKBitmap result = ImageOps.CreateBitmap(source.Width, source.Height);
        using SKCanvas canvas = new(result);
        canvas.Clear(background);
        Random random = new(seed);
        using SKPaint paint = new() { IsAntialias = true };
        int count = source.Width * source.Height / Math.Max(1, size * size / 2);
        for (int i = 0; i < count; i++)
        {
            int x = random.Next(source.Width), y = random.Next(source.Height);
            paint.Color = cells.GetPixel(x, y);
            canvas.DrawCircle(x, y, size / 2f, paint);
        }
        return result;
    }

    private static float Noise2(int x, int y, int seed)
    {
        uint h = (uint)(x * 374761393 + y * 668265263 + seed * 1442695041);
        h = (h ^ (h >> 13)) * 1274126177;
        return ((h ^ (h >> 16)) & 0xFFFFFF) / (float)0xFFFFFF;
    }

    private static float SmoothNoise(float x, float y, int seed)
    {
        int x0 = (int)MathF.Floor(x), y0 = (int)MathF.Floor(y);
        float fx = x - x0, fy = y - y0;
        fx = fx * fx * (3 - 2 * fx);
        fy = fy * fy * (3 - 2 * fy);
        float a = Noise2(x0, y0, seed), b = Noise2(x0 + 1, y0, seed), c = Noise2(x0, y0 + 1, seed), d = Noise2(x0 + 1, y0 + 1, seed);
        return a + (b - a) * fx + (c - a) * fy + (a - b - c + d) * fx * fy;
    }

    public static float FractalNoise(float x, float y, int seed, int octaves = 6)
    {
        float sum = 0, amplitude = 0.5f, frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            sum += SmoothNoise(x * frequency, y * frequency, seed + i * 31) * amplitude;
            amplitude *= 0.5f;
            frequency *= 2;
        }
        return Math.Clamp(sum / 0.984f, 0, 1);
    }

    public static unsafe SKBitmap Clouds(SKBitmap source, SKColor foreground, SKColor background, int seed, bool difference)
    {
        SKBitmap result = difference ? source.Copy() : ImageOps.CreateBitmap(source.Width, source.Height);
        uint* p = (uint*)result.GetPixels();
        int stride = result.RowBytes / 4;
        int width = result.Width;
        float scale = 1f / 128;
        Parallel.For(0, result.Height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                float n = FractalNoise(x * scale, y * scale, seed);
                SKColor c = ColorMath.Lerp(foreground, background, n);
                if (difference)
                {
                    ColorMath.Unpack(p[y * stride + x], out float r, out float g, out float b, out float a);
                    p[y * stride + x] = ColorMath.PackUnpremul(MathF.Abs(r - c.Red / 255f), MathF.Abs(g - c.Green / 255f), MathF.Abs(b - c.Blue / 255f), Math.Max(a, 1));
                }
                else
                {
                    p[y * stride + x] = ColorMath.FromColor(c);
                }
            }
        });
        return result;
    }

    public static unsafe SKBitmap Fibers(SKBitmap source, SKColor foreground, SKColor background, float variance, float strength, int seed)
    {
        SKBitmap result = ImageOps.CreateBitmap(source.Width, source.Height);
        uint* p = (uint*)result.GetPixels();
        int stride = result.RowBytes / 4;
        int width = result.Width;
        Parallel.For(0, result.Height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                float n = FractalNoise(x / variance, y / (variance * strength * 8), seed, 4);
                p[y * stride + x] = ColorMath.FromColor(ColorMath.Lerp(foreground, background, n));
            }
        });
        return result;
    }

    public static SKBitmap Vignette(SKBitmap source, float amount, float size)
    {
        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);
        float radius = MathF.Sqrt(source.Width * source.Width + source.Height * source.Height) / 2;
        using SKShader shader = SKShader.CreateRadialGradient(new SKPoint(source.Width / 2f, source.Height / 2f), radius,
            [SKColors.Transparent, SKColors.Transparent, SKColors.Black.WithAlpha((byte)(amount * 255))], [0, size, 1], SKShaderTileMode.Clamp);
        using SKPaint paint = new() { Shader = shader, BlendMode = SKBlendMode.SrcATop };
        canvas.DrawRect(0, 0, source.Width, source.Height, paint);
        return result;
    }

    public static unsafe SKBitmap Diffuse(SKBitmap source, int radius, int seed)
    {
        SKBitmap result = source.Copy();
        uint* src = (uint*)source.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        int width = source.Width, height = source.Height;
        Parallel.For(0, height, y =>
        {
            Random random = new(seed + y * 977);
            for (int x = 0; x < width; x++)
            {
                int sx = Math.Clamp(x + random.Next(-radius, radius + 1), 0, width - 1);
                int sy = Math.Clamp(y + random.Next(-radius, radius + 1), 0, height - 1);
                dst[y * stride + x] = src[sy * stride + sx];
            }
        });
        return result;
    }

    public static unsafe SKBitmap OilPaint(SKBitmap source, int radius, int levels)
    {
        SKBitmap result = source.Copy();
        uint* src = (uint*)source.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        int width = source.Width, height = source.Height;
        Parallel.For(0, height, y =>
        {
            int[] counts = new int[levels];
            long[] sr = new long[levels], sg = new long[levels], sb = new long[levels], sa = new long[levels];
            for (int x = 0; x < width; x++)
            {
                Array.Clear(counts); Array.Clear(sr); Array.Clear(sg); Array.Clear(sb); Array.Clear(sa);
                for (int ky = -radius; ky <= radius; ky++)
                {
                    int yy = Math.Clamp(y + ky, 0, height - 1);
                    for (int kx = -radius; kx <= radius; kx++)
                    {
                        uint p = src[yy * stride + Math.Clamp(x + kx, 0, width - 1)];
                        ColorMath.Unpack(p, out float r, out float g, out float b, out _);
                        int bin = Math.Clamp((int)(ColorMath.Luminance(r, g, b) * (levels - 1)), 0, levels - 1);
                        counts[bin]++;
                        sa[bin] += p >> 24; sr[bin] += (p >> 16) & 0xFF; sg[bin] += (p >> 8) & 0xFF; sb[bin] += p & 0xFF;
                    }
                }
                int best = 0;
                for (int i = 1; i < levels; i++) if (counts[i] > counts[best]) best = i;
                dst[y * stride + x] = PackAverage(sa[best], sr[best], sg[best], sb[best], Math.Max(1, counts[best]));
            }
        });
        return result;
    }

    public static SKBitmap Tiles(SKBitmap source, int count, float maxOffset, int seed)
    {
        SKBitmap result = ImageOps.CreateBitmap(source.Width, source.Height);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);
        int tile = Math.Max(1, Math.Min(source.Width, source.Height) / Math.Max(1, count));
        Random random = new(seed);
        // Convert once: drawing the bitmap directly would copy the whole image for every tile.
        using SKImage image = SKImage.FromBitmap(source);
        for (int y = 0; y < source.Height; y += tile)
        {
            for (int x = 0; x < source.Width; x += tile)
            {
                float ox = (random.NextSingle() * 2 - 1) * tile * maxOffset;
                float oy = (random.NextSingle() * 2 - 1) * tile * maxOffset;
                canvas.DrawImage(image, SKRect.Create(x, y, tile, tile), SKRect.Create(x + ox, y + oy, tile, tile));
            }
        }
        return result;
    }

    public static unsafe SKBitmap Morphology(SKBitmap source, int radius, bool maximum)
    {
        // Square window maximum/minimum per channel, done as two separable van Herk passes so the cost does not grow with the radius.
        // Pixels beyond the edge repeat the border, so Minimum does not eat into the image edges.
        int width = source.Width, height = source.Height;
        radius = Math.Max(1, radius);
        int window = radius * 2 + 1;
        SKBitmap temp = ImageOps.CreateBitmap(width, height);
        SKBitmap result = ImageOps.CreateBitmap(width, height);
        uint* src = (uint*)source.GetPixels();
        uint* mid = (uint*)temp.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int srcStride = source.RowBytes / 4, midStride = temp.RowBytes / 4, dstStride = result.RowBytes / 4;

        void Pass(int lines, int length, uint* from, int fromLine, int fromStep, uint* to, int toLine, int toStep)
        {
            int padded = length + window - 1;
            Parallel.For(0, lines,
                () => (new byte[padded], new byte[padded], new byte[padded], new byte[length]),
                (line, _, buffers) =>
                {
                    (byte[] input, byte[] g, byte[] h, byte[] output) = buffers;
                    for (int shift = 0; shift < 32; shift += 8)
                    {
                        for (int i = 0; i < padded; i++)
                        {
                            int k = Math.Clamp(i - radius, 0, length - 1);
                            input[i] = (byte)(from[line * fromLine + k * fromStep] >> shift);
                        }
                        for (int i = 0; i < padded; i++)
                            g[i] = i % window == 0 ? input[i] : maximum ? Math.Max(g[i - 1], input[i]) : Math.Min(g[i - 1], input[i]);
                        for (int i = padded - 1; i >= 0; i--)
                            h[i] = i == padded - 1 || (i + 1) % window == 0 ? input[i] : maximum ? Math.Max(h[i + 1], input[i]) : Math.Min(h[i + 1], input[i]);
                        for (int x = 0; x < length; x++)
                        {
                            byte value = maximum ? Math.Max(h[x], g[x + window - 1]) : Math.Min(h[x], g[x + window - 1]);
                            uint* target = to + line * toLine + x * toStep;
                            *target = (*target & ~(0xFFu << shift)) | ((uint)value << shift);
                        }
                    }
                    return buffers;
                },
                _ => { });
        }

        Pass(height, width, src, srcStride, 1, mid, midStride, 1);
        Pass(width, height, mid, 1, midStride, dst, 1, dstStride);
        temp.Dispose();
        return result;
    }

    public static SKBitmap Offset(SKBitmap source, int dx, int dy)
    {
        SKBitmap result = ImageOps.CreateBitmap(source.Width, source.Height);
        using SKCanvas canvas = new(result);
        int w = source.Width, h = source.Height;
        dx = ((dx % w) + w) % w;
        dy = ((dy % h) + h) % h;
        for (int oy = -1; oy <= 0; oy++)
            for (int ox = -1; ox <= 0; ox++)
                canvas.DrawBitmap(source, dx + ox * w, dy + oy * h);
        return result;
    }

    /// <summary>Applies a coordinate mapping (destination to source) with bilinear sampling.</summary>
    public static unsafe SKBitmap Displace(SKBitmap source, Func<float, float, SKPoint> map)
    {
        SKBitmap result = ImageOps.CreateBitmap(source.Width, source.Height);
        uint* src = (uint*)source.GetPixels();
        uint* dst = (uint*)result.GetPixels();
        int stride = source.RowBytes / 4;
        int width = source.Width, height = source.Height;
        Parallel.For(0, height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                SKPoint s = map(x + 0.5f, y + 0.5f);
                dst[y * stride + x] = SampleBilinear(src, stride, width, height, s.X - 0.5f, s.Y - 0.5f);
            }
        });
        return result;
    }

    public static unsafe uint SampleBilinear(uint* src, int stride, int width, int height, float x, float y)
    {
        if (x < -1 || y < -1 || x > width || y > height) return 0;
        int x0 = (int)MathF.Floor(x), y0 = (int)MathF.Floor(y);
        float fx = x - x0, fy = y - y0;
        uint P(int px, int py) => src[Math.Clamp(py, 0, height - 1) * stride + Math.Clamp(px, 0, width - 1)];
        uint top = ColorMath.LerpPremul(P(x0, y0), P(x0 + 1, y0), fx);
        uint bottom = ColorMath.LerpPremul(P(x0, y0 + 1), P(x0 + 1, y0 + 1), fx);
        return ColorMath.LerpPremul(top, bottom, fy);
    }

    public static SKBitmap Twirl(SKBitmap source, float angle)
    {
        float cx = source.Width / 2f, cy = source.Height / 2f, radius = Math.Min(cx, cy);
        float radians = angle * MathF.PI / 180;
        return Displace(source, (x, y) =>
        {
            float dx = x - cx, dy = y - cy;
            float d = MathF.Sqrt(dx * dx + dy * dy);
            if (d >= radius) return new SKPoint(x, y);
            float a = radians * (1 - d / radius);
            float cos = MathF.Cos(a), sin = MathF.Sin(a);
            return new SKPoint(cx + dx * cos - dy * sin, cy + dx * sin + dy * cos);
        });
    }

    public static SKBitmap Pinch(SKBitmap source, float amount)
    {
        float cx = source.Width / 2f, cy = source.Height / 2f, radius = Math.Min(cx, cy);
        return Displace(source, (x, y) =>
        {
            float dx = (x - cx) / radius, dy = (y - cy) / radius;
            float d = MathF.Sqrt(dx * dx + dy * dy);
            if (d >= 1 || d == 0) return new SKPoint(x, y);
            float factor = MathF.Pow(MathF.Sin(MathF.PI * d / 2), -amount);
            return new SKPoint(cx + dx * factor * radius, cy + dy * factor * radius);
        });
    }

    public static SKBitmap Spherize(SKBitmap source, float amount)
    {
        float cx = source.Width / 2f, cy = source.Height / 2f, rx = cx, ry = cy;
        return Displace(source, (x, y) =>
        {
            float dx = (x - cx) / rx, dy = (y - cy) / ry;
            float d = MathF.Sqrt(dx * dx + dy * dy);
            if (d >= 1 || d == 0) return new SKPoint(x, y);
            float bent = amount >= 0 ? MathF.Asin(d) / (MathF.PI / 2) : MathF.Sin(d * MathF.PI / 2);
            float t = d + (bent - d) * MathF.Abs(amount);
            float k = t / d;
            return new SKPoint(cx + dx * k * rx, cy + dy * k * ry);
        });
    }

    public static SKBitmap Wave(SKBitmap source, float amplitude, float wavelengthX, float wavelengthY) =>
        Displace(source, (x, y) => new SKPoint(x + amplitude * MathF.Sin(2 * MathF.PI * y / wavelengthY), y + amplitude * MathF.Sin(2 * MathF.PI * x / wavelengthX)));

    public static SKBitmap Polar(SKBitmap source, bool toPolar)
    {
        float w = source.Width, h = source.Height, cx = w / 2, cy = h / 2;
        float maxR = MathF.Min(cx, cy);
        return Displace(source, (x, y) =>
        {
            if (toPolar)
            {
                float dx = x - cx, dy = y - cy;
                float r = MathF.Sqrt(dx * dx + dy * dy) / maxR;
                float a = (MathF.Atan2(dx, -dy) + MathF.PI) / (2 * MathF.PI);
                return new SKPoint(a * w, r * h);
            }
            float angle = x / w * 2 * MathF.PI - MathF.PI;
            float radius = y / h * maxR;
            return new SKPoint(cx + MathF.Sin(angle) * radius, cy - MathF.Cos(angle) * radius);
        });
    }
}
