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

/// <summary>Helpers for straight (non-premultiplied) 0xAARRGGBB colors, the pixel format of every sprite buffer.</summary>
public static class Argb
{
    public const uint Transparent = 0;
    public const uint Black = 0xFF000000;
    public const uint White = 0xFFFFFFFF;

    public static int A(uint c) => (int)(c >> 24);

    public static int R(uint c) => (int)((c >> 16) & 0xFF);

    public static int G(uint c) => (int)((c >> 8) & 0xFF);

    public static int B(uint c) => (int)(c & 0xFF);

    public static uint Make(int a, int r, int g, int b) =>
        ((uint)Math.Clamp(a, 0, 255) << 24) | ((uint)Math.Clamp(r, 0, 255) << 16) | ((uint)Math.Clamp(g, 0, 255) << 8) | (uint)Math.Clamp(b, 0, 255);

    public static uint WithAlpha(uint c, int a) => (c & 0x00FFFFFF) | ((uint)Math.Clamp(a, 0, 255) << 24);

    public static uint Opaque(uint c) => c | 0xFF000000;

    public static uint FromSK(SKColor c) => (uint)c;

    public static SKColor ToSK(uint c) => new(c);

    public static Avalonia.Media.Color ToAvalonia(uint c) => Avalonia.Media.Color.FromArgb((byte)A(c), (byte)R(c), (byte)G(c), (byte)B(c));

    public static uint FromAvalonia(Avalonia.Media.Color c) => Make(c.A, c.R, c.G, c.B);

    /// <summary>RRGGBB, or RRGGBBAA when the color is not opaque.</summary>
    public static string ToHex(uint c) => A(c) == 255 ? $"{c & 0xFFFFFF:X6}" : $"{c & 0xFFFFFF:X6}{A(c):X2}";

    public static bool TryParseHex(string? text, out uint color)
    {
        color = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;
        string s = text.Trim().TrimStart('#');
        if (s.Length == 3) s = string.Concat(s[0], s[0], s[1], s[1], s[2], s[2]);
        if (s.Length != 6 && s.Length != 8) return false;
        if (!uint.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint value)) return false;
        color = s.Length == 6 ? 0xFF000000 | value : ((value & 0xFF) << 24) | (value >> 8);
        return true;
    }

    public static uint Premultiply(uint c)
    {
        uint a = c >> 24;
        if (a == 255) return c;
        if (a == 0) return 0;
        uint r = (((c >> 16) & 0xFF) * a + 127) / 255, g = (((c >> 8) & 0xFF) * a + 127) / 255, b = ((c & 0xFF) * a + 127) / 255;
        return (a << 24) | (r << 16) | (g << 8) | b;
    }

    /// <summary>Source-over compositing of <paramref name="src"/> on <paramref name="dst"/> with an extra 0..255 opacity.</summary>
    public static uint Over(uint dst, uint src, int opacity = 255)
    {
        int sa = A(src);
        if (opacity != 255) sa = (sa * opacity + 127) / 255;
        if (sa <= 0) return dst;
        int da = A(dst);
        if (sa == 255 || da == 0) return (src & 0x00FFFFFF) | ((uint)sa << 24);
        int rest = da * (255 - sa);
        int outA255 = sa * 255 + rest;
        int r = (R(src) * sa * 255 + R(dst) * rest + outA255 / 2) / outA255;
        int g = (G(src) * sa * 255 + G(dst) * rest + outA255 / 2) / outA255;
        int b = (B(src) * sa * 255 + B(dst) * rest + outA255 / 2) / outA255;
        return Make((outA255 + 127) / 255, r, g, b);
    }

    public static uint Lerp(uint a, uint b, float t)
    {
        t = Math.Clamp(t, 0, 1);
        int aa = A(a), ba = A(b);
        float outA = aa + (ba - aa) * t;
        if (outA <= 0) return 0;
        // Interpolate premultiplied so transparent ends do not bleed their hidden color.
        float wa = aa * (1 - t), wb = ba * t;
        float sum = wa + wb;
        if (sum <= 0) return 0;
        return Make((int)MathF.Round(outA), (int)MathF.Round((R(a) * wa + R(b) * wb) / sum), (int)MathF.Round((G(a) * wa + G(b) * wb) / sum), (int)MathF.Round((B(a) * wa + B(b) * wb) / sum));
    }

    /// <summary>Squared color distance that also weighs alpha; fully transparent colors are all equal.</summary>
    public static int Distance(uint a, uint b)
    {
        int aa = A(a), ba = A(b);
        if (aa == 0 && ba == 0) return 0;
        int dr = R(a) - R(b), dg = G(a) - G(b), db = B(a) - B(b), da = aa - ba;
        return 2 * dr * dr + 4 * dg * dg + 3 * db * db + 3 * da * da;
    }

    public static int Luma(uint c) => (R(c) * 299 + G(c) * 587 + B(c) * 114) / 1000;

    /// <summary>Hue 0..360, saturation and value 0..1.</summary>
    public static (float H, float S, float V) ToHsv(uint c)
    {
        float r = R(c) / 255f, g = G(c) / 255f, b = B(c) / 255f;
        float max = Math.Max(r, Math.Max(g, b)), min = Math.Min(r, Math.Min(g, b)), d = max - min;
        float h = 0;
        if (d > 0)
        {
            if (max == r) h = 60 * (((g - b) / d) % 6);
            else if (max == g) h = 60 * ((b - r) / d + 2);
            else h = 60 * ((r - g) / d + 4);
            if (h < 0) h += 360;
        }
        return (h, max <= 0 ? 0 : d / max, max);
    }

    public static uint FromHsv(float h, float s, float v, int a = 255)
    {
        h = ((h % 360) + 360) % 360;
        s = Math.Clamp(s, 0, 1);
        v = Math.Clamp(v, 0, 1);
        float c = v * s, x = c * (1 - MathF.Abs(h / 60 % 2 - 1)), m = v - c;
        (float r, float g, float b) = ((int)(h / 60)) switch
        {
            0 => (c, x, 0f),
            1 => (x, c, 0f),
            2 => (0f, c, x),
            3 => (0f, x, c),
            4 => (x, 0f, c),
            _ => (c, 0f, x)
        };
        return Make(a, (int)MathF.Round((r + m) * 255), (int)MathF.Round((g + m) * 255), (int)MathF.Round((b + m) * 255));
    }

    /// <summary>Hue 0..360, saturation and lightness 0..1.</summary>
    public static (float H, float S, float L) ToHsl(uint c)
    {
        float r = R(c) / 255f, g = G(c) / 255f, b = B(c) / 255f;
        float max = Math.Max(r, Math.Max(g, b)), min = Math.Min(r, Math.Min(g, b)), d = max - min;
        float l = (max + min) / 2;
        if (d <= 0) return (0, 0, l);
        float s = d / (1 - MathF.Abs(2 * l - 1));
        float h;
        if (max == r) h = 60 * (((g - b) / d) % 6);
        else if (max == g) h = 60 * ((b - r) / d + 2);
        else h = 60 * ((r - g) / d + 4);
        if (h < 0) h += 360;
        return (h, Math.Clamp(s, 0, 1), l);
    }

    public static uint FromHsl(float h, float s, float l, int a = 255)
    {
        h = ((h % 360) + 360) % 360;
        s = Math.Clamp(s, 0, 1);
        l = Math.Clamp(l, 0, 1);
        float c = (1 - MathF.Abs(2 * l - 1)) * s, x = c * (1 - MathF.Abs(h / 60 % 2 - 1)), m = l - c / 2;
        (float r, float g, float b) = ((int)(h / 60)) switch
        {
            0 => (c, x, 0f),
            1 => (x, c, 0f),
            2 => (0f, c, x),
            3 => (0f, x, c),
            4 => (x, 0f, c),
            _ => (c, 0f, x)
        };
        return Make(a, (int)MathF.Round((r + m) * 255), (int)MathF.Round((g + m) * 255), (int)MathF.Round((b + m) * 255));
    }
}
