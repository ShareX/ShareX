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

using System;
using System.Linq;

namespace ShareX.UploadersLib
{
    public static class Base58Converter
    {
        public static string Encode(byte[] data)
        {
            var intData = data.Aggregate<byte, System.Numerics.BigInteger>(0, (current, t) => (current * 256) + t);
            var b58 = new System.Numerics.BigInteger(58);

            var result = string.Empty;

            while (intData > 0)
            {
                var remainder = (int)(intData % b58);
                intData /= 58;
                result = HelpersLib.Helpers.Base58[remainder] + result;
            }

            for (var i = 0; i < data.Length && data[i] == 0; i++)
                result = '1' + result;

            return result;
        }

        public static byte[] Decode(string data)
        {
            System.Numerics.BigInteger intData = 0;
            for (var i = 0; i < data.Length; i++)
            {
                var digit = HelpersLib.Helpers.Base58.IndexOf(data[i]);

                if (digit < 0)
                    throw new FormatException(string.Format(Localization.Strings.Base58Converter_Invalid_character,
                        data[i], i));

                intData = intData * 58 + digit;
            }

            var leadingZeroCount = data.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros = intData.ToByteArray().Reverse().SkipWhile(b => b == 0);
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();

            return result;
        }
    }
}
