#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using SkiaSharp;

namespace ShareX.HelpersLib;

/// <summary>A color transform stored in the row-vector form used by existing image effect presets.</summary>
public sealed class ImageColorMatrix
{
    private readonly float[][] values;
    public ImageColorMatrix() : this(new[]
    {
        new float[] { 1, 0, 0, 0, 0 }, new float[] { 0, 1, 0, 0, 0 },
        new float[] { 0, 0, 1, 0, 0 }, new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
    })
    { }
    public ImageColorMatrix(float[][] values) => this.values = values;
    public float Matrix33 { get => values[3][3]; set => values[3][3] = value; }
    public float Matrix00 { get => values[0][0]; set => values[0][0] = value; }
    public float Matrix01 { get => values[0][1]; set => values[0][1] = value; }
    public float Matrix02 { get => values[0][2]; set => values[0][2] = value; }
    public float Matrix03 { get => values[0][3]; set => values[0][3] = value; }
    public float Matrix04 { get => values[0][4]; set => values[0][4] = value; }
    public float Matrix10 { get => values[1][0]; set => values[1][0] = value; }
    public float Matrix11 { get => values[1][1]; set => values[1][1] = value; }
    public float Matrix12 { get => values[1][2]; set => values[1][2] = value; }
    public float Matrix13 { get => values[1][3]; set => values[1][3] = value; }
    public float Matrix14 { get => values[1][4]; set => values[1][4] = value; }
    public float Matrix20 { get => values[2][0]; set => values[2][0] = value; }
    public float Matrix21 { get => values[2][1]; set => values[2][1] = value; }
    public float Matrix22 { get => values[2][2]; set => values[2][2] = value; }
    public float Matrix23 { get => values[2][3]; set => values[2][3] = value; }
    public float Matrix24 { get => values[2][4]; set => values[2][4] = value; }
    public float Matrix30 { get => values[3][0]; set => values[3][0] = value; }
    public float Matrix31 { get => values[3][1]; set => values[3][1] = value; }
    public float Matrix32 { get => values[3][2]; set => values[3][2] = value; }
    public float Matrix34 { get => values[3][4]; set => values[3][4] = value; }
    public float Matrix40 { get => values[4][0]; set => values[4][0] = value; }
    public float Matrix41 { get => values[4][1]; set => values[4][1] = value; }
    public float Matrix42 { get => values[4][2]; set => values[4][2] = value; }
    public float Matrix43 { get => values[4][3]; set => values[4][3] = value; }
    public float Matrix44 { get => values[4][4]; set => values[4][4] = value; }
    // Skia 3.119 uses normalized translation offsets, as well as a column-vector matrix.
    public SKColorFilter CreateFilter() => SKColorFilter.CreateColorMatrix(new[]
    {
        values[0][0], values[1][0], values[2][0], values[3][0], values[4][0],
        values[0][1], values[1][1], values[2][1], values[3][1], values[4][1],
        values[0][2], values[1][2], values[2][2], values[3][2], values[4][2],
        values[0][3], values[1][3], values[2][3], values[3][3], values[4][3]
    });
}
