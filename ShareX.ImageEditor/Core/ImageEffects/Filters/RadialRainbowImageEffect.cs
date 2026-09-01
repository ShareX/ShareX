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

public sealed class RadialRainbowImageEffect : ImageEffectBase
{
    public override string Id => "radial_rainbow";
    public override string Name => "Radial rainbow";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.rainbow;
    public override string Description => "Applies a rainbow hue shift based on angle from center.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<RadialRainbowImageEffect>("strength", "Strength", 0, 100, 50, (e, v) => e.Strength = v),
        EffectParameters.IntSlider<RadialRainbowImageEffect>("rotations", "Rotations", 1, 5, 1, (e, v) => e.Rotations = v)
    ];

    public float Strength { get; set; } = 50f;
    public int Rotations { get; set; } = 1;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        float s = Math.Clamp(Strength / 100f, 0f, 1f);
        if (s <= 0f) return source.Copy();

        int w = source.Width, h = source.Height;
        float cx = w / 2f, cy = h / 2f;
        int rots = Math.Max(1, Rotations);
        SKColor[] src = source.Pixels;
        SKColor[] dst = new SKColor[src.Length];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float angle = MathF.Atan2(y - cy, x - cx);
                float hueShift = ((angle + MathF.PI) / (2f * MathF.PI)) * 360f * rots;

                SKColor c = src[y * w + x];
                c.ToHsl(out float hue, out float sat, out float lum);
                float newHue = (hue + hueShift * s) % 360f;
                if (newHue < 0) newHue += 360f;

                dst[y * w + x] = SKColor.FromHsl(newHue, sat, lum, c.Alpha);
            }
        }

        return new SKBitmap(w, h, source.ColorType, source.AlphaType) { Pixels = dst };
    }
}