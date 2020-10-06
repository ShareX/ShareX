#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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

        public static Bitmap ResizeImage(Bitmap bmp, int width, int height, InterpolationMode interpolationMode = DefaultInterpolationMode)
        {
            if (width < 1 || height < 1 || (bmp.Width == width && bmp.Height == height))
            {
                return bmp;
            }

            Bitmap bmpResult = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmpResult.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (bmp)
            using (Graphics g = Graphics.FromImage(bmpResult))
            {
                g.InterpolationMode = interpolationMode;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.CompositingMode = CompositingMode.SourceOver;

                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(bmp, new Rectangle(0, 0, width, height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);
                }
            }

            return bmpResult;
        }

        public static Bitmap ResizeImage(Bitmap bmp, Size size, InterpolationMode interpolationMode = DefaultInterpolationMode)
        {
            return ResizeImage(bmp, size.Width, size.Height, interpolationMode);
        }

        public static Bitmap ResizeImage(Bitmap bmp, Size size, bool allowEnlarge, bool centerImage = true)
        {
            return ResizeImage(bmp, size.Width, size.Height, allowEnlarge, centerImage);
        }

        public static Bitmap ResizeImage(Bitmap bmp, int width, int height, bool allowEnlarge, bool centerImage = true)
        {
            return ResizeImage(bmp, width, height, allowEnlarge, centerImage, Color.Transparent);
        }

        public static Bitmap ResizeImage(Bitmap bmp, int width, int height, bool allowEnlarge, bool centerImage, Color backColor)
        {
            double ratio;
            int newWidth, newHeight;

            if (!allowEnlarge && bmp.Width <= width && bmp.Height <= height)
            {
                ratio = 1.0;
                newWidth = bmp.Width;
                newHeight = bmp.Height;
            }
            else
            {
                double ratioX = (double)width / bmp.Width;
                double ratioY = (double)height / bmp.Height;
                ratio = ratioX < ratioY ? ratioX : ratioY;
                newWidth = (int)(bmp.Width * ratio);
                newHeight = (int)(bmp.Height * ratio);
            }

            int newX = 0;
            int newY = 0;

            if (centerImage)
            {
                newX += (int)((width - (bmp.Width * ratio)) / 2);
                newY += (int)((height - (bmp.Height * ratio)) / 2);
            }

            Bitmap bmpResult = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmpResult.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bmpResult))
            {
                if (backColor.A > 0)
                {
                    g.Clear(backColor);
                }

                g.SetHighQuality();
                g.DrawImage(bmp, newX, newY, newWidth, newHeight);
            }

            return bmpResult;
        }

        public static Bitmap CreateThumbnail(Bitmap bmp, int width, int height)
        {
            double srcRatio = (double)bmp.Width / bmp.Height;
            double dstRatio = (double)width / height;
            int w, h;

            if (srcRatio >= dstRatio)
            {
                if (srcRatio >= 1)
                {
                    w = (int)(bmp.Height * dstRatio);
                }
                else
                {
                    w = (int)(bmp.Width / srcRatio * dstRatio);
                }

                h = bmp.Height;
            }
            else
            {
                w = bmp.Width;

                if (srcRatio >= 1)
                {
                    h = (int)(bmp.Height / dstRatio * srcRatio);
                }
                else
                {
                    h = (int)(bmp.Height * srcRatio / dstRatio);
                }
            }

            int x = (bmp.Width - w) / 2;
            int y = (bmp.Height - h) / 2;

            Bitmap bmpResult = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmpResult.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bmpResult))
            {
                g.SetHighQuality();
                g.DrawImage(bmp, new Rectangle(0, 0, width, height), new Rectangle(x, y, w, h), GraphicsUnit.Pixel);
            }

            return bmpResult;
        }

        /// <summary>If image size is bigger than specified size then resize it and keep aspect ratio else return image.</summary>
        public static Bitmap ResizeImageLimit(Bitmap bmp, int width, int height)
        {
            if (bmp.Width <= width && bmp.Height <= height)
            {
                return bmp;
            }

            double ratioX = (double)width / bmp.Width;
            double ratioY = (double)height / bmp.Height;

            if (ratioX < ratioY)
            {
                height = (int)Math.Round(bmp.Height * ratioX);
            }
            else if (ratioX > ratioY)
            {
                width = (int)Math.Round(bmp.Width * ratioY);
            }

            return ResizeImage(bmp, width, height);
        }

        public static Bitmap ResizeImageLimit(Bitmap bmp, Size size)
        {
            return ResizeImageLimit(bmp, size.Width, size.Height);
        }

        public static Bitmap ResizeImageLimit(Bitmap bmp, int size)
        {
            return ResizeImageLimit(bmp, size, size);
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

        public static Bitmap AddCanvas(Image img, Padding margin)
        {
            return AddCanvas(img, margin, Color.Transparent);
        }

        public static Bitmap AddCanvas(Image img, Padding margin, Color canvasColor)
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

        public static Bitmap RoundedCorners(Bitmap bmp, int cornerRadius)
        {
            Bitmap bmpResult = bmp.CreateEmptyBitmap();

            using (bmp)
            using (Graphics g = Graphics.FromImage(bmpResult))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.Half;

                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddRoundedRectangleProper(new RectangleF(0, 0, bmp.Width, bmp.Height), cornerRadius, 0);

                    using (TextureBrush brush = new TextureBrush(bmp))
                    {
                        g.FillPath(brush, gp);
                    }
                }
            }

            return bmpResult;
        }

        public static Bitmap OutlineOld(Bitmap bmp, int borderSize, Color borderColor)
        {
            Bitmap bmpResult = bmp.CreateEmptyBitmap(borderSize * 2, borderSize * 2);

            ColorMatrix maskMatrix = new ColorMatrix();
            maskMatrix.Matrix00 = 0;
            maskMatrix.Matrix11 = 0;
            maskMatrix.Matrix22 = 0;
            maskMatrix.Matrix33 = 1;
            maskMatrix.Matrix40 = ((float)borderColor.R).Remap(0, 255, 0, 1);
            maskMatrix.Matrix41 = ((float)borderColor.G).Remap(0, 255, 0, 1);
            maskMatrix.Matrix42 = ((float)borderColor.B).Remap(0, 255, 0, 1);

            using (bmp)
            using (Image shadow = maskMatrix.Apply(bmp))
            using (Graphics g = Graphics.FromImage(bmpResult))
            {
                for (int i = 0; i <= borderSize * 2; i++)
                {
                    g.DrawImage(shadow, new Rectangle(i, 0, shadow.Width, shadow.Height));
                    g.DrawImage(shadow, new Rectangle(i, borderSize * 2, shadow.Width, shadow.Height));
                    g.DrawImage(shadow, new Rectangle(0, i, shadow.Width, shadow.Height));
                    g.DrawImage(shadow, new Rectangle(borderSize * 2, i, shadow.Width, shadow.Height));
                }

                g.DrawImage(bmp, new Rectangle(borderSize, borderSize, bmp.Width, bmp.Height));
            }

            return bmpResult;
        }

        public static Bitmap Outline(Bitmap bmp, int borderSize, Color borderColor, int padding = 0, bool outlineOnly = false)
        {
            Bitmap outline = MakeOutline(bmp, padding, padding + borderSize + 1, borderColor);

            if (outlineOnly)
            {
                bmp.Dispose();
                return outline;
            }
            else
            {
                using (outline)
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(outline, 0, 0, outline.Width, outline.Height);
                }

                return bmp;
            }
        }

        public static Bitmap MakeOutline(Bitmap bmp, int minRadius, int maxRadius, Color color)
        {
            Bitmap bmpResult = bmp.CreateEmptyBitmap();

            using (UnsafeBitmap source = new UnsafeBitmap(bmp, true, ImageLockMode.ReadOnly))
            using (UnsafeBitmap dest = new UnsafeBitmap(bmpResult, true, ImageLockMode.WriteOnly))
            {
                for (int x = 0; x < source.Width; x++)
                {
                    for (int y = 0; y < source.Height; y++)
                    {
                        float dist = DistanceToThreshold(source, x, y, maxRadius, 255);

                        if (dist > minRadius && dist < maxRadius)
                        {
                            byte alpha = 255;

                            if (dist - minRadius < 1)
                            {
                                alpha = (byte)(255 * (dist - minRadius));
                            }
                            else if (maxRadius - dist < 1)
                            {
                                alpha = (byte)(255 * (maxRadius - dist));
                            }

                            ColorBgra bgra = new ColorBgra(color.B, color.G, color.R, alpha);
                            dest.SetPixel(x, y, bgra);
                        }
                    }
                }
            }

            return bmpResult;
        }

        private static float DistanceToThreshold(UnsafeBitmap unsafeBitmap, int x, int y, int radius, int threshold)
        {
            int minx = Math.Max(x - radius, 0);
            int maxx = Math.Min(x + radius, unsafeBitmap.Width - 1);
            int miny = Math.Max(y - radius, 0);
            int maxy = Math.Min(y + radius, unsafeBitmap.Height - 1);
            int dist2 = (radius * radius) + 1;

            for (int tx = minx; tx <= maxx; tx++)
            {
                for (int ty = miny; ty <= maxy; ty++)
                {
                    ColorBgra color = unsafeBitmap.GetPixel(tx, ty);

                    if (color.Alpha >= threshold)
                    {
                        int dx = tx - x;
                        int dy = ty - y;
                        int test_dist2 = (dx * dx) + (dy * dy);
                        if (test_dist2 < dist2)
                        {
                            dist2 = test_dist2;
                        }
                    }
                }
            }

            return (float)Math.Sqrt(dist2);
        }

        public static Bitmap DrawReflection(Bitmap bmp, int percentage, int maxAlpha, int minAlpha, int offset, bool skew, int skewSize)
        {
            Bitmap reflection = AddReflection(bmp, percentage, maxAlpha, minAlpha);

            if (skew)
            {
                reflection = AddSkew(reflection, skewSize, 0);
            }

            Bitmap bmpResult = new Bitmap(reflection.Width, bmp.Height + reflection.Height + offset);

            using (bmp)
            using (reflection)
            using (Graphics g = Graphics.FromImage(bmpResult))
            {
                g.SetHighQuality();
                g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                g.DrawImage(reflection, 0, bmp.Height + offset, reflection.Width, reflection.Height);
            }

            return bmpResult;
        }

        public static Bitmap AddSkew(Image img, int x, int y)
        {
            Bitmap result = img.CreateEmptyBitmap(Math.Abs(x), Math.Abs(y));

            using (img)
            using (Graphics g = Graphics.FromImage(result))
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
            percentage = percentage.Clamp(1, 100);
            maxAlpha = maxAlpha.Clamp(0, 255);
            minAlpha = minAlpha.Clamp(0, 255);

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

        public static Bitmap DrawBorder(Bitmap bmp, Color borderColor, int borderSize, BorderType borderType)
        {
            using (Pen borderPen = new Pen(borderColor, borderSize) { Alignment = PenAlignment.Inset })
            {
                return DrawBorder(bmp, borderPen, borderType);
            }
        }

        public static Bitmap DrawBorder(Bitmap bmp, Color fromBorderColor, Color toBorderColor, LinearGradientMode gradientType, int borderSize, BorderType borderType)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            if (borderType == BorderType.Outside)
            {
                width += borderSize * 2;
                height += borderSize * 2;
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, width, height), fromBorderColor, toBorderColor, gradientType))
            using (Pen borderPen = new Pen(brush, borderSize) { Alignment = PenAlignment.Inset })
            {
                return DrawBorder(bmp, borderPen, borderType);
            }
        }

        public static Bitmap DrawBorder(Bitmap bmp, GradientInfo gradientInfo, int borderSize, BorderType borderType)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            if (borderType == BorderType.Outside)
            {
                width += borderSize * 2;
                height += borderSize * 2;
            }

            using (LinearGradientBrush brush = gradientInfo.GetGradientBrush(new Rectangle(0, 0, width, height)))
            using (Pen borderPen = new Pen(brush, borderSize) { Alignment = PenAlignment.Inset })
            {
                return DrawBorder(bmp, borderPen, borderType);
            }
        }

        public static Bitmap DrawBorder(Bitmap bmp, Pen borderPen, BorderType borderType)
        {
            Bitmap bmpResult;

            if (borderType == BorderType.Inside)
            {
                bmpResult = bmp;

                using (Graphics g = Graphics.FromImage(bmpResult))
                {
                    g.DrawRectangleProper(borderPen, 0, 0, bmp.Width, bmp.Height);
                }
            }
            else
            {
                int borderSize = (int)borderPen.Width;
                bmpResult = bmp.CreateEmptyBitmap(borderSize * 2, borderSize * 2);

                using (bmp)
                using (Graphics g = Graphics.FromImage(bmpResult))
                {
                    g.DrawRectangleProper(borderPen, 0, 0, bmpResult.Width, bmpResult.Height);
                    g.SetHighQuality();
                    g.DrawImage(bmp, borderSize, borderSize, bmp.Width, bmp.Height);
                }
            }

            return bmpResult;
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

        public static Bitmap FillBackground(Image img, GradientInfo gradientInfo)
        {
            using (LinearGradientBrush brush = gradientInfo.GetGradientBrush(new Rectangle(0, 0, img.Width, img.Height)))
            {
                return FillBackground(img, brush);
            }
        }

        public static Bitmap FillBackground(Image img, Brush brush)
        {
            Bitmap result = img.CreateEmptyBitmap();

            using (img)
            using (Graphics g = Graphics.FromImage(result))
            {
                g.FillRectangle(brush, 0, 0, result.Width, result.Height);
                g.DrawImage(img, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        public static Bitmap DrawCheckers(Image img)
        {
            return DrawCheckers(img, 10, SystemColors.ControlLight, SystemColors.ControlLightLight);
        }

        public static Bitmap DrawCheckers(Image img, int checkerSize, Color checkerColor1, Color checkerColor2)
        {
            Bitmap bmpResult = img.CreateEmptyBitmap();

            using (img)
            using (Graphics g = Graphics.FromImage(bmpResult))
            using (Image checker = CreateCheckerPattern(checkerSize, checkerSize, checkerColor1, checkerColor2))
            using (Brush checkerBrush = new TextureBrush(checker, WrapMode.Tile))
            {
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmpResult.Width, bmpResult.Height));
                g.SetHighQuality();
                g.DrawImage(img, 0, 0, img.Width, img.Height);
            }

            return bmpResult;
        }

        public static Bitmap DrawCheckers(int width, int height)
        {
            return DrawCheckers(width, height, 10, SystemColors.ControlLight, SystemColors.ControlLightLight);
        }

        public static Bitmap DrawCheckers(int width, int height, int checkerSize, Color checkerColor1, Color checkerColor2)
        {
            Bitmap bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            using (Image checker = CreateCheckerPattern(checkerSize, checkerSize, checkerColor1, checkerColor2))
            using (Brush checkerBrush = new TextureBrush(checker, WrapMode.Tile))
            {
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }

            return bmp;
        }

        public static Bitmap CreateCheckerPattern()
        {
            return CreateCheckerPattern(10, 10);
        }

        public static Bitmap CreateCheckerPattern(int width, int height)
        {
            return CreateCheckerPattern(width, height, SystemColors.ControlLight, SystemColors.ControlLightLight);
        }

        public static Bitmap CreateCheckerPattern(int width, int height, Color checkerColor1, Color checkerColor2)
        {
            Bitmap bmp = new Bitmap(width * 2, height * 2);

            using (Graphics g = Graphics.FromImage(bmp))
            using (Brush brush1 = new SolidBrush(checkerColor1))
            using (Brush brush2 = new SolidBrush(checkerColor2))
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
            try
            {
                PropertyItem pi = (PropertyItem)typeof(PropertyItem).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { }, null).Invoke(null);
                pi.Id = id;
                pi.Len = text.Length + 1;
                pi.Type = 2;
                byte[] bytesText = Encoding.ASCII.GetBytes(text + " ");
                bytesText[bytesText.Length - 1] = 0;
                pi.Value = bytesText;
                img.SetPropertyItem(pi);
                return true;
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "AddMetadata reflection failed.");
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
        /// <param name="bmp">input Image object, is not modified</param>
        /// <param name="angleDegrees">angle of rotation, in degrees</param>
        /// <param name="upsize">see comments above</param>
        /// <param name="clip">see comments above, not used if upsizeOk = true</param>
        /// <returns>new Bitmap object, may be larger than input image</returns>
        public static Bitmap RotateImage(Bitmap bmp, float angleDegrees, bool upsize, bool clip)
        {
            // Test for zero rotation and return a clone of the input image
            if (angleDegrees == 0f)
            {
                return (Bitmap)bmp.Clone();
            }

            // Set up old and new image dimensions, assuming upsizing not wanted and clipping OK
            int oldWidth = bmp.Width;
            int oldHeight = bmp.Height;
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
            Bitmap bmpResult = bmp.CreateEmptyBitmap();

            // Create the Graphics object that does the work
            using (Graphics g = Graphics.FromImage(bmpResult))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;

                // Set up the built-in transformation matrix to do the rotation and maybe scaling
                g.TranslateTransform(newWidth / 2f, newHeight / 2f);

                if (scaleFactor != 1f)
                {
                    g.ScaleTransform(scaleFactor, scaleFactor);
                }

                g.RotateTransform(angleDegrees);
                g.TranslateTransform(-oldWidth / 2f, -oldHeight / 2f);

                // Draw the result
                g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
            }

            return bmpResult;
        }

        public static Bitmap AddShadow(Bitmap bmp, float opacity, int size)
        {
            return AddShadow(bmp, opacity, size, 1, Color.Black, new Point(0, 0));
        }

        public static Bitmap AddShadow(Bitmap bmp, float opacity, int size, float darkness, Color color, Point offset)
        {
            Bitmap shadowImage = null;

            try
            {
                shadowImage = bmp.CreateEmptyBitmap(size * 2, size * 2);

                ColorMatrix maskMatrix = new ColorMatrix();
                maskMatrix.Matrix00 = 0;
                maskMatrix.Matrix11 = 0;
                maskMatrix.Matrix22 = 0;
                maskMatrix.Matrix33 = opacity;
                maskMatrix.Matrix40 = ((float)color.R).Remap(0, 255, 0, 1);
                maskMatrix.Matrix41 = ((float)color.G).Remap(0, 255, 0, 1);
                maskMatrix.Matrix42 = ((float)color.B).Remap(0, 255, 0, 1);

                Rectangle shadowRectangle = new Rectangle(size, size, bmp.Width, bmp.Height);
                maskMatrix.Apply(bmp, shadowImage, shadowRectangle);

                if (size > 0)
                {
                    BoxBlur(shadowImage, size);
                }

                if (darkness > 1)
                {
                    ColorMatrix alphaMatrix = new ColorMatrix();
                    alphaMatrix.Matrix33 = darkness;

                    Bitmap shadowImage2 = alphaMatrix.Apply(shadowImage);
                    shadowImage.Dispose();
                    shadowImage = shadowImage2;
                }

                Bitmap bmpResult = shadowImage.CreateEmptyBitmap(Math.Abs(offset.X), Math.Abs(offset.Y));

                using (Graphics g = Graphics.FromImage(bmpResult))
                {
                    g.SetHighQuality();
                    g.DrawImage(shadowImage, Math.Max(0, offset.X), Math.Max(0, offset.Y), shadowImage.Width, shadowImage.Height);
                    g.DrawImage(bmp, Math.Max(size, -offset.X + size), Math.Max(size, -offset.Y + size), bmp.Width, bmp.Height);
                }

                return bmpResult;
            }
            finally
            {
                if (bmp != null) bmp.Dispose();
                if (shadowImage != null) shadowImage.Dispose();
            }
        }

        public static Bitmap Sharpen(Bitmap bmp, double strength)
        {
            if (bmp != null)
            {
                using (bmp)
                {
                    Bitmap sharpenImage = (Bitmap)bmp.Clone();
                    int width = sharpenImage.Width;
                    int height = sharpenImage.Height;

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

                    Color[,] result = new Color[sharpenImage.Width, sharpenImage.Height];

                    // Lock image bits for read/write.
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

                                    float pixelWeight = color.Alpha / 255f;

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

        public static void ColorDepth(Bitmap bmp, int bitsPerChannel = 4)
        {
            if (bitsPerChannel < 1 || bitsPerChannel > 8)
            {
                return;
            }

            double colorsPerChannel = Math.Pow(2, bitsPerChannel);
            double colorInterval = 255 / (colorsPerChannel - 1);

            byte Remap(byte color, double interval)
            {
                return (byte)Math.Round(Math.Round(color / interval) * interval);
            }

            using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true))
            {
                for (int y = 0; y < unsafeBitmap.Height; y++)
                {
                    for (int x = 0; x < unsafeBitmap.Width; x++)
                    {
                        ColorBgra color = unsafeBitmap.GetPixel(x, y);
                        color.Red = Remap(color.Red, colorInterval);
                        color.Green = Remap(color.Green, colorInterval);
                        color.Blue = Remap(color.Blue, colorInterval);
                        unsafeBitmap.SetPixel(x, y, color);
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

        public static Bitmap TornEdges(Bitmap bmp, int tornDepth, int tornRange, AnchorStyles sides, bool curvedEdges)
        {
            if (tornDepth < 1 || tornRange < 1 || sides == AnchorStyles.None)
            {
                return bmp;
            }

            List<Point> points = new List<Point>();

            int horizontalTornCount = bmp.Width / tornRange;
            int verticalTornCount = bmp.Height / tornRange;

            if (horizontalTornCount < 2 && verticalTornCount < 2)
            {
                return bmp;
            }

            if (sides.HasFlag(AnchorStyles.Top) && horizontalTornCount > 1)
            {
                for (int x = 0; x < horizontalTornCount - 1; x++)
                {
                    points.Add(new Point(tornRange * x, RandomFast.Next(0, tornDepth)));
                }
            }
            else
            {
                points.Add(new Point(0, 0));
                points.Add(new Point(bmp.Width - 1, 0));
            }

            if (sides.HasFlag(AnchorStyles.Right) && verticalTornCount > 1)
            {
                for (int y = 0; y < verticalTornCount - 1; y++)
                {
                    points.Add(new Point(bmp.Width - 1 - RandomFast.Next(0, tornDepth), tornRange * y));
                }
            }
            else
            {
                points.Add(new Point(bmp.Width - 1, 0));
                points.Add(new Point(bmp.Width - 1, bmp.Height - 1));
            }

            if (sides.HasFlag(AnchorStyles.Bottom) && horizontalTornCount > 1)
            {
                for (int x = 0; x < horizontalTornCount - 1; x++)
                {
                    points.Add(new Point(bmp.Width - 1 - (tornRange * x), bmp.Height - 1 - RandomFast.Next(0, tornDepth)));
                }
            }
            else
            {
                points.Add(new Point(bmp.Width - 1, bmp.Height - 1));
                points.Add(new Point(0, bmp.Height - 1));
            }

            if (sides.HasFlag(AnchorStyles.Left) && verticalTornCount > 1)
            {
                for (int y = 0; y < verticalTornCount - 1; y++)
                {
                    points.Add(new Point(RandomFast.Next(0, tornDepth), bmp.Height - 1 - (tornRange * y)));
                }
            }
            else
            {
                points.Add(new Point(0, bmp.Height - 1));
                points.Add(new Point(0, 0));
            }

            Bitmap bmpResult = bmp.CreateEmptyBitmap();

            using (bmp)
            using (Graphics g = Graphics.FromImage(bmpResult))
            using (TextureBrush brush = new TextureBrush(bmp))
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
            }

            return bmpResult;
        }

        public static Bitmap Slice(Bitmap bmp, int minSliceHeight, int maxSliceHeight, int minSliceShift, int maxSliceShift)
        {
            if (minSliceHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(minSliceHeight));
            }

            if (maxSliceHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSliceHeight));
            }

            Bitmap bmpResult = bmp.CreateEmptyBitmap();

            using (Graphics g = Graphics.FromImage(bmpResult))
            {
                int y = 0;

                while (y < bmp.Height)
                {
                    Rectangle sourceRect = new Rectangle(0, y, bmp.Width, RandomFast.Next(minSliceHeight, maxSliceHeight));
                    Rectangle destRect = sourceRect;

                    if (RandomFast.Next(1) == 0) // Shift left
                    {
                        destRect.X = RandomFast.Next(-maxSliceShift, -minSliceShift);
                    }
                    else // Shift right
                    {
                        destRect.X = RandomFast.Next(minSliceShift, maxSliceShift);
                    }

                    g.DrawImage(bmp, destRect, sourceRect, GraphicsUnit.Pixel);

                    y += sourceRect.Height;
                }
            }

            return bmpResult;
        }

        public static string OpenImageFileDialog(Form form = null, string initialDirectory = null)
        {
            string[] images = OpenImageFileDialog(false, form, initialDirectory);

            if (images != null && images.Length > 0)
            {
                return images[0];
            }

            return null;
        }

        public static string[] OpenImageFileDialog(bool multiselect, Form form = null, string initialDirectory = null)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image files (*.png, *.jpg, *.jpeg, *.jpe, *.jfif, *.gif, *.bmp, *.tif, *.tiff)|*.png;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.bmp;*.tif;*.tiff|" +
                    "PNG (*.png)|*.png|JPEG (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|GIF (*.gif)|*.gif|BMP (*.bmp)|*.bmp|TIFF (*.tif, *.tiff)|*.tif;*.tiff";

                ofd.Multiselect = multiselect;

                if (!string.IsNullOrEmpty(initialDirectory))
                {
                    ofd.InitialDirectory = initialDirectory;
                }

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

        public static bool SaveImage(Image img, string filePath)
        {
            Helpers.CreateDirectoryFromFilePath(filePath);
            ImageFormat imageFormat = GetImageFormat(filePath);

            try
            {
                img.Save(filePath, imageFormat);
                return true;
            }
            catch (Exception e)
            {
                e.ShowError();
            }

            return false;
        }

        public static string SaveImageFileDialog(Image img, string filePath = "", bool useLastDirectory = true)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG (*.png)|*.png|JPEG (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|GIF (*.gif)|*.gif|BMP (*.bmp)|*.bmp|TIFF (*.tif, *.tiff)|*.tif;*.tiff";
                sfd.DefaultExt = "png";

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

                    sfd.FileName = Path.GetFileName(filePath);

                    string ext = Helpers.GetFilenameExtension(filePath);

                    if (!string.IsNullOrEmpty(ext))
                    {
                        ext = ext.ToLowerInvariant();

                        switch (ext)
                        {
                            case "png":
                                sfd.FilterIndex = 1;
                                break;
                            case "jpg":
                            case "jpeg":
                            case "jpe":
                            case "jfif":
                                sfd.FilterIndex = 2;
                                break;
                            case "gif":
                                sfd.FilterIndex = 3;
                                break;
                            case "bmp":
                                sfd.FilterIndex = 4;
                                break;
                            case "tif":
                            case "tiff":
                                sfd.FilterIndex = 5;
                                break;
                        }
                    }
                }

                sfd.InitialDirectory = initialDirectory;

                if (sfd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(sfd.FileName))
                {
                    SaveImage(img, sfd.FileName);
                    HelpersOptions.LastSaveDirectory = Path.GetDirectoryName(sfd.FileName);
                    return sfd.FileName;
                }
            }

            return null;
        }

        public static Bitmap LoadImage(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    filePath = Helpers.GetAbsolutePath(filePath);

                    if (!string.IsNullOrEmpty(filePath) && Helpers.IsImageFile(filePath) && File.Exists(filePath))
                    {
                        // http://stackoverflow.com/questions/788335/why-does-image-fromfile-keep-a-file-handle-open-sometimes
                        Bitmap bmp = (Bitmap)Image.FromStream(new MemoryStream(File.ReadAllBytes(filePath)));

                        if (HelpersOptions.RotateImageByExifOrientationData)
                        {
                            RotateImageByExifOrientationData(bmp);
                        }

                        return bmp;
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return null;
        }

        public static Bitmap LoadImageWithFileDialog()
        {
            string filepath = OpenImageFileDialog();

            if (!string.IsNullOrEmpty(filepath))
            {
                return LoadImage(filepath);
            }

            return null;
        }

        public static Bitmap CombineImages(IEnumerable<Image> images, Orientation orientation = Orientation.Vertical,
            ImageCombinerAlignment alignment = ImageCombinerAlignment.LeftOrTop, int space = 0)
        {
            int width, height;
            int imageCount = images.Count();
            int spaceSize = space * (imageCount - 1);

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
                        int x;
                        switch (alignment)
                        {
                            default:
                            case ImageCombinerAlignment.LeftOrTop:
                                x = 0;
                                break;
                            case ImageCombinerAlignment.Center:
                                x = (width / 2) - (image.Width / 2);
                                break;
                            case ImageCombinerAlignment.RightOrBottom:
                                x = width - image.Width;
                                break;
                        }
                        rect = new Rectangle(x, position, image.Width, image.Height);
                        position += image.Height + space;
                    }
                    else
                    {
                        int y;
                        switch (alignment)
                        {
                            default:
                            case ImageCombinerAlignment.LeftOrTop:
                                y = 0;
                                break;
                            case ImageCombinerAlignment.Center:
                                y = (height / 2) - (image.Height / 2);
                                break;
                            case ImageCombinerAlignment.RightOrBottom:
                                y = height - image.Height;
                                break;
                        }
                        rect = new Rectangle(position, y, image.Width, image.Height);
                        position += image.Width + space;
                    }

                    g.DrawImage(image, rect);
                }
            }

            return bmp;
        }

        public static List<Bitmap> SplitImage(Image img, int rowCount, int columnCount)
        {
            List<Bitmap> images = new List<Bitmap>();

            int width = img.Width / columnCount;
            int height = img.Height / rowCount;

            for (int y = 0; y < rowCount; y++)
            {
                for (int x = 0; x < columnCount; x++)
                {
                    Bitmap bmp = new Bitmap(width, height);

                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle destRect = new Rectangle(0, 0, width, height);
                        Rectangle srcRect = new Rectangle(x * width, y * height, width, height);
                        g.DrawImage(img, destRect, srcRect, GraphicsUnit.Pixel);
                    }

                    images.Add(bmp);
                }
            }

            return images;
        }

        public static Bitmap CreateColorPickerIcon(Color color, Rectangle rect, int holeSize = 0)
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

        public static RotateFlipType RotateImageByExifOrientationData(Bitmap bmp, bool removeExifOrientationData = true)
        {
            int orientationId = 0x0112;
            RotateFlipType rotateType = RotateFlipType.RotateNoneFlipNone;

            if (bmp.PropertyIdList.Contains(orientationId))
            {
                PropertyItem propertyItem = bmp.GetPropertyItem(orientationId);
                rotateType = GetRotateFlipTypeByExifOrientationData(propertyItem.Value[0]);

                if (rotateType != RotateFlipType.RotateNoneFlipNone)
                {
                    bmp.RotateFlip(rotateType);

                    if (removeExifOrientationData)
                    {
                        bmp.RemovePropertyItem(orientationId);
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

        public static void SelectiveColor(Bitmap bmp, Color lightColor, Color darkColor, int paletteSize = 2)
        {
            paletteSize = Math.Max(paletteSize, 2);

            Dictionary<int, Color> colors = new Dictionary<int, Color>();
            for (int i = 0; i < paletteSize; i++)
            {
                Color color = ColorHelpers.Lerp(lightColor, darkColor, (float)i / (paletteSize - 1));
                int perceivedBrightness = ColorHelpers.PerceivedBrightness(color);
                if (!colors.ContainsKey(perceivedBrightness))
                {
                    colors.Add(perceivedBrightness, color);
                }
            }

            using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true))
            {
                for (int i = 0; i < unsafeBitmap.PixelCount; i++)
                {
                    ColorBgra color = unsafeBitmap.GetPixel(i);
                    int perceivedBrightness = ColorHelpers.PerceivedBrightness(color.ToColor());
                    KeyValuePair<int, Color> closest =
                        colors.Aggregate((current, next) => Math.Abs(current.Key - perceivedBrightness) < Math.Abs(next.Key - perceivedBrightness) ? current : next);
                    Color newColor = closest.Value;
                    color.Red = newColor.R;
                    color.Green = newColor.G;
                    color.Blue = newColor.B;
                    unsafeBitmap.SetPixel(i, color);
                }
            }
        }

        public static Size GetImageFileDimensions(string filePath)
        {
            using (Bitmap bmp = LoadImage(filePath))
            {
                if (bmp != null)
                {
                    return bmp.Size;
                }
            }

            return Size.Empty;
        }

        public static InterpolationMode GetInterpolationMode(ImageInterpolationMode interpolationMode)
        {
            switch (interpolationMode)
            {
                default:
                case ImageInterpolationMode.HighQualityBicubic:
                    return InterpolationMode.HighQualityBicubic;
                case ImageInterpolationMode.Bicubic:
                    return InterpolationMode.Bicubic;
                case ImageInterpolationMode.HighQualityBilinear:
                    return InterpolationMode.HighQualityBilinear;
                case ImageInterpolationMode.Bilinear:
                    return InterpolationMode.Bilinear;
                case ImageInterpolationMode.NearestNeighbor:
                    return InterpolationMode.NearestNeighbor;
            }
        }

        public static Size ApplyAspectRatio(int width, int height, Bitmap bmp)
        {
            int newWidth, newHeight;

            if (width == 0)
            {
                newWidth = (int)Math.Round((float)height / bmp.Height * bmp.Width);
                newHeight = height;
            }
            else if (height == 0)
            {
                newWidth = width;
                newHeight = (int)Math.Round((float)width / bmp.Width * bmp.Height);
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }

            return new Size(newWidth, newHeight);
        }

        public static Size ApplyAspectRatio(Size size, Bitmap bmp)
        {
            return ApplyAspectRatio(size.Width, size.Height, bmp);
        }
    }
}