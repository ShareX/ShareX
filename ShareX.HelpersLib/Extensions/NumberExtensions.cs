#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
        public static int Min(this int num, int min)
        {
            if (num < min) return min;
            return num;
        }

        public static int Max(this int num, int max)
        {
            if (num > max) return max;
            return num;
        }

        public static int Between(this int num, int min, int max)
        {
            if (num <= min) return min;
            if (num >= max) return max;
            return num;
        }

        public static float Between(this float num, float min, float max)
        {
            if (num <= min) return min;
            if (num >= max) return max;
            return num;
        }

        public static double Between(this double num, double min, double max)
        {
            if (num <= min) return min;
            if (num >= max) return max;
            return num;
        }

        public static byte Between(this byte num, byte min, byte max)
        {
            if (num <= min) return min;
            if (num >= max) return max;
            return num;
        }

        public static bool IsBetween(this int num, int min, int max)
        {
            return num >= min && num <= max;
        }

        public static bool IsBetween(this byte num, int min, int max)
        {
            return num >= min && num <= max;
        }

        public static int BetweenOrDefault(this int num, int min, int max, int defaultValue = 0)
        {
            if (num.IsBetween(min, max)) return num;
            return defaultValue;
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static int RandomAdd(this int num, int min, int max)
        {
            return num + MathHelpers.Random(min, max);
        }

        private static readonly string[] Suffix_Decimal = new[] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        private static readonly string[] Suffix_Binary = new[] { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

        public static string ToSizeString(this long size, bool binary = false, int decimalPlaces = 2)
        {
            if (size < 1024) return Math.Max(size, 0) + " B";
            int place = (int)Math.Floor(Math.Log(size, 1024));
            double num = size / Math.Pow(1024, place);
            string suffix = binary ? Suffix_Binary[place] : Suffix_Decimal[place];
            return string.Format("{0} {1}", num.ToDecimalString(decimalPlaces.Between(0, 3)), suffix);
        }

        public static string ToDecimalString(this double number, int decimalPlaces)
        {
            string format = "0";
            if (decimalPlaces > 0) format += "." + new string('0', decimalPlaces);
            return number.ToString(format);
        }
    }
}