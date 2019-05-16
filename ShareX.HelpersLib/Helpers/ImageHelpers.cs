#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
        private const InterpolationMode DefaultInterpolationMode = InterpolationMode.HighQualityBicubic;

        public static Image ResizeImage(Image img, int width, int height, InterpolationMode interpolationMode = DefaultInterpolationMode)
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
                g.InterpolationMode = interpolationMode;
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

        public static Image ResizeImage(Image img, Size size, InterpolationMode interpolationMode = DefaultInterpolationMode)
        {
            return ResizeImage(img, size.Width, size.Height, interpolationMode);
        }

        public static Image ResizeImageByPercentage(Image img, float percentageWidth, float percentageHeight, InterpolationMode interpolationMode = DefaultInterpolationMode)
        {
            int width = (int)Math.Round(percentageWidth / 100 * img.Width);
            int height = (int)Math.Round(percentageHeight / 100 * img.Height);
            return ResizeImage(img, width, height, interpolationMode);
        }

        public static Image ResizeImageByPercentage(Image img, float percentage, InterpolationMode interpolationMode = DefaultInterpolationMode)
        {
            return ResizeImageByPercentage(img, percentage, percentage, interpolationMode);
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

        public static Image CreateThumbnail(Image img, int width, int height)
        {
            double srcRatio = (double)img.Width / img.Height;
            double dstRatio = (double)width / height;
            int w, h;

            if (srcRatio >= dstRatio)
            {
                if (srcRatio >= 1)
                {
                    w = (int)(img.Height * dstRatio);
                }
                else
                {
                    w = (int)(img.Width / srcRatio * dstRatio);
                }

                h = img.Height;
            }
            else
            {
                w = img.Width;

                if (srcRatio >= 1)
                {
                    h = (int)(img.Height / dstRatio * srcRatio);
                }
                else
                {
                    h = (int)(img.Height * srcRatio / dstRatio);
                }
            }

            int x = (img.Width - w) / 2;
            int y = (img.Height - h) / 2;

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SetHighQuality();
                g.DrawImage(img, new Rectangle(0, 0, width, height), new Rectangle(x, y, w, h), GraphicsUnit.Pixel);
            }

            return bmp;
        }

        /// <summary>If image size is bigger than specified size then resize it and keep aspect ratio else return image.</summary>
        public static Image ResizeImageLimit(Image img, int width, int height)
        {
            if (img.Width <= width && img.Height <= height)
            {
                return img;
            }

            double ratioX = (double)width / img.Width;
            double ratioY = (double)height / img.Height;

            if (ratioX < ratioY)
            {
                height = (int)Math.Round(img.Height * ratioX);
            }
            else if (ratioX > ratioY)
            {
                width = (int)Math.Round(img.Width * ratioY);
            }

            return ResizeImage(img, width, height);
        }

        public static Image ResizeImageLimit(Image img, Size size)
        {
            return ResizeImageLimit(img, size.Width, size.Height);
        }

        public static Image ResizeImageLimit(Image img, int size)
        {
            return ResizeImageLimit(img, size, size);
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

        /// <summary>Automatically crop image to remove transparent outside area.</summary>
        public static Bitmap AutoCropTransparent(Bitmap bmp)
        {
            Rectangle source = new Rectangle(0, 0, bmp.Width, bmp.Height);
            Rectangle rect = source;

            using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true, ImageLockMode.ReadOnly))
            {
                bool leave = false;

                // Find X
                for (int x = rect.X; x < rect.Width && !leave; x++)
                {
                    for (int y = rect.Y; y < rect.Height; y++)
                    {
                        if (unsafeBitmap.GetPixel(x, y).Alpha > 0)
                        {
                            rect.X = x;
                            leave = true;
                            break;
                        }
                    }
                }

                // If all pixels transparent
                if (!leave)
                {
                    return bmp;
                }

                leave = false;

                // Find Y
                for (int y = rect.Y; y < rect.Height && !leave; y++)
                {
                    for (int x = rect.X; x < rect.Width; x++)
                    {
                        if (unsafeBitmap.GetPixel(x, y).Alpha > 0)
                        {
                            rect.Y = y;
                            leave = true;
                            break;
                        }
                    }
                }

                leave = false;

                // Find Width
                for (int x = rect.Width - 1; x >= rect.X && !leave; x--)
                {
                    for (int y = rect.Y; y < rect.Height; y++)
                    {
                        if (unsafeBitmap.GetPixel(x, y).Alpha > 0)
                        {
                            rect.Width = x - rect.X + 1;
                            leave = true;
                            break;
                        }
                    }
                }

                leave = false;

                // Find Height
                for (int y = rect.Height - 1; y >= rect.Y && !leave; y--)
                {
                    for (int x = rect.X; x < rect.Width; x++)
                    {
                        if (unsafeBitmap.GetPixel(x, y).Alpha > 0)
                        {
                            rect.Height = y - rect.Y + 1;
                            leave = true;
                            break;
                        }
                    }
                }
            }

            if (source != rect)
            {
                Bitmap croppedBitmap = CropBitmap(bmp, rect);

                if (croppedBitmap != null)
                {
                    bmp.Dispose();
                    return croppedBitmap;
                }
            }

            return bmp;
        }

        /// <summary>Automatically crop image to remove transparent outside area. Only checks center pixels.</summary>
        public static Bitmap QuickAutoCropTransparent(Bitmap bmp)
        {
            Rectangle source = new Rectangle(0, 0, bmp.Width, bmp.Height);
            Rectangle rect = source;

            using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true, ImageLockMode.ReadOnly))
            {
                int middleX = rect.Width / 2;
                int middleY = rect.Height / 2;

                // Find X
                for (int x = rect.X; x < rect.Width; x++)
                {
                    if (unsafeBitmap.GetPixel(x, middleY).Alpha > 0)
                    {
                        rect.X = x;
                        break;
                    }
                }

                // Find Y
                for (int y = rect.Y; y < rect.Height; y++)
                {
                    if (unsafeBitmap.GetPixel(middleX, y).Alpha > 0)
                    {
                        rect.Y = y;
                        break;
                    }
                }

                // Find Width
                for (int x = rect.Width - 1; x >= rect.X; x--)
                {
                    if (unsafeBitmap.GetPixel(x, middleY).Alpha > 0)
                    {
                        rect.Width = x - rect.X + 1;
                        break;
                    }
                }

                // Find Height
                for (int y = rect.Height - 1; y >= rect.Y; y--)
                {
                    if (unsafeBitmap.GetPixel(middleX, y).Alpha > 0)
                    {
                        rect.Height = y - rect.Y + 1;
                        break;
                    }
                }
            }

            if (source != rect)
            {
                Bitmap croppedBitmap = CropBitmap(bmp, rect);

                if (croppedBitmap != null)
                {
                    bmp.Dispose();
                    return croppedBitmap;
                }
            }

            return bmp;
        }

        public static Image AddCanvas(Image img, Padding margin)
        {
            return AddCanvas(img, margin, Color.Transparent);
        }

        public static Image AddCanvas(Image img, Padding margin, Color canvasColor)
        {
            if (margin.All == 0 || img.Width + margin.Horizontal < 1 || img.Height + margin.Vertical < 1)
            {
                return null;
            }

            Bitmap bmp = img.CreateEmptyBitmap(margin.Horizontal, margin.Vertical);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SetHighQuality();
                g.DrawImage(img, margin.Left, margin.Top, img.Width, img.Height);

                if (canvasColor.A > 0)
                {
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.SmoothingMode = SmoothingMode.None;

                    using (Brush brush = new SolidBrush(canvasColor))
                    {
                        if (margin.Top > 0)
                        {
                            g.FillRectangle(brush, 0, 0, bmp.Width, margin.Top);
                        }

                        if (margin.Right > 0)
                        {
                            g.FillRectangle(brush, bmp.Width - margin.Right, 0, margin.Right, bmp.Height);
                        }

                        if (margin.Bottom > 0)
                        {
                            g.FillRectangle(brush, 0, bmp.Height - margin.Bottom, bmp.Width, margin.Bottom);
                        }

                        if (margin.Left > 0)
                        {
                            g.FillRectangle(brush, 0, 0, margin.Left, bmp.Height);
                        }
                    }
                }
            }

            return bmp;
        }

        public static Image RoundedCorners(Image img, int cornerRadius)
        {
            Bitmap bmp = img.CreateEmptyBitmap();

            using (Graphics g = Graphics.FromImage(bmp))
            using (img)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.Half;

                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddRoundedRectangleProper(new RectangleF(0, 0, img.Width, img.Height), cornerRadius, 0);

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

        public static Bitmap CreateBitmap(int width, int height, Color color)
        {
            if (width > 0 && height > 0)
            {
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(color);
                }

                return bmp;
            }

            return null;
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
            return DrawCheckers(img, 10, SystemColors.ControlLight, SystemColors.ControlLightLight);
        }

        public static Image DrawCheckers(Image img, int size, Color color1, Color color2)
        {
            Bitmap bmp = img.CreateEmptyBitmap();

            using (Graphics g = Graphics.FromImage(bmp))
            using (Image checker = CreateCheckerPattern(size, size, color1, color2))
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
            using (Image checker = CreateCheckerPattern())
            using (Brush checkerBrush = new TextureBrush(checker, WrapMode.Tile))
            {
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }

            return bmp;
        }

        public static Image CreateCheckerPattern()
        {
            return CreateCheckerPattern(10, 10);
        }

        public static Image CreateCheckerPattern(int width, int height)
        {
            return CreateCheckerPattern(width, height, SystemColors.ControlLight, SystemColors.ControlLightLight);
        }

        public static Image CreateCheckerPattern(int width, int height, Color color1, Color color2)
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

        public static bool IsImageTransparent(Bitmap bmp)
        {
            if (bmp.PixelFormat == PixelFormat.Format32bppArgb)
            {
                using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true, ImageLockMode.ReadOnly))
                {
                    return unsafeBitmap.IsTransparent();
                }
            }

            return false;
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
                newWidth = (int)Math.Round((oldWidth * cos) + (oldHeight * sin));
                newHeight = (int)Math.Round((oldWidth * sin) + (oldHeight * cos));
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
                    BoxBlur((Bitmap)shadowImage, size);
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
                        { -1, -1, -1, -1, -1 },
                        { -1,  2,  2,  2, -1 },
                        { -1,  2, 16,  2, -1 },
                        { -1,  2,  2,  2, -1 },
                        { -1, -1, -1, -1, -1 }
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

                                        rgb = (imageY * pbits.Stride) + (3 * imageX);

                                        red += rgbValues[rgb + 2] * filter[filterX, filterY];
                                        green += rgbValues[rgb + 1] * filter[filterX, filterY];
                                        blue += rgbValues[rgb + 0] * filter[filterX, filterY];
                                    }

                                    rgb = (y * pbits.Stride) + (3 * x);

                                    int r = Math.Min(Math.Max((int)((factor * red) + (bias * rgbValues[rgb + 2])), 0), 255);
                                    int g = Math.Min(Math.Max((int)((factor * green) + (bias * rgbValues[rgb + 1])), 0), 255);
                                    int b = Math.Min(Math.Max((int)((factor * blue) + (bias * rgbValues[rgb + 0])), 0), 255);

                                    result[x, y] = Color.FromArgb(r, g, b);
                                }
                            }
                        }

                        // Update the image with the sharpened pixels.
                        for (int x = s; x < width - s; x++)
                        {
                            for (int y = s; y < height - s; y++)
                            {
                                rgb = (y * pbits.Stride) + (3 * x);

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
                            int xLimit = Math.Min(x + pixelSize, unsafeBitmap.Width);
                            int yLimit = Math.Min(y + pixelSize, unsafeBitmap.Height);
                            int pixelCount = (xLimit - x) * (yLimit - y);
                            float r = 0, g = 0, b = 0, a = 0;
                            float weightedCount = 0;

                            for (int y2 = y; y2 < yLimit; y2++)
                            {
                                for (int x2 = x; x2 < xLimit; x2++)
                                {
                                    ColorBgra color = unsafeBitmap.GetPixel(x2, y2);

                                    float pixelWeight = color.Alpha / 255;

                                    r += color.Red * pixelWeight;
                                    g += color.Green * pixelWeight;
                                    b += color.Blue * pixelWeight;
                                    a += color.Alpha * pixelWeight;

                                    weightedCount += pixelWeight;
                                }
                            }

                            ColorBgra averageColor = new ColorBgra((byte)(b / weightedCount), (byte)(g / weightedCount), (byte)(r / weightedCount), (byte)(a / pixelCount));

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

        public static void BoxBlur(Bitmap bmp, int range)
        {
            BoxBlur(bmp, range, new Rectangle(0, 0, bmp.Width, bmp.Height));
        }

        // https://lotsacode.wordpress.com/2010/12/08/fast-blur-box-blur-with-accumulator/
        public static void BoxBlur(Bitmap bmp, int range, Rectangle rect)
        {
            if (range > 1)
            {
                if (range.IsEvenNumber())
                {
                    range++;
                }

                using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true))
                {
                    BoxBlurHorizontal(unsafeBitmap, range, rect);
                    BoxBlurVertical(unsafeBitmap, range, rect);
                    BoxBlurHorizontal(unsafeBitmap, range, rect);
                    BoxBlurVertical(unsafeBitmap, range, rect);
                }
            }
        }

        private static void BoxBlurHorizontal(UnsafeBitmap unsafeBitmap, int range, Rectangle rect)
        {
            int left = rect.X;
            int top = rect.Y;
            int right = rect.Right;
            int bottom = rect.Bottom;
            int halfRange = range / 2;
            ColorBgra[] newColors = new ColorBgra[unsafeBitmap.Width];

            for (int y = top; y < bottom; y++)
            {
                int hits = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                int a = 0;

                for (int x = left - halfRange; x < right; x++)
                {
                    int oldPixel = x - halfRange - 1;
                    if (oldPixel >= left)
                    {
                        ColorBgra color = unsafeBitmap.GetPixel(oldPixel, y);

                        if (color.Bgra != 0)
                        {
                            r -= color.Red;
                            g -= color.Green;
                            b -= color.Blue;
                            a -= color.Alpha;
                        }

                        hits--;
                    }

                    int newPixel = x + halfRange;
                    if (newPixel < right)
                    {
                        ColorBgra color = unsafeBitmap.GetPixel(newPixel, y);

                        if (color.Bgra != 0)
                        {
                            r += color.Red;
                            g += color.Green;
                            b += color.Blue;
                            a += color.Alpha;
                        }

                        hits++;
                    }

                    if (x >= left)
                    {
                        newColors[x] = new ColorBgra((byte)(b / hits), (byte)(g / hits), (byte)(r / hits), (byte)(a / hits));
                    }
                }

                for (int x = left; x < right; x++)
                {
                    unsafeBitmap.SetPixel(x, y, newColors[x]);
                }
            }
        }

        private static void BoxBlurVertical(UnsafeBitmap unsafeBitmap, int range, Rectangle rect)
        {
            int left = rect.X;
            int top = rect.Y;
            int right = rect.Right;
            int bottom = rect.Bottom;
            int halfRange = range / 2;
            ColorBgra[] newColors = new ColorBgra[unsafeBitmap.Height];

            for (int x = left; x < right; x++)
            {
                int hits = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                int a = 0;

                for (int y = top - halfRange; y < bottom; y++)
                {
                    int oldPixel = y - halfRange - 1;
                    if (oldPixel >= top)
                    {
                        ColorBgra color = unsafeBitmap.GetPixel(x, oldPixel);

                        if (color.Bgra != 0)
                        {
                            r -= color.Red;
                            g -= color.Green;
                            b -= color.Blue;
                            a -= color.Alpha;
                        }

                        hits--;
                    }

                    int newPixel = y + halfRange;
                    if (newPixel < bottom)
                    {
                        ColorBgra color = unsafeBitmap.GetPixel(x, newPixel);

                        if (color.Bgra != 0)
                        {
                            r += color.Red;
                            g += color.Green;
                            b += color.Blue;
                            a += color.Alpha;
                        }

                        hits++;
                    }

                    if (y >= top)
                    {
                        newColors[y] = new ColorBgra((byte)(b / hits), (byte)(g / hits), (byte)(r / hits), (byte)(a / hits));
                    }
                }

                for (int y = top; y < bottom; y++)
                {
                    unsafeBitmap.SetPixel(x, y, newColors[y]);
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

        public static Image TornEdges(Image img, int tornDepth, int tornRange, AnchorStyles sides, bool curvedEdges)
        {
            if (tornDepth < 1 || tornRange < 1 || sides == AnchorStyles.None)
            {
                return img;
            }

            List<Point> points = new List<Point>();

            int horizontalTornCount = img.Width / tornRange;
            int verticalTornCount = img.Height / tornRange;

            if (sides.HasFlag(AnchorStyles.Top))
            {
                for (int x = 0; x < horizontalTornCount - 1; x++)
                {
                    points.Add(new Point(tornRange * x, MathHelpers.Random(0, tornDepth)));
                }
            }
            else
            {
                points.Add(new Point(0, 0));
                points.Add(new Point(img.Width - 1, 0));
            }

            if (sides.HasFlag(AnchorStyles.Right))
            {
                for (int y = 0; y < verticalTornCount - 1; y++)
                {
                    points.Add(new Point(img.Width - 1 - MathHelpers.Random(0, tornDepth), tornRange * y));
                }
            }
            else
            {
                points.Add(new Point(img.Width - 1, 0));
                points.Add(new Point(img.Width - 1, img.Height - 1));
            }

            if (sides.HasFlag(AnchorStyles.Bottom))
            {
                for (int x = 0; x < horizontalTornCount - 1; x++)
                {
                    points.Add(new Point(img.Width - 1 - (tornRange * x), img.Height - 1 - MathHelpers.Random(0, tornDepth)));
                }
            }
            else
            {
                points.Add(new Point(img.Width - 1, img.Height - 1));
                points.Add(new Point(0, img.Height - 1));
            }

            if (sides.HasFlag(AnchorStyles.Left))
            {
                for (int y = 0; y < verticalTornCount - 1; y++)
                {
                    points.Add(new Point(MathHelpers.Random(0, tornDepth), img.Height - 1 - (tornRange * y)));
                }
            }
            else
            {
                points.Add(new Point(0, img.Height - 1));
                points.Add(new Point(0, 0));
            }

            Bitmap result = img.CreateEmptyBitmap();

            using (img)
            using (Graphics g = Graphics.FromImage(result))
            using (TextureBrush brush = new TextureBrush(img))
            {
                g.SetHighQuality();

                Point[] fillPoints = points.Distinct().ToArray();

                if (curvedEdges)
                {
                    g.FillClosedCurve(brush, fillPoints);
                }
                else
                {
                    g.FillPolygon(brush, fillPoints);
                }

                return result;
            }
        }

        public static string OpenImageFileDialog(Form form = null)
        {
            string[] images = OpenImageFileDialog(false, form);

            if (images != null && images.Length > 0)
            {
                return images[0];
            }

            return null;
        }

        public static string[] OpenImageFileDialog(bool multiselect, Form form = null)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image files (*.png, *.jpg, *.jpeg, *.jpe, *.jfif, *.gif, *.bmp, *.tif, *.tiff)|*.png;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.bmp;*.tif;*.tiff|" +
                    "PNG (*.png)|*.png|JPEG (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|GIF (*.gif)|*.gif|BMP (*.bmp)|*.bmp|TIFF (*.tif, *.tiff)|*.tif;*.tiff";

                ofd.Multiselect = multiselect;

                if (ofd.ShowDialog(form) == DialogResult.OK)
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
                if (ext.Equals("png", StringComparison.OrdinalIgnoreCase))
                {
                    imageFormat = ImageFormat.Png;
                }
                else if (ext.Equals("jpg", StringComparison.OrdinalIgnoreCase) || ext.Equals("jpeg", StringComparison.OrdinalIgnoreCase) ||
                    ext.Equals("jpe", StringComparison.OrdinalIgnoreCase) || ext.Equals("jfif", StringComparison.OrdinalIgnoreCase))
                {
                    imageFormat = ImageFormat.Jpeg;
                }
                else if (ext.Equals("gif", StringComparison.OrdinalIgnoreCase))
                {
                    imageFormat = ImageFormat.Gif;
                }
                else if (ext.Equals("bmp", StringComparison.OrdinalIgnoreCase))
                {
                    imageFormat = ImageFormat.Bmp;
                }
                else if (ext.Equals("tif", StringComparison.OrdinalIgnoreCase) || ext.Equals("tiff", StringComparison.OrdinalIgnoreCase))
                {
                    imageFormat = ImageFormat.Tiff;
                }
            }

            return imageFormat;
        }

        public static void SaveImage(Image img, string filePath)
        {
            Helpers.CreateDirectoryFromFilePath(filePath);

            img.Save(filePath, GetImageFormat(filePath));
        }

        public static string SaveImageFileDialog(Image img, string filePath = "", bool useLastDirectory = true)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                string initialDirectory = null;

                if (useLastDirectory && !string.IsNullOrEmpty(HelpersOptions.LastSaveDirectory) && Directory.Exists(HelpersOptions.LastSaveDirectory))
                {
                    initialDirectory = HelpersOptions.LastSaveDirectory;
                }

                if (!string.IsNullOrEmpty(filePath))
                {
                    string folder = Path.GetDirectoryName(filePath);

                    if (string.IsNullOrEmpty(initialDirectory) && !string.IsNullOrEmpty(folder) && Directory.Exists(folder))
                    {
                        initialDirectory = folder;
                    }

                    sfd.FileName = Path.GetFileNameWithoutExtension(filePath);
                }

                sfd.InitialDirectory = initialDirectory;
                sfd.DefaultExt = "png";
                sfd.Filter = "PNG (*.png)|*.png|JPEG (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|GIF (*.gif)|*.gif|BMP (*.bmp)|*.bmp|TIFF (*.tif, *.tiff)|*.tif;*.tiff";

                if (sfd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(sfd.FileName))
                {
                    SaveImage(img, sfd.FileName);
                    HelpersOptions.LastSaveDirectory = Path.GetDirectoryName(sfd.FileName);
                    return sfd.FileName;
                }
            }

            return null;
        }

        public static Image LoadImage(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    filePath = Helpers.GetAbsolutePath(filePath);

                    if (!string.IsNullOrEmpty(filePath) && Helpers.IsImageFile(filePath) && File.Exists(filePath))
                    {
                        // http://stackoverflow.com/questions/788335/why-does-image-fromfile-keep-a-file-handle-open-sometimes
                        Image img = Image.FromStream(new MemoryStream(File.ReadAllBytes(filePath)));

                        if (HelpersOptions.RotateImageByExifOrientationData)
                        {
                            RotateImageByExifOrientationData(img);
                        }

                        return img;
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return null;
        }

        public static Image LoadImageWithFileDialog()
        {
            string filepath = OpenImageFileDialog();

            if (!string.IsNullOrEmpty(filepath))
            {
                return LoadImage(filepath);
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
                using (Image checker = CreateCheckerPattern(rect.Width / 2, rect.Height / 2))
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

                Rectangle holeRect = new Rectangle((rect.Width / 2) - (holeSize / 2), (rect.Height / 2) - (holeSize / 2), holeSize, holeSize);

                g.FillRectangle(Brushes.Transparent, holeRect);
                g.DrawRectangleProper(Pens.Black, holeRect);
            }
        }

        public static Rectangle FindAutoCropRectangle(Bitmap bmp, bool sameColorCrop = false,
            AnchorStyles sides = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right)
        {
            Rectangle source = new Rectangle(0, 0, bmp.Width, bmp.Height);

            if (sides == AnchorStyles.None)
            {
                return source;
            }

            Rectangle crop = source;

            using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true, ImageLockMode.ReadOnly))
            {
                bool leave = false;

                ColorBgra checkColor = unsafeBitmap.GetPixel(0, 0);
                uint mask = checkColor.Alpha == 0 ? 0xFF000000 : 0xFFFFFFFF;
                uint check = checkColor.Bgra & mask;

                if (sides.HasFlag(AnchorStyles.Left))
                {
                    // Find X (Left to right)
                    for (int x = 0; x < bmp.Width && !leave; x++)
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            if ((unsafeBitmap.GetPixel(x, y).Bgra & mask) != check)
                            {
                                crop.X = x;
                                crop.Width -= x;
                                leave = true;
                                break;
                            }
                        }
                    }

                    // If all pixels same color
                    if (!leave)
                    {
                        return crop;
                    }

                    leave = false;
                }

                if (sides.HasFlag(AnchorStyles.Top))
                {
                    // Find Y (Top to bottom)
                    for (int y = 0; y < bmp.Height && !leave; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            if ((unsafeBitmap.GetPixel(x, y).Bgra & mask) != check)
                            {
                                crop.Y = y;
                                crop.Height -= y;
                                leave = true;
                                break;
                            }
                        }
                    }

                    leave = false;
                }

                if (!sameColorCrop)
                {
                    checkColor = unsafeBitmap.GetPixel(bmp.Width - 1, bmp.Height - 1);
                    mask = checkColor.Alpha == 0 ? 0xFF000000 : 0xFFFFFFFF;
                    check = checkColor.Bgra & mask;
                }

                if (sides.HasFlag(AnchorStyles.Right))
                {
                    // Find Width (Right to left)
                    for (int x = bmp.Width - 1; x >= 0 && !leave; x--)
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            if ((unsafeBitmap.GetPixel(x, y).Bgra & mask) != check)
                            {
                                crop.Width = x - crop.X + 1;
                                leave = true;
                                break;
                            }
                        }
                    }

                    leave = false;
                }

                if (sides.HasFlag(AnchorStyles.Bottom))
                {
                    // Find Height (Bottom to top)
                    for (int y = bmp.Height - 1; y >= 0 && !leave; y--)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            if ((unsafeBitmap.GetPixel(x, y).Bgra & mask) != check)
                            {
                                crop.Height = y - crop.Y + 1;
                                leave = true;
                                break;
                            }
                        }
                    }
                }
            }

            return crop;
        }

        public static Bitmap AutoCropImage(Bitmap bmp, bool sameColorCrop = false,
            AnchorStyles sides = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right)
        {
            Rectangle source = new Rectangle(0, 0, bmp.Width, bmp.Height);
            Rectangle rect = FindAutoCropRectangle(bmp, sameColorCrop, sides);

            if (source != rect)
            {
                Bitmap croppedBitmap = CropBitmap(bmp, rect);

                if (croppedBitmap != null)
                {
                    bmp.Dispose();
                    return croppedBitmap;
                }
            }

            return bmp;
        }

        public static RotateFlipType RotateImageByExifOrientationData(Image img, bool removeExifOrientationData = true)
        {
            int orientationId = 0x0112;
            RotateFlipType rotateType = RotateFlipType.RotateNoneFlipNone;

            if (img.PropertyIdList.Contains(orientationId))
            {
                PropertyItem propertyItem = img.GetPropertyItem(orientationId);
                rotateType = GetRotateFlipTypeByExifOrientationData(propertyItem.Value[0]);

                if (rotateType != RotateFlipType.RotateNoneFlipNone)
                {
                    img.RotateFlip(rotateType);

                    if (removeExifOrientationData)
                    {
                        img.RemovePropertyItem(orientationId);
                    }
                }
            }

            return rotateType;
        }

        private static RotateFlipType GetRotateFlipTypeByExifOrientationData(int orientation)
        {
            switch (orientation)
            {
                default:
                case 1:
                    return RotateFlipType.RotateNoneFlipNone;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
            }
        }

        public static void SelectiveColor(Bitmap bmp, Color lightColor, Color darkColor, int threshold)
        {
            using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true))
            {
                for (int i = 0; i < unsafeBitmap.PixelCount; i++)
                {
                    ColorBgra color = unsafeBitmap.GetPixel(i);
                    Color newColor = ColorHelpers.PerceivedBrightness(color.ToColor()) > threshold ? lightColor : darkColor;
                    color.Red = newColor.R;
                    color.Green = newColor.G;
                    color.Blue = newColor.B;
                    unsafeBitmap.SetPixel(i, color);
                }
            }
        }

        public static Size GetImageFileDimensions(string filePath)
        {
            using (Image img = LoadImage(filePath))
            {
                if (img != null)
                {
                    return img.Size;
                }
            }

            return Size.Empty;
        }
    }
}