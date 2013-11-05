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
using System.Drawing.Imaging;

namespace ImageEffectsLib
{
    internal class MatrixColor : ImageEffect
    {
        [DefaultValue(1f), Description("Red = (Red * Rr) + (Green * Rg) + (Blue * Rb) + (Alpha * Ra) + Ro")]
        public float Rr { get; set; }
        [DefaultValue(0f)]
        public float Rg { get; set; }
        [DefaultValue(0f)]
        public float Rb { get; set; }
        [DefaultValue(0f)]
        public float Ra { get; set; }
        [DefaultValue(0f)]
        public float Ro { get; set; }

        [DefaultValue(0f)]
        public float Gr { get; set; }
        [DefaultValue(1f)]
        public float Gg { get; set; }
        [DefaultValue(0f)]
        public float Gb { get; set; }
        [DefaultValue(0f)]
        public float Ga { get; set; }
        [DefaultValue(0f)]
        public float Go { get; set; }

        [DefaultValue(0f)]
        public float Br { get; set; }
        [DefaultValue(0f)]
        public float Bg { get; set; }
        [DefaultValue(1f)]
        public float Bb { get; set; }
        [DefaultValue(0f)]
        public float Ba { get; set; }
        [DefaultValue(0f)]
        public float Bo { get; set; }

        [DefaultValue(0f)]
        public float Ar { get; set; }
        [DefaultValue(0f)]
        public float Ag { get; set; }
        [DefaultValue(0f)]
        public float Ab { get; set; }
        [DefaultValue(1f)]
        public float Aa { get; set; }
        [DefaultValue(0f)]
        public float Ao { get; set; }

        public MatrixColor()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Image Apply(Image img)
        {
            ColorMatrix colorMatrix = new ColorMatrix(new[]
            {
                new float[] { Rr, Gr, Br, Ar, 0 },
                new float[] { Rg, Gg, Bg, Ag, 0 },
                new float[] { Rb, Gb, Bb, Ab, 0 },
                new float[] { Ra, Ga, Ba, Aa, 0 },
                new float[] { Ro, Go, Bo, Ao, 1 }
            });

            using (img)
            {
                return colorMatrix.Apply(img);
            }
        }
    }
}