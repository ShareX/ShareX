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

public sealed class FilmGrainImageEffect : ImageEffectBase
{
    public override string Id => "film_grain";
    public override string Name => "Film grain";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.film;
    public override string Description => "Adds organic film-like grain texture.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<FilmGrainImageEffect>("amount", "Amount", 1, 100, 30, (e, v) => e.Amount = v),
        EffectParameters.Bool<FilmGrainImageEffect>("monochrome", "Monochrome", true, (e, v) => e.Monochrome = v)
    ];

    public int Amount { get; set; } = 30;
    public bool Monochrome { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int w = source.Width, h = source.Height;
        SKColor[] src = source.Pixels;
        SKColor[] dst = new SKColor[src.Length];
        Random rng = new(42);

        int range = (int)(Amount * 2.55f);

        for (int i = 0; i < src.Length; i++)
        {
            SKColor c = src[i];
            if (Monochrome)
            {
                int noise = rng.Next(-range, range + 1);
                dst[i] = new SKColor(
                    ClampByte(c.Red + noise),
                    ClampByte(c.Green + noise),
                    ClampByte(c.Blue + noise),
                    c.Alpha);
            }
            else
            {
                dst[i] = new SKColor(
                    ClampByte(c.Red + rng.Next(-range, range + 1)),
                    ClampByte(c.Green + rng.Next(-range, range + 1)),
                    ClampByte(c.Blue + rng.Next(-range, range + 1)),
                    c.Alpha);
            }
        }

        return new SKBitmap(w, h, source.ColorType, source.AlphaType) { Pixels = dst };
    }

    private static byte ClampByte(int value) => (byte)Math.Clamp(value, 0, 255);
}