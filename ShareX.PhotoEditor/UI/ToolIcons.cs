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

using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ShareX.PhotoEditor.Tools;

namespace ShareX.PhotoEditor.UI;

/// <summary>
/// Original line-art icons for the Photo editor, drawn on a 24x24 grid; tool icons use the 20x20 grid in ToolIcons.Tools.cs.
/// Entries starting with "F:" are filled shapes; others are stroked.
/// </summary>
public static partial class ToolIcons
{
    private static readonly Dictionary<string, string[]> Icons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["zoom-in"] = ["M10 3 A7 7 0 1 0 10.01 3 Z", "M15 15 L21 21", "M7 10 H13 M10 7 V13"],
        ["zoom-out"] = ["M10 3 A7 7 0 1 0 10.01 3 Z", "M15 15 L21 21", "M7 10 H13"],
        ["sel-new"] = ["M5 5 H19 V19 H5 Z"],
        ["sel-add"] = ["M3 3 H14 V14 H3 Z", "M10 10 H21 V21 H10 Z"],
        ["sel-sub"] = ["M3 3 H14 V14 H3 Z", "M10 14 V21 H21 V10 H14"],
        ["sel-int"] = ["M3 3 H14 V14 H3 Z", "M10 10 H21 V21 H10 Z", "F:M10 10 H14 V14 H10 Z"],
        ["brush-tip"] = ["F:M12 6 a6 6 0 1 0 0.01 0 Z"],
        ["airbrush"] = ["M4 14 L12 6 L16 10 L8 18 Z", "M14 8 L18 4", "M18 12 h1 M20 9 h1 M20 15 h1"],
        ["gradient-linear"] = ["M3 4 H21 V20 H3 Z", "F:M3 4 H9 V20 H3 Z"],
        ["gradient-radial"] = ["M3 4 H21 V20 H3 Z", "F:M12 8 a4 4 0 1 0 0.01 0 Z"],
        ["gradient-angle"] = ["M3 4 H21 V20 H3 Z", "F:M12 12 L12 4 L21 4 L21 12 Z"],
        ["gradient-reflected"] = ["M3 4 H21 V20 H3 Z", "F:M10 4 H14 V20 H10 Z"],
        ["gradient-diamond"] = ["M3 4 H21 V20 H3 Z", "F:M12 7 L17 12 L12 17 L7 12 Z"],
        ["align-left"] = ["M4 3 V21", "M7 7 H17 V11 H7 Z", "M7 14 H13 V18 H7 Z"],
        ["align-hcenter"] = ["M12 3 V21", "M6 7 H18 V11 H6 Z", "M8 14 H16 V18 H8 Z"],
        ["align-right"] = ["M20 3 V21", "M7 7 H17 V11 H7 Z", "M11 14 H17 V18 H11 Z"],
        ["align-top"] = ["M3 4 H21", "M7 7 H11 V17 H7 Z", "M14 7 H18 V13 H14 Z"],
        ["align-vcenter"] = ["M3 12 H21", "M7 6 H11 V18 H7 Z", "M14 8 H18 V16 H14 Z"],
        ["align-bottom"] = ["M3 20 H21", "M7 7 H11 V17 H7 Z", "M14 11 H18 V17 H14 Z"],
        ["type-orientation"] = ["M4 6 H12 M8 6 V14", "M15 10 V20 M13 18 L15 20 L17 18", "M14 4 H20 L18 2 M20 4 L18 6"],
        ["align-text-left"] = ["M4 6 H20 M4 10 H14 M4 14 H20 M4 18 H12"],
        ["align-text-center"] = ["M4 6 H20 M7 10 H17 M4 14 H20 M8 18 H16"],
        ["align-text-right"] = ["M4 6 H20 M10 10 H20 M4 14 H20 M12 18 H20"],
        ["faux-bold"] = ["M7 4 H13 C16 4 16 11 13 11 H7 Z M7 11 H14 C18 11 18 20 14 20 H7 Z"],
        ["faux-italic"] = ["M10 4 H18 M6 20 H14 M14 4 L10 20"],
        ["underline"] = ["M7 4 V11 C7 16 17 16 17 11 V4", "M5 20 H19"],
        ["strikethrough"] = ["M17 6 C16 4 8 3 8 8 C8 13 17 11 17 16 C17 21 8 20 7 18", "M4 12 H20"],
        ["all-caps"] = ["M3 18 L7 6 L11 18 M4.5 14 H9.5", "M13 18 L17 6 L21 18 M14.5 14 H19.5"],
        ["frame-rect"] = ["M4 4 H20 V20 H4 Z", "M4 4 L20 20 M20 4 L4 20"],
        ["frame-ellipse"] = ["M12 4 C17 4 21 7.5 21 12 C21 16.5 17 20 12 20 C7 20 3 16.5 3 12 C3 7.5 7 4 12 4 Z", "M6 6 L18 18 M18 6 L6 18"],
        ["eye"] = ["M2 12 C5 6 19 6 22 12 C19 18 5 18 2 12 Z", "M12 9 a3 3 0 1 0 0.01 0 Z"],
        ["lock"] = ["M6 11 H18 V20 H6 Z", "M8 11 V8 C8 4 16 4 16 8 V11"],
        ["link"] = ["M10 14 L14 10", "M8 12 L6 14 C4 16 8 20 10 18 L12 16", "M16 12 L18 10 C20 8 16 4 14 6 L12 8"],
        ["fx"] = ["M10 4 C7 4 7 6 7 8 V20 M4 10 H11", "M13 11 L20 20 M20 11 L13 20"],
        ["mask"] = ["M4 4 H20 V20 H4 Z", "F:M12 8 a4 4 0 1 0 0.01 0 Z"],
        ["adjust"] = ["M12 3 a9 9 0 1 0 0.01 0 Z", "F:M12 3 a9 9 0 0 0 0 18 Z"],
        ["folder"] = ["M3 6 H9 L11 8 H21 V19 H3 Z"],
        ["tile-view"] = ["M4 4 H10 V10 H4 Z", "M14 4 H20 V10 H14 Z", "M4 14 H10 V20 H4 Z", "M14 14 H20 V20 H14 Z"],
        ["list-view"] = ["M4 6 H20", "M4 12 H20", "M4 18 H20"],
        ["new-layer"] = ["M5 5 H17 V17 H5 Z", "M8 8 H20 V20 H8"],
        ["trash"] = ["M4 6 H20 M9 6 V3 H15 V6 M6 6 L7 21 H17 L18 6", "M10 10 V17 M14 10 V17"],
        ["chevron-down"] = ["M6 9 L12 15 L18 9"],
        ["chevron-right"] = ["M9 6 L15 12 L9 18"],
        ["close"] = ["M6 6 L18 18 M18 6 L6 18"],
        ["adj-brightnesscontrast"] = ["M12 7 a5 5 0 1 0 0.01 0 Z", "F:M12 7 a5 5 0 0 0 0 10 Z", "M12 2 V4 M12 20 V22 M2 12 H4 M20 12 H22 M5 5 L6.5 6.5 M17.5 17.5 L19 19 M5 19 L6.5 17.5 M17.5 6.5 L19 5"],
        ["adj-levels"] = ["M3 20 H21", "F:M4 20 V16 H6 V20 Z M7 20 V10 H9 V20 Z M10 20 V6 H12 V20 Z M13 20 V9 H15 V20 Z M16 20 V14 H18 V20 Z M19 20 V17 H21 V20 Z"],
        ["adj-curves"] = ["M3 3 H21 V21 H3 Z", "M3 21 C9 21 9 3 21 3"],
        ["adj-exposure"] = ["M3 3 H21 V21 H3 Z", "M3 21 L21 3", "M6 8 H10 M8 6 V10 M14 16 H18"],
        ["adj-vibrance"] = ["M12 4 L21 20 H3 Z", "M8 15 L16 15"],
        ["adj-huesaturation"] = ["F:M3 5 H21 V9 H3 Z", "M3 12 H21 V16 H3 Z", "M3 19 H21"],
        ["adj-colorbalance"] = ["M12 4 V20 M5 20 H19", "M4 9 L8 5 L12 9 M12 9 L16 5 L20 9", "M4 9 C4 12 8 12 8 9 M16 9 C16 12 20 12 20 9"],
        ["adj-blackandwhite"] = ["M4 4 H20 V20 H4 Z", "F:M4 20 L20 4 V20 Z"],
        ["adj-photofilter"] = ["M4 7 H8 L10 5 H14 L16 7 H20 V19 H4 Z", "F:M12 9 a4 4 0 1 0 0.01 0 Z"],
        ["adj-channelmixer"] = ["M9 5 a5 5 0 1 0 0.01 0 Z", "M15 5 a5 5 0 1 0 0.01 0 Z", "M12 11 a5 5 0 1 0 0.01 0 Z"],
        ["adj-invert"] = ["M4 4 H20 V20 H4 Z", "F:M4 4 H12 V20 H4 Z", "M14 9 a3 3 0 1 0 0.01 0 Z"],
        ["adj-posterize"] = ["M3 20 H8 V15 H13 V10 H18 V5 H21"],
        ["adj-threshold"] = ["M4 4 H20 V20 H4 Z", "F:M4 4 H20 V12 H4 Z"],
        ["adj-gradientmap"] = ["M3 7 H21 V17 H3 Z", "F:M3 7 H9 V17 H3 Z", "M12 7 V17"],
        ["adj-selectivecolor"] = ["M12 3 a9 9 0 1 0 0.01 0 Z", "M12 8 a4 4 0 1 0 0.01 0 Z", "F:M12 11 a1 1 0 1 0 0.01 0 Z"],
        ["adj-desaturate"] = ["M12 3 a9 9 0 1 0 0.01 0 Z", "M5 19 L19 5"],
        ["adj-colorlookup"] = ["M4 4 H20 V20 H4 Z", "M4 9.3 H20 M4 14.6 H20 M9.3 4 V20 M14.6 4 V20"],
        ["web-color"] = ["M12 3 L21 8 V16 L12 21 L3 16 V8 Z", "M3 8 L12 13 L21 8", "M12 13 V21"],
        ["clip-arrow"] = ["M6 4 V14 H18", "M15 11 L18 14 L15 17"],
        ["smart-object"] = ["M4 4 H20 V20 H4 Z", "M14 14 H20 V20 H14 Z"],
        ["lock-transparency"] = ["M4 4 H20 V20 H4 Z", "F:M4 4 H12 V12 H4 Z", "F:M12 12 H20 V20 H12 Z"],
        ["lock-pixels"] = ["M4 20 L5 16 L15 6 L18 9 L8 19 Z", "M13 8 L16 11"],
        ["lock-position"] = ["M12 3 V21 M3 12 H21", "M12 3 L9.5 5.5 M12 3 L14.5 5.5 M12 21 L9.5 18.5 M12 21 L14.5 18.5 M3 12 L5.5 9.5 M3 12 L5.5 14.5 M21 12 L18.5 9.5 M21 12 L18.5 14.5"],
        ["toolbar-expand"] = ["M5 7 L10 12 L5 17 M13 7 L18 12 L13 17"],
        ["toolbar-collapse"] = ["M11 7 L6 12 L11 17 M19 7 L14 12 L19 17"],
        ["plus"] = ["M12 5 V19 M5 12 H19"],
        ["snapshot"] = ["M4 7 H8 L10 5 H14 L16 7 H20 V19 H4 Z", "M12 10 a3 3 0 1 0 0.01 0 Z"],
        ["new-document"] = ["M6 3 H14 L18 7 V21 H6 Z", "M14 3 V7 H18"],
        ["selection"] = ["M4 4 H8 M10 4 H14 M16 4 H20 V8 M20 10 V14 M20 16 V20 H16 M14 20 H10 M8 20 H4 V16 M4 14 V10 M4 8 V4"],
        ["path-fill"] = ["F:M5 5 H19 V19 H5 Z"],
        ["path-stroke"] = ["M5 5 H19 V19 H5 Z"],
        ["clipboard"] = ["M8 5 H6 V21 H18 V5 H16", "M9 3 H15 V7 H9 Z"],
        ["capture-region"] = ["M3 8 V3 H8 M16 3 H21 V8 M21 16 V21 H16 M8 21 H3 V16", "M8 9 H16 V15 H8 Z"],
        ["capture-screen"] = ["M3 4 H21 V16 H3 Z", "M9 20 H15 M12 16 V20"]
    };

    public static string KeyFor(ToolId id) => id.ToString().ToLowerInvariant();

    public static Control Create(ToolId id, double size = 20) => Create(KeyFor(id), size);

    public static Control Create(string key, double size = 20)
    {
        // Tool glyphs are drawn on a 20 px grid with 1 px strokes on pixel centers, so they are sharp at 20 px.
        bool tool = ToolGlyphs.TryGetValue(key, out string[]? parts);
        if (!tool && !Icons.TryGetValue(key, out parts))
        {
            parts = ["M5 5 H19 V19 H5 Z"];
        }
        double grid = tool ? 20 : 24;
        Canvas canvas = new() { Width = grid, Height = grid };
        foreach (string part in parts!)
        {
            // "F:" filled, "H:" half-tone fill, "D:" dashed stroke, otherwise a solid stroke.
            string kind = part.Length > 2 && part[1] == ':' ? part[..2] : string.Empty;
            bool filled = kind is "F:" or "H:";
            Avalonia.Controls.Shapes.Path path = new()
            {
                Data = Geometry.Parse(kind.Length > 0 ? part[2..] : part),
                StrokeThickness = filled ? 0 : tool ? 1 : 1.6,
                StrokeLineCap = tool ? PenLineCap.Flat : PenLineCap.Round,
                StrokeJoin = tool ? PenLineJoin.Miter : PenLineJoin.Round,
                Opacity = kind == "H:" ? 0.45 : 1
            };
            if (kind == "D:") path.StrokeDashArray = [2, 2];
            if (filled) path.Bind(Shape.FillProperty, path.GetResourceObservable("ShareX.Brush.Text"));
            else path.Bind(Shape.StrokeProperty, path.GetResourceObservable("ShareX.Brush.Text"));
            canvas.Children.Add(path);
        }
        return new Viewbox { Width = size, Height = size, Child = canvas };
    }
}
