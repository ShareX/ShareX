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

public sealed class CelticKnotBorderImageEffect : ImageEffectBase
{
    public override string Id => "celtic_knot_border";
    public override string Name => "Celtic knot border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.clover;
    public override string Description => "Adds an interweaving Celtic knot pattern border inspired by Insular art manuscripts.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<CelticKnotBorderImageEffect>("border_size", "Border size", 16, 200, 52, (e, v) => e.BorderSize = v),
        EffectParameters.Color<CelticKnotBorderImageEffect>("knot_color", "Knot color", new SKColor(180, 140, 60), (e, v) => e.KnotColor = v),
        EffectParameters.Color<CelticKnotBorderImageEffect>("bg_color", "Background color", new SKColor(35, 55, 25), (e, v) => e.BgColor = v),
        EffectParameters.FloatSlider<CelticKnotBorderImageEffect>("strand_width", "Strand width", 10, 80, 35, (e, v) => e.StrandWidth = v),
        EffectParameters.Bool<CelticKnotBorderImageEffect>("outline_strands", "Outline strands", true, (e, v) => e.OutlineStrands = v),
        EffectParameters.Bool<CelticKnotBorderImageEffect>("inner_line", "Inner line", true, (e, v) => e.InnerLine = v)
    ];

    public int BorderSize { get; set; } = 52;
    public SKColor KnotColor { get; set; } = new SKColor(180, 140, 60);
    public SKColor BgColor { get; set; } = new SKColor(35, 55, 25);
    public float StrandWidth { get; set; } = 35f;
    public bool OutlineStrands { get; set; } = true;
    public bool InnerLine { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 16, 200);
        float strandFrac = Math.Clamp(StrandWidth, 10f, 80f) / 100f;

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        // Fill border bands with background color
        using SKPaint bgPaint = new() { Color = BgColor, Style = SKPaintStyle.Fill };
        canvas.DrawRect(0, 0, newWidth, border, bgPaint);
        canvas.DrawRect(0, newHeight - border, newWidth, border, bgPaint);
        canvas.DrawRect(0, border, border, source.Height, bgPaint);
        canvas.DrawRect(newWidth - border, border, border, source.Height, bgPaint);

        float strandW = border * strandFrac;

        // Knot painting
        using SKPaint knotPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = strandW,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            Color = KnotColor
        };

        using SKPaint outlinePaint = OutlineStrands ? new SKPaint()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = strandW + 3f,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            Color = DarkenColor(BgColor, 0.3f)
        } : null!;

        // Cell size for the knot pattern (each cell is one over/under crossing)
        float cellSize = border * 1.2f;

        // Clip each border band and draw knot strands
        DrawBandKnots(canvas, knotPaint, outlinePaint, OutlineStrands,
            0, 0, newWidth, border, true, cellSize, border);
        DrawBandKnots(canvas, knotPaint, outlinePaint, OutlineStrands,
            0, newHeight - border, newWidth, border, true, cellSize, border);
        DrawBandKnots(canvas, knotPaint, outlinePaint, OutlineStrands,
            0, border, border, source.Height, false, cellSize, border);
        DrawBandKnots(canvas, knotPaint, outlinePaint, OutlineStrands,
            newWidth - border, border, border, source.Height, false, cellSize, border);

        // Source image
        canvas.DrawBitmap(source, border, border);

        // Inner edge line
        if (InnerLine)
        {
            using SKPaint linePaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2.5f,
                Color = KnotColor
            };
            canvas.DrawRect(border - 1.25f, border - 1.25f, source.Width + 2.5f, source.Height + 2.5f, linePaint);
        }

        // Outer edge line
        using SKPaint outerLinePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Color = KnotColor
        };
        canvas.DrawRect(1f, 1f, newWidth - 2f, newHeight - 2f, outerLinePaint);

        return result;
    }

    private static void DrawBandKnots(SKCanvas canvas, SKPaint knotPaint, SKPaint outlinePaint,
        bool drawOutline, float rx, float ry, float rw, float rh, bool horizontal, float cellSize, int border)
    {
        canvas.Save();
        canvas.ClipRect(new SKRect(rx, ry, rx + rw, ry + rh));

        float primaryLen = horizontal ? rw : rh;
        float mid = horizontal ? (ry + rh * 0.5f) : (rx + rw * 0.5f);
        float amplitude = rh * 0.25f;
        if (!horizontal) amplitude = rw * 0.25f;

        int numCells = Math.Max(2, (int)(primaryLen / cellSize) + 1);

        // Draw two interweaving sine-wave strands, offset by half a period
        for (int strand = 0; strand < 2; strand++)
        {
            float phase = strand * MathF.PI;
            using SKPath path = new();

            float startPos = rx;
            if (!horizontal) startPos = ry;

            bool first = true;
            int segments = numCells * 8;
            float segLen = primaryLen / segments;

            for (int s = 0; s <= segments; s++)
            {
                float along = s * segLen;
                float angle = (along / cellSize) * MathF.PI * 2f + phase;
                float wave = MathF.Sin(angle) * amplitude;

                float px, py;
                if (horizontal)
                {
                    px = rx + along;
                    py = mid + wave;
                }
                else
                {
                    py = ry + along;
                    px = mid + wave;
                }

                if (first)
                {
                    path.MoveTo(px, py);
                    first = false;
                }
                else
                {
                    path.LineTo(px, py);
                }
            }

            // Draw outline first for over/under illusion
            if (drawOutline)
            {
                canvas.DrawPath(path, outlinePaint);
            }
            canvas.DrawPath(path, knotPaint);
        }

        canvas.Restore();
    }

    private static SKColor DarkenColor(SKColor c, float factor)
    {
        return new SKColor(
            (byte)Math.Max(0, (int)(c.Red * (1f - factor))),
            (byte)Math.Max(0, (int)(c.Green * (1f - factor))),
            (byte)Math.Max(0, (int)(c.Blue * (1f - factor))),
            c.Alpha);
    }
}