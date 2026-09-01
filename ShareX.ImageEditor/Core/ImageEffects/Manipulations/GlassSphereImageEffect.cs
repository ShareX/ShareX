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

public sealed class GlassSphereImageEffect : ImageEffectBase
{
    public override string Id => "glass_sphere";
    public override string Name => "Glass sphere";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.globe;
    public override string Description => "Applies a spherical glass lens distortion at the center.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<GlassSphereImageEffect>("radius", "Radius %", 10, 100, 50, (e, v) => e.RadiusPercent = v),
        EffectParameters.FloatSlider<GlassSphereImageEffect>("refraction", "Refraction", 0.1, 2.0, 1.5,
            (e, v) => e.Refraction = v, tickFrequency: 0.05, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.00}")
    ];

    public float RadiusPercent { get; set; } = 50f;
    public float Refraction { get; set; } = 1.5f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int w = source.Width, h = source.Height;
        float cx = w / 2f, cy = h / 2f;
        float radius = Math.Min(w, h) * RadiusPercent / 200f;
        float radius2 = radius * radius;

        SKColor[] src = source.Pixels;
        SKColor[] dst = (SKColor[])src.Clone();

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                float dist2 = dx * dx + dy * dy;

                if (dist2 >= radius2) continue;

                float dist = MathF.Sqrt(dist2);
                float normalizedDist = dist / radius;

                // Spherical refraction
                float theta = MathF.Asin(normalizedDist);
                float newDist = radius * MathF.Sin(theta * Refraction) / MathF.Sin(theta + 0.0001f);

                float ratio = dist > 0.0001f ? newDist / dist : 1f;
                int sx = (int)(cx + dx * ratio);
                int sy = (int)(cy + dy * ratio);

                if (sx >= 0 && sx < w && sy >= 0 && sy < h)
                    dst[y * w + x] = src[sy * w + sx];
            }
        }

        return new SKBitmap(w, h, source.ColorType, source.AlphaType) { Pixels = dst };
    }
}