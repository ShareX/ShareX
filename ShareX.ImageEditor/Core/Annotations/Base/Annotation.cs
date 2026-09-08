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
/// Base class for all annotation types
/// </summary>
[JsonDerivedType(typeof(ArrowAnnotation), typeDiscriminator: "Arrow")]
[JsonDerivedType(typeof(BlurAnnotation), typeDiscriminator: "Blur")]
[JsonDerivedType(typeof(CropAnnotation), typeDiscriminator: "Crop")]
[JsonDerivedType(typeof(CursorAnnotation), typeDiscriminator: "Cursor")]
[JsonDerivedType(typeof(EllipseAnnotation), typeDiscriminator: "Ellipse")]
[JsonDerivedType(typeof(EmojiAnnotation), typeDiscriminator: "Emoji")]
[JsonDerivedType(typeof(FreehandAnnotation), typeDiscriminator: "Freehand")]
[JsonDerivedType(typeof(HighlightAnnotation), typeDiscriminator: "Highlight")]
[JsonDerivedType(typeof(ImageAnnotation), typeDiscriminator: "Image")]
[JsonDerivedType(typeof(LineAnnotation), typeDiscriminator: "Line")]
[JsonDerivedType(typeof(MagnifyAnnotation), typeDiscriminator: "Magnify")]
[JsonDerivedType(typeof(NumberAnnotation), typeDiscriminator: "Number")]
[JsonDerivedType(typeof(PixelateAnnotation), typeDiscriminator: "Pixelate")]
[JsonDerivedType(typeof(RectangleAnnotation), typeDiscriminator: "Rectangle")]
[JsonDerivedType(typeof(SmartEraserAnnotation), typeDiscriminator: "SmartEraser")]
[JsonDerivedType(typeof(SpeechBalloonAnnotation), typeDiscriminator: "SpeechBalloon")]
[JsonDerivedType(typeof(SpotlightAnnotation), typeDiscriminator: "Spotlight")]
[JsonDerivedType(typeof(TextAnnotation), typeDiscriminator: "Text")]
public abstract class Annotation
{
    public const string DefaultShadowColorHex = "#80000000";
    public const double DefaultShadowBlurRadius = 4;
    public const double DefaultShadowOpacity = 1;
    public const double DefaultShadowOffsetX = 3;
    public const double DefaultShadowOffsetY = 3;

    /// <summary>
    /// Functional category of this annotation type (Shapes, Effects, or Text).
    /// </summary>
    public abstract AnnotationCategory Category { get; }

    /// <summary>
    /// Unique identifier for this annotation
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tool type that created this annotation
    /// </summary>
    public EditorTool ToolType { get; set; }

    /// <summary>
    /// Stroke/border color (hex color string)
    /// </summary>
    public string StrokeColor { get; set; } = "#ef4444";

    /// <summary>
    /// Stroke width in pixels
    /// </summary>
    public float StrokeWidth { get; set; } = 4;

    /// <summary>
    /// Fill color (hex color string, transparent by default for stroke-only shapes)
    /// </summary>
    public string FillColor { get; set; } = "#00000000";

    /// <summary>
    /// Whether shadow is enabled for this annotation
    /// </summary>
    public bool ShadowEnabled { get; set; }

    /// <summary>
    /// Shadow color (hex color string)
    /// </summary>
    public string ShadowColor { get; set; } = DefaultShadowColorHex;

    /// <summary>
    /// Shadow blur radius in pixels
    /// </summary>
    public double ShadowBlurRadius { get; set; } = DefaultShadowBlurRadius;

    /// <summary>
    /// Shadow opacity from 0 to 1
    /// </summary>
    public double ShadowOpacity { get; set; } = DefaultShadowOpacity;

    /// <summary>
    /// Horizontal shadow offset in pixels
    /// </summary>
    public double ShadowOffsetX { get; set; } = DefaultShadowOffsetX;

    /// <summary>
    /// Vertical shadow offset in pixels
    /// </summary>
    public double ShadowOffsetY { get; set; } = DefaultShadowOffsetY;

    /// <summary>
    /// Starting point (top-left for rectangles, start for lines/arrows)
    /// </summary>
    public SKPoint StartPoint { get; set; }

    /// <summary>
    /// Ending point (bottom-right for rectangles, end for lines/arrows)
    /// </summary>
    public SKPoint EndPoint { get; set; }

    /// <summary>
    /// Whether this annotation is currently selected
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Z-order for rendering (higher = on top)
    /// </summary>
    public int ZIndex { get; set; }

    /// <summary>
    /// Rotation angle in degrees (clockwise). Used for rotating annotations.
    /// </summary>
    public float RotationAngle { get; set; }

    /// <summary>
    /// Hit test to determine if a point intersects this annotation
    /// </summary>
    /// <param name="point">Point to test</param>
    /// <param name="tolerance">Hit test tolerance in pixels</param>
    /// <returns>True if the point hits this annotation</returns>
    public abstract bool HitTest(SKPoint point, float tolerance = 10);

    /// <summary>
    /// Get the bounding rectangle for this annotation
    /// </summary>
    public virtual SKRect GetBounds()
    {
        return new SKRect(
            Math.Min(StartPoint.X, EndPoint.X),
            Math.Min(StartPoint.Y, EndPoint.Y),
            Math.Max(StartPoint.X, EndPoint.X),
            Math.Max(StartPoint.Y, EndPoint.Y));
    }

    /// <summary>
    /// Creates a deep clone of this annotation for undo/redo history.
    /// Derived classes should override to handle reference-type properties.
    /// </summary>
    /// <remarks>
    /// XIP0039 Guardrail 3: Clones preserve the original Id so that
    /// <see cref="EditorHistory"/> mementos can restore selection by Id after undo/redo.
    /// Clones live only inside history snapshots and are never added to a live canvas
    /// alongside their source, so Id uniqueness within the active canvas is maintained.
    /// </remarks>
    public virtual Annotation Clone()
    {
        // MemberwiseClone handles value types (SKPoint, float, int, etc.) correctly
        // Derived classes override to deep copy reference types (lists, bitmaps)
        var clone = (Annotation)MemberwiseClone();
        // Id is preserved (not regenerated) so history can match SelectedAnnotationId on restore.
        return clone;
    }

    /// <summary>
    /// Parse hex color string to SKColor
    /// </summary>
    protected SKColor ParseColor(string hexColor)
    {
        return SKColor.Parse(hexColor);
    }

    /// <summary>
    /// Transforms the annotation's endpoints and any shape-specific points.
    /// </summary>
    internal void TransformPoints(Func<SKPoint, SKPoint> transformPoint)
    {
        StartPoint = transformPoint(StartPoint);
        EndPoint = transformPoint(EndPoint);
        TransformAdditionalPoints(transformPoint);
    }

    internal virtual void TransformAdditionalPoints(Func<SKPoint, SKPoint> transformPoint)
    {
    }

    internal virtual void Scale(float scaleX, float scaleY)
    {
        TransformPoints(point => new SKPoint(point.X * scaleX, point.Y * scaleY));
        StrokeWidth *= Math.Min(scaleX, scaleY);
    }

    /// <summary>
    /// Converts a canvas point to the annotation's unrotated coordinates for hit testing.
    /// </summary>
    protected SKPoint GetUnrotatedPoint(SKPoint point, SKRect bounds)
    {
        if (RotationAngle == 0)
        {
            return point;
        }

        float cx = bounds.MidX;
        float cy = bounds.MidY;
        float rad = -RotationAngle * (float)Math.PI / 180f;
        float cos = (float)Math.Cos(rad);
        float sin = (float)Math.Sin(rad);
        float dx = point.X - cx;
        float dy = point.Y - cy;
        return new SKPoint(cx + dx * cos - dy * sin, cy + dx * sin + dy * cos);
    }

    /// <summary>
    /// Moves the annotation during a selection drag or keyboard nudge.
    /// </summary>
    internal virtual void MoveBy(float deltaX, float deltaY)
    {
        StartPoint = new SKPoint(StartPoint.X + deltaX, StartPoint.Y + deltaY);
        EndPoint = new SKPoint(EndPoint.X + deltaX, EndPoint.Y + deltaY);
    }
}
