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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public abstract class BaseShape : IDisposable
    {
        public abstract ShapeCategory ShapeCategory { get; }

        public abstract ShapeType ShapeType { get; }

        private RectangleF rectangle;

        public RectangleF Rectangle
        {
            get
            {
                return rectangle;
            }
            set
            {
                rectangle = value;
                startPosition = rectangle.Location;
                endPosition = new PointF(rectangle.X + rectangle.Width - 1, rectangle.Y + rectangle.Height - 1);
            }
        }

        public RectangleF RectangleInsideCanvas => RectangleF.Intersect(Rectangle, Manager.Form.CanvasRectangle);

        public bool IsInsideCanvas => !RectangleInsideCanvas.IsEmpty;

        public virtual bool LimitRectangleToInsideCanvas { get; }

        private PointF startPosition;

        public PointF StartPosition
        {
            get
            {
                return startPosition;
            }
            private set
            {
                startPosition = value;
                rectangle = CaptureHelpers.CreateRectangle(startPosition, endPosition);
            }
        }

        private PointF endPosition;

        public PointF EndPosition
        {
            get
            {
                return endPosition;
            }
            private set
            {
                endPosition = value;
                rectangle = CaptureHelpers.CreateRectangle(startPosition, endPosition);
            }
        }

        public SizeF InitialSize { get; set; }

        public virtual bool IsValidShape => !Rectangle.IsEmpty && Rectangle.Width >= Options.MinimumSize && Rectangle.Height >= Options.MinimumSize;

        public virtual bool IsSelectable => Manager.CurrentTool == ShapeType || Manager.CurrentTool == ShapeType.ToolSelect;

        public bool ForceProportionalResizing { get; protected set; }

        internal ShapeManager Manager { get; set; }

        protected InputManager InputManager => Manager.InputManager;
        protected RegionCaptureOptions Options => Manager.Options;
        protected AnnotationOptions AnnotationOptions => Manager.Options.AnnotationOptions;

        private PointF tempNodePos, tempStartPos, tempEndPos;
        private RectangleF tempRectangle;

        public bool IsHandledBySelectTool
        {
            get
            {
                switch (ShapeCategory)
                {
                    case ShapeCategory.Drawing:
                    case ShapeCategory.Effect:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public virtual bool Intersects(PointF position)
        {
            return Rectangle.Contains(position);
        }

        internal void ChangeNodeShape(NodeShape nodeShape)
        {
            foreach (ResizeNode node in Manager.ResizeNodes)
            {
                node.Shape = nodeShape;
            }
        }

        protected virtual void UseLightResizeNodes()
        {
            ChangeNodeShape(NodeShape.Square);
        }

        protected void UpdateNodeShape()
        {
            if (Options.UseLightResizeNodes)
            {
                UseLightResizeNodes();
            }
            else
            {
                ChangeNodeShape(NodeShape.CustomNode);
            }
        }

        public virtual void ShowNodes()
        {
            UpdateNodeShape();
            Manager.NodesVisible = true;
        }

        public virtual void Remove()
        {
            Manager.DeleteShape(this);
        }

        public void AddShapePath(GraphicsPath gp, int sizeOffset = 0)
        {
            RectangleF rect = Rectangle;

            if (sizeOffset != 0)
            {
                rect = rect.SizeOffset(sizeOffset);
            }

            OnShapePathRequested(gp, rect);
        }

        public virtual void Move(float x, float y)
        {
            StartPosition = StartPosition.Add(x, y);
            EndPosition = EndPosition.Add(x, y);
        }

        public void Move(PointF offset)
        {
            Move(offset.X, offset.Y);
        }

        public void MoveAbsolute(float x, float y, bool center = false)
        {
            if (center)
            {
                x -= Rectangle.Size.Width / 2;
                y -= Rectangle.Size.Height / 2;
            }

            Move(x - Rectangle.X, y - Rectangle.Y);
        }

        public void MoveAbsolute(PointF point, bool center = false)
        {
            MoveAbsolute(point.X, point.Y, center);
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

        public virtual BaseShape Duplicate()
        {
            ShapeManager manager = Manager;
            Manager = null;
            BaseShape shape = this.Copy();
            Manager = manager;
            shape.Manager = manager;
            return shape;
        }

        public virtual void OnCreating()
        {
            PointF pos = Manager.Form.ScaledClientMousePosition;

            if (Options.IsFixedSize && ShapeCategory == ShapeCategory.Region)
            {
                Manager.IsMoving = true;
                Rectangle = new RectangleF(new PointF(pos.X - (Options.FixedSize.Width / 2), pos.Y - (Options.FixedSize.Height / 2)), Options.FixedSize);
                OnCreated();
            }
            else
            {
                Manager.IsCreating = true;
                Rectangle = new RectangleF(pos.X, pos.Y, 1, 1);
            }
        }

        public virtual void OnCreated()
        {
            InitialSize = Rectangle.Size;

            if (ShapeCategory == ShapeCategory.Drawing || ShapeCategory == ShapeCategory.Effect)
            {
                Manager.OnImageModified();
            }
        }

        public virtual void OnMoving()
        {
        }

        public virtual void OnMoved()
        {
        }

        public virtual void OnResizing()
        {
        }

        public virtual void OnResized()
        {
        }

        public virtual void OnUpdate()
        {
            if (Manager.IsCreating)
            {
                PointF pos = Manager.Form.ScaledClientMousePosition;

                if (Manager.IsCornerMoving && !Manager.IsPanning)
                {
                    StartPosition = StartPosition.Add(Manager.Form.ScaledClientMouseVelocity);
                }

                if (Manager.IsProportionalResizing || ForceProportionalResizing)
                {
                    float degree, startDegree;

                    if (ShapeType == ShapeType.DrawingLine || ShapeType == ShapeType.DrawingArrow)
                    {
                        degree = 45;
                        startDegree = 0;
                    }
                    else
                    {
                        degree = 90;
                        startDegree = 45;
                    }

                    pos = CaptureHelpers.SnapPositionToDegree(StartPosition, pos, degree, startDegree).Round();
                }
                else if (Manager.IsSnapResizing)
                {
                    pos = Manager.SnapPosition(StartPosition, pos);
                }

                EndPosition = pos;
            }
            else if (Manager.IsMoving && !Manager.IsPanning)
            {
                Move(Manager.Form.ScaledClientMouseVelocity);
            }

            if (LimitRectangleToInsideCanvas)
            {
                StartPosition = StartPosition.Restrict(Manager.Form.CanvasRectangle);
                EndPosition = EndPosition.Restrict(Manager.Form.CanvasRectangle);
            }
        }

        public virtual void OnShapePathRequested(GraphicsPath gp, RectangleF rect)
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
            for (int i = 0; i < 8; i++)
            {
                ResizeNode node = Manager.ResizeNodes[i];
                node.Visible = true;
            }
        }

        public virtual void OnNodeUpdate()
        {
            for (int i = 0; i < 8; i++)
            {
                ResizeNode node = Manager.ResizeNodes[i];

                if (node.IsDragging)
                {
                    Manager.IsResizing = true;

                    if (!InputManager.IsBeforeMouseDown(MouseButtons.Left))
                    {
                        tempNodePos = node.Position;
                        tempStartPos = Rectangle.Location;
                        tempEndPos = new PointF(Rectangle.X + Rectangle.Width - 1, Rectangle.Y + Rectangle.Height - 1);
                        tempRectangle = Rectangle;

                        OnResizing();
                    }

                    if (Manager.IsCornerMoving || Manager.IsPanning)
                    {
                        tempStartPos.Offset(Manager.Form.ScaledClientMouseVelocity);
                        tempEndPos.Offset(Manager.Form.ScaledClientMouseVelocity);
                        tempNodePos.Offset(Manager.Form.ScaledClientMouseVelocity);
                        tempRectangle.LocationOffset(Manager.Form.ScaledClientMouseVelocity);
                    }

                    PointF pos = Manager.Form.ScaledClientMousePosition;
                    PointF startPos = tempStartPos;
                    PointF endPos = tempEndPos;

                    NodePosition nodePosition = (NodePosition)i;

                    float x = pos.X - tempNodePos.X;

                    switch (nodePosition)
                    {
                        case NodePosition.TopLeft:
                        case NodePosition.Left:
                        case NodePosition.BottomLeft:
                            startPos.X += x;
                            break;
                        case NodePosition.TopRight:
                        case NodePosition.Right:
                        case NodePosition.BottomRight:
                            endPos.X += x;
                            break;
                    }

                    float y = pos.Y - tempNodePos.Y;

                    switch (nodePosition)
                    {
                        case NodePosition.TopLeft:
                        case NodePosition.Top:
                        case NodePosition.TopRight:
                            startPos.Y += y;
                            break;
                        case NodePosition.BottomLeft:
                        case NodePosition.Bottom:
                        case NodePosition.BottomRight:
                            endPos.Y += y;
                            break;
                    }

                    StartPosition = startPos;
                    EndPosition = endPos;

                    if (Manager.IsProportionalResizing && !InitialSize.IsEmpty)
                    {
                        switch (nodePosition)
                        {
                            case NodePosition.Top:
                            case NodePosition.Right:
                            case NodePosition.Bottom:
                            case NodePosition.Left:
                                return;
                        }

                        double ratio = Math.Min(Rectangle.Width / (double)InitialSize.Width, Rectangle.Height / (double)InitialSize.Height);
                        int newWidth = (int)Math.Round(InitialSize.Width * ratio);
                        int newHeight = (int)Math.Round(InitialSize.Height * ratio);

                        PointF anchor = new PointF();

                        switch (nodePosition)
                        {
                            case NodePosition.TopLeft:
                            case NodePosition.Left:
                            case NodePosition.BottomLeft:
                                anchor.X = tempRectangle.Right - 1;
                                break;
                            case NodePosition.TopRight:
                            case NodePosition.Right:
                            case NodePosition.BottomRight:
                                anchor.X = tempRectangle.X;
                                break;
                        }

                        switch (nodePosition)
                        {
                            case NodePosition.TopLeft:
                            case NodePosition.Top:
                            case NodePosition.TopRight:
                                anchor.Y = tempRectangle.Bottom - 1;
                                break;
                            case NodePosition.BottomLeft:
                            case NodePosition.Bottom:
                            case NodePosition.BottomRight:
                                anchor.Y = tempRectangle.Y;
                                break;
                        }

                        RectangleF newRect = Rectangle;

                        if (pos.X < anchor.X)
                        {
                            newRect.X = newRect.Right - newWidth;
                        }

                        newRect.Width = newWidth;

                        if (pos.Y < anchor.Y)
                        {
                            newRect.Y = newRect.Bottom - newHeight;
                        }

                        newRect.Height = newHeight;

                        Rectangle = newRect;
                    }

                    if (LimitRectangleToInsideCanvas)
                    {
                        Rectangle = RectangleInsideCanvas;
                    }
                }
            }
        }

        public virtual void OnNodePositionUpdate()
        {
            float xStart = Rectangle.X;
            float xMid = Rectangle.X + (Rectangle.Width / 2);
            float xEnd = Rectangle.X + Rectangle.Width - 1;

            float yStart = Rectangle.Y;
            float yMid = Rectangle.Y + (Rectangle.Height / 2);
            float yEnd = Rectangle.Y + Rectangle.Height - 1;

            Manager.ResizeNodes[(int)NodePosition.TopLeft].Position = new PointF(xStart, yStart);
            Manager.ResizeNodes[(int)NodePosition.Top].Position = new PointF(xMid, yStart);
            Manager.ResizeNodes[(int)NodePosition.TopRight].Position = new PointF(xEnd, yStart);
            Manager.ResizeNodes[(int)NodePosition.Right].Position = new PointF(xEnd, yMid);
            Manager.ResizeNodes[(int)NodePosition.BottomRight].Position = new PointF(xEnd, yEnd);
            Manager.ResizeNodes[(int)NodePosition.Bottom].Position = new PointF(xMid, yEnd);
            Manager.ResizeNodes[(int)NodePosition.BottomLeft].Position = new PointF(xStart, yEnd);
            Manager.ResizeNodes[(int)NodePosition.Left].Position = new PointF(xStart, yMid);

            for (int i = 0; i < 8; i++)
            {
                Manager.ResizeNodes[i].Visible = true;
            }

            if (Manager.ResizeNodes[(int)NodePosition.Right].Rectangle.IntersectsWith(Manager.ResizeNodes[(int)NodePosition.BottomRight].Rectangle))
            {
                Manager.ResizeNodes[(int)NodePosition.Left].Visible =
                    Manager.ResizeNodes[(int)NodePosition.Right].Visible = false;
            }

            if (Manager.ResizeNodes[(int)NodePosition.Bottom].Rectangle.IntersectsWith(Manager.ResizeNodes[(int)NodePosition.BottomRight].Rectangle))
            {
                Manager.ResizeNodes[(int)NodePosition.Top].Visible =
                    Manager.ResizeNodes[(int)NodePosition.Bottom].Visible = false;
            }

            if (Manager.ResizeNodes[(int)NodePosition.TopRight].Rectangle.IntersectsWith(Manager.ResizeNodes[(int)NodePosition.BottomRight].Rectangle))
            {
                Manager.ResizeNodes[(int)NodePosition.TopLeft].Visible =
                    Manager.ResizeNodes[(int)NodePosition.Top].Visible =
                    Manager.ResizeNodes[(int)NodePosition.TopRight].Visible = false;
            }

            if (Manager.ResizeNodes[(int)NodePosition.BottomLeft].Rectangle.IntersectsWith(Manager.ResizeNodes[(int)NodePosition.BottomRight].Rectangle))
            {
                Manager.ResizeNodes[(int)NodePosition.TopLeft].Visible =
                    Manager.ResizeNodes[(int)NodePosition.Left].Visible =
                    Manager.ResizeNodes[(int)NodePosition.BottomLeft].Visible = false;
            }
        }

        public virtual void Dispose()
        {
        }
    }
}