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

using System;
using System.Drawing;

namespace ShareX.HelpersLib
{
    [Serializable]
    public class XmlColor
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public XmlColor() : this(0, 0, 0)
        {
        }

        public XmlColor(byte r, byte g, byte b) : this(255, r, g, b)
        {
        }

        public XmlColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public XmlColor(Color color) : this(color.A, color.R, color.G, color.B)
        {
        }

        public Color ToColor()
        {
            return Color.FromArgb(A, R, G, B);
        }

        public int ToArgb()
        {
            return (A << 24) | (R << 16) | (G << 8) | B;
        }

        public override string ToString()
        {
            return string.Format("A:{0}, R:{1}, G:{2}, B:{3}", A, R, G, B);
        }

        public static implicit operator Color(XmlColor color)
        {
            return color.ToColor();
        }

        public static implicit operator XmlColor(Color color)
        {
            return new XmlColor(color);
        }
    }
}