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
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", data[i], i));

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