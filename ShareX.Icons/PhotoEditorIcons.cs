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

namespace ShareX.Icons;

/// <summary>
/// Keys of the Photo editor icons: pixel line art drawn at the size each group is shown at; see
/// <see cref="PhotoEditorIconDefinitions"/>. Tool icons are named after the tools.
/// </summary>
public static class PhotoEditorIcons
{
    private const string Prefix = "photo-editor/";

    // Tools, 20 px.
    public const string Move = Prefix + "move";
    public const string Artboard = Prefix + "artboard";
    public const string RectangularMarquee = Prefix + "rectangular-marquee";
    public const string EllipticalMarquee = Prefix + "elliptical-marquee";
    public const string SingleRowMarquee = Prefix + "single-row-marquee";
    public const string SingleColumnMarquee = Prefix + "single-column-marquee";
    public const string Lasso = Prefix + "lasso";
    public const string PolygonalLasso = Prefix + "polygonal-lasso";
    public const string MagneticLasso = Prefix + "magnetic-lasso";
    public const string ObjectSelection = Prefix + "object-selection";
    public const string QuickSelection = Prefix + "quick-selection";
    public const string MagicWand = Prefix + "magic-wand";
    public const string SelectionBrush = Prefix + "selection-brush";
    public const string Crop = Prefix + "crop";
    public const string PerspectiveCrop = Prefix + "perspective-crop";
    public const string Slice = Prefix + "slice";
    public const string SliceSelect = Prefix + "slice-select";
    public const string Frame = Prefix + "frame";
    public const string Eyedropper = Prefix + "eyedropper";
    public const string ColorSampler = Prefix + "color-sampler";
    public const string Ruler = Prefix + "ruler";
    public const string Note = Prefix + "note";
    public const string Count = Prefix + "count";
    public const string SpotHealingBrush = Prefix + "spot-healing-brush";
    public const string HealingBrush = Prefix + "healing-brush";
    public const string Remove = Prefix + "remove";
    public const string Patch = Prefix + "patch";
    public const string ContentAwareMove = Prefix + "content-aware-move";
    public const string RedEye = Prefix + "red-eye";
    public const string Brush = Prefix + "brush";
    public const string Pencil = Prefix + "pencil";
    public const string ColorReplacement = Prefix + "color-replacement";
    public const string MixerBrush = Prefix + "mixer-brush";
    public const string AdjustmentBrush = Prefix + "adjustment-brush";
    public const string CloneStamp = Prefix + "clone-stamp";
    public const string PatternStamp = Prefix + "pattern-stamp";
    public const string HistoryBrush = Prefix + "history-brush";
    public const string ArtHistoryBrush = Prefix + "art-history-brush";
    public const string Eraser = Prefix + "eraser";
    public const string BackgroundEraser = Prefix + "background-eraser";
    public const string MagicEraser = Prefix + "magic-eraser";
    public const string Gradient = Prefix + "gradient";
    public const string PaintBucket = Prefix + "paint-bucket";
    public const string Blur = Prefix + "blur";
    public const string Sharpen = Prefix + "sharpen";
    public const string Smudge = Prefix + "smudge";
    public const string Dodge = Prefix + "dodge";
    public const string Burn = Prefix + "burn";
    public const string Sponge = Prefix + "sponge";
    public const string Pen = Prefix + "pen";
    public const string FreeformPen = Prefix + "freeform-pen";
    public const string CurvaturePen = Prefix + "curvature-pen";
    public const string AddAnchorPoint = Prefix + "add-anchor-point";
    public const string DeleteAnchorPoint = Prefix + "delete-anchor-point";
    public const string ConvertPoint = Prefix + "convert-point";
    public const string HorizontalType = Prefix + "horizontal-type";
    public const string VerticalType = Prefix + "vertical-type";
    public const string HorizontalTypeMask = Prefix + "horizontal-type-mask";
    public const string VerticalTypeMask = Prefix + "vertical-type-mask";
    public const string PathSelection = Prefix + "path-selection";
    public const string DirectSelection = Prefix + "direct-selection";
    public const string Rectangle = Prefix + "rectangle";
    public const string Ellipse = Prefix + "ellipse";
    public const string Triangle = Prefix + "triangle";
    public const string Polygon = Prefix + "polygon";
    public const string Line = Prefix + "line";
    public const string CustomShape = Prefix + "custom-shape";
    public const string Hand = Prefix + "hand";
    public const string RotateView = Prefix + "rotate-view";
    public const string Zoom = Prefix + "zoom";
    public const string FreeTransform = Prefix + "free-transform";

    // Toolbar buttons below the tools, 20 px.
    public const string EditToolbar = Prefix + "edit-toolbar";
    public const string QuickMask = Prefix + "quick-mask";
    public const string ScreenMode = Prefix + "screen-mode";

    // Interface, 16 px.
    public const string ZoomIn = Prefix + "zoom-in";
    public const string ZoomOut = Prefix + "zoom-out";
    public const string SelectionNew = Prefix + "selection-new";
    public const string SelectionAdd = Prefix + "selection-add";
    public const string SelectionSubtract = Prefix + "selection-subtract";
    public const string SelectionIntersect = Prefix + "selection-intersect";
    public const string BrushTip = Prefix + "brush-tip";
    public const string Airbrush = Prefix + "airbrush";
    public const string GradientLinear = Prefix + "gradient-linear";
    public const string GradientRadial = Prefix + "gradient-radial";
    public const string GradientAngle = Prefix + "gradient-angle";
    public const string GradientReflected = Prefix + "gradient-reflected";
    public const string GradientDiamond = Prefix + "gradient-diamond";
    public const string AlignLeft = Prefix + "align-left";
    public const string AlignHorizontalCenter = Prefix + "align-horizontal-center";
    public const string AlignRight = Prefix + "align-right";
    public const string AlignTop = Prefix + "align-top";
    public const string AlignVerticalCenter = Prefix + "align-vertical-center";
    public const string AlignBottom = Prefix + "align-bottom";
    public const string TextOrientation = Prefix + "text-orientation";
    public const string TextAlignLeft = Prefix + "text-align-left";
    public const string TextAlignCenter = Prefix + "text-align-center";
    public const string TextAlignRight = Prefix + "text-align-right";
    public const string FauxBold = Prefix + "faux-bold";
    public const string FauxItalic = Prefix + "faux-italic";
    public const string Underline = Prefix + "underline";
    public const string Strikethrough = Prefix + "strikethrough";
    public const string AllCaps = Prefix + "all-caps";
    public const string FrameRectangle = Prefix + "frame-rectangle";
    public const string FrameEllipse = Prefix + "frame-ellipse";
    public const string Eye = Prefix + "eye";
    public const string Lock = Prefix + "lock";
    public const string Mask = Prefix + "mask";
    public const string Adjustment = Prefix + "adjustment";
    public const string Folder = Prefix + "folder";
    public const string TileView = Prefix + "tile-view";
    public const string ListView = Prefix + "list-view";
    public const string NewLayer = Prefix + "new-layer";
    public const string Trash = Prefix + "trash";
    public const string SwapDimensions = Prefix + "swap-dimensions";
    public const string Plus = Prefix + "plus";
    public const string Snapshot = Prefix + "snapshot";
    public const string NewDocument = Prefix + "new-document";
    public const string Selection = Prefix + "selection";
    public const string FillPath = Prefix + "fill-path";
    public const string StrokePath = Prefix + "stroke-path";
    public const string Clipboard = Prefix + "clipboard";
    public const string CaptureRegion = Prefix + "capture-region";
    public const string CaptureScreen = Prefix + "capture-screen";
    public const string WebColor = Prefix + "web-color";
    public const string LockTransparency = Prefix + "lock-transparency";
    public const string LockPixels = Prefix + "lock-pixels";
    public const string LockPosition = Prefix + "lock-position";

    // Adjustments panel and adjustment layer menus, 16 px.
    public const string AdjustmentBrightnessContrast = Prefix + "adjustment-brightness-contrast";
    public const string AdjustmentLevels = Prefix + "adjustment-levels";
    public const string AdjustmentCurves = Prefix + "adjustment-curves";
    public const string AdjustmentExposure = Prefix + "adjustment-exposure";
    public const string AdjustmentVibrance = Prefix + "adjustment-vibrance";
    public const string AdjustmentHueSaturation = Prefix + "adjustment-hue-saturation";
    public const string AdjustmentColorBalance = Prefix + "adjustment-color-balance";
    public const string AdjustmentBlackAndWhite = Prefix + "adjustment-black-and-white";
    public const string AdjustmentPhotoFilter = Prefix + "adjustment-photo-filter";
    public const string AdjustmentChannelMixer = Prefix + "adjustment-channel-mixer";
    public const string AdjustmentInvert = Prefix + "adjustment-invert";
    public const string AdjustmentPosterize = Prefix + "adjustment-posterize";
    public const string AdjustmentThreshold = Prefix + "adjustment-threshold";
    public const string AdjustmentGradientMap = Prefix + "adjustment-gradient-map";
    public const string AdjustmentSelectiveColor = Prefix + "adjustment-selective-color";
    public const string AdjustmentDesaturate = Prefix + "adjustment-desaturate";
    public const string AdjustmentColorLookup = Prefix + "adjustment-color-lookup";

    // Layer badges and toolbar toggles, 12 px.
    public const string Link = Prefix + "link";
    public const string Effects = Prefix + "effects";
    public const string SmartObject = Prefix + "smart-object";
    public const string ClippingMask = Prefix + "clipping-mask";
    public const string PanelMenu = Prefix + "panel-menu";
    public const string ToolbarExpand = Prefix + "toolbar-expand";
    public const string ToolbarCollapse = Prefix + "toolbar-collapse";

    // Chevrons and close crosses, 10 px.
    public const string ChevronDown = Prefix + "chevron-down";
    public const string ChevronRight = Prefix + "chevron-right";
    public const string Close = Prefix + "close";
}
