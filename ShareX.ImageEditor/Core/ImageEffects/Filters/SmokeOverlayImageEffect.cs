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

public sealed class SmokeOverlayImageEffect : ImageEffectBase
{
    public override string Id => "smoke_overlay";
    public override string Name => "Smoke overlay";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.haze;
    public override string Description => "Adds a soft procedural smoke overlay to the image.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<SmokeOverlayImageEffect>("density", "Density", 0f, 100f, 42f, (e, v) => e.Density = v),
        EffectParameters.FloatSlider<SmokeOverlayImageEffect>("scale", "Scale", 0f, 100f, 68f, (e, v) => e.Scale = v),
        EffectParameters.FloatSlider<SmokeOverlayImageEffect>("drift", "Drift", -100f, 100f, 18f, (e, v) => e.Drift = v),
        EffectParameters.FloatSlider<SmokeOverlayImageEffect>("softness", "Softness", 0f, 100f, 55f, (e, v) => e.Softness = v),
        EffectParameters.FloatSlider<SmokeOverlayImageEffect>("contrast", "Contrast", 0f, 100f, 48f, (e, v) => e.Contrast = v),
    ];

    public float Density { get; set; } = 42f; // 0..100
    public float Scale { get; set; } = 68f; // 0..100
    public float Drift { get; set; } = 18f; // -100..100
    public float Softness { get; set; } = 55f; // 0..100
    public float Contrast { get; set; } = 48f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float density = Math.Clamp(Density, 0f, 100f) / 100f;
        float scale = Math.Clamp(Scale, 0f, 100f) / 100f;
        float drift = Math.Clamp(Drift, -100f, 100f) / 100f;
        float softness = Math.Clamp(Softness, 0f, 100f) / 100f;
        float contrast = Math.Clamp(Contrast, 0f, 100f) / 100f;

        if (density <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float coarseFreq = 0.012f + ((1f - scale) * 0.035f);
        float mediumFreq = coarseFreq * 2.2f;
        float fineFreq = coarseFreq * 4.4f;

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            float driftOffset = drift * y * 0.014f;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float sr = src.Red / 255f;
                float sg = src.Green / 255f;
                float sb = src.Blue / 255f;

                float px = x + driftOffset;
                float noiseX = px * coarseFreq;
                float noiseY = y * coarseFreq;

                float warpX = ((ProceduralEffectHelper.FractalNoise((noiseX * 0.85f) + 7.2f, (noiseY * 0.85f) - 3.1f, 3, 2.05f, 0.56f, 911) * 2f) - 1f) * (0.75f + (scale * 1.45f));
                float warpY = ((ProceduralEffectHelper.FractalNoise((noiseX * 0.90f) - 5.4f, (noiseY * 0.90f) + 11.8f, 3, 2.00f, 0.56f, 1201) * 2f) - 1f) * (0.60f + (scale * 1.15f));

                float sampleX = noiseX + warpX;
                float sampleY = noiseY + warpY;

                float coarse = ProceduralEffectHelper.FractalNoise(sampleX, sampleY, 4, 2.00f, 0.55f, 1559);
                float medium = ProceduralEffectHelper.FractalNoise((sampleX * (mediumFreq / coarseFreq)) + 13.1f, (sampleY * (mediumFreq / coarseFreq)) - 7.6f, 3, 2.10f, 0.52f, 2081);
                float fine = ProceduralEffectHelper.FractalNoise((sampleX * (fineFreq / coarseFreq)) - 9.4f, (sampleY * (fineFreq / coarseFreq)) + 4.7f, 2, 2.20f, 0.58f, 2593);

                float flowA = MathF.Sin((x * coarseFreq * 16f) + (y * coarseFreq * 6f) + 0.7f);
                float flowB = MathF.Cos((x * coarseFreq * 9f) - (y * coarseFreq * 13f) - 1.4f);

                float wisps = 1f - MathF.Abs((medium * 2f) - 1f);
                float smoke = (coarse * 0.58f) + (wisps * 0.24f) + (fine * 0.18f);
                smoke = ProceduralEffectHelper.Clamp01(smoke + flowA * 0.08f + flowB * 0.06f);
                smoke = ProceduralEffectHelper.Clamp01(((smoke - 0.5f) * (0.92f + (contrast * 1.5f))) + 0.5f);

                float threshold = 0.44f - (density * 0.16f);
                float alpha = ProceduralEffectHelper.SmoothStep(threshold, 0.96f, smoke);
                alpha = MathF.Pow(alpha, 1.55f - (softness * 1.10f)) * density * 0.68f;

                float smokeR = 0.82f + (smoke * 0.10f);
                float smokeG = 0.84f + (smoke * 0.10f);
                float smokeB = 0.86f + (smoke * 0.12f);

                float outR = ProceduralEffectHelper.Lerp(sr, smokeR, alpha);
                float outG = ProceduralEffectHelper.Lerp(sg, smokeG, alpha);
                float outB = ProceduralEffectHelper.Lerp(sb, smokeB, alpha);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(outR * 255f),
                    ProceduralEffectHelper.ClampToByte(outG * 255f),
                    ProceduralEffectHelper.ClampToByte(outB * 255f),
                    src.Alpha);
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }
}