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

using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class ChalkDrawingImageEffect : ImageEffectBase
{
    public override string Id => "chalk_drawing";
    public override string Name => "Chalk Drawing";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.brush;
    public override string Description => "Simulates a chalk drawing on a dark surface.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<ChalkDrawingImageEffect>("detail", "Detail", 0, 100, 50, (e, v) => e.Detail = v),
        EffectParameters.FloatSlider<ChalkDrawingImageEffect>("roughness", "Roughness", 0, 100, 40, (e, v) => e.Roughness = v),
        EffectParameters.Color<ChalkDrawingImageEffect>("chalk_color", "Chalk Color", new SKColor(255, 255, 255, 255), (e, v) => e.ChalkColor = v),
        EffectParameters.Color<ChalkDrawingImageEffect>("board_color", "Board Color", new SKColor(30, 50, 40, 255), (e, v) => e.BoardColor = v)
    ];

    public float Detail { get; set; } = 50f;
    public float Roughness { get; set; } = 40f;
    public SKColor ChalkColor { get; set; } = new SKColor(255, 255, 255, 255);
    public SKColor BoardColor { get; set; } = new SKColor(30, 50, 40, 255);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        float detail = Math.Clamp(Detail, 0f, 100f) / 100f;
        float roughness = Math.Clamp(Roughness, 0f, 100f) / 100f;

        SKColor[] srcPixels = source.Pixels;

        // Step 1: Compute edge intensity using Sobel-like operator on luminance.
        float[] luminance = new float[width * height];
        for (int i = 0; i < srcPixels.Length; i++)
        {
            SKColor c = srcPixels[i];
            luminance[i] = (0.299f * c.Red + 0.587f * c.Green + 0.114f * c.Blue) / 255f;
        }

        float[] edgeStrength = new float[width * height];
        float maxEdge = 0f;

        for (int y = 1; y < height - 1; y++)
        {
            int row = y * width;
            for (int x = 1; x < width - 1; x++)
            {
                // Sobel X
                float gx =
                    -luminance[(y - 1) * width + (x - 1)] + luminance[(y - 1) * width + (x + 1)]
                    - 2f * luminance[row + (x - 1)] + 2f * luminance[row + (x + 1)]
                    - luminance[(y + 1) * width + (x - 1)] + luminance[(y + 1) * width + (x + 1)];
                // Sobel Y
                float gy =
                    -luminance[(y - 1) * width + (x - 1)] - 2f * luminance[(y - 1) * width + x] - luminance[(y - 1) * width + (x + 1)]
                    + luminance[(y + 1) * width + (x - 1)] + 2f * luminance[(y + 1) * width + x] + luminance[(y + 1) * width + (x + 1)];

                float magnitude = MathF.Sqrt(gx * gx + gy * gy);
                edgeStrength[row + x] = magnitude;
                if (magnitude > maxEdge) maxEdge = magnitude;
            }
        }

        // Normalize edge strength.
        if (maxEdge > 0f)
        {
            float invMax = 1f / maxEdge;
            for (int i = 0; i < edgeStrength.Length; i++)
            {
                edgeStrength[i] *= invMax;
            }
        }

        // Step 2: Generate chalk texture noise (deterministic).
        float[] noise = new float[width * height];
        int rng = 12345;
        for (int i = 0; i < noise.Length; i++)
        {
            rng ^= rng << 13;
            rng ^= rng >> 17;
            rng ^= rng << 5;
            noise[i] = (rng & 0x7FFFFFFF) / (float)int.MaxValue;
        }

        // Step 3: Combine edges, luminance, and noise into chalk drawing.
        float boardR = BoardColor.Red / 255f;
        float boardG = BoardColor.Green / 255f;
        float boardB = BoardColor.Blue / 255f;
        float chalkR = ChalkColor.Red / 255f;
        float chalkG = ChalkColor.Green / 255f;
        float chalkB = ChalkColor.Blue / 255f;

        // Detail controls how much edge detection contributes.
        // Low detail = more luminance-based, high detail = more edge-based.
        float edgeWeight = 0.3f + detail * 0.7f;
        float lumWeight = 1f - edgeWeight * 0.5f;

        SKColor[] result = new SKColor[width * height];

        for (int i = 0; i < result.Length; i++)
        {
            float edge = edgeStrength[i];
            float lum = luminance[i];

            // Chalk intensity: blend of edge and luminance
            float chalkIntensity = edge * edgeWeight + lum * lumWeight;
            chalkIntensity = Math.Clamp(chalkIntensity, 0f, 1f);

            // Apply chalk texture: roughness modulates noise impact.
            float textureFactor = 1f - roughness * (1f - noise[i]) * 0.6f;
            chalkIntensity *= textureFactor;

            // Reduce chalk in very dark areas to keep the board visible.
            float darkMask = MathF.Pow(lum, 0.5f);
            chalkIntensity *= darkMask;

            chalkIntensity = Math.Clamp(chalkIntensity, 0f, 1f);

            float outR = boardR + (chalkR - boardR) * chalkIntensity;
            float outG = boardG + (chalkG - boardG) * chalkIntensity;
            float outB = boardB + (chalkB - boardB) * chalkIntensity;

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