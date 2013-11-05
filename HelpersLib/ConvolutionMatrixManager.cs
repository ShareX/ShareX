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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace HelpersLib
{
    public static class ConvolutionMatrixManager
    {
        public static Image Apply(this ConvolutionMatrix matrix, Image img)
        {
            Bitmap result = (Bitmap)img.Clone();

            using (UnsafeBitmap source = new UnsafeBitmap((Bitmap)img, true, ImageLockMode.ReadOnly))
            using (UnsafeBitmap dest = new UnsafeBitmap(result, true, ImageLockMode.WriteOnly))
            {
                ColorBgra[,] pixelColor = new ColorBgra[3, 3];
                ColorBgra color = new ColorBgra();

                for (int y = 0; y < source.Height - 2; y++)
                {
                    for (int x = 0; x < source.Width - 2; x++)
                    {
                        pixelColor[0, 0] = source.GetPixel(x, y);
                        pixelColor[0, 1] = source.GetPixel(x, y + 1);
                        pixelColor[0, 2] = source.GetPixel(x, y + 2);
                        pixelColor[1, 0] = source.GetPixel(x + 1, y);
                        pixelColor[1, 1] = source.GetPixel(x + 1, y + 1);
                        pixelColor[1, 2] = source.GetPixel(x + 1, y + 2);
                        pixelColor[2, 0] = source.GetPixel(x + 2, y);
                        pixelColor[2, 1] = source.GetPixel(x + 2, y + 1);
                        pixelColor[2, 2] = source.GetPixel(x + 2, y + 2);

                        color.Alpha = pixelColor[1, 1].Alpha;

                        color.Red = (byte)(((((pixelColor[0, 0].Red * matrix.Matrix[0, 0]) +
                            (pixelColor[1, 0].Red * matrix.Matrix[1, 0]) +
                            (pixelColor[2, 0].Red * matrix.Matrix[2, 0]) +
                            (pixelColor[0, 1].Red * matrix.Matrix[0, 1]) +
                            (pixelColor[1, 1].Red * matrix.Matrix[1, 1]) +
                            (pixelColor[2, 1].Red * matrix.Matrix[2, 1]) +
                            (pixelColor[0, 2].Red * matrix.Matrix[0, 2]) +
                            (pixelColor[1, 2].Red * matrix.Matrix[1, 2]) +
                            (pixelColor[2, 2].Red * matrix.Matrix[2, 2])) / matrix.Factor) + matrix.Offset).Between(0, 255));

                        color.Green = (byte)(((((pixelColor[0, 0].Green * matrix.Matrix[0, 0]) +
                            (pixelColor[1, 0].Green * matrix.Matrix[1, 0]) +
                            (pixelColor[2, 0].Green * matrix.Matrix[2, 0]) +
                            (pixelColor[0, 1].Green * matrix.Matrix[0, 1]) +
                            (pixelColor[1, 1].Green * matrix.Matrix[1, 1]) +
                            (pixelColor[2, 1].Green * matrix.Matrix[2, 1]) +
                            (pixelColor[0, 2].Green * matrix.Matrix[0, 2]) +
                            (pixelColor[1, 2].Green * matrix.Matrix[1, 2]) +
                            (pixelColor[2, 2].Green * matrix.Matrix[2, 2])) / matrix.Factor) + matrix.Offset).Between(0, 255));

                        color.Blue = (byte)(((((pixelColor[0, 0].Blue * matrix.Matrix[0, 0]) +
                            (pixelColor[1, 0].Blue * matrix.Matrix[1, 0]) +
                            (pixelColor[2, 0].Blue * matrix.Matrix[2, 0]) +
                            (pixelColor[0, 1].Blue * matrix.Matrix[0, 1]) +
                            (pixelColor[1, 1].Blue * matrix.Matrix[1, 1]) +
                            (pixelColor[2, 1].Blue * matrix.Matrix[2, 1]) +
                            (pixelColor[0, 2].Blue * matrix.Matrix[0, 2]) +
                            (pixelColor[1, 2].Blue * matrix.Matrix[1, 2]) +
                            (pixelColor[2, 2].Blue * matrix.Matrix[2, 2])) / matrix.Factor) + matrix.Offset).Between(0, 255));

                        dest.SetPixel(x + 1, y + 1, color);
                    }
                }
            }

            return result;
        }

        public static ConvolutionMatrix Sharpen(int weight = 11)
        {
            ConvolutionMatrix matrix = new ConvolutionMatrix();
            matrix.SetAll(0);
            matrix.Matrix[1, 1] = weight;
            matrix.Matrix[1, 0] = matrix.Matrix[0, 1] = matrix.Matrix[2, 1] = matrix.Matrix[1, 2] = -2;
            matrix.Factor = Math.Max(weight - 8, 1);
            return matrix;
        }
    }
}