#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib
{
    public static partial class SkiaImageHelpers
    {
        public static SKBitmap ResizeImageLimit(SKBitmap bmp, int width, int height)
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

        public static SKBitmap ResizeImageLimit(SKBitmap bmp, Size size)
        {
            return ResizeImageLimit(bmp, size.Width, size.Height);
        }

        public static SKBitmap ResizeImageLimit(SKBitmap bmp, int size)
        {
            return ResizeImageLimit(bmp, size, size);
        }

        private static SKBitmap ApplyCutOutEffect(SKBitmap bmp, ImageSides effectEdge, CutOutEffectType effectType, int effectSize, Color backgroundColor)
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

        public static SKBitmap CutOutBitmapMiddle(SKBitmap bmp, ImageOrientation orientation, int start, int size, CutOutEffectType effectType, int effectSize, Color backgroundColor)
        {
            if (bmp != null && size > 0)
            {
                SKBitmap firstPart = null, secondPart = null;

                if (start > 0)
                {
                    Rectangle r = orientation == ImageOrientation.Horizontal
                        ? new Rectangle(0, 0, Math.Min(start, bmp.Width), bmp.Height)
                        : new Rectangle(0, 0, bmp.Width, Math.Min(start, bmp.Height));
                    firstPart = CropBitmap(bmp, r);
                    ImageSides effectEdge = orientation == ImageOrientation.Horizontal ? ImageSides.Right : ImageSides.Bottom;
                    firstPart = ApplyCutOutEffect(firstPart, effectEdge, effectType, effectSize, backgroundColor);
                }

                int cutDimension = orientation == ImageOrientation.Horizontal ? bmp.Width : bmp.Height;
                if (start + size < cutDimension)
                {
                    int end = Math.Max(start + size, 0);
                    Rectangle r = orientation == ImageOrientation.Horizontal
                        ? new Rectangle(end, 0, bmp.Width - end, bmp.Height)
                        : new Rectangle(0, end, bmp.Width, bmp.Height - end);
                    secondPart = CropBitmap(bmp, r);
                    ImageSides effectEdge = orientation == ImageOrientation.Horizontal ? ImageSides.Left : ImageSides.Top;
                    secondPart = ApplyCutOutEffect(secondPart, effectEdge, effectType, effectSize, backgroundColor);
                }

                if (firstPart != null && secondPart != null)
                {
                    return CombineImages(new List<SKBitmap> { firstPart, secondPart }, orientation);
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

        public static SKBitmap AutoCropTransparent(SKBitmap bmp)
        {
            Rectangle source = new Rectangle(0, 0, bmp.Width, bmp.Height);
            Rectangle rect = source;

            using (SkiaPixelBuffer unsafeBitmap = new SkiaPixelBuffer(bmp, true, PixelAccess.ReadOnly))
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
                SKBitmap croppedBitmap = CropBitmap(bmp, rect);

                if (croppedBitmap != null)
                {
                    bmp.Dispose();
                    return croppedBitmap;
                }
            }

            return bmp;
        }

        public static SKBitmap QuickAutoCropTransparent(SKBitmap bmp)
        {
            Rectangle source = new Rectangle(0, 0, bmp.Width, bmp.Height);
            Rectangle rect = source;

            using (SkiaPixelBuffer unsafeBitmap = new SkiaPixelBuffer(bmp, true, PixelAccess.ReadOnly))
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
                SKBitmap croppedBitmap = CropBitmap(bmp, rect);

                if (croppedBitmap != null)
                {
                    bmp.Dispose();
                    return croppedBitmap;
                }
            }

            return bmp;
        }

        public static SKBitmap AddCanvas(SKBitmap img, int margin)
        {
            return AddCanvas(img, new ImageMargins(margin));
        }

        public static SKBitmap AddCanvas(SKBitmap img, int margin, Color canvasColor)
        {
            return AddCanvas(img, new ImageMargins(margin), canvasColor);
        }

        public static SKBitmap AddCanvas(SKBitmap img, ImageMargins margin)
        {
            return AddCanvas(img, margin, Color.Transparent);
        }

        public static SKBitmap AddCanvas(SKBitmap img, ImageMargins margin, Color canvasColor)
        {
            if (margin.All == 0 || img.Width + margin.Horizontal < 1 || img.Height + margin.Vertical < 1)
            {
                return null;
            }

            SKBitmap bmp = img.CreateEmptyBitmap(margin.Horizontal, margin.Vertical);

            using (SKCanvas g = new SKCanvas(bmp))
            {
                g.DrawImage(img, margin.Left, margin.Top, img.Width, img.Height);

                if (canvasColor.A > 0)
                {

                    using (SKPaint brush = SkiaDrawing.Fill(canvasColor))
                    {
                        brush.BlendMode = SKBlendMode.Src;
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

        public static void DrawImageCentered(SKBitmap bmp1, SKBitmap bmp2)
        {
            using (SKCanvas g = new SKCanvas(bmp1))
            {

                int x = (bmp1.Width - bmp2.Width) / 2;
                int y = (bmp1.Height - bmp2.Height) / 2;
                g.DrawImage(bmp2, x, y, bmp2.Width, bmp2.Height);
            }
        }

        public static SKBitmap DrawBackgroundImage(SKBitmap bmp, SKBitmap backgroundImage, bool center = true, bool tile = false)
        {
            if (bmp != null && backgroundImage != null)
            {
                using (bmp)
                using (backgroundImage)
                {
                    SKBitmap bmpResult = bmp.CreateEmptyBitmap();

                    using (SKCanvas g = new SKCanvas(bmpResult))
                    {

                        if (tile)
                        {
                            using (SKPaint brush = SkiaDrawing.Texture(backgroundImage, ImageTileMode.Tile))
                            {
                                if (center)
                                {
                                    int tileX = (bmpResult.Width - backgroundImage.Width) / 2 % backgroundImage.Width;
                                    int tileY = (bmpResult.Height - backgroundImage.Height) / 2 % backgroundImage.Height;

                                    brush.Translate(tileX, tileY);
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

        public static SKBitmap DrawBackgroundImage(SKBitmap bmp, string backgroundImageFilePath, bool center = true, bool tile = false)
        {
            SKBitmap backgroundImage = LoadImage(backgroundImageFilePath);
            return DrawBackgroundImage(bmp, backgroundImage, center, tile);
        }

        public static SKBitmap RoundedCorners(SKBitmap bmp, int cornerRadius)
        {
            SKBitmap bmpResult = bmp.CreateEmptyBitmap();

            using (bmp)
            using (SKCanvas g = new SKCanvas(bmpResult))
            {

                using (SKPath gp = new SKPath())
                {
                    gp.AddRoundedRectangleProper(new RectangleF(0, 0, bmp.Width, bmp.Height), cornerRadius, 0);

                    using (SKPaint brush = SkiaDrawing.Texture(bmp))
                    {
                        g.FillPath(brush, gp);
                    }
                }
            }

            return bmpResult;
        }

        public static SKBitmap OutlineOld(SKBitmap bmp, int borderSize, Color borderColor)
        {
            SKBitmap bmpResult = bmp.CreateEmptyBitmap(borderSize * 2, borderSize * 2);

            ImageColorMatrix maskMatrix = new ImageColorMatrix();
            maskMatrix.Matrix00 = 0;
            maskMatrix.Matrix11 = 0;
            maskMatrix.Matrix22 = 0;
            maskMatrix.Matrix33 = 1;
            maskMatrix.Matrix40 = ((float)borderColor.R).Remap(0, 255, 0, 1);
            maskMatrix.Matrix41 = ((float)borderColor.G).Remap(0, 255, 0, 1);
            maskMatrix.Matrix42 = ((float)borderColor.B).Remap(0, 255, 0, 1);

            using (bmp)
            using (SKBitmap shadow = maskMatrix.Apply(bmp))
            using (SKCanvas g = new SKCanvas(bmpResult))
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

        public static SKBitmap Outline(SKBitmap bmp, int borderSize, Color borderColor, int padding = 0, bool outlineOnly = false)
        {
            SKBitmap outline = MakeOutline(bmp, padding, padding + borderSize + 1, borderColor);

            if (outlineOnly)
            {
                bmp.Dispose();
                return outline;
            }
            else
            {
                using (outline)
                using (SKCanvas g = new SKCanvas(bmp))
                {
                    g.DrawImage(outline, 0, 0, outline.Width, outline.Height);
                }

                return bmp;
            }
        }

        public static SKBitmap MakeOutline(SKBitmap bmp, int minRadius, int maxRadius, Color color)
        {
            SKBitmap bmpResult = bmp.CreateEmptyBitmap();

            using (SkiaPixelBuffer source = new SkiaPixelBuffer(bmp, true, PixelAccess.ReadOnly))
            using (SkiaPixelBuffer dest = new SkiaPixelBuffer(bmpResult, true, PixelAccess.WriteOnly))
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

        private static float DistanceToThreshold(SkiaPixelBuffer unsafeBitmap, int x, int y, int radius, int threshold)
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

        public static SKBitmap DrawReflection(SKBitmap bmp, int percentage, int maxAlpha, int minAlpha, int offset, bool skew, int skewSize)
        {
            SKBitmap reflection = AddReflection(bmp, percentage, maxAlpha, minAlpha);

            if (skew)
            {
                reflection = AddSkew(reflection, skewSize, 0);
            }

            SKBitmap bmpResult = CreateBitmap(reflection.Width, bmp.Height + reflection.Height + offset);

            using (bmp)
            using (reflection)
            using (SKCanvas g = new SKCanvas(bmpResult))
            {
                g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                g.DrawImage(reflection, 0, bmp.Height + offset, reflection.Width, reflection.Height);
            }

            return bmpResult;
        }

        public static SKBitmap AddSkew(SKBitmap img, int x, int y)
        {
            SKBitmap result = img.CreateEmptyBitmap(Math.Abs(x), Math.Abs(y));

            using (img)
            using (SKCanvas g = new SKCanvas(result))
            {

                int startX = -Math.Min(0, x);
                int startY = -Math.Min(0, y);
                int endX = Math.Max(0, x);
                int endY = Math.Max(0, y);
                Point[] destinationPoints = { new Point(startX, startY), new Point(startX + img.Width - 1, endY), new Point(endX, startY + img.Height - 1) };
                g.DrawImage(img, destinationPoints);
            }

            return result;
        }

        public static SKBitmap DrawBorder(SKBitmap bmp, Color borderColor, int borderSize, BorderType borderType, ImageDashStyle dashStyle = ImageDashStyle.Solid)
        {
            using (SKPaint borderPen = SkiaDrawing.Stroke(borderColor, borderSize))
            {
                borderPen.PathEffect = SkiaDrawing.DashEffect(dashStyle, borderSize);
                return DrawBorder(bmp, borderPen, borderType);
            }
        }

        public static SKBitmap DrawBorder(SKBitmap bmp, Color fromBorderColor, Color toBorderColor, ImageGradientMode gradientType, int borderSize, BorderType borderType,
            ImageDashStyle dashStyle = ImageDashStyle.Solid)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            if (borderType == BorderType.Outside)
            {
                width += borderSize * 2;
                height += borderSize * 2;
            }

            using (SKPaint brush = SkiaDrawing.Gradient(new Rectangle(0, 0, width, height), fromBorderColor, toBorderColor, gradientType))
            using (SKPaint borderPen = SkiaDrawing.Stroke(brush, borderSize))
            {
                borderPen.PathEffect = SkiaDrawing.DashEffect(dashStyle, borderSize);
                return DrawBorder(bmp, borderPen, borderType);
            }
        }

        public static SKBitmap DrawBorder(SKBitmap bmp, GradientInfo gradientInfo, int borderSize, BorderType borderType, ImageDashStyle dashStyle = ImageDashStyle.Solid)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            if (borderType == BorderType.Outside)
            {
                width += borderSize * 2;
                height += borderSize * 2;
            }

            using (SKPaint brush = gradientInfo.GetSkiaPaint(new Rectangle(0, 0, width, height)))
            using (SKPaint borderPen = SkiaDrawing.Stroke(brush, borderSize))
            {
                borderPen.PathEffect = SkiaDrawing.DashEffect(dashStyle, borderSize);
                return DrawBorder(bmp, borderPen, borderType);
            }
        }

        public static SKBitmap DrawBorder(SKBitmap bmp, SKPaint borderPen, BorderType borderType)
        {
            SKBitmap bmpResult;

            if (borderType == BorderType.Inside)
            {
                bmpResult = bmp;

                using (SKCanvas g = new SKCanvas(bmpResult))
                {
                    g.DrawRectangleProper(borderPen, 0, 0, bmp.Width, bmp.Height);
                }
            }
            else
            {
                int borderSize = (int)borderPen.StrokeWidth;
                bmpResult = bmp.CreateEmptyBitmap(borderSize * 2, borderSize * 2);

                using (bmp)
                using (SKCanvas g = new SKCanvas(bmpResult))
                {
                    g.DrawRectangleProper(borderPen, 0, 0, bmpResult.Width, bmpResult.Height);
                    g.DrawImage(bmp, borderSize, borderSize, bmp.Width, bmp.Height);
                }
            }

            return bmpResult;
        }

        public static SKBitmap CreateBitmap(int width, int height, Color color)
        {
            if (width > 0 && height > 0)
            {
                SKBitmap bmp = CreateBitmap(width, height);

                using (SKCanvas g = new SKCanvas(bmp))
                {
                    g.Clear(color);
                }

                return bmp;
            }

            return null;
        }

        public static SKBitmap FillBackground(SKBitmap img, Color fromColor, Color toColor, ImageGradientMode gradientType)
        {
            using (SKPaint brush = SkiaDrawing.Gradient(new Rectangle(0, 0, img.Width, img.Height), fromColor, toColor, gradientType))
            {
                return FillBackground(img, brush);
            }
        }

        public static SKBitmap FillBackground(SKBitmap img, GradientInfo gradientInfo)
        {
            using (SKPaint brush = gradientInfo.GetSkiaPaint(new Rectangle(0, 0, img.Width, img.Height)))
            {
                return FillBackground(img, brush);
            }
        }

        public static SKBitmap FillBackground(SKBitmap img, SKPaint brush)
        {
            SKBitmap result = img.CreateEmptyBitmap();

            using (SKCanvas g = new SKCanvas(result))
            {
                g.FillRectangle(brush, 0, 0, result.Width, result.Height);
                g.DrawImage(img, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        public static SKBitmap DrawCheckers(SKBitmap img)
        {
            return DrawCheckers(img, 10, SystemColors.ControlLight, SystemColors.ControlLightLight);
        }

        public static SKBitmap DrawCheckers(SKBitmap img, int checkerSize, Color checkerColor1, Color checkerColor2)
        {
            SKBitmap bmpResult = img.CreateEmptyBitmap();

            using (img)
            using (SKCanvas g = new SKCanvas(bmpResult))
            using (SKBitmap checker = CreateCheckerPattern(checkerSize, checkerSize, checkerColor1, checkerColor2))
            using (SKPaint checkerBrush = SkiaDrawing.Texture(checker, ImageTileMode.Tile))
            {
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmpResult.Width, bmpResult.Height));
                g.DrawImage(img, 0, 0, img.Width, img.Height);
            }

            return bmpResult;
        }

        public static SKBitmap DrawCheckers(int width, int height)
        {
            return DrawCheckers(width, height, 10, SystemColors.ControlLight, SystemColors.ControlLightLight);
        }

        public static SKBitmap DrawCheckers(int width, int height, int checkerSize, Color checkerColor1, Color checkerColor2)
        {
            SKBitmap bmp = CreateBitmap(width, height);

            using (SKCanvas g = new SKCanvas(bmp))
            using (SKBitmap checker = CreateCheckerPattern(checkerSize, checkerSize, checkerColor1, checkerColor2))
            using (SKPaint checkerBrush = SkiaDrawing.Texture(checker, ImageTileMode.Tile))
            {
                g.FillRectangle(checkerBrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }

            return bmp;
        }

        public static SKBitmap CreateCheckerPattern()
        {
            return CreateCheckerPattern(10, 10);
        }

        public static SKBitmap CreateCheckerPattern(int width, int height)
        {
            return CreateCheckerPattern(width, height, SystemColors.ControlLight, SystemColors.ControlLightLight);
        }

        public static SKBitmap CreateCheckerPattern(int width, int height, Color checkerColor1, Color checkerColor2)
        {
            SKBitmap bmp = CreateBitmap(width * 2, height * 2);

            using (SKCanvas g = new SKCanvas(bmp))
            using (SKPaint brush1 = SkiaDrawing.Fill(checkerColor1))
            using (SKPaint brush2 = SkiaDrawing.Fill(checkerColor2))
            {
                g.FillRectangle(brush1, 0, 0, width, height);
                g.FillRectangle(brush1, width, height, width, height);
                g.FillRectangle(brush2, width, 0, width, height);
                g.FillRectangle(brush2, 0, height, width, height);
            }

            return bmp;
        }

        public static SKBitmap RotateImage(SKBitmap bmp, float angleDegrees, bool upsize, bool clip)
        {
            // Test for zero rotation and return a clone of the input image
            if (angleDegrees == 0f)
            {
                return (SKBitmap)bmp.Copy();
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
            SKBitmap bmpResult = CreateBitmap(newWidth, newHeight);

            // Create the SKCanvas object that does the work
            using (SKCanvas g = new SKCanvas(bmpResult))
            {

                // Set up the built-in transformation matrix to do the rotation and maybe scaling
                g.Translate(newWidth / 2f, newHeight / 2f);

                if (scaleFactor != 1f)
                {
                    g.Scale(scaleFactor, scaleFactor);
                }

                g.RotateDegrees(angleDegrees);
                g.Translate(-oldWidth / 2f, -oldHeight / 2f);

                // Draw the result
                g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
            }

            return bmpResult;
        }

        public static SKBitmap AddShadow(SKBitmap bmp, float opacity, int size)
        {
            return AddShadow(bmp, opacity, size, 1, Color.Black, new Point(0, 0));
        }

        public static SKBitmap AddShadow(SKBitmap bmp, float opacity, int size, float darkness, Color color, Point offset, bool autoResize = true)
        {
            SKBitmap bmpShadow = null;

            try
            {
                bmpShadow = bmp.CreateEmptyBitmap(size * 2, size * 2);
                Rectangle shadowRectangle = new Rectangle(size, size, bmp.Width, bmp.Height);
                SkiaColorMatrixManager.Mask(opacity, color).Apply(bmp, bmpShadow, shadowRectangle);

                if (size > 0)
                {
                    BoxBlur(bmpShadow, size);
                }

                if (darkness > 1)
                {
                    SKBitmap shadowImage2 = SkiaColorMatrixManager.Alpha(darkness).Apply(bmpShadow);
                    bmpShadow.Dispose();
                    bmpShadow = shadowImage2;
                }

                SKBitmap bmpResult;

                if (autoResize)
                {
                    bmpResult = bmpShadow.CreateEmptyBitmap(Math.Abs(offset.X), Math.Abs(offset.Y));

                    using (SKCanvas g = new SKCanvas(bmpResult))
                    {
                        g.DrawImage(bmpShadow, Math.Max(0, offset.X), Math.Max(0, offset.Y), bmpShadow.Width, bmpShadow.Height);
                        g.DrawImage(bmp, Math.Max(size, -offset.X + size), Math.Max(size, -offset.Y + size), bmp.Width, bmp.Height);
                    }
                }
                else
                {
                    bmpResult = bmp.CreateEmptyBitmap();

                    using (SKCanvas g = new SKCanvas(bmpResult))
                    {
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

        public static SKBitmap AddGlow(SKBitmap bmp, int size, float strength, Color color, Point offset, GradientInfo gradient = null)
        {
            if (size < 0 || strength < 0.1f)
            {
                return bmp;
            }

            SKBitmap bmpBlur = null, bmpMask = null;

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
                    bmpMask = SkiaColorMatrixManager.Mask(strength, color).Apply(bmpBlur);
                }

                SKBitmap bmpResult = bmpMask.CreateEmptyBitmap(Math.Abs(offset.X), Math.Abs(offset.Y));

                using (SKCanvas g = new SKCanvas(bmpResult))
                {
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

        public static SKBitmap CreateGradientMask(SKBitmap bmp, GradientInfo gradient, float opacity = 1f)
        {
            SKBitmap mask = bmp.CreateEmptyBitmap();

            if (opacity <= 0)
            {
                return mask;
            }

            gradient.DrawSkia(mask);

            using (SkiaPixelBuffer bmpSource = new SkiaPixelBuffer(bmp, true, PixelAccess.ReadOnly))
            using (SkiaPixelBuffer bmpMask = new SkiaPixelBuffer(mask, true, PixelAccess.ReadWrite))
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

        public static unsafe SKBitmap Sharpen(SKBitmap bmp, double strength)
        {
            if (bmp != null)
            {
                using (bmp)
                {
                    SKBitmap sharpenImage = (SKBitmap)bmp.Copy();
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
                    using SkiaPixelBuffer pixels = new(sharpenImage, true);
                    int stride = width * 4;

                    // Declare an array to hold the bytes of the bitmap.
                    int bytes = stride * height;
                    byte[] rgbValues = new byte[bytes];

                    // Copy the RGB values into the array.
                    Marshal.Copy((IntPtr)pixels.Pointer, rgbValues, 0, bytes);

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

                                    rgb = (imageY * stride) + (4 * imageX);

                                    red += rgbValues[rgb + 2] * filter[filterX, filterY];
                                    green += rgbValues[rgb + 1] * filter[filterX, filterY];
                                    blue += rgbValues[rgb + 0] * filter[filterX, filterY];
                                }

                                rgb = (y * stride) + (4 * x);

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
                            rgb = (y * stride) + (4 * x);

                            rgbValues[rgb + 2] = result[x, y].R;
                            rgbValues[rgb + 1] = result[x, y].G;
                            rgbValues[rgb + 0] = result[x, y].B;
                        }
                    }

                    // Copy the RGB values back to the bitmap.
                    Marshal.Copy(rgbValues, 0, (IntPtr)pixels.Pointer, bytes);
                    // Release image bits.

                    return sharpenImage;
                }
            }

            return null;
        }

        public static void HighlightImage(SKBitmap bmp)
        {
            HighlightImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
        }

        public static void HighlightImage(SKBitmap bmp, Color highlightColor)
        {
            HighlightImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), highlightColor);
        }

        public static void HighlightImage(SKBitmap bmp, Rectangle rect)
        {
            HighlightImage(bmp, rect, Color.Yellow);
        }

        public static void HighlightImage(SKBitmap bmp, Rectangle rect, Color highlightColor)
        {
            using (SkiaPixelBuffer unsafeBitmap = new SkiaPixelBuffer(bmp, true))
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

        public static void Pixelate(SKBitmap bmp, int pixelSize)
        {
            if (pixelSize > 1)
            {
                using (SkiaPixelBuffer unsafeBitmap = new SkiaPixelBuffer(bmp, true))
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

        public static void Pixelate(SKBitmap bmp, int pixelSize, int borderSize, Color borderColor)
        {
            Pixelate(bmp, pixelSize);

            if (pixelSize > 1 && borderSize > 0 && borderColor.A > 0)
            {
                using (SKBitmap bmpTexture = CreateBitmap(pixelSize, pixelSize))
                {
                    using (SKCanvas g = new SKCanvas(bmpTexture))
                    using (SKPaint pen = SkiaDrawing.Stroke(borderColor, borderSize))
                    {
                        g.DrawRectangleProper(pen, new Rectangle(0, 0, bmpTexture.Width, bmpTexture.Height));
                    }

                    using (SKCanvas g = new SKCanvas(bmp))
                    using (SKPaint brush = SkiaDrawing.Texture(bmpTexture))
                    {
                        g.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
                    }
                }
            }
        }

        public static void BoxBlur(SKBitmap bmp, int range)
        {
            BoxBlur(bmp, range, new Rectangle(0, 0, bmp.Width, bmp.Height));
        }

        public static void BoxBlur(SKBitmap bmp, int range, Rectangle rect)
        {
            if (range > 1)
            {
                if (range.IsEvenNumber())
                {
                    range++;
                }

                using (SkiaPixelBuffer unsafeBitmap = new SkiaPixelBuffer(bmp, true))
                {
                    BoxBlurHorizontal(unsafeBitmap, range, rect);
                    BoxBlurVertical(unsafeBitmap, range, rect);
                    BoxBlurHorizontal(unsafeBitmap, range, rect);
                    BoxBlurVertical(unsafeBitmap, range, rect);
                }
            }
        }

        private static void BoxBlurHorizontal(SkiaPixelBuffer unsafeBitmap, int range, Rectangle rect)
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

        private static void BoxBlurVertical(SkiaPixelBuffer unsafeBitmap, int range, Rectangle rect)
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

        public static SKBitmap GaussianBlur(SKBitmap bmp, int radius)
        {
            int size = radius * 2 + 1;
            double sigma = radius / 3.0;

            ConvolutionMatrix kernelHorizontal = SkiaConvolutionMatrixManager.GaussianBlur(1, size, sigma);

            ConvolutionMatrix kernelVertical = new ConvolutionMatrix(size, 1)
            {
                ConsiderAlpha = kernelHorizontal.ConsiderAlpha
            };

            for (int i = 0; i < size; i++)
            {
                kernelVertical[i, 0] = kernelHorizontal[0, i];
            }

            using (SKBitmap horizontalPass = kernelHorizontal.Apply(bmp))
            {
                return kernelVertical.Apply(horizontalPass);
            }
        }

        public static void ColorDepth(SKBitmap bmp, int bitsPerChannel = 4)
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

            using (SkiaPixelBuffer unsafeBitmap = new SkiaPixelBuffer(bmp, true))
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

        public static void FastBoxBlur(SKBitmap bmp, int radius)
        {
            if (radius < 1) return;

            using (SkiaPixelBuffer unsafeBitmap = new SkiaPixelBuffer(bmp, true))
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

        public static SKBitmap WavyEdges(SKBitmap bmp, int waveDepth, int waveRange, ImageSides sides)
        {
            return WavyEdges(bmp, waveDepth, waveRange, sides, Color.Transparent);
        }

        public static SKBitmap WavyEdges(SKBitmap bmp, int waveDepth, int waveRange, ImageSides sides, Color backgroundColor)
        {
            if (waveDepth < 1 || waveRange < 1 || sides == ImageSides.None)
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

            if (sides.HasFlag(ImageSides.Top))
            {
                int startX = sides.HasFlag(ImageSides.Left) ? waveDepth : 0;
                int endX = sides.HasFlag(ImageSides.Right) ? bmp.Width - waveDepth : bmp.Width;
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

            if (sides.HasFlag(ImageSides.Right))
            {
                int startY = sides.HasFlag(ImageSides.Top) ? waveDepth : 0;
                int endY = sides.HasFlag(ImageSides.Bottom) ? bmp.Height - waveDepth : bmp.Height;
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

            if (sides.HasFlag(ImageSides.Bottom))
            {
                int startX = sides.HasFlag(ImageSides.Right) ? bmp.Width - waveDepth : bmp.Width;
                int endX = sides.HasFlag(ImageSides.Left) ? waveDepth : 0;
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

            if (sides.HasFlag(ImageSides.Left))
            {
                int startY = sides.HasFlag(ImageSides.Bottom) ? bmp.Height - waveDepth : bmp.Height;
                int endY = sides.HasFlag(ImageSides.Top) ? waveDepth : 0;
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

            if (!sides.HasFlag(ImageSides.Top))
            {
                points[0] = new Point(points[points.Count - 1].X, 0);
            }

            SKBitmap bmpResult = bmp.CreateEmptyBitmap();

            using (bmp)
            using (SKCanvas g = new SKCanvas(bmpResult))
            using (SKPaint brush = SkiaDrawing.Texture(bmp))
            {
                if (backgroundColor.A > 0)
                {
                    g.Clear(backgroundColor);
                }

                g.FillPolygon(brush, points.ToArray());
            }

            return bmpResult;
        }

        public static SKBitmap TornEdges(SKBitmap bmp, int tornDepth, int tornRange, ImageSides sides, bool curvedEdges, bool random)
        {
            return TornEdges(bmp, tornDepth, tornRange, sides, curvedEdges, random, Color.Transparent);
        }

        public static SKBitmap TornEdges(SKBitmap bmp, int tornDepth, int tornRange, ImageSides sides, bool curvedEdges, bool random, Color backgroundColor)
        {
            if (tornDepth < 1 || tornRange < 1 || sides == ImageSides.None)
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

            if (sides.HasFlag(ImageSides.Top) && horizontalTornCount > 1)
            {
                int startX = (sides.HasFlag(ImageSides.Left) && verticalTornCount > 1) ? tornDepth : 0;
                int endX = (sides.HasFlag(ImageSides.Right) && verticalTornCount > 1) ? bmp.Width - tornDepth : bmp.Width;
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

            if (sides.HasFlag(ImageSides.Right) && verticalTornCount > 1)
            {
                int startY = (sides.HasFlag(ImageSides.Top) && horizontalTornCount > 1) ? tornDepth : 0;
                int endY = (sides.HasFlag(ImageSides.Bottom) && horizontalTornCount > 1) ? bmp.Height - tornDepth : bmp.Height;
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

            if (sides.HasFlag(ImageSides.Bottom) && horizontalTornCount > 1)
            {
                int startX = (sides.HasFlag(ImageSides.Right) && verticalTornCount > 1) ? bmp.Width - tornDepth : bmp.Width;
                int endX = (sides.HasFlag(ImageSides.Left) && verticalTornCount > 1) ? tornDepth : 0;
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

            if (sides.HasFlag(ImageSides.Left) && verticalTornCount > 1)
            {
                int startY = (sides.HasFlag(ImageSides.Bottom) && horizontalTornCount > 1) ? bmp.Height - tornDepth : bmp.Height;
                int endY = (sides.HasFlag(ImageSides.Top) && horizontalTornCount > 1) ? tornDepth : 0;
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

            SKBitmap bmpResult = bmp.CreateEmptyBitmap();

            using (bmp)
            using (SKCanvas g = new SKCanvas(bmpResult))
            using (SKPaint brush = SkiaDrawing.Texture(bmp))
            {
                if (backgroundColor.A > 0)
                {
                    g.Clear(backgroundColor);
                }

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

        public static SKBitmap Slice(SKBitmap bmp, int minSliceHeight, int maxSliceHeight, int minSliceShift, int maxSliceShift)
        {
            if (minSliceHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(minSliceHeight));
            }

            if (maxSliceHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSliceHeight));
            }

            SKBitmap bmpResult = bmp.CreateEmptyBitmap();

            using (SKCanvas g = new SKCanvas(bmpResult))
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

                    g.DrawImage(bmp, destRect, sourceRect);

                    y += sourceRect.Height;
                }
            }

            return bmpResult;
        }

        public static SKBitmap CombineImages(List<SKBitmap> images, ImageOrientation orientation, ImageCombinerAlignment alignment = ImageCombinerAlignment.LeftOrTop,
            int space = 0, int wrapAfter = 0, bool autoFillBackground = false)
        {
            int imageCount = images.Count;
            Rectangle[] imageRects = new Rectangle[imageCount];
            Point position = new Point(0, 0);
            int currentSize = 0;

            for (int i = 0; i < imageCount; i++)
            {
                SKBitmap image = images[i];
                Point offset = new Point(0, 0);

                if (orientation == ImageOrientation.Horizontal)
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
            SKBitmap bmp = CreateBitmap(totalImageRect.Width, totalImageRect.Height);

            using (SKCanvas g = new SKCanvas(bmp))
            {
                for (int i = 0; i < imageCount; i++)
                {
                    SKBitmap image = images[i];

                    if (autoFillBackground && i == 0)
                    {
                        Color backgroundColor = image.GetPixel(image.Width - 1, image.Height - 1).ToDrawingColor();
                        g.Clear(backgroundColor);
                    }

                    g.DrawImage(image, imageRects[i]);
                }
            }

            return bmp;
        }

        public static SKBitmap CombineImages(IEnumerable<string> imageFiles, ImageOrientation orientation, ImageCombinerAlignment alignment = ImageCombinerAlignment.LeftOrTop,
            int space = 0, int wrapAfter = 0, bool autoFillBackground = false)
        {
            List<SKBitmap> images = new List<SKBitmap>();

            try
            {
                foreach (string filePath in imageFiles)
                {
                    SKBitmap bmp = LoadImage(filePath);

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
                foreach (SKBitmap bmp in images)
                {
                    if (bmp != null)
                    {
                        bmp.Dispose();
                    }
                }
            }

            return null;
        }

        public static List<SKBitmap> SplitImage(SKBitmap img, int rowCount, int columnCount)
        {
            List<SKBitmap> images = new List<SKBitmap>();

            int width = img.Width / columnCount;
            int height = img.Height / rowCount;

            for (int y = 0; y < rowCount; y++)
            {
                for (int x = 0; x < columnCount; x++)
                {
                    SKBitmap bmp = CreateBitmap(width, height);

                    using (SKCanvas g = new SKCanvas(bmp))
                    {
                        Rectangle destRect = new Rectangle(0, 0, width, height);
                        Rectangle srcRect = new Rectangle(x * width, y * height, width, height);
                        g.DrawImage(img, destRect, srcRect);
                    }

                    images.Add(bmp);
                }
            }

            return images;
        }

        public static Rectangle FindAutoCropRectangle(SKBitmap bmp, bool sameColorCrop = false,
            ImageSides sides = ImageSides.Top | ImageSides.Bottom | ImageSides.Left | ImageSides.Right)
        {
            Rectangle source = new Rectangle(0, 0, bmp.Width, bmp.Height);

            if (sides == ImageSides.None)
            {
                return source;
            }

            Rectangle crop = source;

            using (SkiaPixelBuffer unsafeBitmap = new SkiaPixelBuffer(bmp, true, PixelAccess.ReadOnly))
            {
                bool leave = false;

                ColorBgra checkColor = unsafeBitmap.GetPixel(0, 0);
                uint mask = checkColor.Alpha == 0 ? 0xFF000000 : 0xFFFFFFFF;
                uint check = checkColor.Bgra & mask;

                if (sides.HasFlag(ImageSides.Left))
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

                if (sides.HasFlag(ImageSides.Top))
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

                if (sides.HasFlag(ImageSides.Right))
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

                if (sides.HasFlag(ImageSides.Bottom))
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

        public static SKBitmap AutoCropImage(SKBitmap bmp, bool sameColorCrop = false,
            ImageSides sides = ImageSides.Top | ImageSides.Bottom | ImageSides.Left | ImageSides.Right, int padding = 0)
        {
            Rectangle source = new Rectangle(0, 0, bmp.Width, bmp.Height);
            Rectangle rect = FindAutoCropRectangle(bmp, sameColorCrop, sides);

            if (source != rect)
            {
                SKBitmap croppedBitmap = CropBitmap(bmp, rect);

                if (croppedBitmap != null)
                {
                    using (bmp)
                    {
                        if (padding > 0)
                        {
                            using (croppedBitmap)
                            {
                                Color color = bmp.GetPixel(0, 0).ToDrawingColor();
                                return AddCanvas(croppedBitmap, padding, color);
                            }
                        }

                        return croppedBitmap;
                    }
                }
            }

            return bmp;
        }

        public static void SelectiveColor(SKBitmap bmp, Color lightColor, Color darkColor, int paletteSize = 2)
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

            using (SkiaPixelBuffer unsafeBitmap = new SkiaPixelBuffer(bmp, true))
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

        public static void ReplaceColor(SKBitmap bmp, Color sourceColor, Color targetColor, bool autoSourceColor = false, int threshold = 0)
        {
            ColorBgra sourceBgra = new ColorBgra(sourceColor);
            ColorBgra targetBgra = new ColorBgra(targetColor);

            using (SkiaPixelBuffer unsafeBitmap = new SkiaPixelBuffer(bmp, true))
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



        public static ImageSamplingMode GetInterpolationMode(ImageInterpolationMode interpolationMode)
        {
            switch (interpolationMode)
            {
                default:
                case ImageInterpolationMode.HighQualityBicubic:
                    return ImageSamplingMode.HighQualityBicubic;
                case ImageInterpolationMode.Bicubic:
                    return ImageSamplingMode.Bicubic;
                case ImageInterpolationMode.HighQualityBilinear:
                    return ImageSamplingMode.HighQualityBilinear;
                case ImageInterpolationMode.Bilinear:
                    return ImageSamplingMode.Bilinear;
                case ImageInterpolationMode.NearestNeighbor:
                    return ImageSamplingMode.NearestNeighbor;
            }
        }

        public static Size ApplyAspectRatio(int width, int height, SKBitmap bmp)
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

        public static Size ApplyAspectRatio(Size size, SKBitmap bmp)
        {
            return ApplyAspectRatio(size.Width, size.Height, bmp);
        }

        public static SKBitmap DrawGrip(Color color, Color shadow)
        {
            int size = 16;
            SKBitmap bmp = CreateBitmap(size, size);

            using (SKCanvas g = new SKCanvas(bmp))
            using (SKPaint brush = SkiaDrawing.Fill(color))
            using (SKPaint shadowBrush = SkiaDrawing.Fill(shadow))
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

        public static MemoryStream SaveJPEGAutoQuality(SKBitmap img, int sizeLimit, int qualityDecrement = 5,
            int minQuality = 0, int maxQuality = 100, SKJpegEncoderDownsample subsampling = SKJpegEncoderDownsample.Downsample420)
        {
            qualityDecrement = qualityDecrement.Clamp(1, 100);
            minQuality = minQuality.Clamp(0, 100);
            maxQuality = maxQuality.Clamp(0, 100);

            if (minQuality >= maxQuality)
            {
                return SaveJPEG(img, minQuality, subsampling);
            }

            MemoryStream ms = null;

            for (int quality = maxQuality; quality >= minQuality; quality -= qualityDecrement)
            {
                if (ms != null)
                {
                    ms.Dispose();
                }

                ms = SaveJPEG(img, quality, subsampling);

                //DebugHelper.WriteLine($"Quality: {quality}% - Size: {ms.Length.ToSizeString()}");

                if (ms.Length <= sizeLimit)
                {
                    break;
                }
            }

            return ms;
        }
    }
}
