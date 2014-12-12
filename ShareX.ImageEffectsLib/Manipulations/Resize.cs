#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
    public class Resize : ImageEffect
    {
        [DefaultValue(250), Description("Use width as 0 to automatically adjust width to maintain aspect ratio.")]
        public int Width { get; set; }

        [DefaultValue(0), Description("Use height as 0 to automatically adjust height to maintain aspect ratio.")]
        public int Height { get; set; }

        [DefaultValue(false), Description("Only resize image if it is bigger than specified size.")]
        public bool ResizeIfBigger { get; set; }

        public Resize()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Image Apply(Image img)
        {
            if (Width <= 0 && Height <= 0) return img;

            int width = Width <= 0 ? (int)((float)Height / img.Height * img.Width) : Width;
            int height = Height <= 0 ? (int)((float)Width / img.Width * img.Height) : Height;

            if (ResizeIfBigger && img.Width <= width && img.Height <= height)
            {
                return img;
            }

            return ImageHelpers.ResizeImage(img, width, height);
        }
    }
}