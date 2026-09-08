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
using Avalonia.Controls;
using Avalonia.Media;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class ArrowAnnotation
{
    /// <summary>
    /// Creates the Avalonia visual for this annotation
    /// </summary>
    public Control CreateVisual()
    {
        var brush = new SolidColorBrush(Color.Parse(StrokeColor));
        var path = new Avalonia.Controls.Shapes.Path
        {
            Stroke = brush,
            StrokeThickness = StrokeWidth,
            StrokeLineCap = PenLineCap.Round,
            StrokeJoin = PenLineJoin.Round,
            Fill = brush,
            Tag = this
        };

        // Populate the arrow geometry using the annotation's points.
        path.Data = CreateArrowGeometry();

        if (ShadowEnabled)
        {
            path.Effect = ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateDropShadow(this);
        }

        return path;
    }

    /// <summary>
    /// Creates arrow geometry for the Avalonia Path.
    /// Consumes <see cref="ComputeArrowPoints"/> to guarantee identical shape with <see cref="Render"/>.
    /// </summary>
    public Geometry CreateArrowGeometry()
    {
        var start = new Point(StartPoint.X, StartPoint.Y);
        var end = new Point(EndPoint.X, EndPoint.Y);
        return CreateArrowGeometry(start, end, StrokeWidth * GetArrowHeadWidthMultiplier(Style));
    }

    public Geometry CreateArrowGeometry(Point start, Point end, double headSize)
    {
        return Style switch
        {
            ArrowStyle.Double when CurvedSegmentHelper.HasCurve(this) => CreateCurvedClassicArrowGeometry(start, end, headSize, includeStartCap: true),
            ArrowStyle.Double => CreateClassicArrowGeometry(start, end, headSize, includeStartCap: true),
            ArrowStyle.Line when CurvedSegmentHelper.HasCurve(this) => CreateCurvedLineArrowGeometry(start, end, headSize),
            ArrowStyle.Line => CreateLineArrowGeometry(start, end, headSize),
            ArrowStyle.Basic when CurvedSegmentHelper.HasCurve(this) => CreateCurvedBasicArrowGeometry(start, end, headSize),
            ArrowStyle.Basic => CreateBasicArrowGeometry(start, end, headSize),
            ArrowStyle.Modern when CurvedSegmentHelper.HasCurve(this) => CreateCurvedModernArrowGeometry(start, end, headSize),
            ArrowStyle.Modern => CreateModernArrowGeometry(start, end, headSize),
            _ when CurvedSegmentHelper.HasCurve(this) => CreateCurvedClassicArrowGeometry(start, end, headSize),
            _ => CreateClassicArrowGeometry(start, end, headSize)
        };
    }

    private Geometry CreateClassicArrowGeometry(Point start, Point end, double headSize, bool includeStartCap = false)
    {
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            var endCap = ComputeArrowCapPoints(
                (float)start.X, (float)start.Y,
                (float)end.X, (float)end.Y,
                headSize);

            if (endCap is { } p)
            {
                ctx.BeginFigure(start, false);
                ctx.LineTo(end);
                ctx.EndFigure(false);

                if (includeStartCap)
                {
                    var startCap = ComputeArrowCapPoints(
                        (float)end.X, (float)end.Y,
                        (float)start.X, (float)start.Y,
                        headSize);

                    if (startCap is { } startPoints)
                    {
                        AppendArrowCapFigure(ctx, start, startPoints);
                    }
                }

                AppendArrowCapFigure(ctx, end, p);
            }
            else
            {
                var radius = 2.0;
                ctx.BeginFigure(new Point(start.X - radius, start.Y), true);
                ctx.ArcTo(new Point(start.X + radius, start.Y), new Size(radius, radius), 0, false, SweepDirection.Clockwise);
                ctx.ArcTo(new Point(start.X - radius, start.Y), new Size(radius, radius), 0, false, SweepDirection.Clockwise);
                ctx.EndFigure(true);
            }
        }

        return geometry;
    }

    private Geometry CreateModernArrowGeometry(Point start, Point end, double headSize)
    {
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            var points = ComputeArrowPoints(
                (float)start.X, (float)start.Y,
                (float)end.X, (float)end.Y,
                headSize);

            if (points is { } p)
            {
                ctx.BeginFigure(start, true);
                ctx.LineTo(new Point(p.ShaftEndLeft.X, p.ShaftEndLeft.Y));
                ctx.LineTo(new Point(p.WingLeft.X, p.WingLeft.Y));
                ctx.LineTo(end);
                ctx.LineTo(new Point(p.WingRight.X, p.WingRight.Y));
                ctx.LineTo(new Point(p.ShaftEndRight.X, p.ShaftEndRight.Y));
                ctx.EndFigure(true);
            }
            else
            {
                var radius = 2.0;
                ctx.BeginFigure(new Point(start.X - radius, start.Y), true);
                ctx.ArcTo(new Point(start.X + radius, start.Y), new Size(radius, radius), 0, false, SweepDirection.Clockwise);
                ctx.ArcTo(new Point(start.X - radius, start.Y), new Size(radius, radius), 0, false, SweepDirection.Clockwise);
                ctx.EndFigure(true);
            }
        }

        return geometry;
    }

    private Geometry CreateBasicArrowGeometry(Point start, Point end, double headSize)
    {
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            var head = ComputeBasicArrowHeadPoints(
                (float)start.X,
                (float)start.Y,
                (float)end.X,
                (float)end.Y,
                headSize);

            if (head is { } points)
            {
                ctx.BeginFigure(start, false);
                ctx.LineTo(end);
                ctx.EndFigure(false);

                AppendBasicArrowHeadFigure(ctx, end, points);
            }
            else
            {
                var radius = 2.0;
                ctx.BeginFigure(new Point(start.X - radius, start.Y), true);
                ctx.ArcTo(new Point(start.X + radius, start.Y), new Size(radius, radius), 0, false, SweepDirection.Clockwise);
                ctx.ArcTo(new Point(start.X - radius, start.Y), new Size(radius, radius), 0, false, SweepDirection.Clockwise);
                ctx.EndFigure(true);
            }
        }

        return geometry;
    }

    private Geometry CreateLineArrowGeometry(Point start, Point end, double headSize)
    {
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            var head = ComputeLineArrowHeadPoints(
                (float)start.X,
                (float)start.Y,
                (float)end.X,
                (float)end.Y,
                headSize);

            if (head is { } points)
            {
                ctx.BeginFigure(start, false);
                ctx.LineTo(end);
                ctx.EndFigure(false);

                AppendLineArrowHeadFigure(ctx, end, points);
            }
            else
            {
                var radius = 2.0;
                ctx.BeginFigure(new Point(start.X - radius, start.Y), true);
                ctx.ArcTo(new Point(start.X + radius, start.Y), new Size(radius, radius), 0, false, SweepDirection.Clockwise);
                ctx.ArcTo(new Point(start.X - radius, start.Y), new Size(radius, radius), 0, false, SweepDirection.Clockwise);
                ctx.EndFigure(true);
            }
        }

        return geometry;
    }

    private Geometry CreateCurvedClassicArrowGeometry(Point start, Point end, double headSize, bool includeStartCap = false)
    {
        var geometry = new StreamGeometry();
        var controlPoint = CurvedSegmentHelper.GetQuadraticControlPoint(this);
        var endTangent = CurvedSegmentHelper.GetQuadraticTangentAtEnd(this);

        using (var context = geometry.Open())
        {
            context.BeginFigure(start, false);
            context.QuadraticBezierTo(new Point(controlPoint.X, controlPoint.Y), end);
            context.EndFigure(false);

            var endCap = ComputeArrowCapPointsFromTangent(
                (float)end.X,
                (float)end.Y,
                endTangent.X,
                endTangent.Y,
                headSize);

            if (includeStartCap)
            {
                var startTangent = CurvedSegmentHelper.GetQuadraticTangentAtStart(this);
                var startCap = ComputeArrowCapPointsFromTangent(
                    (float)start.X,
                    (float)start.Y,
                    -startTangent.X,
                    -startTangent.Y,
                    headSize);

                if (startCap is { } startPoints)
                {
                    AppendArrowCapFigure(context, start, startPoints);
                }
            }

            if (endCap is { } endPoints)
            {
                AppendArrowCapFigure(context, end, endPoints);
            }
        }

        return geometry;
    }

    private Geometry CreateCurvedBasicArrowGeometry(Point start, Point end, double headSize)
    {
        var geometry = new StreamGeometry();
        var controlPoint = CurvedSegmentHelper.GetQuadraticControlPoint(this);
        var tangent = CurvedSegmentHelper.GetQuadraticTangentAtEnd(this);

        using (var context = geometry.Open())
        {
            context.BeginFigure(start, false);
            context.QuadraticBezierTo(new Point(controlPoint.X, controlPoint.Y), end);
            context.EndFigure(false);

            var head = ComputeBasicArrowHeadPointsFromTangent(
                (float)end.X,
                (float)end.Y,
                tangent.X,
                tangent.Y,
                headSize);

            if (head is { } points)
            {
                AppendBasicArrowHeadFigure(context, end, points);
            }
        }

        return geometry;
    }

    private Geometry CreateCurvedLineArrowGeometry(Point start, Point end, double headSize)
    {
        var geometry = new StreamGeometry();
        var controlPoint = CurvedSegmentHelper.GetQuadraticControlPoint(this);
        var tangent = CurvedSegmentHelper.GetQuadraticTangentAtEnd(this);

        using (var context = geometry.Open())
        {
            context.BeginFigure(start, false);
            context.QuadraticBezierTo(new Point(controlPoint.X, controlPoint.Y), end);
            context.EndFigure(false);

            var head = ComputeLineArrowHeadPointsFromTangent(
                (float)end.X,
                (float)end.Y,
                tangent.X,
                tangent.Y,
                headSize);

            if (head is { } points)
            {
                AppendLineArrowHeadFigure(context, end, points);
            }
        }

        return geometry;
    }

    private Geometry CreateCurvedModernArrowGeometry(Point start, Point end, double headSize)
    {
        var geometry = new StreamGeometry();
        var controlPoint = CurvedSegmentHelper.GetQuadraticControlPoint(this);
        var tangent = CurvedSegmentHelper.GetQuadraticTangentAtEnd(this);

        using (var context = geometry.Open())
        {
            context.BeginFigure(start, false);
            context.QuadraticBezierTo(new Point(controlPoint.X, controlPoint.Y), end);
            context.EndFigure(false);

            var head = ComputeModernArrowHeadPointsFromTangent(
                (float)end.X,
                (float)end.Y,
                tangent.X,
                tangent.Y,
                headSize);

            if (head is null)
            {
                return geometry;
            }

            context.BeginFigure(end, true);
            context.LineTo(new Point(head.Value.WingLeft.X, head.Value.WingLeft.Y));
            context.LineTo(new Point(head.Value.WingRight.X, head.Value.WingRight.Y));
            context.EndFigure(true);
        }

        return geometry;
    }

    private static void AppendArrowCapFigure(StreamGeometryContext context, Point tip, ArrowCapPoints cap)
    {
        context.BeginFigure(tip, true);
        context.LineTo(new Point(cap.LeftBase.X, cap.LeftBase.Y));
        context.QuadraticBezierTo(
            new Point(cap.BackCurveControl.X, cap.BackCurveControl.Y),
            new Point(cap.RightBase.X, cap.RightBase.Y));
        context.EndFigure(true);
    }

    private static void AppendBasicArrowHeadFigure(StreamGeometryContext context, Point tip, BasicArrowHeadPoints head)
    {
        context.BeginFigure(tip, true);
        context.LineTo(new Point(head.LeftBase.X, head.LeftBase.Y));
        context.LineTo(new Point(head.RightBase.X, head.RightBase.Y));
        context.EndFigure(true);
    }

    private static void AppendLineArrowHeadFigure(StreamGeometryContext context, Point tip, LineArrowHeadPoints head)
    {
        context.BeginFigure(new Point(head.LeftBase.X, head.LeftBase.Y), false);
        context.LineTo(tip);
        context.LineTo(new Point(head.RightBase.X, head.RightBase.Y));
        context.EndFigure(false);
    }

    internal void UpdateVisual(Avalonia.Controls.Shapes.Path arrowPath)
    {
        var brush = new SolidColorBrush(Color.Parse(StrokeColor));
        arrowPath.Stroke = brush;
        arrowPath.Fill = brush;
        arrowPath.StrokeThickness = StrokeWidth;
        arrowPath.StrokeLineCap = PenLineCap.Round;
        arrowPath.StrokeJoin = PenLineJoin.Round;
        arrowPath.Data = CreateArrowGeometry();
    }
}
