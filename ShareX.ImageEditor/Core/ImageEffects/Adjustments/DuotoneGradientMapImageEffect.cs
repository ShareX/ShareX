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
using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Adjustments;

public sealed class DuotoneGradientMapImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "duotone_gradient_map";
    public override string Name => "Duotone / Gradient map";
    public override string IconKey => LucideIcons.blend;
    public override string Description => "Maps grayscale tones to a custom multi-color gradient.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Color<DuotoneGradientMapImageEffect>("shadow_color", "Shadow color", new SKColor(24, 28, 78), (effect, value) => effect.ShadowColor = value),
        EffectParameters.Color<DuotoneGradientMapImageEffect>("midtone_color", "Midtone color", new SKColor(182, 60, 132), (effect, value) => effect.MidtoneColor = value),
        EffectParameters.Color<DuotoneGradientMapImageEffect>("highlight_color", "Highlight color", new SKColor(255, 224, 132), (effect, value) => effect.HighlightColor = value),
        EffectParameters.FloatSlider<DuotoneGradientMapImageEffect>("contrast", "Contrast", 50, 200, 110, (effect, value) => effect.Contrast = value),
        EffectParameters.FloatSlider<DuotoneGradientMapImageEffect>("gamma", "Gamma", 0.1, 5, 1, (effect, value) => effect.Gamma = value, tickFrequency: 0.1, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.0}"),
        EffectParameters.FloatSlider<DuotoneGradientMapImageEffect>("blend", "Blend", 0, 100, 100, (effect, value) => effect.Blend = value)
    ];

    public SKColor ShadowColor { get; set; } = new SKColor(24, 28, 78);
    public SKColor MidtoneColor { get; set; } = new SKColor(182, 60, 132);
    public SKColor HighlightColor { get; set; } = new SKColor(255, 224, 132);

    public float Contrast { get; set; } = 110f; // 50..200
    public float Gamma { get; set; } = 1f; // 0.3..3
    public float Blend { get; set; } = 100f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float contrast = Math.Clamp(Contrast, 50f, 200f) / 100f;
        float gamma = Math.Clamp(Gamma, 0.3f, 3f);
        float blend = Math.Clamp(Blend, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        for (int i = 0; i < srcPixels.Length; i++)
        {
            SKColor src = srcPixels[i];
            float lum = ((0.2126f * src.Red) + (0.7152f * src.Green) + (0.0722f * src.Blue)) / 255f;
            lum = Math.Clamp(((lum - 0.5f) * contrast) + 0.5f, 0f, 1f);
            lum = MathF.Pow(lum, 1f / gamma);

            SKColor mapped = SampleThreeColorGradient(lum, ShadowColor, MidtoneColor, HighlightColor);

            float r = ProceduralEffectHelper.Lerp(src.Red, mapped.Red, blend);
            float g = ProceduralEffectHelper.Lerp(src.Green, mapped.Green, blend);
            float b = ProceduralEffectHelper.Lerp(src.Blue, mapped.Blue, blend);

            dstPixels[i] = new SKColor(
                ProceduralEffectHelper.ClampToByte(r),
                ProceduralEffectHelper.ClampToByte(g),
                ProceduralEffectHelper.ClampToByte(b),
                src.Alpha);
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static SKColor SampleThreeColorGradient(float t, SKColor shadows, SKColor mids, SKColor highs)
    {
        t = Math.Clamp(t, 0f, 1f);
        if (t <= 0.5f)
        {
            return Lerp(shadows, mids, t * 2f);
        }

        return Lerp(mids, highs, (t - 0.5f) * 2f);
    }

    private static SKColor Lerp(SKColor a, SKColor b, float t)
    {
        return new SKColor(
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(a.Red, b.Red, t)),
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(a.Green, b.Green, t)),
            ProceduralEffectHelper.ClampToByte(ProceduralEffectHelper.Lerp(a.Blue, b.Blue, t)),
            255);
    }
}