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

using System;

namespace Greenshot.Helpers
{
    /// <summary>
    /// Description of GeometryHelper.
    /// </summary>
    public static class GeometryHelper
    {
        /// <summary>
        /// Finds the distance between two points on a 2D surface.
        /// </summary>
        /// <param name="x1">The point on the x-axis of the first point</param>
        /// <param name="x2">The point on the x-axis of the second point</param>
        /// <param name="y1">The point on the y-axis of the first point</param>
        /// <param name="y2">The point on the y-axis of the second point</param>
        /// <returns></returns>
        public static int Distance2D(int x1, int y1, int x2, int y2)
        {
            //Our end result
            int result = 0;
            //Take x2-x1, then square it
            double part1 = Math.Pow((x2 - x1), 2);
            //Take y2-y1, then square it
            double part2 = Math.Pow((y2 - y1), 2);
            //Add both of the parts together
            double underRadical = part1 + part2;
            //Get the square root of the parts
            result = (int)Math.Sqrt(underRadical);
            //Return our result
            return result;
        }

        /// <summary>
        /// Calculates the angle of a line defined by two points on a 2D surface.
        /// </summary>
        /// <param name="x1">The point on the x-axis of the first point</param>
        /// <param name="x2">The point on the x-axis of the second point</param>
        /// <param name="y1">The point on the y-axis of the first point</param>
        /// <param name="y2">The point on the y-axis of the second point</param>
        /// <returns></returns>
        public static double Angle2D(int x1, int y1, int x2, int y2)
        {
            return Math.Atan2(y2 - y1, x2 - x1) * 180 / Math.PI;
        }
    }
}