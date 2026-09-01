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

public sealed class MatrixDigitalRainImageEffect : ImageEffectBase
{
    public override string Id => "matrix_digital_rain";
    public override string Name => "Matrix digital rain";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.terminal;
    public override string Description => "Overlays a Matrix-style digital rain effect using source luminance.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<MatrixDigitalRainImageEffect>("cell_size", "Cell size", 6, 24, 12, (e, v) => e.CellSize = v),
        EffectParameters.FloatSlider<MatrixDigitalRainImageEffect>("density", "Density", 0, 100, 85, (e, v) => e.Density = v),
        EffectParameters.IntSlider<MatrixDigitalRainImageEffect>("trail_length", "Trail length", 3, 50, 12, (e, v) => e.TrailLength = v),
        EffectParameters.FloatSlider<MatrixDigitalRainImageEffect>("glow_amount", "Glow amount", 0, 100, 40, (e, v) => e.GlowAmount = v),
        EffectParameters.FloatSlider<MatrixDigitalRainImageEffect>("source_blend", "Source blend", 0, 100, 22, (e, v) => e.SourceBlend = v),
        EffectParameters.FloatSlider<MatrixDigitalRainImageEffect>("rain_offset", "Rain offset", 0, 100, 0, (e, v) => e.RainOffset = v),
        EffectParameters.FloatSlider<MatrixDigitalRainImageEffect>("luminance_influence", "Luminance influence", 0, 100, 65, (e, v) => e.LuminanceInfluence = v),
        EffectParameters.Text<MatrixDigitalRainImageEffect>("character_set", "Character set", "01<>[]{}*+-/\\=#$%&", (e, v) => e.CharacterSet = v),
    ];

    public int CellSize { get; set; } = 12; // 6..24
    public float Density { get; set; } = 85f; // 0..100
    public int TrailLength { get; set; } = 12; // 3..50
    public float GlowAmount { get; set; } = 40f; // 0..100
    public float SourceBlend { get; set; } = 22f; // 0..100
    public float RainOffset { get; set; } = 0f; // 0..100
    public float LuminanceInfluence { get; set; } = 65f; // 0..100
    public string CharacterSet { get; set; } = "01<>[]{}*+-/\\=#$%&";
    public int Seed { get; set; } = 1337;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        int cell = Math.Clamp(CellSize, 6, 24);
        float density = Math.Clamp(Density, 0f, 100f) / 100f;
        int trail = Math.Clamp(TrailLength, 3, 50);
        float glow = Math.Clamp(GlowAmount, 0f, 100f) / 100f;
        float sourceBlend = Math.Clamp(SourceBlend, 0f, 100f) / 100f;
        float rainOffset = Math.Clamp(RainOffset, 0f, 100f) / 100f;
        float lumInfluence = Math.Clamp(LuminanceInfluence, 0f, 100f) / 100f;
        string chars = NormalizeCharacters(CharacterSet);

        int columns = (int)Math.Ceiling(width / (float)cell);
        int rows = (int)Math.Ceiling(height / (float)cell);

        SKColor[] srcPixels = source.Pixels;
        SKBitmap result = new SKBitmap(width, height, source.ColorType, source.AlphaType);

        using SKCanvas canvas = new SKCanvas(result);
        using SKPaint backgroundPaint = new SKPaint
        {
            Color = new SKColor(0, 12, 0, 255)
        };
        canvas.DrawRect(new SKRect(0, 0, width, height), backgroundPaint);

        if (sourceBlend > 0f)
        {
            using SKPaint srcBlendPaint = new SKPaint
            {
                Color = new SKColor(255, 255, 255, ProceduralEffectHelper.ClampToByte(sourceBlend * 255f))
            };
            canvas.DrawBitmap(source, 0, 0, srcBlendPaint);
        }

        SKTypeface? customTypeface = SKTypeface.FromFamilyName("Consolas");
        using SKFont glyphFont = new SKFont(customTypeface ?? SKTypeface.Default, cell * 1.02f);
        using SKPaint glyphPaint = new SKPaint
        {
            IsAntialias = true
        };
        using SKPaint glowPaint = new SKPaint
        {
            IsAntialias = true
        };
        if (glow > 0f)
        {
            glowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 2.2f + (glow * 3.4f));
        }

        float baseline = cell * 0.88f;

        for (int col = 0; col < columns; col++)
        {
            float head = ((ProceduralEffectHelper.Hash01(col, Seed, Seed ^ 9) + rainOffset) % 1f) * rows;
            float secondaryHead = ((ProceduralEffectHelper.Hash01(col, Seed ^ 37, Seed ^ 101) + (rainOffset * 0.63f)) % 1f) * rows;

            for (int row = 0; row < rows; row++)
            {
                int x0 = col * cell;
                int y0 = row * cell;
                if (x0 >= width || y0 >= height)
                {
                    continue;
                }

                int x1 = Math.Min(width, x0 + cell);
                int y1 = Math.Min(height, y0 + cell);

                GetAverageLuminance(srcPixels, width, x0, y0, x1, y1, out float lum, out byte alpha);

                float spawn = ProceduralEffectHelper.Hash01(col, row, Seed ^ 503);
                float localDensity = density * (0.35f + (0.65f * (lumInfluence * lum + (1f - lumInfluence))));
                if (spawn > localDensity)
                {
                    continue;
                }

                float t1 = RowTrailFactor(row, head, rows, trail);
                float t2 = RowTrailFactor(row, secondaryHead, rows, trail * 0.75f);
                float trailFactor = Math.Max(t1, t2);

                float baseIntensity = (lum * lumInfluence) + (0.35f * (1f - lumInfluence));
                float intensity = Math.Clamp((baseIntensity * 0.55f) + (trailFactor * 0.9f), 0f, 1.2f);
                if (intensity <= 0.03f)
                {
                    continue;
                }

                int charIndex = (int)MathF.Abs(MathF.Floor(((lum * chars.Length) + (row * 0.61f) + (col * 0.17f)) % chars.Length));
                if (charIndex >= chars.Length) charIndex = chars.Length - 1;
                char glyph = chars[charIndex];

                float glowStrength = glow * MathF.Max(0f, intensity - 0.35f);
                if (glowStrength > 0f)
                {
                    glowPaint.Color = new SKColor(
                        ProceduralEffectHelper.ClampToByte(56f + (intensity * 72f)),
                        ProceduralEffectHelper.ClampToByte(180f + (intensity * 60f)),
                        ProceduralEffectHelper.ClampToByte(56f + (intensity * 70f)),
                        ProceduralEffectHelper.ClampToByte(glowStrength * 180f));
                    canvas.DrawText(glyph.ToString(), x0, y0 + baseline, glyphFont, glowPaint);
                }

                glyphPaint.MaskFilter = null;
                glyphPaint.Color = new SKColor(
                    ProceduralEffectHelper.ClampToByte(28f + (intensity * 118f)),
                    ProceduralEffectHelper.ClampToByte(120f + (intensity * 135f)),
                    ProceduralEffectHelper.ClampToByte(28f + (intensity * 118f)),
                    alpha);
                canvas.DrawText(glyph.ToString(), x0, y0 + baseline, glyphFont, glyphPaint);
            }
        }

        customTypeface?.Dispose();
        return result;
    }

    private static string NormalizeCharacters(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "01<>[]{}*+-/\\=#$%&";
        }

        string trimmed = input.Trim();
        if (trimmed.Length == 1)
        {
            return trimmed + "0";
        }

        return trimmed;
    }

    private static float RowTrailFactor(int row, float head, int rows, float length)
    {
        float dist = row - head;
        if (dist < 0f) dist += rows;
        if (dist < 0f || dist > length) return 0f;
        return 1f - (dist / Math.Max(1f, length));
    }

    private static void GetAverageLuminance(SKColor[] pixels, int width, int x0, int y0, int x1, int y1, out float luminance, out byte alpha)
    {
        float sumLum = 0f;
        float sumAlpha = 0f;
        int count = 0;

        for (int y = y0; y < y1; y++)
        {
            int row = y * width;
            for (int x = x0; x < x1; x++)
            {
                SKColor c = pixels[row + x];
                sumLum += ((0.2126f * c.Red) + (0.7152f * c.Green) + (0.0722f * c.Blue)) / 255f;
                sumAlpha += c.Alpha;
                count++;
            }
        }

        if (count == 0)
        {
            luminance = 0f;
            alpha = 255;
            return;
        }

        float inv = 1f / count;
        luminance = sumLum * inv;
        alpha = ProceduralEffectHelper.ClampToByte(sumAlpha * inv);
    }
}