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

using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class AnamorphicLensFlareImageEffect : ImageEffectBase
{
    public override string Id => "anamorphic_lens_flare";
    public override string Name => "Anamorphic lens flare";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.sparkle;
    public override string Description => "Adds cinematic horizontal lens flare streaks to bright areas.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<AnamorphicLensFlareImageEffect>("intensity", "Intensity", 0, 100, 60, (e, v) => e.Intensity = v),
        EffectParameters.FloatSlider<AnamorphicLensFlareImageEffect>("threshold", "Threshold", 0, 100, 72, (e, v) => e.Threshold = v),
        EffectParameters.FloatSlider<AnamorphicLensFlareImageEffect>("streak_length", "Streak length", 0, 100, 55, (e, v) => e.StreakLength = v),
        EffectParameters.FloatSlider<AnamorphicLensFlareImageEffect>("warmth", "Warmth", 0, 100, 45, (e, v) => e.Warmth = v),
        EffectParameters.FloatSlider<AnamorphicLensFlareImageEffect>("ghosting", "Ghosting", 0, 100, 35, (e, v) => e.Ghosting = v)
    ];

    public float Intensity { get; set; } = 60f; // 0..100
    public float Threshold { get; set; } = 72f; // 0..100
    public float StreakLength { get; set; } = 55f; // 0..100
    public float Warmth { get; set; } = 45f; // 0..100
    public float Ghosting { get; set; } = 35f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float intensity = Math.Clamp(Intensity, 0f, 100f) / 100f;
        float threshold = Math.Clamp(Threshold, 0f, 100f) / 100f;
        float streakLength = Math.Clamp(StreakLength, 0f, 100f) / 100f;
        float warmth = Math.Clamp(Warmth, 0f, 100f) / 100f;
        float ghosting = Math.Clamp(Ghosting, 0f, 100f) / 100f;

        if (intensity <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        float centerX = (width - 1) * 0.5f;
        float centerY = (height - 1) * 0.5f;
        float sigma = 0.14f + (streakLength * 0.22f);
        float sigmaInv = 1f / MathF.Max(0.0001f, 2f * sigma * sigma);
        float maxOffset = 10f + (streakLength * 120f);
        int steps = 5 + (int)MathF.Round(streakLength * 4f);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            float yNorm = (y - centerY) / MathF.Max(1f, height - 1);
            float streakProfile = 0.62f + (0.38f * MathF.Exp(-(yNorm * yNorm) * sigmaInv));

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float sr = src.Red / 255f;
                float sg = src.Green / 255f;
                float sb = src.Blue / 255f;

                float flare = 0f;

                for (int i = -steps; i <= steps; i++)
                {
                    float t = i / (float)Math.Max(1, steps);
                    float offset = t * maxOffset;
                    SKColor sample = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, x + offset, y);
                    float lum = GetLuminance(sample) / 255f;
                    float bright = ProceduralEffectHelper.Clamp01((lum - threshold) / MathF.Max(0.0001f, 1f - threshold));
                    float weight = 1f / (1f + (MathF.Abs(i) * 0.7f));
                    flare += bright * weight;
                }

                float ghostX = ((width - 1) - x) + ((x - centerX) * (0.40f + (ghosting * 0.85f)));
                SKColor ghostSample = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, ghostX, y);
                float ghostLum = GetLuminance(ghostSample) / 255f;
                float ghost = ProceduralEffectHelper.Clamp01((ghostLum - (threshold * 0.82f)) / MathF.Max(0.0001f, 1f - (threshold * 0.82f)));
                ghost *= intensity * ghosting * 0.55f * streakProfile;

                float flareStrength = flare * intensity * 0.12f * streakProfile;

                float warmR = 1f;
                float warmG = ProceduralEffectHelper.Lerp(1f, 0.86f, warmth);
                float warmB = ProceduralEffectHelper.Lerp(1f, 0.58f, warmth);

                float ghostR = ProceduralEffectHelper.Lerp(0.88f, 1.0f, warmth);
                float ghostG = 0.82f;
                float ghostB = ProceduralEffectHelper.Lerp(1.0f, 0.74f, warmth);

                float overlayR = (flareStrength * warmR) + (ghost * ghostR);
                float overlayG = (flareStrength * warmG) + (ghost * ghostG);
                float overlayB = (flareStrength * warmB) + (ghost * ghostB);

                float outR = Screen(sr, overlayR);
                float outG = Screen(sg, overlayG);
                float outB = Screen(sb, overlayB);

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

    private static float GetLuminance(SKColor color)
    {
        return (0.2126f * color.Red) + (0.7152f * color.Green) + (0.0722f * color.Blue);
    }

    private static float Screen(float source, float overlay)
    {
        overlay = MathF.Min(1f, MathF.Max(0f, overlay));
        return 1f - ((1f - source) * (1f - overlay));
    }
}