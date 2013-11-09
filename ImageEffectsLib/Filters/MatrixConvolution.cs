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

using HelpersLib;
using System.ComponentModel;
using System.Drawing;

namespace ImageEffectsLib
{
    [Description("Convolution matrix")]
    internal class MatrixConvolution : ImageEffect
    {
        [DefaultValue(0)]
        public int X0Y0 { get; set; }
        [DefaultValue(0)]
        public int X1Y0 { get; set; }
        [DefaultValue(0)]
        public int X2Y0 { get; set; }

        [DefaultValue(0)]
        public int X0Y1 { get; set; }
        [DefaultValue(1)]
        public int X1Y1 { get; set; }
        [DefaultValue(0)]
        public int X2Y1 { get; set; }

        [DefaultValue(0)]
        public int X0Y2 { get; set; }
        [DefaultValue(0)]
        public int X1Y2 { get; set; }
        [DefaultValue(0)]
        public int X2Y2 { get; set; }

        [DefaultValue(1)]
        public int Factor { get; set; }

        [DefaultValue(0)]
        public int Offset { get; set; }

        public MatrixConvolution()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Image Apply(Image img)
        {
            using (img)
            {
                ConvolutionMatrix cm = new ConvolutionMatrix();
                cm.Matrix[0, 0] = X0Y0;
                cm.Matrix[1, 0] = X1Y0;
                cm.Matrix[2, 0] = X2Y0;
                cm.Matrix[0, 1] = X0Y1;
                cm.Matrix[1, 1] = X1Y1;
                cm.Matrix[2, 1] = X2Y1;
                cm.Matrix[0, 2] = X0Y2;
                cm.Matrix[1, 2] = X1Y2;
                cm.Matrix[2, 2] = X2Y2;
                cm.Factor = Factor;
                cm.Offset = Offset;
                return cm.Apply(img);
            }
        }
    }
}