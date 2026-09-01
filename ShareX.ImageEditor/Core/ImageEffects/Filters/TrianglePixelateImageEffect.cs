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

public sealed class TrianglePixelateImageEffect : ImageEffectBase
{
    public override string Id => "triangle_pixelate";
    public override string Name => "Triangle pixelate";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.triangle;
    public override string Description => "Pixelates the image using triangular tiles.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<TrianglePixelateImageEffect>("size", "Size", 8, 100, 24, (e, v) => e.Size = v)
    ];

    public int Size { get; set; } = 24;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        int size = Math.Max(8, Size);
        int w = source.Width, h = source.Height;
        SKColor[] src = source.Pixels;

        SKBitmap result = new(w, h, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        using SKPaint paint = new() { IsAntialias = false, Style = SKPaintStyle.Fill };

        float halfSize = size / 2f;

        for (int row = 0; row * halfSize < h + size; row++)
        {
            for (int col = 0; col * size < w + size; col++)
            {
                float x = col * size;
                float y = row * halfSize;

                // Two triangles per cell: upper-left and lower-right
                bool flipped = (row + col) % 2 == 0;

                SKPoint p1, p2, p3;
                if (flipped)
                {
                    p1 = new SKPoint(x, y);
                    p2 = new SKPoint(x + size, y);
                    p3 = new SKPoint(x + size / 2f, y + halfSize);
                }
                else
                {
                    p1 = new SKPoint(x, y + halfSize);
                    p2 = new SKPoint(x + size, y + halfSize);
                    p3 = new SKPoint(x + size / 2f, y);
                }

                // Sample color from triangle center
                float cx = (p1.X + p2.X + p3.X) / 3f;
                float cy = (p1.Y + p2.Y + p3.Y) / 3f;
                int sx = Math.Clamp((int)cx, 0, w - 1);
                int sy = Math.Clamp((int)cy, 0, h - 1);
                paint.Color = src[sy * w + sx];

                using SKPath path = new();
                path.MoveTo(p1);
                path.LineTo(p2);
                path.LineTo(p3);
                path.Close();
                canvas.DrawPath(path, paint);
            }
        }

        return result;
    }
}