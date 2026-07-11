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

public sealed class ColorHalftoneImageEffect : ImageEffectBase
{
    public override string Id => "color_halftone";
    public override string Name => "Color halftone";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.circle_dot;
    public override string Description => "Simulates CMYK color halftone printing with rotated dot grids.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<ColorHalftoneImageEffect>("size", "Dot size", 4, 30, 8, (e, v) => e.DotSize = v)
    ];

    public int DotSize { get; set; } = 8;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        int size = Math.Max(4, DotSize);
        int w = source.Width, h = source.Height;

        SKBitmap result = new(w, h, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.White);

        // CMYK channels with different rotation angles
        (SKColor color, float angle)[] channels =
        [
            (new SKColor(0, 255, 255), 15f),    // Cyan
            (new SKColor(255, 0, 255), 75f),    // Magenta
            (new SKColor(255, 255, 0), 0f),     // Yellow
            (new SKColor(0, 0, 0), 45f)         // Key (black)
        ];

        SKColor[] src = source.Pixels;

        foreach (var (color, angle) in channels)
        {
            using SKPaint paint = new()
            {
                Color = color,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                BlendMode = SKBlendMode.Multiply
            };

            float rad = angle * MathF.PI / 180f;
            float cosA = MathF.Cos(rad), sinA = MathF.Sin(rad);

            for (int gy = -size; gy < h + size * 2; gy += size)
            {
                for (int gx = -size; gx < w + size * 2; gx += size)
                {
                    // Rotate grid
                    float rx = gx * cosA - gy * sinA;
                    float ry = gx * sinA + gy * cosA;

                    // Sample original at this point
                    int sx = Math.Clamp((int)rx, 0, w - 1);
                    int sy = Math.Clamp((int)ry, 0, h - 1);

                    // Use unrotated position for sampling instead
                    sx = Math.Clamp(gx, 0, w - 1);
                    sy = Math.Clamp(gy, 0, h - 1);
                    SKColor sc = src[sy * w + sx];

                    float channelValue = color.Red == 0 && color.Green == 0 && color.Blue == 0
                        ? 1f - (0.2126f * sc.Red + 0.7152f * sc.Green + 0.0722f * sc.Blue) / 255f
                        : color.Red == 0
                            ? 1f - sc.Red / 255f
                            : color.Green == 0
                                ? 1f - sc.Green / 255f
                                : 1f - sc.Blue / 255f;

                    float dotRadius = size * 0.5f * channelValue;
                    if (dotRadius > 0.5f)
                    {
                        canvas.DrawCircle(gx, gy, dotRadius, paint);
                    }
                }
            }
        }

        return result;
    }
}