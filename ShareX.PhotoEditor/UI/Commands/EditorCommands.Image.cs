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
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Commands;

public static partial class EditorCommands
{
    public static readonly (AdjustmentKind Kind, string? Gesture)[] AdjustmentMenu =
    [
        (AdjustmentKind.BrightnessContrast, null), (AdjustmentKind.Levels, "Ctrl+L"), (AdjustmentKind.Curves, "Ctrl+M"), (AdjustmentKind.Exposure, null),
        (AdjustmentKind.Vibrance, null), (AdjustmentKind.HueSaturation, "Ctrl+U"), (AdjustmentKind.ColorBalance, "Ctrl+B"), (AdjustmentKind.BlackAndWhite, "Ctrl+Shift+Alt+B"),
        (AdjustmentKind.PhotoFilter, null), (AdjustmentKind.ChannelMixer, null), (AdjustmentKind.ColorLookup, null), (AdjustmentKind.Invert, "Ctrl+I"), (AdjustmentKind.Posterize, null),
        (AdjustmentKind.Threshold, null), (AdjustmentKind.GradientMap, null), (AdjustmentKind.SelectiveColor, null), (AdjustmentKind.Desaturate, "Ctrl+Shift+U"), (AdjustmentKind.ReplaceColor, null)
    ];

    /// <summary>Adjustment layer kinds, grouped as in the New Adjustment Layer menu and the Adjustments panel.</summary>
    public static readonly AdjustmentKind[][] AdjustmentLayerGroups =
    [
        [AdjustmentKind.BrightnessContrast, AdjustmentKind.Levels, AdjustmentKind.Curves, AdjustmentKind.Exposure],
        [AdjustmentKind.Vibrance, AdjustmentKind.HueSaturation, AdjustmentKind.ColorBalance, AdjustmentKind.BlackAndWhite, AdjustmentKind.PhotoFilter, AdjustmentKind.ChannelMixer, AdjustmentKind.ColorLookup],
        [AdjustmentKind.Invert, AdjustmentKind.Posterize, AdjustmentKind.Threshold, AdjustmentKind.GradientMap, AdjustmentKind.SelectiveColor, AdjustmentKind.Desaturate]
    ];

    private static void RegisterImage()
    {
        R("image.mode.grayscale", "Grayscale", null, w => ApplyAdjustmentDirect(w, new AdjustmentSettings(AdjustmentKind.Desaturate), "Grayscale"), HasDoc);
        foreach (var (kind, gesture) in AdjustmentMenu)
        {
            AdjustmentKind captured = kind;
            bool immediate = kind is AdjustmentKind.Invert or AdjustmentKind.Desaturate;
            R($"image.adjust.{kind}", AdjustmentSettings.GetName(kind) + (immediate ? string.Empty : "..."), gesture,
                w => { if (immediate) ApplyAdjustmentDirect(w, new AdjustmentSettings(captured), AdjustmentSettings.GetName(captured)); else _ = AdjustmentDialogAsync(w, captured); }, HasDoc);
        }
        R("image.adjust.equalize", "Equalize", null, Equalize, HasDoc);
        R("image.adjust.shadowsHighlights", "Shadows/Highlights...", null, w => _ = LibraryEffectAsync(w, "ShadowsHighlights"), HasDoc);
        R("image.autoTone", "Auto Tone", "Ctrl+Shift+L", w => AutoLevels(w, true, "Auto Tone"), HasDoc);
        R("image.autoContrast", "Auto Contrast", "Ctrl+Shift+Alt+L", w => AutoLevels(w, false, "Auto Contrast"), HasDoc);
        R("image.autoColor", "Auto Color", "Ctrl+Shift+B", w => AutoLevels(w, true, "Auto Color"), HasDoc);
        R("image.imageSize", "Image Size...", "Ctrl+Alt+I", w => _ = ImageSizeAsync(w), HasDoc);
        R("image.canvasSize", "Canvas Size...", "Ctrl+Alt+C", w => _ = CanvasSizeAsync(w), HasDoc);
        R("image.rotate180", "180°", null, w => _ = RotateCanvasAsync(w, 180), HasDoc);
        R("image.rotate90cw", "90° Clockwise", null, w => _ = RotateCanvasAsync(w, 90), HasDoc);
        R("image.rotate90ccw", "90° Counter Clockwise", null, w => _ = RotateCanvasAsync(w, -90), HasDoc);
        R("image.rotateArbitrary", "Arbitrary...", null, w => _ = RotateArbitraryAsync(w), HasDoc);
        R("image.flipH", "Flip Canvas Horizontal", null, w => _ = EditDocumentAsync(w, "flip the canvas", "Flip Canvas Horizontal", d => DocumentTransforms.Flip(d, true)), HasDoc);
        R("image.flipV", "Flip Canvas Vertical", null, w => _ = EditDocumentAsync(w, "flip the canvas", "Flip Canvas Vertical", d => DocumentTransforms.Flip(d, false)), HasDoc);
        R("image.crop", "Crop", null, w => _ = CropToSelectionAsync(w), HasSelection);
        R("image.trim", "Trim...", null, w => _ = TrimAsync(w), HasDoc);
        R("image.revealAll", "Reveal All", null, w => _ = RevealAllAsync(w), HasDoc);
        R("image.duplicate", "Duplicate...", null, DuplicateDocument, HasDoc);
    }

    /// <summary>Prepares a layer for an in-place edit limited to the selection: its writable surface, the edited area in surface pixels and the selection weight.</summary>
    private static (PixelSurface Surface, SKRectI Area, Func<int, int, float>? Weight) BeginSelectionEdit(PhotoDocument doc, RasterLayer layer)
    {
        layer.EnsureCovers(doc.Bounds);
        PixelSurface surface = layer.GetWritableSurface();
        SKRectI area = doc.SelectionBounds;
        area.Offset(-layer.Offset.X, -layer.Offset.Y);
        area.Intersect(new SKRectI(0, 0, surface.Width, surface.Height));
        Func<int, int, float>? weight = doc.Selection == null ? null : (x, y) => doc.SelectionAt(x + layer.Offset.X, y + layer.Offset.Y);
        return (surface, area, weight);
    }

    /// <summary>Applies an adjustment to the active layer (within the selection) without a dialog.</summary>
    private static void ApplyAdjustmentDirect(PhotoEditorWindow w, AdjustmentSettings settings, string name)
    {
        PhotoDocument doc = w.Document!;
        if (doc.ActiveLayer is SmartObjectLayer smart)
        {
            AddSmartFilter(w, smart, SmartFilterFactory.CreateAdjustment(name, settings, smart.Filters.Count));
            return;
        }
        RasterLayer? layer = EditableRaster(w, name.ToLowerInvariant());
        if (layer == null) return;
        (PixelSurface surface, SKRectI area, Func<int, int, float>? weight) = BeginSelectionEdit(doc, layer);
        Adjustments.Apply(settings, surface.Bitmap, area, weight);
        w.Commit(name);
    }

    private static async Task AdjustmentDialogAsync(PhotoEditorWindow w, AdjustmentKind kind)
    {
        PhotoDocument doc = w.Document!;
        string name = AdjustmentSettings.GetName(kind);
        AdjustmentSettings settings = new(kind);
        if (doc.ActiveLayer is AdjustmentLayer existing && existing.Settings.Kind == kind)
        {
            w.Panels.ShowPanel("properties");
            return;
        }
        if (doc.ActiveLayer is SmartObjectLayer smart)
        {
            Control smartEditor = AdjustmentEditor.Build(settings, () => { }, smart.Source.Bitmap, w);
            if (await DialogBuilder.ShowAsync(w, name, smartEditor, 460))
            {
                AddSmartFilter(w, smart, SmartFilterFactory.CreateAdjustment(name, settings, smart.Filters.Count));
            }
            return;
        }
        RasterLayer? layer = EditableRaster(w, "apply the adjustment");
        if (layer == null) return;
        PreviewSession session = new(w, layer, 0);
        Func<SKBitmap, SKBitmap> Filter()
        {
            AdjustmentSettings snapshot = settings.Clone();
            return b =>
            {
                SKBitmap copy = b.Copy();
                Adjustments.Apply(snapshot, copy, new SKRectI(0, 0, copy.Width, copy.Height));
                return copy;
            };
        }
        using SKBitmap histogramSource = w.GetSampleImage(false);
        Control editor = AdjustmentEditor.Build(settings, () => session.Preview(Filter()), histogramSource, w);
        CheckBox preview = new() { Content = "Preview", IsChecked = true };
        preview.IsCheckedChanged += (_, _) =>
        {
            session.PreviewEnabled = preview.IsChecked == true;
            session.Preview(Filter());
        };
        StackPanelWith(editor, preview, out Control content);
        session.Preview(Filter());
        if (await DialogBuilder.ShowAsync(w, name, content, 480)) await session.CommitAsync(Filter(), name);
        else session.Cancel();
    }

    private static void StackPanelWith(Control main, Control extra, out Control result)
    {
        StackPanel panel = new() { Spacing = 10 };
        panel.Children.Add(main);
        panel.Children.Add(extra);
        result = panel;
    }

    private static void AddSmartFilter(PhotoEditorWindow w, SmartObjectLayer smart, SmartFilter filter)
    {
        smart.Filters.Add(filter);
        w.Commit(filter.Name + " (Smart Filter)");
    }

    private static void Equalize(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        RasterLayer? layer = EditableRaster(w, "equalize");
        if (layer == null) return;
        (PixelSurface surface, SKRectI area, Func<int, int, float>? weight) = BeginSelectionEdit(doc, layer);
        Adjustments.Equalize(surface.Bitmap, area, weight);
        w.Commit("Equalize");
    }

    private static void AutoLevels(PhotoEditorWindow w, bool perChannel, string name)
    {
        RasterLayer? layer = EditableRaster(w, name.ToLowerInvariant());
        if (layer == null) return;
        AdjustmentSettings settings = Adjustments.AutoLevels(layer.Surface.Bitmap, perChannel, perChannel ? 0.001f : 0.005f);
        ApplyAdjustmentDirect(w, settings, name);
    }

    /// <summary>Documents with at least this many layer pixels are edited on a background thread behind the busy overlay.</summary>
    private const long BackgroundEditPixels = 3_000_000;

    /// <summary>
    /// Runs a whole-document edit on a private copy of the document and swaps the result in only when it succeeds, so a
    /// failure (for example out of memory) never leaves layers half transformed. Large documents are processed on a
    /// background thread so the window stays responsive. Returns false if the edit failed or was abandoned.
    /// </summary>
    private static async Task<bool> EditDocumentAsync(PhotoEditorWindow w, string action, string historyName, Action<PhotoDocument> edit)
    {
        PhotoDocument doc = w.Document!;
        DocumentState before = doc.CaptureState();
        PhotoDocument work = new(before.Width, before.Height, before.Resolution, createBackground: false);
        work.RestoreState(before);

        DocumentState? after = null;
        Exception? error = null;
        void Run()
        {
            try
            {
                edit(work);
                after = work.CaptureState();
            }
            catch (OperationCanceledException)
            {
                // The edit found nothing to do.
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }

        long pixels = work.Root.Surfaces.Sum(s => (long)s.Width * s.Height);
        if (pixels < BackgroundEditPixels) Run();
        else await w.RunBusyAsync(historyName + "...", _ => Task.Run(Run));

        if (error != null)
        {
            w.ShowMessage($"Could not {action}: {error.Message}");
            return false;
        }
        // The document may have been closed or replaced while the work ran.
        if (after == null || !ReferenceEquals(w.Document, doc)) return false;

        HashSet<int> selected = [.. doc.SelectedLayerIds];
        doc.RestoreState(after);
        foreach (int id in selected)
        {
            if (doc.FindLayer(id) != null) doc.SelectedLayerIds.Add(id);
        }
        w.View?.FitOnScreen();
        w.Commit(historyName);
        return true;
    }

    private static async Task ImageSizeAsync(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        ImageSizeDialog.Result? result = await ImageSizeDialog.ShowAsync(w, doc.Width, doc.Height, doc.Resolution, w.Settings.DefaultResample);
        if (result == null) return;
        if (result.Width == doc.Width && result.Height == doc.Height)
        {
            doc.Resolution = result.Resolution;
            w.Commit("Image Size");
            return;
        }
        await EditDocumentAsync(w, "resize the image", "Image Size", d => DocumentTransforms.ResizeImage(d, result.Width, result.Height, result.Method, result.Resolution));
    }

    private static async Task CanvasSizeAsync(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        CanvasSizeDialog.Result? result = await CanvasSizeDialog.ShowAsync(w, doc.Width, doc.Height, w.Background);
        if (result == null) return;
        await EditDocumentAsync(w, "change the canvas size", "Canvas Size", d => DocumentTransforms.CanvasSize(d, result.Width, result.Height, result.Anchor, result.Extension));
    }

    private static Task<bool> RotateCanvasAsync(PhotoEditorWindow w, float degrees)
    {
        SKColor background = w.Background;
        return EditDocumentAsync(w, "rotate the canvas", $"Rotate Canvas {degrees}°", d => DocumentTransforms.Rotate(d, degrees, background));
    }

    /// <summary>Rotates the canvas (background layers) or just the active layer (used by the Ruler tool).</summary>
    public static void RotateCanvasOrLayer(Tools.IEditorHost host, float degrees, bool canvas)
    {
        PhotoEditorWindow w = W(host);
        if (canvas)
        {
            _ = RotateCanvasAsync(w, degrees);
            return;
        }
        w.Tools.Get<Tools.TransformTool>().ApplyImmediate(bounds => SKMatrix.CreateRotationDegrees(degrees, bounds.MidX, bounds.MidY), "Straighten Layer");
    }

    private static async Task RotateArbitraryAsync(PhotoEditorWindow w)
    {
        double angle = 0;
        bool clockwise = true;
        Grid form = DialogBuilder.Form();
        NumericUpDown number = DialogBuilder.Number(-359.99, 359.99, 0, 0.1, "0.00");
        number.ValueChanged += (_, e) => angle = (double)(e.NewValue ?? 0);
        DialogBuilder.AddRow(form, "Angle (°):", number);
        DialogBuilder.AddRow(form, "Direction:", DialogBuilder.Combo([("Clockwise", true), ("Counter Clockwise", false)], clockwise, v => clockwise = v));
        if (!await DialogBuilder.ShowAsync(w, "Rotate Canvas", form, 360) || angle == 0) return;
        await RotateCanvasAsync(w, (float)(clockwise ? angle : -angle));
    }

    private static Task<bool> CropToSelectionAsync(PhotoEditorWindow w)
    {
        SKRectI bounds = w.Document!.SelectionBounds;
        SKColor background = w.Background;
        return EditDocumentAsync(w, "crop the image", "Crop", d =>
        {
            d.SetSelectionLive(null);
            DocumentTransforms.Crop(d, bounds, true, background);
        });
    }

    private static async Task TrimAsync(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        int mode = 0;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Based on:", DialogBuilder.Combo([("Transparent Pixels", 0), ("Top Left Pixel Color", 1), ("Bottom Right Pixel Color", 2)], mode, v => mode = v));
        if (!await DialogBuilder.ShowAsync(w, "Trim", form, 380)) return;
        SKColor background = w.Background;
        await EditDocumentAsync(w, "trim the image", "Trim", d =>
        {
            using SKBitmap flat = Compositor.Flatten(d);
            SKRectI bounds = DocumentTransforms.TrimBounds(flat, mode == 0, mode == 1);
            if (bounds.IsEmpty || bounds == d.Bounds) throw new OperationCanceledException();
            DocumentTransforms.Crop(d, bounds, true, background);
        });
    }

    private static Task<bool> RevealAllAsync(PhotoEditorWindow w)
    {
        SKColor background = w.Background;
        return EditDocumentAsync(w, "reveal all", "Reveal All", d =>
        {
            SKRectI bounds = DocumentTransforms.ContentBounds(d);
            if (bounds == d.Bounds) throw new OperationCanceledException();
            DocumentTransforms.ApplyMatrix(d, SKMatrix.CreateTranslation(-bounds.Left, -bounds.Top), bounds.Width, bounds.Height, ResampleMethod.NearestNeighbor, background, true);
        });
    }

    private static void DuplicateDocument(PhotoEditorWindow w)
    {
        PhotoDocument source = w.Document!;
        PhotoDocument copy = new(source.Width, source.Height, source.Resolution, createBackground: false);
        copy.RestoreState(source.CaptureState());
        foreach (Layer layer in copy.Root.Children.ToList())
        {
            copy.Root.Remove(layer);
            copy.Root.Add(layer.Duplicate());
        }
        copy.ActiveLayer = copy.Root.Children.LastOrDefault();
        copy.Title = source.DisplayName.TrimEnd('*') + " copy";
        w.OpenDocument(copy, "Duplicate Image");
    }

    public static void NewDocumentFromState(PhotoEditorWindow w) => DuplicateDocument(w);
}
