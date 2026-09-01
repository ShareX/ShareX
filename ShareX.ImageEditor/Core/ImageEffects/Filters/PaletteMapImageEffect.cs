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

public sealed class PaletteMapImageEffect : ImageEffectBase
{
    public override string Id => "palette_map";
    public override string Name => "Palette map";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.swatch_book;
    public override string Description => "Reduces the image to a limited color palette by quantization.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<PaletteMapImageEffect>("levels", "Color levels", 2, 32, 6, (e, v) => e.Levels = v)
    ];

    public int Levels { get; set; } = 6;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int levels = Math.Clamp(Levels, 2, 32);
        int w = source.Width, h = source.Height;
        SKColor[] src = source.Pixels;
        SKColor[] dst = new SKColor[src.Length];

        float step = 255f / (levels - 1);

        for (int i = 0; i < src.Length; i++)
        {
            SKColor c = src[i];
            byte r = Quantize(c.Red, step, levels);
            byte g = Quantize(c.Green, step, levels);
            byte b = Quantize(c.Blue, step, levels);
            dst[i] = new SKColor(r, g, b, c.Alpha);
        }

        return new SKBitmap(w, h, source.ColorType, source.AlphaType) { Pixels = dst };
    }

    private static byte Quantize(byte value, float step, int levels)
    {
        int index = (int)MathF.Round(value / step);
        index = Math.Clamp(index, 0, levels - 1);
        return (byte)Math.Clamp((int)(index * step), 0, 255);
    }
}