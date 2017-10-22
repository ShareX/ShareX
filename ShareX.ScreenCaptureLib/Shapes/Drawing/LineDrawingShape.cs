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

namespace ShareX.ScreenCaptureLib
{
    public class LineDrawingShape : BaseDrawingShape
    {
        public const int MaximumCenterPointCount = 5;

        public override ShapeType ShapeType { get; } = ShapeType.DrawingLine;

        public bool CenterNodeActive { get; set; }
        public Point[] CenterPoints { get; set; } = new Point[MaximumCenterPointCount];
        public int CenterPointCount { get; set; }

        public override bool IsValidShape => StartPosition != EndPosition;

        public override void OnConfigLoad()
        {
            base.OnConfigLoad();
            CenterPointCount = AnnotationOptions.LineCenterPointCount;

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

            if (CenterNodeActive)
            {
                Rectangle = CreatePoints().CreateRectangle();
            }
        }

        public override void OnDraw(Graphics g)
        {
            DrawLine(g);
        }

        protected void DrawLine(Graphics g)
        {
            List<Point> points = CreatePoints();

            if (Shadow)
            {
                List<Point> shadowPoints = new List<Point>();

                foreach (Point point in points)
                {
                    shadowPoints.Add(point.Add(ShadowOffset));
                }

                DrawLine(g, ShadowColor, BorderSize, shadowPoints);
            }

            DrawLine(g, BorderColor, BorderSize, points);
        }

        protected void DrawLine(Graphics g, Color borderColor, int borderSize, List<Point> points)
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
                    if (points.Count > 2)
                    {
                        g.DrawCurve(pen, points.ToArray());
                    }
                    else
                    {
                        g.DrawLine(pen, points[0], points[1]);
                    }
                }

                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.Default;
            }
        }

        private List<Point> CreatePoints()
        {
            List<Point> points = new List<Point>();
            points.Add(StartPosition);
            if (CenterNodeActive)
            {
                for (int i = 0; i < CenterPointCount; i++)
                {
                    points.Add(CenterPoints[i]);
                }
            }
            points.Add(EndPosition);
            return points;
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
            StartPosition = StartPosition.Add(x, y);
            EndPosition = EndPosition.Add(x, y);

            for (int i = 0; i < CenterPointCount; i++)
            {
                CenterPoints[i] = CenterPoints[i].Add(x, y);
            }
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
            for (int i = 0; i < 8; i++)
            {
                ResizeNode node = Manager.ResizeNodes[i];
                node.Visible = false;
            }

            for (int i = 0; i < 2 + CenterPointCount; i++)
            {
                Manager.ResizeNodes[i].Visible = true;
            }
        }

        public override void OnNodeUpdate()
        {
            if (Manager.ResizeNodes[0].IsDragging)
            {
                Manager.IsResizing = true;

                StartPosition = InputManager.MousePosition0Based;
            }
            else if (Manager.ResizeNodes[1].IsDragging)
            {
                Manager.IsResizing = true;

                EndPosition = InputManager.MousePosition0Based;
            }
            else
            {
                for (int i = 2; i < 2 + CenterPointCount; i++)
                {
                    if (Manager.ResizeNodes[i].IsDragging)
                    {
                        CenterNodeActive = true;

                        Manager.IsResizing = true;

                        CenterPoints[i - 2] = InputManager.MousePosition0Based;
                    }
                }
            }
        }

        public override void OnNodePositionUpdate()
        {
            Manager.ResizeNodes[0].Position = StartPosition;
            Manager.ResizeNodes[1].Position = EndPosition;

            if (!CenterNodeActive)
            {
                for (int i = 0; i < CenterPointCount; i++)
                {
                    CenterPoints[i] = new Point((int)MathHelpers.Lerp(StartPosition.X, EndPosition.X, (i + 1f) / (CenterPointCount + 1f)),
                        (int)MathHelpers.Lerp(StartPosition.Y, EndPosition.Y, (i + 1f) / (CenterPointCount + 1f)));
                }
            }

            for (int i = 2; i < 2 + CenterPointCount; i++)
            {
                Manager.ResizeNodes[i].Position = CenterPoints[i - 2];
            }

            Manager.ResizeNodes[0].Visible = !Manager.ResizeNodes[0].Rectangle.IntersectsWith(Manager.ResizeNodes[1].Rectangle);

            for (int i = 2; i < 2 + CenterPointCount; i++)
            {
                Manager.ResizeNodes[i].Visible = !Manager.ResizeNodes[i].Rectangle.IntersectsWith(Manager.ResizeNodes[1].Rectangle);
            }
        }
    }
}