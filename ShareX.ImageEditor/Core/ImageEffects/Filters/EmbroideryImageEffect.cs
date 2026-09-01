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

public sealed class EmbroideryImageEffect : ImageEffectBase
{
    public override string Id => "embroidery";
    public override string Name => "Embroidery";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.stamp;
    public override string Description => "Simulates a cross-stitch embroidery pattern.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<EmbroideryImageEffect>("stitch_size", "Stitch Size", 4, 40, 10, (e, v) => e.StitchSize = v),
        EffectParameters.IntSlider<EmbroideryImageEffect>("color_count", "Color Count", 2, 64, 16, (e, v) => e.ColorCount = v),
        EffectParameters.Bool<EmbroideryImageEffect>("show_grid", "Show Grid", true, (e, v) => e.ShowGrid = v),
        EffectParameters.Color<EmbroideryImageEffect>("fabric_color", "Fabric Color", new SKColor(240, 235, 220, 255), (e, v) => e.FabricColor = v)
    ];

    public int StitchSize { get; set; } = 10;
    public int ColorCount { get; set; } = 16;
    public bool ShowGrid { get; set; } = true;
    public SKColor FabricColor { get; set; } = new SKColor(240, 235, 220, 255);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        int size = Math.Clamp(StitchSize, 2, 100);
        int colorCount = Math.Clamp(ColorCount, 2, 256);

        SKColor[] srcPixels = source.Pixels;

        // Quantize palette: reduce color levels per channel.
        int levelsPerChannel = Math.Max(2, (int)MathF.Ceiling(MathF.Pow(colorCount, 1f / 3f)));

        SKBitmap result = new(width, height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);

        // Fill with fabric color.
        canvas.Clear(FabricColor);

        // Draw cross-stitches.
        int gridCols = (width + size - 1) / size;
        int gridRows = (height + size - 1) / size;

        using SKPaint stitchPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        for (int gy = 0; gy < gridRows; gy++)
        {
            for (int gx = 0; gx < gridCols; gx++)
            {
                int x0 = gx * size;
                int y0 = gy * size;
                int x1 = Math.Min(x0 + size, width);
                int y1 = Math.Min(y0 + size, height);

                // Compute average color in this cell.
                long sumR = 0, sumG = 0, sumB = 0;
                int count = 0;
                for (int py = y0; py < y1; py++)
                {
                    int row = py * width;
                    for (int px = x0; px < x1; px++)
                    {
                        SKColor c = srcPixels[row + px];
                        sumR += c.Red;
                        sumG += c.Green;
                        sumB += c.Blue;
                        count++;
                    }
                }

                if (count == 0) continue;

                byte avgR = QuantizeChannel((byte)(sumR / count), levelsPerChannel);
                byte avgG = QuantizeChannel((byte)(sumG / count), levelsPerChannel);
                byte avgB = QuantizeChannel((byte)(sumB / count), levelsPerChannel);
                SKColor stitchColor = new(avgR, avgG, avgB, 255);

                // Draw an "X" cross-stitch pattern.
                float margin = size * 0.1f;
                float sx = x0 + margin;
                float sy = y0 + margin;
                float ex = x1 - margin;
                float ey = y1 - margin;
                float strokeWidth = Math.Max(1f, size * 0.15f);

                stitchPaint.Color = stitchColor;
                stitchPaint.StrokeWidth = strokeWidth;

                // First stroke of X: top-left to bottom-right
                canvas.DrawLine(sx, sy, ex, ey, stitchPaint);
                // Second stroke of X: bottom-left to top-right (drawn on top)
                canvas.DrawLine(sx, ey, ex, sy, stitchPaint);
            }
        }

        // Draw grid lines to simulate fabric holes.
        if (ShowGrid)
        {
            using SKPaint gridPaint = new()
            {
                IsAntialias = false,
                Color = new SKColor(
                    (byte)Math.Clamp(FabricColor.Red - 30, 0, 255),
                    (byte)Math.Clamp(FabricColor.Green - 30, 0, 255),
                    (byte)Math.Clamp(FabricColor.Blue - 30, 0, 255),
                    80),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1f
            };

            for (int gx = 0; gx <= gridCols; gx++)
            {
                float x = Math.Min(gx * size, width - 1);
                canvas.DrawLine(x, 0, x, height, gridPaint);
            }
            for (int gy = 0; gy <= gridRows; gy++)
            {
                float y = Math.Min(gy * size, height - 1);
                canvas.DrawLine(0, y, width, y, gridPaint);
            }
        }

        return result;
    }

    private static byte QuantizeChannel(byte value, int levels)
    {
        float step = 255f / (levels - 1);
        int quantized = (int)MathF.Round(value / step);
        return (byte)Math.Clamp((int)MathF.Round(quantized * step), 0, 255);
    }
}