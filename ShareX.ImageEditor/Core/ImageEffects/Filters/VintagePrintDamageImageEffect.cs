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

public sealed class VintagePrintDamageImageEffect : ImageEffectBase
{
    public override string Id => "vintage_print_damage";
    public override string Name => "Vintage print damage";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.newspaper;
    public override string Description => "Simulates aged print damage with sepia toning, grain, scratches, and dust.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<VintagePrintDamageImageEffect>("sepia_amount", "Sepia amount", 0, 100, 65, (e, v) => e.SepiaAmount = v),
        EffectParameters.FloatSlider<VintagePrintDamageImageEffect>("fade_amount", "Fade amount", 0, 100, 30, (e, v) => e.FadeAmount = v),
        EffectParameters.FloatSlider<VintagePrintDamageImageEffect>("grain_amount", "Grain amount", 0, 100, 25, (e, v) => e.GrainAmount = v),
        EffectParameters.FloatSlider<VintagePrintDamageImageEffect>("scratch_amount", "Scratch amount", 0, 100, 25, (e, v) => e.ScratchAmount = v),
        EffectParameters.FloatSlider<VintagePrintDamageImageEffect>("dust_amount", "Dust amount", 0, 100, 20, (e, v) => e.DustAmount = v)
    ];

    public float SepiaAmount { get; set; } = 65f; // 0..100
    public float FadeAmount { get; set; } = 30f; // 0..100
    public float GrainAmount { get; set; } = 25f; // 0..100
    public float ScratchAmount { get; set; } = 25f; // 0..100
    public float DustAmount { get; set; } = 20f; // 0..100
    public int Seed { get; set; } = 1947;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float sepia = Math.Clamp(SepiaAmount, 0f, 100f) / 100f;
        float fade = Math.Clamp(FadeAmount, 0f, 100f) / 100f;
        float grain = Math.Clamp(GrainAmount, 0f, 100f) / 100f;
        float scratches = Math.Clamp(ScratchAmount, 0f, 100f) / 100f;
        float dust = Math.Clamp(DustAmount, 0f, 100f) / 100f;

        if (sepia <= 0f && fade <= 0f && grain <= 0f && scratches <= 0f && dust <= 0f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float cx = (width - 1) * 0.5f;
        float cy = (height - 1) * 0.5f;
        float invCx = cx > 0f ? 1f / cx : 1f;
        float invCy = cy > 0f ? 1f / cy : 1f;
        const float invSqrt2 = 0.70710678f;

        float dustCellSize = MathF.Max(8f, 26f - (dust * 18f));

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            float dyNorm = (y - cy) * invCy;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];

                float r = src.Red / 255f;
                float g = src.Green / 255f;
                float b = src.Blue / 255f;

                float sr = (r * 0.393f) + (g * 0.769f) + (b * 0.189f);
                float sg = (r * 0.349f) + (g * 0.686f) + (b * 0.168f);
                float sb = (r * 0.272f) + (g * 0.534f) + (b * 0.131f);

                r = ProceduralEffectHelper.Lerp(r, sr, sepia);
                g = ProceduralEffectHelper.Lerp(g, sg, sepia);
                b = ProceduralEffectHelper.Lerp(b, sb, sepia);

                float contrast = 1f - (fade * 0.35f);
                float blackLift = fade * 0.045f;
                r = ((r - 0.5f) * contrast) + 0.5f + blackLift;
                g = ((g - 0.5f) * contrast) + 0.5f + blackLift;
                b = ((b - 0.5f) * contrast) + 0.5f + blackLift;

                float paperMix = 0.08f + (fade * 0.10f);
                r = ProceduralEffectHelper.Lerp(r, 0.93f, paperMix);
                g = ProceduralEffectHelper.Lerp(g, 0.86f, paperMix);
                b = ProceduralEffectHelper.Lerp(b, 0.74f, paperMix);

                if (grain > 0f)
                {
                    float n0 = (ProceduralEffectHelper.Hash01(x, y, Seed) * 2f) - 1f;
                    float n1 = (ProceduralEffectHelper.Hash01(x, y, Seed ^ 193) * 2f) - 1f;
                    float n2 = (ProceduralEffectHelper.Hash01(x, y, Seed ^ 761) * 2f) - 1f;
                    float grainAmp = grain * 0.12f;
                    r += n0 * grainAmp;
                    g += n1 * grainAmp;
                    b += n2 * grainAmp;
                }

                if (scratches > 0f)
                {
                    int column = x / 2;
                    float scratchChance = ProceduralEffectHelper.Hash01(column, 0, Seed ^ 71);
                    float threshold = 0.985f - (scratches * 0.12f);
                    if (scratchChance > threshold)
                    {
                        float center = (column * 2f) +
                            ((ProceduralEffectHelper.Hash01(column, 1, Seed ^ 97) - 0.5f) * 2f) +
                            (MathF.Sin((y * 0.03f) + (scratchChance * 12f)) * (0.5f + (scratches * 2f)));

                        float widthPx = 0.5f + (ProceduralEffectHelper.Hash01(column, 2, Seed ^ 131) * 1.8f);
                        float distance = MathF.Abs(x - center) / MathF.Max(widthPx, 0.001f);
                        float lineMask = 1f - ProceduralEffectHelper.SmoothStep(0.1f, 1f, distance);
                        float lineIntensity = lineMask * scratches;

                        bool brighten = ProceduralEffectHelper.Hash01(column, 3, Seed ^ 173) > 0.55f;
                        if (brighten)
                        {
                            r += lineIntensity * 0.25f;
                            g += lineIntensity * 0.23f;
                            b += lineIntensity * 0.20f;
                        }
                        else
                        {
                            r -= lineIntensity * 0.22f;
                            g -= lineIntensity * 0.22f;
                            b -= lineIntensity * 0.22f;
                        }
                    }
                }

                if (dust > 0f)
                {
                    int cellX = (int)(x / dustCellSize);
                    int cellY = (int)(y / dustCellSize);
                    float spotChance = ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 521);
                    float threshold = 0.988f - (dust * 0.23f);
                    if (spotChance > threshold)
                    {
                        float spotX = (cellX + ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 577)) * dustCellSize;
                        float spotY = (cellY + ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 653)) * dustCellSize;
                        float radius = dustCellSize * (0.10f + (0.24f * ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 997)));

                        float dx = x - spotX;
                        float dy = y - spotY;
                        float dist = MathF.Sqrt((dx * dx) + (dy * dy));
                        float radial = dist / MathF.Max(radius, 0.001f);
                        float spot = 1f - ProceduralEffectHelper.SmoothStep(0.15f, 1f, radial);
                        float spotStrength = spot * dust;

                        bool brighten = ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 733) > 0.60f;
                        if (brighten)
                        {
                            r += spotStrength * 0.18f;
                            g += spotStrength * 0.16f;
                            b += spotStrength * 0.14f;
                        }
                        else
                        {
                            r -= spotStrength * 0.20f;
                            g -= spotStrength * 0.19f;
                            b -= spotStrength * 0.18f;
                        }
                    }
                }

                float dxNorm = (x - cx) * invCx;
                float distance01 = MathF.Sqrt((dxNorm * dxNorm) + (dyNorm * dyNorm)) * invSqrt2;
                float edge = ProceduralEffectHelper.SmoothStep(0.62f, 1f, distance01);
                float edgeBurn = edge * (0.10f + (fade * 0.32f) + (scratches * 0.18f));

                r *= 1f - edgeBurn;
                g *= 1f - edgeBurn;
                b *= 1f - edgeBurn;

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r * 255f),
                    ProceduralEffectHelper.ClampToByte(g * 255f),
                    ProceduralEffectHelper.ClampToByte(b * 255f),
                    src.Alpha);
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }
}