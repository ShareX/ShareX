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

using ShareX.HelpersLib.Properties;
using System.Drawing;

namespace ShareX.HelpersLib
{
    public struct CMYK
    {
        private double cyan;
        private double magenta;
        private double yellow;
        private double key;
        private int alpha;

        public double Cyan
        {
            get
            {
                return cyan;
            }
            set
            {
                cyan = ColorHelpers.ValidColor(value);
            }
        }

        public double Cyan100
        {
            get
            {
                return cyan * 100;
            }
            set
            {
                cyan = ColorHelpers.ValidColor(value / 100);
            }
        }

        public double Magenta
        {
            get
            {
                return magenta;
            }
            set
            {
                magenta = ColorHelpers.ValidColor(value);
            }
        }

        public double Magenta100
        {
            get
            {
                return magenta * 100;
            }
            set
            {
                magenta = ColorHelpers.ValidColor(value / 100);
            }
        }

        public double Yellow
        {
            get
            {
                return yellow;
            }
            set
            {
                yellow = ColorHelpers.ValidColor(value);
            }
        }

        public double Yellow100
        {
            get
            {
                return yellow * 100;
            }
            set
            {
                yellow = ColorHelpers.ValidColor(value / 100);
            }
        }

        public double Key
        {
            get
            {
                return key;
            }
            set
            {
                key = ColorHelpers.ValidColor(value);
            }
        }

        public double Key100
        {
            get
            {
                return key * 100;
            }
            set
            {
                key = ColorHelpers.ValidColor(value / 100);
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

        public CMYK(double cyan, double magenta, double yellow, double key, int alpha = 255) : this()
        {
            Cyan = cyan;
            Magenta = magenta;
            Yellow = yellow;
            Key = key;
            Alpha = alpha;
        }

        public CMYK(int cyan, int magenta, int yellow, int key, int alpha = 255) : this()
        {
            Cyan100 = cyan;
            Magenta100 = magenta;
            Yellow100 = yellow;
            Key100 = key;
            Alpha = alpha;
        }

        public CMYK(Color color)
        {
            this = ColorHelpers.ColorToCMYK(color);
        }

        public static implicit operator CMYK(Color color)
        {
            return ColorHelpers.ColorToCMYK(color);
        }

        public static implicit operator Color(CMYK color)
        {
            return color.ToColor();
        }

        public static implicit operator RGBA(CMYK color)
        {
            return color.ToColor();
        }

        public static implicit operator HSB(CMYK color)
        {
            return color.ToColor();
        }

        public static bool operator ==(CMYK left, CMYK right)
        {
            return (left.Cyan == right.Cyan) && (left.Magenta == right.Magenta) && (left.Yellow == right.Yellow) && (left.Key == right.Key);
        }

        public static bool operator !=(CMYK left, CMYK right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return string.Format(Resources.CMYK_ToString_Cyan___0_0_0____Magenta___1_0_0____Yellow___2_0_0____Key___3_0_0__, Cyan100, Magenta100, Yellow100, Key100);
        }

        public Color ToColor()
        {
            return ColorHelpers.CMYKToColor(this);
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