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
    /// Tracks whether the user has manually placed/dragged the tail handle at least once.
    /// Once placed, the tail remains fixed in canvas coordinates during body moves, resizes, and rotations.
    /// </summary>
    public bool TailPointPlaced { get; set; }

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

    public void SetPlacedTailPoint(SKPoint tailPoint)
    {
        SetTailPoint(tailPoint);
        TailPointPlaced = true;
    }

    internal SKPoint GetWorldTailPoint()
    {
        return RotateAroundBoundsCenter(GetEffectiveTailPoint(), GetBounds(), RotationAngle);
    }

    internal void SetTailPointFromWorld(SKPoint worldTailPoint)
    {
        SKPoint localTailPoint = RotateAroundBoundsCenter(worldTailPoint, GetBounds(), -RotationAngle);
        SetPlacedTailPoint(localTailPoint);
    }

    private static SKPoint RotateAroundBoundsCenter(SKPoint point, SKRect bounds, float angleDeg)
    {
        if (angleDeg == 0)
        {
            return point;
        }

        float cx = bounds.MidX;
        float cy = bounds.MidY;
        float rad = angleDeg * MathF.PI / 180f;
        float cos = MathF.Cos(rad);
        float sin = MathF.Sin(rad);
        float dx = point.X - cx;
        float dy = point.Y - cy;
        return new SKPoint(
            cx + (dx * cos) - (dy * sin),
            cy + (dx * sin) + (dy * cos));
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
            SKRect tailBounds = TailGeometryHelper.GetBounds(tailBaseStart, tailTip, tailBaseEnd);
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

        return TailGeometryHelper.HitTest(point, tailBaseStart, tailTip, tailBaseEnd, tolerance);
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

    internal override void TransformAdditionalPoints(Func<SKPoint, SKPoint> transformPoint)
    {
        SKPoint transformed = transformPoint(GetEffectiveTailPoint());
        if (TailPointPlaced)
        {
            SetPlacedTailPoint(transformed);
        }
        else if (HasTailPoint)
        {
            SetTailPoint(transformed);
        }
    }

    internal void ResizeTail(SKRect previousBounds, SKRect newBounds)
    {
        if (!TailPointPlaced)
        {
            if (HasTailPoint)
            {
                SetTailPoint(GetDefaultTailPoint());
            }

            return;
        }

        if (RotationAngle == 0)
        {
            return;
        }

        SKPoint worldTailPoint = RotateAroundBoundsCenter(GetEffectiveTailPoint(), previousBounds, RotationAngle);
        SetTailPointFromWorld(worldTailPoint);
    }

    internal override void MoveBy(float deltaX, float deltaY)
    {
        if (!TailPointPlaced)
        {
            bool hadTailPoint = HasTailPoint;
            base.MoveBy(deltaX, deltaY);
            if (hadTailPoint)
            {
                SetTailPoint(GetDefaultTailPoint());
            }

            return;
        }

        SKPoint? worldTailPoint = RotationAngle != 0 ? GetWorldTailPoint() : null;
        base.MoveBy(deltaX, deltaY);

        if (worldTailPoint.HasValue)
        {
            SetTailPointFromWorld(worldTailPoint.Value);
        }
    }
}