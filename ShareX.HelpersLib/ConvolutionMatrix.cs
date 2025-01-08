#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

namespace ShareX.HelpersLib
{
    public class ConvolutionMatrix
    {
        private readonly double[,] matrix;

        public int Width => matrix.GetLength(1);
        public int Height => matrix.GetLength(0);
        public byte Offset { get; set; }

        public bool ConsiderAlpha { get; set; }

        public ConvolutionMatrix() : this(3)
        {
        }

        public ConvolutionMatrix(int size) : this(size, size)
        {
        }

        public ConvolutionMatrix(int height, int width)
        {
            matrix = new double[height, width];
        }

        public void SetAll(double value)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    matrix[y, x] = value;
                }
            }
        }

        public ref double this[int y, int x] => ref matrix[y, x];
    }
}