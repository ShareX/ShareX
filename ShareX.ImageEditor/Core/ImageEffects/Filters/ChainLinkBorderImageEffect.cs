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

public sealed class ChainLinkBorderImageEffect : ImageEffectBase
{
    public override string Id => "chain_link_border";
    public override string Name => "Chain link border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.fence;
    public override string Description => "Adds a metallic chain-link fence border with interlocking wire loops and highlights.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<ChainLinkBorderImageEffect>("border_size", "Border size", 16, 250, 52, (e, v) => e.BorderSize = v),
        EffectParameters.Color<ChainLinkBorderImageEffect>("wire_color", "Wire color", new SKColor(170, 175, 180), (e, v) => e.WireColor = v),
        EffectParameters.Color<ChainLinkBorderImageEffect>("bg_color", "Background color", new SKColor(35, 40, 48), (e, v) => e.BgColor = v),
        EffectParameters.FloatSlider<ChainLinkBorderImageEffect>("cell_size", "Cell size", 8, 60, 20, (e, v) => e.CellSize = v),
        EffectParameters.FloatSlider<ChainLinkBorderImageEffect>("wire_width", "Wire width", 1, 8, 2.5f, (e, v) => e.WireWidth = v),
        EffectParameters.Bool<ChainLinkBorderImageEffect>("highlight", "Wire highlight", true, (e, v) => e.Highlight = v)
    ];

    public int BorderSize { get; set; } = 52;
    public SKColor WireColor { get; set; } = new SKColor(170, 175, 180);
    public SKColor BgColor { get; set; } = new SKColor(35, 40, 48);
    public float CellSize { get; set; } = 20f;
    public float WireWidth { get; set; } = 2.5f;
    public bool Highlight { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 16, 250);
        float cell = Math.Clamp(CellSize, 8f, 60f);
        float wireW = Math.Clamp(WireWidth, 1f, 8f);

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);

        canvas.Clear(BgColor);

        // Draw chain link diamond pattern across all 4 borders
        // Each "diamond" is drawn as 4 line segments forming a zigzag mesh
        float halfCell = cell * 0.5f;

        using SKPaint wirePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = wireW,
            StrokeCap = SKStrokeCap.Round,
            Color = WireColor
        };

        using SKPaint highlightPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = wireW * 0.5f,
            StrokeCap = SKStrokeCap.Round,
            Color = LightenColor(WireColor, 0.4f)
        };

        // Draw zigzag diamonds
        // Two sets of parallel diagonal lines create the chain-link mesh
        DrawChainMeshOnBand(canvas, wirePaint, highlightPaint, cell, halfCell,
            0, 0, newWidth, border); // top
        DrawChainMeshOnBand(canvas, wirePaint, highlightPaint, cell, halfCell,
            0, border + source.Height, newWidth, border); // bottom
        DrawChainMeshOnBand(canvas, wirePaint, highlightPaint, cell, halfCell,
            0, border, border, source.Height); // left
        DrawChainMeshOnBand(canvas, wirePaint, highlightPaint, cell, halfCell,
            border + source.Width, border, border, source.Height); // right

        // Border outline
        using SKPaint outlinePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = wireW * 1.5f,
            Color = DarkenColor(WireColor, 0.3f)
        };
        canvas.DrawRect(border - 1, border - 1, source.Width + 2, source.Height + 2, outlinePaint);

        canvas.DrawBitmap(source, border, border);

        return result;
    }

    private void DrawChainMeshOnBand(SKCanvas canvas, SKPaint wirePaint, SKPaint highlightPaint,
        float cell, float halfCell, float bandX, float bandY, float bandW, float bandH)
    {
        canvas.Save();
        canvas.ClipRect(new SKRect(bandX, bandY, bandX + bandW, bandY + bandH));

        // Diagonal lines going ↘ and ↗ form diamond shapes
        // Set 1: top-left to bottom-right diagonals
        float startX = bandX - cell * 2;
        float startY = bandY - cell * 2;
        float endX = bandX + bandW + cell * 2;
        float endY = bandY + bandH + cell * 2;

        for (float y = startY; y <= endY; y += cell)
        {
            for (float x = startX; x <= endX; x += cell)
            {
                // Draw diamond: 4 segments
                float cx = x;
                float cy = y;

                using SKPath path = new();
                // Top to right
                path.MoveTo(cx, cy - halfCell);
                path.LineTo(cx + halfCell, cy);
                // Right to bottom
                path.LineTo(cx, cy + halfCell);
                // Bottom to left
                path.LineTo(cx - halfCell, cy);
                // Left to top
                path.LineTo(cx, cy - halfCell);

                canvas.DrawPath(path, wirePaint);

                if (Highlight)
                {
                    // Highlight on top-left edges
                    using SKPath hlPath = new();
                    hlPath.MoveTo(cx - halfCell, cy);
                    hlPath.LineTo(cx, cy - halfCell);
                    hlPath.LineTo(cx + halfCell, cy);
                    canvas.DrawPath(hlPath, highlightPaint);
                }
            }
        }

        canvas.Restore();
    }

    private static SKColor LightenColor(SKColor color, float amount)
    {
        return new SKColor(
            ProceduralEffectHelper.ClampToByte(color.Red + (255 - color.Red) * amount),
            ProceduralEffectHelper.ClampToByte(color.Green + (255 - color.Green) * amount),
            ProceduralEffectHelper.ClampToByte(color.Blue + (255 - color.Blue) * amount),
            color.Alpha);
    }

    private static SKColor DarkenColor(SKColor color, float amount)
    {
        return new SKColor(
            ProceduralEffectHelper.ClampToByte(color.Red * (1f - amount)),
            ProceduralEffectHelper.ClampToByte(color.Green * (1f - amount)),
            ProceduralEffectHelper.ClampToByte(color.Blue * (1f - amount)),
            color.Alpha);
    }
}