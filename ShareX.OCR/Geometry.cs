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

namespace ShareX.OCR;

internal static class Geometry
{
    // Andrew's monotone chain. Returns the hull in counter-clockwise order (in y-up terms) without repeating the first point.
    public static List<SKPoint> ConvexHull(List<SKPoint> points)
    {
        if (points.Count < 3)
        {
            return [.. points];
        }

        points.Sort((a, b) => a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y));
        SKPoint[] hull = new SKPoint[points.Count * 2];
        int count = 0;

        for (int i = 0; i < points.Count; i++)
        {
            while (count >= 2 && Cross(hull[count - 2], hull[count - 1], points[i]) <= 0)
            {
                count--;
            }

            hull[count++] = points[i];
        }

        for (int i = points.Count - 2, lowerCount = count + 1; i >= 0; i--)
        {
            while (count >= lowerCount && Cross(hull[count - 2], hull[count - 1], points[i]) <= 0)
            {
                count--;
            }

            hull[count++] = points[i];
        }

        return [.. hull.AsSpan(0, count - 1)];
    }

    // The smallest rotated rectangle around a convex hull, found by trying each hull edge as a side (rotating calipers).
    // Returns the four corners and the length of the shorter side.
    public static (SKPoint[] Box, float ShortSide) MinAreaRect(List<SKPoint> hull)
    {
        if (hull.Count == 0)
        {
            return (new SKPoint[4], 0);
        }

        if (hull.Count == 1)
        {
            return ([hull[0], hull[0], hull[0], hull[0]], 0);
        }

        float bestArea = float.MaxValue;
        SKPoint bestAxis = new(1, 0);
        float bestMinU = 0, bestMaxU = 0, bestMinV = 0, bestMaxV = 0;

        for (int i = 0; i < hull.Count; i++)
        {
            SKPoint edge = hull[(i + 1) % hull.Count] - hull[i];
            float length = edge.Length;

            if (length < 1e-6f)
            {
                continue;
            }

            SKPoint axis = new(edge.X / length, edge.Y / length);
            float minU = float.MaxValue, maxU = float.MinValue, minV = float.MaxValue, maxV = float.MinValue;

            foreach (SKPoint point in hull)
            {
                float u = (point.X * axis.X) + (point.Y * axis.Y);
                float v = (point.Y * axis.X) - (point.X * axis.Y);
                minU = Math.Min(minU, u);
                maxU = Math.Max(maxU, u);
                minV = Math.Min(minV, v);
                maxV = Math.Max(maxV, v);
            }

            float area = (maxU - minU) * (maxV - minV);

            if (area < bestArea)
            {
                bestArea = area;
                bestAxis = axis;
                bestMinU = minU;
                bestMaxU = maxU;
                bestMinV = minV;
                bestMaxV = maxV;
            }
        }

        SKPoint Corner(float u, float v) => new((u * bestAxis.X) - (v * bestAxis.Y), (u * bestAxis.Y) + (v * bestAxis.X));

        SKPoint[] box =
        [
            Corner(bestMinU, bestMinV),
            Corner(bestMaxU, bestMinV),
            Corner(bestMaxU, bestMaxV),
            Corner(bestMinU, bestMaxV)
        ];

        return (OrderBox(box), Math.Min(bestMaxU - bestMinU, bestMaxV - bestMinV));
    }

    // Orders the corners of a rectangle as top-left, top-right, bottom-right, bottom-left, the same way PaddleOCR does:
    // the two leftmost points are the left side and the higher of each pair is the top.
    public static SKPoint[] OrderBox(SKPoint[] box)
    {
        SKPoint[] sorted = [.. box.OrderBy(point => point.X)];
        (SKPoint topLeft, SKPoint bottomLeft) = sorted[0].Y <= sorted[1].Y ? (sorted[0], sorted[1]) : (sorted[1], sorted[0]);
        (SKPoint topRight, SKPoint bottomRight) = sorted[2].Y <= sorted[3].Y ? (sorted[2], sorted[3]) : (sorted[3], sorted[2]);
        return [topLeft, topRight, bottomRight, bottomLeft];
    }

    // Grows a rectangle outwards by the distance PaddleOCR's unclip uses (area * ratio / perimeter). Offsetting a
    // rectangle with rounded corners and taking the smallest rectangle around the result gives the same rectangle
    // grown by that distance on every side.
    public static (SKPoint[] Box, float ShortSide) Unclip(SKPoint[] box, float unclipRatio)
    {
        SKPoint center = new((box[0].X + box[1].X + box[2].X + box[3].X) / 4, (box[0].Y + box[1].Y + box[2].Y + box[3].Y) / 4);
        SKPoint widthVector = box[1] - box[0];
        SKPoint heightVector = box[3] - box[0];
        float width = widthVector.Length;
        float height = heightVector.Length;
        float perimeter = 2 * (width + height);

        if (perimeter < 1e-6f || width < 1e-6f || height < 1e-6f)
        {
            return (box, 0);
        }

        float distance = width * height * unclipRatio / perimeter;
        SKPoint widthAxis = new(widthVector.X / width, widthVector.Y / width);
        SKPoint heightAxis = new(heightVector.X / height, heightVector.Y / height);
        float halfWidth = (width / 2) + distance;
        float halfHeight = (height / 2) + distance;

        SKPoint Corner(float u, float v) => new(center.X + (widthAxis.X * u) + (heightAxis.X * v), center.Y + (widthAxis.Y * u) + (heightAxis.Y * v));

        SKPoint[] result =
        [
            Corner(-halfWidth, -halfHeight),
            Corner(halfWidth, -halfHeight),
            Corner(halfWidth, halfHeight),
            Corner(-halfWidth, halfHeight)
        ];

        return (OrderBox(result), Math.Min(halfWidth, halfHeight) * 2);
    }

    public static SKRect GetBounds(SKPoint[] points)
    {
        float left = float.MaxValue, top = float.MaxValue, right = float.MinValue, bottom = float.MinValue;

        foreach (SKPoint point in points)
        {
            left = Math.Min(left, point.X);
            top = Math.Min(top, point.Y);
            right = Math.Max(right, point.X);
            bottom = Math.Max(bottom, point.Y);
        }

        return new SKRect(left, top, right, bottom);
    }

    private static float Cross(SKPoint origin, SKPoint a, SKPoint b)
    {
        return ((a.X - origin.X) * (b.Y - origin.Y)) - ((a.Y - origin.Y) * (b.X - origin.X));
    }
}
