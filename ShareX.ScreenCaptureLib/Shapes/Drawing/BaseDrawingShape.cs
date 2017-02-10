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
    public abstract class BaseDrawingShape : BaseShape
    {
        public override ShapeCategory ShapeCategory { get; } = ShapeCategory.Drawing;

        public Color BorderColor { get; set; }
        public int BorderSize { get; set; }
        public Color FillColor { get; set; }

        public bool Shadow { get; set; }
        public Color ShadowColor { get; set; } = Color.FromArgb(125, 0, 0, 0);
        public Point ShadowOffset { get; set; } = new Point(0, 1);

        public bool IsShapeVisible => IsBorderVisible || IsFillVisible;
        public bool IsBorderVisible => BorderSize > 0 && BorderColor.A > 0;
        public bool IsFillVisible => FillColor.A > 0;

        public override void OnConfigLoad()
        {
            BorderColor = AnnotationOptions.BorderColor;
            BorderSize = AnnotationOptions.BorderSize;
            FillColor = AnnotationOptions.FillColor;
            Shadow = AnnotationOptions.Shadow;
        }

        public override void OnConfigSave()
        {
            AnnotationOptions.BorderColor = BorderColor;
            AnnotationOptions.BorderSize = BorderSize;
            AnnotationOptions.FillColor = FillColor;
            AnnotationOptions.Shadow = Shadow;
        }

        public abstract void OnDraw(Graphics g);
    }
}