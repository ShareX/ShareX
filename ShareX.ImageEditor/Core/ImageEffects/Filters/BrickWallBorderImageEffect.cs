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

public sealed class BrickWallBorderImageEffect : ImageEffectBase
{
    public override string Id => "brick_wall_border";
    public override string Name => "Brick wall border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.brick_wall;
    public override string Description => "Adds a procedural brick wall texture border with mortar lines, shading, and color variation.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<BrickWallBorderImageEffect>("border_size", "Border size", 10, 300, 56, (e, v) => e.BorderSize = v),
        EffectParameters.Color<BrickWallBorderImageEffect>("brick_color", "Brick color", new SKColor(178, 90, 62), (e, v) => e.BrickColor = v),
        EffectParameters.Color<BrickWallBorderImageEffect>("mortar_color", "Mortar color", new SKColor(195, 188, 175), (e, v) => e.MortarColor = v),
        EffectParameters.FloatSlider<BrickWallBorderImageEffect>("mortar_thickness", "Mortar thickness", 1, 20, 4, (e, v) => e.MortarThickness = v),
        EffectParameters.FloatSlider<BrickWallBorderImageEffect>("color_variation", "Color variation", 0, 100, 40, (e, v) => e.ColorVariation = v),
        EffectParameters.FloatSlider<BrickWallBorderImageEffect>("bevel_strength", "Bevel strength", 0, 100, 50, (e, v) => e.BevelStrength = v)
    ];

    public int BorderSize { get; set; } = 56;
    public SKColor BrickColor { get; set; } = new SKColor(178, 90, 62);
    public SKColor MortarColor { get; set; } = new SKColor(195, 188, 175);
    public float MortarThickness { get; set; } = 4f;
    public float ColorVariation { get; set; } = 40f;
    public float BevelStrength { get; set; } = 50f;

    private const int Seed = 3271;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 10, 300);
        float mortarT = Math.Clamp(MortarThickness, 1f, 20f);
        float colorVar = Math.Clamp(ColorVariation, 0f, 100f) / 100f;
        float bevel = Math.Clamp(BevelStrength, 0f, 100f) / 100f;

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        // Brick aspect ratio: width ~ 2.4x height (standard brick)
        float brickH = border * 0.45f;
        brickH = Math.Max(brickH, mortarT * 2 + 4f);
        float brickW = brickH * 2.4f;

        SKColor[] dstPixels = new SKColor[newWidth * newHeight];

        for (int py = 0; py < newHeight; py++)
        {
            bool topBand = py < border;
            bool bottomBand = py >= border + source.Height;

            for (int px = 0; px < newWidth; px++)
            {
                bool leftBand = px < border;
                bool rightBand = px >= border + source.Width;

                if (!topBand && !bottomBand && !leftBand && !rightBand)
                    continue;

                // Local coordinates within the band
                float localX, localY;
                if (topBand)
                {
                    localX = px;
                    localY = py;
                }
                else if (bottomBand)
                {
                    localX = newWidth - 1 - px; // mirror so bricks look continuous
                    localY = newHeight - 1 - py;
                }
                else if (leftBand)
                {
                    localX = py;
                    localY = px;
                }
                else // rightBand
                {
                    localX = newHeight - 1 - py;
                    localY = newWidth - 1 - px;
                }

                // Which brick row?
                int row = (int)(localY / brickH);
                float rowFrac = (localY % brickH) / brickH;

                // Stagger odd rows by half brick
                float offset = (row % 2 == 0) ? 0f : brickW * 0.5f;
                int col = (int)((localX + offset) / brickW);
                float colFrac = ((localX + offset) % brickW) / brickW;

                // Mortar detection
                float mortarFracH = mortarT / brickH;
                float mortarFracW = mortarT / brickW;

                bool isMortar = rowFrac < mortarFracH || colFrac < mortarFracW;

                if (isMortar)
                {
                    dstPixels[(py * newWidth) + px] = MortarColor;
                    continue;
                }

                // Brick color with variation per brick
                float varHash = ProceduralEffectHelper.Hash01(col, row ^ (row * 31 + Seed), Seed);
                float varHash2 = ProceduralEffectHelper.Hash01(col ^ (col * 13), row, Seed ^ 0x5A);
                float redVar = (varHash - 0.5f) * colorVar * 0.5f;
                float greenVar = (varHash2 - 0.5f) * colorVar * 0.25f;

                // Bevel shading within brick (darker near mortar joints)
                float bevelFrac = Math.Min(
                    Math.Min(rowFrac - mortarFracH, 1f - rowFrac),
                    Math.Min(colFrac - mortarFracW, 1f - colFrac));
                float bevelZone = 0.18f;
                float bevelShade = bevel > 0f
                    ? (1f - ProceduralEffectHelper.SmoothStep(0f, bevelZone, bevelFrac)) * bevel * 0.35f
                    : 0f;

                // Fine surface noise
                float surfaceNoise = ProceduralEffectHelper.Hash01((int)(localX * 2), (int)(localY * 2), Seed ^ 0x99) * 0.08f - 0.04f;

                float r = BrickColor.Red / 255f + redVar + surfaceNoise - bevelShade;
                float g = BrickColor.Green / 255f + greenVar * 0.5f + surfaceNoise * 0.4f - bevelShade;
                float b = BrickColor.Blue / 255f - redVar * 0.2f - bevelShade;

                dstPixels[(py * newWidth) + px] = new SKColor(
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

        // Inner edge line
        using SKPaint linePaint = new()
        {
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = mortarT,
            Color = MortarColor
        };
        canvas.DrawRect(
            border - mortarT * 0.5f,
            border - mortarT * 0.5f,
            source.Width + mortarT,
            source.Height + mortarT,
            linePaint);

        return result;
    }
}