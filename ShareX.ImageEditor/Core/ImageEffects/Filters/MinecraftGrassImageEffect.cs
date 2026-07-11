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

public sealed class MinecraftGrassImageEffect : ImageEffectBase
{
    public override string Id => "minecraft_grass";
    public override string Name => "Minecraft grass";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.flower;
    public override string Description => "Adds a Minecraft-style pixelated grass border to the top or bottom of the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<MinecraftGrassImageEffect>("block_size", "Block size", 4, 64, 16, (e, v) => e.BlockSize = v),
        EffectParameters.IntSlider<MinecraftGrassImageEffect>("grass_rows", "Grass rows", 1, 8, 2, (e, v) => e.GrassRows = v),
        EffectParameters.IntSlider<MinecraftGrassImageEffect>("dirt_rows", "Dirt rows", 1, 16, 4, (e, v) => e.DirtRows = v),
        EffectParameters.Bool<MinecraftGrassImageEffect>("top_edge", "Top edge", false, (e, v) => e.TopEdge = v),
        EffectParameters.IntSlider<MinecraftGrassImageEffect>("overlap", "Overlap", 0, 8, 1, (e, v) => e.Overlap = v),
        EffectParameters.Color<MinecraftGrassImageEffect>("grass_color", "Grass color", new SKColor(89, 176, 48), (e, v) => e.GrassColor = v),
        EffectParameters.Color<MinecraftGrassImageEffect>("dirt_color", "Dirt color", new SKColor(134, 96, 67), (e, v) => e.DirtColor = v)
    ];

    public int BlockSize { get; set; } = 16;
    public int GrassRows { get; set; } = 2;
    public int DirtRows { get; set; } = 4;
    public bool TopEdge { get; set; }
    public int Overlap { get; set; } = 1;
    public SKColor GrassColor { get; set; } = new SKColor(89, 176, 48);
    public SKColor DirtColor { get; set; } = new SKColor(134, 96, 67);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int blockSize = Math.Clamp(BlockSize, 4, 64);
        int grassRows = Math.Clamp(GrassRows, 1, 8);
        int dirtRows = Math.Clamp(DirtRows, 1, 16);
        int totalRows = grassRows + dirtRows;
        int borderHeight = totalRows * blockSize;
        int overlapPx = Math.Clamp(Overlap, 0, 8) * blockSize;

        int newHeight = source.Height + borderHeight - overlapPx;
        SKBitmap result = new(source.Width, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        int imageY = TopEdge ? borderHeight - overlapPx : 0;
        canvas.DrawBitmap(source, 0, imageY);

        int borderY = TopEdge ? 0 : source.Height - overlapPx;
        int cols = (source.Width + blockSize - 1) / blockSize;

        using SKPaint paint = new() { IsAntialias = false, Style = SKPaintStyle.Fill };

        for (int col = 0; col < cols; col++)
        {
            // Jagged top edge: vary the start of the grass by 0-1 blocks
            float noise = ProceduralEffectHelper.Hash01(col, 0, 42);
            int jaggedOffset = noise > 0.5f ? blockSize : 0;

            for (int row = 0; row < totalRows; row++)
            {
                int x = col * blockSize;
                int y;

                if (TopEdge)
                {
                    y = borderHeight - (row + 1) * blockSize;
                }
                else
                {
                    y = borderY + row * blockSize;
                }

                // Skip blocks above the jagged edge
                int rowFromTop = TopEdge ? (totalRows - 1 - row) : row;
                if (rowFromTop == 0 && jaggedOffset > 0)
                {
                    continue;
                }

                bool isGrass = rowFromTop < grassRows;

                // Determine color with variation
                float variation = ProceduralEffectHelper.Hash01(col, row, 137);
                SKColor baseColor;

                if (isGrass)
                {
                    // Grass with natural variation
                    int rVar = (int)(variation * 20 - 10);
                    int gVar = (int)(variation * 30 - 15);
                    baseColor = new SKColor(
                        (byte)Math.Clamp(GrassColor.Red + rVar, 0, 255),
                        (byte)Math.Clamp(GrassColor.Green + gVar, 0, 255),
                        (byte)Math.Clamp(GrassColor.Blue + rVar, 0, 255),
                        255);
                }
                else
                {
                    // Dirt with variation
                    int dVar = (int)(variation * 24 - 12);
                    baseColor = new SKColor(
                        (byte)Math.Clamp(DirtColor.Red + dVar, 0, 255),
                        (byte)Math.Clamp(DirtColor.Green + dVar, 0, 255),
                        (byte)Math.Clamp(DirtColor.Blue + dVar + 4, 0, 255),
                        255);
                }

                // Add per-pixel noise within the block for texture
                int drawWidth = Math.Min(blockSize, source.Width - x);
                if (drawWidth <= 0) continue;

                paint.Color = baseColor;
                canvas.DrawRect(x, y, drawWidth, blockSize, paint);

                // Sprinkle a few darker pixels for texture
                float darkSpeckle = ProceduralEffectHelper.Hash01(col, row + 100, 271);
                if (darkSpeckle > 0.6f)
                {
                    int speckleSize = Math.Max(1, blockSize / 4);
                    float sx = x + ProceduralEffectHelper.Hash01(col, row, 311) * (blockSize - speckleSize);
                    float sy = y + ProceduralEffectHelper.Hash01(col, row, 419) * (blockSize - speckleSize);
                    paint.Color = new SKColor(
                        (byte)Math.Clamp(baseColor.Red - 25, 0, 255),
                        (byte)Math.Clamp(baseColor.Green - 20, 0, 255),
                        (byte)Math.Clamp(baseColor.Blue - 20, 0, 255),
                        255);
                    canvas.DrawRect(sx, sy, speckleSize, speckleSize, paint);
                }
            }
        }

        return result;
    }
}