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

public sealed class WatercolorBorderImageEffect : ImageEffectBase
{
    public override string Id => "watercolor_border";
    public override string Name => "Watercolor border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.droplets;
    public override string Description => "Adds a soft watercolor wash border with organic, painted edges that bleed into the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<WatercolorBorderImageEffect>("border_size", "Border size", 10, 300, 60, (e, v) => e.BorderSize = v),
        EffectParameters.Color<WatercolorBorderImageEffect>("wash_color", "Wash color", new SKColor(185, 160, 130), (e, v) => e.WashColor = v),
        EffectParameters.FloatSlider<WatercolorBorderImageEffect>("edge_roughness", "Edge roughness", 0, 100, 65, (e, v) => e.EdgeRoughness = v),
        EffectParameters.FloatSlider<WatercolorBorderImageEffect>("bleed_amount", "Bleed amount", 0, 100, 40, (e, v) => e.BleedAmount = v),
        EffectParameters.FloatSlider<WatercolorBorderImageEffect>("opacity", "Opacity", 10, 100, 90, (e, v) => e.Opacity = v)
    ];

    public int BorderSize { get; set; } = 60;
    public SKColor WashColor { get; set; } = new SKColor(185, 160, 130);
    public float EdgeRoughness { get; set; } = 65f;
    public float BleedAmount { get; set; } = 40f;
    public float Opacity { get; set; } = 90f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 10, 300);
        float roughness = Math.Clamp(EdgeRoughness, 0f, 100f) / 100f;
        float bleed = Math.Clamp(BleedAmount, 0f, 100f) / 100f;
        float opacity = Math.Clamp(Opacity, 10f, 100f) / 100f;

        // Bleed makes the image area smaller (wash bleeds in from edges)
        int bleedPx = (int)(border * bleed * 0.5f);
        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        float wr = WashColor.Red / 255f;
        float wg = WashColor.Green / 255f;
        float wb = WashColor.Blue / 255f;

        SKColor[] dstPixels = new SKColor[newWidth * newHeight];

        // Precompute source pixels for blending in the bleed zone
        SKColor[] srcPixels = source.Pixels;

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                int srcX = x - border;
                int srcY = y - border;

                // Distance from image inner edges (positive = inside frame+bleed zone)
                float distL = border - x + bleedPx;
                float distT = border - y + bleedPx;
                float distR = (x - (border + source.Width - 1 - bleedPx));
                float distB = (y - (border + source.Height - 1 - bleedPx));

                // Maximum distance from any inner edge
                float maxDist = Math.Max(Math.Max(distL, distT), Math.Max(distR, distB));

                if (maxDist <= 0f)
                {
                    // Pure source pixel zone
                    dstPixels[(y * newWidth) + x] = srcPixels[(srcY * source.Width) + srcX];
                    continue;
                }

                // Normalize distance across the full border zone
                float crossRatio = Math.Clamp(maxDist / (border + bleedPx), 0f, 1f);

                // Add organic noise to the transition edge
                float noiseScale = 0.015f;
                float noise = ProceduralEffectHelper.FractalNoise(x * noiseScale, y * noiseScale, 4, 2.0f, 0.55f, 37);
                float roughOffset = roughness * (noise * 2f - 1f) * 0.35f;
                float t = Math.Clamp(crossRatio + roughOffset, 0f, 1f);

                // Watercolor falloff: smooth, with slightly abrupt outer edge
                float washAlpha = ProceduralEffectHelper.SmoothStep(0f, 0.7f, t) * opacity;

                // Secondary texture: subtle paper-fiber noise
                float paperNoise = ProceduralEffectHelper.FractalNoise(x * 0.04f, y * 0.04f, 3, 2.0f, 0.4f, 99);
                washAlpha *= (0.85f + paperNoise * 0.3f);
                washAlpha = Math.Clamp(washAlpha, 0f, 1f);

                if (washAlpha >= 0.999f)
                {
                    // Fully opaque wash, no source pixel
                    float paperShade = paperNoise * 0.12f - 0.06f;
                    dstPixels[(y * newWidth) + x] = new SKColor(
                        ProceduralEffectHelper.ClampToByte((wr + paperShade) * 255f),
                        ProceduralEffectHelper.ClampToByte((wg + paperShade) * 255f),
                        ProceduralEffectHelper.ClampToByte((wb + paperShade) * 255f),
                        255);
                }
                else if (srcX >= 0 && srcX < source.Width && srcY >= 0 && srcY < source.Height)
                {
                    // Blend wash over source
                    SKColor srcCol = srcPixels[(srcY * source.Width) + srcX];
                    float srcAlpha = srcCol.Alpha / 255f * (1f - washAlpha);

                    float paperShade = paperNoise * 0.10f - 0.05f;
                    float outR = ProceduralEffectHelper.Lerp(srcCol.Red / 255f, wr + paperShade, washAlpha);
                    float outG = ProceduralEffectHelper.Lerp(srcCol.Green / 255f, wg + paperShade, washAlpha);
                    float outB = ProceduralEffectHelper.Lerp(srcCol.Blue / 255f, wb + paperShade, washAlpha);
                    float outA = srcAlpha + washAlpha;

                    dstPixels[(y * newWidth) + x] = new SKColor(
                        ProceduralEffectHelper.ClampToByte(outR * 255f),
                        ProceduralEffectHelper.ClampToByte(outG * 255f),
                        ProceduralEffectHelper.ClampToByte(outB * 255f),
                        ProceduralEffectHelper.ClampToByte(outA * 255f));
                }
                else
                {
                    // Outside source area — pure wash
                    float paperShade = paperNoise * 0.12f - 0.06f;
                    dstPixels[(y * newWidth) + x] = new SKColor(
                        ProceduralEffectHelper.ClampToByte((wr + paperShade) * 255f),
                        ProceduralEffectHelper.ClampToByte((wg + paperShade) * 255f),
                        ProceduralEffectHelper.ClampToByte((wb + paperShade) * 255f),
                        ProceduralEffectHelper.ClampToByte(washAlpha * 255f));
                }
            }
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        return result;
    }
}