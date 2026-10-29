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

namespace ShareX.PhotoEditor.Tools;

/// <summary>Creates every tool and defines the default toolbar grouping and shortcuts.</summary>
public static class ToolRegistry
{
    public static IEnumerable<ToolBase> CreateAll(IEditorHost host) =>
    [
        new MoveTool(host), new ArtboardTool(host),
        new MarqueeTool(host, false), new MarqueeTool(host, true), new SingleLineMarqueeTool(host, true), new SingleLineMarqueeTool(host, false),
        new LassoTool(host), new PolygonalLassoTool(host), new MagneticLassoTool(host),
        new ObjectSelectionTool(host), new QuickSelectionTool(host), new MagicWandTool(host), new SelectionBrushTool(host),
        new CropTool(host), new PerspectiveCropTool(host), new SliceTool(host), new SliceSelectTool(host),
        new FrameTool(host),
        new EyedropperTool(host), new ColorSamplerTool(host), new RulerTool(host), new NoteTool(host), new CountTool(host),
        new SpotHealingTool(host, false), new HealingBrushTool(host), new PatchTool(host), new ContentAwareMoveTool(host), new RedEyeTool(host), new SpotHealingTool(host, true),
        new BrushTool(host), new PencilTool(host), new ColorReplacementTool(host), new MixerBrushTool(host), new AdjustmentBrushTool(host),
        new CloneStampTool(host), new PatternStampTool(host),
        new HistoryBrushTool(host), new ArtHistoryBrushTool(host),
        new EraserTool(host), new BackgroundEraserTool(host), new MagicEraserTool(host),
        new GradientTool(host), new PaintBucketTool(host),
        new BlurTool(host), new SharpenTool(host), new SmudgeTool(host),
        new DodgeBurnTool(host, true), new DodgeBurnTool(host, false), new SpongeTool(host),
        new PenTool(host), new FreeformPenTool(host), new CurvaturePenTool(host),
        new AnchorPointTool(host, ToolId.AddAnchorPoint), new AnchorPointTool(host, ToolId.DeleteAnchorPoint), new AnchorPointTool(host, ToolId.ConvertPoint),
        new TypeTool(host, false, false), new TypeTool(host, true, false), new TypeTool(host, true, true), new TypeTool(host, false, true),
        new PathSelectionTool(host), new DirectSelectionTool(host),
        new ShapeTool(host, ShapeKind.Rectangle), new ShapeTool(host, ShapeKind.Ellipse), new ShapeTool(host, ShapeKind.Triangle),
        new ShapeTool(host, ShapeKind.Polygon), new ShapeTool(host, ShapeKind.Line), new ShapeTool(host, ShapeKind.Custom),
        new HandTool(host), new RotateViewTool(host), new ZoomTool(host),
        new TransformTool(host)
    ];

    public static IReadOnlyList<(string Name, ToolId[] Tools)> DefaultLayout { get; } =
    [
        ("Move", [ToolId.Move, ToolId.Artboard]),
        ("Marquee", [ToolId.RectangularMarquee, ToolId.EllipticalMarquee, ToolId.SingleRowMarquee, ToolId.SingleColumnMarquee]),
        ("Lasso", [ToolId.Lasso, ToolId.PolygonalLasso, ToolId.MagneticLasso]),
        ("Object Selection", [ToolId.ObjectSelection, ToolId.QuickSelection, ToolId.MagicWand, ToolId.SelectionBrush]),
        ("Crop", [ToolId.Crop, ToolId.PerspectiveCrop, ToolId.Slice, ToolId.SliceSelect]),
        ("Frame", [ToolId.Frame]),
        ("Eyedropper", [ToolId.Eyedropper, ToolId.ColorSampler, ToolId.Ruler, ToolId.Note, ToolId.Count]),
        ("Healing", [ToolId.SpotHealingBrush, ToolId.Remove, ToolId.HealingBrush, ToolId.Patch, ToolId.ContentAwareMove, ToolId.RedEye]),
        ("Brush", [ToolId.Brush, ToolId.Pencil, ToolId.ColorReplacement, ToolId.MixerBrush, ToolId.AdjustmentBrush]),
        ("Stamp", [ToolId.CloneStamp, ToolId.PatternStamp]),
        ("History Brush", [ToolId.HistoryBrush, ToolId.ArtHistoryBrush]),
        ("Eraser", [ToolId.Eraser, ToolId.BackgroundEraser, ToolId.MagicEraser]),
        ("Gradient", [ToolId.Gradient, ToolId.PaintBucket]),
        ("Blur", [ToolId.Blur, ToolId.Sharpen, ToolId.Smudge]),
        ("Dodge", [ToolId.Dodge, ToolId.Burn, ToolId.Sponge]),
        ("Pen", [ToolId.Pen, ToolId.FreeformPen, ToolId.CurvaturePen, ToolId.AddAnchorPoint, ToolId.DeleteAnchorPoint, ToolId.ConvertPoint]),
        ("Type", [ToolId.HorizontalType, ToolId.VerticalType, ToolId.VerticalTypeMask, ToolId.HorizontalTypeMask]),
        ("Path Selection", [ToolId.PathSelection, ToolId.DirectSelection]),
        ("Shape", [ToolId.Rectangle, ToolId.Ellipse, ToolId.Triangle, ToolId.Polygon, ToolId.Line, ToolId.CustomShape]),
        ("Hand", [ToolId.Hand, ToolId.RotateView]),
        ("Zoom", [ToolId.Zoom])
    ];

    public static IReadOnlyDictionary<ToolId, char> Shortcuts { get; } = new Dictionary<ToolId, char>
    {
        [ToolId.Move] = 'V', [ToolId.Artboard] = 'V',
        [ToolId.RectangularMarquee] = 'M', [ToolId.EllipticalMarquee] = 'M',
        [ToolId.Lasso] = 'L', [ToolId.PolygonalLasso] = 'L', [ToolId.MagneticLasso] = 'L',
        [ToolId.ObjectSelection] = 'W', [ToolId.QuickSelection] = 'W', [ToolId.MagicWand] = 'W',
        [ToolId.Crop] = 'C', [ToolId.PerspectiveCrop] = 'C', [ToolId.Slice] = 'C', [ToolId.SliceSelect] = 'C',
        [ToolId.Frame] = 'K',
        [ToolId.Eyedropper] = 'I', [ToolId.ColorSampler] = 'I', [ToolId.Ruler] = 'I', [ToolId.Note] = 'I', [ToolId.Count] = 'I',
        [ToolId.SpotHealingBrush] = 'J', [ToolId.Remove] = 'J', [ToolId.HealingBrush] = 'J', [ToolId.Patch] = 'J', [ToolId.ContentAwareMove] = 'J', [ToolId.RedEye] = 'J',
        [ToolId.Brush] = 'B', [ToolId.Pencil] = 'B', [ToolId.ColorReplacement] = 'B', [ToolId.MixerBrush] = 'B', [ToolId.AdjustmentBrush] = 'B',
        [ToolId.CloneStamp] = 'S', [ToolId.PatternStamp] = 'S',
        [ToolId.HistoryBrush] = 'Y', [ToolId.ArtHistoryBrush] = 'Y',
        [ToolId.Eraser] = 'E', [ToolId.BackgroundEraser] = 'E', [ToolId.MagicEraser] = 'E',
        [ToolId.Gradient] = 'G', [ToolId.PaintBucket] = 'G',
        [ToolId.Dodge] = 'O', [ToolId.Burn] = 'O', [ToolId.Sponge] = 'O',
        [ToolId.Pen] = 'P', [ToolId.FreeformPen] = 'P', [ToolId.CurvaturePen] = 'P',
        [ToolId.HorizontalType] = 'T', [ToolId.VerticalType] = 'T', [ToolId.VerticalTypeMask] = 'T', [ToolId.HorizontalTypeMask] = 'T',
        [ToolId.PathSelection] = 'A', [ToolId.DirectSelection] = 'A',
        [ToolId.Rectangle] = 'U', [ToolId.Ellipse] = 'U', [ToolId.Triangle] = 'U', [ToolId.Polygon] = 'U', [ToolId.Line] = 'U', [ToolId.CustomShape] = 'U',
        [ToolId.Hand] = 'H', [ToolId.RotateView] = 'R',
        [ToolId.Zoom] = 'Z'
    };
}
