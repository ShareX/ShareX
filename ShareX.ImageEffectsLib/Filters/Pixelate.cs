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
using System.Drawing.Design;

namespace ShareX.ImageEffectsLib
{
    internal class Pixelate : ImageEffect
    {
        private int size;

        [DefaultValue(10)]
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value.Max(2);
            }
        }

        private int borderSize;

        [DefaultValue(0)]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }
            set
            {
                borderSize = value.Max(0);
            }
        }

        [DefaultValue(typeof(Color), "Black"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color BorderColor { get; set; }

        public Pixelate()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            ImageHelpers.Pixelate(bmp, Size, BorderSize, BorderColor);
            return bmp;
        }

        protected override string GetSummary()
        {
            return Size.ToString();
        }
    }
}