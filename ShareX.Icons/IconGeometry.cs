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

using Avalonia;
using Avalonia.Media;

namespace ShareX.Icons;

/// <summary>Geometries for icon shapes, in the coordinates of the icon's grid.</summary>
internal static class IconGeometry
{
    /// <summary>A geometry from SVG path data.</summary>
    public static Geometry Path(string data) => Geometry.Parse(data);

    public static Geometry Circle(double centerX, double centerY, double radius) =>
        Ellipse(centerX - radius, centerY - radius, radius * 2, radius * 2);

    public static Geometry Ellipse(double x, double y, double width, double height) =>
        new EllipseGeometry(new Rect(x, y, width, height));

    public static Geometry Rectangle(double x, double y, double width, double height, double radius = 0) =>
        new RectangleGeometry(new Rect(x, y, width, height), radius, radius);
}
