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

public sealed class VhsTapeDamageImageEffect : ImageEffectBase
{
    public override string Id => "vhs_tape_damage";
    public override string Name => "VHS tape damage";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.videotape;
    public override string Description => "Simulates VHS tape degradation with distortion, noise, and color bleeding.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<VhsTapeDamageImageEffect>("distortion", "Distortion", 0, 100, 55, (e, v) => e.Distortion = v),
        EffectParameters.FloatSlider<VhsTapeDamageImageEffect>("noise", "Noise", 0, 100, 28, (e, v) => e.Noise = v),
        EffectParameters.FloatSlider<VhsTapeDamageImageEffect>("color_bleed", "Color bleed", 0, 100, 30, (e, v) => e.ColorBleed = v),
        EffectParameters.FloatSlider<VhsTapeDamageImageEffect>("tracking", "Tracking", 0, 100, 24, (e, v) => e.Tracking = v),
        EffectParameters.FloatSlider<VhsTapeDamageImageEffect>("scanlines", "Scanlines", 0, 100, 48, (e, v) => e.Scanlines = v)
    ];

    public float Distortion { get; set; } = 55f; // 0..100
    public float Noise { get; set; } = 28f; // 0..100
    public float ColorBleed { get; set; } = 30f; // 0..100
    public float Tracking { get; set; } = 24f; // 0..100
    public float Scanlines { get; set; } = 48f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float distortion = Math.Clamp(Distortion, 0f, 100f) / 100f;
        float noise = Math.Clamp(Noise, 0f, 100f) / 100f;
        float colorBleed = Math.Clamp(ColorBleed, 0f, 100f) / 100f;
        float tracking = Math.Clamp(Tracking, 0f, 100f) / 100f;
        float scanlines = Math.Clamp(Scanlines, 0f, 100f) / 100f;

        if (distortion <= 0.0001f && noise <= 0.0001f && colorBleed <= 0.0001f && tracking <= 0.0001f && scanlines <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            float rowNoise = (ProceduralEffectHelper.Hash01(y, 0, 1703) * 2f) - 1f;
            float shift = (MathF.Sin((y * 0.085f) + 0.7f) * 0.55f + MathF.Cos((y * 0.024f) - 1.6f) * 0.45f + (rowNoise * 0.65f))
                * distortion
                * 10f;

            float dropoutMask = 0f;
            if (ProceduralEffectHelper.Hash01(y / 3, 4, 4201) > 0.987f)
            {
                dropoutMask = 0.35f + (tracking * 0.45f);
            }

            float trackingWave = MathF.Pow(MathF.Max(0f, MathF.Sin((y * 0.022f) + 1.1f)), 10f) * tracking;
            float scanMask = 1f - (scanlines * 0.18f * (0.5f + (0.5f * MathF.Sin(y * MathF.PI))));

            for (int x = 0; x < width; x++)
            {
                float bleedPx = 0.5f + (colorBleed * 4.5f);
                float sx = x + shift;

                SKColor rSample = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sx + bleedPx, y);
                SKColor gSample = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sx, y);
                SKColor bSample = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sx - bleedPx, y);

                float r = rSample.Red;
                float g = gSample.Green;
                float b = bSample.Blue;
                float a = gSample.Alpha;

                float lineNoise = ((ProceduralEffectHelper.Hash01(x, y, 6113) * 2f) - 1f) * noise * 28f;
                r += lineNoise * 1.05f;
                g += lineNoise;
                b += lineNoise * 0.95f;

                if (trackingWave > 0.0001f)
                {
                    float trackingLift = trackingWave * 42f;
                    r = ProceduralEffectHelper.Lerp(r, 255f, trackingWave * 0.18f);
                    g += trackingLift * 0.22f;
                    b += trackingLift * 0.30f;
                }

                if (dropoutMask > 0.0001f)
                {
                    float dropoutNoise = ProceduralEffectHelper.Hash01(x / 4, y, 7001);
                    if (dropoutNoise > 0.35f)
                    {
                        r *= 1f - dropoutMask;
                        g *= 1f - (dropoutMask * 0.95f);
                        b *= 1f - (dropoutMask * 0.90f);
                    }
                }

                r *= scanMask;
                g *= scanMask;
                b *= scanMask;

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