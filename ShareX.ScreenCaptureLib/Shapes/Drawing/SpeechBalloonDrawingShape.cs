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
    public class SpeechBalloonDrawingShape : TextDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingSpeechBalloon;

        public int TailWidth { get; } = 40;

        internal ResizeNode TailNode => Manager.ResizeNodes[(int)NodePosition.Extra];

        public override void OnCreated()
        {
            base.OnCreated();

            TailNode.Position = Rectangle.Location.Add(-30, -30);
        }

        public override void OnNodeVisible()
        {
            base.OnNodeVisible();

            TailNode.Shape = NodeShape.Circle;
            TailNode.Visible = true;
        }

        public override void OnDraw(Graphics g)
        {
            DrawTail(g);

            base.OnDraw(g);
        }

        private void DrawTail(Graphics g)
        {
            g.ExcludeClip(Rectangle);

            using (GraphicsPath gpTail = new GraphicsPath())
            {
                Point center = Rectangle.Center();
                int tailOrigin = TailWidth / 2;
                int tailLength = (int)MathHelpers.Distance(center, TailNode.Position);
                gpTail.AddLine(0, -tailOrigin, 0, tailOrigin);
                gpTail.AddLine(0, tailOrigin, tailLength, 0);
                gpTail.CloseFigure();

                g.TranslateTransform(center.X, center.Y);
                float tailDegree = MathHelpers.LookAtDegree(center, TailNode.Position);
                g.RotateTransform(tailDegree);

                g.SmoothingMode = SmoothingMode.HighQuality;

                if (FillColor.A > 0)
                {
                    using (Brush brush = new SolidBrush(FillColor))
                    {
                        g.FillPath(brush, gpTail);
                    }
                }

                if (BorderSize > 0 && BorderColor.A > 0)
                {
                    using (Pen pen = new Pen(BorderColor, BorderSize))
                    {
                        g.DrawPath(pen, gpTail);
                    }
                }

                g.SmoothingMode = SmoothingMode.None;

                g.ResetTransform();
            }

            g.ResetClip();
        }

        public override void OnNodeUpdate()
        {
            base.OnNodeUpdate();

            if (TailNode.IsDragging)
            {
                TailNode.Position = InputManager.MousePosition0Based;
            }
        }
    }
}