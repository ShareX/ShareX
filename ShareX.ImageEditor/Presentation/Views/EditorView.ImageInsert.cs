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
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.Controllers;
using ShareX.ImageEditor.Presentation.Rendering;
using ShareX.ImageEditor.Presentation.ViewModels;
using ShareX.Platform;
using SkiaSharp;
using System.ComponentModel;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class EditorView : UserControl
    {
        private async void OnImageInsertionRequested(object? sender, EventArgs e)
        {
            if (DataContext is not MainViewModel vm || !IsImageRequestCurrent(sender, vm)) return;
            using var operation = BeginImageOperation(vm);
            try
            {
                await operation.RunOwnedAsync(_ => PickImageBitmapAsync(Strings.EditorView_SelectImage, operation),
                    image => InsertExternalImageAsync(image, operation));
            }
            catch (Exception ex)
            {
                if (IsEditorContextCurrent(vm)) EditorServices.ReportWarning(nameof(EditorView), "Failed to insert image.", ex);
            }
        }

        internal async Task<bool> ReplaceImageAnnotationFromFilePickerAsync(ImageAnnotation annotation, Image imageControl)
        {
            if (DataContext is not MainViewModel vm) return false;
            using var operation = _imageOperations.Begin(() => IsEditorContextCurrent(vm) &&
                !annotation.IsDisposed && _editorCore.Annotations.Contains(annotation));
            try
            {
                return await operation.RunOwnedAsync(_ => PickImageBitmapAsync(Strings.EditorView_SelectImage, operation), image =>
                {
                    SKRect existingBounds = annotation.GetBounds();
                    float newWidth = image.Bitmap.Width;
                    float newHeight = image.Bitmap.Height;
                    annotation.StartPoint = new SKPoint(existingBounds.MidX - newWidth / 2f, existingBounds.MidY - newHeight / 2f);
                    annotation.EndPoint = new SKPoint(existingBounds.MidX + newWidth / 2f, existingBounds.MidY + newHeight / 2f);
                    annotation.ImagePath = image.SourceFilePath ?? string.Empty;
                    annotation.SetImage(image.TakeBitmap());
                    AnnotationVisualFactory.UpdateVisualControl(imageControl, annotation);
                    vm.HasAnnotations = true;
                    vm.IsDirty = true;
                    return Task.FromResult(true);
                });
            }
            catch (Exception ex)
            {
                if (IsEditorContextCurrent(vm)) EditorServices.ReportWarning(nameof(EditorView), "Failed to replace image annotation.", ex);
                return false;
            }
        }

        private bool IsEditorContextCurrent(MainViewModel vm) => !_workspaceDisposed && !vm.IsDisposed && ReferenceEquals(DataContext, vm);

        private bool IsImageRequestCurrent(object? sender, MainViewModel vm) => IsEditorContextCurrent(vm) &&
            (sender is not MainViewModel requestOwner || ReferenceEquals(requestOwner, vm));

        private EditorImageOperationLifetime.Operation BeginImageOperation(MainViewModel vm) =>
            _imageOperations.Begin(() => IsEditorContextCurrent(vm));

        private void InvalidateImageOperations()
        {
            _imageOperations.Invalidate();
            _cancelPendingImageInsertion?.Invoke();
        }

        private async Task<EditorImportedImage?> PickImageBitmapAsync(string dialogTitle, EditorImageOperationLifetime.Operation operation)
        {
            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (!operation.IsCurrent || topLevel?.StorageProvider == null)
            {
                return null;
            }

            IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = dialogTitle,
                AllowMultiple = false,
                FileTypeFilter = [FilePickerFileTypes.ImageAll]
            });

            if (!operation.IsCurrent || files.Count == 0)
            {
                return null;
            }

            return await ReadImageFileAsync(files[0], operation);
        }

        private static Task<EditorImportedImage?> ReadImageFileAsync(IStorageFile file, EditorImageOperationLifetime.Operation operation) =>
            EditorImageFileReader.ReadAsync(file.OpenReadAsync, operation, file.TryGetLocalPath());

        private Action? _cancelPendingImageInsertion;

        private async Task<bool> InsertExternalImageAsync(EditorImportedImage image, EditorImageOperationLifetime.Operation operation)
        {
            if (!operation.IsCurrent || DataContext is not MainViewModel vm) return false;

            if (!vm.HasPreviewImage || _editorCore.SourceImage == null)
            {
                LoadBitmapIntoEditor(vm, image.TakeBitmap(), image.SourceFilePath);
                return true;
            }

            InsertImagePlacement? placement = vm.Options.ShowInsertImageDialog && !_isWorkspaceHostMode
                ? await ShowInsertImageDialogAsync(vm, image.Bitmap, operation)
                : InsertImagePlacement.Center;

            if (!operation.IsCurrent || !placement.HasValue) return false;

            return await InsertImageWithPlacementAsync(image, placement.Value, operation);
        }

        private Task<InsertImagePlacement?> ShowInsertImageDialogAsync(MainViewModel vm, SKBitmap skBitmap,
            EditorImageOperationLifetime.Operation operation)
        {
            if (!operation.IsCurrent || vm.IsModalOpen)
            {
                return Task.FromResult<InsertImagePlacement?>(null);
            }

            var completionSource = new TaskCompletionSource<InsertImagePlacement?>(TaskCreationOptions.RunContinuationsAsynchronously);
            PropertyChangedEventHandler? propertyChangedHandler = null;

            bool Complete(InsertImagePlacement? result)
            {
                if (!completionSource.TrySetResult(result))
                {
                    return false;
                }

                _cancelPendingImageInsertion = null;

                if (propertyChangedHandler != null)
                {
                    vm.PropertyChanged -= propertyChangedHandler;
                }
                return true;
            }

            propertyChangedHandler = (_, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.IsModalOpen) && !vm.IsModalOpen)
                {
                    ResetModalContentPosition();
                    Complete(null);
                }
            };

            vm.PropertyChanged += propertyChangedHandler;

            var dialog = new InsertImageDialogViewModel(
                skBitmap.Width,
                skBitmap.Height,
                onSelect: placement =>
                {
                    if (!operation.IsCurrent) { Complete(null); return; }
                    if (!Complete(placement)) return;
                    vm.CloseModalCommand.Execute(null);
                    ResetModalContentPosition();
                },
                onCancel: () =>
                {
                    if (!operation.IsCurrent) { Complete(null); return; }
                    if (!Complete(null)) return;
                    vm.CloseModalCommand.Execute(null);
                    ResetModalContentPosition();
                });

            _cancelPendingImageInsertion = () =>
            {
                Complete(null);
                if (ReferenceEquals(vm.ModalContent, dialog))
                {
                    vm.CloseModalCommand.Execute(null);
                    ResetModalContentPosition();
                }
            };

            vm.ModalContent = dialog;
            PositionModalOnCursorScreen();
            vm.IsModalOpen = true;

            return completionSource.Task;
        }

        private async Task<bool> InsertImageWithPlacementAsync(EditorImportedImage image, InsertImagePlacement placement,
            EditorImageOperationLifetime.Operation operation)
        {
            if (!operation.IsCurrent) return false;
            SKBitmap skBitmap = image.Bitmap;
            int canvasWidth = (int)Math.Round(_editorCore.CanvasSize.Width);
            int canvasHeight = (int)Math.Round(_editorCore.CanvasSize.Height);
            Point? position = null;
            bool waitForResizeSync = false;

            switch (placement)
            {
                case InsertImagePlacement.Center:
                    Canvas? canvas = this.FindControl<Canvas>("AnnotationCanvas");
                    Point? screenCenter = canvas == null ? null : GetCursorScreenCenter(canvas);
                    position = screenCenter.HasValue
                        ? new Point(
                            Math.Clamp(screenCenter.Value.X, 0, _editorCore.CanvasSize.Width) - skBitmap.Width / 2.0,
                            Math.Clamp(screenCenter.Value.Y, 0, _editorCore.CanvasSize.Height) - skBitmap.Height / 2.0)
                        : null;
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

            return await operation.PublishAfterAsync(image,
                async () => { if (waitForResizeSync) await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background); },
                ownedImage => InsertImageAnnotationCore(ownedImage.TakeBitmap(), position));
        }

        private bool InsertImageAnnotationCore(
            SKBitmap skBitmap,
            Point? position = null,
            bool showNotification = true,
            bool selectAnnotation = true)
        {
            var canvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (_workspaceDisposed || canvas == null || DataContext is not MainViewModel vm || vm.IsDisposed)
            {
                skBitmap.Dispose();
                return false;
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

            Control? control = null;
            try
            {
                control = CreateControlForAnnotation(annotation);
                if (control == null) return false;
                canvas.Children.Add(control);
                _editorCore.AddAnnotation(annotation);
                vm.HasAnnotations = true;
                if (selectAnnotation)
                {
                    vm.ActiveTool = EditorTool.Select;
                    _selectionController.SetSelectedShape(control);
                }

                if (showNotification) vm.ShowImageInsertedNotification();
                return true;
            }
            finally
            {
                // AddAnnotation may notify listeners after ownership has already moved to the core.
                if (!_editorCore.Annotations.Contains(annotation))
                {
                    if (control != null)
                    {
                        canvas.Children.Remove(control);
                        if (control is Image imageControl)
                        {
                            (imageControl.Source as IDisposable)?.Dispose();
                            imageControl.Source = null;
                        }
                    }
                    annotation.Dispose();
                }
            }
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

        private void PositionModalOnCursorScreen()
        {
            ContentControl? modalHost = this.FindControl<ContentControl>("ModalContentHost");
            if (modalHost == null)
            {
                return;
            }

            modalHost.RenderTransform = null;
            Point? targetCenter = GetCursorScreenCenter(this);
            if (!targetCenter.HasValue)
            {
                return;
            }

            modalHost.RenderTransform = new TranslateTransform(
                targetCenter.Value.X - Bounds.Width / 2,
                targetCenter.Value.Y - Bounds.Height / 2);
        }

        private void ResetModalContentPosition()
        {
            ContentControl? modalHost = this.FindControl<ContentControl>("ModalContentHost");
            if (modalHost != null)
            {
                modalHost.RenderTransform = null;
            }
        }

        private Point? GetCursorScreenCenter(Visual relativeTo)
        {
            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null)
            {
                return null;
            }

            Screens? screens = topLevel.Screens;
            PlatformPoint? cursorPosition = PlatformServices.IsInitialized
                ? PlatformServices.Current.Windows.GetCursorPosition()
                : null;
            Screen? screen = cursorPosition.HasValue
                ? screens?.ScreenFromPoint(new PixelPoint(cursorPosition.Value.X, cursorPosition.Value.Y))
                : null;
            screen ??= screens?.ScreenFromTopLevel(topLevel) ?? screens?.Primary;
            if (screen == null)
            {
                return null;
            }

            PixelPoint screenCenter = new(
                screen.Bounds.X + screen.Bounds.Width / 2,
                screen.Bounds.Y + screen.Bounds.Height / 2);
            return topLevel.TranslatePoint(topLevel.PointToClient(screenCenter), relativeTo);
        }
    }
}