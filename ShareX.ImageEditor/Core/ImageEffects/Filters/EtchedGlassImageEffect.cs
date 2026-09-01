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

public sealed class EtchedGlassImageEffect : ImageEffectBase
{
    public override string Id => "etched_glass";
    public override string Name => "Etched glass";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.glass_water;
    public override string Description => "Simulates an etched glass surface with frost, engraving, and refraction effects.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<EtchedGlassImageEffect>("frost", "Frost", 0, 100, 48, (e, v) => e.Frost = v),
        EffectParameters.FloatSlider<EtchedGlassImageEffect>("engrave", "Engrave", 0, 100, 68, (e, v) => e.Engrave = v),
        EffectParameters.FloatSlider<EtchedGlassImageEffect>("refraction", "Refraction", 0, 100, 18, (e, v) => e.Refraction = v),
        EffectParameters.FloatSlider<EtchedGlassImageEffect>("highlight", "Highlight", 0, 100, 42, (e, v) => e.Highlight = v),
        EffectParameters.FloatSlider<EtchedGlassImageEffect>("background_fade", "Background fade", 0, 100, 38, (e, v) => e.BackgroundFade = v)
    ];

    public float Frost { get; set; } = 48f; // 0..100
    public float Engrave { get; set; } = 68f; // 0..100
    public float Refraction { get; set; } = 18f; // 0..100
    public float Highlight { get; set; } = 42f; // 0..100
    public float BackgroundFade { get; set; } = 38f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float frost = Math.Clamp(Frost, 0f, 100f) / 100f;
        float engrave = Math.Clamp(Engrave, 0f, 100f) / 100f;
        float refraction = Math.Clamp(Refraction, 0f, 100f) / 100f;
        float highlight = Math.Clamp(Highlight, 0f, 100f) / 100f;
        float backgroundFade = Math.Clamp(BackgroundFade, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float refractionPx = 0.6f + (refraction * 5.4f);

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float noiseX = ((ProceduralEffectHelper.Hash01(x / 3, y / 3, 2011) * 2f) - 1f) * refractionPx;
                float noiseY = ((ProceduralEffectHelper.Hash01(x / 3, y / 3, 3001) * 2f) - 1f) * refractionPx;

                SKColor refracted = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, x + noiseX, y + noiseY);
                SKColor blurred = SampleBlurred(srcPixels, width, height, x, y);

                float lum = GetLuminance(refracted) / 255f;
                float edge = SampleEdge(srcPixels, width, height, x, y);

                float baseR = ProceduralEffectHelper.Lerp(src.Red / 255f, blurred.Red / 255f, backgroundFade * 0.72f);
                float baseG = ProceduralEffectHelper.Lerp(src.Green / 255f, blurred.Green / 255f, backgroundFade * 0.72f);
                float baseB = ProceduralEffectHelper.Lerp(src.Blue / 255f, blurred.Blue / 255f, backgroundFade * 0.72f);

                float gray = lum * 0.88f + 0.07f;
                float glassR = ProceduralEffectHelper.Lerp(baseR, gray + 0.07f, frost * 0.58f);
                float glassG = ProceduralEffectHelper.Lerp(baseG, gray + 0.08f, frost * 0.60f);
                float glassB = ProceduralEffectHelper.Lerp(baseB, gray + 0.10f, frost * 0.64f);

                float engraving = edge * engrave;
                glassR *= 1f - (engraving * 0.36f);
                glassG *= 1f - (engraving * 0.38f);
                glassB *= 1f - (engraving * 0.40f);

                float spec = edge * highlight;
                glassR = ProceduralEffectHelper.Lerp(glassR, 1f, spec * 0.16f);
                glassG = ProceduralEffectHelper.Lerp(glassG, 1f, spec * 0.18f);
                glassB = ProceduralEffectHelper.Lerp(glassB, 1f, spec * 0.22f);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(glassR * 255f),
                    ProceduralEffectHelper.ClampToByte(glassG * 255f),
                    ProceduralEffectHelper.ClampToByte(glassB * 255f),
                    src.Alpha);
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static SKColor SampleBlurred(SKColor[] pixels, int width, int height, int x, int y)
    {
        int left = Math.Max(0, x - 2);
        int right = Math.Min(width - 1, x + 2);
        int top = Math.Max(0, y - 2);
        int bottom = Math.Min(height - 1, y + 2);

        SKColor c0 = pixels[(y * width) + x];
        SKColor c1 = pixels[(y * width) + left];
        SKColor c2 = pixels[(y * width) + right];
        SKColor c3 = pixels[(top * width) + x];
        SKColor c4 = pixels[(bottom * width) + x];

        float r = (c0.Red + c1.Red + c2.Red + c3.Red + c4.Red) / 5f;
        float g = (c0.Green + c1.Green + c2.Green + c3.Green + c4.Green) / 5f;
        float b = (c0.Blue + c1.Blue + c2.Blue + c3.Blue + c4.Blue) / 5f;
        float a = (c0.Alpha + c1.Alpha + c2.Alpha + c3.Alpha + c4.Alpha) / 5f;

        return new SKColor(
            ProceduralEffectHelper.ClampToByte(r),
            ProceduralEffectHelper.ClampToByte(g),
            ProceduralEffectHelper.ClampToByte(b),
            ProceduralEffectHelper.ClampToByte(a));
    }

    private static float SampleEdge(SKColor[] pixels, int width, int height, int x, int y)
    {
        int left = Math.Max(0, x - 1);
        int right = Math.Min(width - 1, x + 1);
        int top = Math.Max(0, y - 1);
        int bottom = Math.Min(height - 1, y + 1);

        float lumLeft = GetLuminance(pixels[(y * width) + left]) / 255f;
        float lumRight = GetLuminance(pixels[(y * width) + right]) / 255f;
        float lumTop = GetLuminance(pixels[(top * width) + x]) / 255f;
        float lumBottom = GetLuminance(pixels[(bottom * width) + x]) / 255f;

        return ProceduralEffectHelper.Clamp01((MathF.Abs(lumRight - lumLeft) + MathF.Abs(lumBottom - lumTop)) * 1.25f);
    }

    private static float GetLuminance(SKColor color)
    {
        return (0.2126f * color.Red) + (0.7152f * color.Green) + (0.0722f * color.Blue);
    }
}