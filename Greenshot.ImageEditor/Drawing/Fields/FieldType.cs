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

using System;

namespace Greenshot.Drawing.Fields
{
    /// <summary>
    /// Defines all FieldTypes + their default value.
    /// (The additional value is why this is not an enum)
    /// </summary>
    [Serializable]
    public class FieldType
    {
        public static readonly FieldType ARROWHEADS = new FieldType("ARROWHEADS");
        public static readonly FieldType BLUR_RADIUS = new FieldType("BLUR_RADIUS");
        public static readonly FieldType BRIGHTNESS = new FieldType("BRIGHTNESS");
        public static readonly FieldType FILL_COLOR = new FieldType("FILL_COLOR");
        public static readonly FieldType FONT_BOLD = new FieldType("FONT_BOLD");
        public static readonly FieldType FONT_FAMILY = new FieldType("FONT_FAMILY");
        public static readonly FieldType FONT_ITALIC = new FieldType("FONT_ITALIC");
        public static readonly FieldType FONT_SIZE = new FieldType("FONT_SIZE");
        public static readonly FieldType TEXT_HORIZONTAL_ALIGNMENT = new FieldType("TEXT_HORIZONTAL_ALIGNMENT");
        public static readonly FieldType TEXT_VERTICAL_ALIGNMENT = new FieldType("TEXT_VERTICAL_ALIGNMENT");
        public static readonly FieldType HIGHLIGHT_COLOR = new FieldType("HIGHLIGHT_COLOR");
        public static readonly FieldType LINE_COLOR = new FieldType("LINE_COLOR");
        public static readonly FieldType LINE_THICKNESS = new FieldType("LINE_THICKNESS");
        public static readonly FieldType MAGNIFICATION_FACTOR = new FieldType("MAGNIFICATION_FACTOR");
        public static readonly FieldType PIXEL_SIZE = new FieldType("PIXEL_SIZE");
        public static readonly FieldType PREVIEW_QUALITY = new FieldType("PREVIEW_QUALITY");
        public static readonly FieldType SHADOW = new FieldType("SHADOW");
        public static readonly FieldType PREPARED_FILTER_OBFUSCATE = new FieldType("PREPARED_FILTER_OBFUSCATE");
        public static readonly FieldType PREPARED_FILTER_HIGHLIGHT = new FieldType("PREPARED_FILTER_HIGHLIGHT");
        public static readonly FieldType FLAGS = new FieldType("FLAGS");

        public static FieldType[] Values = new FieldType[]{
            ARROWHEADS,
            BLUR_RADIUS,
            BRIGHTNESS,
            FILL_COLOR,
            FONT_BOLD,
            FONT_FAMILY,
            FONT_ITALIC,
            FONT_SIZE,
            TEXT_HORIZONTAL_ALIGNMENT,
            TEXT_VERTICAL_ALIGNMENT,
            HIGHLIGHT_COLOR,
            LINE_COLOR,
            LINE_THICKNESS,
            MAGNIFICATION_FACTOR,
            PIXEL_SIZE,
            PREVIEW_QUALITY,
            SHADOW,
            PREPARED_FILTER_OBFUSCATE,
            PREPARED_FILTER_HIGHLIGHT,
            FLAGS
        };

        [Flags]
        public enum Flag
        {
            NONE = 0,
            CONFIRMABLE = 1
        }

        public string Name;

        private FieldType(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            unchecked
            {
                if (Name != null)
                    hashCode += 1000000009 * Name.GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            FieldType other = obj as FieldType;
            if (other == null)
                return false;
            return Equals(Name, other.Name);
        }

        public static bool operator ==(FieldType a, FieldType b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(FieldType a, FieldType b)
        {
            return !Equals(a, b);
        }
    }
}