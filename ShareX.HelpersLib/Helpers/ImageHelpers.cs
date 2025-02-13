#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Windows.Media.Imaging;

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

        public static Bitmap ScaleImageFast(Bitmap bmp, double scale)
        {
            return ScaleImageFast(bmp, scale, scale);
        }

        public static Bitmap ScaleImageFast(Bitmap bmp, double scaleX, double scaleY)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bmp.Save(memoryStream, ImageFormat.Bmp);
                memoryStream.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();

                TransformedBitmap transformedBitmap = new TransformedBitmap();
                transformedBitmap.BeginInit();
                transformedBitmap.Source = bitmapImage;
                transformedBitmap.Transform = new System.Windows.Media.ScaleTransform(scaleX, scaleY);
                transformedBitmap.EndInit();

                return GetBitmap(transformedBitmap);
            }
        }

        private static Bitmap GetBitmap(BitmapSource bitmapSource, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
        {
            Bitmap bmp = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, pixelFormat);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, pixelFormat);
            bitmapSource.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
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

        private static Bitmap ApplyCutOutEffect(Bitmap bmp, AnchorStyles effectEdge, CutOutEffectType effectType, int effectSize, Color backgroundColor)
        {
            switch (effectType)
            {
                case CutOutEffectType.None:
                    return bmp;
                case CutOutEffectType.ZigZag:
                    return TornEdges(bmp, effectSize, effectSize, effectEdge, false, false, backgroundColor);
                case CutOutEffectType.TornEdge:
                    return TornEdges(bmp, effectSize, effectSize * 2, effectEdge, false, true, backgroundColor);
                case CutOutEffectType.Wave:
                    return WavyEdges(bmp, effectSize, effectSize * 5, effectEdge, backgroundColor);
            }

            throw new NotImplementedException();
        }

        public static Bitmap CutOutBitmapMiddle(Bitmap bmp, Orientation orientation, int start, int size, CutOutEffectType effectType, int effectSize, Color backgroundColor)
        {
            if (bmp != null && size > 0)
            {
                Bitmap firstPart = null, secondPart = null;

                if (start > 0)
                {
                    Rectangle r = orientation == Orientation.Horizontal
                        ? new Rectangle(0, 0, Math.Min(start, bmp.Width), bmp.Height)
                        : new Rectangle(0, 0, bmp.Width, Math.Min(start, bmp.Height));
                    firstPart = CropBitmap(bmp, r);
                    AnchorStyles effectEdge = orientation == Orientation.Horizontal ? AnchorStyles.Right : AnchorStyles.Bottom;
                    firstPart = ApplyCutOutEffect(firstPart, effectEdge, effectType, effectSize, backgroundColor);
                }

                int cutDimension = orientation == Orientation.Horizontal ? bmp.Width : bmp.Height;
                if (start + size < cutDimension)
                {
                    int end = Math.Max(start + size, 0);
                    Rectangle r = orientation == Orientation.Horizontal
                        ? new Rectangle(end, 0, bmp.Width - end, bmp.Height)
                        : new Rectangle(0, end, bmp.Width, bmp.Height - end);
                    secondPart = CropBitmap(bmp, r);
                    AnchorStyles effectEdge = orientation == Orientation.Horizontal ? AnchorStyles.Left : AnchorStyles.Top;
                    secondPart = ApplyCutOutEffect(secondPart, effectEdge, effectType, effectSize, backgroundColor);
                }

                if (firstPart != null && secondPart != null)
                {
                    return CombineImages(new List<Bitmap> { firstPart, secondPart }, orientation);
                }
                else if (firstPart != null)
                {
                    return firstPart;
                }
                else if (secondPart != null)
                {
                    return secondPart;
                }
            }

            return bmp;
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

        public static Bitmap AddCanvas(Image img, int margin)
        {
            return AddCanvas(img, new Padding(margin));
        }

        public static Bitmap AddCanvas(Image img, int margin, Color canvasColor)
        {
            return AddCanvas(img, new Padding(margin), canvasColor);
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
                        if (margin.Left > 0)
                        {
                            g.FillRectangle(brush, 0, 0, margin.Left, bmp.Height);
                        }

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
                    }
                }
            }

            return bmp;
        }

        public static Bitmap DrawBackgroundImage(Bitmap bmp, Bitmap backgroundImage, bool center = true, bool tile = false)
        {
            if (bmp != null && backgroundImage != null)
            {
                using (bmp)
                using (backgroundImage)
                {
                    Bitmap bmpResult = bmp.CreateEmptyBitmap();

                    using (Graphics g = Graphics.FromImage(bmpResult))
                    {
                        g.SetHighQuality();
                        g.PixelOffsetMode = PixelOffsetMode.Half;

                        if (tile)
                        {
                            using (TextureBrush brush = new TextureBrush(backgroundImage, WrapMode.Tile))
                            {
                                if (center)
                                {
                                    int tileX = (bmpResult.Width - backgroundImage.Width) / 2 % backgroundImage.Width;
                                    int tileY = (bmpResult.Height - backgroundImage.Height) / 2 % backgroundImage.Height;

                                    brush.TranslateTransform(tileX, tileY);
                                }

                                g.FillRectangle(brush, 0, 0, bmpResult.Width, bmpResult.Height);
                            }
                        }
                        else
                        {
                            float aspectRatio = (float)backgroundImage.Width / backgroundImage.Height;

                            int width = bmpResult.Width;
                            int height = (int)(width / aspectRatio);

                            if (height < bmpResult.Height)
                            {
                                height = bmpResult.Height;
                                width = (int)(height * aspectRatio);
                            }

                            int x = 0;
                            int y = 0;

                            if (center)
                            {
                                x = (bmpResult.Width - width) / 2;
                                y = (bmpResult.Height - height) / 2;
                            }

                            g.DrawImage(backgroundImage, x, y, width, height);
                        }

                        g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                    }

                    return bmpResult;
                }
            }

            return bmp;
        }

        public static Bitmap DrawBackgroundImage(Bitmap bmp, string backgroundImageFilePath, bool center = true, bool tile = false)
        {
            Bitmap backgroundImage = LoadImage(backgroundImageFilePath);
            return DrawBackgroundImage(bmp, backgroundImage, center, tile);
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

        public static Bitmap DrawBorder(Bitmap bmp, Color borderColor, int borderSize, BorderType borderType, DashStyle dashStyle = DashStyle.Solid)
        {
            using (Pen borderPen = new Pen(borderColor, borderSize) { Alignment = PenAlignment.Inset, DashStyle = dashStyle })
            {
                return DrawBorder(bmp, borderPen, borderType);
            }
        }

        public static Bitmap DrawBorder(Bitmap bmp, Color fromBorderColor, Color toBorderColor, LinearGradientMode gradientType, int borderSize, BorderType borderType,
            DashStyle dashStyle = DashStyle.Solid)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            if (borderType == BorderType.Outside)
            {
                width += borderSize * 2;
                height += borderSize * 2;
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, width, height), fromBorderColor, toBorderColor, gradientType))
            using (Pen borderPen = new Pen(brush, borderSize) { Alignment = PenAlignment.Inset, DashStyle = dashStyle })
            {
                return DrawBorder(bmp, borderPen, borderType);
            }
        }

        public static Bitmap DrawBorder(Bitmap bmp, GradientInfo gradientInfo, int borderSize, BorderType borderType, DashStyle dashStyle = DashStyle.Solid)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            if (borderType == BorderType.Outside)
            {
                width += borderSize * 2;
                height += borderSize * 2;
            }

            using (LinearGradientBrush brush = gradientInfo.GetGradientBrush(new Rectangle(0, 0, width, height)))
            using (Pen borderPen = new Pen(brush, borderSize) { Alignment = PenAlignment.Inset, DashStyle = dashStyle })
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

        public static bool CompareImages(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 != null && bmp2 != null && bmp1.Width == bmp2.Width && bmp1.Height == bmp2.Height)
            {
                BitmapData bd1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData bd2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                try
                {
                    return NativeMethods.memcmp(bd1.Scan0, bd2.Scan0, bd1.Stride * bmp1.Height) == 0;
                }
                finally
                {
                    bmp1.UnlockBits(bd1);
                    bmp2.UnlockBits(bd2);
                }
            }

            return false;
        }

        public static bool IsImageTransparent(Bitmap bmp)
        {
            if (bmp.PixelFormat == PixelFormat.Format32bppArgb || bmp.PixelFormat == PixelFormat.Format32bppPArgb)
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

        public static Bitmap AddShadow(Bitmap bmp, float opacity, int size, float darkness, Color color, Point offset, bool autoResize = true)
        {
            Bitmap bmpShadow = null;

            try
            {
                bmpShadow = bmp.CreateEmptyBitmap(size * 2, size * 2);
                Rectangle shadowRectangle = new Rectangle(size, size, bmp.Width, bmp.Height);
                ColorMatrixManager.Mask(opacity, color).Apply(bmp, bmpShadow, shadowRectangle);

                if (size > 0)
                {
                    BoxBlur(bmpShadow, size);
                }

                if (darkness > 1)
                {
                    Bitmap shadowImage2 = ColorMatrixManager.Alpha(darkness).Apply(bmpShadow);
                    bmpShadow.Dispose();
                    bmpShadow = shadowImage2;
                }

                Bitmap bmpResult;

                if (autoResize)
                {
                    bmpResult = bmpShadow.CreateEmptyBitmap(Math.Abs(offset.X), Math.Abs(offset.Y));

                    using (Graphics g = Graphics.FromImage(bmpResult))
                    {
                        g.SetHighQuality();
                        g.DrawImage(bmpShadow, Math.Max(0, offset.X), Math.Max(0, offset.Y), bmpShadow.Width, bmpShadow.Height);
                        g.DrawImage(bmp, Math.Max(size, -offset.X + size), Math.Max(size, -offset.Y + size), bmp.Width, bmp.Height);
                    }
                }
                else
                {
                    bmpResult = bmp.CreateEmptyBitmap();

                    using (Graphics g = Graphics.FromImage(bmpResult))
                    {
                        g.SetHighQuality();
                        g.DrawImage(bmpShadow, -size + offset.X, -size + offset.Y, bmpShadow.Width, bmpShadow.Height);
                        g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                    }
                }

                return bmpResult;
            }
            finally
            {
                bmp?.Dispose();
                bmpShadow?.Dispose();
            }
        }

        public static Bitmap AddGlow(Bitmap bmp, int size, float strength, Color color, Point offset, GradientInfo gradient = null)
        {
            if (size < 0 || strength < 0.1f)
            {
                return bmp;
            }

            Bitmap bmpBlur = null, bmpMask = null;

            try
            {
                if (size > 0)
                {
                    bmpBlur = AddCanvas(bmp, size);
                    BoxBlur(bmpBlur, size);
                }
                else
                {
                    bmpBlur = bmp;
                }

                if (gradient != null && gradient.IsValid)
                {
                    bmpMask = CreateGradientMask(bmpBlur, gradient, strength);
                }
                else
                {
                    bmpMask = ColorMatrixManager.Mask(strength, color).Apply(bmpBlur);
                }

                Bitmap bmpResult = bmpMask.CreateEmptyBitmap(Math.Abs(offset.X), Math.Abs(offset.Y));

                using (Graphics g = Graphics.FromImage(bmpResult))
                {
                    g.SetHighQuality();
                    g.DrawImage(bmpMask, Math.Max(0, offset.X), Math.Max(0, offset.Y), bmpMask.Width, bmpMask.Height);
                    g.DrawImage(bmp, Math.Max(size, -offset.X + size), Math.Max(size, -offset.Y + size), bmp.Width, bmp.Height);
                }

                return bmpResult;
            }
            finally
            {
                bmp?.Dispose();
                bmpBlur?.Dispose();
                bmpMask?.Dispose();
            }
        }

        public static Bitmap CreateGradientMask(Bitmap bmp, GradientInfo gradient, float opacity = 1f)
        {
            Bitmap mask = bmp.CreateEmptyBitmap();

            if (opacity <= 0)
            {
                return mask;
            }

            gradient.Draw(mask);

            using (UnsafeBitmap bmpSource = new UnsafeBitmap(bmp, true, ImageLockMode.ReadOnly))
            using (UnsafeBitmap bmpMask = new UnsafeBitmap(mask, true, ImageLockMode.ReadWrite))
            {
                int pixelCount = bmpSource.PixelCount;

                for (int i = 0; i < pixelCount; i++)
                {
                    ColorBgra sourceColor = bmpSource.GetPixel(i);
                    ColorBgra maskColor = bmpMask.GetPixel(i);
                    maskColor.Alpha = (byte)Math.Min(255, sourceColor.Alpha * (maskColor.Alpha / 255f) * opacity);
                    bmpMask.SetPixel(i, maskColor);
                }
            }

            return mask;
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

        public static void Pixelate(Bitmap bmp, int pixelSize, int borderSize, Color borderColor)
        {
            Pixelate(bmp, pixelSize);

            if (pixelSize > 1 && borderSize > 0 && borderColor.A > 0)
            {
                using (Bitmap bmpTexture = new Bitmap(pixelSize, pixelSize))
                {
                    using (Graphics g = Graphics.FromImage(bmpTexture))
                    using (Pen pen = new Pen(borderColor, borderSize) { Alignment = PenAlignment.Inset })
                    {
                        g.DrawRectangleProper(pen, new Rectangle(0, 0, bmpTexture.Width, bmpTexture.Height));
                    }

                    using (Graphics g = Graphics.FromImage(bmp))
                    using (TextureBrush brush = new TextureBrush(bmpTexture))
                    {
                        g.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
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

        public static Bitmap GaussianBlur(Bitmap bmp, int radius)
        {
            int size = radius * 2 + 1;
            double sigma = radius / 3.0;

            ConvolutionMatrix kernelHorizontal = ConvolutionMatrixManager.GaussianBlur(1, size, sigma);

            ConvolutionMatrix kernelVertical = new ConvolutionMatrix(size, 1)
            {
                ConsiderAlpha = kernelHorizontal.ConsiderAlpha
            };

            for (int i = 0; i < size; i++)
            {
                kernelVertical[i, 0] = kernelHorizontal[0, i];
            }

            using (Bitmap horizontalPass = kernelHorizontal.Apply(bmp))
            {
                return kernelVertical.Apply(horizontalPass);
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

        public static Bitmap WavyEdges(Bitmap bmp, int waveDepth, int waveRange, AnchorStyles sides)
        {
            return WavyEdges(bmp, waveDepth, waveRange, sides, Color.Transparent);
        }

        public static Bitmap WavyEdges(Bitmap bmp, int waveDepth, int waveRange, AnchorStyles sides, Color backgroundColor)
        {
            if (waveDepth < 1 || waveRange < 1 || sides == AnchorStyles.None)
            {
                return bmp;
            }

            List<Point> points = new List<Point>();

            int horizontalWaveCount = Math.Max(2, (bmp.Width / waveRange + 1) / 2 * 2) - 1;
            int verticalWaveCount = Math.Max(2, (bmp.Height / waveRange + 1) / 2 * 2) - 1;
            int horizontalWaveRange = bmp.Width / horizontalWaveCount;
            int verticalWaveRange = bmp.Height / verticalWaveCount;

            int step = Math.Min(Math.Max(1, waveRange / waveDepth), 10);

            int waveFunction(int t, int max, int depth) => (int)((1 - Math.Cos(t * Math.PI / max)) * depth / 2);

            if (sides.HasFlag(AnchorStyles.Top))
            {
                int startX = sides.HasFlag(AnchorStyles.Left) ? waveDepth : 0;
                int endX = sides.HasFlag(AnchorStyles.Right) ? bmp.Width - waveDepth : bmp.Width;
                for (int x = startX; x < endX; x += step)
                {
                    points.Add(new Point(x, waveFunction(x, horizontalWaveRange, waveDepth)));
                }
                points.Add(new Point(endX, waveFunction(endX, horizontalWaveRange, waveDepth)));
            }
            else
            {
                points.Add(new Point(0, 0));
            }

            if (sides.HasFlag(AnchorStyles.Right))
            {
                int startY = sides.HasFlag(AnchorStyles.Top) ? waveDepth : 0;
                int endY = sides.HasFlag(AnchorStyles.Bottom) ? bmp.Height - waveDepth : bmp.Height;
                for (int y = startY; y < endY; y += step)
                {
                    points.Add(new Point(bmp.Width - waveDepth + waveFunction(y, verticalWaveRange, waveDepth), y));
                }
                points.Add(new Point(bmp.Width - waveDepth + waveFunction(endY, verticalWaveRange, waveDepth), endY));
            }
            else
            {
                points.Add(new Point(bmp.Width, points[points.Count - 1].Y));
            }

            if (sides.HasFlag(AnchorStyles.Bottom))
            {
                int startX = sides.HasFlag(AnchorStyles.Right) ? bmp.Width - waveDepth : bmp.Width;
                int endX = sides.HasFlag(AnchorStyles.Left) ? waveDepth : 0;
                for (int x = startX; x >= endX; x -= step)
                {
                    points.Add(new Point(x, bmp.Height - waveDepth + waveFunction(x, horizontalWaveRange, waveDepth)));
                }
                points.Add(new Point(endX, bmp.Height - waveDepth + waveFunction(endX, horizontalWaveRange, waveDepth)));
            }
            else
            {
                points.Add(new Point(points[points.Count - 1].X, bmp.Height));
            }

            if (sides.HasFlag(AnchorStyles.Left))
            {
                int startY = sides.HasFlag(AnchorStyles.Bottom) ? bmp.Height - waveDepth : bmp.Height;
                int endY = sides.HasFlag(AnchorStyles.Top) ? waveDepth : 0;
                for (int y = startY; y >= endY; y -= step)
                {
                    points.Add(new Point(waveFunction(y, verticalWaveRange, waveDepth), y));
                }
                points.Add(new Point(waveFunction(endY, verticalWaveRange, waveDepth), endY));
            }
            else
            {
                points.Add(new Point(0, points[points.Count - 1].Y));
            }

            if (!sides.HasFlag(AnchorStyles.Top))
            {
                points[0] = new Point(points[points.Count - 1].X, 0);
            }

            Bitmap bmpResult = bmp.CreateEmptyBitmap();

            using (bmp)
            using (Graphics g = Graphics.FromImage(bmpResult))
            using (TextureBrush brush = new TextureBrush(bmp))
            {
                if (backgroundColor.A > 0)
                {
                    g.Clear(backgroundColor);
                }

                g.SetHighQuality();
                g.PixelOffsetMode = PixelOffsetMode.Half;

                g.FillPolygon(brush, points.ToArray());
            }

            return bmpResult;
        }

        public static Bitmap TornEdges(Bitmap bmp, int tornDepth, int tornRange, AnchorStyles sides, bool curvedEdges, bool random)
        {
            return TornEdges(bmp, tornDepth, tornRange, sides, curvedEdges, random, Color.Transparent);
        }

        public static Bitmap TornEdges(Bitmap bmp, int tornDepth, int tornRange, AnchorStyles sides, bool curvedEdges, bool random, Color backgroundColor)
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
                int startX = (sides.HasFlag(AnchorStyles.Left) && verticalTornCount > 1) ? tornDepth : 0;
                int endX = (sides.HasFlag(AnchorStyles.Right) && verticalTornCount > 1) ? bmp.Width - tornDepth : bmp.Width;
                for (int x = startX; x < endX; x += tornRange)
                {
                    int y = random ? RandomFast.Next(0, tornDepth) : ((x / tornRange) & 1) * tornDepth;
                    points.Add(new Point(x, y));
                }
            }
            else
            {
                points.Add(new Point(0, 0));
                points.Add(new Point(bmp.Width, 0));
            }

            if (sides.HasFlag(AnchorStyles.Right) && verticalTornCount > 1)
            {
                int startY = (sides.HasFlag(AnchorStyles.Top) && horizontalTornCount > 1) ? tornDepth : 0;
                int endY = (sides.HasFlag(AnchorStyles.Bottom) && horizontalTornCount > 1) ? bmp.Height - tornDepth : bmp.Height;
                for (int y = startY; y < endY; y += tornRange)
                {
                    int x = random ? RandomFast.Next(0, tornDepth) : ((y / tornRange) & 1) * tornDepth;
                    points.Add(new Point(bmp.Width - tornDepth + x, y));
                }
            }
            else
            {
                points.Add(new Point(bmp.Width, 0));
                points.Add(new Point(bmp.Width, bmp.Height));
            }

            if (sides.HasFlag(AnchorStyles.Bottom) && horizontalTornCount > 1)
            {
                int startX = (sides.HasFlag(AnchorStyles.Right) && verticalTornCount > 1) ? bmp.Width - tornDepth : bmp.Width;
                int endX = (sides.HasFlag(AnchorStyles.Left) && verticalTornCount > 1) ? tornDepth : 0;
                for (int x = startX; x >= endX; x = (x / tornRange - 1) * tornRange)
                {
                    int y = random ? RandomFast.Next(0, tornDepth) : ((x / tornRange) & 1) * tornDepth;
                    points.Add(new Point(x, bmp.Height - tornDepth + y));
                }
            }
            else
            {
                points.Add(new Point(bmp.Width, bmp.Height));
                points.Add(new Point(0, bmp.Height));
            }

            if (sides.HasFlag(AnchorStyles.Left) && verticalTornCount > 1)
            {
                int startY = (sides.HasFlag(AnchorStyles.Bottom) && horizontalTornCount > 1) ? bmp.Height - tornDepth : bmp.Height;
                int endY = (sides.HasFlag(AnchorStyles.Top) && horizontalTornCount > 1) ? tornDepth : 0;
                for (int y = startY; y >= endY; y = (y / tornRange - 1) * tornRange)
                {
                    int x = random ? RandomFast.Next(0, tornDepth) : ((y / tornRange) & 1) * tornDepth;
                    points.Add(new Point(x, y));
                }
            }
            else
            {
                points.Add(new Point(0, bmp.Height));
                points.Add(new Point(0, 0));
            }

            Bitmap bmpResult = bmp.CreateEmptyBitmap();

            using (bmp)
            using (Graphics g = Graphics.FromImage(bmpResult))
            using (TextureBrush brush = new TextureBrush(bmp))
            {
                if (backgroundColor.A > 0)
                {
                    g.Clear(backgroundColor);
                }

                g.SetHighQuality();
                g.PixelOffsetMode = PixelOffsetMode.Half;

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
            string ext = FileHelpers.GetFileNameExtension(filePath);

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
            FileHelpers.CreateDirectoryFromFilePath(filePath);
            ImageFormat imageFormat = GetImageFormat(filePath);

            try
            {
                img.Save(filePath, imageFormat);
                return true;
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
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

                    string ext = FileHelpers.GetFileNameExtension(filePath);

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
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    filePath = FileHelpers.GetAbsolutePath(filePath);

                    if (!string.IsNullOrEmpty(filePath) && FileHelpers.IsImageFile(filePath) && File.Exists(filePath))
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
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return null;
        }

        public static Bitmap LoadImageWithFileDialog(Form form = null)
        {
            string filePath = OpenImageFileDialog(form);

            if (!string.IsNullOrEmpty(filePath))
            {
                return LoadImage(filePath);
            }

            return null;
        }

        public static Bitmap CombineImages(List<Bitmap> images, Orientation orientation, ImageCombinerAlignment alignment = ImageCombinerAlignment.LeftOrTop,
            int space = 0, int wrapAfter = 0, bool autoFillBackground = false)
        {
            int imageCount = images.Count;
            Rectangle[] imageRects = new Rectangle[imageCount];
            Point position = new Point(0, 0);
            int currentSize = 0;

            for (int i = 0; i < imageCount; i++)
            {
                Bitmap image = images[i];
                Point offset = new Point(0, 0);

                if (orientation == Orientation.Horizontal)
                {
                    if (wrapAfter > 0)
                    {
                        if (i % wrapAfter == 0)
                        {
                            if (i > 0)
                            {
                                position.X = 0;
                                position.Y += currentSize + space;
                            }

                            currentSize = images.Skip(i).Take(wrapAfter).Max(x => x.Height);
                        }
                    }
                    else if (i == 0)
                    {
                        currentSize = images.Max(x => x.Height);
                    }

                    switch (alignment)
                    {
                        default:
                        case ImageCombinerAlignment.LeftOrTop:
                            offset.Y = 0;
                            break;
                        case ImageCombinerAlignment.Center:
                            offset.Y = (currentSize / 2) - (image.Height / 2);
                            break;
                        case ImageCombinerAlignment.RightOrBottom:
                            offset.Y = currentSize - image.Height;
                            break;
                    }

                    imageRects[i] = new Rectangle(position.X + offset.X, position.Y + offset.Y, image.Width, image.Height);
                    position.X += image.Width + space;
                }
                else
                {
                    if (wrapAfter > 0)
                    {
                        if (i % wrapAfter == 0)
                        {
                            if (i > 0)
                            {
                                position.X += currentSize + space;
                                position.Y = 0;
                            }

                            currentSize = images.Skip(i).Take(wrapAfter).Max(x => x.Width);
                        }
                    }
                    else if (i == 0)
                    {
                        currentSize = images.Max(x => x.Width);
                    }

                    switch (alignment)
                    {
                        default:
                        case ImageCombinerAlignment.LeftOrTop:
                            offset.X = 0;
                            break;
                        case ImageCombinerAlignment.Center:
                            offset.X = (currentSize / 2) - (image.Width / 2);
                            break;
                        case ImageCombinerAlignment.RightOrBottom:
                            offset.X = currentSize - image.Width;
                            break;
                    }

                    imageRects[i] = new Rectangle(position.X + offset.X, position.Y + offset.Y, image.Width, image.Height);
                    position.Y += image.Height + space;
                }
            }

            Rectangle totalImageRect = imageRects.Combine();
            Bitmap bmp = new Bitmap(totalImageRect.Width, totalImageRect.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SetHighQuality();

                for (int i = 0; i < imageCount; i++)
                {
                    Bitmap image = images[i];

                    if (autoFillBackground && i == 0)
                    {
                        Color backgroundColor = image.GetPixel(image.Width - 1, image.Height - 1);
                        g.Clear(backgroundColor);
                    }

                    g.DrawImage(image, imageRects[i]);
                }
            }

            return bmp;
        }

        public static Bitmap CombineImages(IEnumerable<string> imageFiles, Orientation orientation, ImageCombinerAlignment alignment = ImageCombinerAlignment.LeftOrTop,
            int space = 0, int wrapAfter = 0, bool autoFillBackground = false)
        {
            List<Bitmap> images = new List<Bitmap>();

            try
            {
                foreach (string filePath in imageFiles)
                {
                    Bitmap bmp = LoadImage(filePath);

                    if (bmp != null)
                    {
                        images.Add(bmp);
                    }
                }

                if (images.Count > 1)
                {
                    return CombineImages(images, orientation, alignment, space, wrapAfter, autoFillBackground);
                }
            }
            finally
            {
                foreach (Bitmap bmp in images)
                {
                    if (bmp != null)
                    {
                        bmp.Dispose();
                    }
                }
            }

            return null;
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
            if (color.IsTransparent())
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
            AnchorStyles sides = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, int padding = 0)
        {
            Rectangle source = new Rectangle(0, 0, bmp.Width, bmp.Height);
            Rectangle rect = FindAutoCropRectangle(bmp, sameColorCrop, sides);

            if (source != rect)
            {
                Bitmap croppedBitmap = CropBitmap(bmp, rect);

                if (croppedBitmap != null)
                {
                    using (bmp)
                    {
                        if (padding > 0)
                        {
                            using (croppedBitmap)
                            {
                                Color color = bmp.GetPixel(0, 0);
                                return AddCanvas(croppedBitmap, padding, color);
                            }
                        }

                        return croppedBitmap;
                    }
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

        public static void ReplaceColor(Bitmap bmp, Color sourceColor, Color targetColor, bool autoSourceColor = false, int threshold = 0)
        {
            ColorBgra sourceBgra = new ColorBgra(sourceColor);
            ColorBgra targetBgra = new ColorBgra(targetColor);

            using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bmp, true))
            {
                if (autoSourceColor)
                {
                    sourceBgra = unsafeBitmap.GetPixel(0);
                    sourceColor = sourceBgra.ToColor();
                }

                for (int i = 0; i < unsafeBitmap.PixelCount; i++)
                {
                    ColorBgra color = unsafeBitmap.GetPixel(i);

                    if (threshold == 0)
                    {
                        if (color == sourceBgra)
                        {
                            unsafeBitmap.SetPixel(i, targetBgra);
                        }
                    }
                    else if (ColorHelpers.ColorsAreClose(color.ToColor(), sourceColor, threshold))
                    {
                        unsafeBitmap.SetPixel(i, targetBgra);
                    }
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

        public static Bitmap NonIndexedBitmap(Bitmap bmp)
        {
            if (bmp != null && bmp.PixelFormat.HasFlag(PixelFormat.Indexed))
            {
                using (bmp)
                {
                    return bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format32bppArgb);
                }
            }

            return bmp;
        }

        public static Bitmap DrawGrip(Color color, Color shadow)
        {
            int size = 16;
            Bitmap bmp = new Bitmap(size, size);

            using (Graphics g = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(color))
            using (SolidBrush shadowBrush = new SolidBrush(shadow))
            {
                int x = size / 2;
                int boxSize = 2;

                for (int i = 0; i < 4; i++)
                {
                    g.FillRectangle(shadowBrush, x - boxSize, (i * 4) + 2, boxSize, boxSize);
                    g.FillRectangle(brush, x - boxSize - 1, (i * 4) + 1, boxSize, boxSize);

                    g.FillRectangle(shadowBrush, x + 2, (i * 4) + 2, boxSize, boxSize);
                    g.FillRectangle(brush, x + 1, (i * 4) + 1, boxSize, boxSize);
                }
            }

            return bmp;
        }

        public static MemoryStream SavePNG(Image img, PNGBitDepth bitDepth)
        {
            MemoryStream ms = new MemoryStream();
            SavePNG(img, ms, bitDepth);
            return ms;
        }

        public static void SavePNG(Image img, Stream stream, PNGBitDepth bitDepth)
        {
            if (bitDepth == PNGBitDepth.Automatic)
            {
                if (IsImageTransparent((Bitmap)img))
                {
                    bitDepth = PNGBitDepth.Bit32;
                }
                else
                {
                    bitDepth = PNGBitDepth.Bit24;
                }
            }

            if (bitDepth == PNGBitDepth.Bit32)
            {
                if (img.PixelFormat != PixelFormat.Format32bppArgb && img.PixelFormat != PixelFormat.Format32bppRgb)
                {
                    using (Bitmap bmpNew = ((Bitmap)img).Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format32bppArgb))
                    {
                        bmpNew.Save(stream, ImageFormat.Png);
                        return;
                    }
                }
            }
            else if (bitDepth == PNGBitDepth.Bit24)
            {
                if (img.PixelFormat != PixelFormat.Format24bppRgb)
                {
                    using (Bitmap bmpNew = ((Bitmap)img).Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb))
                    {
                        bmpNew.Save(stream, ImageFormat.Png);
                        return;
                    }
                }
            }

            img.Save(stream, ImageFormat.Png);
        }

        public static MemoryStream PNGStripChunks(MemoryStream stream, params string[] chunks)
        {
            MemoryStream output = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);

            byte[] signature = new byte[8];
            stream.Read(signature, 0, 8);
            output.Write(signature, 0, 8);

            while (true)
            {
                byte[] lenBytes = new byte[4];
                if (stream.Read(lenBytes, 0, 4) != 4)
                {
                    break;
                }

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lenBytes);
                }

                int len = BitConverter.ToInt32(lenBytes, 0);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lenBytes);
                }

                byte[] type = new byte[4];
                stream.Read(type, 0, 4);

                byte[] data = new byte[len + 4];
                stream.Read(data, 0, data.Length);

                string strType = Encoding.ASCII.GetString(type);

                if (!chunks.Contains(strType))
                {
                    output.Write(lenBytes, 0, lenBytes.Length);
                    output.Write(type, 0, type.Length);
                    output.Write(data, 0, data.Length);
                }
            }

            return output;
        }

        public static MemoryStream PNGStripColorSpaceInformation(MemoryStream stream)
        {
            // http://www.libpng.org/pub/png/spec/1.2/PNG-Chunks.html
            // 4.2.2.1. gAMA Image gamma
            // 4.2.2.2. cHRM Primary chromaticities
            // 4.2.2.3. sRGB Standard RGB color space
            // 4.2.2.4. iCCP Embedded ICC profile
            return PNGStripChunks(stream, "gAMA", "cHRM", "sRGB", "iCCP");
        }

        public static MemoryStream SaveJPEG(Image img, int quality)
        {
            MemoryStream ms = new MemoryStream();
            SaveJPEG(img, ms, quality);
            return ms;
        }

        public static void SaveJPEG(Image img, string filePath, int quality)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                SaveJPEG(img, fs, quality);
            }
        }

        public static void SaveJPEG(Image img, Stream stream, int quality)
        {
            quality = quality.Clamp(0, 100);

            using (EncoderParameters encoderParameters = new EncoderParameters(1))
            {
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                img.Save(stream, ImageFormat.Jpeg.GetCodecInfo(), encoderParameters);
            }
        }

        public static MemoryStream SaveJPEGAutoQuality(Image img, int sizeLimit, int qualityDecrement = 5, int minQuality = 0, int maxQuality = 100)
        {
            qualityDecrement = qualityDecrement.Clamp(1, 100);
            minQuality = minQuality.Clamp(0, 100);
            maxQuality = maxQuality.Clamp(0, 100);

            if (minQuality >= maxQuality)
            {
                return SaveJPEG(img, minQuality);
            }

            MemoryStream ms = null;

            for (int quality = maxQuality; quality >= minQuality; quality -= qualityDecrement)
            {
                if (ms != null)
                {
                    ms.Dispose();
                }

                ms = SaveJPEG(img, quality);

                //DebugHelper.WriteLine($"Quality: {quality}% - Size: {ms.Length.ToSizeString()}");

                if (ms.Length <= sizeLimit)
                {
                    break;
                }
            }

            return ms;
        }

        public static MemoryStream SaveGIF(Image img, GIFQuality quality)
        {
            MemoryStream ms = new MemoryStream();
            SaveGIF(img, ms, quality);
            return ms;
        }

        public static void SaveGIF(Image img, Stream stream, GIFQuality quality)
        {
            if (quality == GIFQuality.Default)
            {
                img.Save(stream, ImageFormat.Gif);
            }
            else
            {
                Quantizer quantizer;

                switch (quality)
                {
                    case GIFQuality.Grayscale:
                        quantizer = new GrayscaleQuantizer();
                        break;
                    case GIFQuality.Bit4:
                        quantizer = new OctreeQuantizer(15, 4);
                        break;
                    default:
                    case GIFQuality.Bit8:
                        quantizer = new OctreeQuantizer(255, 4);
                        break;
                }

                using (Bitmap quantized = quantizer.Quantize(img))
                {
                    quantized.Save(stream, ImageFormat.Gif);
                }
            }
        }
    }
}