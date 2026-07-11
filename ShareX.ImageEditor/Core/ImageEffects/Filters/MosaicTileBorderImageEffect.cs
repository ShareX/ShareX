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

public sealed class MosaicTileBorderImageEffect : ImageEffectBase
{
    public override string Id => "mosaic_tile_border";
    public override string Name => "Mosaic tile border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.grid_3x3;
    public override string Description => "Adds a colorful mosaic tile border with individual square tiles, grout lines, and subtle per-tile variation.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<MosaicTileBorderImageEffect>("border_size", "Border size", 12, 300, 48, (e, v) => e.BorderSize = v),
        EffectParameters.FloatSlider<MosaicTileBorderImageEffect>("tile_size", "Tile size", 4, 60, 14, (e, v) => e.TileSize = v),
        EffectParameters.Color<MosaicTileBorderImageEffect>("grout_color", "Grout color", new SKColor(210, 205, 195), (e, v) => e.GroutColor = v),
        EffectParameters.FloatSlider<MosaicTileBorderImageEffect>("grout_width", "Grout width", 0.5f, 8, 2, (e, v) => e.GroutWidth = v),
        EffectParameters.FloatSlider<MosaicTileBorderImageEffect>("color_variety", "Color variety", 0, 100, 70, (e, v) => e.ColorVariety = v),
        EffectParameters.FloatSlider<MosaicTileBorderImageEffect>("saturation", "Saturation", 0, 100, 85, (e, v) => e.Saturation = v)
    ];

    public int BorderSize { get; set; } = 48;
    public float TileSize { get; set; } = 14f;
    public SKColor GroutColor { get; set; } = new SKColor(210, 205, 195);
    public float GroutWidth { get; set; } = 2f;
    public float ColorVariety { get; set; } = 70f;
    public float Saturation { get; set; } = 85f;

    private const int Seed = 5923;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 12, 300);
        float tileSz = Math.Clamp(TileSize, 4f, 60f);
        float groutW = Math.Clamp(GroutWidth, 0.5f, 8f);
        float variety = Math.Clamp(ColorVariety, 0f, 100f) / 100f;
        float sat = Math.Clamp(Saturation, 0f, 100f);

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKColor[] dstPixels = new SKColor[newWidth * newHeight];

        float groutHalf = groutW * 0.5f;

        for (int y = 0; y < newHeight; y++)
        {
            bool topBand = y < border;
            bool bottomBand = y >= border + source.Height;

            for (int x = 0; x < newWidth; x++)
            {
                bool leftBand = x < border;
                bool rightBand = x >= border + source.Width;

                if (!topBand && !bottomBand && !leftBand && !rightBand)
                    continue;

                // Global tile grid coordinates
                int tileCol = (int)MathF.Floor(x / tileSz);
                int tileRow = (int)MathF.Floor(y / tileSz);
                float withinTileX = (x % tileSz);
                float withinTileY = (y % tileSz);
                if (withinTileX < 0) withinTileX += tileSz;
                if (withinTileY < 0) withinTileY += tileSz;

                // Grout detection: near tile edges
                bool isGrout = withinTileX < groutHalf || withinTileX > tileSz - groutHalf
                            || withinTileY < groutHalf || withinTileY > tileSz - groutHalf;

                if (isGrout)
                {
                    dstPixels[(y * newWidth) + x] = GroutColor;
                    continue;
                }

                // Tile color from hue hash
                float hueHash = ProceduralEffectHelper.Hash01(tileCol, tileRow, Seed);
                float hue = hueHash * 360f * variety;
                float brightnessHash = ProceduralEffectHelper.Hash01(tileCol ^ 0x55, tileRow ^ 0xAA, Seed ^ 0x17);
                float brightness = 55f + brightnessHash * 40f;

                SKColor tileColor = SKColor.FromHsv(hue, sat, brightness);

                // Subtle bevel: darken near grout edges
                float distFromEdge = Math.Min(
                    Math.Min(withinTileX - groutHalf, tileSz - groutHalf - withinTileX),
                    Math.Min(withinTileY - groutHalf, tileSz - groutHalf - withinTileY));
                float bevelZone = tileSz * 0.15f;
                float bevelShade = ProceduralEffectHelper.SmoothStep(bevelZone, 0f, distFromEdge) * 0.2f;

                // Very subtle per-pixel noise for ceramic texture
                float noise = (ProceduralEffectHelper.Hash01(x, y, Seed ^ 0x99) - 0.5f) * 0.06f;

                float r = tileColor.Red / 255f - bevelShade + noise;
                float g = tileColor.Green / 255f - bevelShade + noise;
                float b = tileColor.Blue / 255f - bevelShade + noise;

                dstPixels[(y * newWidth) + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r * 255f),
                    ProceduralEffectHelper.ClampToByte(g * 255f),
                    ProceduralEffectHelper.ClampToByte(b * 255f),
                    255);
            }
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        using SKCanvas canvas = new SKCanvas(result);
        canvas.DrawBitmap(source, border, border);

        // Inner grout line
        using SKPaint groutPaint = new()
        {
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = groutW,
            Color = GroutColor
        };
        canvas.DrawRect(border - groutW * 0.5f, border - groutW * 0.5f,
            source.Width + groutW, source.Height + groutW, groutPaint);

        return result;
    }
}