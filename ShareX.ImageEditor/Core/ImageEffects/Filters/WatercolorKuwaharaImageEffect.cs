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

public sealed class WatercolorKuwaharaImageEffect : ImageEffectBase
{
    public override string Id => "watercolor_kuwahara";
    public override string Name => "Watercolor / Kuwahara";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.paintbrush;
    public override string Description => "Applies a Kuwahara filter for a watercolor painting effect.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<WatercolorKuwaharaImageEffect>("radius", "Radius", 2, 10, 4, (e, v) => e.Radius = v),
        EffectParameters.FloatSlider<WatercolorKuwaharaImageEffect>("saturation_boost", "Saturation boost", 0, 100, 20, (e, v) => e.SaturationBoost = v),
        EffectParameters.FloatSlider<WatercolorKuwaharaImageEffect>("detail_blend", "Detail blend", 0, 100, 18, (e, v) => e.DetailBlend = v)
    ];

    public int Radius { get; set; } = 4; // 2..10
    public float SaturationBoost { get; set; } = 20f; // 0..100
    public float DetailBlend { get; set; } = 18f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        int radius = Math.Clamp(Radius, 2, 10);
        float saturationBoost = Math.Clamp(SaturationBoost, 0f, 100f) / 100f;
        float detailBlend = Math.Clamp(DetailBlend, 0f, 100f) / 100f;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        int stride = width + 1;
        int integralSize = stride * (height + 1);

        double[] integralR = new double[integralSize];
        double[] integralG = new double[integralSize];
        double[] integralB = new double[integralSize];
        double[] integralL = new double[integralSize];
        double[] integralL2 = new double[integralSize];

        for (int y = 1; y <= height; y++)
        {
            double rowR = 0d;
            double rowG = 0d;
            double rowB = 0d;
            double rowL = 0d;
            double rowL2 = 0d;

            int srcRow = (y - 1) * width;
            int rowIndex = y * stride;
            int prevRowIndex = (y - 1) * stride;

            for (int x = 1; x <= width; x++)
            {
                SKColor c = srcPixels[srcRow + (x - 1)];
                double l = ((0.2126 * c.Red) + (0.7152 * c.Green) + (0.0722 * c.Blue)) / 255d;

                rowR += c.Red;
                rowG += c.Green;
                rowB += c.Blue;
                rowL += l;
                rowL2 += l * l;

                int index = rowIndex + x;
                integralR[index] = integralR[prevRowIndex + x] + rowR;
                integralG[index] = integralG[prevRowIndex + x] + rowG;
                integralB[index] = integralB[prevRowIndex + x] + rowB;
                integralL[index] = integralL[prevRowIndex + x] + rowL;
                integralL2[index] = integralL2[prevRowIndex + x] + rowL2;
            }
        }

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                QuadrantStats best = default;
                double bestVariance = double.MaxValue;

                EvaluateQuadrant(x - radius, y - radius, x + 1, y + 1, width, height, stride, integralR, integralG, integralB, integralL, integralL2, ref best, ref bestVariance);
                EvaluateQuadrant(x, y - radius, x + radius + 1, y + 1, width, height, stride, integralR, integralG, integralB, integralL, integralL2, ref best, ref bestVariance);
                EvaluateQuadrant(x - radius, y, x + 1, y + radius + 1, width, height, stride, integralR, integralG, integralB, integralL, integralL2, ref best, ref bestVariance);
                EvaluateQuadrant(x, y, x + radius + 1, y + radius + 1, width, height, stride, integralR, integralG, integralB, integralL, integralL2, ref best, ref bestVariance);

                SKColor filtered = new SKColor(
                    ProceduralEffectHelper.ClampToByte((float)best.MeanR),
                    ProceduralEffectHelper.ClampToByte((float)best.MeanG),
                    ProceduralEffectHelper.ClampToByte((float)best.MeanB),
                    srcPixels[row + x].Alpha);

                if (saturationBoost > 0f)
                {
                    filtered = BoostSaturation(filtered, saturationBoost);
                }

                if (detailBlend > 0f)
                {
                    SKColor src = srcPixels[row + x];
                    filtered = LerpColor(filtered, src, detailBlend * 0.55f);
                }

                dstPixels[row + x] = filtered;
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private readonly struct QuadrantStats
    {
        public QuadrantStats(double meanR, double meanG, double meanB)
        {
            MeanR = meanR;
            MeanG = meanG;
            MeanB = meanB;
        }

        public double MeanR { get; }
        public double MeanG { get; }
        public double MeanB { get; }
    }

    private static void EvaluateQuadrant(
        int x0,
        int y0,
        int x1,
        int y1,
        int width,
        int height,
        int stride,
        double[] integralR,
        double[] integralG,
        double[] integralB,
        double[] integralL,
        double[] integralL2,
        ref QuadrantStats best,
        ref double bestVariance)
    {
        int left = Math.Clamp(x0, 0, width);
        int top = Math.Clamp(y0, 0, height);
        int right = Math.Clamp(x1, 0, width);
        int bottom = Math.Clamp(y1, 0, height);

        if (right <= left || bottom <= top)
        {
            return;
        }

        int count = (right - left) * (bottom - top);
        if (count <= 0)
        {
            return;
        }

        double sumR = RectSum(integralR, stride, left, top, right, bottom);
        double sumG = RectSum(integralG, stride, left, top, right, bottom);
        double sumB = RectSum(integralB, stride, left, top, right, bottom);
        double sumL = RectSum(integralL, stride, left, top, right, bottom);
        double sumL2 = RectSum(integralL2, stride, left, top, right, bottom);

        double inv = 1d / count;
        double meanL = sumL * inv;
        double variance = (sumL2 * inv) - (meanL * meanL);

        if (variance < bestVariance)
        {
            bestVariance = variance;
            best = new QuadrantStats(sumR * inv, sumG * inv, sumB * inv);
        }
    }

    private static double RectSum(double[] integral, int stride, int x0, int y0, int x1, int y1)
    {
        int a = (y0 * stride) + x0;
        int b = (y0 * stride) + x1;
        int c = (y1 * stride) + x0;
        int d = (y1 * stride) + x1;
        return integral[d] - integral[b] - integral[c] + integral[a];
    }

    private static SKColor BoostSaturation(SKColor color, float amount01)
    {
        float r = color.Red;
        float g = color.Green;
        float b = color.Blue;
        float gray = (r + g + b) / 3f;

        float factor = 1f + amount01;
        r = gray + ((r - gray) * factor);
        g = gray + ((g - gray) * factor);
        b = gray + ((b - gray) * factor);

        return new SKColor(
            ProceduralEffectHelper.ClampToByte(r),
            ProceduralEffectHelper.ClampToByte(g),
            ProceduralEffectHelper.ClampToByte(b),
            color.Alpha);
    }

    private static SKColor LerpColor(SKColor from, SKColor to, float t)
    {
        float alpha = Math.Clamp(t, 0f, 1f);
        return new SKColor(
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(from.Red, to.Red, alpha)),
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(from.Green, to.Green, alpha)),
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(from.Blue, to.Blue, alpha)),
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(from.Alpha, to.Alpha, alpha)));
    }
}