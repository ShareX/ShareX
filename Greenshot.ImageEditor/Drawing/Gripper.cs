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

using System.Drawing;
using System.Windows.Forms;

namespace Greenshot.Drawing
{
    /// <summary>
    /// Description of Gripper.
    /// </summary>
    public class Gripper : Label
    {
        /// <summary>
        /// Constants for anchor/gripper position:
        /// 0 1 2
        /// 7   3
        /// 6 5 4
        /// </summary>
        public const int POSITION_TOP_LEFT = 0;
        public const int POSITION_TOP_CENTER = 1;
        public const int POSITION_TOP_RIGHT = 2;
        public const int POSITION_MIDDLE_RIGHT = 3;
        public const int POSITION_BOTTOM_RIGHT = 4;
        public const int POSITION_BOTTOM_CENTER = 5;
        public const int POSITION_BOTTOM_LEFT = 6;
        public const int POSITION_MIDDLE_LEFT = 7;
        public const int GripperSize = 7;

        public int Position;

        public Gripper()
        {
            Width = GripperSize;
            Height = GripperSize;
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
        }

        public bool IsTop()
        {
            return Position == 0 || Position == 1 || Position == 2;
        }

        public bool IsRight()
        {
            return Position == 2 || Position == 3 || Position == 4;
        }

        public bool IsBottom()
        {
            return Position == 4 || Position == 5 || Position == 6;
        }

        public bool IsLeft()
        {
            return Position == 6 || Position == 7 || Position == 0;
        }

        public bool IsCorner()
        {
            return Position == 0 || Position == 2 || Position == 4 || Position == 6;
        }
    }
}