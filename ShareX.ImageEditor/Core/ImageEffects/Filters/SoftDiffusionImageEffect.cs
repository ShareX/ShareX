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

using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class SoftDiffusionImageEffect : ImageEffectBase
{
    public override string Id => "soft_diffusion";
    public override string Name => "Soft diffusion";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.circle_fading_plus;
    public override string Description => "Applies a soft diffusion glow with bloom and warmth.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<SoftDiffusionImageEffect>("amount", "Amount", 0f, 100f, 52f, (e, v) => e.Amount = v),
        EffectParameters.FloatSlider<SoftDiffusionImageEffect>("radius", "Radius", 0f, 30f, 12f, (e, v) => e.Radius = v),
        EffectParameters.FloatSlider<SoftDiffusionImageEffect>("highlight_bloom", "Highlight bloom", 0f, 100f, 40f, (e, v) => e.HighlightBloom = v),
        EffectParameters.FloatSlider<SoftDiffusionImageEffect>("contrast_softening", "Contrast softening", 0f, 100f, 28f, (e, v) => e.ContrastSoftening = v),
        EffectParameters.FloatSlider<SoftDiffusionImageEffect>("warmth", "Warmth", 0f, 100f, 22f, (e, v) => e.Warmth = v),
    ];

    public float Amount { get; set; } = 52f;
    public float Radius { get; set; } = 12f;
    public float HighlightBloom { get; set; } = 40f;
    public float ContrastSoftening { get; set; } = 28f;
    public float Warmth { get; set; } = 22f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float amount = Math.Clamp(Amount, 0f, 100f) / 100f;
        float radius = Math.Clamp(Radius, 0f, 30f);
        float bloom = Math.Clamp(HighlightBloom, 0f, 100f) / 100f;
        float softness = Math.Clamp(ContrastSoftening, 0f, 100f) / 100f;
        float warmth = Math.Clamp(Warmth, 0f, 100f) / 100f;

        if (amount <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        using SKBitmap blurBitmap = AnalogEffectHelper.CreateBlurredClamp(source, Math.Max(0.1f, radius));
        SKColor[] blurPixels = blurBitmap.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];
        float postContrast = 1f - (softness * 0.28f);

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                SKColor blur = blurPixels[row + x];

                float sr = src.Red / 255f;
                float sg = src.Green / 255f;
                float sb = src.Blue / 255f;
                float br = blur.Red / 255f;
                float bg = blur.Green / 255f;
                float bb = blur.Blue / 255f;
                float blurLum = AnalogEffectHelper.Luminance01(blur);

                float bloomMask = 0.45f + (bloom * MathF.Pow(blurLum, 1.7f) * 0.85f);
                float diffR = AnalogEffectHelper.Screen(sr, br * bloomMask * (1.02f + (warmth * 0.08f)));
                float diffG = AnalogEffectHelper.Screen(sg, bg * bloomMask * 0.96f);
                float diffB = AnalogEffectHelper.Screen(sb, bb * bloomMask * (0.92f - (warmth * 0.06f)));

                float r = ProceduralEffectHelper.Lerp(sr, diffR, amount);
                float g = ProceduralEffectHelper.Lerp(sg, diffG, amount);
                float b = ProceduralEffectHelper.Lerp(sb, diffB, amount);

                r = AnalogEffectHelper.ApplyContrast(r, postContrast);
                g = AnalogEffectHelper.ApplyContrast(g, postContrast);
                b = AnalogEffectHelper.ApplyContrast(b, postContrast);

                if (warmth > 0.0001f)
                {
                    float warmMask = warmth * MathF.Pow(blurLum, 1.35f) * 0.22f;
                    r = AnalogEffectHelper.Screen(r, warmMask);
                    g = AnalogEffectHelper.Screen(g, warmMask * 0.55f);
                    b = ProceduralEffectHelper.Lerp(b, MathF.Max(0f, b - (warmMask * 0.12f)), warmMask);
                }

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r * 255f),
                    ProceduralEffectHelper.ClampToByte(g * 255f),
                    ProceduralEffectHelper.ClampToByte(b * 255f),
                    src.Alpha);
            }
        });

        return AnalogEffectHelper.CreateBitmap(source, dstPixels);
    }
}