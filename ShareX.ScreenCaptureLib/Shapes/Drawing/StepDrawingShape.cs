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
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class StepDrawingShape : BaseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingStep;

        public int Number { get; set; }

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
                using (Pen pen = new Pen(BorderColor, BorderSize))
                {
                    g.DrawEllipse(pen, Rectangle);
                }
            }

            if (Rectangle.Width > 20 && Rectangle.Height > 20)
            {
                int fontSize = Math.Min(Rectangle.Width, Rectangle.Height) - 10;

                using (Font font = new Font("Verdana", fontSize, GraphicsUnit.Pixel))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    g.DrawString(Number.ToString(), font, Brushes.Black, Rectangle.LocationOffset(1, 1), sf);
                    g.DrawString(Number.ToString(), font, Brushes.White, Rectangle, sf);
                    g.TextRenderingHint = TextRenderingHint.SystemDefault;
                }
            }

            g.SmoothingMode = SmoothingMode.None;
        }

        public override void AddShapePath(GraphicsPath gp, Rectangle rect)
        {
            gp.AddEllipse(rect);
        }
    }
}