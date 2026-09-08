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
/// Freehand pen/drawing annotation
/// ISSUE-013 fix: Implements IPointBasedAnnotation for unified polyline handling.
/// </summary>
public partial class FreehandAnnotation : Annotation, IPointBasedAnnotation
{
    public override AnnotationCategory Category => AnnotationCategory.Shapes;
    public List<SKPoint> Points { get; set; } = new List<SKPoint>();
    public BorderStyle BorderStyle { get; set; } = BorderStyle.Solid;

    /// <summary>
    /// Simplification tolerance for smoothing
    /// </summary>
    public float SmoothingTolerance { get; set; } = 2.0f;

    public FreehandAnnotation()
    {
        ToolType = EditorTool.Freehand;
    }

    public override Annotation Clone()
    {
        var clone = (FreehandAnnotation)base.Clone();
        clone.Points = new List<SKPoint>(Points); // Deep copy the points list
        return clone;
    }

    public override bool HitTest(SKPoint point, float tolerance = 5)
    {
        // Simple bounding box check first
        var bounds = GetBounds();
        var inflatedBounds = SKRect.Inflate(bounds, tolerance, tolerance);
        if (!inflatedBounds.Contains(point)) return false;

        // Detailed segment check
        for (int i = 0; i < Points.Count - 1; i++)
        {
            if (DistanceToSegment(point, Points[i], Points[i + 1]) <= tolerance)
                return true;
        }

        return false;
    }

    public override SKRect GetBounds()
    {
        if (Points.Count == 0) return SKRect.Empty;

        float minX = Points.Min(p => p.X);
        float minY = Points.Min(p => p.Y);
        float maxX = Points.Max(p => p.X);
        float maxY = Points.Max(p => p.Y);

        return new SKRect(minX, minY, maxX, maxY);
    }

    private float DistanceToSegment(SKPoint p, SKPoint v, SKPoint w)
    {
        float l2 = (v.X - w.X) * (v.X - w.X) + (v.Y - w.Y) * (v.Y - w.Y);
        if (l2 == 0) return Distance(p, v);

        float t = ((p.X - v.X) * (w.X - v.X) + (p.Y - v.Y) * (w.Y - v.Y)) / l2;
        t = Math.Max(0, Math.Min(1, t));

        var projection = new SKPoint(v.X + t * (w.X - v.X), v.Y + t * (w.Y - v.Y));
        return Distance(p, projection);
    }

    private float Distance(SKPoint p1, SKPoint p2)
    {
        return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }

    internal override void TransformAdditionalPoints(Func<SKPoint, SKPoint> transformPoint)
    {
        for (int i = 0; i < Points.Count; i++)
        {
            Points[i] = transformPoint(Points[i]);
        }
    }

    internal override void MoveBy(float deltaX, float deltaY)
    {
        // A freehand selection is positioned by its path points, not its drawing endpoints.
        TransformAdditionalPoints(point => new SKPoint(point.X + deltaX, point.Y + deltaY));
    }
}