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

using System.Drawing;

namespace Greenshot.Helpers
{
    /// <summary>
    /// Helper class for creating rectangles with positive dimensions, regardless of input coordinates
    /// </summary>
    public static class GuiRectangle
    {
        public static Rectangle GetGuiRectangle(int x, int y, int w, int h)
        {
            Rectangle rect = new Rectangle(x, y, w, h);
            MakeGuiRectangle(ref rect);
            return rect;
        }

        public static void MakeGuiRectangle(ref Rectangle rect)
        {
            if (rect.Width < 0)
            {
                rect.X += rect.Width;
                rect.Width = -rect.Width;
            }
            if (rect.Height < 0)
            {
                rect.Y += rect.Height;
                rect.Height = -rect.Height;
            }
        }

        public static RectangleF GetGuiRectangleF(float x, float y, float w, float h)
        {
            RectangleF rect = new RectangleF(x, y, w, h);
            MakeGuiRectangleF(ref rect);
            return rect;
        }

        public static void MakeGuiRectangleF(ref RectangleF rect)
        {
            if (rect.Width < 0)
            {
                rect.X += rect.Width;
                rect.Width = -rect.Width;
            }
            if (rect.Height < 0)
            {
                rect.Y += rect.Height;
                rect.Height = -rect.Height;
            }
        }
    }
}