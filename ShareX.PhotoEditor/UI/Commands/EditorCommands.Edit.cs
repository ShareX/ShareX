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
using ShareX.HelpersLib;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Commands;

public static partial class EditorCommands
{
    private static SKBitmap? _clipboard;
    private static SKPointI _clipboardOrigin;
    private static int _clipboardHash;

    private static void RegisterEdit()
    {
        R("edit.undo", "Undo", "Ctrl+Z", w => { if (!CancelPending(w)) { w.Document!.History.Undo(); w.AfterHistoryChange(); } }, w => w.Document?.History.CanUndo == true || w.Tools.Active.HasPendingOperation);
        R("edit.redo", "Redo", "Ctrl+Shift+Z, Ctrl+Y", w => { w.Document!.History.Redo(); w.AfterHistoryChange(); }, w => w.Document?.History.CanRedo == true);
        R("edit.stepBackward", "Step Backward", "Ctrl+Alt+Z", w => { w.Document!.History.Undo(); w.AfterHistoryChange(); }, w => w.Document?.History.CanUndo == true);
        R("edit.stepForward", "Step Forward", "Ctrl+Shift+Alt+Z", w => { w.Document!.History.Redo(); w.AfterHistoryChange(); }, w => w.Document?.History.CanRedo == true);
        R("edit.fade", "Fade...", "Ctrl+Shift+F", w => _ = FadeAsync(w), w => w.Document?.History.Previous != null);
        R("edit.cut", "Cut", "Ctrl+X", w => CopyCore(w, false, true), HasDoc);
        R("edit.copy", "Copy", "Ctrl+C", w => CopyCore(w, false, false), HasDoc);
        R("edit.copyMerged", "Copy Merged", "Ctrl+Shift+C", w => CopyCore(w, true, false), HasDoc);
        R("edit.paste", "Paste", "Ctrl+V", w => Paste(w, false, false));
        R("edit.pasteInPlace", "Paste in Place", "Ctrl+Shift+V", w => Paste(w, true, false), HasDoc);
        R("edit.pasteInto", "Paste Into", "Ctrl+Shift+Alt+V", w => Paste(w, true, true), HasSelection);
        R("edit.pasteNew", "Paste as New Document", null, w => NewFromClipboard(w));
        R("edit.clear", "Clear", "Delete, Back", Clear, HasDoc);
        R("edit.fill", "Fill...", "Shift+F5, Shift+Back", w => _ = FillAsync(w), HasDoc);
        R("edit.fillForeground", "Fill with Foreground", "Alt+Back", w => FillWith(w, w.Foreground), HasDoc);
        R("edit.fillBackground", "Fill with Background", "Ctrl+Back", w => FillWith(w, w.Background), HasDoc);
        R("edit.stroke", "Stroke...", null, w => _ = StrokeAsync(w), HasSelection);
        R("edit.contentAwareFill", "Content-Aware Fill", null, w => _ = ContentAwareFillAsync(w), HasSelection);
        R("edit.freeTransform", "Free Transform", "Ctrl+T", w => w.Tools.Get<TransformTool>().Begin(TransformMode.Free), HasLayer);
        R("edit.transform.again", "Again", "Ctrl+Shift+T", w => _lastTransform?.Invoke(w), HasLayer);
        R("edit.transform.scale", "Scale", null, w => w.Tools.Get<TransformTool>().Begin(TransformMode.Scale, "Scale"), HasLayer);
        R("edit.transform.rotate", "Rotate", null, w => w.Tools.Get<TransformTool>().Begin(TransformMode.Rotate, "Rotate"), HasLayer);
        R("edit.transform.skew", "Skew", null, w => w.Tools.Get<TransformTool>().Begin(TransformMode.Skew, "Skew"), HasLayer);
        R("edit.transform.distort", "Distort", null, w => w.Tools.Get<TransformTool>().Begin(TransformMode.Distort, "Distort"), HasLayer);
        R("edit.transform.perspective", "Perspective", null, w => w.Tools.Get<TransformTool>().Begin(TransformMode.Perspective, "Perspective"), HasLayer);
        R("edit.transform.rotate180", "Rotate 180°", null, w => QuickTransform(w, 180, false, false), HasLayer);
        R("edit.transform.rotate90cw", "Rotate 90° Clockwise", null, w => QuickTransform(w, 90, false, false), HasLayer);
        R("edit.transform.rotate90ccw", "Rotate 90° Counter Clockwise", null, w => QuickTransform(w, -90, false, false), HasLayer);
        R("edit.transform.flipH", "Flip Horizontal", null, w => QuickTransform(w, 0, true, false), HasLayer);
        R("edit.transform.flipV", "Flip Vertical", null, w => QuickTransform(w, 0, false, true), HasLayer);
        R("edit.defineBrush", "Define Brush Preset...", null, w => _ = DefineBrushAsync(w), HasDoc);
        R("edit.definePattern", "Define Pattern...", null, w => _ = DefinePatternAsync(w), HasDoc);
        R("edit.defineShape", "Define Custom Shape...", null, w => _ = DefineShapeAsync(w), w => w.Document?.ActivePath != null || w.Document?.ActiveLayer is ShapeLayer);
        R("edit.purge.history", "Purge Histories", null, w => { w.Document!.History.Purge(); w.RefreshPanels(); }, HasDoc);
        R("edit.purge.clipboard", "Purge Clipboard", null, w => { _clipboard?.Dispose(); _clipboard = null; });
        R("edit.preferences", "Preferences...", "Ctrl+K", w => _ = PreferencesDialog.ShowAsync(w));
        R("edit.shortcuts", "Keyboard Shortcuts...", "Ctrl+Shift+Alt+K", w => _ = ShortcutsDialog.ShowAsync(w));
        R("edit.toolbar", "Toolbar...", null, w => _ = ToolbarCustomizeDialog.ShowAsync(w));
        R("color.swap", "Switch Foreground and Background Colors", "X", w => SwapColors(w));
        R("color.reset", "Default Foreground and Background Colors", "D", w => ResetColors(w));
        R("select.quickMask", "Edit in Quick Mask Mode", "Q", w => ToggleQuickMask(w), HasDoc, w => w.Document?.QuickMask == true);
        R("view.screenMode", "Cycle Screen Mode", "F", w => w.CycleScreenMode());
    }

    private static Action<PhotoEditorWindow>? _lastTransform;

    private static bool CancelPending(PhotoEditorWindow w)
    {
        if (!w.Tools.Active.HasPendingOperation) return false;
        w.Tools.Active.Cancel();
        return true;
    }

    public static void SwapColors(IEditorHost host) => (host.Foreground, host.Background) = (host.Background, host.Foreground);

    public static void ResetColors(IEditorHost host)
    {
        host.Foreground = SKColors.Black;
        host.Background = SKColors.White;
    }

    public static void ToggleQuickMask(IEditorHost host)
    {
        PhotoDocument? doc = host.Document;
        if (doc == null) return;
        doc.QuickMask = !doc.QuickMask;
        if (!doc.QuickMask) doc.SetSelection(doc.Selection);
        doc.Invalidate();
        W(host).Commit(doc.QuickMask ? "Enter Quick Mask" : "Exit Quick Mask");
    }

    /// <summary>Copies (or cuts) the selected pixels of the active layer, or of all visible layers when merged.</summary>
    private static void CopyCore(PhotoEditorWindow w, bool merged, bool cut)
    {
        PhotoDocument doc = w.Document!;
        SKRectI bounds = doc.SelectionBounds;
        SKBitmap copy;
        if (merged || doc.ActiveLayer is not RasterLayer)
        {
            using SKBitmap source = merged ? Compositor.Flatten(doc) : doc.ActiveLayer != null ? Compositor.RenderLayer(doc, doc.ActiveLayer) : Compositor.Flatten(doc);
            copy = doc.Selection != null ? ImageOps.ExtractMasked(source, default, doc.Selection, bounds) : ImageOps.Crop(source, bounds);
        }
        else
        {
            RasterLayer raster = (RasterLayer)doc.ActiveLayer;
            if (doc.Selection != null)
            {
                copy = ImageOps.ExtractMasked(raster.Surface.Bitmap, raster.Offset, doc.Selection, bounds);
            }
            else
            {
                copy = ImageOps.Crop(raster.Surface.Bitmap, SKRectI.Create(bounds.Left - raster.Offset.X, bounds.Top - raster.Offset.Y, bounds.Width, bounds.Height));
            }
        }
        _clipboard?.Dispose();
        _clipboard = copy;
        _clipboardOrigin = new SKPointI(bounds.Left, bounds.Top);
        try
        {
            ClipboardHelpers.CopyImage(copy);
            _clipboardHash = ClipboardSignature();
        }
        catch (Exception)
        {
            _clipboardHash = 0;
        }
        if (cut) Clear(w);
    }

    private static int ClipboardSignature()
    {
        using SKBitmap? image = TryGetClipboardImage();
        return image == null ? 0 : HashCode.Combine(image.Width, image.Height, image.GetPixel(image.Width / 2, image.Height / 2));
    }

    private static void Paste(PhotoEditorWindow w, bool inPlace, bool into)
    {
        SKBitmap? image = null;
        SKPointI? origin = null;
        using SKBitmap? system = TryGetClipboardImage();
        if (_clipboard != null && (system == null || HashCode.Combine(system.Width, system.Height, system.GetPixel(system.Width / 2, system.Height / 2)) == _clipboardHash))
        {
            image = _clipboard.Copy();
            origin = _clipboardOrigin;
        }
        else if (system != null)
        {
            image = ImageOps.Convert(system);
        }
        if (image == null)
        {
            w.ShowMessage("The clipboard does not contain an image.");
            return;
        }
        using (image)
        {
            if (w.Document == null)
            {
                OpenBitmap(w, image, "Clipboard");
                return;
            }
            PhotoDocument doc = w.Document;
            if (doc.ActiveLayer is FrameLayer && !inPlace)
            {
                PlaceBitmap(w, image, "Pasted");
                return;
            }
            SKPointI position;
            if (inPlace && origin is SKPointI o) position = o;
            else
            {
                SKRectI target = doc.Selection != null ? doc.SelectionBounds : doc.Bounds;
                position = new SKPointI(target.MidX - image.Width / 2, target.MidY - image.Height / 2);
            }
            RasterLayer layer = new($"Layer {doc.AllLayers.Count() + 1}", PixelSurface.FromBitmap(image), position);
            if (into && doc.Selection != null) layer.Mask = doc.Selection.Clone();
            doc.AddLayer(layer);
            if (!into) doc.SetSelectionLive(null);
            w.Commit(into ? "Paste Into" : "Paste");
        }
    }

    private static void Clear(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        if (doc.EditingMask && doc.ActiveLayer?.Mask != null)
        {
            PixelSurface mask = doc.ActiveLayer.GetWritableMask(doc.Width, doc.Height);
            ClearMask(doc, mask);
            w.Commit("Clear");
            return;
        }
        if (doc.ActiveLayer is not RasterLayer && doc.Selection == null)
        {
            DeleteLayers(w);
            return;
        }
        RasterLayer? layer = EditableRaster(w, "clear");
        if (layer == null) return;
        if (layer.LockTransparency)
        {
            uint background = ColorMath.FromColor(w.Background);
            LayerOps.CompositeMasked(doc, layer, (_, _) => background, null, 1, LayerBlendMode.Normal);
        }
        else
        {
            LayerOps.EraseMasked(doc, layer, null, 1);
        }
        w.Commit("Clear");
    }

    private static unsafe void ClearMask(PhotoDocument doc, PixelSurface mask)
    {
        byte* p = (byte*)mask.Bitmap.GetPixels();
        for (int y = 0; y < mask.Height; y++)
            for (int x = 0; x < mask.Width; x++)
            {
                float s = doc.SelectionAt(x, y);
                p[y * mask.RowBytes + x] = (byte)(p[y * mask.RowBytes + x] * (1 - s));
            }
    }

    private static void FillWith(PhotoEditorWindow w, SKColor color)
    {
        RasterLayer? layer = EditableRaster(w, "fill");
        if (layer == null) return;
        uint c = ColorMath.FromColor(color);
        LayerOps.CompositeMasked(w.Document!, layer, (_, _) => c, null, 1, LayerBlendMode.Normal);
        w.Commit("Fill");
    }

    private static async Task FillAsync(PhotoEditorWindow w)
    {
        FillDialog.Result? result = await FillDialog.ShowAsync(w);
        if (result == null || w.Document == null) return;
        PhotoDocument doc = w.Document;
        if (result.ContentAware)
        {
            await ContentAwareFillAsync(w);
            return;
        }
        RasterLayer? layer = EditableRaster(w, "fill");
        if (layer == null) return;
        Func<int, int, uint> source;
        if (result.Pattern != null)
        {
            source = result.Pattern.Sample;
        }
        else if (result.History)
        {
            if (doc.History.Source == null) return;
            source = HistoryBrushTool.HistorySampler(doc, layer);
        }
        else
        {
            uint c = ColorMath.FromColor(result.Color);
            source = (_, _) => c;
        }
        bool savedLock = layer.LockTransparency;
        layer.LockTransparency = savedLock || result.PreserveTransparency;
        LayerOps.CompositeMasked(doc, layer, source, null, result.Opacity, result.Mode);
        layer.LockTransparency = savedLock;
        w.Commit("Fill");
    }

    private static async Task StrokeAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc?.Selection == null) return;
        float width = 3;
        SKColor color = w.Foreground;
        StrokePosition position = StrokePosition.Center;
        float opacity = 1;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Width:", DialogBuilder.SliderNumber(1, 250, width, v => width = (float)v));
        Button colorButton = new() { Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(color) } };
        colorButton.Click += async (_, _) =>
        {
            SKColor? c = await ColorPickerDialog.ShowAsync(w, color, "Stroke Color", w);
            if (c is SKColor value)
            {
                color = value;
                colorButton.Content = new Border { Width = 40, Height = 16, Background = DialogBuilder.ToBrush(color) };
            }
        };
        DialogBuilder.AddRow(form, "Color:", colorButton);
        DialogBuilder.AddRow(form, "Location:", DialogBuilder.Combo([("Inside", StrokePosition.Inside), ("Center", StrokePosition.Center), ("Outside", StrokePosition.Outside)], position, v => position = v));
        DialogBuilder.AddRow(form, "Opacity:", DialogBuilder.SliderNumber(1, 100, 100, v => opacity = (float)v / 100));
        if (!await DialogBuilder.ShowAsync(w, "Stroke", form)) return;
        RasterLayer? layer = EditableRaster(w, "stroke");
        if (layer == null) return;
        LayerOps.StrokeSelection(doc, layer, width, color, position, opacity, LayerBlendMode.Normal);
        w.Commit("Stroke");
    }

    public static async Task ContentAwareFillAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc?.Selection == null) return;
        RasterLayer? layer = EditableRaster(w, "fill");
        if (layer == null) return;
        layer.EnsureCovers(doc.Bounds);
        PixelSurface surface = layer.GetWritableSurface();
        using PixelSurface hole = MaskOps.Reframe(doc.Selection, surface.Width, surface.Height, -layer.Offset.X, -layer.Offset.Y, 0);
        MaskOps.Threshold(hole, 1);
        await w.RunBusyAsync("Content-Aware Fill...", progress => Task.Run(() => Inpainting.Fill(surface.Bitmap, hole, 3, null, progress)));
        w.Commit("Content-Aware Fill");
    }

    private static void QuickTransform(PhotoEditorWindow w, float angle, bool flipH, bool flipV)
    {
        Func<SKRect, SKMatrix> matrix = bounds =>
        {
            SKPoint c = new(bounds.MidX, bounds.MidY);
            if (flipH) return SKMatrix.CreateScale(-1, 1, c.X, c.Y);
            if (flipV) return SKMatrix.CreateScale(1, -1, c.X, c.Y);
            return SKMatrix.CreateRotationDegrees(angle, c.X, c.Y);
        };
        string name = flipH ? "Flip Horizontal" : flipV ? "Flip Vertical" : $"Rotate {angle}°";
        w.Tools.Get<TransformTool>().ApplyImmediate(matrix, name);
        _lastTransform = win => win.Tools.Get<TransformTool>().ApplyImmediate(matrix, name);
    }

    private static async Task FadeAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        HistoryEntry? previous = doc?.History.Previous;
        if (doc == null || previous == null || doc.ActiveLayer is not RasterLayer current) return;
        RasterLayer? before = previous.State.FindRasterLayer(current.Id);
        if (before == null)
        {
            w.ShowMessage("The last operation cannot be faded.");
            return;
        }
        float opacity = 1;
        LayerBlendMode mode = LayerBlendMode.Normal;
        using SKBitmap after = ImageOps.CreateBitmap(doc.Width, doc.Height);
        after.Erase(SKColors.Transparent);
        using (SKCanvas canvas = new(after)) canvas.DrawBitmap(current.Surface.Bitmap, current.Offset.X, current.Offset.Y);
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Opacity:", DialogBuilder.SliderNumber(0, 100, 100, v => opacity = (float)v / 100));
        DialogBuilder.AddRow(form, "Mode:", DialogBuilder.Combo(BlendModes.LayerModes.Select(m => (BlendModes.GetName(m), m)), mode, v => mode = v));
        if (!await DialogBuilder.ShowAsync(w, "Fade " + doc.History.Current?.Name, form)) return;
        // Rebuild the layer from the previous pixels and blend the result of the last operation back on top.
        PixelSurface restored = before.Surface.Clone();
        current.Surface = restored;
        current.Offset = before.Offset;
        current.EnsureCovers(doc.Bounds);
        PixelSurface? selection = doc.Selection;
        doc.SetSelectionLive(null);
        LayerOps.CompositeMasked(doc, current, (x, y) => ImageOps.GetPixel(after, x, y), null, opacity, mode);
        doc.SetSelectionLive(selection);
        w.Commit("Fade");
    }

    private static async Task DefineBrushAsync(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        using SKBitmap flat = Compositor.Flatten(doc);
        SKRectI bounds = doc.SelectionBounds;
        string? name = await DialogBuilder.PromptAsync(w, "Brush Name", "Name:", $"Sampled Brush {w.BrushPresets.Count + 1}");
        if (string.IsNullOrWhiteSpace(name)) return;
        SKBitmap tip = ImageOps.Crop(flat, bounds);
        w.BrushPresets.Add(new BrushPreset { Name = name, Shape = BrushTipShape.Sampled, SampledTip = tip, Size = Math.Max(bounds.Width, bounds.Height), Hardness = 1, Spacing = 0.25f });
        w.RefreshPanels();
    }

    private static async Task DefinePatternAsync(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        if (doc.Selection == null && (doc.Width > 1024 || doc.Height > 1024))
        {
            w.ShowMessage("Make a rectangular selection of the pattern tile first.");
            return;
        }
        string? name = await DialogBuilder.PromptAsync(w, "Pattern Name", "Name:", $"Pattern {w.Patterns.Count + 1}");
        if (string.IsNullOrWhiteSpace(name)) return;
        using SKBitmap flat = Compositor.Flatten(doc);
        PatternDefinition pattern = new(name, ImageOps.Crop(flat, doc.SelectionBounds));
        w.Patterns.Add(pattern);
        w.Pattern = pattern;
        w.RefreshOptions();
    }

    private static async Task DefineShapeAsync(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        VectorPath? path = doc.ActiveLayer is ShapeLayer shape ? shape.Path : doc.ActivePath?.Path;
        if (path == null || path.IsEmpty) return;
        string? name = await DialogBuilder.PromptAsync(w, "Shape Name", "Name:", $"Shape {CustomShape.Defaults.Count + 1}");
        if (string.IsNullOrWhiteSpace(name)) return;
        using SKPath sk = path.ToSKPath();
        SKRect b = sk.Bounds;
        sk.Transform(SKMatrix.CreateTranslation(-b.Left, -b.Top).PostConcat(SKMatrix.CreateScale(100 / Math.Max(1, b.Width), 100 / Math.Max(1, b.Height))));
        CustomShape.Defaults.Add(new CustomShape(name, sk.ToSvgPathData()));
        w.RefreshOptions();
    }
}
