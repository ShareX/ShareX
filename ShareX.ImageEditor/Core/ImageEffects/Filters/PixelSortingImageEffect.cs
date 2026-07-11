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

using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public enum PixelSortDirection
{
    Horizontal,
    Vertical
}

public enum PixelSortMetric
{
    Brightness,
    Hue
}

public sealed class PixelSortingImageEffect : ImageEffectBase
{
    public override string Id => "pixel_sorting";
    public override string Name => "Pixel sorting";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.arrow_down_wide_narrow;
    public override string Description => "Sorts pixels along rows or columns based on brightness or hue thresholds.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<PixelSortingImageEffect, PixelSortDirection>("direction", "Direction", PixelSortDirection.Vertical, (e, v) => e.Direction = v,
            new (string, PixelSortDirection)[] { ("Horizontal", PixelSortDirection.Horizontal), ("Vertical", PixelSortDirection.Vertical) }),
        EffectParameters.Enum<PixelSortingImageEffect, PixelSortMetric>("metric", "Metric", PixelSortMetric.Brightness, (e, v) => e.Metric = v,
            new (string, PixelSortMetric)[] { ("Brightness", PixelSortMetric.Brightness), ("Hue", PixelSortMetric.Hue) }),
        EffectParameters.FloatSlider<PixelSortingImageEffect>("threshold_low", "Threshold low", 0f, 100f, 12f, (e, v) => e.ThresholdLow = v),
        EffectParameters.FloatSlider<PixelSortingImageEffect>("threshold_high", "Threshold high", 0f, 100f, 85f, (e, v) => e.ThresholdHigh = v),
        EffectParameters.IntSlider<PixelSortingImageEffect>("min_span_length", "Min span length", 2, 256, 8, (e, v) => e.MinSpanLength = v),
        EffectParameters.IntSlider<PixelSortingImageEffect>("max_span_length", "Max span length", 2, 512, 120, (e, v) => e.MaxSpanLength = v),
        EffectParameters.FloatSlider<PixelSortingImageEffect>("sort_probability", "Sort probability", 0f, 100f, 85f, (e, v) => e.SortProbability = v),
    ];

    public PixelSortDirection Direction { get; set; } = PixelSortDirection.Vertical;
    public PixelSortMetric Metric { get; set; } = PixelSortMetric.Brightness;
    public float ThresholdLow { get; set; } = 12f; // 0..100
    public float ThresholdHigh { get; set; } = 85f; // 0..100
    public int MinSpanLength { get; set; } = 8; // 2..256
    public int MaxSpanLength { get; set; } = 120; // 2..512
    public float SortProbability { get; set; } = 85f; // 0..100
    public int Seed { get; set; } = 3110;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        float low = Math.Clamp(ThresholdLow, 0f, 100f) / 100f;
        float high = Math.Clamp(ThresholdHigh, 0f, 100f) / 100f;
        if (high < low)
        {
            (low, high) = (high, low);
        }

        int minSpan = Math.Clamp(MinSpanLength, 2, 256);
        int maxSpan = Math.Clamp(MaxSpanLength, minSpan, 512);
        float probability = Math.Clamp(SortProbability, 0f, 100f) / 100f;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];
        Array.Copy(srcPixels, dstPixels, srcPixels.Length);

        if (Direction == PixelSortDirection.Horizontal)
        {
            SortHorizontal(dstPixels, width, height, low, high, minSpan, maxSpan, probability);
        }
        else
        {
            SortVertical(dstPixels, width, height, low, high, minSpan, maxSpan, probability);
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private void SortHorizontal(
        SKColor[] pixels,
        int width,
        int height,
        float low,
        float high,
        int minSpan,
        int maxSpan,
        float probability)
    {
        for (int y = 0; y < height; y++)
        {
            int x = 0;
            while (x < width)
            {
                int row = y * width;
                float metric = ComputeMetric(pixels[row + x]);
                if (metric < low || metric > high)
                {
                    x++;
                    continue;
                }

                int start = x;
                x++;
                while (x < width)
                {
                    float m = ComputeMetric(pixels[row + x]);
                    if (m < low || m > high)
                    {
                        break;
                    }
                    x++;
                }

                int spanLength = x - start;
                if (spanLength < minSpan)
                {
                    continue;
                }

                int cursor = start;
                while (cursor < x)
                {
                    int remaining = x - cursor;
                    int length = Math.Min(remaining, maxSpan);
                    if (length < minSpan)
                    {
                        break;
                    }

                    float selector = ProceduralEffectHelper.Hash01(cursor, y, Seed ^ 1187);
                    if (selector <= probability)
                    {
                        SortSegment(pixels, row + cursor, length, 1);
                    }

                    cursor += length;
                }
            }
        }
    }

    private void SortVertical(
        SKColor[] pixels,
        int width,
        int height,
        float low,
        float high,
        int minSpan,
        int maxSpan,
        float probability)
    {
        for (int x = 0; x < width; x++)
        {
            int y = 0;
            while (y < height)
            {
                float metric = ComputeMetric(pixels[(y * width) + x]);
                if (metric < low || metric > high)
                {
                    y++;
                    continue;
                }

                int start = y;
                y++;
                while (y < height)
                {
                    float m = ComputeMetric(pixels[(y * width) + x]);
                    if (m < low || m > high)
                    {
                        break;
                    }
                    y++;
                }

                int spanLength = y - start;
                if (spanLength < minSpan)
                {
                    continue;
                }

                int cursor = start;
                while (cursor < y)
                {
                    int remaining = y - cursor;
                    int length = Math.Min(remaining, maxSpan);
                    if (length < minSpan)
                    {
                        break;
                    }

                    float selector = ProceduralEffectHelper.Hash01(x, cursor, Seed ^ 7723);
                    if (selector <= probability)
                    {
                        SortSegment(pixels, (cursor * width) + x, length, width);
                    }

                    cursor += length;
                }
            }
        }
    }

    private void SortSegment(SKColor[] pixels, int startIndex, int length, int stride)
    {
        SKColor[] segment = new SKColor[length];
        for (int i = 0; i < length; i++)
        {
            segment[i] = pixels[startIndex + (i * stride)];
        }

        Array.Sort(segment, (a, b) => ComputeMetric(a).CompareTo(ComputeMetric(b)));

        for (int i = 0; i < length; i++)
        {
            pixels[startIndex + (i * stride)] = segment[i];
        }
    }

    private float ComputeMetric(SKColor color)
    {
        if (Metric == PixelSortMetric.Hue)
        {
            return ComputeHue(color);
        }

        return ((0.2126f * color.Red) + (0.7152f * color.Green) + (0.0722f * color.Blue)) / 255f;
    }

    private static float ComputeHue(SKColor color)
    {
        float r = color.Red / 255f;
        float g = color.Green / 255f;
        float b = color.Blue / 255f;

        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));
        float delta = max - min;

        if (delta <= 0.0001f)
        {
            return 0f;
        }

        float hue;
        if (max == r)
        {
            hue = ((g - b) / delta) % 6f;
        }
        else if (max == g)
        {
            hue = ((b - r) / delta) + 2f;
        }
        else
        {
            hue = ((r - g) / delta) + 4f;
        }

        hue /= 6f;
        if (hue < 0f) hue += 1f;
        return hue;
    }
}