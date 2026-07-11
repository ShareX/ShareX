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

public sealed class HexagonalPixelateImageEffect : ImageEffectBase
{
    public override string Id => "hexagonal_pixelate";
    public override string Name => "Hexagonal pixelate";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.hexagon;
    public override string Description => "Pixelates the image using hexagonal tiles.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<HexagonalPixelateImageEffect>("size", "Size", 4, 100, 16, (e, v) => e.Size = v)
    ];

    public int Size { get; set; } = 16;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        int size = Math.Max(4, Size);
        int w = source.Width, h = source.Height;
        SKColor[] src = source.Pixels;

        SKBitmap result = new(w, h, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        float hexW = size * 2f;
        float hexH = size * MathF.Sqrt(3f);

        using SKPaint paint = new() { IsAntialias = false, Style = SKPaintStyle.Fill };

        for (float row = -1; row * hexH / 2f < h + hexH; row++)
        {
            float yCenter = row * hexH / 2f;
            float xOffset = (row % 2 == 0) ? 0 : size * 1.5f;

            for (float col = -1; col * hexW * 0.75f < w + hexW; col++)
            {
                float xCenter = col * size * 3f + xOffset;

                int sx = Math.Clamp((int)xCenter, 0, w - 1);
                int sy = Math.Clamp((int)yCenter, 0, h - 1);
                paint.Color = src[sy * w + sx];

                using SKPath hex = CreateHexPath(xCenter, yCenter, size);
                canvas.DrawPath(hex, paint);
            }
        }

        return result;
    }

    private static SKPath CreateHexPath(float cx, float cy, float size)
    {
        SKPath path = new();
        for (int i = 0; i < 6; i++)
        {
            float angle = MathF.PI / 3f * i - MathF.PI / 6f;
            float x = cx + size * MathF.Cos(angle);
            float y = cy + size * MathF.Sin(angle);

            if (i == 0) path.MoveTo(x, y);
            else path.LineTo(x, y);
        }
        path.Close();
        return path;
    }
}