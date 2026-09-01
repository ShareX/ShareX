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

public sealed class CrystalizeShardsImageEffect : ImageEffectBase
{
    public override string Id => "crystalize_shards";
    public override string Name => "Crystalize shards";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.diamond;
    public override string Description => "Breaks the image into crystalline Voronoi shards with edge highlights.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<CrystalizeShardsImageEffect>("shardSize", "Shard size", 6, 80, 22, (e, v) => e.ShardSize = v),
        EffectParameters.FloatSlider<CrystalizeShardsImageEffect>("jitter", "Jitter", 0f, 100f, 65f, (e, v) => e.Jitter = v),
        EffectParameters.FloatSlider<CrystalizeShardsImageEffect>("edgeStrength", "Edge strength", 0f, 100f, 75f, (e, v) => e.EdgeStrength = v),
        EffectParameters.FloatSlider<CrystalizeShardsImageEffect>("shine", "Shine", 0f, 100f, 30f, (e, v) => e.Shine = v),
    ];

    public int ShardSize { get; set; } = 22; // 6..80
    public float Jitter { get; set; } = 65f; // 0..100
    public float EdgeStrength { get; set; } = 75f; // 0..100
    public float Shine { get; set; } = 30f; // 0..100
    public int Seed { get; set; } = 4242;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float cellSize = Math.Clamp(ShardSize, 6, 80);
        float invCell = 1f / cellSize;
        float jitter = Math.Clamp(Jitter, 0f, 100f) / 100f;
        float edgeStrength = Math.Clamp(EdgeStrength, 0f, 100f) / 100f;
        float shine = Math.Clamp(Shine, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                float fx = x * invCell;
                float fy = y * invCell;
                int cx = (int)MathF.Floor(fx);
                int cy = (int)MathF.Floor(fy);

                float nearestDist = float.MaxValue;
                float secondDist = float.MaxValue;
                float nearestSeedX = x;
                float nearestSeedY = y;
                int nearestCellX = cx;
                int nearestCellY = cy;

                for (int oy = -1; oy <= 1; oy++)
                {
                    for (int ox = -1; ox <= 1; ox++)
                    {
                        int cellX = cx + ox;
                        int cellY = cy + oy;

                        float jx = ((ProceduralEffectHelper.Hash01(cellX, cellY, Seed) * 2f) - 1f) * 0.45f * jitter;
                        float jy = ((ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 911) * 2f) - 1f) * 0.45f * jitter;

                        float seedX = (cellX + 0.5f + jx) * cellSize;
                        float seedY = (cellY + 0.5f + jy) * cellSize;

                        float dx = seedX - x;
                        float dy = seedY - y;
                        float dist = (dx * dx) + (dy * dy);

                        if (dist < nearestDist)
                        {
                            secondDist = nearestDist;
                            nearestDist = dist;
                            nearestSeedX = seedX;
                            nearestSeedY = seedY;
                            nearestCellX = cellX;
                            nearestCellY = cellY;
                        }
                        else if (dist < secondDist)
                        {
                            secondDist = dist;
                        }
                    }
                }

                SKColor sample = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, nearestSeedX, nearestSeedY);
                float r = sample.Red;
                float g = sample.Green;
                float b = sample.Blue;
                float a = sample.Alpha;

                float nearest = MathF.Sqrt(MathF.Max(0f, nearestDist));
                float second = MathF.Sqrt(MathF.Max(0f, secondDist));
                float borderDiff = second - nearest;
                float edge = 1f - ProceduralEffectHelper.SmoothStep(0f, cellSize * 0.12f, borderDiff);
                edge *= edgeStrength;

                float facetNoise = ProceduralEffectHelper.Hash01(nearestCellX, nearestCellY, Seed ^ 1777);
                float centerFactor = 1f - MathF.Min(1f, nearest / MathF.Max(1f, cellSize));
                float shade = 1f + ((facetNoise - 0.5f) * 0.25f) + (centerFactor * shine * 0.45f);

                r *= shade;
                g *= shade;
                b *= shade;

                if (edge > 0.001f)
                {
                    float edgeDark = edge * 0.78f;
                    r = ProceduralEffectHelper.Lerp(r, r * 0.22f, edgeDark);
                    g = ProceduralEffectHelper.Lerp(g, g * 0.26f, edgeDark);
                    b = ProceduralEffectHelper.Lerp(b, b * 0.34f, edgeDark);

                    float glint = edge * (0.20f + (facetNoise * 0.25f));
                    r += 38f * glint;
                    g += 52f * glint;
                    b += 80f * glint;
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