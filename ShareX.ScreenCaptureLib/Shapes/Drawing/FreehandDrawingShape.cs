#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace ShareX.ScreenCaptureLib
{
    public class FreehandDrawingShape : BaseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingFreehand;

        public override bool IsValidShape => positions.Count > 0;

        public Point LastPosition
        {
            get
            {
                if (positions.Count > 0)
                {
                    return positions[positions.Count - 1];
                }

                return Point.Empty;
            }
            set
            {
                if (positions.Count > 0)
                {
                    positions[positions.Count - 1] = value;
                }
            }
        }

        private List<Point> positions = new List<Point>();
        private bool isPolygonMode;

        public override bool Intersects(Point position)
        {
            return false;
        }

        public override void ShowNodes()
        {
        }

        public override void OnUpdate()
        {
            if (Manager.IsCreating)
            {
                if (Manager.IsCornerMoving)
                {
                    Move(InputManager.MouseVelocity);
                }
                else
                {
                    Point pos = InputManager.ClientMousePosition;

                    if (positions.Count == 0 || (!Manager.IsProportionalResizing && LastPosition != pos))
                    {
                        positions.Add(pos);
                    }

                    if (Manager.IsProportionalResizing)
                    {
                        if (!isPolygonMode)
                        {
                            positions.Add(pos);
                        }

                        LastPosition = pos;
                    }

                    isPolygonMode = Manager.IsProportionalResizing;

                    Rectangle = positions.CreateRectangle();
                }
            }
            else if (Manager.IsMoving)
            {
                Move(InputManager.MouseVelocity);
            }
        }

        public override void OnDraw(Graphics g)
        {
            DrawFreehand(g);
        }

        protected void DrawFreehand(Graphics g)
        {
            if (Shadow)
            {
                DrawFreehand(g, ShadowColor, BorderSize, positions.Select(x => x.Add(ShadowOffset)).ToArray());
            }

            DrawFreehand(g, BorderColor, BorderSize, positions.ToArray());
        }

        protected void DrawFreehand(Graphics g, Color borderColor, int borderSize, Point[] points)
        {
            if (points.Length > 0 && borderSize > 0 && borderColor.A > 0)
            {
                if (Manager.IsRenderingOutput)
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                }
                else
                {
                    g.SmoothingMode = SmoothingMode.HighSpeed;
                }

                if (points.Length == 1)
                {
                    using (Brush brush = new SolidBrush(borderColor))
                    {
                        Rectangle rect = new Rectangle((int)(points[0].X - borderSize / 2f), (int)(points[0].Y - borderSize / 2f), borderSize, borderSize);
                        g.FillEllipse(brush, rect);
                    }
                }
                else
                {
                    using (Pen pen = new Pen(borderColor, borderSize) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
                    {
                        g.DrawLines(pen, points.ToArray());
                    }
                }

                g.SmoothingMode = SmoothingMode.None;
            }
        }

        public override void Move(int x, int y)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                positions[i] = positions[i].Add(x, y);
            }

            Rectangle = Rectangle.LocationOffset(x, y);
        }

        public override void Resize(int x, int y, bool fromBottomRight)
        {
            Move(x, y);
        }
    }
}