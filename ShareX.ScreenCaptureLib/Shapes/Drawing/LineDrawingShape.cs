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
    public class LineDrawingShape : BaseDrawingShape
    {
        public const int MaximumCenterPointCount = 5;

        public override ShapeType ShapeType { get; } = ShapeType.DrawingLine;

        public Point[] Points { get; set; }
        public bool CenterNodeActive { get; set; }
        public int CenterPointCount { get; set; }

        public override bool IsValidShape => Rectangle.Width > 1 || Rectangle.Height > 1;

        private void AdjustPoints(int centerPointCount)
        {
            Point[] newPoints = new Point[2 + centerPointCount];

            if (Points != null)
            {
                newPoints[0] = Points[0];
                newPoints[newPoints.Length - 1] = Points[Points.Length - 1];
            }

            for (int i = 0; i < newPoints.Length; i++)
            {
                if (newPoints[i] == null) newPoints[i] = new Point();
            }

            Points = newPoints;
        }

        public override void OnConfigLoad()
        {
            base.OnConfigLoad();

            int previousCenterPointCount = CenterPointCount;
            CenterPointCount = AnnotationOptions.LineCenterPointCount.Between(0, MaximumCenterPointCount);

            AdjustPoints(CenterPointCount);

            if (CenterPointCount != previousCenterPointCount)
            {
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

                Rectangle = Points.CreateRectangle();
            }
        }

        public override void OnDraw(Graphics g)
        {
            DrawLine(g);
        }

        protected void DrawLine(Graphics g)
        {
            if (Shadow)
            {
                Point[] shadowPoints = new Point[Points.Length];

                for (int i = 0; i < shadowPoints.Length; i++)
                {
                    shadowPoints[i] = Points[i].Add(ShadowOffset);
                }

                DrawLine(g, ShadowColor, BorderSize, shadowPoints);
            }

            DrawLine(g, BorderColor, BorderSize, Points);
        }

        protected void DrawLine(Graphics g, Color borderColor, int borderSize, Point[] points)
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

        protected virtual Pen CreatePen(Color borderColor, int borderSize)
        {
            return new Pen(borderColor, borderSize)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };
        }

        public override void Move(int x, int y)
        {
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
            for (int i = 0; i < 8; i++)
            {
                ResizeNode node = Manager.ResizeNodes[i];
                node.Visible = false;
            }

            for (int i = 0; i < Points.Length; i++)
            {
                Manager.ResizeNodes[i].Visible = true;
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

                    Points[i] = InputManager.ClientMousePosition;
                }
            }
        }

        private void AutoPositionCenterPoints()
        {
            if (!CenterNodeActive)
            {
                for (int i = 1; i < Points.Length - 1; i++)
                {
                    Points[i] = new Point((int)MathHelpers.Lerp(Points[0].X, Points[Points.Length - 1].X, i / (CenterPointCount + 1f)),
                        (int)MathHelpers.Lerp(Points[0].Y, Points[Points.Length - 1].Y, i / (CenterPointCount + 1f)));
                }
            }
        }

        public override void OnNodePositionUpdate()
        {
            for (int i = 0; i < Points.Length; i++)
            {
                Manager.ResizeNodes[i].Position = Points[i];
            }

            Manager.ResizeNodes[0].Visible = !Manager.ResizeNodes[0].Rectangle.IntersectsWith(Manager.ResizeNodes[Manager.ResizeNodes.Count - 1].Rectangle);

            for (int i = 1; i < Points.Length - 1; i++)
            {
                Manager.ResizeNodes[i].Visible = !Manager.ResizeNodes[i].Rectangle.IntersectsWith(Manager.ResizeNodes[Manager.ResizeNodes.Count - 1].Rectangle);
            }
        }
    }
}