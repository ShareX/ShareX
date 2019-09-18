using NUnit.Framework;
using ShareX.HelpersLib;

namespace ShareX.UnitTests.HelpersLib.Cryptographic
{
    public class TranslatorTests
    {
        private Translator _translator;

        [SetUp]
        public void Setup()
        {
            _translator = new Translator();
        }

        [Test]
        [TestCase("a")]
        [TestCase("ShareX")]
        public void Encoder_WhenCalled_ShouldFillAllPropertiesWithCorrectValue(string value)
        {
            var encodeText = _translator.EncodeText(value);
            Assert.That(encodeText,Is.EqualTo(true));
            Assert.That(_translator.Base64,Is.Not.Empty);
            Assert.That(_translator.Binary,Is.Not.Empty);
            Assert.That(_translator.Hexadecimal,Is.Not.Empty);
            Assert.That(_translator.Text,Is.Not.Empty);
            Assert.That(_translator.BinaryText,Is.Not.Empty);
            Assert.That(_translator.HexadecimalText,Is.Not.Empty);
            Assert.That(_translator.MD5,Is.Not.Empty);
            Assert.That(_translator.CRC32,Is.Not.Empty);
            Assert.That(_translator.SHA1,Is.Not.Empty);
            Assert.That(_translator.SHA256,Is.Not.Empty);
            Assert.That(_translator.SHA384,Is.Not.Empty);
            Assert.That(_translator.SHA512,Is.Not.Empty);
            Assert.That(_translator.ASCII,Is.Not.Empty);
            Assert.That(_translator.ASCIIText,Is.Not.Empty);
            Assert.That(_translator.RIPEMD160,Is.Not.Empty);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Encode_WhenCalledWithInvalidValue_ReturnFalse(string value)
        {
            var encodeText = _translator.EncodeText(value);
            Assert.That(encodeText,Is.EqualTo(false));
        }
    }
}