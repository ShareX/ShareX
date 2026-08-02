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
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.Controls;
using ShareX.ImageEditor.Presentation.Helpers;
using ShareX.ImageEditor.Presentation.Rendering;
using ShareX.ImageEditor.Presentation.ViewModels;
using ShareX.ImageEditor.Presentation.Views;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Controllers;

public class EditorInputController
{
    private readonly EditorView _view;
    private readonly EditorSelectionController _selectionController;
    private readonly EditorZoomController _zoomController;

    // Minimum shape size to prevent accidental creation of tiny shapes
    private const double MinShapeSize = 5;

    private Point _startPoint;
    private Point _lastDrawPoint;
    private Control? _currentShape;
    private bool _isDrawing;
    private bool _isCreatingEffect;
    private bool _isMovingShapeDuringCreation;
    private bool _wasCtrlHeldDuringDraw;

    private static bool UsesCrosshairInteractionCapture(EditorTool tool)
        => tool != EditorTool.Select && tool != EditorTool.Image && tool != EditorTool.Text;

    private static Point ClampPointToCanvasBounds(Canvas canvas, Point point)
        => new(
            Math.Clamp(point.X, 0, canvas.Bounds.Width),
            Math.Clamp(point.Y, 0, canvas.Bounds.Height));

    // Track cut-out direction (null = not determined yet, true = vertical, false = horizontal)
    private bool? _cutOutDirection;

    // Cached SKBitmap for effect updates
    private SKBitmap? _cachedSkBitmap;

    // Crop handle state: after drawing the crop rect, before confirming
    private bool _cropActive;
    private List<Control> _cropHandles = new();
    private bool _isDraggingCropHandle;
    private string? _draggedCropHandleTag;
    private Point _cropDragStartPoint;
    private Rect _cropDragStartRect;
    private readonly List<Rectangle> _cropShadeRects = new();
    private readonly List<Line> _cropGuideLines = new();
    private Button? _cropConfirmButton;

    public EditorInputController(EditorView view, EditorSelectionController selectionController, EditorZoomController zoomController)
    {
        _view = view;
        _selectionController = selectionController;
        _zoomController = zoomController;
    }

    public bool IsDrawingActive => _isDrawing;
    public bool IsCropInteractionActive => _isDraggingCropHandle;

    private MainViewModel? ViewModel => _view.DataContext as MainViewModel;
    private bool IsQuickCropEnabled => ViewModel?.Options.QuickCrop == true;
    private static double ToOverlayCoordinate(double value) => value + EditorView.OverlayCanvasBleed;
    private static double FromOverlayCoordinate(double value) => value - EditorView.OverlayCanvasBleed;
    private static Point ToOverlayPoint(Point value) => new(ToOverlayCoordinate(value.X), ToOverlayCoordinate(value.Y));
    private static bool ShouldClearSelectionOnMouseDrawStart(EditorTool tool)
        => tool != EditorTool.Select && tool != EditorTool.Image && tool != EditorTool.Emoji;

    private static Rect GetCropOverlayCanvasRect(global::Avalonia.Controls.Shapes.Rectangle cropOverlay)
        => new(
            FromOverlayCoordinate(Canvas.GetLeft(cropOverlay)),
            FromOverlayCoordinate(Canvas.GetTop(cropOverlay)),
            cropOverlay.Width,
            cropOverlay.Height);

    public async void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var vm = ViewModel;
        if (vm == null) return;

        var canvas = _view.FindControl<Canvas>("AnnotationCanvas") ?? sender as Canvas;
        if (canvas == null) return;

        var props = e.GetCurrentPoint(canvas).Properties;
        if (props.IsMiddleButtonPressed)
        {
            _zoomController.OnScrollViewerPointerPressed(_view.FindControl<ScrollViewer>("CanvasScrollViewer"), e);
            return;
        }

        if (_isDrawing && props.IsRightButtonPressed && (vm.ActiveTool == EditorTool.Crop || vm.ActiveTool == EditorTool.CutOut))
        {
            CancelActiveRegionDrawing(canvas);
            e.Pointer.Capture(null);
            e.Handled = true;
            return;
        }

        // Right-click cancels an active (drawn but unconfirmed) crop
        if (_cropActive && props.IsRightButtonPressed && vm.ActiveTool == EditorTool.Crop)
        {
            CancelCrop();
            e.Pointer.Capture(null);
            e.Handled = true;
            return;
        }

        if (props.IsRightButtonPressed)
        {
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

            if (hitTarget == null)
            {
                hitTarget = _selectionController.HitTestShape(canvas, e.GetPosition(canvas));
            }

            if (hitTarget != null)
            {
                // Select the shape if not already selected (standard right-click behavior)
                if (_selectionController.SelectedShape != hitTarget)
                {
                    _selectionController.SetSelectedShape(hitTarget);
                }

                _view.OpenContextMenu(hitTarget);
                e.Handled = true;
                return;
            }

            // If clicked on empty space, open context menu on canvas
            _view.OpenContextMenu(canvas);
            e.Handled = true;
            return;
        }

        // When a crop rect is drawn and awaiting confirmation, intercept pointer events
        if (_cropActive)
        {
            var overlayCanvas = _view.FindControl<Canvas>("OverlayCanvas");
            var cropOverlay = _view.FindControl<global::Avalonia.Controls.Shapes.Rectangle>("CropOverlay");

            // Check if a crop resize/move handle was clicked (walk up from source so child Path/Rectangle still counts)
            if (overlayCanvas != null)
            {
                var hitSource = e.Source as Control;
                Control? cropBorder = null;
                string? cropTag = null;
                while (hitSource != null && hitSource != overlayCanvas)
                {
                    if (hitSource.Tag is string tag && tag.StartsWith("Crop_"))
                    {
                        cropBorder = hitSource;
                        cropTag = tag;
                        break;
                    }
                    hitSource = hitSource.GetVisualParent() as Control;
                }
                if (cropBorder != null && cropTag != null && overlayCanvas.Children.Contains(cropBorder) && cropOverlay != null)
                {
                    _cropDragStartRect = GetCropOverlayCanvasRect(cropOverlay);
                    _cropDragStartPoint = e.GetPosition(canvas);
                    _draggedCropHandleTag = cropTag;
                    _isDraggingCropHandle = true;
                    _view.BeginInteractionCursorCapture(e.Pointer, CursorAssetLoader.CustomCursorKind.ClosedHand);
                    e.Handled = true;
                    return;
                }
            }

            if (cropOverlay != null)
            {
                var clickPos = e.GetPosition(canvas);
                var cropBounds = GetCropOverlayCanvasRect(cropOverlay);

                // Double-click inside crop area → confirm
                if (e.ClickCount == 2 && cropBounds.Contains(clickPos))
                {
                    TryConfirmCrop();
                    e.Handled = true;
                    return;
                }

                // Single-click inside crop area → drag/move entire crop rect
                if (cropBounds.Contains(clickPos))
                {
                    _cropDragStartRect = cropBounds;
                    _cropDragStartPoint = clickPos;
                    _draggedCropHandleTag = "Crop_Move";
                    _isDraggingCropHandle = true;
                    _view.BeginInteractionCursorCapture(e.Pointer, CursorAssetLoader.CustomCursorKind.ClosedHand);
                    e.Handled = true;
                    return;
                }
            }

            // Click outside crop area → cancel and fall through to start a new crop
            CancelCrop();
        }

        var selectionSender = sender ?? canvas;
        if (_selectionController.OnPointerPressed(selectionSender, e))
        {
            return;
        }

        if (ShouldClearSelectionOnMouseDrawStart(vm.ActiveTool))
        {
            _selectionController.ClearSelection();
        }
        else
        {
            _selectionController.ClearHoverFeedback();
        }

        // ISSUE-019 fix: Dead code removed - redo stack cleared by EditorCore

        var point = e.GetPosition(canvas);
        if (vm.ActiveTool == EditorTool.Crop || vm.ActiveTool == EditorTool.CutOut)
        {
            point = ClampPointToCanvasBounds(canvas, point);
        }

        _startPoint = point;
        _lastDrawPoint = point;
        _isMovingShapeDuringCreation = false;
        _wasCtrlHeldDuringDraw = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        _isDrawing = true;
        if (UsesCrosshairInteractionCapture(vm.ActiveTool))
        {
            _view.BeginInteractionCursorCapture(e.Pointer, CursorAssetLoader.CustomCursorKind.Crosshair);
        }
        else
        {
            e.Pointer.Capture(canvas);
        }

        e.Handled = true;

        var brush = new SolidColorBrush(Color.Parse(vm.SelectedColor));

        if (vm.ActiveTool == EditorTool.Crop)
        {
            if (IsQuickCropEnabled)
            {
                BeginQuickCrop();
            }
            else if (!_cropActive)
            {
                ActivateCropToFullImage();
            }
            return;
        }

        if (vm.ActiveTool == EditorTool.CutOut)
        {
            _cutOutDirection = null;
            var cutOutOverlay = new global::Avalonia.Controls.Shapes.Rectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)),
                Stroke = Brushes.White,
                StrokeThickness = 1,
                StrokeDashArray = new global::Avalonia.Collections.AvaloniaList<double> { 5, 3 },
                Name = "CutOutOverlay",
                IsVisible = false
            };
            Canvas.SetLeft(cutOutOverlay, _startPoint.X);
            Canvas.SetTop(cutOutOverlay, _startPoint.Y);
            cutOutOverlay.Width = 0;
            cutOutOverlay.Height = 0;
            canvas.Children.Add(cutOutOverlay);
            _currentShape = cutOutOverlay;
            return;
        }

        if (vm.ActiveTool == EditorTool.Image)
        {
            _isDrawing = false;
            await HandleImageTool(canvas, point);
            return;
        }

        switch (vm.ActiveTool)
        {
            case EditorTool.Rectangle:
                var rectAnnotation = new RectangleAnnotation { StrokeColor = vm.SelectedColor, StrokeWidth = vm.StrokeWidth, BorderStyle = vm.SelectedBorderStyle, FillColor = vm.FillColor, CornerRadius = vm.CornerRadius, StartPoint = ToSKPoint(_startPoint), EndPoint = ToSKPoint(_startPoint) };
                ApplyShadowOptions(rectAnnotation, vm);
                _currentShape = rectAnnotation.CreateVisual();
                break;
            case EditorTool.Ellipse:
                var ellipseAnnotation = new EllipseAnnotation { StrokeColor = vm.SelectedColor, StrokeWidth = vm.StrokeWidth, BorderStyle = vm.SelectedBorderStyle, FillColor = vm.FillColor, StartPoint = ToSKPoint(_startPoint), EndPoint = ToSKPoint(_startPoint) };
                ApplyShadowOptions(ellipseAnnotation, vm);
                _currentShape = ellipseAnnotation.CreateVisual();
                break;
            case EditorTool.Line:
                var lineAnnotation = new LineAnnotation { StrokeColor = vm.SelectedColor, StrokeWidth = vm.StrokeWidth, BorderStyle = vm.SelectedBorderStyle, StartPoint = ToSKPoint(_startPoint), EndPoint = ToSKPoint(_startPoint) };
                ApplyShadowOptions(lineAnnotation, vm);
                _currentShape = lineAnnotation.CreateVisual();
                _currentShape.IsHitTestVisible = false;
                break;
            case EditorTool.Arrow:
                var arrowAnnotation = new ArrowAnnotation { StrokeColor = vm.SelectedColor, StrokeWidth = vm.StrokeWidth, Style = vm.SelectedArrowStyle, StartPoint = ToSKPoint(_startPoint), EndPoint = ToSKPoint(_startPoint) };
                ApplyShadowOptions(arrowAnnotation, vm);
                _currentShape = arrowAnnotation.CreateVisual();
                _currentShape.IsHitTestVisible = false;
                break;
            case EditorTool.Cursor:
                var cursorAnnotation = new CursorAnnotation { CursorType = vm.SelectedCursorType };
                var cursorBitmap = WindowsCursorBitmapRenderer.CreateAnnotationBitmap(vm.SelectedCursorType);

                if (cursorBitmap == null)
                {
                    _isDrawing = false;
                    return;
                }

                cursorAnnotation.SetImage(cursorBitmap);
                cursorAnnotation.StartPoint = ToSKPoint(_startPoint);
                cursorAnnotation.EndPoint = new SKPoint((float)(_startPoint.X + cursorBitmap.Width), (float)(_startPoint.Y + cursorBitmap.Height));

                _currentShape = AnnotationVisualFactory.CreateVisualControl(cursorAnnotation, AnnotationVisualMode.Persisted);
                if (_currentShape == null)
                {
                    _isDrawing = false;
                    return;
                }

                AnnotationVisualFactory.UpdateVisualControl(
                    _currentShape,
                    cursorAnnotation,
                    AnnotationVisualMode.Persisted,
                    canvas.Bounds.Width,
                    canvas.Bounds.Height);
                break;
            case EditorTool.Text:
                HandleTextTool(canvas, brush, vm.StrokeWidth);
                return;
            case EditorTool.Spotlight:
                // Map EffectStrength (0-100) to DarkenOpacity (0-255)
                var opacity = (byte)Math.Clamp(vm.EffectStrength / MainViewModel.GetMaxEffectStrength(EditorTool.Spotlight) * 255, 0, 255);
                var spotlightAnnotation = new SpotlightAnnotation
                {
                    StartPoint = ToSKPoint(_startPoint),
                    EndPoint = ToSKPoint(_startPoint),
                    CanvasSize = ToSKSize(canvas.Bounds.Size),
                    DarkenOpacity = opacity,
                    BlurAmount = vm.SpotlightBlur,
                    IsEllipse = vm.EffectEllipse
                };
                var spotlightControl = spotlightAnnotation.CreateVisual();
                spotlightControl.Width = canvas.Bounds.Width;
                spotlightControl.Height = canvas.Bounds.Height;
                Canvas.SetLeft(spotlightControl, 0);
                Canvas.SetTop(spotlightControl, 0);
                _currentShape = spotlightControl;
                break;
            case EditorTool.Blur:
                _currentShape = new BlurAnnotation { Amount = vm.EffectStrength, StartPoint = ToSKPoint(_startPoint), EndPoint = ToSKPoint(_startPoint) }.CreateVisual();
                _isCreatingEffect = true;
                break;
            case EditorTool.Pixelate:
                _currentShape = new PixelateAnnotation { Amount = vm.EffectStrength, StartPoint = ToSKPoint(_startPoint), EndPoint = ToSKPoint(_startPoint) }.CreateVisual();
                _isCreatingEffect = true;
                break;
            case EditorTool.Magnify:
                _currentShape = new MagnifyAnnotation { Amount = vm.EffectStrength, IsEllipse = vm.EffectEllipse, StartPoint = ToSKPoint(_startPoint), EndPoint = ToSKPoint(_startPoint) }.CreateVisual();
                _isCreatingEffect = true;
                break;
            case EditorTool.Highlight:
                _currentShape = new HighlightAnnotation { FillColor = vm.FillColor, StartPoint = ToSKPoint(_startPoint), EndPoint = ToSKPoint(_startPoint) }.CreateVisual();
                _isCreatingEffect = true;
                break;
            case EditorTool.SpeechBalloon:
                var fillColor = vm.FillColor;
                // Smart default: If user selected transparent fill, default to White or Black based on Stroke contrast
                if (string.IsNullOrEmpty(fillColor) || fillColor == "#00000000" || fillColor == "Transparent")
                {
                    fillColor = IsColorLight(vm.SelectedColor) ? "#FF000000" : "#FFFFFFFF";
                }
                var balloonAnnotation = new SpeechBalloonAnnotation { StrokeColor = vm.SelectedColor, StrokeWidth = vm.StrokeWidth, FillColor = fillColor, TextColor = vm.TextColor, FontSize = vm.FontSize, FontFamily = vm.SelectedFontFamily, IsBold = vm.TextBold, IsItalic = vm.TextItalic, HorizontalAlignment = vm.SelectedTextHorizontalAlignment, CornerRadius = vm.CornerRadius, TailEnabled = vm.SpeechBalloonTail, StartPoint = ToSKPoint(_startPoint), EndPoint = ToSKPoint(_startPoint) };
                ApplyShadowOptions(balloonAnnotation, vm);
                var balloonControl = balloonAnnotation.CreateVisual();
                balloonControl.Width = 0;
                balloonControl.Height = 0;
                Canvas.SetLeft(balloonControl, _startPoint.X);
                Canvas.SetTop(balloonControl, _startPoint.Y);
                _currentShape = balloonControl;
                break;
            case EditorTool.Step:
                var numberAnnotation = new NumberAnnotation
                {
                    StrokeColor = vm.SelectedColor,
                    StrokeWidth = vm.StrokeWidth,
                    FillColor = vm.FillColor,
                    TextColor = vm.TextColor,
                    FontSize = vm.FontSize,
                    IsBold = vm.TextBold,
                    StepType = vm.SelectedStepType,
                    TailEnabled = vm.SpeechBalloonTail,
                    StartPoint = ToSKPoint(_startPoint),
                    Number = vm.NumberCounter
                }; ;
                ApplyShadowOptions(numberAnnotation, vm);

                _currentShape = numberAnnotation.CreateVisual();

                // Center the number on the click point using calculated radius
                var numberRadius = numberAnnotation.Radius;
                Canvas.SetLeft(_currentShape, _startPoint.X - numberRadius);
                Canvas.SetTop(_currentShape, _startPoint.Y - numberRadius);

                vm.NumberCounter++;
                _isDrawing = true; // Keep true so released handler can select it (or we explicitly select it)?
                // Legacy said: "Keep _isDrawing true so it goes through OnCanvasPointerReleased for auto-selection"
                break;
            case EditorTool.SmartEraser:
                var sampledColor = _view.EditorCore.SampleCanvasColor(ToSKPoint(_startPoint)) ?? "#80FF0000";
                var smartEraser = new SmartEraserAnnotation
                {
                    StrokeColor = sampledColor,
                    FillColor = sampledColor,
                    StrokeWidth = 0,
                    StartPoint = ToSKPoint(_startPoint),
                    EndPoint = ToSKPoint(_startPoint)
                };
                _currentShape = smartEraser.CreateVisual();
                _currentShape.IsHitTestVisible = false;
                break;
            case EditorTool.Freehand:
                var path = new global::Avalonia.Controls.Shapes.Path
                {
                    Stroke = brush,
                    StrokeThickness = vm.StrokeWidth,
                    StrokeDashArray = BorderStyleDashHelper.CreateStrokeDashArray(vm.SelectedBorderStyle),
                    StrokeLineCap = BorderStyleDashHelper.CreateStrokeLineCap(vm.SelectedBorderStyle),
                    StrokeJoin = PenLineJoin.Round,
                    UseLayoutRounding = false,
                    IsHitTestVisible = false
                    // Data will be set on move
                };

                path.SetValue(Panel.ZIndexProperty, 1);

                var freehand = new FreehandAnnotation { StrokeColor = vm.SelectedColor, StrokeWidth = vm.StrokeWidth, BorderStyle = vm.SelectedBorderStyle, Points = new List<SKPoint> { ToSKPoint(_startPoint) } };
                ApplyShadowOptions(freehand, vm);
                if (freehand.ShadowEnabled)
                {
                    path.Effect = ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateDropShadow(freehand);
                }

                path.Tag = freehand;
                path.Data = freehand.CreateSmoothedGeometry();
                _currentShape = path;
                break;
        }

        if (_currentShape != null)
        {
            _currentShape.Cursor = _view.GetCrosshairCursor();

            var currentLeft = Canvas.GetLeft(_currentShape);
            var currentTop = Canvas.GetTop(_currentShape);

            // Check for 0 OR NaN (default can be either depending on platform/version)
            bool isPositionUnset = (currentLeft == 0 || double.IsNaN(currentLeft)) &&
                                   (currentTop == 0 || double.IsNaN(currentTop));

            if (isPositionUnset
                && vm.ActiveTool != EditorTool.Spotlight
                && vm.ActiveTool != EditorTool.SpeechBalloon
                && vm.ActiveTool != EditorTool.Line
                && vm.ActiveTool != EditorTool.Arrow
                && vm.ActiveTool != EditorTool.Freehand
                && vm.ActiveTool != EditorTool.Step)
            {
                Canvas.SetLeft(_currentShape, _startPoint.X);
                Canvas.SetTop(_currentShape, _startPoint.Y);
            }

            // HighlightAnnotation visuals are inserted before non-effect shapes so they render
            // below vector shapes (arrows, rectangles, text) by default.
            if (_currentShape.Tag is HighlightAnnotation)
            {
                int insertIdx = canvas.Children.Count;
                for (int i = 0; i < canvas.Children.Count; i++)
                {
                    if (canvas.Children[i] is Control child && child.Tag is Annotation ann && ann is not BaseEffectAnnotation)
                    {
                        insertIdx = i;
                        break;
                    }
                }
                canvas.Children.Insert(insertIdx, _currentShape);
            }
            else
            {
                canvas.Children.Add(_currentShape);
            }

            if (_currentShape.Tag is SpotlightAnnotation)
            {
                _view.RefreshSpotlightOverlay();
            }
            // ISSUE-019 fix: Dead code removed - undo handled by EditorCore
        }
    }

    public void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
    {
        bool wasPanning = _zoomController.IsPanning;
        _zoomController.OnScrollViewerPointerMoved(_view.FindControl<ScrollViewer>("CanvasScrollViewer"), e);

        if (wasPanning && !_zoomController.IsPanning)
        {
            RestoreInteractionCaptureAfterPanning(e.Pointer);
        }

        var selectionSender = sender ?? _view;
        if (!_isDrawing && _selectionController.OnPointerMoved(selectionSender, e))
        {
            return;
        }

        // Handle active crop handle / move drag
        if (_isDraggingCropHandle)
        {
            var cvs = _view.FindControl<Canvas>("AnnotationCanvas") ?? sender as Canvas;
            if (cvs != null)
            {
                _view.ApplyInteractionCursor(CursorAssetLoader.CustomCursorKind.ClosedHand);
                var cropCurrent = e.GetPosition(cvs);
                var newRect = ComputeCropHandleResizedRect(_draggedCropHandleTag!, _cropDragStartPoint, cropCurrent, _cropDragStartRect, cvs.Bounds.Width, cvs.Bounds.Height);
                UpdateCropOverlayBounds(newRect);
            }
            e.Handled = true;
            return;
        }

        if (!_isDrawing || _currentShape == null) return;

        var canvas = _view.FindControl<Canvas>("AnnotationCanvas") ?? sender as Canvas;
        if (canvas == null) return;

        var currentPoint = e.GetPosition(canvas);

        // Clamp current point to canvas bounds for crop and cut-out tools
        var vm = ViewModel;
        if (vm != null && (vm.ActiveTool == EditorTool.Crop || vm.ActiveTool == EditorTool.CutOut))
        {
            // Allow cancelling selection by right-clicking while holding left button
            var props = e.GetCurrentPoint(canvas).Properties;
            if (props.IsRightButtonPressed)
            {
                CancelActiveRegionDrawing(canvas);
                e.Pointer.Capture(null);
                return;
            }

            currentPoint = new Point(
                Math.Max(0, Math.Min(currentPoint.X, canvas.Bounds.Width)),
                Math.Max(0, Math.Min(currentPoint.Y, canvas.Bounds.Height))
            );
        }

        if (vm == null) return;

        bool ctrlHeld = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        if (!ctrlHeld)
        {
            _isMovingShapeDuringCreation = false;
        }
        else if (CanMoveShapeDuringCreation(_currentShape) && (_isMovingShapeDuringCreation || !_wasCtrlHeldDuringDraw))
        {
            MoveShapeDuringCreation(currentPoint);
            _isMovingShapeDuringCreation = true;
            _lastDrawPoint = currentPoint;
            _wasCtrlHeldDuringDraw = true;
            return;
        }

        if (_currentShape is global::Avalonia.Controls.Shapes.Path freehandPath && freehandPath.Tag is FreehandAnnotation freehand)
        {
            freehand.Points.Add(ToSKPoint(currentPoint));
            freehandPath.Data = freehand.CreateSmoothedGeometry();
            freehandPath.InvalidateVisual();
            _lastDrawPoint = currentPoint;
            _wasCtrlHeldDuringDraw = ctrlHeld;
            return;
        }

        if (_currentShape.Name == "CutOutOverlay")
        {
            // Restored from ref\EditorView_master.axaml.cs lines 2024-2075
            // Calculate deltas from start point
            var deltaX = Math.Abs(currentPoint.X - _startPoint.X);
            var deltaY = Math.Abs(currentPoint.Y - _startPoint.Y);

            const double directionThreshold = 15;

            // ISSUE-014 fix: Always show overlay to provide immediate visual feedback
            _currentShape.IsVisible = true;

            // Determine direction based on current movement
            bool currentIsVertical = deltaX > deltaY;

            // Below threshold: show preview feedback (small indicator at start point)
            if (deltaX < directionThreshold && deltaY < directionThreshold)
            {
                _cutOutDirection = null;

                // Show small preview square at start point (30x30px) to indicate tool is active
                const double previewSize = 30;
                Canvas.SetLeft(_currentShape, _startPoint.X - previewSize / 2);
                Canvas.SetTop(_currentShape, _startPoint.Y - previewSize / 2);
                _currentShape.Width = previewSize;
                _currentShape.Height = previewSize;
                return;
            }

            // Update direction once threshold exceeded (can change if user changes drag direction)
            if (deltaX > directionThreshold || deltaY > directionThreshold)
            {
                _cutOutDirection = currentIsVertical;
            }

            // Show and position the cut-out overlay rectangle in determined direction
            if (_cutOutDirection.HasValue)
            {
                if (_cutOutDirection.Value)
                {
                    // Vertical cut - show full-height rectangle between start and current X
                    var cutLeft = Math.Min(_startPoint.X, currentPoint.X);
                    var cutWidth = Math.Abs(currentPoint.X - _startPoint.X);

                    Canvas.SetLeft(_currentShape, cutLeft);
                    Canvas.SetTop(_currentShape, 0); // Full height from top
                    _currentShape.Width = cutWidth;
                    _currentShape.Height = canvas.Bounds.Height; // Full canvas height
                }
                else
                {
                    // Horizontal cut - show full-width rectangle between start and current Y
                    var cutTop = Math.Min(_startPoint.Y, currentPoint.Y);
                    var cutHeight = Math.Abs(currentPoint.Y - _startPoint.Y);

                    Canvas.SetLeft(_currentShape, 0); // Full width from left
                    Canvas.SetTop(_currentShape, cutTop);
                    _currentShape.Width = canvas.Bounds.Width; // Full canvas width
                    _currentShape.Height = cutHeight;
                }
            }
            return;
        }

        if (vm.ActiveTool == EditorTool.Crop && _currentShape.Name == "CropOverlay")
        {
            var cropLeft = Math.Min(_startPoint.X, currentPoint.X);
            var cropTop = Math.Min(_startPoint.Y, currentPoint.Y);
            var cropWidth = Math.Abs(currentPoint.X - _startPoint.X);
            var cropHeight = Math.Abs(currentPoint.Y - _startPoint.Y);

            if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                cropWidth = Math.Max(cropWidth, cropHeight);
                cropHeight = cropWidth;
                if (currentPoint.X < _startPoint.X) cropLeft = _startPoint.X - cropWidth;
                if (currentPoint.Y < _startPoint.Y) cropTop = _startPoint.Y - cropHeight;
            }

            UpdateQuickCropPreview(new Rect(cropLeft, cropTop, cropWidth, cropHeight));
            return;
        }

        // Standard shape resizing
        var left = Math.Min(_startPoint.X, currentPoint.X);
        var top = Math.Min(_startPoint.Y, currentPoint.Y);
        var width = Math.Abs(currentPoint.X - _startPoint.X);
        var height = Math.Abs(currentPoint.Y - _startPoint.Y);

        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            width = Math.Max(width, height);
            height = width;
            if (currentPoint.X < _startPoint.X) left = _startPoint.X - width;
            if (currentPoint.Y < _startPoint.Y) top = _startPoint.Y - height;
        }

        if (_currentShape is global::Avalonia.Controls.Shapes.Rectangle || _currentShape is global::Avalonia.Controls.Shapes.Ellipse)
        {
            Canvas.SetLeft(_currentShape, left);
            Canvas.SetTop(_currentShape, top);
            _currentShape.Width = width;
            _currentShape.Height = height;

            UpdateEffectVisual(_currentShape, left, top, width, height);

            if (_currentShape.Tag is RectangleAnnotation rectAnn) { rectAnn.StartPoint = ToSKPoint(new Point(left, top)); rectAnn.EndPoint = ToSKPoint(new Point(left + width, top + height)); }
            else if (_currentShape.Tag is EllipseAnnotation ellAnn) { ellAnn.StartPoint = ToSKPoint(new Point(left, top)); ellAnn.EndPoint = ToSKPoint(new Point(left + width, top + height)); }
            else if (_currentShape.Tag is BaseEffectAnnotation effectAnn) { effectAnn.StartPoint = ToSKPoint(new Point(left, top)); effectAnn.EndPoint = ToSKPoint(new Point(left + width, top + height)); }
        }
        else if (_currentShape is global::Avalonia.Controls.Shapes.Path linePath && linePath.Tag is LineAnnotation lineAnn)
        {
            var lineEnd = e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                ? SnapTo45Degrees(_startPoint, currentPoint)
                : currentPoint;
            lineAnn.EndPoint = ToSKPoint(lineEnd);
            AnnotationVisualFactory.UpdateVisualControl(linePath, lineAnn);
        }
        else if (_currentShape is global::Avalonia.Controls.Shapes.Path arrowPath && arrowPath.Tag is ArrowAnnotation arrowAnn)
        {
            var arrowEnd = e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                ? SnapTo45Degrees(_startPoint, currentPoint)
                : currentPoint;
            arrowAnn.EndPoint = ToSKPoint(arrowEnd);
            AnnotationVisualFactory.UpdateVisualControl(arrowPath, arrowAnn);
        }
        else if (_currentShape is ShareX.ImageEditor.Presentation.Controls.SpotlightControl spotlight)
        {
            if (spotlight.Annotation is SpotlightAnnotation spotAnn)
            {
                spotAnn.StartPoint = ToSKPoint(new Point(left, top));
                spotAnn.EndPoint = ToSKPoint(new Point(left + width, top + height));
                _view.RefreshSpotlightOverlay();
            }
        }
        else if (_currentShape is SpeechBalloonControl balloon)
        {
            if (balloon.Annotation is SpeechBalloonAnnotation balloonAnn)
            {
                balloonAnn.StartPoint = ToSKPoint(new Point(left, top));
                balloonAnn.EndPoint = ToSKPoint(new Point(left + width, top + height));
                AnnotationVisualFactory.UpdateVisualControl(
                    balloon,
                    balloonAnn,
                    AnnotationVisualMode.Persisted,
                    _view.EditorCore.CanvasSize.Width,
                    _view.EditorCore.CanvasSize.Height);
            }
        }
        else if (_currentShape is Image imageControl && _currentShape.Tag is CursorAnnotation cursorAnnotation)
        {
            double cursorWidth = Math.Max(1, imageControl.Width);
            double cursorHeight = Math.Max(1, imageControl.Height);

            Canvas.SetLeft(imageControl, currentPoint.X);
            Canvas.SetTop(imageControl, currentPoint.Y);

            cursorAnnotation.StartPoint = ToSKPoint(currentPoint);
            cursorAnnotation.EndPoint = ToSKPoint(new Point(currentPoint.X + cursorWidth, currentPoint.Y + cursorHeight));
        }
        else if (_currentShape is StepControl && _currentShape.Tag is NumberAnnotation numberAnn)
        {
            // Allow dragging the step shape immediately after inserting it
            var numberRadius = numberAnn.Radius;
            Canvas.SetLeft(_currentShape, currentPoint.X - numberRadius);
            Canvas.SetTop(_currentShape, currentPoint.Y - numberRadius);
            numberAnn.StartPoint = ToSKPoint(currentPoint);
        }

        _lastDrawPoint = currentPoint;
        _wasCtrlHeldDuringDraw = ctrlHeld;
    }

    public void OnCanvasPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        bool wasPanning = _zoomController.IsPanning;
        _zoomController.OnScrollViewerPointerReleased(_view.FindControl<ScrollViewer>("CanvasScrollViewer"), e);

        if (wasPanning && !_zoomController.IsPanning)
        {
            RestoreInteractionCaptureAfterPanning(e.Pointer);
            return;
        }

        var selectionSender = sender ?? _view;
        if (_selectionController.OnPointerReleased(selectionSender, e)) return;

        // Stop crop handle / move drag
        if (_isDraggingCropHandle)
        {
            _isDraggingCropHandle = false;
            _draggedCropHandleTag = null;
            _view.RestoreEditorSurfaceCursorForActiveTool();
            e.Pointer.Capture(null);
            return;
        }

        if (_isDrawing)
        {
            var canvas = _view.FindControl<Canvas>("AnnotationCanvas") ?? sender as Canvas;
            if (canvas == null) return;

            e.Pointer.Capture(null);
            _isDrawing = false;
            _isMovingShapeDuringCreation = false;
            _wasCtrlHeldDuringDraw = false;
            _view.RestoreEditorSurfaceCursorForActiveTool();

            var vm = ViewModel;
            if (vm != null)
            {
                if (_currentShape != null)
                {
                    _view.SyncAnnotationCursor(_currentShape);
                }

                if (vm.ActiveTool == EditorTool.Crop)
                {
                    var cropOverlay = _view.FindControl<global::Avalonia.Controls.Shapes.Rectangle>("CropOverlay");
                    if (cropOverlay != null && cropOverlay.IsVisible && cropOverlay.Width >= MinShapeSize && cropOverlay.Height >= MinShapeSize)
                    {
                        if (IsQuickCropEnabled)
                        {
                            HideCropChrome();
                            PerformCrop();
                        }
                        else
                        {
                            var overlayCanvas = _view.FindControl<Canvas>("OverlayCanvas");
                            if (overlayCanvas != null)
                            {
                                var cropRect = GetCropOverlayCanvasRect(cropOverlay);
                                ShowCropHandles(overlayCanvas, cropRect);
                                _cropActive = true;
                            }
                        }
                    }
                    else if (cropOverlay != null)
                    {
                        CancelCrop();
                    }
                    _currentShape = null;
                    return;
                }
                else if (vm.ActiveTool == EditorTool.CutOut)
                {
                    PerformCutOut(canvas);
                    return;
                }
                else if (_currentShape != null)
                {
                    // Check MinSize for shapes that support size validation
                    // Skip check for Number (single-click), Pen, and Text.
                    bool isSizeBased = vm.ActiveTool != EditorTool.Step
                                    && vm.ActiveTool != EditorTool.Freehand
                                    && vm.ActiveTool != EditorTool.Text
                                    && vm.ActiveTool != EditorTool.Cursor;

                    if (isSizeBased)
                    {
                        // Calculate size based on pointer position difference (most reliable method)
                        var releasePoint = e.GetPosition(canvas);
                        double shapeWidth = Math.Abs(releasePoint.X - _startPoint.X);
                        double shapeHeight = Math.Abs(releasePoint.Y - _startPoint.Y);

                        // Discard shape if too small (prevents accidental clicks creating tiny shapes)
                        if (shapeWidth < MinShapeSize && shapeHeight < MinShapeSize)
                        {
                            bool wasSpotlight = _currentShape?.Tag is SpotlightAnnotation;
                            if (_currentShape != null)
                            {
                                canvas.Children.Remove(_currentShape);
                            }
                            _currentShape = null;
                            _cachedSkBitmap?.Dispose();
                            _cachedSkBitmap = null;
                            _isCreatingEffect = false;
                            if (wasSpotlight)
                            {
                                _view.RefreshSpotlightOverlay();
                            }
                            _view.ClearInteractiveEffectPreviewCache();
                            return;
                        }
                    }

                    // Restored from ref\EditorView_master.axaml.cs lines 2211-2238
                    // Auto-select newly created shape so resize handles appear immediately,
                    // but skip freehand paths which are not resizable with our current
                    // handle logic.
                    if (!(_currentShape is global::Avalonia.Controls.Shapes.Path && _currentShape.Tag is FreehandAnnotation))
                    {
                        // Apply final effect for effect tools
                        if (_currentShape.Tag is BaseEffectAnnotation)
                        {
                            UpdateEffectVisual(_currentShape,
                                Canvas.GetLeft(_currentShape),
                                Canvas.GetTop(_currentShape),
                                _currentShape.Width,
                                _currentShape.Height);
                        }

                        // Restore hit testing for the finalized shape (was disabled for performance during drawing)
                        _currentShape.IsHitTestVisible = true;

                        _selectionController.SetSelectedShape(_currentShape);
                    }

                    // Ensure annotation is added to Core history
                    if (_currentShape.Tag is Annotation annotation && vm.ActiveTool != EditorTool.Crop && vm.ActiveTool != EditorTool.CutOut)
                    {
                        _view.EditorCore.AddAnnotation(annotation);

                        // Update HasAnnotations state for Clear button
                        vm.HasAnnotations = true;
                    }
                }
            }

            _currentShape = null;
            _cachedSkBitmap?.Dispose();
            _cachedSkBitmap = null;
            _isCreatingEffect = false;
            _view.ClearInteractiveEffectPreviewCache();
        }
    }

    public void OnKeyDown(object sender, KeyEventArgs e)
    {
        // Do not re-arm shape movement here. Ctrl KeyDown events can repeat while
        // the key remains held, so movement must only be re-armed by OnKeyUp.
    }

    public void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (!_isDrawing || e.Key is not Key.LeftCtrl and not Key.RightCtrl)
        {
            return;
        }

        _isMovingShapeDuringCreation = false;
        _wasCtrlHeldDuringDraw = false;
    }

    private void CancelActiveRegionDrawing(Canvas canvas)
    {
        if (_cropActive) CancelCrop();
        if (_currentShape is global::Avalonia.Controls.Shapes.Rectangle rect)
        {
            if (rect.Name == "CropOverlay")
            {
                HideCropChrome();
                rect.IsVisible = false;
                rect.Width = 0;
                rect.Height = 0;
            }
            else if (rect.Name == "CutOutOverlay") { canvas.Children.Remove(rect); }
        }
        _currentShape = null;
        _cutOutDirection = null;
        _isDrawing = false;
        _isMovingShapeDuringCreation = false;
        _wasCtrlHeldDuringDraw = false;
        _view.RestoreEditorSurfaceCursorForActiveTool();
    }

    private void RestoreInteractionCaptureAfterPanning(IPointer pointer)
    {
        if (_isDraggingCropHandle)
        {
            _view.BeginInteractionCursorCapture(pointer, CursorAssetLoader.CustomCursorKind.ClosedHand);
            return;
        }

        if (_isDrawing)
        {
            var vm = ViewModel;
            if (vm != null && UsesCrosshairInteractionCapture(vm.ActiveTool))
            {
                _view.BeginInteractionCursorCapture(pointer, CursorAssetLoader.CustomCursorKind.Crosshair);
            }
        }
    }

    private void UpdateEffectVisual(Control shape, double x, double y, double width, double height)
    {
        // ISSUE-004 fix: Store ViewModel locally to prevent null reference if it changes
        var vm = ViewModel;
        if (!_isCreatingEffect || vm == null) return;
        if (vm.PreviewImage == null || shape.Tag is not BaseEffectAnnotation) return;

        if (_cachedSkBitmap == null)
        {
            _cachedSkBitmap = BitmapConversionHelpers.ToSKBitmap(vm.PreviewImage);
        }

        if (width <= 0 || height <= 0) return;

        try
        {
            if (_cachedSkBitmap != null)
            {
                _view.UpdateInteractiveEffectVisual(shape, _cachedSkBitmap, new Rect(x, y, width, height));
            }
        }
        catch { }
    }

    /// <summary>
    /// Executes the pending crop operation. Returns true if a crop was confirmed.
    /// </summary>
    public bool TryConfirmCrop()
    {
        if (!_cropActive) return false;
        HideCropChrome();
        PerformCrop();
        return true;
    }

    /// <summary>
    /// Cancels the pending crop, hiding the overlay and handles. Returns true if a crop was active.
    /// </summary>
    public bool CancelCrop()
    {
        var cropOverlay = _view.FindControl<global::Avalonia.Controls.Shapes.Rectangle>("CropOverlay");
        bool hasVisibleOverlay = cropOverlay != null && cropOverlay.IsVisible;
        if (!_cropActive && !hasVisibleOverlay) return false;

        HideCropChrome();

        if (cropOverlay != null)
        {
            cropOverlay.IsVisible = false;
            cropOverlay.Width = 0;
            cropOverlay.Height = 0;
        }
        return true;
    }

    /// <summary>
    /// Shows the crop overlay at full image bounds with 8 handles so the user can drag them inwards immediately.
    /// Uses auto-crop detection to suggest an initial crop rectangle when possible.
    /// Call when the user selects the Crop tool.
    /// </summary>
    public void ActivateCropToFullImage()
    {
        var canvas = _view.FindControl<Canvas>("AnnotationCanvas");
        var overlayCanvas = _view.FindControl<Canvas>("OverlayCanvas");
        var cropOverlay = _view.FindControl<global::Avalonia.Controls.Shapes.Rectangle>("CropOverlay");
        if (canvas == null || overlayCanvas == null || cropOverlay == null) return;

        double w = canvas.Bounds.Width;
        double h = canvas.Bounds.Height;
        if (w <= 0 || h <= 0) return;

        var cropRect = ComputeAutoCropRect(w, h);
        cropOverlay.Fill = Brushes.Transparent;
        cropOverlay.Stroke = Brushes.White;
        cropOverlay.StrokeThickness = 1;
        cropOverlay.StrokeDashArray = new global::Avalonia.Collections.AvaloniaList<double>();
        cropOverlay.SetValue(Panel.ZIndexProperty, CropOverlayZIndex);
        cropOverlay.IsVisible = true;
        Canvas.SetLeft(cropOverlay, ToOverlayCoordinate(cropRect.Left));
        Canvas.SetTop(cropOverlay, ToOverlayCoordinate(cropRect.Top));
        cropOverlay.Width = cropRect.Width;
        cropOverlay.Height = cropRect.Height;
        _cropActive = true;
        EnsureCropAdorners(overlayCanvas);
        UpdateCropAdorners(overlayCanvas, cropRect);
        ShowCropHandles(overlayCanvas, cropRect);
    }

    private void BeginQuickCrop()
    {
        var cropOverlay = _view.FindControl<global::Avalonia.Controls.Shapes.Rectangle>("CropOverlay");
        if (cropOverlay == null) return;

        HideCropChrome();

        cropOverlay.Fill = Brushes.Transparent;
        cropOverlay.Stroke = Brushes.White;
        cropOverlay.StrokeThickness = 2;
        cropOverlay.StrokeDashArray = new global::Avalonia.Collections.AvaloniaList<double> { 4, 2 };
        cropOverlay.SetValue(Panel.ZIndexProperty, CropOverlayZIndex);
        cropOverlay.IsVisible = true;
        Canvas.SetLeft(cropOverlay, ToOverlayCoordinate(_startPoint.X));
        Canvas.SetTop(cropOverlay, ToOverlayCoordinate(_startPoint.Y));
        cropOverlay.Width = 0;
        cropOverlay.Height = 0;
        _currentShape = cropOverlay;
    }

    /// <summary>
    /// Computes the auto-crop rectangle by detecting content bounds from the source image.
    /// Falls back to full image bounds if auto-crop finds no meaningful region.
    /// </summary>
    private Rect ComputeAutoCropRect(double canvasWidth, double canvasHeight)
    {
        const int AutoCropTolerance = 10;
        var fullRect = new Rect(0, 0, canvasWidth, canvasHeight);

        var sourceImage = _view.EditorCore?.SourceImage;
        if (sourceImage == null || sourceImage.Width <= 0 || sourceImage.Height <= 0)
            return fullRect;

        int imgW = sourceImage.Width;
        int imgH = sourceImage.Height;
        SKColor topLeft = sourceImage.GetPixel(0, 0);

        int minX = imgW, minY = imgH, maxX = 0, maxY = 0;
        bool hasContent = false;

        for (int y = 0; y < imgH; y++)
        {
            for (int x = 0; x < imgW; x++)
            {
                SKColor pixel = sourceImage.GetPixel(x, y);
                if (!ImageHelpers.ColorsMatch(pixel, topLeft, AutoCropTolerance))
                {
                    hasContent = true;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (!hasContent)
            return fullRect;

        int cropWidth = maxX - minX + 1;
        int cropHeight = maxY - minY + 1;

        // Only suggest auto-crop if it's meaningfully smaller than the full image
        if (cropWidth >= imgW && cropHeight >= imgH)
            return fullRect;

        // Scale pixel coordinates to canvas coordinates
        double scaleX = canvasWidth / imgW;
        double scaleY = canvasHeight / imgH;

        return new Rect(minX * scaleX, minY * scaleY, cropWidth * scaleX, cropHeight * scaleY);
    }

    private void ShowCropHandles(Canvas overlay, Rect cropRect)
    {
        HideCropHandles(overlay);
        EnsureCropAdorners(overlay);
        UpdateCropAdorners(overlay, cropRect);

        double centerX = cropRect.Left + (cropRect.Width / 2.0);
        double centerY = cropRect.Top + (cropRect.Height / 2.0);

        _cropHandles.Add(CreateCropHandle(overlay, cropRect.Left, cropRect.Top, "Crop_TopLeft"));
        _cropHandles.Add(CreateCropHandle(overlay, centerX, cropRect.Top, "Crop_TopCenter"));
        _cropHandles.Add(CreateCropHandle(overlay, cropRect.Right, cropRect.Top, "Crop_TopRight"));
        _cropHandles.Add(CreateCropHandle(overlay, cropRect.Right, centerY, "Crop_RightCenter"));
        _cropHandles.Add(CreateCropHandle(overlay, cropRect.Right, cropRect.Bottom, "Crop_BottomRight"));
        _cropHandles.Add(CreateCropHandle(overlay, centerX, cropRect.Bottom, "Crop_BottomCenter"));
        _cropHandles.Add(CreateCropHandle(overlay, cropRect.Left, cropRect.Bottom, "Crop_BottomLeft"));
        _cropHandles.Add(CreateCropHandle(overlay, cropRect.Left, centerY, "Crop_LeftCenter"));

        ShowCropConfirmButton(overlay, cropRect);

        overlay.InvalidateArrange();
        overlay.InvalidateVisual();
    }

    private void HideCropHandles(Canvas overlay)
    {
        foreach (var handle in _cropHandles)
            overlay.Children.Remove(handle);
        _cropHandles.Clear();
        HideCropConfirmButton(overlay);
    }

    private void HideCropChrome()
    {
        var overlayCanvas = _view.FindControl<Canvas>("OverlayCanvas");
        if (overlayCanvas != null)
        {
            HideCropHandles(overlayCanvas);
        }

        HideCropAdorners();
        ResetCropDragState();
        _cropActive = false;
    }

    private void ResetCropDragState()
    {
        _isDraggingCropHandle = false;
        _draggedCropHandleTag = null;
    }

    private const double CropConfirmButtonMargin = 10;
    private const int CropConfirmButtonZIndex = 8000;

    private void ShowCropConfirmButton(Canvas overlay, Rect cropRect)
    {
        HideCropConfirmButton(overlay);

        var button = new Button
        {
            Content = Strings.EditorView_Crop,
            Padding = new Thickness(16, 6),
            Cursor = new Cursor(StandardCursorType.Hand),
            Width = 80,
            Height = 40,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        button.SetValue(Panel.ZIndexProperty, CropConfirmButtonZIndex);
        button.Click += OnCropConfirmButtonClick;

        // Measure to get desired size
        button.Measure(Size.Infinity);
        double btnWidth = button.DesiredSize.Width;
        double btnHeight = button.DesiredSize.Height;

        // Position centered below crop rect, or above if it would clip the bottom
        double btnX = cropRect.Left + (cropRect.Width - btnWidth) / 2.0;

        var annotationCanvas = _view.FindControl<Canvas>("AnnotationCanvas");
        double canvasHeight = annotationCanvas?.Bounds.Height ?? overlay.Bounds.Height;

        double belowY = cropRect.Bottom + CropConfirmButtonMargin;
        double aboveY = cropRect.Top - CropConfirmButtonMargin - btnHeight;

        double btnY = (belowY + btnHeight <= canvasHeight) ? belowY : aboveY;

        Canvas.SetLeft(button, ToOverlayCoordinate(btnX));
        Canvas.SetTop(button, ToOverlayCoordinate(btnY));

        overlay.Children.Add(button);
        _cropConfirmButton = button;
    }

    private void HideCropConfirmButton(Canvas overlay)
    {
        if (_cropConfirmButton != null)
        {
            _cropConfirmButton.Click -= OnCropConfirmButtonClick;
            overlay.Children.Remove(_cropConfirmButton);
            _cropConfirmButton = null;
        }
    }

    private static bool CanMoveShapeDuringCreation(Control? shape)
    {
        return (shape is global::Avalonia.Controls.Shapes.Rectangle rectangle
                && rectangle.Name != "CropOverlay"
                && rectangle.Name != "CutOutOverlay")
            || shape is global::Avalonia.Controls.Shapes.Ellipse
            or SpeechBalloonControl
            or SpotlightControl
            || shape is global::Avalonia.Controls.Shapes.Path { Tag: FreehandAnnotation }
            || shape is global::Avalonia.Controls.Shapes.Path { Tag: LineAnnotation }
            || shape is global::Avalonia.Controls.Shapes.Path { Tag: ArrowAnnotation };
    }

    private void MoveShapeDuringCreation(Point currentPoint)
    {
        if (_currentShape == null)
        {
            return;
        }

        double deltaX = currentPoint.X - _lastDrawPoint.X;
        double deltaY = currentPoint.Y - _lastDrawPoint.Y;

        if (deltaX == 0 && deltaY == 0)
        {
            return;
        }

        _startPoint = new Point(_startPoint.X + deltaX, _startPoint.Y + deltaY);

        if (_currentShape is global::Avalonia.Controls.Shapes.Rectangle or global::Avalonia.Controls.Shapes.Ellipse)
        {
            double left = Canvas.GetLeft(_currentShape);
            double top = Canvas.GetTop(_currentShape);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            double newLeft = left + deltaX;
            double newTop = top + deltaY;
            Canvas.SetLeft(_currentShape, newLeft);
            Canvas.SetTop(_currentShape, newTop);

            UpdateEffectVisual(_currentShape, newLeft, newTop, _currentShape.Width, _currentShape.Height);

            if (_currentShape.Tag is RectangleAnnotation rectAnn)
            {
                rectAnn.StartPoint = ToSKPoint(new Point(newLeft, newTop));
                rectAnn.EndPoint = ToSKPoint(new Point(newLeft + _currentShape.Width, newTop + _currentShape.Height));
            }
            else if (_currentShape.Tag is EllipseAnnotation ellAnn)
            {
                ellAnn.StartPoint = ToSKPoint(new Point(newLeft, newTop));
                ellAnn.EndPoint = ToSKPoint(new Point(newLeft + _currentShape.Width, newTop + _currentShape.Height));
            }
            else if (_currentShape.Tag is BaseEffectAnnotation effectAnn)
            {
                effectAnn.StartPoint = ToSKPoint(new Point(newLeft, newTop));
                effectAnn.EndPoint = ToSKPoint(new Point(newLeft + _currentShape.Width, newTop + _currentShape.Height));
            }

            return;
        }

        if (_currentShape is SpeechBalloonControl balloon && balloon.Annotation is SpeechBalloonAnnotation balloonAnn)
        {
            balloonAnn.StartPoint = new SKPoint(balloonAnn.StartPoint.X + (float)deltaX, balloonAnn.StartPoint.Y + (float)deltaY);
            balloonAnn.EndPoint = new SKPoint(balloonAnn.EndPoint.X + (float)deltaX, balloonAnn.EndPoint.Y + (float)deltaY);

            if (balloonAnn.HasTailPoint)
            {
                balloonAnn.SetTailPoint(new SKPoint(balloonAnn.TailPoint.X + (float)deltaX, balloonAnn.TailPoint.Y + (float)deltaY));
            }

            AnnotationVisualFactory.UpdateVisualControl(
                balloon,
                balloonAnn,
                AnnotationVisualMode.Persisted,
                _view.EditorCore.CanvasSize.Width,
                _view.EditorCore.CanvasSize.Height);
            return;
        }

        if (_currentShape is global::Avalonia.Controls.Shapes.Path freehandPath && freehandPath.Tag is FreehandAnnotation freehand)
        {
            for (int index = 0; index < freehand.Points.Count; index++)
            {
                var point = freehand.Points[index];
                freehand.Points[index] = new SKPoint(point.X + (float)deltaX, point.Y + (float)deltaY);
            }

            freehandPath.Data = freehand.CreateSmoothedGeometry();
            freehandPath.InvalidateVisual();
            return;
        }

        if (_currentShape is global::Avalonia.Controls.Shapes.Path linePath && linePath.Tag is LineAnnotation lineAnn)
        {
            lineAnn.StartPoint = new SKPoint(lineAnn.StartPoint.X + (float)deltaX, lineAnn.StartPoint.Y + (float)deltaY);
            lineAnn.EndPoint = new SKPoint(lineAnn.EndPoint.X + (float)deltaX, lineAnn.EndPoint.Y + (float)deltaY);
            AnnotationVisualFactory.UpdateVisualControl(linePath, lineAnn);
            return;
        }

        if (_currentShape is global::Avalonia.Controls.Shapes.Path arrowPath && arrowPath.Tag is ArrowAnnotation arrowAnn)
        {
            arrowAnn.StartPoint = new SKPoint(arrowAnn.StartPoint.X + (float)deltaX, arrowAnn.StartPoint.Y + (float)deltaY);
            arrowAnn.EndPoint = new SKPoint(arrowAnn.EndPoint.X + (float)deltaX, arrowAnn.EndPoint.Y + (float)deltaY);
            AnnotationVisualFactory.UpdateVisualControl(arrowPath, arrowAnn);
            return;
        }

        if (_currentShape is SpotlightControl spotlight && spotlight.Annotation is SpotlightAnnotation spotAnn)
        {
            spotAnn.StartPoint = new SKPoint(spotAnn.StartPoint.X + (float)deltaX, spotAnn.StartPoint.Y + (float)deltaY);
            spotAnn.EndPoint = new SKPoint(spotAnn.EndPoint.X + (float)deltaX, spotAnn.EndPoint.Y + (float)deltaY);
            _view.RefreshSpotlightOverlay();
        }
    }

    private void OnCropConfirmButtonClick(object? sender, RoutedEventArgs e)
    {
        TryConfirmCrop();
    }

    // Crop UI layers and dimensions tuned after surveying common editor patterns.
    private const int CropShadeZIndex = 5000;
    private const int CropOverlayZIndex = 6000;
    private const int CropGuideZIndex = 6500;
    private const int CropHandleZIndex = 7000;
    private const double CropHandleCornerHitSize = 40;
    private const double CropHandleEdgeHitLength = 40;
    private const double CropHandleEdgeHitThickness = 28;
    private const double CropHandleCornerArmLength = 24;
    private const double CropHandleCenterBarLength = 30;
    private const double CropHandleThickness = 5;
    private const double MinCropGuideSize = 24;

    private static readonly Color CropHandleFill = Color.FromRgb(255, 255, 255);
    private static readonly Color CropShadeFill = Color.FromArgb(140, 0, 0, 0);
    private static readonly Color CropGuideStroke = Color.FromArgb(210, 255, 255, 255);

    private void EnsureCropAdorners(Canvas overlay)
    {
        if (_cropShadeRects.Count == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                var shade = new Rectangle
                {
                    Fill = new SolidColorBrush(CropShadeFill),
                    Stroke = null,
                    IsHitTestVisible = false,
                    IsVisible = false
                };
                shade.SetValue(Panel.ZIndexProperty, CropShadeZIndex);
                _cropShadeRects.Add(shade);
                overlay.Children.Add(shade);
            }
        }
        else
        {
            foreach (var shade in _cropShadeRects)
            {
                if (shade.Parent != overlay)
                {
                    (shade.Parent as Panel)?.Children.Remove(shade);
                    overlay.Children.Add(shade);
                }
            }
        }

        if (_cropGuideLines.Count == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                var guide = new Line
                {
                    Stroke = new SolidColorBrush(CropGuideStroke),
                    StrokeThickness = 1,
                    StrokeDashArray = new global::Avalonia.Collections.AvaloniaList<double> { 3, 3 },
                    IsHitTestVisible = false,
                    IsVisible = false
                };
                guide.SetValue(Panel.ZIndexProperty, CropGuideZIndex);
                _cropGuideLines.Add(guide);
                overlay.Children.Add(guide);
            }
        }
        else
        {
            foreach (var guide in _cropGuideLines)
            {
                if (guide.Parent != overlay)
                {
                    (guide.Parent as Panel)?.Children.Remove(guide);
                    overlay.Children.Add(guide);
                }
            }
        }
    }

    private void HideCropAdorners()
    {
        foreach (var shade in _cropShadeRects)
        {
            shade.IsVisible = false;
        }

        foreach (var guide in _cropGuideLines)
        {
            guide.IsVisible = false;
        }
    }

    private void UpdateCropAdorners(Canvas overlay, Rect cropRect)
    {
        EnsureCropAdorners(overlay);

        var annotationCanvas = _view.FindControl<Canvas>("AnnotationCanvas");
        double canvasWidth = annotationCanvas?.Bounds.Width ?? overlay.Bounds.Width;
        double canvasHeight = annotationCanvas?.Bounds.Height ?? overlay.Bounds.Height;

        if (canvasWidth <= 0 || canvasHeight <= 0)
        {
            HideCropAdorners();
            return;
        }

        double left = ClampSafe(cropRect.Left, 0, canvasWidth);
        double top = ClampSafe(cropRect.Top, 0, canvasHeight);
        double right = ClampSafe(cropRect.Right, 0, canvasWidth);
        double bottom = ClampSafe(cropRect.Bottom, 0, canvasHeight);
        double width = Math.Max(0, right - left);
        double height = Math.Max(0, bottom - top);

        double overlayImageLeft = EditorView.OverlayCanvasBleed;
        double overlayImageTop = EditorView.OverlayCanvasBleed;
        double overlayLeft = ToOverlayCoordinate(left);
        double overlayTop = ToOverlayCoordinate(top);
        double overlayRight = ToOverlayCoordinate(right);
        double overlayBottom = ToOverlayCoordinate(bottom);

        SetCropAdornerRect(_cropShadeRects[0], overlayImageLeft, overlayImageTop, canvasWidth, Math.Max(0, overlayTop - overlayImageTop));
        SetCropAdornerRect(_cropShadeRects[1], overlayImageLeft, overlayTop, Math.Max(0, overlayLeft - overlayImageLeft), height);
        SetCropAdornerRect(_cropShadeRects[2], overlayRight, overlayTop, Math.Max(0, (overlayImageLeft + canvasWidth) - overlayRight), height);
        SetCropAdornerRect(_cropShadeRects[3], overlayImageLeft, overlayBottom, canvasWidth, Math.Max(0, (overlayImageTop + canvasHeight) - overlayBottom));

        bool showGuides = width >= MinCropGuideSize && height >= MinCropGuideSize;
        if (!showGuides)
        {
            foreach (var guide in _cropGuideLines)
            {
                guide.IsVisible = false;
            }
            return;
        }

        double v1 = left + width / 3.0;
        double v2 = left + (2.0 * width / 3.0);
        double h1 = top + height / 3.0;
        double h2 = top + (2.0 * height / 3.0);

        _cropGuideLines[0].StartPoint = ToOverlayPoint(new Point(v1, top));
        _cropGuideLines[0].EndPoint = ToOverlayPoint(new Point(v1, bottom));
        _cropGuideLines[1].StartPoint = ToOverlayPoint(new Point(v2, top));
        _cropGuideLines[1].EndPoint = ToOverlayPoint(new Point(v2, bottom));
        _cropGuideLines[2].StartPoint = ToOverlayPoint(new Point(left, h1));
        _cropGuideLines[2].EndPoint = ToOverlayPoint(new Point(right, h1));
        _cropGuideLines[3].StartPoint = ToOverlayPoint(new Point(left, h2));
        _cropGuideLines[3].EndPoint = ToOverlayPoint(new Point(right, h2));

        foreach (var guide in _cropGuideLines)
        {
            guide.IsVisible = true;
        }
    }

    private static void SetCropAdornerRect(Rectangle rect, double left, double top, double width, double height)
    {
        Canvas.SetLeft(rect, left);
        Canvas.SetTop(rect, top);
        rect.Width = Math.Max(0, width);
        rect.Height = Math.Max(0, height);
        rect.IsVisible = rect.Width > 0 && rect.Height > 0;
    }

    private Border CreateCropHandle(Canvas overlay, double x, double y, string tag)
    {
        Cursor cursor = new Cursor(StandardCursorType.SizeNorthSouth);
        if (tag.Contains("TopLeft") || tag.Contains("BottomRight")) cursor = new Cursor(StandardCursorType.TopLeftCorner);
        else if (tag.Contains("TopRight") || tag.Contains("BottomLeft")) cursor = new Cursor(StandardCursorType.TopRightCorner);
        else if (tag.Contains("Top") || tag.Contains("Bottom")) cursor = new Cursor(StandardCursorType.SizeNorthSouth);
        else if (tag.Contains("Left") || tag.Contains("Right")) cursor = new Cursor(StandardCursorType.SizeWestEast);

        bool isCorner = tag.EndsWith("TopLeft", StringComparison.Ordinal) || tag.EndsWith("TopRight", StringComparison.Ordinal)
            || tag.EndsWith("BottomRight", StringComparison.Ordinal) || tag.EndsWith("BottomLeft", StringComparison.Ordinal);

        bool isHorizontalEdge = tag.Contains("Top", StringComparison.Ordinal) || tag.Contains("Bottom", StringComparison.Ordinal);
        double width = isCorner ? CropHandleCornerHitSize : (isHorizontalEdge ? CropHandleEdgeHitLength : CropHandleEdgeHitThickness);
        double height = isCorner ? CropHandleCornerHitSize : (isHorizontalEdge ? CropHandleEdgeHitThickness : CropHandleEdgeHitLength);
        Control visual;

        if (isCorner)
        {
            visual = CreateCropCornerLShape(tag);
        }
        else
        {
            visual = CreateCropEdgeBar(width, height);
        }

        var handle = new Border
        {
            Width = width,
            Height = height,
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Tag = tag,
            Cursor = cursor,
            Child = visual,
            ClipToBounds = false
        };
        handle.SetValue(Panel.ZIndexProperty, CropHandleZIndex);

        Canvas.SetLeft(handle, ToOverlayCoordinate(x) - (width / 2.0));
        Canvas.SetTop(handle, ToOverlayCoordinate(y) - (height / 2.0));
        overlay.Children.Add(handle);
        return handle;
    }

    /// <summary>
    /// Creates an L-shaped crop bracket that sits on the crop corner and points into the crop area.
    /// </summary>
    private static Control CreateCropCornerLShape(string tag)
    {
        const double size = CropHandleCornerHitSize;
        const double arm = CropHandleCornerArmLength;
        const double w = CropHandleThickness;
        const double half = size / 2.0;

        // Build a 6-point polygon that forms a single connected L shape.
        // The vertex of the L sits at (half, half), centered in the hit area.
        // The polygon traces the outer boundary of the L clockwise.
        //
        // For TopLeft corner (arms extend right and down into the crop area):
        //   P0 ──────── P1
        //   │            │
        //   │   P3 ── P2
        //   │   │
        //   │   │
        //   P5  P4
        //
        bool extendsLeft = tag.Contains("Right", StringComparison.Ordinal);
        bool extendsUp = tag.Contains("Bottom", StringComparison.Ordinal);
        double horizontalEnd = extendsLeft ? half - arm : half + arm;
        double verticalEnd = extendsUp ? half - arm : half + arm;

        var geometry = new StreamGeometry();
        using (StreamGeometryContext context = geometry.Open())
        {
            context.BeginFigure(new Point(horizontalEnd, half), false);
            context.LineTo(new Point(half, half));
            context.LineTo(new Point(half, verticalEnd));
        }

        return new global::Avalonia.Controls.Shapes.Path
        {
            Width = size,
            Height = size,
            Data = geometry,
            Stroke = new SolidColorBrush(CropHandleFill),
            StrokeThickness = w,
            StrokeLineCap = PenLineCap.Round,
            StrokeJoin = PenLineJoin.Round,
            Stretch = Stretch.None,
            IsHitTestVisible = false
        };
    }

    /// <summary>
    /// Creates a small center resize node while keeping a larger hit target around it.
    /// </summary>
    private static Control CreateCropEdgeBar(double width, double height)
    {
        bool isHorizontal = width >= height;
        double barWidth = isHorizontal ? CropHandleCenterBarLength : CropHandleThickness;
        double barHeight = isHorizontal ? CropHandleThickness : CropHandleCenterBarLength;

        var canvas = new Canvas
        {
            Width = width,
            Height = height,
            IsHitTestVisible = false,
            ClipToBounds = false
        };

        canvas.Children.Add(CreateCropHandleRect(
            (width - barWidth) / 2.0,
            (height - barHeight) / 2.0,
            barWidth,
            barHeight));

        return canvas;
    }

    private static Rectangle CreateCropHandleRect(double left, double top, double width, double height)
    {
        var rect = new Rectangle
        {
            Width = width,
            Height = height,
            Fill = new SolidColorBrush(CropHandleFill),
            RadiusX = CropHandleThickness / 2.0,
            RadiusY = CropHandleThickness / 2.0,
            IsHitTestVisible = false
        };

        Canvas.SetLeft(rect, left);
        Canvas.SetTop(rect, top);
        return rect;
    }

    private void UpdateCropOverlayBounds(Rect newRect)
    {
        var cropOverlay = _view.FindControl<global::Avalonia.Controls.Shapes.Rectangle>("CropOverlay");
        if (cropOverlay == null) return;
        Canvas.SetLeft(cropOverlay, ToOverlayCoordinate(newRect.Left));
        Canvas.SetTop(cropOverlay, ToOverlayCoordinate(newRect.Top));
        cropOverlay.Width = newRect.Width;
        cropOverlay.Height = newRect.Height;
        var overlay = _view.FindControl<Canvas>("OverlayCanvas");
        if (overlay != null)
        {
            // Recreate handles so the nodes stay aligned with the latest crop bounds.
            ShowCropHandles(overlay, newRect);
        }
    }

    private void UpdateQuickCropPreview(Rect newRect)
    {
        var cropOverlay = _view.FindControl<global::Avalonia.Controls.Shapes.Rectangle>("CropOverlay");
        if (cropOverlay == null) return;

        Canvas.SetLeft(cropOverlay, ToOverlayCoordinate(newRect.Left));
        Canvas.SetTop(cropOverlay, ToOverlayCoordinate(newRect.Top));
        cropOverlay.Width = newRect.Width;
        cropOverlay.Height = newRect.Height;
        cropOverlay.IsVisible = newRect.Width > 0 && newRect.Height > 0;

        var overlayCanvas = _view.FindControl<Canvas>("OverlayCanvas");
        if (overlayCanvas == null) return;

        if (cropOverlay.IsVisible)
        {
            EnsureCropAdorners(overlayCanvas);
            UpdateCropAdorners(overlayCanvas, newRect);
        }
        else
        {
            HideCropAdorners();
        }
    }

    private const double MinCropSize = 16;

    private static Rect ComputeCropHandleResizedRect(string handleTag, Point dragStart, Point current, Rect originalRect, double canvasW, double canvasH)
    {
        double left = originalRect.Left;
        double top = originalRect.Top;
        double right = originalRect.Right;
        double bottom = originalRect.Bottom;
        double cx = ClampSafe(current.X, 0, canvasW);
        double cy = ClampSafe(current.Y, 0, canvasH);

        switch (handleTag)
        {
            case "Crop_TopLeft":
                left = ClampSafe(cx, 0, right - MinCropSize);
                top = ClampSafe(cy, 0, bottom - MinCropSize);
                break;
            case "Crop_TopCenter":
                top = ClampSafe(cy, 0, bottom - MinCropSize);
                break;
            case "Crop_TopRight":
                right = ClampSafe(cx, left + MinCropSize, canvasW);
                top = ClampSafe(cy, 0, bottom - MinCropSize);
                break;
            case "Crop_RightCenter":
                right = ClampSafe(cx, left + MinCropSize, canvasW);
                break;
            case "Crop_BottomRight":
                right = ClampSafe(cx, left + MinCropSize, canvasW);
                bottom = ClampSafe(cy, top + MinCropSize, canvasH);
                break;
            case "Crop_BottomCenter":
                bottom = ClampSafe(cy, top + MinCropSize, canvasH);
                break;
            case "Crop_BottomLeft":
                left = ClampSafe(cx, 0, right - MinCropSize);
                bottom = ClampSafe(cy, top + MinCropSize, canvasH);
                break;
            case "Crop_LeftCenter":
                left = ClampSafe(cx, 0, right - MinCropSize);
                break;
            case "Crop_Move":
                double deltaX = current.X - dragStart.X;
                double deltaY = current.Y - dragStart.Y;
                double maxLeft = Math.Max(0, canvasW - originalRect.Width);
                double maxTop = Math.Max(0, canvasH - originalRect.Height);
                double newLeft = ClampSafe(originalRect.Left + deltaX, 0, maxLeft);
                double newTop = ClampSafe(originalRect.Top + deltaY, 0, maxTop);
                return new Rect(newLeft, newTop, originalRect.Width, originalRect.Height);
            default:
                return originalRect;
        }

        left = ClampSafe(left, 0, canvasW - MinCropSize);
        top = ClampSafe(top, 0, canvasH - MinCropSize);
        right = ClampSafe(right, left + MinCropSize, canvasW);
        bottom = ClampSafe(bottom, top + MinCropSize, canvasH);

        return new Rect(left, top, Math.Max(MinCropSize, right - left), Math.Max(MinCropSize, bottom - top));
    }

    private static double ClampSafe(double value, double min, double max)
    {
        if (max < min) return min;
        return Math.Clamp(value, min, max);
    }

    private void PerformCrop()
    {
        var cropOverlay = _view.FindControl<global::Avalonia.Controls.Shapes.Rectangle>("CropOverlay");
        // ISSUE-004 fix: Store ViewModel locally to prevent null reference if it changes
        var vm = ViewModel;
        if (cropOverlay == null || !cropOverlay.IsVisible || vm == null) return;

        var x = FromOverlayCoordinate(Canvas.GetLeft(cropOverlay));
        var y = FromOverlayCoordinate(Canvas.GetTop(cropOverlay));
        var w = cropOverlay.Width;
        var h = cropOverlay.Height;

        if (w <= 0 || h <= 0)
        {
            cropOverlay.IsVisible = false;
            return;
        }

        // Canvas coordinates are already in image-pixel space (AnnotationCanvas is sized
        // to CanvasSize = bitmap.Width/Height). No DPI scaling needed — RenderScaling
        // only affects physical display pixels, not the logical layout coordinate space.
        var cropX = (int)Math.Round(x);
        var cropY = (int)Math.Round(y);
        var cropW = (int)Math.Round(w);
        var cropH = (int)Math.Round(h);

        vm.CropImage(cropX, cropY, cropW, cropH);

        cropOverlay.IsVisible = false;
        _currentShape = null; // Ensure we clear current shape
    }

    private void PerformCutOut(Canvas canvas)
    {
        // ISSUE-004 fix: Store ViewModel locally to prevent null reference if it changes
        var vm = ViewModel;
        if (_currentShape is global::Avalonia.Controls.Shapes.Rectangle cutOverlay && vm != null)
        {
            if (cutOverlay.Width > 0 && cutOverlay.Height > 0 && _cutOutDirection.HasValue)
            {
                // XIP0039 Guardrail 1: Annotation model coordinates are logical image pixels.
                // AnnotationCanvas is sized 1:1 with the source bitmap in logical pixels,
                // so no RenderScaling factor is needed — consistent with the Crop path.
                // The previous code incorrectly multiplied by RenderScaling, which caused
                // CutOut bounds to be scaled by the display DPI factor on high-DPI screens.
                if (_cutOutDirection.Value) // Vertical
                {
                    var left = Canvas.GetLeft(cutOverlay);
                    var w = cutOverlay.Width;
                    int startX = (int)Math.Round(left);
                    int endX = (int)Math.Round(left + w);
                    vm.CutOutImage(startX, endX, true);
                }
                else // Horizontal
                {
                    var top = Canvas.GetTop(cutOverlay);
                    var h = cutOverlay.Height;
                    int startY = (int)Math.Round(top);
                    int endY = (int)Math.Round(top + h);
                    vm.CutOutImage(startY, endY, false);
                }
            }
            canvas.Children.Remove(cutOverlay);
            _currentShape = null;
        }
    }

    private async Task HandleImageTool(Canvas canvas, Point point)
    {
        var topLevel = TopLevel.GetTopLevel(_view);
        if (topLevel?.StorageProvider != null)
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = Strings.EditorView_SelectImage,
                AllowMultiple = false,
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
            });

            if (files.Count > 0)
            {
                using var stream = await files[0].OpenReadAsync();
                var bitmap = new global::Avalonia.Media.Imaging.Bitmap(stream);
                var imageControl = new Image { Source = bitmap, Width = bitmap.Size.Width, Height = bitmap.Size.Height };
                var annotation = new ImageAnnotation();
                annotation.SetImage(BitmapConversionHelpers.ToSKBitmap(bitmap));
                imageControl.Tag = annotation;
                _view.SyncAnnotationCursor(imageControl);

                Canvas.SetLeft(imageControl, point.X - bitmap.Size.Width / 2);
                Canvas.SetTop(imageControl, point.Y - bitmap.Size.Height / 2);

                canvas.Children.Add(imageControl);
                // ISSUE-019 fix: Dead code removed - undo handled by EditorCore

                // Add to Core history
                _view.EditorCore.AddAnnotation(annotation);

                // Update HasAnnotations state for Clear button
                if (ViewModel != null) ViewModel.HasAnnotations = true;

                _selectionController.SetSelectedShape(imageControl);
            }
        }
    }

    private void HandleTextTool(Canvas canvas, SolidColorBrush brush, double strokeWidth)
    {
        var vm = ViewModel;
        if (vm == null) return;

        // For Text, FillColor is the text color, StrokeColor is the outline.
        // If TextColor is transparent, set it to the default text color (often black/white or Options.TextColor)
        string textColor = vm.TextColor;
        if (Avalonia.Media.Color.TryParse(textColor, out var parsedText) && parsedText.A == 0)
        {
            var fallback = vm.Options?.TextTextColor ?? Avalonia.Media.Color.FromArgb(255, 0, 0, 0);
            textColor = $"#{fallback.A:X2}{fallback.R:X2}{fallback.G:X2}{fallback.B:X2}";
            vm.TextColorValue = fallback; // Sync back to the UI so the user sees it
        }

        // Stroke is the outline color. If stroke width is 0, outline is effectively disabled.
        string strokeColor = vm.SelectedColor;
        string fillColor = vm.FillColor;

        var textAnnotation = new TextAnnotation
        {
            StrokeColor = strokeColor,
            FillColor = fillColor,
            TextColor = textColor,
            StrokeWidth = (float)strokeWidth,
            FontSize = vm.FontSize,
            FontFamily = vm.SelectedFontFamily,
            HorizontalAlignment = vm.SelectedTextHorizontalAlignment,
            IsBold = vm.TextBold,
            IsItalic = vm.TextItalic,
            StartPoint = ToSKPoint(_startPoint),
            EndPoint = ToSKPoint(_startPoint) // Will be updated when text is finalized
        };
        ApplyShadowOptions(textAnnotation, vm);

        var textBrush = Avalonia.Media.Color.TryParse(textColor, out var c) ? new SolidColorBrush(c) : brush;
        var textBox = new TextBox
        {
            Foreground = textBrush,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.White,
            FontSize = vm.FontSize,
            FontFamily = new Avalonia.Media.FontFamily(textAnnotation.FontFamily),
            FontWeight = vm.TextBold ? Avalonia.Media.FontWeight.Bold : Avalonia.Media.FontWeight.Normal,
            FontStyle = vm.TextItalic ? Avalonia.Media.FontStyle.Italic : Avalonia.Media.FontStyle.Normal,
            TextAlignment = TextHorizontalAlignmentHelper.ToAvaloniaTextAlignment(textAnnotation.HorizontalAlignment),
            HorizontalContentAlignment = TextHorizontalAlignmentHelper.ToHorizontalContentAlignment(textAnnotation.HorizontalAlignment),
            VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Text = string.Empty,
            Padding = new Thickness(4),
            AcceptsReturn = false,
            Tag = textAnnotation,
            MinWidth = 0
        };

        // Force Avalonia's internal text box states to be transparent
        textBox.Resources["TextControlBackground"] = Brushes.Transparent;
        textBox.Resources["TextControlBackgroundFocused"] = Brushes.Transparent;
        textBox.Resources["TextControlBackgroundPointerOver"] = Brushes.Transparent;

        if (textAnnotation.ShadowEnabled)
        {
            textBox.Effect = ShareX.ImageEditor.Presentation.Helpers.ShadowEffectHelper.CreateDropShadow(textAnnotation);
        }

        Canvas.SetLeft(textBox, _startPoint.X);
        Canvas.SetTop(textBox, _startPoint.Y);

        void OnCreationLostFocus(object? s, global::Avalonia.Interactivity.RoutedEventArgs args)
        {
            if (s is TextBox tb && tb.Tag is TextAnnotation annotation)
            {
                tb.LostFocus -= OnCreationLostFocus;

                tb.BorderThickness = new Thickness(0);
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    if (tb.Parent is Panel panel)
                    {
                        panel.Children.Remove(tb);
                    }
                }
                else
                {
                    // Update annotation with final text and bounds
                    annotation.Text = tb.Text ?? string.Empty;
                    Size naturalSize = OutlinedTextControl.MeasureNaturalSize(annotation);
                    double initialWidth = naturalSize.Width > 0 ? naturalSize.Width : tb.Bounds.Width;
                    double initialHeight = naturalSize.Height > 0 ? naturalSize.Height : tb.Bounds.Height;
                    annotation.EndPoint = new SKPoint(
                        (float)(Canvas.GetLeft(tb) + initialWidth),
                        (float)(Canvas.GetTop(tb) + initialHeight));

                    // Add to EditorCore to enable undo/redo
                    // ISSUE-012 fix: Null check for EditorCore in closure
                    if (_view?.EditorCore != null)
                    {
                        _view.EditorCore.AddAnnotation(annotation);

                        // Update HasAnnotations state for Clear button
                        if (_view?.DataContext is MainViewModel viewModel)
                        {
                            viewModel.HasAnnotations = true;
                        }

                        // Replace temporary TextBox with OutlinedTextControl
                        var control = AnnotationVisualFactory.CreateVisualControl(annotation, AnnotationVisualMode.Persisted);
                        if (control != null)
                        {
                            var panel = tb.Parent as Panel;
                            panel?.Children.Remove(tb);
                            panel?.Children.Add(control);

                            AnnotationVisualFactory.UpdateVisualControl(
                                control,
                                annotation,
                                AnnotationVisualMode.Persisted,
                                _view!.EditorCore!.CanvasSize.Width,
                                _view!.EditorCore!.CanvasSize.Height);

                            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                            var desiredWidth = control.DesiredSize.Width > 0 ? control.DesiredSize.Width : tb.Bounds.Width;
                            var desiredHeight = control.DesiredSize.Height > 0 ? control.DesiredSize.Height : tb.Bounds.Height;

                            annotation.EndPoint = new SKPoint(
                                (float)(Canvas.GetLeft(control) + desiredWidth),
                                (float)(Canvas.GetTop(control) + desiredHeight));

                            AnnotationVisualFactory.UpdateVisualControl(
                                control,
                                annotation,
                                AnnotationVisualMode.Persisted,
                                _view!.EditorCore!.CanvasSize.Width,
                                _view!.EditorCore!.CanvasSize.Height);

                            control.InvalidateVisual();

                            // Auto-select the newly created text
                            _selectionController.SetSelectedShape(control);
                        }
                    }
                }
            }
        }

        textBox.LostFocus += OnCreationLostFocus;

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

        canvas.Children.Add(textBox);

        var canvasScrollViewer = _view.FindControl<ScrollViewer>("CanvasScrollViewer");
        var preservedOffset = canvasScrollViewer?.Offset ?? default;

        Dispatcher.UIThread.Post(() =>
        {
            textBox.Focus();
            textBox.CaretIndex = 0;

            if (canvasScrollViewer != null)
            {
                canvasScrollViewer.Offset = preservedOffset;
            }
        }, DispatcherPriority.Render);

        _isDrawing = false;
    }

    private static bool IsColorLight(string colorHex)
    {
        if (Avalonia.Media.Color.TryParse(colorHex, out var color))
        {
            double lum = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255.0;
            return lum > 0.5;
        }
        return true; // Default to light if parse fails
    }

    private static void ApplyShadowOptions(Annotation annotation, MainViewModel vm)
    {
        annotation.ShadowEnabled = vm.ShadowEnabled;
        annotation.ShadowColor = vm.Options?.ShadowColorHex ?? Annotation.DefaultShadowColorHex;
        annotation.ShadowBlurRadius = vm.Options?.ShadowBlurRadius ?? Annotation.DefaultShadowBlurRadius;
        annotation.ShadowOpacity = vm.Options?.ShadowOpacity ?? Annotation.DefaultShadowOpacity;
        annotation.ShadowOffsetX = vm.Options?.ShadowOffsetX ?? Annotation.DefaultShadowOffsetX;
        annotation.ShadowOffsetY = vm.Options?.ShadowOffsetY ?? Annotation.DefaultShadowOffsetY;
    }

    private static SKPoint ToSKPoint(Point point) => new((float)point.X, (float)point.Y);
    private static SKSize ToSKSize(Size size) => new((float)size.Width, (float)size.Height);

    /// <summary>
    /// Snaps the endpoint so the line from <paramref name="start"/> to <paramref name="end"/>
    /// is locked to the nearest 45-degree increment (0°, 45°, 90°, 135°, etc.).
    /// The distance from start to end is preserved.
    /// </summary>
    internal static Point SnapTo45Degrees(Point start, Point end)
    {
        double dx = end.X - start.X;
        double dy = end.Y - start.Y;
        double distance = Math.Sqrt(dx * dx + dy * dy);
        if (distance < 0.001) return end;

        double angle = Math.Atan2(dy, dx);
        double snapped = Math.Round(angle / (Math.PI / 4)) * (Math.PI / 4);

        return new Point(
            start.X + distance * Math.Cos(snapped),
            start.Y + distance * Math.Sin(snapped));
    }
}
