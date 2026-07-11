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

public sealed class LavaMoltenBorderImageEffect : ImageEffectBase
{
    public override string Id => "lava_molten_border";
    public override string Name => "Lava molten border";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.flame;
    public override string Description => "Adds a volcanic border with dark cracked rock and glowing molten lava seams.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntNumeric<LavaMoltenBorderImageEffect>("border_size", "Border size", 12, 300, 56, (e, v) => e.BorderSize = v),
        EffectParameters.Color<LavaMoltenBorderImageEffect>("rock_color", "Rock color", new SKColor(30, 25, 22), (e, v) => e.RockColor = v),
        EffectParameters.Color<LavaMoltenBorderImageEffect>("lava_color", "Lava color", new SKColor(255, 80, 10), (e, v) => e.LavaColor = v),
        EffectParameters.FloatSlider<LavaMoltenBorderImageEffect>("crack_density", "Crack density", 10, 100, 60, (e, v) => e.CrackDensity = v),
        EffectParameters.FloatSlider<LavaMoltenBorderImageEffect>("glow_intensity", "Glow intensity", 10, 100, 70, (e, v) => e.GlowIntensity = v),
        EffectParameters.Bool<LavaMoltenBorderImageEffect>("ember_sparks", "Ember sparks", true, (e, v) => e.EmberSparks = v)
    ];

    public int BorderSize { get; set; } = 56;
    public SKColor RockColor { get; set; } = new SKColor(30, 25, 22);
    public SKColor LavaColor { get; set; } = new SKColor(255, 80, 10);
    public float CrackDensity { get; set; } = 60f;
    public float GlowIntensity { get; set; } = 70f;
    public bool EmberSparks { get; set; } = true;

    private const int Seed = 8371;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int border = Math.Clamp(BorderSize, 12, 300);
        float density = Math.Clamp(CrackDensity, 10f, 100f) / 100f;
        float glowStr = Math.Clamp(GlowIntensity, 10f, 100f) / 100f;

        int newWidth = source.Width + border * 2;
        int newHeight = source.Height + border * 2;

        SKColor[] dstPixels = new SKColor[newWidth * newHeight];

        float noiseScale = 0.04f + density * 0.06f;

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

                // Fractal noise for crack pattern
                float n1 = ProceduralEffectHelper.FractalNoise(x * noiseScale, y * noiseScale, 4, 2f, 0.5f, Seed);
                float n2 = ProceduralEffectHelper.FractalNoise(x * noiseScale * 1.7f + 100f, y * noiseScale * 1.7f + 200f, 3, 2f, 0.5f, Seed ^ 0x55);

                // Cracks form where noise crosses zero - narrow band around 0
                float crackWidth = 0.08f + density * 0.12f;
                float crack1 = 1f - ProceduralEffectHelper.SmoothStep(0f, crackWidth, MathF.Abs(n1));
                float crack2 = 1f - ProceduralEffectHelper.SmoothStep(0f, crackWidth, MathF.Abs(n2));
                float crackMask = Math.Max(crack1, crack2);

                // Lava glow: blend between rock and lava based on crack mask
                float lavaBlend = crackMask * glowStr;

                // Rock texture: add slight noise variation to rock color
                float rockNoise = ProceduralEffectHelper.Hash01(x, y, Seed ^ 0x77) * 0.15f - 0.075f;

                float rr = (RockColor.Red / 255f + rockNoise) * (1f - lavaBlend) + (LavaColor.Red / 255f) * lavaBlend;
                float rg = (RockColor.Green / 255f + rockNoise) * (1f - lavaBlend) + (LavaColor.Green / 255f) * lavaBlend;
                float rb = (RockColor.Blue / 255f + rockNoise) * (1f - lavaBlend) + (LavaColor.Blue / 255f) * lavaBlend;

                // Bright center of cracks
                if (crackMask > 0.7f)
                {
                    float brightBoost = (crackMask - 0.7f) / 0.3f * glowStr;
                    rr += brightBoost * 0.6f;
                    rg += brightBoost * 0.3f;
                    rb += brightBoost * 0.05f;
                }

                dstPixels[(y * newWidth) + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(rr * 255f),
                    ProceduralEffectHelper.ClampToByte(rg * 255f),
                    ProceduralEffectHelper.ClampToByte(rb * 255f),
                    255);
            }
        }

        SKBitmap result = new SKBitmap(newWidth, newHeight, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };

        using SKCanvas canvas = new SKCanvas(result);

        if (EmberSparks)
        {
            // Scatter bright ember sparks
            using SKPaint sparkPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 2f)
            };

            int sparkCount = (newWidth + newHeight) / 8;
            for (int i = 0; i < sparkCount; i++)
            {
                float sx = ProceduralEffectHelper.Hash01(i, 0, Seed ^ 0xAB) * newWidth;
                float sy = ProceduralEffectHelper.Hash01(0, i, Seed ^ 0xCD) * newHeight;

                // Only draw in border area
                bool inBorder = sx < border || sx >= border + source.Width
                             || sy < border || sy >= border + source.Height;
                if (!inBorder) continue;

                // Only place near cracks
                float localNoise = MathF.Abs(ProceduralEffectHelper.FractalNoise(sx * noiseScale, sy * noiseScale, 4, 2f, 0.5f, Seed));
                if (localNoise > 0.15f) continue;

                float brightness = ProceduralEffectHelper.Hash01(i, i, Seed ^ 0xEF);
                byte r = ProceduralEffectHelper.ClampToByte(255 * (0.8f + brightness * 0.2f));
                byte g = ProceduralEffectHelper.ClampToByte(180 * brightness);
                byte b = ProceduralEffectHelper.ClampToByte(40 * brightness);
                sparkPaint.Color = new SKColor(r, g, b, ProceduralEffectHelper.ClampToByte(150 + brightness * 100));

                float radius = 1f + brightness * 2f;
                canvas.DrawCircle(sx, sy, radius, sparkPaint);
            }
        }

        // Inner edge glow
        using SKPaint glowPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 4f,
            Color = new SKColor(LavaColor.Red, LavaColor.Green, LavaColor.Blue, (byte)(60 * glowStr)),
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 6f)
        };
        canvas.DrawRect(border, border, source.Width, source.Height, glowPaint);

        canvas.DrawBitmap(source, border, border);

        return result;
    }
}