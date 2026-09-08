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

namespace ShareX.ImageEditor.Core.Annotations;

/// <summary>
/// Ellipse/circle annotation
/// </summary>
public partial class EllipseAnnotation : Annotation
{
    public override AnnotationCategory Category => AnnotationCategory.Shapes;
    public BorderStyle BorderStyle { get; set; } = BorderStyle.Solid;

    public EllipseAnnotation()
    {
        ToolType = EditorTool.Ellipse;
    }

    public override bool HitTest(SKPoint point, float tolerance = 5)
    {
        var rect = GetBounds();

        point = GetUnrotatedPoint(point, rect);

        var expanded = SKRect.Inflate(rect, tolerance, tolerance);

        if (!expanded.Contains(point)) return false;

        // Get ellipse center and radii (original bounds, not expanded)
        var centerX = rect.MidX;
        var centerY = rect.MidY;
        var radiusX = rect.Width / 2 + tolerance;
        var radiusY = rect.Height / 2 + tolerance;

        if (radiusX <= 0 || radiusY <= 0) return false;

        // Normalize point relative to center
        var dx = point.X - centerX;
        var dy = point.Y - centerY;

        // Check if point is inside the ellipse (with tolerance)
        // Point is inside if (dx/rx)^2 + (dy/ry)^2 <= 1
        var normalizedDist = (dx * dx) / (radiusX * radiusX) + (dy * dy) / (radiusY * radiusY);
        return normalizedDist <= 1.0f;
    }
}