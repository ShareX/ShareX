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

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public enum WaveDirection
{
    Horizontal,
    Vertical,
    Both
}

public sealed class WaveDistortionImageEffect : ImageEffectBase
{
    public override string Id => "wave_distortion";
    public override string Name => "Wave distortion";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.waves;
    public override string Description => "Applies a sine wave distortion to the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<WaveDistortionImageEffect>("amplitude", "Amplitude", 1, 100, 20, (e, v) => e.Amplitude = v),
        EffectParameters.IntSlider<WaveDistortionImageEffect>("wavelength", "Wavelength", 10, 500, 100, (e, v) => e.Wavelength = v),
        EffectParameters.Enum<WaveDistortionImageEffect, WaveDirection>(
            "direction", "Direction", WaveDirection.Horizontal, (e, v) => e.Direction = v,
            new (string, WaveDirection)[]
            {
                ("Horizontal", WaveDirection.Horizontal),
                ("Vertical", WaveDirection.Vertical),
                ("Both", WaveDirection.Both)
            })
    ];

    public int Amplitude { get; set; } = 20;
    public int Wavelength { get; set; } = 100;
    public WaveDirection Direction { get; set; } = WaveDirection.Horizontal;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Amplitude <= 0) return source.Copy();

        int w = source.Width, h = source.Height;
        SKColor[] src = source.Pixels;
        SKColor[] dst = new SKColor[src.Length];
        float freq = MathF.PI * 2f / Math.Max(10, Wavelength);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int sx = x, sy = y;

                if (Direction is WaveDirection.Horizontal or WaveDirection.Both)
                    sx = x + (int)(Amplitude * MathF.Sin(y * freq));

                if (Direction is WaveDirection.Vertical or WaveDirection.Both)
                    sy = y + (int)(Amplitude * MathF.Sin(x * freq));

                if (sx >= 0 && sx < w && sy >= 0 && sy < h)
                    dst[y * w + x] = src[sy * w + sx];
                else
                    dst[y * w + x] = SKColors.Transparent;
            }
        }

        return new SKBitmap(w, h, source.ColorType, source.AlphaType) { Pixels = dst };
    }
}