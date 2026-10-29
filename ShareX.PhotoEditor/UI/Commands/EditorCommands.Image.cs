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
        (AdjustmentKind.PhotoFilter, null), (AdjustmentKind.ChannelMixer, null), (AdjustmentKind.Invert, "Ctrl+I"), (AdjustmentKind.Posterize, null),
        (AdjustmentKind.Threshold, null), (AdjustmentKind.GradientMap, null), (AdjustmentKind.SelectiveColor, null), (AdjustmentKind.Desaturate, "Ctrl+Shift+U")
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
        R("image.rotate180", "180°", null, w => RotateCanvas(w, 180), HasDoc);
        R("image.rotate90cw", "90° Clockwise", null, w => RotateCanvas(w, 90), HasDoc);
        R("image.rotate90ccw", "90° Counter Clockwise", null, w => RotateCanvas(w, -90), HasDoc);
        R("image.rotateArbitrary", "Arbitrary...", null, w => _ = RotateArbitraryAsync(w), HasDoc);
        R("image.flipH", "Flip Canvas Horizontal", null, w => { DocumentTransforms.Flip(w.Document!, true); w.Commit("Flip Canvas Horizontal"); }, HasDoc);
        R("image.flipV", "Flip Canvas Vertical", null, w => { DocumentTransforms.Flip(w.Document!, false); w.Commit("Flip Canvas Vertical"); }, HasDoc);
        R("image.crop", "Crop", null, CropToSelection, HasSelection);
        R("image.trim", "Trim...", null, w => _ = TrimAsync(w), HasDoc);
        R("image.revealAll", "Reveal All", null, RevealAll, HasDoc);
        R("image.duplicate", "Duplicate...", null, DuplicateDocument, HasDoc);
    }

    /// <summary>Applies an adjustment to the active layer (within the selection) without a dialog.</summary>
    private static void ApplyAdjustmentDirect(PhotoEditorWindow w, AdjustmentSettings settings, string name)
    {
        PhotoDocument doc = w.Document!;
        if (doc.ActiveLayer is SmartObjectLayer smart)
        {
            AddSmartFilter(w, smart, name, settings.Kind.ToString(), b => { SKBitmap copy = b.Copy(); Adjustments.Apply(settings, copy, new SKRectI(0, 0, copy.Width, copy.Height)); return copy; });
            return;
        }
        RasterLayer? layer = EditableRaster(w, name.ToLowerInvariant());
        if (layer == null) return;
        layer.EnsureCovers(doc.Bounds);
        PixelSurface surface = layer.GetWritableSurface();
        SKRectI area = doc.SelectionBounds;
        area.Offset(-layer.Offset.X, -layer.Offset.Y);
        Adjustments.Apply(settings, surface.Bitmap, area, doc.Selection == null ? null : (x, y) => doc.SelectionAt(x + layer.Offset.X, y + layer.Offset.Y));
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
                AdjustmentSettings captured = settings.Clone();
                AddSmartFilter(w, smart, name, $"{kind}:{string.Join(",", captured.Values.Select(v => v.Key + v.Value))}", b =>
                {
                    SKBitmap copy = b.Copy();
                    Adjustments.Apply(captured, copy, new SKRectI(0, 0, copy.Width, copy.Height));
                    return copy;
                });
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

    private static void AddSmartFilter(PhotoEditorWindow w, SmartObjectLayer smart, string name, string key, Func<SKBitmap, SKBitmap> apply)
    {
        smart.Filters.Add(new SmartFilter(name, key + "#" + smart.Filters.Count, apply));
        w.Commit(name + " (Smart Filter)");
    }

    private static void Equalize(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        RasterLayer? layer = EditableRaster(w, "equalize");
        if (layer == null) return;
        layer.EnsureCovers(doc.Bounds);
        PixelSurface surface = layer.GetWritableSurface();
        SKRectI area = doc.SelectionBounds;
        area.Offset(-layer.Offset.X, -layer.Offset.Y);
        area.Intersect(new SKRectI(0, 0, surface.Width, surface.Height));
        Adjustments.Equalize(surface.Bitmap, area, doc.Selection == null ? null : (x, y) => doc.SelectionAt(x + layer.Offset.X, y + layer.Offset.Y));
        w.Commit("Equalize");
    }

    private static void AutoLevels(PhotoEditorWindow w, bool perChannel, string name)
    {
        RasterLayer? layer = EditableRaster(w, name.ToLowerInvariant());
        if (layer == null) return;
        AdjustmentSettings settings = Adjustments.AutoLevels(layer.Surface.Bitmap, perChannel, perChannel ? 0.001f : 0.005f);
        ApplyAdjustmentDirect(w, settings, name);
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
        await w.RunBusyAsync("Resizing...", _ =>
        {
            DocumentTransforms.ResizeImage(doc, result.Width, result.Height, result.Method, result.Resolution);
            return Task.CompletedTask;
        });
        w.View?.FitOnScreen();
        w.Commit("Image Size");
    }

    private static async Task CanvasSizeAsync(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        CanvasSizeDialog.Result? result = await CanvasSizeDialog.ShowAsync(w, doc.Width, doc.Height, w.Background);
        if (result == null) return;
        DocumentTransforms.CanvasSize(doc, result.Width, result.Height, result.Anchor, result.Extension);
        w.View?.FitOnScreen();
        w.Commit("Canvas Size");
    }

    private static void RotateCanvas(PhotoEditorWindow w, float degrees)
    {
        DocumentTransforms.Rotate(w.Document!, degrees, w.Background);
        w.View?.FitOnScreen();
        w.Commit($"Rotate Canvas {degrees}°");
    }

    /// <summary>Rotates the canvas (background layers) or just the active layer (used by the Ruler tool).</summary>
    public static void RotateCanvasOrLayer(Tools.IEditorHost host, float degrees, bool canvas)
    {
        PhotoEditorWindow w = W(host);
        if (canvas)
        {
            RotateCanvas(w, degrees);
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
        RotateCanvas(w, (float)(clockwise ? angle : -angle));
    }

    private static void CropToSelection(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        SKRectI bounds = doc.SelectionBounds;
        doc.SetSelectionLive(null);
        DocumentTransforms.Crop(doc, bounds, true, w.Background);
        w.View?.FitOnScreen();
        w.Commit("Crop");
    }

    private static async Task TrimAsync(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        int mode = 0;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Based on:", DialogBuilder.Combo([("Transparent Pixels", 0), ("Top Left Pixel Color", 1), ("Bottom Right Pixel Color", 2)], mode, v => mode = v));
        if (!await DialogBuilder.ShowAsync(w, "Trim", form, 380)) return;
        using SKBitmap flat = Compositor.Flatten(doc);
        SKRectI bounds = DocumentTransforms.TrimBounds(flat, mode == 0, mode == 1);
        if (bounds.IsEmpty || bounds == doc.Bounds) return;
        DocumentTransforms.Crop(doc, bounds, true, w.Background);
        w.View?.FitOnScreen();
        w.Commit("Trim");
    }

    private static void RevealAll(PhotoEditorWindow w)
    {
        PhotoDocument doc = w.Document!;
        SKRectI bounds = DocumentTransforms.ContentBounds(doc);
        if (bounds == doc.Bounds) return;
        DocumentTransforms.ApplyMatrix(doc, SKMatrix.CreateTranslation(-bounds.Left, -bounds.Top), bounds.Width, bounds.Height, ResampleMethod.NearestNeighbor, w.Background, true);
        w.View?.FitOnScreen();
        w.Commit("Reveal All");
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
