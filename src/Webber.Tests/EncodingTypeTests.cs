using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xizmark.Webber.Tests
{
    [TestClass]
    public class EncodingTypeTests
    {
        [TestMethod]
        public void Can_Get_ASCII_Encoder()
        {
            EncodingType encodingType = EncodingType.Ascii;

            Encoding encoding = Xizmark.Webber.Webber.GetEncoding(encodingType);

            Assert.IsTrue(encoding is ASCIIEncoding);
        }

        [TestMethod]
        public void Can_Get_UTF7_Encoder()
        {
            EncodingType encodingType = EncodingType.Utf7;

            Encoding encoding = Xizmark.Webber.Webber.GetEncoding(encodingType);

            Assert.IsTrue(encoding is UTF7Encoding);
        }

        [TestMethod]
        public void Can_Get_UTF8_Encoder()
        {
            EncodingType encodingType = EncodingType.Utf8;

            Encoding encoding = Xizmark.Webber.Webber.GetEncoding(encodingType);

            Assert.IsTrue(encoding is UTF8Encoding);
        }

        [TestMethod]
        public void Can_Get_UTF32_Encoder()
        {
            EncodingType encodingType = EncodingType.Utf32;

            Encoding encoding = Xizmark.Webber.Webber.GetEncoding(encodingType);

            Assert.IsTrue(encoding is UTF32Encoding);
        }

        [TestMethod]
        public void Can_Get_Unicode_Encoder()
        {
            EncodingType encodingType = EncodingType.Unicode;

            Encoding encoding = Xizmark.Webber.Webber.GetEncoding(encodingType);

            Assert.IsTrue(encoding is UnicodeEncoding);
        }
    }
}
