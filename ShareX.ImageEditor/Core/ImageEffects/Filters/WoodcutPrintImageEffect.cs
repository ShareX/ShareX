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

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class WoodcutPrintImageEffect : ImageEffectBase
{
    public override string Id => "woodcut_print";
    public override string Name => "Woodcut Print";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.axe;
    public override string Description => "Simulates a woodcut or linocut print style.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<WoodcutPrintImageEffect>("threshold", "Threshold", 0, 100, 50, (e, v) => e.Threshold = v),
        EffectParameters.FloatSlider<WoodcutPrintImageEffect>("edge_strength", "Edge Strength", 0, 100, 60, (e, v) => e.EdgeStrength = v),
        EffectParameters.FloatSlider<WoodcutPrintImageEffect>("grain_amount", "Wood Grain", 0, 100, 30, (e, v) => e.GrainAmount = v),
        EffectParameters.Color<WoodcutPrintImageEffect>("ink_color", "Ink Color", new SKColor(15, 10, 5, 255), (e, v) => e.InkColor = v),
        EffectParameters.Color<WoodcutPrintImageEffect>("paper_color", "Paper Color", new SKColor(235, 225, 200, 255), (e, v) => e.PaperColor = v)
    ];

    public float Threshold { get; set; } = 50f;
    public float EdgeStrength { get; set; } = 60f;
    public float GrainAmount { get; set; } = 30f;
    public SKColor InkColor { get; set; } = new SKColor(15, 10, 5, 255);
    public SKColor PaperColor { get; set; } = new SKColor(235, 225, 200, 255);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        float threshold = Math.Clamp(Threshold, 0f, 100f) / 100f;
        float edgeWeight = Math.Clamp(EdgeStrength, 0f, 100f) / 100f;
        float grainWeight = Math.Clamp(GrainAmount, 0f, 100f) / 100f;

        SKColor[] srcPixels = source.Pixels;
        int count = srcPixels.Length;

        // Step 1: Compute luminance.
        float[] lum = new float[count];
        for (int i = 0; i < count; i++)
        {
            SKColor c = srcPixels[i];
            lum[i] = (0.299f * c.Red + 0.587f * c.Green + 0.114f * c.Blue) / 255f;
        }

        // Step 2: Edge detection (Sobel magnitude).
        float[] edges = new float[count];
        for (int y = 1; y < height - 1; y++)
        {
            int row = y * width;
            for (int x = 1; x < width - 1; x++)
            {
                float gx =
                    -lum[(y - 1) * width + (x - 1)] + lum[(y - 1) * width + (x + 1)]
                    - 2f * lum[row + (x - 1)] + 2f * lum[row + (x + 1)]
                    - lum[(y + 1) * width + (x - 1)] + lum[(y + 1) * width + (x + 1)];
                float gy =
                    -lum[(y - 1) * width + (x - 1)] - 2f * lum[(y - 1) * width + x] - lum[(y - 1) * width + (x + 1)]
                    + lum[(y + 1) * width + (x - 1)] + 2f * lum[(y + 1) * width + x] + lum[(y + 1) * width + (x + 1)];

                edges[row + x] = MathF.Sqrt(gx * gx + gy * gy);
            }
        }

        // Step 3: Generate wood grain pattern (horizontal wave noise).
        float[] grain = new float[count];
        if (grainWeight > 0.001f)
        {
            int rng = 7919;
            // Pre-compute per-row phase offsets for a natural grain look.
            float[] rowPhase = new float[height];
            for (int y = 0; y < height; y++)
            {
                rng ^= rng << 13;
                rng ^= rng >> 17;
                rng ^= rng << 5;
                rowPhase[y] = (rng & 0x7FFFFFFF) / (float)int.MaxValue * MathF.PI * 2f;
            }

            for (int y = 0; y < height; y++)
            {
                int row = y * width;
                float phase = rowPhase[y];
                // Slowly varying wave along the row.
                for (int x = 0; x < width; x++)
                {
                    float wave = MathF.Sin(x * 0.08f + phase) * 0.5f + 0.5f;
                    float fineWave = MathF.Sin(x * 0.3f + y * 0.15f + phase * 2f) * 0.5f + 0.5f;
                    grain[row + x] = wave * 0.6f + fineWave * 0.4f;
                }
            }
        }

        // Step 4: Compose. Dark areas become ink, light areas become paper.
        // Edges force ink. Grain modulates the threshold.
        float inkR = InkColor.Red / 255f;
        float inkG = InkColor.Green / 255f;
        float inkB = InkColor.Blue / 255f;
        float papR = PaperColor.Red / 255f;
        float papG = PaperColor.Green / 255f;
        float papB = PaperColor.Blue / 255f;

        SKColor[] result = new SKColor[count];

        for (int i = 0; i < count; i++)
        {
            float l = lum[i];
            float e = edges[i];
            float g = grain[i];

            // Modulate threshold with grain to create wood‐like texture variation.
            float localThreshold = threshold + (g - 0.5f) * grainWeight * 0.4f;

            // Score: lower = more likely to be ink.
            // Edges push toward ink.
            float score = l - e * edgeWeight;

            float isInk;
            if (score < localThreshold - 0.05f)
                isInk = 1f;
            else if (score > localThreshold + 0.05f)
                isInk = 0f;
            else
            {
                // Soft transition band.
                isInk = 1f - (score - (localThreshold - 0.05f)) / 0.1f;
            }

            float outR = inkR * isInk + papR * (1f - isInk);
            float outG = inkG * isInk + papG * (1f - isInk);
            float outB = inkB * isInk + papB * (1f - isInk);

            result[i] = new SKColor(
                (byte)MathF.Round(Math.Clamp(outR, 0f, 1f) * 255f),
                (byte)MathF.Round(Math.Clamp(outG, 0f, 1f) * 255f),
                (byte)MathF.Round(Math.Clamp(outB, 0f, 1f) * 255f),
                srcPixels[i].Alpha);
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = result
        };
    }
}