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

namespace ShareX.ScreenCaptureLib
{
    public class LineDrawingShape : BaseDrawingShape
    {
        public const int MaximumCenterPointCount = 5;
        private const int MinimumCollisionSize = 10;

        public override ShapeType ShapeType { get; } = ShapeType.DrawingLine;

        public PointF[] Points { get; private set; } = new PointF[2];
        public bool CenterNodeActive { get; private set; }
        public int CenterPointCount { get; private set; }

        public override bool IsValidShape => Rectangle.Width >= Options.MinimumSize || Rectangle.Height >= Options.MinimumSize;

        protected override void UseLightResizeNodes()
        {
            ChangeNodeShape(NodeShape.Circle);
        }

        private void AdjustPoints(int centerPointCount)
        {
            PointF[] newPoints = new PointF[2 + centerPointCount];

            if (Points != null)
            {
                newPoints[0] = Points[0];
                newPoints[newPoints.Length - 1] = Points[Points.Length - 1];
            }

            Points = newPoints;
        }

        private void AutoPositionCenterPoints()
        {
            if (!CenterNodeActive)
            {
                for (int i = 1; i < Points.Length - 1; i++)
                {
                    Points[i] = new PointF(MathHelpers.Lerp(Points[0].X, Points[Points.Length - 1].X, i / (CenterPointCount + 1f)),
                        MathHelpers.Lerp(Points[0].Y, Points[Points.Length - 1].Y, i / (CenterPointCount + 1f)));
                }
            }
        }

        public override void OnConfigLoad()
        {
            base.OnConfigLoad();

            int previousCenterPointCount = CenterPointCount;
            CenterPointCount = AnnotationOptions.LineCenterPointCount.Clamp(0, MaximumCenterPointCount);

            if (CenterPointCount != previousCenterPointCount)
            {
                AdjustPoints(CenterPointCount);
                CenterNodeActive = false;
                AutoPositionCenterPoints();
            }

            if (Manager.NodesVisible)
            {
                OnNodeVisible();
            }
        }

        public override void OnConfigSave()
        {
            base.OnConfigSave();
            AnnotationOptions.LineCenterPointCount = CenterPointCount;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Manager.IsCreating)
            {
                Points[0] = StartPosition;
                Points[Points.Length - 1] = EndPosition;
            }
            else
            {
                AutoPositionCenterPoints();
                CalculateRectangle();
            }
        }

        private void CalculateRectangle()
        {
            Rectangle = Points.CreateRectangle();

            if (Rectangle.Width < MinimumCollisionSize)
            {
                Rectangle = new RectangleF(Rectangle.X - (MinimumCollisionSize / 2), Rectangle.Y, Rectangle.Width + MinimumCollisionSize, Rectangle.Height);
            }

            if (Rectangle.Height < MinimumCollisionSize)
            {
                Rectangle = new RectangleF(Rectangle.X, Rectangle.Y - (MinimumCollisionSize / 2), Rectangle.Width, Rectangle.Height + MinimumCollisionSize);
            }
        }

        public override void OnDraw(Graphics g)
        {
            DrawLine(g);
        }

        protected void DrawLine(Graphics g)
        {
            int borderSize = Math.Max(BorderSize, 1);

            if (Shadow)
            {
                PointF[] shadowPoints = new PointF[Points.Length];

                for (int i = 0; i < shadowPoints.Length; i++)
                {
                    shadowPoints[i] = Points[i].Add(ShadowOffset);
                }

                DrawLine(g, ShadowColor, borderSize, BorderStyle, shadowPoints);
            }

            DrawLine(g, BorderColor, borderSize, BorderStyle, Points);
        }

        protected void DrawLine(Graphics g, Color borderColor, int borderSize, BorderStyle borderStyle, PointF[] points)
        {
            if (borderSize > 0 && borderColor.A > 0)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;

                if (borderSize.IsEvenNumber())
                {
                    g.PixelOffsetMode = PixelOffsetMode.Half;
                }

                using (Pen pen = CreatePen(borderColor, borderSize, borderStyle))
                {
                    if (CenterNodeActive && points.Length > 2)
                    {
                        g.DrawCurve(pen, points);
                    }
                    else
                    {
                        g.DrawLine(pen, points[0], points[points.Length - 1]);
                    }
                }

                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.Default;
            }
        }

        protected virtual Pen CreatePen(Color borderColor, int borderSize, BorderStyle borderStyle)
        {
            return new Pen(borderColor, borderSize)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round,
                DashStyle = (DashStyle)borderStyle
            };
        }

        public override void Move(float x, float y)
        {
            base.Move(x, y);

            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = Points[i].Add(x, y);
            }
        }

        public override void Resize(int x, int y, bool fromBottomRight)
        {
            if (fromBottomRight)
            {
                Points[Points.Length - 1] = Points[Points.Length - 1].Add(x, y);
            }
            else
            {
                Points[0] = Points[0].Add(x, y);
            }
        }

        public override void OnNodeVisible()
        {
            for (int i = 0; i < Manager.ResizeNodes.Length; i++)
            {
                Manager.ResizeNodes[i].Visible = i < Points.Length;
            }
        }

        public override void OnNodeUpdate()
        {
            for (int i = 0; i < Points.Length; i++)
            {
                if (Manager.ResizeNodes[i].IsDragging)
                {
                    Manager.IsResizing = true;

                    if (i > 0 && i < Points.Length - 1)
                    {
                        CenterNodeActive = true;
                    }

                    Points[i] = Manager.Form.ScaledClientMousePosition;
                }
            }
        }

        public override void OnNodePositionUpdate()
        {
            for (int i = 0; i < Points.Length; i++)
            {
                Manager.ResizeNodes[i].Position = Points[i];

                if (i < Points.Length - 1)
                {
                    Manager.ResizeNodes[i].Visible = !Manager.ResizeNodes[i].Rectangle.IntersectsWith(Manager.ResizeNodes[Points.Length - 1].Rectangle);
                }
            }
        }
    }
}