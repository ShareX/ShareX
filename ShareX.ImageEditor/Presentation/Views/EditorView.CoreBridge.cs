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
using Avalonia.Media.Imaging;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Presentation.Controls;
using ShareX.ImageEditor.Presentation.Rendering;
using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views
{
    public partial class EditorView : UserControl
    {
        private SkiaSharp.SKBitmap? _cachedEffectPreviewSource;
        private SkiaSharp.SKBitmap? _cachedEffectPreviewBitmap;
        private string? _cachedEffectPreviewKey;

        private void UpdateViewModelHistoryState(MainViewModel vm)
        {
            vm.UpdateCoreHistoryState(_editorCore.CanUndo, _editorCore.CanRedo);
        }

        private void UpdateViewModelMetadata(MainViewModel vm)
        {
            // Initial sync of metadata if needed
            UpdateViewModelHistoryState(vm);
        }

        internal void RefreshSpotlightOverlay()
        {
            var spotlightOverlay = this.FindControl<SpotlightOverlayControl>("SpotlightOverlayControl");
            var annotationCanvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (spotlightOverlay == null || annotationCanvas == null)
            {
                return;
            }

            var spotlights = annotationCanvas.Children
                .OfType<SpotlightControl>()
                .Select(control => control.Annotation)
                .Where(annotation => annotation != null)
                .Cast<SpotlightAnnotation>()
                .ToList();

            int canvasWidth = Math.Max(1, (int)Math.Ceiling(_editorCore.CanvasSize.Width));
            int canvasHeight = Math.Max(1, (int)Math.Ceiling(_editorCore.CanvasSize.Height));

            spotlightOverlay.UpdateSpotlights(spotlights, _editorCore.SourceImage, canvasWidth, canvasHeight);
        }

        private void RenderCore()
        {
            if (_canvasControl == null) return;
            // Hybrid rendering: Render only background + raster effects from Core
            // Vector annotations are handled by Avalonia Canvas
            _canvasControl.Draw(canvas => _editorCore.Render(canvas));
        }

        private void RequestRenderCore()
        {
            if (Interlocked.Exchange(ref _renderCorePending, 1) != 0)
            {
                return;
            }

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Interlocked.Exchange(ref _renderCorePending, 0);
                RenderCore();
            }, Avalonia.Threading.DispatcherPriority.Render);
        }

        public SkiaSharp.SKBitmap? GetSource()
        {
            if (_editorCore.SourceImage != null)
            {
                return _editorCore.SourceImage.Copy();
            }

            return null;
        }

        public SkiaSharp.SKBitmap? GetSnapshot()
        {
            if (_editorCore.SourceImage == null) return null;

            var previewFrame = this.FindControl<Border>("PreviewFrame");
            var canvasContainer = this.FindControl<Grid>("CanvasContainer");
            var overlayCanvas = this.FindControl<Canvas>("OverlayCanvas");
            Control? snapshotTarget = previewFrame as Control ?? canvasContainer;
            if (snapshotTarget == null) return _editorCore.GetSnapshot();

            // Hide OverlayCanvas (selection handles, crop overlay) during capture
            bool overlayWasVisible = overlayCanvas?.IsVisible ?? false;
            if (overlayCanvas != null) overlayCanvas.IsVisible = false;

            try
            {
                snapshotTarget.Measure(Size.Infinity);

                int pixelWidth = 0;
                int pixelHeight = 0;

                if (DataContext is MainViewModel vm)
                {
                    double width = vm.SmartPaddingViewportWidth + vm.SmartPaddingThickness.Left + vm.SmartPaddingThickness.Right + vm.CanvasPadding.Left + vm.CanvasPadding.Right;
                    double height = vm.SmartPaddingViewportHeight + vm.SmartPaddingThickness.Top + vm.SmartPaddingThickness.Bottom + vm.CanvasPadding.Top + vm.CanvasPadding.Bottom;

                    pixelWidth = Math.Max(1, (int)Math.Round(width, MidpointRounding.AwayFromZero));
                    pixelHeight = Math.Max(1, (int)Math.Round(height, MidpointRounding.AwayFromZero));
                }

                if (pixelWidth <= 0 || pixelHeight <= 0)
                {
                    double width = snapshotTarget.Bounds.Width;
                    double height = snapshotTarget.Bounds.Height;

                    if (width <= 0 || height <= 0)
                    {
                        width = snapshotTarget.DesiredSize.Width;
                        height = snapshotTarget.DesiredSize.Height;
                    }

                    if (width <= 0 || height <= 0)
                    {
                        width = _editorCore.SourceImage.Width;
                        height = _editorCore.SourceImage.Height;
                    }

                    // Fallback to live bounds only when the view-model export size is unavailable.
                    pixelWidth = Math.Max(1, (int)Math.Round(width, MidpointRounding.AwayFromZero));
                    pixelHeight = Math.Max(1, (int)Math.Round(height, MidpointRounding.AwayFromZero));
                }

                // Force layout at native resolution (un-zoomed)
                snapshotTarget.Arrange(new Rect(0, 0, pixelWidth, pixelHeight));

                // Render Avalonia visual tree to bitmap
                using var rtb = new RenderTargetBitmap(
                        new PixelSize(pixelWidth, pixelHeight),
                    new Vector(96, 96));
                rtb.Render(snapshotTarget);

                // Convert Avalonia RenderTargetBitmap → SKBitmap via direct pixel copy (no PNG encode/decode)
                var skBitmap = BitmapConversionHelpers.ToSKBitmap(rtb);

                return skBitmap;
            }
            finally
            {
                // Restore OverlayCanvas visibility
                if (overlayCanvas != null) overlayCanvas.IsVisible = overlayWasVisible;

                // Re-trigger layout with current zoom
                snapshotTarget.InvalidateMeasure();
                snapshotTarget.InvalidateArrange();
            }
        }

        /// <summary>
        /// Renders an image-pixel rectangle from the shared workspace. Hosts such as region
        /// capture can avoid allocating a second full-size virtual-desktop bitmap.
        /// </summary>
        public SkiaSharp.SKBitmap? GetSnapshot(Rect sourceRectangle)
        {
            SkiaSharp.SKBitmap? sourceImage = _editorCore.SourceImage;
            if (sourceImage == null)
            {
                return null;
            }

            Rect imageBounds = new Rect(0, 0, sourceImage.Width, sourceImage.Height);
            Rect clipped = sourceRectangle.Intersect(imageBounds);
            if (clipped.Width <= 0 || clipped.Height <= 0)
            {
                return null;
            }

            int left = Math.Clamp((int)Math.Floor(clipped.Left), 0, sourceImage.Width - 1);
            int top = Math.Clamp((int)Math.Floor(clipped.Top), 0, sourceImage.Height - 1);
            int right = Math.Clamp((int)Math.Ceiling(clipped.Right), left + 1, sourceImage.Width);
            int bottom = Math.Clamp((int)Math.Ceiling(clipped.Bottom), top + 1, sourceImage.Height);
            int width = right - left;
            int height = bottom - top;

            if (width <= 0 || height <= 0)
            {
                return null;
            }

            if (_editorCore.Annotations.Count == 0)
            {
                SkiaSharp.SKBitmap output = new(new SkiaSharp.SKImageInfo(
                    width,
                    height,
                    sourceImage.ColorType,
                    sourceImage.AlphaType));
                using SkiaSharp.SKCanvas canvas = new(output);
                canvas.DrawBitmap(
                    sourceImage,
                    new SkiaSharp.SKRect(left, top, right, bottom),
                    new SkiaSharp.SKRect(0, 0, width, height));
                return output;
            }

            Border? previewFrame = this.FindControl<Border>("PreviewFrame");
            Canvas? overlayCanvas = this.FindControl<Canvas>("OverlayCanvas");
            if (previewFrame == null)
            {
                return null;
            }

            bool overlayWasVisible = overlayCanvas?.IsVisible ?? false;
            if (overlayCanvas != null)
            {
                overlayCanvas.IsVisible = false;
            }

            try
            {
                previewFrame.Measure(Size.Infinity);
                previewFrame.Arrange(new Rect(0, 0, sourceImage.Width, sourceImage.Height));

                using RenderTargetBitmap renderTarget = new(
                    new PixelSize(width, height),
                    new Vector(96, 96));
                VisualBrush brush = new(previewFrame)
                {
                    SourceRect = new RelativeRect(new Rect(left, top, width, height), RelativeUnit.Absolute),
                    DestinationRect = RelativeRect.Fill,
                    Stretch = Stretch.Fill,
                    TileMode = TileMode.None
                };

                using (DrawingContext context = renderTarget.CreateDrawingContext())
                {
                    context.DrawRectangle(brush, null, new Rect(0, 0, width, height));
                }

                return BitmapConversionHelpers.ToSKBitmap(renderTarget);
            }
            finally
            {
                if (overlayCanvas != null)
                {
                    overlayCanvas.IsVisible = overlayWasVisible;
                }

                previewFrame.InvalidateMeasure();
                previewFrame.InvalidateArrange();
            }
        }

        public Task<Bitmap?> RenderSnapshot()
        {
            var skBitmap = GetSnapshot();
            var snapshot = skBitmap != null ? BitmapConversionHelpers.ToAvaloniBitmap(skBitmap) : null;
            return Task.FromResult<Bitmap?>(snapshot);
        }

        private bool TryUpdateCachedEffectVisual(Control shape, BaseEffectAnnotation annotation, SkiaSharp.SKBitmap sourceBitmap, Rect? overrideBounds = null)
        {
            Rect? effectBounds = null;

            if (overrideBounds.HasValue)
            {
                var bounds = overrideBounds.Value;
                if (bounds.Width <= 0 || bounds.Height <= 0)
                {
                    return false;
                }

                annotation.StartPoint = new SkiaSharp.SKPoint((float)bounds.X, (float)bounds.Y);
                annotation.EndPoint = new SkiaSharp.SKPoint((float)(bounds.X + bounds.Width), (float)(bounds.Y + bounds.Height));
                effectBounds = bounds;
            }

            if (!EnsureEffectPreviewCache(annotation, sourceBitmap) || _cachedEffectPreviewBitmap == null)
            {
                return false;
            }

            annotation.UpdateEffectFromInteractionCache(sourceBitmap, _cachedEffectPreviewBitmap);
            AnnotationEffectVisualUpdater.ApplyEffectClip(shape, annotation, effectBounds ?? GetEffectVisualBounds(shape, annotation));
            AnnotationEffectVisualUpdater.ApplyEffectBrush(shape, annotation);
            return true;
        }

        private static Rect GetEffectVisualBounds(Control shape, BaseEffectAnnotation annotation)
        {
            double left = Canvas.GetLeft(shape);
            double top = Canvas.GetTop(shape);
            double width = shape.Width;
            double height = shape.Height;

            if (double.IsNaN(width) || width <= 0)
            {
                width = shape.Bounds.Width;
            }

            if (double.IsNaN(height) || height <= 0)
            {
                height = shape.Bounds.Height;
            }

            if (double.IsNaN(left) || double.IsNaN(top) || width <= 0 || height <= 0)
            {
                var annotationBounds = annotation.GetBounds();
                left = annotationBounds.Left;
                top = annotationBounds.Top;
                width = annotationBounds.Width;
                height = annotationBounds.Height;
            }

            return new Rect(left, top, width, height);
        }

        private bool EnsureEffectPreviewCache(BaseEffectAnnotation annotation, SkiaSharp.SKBitmap sourceBitmap)
        {
            string cacheKey = annotation.GetInteractionCacheKey();

            if (_cachedEffectPreviewBitmap != null
                && ReferenceEquals(_cachedEffectPreviewSource, sourceBitmap)
                && string.Equals(_cachedEffectPreviewKey, cacheKey, StringComparison.Ordinal))
            {
                return true;
            }

            ClearEffectPreviewCache();

            _cachedEffectPreviewBitmap = annotation.CreateInteractionCacheBitmap(sourceBitmap);
            if (_cachedEffectPreviewBitmap == null)
            {
                return false;
            }

            _cachedEffectPreviewSource = sourceBitmap;
            _cachedEffectPreviewKey = cacheKey;
            return true;
        }

        private void ClearEffectPreviewCache()
        {
            _cachedEffectPreviewBitmap?.Dispose();
            _cachedEffectPreviewBitmap = null;
            _cachedEffectPreviewSource = null;
            _cachedEffectPreviewKey = null;
        }

        internal void ClearInteractiveEffectPreviewCache()
        {
            ClearEffectPreviewCache();
        }

        internal void UpdateInteractiveEffectVisual(Control shape, SkiaSharp.SKBitmap sourceBitmap, Rect? overrideBounds = null)
        {
            if (shape?.Tag is not BaseEffectAnnotation effectAnnotation || sourceBitmap == null)
            {
                return;
            }

            if (TryUpdateCachedEffectVisual(shape, effectAnnotation, sourceBitmap, overrideBounds))
            {
                return;
            }

            AnnotationEffectVisualUpdater.UpdateEffectVisual(shape, sourceBitmap, overrideBounds);
        }

        // This is called by SelectionController/InputController via event when an effect logic needs update
        // We replicate the UpdateEffectVisual logic here or expose it
        private void OnRequestUpdateEffect(Control shape)
        {
            if (shape == null || shape.Tag is not BaseEffectAnnotation) return;
            if (DataContext is not MainViewModel vm) return;

            SkiaSharp.SKBitmap? temporarySource = null;

            try
            {
                // Reuse the core source bitmap during interactive effect updates.
                // Converting PreviewImage to a fresh SKBitmap on every drag frame is the
                // main difference from the creation path and causes the lag seen after
                // an effect annotation has been created.
                var sourceBitmap = _editorCore.SourceImage;

                if (sourceBitmap == null && vm.PreviewImage != null)
                {
                    temporarySource = BitmapConversionHelpers.ToSKBitmap(vm.PreviewImage);
                    sourceBitmap = temporarySource;
                }

                if (sourceBitmap == null)
                {
                    return;
                }

                UpdateInteractiveEffectVisual(shape, sourceBitmap);
            }
            catch (Exception ex)
            {
                EditorServices.ReportWarning(nameof(EditorView), "Failed to update effect annotation preview.", ex);
            }
            finally
            {
                temporarySource?.Dispose();
            }
        }

        private void OnAnnotationsRestored()
        {
            // Fully rebuild annotation layer from Core state
            // 1. Clear current UI annotations
            var canvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (canvas == null) return;

            canvas.Children.Clear();
            _selectionController.ClearSelection();

            // 2. Re-create UI for all vector annotations in Core
            foreach (var annotation in _editorCore.Annotations)
            {
                // Only create UI for vector annotations (Hybrid model)
                Control? shape = CreateControlForAnnotation(annotation);
                if (shape != null)
                {
                    canvas.Children.Add(shape);

                }
            }

            RefreshSpotlightOverlay();

            RenderCore();

            // 3. Validate state synchronization (ISSUE-001 mitigation)
            ValidateAnnotationSync();

            // Update HasAnnotations state
            UpdateHasAnnotationsState();
        }

        private void OnAnnotationOrderChanged()
        {
            var canvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (canvas == null) return;

            var children = canvas.Children.OfType<Control>().ToList();
            if (children.Count == 0) return;

            var coreAnnotations = _editorCore.Annotations;
            // Create a lookup for O(1) index access
            var indexLookup = new Dictionary<Annotation, int>();
            for (int i = 0; i < coreAnnotations.Count; i++)
            {
                indexLookup[coreAnnotations[i]] = i;
            }

            children.Sort((a, b) =>
            {
                int indexA = int.MaxValue;
                if (a.Tag is Annotation tagA && indexLookup.TryGetValue(tagA, out var ia))
                {
                    indexA = ia;
                }

                int indexB = int.MaxValue;
                if (b.Tag is Annotation tagB && indexLookup.TryGetValue(tagB, out var ib))
                {
                    indexB = ib;
                }

                return indexA.CompareTo(indexB);
            });

            canvas.Children.Clear();
            canvas.Children.AddRange(children);
        }

        /// <summary>
        /// Updates the ViewModel's HasAnnotations property based on current annotation count.
        /// </summary>
        private void UpdateHasAnnotationsState()
        {
            if (DataContext is MainViewModel vm)
            {
                var canvas = this.FindControl<Canvas>("AnnotationCanvas");
                int coreAnnotationCount = _editorCore.Annotations.Count;
                int canvasChildCount = canvas?.Children.Count ?? 0;
                vm.HasAnnotations = coreAnnotationCount > 0 || canvasChildCount > 0;
            }
        }

        /// <summary>
        /// Sample pixel color from the rendered canvas (including annotations) at the specified canvas coordinates
        /// </summary>
        internal async System.Threading.Tasks.Task<string?> GetPixelColorFromRenderedCanvas(Point canvasPoint)
        {
            if (DataContext is not MainViewModel vm || !vm.HasPreviewImage) return null;

            try
            {
                var container = this.FindControl<Grid>("CanvasContainer");
                if (container == null || container.Width <= 0 || container.Height <= 0) return null;

                var rtb = new global::Avalonia.Media.Imaging.RenderTargetBitmap(
                    new PixelSize((int)container.Width, (int)container.Height),
                    new Vector(96, 96));

                rtb.Render(container);

                using var skBitmap = BitmapConversionHelpers.ToSKBitmap(rtb);

                int x = (int)Math.Round(canvasPoint.X);
                int y = (int)Math.Round(canvasPoint.Y);

                if (x < 0 || y < 0 || x >= skBitmap.Width || y >= skBitmap.Height)
                    return null;

                var skColor = skBitmap.GetPixel(x, y);
                return $"#{skColor.Red:X2}{skColor.Green:X2}{skColor.Blue:X2}";
            }
            catch (Exception ex)
            {
                EditorServices.ReportWarning(nameof(EditorView), "Failed to sample pixel color from rendered canvas.", ex);
                return null;
            }
        }
    }
}
