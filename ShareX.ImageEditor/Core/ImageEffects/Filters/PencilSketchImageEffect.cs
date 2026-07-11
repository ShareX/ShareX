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

public sealed class PencilSketchImageEffect : ImageEffectBase
{
    public override string Id => "pencil_sketch";
    public override string Name => "Pencil sketch";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.pencil_line;
    public override string Description => "Converts the image to a pencil sketch style drawing.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<PencilSketchImageEffect>("blur_radius", "Blur radius", 1, 24, 8, (e, v) => e.BlurRadius = v),
        EffectParameters.FloatSlider<PencilSketchImageEffect>("edge_strength", "Edge strength", 0f, 160f, 65f, (e, v) => e.EdgeStrength = v),
        EffectParameters.FloatSlider<PencilSketchImageEffect>("pencil_darkness", "Pencil darkness", 0f, 100f, 70f, (e, v) => e.PencilDarkness = v),
        EffectParameters.FloatSlider<PencilSketchImageEffect>("paper_brightness", "Paper brightness", 40f, 130f, 100f, (e, v) => e.PaperBrightness = v),
    ];

    public int BlurRadius { get; set; } = 8; // 1..24
    public float EdgeStrength { get; set; } = 65f; // 0..160
    public float PencilDarkness { get; set; } = 70f; // 0..100
    public float PaperBrightness { get; set; } = 100f; // 40..130

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        int blurRadius = Math.Clamp(BlurRadius, 1, 24);
        float edgeStrength = Math.Clamp(EdgeStrength, 0f, 160f) / 100f;
        float darkness = Math.Clamp(PencilDarkness, 0f, 100f) / 100f;
        float paper = Math.Clamp(PaperBrightness, 40f, 130f) / 100f;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];
        float[] gray = new float[srcPixels.Length];
        float[] inverse = new float[srcPixels.Length];

        for (int i = 0; i < srcPixels.Length; i++)
        {
            SKColor c = srcPixels[i];
            float luminance = ((0.2126f * c.Red) + (0.7152f * c.Green) + (0.0722f * c.Blue)) / 255f;
            gray[i] = luminance;
            inverse[i] = 1f - luminance;
        }

        float[] blurredInverse = BoxBlur(inverse, width, height, blurRadius);
        float[] edges = Sobel(gray, width, height);

        for (int i = 0; i < srcPixels.Length; i++)
        {
            float baseGray = gray[i];
            float dodge = baseGray / MathF.Max(1f - blurredInverse[i], 0.03f);
            float sketch = Math.Clamp(dodge, 0f, 1f);

            float edgeDarken = edges[i] * edgeStrength * 0.85f;
            float value = Math.Clamp(sketch - edgeDarken, 0f, 1f);

            float gamma = 1.0f + (darkness * 1.75f);
            value = MathF.Pow(value, gamma);
            value = ProceduralEffectHelper.Lerp(value, 1f, Math.Clamp(paper - 1f, 0f, 0.6f));

            byte v = ProceduralEffectHelper.ClampToByte(value * 255f);
            dstPixels[i] = new SKColor(v, v, v, srcPixels[i].Alpha);
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static float[] BoxBlur(float[] source, int width, int height, int radius)
    {
        int right = width - 1;
        int bottom = height - 1;
        int window = (radius * 2) + 1;

        float[] horizontal = new float[source.Length];
        float[] vertical = new float[source.Length];

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            float acc = 0f;

            for (int k = -radius; k <= radius; k++)
            {
                int sx = Clamp(k, 0, right);
                acc += source[row + sx];
            }

            for (int x = 0; x < width; x++)
            {
                horizontal[row + x] = acc / window;

                int removeX = Clamp(x - radius, 0, right);
                int addX = Clamp(x + radius + 1, 0, right);
                acc += source[row + addX] - source[row + removeX];
            }
        }

        for (int x = 0; x < width; x++)
        {
            float acc = 0f;

            for (int k = -radius; k <= radius; k++)
            {
                int sy = Clamp(k, 0, bottom);
                acc += horizontal[(sy * width) + x];
            }

            for (int y = 0; y < height; y++)
            {
                vertical[(y * width) + x] = acc / window;

                int removeY = Clamp(y - radius, 0, bottom);
                int addY = Clamp(y + radius + 1, 0, bottom);
                acc += horizontal[(addY * width) + x] - horizontal[(removeY * width) + x];
            }
        }

        return vertical;
    }

    private static float[] Sobel(float[] gray, int width, int height)
    {
        float[] edges = new float[gray.Length];
        int right = width - 1;
        int bottom = height - 1;

        for (int y = 0; y < height; y++)
        {
            int y0 = y > 0 ? y - 1 : 0;
            int y1 = y;
            int y2 = y < bottom ? y + 1 : bottom;

            for (int x = 0; x < width; x++)
            {
                int x0 = x > 0 ? x - 1 : 0;
                int x1 = x;
                int x2 = x < right ? x + 1 : right;

                float tl = gray[(y0 * width) + x0];
                float tc = gray[(y0 * width) + x1];
                float tr = gray[(y0 * width) + x2];
                float ml = gray[(y1 * width) + x0];
                float mr = gray[(y1 * width) + x2];
                float bl = gray[(y2 * width) + x0];
                float bc = gray[(y2 * width) + x1];
                float br = gray[(y2 * width) + x2];

                float gx = (-tl + tr) + (-2f * ml + (2f * mr)) + (-bl + br);
                float gy = (-tl - (2f * tc) - tr) + (bl + (2f * bc) + br);

                float magnitude = MathF.Sqrt((gx * gx) + (gy * gy));
                edges[(y * width) + x] = Math.Clamp(magnitude / 4f, 0f, 1f);
            }
        }

        return edges;
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}