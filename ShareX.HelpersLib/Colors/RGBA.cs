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

using System.Drawing;

namespace ShareX.HelpersLib
{
    public struct RGBA
    {
        private int red, green, blue, alpha;

        public int Red
        {
            get
            {
                return red;
            }
            set
            {
                red = ColorHelpers.ValidColor(value);
            }
        }

        public int Green
        {
            get
            {
                return green;
            }
            set
            {
                green = ColorHelpers.ValidColor(value);
            }
        }

        public int Blue
        {
            get
            {
                return blue;
            }
            set
            {
                blue = ColorHelpers.ValidColor(value);
            }
        }

        public int Alpha
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = ColorHelpers.ValidColor(value);
            }
        }

        public RGBA(int red, int green, int blue, int alpha = 255) : this()
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public RGBA(Color color) : this(color.R, color.G, color.B, color.A)
        {
        }

        public static implicit operator RGBA(Color color)
        {
            return new RGBA(color);
        }

        public static implicit operator Color(RGBA color)
        {
            return color.ToColor();
        }

        public static implicit operator HSB(RGBA color)
        {
            return color.ToHSB();
        }

        public static implicit operator CMYK(RGBA color)
        {
            return color.ToCMYK();
        }

        public static bool operator ==(RGBA left, RGBA right)
        {
            return (left.Red == right.Red) && (left.Green == right.Green) && (left.Blue == right.Blue) && (left.Alpha == right.Alpha);
        }

        public static bool operator !=(RGBA left, RGBA right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"R: {Red}, G: {Green}, B: {Blue}, A: {Alpha}";
        }

        public Color ToColor()
        {
            return Color.FromArgb(Alpha, Red, Green, Blue);
        }

        public string ToHex(ColorFormat format = ColorFormat.RGB)
        {
            return ColorHelpers.ColorToHex(this, format);
        }

        public int ToDecimal(ColorFormat format = ColorFormat.RGB)
        {
            return ColorHelpers.ColorToDecimal(this, format);
        }

        public HSB ToHSB()
        {
            return ColorHelpers.ColorToHSB(this);
        }

        public CMYK ToCMYK()
        {
            return ColorHelpers.ColorToCMYK(this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}