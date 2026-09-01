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

public sealed class HolographicFoilShimmerImageEffect : ImageEffectBase
{
    public override string Id => "holographic_foil_shimmer";
    public override string Name => "Holographic foil shimmer";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.rainbow;
    public override string Description => "Overlays a rainbow holographic foil shimmer effect.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<HolographicFoilShimmerImageEffect>("intensity", "Intensity", 0, 100, 65, (e, v) => e.Intensity = v),
        EffectParameters.FloatSlider<HolographicFoilShimmerImageEffect>("scale", "Scale", 40, 300, 120, (e, v) => e.Scale = v),
        EffectParameters.FloatSlider<HolographicFoilShimmerImageEffect>("shift", "Shift", 0, 360, 0, (e, v) => e.Shift = v),
        EffectParameters.FloatSlider<HolographicFoilShimmerImageEffect>("specular", "Specular", 0, 100, 45, (e, v) => e.Specular = v),
        EffectParameters.FloatSlider<HolographicFoilShimmerImageEffect>("grain", "Grain", 0, 100, 20, (e, v) => e.Grain = v)
    ];

    public float Intensity { get; set; } = 65f; // 0..100
    public float Scale { get; set; } = 120f; // 40..300
    public float Shift { get; set; } = 0f; // 0..360
    public float Specular { get; set; } = 45f; // 0..100
    public float Grain { get; set; } = 20f; // 0..100
    public int Seed { get; set; } = 2026;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float intensity = Math.Clamp(Intensity, 0f, 100f) / 100f;
        float scale = Math.Clamp(Scale, 40f, 300f);
        float invScale = 1f / scale;
        float phase = Math.Clamp(Shift, 0f, 360f) * (MathF.PI / 180f);
        float specular = Math.Clamp(Specular, 0f, 100f) / 100f;
        float grain = Math.Clamp(Grain, 0f, 100f) / 100f;

        if (intensity <= 0.0001f)
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

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float sr = src.Red / 255f;
                float sg = src.Green / 255f;
                float sb = src.Blue / 255f;
                float sa = src.Alpha;

                float lum = (0.2126f * sr) + (0.7152f * sg) + (0.0722f * sb);
                float u = x * invScale;
                float v = y * invScale;

                float waveA = MathF.Sin((u * 2.7f) + (v * 1.9f) + phase);
                float waveB = MathF.Cos((u * 3.6f) - (v * 2.2f) - (phase * 0.7f));
                float waveC = MathF.Sin(((u + v) * 4.1f) + (phase * 1.3f));
                float wave = (waveA * 0.45f) + (waveB * 0.35f) + (waveC * 0.2f);

                float hue = wave * 0.5f + 0.5f + (u * 0.08f) + (v * 0.04f);
                hue -= MathF.Floor(hue);
                float sat = 0.65f + (0.25f * lum);
                float val = 0.80f + (0.20f * lum);
                (float hr, float hg, float hb) = HsvToRgb(hue, sat, val);

                float scan = 0.5f + (0.5f * MathF.Sin(((u + (v * 0.35f)) * 35f) + (phase * 2f)));
                float shimmer = intensity * ((scan * 0.55f) + 0.45f);

                float specBand = MathF.Sin((u * 8f) - (v * 5f) + (phase * 1.5f));
                specBand = MathF.Pow(MathF.Max(0f, specBand), 8f) * specular;

                float overlayR = (hr * shimmer) + specBand;
                float overlayG = (hg * shimmer) + (specBand * 0.92f);
                float overlayB = (hb * shimmer) + (specBand * 1.08f);

                if (grain > 0.001f)
                {
                    float micro = ProceduralEffectHelper.Hash01(x, y, Seed);
                    float sparkle = MathF.Pow(micro, 22f) * grain * 1.8f;
                    overlayR += sparkle;
                    overlayG += sparkle * 0.95f;
                    overlayB += sparkle * 1.1f;
                }

                float blend = intensity * (0.42f + (0.58f * lum));

                float screenedR = 1f - ((1f - sr) * (1f - MathF.Min(1f, overlayR)));
                float screenedG = 1f - ((1f - sg) * (1f - MathF.Min(1f, overlayG)));
                float screenedB = 1f - ((1f - sb) * (1f - MathF.Min(1f, overlayB)));

                float outR = ProceduralEffectHelper.Lerp(sr, screenedR, blend);
                float outG = ProceduralEffectHelper.Lerp(sg, screenedG, blend);
                float outB = ProceduralEffectHelper.Lerp(sb, screenedB, blend);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(outR * 255f),
                    ProceduralEffectHelper.ClampToByte(outG * 255f),
                    ProceduralEffectHelper.ClampToByte(outB * 255f),
                    ProceduralEffectHelper.ClampToByte(sa));
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static (float R, float G, float B) HsvToRgb(float h, float s, float v)
    {
        if (s <= 0.0001f)
        {
            return (v, v, v);
        }

        float scaled = h * 6f;
        int sector = (int)MathF.Floor(scaled);
        float frac = scaled - sector;

        float p = v * (1f - s);
        float q = v * (1f - (s * frac));
        float t = v * (1f - (s * (1f - frac)));

        return sector switch
        {
            0 => (v, t, p),
            1 => (q, v, p),
            2 => (p, v, t),
            3 => (p, q, v),
            4 => (t, p, v),
            _ => (v, p, q)
        };
    }
}