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

namespace Greenshot.Drawing
{
    /// <summary>
    /// empty container for filter-only elements
    /// </summary>
    [Serializable()]
    public abstract class FilterContainer : DrawableContainer
    {
        public enum PreparedFilterMode { OBFUSCATE, HIGHLIGHT };
        public enum PreparedFilter { BLUR, PIXELIZE, TEXT_HIGHTLIGHT, AREA_HIGHLIGHT, GRAYSCALE, MAGNIFICATION };

        public PreparedFilter Filter
        {
            get { return (PreparedFilter)GetFieldValue(FieldType.PREPARED_FILTER_HIGHLIGHT); }
        }

        public FilterContainer(Surface parent)
            : base(parent)
        {
            AddField(GetType(), FieldType.LINE_THICKNESS, 0);
            AddField(GetType(), FieldType.LINE_COLOR, Color.Red);
            AddField(GetType(), FieldType.SHADOW, false);
        }

        public override void Draw(Graphics graphics, RenderMode rm)
        {
            int lineThickness = GetFieldValueAsInt(FieldType.LINE_THICKNESS);
            Color lineColor = GetFieldValueAsColor(FieldType.LINE_COLOR);
            bool shadow = GetFieldValueAsBool(FieldType.SHADOW);
            bool lineVisible = (lineThickness > 0 && Colors.IsVisible(lineColor));
            if (lineVisible)
            {
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.None;
                //draw shadow first
                if (shadow)
                {
                    int basealpha = 100;
                    int alpha = basealpha;
                    int steps = 5;
                    int currentStep = lineVisible ? 1 : 0;
                    while (currentStep <= steps)
                    {
                        using (Pen shadowPen = new Pen(Color.FromArgb(alpha, 100, 100, 100), lineThickness))
                        {
                            Rectangle shadowRect = GuiRectangle.GetGuiRectangle(Left + currentStep, Top + currentStep, Width, Height);
                            graphics.DrawRectangle(shadowPen, shadowRect);
                            currentStep++;
                            alpha = alpha - (basealpha / steps);
                        }
                    }
                }
                Rectangle rect = GuiRectangle.GetGuiRectangle(Left, Top, Width, Height);
                if (lineThickness > 0)
                {
                    using (Pen pen = new Pen(lineColor, lineThickness))
                    {
                        graphics.DrawRectangle(pen, rect);
                    }
                }
            }
        }
    }
}