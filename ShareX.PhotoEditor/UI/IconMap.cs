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

using ShareX.PhotoEditor.Tools;

namespace ShareX.PhotoEditor.UI;

/// <summary>The <see cref="PhotoEditorIcons"/> key for each tool, adjustment and gradient type.</summary>
public static class IconMap
{
    public static string ForTool(ToolId tool) => tool switch
    {
        ToolId.Move => PhotoEditorIcons.Move,
        ToolId.Artboard => PhotoEditorIcons.Artboard,
        ToolId.RectangularMarquee => PhotoEditorIcons.RectangularMarquee,
        ToolId.EllipticalMarquee => PhotoEditorIcons.EllipticalMarquee,
        ToolId.SingleRowMarquee => PhotoEditorIcons.SingleRowMarquee,
        ToolId.SingleColumnMarquee => PhotoEditorIcons.SingleColumnMarquee,
        ToolId.Lasso => PhotoEditorIcons.Lasso,
        ToolId.PolygonalLasso => PhotoEditorIcons.PolygonalLasso,
        ToolId.MagneticLasso => PhotoEditorIcons.MagneticLasso,
        ToolId.ObjectSelection => PhotoEditorIcons.ObjectSelection,
        ToolId.QuickSelection => PhotoEditorIcons.QuickSelection,
        ToolId.MagicWand => PhotoEditorIcons.MagicWand,
        ToolId.SelectionBrush => PhotoEditorIcons.SelectionBrush,
        ToolId.Crop => PhotoEditorIcons.Crop,
        ToolId.PerspectiveCrop => PhotoEditorIcons.PerspectiveCrop,
        ToolId.Slice => PhotoEditorIcons.Slice,
        ToolId.SliceSelect => PhotoEditorIcons.SliceSelect,
        ToolId.Frame => PhotoEditorIcons.Frame,
        ToolId.Eyedropper => PhotoEditorIcons.Eyedropper,
        ToolId.ColorSampler => PhotoEditorIcons.ColorSampler,
        ToolId.Ruler => PhotoEditorIcons.Ruler,
        ToolId.Note => PhotoEditorIcons.Note,
        ToolId.Count => PhotoEditorIcons.Count,
        ToolId.SpotHealingBrush => PhotoEditorIcons.SpotHealingBrush,
        ToolId.HealingBrush => PhotoEditorIcons.HealingBrush,
        ToolId.Patch => PhotoEditorIcons.Patch,
        ToolId.ContentAwareMove => PhotoEditorIcons.ContentAwareMove,
        ToolId.RedEye => PhotoEditorIcons.RedEye,
        ToolId.Remove => PhotoEditorIcons.Remove,
        ToolId.Brush => PhotoEditorIcons.Brush,
        ToolId.Pencil => PhotoEditorIcons.Pencil,
        ToolId.ColorReplacement => PhotoEditorIcons.ColorReplacement,
        ToolId.MixerBrush => PhotoEditorIcons.MixerBrush,
        ToolId.AdjustmentBrush => PhotoEditorIcons.AdjustmentBrush,
        ToolId.CloneStamp => PhotoEditorIcons.CloneStamp,
        ToolId.PatternStamp => PhotoEditorIcons.PatternStamp,
        ToolId.HistoryBrush => PhotoEditorIcons.HistoryBrush,
        ToolId.ArtHistoryBrush => PhotoEditorIcons.ArtHistoryBrush,
        ToolId.Eraser => PhotoEditorIcons.Eraser,
        ToolId.BackgroundEraser => PhotoEditorIcons.BackgroundEraser,
        ToolId.MagicEraser => PhotoEditorIcons.MagicEraser,
        ToolId.Gradient => PhotoEditorIcons.Gradient,
        ToolId.PaintBucket => PhotoEditorIcons.PaintBucket,
        ToolId.Blur => PhotoEditorIcons.Blur,
        ToolId.Sharpen => PhotoEditorIcons.Sharpen,
        ToolId.Smudge => PhotoEditorIcons.Smudge,
        ToolId.Dodge => PhotoEditorIcons.Dodge,
        ToolId.Burn => PhotoEditorIcons.Burn,
        ToolId.Sponge => PhotoEditorIcons.Sponge,
        ToolId.Pen => PhotoEditorIcons.Pen,
        ToolId.FreeformPen => PhotoEditorIcons.FreeformPen,
        ToolId.CurvaturePen => PhotoEditorIcons.CurvaturePen,
        ToolId.AddAnchorPoint => PhotoEditorIcons.AddAnchorPoint,
        ToolId.DeleteAnchorPoint => PhotoEditorIcons.DeleteAnchorPoint,
        ToolId.ConvertPoint => PhotoEditorIcons.ConvertPoint,
        ToolId.HorizontalType => PhotoEditorIcons.HorizontalType,
        ToolId.VerticalType => PhotoEditorIcons.VerticalType,
        ToolId.HorizontalTypeMask => PhotoEditorIcons.HorizontalTypeMask,
        ToolId.VerticalTypeMask => PhotoEditorIcons.VerticalTypeMask,
        ToolId.PathSelection => PhotoEditorIcons.PathSelection,
        ToolId.DirectSelection => PhotoEditorIcons.DirectSelection,
        ToolId.Rectangle => PhotoEditorIcons.Rectangle,
        ToolId.Ellipse => PhotoEditorIcons.Ellipse,
        ToolId.Triangle => PhotoEditorIcons.Triangle,
        ToolId.Polygon => PhotoEditorIcons.Polygon,
        ToolId.Line => PhotoEditorIcons.Line,
        ToolId.CustomShape => PhotoEditorIcons.CustomShape,
        ToolId.Hand => PhotoEditorIcons.Hand,
        ToolId.RotateView => PhotoEditorIcons.RotateView,
        ToolId.Zoom => PhotoEditorIcons.Zoom,
        ToolId.FreeTransform => PhotoEditorIcons.FreeTransform,
        _ => throw new ArgumentOutOfRangeException(nameof(tool), tool, null)
    };

    /// <summary>Adjustments without an icon of their own get the generic adjustment icon.</summary>
    public static string ForAdjustment(AdjustmentKind kind) => kind switch
    {
        AdjustmentKind.BrightnessContrast => PhotoEditorIcons.AdjustmentBrightnessContrast,
        AdjustmentKind.Levels => PhotoEditorIcons.AdjustmentLevels,
        AdjustmentKind.Curves => PhotoEditorIcons.AdjustmentCurves,
        AdjustmentKind.Exposure => PhotoEditorIcons.AdjustmentExposure,
        AdjustmentKind.Vibrance => PhotoEditorIcons.AdjustmentVibrance,
        AdjustmentKind.HueSaturation => PhotoEditorIcons.AdjustmentHueSaturation,
        AdjustmentKind.ColorBalance => PhotoEditorIcons.AdjustmentColorBalance,
        AdjustmentKind.BlackAndWhite => PhotoEditorIcons.AdjustmentBlackAndWhite,
        AdjustmentKind.PhotoFilter => PhotoEditorIcons.AdjustmentPhotoFilter,
        AdjustmentKind.ChannelMixer => PhotoEditorIcons.AdjustmentChannelMixer,
        AdjustmentKind.ColorLookup => PhotoEditorIcons.AdjustmentColorLookup,
        AdjustmentKind.Invert => PhotoEditorIcons.AdjustmentInvert,
        AdjustmentKind.Posterize => PhotoEditorIcons.AdjustmentPosterize,
        AdjustmentKind.Threshold => PhotoEditorIcons.AdjustmentThreshold,
        AdjustmentKind.GradientMap => PhotoEditorIcons.AdjustmentGradientMap,
        AdjustmentKind.SelectiveColor => PhotoEditorIcons.AdjustmentSelectiveColor,
        AdjustmentKind.Desaturate => PhotoEditorIcons.AdjustmentDesaturate,
        _ => PhotoEditorIcons.Adjustment
    };

    public static string ForGradient(GradientType type) => type switch
    {
        GradientType.Linear => PhotoEditorIcons.GradientLinear,
        GradientType.Radial => PhotoEditorIcons.GradientRadial,
        GradientType.Angle => PhotoEditorIcons.GradientAngle,
        GradientType.Reflected => PhotoEditorIcons.GradientReflected,
        GradientType.Diamond => PhotoEditorIcons.GradientDiamond,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
