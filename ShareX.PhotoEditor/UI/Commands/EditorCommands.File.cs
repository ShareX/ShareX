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
        R("file.new", Strings.EditorCommands_NewEllipsis, "Ctrl+N", w => _ = NewDocumentAsync(w));
        R("file.open", Strings.EditorCommands_OpenEllipsis, "Ctrl+O", w => _ = OpenAsync(w));
        R("file.newFromClipboard", Strings.EditorCommands_NewFromClipboard, "Ctrl+Alt+N", NewFromClipboard);
        R("file.newFromRegion", Strings.EditorCommands_NewFromScreenshotRegionEllipsis, null, w => _ = NewFromCaptureAsync(w, false), w => w.Services.CaptureRegion != null);
        R("file.newFromScreen", Strings.EditorCommands_NewFromScreenshotFullScreen, null, w => _ = NewFromCaptureAsync(w, true), w => w.Services.CaptureFullscreen != null);
        R("file.close", Strings.EditorCommands_Close, "Ctrl+W", w => { if (w.View != null) _ = w.CloseViewAsync(w.View); }, HasDoc);
        R("file.closeAll", Strings.EditorCommands_CloseAll, "Ctrl+Alt+W", w => _ = CloseAllAsync(w), HasDoc);
        R("file.save", Strings.EditorCommands_Save, "Ctrl+S", w => _ = SaveAsync(w, false), HasDoc);
        R("file.saveAs", Strings.EditorCommands_SaveAsEllipsis, "Ctrl+Shift+S", w => _ = SaveAsync(w, true), HasDoc);
        R("file.saveCopy", Strings.EditorCommands_SaveACopyEllipsis, "Ctrl+Alt+S", w => _ = SaveCopyAsync(w), HasDoc);
        R("file.revert", Strings.EditorCommands_Revert, "F12", Revert, w => w.Document?.FilePath != null);
        R("file.quickExport", Strings.EditorCommands_QuickExportAsPng, "Ctrl+Shift+Alt+Q", w => _ = QuickExportAsync(w), HasDoc);
        R("file.exportAs", Strings.EditorCommands_ExportAsEllipsis, "Ctrl+Shift+Alt+W", w => _ = ExportAsAsync(w), HasDoc);
        R("file.exportSlices", Strings.EditorCommands_SaveSlicesEllipsis, null, w => _ = ExportSlicesAsync(w), w => w.Document?.Slices.Count > 0);
        R("file.exportLayers", Strings.EditorCommands_LayersToFilesEllipsis, null, w => _ = ExportLayersAsync(w), HasDoc);
        R("file.place", Strings.EditorCommands_PlaceEmbeddedEllipsis, "Ctrl+Shift+P", w => _ = PlaceAsync(w), HasDoc);
        R("file.upload", Strings.EditorCommands_UploadWithShareX, null, w => SendFlat(w, w.Services.UploadImage), w => HasDoc(w) && w.Services.UploadImage != null);
        R("file.runTasks", Strings.EditorCommands_SendToShareX, "Ctrl+Shift+Alt+U", w => SendFlat(w, b => w.Services.RunImageTask?.Invoke(b, w.Document?.FilePath)), w => HasDoc(w) && w.Services.RunImageTask != null);
        R("file.pin", Strings.EditorCommands_PinToScreen, null, w => SendFlat(w, w.Services.PinImage), w => HasDoc(w) && w.Services.PinImage != null);
        R("file.print", Strings.EditorCommands_PrintEllipsis, "Ctrl+P", w => SendFlat(w, w.Services.PrintImage), w => HasDoc(w) && w.Services.PrintImage != null);
        R("file.exit", Strings.EditorCommands_Exit, "Ctrl+Q", w => w.Close());
    }

    private static IEnumerable<MenuNode> RecentFiles(PhotoEditorWindow w)
    {
        // Doubled underscores keep file names intact; a single underscore would mark a menu access key.
        List<MenuNode> items = w.Settings.RecentFiles.Where(File.Exists).Take(15)
            .Select(path => new MenuNode { Header = Path.GetFileName(path).Replace("_", "__"), Action = win => OpenFile(win, path) }).ToList();
        if (items.Count > 0)
        {
            items.Add(MenuNode.Separator);
            items.Add(new MenuNode { Header = Strings.EditorCommands_ClearRecentFileList, Action = ClearRecentFiles });
        }
        return items;
    }

    public static void ClearRecentFiles(PhotoEditorWindow w)
    {
        w.Settings.RecentFiles.Clear();
        w.SaveSettings();
        w.RefreshStartScreen();
    }

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
            w.ShowMessage(string.Format(Strings.EditorCommands_CouldNotCreateDocument, ex.Message));
            return;
        }
        doc.Title = result.Name;
        w.OpenDocument(doc, Strings.EditorCommands_New);
    }

    public static async Task OpenAsync(PhotoEditorWindow w)
    {
        foreach (string path in await w.PickOpenFilesAsync(Strings.EditorCommands_Open)) OpenFile(w, path);
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
            w.ShowMessage(ex is DocumentOpenException ? ex.Message : string.Format(Strings.ImageIO_CouldNotOpenReason, Path.GetFileName(path), ex.Message));
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
            w.ShowMessage(Strings.EditorCommands_ClipboardHasNoImage);
            return;
        }
        OpenBitmap(w, image, Strings.EditorCommands_Clipboard);
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
            if (image != null) OpenBitmap(w, image, Strings.EditorCommands_Screenshot);
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
            bool? keep = await DialogBuilder.ConfirmAsync(w, Strings.EditorCommands_Save, string.Format(Strings.EditorCommands_FormatDoesNotSupportLayers, Path.GetFileName(path)),
                Strings.EditorCommands_Flatten, Strings.EditorCommands_SaveAsProject);
            if (keep == null) return false;
            if (keep == false) saveAs = true;
        }
        if (saveAs || path == null)
        {
            string name = Path.GetFileNameWithoutExtension(doc.FilePath ?? doc.Title);
            path = await w.PickSaveFileAsync(Strings.EditorCommands_SaveAs, name + (layered ? ImageIO.ProjectExtension : ".png"), layered);
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
            w.SetStatus(string.Format(Strings.EditorCommands_SavedPath, path));
            return true;
        }
        catch (Exception ex)
        {
            w.ShowMessage(string.Format(Strings.EditorCommands_CouldNotSaveFile, Path.GetFileName(path), ex.Message));
            return false;
        }
    }

    private static async Task<int?> QualityDialogAsync(PhotoEditorWindow w)
    {
        int quality = 90;
        Grid form = DialogBuilder.Form();
        DialogBuilder.AddRow(form, Strings.EditorCommands_QualityColon, DialogBuilder.SliderNumber(1, 100, quality, v => quality = (int)v));
        return await DialogBuilder.ShowAsync(w, Strings.EditorCommands_ImageOptions, form, 380) ? quality : null;
    }

    private static async Task SaveCopyAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        string? path = await w.PickSaveFileAsync(Strings.EditorCommands_SaveACopy, string.Format(Strings.LayerNames_Copy, Path.GetFileNameWithoutExtension(doc.Title)) + ".png", false);
        if (path == null) return;
        try
        {
            ImageIO.Save(doc, path);
            w.SetStatus(string.Format(Strings.EditorCommands_SavedCopyTo, path));
        }
        catch (Exception ex)
        {
            w.ShowMessage(string.Format(Strings.EditorCommands_CouldNotSave, ex.Message));
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
            doc.History.Push(Strings.EditorCommands_Revert);
            doc.IsDirty = false;
            w.AfterHistoryChange();
        }
        catch (Exception ex)
        {
            w.ShowMessage(string.Format(Strings.EditorCommands_CouldNotRevert, ex.Message));
        }
    }

    private static async Task QuickExportAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        string? path = await w.PickSaveFileAsync(Strings.EditorCommands_QuickExportAsPng, Path.GetFileNameWithoutExtension(doc.Title) + ".png", false);
        if (path == null) return;
        try
        {
            using SKBitmap flat = Compositor.Flatten(doc);
            ImageIO.SaveBitmap(flat, path);
            w.SetStatus(string.Format(Strings.EditorCommands_ExportedPath, path));
        }
        catch (Exception ex)
        {
            w.ShowMessage(string.Format(Strings.EditorCommands_CouldNotExport, ex.Message));
        }
    }

    private static async Task ExportAsAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        ExportDialog.Result? options = await ExportDialog.ShowAsync(w, doc, w.Settings);
        if (options == null) return;
        string? path = await w.PickSaveFileAsync(Strings.EditorCommands_ExportAs, Path.GetFileNameWithoutExtension(doc.Title) + options.Extension, false);
        if (path == null) return;
        using SKBitmap flat = Compositor.Flatten(doc);
        SKBitmap output = flat;
        if (options.Width != doc.Width || options.Height != doc.Height) output = ImageOps.Resize(flat, options.Width, options.Height, ResampleMethod.Bicubic);
        try
        {
            ImageIO.SaveBitmap(output, path, options.Quality, options.Lossless);
            w.SetStatus(string.Format(Strings.EditorCommands_ExportedPath, path));
        }
        catch (Exception ex)
        {
            w.ShowMessage(string.Format(Strings.EditorCommands_CouldNotExport, ex.Message));
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
        string? folder = await PickFolderAsync(w, Strings.EditorCommands_SaveSlices);
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
            w.SetStatus(string.Format(Strings.EditorCommands_SavedSlicesTo, index - 1, folder));
        }
        catch (Exception ex)
        {
            w.ShowMessage(string.Format(Strings.EditorCommands_CouldNotSaveSlices, ex.Message));
        }
    }

    private static async Task ExportLayersAsync(PhotoEditorWindow w)
    {
        PhotoDocument? doc = w.Document;
        if (doc == null) return;
        string? folder = await PickFolderAsync(w, Strings.EditorCommands_LayersToFiles);
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
            w.SetStatus(string.Format(Strings.EditorCommands_ExportedLayersTo, index, folder));
        }
        catch (Exception ex)
        {
            w.ShowMessage(string.Format(Strings.EditorCommands_CouldNotExportLayers, ex.Message));
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
            w.ShowMessage(string.Format(Strings.EditorCommands_CouldNotPlaceUnsupported, Path.GetFileName(path)));
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
            w.Commit(Strings.EditorCommands_PlaceIntoFrame);
            return;
        }
        SmartObjectLayer layer = new(name, PixelSurface.FromBitmap(bitmap));
        float scale = Math.Min(1, Math.Min(doc.Width / (float)bitmap.Width, doc.Height / (float)bitmap.Height));
        layer.Transform = SKMatrix.CreateScale(scale, scale).PostConcat(SKMatrix.CreateTranslation((doc.Width - bitmap.Width * scale) / 2, (doc.Height - bitmap.Height * scale) / 2));
        doc.AddLayer(layer);
        w.Commit(Strings.EditorCommands_PlaceEmbedded);
    }

    private static async Task PlaceAsync(PhotoEditorWindow w)
    {
        IReadOnlyList<string> files = await w.PickOpenFilesAsync(Strings.EditorCommands_PlaceEmbedded, false);
        if (files.Count > 0) PlaceFile(w, files[0]);
    }

    public static async void PlaceIntoFrame(IEditorHost host)
    {
        PhotoEditorWindow w = W(host);
        if (w.Document?.ActiveLayer is not FrameLayer)
        {
            w.ShowMessage(Strings.EditorCommands_SelectFrameLayerFirst);
            return;
        }
        IReadOnlyList<string> files = await w.PickOpenFilesAsync(Strings.EditorCommands_PlaceImageIntoFrame, false);
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
