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

public sealed class VignetteImageEffect : ImageEffectBase
{
    public override string Id => "vignette";
    public override string Name => "Vignette";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.circle_dashed;
    public override string Description => "Applies a vignette effect.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<VignetteImageEffect>("strength", "Strength", 0, 1, 0.5, (effect, value) => effect.Strength = value, tickFrequency: 0.05, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.00}"),
        EffectParameters.FloatSlider<VignetteImageEffect>("radius", "Radius", 0.05, 0.999, 0.75, (effect, value) => effect.Radius = value, tickFrequency: 0.01, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.00}")
    ];

    public float Strength { get; set; } = 0.5f;
    public float Radius { get; set; } = 0.75f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float strength = Math.Clamp(Strength, 0f, 1f);
        float radius = Math.Clamp(Radius, 0.05f, 0.999f);
        if (strength <= 0f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;

        float cx = (width - 1) * 0.5f;
        float cy = (height - 1) * 0.5f;
        float invCx = cx > 0f ? 1f / cx : 1f;
        float invCy = cy > 0f ? 1f / cy : 1f;
        const float invSqrt2 = 0.70710678f;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            float dy = (y - cy) * invCy;

            for (int x = 0; x < width; x++)
            {
                float dx = (x - cx) * invCx;
                float distance01 = MathF.Sqrt((dx * dx) + (dy * dy)) * invSqrt2;

                float t = (distance01 - radius) / (1f - radius);
                if (t < 0f) t = 0f;
                if (t > 1f) t = 1f;

                // Smoothstep falloff for soft edge transition.
                float falloff = t * t * (3f - (2f * t));
                float factor = 1f - (strength * falloff);

                SKColor c = srcPixels[row + x];
                byte r = ClampToByte(c.Red * factor);
                byte g = ClampToByte(c.Green * factor);
                byte b = ClampToByte(c.Blue * factor);
                dstPixels[row + x] = new SKColor(r, g, b, c.Alpha);
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static byte ClampToByte(float value)
    {
        if (value <= 0f) return 0;
        if (value >= 255f) return 255;
        return (byte)MathF.Round(value);
    }
}