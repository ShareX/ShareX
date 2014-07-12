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

using Greenshot.Plugin;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Greenshot.Drawing.Fields.Binding
{
    /// <summary>
    /// Converting horizontal alignment to its StringAlignment representation and vice versa.
    /// Beware: there's currently no RTL support.
    /// </summary>
    public class HorizontalAlignmentConverter : AbstractBindingConverter<HorizontalAlignment, StringAlignment>
    {
        private static HorizontalAlignmentConverter uniqueInstance;

        protected override HorizontalAlignment convert(StringAlignment stringAlignment)
        {
            switch (stringAlignment)
            {
                case StringAlignment.Near: return HorizontalAlignment.Left;
                case StringAlignment.Center: return HorizontalAlignment.Center;
                case StringAlignment.Far: return HorizontalAlignment.Right;
                default: throw new NotImplementedException("Cannot handle: " + stringAlignment);
            }
        }

        protected override StringAlignment convert(HorizontalAlignment horizontalAligment)
        {
            switch (horizontalAligment)
            {
                case HorizontalAlignment.Left: return StringAlignment.Near;
                case HorizontalAlignment.Center: return StringAlignment.Center;
                case HorizontalAlignment.Right: return StringAlignment.Far;
                default: throw new NotImplementedException("Cannot handle: " + horizontalAligment);
            }
        }

        public static HorizontalAlignmentConverter GetInstance()
        {
            if (uniqueInstance == null) uniqueInstance = new HorizontalAlignmentConverter();
            return uniqueInstance;
        }
    }

    /// <summary>
    /// Converting vertical alignment to its StringAlignment representation and vice versa.
    /// </summary>
    public class VerticalAlignmentConverter : AbstractBindingConverter<VerticalAlignment, StringAlignment>
    {
        private static VerticalAlignmentConverter uniqueInstance;

        protected override VerticalAlignment convert(StringAlignment stringAlignment)
        {
            switch (stringAlignment)
            {
                case StringAlignment.Near: return VerticalAlignment.TOP;
                case StringAlignment.Center: return VerticalAlignment.CENTER;
                case StringAlignment.Far: return VerticalAlignment.BOTTOM;
                default: throw new NotImplementedException("Cannot handle: " + stringAlignment);
            }
        }

        protected override StringAlignment convert(VerticalAlignment verticalAligment)
        {
            switch (verticalAligment)
            {
                case VerticalAlignment.TOP: return StringAlignment.Near;
                case VerticalAlignment.CENTER: return StringAlignment.Center;
                case VerticalAlignment.BOTTOM: return StringAlignment.Far;
                default: throw new NotImplementedException("Cannot handle: " + verticalAligment);
            }
        }

        public static VerticalAlignmentConverter GetInstance()
        {
            if (uniqueInstance == null) uniqueInstance = new VerticalAlignmentConverter();
            return uniqueInstance;
        }
    }
}