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
/// Number annotation - auto-incrementing numbered circle markers
/// </summary>
public partial class NumberAnnotation : Annotation
{
    private const float GeometryEpsilon = 0.001f;
    private const float TextPaddingMultiplier = 0.35f;

    public override AnnotationCategory Category => AnnotationCategory.Text;
    /// <summary>
    /// Number to display (typically auto-incremented)
    /// </summary>
    public int Number { get; set; } = 1;

    /// <summary>
    /// Display style for the step label.
    /// </summary>
    public StepType StepType { get; set; } = StepType.Numeric;

    /// <summary>
    /// Font size for the number
    /// </summary>
    public float FontSize { get; set; } = 24;

    /// <summary>
    /// Text body color
    /// </summary>
    public string TextColor { get; set; } = "#FFFFFFFF";

    /// <summary>
    /// Bold style for the step label.
    /// </summary>
    public bool IsBold { get; set; } = true;

    /// <summary>
    /// Circle radius - auto-calculated based on FontSize if not explicitly set
    /// </summary>
    public float Radius
    {
        get => CalculateRadius();
        set { } // Allow setting but use calculated value
    }

    /// <summary>
    /// Tail point (absolute position). The tail is only rendered after this point is explicitly set.
    /// </summary>
    public SKPoint TailPoint { get; set; }

    /// <summary>
    /// Tracks whether the tail point was explicitly initialized.
    /// </summary>
    public bool TailPointInitialized { get; set; }

    /// <summary>
    /// Controls whether the tail geometry and handle are shown.
    /// </summary>
    public bool TailEnabled { get; set; } = true;

    /// <summary>
    /// Calculate radius based on font size to ensure text fits
    /// </summary>
    private float CalculateRadius()
    {
        float baseRadius = Math.Max(12, FontSize * 0.7f);
        string displayText = GetDisplayText();

        using var font = new SKFont(SKTypeface.Default, Math.Max(1, FontSize * 0.6f))
        {
            Embolden = IsBold
        };

        float textWidth = font.MeasureText(displayText);
        SKFontMetrics metrics = font.Metrics;
        float textHeight = metrics.Descent - metrics.Ascent;
        float padding = Math.Max(6, FontSize * TextPaddingMultiplier);
        float measuredRadius = Math.Max(textWidth, textHeight) * 0.5f + padding;

        return Math.Max(baseRadius, measuredRadius);
    }

    public NumberAnnotation()
    {
        ToolType = EditorTool.Step;
    }

    [JsonIgnore]
    public bool HasTailPoint => TailPointInitialized;

    [JsonIgnore]
    public string DisplayText => GetDisplayText();

    public string GetDisplayText() => StepTypeFormatter.Format(Number, StepType);

    public SKPoint GetDefaultTailHandlePoint()
    {
        var bounds = GetBounds();
        return new SKPoint(bounds.Right, bounds.Bottom);
    }

    public SKPoint GetTailHandlePoint() => HasTailPoint ? TailPoint : GetDefaultTailHandlePoint();

    public void SetTailPoint(SKPoint tailPoint)
    {
        TailPoint = tailPoint;
        TailPointInitialized = true;
    }

    public bool IsTailVisible()
    {
        if (!TailEnabled)
        {
            return false;
        }

        return TryGetTailPolygon(out _, out _, out _);
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

        if (!TailEnabled || !HasTailPoint)
        {
            return false;
        }

        var center = StartPoint;
        float radius = Radius;
        if (radius <= GeometryEpsilon)
        {
            return false;
        }

        tailTip = TailPoint;

        float directionX = tailTip.X - center.X;
        float directionY = tailTip.Y - center.Y;
        float directionLength = MathF.Sqrt(directionX * directionX + directionY * directionY);
        if (directionLength <= radius + GeometryEpsilon)
        {
            return false;
        }

        float normalizedDirectionX = directionX / directionLength;
        float normalizedDirectionY = directionY / directionLength;

        var perpendicular = new SKPoint(-normalizedDirectionY, normalizedDirectionX);
        float projectionDistance = (radius * radius) / directionLength;
        float offsetDistance = radius * MathF.Sqrt((directionLength * directionLength) - (radius * radius)) / directionLength;
        var tangentCenter = new SKPoint(
            center.X + normalizedDirectionX * projectionDistance,
            center.Y + normalizedDirectionY * projectionDistance);

        tailBaseStart = new SKPoint(
            tangentCenter.X + perpendicular.X * offsetDistance,
            tangentCenter.Y + perpendicular.Y * offsetDistance);
        tailBaseEnd = new SKPoint(
            tangentCenter.X - perpendicular.X * offsetDistance,
            tangentCenter.Y - perpendicular.Y * offsetDistance);

        return true;
    }

    public override bool HitTest(SKPoint point, float tolerance = 5)
    {
        var dx = point.X - StartPoint.X;
        var dy = point.Y - StartPoint.Y;
        var distance = (float)Math.Sqrt(dx * dx + dy * dy);
        if (distance <= (Radius + tolerance))
        {
            return true;
        }

        if (!TryGetTailPolygon(out var tailBaseStart, out var tailTip, out var tailBaseEnd))
        {
            return false;
        }

        return TailGeometryHelper.HitTest(point, tailBaseStart, tailTip, tailBaseEnd, tolerance);
    }

    public override SKRect GetBounds()
    {
        float radius = Radius;
        return new SKRect(
            StartPoint.X - radius,
            StartPoint.Y - radius,
            StartPoint.X + radius,
            StartPoint.Y + radius);
    }

    internal override void TransformAdditionalPoints(Func<SKPoint, SKPoint> transformPoint)
    {
        if (HasTailPoint)
        {
            SetTailPoint(transformPoint(TailPoint));
        }
    }

    internal override void MoveBy(float deltaX, float deltaY)
    {
        base.MoveBy(deltaX, deltaY);
        TransformAdditionalPoints(point => new SKPoint(point.X + deltaX, point.Y + deltaY));
    }
}
