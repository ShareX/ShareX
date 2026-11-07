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

/// <summary>The shapes of the <see cref="PhotoEditorIcons"/>.</summary>
/// <remarks>
/// Original line art, drawn on the pixel grid each group is shown at (tools 20 px in
/// PhotoEditorIconDefinitions.Tools.cs, interface icons 16 px, badges 12 px and controls 10 px) with 1 px strokes on
/// pixel centers, so icons are sharp at their native size. Shading is the Foreground at <see cref="HalfTone"/> opacity.
/// </remarks>
internal static partial class PhotoEditorIconDefinitions
{
    private const double HalfTone = 0.45;

    private static readonly double[] Dashes = [2, 2];

    // Shared parts of the interface icons.
    private const string LayerBrushHandle = "M13.5 1 L15 2.5 L9 8.5 L7.5 7 Z";
    private const string LayerBrushTip = "M6.5 8 L8 9.5 C8 12.5 5.5 15 1 15 C1 11.5 3 8 6.5 8 Z";
    private const string GradientFrame = "M1.5 2.5 H14.5 V13.5 H1.5 Z";
    private const string Magnifier = "M1.5 6.5 A5 5 0 1 0 11.5 6.5 A5 5 0 1 0 1.5 6.5 Z";
    private const string MagnifierHandle = "M10 11 L11 10 L15.5 14.5 L14.5 15.5 Z";
    private const string AdjustmentFrame = "M1.5 1.5 H14.5 V14.5 H1.5 Z";

    public static IEnumerable<IconGroup> CreateGroups() => [CreateTools(), CreateInterface(), CreateBadges(), CreateControls()];

    /// <summary>Interface icons on a 16 px grid: options bar, panel footers, panel rows and the start screen.</summary>
    private static IconGroup CreateInterface() => new(16, IconStyle.Pixel)
    {
        {
            Icon.ZoomIn,
            Stroke(Magnifier),
            Fill(MagnifierHandle),
            Stroke("M4 6.5 H9 M6.5 4 V9")
        },
        {
            Icon.ZoomOut,
            Stroke(Magnifier),
            Fill(MagnifierHandle),
            Stroke("M4 6.5 H9")
        },
        {
            Icon.SelectionNew,
            Stroke("M2.5 2.5 H13.5 V13.5 H2.5 Z"),
            Fill("M3 3 H13 V13 H3 Z", HalfTone)
        },
        {
            Icon.SelectionAdd,
            Stroke("M1.5 1.5 H10.5 V5.5 H14.5 V14.5 H5.5 V10.5 H1.5 Z"),
            Fill("M2 2 H10 V6 H14 V14 H6 V10 H2 Z", HalfTone)
        },
        {
            Icon.SelectionSubtract,
            Stroke("M1.5 1.5 H10.5 V10.5 H1.5 Z"),
            Stroke("M5.5 5.5 H14.5 V14.5 H5.5 Z"),
            Fill("M2 2 H10 V5 H5 V10 H2 Z", HalfTone)
        },
        {
            Icon.SelectionIntersect,
            Stroke("M1.5 1.5 H10.5 V10.5 H1.5 Z"),
            Stroke("M5.5 5.5 H14.5 V14.5 H5.5 Z"),
            Fill("M6 6 H10 V10 H6 Z", HalfTone)
        },
        {
            Icon.BrushTip,
            Fill("M2.5 8 A5.5 5.5 0 1 0 13.5 8 A5.5 5.5 0 1 0 2.5 8 Z")
        },
        {
            Icon.Airbrush,
            Stroke("M2.5 6.5 H9.5 V14.5 H2.5 Z"),
            Fill("M4 3 H8 V6 H4 Z"),
            Fill("M10 3 H11 V4 H10 Z M12 1 H13 V2 H12 Z M12 4 H13 V5 H12 Z M14 2 H15 V3 H14 Z M14 6 H15 V7 H14 Z M12 7 H13 V8 H12 Z")
        },
        {
            Icon.GradientLinear,
            Stroke(GradientFrame),
            Fill("M2 3 H6 V13 H2 Z"),
            Fill("M6 3 H10 V13 H6 Z", HalfTone)
        },
        {
            Icon.GradientRadial,
            Stroke(GradientFrame),
            Fill("M3.5 8 A4.5 4.5 0 1 0 12.5 8 A4.5 4.5 0 1 0 3.5 8 Z", HalfTone),
            Fill("M6 8 A2 2 0 1 0 10 8 A2 2 0 1 0 6 8 Z")
        },
        {
            Icon.GradientAngle,
            Stroke(GradientFrame),
            Fill("M8 8 V3 H14 V8 Z"),
            Fill("M8 8 H14 V13 H8 Z", HalfTone)
        },
        {
            Icon.GradientReflected,
            Stroke(GradientFrame),
            Fill("M7 3 H9 V13 H7 Z"),
            Fill("M4 3 H7 V13 H4 Z M9 3 H12 V13 H9 Z", HalfTone)
        },
        {
            Icon.GradientDiamond,
            Stroke(GradientFrame),
            Fill("M8 3 L13 8 L8 13 L3 8 Z", HalfTone),
            Fill("M8 5.5 L10.5 8 L8 10.5 L5.5 8 Z")
        },
        {
            Icon.AlignLeft,
            Stroke("M1.5 1 V15"),
            Fill("M3 3 H13 V7 H3 Z M3 9 H9 V13 H3 Z")
        },
        {
            Icon.AlignHorizontalCenter,
            Stroke("M7.5 1 V15"),
            Fill("M2 3 H13 V7 H2 Z M4 9 H11 V13 H4 Z")
        },
        {
            Icon.AlignRight,
            Stroke("M14.5 1 V15"),
            Fill("M3 3 H13 V7 H3 Z M7 9 H13 V13 H7 Z")
        },
        {
            Icon.AlignTop,
            Stroke("M1 1.5 H15"),
            Fill("M3 3 H7 V13 H3 Z M9 3 H13 V9 H9 Z")
        },
        {
            Icon.AlignVerticalCenter,
            Stroke("M1 7.5 H15"),
            Fill("M3 2 H7 V13 H3 Z M9 4 H13 V11 H9 Z")
        },
        {
            Icon.AlignBottom,
            Stroke("M1 14.5 H15"),
            Fill("M3 3 H7 V13 H3 Z M9 7 H13 V13 H9 Z")
        },
        {
            Icon.TextOrientation,
            Fill("M1 1 H9 V3 H6 V10 H4 V3 H1 Z"),
            Stroke("M6 13.5 H10.5 C12.5 13.5 13.5 12.5 13.5 10.5 V7"),
            Fill("M13.5 4 L16 7.5 H11 Z")
        },
        {
            Icon.TextAlignLeft,
            Stroke("M1 3.5 H15 M1 6.5 H10 M1 9.5 H15 M1 12.5 H8")
        },
        {
            Icon.TextAlignCenter,
            Stroke("M1 3.5 H15 M4 6.5 H12 M1 9.5 H15 M5 12.5 H11")
        },
        {
            Icon.TextAlignRight,
            Stroke("M1 3.5 H15 M6 6.5 H15 M1 9.5 H15 M8 12.5 H15")
        },
        {
            Icon.FauxBold,
            Fill("M3 2 H8.5 C11 2 12 3.2 12 4.8 C12 6 11.3 6.9 10.3 7.3 C11.7 7.7 12.7 8.8 12.7 10.4 C12.7 12.5 11.2 14 8.5 14 H3 Z M5.5 4 V6.8 H8 C9 6.8 9.5 6.2 9.5 5.4 C9.5 4.6 9 4 8 4 Z M5.5 8.8 V12 H8.3 C9.5 12 10.1 11.3 10.1 10.4 C10.1 9.5 9.5 8.8 8.3 8.8 Z")
        },
        {
            Icon.FauxItalic,
            Fill("M6 2 H13 V4 H10.8 L8.2 12 H10 V14 H3 V12 H5.2 L7.8 4 H6 Z")
        },
        {
            Icon.Underline,
            Stroke("M4.5 2 V7.5 C4.5 11 11.5 11 11.5 7.5 V2"),
            Stroke("M3 14.5 H13")
        },
        {
            Icon.Strikethrough,
            Stroke("M11.5 3.5 C10.5 2 5 1.5 5 5 C5 8.5 11.5 7 11.5 11 C11.5 14.5 5.5 14 4.5 12.5"),
            Stroke("M2 8.5 H14")
        },
        {
            Icon.AllCaps,
            Fill("M1 3 H9 V5 H6 V14 H4 V5 H1 Z M9 7 H15 V9 H13 V14 H11 V9 H9 Z")
        },
        {
            Icon.FrameRectangle,
            Stroke("M2.5 2.5 H13.5 V13.5 H2.5 Z"),
            Stroke("M2.5 2.5 L13.5 13.5 M13.5 2.5 L2.5 13.5")
        },
        {
            Icon.FrameEllipse,
            Stroke("M1.5 8 A6.5 6.5 0 1 0 14.5 8 A6.5 6.5 0 1 0 1.5 8 Z"),
            Stroke("M3.5 3.5 L12.5 12.5 M12.5 3.5 L3.5 12.5")
        },
        {
            Icon.Eye,
            Stroke("M0.5 8 C3 3.5 13 3.5 15.5 8 C13 12.5 3 12.5 0.5 8 Z"),
            Fill("M5.5 8 A2.5 2.5 0 1 0 10.5 8 A2.5 2.5 0 1 0 5.5 8 Z")
        },
        {
            Icon.Lock,
            Fill("M3 7 H13 V14 H3 Z M7 9 H9 V12 H7 Z"),
            Stroke("M5.5 7 V5 C5.5 2 10.5 2 10.5 5 V7")
        },
        {
            Icon.Mask,
            Stroke("M1.5 2.5 H14.5 V13.5 H1.5 Z"),
            Fill("M5 8 A3 3 0 1 0 11 8 A3 3 0 1 0 5 8 Z")
        },
        {
            Icon.Adjustment,
            Stroke("M1.5 8 A6.5 6.5 0 1 0 14.5 8 A6.5 6.5 0 1 0 1.5 8 Z"),
            Fill("M8 1.5 A6.5 6.5 0 0 0 8 14.5 Z")
        },
        {
            Icon.Folder,
            Stroke("M1.5 2.5 H6 L7.5 4.5 H14.5 V13.5 H1.5 Z"),
            Stroke("M1.5 6.5 H14.5")
        },
        {
            Icon.TileView,
            Stroke("M1.5 1.5 H6.5 V6.5 H1.5 Z M9.5 1.5 H14.5 V6.5 H9.5 Z M1.5 9.5 H6.5 V14.5 H1.5 Z M9.5 9.5 H14.5 V14.5 H9.5 Z")
        },
        {
            Icon.ListView,
            Fill("M1 3 H4 V6 H1 Z M1 7 H4 V10 H1 Z M1 11 H4 V14 H1 Z"),
            Stroke("M6 4.5 H15 M6 8.5 H15 M6 12.5 H15")
        },
        {
            Icon.NewLayer,
            Stroke("M2.5 2.5 H12.5 V12.5 H2.5 Z"),
            Stroke("M7.5 5 V10 M5 7.5 H10")
        },
        {
            Icon.Trash,
            Stroke("M1.5 3.5 H13.5"),
            Stroke("M5.5 3 V1.5 H9.5 V3"),
            Stroke("M3.5 4 V14.5 H11.5 V4"),
            Stroke("M6.5 6 V12 M8.5 6 V12")
        },
        {
            Icon.SwapDimensions,
            Stroke("M1 5.5 H11"),
            Fill("M14 5.5 L10.5 2 V9 Z"),
            Stroke("M5 10.5 H15"),
            Fill("M2 10.5 L5.5 7 V14 Z")
        },
        {
            Icon.Plus,
            Stroke("M7.5 2 V13 M2 7.5 H13")
        },
        {
            Icon.Snapshot,
            Stroke("M1.5 4.5 H4.5 L6 2.5 H10 L11.5 4.5 H14.5 V13.5 H1.5 Z"),
            Stroke("M5 9 A3 3 0 1 0 11 9 A3 3 0 1 0 5 9 Z")
        },
        {
            Icon.NewDocument,
            Stroke("M3.5 1.5 H9.5 L12.5 4.5 V14.5 H3.5 Z"),
            Stroke("M9.5 1.5 V4.5 H12.5")
        },
        {
            Icon.Selection,
            Stroke("M1 1.5 H4 M6 1.5 H10 M12 1.5 H15 M1 14.5 H4 M6 14.5 H10 M12 14.5 H15 M1.5 2 V4 M1.5 6 V10 M1.5 12 V14 M14.5 2 V4 M14.5 6 V10 M14.5 12 V14")
        },
        {
            Icon.FillPath,
            Fill("M2.5 12.5 C3 6 6 3 12.5 2.5 L13.5 13.5 Z"),
            Fill("M1 11 H4 V14 H1 Z M11 1 H14 V4 H11 Z M12 12 H15 V15 H12 Z")
        },
        {
            Icon.StrokePath,
            Stroke("M2.5 12.5 C3 6 6 3 12.5 2.5 L13.5 13.5 Z"),
            Fill("M1 11 H4 V14 H1 Z M11 1 H14 V4 H11 Z M12 12 H15 V15 H12 Z")
        },
        {
            Icon.Clipboard,
            Stroke("M3.5 2.5 H12.5 V14.5 H3.5 Z"),
            Fill("M6 1 H10 V4 H6 Z"),
            Stroke("M6 7.5 H10 M6 10.5 H10")
        },
        {
            Icon.CaptureRegion,
            Stroke("M1.5 5 V1.5 H5 M11 1.5 H14.5 V5 M14.5 11 V14.5 H11 M5 14.5 H1.5 V11"),
            Stroke("M7.5 5 V10 M5 7.5 H10")
        },
        {
            Icon.CaptureScreen,
            Stroke("M1.5 2.5 H14.5 V11.5 H1.5 Z"),
            Stroke("M7.5 12 V14 M5 14.5 H11")
        },
        {
            Icon.WebColor,
            Stroke("M1.5 7.5 A6 6 0 1 0 13.5 7.5 A6 6 0 1 0 1.5 7.5 Z"),
            Stroke("M1.5 7.5 H13.5"),
            Stroke("M7.5 1.5 C4.5 4.5 4.5 10.5 7.5 13.5 C10.5 10.5 10.5 4.5 7.5 1.5")
        },
        {
            Icon.LockTransparency,
            Stroke("M1.5 1.5 H14.5 V14.5 H1.5 Z"),
            Fill("M2 2 H5 V5 H2 Z M8 2 H11 V5 H8 Z M5 5 H8 V8 H5 Z M11 5 H14 V8 H11 Z M2 8 H5 V11 H2 Z M8 8 H11 V11 H8 Z M5 11 H8 V14 H5 Z M11 11 H14 V14 H11 Z")
        },
        {
            Icon.LockPixels,
            Fill(LayerBrushHandle),
            Fill(LayerBrushTip)
        },
        {
            Icon.LockPosition,
            Stroke("M7.5 3 V12 M3 7.5 H12"),
            Fill("M7.5 0.5 L10.5 3.5 H4.5 Z M7.5 14.5 L10.5 11.5 H4.5 Z M0.5 7.5 L3.5 4.5 V10.5 Z M14.5 7.5 L11.5 4.5 V10.5 Z")
        },

        // Adjustments panel and adjustment layer menus.
        {
            Icon.AdjustmentBrightnessContrast,
            Stroke("M4 7.5 A3.5 3.5 0 1 0 11 7.5 A3.5 3.5 0 1 0 4 7.5 Z"),
            Fill("M7.5 4 A3.5 3.5 0 0 0 7.5 11 Z"),
            Stroke("M7.5 0 V2 M7.5 13 V15 M0 7.5 H2 M13 7.5 H15 M2.5 2.5 L3.5 3.5 M11.5 11.5 L12.5 12.5 M2.5 12.5 L3.5 11.5 M11.5 3.5 L12.5 2.5")
        },
        {
            Icon.AdjustmentLevels,
            Stroke("M0 14.5 H16"),
            Fill("M1 14 V10 H3 V14 Z M4 14 V5 H6 V14 Z M7 14 V2 H9 V14 Z M10 14 V7 H12 V14 Z M13 14 V11 H15 V14 Z")
        },
        {
            Icon.AdjustmentCurves,
            Stroke(AdjustmentFrame),
            Stroke("M2 14 C7 14 9 2 14 2")
        },
        {
            Icon.AdjustmentExposure,
            Stroke(AdjustmentFrame),
            Stroke("M14.5 1.5 L1.5 14.5"),
            Stroke("M3 5.5 H8 M5.5 3 V8"),
            Stroke("M8 11.5 H13")
        },
        {
            Icon.AdjustmentVibrance,
            Stroke("M7.5 1.5 L14 13.5 H1 Z"),
            Fill("M7.5 1.5 L14 13.5 H7.5 Z", HalfTone)
        },
        {
            Icon.AdjustmentHueSaturation,
            Stroke("M1 3.5 H15 M1 8.5 H15 M1 13.5 H15"),
            Fill("M3 2 H6 V5 H3 Z M9 7 H12 V10 H9 Z M5 12 H8 V15 H5 Z")
        },
        {
            Icon.AdjustmentColorBalance,
            Stroke("M1 3.5 H14"),
            Stroke("M7.5 2 V14 M4 14.5 H11"),
            Stroke("M2.5 4 L0.5 9 M2.5 4 L4.5 9 M12.5 4 L10.5 9 M12.5 4 L14.5 9"),
            Fill("M0 9 H5 C5 11 0 11 0 9 Z M10 9 H15 C15 11 10 11 10 9 Z")
        },
        {
            Icon.AdjustmentBlackAndWhite,
            Stroke(AdjustmentFrame),
            Fill("M2 2 H8 V14 H2 Z")
        },
        {
            Icon.AdjustmentPhotoFilter,
            Stroke("M1.5 8 A6.5 6.5 0 1 0 14.5 8 A6.5 6.5 0 1 0 1.5 8 Z"),
            Fill("M2 8 A6 6 0 1 0 14 8 A6 6 0 1 0 2 8 Z", HalfTone),
            Fill("M4.5 6 A1.5 1.5 0 1 0 7.5 6 A1.5 1.5 0 1 0 4.5 6 Z")
        },
        {
            Icon.AdjustmentChannelMixer,
            Stroke("M1.5 5.5 A3.5 3.5 0 1 0 8.5 5.5 A3.5 3.5 0 1 0 1.5 5.5 Z"),
            Stroke("M6.5 5.5 A3.5 3.5 0 1 0 13.5 5.5 A3.5 3.5 0 1 0 6.5 5.5 Z"),
            Stroke("M4 10 A3.5 3.5 0 1 0 11 10 A3.5 3.5 0 1 0 4 10 Z")
        },
        {
            Icon.AdjustmentInvert,
            Stroke(AdjustmentFrame),
            Fill("M2 2 H14 L2 14 Z")
        },
        {
            Icon.AdjustmentPosterize,
            Fill("M1 14 V11 H4 V8 H7 V5 H10 V2 H14 V14 Z")
        },
        {
            Icon.AdjustmentThreshold,
            Stroke(AdjustmentFrame),
            Fill("M2 6 H6 V10 H10 V14 H2 Z")
        },
        {
            Icon.AdjustmentGradientMap,
            Stroke("M1.5 9.5 H14.5 V14.5 H1.5 Z"),
            Fill("M2 10 H6 V14 H2 Z"),
            Fill("M6 10 H10 V14 H6 Z", HalfTone),
            Stroke("M7.5 1 V5"),
            Fill("M7.5 8 L10 5 H5 Z")
        },
        {
            Icon.AdjustmentSelectiveColor,
            Stroke("M2 7.5 A5.5 5.5 0 1 0 13 7.5 A5.5 5.5 0 1 0 2 7.5 Z"),
            Fill("M5.5 7.5 A2 2 0 1 0 9.5 7.5 A2 2 0 1 0 5.5 7.5 Z"),
            Stroke("M7.5 0 V2 M7.5 13 V15 M0 7.5 H2 M13 7.5 H15")
        },
        {
            Icon.AdjustmentDesaturate,
            Stroke("M7.5 1 C7.5 1 3 6.5 3 9.5 C3 12 5 14 7.5 14 C10 14 12 12 12 9.5 C12 6.5 7.5 1 7.5 1 Z"),
            Stroke("M1.5 14.5 L14.5 1.5")
        },
        {
            Icon.AdjustmentColorLookup,
            Stroke(AdjustmentFrame),
            Stroke("M5.5 2 V14 M10.5 2 V14 M2 5.5 H14 M2 10.5 H14"),
            Fill("M6 6 H10 V10 H6 Z", HalfTone)
        }
    };

    /// <summary>Layer badges and toolbar toggles on a 12 px grid.</summary>
    private static IconGroup CreateBadges() => new(12, IconStyle.Pixel)
    {
        {
            Icon.Link,
            Stroke("M5.5 3.5 L7 2 C8.5 0.5 11.5 3.5 10 5 L8.5 6.5"),
            Stroke("M6.5 8.5 L5 10 C3.5 11.5 0.5 8.5 2 7 L3.5 5.5"),
            Stroke("M4.5 7.5 L7.5 4.5")
        },
        {
            Icon.Effects,
            Stroke("M7 1.5 C5.5 1.5 4.5 2.2 4.5 4 V11"),
            Stroke("M2.5 5.5 H7"),
            Stroke("M7 6.5 L11 11 M11 6.5 L7 11")
        },
        {
            Icon.SmartObject,
            Stroke("M1.5 1.5 H10.5 V10.5 H1.5 Z"),
            Fill("M6 6 H11 V11 H6 Z")
        },
        {
            Icon.ClippingMask,
            Stroke("M3.5 1 V6.5 H8"),
            Fill("M8 4 L11 6.5 L8 9 Z")
        },
        {
            Icon.PanelMenu,
            Stroke("M1 3.5 H11 M1 6.5 H11 M1 9.5 H11")
        },
        {
            Icon.ToolbarExpand,
            Stroke("M2 2.5 L5.5 6 L2 9.5 M6.5 2.5 L10 6 L6.5 9.5")
        },
        {
            Icon.ToolbarCollapse,
            Stroke("M5.5 2.5 L2 6 L5.5 9.5 M10 2.5 L6.5 6 L10 9.5")
        }
    };

    /// <summary>Chevrons and close crosses on a 10 px grid.</summary>
    private static IconGroup CreateControls() => new(10, IconStyle.Pixel)
    {
        {
            Icon.ChevronDown,
            Stroke("M1 3 L5 7 L9 3")
        },
        {
            Icon.ChevronRight,
            Stroke("M3 1 L7 5 L3 9")
        },
        {
            Icon.Close,
            Stroke("M1.5 1.5 L8.5 8.5 M8.5 1.5 L1.5 8.5")
        }
    };
}
