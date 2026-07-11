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

public sealed class OilSlickInterferenceImageEffect : ImageEffectBase
{
    public override string Id => "oil_slick_interference";
    public override string Name => "Oil slick interference";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.rainbow;
    public override string Description => "Simulates iridescent oil slick interference patterns.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<OilSlickInterferenceImageEffect>("intensity", "Intensity", 0, 100, 58, (e, v) => e.Intensity = v),
        EffectParameters.FloatSlider<OilSlickInterferenceImageEffect>("scale", "Scale", 0, 100, 70, (e, v) => e.Scale = v),
        EffectParameters.FloatSlider<OilSlickInterferenceImageEffect>("darkness", "Darkness", 0, 100, 45, (e, v) => e.Darkness = v),
        EffectParameters.FloatSlider<OilSlickInterferenceImageEffect>("gloss", "Gloss", 0, 100, 60, (e, v) => e.Gloss = v),
        EffectParameters.FloatSlider<OilSlickInterferenceImageEffect>("shift", "Shift", 0, 360, 0, (e, v) => e.Shift = v),
    ];

    public float Intensity { get; set; } = 58f; // 0..100
    public float Scale { get; set; } = 70f; // 0..100
    public float Darkness { get; set; } = 45f; // 0..100
    public float Gloss { get; set; } = 60f; // 0..100
    public float Shift { get; set; } = 0f; // 0..360

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float intensity = Math.Clamp(Intensity, 0f, 100f) / 100f;
        float scale = 24f + (Math.Clamp(Scale, 0f, 100f) / 100f * 180f);
        float invScale = 1f / scale;
        float darkness = Math.Clamp(Darkness, 0f, 100f) / 100f;
        float gloss = Math.Clamp(Gloss, 0f, 100f) / 100f;
        float shift = Math.Clamp(Shift, 0f, 360f) / 360f;

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

                float lum = (0.2126f * sr) + (0.7152f * sg) + (0.0722f * sb);
                float edge = SampleEdge(srcPixels, width, height, x, y);

                float u = x * invScale;
                float v = y * invScale;
                float waveA = MathF.Sin((u * 2.7f) + (v * 1.6f) + 0.7f);
                float waveB = MathF.Cos((u * 4.4f) - (v * 2.8f) - 1.1f);
                float waveC = MathF.Sin(((u + v) * 3.6f) + 1.9f);
                float bands = (waveA * 0.40f) + (waveB * 0.34f) + (waveC * 0.26f);
                bands = (bands * 0.5f) + 0.5f;

                float hue = shift + (bands * 0.32f) + (u * 0.06f) + (v * 0.03f);
                hue -= MathF.Floor(hue);

                float slickMask = intensity * ((1f - lum) * 0.45f + (edge * 0.30f) + (bands * 0.25f));
                slickMask = ProceduralEffectHelper.Clamp01(slickMask);

                (float ir, float ig, float ib) = HsvToRgb(hue, 0.84f, 0.92f + (gloss * 0.08f));

                float darkR = sr * (1f - darkness * 0.58f);
                float darkG = sg * (1f - darkness * 0.60f);
                float darkB = sb * (1f - darkness * 0.66f);

                float glossMask = gloss * ProceduralEffectHelper.SmoothStep(0.58f, 0.96f, bands);
                float overlayR = (ir * slickMask) + (glossMask * 0.24f);
                float overlayG = (ig * slickMask) + (glossMask * 0.20f);
                float overlayB = (ib * slickMask) + (glossMask * 0.28f);

                float outR = Screen(darkR, overlayR);
                float outG = Screen(darkG, overlayG);
                float outB = Screen(darkB, overlayB);

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

    private static float SampleEdge(SKColor[] pixels, int width, int height, int x, int y)
    {
        int left = Math.Max(0, x - 1);
        int right = Math.Min(width - 1, x + 1);
        int top = Math.Max(0, y - 1);
        int bottom = Math.Min(height - 1, y + 1);

        float lumLeft = GetLuminance01(pixels[(y * width) + left]);
        float lumRight = GetLuminance01(pixels[(y * width) + right]);
        float lumTop = GetLuminance01(pixels[(top * width) + x]);
        float lumBottom = GetLuminance01(pixels[(bottom * width) + x]);
        return ProceduralEffectHelper.Clamp01((MathF.Abs(lumRight - lumLeft) + MathF.Abs(lumBottom - lumTop)) * 1.2f);
    }

    private static float GetLuminance01(SKColor color)
    {
        return ((0.2126f * color.Red) + (0.7152f * color.Green) + (0.0722f * color.Blue)) / 255f;
    }

    private static float Screen(float source, float overlay)
    {
        overlay = ProceduralEffectHelper.Clamp01(overlay);
        return 1f - ((1f - source) * (1f - overlay));
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