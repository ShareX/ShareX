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
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ImageEffectsLib
{
    internal class Crop : ImageEffect
    {
        private Padding margin;

        [DefaultValue(typeof(Padding), "0, 0, 0, 0")]
        public Padding Margin
        {
            get
            {
                return margin;
            }
            set
            {
                if (value.Top >= 0 && value.Right >= 0 && value.Bottom >= 0 && value.Left >= 0)
                {
                    margin = value;
                }
            }
        }

        public Crop()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Image Apply(Image img)
        {
            if (Margin.All == 0) return img;

            return ImageHelpers.CropImage(img, new Rectangle(Margin.Left, Margin.Top, img.Width - Margin.Horizontal, img.Height - Margin.Vertical));
        }
    }
}