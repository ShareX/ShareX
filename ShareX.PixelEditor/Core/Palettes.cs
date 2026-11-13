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
using System.Text;

namespace ShareX.PixelEditor.Core;

public sealed record PalettePreset(string Name, uint[] Colors);

public enum RemapDither
{
    None,
    Ordered,
    FloydSteinberg
}

/// <summary>Built-in palettes, palette files and color reduction.</summary>
public static class Palettes
{
    private static uint[] Parse(string hex) => hex.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(h => 0xFF000000 | uint.Parse(h, NumberStyles.HexNumber, CultureInfo.InvariantCulture)).ToArray();

    public static IReadOnlyList<PalettePreset> Presets { get; } =
    [
        new("DawnBringer 32", Parse("000000 222034 45283c 663931 8f563b df7126 d9a066 eec39a fbf236 99e550 6abe30 37946e 4b692f 524b24 323c39 3f3f74 306082 5b6ee1 639bff 5fcde4 cbdbfc ffffff 9badb7 847e87 696a6a 595652 76428a ac3232 d95763 d77bba 8f974a 8a6f30")),
        new("DawnBringer 16", Parse("140c1c 442434 30346d 4e4a4e 854c30 346524 d04648 757161 597dce d27d2c 8595a1 6daa2c d2aa99 6dc2ca dad45e deeed6")),
        new("Endesga 32", Parse("be4a2f d77643 ead4aa e4a672 b86f50 733e39 3e2731 a22633 e43b44 f77622 feae34 fee761 63c74d 3e8948 265c42 193c3e 124e89 0099db 2ce8f5 ffffff c0cbdc 8b9bb4 5a6988 3a4466 262b44 181425 ff0044 68386c b55088 f6757a e8b796 c28569")),
        new("Sweetie 16", Parse("1a1c2c 5d275d b13e53 ef7d57 ffcd75 a7f070 38b764 257179 29366f 3b5dc9 41a6f6 73eff7 f4f4f4 94b0c2 566c86 333c57")),
        new("PICO-8", Parse("000000 1d2b53 7e2553 008751 ab5236 5f574f c2c3c7 fff1e8 ff004d ffa300 ffec27 00e436 29adff 83769c ff77a8 ffccaa")),
        new("Arne 16", Parse("000000 9d9d9d ffffff be2633 e06f8b 493c2b a46422 eb8931 f7e26b 2f484e 44891a a3ce27 1b2632 005784 31a2f2 b2dcef")),
        new("Game Boy", Parse("0f380f 306230 8bac0f 9bbc0f")),
        new("CGA", Parse("000000 0000aa 00aa00 00aaaa aa0000 aa00aa aa5500 aaaaaa 555555 5555ff 55ff55 55ffff ff5555 ff55ff ffff55 ffffff")),
        new("Commodore 64", Parse("000000 ffffff 68372b 70a4b2 6f3d86 588d43 352879 b8c76f 6f4f25 433900 9a6759 444444 6c6c6c 9ad284 6c5eb5 959595")),
        new("ZX Spectrum", Parse("000000 0000d7 d70000 d700d7 00d700 00d7d7 d7d700 d7d7d7 0000ff ff0000 ff00ff 00ff00 00ffff ffff00 ffffff")),
        new("Grayscale 16", Enumerable.Range(0, 16).Select(i => Argb.Make(255, i * 17, i * 17, i * 17)).ToArray()),
        new("Grayscale 4", Parse("000000 555555 aaaaaa ffffff")),
        new("1-Bit", Parse("000000 ffffff")),
        new("RGB 3-3-2", Enumerable.Range(0, 256).Select(i => Argb.Make(255, (i >> 5) * 255 / 7, ((i >> 2) & 7) * 255 / 7, (i & 3) * 255 / 3)).ToArray()),
        new("Web Safe", Enumerable.Range(0, 216).Select(i => Argb.Make(255, i / 36 * 51, i / 6 % 6 * 51, i % 6 * 51)).ToArray())
    ];

    public static uint[] Default => Presets[0].Colors;

    // ------------------------------------------------------------------ files

    public static readonly string[] FileExtensions = [".gpl", ".hex", ".pal", ".txt", ".png", ".gif", ".bmp"];

    /// <summary>Reads a GIMP (.gpl), hex list (.hex, .txt), JASC (.pal) palette or the colors of an image.</summary>
    public static List<uint> Load(string path)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension is ".png" or ".gif" or ".bmp" or ".jpg" or ".jpeg" or ".webp")
        {
            using SKBitmap bitmap = SKBitmap.Decode(path) ?? throw new InvalidDataException();
            return FromImage(PixelBuffer.FromBitmap(bitmap), 256);
        }
        string[] lines = File.ReadAllLines(path);
        List<uint> colors = [];
        bool jasc = lines.Length > 0 && lines[0].Trim().Equals("JASC-PAL", StringComparison.OrdinalIgnoreCase);
        bool gimp = lines.Length > 0 && lines[0].Trim().StartsWith("GIMP Palette", StringComparison.OrdinalIgnoreCase);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.Length == 0 || line.StartsWith('#') && (gimp || jasc) || line.StartsWith(';')) continue;
            if (jasc || gimp)
            {
                if (jasc && i < 3) continue;
                string[] parts = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3 && int.TryParse(parts[0], out int r) && int.TryParse(parts[1], out int g) && int.TryParse(parts[2], out int b))
                {
                    int a = jasc && parts.Length >= 4 && int.TryParse(parts[3], out int alpha) ? alpha : 255;
                    colors.Add(Argb.Make(a, r, g, b));
                }
                continue;
            }
            // Hex lists: one color per line, optionally prefixed with # or 0x, sometimes several per line.
            foreach (string token in line.Split([' ', '\t', ','], StringSplitOptions.RemoveEmptyEntries))
            {
                string hex = token.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? token[2..] : token;
                if (Argb.TryParseHex(hex, out uint color)) colors.Add(color);
            }
        }
        if (colors.Count == 0) throw new InvalidDataException();
        return colors;
    }

    public static void Save(string path, IReadOnlyList<uint> colors, string name)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();
        StringBuilder text = new();
        switch (extension)
        {
            case ".gpl":
                text.Append("GIMP Palette\n").Append("Name: ").Append(name).Append('\n').Append("Columns: 8\n#\n");
                foreach (uint c in colors) text.Append(CultureInfo.InvariantCulture, $"{Argb.R(c),3} {Argb.G(c),3} {Argb.B(c),3}\t#{Argb.ToHex(Argb.Opaque(c))}\n");
                break;
            case ".pal":
                text.Append("JASC-PAL\r\n0100\r\n").Append(colors.Count).Append("\r\n");
                foreach (uint c in colors) text.Append(CultureInfo.InvariantCulture, $"{Argb.R(c)} {Argb.G(c)} {Argb.B(c)}\r\n");
                break;
            case ".png":
                PixelBuffer strip = new(colors.Count, 1);
                for (int i = 0; i < colors.Count; i++) strip.Pixels[i] = colors[i];
                using (SKBitmap bitmap = strip.ToBitmap())
                using (SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100))
                    File.WriteAllBytes(path, data.ToArray());
                return;
            default:
                foreach (uint c in colors) text.Append(Argb.ToHex(Argb.Opaque(c)).ToLowerInvariant()).Append('\n');
                break;
        }
        File.WriteAllText(path, text.ToString());
    }

    // ------------------------------------------------------------------ color reduction

    /// <summary>The distinct colors of an image in order of first use, reduced with median cut when there are too many.</summary>
    public static List<uint> FromImage(PixelBuffer buffer, int maxColors) => FromImages([buffer], maxColors);

    public static List<uint> FromImages(IEnumerable<PixelBuffer> buffers, int maxColors)
    {
        Dictionary<uint, int> histogram = [];
        List<uint> order = [];
        foreach (PixelBuffer buffer in buffers)
        {
            foreach (uint p in buffer.Pixels)
            {
                if (p >> 24 == 0) continue;
                uint key = Argb.Opaque(p);
                if (histogram.TryGetValue(key, out int count)) histogram[key] = count + 1;
                else
                {
                    histogram[key] = 1;
                    order.Add(key);
                }
            }
        }
        return order.Count <= maxColors ? order : MedianCut(histogram, maxColors);
    }

    private sealed class Box
    {
        public List<(uint Color, int Count)> Colors = [];
        public int Range, Axis;

        public void Measure()
        {
            int minR = 255, minG = 255, minB = 255, maxR = 0, maxG = 0, maxB = 0;
            foreach ((uint c, _) in Colors)
            {
                minR = Math.Min(minR, Argb.R(c));
                maxR = Math.Max(maxR, Argb.R(c));
                minG = Math.Min(minG, Argb.G(c));
                maxG = Math.Max(maxG, Argb.G(c));
                minB = Math.Min(minB, Argb.B(c));
                maxB = Math.Max(maxB, Argb.B(c));
            }
            int r = maxR - minR, g = maxG - minG, b = maxB - minB;
            Range = Math.Max(r, Math.Max(g, b));
            Axis = g >= r && g >= b ? 1 : r >= b ? 0 : 2;
        }
    }

    private static List<uint> MedianCut(Dictionary<uint, int> histogram, int maxColors)
    {
        Box first = new() { Colors = histogram.Select(p => (p.Key, p.Value)).ToList() };
        first.Measure();
        List<Box> boxes = [first];
        while (boxes.Count < maxColors)
        {
            Box? box = boxes.Where(b => b.Colors.Count > 1).OrderByDescending(b => b.Range * Math.Log(b.Colors.Count + 1)).FirstOrDefault();
            if (box == null) break;
            int axis = box.Axis;
            box.Colors.Sort((a, b) => Component(a.Color, axis).CompareTo(Component(b.Color, axis)));
            long total = box.Colors.Sum(c => (long)c.Count), half = 0;
            int split = 0;
            for (; split < box.Colors.Count - 1; split++)
            {
                half += box.Colors[split].Count;
                if (half >= total / 2) break;
            }
            split = Math.Clamp(split + 1, 1, box.Colors.Count - 1);
            Box other = new() { Colors = box.Colors.GetRange(split, box.Colors.Count - split) };
            box.Colors.RemoveRange(split, box.Colors.Count - split);
            box.Measure();
            other.Measure();
            boxes.Add(other);
        }
        List<uint> result = [];
        foreach (Box box in boxes)
        {
            long r = 0, g = 0, b = 0, n = 0;
            foreach ((uint c, int count) in box.Colors)
            {
                r += (long)Argb.R(c) * count;
                g += (long)Argb.G(c) * count;
                b += (long)Argb.B(c) * count;
                n += count;
            }
            if (n > 0) result.Add(Argb.Make(255, (int)(r / n), (int)(g / n), (int)(b / n)));
        }
        return result.Distinct().ToList();
    }

    private static int Component(uint c, int axis) => axis == 0 ? Argb.R(c) : axis == 1 ? Argb.G(c) : Argb.B(c);

    public static int Nearest(IReadOnlyList<uint> palette, uint color)
    {
        int best = 0;
        long bestDistance = long.MaxValue;
        for (int i = 0; i < palette.Count; i++)
        {
            int dr = Argb.R(palette[i]) - Argb.R(color), dg = Argb.G(palette[i]) - Argb.G(color), db = Argb.B(palette[i]) - Argb.B(color);
            long d = 2 * dr * dr + 4 * dg * dg + 3 * db * db;
            if (d < bestDistance)
            {
                bestDistance = d;
                best = i;
            }
        }
        return best;
    }

    /// <summary>Maps every pixel to its nearest palette color, optionally with dithering. Alpha is kept, or made binary.</summary>
    public static PixelBuffer Remap(PixelBuffer source, IReadOnlyList<uint> palette, RemapDither dither, bool binaryAlpha, bool[]? mask = null)
    {
        PixelBuffer result = source.Clone();
        if (palette.Count == 0) return result;
        int w = source.Width, h = source.Height;
        Dictionary<uint, uint> cache = [];
        uint Map(uint rgb)
        {
            if (!cache.TryGetValue(rgb, out uint mapped))
            {
                mapped = palette[Nearest(palette, rgb)] & 0xFFFFFF;
                cache[rgb] = mapped;
            }
            return mapped;
        }
        float[]? errors = dither == RemapDither.FloydSteinberg ? new float[w * h * 3] : null;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int i = y * w + x;
                uint p = source.Pixels[i];
                int a = Argb.A(p);
                if (a == 0 || mask != null && !mask[i]) continue;
                if (binaryAlpha) a = a >= 128 ? 255 : 0;
                if (a == 0)
                {
                    result.Pixels[i] = 0;
                    continue;
                }
                int r = Argb.R(p), g = Argb.G(p), b = Argb.B(p);
                if (dither == RemapDither.Ordered)
                {
                    int offset = (int)((Raster.BayerThreshold(x, y, false) - 0.5f) * 48);
                    r += offset;
                    g += offset;
                    b += offset;
                }
                else if (errors != null)
                {
                    r += (int)MathF.Round(errors[i * 3]);
                    g += (int)MathF.Round(errors[i * 3 + 1]);
                    b += (int)MathF.Round(errors[i * 3 + 2]);
                }
                uint wanted = Argb.Make(255, r, g, b) & 0xFFFFFF;
                uint mapped = Map(wanted);
                result.Pixels[i] = mapped | ((uint)a << 24);
                if (errors != null)
                {
                    float er = Math.Clamp(r, 0, 255) - Argb.R(mapped), eg = Math.Clamp(g, 0, 255) - Argb.G(mapped), eb = Math.Clamp(b, 0, 255) - Argb.B(mapped);
                    void Spread(int nx, int ny, float factor)
                    {
                        if ((uint)nx >= (uint)w || (uint)ny >= (uint)h) return;
                        int n = (ny * w + nx) * 3;
                        errors[n] += er * factor;
                        errors[n + 1] += eg * factor;
                        errors[n + 2] += eb * factor;
                    }
                    Spread(x + 1, y, 7 / 16f);
                    Spread(x - 1, y + 1, 3 / 16f);
                    Spread(x, y + 1, 5 / 16f);
                    Spread(x + 1, y + 1, 1 / 16f);
                }
            }
        }
        return result;
    }

    /// <summary>Sorts colors by hue, brightness or saturation.</summary>
    public static List<uint> Sort(IEnumerable<uint> colors, string by) => by switch
    {
        "hue" => colors.OrderBy(c => Argb.ToHsv(c).S < 0.08f ? -1 : Argb.ToHsv(c).H).ThenBy(Argb.Luma).ToList(),
        "saturation" => colors.OrderBy(c => Argb.ToHsv(c).S).ThenBy(Argb.Luma).ToList(),
        "reverse" => colors.Reverse().ToList(),
        _ => colors.OrderBy(Argb.Luma).ToList()
    };

    /// <summary>A ramp of evenly spaced colors between two colors, including both ends.</summary>
    public static List<uint> Ramp(uint from, uint to, int steps)
    {
        List<uint> result = [];
        (float h0, float s0, float v0) = Argb.ToHsv(from);
        (float h1, float s1, float v1) = Argb.ToHsv(to);
        float dh = h1 - h0;
        if (dh > 180) dh -= 360;
        if (dh < -180) dh += 360;
        // Gray ends have no hue of their own, so they take the hue of the other end.
        if (s0 < 0.02f) { h0 = h1; dh = 0; }
        if (s1 < 0.02f) dh = 0;
        for (int i = 0; i < steps; i++)
        {
            float t = steps <= 1 ? 0 : i / (float)(steps - 1);
            result.Add(Argb.FromHsv(h0 + dh * t, s0 + (s1 - s0) * t, v0 + (v1 - v0) * t));
        }
        return result;
    }
}
