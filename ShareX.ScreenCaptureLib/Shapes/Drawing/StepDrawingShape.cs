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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace ShareX.ScreenCaptureLib
{
    public class StepDrawingShape : BaseDrawingShape
    {
        private const int DefaultSize = 30;

        public override ShapeType ShapeType { get; } = ShapeType.DrawingStep;
        public override NodeType NodeType { get; } = NodeType.Point;

        public int Number { get; set; }

        public StepDrawingShape()
        {
            Rectangle = new Rectangle(0, 0, DefaultSize, DefaultSize);
        }

        public override void OnConfigLoad()
        {
            BorderColor = AnnotationOptions.StepBorderColor;
            BorderSize = AnnotationOptions.StepBorderSize;
            FillColor = AnnotationOptions.StepFillColor;
        }

        public override void OnConfigSave()
        {
            AnnotationOptions.StepBorderColor = BorderColor;
            AnnotationOptions.StepBorderSize = BorderSize;
            AnnotationOptions.StepFillColor = FillColor;
        }

        public override void OnDraw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;

            if (FillColor.A > 0)
            {
                using (Brush brush = new SolidBrush(FillColor))
                {
                    g.FillEllipse(brush, Rectangle);
                }
            }

            if (BorderSize > 0 && BorderColor.A > 0)
            {
                //g.DrawEllipse(Pens.Black, Rectangle.LocationOffset(0, 1));

                using (Pen pen = new Pen(BorderColor, BorderSize))
                {
                    g.DrawEllipse(pen, Rectangle);
                }
            }

            g.SmoothingMode = SmoothingMode.None;

            if (Rectangle.Width > 20 && Rectangle.Height > 20)
            {
                int offset;

                if (Number > 99)
                {
                    offset = 20;
                }
                else if (Number > 9)
                {
                    offset = 15;
                }
                else
                {
                    offset = 10;
                }

                int fontSize = Math.Min(Rectangle.Width, Rectangle.Height) - offset;

                using (Font font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (Brush textBrush = new SolidBrush(BorderColor))
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    //g.DrawString(Number.ToString(), font, Brushes.Black, Rectangle.LocationOffset(1, 1), sf);
                    g.DrawString(Number.ToString(), font, textBrush, Rectangle, sf);
                    g.TextRenderingHint = TextRenderingHint.SystemDefault;
                }
            }
        }

        public override void OnShapePathRequested(GraphicsPath gp, Rectangle rect)
        {
            gp.AddEllipse(rect);
        }
    }
}