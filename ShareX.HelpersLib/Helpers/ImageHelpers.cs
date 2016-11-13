#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class ImageHelpers
    {
        public static Image ResizeImage(Image img, Size size)
        {
            return ResizeImage(img, size.Width, size.Height);
        }

        public static Image ResizeImage(Image img, int width, int height)
        {
            if (width < 1 || height < 1 || (img.Width == width && img.Height == height))
            {
                return img;
            }

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            using (img)
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.CompositingMode = CompositingMode.SourceOver;

                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(img, new Rectangle(0, 0, width, height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
                }
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

        public static Image ResizeImage(Image img, Size size, bool allowEnlarge, bool centerImage = true)
        {
            return ResizeImage(img, size.Width, size.Height, allowEnlarge, centerImage);
        }

        public static Image ResizeImage(Image img, int width, int height, bool allowEnlarge, bool centerImage = true)
        {
            return ResizeImage(img, width, height, allowEnlarge, centerImage, Color.Transparent);
        }

        public static Image ResizeImage(Image img, int width, int height, bool allowEnlarge, bool centerImage, Color backColor)
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

            int newX = 0;
            int newY = 0;

            if (centerImage)
            {
                newX += (int)((width - (img.Width * ratio)) / 2);
                newY += (int)((height - (img.Height * ratio)) / 2);
            }

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bmp))
            using (img)
            {
                g.Clear(backColor);
                g.SetHighQuality();
                g.DrawImage(img, newX, newY, newWidth, newHeight);
            }

            return bmp;
        }

        public static Image ResizeImageLimit(Image img, Size size)
        {
            return ResizeImageLimit(img, size.Width, size.Height);
        }

        /// <summary>If image size bigger than "size" then resize it and keep aspect ratio else return image.</summary>
        public static Image ResizeImageLimit(Image img, int width, int height)
        {
            if (img.Width <= width && img.Height <= height)
            {
                return img;
            }

            int newWidth, newHeight;
            double ratioX = (double)width / img.Width;
            double ratioY = (double)height / img.Height;

            if (ratioX < ratioY)
            {
                newWidth = width;
                newHeight = (int)(img.Height * ratioX);
            }
            else
            {
                newWidth = (int)(img.Width * ratioY);
                newHeight = height;
            }

            return ResizeImage(img, newWidth, newHeight);
        }

        public static Image ResizeImageLimit(Image img, int maxSize)
        {
            double ratio = (double)img.Width / img.Height;
            double x = Math.Sqrt(maxSize / ratio);

            int width, height;
            if (ratio > 1)
            {
                width = (int)(ratio * x);
                height = (int)(width / ratio);
            }
            else
            {
                height = (int)(ratio * x);
                width = (int)(height / ratio);
            }

            return ResizeImage(img, width, height);
        }

        public static Image CropImage(Image img, Rectangle rect)
        {
            if (img != null && rect.X >= 0 && rect.Y >= 0 && rect.Width > 0 && rect.Height > 0 && new Rectangle(0, 0, img.Width, img.Height).Contains(rect))
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
            if (bmp != null && rect.X >= 0 && rect.Y >= 0 && rect.Width > 0 && rect.Height > 0 && new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(rect))
            {
                return bmp.Clone(rect, bmp.PixelFormat);
            }

            return null;
        }

        /// <summary>Adds empty space around image.</summary>
        public static Image AddCanvas(Image img, Padding margin)
        {
            Bitmap bmp = img.CreateEmptyBitmap(margin.Horizontal, margin.Vertical);

            using (Graphics g = Graphics.FromImage(bmp))
            using (img)
            {
                g.SetHighQuality();
                g.DrawImage(img, margin.Left, margin.Top, img.Width, img.Height);
            }

            return bmp;
        }

        public static Image RoundedCorners(Image img, int cornerRadius)
        {
            Bitmap bmp = img.CreateEmptyBitmap();

            using (Graphics g = Graphics.FromImage(bmp))
            using (img)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddRoundedRectangleProper(new RectangleF(0, 0, img.Width, img.Height), cornerRadius);

                    using (TextureBrush brush = new TextureBrush(img))
                    {
                        g.FillPath(brush, gp);
                    }
                }
            }

            return bmp;
        }

        public static Image Outline(Image img, int borderSize, Color borderColor)
        {
            Bitmap result = img.CreateEmptyBitmap(borderSize * 2, borderSize * 2);

            ColorMatrix maskMatrix = new ColorMatrix();
            maskMatrix.Matrix00 = 0;
            maskMatrix.Matrix11 = 0;
            maskMatrix.Matrix22 = 0;
            maskMatrix.Matrix33 = 1;
            maskMatrix.Matrix40 = ((float)borderColor.R).Remap(0, 255, 0, 1);
            maskMatrix.Matrix41 = ((float)borderColor.G).Remap(0, 255, 0, 1);
            maskMatrix.Matrix42 = ((float)borderColor.B).Remap(0, 255, 0, 1);

            using (img)
            using (Image shadow = maskMatrix.Apply(img))
            using (Graphics g = Graphics.FromImage(result))
            {
                for (int i = 0; i <= borderSize * 2; i++)
                {
                    g.DrawImage(shadow, new Rectangle(i, 0, shadow.Width, shadow.Height));
                    g.DrawImage(shadow, new Rectangle(i, borderSize * 2, shadow.Width, shadow.Height));
                    g.DrawImage(shadow, new Rectangle(0, i, shadow.Width, shadow.Height));
                    g.DrawImage(shadow, new Rectangle(borderSize * 2, i, shadow.Width, shadow.Height));
                }

                g.DrawImage(img, new Rectangle(borderSize, borderSize, img.Width, img.Height));
            }

            return result;
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
            using (img)
            using (reflection)
            {
                g.SetHighQuality();
                g.DrawImage(img, 0, 0, img.Width, img.Height);
                g.DrawImage(reflection, 0, img.Height + offset, reflection.Width, reflection.Height);
            }

            return result;
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

        public static Image DrawBorder(Image img, Color borderColor, int borderSize, BorderType borderType)
        {
            using (Pen borderPen = new Pen(borderColor, borderSize) { Alignment = PenAlignment.Inset })
            {
                return DrawBorder(img, borderPen, borderType);
            }
        }

        public static Image DrawBorder(Image img, Color fromBorderColor, Color toBorderColor, LinearGradientMode gradientType, int borderSize, BorderType borderType)
        {
            int width = img.Width;
            int height = img.Height;

            if (borderType == BorderType.Outside)
            {
                width += borderSize * 2;
                height += borderSize * 2;
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, width, height), fromBorderColor, toBorderColor, gradientType))
            using (Pen borderPen = new Pen(brush, borderSize) { Alignment = PenAlignment.Inset })
            {
                return DrawBorder(img, borderPen, borderType);
            }
        }

        public static Image DrawBorder(Image img, Pen borderPen, BorderType borderType)
        {
            Bitmap bmp;

            if (borderType == BorderType.Inside)
            {
                bmp = (Bitmap)img;

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawRectangleProper(borderPen, 0, 0, img.Width, img.Height);
                }
            }
            else
            {
                int borderSize = (int)borderPen.Width;
                bmp = img.CreateEmptyBitmap(borderSize * 2, borderSize * 2);

                using (Graphics g = Graphics.FromImage(bmp))
                using (img)
                {
                    g.DrawRectangleProper(borderPen, 0, 0, bmp.Width, bmp.Height);
                    g.SetHighQuality();
                    g.DrawImage(img, borderSize, borderSize, img.Width, img.Height);
                }
            }

            return bmp;
        }

        public static Bitmap FillBackground(Image img, Color color)
        {
            using (Brush brush = new SolidBrush(color))
            {
                return FillBackground(img, brush);
            }
        }

        public static Bitmap FillBackground(Image img, Color fromColor, Color toColor, LinearGradientMode gradientType)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, img.Width, img.Height), fromColor, toColor, gradientType))
            {
                return FillBackground(img, brush);
            }
        }

        public static Bitmap FillBackground(Image img, Brush brush)
        {
            Bitmap result = img.CreateEmptyBitmap();

            using (Graphics g = Graphics.FromImage(result))
            using (img)
            {
                g.FillRectangle(brush, 0, 0, result.Width, result.Height);
                g.SetHighQuality();
                g.DrawImage(img, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        public static Image DrawCheckers(Image img)
        {
            return DrawCheckers(img, 10, Color.LightGray, Color.White);
        }

        public static Image DrawCheckers(Image img, int size, Color color1, Color color2)
        {
            Bitmap bmp = img.CreateEmptyBitmap();

            using (Graphics g = Graphics.FromImage(bmp))
            using (Image checker = CreateCheckers(size, color1, color2))
            using (Brush checkerBrush = new TextureBrush(checker, WrapMode.Tile))
            using (img)
            {
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
                g.SetHighQuality();
                g.DrawImage(img, 0, 0, img.Width, img.Height);
            }

            return bmp;
        }

        public static Image DrawCheckers(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            using (Image checker = CreateCheckers())
            using (Brush checkerBrush = new TextureBrush(checker, WrapMode.Tile))
            {
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }

            return bmp;
        }

        public static Image CreateCheckers()
        {
            return CreateCheckers(10, Color.LightGray, Color.White);
        }

        public static Image CreateCheckers(int size, Color color1, Color color2)
        {
            return CreateCheckers(size, size, color1, color2);
        }

        public static Image CreateCheckers(int width, int height, Color color1, Color color2)
        {
            Bitmap bmp = new Bitmap(width * 2, height * 2);

            using (Graphics g = Graphics.FromImage(bmp))
            using (Brush brush1 = new SolidBrush(color1))
            using (Brush brush2 = new SolidBrush(color2))
            {
                g.FillRectangle(brush1, 0, 0, width, height);
                g.FillRectangle(brush1, width, height, width, height);
                g.FillRectangle(brush2, width, 0, width, height);
                g.FillRectangle(brush2, 0, height, width, height);
            }

            return bmp;
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
                DebugHelper.WriteException(e, "Reflection failed.");
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
        /// <param name="img">input Image object, is not modified</param>
        /// <param name="angleDegrees">angle of rotation, in degrees</param>
        /// <param name="upsize">see comments above</param>
        /// <param name="clip">see comments above, not used if upsizeOk = true</param>
        /// <returns>new Bitmap object, may be larger than input image</returns>
        public static Bitmap RotateImage(Image img, float angleDegrees, bool upsize, bool clip)
        {
            // Test for zero rotation and return a clone of the input image
            if (angleDegrees == 0f)
                return (Bitmap)img.Clone();

            // Set up old and new image dimensions, assuming upsizing not wanted and clipping OK
            int oldWidth = img.Width;
            int oldHeight = img.Height;
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
            Bitmap newBitmap = img.CreateEmptyBitmap();

            // Create the Graphics object that does the work
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;

                // Set up the built-in transformation matrix to do the rotation and maybe scaling
                g.TranslateTransform(newWidth / 2f, newHeight / 2f);

                if (scaleFactor != 1f)
                    g.ScaleTransform(scaleFactor, scaleFactor);

                g.RotateTransform(angleDegrees);
                g.TranslateTransform(-oldWidth / 2f, -oldHeight / 2f);

                // Draw the result
                g.DrawImage(img, 0, 0, img.Width, img.Height);
            }

            return newBitmap;
        }

        public static Bitmap AddShadow(Image img, float opacity, int size)
        {
            return AddShadow(img, opacity, size, 1, Color.Black, new Point(0, 0));
        }

        public static Bitmap AddShadow(Image img, float opacity, int size, float darkness, Color color, Point offset)
        {
            Image shadowImage = null;

            try
            {
                shadowImage = img.CreateEmptyBitmap(size * 2, size * 2);

                ColorMatrix maskMatrix = new ColorMatrix();
                maskMatrix.Matrix00 = 0;
                maskMatrix.Matrix11 = 0;
                maskMatrix.Matrix22 = 0;
                maskMatrix.Matrix33 = opacity;
                maskMatrix.Matrix40 = ((float)color.R).Remap(0, 255, 0, 1);
                maskMatrix.Matrix41 = ((float)color.G).Remap(0, 255, 0, 1);
                maskMatrix.Matrix42 = ((float)color.B).Remap(0, 255, 0, 1);

                Rectangle shadowRectangle = new Rectangle(size, size, img.Width, img.Height);
                maskMatrix.Apply(img, shadowImage, shadowRectangle);

                if (size > 0)
                {
                    FastBoxBlur((Bitmap)shadowImage, size);
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
                    g.DrawImage(img, Math.Max(size, -offset.X + size), Math.Max(size, -offset.Y + size), img.Width, img.Height);
                }

                return result;
            }
            finally
            {
                if (img != null) img.Dispose();
                if (shadowImage != null) shadowImage.Dispose();
            }
        }

        public static Bitmap Sharpen(Image img, double strength)
        {
            using (Bitmap bitmap = (Bitmap)img)
            {
                if (bitmap != null)
                {
                    Bitmap sharpenImage = bitmap.Clone() as Bitmap;

                    int width = img.Width;
                    int height = img.Height;

                    // Create sharpening filter.
                    const int filterSize = 5;

                    double[,] filter = new double[,]
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

                    Color[,] result = new Color[img.Width, img.Height];

                    // Lock image bits for read/write.
                    if (sharpenImage != null)
                    {
                        BitmapData pbits = sharpenImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                        // Declare an array to hold the bytes of the bitmap.
                        int bytes = pbits.Stride * height;
                        byte[] rgbValues = new byte[bytes];

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

        public static void HighlightImage(Bitmap bmp)
        {
            HighlightImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
        }

        public static void HighlightImage(Bitmap bmp, Color highlightColor)
        {
            HighlightImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), highlightColor);
        }

        public static void HighlightImage(Bitmap bmp, Rectangle rect)
        {
            HighlightImage(bmp, rect, Color.Yellow);
        }

        public static void HighlightImage(Bitmap bmp, Rectangle rect, Color highlightColor)
        {
            using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true))
            {
                for (int y = rect.Y; y < rect.Height; y++)
                {
                    for (int x = rect.X; x < rect.Width; x++)
                    {
                        ColorBgra color = unsafeBitmap.GetPixel(x, y);
                        color.Red = Math.Min(color.Red, highlightColor.R);
                        color.Green = Math.Min(color.Green, highlightColor.G);
                        color.Blue = Math.Min(color.Blue, highlightColor.B);
                        unsafeBitmap.SetPixel(x, y, color);
                    }
                }
            }
        }

        public static void Pixelate(Bitmap bmp, int pixelSize)
        {
            if (pixelSize > 1)
            {
                using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true))
                {
                    for (int y = 0; y < unsafeBitmap.Height; y += pixelSize)
                    {
                        for (int x = 0; x < unsafeBitmap.Width; x += pixelSize)
                        {
                            int r = 0, g = 0, b = 0, a = 0, count = 0;

                            int yLimit = Math.Min(y + pixelSize, unsafeBitmap.Height);
                            int xLimit = Math.Min(x + pixelSize, unsafeBitmap.Width);

                            for (int y2 = y; y2 < yLimit; y2++)
                            {
                                for (int x2 = x; x2 < xLimit; x2++)
                                {
                                    ColorBgra color = unsafeBitmap.GetPixel(x2, y2);

                                    r += color.Red;
                                    g += color.Green;
                                    b += color.Blue;
                                    a += color.Alpha;
                                    count++;
                                }
                            }

                            ColorBgra averageColor = new ColorBgra((byte)(b / count), (byte)(g / count), (byte)(r / count), (byte)(a / count));

                            for (int y2 = y; y2 < yLimit; y2++)
                            {
                                for (int x2 = x; x2 < xLimit; x2++)
                                {
                                    unsafeBitmap.SetPixel(x2, y2, averageColor);
                                }
                            }
                        }
                    }
                }
            }
        }

        // http://incubator.quasimondo.com/processing/superfast_blur.php
        public static void FastBoxBlur(Bitmap bmp, int radius)
        {
            if (radius < 1) return;

            using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true))
            {
                int w = unsafeBitmap.Width;
                int h = unsafeBitmap.Height;
                int wm = w - 1;
                int hm = h - 1;
                int wh = w * h;
                int div = radius + radius + 1;
                byte[] r = new byte[wh];
                byte[] g = new byte[wh];
                byte[] b = new byte[wh];
                byte[] a = new byte[wh];
                int rsum, gsum, bsum, asum, x, y, i, p, p1, p2, yp, yi, yw;
                int[] vmin = new int[Math.Max(w, h)];
                int[] vmax = new int[Math.Max(w, h)];

                byte[] dv = new byte[256 * div];

                for (i = 0; i < 256 * div; i++)
                {
                    dv[i] = (byte)(i / div);
                }

                yw = yi = 0;

                for (y = 0; y < h; y++)
                {
                    rsum = gsum = bsum = asum = 0;

                    for (i = -radius; i <= radius; i++)
                    {
                        p = (yi + Math.Min(wm, Math.Max(i, 0)));

                        ColorBgra color = unsafeBitmap.GetPixel(p);
                        rsum += color.Red;
                        gsum += color.Green;
                        bsum += color.Blue;
                        asum += color.Alpha;
                    }

                    for (x = 0; x < w; x++)
                    {
                        r[yi] = dv[rsum];
                        g[yi] = dv[gsum];
                        b[yi] = dv[bsum];
                        a[yi] = dv[asum];

                        if (y == 0)
                        {
                            vmin[x] = Math.Min(x + radius + 1, wm);
                            vmax[x] = Math.Max(x - radius, 0);
                        }

                        p1 = (yw + vmin[x]);
                        p2 = (yw + vmax[x]);

                        ColorBgra color1 = unsafeBitmap.GetPixel(p1);
                        ColorBgra color2 = unsafeBitmap.GetPixel(p2);

                        rsum += color1.Red - color2.Red;
                        gsum += color1.Green - color2.Green;
                        bsum += color1.Blue - color2.Blue;
                        asum += color1.Alpha - color2.Alpha;

                        yi++;
                    }

                    yw += w;
                }

                for (x = 0; x < w; x++)
                {
                    rsum = gsum = bsum = asum = 0;
                    yp = -radius * w;

                    for (i = -radius; i <= radius; i++)
                    {
                        yi = Math.Max(0, yp) + x;
                        rsum += r[yi];
                        gsum += g[yi];
                        bsum += b[yi];
                        asum += a[yi];
                        yp += w;
                    }

                    yi = x;

                    for (y = 0; y < h; y++)
                    {
                        ColorBgra color = new ColorBgra(dv[bsum], dv[gsum], dv[rsum], dv[asum]);
                        unsafeBitmap.SetPixel(yi, color);

                        if (x == 0)
                        {
                            vmin[y] = Math.Min(y + radius + 1, hm) * w;
                            vmax[y] = Math.Max(y - radius, 0) * w;
                        }

                        p1 = x + vmin[y];
                        p2 = x + vmax[y];

                        rsum += r[p1] - r[p2];
                        gsum += g[p1] - g[p2];
                        bsum += b[p1] - b[p2];
                        asum += a[p1] - a[p2];

                        yi += w;
                    }
                }
            }
        }

        public static string OpenImageFileDialog()
        {
            string[] images = OpenImageFileDialog(false);

            if (images != null && images.Length > 0)
            {
                return images[0];
            }

            return null;
        }

        public static string[] OpenImageFileDialog(bool multiselect)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image files (*.png, *.jpg, *.jpeg, *.jpe, *.jfif, *.gif, *.bmp, *.tif, *.tiff)|*.png;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.bmp;*.tif;*.tiff|" +
                    "PNG (*.png)|*.png|JPEG (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|GIF (*.gif)|*.gif|BMP (*.bmp)|*.bmp|TIFF (*.tif, *.tiff)|*.tif;*.tiff";

                ofd.Multiselect = multiselect;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return ofd.FileNames;
                }
            }

            return null;
        }

        public static ImageFormat GetImageFormat(string filePath)
        {
            ImageFormat imageFormat = ImageFormat.Png;
            string ext = Helpers.GetFilenameExtension(filePath);

            if (!string.IsNullOrEmpty(ext))
            {
                ext = ext.ToLowerInvariant();

                switch (ext)
                {
                    default:
                    case "png":
                        imageFormat = ImageFormat.Png;
                        break;
                    case "jpg":
                    case "jpeg":
                    case "jpe":
                    case "jfif":
                        imageFormat = ImageFormat.Jpeg;
                        break;
                    case "gif":
                        imageFormat = ImageFormat.Gif;
                        break;
                    case "bmp":
                        imageFormat = ImageFormat.Bmp;
                        break;
                    case "tif":
                    case "tiff":
                        imageFormat = ImageFormat.Tiff;
                        break;
                }
            }

            return imageFormat;
        }

        public static void SaveImage(Image img, string filePath)
        {
            img.Save(filePath, GetImageFormat(filePath));
        }

        public static string SaveImageFileDialog(Image img, string filePath = "")
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    string folder = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(folder))
                    {
                        sfd.InitialDirectory = folder;
                    }
                    sfd.FileName = Path.GetFileNameWithoutExtension(filePath);
                }

                sfd.DefaultExt = "png";
                sfd.Filter = "PNG (*.png)|*.png|JPEG (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|GIF (*.gif)|*.gif|BMP (*.bmp)|*.bmp|TIFF (*.tif, *.tiff)|*.tif;*.tiff";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveImage(img, sfd.FileName);
                    return sfd.FileName;
                }
            }

            return null;
        }

        // http://stackoverflow.com/questions/788335/why-does-image-fromfile-keep-a-file-handle-open-sometimes
        public static Image LoadImage(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && Helpers.IsImageFile(filePath) && File.Exists(filePath))
                {
                    return Image.FromStream(new MemoryStream(File.ReadAllBytes(filePath)));
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return null;
        }

        public static Image CombineImages(IEnumerable<Image> images, Orientation orientation = Orientation.Vertical, int space = 0)
        {
            int width, height;

            int spaceSize = space * (images.Count() - 1);

            if (orientation == Orientation.Vertical)
            {
                width = images.Max(x => x.Width);
                height = images.Sum(x => x.Height) + spaceSize;
            }
            else
            {
                width = images.Sum(x => x.Width) + spaceSize;
                height = images.Max(x => x.Height);
            }

            Bitmap bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SetHighQuality();
                int position = 0;

                foreach (Image image in images)
                {
                    Rectangle rect;

                    if (orientation == Orientation.Vertical)
                    {
                        rect = new Rectangle(0, position, image.Width, image.Height);
                        position += image.Height + space;
                    }
                    else
                    {
                        rect = new Rectangle(position, 0, image.Width, image.Height);
                        position += image.Width + space;
                    }

                    g.DrawImage(image, rect);
                }
            }

            return bmp;
        }

        public static Image CreateColorPickerIcon(Color color, Rectangle rect, int holeSize = 0)
        {
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                DrawColorPickerIcon(g, color, rect, holeSize);
            }

            return bmp;
        }

        public static void DrawColorPickerIcon(Graphics g, Color color, Rectangle rect, int holeSize = 0)
        {
            if (color.A < 255)
            {
                using (Image checker = CreateCheckers(rect.Width / 2, rect.Height / 2, Color.LightGray, Color.White))
                {
                    g.DrawImage(checker, rect);
                }
            }

            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, rect);
            }

            g.DrawRectangleProper(Pens.Black, rect);

            if (holeSize > 0)
            {
                g.CompositingMode = CompositingMode.SourceCopy;

                Rectangle holeRect = new Rectangle(rect.Width / 2 - holeSize / 2, rect.Height / 2 - holeSize / 2, holeSize, holeSize);

                g.FillRectangle(Brushes.Transparent, holeRect);
                g.DrawRectangleProper(Pens.Black, holeRect);
            }
        }

        #region Greenshot methods

        public static Image AnnotateImage(Image img, string imgPath, bool allowSave, string configPath,
            Action<Image> clipboardCopyRequested,
            Action<Image> imageUploadRequested,
            Action<Image, string> imageSaveRequested,
            Func<Image, string, string> imageSaveAsRequested,
            Action<Image> printImageRequested)
        {
            if (!IniConfig.isInitialized)
            {
                IniConfig.AllowSave = allowSave;
                IniConfig.Init(configPath);
            }

            using (Image cloneImage = img != null ? (Image)img.Clone() : LoadImage(imgPath))
            using (ICapture capture = new Capture { Image = cloneImage })
            using (Surface surface = new Surface(capture))
            using (ImageEditorForm editor = new ImageEditorForm(surface, true))
            {
                editor.IsTaskWork = img != null;
                editor.SetImagePath(imgPath);
                editor.ClipboardCopyRequested += clipboardCopyRequested;
                editor.ImageUploadRequested += imageUploadRequested;
                editor.ImageSaveRequested += imageSaveRequested;
                editor.ImageSaveAsRequested += imageSaveAsRequested;
                editor.PrintImageRequested += printImageRequested;

                DialogResult result = editor.ShowDialog();

                if (result == DialogResult.OK && editor.IsTaskWork)
                {
                    using (img)
                    {
                        return editor.GetImageForExport();
                    }
                }

                if (result == DialogResult.Abort)
                {
                    return null;
                }
            }

            return img;
        }

        public static Image CreateTornEdge(Image sourceImage, int toothHeight, int horizontalToothRange, int verticalToothRange, AnchorStyles sides)
        {
            Image result = sourceImage.CreateEmptyBitmap();

            using (GraphicsPath path = new GraphicsPath())
            {
                Random random = new Random();
                int horizontalRegions = sourceImage.Width / horizontalToothRange;
                int verticalRegions = sourceImage.Height / verticalToothRange;

                Point previousEndingPoint = new Point(horizontalToothRange, random.Next(1, toothHeight));
                Point newEndingPoint;

                if (sides.HasFlag(AnchorStyles.Top))
                {
                    for (int i = 0; i < horizontalRegions; i++)
                    {
                        int x = previousEndingPoint.X + horizontalToothRange;
                        int y = random.Next(1, toothHeight);
                        newEndingPoint = new Point(x, y);
                        path.AddLine(previousEndingPoint, newEndingPoint);
                        previousEndingPoint = newEndingPoint;
                    }
                }
                else
                {
                    previousEndingPoint = new Point(0, 0);
                    newEndingPoint = new Point(sourceImage.Width, 0);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }

                if (sides.HasFlag(AnchorStyles.Right))
                {
                    for (int i = 0; i < verticalRegions; i++)
                    {
                        int x = sourceImage.Width - random.Next(1, toothHeight);
                        int y = previousEndingPoint.Y + verticalToothRange;
                        newEndingPoint = new Point(x, y);
                        path.AddLine(previousEndingPoint, newEndingPoint);
                        previousEndingPoint = newEndingPoint;
                    }
                }
                else
                {
                    previousEndingPoint = new Point(sourceImage.Width, 0);
                    newEndingPoint = new Point(sourceImage.Width, sourceImage.Height);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }

                if (sides.HasFlag(AnchorStyles.Bottom))
                {
                    for (int i = 0; i < horizontalRegions; i++)
                    {
                        int x = previousEndingPoint.X - horizontalToothRange;
                        int y = sourceImage.Height - random.Next(1, toothHeight);
                        newEndingPoint = new Point(x, y);
                        path.AddLine(previousEndingPoint, newEndingPoint);
                        previousEndingPoint = newEndingPoint;
                    }
                }
                else
                {
                    previousEndingPoint = new Point(sourceImage.Width, sourceImage.Height);
                    newEndingPoint = new Point(0, sourceImage.Height);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }

                if (sides.HasFlag(AnchorStyles.Left))
                {
                    for (int i = 0; i < verticalRegions; i++)
                    {
                        int x = random.Next(1, toothHeight);
                        int y = previousEndingPoint.Y - verticalToothRange;
                        newEndingPoint = new Point(x, y);
                        path.AddLine(previousEndingPoint, newEndingPoint);
                        previousEndingPoint = newEndingPoint;
                    }
                }
                else
                {
                    previousEndingPoint = new Point(0, sourceImage.Height);
                    newEndingPoint = new Point(0, 0);
                    path.AddLine(previousEndingPoint, newEndingPoint);
                    previousEndingPoint = newEndingPoint;
                }

                path.CloseFigure();

                using (Graphics graphics = Graphics.FromImage(result))
                {
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // Draw the created figure with the original image by using a TextureBrush so we have anti-aliasing
                    using (Brush brush = new TextureBrush(sourceImage))
                    {
                        graphics.FillPath(brush, path);
                    }
                }
            }

            return result;
        }

        #endregion Greenshot methods
    }
}