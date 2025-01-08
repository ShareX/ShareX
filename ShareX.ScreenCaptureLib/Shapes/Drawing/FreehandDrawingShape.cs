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

        public override bool IsSelectable => Manager.CurrentTool == ShapeType.ToolSelect;

        public PointF LastPosition
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

        protected List<PointF> positions = new List<PointF>();
        private bool isPolygonMode;

        public override void ShowNodes()
        {
        }

        public override void OnUpdate()
        {
            if (Manager.IsCreating)
            {
                if (Manager.IsCornerMoving && !Manager.IsPanning)
                {
                    Move(Manager.Form.ScaledClientMouseVelocity);
                }
                else
                {
                    PointF pos = Manager.Form.ScaledClientMousePosition;

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
                Move(Manager.Form.ScaledClientMouseVelocity);
            }
        }

        public override void OnDraw(Graphics g)
        {
            DrawFreehand(g);
        }

        protected void DrawFreehand(Graphics g)
        {
            int borderSize = Math.Max(BorderSize, 1);

            if (Shadow)
            {
                DrawFreehand(g, ShadowColor, borderSize, BorderStyle, positions.Select(x => x.Add(ShadowOffset)).ToArray());
            }

            DrawFreehand(g, BorderColor, borderSize, BorderStyle, positions.ToArray());
        }

        protected void DrawFreehand(Graphics g, Color borderColor, int borderSize, BorderStyle borderStyle, PointF[] points)
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
                        Rectangle rect = new Rectangle((int)(points[0].X - (borderSize / 2f)), (int)(points[0].Y - (borderSize / 2f)), borderSize, borderSize);
                        g.FillEllipse(brush, rect);
                    }
                }
                else
                {
                    using (Pen pen = CreatePen(borderColor, borderSize, borderStyle))
                    {
                        g.DrawLines(pen, points.ToArray());
                    }
                }

                g.SmoothingMode = SmoothingMode.None;
            }
        }

        protected virtual Pen CreatePen(Color borderColor, int borderSize, BorderStyle borderStyle)
        {
            Pen pen = new Pen(borderColor, borderSize);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            pen.LineJoin = LineJoin.Round;
            pen.DashStyle = (DashStyle)borderStyle;
            return pen;
        }

        public override void Move(float x, float y)
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