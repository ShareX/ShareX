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

public sealed class LiquidGlassImageEffect : ImageEffectBase
{
    public override string Id => "liquid_glass";
    public override string Name => "Liquid glass";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.glass_water;
    public override string Description => "Distorts the image as if viewed through liquid glass.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<LiquidGlassImageEffect>("distortion", "Distortion", 0, 35, 9, (e, v) => e.Distortion = v),
        EffectParameters.FloatSlider<LiquidGlassImageEffect>("refraction", "Refraction", 0, 100, 45, (e, v) => e.Refraction = v),
        EffectParameters.IntSlider<LiquidGlassImageEffect>("chroma_shift", "Chroma shift", 0, 8, 1, (e, v) => e.ChromaShift = v),
        EffectParameters.FloatSlider<LiquidGlassImageEffect>("gloss", "Gloss", 0, 100, 40, (e, v) => e.Gloss = v),
        EffectParameters.FloatSlider<LiquidGlassImageEffect>("flow_scale", "Flow scale", 40, 220, 100, (e, v) => e.FlowScale = v)
    ];

    public float Distortion { get; set; } = 9f; // 0..35
    public float Refraction { get; set; } = 45f; // 0..100
    public int ChromaShift { get; set; } = 1; // 0..8
    public float Gloss { get; set; } = 40f; // 0..100
    public float FlowScale { get; set; } = 100f; // 40..220
    public int Seed { get; set; } = 9421;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float distortionPx = Math.Clamp(Distortion, 0f, 35f);
        float refraction = Math.Clamp(Refraction, 0f, 100f) / 100f;
        int chroma = Math.Clamp(ChromaShift, 0, 8);
        float gloss = Math.Clamp(Gloss, 0f, 100f) / 100f;
        float flowScale = Math.Clamp(FlowScale, 40f, 220f) / 100f;

        if (distortionPx <= 0f && refraction <= 0f && chroma <= 0 && gloss <= 0f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float freqX = 0.072f / flowScale;
        float freqY = 0.061f / flowScale;

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            int yTop = y > 0 ? y - 1 : 0;
            int yBottom = y < (height - 1) ? y + 1 : height - 1;

            for (int x = 0; x < width; x++)
            {
                int xLeft = x > 0 ? x - 1 : 0;
                int xRight = x < (width - 1) ? x + 1 : width - 1;

                float cellNoise = (ProceduralEffectHelper.Hash01(x / 18, y / 18, Seed) * 2f) - 1f;
                float wave1 = MathF.Sin((x * freqX) + (y * (freqY * 0.45f)) + (cellNoise * 2.4f) + (Seed * 0.001f));
                float wave2 = MathF.Cos((x * (freqX * 0.58f)) - (y * (freqY * 1.12f)) + (cellNoise * 1.7f));
                float wave3 = MathF.Sin(((x + y) * (freqX * 0.62f)) + (cellNoise * 3.1f));

                float offsetX = distortionPx * ((0.52f * wave1) + (0.26f * wave3));
                float offsetY = distortionPx * ((0.46f * wave2) - (0.22f * wave3));

                if (refraction > 0f)
                {
                    SKColor left = srcPixels[row + xLeft];
                    SKColor right = srcPixels[row + xRight];
                    SKColor top = srcPixels[(yTop * width) + x];
                    SKColor bottom = srcPixels[(yBottom * width) + x];

                    float gradX = Luminance(right) - Luminance(left);
                    float gradY = Luminance(bottom) - Luminance(top);
                    float edge = MathF.Sqrt((gradX * gradX) + (gradY * gradY));

                    float edgeBoost = 1f + (edge * 3.2f * refraction);
                    offsetX = (offsetX * edgeBoost) + (gradX * refraction * distortionPx * 3f);
                    offsetY = (offsetY * edgeBoost) + (gradY * refraction * distortionPx * 3f);
                }

                float sampleX = x + offsetX;
                float sampleY = y + offsetY;

                float r;
                float g;
                float b;
                byte a;

                if (chroma > 0)
                {
                    float shift = chroma * 0.75f;
                    SKColor sampleR = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX + shift, sampleY - (shift * 0.15f));
                    SKColor sampleG = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX, sampleY);
                    SKColor sampleB = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX - shift, sampleY + (shift * 0.15f));

                    r = sampleR.Red;
                    g = sampleG.Green;
                    b = sampleB.Blue;
                    a = sampleG.Alpha;
                }
                else
                {
                    SKColor sample = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX, sampleY);
                    r = sample.Red;
                    g = sample.Green;
                    b = sample.Blue;
                    a = sample.Alpha;
                }

                if (refraction > 0f)
                {
                    SKColor n1 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX + 1f, sampleY);
                    SKColor n2 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX - 1f, sampleY);
                    SKColor n3 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX, sampleY + 1f);
                    SKColor n4 = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX, sampleY - 1f);

                    float blurR = (n1.Red + n2.Red + n3.Red + n4.Red) * 0.25f;
                    float blurG = (n1.Green + n2.Green + n3.Green + n4.Green) * 0.25f;
                    float blurB = (n1.Blue + n2.Blue + n3.Blue + n4.Blue) * 0.25f;

                    float blurMix = 0.06f + (0.18f * refraction);
                    r = ProceduralEffectHelper.Lerp(r, blurR, blurMix);
                    g = ProceduralEffectHelper.Lerp(g, blurG, blurMix);
                    b = ProceduralEffectHelper.Lerp(b, blurB, blurMix);
                }

                float ridge = MathF.Abs(wave1 - wave2);
                float micro = MathF.Abs(wave3);
                float spec = MathF.Pow(MathF.Max(0f, 1f - (ridge * 1.35f)), 6f);
                float streak = MathF.Pow(MathF.Max(0f, 1f - (micro * 1.55f)), 4f);
                float luminance = ((0.2126f * r) + (0.7152f * g) + (0.0722f * b)) / 255f;

                float highlight = gloss * ((spec * 0.62f) + (streak * 0.24f) + (MathF.Pow(luminance, 2.8f) * 0.14f));
                r += highlight * 88f;
                g += highlight * 102f;
                b += highlight * 126f;

                float tint = (0.04f + (0.10f * refraction)) * (0.4f + (0.6f * gloss));
                r = ProceduralEffectHelper.Lerp(r, r + 6f, tint);
                g = ProceduralEffectHelper.Lerp(g, g + 12f, tint);
                b = ProceduralEffectHelper.Lerp(b, b + 22f, tint);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r),
                    ProceduralEffectHelper.ClampToByte(g),
                    ProceduralEffectHelper.ClampToByte(b),
                    a);
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static float Luminance(SKColor c)
    {
        return ((0.2126f * c.Red) + (0.7152f * c.Green) + (0.0722f * c.Blue)) / 255f;
    }
}