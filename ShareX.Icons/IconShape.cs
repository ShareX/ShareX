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

using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace ShareX.Icons;

/// <summary>
/// One part of an icon: a geometry stroked or filled in one color, the Foreground unless the shape has its own
/// brush. Strokes use the icon style's width unless they set their own.
/// </summary>
internal sealed class IconShape
{
    private readonly Geometry _geometry;
    private readonly bool _filled;
    private readonly double? _strokeWidth;
    private readonly double[]? _dashes;
    private readonly double _opacity;
    private readonly IBrush? _brush;
    private readonly Geometry? _cut;
    private IDashStyle? _dashStyle;
    private Geometry? _cutArea;

    private IconShape(Geometry geometry, bool filled, double? strokeWidth, double[]? dashes, double opacity, IBrush? brush, Geometry? cut = null)
    {
        _geometry = geometry;
        _filled = filled;
        _strokeWidth = strokeWidth;
        _dashes = dashes;
        _opacity = opacity;
        _brush = brush;
        _cut = cut;
    }

    /// <summary>A line. Dash and gap lengths are in grid units, as in SVG.</summary>
    public static IconShape Stroke(string pathData, double? width = null, double[]? dashes = null) =>
        Stroke(IconGeometry.Path(pathData), width, dashes);

    /// <inheritdoc cref="Stroke(string, double?, double[])"/>
    public static IconShape Stroke(Geometry geometry, double? width = null, double[]? dashes = null) =>
        new(geometry, filled: false, width, dashes, opacity: 1, brush: null);

    /// <summary>
    /// A filled area. A partial opacity shades it in the Foreground; an outline, drawn in the fill's color, rounds
    /// the corners of small shapes such as arrowheads.
    /// </summary>
    public static IconShape Fill(string pathData, double opacity = 1, IBrush? brush = null, double outline = 0) =>
        Fill(IconGeometry.Path(pathData), opacity, brush, outline);

    /// <inheritdoc cref="Fill(string, double, IBrush, double)"/>
    public static IconShape Fill(Geometry geometry, double opacity = 1, IBrush? brush = null, double outline = 0) =>
        new(geometry, filled: true, outline, dashes: null, opacity, brush);

    /// <summary>
    /// This shape with the area of <paramref name="cut"/> left out, so a badge or slash drawn over it stands apart
    /// with a gap. See <see cref="IconGeometry.Halo"/>.
    /// </summary>
    public IconShape Except(Geometry cut) => new(_geometry, _filled, _strokeWidth, _dashes, _opacity, _brush, cut);

    public void Draw(DrawingContext context, IconStyle style, IBrush foreground)
    {
        IBrush color = _brush ?? foreground;
        double strokeWidth = _strokeWidth ?? style.StrokeWidth;
        IPen? pen = null;

        if (strokeWidth > 0)
        {
            // Avalonia dash lengths are multiples of the stroke width.
            if (_dashes != null)
            {
                _dashStyle ??= new ImmutableDashStyle(_dashes.Select(length => length / strokeWidth), 0);
            }

            pen = new Pen(color, strokeWidth, _dashStyle, style.LineCap, style.LineJoin);
        }

        using (context.PushOpacity(_opacity))
        {
            if (_cut == null)
            {
                context.DrawGeometry(_filled ? color : null, pen, _geometry);
            }
            else
            {
                // The stroke becomes an area, so the cut can be taken out of it.
                _cutArea ??= IconGeometry.Subtract(GetArea(pen), _cut);
                context.DrawGeometry(color, null, _cutArea);
            }
        }
    }

    private Geometry GetArea(IPen? pen)
    {
        Geometry? stroke = pen?.Thickness > 0 ? _geometry.GetWidenedGeometry(pen) : null;

        if (!_filled)
        {
            return stroke ?? _geometry;
        }

        return stroke == null ? _geometry : IconGeometry.Union(_geometry, stroke);
    }
}
