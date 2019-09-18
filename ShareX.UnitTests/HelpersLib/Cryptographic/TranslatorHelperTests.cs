
using NUnit.Framework;
using ShareX.HelpersLib;

namespace ShareX.UnitTests.HelpersLib.Cryptographic
{
    [TestFixture]
    public class TranslatorHelperTests
    {
        [Test]
        [TestCase("Test", new[]{ "01010100","01100101","01110011","01110100"})]
        [TestCase("0",new[]{"00110000"})]
        [TestCase("ShareX",new[]{"01010011","01101000","01100001","01110010","01100101","01011000"})]
        public void TextToBinary_WhenCalled_ShouldReturnTextInBinary(string input,string[] output)
        {
            var result=TranslatorHelper.TextToBinary(input);
            Assert.That(result,Is.EquivalentTo(output));
        }

        [Test]
        [TestCase("0",new []{"30"})]
        [TestCase("ShareX",new []{"53","68","61","72","65","58"})]
        [TestCase("Test",new []{"54","65","73","74"})]
        public void TextToHexaDecimal_WhenCalled_ShouldReturnTextInHexaDecimal(string input,string[] output)
        {
            var result = TranslatorHelper.TextToHexadecimal(input);
            Assert.That(result,Is.EquivalentTo(output));
        }

        [Test]
        [TestCase("ShareX",new byte[]{83,104,97,114,101,88})]
        [TestCase("Test",new byte[]{84,101,115,116})]
        [TestCase("0",new byte[]{48})]
        public void TextToASCII_WhenCalled_ShouldReturnTextInASCII(string input, byte[] output)
        {
            var result = TranslatorHelper.TextToASCII(input);
            Assert.That(result,Is.EquivalentTo(output));
        }
    }
}
