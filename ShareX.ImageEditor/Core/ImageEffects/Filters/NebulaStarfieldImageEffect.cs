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

public sealed class NebulaStarfieldImageEffect : ImageEffectBase
{
    public override string Id => "nebula_starfield";
    public override string Name => "Nebula starfield";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.sparkles;
    public override string Description => "Generates a procedural nebula and starfield overlay.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<NebulaStarfieldImageEffect>("intensity", "Intensity", 0, 100, 70, (e, v) => e.Intensity = v),
        EffectParameters.FloatSlider<NebulaStarfieldImageEffect>("scale", "Scale", 0, 100, 80, (e, v) => e.Scale = v),
        EffectParameters.FloatSlider<NebulaStarfieldImageEffect>("hue_shift", "Hue shift", -180, 180, -15, (e, v) => e.HueShift = v),
        EffectParameters.FloatSlider<NebulaStarfieldImageEffect>("star_density", "Star density", 0, 100, 55, (e, v) => e.StarDensity = v),
        EffectParameters.FloatSlider<NebulaStarfieldImageEffect>("star_size", "Star size", 0, 200, 10, (e, v) => e.StarSize = v),
        EffectParameters.FloatSlider<NebulaStarfieldImageEffect>("twinkle", "Twinkle", 0, 100, 40, (e, v) => e.Twinkle = v),
        EffectParameters.FloatSlider<NebulaStarfieldImageEffect>("vignette_strength", "Vignette strength", 0, 100, 18, (e, v) => e.VignetteStrength = v),
    ];

    public float Intensity { get; set; } = 70f; // 0..100
    public float Scale { get; set; } = 80f; // 0..100
    public float HueShift { get; set; } = -15f; // -180..180
    public float StarDensity { get; set; } = 55f; // 0..100
    public float StarSize { get; set; } = 10f; // 0..200
    public float Twinkle { get; set; } = 40f; // 0..100
    public float VignetteStrength { get; set; } = 18f; // 0..100
    public int Seed { get; set; } = 1337;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float intensity01 = Math.Clamp(Intensity, 0f, 100f) / 100f;
        if (intensity01 <= 0f)
        {
            return source.Copy();
        }

        float scale01 = Math.Clamp(Scale, 0f, 100f) / 100f;
        float hueShift = Math.Clamp(HueShift, -180f, 180f);
        float starDensity01 = Math.Clamp(StarDensity, 0f, 100f) / 100f;
        float starSize01 = Math.Clamp(StarSize, 0f, 200f) / 200f;
        float twinkle01 = Math.Clamp(Twinkle, 0f, 100f) / 100f;
        float vignette01 = Math.Clamp(VignetteStrength, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        // Seeded nebula center.
        float cx = ProceduralEffectHelper.Hash01(Seed, 1, Seed ^ 31) * (width - 1);
        float cy = ProceduralEffectHelper.Hash01(Seed, 2, Seed ^ 47) * (height - 1);

        float maxDist = MathF.Sqrt(((width - 1) * (float)(width - 1)) + ((height - 1) * (float)(height - 1)));
        maxDist = MathF.Max(1f, maxDist);

        // Nebula color from hue shift.
        float baseHue = (hueShift + 180f) * (1f / 360f); // 0..1
        (float nebR, float nebG, float nebB) = HsvToRgb(baseHue, 0.85f, 1f);

        // Star grid resolution (bigger stars => finer grid).
        int cell = Math.Clamp((int)MathF.Round(10f - (starSize01 * 6f)), 3, 16);
        float starThreshold = 1f - starDensity01;
        float radiusBase = 0.18f + (0.55f * starSize01); // normalized radius inside a cell
        float radiusFeather = 0.05f + (0.10f * starSize01);

        float freq = 0.25f + (scale01 * 2.25f);
        float phase = (Seed % 1000) * 0.013f;

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                int idx = row + x;
                SKColor src = srcPixels[idx];

                // Normalized coordinates.
                float nx = (x - cx) / width;
                float ny = (y - cy) / height;
                float dist = MathF.Sqrt((nx * nx) + (ny * ny));

                // Nebula: a few layered waves + radial falloff.
                float u = x / (float)MathF.Max(1, width - 1);
                float v = y / (float)MathF.Max(1, height - 1);

                float waveA = MathF.Sin((u * freq * 8f) + (v * freq * 5f) + phase);
                float waveB = MathF.Sin((u * freq * 13f) - (v * freq * 9f) + (phase * 1.37f));
                float waveC = MathF.Sin(((u + v) * freq * 7f) + (phase * 0.77f));
                float nebulaNoise = (waveA * 0.45f) + (waveB * 0.35f) + (waveC * 0.20f);
                nebulaNoise = (nebulaNoise * 0.5f) + 0.5f;

                float falloff = MathF.Exp(-(dist * dist) * (2.2f + (scale01 * 3.2f)));
                float nebulaStrength = intensity01 * (0.10f + (0.90f * nebulaNoise)) * falloff;

                // Stars: pick a random star center per cell and compute smooth mask around it.
                int cellX = x / cell;
                int cellY = y / cell;
                float cellSpawn = ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 0x3A17);
                float starMask = 0f;

                if (cellSpawn > starThreshold)
                {
                    float oX = ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 0xBEEF);
                    float oY = ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 0xCAFE);
                    float starCenterX = (cellX * cell) + (oX * cell);
                    float starCenterY = (cellY * cell) + (oY * cell);

                    float dx = x - starCenterX;
                    float dy = y - starCenterY;
                    float d01 = MathF.Sqrt((dx * dx) + (dy * dy)) / (cell * 0.5f);

                    float twinkleSeed = ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 0x51A7);
                    float twinkleWave = 0.5f + (0.5f * MathF.Sin((twinkleSeed * MathF.PI * 10f) + phase + (cellX * 0.91f) + (cellY * 1.37f)));
                    float twinkleRadius = ProceduralEffectHelper.Lerp(1f, 0.82f + (twinkleWave * 0.70f), twinkle01);
                    float twinkleIntensity = ProceduralEffectHelper.Lerp(1f, 0.35f + (twinkleWave * 1.65f), twinkle01);

                    d01 /= MathF.Max(0.35f, twinkleRadius);

                    float r = radiusBase;
                    float w = radiusFeather * ProceduralEffectHelper.Lerp(1f, 1.35f, twinkle01);
                    float edge = ProceduralEffectHelper.SmoothStep(r, r + w, d01);
                    starMask = 1f - edge;

                    if (twinkle01 > 0.001f)
                    {
                        float streakScale = 0.08f + (starSize01 * 0.20f);
                        float axisX = MathF.Abs(dx) / MathF.Max(0.0001f, cell * streakScale);
                        float axisY = MathF.Abs(dy) / MathF.Max(0.0001f, cell * streakScale);
                        float streakX = 1f - ProceduralEffectHelper.SmoothStep(0.2f, 1.45f, axisX);
                        float streakY = 1f - ProceduralEffectHelper.SmoothStep(0.2f, 1.45f, axisY);
                        float halo = 1f - ProceduralEffectHelper.SmoothStep(r * 0.7f, r + (1.25f + starSize01), d01);
                        float glint = MathF.Max(streakX, streakY) * halo;

                        starMask = (starMask * twinkleIntensity) + (glint * twinkle01 * MathF.Max(0f, twinkleIntensity - 1f) * 0.45f);
                    }
                }

                float starStrength = intensity01 * starMask * (0.25f + (0.75f * starSize01));

                // Vignette.
                float rFromCenter = MathF.Sqrt(((x - cx) * (x - cx)) + ((y - cy) * (y - cy))) / maxDist;
                float vign = 1f - (vignette01 * rFromCenter * rFromCenter);
                vign = ProceduralEffectHelper.Clamp01(vign);

                // Blend: keep source visible as intensity grows.
                float add = nebulaStrength + starStrength;
                add = MathF.Min(1f, add);

                float remaining = 1f - add;

                float srcR = src.Red / 255f;
                float srcG = src.Green / 255f;
                float srcB = src.Blue / 255f;

                float outR = (srcR * remaining) + (nebR * nebulaStrength) + (nebR * starStrength);
                float outG = (srcG * remaining) + (nebG * nebulaStrength) + (nebG * starStrength);
                float outB = (srcB * remaining) + (nebB * nebulaStrength) + (nebB * starStrength);

                outR *= vign;
                outG *= vign;
                outB *= vign;

                dstPixels[idx] = new SKColor(
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

    private static (float R, float G, float B) HsvToRgb(float h, float s, float v)
    {
        h = h - MathF.Floor(h);
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
            _ => (v, p, q),
        };
    }
}