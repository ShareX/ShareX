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
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Dialogs;
using ShareX.PhotoEditor.UI.Panels;

namespace ShareX.PhotoEditor.UI.Commands;

public static partial class EditorCommands
{
    private static LayerStyle? _copiedStyle;

    private static void RegisterLayer()
    {
        R("layer.new", Strings.EditorCommands_NewLayerEllipsis, "Ctrl+Shift+N", w => NewLayer(w), HasDoc);
        R("layer.newViaCopy", Strings.EditorCommands_LayerViaCopy, "Ctrl+J", w => LayerVia(w, false), HasLayer);
        R("layer.newViaCut", Strings.EditorCommands_LayerViaCut, "Ctrl+Shift+J", w => LayerVia(w, true), HasSelection);
        R("layer.newGroup", Strings.EditorCommands_NewGroup, null, w => NewGroup(w), HasDoc);
        R("layer.group", Strings.EditorCommands_GroupLayers, "Ctrl+G", GroupLayers, HasLayer);
        R("layer.ungroup", Strings.EditorCommands_UngroupLayers, "Ctrl+Shift+G", Ungroup, w => w.Document?.ActiveLayer is GroupLayer);
        R("layer.duplicate", Strings.EditorCommands_DuplicateLayer, null, DuplicateLayer, HasLayer);
        R("layer.delete", Strings.EditorCommands_DeleteLayer, null, w => DeleteLayers(w), HasLayer);
        R("layer.rename", Strings.EditorCommands_RenameLayerEllipsis, null, w => _ = RenameLayerAsync(w), HasLayer);
        R("layer.style", Strings.EditorCommands_LayerStyleEllipsis, null, w => _ = LayerStyle(w), HasLayer);
        R("layer.style.copy", Strings.EditorCommands_CopyLayerStyle, null, w => _copiedStyle = w.Document!.ActiveLayer!.Style?.Clone(), HasLayer);
        R("layer.style.paste", Strings.EditorCommands_PasteLayerStyle, null, w => { foreach (Layer l in w.Document!.SelectedLayers) l.Style = _copiedStyle?.Clone(); w.Commit(Strings.EditorCommands_PasteLayerStyle); }, w => _copiedStyle != null && HasLayer(w));
        R("layer.style.clear", Strings.EditorCommands_ClearLayerStyle, null, w => { w.Document!.ActiveLayer!.Style = null; w.Commit(Strings.EditorCommands_ClearLayerStyle); }, HasLayer);
        R("layer.rasterizeStyle", Strings.EditorCommands_RasterizeLayerStyle, null, w => Rasterize(w, true), HasLayer);
        R("layer.fill.solid", Strings.EditorCommands_SolidColorEllipsis, null, w => _ = NewFillLayerAsync(w, FillKind.SolidColor), HasDoc);
        R("layer.fill.gradient", Strings.EditorCommands_GradientEllipsis, null, w => _ = NewFillLayerAsync(w, FillKind.Gradient), HasDoc);
        R("layer.fill.pattern", Strings.EditorCommands_PatternEllipsis, null, w => _ = NewFillLayerAsync(w, FillKind.Pattern), HasDoc);
        foreach (AdjustmentKind kind in AdjustmentLayerGroups.SelectMany(group => group))        {
            AdjustmentKind captured = kind;
            R($"layer.adjust.{kind}", AdjustmentSettings.GetName(kind) + "...", null, w => NewAdjustmentLayer(w, captured), HasDoc);
        }
        R("layer.mask.revealAll", Strings.EditorCommands_RevealAll, null, w => AddMask(w, MaskMode.RevealAll), HasLayer);
        R("layer.mask.hideAll", Strings.EditorCommands_HideAll, null, w => AddMask(w, MaskMode.HideAll), HasLayer);
        R("layer.mask.revealSelection", Strings.EditorCommands_RevealSelection, null, w => AddMask(w, MaskMode.RevealSelection), w => HasSelection(w) && HasLayer(w));
        R("layer.mask.hideSelection", Strings.EditorCommands_HideSelection, null, w => AddMask(w, MaskMode.HideSelection), w => HasSelection(w) && HasLayer(w));
        R("layer.mask.fromTransparency", Strings.EditorCommands_FromTransparency, null, w => AddMask(w, MaskMode.FromTransparency), w => w.Document?.ActiveLayer is RasterLayer);
        R("layer.mask.delete", Strings.EditorCommands_Delete, null, DeleteMask, w => w.Document?.ActiveLayer?.Mask != null);
        R("layer.mask.apply", Strings.EditorCommands_Apply, null, ApplyMask, w => w.Document?.ActiveLayer is RasterLayer { Mask: not null });
        R("layer.mask.toggle", Strings.EditorCommands_Disable, null, w => { Layer l = w.Document!.ActiveLayer!; l.MaskEnabled = !l.MaskEnabled; w.Commit(l.MaskEnabled ? Strings.EditorCommands_EnableLayerMask : Strings.EditorCommands_DisableLayerMask); }, w => w.Document?.ActiveLayer?.Mask != null, w => w.Document?.ActiveLayer?.MaskEnabled == false);
        R("layer.mask.edit", Strings.EditorCommands_EditMask, @"Ctrl+OemBackslash", w => { w.Document!.EditingMask = !w.Document.EditingMask; w.RefreshPanels(); }, w => w.Document?.ActiveLayer?.Mask != null, w => w.Document?.EditingMask == true);
        R("layer.clip", Strings.EditorCommands_CreateClippingMask, "Ctrl+Alt+G", w => { Layer l = w.Document!.ActiveLayer!; l.IsClipped = !l.IsClipped; w.Commit(l.IsClipped ? Strings.EditorCommands_CreateClippingMask : Strings.EditorCommands_ReleaseClippingMask); }, HasLayer, w => w.Document?.ActiveLayer?.IsClipped == true);
        R("layer.smart.convert", Strings.EditorCommands_ConvertToSmartObject, null, ConvertToSmartObject, HasLayer);
        R("layer.rasterize", Strings.EditorCommands_Rasterize, null, w => Rasterize(w, false), w => w.Document?.ActiveLayer is TextLayer or ShapeLayer or SmartObjectLayer or FillLayer or FrameLayer);
        R("layer.arrange.front", Strings.EditorCommands_BringToFront, "Ctrl+Shift+OemCloseBrackets", w => Arrange(w, int.MaxValue), HasLayer);
        R("layer.arrange.forward", Strings.EditorCommands_BringForward, "Ctrl+OemCloseBrackets", w => Arrange(w, 1), HasLayer);
        R("layer.arrange.backward", Strings.EditorCommands_SendBackward, "Ctrl+OemOpenBrackets", w => Arrange(w, -1), HasLayer);
        R("layer.arrange.back", Strings.EditorCommands_SendToBack, "Ctrl+Shift+OemOpenBrackets", w => Arrange(w, int.MinValue), HasLayer);
        R("layer.align.left", Strings.EditorCommands_LeftEdges, null, w => AlignLayers(w, LayerAlignment.Left), HasLayer);
        R("layer.align.hcenter", Strings.EditorCommands_HorizontalCenters, null, w => AlignLayers(w, LayerAlignment.HorizontalCenter), HasLayer);
        R("layer.align.right", Strings.EditorCommands_RightEdges, null, w => AlignLayers(w, LayerAlignment.Right), HasLayer);
        R("layer.align.top", Strings.EditorCommands_TopEdges, null, w => AlignLayers(w, LayerAlignment.Top), HasLayer);
        R("layer.align.vcenter", Strings.EditorCommands_VerticalCenters, null, w => AlignLayers(w, LayerAlignment.VerticalCenter), HasLayer);
        R("layer.align.bottom", Strings.EditorCommands_BottomEdges, null, w => AlignLayers(w, LayerAlignment.Bottom), HasLayer);
        R("layer.mergeDown", Strings.EditorCommands_MergeDown, "Ctrl+E", MergeDown, HasLayer);
        R("layer.mergeVisible", Strings.EditorCommands_MergeVisible, "Ctrl+Shift+E", MergeVisible, HasDoc);
        R("layer.stampVisible", Strings.EditorCommands_StampVisible, "Ctrl+Shift+Alt+E", StampVisible, HasDoc);
        R("layer.flatten", Strings.EditorCommands_FlattenImage, null, Flatten, HasDoc);
        R("layer.lock", Strings.EditorCommands_LockAll, "Ctrl+OemQuestion", w => { Layer l = w.Document!.ActiveLayer!; l.LockAll = !l.LockAll; w.Commit(Strings.EditorCommands_LockLayer); }, HasLayer, w => w.Document?.ActiveLayer?.LockAll == true);
        R("layer.link", Strings.EditorCommands_LinkUnlinkLayers, null, ToggleLinkLayers, HasLayer);
        R("layer.hide", Strings.EditorCommands_HideLayers, "Ctrl+OemComma", w => { foreach (Layer l in w.Document!.SelectedLayers) l.Visible = !l.Visible; w.Commit(Strings.EditorCommands_HideLayers); }, HasLayer);
        R("layer.fromBackground", Strings.EditorCommands_LayerFromBackground, null, w => { if (w.Document!.ActiveLayer is RasterLayer r) { LayerOps.UnlockBackground(r); w.Commit(Strings.EditorCommands_LayerFromBackground); } }, w => w.Document?.ActiveLayer is RasterLayer { IsBackground: true });
        R("layer.toBackground", Strings.EditorCommands_BackgroundFromLayer, null, BackgroundFromLayer, w => w.Document?.ActiveLayer is RasterLayer { IsBackground: false });
        R("layer.selectNext", Strings.EditorCommands_SelectNextLayer, "Alt+OemCloseBrackets", w => SelectAdjacent(w, -1), HasLayer);
        R("layer.selectPrevious", Strings.EditorCommands_SelectPreviousLayer, "Alt+OemOpenBrackets", w => SelectAdjacent(w, 1), HasLayer);
    }

    public static void NewLayer(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        RasterLayer layer = new(string.Format(Strings.EditorCommands_LayerNumber, doc.AllLayers.Count(l => l is RasterLayer) + 1), PixelSurface.CreateColor(doc.Width, doc.Height));
        doc.AddLayer(layer);
        w.Commit(Strings.EditorCommands_NewLayer);
    }

    /// <summary>Links the selected layers so they move together, or unlinks them when they are already linked.</summary>
    public static void ToggleLinkLayers(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        List<Layer> layers = doc.SelectedLayers.Where(l => !l.IsBackground).ToList();
        bool anyLinked = layers.Any(l => l.LinkId != 0);
        bool sameLink = layers.Count > 0 && layers.All(l => l.LinkId != 0) && layers.Select(l => l.LinkId).Distinct().Count() == 1;
        if (layers.Count < 2 && !anyLinked)
        {
            w.ShowMessage(Strings.EditorCommands_SelectLayersToLink);
            return;
        }
        if (sameLink || layers.Count < 2)
        {
            HashSet<int> unlink = layers.Where(l => l.LinkId != 0).Select(l => l.LinkId).ToHashSet();
            foreach (Layer layer in doc.AllLayers.Where(l => unlink.Contains(l.LinkId))) layer.LinkId = 0;
            w.Commit(Strings.EditorCommands_UnlinkLayers);
            return;
        }
        HashSet<int> merged = layers.Where(l => l.LinkId != 0).Select(l => l.LinkId).ToHashSet();
        int id = doc.AllLayers.Select(l => l.LinkId).DefaultIfEmpty(0).Max() + 1;
        foreach (Layer layer in doc.AllLayers.Where(l => merged.Contains(l.LinkId)).Concat(layers)) layer.LinkId = id;
        w.Commit(Strings.EditorCommands_LinkLayers);
    }

    public static void NewGroup(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        doc.AddLayer(new GroupLayer(string.Format(Strings.EditorCommands_GroupNumber, doc.AllLayers.Count(l => l is GroupLayer) + 1)));
        w.Commit(Strings.EditorCommands_NewGroup);
    }

    private static void LayerVia(PhotoEditorWindow w, bool cut)
    {
        PhotoDocument doc = w.Document!;
        if (doc.ActiveLayer is not RasterLayer raster)
        {
            DuplicateLayer(w);
            return;
        }
        SKRectI bounds = doc.SelectionBounds;
        SKBitmap pixels = doc.Selection != null
            ? ImageOps.ExtractMasked(raster.Surface.Bitmap, raster.Offset, doc.Selection, bounds)
            : ImageOps.Crop(raster.Surface.Bitmap, SKRectI.Create(bounds.Left - raster.Offset.X, bounds.Top - raster.Offset.Y, bounds.Width, bounds.Height));
        if (cut)
        {
            if (!raster.CanEditPixels)
            {
                pixels.Dispose();
                w.ShowMessage(Strings.EditorCommands_CannotCutLockedLayer);
                return;
            }
            if (raster.LockTransparency)
            {
                uint background = ColorMath.FromColor(w.Background);
                LayerOps.CompositeMasked(doc, raster, (_, _) => background, null, 1, LayerBlendMode.Normal);
            }
            else
            {
                LayerOps.EraseMasked(doc, raster, null, 1);
            }
        }
        RasterLayer layer = new(string.Format(Strings.EditorCommands_LayerNumber, doc.AllLayers.Count(l => l is RasterLayer) + 1), new PixelSurface(pixels), new SKPointI(bounds.Left, bounds.Top));
        doc.AddLayer(layer, raster);
        doc.SetSelectionLive(null);
        w.Commit(cut ? Strings.EditorCommands_LayerViaCut : Strings.EditorCommands_LayerViaCopy);
    }

    private static void GroupLayers(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        List<Layer> layers = doc.SelectedLayers.Where(l => !l.IsBackground).ToList();
        if (layers.Count == 0) return;
        GroupLayer parent = layers[0].Parent ?? doc.Root;
        int index = layers.Where(l => l.Parent == parent).Max(l => parent.Children.IndexOf(l));
        GroupLayer group = new(string.Format(Strings.EditorCommands_GroupNumber, doc.AllLayers.Count(l => l is GroupLayer) + 1));
        parent.Add(group, index + 1);
        foreach (Layer layer in layers.OrderBy(l => doc.AllLayers.ToList().IndexOf(l))) group.Add(layer);
        doc.ActiveLayer = group;
        doc.SelectedLayerIds.Clear();
        doc.SelectedLayerIds.Add(group.Id);
        w.Commit(Strings.EditorCommands_GroupLayers);
    }

    private static void Ungroup(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        if (doc.ActiveLayer is not GroupLayer group) return;
        GroupLayer parent = group.Parent ?? doc.Root;
        int index = parent.Children.IndexOf(group);
        foreach (Layer child in group.Children.ToList()) parent.Add(child, index++);
        parent.Remove(group);
        doc.ActiveLayer = parent.Children.ElementAtOrDefault(Math.Max(0, index - 1));
        w.Commit(Strings.EditorCommands_UngroupLayers);
    }

    private static void DuplicateLayer(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        foreach (Layer layer in doc.SelectedLayers.ToList())
        {
            Layer copy = layer.Duplicate();
            copy.Name = string.Format(Strings.LayerNames_Copy, layer.Name);
            copy.IsBackground = false;
            copy.LockPosition = false;
            if (layer.IsBackground) copy.LockTransparency = false;
            doc.AddLayer(copy, layer);
        }
        w.Commit(Strings.EditorCommands_DuplicateLayer);
    }

    public static void DeleteLayers(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        List<Layer> layers = doc.SelectedLayers.ToList();
        if (layers.Count == 0) return;
        if (doc.EditingMask && doc.ActiveLayer?.Mask != null)
        {
            DeleteMask(w);
            return;
        }
        foreach (Layer layer in layers) doc.RemoveLayer(layer);
        w.Commit(layers.Count > 1 ? Strings.EditorCommands_DeleteLayers : Strings.EditorCommands_DeleteLayer);
    }

    private static async Task RenameLayerAsync(PhotoEditorWindow w)
    {
        Layer layer = w.Document!.ActiveLayer!;
        string? name = await DialogBuilder.PromptAsync(w, Strings.EditorCommands_LayerProperties, Strings.EditorCommands_NameColon, layer.Name);
        if (string.IsNullOrWhiteSpace(name)) return;
        layer.Name = name;
        w.Commit(Strings.EditorCommands_RenameLayer);
    }

    public static async Task LayerStyle(PhotoEditorWindow w)
    {
        Layer? layer = w.Document?.ActiveLayer;
        if (layer == null) return;
        if (layer.IsBackground)
        {
            w.ShowMessage(Strings.EditorCommands_BackgroundCannotHaveStyles);
            return;
        }
        LayerStyle? original = layer.Style?.Clone();
        float opacity = layer.Opacity, fill = layer.FillOpacity;
        LayerBlendMode mode = layer.BlendMode;
        layer.Style ??= new LayerStyle();
        bool ok = await LayerStyleDialog.ShowAsync(w, layer);
        if (ok)
        {
            if (layer.Style is { HasAny: false }) layer.Style = null;
            w.Commit(Strings.EditorCommands_LayerStyle);
        }
        else
        {
            layer.Style = original;
            layer.Opacity = opacity;
            layer.FillOpacity = fill;
            layer.BlendMode = mode;
            w.InvalidateCanvas();
        }
    }

    private static async Task NewFillLayerAsync(PhotoEditorWindow w, FillKind kind)
    {
        PhotoDocument doc = w.Document!;
        FillLayer layer = new(kind switch { FillKind.Gradient => Strings.EditorCommands_GradientFill, FillKind.Pattern => Strings.EditorCommands_PatternFill, _ => Strings.EditorCommands_ColorFill }) { FillKind = kind };
        switch (kind)
        {
            case FillKind.SolidColor:
                SKColor? color = await ColorPickerDialog.ShowAsync(w, w.Foreground, Strings.EditorCommands_SolidColor, w);
                if (color == null) return;
                layer.Color = color.Value;
                break;
            case FillKind.Gradient:
                GradientDefinition? gradient = await GradientEditorDialog.ShowAsync(w, w.Gradient, w);
                if (gradient == null) return;
                layer.Gradient = gradient;
                break;
            default:
                layer.Pattern = w.Pattern ?? w.Patterns[0];
                break;
        }
        if (doc.Selection != null) layer.Mask = doc.Selection.Clone();
        doc.AddLayer(layer);
        w.Commit(Strings.EditorCommands_NewFillLayer);
        w.Panels.ShowPanel("properties");
    }

    public static void NewAdjustmentLayer(IEditorHost host, AdjustmentKind kind)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        AdjustmentSettings settings = new(kind);
        if (kind == AdjustmentKind.GradientMap) settings.Gradient = GradientDefinition.CreateDefault(w.Foreground, w.Background);
        AdjustmentLayer layer = new(AdjustmentSettings.GetName(kind) + " 1", settings);
        if (doc.Selection != null) layer.Mask = doc.Selection.Clone();
        else layer.Mask = PixelSurface.CreateMask(doc.Width, doc.Height, 255);
        doc.AddLayer(layer);
        w.Commit(string.Format(Strings.EditorCommands_NewAdjustmentLayerHistory, AdjustmentSettings.GetName(kind)));
        w.Panels.ShowPanel("properties");
    }

    public static MenuFlyout CreateAdjustmentFlyout(PhotoEditorWindow w)
    {
        MenuFlyout flyout = new();
        foreach (var (id, text) in new[] { ("layer.fill.solid", Strings.EditorCommands_SolidColorEllipsis), ("layer.fill.gradient", Strings.EditorCommands_GradientEllipsis), ("layer.fill.pattern", Strings.EditorCommands_PatternEllipsis) })
        {
            MenuItem item = new() { Header = text };
            item.Click += (_, _) => w.Execute(id);
            flyout.Items.Add(item);
        }
        flyout.Items.Add(new Separator());
        foreach (AdjustmentKind[] group in AdjustmentLayerGroups)
        {
            if (flyout.Items.Count > 4) flyout.Items.Add(new Separator());
            foreach (AdjustmentKind kind in group)
            {
                MenuItem item = new() { Header = AdjustmentSettings.GetName(kind) + "..." };
                AdjustmentKind captured = kind;
                item.Click += (_, _) => NewAdjustmentLayer(w, captured);
                flyout.Items.Add(item);
            }
        }
        return flyout;
    }

    public static void AddMask(IEditorHost host, MaskMode mode)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        Layer? layer = doc.ActiveLayer;
        if (layer == null) return;
        if (layer.IsBackground)
        {
            w.ShowMessage(Strings.EditorCommands_BackgroundCannotHaveMask);
            return;
        }
        if (layer.Mask != null && mode is MaskMode.RevealAll or MaskMode.HideAll)
        {
            doc.EditingMask = true;
            w.RefreshPanels();
            return;
        }
        PixelSurface mask;
        switch (mode)
        {
            case MaskMode.HideAll:
                mask = PixelSurface.CreateMask(doc.Width, doc.Height, 0);
                break;
            case MaskMode.RevealSelection when doc.Selection != null:
                mask = doc.Selection.Clone();
                break;
            case MaskMode.HideSelection when doc.Selection != null:
                mask = doc.Selection.Clone();
                MaskOps.Invert(mask);
                break;
            case MaskMode.FromTransparency when layer is RasterLayer raster:
            {
                using SKBitmap full = Compositor.RenderLayer(doc, raster, false);
                mask = ImageOps.ToMask(full, true);
                break;
            }
            default:
                mask = PixelSurface.CreateMask(doc.Width, doc.Height, 255);
                break;
        }
        layer.Mask = mask;
        layer.MaskEnabled = true;
        doc.EditingMask = true;
        if (mode is MaskMode.RevealSelection or MaskMode.HideSelection) doc.SetSelectionLive(null);
        w.Commit(Strings.EditorCommands_AddLayerMask);
    }

    private static void DeleteMask(PhotoEditorWindow w)
    {
        Layer layer = w.Document!.ActiveLayer!;
        layer.Mask = null;
        w.Document.EditingMask = false;
        w.Commit(Strings.EditorCommands_DeleteLayerMask);
    }

    private static unsafe void ApplyMask(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        if (doc.ActiveLayer is not RasterLayer { Mask: not null } raster) return;
        raster.EnsureCovers(doc.Bounds);
        PixelSurface surface = raster.GetWritableSurface();
        uint* p = (uint*)surface.Bitmap.GetPixels();
        int stride = surface.RowBytes / 4;
        for (int y = 0; y < surface.Height; y++)
        {
            for (int x = 0; x < surface.Width; x++)
            {
                int dx = x + raster.Offset.X, dy = y + raster.Offset.Y;
                int coverage = dx >= 0 && dy >= 0 && dx < doc.Width && dy < doc.Height ? MaskOps.Get(raster.Mask!, dx, dy) : 255;
                if (coverage < 255) p[y * stride + x] = ImageOps.ScalePixel(p[y * stride + x], coverage);
            }
        }
        raster.Mask = null;
        doc.EditingMask = false;
        w.Commit(Strings.EditorCommands_ApplyLayerMask);
    }

    private static void ConvertToSmartObject(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        Layer layer = doc.ActiveLayer!;
        if (layer is SmartObjectLayer) return;
        using SKBitmap rendered = Compositor.RenderLayer(doc, layer, false);
        SKRectI bounds = ImageOps.GetOpaqueBounds(rendered);
        if (bounds.IsEmpty)
        {
            w.ShowMessage(Strings.EditorCommands_CannotConvertEmptyToSmartObject);
            return;
        }
        SmartObjectLayer smart = new(layer.Name, new PixelSurface(ImageOps.Crop(rendered, bounds)))
        {
            Transform = SKMatrix.CreateTranslation(bounds.Left, bounds.Top),
            Opacity = layer.Opacity,
            BlendMode = layer.BlendMode == LayerBlendMode.PassThrough ? LayerBlendMode.Normal : layer.BlendMode,
            Mask = layer.Mask,
            Visible = layer.Visible,
            IsClipped = layer.IsClipped
        };
        ReplaceLayer(doc, layer, smart);
        w.Commit(Strings.EditorCommands_ConvertToSmartObject);
    }

    private static void ReplaceLayer(PhotoDocument doc, Layer old, Layer replacement)
    {
        GroupLayer parent = old.Parent ?? doc.Root;
        int index = parent.Children.IndexOf(old);
        parent.Remove(old);
        parent.Add(replacement, index);
        doc.ActiveLayer = replacement;
        doc.SelectedLayerIds.Clear();
        doc.SelectedLayerIds.Add(replacement.Id);
    }

    private static void Rasterize(PhotoEditorWindow w, bool includeStyle)
    {
        PhotoDocument doc = w.Document!;
        Layer layer = doc.ActiveLayer!;
        if (layer is GroupLayer or AdjustmentLayer) return;
        LayerStyle? style = layer.Style;
        float fill = layer.FillOpacity;
        if (!includeStyle) layer.Style = null;
        layer.FillOpacity = includeStyle ? fill : 1;
        using SKBitmap rendered = Compositor.RenderLayer(doc, layer, false);
        layer.Style = style;
        layer.FillOpacity = fill;
        RasterLayer raster = new(layer.Name, PixelSurface.FromBitmap(rendered))
        {
            Opacity = layer.Opacity,
            FillOpacity = includeStyle ? 1 : fill,
            BlendMode = layer.BlendMode,
            Mask = layer.Mask,
            MaskEnabled = layer.MaskEnabled,
            IsClipped = layer.IsClipped,
            Visible = layer.Visible,
            Style = includeStyle ? null : style
        };
        ReplaceLayer(doc, layer, raster);
        w.Commit(includeStyle ? Strings.EditorCommands_RasterizeLayerStyle : Strings.EditorCommands_RasterizeLayer);
    }

    private static void Arrange(PhotoEditorWindow w, int delta)
    {
        PhotoDocument doc = w.Document!;
        Layer layer = doc.ActiveLayer!;
        GroupLayer parent = layer.Parent ?? doc.Root;
        int index = parent.Children.IndexOf(layer);
        int minimum = parent == doc.Root && parent.Children.FirstOrDefault()?.IsBackground == true && !layer.IsBackground ? 1 : 0;
        if (layer.IsBackground) return;
        int target = delta == int.MaxValue ? parent.Children.Count - 1 : delta == int.MinValue ? minimum : Math.Clamp(index + delta, minimum, parent.Children.Count - 1);
        if (target == index) return;
        parent.Children.RemoveAt(index);
        parent.Children.Insert(target, layer);
        w.Commit(Strings.EditorCommands_ArrangeLayer);
    }

    /// <summary>Moves a layer relative to another (Layers panel drag and drop). Position: 0 above, 1 below, 2 inside group.</summary>
    public static void MoveLayer(PhotoEditorWindow w, Layer layer, Layer target, int position)
    {
        PhotoDocument doc = w.Document!;
        if (layer == target || PhotoDocument.IsDescendant(target, layer) || layer.IsBackground) return;
        if (position == 2 && target is GroupLayer group)
        {
            group.Add(layer);
            group.Expanded = true;
        }
        else
        {
            if (target.IsBackground && position == 1) return;
            GroupLayer parent = target.Parent ?? doc.Root;
            layer.Parent?.Remove(layer);
            int index = parent.Children.IndexOf(target);
            // The panel lists layers top-down, so "above" means a higher index.
            parent.Add(layer, position == 0 ? index + 1 : index);
        }
        doc.ActiveLayer = layer;
        w.Commit(Strings.EditorCommands_MoveLayer);
    }

    public static void AlignLayers(IEditorHost host, LayerAlignment alignment)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        List<Layer> layers = doc.SelectedLayers.Where(l => l.CanMove && !l.IsBackground).ToList();
        if (layers.Count == 0) return;
        static SKRect Bounds(Layer l) => l is RasterLayer r ? r.GetOpaqueBounds() : l.GetContentBounds();
        SKRect reference;
        if (doc.Selection != null) reference = doc.SelectionBounds;
        else if (layers.Count > 1) reference = layers.Select(Bounds).Aggregate(SKRect.Union);
        else reference = doc.Bounds;
        foreach (Layer layer in layers)
        {
            SKRect b = Bounds(layer);
            if (b.IsEmpty) continue;
            float dx = alignment switch
            {
                LayerAlignment.Left => reference.Left - b.Left,
                LayerAlignment.HorizontalCenter => reference.MidX - b.MidX,
                LayerAlignment.Right => reference.Right - b.Right,
                _ => 0
            };
            float dy = alignment switch
            {
                LayerAlignment.Top => reference.Top - b.Top,
                LayerAlignment.VerticalCenter => reference.MidY - b.MidY,
                LayerAlignment.Bottom => reference.Bottom - b.Bottom,
                _ => 0
            };
            layer.Translate(dx, dy);
        }
        w.Commit(Strings.EditorCommands_Align);
    }

    private static RasterLayer MergeLayers(PhotoDocument doc, IReadOnlyList<Layer> layers, string name)
    {
        using SKBitmap merged = Compositor.RenderLayers(doc, layers);
        return new RasterLayer(name, PixelSurface.FromBitmap(merged));
    }

    private static void MergeDown(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        Layer top = doc.ActiveLayer!;
        GroupLayer parent = top.Parent ?? doc.Root;
        int index = parent.Children.IndexOf(top);
        if (top is GroupLayer)
        {
            Layer flattened = MergeLayers(doc, [top], top.Name);
            ReplaceLayer(doc, top, flattened);
            w.Commit(Strings.EditorCommands_MergeGroup);
            return;
        }
        if (index <= 0)
        {
            w.ShowMessage(Strings.EditorCommands_CannotMergeDownNoLayerBelow);
            return;
        }
        Layer below = parent.Children[index - 1];
        if (below is AdjustmentLayer)
        {
            w.ShowMessage(Strings.EditorCommands_CannotMergeDownNotPixelLayer);
            return;
        }
        LayerBlendMode belowMode = below.BlendMode;
        float belowOpacity = below.Opacity;
        below.BlendMode = LayerBlendMode.Normal;
        below.Opacity = 1;
        RasterLayer merged = MergeLayers(doc, [below, top], below.Name);
        below.BlendMode = belowMode;
        below.Opacity = belowOpacity;
        merged.BlendMode = belowMode == LayerBlendMode.PassThrough ? LayerBlendMode.Normal : belowMode;
        merged.Opacity = belowOpacity;
        merged.IsBackground = below.IsBackground;
        merged.LockPosition = below.IsBackground;
        merged.LockTransparency = below.IsBackground;
        parent.Remove(top);
        ReplaceLayer(doc, below, merged);
        w.Commit(Strings.EditorCommands_MergeDown);
    }

    private static void MergeVisible(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        List<Layer> visible = doc.Root.Children.Where(l => l.Visible).ToList();
        if (visible.Count < 2) return;
        RasterLayer merged = MergeLayers(doc, visible, visible.FirstOrDefault(l => l.IsBackground)?.Name ?? Strings.EditorCommands_Merged);
        merged.IsBackground = visible.Any(l => l.IsBackground);
        merged.LockPosition = merged.LockTransparency = merged.IsBackground;
        int index = doc.Root.Children.IndexOf(visible[0]);
        foreach (Layer layer in visible) doc.Root.Remove(layer);
        doc.Root.Add(merged, Math.Max(0, index));
        doc.ActiveLayer = merged;
        w.Commit(Strings.EditorCommands_MergeVisible);
    }

    private static void StampVisible(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        using SKBitmap flat = Compositor.Flatten(doc);
        RasterLayer layer = new(string.Format(Strings.EditorCommands_LayerNumber, doc.AllLayers.Count() + 1), PixelSurface.FromBitmap(flat));
        doc.Root.Add(layer);
        doc.ActiveLayer = layer;
        w.Commit(Strings.EditorCommands_StampVisible);
    }

    private static void Flatten(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        using SKBitmap flat = Compositor.Flatten(doc);
        using SKBitmap opaque = ImageIO.FlattenOnto(flat, SKColors.White);
        RasterLayer background = new(Strings.LayerNames_Background, PixelSurface.FromBitmap(opaque)) { IsBackground = true, LockPosition = true, LockTransparency = true };
        GroupLayer root = new("Root") { BlendMode = LayerBlendMode.Normal };
        root.Add(background);
        doc.ReplaceRoot(root);
        doc.EditingMask = false;
        w.Commit(Strings.EditorCommands_FlattenImage);
    }

    private static void BackgroundFromLayer(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        if (doc.ActiveLayer is not RasterLayer layer) return;
        foreach (RasterLayer other in doc.AllLayers.OfType<RasterLayer>().Where(l => l.IsBackground)) LayerOps.UnlockBackground(other);
        using SKBitmap rendered = Compositor.RenderLayer(doc, layer);
        using SKBitmap opaque = ImageIO.FlattenOnto(rendered, w.Background);
        RasterLayer background = new(Strings.LayerNames_Background, PixelSurface.FromBitmap(opaque)) { IsBackground = true, LockPosition = true, LockTransparency = true };
        doc.RemoveLayer(layer);
        doc.Root.Add(background, 0);
        doc.ActiveLayer = background;
        w.Commit(Strings.EditorCommands_BackgroundFromLayer);
    }

    private static void SelectAdjacent(PhotoEditorWindow w, int delta)
    {
        PhotoDocument doc = w.Document!;
        List<Layer> list = doc.GetLayerListTopDown().Select(x => x.Layer).ToList();
        int index = list.IndexOf(doc.ActiveLayer!);
        Layer next = list[Math.Clamp(index + delta, 0, list.Count - 1)];
        doc.ActiveLayer = next;
        doc.SelectedLayerIds.Clear();
        doc.SelectedLayerIds.Add(next.Id);
        doc.EditingMask = false;
        w.RefreshPanels();
        w.RefreshOptions();
    }

    /// <summary>Ctrl-click on a layer thumbnail: loads its transparency as a selection.</summary>
    public static void LoadLayerTransparency(PhotoEditorWindow w, Layer layer, SelectionMode mode)
    {
        PhotoDocument doc = w.Document!;
        using SKBitmap rendered = Compositor.RenderLayer(doc, layer);
        PixelSurface mask = ImageOps.ToMask(rendered, true);
        doc.SetSelection(MaskOps.Combine(doc.Selection, mask, mode));
        w.Commit(Strings.EditorCommands_LoadSelection);
    }
}
