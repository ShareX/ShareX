#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
                return Binary.Join();
            }
        }

        // http://en.wikipedia.org/wiki/Hexadecimal
        public string[] Hexadecimal { get; private set; }

        public string HexadecimalText
        {
            get
            {
                return Hexadecimal.Join().ToUpperInvariant();
            }
        }

        // http://en.wikipedia.org/wiki/ASCII
        public byte[] ASCII { get; private set; }

        public string ASCIIText
        {
            get
            {
                return ASCII.Join();
            }
        }

        // http://en.wikipedia.org/wiki/Base64
        public string Base64 { get; private set; }

        // http://en.wikipedia.org/wiki/MD5
        public string MD5 { get; private set; }

        // http://en.wikipedia.org/wiki/SHA-1
        public string SHA1 { get; private set; }

        // http://en.wikipedia.org/wiki/SHA-2
        public string SHA256 { get; private set; }
        public string SHA384 { get; private set; }
        public string SHA512 { get; private set; }

        // http://en.wikipedia.org/wiki/RIPEMD
        public string RIPEMD160 { get; private set; }

        public bool EncodeText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Text = text;
                Binary = TranslatorHelper.TextToBinary(text);
                Hexadecimal = TranslatorHelper.TextToHexadecimal(text);
                ASCII = TranslatorHelper.TextToASCII(text);
                Base64 = TranslatorHelper.TextToBase64(text);
                MD5 = TranslatorHelper.TextToHash(text, HashType.MD5, true);
                SHA1 = TranslatorHelper.TextToHash(text, HashType.SHA1, true);
                SHA256 = TranslatorHelper.TextToHash(text, HashType.SHA256, true);
                SHA384 = TranslatorHelper.TextToHash(text, HashType.SHA384, true);
                SHA512 = TranslatorHelper.TextToHash(text, HashType.SHA512, true);
                RIPEMD160 = TranslatorHelper.TextToHash(text, HashType.RIPEMD160, true);
                return true;
            }

            return false;
        }

        public bool DecodeBinary(string binary)
        {
            string result = TranslatorHelper.BinaryToText(binary);
            return EncodeText(result);
        }

        public bool DecodeHex(string hex)
        {
            string result = TranslatorHelper.HexadecimalToText(hex);
            return EncodeText(result);
        }

        public bool DecodeASCII(string ascii)
        {
            string result = TranslatorHelper.ASCIIToText(ascii);
            return EncodeText(result);
        }

        public bool DecodeBase64(string base64)
        {
            string result = TranslatorHelper.Base64ToText(base64);
            return EncodeText(result);
        }

        public static bool Test()
        {
            bool result = true;
            Translator translator = new Translator();
            string text = Helpers.GetAllCharacters();
            translator.EncodeText(text);
            translator.DecodeBinary(translator.BinaryText);
            result &= translator.Text == text;
            translator.DecodeHex(translator.HexadecimalText);
            result &= translator.Text == text;
            //translator.DecodeASCII(translator.ASCIIText);
            //result &= translator.Text == text;
            translator.DecodeBase64(translator.Base64);
            result &= translator.Text == text;
            return result;
        }

        public string HashToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("MD5: ");
            sb.AppendLine(MD5);
            sb.Append("SHA1: ");
            sb.AppendLine(SHA1);
            sb.Append("SHA256: ");
            sb.AppendLine(SHA256);
            sb.Append("SHA384: ");
            sb.AppendLine(SHA384);
            sb.Append("SHA512: ");
            sb.AppendLine(SHA512);
            sb.Append("RIPEMD160: ");
            sb.Append(RIPEMD160);
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Text: ");
            sb.AppendLine(Text);
            sb.Append("Binary: ");
            sb.AppendLine(BinaryText);
            sb.Append("Hex: ");
            sb.AppendLine(HexadecimalText);
            sb.Append("ASCII: ");
            sb.AppendLine(ASCIIText);
            sb.Append("Base64: ");
            sb.AppendLine(Base64);
            sb.Append(HashToString());
            return sb.ToString();
        }
    }
}