using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.IO;
namespace ShareX.HelpersLib.UnitTest
{
    [TestClass]
    public class ImageHelpersTest
    {
        public static string resourcePath = Directory.GetCurrentDirectory() + "/Resources/";
        public static Image originalImage = Image.FromFile(Directory.GetCurrentDirectory() + "/Resources/TestImage.jpg");

        [TestMethod]
        public void ResizeImage1Test()
        {
            Size newImageSize = new Size(100, 100);
            Image newImage = ImageHelpers.ResizeImage(originalImage, newImageSize);
            Image expectedImage = Image.FromFile(resourcePath + "ResizeImage1TestResult.jpg");

            Assert.AreEqual(expectedImage.ToString(), newImage.ToString());
        }

        [TestMethod]
        public void ResizeImage2Test()
        {
            Image newImage = ImageHelpers.ResizeImage(originalImage, 100, 100);
            Image expectedImage = Image.FromFile(resourcePath + "ResizeImage2TestResult.jpg");

            Assert.AreEqual(expectedImage.ToString(), newImage.ToString());
        }

        [TestMethod]
        public void ResizeImage3Test()
        {
            //System.AppDomain.CurrentDomain.BaseDirectory
            Image originalImage = new Bitmap("C:\\Users\\horace92\\Pictures\\city.jpg");
            Size newImageSize = new Size(100, 100);
            int expectedWidth = 100;
            int expectedHeight = 100;
            Image newImage = ImageHelpers.ResizeImage(originalImage, newImageSize, true, true);
            newImage.Save("C:\\Users\\horace92\\Pictures\\city1.jpg");
            Assert.AreEqual(expectedWidth, newImage.Width);
            Assert.AreEqual(expectedHeight, newImage.Height);
        }

        [TestMethod]
        public void ResizeImageByPercentage1Test()
        {
            float percentage = 90;
            Image originalImage = new Bitmap("C:\\Users\\horace92\\Pictures\\city.jpg");
            int expectedWidth = (int)(percentage / 100 * originalImage.Width);
            int expectedHeight = (int)(percentage / 100 * originalImage.Height);
            Image newImage = ImageHelpers.ResizeImageByPercentage(originalImage, percentage);

            Assert.AreEqual(expectedWidth, newImage.Width);
            Assert.AreEqual(expectedHeight, newImage.Height);
        }
        
        
        [TestMethod]
        public void ResizeImageByPercentage2Test()
        {
            float percentageWidth = 90;
            float percentageHeight = 100;
            Image originalImage = new Bitmap("C:\\Users\\horace92\\Pictures\\city.jpg");
            int expectedWidth = (int)(percentageWidth / 100 * originalImage.Width);
            int expectedHeight = (int)(percentageHeight / 100 * originalImage.Height);
            Image newImage = ImageHelpers.ResizeImageByPercentage(originalImage, percentageWidth, percentageHeight);

            Assert.AreEqual(expectedWidth, newImage.Width);
            Assert.AreEqual(expectedHeight, newImage.Height);
        }
        
        [TestMethod]
        public void LoadImageTest()
        {
            Image newImage = ImageHelpers.LoadImage("C:\\Users\\horace92\\Pictures\\city.jpg");
            Assert.IsNotNull(newImage);
        }
    }
}
