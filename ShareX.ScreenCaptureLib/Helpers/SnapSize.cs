#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    public sealed class SnapSize
    {
        private const int MINIMUM_WIDTH = 2;
        public int Width { get; }

        private const int MINIMUM_HEIGHT = 2;
        public int Height { get; }

        private static int CoerceWidth (int desired) => Math.Max(desired, MINIMUM_WIDTH);
        private static int CoerceHeight(int desired) => Math.Max(desired, MINIMUM_HEIGHT);

        public SnapSize(int width, int height)
        {
            Width = CoerceWidth(width);
            Height = CoerceHeight(height);
        }

        public static implicit operator Size(SnapSize size)
        {
            return new Size(size.Width, size.Height);
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}