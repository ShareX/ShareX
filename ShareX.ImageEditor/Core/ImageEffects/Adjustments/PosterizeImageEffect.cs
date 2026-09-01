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

namespace ShareX.ImageEditor.Core.ImageEffects.Adjustments;

public sealed class PosterizeImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "posterize";
    public override string Name => "Posterize";
    public override string IconKey => LucideIcons.layers_3;
    public override string Description => "Reduces the number of colors to create a poster-like effect.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<PosterizeImageEffect>("levels", "Levels", 2, 64, 8, (effect, value) => effect.Levels = value)
    ];

    public int Levels { get; set; } = 8;

    public override SKBitmap Apply(SKBitmap source)
    {
        int levels = Math.Clamp(Levels, 2, 64);
        float scale = levels - 1;

        return ApplyPixelOperation(source, c =>
        {
            byte r = Quantize(c.Red, scale);
            byte g = Quantize(c.Green, scale);
            byte b = Quantize(c.Blue, scale);
            return new SKColor(r, g, b, c.Alpha);
        });
    }

    private static byte Quantize(byte value, float scale)
    {
        float bucket = MathF.Round(value * scale / 255f);
        float mapped = bucket * 255f / scale;
        if (mapped <= 0f) return 0;
        if (mapped >= 255f) return 255;
        return (byte)MathF.Round(mapped);
    }
}