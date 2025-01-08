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
                size = value.Max(1);
            }
        }

        [DefaultValue(DashStyle.Solid), TypeConverter(typeof(EnumProperNameConverter))]
        public DashStyle DashStyle { get; set; }

        [DefaultValue(typeof(Color), "Black"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), TypeConverter(typeof(MyColorConverter))]
        public Color Color { get; set; }

        [DefaultValue(false)]
        public bool UseGradient { get; set; }

        [Editor(typeof(GradientEditor), typeof(UITypeEditor))]
        public GradientInfo Gradient { get; set; }

        public DrawBorder()
        {
            this.ApplyDefaultPropertyValues();
            AddDefaultGradient();
        }

        private void AddDefaultGradient()
        {
            Gradient = new GradientInfo();
            Gradient.Colors.Add(new GradientStop(Color.FromArgb(68, 120, 194), 0f));
            Gradient.Colors.Add(new GradientStop(Color.FromArgb(13, 58, 122), 50f));
            Gradient.Colors.Add(new GradientStop(Color.FromArgb(6, 36, 78), 50f));
            Gradient.Colors.Add(new GradientStop(Color.FromArgb(23, 89, 174), 100f));
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            if (UseGradient && Gradient != null && Gradient.IsValid)
            {
                return ImageHelpers.DrawBorder(bmp, Gradient, Size, Type, DashStyle);
            }

            return ImageHelpers.DrawBorder(bmp, Color, Size, Type, DashStyle);
        }

        protected override string GetSummary()
        {
            return Size + "px";
        }
    }
}