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

namespace ShareX.HelpersLib
{
    public static class NumberExtensions
    {
        private static readonly string[] suffixDecimal = new[] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        private static readonly string[] suffixBinary = new[] { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

        public static T Min<T>(this T num, T min) where T : IComparable<T>
        {
            return MathHelpers.Min(num, min);
        }

        public static T Max<T>(this T num, T max) where T : IComparable<T>
        {
            return MathHelpers.Max(num, max);
        }

        public static T Clamp<T>(this T num, T min, T max) where T : IComparable<T>
        {
            return MathHelpers.Clamp(num, min, max);
        }

        public static bool IsBetween<T>(this T num, T min, T max) where T : IComparable<T>
        {
            return MathHelpers.IsBetween(num, min, max);
        }

        public static T BetweenOrDefault<T>(this T num, T min, T max, T defaultValue = default) where T : IComparable<T>
        {
            return MathHelpers.BetweenOrDefault(num, min, max, defaultValue);
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return MathHelpers.Remap(value, from1, to1, from2, to2);
        }

        public static bool IsEvenNumber(this int num)
        {
            return MathHelpers.IsEvenNumber(num);
        }

        public static bool IsOddNumber(this int num)
        {
            return MathHelpers.IsOddNumber(num);
        }

        public static string ToSizeString(this long size, bool binary = false, int decimalPlaces = 2)
        {
            int bytes = binary ? 1024 : 1000;
            if (size < bytes) return Math.Max(size, 0) + " B";
            int place = (int)Math.Floor(Math.Log(size, bytes));
            double num = size / Math.Pow(bytes, place);
            string suffix = binary ? suffixBinary[place] : suffixDecimal[place];
            return num.ToDecimalString(decimalPlaces.Clamp(0, 3)) + " " + suffix;
        }

        public static string ToDecimalString(this double number, int decimalPlaces)
        {
            string format = "0";
            if (decimalPlaces > 0) format += "." + new string('0', decimalPlaces);
            return number.ToString(format);
        }

        public static string ToBase(this int value, int radix, string digits)
        {
            if (string.IsNullOrEmpty(digits))
            {
                throw new ArgumentNullException("digits", string.Format("Digits must contain character value representations"));
            }

            radix = Math.Abs(radix);
            if (radix > digits.Length || radix < 2)
            {
                throw new ArgumentOutOfRangeException("radix", radix, string.Format("Radix has to be > 2 and < {0}", digits.Length));
            }

            string result = "";
            int quotient = Math.Abs(value);
            while (quotient > 0)
            {
                int temp = quotient % radix;
                result = digits[temp] + result;
                quotient /= radix;
            }
            return result;
        }
    }
}