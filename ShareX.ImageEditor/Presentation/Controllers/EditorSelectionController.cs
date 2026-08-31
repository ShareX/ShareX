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
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.Controls;
using ShareX.ImageEditor.Presentation.Helpers;
using ShareX.ImageEditor.Presentation.Rendering;
using ShareX.ImageEditor.Presentation.ViewModels;
using ShareX.ImageEditor.Presentation.Views;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Controllers;

public class EditorSelectionController
{
    private static readonly Cursor SelectToolCursor = new(StandardCursorType.Arrow);
    private const string SegmentStartHandleTag = "SegmentStart";
    private const string SegmentEndHandleTag = "SegmentEnd";
    private const string SegmentCenterHandleTag = "SegmentCenter";
    private static double ToOverlayCoordinate(double value) => value + EditorView.OverlayCanvasBleed;
    private static Point ToOverlayPoint(Point value) => new(ToOverlayCoordinate(value.X), ToOverlayCoordinate(value.Y));

    private readonly EditorView _view;
    private Control? _selectedShape;
    private List<Control> _selectionHandles = new();
    private bool _isDraggingHandle;
    private Control? _draggedHandle;
    private Point _lastDragPoint;
    private Point _startPoint; // Used for resizing deltas
    private bool _isDraggingShape;
    private global::Avalonia.Controls.Shapes.Line? _rotationLine; // Dotted line connecting top-center to rotation handle
    private bool _pendingEmojiExactRender;

    // Reactive bounds tracking for text annotations
    private Control? _observedShape;
    private EventHandler<AvaloniaPropertyChangedEventArgs>? _boundsHandler;

    // Hover tracking for ant lines
    private Control? _hoveredShape;
    private global::Avalonia.Controls.Shapes.Rectangle? _hoverOutlineBlack;
    private global::Avalonia.Controls.Shapes.Rectangle? _hoverOutlineWhite;
    private global::Avalonia.Controls.Shapes.Polyline? _hoverPolylineBlack;
    private global::Avalonia.Controls.Shapes.Polyline? _hoverPolylineWhite;
    private global::Avalonia.Controls.Shapes.Ellipse? _hoverEllipseBlack;
    private global::Avalonia.Controls.Shapes.Ellipse? _hoverEllipseWhite;
    private TextBox? _balloonTextEditor;
    private Point _lastPointerCanvasPoint;
    private bool _hasLastPointerCanvasPoint;
    private KeyModifiers _currentKeyModifiers;

    public Control? SelectedShape => _selectedShape;
    public bool IsInteractionActive => IsSelectionInteractionActive();

    // Event invoked when visual needs update (for Effects)
    public event Action<Control>? RequestUpdateEffect;

    // Event invoked when selection state changes (shape selected or deselected)
    public event Action<bool>? SelectionChanged;

    public EditorSelectionController(EditorView view)
    {
        _view = view;
    }

    public void ClearSelection()
    {
        var wasSelected = _selectedShape != null;
        _selectedShape = null;
        _isDraggingHandle = false;
        _draggedHandle = null;
        _isDraggingShape = false;
        _pendingEmojiExactRender = false;
        UpdateBoundsObserver(); // Clear observer
        ClearHoverOutline();
        UpdateSelectionHandles();
        if (wasSelected)
        {
            SelectionChanged?.Invoke(false);
        }
    }

    internal void ClearHoverFeedback()
    {
        ClearHoverOutline();
    }

    public void SetSelectedShape(Control shape)
    {
        var wasNull = _selectedShape == null;
        _selectedShape = shape;
        UpdateBoundsObserver();

        if (_hoveredShape != shape)
        {
            ClearHoverOutline();
        }

        // Set the hovered shape to the selected shape so ant lines appear
        _hoveredShape = shape;
        ApplyHoveredShapeCursor();
        UpdateHoverOutline();
        UpdateSelectionHandles();

        // Notify selection changed
        if (wasNull && shape != null)
        {
            SelectionChanged?.Invoke(true);
        }

        // Auto-enter text edit mode for speech balloon
        if (shape is SpeechBalloonControl balloonControl)
        {
            var canvas = _view.FindControl<Canvas>("AnnotationCanvas");
            if (canvas != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    ShowSpeechBalloonTextEditor(balloonControl, canvas);
                }, DispatcherPriority.Normal);
            }
        }
    }

    public bool OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        var canvas = _view.FindControl<Canvas>("AnnotationCanvas");
        if (canvas == null) return false;

        var point = e.GetPosition(canvas);
        _currentKeyModifiers = e.KeyModifiers;
        if (_view.DataContext is MainViewModel vm && ShouldIgnoreSelection(vm, e.KeyModifiers))
        {
            UpdateSelectionHandles();
            return false;
        }

        // Check if clicked on a handle
        var overlay = _view.FindControl<Canvas>("OverlayCanvas");
        if (overlay != null)
        {
            var handleSource = e.Source as Control;
            if (handleSource != null && overlay.Children.Contains(handleSource) && handleSource is Border)
            {
                if (handleSource.Tag?.ToString() == SegmentCenterHandleTag
                    && _selectedShape?.Tag is ICurvedSegmentAnnotation curvedSegment
                    && CurvedSegmentHelper.SupportsCurve(curvedSegment))
                {
                    CurvedSegmentHelper.EnsureCurveActivated(curvedSegment);
                }

                _isDraggingHandle = true;
                _draggedHandle = handleSource;
                _startPoint = point; // Capture start for resize delta
                _view.BeginInteractionCursorCapture(e.Pointer, CursorAssetLoader.CustomCursorKind.ClosedHand);
                e.Handled = true;
                return true;
            }
        }

        // Check if clicked on an existing shape (Select or Drag)
        // If we are in Select tool OR user holds Ctrl (multi-select not impl yet) or just clicking existing shapes to move
        // BUT: If active tool is a drawing tool, we usually prioritize drawing NEW shapes unless we click ON a selected shape?
        // ShareX logic: If in Select Tool, selecting works. If in other tools, usually drawing takes precedence unless we click a handle.
        // But logic in EditorView.axaml.cs had:
        // if (vm.ActiveTool == EditorTool.Select ...) -> Try select.

        if (_view.DataContext is MainViewModel activeVm)
        {
            // When a drawing tool is active, allow selecting and dragging only shapes
            // that belong to the same tool type, without switching to the Select tool.
            if (activeVm.ActiveTool != EditorTool.Select && activeVm.ActiveTool != EditorTool.Spotlight && activeVm.ActiveTool != EditorTool.Freehand)
            {
                // Hit test - find the direct child of the canvas
                var hitSource = e.Source as global::Avalonia.Visual;
                Control? hitTarget = null;
                while (hitSource != null && hitSource != canvas)
                {
                    var candidate = hitSource as Control;
                    if (candidate != null && canvas.Children.Contains(candidate))
                    {
                        hitTarget = candidate;
                        break;
                    }
                    hitSource = hitSource.GetVisualParent();
                }

                // Fallback: manual hit test for thin shapes (e.g. lines, arrows)
                var manualHit = HitTestShape(canvas, point);
                if (hitTarget == null || GetControlToolType(hitTarget) != activeVm.ActiveTool)
                {
                    if (manualHit != null && GetControlToolType(manualHit) == activeVm.ActiveTool)
                    {
                        hitTarget = manualHit;
                    }
                    else if (hitTarget != null && GetControlToolType(hitTarget) != activeVm.ActiveTool)
                    {
                        hitTarget = null;
                    }
                }

                if (hitTarget != null && GetControlToolType(hitTarget) == activeVm.ActiveTool)
                {
                    if (e.ClickCount == 2 && TryHandleDoubleClickEdit(hitTarget, canvas))
                    {
                        e.Handled = true;
                        return true;
                    }

                    _selectedShape = hitTarget;
                    UpdateBoundsObserver();
                    _isDraggingShape = true;
                    _lastDragPoint = point;
                    UpdateSelectionHandles();
                    SelectionChanged?.Invoke(true);
                    _view.BeginInteractionCursorCapture(e.Pointer, CursorAssetLoader.CustomCursorKind.ClosedHand);
                    e.Handled = true;
                    return true;
                }
            }

            if (activeVm.ActiveTool == EditorTool.Select || activeVm.ActiveTool == EditorTool.Spotlight)
            {
                // Hit test
                var hitSource = e.Source as global::Avalonia.Visual;
                Control? hitTarget = null;
                while (hitSource != null && hitSource != canvas)
                {
                    var candidate = hitSource as Control;
                    if (candidate != null && canvas.Children.Contains(candidate))
                    {
                        hitTarget = candidate;
                        break;
                    }
                    hitSource = hitSource.GetVisualParent();
                }

                var manualHit = HitTestShape(canvas, point);
                if (manualHit is SpotlightControl || manualHit is OutlinedTextControl)
                {
                    hitTarget = manualHit;
                }

                // Integrity Check: Ensure we didn't mistakenly hit the Spotlight overlay (visual pass-through)
                if (hitTarget is SpotlightControl sc && manualHit != sc)
                {
                    hitTarget = null;
                }

                if (activeVm.ActiveTool == EditorTool.Spotlight)
                {
                    if (!(hitTarget is SpotlightControl)) hitTarget = null;
                    if (!(manualHit is SpotlightControl)) manualHit = null;
                }

                if (hitTarget != null)
                {
                    if (e.ClickCount == 2 && TryHandleDoubleClickEdit(hitTarget, canvas))
                    {
                        e.Handled = true;
                        return true;
                    }

                    _selectedShape = hitTarget;
                    UpdateBoundsObserver();
                    _isDraggingShape = true;
                    _lastDragPoint = point;
                    UpdateSelectionHandles();
                    SelectionChanged?.Invoke(true);
                    _view.BeginInteractionCursorCapture(e.Pointer, CursorAssetLoader.CustomCursorKind.ClosedHand);
                    e.Handled = true;
                    return true;
                }
                else
                {
                    if (manualHit != null)
                    {
                        if (e.ClickCount == 2 && TryHandleDoubleClickEdit(manualHit, canvas))
                        {
                            e.Handled = true;
                            return true;
                        }

                        _selectedShape = manualHit;
                        UpdateBoundsObserver();
                        _isDraggingShape = true;
                        _lastDragPoint = point;
                        UpdateSelectionHandles();
                        SelectionChanged?.Invoke(true);
                        _view.BeginInteractionCursorCapture(e.Pointer, CursorAssetLoader.CustomCursorKind.ClosedHand);
                        e.Handled = true;
                        return true;
                    }

                    ClearSelection();
                    // Don't return true, allowing rubber band selection (if implemented) or just clearing
                    return false;
                }
            }
        }

        return false;
    }

    private bool TryHandleDoubleClickEdit(Control hitTarget, Canvas canvas)
    {
        switch (hitTarget)
        {
            case OutlinedTextControl otc:
                ShowTextEditor(otc, canvas);
                return true;
            case SpeechBalloonControl balloon:
                ShowSpeechBalloonTextEditor(balloon, canvas);
                return true;
            case global::Avalonia.Controls.Image emojiControl when emojiControl.Tag is EmojiAnnotation emojiAnnotation:
                ShowEmojiPickerForReplacement(emojiControl, emojiAnnotation);
                return true;
            case global::Avalonia.Controls.Image imageControl when imageControl.Tag is ImageAnnotation imageAnnotation && imageAnnotation is not EmojiAnnotation && imageAnnotation is not CursorAnnotation:
                ShowImagePickerForReplacement(imageControl, imageAnnotation);
                return true;
            default:
                return false;
        }
    }

    private void ShowEmojiPickerForReplacement(global::Avalonia.Controls.Image emojiControl, EmojiAnnotation emojiAnnotation)
    {
        if (_view.DataContext is not MainViewModel vm)
        {
            return;
        }

        _selectedShape = emojiControl;
        UpdateBoundsObserver();
        UpdateSelectionHandles();
        SelectionChanged?.Invoke(true);

        vm.ShowEmojiPickerDialog(entry =>
        {
            emojiAnnotation.UnicodeSequence = entry.Unicode;
            emojiAnnotation.DisplayName = entry.DisplayName;
            emojiAnnotation.ClearImage();

            AnnotationVisualFactory.UpdateVisualControl(emojiControl, emojiAnnotation);
            UpdateSelectionHandles();

            if (_view.DataContext is MainViewModel activeVm)
            {
                activeVm.HasAnnotations = true;
                activeVm.IsDirty = true;
            }
        });
    }

    private async void ShowImagePickerForReplacement(global::Avalonia.Controls.Image imageControl, ImageAnnotation imageAnnotation)
    {
        _selectedShape = imageControl;
        UpdateBoundsObserver();
        UpdateSelectionHandles();
        SelectionChanged?.Invoke(true);

        bool wasReplaced = await _view.ReplaceImageAnnotationFromFilePickerAsync(imageAnnotation, imageControl);
        if (wasReplaced)
        {
            UpdateSelectionHandles();
        }
    }

    public bool OnPointerMoved(object sender, PointerEventArgs e)
    {
        var canvas = _view.FindControl<Canvas>("AnnotationCanvas");
        if (canvas == null) return false;

        var currentPoint = e.GetPosition(canvas);
        _currentKeyModifiers = e.KeyModifiers;
        _lastPointerCanvasPoint = currentPoint;
        _hasLastPointerCanvasPoint = true;

        if (_isDraggingHandle && _draggedHandle != null && _selectedShape != null)
        {
            UpdateCanvasCursorForSelectionInteraction();
            RefreshSelectionHandleCursors();
            HandleResize(currentPoint, e.KeyModifiers.HasFlag(KeyModifiers.Shift));
            e.Handled = true;
            return true;
        }

        if (_isDraggingShape && _selectedShape != null)
        {
            UpdateCanvasCursorForSelectionInteraction();
            HandleMove(currentPoint);
            e.Handled = true;
            return true;
        }

        if (_view.DataContext is MainViewModel vm && ShouldIgnoreSelection(vm, e.KeyModifiers))
        {
            ClearHoverOutline();
            return false;
        }

        // Update hover state when not dragging
        UpdateHoverState(canvas, currentPoint);

        return false;
    }

    internal void RefreshHoverFeedback(KeyModifiers keyModifiers)
    {
        _currentKeyModifiers = keyModifiers;

        if (IsSelectionInteractionActive())
        {
            return;
        }

        UpdateSelectionHandles();

        if (_view.DataContext is not MainViewModel vm)
        {
            return;
        }

        if (ShouldIgnoreSelection(vm, keyModifiers))
        {
            ClearHoverOutline();
            return;
        }

        if (!_hasLastPointerCanvasPoint)
        {
            return;
        }

        var canvas = _view.FindControl<Canvas>("AnnotationCanvas");
        if (canvas == null)
        {
            return;
        }

        UpdateHoverState(canvas, _lastPointerCanvasPoint);
    }

    public bool OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        _currentKeyModifiers = e.KeyModifiers;

        if (_isDraggingHandle)
        {
            FinalizeEmojiInteractiveRender();
            _isDraggingHandle = false;
            _draggedHandle = null;
            UpdateCanvasCursorForSelectionInteraction();
            RefreshSelectionHandleCursors();
            e.Pointer.Capture(null);

            if (_selectedShape?.Tag is BaseEffectAnnotation)
            {
                RequestUpdateEffect?.Invoke(_selectedShape);
            }
            return true;
        }

        if (_isDraggingShape)
        {
            _pendingEmojiExactRender = false;
            _isDraggingShape = false;
            UpdateCanvasCursorForSelectionInteraction();
            RefreshSelectionHandleCursors();
            e.Pointer.Capture(null);

            if (_selectedShape?.Tag is BaseEffectAnnotation)
            {
                RequestUpdateEffect?.Invoke(_selectedShape);
            }
            return true;
        }

        return false;
    }

    private void HandleResize(Point currentPoint, bool isShiftHeld = false)
    {
        if (_selectedShape == null || _draggedHandle == null)
        {
            return;
        }

        var handleTag = _draggedHandle.Tag?.ToString();
        if (string.IsNullOrEmpty(handleTag))
        {
            return;
        }

        var deltaX = currentPoint.X - _startPoint.X;
        var deltaY = currentPoint.Y - _startPoint.Y;

        // Special handling for line/arrow endpoints and curve control point.
        if (_selectedShape is global::Avalonia.Controls.Shapes.Path segmentPath
            && segmentPath.Tag is Annotation segmentAnnotation
            && segmentPath.Tag is ICurvedSegmentAnnotation curvedSegment)
        {
            var startPoint = new Point(curvedSegment.StartPoint.X, curvedSegment.StartPoint.Y);
            var endPoint = new Point(curvedSegment.EndPoint.X, curvedSegment.EndPoint.Y);

            if (handleTag == SegmentStartHandleTag)
            {
                startPoint = isShiftHeld
                    ? EditorInputController.SnapTo45Degrees(endPoint, currentPoint)
                    : currentPoint;
                CurvedSegmentHelper.SetEndpoints(curvedSegment, ToSKPoint(startPoint), ToSKPoint(endPoint));
            }
            else if (handleTag == SegmentEndHandleTag)
            {
                endPoint = isShiftHeld
                    ? EditorInputController.SnapTo45Degrees(startPoint, currentPoint)
                    : currentPoint;
                CurvedSegmentHelper.SetEndpoints(curvedSegment, ToSKPoint(startPoint), ToSKPoint(endPoint));
            }
            else if (handleTag == SegmentCenterHandleTag)
            {
                CurvedSegmentHelper.SetCurvePoint(curvedSegment, ToSKPoint(currentPoint));
            }

            AnnotationVisualFactory.UpdateVisualControl(segmentPath, segmentAnnotation);
            _startPoint = currentPoint;
            UpdateSelectionHandles();
            return;
        }

        // Special handling for SpeechBalloonControl tail dragging
        if (_selectedShape is SpeechBalloonControl balloonControl && balloonControl.Annotation is SpeechBalloonAnnotation balloon && balloon.TailEnabled && handleTag == "BalloonTail")
        {
            balloon.SetTailPoint(GetSpeechBalloonTailPoint(balloon, currentPoint));
            AnnotationVisualFactory.UpdateVisualControl(
                balloonControl,
                balloon,
                AnnotationVisualMode.Persisted,
                _view.EditorCore.CanvasSize.Width,
                _view.EditorCore.CanvasSize.Height);
            _startPoint = currentPoint;
            UpdateSelectionHandles();
            return;
        }

        if (_selectedShape is StepControl stepControl && stepControl.Annotation is NumberAnnotation number && number.TailEnabled && handleTag == "StepTail")
        {
            number.SetTailPoint(new SKPoint((float)currentPoint.X, (float)currentPoint.Y));
            AnnotationVisualFactory.UpdateVisualControl(
                stepControl,
                number,
                AnnotationVisualMode.Persisted,
                _view.EditorCore.CanvasSize.Width,
                _view.EditorCore.CanvasSize.Height);
            _startPoint = currentPoint;
            UpdateSelectionHandles();
            return;
        }

        if (handleTag == "Rotate"
            && TryGetRotatableAnnotation(_selectedShape, out Annotation? rotatableAnnotation)
            && rotatableAnnotation != null)
        {
            ApplyRotationToSelectedShape(currentPoint, rotatableAnnotation, isShiftHeld);
            _startPoint = currentPoint;
            UpdateSelectionHandles();
            return;
        }

        if (TryGetRotatableAnnotation(_selectedShape, out Annotation? resizableRotatedAnnotation)
            && resizableRotatedAnnotation is not null
            && resizableRotatedAnnotation is not EmojiAnnotation
            && resizableRotatedAnnotation.RotationAngle != 0
            && TryResizeRotatedAnnotation(resizableRotatedAnnotation, currentPoint, handleTag))
        {
            _startPoint = currentPoint;
            UpdateSelectionHandles();

            if (_selectedShape?.Tag is BaseEffectAnnotation)
            {
                RequestUpdateEffect?.Invoke(_selectedShape);
            }

            return;
        }

        if (_selectedShape.Tag is EmojiAnnotation emojiAnnotation)
        {
            ResizeEmojiAnnotation(emojiAnnotation, currentPoint, handleTag);
            _startPoint = currentPoint;
            UpdateSelectionHandles();
            return;
        }

        // Regular shapes
        var shapeRect = GetLogicalRect(_selectedShape);
        var left = shapeRect.Left;
        var top = shapeRect.Top;
        var width = shapeRect.Width;
        var height = shapeRect.Height;

        // Special handling for SpeechBalloonControl resizing
        if (_selectedShape is SpeechBalloonControl resizeBalloonControl && resizeBalloonControl.Annotation is SpeechBalloonAnnotation resizeBalloon)
        {
            double newLeft = left;
            double newTop = top;
            double newWidth = width;
            double newHeight = height;

            if (handleTag.Contains("Right")) newWidth = Math.Max(20, width + deltaX);
            else if (handleTag.Contains("Left")) { var change = Math.Min(width - 20, deltaX); newLeft += change; newWidth -= change; }

            if (handleTag.Contains("Bottom")) newHeight = Math.Max(20, height + deltaY);
            else if (handleTag.Contains("Top")) { var change = Math.Min(height - 20, deltaY); newTop += change; newHeight -= change; }

            ApplyResizedBounds(resizeBalloon, newLeft, newTop, newWidth, newHeight);

            _startPoint = currentPoint;
            UpdateSelectionHandles();
            return;
        }

        if (_selectedShape is ShareX.ImageEditor.Presentation.Controls.SpotlightControl spotlight && spotlight.Annotation is SpotlightAnnotation sa)
        {
            var bounds = sa.GetBounds();
            var newLeft = bounds.Left;
            var newTop = bounds.Top;
            var newWidth = bounds.Width;
            var newHeight = bounds.Height;

            float dx = (float)deltaX;
            float dy = (float)deltaY;

            if (handleTag.Contains("Right")) newWidth = Math.Max(10, newWidth + dx);
            else if (handleTag.Contains("Left")) { var change = Math.Min(newWidth - 10, dx); newLeft += change; newWidth -= change; }

            if (handleTag.Contains("Bottom")) newHeight = Math.Max(10, newHeight + dy);
            else if (handleTag.Contains("Top")) { var change = Math.Min(newHeight - 10, dy); newTop += change; newHeight -= change; }

            sa.StartPoint = ToSKPoint(new Point(newLeft, newTop));
            sa.EndPoint = ToSKPoint(new Point(newLeft + newWidth, newTop + newHeight));
            _view.RefreshSpotlightOverlay();

            _startPoint = currentPoint;
            UpdateSelectionHandles();
            return;
        }

        if (_selectedShape is global::Avalonia.Controls.Shapes.Rectangle
            || _selectedShape is global::Avalonia.Controls.Shapes.Ellipse
            || _selectedShape is global::Avalonia.Controls.Image
            || _selectedShape is Grid
            || _selectedShape is OutlinedTextControl)
        {
            Rect resized = SelectionResizeNode.Resize(shapeRect, handleTag, new Vector(deltaX, deltaY));

            Canvas.SetLeft(_selectedShape, resized.Left);
            Canvas.SetTop(_selectedShape, resized.Top);
            _selectedShape.Width = resized.Width;
            _selectedShape.Height = resized.Height;

            // Sync annotation points for hit testing
            if (_selectedShape.Tag is Annotation annotation)
            {
                annotation.StartPoint = new SKPoint((float)resized.Left, (float)resized.Top);
                annotation.EndPoint = new SKPoint((float)resized.Right, (float)resized.Bottom);
            }
        }

        _startPoint = currentPoint;
        UpdateSelectionHandles();

        // Real-time effect update during resize
        if (_selectedShape?.Tag is BaseEffectAnnotation)
        {
            RequestUpdateEffect?.Invoke(_selectedShape);
        }
    }

    private void ResizeEmojiAnnotation(EmojiAnnotation annotation, Point currentPoint, string handleTag)
    {
        if (_selectedShape is not global::Avalonia.Controls.Image imageControl)
        {
            return;
        }

        const double minSize = 16;

        if (!TryGetRotatedSquareResizeBounds(annotation, currentPoint, handleTag, minSize, out Rect resizedBounds))
        {
            return;
        }

        annotation.StartPoint = new SKPoint((float)resizedBounds.Left, (float)resizedBounds.Top);
        annotation.EndPoint = new SKPoint((float)resizedBounds.Right, (float)resizedBounds.Bottom);
        _pendingEmojiExactRender = true;

        AnnotationVisualFactory.UpdateVisualControl(
            imageControl,
            annotation,
            AnnotationVisualMode.Persisted,
            _view.EditorCore.CanvasSize.Width,
            _view.EditorCore.CanvasSize.Height,
            useInteractiveEmojiRender: true);
    }

    private bool TryGetRotatedSquareResizeBounds(Annotation annotation, Point currentPoint, string handleTag, double minSize, out Rect resizedBounds)
    {
        resizedBounds = default;

        if (_selectedShape == null || !TryGetHandleDirection(handleTag, out int horizontalDirection, out int verticalDirection))
        {
            return false;
        }

        var bounds = GetLogicalRect(_selectedShape);
        double size = Math.Max(bounds.Width, bounds.Height);
        if (size <= 0)
        {
            return false;
        }

        Point origin = default;
        Point localCenter = UnrotatePoint(bounds.Center, origin, annotation.RotationAngle);
        Point localPointer = UnrotatePoint(currentPoint, origin, annotation.RotationAngle);
        double halfSize = size / 2.0;
        double newLocalCenterX = localCenter.X;
        double newLocalCenterY = localCenter.Y;
        double newSize;

        if (horizontalDirection != 0 && verticalDirection != 0)
        {
            double anchorX = localCenter.X - (horizontalDirection * halfSize);
            double anchorY = localCenter.Y - (verticalDirection * halfSize);
            double sizeFromX = horizontalDirection < 0 ? anchorX - localPointer.X : localPointer.X - anchorX;
            double sizeFromY = verticalDirection < 0 ? anchorY - localPointer.Y : localPointer.Y - anchorY;

            newSize = Math.Max(minSize, Math.Max(sizeFromX, sizeFromY));
            double draggedX = anchorX + (horizontalDirection * newSize);
            double draggedY = anchorY + (verticalDirection * newSize);
            newLocalCenterX = (anchorX + draggedX) / 2.0;
            newLocalCenterY = (anchorY + draggedY) / 2.0;
        }
        else if (horizontalDirection != 0)
        {
            double anchorX = localCenter.X - (horizontalDirection * halfSize);
            double sizeFromX = horizontalDirection < 0 ? anchorX - localPointer.X : localPointer.X - anchorX;

            newSize = Math.Max(minSize, sizeFromX);
            double draggedX = anchorX + (horizontalDirection * newSize);
            newLocalCenterX = (anchorX + draggedX) / 2.0;
        }
        else if (verticalDirection != 0)
        {
            double anchorY = localCenter.Y - (verticalDirection * halfSize);
            double sizeFromY = verticalDirection < 0 ? anchorY - localPointer.Y : localPointer.Y - anchorY;

            newSize = Math.Max(minSize, sizeFromY);
            double draggedY = anchorY + (verticalDirection * newSize);
            newLocalCenterY = (anchorY + draggedY) / 2.0;
        }
        else
        {
            return false;
        }

        Point newWorldCenter = RotatePoint(new Point(newLocalCenterX, newLocalCenterY), origin, annotation.RotationAngle);
        resizedBounds = new Rect(newWorldCenter.X - (newSize / 2.0), newWorldCenter.Y - (newSize / 2.0), newSize, newSize);
        return true;
    }

    private bool TryResizeRotatedAnnotation(Annotation annotation, Point currentPoint, string handleTag)
    {
        if (_selectedShape == null || !TryGetHandleDirection(handleTag, out int horizontalDirection, out int verticalDirection))
        {
            return false;
        }

        var bounds = GetLogicalRect(_selectedShape);
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return false;
        }

        double minWidth = annotation is TextAnnotation ? 10 : 1;
        double minHeight = annotation is TextAnnotation ? 10 : 1;
        Point origin = default;

        // Project the drag point into the annotation's local axes so resize follows the rotated frame.
        Point localCenter = UnrotatePoint(bounds.Center, origin, annotation.RotationAngle);
        Point localPointer = UnrotatePoint(currentPoint, origin, annotation.RotationAngle);

        double halfWidth = bounds.Width / 2.0;
        double halfHeight = bounds.Height / 2.0;
        double anchorX = localCenter.X - (horizontalDirection * halfWidth);
        double anchorY = localCenter.Y - (verticalDirection * halfHeight);
        double newLocalCenterX = localCenter.X;
        double newLocalCenterY = localCenter.Y;
        double newWidth = bounds.Width;
        double newHeight = bounds.Height;

        if (horizontalDirection != 0)
        {
            double clampedPointerX = horizontalDirection < 0
                ? Math.Min(localPointer.X, anchorX - minWidth)
                : Math.Max(localPointer.X, anchorX + minWidth);

            newWidth = Math.Abs(anchorX - clampedPointerX);
            newLocalCenterX = (anchorX + clampedPointerX) / 2.0;
        }

        if (verticalDirection != 0)
        {
            double clampedPointerY = verticalDirection < 0
                ? Math.Min(localPointer.Y, anchorY - minHeight)
                : Math.Max(localPointer.Y, anchorY + minHeight);

            newHeight = Math.Abs(anchorY - clampedPointerY);
            newLocalCenterY = (anchorY + clampedPointerY) / 2.0;
        }

        Point newWorldCenter = RotatePoint(new Point(newLocalCenterX, newLocalCenterY), origin, annotation.RotationAngle);
        ApplyResizedBounds(annotation, newWorldCenter.X - (newWidth / 2.0), newWorldCenter.Y - (newHeight / 2.0), newWidth, newHeight);
        return true;
    }

    private void ApplyResizedBounds(Annotation annotation, double left, double top, double width, double height)
    {
        if (_selectedShape == null)
        {
            return;
        }

        var previousBounds = annotation.GetBounds();
        var newBounds = new SKRect((float)left, (float)top, (float)(left + width), (float)(top + height));

        Canvas.SetLeft(_selectedShape, left);
        Canvas.SetTop(_selectedShape, top);
        _selectedShape.Width = Math.Max(1, width);
        _selectedShape.Height = Math.Max(1, height);

        annotation.StartPoint = new SKPoint((float)left, (float)top);
        annotation.EndPoint = new SKPoint((float)(left + width), (float)(top + height));

        if (annotation is SpeechBalloonAnnotation balloon)
        {
            UpdateSpeechBalloonTailForResize(balloon, previousBounds, newBounds);
        }

        if (annotation.RotationAngle != 0)
        {
            _selectedShape.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
            _selectedShape.RenderTransform = new RotateTransform(annotation.RotationAngle);
        }

        AnnotationVisualFactory.UpdateVisualControl(
            _selectedShape,
            annotation,
            AnnotationVisualMode.Persisted,
            _view.EditorCore.CanvasSize.Width,
            _view.EditorCore.CanvasSize.Height);

        _selectedShape.InvalidateVisual();
        _selectedShape.InvalidateMeasure();
    }

    private static void UpdateSpeechBalloonTailForResize(SpeechBalloonAnnotation annotation, SKRect previousBounds, SKRect newBounds)
    {
        if (!annotation.HasTailPoint || previousBounds.Width <= 0 || previousBounds.Height <= 0)
        {
            return;
        }

        var tailPoint = annotation.GetEffectiveTailPoint();
        float scaleX = newBounds.Width / previousBounds.Width;
        float scaleY = newBounds.Height / previousBounds.Height;

        annotation.SetTailPoint(new SKPoint(
            newBounds.Left + ((tailPoint.X - previousBounds.Left) * scaleX),
            newBounds.Top + ((tailPoint.Y - previousBounds.Top) * scaleY)));
    }

    private static bool TryGetHandleDirection(string handleTag, out int horizontalDirection, out int verticalDirection)
    {
        return SelectionResizeNode.TryGetDirection(handleTag, out horizontalDirection, out verticalDirection);
    }

    public void MoveSelectedShape(double deltaX, double deltaY)
    {
        if (_selectedShape == null) return;

        _lastDragPoint = new Point(0, 0);
        HandleMove(new Point(deltaX, deltaY));

        if (_selectedShape?.Tag is BaseEffectAnnotation)
        {
            RequestUpdateEffect?.Invoke(_selectedShape);
        }
    }

    private void HandleMove(Point currentPoint)
    {
        if (_selectedShape == null)
        {
            return;
        }

        var deltaX = currentPoint.X - _lastDragPoint.X;
        var deltaY = currentPoint.Y - _lastDragPoint.Y;

        if (_selectedShape is global::Avalonia.Controls.Shapes.Path segmentPath
            && segmentPath.Tag is Annotation segmentAnnotation
            && segmentPath.Tag is ICurvedSegmentAnnotation curvedSegment)
        {
            curvedSegment.StartPoint = new SKPoint(curvedSegment.StartPoint.X + (float)deltaX, curvedSegment.StartPoint.Y + (float)deltaY);
            curvedSegment.EndPoint = new SKPoint(curvedSegment.EndPoint.X + (float)deltaX, curvedSegment.EndPoint.Y + (float)deltaY);
            CurvedSegmentHelper.OffsetCurvePoint(curvedSegment, (float)deltaX, (float)deltaY);
            AnnotationVisualFactory.UpdateVisualControl(segmentPath, segmentAnnotation);

            _lastDragPoint = currentPoint;
            UpdateSelectionHandles();
            return;
        }

        if (_selectedShape is SpeechBalloonControl balloonControl && balloonControl.Annotation is SpeechBalloonAnnotation balloon)
        {
            var currentStart = balloon.StartPoint;
            var currentEnd = balloon.EndPoint;
            var currentTailPoint = balloon.GetEffectiveTailPoint();

            var newStartPoint = new SKPoint(currentStart.X + (float)deltaX, currentStart.Y + (float)deltaY);
            var newEndPoint = new SKPoint(currentEnd.X + (float)deltaX, currentEnd.Y + (float)deltaY);

            balloon.StartPoint = newStartPoint;
            balloon.EndPoint = newEndPoint;

            balloon.SetTailPoint(new SKPoint(currentTailPoint.X + (float)deltaX, currentTailPoint.Y + (float)deltaY));

            var newLeft = Canvas.GetLeft(balloonControl) + deltaX;
            var newTop = Canvas.GetTop(balloonControl) + deltaY;
            Canvas.SetLeft(balloonControl, newLeft);
            Canvas.SetTop(balloonControl, newTop);

            balloonControl.InvalidateVisual();
            _lastDragPoint = currentPoint;
            UpdateSelectionHandles();
            return;
        }

        if (_selectedShape is StepControl stepControl && stepControl.Annotation is NumberAnnotation number)
        {
            number.StartPoint = new SKPoint(number.StartPoint.X + (float)deltaX, number.StartPoint.Y + (float)deltaY);
            number.EndPoint = new SKPoint(number.EndPoint.X + (float)deltaX, number.EndPoint.Y + (float)deltaY);

            if (number.HasTailPoint)
            {
                number.SetTailPoint(new SKPoint(number.TailPoint.X + (float)deltaX, number.TailPoint.Y + (float)deltaY));
            }

            var newLeft = Canvas.GetLeft(stepControl) + deltaX;
            var newTop = Canvas.GetTop(stepControl) + deltaY;
            Canvas.SetLeft(stepControl, newLeft);
            Canvas.SetTop(stepControl, newTop);

            stepControl.InvalidateVisual();
            _lastDragPoint = currentPoint;
            UpdateSelectionHandles();
            return;
        }

        // Handle Path (Freehand/SmartEraser) movement by translating all points
        if (_selectedShape is global::Avalonia.Controls.Shapes.Path path && path.Tag is FreehandAnnotation freehand)
        {
            // Update all points in the annotation
            for (int i = 0; i < freehand.Points.Count; i++)
            {
                var oldPt = freehand.Points[i];
                freehand.Points[i] = new SKPoint(oldPt.X + (float)deltaX, oldPt.Y + (float)deltaY);
            }

            // Regenerate the path geometry using the new points
            path.Data = freehand.CreateSmoothedGeometry();

            // Force visual update
            path.InvalidateVisual();
            _lastDragPoint = currentPoint;
            UpdateSelectionHandles();
            return;
        }

        if (_selectedShape is SpotlightControl spotlight && spotlight.Annotation is SpotlightAnnotation sa)
        {
            var currentStart = sa.StartPoint;
            var currentEnd = sa.EndPoint;
            sa.StartPoint = new SKPoint(currentStart.X + (float)deltaX, currentStart.Y + (float)deltaY);
            sa.EndPoint = new SKPoint(currentEnd.X + (float)deltaX, currentEnd.Y + (float)deltaY);
            _view.RefreshSpotlightOverlay();

            _lastDragPoint = currentPoint;
            UpdateSelectionHandles();
            UpdateHoverOutline();
            return;
        }

        var left = Canvas.GetLeft(_selectedShape);
        var top = Canvas.GetTop(_selectedShape);
        Canvas.SetLeft(_selectedShape, left + deltaX);
        Canvas.SetTop(_selectedShape, top + deltaY);

        // Sync annotation points for hit testing (Rectangle, Ellipse, Highlight, etc.)
        if (_selectedShape.Tag is Annotation annotation)
        {
            annotation.StartPoint = new SKPoint(annotation.StartPoint.X + (float)deltaX, annotation.StartPoint.Y + (float)deltaY);
            annotation.EndPoint = new SKPoint(annotation.EndPoint.X + (float)deltaX, annotation.EndPoint.Y + (float)deltaY);
        }

        _lastDragPoint = currentPoint;
        UpdateSelectionHandles();

        // Real-time effect update during move
        if (_selectedShape?.Tag is BaseEffectAnnotation)
        {
            RequestUpdateEffect?.Invoke(_selectedShape);
        }
    }

    public void UpdateSelectionHandles()
    {
        var overlay = _view.FindControl<Canvas>("OverlayCanvas");
        if (overlay == null) return;

        foreach (var handle in _selectionHandles)
        {
            overlay.Children.Remove(handle);
        }
        _selectionHandles.Clear();

        if (_selectedShape == null) return;

        if (ShouldSuppressSelectionHandles()) return;

        if (_selectedShape is global::Avalonia.Controls.Shapes.Path segmentPath
            && segmentPath.Tag is ICurvedSegmentAnnotation curvedSegment)
        {
            CreateHandle(curvedSegment.StartPoint.X, curvedSegment.StartPoint.Y, SegmentStartHandleTag);
            CreateHandle(curvedSegment.EndPoint.X, curvedSegment.EndPoint.Y, SegmentEndHandleTag);

            if (CurvedSegmentHelper.SupportsCurve(curvedSegment))
            {
                var curvePoint = CurvedSegmentHelper.GetEffectiveCurvePoint(curvedSegment);
                CreateHandle(curvePoint.X, curvePoint.Y, SegmentCenterHandleTag);
            }

            UpdateHoverOutline();
            return;
        }

        if (_selectedShape is SpeechBalloonControl balloonControl && balloonControl.Annotation is SpeechBalloonAnnotation balloon)
        {
            if (balloon.TailEnabled)
            {
                if (!balloon.HasTailPoint)
                {
                    balloon.EnsureTailPointInitialized();
                    AnnotationVisualFactory.UpdateVisualControl(
                        balloonControl,
                        balloon,
                        AnnotationVisualMode.Persisted,
                        _view.EditorCore.CanvasSize.Width,
                        _view.EditorCore.CanvasSize.Height);
                }
            }

            if (TryCreateRotatableSelectionHandles(balloonControl, overlay))
            {
                if (balloon.TailEnabled)
                {
                    Point rotatedTailHandle = GetSpeechBalloonTailHandlePoint(balloon);
                    CreateHandle(rotatedTailHandle.X, rotatedTailHandle.Y, "BalloonTail");
                }

                UpdateHoverOutline();
                return;
            }

            UpdateHoverOutline();
            return;
        }

        if (_selectedShape is StepControl stepControl && stepControl.Annotation is NumberAnnotation number)
        {
            if (number.TailEnabled)
            {
                var tailHandlePoint = number.GetTailHandlePoint();
                CreateHandle(tailHandlePoint.X, tailHandlePoint.Y, "StepTail");
            }

            UpdateHoverOutline();
            return;
        }

        if (_selectedShape is global::Avalonia.Controls.Image { Tag: CursorAnnotation })
        {
            UpdateHoverOutline();
            return;
        }

        if (_selectedShape is Polyline
            || _selectedShape is global::Avalonia.Controls.Shapes.Path { Tag: FreehandAnnotation })
        {
            UpdateHoverOutline();
            return;
        }

        if (TryCreateRotatableSelectionHandles(_selectedShape, overlay))
        {
            return;
        }

        if (_selectedShape is ShareX.ImageEditor.Presentation.Controls.SpotlightControl spotlightControl && spotlightControl.Annotation is SpotlightAnnotation spotlightAnn)
        {
            var bounds = spotlightAnn.GetBounds();
            CreateHandle(bounds.Left, bounds.Top, "TopLeft");
            CreateHandle(bounds.Left + bounds.Width / 2, bounds.Top, "TopCenter");
            CreateHandle(bounds.Right, bounds.Top, "TopRight");
            CreateHandle(bounds.Right, bounds.Top + bounds.Height / 2, "RightCenter");
            CreateHandle(bounds.Right, bounds.Bottom, "BottomRight");
            CreateHandle(bounds.Left + bounds.Width / 2, bounds.Bottom, "BottomCenter");
            CreateHandle(bounds.Left, bounds.Bottom, "BottomLeft");
            CreateHandle(bounds.Left, bounds.Top + bounds.Height / 2, "LeftCenter");

            UpdateHoverOutline();
            return;
        }

        if (_selectedShape is Grid)
        {
            UpdateHoverOutline();
            return;
        }

        var boundsRect = GetLogicalRect(_selectedShape);
        var width = boundsRect.Width;
        var height = boundsRect.Height;

        if (width <= 0 || height <= 0) return;

        var shapeLeft = boundsRect.Left;
        var shapeTop = boundsRect.Top;

        Rect selectionBounds = new(shapeLeft, shapeTop, width, height);
        foreach (SelectionResizeNodeKind node in SelectionResizeNode.RectangleNodes)
        {
            Point position = SelectionResizeNode.GetPosition(selectionBounds, node);
            CreateHandle(position.X, position.Y, node.ToString());
        }
        UpdateHoverOutline();
    }

    private void FinalizeEmojiInteractiveRender()
    {
        if (!_pendingEmojiExactRender)
        {
            return;
        }

        _pendingEmojiExactRender = false;

        if (_selectedShape is not global::Avalonia.Controls.Image imageControl || imageControl.Tag is not EmojiAnnotation emojiAnnotation)
        {
            return;
        }

        AnnotationVisualFactory.UpdateVisualControl(
            imageControl,
            emojiAnnotation,
            AnnotationVisualMode.Persisted,
            _view.EditorCore.CanvasSize.Width,
            _view.EditorCore.CanvasSize.Height);
    }

    private bool TryCreateRotatableSelectionHandles(Control shape, Canvas overlay)
    {
        if (!TryGetRotatableAnnotation(shape, out Annotation? annotation))
        {
            return false;
        }

        if (annotation == null)
        {
            return false;
        }

        var bounds = GetLogicalRect(shape);
        double left = bounds.Left;
        double top = bounds.Top;
        double width = bounds.Width;
        double height = bounds.Height;

        if (double.IsNaN(width) || width <= 0) width = 20;
        if (double.IsNaN(height) || height <= 0) height = 20;

        Point center = new(left + width / 2, top + height / 2);
        double rotationAngle = annotation.RotationAngle;

        var topLeft = RotatePoint(new Point(left, top), center, rotationAngle);
        var topCenter = RotatePoint(new Point(left + width / 2, top), center, rotationAngle);
        var topRight = RotatePoint(new Point(left + width, top), center, rotationAngle);
        var rightCenter = RotatePoint(new Point(left + width, top + height / 2), center, rotationAngle);
        var bottomRight = RotatePoint(new Point(left + width, top + height), center, rotationAngle);
        var bottomCenter = RotatePoint(new Point(left + width / 2, top + height), center, rotationAngle);
        var bottomLeft = RotatePoint(new Point(left, top + height), center, rotationAngle);
        var leftCenter = RotatePoint(new Point(left, top + height / 2), center, rotationAngle);

        CreateHandle(topLeft.X, topLeft.Y, "TopLeft");
        CreateHandle(topCenter.X, topCenter.Y, "TopCenter");
        CreateHandle(topRight.X, topRight.Y, "TopRight");
        CreateHandle(rightCenter.X, rightCenter.Y, "RightCenter");
        CreateHandle(bottomRight.X, bottomRight.Y, "BottomRight");
        CreateHandle(bottomCenter.X, bottomCenter.Y, "BottomCenter");
        CreateHandle(bottomLeft.X, bottomLeft.Y, "BottomLeft");
        CreateHandle(leftCenter.X, leftCenter.Y, "LeftCenter");

        var rotateHandlePoint = RotatePoint(new Point(left + width / 2, top - 30), center, rotationAngle);
        _rotationLine = new global::Avalonia.Controls.Shapes.Line
        {
            StartPoint = ToOverlayPoint(topCenter),
            EndPoint = ToOverlayPoint(rotateHandlePoint),
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            StrokeDashArray = new Avalonia.Collections.AvaloniaList<double> { 3, 3 },
            IsHitTestVisible = false
        };
        _rotationLine.SetValue(Panel.ZIndexProperty, 1);

        var rotationLineWhite = new global::Avalonia.Controls.Shapes.Line
        {
            StartPoint = ToOverlayPoint(topCenter),
            EndPoint = ToOverlayPoint(rotateHandlePoint),
            Stroke = Brushes.White,
            StrokeThickness = 1,
            StrokeDashArray = new Avalonia.Collections.AvaloniaList<double> { 3, 3 },
            StrokeDashOffset = 3,
            IsHitTestVisible = false
        };
        rotationLineWhite.SetValue(Panel.ZIndexProperty, 1);

        overlay.Children.Add(_rotationLine);
        overlay.Children.Add(rotationLineWhite);
        _selectionHandles.Add(_rotationLine);
        _selectionHandles.Add(rotationLineWhite);

        CreateHandle(rotateHandlePoint.X, rotateHandlePoint.Y, "Rotate");
        UpdateHoverOutline();
        return true;
    }

    private static bool TryGetRotatableAnnotation(Control? control, out Annotation? annotation)
    {
        switch (control)
        {
            case SpeechBalloonControl { Annotation: SpeechBalloonAnnotation balloonAnnotation }:
                annotation = balloonAnnotation;
                return true;
            case global::Avalonia.Controls.Shapes.Rectangle { Tag: BaseEffectAnnotation effectAnnotation }:
                annotation = effectAnnotation;
                return true;
            case global::Avalonia.Controls.Shapes.Rectangle { Tag: RectangleAnnotation rectangleAnnotation }
                when rectangleAnnotation is not SmartEraserAnnotation:
                annotation = rectangleAnnotation;
                return true;
            case global::Avalonia.Controls.Shapes.Ellipse { Tag: EllipseAnnotation ellipseAnnotation }:
                annotation = ellipseAnnotation;
                return true;
            case OutlinedTextControl { Tag: TextAnnotation textAnnotation }:
                annotation = textAnnotation;
                return true;
            case global::Avalonia.Controls.Image { Tag: EmojiAnnotation emojiAnnotation }:
                annotation = emojiAnnotation;
                return true;
            case global::Avalonia.Controls.Image { Tag: CursorAnnotation }:
                annotation = null;
                return false;
            case global::Avalonia.Controls.Image { Tag: ImageAnnotation imageAnnotation }:
                annotation = imageAnnotation;
                return true;
            default:
                annotation = null;
                return false;
        }
    }

    private void ApplyRotationToSelectedShape(Point currentPoint, Annotation annotation, bool isShiftHeld)
    {
        if (_selectedShape == null)
        {
            return;
        }

        var bounds = GetLogicalRect(_selectedShape);
        double centerX = bounds.Left + bounds.Width / 2;
        double centerY = bounds.Top + bounds.Height / 2;
        double dx = currentPoint.X - centerX;
        double dy = currentPoint.Y - centerY;
        double angleRad = Math.Atan2(dx, -dy);
        double angleDeg = angleRad * 180.0 / Math.PI;

        if (isShiftHeld)
        {
            angleDeg = Math.Round(angleDeg / 45.0) * 45.0;
        }

        annotation.RotationAngle = (float)angleDeg;

        AnnotationVisualFactory.UpdateVisualControl(
            _selectedShape,
            annotation,
            AnnotationVisualMode.Persisted,
            _view.EditorCore.CanvasSize.Width,
            _view.EditorCore.CanvasSize.Height);

        if (annotation is BaseEffectAnnotation)
        {
            RequestUpdateEffect?.Invoke(_selectedShape);
        }
    }

    private static Point RotatePoint(Point point, Point center, double angleDeg)
    {
        if (angleDeg == 0)
        {
            return point;
        }

        double rad = angleDeg * Math.PI / 180.0;
        double cos = Math.Cos(rad);
        double sin = Math.Sin(rad);
        double dx = point.X - center.X;
        double dy = point.Y - center.Y;
        return new Point(
            center.X + dx * cos - dy * sin,
            center.Y + dx * sin + dy * cos);
    }

    private static Point UnrotatePoint(Point point, Point center, double angleDeg)
    {
        return RotatePoint(point, center, -angleDeg);
    }

    private static Point GetSpeechBalloonTailHandlePoint(SpeechBalloonAnnotation annotation)
    {
        var tailPoint = annotation.GetEffectiveTailPoint();
        Point handlePoint = new(tailPoint.X, tailPoint.Y);

        if (annotation.RotationAngle == 0)
        {
            return handlePoint;
        }

        var bounds = annotation.GetBounds();
        Point center = new(bounds.MidX, bounds.MidY);
        return RotatePoint(handlePoint, center, annotation.RotationAngle);
    }

    private static SKPoint GetSpeechBalloonTailPoint(SpeechBalloonAnnotation annotation, Point visualPoint)
    {
        Point unrotatedPoint = visualPoint;

        if (annotation.RotationAngle != 0)
        {
            var bounds = annotation.GetBounds();
            Point center = new(bounds.MidX, bounds.MidY);
            unrotatedPoint = UnrotatePoint(visualPoint, center, annotation.RotationAngle);
        }

        return new SKPoint((float)unrotatedPoint.X, (float)unrotatedPoint.Y);
    }

    private void CreateHandle(double x, double y, string tag)
    {
        var overlay = _view.FindControl<Canvas>("OverlayCanvas");
        if (overlay == null) return;

        Border handleBorder = SelectionResizeNode.Create(
            x,
            y,
            tag,
            GetSelectionHandleCursor(),
            EditorView.OverlayCanvasBleed);

        overlay.Children.Add(handleBorder);
        _selectionHandles.Add(handleBorder);
    }

    private bool IsSelectionInteractionActive()
    {
        return _isDraggingHandle || _isDraggingShape;
    }

    private Cursor GetSelectionHandleCursor()
    {
        return IsSelectionInteractionActive()
            ? _view.GetClosedHandCursor()
            : _view.GetOpenHandCursor();
    }

    private void RefreshSelectionHandleCursors()
    {
        var cursor = GetSelectionHandleCursor();

        foreach (var handle in _selectionHandles)
        {
            if (handle is Border border)
            {
                border.Cursor = cursor;
            }
        }
    }

    private void UpdateCanvasCursorForSelectionInteraction()
    {
        if (IsSelectionInteractionActive())
        {
            _view.ApplyInteractionCursor(CursorAssetLoader.CustomCursorKind.ClosedHand);
        }
        else
        {
            _view.RestoreEditorSurfaceCursorForActiveTool();
        }
    }

    private Cursor GetDefaultCanvasCursor()
    {
        if (_view.DataContext is not MainViewModel vm)
        {
            return SelectToolCursor;
        }

        return vm.ActiveTool switch
        {
            EditorTool.Select => SelectToolCursor,
            EditorTool.Crop or EditorTool.CutOut => _view.GetCrosshairCursor(),
            _ => _view.GetCrosshairCursor()
        };
    }

    private void ShowSpeechBalloonTextEditor(SpeechBalloonControl balloonControl, Canvas unusedCanvas)
    {
        if (balloonControl.Annotation == null) return;

        // Use OverlayCanvas to ensure TextBox is on top of everything
        var overlay = _view.FindControl<Canvas>("OverlayCanvas");
        if (overlay == null) return;

        if (_balloonTextEditor != null)
        {
            // Remove from whichever parent it has
            (_balloonTextEditor.Parent as Panel)?.Children.Remove(_balloonTextEditor);
            _balloonTextEditor = null;
        }

        var annotation = balloonControl.Annotation;
        var balloonRect = GetLogicalRect(balloonControl);
        var balloonLeft = balloonRect.Left;
        var balloonTop = balloonRect.Top;
        var balloonWidth = balloonRect.Width;
        var balloonHeight = balloonRect.Height;

        // Check if balloon is too small (e.g. user just clicked without dragging)
        if (balloonWidth < 50 || balloonHeight < 30)
        {
            balloonWidth = Math.Max(balloonWidth, 200);
            balloonHeight = Math.Max(balloonHeight, 100);

            annotation.EndPoint = new SKPoint(
                annotation.StartPoint.X + (float)balloonWidth,
                annotation.StartPoint.Y + (float)balloonHeight
            );

            // Fix Tail Point if it was at 0,0 or default
            if (annotation.TailEnabled &&
                (!annotation.HasTailPoint ||
                (Math.Abs(annotation.TailPoint.X - annotation.StartPoint.X) < 1 && Math.Abs(annotation.TailPoint.Y - annotation.StartPoint.Y) < 1)))
            {
                annotation.SetTailPoint(annotation.GetDefaultTailPoint());
            }

            AnnotationVisualFactory.UpdateVisualControl(
                balloonControl,
                annotation,
                AnnotationVisualMode.Persisted,
                _view.EditorCore.CanvasSize.Width,
                _view.EditorCore.CanvasSize.Height);
            UpdateSelectionHandles();
        }

        // Ensure text is visible during editing
        IBrush foregroundBrush = new SolidColorBrush(Avalonia.Media.Color.Parse(annotation.TextColor));
        try
        {
            var foreColor = Avalonia.Media.Color.Parse(annotation.TextColor);
            var backColor = Avalonia.Media.Color.Parse(annotation.FillColor);

            // Calculate relative luminance
            double foreLum = (0.299 * foreColor.R + 0.587 * foreColor.G + 0.114 * foreColor.B) / 255.0;
            double backLum = (0.299 * backColor.R + 0.587 * backColor.G + 0.114 * backColor.B) / 255.0;
            double backAlpha = backColor.A / 255.0;

            // If background is transparent (showing image/canvas behind), we can't guarantee contrast.

            // If background is visible and contrast is low, pick black or white
            if (backAlpha > 0.1)
            {
                if (Math.Abs(foreLum - backLum) < 0.3)
                {
                    foregroundBrush = backLum > 0.5 ? Avalonia.Media.Brushes.Black : Avalonia.Media.Brushes.White;
                }
            }
        }
        catch { /* Fallback to stroke color */ }

        // Determine a safe background for the TextBox to ensure visibility
        // Use the fill color of the balloon as the base background for the editor
        IBrush editorBackground;
        var fillColor = Avalonia.Media.Color.Parse(annotation.FillColor);

        if (fillColor.A < 20)
        {
            // If balloon is transparent, continue to use the high-contrast semi-transparent background
            var fgBrush = foregroundBrush as SolidColorBrush;
            var fgColor = fgBrush?.Color ?? Avalonia.Media.Colors.Black;
            editorBackground = fgColor.R > 127
                ? new SolidColorBrush(Avalonia.Media.Color.Parse("#AA000000"))
                : new SolidColorBrush(Avalonia.Media.Color.Parse("#AAFFFFFF"));
        }
        else
        {
            // Use the actual fill color
            // We use the exact fill color so it looks seamless
            editorBackground = new SolidColorBrush(fillColor);
        }

        var textBox = new TextBox
        {
            Text = annotation.Text,
            Background = editorBackground,
            BorderThickness = new Thickness(0),
            CornerRadius = new CornerRadius(0), // No corner radius needed if no border/background match
            Foreground = foregroundBrush,
            FontSize = annotation.FontSize,
            FontFamily = new Avalonia.Media.FontFamily(string.IsNullOrWhiteSpace(annotation.FontFamily) ? "Segoe UI" : annotation.FontFamily),
            FontWeight = annotation.IsBold ? FontWeight.Bold : FontWeight.Normal,
            FontStyle = annotation.IsItalic ? FontStyle.Italic : FontStyle.Normal,
            Padding = new Thickness(12),
            TextAlignment = TextHorizontalAlignmentHelper.ToAvaloniaTextAlignment(annotation.HorizontalAlignment),
            HorizontalContentAlignment = TextHorizontalAlignmentHelper.ToHorizontalContentAlignment(annotation.HorizontalAlignment),
            VerticalContentAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
            AcceptsReturn = false,
            TextWrapping = TextWrapping.Wrap,
            Tag = annotation
        };

        // ISSUE-FIX: Override theme resources to ensure Focus state doesn't revert to White background
        textBox.Resources["TextControlBackground"] = editorBackground;
        textBox.Resources["TextControlBackgroundFocused"] = editorBackground;
        textBox.Resources["TextControlBackgroundPointerOver"] = editorBackground;

        // ISSUE-FIX: Override border resources to remove them completely in all states
        textBox.Resources["TextControlBorderThemeThickness"] = new Thickness(0);
        textBox.Resources["TextControlBorderThemeThicknessFocused"] = new Thickness(0);
        textBox.Resources["TextControlBorderThemeThicknessPointerOver"] = new Thickness(0);
        textBox.Resources["TextControlBorderBrush"] = Avalonia.Media.Brushes.Transparent;
        textBox.Resources["TextControlBorderBrushFocused"] = Avalonia.Media.Brushes.Transparent;
        textBox.Resources["TextControlBorderBrushPointerOver"] = Avalonia.Media.Brushes.Transparent;

        // Ensure TextBox is above the balloon geometry (ZIndex 100 might not be enough if Overlay is higher)
        // But AnnotationCanvas is usually below Overlay.
        // We can't put TextBox in Overlay because Overlay is for handles.
        textBox.SetValue(Panel.ZIndexProperty, 9999);
        ApplySpeechBalloonTextEditorLayout(textBox, annotation, balloonLeft, balloonTop, balloonWidth, balloonHeight);

        textBox.LostFocus += (s, args) =>
        {
            if (s is TextBox tb)
            {
                annotation.Text = tb.Text ?? string.Empty;
                balloonControl.InvalidateVisual();
                (tb.Parent as Panel)?.Children.Remove(tb); // Remove from OverlayCanvas
                _balloonTextEditor = null;
            }
        };

        textBox.KeyDown += (s, args) =>
        {
            if (args.Key == Key.Enter && args.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                args.Handled = true;
                int caretIndex = textBox.CaretIndex;
                string currentText = textBox.Text ?? string.Empty;
                textBox.Text = currentText.Substring(0, caretIndex) + "\n" + currentText.Substring(caretIndex);
                textBox.CaretIndex = caretIndex + 1;
            }
        };

        textBox.KeyUp += (s, args) =>
        {
            if ((args.Key == Key.Enter && !args.KeyModifiers.HasFlag(KeyModifiers.Control)) || args.Key == Key.Escape)
            {
                args.Handled = true;
                _view.Focus();
            }
        };

        overlay.Children.Add(textBox); // Add to OverlayCanvas
        _balloonTextEditor = textBox;
        textBox.Focus();
        textBox.CaretIndex = textBox.Text.Length; // Place caret at end

        // Attach extended handlers for live update if needed, or rely on LostFocus
        AttachTextBoxEditHandlers(textBox);
    }

    public void PerformDelete()
    {
        if (_selectedShape != null)
        {
            var canvas = _view.FindControl<Canvas>("AnnotationCanvas");
            if (canvas != null && canvas.Children.Contains(_selectedShape))
            {
                canvas.Children.Remove(_selectedShape);

                if (_selectedShape is SpeechBalloonControl && _balloonTextEditor != null)
                {
                    canvas.Children.Remove(_balloonTextEditor);
                    _balloonTextEditor = null;
                }

                // Note: Undo stack logic not fully integrated here since EditorView owned it.
                // For now we assume EditorView handles undo/redo wrapping or we rely on events.
                // BUT: PerformDelete in EditorView used to push to redo stack?
                // Wait, EditorView.PerformDelete handles undo stack logic.
                // To avoid breaking Undo, we should perhaps keep PerformDelete in EditorView
                // OR EditorSelectionController delegates back to EditorView/UndoService?
                // Given constraints to NO logic changes, I should probably expose the inner delete logic
                // or call back to EditorView to delete.
                // But cleaning up handles is definitely SelectionController job.
            }
            ClearSelection();
        }
    }

    private void UpdateHoverState(Canvas canvas, Point currentPoint)
    {
        // Crop/CutOut/Freehand tools never show hover outlines.
        if (_view.DataContext is MainViewModel vm)
        {
            if (vm.ActiveTool == EditorTool.Crop || vm.ActiveTool == EditorTool.CutOut || vm.ActiveTool == EditorTool.Freehand)
            {
                ClearHoverOutline();
                return;
            }
        }

        // Find shape under cursor (hit test)
        Control? hitShape = HitTestShape(canvas, currentPoint);

        // Filter: If using Spotlight tool, only highlight existing Spotlights
        if (_view.DataContext is MainViewModel vm2 && vm2.ActiveTool == EditorTool.Spotlight)
        {
            if (!(hitShape is SpotlightControl)) hitShape = null;
        }

        // When a drawing tool is active, only hover shapes created by that same tool.
        if (_view.DataContext is MainViewModel vm3
            && vm3.ActiveTool != EditorTool.Select
            && vm3.ActiveTool != EditorTool.Spotlight)
        {
            if (hitShape != null && GetControlToolType(hitShape) != vm3.ActiveTool)
            {
                hitShape = null;
            }
        }

        // If we're hovering over the selected shape, keep showing ant lines on it
        // Otherwise, show ant lines on the hovered (unselected) shape
        if (hitShape == _selectedShape && _selectedShape != null)
        {
            // Keep showing ant lines on selected shape
            if (_hoveredShape != _selectedShape)
            {
                ClearHoverOutline();
                _hoveredShape = _selectedShape;
            }
            ApplyHoveredShapeCursor();
            UpdateHoverOutline();
        }
        else if (hitShape != _hoveredShape)
        {
            // Hovering over a different shape (or no shape)
            ClearHoverOutline();
            _hoveredShape = hitShape;

            if (_hoveredShape != null)
            {
                ApplyHoveredShapeCursor();
                UpdateHoverOutline();
            }
        }
        else if (_hoveredShape != null)
        {
            // Shape is still hovered, update outline position (in case shape moved)
            ApplyHoveredShapeCursor();
            UpdateHoverOutline();
        }
    }

    private static bool ShouldIgnoreSelection(MainViewModel vm, KeyModifiers keyModifiers)
    {
        return vm.ActiveTool != EditorTool.Select && keyModifiers.HasFlag(KeyModifiers.Control);
    }

    private bool ShouldSuppressSelectionHandles()
    {
        return !IsSelectionInteractionActive()
            && _selectedShape != null
            && _view.DataContext is MainViewModel vm
            && ShouldIgnoreSelection(vm, _currentKeyModifiers);
    }

    public Control? HitTestShape(Canvas canvas, Point currentPoint)
    {
        // Iterate through canvas children in reverse (top-most first)
        for (int i = canvas.Children.Count - 1; i >= 0; i--)
        {
            var child = canvas.Children[i] as Control;
            if (child == null) continue;

            // Skip non-moveable overlays and text editors
            if (child.Name == "CropOverlay" || child.Name == "CutOutOverlay") continue;
            // TextBox excluded? No, we want to select it now.
            // if (child is TextBox) continue;

            if (child is SpotlightControl sc && sc.Annotation is SpotlightAnnotation sa)
            {
                if (sa.HitTest(ToSKPoint(currentPoint))) return sc;
                continue;
            }

            // Check if point is within the bounds of this control
            var shapeBounds = GetLogicalRect(child);

            // Special handling for line/arrow paths
            if (child is global::Avalonia.Controls.Shapes.Path && child.Tag is Annotation curveAnnotation && child.Tag is ICurvedSegmentAnnotation)
            {
                var bounds = curveAnnotation.GetBounds();
                shapeBounds = new Rect(bounds.Left - 10, bounds.Top - 10, bounds.Width + 20, bounds.Height + 20);
            }
            // Special handling for Path (Freehand/SmartEraser) - use annotation bounds
            else if (child is global::Avalonia.Controls.Shapes.Path && child.Tag is IPointBasedAnnotation pointAnnotation)
            {
                var annBounds = ((Annotation)pointAnnotation).GetBounds();
                shapeBounds = new Rect(annBounds.Left - 5, annBounds.Top - 5, annBounds.Width + 10, annBounds.Height + 10);
            }
            // Special handling for Polyline (Freehand)
            else if (child is global::Avalonia.Controls.Shapes.Polyline polyline && polyline.Points != null)
            {
                double minX = double.MaxValue, minY = double.MaxValue;
                double maxX = double.MinValue, maxY = double.MinValue;
                foreach (var p in polyline.Points)
                {
                    if (p.X < minX) minX = p.X;
                    if (p.Y < minY) minY = p.Y;
                    if (p.X > maxX) maxX = p.X;
                    if (p.Y > maxY) maxY = p.Y;
                }
                if (minX == double.MaxValue)
                {
                    minX = 0; minY = 0; maxX = 0; maxY = 0;
                }
                else
                {
                    minX -= 5; minY -= 5; maxX += 5; maxY += 5;
                }
                shapeBounds = new Rect(minX, minY, maxX - minX, maxY - minY);
            }

            if (child is SpeechBalloonControl balloonControl && balloonControl.Annotation is SpeechBalloonAnnotation balloonAnnotation)
            {
                shapeBounds = ToRect(balloonAnnotation.GetInteractionBounds(5));
            }
            else if (child.Tag is NumberAnnotation numberAnnotation)
            {
                shapeBounds = ToRect(numberAnnotation.GetInteractionBounds(5));
            }

            // Start with rough bounds check
            bool isRotated = false;
            if (child.Tag is Annotation tagAnn && tagAnn.RotationAngle != 0)
            {
                isRotated = true;
            }

            // If rotated, the axis-aligned 'shapeBounds' is invalid/insufficient.
            // We skip the Contains check and rely on the annotation's own HitTest (which handles rotation).
            if (isRotated || shapeBounds.Contains(currentPoint))
            {
                // Use the annotation's HitTest method if available
                if (child.Tag is Annotation annotation)
                {
                    var skPoint = ToSKPoint(currentPoint);
                    if (annotation.HitTest(skPoint))
                    {
                        return child;
                    }
                }
                else
                {
                    return child;
                }
            }
        }
        return null;
    }

    private void ClearHoverOutline()
    {
        if (_hoveredShape != null)
        {
            _view.SyncAnnotationCursor(_hoveredShape);
        }

        var overlay = _view.FindControl<Canvas>("OverlayCanvas");
        if (_hoverOutlineBlack != null)
        {
            overlay?.Children.Remove(_hoverOutlineBlack);
            _hoverOutlineBlack = null;
        }
        if (_hoverOutlineWhite != null)
        {
            overlay?.Children.Remove(_hoverOutlineWhite);
            _hoverOutlineWhite = null;
        }
        if (_hoverPolylineBlack != null)
        {
            overlay?.Children.Remove(_hoverPolylineBlack);
            _hoverPolylineBlack = null;
        }
        if (_hoverPolylineWhite != null)
        {
            overlay?.Children.Remove(_hoverPolylineWhite);
            _hoverPolylineWhite = null;
        }
        if (_hoverEllipseBlack != null)
        {
            overlay?.Children.Remove(_hoverEllipseBlack);
            _hoverEllipseBlack = null;
        }
        if (_hoverEllipseWhite != null)
        {
            overlay?.Children.Remove(_hoverEllipseWhite);
            _hoverEllipseWhite = null;
        }
        _hoveredShape = null;
    }

    internal void RefreshHoveredShapeCursor()
    {
        ApplyHoveredShapeCursor();
    }

    private void ApplyHoveredShapeCursor()
    {
        if (_hoveredShape == null)
        {
            return;
        }

        if (_view.DataContext is MainViewModel vm
            && vm.ActiveTool != EditorTool.Crop
            && vm.ActiveTool != EditorTool.CutOut)
        {
            _view.ApplyAnnotationCursor(_hoveredShape, SelectToolCursor);
            return;
        }

        _view.SyncAnnotationCursor(_hoveredShape);
    }

    private void UpdateHoverOutline()
    {
        if (_hoveredShape == null) return;

        var overlay = _view.FindControl<Canvas>("OverlayCanvas");
        if (overlay == null) return;

        // 1. Path-based Outline (Polyline)
        // Used for Line, Arrow (Path), and Freehand (Polyline) to show outline along the stroke
        IList<Point>? outlinePoints = null;

        if (_hoveredShape is global::Avalonia.Controls.Shapes.Path curvedPath && curvedPath.Tag is ICurvedSegmentAnnotation curvedSegment)
        {
            outlinePoints = CurvedSegmentHelper.GetPathPoints(curvedSegment)
                .Select(point => new Point(point.X, point.Y))
                .ToList();
        }
        else if (_hoveredShape is global::Avalonia.Controls.Shapes.Path pointPath && pointPath.Tag is IPointBasedAnnotation pointAnnotation)
        {
            // Convert SKPoints to Avalonia Points for the outline
            outlinePoints = new List<Point>();
            foreach (var pt in pointAnnotation.Points)
            {
                outlinePoints.Add(new Point(pt.X, pt.Y));
            }
        }
        else if (_hoveredShape is Polyline polyline)
        {
            outlinePoints = polyline.Points;
        }

        if (outlinePoints != null)
        {
            if (_hoverPolylineBlack == null)
            {
                _hoverPolylineBlack = new Polyline
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    StrokeDashArray = new global::Avalonia.Collections.AvaloniaList<double> { 3, 3 },
                    IsHitTestVisible = false
                };
                overlay.Children.Add(_hoverPolylineBlack);
            }
            if (_hoverPolylineWhite == null)
            {
                _hoverPolylineWhite = new Polyline
                {
                    Stroke = Brushes.White,
                    StrokeThickness = 1,
                    StrokeDashArray = new global::Avalonia.Collections.AvaloniaList<double> { 3, 3 },
                    StrokeDashOffset = 3,
                    IsHitTestVisible = false
                };
                overlay.Children.Add(_hoverPolylineWhite);
            }

            var overlayPoints = new Avalonia.Collections.AvaloniaList<Point>(outlinePoints.Select(ToOverlayPoint));
            _hoverPolylineBlack.Points = overlayPoints;
            _hoverPolylineWhite.Points = overlayPoints;
            return;
        }

        // 2. Bounds-based Calculation
        double left, top, width, height;

        if (_hoveredShape is SpotlightControl sc && sc.Annotation is SpotlightAnnotation sa)
        {
            var b = sa.GetBounds();
            left = b.Left; top = b.Top; width = b.Width; height = b.Height;
        }
        else if (_hoveredShape is StepControl hoveredStep && hoveredStep.Annotation is NumberAnnotation hoveredNumber && hoveredNumber.IsTailVisible())
        {
            var b = hoveredNumber.GetInteractionBounds();
            left = b.Left; top = b.Top; width = b.Width; height = b.Height;
        }
        else
        {
            var hoveredRect = GetLogicalRect(_hoveredShape);
            left = hoveredRect.Left;
            top = hoveredRect.Top;
            width = hoveredRect.Width;
            height = hoveredRect.Height;
        }

        if (width <= 0 || height <= 0) return;

        // 3. Ellipse Outline (for Ellipse, Step/Number, and ellipse effect regions)
        if (_hoveredShape is Ellipse ||
            (_hoveredShape?.Tag is MagnifyAnnotation { IsEllipse: true }) ||
            (_hoveredShape is SpotlightControl { Annotation.IsEllipse: true }) ||
            (_hoveredShape is StepControl hoveredStepControl && hoveredStepControl.Annotation is NumberAnnotation hoveredNumberAnnotation && !hoveredNumberAnnotation.IsTailVisible()))
        {
            if (_hoverEllipseBlack == null)
            {
                _hoverEllipseBlack = new Ellipse
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    StrokeDashArray = new global::Avalonia.Collections.AvaloniaList<double> { 3, 3 },
                    IsHitTestVisible = false
                };
                overlay.Children.Add(_hoverEllipseBlack);
            }
            if (_hoverEllipseWhite == null)
            {
                _hoverEllipseWhite = new Ellipse
                {
                    Stroke = Brushes.White,
                    StrokeThickness = 1,
                    StrokeDashArray = new global::Avalonia.Collections.AvaloniaList<double> { 3, 3 },
                    StrokeDashOffset = 3,
                    IsHitTestVisible = false
                };
                overlay.Children.Add(_hoverEllipseWhite);
            }

            Canvas.SetLeft(_hoverEllipseBlack, ToOverlayCoordinate(left - 2));
            Canvas.SetTop(_hoverEllipseBlack, ToOverlayCoordinate(top - 2));
            _hoverEllipseBlack.Width = width + 4;
            _hoverEllipseBlack.Height = height + 4;

            Canvas.SetLeft(_hoverEllipseWhite, ToOverlayCoordinate(left - 2));
            Canvas.SetTop(_hoverEllipseWhite, ToOverlayCoordinate(top - 2));
            _hoverEllipseWhite.Width = width + 4;
            _hoverEllipseWhite.Height = height + 4;
            return;
        }

        // 4. Rectangle Outline (Default)
        if (_hoverOutlineBlack == null)
        {
            _hoverOutlineBlack = new global::Avalonia.Controls.Shapes.Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                StrokeDashArray = new global::Avalonia.Collections.AvaloniaList<double> { 3, 3 },
                Fill = null,
                IsHitTestVisible = false
            };
            overlay.Children.Add(_hoverOutlineBlack);
        }

        if (_hoverOutlineWhite == null)
        {
            _hoverOutlineWhite = new global::Avalonia.Controls.Shapes.Rectangle
            {
                Stroke = Brushes.White,
                StrokeThickness = 1,
                StrokeDashArray = new global::Avalonia.Collections.AvaloniaList<double> { 3, 3 },
                StrokeDashOffset = 3,
                Fill = null,
                IsHitTestVisible = false
            };
            overlay.Children.Add(_hoverOutlineWhite);
        }

        Canvas.SetLeft(_hoverOutlineBlack, ToOverlayCoordinate(left - 2));
        Canvas.SetTop(_hoverOutlineBlack, ToOverlayCoordinate(top - 2));
        _hoverOutlineBlack.Width = width + 4;
        _hoverOutlineBlack.Height = height + 4;

        Canvas.SetLeft(_hoverOutlineWhite, ToOverlayCoordinate(left - 2));
        Canvas.SetTop(_hoverOutlineWhite, ToOverlayCoordinate(top - 2));
        _hoverOutlineWhite.Width = width + 4;
        _hoverOutlineWhite.Height = height + 4;

        if (TryGetRotatableAnnotation(_hoveredShape, out Annotation? rotatedAnnotation)
            && rotatedAnnotation != null
            && rotatedAnnotation.RotationAngle != 0)
        {
            var rotTransform = new RotateTransform(rotatedAnnotation.RotationAngle);
            // Rotate around the center of the outline (which matches the shape center)
            var originX = (width + 4) > 0 ? ((left + width / 2) - (left - 2)) / (width + 4) : 0.5;
            var originY = (height + 4) > 0 ? ((top + height / 2) - (top - 2)) / (height + 4) : 0.5;
            var origin = new RelativePoint(originX, originY, RelativeUnit.Relative);

            _hoverOutlineBlack.RenderTransformOrigin = origin;
            _hoverOutlineBlack.RenderTransform = rotTransform;
            _hoverOutlineWhite.RenderTransformOrigin = origin;
            _hoverOutlineWhite.RenderTransform = rotTransform;
        }
        else
        {
            _hoverOutlineBlack.RenderTransform = null;
            _hoverOutlineWhite.RenderTransform = null;
        }
    }

    private void AttachTextBoxEditHandlers(TextBox tb)
    {
        EventHandler<FocusChangedEventArgs>? lostFocusHandler = null;
        EventHandler<KeyEventArgs>? keyDownHandler = null;
        EventHandler<KeyEventArgs>? keyUpHandler = null;

        lostFocusHandler = (s, args) =>
        {
            if (lostFocusHandler != null) tb.LostFocus -= lostFocusHandler;
            if (keyDownHandler != null) tb.KeyDown -= keyDownHandler;
            if (keyUpHandler != null) tb.KeyUp -= keyUpHandler;

            tb.IsHitTestVisible = false;

            if (tb.Tag is Annotation annotation)
            {
                // Sync Text
                if (annotation is TextAnnotation textAnn)
                {
                    textAnn.Text = tb.Text ?? string.Empty;

                    // Sync Bounds
                    var textBoxRect = GetLogicalRect(tb);
                    textAnn.EndPoint = new SKPoint(
                        (float)(textBoxRect.Left + textBoxRect.Width),
                        (float)(textBoxRect.Top + textBoxRect.Height)
                    );

                    UpdateSelectionHandles();
                    UpdateHoverOutline();
                }
            }
        };

        keyDownHandler = (s, args) =>
        {
            if (args.Key == Key.Enter && args.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                args.Handled = true;
                int caretIndex = tb.CaretIndex;
                string currentText = tb.Text ?? string.Empty;
                tb.Text = currentText.Substring(0, caretIndex) + "\n" + currentText.Substring(caretIndex);
                tb.CaretIndex = caretIndex + 1;
            }
        };

        keyUpHandler = (s, args) =>
        {
            if ((args.Key == Key.Enter && !args.KeyModifiers.HasFlag(KeyModifiers.Control)) || args.Key == Key.Escape)
            {
                args.Handled = true;
                _view.Focus();
            }
        };

        tb.LostFocus += lostFocusHandler;
        tb.KeyDown += keyDownHandler;
        tb.KeyUp += keyUpHandler;
    }

    private static SKPoint ToSKPoint(Point point) => new((float)point.X, (float)point.Y);

    /// <summary>
    /// Returns the EditorTool that created the given control, or null if not determinable.
    /// </summary>
    private static EditorTool? GetControlToolType(Control control)
    {
        if (control is OutlinedTextControl otc) return otc.Annotation?.ToolType;
        if (control is SpeechBalloonControl sbc) return sbc.Annotation?.ToolType;
        if (control is SpotlightControl sc) return sc.Annotation?.ToolType;
        if (control is StepControl stc) return stc.Annotation?.ToolType;
        if (control.Tag is Annotation ann) return ann.ToolType;
        return null;
    }
    public void UpdateActiveTextEditorProperties()
    {
        if (_balloonTextEditor == null || !(_selectedShape is SpeechBalloonControl balloonControl)) return;
        if (balloonControl.Annotation is not SpeechBalloonAnnotation annotation) return;

        // Update Font Size
        _balloonTextEditor.FontSize = annotation.FontSize;
        _balloonTextEditor.FontFamily = new Avalonia.Media.FontFamily(string.IsNullOrWhiteSpace(annotation.FontFamily) ? "Segoe UI" : annotation.FontFamily);
        _balloonTextEditor.FontWeight = annotation.IsBold ? FontWeight.Bold : FontWeight.Normal;
        _balloonTextEditor.FontStyle = annotation.IsItalic ? FontStyle.Italic : FontStyle.Normal;
        _balloonTextEditor.TextAlignment = TextHorizontalAlignmentHelper.ToAvaloniaTextAlignment(annotation.HorizontalAlignment);
        _balloonTextEditor.HorizontalContentAlignment = TextHorizontalAlignmentHelper.ToHorizontalContentAlignment(annotation.HorizontalAlignment);

        // Re-execute color logic (matching ShowSpeechBalloonTextEditor logic)
        IBrush foregroundBrush = new SolidColorBrush(Avalonia.Media.Color.Parse(annotation.StrokeColor));

        try
        {
            var foreColor = Avalonia.Media.Color.Parse(annotation.StrokeColor);
            var backColor = Avalonia.Media.Color.Parse(annotation.FillColor);
            double foreLum = (0.299 * foreColor.R + 0.587 * foreColor.G + 0.114 * foreColor.B) / 255.0;
            double backLum = (0.299 * backColor.R + 0.587 * backColor.G + 0.114 * backColor.B) / 255.0;
            double backAlpha = backColor.A / 255.0;

            if (backAlpha > 0.1)
            {
                if (Math.Abs(foreLum - backLum) < 0.3)
                {
                    foregroundBrush = backLum > 0.5 ? Avalonia.Media.Brushes.Black : Avalonia.Media.Brushes.White;
                }
            }
        }
        catch { }

        IBrush editorBackground;
        var fillColor = Avalonia.Media.Color.Parse(annotation.FillColor);
        if (fillColor.A < 20)
        {
            var fgBrush = foregroundBrush as SolidColorBrush;
            var fgColor = fgBrush?.Color ?? Avalonia.Media.Colors.Black;
            editorBackground = fgColor.R > 127
                ? new SolidColorBrush(Avalonia.Media.Color.Parse("#AA000000"))
                : new SolidColorBrush(Avalonia.Media.Color.Parse("#AAFFFFFF"));
        }
        else
        {
            editorBackground = new SolidColorBrush(fillColor);
        }

        _balloonTextEditor.Background = editorBackground;
        _balloonTextEditor.Foreground = foregroundBrush;

        // Update resource overrides for Focus state
        _balloonTextEditor.Resources["TextControlBackground"] = editorBackground;
        _balloonTextEditor.Resources["TextControlBackgroundFocused"] = editorBackground;
        _balloonTextEditor.Resources["TextControlBackgroundPointerOver"] = editorBackground;

        _balloonTextEditor.Resources["TextControlBorderThemeThickness"] = new Thickness(0);
        _balloonTextEditor.Resources["TextControlBorderThemeThicknessFocused"] = new Thickness(0);
        _balloonTextEditor.Resources["TextControlBorderThemeThicknessPointerOver"] = new Thickness(0);
        _balloonTextEditor.Resources["TextControlBorderBrush"] = Avalonia.Media.Brushes.Transparent;
        _balloonTextEditor.Resources["TextControlBorderBrushFocused"] = Avalonia.Media.Brushes.Transparent;
        _balloonTextEditor.Resources["TextControlBorderBrushPointerOver"] = Avalonia.Media.Brushes.Transparent;

        var bodyBounds = annotation.GetBounds();
        ApplySpeechBalloonTextEditorLayout(
            _balloonTextEditor,
            annotation,
            bodyBounds.Left,
            bodyBounds.Top,
            bodyBounds.Width,
            bodyBounds.Height);
    }

    private static void ApplySpeechBalloonTextEditorLayout(TextBox textBox, SpeechBalloonAnnotation annotation, double left, double top, double width, double height)
    {
        Canvas.SetLeft(textBox, ToOverlayCoordinate(left));
        Canvas.SetTop(textBox, ToOverlayCoordinate(top));
        textBox.Width = width;
        textBox.Height = height;

        if (annotation.RotationAngle != 0)
        {
            textBox.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
            textBox.RenderTransform = new RotateTransform(annotation.RotationAngle);
        }
        else
        {
            textBox.RenderTransform = null;
        }
    }

    private void ShowTextEditor(OutlinedTextControl textControl, Canvas canvas)
    {
        if (textControl.Annotation is not TextAnnotation annotation) return;

        var overlay = _view.FindControl<Canvas>("OverlayCanvas");
        if (overlay == null) return;

        // Hide the original control while editing
        textControl.IsVisible = false;

        // Create a temporary TextBox for editing
        string textColor = annotation.TextColor;
        if (string.IsNullOrEmpty(textColor) || textColor == "#00000000")
        {
            // Fallback to black for editing if text is transparent
            // Only if stroke isn't set. Actually if TextColor is transparent but StrokeColor is set, text is stroke-only.
            // Using StrokeColor for the editor text if TextColor is transparent makes sense so it's visible.
            string strokeColor = annotation.StrokeColor;
            if (string.IsNullOrEmpty(strokeColor) || strokeColor == "#00000000")
            {
                textColor = "#FF000000";
            }
            else
            {
                textColor = strokeColor;
            }
        }

        var textBox = new TextBox
        {
            Text = annotation.Text,
            Foreground = new SolidColorBrush(Avalonia.Media.Color.Parse(textColor)),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Gray,
            FontSize = annotation.FontSize,
            FontFamily = new Avalonia.Media.FontFamily(string.IsNullOrWhiteSpace(annotation.FontFamily) ? "Segoe UI" : annotation.FontFamily),
            FontWeight = annotation.IsBold ? FontWeight.Bold : FontWeight.Normal,
            FontStyle = annotation.IsItalic ? FontStyle.Italic : FontStyle.Normal,
            Padding = new Thickness(4),
            AcceptsReturn = false,
            TextAlignment = TextHorizontalAlignmentHelper.ToAvaloniaTextAlignment(annotation.HorizontalAlignment),
            HorizontalContentAlignment = TextHorizontalAlignmentHelper.ToHorizontalContentAlignment(annotation.HorizontalAlignment),
            VerticalContentAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            MinWidth = 20,
            Tag = annotation
        };

        // Force Avalonia's internal text box states to be transparent
        textBox.Resources["TextControlBackground"] = Brushes.Transparent;
        textBox.Resources["TextControlBackgroundFocused"] = Brushes.Transparent;
        textBox.Resources["TextControlBackgroundPointerOver"] = Brushes.Transparent;

        // Apply rotation to make editing match display
        if (annotation.RotationAngle != 0)
        {
            textBox.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
            textBox.RenderTransform = new RotateTransform(annotation.RotationAngle);
        }

        var annotationBounds = annotation.GetBounds();
        Canvas.SetLeft(textBox, ToOverlayCoordinate(annotationBounds.Left));
        Canvas.SetTop(textBox, ToOverlayCoordinate(annotationBounds.Top));
        textBox.Width = Math.Max(20, annotationBounds.Width);
        textBox.Height = Math.Max(20, annotationBounds.Height);

        EventHandler<FocusChangedEventArgs>? lostFocusHandler = null;
        EventHandler<KeyEventArgs>? keyDownHandler = null;
        EventHandler<KeyEventArgs>? keyUpHandler = null;

        void CompleteEditing()
        {
            if (lostFocusHandler != null) textBox.LostFocus -= lostFocusHandler;
            if (keyDownHandler != null) textBox.KeyDown -= keyDownHandler;
            if (keyUpHandler != null) textBox.KeyUp -= keyUpHandler;

            annotation.Text = textBox.Text ?? string.Empty;

            // Remove from overlay
            overlay.Children.Remove(textBox);

            // Keep the existing annotation rectangle so wrapped text stays inside the resized bounds.
            textControl.IsVisible = true;
            textControl.InvalidateMeasure();
            textControl.InvalidateVisual();

            // Fire RequestUpdateEffect to save new state if needed
            RequestUpdateEffect?.Invoke(textControl);

            UpdateSelectionHandles();
            UpdateHoverOutline();
        }

        lostFocusHandler = (s, args) => CompleteEditing();

        keyDownHandler = (s, args) =>
        {
            if (args.Key == Key.Enter && args.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                args.Handled = true;
                int caretIndex = textBox.CaretIndex;
                string currentText = textBox.Text ?? string.Empty;
                textBox.Text = currentText.Substring(0, caretIndex) + "\n" + currentText.Substring(caretIndex);
                textBox.CaretIndex = caretIndex + 1;
            }
        };

        keyUpHandler = (s, args) =>
        {
            if ((args.Key == Key.Enter && !args.KeyModifiers.HasFlag(KeyModifiers.Control)) || args.Key == Key.Escape)
            {
                args.Handled = true;
                _view.Focus();
            }
        };

        textBox.LostFocus += lostFocusHandler;
        textBox.KeyDown += keyDownHandler;
        textBox.KeyUp += keyUpHandler;

        overlay.Children.Add(textBox);
        textBox.Focus();
        textBox.CaretIndex = textBox.Text?.Length ?? 0;
        textBox.SelectionStart = textBox.CaretIndex;
        textBox.SelectionEnd = textBox.CaretIndex;
    }

    private void UpdateBoundsObserver()
    {
        if (_observedShape != null && _boundsHandler != null)
        {
            _observedShape.PropertyChanged -= _boundsHandler;
            _observedShape = null;
            _boundsHandler = null;
        }

        if (_selectedShape is OutlinedTextControl tb && tb.Tag is TextAnnotation textAnn)
        {
            _observedShape = tb;
            _boundsHandler = (s, args) =>
            {
                if (args.Property == Visual.BoundsProperty)
                {
                    var textRect = GetLogicalRect(tb);
                    // Handle valid size (ignore 0x0 or uninitialized)
                    if (textRect.Width > 0 && textRect.Height > 0)
                    {
                        var left = textRect.Left;
                        var top = textRect.Top;

                        // Sync visual bounds to annotation model
                        // This ensures hit tests (which rely on annotation start/end points) are accurate
                        textAnn.StartPoint = new SKPoint((float)left, (float)top);
                        textAnn.EndPoint = new SKPoint((float)(left + textRect.Width), (float)(top + textRect.Height));

                        // Refresh selection handles to match new bounds
                        UpdateSelectionHandles();
                    }
                }
            };
            _observedShape.PropertyChanged += _boundsHandler;
        }
    }

    private static Rect GetLogicalRect(Control control)
    {
        if (control is SpotlightControl spotlight && spotlight.Annotation is SpotlightAnnotation spotlightAnnotation)
        {
            return ToRect(spotlightAnnotation.GetBounds());
        }

        if (control is SpeechBalloonControl balloonControl && balloonControl.Annotation is SpeechBalloonAnnotation balloonAnnotation)
        {
            return ToRect(balloonAnnotation.GetBounds());
        }

        if (control.Tag is Annotation annotation)
        {
            var annotationRect = ToRect(annotation.GetBounds());
            if (annotationRect.Width > 0 && annotationRect.Height > 0)
            {
                return annotationRect;
            }
        }

        var left = Canvas.GetLeft(control);
        var top = Canvas.GetTop(control);
        if (double.IsNaN(left)) left = 0;
        if (double.IsNaN(top)) top = 0;

        if (control.Parent is Canvas parent && parent.Name == "OverlayCanvas")
        {
            left -= EditorView.OverlayCanvasBleed;
            top -= EditorView.OverlayCanvasBleed;
        }

        var width = control.Width;
        var height = control.Height;

        if (double.IsNaN(width) || width <= 0) width = control.DesiredSize.Width;
        if (double.IsNaN(height) || height <= 0) height = control.DesiredSize.Height;

        // Last-resort fallback for controls that have not been initialized with explicit size.
        if (width <= 0 || height <= 0)
        {
            width = control.Bounds.Width;
            height = control.Bounds.Height;
        }

        if (double.IsNaN(width) || width < 0) width = 0;
        if (double.IsNaN(height) || height < 0) height = 0;

        return new Rect(left, top, width, height);
    }

    private static Rect ToRect(SKRect rect)
        => new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
}
