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
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// Image operations on SkiaSharp bitmaps, ported from the GDI+ ImageHelpers so they run on every OS and give the same results.
    /// Methods return a new bitmap and leave the input alone unless the name says otherwise.
    /// </summary>
    public static class SkiaImageHelpers
    {
        // Grayscale weights used by the colour matrices, the same as ColorMatrixManager.
        private const float rw = 0.212671f;
        private const float gw = 0.715160f;
        private const float bw = 0.072169f;

        public static SKColor ToSKColor(this Color color) => new SKColor(color.R, color.G, color.B, color.A);

        public static Color ToColor(this SKColor color) => Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);

        public static SKBitmap CreateEmpty(int width, int height)
        {
            SKBitmap bitmap = new SKBitmap(new SKImageInfo(Math.Max(1, width), Math.Max(1, height), SKColorType.Bgra8888, SKAlphaType.Premul));
            bitmap.Erase(SKColors.Transparent);
            return bitmap;
        }

        /// <summary>A transparent bitmap the size of the source plus the offsets, like Bitmap.CreateEmptyBitmap.</summary>
        public static SKBitmap CreateEmpty(SKBitmap source, int widthOffset = 0, int heightOffset = 0) =>
            CreateEmpty(source.Width + widthOffset, source.Height + heightOffset);

        public static SKBitmap Clone(SKBitmap source) => source.Copy(SKColorType.Bgra8888) ?? throw new InvalidOperationException("The image could not be copied.");

        #region Colour matrices

        /// <summary>Applies a 5x5 GDI+ style colour matrix (row vectors, translation in the last row, values 0 to 1).</summary>
        public static SKBitmap ApplyColorMatrix(SKBitmap source, float[][] gdiMatrix)
        {
            float[] matrix = new float[20];

            for (int output = 0; output < 4; output++)
            {
                for (int input = 0; input < 4; input++)
                {
                    matrix[(output * 5) + input] = gdiMatrix[input][output];
                }

                matrix[(output * 5) + 4] = gdiMatrix[4][output];
            }

            using SKColorFilter filter = SKColorFilter.CreateColorMatrix(matrix);
            return ApplyColorFilter(source, filter);
        }

        public static SKBitmap ApplyColorFilter(SKBitmap source, SKColorFilter filter)
        {
            SKBitmap result = CreateEmpty(source);
            using SKCanvas canvas = new SKCanvas(result);
            using SKPaint paint = new SKPaint { ColorFilter = filter };
            canvas.DrawBitmap(source, 0, 0, paint);
            return result;
        }

        public static float[][] InverseMatrix() =>
        [
            [-1, 0, 0, 0, 0],
            [0, -1, 0, 0, 0],
            [0, 0, -1, 0, 0],
            [0, 0, 0, 1, 0],
            [1, 1, 1, 0, 1]
        ];

        public static float[][] AlphaMatrix(float value, float add = 0f) =>
        [
            [1, 0, 0, 0, 0],
            [0, 1, 0, 0, 0],
            [0, 0, 1, 0, 0],
            [0, 0, 0, value, 0],
            [0, 0, 0, add, 1]
        ];

        public static float[][] BrightnessMatrix(float value) =>
        [
            [1, 0, 0, 0, 0],
            [0, 1, 0, 0, 0],
            [0, 0, 1, 0, 0],
            [0, 0, 0, 1, 0],
            [value, value, value, 0, 1]
        ];

        public static float[][] ContrastMatrix(float value) =>
        [
            [value, 0, 0, 0, 0],
            [0, value, 0, 0, 0],
            [0, 0, value, 0, 0],
            [0, 0, 0, 1, 0],
            [0, 0, 0, 0, 1]
        ];

        public static float[][] BlackWhiteMatrix() =>
        [
            [1.5f, 1.5f, 1.5f, 0, 0],
            [1.5f, 1.5f, 1.5f, 0, 0],
            [1.5f, 1.5f, 1.5f, 0, 0],
            [0, 0, 0, 1, 0],
            [-1, -1, -1, 0, 1]
        ];

        public static float[][] PolaroidMatrix() =>
        [
            [1.438f, -0.062f, -0.062f, 0, 0],
            [-0.122f, 1.378f, -0.122f, 0, 0],
            [-0.016f, -0.016f, 1.483f, 0, 0],
            [0, 0, 0, 1, 0],
            [-0.03f, 0.05f, -0.02f, 0, 1]
        ];

        public static float[][] GrayscaleMatrix(float value = 1) =>
        [
            [rw * value, rw * value, rw * value, 0, 0],
            [gw * value, gw * value, gw * value, 0, 0],
            [bw * value, bw * value, bw * value, 0, 0],
            [0, 0, 0, 1, 0],
            [0, 0, 0, 0, 1]
        ];

        public static float[][] SepiaMatrix(float value = 1) =>
        [
            [0.393f * value, 0.349f * value, 0.272f * value, 0, 0],
            [0.769f * value, 0.686f * value, 0.534f * value, 0, 0],
            [0.189f * value, 0.168f * value, 0.131f * value, 0, 0],
            [0, 0, 0, 1, 0],
            [0, 0, 0, 0, 1]
        ];

        public static float[][] HueMatrix(float angle)
        {
            float a = angle * (float)(Math.PI / 180);
            float c = (float)Math.Cos(a);
            float s = (float)Math.Sin(a);

            return
            [
                [(rw + (c * (1 - rw))) + (s * -rw), (rw + (c * -rw)) + (s * 0.143f), (rw + (c * -rw)) + (s * -(1 - rw)), 0, 0],
                [(gw + (c * -gw)) + (s * -gw), (gw + (c * (1 - gw))) + (s * 0.14f), (gw + (c * -gw)) + (s * gw), 0, 0],
                [(bw + (c * -bw)) + (s * (1 - bw)), (bw + (c * -bw)) + (s * -0.283f), (bw + (c * (1 - bw))) + (s * bw), 0, 0],
                [0, 0, 0, 1, 0],
                [0, 0, 0, 0, 1]
            ];
        }

        public static float[][] SaturationMatrix(float value) =>
        [
            [((1.0f - value) * rw) + value, (1.0f - value) * rw, (1.0f - value) * rw, 0, 0],
            [(1.0f - value) * gw, ((1.0f - value) * gw) + value, (1.0f - value) * gw, 0, 0],
            [(1.0f - value) * bw, (1.0f - value) * bw, ((1.0f - value) * bw) + value, 0, 0],
            [0, 0, 0, 1, 0],
            [0, 0, 0, 0, 1]
        ];

        public static float[][] ColorizeMatrix(Color color, float value)
        {
            float r = (float)color.R / 255;
            float g = (float)color.G / 255;
            float b = (float)color.B / 255;
            float invAmount = 1 - value;

            return
            [
                [invAmount + (value * r * rw), value * g * rw, value * b * rw, 0, 0],
                [value * r * gw, invAmount + (value * g * gw), value * b * gw, 0, 0],
                [value * r * bw, value * g * bw, invAmount + (value * b * bw), 0, 0],
                [0, 0, 0, 1, 0],
                [0, 0, 0, 0, 1]
            ];
        }

        /// <summary>Paints every pixel in the colour, keeping its alpha scaled by the opacity. Used for shadows and glows.</summary>
        public static float[][] MaskMatrix(float opacity, Color color) =>
        [
            [0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0],
            [0, 0, 0, color.A / 255f * opacity, 0],
            [color.R / 255f, color.G / 255f, color.B / 255f, 0, 1]
        ];

        /// <summary>GDI+ gamma correction: each channel becomes channel ^ value. 1 is no change, the range is 0.1 to 5.</summary>
        public static SKBitmap Gamma(SKBitmap source, float value)
        {
            value = Math.Clamp(value, 0.1f, 5.0f);
            byte[] table = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                table[i] = (byte)Math.Clamp(Math.Round(Math.Pow(i / 255.0, value) * 255), 0, 255);
            }

            byte[] alpha = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                alpha[i] = (byte)i;
            }

            using SKColorFilter filter = SKColorFilter.CreateTable(alpha, table, table, table);
            return ApplyColorFilter(source, filter);
        }

        #endregion Colour matrices

        #region Convolution and blur

        /// <summary>Applies a convolution kernel with edge clamping, like ConvolutionMatrixManager.Apply.</summary>
        public static SKBitmap Convolve(SKBitmap source, double[,] kernel, byte offset = 0, bool considerAlpha = false)
        {
            int kernelHeight = kernel.GetLength(0);
            int kernelWidth = kernel.GetLength(1);
            int originX = (kernelWidth - 1) / 2;
            int originY = (kernelHeight - 1) / 2;

            using SkiaPixels src = SkiaPixels.From(source);
            using SkiaPixels dest = SkiaPixels.Create(src.Width, src.Height);

            Parallel.For(0, src.Height, y =>
            {
                for (int x = 0; x < src.Width; x++)
                {
                    double r = 0, g = 0, b = 0, a = 0;

                    for (int fy = 0; fy < kernelHeight; fy++)
                    {
                        int offsetY = Math.Clamp(y + fy - originY, 0, src.Height - 1);

                        for (int fx = 0; fx < kernelWidth; fx++)
                        {
                            int offsetX = Math.Clamp(x + fx - originX, 0, src.Width - 1);
                            uint color = src[offsetX, offsetY];
                            double weight = kernel[fy, fx];

                            r += weight * SkiaPixels.R(color);
                            g += weight * SkiaPixels.G(color);
                            b += weight * SkiaPixels.B(color);

                            if (considerAlpha)
                            {
                                a += weight * SkiaPixels.A(color);
                            }
                        }
                    }

                    r = Math.Clamp(r + offset, 0, 255);
                    g = Math.Clamp(g + offset, 0, 255);
                    b = Math.Clamp(b + offset, 0, 255);
                    byte alpha = considerAlpha ? (byte)Math.Clamp(a + offset, 0, 255) : SkiaPixels.A(src[x, y]);

                    dest[x, y] = SkiaPixels.Pack((byte)b, (byte)g, (byte)r, alpha);
                }
            });

            return dest.ToBitmap();
        }

        public static double[,] SmoothKernel(int weight = 1)
        {
            double factor = weight + 8;
            double[,] kernel = Fill(3, 3, 1 / factor);
            kernel[1, 1] = weight / factor;
            return kernel;
        }

        public static double[,] MeanRemovalKernel(int weight = 9)
        {
            double factor = weight - 8;
            double[,] kernel = Fill(3, 3, -1 / factor);
            kernel[1, 1] = weight / factor;
            return kernel;
        }

        public static double[,] SharpenKernel(int weight = 11)
        {
            double factor = weight - 8;
            double[,] kernel = Fill(3, 3, 0);
            kernel[1, 1] = weight / factor;
            kernel[1, 0] = kernel[0, 1] = kernel[2, 1] = kernel[1, 2] = -2 / factor;
            return kernel;
        }

        public static double[,] EmbossKernel()
        {
            double[,] kernel = Fill(3, 3, -1);
            kernel[1, 1] = 4;
            kernel[1, 0] = kernel[0, 1] = kernel[2, 1] = kernel[1, 2] = 0;
            return kernel;
        }

        public static double[,] EdgeDetectKernel()
        {
            double[,] kernel = new double[3, 3];
            kernel[0, 0] = kernel[0, 1] = kernel[0, 2] = -1;
            kernel[2, 0] = kernel[2, 1] = kernel[2, 2] = 1;
            return kernel;
        }

        private static double[,] Fill(int height, int width, double value)
        {
            double[,] kernel = new double[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    kernel[y, x] = value;
                }
            }

            return kernel;
        }

        /// <summary>Separable gaussian blur with sigma radius / 3, the same kernels as ImageHelpers.GaussianBlur.</summary>
        public static SKBitmap GaussianBlur(SKBitmap source, int radius)
        {
            int size = (radius * 2) + 1;
            double sigma = radius / 3.0;
            double[] weights = new double[size];
            double sum = 0;
            double midpoint = (size - 1) / 2.0;

            for (int i = 0; i < size; i++)
            {
                double x = i - midpoint;
                // GaussianFunction(x) * GaussianFunction(0): the second factor cancels out in the normalisation.
                sum += weights[i] = Math.Exp(-(x * x) / (2 * sigma * sigma));
            }

            double[,] horizontal = new double[1, size];
            double[,] vertical = new double[size, 1];

            for (int i = 0; i < size; i++)
            {
                horizontal[0, i] = vertical[i, 0] = weights[i] / sum;
            }

            using SKBitmap horizontalPass = Convolve(source, horizontal, considerAlpha: true);
            return Convolve(horizontalPass, vertical, considerAlpha: true);
        }

        /// <summary>Three-pass box blur in place on the pixels, the same as ImageHelpers.BoxBlur.</summary>
        public static void BoxBlur(SkiaPixels pixels, int range)
        {
            if (range > 1)
            {
                if (range % 2 == 0)
                {
                    range++;
                }

                BoxBlurHorizontal(pixels, range);
                BoxBlurVertical(pixels, range);
                BoxBlurHorizontal(pixels, range);
                BoxBlurVertical(pixels, range);
            }
        }

        public static SKBitmap BoxBlur(SKBitmap source, int range)
        {
            using SkiaPixels pixels = SkiaPixels.From(source);
            BoxBlur(pixels, range);
            return pixels.ToBitmap();
        }

        private static void BoxBlurHorizontal(SkiaPixels pixels, int range)
        {
            int halfRange = range / 2;
            uint[] newColors = new uint[pixels.Width];

            for (int y = 0; y < pixels.Height; y++)
            {
                int hits = 0, r = 0, g = 0, b = 0, a = 0;

                for (int x = -halfRange; x < pixels.Width; x++)
                {
                    int oldPixel = x - halfRange - 1;

                    if (oldPixel >= 0)
                    {
                        uint color = pixels[oldPixel, y];

                        if (color != 0)
                        {
                            r -= SkiaPixels.R(color);
                            g -= SkiaPixels.G(color);
                            b -= SkiaPixels.B(color);
                            a -= SkiaPixels.A(color);
                        }

                        hits--;
                    }

                    int newPixel = x + halfRange;

                    if (newPixel < pixels.Width)
                    {
                        uint color = pixels[newPixel, y];

                        if (color != 0)
                        {
                            r += SkiaPixels.R(color);
                            g += SkiaPixels.G(color);
                            b += SkiaPixels.B(color);
                            a += SkiaPixels.A(color);
                        }

                        hits++;
                    }

                    if (x >= 0)
                    {
                        newColors[x] = SkiaPixels.Pack((byte)(b / hits), (byte)(g / hits), (byte)(r / hits), (byte)(a / hits));
                    }
                }

                for (int x = 0; x < pixels.Width; x++)
                {
                    pixels[x, y] = newColors[x];
                }
            }
        }

        private static void BoxBlurVertical(SkiaPixels pixels, int range)
        {
            int halfRange = range / 2;
            uint[] newColors = new uint[pixels.Height];

            for (int x = 0; x < pixels.Width; x++)
            {
                int hits = 0, r = 0, g = 0, b = 0, a = 0;

                for (int y = -halfRange; y < pixels.Height; y++)
                {
                    int oldPixel = y - halfRange - 1;

                    if (oldPixel >= 0)
                    {
                        uint color = pixels[x, oldPixel];

                        if (color != 0)
                        {
                            r -= SkiaPixels.R(color);
                            g -= SkiaPixels.G(color);
                            b -= SkiaPixels.B(color);
                            a -= SkiaPixels.A(color);
                        }

                        hits--;
                    }

                    int newPixel = y + halfRange;

                    if (newPixel < pixels.Height)
                    {
                        uint color = pixels[x, newPixel];

                        if (color != 0)
                        {
                            r += SkiaPixels.R(color);
                            g += SkiaPixels.G(color);
                            b += SkiaPixels.B(color);
                            a += SkiaPixels.A(color);
                        }

                        hits++;
                    }

                    if (y >= 0)
                    {
                        newColors[y] = SkiaPixels.Pack((byte)(b / hits), (byte)(g / hits), (byte)(r / hits), (byte)(a / hits));
                    }
                }

                for (int y = 0; y < pixels.Height; y++)
                {
                    pixels[x, y] = newColors[y];
                }
            }
        }

        #endregion Convolution and blur

        #region Per-pixel colour

        public static SKBitmap ColorDepth(SKBitmap source, int bitsPerChannel = 4)
        {
            using SkiaPixels pixels = SkiaPixels.From(source);

            if (bitsPerChannel >= 1 && bitsPerChannel <= 8)
            {
                double colorsPerChannel = Math.Pow(2, bitsPerChannel);
                double interval = 255 / (colorsPerChannel - 1);
                byte Remap(byte color) => (byte)Math.Round(Math.Round(color / interval) * interval);

                for (int i = 0; i < pixels.PixelCount; i++)
                {
                    uint color = pixels[i];
                    pixels[i] = SkiaPixels.Pack(Remap(SkiaPixels.B(color)), Remap(SkiaPixels.G(color)), Remap(SkiaPixels.R(color)), SkiaPixels.A(color));
                }
            }

            return pixels.ToBitmap();
        }

        /// <summary>Alpha weighted block averages, then an optional grid of borders, like ImageHelpers.Pixelate.</summary>
        public static SKBitmap Pixelate(SKBitmap source, int pixelSize, int borderSize = 0, Color borderColor = default)
        {
            SKBitmap result;

            using (SkiaPixels pixels = SkiaPixels.From(source))
            {
                if (pixelSize > 1)
                {
                    for (int y = 0; y < pixels.Height; y += pixelSize)
                    {
                        for (int x = 0; x < pixels.Width; x += pixelSize)
                        {
                            int xLimit = Math.Min(x + pixelSize, pixels.Width);
                            int yLimit = Math.Min(y + pixelSize, pixels.Height);
                            int pixelCount = (xLimit - x) * (yLimit - y);
                            float r = 0, g = 0, b = 0, a = 0, weightedCount = 0;

                            for (int y2 = y; y2 < yLimit; y2++)
                            {
                                for (int x2 = x; x2 < xLimit; x2++)
                                {
                                    uint color = pixels[x2, y2];
                                    float weight = SkiaPixels.A(color) / 255f;
                                    r += SkiaPixels.R(color) * weight;
                                    g += SkiaPixels.G(color) * weight;
                                    b += SkiaPixels.B(color) * weight;
                                    a += SkiaPixels.A(color) * weight;
                                    weightedCount += weight;
                                }
                            }

                            uint average = weightedCount > 0
                                ? SkiaPixels.Pack((byte)(b / weightedCount), (byte)(g / weightedCount), (byte)(r / weightedCount), (byte)(a / pixelCount))
                                : 0;

                            for (int y2 = y; y2 < yLimit; y2++)
                            {
                                for (int x2 = x; x2 < xLimit; x2++)
                                {
                                    pixels[x2, y2] = average;
                                }
                            }
                        }
                    }
                }

                result = pixels.ToBitmap();
            }

            if (pixelSize > 1 && borderSize > 0 && borderColor.A > 0)
            {
                using SKCanvas canvas = new SKCanvas(result);
                using SKPaint paint = new SKPaint { Color = borderColor.ToSKColor(), IsStroke = true, StrokeWidth = borderSize };
                float inset = borderSize / 2f;

                for (int y = 0; y < result.Height; y += pixelSize)
                {
                    for (int x = 0; x < result.Width; x += pixelSize)
                    {
                        canvas.DrawRect(x + inset, y + inset, pixelSize - borderSize, pixelSize - borderSize, paint);
                    }
                }
            }

            return result;
        }

        public static SKBitmap ReplaceColor(SKBitmap source, Color sourceColor, Color targetColor, bool autoSourceColor = false, int threshold = 0)
        {
            using SkiaPixels pixels = SkiaPixels.From(source);
            uint sourceBgra = SkiaPixels.Pack(sourceColor.B, sourceColor.G, sourceColor.R, sourceColor.A);
            uint targetBgra = SkiaPixels.Pack(targetColor.B, targetColor.G, targetColor.R, targetColor.A);

            if (autoSourceColor && pixels.PixelCount > 0)
            {
                sourceBgra = pixels[0];
                sourceColor = Color.FromArgb(SkiaPixels.A(sourceBgra), SkiaPixels.R(sourceBgra), SkiaPixels.G(sourceBgra), SkiaPixels.B(sourceBgra));
            }

            for (int i = 0; i < pixels.PixelCount; i++)
            {
                uint color = pixels[i];

                if (threshold == 0 ? color == sourceBgra :
                    ColorHelpers.ColorsAreClose(Color.FromArgb(SkiaPixels.A(color), SkiaPixels.R(color), SkiaPixels.G(color), SkiaPixels.B(color)), sourceColor, threshold))
                {
                    pixels[i] = targetBgra;
                }
            }

            return pixels.ToBitmap();
        }

        public static SKBitmap SelectiveColor(SKBitmap source, Color lightColor, Color darkColor, int paletteSize = 2)
        {
            paletteSize = Math.Max(paletteSize, 2);
            Dictionary<int, Color> colors = new Dictionary<int, Color>();

            for (int i = 0; i < paletteSize; i++)
            {
                Color color = ColorHelpers.Lerp(lightColor, darkColor, (float)i / (paletteSize - 1));
                colors.TryAdd(ColorHelpers.PerceivedBrightness(color), color);
            }

            using SkiaPixels pixels = SkiaPixels.From(source);

            for (int i = 0; i < pixels.PixelCount; i++)
            {
                uint color = pixels[i];
                int brightness = ColorHelpers.PerceivedBrightness(Color.FromArgb(SkiaPixels.A(color), SkiaPixels.R(color), SkiaPixels.G(color), SkiaPixels.B(color)));
                KeyValuePair<int, Color> closest = colors.Aggregate((current, next) => Math.Abs(current.Key - brightness) < Math.Abs(next.Key - brightness) ? current : next);
                pixels[i] = SkiaPixels.Pack(closest.Value.B, closest.Value.G, closest.Value.R, SkiaPixels.A(color));
            }

            return pixels.ToBitmap();
        }

        public static SKBitmap RGBSplit(SKBitmap source, Point offsetRed, Point offsetGreen, Point offsetBlue)
        {
            using SkiaPixels src = SkiaPixels.From(source);
            using SkiaPixels dest = SkiaPixels.Create(src.Width, src.Height);
            int right = src.Width - 1;
            int bottom = src.Height - 1;

            for (int y = 0; y < src.Height; y++)
            {
                for (int x = 0; x < src.Width; x++)
                {
                    uint colorR = src[Math.Clamp(x - offsetRed.X, 0, right), Math.Clamp(y - offsetRed.Y, 0, bottom)];
                    uint colorG = src[Math.Clamp(x - offsetGreen.X, 0, right), Math.Clamp(y - offsetGreen.Y, 0, bottom)];
                    uint colorB = src[Math.Clamp(x - offsetBlue.X, 0, right), Math.Clamp(y - offsetBlue.Y, 0, bottom)];

                    dest[x, y] = SkiaPixels.Pack(
                        (byte)(SkiaPixels.B(colorB) * SkiaPixels.A(colorB) / 255),
                        (byte)(SkiaPixels.G(colorG) * SkiaPixels.A(colorG) / 255),
                        (byte)(SkiaPixels.R(colorR) * SkiaPixels.A(colorR) / 255),
                        (byte)((SkiaPixels.A(colorR) + SkiaPixels.A(colorG) + SkiaPixels.A(colorB)) / 3));
                }
            }

            return dest.ToBitmap();
        }

        /// <summary>The legacy 5x5 sharpen filter. Strength 1 is a full sharpen.</summary>
        public static SKBitmap Sharpen(SKBitmap source, double strength)
        {
            double[,] filter =
            {
                { -1, -1, -1, -1, -1 },
                { -1, 2, 2, 2, -1 },
                { -1, 2, 16, 2, -1 },
                { -1, 2, 2, 2, -1 },
                { -1, -1, -1, -1, -1 }
            };

            double bias = 1.0 - strength;
            double factor = strength / 16.0;
            const int s = 2;

            using SkiaPixels src = SkiaPixels.From(source);
            using SkiaPixels dest = SkiaPixels.From(source);
            int width = src.Width;
            int height = src.Height;

            for (int x = s; x < width - s; x++)
            {
                for (int y = s; y < height - s; y++)
                {
                    double red = 0, green = 0, blue = 0;

                    for (int filterX = 0; filterX < 5; filterX++)
                    {
                        for (int filterY = 0; filterY < 5; filterY++)
                        {
                            uint color = src[(x - s + filterX + width) % width, (y - s + filterY + height) % height];
                            red += SkiaPixels.R(color) * filter[filterX, filterY];
                            green += SkiaPixels.G(color) * filter[filterX, filterY];
                            blue += SkiaPixels.B(color) * filter[filterX, filterY];
                        }
                    }

                    uint original = src[x, y];
                    int r = Math.Clamp((int)((factor * red) + (bias * SkiaPixels.R(original))), 0, 255);
                    int g = Math.Clamp((int)((factor * green) + (bias * SkiaPixels.G(original))), 0, 255);
                    int b = Math.Clamp((int)((factor * blue) + (bias * SkiaPixels.B(original))), 0, 255);
                    // The GDI+ version worked on 24 bit pixels, so the result is opaque.
                    dest[x, y] = SkiaPixels.Pack((byte)b, (byte)g, (byte)r, 255);
                }
            }

            return dest.ToBitmap();
        }

        #endregion Per-pixel colour

        #region Geometry

        public static Size ApplyAspectRatio(int width, int height, SKBitmap bitmap)
        {
            if (width == 0)
            {
                return new Size((int)Math.Round((float)height / bitmap.Height * bitmap.Width), height);
            }

            if (height == 0)
            {
                return new Size(width, (int)Math.Round((float)width / bitmap.Width * bitmap.Height));
            }

            return new Size(width, height);
        }

        public static SKBitmap Resize(SKBitmap source, int width, int height)
        {
            if (width < 1 || height < 1 || (source.Width == width && source.Height == height))
            {
                return Clone(source);
            }

            SKBitmap result = CreateEmpty(width, height);
            using SKCanvas canvas = new SKCanvas(result);
            using SKImage image = SKImage.FromBitmap(source);
            using SKPaint paint = new SKPaint { IsAntialias = true };
            canvas.DrawImage(image, new SKRect(0, 0, width, height), new SKSamplingOptions(SKCubicResampler.Mitchell), paint);
            return result;
        }

        /// <summary>Fits the image into the box keeping its aspect ratio, like ImageHelpers.ResizeImage(allowEnlarge, centerImage, backColor).</summary>
        public static SKBitmap ResizeToFit(SKBitmap source, int width, int height, bool allowEnlarge, bool centerImage, Color backColor)
        {
            double ratio;
            int newWidth, newHeight;

            if (!allowEnlarge && source.Width <= width && source.Height <= height)
            {
                ratio = 1.0;
                newWidth = source.Width;
                newHeight = source.Height;
            }
            else
            {
                double ratioX = (double)width / source.Width;
                double ratioY = (double)height / source.Height;
                ratio = ratioX < ratioY ? ratioX : ratioY;
                newWidth = (int)(source.Width * ratio);
                newHeight = (int)(source.Height * ratio);
            }

            int newX = centerImage ? (int)((width - (source.Width * ratio)) / 2) : 0;
            int newY = centerImage ? (int)((height - (source.Height * ratio)) / 2) : 0;

            SKBitmap result = CreateEmpty(width, height);
            using SKCanvas canvas = new SKCanvas(result);

            if (backColor.A > 0)
            {
                canvas.Clear(backColor.ToSKColor());
            }

            using SKImage image = SKImage.FromBitmap(source);
            canvas.DrawImage(image, new SKRect(newX, newY, newX + newWidth, newY + newHeight), new SKSamplingOptions(SKCubicResampler.Mitchell));
            return result;
        }

        /// <summary>The part of the bitmap inside the rectangle, or null when the rectangle is not inside the bitmap.</summary>
        public static SKBitmap Crop(SKBitmap source, Rectangle rect)
        {
            if (rect.X < 0 || rect.Y < 0 || rect.Width <= 0 || rect.Height <= 0 || !new Rectangle(0, 0, source.Width, source.Height).Contains(rect))
            {
                return null;
            }

            SKBitmap result = CreateEmpty(rect.Width, rect.Height);
            using SKCanvas canvas = new SKCanvas(result);
            canvas.DrawBitmap(source, new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom), new SKRect(0, 0, rect.Width, rect.Height));
            return result;
        }

        /// <summary>Adds a margin, filled with the colour. Null when there is nothing to add, like ImageHelpers.AddCanvas.</summary>
        public static SKBitmap AddCanvas(SKBitmap source, Insets margin, Color canvasColor)
        {
            if (margin.All == 0 || source.Width + margin.Horizontal < 1 || source.Height + margin.Vertical < 1)
            {
                return null;
            }

            SKBitmap result = CreateEmpty(source, margin.Horizontal, margin.Vertical);
            using SKCanvas canvas = new SKCanvas(result);

            if (canvasColor.A > 0)
            {
                canvas.Clear(canvasColor.ToSKColor());
                using SKPaint copy = new SKPaint { BlendMode = SKBlendMode.Src };
                canvas.DrawBitmap(source, margin.Left, margin.Top, copy);
            }
            else
            {
                canvas.DrawBitmap(source, margin.Left, margin.Top);
            }

            return result;
        }

        public static SKBitmap Flip(SKBitmap source, bool horizontally, bool vertically)
        {
            SKBitmap result = CreateEmpty(source);
            using SKCanvas canvas = new SKCanvas(result);
            canvas.Scale(horizontally ? -1 : 1, vertically ? -1 : 1, source.Width / 2f, source.Height / 2f);
            canvas.DrawBitmap(source, 0, 0);
            return result;
        }

        public static SKBitmap Rotate(SKBitmap source, float angleDegrees, bool upsize, bool clip)
        {
            if (angleDegrees == 0f)
            {
                return Clone(source);
            }

            int oldWidth = source.Width;
            int oldHeight = source.Height;
            int newWidth = oldWidth;
            int newHeight = oldHeight;
            float scaleFactor = 1f;

            if (upsize || !clip)
            {
                double angleRadians = angleDegrees * Math.PI / 180d;
                double cos = Math.Abs(Math.Cos(angleRadians));
                double sin = Math.Abs(Math.Sin(angleRadians));
                newWidth = (int)Math.Round((oldWidth * cos) + (oldHeight * sin));
                newHeight = (int)Math.Round((oldWidth * sin) + (oldHeight * cos));
            }

            if (!upsize && !clip)
            {
                scaleFactor = Math.Min((float)oldWidth / newWidth, (float)oldHeight / newHeight);
                newWidth = oldWidth;
                newHeight = oldHeight;
            }

            // The GDI+ code drew into a canvas the size of the source; a larger rotation needs room, so use the computed size.
            SKBitmap result = CreateEmpty(upsize ? newWidth : oldWidth, upsize ? newHeight : oldHeight);
            using SKCanvas canvas = new SKCanvas(result);
            using SKImage image = SKImage.FromBitmap(source);
            canvas.Translate(result.Width / 2f, result.Height / 2f);

            if (scaleFactor != 1f)
            {
                canvas.Scale(scaleFactor);
            }

            canvas.RotateDegrees(angleDegrees);
            canvas.Translate(-oldWidth / 2f, -oldHeight / 2f);
            using SKPaint paint = new SKPaint { IsAntialias = true };
            canvas.DrawImage(image, 0, 0, new SKSamplingOptions(SKCubicResampler.Mitchell), paint);
            return result;
        }

        /// <summary>Shears the image by the pixel amounts, like ImageHelpers.AddSkew.</summary>
        public static SKBitmap Skew(SKBitmap source, int x, int y)
        {
            SKBitmap result = CreateEmpty(source, Math.Abs(x), Math.Abs(y));
            using SKCanvas canvas = new SKCanvas(result);
            int startX = -Math.Min(0, x);
            int startY = -Math.Min(0, y);
            int endX = Math.Max(0, x);
            int endY = Math.Max(0, y);

            // GDI+ maps the source rectangle's top left, top right and bottom left corners onto these three points.
            SKPoint p0 = new SKPoint(startX, startY);
            SKPoint p1 = new SKPoint(startX + source.Width - 1, endY);
            SKPoint p2 = new SKPoint(endX, startY + source.Height - 1);
            float w = source.Width;
            float h = source.Height;
            SKMatrix matrix = new SKMatrix
            {
                ScaleX = (p1.X - p0.X) / w,
                SkewX = (p2.X - p0.X) / h,
                TransX = p0.X,
                SkewY = (p1.Y - p0.Y) / w,
                ScaleY = (p2.Y - p0.Y) / h,
                TransY = p0.Y,
                Persp2 = 1
            };

            canvas.SetMatrix(matrix);
            using SKPaint paint = new SKPaint { IsAntialias = true };
            using SKImage image = SKImage.FromBitmap(source);
            canvas.DrawImage(image, 0, 0, new SKSamplingOptions(SKFilterMode.Linear), paint);
            return result;
        }

        public static SKBitmap RoundedCorners(SKBitmap source, int cornerRadius)
        {
            SKBitmap result = CreateEmpty(source);
            using SKCanvas canvas = new SKCanvas(result);
            using SKPath path = new SKPath();
            path.AddRoundRect(new SKRect(0, 0, source.Width, source.Height), cornerRadius, cornerRadius);
            canvas.ClipPath(path, antialias: true);
            canvas.DrawBitmap(source, 0, 0);
            return result;
        }

        /// <summary>The rectangle left after trimming borders of the corner colour, like ImageHelpers.FindAutoCropRectangle.</summary>
        public static Rectangle FindAutoCropRectangle(SKBitmap source, bool sameColorCrop = false, AnchorSides sides = AnchorSides.Top | AnchorSides.Bottom | AnchorSides.Left | AnchorSides.Right)
        {
            Rectangle crop = new Rectangle(0, 0, source.Width, source.Height);

            if (sides == AnchorSides.None)
            {
                return crop;
            }

            using SkiaPixels pixels = SkiaPixels.From(source);
            bool leave = false;
            uint checkColor = pixels[0, 0];
            uint mask = SkiaPixels.A(checkColor) == 0 ? 0xFF000000 : 0xFFFFFFFF;
            uint check = checkColor & mask;

            if (sides.HasFlag(AnchorSides.Left))
            {
                for (int x = 0; x < pixels.Width && !leave; x++)
                {
                    for (int y = 0; y < pixels.Height; y++)
                    {
                        if ((pixels[x, y] & mask) != check)
                        {
                            crop.X = x;
                            crop.Width -= x;
                            leave = true;
                            break;
                        }
                    }
                }

                if (!leave)
                {
                    return crop;
                }

                leave = false;
            }

            if (sides.HasFlag(AnchorSides.Top))
            {
                for (int y = 0; y < pixels.Height && !leave; y++)
                {
                    for (int x = 0; x < pixels.Width; x++)
                    {
                        if ((pixels[x, y] & mask) != check)
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
                checkColor = pixels[pixels.Width - 1, pixels.Height - 1];
                mask = SkiaPixels.A(checkColor) == 0 ? 0xFF000000 : 0xFFFFFFFF;
                check = checkColor & mask;
            }

            if (sides.HasFlag(AnchorSides.Right))
            {
                for (int x = pixels.Width - 1; x >= 0 && !leave; x--)
                {
                    for (int y = 0; y < pixels.Height; y++)
                    {
                        if ((pixels[x, y] & mask) != check)
                        {
                            crop.Width = x - crop.X + 1;
                            leave = true;
                            break;
                        }
                    }
                }

                leave = false;
            }

            if (sides.HasFlag(AnchorSides.Bottom))
            {
                for (int y = pixels.Height - 1; y >= 0 && !leave; y--)
                {
                    for (int x = 0; x < pixels.Width; x++)
                    {
                        if ((pixels[x, y] & mask) != check)
                        {
                            crop.Height = y - crop.Y + 1;
                            leave = true;
                            break;
                        }
                    }
                }
            }

            return crop;
        }

        /// <summary>Trims the borders and adds padding in the corner colour, or returns null when there is nothing to trim.</summary>
        public static SKBitmap AutoCrop(SKBitmap source, bool sameColorCrop = false, AnchorSides sides = AnchorSides.Top | AnchorSides.Bottom | AnchorSides.Left | AnchorSides.Right, int padding = 0)
        {
            Rectangle rect = FindAutoCropRectangle(source, sameColorCrop, sides);

            if (rect == new Rectangle(0, 0, source.Width, source.Height))
            {
                return null;
            }

            SKBitmap cropped = Crop(source, rect);

            if (cropped == null || padding <= 0)
            {
                return cropped;
            }

            using (cropped)
            {
                return AddCanvas(cropped, new Insets(padding), source.GetPixel(0, 0).ToColor()) ?? Clone(cropped);
            }
        }

        #endregion Geometry

        #region Drawing

        public static SKBitmap LoadImage(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    filePath = FileHelpers.GetAbsolutePath(filePath);

                    if (!string.IsNullOrEmpty(filePath) && FileHelpers.IsImageFile(filePath) && File.Exists(filePath))
                    {
                        // SKCodec applies EXIF orientation through the decoded origin.
                        using SKCodec codec = SKCodec.Create(filePath);

                        if (codec == null)
                        {
                            return null;
                        }

                        SKBitmap decoded = SKBitmap.Decode(codec);
                        return HelpersOptions.RotateImageByExifOrientationData ? ApplyOrigin(decoded, codec.EncodedOrigin) : decoded;
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return null;
        }

        private static SKBitmap ApplyOrigin(SKBitmap bitmap, SKEncodedOrigin origin)
        {
            if (bitmap == null || origin == SKEncodedOrigin.TopLeft)
            {
                return bitmap;
            }

            bool swap = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
            SKBitmap result = CreateEmpty(swap ? bitmap.Height : bitmap.Width, swap ? bitmap.Width : bitmap.Height);

            using (SKCanvas canvas = new SKCanvas(result))
            {
                SKMatrix matrix = origin switch
                {
                    SKEncodedOrigin.TopRight => new SKMatrix(-1, 0, bitmap.Width, 0, 1, 0, 0, 0, 1),
                    SKEncodedOrigin.BottomRight => new SKMatrix(-1, 0, bitmap.Width, 0, -1, bitmap.Height, 0, 0, 1),
                    SKEncodedOrigin.BottomLeft => new SKMatrix(1, 0, 0, 0, -1, bitmap.Height, 0, 0, 1),
                    SKEncodedOrigin.LeftTop => new SKMatrix(0, 1, 0, 1, 0, 0, 0, 0, 1),
                    SKEncodedOrigin.RightTop => new SKMatrix(0, -1, bitmap.Height, 1, 0, 0, 0, 0, 1),
                    SKEncodedOrigin.RightBottom => new SKMatrix(0, -1, bitmap.Height, -1, 0, bitmap.Width, 0, 0, 1),
                    SKEncodedOrigin.LeftBottom => new SKMatrix(0, 1, 0, -1, 0, bitmap.Width, 0, 0, 1),
                    _ => SKMatrix.Identity
                };

                canvas.SetMatrix(matrix);
                canvas.DrawBitmap(bitmap, 0, 0);
            }

            bitmap.Dispose();
            return result;
        }

        public static SKBitmap FillBackground(SKBitmap source, Color color)
        {
            SKBitmap result = CreateEmpty(source);
            using SKCanvas canvas = new SKCanvas(result);
            canvas.Clear(color.ToSKColor());
            canvas.DrawBitmap(source, 0, 0);
            return result;
        }

        public static SKBitmap FillBackground(SKBitmap source, GradientInfo gradient)
        {
            SKBitmap result = CreateEmpty(source);
            using SKCanvas canvas = new SKCanvas(result);
            gradient.Draw(canvas, new SKRect(0, 0, result.Width, result.Height));
            canvas.DrawBitmap(source, 0, 0);
            return result;
        }

        /// <summary>A checkerboard behind the image, like ImageHelpers.DrawCheckers.</summary>
        public static SKBitmap DrawCheckers(SKBitmap source, int checkerSize, Color color1, Color color2)
        {
            SKBitmap result = CreateEmpty(source);
            using SKCanvas canvas = new SKCanvas(result);
            using SKPaint paint1 = new SKPaint { Color = color1.ToSKColor() };
            using SKPaint paint2 = new SKPaint { Color = color2.ToSKColor() };
            checkerSize = Math.Max(1, checkerSize);

            for (int y = 0; y < result.Height; y += checkerSize)
            {
                for (int x = 0; x < result.Width; x += checkerSize)
                {
                    canvas.DrawRect(x, y, checkerSize, checkerSize, ((x / checkerSize) + (y / checkerSize)) % 2 == 0 ? paint1 : paint2);
                }
            }

            canvas.DrawBitmap(source, 0, 0);
            return result;
        }

        public static SKBitmap DrawBackgroundImage(SKBitmap source, SKBitmap background, bool center = true, bool tile = false)
        {
            SKBitmap result = CreateEmpty(source);
            using SKCanvas canvas = new SKCanvas(result);

            if (tile)
            {
                using SKShader shader = SKShader.CreateBitmap(background, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat,
                    center ? SKMatrix.CreateTranslation((result.Width - background.Width) / 2 % background.Width, (result.Height - background.Height) / 2 % background.Height) : SKMatrix.Identity);
                using SKPaint paint = new SKPaint { Shader = shader };
                canvas.DrawRect(0, 0, result.Width, result.Height, paint);
            }
            else
            {
                float aspectRatio = (float)background.Width / background.Height;
                int width = result.Width;
                int height = (int)(width / aspectRatio);

                if (height < result.Height)
                {
                    height = result.Height;
                    width = (int)(height * aspectRatio);
                }

                int x = center ? (result.Width - width) / 2 : 0;
                int y = center ? (result.Height - height) / 2 : 0;
                using SKImage image = SKImage.FromBitmap(background);
                canvas.DrawImage(image, new SKRect(x, y, x + width, y + height), new SKSamplingOptions(SKCubicResampler.Mitchell));
            }

            canvas.DrawBitmap(source, 0, 0);
            return result;
        }

        /// <summary>A border drawn with a pen, inside or outside the image, like ImageHelpers.DrawBorder.</summary>
        public static SKBitmap DrawBorder(SKBitmap source, int borderSize, BorderType borderType, LineDashStyle dashStyle, Color color, GradientInfo gradient = null)
        {
            bool outside = borderType == BorderType.Outside;
            SKBitmap result = outside ? CreateEmpty(source, borderSize * 2, borderSize * 2) : Clone(source);
            using SKCanvas canvas = new SKCanvas(result);

            if (outside)
            {
                canvas.DrawBitmap(source, borderSize, borderSize);
            }

            using SKPaint paint = new SKPaint { IsStroke = true, StrokeWidth = borderSize, IsAntialias = false };
            using SKShader shader = gradient != null && gradient.IsValid ? gradient.CreateShader(new SKRect(0, 0, result.Width, result.Height)) : null;

            if (shader != null)
            {
                paint.Shader = shader;
            }
            else
            {
                paint.Color = color.ToSKColor();
            }

            using SKPathEffect dash = CreateDash(dashStyle, borderSize);
            paint.PathEffect = dash;

            // An inset pen: the stroke is centred half its width inside the image edge.
            float inset = borderSize / 2f;
            canvas.DrawRect(inset, inset, result.Width - borderSize, result.Height - borderSize, paint);
            return result;
        }

        private static SKPathEffect CreateDash(LineDashStyle style, float width)
        {
            float w = Math.Max(1, width);

            return style switch
            {
                LineDashStyle.Dash => SKPathEffect.CreateDash([3 * w, w], 0),
                LineDashStyle.Dot => SKPathEffect.CreateDash([w, w], 0),
                LineDashStyle.DashDot => SKPathEffect.CreateDash([3 * w, w, w, w], 0),
                LineDashStyle.DashDotDot => SKPathEffect.CreateDash([3 * w, w, w, w, w, w], 0),
                _ => null
            };
        }

        /// <summary>The image followed by a fading, upside down copy, like ImageHelpers.DrawReflection.</summary>
        public static SKBitmap DrawReflection(SKBitmap source, int percentage, int maxAlpha, int minAlpha, int offset, bool skew, int skewSize)
        {
            percentage = Math.Clamp(percentage, 1, 100);
            maxAlpha = Math.Clamp(maxAlpha, 0, 255);
            minAlpha = Math.Clamp(minAlpha, 0, 255);
            int reflectionHeight = Math.Max(1, (int)(source.Height * ((float)percentage / 100)));

            SKBitmap reflection;

            using (SKBitmap flipped = Flip(source, false, true))
            using (SKBitmap cropped = Crop(flipped, new Rectangle(0, 0, flipped.Width, reflectionHeight)))
            using (SkiaPixels pixels = SkiaPixels.From(cropped))
            {
                int alphaAdd = maxAlpha - minAlpha;
                float height = pixels.Height - 1;

                for (int y = 0; y < pixels.Height; y++)
                {
                    byte alpha = (byte)(maxAlpha - (alphaAdd * (height > 0 ? y / height : 0)));

                    for (int x = 0; x < pixels.Width; x++)
                    {
                        uint color = pixels[x, y];

                        if (SkiaPixels.A(color) > alpha)
                        {
                            pixels[x, y] = (color & 0x00FFFFFF) | ((uint)alpha << 24);
                        }
                    }
                }

                reflection = pixels.ToBitmap();
            }

            if (skew)
            {
                using SKBitmap unskewed = reflection;
                reflection = Skew(unskewed, skewSize, 0);
            }

            using (reflection)
            {
                SKBitmap result = CreateEmpty(reflection.Width, source.Height + reflection.Height + offset);
                using SKCanvas canvas = new SKCanvas(result);
                canvas.DrawBitmap(source, 0, 0);
                canvas.DrawBitmap(reflection, 0, source.Height + offset);
                return result;
            }
        }

        /// <summary>A blurred, tinted copy behind the image, like ImageHelpers.AddShadow. Darkness 1 is the default.</summary>
        public static SKBitmap AddShadow(SKBitmap source, float opacity, int size, float darkness, Color color, Point offset, bool autoResize = true)
        {
            SKBitmap shadow = CreateEmpty(source, size * 2, size * 2);

            using (SKBitmap mask = ApplyColorMatrix(source, MaskMatrix(opacity, color)))
            using (SKCanvas canvas = new SKCanvas(shadow))
            {
                canvas.DrawBitmap(mask, size, size);
            }

            if (size > 0)
            {
                using SKBitmap unblurred = shadow;
                shadow = BoxBlur(unblurred, size);
            }

            if (darkness > 1)
            {
                using SKBitmap light = shadow;
                shadow = ApplyColorMatrix(light, AlphaMatrix(darkness));
            }

            using (shadow)
            {
                SKBitmap result;

                if (autoResize)
                {
                    result = CreateEmpty(shadow, Math.Abs(offset.X), Math.Abs(offset.Y));
                    using SKCanvas canvas = new SKCanvas(result);
                    canvas.DrawBitmap(shadow, Math.Max(0, offset.X), Math.Max(0, offset.Y));
                    canvas.DrawBitmap(source, Math.Max(size, -offset.X + size), Math.Max(size, -offset.Y + size));
                }
                else
                {
                    result = CreateEmpty(source);
                    using SKCanvas canvas = new SKCanvas(result);
                    canvas.DrawBitmap(shadow, -size + offset.X, -size + offset.Y);
                    canvas.DrawBitmap(source, 0, 0);
                }

                return result;
            }
        }

        /// <summary>A blurred halo in a colour or gradient behind the image, like ImageHelpers.AddGlow.</summary>
        public static SKBitmap AddGlow(SKBitmap source, int size, float strength, Color color, Point offset, GradientInfo gradient = null)
        {
            if (size < 0 || strength < 0.1f)
            {
                return Clone(source);
            }

            SKBitmap blurred = size > 0 ? BoxBlur(AddCanvas(source, new Insets(size), Color.Transparent) ?? Clone(source), size) : Clone(source);

            using (blurred)
            using (SKBitmap mask = gradient != null && gradient.IsValid ? CreateGradientMask(blurred, gradient, strength) : ApplyColorMatrix(blurred, MaskMatrix(strength, color)))
            {
                SKBitmap result = CreateEmpty(mask, Math.Abs(offset.X), Math.Abs(offset.Y));
                using SKCanvas canvas = new SKCanvas(result);
                canvas.DrawBitmap(mask, Math.Max(0, offset.X), Math.Max(0, offset.Y));
                canvas.DrawBitmap(source, Math.Max(size, -offset.X + size), Math.Max(size, -offset.Y + size));
                return result;
            }
        }

        private static SKBitmap CreateGradientMask(SKBitmap source, GradientInfo gradient, float opacity)
        {
            SKBitmap gradientBitmap = CreateEmpty(source);

            using (SKCanvas canvas = new SKCanvas(gradientBitmap))
            {
                gradient.Draw(canvas, new SKRect(0, 0, source.Width, source.Height));
            }

            using (gradientBitmap)
            using (SkiaPixels src = SkiaPixels.From(source))
            using (SkiaPixels mask = SkiaPixels.From(gradientBitmap))
            {
                for (int i = 0; i < mask.PixelCount; i++)
                {
                    uint maskColor = mask[i];
                    byte alpha = (byte)Math.Min(255, SkiaPixels.A(src[i]) * (SkiaPixels.A(maskColor) / 255f) * opacity);
                    mask[i] = (maskColor & 0x00FFFFFF) | ((uint)alpha << 24);
                }

                return mask.ToBitmap();
            }
        }

        /// <summary>An outline around the opaque parts of the image, optionally on its own, like ImageHelpers.Outline.</summary>
        public static SKBitmap Outline(SKBitmap source, int borderSize, Color borderColor, int padding = 0, bool outlineOnly = false)
        {
            int minRadius = padding;
            int maxRadius = padding + borderSize + 1;

            using SkiaPixels src = SkiaPixels.From(source);
            using SkiaPixels outline = SkiaPixels.Create(src.Width, src.Height);

            Parallel.For(0, src.Width, x =>
            {
                for (int y = 0; y < src.Height; y++)
                {
                    float dist = DistanceToOpaque(src, x, y, maxRadius);

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

                        outline[x, y] = SkiaPixels.Pack(borderColor.B, borderColor.G, borderColor.R, alpha);
                    }
                }
            });

            SKBitmap outlineBitmap = outline.ToBitmap();

            if (outlineOnly)
            {
                return outlineBitmap;
            }

            using (outlineBitmap)
            {
                SKBitmap result = Clone(source);
                using SKCanvas canvas = new SKCanvas(result);
                canvas.DrawBitmap(outlineBitmap, 0, 0);
                return result;
            }
        }

        private static float DistanceToOpaque(SkiaPixels pixels, int x, int y, int radius)
        {
            int minX = Math.Max(x - radius, 0);
            int maxX = Math.Min(x + radius, pixels.Width - 1);
            int minY = Math.Max(y - radius, 0);
            int maxY = Math.Min(y + radius, pixels.Height - 1);
            int dist2 = (radius * radius) + 1;

            for (int tx = minX; tx <= maxX; tx++)
            {
                for (int ty = minY; ty <= maxY; ty++)
                {
                    if (SkiaPixels.A(pixels[tx, ty]) >= 255)
                    {
                        int dx = tx - x;
                        int dy = ty - y;
                        int testDist2 = (dx * dx) + (dy * dy);

                        if (testDist2 < dist2)
                        {
                            dist2 = testDist2;
                        }
                    }
                }
            }

            return (float)Math.Sqrt(dist2);
        }

        /// <summary>Horizontal bands shifted left or right by random amounts, like ImageHelpers.Slice.</summary>
        public static SKBitmap Slice(SKBitmap source, int minSliceHeight, int maxSliceHeight, int minSliceShift, int maxSliceShift)
        {
            if (minSliceHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(minSliceHeight));
            }

            if (maxSliceHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSliceHeight));
            }

            SKBitmap result = CreateEmpty(source);
            using SKCanvas canvas = new SKCanvas(result);
            int y = 0;

            while (y < source.Height)
            {
                int height = RandomFast.Next(minSliceHeight, maxSliceHeight);
                // RandomFast.Next(1) is always 0, so the GDI+ version always shifted left; keep that.
                int shift = RandomFast.Next(1) == 0 ? RandomFast.Next(-maxSliceShift, -minSliceShift) : RandomFast.Next(minSliceShift, maxSliceShift);
                SKRect sourceRect = new SKRect(0, y, source.Width, y + height);
                canvas.DrawBitmap(source, sourceRect, SKRect.Create(shift, y, source.Width, height));
                y += height;
            }

            return result;
        }

        /// <summary>Ragged edges, like ImageHelpers.TornEdges.</summary>
        public static SKBitmap TornEdges(SKBitmap source, int tornDepth, int tornRange, AnchorSides sides, bool curvedEdges, bool random)
        {
            if (tornDepth < 1 || tornRange < 1 || sides == AnchorSides.None)
            {
                return Clone(source);
            }

            int width = source.Width;
            int height = source.Height;
            int horizontalTornCount = width / tornRange;
            int verticalTornCount = height / tornRange;

            if (horizontalTornCount < 2 && verticalTornCount < 2)
            {
                return Clone(source);
            }

            List<Point> points = new List<Point>();

            if (sides.HasFlag(AnchorSides.Top) && horizontalTornCount > 1)
            {
                int startX = (sides.HasFlag(AnchorSides.Left) && verticalTornCount > 1) ? tornDepth : 0;
                int endX = (sides.HasFlag(AnchorSides.Right) && verticalTornCount > 1) ? width - tornDepth : width;

                for (int x = startX; x < endX; x += tornRange)
                {
                    points.Add(new Point(x, random ? RandomFast.Next(0, tornDepth) : ((x / tornRange) & 1) * tornDepth));
                }
            }
            else
            {
                points.Add(new Point(0, 0));
                points.Add(new Point(width, 0));
            }

            if (sides.HasFlag(AnchorSides.Right) && verticalTornCount > 1)
            {
                int startY = (sides.HasFlag(AnchorSides.Top) && horizontalTornCount > 1) ? tornDepth : 0;
                int endY = (sides.HasFlag(AnchorSides.Bottom) && horizontalTornCount > 1) ? height - tornDepth : height;

                for (int y = startY; y < endY; y += tornRange)
                {
                    int x = random ? RandomFast.Next(0, tornDepth) : ((y / tornRange) & 1) * tornDepth;
                    points.Add(new Point(width - tornDepth + x, y));
                }
            }
            else
            {
                points.Add(new Point(width, 0));
                points.Add(new Point(width, height));
            }

            if (sides.HasFlag(AnchorSides.Bottom) && horizontalTornCount > 1)
            {
                int startX = (sides.HasFlag(AnchorSides.Right) && verticalTornCount > 1) ? width - tornDepth : width;
                int endX = (sides.HasFlag(AnchorSides.Left) && verticalTornCount > 1) ? tornDepth : 0;

                for (int x = startX; x >= endX; x = ((x / tornRange) - 1) * tornRange)
                {
                    int y = random ? RandomFast.Next(0, tornDepth) : ((x / tornRange) & 1) * tornDepth;
                    points.Add(new Point(x, height - tornDepth + y));
                }
            }
            else
            {
                points.Add(new Point(width, height));
                points.Add(new Point(0, height));
            }

            if (sides.HasFlag(AnchorSides.Left) && verticalTornCount > 1)
            {
                int startY = (sides.HasFlag(AnchorSides.Bottom) && horizontalTornCount > 1) ? height - tornDepth : height;
                int endY = (sides.HasFlag(AnchorSides.Top) && horizontalTornCount > 1) ? tornDepth : 0;

                for (int y = startY; y >= endY; y = ((y / tornRange) - 1) * tornRange)
                {
                    int x = random ? RandomFast.Next(0, tornDepth) : ((y / tornRange) & 1) * tornDepth;
                    points.Add(new Point(x, y));
                }
            }
            else
            {
                points.Add(new Point(0, height));
                points.Add(new Point(0, 0));
            }

            return FillShape(source, points.Distinct().ToList(), curvedEdges);
        }

        /// <summary>Wavy edges, like ImageHelpers.WavyEdges.</summary>
        public static SKBitmap WavyEdges(SKBitmap source, int waveDepth, int waveRange, AnchorSides sides)
        {
            if (waveDepth < 1 || waveRange < 1 || sides == AnchorSides.None)
            {
                return Clone(source);
            }

            int width = source.Width;
            int height = source.Height;
            List<Point> points = new List<Point>();
            int horizontalWaveCount = Math.Max(2, ((width / waveRange) + 1) / 2 * 2) - 1;
            int verticalWaveCount = Math.Max(2, ((height / waveRange) + 1) / 2 * 2) - 1;
            int horizontalWaveRange = width / horizontalWaveCount;
            int verticalWaveRange = height / verticalWaveCount;
            int step = Math.Min(Math.Max(1, waveRange / waveDepth), 10);
            int Wave(int t, int max, int depth) => (int)((1 - Math.Cos(t * Math.PI / max)) * depth / 2);

            if (sides.HasFlag(AnchorSides.Top))
            {
                int startX = sides.HasFlag(AnchorSides.Left) ? waveDepth : 0;
                int endX = sides.HasFlag(AnchorSides.Right) ? width - waveDepth : width;

                for (int x = startX; x < endX; x += step)
                {
                    points.Add(new Point(x, Wave(x, horizontalWaveRange, waveDepth)));
                }

                points.Add(new Point(endX, Wave(endX, horizontalWaveRange, waveDepth)));
            }
            else
            {
                points.Add(new Point(0, 0));
            }

            if (sides.HasFlag(AnchorSides.Right))
            {
                int startY = sides.HasFlag(AnchorSides.Top) ? waveDepth : 0;
                int endY = sides.HasFlag(AnchorSides.Bottom) ? height - waveDepth : height;

                for (int y = startY; y < endY; y += step)
                {
                    points.Add(new Point(width - waveDepth + Wave(y, verticalWaveRange, waveDepth), y));
                }

                points.Add(new Point(width - waveDepth + Wave(endY, verticalWaveRange, waveDepth), endY));
            }
            else
            {
                points.Add(new Point(width, points[points.Count - 1].Y));
            }

            if (sides.HasFlag(AnchorSides.Bottom))
            {
                int startX = sides.HasFlag(AnchorSides.Right) ? width - waveDepth : width;
                int endX = sides.HasFlag(AnchorSides.Left) ? waveDepth : 0;

                for (int x = startX; x >= endX; x -= step)
                {
                    points.Add(new Point(x, height - waveDepth + Wave(x, horizontalWaveRange, waveDepth)));
                }

                points.Add(new Point(endX, height - waveDepth + Wave(endX, horizontalWaveRange, waveDepth)));
            }
            else
            {
                points.Add(new Point(points[points.Count - 1].X, height));
            }

            if (sides.HasFlag(AnchorSides.Left))
            {
                int startY = sides.HasFlag(AnchorSides.Bottom) ? height - waveDepth : height;
                int endY = sides.HasFlag(AnchorSides.Top) ? waveDepth : 0;

                for (int y = startY; y >= endY; y -= step)
                {
                    points.Add(new Point(Wave(y, verticalWaveRange, waveDepth), y));
                }

                points.Add(new Point(Wave(endY, verticalWaveRange, waveDepth), endY));
            }
            else
            {
                points.Add(new Point(0, points[points.Count - 1].Y));
            }

            if (!sides.HasFlag(AnchorSides.Top))
            {
                points[0] = new Point(points[points.Count - 1].X, 0);
            }

            return FillShape(source, points, curved: false);
        }

        /// <summary>The image clipped to a polygon, or to a closed curve through the points.</summary>
        private static SKBitmap FillShape(SKBitmap source, List<Point> points, bool curved)
        {
            SKBitmap result = CreateEmpty(source);

            if (points.Count < 3)
            {
                return result;
            }

            using SKCanvas canvas = new SKCanvas(result);
            using SKPath path = curved ? CreateClosedCurve(points) : CreatePolygon(points);
            using SKShader shader = SKShader.CreateBitmap(source, SKShaderTileMode.Clamp, SKShaderTileMode.Clamp);
            using SKPaint paint = new SKPaint { Shader = shader, IsAntialias = true };
            canvas.DrawPath(path, paint);
            return result;
        }

        private static SKPath CreatePolygon(List<Point> points)
        {
            SKPath path = new SKPath();
            path.AddPoly(points.Select(p => new SKPoint(p.X, p.Y)).ToArray(), close: true);
            return path;
        }

        /// <summary>A closed cardinal spline through the points with GDI+'s default tension of 0.5.</summary>
        private static SKPath CreateClosedCurve(List<Point> points)
        {
            const float tension = 0.5f / 3f;
            SKPath path = new SKPath();
            int count = points.Count;
            path.MoveTo(points[0].X, points[0].Y);

            for (int i = 0; i < count; i++)
            {
                Point p0 = points[(i - 1 + count) % count];
                Point p1 = points[i];
                Point p2 = points[(i + 1) % count];
                Point p3 = points[(i + 2) % count];
                path.CubicTo(
                    p1.X + ((p2.X - p0.X) * tension), p1.Y + ((p2.Y - p0.Y) * tension),
                    p2.X - ((p3.X - p1.X) * tension), p2.Y - ((p3.Y - p1.Y) * tension),
                    p2.X, p2.Y);
            }

            path.Close();
            return path;
        }

        #endregion Drawing

        #region Placement and text

        /// <summary>Top left corner for an object of the given size placed in the background, like Helpers.GetPosition.</summary>
        public static Point GetPosition(ImageAlignment placement, Point offset, Size backgroundSize, Size objectSize)
        {
            int midX = (int)Math.Round((backgroundSize.Width / 2f) - (objectSize.Width / 2f));
            int midY = (int)Math.Round((backgroundSize.Height / 2f) - (objectSize.Height / 2f));
            int right = backgroundSize.Width - objectSize.Width;
            int bottom = backgroundSize.Height - objectSize.Height;

            return placement switch
            {
                ImageAlignment.TopCenter => new Point(midX, offset.Y),
                ImageAlignment.TopRight => new Point(right - offset.X, offset.Y),
                ImageAlignment.MiddleLeft => new Point(offset.X, midY),
                ImageAlignment.MiddleCenter => new Point(midX, midY),
                ImageAlignment.MiddleRight => new Point(right - offset.X, midY),
                ImageAlignment.BottomLeft => new Point(offset.X, bottom - offset.Y),
                ImageAlignment.BottomCenter => new Point(midX, bottom - offset.Y),
                ImageAlignment.BottomRight => new Point(right - offset.X, bottom - offset.Y),
                _ => new Point(offset.X, offset.Y)
            };
        }

        /// <summary>Rotates then flips, with the same numbering as System.Drawing.RotateFlipType (0 none to 7 rotate 90 and flip Y).</summary>
        public static SKBitmap RotateFlip(SKBitmap source, int rotateFlipType)
        {
            int rotation = (rotateFlipType & 3) * 90;
            bool flipX = (rotateFlipType & 4) != 0;
            bool swap = rotation == 90 || rotation == 270;
            int width = swap ? source.Height : source.Width;
            int height = swap ? source.Width : source.Height;
            SKBitmap result = CreateEmpty(width, height);
            using SKCanvas canvas = new SKCanvas(result);
            canvas.Translate(width / 2f, height / 2f);

            if (flipX)
            {
                canvas.Scale(-1, 1);
            }

            canvas.RotateDegrees(rotation);
            canvas.Translate(-source.Width / 2f, -source.Height / 2f);
            canvas.DrawBitmap(source, 0, 0);
            return result;
        }

        /// <summary>The size of the text drawn in the font, close to what GDI+ MeasureString returned (it adds a little padding).</summary>
        public static Size MeasureText(string text, FontInfo fontInfo)
        {
            using SKFont font = fontInfo.CreateFont();
            string[] lines = text.Replace("\r\n", "\n").Split('\n');
            float width = lines.Max(line => font.MeasureText(line));
            float lineHeight = font.Spacing;
            // GDI+ MeasureString pads by about a sixth of the em size on each side.
            float padding = fontInfo.SizeInPixels / 6f;
            return new Size((int)Math.Ceiling(width + (padding * 2)), (int)Math.Ceiling(lineHeight * lines.Length));
        }

        /// <summary>Draws text with its top left corner at the point, the same anchor GDI+ DrawString uses.</summary>
        public static void DrawText(SKCanvas canvas, string text, FontInfo fontInfo, SKPaint paint, float x, float y)
        {
            using SKFont font = fontInfo.CreateFont();
            float padding = fontInfo.SizeInPixels / 6f;
            float lineY = y - font.Metrics.Ascent;

            foreach (string line in text.Replace("\r\n", "\n").Split('\n'))
            {
                canvas.DrawText(line, x + padding, lineY, SKTextAlign.Left, font, paint);
                lineY += font.Spacing;
            }
        }

        /// <summary>The outline of the text as a path, with the top left of its line box at the origin, for outlined and gradient text.</summary>
        public static SKPath GetTextPath(string text, FontInfo fontInfo)
        {
            using SKFont font = fontInfo.CreateFont();
            SKPath path = new SKPath();
            float lineY = -font.Metrics.Ascent;

            foreach (string line in text.Replace("\r\n", "\n").Split('\n'))
            {
                using SKPath linePath = font.GetTextPath(line, new SKPoint(0, lineY));
                path.AddPath(linePath);
                lineY += font.Spacing;
            }

            return path;
        }

        #endregion Placement and text
    }
}
