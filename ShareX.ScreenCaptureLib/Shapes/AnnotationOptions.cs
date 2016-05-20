#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ShareX.ScreenCaptureLib
{
    public class AnnotationOptions
    {
        public Color BorderColor { get; set; } = Color.Red;
        public int BorderSize { get; set; } = 2;
        public Color FillColor { get; set; } = Color.FromArgb(0, 0, 0, 0);
        public int RoundedRectangleRadius { get; set; } = 15;
        public TextDrawingOptions TextOptions { get; set; } = new TextDrawingOptions();
        public int BlurRadius { get; set; } = 15;
        public int PixelateSize { get; set; } = 7;
        public Color HighlightColor { get; set; } = Color.Yellow;
    }
}