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

public sealed class DotMatrixImageEffect : ImageEffectBase
{
    public override string Id => "dot_matrix";
    public override string Name => "Dot Matrix";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.printer;
    public override string Description => "Simulates a vintage dot matrix printer output.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<DotMatrixImageEffect>("cell_size", "Cell Size", 3, 30, 8, (e, v) => e.CellSize = v),
        EffectParameters.FloatSlider<DotMatrixImageEffect>("dot_scale", "Dot Scale", 0.1, 1, 0.85, (e, v) => e.DotScale = v,
            tickFrequency: 0.05, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.00}"),
        EffectParameters.Bool<DotMatrixImageEffect>("color_mode", "Color", false, (e, v) => e.ColorMode = v),
        EffectParameters.Color<DotMatrixImageEffect>("dot_color", "Dot Color", new SKColor(20, 20, 20, 255), (e, v) => e.DotColor = v),
        EffectParameters.Color<DotMatrixImageEffect>("paper_color", "Paper Color", new SKColor(250, 245, 235, 255), (e, v) => e.PaperColor = v)
    ];

    public int CellSize { get; set; } = 8;
    public float DotScale { get; set; } = 0.85f;
    public bool ColorMode { get; set; }
    public SKColor DotColor { get; set; } = new SKColor(20, 20, 20, 255);
    public SKColor PaperColor { get; set; } = new SKColor(250, 245, 235, 255);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        int size = Math.Clamp(CellSize, 2, 100);
        float dotScale = Math.Clamp(DotScale, 0.05f, 1f);
        float maxRadius = size * 0.5f * dotScale;

        SKColor[] srcPixels = source.Pixels;

        int gridCols = (width + size - 1) / size;
        int gridRows = (height + size - 1) / size;

        SKBitmap result = new(width, height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);
        canvas.Clear(PaperColor);

        using SKPaint dotPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        for (int gy = 0; gy < gridRows; gy++)
        {
            for (int gx = 0; gx < gridCols; gx++)
            {
                int x0 = gx * size;
                int y0 = gy * size;
                int x1 = Math.Min(x0 + size, width);
                int y1 = Math.Min(y0 + size, height);

                // Average color in this cell.
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

                byte avgR = (byte)(sumR / count);
                byte avgG = (byte)(sumG / count);
                byte avgB = (byte)(sumB / count);

                float luminance = (0.299f * avgR + 0.587f * avgG + 0.114f * avgB) / 255f;
                // Darker = larger dot.
                float darkness = 1f - luminance;
                float radius = darkness * maxRadius;

                if (radius < 0.3f) continue;

                if (ColorMode)
                {
                    dotPaint.Color = new SKColor(avgR, avgG, avgB, 255);
                }
                else
                {
                    dotPaint.Color = DotColor;
                }

                float cx = x0 + size * 0.5f;
                float cy = y0 + size * 0.5f;
                canvas.DrawCircle(cx, cy, radius, dotPaint);
            }
        }

        return result;
    }
}