/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Greenshot.Drawing.Fields;
using Greenshot.Helpers;
using Greenshot.Plugin.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Greenshot.Drawing
{
    /// <summary>
    /// Description of PathContainer.
    /// </summary>
    [Serializable]
    public class FreehandContainer : DrawableContainer
    {
        private static readonly float[] POINT_OFFSET = new float[] { 0.5f, 0.25f, 0.75f };

        [NonSerialized]
        private GraphicsPath freehandPath = new GraphicsPath();
        private Rectangle myBounds = Rectangle.Empty;
        private Point lastMouse = Point.Empty;
        private readonly List<Point> capturePoints = new List<Point>();
        private bool isRecalculated;

        /// <summary>
        /// Constructor
        /// </summary>
        public FreehandContainer(Surface parent) : base(parent)
        {
            Init();
            Width = parent.Width;
            Height = parent.Height;
            Top = 0;
            Left = 0;
        }

        protected override void InitializeFields()
        {
            AddField(GetType(), FieldType.LINE_THICKNESS, 3);
            AddField(GetType(), FieldType.LINE_COLOR, DefaultLineColor);
        }

        protected void Init()
        {
            if (_grippers != null)
            {
                for (int i = 0; i < _grippers.Length; i++)
                {
                    _grippers[i].Enabled = false;
                    _grippers[i].Visible = false;
                }
            }
        }

        public override void Transform(Matrix matrix)
        {
            Point[] points = capturePoints.ToArray();

            matrix.TransformPoints(points);
            capturePoints.Clear();
            capturePoints.AddRange(points);
            RecalculatePath();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            InitGrippers();
            DoLayout();
            Init();
            RecalculatePath();
        }

        /// <summary>
        /// This Dispose is called from the Dispose and the Destructor.
        /// </summary>
        /// <param name="disposing">When disposing==true all non-managed resources should be freed too!</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (freehandPath != null)
                {
                    freehandPath.Dispose();
                }
            }
            freehandPath = null;
        }

        /// <summary>
        /// Called from Surface (the parent) when the drawing begins (mouse-down)
        /// </summary>
        /// <returns>true if the surface doesn't need to handle the event</returns>
        public override bool HandleMouseDown(int mouseX, int mouseY)
        {
            lastMouse = new Point(mouseX, mouseY);
            capturePoints.Add(lastMouse);
            return true;
        }

        /// <summary>
        /// Called from Surface (the parent) if a mouse move is made while drawing
        /// </summary>
        /// <returns>true if the surface doesn't need to handle the event</returns>
        public override bool HandleMouseMove(int mouseX, int mouseY)
        {
            Point previousPoint = capturePoints[capturePoints.Count - 1];

            if (GeometryHelper.Distance2D(previousPoint.X, previousPoint.Y, mouseX, mouseY) >= 2 * EditorConfig.FreehandSensitivity)
            {
                capturePoints.Add(new Point(mouseX, mouseY));
            }
            if (GeometryHelper.Distance2D(lastMouse.X, lastMouse.Y, mouseX, mouseY) >= EditorConfig.FreehandSensitivity)
            {
                //path.AddCurve(new Point[]{lastMouse, new Point(mouseX, mouseY)});
                freehandPath.AddLine(lastMouse, new Point(mouseX, mouseY));
                lastMouse = new Point(mouseX, mouseY);
                // Only re-calculate the bounds & redraw when we added something to the path
                myBounds = Rectangle.Round(freehandPath.GetBounds());
                Invalidate();
            }
            return true;
        }

        /// <summary>
        /// Called when the surface finishes drawing the element
        /// </summary>
        public override void HandleMouseUp(int mouseX, int mouseY)
        {
            // Make sure we don't loose the ending point
            if (GeometryHelper.Distance2D(lastMouse.X, lastMouse.Y, mouseX, mouseY) >= EditorConfig.FreehandSensitivity)
            {
                capturePoints.Add(new Point(mouseX, mouseY));
            }
            RecalculatePath();
        }

        /// <summary>
        /// Here we recalculate the freehand path by smoothing out the lines with Beziers.
        /// </summary>
        private void RecalculatePath()
        {
            isRecalculated = true;
            // Dispose the previous path, if we have one
            if (freehandPath != null)
            {
                freehandPath.Dispose();
            }
            freehandPath = new GraphicsPath();

            // Here we can put some cleanup... like losing all the uninteresting  points.
            if (capturePoints.Count >= 3)
            {
                int index = 0;
                while ((capturePoints.Count - 1) % 3 != 0)
                {
                    // duplicate points, first at 50% than 25% than 75%
                    capturePoints.Insert((int)(capturePoints.Count * POINT_OFFSET[index]), capturePoints[(int)(capturePoints.Count * POINT_OFFSET[index++])]);
                }
                freehandPath.AddBeziers(capturePoints.ToArray());
            }
            else if (capturePoints.Count == 2)
            {
                freehandPath.AddLine(capturePoints[0], capturePoints[1]);
            }

            // Recalculate the bounds
            myBounds = Rectangle.Round(freehandPath.GetBounds());
        }

        /// <summary>
        /// Do the drawing of the freehand "stroke"
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="renderMode"></param>
        public override void Draw(Graphics graphics, RenderMode renderMode)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);
            Color lineColor = GetFieldValueAsColor(FieldType.LINE_COLOR);
            using (Pen pen = new Pen(lineColor))
            {
                pen.Width = lineThickness;
                if (pen.Width > 0)
                {
                    // Make sure the lines are nicely rounded
                    pen.EndCap = LineCap.Round;
                    pen.StartCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;

                    // Move to where we need to draw
                    graphics.TranslateTransform(Left, Top);
                    if (isRecalculated && Selected && renderMode == RenderMode.EDIT)
                    {
                        DrawSelectionBorder(graphics, pen);
                    }
                    graphics.DrawPath(pen, freehandPath);
                    // Move back, otherwise everything is shifted
                    graphics.TranslateTransform(-Left, -Top);
                }
            }
        }

        /// <summary>
        /// Draw a selectionborder around the freehand path
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="linePen"></param>
        protected void DrawSelectionBorder(Graphics graphics, Pen linePen)
        {
            using (Pen selectionPen = (Pen)linePen.Clone())
            {
                using (GraphicsPath selectionPath = (GraphicsPath)freehandPath.Clone())
                {
                    selectionPen.Width += 5;
                    selectionPen.Color = Color.FromArgb(120, Color.LightSeaGreen);
                    graphics.DrawPath(selectionPen, selectionPath);
                    selectionPath.Widen(selectionPen);
                    selectionPen.DashPattern = new float[] { 2, 2 };
                    selectionPen.Color = Color.LightSeaGreen;
                    selectionPen.Width = 1;
                    graphics.DrawPath(selectionPen, selectionPath);
                }
            }
        }

        /// <summary>
        /// Get the bounds in which we have something drawn, plus safety margin, these are not the normal bounds...
        /// </summary>
        public override Rectangle DrawingBounds
        {
            get
            {
                if (!myBounds.IsEmpty)
                {
                    int lineThickness = Math.Max(10, GetFieldValueAsInt(FieldType.LINE_THICKNESS));
                    int safetymargin = 10;
                    return new Rectangle(myBounds.Left + Left - (safetymargin + lineThickness), myBounds.Top + Top - (safetymargin + lineThickness), myBounds.Width + 2 * (lineThickness + safetymargin), myBounds.Height + 2 * (lineThickness + safetymargin));
                }
                return new Rectangle(0, 0, _parent.Width, _parent.Height);
            }
        }

        /// <summary>
        /// FreehandContainer are regarded equal if they are of the same type and their paths are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            bool ret = false;
            if (obj != null && GetType().Equals(obj.GetType()))
            {
                FreehandContainer other = obj as FreehandContainer;
                if (freehandPath.Equals(other.freehandPath))
                {
                    ret = true;
                }
            }
            return ret;
        }

        public override int GetHashCode()
        {
            return freehandPath.GetHashCode();
        }

        /// <summary>
        /// This is overriden to prevent the grippers to be modified.
        /// Might not be the best way...
        /// </summary>
        protected override void DoLayout()
        {
        }

        public override void ShowGrippers()
        {
            ResumeLayout();
        }

        public override bool ClickableAt(int x, int y)
        {
            bool returnValue = base.ClickableAt(x, y);
            if (returnValue)
            {
                int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);
                using (Pen pen = new Pen(Color.White))
                {
                    pen.Width = lineThickness + 10;
                    returnValue = freehandPath.IsOutlineVisible(x - Left, y - Top, pen);
                }
            }
            return returnValue;
        }
    }
}