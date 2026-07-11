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

public sealed class MiniatureImageEffect : ImageEffectBase
{
    public override string Id => "miniature";
    public override string Name => "Miniature";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.box;
    public override string Description => "Creates a toy miniature look by blurring top and bottom with boosted saturation.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<MiniatureImageEffect>("blur_radius", "Blur radius", 2, 40, 15, (e, v) => e.BlurRadius = v),
        EffectParameters.FloatSlider<MiniatureImageEffect>("focus_position", "Focus position %", 10, 90, 50, (e, v) => e.FocusPosition = v),
        EffectParameters.FloatSlider<MiniatureImageEffect>("focus_band", "Focus band %", 5, 50, 20, (e, v) => e.FocusBand = v),
        EffectParameters.FloatSlider<MiniatureImageEffect>("saturation", "Saturation boost", 0, 50, 20, (e, v) => e.SaturationBoost = v)
    ];

    public int BlurRadius { get; set; } = 15;
    public float FocusPosition { get; set; } = 50f;
    public float FocusBand { get; set; } = 20f;
    public float SaturationBoost { get; set; } = 20f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        int radius = Math.Max(1, BlurRadius);
        int w = source.Width, h = source.Height;

        // Create blurred version
        SKBitmap blurred = new(w, h, source.ColorType, source.AlphaType);
        using (SKCanvas bc = new(blurred))
        {
            using SKPaint bp = new() { ImageFilter = SKImageFilter.CreateBlur(radius, radius) };
            bc.DrawBitmap(source, 0, 0, bp);
        }

        // Blend sharp center with blurred edges
        float focusY = h * FocusPosition / 100f;
        float bandHalf = h * FocusBand / 200f;
        float topEdge = focusY - bandHalf;
        float bottomEdge = focusY + bandHalf;

        SKColor[] sharp = source.Pixels;
        SKColor[] blur = blurred.Pixels;
        SKColor[] dst = new SKColor[sharp.Length];

        float satBoost = 1f + SaturationBoost / 100f;

        for (int y = 0; y < h; y++)
        {
            float t;
            if (y < topEdge)
                t = 1f - Math.Clamp((topEdge - y) / (topEdge + 1f), 0f, 1f);
            else if (y > bottomEdge)
                t = 1f - Math.Clamp((y - bottomEdge) / (h - bottomEdge + 1f), 0f, 1f);
            else
                t = 1f; // In focus

            // Smooth the transition
            float sharpWeight = t * t * (3f - 2f * t);

            for (int x = 0; x < w; x++)
            {
                int idx = y * w + x;
                SKColor s = sharp[idx];
                SKColor b = blur[idx];

                byte r = (byte)(s.Red * sharpWeight + b.Red * (1f - sharpWeight));
                byte g = (byte)(s.Green * sharpWeight + b.Green * (1f - sharpWeight));
                byte bv = (byte)(s.Blue * sharpWeight + b.Blue * (1f - sharpWeight));

                // Boost saturation
                SKColor mixed = new(r, g, bv, s.Alpha);
                mixed.ToHsl(out float hue, out float sat, out float lum);
                sat = Math.Clamp(sat * satBoost, 0f, 100f);
                dst[idx] = SKColor.FromHsl(hue, sat, lum, s.Alpha);
            }
        }

        blurred.Dispose();
        return new SKBitmap(w, h, source.ColorType, source.AlphaType) { Pixels = dst };
    }
}