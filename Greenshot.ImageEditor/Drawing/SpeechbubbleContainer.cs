/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2012  Thomas Braun, Jens Klingen, Robin Krom
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
using Greenshot.Plugin;
using Greenshot.Plugin.Drawing;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Greenshot.Drawing
{
    /// <summary>
    /// Description of SpeechbubbleContainer.
    /// </summary>
    [Serializable]
    public class SpeechbubbleContainer : TextContainer
    {
        private Point _initialGripperPoint;

        #region TargetGripper serializing code

        // Only used for serializing the TargetGripper location
        private Point _storedTargetGripperLocation;

        /// <summary>
        /// Store the current location of the target gripper
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing]
        private void SetValuesOnSerializing(StreamingContext context)
        {
            if (TargetGripper != null)
            {
                _storedTargetGripperLocation = TargetGripper.Location;
            }
        }

        /// <summary>
        /// Restore the target gripper
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void SetValuesOnDeserialized(StreamingContext context)
        {
            InitTargetGripper(Color.Yellow, _storedTargetGripperLocation);
        }

        #endregion TargetGripper serializing code

        public SpeechbubbleContainer(Surface parent)
            : base(parent)
        {
        }

        /// <summary>
        /// We set our own field values
        /// </summary>
        protected override void InitializeFields()
        {
            AddField(GetType(), FieldType.LINE_THICKNESS, 2);
            AddField(GetType(), FieldType.LINE_COLOR, DefaultLineColor);
            AddField(GetType(), FieldType.SHADOW, false);
            AddField(GetType(), FieldType.FONT_ITALIC, false);
            AddField(GetType(), FieldType.FONT_BOLD, false);
            AddField(GetType(), FieldType.FILL_COLOR, Color.White);
            AddField(GetType(), FieldType.FONT_FAMILY, FontFamily.GenericSansSerif.Name);
            AddField(GetType(), FieldType.FONT_SIZE, 20f);
            AddField(GetType(), FieldType.TEXT_HORIZONTAL_ALIGNMENT, StringAlignment.Center);
            AddField(GetType(), FieldType.TEXT_VERTICAL_ALIGNMENT, StringAlignment.CENTER);
        }

        /// <summary>
        /// Called from Surface (the _parent) when the drawing begins (mouse-down)
        /// </summary>
        /// <returns>true if the surface doesn't need to handle the event</returns>
        public override bool HandleMouseDown(int mouseX, int mouseY)
        {
            if (TargetGripper == null)
            {
                _initialGripperPoint = new Point(mouseX, mouseY);
                InitTargetGripper(Color.Yellow, new Point(mouseX, mouseY));
            }
            return base.HandleMouseDown(mouseX, mouseY);
        }

        /// <summary>
        /// Overriding the HandleMouseMove will help us to make sure the tail is always visible.
        /// Should fix BUG-1682
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>base.HandleMouseMove</returns>
        public override bool HandleMouseMove(int x, int y)
        {
            bool returnValue = base.HandleMouseMove(x, y);

            bool leftAligned = _boundsAfterResize.Right - _boundsAfterResize.Left >= 0;
            bool topAligned = _boundsAfterResize.Bottom - _boundsAfterResize.Top >= 0;

            int xOffset = leftAligned ? -20 : 20;
            int yOffset = topAligned ? -20 : 20;

            Point newGripperLocation = _initialGripperPoint;
            newGripperLocation.Offset(xOffset, yOffset);

            if (TargetGripper.Location != newGripperLocation)
            {
                Invalidate();
                TargetGripperMove(newGripperLocation.X, newGripperLocation.Y);
                Invalidate();
            }
            return returnValue;
        }

        /// <summary>
        /// The DrawingBound should be so close as possible to the shape, so we don't invalidate to much.
        /// </summary>
        public override Rectangle DrawingBounds
        {
            get
            {
                if (Status != EditStatus.UNDRAWN)
                {
                    int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);
                    Color lineColor = GetFieldValueAsColor(FieldType.LINE_COLOR);
                    using (Pen pen = new Pen(lineColor, lineThickness))
                    {
                        using (GraphicsPath tailPath = CreateTail())
                        {
                            return Rectangle.Inflate(Rectangle.Union(Rectangle.Round(tailPath.GetBounds(new Matrix(), pen)), GuiRectangle.GetGuiRectangle(Left, Top, Width, Height)), lineThickness + 2, lineThickness + 2);
                        }
                    }
                }
                return Rectangle.Empty;
            }
        }

        /// <summary>
        /// Helper method to create the bubble GraphicsPath, so we can also calculate the bounds
        /// </summary>
        /// <param name="lineThickness"></param>
        /// <returns></returns>
        private GraphicsPath CreateBubble(int lineThickness)
        {
            GraphicsPath bubble = new GraphicsPath();
            Rectangle rect = GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);

            Rectangle bubbleRect = GuiRectangle.GetGuiRectangle(0, 0, rect.Width, rect.Height);
            // adapt corner radius to small rectangle dimensions
            int smallerSideLength = Math.Min(bubbleRect.Width, bubbleRect.Height);
            int cornerRadius = Math.Min(30, smallerSideLength / 2 - lineThickness);
            if (cornerRadius > 0)
            {
                bubble.AddArc(bubbleRect.X, bubbleRect.Y, cornerRadius, cornerRadius, 180, 90);
                bubble.AddArc(bubbleRect.X + bubbleRect.Width - cornerRadius, bubbleRect.Y, cornerRadius, cornerRadius, 270, 90);
                bubble.AddArc(bubbleRect.X + bubbleRect.Width - cornerRadius, bubbleRect.Y + bubbleRect.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                bubble.AddArc(bubbleRect.X, bubbleRect.Y + bubbleRect.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            }
            else
            {
                bubble.AddRectangle(bubbleRect);
            }
            bubble.CloseAllFigures();
            using (Matrix bubbleMatrix = new Matrix())
            {
                bubbleMatrix.Translate(rect.Left, rect.Top);
                bubble.Transform(bubbleMatrix);
            }
            return bubble;
        }

        /// <summary>
        /// Helper method to create the tail of the bubble, so we can also calculate the bounds
        /// </summary>
        /// <returns></returns>
        private GraphicsPath CreateTail()
        {
            Rectangle rect = GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);

            int tailLength = GeometryHelper.Distance2D(rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2), TargetGripper.Left, TargetGripper.Top);
            int tailWidth = (Math.Abs(rect.Width) + Math.Abs(rect.Height)) / 20;

            // This should fix a problem with the tail being to wide
            tailWidth = Math.Min(Math.Abs(rect.Width) / 2, tailWidth);
            tailWidth = Math.Min(Math.Abs(rect.Height) / 2, tailWidth);

            GraphicsPath tail = new GraphicsPath();
            tail.AddLine(-tailWidth, 0, tailWidth, 0);
            tail.AddLine(tailWidth, 0, 0, -tailLength);
            tail.CloseFigure();

            int tailAngle = 90 + (int)GeometryHelper.Angle2D(rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2), TargetGripper.Left, TargetGripper.Top);

            using (Matrix tailMatrix = new Matrix())
            {
                tailMatrix.Translate(rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2));
                tailMatrix.Rotate(tailAngle);
                tail.Transform(tailMatrix);
            }

            return tail;
        }

        /// <summary>
        /// This is to draw the actual container
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="renderMode"></param>
        public override void Draw(Graphics graphics, RenderMode renderMode)
        {
            if (TargetGripper == null)
            {
                return;
            }
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.None;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            Color lineColor = GetFieldValueAsColor(FieldType.LINE_COLOR);
            Color fillColor = GetFieldValueAsColor(FieldType.FILL_COLOR);
            bool shadow = GetFieldValueAsBool(FieldType.SHADOW);
            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);

            bool lineVisible = (lineThickness > 0 && Colors.IsVisible(lineColor));
            Rectangle rect = GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);

            if (Selected && renderMode == RenderMode.EDIT)
            {
                DrawSelectionBorder(graphics, rect);
            }

            GraphicsPath bubble = CreateBubble(lineThickness);

            GraphicsPath tail = CreateTail();

            //draw shadow first
            if (shadow && (lineVisible || Colors.IsVisible(fillColor)))
            {
                const int basealpha = 100;
                int alpha = basealpha;
                const int steps = 5;
                int currentStep = lineVisible ? 1 : 0;
                using (Matrix shadowMatrix = new Matrix())
                using (GraphicsPath bubbleClone = (GraphicsPath)bubble.Clone())
                using (GraphicsPath tailClone = (GraphicsPath)tail.Clone())
                {
                    shadowMatrix.Translate(1, 1);
                    while (currentStep <= steps)
                    {
                        using (Pen shadowPen = new Pen(Color.FromArgb(alpha, 100, 100, 100)))
                        {
                            shadowPen.Width = lineVisible ? lineThickness : 1;
                            tailClone.Transform(shadowMatrix);
                            graphics.DrawPath(shadowPen, tailClone);
                            bubbleClone.Transform(shadowMatrix);
                            graphics.DrawPath(shadowPen, bubbleClone);
                        }
                        currentStep++;
                        alpha = alpha - (basealpha / steps);
                    }
                }
            }

            GraphicsState state = graphics.Save();
            // draw the tail border where the bubble is not visible
            using (Region clipRegion = new Region(bubble))
            {
                graphics.SetClip(clipRegion, CombineMode.Exclude);
                using (Pen pen = new Pen(lineColor, lineThickness))
                {
                    graphics.DrawPath(pen, tail);
                }
            }
            graphics.Restore(state);

            if (Colors.IsVisible(fillColor))
            {
                //draw the bubbleshape
                state = graphics.Save();
                using (Brush brush = new SolidBrush(fillColor))
                {
                    graphics.FillPath(brush, bubble);
                }
                graphics.Restore(state);
            }

            if (lineVisible)
            {
                //draw the bubble border
                state = graphics.Save();
                // Draw bubble where the Tail is not visible.
                using (Region clipRegion = new Region(tail))
                {
                    graphics.SetClip(clipRegion, CombineMode.Exclude);
                    using (Pen pen = new Pen(lineColor, lineThickness))
                    {
                        //pen.EndCap = pen.StartCap = LineCap.Round;
                        graphics.DrawPath(pen, bubble);
                    }
                }
                graphics.Restore(state);
            }

            if (Colors.IsVisible(fillColor))
            {
                // Draw the tail border
                state = graphics.Save();
                using (Brush brush = new SolidBrush(fillColor))
                {
                    graphics.FillPath(brush, tail);
                }
                graphics.Restore(state);
            }

            // cleanup the paths
            bubble.Dispose();
            tail.Dispose();

            // Draw the text
            UpdateFormat();
            DrawText(graphics, rect, lineThickness, lineColor, shadow, StringFormat, Text, Font);
        }

        public override bool Contains(int x, int y)
        {
            double xDistanceFromCenter = x - (Left + Width / 2);
            double yDistanceFromCenter = y - (Top + Height / 2);
            // ellipse: x^2/a^2 + y^2/b^2 = 1
            return Math.Pow(xDistanceFromCenter, 2) / Math.Pow(Width / 2, 2) + Math.Pow(yDistanceFromCenter, 2) / Math.Pow(Height / 2, 2) < 1;
        }

        public override bool ClickableAt(int x, int y)
        {
            Rectangle rect = GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);
            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);
            int lineThicknessPlusSafety = lineThickness + 10;

            // If we clicked inside the rectangle and it's visible we are clickable at.
            Color fillColor = GetFieldValueAsColor(FieldType.FILL_COLOR);
            if (!Color.Transparent.Equals(fillColor))
            {
                if (Contains(x, y))
                {
                    return true;
                }
            }

            // check the rest of the lines
            if (lineThicknessPlusSafety > 0)
            {
                using (Pen pen = new Pen(Color.White, lineThicknessPlusSafety))
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddEllipse(rect);
                        return path.IsOutlineVisible(x, y, pen);
                    }
                }
            }
            return false;
        }
    }
}