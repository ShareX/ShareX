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

using static ShareX.Icons.IconShape;
using Icon = ShareX.Icons.PhotoEditorIcons;

namespace ShareX.Icons;

internal static partial class PhotoEditorIconDefinitions
{
    // Shared parts of the tool icons. Strokes are 1 px, so straight lines sit on .5 coordinates to cover whole pixels.
    private const string BrushHandle = "M17.5 1 L19 2.5 L11.5 10.5 L9.5 8.5 Z";
    private const string BrushTip = "M8.5 9.5 L10.5 11.5 C10.5 15.5 7 18.5 1.5 18.5 C1.5 14 4 9.5 8.5 9.5 Z";
    private const string Bandage = "M2.5 11.5 L11.5 2.5 C13 1 15.5 1 17 2.5 C18.5 4 18.5 6.5 17 8 L8 17 C6.5 18.5 4 18.5 2.5 17 C1 15.5 1 13 2.5 11.5 Z";
    private const string BandagePad = "M7 9.5 L9.5 7 L12 9.5 L9.5 12 Z";
    private const string EraserBody = "M1.5 11.5 L9.5 3.5 L16.5 10.5 L8.5 18.5 Z";
    private const string EraserTip = "M1.5 11.5 L5 8 L12 15 L8.5 18.5 Z";
    private const string Nib = "M9.5 18.5 L2.5 10.5 L6 4.5 H13 L16.5 10.5 Z";
    private const string NibCap = "M6 1 H13 V3 H6 Z";
    private const string NibSlit = "M9.5 12.5 V18";
    private const string NibDot = "M8 10.5 A1.5 1.5 0 1 0 11 10.5 A1.5 1.5 0 1 0 8 10.5 Z";
    private const string SmallNib = "M6.5 18.5 L1 11.5 L4 6.5 H9 L12 11.5 Z";
    private const string SmallNibCap = "M4 3 H9 V5 H4 Z M5.5 11.5 A1 1 0 1 0 7.5 11.5 A1 1 0 1 0 5.5 11.5 Z";
    private const string SmallNibSlit = "M6.5 13 V18";
    private const string Eyedropper = "M10.5 6.5 L3 14 L2 18 L6 17 L13.5 9.5";
    private const string EyedropperCap = "M12.5 2.5 C14 1 16.5 0.5 18 2 C19.5 3.5 19 6 17.5 7.5 L16 9 L11 4 Z";
    private const string EyedropperCollar = "M9.5 3.5 L16.5 10.5 L15 12 L8 5 Z";
    private const string StampTop = "M7 4.5 A2.5 2.5 0 1 0 12 4.5 A2.5 2.5 0 1 0 7 4.5 Z";
    private const string StampBody = "M8.5 7 H10.5 V10 H8.5 Z M3 10 H16 L17.5 14 H1.5 Z";
    private const string TypeT = "M3 2 H16 V5 H14.5 V3.5 H10.5 V16.5 H12.5 V18 H6.5 V16.5 H8.5 V3.5 H4.5 V5 H3 Z";
    private const string SmallTypeT = "M1 2 H11 V4.5 H10 V3.5 H7 V14.5 H8.5 V16 H3.5 V14.5 H5 V3.5 H2 V4.5 H1 Z";
    private const string Arrow = "M4 1 V16 L7.5 12.5 L10 18 L12.5 17 L10 11.5 H15 Z";

    /// <summary>Tool icons on a 20 px grid, and the toolbar buttons below the tools.</summary>
    private static IconGroup CreateTools() => new(20, IconStyle.Pixel)
    {
        {
            Icon.Move,
            Stroke("M9.5 4 V15 M4 9.5 H15"),
            Fill("M9.5 0.5 L13 4 H6 Z M9.5 18.5 L13 15 H6 Z M0.5 9.5 L4 6 V13 Z M18.5 9.5 L15 6 V13 Z")
        },
        {
            Icon.Artboard,
            Stroke("M5.5 1 V18 M13.5 1 V18 M1 5.5 H18 M1 13.5 H18")
        },
        {
            Icon.RectangularMarquee,
            Stroke("M2 2.5 H5 M8 2.5 H11 M14 2.5 H17 M2 16.5 H5 M8 16.5 H11 M14 16.5 H17 M2.5 3 V5 M2.5 8 V11 M2.5 14 V16 M16.5 3 V5 M16.5 8 V11 M16.5 14 V16")
        },
        {
            Icon.EllipticalMarquee,
            Stroke("M2 9.5 A7.5 6.5 0 1 0 17 9.5 A7.5 6.5 0 1 0 2 9.5 Z", dashes: Dashes)
        },
        {
            Icon.SingleRowMarquee,
            Stroke("M0.5 7 V12 M18.5 7 V12"),
            Stroke("M2 9.5 H5 M8 9.5 H11 M14 9.5 H17")
        },
        {
            Icon.SingleColumnMarquee,
            Stroke("M7 0.5 H12 M7 18.5 H12"),
            Stroke("M9.5 2 V5 M9.5 8 V11 M9.5 14 V17")
        },
        {
            Icon.Lasso,
            Stroke("M10 13.5 C5.3 13.5 1.5 11.3 1.5 8 C1.5 4.7 5.3 2.5 10 2.5 C14.7 2.5 18.5 4.7 18.5 8 C18.5 11.3 14.7 13.5 10 13.5 Z"),
            Stroke("M5.5 12.8 C4.5 14.5 5 16 6.5 16.5 C8 17 7.5 18.5 6 19")
        },
        {
            Icon.PolygonalLasso,
            Stroke("M6 13.5 L1.5 6.5 L8.5 1.5 L18 4.5 L16.5 12.5 Z"),
            Stroke("M5.5 13.5 L4.5 16.5 L7 19")
        },
        {
            Icon.MagneticLasso,
            Stroke("M9 11.5 C4.9 11.5 1.5 9.7 1.5 7 C1.5 4.3 4.9 2.5 9 2.5 C13.1 2.5 16.5 4.3 16.5 7 C16.5 9.4 13.8 11.1 10.5 11.4"),
            Stroke("M4.8 10.6 C3.8 12.5 4.5 14 6 14.5"),
            Fill("M11 13 H13 V16 C13 17.3 16 17.3 16 16 V13 H18 V16 C18 19.7 11 19.7 11 16 Z")
        },
        {
            Icon.ObjectSelection,
            Stroke("M1.5 6 V1.5 H6 M13 1.5 H17.5 V6 M17.5 13 V17.5 H13 M6 17.5 H1.5 V13"),
            Fill("M9.5 5 L14.5 14 H4.5 Z")
        },
        {
            Icon.QuickSelection,
            Stroke("M12.5 7 A5.5 5.5 0 1 0 1.5 7 A5.5 5.5 0 1 0 12.5 7 Z", dashes: Dashes),
            Fill("M7 4.5 C8.5 4.5 9.5 5.5 9.5 7 V9.5 H7 C5.5 9.5 4.5 8.5 4.5 7 C4.5 5.5 5.5 4.5 7 4.5 Z"),
            Fill("M10 11 L11 10 L19 18 L18 19 Z")
        },
        {
            Icon.MagicWand,
            Fill("M1.3 17.3 L11.3 7.3 L12.7 8.7 L2.7 18.7 Z"),
            Stroke("M15.5 1 V8 M12 4.5 H19"),
            Stroke("M17.5 10 V13 M16 11.5 H19"),
            Stroke("M8.5 1 V4 M7 2.5 H10")
        },
        {
            Icon.SelectionBrush,
            Fill(BrushHandle),
            Fill(BrushTip),
            Stroke("M12 18.5 H14 M16 18.5 H19 M18.5 13 V15 M18.5 17 V18")
        },
        {
            Icon.Crop,
            Stroke("M5.5 1 V13.5 H19"),
            Stroke("M1 5.5 H13.5 V19"),
            Fill("M5 13 H6 V14 H5 Z")
        },
        {
            Icon.PerspectiveCrop,
            Stroke("M3.5 3.5 L15.5 5.5 L16.5 16.5 L2.5 15.5 Z"),
            Stroke("M3.5 0 V3 M19 16.5 H17")
        },
        {
            Icon.Slice,
            Fill("M1 19 L12 6 L14.5 8.5 C11 13 6.5 16.5 1 19 Z"),
            Fill("M13 5 L16 2 C17 1 19 3 18 4 L15.5 7 Z")
        },
        {
            Icon.SliceSelect,
            Fill(Arrow),
            Fill("M11 10 L17 3 L18.5 4.5 C16.5 7 14 9 11 10 Z")
        },
        {
            Icon.Frame,
            Stroke("M2.5 2.5 H16.5 V16.5 H2.5 Z"),
            Stroke("M2.5 2.5 L16.5 16.5 M16.5 2.5 L2.5 16.5")
        },
        {
            Icon.Eyedropper,
            Stroke(Eyedropper),
            Fill(EyedropperCap),
            Fill(EyedropperCollar)
        },
        {
            Icon.ColorSampler,
            Stroke(Eyedropper),
            Fill(EyedropperCap),
            Fill(EyedropperCollar),
            Stroke("M15.5 13 V19 M12.5 16 H18.5")
        },
        {
            Icon.Ruler,
            Stroke("M1.5 6.5 H17.5 V12.5 H1.5 Z"),
            Stroke("M4.5 7 V9 M7.5 7 V10 M10.5 7 V9 M13.5 7 V10")
        },
        {
            Icon.Note,
            Stroke("M3.5 1.5 H12.5 L16.5 5.5 V18.5 H3.5 Z"),
            Stroke("M12.5 1.5 V5.5 H16.5"),
            Stroke("M6 9.5 H14 M6 12.5 H14 M6 15.5 H11")
        },
        {
            Icon.Count,
            Stroke("M2.5 6 L5.5 3.5 V16"),
            Stroke("M2.5 16.5 H8.5"),
            Stroke("M10.5 6 C11 2.5 17.5 2.5 17.5 6.5 C17.5 10 11 11.5 10.5 16 H18")
        },
        {
            Icon.SpotHealingBrush,
            Stroke(Bandage),
            Fill(BandagePad),
            Stroke("M1 7 A6 6 0 0 1 7 1", dashes: Dashes)
        },
        {
            Icon.HealingBrush,
            Stroke(Bandage),
            Fill(BandagePad)
        },
        {
            Icon.Remove,
            Fill(BrushHandle),
            Fill(BrushTip),
            Stroke("M13 13 L18 18 M18 13 L13 18")
        },
        {
            Icon.Patch,
            Stroke("M4 2.5 H15 C16 2.5 16.5 3 16.5 4 V15 C16.5 16 16 16.5 15 16.5 H4 C3 16.5 2.5 16 2.5 15 V4 C2.5 3 3 2.5 4 2.5 Z"),
            Stroke("M5.5 5.5 H13.5 V13.5 H5.5 Z", dashes: Dashes)
        },
        {
            Icon.ContentAwareMove,
            Stroke("M1.5 1.5 H10.5 V10.5 H1.5 Z", dashes: Dashes),
            Stroke("M8 8 L15 15"),
            Fill("M18.5 18.5 V12.5 L12.5 18.5 Z")
        },
        {
            Icon.RedEye,
            Stroke("M1 9.5 C4 4.5 15 4.5 18 9.5 C15 14.5 4 14.5 1 9.5 Z"),
            Fill("M6.5 9.5 A3 3 0 1 0 12.5 9.5 A3 3 0 1 0 6.5 9.5 Z")
        },
        {
            Icon.Brush,
            Fill(BrushHandle),
            Fill(BrushTip)
        },
        {
            Icon.Pencil,
            Stroke("M3.5 13.5 L14 3 L17 6 L6.5 16.5"),
            Fill("M2 18 L3 13 L7 17 Z"),
            Stroke("M12 5 L15 8")
        },
        {
            Icon.ColorReplacement,
            Fill(BrushHandle),
            Fill(BrushTip),
            Fill("M12 12 H16 V16 H12 Z"),
            Stroke("M14.5 14.5 H18.5 V18.5 H14.5 Z")
        },
        {
            Icon.MixerBrush,
            Fill(BrushHandle),
            Fill(BrushTip),
            Fill("M15.5 11 C15.5 11 18.5 14.5 18.5 16 C18.5 17.7 17.2 19 15.5 19 C13.8 19 12.5 17.7 12.5 16 C12.5 14.5 15.5 11 15.5 11 Z")
        },
        {
            Icon.AdjustmentBrush,
            Fill(BrushHandle),
            Fill(BrushTip),
            Stroke("M12 15.5 A3.5 3.5 0 1 0 19 15.5 A3.5 3.5 0 1 0 12 15.5 Z"),
            Fill("M15.5 12 A3.5 3.5 0 0 0 15.5 19 Z")
        },
        {
            Icon.CloneStamp,
            Stroke(StampTop),
            Fill(StampBody),
            Stroke("M1.5 16.5 H17.5")
        },
        {
            Icon.PatternStamp,
            Stroke(StampTop),
            Fill(StampBody),
            Fill("M2 16 H4 V18 H2 Z M6 16 H8 V18 H6 Z M10 16 H12 V18 H10 Z M14 16 H16 V18 H14 Z")
        },
        {
            Icon.HistoryBrush,
            Fill(BrushHandle),
            Fill(BrushTip),
            Stroke("M12 15.5 A3.5 3.5 0 1 0 19 15.5 A3.5 3.5 0 1 0 12 15.5 Z"),
            Stroke("M15.5 13.5 V15.5 H17")
        },
        {
            Icon.ArtHistoryBrush,
            Fill(BrushHandle),
            Fill(BrushTip),
            Stroke("M12 17.5 C13.5 12.5 15.5 19.5 17 14.5 C17.5 13 18.5 12.5 19.5 13")
        },
        {
            Icon.Eraser,
            Stroke(EraserBody),
            Fill(EraserTip),
            Stroke("M12 18.5 H19")
        },
        {
            Icon.BackgroundEraser,
            Stroke(EraserBody),
            Fill(EraserTip),
            Fill("M14 1 H16 V3 H14 Z M16 3 H18 V5 H16 Z M18 1 H20 V3 H18 Z")
        },
        {
            Icon.MagicEraser,
            Stroke(EraserBody),
            Fill(EraserTip),
            Stroke("M17.5 0 V5 M15 2.5 H20")
        },
        {
            Icon.Gradient,
            Stroke("M1.5 3.5 H17.5 V15.5 H1.5 Z"),
            Fill("M2 4 H7 V15 H2 Z"),
            Fill("M7 4 H12 V15 H7 Z", HalfTone)
        },
        {
            Icon.PaintBucket,
            Stroke("M2.5 9.5 L9.5 2.5 L16.5 9.5 L9.5 16.5 Z"),
            Fill("M2.5 9.5 H16.5 L9.5 16.5 Z"),
            Fill("M17.5 12 C17.5 12 19.5 15 19.5 16.3 C19.5 17.5 18.6 18.5 17.5 18.5 C16.4 18.5 15.5 17.5 15.5 16.3 C15.5 15 17.5 12 17.5 12 Z")
        },
        {
            Icon.Blur,
            Stroke("M9.5 1.5 C9.5 1.5 3.5 8.5 3.5 12.5 C3.5 15.8 6.2 18.5 9.5 18.5 C12.8 18.5 15.5 15.8 15.5 12.5 C15.5 8.5 9.5 1.5 9.5 1.5 Z"),
            Stroke("M6.5 12.5 C6.5 14.2 7.6 15.5 9.5 15.5")
        },
        {
            Icon.Sharpen,
            Stroke("M9.5 1 L15.5 18.5 H3.5 Z"),
            Fill("M9.5 1 L15.5 18.5 H9.5 Z", HalfTone)
        },
        {
            Icon.Smudge,
            Stroke("M7.5 11.5 V3 C7.5 1.5 10.5 1.5 10.5 3 V9 C10.5 7.8 13 7.8 13 9 V10 C13 8.8 15.5 8.8 15.5 10.5 V14 C15.5 17 13.5 18.5 11 18.5 H10 C7.5 18.5 6.5 17.5 5 15.5 L3 12.5 C2.5 11.5 3.5 10.5 4.8 11.3 L7.5 13.5")
        },
        {
            Icon.Dodge,
            Stroke("M4.5 7 A5 5 0 1 0 14.5 7 A5 5 0 1 0 4.5 7 Z"),
            Stroke("M9.5 12 V19")
        },
        {
            Icon.Burn,
            Stroke("M9.5 1 C11 5 15.5 7 15.5 12.5 C15.5 16 12.8 18.5 9.5 18.5 C6.2 18.5 3.5 16 3.5 12.5 C3.5 9.5 5.5 8 6.5 6 C7 8 8 9 9 9 C8.5 6 9 3.5 9.5 1 Z"),
            Fill("M9.5 11 C10.5 13 12.5 13.5 12.5 15.5 C12.5 17 11.2 18 9.5 18 C7.8 18 6.5 17 6.5 15.5 C6.5 14 8 13 9.5 11 Z")
        },
        {
            Icon.Sponge,
            Stroke("M2.5 6 C2.5 4.3 3.8 3.5 5.5 3.5 H13.5 C15.2 3.5 16.5 4.3 16.5 6 V13 C16.5 14.7 15.2 15.5 13.5 15.5 H5.5 C3.8 15.5 2.5 14.7 2.5 13 Z"),
            Fill("M5 6 H7 V8 H5 Z M10 6 H12 V8 H10 Z M7 10 H9 V12 H7 Z M12 10 H14 V12 H12 Z")
        },
        {
            Icon.Pen,
            Stroke(Nib),
            Fill(NibCap),
            Stroke(NibSlit),
            Fill(NibDot)
        },
        {
            Icon.FreeformPen,
            Stroke(SmallNib),
            Fill(SmallNibCap),
            Stroke(SmallNibSlit),
            Stroke("M12 4.5 C13.5 1.5 15 7.5 16.5 4.5 C17.5 2.5 18.5 3 19.5 3.5")
        },
        {
            Icon.CurvaturePen,
            Stroke(SmallNib),
            Fill(SmallNibCap),
            Stroke(SmallNibSlit),
            Stroke("M12 8 C12 2.5 19 2.5 19 8"),
            Fill("M11 7 H13 V9 H11 Z M18 7 H20 V9 H18 Z")
        },
        {
            Icon.AddAnchorPoint,
            Stroke(SmallNib),
            Fill(SmallNibCap),
            Stroke(SmallNibSlit),
            Stroke("M15.5 1 V8 M12 4.5 H19")
        },
        {
            Icon.DeleteAnchorPoint,
            Stroke(SmallNib),
            Fill(SmallNibCap),
            Stroke(SmallNibSlit),
            Stroke("M12 4.5 H19")
        },
        {
            Icon.ConvertPoint,
            Stroke("M2 18 L9.5 4 L17 18"),
            Stroke("M3.5 3.5 H15.5"),
            Fill("M8 2 H11 V5 H8 Z M2 2 H4 V5 H2 Z M15 2 H17 V5 H15 Z")
        },
        {
            Icon.HorizontalType,
            Fill(TypeT)
        },
        {
            Icon.VerticalType,
            Fill(SmallTypeT),
            Stroke("M15.5 2 V16"),
            Fill("M15.5 19 L12.5 15 H18.5 Z")
        },
        {
            Icon.HorizontalTypeMask,
            Stroke("M3.5 2.5 H15.5 V4.5 M3.5 2.5 V4.5 M9.5 2.5 V17.5 M6.5 17.5 H12.5", dashes: Dashes)
        },
        {
            Icon.VerticalTypeMask,
            Stroke("M1.5 2.5 H10.5 V4.5 M1.5 2.5 V4.5 M6 2.5 V15.5 M3.5 15.5 H8.5", dashes: Dashes),
            Stroke("M15.5 2 V16"),
            Fill("M15.5 19 L12.5 15 H18.5 Z")
        },
        {
            Icon.PathSelection,
            Fill(Arrow)
        },
        {
            Icon.DirectSelection,
            Stroke("M4.5 2 V14.8 L7.7 11.6 L10.2 17.3 L11.8 16.6 L9.3 10.9 H13.8 Z")
        },
        {
            Icon.Rectangle,
            Stroke("M2.5 3.5 H16.5 V15.5 H2.5 Z")
        },
        {
            Icon.Ellipse,
            Stroke("M1.5 9.5 A8 6.5 0 1 0 17.5 9.5 A8 6.5 0 1 0 1.5 9.5 Z")
        },
        {
            Icon.Triangle,
            Stroke("M9.5 2 L17.5 16.5 H1.5 Z")
        },
        {
            Icon.Polygon,
            Stroke("M9.5 1.5 L16.5 5.5 V13.5 L9.5 17.5 L2.5 13.5 V5.5 Z")
        },
        {
            Icon.Line,
            Stroke("M2.5 16.5 L16.5 2.5"),
            Fill("M1 15 H4 V18 H1 Z M15 1 H18 V4 H15 Z")
        },
        {
            Icon.CustomShape,
            Stroke("M9.5 1.5 L11.6 7.1 L17.6 7.4 L12.9 11.1 L14.5 16.9 L9.5 13.6 L4.5 16.9 L6.1 11.1 L1.4 7.4 L7.4 7.1 Z")
        },
        {
            Icon.Hand,
            Stroke("M4.5 12 V4 C4.5 2.5 7.5 2.5 7.5 4 V9 V2.5 C7.5 1 10.5 1 10.5 2.5 V9 V3 C10.5 1.5 13.5 1.5 13.5 3 V9.5 V5.5 C13.5 4 16.5 4 16.5 5.5 V13 C16.5 16.5 14 18.5 11 18.5 H9.5 C7 18.5 5.5 17 4 15 L1.5 11.5 C0.8 10.3 2.3 9.2 3.3 10.3 L4.5 11.8")
        },
        {
            Icon.RotateView,
            Stroke("M16.5 9.5 A7 7 0 1 1 12.5 3.2"),
            Fill("M16 4.5 L11 6.5 L11.5 0.5 Z"),
            Fill("M9.5 6.5 L12.5 9.5 L9.5 12.5 L6.5 9.5 Z")
        },
        {
            Icon.Zoom,
            Stroke("M2 8 A6 6 0 1 0 14 8 A6 6 0 1 0 2 8 Z"),
            Fill("M12.3 13.7 L13.7 12.3 L19 17.6 L17.6 19 Z")
        },
        {
            Icon.FreeTransform,
            Stroke("M3.5 3.5 H15.5 V15.5 H3.5 Z"),
            Fill("M2 2 H5 V5 H2 Z M14 2 H17 V5 H14 Z M14 14 H17 V17 H14 Z M2 14 H5 V17 H2 Z")
        },

        // Toolbar buttons below the tools.
        {
            Icon.EditToolbar,
            Fill("M3 9.5 A1.5 1.5 0 1 0 6 9.5 A1.5 1.5 0 1 0 3 9.5 Z M8 9.5 A1.5 1.5 0 1 0 11 9.5 A1.5 1.5 0 1 0 8 9.5 Z M13 9.5 A1.5 1.5 0 1 0 16 9.5 A1.5 1.5 0 1 0 13 9.5 Z")
        },
        {
            Icon.QuickMask,
            Stroke("M2.5 3.5 H16.5 V15.5 H2.5 Z"),
            Fill("M6 9.5 A3.5 3.5 0 1 0 13 9.5 A3.5 3.5 0 1 0 6 9.5 Z")
        },
        {
            Icon.ScreenMode,
            Stroke("M1.5 3.5 H17.5 V13.5 H1.5 Z"),
            Stroke("M9.5 14 V16 M6 16.5 H13")
        }
    };
}
