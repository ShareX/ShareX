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

public sealed class OldCameraFlashBurnImageEffect : ImageEffectBase
{
    public override string Id => "old_camera_flash_burn";
    public override string Name => "Old camera flash burn";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.flashlight;
    public override string Description => "Simulates an old camera flash burn with warm glow and edge burn effects.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<OldCameraFlashBurnImageEffect>("flash_strength", "Flash strength", 0f, 100f, 70f, (e, v) => e.FlashStrength = v),
        EffectParameters.FloatSlider<OldCameraFlashBurnImageEffect>("flash_radius", "Flash radius", 20f, 100f, 68f, (e, v) => e.FlashRadius = v),
        EffectParameters.FloatSlider<OldCameraFlashBurnImageEffect>("edge_burn", "Edge burn", 0f, 100f, 45f, (e, v) => e.EdgeBurn = v),
        EffectParameters.FloatSlider<OldCameraFlashBurnImageEffect>("warmth", "Warmth", 0f, 100f, 35f, (e, v) => e.Warmth = v),
        EffectParameters.FloatSlider<OldCameraFlashBurnImageEffect>("grain", "Grain", 0f, 100f, 20f, (e, v) => e.Grain = v),
        EffectParameters.FloatSlider<OldCameraFlashBurnImageEffect>("center_x", "Center X", 0f, 100f, 50f, (e, v) => e.CenterX = v),
        EffectParameters.FloatSlider<OldCameraFlashBurnImageEffect>("center_y", "Center Y", 0f, 100f, 50f, (e, v) => e.CenterY = v),
    ];

    public float FlashStrength { get; set; } = 70f; // 0..100
    public float FlashRadius { get; set; } = 68f; // 20..100
    public float EdgeBurn { get; set; } = 45f; // 0..100
    public float Warmth { get; set; } = 35f; // 0..100
    public float Grain { get; set; } = 20f; // 0..100
    public float CenterX { get; set; } = 50f; // 0..100
    public float CenterY { get; set; } = 50f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float flashStrength = Math.Clamp(FlashStrength, 0f, 100f) / 100f;
        float flashRadius = Math.Clamp(FlashRadius, 20f, 100f) / 100f;
        float edgeBurn = Math.Clamp(EdgeBurn, 0f, 100f) / 100f;
        float warmth = Math.Clamp(Warmth, 0f, 100f);
        float grain = Math.Clamp(Grain, 0f, 100f) / 100f;

        if (flashStrength <= 0.0001f && edgeBurn <= 0.0001f && grain <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        int right = Math.Max(1, width - 1);
        int bottom = Math.Max(1, height - 1);

        float cx = (Math.Clamp(CenterX, 0f, 100f) / 100f) * right;
        float cy = (Math.Clamp(CenterY, 0f, 100f) / 100f) * bottom;
        float invWidth = 1f / right;
        float invHeight = 1f / bottom;
        float sigma = 0.06f + (flashRadius * 0.38f);
        float sigmaInv = 1f / Math.Max(0.0001f, 2f * sigma * sigma);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float r = src.Red;
                float g = src.Green;
                float b = src.Blue;
                float a = src.Alpha;

                float dx = (x - cx) * invWidth;
                float dy = (y - cy) * invHeight;
                float dist = MathF.Sqrt((dx * dx) + (dy * dy));

                float flash = flashStrength * MathF.Exp(-(dist * dist) * sigmaInv);
                if (flash > 0.0001f)
                {
                    float shoulder = MathF.Pow(flash, 0.72f);
                    float warmR = 255f;
                    float warmG = 242f + (warmth * 0.10f);
                    float warmB = 218f - (warmth * 0.08f);

                    r = ProceduralEffectHelper.Lerp(r, warmR, shoulder);
                    g = ProceduralEffectHelper.Lerp(g, warmG, shoulder * 0.94f);
                    b = ProceduralEffectHelper.Lerp(b, warmB, shoulder * 0.82f);
                }

                if (edgeBurn > 0.0001f)
                {
                    float burnRing = ProceduralEffectHelper.SmoothStep(0.54f, 1.05f, dist);
                    if (burnRing > 0.0001f)
                    {
                        float burnNoiseA = ProceduralEffectHelper.Hash01((int)(x * 0.12f), (int)(y * 0.12f), 611);
                        float burnNoiseB = ProceduralEffectHelper.Hash01((int)(x * 0.032f), (int)(y * 0.032f), 1201);
                        float burn = burnRing * edgeBurn * (0.68f + (0.32f * ((burnNoiseA * 0.6f) + (burnNoiseB * 0.4f))));

                        float burnR = 220f + (warmth * 0.22f);
                        float burnG = 98f + (warmth * 0.18f);
                        float burnB = 36f + (warmth * 0.12f);
                        r = ProceduralEffectHelper.Lerp(r, burnR, burn * 0.44f);
                        g = ProceduralEffectHelper.Lerp(g, burnG, burn * 0.38f);
                        b = ProceduralEffectHelper.Lerp(b, burnB, burn * 0.30f);

                        float dark = burn * 0.45f;
                        r *= 1f - (dark * 0.60f);
                        g *= 1f - (dark * 0.68f);
                        b *= 1f - (dark * 0.78f);
                    }
                }

                if (grain > 0.0001f)
                {
                    float noise = ((ProceduralEffectHelper.Hash01(x, y, 1703) * 2f) - 1f) * grain * 18f;
                    r += noise * 1.05f;
                    g += noise;
                    b += noise * 0.9f;
                }

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r),
                    ProceduralEffectHelper.ClampToByte(g),
                    ProceduralEffectHelper.ClampToByte(b),
                    ProceduralEffectHelper.ClampToByte(a));
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }
}