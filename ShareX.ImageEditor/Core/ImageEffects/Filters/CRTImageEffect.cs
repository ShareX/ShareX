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
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class CRTImageEffect : ImageEffectBase
{
    public override string Id => "crt";
    public override string Name => "CRT";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.monitor;
    public override string Description => "Simulates a CRT monitor with scanlines, RGB shift, noise, and vignette.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<CRTImageEffect>("scanlineStrength", "Scanline strength", 0f, 100f, 35f, (e, v) => e.ScanlineStrength = v),
        EffectParameters.IntSlider<CRTImageEffect>("rgbShift", "RGB shift", 0, 8, 1, (e, v) => e.RGBShift = v),
        EffectParameters.FloatSlider<CRTImageEffect>("noiseAmount", "Noise amount", 0f, 100f, 8f, (e, v) => e.NoiseAmount = v),
        EffectParameters.FloatSlider<CRTImageEffect>("vignetteStrength", "Vignette strength", 0f, 100f, 18f, (e, v) => e.VignetteStrength = v),
    ];

    public float ScanlineStrength { get; set; } = 35f; // 0..100
    public int RGBShift { get; set; } = 1; // 0..8 px
    public float NoiseAmount { get; set; } = 8f; // 0..100
    public float VignetteStrength { get; set; } = 18f; // 0..100
    public int Seed { get; set; } = 1337;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float scanline = Math.Clamp(ScanlineStrength, 0f, 100f) / 100f;
        int shift = Math.Clamp(RGBShift, 0, 8);
        float noise = Math.Clamp(NoiseAmount, 0f, 100f);
        float vignette = Math.Clamp(VignetteStrength, 0f, 100f) / 100f;

        if (scanline <= 0f && shift == 0 && noise <= 0f && vignette <= 0f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int noiseAmplitude = (int)MathF.Round(noise * 0.6f); // 0..60

        float cx = (width - 1) * 0.5f;
        float cy = (height - 1) * 0.5f;
        float invCx = cx > 0f ? 1f / cx : 1f;
        float invCy = cy > 0f ? 1f / cy : 1f;
        const float invSqrt2 = 0.70710678f;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];
        uint seed = unchecked((uint)Seed * 747796405u);

        for (int y = 0; y < height; y++)
        {
            int row = y * width;

            float lineFactor = 1f;
            if (scanline > 0f)
            {
                // Alternate row attenuation with mild bias, emulating CRT scanlines.
                float rowDarkness = (y & 1) == 0 ? 0.62f : 0.18f;
                lineFactor = 1f - (scanline * rowDarkness);
            }

            float dy = (y - cy) * invCy;

            for (int x = 0; x < width; x++)
            {
                int idx = row + x;

                int xr = Clamp(x + shift, 0, right);
                int xb = Clamp(x - shift, 0, right);

                SKColor sampleR = srcPixels[row + xr];
                SKColor sampleG = srcPixels[idx];
                SKColor sampleB = srcPixels[row + xb];

                // Simple shadow-mask tinting by triads.
                float maskR = 1f;
                float maskG = 1f;
                float maskB = 1f;
                switch (x % 3)
                {
                    case 0:
                        maskR = 1.06f;
                        maskG = 0.97f;
                        maskB = 0.93f;
                        break;
                    case 1:
                        maskR = 0.93f;
                        maskG = 1.06f;
                        maskB = 0.97f;
                        break;
                    default:
                        maskR = 0.97f;
                        maskG = 0.93f;
                        maskB = 1.06f;
                        break;
                }

                float vignetteFactor = 1f;
                if (vignette > 0f)
                {
                    float dx = (x - cx) * invCx;
                    float distance01 = MathF.Sqrt((dx * dx) + (dy * dy)) * invSqrt2;
                    float t = (distance01 - 0.55f) / 0.45f;
                    t = Math.Clamp(t, 0f, 1f);
                    float smooth = t * t * (3f - (2f * t));
                    vignetteFactor = 1f - (vignette * smooth);
                }

                float brightness = lineFactor * vignetteFactor;

                int nr = 0;
                int ng = 0;
                int nb = 0;
                if (noiseAmplitude > 0)
                {
                    uint h = Hash((uint)idx ^ seed);
                    nr = (((int)(h & 0xFF)) - 128) * noiseAmplitude / 128;
                    h = Hash(h ^ 0x9E3779B9u);
                    ng = (((int)(h & 0xFF)) - 128) * noiseAmplitude / 128;
                    h = Hash(h ^ 0x85EBCA6Bu);
                    nb = (((int)(h & 0xFF)) - 128) * noiseAmplitude / 128;
                }

                int r = (int)MathF.Round(sampleR.Red * maskR * brightness) + nr;
                int g = (int)MathF.Round(sampleG.Green * maskG * brightness) + ng;
                int b = (int)MathF.Round(sampleB.Blue * maskB * brightness) + nb;

                dstPixels[idx] = new SKColor(
                    ClampToByte(r),
                    ClampToByte(g),
                    ClampToByte(b),
                    sampleG.Alpha);
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static uint Hash(uint x)
    {
        x ^= x >> 16;
        x *= 0x7FEB352Du;
        x ^= x >> 15;
        x *= 0x846CA68Bu;
        x ^= x >> 16;
        return x;
    }

    private static byte ClampToByte(int value)
    {
        if (value <= 0) return 0;
        if (value >= 255) return 255;
        return (byte)value;
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}