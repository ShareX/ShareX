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

using System;
using System.Drawing;

namespace HelpersLib
{
    [Serializable]
    public class XmlFont
    {
        public string FontFamily { get; set; }
        public float Size { get; set; }
        public FontStyle Style { get; set; }
        public GraphicsUnit GraphicsUnit { get; set; }

        public XmlFont()
        {
        }

        public XmlFont(Font font)
        {
            Init(font);
        }

        public XmlFont(string fontName, float fontSize, FontStyle fontStyle = FontStyle.Regular)
        {
            Font font = CreateFont(fontName, fontSize, fontStyle);
            Init(font);
        }

        private void Init(Font font)
        {
            FontFamily = font.FontFamily.Name;
            GraphicsUnit = font.Unit;
            Size = font.Size;
            Style = font.Style;
        }

        private Font CreateFont(string fontName, float fontSize, FontStyle fontStyle)
        {
            try
            {
                return new Font(fontName, fontSize, fontStyle);
            }
            catch
            {
                return new Font(SystemFonts.DefaultFont.FontFamily, fontSize, fontStyle);
            }
        }

        public static implicit operator Font(XmlFont font)
        {
            return new Font(font.FontFamily, font.Size, font.Style, font.GraphicsUnit);
        }

        public static implicit operator XmlFont(Font font)
        {
            return new XmlFont(font);
        }
    }
}