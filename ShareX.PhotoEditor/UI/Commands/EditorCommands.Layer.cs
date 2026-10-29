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
        R("layer.new", "New Layer...", "Ctrl+Shift+N", w => NewLayer(w), HasDoc);
        R("layer.newViaCopy", "Layer via Copy", "Ctrl+J", w => LayerVia(w, false), HasLayer);
        R("layer.newViaCut", "Layer via Cut", "Ctrl+Shift+J", w => LayerVia(w, true), HasSelection);
        R("layer.newGroup", "New Group", null, w => NewGroup(w), HasDoc);
        R("layer.group", "Group Layers", "Ctrl+G", GroupLayers, HasLayer);
        R("layer.ungroup", "Ungroup Layers", "Ctrl+Shift+G", Ungroup, w => w.Document?.ActiveLayer is GroupLayer);
        R("layer.duplicate", "Duplicate Layer", null, DuplicateLayer, HasLayer);
        R("layer.delete", "Delete Layer", null, w => DeleteLayers(w), HasLayer);
        R("layer.rename", "Rename Layer...", null, w => _ = RenameLayerAsync(w), HasLayer);
        R("layer.style", "Layer Style...", null, w => _ = LayerStyle(w), HasLayer);
        R("layer.style.copy", "Copy Layer Style", null, w => _copiedStyle = w.Document!.ActiveLayer!.Style?.Clone(), HasLayer);
        R("layer.style.paste", "Paste Layer Style", null, w => { foreach (Layer l in w.Document!.SelectedLayers) l.Style = _copiedStyle?.Clone(); w.Commit("Paste Layer Style"); }, w => _copiedStyle != null && HasLayer(w));
        R("layer.style.clear", "Clear Layer Style", null, w => { w.Document!.ActiveLayer!.Style = null; w.Commit("Clear Layer Style"); }, HasLayer);
        R("layer.rasterizeStyle", "Rasterize Layer Style", null, w => Rasterize(w, true), HasLayer);
        R("layer.fill.solid", "Solid Color...", null, w => _ = NewFillLayerAsync(w, FillKind.SolidColor), HasDoc);
        R("layer.fill.gradient", "Gradient...", null, w => _ = NewFillLayerAsync(w, FillKind.Gradient), HasDoc);
        R("layer.fill.pattern", "Pattern...", null, w => _ = NewFillLayerAsync(w, FillKind.Pattern), HasDoc);
        foreach (AdjustmentKind kind in Enum.GetValues<AdjustmentKind>())
        {
            AdjustmentKind captured = kind;
            R($"layer.adjust.{kind}", AdjustmentSettings.GetName(kind) + "...", null, w => NewAdjustmentLayer(w, captured), HasDoc);
        }
        R("layer.mask.revealAll", "Reveal All", null, w => AddMask(w, MaskMode.RevealAll), HasLayer);
        R("layer.mask.hideAll", "Hide All", null, w => AddMask(w, MaskMode.HideAll), HasLayer);
        R("layer.mask.revealSelection", "Reveal Selection", null, w => AddMask(w, MaskMode.RevealSelection), w => HasSelection(w) && HasLayer(w));
        R("layer.mask.hideSelection", "Hide Selection", null, w => AddMask(w, MaskMode.HideSelection), w => HasSelection(w) && HasLayer(w));
        R("layer.mask.fromTransparency", "From Transparency", null, w => AddMask(w, MaskMode.FromTransparency), w => w.Document?.ActiveLayer is RasterLayer);
        R("layer.mask.delete", "Delete", null, DeleteMask, w => w.Document?.ActiveLayer?.Mask != null);
        R("layer.mask.apply", "Apply", null, ApplyMask, w => w.Document?.ActiveLayer is RasterLayer { Mask: not null });
        R("layer.mask.toggle", "Disable", null, w => { Layer l = w.Document!.ActiveLayer!; l.MaskEnabled = !l.MaskEnabled; w.Commit(l.MaskEnabled ? "Enable Layer Mask" : "Disable Layer Mask"); }, w => w.Document?.ActiveLayer?.Mask != null, w => w.Document?.ActiveLayer?.MaskEnabled == false);
        R("layer.mask.edit", "Edit Mask", @"Ctrl+OemBackslash", w => { w.Document!.EditingMask = !w.Document.EditingMask; w.RefreshPanels(); }, w => w.Document?.ActiveLayer?.Mask != null, w => w.Document?.EditingMask == true);
        R("layer.clip", "Create Clipping Mask", "Ctrl+Alt+G", w => { Layer l = w.Document!.ActiveLayer!; l.IsClipped = !l.IsClipped; w.Commit(l.IsClipped ? "Create Clipping Mask" : "Release Clipping Mask"); }, HasLayer, w => w.Document?.ActiveLayer?.IsClipped == true);
        R("layer.smart.convert", "Convert to Smart Object", null, ConvertToSmartObject, HasLayer);
        R("layer.rasterize", "Rasterize", null, w => Rasterize(w, false), w => w.Document?.ActiveLayer is TextLayer or ShapeLayer or SmartObjectLayer or FillLayer or FrameLayer);
        R("layer.arrange.front", "Bring to Front", "Ctrl+Shift+OemCloseBrackets", w => Arrange(w, int.MaxValue), HasLayer);
        R("layer.arrange.forward", "Bring Forward", "Ctrl+OemCloseBrackets", w => Arrange(w, 1), HasLayer);
        R("layer.arrange.backward", "Send Backward", "Ctrl+OemOpenBrackets", w => Arrange(w, -1), HasLayer);
        R("layer.arrange.back", "Send to Back", "Ctrl+Shift+OemOpenBrackets", w => Arrange(w, int.MinValue), HasLayer);
        R("layer.align.left", "Left Edges", null, w => AlignLayers(w, LayerAlignment.Left), HasLayer);
        R("layer.align.hcenter", "Horizontal Centers", null, w => AlignLayers(w, LayerAlignment.HorizontalCenter), HasLayer);
        R("layer.align.right", "Right Edges", null, w => AlignLayers(w, LayerAlignment.Right), HasLayer);
        R("layer.align.top", "Top Edges", null, w => AlignLayers(w, LayerAlignment.Top), HasLayer);
        R("layer.align.vcenter", "Vertical Centers", null, w => AlignLayers(w, LayerAlignment.VerticalCenter), HasLayer);
        R("layer.align.bottom", "Bottom Edges", null, w => AlignLayers(w, LayerAlignment.Bottom), HasLayer);
        R("layer.mergeDown", "Merge Down", "Ctrl+E", MergeDown, HasLayer);
        R("layer.mergeVisible", "Merge Visible", "Ctrl+Shift+E", MergeVisible, HasDoc);
        R("layer.stampVisible", "Stamp Visible", "Ctrl+Shift+Alt+E", StampVisible, HasDoc);
        R("layer.flatten", "Flatten Image", null, Flatten, HasDoc);
        R("layer.lock", "Lock All", "Ctrl+OemQuestion", w => { Layer l = w.Document!.ActiveLayer!; l.LockAll = !l.LockAll; w.Commit("Lock Layer"); }, HasLayer, w => w.Document?.ActiveLayer?.LockAll == true);
        R("layer.hide", "Hide Layers", "Ctrl+OemComma", w => { foreach (Layer l in w.Document!.SelectedLayers) l.Visible = !l.Visible; w.Commit("Hide Layers"); }, HasLayer);
        R("layer.fromBackground", "Layer from Background", null, w => { if (w.Document!.ActiveLayer is RasterLayer r) { LayerOps.UnlockBackground(r); w.Commit("Layer from Background"); } }, w => w.Document?.ActiveLayer is RasterLayer { IsBackground: true });
        R("layer.toBackground", "Background from Layer", null, BackgroundFromLayer, w => w.Document?.ActiveLayer is RasterLayer { IsBackground: false });
        R("layer.selectNext", "Select Next Layer", "Alt+OemCloseBrackets", w => SelectAdjacent(w, -1), HasLayer);
        R("layer.selectPrevious", "Select Previous Layer", "Alt+OemOpenBrackets", w => SelectAdjacent(w, 1), HasLayer);
    }

    public static void NewLayer(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        RasterLayer layer = new($"Layer {doc.AllLayers.Count(l => l is RasterLayer) + 1}", PixelSurface.CreateColor(doc.Width, doc.Height));
        doc.AddLayer(layer);
        w.Commit("New Layer");
    }

    public static void NewGroup(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        PhotoDocument doc = w.Document!;
        doc.AddLayer(new GroupLayer($"Group {doc.AllLayers.Count(l => l is GroupLayer) + 1}"));
        w.Commit("New Group");
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
                w.ShowMessage("Could not cut because the layer is locked.");
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
        RasterLayer layer = new($"Layer {doc.AllLayers.Count(l => l is RasterLayer) + 1}", new PixelSurface(pixels), new SKPointI(bounds.Left, bounds.Top));
        doc.AddLayer(layer, raster);
        doc.SetSelectionLive(null);
        w.Commit(cut ? "Layer via Cut" : "Layer via Copy");
    }

    private static void GroupLayers(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        List<Layer> layers = doc.SelectedLayers.Where(l => !l.IsBackground).ToList();
        if (layers.Count == 0) return;
        GroupLayer parent = layers[0].Parent ?? doc.Root;
        int index = layers.Where(l => l.Parent == parent).Max(l => parent.Children.IndexOf(l));
        GroupLayer group = new($"Group {doc.AllLayers.Count(l => l is GroupLayer) + 1}");
        parent.Add(group, index + 1);
        foreach (Layer layer in layers.OrderBy(l => doc.AllLayers.ToList().IndexOf(l))) group.Add(layer);
        doc.ActiveLayer = group;
        doc.SelectedLayerIds.Clear();
        doc.SelectedLayerIds.Add(group.Id);
        w.Commit("Group Layers");
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
        w.Commit("Ungroup Layers");
    }

    private static void DuplicateLayer(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        foreach (Layer layer in doc.SelectedLayers.ToList())
        {
            Layer copy = layer.Duplicate();
            copy.Name = layer.Name + " copy";
            copy.IsBackground = false;
            copy.LockPosition = false;
            if (layer.IsBackground) copy.LockTransparency = false;
            doc.AddLayer(copy, layer);
        }
        w.Commit("Duplicate Layer");
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
        w.Commit(layers.Count > 1 ? "Delete Layers" : "Delete Layer");
    }

    private static async Task RenameLayerAsync(PhotoEditorWindow w)
    {
        Layer layer = w.Document!.ActiveLayer!;
        string? name = await DialogBuilder.PromptAsync(w, "Layer Properties", "Name:", layer.Name);
        if (string.IsNullOrWhiteSpace(name)) return;
        layer.Name = name;
        w.Commit("Rename Layer");
    }

    public static async Task LayerStyle(PhotoEditorWindow w)
    {
        Layer? layer = w.Document?.ActiveLayer;
        if (layer == null) return;
        if (layer.IsBackground)
        {
            w.ShowMessage("The Background layer cannot have layer styles. Convert it to a layer first (Layer > New > Layer from Background).");
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
            w.Commit("Layer Style");
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
        FillLayer layer = new(kind switch { FillKind.Gradient => "Gradient Fill", FillKind.Pattern => "Pattern Fill", _ => "Color Fill" }) { FillKind = kind };
        switch (kind)
        {
            case FillKind.SolidColor:
                SKColor? color = await ColorPickerDialog.ShowAsync(w, w.Foreground, "Solid Color", w);
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
        w.Commit("New Fill Layer");
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
        w.Commit("New " + AdjustmentSettings.GetName(kind) + " Layer");
        w.Panels.ShowPanel("properties");
    }

    public static MenuFlyout CreateAdjustmentFlyout(PhotoEditorWindow w)
    {
        MenuFlyout flyout = new();
        foreach (var (id, text) in new[] { ("layer.fill.solid", "Solid Color..."), ("layer.fill.gradient", "Gradient..."), ("layer.fill.pattern", "Pattern...") })
        {
            MenuItem item = new() { Header = text };
            item.Click += (_, _) => w.Execute(id);
            flyout.Items.Add(item);
        }
        flyout.Items.Add(new Separator());
        foreach (AdjustmentKind kind in Enum.GetValues<AdjustmentKind>())
        {
            MenuItem item = new() { Header = AdjustmentSettings.GetName(kind) + "..." };
            AdjustmentKind captured = kind;
            item.Click += (_, _) => NewAdjustmentLayer(w, captured);
            flyout.Items.Add(item);
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
            w.ShowMessage("The Background layer cannot have a mask. Convert it to a layer first.");
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
        w.Commit("Add Layer Mask");
    }

    private static void DeleteMask(PhotoEditorWindow w)
    {
        Layer layer = w.Document!.ActiveLayer!;
        layer.Mask = null;
        w.Document.EditingMask = false;
        w.Commit("Delete Layer Mask");
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
        w.Commit("Apply Layer Mask");
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
            w.ShowMessage("Could not convert to a smart object because the layer is empty.");
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
        w.Commit("Convert to Smart Object");
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
        w.Commit(includeStyle ? "Rasterize Layer Style" : "Rasterize Layer");
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
        w.Commit("Arrange Layer");
    }

    /// <summary>Moves a layer relative to another (Layers panel drag and drop). Position: 0 above, 1 below, 2 inside group.</summary>
    public static void MoveLayer(PhotoEditorWindow w, Layer layer, Layer target, int position)
    {
        PhotoDocument doc = w.Document!;
        if (layer == target || target is GroupLayer && PhotoDocument.IsDescendant(target, layer) || layer.IsBackground) return;
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
        w.Commit("Move Layer");
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
        w.Commit("Align");
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
            w.Commit("Merge Group");
            return;
        }
        if (index <= 0)
        {
            w.ShowMessage("Could not merge down because there is no layer below.");
            return;
        }
        Layer below = parent.Children[index - 1];
        if (below is AdjustmentLayer)
        {
            w.ShowMessage("Could not merge down because the layer below is not a pixel layer.");
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
        w.Commit("Merge Down");
    }

    private static void MergeVisible(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        List<Layer> visible = doc.Root.Children.Where(l => l.Visible).ToList();
        if (visible.Count < 2) return;
        RasterLayer merged = MergeLayers(doc, visible, visible.FirstOrDefault(l => l.IsBackground)?.Name ?? "Merged");
        merged.IsBackground = visible.Any(l => l.IsBackground);
        merged.LockPosition = merged.LockTransparency = merged.IsBackground;
        int index = doc.Root.Children.IndexOf(visible[0]);
        foreach (Layer layer in visible) doc.Root.Remove(layer);
        doc.Root.Add(merged, Math.Max(0, index));
        doc.ActiveLayer = merged;
        w.Commit("Merge Visible");
    }

    private static void StampVisible(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        using SKBitmap flat = Compositor.Flatten(doc);
        RasterLayer layer = new($"Layer {doc.AllLayers.Count() + 1}", PixelSurface.FromBitmap(flat));
        doc.Root.Add(layer);
        doc.ActiveLayer = layer;
        w.Commit("Stamp Visible");
    }

    private static void Flatten(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        using SKBitmap flat = Compositor.Flatten(doc);
        using SKBitmap opaque = ImageIO.FlattenOnto(flat, SKColors.White);
        RasterLayer background = new("Background", PixelSurface.FromBitmap(opaque)) { IsBackground = true, LockPosition = true, LockTransparency = true };
        GroupLayer root = new("Root") { BlendMode = LayerBlendMode.Normal };
        root.Add(background);
        doc.ReplaceRoot(root);
        doc.EditingMask = false;
        w.Commit("Flatten Image");
    }

    private static void BackgroundFromLayer(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        if (doc.ActiveLayer is not RasterLayer layer) return;
        foreach (RasterLayer other in doc.AllLayers.OfType<RasterLayer>().Where(l => l.IsBackground)) LayerOps.UnlockBackground(other);
        using SKBitmap rendered = Compositor.RenderLayer(doc, layer);
        using SKBitmap opaque = ImageIO.FlattenOnto(rendered, w.Background);
        RasterLayer background = new("Background", PixelSurface.FromBitmap(opaque)) { IsBackground = true, LockPosition = true, LockTransparency = true };
        doc.RemoveLayer(layer);
        doc.Root.Add(background, 0);
        doc.ActiveLayer = background;
        w.Commit("Background from Layer");
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
        w.Commit("Load Selection");
    }
}
