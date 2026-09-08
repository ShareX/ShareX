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

public partial class FreehandAnnotation
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
            Data = CreateSmoothedGeometry(),
            Tag = this
        };

        if (ShadowEnabled)
        {
            path.Effect = ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateDropShadow(this);
        }

        return path;
    }

    public Geometry CreateSmoothedGeometry()
    {
        if (Points.Count < 2) return new StreamGeometry();

        var geometry = new StreamGeometry();
        using var context = geometry.Open();

        context.BeginFigure(new Point(Points[0].X, Points[0].Y), false);

        if (Points.Count == 2)
        {
            context.LineTo(new Point(Points[1].X, Points[1].Y));
        }
        else
        {
            var p0 = Points[0];
            var p1 = Points[1];
            var mid = new Point((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2);
            context.LineTo(mid);

            for (int i = 1; i < Points.Count - 1; i++)
            {
                var pControl = new Point(Points[i].X, Points[i].Y);
                var pNext = Points[i + 1];
                var nextMid = new Point((pControl.X + pNext.X) / 2, (pControl.Y + pNext.Y) / 2);

                context.QuadraticBezierTo(pControl, nextMid);
            }

            context.LineTo(new Point(Points[Points.Count - 1].X, Points[Points.Count - 1].Y));
        }

        context.EndFigure(false);
        return geometry;
    }

    internal void UpdateVisual(Avalonia.Controls.Shapes.Path freehandPath)
    {
        freehandPath.StrokeDashArray = BorderStyleDashHelper.CreateStrokeDashArray(BorderStyle);
        freehandPath.StrokeLineCap = BorderStyleDashHelper.CreateStrokeLineCap(BorderStyle);
        freehandPath.Data = CreateSmoothedGeometry();
    }
}
