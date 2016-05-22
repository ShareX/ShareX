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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ShareX.ScreenCaptureLib
{
    public abstract class BaseDrawingShape : BaseShape
    {
        public Color BorderColor { get; set; }
        public int BorderSize { get; set; }
        public Color FillColor { get; set; }

        public override void UpdateShapeConfig()
        {
            BorderColor = AnnotationOptions.BorderColor;
            BorderSize = AnnotationOptions.BorderSize;
            FillColor = AnnotationOptions.FillColor;
        }

        public override void ApplyShapeConfig()
        {
            AnnotationOptions.BorderColor = BorderColor;
            AnnotationOptions.BorderSize = BorderSize;
            AnnotationOptions.FillColor = FillColor;
        }

        public abstract void OnDraw(Graphics g);
    }
}