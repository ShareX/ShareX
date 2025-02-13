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

using System.Text;

namespace ShareX.HelpersLib
{
    public class Translator
    {
        // http://en.wikipedia.org/wiki/UTF-8
        public string Text { get; private set; }

        // http://en.wikipedia.org/wiki/Binary_numeral_system
        public string[] Binary { get; private set; }

        public string BinaryText
        {
            get
            {
                if (Binary != null && Binary.Length > 0)
                {
                    return Binary.Join();
                }

                return null;
            }
        }

        // http://en.wikipedia.org/wiki/Hexadecimal
        public string[] Hexadecimal { get; private set; }

        public string HexadecimalText
        {
            get
            {
                if (Hexadecimal != null && Hexadecimal.Length > 0)
                {
                    return Hexadecimal.Join().ToUpperInvariant();
                }

                return null;
            }
        }

        // http://en.wikipedia.org/wiki/ASCII
        public byte[] ASCII { get; private set; }

        public string ASCIIText
        {
            get
            {
                if (ASCII != null && ASCII.Length > 0)
                {
                    return ASCII.Join();
                }

                return null;
            }
        }

        // http://en.wikipedia.org/wiki/Base64
        public string Base64 { get; private set; }

        // https://en.wikipedia.org/wiki/Cyclic_redundancy_check
        public string CRC32 { get; private set; }

        // http://en.wikipedia.org/wiki/MD5
        public string MD5 { get; private set; }

        // http://en.wikipedia.org/wiki/SHA-1
        public string SHA1 { get; private set; }

        // http://en.wikipedia.org/wiki/SHA-2
        public string SHA256 { get; private set; }
        public string SHA384 { get; private set; }
        public string SHA512 { get; private set; }

        public void Clear()
        {
            Text = Base64 = CRC32 = MD5 = SHA1 = SHA256 = SHA384 = SHA512 = null;
            Binary = null;
            Hexadecimal = null;
            ASCII = null;
        }

        public bool EncodeText(string text)
        {
            try
            {
                Clear();

                if (!string.IsNullOrEmpty(text))
                {
                    Text = text;
                    Binary = TranslatorHelper.TextToBinary(text);
                    Hexadecimal = TranslatorHelper.TextToHexadecimal(text);
                    ASCII = TranslatorHelper.TextToASCII(text);
                    Base64 = TranslatorHelper.TextToBase64(text);
                    CRC32 = TranslatorHelper.TextToHash(text, HashType.CRC32, true);
                    MD5 = TranslatorHelper.TextToHash(text, HashType.MD5, true);
                    SHA1 = TranslatorHelper.TextToHash(text, HashType.SHA1, true);
                    SHA256 = TranslatorHelper.TextToHash(text, HashType.SHA256, true);
                    SHA384 = TranslatorHelper.TextToHash(text, HashType.SHA384, true);
                    SHA512 = TranslatorHelper.TextToHash(text, HashType.SHA512, true);
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        public bool DecodeBinary(string binary)
        {
            try
            {
                Text = TranslatorHelper.BinaryToText(binary);
                return !string.IsNullOrEmpty(Text);
            }
            catch
            {
            }

            Text = null;
            return false;
        }

        public bool DecodeHex(string hex)
        {
            try
            {
                Text = TranslatorHelper.HexadecimalToText(hex);
                return !string.IsNullOrEmpty(Text);
            }
            catch
            {
            }

            Text = null;
            return false;
        }

        public bool DecodeASCII(string ascii)
        {
            try
            {
                Text = TranslatorHelper.ASCIIToText(ascii);
                return !string.IsNullOrEmpty(Text);
            }
            catch
            {
            }

            Text = null;
            return false;
        }

        public bool DecodeBase64(string base64)
        {
            try
            {
                Text = TranslatorHelper.Base64ToText(base64);
                return !string.IsNullOrEmpty(Text);
            }
            catch
            {
            }

            Text = null;
            return false;
        }

        public string HashToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CRC-32: {CRC32}");
            sb.AppendLine($"MD5: {MD5}");
            sb.AppendLine($"SHA-1: {SHA1}");
            sb.AppendLine($"SHA-256: {SHA256}");
            sb.AppendLine($"SHA-384: {SHA384}");
            sb.AppendLine($"SHA-512: {SHA512}");
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Text: {Text}");
            sb.AppendLine($"Binary: {BinaryText}");
            sb.AppendLine($"Hexadecimal: {HexadecimalText}");
            sb.AppendLine($"ASCII: {ASCIIText}");
            sb.AppendLine($"Base64: {Base64}");
            sb.Append(HashToString());
            return sb.ToString();
        }
    }
}