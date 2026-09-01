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

public sealed class CircuitBoardBorderImageEffect : ImageEffectBase
{
    public override string Id => "circuit_board_border";
    public override string Name => "Circuit board border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.circuit_board;
    public override string Description => "Adds a PCB circuit board border with copper traces, vias, and pads on a dark substrate.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<CircuitBoardBorderImageEffect>("border_size", "Border size", 20, 300, 64, (e, v) => e.BorderSize = v),
        EffectParameters.Color<CircuitBoardBorderImageEffect>("board_color", "Board color", new SKColor(12, 58, 32), (e, v) => e.BoardColor = v),
        EffectParameters.Color<CircuitBoardBorderImageEffect>("trace_color", "Trace color", new SKColor(184, 115, 51), (e, v) => e.TraceColor = v),
        EffectParameters.FloatSlider<CircuitBoardBorderImageEffect>("trace_density", "Trace density", 10, 100, 55, (e, v) => e.TraceDensity = v),
        EffectParameters.Bool<CircuitBoardBorderImageEffect>("glow", "Trace glow", true, (e, v) => e.Glow = v),
        EffectParameters.Bool<CircuitBoardBorderImageEffect>("inner_line", "Inner line", true, (e, v) => e.InnerLine = v)
    ];

    public int BorderSize { get; set; } = 64;
    public SKColor BoardColor { get; set; } = new SKColor(12, 58, 32);
    public SKColor TraceColor { get; set; } = new SKColor(184, 115, 51);
    public float TraceDensity { get; set; } = 55f;
    public bool Glow { get; set; } = true;
    public bool InnerLine { get; set; } = true;

    private const int Seed = 8157;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 20, 300);
        float density = Math.Clamp(TraceDensity, 10f, 100f) / 100f;

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        // Board background
        using (SKPaint boardPaint = new() { Color = BoardColor, Style = SKPaintStyle.Fill })
        {
            canvas.DrawRect(0, 0, newWidth, border, boardPaint);
            canvas.DrawRect(0, newHeight - border, newWidth, border, boardPaint);
            canvas.DrawRect(0, border, border, source.Height, boardPaint);
            canvas.DrawRect(newWidth - border, border, border, source.Height, boardPaint);
        }

        // Draw traces on each side
        float traceW = Math.Max(1.5f, border * 0.045f);
        float padR = traceW * 1.8f;
        int gridStep = (int)Math.Max(6, border * 0.22f);

        // Glow paint
        using SKPaint glowPaint = Glow ? new SKPaint()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = traceW * 3.5f,
            Color = new SKColor(TraceColor.Red, TraceColor.Green, TraceColor.Blue, 45),
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, traceW * 1.5f)
        } : null!;

        using SKPaint tracePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = traceW,
            Color = TraceColor,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        using SKPaint padPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = TraceColor
        };

        using SKPaint viaRingPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = traceW * 0.6f,
            Color = TraceColor
        };

        using SKPaint viaHolePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = BoardColor
        };

        // Draw traces for each border band
        DrawBorderTraces(canvas, tracePaint, glowPaint, padPaint, viaRingPaint, viaHolePaint,
            0, 0, newWidth, border, true, border, gridStep, density, padR, Seed ^ 0x1A);
        DrawBorderTraces(canvas, tracePaint, glowPaint, padPaint, viaRingPaint, viaHolePaint,
            0, newHeight - border, newWidth, border, true, border, gridStep, density, padR, Seed ^ 0x2B);
        DrawBorderTraces(canvas, tracePaint, glowPaint, padPaint, viaRingPaint, viaHolePaint,
            0, border, border, source.Height, false, border, gridStep, density, padR, Seed ^ 0x3C);
        DrawBorderTraces(canvas, tracePaint, glowPaint, padPaint, viaRingPaint, viaHolePaint,
            newWidth - border, border, border, source.Height, false, border, gridStep, density, padR, Seed ^ 0x4D);

        // Source image
        canvas.DrawBitmap(source, border, border);

        // Inner edge line
        if (InnerLine)
        {
            using SKPaint linePaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = traceW * 0.8f,
                Color = TraceColor.WithAlpha(200)
            };
            canvas.DrawRect(border - traceW * 0.4f, border - traceW * 0.4f,
                source.Width + traceW * 0.8f, source.Height + traceW * 0.8f, linePaint);
        }

        return result;
    }

    private static void DrawBorderTraces(
        SKCanvas canvas,
        SKPaint tracePaint,
        SKPaint glowPaint,
        SKPaint padPaint,
        SKPaint viaRingPaint,
        SKPaint viaHolePaint,
        float rx, float ry, float rw, float rh,
        bool horizontal, int border, int gridStep, float density, float padR, int seed)
    {
        // Clip to this border band so traces don't overflow
        canvas.Save();
        canvas.ClipRect(new SKRect(rx, ry, rx + rw, ry + rh));

        float primaryLen = horizontal ? rw : rh;
        float crossLen = horizontal ? rh : rw;

        int numTraces = (int)(primaryLen / gridStep * density) + 1;

        for (int i = 0; i < numTraces; i++)
        {
            float hash = ProceduralEffectHelper.Hash01(i, seed, seed ^ 0x7F);
            float hash2 = ProceduralEffectHelper.Hash01(i, seed ^ 0x13, seed);
            float hash3 = ProceduralEffectHelper.Hash01(i, seed ^ 0x31, seed ^ 0x55);

            // Random start along primary axis
            float along = hash * primaryLen;
            float across = hash2 * crossLen * 0.85f + crossLen * 0.05f;

            // Trace length (varies)
            float tLen = (0.15f + hash3 * 0.65f) * primaryLen * 0.5f;
            float tEnd = Math.Min(along + tLen, primaryLen - 1);

            float x1, y1, x2, y2;
            if (horizontal)
            {
                x1 = rx + along; y1 = ry + across;
                x2 = rx + tEnd; y2 = ry + across;
            }
            else
            {
                x1 = rx + across; y1 = ry + along;
                x2 = rx + across; y2 = ry + tEnd;
            }

            // Draw glow first
            if (glowPaint != null)
                canvas.DrawLine(x1, y1, x2, y2, glowPaint);

            canvas.DrawLine(x1, y1, x2, y2, tracePaint);

            // Pad at start
            canvas.DrawCircle(x1, y1, padR, padPaint);

            // Via at end (ring + hole)
            float viaR = padR * 0.85f;
            canvas.DrawCircle(x2, y2, viaR * 1.5f, padPaint);
            canvas.DrawCircle(x2, y2, viaR * 0.65f, viaHolePaint);

            // Occasional 90-degree elbow connector
            float hash4 = ProceduralEffectHelper.Hash01(i, seed ^ 0x9A, seed ^ 0xCF);
            if (hash4 < 0.4f && tLen > gridStep * 1.5f)
            {
                float elbowAt = along + tLen * 0.5f;
                float elbowCross = across + (hash4 < 0.2f ? -1f : 1f) * gridStep * (0.5f + hash3 * 0.8f);
                float ex, ey, ex2, ey2;
                if (horizontal)
                {
                    ex = rx + elbowAt; ey = ry + across;
                    ex2 = rx + elbowAt; ey2 = ry + elbowCross;
                }
                else
                {
                    ex = rx + across; ey = ry + elbowAt;
                    ex2 = rx + elbowCross; ey2 = ry + elbowAt;
                }
                canvas.DrawLine(ex, ey, ex2, ey2, tracePaint);
                canvas.DrawCircle(ex2, ey2, padR * 0.9f, padPaint);
            }
        }

        canvas.Restore();
    }
}