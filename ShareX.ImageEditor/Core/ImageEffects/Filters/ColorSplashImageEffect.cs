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

public sealed class ColorSplashImageEffect : ImageEffectBase
{
    public override string Id => "color_splash";
    public override string Name => "Color splash";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.droplets;
    public override string Description => "Keeps one color and desaturates the rest of the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<ColorSplashImageEffect>("hue", "Target hue", 0, 360, 0, (e, v) => e.TargetHue = v),
        EffectParameters.IntSlider<ColorSplashImageEffect>("tolerance", "Tolerance", 1, 180, 30, (e, v) => e.Tolerance = v)
    ];

    public int TargetHue { get; set; } = 0;
    public int Tolerance { get; set; } = 30;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int w = source.Width, h = source.Height;
        SKColor[] src = source.Pixels;
        SKColor[] dst = new SKColor[src.Length];

        float targetH = TargetHue;
        float tol = Tolerance;

        for (int i = 0; i < src.Length; i++)
        {
            SKColor c = src[i];
            c.ToHsl(out float hue, out float sat, out float lum);

            float diff = MathF.Abs(hue - targetH);
            if (diff > 180f) diff = 360f - diff;

            if (diff <= tol && sat > 5f)
            {
                dst[i] = c; // Keep original color
            }
            else
            {
                // Desaturate
                byte gray = (byte)(0.2126f * c.Red + 0.7152f * c.Green + 0.0722f * c.Blue);
                dst[i] = new SKColor(gray, gray, gray, c.Alpha);
            }
        }

        return new SKBitmap(w, h, source.ColorType, source.AlphaType) { Pixels = dst };
    }
}