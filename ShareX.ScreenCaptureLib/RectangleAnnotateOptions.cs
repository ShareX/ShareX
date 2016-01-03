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

using ShareX.HelpersLib;
using System.ComponentModel;
using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    public class RectangleAnnotateOptions
    {
        [DefaultValue(true), Description("Show cursor position and region size.")]
        public bool ShowInfo { get; set; }

        [DefaultValue(true), Description("Show hotkey tips.")]
        public bool ShowTips { get; set; }

        [DefaultValue(typeof(Color), "255, 0, 0"), Description("Color of pen and rectangle border.")]
        public Color DrawingPenColor { get; set; }

        private int drawingPenSize;

        [DefaultValue(5), Description("Size of pen.")]
        public int DrawingPenSize
        {
            get
            {
                return drawingPenSize;
            }
            set
            {
                drawingPenSize = value.Between(1, 100);
            }
        }

        private int drawingRectangleBorderSize;

        [DefaultValue(2), Description("Size of rectangle border.")]
        public int DrawingRectangleBorderSize
        {
            get
            {
                return drawingRectangleBorderSize;
            }
            set
            {
                drawingRectangleBorderSize = value.Between(1, 100);
            }
        }

        [DefaultValue(true), Description("Draw shadow around rectangle.")]
        public bool DrawingRectangleShadow { get; set; }

        public RectangleAnnotateOptions()
        {
            this.ApplyDefaultPropertyValues();
        }
    }
}