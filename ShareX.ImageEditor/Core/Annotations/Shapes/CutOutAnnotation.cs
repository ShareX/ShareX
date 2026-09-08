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
/// CutOut annotation - cuts out a horizontal or vertical section and joins remaining parts
/// Note: This is a special annotation that triggers actual image modification
/// </summary>
public class CutOutAnnotation : Annotation
{
    public override AnnotationCategory Category => AnnotationCategory.Shapes;
    /// <summary>
    /// Indicates if the cut is vertical (true) or horizontal (false)
    /// </summary>
    public bool IsVertical { get; set; }

    public CutOutAnnotation()
    {
        ToolType = EditorTool.CutOut;
    }

    public override bool HitTest(SKPoint point, float tolerance = 5)
    {
        var rect = GetBounds();

        if (IsVertical)
        {
            // Check if point is near the vertical line
            float x = rect.MidX;
            return Math.Abs(point.X - x) <= tolerance;
        }
        else
        {
            // Check if point is near the horizontal line
            float y = rect.MidY;
            return Math.Abs(point.Y - y) <= tolerance;
        }
    }

    /// <summary>
    /// Adjusts an annotation after removing a strip. Returns false when it lies entirely inside the strip.
    /// </summary>
    internal static bool AdjustAnnotation(Annotation annotation, int startPos, int endPos, bool isVertical, SKBitmap source)
    {
        SKRect bounds = annotation.GetBounds();
        float leadingEdge = isVertical ? bounds.Left : bounds.Top;
        float trailingEdge = isVertical ? bounds.Right : bounds.Bottom;

        if (leadingEdge >= startPos && trailingEdge <= endPos)
        {
            return false;
        }

        bool isAfterCut = leadingEdge >= endPos;
        bool needsAdjustment = isAfterCut || trailingEdge > endPos;
        if (!needsAdjustment)
        {
            return true;
        }

        int cutLength = endPos - startPos;
        annotation.StartPoint = ShiftEndpoint(annotation.StartPoint);
        annotation.EndPoint = ShiftEndpoint(annotation.EndPoint);
        annotation.TransformAdditionalPoints(CollapsePoint);

        if (annotation is BaseEffectAnnotation effect)
        {
            effect.UpdateEffect(source);
        }

        return true;

        SKPoint ShiftEndpoint(SKPoint point)
        {
            float position = isVertical ? point.X : point.Y;
            if (!isAfterCut && position > startPos && position < endPos)
            {
                position = startPos;
            }

            position -= cutLength;
            return isVertical ? new SKPoint(position, point.Y) : new SKPoint(point.X, position);
        }

        SKPoint CollapsePoint(SKPoint point)
        {
            float position = isVertical ? point.X : point.Y;
            if (position >= endPos)
            {
                position -= cutLength;
            }
            else if (position > startPos)
            {
                position = startPos;
            }

            return isVertical ? new SKPoint(position, point.Y) : new SKPoint(point.X, position);
        }
    }
}