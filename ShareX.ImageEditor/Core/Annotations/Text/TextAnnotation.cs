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
/// Text annotation
/// </summary>
public partial class TextAnnotation : Annotation
{
    public override AnnotationCategory Category => AnnotationCategory.Text;
    /// <summary>
    /// Text content
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Text body color
    /// </summary>
    public string TextColor { get; set; } = "#FF000000";

    /// <summary>
    /// Font size in pixels
    /// </summary>
    public float FontSize { get; set; } = 48;

    /// <summary>
    /// Font family
    /// </summary>
    public string FontFamily { get; set; } = "Segoe UI";

    /// <summary>
    /// Bold style
    /// </summary>
    public bool IsBold { get; set; }

    /// <summary>
    /// Italic style
    /// </summary>
    public bool IsItalic { get; set; }

    /// <summary>
    /// Horizontal alignment for text within the annotation bounds.
    /// </summary>
    public TextHorizontalAlignment HorizontalAlignment { get; set; } = TextHorizontalAlignment.Center;

    public TextAnnotation()
    {
        ToolType = EditorTool.Text;
    }

    public override bool HitTest(SKPoint point, float tolerance = 5)
    {
        var textBounds = GetBounds();

        // If rotated, transform the test point by the inverse rotation
        point = GetUnrotatedPoint(point, textBounds);

        var inflatedBounds = SKRect.Inflate(textBounds, tolerance, tolerance);
        return inflatedBounds.Contains(point);
    }

    public override SKRect GetBounds()
    {
        // Use StartPoint and EndPoint like other rectangle-based annotations
        float left = Math.Min(StartPoint.X, EndPoint.X);
        float top = Math.Min(StartPoint.Y, EndPoint.Y);
        float right = Math.Max(StartPoint.X, EndPoint.X);
        float bottom = Math.Max(StartPoint.Y, EndPoint.Y);

        // Ensure minimum size for visibility
        const float minSize = 10f;
        if (right - left < minSize) right = left + minSize;
        if (bottom - top < minSize) bottom = top + minSize;

        // Return the bounds defined by StartPoint and EndPoint
        return new SKRect(left, top, right, bottom);
    }

    internal override void Scale(float scaleX, float scaleY)
    {
        base.Scale(scaleX, scaleY);
        FontSize *= scaleY;
    }
}