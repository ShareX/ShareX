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

public sealed class RGBSplitImageEffect : ImageEffectBase
{
    public override string Id => "rgb_split";
    public override string Name => "RGB split";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.split;
    public override string Description => "Splits and offsets the red, green, and blue color channels.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<RGBSplitImageEffect>("offset_red_x", "Offset red X", -100, 100, -5, (e, v) => e.OffsetRedX = v),
        EffectParameters.IntSlider<RGBSplitImageEffect>("offset_red_y", "Offset red Y", -100, 100, 0, (e, v) => e.OffsetRedY = v),
        EffectParameters.IntSlider<RGBSplitImageEffect>("offset_green_x", "Offset green X", -100, 100, 0, (e, v) => e.OffsetGreenX = v),
        EffectParameters.IntSlider<RGBSplitImageEffect>("offset_green_y", "Offset green Y", -100, 100, 0, (e, v) => e.OffsetGreenY = v),
        EffectParameters.IntSlider<RGBSplitImageEffect>("offset_blue_x", "Offset blue X", -100, 100, 5, (e, v) => e.OffsetBlueX = v),
        EffectParameters.IntSlider<RGBSplitImageEffect>("offset_blue_y", "Offset blue Y", -100, 100, 0, (e, v) => e.OffsetBlueY = v),
    ];

    public int OffsetRedX { get; set; } = -5;
    public int OffsetRedY { get; set; }

    public int OffsetGreenX { get; set; }
    public int OffsetGreenY { get; set; }

    public int OffsetBlueX { get; set; } = 5;
    public int OffsetBlueY { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        // Precompute clamped source X indices for each destination X per channel.
        int[] xRedMap = new int[width];
        int[] xGreenMap = new int[width];
        int[] xBlueMap = new int[width];

        for (int x = 0; x < width; x++)
        {
            xRedMap[x] = Clamp(x - OffsetRedX, 0, right);
            xGreenMap[x] = Clamp(x - OffsetGreenX, 0, right);
            xBlueMap[x] = Clamp(x - OffsetBlueX, 0, right);
        }

        // Precompute source row starts for each destination Y per channel.
        int[] rowRedMap = new int[height];
        int[] rowGreenMap = new int[height];
        int[] rowBlueMap = new int[height];

        for (int y = 0; y < height; y++)
        {
            rowRedMap[y] = Clamp(y - OffsetRedY, 0, bottom) * width;
            rowGreenMap[y] = Clamp(y - OffsetGreenY, 0, bottom) * width;
            rowBlueMap[y] = Clamp(y - OffsetBlueY, 0, bottom) * width;
        }

        for (int y = 0; y < height; y++)
        {
            int dstRow = y * width;
            int redRow = rowRedMap[y];
            int greenRow = rowGreenMap[y];
            int blueRow = rowBlueMap[y];

            for (int x = 0; x < width; x++)
            {
                SKColor colorR = srcPixels[redRow + xRedMap[x]];
                SKColor colorG = srcPixels[greenRow + xGreenMap[x]];
                SKColor colorB = srcPixels[blueRow + xBlueMap[x]];

                byte red = (byte)(colorR.Red * colorR.Alpha / 255);
                byte green = (byte)(colorG.Green * colorG.Alpha / 255);
                byte blue = (byte)(colorB.Blue * colorB.Alpha / 255);
                byte alpha = (byte)((colorR.Alpha + colorG.Alpha + colorB.Alpha) / 3);

                dstPixels[dstRow + x] = new SKColor(red, green, blue, alpha);
            }
        }

        SKBitmap result = new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        return result;
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}