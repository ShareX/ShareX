#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScreenCapture
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

                foreach (NodeObject node in nodes)
                {
                    node.Visible = visible;
                }
            }
        }

        public bool IsResizing { get; private set; }
        public int MaxMoveSpeed { get; set; }
        public int MinMoveSpeed { get; set; }
        public bool IsBottomRightResizing { get; set; }

        private Surface surface;
        private AreaManager areaManager;
        private NodeObject[] nodes;
        private Rectangle tempRect;

        public ResizeManager(Surface surface, AreaManager areaManager)
        {
            this.surface = surface;
            this.areaManager = areaManager;

            MinMoveSpeed = surface.Config.MinMoveSpeed;
            MaxMoveSpeed = surface.Config.MaxMoveSpeed;

            surface.KeyDown += surface_KeyDown;

            nodes = new NodeObject[8];

            for (int i = 0; i < 8; i++)
            {
                nodes[i] = surface.MakeNode();
            }

            nodes[(int)NodePosition.BottomRight].Order = 10;
        }

        public void Update()
        {
            if (Visible && nodes != null)
            {
                if (InputManager.IsMouseDown(MouseButtons.Left))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (nodes[i].IsDragging)
                        {
                            IsResizing = true;

                            if (!InputManager.IsBeforeMouseDown(MouseButtons.Left))
                            {
                                tempRect = areaManager.CurrentArea;
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

                            areaManager.CurrentArea = CaptureHelpers.FixRectangle(tempRect);

                            break;
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
            int speed;

            if (e.Control)
            {
                speed = MaxMoveSpeed;
            }
            else
            {
                speed = MinMoveSpeed;
            }

            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (!areaManager.IsCurrentAreaValid || areaManager.IsCreating)
                    {
                        Cursor.Position = new Point(Cursor.Position.X - speed, Cursor.Position.Y);
                    }
                    else
                    {
                        if (e.Shift)
                        {
                            MoveCurrentArea(-speed, 0);
                        }
                        else
                        {
                            ResizeCurrentArea(-speed, 0, IsBottomRightResizing);
                        }
                    }
                    break;
                case Keys.Right:
                    if (!areaManager.IsCurrentAreaValid || areaManager.IsCreating)
                    {
                        Cursor.Position = new Point(Cursor.Position.X + speed, Cursor.Position.Y);
                    }
                    else
                    {
                        if (e.Shift)
                        {
                            MoveCurrentArea(speed, 0);
                        }
                        else
                        {
                            ResizeCurrentArea(speed, 0, IsBottomRightResizing);
                        }
                    }
                    break;
                case Keys.Up:
                    if (!areaManager.IsCurrentAreaValid || areaManager.IsCreating)
                    {
                        Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y - speed);
                    }
                    else
                    {
                        if (e.Shift)
                        {
                            MoveCurrentArea(0, -speed);
                        }
                        else
                        {
                            ResizeCurrentArea(0, -speed, IsBottomRightResizing);
                        }
                    }
                    break;
                case Keys.Down:
                    if (!areaManager.IsCurrentAreaValid || areaManager.IsCreating)
                    {
                        Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y + speed);
                    }
                    else
                    {
                        if (e.Shift)
                        {
                            MoveCurrentArea(0, speed);
                        }
                        else
                        {
                            ResizeCurrentArea(0, speed, IsBottomRightResizing);
                        }
                    }
                    break;
                case Keys.Tab:
                    IsBottomRightResizing = !IsBottomRightResizing;
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

        public void UpdateNodePositions()
        {
            UpdateNodePositions(areaManager.CurrentArea);
        }

        private void UpdateNodePositions(Rectangle rect)
        {
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

        public void MoveCurrentArea(int x, int y)
        {
            areaManager.CurrentArea = new Rectangle(new Point(areaManager.CurrentArea.X + x, areaManager.CurrentArea.Y + y), areaManager.CurrentArea.Size);
        }

        public void ResizeCurrentArea(int x, int y, bool isBottomRightMoving)
        {
            if (isBottomRightMoving)
            {
                areaManager.CurrentArea = new Rectangle(areaManager.CurrentArea.X, areaManager.CurrentArea.Y,
                    areaManager.CurrentArea.Width + x, areaManager.CurrentArea.Height + y);
            }
            else
            {
                areaManager.CurrentArea = new Rectangle(areaManager.CurrentArea.X + x, areaManager.CurrentArea.Y + y,
                    areaManager.CurrentArea.Width - x, areaManager.CurrentArea.Height - y);
            }
        }
    }
}