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

public sealed class ColorDepthImageEffect : ImageEffectBase
{
    public override string Id => "color_depth";
    public override string Name => "Color depth";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.layers_2;
    public override string Description => "Reduces the number of bits per color channel.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<ColorDepthImageEffect>("bitsPerChannel", "Bits per channel", 1, 8, 4, (e, v) => e.BitsPerChannel = v),
    ];

    public int BitsPerChannel { get; set; } = 4;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int bits = Math.Clamp(BitsPerChannel, 1, 8);
        if (bits == 8)
        {
            return source.Copy();
        }

        double colorsPerChannel = Math.Pow(2, bits);
        double interval = 255d / (colorsPerChannel - 1d);

        static byte Remap(byte color, double remapInterval)
        {
            return (byte)Math.Round(Math.Round(color / remapInterval) * remapInterval);
        }

        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        for (int i = 0; i < srcPixels.Length; i++)
        {
            SKColor src = srcPixels[i];
            dstPixels[i] = new SKColor(
                Remap(src.Red, interval),
                Remap(src.Green, interval),
                Remap(src.Blue, interval),
                src.Alpha);
        }

        result.Pixels = dstPixels;
        return result;
    }
}