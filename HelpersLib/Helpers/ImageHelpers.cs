#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

#region License Information (Greenshot)

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

#endregion License Information (Greenshot)

using Greenshot;
using Greenshot.Drawing;
using Greenshot.IniFile;
using Greenshot.Plugin;
using GreenshotPlugin.Core;
using GreenshotPlugin.UnmanagedHelpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HelpersLib
{
    public static class ImageHelpers
    {
        public static Image ResizeImage(Image img, int width, int height)
        {
            return ResizeImage(img, 0, 0, width, height);
        }

        public static Image ResizeImage(Image img, Size size)
        {
            return ResizeImage(img, 0, 0, size.Width, size.Height);
        }

        public static Image ResizeImage(Image img, int x, int y, int width, int height)
        {
            if (width < 1 || height < 1 || (img.Width == width && img.Height == height))
            {
                return img;
            }

            Bitmap bmp = new Bitmap(x + width, y + height, img.PixelFormat);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bmp))
            using (img)
            {
                g.SetHighQuality();
                g.DrawImage(img, x, y, width, height);
            }

            return bmp;
        }

        public static Image ResizeImageByPercentage(Image img, float percentage)
        {
            return ResizeImageByPercentage(img, percentage, percentage);
        }

        public static Image ResizeImageByPercentage(Image img, float percentageWidth, float percentageHeight)
        {
            int width = (int)(percentageWidth / 100 * img.Width);
            int height = (int)(percentageHeight / 100 * img.Height);
            return ResizeImage(img, width, height);
        }

        public static Image ResizeImage(Image img, int width, int height, bool allowEnlarge, bool centerImage)
        {
            return ResizeImage(img, 0, 0, width, height, allowEnlarge, centerImage);
        }

        public static Image ResizeImage(Image img, Rectangle rect, bool allowEnlarge, bool centerImage)
        {
            return ResizeImage(img, rect.X, rect.Y, rect.Width, rect.Height, allowEnlarge, centerImage);
        }

        public static Image ResizeImage(Image img, int x, int y, int width, int height, bool allowEnlarge, bool centerImage)
        {
            double ratio;
            int newWidth, newHeight;

            if (!allowEnlarge && img.Width <= width && img.Height <= height)
            {
                ratio = 1.0;
                newWidth = img.Width;
                newHeight = img.Height;
            }
            else
            {
                double ratioX = (double)width / img.Width;
                double ratioY = (double)height / img.Height;
                ratio = ratioX < ratioY ? ratioX : ratioY;
                newWidth = (int)(img.Width * ratio);
                newHeight = (int)(img.Height * ratio);
            }

            int newX = x;
            int newY = y;

            if (centerImage)
            {
                newX += (int)((width - (img.Width * ratio)) / 2);
                newY += (int)((height - (img.Height * ratio)) / 2);
            }

            return ResizeImage(img, newX, newY, newWidth, newHeight);
        }

        public static Image CropImage(Image img, Rectangle rect)
        {
            if (img != null && rect.X >= 0 && rect.Y >= 0 && rect.Width > 0 && rect.Height > 0 &&
                new Rectangle(0, 0, img.Width, img.Height).Contains(rect))
            {
                using (Bitmap bmp = new Bitmap(img))
                {
                    return bmp.Clone(rect, bmp.PixelFormat);
                }
            }

            return null;
        }

        public static Bitmap CropBitmap(Bitmap bmp, Rectangle rect)
        {
            if (bmp != null && rect.X >= 0 && rect.Y >= 0 && rect.Width > 0 && rect.Height > 0 &&
                new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(rect))
            {
                return bmp.Clone(rect, bmp.PixelFormat);
            }

            return null;
        }

        public static Image CropImage(Image img, Rectangle rect, GraphicsPath gp)
        {
            if (img != null && rect.Width > 0 && rect.Height > 0 && gp != null)
            {
                Bitmap bmp = new Bitmap(rect.Width, rect.Height);
                bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SetHighQuality();

                    using (Region region = new Region(gp))
                    {
                        g.Clip = region;
                        g.DrawImage(img, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
                    }
                }

                return bmp;
            }

            return null;
        }

        public static Image DrawBorder(Image img, BorderType borderType, Color borderColor, int borderSize)
        {
            Bitmap bmp;

            using (Pen borderPen = new Pen(borderColor, borderSize) { Alignment = PenAlignment.Inset })
            {
                if (borderType == BorderType.Inside)
                {
                    bmp = (Bitmap)img;

                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawRectangleProper(borderPen, new Rectangle(0, 0, img.Width, img.Height));
                    }
                }
                else
                {
                    bmp = img.CreateEmptyBitmap(borderSize * 2, borderSize * 2);

                    using (Graphics g = Graphics.FromImage(bmp))
                    using (img)
                    {
                        g.SetHighQuality();
                        g.DrawImage(img, borderSize, borderSize, img.Width, img.Height);
                        g.DrawRectangleProper(borderPen, new Rectangle(0, 0, img.Width + borderSize * 2, img.Height + borderSize * 2));
                    }
                }
            }

            return bmp;
        }

        public static Image DrawOutline(Image img, GraphicsPath gp)
        {
            if (img != null && gp != null)
            {
                Bitmap bmp = new Bitmap(img);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    gp.WindingModeOutline();
                    g.DrawPath(Pens.Black, gp);
                }

                return bmp;
            }

            return null;
        }

        public static Bitmap AddSkew(Image img, int x, int y)
        {
            Bitmap result = img.CreateEmptyBitmap(Math.Abs(x), Math.Abs(y));

            using (Graphics g = Graphics.FromImage(result))
            using (img)
            {
                g.SetHighQuality();
                int startX = -Math.Min(0, x);
                int startY = -Math.Min(0, y);
                int endX = Math.Max(0, x);
                int endY = Math.Max(0, y);
                Point[] destinationPoints = { new Point(startX, startY), new Point(startX + img.Width - 1, endY), new Point(endX, startY + img.Height - 1) };
                g.DrawImage(img, destinationPoints);
            }

            return result;
        }

        public static Image AddCanvas(Image img, int width, int height)
        {
            Bitmap bmp = img.CreateEmptyBitmap(width * 2, height * 2);

            using (Graphics g = Graphics.FromImage(bmp))
            using (img)
            {
                g.SetHighQuality();
                g.DrawImage(img, width, height, img.Width, img.Height);
            }

            return bmp;
        }

        public static Image DrawReflection(Image img, int percentage, int maxAlpha, int minAlpha, int offset, bool skew, int skewSize)
        {
            Bitmap reflection = AddReflection(img, percentage, maxAlpha, minAlpha);

            if (skew)
            {
                reflection = AddSkew(reflection, skewSize, 0);
            }

            Bitmap result = new Bitmap(reflection.Width, img.Height + reflection.Height + offset);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.SetHighQuality();
                g.DrawImage(img, 0, 0, img.Width, img.Height);
                g.DrawImage(reflection, 0, img.Height + offset, reflection.Width, reflection.Height);
                img.Dispose();
            }

            return result;
        }

        public static Bitmap AddReflection(Image img, int percentage, int maxAlpha, int minAlpha)
        {
            percentage = percentage.Between(1, 100);
            maxAlpha = maxAlpha.Between(0, 255);
            minAlpha = minAlpha.Between(0, 255);

            Bitmap reflection;

            using (Bitmap bitmapRotate = (Bitmap)img.Clone())
            {
                bitmapRotate.RotateFlip(RotateFlipType.RotateNoneFlipY);
                reflection = bitmapRotate.Clone(new Rectangle(0, 0, bitmapRotate.Width, (int)(bitmapRotate.Height * ((float)percentage / 100))), PixelFormat.Format32bppArgb);
            }

            using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(reflection, true))
            {
                int alphaAdd = maxAlpha - minAlpha;
                float reflectionHeight = reflection.Height - 1;

                for (int y = 0; y < reflection.Height; ++y)
                {
                    for (int x = 0; x < reflection.Width; ++x)
                    {
                        ColorBgra color = unsafeBitmap.GetPixel(x, y);
                        byte alpha = (byte)(maxAlpha - (alphaAdd * (y / reflectionHeight)));

                        if (color.Alpha > alpha)
                        {
                            color.Alpha = alpha;
                            unsafeBitmap.SetPixel(x, y, color);
                        }
                    }
                }
            }

            return reflection;
        }

        public static Bitmap FillImageBackground(Image img, Color color)
        {
            Bitmap result = img.CreateEmptyBitmap();

            using (Graphics g = Graphics.FromImage(result))
            using (img)
            {
                g.Clear(color);
                g.SetHighQuality();
                g.DrawImage(img, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        public static Bitmap FillImageBackground(Image img, Color fromColor, Color toColor, LinearGradientMode gradientType)
        {
            Bitmap result = img.CreateEmptyBitmap();

            using (Graphics g = Graphics.FromImage(result))
            using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, result.Width, result.Height), fromColor, toColor, gradientType))
            using (img)
            {
                g.FillRectangle(brush, 0, 0, result.Width, result.Height);
                g.SetHighQuality();
                g.DrawImage(img, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        public static Image CreateCheckers(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            using (Image checker = CreateCheckers(8, Color.LightGray, Color.White))
            using (Brush checkerBrush = new TextureBrush(checker, WrapMode.Tile))
            {
                g.SetHighQuality();
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }

            return bmp;
        }

        public static Image DrawCheckers(Image img)
        {
            Bitmap bmp = img.CreateEmptyBitmap();

            using (Graphics g = Graphics.FromImage(bmp))
            using (Image checker = CreateCheckers(8, Color.LightGray, Color.White))
            using (Brush checkerBrush = new TextureBrush(checker, WrapMode.Tile))
            using (img)
            {
                g.SetHighQuality();
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
                g.DrawImage(img, 0, 0, img.Width, img.Height);
            }

            return bmp;
        }

        public static Image CreateCheckers(int size, Color color1, Color color2)
        {
            Bitmap bmp = new Bitmap(size * 2, size * 2);

            using (Graphics g = Graphics.FromImage(bmp))
            using (Brush brush1 = new SolidBrush(color1))
            using (Brush brush2 = new SolidBrush(color2))
            {
                g.FillRectangle(brush1, 0, 0, size, size);
                g.FillRectangle(brush1, size, size, size, size);
                g.FillRectangle(brush2, size, 0, size, size);
                g.FillRectangle(brush2, 0, size, size, size);
            }

            return bmp;
        }

        public static void DrawTextWithOutline(Graphics g, string text, PointF position, Font font, Color textColor, Color borderColor, int shadowOffset = 1)
        {
            SmoothingMode tempMode = g.SmoothingMode;

            g.SmoothingMode = SmoothingMode.HighQuality;

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddString(text, font.FontFamily, (int)font.Style, font.Size, position, new StringFormat());

                using (Pen borderPen = new Pen(borderColor, 2) { LineJoin = LineJoin.Round })
                {
                    g.DrawPath(borderPen, gp);
                }

                using (Brush textBrush = new SolidBrush(textColor))
                {
                    g.FillPath(textBrush, gp);
                }
            }

            g.SmoothingMode = tempMode;
        }

        public static void DrawTextWithShadow(Graphics g, string text, PointF position, Font font, Color textColor, Color shadowColor, int shadowOffset = 1)
        {
            using (Brush shadowBrush = new SolidBrush(shadowColor))
            {
                g.DrawString(text, font, shadowBrush, position.X + shadowOffset, position.Y + shadowOffset);
            }

            using (Brush textBrush = new SolidBrush(textColor))
            {
                g.DrawString(text, font, textBrush, position.X, position.Y);
            }
        }

        public static bool IsImagesEqual(Bitmap bmp1, Bitmap bmp2)
        {
            using (UnsafeBitmap unsafeBitmap1 = new UnsafeBitmap(bmp1))
            using (UnsafeBitmap unsafeBitmap2 = new UnsafeBitmap(bmp2))
            {
                return unsafeBitmap1 == unsafeBitmap2;
            }
        }

        public static bool AddMetadata(Image img, int id, string text)
        {
            PropertyItem pi;

            try
            {
                // TODO: Need better solution
                pi = (PropertyItem)typeof(PropertyItem).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { }, null).Invoke(null);
                pi.Id = id;
                pi.Len = text.Length + 1;
                pi.Type = 2;
                byte[] bytesText = Encoding.ASCII.GetBytes(text + " ");
                bytesText[bytesText.Length - 1] = 0;
                pi.Value = bytesText;

                if (pi != null)
                {
                    img.SetPropertyItem(pi);
                    return true;
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "Reflection fail");
            }

            return false;
        }

        public static void CopyMetadata(Image fromImage, Image toImage)
        {
            PropertyItem[] propertyItems = fromImage.PropertyItems;
            foreach (PropertyItem pi in propertyItems)
            {
                try
                {
                    toImage.SetPropertyItem(pi);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }
        }

        /// <summary>
        /// Method to rotate an Image object. The result can be one of three cases:
        /// - upsizeOk = true: output image will be larger than the input and no clipping occurs
        /// - upsizeOk = false & clipOk = true: output same size as input, clipping occurs
        /// - upsizeOk = false & clipOk = false: output same size as input, image reduced, no clipping
        ///
        /// Note that this method always returns a new Bitmap object, even if rotation is zero - in
        /// which case the returned object is a clone of the input object.
        /// </summary>
        /// <param name="inputImage">input Image object, is not modified</param>
        /// <param name="angleDegrees">angle of rotation, in degrees</param>
        /// <param name="upsize">see comments above</param>
        /// <param name="clip">see comments above, not used if upsizeOk = true</param>
        /// <returns>new Bitmap object, may be larger than input image</returns>
        public static Bitmap RotateImage(Image inputImage, float angleDegrees, bool upsize, bool clip)
        {
            // Test for zero rotation and return a clone of the input image
            if (angleDegrees == 0f)
                return (Bitmap)inputImage.Clone();

            // Set up old and new image dimensions, assuming upsizing not wanted and clipping OK
            int oldWidth = inputImage.Width;
            int oldHeight = inputImage.Height;
            int newWidth = oldWidth;
            int newHeight = oldHeight;
            float scaleFactor = 1f;

            // If upsizing wanted or clipping not OK calculate the size of the resulting bitmap
            if (upsize || !clip)
            {
                double angleRadians = angleDegrees * Math.PI / 180d;

                double cos = Math.Abs(Math.Cos(angleRadians));
                double sin = Math.Abs(Math.Sin(angleRadians));
                newWidth = (int)Math.Round(oldWidth * cos + oldHeight * sin);
                newHeight = (int)Math.Round(oldWidth * sin + oldHeight * cos);
            }

            // If upsizing not wanted and clipping not OK need a scaling factor
            if (!upsize && !clip)
            {
                scaleFactor = Math.Min((float)oldWidth / newWidth, (float)oldHeight / newHeight);
                newWidth = oldWidth;
                newHeight = oldHeight;
            }

            // Create the new bitmap object.
            Bitmap newBitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
            newBitmap.SetResolution(inputImage.HorizontalResolution, inputImage.VerticalResolution);

            // Create the Graphics object that does the work
            using (Graphics graphicsObject = Graphics.FromImage(newBitmap))
            {
                graphicsObject.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsObject.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphicsObject.SmoothingMode = SmoothingMode.HighQuality;

                // Set up the built-in transformation matrix to do the rotation and maybe scaling
                graphicsObject.TranslateTransform(newWidth / 2f, newHeight / 2f);

                if (scaleFactor != 1f)
                    graphicsObject.ScaleTransform(scaleFactor, scaleFactor);

                graphicsObject.RotateTransform(angleDegrees);
                graphicsObject.TranslateTransform(-oldWidth / 2f, -oldHeight / 2f);

                // Draw the result
                graphicsObject.DrawImage(inputImage, 0, 0, inputImage.Width, inputImage.Height);
            }

            return newBitmap;
        }

        public static Image AnnotateImage(Image img)
        {
            return AnnotateImage(img, false, null, null, null);
        }

        public static Image AnnotateImage(Image img, bool allowSave, string configPath, Action<Image> clipboardCopyRequested, Action<Image> imageUploadRequested)
        {
            if (!IniConfig.isInitialized)
            {
                IniConfig.AllowSave = allowSave;
                IniConfig.Init(configPath);
            }

            using (Image cloneImage = (Image)img.Clone())
            using (ICapture capture = new Capture { Image = cloneImage })
            using (Surface surface = new Surface(capture))
            using (ImageEditorForm editor = new ImageEditorForm(surface, true))
            {
                editor.ClipboardCopyRequested += clipboardCopyRequested;
                editor.ImageUploadRequested += imageUploadRequested;

                if (editor.ShowDialog() == DialogResult.OK)
                {
                    using (img)
                    {
                        return editor.GetImageForExport();
                    }
                }
            }

            return img;
        }

        public static Bitmap AddShadow(Image sourceImage, float opacity, int size)
        {
            return AddShadow(sourceImage, opacity, size, 1, Color.Black, new Point(0, 0));
        }

        public static Bitmap AddShadow(Image sourceImage, float opacity, int size, float darkness, Color color, Point offset)
        {
            Image shadowImage = null;

            try
            {
                shadowImage = sourceImage.CreateEmptyBitmap(size * 2, size * 2);

                ColorMatrix maskMatrix = new ColorMatrix();
                maskMatrix.Matrix00 = 0;
                maskMatrix.Matrix11 = 0;
                maskMatrix.Matrix22 = 0;
                maskMatrix.Matrix33 = opacity;
                maskMatrix.Matrix40 = ((float)color.R).Remap(0, 255, 0, 1);
                maskMatrix.Matrix41 = ((float)color.G).Remap(0, 255, 0, 1);
                maskMatrix.Matrix42 = ((float)color.B).Remap(0, 255, 0, 1);

                Rectangle shadowRectangle = new Rectangle(size, size, sourceImage.Width, sourceImage.Height);
                maskMatrix.Apply(sourceImage, shadowImage, shadowRectangle);

                if (size > 0)
                {
                    Blur((Bitmap)shadowImage, size);
                }

                if (darkness > 1)
                {
                    ColorMatrix alphaMatrix = new ColorMatrix();
                    alphaMatrix.Matrix33 = darkness;

                    Image shadowImage2 = alphaMatrix.Apply(shadowImage);
                    shadowImage.Dispose();
                    shadowImage = shadowImage2;
                }

                Bitmap result = shadowImage.CreateEmptyBitmap(Math.Abs(offset.X), Math.Abs(offset.Y));

                using (Graphics g = Graphics.FromImage(result))
                {
                    g.SetHighQuality();
                    g.DrawImage(shadowImage, Math.Max(0, offset.X), Math.Max(0, offset.Y), shadowImage.Width, shadowImage.Height);
                    g.DrawImage(sourceImage, Math.Max(size, -offset.X + size), Math.Max(size, -offset.Y + size), sourceImage.Width, sourceImage.Height);
                }

                return result;
            }
            finally
            {
                if (sourceImage != null) sourceImage.Dispose();
                if (shadowImage != null) shadowImage.Dispose();
            }
        }

        public static void Blur(Bitmap sourceImage, int radius)
        {
            if (GDIplus.IsBlurPossible(radius))
            {
                GDIplus.ApplyBlur(sourceImage, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), radius, false);
            }
            else
            {
                ImageHelper.ApplyBoxBlur(sourceImage, radius);
            }
        }

        public static Bitmap Pixelate(Bitmap sourceImage, int pixelSize)
        {
            pixelSize = Math.Min(pixelSize, sourceImage.Width);
            pixelSize = Math.Min(pixelSize, sourceImage.Height);

            Bitmap result = sourceImage.CreateEmptyBitmap();

            using (IFastBitmap src = FastBitmap.Create(sourceImage, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height)))
            using (IFastBitmap dest = FastBitmap.Create(result))
            {
                List<Color> colors = new List<Color>();
                int halbPixelSize = pixelSize / 2;
                for (int y = src.Top - halbPixelSize; y < src.Bottom + halbPixelSize; y = y + pixelSize)
                {
                    for (int x = src.Left - halbPixelSize; x <= src.Right + halbPixelSize; x = x + pixelSize)
                    {
                        colors.Clear();
                        for (int yy = y; yy < y + pixelSize; yy++)
                        {
                            if (yy >= src.Top && yy < src.Bottom)
                            {
                                for (int xx = x; xx < x + pixelSize; xx++)
                                {
                                    if (xx >= src.Left && xx < src.Right)
                                    {
                                        colors.Add(src.GetColorAt(xx, yy));
                                    }
                                }
                            }
                        }
                        Color currentAvgColor = ColorHelpers.Mix(colors);
                        for (int yy = y; yy <= y + pixelSize; yy++)
                        {
                            if (yy >= src.Top && yy < src.Bottom)
                            {
                                for (int xx = x; xx <= x + pixelSize; xx++)
                                {
                                    if (xx >= src.Left && xx < src.Right)
                                    {
                                        dest.SetColorAt(xx, yy, currentAvgColor);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static Image CreateTornEdge(Image sourceImage, int toothHeight, int horizontalToothRange, int verticalToothRange)
        {
            Image result = sourceImage.CreateEmptyBitmap();

            using (GraphicsPath path = new GraphicsPath())
            {
                Random random = new Random();
                int horizontalRegions = sourceImage.Width / horizontalToothRange;
                int verticalRegions = sourceImage.Height / verticalToothRange;

                // Start
                Point previousEndingPoint = new Point(horizontalToothRange, random.Next(1, toothHeight));
                Point newEndingPoint;

                // Top
                for (int i = 0; i < horizontalRegions; i++)
                {
                    int x = previousEndingPoint.X + horizontalToothRange;
                    int y = random.Next(1, toothHeight);
                    newEndingPoint = new Point(x, y);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }

                // Right
                for (int i = 0; i < verticalRegions; i++)
                {
                    int x = sourceImage.Width - random.Next(1, toothHeight);
                    int y = previousEndingPoint.Y + verticalToothRange;
                    newEndingPoint = new Point(x, y);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }

                // Bottom
                for (int i = 0; i < horizontalRegions; i++)
                {
                    int x = previousEndingPoint.X - horizontalToothRange;
                    int y = sourceImage.Height - random.Next(1, toothHeight);
                    newEndingPoint = new Point(x, y);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }

                // Left
                for (int i = 0; i < verticalRegions; i++)
                {
                    int x = random.Next(1, toothHeight);
                    int y = previousEndingPoint.Y - verticalToothRange;
                    newEndingPoint = new Point(x, y);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }
                path.CloseFigure();

                // Draw the created figure with the original image by using a TextureBrush so we have anti-aliasing
                using (Graphics graphics = Graphics.FromImage(result))
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

            return result;
        }

        public static Bitmap Sharpen(Image image, double strength)
        {
            using (Bitmap bitmap = (Bitmap)image)
            {
                if (bitmap != null)
                {
                    Bitmap sharpenImage = bitmap.Clone() as Bitmap;

                    int width = image.Width;
                    int height = image.Height;

                    // Create sharpening filter.
                    const int filterSize = 5;

                    var filter = new double[,]
                    {
                        {-1, -1, -1, -1, -1},
                        {-1,  2,  2,  2, -1},
                        {-1,  2, 16,  2, -1},
                        {-1,  2,  2,  2, -1},
                        {-1, -1, -1, -1, -1}
                    };

                    double bias = 1.0 - strength;
                    double factor = strength / 16.0;

                    const int s = filterSize / 2;

                    var result = new Color[image.Width, image.Height];

                    // Lock image bits for read/write.
                    if (sharpenImage != null)
                    {
                        BitmapData pbits = sharpenImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                        // Declare an array to hold the bytes of the bitmap.
                        int bytes = pbits.Stride * height;
                        var rgbValues = new byte[bytes];

                        // Copy the RGB values into the array.
                        Marshal.Copy(pbits.Scan0, rgbValues, 0, bytes);

                        int rgb;
                        // Fill the color array with the new sharpened color values.
                        for (int x = s; x < width - s; x++)
                        {
                            for (int y = s; y < height - s; y++)
                            {
                                double red = 0.0, green = 0.0, blue = 0.0;

                                for (int filterX = 0; filterX < filterSize; filterX++)
                                {
                                    for (int filterY = 0; filterY < filterSize; filterY++)
                                    {
                                        int imageX = (x - s + filterX + width) % width;
                                        int imageY = (y - s + filterY + height) % height;

                                        rgb = imageY * pbits.Stride + 3 * imageX;

                                        red += rgbValues[rgb + 2] * filter[filterX, filterY];
                                        green += rgbValues[rgb + 1] * filter[filterX, filterY];
                                        blue += rgbValues[rgb + 0] * filter[filterX, filterY];
                                    }

                                    rgb = y * pbits.Stride + 3 * x;

                                    int r = Math.Min(Math.Max((int)(factor * red + (bias * rgbValues[rgb + 2])), 0), 255);
                                    int g = Math.Min(Math.Max((int)(factor * green + (bias * rgbValues[rgb + 1])), 0), 255);
                                    int b = Math.Min(Math.Max((int)(factor * blue + (bias * rgbValues[rgb + 0])), 0), 255);

                                    result[x, y] = Color.FromArgb(r, g, b);
                                }
                            }
                        }

                        // Update the image with the sharpened pixels.
                        for (int x = s; x < width - s; x++)
                        {
                            for (int y = s; y < height - s; y++)
                            {
                                rgb = y * pbits.Stride + 3 * x;

                                rgbValues[rgb + 2] = result[x, y].R;
                                rgbValues[rgb + 1] = result[x, y].G;
                                rgbValues[rgb + 0] = result[x, y].B;
                            }
                        }

                        // Copy the RGB values back to the bitmap.
                        Marshal.Copy(rgbValues, 0, pbits.Scan0, bytes);
                        // Release image bits.
                        sharpenImage.UnlockBits(pbits);
                    }

                    return sharpenImage;
                }
            }
            return null;
        }
    }
}