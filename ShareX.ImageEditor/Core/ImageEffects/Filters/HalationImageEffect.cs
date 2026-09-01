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

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class HalationImageEffect : ImageEffectBase
{
    public override string Id => "halation";
    public override string Name => "Halation";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.sunrise;
    public override string Description => "Simulates film halation where bright areas bleed warm light into surrounding regions.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<HalationImageEffect>("threshold", "Threshold", 0, 100, 72, (e, v) => e.Threshold = v),
        EffectParameters.FloatSlider<HalationImageEffect>("radius", "Radius", 1, 100, 18, (e, v) => e.Radius = v),
        EffectParameters.FloatSlider<HalationImageEffect>("strength", "Strength", 0, 150, 60, (e, v) => e.Strength = v),
        EffectParameters.FloatSlider<HalationImageEffect>("warmth", "Warmth", 0, 100, 70, (e, v) => e.Warmth = v)
    ];

    public float Threshold { get; set; } = 72f; // 0..100
    public float Radius { get; set; } = 18f; // 1..100
    public float Strength { get; set; } = 60f; // 0..150
    public float Warmth { get; set; } = 70f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float threshold = Math.Clamp(Threshold, 0f, 100f) / 100f;
        float radius = Math.Clamp(Radius, 1f, 100f);
        float strength = Math.Clamp(Strength, 0f, 150f) / 100f;
        float warmth = Math.Clamp(Warmth, 0f, 100f) / 100f;

        if (strength <= 0f)
        {
            return source.Copy();
        }

        using SKBitmap halationSource = ExtractHalationSource(source, threshold, warmth);
        using SKBitmap blurredHalation = ApplyBlurWithClamp(halationSource, radius);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] haloPixels = blurredHalation.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        for (int i = 0; i < srcPixels.Length; i++)
        {
            SKColor src = srcPixels[i];
            SKColor halo = haloPixels[i];

            float haloR = (halo.Red / 255f) * strength * (1f + (warmth * 0.5f));
            float haloG = (halo.Green / 255f) * strength * (0.85f + (warmth * 0.15f));
            float haloB = (halo.Blue / 255f) * strength * (0.65f - (warmth * 0.25f));

            byte r = BlendHalationChannel(src.Red, haloR);
            byte g = BlendHalationChannel(src.Green, haloG);
            byte b = BlendHalationChannel(src.Blue, haloB);

            dstPixels[i] = new SKColor(r, g, b, src.Alpha);
        }

        return new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static SKBitmap ExtractHalationSource(SKBitmap source, float threshold, float warmth)
    {
        SKColor[] srcPixels = source.Pixels;
        SKColor[] haloPixels = new SKColor[srcPixels.Length];

        float low = Math.Clamp(threshold - 0.18f, 0f, 1f);
        float high = Math.Clamp(threshold + 0.12f, 0f, 1f);

        float redBoost = ProceduralEffectHelper.Lerp(1.20f, 1.85f, warmth);
        float greenBoost = ProceduralEffectHelper.Lerp(0.55f, 0.85f, warmth);
        float blueBoost = ProceduralEffectHelper.Lerp(0.35f, 0.15f, warmth);

        for (int i = 0; i < srcPixels.Length; i++)
        {
            SKColor src = srcPixels[i];
            float luminance = Luminance(src);
            float mask = ProceduralEffectHelper.SmoothStep(low, high, luminance);

            if (mask <= 0f)
            {
                haloPixels[i] = new SKColor(0, 0, 0, 0);
                continue;
            }

            float gain = mask * (0.40f + (0.60f * MathF.Pow(luminance, 1.4f)));

            haloPixels[i] = new SKColor(
                ProceduralEffectHelper.ClampToByte(src.Red * gain * redBoost),
                ProceduralEffectHelper.ClampToByte(src.Green * gain * greenBoost),
                ProceduralEffectHelper.ClampToByte(src.Blue * gain * blueBoost),
                ProceduralEffectHelper.ClampToByte(src.Alpha * mask));
        }

        return new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType)
        {
            Pixels = haloPixels
        };
    }

    private static SKBitmap ApplyBlurWithClamp(SKBitmap source, float radius)
    {
        if (radius <= 0.01f)
        {
            return source.Copy();
        }

        int padding = Math.Max(2, (int)MathF.Ceiling(radius * 2f));
        int expandedWidth = source.Width + (padding * 2);
        int expandedHeight = source.Height + (padding * 2);

        using SKBitmap expanded = new SKBitmap(expandedWidth, expandedHeight, source.ColorType, source.AlphaType);
        using (SKCanvas expandCanvas = new SKCanvas(expanded))
        {
            using SKShader shader = SKShader.CreateBitmap(
                source,
                SKShaderTileMode.Clamp,
                SKShaderTileMode.Clamp,
                SKMatrix.CreateTranslation(padding, padding));
            using SKPaint paint = new SKPaint { Shader = shader };
            expandCanvas.DrawRect(new SKRect(0, 0, expandedWidth, expandedHeight), paint);
        }

        float sigma = Math.Max(0.001f, radius / 3f);

        using SKBitmap blurredExpanded = new SKBitmap(expandedWidth, expandedHeight, source.ColorType, source.AlphaType);
        using (SKCanvas blurCanvas = new SKCanvas(blurredExpanded))
        {
            using SKPaint blurPaint = new SKPaint
            {
                ImageFilter = SKImageFilter.CreateBlur(sigma, sigma)
            };
            blurCanvas.DrawBitmap(expanded, 0, 0, blurPaint);
        }

        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using (SKCanvas resultCanvas = new SKCanvas(result))
        {
            resultCanvas.DrawBitmap(
                blurredExpanded,
                new SKRect(padding, padding, padding + source.Width, padding + source.Height),
                new SKRect(0, 0, source.Width, source.Height));
        }

        return result;
    }

    private static byte BlendHalationChannel(byte sourceChannel, float haloNormalized)
    {
        float src = sourceChannel / 255f;
        float screen = 1f - ((1f - src) * (1f - Math.Clamp(haloNormalized, 0f, 1f)));
        if (haloNormalized > 1f)
        {
            screen += (haloNormalized - 1f) * 0.35f;
        }

        return ProceduralEffectHelper.ClampToByte(screen * 255f);
    }

    private static float Luminance(SKColor c)
    {
        return ((0.2126f * c.Red) + (0.7152f * c.Green) + (0.0722f * c.Blue)) / 255f;
    }
}