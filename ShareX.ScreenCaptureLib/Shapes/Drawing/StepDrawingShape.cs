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
using System.Drawing;
using System.Drawing.Text;

namespace ShareX.ScreenCaptureLib
{
    public class StepDrawingShape : EllipseDrawingShape
    {
        private const int DefaultSize = 30;

        public override ShapeType ShapeType { get; } = ShapeType.DrawingStep;

        public int Number { get; set; }

        public StepDrawingShape()
        {
            Rectangle = new Rectangle(0, 0, DefaultSize, DefaultSize);
        }

        public override void ShowNodes()
        {
        }

        public override void OnCreating()
        {
            Manager.IsMoving = true;
            Point pos = InputManager.MousePosition0Based;
            Rectangle = new Rectangle(new Point(pos.X - Rectangle.Width / 2, pos.Y - Rectangle.Height / 2), Rectangle.Size);
        }

        public override void OnConfigLoad()
        {
            BorderColor = AnnotationOptions.StepBorderColor;
            BorderSize = AnnotationOptions.StepBorderSize;
            FillColor = AnnotationOptions.StepFillColor;
            Shadow = AnnotationOptions.Shadow;
        }

        public override void OnConfigSave()
        {
            AnnotationOptions.StepBorderColor = BorderColor;
            AnnotationOptions.StepBorderSize = BorderSize;
            AnnotationOptions.StepFillColor = FillColor;
            AnnotationOptions.Shadow = Shadow;
        }

        public override void OnDraw(Graphics g)
        {
            DrawEllipse(g);
            DrawNumber(g);
        }

        protected void DrawNumber(Graphics g)
        {
            if (Shadow)
            {
                DrawNumber(g, Number, ShadowColor, Rectangle.LocationOffset(ShadowOffset));
            }

            DrawNumber(g, Number, BorderColor, Rectangle);
        }

        protected void DrawNumber(Graphics g, int number, Color textColor, Rectangle rect)
        {
            if (rect.Width > 20 && rect.Height > 20)
            {
                int offset;

                if (number > 99)
                {
                    offset = 20;
                }
                else if (number > 9)
                {
                    offset = 15;
                }
                else
                {
                    offset = 10;
                }

                int fontSize = Math.Min(rect.Width, rect.Height) - offset;

                using (Font font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    g.DrawString(number.ToString(), font, textBrush, rect, sf);
                    g.TextRenderingHint = TextRenderingHint.SystemDefault;
                }
            }
        }

        public override void Resize(int x, int y, bool fromBottomRight)
        {
            Move(x, y);
        }
    }
}