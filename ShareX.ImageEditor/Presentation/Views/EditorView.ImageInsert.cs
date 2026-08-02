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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.Rendering;
using ShareX.ImageEditor.Presentation.ViewModels;
using SkiaSharp;
using System.ComponentModel;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class EditorView : UserControl
    {
        private async void OnImageInsertionRequested(object? sender, EventArgs e)
        {
            if (DataContext is not MainViewModel)
            {
                return;
            }

            var pickedImage = await PickImageBitmapAsync(Strings.EditorView_SelectImage);
            if (!pickedImage.HasValue)
            {
                return;
            }

            await InsertExternalImageAsync(pickedImage.Value.Bitmap, pickedImage.Value.SourceFilePath);
        }

        internal async Task<bool> ReplaceImageAnnotationFromFilePickerAsync(ImageAnnotation annotation, Image imageControl)
        {
            try
            {
                var pickedImage = await PickImageBitmapAsync(Strings.EditorView_SelectImage);
                if (!pickedImage.HasValue)
                {
                    return false;
                }

                SKRect existingBounds = annotation.GetBounds();
                float centerX = existingBounds.MidX;
                float centerY = existingBounds.MidY;
                float newWidth = pickedImage.Value.Bitmap.Width;
                float newHeight = pickedImage.Value.Bitmap.Height;

                annotation.StartPoint = new SKPoint(centerX - newWidth / 2f, centerY - newHeight / 2f);
                annotation.EndPoint = new SKPoint(centerX + newWidth / 2f, centerY + newHeight / 2f);
                annotation.ImagePath = pickedImage.Value.SourceFilePath ?? string.Empty;
                annotation.SetImage(pickedImage.Value.Bitmap);
                AnnotationVisualFactory.UpdateVisualControl(imageControl, annotation);

                if (DataContext is MainViewModel vm)
                {
                    vm.HasAnnotations = true;
                    vm.IsDirty = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                EditorServices.ReportWarning(nameof(EditorView), "Failed to replace image annotation.", ex);
                return false;
            }
        }

        private async Task<(SKBitmap Bitmap, string? SourceFilePath)?> PickImageBitmapAsync(string dialogTitle)
        {
            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.StorageProvider == null)
            {
                return null;
            }

            IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = dialogTitle,
                AllowMultiple = false,
                FileTypeFilter = [FilePickerFileTypes.ImageAll]
            });

            if (files.Count == 0)
            {
                return null;
            }

            using var stream = await files[0].OpenReadAsync();
            using var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream);
            memStream.Position = 0;

            SKBitmap? skBitmap = SKBitmap.Decode(memStream);
            return skBitmap == null ? null : (skBitmap, files[0].Path.LocalPath);
        }

        private async Task InsertExternalImageAsync(SKBitmap skBitmap, string? sourceFilePath = null)
        {
            if (DataContext is not MainViewModel vm)
            {
                InsertImageAnnotation(skBitmap);
                return;
            }

            if (!vm.HasPreviewImage || _editorCore.SourceImage == null)
            {
                LoadBitmapIntoEditor(vm, skBitmap, sourceFilePath);
                return;
            }

            InsertImagePlacement? placement = vm.Options.ShowInsertImageDialog
                ? await ShowInsertImageDialogAsync(vm, skBitmap)
                : InsertImagePlacement.Center;

            if (!placement.HasValue)
            {
                skBitmap.Dispose();
                return;
            }

            await InsertImageWithPlacementAsync(skBitmap, placement.Value);
        }

        private Task<InsertImagePlacement?> ShowInsertImageDialogAsync(MainViewModel vm, SKBitmap skBitmap)
        {
            if (vm.IsModalOpen)
            {
                return Task.FromResult<InsertImagePlacement?>(null);
            }

            var completionSource = new TaskCompletionSource<InsertImagePlacement?>(TaskCreationOptions.RunContinuationsAsynchronously);
            PropertyChangedEventHandler? propertyChangedHandler = null;

            void Complete(InsertImagePlacement? result)
            {
                if (!completionSource.TrySetResult(result))
                {
                    return;
                }

                if (propertyChangedHandler != null)
                {
                    vm.PropertyChanged -= propertyChangedHandler;
                }
            }

            propertyChangedHandler = (_, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.IsModalOpen) && !vm.IsModalOpen)
                {
                    Complete(null);
                }
            };

            vm.PropertyChanged += propertyChangedHandler;

            var dialog = new InsertImageDialogViewModel(
                skBitmap.Width,
                skBitmap.Height,
                onSelect: placement =>
                {
                    Complete(placement);
                    vm.CloseModalCommand.Execute(null);
                },
                onCancel: () =>
                {
                    Complete(null);
                    vm.CloseModalCommand.Execute(null);
                });

            vm.ModalContent = dialog;
            vm.IsModalOpen = true;

            return completionSource.Task;
        }

        private async Task InsertImageWithPlacementAsync(SKBitmap skBitmap, InsertImagePlacement placement)
        {
            int canvasWidth = (int)Math.Round(_editorCore.CanvasSize.Width);
            int canvasHeight = (int)Math.Round(_editorCore.CanvasSize.Height);
            Point? position = null;
            bool waitForResizeSync = false;

            switch (placement)
            {
                case InsertImagePlacement.Center:
                    position = null;
                    break;
                case InsertImagePlacement.CanvasExpandDown:
                    int rightPadding = Math.Max(0, skBitmap.Width - canvasWidth);
                    _editorCore.ResizeCanvas(top: 0, right: rightPadding, bottom: skBitmap.Height, left: 0, backgroundColor: SKColors.Transparent);
                    position = new Point(0, canvasHeight);
                    waitForResizeSync = true;
                    break;
                case InsertImagePlacement.CanvasExpandRight:
                    int bottomPadding = Math.Max(0, skBitmap.Height - canvasHeight);
                    _editorCore.ResizeCanvas(top: 0, right: skBitmap.Width, bottom: bottomPadding, left: 0, backgroundColor: SKColors.Transparent);
                    position = new Point(canvasWidth, 0);
                    waitForResizeSync = true;
                    break;
            }

            if (waitForResizeSync)
            {
                await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
            }

            InsertImageAnnotationCore(skBitmap, position);
        }

        private void InsertImageAnnotationCore(SKBitmap skBitmap, Point? position = null)
        {
            var canvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (canvas == null || DataContext is not MainViewModel vm)
            {
                return;
            }

            double posX;
            double posY;

            if (position.HasValue)
            {
                posX = position.Value.X;
                posY = position.Value.Y;
            }
            else
            {
                Point visibleCanvasCenter = GetVisibleCanvasCenter(canvas) ?? new Point(
                    _editorCore.CanvasSize.Width / 2,
                    _editorCore.CanvasSize.Height / 2);

                posX = visibleCanvasCenter.X - skBitmap.Width / 2.0;
                posY = visibleCanvasCenter.Y - skBitmap.Height / 2.0;
            }

            var annotation = new ImageAnnotation();
            annotation.SetImage(skBitmap);
            annotation.StartPoint = new SKPoint((float)posX, (float)posY);
            annotation.EndPoint = new SKPoint((float)(posX + skBitmap.Width), (float)(posY + skBitmap.Height));

            Control? control = CreateControlForAnnotation(annotation);
            if (control == null)
            {
                return;
            }

            canvas.Children.Add(control);
            _editorCore.AddAnnotation(annotation);
            vm.HasAnnotations = true;
            vm.ActiveTool = EditorTool.Select;
            _selectionController.SetSelectedShape(control);
            vm.ShowImageInsertedNotification();
        }

        private Point? GetVisibleCanvasCenter(Canvas canvas)
        {
            var canvasScrollViewer = this.FindControl<ScrollViewer>("CanvasScrollViewer");
            if (canvasScrollViewer == null)
            {
                return null;
            }

            Size viewport = canvasScrollViewer.Viewport;
            if (viewport.Width <= 0 || viewport.Height <= 0)
            {
                return null;
            }

            Point viewportCenter = new(viewport.Width / 2, viewport.Height / 2);
            Point? visibleCanvasCenter = canvasScrollViewer.TranslatePoint(viewportCenter, canvas);

            if (!visibleCanvasCenter.HasValue)
            {
                return null;
            }

            return new Point(
                Math.Clamp(visibleCanvasCenter.Value.X, 0, _editorCore.CanvasSize.Width),
                Math.Clamp(visibleCanvasCenter.Value.Y, 0, _editorCore.CanvasSize.Height));
        }
    }
}
