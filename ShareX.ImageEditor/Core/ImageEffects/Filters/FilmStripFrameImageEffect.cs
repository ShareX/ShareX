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

using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class FilmStripFrameImageEffect : ImageEffectBase
{
    public override string Id => "film_strip_frame";
    public override string Name => "Film strip frame";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.film;
    public override string Description => "Wraps the image in a cinema film strip frame with sprocket holes.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<FilmStripFrameImageEffect>("border_size", "Border size", 10, 120, 40, (e, v) => e.BorderSize = v),
        EffectParameters.FloatSlider<FilmStripFrameImageEffect>("hole_size", "Hole size", 20, 80, 55, (e, v) => e.HoleSize = v),
        EffectParameters.Color<FilmStripFrameImageEffect>("frame_color", "Frame color", new SKColor(15, 15, 15), (e, v) => e.FrameColor = v),
        EffectParameters.Bool<FilmStripFrameImageEffect>("side_strips", "Side strips", false, (e, v) => e.SideStrips = v),
        EffectParameters.Bool<FilmStripFrameImageEffect>("scratches", "Scratches", true, (e, v) => e.Scratches = v)
    ];

    public int BorderSize { get; set; } = 40;
    public float HoleSize { get; set; } = 55f;
    public SKColor FrameColor { get; set; } = new SKColor(15, 15, 15);
    public bool SideStrips { get; set; } = false;
    public bool Scratches { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 10, 120);
        float holeSizeFraction = Math.Clamp(HoleSize, 20f, 80f) / 100f;

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        using SKPaint framePaint = new() { Color = FrameColor, Style = SKPaintStyle.Fill, IsAntialias = false };

        // Top and bottom bands
        canvas.DrawRect(0, 0, newWidth, border, framePaint);
        canvas.DrawRect(0, newHeight - border, newWidth, border, framePaint);

        // Left and right bands (always drawn so corner regions are filled)
        canvas.DrawRect(0, border, border, source.Height, framePaint);
        canvas.DrawRect(newWidth - border, border, border, source.Height, framePaint);

        // Source image in center
        canvas.DrawBitmap(source, border, border);

        // Scratch lines (subtle vertical lines on the frame)
        if (Scratches)
        {
            DrawScratches(canvas, newWidth, newHeight, border);
        }

        // Sprocket holes (transparent punch-outs)
        float holeH = border * holeSizeFraction;
        float holeW = holeH;
        float holeRadius = holeH * 0.15f;
        float holeMarginV = (border - holeH) / 2f;
        float step = holeW * 1.75f;

        using SKPaint holePaint = new() { BlendMode = SKBlendMode.Clear };

        // Top and bottom holes
        for (float x = step * 0.4f; x + holeW <= newWidth - step * 0.2f; x += step)
        {
            float topY = holeMarginV;
            float botY = newHeight - border + holeMarginV;
            canvas.DrawRoundRect(x, topY, holeW, holeH, holeRadius, holeRadius, holePaint);
            canvas.DrawRoundRect(x, botY, holeW, holeH, holeRadius, holeRadius, holePaint);
        }

        // Side holes if enabled
        if (SideStrips)
        {
            float sideHoleMarginH = (border - holeW) / 2f;
            for (float y = border + step * 0.4f; y + holeH <= newHeight - border - step * 0.2f; y += step)
            {
                canvas.DrawRoundRect(sideHoleMarginH, y, holeW, holeH, holeRadius, holeRadius, holePaint);
                canvas.DrawRoundRect(newWidth - border + sideHoleMarginH, y, holeW, holeH, holeRadius, holeRadius, holePaint);
            }
        }

        // Inner edge lines for a polished look
        using SKPaint linePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.2f,
            Color = new SKColor(60, 60, 60)
        };
        canvas.DrawRect(border - 0.6f, border - 0.6f, source.Width + 1.2f, source.Height + 1.2f, linePaint);

        return result;
    }

    private static void DrawScratches(SKCanvas canvas, int newWidth, int newHeight, int border)
    {
        using SKPaint scratchPaint = new()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 0.7f,
            IsAntialias = false
        };

        int seed = 7391;
        for (int i = 0; i < 10; i++)
        {
            seed = seed * 1664525 + 1013904223;
            float x = ((uint)seed % (uint)newWidth) / (float)newWidth * newWidth;
            seed = seed * 1664525 + 1013904223;
            float alpha = (((uint)seed % 100u) / 100f) * 0.35f + 0.07f;
            scratchPaint.Color = new SKColor(140, 140, 140, (byte)(alpha * 255));
            canvas.DrawLine(x, 0, x, newHeight, scratchPaint);
        }
    }
}