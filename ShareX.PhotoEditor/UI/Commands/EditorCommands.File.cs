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
using Avalonia.Platform.Storage;
using ShareX.HelpersLib;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Dialogs;

namespace ShareX.PhotoEditor.UI.Commands;

public static partial class EditorCommands
{
    private static void RegisterFile()
    {
        R("file.new", "New...", "Ctrl+N", w => _ = NewDocumentAsync(w));
        R("file.open", "Open...", "Ctrl+O", w => _ = OpenAsync(w));
        R("file.newFromClipboard", "New from Clipboard", "Ctrl+Alt+N", NewFromClipboard);
        R("file.newFromRegion", "New from Screenshot (Region)...", null, w => _ = NewFromCaptureAsync(w, false), w => w.Services.CaptureRegion != null);
        R("file.newFromScreen", "New from Screenshot (Full Screen)", null, w => _ = NewFromCaptureAsync(w, true), w => w.Services.CaptureFullscreen != null);
        R("file.close", "Close", "Ctrl+W", w => { if (w.View != null) _ = w.CloseViewAsync(w.View); }, HasDoc);
        R("file.closeAll", "Close All", "Ctrl+Alt+W", w => _ = CloseAllAsync(w), HasDoc);
        R("file.save", "Save", "Ctrl+S", w => _ = SaveAsync(w, false), HasDoc);
        R("file.saveAs", "Save As...", "Ctrl+Shift+S", w => _ = SaveAsync(w, true), HasDoc);
        R("file.saveCopy", "Save a Copy...", "Ctrl+Alt+S", w => _ = SaveCopyAsync(w), HasDoc);
        R("file.revert", "Revert", "F12", Revert, w => w.Document?.FilePath != null);
        R("file.quickExport", "Quick Export as PNG", "Ctrl+Shift+Alt+Q", w => _ = QuickExportAsync(w), HasDoc);
        R("file.exportAs", "Export As...", "Ctrl+Shift+Alt+W", w => _ = ExportAsAsync(w), HasDoc);
        R("file.exportSlices", "Save Slices...", null, w => _ = ExportSlicesAsync(w), w => w.Document?.Slices.Count > 0);
        R("file.exportLayers", "Layers to Files...", null, w => _ = ExportLayersAsync(w), HasDoc);
        R("file.place", "Place Embedded...", "Ctrl+Shift+P", w => _ = PlaceAsync(w), HasDoc);
        R("file.upload", "Upload with ShareX", null, w => SendFlat(w, w.Services.UploadImage), w => HasDoc(w) && w.Services.UploadImage != null);
        R("file.runTasks", "Send to ShareX (After Capture Tasks)", "Ctrl+Shift+Alt+U", w => SendFlat(w, b => w.Services.RunImageTask?.Invoke(b, w.Document?.FilePath)), w => HasDoc(w) && w.Services.RunImageTask != null);
        R("file.pin", "Pin to Screen", null, w => SendFlat(w, w.Services.PinImage), w => HasDoc(w) && w.Services.PinImage != null);
        R("file.print", "Print...", "Ctrl+P", w => SendFlat(w, w.Services.PrintImage), w => HasDoc(w) && w.Services.PrintImage != null);
        R("file.exit", "Exit", "Ctrl+Q", w => w.Close());
    }

    private static IEnumerable<MenuNode> RecentFiles(PhotoEditorWindow w) =>
        w.Settings.RecentFiles.Where(File.Exists).Take(15).Select(path => new MenuNode { Header = Path.GetFileName(path) + "   " + Path.GetDirectoryName(path), Action = win => OpenFile(win, path) });

    public static async Task NewDocumentAsync(PhotoEditorWindow w)
    {
        SKSizeI clipboardSize = default;
        using (SKBitmap? clip = TryGetClipboardImage()) if (clip != null) clipboardSize = new SKSizeI(clip.Width, clip.Height);
        NewDocumentDialog.Result? result = await NewDocumentDialog.ShowAsync(w, clipboardSize, w.Background);
        if (result == null) return;
        PhotoDocument doc;
        try
        {
            doc = new PhotoDocument(result.Width, result.Height, result.Resolution, true, result.Background);
        }
        catch (Exception ex)
        {
            w.ShowMessage($"Could not create the document: {ex.Message}");
            return;
        }
        doc.Title = result.Name;
        w.OpenDocument(doc, "New");
    }

    public static async Task OpenAsync(PhotoEditorWindow w)
    {
        foreach (string path in await w.PickOpenFilesAsync("Open")) OpenFile(w, path);
    }

    public static void OpenFile(PhotoEditorWindow w, string path)
    {
        DocumentView? existing = w.Views.FirstOrDefault(v => string.Equals(v.Document.FilePath, path, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            w.ActivateView(existing);
            return;
        }
        try
        {
            PhotoDocument doc = ImageIO.Open(path);
            w.Settings.AddRecentFile(path);
            w.OpenDocument(doc);
        }
        catch (Exception ex)
        {
            w.ShowMessage(ex.Message.StartsWith("Could not open", StringComparison.Ordinal) ? ex.Message : $"Could not open \"{Path.GetFileName(path)}\": {ex.Message}");
        }
    }

    /// <summary>Opens an in-memory image (for example from a ShareX capture).</summary>
    public static void OpenBitmap(PhotoEditorWindow w, SKBitmap bitmap, string title, string? filePath = null)
    {
        PhotoDocument doc = ImageIO.FromBitmap(bitmap, title);
        doc.FilePath = filePath != null && !ImageIO.IsProject(filePath) ? filePath : null;
        w.OpenDocument(doc);
    }

    public static SKBitmap? TryGetClipboardImage()
    {
        try
        {
            return ClipboardHelpers.TryGetImage();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static void NewFromClipboard(PhotoEditorWindow w)
    {
        using SKBitmap? image = TryGetClipboardImage();
        if (image == null)
        {
            w.ShowMessage("The clipboard does not contain an image.");
            return;
        }
        OpenBitmap(w, image, "Clipboard");
    }

    private static async Task NewFromCaptureAsync(PhotoEditorWindow w, bool fullscreen)
    {
        Func<Task<SKBitmap?>>? capture = fullscreen ? w.Services.CaptureFullscreen : w.Services.CaptureRegion;
        if (capture == null) return;
        w.Hide();
        try
        {
            await Task.Delay(250);
            using SKBitmap? image = await capture();
            if (image != null) OpenBitmap(w, image, "Screenshot");
        }
        finally
        {
            w.Show();
            w.Activate();
        }
    }

    private static async Task CloseAllAsync(PhotoEditorWindow w)
    {
        foreach (DocumentView view in w.Views.ToList())
        {
            if (!await w.CloseViewAsync(view)) return;
        }
    }

    /// <summary>Saves the active document. Layered documents default to the project format to keep layers.</summary>
    public static async Task<bool> SaveAsync(PhotoEditorWindow w, bool saveAs)
    {
        PhotoDocument? doc = w.Document;
        if (doc == null) return false;
        string? path = doc.FilePath;
        bool layered = doc.AllLayers.Count() > 1 || doc.AllLayers.Any(l => l is not RasterLayer) || doc.Channels.Count > 0 || doc.Paths.Count > 0;
        if (!saveAs && path != null && !ImageIO.IsProject(path) && layered)
        {
            bool? keep = await DialogBuilder.ConfirmAsync(w, "Save", $"\"{Path.GetFileName(path)}\" does not support layers. Save a flattened copy in this format, or save as a ShareX Photo Editor project to keep the layers?",
                "Flatten", "Save as Project");
            if (keep == null) return false;
            if (keep == false) saveAs = true;
        }
        if (saveAs || path == null)
        {
            string name = Path.GetFileNameWithoutExtension(doc.FilePath ?? doc.Title);
            path = await w.PickSaveFileAsync("Save As", name + (layered ? ImageIO.ProjectExtension : ".png"), layered);
            if (path == null) return false;
        }
        int quality = 90;
        if (Path.GetExtension(path).ToLowerInvariant() is ".jpg" or ".jpeg" or ".webp")
        {
            int? q = await QualityDialogAsync(w);
            if (q == null) return false;
            quality = q.Value;
        }
        try
        {
            ImageIO.Save(doc, path, quality);
            doc.FilePath = path;
            doc.Title = Path.GetFileName(path);
            doc.IsDirty = false;
            w.Settings.AddRecentFile(path);
            w.RefreshPanels();
            w.SetStatus($"Saved {path}");
            return true;
        }
        catch (Exception ex)
        {
            w.ShowMessage($"Could not save \"{Path.GetFileName(path)}\": {ex.Message}");
            return false;
        }
    }

    private static async Task<int?> QualityDialogAsync(PhotoEditorWindow w)
    {
        int quality = 90;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, "Quality:", DialogBuilder.SliderNumber(1, 100, quality, v => quality = (int)v));
        return await DialogBuilder.ShowAsync(w, "Image Options", form, 380) ? quality : null;
    }

    private static async Task SaveCopyAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        string? path = await w.PickSaveFileAsync("Save a Copy", Path.GetFileNameWithoutExtension(doc.Title) + " copy.png", false);
        if (path == null) return;
        try
        {
            ImageIO.Save(doc, path);
            w.SetStatus($"Saved a copy to {path}");
        }
        catch (Exception ex)
        {
            w.ShowMessage($"Could not save: {ex.Message}");
        }
    }

    private static void Revert(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc?.FilePath == null) return;
        try
        {
            // The reverted document is not disposed: the restored state shares its pixel surfaces.
            PhotoDocument reverted = ImageIO.Open(doc.FilePath);
            doc.RestoreState(reverted.CaptureState());
            doc.History.Push("Revert");
            doc.IsDirty = false;
            w.AfterHistoryChange();
        }
        catch (Exception ex)
        {
            w.ShowMessage($"Could not revert: {ex.Message}");
        }
    }

    private static async Task QuickExportAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        string? path = await w.PickSaveFileAsync("Quick Export as PNG", Path.GetFileNameWithoutExtension(doc.Title) + ".png", false);
        if (path == null) return;
        try
        {
            using SKBitmap flat = Compositor.Flatten(doc);
            ImageIO.SaveBitmap(flat, path);
            w.SetStatus($"Exported {path}");
        }
        catch (Exception ex)
        {
            w.ShowMessage($"Could not export: {ex.Message}");
        }
    }

    private static async Task ExportAsAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        ExportDialog.Result? options = await ExportDialog.ShowAsync(w, doc);
        if (options == null) return;
        string? path = await w.PickSaveFileAsync("Export As", Path.GetFileNameWithoutExtension(doc.Title) + options.Extension, false);
        if (path == null) return;
        using SKBitmap flat = Compositor.Flatten(doc);
        SKBitmap output = flat;
        if (options.Width != doc.Width || options.Height != doc.Height) output = ImageOps.Resize(flat, options.Width, options.Height, ResampleMethod.Bicubic);
        try
        {
            ImageIO.SaveBitmap(output, path, options.Quality);
            w.SetStatus($"Exported {path}");
        }
        catch (Exception ex)
        {
            w.ShowMessage($"Could not export: {ex.Message}");
        }
        finally
        {
            if (!ReferenceEquals(output, flat)) output.Dispose();
        }
    }

    private static async Task<string?> PickFolderAsync(PhotoEditorWindow w, string title)
    {
        IReadOnlyList<IStorageFolder> folders = await w.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = title });
        return folders.FirstOrDefault()?.TryGetLocalPath();
    }

    private static async Task ExportSlicesAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        string? folder = await PickFolderAsync(w, "Save Slices");
        if (folder == null) return;
        try
        {
            using SKBitmap flat = Compositor.Flatten(doc);
            int index = 1;
            foreach (SliceItem slice in doc.Slices)
            {
                SKRectI bounds = slice.Bounds;
                bounds.Intersect(doc.Bounds);
                if (bounds.IsEmpty) continue;
                using SKBitmap part = ImageOps.Crop(flat, bounds);
                string name = string.IsNullOrWhiteSpace(slice.Name) ? $"slice_{index:00}" : string.Concat(slice.Name.Split(Path.GetInvalidFileNameChars()));
                ImageIO.SaveBitmap(part, Path.Combine(folder, name + ".png"));
                index++;
            }
            w.SetStatus($"Saved {index - 1} slices to {folder}");
        }
        catch (Exception ex)
        {
            w.ShowMessage($"Could not save slices: {ex.Message}");
        }
    }

    private static async Task ExportLayersAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        string? folder = await PickFolderAsync(w, "Layers to Files");
        if (folder == null) return;
        try
        {
            int index = 0;
            foreach (Layer layer in doc.AllLayers.Where(l => l is not GroupLayer and not AdjustmentLayer))
            {
                using SKBitmap image = Compositor.RenderLayer(doc, layer);
                string name = $"{index++:000}_{string.Concat(layer.Name.Split(Path.GetInvalidFileNameChars()))}.png";
                ImageIO.SaveBitmap(image, Path.Combine(folder, name));
            }
            w.SetStatus($"Exported {index} layers to {folder}");
        }
        catch (Exception ex)
        {
            w.ShowMessage($"Could not export layers: {ex.Message}");
        }
    }

    /// <summary>Places an image file as a smart object layer centered on the canvas.</summary>
    public static void PlaceFile(PhotoEditorWindow w, string path)
    {
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        SKBitmap? bitmap = ImageIO.DecodeImage(path);
        if (bitmap == null)
        {
            w.ShowMessage($"Could not place \"{Path.GetFileName(path)}\" because the file format is not supported.");
            return;
        }
        using (bitmap) PlaceBitmap(w, bitmap, Path.GetFileNameWithoutExtension(path));
    }

    public static void PlaceBitmap(PhotoEditorWindow w, SKBitmap bitmap, string name)
    {
        PhotoDocument doc = w.Document!;
        if (doc.ActiveLayer is FrameLayer frame)
        {
            frame.PlaceContent(PixelSurface.FromBitmap(bitmap));
            w.Commit("Place Into Frame");
            return;
        }
        SmartObjectLayer layer = new(name, PixelSurface.FromBitmap(bitmap));
        float scale = Math.Min(1, Math.Min(doc.Width / (float)bitmap.Width, doc.Height / (float)bitmap.Height));
        layer.Transform = SKMatrix.CreateScale(scale, scale).PostConcat(SKMatrix.CreateTranslation((doc.Width - bitmap.Width * scale) / 2, (doc.Height - bitmap.Height * scale) / 2));
        doc.AddLayer(layer);
        w.Commit("Place Embedded");
    }

    private static async Task PlaceAsync(PhotoEditorWindow w)
    {
        IReadOnlyList<string> files = await w.PickOpenFilesAsync("Place Embedded", false);
        if (files.Count > 0) PlaceFile(w, files[0]);
    }

    public static async void PlaceIntoFrame(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        if (w.Document?.ActiveLayer is not FrameLayer)
        {
            w.ShowMessage("Select a frame layer first (draw one with the Frame tool).");
            return;
        }
        IReadOnlyList<string> files = await w.PickOpenFilesAsync("Place Image Into Frame", false);
        if (files.Count > 0) PlaceFile(w, files[0]);
    }

    private static void SendFlat(PhotoEditorWindow w, Action<SKBitmap>? action)
    {
        if (w.Document == null || action == null) return;
        SKBitmap flat = Compositor.Flatten(w.Document);
        try
        {
            action(flat);
        }
        finally
        {
            flat.Dispose();
        }
    }
}
