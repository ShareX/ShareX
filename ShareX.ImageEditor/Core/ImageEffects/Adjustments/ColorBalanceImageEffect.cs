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

public sealed class ColorBalanceImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "color_balance";
    public override string Name => "Color Balance";
    public override string IconKey => LucideIcons.sliders_horizontal;
    public override string Description => "Adjusts color balance in shadows, midtones, and highlights.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<ColorBalanceImageEffect>("shadow_cyan_red", "Shadows: Cyan-Red", -100, 100, 0, (e, v) => e.ShadowCyanRed = v),
        EffectParameters.FloatSlider<ColorBalanceImageEffect>("shadow_magenta_green", "Shadows: Magenta-Green", -100, 100, 0, (e, v) => e.ShadowMagentaGreen = v),
        EffectParameters.FloatSlider<ColorBalanceImageEffect>("shadow_yellow_blue", "Shadows: Yellow-Blue", -100, 100, 0, (e, v) => e.ShadowYellowBlue = v),
        EffectParameters.FloatSlider<ColorBalanceImageEffect>("mid_cyan_red", "Midtones: Cyan-Red", -100, 100, 0, (e, v) => e.MidCyanRed = v),
        EffectParameters.FloatSlider<ColorBalanceImageEffect>("mid_magenta_green", "Midtones: Magenta-Green", -100, 100, 0, (e, v) => e.MidMagentaGreen = v),
        EffectParameters.FloatSlider<ColorBalanceImageEffect>("mid_yellow_blue", "Midtones: Yellow-Blue", -100, 100, 0, (e, v) => e.MidYellowBlue = v),
        EffectParameters.FloatSlider<ColorBalanceImageEffect>("highlight_cyan_red", "Highlights: Cyan-Red", -100, 100, 0, (e, v) => e.HighlightCyanRed = v),
        EffectParameters.FloatSlider<ColorBalanceImageEffect>("highlight_magenta_green", "Highlights: Magenta-Green", -100, 100, 0, (e, v) => e.HighlightMagentaGreen = v),
        EffectParameters.FloatSlider<ColorBalanceImageEffect>("highlight_yellow_blue", "Highlights: Yellow-Blue", -100, 100, 0, (e, v) => e.HighlightYellowBlue = v)
    ];

    public float ShadowCyanRed { get; set; }
    public float ShadowMagentaGreen { get; set; }
    public float ShadowYellowBlue { get; set; }
    public float MidCyanRed { get; set; }
    public float MidMagentaGreen { get; set; }
    public float MidYellowBlue { get; set; }
    public float HighlightCyanRed { get; set; }
    public float HighlightMagentaGreen { get; set; }
    public float HighlightYellowBlue { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        bool allZero =
            Math.Abs(ShadowCyanRed) < 0.01f && Math.Abs(ShadowMagentaGreen) < 0.01f && Math.Abs(ShadowYellowBlue) < 0.01f &&
            Math.Abs(MidCyanRed) < 0.01f && Math.Abs(MidMagentaGreen) < 0.01f && Math.Abs(MidYellowBlue) < 0.01f &&
            Math.Abs(HighlightCyanRed) < 0.01f && Math.Abs(HighlightMagentaGreen) < 0.01f && Math.Abs(HighlightYellowBlue) < 0.01f;

        if (allZero) return source.Copy();

        // Normalize to -1..1 range
        float sCR = ShadowCyanRed / 100f;
        float sMG = ShadowMagentaGreen / 100f;
        float sYB = ShadowYellowBlue / 100f;
        float mCR = MidCyanRed / 100f;
        float mMG = MidMagentaGreen / 100f;
        float mYB = MidYellowBlue / 100f;
        float hCR = HighlightCyanRed / 100f;
        float hMG = HighlightMagentaGreen / 100f;
        float hYB = HighlightYellowBlue / 100f;

        return ApplyPixelOperation(source, c =>
        {
            float r = c.Red / 255f;
            float g = c.Green / 255f;
            float b = c.Blue / 255f;

            // Compute luminance for tonal range weighting
            float lum = 0.299f * r + 0.587f * g + 0.114f * b;

            // Shadow weight: strongest at darks, fades to zero at midtones
            float shadowWeight = Math.Clamp(1f - (lum * 4f), 0f, 1f);
            // Highlight weight: strongest at lights, fades to zero at midtones
            float highlightWeight = Math.Clamp((lum - 0.75f) * 4f, 0f, 1f);
            // Midtone weight: strongest in the middle
            float midWeight = 1f - shadowWeight - highlightWeight;
            midWeight = Math.Max(midWeight, 0f);

            // Cyan-Red: positive = more red, negative = more cyan (less red)
            float rShift = (sCR * shadowWeight + mCR * midWeight + hCR * highlightWeight) * 0.5f;
            // Magenta-Green: positive = more green, negative = more magenta (less green)
            float gShift = (sMG * shadowWeight + mMG * midWeight + hMG * highlightWeight) * 0.5f;
            // Yellow-Blue: positive = more blue, negative = more yellow (less blue)
            float bShift = (sYB * shadowWeight + mYB * midWeight + hYB * highlightWeight) * 0.5f;

            r = Math.Clamp(r + rShift, 0f, 1f);
            g = Math.Clamp(g + gShift, 0f, 1f);
            b = Math.Clamp(b + bShift, 0f, 1f);

            return new SKColor(
                (byte)MathF.Round(r * 255f),
                (byte)MathF.Round(g * 255f),
                (byte)MathF.Round(b * 255f),
                c.Alpha);
        });
    }
}