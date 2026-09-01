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

public sealed class OilPaintImageEffect : ImageEffectBase
{
    public override string Id => "oil_paint";
    public override string Name => "Oil paint";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.paintbrush_vertical;
    public override string Description => "Simulates an oil painting by quantizing color intensity levels.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<OilPaintImageEffect>("radius", "Radius", 1, 6, 3, (e, v) => e.Radius = v),
        EffectParameters.IntSlider<OilPaintImageEffect>("levels", "Levels", 8, 64, 24, (e, v) => e.Levels = v),
    ];

    public int Radius { get; set; } = 3;
    public int Levels { get; set; } = 24;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int radius = Math.Clamp(Radius, 1, 6);
        int levels = Math.Clamp(Levels, 8, 64);

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        int[] count = new int[64];
        int[] sumR = new int[64];
        int[] sumG = new int[64];
        int[] sumB = new int[64];
        int[] sumA = new int[64];

        for (int y = 0; y < height; y++)
        {
            int dstRow = y * width;

            for (int x = 0; x < width; x++)
            {
                Array.Clear(count, 0, levels);
                Array.Clear(sumR, 0, levels);
                Array.Clear(sumG, 0, levels);
                Array.Clear(sumB, 0, levels);
                Array.Clear(sumA, 0, levels);

                for (int ky = -radius; ky <= radius; ky++)
                {
                    int sy = Clamp(y + ky, 0, bottom);
                    int srcRow = sy * width;

                    for (int kx = -radius; kx <= radius; kx++)
                    {
                        int sx = Clamp(x + kx, 0, right);
                        SKColor c = srcPixels[srcRow + sx];

                        int intensity = ((c.Red + c.Green + c.Blue) * levels) / 768;
                        if (intensity >= levels) intensity = levels - 1;

                        count[intensity]++;
                        sumR[intensity] += c.Red;
                        sumG[intensity] += c.Green;
                        sumB[intensity] += c.Blue;
                        sumA[intensity] += c.Alpha;
                    }
                }

                int best = 0;
                for (int i = 1; i < levels; i++)
                {
                    if (count[i] > count[best])
                    {
                        best = i;
                    }
                }

                int bestCount = Math.Max(1, count[best]);
                dstPixels[dstRow + x] = new SKColor(
                    (byte)(sumR[best] / bestCount),
                    (byte)(sumG[best] / bestCount),
                    (byte)(sumB[best] / bestCount),
                    (byte)(sumA[best] / bestCount));
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}