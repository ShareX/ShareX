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

public sealed class NeonGlowBorderImageEffect : ImageEffectBase
{
    public override string Id => "neon_glow_border";
    public override string Name => "Neon glow border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.zap;
    public override string Description => "Adds a vibrant neon light tube border with a diffused outer glow on a dark background.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<NeonGlowBorderImageEffect>("border_size", "Border size", 10, 200, 50, (e, v) => e.BorderSize = v),
        EffectParameters.Color<NeonGlowBorderImageEffect>("neon_color", "Neon color", new SKColor(0, 255, 200), (e, v) => e.NeonColor = v),
        EffectParameters.FloatSlider<NeonGlowBorderImageEffect>("glow_intensity", "Glow intensity", 10, 100, 75, (e, v) => e.GlowIntensity = v),
        EffectParameters.FloatSlider<NeonGlowBorderImageEffect>("tube_width", "Tube width", 1, 20, 4, (e, v) => e.TubeWidth = v),
        EffectParameters.Color<NeonGlowBorderImageEffect>("bg_color", "Background color", new SKColor(10, 10, 18), (e, v) => e.BgColor = v),
        EffectParameters.Bool<NeonGlowBorderImageEffect>("double_tube", "Double tube", false, (e, v) => e.DoubleTube = v)
    ];

    public int BorderSize { get; set; } = 50;
    public SKColor NeonColor { get; set; } = new SKColor(0, 255, 200);
    public float GlowIntensity { get; set; } = 75f;
    public float TubeWidth { get; set; } = 4f;
    public SKColor BgColor { get; set; } = new SKColor(10, 10, 18);
    public bool DoubleTube { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 10, 200);
        float glow = Math.Clamp(GlowIntensity, 10f, 100f) / 100f;
        float tube = Math.Clamp(TubeWidth, 1f, 20f);

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(BgColor);

        // Source image
        canvas.DrawBitmap(source, border, border);

        // Neon tube position: centered in the border band
        float tubeOffset = border * 0.5f;
        SKRect tubeRect = new(tubeOffset, tubeOffset, newWidth - tubeOffset, newHeight - tubeOffset);

        // Outer glow (large blur)
        float glowSigma = border * 0.35f * glow;
        using (SKPaint outerGlow = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = tube * 5f,
            Color = NeonColor.WithAlpha((byte)(120 * glow)),
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, glowSigma)
        })
        {
            canvas.DrawRect(tubeRect, outerGlow);
        }

        // Mid glow (medium blur)
        using (SKPaint midGlow = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = tube * 3f,
            Color = NeonColor.WithAlpha((byte)(180 * glow)),
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, glowSigma * 0.35f)
        })
        {
            canvas.DrawRect(tubeRect, midGlow);
        }

        // Core tube (sharp bright line)
        using (SKPaint corePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = tube,
            Color = SKColors.White.WithAlpha((byte)(220 * glow))
        })
        {
            canvas.DrawRect(tubeRect, corePaint);
        }

        // Bright color tube on top of white core
        using (SKPaint tubePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = tube * 1.4f,
            Color = NeonColor.WithAlpha((byte)(240 * glow))
        })
        {
            canvas.DrawRect(tubeRect, tubePaint);
        }

        // Second inner tube if requested
        if (DoubleTube)
        {
            float innerOffset = border * 0.78f;
            SKRect innerTubeRect = new(innerOffset, innerOffset, newWidth - innerOffset, newHeight - innerOffset);

            using SKPaint innerGlow = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = tube * 2.5f,
                Color = NeonColor.WithAlpha((byte)(100 * glow)),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, glowSigma * 0.25f)
            };
            canvas.DrawRect(innerTubeRect, innerGlow);

            using SKPaint innerCore = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = tube * 0.8f,
                Color = NeonColor.WithAlpha((byte)(200 * glow))
            };
            canvas.DrawRect(innerTubeRect, innerCore);
        }

        return result;
    }
}