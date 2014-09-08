#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

namespace HelpersLib
{
    public static class ColorHelpers
    {
        public static double ValidColor(double number)
        {
            return number.Between(0, 1);
        }

        public static int ValidColor(int number)
        {
            return number.Between(0, 255);
        }

        public static byte ValidColor(byte number)
        {
            return number.Between(0, 255);
        }

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
                if (color.G < color.B) hsb.Hue = (360 + q * (color.G - color.B)) / 360;
                else hsb.Hue = q * (color.G - color.B) / 360;
            }
            else if (Max == color.G) hsb.Hue = (120 + q * (color.B - color.R)) / 360;
            else if (Max == color.B) hsb.Hue = (240 + q * (color.R - color.G)) / 360;
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
                Mid = (int)Math.Round(((hsb.Hue - 0) * q) * 1530 + Min);
                return Color.FromArgb(hsb.Alpha, Max, Mid, Min);
            }

            if (hsb.Hue <= (double)1 / 3)
            {
                Mid = (int)Math.Round(-((hsb.Hue - (double)1 / 6) * q) * 1530 + Max);
                return Color.FromArgb(hsb.Alpha, Mid, Max, Min);
            }

            if (hsb.Hue <= 0.5)
            {
                Mid = (int)Math.Round(((hsb.Hue - (double)1 / 3) * q) * 1530 + Min);
                return Color.FromArgb(hsb.Alpha, Min, Max, Mid);
            }

            if (hsb.Hue <= (double)2 / 3)
            {
                Mid = (int)Math.Round(-((hsb.Hue - 0.5) * q) * 1530 + Max);
                return Color.FromArgb(hsb.Alpha, Min, Mid, Max);
            }

            if (hsb.Hue <= (double)5 / 6)
            {
                Mid = (int)Math.Round(((hsb.Hue - (double)2 / 3) * q) * 1530 + Min);
                return Color.FromArgb(hsb.Alpha, Mid, Min, Max);
            }

            if (hsb.Hue <= 1.0)
            {
                Mid = (int)Math.Round(-((hsb.Hue - (double)5 / 6) * q) * 1530 + Max);
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

            double c = cmyk.Cyan * (1 - cmyk.Key) + cmyk.Key;
            double m = cmyk.Magenta * (1 - cmyk.Key) + cmyk.Key;
            double y = cmyk.Yellow * (1 - cmyk.Key) + cmyk.Key;

            int r = (int)Math.Round((1 - c) * 255);
            int g = (int)Math.Round((1 - m) * 255);
            int b = (int)Math.Round((1 - y) * 255);

            return Color.FromArgb(cmyk.Alpha, r, g, b);
        }

        #endregion Convert CMYK to ...

        public static Color RandomColor()
        {
            return Color.FromArgb(MathHelpers.Random(255), MathHelpers.Random(255), MathHelpers.Random(255));
        }

        public static Color ParseColor(string color)
        {
            if (color.StartsWith("#"))
            {
                return HexToColor(color);
            }

            if (color.Contains(','))
            {
                int[] colors = color.Split(',').Select(x => int.Parse(x.Trim())).ToArray();

                if (colors.Length == 3)
                {
                    return Color.FromArgb(colors[0], colors[1], colors[2]);
                }

                if (colors.Length == 4)
                {
                    return Color.FromArgb(colors[0], colors[1], colors[2], colors[3]);
                }
            }

            return Color.FromName(color);
        }

        public static Color Mix(List<Color> colors)
        {
            int a = 0;
            int r = 0;
            int g = 0;
            int b = 0;
            int count = 0;

            foreach (Color color in colors)
            {
                if (!color.Equals(Color.Empty))
                {
                    a += color.A;
                    r += color.R;
                    g += color.G;
                    b += color.B;
                    count++;
                }
            }

            if (count == 0)
            {
                return Color.Empty;
            }

            return Color.FromArgb(a / count, r / count, g / count, b / count);
        }

        public static int PerceivedBrightness(Color c)
        {
            return (int)Math.Sqrt(
                c.R * c.R * .299 +
                c.G * c.G * .587 +
                c.B * c.B * .114);
        }

        public static Color VisibleTextColor(Color c)
        {
            return PerceivedBrightness(c) > 130 ? Color.Black : Color.White;
        }
    }
}