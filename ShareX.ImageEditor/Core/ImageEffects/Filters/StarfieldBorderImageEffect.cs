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

public sealed class StarfieldBorderImageEffect : ImageEffectBase
{
    public override string Id => "starfield_border";
    public override string Name => "Starfield border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.orbit;
    public override string Description => "Adds a cosmic starfield border with scattered stars, nebula wisps, and subtle color tints.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<StarfieldBorderImageEffect>("border_size", "Border size", 12, 300, 60, (e, v) => e.BorderSize = v),
        EffectParameters.Color<StarfieldBorderImageEffect>("bg_color", "Background color", new SKColor(5, 5, 18), (e, v) => e.BgColor = v),
        EffectParameters.Color<StarfieldBorderImageEffect>("nebula_color", "Nebula tint", new SKColor(40, 10, 80), (e, v) => e.NebulaColor = v),
        EffectParameters.FloatSlider<StarfieldBorderImageEffect>("star_density", "Star density", 10, 100, 65, (e, v) => e.StarDensity = v),
        EffectParameters.FloatSlider<StarfieldBorderImageEffect>("nebula_intensity", "Nebula intensity", 0, 100, 45, (e, v) => e.NebulaIntensity = v),
        EffectParameters.Bool<StarfieldBorderImageEffect>("bright_stars", "Bright star glow", true, (e, v) => e.BrightStars = v)
    ];

    public int BorderSize { get; set; } = 60;
    public SKColor BgColor { get; set; } = new SKColor(5, 5, 18);
    public SKColor NebulaColor { get; set; } = new SKColor(40, 10, 80);
    public float StarDensity { get; set; } = 65f;
    public float NebulaIntensity { get; set; } = 45f;
    public bool BrightStars { get; set; } = true;

    private const int Seed = 4207;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 12, 300);
        float starDens = Math.Clamp(StarDensity, 10f, 100f) / 100f;
        float nebulaStr = Math.Clamp(NebulaIntensity, 0f, 100f) / 100f;

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKColor[] dstPixels = new SKColor[newWidth * newHeight];

        float nebulaScale = 0.012f;

        for (int y = 0; y < newHeight; y++)
        {
            bool topBand = y < border;
            bool bottomBand = y >= border + source.Height;

            for (int x = 0; x < newWidth; x++)
            {
                bool leftBand = x < border;
                bool rightBand = x >= border + source.Width;

                if (!topBand && !bottomBand && !leftBand && !rightBand)
                    continue;

                // Base background
                float r = BgColor.Red / 255f;
                float g = BgColor.Green / 255f;
                float b = BgColor.Blue / 255f;

                // Nebula: slow fractal noise tinted with nebula color
                float neb1 = ProceduralEffectHelper.FractalNoise(x * nebulaScale, y * nebulaScale, 4, 2f, 0.5f, Seed);
                float neb2 = ProceduralEffectHelper.FractalNoise(x * nebulaScale * 0.7f + 300f, y * nebulaScale * 0.7f + 500f, 3, 2f, 0.5f, Seed ^ 0x33);

                float nebMask = ProceduralEffectHelper.Clamp01((neb1 + 0.3f) * 0.7f) * nebulaStr;
                float nebMask2 = ProceduralEffectHelper.Clamp01((neb2 + 0.2f) * 0.5f) * nebulaStr * 0.5f;

                r += (NebulaColor.Red / 255f) * nebMask + (0.1f * nebMask2);
                g += (NebulaColor.Green / 255f) * nebMask + (0.15f * nebMask2);
                b += (NebulaColor.Blue / 255f) * nebMask + (0.25f * nebMask2);

                // Tiny faint stars via hash threshold
                float starHash = ProceduralEffectHelper.Hash01(x, y, Seed ^ 0xAA);
                float starThreshold = 1f - starDens * 0.015f;
                if (starHash > starThreshold)
                {
                    float starBright = (starHash - starThreshold) / (1f - starThreshold);
                    starBright *= starBright; // sharpen
                    // Slight color variation
                    float hueShift = ProceduralEffectHelper.Hash01(x ^ 0x55, y ^ 0x33, Seed ^ 0xBB);
                    r += starBright * (0.7f + hueShift * 0.3f);
                    g += starBright * (0.7f + (1f - hueShift) * 0.3f);
                    b += starBright * 0.9f;
                }

                dstPixels[(y * newWidth) + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r * 255f),
                    ProceduralEffectHelper.ClampToByte(g * 255f),
                    ProceduralEffectHelper.ClampToByte(b * 255f),
                    255);
            }
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        using SKCanvas canvas = new SKCanvas(result);

        if (BrightStars)
        {
            // Larger bright stars with glow
            using SKPaint starPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            using SKPaint glowPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 3f)
            };

            int bigStarCount = (int)((newWidth + newHeight) * 0.15f * starDens);
            for (int i = 0; i < bigStarCount; i++)
            {
                float sx = ProceduralEffectHelper.Hash01(i, 0, Seed ^ 0xDE) * newWidth;
                float sy = ProceduralEffectHelper.Hash01(0, i, Seed ^ 0xEF) * newHeight;

                bool inBorder = sx < border || sx >= border + source.Width
                             || sy < border || sy >= border + source.Height;
                if (!inBorder) continue;

                float brightness = ProceduralEffectHelper.Hash01(i, i, Seed ^ 0xF1);
                float size = 0.8f + brightness * 2.5f;

                // Star color: mostly white with slight warmth or coolness
                float temp = ProceduralEffectHelper.Hash01(i ^ 0x77, i ^ 0x99, Seed ^ 0xF2);
                byte sr = ProceduralEffectHelper.ClampToByte(200 + 55 * brightness);
                byte sg = ProceduralEffectHelper.ClampToByte(190 + 55 * brightness * (0.9f + temp * 0.1f));
                byte sb = ProceduralEffectHelper.ClampToByte(180 + 75 * brightness * (0.5f + temp * 0.5f));
                byte alpha = ProceduralEffectHelper.ClampToByte(160 + 95 * brightness);

                glowPaint.Color = new SKColor(sr, sg, sb, (byte)(alpha / 2));
                canvas.DrawCircle(sx, sy, size * 2.5f, glowPaint);

                starPaint.Color = new SKColor(sr, sg, sb, alpha);
                canvas.DrawCircle(sx, sy, size, starPaint);
            }
        }

        canvas.DrawBitmap(source, border, border);

        return result;
    }
}