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
    public struct MyColor
    {
        public RGBA RGBA;
        public HSB HSB;
        public CMYK CMYK;

        public bool IsTransparent
        {
            get
            {
                return RGBA.Alpha < 255;
            }
        }

        public MyColor(Color color)
        {
            RGBA = color;
            HSB = color;
            CMYK = color;
        }

        public static implicit operator MyColor(Color color)
        {
            return new MyColor(color);
        }

        public static implicit operator Color(MyColor color)
        {
            return color.RGBA;
        }

        public static bool operator ==(MyColor left, MyColor right)
        {
            return (left.RGBA == right.RGBA) && (left.HSB == right.HSB) && (left.CMYK == right.CMYK);
        }

        public static bool operator !=(MyColor left, MyColor right)
        {
            return !(left == right);
        }

        public void RGBAUpdate()
        {
            HSB = RGBA;
            CMYK = RGBA;
        }

        public void HSBUpdate()
        {
            RGBA = HSB;
            CMYK = HSB;
        }

        public void CMYKUpdate()
        {
            RGBA = CMYK;
            HSB = CMYK;
        }

        public override string ToString()
        {
            return string.Format(
@"RGBA (Red, Green, Blue, Alpha) = {0}, {1}, {2}, {3}
HSB (Hue, Saturation, Brightness) = {4:0.0}°, {5:0.0}%, {6:0.0}%
CMYK (Cyan, Magenta, Yellow, Key) = {7:0.0}%, {8:0.0}%, {9:0.0}%, {10:0.0}%
Hex (RGB, RGBA, ARGB) = #{11}, #{12}, #{13}
Decimal (RGB, RGBA, ARGB) = {14}, {15}, {16}",
                RGBA.Red, RGBA.Green, RGBA.Blue, RGBA.Alpha,
                HSB.Hue360, HSB.Saturation100, HSB.Brightness100,
                CMYK.Cyan100, CMYK.Magenta100, CMYK.Yellow100, CMYK.Key100,
                ColorHelpers.ColorToHex(this), ColorHelpers.ColorToHex(this, ColorFormat.RGBA), ColorHelpers.ColorToHex(this, ColorFormat.ARGB),
                ColorHelpers.ColorToDecimal(this), ColorHelpers.ColorToDecimal(this, ColorFormat.RGBA), ColorHelpers.ColorToDecimal(this, ColorFormat.ARGB));
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