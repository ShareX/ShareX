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

using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    public class AnnotationOptions
    {
        // Drawing
        public Color BorderColor { get; set; } = Color.Red;
        public int BorderSize { get; set; } = 2;
        public Color FillColor { get; set; } = Color.FromArgb(0, 0, 0, 0);

        // Rounded rectangle region, rounded rectangle drawing
        public int RoundedRectangleRadius { get; set; } = 15;

        // Text drawing
        public TextDrawingOptions TextOptions { get; set; } = new TextDrawingOptions();
        public Color TextBorderColor { get; set; } = Color.White;
        public int TextBorderSize { get; set; } = 0;
        public Color TextFillColor { get; set; } = Color.FromArgb(150, Color.Black);

        // Step drawing
        public Color StepBorderColor { get; set; } = Color.White;
        public int StepBorderSize { get; set; } = 2;
        public Color StepFillColor { get; set; } = Color.Red;

        // Blur effect
        public int BlurRadius { get; set; } = 15;

        // Pixelate effect
        public int PixelateSize { get; set; } = 7;

        // Highlight effect
        public Color HighlightColor { get; set; } = Color.Yellow;
    }
}