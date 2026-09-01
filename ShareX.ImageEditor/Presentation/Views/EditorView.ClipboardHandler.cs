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
using ShareX.ImageEditor.Presentation.ViewModels;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class EditorView : UserControl
    {
        private async void OnCutRequested(object? sender, EventArgs e)
        {
            if (_selectionController.SelectedShape?.Tag is Annotation annotation)
            {
                // Copy to internal clipboard
                _clipboardAnnotation = annotation.Clone();

                // Update clipboard status
                _ = CheckClipboardStatus();

                // Clear system clipboard to avoid ambiguity when pasting back

                // Clear system clipboard to avoid ambiguity when pasting back
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
                    EditorServices.ReportWarning(nameof(EditorView), "Failed to clear system clipboard during cut operation.", ex);
                }

                // Delete original using ViewModel command to ensure undo history is recorded
                if (DataContext is MainViewModel vm)
                {
                    vm.DeleteSelectedCommand.Execute(null);
                }
            }
        }

        private async void OnCopyRequested(object? sender, EventArgs e)
        {
            if (_selectionController.SelectedShape?.Tag is Annotation annotation)
            {
                // Deep clone to internal clipboard
                _clipboardAnnotation = annotation.Clone();

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
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;
            var clipboard = topLevel.Clipboard;

            try
            {
                // Priority 1: Check system clipboard for images/files (external content)
                // This allows users to copy from browser/explorer and paste even if they previously copied a shape
                if (clipboard != null)
                {
                    // Check for files
                    var files = await clipboard.TryGetFilesAsync();
                    if (files != null && files.Any())
                    {
                        await PasteImageFromClipboard();
                        return;
                    }

                    // Check for bitmap
                    // Use TryGetBitmapAsync for reliable cross-format bitmap detection instead of
                    // hardcoded format name strings which are platform-specific and may not match.
                    var clipboardBitmap = await clipboard.TryGetBitmapAsync();
                    if (clipboardBitmap != null)
                    {
                        (clipboardBitmap as IDisposable)?.Dispose();
                        await PasteImageFromClipboard();
                        return;
                    }
                }

                // Priority 2: Internal shape clipboard
                if (_clipboardAnnotation != null)
                {
                    PasteInternalShape();
                    return;
                }
            }
            catch (Exception ex)
            {
                EditorServices.ReportWarning(nameof(EditorView), "Failed to handle paste request.", ex);
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
            var topLevel = TopLevel.GetTopLevel(this);
            var clipboard = topLevel?.Clipboard;
            if (clipboard == null) return;

            try
            {
                // Check for image file paths in clipboard (e.g. copied from Explorer)
                var files = await clipboard.TryGetFilesAsync();
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file is IStorageFile storageFile)
                        {
                            var ext = System.IO.Path.GetExtension(storageFile.Name)?.ToLowerInvariant();
                            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif" || ext == ".webp" || ext == ".ico" || ext == ".tiff" || ext == ".tif")
                            {
                                try
                                {
                                    using var stream = await storageFile.OpenReadAsync();
                                    using var memStream = new System.IO.MemoryStream();
                                    await stream.CopyToAsync(memStream);
                                    memStream.Position = 0;
                                    var skBitmap = SKBitmap.Decode(memStream);
                                    if (skBitmap != null)
                                    {
                                        await InsertExternalImageAsync(skBitmap, storageFile.Path.LocalPath);
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    EditorServices.ReportWarning(nameof(EditorView), $"Failed to decode clipboard image file '{storageFile.Name}'.", ex);
                                }
                            }
                        }
                    }
                }

                // Try clipboard bitmap data (e.g. PrintScreen, copy from image app)
                var clipboardBitmap = await clipboard.TryGetBitmapAsync();
                if (clipboardBitmap != null)
                {
                    var skBitmap = BitmapConversionHelpers.ToSKBitmap(clipboardBitmap);
                    if (skBitmap != null)
                    {
                        await InsertExternalImageAsync(skBitmap);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                EditorServices.ReportWarning(nameof(EditorView), "Failed to paste image content from clipboard.", ex);
            }
        }

        /// <summary>
        /// Checks if there is content on the system clipboard or internal clipboard
        /// and updates the ViewModel's CanPaste property.
        /// </summary>
        private async Task CheckClipboardStatus()
        {
            if (DataContext is not MainViewModel vm) return;

            bool canPaste = false;

            // 1. Check internal clipboard
            if (_clipboardAnnotation != null)
            {
                canPaste = true;
            }
            // 2. Check system clipboard
            else
            {
                var topLevel = TopLevel.GetTopLevel(this);
                var clipboard = topLevel?.Clipboard;
                if (clipboard != null)
                {
                    try
                    {
                        // Check for files
                        var files = await clipboard.TryGetFilesAsync();
                        if (files != null && files.Any())
                        {
                            canPaste = true;
                        }
                        else
                        {
                            // Use TryGetBitmapAsync for reliable cross-format bitmap detection.
                            // Format name strings (e.g. "PNG", "Bitmap") are platform-specific and
                            // unreliable; TryGetBitmapAsync handles all native image clipboard formats.
                            var bitmap = await clipboard.TryGetBitmapAsync();
                            if (bitmap != null)
                            {
                                (bitmap as IDisposable)?.Dispose();
                                canPaste = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EditorServices.ReportWarning(nameof(EditorView), "Failed to query system clipboard formats.", ex);
                    }
                }
            }

            vm.CanPaste = canPaste;
        }

        /// <summary>
        /// Duplicates the currently selected annotation with a deep copy.
        /// The duplicate is offset by 20px and becomes the new selection.
        /// </summary>
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