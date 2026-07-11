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

public sealed class ChromaticAberrationImageEffect : ImageEffectBase
{
    public override string Id => "chromatic_aberration";
    public override string Name => "Chromatic aberration";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.aperture;
    public override string Description => "Simulates lens chromatic aberration with RGB channel splitting.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<ChromaticAberrationImageEffect>("amount", "Amount", 0f, 40f, 8f, (e, v) => e.Amount = v),
        EffectParameters.FloatSlider<ChromaticAberrationImageEffect>("edgeStart", "Edge start", 0f, 0.95f, 0.20f, (e, v) => e.EdgeStart = v),
        EffectParameters.FloatSlider<ChromaticAberrationImageEffect>("strength", "Strength", 0f, 100f, 75f, (e, v) => e.Strength = v),
    ];

    public float Amount { get; set; } = 8f; // 0..40 px
    public float EdgeStart { get; set; } = 0.20f; // 0..0.95
    public float Strength { get; set; } = 75f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float amount = Math.Clamp(Amount, 0f, 40f);
        float edgeStart = Math.Clamp(EdgeStart, 0f, 0.95f);
        float strength = Math.Clamp(Strength, 0f, 100f) / 100f;

        if (amount <= 0f || strength <= 0f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float cx = right * 0.5f;
        float cy = bottom * 0.5f;
        float invCx = cx > 0f ? 1f / cx : 0f;
        float invCy = cy > 0f ? 1f / cy : 0f;
        const float invSqrt2 = 0.70710678f;

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            float dyNorm = (y - cy) * invCy;

            for (int x = 0; x < width; x++)
            {
                SKColor original = srcPixels[row + x];

                float dxNorm = (x - cx) * invCx;
                float distance01 = MathF.Sqrt((dxNorm * dxNorm) + (dyNorm * dyNorm)) * invSqrt2;
                float radialMask = ProceduralEffectHelper.SmoothStep(edgeStart, 1f, distance01);

                if (radialMask <= 0f)
                {
                    dstPixels[row + x] = original;
                    continue;
                }

                float dirLength = MathF.Sqrt((dxNorm * dxNorm) + (dyNorm * dyNorm));
                float dirX = dirLength > 0.0001f ? dxNorm / dirLength : 0f;
                float dirY = dirLength > 0.0001f ? dyNorm / dirLength : 0f;

                float mix = strength * radialMask;
                if (mix <= 0.0001f)
                {
                    dstPixels[row + x] = original;
                    continue;
                }

                float shift = amount * radialMask;
                SKColor sampleR = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, x + (dirX * shift), y + (dirY * shift));
                SKColor sampleB = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, x - (dirX * shift), y - (dirY * shift));

                float splitContrast = ProceduralEffectHelper.SmoothStep(
                    0.01f,
                    0.30f,
                    MathF.Abs(Luminance(sampleR) - Luminance(sampleB)));
                float boost = mix * (0.85f + (1.35f * splitContrast));

                float red = ProceduralEffectHelper.Lerp(original.Red, sampleR.Red, mix) +
                            ((sampleR.Red - original.Red) * boost);
                float green = ProceduralEffectHelper.Lerp(
                    original.Green,
                    (sampleR.Green + original.Green + sampleB.Green) / 3f,
                    mix * 0.06f);
                float blue = ProceduralEffectHelper.Lerp(original.Blue, sampleB.Blue, mix) +
                             ((sampleB.Blue - original.Blue) * boost);
                float alpha = ProceduralEffectHelper.Lerp(original.Alpha, (sampleR.Alpha + original.Alpha + sampleB.Alpha) / 3f, mix);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(red),
                    ProceduralEffectHelper.ClampToByte(green),
                    ProceduralEffectHelper.ClampToByte(blue),
                    ProceduralEffectHelper.ClampToByte(alpha));
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static float Luminance(SKColor color)
    {
        return ((0.2126f * color.Red) + (0.7152f * color.Green) + (0.0722f * color.Blue)) / 255f;
    }
}