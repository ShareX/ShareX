using NUnit.Framework;
using ShareX.HelpersLib;

namespace ShareX.UnitTests.HelpersLib.Cryptographic
{
    [TestFixture]
    public class TranslatorHelperTests
    {
        [Test]
        [TestCase("ShareX",new[]{"01010011","01101000","01100001","01110010","01100101","01011000"})]
        public void TextToBinary_WhenCalled_ShouldReturnTextInBinary(string input,string[] output)
        {
            var result=TranslatorHelper.TextToBinary(input);
            Assert.That(result,Is.EquivalentTo(output));
        }

        [Test]
        [TestCase("ShareX",new []{"53","68","61","72","65","58"})]
        public void TextToHexaDecimal_WhenCalled_ShouldReturnTextInHexaDecimal(string input,string[] output)
        {
            var result = TranslatorHelper.TextToHexadecimal(input);
            Assert.That(result,Is.EquivalentTo(output));
        }

        [Test]
        [TestCase("ShareX",new byte[]{83,104,97,114,101,88})]
        public void TextToASCII_WhenCalled_ShouldReturnTextInASCII(string input, byte[] output)
        {
            var result = TranslatorHelper.TextToASCII(input);
            Assert.That(result,Is.EquivalentTo(output));
        }

        [Test]
        [TestCase("ShareX","U2hhcmVY")]
        public void TextToBase64_WhenCalled_ShouldReturnTextInBase64(string input, string output)
        {
            var result = TranslatorHelper.TextToBase64(input);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase("ShareX","e0ace74469217da1f250003857c8bad8")]
        public void TextToHash_WhenCalledWithTypeMD5AndLowerCaseLetter_ShouldReturnMD5HashOfTextInLowerCase(string input, string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.MD5);
            Assert.That(result,Is.EqualTo(output));
        }
        [Test]
        [TestCase("ShareX","E0ACE74469217DA1F250003857C8BAD8")]
        public void TextToHash_WhenCalledWithTypeMD5AndUpperCase_ShouldReturnMd5HashOfTextInUpperCase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.MD5,true);
            Assert.That(result,Is.EqualTo(output));
        }
        [Test]
        [TestCase("ShareX","3b4f8d0a")]
        public void TextToHash_WhenCalledWithTypeCrc32AndLowerCase_ShouldReturnCrc32HashOfTextInLowerCase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.CRC32);
            Assert.That(result,Is.EqualTo(output));
        }
        [Test]
        [TestCase("ShareX","3B4F8D0A")]
        public void TextToHash_WhenCalledWithTypeCrc32AndUpperCase_ShouldReturnCrc32HashOfTextInUpperCase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.CRC32,true);
            Assert.That(result,Is.EqualTo(output));
        }
        [Test]
        [TestCase("ShareX","4f5d90f368de0be11792481eab511b7505574e61")]
        public void TextToHash_WhenCalledWithTypeSHA1AndLowerCase_ShouldReturnSHA1HashOfTextInLowerCase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.SHA1);
            Assert.That(result,Is.EqualTo(output));
        }
        [Test]
        [TestCase("ShareX","4F5D90F368DE0BE11792481EAB511B7505574E61")]
        public void TextToHash_WhenCalledWithTypeSHA1AndUppercase_ShouldReturnSHA1HashOfTextInUppercase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.SHA1,true);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase("ShareX","7947880f6d8b6dec76c60782c85e6dedcae2b94ce5e8c6c6914dff2ffdb9e007")]
        public void TextToHash_WhenCalledWithTypeSHA256AndLowerCase_ShouldReturnSHA256HashOfTextInLowerCase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.SHA256);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase("ShareX","7947880F6D8B6DEC76C60782C85E6DEDCAE2B94CE5E8C6C6914DFF2FFDB9E007")]
        public void TextToHash_WhenCalledWithTypeSHA256AndUppercase_ShouldReturnSHA256HashOfTextInUppercase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.SHA256,true);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase("ShareX","8bb9d7eb2ae4521df36d89d61a8aaafd5c71f3aa796e4dc178b1368b193fda6c78d4eab42f14e9bc4495d26c1606d4c9")]
        public void TextToHash_WhenCalledWithTypeSHA384AndLowerCase_ShouldReturnSHA384HashOfTextInLowerCase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.SHA384);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase("ShareX","8BB9D7EB2AE4521DF36D89D61A8AAAFD5C71F3AA796E4DC178B1368B193FDA6C78D4EAB42F14E9BC4495D26C1606D4C9")]
        public void TextToHash_WhenCalledWithTypeSHA384AndUppercase_ShouldReturnSHA384HashOfTextInUppercase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.SHA384,true);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase("ShareX","8cb1e3e8cc4d09dc0c973693902d36f708b4e1b2b526ba917856d1c446739376beee9502aa5eb035bb7697190c3b04104f296e060e51ec6ef04dd25b345c3051")]
        public void TextToHash_WhenCalledWithTypeSHA512AndLowerCase_ShouldReturnSHA512HashOfTextInLowerCase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.SHA512);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase("ShareX","8CB1E3E8CC4D09DC0C973693902D36F708B4E1B2B526BA917856D1C446739376BEEE9502AA5EB035BB7697190C3B04104F296E060E51EC6EF04DD25B345C3051")]
        public void TextToHash_WhenCalledWithTypeSHA512AndUppercase_ShouldReturnSHA512HashOfTextInUppercase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.SHA512,true);
            Assert.That(result,Is.EqualTo(output));
        }
        [Test]
        [TestCase("ShareX","275f2016a87554554bab165a0b085ebe15100b63")]
        public void TextToHash_WhenCalledWithTypeRIPEMD160AndLowerCase_ShouldReturnRIPEMD160HashOfTextInLowerCase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.RIPEMD160);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase("ShareX","275F2016A87554554BAB165A0B085EBE15100B63")]
        public void TextToHash_WhenCalledWithTypeRIPEMD160AndUppercase_ShouldReturnRIPEMD160HashOfTextInUppercase(string input,string output)
        {
            var result = TranslatorHelper.TextToHash(input, HashType.RIPEMD160,true);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase("0", 0)]
        [TestCase("1010",10)]
        [TestCase("11111111",255)]
        public void BinaryToByte_WhenCalled_ShouldReturnInByte(string input, byte output)
        {
            var result = TranslatorHelper.BinaryToByte(input);
            Assert.That((result),Is.EqualTo(output));
        }

        [Test]
        [TestCase("010100110110100001100001011100100110010101011000","ShareX")]
        public void BinaryToText_WhenCalled_ShouldReturnText(string binary, string output)
        {
            var result = TranslatorHelper.BinaryToText(binary);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase(0,"00000000")]
        [TestCase(255,"11111111")]
        public void ByteToBinary_WhenCalled_ShouldReturnInBinary(byte input, string binary)
        {
            var result = TranslatorHelper.ByteToBinary(input);
            Assert.That(result,Is.EqualTo(binary));
        }

        [Test]
        [TestCase(new byte[]{0},new string[]{"00"})]
        [TestCase(new byte[]{1},new string[]{"01"})]
        [TestCase(new byte[]{144,66},new string[]{"90","42"})]
        public void ByteToHexadecimal_WhenCalled_ShouldReturnInHexadecimal(byte[] input, string[] output)
        {
            var result = TranslatorHelper.BytesToHexadecimal(input);
            Assert.That(result,Is.EquivalentTo(output));
        }

        [Test]
        [TestCase("90",144)]
        [TestCase("42",66)]
        public void HexadecimalToByte_WhenCalled_ShouldReturnInByte(string input,byte output)
        {
            var result = TranslatorHelper.HexadecimalToByte(input);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase("536861726558","ShareX")]
        public void HexadecimalToText_WhenCalled_ShouldReturnInText(string input,string output)
        {
            var result = TranslatorHelper.HexadecimalToText(input);
            Assert.That(result,Is.EqualTo(output));
        }

        [TestCase("U2hhcmVY","ShareX")]
        [Test]
        public void Base64ToText_WhenCalled_ShouldReturnInText(string input, string output)
        {
            var result = TranslatorHelper.Base64ToText(input);
            Assert.That(result,Is.EqualTo(output));
        }

        [Test]
        [TestCase("32"," ")]
        [TestCase("33","!")]
        public void ASCIIToText_WhenCalled_ShouldReturnInText(string input, string output)
        {
            var result = TranslatorHelper.ASCIIToText(input);
            Assert.That(result,Is.EqualTo(output));
        }
    }
}
