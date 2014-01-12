#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.Text;

namespace HelpersLib
{
    public static class StringExtensions
    {
        public static string Left(this string str, int length)
        {
            if (length < 1) return string.Empty;
            if (length < str.Length) return str.Substring(0, length);
            return str;
        }

        public static string Right(this string str, int length)
        {
            if (length < 1) return string.Empty;
            if (length < str.Length) return str.Substring(str.Length - length);
            return str;
        }

        public static string RemoveLeft(this string str, int length)
        {
            if (length < 1) return string.Empty;
            if (length < str.Length) return str.Remove(0, length);
            return str;
        }

        public static string RemoveRight(this string str, int length)
        {
            if (length < 1) return string.Empty;
            if (length < str.Length) return str.Remove(str.Length - length);
            return str;
        }

        public static string Between(this string text, string first, string last, bool isFirstMatchForEnd = false, bool includeFirstAndLast = false)
        {
            int start = text.IndexOf(first);
            if (start < 0) return null;
            if (!includeFirstAndLast) start += first.Length;
            text = text.Substring(start);

            int end = isFirstMatchForEnd ? text.IndexOf(last) : text.LastIndexOf(last);
            if (end < 0) return null;
            if (includeFirstAndLast) end += last.Length;
            return text.Remove(end);
        }

        public static string Repeat(this string str, int count)
        {
            if (!string.IsNullOrEmpty(str) && count > 0)
            {
                StringBuilder sb = new StringBuilder(str.Length * count);

                for (int i = 0; i < count; i++)
                {
                    sb.Append(str);
                }

                return sb.ToString();
            }

            return null;
        }

        public static string ReplaceWith(this string str, string search, string replace,
            int occurrence = 0, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (!string.IsNullOrEmpty(search))
            {
                int count = 0, location;

                while (occurrence == 0 || occurrence > count)
                {
                    location = str.IndexOf(search, comparison);
                    if (location < 0) break;
                    count++;
                    str = str.Remove(location, search.Length).Insert(location, replace);
                }
            }

            return str;
        }

        public static bool ReplaceFirst(this string text, string search, string replace, out string result)
        {
            int location = text.IndexOf(search);

            if (location < 0)
            {
                result = text;
                return false;
            }

            result = text.Remove(location, search.Length).Insert(location, replace);
            return true;
        }

        public static string ReplaceAll(this string text, string search, Func<string> replace)
        {
            while (true)
            {
                int location = text.IndexOf(search);

                if (location < 0) break;

                text = text.Remove(location, search.Length).Insert(location, replace());
            }

            return text;
        }

        public static string RemoveWhiteSpaces(this string str)
        {
            StringBuilder result = new StringBuilder();

            foreach (char c in str)
            {
                if (!Char.IsWhiteSpace(c)) result.Append(c);
            }

            return result.ToString();
        }

        public static string Reverse(this string str)
        {
            char[] chars = str.ToCharArray();
            Array.Reverse(chars);
            return new String(chars);
        }

        public static string Truncate(this string str, int length, string endings = "...")
        {
            if (length < 1) return string.Empty;

            if (length < str.Length)
            {
                str = str.Left(length - endings.Length);
                str += endings;
            }

            return str;
        }

        public static bool IsValidUrl(this string url)
        {
            return Uri.IsWellFormedUriString(url.Trim(), UriKind.Absolute);
        }

        public static byte[] HexToBytes(this string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        public static string ParseQuoteString(this string str)
        {
            str = str.Trim();

            int firstQuote = str.IndexOf('"');

            if (firstQuote >= 0)
            {
                str = str.Substring(firstQuote + 1);

                int secondQuote = str.IndexOf('"');

                if (secondQuote >= 0)
                {
                    str = str.Remove(secondQuote);
                }
            }

            return str;
        }

        public static bool IsNumber(this string text)
        {
            foreach (char c in text)
            {
                if (!char.IsNumber(c)) return false;
            }

            return true;
        }
    }
}