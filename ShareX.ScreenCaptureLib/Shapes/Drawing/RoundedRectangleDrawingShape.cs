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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.ScreenCaptureLib
{
    public class RoundedRectangleDrawingShape : BaseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingRoundedRectangle;

        public float Radius { get; set; }

        public override void OnConfigLoad()
        {
            base.OnConfigLoad();
            Radius = AnnotationOptions.RoundedRectangleRadius;
        }

        public override void OnConfigSave()
        {
            base.OnConfigSave();
            AnnotationOptions.RoundedRectangleRadius = (int)Radius;
        }

        public override void OnDraw(Graphics g)
        {
            Brush brush = null;
            Pen pen = null;

            try
            {
                if (FillColor.A > 0)
                {
                    brush = new SolidBrush(FillColor);
                }

                if (BorderSize > 0 && BorderColor.A > 0)
                {
                    pen = new Pen(BorderColor, BorderSize);
                }

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawRoundedRectangle(brush, pen, Rectangle, Radius);
                g.SmoothingMode = SmoothingMode.None;
            }
            finally
            {
                if (brush != null) brush.Dispose();
                if (pen != null) pen.Dispose();
            }
        }

        public override void OnShapePathRequested(GraphicsPath gp, Rectangle rect)
        {
            gp.AddRoundedRectangle(rect, Radius);
        }
    }
}