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
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public abstract class BaseShape
    {
        protected const int MinimumSize = 3;

        public abstract ShapeType ShapeType { get; }

        public virtual NodeType NodeType { get; } = NodeType.Rectangle;

        public Rectangle Rectangle { get; set; }

        protected AnnotationOptions AnnotationOptions
        {
            get
            {
                return Manager.Config.AnnotationOptions;
            }
        }

        private Point startPosition;

        public Point StartPosition
        {
            get
            {
                return startPosition;
            }
            set
            {
                startPosition = value;

                Rectangle = CaptureHelpers.CreateRectangle(StartPosition, EndPosition);
            }
        }

        private Point endPosition;

        public Point EndPosition
        {
            get
            {
                return endPosition;
            }
            set
            {
                endPosition = value;

                Rectangle = CaptureHelpers.CreateRectangle(StartPosition, EndPosition);
            }
        }

        public virtual bool IsValidShape
        {
            get
            {
                return !Rectangle.IsEmpty && Rectangle.Width >= MinimumSize && Rectangle.Height >= MinimumSize;
            }
        }

        internal ShapeManager Manager { get; set; }

        private Rectangle tempNodeRect;

        public virtual bool Intersects(Point position)
        {
            return Rectangle.Contains(position);
        }

        public void AddShapePath(GraphicsPath gp, int sizeOffset = 0)
        {
            Rectangle rect = Rectangle;

            if (sizeOffset != 0)
            {
                rect = rect.SizeOffset(sizeOffset);
            }

            OnShapePathRequested(gp, rect);
        }

        public virtual void Move(int x, int y)
        {
            Rectangle = Rectangle.LocationOffset(x, y);
        }

        public void Move(Point point)
        {
            Move(point.X, point.Y);
        }

        public virtual void Resize(int x, int y, bool fromBottomRight)
        {
            if (fromBottomRight)
            {
                Rectangle = Rectangle.SizeOffset(x, y);
            }
            else
            {
                Rectangle = Rectangle.LocationOffset(x, y).SizeOffset(-x, -y);
            }
        }

        public virtual void OnCreated()
        {
        }

        public virtual void OnUpdate()
        {
            if (Manager.IsCreating)
            {
                Point pos = InputManager.MousePosition0Based;

                if (Manager.IsCornerMoving)
                {
                    StartPosition = StartPosition.Add(InputManager.MouseVelocity);
                }
                else if (Manager.IsProportionalResizing)
                {
                    if (NodeType == NodeType.Rectangle)
                    {
                        pos = CaptureHelpers.SnapPositionToDegree(StartPosition, pos, 90, 45);
                    }
                    else if (NodeType == NodeType.Line)
                    {
                        pos = CaptureHelpers.SnapPositionToDegree(StartPosition, pos, 45, 0);
                    }
                }
                else if (Manager.IsSnapResizing)
                {
                    pos = Manager.SnapPosition(StartPosition, pos);
                }

                EndPosition = pos;
            }
            else if (Manager.IsMoving)
            {
                Move(InputManager.MouseVelocity);
            }
        }

        public virtual void OnShapePathRequested(GraphicsPath gp, Rectangle rect)
        {
            gp.AddRectangle(rect);
        }

        public virtual void OnConfigLoad()
        {
        }

        public virtual void OnConfigSave()
        {
        }

        public virtual void OnDoubleClicked()
        {
        }

        public virtual void OnNodeVisible()
        {
            foreach (NodeObject node in Manager.Nodes)
            {
                node.Shape = NodeShape.Square;
                node.Visible = true;
            }
        }

        public virtual void OnNodeUpdate()
        {
            for (int i = 0; i < 8; i++)
            {
                if (Manager.Nodes[i].IsDragging)
                {
                    Manager.IsResizing = true;

                    if (!InputManager.IsBeforeMouseDown(MouseButtons.Left))
                    {
                        tempNodeRect = Rectangle;
                    }

                    NodePosition nodePosition = (NodePosition)i;

                    int x = InputManager.MouseVelocity.X;

                    switch (nodePosition)
                    {
                        case NodePosition.TopLeft:
                        case NodePosition.Left:
                        case NodePosition.BottomLeft:
                            tempNodeRect.X += x;
                            tempNodeRect.Width -= x;
                            break;
                        case NodePosition.TopRight:
                        case NodePosition.Right:
                        case NodePosition.BottomRight:
                            tempNodeRect.Width += x;
                            break;
                    }

                    int y = InputManager.MouseVelocity.Y;

                    switch (nodePosition)
                    {
                        case NodePosition.TopLeft:
                        case NodePosition.Top:
                        case NodePosition.TopRight:
                            tempNodeRect.Y += y;
                            tempNodeRect.Height -= y;
                            break;
                        case NodePosition.BottomLeft:
                        case NodePosition.Bottom:
                        case NodePosition.BottomRight:
                            tempNodeRect.Height += y;
                            break;
                    }

                    Rectangle = CaptureHelpers.FixRectangle(tempNodeRect);
                }
            }
        }

        public virtual void OnNodePositionUpdate()
        {
            int xStart = Rectangle.X;
            int xMid = Rectangle.X + Rectangle.Width / 2;
            int xEnd = Rectangle.X + Rectangle.Width - 1;

            int yStart = Rectangle.Y;
            int yMid = Rectangle.Y + Rectangle.Height / 2;
            int yEnd = Rectangle.Y + Rectangle.Height - 1;

            Manager.Nodes[(int)NodePosition.TopLeft].Position = new Point(xStart, yStart);
            Manager.Nodes[(int)NodePosition.Top].Position = new Point(xMid, yStart);
            Manager.Nodes[(int)NodePosition.TopRight].Position = new Point(xEnd, yStart);
            Manager.Nodes[(int)NodePosition.Right].Position = new Point(xEnd, yMid);
            Manager.Nodes[(int)NodePosition.BottomRight].Position = new Point(xEnd, yEnd);
            Manager.Nodes[(int)NodePosition.Bottom].Position = new Point(xMid, yEnd);
            Manager.Nodes[(int)NodePosition.BottomLeft].Position = new Point(xStart, yEnd);
            Manager.Nodes[(int)NodePosition.Left].Position = new Point(xStart, yMid);
        }
    }
}