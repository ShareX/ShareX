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
using ShareX.ImageEditor.Presentation.Helpers;

namespace ShareX.ImageEditor.Core.Annotations;

public partial class LineAnnotation
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
            StrokeDashArray = BorderStyleDashHelper.CreateStrokeDashArray(BorderStyle),
            StrokeLineCap = BorderStyleDashHelper.CreateStrokeLineCap(BorderStyle),
            StrokeJoin = PenLineJoin.Round,
            Fill = Brushes.Transparent,
            Data = CreateLineGeometry(),
            Tag = this
        };

        if (ShadowEnabled)
        {
            path.Effect = ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateDropShadow(this);
        }

        return path;
    }

    public Geometry CreateLineGeometry()
    {
        var start = new Point(StartPoint.X, StartPoint.Y);
        var end = new Point(EndPoint.X, EndPoint.Y);
        var geometry = new StreamGeometry();

        using (var context = geometry.Open())
        {
            context.BeginFigure(start, false);

            if (CurvedSegmentHelper.HasCurve(this))
            {
                var controlPoint = CurvedSegmentHelper.GetQuadraticControlPoint(this);
                context.QuadraticBezierTo(new Point(controlPoint.X, controlPoint.Y), end);
            }
            else
            {
                context.LineTo(end);
            }

            context.EndFigure(false);
        }

        return geometry;
    }

    internal void UpdateVisual(Avalonia.Controls.Shapes.Path linePath)
    {
        linePath.StrokeDashArray = BorderStyleDashHelper.CreateStrokeDashArray(BorderStyle);
        linePath.StrokeLineCap = BorderStyleDashHelper.CreateStrokeLineCap(BorderStyle);
        linePath.Data = CreateLineGeometry();
    }
}
