#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

namespace ShareX.HelpersLib;

[Serializable]
public class XmlFont
{
    public string FontFamily { get; set; }
    public float Size { get; set; }
    public ImageFontStyle Style { get; set; }
    public ImageFontUnit GraphicsUnit { get; set; }
    public XmlFont() { }
    public XmlFont(ImageFont font)
    {
        FontFamily = font.Name; Size = font.Size; Style = font.Style; GraphicsUnit = font.Unit;
    }
    public XmlFont(string name, float size, ImageFontStyle style = ImageFontStyle.Regular)
        : this(new ImageFont(name, size, style)) { }
    public static implicit operator ImageFont(XmlFont font) => font.ToFont();
    public static implicit operator XmlFont(ImageFont font) => new(font);
    public ImageFont ToFont() => new(FontFamily, Size, Style, GraphicsUnit);
    public override string ToString() => $"{FontFamily}; {Size}" + (Style == ImageFontStyle.Regular ? "" : $"; {Style}");
}
