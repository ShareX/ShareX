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

public enum SmartEraserFillMode
{
    SolidColor,
    StretchHorizontally,
    StretchVertically
}

/// <summary>
/// Smart Eraser annotation - hides content by stretching a matching pair of opposing edges,
/// or by using the color sampled when drawing started when neither edge pair matches.
/// </summary>
public partial class SmartEraserAnnotation : RectangleAnnotation
{
    public SmartEraserFillMode FillMode { get; set; }

    /// <summary>
    /// Packed ARGB pixels for the matching edge. A horizontal stretch stores a
    /// top-to-bottom column; a vertical stretch stores a left-to-right row.
    /// </summary>
    public uint[]? EdgePixels { get; set; }

    public SmartEraserAnnotation()
    {
        ToolType = EditorTool.SmartEraser;
        // Default to a visible preview color until the canvas sample is available.
        StrokeColor = "#80FF0000";
        FillColor = "#80FF0000";
        StrokeWidth = 0;
        CornerRadius = 0;
        ShadowEnabled = false;
    }

    /// <summary>
    /// Chooses the best fill for the current bounds from the source image.
    /// Matching left/right columns are preferred, followed by matching top/bottom
    /// rows and finally the annotation's existing fill color.
    /// </summary>
    public void ConfigureFill(SKBitmap source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source.Width <= 0 || source.Height <= 0)
        {
            return;
        }

        var bounds = GetBounds();
        int left = Math.Clamp((int)Math.Floor(bounds.Left), 0, source.Width - 1);
        int top = Math.Clamp((int)Math.Floor(bounds.Top), 0, source.Height - 1);
        int right = Math.Clamp(Math.Max(left, (int)Math.Ceiling(bounds.Right) - 1), 0, source.Width - 1);
        int bottom = Math.Clamp(Math.Max(top, (int)Math.Ceiling(bounds.Bottom) - 1), 0, source.Height - 1);

        FillMode = SmartEraserFillMode.SolidColor;
        EdgePixels = null;

        bool columnsMatch = true;
        for (int y = top; y <= bottom; y++)
        {
            if (source.GetPixel(left, y) != source.GetPixel(right, y))
            {
                columnsMatch = false;
                break;
            }
        }

        if (columnsMatch)
        {
            FillMode = SmartEraserFillMode.StretchHorizontally;
            EdgePixels = new uint[bottom - top + 1];
            for (int y = top; y <= bottom; y++)
            {
                EdgePixels[y - top] = PackColor(source.GetPixel(left, y));
            }
            return;
        }

        bool rowsMatch = true;
        for (int x = left; x <= right; x++)
        {
            if (source.GetPixel(x, top) != source.GetPixel(x, bottom))
            {
                rowsMatch = false;
                break;
            }
        }

        if (rowsMatch)
        {
            FillMode = SmartEraserFillMode.StretchVertically;
            EdgePixels = new uint[right - left + 1];
            for (int x = left; x <= right; x++)
            {
                EdgePixels[x - left] = PackColor(source.GetPixel(x, top));
            }
        }
    }

    internal static SKColor UnpackColor(uint color) => new(
        red: (byte)(color >> 16),
        green: (byte)(color >> 8),
        blue: (byte)color,
        alpha: (byte)(color >> 24));

    private static uint PackColor(SKColor color) =>
        ((uint)color.Alpha << 24) |
        ((uint)color.Red << 16) |
        ((uint)color.Green << 8) |
        color.Blue;

    public override Annotation Clone()
    {
        var clone = (SmartEraserAnnotation)base.Clone();
        clone.EdgePixels = EdgePixels?.ToArray();
        return clone;
    }
}
