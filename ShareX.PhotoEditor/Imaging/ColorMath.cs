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

using System.Runtime.CompilerServices;

namespace ShareX.PhotoEditor.Imaging;

/// <summary>Pixel packing and color space helpers. Pixels are premultiplied BGRA stored as uint (0xAARRGGBB).</summary>
public static class ColorMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Unpack(uint p, out float r, out float g, out float b, out float a)
    {
        a = (p >> 24) / 255f;
        if (a <= 0) { r = g = b = 0; return; }
        r = MathF.Min(1, ((p >> 16) & 0xFF) / 255f / a);
        g = MathF.Min(1, ((p >> 8) & 0xFF) / 255f / a);
        b = MathF.Min(1, (p & 0xFF) / 255f / a);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint PackUnpremul(float r, float g, float b, float a)
    {
        a = Math.Clamp(a, 0, 1);
        return BlendModes.Pack(Math.Clamp(r, 0, 1) * a, Math.Clamp(g, 0, 1) * a, Math.Clamp(b, 0, 1) * a, a);
    }

    public static uint FromColor(SKColor c) => PackUnpremul(c.Red / 255f, c.Green / 255f, c.Blue / 255f, c.Alpha / 255f);

    public static SKColor ToColor(uint p)
    {
        Unpack(p, out float r, out float g, out float b, out float a);
        return new SKColor((byte)MathF.Round(r * 255), (byte)MathF.Round(g * 255), (byte)MathF.Round(b * 255), (byte)MathF.Round(a * 255));
    }

    public static float Luminance(float r, float g, float b) => 0.299f * r + 0.587f * g + 0.114f * b;

    public static float Luminance(SKColor c) => Luminance(c.Red / 255f, c.Green / 255f, c.Blue / 255f);

    /// <summary>RGB (0..1) to HSL with hue in degrees.</summary>
    public static void RgbToHsl(float r, float g, float b, out float h, out float s, out float l)
    {
        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));
        l = (max + min) / 2;
        float d = max - min;
        if (d < 1e-6f) { h = 0; s = 0; return; }
        s = l > 0.5f ? d / (2 - max - min) : d / (max + min);
        if (max == r) h = (g - b) / d + (g < b ? 6 : 0);
        else if (max == g) h = (b - r) / d + 2;
        else h = (r - g) / d + 4;
        h *= 60;
    }

    public static void HslToRgb(float h, float s, float l, out float r, out float g, out float b)
    {
        if (s <= 0) { r = g = b = l; return; }
        h = ((h % 360) + 360) % 360 / 360f;
        float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
        float p = 2 * l - q;
        r = HueToRgb(p, q, h + 1f / 3);
        g = HueToRgb(p, q, h);
        b = HueToRgb(p, q, h - 1f / 3);
    }

    private static float HueToRgb(float p, float q, float t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1f / 6) return p + (q - p) * 6 * t;
        if (t < 0.5f) return q;
        if (t < 2f / 3) return p + (q - p) * (2f / 3 - t) * 6;
        return p;
    }

    public static void RgbToHsv(float r, float g, float b, out float h, out float s, out float v)
    {
        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));
        v = max;
        float d = max - min;
        s = max <= 0 ? 0 : d / max;
        if (d < 1e-6f) { h = 0; return; }
        if (max == r) h = (g - b) / d + (g < b ? 6 : 0);
        else if (max == g) h = (b - r) / d + 2;
        else h = (r - g) / d + 4;
        h *= 60;
    }

    public static void HsvToRgb(float h, float s, float v, out float r, out float g, out float b)
    {
        h = ((h % 360) + 360) % 360 / 60f;
        int i = (int)MathF.Floor(h);
        float f = h - i;
        float p = v * (1 - s), q = v * (1 - s * f), t = v * (1 - s * (1 - f));
        switch (i % 6)
        {
            case 0: r = v; g = t; b = p; break;
            case 1: r = q; g = v; b = p; break;
            case 2: r = p; g = v; b = t; break;
            case 3: r = p; g = q; b = v; break;
            case 4: r = t; g = p; b = v; break;
            default: r = v; g = p; b = q; break;
        }
    }

    public static SKColor FromHsv(float h, float s, float v, byte alpha = 255)
    {
        HsvToRgb(h, s, v, out float r, out float g, out float b);
        return new SKColor((byte)MathF.Round(r * 255), (byte)MathF.Round(g * 255), (byte)MathF.Round(b * 255), alpha);
    }

    public static SKColor FromHsl(float h, float s, float l, byte alpha = 255)
    {
        HslToRgb(h, s, l, out float r, out float g, out float b);
        return new SKColor((byte)MathF.Round(r * 255), (byte)MathF.Round(g * 255), (byte)MathF.Round(b * 255), alpha);
    }

    public static string ToHex(SKColor c, bool includeAlpha = false) =>
        includeAlpha ? $"{c.Red:X2}{c.Green:X2}{c.Blue:X2}{c.Alpha:X2}" : $"{c.Red:X2}{c.Green:X2}{c.Blue:X2}";

    public static bool TryParseHex(string? text, out SKColor color)
    {
        color = SKColors.Black;
        if (string.IsNullOrWhiteSpace(text)) return false;
        string hex = text.Trim().TrimStart('#');
        if (hex.Length == 3) hex = string.Concat(hex.Select(c => new string(c, 2)));
        if (hex.Length != 6 && hex.Length != 8) return false;
        if (!uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out uint value)) return false;
        color = hex.Length == 6
            ? new SKColor((byte)(value >> 16), (byte)(value >> 8), (byte)value)
            : new SKColor((byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value);
        return true;
    }

    /// <summary>Distance between two colors on a 0..255 scale, using the max channel difference.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Distance(uint a, uint b)
    {
        int d = Math.Abs((int)((a >> 16) & 0xFF) - (int)((b >> 16) & 0xFF));
        d = Math.Max(d, Math.Abs((int)((a >> 8) & 0xFF) - (int)((b >> 8) & 0xFF)));
        d = Math.Max(d, Math.Abs((int)(a & 0xFF) - (int)(b & 0xFF)));
        d = Math.Max(d, Math.Abs((int)(a >> 24) - (int)(b >> 24)));
        return d;
    }

    public static SKColor Lerp(SKColor a, SKColor b, float t) => new(
        (byte)MathF.Round(a.Red + (b.Red - a.Red) * t),
        (byte)MathF.Round(a.Green + (b.Green - a.Green) * t),
        (byte)MathF.Round(a.Blue + (b.Blue - a.Blue) * t),
        (byte)MathF.Round(a.Alpha + (b.Alpha - a.Alpha) * t));

    /// <summary>Linear interpolation between premultiplied pixels.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint LerpPremul(uint a, uint b, float t)
    {
        if (t <= 0) return a;
        if (t >= 1) return b;
        int it = (int)(t * 256);
        int ia = 256 - it;
        uint A = (uint)(((a >> 24) * ia + (b >> 24) * it) >> 8);
        uint R = (uint)((((a >> 16) & 0xFF) * ia + ((b >> 16) & 0xFF) * it) >> 8);
        uint G = (uint)((((a >> 8) & 0xFF) * ia + ((b >> 8) & 0xFF) * it) >> 8);
        uint B = (uint)(((a & 0xFF) * ia + (b & 0xFF) * it) >> 8);
        return (A << 24) | (R << 16) | (G << 8) | B;
    }
}
