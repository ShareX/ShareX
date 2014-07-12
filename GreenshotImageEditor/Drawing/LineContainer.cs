/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Greenshot.Drawing
{
    /// <summary>
    /// Description of LineContainer.
    /// </summary>
    [Serializable()]
    public class LineContainer : DrawableContainer
    {
        public static readonly int MAX_CLICK_DISTANCE_TOLERANCE = 10;

        public LineContainer(Surface parent)
            : base(parent)
        {
            Init();
            AddField(GetType(), FieldType.LINE_THICKNESS, 2);
            AddField(GetType(), FieldType.LINE_COLOR, Color.Red);
            AddField(GetType(), FieldType.SHADOW, true);
        }

        [OnDeserialized()]
        private void OnDeserialized(StreamingContext context)
        {
            InitGrippers();
            DoLayout();
            Init();
        }

        protected void Init()
        {
            if (grippers != null)
            {
                foreach (int index in new int[] { 1, 2, 3, 5, 6, 7 })
                {
                    grippers[index].Enabled = false;
                }
            }
        }

        public override void Draw(Graphics graphics, RenderMode rm)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.None;

            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);
            Color lineColor = GetFieldValueAsColor(FieldType.LINE_COLOR);
            bool shadow = GetFieldValueAsBool(FieldType.SHADOW);

            if (lineThickness > 0)
            {
                if (shadow)
                {
                    //draw shadow first
                    int basealpha = 100;
                    int alpha = basealpha;
                    int steps = 5;
                    int currentStep = 1;
                    while (currentStep <= steps)
                    {
                        using (Pen shadowCapPen = new Pen(Color.FromArgb(alpha, 100, 100, 100), lineThickness))
                        {
                            graphics.DrawLine(shadowCapPen,
                                Left + currentStep,
                                Top + currentStep,
                                Left + currentStep + Width,
                                Top + currentStep + Height);

                            currentStep++;
                            alpha = alpha - (basealpha / steps);
                        }
                    }
                }

                using (Pen pen = new Pen(lineColor, lineThickness))
                {
                    graphics.DrawLine(pen, Left, Top, Left + Width, Top + Height);
                }
            }
        }

        public override bool ClickableAt(int x, int y)
        {
            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS) + 5;
            if (lineThickness > 0)
            {
                using (Pen pen = new Pen(Color.White))
                {
                    pen.Width = lineThickness;
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddLine(Left, Top, Left + Width, Top + Height);
                        return path.IsOutlineVisible(x, y, pen);
                    }
                }
            }
            else
            {
                return false;
            }
        }

        protected override ScaleHelper.IDoubleProcessor GetAngleRoundProcessor()
        {
            return ScaleHelper.LineAngleRoundBehavior.Instance;
        }
    }
}