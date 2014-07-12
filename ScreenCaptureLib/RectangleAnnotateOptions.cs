#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    public class RectangleAnnotateOptions
    {
        [DefaultValue(false), Description("Show position and size of selected rectangle area.")]
        public bool ShowRectangleInfo { get; set; }

        [DefaultValue(true), Description("Show hotkey tips.")]
        public bool ShowTips { get; set; }

        [DefaultValue(typeof(Color), "0, 230, 0"), Description("In drawing mode color of pen.")]
        public Color DrawingPenColor { get; set; }

        private int drawingPenSize;

        [DefaultValue(7), Description("In drawing mode size of pen.")]
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

        public RectangleAnnotateOptions()
        {
            this.ApplyDefaultPropertyValues();
        }
    }
}