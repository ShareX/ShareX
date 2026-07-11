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

public sealed class HologramScanImageEffect : ImageEffectBase
{
    public override string Id => "hologram_scan";
    public override string Name => "Hologram scan";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.scan_face;
    public override string Description => "Applies a holographic scanline effect with glitch and glow.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<HologramScanImageEffect>("scanline_strength", "Scanline strength", 0, 100, 45, (e, v) => e.ScanlineStrength = v),
        EffectParameters.FloatSlider<HologramScanImageEffect>("glitch_amount", "Glitch amount", 0, 100, 25, (e, v) => e.GlitchAmount = v),
        EffectParameters.IntSlider<HologramScanImageEffect>("chroma_shift", "Chroma shift", 0, 12, 2, (e, v) => e.ChromaShift = v),
        EffectParameters.FloatSlider<HologramScanImageEffect>("glow_amount", "Glow amount", 0, 100, 30, (e, v) => e.GlowAmount = v)
    ];

    public float ScanlineStrength { get; set; } = 45f; // 0..100
    public float GlitchAmount { get; set; } = 25f; // 0..100
    public int ChromaShift { get; set; } = 2; // 0..12
    public float GlowAmount { get; set; } = 30f; // 0..100
    public int Seed { get; set; } = 6006;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float scanline = Math.Clamp(ScanlineStrength, 0f, 100f) / 100f;
        float glitch = Math.Clamp(GlitchAmount, 0f, 100f) / 100f;
        int chroma = Math.Clamp(ChromaShift, 0, 12);
        float glow = Math.Clamp(GlowAmount, 0f, 100f) / 100f;

        if (scanline <= 0f && glitch <= 0f && chroma <= 0 && glow <= 0f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            int band = y / 6;
            float bandNoise = ProceduralEffectHelper.Hash01(band, 13, Seed);
            float bandSign = (ProceduralEffectHelper.Hash01(band, 31, Seed) * 2f) - 1f;

            float maxShift = glitch * 18f;
            float burst = bandNoise > 0.60f ? ((bandNoise - 0.60f) / 0.40f) : 0f;
            float rowShift = (burst * maxShift * bandSign) +
                             (MathF.Sin((y * 0.11f) + (Seed * 0.001f)) * glitch * 2.5f);

            float lineSine = 0.5f + (0.5f * MathF.Sin(y * MathF.PI));
            float scanlineFactor = 1f - (scanline * (0.30f + (0.70f * lineSine)));

            for (int x = 0; x < width; x++)
            {
                float fx = x + rowShift;

                SKColor sampleR = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, fx + chroma, y);
                SKColor sampleG = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, fx, y);
                SKColor sampleB = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, fx - chroma, y);

                float r = sampleR.Red / 255f;
                float g = sampleG.Green / 255f;
                float b = sampleB.Blue / 255f;

                float luminance = (0.2126f * r) + (0.7152f * g) + (0.0722f * b);

                float hr = (luminance * 0.18f) + (r * 0.20f);
                float hg = (luminance * 0.85f) + (g * 0.35f);
                float hb = (luminance * 1.25f) + (b * 0.45f);

                float pulse = 0.85f + (0.15f * MathF.Sin((y * 0.45f) + (x * 0.03f) + (Seed * 0.0003f)));
                hr *= scanlineFactor * pulse;
                hg *= scanlineFactor * pulse;
                hb *= scanlineFactor * pulse;

                float glowBoost = glow * MathF.Pow(luminance, 2.2f);
                hr += glowBoost * 0.08f;
                hg += glowBoost * 0.25f;
                hb += glowBoost * 0.45f;

                if (bandNoise > 0.92f && ProceduralEffectHelper.Hash01(x, y, Seed ^ 99) > 0.97f)
                {
                    hr += 0.35f;
                    hg += 0.35f;
                    hb += 0.35f;
                }

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(hr * 255f),
                    ProceduralEffectHelper.ClampToByte(hg * 255f),
                    ProceduralEffectHelper.ClampToByte(hb * 255f),
                    sampleG.Alpha);
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }
}