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

using System.Globalization;
using Avalonia;
using Avalonia.Media;

namespace ShareX.Icons;

/// <summary>Geometries for icon shapes, in the coordinates of the icon's grid.</summary>
internal static class IconGeometry
{
    /// <summary>A geometry from SVG path data.</summary>
    public static Geometry Path(string data) => Geometry.Parse(data);

    /// <summary>A geometry from SVG path data with computed numbers, which are written the same in every culture.</summary>
    public static Geometry FormattedPath(FormattableString data) => Path(data.ToString(CultureInfo.InvariantCulture));

    public static Geometry Circle(double centerX, double centerY, double radius) =>
        Ellipse(centerX - radius, centerY - radius, radius * 2, radius * 2);

    public static Geometry Ellipse(double x, double y, double width, double height) =>
        new EllipseGeometry(new Rect(x, y, width, height));

    public static Geometry Rectangle(double x, double y, double width, double height, double radius = 0) =>
        new RectangleGeometry(new Rect(x, y, width, height), radius, radius);

    /// <summary>A four-pointed star with curved sides, such as a sparkle.</summary>
    public static Geometry Sparkle(double centerX, double centerY, double radius)
    {
        double k = radius * 0.18;
        double left = centerX - radius, top = centerY - radius, right = centerX + radius, bottom = centerY + radius;
        return FormattedPath($"M{centerX} {top}Q{centerX + k} {centerY - k} {right} {centerY}Q{centerX + k} {centerY + k} {centerX} {bottom}Q{centerX - k} {centerY + k} {left} {centerY}Q{centerX - k} {centerY - k} {centerX} {top}z");
    }

    /// <summary>A star with <paramref name="points"/> points, the first one straight up.</summary>
    public static Geometry Star(double centerX, double centerY, double outerRadius, double innerRadius, int points = 5)
    {
        List<string> corners = [];

        for (int i = 0; i < points * 2; i++)
        {
            double radius = i % 2 == 0 ? outerRadius : innerRadius;
            double angle = Math.PI * i / points - Math.PI / 2;
            corners.Add(string.Create(CultureInfo.InvariantCulture, $"{centerX + radius * Math.Cos(angle):0.##} {centerY + radius * Math.Sin(angle):0.##}"));
        }

        return Path("M" + string.Join("L", corners) + "z");
    }

    /// <summary>The outline around all of <paramref name="geometries"/>, such as a cloud made of circles.</summary>
    public static Geometry Union(params Geometry[] geometries) =>
        geometries.Aggregate((combined, next) => new CombinedGeometry(GeometryCombineMode.Union, combined, next));

    /// <summary><paramref name="geometry"/> with the area of <paramref name="cut"/> removed.</summary>
    public static Geometry Subtract(Geometry geometry, Geometry cut) =>
        new CombinedGeometry(GeometryCombineMode.Exclude, geometry, cut);

    /// <summary>Turns <paramref name="geometry"/> clockwise by <paramref name="degrees"/> around a point.</summary>
    public static Geometry Rotate(Geometry geometry, double degrees, double centerX, double centerY)
    {
        geometry.Transform = new RotateTransform(degrees, centerX, centerY);
        return geometry;
    }

    /// <summary>
    /// The area within <paramref name="width"/> / 2 of the line <paramref name="pathData"/>. As the cut of
    /// <see cref="IconShape.Except"/>, it leaves a gap around a line or badge drawn over other shapes.
    /// </summary>
    public static Geometry Halo(string pathData, double width) => Halo(Path(pathData), width);

    /// <inheritdoc cref="Halo(string, double)"/>
    public static Geometry Halo(Geometry geometry, double width) =>
        geometry.GetWidenedGeometry(new Pen(Brushes.Black, width, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round));
}
