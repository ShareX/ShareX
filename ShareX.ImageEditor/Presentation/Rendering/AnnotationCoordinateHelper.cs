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
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Rendering;

/// <summary>
/// XIP0039 Guardrail 1: Centralises the coordinate-space contract for annotation model values.
/// </summary>
/// <remarks>
/// <para><strong>Coordinate contract:</strong></para>
/// <para>
/// All annotation model coordinates (StartPoint, EndPoint, Points, TailPoint) are stored
/// in <em>logical image pixels</em>.  "Logical image pixels" means the same unit as
/// <c>AnnotationCanvas.Width/Height</c>, which is set to
/// <c>EditorCore.CanvasSize.Width/Height</c> (= source bitmap dimensions in logical px).
/// </para>
/// <para>
/// Avalonia lays out <c>AnnotationCanvas</c> in logical pixels at 1:1 with the source image
/// dimensions, so no DPI scaling factor is required when reading or writing canvas coordinates
/// to annotation model fields.  <c>RenderScaling</c> affects only the physical display pixel
/// count and must NOT be applied to annotation model coordinates.
/// </para>
/// <para><strong>Historical inconsistency (RED – XIP0039 Guardrail 1):</strong></para>
/// <para>
/// Several paths incorrectly applied <c>RenderScaling</c> before writing annotation bounds:
/// <list type="bullet">
///   <item>Effect creation: <c>EditorInputController.cs:793-794</c> multiplied by scaling.</item>
///   <item>CutOut: <c>EditorInputController.cs:1489-1498</c> multiplied by scaling.</item>
/// </list>
/// The CutOut path has been corrected in this XIP (see <c>PerformCutOut</c>).
/// The effect-creation path correction is tracked as a follow-up because it may interact
/// with how <c>BaseEffectAnnotation.UpdateEffect</c> crops the source bitmap.
/// </para>
/// </remarks>
public static class AnnotationCoordinateHelper
{
    /// <summary>
    /// Converts a canvas-space <see cref="Point"/> to the annotation model coordinate.
    /// Because annotation model coordinates equal canvas logical pixels (no DPI scaling),
    /// this is effectively an identity conversion with an explicit type change.
    /// </summary>
    public static SKPoint ToAnnotationPoint(Point canvasPoint)
        => new SKPoint((float)canvasPoint.X, (float)canvasPoint.Y);

    /// <summary>
    /// Converts a canvas-space <see cref="Rect"/> to annotation model start/end points.
    /// </summary>
    public static (SKPoint Start, SKPoint End) ToAnnotationBounds(Rect canvasRect)
        => (new SKPoint((float)canvasRect.Left, (float)canvasRect.Top),
            new SKPoint((float)canvasRect.Right, (float)canvasRect.Bottom));

    /// <summary>
    /// Converts annotation model coordinates back to a canvas-space <see cref="Rect"/>.
    /// </summary>
    public static Rect ToCanvasRect(SKPoint start, SKPoint end)
        => new Rect(
            Math.Min(start.X, end.X),
            Math.Min(start.Y, end.Y),
            Math.Abs(end.X - start.X),
            Math.Abs(end.Y - start.Y));

    internal static Point RotatePoint(Point point, Point center, double angleDeg)
    {
        if (angleDeg == 0)
        {
            return point;
        }

        double rad = angleDeg * Math.PI / 180.0;
        double cos = Math.Cos(rad);
        double sin = Math.Sin(rad);
        double dx = point.X - center.X;
        double dy = point.Y - center.Y;
        return new Point(
            center.X + dx * cos - dy * sin,
            center.Y + dx * sin + dy * cos);
    }

    internal static Point UnrotatePoint(Point point, Point center, double angleDeg)
    {
        return RotatePoint(point, center, -angleDeg);
    }
}