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

public sealed class ASCIIArtImageEffect : ImageEffectBase
{
    public override string Id => "ascii_art";
    public override string Name => "ASCII art";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.square_terminal;
    public override string Description => "Converts the image into ASCII character art.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<ASCIIArtImageEffect>("cell_size", "Cell size", 4, 24, 8, (e, v) => e.CellSize = v),
        EffectParameters.FloatSlider<ASCIIArtImageEffect>("contrast", "Contrast", 50, 200, 110, (e, v) => e.Contrast = v),
        EffectParameters.Text<ASCIIArtImageEffect>("character_set", "Character set", "@%#*+=-:. ", (e, v) => e.CharacterSet = v),
        EffectParameters.Bool<ASCIIArtImageEffect>("invert", "Invert", false, (e, v) => e.Invert = v),
        EffectParameters.Bool<ASCIIArtImageEffect>("dark_background", "Dark background", true, (e, v) => e.DarkBackground = v),
        EffectParameters.Bool<ASCIIArtImageEffect>("use_source_color", "Use source color", true, (e, v) => e.UseSourceColor = v)
    ];

    public int CellSize { get; set; } = 8; // 4..24
    public float Contrast { get; set; } = 110f; // 50..200
    public string CharacterSet { get; set; } = "@%#*+=-:. ";
    public bool Invert { get; set; }
    public bool DarkBackground { get; set; } = true;
    public bool UseSourceColor { get; set; } = true;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int cell = Math.Clamp(CellSize, 4, 24);
        float contrast = Math.Clamp(Contrast, 50f, 200f) / 100f;
        string charset = NormalizeCharacterSet(CharacterSet);

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKBitmap result = new SKBitmap(width, height, source.ColorType, source.AlphaType);
        SKColor[] srcPixels = source.Pixels;

        using SKCanvas canvas = new SKCanvas(result);
        canvas.Clear(DarkBackground ? new SKColor(12, 12, 12, 255) : SKColors.White);

        SKTypeface? customTypeface = SKTypeface.FromFamilyName("Consolas");
        using SKFont font = new SKFont(customTypeface ?? SKTypeface.Default, cell * 1.02f);
        using SKPaint paint = new SKPaint
        {
            IsAntialias = true
        };

        int columns = (int)Math.Ceiling(width / (float)cell);
        int rows = (int)Math.Ceiling(height / (float)cell);
        float baselineOffset = cell * 0.90f;

        for (int row = 0; row < rows; row++)
        {
            int y0 = row * cell;
            int y1 = Math.Min(height, y0 + cell);

            for (int col = 0; col < columns; col++)
            {
                int x0 = col * cell;
                int x1 = Math.Min(width, x0 + cell);

                if (x0 >= x1 || y0 >= y1)
                {
                    continue;
                }

                GetAverageColor(srcPixels, width, x0, y0, x1, y1, out float avgR, out float avgG, out float avgB, out byte avgA);

                float luminance = ((0.2126f * avgR) + (0.7152f * avgG) + (0.0722f * avgB)) / 255f;
                luminance = Math.Clamp(((luminance - 0.5f) * contrast) + 0.5f, 0f, 1f);
                if (Invert)
                {
                    luminance = 1f - luminance;
                }

                int index = (int)MathF.Round(luminance * (charset.Length - 1));
                char c = charset[Math.Clamp(index, 0, charset.Length - 1)];
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                paint.Color = UseSourceColor
                    ? new SKColor(
                        ProceduralEffectHelper.ClampToByte(avgR),
                        ProceduralEffectHelper.ClampToByte(avgG),
                        ProceduralEffectHelper.ClampToByte(avgB),
                        avgA)
                    : GetMonochromeColor(DarkBackground, luminance, avgA);

                canvas.DrawText(c.ToString(), x0, y0 + baselineOffset, font, paint);
            }
        }

        customTypeface?.Dispose();
        return result;
    }

    private static string NormalizeCharacterSet(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "@%#*+=-:. ";
        }

        // Keep insertion order but remove duplicate glyphs.
        HashSet<char> seen = new HashSet<char>();
        List<char> chars = new List<char>(input.Length);
        foreach (char c in input)
        {
            if (seen.Add(c))
            {
                chars.Add(c);
            }
        }

        if (chars.Count == 0)
        {
            return "@%#*+=-:. ";
        }

        if (chars.Count == 1)
        {
            chars.Add(' ');
        }

        return new string(chars.ToArray());
    }

    private static void GetAverageColor(
        SKColor[] pixels,
        int width,
        int x0,
        int y0,
        int x1,
        int y1,
        out float avgR,
        out float avgG,
        out float avgB,
        out byte avgA)
    {
        float sumR = 0f;
        float sumG = 0f;
        float sumB = 0f;
        float sumA = 0f;
        int count = 0;

        for (int y = y0; y < y1; y++)
        {
            int row = y * width;
            for (int x = x0; x < x1; x++)
            {
                SKColor c = pixels[row + x];
                sumR += c.Red;
                sumG += c.Green;
                sumB += c.Blue;
                sumA += c.Alpha;
                count++;
            }
        }

        if (count == 0)
        {
            avgR = 0f;
            avgG = 0f;
            avgB = 0f;
            avgA = 0;
            return;
        }

        float inv = 1f / count;
        avgR = sumR * inv;
        avgG = sumG * inv;
        avgB = sumB * inv;
        avgA = ProceduralEffectHelper.ClampToByte(sumA * inv);
    }

    private static SKColor GetMonochromeColor(bool darkBackground, float luminance, byte alpha)
    {
        if (darkBackground)
        {
            byte v = ProceduralEffectHelper.ClampToByte((0.80f + (0.20f * luminance)) * 255f);
            return new SKColor(v, v, v, alpha);
        }

        byte value = ProceduralEffectHelper.ClampToByte((0.10f + (0.55f * (1f - luminance))) * 255f);
        return new SKColor(value, value, value, alpha);
    }
}