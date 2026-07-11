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

public sealed class RisoPrintImageEffect : ImageEffectBase
{
    public override string Id => "riso_print";
    public override string Name => "Riso print";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.printer;
    public override string Description => "Simulates a risograph print with halftone dots and ink colors.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<RisoPrintImageEffect>("ink_strength", "Ink strength", 0f, 100f, 70f, (e, v) => e.InkStrength = v),
        EffectParameters.FloatSlider<RisoPrintImageEffect>("paper_fade", "Paper fade", 0f, 100f, 25f, (e, v) => e.PaperFade = v),
        EffectParameters.FloatSlider<RisoPrintImageEffect>("offset", "Offset", -200f, 200f, 3f, (e, v) => e.Offset = v),
        EffectParameters.FloatSlider<RisoPrintImageEffect>("dot_scale", "Dot scale", 0f, 100f, 18f, (e, v) => e.DotScale = v),
        EffectParameters.FloatSlider<RisoPrintImageEffect>("ink_noise", "Ink noise", 0f, 100f, 35f, (e, v) => e.InkNoise = v),
        EffectParameters.Color<RisoPrintImageEffect>("ink_color_a", "Ink color A", new SKColor(220, 70, 70, 255), (e, v) => e.InkColorA = v),
        EffectParameters.Color<RisoPrintImageEffect>("ink_color_b", "Ink color B", new SKColor(70, 200, 210, 255), (e, v) => e.InkColorB = v),
    ];

    public float InkStrength { get; set; } = 70f; // 0..100
    public float PaperFade { get; set; } = 25f; // 0..100
    public float Offset { get; set; } = 3f; // -200..200
    public float DotScale { get; set; } = 18f; // 0..100
    public float InkNoise { get; set; } = 35f; // 0..100
    public int Seed { get; set; } = 2026;

    public SKColor InkColorA { get; set; } = new SKColor(220, 70, 70, 255);
    public SKColor InkColorB { get; set; } = new SKColor(70, 200, 210, 255);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float inkStrength01 = Math.Clamp(InkStrength, 0f, 100f) / 100f;
        if (inkStrength01 <= 0f)
        {
            return source.Copy();
        }

        float paperFade01 = Math.Clamp(PaperFade, 0f, 100f) / 100f;
        float dotScale01 = Math.Clamp(DotScale, 0f, 100f) / 100f;
        float inkNoise01 = Math.Clamp(InkNoise, 0f, 100f) / 100f;
        float misregPx = Math.Clamp(Offset, -200f, 200f) * 0.30f; // keep effect subtle

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        // Dot cell size: smaller cells => finer halftone.
        int cell = Math.Clamp((int)MathF.Round(4f + (dotScale01 * 20f)), 3, 28);

        float aWeight = (InkColorA.Alpha / 255f) * inkStrength01;
        float bWeight = (InkColorB.Alpha / 255f) * inkStrength01;

        float paperKeep = 1f - (paperFade01 * 0.90f); // 0..1 (high fade => less ink)
        paperKeep = ProceduralEffectHelper.Clamp01(paperKeep);

        float inkAR = InkColorA.Red / 255f;
        float inkAG = InkColorA.Green / 255f;
        float inkAB = InkColorA.Blue / 255f;

        float inkBR = InkColorB.Red / 255f;
        float inkBG = InkColorB.Green / 255f;
        float inkBB = InkColorB.Blue / 255f;

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                int idx = row + x;
                SKColor src = srcPixels[idx];

                float lum01 = ((0.2126f * src.Red) + (0.7152f * src.Green) + (0.0722f * src.Blue)) / (255f * 1f);
                // Darker => more ink coverage.
                float coverage = ProceduralEffectHelper.Clamp01(1f - lum01);

                // Dot mask for ink A at original position.
                float dotA = ComputeDotMask(x, y, cell, coverage, inkNoise01, Seed, inkNoiseSeed: 0xA11);

                // Dot mask for ink B with slight misregistration.
                int xB = (int)MathF.Round(x + misregPx);
                float dotB = ComputeDotMask(xB, y, cell, coverage, inkNoise01, Seed, inkNoiseSeed: 0xB22);

                // Ink alpha (coverage) scaled by paper fade.
                float a = ProceduralEffectHelper.Clamp01(dotA * aWeight * paperKeep);
                float b = ProceduralEffectHelper.Clamp01(dotB * bWeight * paperKeep);

                // Start from paper (white).
                float outR = 1f;
                float outG = 1f;
                float outB = 1f;

                // Blend ink A then ink B (sequential overlay).
                outR = (outR * (1f - a)) + (inkAR * a);
                outG = (outG * (1f - a)) + (inkAG * a);
                outB = (outB * (1f - a)) + (inkAB * a);

                outR = (outR * (1f - b)) + (inkBR * b);
                outG = (outG * (1f - b)) + (inkBG * b);
                outB = (outB * (1f - b)) + (inkBB * b);

                // Grain/noise on ink.
                if (inkNoise01 > 0.001f)
                {
                    float n = (ProceduralEffectHelper.Hash01(x, y, Seed ^ 0x99) * 2f) - 1f;
                    float grain = n * inkNoise01 * 0.06f;
                    outR = MathF.Max(0f, outR + grain);
                    outG = MathF.Max(0f, outG + grain * 0.92f);
                    outB = MathF.Max(0f, outB + grain * 1.06f);
                }

                dstPixels[idx] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(outR * 255f),
                    ProceduralEffectHelper.ClampToByte(outG * 255f),
                    ProceduralEffectHelper.ClampToByte(outB * 255f),
                    src.Alpha);
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static float ComputeDotMask(
        int x,
        int y,
        int cell,
        float coverage,
        float inkNoise01,
        int seed,
        int inkNoiseSeed)
    {
        // Cell center.
        int cx = (x / cell) * cell + (cell / 2);
        int cy = (y / cell) * cell + (cell / 2);

        float dx = x - cx;
        float dy = y - cy;
        float dist = MathF.Sqrt((dx * dx) + (dy * dy));

        // Dot radius: more coverage => bigger dots.
        float maxR = cell * 0.45f;
        float r = maxR * MathF.Sqrt(MathF.Max(0f, coverage));

        // Seeded randomness adds ink irregularity.
        float h = ProceduralEffectHelper.Hash01(x, y, seed ^ inkNoiseSeed);
        float jitter = (h * 2f - 1f) * inkNoise01;
        r *= 1f + (0.25f * jitter);

        // Smooth edge for better visuals.
        float edge = 1.0f + (0.8f * inkNoise01);
        float t = ProceduralEffectHelper.SmoothStep(r, r + edge, dist);
        return 1f - t;
    }
}