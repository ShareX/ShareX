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

using ShareX.HelpersLib;
using System.ComponentModel;
using System.Drawing;

namespace ShareX.ImageEffectsLib
{
    internal class Rotate : ImageEffect
    {
        [DefaultValue(0f), Description("Choose a value between -360 and 360.")]
        public float Angle { get; set; }

        [DefaultValue(true), Description("If true, output image will be larger than the input and no clipping will occur.")]
        public bool Upsize { get; set; }

        [DefaultValue(false), Description("Upsize must be false for this setting to work. If true, clipping will occur or else image size will be reduced.")]
        public bool Clip { get; set; }

        public Rotate()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            if (Angle == 0)
            {
                return bmp;
            }

            using (bmp)
            {
                return ImageHelpers.RotateImage(bmp, Angle, Upsize, Clip);
            }
        }

        protected override string GetSummary()
        {
            return Angle + "°";
        }
    }
}