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

public enum HdrMethod
{
    LocalAdaptation,
    EqualizeHistogram,
    ExposureAndGamma,
    HighlightCompression
}

/// <summary>HDR Toning: separates tone from detail so the tonal range and the local contrast can be changed independently.</summary>
public static class HdrToning
{
    /// <summary>Above this many pixels the tone is measured on a smaller grid, which keeps previews fast and memory use bounded.</summary>
    private const double MaxGridPixels = 2_000_000;

    /// <summary>Added to the luminance before taking its logarithm so black stays finite.</summary>
    private const float LuminanceFloor = 1 / 1024f;

    private const float ShadowHighlightStops = 2;
    private const float ShadowHighlightRange = 3;

    public static readonly (string Name, float Radius, float Strength, bool SmoothEdges, float Gamma, float Exposure, float Detail, float Shadow, float Highlight, float Vibrance, float Saturation)[] Presets =
    [
        (Strings.HdrToning_PresetDefault, 300, 0.52f, false, 1, 0, 30, 0, 0, 0, 20),
        (Strings.HdrToning_PresetFlat, 300, 0.52f, true, 1.4f, 0, 0, 20, -20, 0, 0),
        (Strings.HdrToning_PresetMonochromatic, 300, 0.52f, false, 1, 0, 30, 0, 0, 0, -100),
        (Strings.HdrToning_PresetPhotorealistic, 300, 0.8f, true, 1.1f, 0, 30, 10, -10, 10, 10),
        (Strings.HdrToning_PresetSaturated, 300, 0.52f, false, 1, 0, 30, 0, 0, 40, 40),
        (Strings.HdrToning_PresetSurrealistic, 40, 2, false, 1, 0, 120, 30, -30, 20, 40)
    ];

    /// <summary>Sets the sliders of a preset and resets the toning curve. The method is left unchanged.</summary>
    public static void ApplyPreset(AdjustmentSettings s, int index)
    {
        var preset = Presets[index];
        s.Set("radius", preset.Radius);
        s.Set("strength", preset.Strength);
        s.Set("smoothEdges", preset.SmoothEdges ? 1 : 0);
        s.Set("gamma", preset.Gamma);
        s.Set("exposure", preset.Exposure);
        s.Set("detail", preset.Detail);
        s.Set("shadow", preset.Shadow);
        s.Set("highlight", preset.Highlight);
        s.Set("vibrance", preset.Vibrance);
        s.Set("saturation", preset.Saturation);
        s.Curves[0] = [new SKPoint(0, 0), new SKPoint(255, 255)];
    }

    public static void Apply(AdjustmentSettings s, SKBitmap bitmap, SKRectI area, Func<int, int, float>? weight)
    {
        switch ((HdrMethod)(int)s.Get("method"))
        {
            case HdrMethod.EqualizeHistogram:
                Adjustments.Equalize(bitmap, area, weight);
                break;
            case HdrMethod.ExposureAndGamma:
                AdjustmentSettings exposure = new(AdjustmentKind.Exposure);
                exposure.Set("exposure", s.Get("globalExposure"));
                exposure.Set("gamma", s.Get("globalGamma", 1));
                Adjustments.Apply(Adjustments.CreateFunction(exposure), bitmap, area, weight);
                break;
            case HdrMethod.HighlightCompression:
                Adjustments.Apply(CompressHighlights, bitmap, area, weight);
                break;
            default:
                ApplyLocalAdaptation(s, bitmap, area, weight);
                break;
        }
    }

    /// <summary>Maps white to white while rolling the highlights off and lifting the midtones.</summary>
    private static void CompressHighlights(ref float r, ref float g, ref float b)
    {
        float lr = ColorMath.ToLinear(r), lg = ColorMath.ToLinear(g), lb = ColorMath.ToLinear(b);
        float luminance = LinearLuminance(lr, lg, lb);
        if (luminance <= 0) return;
        float scaled = luminance * 2;
        float ratio = scaled * (1 + scaled / 4) / (1 + scaled) / luminance;
        r = ColorMath.FromLinear(lr * ratio);
        g = ColorMath.FromLinear(lg * ratio);
        b = ColorMath.FromLinear(lb * ratio);
    }

    private static float LinearLuminance(float r, float g, float b) => 0.2126f * r + 0.7152f * g + 0.0722f * b;

    private static float LogLuminance(float r, float g, float b) =>
        MathF.Log2(LinearLuminance(ColorMath.ToLinear(r), ColorMath.ToLinear(g), ColorMath.ToLinear(b)) + LuminanceFloor);

    private static unsafe void ApplyLocalAdaptation(AdjustmentSettings s, SKBitmap bitmap, SKRectI area, Func<int, int, float>? weight)
    {
        int width = area.Width, height = area.Height;
        uint* pixels = (uint*)bitmap.GetPixels();
        int stride = bitmap.RowBytes / 4;
        int left = area.Left, top = area.Top;

        float[] log = new float[width * height];
        double sum = 0;
        long count = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                uint p = pixels[(y + top) * stride + x + left];
                if ((p >> 24) == 0)
                {
                    log[y * width + x] = float.NaN;
                    continue;
                }
                ColorMath.Unpack(p, out float r, out float g, out float b, out _);
                float value = LogLuminance(r, g, b);
                log[y * width + x] = value;
                sum += value;
                count++;
            }
        }
        if (count == 0) return;
        float pivot = (float)(sum / count);
        // Transparent pixels take the average tone so they do not darken the tone of their neighbors.
        for (int i = 0; i < log.Length; i++)
        {
            if (float.IsNaN(log[i])) log[i] = pivot;
        }

        float strength = s.Get("strength", 0.52f);
        float epsilon = strength * strength * (s.Get("smoothEdges") > 0 ? 1 : 16);
        ToneGrid grid = new(log, width, height, Math.Max(1, (int)s.Get("radius", 300)), epsilon);

        float gamma = Math.Max(0.01f, s.Get("gamma", 1));
        float exposure = s.Get("exposure");
        float detailGain = 1 + s.Get("detail") / 100f;
        float shadow = s.Get("shadow") / 100f * ShadowHighlightStops;
        float highlight = s.Get("highlight") / 100f * ShadowHighlightStops;
        float[] curve = Adjustments.CurveLut(s.Curves[0]);
        AdjustmentSettings saturation = new(AdjustmentKind.Vibrance);
        saturation.Set("vibrance", s.Get("vibrance"));
        saturation.Set("saturation", s.Get("saturation"));
        Adjustments.PixelFunction saturate = Adjustments.CreateFunction(saturation);

        Parallel.For(0, height, y =>
        {
            uint* row = pixels + (y + top) * stride + left;
            for (int x = 0; x < width; x++)
            {
                uint p = row[x];
                if ((p >> 24) == 0) continue;
                float w = weight?.Invoke(x + left, y + top) ?? 1;
                if (w <= 0) continue;
                ColorMath.Unpack(p, out float r0, out float g0, out float b0, out float alpha);
                float value = log[y * width + x];
                float tone = grid.Smooth(x, y, value);
                float adjusted = pivot + (tone - pivot) / gamma + exposure
                    + shadow * Math.Clamp((pivot - tone) / ShadowHighlightRange, 0, 1)
                    + highlight * Math.Clamp((tone - pivot) / ShadowHighlightRange, 0, 1);
                float ratio = MathF.Pow(2, adjusted + (value - tone) * detailGain - value);
                float r = ColorMath.FromLinear(ColorMath.ToLinear(r0) * ratio);
                float g = ColorMath.FromLinear(ColorMath.ToLinear(g0) * ratio);
                float b = ColorMath.FromLinear(ColorMath.ToLinear(b0) * ratio);
                r = curve[(int)(r * 255 + 0.5f)];
                g = curve[(int)(g * 255 + 0.5f)];
                b = curve[(int)(b * 255 + 0.5f)];
                saturate(ref r, ref g, ref b);
                row[x] = ColorMath.PackUnpremul(r0 + (r - r0) * w, g0 + (g - g0) * w, b0 + (b - b0) * w, alpha);
            }
        });
    }

    /// <summary>Edge-preserving smoothing of the tone (a guided filter), stored as coefficients on a coarse grid.</summary>
    private sealed class ToneGrid
    {
        private readonly float[] _scale;
        private readonly float[] _offset;
        private readonly int _width;
        private readonly int _height;
        private readonly int _step;

        public ToneGrid(float[] log, int width, int height, int radius, float epsilon)
        {
            _step = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(width * (double)height / MaxGridPixels)));
            _width = (width + _step - 1) / _step;
            _height = (height + _step - 1) / _step;
            float[] guide = Downsample(log, width, height);
            int gridRadius = Math.Max(1, radius / _step);
            float[] mean = Box(guide, gridRadius);
            float[] squares = new float[guide.Length];
            for (int i = 0; i < guide.Length; i++) squares[i] = guide[i] * guide[i];
            float[] meanSquares = Box(squares, gridRadius);
            float[] scale = new float[guide.Length];
            float[] offset = new float[guide.Length];
            for (int i = 0; i < guide.Length; i++)
            {
                float variance = Math.Max(0, meanSquares[i] - mean[i] * mean[i]);
                scale[i] = variance / (variance + epsilon);
                offset[i] = mean[i] * (1 - scale[i]);
            }
            _scale = Box(scale, gridRadius);
            _offset = Box(offset, gridRadius);
        }

        /// <summary>Returns the smoothed tone of an image pixel with the given value.</summary>
        public float Smooth(int x, int y, float value)
        {
            float gx = Math.Clamp((x + 0.5f) / _step - 0.5f, 0, _width - 1);
            float gy = Math.Clamp((y + 0.5f) / _step - 0.5f, 0, _height - 1);
            int x0 = (int)gx, y0 = (int)gy;
            int x1 = Math.Min(x0 + 1, _width - 1), y1 = Math.Min(y0 + 1, _height - 1);
            float fx = gx - x0, fy = gy - y0;
            float Interpolate(float[] grid)
            {
                float top = grid[y0 * _width + x0] + (grid[y0 * _width + x1] - grid[y0 * _width + x0]) * fx;
                float bottom = grid[y1 * _width + x0] + (grid[y1 * _width + x1] - grid[y1 * _width + x0]) * fx;
                return top + (bottom - top) * fy;
            }
            return Interpolate(_scale) * value + Interpolate(_offset);
        }

        private float[] Downsample(float[] source, int width, int height)
        {
            if (_step == 1) return source;
            float[] result = new float[_width * _height];
            int[] counts = new int[result.Length];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int cell = y / _step * _width + x / _step;
                    result[cell] += source[y * width + x];
                    counts[cell]++;
                }
            }
            for (int i = 0; i < result.Length; i++) result[i] /= counts[i];
            return result;
        }

        /// <summary>Box blur whose cost does not depend on the radius; the window shrinks at the edges.</summary>
        private float[] Box(float[] source, int radius)
        {
            int width = _width, height = _height;
            float[] rows = new float[source.Length];
            float[] result = new float[source.Length];
            Parallel.For(0, height, y => BoxLine(source, rows, y * width, 1, width, radius));
            Parallel.For(0, width, x => BoxLine(rows, result, x, width, height, radius));
            return result;
        }

        private static void BoxLine(float[] source, float[] target, int start, int stride, int length, int radius)
        {
            double sum = 0;
            int count = 0;
            for (int i = 0; i <= Math.Min(radius, length - 1); i++)
            {
                sum += source[start + i * stride];
                count++;
            }
            for (int i = 0; i < length; i++)
            {
                target[start + i * stride] = (float)(sum / count);
                int entering = i + radius + 1, leaving = i - radius;
                if (entering < length)
                {
                    sum += source[start + entering * stride];
                    count++;
                }
                if (leaving >= 0)
                {
                    sum -= source[start + leaving * stride];
                    count--;
                }
            }
        }
    }
}
