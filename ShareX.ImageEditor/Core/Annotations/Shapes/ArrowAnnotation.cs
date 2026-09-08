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
/// Arrow annotation (line with arrowhead)
/// </summary>
public partial class ArrowAnnotation : Annotation, ICurvedSegmentAnnotation
{
    public override AnnotationCategory Category => AnnotationCategory.Shapes;
    /// <summary>
    /// Half-width of the classic ShareX arrow cap, proportional to stroke width.
    /// </summary>
    public const double ClassicArrowHeadWidthMultiplier = 2.0;
    public const double ModernArrowHeadWidthMultiplier = 3.0;
    public const double BasicArrowHeadWidthMultiplier = 1.75;
    public const double LineArrowHeadWidthMultiplier = 1.75;
    private const double ArrowHeadLengthRatio = 3.0;
    private const double BasicArrowHeadLengthRatio = 3.0;
    private const double LineArrowHeadLengthRatio = 3.0;
    private const double ArrowHeadBackCurveDepthRatio = 0.5;
    private const double ArrowHeadBackCurveControlRatio = 2.0;
    private ArrowStyle _style = ArrowStyle.Classic;
    public SKPoint CurvePoint { get; set; }
    public bool CurvePointActivated { get; set; }
    public ArrowStyle Style
    {
        get => _style;
        set
        {
            _style = value;

            if (_style == ArrowStyle.Modern)
            {
                CurvedSegmentHelper.ResetCurvePoint(this);
            }
        }
    }

    public ArrowAnnotation()
    {
        ToolType = EditorTool.Arrow;
    }

    public static double GetArrowHeadWidthMultiplier(ArrowStyle style)
    {
        return style switch
        {
            ArrowStyle.Line => LineArrowHeadWidthMultiplier,
            ArrowStyle.Basic => BasicArrowHeadWidthMultiplier,
            ArrowStyle.Modern => ModernArrowHeadWidthMultiplier,
            _ => ClassicArrowHeadWidthMultiplier
        };
    }

    public static ArrowCapPoints? ComputeArrowCapPoints(
        float startX, float startY,
        float endX, float endY,
        double headHalfWidth)
    {
        return ComputeArrowCapPointsFromTangent(
            endX,
            endY,
            endX - startX,
            endY - startY,
            headHalfWidth);
    }

    public static ArrowCapPoints? ComputeArrowCapPointsFromTangent(
        float tipX,
        float tipY,
        float tangentX,
        float tangentY,
        double headHalfWidth)
    {
        var tangentLength = Math.Sqrt(tangentX * tangentX + tangentY * tangentY);
        if (tangentLength <= 0)
        {
            return null;
        }

        var unitX = tangentX / tangentLength;
        var unitY = tangentY / tangentLength;
        var perpendicularX = -unitY;
        var perpendicularY = unitX;

        var headLength = headHalfWidth * ArrowHeadLengthRatio;
        var backCurveControlDistance = headHalfWidth * ArrowHeadBackCurveControlRatio;

        var baseX = tipX - headLength * unitX;
        var baseY = tipY - headLength * unitY;

        return new ArrowCapPoints(
            LeftBase: new SKPoint(
                (float)(baseX + perpendicularX * headHalfWidth),
                (float)(baseY + perpendicularY * headHalfWidth)),
            RightBase: new SKPoint(
                (float)(baseX - perpendicularX * headHalfWidth),
                (float)(baseY - perpendicularY * headHalfWidth)),
            BackCurveControl: new SKPoint(
                (float)(tipX - unitX * backCurveControlDistance),
                (float)(tipY - unitY * backCurveControlDistance)),
            BackCurveDepth: (float)(headHalfWidth * ArrowHeadBackCurveDepthRatio));
    }

    public static ModernArrowHeadPoints? ComputeModernArrowHeadPointsFromTangent(
        float tipX,
        float tipY,
        float tangentX,
        float tangentY,
        double headSize)
    {
        var tangentLength = Math.Sqrt(tangentX * tangentX + tangentY * tangentY);
        if (tangentLength <= 0)
        {
            return null;
        }

        var unitX = tangentX / tangentLength;
        var unitY = tangentY / tangentLength;
        var perpendicularX = -unitY;
        var perpendicularY = unitX;

        var headHeight = headSize * 2.5;
        var headWidthBase = headSize * 1.5;
        var wingWidth = headWidthBase * Math.Tan(Math.PI / 5.14);

        var baseX = tipX - headHeight * unitX;
        var baseY = tipY - headHeight * unitY;

        return new ModernArrowHeadPoints(
            WingLeft: new SKPoint((float)(baseX + perpendicularX * wingWidth), (float)(baseY + perpendicularY * wingWidth)),
            WingRight: new SKPoint((float)(baseX - perpendicularX * wingWidth), (float)(baseY - perpendicularY * wingWidth)));
    }

    public static BasicArrowHeadPoints? ComputeBasicArrowHeadPoints(
        float startX,
        float startY,
        float endX,
        float endY,
        double headHalfWidth)
    {
        return ComputeBasicArrowHeadPointsFromTangent(
            endX,
            endY,
            endX - startX,
            endY - startY,
            headHalfWidth);
    }

    public static BasicArrowHeadPoints? ComputeBasicArrowHeadPointsFromTangent(
        float tipX,
        float tipY,
        float tangentX,
        float tangentY,
        double headHalfWidth)
    {
        var tangentLength = Math.Sqrt(tangentX * tangentX + tangentY * tangentY);
        if (tangentLength <= 0)
        {
            return null;
        }

        var unitX = tangentX / tangentLength;
        var unitY = tangentY / tangentLength;
        var perpendicularX = -unitY;
        var perpendicularY = unitX;

        var headLength = headHalfWidth * BasicArrowHeadLengthRatio;
        var baseX = tipX - headLength * unitX;
        var baseY = tipY - headLength * unitY;

        return new BasicArrowHeadPoints(
            LeftBase: new SKPoint(
                (float)(baseX + perpendicularX * headHalfWidth),
                (float)(baseY + perpendicularY * headHalfWidth)),
            RightBase: new SKPoint(
                (float)(baseX - perpendicularX * headHalfWidth),
                (float)(baseY - perpendicularY * headHalfWidth)));
    }

    public static LineArrowHeadPoints? ComputeLineArrowHeadPoints(
        float startX,
        float startY,
        float endX,
        float endY,
        double headHalfWidth)
    {
        return ComputeLineArrowHeadPointsFromTangent(
            endX,
            endY,
            endX - startX,
            endY - startY,
            headHalfWidth);
    }

    public static LineArrowHeadPoints? ComputeLineArrowHeadPointsFromTangent(
        float tipX,
        float tipY,
        float tangentX,
        float tangentY,
        double headHalfWidth)
    {
        var tangentLength = Math.Sqrt(tangentX * tangentX + tangentY * tangentY);
        if (tangentLength <= 0)
        {
            return null;
        }

        var unitX = tangentX / tangentLength;
        var unitY = tangentY / tangentLength;
        var perpendicularX = -unitY;
        var perpendicularY = unitX;

        var headLength = headHalfWidth * LineArrowHeadLengthRatio;
        var baseX = tipX - headLength * unitX;
        var baseY = tipY - headLength * unitY;

        return new LineArrowHeadPoints(
            LeftBase: new SKPoint(
                (float)(baseX + perpendicularX * headHalfWidth),
                (float)(baseY + perpendicularY * headHalfWidth)),
            RightBase: new SKPoint(
                (float)(baseX - perpendicularX * headHalfWidth),
                (float)(baseY - perpendicularY * headHalfWidth)));
    }

    /// <summary>
    /// Single source of truth for arrow geometry points.
    /// Both <see cref="Render"/> (SKCanvas) and <c>CreateArrowGeometry</c> (Avalonia)
    /// consume this method so the shape can never diverge between rendering backends.
    /// Returns null when the arrow has zero length.
    /// </summary>
    public static ArrowPoints? ComputeArrowPoints(
        float startX, float startY,
        float endX, float endY,
        double headSize)
    {
        var dx = endX - startX;
        var dy = endY - startY;
        var length = Math.Sqrt(dx * dx + dy * dy);

        if (length <= 0) return null;

        var ux = dx / length;
        var uy = dy / length;
        var perpX = -uy;
        var perpY = ux;

        var headHeight = headSize * 2.5;
        var headWidthBase = headSize * 1.5;
        var arrowAngle = Math.PI / 5.14; // 35 degrees

        var baseX = endX - headHeight * ux;
        var baseY = endY - headHeight * uy;

        var wingWidth = headWidthBase * Math.Tan(arrowAngle);
        var shaftWidth = headWidthBase * 0.25;

        return new ArrowPoints(
            ShaftEndLeft: new SKPoint((float)(baseX + perpX * shaftWidth), (float)(baseY + perpY * shaftWidth)),
            ShaftEndRight: new SKPoint((float)(baseX - perpX * shaftWidth), (float)(baseY - perpY * shaftWidth)),
            WingLeft: new SKPoint((float)(baseX + perpX * wingWidth), (float)(baseY + perpY * wingWidth)),
            WingRight: new SKPoint((float)(baseX - perpX * wingWidth), (float)(baseY - perpY * wingWidth))
        );
    }

    /// <summary>
    /// Computed arrow geometry points returned by <see cref="ComputeArrowPoints"/>.
    /// </summary>
    public record struct ArrowPoints(
        SKPoint ShaftEndLeft,
        SKPoint ShaftEndRight,
        SKPoint WingLeft,
        SKPoint WingRight);

    public record struct ArrowCapPoints(
        SKPoint LeftBase,
        SKPoint RightBase,
        SKPoint BackCurveControl,
        float BackCurveDepth);

    public record struct BasicArrowHeadPoints(
        SKPoint LeftBase,
        SKPoint RightBase);

    public record struct LineArrowHeadPoints(
        SKPoint LeftBase,
        SKPoint RightBase);

    public record struct ModernArrowHeadPoints(
        SKPoint WingLeft,
        SKPoint WingRight);

    public override bool HitTest(SKPoint point, float tolerance = 5)
    {
        return CurvedSegmentHelper.DistanceToPath(this, point) <= tolerance;
    }

    public override SKRect GetBounds()
    {
        return CurvedSegmentHelper.GetBounds(this);
    }

    internal override void MoveBy(float deltaX, float deltaY)
    {
        base.MoveBy(deltaX, deltaY);
        CurvedSegmentHelper.OffsetCurvePoint(this, deltaX, deltaY);
    }
}