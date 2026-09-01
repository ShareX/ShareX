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

public sealed class SplitToningImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "split_toning";
    public override string Name => "Split Toning";
    public override string IconKey => LucideIcons.blend;
    public override string Description => "Tints shadows and highlights with different colors.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Color<SplitToningImageEffect>("shadow_tint", "Shadow Tint", new SKColor(40, 50, 120, 255), (e, v) => e.ShadowTint = v),
        EffectParameters.Color<SplitToningImageEffect>("highlight_tint", "Highlight Tint", new SKColor(200, 170, 100, 255), (e, v) => e.HighlightTint = v),
        EffectParameters.FloatSlider<SplitToningImageEffect>("strength", "Strength", 0, 100, 30, (e, v) => e.Strength = v),
        EffectParameters.FloatSlider<SplitToningImageEffect>("balance", "Balance", -100, 100, 0, (e, v) => e.Balance = v)
    ];

    public SKColor ShadowTint { get; set; } = new SKColor(40, 50, 120, 255);
    public SKColor HighlightTint { get; set; } = new SKColor(200, 170, 100, 255);

    /// <summary>Strength of the tinting effect (0-100).</summary>
    public float Strength { get; set; } = 30f;

    /// <summary>
    /// Shifts the midpoint between shadows and highlights.
    /// Negative pushes more toward shadow tint, positive toward highlight tint.
    /// Range: -100 to 100.
    /// </summary>
    public float Balance { get; set; } = 0f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float strength = Math.Clamp(Strength, 0f, 100f) / 100f;
        if (strength < 0.001f)
        {
            return source.Copy();
        }

        float balance = Math.Clamp(Balance, -100f, 100f) / 200f + 0.5f; // Remap to 0..1
        float sR = ShadowTint.Red / 255f;
        float sG = ShadowTint.Green / 255f;
        float sB = ShadowTint.Blue / 255f;
        float hR = HighlightTint.Red / 255f;
        float hG = HighlightTint.Green / 255f;
        float hB = HighlightTint.Blue / 255f;

        return ApplyPixelOperation(source, c =>
        {
            float r = c.Red / 255f;
            float g = c.Green / 255f;
            float b = c.Blue / 255f;

            // Luminance determines shadow/highlight region.
            float lum = 0.299f * r + 0.587f * g + 0.114f * b;

            // Shadow weight: smoothstep from 1 at black to 0 at balance point.
            float shadowWeight = 1f - SmoothStep(0f, balance, lum);
            // Highlight weight: smoothstep from 0 at balance point to 1 at white.
            float highlightWeight = SmoothStep(balance, 1f, lum);

            // Blend the tint colors weighted by their region.
            float tintR = sR * shadowWeight + hR * highlightWeight;
            float tintG = sG * shadowWeight + hG * highlightWeight;
            float tintB = sB * shadowWeight + hB * highlightWeight;

            // Apply tint using soft light blending.
            r = Lerp(r, SoftLight(r, tintR), strength);
            g = Lerp(g, SoftLight(g, tintG), strength);
            b = Lerp(b, SoftLight(b, tintB), strength);

            return new SKColor(ClampToByte(r * 255f), ClampToByte(g * 255f), ClampToByte(b * 255f), c.Alpha);
        });
    }

    private static float SoftLight(float @base, float blend)
    {
        // Pegtop soft light formula.
        return (1f - 2f * blend) * @base * @base + 2f * blend * @base;
    }

    private static float SmoothStep(float edge0, float edge1, float x)
    {
        float range = edge1 - edge0;
        if (range <= 0f) return x >= edge1 ? 1f : 0f;
        float t = Math.Clamp((x - edge0) / range, 0f, 1f);
        return t * t * (3f - 2f * t);
    }

    private static float Lerp(float a, float b, float t) => a + (b - a) * t;

    private static byte ClampToByte(float value)
    {
        if (value <= 0f) return 0;
        if (value >= 255f) return 255;
        return (byte)MathF.Round(value);
    }
}