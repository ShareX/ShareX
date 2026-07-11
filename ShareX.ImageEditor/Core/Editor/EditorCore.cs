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

using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Core.History;
using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Manipulations;
using ShareX.ImageEditor.Integration;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.Editor;

/// <summary>
/// Platform-agnostic image editor core. Handles all editing logic including:
/// - Annotation management (create, select, delete)
/// - Mouse/pointer input processing
/// - Undo/redo operations
/// - Rendering to SKCanvas
///
/// Platform hosts (Avalonia, WinForms) provide:
/// - SKCanvas surface for rendering
/// - Forward input events to this core
/// - Display rendered results
///
/// <para><strong>THREADING CONTRACT (ISSUE-007 fix):</strong></para>
/// <para>
/// All events (<see cref="InvalidateRequested"/>, <see cref="ImageChanged"/>,
/// <see cref="AnnotationsRestored"/>, <see cref="HistoryChanged"/>, etc.) are fired
/// on the calling thread, which may NOT be the UI thread.
/// </para>
/// <para>
/// Subscribers MUST dispatch to the UI thread when performing UI operations.
/// Example: <c>Dispatcher.UIThread.Post(() => { /* UI update */ });</c>
/// </para>
/// </summary>
public class EditorCore : IDisposable
{
    #region Events

    /// <summary>
    /// Raised when the editor state changes and a redraw is needed
    /// </summary>
    public event Action? InvalidateRequested;

    /// <summary>
    /// Raised when the z-order of annotations changes
    /// </summary>
    public event Action? AnnotationOrderChanged;

    public event Action? ImageChanged;
    public event Action<Annotation>? EditAnnotationRequested;

    /// <summary>
    /// Raised when annotations are restored from history and the UI needs to fully sync
    /// </summary>
    public event Action? AnnotationsRestored;

    /// <summary>
    /// Raised when undo/redo history state changes
    /// </summary>
    public event Action? HistoryChanged;

    #endregion

    #region State

    /// <summary>
    /// The source image being edited
    /// </summary>
    public SKBitmap? SourceImage { get; private set; }

    /// <summary>
    /// Current active tool
    /// </summary>
    public EditorTool ActiveTool { get; set; } = EditorTool.Select;

    /// <summary>
    /// Current stroke color (hex string)
    /// </summary>
    public string StrokeColor { get; set; } = "#ef4444";

    /// <summary>
    /// Current stroke width
    /// </summary>
    public float StrokeWidth { get; set; } = 4;

    /// <summary>
    /// Current zoom level (1.0 = 100%)
    /// </summary>
    public float Zoom { get; set; } = 1.0f;

    /// <summary>
    /// Canvas size for rendering
    /// </summary>
    public SKSize CanvasSize { get; set; }

    /// <summary>
    /// Auto-incrementing number counter for Number tool
    /// </summary>
    public int NumberCounter { get; set; } = 1;

    #endregion

    #region Annotations

    private readonly List<Annotation> _annotations = new();
    private EditorHistory _history;

    private Annotation? _currentAnnotation;
    private Annotation? _selectedAnnotation;
    private bool _isDrawing;
    private SKPoint _startPoint;
    private SKPoint _lastDragPoint;
    private bool _isDragging;
    private bool _isResizing;
    private HandleType _activeHandle;
    private SKRect _initialBounds;
    private SKPoint _rotationCenter;

    private const float HandleSize = 10f;
    private const float RotationHandleOffset = 30f;
    private enum HandleType { None, TopLeft, TopMiddle, TopRight, MiddleRight, BottomRight, BottomMiddle, BottomLeft, MiddleLeft, Start, End, Rotate }

    /// <summary>
    /// All annotations in the editor
    /// </summary>
    public IReadOnlyList<Annotation> Annotations => _annotations;

    /// <summary>
    /// Currently selected annotation
    /// </summary>
    public Annotation? SelectedAnnotation => _selectedAnnotation;

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the editor core
    /// </summary>
    public EditorCore()
    {
        _history = new EditorHistory(this);
    }

    /// <summary>
    /// Load an image into the editor
    /// </summary>
    public void LoadImage(SKBitmap bitmap)
    {
        SourceImage?.Dispose();
        SourceImage = bitmap;
        CanvasSize = new SKSize(bitmap.Width, bitmap.Height);
        ClearAll();
        InvalidateRequested?.Invoke();
    }

    /// <summary>
    /// Load image from file path
    /// </summary>
    public void LoadImage(string filePath)
    {
        var bitmap = SKBitmap.Decode(filePath);
        if (bitmap != null)
        {
            LoadImage(bitmap);
        }
    }

    /// <summary>
    /// Update the source image without clearing history or annotations.
    /// Used for smart padding operations where we need to update the background
    /// but preserve the editing state.
    /// </summary>
    public void UpdateSourceImage(SKBitmap bitmap)
    {
        SourceImage?.Dispose();
        SourceImage = bitmap;
        CanvasSize = new SKSize(bitmap.Width, bitmap.Height);
        InvalidateRequested?.Invoke();
        ImageChanged?.Invoke();
    }

    /// <summary>
    /// Applies an image mutation and records it in the unified core history stack.
    /// </summary>
    /// <param name="operation">Operation that takes current source image and returns the mutated image.</param>
    /// <param name="clearAnnotations">Whether annotations should be cleared after mutation.</param>
    /// <returns>True if operation succeeded and image changed.</returns>
    public bool ApplyImageOperation(Func<SKBitmap, SKBitmap> operation, bool clearAnnotations = false, Action? transformAnnotations = null)
    {
        if (SourceImage == null || operation == null)
        {
            return false;
        }

        SKBitmap? result = null;

        try
        {
            result = operation(SourceImage);

            if (result == null || result.Width <= 0 || result.Height <= 0)
            {
                result?.Dispose();
                return false;
            }

            // Some operations may return the same bitmap instance. Normalize to an owned copy.
            if (ReferenceEquals(result, SourceImage))
            {
                SKBitmap? copy = SourceImage.Copy();
                if (copy == null)
                {
                    return false;
                }

                result = copy;
            }
        }
        catch (Exception ex)
        {
            result?.Dispose();
            EditorServices.ReportError(nameof(EditorCore), "Image operation failed.", ex);
            return false;
        }

        _history.CreateCanvasMemento();

        SourceImage.Dispose();
        SourceImage = result;
        CanvasSize = new SKSize(SourceImage.Width, SourceImage.Height);

        if (clearAnnotations)
        {
            ClearAnnotationsForImageMutation();
        }
        else
        {
            transformAnnotations?.Invoke();
            RefreshAnnotationsForCurrentImage();
            if (transformAnnotations != null)
            {
                AnnotationsRestored?.Invoke();
            }
        }

        ImageChanged?.Invoke();
        HistoryChanged?.Invoke();
        InvalidateRequested?.Invoke();
        return true;
    }

    /// <summary>
    /// Applies an image effect operation and tracks it in unified core history.
    /// </summary>
    public bool ApplyImageEffect(Func<SKBitmap, SKBitmap> effectOperation)
    {
        return ApplyImageOperation(effectOperation, clearAnnotations: false);
    }

    public bool ResizeImage(int newWidth, int newHeight, SKSamplingOptions sampling = default)
    {
        if (newWidth <= 0 || newHeight <= 0)
        {
            return false;
        }

        float scaleX = SourceImage == null ? 1f : (float)newWidth / SourceImage.Width;
        float scaleY = SourceImage == null ? 1f : (float)newHeight / SourceImage.Height;

        return ApplyImageOperation(
            img => ImageHelpers.Resize(img, newWidth, newHeight, maintainAspectRatio: false, sampling),
            clearAnnotations: false,
            transformAnnotations: () => ScaleAnnotations(scaleX, scaleY));
    }

    public bool ResizeCanvas(int top, int right, int bottom, int left, SKColor backgroundColor)
    {
        return ApplyImageOperation(
            img => ImageHelpers.ResizeCanvas(img, left, top, right, bottom, backgroundColor),
            clearAnnotations: false,
            transformAnnotations: () => TranslateAnnotations(left, top));
    }

    public bool Rotate90Clockwise()
    {
        int oldW = SourceImage?.Width ?? 0;
        int oldH = SourceImage?.Height ?? 0;
        return ApplyImageOperation(
            ImageHelpers.Rotate90Clockwise,
            clearAnnotations: false,
            transformAnnotations: () => RotateAnnotationsOrthogonal(90, oldW, oldH));
    }

    public bool Rotate90CounterClockwise()
    {
        int oldW = SourceImage?.Width ?? 0;
        int oldH = SourceImage?.Height ?? 0;
        return ApplyImageOperation(
            ImageHelpers.Rotate90CounterClockwise,
            clearAnnotations: false,
            transformAnnotations: () => RotateAnnotationsOrthogonal(270, oldW, oldH));
    }

    public bool Rotate180()
    {
        int oldW = SourceImage?.Width ?? 0;
        int oldH = SourceImage?.Height ?? 0;
        return ApplyImageOperation(
            ImageHelpers.Rotate180,
            clearAnnotations: false,
            transformAnnotations: () => RotateAnnotationsOrthogonal(180, oldW, oldH));
    }

    public bool RotateCustomAngle(float angle, bool autoResize = true)
    {
        int oldW = SourceImage?.Width ?? 0;
        int oldH = SourceImage?.Height ?? 0;
        int newW = -1;
        int newH = -1;

        return ApplyImageOperation(
            img =>
            {
                var effect = RotateImageEffect.Custom(angle, autoResize);
                var result = effect.Apply(img);
                newW = result.Width;
                newH = result.Height;
                return result;
            },
            clearAnnotations: false,
            transformAnnotations: () => RotateAnnotationsArbitrary(angle, oldW, oldH, newW, newH, autoResize));
    }

    public bool FlipHorizontal()
    {
        int w = SourceImage?.Width ?? 0;
        return ApplyImageOperation(
            ImageHelpers.FlipHorizontal,
            clearAnnotations: false,
            transformAnnotations: () => FlipAnnotations(true, false, w, 0));
    }

    public bool FlipVertical()
    {
        int h = SourceImage?.Height ?? 0;
        return ApplyImageOperation(
            ImageHelpers.FlipVertical,
            clearAnnotations: false,
            transformAnnotations: () => FlipAnnotations(false, true, 0, h));
    }

    /// <summary>
    /// Flatten (merge) all annotations into the source image.
    /// The composite snapshot is provided by the View layer which renders image + annotations together.
    /// </summary>
    /// <param name="compositeSnapshot">Pre-rendered bitmap containing the source image with all annotations baked in.</param>
    /// <returns>True if the operation succeeded.</returns>
    public bool FlattenImage(SKBitmap compositeSnapshot)
    {
        if (compositeSnapshot == null || _annotations.Count == 0)
        {
            return false;
        }

        return ApplyImageOperation(_ => compositeSnapshot, clearAnnotations: true);
    }

    public bool AutoCrop(int tolerance = 10)
    {
        if (SourceImage == null)
        {
            return false;
        }

        SKColor topLeft = SourceImage.GetPixel(0, 0);
        int w = SourceImage.Width;
        int h = SourceImage.Height;
        int minX = w, minY = h, maxX = 0, maxY = 0;
        bool hasContent = false;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                SKColor pixel = SourceImage.GetPixel(x, y);
                if (!ImageHelpers.ColorsMatch(pixel, topLeft, tolerance))
                {
                    hasContent = true;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (!hasContent) minX = minY = 0;

        return ApplyImageOperation(
            img => ImageHelpers.AutoCrop(img, topLeft, tolerance),
            clearAnnotations: false,
            transformAnnotations: () => TranslateAnnotations(-minX, -minY));
    }

    #region Annotation Transformations

    private void TransformAnnotations(Func<SKPoint, SKPoint> transformPoint)
    {
        foreach (var ann in _annotations)
        {
            ann.StartPoint = transformPoint(ann.StartPoint);
            ann.EndPoint = transformPoint(ann.EndPoint);

            if (ann is FreehandAnnotation freehand)
            {
                for (int i = 0; i < freehand.Points.Count; i++)
                    freehand.Points[i] = transformPoint(freehand.Points[i]);
            }
            else if (ann is NumberAnnotation number && number.HasTailPoint)
            {
                number.SetTailPoint(transformPoint(number.TailPoint));
            }
            else if (ann is SpeechBalloonAnnotation balloon)
            {
                balloon.SetTailPoint(transformPoint(balloon.GetEffectiveTailPoint()));
            }
        }
    }

    private void TranslateAnnotations(float dx, float dy)
    {
        TransformAnnotations(p => new SKPoint(p.X + dx, p.Y + dy));
    }

    private void ScaleAnnotations(float scaleX, float scaleY)
    {
        TransformAnnotations(p => new SKPoint(p.X * scaleX, p.Y * scaleY));
        foreach (var ann in _annotations)
        {
            ann.StrokeWidth *= Math.Min(scaleX, scaleY);
            if (ann is TextAnnotation text)
            {
                text.FontSize *= scaleY;
            }
        }
    }

    private void RotateAnnotationsOrthogonal(int angleDegrees, int oldWidth, int oldHeight)
    {
        TransformAnnotations(p =>
        {
            return angleDegrees switch
            {
                90 => new SKPoint(oldHeight - p.Y, p.X),
                180 => new SKPoint(oldWidth - p.X, oldHeight - p.Y),
                270 => new SKPoint(p.Y, oldWidth - p.X),
                _ => p
            };
        });

        foreach (var ann in _annotations)
        {
            ann.RotationAngle += angleDegrees;
            while (ann.RotationAngle >= 360) ann.RotationAngle -= 360;
            while (ann.RotationAngle < 0) ann.RotationAngle += 360;
        }
    }

    private void RotateAnnotationsArbitrary(float angleDegrees, int oldWidth, int oldHeight, int newWidth, int newHeight, bool autoResize)
    {
        float rad = angleDegrees * (float)Math.PI / 180f;
        float cos = (float)Math.Cos(rad);
        float sin = (float)Math.Sin(rad);

        float cx = oldWidth / 2f;
        float cy = oldHeight / 2f;

        float targetCx = newWidth / 2f;
        float targetCy = newHeight / 2f;

        TransformAnnotations(p =>
        {
            float nx = cx + (p.X - cx) * cos - (p.Y - cy) * sin;
            float ny = cy + (p.X - cx) * sin + (p.Y - cy) * cos;

            if (autoResize)
            {
                // Mapped to new container center
                return new SKPoint(nx - cx + targetCx, ny - cy + targetCy);
            }
            else
            {
                return new SKPoint(nx, ny);
            }
        });

        foreach (var ann in _annotations)
        {
            ann.RotationAngle += angleDegrees;
            while (ann.RotationAngle >= 360) ann.RotationAngle -= 360;
            while (ann.RotationAngle < 0) ann.RotationAngle += 360;
        }
    }

    private void FlipAnnotations(bool flipH, bool flipV, int width, int height)
    {
        TransformAnnotations(p =>
        {
            float nx = flipH ? width - p.X : p.X;
            float ny = flipV ? height - p.Y : p.Y;
            return new SKPoint(nx, ny);
        });

        foreach (var ann in _annotations)
        {
            if (flipH)
            {
                ann.RotationAngle = 360 - ann.RotationAngle;
            }
            if (flipV)
            {
                ann.RotationAngle = 360 - ann.RotationAngle;
            }
            while (ann.RotationAngle >= 360) ann.RotationAngle -= 360;
            while (ann.RotationAngle < 0) ann.RotationAngle += 360;
        }
    }

    #endregion

    /// <summary>
    /// Perform cut-out operation with explicit start/end coordinates.
    /// </summary>
    public bool CutOut(int startPos, int endPos, bool isVertical)
    {
        if (SourceImage == null)
        {
            return false;
        }

        if (startPos > endPos)
        {
            (startPos, endPos) = (endPos, startPos);
        }

        if (isVertical)
        {
            if (startPos < 0 || endPos > SourceImage.Width || startPos >= endPos)
            {
                return false;
            }
        }
        else
        {
            if (startPos < 0 || endPos > SourceImage.Height || startPos >= endPos)
            {
                return false;
            }
        }

        _history.CreateCanvasMemento();

        if (isVertical)
        {
            int cutX = startPos;
            int cutWidth = endPos - startPos;
            int newWidth = SourceImage.Width - cutWidth;
            if (newWidth <= 0)
            {
                return false;
            }

            var resultBitmap = new SKBitmap(newWidth, SourceImage.Height);
            using (var canvas = new SKCanvas(resultBitmap))
            {
                if (cutX > 0)
                {
                    var sourceRect = new SKRect(0, 0, cutX, SourceImage.Height);
                    var destRect = new SKRect(0, 0, cutX, SourceImage.Height);
                    canvas.DrawBitmap(SourceImage, sourceRect, destRect);
                }

                int rightStart = cutX + cutWidth;
                if (rightStart < SourceImage.Width)
                {
                    var sourceRect = new SKRect(rightStart, 0, SourceImage.Width, SourceImage.Height);
                    var destRect = new SKRect(cutX, 0, newWidth, SourceImage.Height);
                    canvas.DrawBitmap(SourceImage, sourceRect, destRect);
                }
            }

            SourceImage.Dispose();
            SourceImage = resultBitmap;
            CanvasSize = new SKSize(newWidth, SourceImage.Height);
            AdjustAnnotationsForVerticalCut(cutX, cutWidth, newWidth);
        }
        else
        {
            int cutY = startPos;
            int cutHeight = endPos - startPos;
            int newHeight = SourceImage.Height - cutHeight;
            if (newHeight <= 0)
            {
                return false;
            }

            var resultBitmap = new SKBitmap(SourceImage.Width, newHeight);
            using (var canvas = new SKCanvas(resultBitmap))
            {
                if (cutY > 0)
                {
                    var sourceRect = new SKRect(0, 0, SourceImage.Width, cutY);
                    var destRect = new SKRect(0, 0, SourceImage.Width, cutY);
                    canvas.DrawBitmap(SourceImage, sourceRect, destRect);
                }

                int bottomStart = cutY + cutHeight;
                if (bottomStart < SourceImage.Height)
                {
                    var sourceRect = new SKRect(0, bottomStart, SourceImage.Width, SourceImage.Height);
                    var destRect = new SKRect(0, cutY, SourceImage.Width, newHeight);
                    canvas.DrawBitmap(SourceImage, sourceRect, destRect);
                }
            }

            SourceImage.Dispose();
            SourceImage = resultBitmap;
            CanvasSize = new SKSize(SourceImage.Width, newHeight);
            AdjustAnnotationsForHorizontalCut(cutY, cutHeight, newHeight);
        }

        AnnotationsRestored?.Invoke();
        ImageChanged?.Invoke();
        HistoryChanged?.Invoke();
        InvalidateRequested?.Invoke();
        return true;
    }

    /// <summary>
    /// Clear all annotations
    /// </summary>
    public void ClearAll(bool resetHistory = true)
    {
        if (!resetHistory)
        {
            _history.CreateAnnotationsMemento(force: true);
        }

        _annotations.Clear();

        if (resetHistory)
        {
            _history?.Dispose();
            _history = new EditorHistory(this);
        }

        _currentAnnotation = null;
        _selectedAnnotation = null;
        _isDrawing = false;
        NumberCounter = 1;
        InvalidateRequested?.Invoke();
        HistoryChanged?.Invoke();
    }

    private void RefreshAnnotationsForCurrentImage()
    {
        if (SourceImage == null)
        {
            return;
        }

        foreach (Annotation annotation in _annotations)
        {
            if (annotation is BaseEffectAnnotation effect)
            {
                effect.UpdateEffect(SourceImage);
            }
            else if (annotation is SpotlightAnnotation spotlight)
            {
                spotlight.CanvasSize = CanvasSize;
            }
        }
    }

    private void ClearAnnotationsForImageMutation()
    {
        if (_annotations.Count == 0)
        {
            return;
        }

        _annotations.Clear();
        _selectedAnnotation = null;
        _currentAnnotation = null;
        _isDrawing = false;
        NumberCounter = 1;
        AnnotationsRestored?.Invoke();
    }

    #endregion

    #region Input Handling

    /// <summary>
    /// Handle pointer/mouse press
    /// </summary>
    /// <param name="point">Position in canvas coordinates</param>
    /// <param name="isRightButton">True if right mouse button</param>
    public void OnPointerPressed(SKPoint point, bool isRightButton = false)
    {
        // Right-click deletes annotation under cursor
        if (isRightButton)
        {
            var hitAnnotation = HitTest(point);
            if (hitAnnotation != null)
            {
                RemoveAnnotation(hitAnnotation);
            }
            return;
        }

        _startPoint = point;

        string? sampledSmartEraserColor = null;

        // Sample rendered color for Smart Eraser to mirror Avalonia behavior
        if (ActiveTool == EditorTool.SmartEraser)
        {
            sampledSmartEraserColor = SampleCanvasColor(point);
        }

        // Interact with currently selected annotation first so users can resize/move immediately after drawing
        if (_selectedAnnotation != null)
        {
            var handle = GetHitHandle(point);
            if (handle != HandleType.None)
            {
                BeginResize(handle, point);
                return;
            }

            // Allow dragging a selected annotation even if the current tool is not Select
            if (_selectedAnnotation.HitTest(point))
            {
                _isDragging = true;
                _lastDragPoint = point;
                return;
            }
        }

        // Select mode - hit test existing annotations
        if (ActiveTool == EditorTool.Select)
        {
            var hit = HitTest(point);
            if (hit != null)
            {
                _selectedAnnotation = hit;
                _lastDragPoint = point;
                _isDragging = true;
            }
            else
            {
                _selectedAnnotation = null;
            }
            InvalidateRequested?.Invoke();
            return;
        }

        // Create new annotation based on active tool
        _currentAnnotation = CreateAnnotation(ActiveTool);
        if (_currentAnnotation != null)
        {
            _currentAnnotation.StartPoint = point;
            _currentAnnotation.EndPoint = point;
            _currentAnnotation.StrokeColor = StrokeColor;
            _currentAnnotation.StrokeWidth = StrokeWidth;

            // Handle special tools
            if (_currentAnnotation is SmartEraserAnnotation smartEraser)
            {
                smartEraser.StrokeWidth = 0;
                if (!string.IsNullOrEmpty(sampledSmartEraserColor))
                {
                    smartEraser.StrokeColor = sampledSmartEraserColor;
                    smartEraser.FillColor = sampledSmartEraserColor;
                }
                else
                {
                    smartEraser.StrokeColor = "#80FF0000";
                    smartEraser.FillColor = "#80FF0000";
                }
            }
            else if (_currentAnnotation is FreehandAnnotation freehand)
            {
                freehand.Points.Add(point);
            }
            else if (_currentAnnotation is TextAnnotation textAnn)
            {
                textAnn.FontSize = Math.Max(12, StrokeWidth * 4);
            }
            else if (_currentAnnotation is NumberAnnotation num)
            {
                num.Number = NumberCounter++;
            }
            else if (_currentAnnotation is SpotlightAnnotation spotlight)
            {
                spotlight.CanvasSize = CanvasSize;
            }

            _annotations.Add(_currentAnnotation);
            _isDrawing = true;
            InvalidateRequested?.Invoke();
        }
    }

    /// <summary>
    /// Handle pointer/mouse move
    /// </summary>
    public void OnPointerMoved(SKPoint point)
    {
        if (_isResizing && _selectedAnnotation != null)
        {
            if (_activeHandle == HandleType.Start)
            {
                _selectedAnnotation.StartPoint = point;
                UpdateAnnotationState(_selectedAnnotation);
                InvalidateRequested?.Invoke();
                return;
            }
            if (_activeHandle == HandleType.End)
            {
                _selectedAnnotation.EndPoint = point;
                UpdateAnnotationState(_selectedAnnotation);
                InvalidateRequested?.Invoke();
                return;
            }
            if (_activeHandle == HandleType.Rotate)
            {
                // Compute rotation angle from center to mouse position
                float dx = point.X - _rotationCenter.X;
                float dy = point.Y - _rotationCenter.Y;
                float angleRad = (float)Math.Atan2(dx, -dy); // 0 = up, clockwise positive
                float angleDeg = angleRad * 180f / (float)Math.PI;
                _selectedAnnotation.RotationAngle = angleDeg;
                UpdateAnnotationState(_selectedAnnotation);
                InvalidateRequested?.Invoke();
                return;
            }

            ApplyResize(point);

            _lastDragPoint = point;
            InvalidateRequested?.Invoke();
            return;
        }

        if (_isDragging && _selectedAnnotation != null)
        {
            var delta = new SKPoint(point.X - _lastDragPoint.X, point.Y - _lastDragPoint.Y);

            if (_selectedAnnotation is FreehandAnnotation freehand)
            {
                for (int i = 0; i < freehand.Points.Count; i++)
                {
                    freehand.Points[i] = new SKPoint(freehand.Points[i].X + delta.X, freehand.Points[i].Y + delta.Y);
                }
            }

            _selectedAnnotation.StartPoint = new SKPoint(_selectedAnnotation.StartPoint.X + delta.X, _selectedAnnotation.StartPoint.Y + delta.Y);
            _selectedAnnotation.EndPoint = new SKPoint(_selectedAnnotation.EndPoint.X + delta.X, _selectedAnnotation.EndPoint.Y + delta.Y);

            UpdateAnnotationState(_selectedAnnotation);

            _lastDragPoint = point;
            InvalidateRequested?.Invoke();
            return;
        }

        if (!_isDrawing || _currentAnnotation == null) return;

        if (_currentAnnotation is FreehandAnnotation freehandDraw)
        {
            freehandDraw.Points.Add(point);
        }
        else if (_currentAnnotation is CutOutAnnotation cutOut)
        {
            // Determine direction based on drag distance
            var deltaX = Math.Abs(point.X - _startPoint.X);
            var deltaY = Math.Abs(point.Y - _startPoint.Y);

            // Vertical cut if horizontal drag is greater
            cutOut.IsVertical = deltaX > deltaY;

            _currentAnnotation.EndPoint = point;
        }
        else
        {
            _currentAnnotation.EndPoint = point;
        }

        if (_currentAnnotation is SpotlightAnnotation spotlight)
        {
            spotlight.CanvasSize = CanvasSize;
        }

        UpdateAnnotationState(_currentAnnotation);

        InvalidateRequested?.Invoke();
    }

    /// <summary>
    /// Handle pointer/mouse release
    /// </summary>
    public void OnPointerReleased(SKPoint point)
    {
        if (_isResizing)
        {
            _isResizing = false;
            _activeHandle = HandleType.None;
            return;
        }

        if (_isDragging)
        {
            _isDragging = false;
            return;
        }

        if (!_isDrawing || _currentAnnotation == null) return;

        _isDrawing = false;

        // Finalize annotation
        _currentAnnotation.EndPoint = point;

        // Crop executes immediately like the Avalonia master behavior
        if (_currentAnnotation is CropAnnotation)
        {
            PerformCrop();
            _currentAnnotation = null;
            HistoryChanged?.Invoke();
            return;
        }

        // CutOut executes immediately
        if (_currentAnnotation is CutOutAnnotation)
        {
            PerformCutOut();
            _currentAnnotation = null;
            HistoryChanged?.Invoke();
            return;
        }

        // Create annotation memento for undo/redo
        // We exclude the current annotation because the undo stack must contain the state BEFORE this addition
        _history.CreateAnnotationsMemento(excludeAnnotation: _currentAnnotation);
        HistoryChanged?.Invoke();

        // Request edit for text/speech annotations
        if (_currentAnnotation is TextAnnotation || _currentAnnotation is SpeechBalloonAnnotation)
        {
            EditAnnotationRequested?.Invoke(_currentAnnotation);
        }

        // Auto-select the created annotation (skip only freehand which is not resizable)
        if (_currentAnnotation is not FreehandAnnotation)
        {
            _selectedAnnotation = _currentAnnotation;
        }
        _currentAnnotation = null;

        // Update effect with final bounds
        if (_selectedAnnotation is BaseEffectAnnotation effect && SourceImage != null)
        {
            effect.UpdateEffect(SourceImage);
        }

        InvalidateRequested?.Invoke();
    }

    private HandleType GetHitHandle(SKPoint point)
    {
        if (_selectedAnnotation == null) return HandleType.None;

        var handles = GetAnnotationHandles(_selectedAnnotation);
        float halfSize = HandleSize / 2;

        foreach (var handle in handles)
        {
            var r = new SKRect(
                handle.Position.X - halfSize,
                handle.Position.Y - halfSize,
                handle.Position.X + halfSize,
                handle.Position.Y + halfSize);

            if (r.Contains(point)) return handle.Type;
        }

        return HandleType.None;
    }

    private void BeginResize(HandleType handle, SKPoint point)
    {
        _isResizing = true;
        _activeHandle = handle;
        _initialBounds = _selectedAnnotation!.GetBounds();
        _lastDragPoint = point;

        // Store center of bounds for rotation calculations
        _rotationCenter = new SKPoint(_initialBounds.MidX, _initialBounds.MidY);

        if (_selectedAnnotation is FreehandAnnotation freehand)
        {
        }
        else
        {
        }

        InvalidateRequested?.Invoke();
    }

    private void ApplyResize(SKPoint point)
    {
        if (_selectedAnnotation == null) return;

        // Rotation is handled directly in OnPointerMoved
        if (_activeHandle == HandleType.Rotate) return;

        // Lines/arrows resize via start/end handles only
        if (_selectedAnnotation is LineAnnotation || _selectedAnnotation is ArrowAnnotation)
        {
            return;
        }

        // Freehand is not resizable in the Avalonia editor
        if (_selectedAnnotation is FreehandAnnotation)
        {
            return;
        }

        var newBounds = CalculateResizeBounds(point);

        _selectedAnnotation.StartPoint = new SKPoint(newBounds.Left, newBounds.Top);
        _selectedAnnotation.EndPoint = new SKPoint(newBounds.Right, newBounds.Bottom);

        UpdateAnnotationState(_selectedAnnotation);
    }

    private SKRect CalculateResizeBounds(SKPoint point)
    {
        float left = _initialBounds.Left;
        float right = _initialBounds.Right;
        float top = _initialBounds.Top;
        float bottom = _initialBounds.Bottom;

        switch (_activeHandle)
        {
            case HandleType.TopLeft:
                left = point.X;
                top = point.Y;
                break;
            case HandleType.TopMiddle:
                top = point.Y;
                break;
            case HandleType.TopRight:
                right = point.X;
                top = point.Y;
                break;
            case HandleType.MiddleRight:
                right = point.X;
                break;
            case HandleType.BottomRight:
                right = point.X;
                bottom = point.Y;
                break;
            case HandleType.BottomMiddle:
                bottom = point.Y;
                break;
            case HandleType.BottomLeft:
                left = point.X;
                bottom = point.Y;
                break;
            case HandleType.MiddleLeft:
                left = point.X;
                break;
        }

        return new SKRect(
            Math.Min(left, right),
            Math.Min(top, bottom),
            Math.Max(left, right),
            Math.Max(top, bottom));
    }

    private void UpdateAnnotationState(Annotation annotation)
    {
        if (annotation is SpotlightAnnotation spotlight)
        {
            spotlight.CanvasSize = CanvasSize;
        }

        if (annotation is BaseEffectAnnotation effect && SourceImage != null)
        {
            effect.UpdateEffect(SourceImage);
        }
    }

    public string? SampleCanvasColor(SKPoint point)
    {
        using var snapshot = GetSnapshot();
        if (snapshot == null) return null;

        int x = (int)Math.Round(point.X);
        int y = (int)Math.Round(point.Y);

        if (x < 0 || y < 0 || x >= snapshot.Width || y >= snapshot.Height)
        {
            x = 0;
            y = 0;
        }

        var color = snapshot.GetPixel(x, y);
        return $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}";
    }

    #endregion

    #region Selection & HitTest

    private Annotation? HitTest(SKPoint point)
    {
        // Test in reverse order (top-most first)
        for (int i = _annotations.Count - 1; i >= 0; i--)
        {
            if (_annotations[i].HitTest(point))
            {
                return _annotations[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Select an annotation
    /// </summary>
    public void Select(Annotation? annotation)
    {
        _selectedAnnotation = annotation;
        InvalidateRequested?.Invoke();
    }

    /// <summary>
    /// Deselect current annotation
    /// </summary>
    public void Deselect()
    {
        _selectedAnnotation = null;
        InvalidateRequested?.Invoke();
    }

    /// <summary>
    /// Delete the selected annotation
    /// </summary>
    public void DeleteSelected()
    {
        if (_selectedAnnotation != null)
        {
            RemoveAnnotation(_selectedAnnotation);
        }
    }

    /// <summary>
    /// Bring the selected annotation to the front (render last)
    /// </summary>
    public void BringToFront()
    {
        if (_selectedAnnotation == null || !_annotations.Contains(_selectedAnnotation)) return;
        int index = _annotations.IndexOf(_selectedAnnotation);
        // Already at top
        if (index < 0 || index == _annotations.Count - 1) return;

        _history.CreateAnnotationsMemento();
        _annotations.RemoveAt(index);
        _annotations.Add(_selectedAnnotation);
        AnnotationOrderChanged?.Invoke();
        HistoryChanged?.Invoke();
        InvalidateRequested?.Invoke();
    }

    /// <summary>
    /// Send the selected annotation to the back (render first)
    /// </summary>
    public void SendToBack()
    {
        if (_selectedAnnotation == null || !_annotations.Contains(_selectedAnnotation)) return;
        int index = _annotations.IndexOf(_selectedAnnotation);
        // Already at bottom
        if (index <= 0) return;

        _history.CreateAnnotationsMemento();
        _annotations.RemoveAt(index);
        _annotations.Insert(0, _selectedAnnotation);
        AnnotationOrderChanged?.Invoke();
        HistoryChanged?.Invoke();
        InvalidateRequested?.Invoke();
    }

    /// <summary>
    /// Bring the selected annotation forward one step
    /// </summary>
    public void BringForward()
    {
        if (_selectedAnnotation == null || !_annotations.Contains(_selectedAnnotation)) return;
        int index = _annotations.IndexOf(_selectedAnnotation);
        // Already at top
        if (index < 0 || index == _annotations.Count - 1) return;

        _history.CreateAnnotationsMemento();
        _annotations.RemoveAt(index);
        _annotations.Insert(index + 1, _selectedAnnotation);
        AnnotationOrderChanged?.Invoke();
        HistoryChanged?.Invoke();
        InvalidateRequested?.Invoke();
    }

    /// <summary>
    /// Send the selected annotation backward one step
    /// </summary>
    public void SendBackward()
    {
        if (_selectedAnnotation == null || !_annotations.Contains(_selectedAnnotation)) return;
        int index = _annotations.IndexOf(_selectedAnnotation);
        // Already at bottom
        if (index <= 0) return;

        _history.CreateAnnotationsMemento();
        _annotations.RemoveAt(index);
        _annotations.Insert(index - 1, _selectedAnnotation);
        AnnotationOrderChanged?.Invoke();
        HistoryChanged?.Invoke();
        InvalidateRequested?.Invoke();
    }

    /// <summary>
    /// Removes a specific annotation and handles history/renumbering
    /// </summary>
    public void RemoveAnnotation(Annotation annotation)
    {
        if (annotation == null || !_annotations.Contains(annotation)) return;

        // Capture state BEFORE deleting (so Undo reverts to state with this annotation)
        _history.CreateAnnotationsMemento();

        bool renumbered = false;
        if (annotation is NumberAnnotation num)
        {
            HandleStepRenumbering(num.Number);
            renumbered = true;
        }

        _annotations.Remove(annotation);
        if (_selectedAnnotation == annotation)
            _selectedAnnotation = null;

        HistoryChanged?.Invoke();
        InvalidateRequested?.Invoke();

        if (renumbered)
        {
            AnnotationsRestored?.Invoke();
        }
    }

    private void HandleStepRenumbering(int deletedNumber)
    {
        foreach (var annotation in _annotations)
        {
            if (annotation is NumberAnnotation numberAnnotation && numberAnnotation.Number > deletedNumber)
            {
                numberAnnotation.Number--;
            }
        }
    }

    /// <summary>
    /// Add an annotation from an external controller (e.g. InputController)
    /// This captures the history state BEFORE adding the annotation.
    /// </summary>
    public void AddAnnotation(Annotation annotation)
    {
        // Capture state BEFORE adding the new annotation (Undo will revert to this state)
        _history.CreateAnnotationsMemento();

        _annotations.Add(annotation);

        HistoryChanged?.Invoke();
        // We don't necessarily need to render if the view already added the control,
        // but this ensures raster effects are updated if needed.
        InvalidateRequested?.Invoke();
    }

    #endregion

    #region Undo/Redo

    public bool CanUndo => _history?.CanUndo ?? false;
    public bool CanRedo => _history?.CanRedo ?? false;

    public void Undo()
    {
        _history?.Undo();
        HistoryChanged?.Invoke();
    }

    public void Redo()
    {
        _history?.Redo();
        HistoryChanged?.Invoke();
    }

    /// <summary>
    /// Get a snapshot of current annotations (deep copy for memento)
    /// </summary>
    /// <param name="excludeAnnotation">Optional annotation to exclude from the snapshot (e.g. current one being drawn)</param>
    internal List<Annotation> GetAnnotationsSnapshot(Annotation? excludeAnnotation = null)
    {
        var source = excludeAnnotation != null
            ? _annotations.Where(a => a != excludeAnnotation)
            : _annotations;
        return source.Select(a => a.Clone()).ToList();
    }

    /// <summary>
    /// Restore editor state from a memento
    /// ISSUE-010 fix: Restores selection state along with annotations
    /// </summary>
    internal void RestoreState(EditorMemento memento)
    {
        // Clear current annotations
        _annotations.Clear();

        // Restore annotation list
        _annotations.AddRange(memento.Annotations);

        // If memento has a canvas bitmap, restore it (for crop/cutout undo)
        if (memento.Canvas != null)
        {
            SourceImage?.Dispose();
            SourceImage = memento.Canvas.Copy();
            CanvasSize = memento.CanvasSize;

            // Notify that image has changed so UI can resize canvas control if needed
            ImageChanged?.Invoke();
        }

        // ISSUE-010 fix: Restore selection state if memento captured a selected annotation
        if (memento.SelectedAnnotationId.HasValue)
        {
            _selectedAnnotation = _annotations.FirstOrDefault(a => a.Id == memento.SelectedAnnotationId.Value);
        }
        else
        {
            _selectedAnnotation = null;
        }

        // Trigger redraw
        InvalidateRequested?.Invoke();
        AnnotationsRestored?.Invoke();
    }

    #endregion

    #region Rendering

    /// <summary>
    /// Render the source image to an SKCanvas.
    /// Annotations are rendered by the Avalonia visual tree (hybrid rendering).
    /// </summary>
    /// <param name="canvas">Target canvas</param>
    public void Render(SKCanvas canvas)
    {
        canvas.Clear(SKColors.Transparent);

        // Draw source image only — annotations are handled by Avalonia controls
        if (SourceImage != null)
        {
            canvas.DrawBitmap(SourceImage, 0, 0);
        }
    }

    /// <summary>
    /// Get a snapshot of the source image (without annotations) as an SKBitmap.
    /// Used internally for pixel color sampling (e.g. SmartEraser).
    /// For full export with annotations, use EditorView.GetSnapshot() which
    /// renders the Avalonia visual tree via RenderTargetBitmap.
    /// </summary>
    public SKBitmap? GetSnapshot()
    {
        if (SourceImage == null) return null;

        var bitmap = new SKBitmap(SourceImage.Width, SourceImage.Height);
        using var canvas = new SKCanvas(bitmap);
        canvas.DrawBitmap(SourceImage, 0, 0);

        return bitmap;
    }

    private IEnumerable<(HandleType Type, SKPoint Position)> GetAnnotationHandles(Annotation annotation)
    {
        if (annotation is FreehandAnnotation or CursorAnnotation)
        {
            yield break;
        }
        else if (annotation is LineAnnotation || annotation is ArrowAnnotation)
        {
            yield return (HandleType.Start, annotation.StartPoint);
            yield return (HandleType.End, annotation.EndPoint);
        }
        else
        {
            var bounds = annotation.GetBounds();
            var center = new SKPoint(bounds.MidX, bounds.MidY);
            float angle = annotation.RotationAngle;
            bool rotate = angle != 0;

            var tl = new SKPoint(bounds.Left, bounds.Top);
            var tm = new SKPoint(bounds.MidX, bounds.Top);
            var tr = new SKPoint(bounds.Right, bounds.Top);
            var mr = new SKPoint(bounds.Right, bounds.MidY);
            var br = new SKPoint(bounds.Right, bounds.Bottom);
            var bm = new SKPoint(bounds.MidX, bounds.Bottom);
            var bl = new SKPoint(bounds.Left, bounds.Bottom);
            var ml = new SKPoint(bounds.Left, bounds.MidY);

            yield return (HandleType.TopLeft, rotate ? RotatePoint(tl, center, angle) : tl);
            yield return (HandleType.TopMiddle, rotate ? RotatePoint(tm, center, angle) : tm);
            yield return (HandleType.TopRight, rotate ? RotatePoint(tr, center, angle) : tr);
            yield return (HandleType.MiddleRight, rotate ? RotatePoint(mr, center, angle) : mr);
            yield return (HandleType.BottomRight, rotate ? RotatePoint(br, center, angle) : br);
            yield return (HandleType.BottomMiddle, rotate ? RotatePoint(bm, center, angle) : bm);
            yield return (HandleType.BottomLeft, rotate ? RotatePoint(bl, center, angle) : bl);
            yield return (HandleType.MiddleLeft, rotate ? RotatePoint(ml, center, angle) : ml);

            // Rotation handle above the top-center for annotations that support node rotation.
            if (SupportsRotationHandle(annotation))
            {
                var rotHandle = new SKPoint(bounds.MidX, bounds.Top - RotationHandleOffset);
                yield return (HandleType.Rotate, rotate ? RotatePoint(rotHandle, center, angle) : rotHandle);
            }
        }
    }

    private static bool SupportsRotationHandle(Annotation annotation)
    {
        return annotation switch
        {
            BaseEffectAnnotation => true,
            TextAnnotation => true,
            ImageAnnotation and not CursorAnnotation => true,
            SpeechBalloonAnnotation => true,
            RectangleAnnotation and not SmartEraserAnnotation => true,
            EllipseAnnotation => true,
            _ => false
        };
    }

    /// <summary>
    /// Rotates a point around a center by the given angle in degrees (clockwise).
    /// </summary>
    private static SKPoint RotatePoint(SKPoint point, SKPoint center, float angleDeg)
    {
        float rad = angleDeg * (float)Math.PI / 180f;
        float cos = (float)Math.Cos(rad);
        float sin = (float)Math.Sin(rad);
        float dx = point.X - center.X;
        float dy = point.Y - center.Y;
        return new SKPoint(
            center.X + dx * cos - dy * sin,
            center.Y + dx * sin + dy * cos);
    }

    #endregion

    #region Annotation Factory

    private Annotation? CreateAnnotation(EditorTool tool)
    {
        return tool switch
        {
            EditorTool.Rectangle => new RectangleAnnotation(),
            EditorTool.Ellipse => new EllipseAnnotation(),
            EditorTool.Line => new LineAnnotation(),
            EditorTool.Arrow => new ArrowAnnotation(),
            EditorTool.Text => new TextAnnotation(),
            EditorTool.Freehand => new FreehandAnnotation(),
            EditorTool.SmartEraser => new SmartEraserAnnotation(),
            EditorTool.Step => new NumberAnnotation(),
            EditorTool.Cursor => new CursorAnnotation(),
            EditorTool.Blur => new BlurAnnotation(),
            EditorTool.Pixelate => new PixelateAnnotation(),
            EditorTool.Highlight => new HighlightAnnotation(),
            EditorTool.Spotlight => new SpotlightAnnotation(),
            EditorTool.Magnify => new MagnifyAnnotation(),
            EditorTool.SpeechBalloon => new SpeechBalloonAnnotation(),
            EditorTool.Crop => new CropAnnotation(),
            EditorTool.CutOut => new CutOutAnnotation(),
            _ => null
        };
    }

    #endregion

    #region Crop

    /// <summary>
    /// Perform crop operation using the current crop annotation
    /// </summary>
    public void PerformCrop()
    {
        var cropAnnotation = _annotations.OfType<CropAnnotation>().FirstOrDefault();
        if (cropAnnotation == null || SourceImage == null) return;

        var bounds = cropAnnotation.GetBounds();
        Crop(bounds);

        // Remove crop annotation after processing
        _annotations.Remove(cropAnnotation);
        InvalidateRequested?.Invoke();
    }

    /// <summary>
    /// Perform crop operation with specific bounds
    /// </summary>
    public bool Crop(SKRect bounds)
    {
        if (SourceImage == null) return false;

        int x = (int)Math.Max(0, bounds.Left);
        int y = (int)Math.Max(0, bounds.Top);
        int width = (int)Math.Min(SourceImage.Width - x, bounds.Width);
        int height = (int)Math.Min(SourceImage.Height - y, bounds.Height);

        if (width <= 0 || height <= 0) return false;

        if (x == 0 && y == 0 && width == SourceImage.Width && height == SourceImage.Height)
        {
            return false;
        }

        // Create canvas memento before destructive crop operation
        _history.CreateCanvasMemento();

        var croppedBitmap = new SKBitmap(width, height);
        SourceImage.ExtractSubset(croppedBitmap, new SKRectI(x, y, x + width, y + height));

        // Adjust coordinates of all remaining annotations
        var offsetX = -x;
        var offsetY = -y;

        // Remove annotations that fall completely outside the cropped area
        // and adjust coordinates for those that remain
        for (int i = _annotations.Count - 1; i >= 0; i--)
        {
            var annotation = _annotations[i];

            // Skip CropAnnotation as caller might handle it, or it will be removed/ignored
            if (annotation is CropAnnotation) continue;

            var annotationBounds = annotation.GetBounds();

            // Check if annotation is completely outside the cropped region
            if (annotationBounds.Right < x || annotationBounds.Left > x + width ||
                annotationBounds.Bottom < y || annotationBounds.Top > y + height)
            {
                _annotations.RemoveAt(i);
                continue;
            }

            // Adjust annotation coordinates
            annotation.StartPoint = new SKPoint(
                annotation.StartPoint.X + offsetX,
                annotation.StartPoint.Y + offsetY);
            annotation.EndPoint = new SKPoint(
                annotation.EndPoint.X + offsetX,
                annotation.EndPoint.Y + offsetY);

            // Handle freehand annotations (they have a Points list)
            if (annotation is FreehandAnnotation freehand)
            {
                for (int j = 0; j < freehand.Points.Count; j++)
                {
                    freehand.Points[j] = new SKPoint(
                        freehand.Points[j].X + offsetX,
                        freehand.Points[j].Y + offsetY);
                }
            }

            // XIP0039 Guardrail 2: Adjust SpeechBalloon tail point so it stays
            // anchored to the same visual position relative to the new canvas origin.
            if (annotation is SpeechBalloonAnnotation balloon)
            {
                var tailPoint = balloon.GetEffectiveTailPoint();
                balloon.SetTailPoint(new SKPoint(
                    tailPoint.X + offsetX,
                    tailPoint.Y + offsetY));
            }
            else if (annotation is NumberAnnotation number && number.HasTailPoint)
            {
                number.SetTailPoint(new SKPoint(
                    number.TailPoint.X + offsetX,
                    number.TailPoint.Y + offsetY));
            }

            // Update effect annotations with new bounds
            if (annotation is BaseEffectAnnotation effect)
            {
                effect.UpdateEffect(croppedBitmap);
            }

            // Update spotlight with new canvas size
            if (annotation is SpotlightAnnotation spotlight)
            {
                spotlight.CanvasSize = new SKSize(width, height);
            }
        }

        // Replace source image
        SourceImage.Dispose();
        SourceImage = croppedBitmap;
        CanvasSize = new SKSize(width, height);

        AnnotationsRestored?.Invoke();
        ImageChanged?.Invoke();
        HistoryChanged?.Invoke();
        InvalidateRequested?.Invoke();
        return true;
    }

    /// <summary>
    /// Perform cut out operation using the current cut out annotation
    /// </summary>
    public void PerformCutOut()
    {
        var cutOutAnnotation = _annotations.OfType<CutOutAnnotation>().FirstOrDefault();
        if (cutOutAnnotation == null || SourceImage == null) return;

        var bounds = cutOutAnnotation.GetBounds();

        // Create canvas memento before destructive cutout operation
        _history.CreateCanvasMemento();

        if (cutOutAnnotation.IsVertical)
        {
            // Vertical cut: remove a vertical strip and join left and right parts
            int cutX = (int)Math.Round(bounds.MidX);
            int cutWidth = (int)Math.Max(1, Math.Round(bounds.Width));

            // Clamp to image bounds
            if (cutX < 0) cutX = 0;
            if (cutX >= SourceImage.Width) cutX = SourceImage.Width - 1;

            // Ensure we don't cut past the end of the image
            int maxCutWidth = SourceImage.Width - cutX;
            if (cutWidth > maxCutWidth) cutWidth = maxCutWidth;

            if (cutWidth <= 0)
            {
                return;
            }

            int newWidth = SourceImage.Width - cutWidth;
            if (newWidth <= 0)
            {
                return;
            }

            var resultBitmap = new SKBitmap(newWidth, SourceImage.Height);
            using (var canvas = new SKCanvas(resultBitmap))
            {
                // Draw left part
                if (cutX > 0)
                {
                    var sourceRect = new SKRect(0, 0, cutX, SourceImage.Height);
                    var destRect = new SKRect(0, 0, cutX, SourceImage.Height);
                    canvas.DrawBitmap(SourceImage, sourceRect, destRect);
                }

                // Draw right part
                int rightStart = cutX + cutWidth;
                if (rightStart < SourceImage.Width)
                {
                    var sourceRect = new SKRect(rightStart, 0, SourceImage.Width, SourceImage.Height);
                    var destRect = new SKRect(cutX, 0, newWidth, SourceImage.Height);
                    canvas.DrawBitmap(SourceImage, sourceRect, destRect);
                }
            }

            SourceImage.Dispose();
            SourceImage = resultBitmap;
            CanvasSize = new SKSize(newWidth, SourceImage.Height);

            // Adjust annotations for vertical cut
            AdjustAnnotationsForVerticalCut(cutX, cutWidth, newWidth);
        }
        else
        {
            // Horizontal cut: remove a horizontal strip and join top and bottom parts
            int cutY = (int)Math.Round(bounds.MidY);
            int cutHeight = (int)Math.Max(1, Math.Round(bounds.Height));

            // Clamp to image bounds
            if (cutY < 0) cutY = 0;
            if (cutY >= SourceImage.Height) cutY = SourceImage.Height - 1;

            // Ensure we don't cut past the end of the image
            int maxCutHeight = SourceImage.Height - cutY;
            if (cutHeight > maxCutHeight) cutHeight = maxCutHeight;

            if (cutHeight <= 0)
            {
                return;
            }

            int newHeight = SourceImage.Height - cutHeight;
            if (newHeight <= 0)
            {
                return;
            }

            var resultBitmap = new SKBitmap(SourceImage.Width, newHeight);
            using (var canvas = new SKCanvas(resultBitmap))
            {
                // Draw top part
                if (cutY > 0)
                {
                    var sourceRect = new SKRect(0, 0, SourceImage.Width, cutY);
                    var destRect = new SKRect(0, 0, SourceImage.Width, cutY);
                    canvas.DrawBitmap(SourceImage, sourceRect, destRect);
                }

                // Draw bottom part
                int bottomStart = cutY + cutHeight;
                if (bottomStart < SourceImage.Height)
                {
                    var sourceRect = new SKRect(0, bottomStart, SourceImage.Width, SourceImage.Height);
                    var destRect = new SKRect(0, cutY, SourceImage.Width, newHeight);
                    canvas.DrawBitmap(SourceImage, sourceRect, destRect);
                }
            }

            SourceImage.Dispose();
            SourceImage = resultBitmap;
            CanvasSize = new SKSize(SourceImage.Width, newHeight);

            // Adjust annotations for horizontal cut
            AdjustAnnotationsForHorizontalCut(cutY, cutHeight, newHeight);
        }

        // Remove cutout annotation
        _annotations.Remove(cutOutAnnotation);

        ImageChanged?.Invoke();
        InvalidateRequested?.Invoke();
    }

    private void AdjustAnnotationsForVerticalCut(int cutX, int cutWidth, int newWidth)
    {
        int cutEnd = cutX + cutWidth;

        // Process annotations in reverse to allow safe removal
        for (int i = _annotations.Count - 1; i >= 0; i--)
        {
            var annotation = _annotations[i];
            var bounds = annotation.GetBounds();

            // Remove annotations completely within the cut area
            if (bounds.Left >= cutX && bounds.Right <= cutEnd)
            {
                _annotations.RemoveAt(i);
                continue;
            }

            // Adjust annotations that cross or are to the right of the cut
            bool needsAdjustment = false;
            float offsetX = 0;

            // Annotations to the right of the cut area: shift left by cutWidth
            if (bounds.Left >= cutEnd)
            {
                offsetX = -cutWidth;
                needsAdjustment = true;
            }
            // Annotations that span across the cut: shift right portion left
            else if (bounds.Right > cutEnd)
            {
                offsetX = -cutWidth;
                needsAdjustment = true;
                // Clamp the left edge to not go into the cut area
                if (annotation.StartPoint.X > cutX && annotation.StartPoint.X < cutEnd)
                {
                    annotation.StartPoint = new SKPoint(cutX, annotation.StartPoint.Y);
                }
                if (annotation.EndPoint.X > cutX && annotation.EndPoint.X < cutEnd)
                {
                    annotation.EndPoint = new SKPoint(cutX, annotation.EndPoint.Y);
                }
            }

            if (needsAdjustment)
            {
                annotation.StartPoint = new SKPoint(annotation.StartPoint.X + offsetX, annotation.StartPoint.Y);
                annotation.EndPoint = new SKPoint(annotation.EndPoint.X + offsetX, annotation.EndPoint.Y);

                // Handle freehand annotations
                if (annotation is FreehandAnnotation freehand)
                {
                    for (int j = 0; j < freehand.Points.Count; j++)
                    {
                        var pt = freehand.Points[j];
                        if (pt.X >= cutEnd)
                        {
                            freehand.Points[j] = new SKPoint(pt.X - cutWidth, pt.Y);
                        }
                        else if (pt.X > cutX)
                        {
                            freehand.Points[j] = new SKPoint(cutX, pt.Y);
                        }
                    }
                }

                // XIP0039 Guardrail 2: Adjust SpeechBalloon tail for vertical cut
                if (annotation is SpeechBalloonAnnotation balloon)
                {
                    var tailPoint = balloon.GetEffectiveTailPoint();
                    float tailX = tailPoint.X;
                    if (tailX >= cutEnd)
                        tailX -= cutWidth;
                    else if (tailX > cutX)
                        tailX = cutX;
                    balloon.SetTailPoint(new SKPoint(tailX, tailPoint.Y));
                }
                else if (annotation is NumberAnnotation number && number.HasTailPoint)
                {
                    float tailX = number.TailPoint.X;
                    if (tailX >= cutEnd)
                        tailX -= cutWidth;
                    else if (tailX > cutX)
                        tailX = cutX;
                    number.SetTailPoint(new SKPoint(tailX, number.TailPoint.Y));
                }

                // Update effect annotations
                if (annotation is BaseEffectAnnotation effect && SourceImage != null)
                {
                    effect.UpdateEffect(SourceImage);
                }
            }
        }
    }

    private void AdjustAnnotationsForHorizontalCut(int cutY, int cutHeight, int newHeight)
    {
        int cutEnd = cutY + cutHeight;

        // Process annotations in reverse to allow safe removal
        for (int i = _annotations.Count - 1; i >= 0; i--)
        {
            var annotation = _annotations[i];
            var bounds = annotation.GetBounds();

            // Remove annotations completely within the cut area
            if (bounds.Top >= cutY && bounds.Bottom <= cutEnd)
            {
                _annotations.RemoveAt(i);
                continue;
            }

            // Adjust annotations that cross or are below the cut
            bool needsAdjustment = false;
            float offsetY = 0;

            // Annotations below the cut area: shift up by cutHeight
            if (bounds.Top >= cutEnd)
            {
                offsetY = -cutHeight;
                needsAdjustment = true;
            }
            // Annotations that span across the cut: shift bottom portion up
            else if (bounds.Bottom > cutEnd)
            {
                offsetY = -cutHeight;
                needsAdjustment = true;
                // Clamp the top edge to not go into the cut area
                if (annotation.StartPoint.Y > cutY && annotation.StartPoint.Y < cutEnd)
                {
                    annotation.StartPoint = new SKPoint(annotation.StartPoint.X, cutY);
                }
                if (annotation.EndPoint.Y > cutY && annotation.EndPoint.Y < cutEnd)
                {
                    annotation.EndPoint = new SKPoint(annotation.EndPoint.X, cutY);
                }
            }

            if (needsAdjustment)
            {
                annotation.StartPoint = new SKPoint(annotation.StartPoint.X, annotation.StartPoint.Y + offsetY);
                annotation.EndPoint = new SKPoint(annotation.EndPoint.X, annotation.EndPoint.Y + offsetY);

                // Handle freehand annotations
                if (annotation is FreehandAnnotation freehand)
                {
                    for (int j = 0; j < freehand.Points.Count; j++)
                    {
                        var pt = freehand.Points[j];
                        if (pt.Y >= cutEnd)
                        {
                            freehand.Points[j] = new SKPoint(pt.X, pt.Y - cutHeight);
                        }
                        else if (pt.Y > cutY)
                        {
                            freehand.Points[j] = new SKPoint(pt.X, cutY);
                        }
                    }
                }

                // XIP0039 Guardrail 2: Adjust SpeechBalloon tail for horizontal cut
                if (annotation is SpeechBalloonAnnotation balloon)
                {
                    var tailPoint = balloon.GetEffectiveTailPoint();
                    float tailY = tailPoint.Y;
                    if (tailY >= cutEnd)
                        tailY -= cutHeight;
                    else if (tailY > cutY)
                        tailY = cutY;
                    balloon.SetTailPoint(new SKPoint(tailPoint.X, tailY));
                }
                else if (annotation is NumberAnnotation number && number.HasTailPoint)
                {
                    float tailY = number.TailPoint.Y;
                    if (tailY >= cutEnd)
                        tailY -= cutHeight;
                    else if (tailY > cutY)
                        tailY = cutY;
                    number.SetTailPoint(new SKPoint(number.TailPoint.X, tailY));
                }

                // Update effect annotations
                if (annotation is BaseEffectAnnotation effect && SourceImage != null)
                {
                    effect.UpdateEffect(SourceImage);
                }
            }
        }
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Dispose editor resources
    /// </summary>
    public void Dispose()
    {
        _history?.Dispose();
        SourceImage?.Dispose();
    }

    #endregion
}