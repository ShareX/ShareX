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
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class ResizeManager
    {
        private bool visible;

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;

                if (!visible)
                {
                    foreach (NodeObject node in nodes)
                    {
                        node.Visible = visible;
                    }
                }
                else
                {
                    BaseShape shape = areaManager.CurrentShape;

                    if (shape != null)
                    {
                        if (shape.NodeType == NodeType.Rectangle)
                        {
                            foreach (NodeObject node in nodes)
                            {
                                node.Shape = NodeShape.Square;
                                node.Visible = visible;
                            }
                        }
                        else if (shape.NodeType == NodeType.Line)
                        {
                            nodes[(int)NodePosition.TopLeft].Shape = nodes[(int)NodePosition.BottomRight].Shape = NodeShape.Circle;
                            nodes[(int)NodePosition.TopLeft].Visible = nodes[(int)NodePosition.BottomRight].Visible = true;
                        }
                    }
                }
            }
        }

        public bool IsResizing { get; private set; }
        public int MaxMoveSpeed { get; set; }
        public int MinMoveSpeed { get; set; }
        public bool IsBottomRightResizing { get; set; }

        private bool IsUpPressed { get; set; }
        private bool IsDownPressed { get; set; }
        private bool IsLeftPressed { get; set; }
        private bool IsRightPressed { get; set; }

        private AreaManager areaManager;
        private NodeObject[] nodes;
        private Rectangle tempRect;

        public ResizeManager(SurfaceForm surface, AreaManager areaManager)
        {
            this.areaManager = areaManager;

            MinMoveSpeed = surface.Config.MinMoveSpeed;
            MaxMoveSpeed = surface.Config.MaxMoveSpeed;

            surface.KeyDown += surface_KeyDown;
            surface.KeyUp += surface_KeyUp;

            nodes = new NodeObject[8];

            for (int i = 0; i < 8; i++)
            {
                nodes[i] = surface.MakeNode();
            }

            nodes[(int)NodePosition.BottomRight].Order = 10;
        }

        public void Update()
        {
            BaseShape shape = areaManager.CurrentShape;

            if (shape != null && Visible && nodes != null)
            {
                if (InputManager.IsMouseDown(MouseButtons.Left))
                {
                    if (shape.NodeType == NodeType.Rectangle)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            if (nodes[i].IsDragging)
                            {
                                IsResizing = true;

                                if (!InputManager.IsBeforeMouseDown(MouseButtons.Left))
                                {
                                    tempRect = shape.Rectangle;
                                }

                                NodePosition nodePosition = (NodePosition)i;

                                int x = InputManager.MouseVelocity.X;

                                switch (nodePosition)
                                {
                                    case NodePosition.TopLeft:
                                    case NodePosition.Left:
                                    case NodePosition.BottomLeft:
                                        tempRect.X += x;
                                        tempRect.Width -= x;
                                        break;
                                    case NodePosition.TopRight:
                                    case NodePosition.Right:
                                    case NodePosition.BottomRight:
                                        tempRect.Width += x;
                                        break;
                                }

                                int y = InputManager.MouseVelocity.Y;

                                switch (nodePosition)
                                {
                                    case NodePosition.TopLeft:
                                    case NodePosition.Top:
                                    case NodePosition.TopRight:
                                        tempRect.Y += y;
                                        tempRect.Height -= y;
                                        break;
                                    case NodePosition.BottomLeft:
                                    case NodePosition.Bottom:
                                    case NodePosition.BottomRight:
                                        tempRect.Height += y;
                                        break;
                                }

                                shape.Rectangle = CaptureHelpers.FixRectangle(tempRect);

                                break;
                            }
                        }
                    }
                    else if (shape.NodeType == NodeType.Line)
                    {
                        if (nodes[(int)NodePosition.TopLeft].IsDragging)
                        {
                            IsResizing = true;
                            shape.StartPosition = new Point(InputManager.MousePosition0Based.X, InputManager.MousePosition0Based.Y);
                        }
                        else if (nodes[(int)NodePosition.BottomRight].IsDragging)
                        {
                            IsResizing = true;
                            shape.EndPosition = new Point(InputManager.MousePosition0Based.X, InputManager.MousePosition0Based.Y);
                        }
                    }
                }
                else
                {
                    IsResizing = false;
                }

                UpdateNodePositions();
            }
        }

        private void surface_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    IsUpPressed = true;
                    break;
                case Keys.Down:
                    IsDownPressed = true;
                    break;
                case Keys.Left:
                    IsLeftPressed = true;
                    break;
                case Keys.Right:
                    IsRightPressed = true;
                    break;
                case Keys.Tab:
                    IsBottomRightResizing = !IsBottomRightResizing;
                    return;
            }

            // Calculate cursor movement
            int speed = e.Control ? MaxMoveSpeed : MinMoveSpeed;
            int y = IsUpPressed && IsDownPressed ? 0 : IsDownPressed ? speed : IsUpPressed ? -speed : 0;
            int x = IsLeftPressed && IsRightPressed ? 0 : IsRightPressed ? speed : IsLeftPressed ? -speed : 0;

            // Move the cursor
            if (!areaManager.IsCurrentRegionValid || areaManager.IsCreating)
            {
                Cursor.Position = new Point(Cursor.Position.X + x, Cursor.Position.Y + y);
            }
            else
            {
                if (e.Shift)
                {
                    MoveCurrentArea(x, y);
                }
                else
                {
                    ResizeCurrentArea(x, y, IsBottomRightResizing);
                }
            }
        }

        private void surface_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    IsUpPressed = false;
                    break;
                case Keys.Down:
                    IsDownPressed = false;
                    break;
                case Keys.Left:
                    IsLeftPressed = false;
                    break;
                case Keys.Right:
                    IsRightPressed = false;
                    break;
            }
        }

        public bool IsCursorOnNode()
        {
            return Visible && nodes.Any(node => node.IsMouseHover);
        }

        public void Show()
        {
            UpdateNodePositions();

            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        private void UpdateNodePositions()
        {
            BaseShape shape = areaManager.CurrentShape;

            if (shape != null)
            {
                if (shape.NodeType == NodeType.Rectangle)
                {
                    Rectangle rect = shape.Rectangle;

                    float xStart = rect.X;
                    float xMid = rect.X + rect.Width / 2;
                    float xEnd = rect.X + rect.Width - 1;

                    float yStart = rect.Y;
                    float yMid = rect.Y + rect.Height / 2;
                    float yEnd = rect.Y + rect.Height - 1;

                    nodes[(int)NodePosition.TopLeft].Position = new PointF(xStart, yStart);
                    nodes[(int)NodePosition.Top].Position = new PointF(xMid, yStart);
                    nodes[(int)NodePosition.TopRight].Position = new PointF(xEnd, yStart);
                    nodes[(int)NodePosition.Right].Position = new PointF(xEnd, yMid);
                    nodes[(int)NodePosition.BottomRight].Position = new PointF(xEnd, yEnd);
                    nodes[(int)NodePosition.Bottom].Position = new PointF(xMid, yEnd);
                    nodes[(int)NodePosition.BottomLeft].Position = new PointF(xStart, yEnd);
                    nodes[(int)NodePosition.Left].Position = new PointF(xStart, yMid);
                }
                else if (shape.NodeType == NodeType.Line)
                {
                    nodes[(int)NodePosition.TopLeft].Position = shape.StartPosition;
                    nodes[(int)NodePosition.BottomRight].Position = shape.EndPosition;
                }
            }
        }

        public void MoveCurrentArea(int x, int y)
        {
            areaManager.CurrentRectangle = new Rectangle(new Point(areaManager.CurrentRectangle.X + x, areaManager.CurrentRectangle.Y + y), areaManager.CurrentRectangle.Size);
        }

        public void ResizeCurrentArea(int x, int y, bool isBottomRightMoving)
        {
            if (isBottomRightMoving)
            {
                areaManager.CurrentRectangle = new Rectangle(areaManager.CurrentRectangle.X, areaManager.CurrentRectangle.Y,
                    areaManager.CurrentRectangle.Width + x, areaManager.CurrentRectangle.Height + y);
            }
            else
            {
                areaManager.CurrentRectangle = new Rectangle(areaManager.CurrentRectangle.X + x, areaManager.CurrentRectangle.Y + y,
                    areaManager.CurrentRectangle.Width - x, areaManager.CurrentRectangle.Height - y);
            }
        }
    }
}