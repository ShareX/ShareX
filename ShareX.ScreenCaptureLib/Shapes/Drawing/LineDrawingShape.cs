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
    public class LineDrawingShape : BaseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingLine;

        public Point MiddlePosition { get; private set; } = Point.Empty;

        public override bool IsValidShape
        {
            get
            {
                return MathHelpers.Distance(StartPosition, EndPosition) > MinimumSize;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!MiddlePosition.IsEmpty)
            {
                Rectangle = new Point[] { StartPosition, MiddlePosition, EndPosition }.CreateRectangle();
            }
        }

        public override void OnDraw(Graphics g)
        {
            if (BorderSize > 0 && BorderColor.A > 0)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;

                if (BorderSize.IsEvenNumber())
                {
                    g.PixelOffsetMode = PixelOffsetMode.Half;
                }

                using (Pen pen = new Pen(BorderColor, BorderSize))
                {
                    DrawLine(g, pen);
                }

                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.Default;
            }
        }

        protected virtual void DrawLine(Graphics g, Pen pen)
        {
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;

            if (MiddlePosition.IsEmpty)
            {
                g.DrawLine(pen, StartPosition, EndPosition);
            }
            else
            {
                g.DrawCurve(pen, new Point[] { StartPosition, MiddlePosition, EndPosition });
            }
        }

        public override void Move(int x, int y)
        {
            StartPosition = StartPosition.Add(x, y);
            EndPosition = EndPosition.Add(x, y);
            MiddlePosition = MiddlePosition.Add(x, y);
        }

        public override void Resize(int x, int y, bool fromBottomRight)
        {
            if (fromBottomRight)
            {
                EndPosition = EndPosition.Add(x, y);
            }
            else
            {
                StartPosition = StartPosition.Add(x, y);
            }
        }

        public override void OnNodeVisible()
        {
            Manager.ResizeNodes[(int)NodePosition.TopLeft].Shape = Manager.ResizeNodes[(int)NodePosition.BottomRight].Shape = Manager.ResizeNodes[(int)NodePosition.Extra].Shape = NodeShape.Circle;
            Manager.ResizeNodes[(int)NodePosition.TopLeft].Visible = Manager.ResizeNodes[(int)NodePosition.BottomRight].Visible = Manager.ResizeNodes[(int)NodePosition.Extra].Visible = true;
        }

        public override void OnCreated()
        {
            MiddlePosition = new Point((int)MathHelpers.Lerp(StartPosition.X, EndPosition.X, 0.5f), (int)MathHelpers.Lerp(StartPosition.Y, EndPosition.Y, 0.5f));
        }

        public override void OnNodeUpdate()
        {
            if (Manager.ResizeNodes[(int)NodePosition.TopLeft].IsDragging)
            {
                Manager.IsResizing = true;

                StartPosition = InputManager.MousePosition0Based;
            }
            else if (Manager.ResizeNodes[(int)NodePosition.BottomRight].IsDragging)
            {
                Manager.IsResizing = true;

                EndPosition = InputManager.MousePosition0Based;
            }
            else if (Manager.ResizeNodes[(int)NodePosition.Extra].IsDragging)
            {
                Manager.IsResizing = true;

                MiddlePosition = InputManager.MousePosition0Based;
            }
        }

        public override void OnNodePositionUpdate()
        {
            Manager.ResizeNodes[(int)NodePosition.TopLeft].Position = StartPosition;
            Manager.ResizeNodes[(int)NodePosition.BottomRight].Position = EndPosition;
            Manager.ResizeNodes[(int)NodePosition.Extra].Position = MiddlePosition;
        }
    }
}