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
    public class RectangleDrawingShape : BaseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingRectangle;

        public int CornerRadius { get; set; }

        public override void OnConfigLoad()
        {
            base.OnConfigLoad();
            CornerRadius = AnnotationOptions.DrawingCornerRadius;
        }

        public override void OnConfigSave()
        {
            base.OnConfigSave();
            AnnotationOptions.DrawingCornerRadius = CornerRadius;
        }

        public override void OnDraw(Graphics g)
        {
            DrawRectangle(g);
        }

        protected void DrawRectangle(Graphics g)
        {
            if (Shadow)
            {
                if (IsBorderVisible)
                {
                    DrawRectangle(g, ShadowColor, BorderSize, BorderStyle, Color.Transparent, Rectangle.LocationOffset(ShadowOffset), CornerRadius);
                }
                else if (FillColor.A == 255)
                {
                    DrawRectangle(g, Color.Transparent, 0, BorderStyle, ShadowColor, Rectangle.LocationOffset(ShadowOffset), CornerRadius);
                }
            }

            DrawRectangle(g, BorderColor, BorderSize, BorderStyle, FillColor, Rectangle, CornerRadius);
        }

        protected void DrawRectangle(Graphics g, Color borderColor, int borderSize, BorderStyle borderStyle, Color fillColor, RectangleF rect, int cornerRadius)
        {
            Brush brush = null;
            Pen pen = null;

            try
            {
                if (fillColor.A > 0)
                {
                    brush = new SolidBrush(fillColor);
                }

                if (borderSize > 0 && borderColor.A > 0)
                {
                    pen = new Pen(borderColor, borderSize);
                    pen.DashStyle = (DashStyle)borderStyle;
                }

                if (cornerRadius > 0)
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;

                    if (borderSize.IsEvenNumber())
                    {
                        g.PixelOffsetMode = PixelOffsetMode.Half;
                    }
                }

                g.DrawRoundedRectangle(brush, pen, rect, cornerRadius);

                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.Default;
            }
            finally
            {
                if (brush != null) brush.Dispose();
                if (pen != null) pen.Dispose();
            }
        }

        public override void OnShapePathRequested(GraphicsPath gp, RectangleF rect)
        {
            gp.AddRoundedRectangle(rect, CornerRadius);
        }
    }
}