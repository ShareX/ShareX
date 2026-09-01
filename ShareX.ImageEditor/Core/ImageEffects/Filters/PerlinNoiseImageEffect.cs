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

public sealed class PerlinNoiseImageEffect : ImageEffectBase
{
    public override string Id => "perlin_noise";
    public override string Name => "Perlin noise";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.waves;
    public override string Description => "Generates a Perlin noise pattern and blends it with the image.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<PerlinNoiseImageEffect>("scale", "Scale", 1f, 500f, 64f, (e, v) => e.Scale = v,
            tickFrequency: 1, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0}"),
        EffectParameters.IntSlider<PerlinNoiseImageEffect>("octaves", "Octaves", 1, 8, 4, (e, v) => e.Octaves = v),
        EffectParameters.FloatSlider<PerlinNoiseImageEffect>("persistence", "Persistence", 0f, 1f, 0.5f, (e, v) => e.Persistence = v,
            tickFrequency: 0.01, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.00}"),
        EffectParameters.FloatSlider<PerlinNoiseImageEffect>("strength", "Strength", 0f, 100f, 50f, (e, v) => e.Strength = v,
            tickFrequency: 1, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0}"),
        EffectParameters.Bool<PerlinNoiseImageEffect>("monochrome", "Monochrome", true, (e, v) => e.Monochrome = v),
    ];

    public float Scale { get; set; } = 64f;
    public int Octaves { get; set; } = 4;
    public float Persistence { get; set; } = 0.5f;
    public float Strength { get; set; } = 50f;
    public bool Monochrome { get; set; } = true;

    // Classic Perlin permutation table (Ken Perlin's reference implementation)
    private static readonly byte[] Perm =
    [
        151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
        140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
        247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
        57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
        74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
        60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
        65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
        200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
        52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
        207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
        119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
        129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
        218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
        81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
        184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
        222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
    ];

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float strength = Math.Clamp(Strength, 0f, 100f) / 100f;
        if (strength <= 0f) return source.Copy();

        float scale = Math.Max(1f, Scale);
        int octaves = Math.Clamp(Octaves, 1, 8);
        float persistence = Math.Clamp(Persistence, 0f, 1f);
        bool monochrome = Monochrome;

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0) return source.Copy();

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float invScale = 1f / scale;

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];

                if (monochrome)
                {
                    float n = FractalPerlin(x * invScale, y * invScale, octaves, persistence, seed: 0);
                    // Map from [-1,1] to [0,1]
                    float noiseValue = (n + 1f) * 0.5f;

                    float r = Lerp(src.Red / 255f, noiseValue, strength);
                    float g = Lerp(src.Green / 255f, noiseValue, strength);
                    float b = Lerp(src.Blue / 255f, noiseValue, strength);

                    dstPixels[row + x] = new SKColor(
                        ClampToByte(r * 255f),
                        ClampToByte(g * 255f),
                        ClampToByte(b * 255f),
                        src.Alpha);
                }
                else
                {
                    float nr = (FractalPerlin(x * invScale, y * invScale, octaves, persistence, seed: 0) + 1f) * 0.5f;
                    float ng = (FractalPerlin(x * invScale, y * invScale, octaves, persistence, seed: 1) + 1f) * 0.5f;
                    float nb = (FractalPerlin(x * invScale, y * invScale, octaves, persistence, seed: 2) + 1f) * 0.5f;

                    float r = Lerp(src.Red / 255f, nr, strength);
                    float g = Lerp(src.Green / 255f, ng, strength);
                    float b = Lerp(src.Blue / 255f, nb, strength);

                    dstPixels[row + x] = new SKColor(
                        ClampToByte(r * 255f),
                        ClampToByte(g * 255f),
                        ClampToByte(b * 255f),
                        src.Alpha);
                }
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static float FractalPerlin(float x, float y, int octaves, float persistence, int seed)
    {
        float value = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float maxAmplitude = 0f;

        for (int i = 0; i < octaves; i++)
        {
            // Offset each octave by seed and octave index to decorrelate
            float ox = (seed * 137.3f) + (i * 47.7f);
            float oy = (seed * 251.1f) + (i * 89.3f);
            value += PerlinNoise(x * frequency + ox, y * frequency + oy) * amplitude;
            maxAmplitude += amplitude;
            frequency *= 2f;
            amplitude *= persistence;
        }

        return maxAmplitude > 0f ? value / maxAmplitude : 0f;
    }

    /// <summary>
    /// Classic 2D Perlin noise returning values in approximately [-1, 1].
    /// </summary>
    private static float PerlinNoise(float x, float y)
    {
        // Grid cell coordinates
        int xi = (int)MathF.Floor(x) & 255;
        int yi = (int)MathF.Floor(y) & 255;

        // Relative position within cell
        float xf = x - MathF.Floor(x);
        float yf = y - MathF.Floor(y);

        // Fade curves
        float u = Fade(xf);
        float v = Fade(yf);

        // Hash corners
        int aa = Perm[(Perm[xi] + yi) & 255];
        int ab = Perm[(Perm[xi] + yi + 1) & 255];
        int ba = Perm[(Perm[(xi + 1) & 255] + yi) & 255];
        int bb = Perm[(Perm[(xi + 1) & 255] + yi + 1) & 255];

        // Gradient dot products at each corner
        float g00 = Grad(aa, xf, yf);
        float g10 = Grad(ba, xf - 1f, yf);
        float g01 = Grad(ab, xf, yf - 1f);
        float g11 = Grad(bb, xf - 1f, yf - 1f);

        // Bilinear interpolation
        float lerpX0 = Lerp(g00, g10, u);
        float lerpX1 = Lerp(g01, g11, u);
        return Lerp(lerpX0, lerpX1, v);
    }

    private static float Fade(float t)
    {
        // 6t^5 - 15t^4 + 10t^3 (improved Perlin fade)
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    private static float Grad(int hash, float x, float y)
    {
        // Use lower 2 bits to select gradient direction
        return (hash & 3) switch
        {
            0 => x + y,
            1 => -x + y,
            2 => x - y,
            _ => -x - y,
        };
    }

    private static float Lerp(float a, float b, float t) => a + (b - a) * t;

    private static byte ClampToByte(float value)
    {
        if (value <= 0f) return 0;
        if (value >= 255f) return 255;
        return (byte)MathF.Round(value);
    }
}