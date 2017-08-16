using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareX.HelpersLib;
namespace ShareX.UnitTest
{
    [TestClass]
    public class ImageHelpersTest
    {
        [TestMethod]
        public void ResizeImageTest()
        {
            int expectedWidth = 100;
            int expectedHeight = 100;
            
            Image originalImage = new Bitmap("C:\\Users\\horace92\\Pictures\\city.jpg");
            Image newImage = ImageHelpers.ResizeImage(originalImage, 100, 100);

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
