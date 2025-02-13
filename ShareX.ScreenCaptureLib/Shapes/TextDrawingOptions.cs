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
using System.Drawing.Drawing2D;

namespace ShareX.ScreenCaptureLib
{
    public class TextDrawingOptions
    {
        public string Font { get; set; } = AnnotationOptions.DefaultFont;
        public int Size { get; set; } = 18;
        public Color Color { get; set; } = Color.White;
        public bool Bold { get; set; } = false;
        public bool Italic { get; set; } = false;
        public bool Underline { get; set; } = false;
        public StringAlignment AlignmentHorizontal { get; set; } = StringAlignment.Center;
        public StringAlignment AlignmentVertical { get; set; } = StringAlignment.Center;

        public FontStyle Style
        {
            get
            {
                FontStyle style = FontStyle.Regular;

                if (Bold)
                {
                    style |= FontStyle.Bold;
                }

                if (Italic)
                {
                    style |= FontStyle.Italic;
                }

                if (Underline)
                {
                    style |= FontStyle.Underline;
                }

                return style;
            }
        }

        public bool Gradient { get; set; } = false;
        public Color Color2 { get; set; } = Color.FromArgb(240, 240, 240);
        public LinearGradientMode GradientMode { get; set; } = LinearGradientMode.Vertical;
        public bool EnterKeyNewLine { get; set; } = false;
    }
}