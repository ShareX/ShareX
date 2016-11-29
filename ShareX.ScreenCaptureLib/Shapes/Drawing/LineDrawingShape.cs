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

        public bool CenterNodeActive { get; set; }
        public Point CenterPosition { get; private set; }

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

            if (CenterNodeActive)
            {
                Rectangle = new Point[] { StartPosition, CenterPosition, EndPosition }.CreateRectangle();
            }
        }

        public override void OnDraw(Graphics g)
        {
            if (Shadow)
            {
                DrawLine(g, ShadowColor, BorderSize, StartPosition.Add(ShadowDirection), EndPosition.Add(ShadowDirection), CenterPosition.Add(ShadowDirection));
            }

            DrawLine(g, BorderColor, BorderSize, StartPosition, EndPosition, CenterPosition);
        }

        protected virtual void DrawLine(Graphics g, Color borderColor, int borderSize, Point startPosition, Point endPosition, Point centerPosition)
        {
            if (borderSize > 0 && borderColor.A > 0)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;

                if (borderSize.IsEvenNumber())
                {
                    g.PixelOffsetMode = PixelOffsetMode.Half;
                }

                using (Pen pen = CreatePen(borderColor, borderSize))
                {
                    if (CenterNodeActive)
                    {
                        g.DrawCurve(pen, new Point[] { startPosition, centerPosition, endPosition });
                    }
                    else
                    {
                        g.DrawLine(pen, startPosition, endPosition);
                    }
                }

                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.Default;
            }
        }

        protected virtual Pen CreatePen(Color borderColor, int borderSize)
        {
            return new Pen(borderColor, borderSize)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };
        }

        public override void Move(int x, int y)
        {
            StartPosition = StartPosition.Add(x, y);
            EndPosition = EndPosition.Add(x, y);
            CenterPosition = CenterPosition.Add(x, y);
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
                CenterNodeActive = true;

                Manager.IsResizing = true;

                CenterPosition = InputManager.MousePosition0Based;
            }
        }

        public override void OnNodePositionUpdate()
        {
            Manager.ResizeNodes[(int)NodePosition.TopLeft].Position = StartPosition;
            Manager.ResizeNodes[(int)NodePosition.BottomRight].Position = EndPosition;

            if (!CenterNodeActive)
            {
                CenterPosition = new Point((int)MathHelpers.Lerp(StartPosition.X, EndPosition.X, 0.5f), (int)MathHelpers.Lerp(StartPosition.Y, EndPosition.Y, 0.5f));
            }

            Manager.ResizeNodes[(int)NodePosition.Extra].Position = CenterPosition;
        }
    }
}