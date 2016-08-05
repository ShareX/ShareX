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
    public abstract class BaseShape
    {
        protected const int MinimumSize = 3;

        public abstract ShapeType ShapeType { get; }

        public virtual NodeType NodeType { get; } = NodeType.Rectangle;

        public Rectangle Rectangle { get; set; }

        public ShapeManager Manager { get; set; }

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
            if (Manager.IsCreating && !Rectangle.IsEmpty)
            {
                Point pos = InputManager.MousePosition0Based;

                if (Manager.IsCornerMoving)
                {
                    StartPosition = StartPosition.Add(InputManager.MouseVelocity.X, InputManager.MouseVelocity.Y);
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
    }
}