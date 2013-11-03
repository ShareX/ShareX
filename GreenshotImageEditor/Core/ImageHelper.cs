/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Greenshot.Core;
using GreenshotPlugin.UnmanagedHelpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace GreenshotPlugin.Core
{
    /// <summary>
    /// Description of ImageHelper.
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Create a thumbnail from an image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="thumbWidth"></param>
        /// <param name="thumbHeight"></param>
        /// <returns></returns>
        public static Image CreateThumbnail(Image image, int thumbWidth, int thumbHeight)
        {
            return CreateThumbnail(image, thumbWidth, thumbHeight, -1, -1);
        }

        /// <summary>
        /// Create a Thumbnail
        /// </summary>
        /// <param name="image"></param>
        /// <param name="thumbWidth"></param>
        /// <param name="thumbHeight"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static Image CreateThumbnail(Image image, int thumbWidth, int thumbHeight, int maxWidth, int maxHeight)
        {
            int srcWidth = image.Width;
            int srcHeight = image.Height;
            if (thumbHeight < 0)
            {
                thumbHeight = (int)(thumbWidth * ((float)srcHeight / (float)srcWidth));
            }
            if (thumbWidth < 0)
            {
                thumbWidth = (int)(thumbHeight * ((float)srcWidth / (float)srcHeight));
            }
            if (maxWidth > 0 && thumbWidth > maxWidth)
            {
                thumbWidth = Math.Min(thumbWidth, maxWidth);
                thumbHeight = (int)(thumbWidth * ((float)srcHeight / (float)srcWidth));
            }
            if (maxHeight > 0 && thumbHeight > maxHeight)
            {
                thumbHeight = Math.Min(thumbHeight, maxHeight);
                thumbWidth = (int)(thumbHeight * ((float)srcWidth / (float)srcHeight));
            }

            Bitmap bmp = new Bitmap(thumbWidth, thumbHeight);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                Rectangle rectDestination = new Rectangle(0, 0, thumbWidth, thumbHeight);
                graphics.DrawImage(image, rectDestination, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        /// <summary>
        /// Crops the image to the specified rectangle
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="cropRectangle">Rectangle with bitmap coordinates, will be "intersected" to the bitmap</param>
        public static bool Crop(ref Image image, ref Rectangle cropRectangle)
        {
            Image returnImage = null;
            if (image != null && image is Bitmap && ((image.Width * image.Height) > 0))
            {
                cropRectangle.Intersect(new Rectangle(0, 0, image.Width, image.Height));
                if (cropRectangle.Width != 0 || cropRectangle.Height != 0)
                {
                    returnImage = CloneArea(image, cropRectangle, PixelFormat.DontCare);
                    image.Dispose();
                    image = returnImage;
                    return true;
                }
            }
            LOG.Warn("Can't crop a null/zero size image!");
            return false;
        }

        /// <summary>
        /// Private helper method for the FindAutoCropRectangle
        /// </summary>
        /// <param name="fastBitmap"></param>
        /// <param name="colorPoint"></param>
        /// <returns>Rectangle</returns>
        private static Rectangle FindAutoCropRectangle(IFastBitmap fastBitmap, Point colorPoint, int cropDifference)
        {
            Rectangle cropRectangle = Rectangle.Empty;
            Color referenceColor = fastBitmap.GetColorAt(colorPoint.X, colorPoint.Y);
            Point min = new Point(int.MaxValue, int.MaxValue);
            Point max = new Point(int.MinValue, int.MinValue);

            if (cropDifference > 0)
            {
                for (int y = 0; y < fastBitmap.Height; y++)
                {
                    for (int x = 0; x < fastBitmap.Width; x++)
                    {
                        Color currentColor = fastBitmap.GetColorAt(x, y);
                        int diffR = Math.Abs(currentColor.R - referenceColor.R);
                        int diffG = Math.Abs(currentColor.G - referenceColor.G);
                        int diffB = Math.Abs(currentColor.B - referenceColor.B);
                        if (((diffR + diffG + diffB) / 3) > cropDifference)
                        {
                            if (x < min.X) min.X = x;
                            if (y < min.Y) min.Y = y;
                            if (x > max.X) max.X = x;
                            if (y > max.Y) max.Y = y;
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < fastBitmap.Height; y++)
                {
                    for (int x = 0; x < fastBitmap.Width; x++)
                    {
                        Color currentColor = fastBitmap.GetColorAt(x, y);
                        if (referenceColor.Equals(currentColor))
                        {
                            if (x < min.X) min.X = x;
                            if (y < min.Y) min.Y = y;
                            if (x > max.X) max.X = x;
                            if (y > max.Y) max.Y = y;
                        }
                    }
                }
            }

            if (!(Point.Empty.Equals(min) && max.Equals(new Point(fastBitmap.Width - 1, fastBitmap.Height - 1))))
            {
                if (!(min.X == int.MaxValue || min.Y == int.MaxValue || max.X == int.MinValue || min.X == int.MinValue))
                {
                    cropRectangle = new Rectangle(min.X, min.Y, max.X - min.X + 1, max.Y - min.Y + 1);
                }
            }
            return cropRectangle;
        }

        /// <summary>
        /// Get a rectangle for the image which crops the image of all colors equal to that on 0,0
        /// </summary>
        /// <param name="image"></param>
        /// <returns>Rectangle</returns>
        public static Rectangle FindAutoCropRectangle(Image image, int cropDifference)
        {
            Rectangle cropRectangle = Rectangle.Empty;
            Rectangle currentRectangle = Rectangle.Empty;
            List<Point> checkPoints = new List<Point>();
            // Top Left
            checkPoints.Add(new Point(0, 0));
            // Bottom Left
            checkPoints.Add(new Point(0, image.Height - 1));
            // Top Right
            checkPoints.Add(new Point(image.Width - 1, 0));
            // Bottom Right
            checkPoints.Add(new Point(image.Width - 1, image.Height - 1));
            using (IFastBitmap fastBitmap = FastBitmap.Create((Bitmap)image))
            {
                // find biggest area
                foreach (Point checkPoint in checkPoints)
                {
                    currentRectangle = FindAutoCropRectangle(fastBitmap, checkPoint, cropDifference);
                    if (currentRectangle.Width * currentRectangle.Height > cropRectangle.Width * cropRectangle.Height)
                    {
                        cropRectangle = currentRectangle;
                    }
                }
            }
            return cropRectangle;
        }

        /// <summary>
        /// Load an image from file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Image LoadImage(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }
            if (!File.Exists(filename))
            {
                return null;
            }
            Image fileImage = null;
            LOG.InfoFormat("Loading image from file {0}", filename);
            // Fixed lock problem Bug #3431881
            using (Stream imageFileStream = File.OpenRead(filename))
            {
                // And fixed problem that the bitmap stream is disposed... by Cloning the image
                // This also ensures the bitmap is correctly created

                if (filename.EndsWith(".ico"))
                {
                    // Icon logic, try to get the Vista icon, else the biggest possible
                    try
                    {
                        using (Image tmpImage = ExtractVistaIcon(imageFileStream))
                        {
                            if (tmpImage != null)
                            {
                                fileImage = Clone(tmpImage);
                            }
                        }
                    }
                    catch (Exception vistaIconException)
                    {
                        LOG.Warn("Can't read icon from " + filename, vistaIconException);
                    }
                    if (fileImage == null)
                    {
                        try
                        {
                            // No vista icon, try normal icon
                            imageFileStream.Position = 0;
                            // We create a copy of the bitmap, so everything else can be disposed
                            using (Icon tmpIcon = new Icon(imageFileStream, new Size(1024, 1024)))
                            {
                                using (Image tmpImage = tmpIcon.ToBitmap())
                                {
                                    fileImage = Clone(tmpImage);
                                }
                            }
                        }
                        catch (Exception iconException)
                        {
                            LOG.Warn("Can't read icon from " + filename, iconException);
                        }
                    }
                }
                if (fileImage == null)
                {
                    // We create a copy of the bitmap, so everything else can be disposed
                    imageFileStream.Position = 0;
                    using (Image tmpImage = Image.FromStream(imageFileStream, true, true))
                    {
                        LOG.DebugFormat("Loaded {0} with Size {1}x{2} and PixelFormat {3}", filename, tmpImage.Width, tmpImage.Height, tmpImage.PixelFormat);
                        fileImage = Clone(tmpImage);
                    }
                }
            }
            if (fileImage != null)
            {
                LOG.InfoFormat("Information about file {0}: {1}x{2}-{3} Resolution {4}x{5}", filename, fileImage.Width, fileImage.Height, fileImage.PixelFormat, fileImage.HorizontalResolution, fileImage.VerticalResolution);
            }
            return fileImage;
        }

        /// <summary>
        /// Based on: http://www.codeproject.com/KB/cs/IconExtractor.aspx
        /// And a hint from: http://www.codeproject.com/KB/cs/IconLib.aspx
        /// </summary>
        /// <param name="iconStream">Stream with the icon information</param>
        /// <returns>Bitmap with the Vista Icon (256x256)</returns>
        private static Bitmap ExtractVistaIcon(Stream iconStream)
        {
            const int SizeICONDIR = 6;
            const int SizeICONDIRENTRY = 16;
            Bitmap bmpPngExtracted = null;
            try
            {
                byte[] srcBuf = new byte[iconStream.Length];
                iconStream.Read(srcBuf, 0, (int)iconStream.Length);
                int iCount = BitConverter.ToInt16(srcBuf, 4);
                for (int iIndex = 0; iIndex < iCount; iIndex++)
                {
                    int iWidth = srcBuf[SizeICONDIR + SizeICONDIRENTRY * iIndex];
                    int iHeight = srcBuf[SizeICONDIR + SizeICONDIRENTRY * iIndex + 1];
                    int iBitCount = BitConverter.ToInt16(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 6);
                    if (iWidth == 0 && iHeight == 0)
                    {
                        int iImageSize = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 8);
                        int iImageOffset = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 12);
                        using (MemoryStream destStream = new MemoryStream())
                        {
                            destStream.Write(srcBuf, iImageOffset, iImageSize);
                            destStream.Seek(0, SeekOrigin.Begin);
                            bmpPngExtracted = new Bitmap(destStream); // This is PNG! :)
                        }
                        break;
                    }
                }
            }
            catch
            {
                return null;
            }
            return bmpPngExtracted;
        }

        /// <summary>
        /// See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms648069%28v=vs.85%29.aspx
        /// </summary>
        /// <param name="icon">The icon to </param>
        /// <param name="location">The file (EXE or DLL) to get the icon from</param>
        /// <param name="index">Index of the icon</param>
        /// <param name="takeLarge">true if the large icon is wanted</param>
        /// <returns>Icon</returns>
        public static Icon ExtractAssociatedIcon(string location, int index, bool takeLarge)
        {
            IntPtr large;
            IntPtr small;
            Shell32.ExtractIconEx(location, index, out large, out small, 1);
            Icon returnIcon = null;
            bool isLarge = false;
            bool isSmall = false;
            try
            {
                if (takeLarge && !IntPtr.Zero.Equals(large))
                {
                    returnIcon = Icon.FromHandle(large);
                    isLarge = true;
                }
                else if (!IntPtr.Zero.Equals(small))
                {
                    returnIcon = Icon.FromHandle(small);
                    isSmall = true;
                }
                else if (!IntPtr.Zero.Equals(large))
                {
                    returnIcon = Icon.FromHandle(large);
                    isLarge = true;
                }
            }
            finally
            {
                if (isLarge && !IntPtr.Zero.Equals(small))
                {
                    User32.DestroyIcon(small);
                }
                if (isSmall && !IntPtr.Zero.Equals(large))
                {
                    User32.DestroyIcon(large);
                }
            }
            return returnIcon;
        }

        /// <summary>
        /// Get the number of icon in the file
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="location">Location of the EXE or DLL</param>
        /// <returns></returns>
        public static int CountAssociatedIcons(string location)
        {
            IntPtr large = IntPtr.Zero;
            IntPtr small = IntPtr.Zero;
            return Shell32.ExtractIconEx(location, -1, out large, out small, 0);
        }

        /// <summary>
        /// Apply the effect to the bitmap
        /// </summary>
        /// <param name="sourceBitmap">Bitmap</param>
        /// <param name="effect">IEffect</param>
        /// <returns>Bitmap</returns>
        public static Image ApplyEffect(Image sourceImage, IEffect effect, out Point offset)
        {
            List<IEffect> effects = new List<IEffect>();
            effects.Add(effect);
            return ApplyEffects(sourceImage, effects, out offset);
        }

        /// <summary>
        /// Apply the effects in the supplied order to the bitmap
        /// </summary>
        /// <param name="sourceBitmap">Bitmap</param>
        /// <param name="effects">List<IEffect></param>
        /// <returns>Bitmap</returns>
        public static Image ApplyEffects(Image sourceImage, List<IEffect> effects, out Point offset)
        {
            Image currentImage = sourceImage;
            bool disposeImage = false;
            // Default out value for the offset, will be modified there where needed
            offset = new Point(0, 0);
            Point tmpPoint;
            foreach (IEffect effect in effects)
            {
                Image tmpImage = effect.Apply(currentImage, out tmpPoint);
                if (tmpImage != null)
                {
                    offset.Offset(tmpPoint);
                    if (disposeImage)
                    {
                        currentImage.Dispose();
                    }
                    currentImage = tmpImage;
                    tmpImage = null;
                    // Make sure the "new" image is disposed
                    disposeImage = true;
                }
            }
            return currentImage;
        }

        /// <summary>
        /// Make the picture look like it's torn
        /// </summary>
        /// <param name="sourceImage">Bitmap to make torn edge off</param>
        /// <param name="toothHeight">How large (height) is each tooth</param>
        /// <param name="horizontalToothRange">How wide is a horizontal tooth</param>
        /// <param name="verticalToothRange">How wide is a vertical tooth</param>
        /// <returns>Changed bitmap</returns>
        public static Image CreateTornEdge(Image sourceImage, int toothHeight, int horizontalToothRange, int verticalToothRange)
        {
            Image returnImage = CreateEmpty(sourceImage.Width, sourceImage.Height, PixelFormat.Format32bppArgb, Color.Empty, sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            using (GraphicsPath path = new GraphicsPath())
            {
                Random random = new Random();
                int HorizontalRegions = (int)(sourceImage.Width / horizontalToothRange);
                int VerticalRegions = (int)(sourceImage.Height / verticalToothRange);

                // Start
                Point previousEndingPoint = new Point(horizontalToothRange, random.Next(1, toothHeight));
                Point newEndingPoint;
                // Top
                for (int i = 0; i < HorizontalRegions; i++)
                {
                    int x = (int)previousEndingPoint.X + horizontalToothRange;
                    int y = random.Next(1, toothHeight);
                    newEndingPoint = new Point(x, y);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }

                // Right
                for (int i = 0; i < VerticalRegions; i++)
                {
                    int x = sourceImage.Width - random.Next(1, toothHeight);
                    int y = (int)previousEndingPoint.Y + verticalToothRange;
                    newEndingPoint = new Point(x, y);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }

                // Bottom
                for (int i = 0; i < HorizontalRegions; i++)
                {
                    int x = (int)previousEndingPoint.X - horizontalToothRange;
                    int y = sourceImage.Height - random.Next(1, toothHeight);
                    newEndingPoint = new Point(x, y);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }

                // Left
                for (int i = 0; i < VerticalRegions; i++)
                {
                    int x = random.Next(1, toothHeight);
                    int y = (int)previousEndingPoint.Y - verticalToothRange;
                    newEndingPoint = new Point(x, y);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }
                path.CloseFigure();

                // Draw the created figure with the original image by using a TextureBrush so we have anti-aliasing
                using (Graphics graphics = Graphics.FromImage(returnImage))
                {
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    using (Brush brush = new TextureBrush(sourceImage))
                    {
                        // Imporant note: If the target wouldn't be at 0,0 we need to translate-transform!!
                        graphics.FillPath(brush, path);
                    }
                }
            }
            return returnImage;
        }

        /// <summary>
        /// Helper Method for the ApplyBlur
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        private static int[] CreateGaussianBlurRow(int amount)
        {
            int size = 1 + (amount * 2);
            int[] weights = new int[size];

            for (int i = 0; i <= amount; ++i)
            {
                // 1 + aa - aa + 2ai - ii
                weights[i] = 16 * (i + 1);
                weights[weights.Length - i - 1] = weights[i];
            }

            return weights;
        }

        /// <summary>
        /// Apply BoxBlur to the destinationBitmap
        /// </summary>
        /// <param name="destinationBitmap">Bitmap to blur</param>
        /// <param name="range">Must be ODD!</param>
        public static void ApplyBoxBlur(Bitmap destinationBitmap, int range)
        {
            // We only need one fastbitmap as we use it as source and target (the reading is done for one line H/V, writing after "parsing" one line H/V)
            using (IFastBitmap fastBitmap = FastBitmap.Create(destinationBitmap))
            {
                ApplyBoxBlur(fastBitmap, range);
            }
        }

        /// <summary>
        /// Apply BoxBlur to the fastBitmap
        /// </summary>
        /// <param name="fastBitmap">IFastBitmap to blur</param>
        /// <param name="range">Must be ODD!</param>
        public static void ApplyBoxBlur(IFastBitmap fastBitmap, int range)
        {
            // Range must be odd!
            if ((range & 1) == 0)
            {
                range++;
            }
            if (range <= 1)
            {
                return;
            }
            // Box blurs are frequently used to approximate a Gaussian blur.
            // By the central limit theorem, if applied 3 times on the same image, a box blur approximates the Gaussian kernel to within about 3%, yielding the same result as a quadratic convolution kernel.
            // This might be true, but the GDI+ BlurEffect doesn't look the same, a 2x blur is more simular and we only make 2x Box-Blur.
            // (Might also be a mistake in our blur, but for now it looks great)
            if (fastBitmap.hasAlphaChannel)
            {
                BoxBlurHorizontalAlpha(fastBitmap, range);
                BoxBlurVerticalAlpha(fastBitmap, range);
                BoxBlurHorizontalAlpha(fastBitmap, range);
                BoxBlurVerticalAlpha(fastBitmap, range);
            }
            else
            {
                BoxBlurHorizontal(fastBitmap, range);
                BoxBlurVertical(fastBitmap, range);
                BoxBlurHorizontal(fastBitmap, range);
                BoxBlurVertical(fastBitmap, range);
            }
        }

        /// <summary>
        /// BoxBlurHorizontal is a private helper method for the BoxBlur
        /// </summary>
        /// <param name="targetFastBitmap">Target BitmapBuffer</param>
        /// <param name="range">Range must be odd!</param>
        private static void BoxBlurHorizontal(IFastBitmap targetFastBitmap, int range)
        {
            if (targetFastBitmap.hasAlphaChannel)
            {
                throw new NotSupportedException("BoxBlurHorizontal should NOT be called for bitmaps with alpha channel");
            }
            int halfRange = range / 2;
            Color[] newColors = new Color[targetFastBitmap.Width];
            byte[] tmpColor = new byte[3];
            for (int y = targetFastBitmap.Top; y < targetFastBitmap.Bottom; y++)
            {
                int hits = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                for (int x = targetFastBitmap.Left - halfRange; x < targetFastBitmap.Right; x++)
                {
                    int oldPixel = x - halfRange - 1;
                    if (oldPixel >= targetFastBitmap.Left)
                    {
                        targetFastBitmap.GetColorAt(oldPixel, y, tmpColor);
                        r -= tmpColor[FastBitmap.COLOR_INDEX_R];
                        g -= tmpColor[FastBitmap.COLOR_INDEX_G];
                        b -= tmpColor[FastBitmap.COLOR_INDEX_B];
                        hits--;
                    }

                    int newPixel = x + halfRange;
                    if (newPixel < targetFastBitmap.Right)
                    {
                        targetFastBitmap.GetColorAt(newPixel, y, tmpColor);
                        r += tmpColor[FastBitmap.COLOR_INDEX_R];
                        g += tmpColor[FastBitmap.COLOR_INDEX_G];
                        b += tmpColor[FastBitmap.COLOR_INDEX_B];
                        hits++;
                    }

                    if (x >= targetFastBitmap.Left)
                    {
                        newColors[x - targetFastBitmap.Left] = Color.FromArgb(255, (byte)(r / hits), (byte)(g / hits), (byte)(b / hits));
                    }
                }
                for (int x = targetFastBitmap.Left; x < targetFastBitmap.Right; x++)
                {
                    targetFastBitmap.SetColorAt(x, y, newColors[x - targetFastBitmap.Left]);
                }
            }
        }

        /// <summary>
        /// BoxBlurHorizontal is a private helper method for the BoxBlur, only for IFastBitmaps with alpha channel
        /// </summary>
        /// <param name="bbbSrc">Source BitmapBuffer</param>
        /// <param name="targetFastBitmap">Target BitmapBuffer</param>
        /// <param name="range">Range must be odd!</param>
        private static void BoxBlurHorizontalAlpha(IFastBitmap targetFastBitmap, int range)
        {
            if (!targetFastBitmap.hasAlphaChannel)
            {
                throw new NotSupportedException("BoxBlurHorizontalAlpha should be called for bitmaps with alpha channel");
            }
            int halfRange = range / 2;
            Color[] newColors = new Color[targetFastBitmap.Width];
            byte[] tmpColor = new byte[4];
            for (int y = targetFastBitmap.Top; y < targetFastBitmap.Bottom; y++)
            {
                int hits = 0;
                int a = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                for (int x = targetFastBitmap.Left - halfRange; x < targetFastBitmap.Right; x++)
                {
                    int oldPixel = x - halfRange - 1;
                    if (oldPixel >= targetFastBitmap.Left)
                    {
                        targetFastBitmap.GetColorAt(oldPixel, y, tmpColor);
                        a -= tmpColor[FastBitmap.COLOR_INDEX_A];
                        r -= tmpColor[FastBitmap.COLOR_INDEX_R];
                        g -= tmpColor[FastBitmap.COLOR_INDEX_G];
                        b -= tmpColor[FastBitmap.COLOR_INDEX_B];
                        hits--;
                    }

                    int newPixel = x + halfRange;
                    if (newPixel < targetFastBitmap.Right)
                    {
                        targetFastBitmap.GetColorAt(newPixel, y, tmpColor);
                        a += tmpColor[FastBitmap.COLOR_INDEX_A];
                        r += tmpColor[FastBitmap.COLOR_INDEX_R];
                        g += tmpColor[FastBitmap.COLOR_INDEX_G];
                        b += tmpColor[FastBitmap.COLOR_INDEX_B];
                        hits++;
                    }

                    if (x >= targetFastBitmap.Left)
                    {
                        newColors[x - targetFastBitmap.Left] = Color.FromArgb((byte)(a / hits), (byte)(r / hits), (byte)(g / hits), (byte)(b / hits));
                    }
                }
                for (int x = targetFastBitmap.Left; x < targetFastBitmap.Right; x++)
                {
                    targetFastBitmap.SetColorAt(x, y, newColors[x - targetFastBitmap.Left]);
                }
            }
        }

        /// <summary>
        /// BoxBlurVertical is a private helper method for the BoxBlur
        /// </summary>
        /// <param name="targetFastBitmap">BitmapBuffer which previously was created with BoxBlurHorizontal</param>
        /// <param name="range">Range must be odd!</param>
        private static void BoxBlurVertical(IFastBitmap targetFastBitmap, int range)
        {
            if (targetFastBitmap.hasAlphaChannel)
            {
                throw new NotSupportedException("BoxBlurVertical should NOT be called for bitmaps with alpha channel");
            }
            int w = targetFastBitmap.Width;
            int halfRange = range / 2;
            Color[] newColors = new Color[targetFastBitmap.Height];
            int oldPixelOffset = -(halfRange + 1) * w;
            int newPixelOffset = (halfRange) * w;
            byte[] tmpColor = new byte[4];
            for (int x = targetFastBitmap.Left; x < targetFastBitmap.Right; x++)
            {
                int hits = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                for (int y = targetFastBitmap.Top - halfRange; y < targetFastBitmap.Bottom; y++)
                {
                    int oldPixel = y - halfRange - 1;
                    if (oldPixel >= targetFastBitmap.Top)
                    {
                        targetFastBitmap.GetColorAt(x, oldPixel, tmpColor);
                        r -= tmpColor[FastBitmap.COLOR_INDEX_R];
                        g -= tmpColor[FastBitmap.COLOR_INDEX_G];
                        b -= tmpColor[FastBitmap.COLOR_INDEX_B];
                        hits--;
                    }

                    int newPixel = y + halfRange;
                    if (newPixel < targetFastBitmap.Bottom)
                    {
                        targetFastBitmap.GetColorAt(x, newPixel, tmpColor);
                        r += tmpColor[FastBitmap.COLOR_INDEX_R];
                        g += tmpColor[FastBitmap.COLOR_INDEX_G];
                        b += tmpColor[FastBitmap.COLOR_INDEX_B];
                        hits++;
                    }

                    if (y >= targetFastBitmap.Top)
                    {
                        newColors[y - targetFastBitmap.Top] = Color.FromArgb(255, (byte)(r / hits), (byte)(g / hits), (byte)(b / hits));
                    }
                }

                for (int y = targetFastBitmap.Top; y < targetFastBitmap.Bottom; y++)
                {
                    targetFastBitmap.SetColorAt(x, y, newColors[y - targetFastBitmap.Top]);
                }
            }
        }

        /// <summary>
        /// BoxBlurVertical is a private helper method for the BoxBlur
        /// </summary>
        /// <param name="targetFastBitmap">BitmapBuffer which previously was created with BoxBlurHorizontal</param>
        /// <param name="range">Range must be odd!</param>
        private static void BoxBlurVerticalAlpha(IFastBitmap targetFastBitmap, int range)
        {
            if (!targetFastBitmap.hasAlphaChannel)
            {
                throw new NotSupportedException("BoxBlurVerticalAlpha should be called for bitmaps with alpha channel");
            }

            int w = targetFastBitmap.Width;
            int halfRange = range / 2;
            Color[] newColors = new Color[targetFastBitmap.Height];
            int oldPixelOffset = -(halfRange + 1) * w;
            int newPixelOffset = (halfRange) * w;
            byte[] tmpColor = new byte[4];
            for (int x = targetFastBitmap.Left; x < targetFastBitmap.Right; x++)
            {
                int hits = 0;
                int a = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                for (int y = targetFastBitmap.Top - halfRange; y < targetFastBitmap.Bottom; y++)
                {
                    int oldPixel = y - halfRange - 1;
                    if (oldPixel >= targetFastBitmap.Top)
                    {
                        targetFastBitmap.GetColorAt(x, oldPixel, tmpColor);
                        a -= tmpColor[FastBitmap.COLOR_INDEX_A];
                        r -= tmpColor[FastBitmap.COLOR_INDEX_R];
                        g -= tmpColor[FastBitmap.COLOR_INDEX_G];
                        b -= tmpColor[FastBitmap.COLOR_INDEX_B];
                        hits--;
                    }

                    int newPixel = y + halfRange;
                    if (newPixel < targetFastBitmap.Bottom)
                    {
                        //int colorg = pixels[index + newPixelOffset];
                        targetFastBitmap.GetColorAt(x, newPixel, tmpColor);
                        a += tmpColor[FastBitmap.COLOR_INDEX_A];
                        r += tmpColor[FastBitmap.COLOR_INDEX_R];
                        g += tmpColor[FastBitmap.COLOR_INDEX_G];
                        b += tmpColor[FastBitmap.COLOR_INDEX_B];
                        hits++;
                    }

                    if (y >= targetFastBitmap.Top)
                    {
                        newColors[y - targetFastBitmap.Top] = Color.FromArgb((byte)(a / hits), (byte)(r / hits), (byte)(g / hits), (byte)(b / hits));
                    }
                }

                for (int y = targetFastBitmap.Top; y < targetFastBitmap.Bottom; y++)
                {
                    targetFastBitmap.SetColorAt(x, y, newColors[y - targetFastBitmap.Top]);
                }
            }
        }

        /// <summary>
        /// This method fixes the problem that we can't apply a filter outside the target bitmap,
        /// therefor the filtered-bitmap will be shifted if we try to draw it outside the target bitmap.
        /// It will also account for the Invert flag.
        /// </summary>
        /// <param name="applySize"></param>
        /// <param name="rect"></param>
        /// <param name="invert"></param>
        /// <returns></returns>
        public static Rectangle CreateIntersectRectangle(Size applySize, Rectangle rect, bool invert)
        {
            Rectangle myRect;
            if (invert)
            {
                myRect = new Rectangle(0, 0, applySize.Width, applySize.Height);
            }
            else
            {
                Rectangle applyRect = new Rectangle(0, 0, applySize.Width, applySize.Height);
                myRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                myRect.Intersect(applyRect);
            }
            return myRect;
        }

        /// <summary>
        /// Create a new bitmap where the sourceBitmap has a shadow
        /// </summary>
        /// <param name="sourceBitmap">Bitmap to make a shadow on</param>
        /// <param name="darkness">How dark is the shadow</param>
        /// <param name="shadowSize">Size of the shadow</param>
        /// <param name="targetPixelformat">What pixel format must the returning bitmap have</param>
        /// <param name="offset">How many pixels is the original image moved?</param>
        /// <returns>Bitmap with the shadow, is bigger than the sourceBitmap!!</returns>
        public static Bitmap CreateShadow(Image sourceBitmap, float darkness, int shadowSize, Point shadowOffset, out Point offset, PixelFormat targetPixelformat)
        {
            // Create a new "clean" image
            offset = shadowOffset;
            offset.X += shadowSize - 1;
            offset.Y += shadowSize - 1;
            Bitmap returnImage = CreateEmpty(sourceBitmap.Width + (shadowSize * 2), sourceBitmap.Height + (shadowSize * 2), targetPixelformat, Color.Empty, sourceBitmap.HorizontalResolution, sourceBitmap.VerticalResolution);
            // Make sure the shadow is odd, there is no reason for an even blur!
            if ((shadowSize & 1) == 0)
            {
                shadowSize++;
            }
            bool useGDIBlur = GDIplus.IsBlurPossible(shadowSize);
            // Create "mask" for the shadow
            ColorMatrix maskMatrix = new ColorMatrix();
            maskMatrix.Matrix00 = 0;
            maskMatrix.Matrix11 = 0;
            maskMatrix.Matrix22 = 0;
            if (useGDIBlur)
            {
                maskMatrix.Matrix33 = darkness + 0.1f;
            }
            else
            {
                maskMatrix.Matrix33 = darkness;
            }
            Rectangle shadowRectangle = new Rectangle(new Point(shadowSize, shadowSize), sourceBitmap.Size);
            ApplyColorMatrix((Bitmap)sourceBitmap, Rectangle.Empty, returnImage, shadowRectangle, maskMatrix);

            // blur "shadow", apply to whole new image
            if (useGDIBlur)
            {
                // Use GDI Blur
                Rectangle newImageRectangle = new Rectangle(0, 0, returnImage.Width, returnImage.Height);
                GDIplus.ApplyBlur(returnImage, newImageRectangle, shadowSize + 1, false);
            }
            else
            {
                // try normal software blur
                //returnImage = CreateBlur(returnImage, newImageRectangle, true, shadowSize, 1d, false, newImageRectangle);
                ApplyBoxBlur(returnImage, shadowSize);
            }

            // Draw the original image over the shadow
            using (Graphics graphics = Graphics.FromImage(returnImage))
            {
                // Make sure we draw with the best quality!
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // draw original with a TextureBrush so we have nice antialiasing!
                using (Brush textureBrush = new TextureBrush(sourceBitmap, WrapMode.Clamp))
                {
                    // We need to do a translate-tranform otherwise the image is wrapped
                    graphics.TranslateTransform(offset.X, offset.Y);
                    graphics.FillRectangle(textureBrush, 0, 0, sourceBitmap.Width, sourceBitmap.Height);
                }
            }
            return returnImage;
        }

        /// <summary>
        /// Return negative of Bitmap
        /// </summary>
        /// <param name="sourceImage">Bitmap to create a negative off</param>
        /// <returns>Negative bitmap</returns>
        public static Bitmap CreateNegative(Image sourceImage)
        {
            Bitmap clone = (Bitmap)Clone(sourceImage);
            ColorMatrix invertMatrix = new ColorMatrix(new float[][] {
				new float[] {-1, 0, 0, 0, 0},
				new float[] {0, -1, 0, 0, 0},
				new float[] {0, 0, -1, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {1, 1, 1, 1, 1}
			});
            ApplyColorMatrix(clone, invertMatrix);
            return clone;
        }

        /// <summary>
        /// Apply a color matrix to the image
        /// </summary>
        /// <param name="source">Image to apply matrix to</param>
        /// <param name="colorMatrix">ColorMatrix to apply</param>
        public static void ApplyColorMatrix(Bitmap source, ColorMatrix colorMatrix)
        {
            ApplyColorMatrix(source, Rectangle.Empty, source, Rectangle.Empty, colorMatrix);
        }

        /// <summary>
        /// Apply a color matrix by copying from the source to the destination
        /// </summary>
        /// <param name="source">Image to copy from</param>
        /// <param name="sourceRect">Rectangle to copy from</param>
        /// <param name="destRect">Rectangle to copy to</param>
        /// <param name="dest">Image to copy to</param>
        /// <param name="colorMatrix">ColorMatrix to apply</param>
        public static void ApplyColorMatrix(Bitmap source, Rectangle sourceRect, Bitmap dest, Rectangle destRect, ColorMatrix colorMatrix)
        {
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(colorMatrix);
            ApplyImageAttributes(source, sourceRect, dest, destRect, imageAttributes);
        }

        /// <summary>
        /// Apply image attributes to the image
        /// </summary>
        /// <param name="source">Image to apply matrix to</param>
        /// <param name="imageAttributes">ImageAttributes to apply</param>
        public static void ApplyColorMatrix(Bitmap source, ImageAttributes imageAttributes)
        {
            ApplyImageAttributes(source, Rectangle.Empty, source, Rectangle.Empty, imageAttributes);
        }

        /// <summary>
        /// Apply a color matrix by copying from the source to the destination
        /// </summary>
        /// <param name="source">Image to copy from</param>
        /// <param name="sourceRect">Rectangle to copy from</param>
        /// <param name="destRect">Rectangle to copy to</param>
        /// <param name="dest">Image to copy to</param>
        /// <param name="imageAttributes">ImageAttributes to apply</param>
        public static void ApplyImageAttributes(Bitmap source, Rectangle sourceRect, Bitmap dest, Rectangle destRect, ImageAttributes imageAttributes)
        {
            if (sourceRect == Rectangle.Empty)
            {
                sourceRect = new Rectangle(0, 0, source.Width, source.Height);
            }
            if (dest == null)
            {
                dest = source;
            }
            if (destRect == Rectangle.Empty)
            {
                destRect = new Rectangle(0, 0, dest.Width, dest.Height);
            }
            using (Graphics graphics = Graphics.FromImage(dest))
            {
                // Make sure we draw with the best quality!
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;

                graphics.DrawImage(source, destRect, sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height, GraphicsUnit.Pixel, imageAttributes);
            }
        }

        /// <summary>
        /// Returns a b/w of Bitmap
        /// </summary>
        /// <param name="sourceImage">Bitmap to create a b/w of</param>
        /// <param name="threshold">Threshold for monochrome filter (0 - 255), lower value means less black</param>
        /// <returns>b/w bitmap</returns>
        public static Bitmap CreateMonochrome(Image sourceImage, byte threshold)
        {
            using (IFastBitmap fastBitmap = FastBitmap.CreateCloneOf(sourceImage, sourceImage.PixelFormat))
            {
                for (int y = 0; y < fastBitmap.Height; y++)
                {
                    for (int x = 0; x < fastBitmap.Width; x++)
                    {
                        Color color = fastBitmap.GetColorAt(x, y);
                        int colorBrightness = ((color.R + color.G + color.B) / 3 > threshold) ? 255 : 0;
                        Color monoColor = Color.FromArgb(color.A, colorBrightness, colorBrightness, colorBrightness);
                        fastBitmap.SetColorAt(x, y, monoColor);
                    }
                }
                return fastBitmap.UnlockAndReturnBitmap();
            }
        }

        /// <summary>
        /// Create a new bitmap where the sourceBitmap has a Simple border around it
        /// </summary>
        /// <param name="sourceImage">Bitmap to make a border on</param>
        /// <param name="borderSize">Size of the border</param>
        /// <param name="borderColor">Color of the border</param>
        /// <param name="targetPixelformat">What pixel format must the returning bitmap have</param>
        /// <param name="offset">How many pixels is the original image moved?</param>
        /// <returns>Bitmap with the shadow, is bigger than the sourceBitmap!!</returns>
        public static Image CreateBorder(Image sourceImage, int borderSize, Color borderColor, PixelFormat targetPixelformat, out Point offset)
        {
            // "return" the shifted offset, so the caller can e.g. move elements
            offset = new Point(borderSize, borderSize);

            // Create a new "clean" image
            Bitmap newImage = CreateEmpty(sourceImage.Width + (borderSize * 2), sourceImage.Height + (borderSize * 2), targetPixelformat, Color.Empty, sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                // Make sure we draw with the best quality!
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddRectangle(new Rectangle(borderSize >> 1, borderSize >> 1, newImage.Width - (borderSize), newImage.Height - (borderSize)));
                    using (Pen pen = new Pen(borderColor, borderSize))
                    {
                        pen.LineJoin = LineJoin.Round;
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        graphics.DrawPath(pen, path);
                    }
                }
                // draw original with a TextureBrush so we have nice antialiasing!
                using (Brush textureBrush = new TextureBrush(sourceImage, WrapMode.Clamp))
                {
                    // We need to do a translate-tranform otherwise the image is wrapped
                    graphics.TranslateTransform(offset.X, offset.Y);
                    graphics.FillRectangle(textureBrush, 0, 0, sourceImage.Width, sourceImage.Height);
                }
            }
            return newImage;
        }

        /// <summary>
        /// Create ImageAttributes to modify
        /// </summary>
        /// <param name="brightness"></param>
        /// <param name="contrast"></param>
        /// <param name="gamma"></param>
        /// <returns>ImageAttributes</returns>
        public static ImageAttributes CreateAdjustAttributes(float brightness, float contrast, float gamma)
        {
            float adjustedBrightness = brightness - 1.0f;
            ColorMatrix applyColorMatrix = new ColorMatrix(
                    new float[][] {
						new float[] {contrast, 0, 0, 0, 0}, // scale red
						new float[] {0, contrast, 0, 0, 0}, // scale green
						new float[] {0, 0, contrast, 0, 0}, // scale blue
						new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
						new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}
					});

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();
            attributes.ClearColorMatrix();
            attributes.SetColorMatrix(applyColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            attributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            return attributes;
        }

        /// <summary>
        /// Adjust the brightness, contract or gamma of an image.
        /// Use the value "1.0f" for no changes.
        /// </summary>
        /// <param name="sourceImage">Original bitmap</param>
        /// <returns>Bitmap with grayscale</returns>
        public static Image Adjust(Image sourceImage, float brightness, float contrast, float gamma)
        {
            //create a blank bitmap the same size as original
            // If using 8bpp than the following exception comes: A Graphics object cannot be created from an image that has an indexed pixel format.
            Bitmap newBitmap = CreateEmpty(sourceImage.Width, sourceImage.Height, PixelFormat.Format24bppRgb, Color.Empty, sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            ApplyImageAttributes((Bitmap)sourceImage, Rectangle.Empty, newBitmap, Rectangle.Empty, CreateAdjustAttributes(brightness, contrast, gamma));
            return newBitmap;
        }

        /// <summary>
        /// Create a new bitmap where the sourceBitmap is in grayscale
        /// </summary>
        /// <param name="sourceImage">Original bitmap</param>
        /// <returns>Bitmap with grayscale</returns>
        public static Image CreateGrayscale(Image sourceImage)
        {
            Bitmap clone = (Bitmap)Clone(sourceImage);
            ColorMatrix grayscaleMatrix = new ColorMatrix(new float[][] {
				new float[] {.3f, .3f, .3f, 0, 0},
				new float[] {.59f, .59f, .59f, 0, 0},
				new float[] {.11f, .11f, .11f, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {0, 0, 0, 0, 1}
			});
            ApplyColorMatrix(clone, grayscaleMatrix);
            return clone;
        }

        /// <summary>
        /// Checks if the supplied Bitmap has a PixelFormat we support
        /// </summary>
        /// <param name="image">bitmap to check</param>
        /// <returns>bool if we support it</returns>
        public static bool SupportsPixelFormat(Image image)
        {
            return SupportsPixelFormat(image.PixelFormat);
        }

        /// <summary>
        /// Checks if we support the pixel format
        /// </summary>
        /// <param name="pixelformat">PixelFormat to check</param>
        /// <returns>bool if we support it</returns>
        public static bool SupportsPixelFormat(PixelFormat pixelformat)
        {
            return (pixelformat.Equals(PixelFormat.Format32bppArgb) ||
                    pixelformat.Equals(PixelFormat.Format32bppPArgb) ||
                    pixelformat.Equals(PixelFormat.Format32bppRgb) ||
                    pixelformat.Equals(PixelFormat.Format24bppRgb));
        }

        /// <summary>
        /// Wrapper for just cloning which calls the CloneArea
        /// </summary>
        /// <param name="sourceImage">Image to clone</param>
        /// <returns>Bitmap with clone image data</returns>
        public static Image Clone(Image sourceImage)
        {
            if (sourceImage is Metafile)
            {
                return (Image)sourceImage.Clone();
            }
            return CloneArea(sourceImage, Rectangle.Empty, PixelFormat.DontCare);
        }

        /// <summary>
        /// Wrapper for just cloning & TargetFormat which calls the CloneArea
        /// </summary>
        /// <param name="sourceBitmap">Image to clone</param>
        /// <param name="targetFormat">Target Format, use PixelFormat.DontCare if you want the original (or a default if the source PixelFormat is not supported)</param>
        /// <returns>Bitmap with clone image data</returns>
        public static Bitmap Clone(Image sourceBitmap, PixelFormat targetFormat)
        {
            return CloneArea(sourceBitmap, Rectangle.Empty, targetFormat);
        }

        /// <summary>
        /// Clone an image, taking some rules into account:
        /// 1) When sourceRect is the whole bitmap there is a GDI+ bug in Clone
        ///		Clone will than return the same PixelFormat as the source
        ///		a quick workaround is using new Bitmap which uses a default of Format32bppArgb
        ///	2) When going from a transparent to a non transparent bitmap, we draw the background white!
        /// </summary>
        /// <param name="sourceImage">Source bitmap to clone</param>
        /// <param name="sourceRect">Rectangle to copy from the source, use Rectangle.Empty for all</param>
        /// <param name="targetFormat">Target Format, use PixelFormat.DontCare if you want the original (or a default if the source PixelFormat is not supported)</param>
        /// <returns></returns>
        public static Bitmap CloneArea(Image sourceImage, Rectangle sourceRect, PixelFormat targetFormat)
        {
            Bitmap newImage = null;
            Rectangle bitmapRect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);

            // Make sure the source is not Rectangle.Empty
            if (Rectangle.Empty.Equals(sourceRect))
            {
                sourceRect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
            }
            else
            {
                sourceRect.Intersect(bitmapRect);
            }

            // If no pixelformat is supplied
            if (PixelFormat.DontCare == targetFormat || PixelFormat.Undefined == targetFormat)
            {
                if (SupportsPixelFormat(sourceImage.PixelFormat))
                {
                    targetFormat = sourceImage.PixelFormat;
                }
                else if (Image.IsAlphaPixelFormat(sourceImage.PixelFormat))
                {
                    targetFormat = PixelFormat.Format32bppArgb;
                }
                else
                {
                    targetFormat = PixelFormat.Format24bppRgb;
                }
            }

            // check the target format
            if (!SupportsPixelFormat(targetFormat))
            {
                if (Image.IsAlphaPixelFormat(targetFormat))
                {
                    targetFormat = PixelFormat.Format32bppArgb;
                }
                else
                {
                    targetFormat = PixelFormat.Format24bppRgb;
                }
            }

            bool destinationIsTransparent = Image.IsAlphaPixelFormat(targetFormat);
            bool sourceIsTransparent = Image.IsAlphaPixelFormat(sourceImage.PixelFormat);
            bool fromTransparentToNon = !destinationIsTransparent && sourceIsTransparent;
            bool isBitmap = sourceImage is Bitmap;
            bool isAreaEqual = sourceRect.Equals(bitmapRect);
            if (isAreaEqual || fromTransparentToNon || !isBitmap)
            {
                // Rule 1: if the areas are equal, always copy ourselves
                newImage = new Bitmap(bitmapRect.Width, bitmapRect.Height, targetFormat);
                // Make sure both images have the same resolution
                newImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);

                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    if (fromTransparentToNon)
                    {
                        // Rule 2: Make sure the background color is white
                        graphics.Clear(Color.White);
                    }
                    // decide fastest copy method
                    if (isAreaEqual)
                    {
                        graphics.DrawImageUnscaled(sourceImage, 0, 0);
                    }
                    else
                    {
                        graphics.DrawImage(sourceImage, 0, 0, sourceRect, GraphicsUnit.Pixel);
                    }
                }
            }
            else
            {
                // Let GDI+ decide how to convert, need to test what is quicker...
                newImage = (sourceImage as Bitmap).Clone(sourceRect, targetFormat);
                // Make sure both images have the same resolution
                newImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            }
            return newImage;
        }

        /// <summary>
        /// Rotate the bitmap
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="rotateFlipType"></param>
        /// <returns></returns>
        public static Image RotateFlip(Image sourceImage, RotateFlipType rotateFlipType)
        {
            Image returnImage = Clone(sourceImage);
            returnImage.RotateFlip(rotateFlipType);
            return returnImage;
        }

        /// <summary>
        /// A generic way to create an empty image
        /// </summary>
        /// <param name="sourceImage">the source bitmap as the specifications for the new bitmap</param>
        /// <param name="backgroundColor">The color to fill with, or Color.Empty to take the default depending on the pixel format</param>
        /// <returns></returns>
        public static Bitmap CreateEmptyLike(Image sourceImage, Color backgroundColor)
        {
            PixelFormat pixelFormat = sourceImage.PixelFormat;
            if (backgroundColor.A < 255)
            {
                pixelFormat = PixelFormat.Format32bppArgb;
            }
            return CreateEmpty(sourceImage.Width, sourceImage.Height, pixelFormat, backgroundColor, sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
        }

        /// <summary>
        /// A generic way to create an empty image
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <param name="backgroundColor">The color to fill with, or Color.Empty to take the default depending on the pixel format</param>
        /// <param name="horizontalResolution"></param>
        /// <param name="verticalResolution"></param>
        /// <returns>Bitmap</returns>
        public static Bitmap CreateEmpty(int width, int height, PixelFormat format, Color backgroundColor, float horizontalResolution, float verticalResolution)
        {
            // Create a new "clean" image
            Bitmap newImage = new Bitmap(width, height, format);
            newImage.SetResolution(horizontalResolution, verticalResolution);
            if (format != PixelFormat.Format8bppIndexed)
            {
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    // Make sure the background color is what we want (transparent or white, depending on the pixel format)
                    if (!Color.Empty.Equals(backgroundColor))
                    {
                        graphics.Clear(backgroundColor);
                    }
                    else if (Image.IsAlphaPixelFormat(format))
                    {
                        graphics.Clear(Color.Transparent);
                    }
                    else
                    {
                        graphics.Clear(Color.White);
                    }
                }
            }
            return newImage;
        }

        /// <summary>
        /// Get a scaled version of the sourceBitmap
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="percent">1-99 to make smaller, use 101 and more to make the picture bigger</param>
        /// <returns></returns>
        public static Bitmap ScaleByPercent(Bitmap sourceBitmap, int percent)
        {
            float nPercent = ((float)percent / 100);

            int sourceWidth = sourceBitmap.Width;
            int sourceHeight = sourceBitmap.Height;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap scaledBitmap = CreateEmpty(destWidth, destHeight, sourceBitmap.PixelFormat, Color.Empty, sourceBitmap.HorizontalResolution, sourceBitmap.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(scaledBitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(sourceBitmap, new Rectangle(0, 0, destWidth, destHeight), new Rectangle(0, 0, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
            }
            return scaledBitmap;
        }

        /// <summary>
        /// Resize canvas with pixel to the left, right, top and bottom
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="backgroundColor">The color to fill with, or Color.Empty to take the default depending on the pixel format</param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <returns>a new bitmap with the source copied on it</returns>
        public static Image ResizeCanvas(Image sourceImage, Color backgroundColor, int left, int right, int top, int bottom)
        {
            Bitmap newBitmap = CreateEmpty(sourceImage.Width + left + right, sourceImage.Height + top + bottom, sourceImage.PixelFormat, backgroundColor, sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(newBitmap))
            {
                graphics.DrawImageUnscaled(sourceImage, left, top);
            }
            return newBitmap;
        }

        /// <summary>
        /// Wrapper for the more complex Resize, this resize could be used for e.g. Thumbnails
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="maintainAspectRatio">true to maintain the aspect ratio</param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static Image ResizeImage(Image sourceImage, bool maintainAspectRatio, int newWidth, int newHeight)
        {
            Point throwAway;
            return ResizeImage(sourceImage, maintainAspectRatio, false, Color.Empty, newWidth, newHeight, out throwAway);
        }

        /// <summary>
        /// Count how many times the supplied color exists
        /// </summary>
        /// <param name="sourceImage">Image to count the pixels of</param>
        /// <param name="colorToCount">Color to count/param>
        /// <param name="includeAlpha">true if Alpha needs to be checked</param>
        /// <returns>int with the number of pixels which have colorToCount</returns>
        public static int CountColor(Image sourceImage, Color colorToCount, bool includeAlpha)
        {
            int colors = 0;
            int toCount = colorToCount.ToArgb();
            if (!includeAlpha)
            {
                toCount = toCount & 0xffffff;
            }
            using (IFastBitmap bb = FastBitmap.Create((Bitmap)sourceImage))
            {
                for (int y = 0; y < bb.Height; y++)
                {
                    for (int x = 0; x < bb.Width; x++)
                    {
                        int bitmapcolor = bb.GetColorAt(x, y).ToArgb();
                        if (!includeAlpha)
                        {
                            bitmapcolor = bitmapcolor & 0xffffff;
                        }
                        if (bitmapcolor == toCount)
                        {
                            colors++;
                        }
                    }
                }
                return colors;
            }
        }

        /// <summary>
        /// Scale the bitmap, keeping aspect ratio, but the canvas will always have the specified size.
        /// </summary>
        /// <param name="sourceImage">Image to scale</param>
        /// <param name="maintainAspectRatio">true to maintain the aspect ratio</param>
        /// <param name="backgroundColor">The color to fill with, or Color.Empty to take the default depending on the pixel format</param>
        /// <param name="newWidth">new width</param>
        /// <param name="newHeight">new height</param>
        /// <returns>a new bitmap with the specified size, the source-Image scaled to fit with aspect ratio locked</returns>
        public static Image ResizeImage(Image sourceImage, bool maintainAspectRatio, bool canvasUseNewSize, Color backgroundColor, int newWidth, int newHeight, out Point offset)
        {
            int destX = 0;
            int destY = 0;

            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)newWidth / (float)sourceImage.Width);
            nPercentH = ((float)newHeight / (float)sourceImage.Height);
            if (maintainAspectRatio)
            {
                if (nPercentW == 1)
                {
                    nPercentW = nPercentH;
                    if (canvasUseNewSize)
                    {
                        destX = Math.Max(0, Convert.ToInt32((newWidth - (sourceImage.Width * nPercentW)) / 2));
                    }
                }
                else if (nPercentH == 1)
                {
                    nPercentH = nPercentW;
                    if (canvasUseNewSize)
                    {
                        destY = Math.Max(0, Convert.ToInt32((newHeight - (sourceImage.Height * nPercentH)) / 2));
                    }
                }
                else if (nPercentH != 0 && nPercentH < nPercentW)
                {
                    nPercentW = nPercentH;
                    if (canvasUseNewSize)
                    {
                        destX = Math.Max(0, Convert.ToInt32((newWidth - (sourceImage.Width * nPercentW)) / 2));
                    }
                }
                else
                {
                    nPercentH = nPercentW;
                    if (canvasUseNewSize)
                    {
                        destY = Math.Max(0, Convert.ToInt32((newHeight - (sourceImage.Height * nPercentH)) / 2));
                    }
                }
            }

            offset = new Point(destX, destY);

            int destWidth = (int)(sourceImage.Width * nPercentW);
            int destHeight = (int)(sourceImage.Height * nPercentH);
            if (newWidth == 0)
            {
                newWidth = destWidth;
            }
            if (newHeight == 0)
            {
                newHeight = destHeight;
            }
            Image newImage = null;
            if (maintainAspectRatio && canvasUseNewSize)
            {
                newImage = CreateEmpty(newWidth, newHeight, sourceImage.PixelFormat, backgroundColor, sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            }
            else
            {
                newImage = CreateEmpty(destWidth, destHeight, sourceImage.PixelFormat, backgroundColor, sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            }

            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(sourceImage, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), GraphicsUnit.Pixel);
            }
            return newImage;
        }
    }
}