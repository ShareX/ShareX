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
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.Controls;
using ShareX.ImageEditor.Presentation.ViewModels;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Views
{
    /// <summary>
    /// Canvas size tool: a frame with handles around the image. Dragging a handle outward
    /// adds empty space (filled with the canvas fill color), dragging inward crops.
    /// </summary>
    public partial class EditorView
    {
        [Flags]
        private enum CanvasResizeEdges
        {
            None = 0,
            Left = 1,
            Top = 2,
            Right = 4,
            Bottom = 8
        }

        private const double CanvasResizeHandleSize = 12;
        private const double CanvasResizeHitTolerance = 10;
        private const int CanvasResizeMinSize = 1;
        private const int CanvasResizeMaxSize = 30000;
        private const int CanvasResizeZIndex = 9000;

        /// <summary>Free room shown around the image while the tool is active, as a share of the viewport.</summary>
        private const double CanvasResizeRoomFactor = 0.3;

        /// <summary>If the image is bigger than this share of the viewport, zoom out so the whole frame is visible.</summary>
        private const double CanvasResizeFitFactor = 0.7;

        /// <summary>Zoom before the tool zoomed out to show the whole image; restored when the tool closes.</summary>
        private double? _canvasResizeSavedZoom;

        private bool _canvasResizeActive;
        private bool _canvasResizeApplying;
        private bool _canvasResizeUpdatePosted;

        /// <summary>New canvas bounds in image coordinates. (0, 0, width, height) means no change.</summary>
        private Rect _canvasResizeRect;

        private CanvasResizeEdges _canvasResizeDragEdges;
        private Point _canvasResizeDragStart;
        private Rect _canvasResizeDragStartRect;

        private Grid? _canvasResizeHost;
        private readonly List<Control> _canvasResizeVisuals = new();
        private readonly List<Rectangle> _canvasResizeFillRects = new();
        private readonly List<Rectangle> _canvasResizeShadeRects = new();
        private readonly Dictionary<CanvasResizeEdges, Border> _canvasResizeHandles = new();
        private readonly Dictionary<CanvasResizeEdges, Border> _canvasResizeEdgeStrips = new();
        private Rectangle? _canvasResizeFrame;
        private Border? _canvasResizePanel;
        private TextBlock? _canvasResizeSizeText;

        private static readonly CanvasResizeEdges[] CanvasResizeHandleKinds =
        {
            CanvasResizeEdges.Left | CanvasResizeEdges.Top,
            CanvasResizeEdges.Top,
            CanvasResizeEdges.Right | CanvasResizeEdges.Top,
            CanvasResizeEdges.Right,
            CanvasResizeEdges.Right | CanvasResizeEdges.Bottom,
            CanvasResizeEdges.Bottom,
            CanvasResizeEdges.Left | CanvasResizeEdges.Bottom,
            CanvasResizeEdges.Left
        };

        internal bool IsCanvasResizeActive => _canvasResizeActive;

        private void OnCanvasSizeModeRequested(object? sender, EventArgs e)
        {
            if (_canvasResizeActive)
            {
                TryConfirmCanvasResize();
            }
            else
            {
                BeginCanvasResize();
            }
        }

        private void BeginCanvasResize()
        {
            if (_workspaceDisposed || _canvasResizeActive || _editorCore.SourceImage == null)
            {
                return;
            }

            // Finish whatever is in progress: typing text, a pending crop, a selection.
            this.Focus();
            _inputController.CancelClone();
            _inputController.CancelCrop();
            _selectionController.ClearSelection();

            // Text commit and its canvas fit run on the dispatcher, so start after them.
            Dispatcher.UIThread.Post(StartCanvasResize, DispatcherPriority.Background);
        }

        private void StartCanvasResize()
        {
            if (_workspaceDisposed || _canvasResizeActive || _editorCore.SourceImage == null)
            {
                return;
            }

            var host = this.FindControl<Grid>("EditorCanvasHost");
            var overlayHost = this.FindControl<Canvas>("OverlayHost");
            var scrollViewer = this.FindControl<ScrollViewer>("CanvasScrollViewer");
            if (host == null || overlayHost == null || scrollViewer == null)
            {
                return;
            }

            _canvasResizeActive = true;
            _canvasResizeHost = host;
            _canvasResizeRect = new Rect(0, 0, _editorCore.SourceImage.Width, _editorCore.SourceImage.Height);
            _canvasResizeDragEdges = CanvasResizeEdges.None;

            host.AddHandler(PointerPressedEvent, OnCanvasResizePointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
            host.AddHandler(PointerMovedEvent, OnCanvasResizePointerMoved, RoutingStrategies.Tunnel, handledEventsToo: true);
            host.AddHandler(PointerReleasedEvent, OnCanvasResizePointerReleased, RoutingStrategies.Tunnel, handledEventsToo: true);
            host.AddHandler(PointerCaptureLostEvent, OnCanvasResizePointerCaptureLost, RoutingStrategies.Bubble, handledEventsToo: true);
            scrollViewer.ScrollChanged += OnCanvasResizeScrollChanged;

            CreateCanvasResizeVisuals(overlayHost);
            _canvasResizeSavedZoom = null;

            if (DataContext is MainViewModel vm)
            {
                vm.IsCanvasSizeModeActive = true;

                // A big screenshot does not fit in the window: zoom out so all edges and handles are visible.
                double effectiveZoom = vm.EffectiveZoom > 0 ? vm.EffectiveZoom : 1.0;
                double viewportWidth = scrollViewer.Viewport.Width;
                double viewportHeight = scrollViewer.Viewport.Height;
                if (viewportWidth > 0 && viewportHeight > 0)
                {
                    double fitZoom = Math.Min(
                        viewportWidth * CanvasResizeFitFactor / _canvasResizeRect.Width,
                        viewportHeight * CanvasResizeFitFactor / _canvasResizeRect.Height);

                    if (fitZoom < effectiveZoom * 0.99)
                    {
                        double newZoom = Math.Max(0.25, Math.Floor(vm.Zoom * fitZoom / effectiveZoom * 100) / 100);
                        if (newZoom < vm.Zoom)
                        {
                            _canvasResizeSavedZoom = vm.Zoom;
                            vm.Zoom = newZoom;
                        }
                    }
                }
            }

            // Let the zoom change settle before adding the room around the image.
            Dispatcher.UIThread.Post(() =>
            {
                if (!_canvasResizeActive)
                {
                    return;
                }

                EnsureCanvasResizeRoom();
                UpdateCanvasResizeVisuals();
            }, DispatcherPriority.Background);
            UpdateCanvasResizeVisuals();
        }

        /// <summary>Puts back the zoom the user had before the tool zoomed out.</summary>
        private void RestoreCanvasResizeZoom()
        {
            if (_canvasResizeSavedZoom is double zoom && DataContext is MainViewModel vm)
            {
                vm.Zoom = zoom;
            }

            _canvasResizeSavedZoom = null;
        }

        /// <summary>Applies the new canvas size. Returns true if the tool was active.</summary>
        internal bool TryConfirmCanvasResize()
        {
            if (!_canvasResizeActive)
            {
                return false;
            }

            var sourceImage = _editorCore.SourceImage;
            var scrollViewer = this.FindControl<ScrollViewer>("CanvasScrollViewer");
            var annotationCanvas = this.FindControl<Canvas>("AnnotationCanvas");

            if (sourceImage == null || scrollViewer == null || annotationCanvas == null)
            {
                EndCanvasResize(keepImagePosition: false);
                return true;
            }

            int newLeft = (int)Math.Round(_canvasResizeRect.Left);
            int newTop = (int)Math.Round(_canvasResizeRect.Top);
            int newRight = (int)Math.Round(_canvasResizeRect.Right);
            int newBottom = (int)Math.Round(_canvasResizeRect.Bottom);

            int addLeft = -newLeft;
            int addTop = -newTop;
            int addRight = newRight - sourceImage.Width;
            int addBottom = newBottom - sourceImage.Height;

            if (addLeft == 0 && addTop == 0 && addRight == 0 && addBottom == 0)
            {
                EndCanvasResize(keepImagePosition: true);
                return true;
            }

            // Where the new top-left corner is on screen now: keep it there after the resize.
            Point? targetOrigin = annotationCanvas.TranslatePoint(new Point(newLeft, newTop), scrollViewer);

            RemoveCanvasResizeVisualsAndHandlers();

            _canvasResizeApplying = true;
            _suppressScrollResetForAutoExpand = true;
            bool resized;
            try
            {
                resized = _editorCore.ResizeCanvas(addTop, addRight, addBottom, addLeft, AutoExpandCanvasColor);
            }
            finally
            {
                _canvasResizeApplying = false;
            }

            Dispatcher.UIThread.Post(() =>
            {
                _suppressScrollResetForAutoExpand = false;
                if (_workspaceDisposed)
                {
                    return;
                }

                bool restoreZoom = _canvasResizeSavedZoom.HasValue;
                SetCanvasResizeRoom(new Thickness(0), resized && !restoreZoom ? targetOrigin : null);
                RestoreCanvasResizeZoom();
                _selectionController.UpdateSelectionHandles();
            }, DispatcherPriority.Background);

            return true;
        }

        /// <summary>Closes the tool without changing the canvas. Returns true if the tool was active.</summary>
        internal bool CancelCanvasResize()
        {
            if (!_canvasResizeActive)
            {
                return false;
            }

            EndCanvasResize(keepImagePosition: true);
            return true;
        }

        private void EndCanvasResize(bool keepImagePosition)
        {
            if (!_canvasResizeActive)
            {
                return;
            }

            var scrollViewer = this.FindControl<ScrollViewer>("CanvasScrollViewer");
            var annotationCanvas = this.FindControl<Canvas>("AnnotationCanvas");
            Point? origin = keepImagePosition && scrollViewer != null && annotationCanvas != null
                ? annotationCanvas.TranslatePoint(default, scrollViewer)
                : null;

            RemoveCanvasResizeVisualsAndHandlers();
            SetCanvasResizeRoom(new Thickness(0), _canvasResizeSavedZoom.HasValue ? null : origin);
            RestoreCanvasResizeZoom();
        }

        private void RemoveCanvasResizeVisualsAndHandlers()
        {
            _canvasResizeActive = false;
            _canvasResizeDragEdges = CanvasResizeEdges.None;

            if (_canvasResizeHost != null)
            {
                _canvasResizeHost.RemoveHandler(PointerPressedEvent, OnCanvasResizePointerPressed);
                _canvasResizeHost.RemoveHandler(PointerMovedEvent, OnCanvasResizePointerMoved);
                _canvasResizeHost.RemoveHandler(PointerReleasedEvent, OnCanvasResizePointerReleased);
                _canvasResizeHost.RemoveHandler(PointerCaptureLostEvent, OnCanvasResizePointerCaptureLost);
                _canvasResizeHost.Cursor = null;
                _canvasResizeHost = null;
            }

            var scrollViewer = this.FindControl<ScrollViewer>("CanvasScrollViewer");
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged -= OnCanvasResizeScrollChanged;
            }

            foreach (var visual in _canvasResizeVisuals)
            {
                (visual.Parent as Panel)?.Children.Remove(visual);
            }

            _canvasResizeVisuals.Clear();
            _canvasResizeFillRects.Clear();
            _canvasResizeShadeRects.Clear();
            _canvasResizeHandles.Clear();
            _canvasResizeEdgeStrips.Clear();
            _canvasResizeFrame = null;
            _canvasResizePanel = null;
            _canvasResizeSizeText = null;

            if (DataContext is MainViewModel vm)
            {
                vm.IsCanvasSizeModeActive = false;
            }
        }

        /// <summary>Called when the image changes from somewhere else (undo, new image...): close the tool.</summary>
        private void OnImageChangedDuringCanvasResize()
        {
            if (_canvasResizeActive && !_canvasResizeApplying)
            {
                EndCanvasResize(keepImagePosition: false);
            }
        }

        #region Room around the image

        /// <summary>
        /// Adds empty room around the image inside the scroll viewer, so the frame can be dragged
        /// beyond the image even when the image fills the window. The room only grows while the tool is open.
        /// </summary>
        private void EnsureCanvasResizeRoom()
        {
            var scrollViewer = this.FindControl<ScrollViewer>("CanvasScrollViewer");
            if (scrollViewer?.Content is not Layoutable content || _editorCore.SourceImage == null)
            {
                return;
            }

            double zoom = DataContext is MainViewModel vm && vm.EffectiveZoom > 0 ? vm.EffectiveZoom : 1.0;
            double baseX = scrollViewer.Viewport.Width * CanvasResizeRoomFactor;
            double baseY = scrollViewer.Viewport.Height * CanvasResizeRoomFactor;
            double width = _editorCore.SourceImage.Width;
            double height = _editorCore.SourceImage.Height;

            // Same room on opposite sides: a centered image then stays in place while the room grows.
            double roomX = baseX + Math.Max(Math.Max(0, -_canvasResizeRect.Left), Math.Max(0, _canvasResizeRect.Right - width)) * zoom;
            double roomY = baseY + Math.Max(Math.Max(0, -_canvasResizeRect.Top), Math.Max(0, _canvasResizeRect.Bottom - height)) * zoom;
            var needed = new Thickness(roomX, roomY, roomX, roomY);

            Thickness current = content.Margin;
            var grown = new Thickness(
                Math.Max(current.Left, needed.Left),
                Math.Max(current.Top, needed.Top),
                Math.Max(current.Right, needed.Right),
                Math.Max(current.Bottom, needed.Bottom));

            if (grown == current)
            {
                return;
            }

            var annotationCanvas = this.FindControl<Canvas>("AnnotationCanvas");
            Point? origin = annotationCanvas?.TranslatePoint(default, scrollViewer);
            SetCanvasResizeRoom(grown, origin);
        }

        /// <summary>
        /// Sets the room around the image. When <paramref name="keepOriginAt"/> is given, scrolls so the
        /// image's top-left corner stays at that point of the scroll viewer (so the image does not jump).
        /// </summary>
        private void SetCanvasResizeRoom(Thickness room, Point? keepOriginAt)
        {
            var scrollViewer = this.FindControl<ScrollViewer>("CanvasScrollViewer");
            var annotationCanvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (scrollViewer?.Content is not Layoutable content)
            {
                return;
            }

            if (content.Margin != room)
            {
                content.Margin = room;
            }

            scrollViewer.UpdateLayout();

            if (keepOriginAt.HasValue && annotationCanvas != null)
            {
                Point? now = annotationCanvas.TranslatePoint(default, scrollViewer);
                if (now.HasValue)
                {
                    Vector delta = now.Value - keepOriginAt.Value;
                    scrollViewer.Offset = new Vector(
                        Math.Max(0, scrollViewer.Offset.X + delta.X),
                        Math.Max(0, scrollViewer.Offset.Y + delta.Y));
                }
            }
        }

        #endregion

        #region Pointer handling

        private void OnCanvasResizePointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!_canvasResizeActive || _canvasResizeHost == null || IsCanvasResizePassThroughSource(e.Source))
            {
                return;
            }

            var properties = e.GetCurrentPoint(_canvasResizeHost).Properties;
            if (properties.IsMiddleButtonPressed)
            {
                return; // Panning keeps working.
            }

            e.Handled = true;

            if (properties.IsRightButtonPressed)
            {
                CancelCanvasResize();
                return;
            }

            if (!properties.IsLeftButtonPressed)
            {
                return;
            }

            var overlayHost = this.FindControl<Canvas>("OverlayHost");
            var annotationCanvas = this.FindControl<Canvas>("AnnotationCanvas");
            if (overlayHost == null || annotationCanvas == null)
            {
                return;
            }

            CanvasResizeEdges edges = HitTestCanvasResizeFrame(e.GetPosition(overlayHost));
            if (edges == CanvasResizeEdges.None)
            {
                if (e.ClickCount == 2)
                {
                    TryConfirmCanvasResize();
                }

                return;
            }

            _canvasResizeDragEdges = edges;
            _canvasResizeDragStart = e.GetPosition(annotationCanvas);
            _canvasResizeDragStartRect = _canvasResizeRect;
            _canvasResizeHost.Cursor = GetCanvasResizeCursor(edges);
            e.Pointer.Capture(_canvasResizeHost);
        }

        private void OnCanvasResizePointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_canvasResizeActive || _canvasResizeDragEdges == CanvasResizeEdges.None)
            {
                return;
            }

            var annotationCanvas = this.FindControl<Canvas>("AnnotationCanvas");
            var scrollViewer = this.FindControl<ScrollViewer>("CanvasScrollViewer");
            if (annotationCanvas == null || scrollViewer == null)
            {
                return;
            }

            e.Handled = true;

            AutoScrollForCanvasResize(scrollViewer, e.GetPosition(scrollViewer));

            Point current = e.GetPosition(annotationCanvas);
            _canvasResizeRect = ResizeCanvasFrame(_canvasResizeDragStartRect, _canvasResizeDragEdges, current - _canvasResizeDragStart);

            EnsureCanvasResizeRoom();
            UpdateCanvasResizeVisuals();
        }

        private void OnCanvasResizePointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (!_canvasResizeActive || _canvasResizeDragEdges == CanvasResizeEdges.None)
            {
                return;
            }

            e.Handled = true;
            StopCanvasResizeDrag();
            e.Pointer.Capture(null);
        }

        private void OnCanvasResizePointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
        {
            StopCanvasResizeDrag();
        }

        private void StopCanvasResizeDrag()
        {
            _canvasResizeDragEdges = CanvasResizeEdges.None;
            if (_canvasResizeHost != null)
            {
                _canvasResizeHost.Cursor = null;
            }
        }

        /// <summary>Clicks on the tool panel, its popups and scroll bars must keep working normally.</summary>
        private bool IsCanvasResizePassThroughSource(object? source)
        {
            for (var visual = source as Visual; visual != null; visual = visual.GetVisualParent())
            {
                if (visual == _canvasResizePanel || visual is ScrollBar)
                {
                    return true;
                }
            }

            return false;
        }

        private static Rect ResizeCanvasFrame(Rect start, CanvasResizeEdges edges, Vector delta)
        {
            double left = start.Left;
            double top = start.Top;
            double right = start.Right;
            double bottom = start.Bottom;

            if (edges.HasFlag(CanvasResizeEdges.Left))
            {
                left = Math.Round(start.Left + delta.X);
                left = Math.Clamp(left, right - CanvasResizeMaxSize, right - CanvasResizeMinSize);
            }
            else if (edges.HasFlag(CanvasResizeEdges.Right))
            {
                right = Math.Round(start.Right + delta.X);
                right = Math.Clamp(right, left + CanvasResizeMinSize, left + CanvasResizeMaxSize);
            }

            if (edges.HasFlag(CanvasResizeEdges.Top))
            {
                top = Math.Round(start.Top + delta.Y);
                top = Math.Clamp(top, bottom - CanvasResizeMaxSize, bottom - CanvasResizeMinSize);
            }
            else if (edges.HasFlag(CanvasResizeEdges.Bottom))
            {
                bottom = Math.Round(start.Bottom + delta.Y);
                bottom = Math.Clamp(bottom, top + CanvasResizeMinSize, top + CanvasResizeMaxSize);
            }

            return new Rect(left, top, right - left, bottom - top);
        }

        /// <summary>Scrolls the view when the pointer is dragged past the edge of the visible area.</summary>
        private static void AutoScrollForCanvasResize(ScrollViewer scrollViewer, Point position)
        {
            const double edge = 16;
            const double speed = 0.5;
            double dx = 0;
            double dy = 0;

            if (position.X < edge) dx = (position.X - edge) * speed;
            else if (position.X > scrollViewer.Viewport.Width - edge) dx = (position.X - (scrollViewer.Viewport.Width - edge)) * speed;

            if (position.Y < edge) dy = (position.Y - edge) * speed;
            else if (position.Y > scrollViewer.Viewport.Height - edge) dy = (position.Y - (scrollViewer.Viewport.Height - edge)) * speed;

            if (dx != 0 || dy != 0)
            {
                scrollViewer.Offset = new Vector(
                    Math.Max(0, scrollViewer.Offset.X + dx),
                    Math.Max(0, scrollViewer.Offset.Y + dy));
            }
        }

        /// <summary>Which frame edges are under the point (overlay coordinates).</summary>
        private CanvasResizeEdges HitTestCanvasResizeFrame(Point point)
        {
            if (!TryGetCanvasResizeScreenRects(out Rect frame, out _))
            {
                return CanvasResizeEdges.None;
            }

            const double tolerance = CanvasResizeHitTolerance;
            bool withinX = point.X >= frame.Left - tolerance && point.X <= frame.Right + tolerance;
            bool withinY = point.Y >= frame.Top - tolerance && point.Y <= frame.Bottom + tolerance;
            var edges = CanvasResizeEdges.None;

            if (withinY)
            {
                double toLeft = Math.Abs(point.X - frame.Left);
                double toRight = Math.Abs(point.X - frame.Right);
                if (toLeft <= tolerance || toRight <= tolerance)
                {
                    edges |= toRight <= toLeft ? CanvasResizeEdges.Right : CanvasResizeEdges.Left;
                }
            }

            if (withinX)
            {
                double toTop = Math.Abs(point.Y - frame.Top);
                double toBottom = Math.Abs(point.Y - frame.Bottom);
                if (toTop <= tolerance || toBottom <= tolerance)
                {
                    edges |= toBottom <= toTop ? CanvasResizeEdges.Bottom : CanvasResizeEdges.Top;
                }
            }

            return edges;
        }

        private static Cursor GetCanvasResizeCursor(CanvasResizeEdges edges)
        {
            bool horizontal = edges.HasFlag(CanvasResizeEdges.Left) || edges.HasFlag(CanvasResizeEdges.Right);
            bool vertical = edges.HasFlag(CanvasResizeEdges.Top) || edges.HasFlag(CanvasResizeEdges.Bottom);

            if (horizontal && vertical)
            {
                bool mainDiagonal = edges == (CanvasResizeEdges.Left | CanvasResizeEdges.Top) || edges == (CanvasResizeEdges.Right | CanvasResizeEdges.Bottom);
                return new Cursor(mainDiagonal ? StandardCursorType.TopLeftCorner : StandardCursorType.TopRightCorner);
            }

            return new Cursor(horizontal ? StandardCursorType.SizeWestEast : StandardCursorType.SizeNorthSouth);
        }

        #endregion

        #region Visuals

        private void CreateCanvasResizeVisuals(Canvas overlayHost)
        {
            IBrush accent = FindCanvasResizeBrush("ShareX.Brush.Accent") is IBrush accentBrush
                ? accentBrush
                : new SolidColorBrush(Color.FromRgb(0, 120, 215));

            for (int i = 0; i < 4; i++)
            {
                var fill = new Rectangle { IsHitTestVisible = false, IsVisible = false };
                var shade = new Rectangle { IsHitTestVisible = false, IsVisible = false, Fill = new SolidColorBrush(Color.FromArgb(140, 0, 0, 0)) };
                _canvasResizeFillRects.Add(fill);
                _canvasResizeShadeRects.Add(shade);
                AddCanvasResizeVisual(overlayHost, fill, 0);
                AddCanvasResizeVisual(overlayHost, shade, 1);
            }

            _canvasResizeFrame = new Rectangle
            {
                Stroke = accent,
                StrokeThickness = 2,
                IsHitTestVisible = false
            };
            AddCanvasResizeVisual(overlayHost, _canvasResizeFrame, 2);

            // Invisible strips along the edges give the resize cursor anywhere on the edge.
            foreach (CanvasResizeEdges edge in new[] { CanvasResizeEdges.Left, CanvasResizeEdges.Top, CanvasResizeEdges.Right, CanvasResizeEdges.Bottom })
            {
                var strip = new Border
                {
                    Background = Brushes.Transparent,
                    Cursor = GetCanvasResizeCursor(edge)
                };
                _canvasResizeEdgeStrips[edge] = strip;
                AddCanvasResizeVisual(overlayHost, strip, 3);
            }

            foreach (CanvasResizeEdges kind in CanvasResizeHandleKinds)
            {
                var handle = new Border
                {
                    Width = CanvasResizeHandleSize,
                    Height = CanvasResizeHandleSize,
                    Background = Brushes.White,
                    BorderBrush = accent,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(2),
                    Cursor = GetCanvasResizeCursor(kind)
                };
                _canvasResizeHandles[kind] = handle;
                AddCanvasResizeVisual(overlayHost, handle, 4);
            }

            _canvasResizePanel = CreateCanvasResizePanel();
            AddCanvasResizeVisual(overlayHost, _canvasResizePanel, 5);
        }

        private IBrush? FindCanvasResizeBrush(string key)
        {
            return this.TryFindResource(key, ActualThemeVariant, out object? value) ? value as IBrush : null;
        }

        private void AddCanvasResizeVisual(Canvas overlayHost, Control control, int layer)
        {
            control.SetValue(Panel.ZIndexProperty, CanvasResizeZIndex + layer);
            overlayHost.Children.Add(control);
            _canvasResizeVisuals.Add(control);
        }

        private Border CreateCanvasResizePanel()
        {
            _canvasResizeSizeText = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                MinWidth = 96,
                FontWeight = FontWeight.SemiBold
            };

            var colorPicker = new ColorPickerDropdown
            {
                VerticalAlignment = VerticalAlignment.Center,
                SelectedColorValue = (DataContext as MainViewModel)?.Options?.CanvasFillColor ?? Colors.Transparent
            };
            ToolTip.SetTip(colorPicker, Strings.EditorView_CanvasSizeFillColor);
            colorPicker.ColorChanged += (_, brush) =>
            {
                if (DataContext is MainViewModel vm && vm.Options != null && brush is ISolidColorBrush solid)
                {
                    vm.Options.CanvasFillColor = solid.Color;
                    UpdateCanvasResizeVisuals();
                }
            };

            var applyButton = new Button
            {
                Classes = { "crop-confirm-button" },
                Content = Strings.EditorView_CanvasSizeApply,
                Padding = new Thickness(14, 5),
                VerticalAlignment = VerticalAlignment.Center
            };
            applyButton.Click += (_, _) => TryConfirmCanvasResize();

            var cancelButton = new Button
            {
                Content = Strings.EditorView_CanvasSizeCancel,
                Padding = new Thickness(14, 5),
                VerticalAlignment = VerticalAlignment.Center
            };
            cancelButton.Click += (_, _) => CancelCanvasResize();

            var row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8
            };
            row.Children.Add(_canvasResizeSizeText);
            row.Children.Add(colorPicker);
            row.Children.Add(applyButton);
            row.Children.Add(cancelButton);

            var panel = new Border
            {
                Child = row,
                Padding = new Thickness(10, 6),
                CornerRadius = new CornerRadius(6),
                BorderThickness = new Thickness(1),
                Cursor = new Cursor(StandardCursorType.Arrow)
            };

            if (FindCanvasResizeBrush("ShareX.Brush.Background.Toolbar") is IBrush backgroundBrush)
            {
                panel.Background = backgroundBrush;
            }
            else
            {
                panel.Background = new SolidColorBrush(Color.FromArgb(235, 40, 40, 40));
            }

            if (FindCanvasResizeBrush("ShareX.Brush.Border") is IBrush borderBrush)
            {
                panel.BorderBrush = borderBrush;
            }

            return panel;
        }

        private void OnCanvasResizeScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (!_canvasResizeActive || _canvasResizeUpdatePosted)
            {
                return;
            }

            _canvasResizeUpdatePosted = true;
            Dispatcher.UIThread.Post(() =>
            {
                _canvasResizeUpdatePosted = false;
                UpdateCanvasResizeVisuals();
            }, DispatcherPriority.Background);
        }

        /// <summary>Frame and image rectangles in overlay (screen) coordinates.</summary>
        private bool TryGetCanvasResizeScreenRects(out Rect frame, out Rect image)
        {
            frame = default;
            image = default;

            var annotationCanvas = this.FindControl<Canvas>("AnnotationCanvas");
            var overlayHost = this.FindControl<Canvas>("OverlayHost");
            var sourceImage = _editorCore.SourceImage;
            if (annotationCanvas == null || overlayHost == null || sourceImage == null)
            {
                return false;
            }

            Point? frameTopLeft = annotationCanvas.TranslatePoint(_canvasResizeRect.TopLeft, overlayHost);
            Point? frameBottomRight = annotationCanvas.TranslatePoint(_canvasResizeRect.BottomRight, overlayHost);
            Point? imageTopLeft = annotationCanvas.TranslatePoint(default, overlayHost);
            Point? imageBottomRight = annotationCanvas.TranslatePoint(new Point(sourceImage.Width, sourceImage.Height), overlayHost);

            if (!frameTopLeft.HasValue || !frameBottomRight.HasValue || !imageTopLeft.HasValue || !imageBottomRight.HasValue)
            {
                return false;
            }

            frame = new Rect(frameTopLeft.Value, frameBottomRight.Value);
            image = new Rect(imageTopLeft.Value, imageBottomRight.Value);
            return true;
        }

        private void UpdateCanvasResizeVisuals()
        {
            if (!_canvasResizeActive || _canvasResizeFrame == null || _canvasResizePanel == null)
            {
                return;
            }

            if (!TryGetCanvasResizeScreenRects(out Rect frame, out Rect image))
            {
                return;
            }

            // Area that will be added: preview it with the fill color (checkerboard when transparent).
            Color fillColor = (DataContext as MainViewModel)?.Options?.CanvasFillColor ?? Colors.Transparent;
            IBrush fillBrush = fillColor.A == 0 && FindCanvasResizeBrush("ShareX.Brush.Checkered") is IBrush checkeredBrush
                ? checkeredBrush
                : new SolidColorBrush(fillColor.A == 0 ? Color.FromArgb(60, 255, 255, 255) : fillColor);

            PlaceRects(_canvasResizeFillRects, SubtractRect(frame, image), fillBrush);
            // Area that will be cropped away: darken it.
            PlaceRects(_canvasResizeShadeRects, SubtractRect(image, frame), null);

            SetCanvasResizeRect(_canvasResizeFrame, frame);

            double tolerance = CanvasResizeHitTolerance;
            SetCanvasResizeRect(_canvasResizeEdgeStrips[CanvasResizeEdges.Left], new Rect(frame.Left - tolerance, frame.Top, tolerance * 2, frame.Height));
            SetCanvasResizeRect(_canvasResizeEdgeStrips[CanvasResizeEdges.Right], new Rect(frame.Right - tolerance, frame.Top, tolerance * 2, frame.Height));
            SetCanvasResizeRect(_canvasResizeEdgeStrips[CanvasResizeEdges.Top], new Rect(frame.Left, frame.Top - tolerance, frame.Width, tolerance * 2));
            SetCanvasResizeRect(_canvasResizeEdgeStrips[CanvasResizeEdges.Bottom], new Rect(frame.Left, frame.Bottom - tolerance, frame.Width, tolerance * 2));

            foreach (var (kind, handle) in _canvasResizeHandles)
            {
                double x = kind.HasFlag(CanvasResizeEdges.Left) ? frame.Left : kind.HasFlag(CanvasResizeEdges.Right) ? frame.Right : frame.Center.X;
                double y = kind.HasFlag(CanvasResizeEdges.Top) ? frame.Top : kind.HasFlag(CanvasResizeEdges.Bottom) ? frame.Bottom : frame.Center.Y;
                Canvas.SetLeft(handle, x - CanvasResizeHandleSize / 2);
                Canvas.SetTop(handle, y - CanvasResizeHandleSize / 2);
            }

            if (_canvasResizeSizeText != null)
            {
                _canvasResizeSizeText.Text = $"{(int)Math.Round(_canvasResizeRect.Width)} × {(int)Math.Round(_canvasResizeRect.Height)}";
            }

            PlaceCanvasResizePanel(frame);
        }

        private void PlaceCanvasResizePanel(Rect frame)
        {
            var overlayHost = this.FindControl<Canvas>("OverlayHost");
            if (_canvasResizePanel == null || overlayHost == null)
            {
                return;
            }

            const double gap = 14;
            const double edge = 8;
            _canvasResizePanel.Measure(Size.Infinity);
            Size size = _canvasResizePanel.DesiredSize;
            double hostWidth = overlayHost.Bounds.Width;
            double hostHeight = overlayHost.Bounds.Height;

            double x = frame.Center.X - size.Width / 2;
            double y = frame.Bottom + gap;
            if (y + size.Height > hostHeight - edge)
            {
                y = frame.Top - gap - size.Height;
            }

            if (y < edge)
            {
                // Frame fills the view: keep the panel at the bottom of the visible area.
                y = hostHeight - size.Height - edge;
            }

            x = Math.Clamp(x, edge, Math.Max(edge, hostWidth - size.Width - edge));
            y = Math.Clamp(y, edge, Math.Max(edge, hostHeight - size.Height - edge));

            Canvas.SetLeft(_canvasResizePanel, x);
            Canvas.SetTop(_canvasResizePanel, y);
        }

        private static void PlaceRects(List<Rectangle> pool, List<Rect> rects, IBrush? fill)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (i < rects.Count)
                {
                    if (fill != null)
                    {
                        pool[i].Fill = fill;
                    }

                    SetCanvasResizeRect(pool[i], rects[i]);
                    pool[i].IsVisible = true;
                }
                else
                {
                    pool[i].IsVisible = false;
                }
            }
        }

        private static void SetCanvasResizeRect(Control control, Rect rect)
        {
            Canvas.SetLeft(control, rect.Left);
            Canvas.SetTop(control, rect.Top);
            control.Width = Math.Max(0, rect.Width);
            control.Height = Math.Max(0, rect.Height);
        }

        /// <summary>Parts of <paramref name="a"/> outside <paramref name="b"/>, as up to four rectangles.</summary>
        private static List<Rect> SubtractRect(Rect a, Rect b)
        {
            var result = new List<Rect>(4);
            if (a.Width <= 0 || a.Height <= 0)
            {
                return result;
            }

            double left = Math.Max(a.Left, b.Left);
            double top = Math.Max(a.Top, b.Top);
            double right = Math.Min(a.Right, b.Right);
            double bottom = Math.Min(a.Bottom, b.Bottom);

            if (right <= left || bottom <= top)
            {
                result.Add(a);
                return result;
            }

            if (top > a.Top) result.Add(new Rect(a.Left, a.Top, a.Width, top - a.Top));
            if (bottom < a.Bottom) result.Add(new Rect(a.Left, bottom, a.Width, a.Bottom - bottom));
            if (left > a.Left) result.Add(new Rect(a.Left, top, left - a.Left, bottom - top));
            if (right < a.Right) result.Add(new Rect(right, top, a.Right - right, bottom - top));
            return result;
        }

        #endregion
    }
}
