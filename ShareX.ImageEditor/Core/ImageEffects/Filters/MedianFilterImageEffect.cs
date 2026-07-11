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

public sealed class MedianFilterImageEffect : ImageEffectBase
{
    public override string Id => "median_filter";
    public override string Name => "Median filter";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.filter;
    public override string Description => "Applies a median filter to reduce noise while preserving edges.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<MedianFilterImageEffect>("radius", "Radius", 1, 5, 1, (e, v) => e.Radius = v),
    ];

    public int Radius { get; set; } = 1;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int radius = Math.Clamp(Radius, 1, 5);
        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        int diameter = (radius * 2) + 1;
        int maxSamples = diameter * diameter;

        byte[] rValues = new byte[maxSamples];
        byte[] gValues = new byte[maxSamples];
        byte[] bValues = new byte[maxSamples];
        byte[] aValues = new byte[maxSamples];

        for (int y = 0; y < height; y++)
        {
            int dstRow = y * width;

            for (int x = 0; x < width; x++)
            {
                int count = 0;

                for (int ky = -radius; ky <= radius; ky++)
                {
                    int sy = Clamp(y + ky, 0, bottom);
                    int srcRow = sy * width;

                    for (int kx = -radius; kx <= radius; kx++)
                    {
                        int sx = Clamp(x + kx, 0, right);
                        SKColor c = srcPixels[srcRow + sx];

                        rValues[count] = c.Red;
                        gValues[count] = c.Green;
                        bValues[count] = c.Blue;
                        aValues[count] = c.Alpha;
                        count++;
                    }
                }

                Array.Sort(rValues, 0, count);
                Array.Sort(gValues, 0, count);
                Array.Sort(bValues, 0, count);
                Array.Sort(aValues, 0, count);

                int mid = count >> 1;
                dstPixels[dstRow + x] = new SKColor(
                    rValues[mid],
                    gValues[mid],
                    bValues[mid],
                    aValues[mid]);
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