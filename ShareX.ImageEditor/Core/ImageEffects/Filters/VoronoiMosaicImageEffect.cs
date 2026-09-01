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

public sealed class VoronoiMosaicImageEffect : ImageEffectBase
{
    public override string Id => "voronoi_mosaic";
    public override string Name => "Voronoi Mosaic";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.hexagon;
    public override string Description => "Divides the image into Voronoi cells colored by average region color.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<VoronoiMosaicImageEffect>("cell_count", "Cell Count", 10, 2000, 300, (e, v) => e.CellCount = v),
        EffectParameters.Bool<VoronoiMosaicImageEffect>("draw_edges", "Draw Edges", true, (e, v) => e.DrawEdges = v),
        EffectParameters.FloatSlider<VoronoiMosaicImageEffect>("edge_thickness", "Edge Thickness", 0.5, 5, 1, (e, v) => e.EdgeThickness = v,
            tickFrequency: 0.5, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.0}"),
        EffectParameters.Color<VoronoiMosaicImageEffect>("edge_color", "Edge Color", new SKColor(0, 0, 0, 255), (e, v) => e.EdgeColor = v)
    ];

    public int CellCount { get; set; } = 300;
    public bool DrawEdges { get; set; } = true;
    public float EdgeThickness { get; set; } = 1f;
    public SKColor EdgeColor { get; set; } = new SKColor(0, 0, 0, 255);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        int cellCount = Math.Clamp(CellCount, 2, 5000);

        // Generate seed points using a deterministic pseudo-random distribution.
        SKPoint[] seeds = new SKPoint[cellCount];
        int rngState = 42;
        for (int i = 0; i < cellCount; i++)
        {
            rngState = NextPseudoRandom(rngState);
            float x = (rngState & 0x7FFFFFFF) / (float)int.MaxValue * width;
            rngState = NextPseudoRandom(rngState);
            float y = (rngState & 0x7FFFFFFF) / (float)int.MaxValue * height;
            seeds[i] = new SKPoint(x, y);
        }

        SKColor[] pixels = source.Pixels;

        // Assign each pixel to nearest seed and accumulate color.
        int[] assignment = new int[pixels.Length];
        long[] sumR = new long[cellCount];
        long[] sumG = new long[cellCount];
        long[] sumB = new long[cellCount];
        int[] counts = new int[cellCount];

        // Build a grid acceleration structure to avoid O(n*k) brute force.
        float cellSize = MathF.Sqrt((float)(width * height) / cellCount) * 2f;
        int gridCols = Math.Max(1, (int)MathF.Ceiling(width / cellSize));
        int gridRows = Math.Max(1, (int)MathF.Ceiling(height / cellSize));
        List<int>[] grid = new List<int>[gridCols * gridRows];
        for (int i = 0; i < grid.Length; i++)
            grid[i] = [];

        for (int i = 0; i < cellCount; i++)
        {
            int gc = Math.Clamp((int)(seeds[i].X / cellSize), 0, gridCols - 1);
            int gr = Math.Clamp((int)(seeds[i].Y / cellSize), 0, gridRows - 1);
            grid[gr * gridCols + gc].Add(i);
        }

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            int gr = Math.Clamp((int)(y / cellSize), 0, gridRows - 1);

            for (int x = 0; x < width; x++)
            {
                int gc = Math.Clamp((int)(x / cellSize), 0, gridCols - 1);
                float bestDist = float.MaxValue;
                int bestSeed = 0;

                // Search neighboring grid cells.
                int rMin = Math.Max(0, gr - 2);
                int rMax = Math.Min(gridRows - 1, gr + 2);
                int cMin = Math.Max(0, gc - 2);
                int cMax = Math.Min(gridCols - 1, gc + 2);

                for (int r = rMin; r <= rMax; r++)
                {
                    for (int c = cMin; c <= cMax; c++)
                    {
                        foreach (int si in grid[r * gridCols + c])
                        {
                            float dx = x - seeds[si].X;
                            float dy = y - seeds[si].Y;
                            float dist = dx * dx + dy * dy;
                            if (dist < bestDist)
                            {
                                bestDist = dist;
                                bestSeed = si;
                            }
                        }
                    }
                }

                int idx = row + x;
                assignment[idx] = bestSeed;
                SKColor pixel = pixels[idx];
                sumR[bestSeed] += pixel.Red;
                sumG[bestSeed] += pixel.Green;
                sumB[bestSeed] += pixel.Blue;
                counts[bestSeed]++;
            }
        }

        // Compute average color per cell.
        SKColor[] cellColors = new SKColor[cellCount];
        for (int i = 0; i < cellCount; i++)
        {
            if (counts[i] > 0)
            {
                cellColors[i] = new SKColor(
                    (byte)(sumR[i] / counts[i]),
                    (byte)(sumG[i] / counts[i]),
                    (byte)(sumB[i] / counts[i]),
                    255);
            }
        }

        // Paint result.
        SKColor[] result = new SKColor[pixels.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = new SKColor(cellColors[assignment[i]].Red, cellColors[assignment[i]].Green, cellColors[assignment[i]].Blue, pixels[i].Alpha);
        }

        // Detect edges: pixel differs from neighbor's assignment.
        if (DrawEdges)
        {
            byte eR = EdgeColor.Red, eG = EdgeColor.Green, eB = EdgeColor.Blue, eA = EdgeColor.Alpha;
            int edgeDist = Math.Max(1, (int)MathF.Ceiling(EdgeThickness));

            for (int y = 0; y < height; y++)
            {
                int row = y * width;
                for (int x = 0; x < width; x++)
                {
                    int idx = row + x;
                    int mySeed = assignment[idx];
                    bool isEdge = false;

                    for (int d = 1; d <= edgeDist && !isEdge; d++)
                    {
                        if (x + d < width && assignment[idx + d] != mySeed) isEdge = true;
                        if (y + d < height && assignment[idx + d * width] != mySeed) isEdge = true;
                    }

                    if (isEdge)
                    {
                        result[idx] = new SKColor(eR, eG, eB, eA);
                    }
                }
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = result
        };
    }

    private static int NextPseudoRandom(int state)
    {
        // Xorshift32
        state ^= state << 13;
        state ^= state >> 17;
        state ^= state << 5;
        return state;
    }
}