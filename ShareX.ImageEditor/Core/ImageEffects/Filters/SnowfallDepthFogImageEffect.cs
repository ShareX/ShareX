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

public sealed class SnowfallDepthFogImageEffect : ImageEffectBase
{
    public override string Id => "snowfall_depth_fog";
    public override string Name => "Snowfall + depth fog";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.cloud_snow;
    public override string Description => "Simulates snowfall with depth-based fog.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<SnowfallDepthFogImageEffect>("snow_amount", "Snow amount", 0f, 100f, 55f, (e, v) => e.SnowAmount = v),
        EffectParameters.FloatSlider<SnowfallDepthFogImageEffect>("flake_size", "Flake size", 1f, 14f, 5f, (e, v) => e.FlakeSize = v),
        EffectParameters.FloatSlider<SnowfallDepthFogImageEffect>("wind", "Wind", -100f, 100f, 20f, (e, v) => e.Wind = v),
        EffectParameters.FloatSlider<SnowfallDepthFogImageEffect>("fog_amount", "Fog amount", 0f, 100f, 40f, (e, v) => e.FogAmount = v),
        EffectParameters.FloatSlider<SnowfallDepthFogImageEffect>("fog_height", "Fog height", 10f, 100f, 65f, (e, v) => e.FogHeight = v),
    ];

    public float SnowAmount { get; set; } = 55f; // 0..100
    public float FlakeSize { get; set; } = 5f; // 1..14
    public float Wind { get; set; } = 20f; // -100..100
    public float FogAmount { get; set; } = 40f; // 0..100
    public float FogHeight { get; set; } = 65f; // 10..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float snowAmount = Math.Clamp(SnowAmount, 0f, 100f) / 100f;
        float flakeSize = Math.Clamp(FlakeSize, 1f, 14f);
        float wind = Math.Clamp(Wind, -100f, 100f) / 100f;
        float fogAmount = Math.Clamp(FogAmount, 0f, 100f) / 100f;
        float fogHeight = Math.Clamp(FogHeight, 10f, 100f) / 100f;

        if (snowAmount <= 0.0001f && fogAmount <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        int bottom = height - 1;
        int seed = Random.Shared.Next(1, int.MaxValue);
        float invHeight = 1f / Math.Max(1, bottom);

        float layerOneCell = Math.Clamp(22f - (flakeSize * 0.9f), 8f, 24f);
        float layerTwoCell = Math.Max(6f, layerOneCell * 0.58f);
        float layerOneRadius = 0.9f + (flakeSize * 0.22f);
        float layerTwoRadius = 0.55f + (flakeSize * 0.14f);
        float layerOneDensity = 0.015f + (snowAmount * 0.22f);
        float layerTwoDensity = 0.04f + (snowAmount * 0.30f);
        float fogStartDepth = 1f - fogHeight;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            float normY = y * invHeight;
            float depth = 1f - normY;
            float perspective = 0.55f + (normY * 0.45f);
            float windOffset = wind * y * 0.38f;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float r = src.Red;
                float g = src.Green;
                float b = src.Blue;
                float a = src.Alpha;

                if (fogAmount > 0.0001f)
                {
                    float fogMask = ProceduralEffectHelper.SmoothStep(fogStartDepth, 1f, depth);
                    if (fogMask > 0.0001f)
                    {
                        float coarse = ProceduralEffectHelper.Hash01((int)(x * 0.06f), (int)(y * 0.06f), seed ^ 191);
                        float broad = ProceduralEffectHelper.Hash01((int)(x * 0.018f), (int)(y * 0.018f), seed ^ 379);
                        float fogNoise = (coarse * 0.65f) + (broad * 0.35f);
                        float fog = fogAmount * fogMask * (0.75f + (0.25f * fogNoise)) * (0.62f + (0.38f * depth));

                        r = ProceduralEffectHelper.Lerp(r, 226f, fog);
                        g = ProceduralEffectHelper.Lerp(g, 236f, fog);
                        b = ProceduralEffectHelper.Lerp(b, 248f, fog);
                    }
                }

                if (snowAmount > 0.0001f)
                {
                    float layerOne = SampleFlakeLayer(x, y, layerOneCell, layerOneRadius, layerOneDensity, windOffset, seed ^ 911);
                    float layerTwo = SampleFlakeLayer(x, y, layerTwoCell, layerTwoRadius, layerTwoDensity, windOffset * 1.3f, seed ^ 3001);
                    float sparkle = MathF.Pow(ProceduralEffectHelper.Hash01(x, y, seed ^ 4451), 20f) * snowAmount * 0.9f;

                    float snow = ((layerOne * 0.72f) + (layerTwo * 0.48f) + sparkle) * perspective;
                    snow = Math.Clamp(snow, 0f, 1f);

                    if (snow > 0.0001f)
                    {
                        r = ProceduralEffectHelper.Lerp(r, 255f, snow);
                        g = ProceduralEffectHelper.Lerp(g, 255f, snow);
                        b = ProceduralEffectHelper.Lerp(b, 255f, snow);
                    }
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

    private static float SampleFlakeLayer(int x, int y, float cellSize, float radius, float density, float windOffset, int seed)
    {
        float px = x + windOffset;
        float py = y;

        int cellX = (int)MathF.Floor(px / cellSize);
        int cellY = (int)MathF.Floor(py / cellSize);

        float strongest = 0f;

        for (int oy = -1; oy <= 1; oy++)
        {
            for (int ox = -1; ox <= 1; ox++)
            {
                int nx = cellX + ox;
                int ny = cellY + oy;

                float spawn = ProceduralEffectHelper.Hash01(nx, ny, seed);
                if (spawn > density)
                {
                    continue;
                }

                float centerX = (nx + ProceduralEffectHelper.Hash01(nx, ny, seed ^ 1777)) * cellSize;
                float centerY = (ny + ProceduralEffectHelper.Hash01(nx, ny, seed ^ 3907)) * cellSize;
                float dx = px - centerX;
                float dy = py - centerY;
                float distSq = (dx * dx) + (dy * dy);

                float flakeRadius = radius * (0.6f + (ProceduralEffectHelper.Hash01(nx, ny, seed ^ 727) * 0.9f));
                float radiusSq = flakeRadius * flakeRadius;
                if (distSq >= radiusSq)
                {
                    continue;
                }

                float dist = MathF.Sqrt(distSq);
                float alpha = 1f - ProceduralEffectHelper.SmoothStep(flakeRadius * 0.3f, flakeRadius, dist);
                if (alpha > strongest)
                {
                    strongest = alpha;
                }
            }
        }

        return strongest;
    }
}