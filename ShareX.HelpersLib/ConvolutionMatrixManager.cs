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

// Adapted from https://stackoverflow.com/questions/33569396/correctly-implement-a-2-pass-gaussian-blur
// Filters: http://www.codeproject.com/Articles/2008/Image-Processing-for-Dummies-with-C-and-GDI-Part-2

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public static class ConvolutionMatrixManager
    {
        public static Bitmap Apply(this ConvolutionMatrix kernel, Bitmap bmp)
        {
            Bitmap bmpResult = (Bitmap)bmp.Clone();

            using (UnsafeBitmap source = new UnsafeBitmap(bmp, true, ImageLockMode.ReadOnly))
            using (UnsafeBitmap dest = new UnsafeBitmap(bmpResult, true, ImageLockMode.WriteOnly))
            {
                int originX = (kernel.Width - 1) / 2;
                int originY = (kernel.Height - 1) / 2;

                Parallel.For(0, source.Height, y =>
                {
                    Parallel.For(0, source.Width, x =>
                    {
                        double r = 0.0;
                        double g = 0.0;
                        double b = 0.0;
                        double a = 0.0;

                        // Apply each matrix multiplier to the color components for each pixel.
                        for (int fy = 0; fy < kernel.Height; fy++)
                        {
                            int fyr = fy - originY;
                            int offsetY = y + fyr;

                            offsetY = offsetY.Clamp(0, source.Height - 1);

                            for (int fx = 0; fx < kernel.Width; fx++)
                            {
                                int fxr = fx - originX;
                                int offsetX = x + fxr;

                                offsetX = offsetX.Clamp(0, source.Width - 1);

                                ColorBgra currentColor = source.GetPixel(offsetX, offsetY);

                                r += kernel[fy, fx] * currentColor.Red;
                                g += kernel[fy, fx] * currentColor.Green;
                                b += kernel[fy, fx] * currentColor.Blue;
                                if (kernel.ConsiderAlpha)
                                {
                                    a += kernel[fy, fx] * currentColor.Alpha;
                                }
                            }
                        }

                        r += kernel.Offset;
                        r = r.Clamp(0, 255);

                        g += kernel.Offset;
                        g = g.Clamp(0, 255);

                        b += kernel.Offset;
                        b = b.Clamp(0, 255);

                        if (kernel.ConsiderAlpha)
                        {
                            a += kernel.Offset;
                            a = a.Clamp(0, 255);
                        }

                        dest.SetPixel(x, y, new ColorBgra((byte)b, (byte)g, (byte)r, kernel.ConsiderAlpha ? (byte)a : source.GetPixel(x, y).Alpha));
                    });
                });
            }

            return bmpResult;
        }

        public static ConvolutionMatrix Smooth(int weight = 1)
        {
            ConvolutionMatrix cm = new ConvolutionMatrix();
            double factor = weight + 8;
            cm.SetAll(1 / factor);
            cm[1, 1] = weight / factor;
            return cm;
        }

        private static double GaussianFunction(double x, double sigma)
        {
            double left = 1.0 / (Math.Sqrt(2 * Math.PI) * sigma);

            double exponentNumerator = -x * x;
            double exponentDenominator = 2 * Math.Pow(sigma, 2);
            double right = Math.Exp(exponentNumerator / exponentDenominator);

            return left * right;
        }

        public static ConvolutionMatrix GaussianBlur(int height, int width, double sigma)
        {
            ConvolutionMatrix cm = new ConvolutionMatrix(height, width);
            cm.ConsiderAlpha = true;

            double sum = 0.0;
            double midpointX = (width - 1) / 2.0;
            double midpointY = (height - 1) / 2.0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sum += cm[y, x] = GaussianFunction(x - midpointX, sigma) * GaussianFunction(y - midpointY, sigma);
                }
            }

            // Normalise kernel so that the sum of all weights equals 1
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    cm[y, x] /= sum;
                }
            }

            return cm;
        }

        public static ConvolutionMatrix MeanRemoval(int weight = 9)
        {
            ConvolutionMatrix cm = new ConvolutionMatrix();
            double factor = weight - 8;
            cm.SetAll(-1 / factor);
            cm[1, 1] = weight / factor;
            return cm;
        }

        public static ConvolutionMatrix Sharpen(int weight = 11)
        {
            ConvolutionMatrix cm = new ConvolutionMatrix();
            double factor = weight - 8;
            cm.SetAll(0);
            cm[1, 1] = weight / factor;
            cm[1, 0] = cm[0, 1] = cm[2, 1] = cm[1, 2] = -2 / factor;
            return cm;
        }

        public static ConvolutionMatrix Emboss()
        {
            ConvolutionMatrix cm = new ConvolutionMatrix();
            cm.SetAll(-1);
            cm[1, 1] = 4;
            cm[1, 0] = cm[0, 1] = cm[2, 1] = cm[1, 2] = 0;
            cm.Offset = 127;
            return cm;
        }

        public static ConvolutionMatrix EdgeDetect()
        {
            ConvolutionMatrix cm = new ConvolutionMatrix();
            cm[0, 0] = cm[0, 1] = cm[0, 2] = -1;
            cm[1, 0] = cm[1, 1] = cm[1, 2] = 0;
            cm[2, 0] = cm[2, 1] = cm[2, 2] = 1;
            cm.Offset = 127;
            return cm;
        }
    }
}