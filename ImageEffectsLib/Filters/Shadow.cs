#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.Drawing.Design;

namespace ImageEffectsLib
{
    internal class Shadow : ImageEffect
    {
        private float opacity;

        [DefaultValue(0.6f), Description("Choose a value between 0.1 and 1.0")]
        public float Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                opacity = value.Between(0.1f, 1.0f);
            }
        }

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
                size = value.Min(0);
            }
        }

        [DefaultValue(0f)]
        public float Darkness { get; set; }

        [DefaultValue(typeof(Color), "Black"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color Color { get; set; }

        [DefaultValue(typeof(Point), "0, 0")]
        public Point Offset { get; set; }

        public Shadow()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Image Apply(Image img)
        {
            return ImageHelpers.AddShadow(img, Opacity, Size, Darkness + 1, Color, Offset);
        }
    }
}