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
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ColorBgra
    {
        [FieldOffset(0)]
        public uint Bgra;

        [FieldOffset(0)]
        public byte Blue;
        [FieldOffset(1)]
        public byte Green;
        [FieldOffset(2)]
        public byte Red;
        [FieldOffset(3)]
        public byte Alpha;

        public const byte SizeOf = 4;

        public ColorBgra(uint bgra) : this()
        {
            Bgra = bgra;
        }

        public ColorBgra(byte b, byte g, byte r, byte a = 255) : this()
        {
            Blue = b;
            Green = g;
            Red = r;
            Alpha = a;
        }

        public ColorBgra(Color color) : this(color.B, color.G, color.R, color.A)
        {
        }

        public static bool operator ==(ColorBgra c1, ColorBgra c2)
        {
            return c1.Bgra == c2.Bgra;
        }

        public static bool operator !=(ColorBgra c1, ColorBgra c2)
        {
            return c1.Bgra != c2.Bgra;
        }

        public override bool Equals(object obj)
        {
            return obj is ColorBgra color && color.Bgra == Bgra;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)Bgra;
            }
        }

        public static implicit operator ColorBgra(uint color)
        {
            return new ColorBgra(color);
        }

        public static implicit operator uint(ColorBgra color)
        {
            return color.Bgra;
        }

        public Color ToColor()
        {
            return Color.FromArgb(Alpha, Red, Green, Blue);
        }

        public override string ToString()
        {
            return string.Format("B: {0}, G: {1}, R: {2}, A: {3}", Blue, Green, Red, Alpha);
        }

        public static uint BgraToUInt32(uint b, uint g, uint r, uint a)
        {
            return b + (g << 8) + (r << 16) + (a << 24);
        }

        public static uint BgraToUInt32(byte b, byte g, byte r, byte a)
        {
            return b + ((uint)g << 8) + ((uint)r << 16) + ((uint)a << 24);
        }
    }
}