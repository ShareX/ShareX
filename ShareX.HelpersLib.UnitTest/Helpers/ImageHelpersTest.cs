using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.IO;
namespace ShareX.HelpersLib.UnitTest
{
    [TestClass]
    public class ImageHelpersTest
    {
        public static string resourcePath = Directory.GetCurrentDirectory() + "/Resources/";
        public static string originalImagePath = resourcePath + "TestImage.jpg";

        [TestMethod]
        public void ResizeImage1Test()
        {
            Image originalImage = Image.FromFile(originalImagePath);
            Image actualImage = ImageHelpers.ResizeImage(originalImage, new Size(100, 100));
            Image expectedImage = Image.FromFile(resourcePath + "ResizeImage1TestResult.jpg");
            Assert.IsTrue(compareImage(expectedImage, actualImage));
        }

        [TestMethod]
        public void ResizeImage2Test()
        {
            Image originalImage = Image.FromFile(originalImagePath);
            Image actualImage = ImageHelpers.ResizeImage(originalImage, 100, 100);
            Image expectImage = Image.FromFile(resourcePath + "ResizeImage2TestResult.jpg");
            Assert.IsTrue(compareImage(expectImage, actualImage));
        }

        [TestMethod]
        public void ResizeImage3Test()
        {
            Image originalImage = Image.FromFile(originalImagePath);
            Image actualImage = ImageHelpers.ResizeImage(originalImage, new Size(100, 100), true, true);
            Image expectImage = Image.FromFile(resourcePath + "ResizeImage3TestResult.jpg");
            Assert.IsTrue(compareImage(expectImage, actualImage));
        }

        [TestMethod]
        public void ResizeImageByPercentage1Test()
        {
            Image originalImage = Image.FromFile(originalImagePath);
            Image actualImage = ImageHelpers.ResizeImageByPercentage(originalImage, 50);
            Image expectImage = Image.FromFile(resourcePath + "ResizeImageByPercentage1TestResult.jpg");
            Assert.IsTrue(compareImage(expectImage, actualImage));
        }

        [TestMethod]
        public void ResizeImageByPercentage2Test()
        {
            Image originalImage = new Bitmap(originalImagePath);
            Image actualImage = ImageHelpers.ResizeImageByPercentage(originalImage, 30, 30);
            Image expectImage = Image.FromFile(resourcePath + "ResizeImageByPercentage2TestResult.jpg");
            Assert.IsTrue(compareImage(expectImage, actualImage));
        }

        [TestMethod]
        public void LoadImageTest()
        {
            Image newImage = ImageHelpers.LoadImage(originalImagePath);
            Assert.IsNotNull(newImage);
        }

        //Compare Image Size and Every Single Pixel
        public bool compareImage(Image expectImage, Image actualImage)
        {
            Bitmap expectBitmapImage = (Bitmap)expectImage;
            Bitmap actualBitmapImage = (Bitmap)actualImage;
            if (expectBitmapImage.Size == actualBitmapImage.Size)
            {
                for (int i = 0; i < actualBitmapImage.Height; i++)
                {
                    for (int j = 0; j < actualBitmapImage.Height; j++)
                        if (expectBitmapImage.GetPixel(i, j) != actualBitmapImage.GetPixel(i, j))
                        {
                            return false;
                        }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
