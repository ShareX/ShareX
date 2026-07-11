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

public sealed class DiamondPixelateImageEffect : ImageEffectBase
{
    public override string Id => "diamond_pixelate";
    public override string Name => "Diamond pixelate";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.diamond;
    public override string Description => "Pixelates the image using diamond-shaped blocks.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<DiamondPixelateImageEffect>("size", "Size", 4, 100, 16, (e, v) => e.Size = v)
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

        int half = size / 2;

        for (int cy = 0; cy < h + size; cy += half)
        {
            for (int cx = 0; cx < w + size; cx += size)
            {
                int offsetX = ((cy / half) % 2 == 0) ? 0 : half;
                int dx = cx + offsetX;

                int sampleX = Math.Clamp(dx, 0, w - 1);
                int sampleY = Math.Clamp(cy, 0, h - 1);
                SKColor color = src[sampleY * w + sampleX];

                using SKPath path = new();
                path.MoveTo(dx, cy - half);
                path.LineTo(dx + half, cy);
                path.LineTo(dx, cy + half);
                path.LineTo(dx - half, cy);
                path.Close();

                using SKPaint paint = new() { Color = color, IsAntialias = false, Style = SKPaintStyle.Fill };
                canvas.DrawPath(path, paint);
            }
        }

        return result;
    }
}