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
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ShareX.PhotoEditor.Tools;

namespace ShareX.PhotoEditor.UI;

/// <summary>
/// Original line-art icons for the Photo editor, drawn on a 24x24 grid.
/// Entries starting with "F:" are filled shapes; others are stroked.
/// </summary>
public static class ToolIcons
{
    private static readonly Dictionary<string, string[]> Icons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["move"] = ["M12 2 V22 M2 12 H22", "M12 2 L9 5 M12 2 L15 5 M12 22 L9 19 M12 22 L15 19 M2 12 L5 9 M2 12 L5 15 M22 12 L19 9 M22 12 L19 15"],
        ["artboard"] = ["M6 3 V21 M18 3 V21 M3 6 H21 M3 18 H21"],
        ["rectangularmarquee"] = ["M4 5 H7 M10 5 H14 M17 5 H20 V8 M20 11 V13 M20 16 V19 H17 M14 19 H10 M7 19 H4 V16 M4 13 V11 M4 8 V5"],
        ["ellipticalmarquee"] = ["M12 4 C17 4 21 7.5 21 12 C21 16.5 17 20 12 20 C7 20 3 16.5 3 12 C3 7.5 7 4 12 4 Z"],
        ["singlerowmarquee"] = ["M2 12 H5 M8 12 H11 M14 12 H17 M20 12 H22", "M2 9 V15 M22 9 V15"],
        ["singlecolumnmarquee"] = ["M12 2 V5 M12 8 V11 M12 14 V17 M12 20 V22", "M9 2 H15 M9 22 H15"],
        ["lasso"] = ["M6 15 C2 12 3 5 11 4 C18 3 22 7 20 11 C18 15 11 15 8 14 C6 13 6 16 8 18 C9 19 8 21 6 21"],
        ["polygonallasso"] = ["M4 14 L7 4 L20 6 L17 15 L9 13 Z", "M9 13 L8 18 L6 21"],
        ["magneticlasso"] = ["M6 15 C2 12 3 5 11 4 C18 3 22 7 20 11", "M14 14 L14 19 C14 21 18 21 18 19 L18 14", "M14 16 H18"],
        ["objectselection"] = ["M3 7 V3 H7 M17 3 H21 V7 M21 17 V21 H17 M7 21 H3 V17", "M8 16 L12 8 L16 16 Z"],
        ["quickselection"] = ["M4 20 C6 18 8 18 9 16 L16 5 C17 3 20 4 19 6 L12 17 C11 19 8 20 4 20 Z", "M14 2 C17 1 20 2 21 5"],
        ["magicwand"] = ["M4 20 L15 9", "M15 9 L16.5 7.5", "M18 2 V6 M16 4 H20 M20 9 L21 10 M12 3 L13 4"],
        ["selectionbrush"] = ["M5 19 C7 17 8 17 9 15 L17 4 L20 7 L12 18 C10 19 8 20 5 19 Z", "M3 21 H8 M11 21 H16"],
        ["crop"] = ["M6 2 V18 H22", "M2 6 H18 V22"],
        ["perspectivecrop"] = ["M6 4 L18 6 L20 18 L4 20 Z", "M3 2 L6 4 M21 21 L20 18"],
        ["slice"] = ["M5 20 L19 4", "M15 4 L19 4 L19 8", "M3 3 H9 M3 3 V9"],
        ["sliceselect"] = ["M5 3 L5 17 L9 13 L12 20 L14 19 L11 12 L16 12 Z", "M16 3 H21 V8"],
        ["frame"] = ["M4 4 H20 V20 H4 Z", "M4 4 L20 20 M20 4 L4 20"],
        ["eyedropper"] = ["M4 20 L5 16 L14 7 L17 10 L8 19 Z", "M14 7 L16 5 C17 4 19 4 20 5 C21 6 21 8 20 9 L17 10"],
        ["colorsampler"] = ["M4 20 L5 16 L14 7 L17 10 L8 19 Z", "M14 7 L16 5 C17 4 19 4 20 5 C21 6 21 8 20 9 L17 10", "M17 15 V21 M14 18 H20"],
        ["ruler"] = ["M3 16 L16 3 L21 8 L8 21 Z", "M7 12 L9 14 M10 9 L12 11 M13 6 L15 8"],
        ["note"] = ["M4 3 H16 L20 7 V21 H4 Z", "M16 3 V7 H20", "M8 11 H16 M8 15 H14"],
        ["count"] = ["M5 5 H19 V19 H5 Z", "M10 9 L12 7 V17 M10 17 H14"],
        ["spothealingbrush"] = ["M7 13 L13 7 C15 5 19 9 17 11 L11 17 C9 19 5 15 7 13 Z", "M10 10 L14 14", "M3 3 L5 5 M3 7 H5"],
        ["remove"] = ["M7 13 L13 7 C15 5 19 9 17 11 L11 17 C9 19 5 15 7 13 Z", "M3 21 L8 21 M5 18 L5 21", "M16 16 L21 21 M21 16 L16 21"],
        ["healingbrush"] = ["M7 13 L13 7 C15 5 19 9 17 11 L11 17 C9 19 5 15 7 13 Z", "M10 10 L14 14", "M12 12 m-1 0 a1 1 0 1 0 2 0 a1 1 0 1 0 -2 0"],
        ["patch"] = ["M4 8 C4 5 7 4 9 5 L18 9 C21 10 21 14 18 15 L9 19 C6 20 4 18 4 16 Z", "M8 9 L8 15 M12 10 L12 14"],
        ["contentawaremove"] = ["M4 4 H12 V12 H4 Z", "M12 12 L20 20 M20 14 V20 H14"],
        ["redeye"] = ["M2 12 C5 6 19 6 22 12 C19 18 5 18 2 12 Z", "F:M12 9 a3 3 0 1 0 0.01 0 Z"],
        ["brush"] = ["M3 21 C3 17 5 15 8 15 C10 15 11 17 10 19 C9 21 6 21 3 21 Z", "M9 15 L19 4 C20 3 22 5 21 6 L11 17"],
        ["pencil"] = ["M4 20 L5 16 L16 5 L19 8 L8 19 Z", "M14 7 L17 10 M5 16 L8 19"],
        ["colorreplacement"] = ["M3 21 C3 17 5 15 8 15 C10 15 11 17 10 19 C9 21 6 21 3 21 Z", "M9 15 L19 4 C20 3 22 5 21 6 L11 17", "M14 18 A3 3 0 1 0 20 18 A3 3 0 1 0 14 18"],
        ["mixerbrush"] = ["M3 21 C3 17 5 15 8 15 C10 15 11 17 10 19 C9 21 6 21 3 21 Z", "M9 15 L19 4 C20 3 22 5 21 6 L11 17", "M15 15 L21 15 L20 21 L16 21 Z"],
        ["adjustmentbrush"] = ["M3 21 C3 17 5 15 8 15 C10 15 11 17 10 19 C9 21 6 21 3 21 Z", "M9 15 L19 4 C20 3 22 5 21 6 L11 17", "M16 13 a5 5 0 0 1 0 10 Z"],
        ["clonestamp"] = ["M9 3 H15 V9 L18 13 H6 L9 9 Z", "M4 13 H20 V16 H4 Z", "M6 20 H18"],
        ["patternstamp"] = ["M9 3 H15 V9 L18 13 H6 L9 9 Z", "M4 13 H20 V16 H4 Z", "M5 20 H7 M9 20 H11 M13 20 H15 M17 20 H19"],
        ["historybrush"] = ["M3 21 C3 17 5 15 8 15 C10 15 11 17 10 19 C9 21 6 21 3 21 Z", "M9 15 L13 10", "M17 3 a5 5 0 1 0 0.01 0", "M17 5 V8 L19 9"],
        ["arthistorybrush"] = ["M3 21 C3 17 5 15 8 15 C10 15 11 17 10 19 C9 21 6 21 3 21 Z", "M9 15 L13 10", "M14 3 C16 5 18 3 20 5 C18 7 20 9 18 11"],
        ["eraser"] = ["M3 15 L12 6 L18 12 L11 19 H6 Z", "M8 10 L14 16", "M11 19 H21"],
        ["backgrounderaser"] = ["M3 15 L12 6 L18 12 L11 19 H6 Z", "M8 10 L14 16", "M18 2 V6 M16 4 H20"],
        ["magiceraser"] = ["M3 15 L12 6 L18 12 L11 19 H6 Z", "M8 10 L14 16", "M19 2 V4 M18 3 H20 M21 7 L22 8"],
        ["gradient"] = ["M3 4 H21 V20 H3 Z", "F:M3 4 H8 V20 H3 Z", "M12 4 V20"],
        ["paintbucket"] = ["M5 11 L11 5 L19 13 L13 19 Z", "M5 11 H19", "M20 15 C20 15 22 18 22 19 C22 20.1 21.1 21 20 21 C18.9 21 18 20.1 18 19 C18 18 20 15 20 15 Z"],
        ["blur"] = ["M12 3 C12 3 5 11 5 15 C5 19 8 21 12 21 C16 21 19 19 19 15 C19 11 12 3 12 3 Z"],
        ["sharpen"] = ["M12 3 L17 21 H7 Z"],
        ["smudge"] = ["M8 21 V11 C8 9 11 9 11 11 V5 C11 3 14 3 14 5 V11 C14 10 17 10 17 12 V17 C17 19 15 21 13 21 Z"],
        ["dodge"] = ["M12 3 V13", "M7 13 A5 4 0 1 0 17 13 A5 4 0 1 0 7 13 Z"],
        ["burn"] = ["M6 21 C6 15 12 15 12 9 C12 15 18 15 18 21", "M9 21 C9 18 12 18 12 15 C12 18 15 18 15 21"],
        ["sponge"] = ["M4 8 C4 5 20 5 20 8 V17 C20 20 4 20 4 17 Z", "M8 10 h1 M13 9 h1 M10 14 h1 M15 13 h1 M7 16 h1"],
        ["pen"] = ["M12 2 L18 11 L14 21 H10 L6 11 Z", "M12 2 V11", "M12 13 a1.5 1.5 0 1 0 0.01 0"],
        ["freeformpen"] = ["M12 2 L18 11 L14 21 H10 L6 11 Z", "M12 2 V11", "M2 20 C4 16 6 22 8 18"],
        ["curvaturepen"] = ["M12 2 L18 11 L14 21 H10 L6 11 Z", "M12 2 V11", "M2 8 C2 3 8 3 7 7"],
        ["addanchorpoint"] = ["M12 2 L18 11 L14 21 H10 L6 11 Z", "M12 2 V11", "M19 2 V8 M16 5 H22"],
        ["deleteanchorpoint"] = ["M12 2 L18 11 L14 21 H10 L6 11 Z", "M12 2 V11", "M16 5 H22"],
        ["convertpoint"] = ["M3 20 L12 4 L21 20", "M9 9 H15", "F:M10.5 2.5 H13.5 V5.5 H10.5 Z"],
        ["horizontaltype"] = ["M5 5 V3 H19 V5", "M12 3 V21", "M9 21 H15"],
        ["verticaltype"] = ["M4 5 H10 M7 5 V12", "M4 15 H10", "M17 3 V21 M14 18 L17 21 L20 18"],
        ["horizontaltypemask"] = ["M5 5 V3 H19 V5 M12 3 V21 M9 21 H15", "M3 9 H5 M3 13 H5 M19 9 H21 M19 13 H21"],
        ["verticaltypemask"] = ["M4 5 H10 M7 5 V12 M4 15 H10", "M14 3 V5 M14 8 V10 M14 13 V15 M14 18 V20 M20 3 V20"],
        ["pathselection"] = ["F:M6 3 L6 19 L10 15 L13 21 L15 20 L12 14 L18 14 Z"],
        ["directselection"] = ["M6 3 L6 19 L10 15 L13 21 L15 20 L12 14 L18 14 Z"],
        ["rectangle"] = ["M4 5 H20 V19 H4 Z"],
        ["ellipse"] = ["M12 4 C17 4 21 7.5 21 12 C21 16.5 17 20 12 20 C7 20 3 16.5 3 12 C3 7.5 7 4 12 4 Z"],
        ["triangle"] = ["M12 4 L21 20 H3 Z"],
        ["polygon"] = ["M12 3 L21 10 L17.5 20 H6.5 L3 10 Z"],
        ["line"] = ["M4 20 L20 4"],
        ["customshape"] = ["M12 20 C7 16 3 13 3 9 C3 6 5 4 8 4 C10 4 11 5 12 7 C13 5 14 4 16 4 C19 4 21 6 21 9 C21 13 17 16 12 20 Z"],
        ["hand"] = ["M8 13 V5 C8 3.5 10 3.5 10 5 V11 V4 C10 2.5 12 2.5 12 4 V11 V5 C12 3.5 14 3.5 14 5 V12 V8 C14 6.5 16 6.5 16 8 V15 C16 19 14 21 11 21 H10 C7 21 6 19 4 16 L3 13 C2.5 11.5 4.5 10.5 5.5 12 L8 15"],
        ["rotateview"] = ["M20 12 A8 8 0 1 1 15 4.6", "M15 1 L15 5 L19 5", "M8 12 L12 8 L16 12 L12 16 Z"],
        ["zoom"] = ["M10 3 A7 7 0 1 0 10.01 3 Z", "M15 15 L21 21"],
        ["zoom-in"] = ["M10 3 A7 7 0 1 0 10.01 3 Z", "M15 15 L21 21", "M7 10 H13 M10 7 V13"],
        ["zoom-out"] = ["M10 3 A7 7 0 1 0 10.01 3 Z", "M15 15 L21 21", "M7 10 H13"],
        ["freetransform"] = ["M4 4 H20 V20 H4 Z", "F:M2 2 H6 V6 H2 Z", "F:M18 2 H22 V6 H18 Z", "F:M18 18 H22 V22 H18 Z", "F:M2 18 H6 V22 H2 Z"],
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
        ["new-layer"] = ["M5 5 H17 V17 H5 Z", "M8 8 H20 V20 H8"],
        ["trash"] = ["M4 6 H20 M9 6 V3 H15 V6 M6 6 L7 21 H17 L18 6", "M10 10 V17 M14 10 V17"],
        ["swap"] = ["M6 4 H16 C18 4 20 6 20 8 V14", "M17 11 L20 14 L23 11", "M4 6 L6 4 L4 2"],
        ["default-colors"] = ["F:M3 3 H11 V11 H3 Z", "M8 8 H16 V16 H8 Z"],
        ["quick-mask"] = ["M4 4 H20 V20 H4 Z", "F:M12 8 a4 4 0 1 0 0.01 0 Z"],
        ["screen-mode"] = ["M3 5 H21 V17 H3 Z", "M8 21 H16 M12 17 V21"],
        ["edit-toolbar"] = ["F:M6 12 a1.5 1.5 0 1 0 0.01 0 Z", "F:M12 12 a1.5 1.5 0 1 0 0.01 0 Z", "F:M18 12 a1.5 1.5 0 1 0 0.01 0 Z"],
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
        Canvas canvas = new() { Width = 24, Height = 24 };
        if (!Icons.TryGetValue(key, out string[]? parts))
        {
            parts = ["M5 5 H19 V19 H5 Z"];
        }
        foreach (string part in parts)
        {
            bool filled = part.StartsWith("F:", StringComparison.Ordinal);
            Avalonia.Controls.Shapes.Path path = new()
            {
                Data = Geometry.Parse(filled ? part[2..] : part),
                StrokeThickness = filled ? 0 : 1.6,
                StrokeLineCap = PenLineCap.Round,
                StrokeJoin = PenLineJoin.Round
            };
            if (filled) path.Bind(Shape.FillProperty, path.GetResourceObservable("ShareX.Brush.Text"));
            else path.Bind(Shape.StrokeProperty, path.GetResourceObservable("ShareX.Brush.Text"));
            canvas.Children.Add(path);
        }
        return new Viewbox { Width = size, Height = size, Child = canvas };
    }
}
