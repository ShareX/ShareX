#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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

using ShareX.HelpersLib;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;

// Adapted from https://stackoverflow.com/questions/33569396/correctly-implement-a-2-pass-gaussian-blur

namespace ShareX.ImageEffectsLib
{
    [Description("Gaussian blur")]
    internal class GaussianBlur : ImageEffect
    {
        private double _sigma;
        private int _size;

        private double[,] _cachedKernelHoriz;
        private double[,] _cachedKernelVert;

        [DefaultValue(0.7955555)]
        public double Sigma
        {
            get => _sigma;
            set
            {
                _sigma = Math.Max(value, 0.1);

                CreateKernels(out _cachedKernelHoriz, out _cachedKernelVert);
            }
        }

        [DefaultValue(3)]
        public int Size
        {
            get => _size;
            set
            {
                _size = value.Min(1);

                if (_size.IsEvenNumber())
                {
                    _size++;
                }

                CreateKernels(out _cachedKernelHoriz, out _cachedKernelVert);
            }
        }

        public GaussianBlur()
        {
            this.ApplyDefaultPropertyValues();
        }

        private double Gaussian(double x)
        {
            double left = 1.0 / (Math.Sqrt(2 * Math.PI) * _sigma);

            double exponentNumerator = -x * x;
            double exponentDenominator = 2 * Math.Pow(_sigma, 2);
            double right = Math.Exp(exponentNumerator / exponentDenominator);

            return left * right;
        }

        private void CreateKernels(out double[,] horiz, out double[,] vert)
        {
            horiz = new double[1, _size];

            double sum = 0.0;
            double midpoint = (_size - 1) / 2.0;

            for (int i = 0; i < _size; i++)
            {
                sum += horiz[0, i] = Gaussian(i - midpoint);
            }

            // Normalise kernel so that the sum of all weights equals 1
            for (int i = 0; i < _size; i++)
            {
                horiz[0, i] /= sum;
            }

            // Copy the kernel into the vertical
            vert = new double[_size, 1];
            for (int i = 0; i < _size; i++)
            {
                vert[i, 0] = horiz[0, i];
            }
        }

        private static void ApplyKernel(UnsafeBitmap bmp, double[,] kernel)
        {
            int kernelHeight = kernel.GetLength(0);
            int kernelWidth = kernel.GetLength(1);

            int originX = (kernelWidth - 1) / 2;
            int originY = (kernelHeight - 1) / 2;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    double r = 0.0;
                    double g = 0.0;
                    double b = 0.0;

                    // Apply each matrix multiplier to the color components for each pixel.
                    for (int fy = 0; fy < kernelHeight; fy++)
                    {
                        int fyr = fy - originY;
                        int offsetY = y + fyr;

                        offsetY.Clamp(0, bmp.Height - 1);

                        for (int fx = 0; fx < kernelWidth; fx++)
                        {
                            int fxr = fx - originX;
                            int offsetX = x + fxr;

                            offsetX.Clamp(0, bmp.Width - 1);

                            ColorBgra currentColor = bmp.GetPixel(offsetX, offsetY);

                            r += kernel[fy, fx] * currentColor.Red;
                            g += kernel[fy, fx] * currentColor.Green;
                            b += kernel[fy, fx] * currentColor.Blue;
                        }
                    }

                    bmp.SetPixel(x, y, new ColorBgra((byte)b, (byte)g, (byte)r, bmp.GetPixel(x, y).Alpha));
                }
            }
        }

        public override Image Apply(Image img)
        {
            using (img)
            {
                Bitmap result = (Bitmap)img.Clone();

                using (UnsafeBitmap dest = new UnsafeBitmap(result, true, ImageLockMode.ReadWrite))
                {
                    ApplyKernel(dest, _cachedKernelHoriz);
                    ApplyKernel(dest, _cachedKernelVert);
                }

                return result;
            }
        }
    }
}