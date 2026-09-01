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

public sealed class EngravingImageEffect : ImageEffectBase
{
    public override string Id => "engraving";
    public override string Name => "Engraving";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.pen_line;
    public override string Description => "Simulates a line engraving effect like banknote illustrations.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<EngravingImageEffect>("line_spacing", "Line Spacing", 2, 20, 5, (e, v) => e.LineSpacing = v,
            tickFrequency: 0.5, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.0}"),
        EffectParameters.FloatSlider<EngravingImageEffect>("angle", "Angle", 0, 180, 45, (e, v) => e.Angle = v),
        EffectParameters.FloatSlider<EngravingImageEffect>("line_width_max", "Max Line Width", 0.5, 8, 2.5, (e, v) => e.LineWidthMax = v,
            tickFrequency: 0.1, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.0}"),
        EffectParameters.Color<EngravingImageEffect>("line_color", "Line Color", new SKColor(20, 20, 40, 255), (e, v) => e.LineColor = v),
        EffectParameters.Color<EngravingImageEffect>("paper_color", "Paper Color", new SKColor(245, 240, 230, 255), (e, v) => e.PaperColor = v)
    ];

    public float LineSpacing { get; set; } = 5f;
    public float Angle { get; set; } = 45f;
    public float LineWidthMax { get; set; } = 2.5f;
    public SKColor LineColor { get; set; } = new SKColor(20, 20, 40, 255);
    public SKColor PaperColor { get; set; } = new SKColor(245, 240, 230, 255);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        float spacing = Math.Clamp(LineSpacing, 1f, 50f);
        float maxWidth = Math.Clamp(LineWidthMax, 0.1f, 20f);
        float angleRad = Angle * MathF.PI / 180f;

        // Build a luminance map from source.
        SKColor[] srcPixels = source.Pixels;
        float[] luminance = new float[width * height];
        for (int i = 0; i < srcPixels.Length; i++)
        {
            SKColor c = srcPixels[i];
            luminance[i] = (0.299f * c.Red + 0.587f * c.Green + 0.114f * c.Blue) / 255f;
        }

        // Create result bitmap with paper background.
        SKBitmap result = new(width, height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);
        canvas.Clear(PaperColor);

        // Direction perpendicular to the engraving line angle.
        float cosA = MathF.Cos(angleRad);
        float sinA = MathF.Sin(angleRad);

        // Diagonal length determines max projection distance.
        float diagonal = MathF.Sqrt(width * width + height * height);

        // For each engraving line, walk along it and draw segments with
        // width proportional to darkness at that point.
        int lineCount = (int)MathF.Ceiling(diagonal / spacing) + 1;

        using SKPaint paint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = LineColor,
            StrokeCap = SKStrokeCap.Round
        };

        // Center of image for projection reference.
        float cx = width * 0.5f;
        float cy = height * 0.5f;

        for (int i = -lineCount; i <= lineCount; i++)
        {
            // Perpendicular offset from center line.
            float offset = i * spacing;

            // Line start and end: project far enough to cover the image.
            float basePx = cx + cosA * offset;
            float basePy = cy + sinA * offset;

            float x0 = basePx - sinA * diagonal;
            float y0 = basePy + cosA * diagonal;
            float x1 = basePx + sinA * diagonal;
            float y1 = basePy - cosA * diagonal;

            // Walk along this line in small steps, modulating stroke width by darkness.
            int steps = (int)(diagonal * 2f / spacing) + 1;
            float stepSize = diagonal * 2f / steps;

            float prevX = x0;
            float prevY = y0;

            for (int s = 1; s <= steps; s++)
            {
                float t = s / (float)steps;
                float px = x0 + (x1 - x0) * t;
                float py = y0 + (y1 - y0) * t;

                // Sample luminance at this position.
                int sx = (int)MathF.Round(px);
                int sy = (int)MathF.Round(py);

                if (sx >= 0 && sx < width && sy >= 0 && sy < height)
                {
                    float lum = luminance[sy * width + sx];
                    // Darker areas = thicker lines. White areas = no line.
                    float darkness = 1f - lum;
                    float strokeWidth = darkness * maxWidth;

                    if (strokeWidth > 0.05f)
                    {
                        paint.StrokeWidth = strokeWidth;
                        canvas.DrawLine(prevX, prevY, px, py, paint);
                    }
                }

                prevX = px;
                prevY = py;
            }
        }

        return result;
    }
}