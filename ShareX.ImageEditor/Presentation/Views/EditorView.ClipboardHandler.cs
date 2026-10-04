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
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Presentation.Controllers;
using ShareX.ImageEditor.Presentation.ViewModels;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class EditorView : UserControl
    {
        /// <summary>Clears the shared annotation clipboard and releases its owned bitmap.</summary>
        public static void ClearAnnotationClipboard() => ReplaceClipboardAnnotation(null);

        private static void ReplaceClipboardAnnotation(Annotation? annotation)
        {
            if (ReferenceEquals(_clipboardAnnotation, annotation)) return;
            var previous = _clipboardAnnotation;
            _clipboardAnnotation = annotation;
            (previous as IDisposable)?.Dispose();
        }

        private async void OnCutRequested(object? sender, EventArgs e)
        {
            if (DataContext is not MainViewModel vm || !IsImageRequestCurrent(sender, vm) ||
                _selectionController.SelectedShape is not { Tag: Annotation annotation } selected ||
                this.FindControl<Canvas>("AnnotationCanvas") is not { } canvas || !canvas.Children.Contains(selected)) return;
            using var operation = BeginImageOperation(vm);
            try
            {
                await EditorAnnotationCutController.CutAsync(_editorCore, annotation, operation, copy =>
                {
                    ReplaceClipboardAnnotation(copy);
                    _ = CheckClipboardStatus();
                }, _ =>
                {
                    if (selected is Image { Tag: ImageAnnotation } image)
                    {
                        (image.Source as IDisposable)?.Dispose();
                        image.Source = null;
                    }
                    canvas.Children.Remove(selected);
                    if (ReferenceEquals(_selectionController.SelectedShape, selected)) _selectionController.ClearSelection();
                    RefreshSpotlightOverlay();
                    UpdateHasAnnotationsState();
                }, () => TopLevel.GetTopLevel(this)?.Clipboard?.ClearAsync() ?? Task.CompletedTask,
                    ex => EditorServices.ReportWarning(nameof(EditorView), "Failed to clear system clipboard during cut operation.", ex));
            }
            catch (Exception ex)
            {
                if (operation.IsCurrent) EditorServices.ReportWarning(nameof(EditorView), "Failed to cut annotation.", ex);
            }
        }

        private async void OnCopyRequested(object? sender, EventArgs e)
        {
            if (_selectionController.SelectedShape?.Tag is Annotation annotation)
            {
                // Deep clone to internal clipboard
                ReplaceClipboardAnnotation(annotation.Clone());

                // Update clipboard status
                _ = CheckClipboardStatus();

                // Clear system clipboard to avoid ambiguity when pasting back

                // Clear system clipboard to avoid ambiguity when pasting back
                // This ensures that if the user pastes, we know to use the internal clipboard
                // unless they subsequently copy something externally
                try
                {
                    var topLevel = TopLevel.GetTopLevel(this);
                    if (topLevel?.Clipboard != null)
                    {
                        await topLevel.Clipboard.ClearAsync();
                    }
                }
                catch (Exception ex)
                {
                    EditorServices.ReportWarning(nameof(EditorView), "Failed to clear system clipboard during copy operation.", ex);
                }
            }
        }

        private async void OnPasteRequested(object? sender, EventArgs e)
        {
            if (DataContext is not MainViewModel vm || !IsImageRequestCurrent(sender, vm)) return;
            try
            {
                await PasteImageFromClipboard();
            }
            catch (Exception ex)
            {
                if (!_workspaceDisposed) EditorServices.ReportWarning(nameof(EditorView), "Failed to handle paste request.", ex);
            }
        }

        private void PasteInternalShape()
        {
            if (_clipboardAnnotation == null) return;

            // Clone again from clipboard so we can paste multiple times
            var newAnnotation = _clipboardAnnotation.Clone();

            // Offset position so it's visible (10px offset)
            const float offset = 20f;

            // Adjust points based on type
            if (newAnnotation is ImageAnnotation img)
            {
                // Check if the image bitmap is valid (disposed?)
                if (img.ImageBitmap == null && _clipboardAnnotation is ImageAnnotation clipImg)
                {
                    // Resurrect bitmap if needed (unlikely if deep cloned correctly)
                    // But Clone() manages it.
                }
            }

            // General offset logic
            newAnnotation.StartPoint = new SKPoint(newAnnotation.StartPoint.X + offset, newAnnotation.StartPoint.Y + offset);
            newAnnotation.EndPoint = new SKPoint(newAnnotation.EndPoint.X + offset, newAnnotation.EndPoint.Y + offset);

            if (newAnnotation is FreehandAnnotation freehand)
            {
                for (int i = 0; i < freehand.Points.Count; i++)
                {
                    freehand.Points[i] = new SKPoint(freehand.Points[i].X + offset, freehand.Points[i].Y + offset);
                }
            }

            // Add to Core
            _editorCore.AddAnnotation(newAnnotation);

            // Create UI
            var control = CreateControlForAnnotation(newAnnotation);
            if (control != null)
            {
                var canvas = this.FindControl<Canvas>("AnnotationCanvas");
                if (canvas != null)
                {
                    canvas.Children.Add(control);

                    // Update selection to the pasted object
                    _selectionController.SetSelectedShape(control);
                }
            }

            // Update VM state
            if (DataContext is MainViewModel vm)
            {
                vm.HasAnnotations = true;
            }
        }

        /// <summary>
        /// Handles Ctrl+V paste of images from clipboard (both bitmap data and file references).
        /// </summary>
        private async Task PasteImageFromClipboard()
        {
            if (DataContext is not MainViewModel vm || TopLevel.GetTopLevel(this) is not { } topLevel) return;
            using var operation = BeginImageOperation(vm);
            bool externalContent = false;
            if (topLevel.Clipboard is { } clipboard)
            {
                try
                {
                    await operation.RunOwnedAsync(_ => ReadClipboardImageAsync(clipboard, operation,
                        markExternalContent: () => externalContent = true), image => InsertExternalImageAsync(image, operation));
                }
                catch (Exception ex)
                {
                    if (IsEditorContextCurrent(vm)) EditorServices.ReportWarning(nameof(EditorView), "Failed to paste image content from clipboard.", ex);
                    return;
                }
            }
            if (operation.IsCurrent && !externalContent && _clipboardAnnotation != null) PasteInternalShape();
        }

        private async Task<EditorImportedImage?> ReadClipboardImageAsync(IClipboard clipboard,
            EditorImageOperationLifetime.Operation operation, bool filesFirst = true, bool filterFilesByExtension = true,
            Action? markExternalContent = null)
        {
            async Task<EditorImportedImage?> ReadFilesAsync()
            {
                if (!operation.IsCurrent) return null;
                var files = (await clipboard.TryGetFilesAsync())?.ToList();
                if (!operation.IsCurrent || files == null) return null;
                if (files.Count > 0) markExternalContent?.Invoke();
                foreach (var file in files.OfType<IStorageFile>())
                {
                    if (filterFilesByExtension && !IsImageFileName(file.Name)) continue;
                    try
                    {
                        EditorImportedImage? image = await ReadImageFileAsync(file, operation);
                        if (image != null) return image;
                    }
                    catch (Exception ex)
                    {
                        if (!operation.IsCurrent) return null;
                        EditorServices.ReportWarning(nameof(EditorView), $"Failed to decode clipboard image file '{file.Name}'.", ex);
                    }
                }
                return null;
            }

            async Task<EditorImportedImage?> ReadBitmapAsync()
            {
                if (!operation.IsCurrent) return null;
                var bitmap = await clipboard.TryGetBitmapAsync();
                try
                {
                    if (!operation.IsCurrent || bitmap == null) return null;
                    markExternalContent?.Invoke();
                    SKBitmap? converted = BitmapConversionHelpers.ToSKBitmap(bitmap);
                    return converted == null ? null : new EditorImportedImage(converted);
                }
                finally
                {
                    (bitmap as IDisposable)?.Dispose();
                }
            }

            return filesFirst
                ? await ReadFilesAsync() ?? await ReadBitmapAsync()
                : await ReadBitmapAsync() ?? await ReadFilesAsync();
        }

        private static bool IsImageFileName(string name) => Path.GetExtension(name).ToLowerInvariant() is
            ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif" or ".webp" or ".ico";

        private int _clipboardStatusVersion;

        /// <summary>Only the latest clipboard query for this editor owner may update CanPaste.</summary>
        private async Task CheckClipboardStatus()
        {
            if (DataContext is not MainViewModel vm) return;
            using var operation = BeginImageOperation(vm);
            int version = ++_clipboardStatusVersion;
            bool canPaste = _clipboardAnnotation != null;
            if (!canPaste && operation.IsCurrent && TopLevel.GetTopLevel(this)?.Clipboard is { } clipboard)
            {
                try
                {
                    var files = await clipboard.TryGetFilesAsync();
                    if (!operation.IsCurrent) return;
                    canPaste = files?.Any() == true;
                    if (!canPaste)
                    {
                        var bitmap = await clipboard.TryGetBitmapAsync();
                        try { canPaste = bitmap != null; }
                        finally { (bitmap as IDisposable)?.Dispose(); }
                    }
                }
                catch (Exception ex)
                {
                    if (operation.IsCurrent) EditorServices.ReportWarning(nameof(EditorView), "Failed to query system clipboard formats.", ex);
                }
            }
            if (operation.IsCurrent && version == _clipboardStatusVersion) vm.CanPaste = canPaste;
        }

        /// <summary>Duplicates the selected annotation with a deep copy and the existing 20px offset.</summary>
        private void DuplicateSelectedAnnotation()
        {
            var selectedControl = _selectionController.SelectedShape;
            if (selectedControl == null) return;

            var annotation = selectedControl.Tag as Annotation;
            if (annotation == null) return;

            var canvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (canvas == null) return;

            // Deep clone the annotation (ImageAnnotation.Clone deep-copies the bitmap)
            var clone = annotation.Clone();

            // Offset the duplicate by 20px
            const float offset = 20f;
            clone.StartPoint = new SkiaSharp.SKPoint(clone.StartPoint.X + offset, clone.StartPoint.Y + offset);
            clone.EndPoint = new SkiaSharp.SKPoint(clone.EndPoint.X + offset, clone.EndPoint.Y + offset);

            // Offset freehand points if applicable
            if (clone is FreehandAnnotation freehandClone)
            {
                for (int i = 0; i < freehandClone.Points.Count; i++)
                {
                    var pt = freehandClone.Points[i];
                    freehandClone.Points[i] = new SkiaSharp.SKPoint(pt.X + offset, pt.Y + offset);
                }
            }

            // Add to EditorCore (captures undo history before adding)
            _editorCore.AddAnnotation(clone);

            // Create the UI control for the cloned annotation
            var control = CreateControlForAnnotation(clone);
            if (control != null)
            {
                canvas.Children.Add(control);
                _selectionController.SetSelectedShape(control);
            }

            // Update clipboard status after internal copy
            _ = CheckClipboardStatus();

            // Update HasAnnotations state
            if (DataContext is MainViewModel vm)
            {
                vm.HasAnnotations = true;
            }
        }

        private void OnDuplicateRequested(object? sender, EventArgs e)
        {
            DuplicateSelectedAnnotation();
        }
    }
}