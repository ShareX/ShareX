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
using System.Text.Json.Serialization;

namespace ShareX.ImageEditor.Core.Annotations;

/// <summary>
/// Speech Balloon annotation with tail
/// </summary>
public partial class SpeechBalloonAnnotation : Annotation
{
    public const float DefaultTailOffset = 30f;
    public const float TailWidthMultiplier = 0.3f;
    private const float GeometryEpsilon = 0.001f;

    public override AnnotationCategory Category => AnnotationCategory.Text;
    /// <summary>
    /// Tail point (absolute position)
    /// </summary>
    public SKPoint TailPoint { get; set; }

    /// <summary>
    /// Tracks whether the tail point was explicitly initialized.
    /// This preserves intentional 0,0 tail positions while still allowing
    /// legacy/uninitialized balloons to fall back to the default tail placement.
    /// </summary>
    public bool TailPointInitialized { get; set; }

    /// <summary>
    /// Controls whether the tail geometry and handle are shown.
    /// </summary>
    public bool TailEnabled { get; set; } = true;

    /// <summary>
    /// Optional text content inside the balloon
    /// </summary>
    public string Text { get; set; } = "";

    /// <summary>
    /// Font size for the balloon text
    /// </summary>
    public float FontSize { get; set; } = 20;

    /// <summary>
    /// Font family for the balloon text
    /// </summary>
    public string FontFamily { get; set; } = "Segoe UI";

    /// <summary>
    /// Text body color
    /// </summary>
    public string TextColor { get; set; } = "#FF000000";

    /// <summary>
    /// Bold style for balloon text.
    /// </summary>
    public bool IsBold { get; set; }

    /// <summary>
    /// Italic style for balloon text.
    /// </summary>
    public bool IsItalic { get; set; }

    /// <summary>
    /// Horizontal alignment for text within the balloon body.
    /// </summary>
    public TextHorizontalAlignment HorizontalAlignment { get; set; } = TextHorizontalAlignment.Center;

    /// <summary>
    /// Corner radius for the balloon body.
    /// </summary>
    public int CornerRadius { get; set; } = 10;

    /// <summary>
    /// Background color (hex) - defaults to white for speech balloon
    /// </summary>
    public SpeechBalloonAnnotation()
    {
        ToolType = EditorTool.SpeechBalloon;
        StrokeWidth = 2;
        StrokeColor = "#FF000000";
        FillColor = "#FFFFFFFF"; // Default to white
    }

    [JsonIgnore]
    public bool HasTailPoint => TailPointInitialized || TailPoint != default;

    public SKPoint GetDefaultTailPoint()
    {
        var bounds = GetBounds();
        return new SKPoint(bounds.MidX, bounds.Bottom + DefaultTailOffset);
    }

    public SKPoint GetEffectiveTailPoint() => HasTailPoint ? TailPoint : GetDefaultTailPoint();

    public void SetTailPoint(SKPoint tailPoint)
    {
        TailPoint = tailPoint;
        TailPointInitialized = true;
    }

    public void EnsureTailPointInitialized()
    {
        if (!HasTailPoint)
        {
            SetTailPoint(GetDefaultTailPoint());
        }
        else if (!TailPointInitialized && TailPoint != default)
        {
            TailPointInitialized = true;
        }
    }

    public bool IsTailVisible()
    {
        if (!TailEnabled)
        {
            return false;
        }

        var bounds = GetBounds();
        var tailPoint = GetEffectiveTailPoint();
        return !bounds.Contains(tailPoint.X, tailPoint.Y);
    }

    public SKRect GetInteractionBounds(float tolerance = 0)
    {
        var interactionBounds = GetBounds();

        if (TryGetTailPolygon(out var tailBaseStart, out var tailTip, out var tailBaseEnd))
        {
            var tailBounds = new SKRect(
                MathF.Min(tailBaseStart.X, MathF.Min(tailTip.X, tailBaseEnd.X)),
                MathF.Min(tailBaseStart.Y, MathF.Min(tailTip.Y, tailBaseEnd.Y)),
                MathF.Max(tailBaseStart.X, MathF.Max(tailTip.X, tailBaseEnd.X)),
                MathF.Max(tailBaseStart.Y, MathF.Max(tailTip.Y, tailBaseEnd.Y)));
            interactionBounds = SKRect.Union(interactionBounds, tailBounds);
        }

        if (tolerance > 0)
        {
            interactionBounds = SKRect.Inflate(interactionBounds, tolerance, tolerance);
        }

        return interactionBounds;
    }

    public bool TryGetTailPolygon(out SKPoint tailBaseStart, out SKPoint tailTip, out SKPoint tailBaseEnd)
    {
        tailBaseStart = default;
        tailTip = default;
        tailBaseEnd = default;

        if (!TailEnabled)
        {
            return false;
        }

        var bounds = GetBounds();
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return false;
        }

        tailTip = GetEffectiveTailPoint();
        if (bounds.Contains(tailTip.X, tailTip.Y))
        {
            return false;
        }

        var center = new SKPoint(bounds.MidX, bounds.MidY);
        float directionX = tailTip.X - center.X;
        float directionY = tailTip.Y - center.Y;
        float directionLength = MathF.Sqrt(directionX * directionX + directionY * directionY);
        if (directionLength <= GeometryEpsilon)
        {
            return false;
        }

        float normalizedDirectionX = directionX / directionLength;
        float normalizedDirectionY = directionY / directionLength;

        float rectAverageSize = (bounds.Width + bounds.Height) / 2f;
        float tailWidth = TailWidthMultiplier * rectAverageSize;
        tailWidth = MathF.Min(tailWidth, MathF.Min(bounds.Width, bounds.Height));
        if (tailWidth <= GeometryEpsilon)
        {
            return false;
        }

        float halfTailWidth = tailWidth / 2f;
        var perpendicular = new SKPoint(-normalizedDirectionY, normalizedDirectionX);
        var baseStart = new SKPoint(
            center.X + perpendicular.X * halfTailWidth,
            center.Y + perpendicular.Y * halfTailWidth);
        var baseEnd = new SKPoint(
            center.X - perpendicular.X * halfTailWidth,
            center.Y - perpendicular.Y * halfTailWidth);

        return TryGetSegmentExitPoint(bounds, baseStart, tailTip, out tailBaseStart) &&
               TryGetSegmentExitPoint(bounds, baseEnd, tailTip, out tailBaseEnd);
    }

    public override SKRect GetBounds()
    {
        return new SKRect(
            Math.Min(StartPoint.X, EndPoint.X),
            Math.Min(StartPoint.Y, EndPoint.Y),
            Math.Max(StartPoint.X, EndPoint.X),
            Math.Max(StartPoint.Y, EndPoint.Y));
    }

    public override bool HitTest(SKPoint point, float tolerance = 5)
    {
        var bounds = GetBounds();

        point = GetUnrotatedPoint(point, bounds);

        var bodyBounds = SKRect.Inflate(bounds, tolerance, tolerance);
        if (bodyBounds.Contains(point.X, point.Y))
        {
            return true;
        }

        if (!TryGetTailPolygon(out var tailBaseStart, out var tailTip, out var tailBaseEnd))
        {
            return false;
        }

        if (PointInTriangle(point, tailBaseStart, tailTip, tailBaseEnd))
        {
            return true;
        }

        return DistanceToSegment(point, tailBaseStart, tailTip) <= tolerance ||
               DistanceToSegment(point, tailTip, tailBaseEnd) <= tolerance ||
               DistanceToSegment(point, tailBaseEnd, tailBaseStart) <= tolerance;
    }

    private static bool TryGetSegmentExitPoint(SKRect bounds, SKPoint start, SKPoint end, out SKPoint intersection)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float bestT = float.PositiveInfinity;
        SKPoint bestIntersection = default;

        void Consider(float t)
        {
            if (t < -GeometryEpsilon || t > 1f + GeometryEpsilon || t >= bestT)
            {
                return;
            }

            float x = start.X + dx * t;
            float y = start.Y + dy * t;
            if (x < bounds.Left - GeometryEpsilon || x > bounds.Right + GeometryEpsilon ||
                y < bounds.Top - GeometryEpsilon || y > bounds.Bottom + GeometryEpsilon)
            {
                return;
            }

            bestT = Math.Clamp(t, 0f, 1f);
            bestIntersection = new SKPoint(
                Math.Clamp(x, bounds.Left, bounds.Right),
                Math.Clamp(y, bounds.Top, bounds.Bottom));
        }

        if (MathF.Abs(dx) > GeometryEpsilon)
        {
            Consider((bounds.Left - start.X) / dx);
            Consider((bounds.Right - start.X) / dx);
        }

        if (MathF.Abs(dy) > GeometryEpsilon)
        {
            Consider((bounds.Top - start.Y) / dy);
            Consider((bounds.Bottom - start.Y) / dy);
        }

        intersection = bestIntersection;
        return !float.IsPositiveInfinity(bestT);
    }

    private static bool PointInTriangle(SKPoint point, SKPoint a, SKPoint b, SKPoint c)
    {
        float d1 = Sign(point, a, b);
        float d2 = Sign(point, b, c);
        float d3 = Sign(point, c, a);

        bool hasNegative = d1 < 0 || d2 < 0 || d3 < 0;
        bool hasPositive = d1 > 0 || d2 > 0 || d3 > 0;

        return !(hasNegative && hasPositive);
    }

    private static float Sign(SKPoint p1, SKPoint p2, SKPoint p3)
    {
        return (p1.X - p3.X) * (p2.Y - p3.Y) -
               (p2.X - p3.X) * (p1.Y - p3.Y);
    }

    private static float DistanceToSegment(SKPoint point, SKPoint start, SKPoint end)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float segmentLengthSquared = dx * dx + dy * dy;

        if (segmentLengthSquared <= GeometryEpsilon)
        {
            return MathF.Sqrt((point.X - start.X) * (point.X - start.X) + (point.Y - start.Y) * (point.Y - start.Y));
        }

        float t = ((point.X - start.X) * dx + (point.Y - start.Y) * dy) / segmentLengthSquared;
        t = Math.Clamp(t, 0f, 1f);

        float projectionX = start.X + t * dx;
        float projectionY = start.Y + t * dy;
        float deltaX = point.X - projectionX;
        float deltaY = point.Y - projectionY;

        return MathF.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    internal override void TransformAdditionalPoints(Func<SKPoint, SKPoint> transformPoint)
    {
        SetTailPoint(transformPoint(GetEffectiveTailPoint()));
    }
}