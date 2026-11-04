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
/// Original line-art icons for the Photo editor. Each set is drawn on the pixel grid it is shown at (tools 20 px in
/// ToolIcons.Tools.cs, interface icons 16 px, badges and chevrons 12 or 10 px) with 1 px strokes on pixel centers,
/// so icons are sharp at their native size. Parts starting with "F:" are filled, "H:" are half-tone fills,
/// "D:" are dashed strokes and the rest are solid strokes.
/// </summary>
public static partial class ToolIcons
{
    private const string LayerBrushHandle = "F:M13.5 1 L15 2.5 L9 8.5 L7.5 7 Z";
    private const string LayerBrushTip = "F:M6.5 8 L8 9.5 C8 12.5 5.5 15 1 15 C1 11.5 3 8 6.5 8 Z";
    private const string GradientFrame = "M1.5 2.5 H14.5 V13.5 H1.5 Z";
    private const string Magnifier = "M1.5 6.5 A5 5 0 1 0 11.5 6.5 A5 5 0 1 0 1.5 6.5 Z";
    private const string MagnifierHandle = "F:M10 11 L11 10 L15.5 14.5 L14.5 15.5 Z";
    private const string AdjustmentFrame = "M1.5 1.5 H14.5 V14.5 H1.5 Z";

    /// <summary>Interface icons on a 16 px grid: options bar, panel footers, panel rows and the start screen.</summary>
    private static readonly Dictionary<string, string[]> Icons16 = new(StringComparer.OrdinalIgnoreCase)
    {
        ["zoom-in"] = [Magnifier, MagnifierHandle, "M4 6.5 H9 M6.5 4 V9"],
        ["zoom-out"] = [Magnifier, MagnifierHandle, "M4 6.5 H9"],
        ["sel-new"] = ["M2.5 2.5 H13.5 V13.5 H2.5 Z", "H:M3 3 H13 V13 H3 Z"],
        ["sel-add"] = ["M1.5 1.5 H10.5 V5.5 H14.5 V14.5 H5.5 V10.5 H1.5 Z", "H:M2 2 H10 V6 H14 V14 H6 V10 H2 Z"],
        ["sel-sub"] = ["M1.5 1.5 H10.5 V10.5 H1.5 Z", "M5.5 5.5 H14.5 V14.5 H5.5 Z", "H:M2 2 H10 V5 H5 V10 H2 Z"],
        ["sel-int"] = ["M1.5 1.5 H10.5 V10.5 H1.5 Z", "M5.5 5.5 H14.5 V14.5 H5.5 Z", "H:M6 6 H10 V10 H6 Z"],
        ["brush-tip"] = ["F:M2.5 8 A5.5 5.5 0 1 0 13.5 8 A5.5 5.5 0 1 0 2.5 8 Z"],
        ["airbrush"] = ["M2.5 6.5 H9.5 V14.5 H2.5 Z", "F:M4 3 H8 V6 H4 Z", "F:M10 3 H11 V4 H10 Z M12 1 H13 V2 H12 Z M12 4 H13 V5 H12 Z M14 2 H15 V3 H14 Z M14 6 H15 V7 H14 Z M12 7 H13 V8 H12 Z"],
        ["gradient-linear"] = [GradientFrame, "F:M2 3 H6 V13 H2 Z", "H:M6 3 H10 V13 H6 Z"],
        ["gradient-radial"] = [GradientFrame, "H:M3.5 8 A4.5 4.5 0 1 0 12.5 8 A4.5 4.5 0 1 0 3.5 8 Z", "F:M6 8 A2 2 0 1 0 10 8 A2 2 0 1 0 6 8 Z"],
        ["gradient-angle"] = [GradientFrame, "F:M8 8 V3 H14 V8 Z", "H:M8 8 H14 V13 H8 Z"],
        ["gradient-reflected"] = [GradientFrame, "F:M7 3 H9 V13 H7 Z", "H:M4 3 H7 V13 H4 Z M9 3 H12 V13 H9 Z"],
        ["gradient-diamond"] = [GradientFrame, "H:M8 3 L13 8 L8 13 L3 8 Z", "F:M8 5.5 L10.5 8 L8 10.5 L5.5 8 Z"],
        ["align-left"] = ["M1.5 1 V15", "F:M3 3 H13 V7 H3 Z M3 9 H9 V13 H3 Z"],
        ["align-hcenter"] = ["M7.5 1 V15", "F:M2 3 H13 V7 H2 Z M4 9 H11 V13 H4 Z"],
        ["align-right"] = ["M14.5 1 V15", "F:M3 3 H13 V7 H3 Z M7 9 H13 V13 H7 Z"],
        ["align-top"] = ["M1 1.5 H15", "F:M3 3 H7 V13 H3 Z M9 3 H13 V9 H9 Z"],
        ["align-vcenter"] = ["M1 7.5 H15", "F:M3 2 H7 V13 H3 Z M9 4 H13 V11 H9 Z"],
        ["align-bottom"] = ["M1 14.5 H15", "F:M3 3 H7 V13 H3 Z M9 7 H13 V13 H9 Z"],
        ["type-orientation"] = ["F:M1 1 H9 V3 H6 V10 H4 V3 H1 Z", "M6 13.5 H10.5 C12.5 13.5 13.5 12.5 13.5 10.5 V7", "F:M13.5 4 L16 7.5 H11 Z"],
        ["align-text-left"] = ["M1 3.5 H15 M1 6.5 H10 M1 9.5 H15 M1 12.5 H8"],
        ["align-text-center"] = ["M1 3.5 H15 M4 6.5 H12 M1 9.5 H15 M5 12.5 H11"],
        ["align-text-right"] = ["M1 3.5 H15 M6 6.5 H15 M1 9.5 H15 M8 12.5 H15"],
        ["faux-bold"] = ["F:M3 2 H8.5 C11 2 12 3.2 12 4.8 C12 6 11.3 6.9 10.3 7.3 C11.7 7.7 12.7 8.8 12.7 10.4 C12.7 12.5 11.2 14 8.5 14 H3 Z M5.5 4 V6.8 H8 C9 6.8 9.5 6.2 9.5 5.4 C9.5 4.6 9 4 8 4 Z M5.5 8.8 V12 H8.3 C9.5 12 10.1 11.3 10.1 10.4 C10.1 9.5 9.5 8.8 8.3 8.8 Z"],
        ["faux-italic"] = ["F:M6 2 H13 V4 H10.8 L8.2 12 H10 V14 H3 V12 H5.2 L7.8 4 H6 Z"],
        ["underline"] = ["M4.5 2 V7.5 C4.5 11 11.5 11 11.5 7.5 V2", "M3 14.5 H13"],
        ["strikethrough"] = ["M11.5 3.5 C10.5 2 5 1.5 5 5 C5 8.5 11.5 7 11.5 11 C11.5 14.5 5.5 14 4.5 12.5", "M2 8.5 H14"],
        ["all-caps"] = ["F:M1 3 H9 V5 H6 V14 H4 V5 H1 Z M9 7 H15 V9 H13 V14 H11 V9 H9 Z"],
        ["frame-rect"] = ["M2.5 2.5 H13.5 V13.5 H2.5 Z", "M2.5 2.5 L13.5 13.5 M13.5 2.5 L2.5 13.5"],
        ["frame-ellipse"] = ["M1.5 8 A6.5 6.5 0 1 0 14.5 8 A6.5 6.5 0 1 0 1.5 8 Z", "M3.5 3.5 L12.5 12.5 M12.5 3.5 L3.5 12.5"],
        ["eye"] = ["M0.5 8 C3 3.5 13 3.5 15.5 8 C13 12.5 3 12.5 0.5 8 Z", "F:M5.5 8 A2.5 2.5 0 1 0 10.5 8 A2.5 2.5 0 1 0 5.5 8 Z"],
        ["lock"] = ["F:M3 7 H13 V14 H3 Z M7 9 H9 V12 H7 Z", "M5.5 7 V5 C5.5 2 10.5 2 10.5 5 V7"],
        ["mask"] = ["M1.5 2.5 H14.5 V13.5 H1.5 Z", "F:M5 8 A3 3 0 1 0 11 8 A3 3 0 1 0 5 8 Z"],
        ["adjust"] = ["M1.5 8 A6.5 6.5 0 1 0 14.5 8 A6.5 6.5 0 1 0 1.5 8 Z", "F:M8 1.5 A6.5 6.5 0 0 0 8 14.5 Z"],
        ["folder"] = ["M1.5 2.5 H6 L7.5 4.5 H14.5 V13.5 H1.5 Z", "M1.5 6.5 H14.5"],
        ["tile-view"] = ["M1.5 1.5 H6.5 V6.5 H1.5 Z M9.5 1.5 H14.5 V6.5 H9.5 Z M1.5 9.5 H6.5 V14.5 H1.5 Z M9.5 9.5 H14.5 V14.5 H9.5 Z"],
        ["list-view"] = ["F:M1 3 H4 V6 H1 Z M1 7 H4 V10 H1 Z M1 11 H4 V14 H1 Z", "M6 4.5 H15 M6 8.5 H15 M6 12.5 H15"],
        ["new-layer"] = ["M2.5 2.5 H12.5 V12.5 H2.5 Z", "M7.5 5 V10 M5 7.5 H10"],
        ["trash"] = ["M1.5 3.5 H13.5", "M5.5 3 V1.5 H9.5 V3", "M3.5 4 V14.5 H11.5 V4", "M6.5 6 V12 M8.5 6 V12"],
        ["plus"] = ["M7.5 2 V13 M2 7.5 H13"],
        ["snapshot"] = ["M1.5 4.5 H4.5 L6 2.5 H10 L11.5 4.5 H14.5 V13.5 H1.5 Z", "M5 9 A3 3 0 1 0 11 9 A3 3 0 1 0 5 9 Z"],
        ["new-document"] = ["M3.5 1.5 H9.5 L12.5 4.5 V14.5 H3.5 Z", "M9.5 1.5 V4.5 H12.5"],
        ["selection"] = ["M1 1.5 H4 M6 1.5 H10 M12 1.5 H15 M1 14.5 H4 M6 14.5 H10 M12 14.5 H15 M1.5 2 V4 M1.5 6 V10 M1.5 12 V14 M14.5 2 V4 M14.5 6 V10 M14.5 12 V14"],
        ["path-fill"] = ["F:M2.5 12.5 C3 6 6 3 12.5 2.5 L13.5 13.5 Z", "F:M1 11 H4 V14 H1 Z M11 1 H14 V4 H11 Z M12 12 H15 V15 H12 Z"],
        ["path-stroke"] = ["M2.5 12.5 C3 6 6 3 12.5 2.5 L13.5 13.5 Z", "F:M1 11 H4 V14 H1 Z M11 1 H14 V4 H11 Z M12 12 H15 V15 H12 Z"],
        ["clipboard"] = ["M3.5 2.5 H12.5 V14.5 H3.5 Z", "F:M6 1 H10 V4 H6 Z", "M6 7.5 H10 M6 10.5 H10"],
        ["capture-region"] = ["M1.5 5 V1.5 H5 M11 1.5 H14.5 V5 M14.5 11 V14.5 H11 M5 14.5 H1.5 V11", "M7.5 5 V10 M5 7.5 H10"],
        ["capture-screen"] = ["M1.5 2.5 H14.5 V11.5 H1.5 Z", "M7.5 12 V14 M5 14.5 H11"],
        ["web-color"] = ["M1.5 7.5 A6 6 0 1 0 13.5 7.5 A6 6 0 1 0 1.5 7.5 Z", "M1.5 7.5 H13.5", "M7.5 1.5 C4.5 4.5 4.5 10.5 7.5 13.5 C10.5 10.5 10.5 4.5 7.5 1.5"],
        ["lock-transparency"] = ["M1.5 1.5 H14.5 V14.5 H1.5 Z", "F:M2 2 H5 V5 H2 Z M8 2 H11 V5 H8 Z M5 5 H8 V8 H5 Z M11 5 H14 V8 H11 Z M2 8 H5 V11 H2 Z M8 8 H11 V11 H8 Z M5 11 H8 V14 H5 Z M11 11 H14 V14 H11 Z"],
        ["lock-pixels"] = [LayerBrushHandle, LayerBrushTip],
        ["lock-position"] = ["M7.5 3 V12 M3 7.5 H12", "F:M7.5 0.5 L10.5 3.5 H4.5 Z M7.5 14.5 L10.5 11.5 H4.5 Z M0.5 7.5 L3.5 4.5 V10.5 Z M14.5 7.5 L11.5 4.5 V10.5 Z"],

        // Adjustments panel and adjustment layer menus.
        ["adj-brightnesscontrast"] = ["M4 7.5 A3.5 3.5 0 1 0 11 7.5 A3.5 3.5 0 1 0 4 7.5 Z", "F:M7.5 4 A3.5 3.5 0 0 0 7.5 11 Z", "M7.5 0 V2 M7.5 13 V15 M0 7.5 H2 M13 7.5 H15 M2.5 2.5 L3.5 3.5 M11.5 11.5 L12.5 12.5 M2.5 12.5 L3.5 11.5 M11.5 3.5 L12.5 2.5"],
        ["adj-levels"] = ["M0 14.5 H16", "F:M1 14 V10 H3 V14 Z M4 14 V5 H6 V14 Z M7 14 V2 H9 V14 Z M10 14 V7 H12 V14 Z M13 14 V11 H15 V14 Z"],
        ["adj-curves"] = [AdjustmentFrame, "M2 14 C7 14 9 2 14 2"],
        ["adj-exposure"] = [AdjustmentFrame, "M14.5 1.5 L1.5 14.5", "M3 5.5 H8 M5.5 3 V8", "M8 11.5 H13"],
        ["adj-vibrance"] = ["M7.5 1.5 L14 13.5 H1 Z", "H:M7.5 1.5 L14 13.5 H7.5 Z"],
        ["adj-huesaturation"] = ["M1 3.5 H15 M1 8.5 H15 M1 13.5 H15", "F:M3 2 H6 V5 H3 Z M9 7 H12 V10 H9 Z M5 12 H8 V15 H5 Z"],
        ["adj-colorbalance"] = ["M1 3.5 H14", "M7.5 2 V14 M4 14.5 H11", "M2.5 4 L0.5 9 M2.5 4 L4.5 9 M12.5 4 L10.5 9 M12.5 4 L14.5 9", "F:M0 9 H5 C5 11 0 11 0 9 Z M10 9 H15 C15 11 10 11 10 9 Z"],
        ["adj-blackandwhite"] = [AdjustmentFrame, "F:M2 2 H8 V14 H2 Z"],
        ["adj-photofilter"] = ["M1.5 8 A6.5 6.5 0 1 0 14.5 8 A6.5 6.5 0 1 0 1.5 8 Z", "H:M2 8 A6 6 0 1 0 14 8 A6 6 0 1 0 2 8 Z", "F:M4.5 6 A1.5 1.5 0 1 0 7.5 6 A1.5 1.5 0 1 0 4.5 6 Z"],
        ["adj-channelmixer"] = ["M1.5 5.5 A3.5 3.5 0 1 0 8.5 5.5 A3.5 3.5 0 1 0 1.5 5.5 Z", "M6.5 5.5 A3.5 3.5 0 1 0 13.5 5.5 A3.5 3.5 0 1 0 6.5 5.5 Z", "M4 10 A3.5 3.5 0 1 0 11 10 A3.5 3.5 0 1 0 4 10 Z"],
        ["adj-invert"] = [AdjustmentFrame, "F:M2 2 H14 L2 14 Z"],
        ["adj-posterize"] = ["F:M1 14 V11 H4 V8 H7 V5 H10 V2 H14 V14 Z"],
        ["adj-threshold"] = [AdjustmentFrame, "F:M2 6 H6 V10 H10 V14 H2 Z"],
        ["adj-gradientmap"] = ["M1.5 9.5 H14.5 V14.5 H1.5 Z", "F:M2 10 H6 V14 H2 Z", "H:M6 10 H10 V14 H6 Z", "M7.5 1 V5", "F:M7.5 8 L10 5 H5 Z"],
        ["adj-selectivecolor"] = ["M2 7.5 A5.5 5.5 0 1 0 13 7.5 A5.5 5.5 0 1 0 2 7.5 Z", "F:M5.5 7.5 A2 2 0 1 0 9.5 7.5 A2 2 0 1 0 5.5 7.5 Z", "M7.5 0 V2 M7.5 13 V15 M0 7.5 H2 M13 7.5 H15"],
        ["adj-desaturate"] = ["M7.5 1 C7.5 1 3 6.5 3 9.5 C3 12 5 14 7.5 14 C10 14 12 12 12 9.5 C12 6.5 7.5 1 7.5 1 Z", "M1.5 14.5 L14.5 1.5"],
        ["adj-colorlookup"] = [AdjustmentFrame, "M5.5 2 V14 M10.5 2 V14 M2 5.5 H14 M2 10.5 H14", "H:M6 6 H10 V10 H6 Z"]
    };

    /// <summary>Layer badges and toolbar toggles on a 12 px grid.</summary>
    private static readonly Dictionary<string, string[]> Icons12 = new(StringComparer.OrdinalIgnoreCase)
    {
        ["link"] = ["M5.5 3.5 L7 2 C8.5 0.5 11.5 3.5 10 5 L8.5 6.5", "M6.5 8.5 L5 10 C3.5 11.5 0.5 8.5 2 7 L3.5 5.5", "M4.5 7.5 L7.5 4.5"],
        ["fx"] = ["M7 1.5 C5.5 1.5 4.5 2.2 4.5 4 V11", "M2.5 5.5 H7", "M7 6.5 L11 11 M11 6.5 L7 11"],
        ["smart-object"] = ["M1.5 1.5 H10.5 V10.5 H1.5 Z", "F:M6 6 H11 V11 H6 Z"],
        ["clip-arrow"] = ["M3.5 1 V6.5 H8", "F:M8 4 L11 6.5 L8 9 Z"],
        ["toolbar-expand"] = ["M2 2.5 L5.5 6 L2 9.5 M6.5 2.5 L10 6 L6.5 9.5"],
        ["toolbar-collapse"] = ["M5.5 2.5 L2 6 L5.5 9.5 M10 2.5 L6.5 6 L10 9.5"]
    };

    /// <summary>Chevrons and close crosses on a 10 px grid.</summary>
    private static readonly Dictionary<string, string[]> Icons10 = new(StringComparer.OrdinalIgnoreCase)
    {
        ["chevron-down"] = ["M1 3 L5 7 L9 3"],
        ["chevron-right"] = ["M3 1 L7 5 L3 9"],
        ["close"] = ["M1.5 1.5 L8.5 8.5 M8.5 1.5 L1.5 8.5"]
    };

    public static string KeyFor(ToolId id) => id.ToString().ToLowerInvariant();

    public static Control Create(ToolId id, double size = 20) => Create(KeyFor(id), size);

    public static Control Create(string key, double size = 20)
    {
        (double grid, string[] parts) = ToolGlyphs.TryGetValue(key, out string[]? found) ? (20, found) :
            Icons16.TryGetValue(key, out found) ? (16, found) :
            Icons12.TryGetValue(key, out found) ? (12, found) :
            Icons10.TryGetValue(key, out found) ? (10, found) :
            (16, ["M2.5 2.5 H13.5 V13.5 H2.5 Z"]);
        Canvas canvas = new() { Width = grid, Height = grid };
        foreach (string part in parts)
        {
            string kind = part.Length > 2 && part[1] == ':' ? part[..2] : string.Empty;
            bool filled = kind is "F:" or "H:";
            Avalonia.Controls.Shapes.Path path = new()
            {
                Data = Geometry.Parse(kind.Length > 0 ? part[2..] : part),
                StrokeThickness = filled ? 0 : 1,
                StrokeLineCap = PenLineCap.Flat,
                StrokeJoin = PenLineJoin.Miter,
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
