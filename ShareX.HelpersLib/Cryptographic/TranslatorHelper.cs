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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ShareX.HelpersLib
{
    public static class TranslatorHelper
    {
        #region Text to ...

        public static string[] TextToBinary(string text)
        {
            string[] result = new string[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                result[i] = ByteToBinary((byte)text[i]);
            }

            return result;
        }

        public static string[] TextToHexadecimal(string text)
        {
            return BytesToHexadecimal(Encoding.UTF8.GetBytes(text));
        }

        public static byte[] TextToASCII(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        public static string TextToBase64(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string TextToHash(string text, HashType hashType, bool uppercase = false)
        {
            using (HashAlgorithm hash = HashChecker.GetHashAlgorithm(hashType))
            {
                byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(text));
                string[] hex = BytesToHexadecimal(bytes);
                string result = string.Concat(hex);
                if (uppercase) result = result.ToUpperInvariant();
                return result;
            }
        }

        #endregion Text to ...

        #region Binary to ...

        public static byte BinaryToByte(string binary)
        {
            return Convert.ToByte(binary, 2);
        }

        public static string BinaryToText(string binary)
        {
            binary = Regex.Replace(binary, @"[^01]", "");

            using (MemoryStream stream = new MemoryStream())
            {
                for (int i = 0; i + 8 <= binary.Length; i += 8)
                {
                    stream.WriteByte(BinaryToByte(binary.Substring(i, 8)));
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        #endregion Binary to ...

        #region Byte to ...

        public static string ByteToBinary(byte b)
        {
            char[] result = new char[8];
            int pos = 7;

            for (int i = 0; i < 8; i++)
            {
                if ((b & (1 << i)) != 0)
                {
                    result[pos] = '1';
                }
                else
                {
                    result[pos] = '0';
                }

                pos--;
            }

            return new string(result);
        }

        public static string[] BytesToHexadecimal(byte[] bytes)
        {
            string[] result = new string[bytes.Length];

            for (int i = 0; i < bytes.Length; i++)
            {
                result[i] = bytes[i].ToString("x2");
            }

            return result;
        }

        #endregion Byte to ...

        #region Hexadecimal to ...

        public static byte HexadecimalToByte(string hex)
        {
            return Convert.ToByte(hex, 16);
        }

        public static string HexadecimalToText(string hex)
        {
            hex = Regex.Replace(hex, @"[^0-9a-fA-F]", "");

            using (MemoryStream stream = new MemoryStream())
            {
                for (int i = 0; i + 2 <= hex.Length; i += 2)
                {
                    stream.WriteByte(HexadecimalToByte(hex.Substring(i, 2)));
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        #endregion Hexadecimal to ...

        #region Base64 to ...

        public static string Base64ToText(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }

        #endregion Base64 to ...

        #region ASCII to ...

        public static string ASCIIToText(string ascii)
        {
            string[] numbers = Regex.Split(ascii, @"\D+");

            using (MemoryStream stream = new MemoryStream())
            {
                foreach (string number in numbers)
                {
                    if (byte.TryParse(number, out byte b))
                    {
                        stream.WriteByte(b);
                    }
                }

                return Encoding.ASCII.GetString(stream.ToArray());
            }
        }

        #endregion ASCII to ...
    }
}