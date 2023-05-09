#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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

namespace ShareX.ImageEffectsLib
{
    [Description("Gaussian blur")]
    internal class GaussianBlur : ImageEffect
    {
        private double sigma;
        private int size;

        [DefaultValue(0.7955555)]
        public double Sigma
        {
            get => sigma;
            set => sigma = Math.Max(value, 0.1);
        }

        [DefaultValue(3)]
        public int Size
        {
            get => size;
            set
            {
                size = value.Max(1);

                if (size.IsEvenNumber())
                {
                    size++;
                }
            }
        }

        public GaussianBlur()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            ConvolutionMatrix kernelHoriz = ConvolutionMatrixManager.GaussianBlur(1, size, sigma);

            ConvolutionMatrix kernelVert = new ConvolutionMatrix(size, 1)
            {
                ConsiderAlpha = kernelHoriz.ConsiderAlpha
            };

            for (int i = 0; i < size; i++)
            {
                kernelVert[i, 0] = kernelHoriz[0, i];
            }

            using (bmp)
            using (Bitmap horizPass = kernelHoriz.Apply(bmp))
            {
                return kernelVert.Apply(horizPass);
            }
        }
    }
}