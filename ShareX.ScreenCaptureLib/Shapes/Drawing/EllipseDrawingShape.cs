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

using ShareX.HelpersLib;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.ScreenCaptureLib
{
    public class EllipseDrawingShape : BaseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingEllipse;

        public override void OnDraw(Graphics g)
        {
            DrawEllipse(g);
        }

        protected void DrawEllipse(Graphics g)
        {
            if (Shadow)
            {
                if (IsBorderVisible)
                {
                    DrawEllipse(g, ShadowColor, BorderSize, BorderStyle, Color.Transparent, Rectangle.LocationOffset(ShadowOffset));
                }
                else if (FillColor.A == 255)
                {
                    DrawEllipse(g, Color.Transparent, 0, BorderStyle, ShadowColor, Rectangle.LocationOffset(ShadowOffset));
                }
            }

            DrawEllipse(g, BorderColor, BorderSize, BorderStyle, FillColor, Rectangle);
        }

        protected void DrawEllipse(Graphics g, Color borderColor, int borderSize, BorderStyle borderStyle, Color fillColor, RectangleF rect)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;

            if (fillColor.A > 0)
            {
                using (Brush brush = new SolidBrush(fillColor))
                {
                    g.FillEllipse(brush, rect);
                }
            }

            if (borderSize > 0 && borderColor.A > 0)
            {
                using (Pen pen = new Pen(borderColor, borderSize))
                {
                    pen.DashStyle = (DashStyle)borderStyle;

                    g.DrawEllipse(pen, rect);
                }
            }

            g.SmoothingMode = SmoothingMode.None;
        }

        public override void OnShapePathRequested(GraphicsPath gp, RectangleF rect)
        {
            gp.AddEllipse(rect);
        }
    }
}