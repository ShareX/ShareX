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

using System.Globalization;

namespace ShareX.PixelEditor.Core;

/// <summary>A built-in 5x7 bitmap font for crisp pixel text, plus rendering of system fonts without smoothing.</summary>
public static class PixelFont
{
    public const string BuiltInName = "Pixel 5×7";

    public const string BuiltInMonoName = "Pixel 5×7 Mono";

    // ASCII 32..126, five columns per glyph, bit 0 is the top row.
    private const string Data =
        "0000000000" + "00005F0000" + "0007000700" + "147F147F14" + "242A7F2A12" + "2313086462" + "3649552250" + "0005030000" +
        "001C224100" + "0041221C00" + "14083E0814" + "08083E0808" + "0050300000" + "0808080808" + "0060600000" + "2010080402" +
        "3E5149453E" + "00427F4000" + "4261514946" + "2141454B31" + "1814127F10" + "2745454539" + "3C4A494930" + "0171090503" +
        "3649494936" + "064949291E" + "0036360000" + "0056360000" + "0814224100" + "1414141414" + "0041221408" + "0201510906" +
        "324979413E" + "7E1111117E" + "7F49494936" + "3E41414122" + "7F4141221C" + "7F49494941" + "7F09090901" + "3E4149497A" +
        "7F0808087F" + "00417F4100" + "2040413F01" + "7F08142241" + "7F40404040" + "7F020C027F" + "7F0408107F" + "3E4141413E" +
        "7F09090906" + "3E4151215E" + "7F09192946" + "4649494931" + "01017F0101" + "3F4040403F" + "1F2040201F" + "3F4038403F" +
        "6314081463" + "0708700807" + "6151494543" + "007F414100" + "0204081020" + "0041417F00" + "0402010204" + "4040404040" +
        "0001020400" + "2054545478" + "7F48444438" + "3844444420" + "384444487F" + "3854545418" + "087E090102" + "0C5252523E" +
        "7F08040478" + "00447D4000" + "2040443D00" + "7F10284400" + "00417F4000" + "7C04180478" + "7C08040478" + "3844444438" +
        "7C14141408" + "081414187C" + "7C08040408" + "4854545420" + "043F444020" + "3C4040207C" + "1C2040201C" + "3C4030403C" +
        "4428102844" + "0C5050503C" + "4464544C44" + "0008364100" + "00007F0000" + "0041360800" + "0804081008";

    private static readonly byte[] Columns = Enumerable.Range(0, Data.Length / 2).Select(i => byte.Parse(Data.AsSpan(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)).ToArray();

    public static bool IsBuiltIn(string font) => font is BuiltInName or BuiltInMonoName;

    /// <summary>Draws text with the built-in font. Glyphs are packed proportionally unless monospace is asked for.</summary>
    public static PixelBuffer? RenderBuiltIn(string text, uint color, int scale, bool monospace)
    {
        scale = Math.Clamp(scale, 1, 32);
        string[] lines = text.Replace("\r", string.Empty).Split('\n');
        List<List<(int X, byte Column)>> rows = [];
        int width = 0;
        foreach (string line in lines)
        {
            List<(int, byte)> columns = [];
            int x = 0;
            foreach (char raw in line)
            {
                char c = raw is >= ' ' and <= '~' ? raw : '?';
                int glyph = (c - 32) * 5, first = 0, last = 4;
                if (!monospace)
                {
                    while (first < 4 && Columns[glyph + first] == 0) first++;
                    while (last > first && Columns[glyph + last] == 0) last--;
                    if (c == ' ') (first, last) = (0, 1);
                }
                for (int i = first; i <= last; i++) columns.Add((x++, Columns[glyph + i]));
                x++;
            }
            width = Math.Max(width, x - 1);
            rows.Add(columns);
        }
        if (width <= 0) return null;
        PixelBuffer buffer = new(width * scale, (rows.Count * 9 - 2) * scale);
        for (int r = 0; r < rows.Count; r++)
        {
            foreach ((int x, byte column) in rows[r])
            {
                for (int bit = 0; bit < 7; bit++)
                {
                    if ((column & (1 << bit)) == 0) continue;
                    for (int sy = 0; sy < scale; sy++)
                        for (int sx = 0; sx < scale; sx++)
                            buffer[x * scale + sx, (r * 9 + bit) * scale + sy] = color;
                }
            }
        }
        return buffer.IsEmpty ? null : buffer;
    }

    /// <summary>Draws text with a system font. Without smoothing every pixel is either the color or transparent.</summary>
    public static PixelBuffer? RenderSystem(string text, string family, int size, bool bold, bool italic, bool antialias, uint color)
    {
        using SKTypeface typeface = SKTypeface.FromFamilyName(family, bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright) ?? SKTypeface.Default;
        using SKFont font = new(typeface, size) { Edging = antialias ? SKFontEdging.Antialias : SKFontEdging.Alias, Hinting = SKFontHinting.Full, Subpixel = false };
        using SKPaint paint = new() { Color = SKColors.White, IsAntialias = antialias };
        string[] lines = text.Replace("\r", string.Empty).Split('\n');
        SKFontMetrics metrics = font.Metrics;
        float lineHeight = MathF.Ceiling(metrics.Descent - metrics.Ascent + metrics.Leading);
        int width = (int)MathF.Ceiling(lines.Max(l => font.MeasureText(l))) + size, height = (int)(lineHeight * lines.Length) + 4;
        if (width <= 0 || height <= 0 || width > Sprite.MaxSize || height > Sprite.MaxSize) return null;
        using SKBitmap bitmap = new(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
        using (SKCanvas canvas = new(bitmap))
        {
            canvas.Clear(SKColors.Transparent);
            for (int i = 0; i < lines.Length; i++) canvas.DrawText(lines[i], size / 2f, 2 + i * lineHeight - metrics.Ascent, SKTextAlign.Left, font, paint);
        }
        PixelBuffer coverage = PixelBuffer.FromBitmap(bitmap);
        for (int i = 0; i < coverage.Pixels.Length; i++)
        {
            int alpha = Argb.A(coverage.Pixels[i]);
            if (!antialias) alpha = alpha >= 128 ? 255 : 0;
            coverage.Pixels[i] = alpha == 0 ? 0 : Argb.WithAlpha(color, alpha * Argb.A(color) / 255);
        }
        return coverage.ContentBounds() is SKRectI bounds ? coverage.Crop(bounds) : null;
    }
}
