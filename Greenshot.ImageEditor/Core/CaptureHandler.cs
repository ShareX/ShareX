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

namespace GreenshotPlugin.Core
{
    /// <summary>
    /// This is the method signature which is used to capture a rectangle from the screen.
    /// </summary>
    /// <param name="captureBounds"></param>
    /// <returns>Captured Bitmap</returns>
    public delegate Bitmap CaptureScreenRectangleHandler(Rectangle captureBounds);

    /// <summary>
    /// This is a hack to experiment with different screen capture routines
    /// </summary>
    public static class CaptureHandler
    {
        /// <summary>
        /// By changing this value, null is default
        /// </summary>
        public static CaptureScreenRectangleHandler CaptureScreenRectangle
        {
            get;
            set;
        }
    }
}