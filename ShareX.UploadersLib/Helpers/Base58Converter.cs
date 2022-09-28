using System;
using System.Linq;

namespace ShareX.UploadersLib
{
    public static class Base58Converter
    {
        /// <summary>
        /// Encodes data in plain Base58, without any checksum.
        /// </summary>
        /// <param name="data">The data to be encoded</param>
        /// <returns></returns>
        public static string Encode(byte[] data)
        {
            // Decode byte[] to BigInteger
            var intData = data.Aggregate<byte, System.Numerics.BigInteger>(0, (current, t) => (current * 256) + t);
            var b58 = new System.Numerics.BigInteger(58);

            // Encode BigInteger to Base58 string
            var result = string.Empty;
            while (intData > 0)
            {
                var remainder = (int)(intData % b58);
                intData /= 58;
                result = HelpersLib.Helpers.Base58[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (var i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }

            return result;
        }

        /// <summary>
        /// Decodes data in plain Base58, without any checksum.
        /// </summary>
        /// <param name="data">Data to be decoded</param>
        /// <returns>Returns decoded data if valid; throws FormatException if invalid</returns>
        public static byte[] Decode(string data)
        {
            // Decode Base58 string to BigInteger 
            System.Numerics.BigInteger intData = 0;
            for (var i = 0; i < data.Length; i++)
            {
                var digit = HelpersLib.Helpers.Base58.IndexOf(data[i]); //Slow

                if (digit < 0)
                {
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", data[i], i));
                }

                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            var leadingZeroCount = data.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
              intData.ToByteArray()
              .Reverse()// to big endian
              .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();

            return result;
        }
    }
}