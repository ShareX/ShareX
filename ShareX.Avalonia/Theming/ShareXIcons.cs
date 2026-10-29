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
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace ShareX.AvaloniaUI.Theming;

/// <summary>
/// ShareX's own vector icons, for concepts the generic Lucide glyphs don't draw well (the image editor
/// tools). The constants go anywhere an icon string goes; <see cref="Controls.IconPresenter"/> draws them.
/// </summary>
/// <remarks>
/// Icons are drawn like Lucide so they sit next to its glyphs: a 24x24 grid, SVG path data, 2px strokes
/// with round caps and joins. Line work is drawn in the Foreground. Shading is the Foreground at partial
/// opacity, so it becomes accent shades on the toolbar; use it only where the fill is the point (the blur
/// glow, pixelate cells). A fixed color is only for what the tool puts on the image, such as the
/// highlighter ink.
/// </remarks>
public static class ShareXIcons
{
    public const string tool_arrow = "sharex:tool-arrow";
    public const string tool_blur = "sharex:tool-blur";
    public const string tool_clone = "sharex:tool-clone";
    public const string tool_crop = "sharex:tool-crop";
    public const string tool_cursor = "sharex:tool-cursor";
    public const string tool_cut_out = "sharex:tool-cut-out";
    public const string tool_ellipse = "sharex:tool-ellipse";
    public const string tool_emoji = "sharex:tool-emoji";
    public const string tool_emoji_sunglasses = "sharex:tool-emoji-sunglasses";
    public const string tool_freehand = "sharex:tool-freehand";
    public const string tool_highlight = "sharex:tool-highlight";
    public const string tool_image = "sharex:tool-image";
    public const string tool_line = "sharex:tool-line";
    public const string tool_magnify = "sharex:tool-magnify";
    public const string tool_pixelate = "sharex:tool-pixelate";
    public const string tool_rectangle = "sharex:tool-rectangle";
    public const string tool_region = "sharex:tool-region";
    public const string tool_select = "sharex:tool-select";
    public const string tool_smart_eraser = "sharex:tool-smart-eraser";
    public const string tool_speech_balloon = "sharex:tool-speech-balloon";
    public const string tool_spotlight = "sharex:tool-spotlight";
    public const string tool_step = "sharex:tool-step";
    public const string tool_text = "sharex:tool-text";

    internal const double GridSize = 24;

    private static readonly IBrush HighlighterInk = new ImmutableSolidColorBrush(Color.FromRgb(0xFF, 0xD6, 0x0A));

    private static Dictionary<string, Shape[]>? _icons;

    /// <summary>Returns the shapes of a ShareX icon, or null if <paramref name="icon"/> is a font glyph.</summary>
    internal static Shape[]? GetShapes(string? icon)
    {
        if (icon == null || !icon.StartsWith("sharex:", StringComparison.Ordinal))
        {
            return null;
        }

        // Built on first use: geometries need the platform's render interface.
        _icons ??= CreateIcons();
        return _icons.GetValueOrDefault(icon);
    }

    private static Dictionary<string, Shape[]> CreateIcons() => new()
    {
        [tool_arrow] =
        [
            Stroke(Path("M4 20 16.69 7.31")),
            Fill(Path("M20.5 3.5 17.85 12.16 16.99 7.01 11.84 6.15z"), outline: 0.5)
        ],
        [tool_blur] =
        [
            Fill(Circle(12, 12, 10), 0.3),
            Fill(Circle(12, 12, 7.5), 0.45),
            Fill(Circle(12, 12, 5), 0.65),
            Fill(Circle(12, 12, 3))
        ],
        [tool_clone] =
        [
            Stroke(Path("M5.5 16H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2v1.5"), dashes: [3.3, 2.5]),
            Stroke(Rectangle(8, 8, 14, 14, 2))
        ],
        [tool_crop] =
        [
            Fill(Rectangle(6, 6, 12, 12, 0), 0.3),
            Stroke(Path("M6 2v14a2 2 0 0 0 2 2h14M18 22V8a2 2 0 0 0-2-2H2"))
        ],
        [tool_cursor] = [Stroke(Path("M5.65 2.75v16.2l4.1-3.9 2.7 6.2 3-1.3-2.7-6.1h5.6z"), 1.75)],
        [tool_cut_out] =
        [
            Fill(Rectangle(3, 9, 18, 6, 0), 0.35),
            Stroke(Rectangle(3, 3, 18, 18, 2)),
            Stroke(Path("M3 9h18M3 15h18"), 1.5, [2, 2])
        ],
        [tool_ellipse] = [Stroke(new EllipseGeometry(new Rect(2.5, 4.5, 19, 15)))],
        [tool_emoji] =
        [
            Stroke(Circle(12, 12, 10)),
            Fill(Circle(8.75, 9.5, 1.4)),
            Fill(Circle(15.25, 9.5, 1.4)),
            Stroke(Path("M7.5 14a5 5 0 0 0 9 0"))
        ],
        // The emoji face in sunglasses, with a shine cut out of each lens.
        [tool_emoji_sunglasses] =
        [
            Stroke(Circle(12, 12, 10)),
            Fill(Path("M4.25 8h6.5v2.25a2.75 2.75 0 0 1-2.75 2.75h-1a2.75 2.75 0 0 1-2.75-2.75zM13.25 8h6.5v2.25a2.75 2.75 0 0 1-2.75 2.75h-1a2.75 2.75 0 0 1-2.75-2.75zM6 9.25h1.25l-1.1 2h-1.25zM15 9.25h1.25l-1.1 2h-1.25z")),
            Stroke(Path("M2.85 8h18.3M8 15.5q4 3.25 8 0"))
        ],
        [tool_freehand] = [Stroke(Path("M2.5 20.5c1.5-2.5 3-2.5 3.5-0.5s2.5 1 3.5-2M8.5 15.5 9.6 11 17.1 3.5a2.5 2.5 0 0 1 3.5 3.5L13.1 14.5zM9.6 11l3.5 3.5"))],
        [tool_highlight] =
        [
            Fill(Rectangle(1.5, 19.25, 12, 3, 1.5), brush: HighlighterInk),
            Stroke(Path("M10.95 7.5 5.85 12.6v2.55h7.65l2.55-2.55M22 8.35l-3.91 3.91a1.7 1.7 0 0 1-2.38 0l-4.42-4.42a1.7 1.7 0 0 1 0-2.38L15.2 1.55"))
        ],
        [tool_image] =
        [
            Stroke(Rectangle(3, 3, 18, 18, 3)),
            Stroke(Path("m3 16.5 5.5-5.5 9.5 9.5M14 16.5l3-3 4 4")),
            Fill(Circle(15.5, 8.5, 1.75))
        ],
        [tool_line] = [Stroke(Path("M4 20 20 4"))],
        [tool_magnify] =
        [
            Stroke(Circle(10, 10, 7)),
            Stroke(Path("m20.5 20.5-5.5-5.5"), 3),
            Stroke(Path("M10 7v6M7 10h6"))
        ],
        [tool_pixelate] = Pixels(),
        [tool_rectangle] = [Stroke(Rectangle(3, 5, 18, 14, 2))],
        // Lucide's scan brackets with longer arms.
        [tool_region] = [Stroke(Path("M3 8.5V5a2 2 0 0 1 2-2h3.5M15.5 3H19a2 2 0 0 1 2 2v3.5M21 15.5V19a2 2 0 0 1-2 2h-3.5M8.5 21H5a2 2 0 0 1-2-2v-3.5"))],
        [tool_select] =
        [
            Stroke(Path("M13 3H5a2 2 0 0 0-2 2v8"), dashes: [3.5, 3]),
            Fill(Path("M10.5 10.5 21 14.6l-4.4 1.9-1.9 4.5z"), outline: 1.5)
        ],
        [tool_smart_eraser] =
        [
            Stroke(Path("M10 21.2 6.56 17.76c-0.8-0.8-0.8-2 0-2.72l7.68-7.68c0.8-0.8 2-0.8 2.72 0l4.48 4.48c0.8 0.8 0.8 2 0 2.72L14.8 21.2M22 21.2H10M8.4 13.2l7.2 7.2"), 1.92),
            Fill(Path("M5.5 0.5Q6.5 4.5 10.5 5.5Q6.5 6.5 5.5 10.5Q4.5 6.5 0.5 5.5Q4.5 4.5 5.5 0.5z"))
        ],
        [tool_speech_balloon] = [Stroke(Path("M6 3h12a4 4 0 0 1 4 4v5a4 4 0 0 1-4 4h-6.5L6 20.5V16a4 4 0 0 1-4-4V7a4 4 0 0 1 4-4z"))],
        // A stage light casting a beam onto the floor.
        [tool_spotlight] =
        [
            Fill(Rectangle(9, 2.5, 6, 4, 1.25)),
            Fill(Path("M9.5 6.5 4 18.5h16L14.5 6.5z"), 0.3),
            Stroke(new EllipseGeometry(new Rect(4, 16, 16, 5)))
        ],
        // A badge with the numeral cut out.
        [tool_step] = [Fill(Path("M1.5 12a10.5 10.5 0 1 0 21 0a10.5 10.5 0 1 0-21 0zM13.6 6.6v10.6h-2.2V9.5l-1.2.8-1.1-1.8 2.9-1.9z"))],
        [tool_text] = [Fill(Path("M3.5 3h17v5.5H19l-1-3h-4.5v13.25l2.5.75V21H8v-1.5l2.5-.75V5.5H6l-1 3H3.5z"))]
    };

    // A 4x4 checker of cells, each diagonal in its own shade.
    private static Shape[] Pixels()
    {
        double[] opacities = [1, 0.6, 0.8, 0.3];
        double[] positions = [3, 7.83, 12.67, 17.5];
        List<Shape> cells = [];

        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < 4; column++)
            {
                cells.Add(Fill(Rectangle(positions[column], positions[row], 3.5, 3.5, 0.6), opacities[(column - row + 4) % 4]));
            }
        }

        return [.. cells];
    }

    private static Geometry Path(string data) => Geometry.Parse(data);

    private static Geometry Circle(double centerX, double centerY, double radius) =>
        new EllipseGeometry(new Rect(centerX - radius, centerY - radius, radius * 2, radius * 2));

    private static Geometry Rectangle(double x, double y, double width, double height, double radius) =>
        new RectangleGeometry(new Rect(x, y, width, height), radius, radius);

    private static Shape Stroke(Geometry geometry, double width = 2, double[]? dashes = null) =>
        new(geometry, filled: false, width, dashes, opacity: 1, brush: null);

    // The outline, in the fill's color, rounds the corners of small filled shapes such as arrowheads.
    private static Shape Fill(Geometry geometry, double opacity = 1, IBrush? brush = null, double outline = 0) =>
        new(geometry, filled: true, outline, dashes: null, opacity, brush);

    /// <summary>A geometry filled and/or stroked in one color: the Foreground unless the shape has its own brush.</summary>
    internal sealed class Shape(Geometry geometry, bool filled, double strokeWidth, double[]? dashes, double opacity, IBrush? brush)
    {
        // Avalonia dash lengths are multiples of the stroke width; SVG's are absolute.
        private readonly IDashStyle? _dashStyle = dashes == null ? null : new ImmutableDashStyle(dashes.Select(length => length / strokeWidth), 0);

        public void Draw(DrawingContext context, IBrush foreground)
        {
            IBrush color = brush ?? foreground;
            IPen? pen = strokeWidth > 0 ? new Pen(color, strokeWidth, _dashStyle, PenLineCap.Round, PenLineJoin.Round) : null;

            using (context.PushOpacity(opacity))
            {
                context.DrawGeometry(filled ? color : null, pen, geometry);
            }
        }
    }
}
