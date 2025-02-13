#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShareX.HelpersLib
{
    public static class ColorHelpers
    {
        public static Color[] StandardColors = new Color[]
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(64, 64, 64),
            Color.FromArgb(255, 0, 0),
            Color.FromArgb(255, 106, 0),
            Color.FromArgb(255, 216, 0),
            Color.FromArgb(182, 255, 0),
            Color.FromArgb(76, 255, 0),
            Color.FromArgb(0, 255, 33),
            Color.FromArgb(0, 255, 144),
            Color.FromArgb(0, 255, 255),
            Color.FromArgb(0, 148, 255),
            Color.FromArgb(0, 38, 255),
            Color.FromArgb(72, 0, 255),
            Color.FromArgb(178, 0, 255),
            Color.FromArgb(255, 0, 220),
            Color.FromArgb(255, 0, 110),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(128, 128, 128),
            Color.FromArgb(127, 0, 0),
            Color.FromArgb(127, 51, 0),
            Color.FromArgb(127, 106, 0),
            Color.FromArgb(91, 127, 0),
            Color.FromArgb(38, 127, 0),
            Color.FromArgb(0, 127, 14),
            Color.FromArgb(0, 127, 70),
            Color.FromArgb(0, 127, 127),
            Color.FromArgb(0, 74, 127),
            Color.FromArgb(0, 19, 127),
            Color.FromArgb(33, 0, 127),
            Color.FromArgb(87, 0, 127),
            Color.FromArgb(127, 0, 110),
            Color.FromArgb(127, 0, 55)
        };

        #region Convert Color to ...

        public static string ColorToHex(Color color, ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return string.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
                case ColorFormat.RGBA:
                    return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.R, color.G, color.B, color.A);
                case ColorFormat.ARGB:
                    return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
            }
        }

        public static int ColorToDecimal(Color color, ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return color.R << 16 | color.G << 8 | color.B;
                case ColorFormat.RGBA:
                    return color.R << 24 | color.G << 16 | color.B << 8 | color.A;
                case ColorFormat.ARGB:
                    return color.A << 24 | color.R << 16 | color.G << 8 | color.B;
            }
        }

        public static HSB ColorToHSB(Color color)
        {
            HSB hsb = new HSB();

            int Max, Min;

            if (color.R > color.G)
            {
                Max = color.R;
                Min = color.G;
            }
            else
            {
                Max = color.G;
                Min = color.R;
            }

            if (color.B > Max) Max = color.B;
            else if (color.B < Min) Min = color.B;

            int Diff = Max - Min;

            hsb.Brightness = (double)Max / 255;

            if (Max == 0) hsb.Saturation = 0;
            else hsb.Saturation = (double)Diff / Max;

            double q;
            if (Diff == 0) q = 0;
            else q = (double)60 / Diff;

            if (Max == color.R)
            {
                if (color.G < color.B) hsb.Hue = (360 + (q * (color.G - color.B))) / 360;
                else hsb.Hue = q * (color.G - color.B) / 360;
            }
            else if (Max == color.G) hsb.Hue = (120 + (q * (color.B - color.R))) / 360;
            else if (Max == color.B) hsb.Hue = (240 + (q * (color.R - color.G))) / 360;
            else hsb.Hue = 0.0;

            hsb.Alpha = color.A;

            return hsb;
        }

        public static CMYK ColorToCMYK(Color color)
        {
            if (color.R == 0 && color.G == 0 && color.B == 0)
            {
                return new CMYK(0, 0, 0, 1, color.A);
            }

            double c = 1 - (color.R / 255d);
            double m = 1 - (color.G / 255d);
            double y = 1 - (color.B / 255d);
            double k = Math.Min(c, Math.Min(m, y));

            c = (c - k) / (1 - k);
            m = (m - k) / (1 - k);
            y = (y - k) / (1 - k);

            return new CMYK(c, m, y, k, color.A);
        }

        #endregion Convert Color to ...

        #region Convert Hex to ...

        public static Color HexToColor(string hex, ColorFormat format = ColorFormat.RGB)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return Color.Empty;
            }

            if (hex[0] == '#')
            {
                hex = hex.Remove(0, 1);
            }
            else if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hex = hex.Remove(0, 2);
            }

            if (((format == ColorFormat.RGBA || format == ColorFormat.ARGB) && hex.Length != 8) ||
                (format == ColorFormat.RGB && hex.Length != 6))
            {
                return Color.Empty;
            }

            int r, g, b, a;

            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    r = HexToDecimal(hex.Substring(0, 2));
                    g = HexToDecimal(hex.Substring(2, 2));
                    b = HexToDecimal(hex.Substring(4, 2));
                    a = 255;
                    break;
                case ColorFormat.RGBA:
                    r = HexToDecimal(hex.Substring(0, 2));
                    g = HexToDecimal(hex.Substring(2, 2));
                    b = HexToDecimal(hex.Substring(4, 2));
                    a = HexToDecimal(hex.Substring(6, 2));
                    break;
                case ColorFormat.ARGB:
                    a = HexToDecimal(hex.Substring(0, 2));
                    r = HexToDecimal(hex.Substring(2, 2));
                    g = HexToDecimal(hex.Substring(4, 2));
                    b = HexToDecimal(hex.Substring(6, 2));
                    break;
            }

            return Color.FromArgb(a, r, g, b);
        }

        public static int HexToDecimal(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        #endregion Convert Hex to ...

        #region Convert Decimal to ...

        public static Color DecimalToColor(int dec, ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return Color.FromArgb((dec >> 16) & 0xFF, (dec >> 8) & 0xFF, dec & 0xFF);
                case ColorFormat.RGBA:
                    return Color.FromArgb(dec & 0xFF, (dec >> 24) & 0xFF, (dec >> 16) & 0xFF, (dec >> 8) & 0xFF);
                case ColorFormat.ARGB:
                    return Color.FromArgb((dec >> 24) & 0xFF, (dec >> 16) & 0xFF, (dec >> 8) & 0xFF, dec & 0xFF);
            }
        }

        public static string DecimalToHex(int dec)
        {
            return dec.ToString("X6");
        }

        #endregion Convert Decimal to ...

        #region Convert HSB to ...

        public static Color HSBToColor(HSB hsb)
        {
            int Mid;
            int Max = (int)Math.Round(hsb.Brightness * 255);
            int Min = (int)Math.Round((1.0 - hsb.Saturation) * (hsb.Brightness / 1.0) * 255);
            double q = (double)(Max - Min) / 255;

            if (hsb.Hue >= 0 && hsb.Hue <= (double)1 / 6)
            {
                Mid = (int)Math.Round((((hsb.Hue - 0) * q) * 1530) + Min);
                return Color.FromArgb(hsb.Alpha, Max, Mid, Min);
            }

            if (hsb.Hue <= (double)1 / 3)
            {
                Mid = (int)Math.Round((-((hsb.Hue - ((double)1 / 6)) * q) * 1530) + Max);
                return Color.FromArgb(hsb.Alpha, Mid, Max, Min);
            }

            if (hsb.Hue <= 0.5)
            {
                Mid = (int)Math.Round((((hsb.Hue - ((double)1 / 3)) * q) * 1530) + Min);
                return Color.FromArgb(hsb.Alpha, Min, Max, Mid);
            }

            if (hsb.Hue <= (double)2 / 3)
            {
                Mid = (int)Math.Round((-((hsb.Hue - 0.5) * q) * 1530) + Max);
                return Color.FromArgb(hsb.Alpha, Min, Mid, Max);
            }

            if (hsb.Hue <= (double)5 / 6)
            {
                Mid = (int)Math.Round((((hsb.Hue - ((double)2 / 3)) * q) * 1530) + Min);
                return Color.FromArgb(hsb.Alpha, Mid, Min, Max);
            }

            if (hsb.Hue <= 1.0)
            {
                Mid = (int)Math.Round((-((hsb.Hue - ((double)5 / 6)) * q) * 1530) + Max);
                return Color.FromArgb(hsb.Alpha, Max, Min, Mid);
            }

            return Color.FromArgb(hsb.Alpha, 0, 0, 0);
        }

        #endregion Convert HSB to ...

        #region Convert CMYK to ...

        public static Color CMYKToColor(CMYK cmyk)
        {
            if (cmyk.Cyan == 0 && cmyk.Magenta == 0 && cmyk.Yellow == 0 && cmyk.Key == 1)
            {
                return Color.FromArgb(cmyk.Alpha, 0, 0, 0);
            }

            double c = (cmyk.Cyan * (1 - cmyk.Key)) + cmyk.Key;
            double m = (cmyk.Magenta * (1 - cmyk.Key)) + cmyk.Key;
            double y = (cmyk.Yellow * (1 - cmyk.Key)) + cmyk.Key;

            int r = (int)Math.Round((1 - c) * 255);
            int g = (int)Math.Round((1 - m) * 255);
            int b = (int)Math.Round((1 - y) * 255);

            return Color.FromArgb(cmyk.Alpha, r, g, b);
        }

        #endregion Convert CMYK to ...

        public static double ValidColor(double number)
        {
            return number.Clamp(0, 1);
        }

        public static int ValidColor(int number)
        {
            return number.Clamp(0, 255);
        }

        public static byte ValidColor(byte number)
        {
            return number.Clamp<byte>(0, 255);
        }

        public static Color RandomColor()
        {
            return Color.FromArgb(RandomFast.Next(255), RandomFast.Next(255), RandomFast.Next(255));
        }

        public static bool ParseColor(string text, out Color color)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Trim();

                if (text.Length <= 20)
                {
                    Match matchHex = Regex.Match(text, @"^(?:#|0x)?((?:[0-9A-F]{2}){3})$", RegexOptions.IgnoreCase);

                    if (matchHex.Success)
                    {
                        color = HexToColor(matchHex.Groups[1].Value);
                        return true;
                    }
                    else
                    {
                        Match matchRGB = Regex.Match(text, @"^(?:rgb\()?([1]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])(?:\s|,)+([1]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])(?:\s|,)+([1]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\)?$");

                        if (matchRGB.Success)
                        {
                            color = Color.FromArgb(int.Parse(matchRGB.Groups[1].Value), int.Parse(matchRGB.Groups[2].Value), int.Parse(matchRGB.Groups[3].Value));
                            return true;
                        }
                    }
                }
            }

            color = Color.Empty;
            return false;
        }

        public static int PerceivedBrightness(Color color)
        {
            return (int)Math.Sqrt((color.R * color.R * .299) + (color.G * color.G * .587) + (color.B * color.B * .114));
        }

        public static Color VisibleColor(Color color)
        {
            return VisibleColor(color, Color.White, Color.Black);
        }

        public static Color VisibleColor(Color color, Color lightColor, Color darkColor)
        {
            if (IsLightColor(color))
            {
                return darkColor;
            }

            return lightColor;
        }

        public static bool IsLightColor(Color color)
        {
            return PerceivedBrightness(color) > 130;
        }

        public static bool IsDarkColor(Color color)
        {
            return !IsLightColor(color);
        }

        public static Color Lerp(Color from, Color to, float amount)
        {
            return Color.FromArgb((int)MathHelpers.Lerp(from.R, to.R, amount), (int)MathHelpers.Lerp(from.G, to.G, amount), (int)MathHelpers.Lerp(from.B, to.B, amount));
        }

        public static Color DeterministicStringToColor(string text)
        {
            int hash = text.GetHashCode();
            int r = (hash & 0xFF0000) >> 16;
            int g = (hash & 0x00FF00) >> 8;
            int b = hash & 0x0000FF;
            return Color.FromArgb(r, g, b);
        }

        public static int ColorDifference(Color color1, Color color2)
        {
            int rDiff = Math.Abs(color1.R - color2.R);
            int gDiff = Math.Abs(color1.G - color2.G);
            int bDiff = Math.Abs(color1.B - color2.B);
            return rDiff + gDiff + bDiff;
        }

        public static bool ColorsAreClose(Color color1, Color color2, int threshold)
        {
            return ColorDifference(color1, color2) <= threshold;
        }

        public static Color LighterColor(Color color, float amount)
        {
            return Lerp(color, Color.White, amount);
        }

        public static Color DarkerColor(Color color, float amount)
        {
            return Lerp(color, Color.Black, amount);
        }

        public static List<Color> GetKnownColors()
        {
            List<Color> colors = new List<Color>();

            for (KnownColor knownColor = KnownColor.AliceBlue; knownColor <= KnownColor.YellowGreen; knownColor++)
            {
                Color color = Color.FromKnownColor(knownColor);
                colors.Add(color);
            }

            return colors;
        }

        public static Color FindClosestKnownColor(Color color)
        {
            List<Color> colors = GetKnownColors();
            return colors.Aggregate(Color.Black, (accu, curr) => ColorDifference(color, curr) < ColorDifference(color, accu) ? curr : accu);
        }

        public static string GetColorName(Color color)
        {
            Color knownColor = FindClosestKnownColor(color);
            return Helpers.GetProperName(knownColor.Name);
        }
    }
}