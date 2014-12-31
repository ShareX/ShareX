#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
using System.Drawing.Drawing2D;

namespace ShareX.ImageEffectsLib
{
    [Description("Border")]
    public class DrawBorder : ImageEffect
    {
        [DefaultValue(BorderType.Outside)]
        public BorderType Type { get; set; }

        private int size;

        [DefaultValue(1)]
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value.Min(1);
            }
        }

        [DefaultValue(typeof(Color), "Black"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color Color { get; set; }

        [DefaultValue(false)]
        public bool UseGradient { get; set; }

        [DefaultValue(typeof(Color), "White"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color Color2 { get; set; }

        [DefaultValue(LinearGradientMode.Vertical)]
        public LinearGradientMode GradientType { get; set; }

        public DrawBorder()
        {
            this.ApplyDefaultPropertyValues();
        }

        public override Image Apply(Image img)
        {
            if (UseGradient)
            {
                return ImageHelpers.DrawBorder(img, Color, Color2, GradientType, Size, Type);
            }

            return ImageHelpers.DrawBorder(img, Color, Size, Type);
        }
    }
}