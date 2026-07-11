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

public sealed class CloudsImageEffect : ImageEffectBase
{
    public override string Id => "clouds";
    public override string Name => "Clouds";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.cloud;
    public override string Description => "Overlays procedurally generated clouds onto the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<CloudsImageEffect>("coverage", "Coverage", 0f, 100f, 55f, (e, v) => e.Coverage = v),
        EffectParameters.FloatSlider<CloudsImageEffect>("scale", "Scale", 0f, 100f, 72f, (e, v) => e.Scale = v),
        EffectParameters.FloatSlider<CloudsImageEffect>("heightBias", "Height bias", 0f, 100f, 62f, (e, v) => e.HeightBias = v),
        EffectParameters.FloatSlider<CloudsImageEffect>("softness", "Softness", 0f, 100f, 64f, (e, v) => e.Softness = v),
        EffectParameters.FloatSlider<CloudsImageEffect>("sunlight", "Sunlight", 0f, 100f, 38f, (e, v) => e.Sunlight = v),
    ];

    public float Coverage { get; set; } = 55f; // 0..100
    public float Scale { get; set; } = 72f; // 0..100
    public float HeightBias { get; set; } = 62f; // 0..100
    public float Softness { get; set; } = 64f; // 0..100
    public float Sunlight { get; set; } = 38f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float coverage = Math.Clamp(Coverage, 0f, 100f) / 100f;
        float scale = Math.Clamp(Scale, 0f, 100f) / 100f;
        float heightBias = Math.Clamp(HeightBias, 0f, 100f) / 100f;
        float softness = Math.Clamp(Softness, 0f, 100f) / 100f;
        float sunlight = Math.Clamp(Sunlight, 0f, 100f) / 100f;

        if (coverage <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float cellSize = 36f + (scale * 140f);
        float detailCellSize = MathF.Max(18f, cellSize * 0.52f);
        float warpFreq = 0.0045f + ((1f - scale) * 0.012f);
        float warpAmount = 10f + (scale * 40f);
        float erosionFreq = 0.010f + ((1f - scale) * 0.020f);
        float spawnThreshold = 0.82f - (coverage * 0.45f);
        float invWidth = width > 1 ? 1f / (width - 1f) : 0f;
        float invHeight = height > 1 ? 1f / (height - 1f) : 0f;

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            float v = y * invHeight;
            float bandStart = 0.20f + (heightBias * 0.55f);
            float verticalMask = 1f - ProceduralEffectHelper.SmoothStep(bandStart, 1f, v);

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float sr = src.Red / 255f;
                float sg = src.Green / 255f;
                float sb = src.Blue / 255f;
                float u = x * invWidth;
                float warpX = ((ProceduralEffectHelper.FractalNoise((x * warpFreq) + 4.7f, (y * warpFreq * 0.92f) - 2.1f, 3, 2.02f, 0.56f, 1411) * 2f) - 1f) * warpAmount;
                float warpY = ((ProceduralEffectHelper.FractalNoise((x * warpFreq * 0.88f) - 6.4f, (y * warpFreq) + 3.6f, 3, 2.10f, 0.54f, 2311) * 2f) - 1f) * (warpAmount * 0.65f);

                float sampleX = x + warpX;
                float sampleY = y + warpY;

                SampleCloudLayer(sampleX, sampleY, cellSize, spawnThreshold, 3571, out float macroField, out float macroLight, out float macroShadow);
                SampleCloudLayer(sampleX + (cellSize * 0.24f), sampleY - (cellSize * 0.15f), detailCellSize, spawnThreshold + 0.08f, 5171, out float detailField, out float detailLight, out float detailShadow);

                float field = 1f - ((1f - macroField) * (1f - (detailField * 0.72f)));
                float breakup = SampleBillow((sampleX * erosionFreq) - 3.2f, (sampleY * erosionFreq) + 5.9f, 3, 2.00f, 0.56f, 6211);
                float curls = ProceduralEffectHelper.FractalNoise((sampleX * erosionFreq * 2.2f) + 11.3f, (sampleY * erosionFreq * 1.9f) - 7.6f, 2, 2.12f, 0.58f, 7351);
                float bankWave = 0.5f + (0.5f * MathF.Sin((u * 5.6f) + (v * 1.7f) + 0.8f));

                float structure = field * (0.72f + (0.22f * breakup) + (0.10f * curls));
                structure = ProceduralEffectHelper.Clamp01(structure + (bankWave * 0.07f));

                float threshold = 0.30f - (coverage * 0.18f) - (verticalMask * 0.08f);
                float mask = ProceduralEffectHelper.SmoothStep(threshold, 0.84f, structure * (0.70f + (0.30f * verticalMask)));
                mask = MathF.Pow(mask, 1.45f - (softness * 0.95f));

                float cloudLight = MathF.Max(macroLight, detailLight * 0.88f);
                float cloudShadow = MathF.Max(macroShadow, detailShadow * 0.92f);
                float sunlightBias = ProceduralEffectHelper.Clamp01(0.68f + (((0.78f - u) * 0.42f) + ((0.40f - v) * 0.74f)));
                float highlight = sunlight * mask * (0.18f + (0.34f * cloudLight) + (0.18f * sunlightBias));
                float shade = mask * ((0.08f + (0.22f * cloudShadow)) * (0.90f - (sunlight * 0.20f)));
                float body = 0.79f + (0.12f * structure) + (0.05f * breakup);

                float cloudR = ProceduralEffectHelper.Clamp01(body - shade + (highlight * 1.06f));
                float cloudG = ProceduralEffectHelper.Clamp01((body + 0.03f) - (shade * 0.94f) + highlight);
                float cloudB = ProceduralEffectHelper.Clamp01((body + 0.08f) - (shade * 0.84f) + (highlight * 0.82f));

                float ambientHaze = coverage * verticalMask * (0.006f + (0.016f * structure));
                float hazeR = ProceduralEffectHelper.Lerp(sr, 0.84f, ambientHaze);
                float hazeG = ProceduralEffectHelper.Lerp(sg, 0.89f, ambientHaze);
                float hazeB = ProceduralEffectHelper.Lerp(sb, 0.95f, ambientHaze);

                float blend = mask * (0.52f + (coverage * 0.34f));
                float outR = ProceduralEffectHelper.Lerp(hazeR, cloudR, blend);
                float outG = ProceduralEffectHelper.Lerp(hazeG, cloudG, blend);
                float outB = ProceduralEffectHelper.Lerp(hazeB, cloudB, blend);

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

    private static void SampleCloudLayer(float x, float y, float cellSize, float spawnThreshold, int seed, out float field, out float light, out float shadow)
    {
        field = 0f;
        light = 0f;
        shadow = 0f;

        int cellX = (int)MathF.Floor(x / cellSize);
        int cellY = (int)MathF.Floor(y / cellSize);

        for (int oy = -1; oy <= 1; oy++)
        {
            for (int ox = -1; ox <= 1; ox++)
            {
                int nx = cellX + ox;
                int ny = cellY + oy;

                float spawn = ProceduralEffectHelper.Hash01(nx, ny, seed);
                if (spawn < spawnThreshold)
                {
                    continue;
                }

                float centerX = (nx + 0.18f + (ProceduralEffectHelper.Hash01(nx, ny, seed ^ 0x0713) * 0.64f)) * cellSize;
                float centerY = (ny + 0.22f + (ProceduralEffectHelper.Hash01(nx, ny, seed ^ 0x1731) * 0.52f)) * cellSize;
                float radius = cellSize * (0.28f + (ProceduralEffectHelper.Hash01(nx, ny, seed ^ 0x2A51) * 0.20f));

                AccumulatePuff(x, y, centerX, centerY, radius, ref field, ref light, ref shadow);

                float satelliteChance = ProceduralEffectHelper.Hash01(nx, ny, seed ^ 0x3C17);
                if (satelliteChance > 0.32f)
                {
                    float angle = ProceduralEffectHelper.Hash01(nx, ny, seed ^ 0x49D3) * (MathF.PI * 2f);
                    float offset = radius * (0.35f + (ProceduralEffectHelper.Hash01(nx, ny, seed ^ 0x5123) * 0.40f));
                    float satelliteRadius = radius * (0.42f + (ProceduralEffectHelper.Hash01(nx, ny, seed ^ 0x6891) * 0.28f));
                    float satelliteX = centerX + (MathF.Cos(angle) * offset);
                    float satelliteY = centerY + (MathF.Sin(angle) * offset * 0.72f);

                    AccumulatePuff(x, y, satelliteX, satelliteY, satelliteRadius, ref field, ref light, ref shadow);
                }
            }
        }
    }

    private static void AccumulatePuff(float x, float y, float centerX, float centerY, float radius, ref float field, ref float light, ref float shadow)
    {
        float nx = (x - centerX) / (radius * 1.18f);
        float dy = y - centerY;
        float ny = dy < 0f ? dy / (radius * 0.96f) : dy / (radius * 0.70f);
        float dist = MathF.Sqrt((nx * nx) + (ny * ny));
        if (dist >= 1f)
        {
            return;
        }

        float puff = 1f - ProceduralEffectHelper.SmoothStep(0.10f, 1f, dist);
        float localLight = ProceduralEffectHelper.Clamp01(0.72f - (nx * 0.34f) - (ny * 0.58f));
        float underside = ProceduralEffectHelper.Clamp01((ny * 0.90f) + 0.12f);

        field = 1f - ((1f - field) * (1f - puff));
        light = MathF.Max(light, puff * localLight);
        shadow = MathF.Max(shadow, puff * (1f - localLight) * underside);
    }

    private static float SampleBillow(float x, float y, int octaves, float lacunarity, float gain, int seed)
    {
        float noise = ProceduralEffectHelper.FractalNoise(x, y, octaves, lacunarity, gain, seed);
        return 1f - MathF.Abs((noise * 2f) - 1f);
    }
}