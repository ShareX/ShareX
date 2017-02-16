#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
        public static readonly Color PrimaryColor = Color.Red;
        public static readonly Color SecondaryColor = Color.White;
        public static readonly Color TransparentColor = Color.FromArgb(0, 0, 0, 0);

        // Region
        public int RegionCornerRadius { get; set; } = 0;

        // Drawing
        public Color BorderColor { get; set; } = PrimaryColor;
        public int BorderSize { get; set; } = 5;
        public Color FillColor { get; set; } = TransparentColor;
        public int DrawingCornerRadius { get; set; } = 3;
        public bool Shadow { get; set; } = true;

        // Text (Outline) drawing
        public TextDrawingOptions TextOutlineOptions { get; set; } = new TextDrawingOptions()
        {
            Color = PrimaryColor,
            Size = 40,
            Bold = true
        };
        public Color TextOutlineBorderColor { get; set; } = SecondaryColor;
        public int TextOutlineBorderSize { get; set; } = 3;

        // Text (Background) drawing
        public TextDrawingOptions TextOptions { get; set; } = new TextDrawingOptions()
        {
            Color = SecondaryColor,
            Size = 18
        };
        public Color TextBorderColor { get; set; } = SecondaryColor;
        public int TextBorderSize { get; set; } = 2;
        public Color TextFillColor { get; set; } = PrimaryColor;

        // Step drawing
        public Color StepBorderColor { get; set; } = SecondaryColor;
        public int StepBorderSize { get; set; } = 2;
        public Color StepFillColor { get; set; } = PrimaryColor;

        // Blur effect
        public int BlurRadius { get; set; } = 15;

        // Pixelate effect
        public int PixelateSize { get; set; } = 8;

        // Highlight effect
        public Color HighlightColor { get; set; } = Color.Yellow;
    }
}