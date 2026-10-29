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

namespace ShareX.PhotoEditor.Core;

/// <summary>A path anchor with absolute in/out bezier handles. A handle equal to the point means no handle.</summary>
public sealed class PathAnchor
{
    public PathAnchor(SKPoint point)
    {
        Point = point;
        In = point;
        Out = point;
    }

    public PathAnchor(SKPoint point, SKPoint inHandle, SKPoint outHandle, bool smooth)
    {
        Point = point;
        In = inHandle;
        Out = outHandle;
        Smooth = smooth;
    }

    public SKPoint Point { get; set; }

    public SKPoint In { get; set; }

    public SKPoint Out { get; set; }

    /// <summary>Smooth anchors keep their handles collinear when one handle moves.</summary>
    public bool Smooth { get; set; }

    public bool HasIn => In != Point;

    public bool HasOut => Out != Point;

    public PathAnchor Clone() => new(Point, In, Out, Smooth);

    public void Offset(SKPoint delta)
    {
        Point += delta;
        In += delta;
        Out += delta;
    }

    public void Transform(SKMatrix matrix)
    {
        Point = matrix.MapPoint(Point);
        In = matrix.MapPoint(In);
        Out = matrix.MapPoint(Out);
    }
}

public sealed class SubPath
{
    public List<PathAnchor> Anchors { get; } = [];

    public bool Closed { get; set; }

    public SubPath Clone()
    {
        SubPath copy = new() { Closed = Closed };
        copy.Anchors.AddRange(Anchors.Select(a => a.Clone()));
        return copy;
    }

    public int SegmentCount => Anchors.Count < 2 ? 0 : Closed ? Anchors.Count : Anchors.Count - 1;

    public (SKPoint P0, SKPoint C0, SKPoint C1, SKPoint P1) GetSegment(int index)
    {
        PathAnchor a = Anchors[index];
        PathAnchor b = Anchors[(index + 1) % Anchors.Count];
        return (a.Point, a.Out, b.In, b.Point);
    }
}

/// <summary>Editable bezier path used by pen tools, shape layers and saved paths.</summary>
public sealed class VectorPath
{
    public List<SubPath> SubPaths { get; } = [];

    public SKPathFillType FillType { get; set; } = SKPathFillType.Winding;

    public bool IsEmpty => SubPaths.All(s => s.Anchors.Count == 0);

    public VectorPath Clone()
    {
        VectorPath copy = new() { FillType = FillType };
        copy.SubPaths.AddRange(SubPaths.Select(s => s.Clone()));
        return copy;
    }

    public IEnumerable<PathAnchor> AllAnchors => SubPaths.SelectMany(s => s.Anchors);

    public SKPath ToSKPath()
    {
        SKPath path = new() { FillType = FillType };
        foreach (SubPath sub in SubPaths)
        {
            if (sub.Anchors.Count == 0) continue;
            path.MoveTo(sub.Anchors[0].Point);
            for (int i = 0; i < sub.SegmentCount; i++)
            {
                var (_, c0, c1, p1) = sub.GetSegment(i);
                PathAnchor a = sub.Anchors[i];
                PathAnchor b = sub.Anchors[(i + 1) % sub.Anchors.Count];
                if (!a.HasOut && !b.HasIn) path.LineTo(p1);
                else path.CubicTo(c0, c1, p1);
            }
            if (sub.Closed) path.Close();
        }
        return path;
    }

    public static VectorPath FromSKPath(SKPath source)
    {
        VectorPath result = new() { FillType = source.FillType };
        using SKPath.RawIterator iterator = source.CreateRawIterator();
        SKPoint[] pts = new SKPoint[4];
        SubPath? current = null;
        SKPathVerb verb;
        while ((verb = iterator.Next(pts)) != SKPathVerb.Done)
        {
            switch (verb)
            {
                case SKPathVerb.Move:
                    current = new SubPath();
                    result.SubPaths.Add(current);
                    current.Anchors.Add(new PathAnchor(pts[0]));
                    break;
                case SKPathVerb.Line:
                    current ??= StartSub(result, pts[0]);
                    current.Anchors.Add(new PathAnchor(pts[1]));
                    break;
                case SKPathVerb.Quad:
                {
                    current ??= StartSub(result, pts[0]);
                    SKPoint c0 = pts[0] + new SKPoint((pts[1].X - pts[0].X) * 2 / 3f, (pts[1].Y - pts[0].Y) * 2 / 3f);
                    SKPoint c1 = pts[2] + new SKPoint((pts[1].X - pts[2].X) * 2 / 3f, (pts[1].Y - pts[2].Y) * 2 / 3f);
                    current.Anchors[^1].Out = c0;
                    current.Anchors.Add(new PathAnchor(pts[2], c1, pts[2], false));
                    break;
                }
                case SKPathVerb.Conic:
                {
                    current ??= StartSub(result, pts[0]);
                    float w = iterator.ConicWeight();
                    float k = 4f / 3f * w / (1 + w);
                    SKPoint c0 = pts[0] + new SKPoint((pts[1].X - pts[0].X) * k, (pts[1].Y - pts[0].Y) * k);
                    SKPoint c1 = pts[2] + new SKPoint((pts[1].X - pts[2].X) * k, (pts[1].Y - pts[2].Y) * k);
                    current.Anchors[^1].Out = c0;
                    current.Anchors.Add(new PathAnchor(pts[2], c1, pts[2], false));
                    break;
                }
                case SKPathVerb.Cubic:
                    current ??= StartSub(result, pts[0]);
                    current.Anchors[^1].Out = pts[1];
                    current.Anchors.Add(new PathAnchor(pts[3], pts[2], pts[3], false));
                    break;
                case SKPathVerb.Close:
                    if (current != null)
                    {
                        current.Closed = true;
                        // Merge a duplicated closing anchor into the first anchor.
                        if (current.Anchors.Count > 1 && SKPoint.Distance(current.Anchors[0].Point, current.Anchors[^1].Point) < 0.01f)
                        {
                            current.Anchors[0].In = current.Anchors[^1].In;
                            current.Anchors.RemoveAt(current.Anchors.Count - 1);
                        }
                    }
                    current = null;
                    break;
            }
        }
        result.SubPaths.RemoveAll(s => s.Anchors.Count == 0);
        return result;
    }

    private static SubPath StartSub(VectorPath path, SKPoint start)
    {
        SubPath sub = new();
        sub.Anchors.Add(new PathAnchor(start));
        path.SubPaths.Add(sub);
        return sub;
    }

    public void Transform(SKMatrix matrix)
    {
        foreach (PathAnchor anchor in AllAnchors) anchor.Transform(matrix);
    }

    public void Offset(SKPoint delta)
    {
        foreach (PathAnchor anchor in AllAnchors) anchor.Offset(delta);
    }

    public SKRect GetBounds()
    {
        using SKPath path = ToSKPath();
        return path.TightBounds;
    }

    public static SKPoint EvalCubic(SKPoint p0, SKPoint c0, SKPoint c1, SKPoint p1, float t)
    {
        float u = 1 - t;
        float a = u * u * u, b = 3 * u * u * t, c = 3 * u * t * t, d = t * t * t;
        return new SKPoint(a * p0.X + b * c0.X + c * c1.X + d * p1.X, a * p0.Y + b * c0.Y + c * c1.Y + d * p1.Y);
    }

    /// <summary>Finds the anchor nearest to a point within a tolerance.</summary>
    public (SubPath Sub, int Index)? HitAnchor(SKPoint point, float tolerance)
    {
        (SubPath, int)? best = null;
        float bestDistance = tolerance;
        foreach (SubPath sub in SubPaths)
        {
            for (int i = 0; i < sub.Anchors.Count; i++)
            {
                float d = SKPoint.Distance(sub.Anchors[i].Point, point);
                if (d <= bestDistance)
                {
                    bestDistance = d;
                    best = (sub, i);
                }
            }
        }
        return best;
    }

    /// <summary>Finds the segment nearest to a point and returns the bezier parameter of the closest location.</summary>
    public (SubPath Sub, int Segment, float T)? HitSegment(SKPoint point, float tolerance)
    {
        (SubPath, int, float)? best = null;
        float bestDistance = tolerance;
        foreach (SubPath sub in SubPaths)
        {
            for (int s = 0; s < sub.SegmentCount; s++)
            {
                var (p0, c0, c1, p1) = sub.GetSegment(s);
                const int steps = 48;
                for (int i = 0; i <= steps; i++)
                {
                    float t = i / (float)steps;
                    float d = SKPoint.Distance(EvalCubic(p0, c0, c1, p1, t), point);
                    if (d <= bestDistance)
                    {
                        bestDistance = d;
                        best = (sub, s, t);
                    }
                }
            }
        }
        return best;
    }

    public SubPath? HitSubPath(SKPoint point, float tolerance)
    {
        var segment = HitSegment(point, tolerance);
        if (segment != null) return segment.Value.Sub;
        foreach (SubPath sub in SubPaths)
        {
            if (!sub.Closed) continue;
            VectorPath single = new();
            single.SubPaths.Add(sub);
            using SKPath path = single.ToSKPath();
            if (path.Contains(point.X, point.Y)) return sub;
        }
        return null;
    }

    /// <summary>Inserts an anchor on a segment using de Casteljau subdivision so the curve shape is preserved.</summary>
    public PathAnchor InsertAnchor(SubPath sub, int segment, float t)
    {
        PathAnchor a = sub.Anchors[segment];
        int nextIndex = (segment + 1) % sub.Anchors.Count;
        PathAnchor b = sub.Anchors[nextIndex];
        SKPoint p0 = a.Point, c0 = a.Out, c1 = b.In, p1 = b.Point;
        SKPoint Lerp(SKPoint x, SKPoint y) => new(x.X + (y.X - x.X) * t, x.Y + (y.Y - x.Y) * t);
        SKPoint q0 = Lerp(p0, c0), q1 = Lerp(c0, c1), q2 = Lerp(c1, p1);
        SKPoint r0 = Lerp(q0, q1), r1 = Lerp(q1, q2);
        SKPoint m = Lerp(r0, r1);
        bool curved = a.HasOut || b.HasIn;
        PathAnchor inserted = curved ? new PathAnchor(m, r0, r1, true) : new PathAnchor(m);
        if (curved)
        {
            a.Out = q0;
            b.In = q2;
        }
        sub.Anchors.Insert(segment + 1, inserted);
        return inserted;
    }

    /// <summary>Converts free-hand points into a smooth path using Catmull-Rom splines after simplification.</summary>
    public static SubPath FromFreehand(IReadOnlyList<SKPoint> points, float tolerance, bool closed)
    {
        List<SKPoint> simplified = Simplify(points, tolerance);
        return FromCurvaturePoints(simplified, closed);
    }

    /// <summary>Builds a smooth curve passing through every point (used by the curvature pen).</summary>
    public static SubPath FromCurvaturePoints(IReadOnlyList<SKPoint> points, bool closed)
    {
        SubPath sub = new() { Closed = closed };
        int n = points.Count;
        for (int i = 0; i < n; i++)
        {
            SKPoint p = points[i];
            if (n < 3)
            {
                sub.Anchors.Add(new PathAnchor(p));
                continue;
            }
            SKPoint prev = i > 0 ? points[i - 1] : closed ? points[n - 1] : p;
            SKPoint next = i < n - 1 ? points[i + 1] : closed ? points[0] : p;
            SKPoint tangent = new((next.X - prev.X) / 6f, (next.Y - prev.Y) / 6f);
            bool endpoint = !closed && (i == 0 || i == n - 1);
            sub.Anchors.Add(endpoint ? new PathAnchor(p, i == 0 ? p : p - tangent, i == 0 ? p + tangent : p, false)
                : new PathAnchor(p, p - tangent, p + tangent, true));
        }
        return sub;
    }

    /// <summary>Ramer-Douglas-Peucker polyline simplification.</summary>
    public static List<SKPoint> Simplify(IReadOnlyList<SKPoint> points, float tolerance)
    {
        if (points.Count < 3) return points.ToList();
        bool[] keep = new bool[points.Count];
        keep[0] = keep[^1] = true;
        Stack<(int, int)> stack = new();
        stack.Push((0, points.Count - 1));
        while (stack.Count > 0)
        {
            var (start, end) = stack.Pop();
            float maxDistance = 0;
            int index = -1;
            for (int i = start + 1; i < end; i++)
            {
                float d = DistanceToSegment(points[i], points[start], points[end]);
                if (d > maxDistance) { maxDistance = d; index = i; }
            }
            if (index >= 0 && maxDistance > tolerance)
            {
                keep[index] = true;
                stack.Push((start, index));
                stack.Push((index, end));
            }
        }
        List<SKPoint> result = [];
        for (int i = 0; i < points.Count; i++) if (keep[i]) result.Add(points[i]);
        return result;
    }

    public static float DistanceToSegment(SKPoint p, SKPoint a, SKPoint b)
    {
        SKPoint ab = b - a;
        float lengthSquared = ab.X * ab.X + ab.Y * ab.Y;
        if (lengthSquared < 1e-9f) return SKPoint.Distance(p, a);
        float t = Math.Clamp(((p.X - a.X) * ab.X + (p.Y - a.Y) * ab.Y) / lengthSquared, 0, 1);
        return SKPoint.Distance(p, new SKPoint(a.X + ab.X * t, a.Y + ab.Y * t));
    }
}

public sealed class NamedPath
{
    public NamedPath(string name, VectorPath path)
    {
        Name = name;
        Path = path;
    }

    public string Name { get; set; }

    public VectorPath Path { get; set; }

    public NamedPath Clone() => new(Name, Path.Clone());
}
