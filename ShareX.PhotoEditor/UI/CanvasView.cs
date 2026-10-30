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
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using ShareX.PhotoEditor.Tools;

namespace ShareX.PhotoEditor.UI;

/// <summary>Displays the active document with zoom, pan and view rotation, draws overlays and routes input to tools.</summary>
public sealed class CanvasView : Control
{
    private readonly PhotoEditorWindow _editor;
    private readonly DispatcherTimer _antsTimer;
    private PixelSurface? _outlineSource;
    private List<(SKPoint A, SKPoint B)>? _outline;
    private Point? _panLast;
    private bool _spaceHand;
    private bool _toolPointerDown;
    private WriteableBitmap? _checker;
    private int _checkerSize;
    private SKPoint _lastDocPoint;

    public CanvasView(PhotoEditorWindow editor)
    {
        _editor = editor;
        Focusable = true;
        ClipToBounds = true;
        _antsTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
        _antsTimer.Tick += (_, _) =>
        {
            DocumentView? view = _editor.View;
            if (view?.Document.Selection == null || !_editor.Settings.ShowSelectionEdges || view.Document.QuickMask) return;
            view.AntsPhase = (view.AntsPhase + 1) % 8;
            InvalidateVisual();
        };
        _antsTimer.Start();
        DragDrop.SetAllowDrop(this, true);
    }

    /// <summary>Guide being dragged out of a ruler, in document coordinates.</summary>
    public Guide? PendingGuide { get; set; }

    public SKPoint LastDocPoint => _lastDocPoint;

    public bool SpaceHeld
    {
        get => _spaceHand;
        set
        {
            _spaceHand = value;
            UpdateCursor();
        }
    }

    private void UpdateViewport(DocumentView view, Size size)
    {
        view.Viewport = size;
        view.RenderScaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_editor.View is DocumentView view)
        {
            UpdateViewport(view, finalSize);
            if (view.NeedsFit) view.FitOnScreen();
        }
        return base.ArrangeOverride(finalSize);
    }

    private IBrush CheckerBrush(double zoom)
    {
        int size = Math.Max(2, _editor.Settings.CheckerSize);
        if (_checker == null || _checkerSize != size)
        {
            _checker?.Dispose();
            _checkerSize = size;
            _checker = new WriteableBitmap(new PixelSize(size * 2, size * 2), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
            using ILockedFramebuffer buffer = _checker.Lock();
            unsafe
            {
                uint* p = (uint*)buffer.Address;
                int stride = buffer.RowBytes / 4;
                for (int y = 0; y < size * 2; y++)
                    for (int x = 0; x < size * 2; x++)
                        p[y * stride + x] = (x / size + y / size) % 2 == 0 ? 0xFFFFFFFFu : 0xFFCCCCCCu;
            }
        }
        double tile = size * 2 / zoom;
        return new ImageBrush(_checker)
        {
            TileMode = TileMode.Tile,
            Stretch = Stretch.Fill,
            DestinationRect = new RelativeRect(0, 0, tile, tile, RelativeUnit.Absolute)
        };
    }

    public override void Render(DrawingContext context)
    {
        context.FillRectangle(new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28)), new Rect(Bounds.Size));
        DocumentView? view = _editor.View;
        if (view == null)
        {
            FormattedText hint = new("Open or create a document to start editing (Ctrl+O / Ctrl+N), or drop an image here.",
                System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 14, Brushes.Gray);
            context.DrawText(hint, new Point((Bounds.Width - hint.Width) / 2, (Bounds.Height - hint.Height) / 2));
            return;
        }
        UpdateViewport(view, Bounds.Size);
        view.Update(true);
        PhotoDocument doc = view.Document;
        Rect docRect = new(0, 0, doc.Width, doc.Height);

        using (context.PushTransform(view.DocToScreenMatrix))
        {
            context.DrawRectangle(new SolidColorBrush(Color.FromArgb(90, 0, 0, 0)), null, docRect.Inflate(3 / view.Zoom));
            context.FillRectangle(CheckerBrush(view.Zoom), docRect);
            if (view.Display != null)
            {
                BitmapInterpolationMode mode = view.Zoom >= 2 ? BitmapInterpolationMode.None : view.Zoom >= 1 ? BitmapInterpolationMode.LowQuality : BitmapInterpolationMode.HighQuality;
                using (context.PushRenderOptions(new RenderOptions { BitmapInterpolationMode = mode }))
                {
                    context.DrawImage(view.Display, docRect, docRect);
                }
            }
            if (_editor.Settings.ShowPixelGrid && view.Zoom >= 8) DrawPixelGrid(context, view);
        }

        OverlayContext overlay = new(context, view);
        DrawArtboards(overlay, doc);
        if (_editor.Settings.ShowGrid) DrawGrid(context, view);
        if (_editor.Settings.ShowSlices) DrawSlices(overlay, doc);
        if (_editor.Settings.ShowGuides) DrawGuides(context, view, doc);
        if (_editor.Settings.ShowSelectionEdges && doc.Selection != null && !doc.QuickMask) DrawSelection(context, view, doc.Selection);
        DrawFrames(overlay, doc);
        if (_editor.Settings.ShowNotes) DrawNotes(overlay, doc);
        if (_editor.Settings.ShowCount) DrawCounts(context, view, doc);
        DrawSamplers(overlay, doc);
        if (_editor.Tools.Active is not PathToolBase && doc.ActivePath != null && doc.ActiveLayer is not ShapeLayer)
        {
            using SKPath path = doc.ActivePath.Path.ToSKPath();
            overlay.Path(path, new Pen(new SolidColorBrush(Color.FromArgb(200, 120, 160, 255)), 1));
        }
        if (_editor.Tools.Active is not PathToolBase && doc.ActiveLayer is ShapeLayer shape && _editor.Tools.Active is MoveTool)
        {
            using SKPath path = shape.Path.ToSKPath();
            overlay.Path(path, new Pen(new SolidColorBrush(Color.FromArgb(120, 120, 160, 255)), 1));
        }
        _editor.Tools.Active.DrawOverlay(overlay);
        if (PendingGuide is Guide pending) DrawGuide(context, view, doc, pending, true);
    }

    private static void DrawPixelGrid(DrawingContext context, DocumentView view)
    {
        PhotoDocument doc = view.Document;
        Matrix inverse = view.DocToScreenMatrix.Invert();
        Rect visible = new Rect(view.Viewport).TransformToAABB(inverse).Intersect(new Rect(0, 0, doc.Width, doc.Height));
        Pen pen = new(new SolidColorBrush(Color.FromArgb(40, 128, 128, 128)), 1 / view.Zoom);
        for (int x = (int)Math.Floor(visible.Left); x <= (int)Math.Ceiling(visible.Right); x++) context.DrawLine(pen, new Point(x, visible.Top), new Point(x, visible.Bottom));
        for (int y = (int)Math.Floor(visible.Top); y <= (int)Math.Ceiling(visible.Bottom); y++) context.DrawLine(pen, new Point(visible.Left, y), new Point(visible.Right, y));
    }

    private void DrawGrid(DrawingContext context, DocumentView view)
    {
        PhotoDocument doc = view.Document;
        float spacing = Math.Max(1, _editor.Settings.GridSpacing);
        int subdivisions = Math.Max(1, _editor.Settings.GridSubdivisions);
        Pen major = new(new SolidColorBrush(Color.FromArgb(150, 128, 128, 128)), 1);
        Pen minor = new(new SolidColorBrush(Color.FromArgb(60, 128, 128, 128)), 1);
        float step = spacing / subdivisions;
        if (step * view.Zoom < 4) step = spacing;
        int index = 0;
        for (float x = 0; x <= doc.Width; x += step, index++)
            context.DrawLine(index % subdivisions == 0 ? major : minor, view.DocToScreen(new SKPoint(x, 0)), view.DocToScreen(new SKPoint(x, doc.Height)));
        index = 0;
        for (float y = 0; y <= doc.Height; y += step, index++)
            context.DrawLine(index % subdivisions == 0 ? major : minor, view.DocToScreen(new SKPoint(0, y)), view.DocToScreen(new SKPoint(doc.Width, y)));
    }

    private void DrawGuides(DrawingContext context, DocumentView view, PhotoDocument doc)
    {
        foreach (Guide guide in doc.Guides) DrawGuide(context, view, doc, guide, false);
    }

    private void DrawGuide(DrawingContext context, DocumentView view, PhotoDocument doc, Guide guide, bool pending)
    {
        SKColor c = EditorSettings.ParseColor(_editor.Settings.GuideColor, new SKColor(0, 200, 255));
        Pen pen = new(new SolidColorBrush(Color.FromArgb(pending ? (byte)160 : (byte)255, c.Red, c.Green, c.Blue)), 1);
        float extent = 100000;
        if (guide.Horizontal) context.DrawLine(pen, view.DocToScreen(new SKPoint(-extent, guide.Position)), view.DocToScreen(new SKPoint(extent, guide.Position)));
        else context.DrawLine(pen, view.DocToScreen(new SKPoint(guide.Position, -extent)), view.DocToScreen(new SKPoint(guide.Position, extent)));
    }

    private void DrawSelection(DrawingContext context, DocumentView view, PixelSurface selection)
    {
        if (!ReferenceEquals(_outlineSource, selection))
        {
            _outlineSource = selection;
            _outline = MaskOps.TraceOutline(selection);
        }
        if (_outline == null) return;
        Pen black = new(Brushes.Black, 1);
        Pen white = new(Brushes.White, 1, new DashStyle([4, 4], view.AntsPhase));
        Matrix m = view.DocToScreenMatrix;
        foreach (var (a, b) in _outline)
        {
            Point pa = new Point(a.X, a.Y).Transform(m), pb = new Point(b.X, b.Y).Transform(m);
            context.DrawLine(black, pa, pb);
            context.DrawLine(white, pa, pb);
        }
    }

    private static void DrawArtboards(OverlayContext overlay, PhotoDocument doc)
    {
        foreach (GroupLayer artboard in doc.AllLayers.OfType<GroupLayer>().Where(g => g.IsArtboard))
        {
            SKRectI b = artboard.ArtboardBounds!.Value;
            overlay.Rect(SKRect.Create(b.Left, b.Top, b.Width, b.Height), new Pen(new SolidColorBrush(Color.FromArgb(120, 200, 200, 200)), 1));
            Point p = overlay.ToScreen(new SKPoint(b.Left, b.Top));
            FormattedText text = new(artboard.Name, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 11,
                artboard == doc.ActiveLayer ? OverlayContext.AccentBrush : Brushes.LightGray);
            overlay.Drawing.DrawText(text, new Point(p.X, p.Y - 16));
        }
    }

    private static void DrawFrames(OverlayContext overlay, PhotoDocument doc)
    {
        foreach (FrameLayer frame in doc.AllLayers.OfType<FrameLayer>().Where(f => f.IsEffectivelyVisible))
        {
            bool active = frame == doc.ActiveLayer;
            if (frame.Content == null)
            {
                // Empty frames show a placeholder cross.
                Pen pen = new(new SolidColorBrush(Color.FromArgb(160, 160, 160, 160)), 1);
                if (frame.Ellipse) overlay.Ellipse(frame.Frame, pen);
                else overlay.Rect(frame.Frame, pen);
                overlay.Line(new SKPoint(frame.Frame.Left, frame.Frame.Top), new SKPoint(frame.Frame.Right, frame.Frame.Bottom), pen);
                overlay.Line(new SKPoint(frame.Frame.Right, frame.Frame.Top), new SKPoint(frame.Frame.Left, frame.Frame.Bottom), pen);
            }
            else if (active)
            {
                if (frame.Ellipse) overlay.Ellipse(frame.Frame, OverlayContext.AccentPen);
                else overlay.Rect(frame.Frame, OverlayContext.AccentPen);
            }
        }
    }

    private static void DrawSlices(OverlayContext overlay, PhotoDocument doc)
    {
        int index = 1;
        foreach (SliceItem slice in doc.Slices)
        {
            SKRectI b = slice.Bounds;
            overlay.Rect(SKRect.Create(b.Left, b.Top, b.Width, b.Height), new Pen(new SolidColorBrush(Color.FromRgb(0x3E, 0x83, 0xF2)), 1));
            overlay.Text(new SKPoint(b.Left, b.Top), $"{index++:00}", 2, 2, new SolidColorBrush(Color.FromRgb(0x3E, 0x83, 0xF2)));
        }
    }

    private static void DrawNotes(OverlayContext overlay, PhotoDocument doc)
    {
        foreach (NoteItem note in doc.Notes)
        {
            Point p = overlay.ToScreen(note.Position);
            Rect r = new(p.X, p.Y, 16, 18);
            overlay.Drawing.DrawRectangle(new SolidColorBrush(Color.FromRgb(note.Color.Red, note.Color.Green, note.Color.Blue)), new Pen(Brushes.Black, 1), r);
            overlay.Drawing.DrawLine(new Pen(Brushes.Black, 1), new Point(p.X + 3, p.Y + 6), new Point(p.X + 13, p.Y + 6));
            overlay.Drawing.DrawLine(new Pen(Brushes.Black, 1), new Point(p.X + 3, p.Y + 10), new Point(p.X + 11, p.Y + 10));
            if (!string.IsNullOrEmpty(note.Text) && overlay.Zoom >= 0.25)
            {
                string preview = note.Text.Length > 40 ? note.Text[..40] + "…" : note.Text;
                overlay.Text(note.Position, preview.Replace('\n', ' '), 20, 0);
            }
        }
    }

    private static void DrawCounts(DrawingContext context, DocumentView view, PhotoDocument doc)
    {
        Dictionary<int, int> numbers = [];
        foreach (CountItem item in doc.Counts)
        {
            CountGroup group = doc.CountGroups[Math.Clamp(item.Group, 0, doc.CountGroups.Count - 1)];
            if (!group.Visible) continue;
            numbers[item.Group] = numbers.GetValueOrDefault(item.Group) + 1;
            Point p = view.DocToScreen(item.Position);
            IBrush brush = new SolidColorBrush(Color.FromRgb(group.Color.Red, group.Color.Green, group.Color.Blue));
            context.DrawEllipse(brush, new Pen(Brushes.White, 1), p, group.MarkerSize, group.MarkerSize);
            FormattedText text = new(numbers[item.Group].ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Bold), group.LabelSize, brush);
            context.DrawText(text, new Point(p.X + group.MarkerSize + 2, p.Y - text.Height / 2));
        }
    }

    private static void DrawSamplers(OverlayContext overlay, PhotoDocument doc)
    {
        int index = 1;
        foreach (ColorSamplerPoint sampler in doc.Samplers)
        {
            Point p = overlay.ToScreen(sampler.Position);
            Pen pen = new(Brushes.White, 1);
            overlay.Drawing.DrawEllipse(null, new Pen(Brushes.Black, 3), p, 6, 6);
            overlay.Drawing.DrawEllipse(null, pen, p, 6, 6);
            overlay.Drawing.DrawLine(pen, new Point(p.X - 10, p.Y), new Point(p.X + 10, p.Y));
            overlay.Drawing.DrawLine(pen, new Point(p.X, p.Y - 10), new Point(p.X, p.Y + 10));
            FormattedText text = new((index++).ToString(), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 11, Brushes.White);
            overlay.Drawing.DrawText(text, new Point(p.X + 8, p.Y + 4));
        }
    }

    public void InvalidateOutline() => _outlineSource = null;

    private ToolInput CreateInput(PointerEventArgs e, bool pressed, int clickCount = 1)
    {
        DocumentView view = _editor.View!;
        PointerPoint point = e.GetCurrentPoint(this);
        Point screen = point.Position;
        SKPoint doc = view.ScreenToDoc(screen);
        doc = _editor.Snap(doc);
        _lastDocPoint = doc;
        float pressure = point.Pointer.Type == PointerType.Pen ? Math.Clamp(point.Properties.Pressure, 0.01f, 1) : 1;
        return new ToolInput
        {
            Doc = doc,
            Screen = screen,
            Pressure = pressure,
            LeftButton = point.Properties.IsLeftButtonPressed || !pressed && point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased,
            RightButton = point.Properties.IsRightButtonPressed,
            IsPressed = pressed,
            Modifiers = e.KeyModifiers,
            ClickCount = clickCount
        };
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Focus();
        if (_editor.View == null) return;
        PointerPoint point = e.GetCurrentPoint(this);
        if (point.Properties.IsMiddleButtonPressed || SpaceHeld && point.Properties.IsLeftButtonPressed)
        {
            _panLast = point.Position;
            e.Pointer.Capture(this);
            Cursor = new Cursor(StandardCursorType.SizeAll);
            return;
        }
        if (point.Properties.IsRightButtonPressed)
        {
            _editor.ShowCanvasContextMenu(this, point.Position);
            return;
        }
        e.Pointer.Capture(this);
        _toolPointerDown = true;
        ToolInput input = CreateInput(e, true, e.ClickCount);
        _editor.Tools.Active.PointerDown(input);
        _editor.UpdateStatusForPointer(input.Doc);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_editor.View == null) return;
        if (_panLast is Point last)
        {
            Point current = e.GetPosition(this);
            _editor.View.Pan += current - last;
            _panLast = current;
            _editor.InvalidateOverlay();
            return;
        }
        ToolInput input = CreateInput(e, _toolPointerDown);
        if (PendingGuide != null) return;
        _editor.Tools.Active.PointerMove(input);
        _editor.UpdateStatusForPointer(input.Doc);
        UpdateCursor();
        if (_editor.Tools.Active.BrushOutlineSize > 0) InvalidateVisual();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Pointer.Capture(null);
        if (_panLast != null)
        {
            _panLast = null;
            UpdateCursor();
            return;
        }
        if (_editor.View == null || !_toolPointerDown) return;
        _toolPointerDown = false;
        _editor.Tools.Active.PointerUp(CreateInput(e, false));
        _editor.RefreshInfo();
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        DocumentView? view = _editor.View;
        if (view == null) return;
        Point position = e.GetPosition(this);
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control) || e.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            view.ZoomAt(view.Zoom * Math.Pow(1.2, e.Delta.Y), position);
        }
        else if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            view.Pan += new Vector(e.Delta.Y * 60, 0);
        }
        else
        {
            view.Pan += new Vector(e.Delta.X * 60, e.Delta.Y * 60);
        }
        e.Handled = true;
        _editor.InvalidateOverlay();
        _editor.RefreshStatus();
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        InvalidateVisual();
    }

    public void UpdateCursor()
    {
        if (SpaceHeld)
        {
            Cursor = new Cursor(StandardCursorType.Hand);
            return;
        }
        ToolBase tool = _editor.Tools.Active;
        if (tool.BrushOutlineSize > 0 && _editor.View != null && tool.BrushOutlineSize * _editor.View.Zoom >= 3)
        {
            Cursor = new Cursor(StandardCursorType.None);
            return;
        }
        Cursor = new Cursor(tool.Cursor switch
        {
            ToolCursor.Arrow => StandardCursorType.Arrow,
            ToolCursor.Move or ToolCursor.SizeAll => StandardCursorType.SizeAll,
            ToolCursor.Hand => StandardCursorType.Hand,
            ToolCursor.Grab => StandardCursorType.DragMove,
            ToolCursor.Text => StandardCursorType.Ibeam,
            ToolCursor.NotAllowed => StandardCursorType.No,
            ToolCursor.Bucket or ToolCursor.Eyedropper or ToolCursor.Pen => StandardCursorType.Cross,
            ToolCursor.ZoomIn or ToolCursor.ZoomOut => StandardCursorType.Cross,
            ToolCursor.Rotate => StandardCursorType.Hand,
            _ => StandardCursorType.Cross
        });
    }
}

/// <summary>Horizontal or vertical ruler showing document coordinates. Dragging from it creates a guide.</summary>
public sealed class RulerControl : Control
{
    private readonly PhotoEditorWindow _editor;
    private readonly bool _horizontal;

    public RulerControl(PhotoEditorWindow editor, bool horizontal)
    {
        _editor = editor;
        _horizontal = horizontal;
        if (horizontal) Height = 18;
        else Width = 18;
        Cursor = new Cursor(horizontal ? StandardCursorType.SizeNorthSouth : StandardCursorType.SizeWestEast);
    }

    public override void Render(DrawingContext context)
    {
        context.FillRectangle(new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x30)), new Rect(Bounds.Size));
        DocumentView? view = _editor.View;
        if (view == null) return;
        Pen pen = new(new SolidColorBrush(Color.FromRgb(0x90, 0x90, 0x90)), 1);
        double zoom = view.Zoom;
        double[] steps = [1, 2, 5, 10, 20, 25, 50, 100, 200, 250, 500, 1000, 2000, 5000, 10000];
        double major = steps.FirstOrDefault(s => s * zoom >= 60, 10000);
        double minor = major / (major % 5 == 0 ? 5 : 2);
        // Ruler origin is the document's top-left corner, ignoring view rotation.
        Point origin = view.DocToScreen(new SKPoint(0, 0));
        Point canvasOffset = _editor.CanvasOffsetInRuler(this);
        double length = _horizontal ? Bounds.Width : Bounds.Height;
        double start0 = (_horizontal ? origin.X : origin.Y) + (_horizontal ? canvasOffset.X : canvasOffset.Y);
        double firstValue = Math.Floor(-start0 / zoom / minor) * minor;
        for (double value = firstValue; ; value += minor)
        {
            double position = start0 + value * zoom;
            if (position > length) break;
            bool isMajor = Math.Abs(value / major - Math.Round(value / major)) < 1e-6;
            double tick = isMajor ? 18 : 5;
            if (_horizontal)
            {
                context.DrawLine(pen, new Point(position, 18 - tick), new Point(position, 18));
                if (isMajor) context.DrawText(new FormattedText(value.ToString("0"), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 9, Brushes.LightGray), new Point(position + 2, 0));
            }
            else
            {
                context.DrawLine(pen, new Point(18 - tick, position), new Point(18, position));
                if (isMajor)
                {
                    FormattedText text = new(value.ToString("0"), System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 9, Brushes.LightGray);
                    using (context.PushTransform(Matrix.CreateRotation(-Math.PI / 2) * Matrix.CreateTranslation(1, position - 2)))
                    {
                        context.DrawText(text, new Point(0, 0));
                    }
                }
            }
        }
        // Pointer position marker.
        SKPoint doc = _editor.Canvas.LastDocPoint;
        double marker = start0 + (_horizontal ? doc.X : doc.Y) * zoom;
        Pen markerPen = new(OverlayContext.AccentBrush, 1);
        if (_horizontal) context.DrawLine(markerPen, new Point(marker, 0), new Point(marker, 18));
        else context.DrawLine(markerPen, new Point(0, marker), new Point(18, marker));
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (_editor.View == null || _editor.Settings.LockGuides) return;
        e.Pointer.Capture(this);
        UpdateGuide(e);
    }

    private void UpdateGuide(PointerEventArgs e)
    {
        Point p = e.GetPosition(_editor.Canvas);
        SKPoint doc = _editor.Snap(_editor.View!.ScreenToDoc(p));
        _editor.Canvas.PendingGuide = new Guide(_horizontal, MathF.Round(_horizontal ? doc.Y : doc.X));
        _editor.InvalidateOverlay();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_editor.Canvas.PendingGuide != null) UpdateGuide(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Pointer.Capture(null);
        if (_editor.Canvas.PendingGuide is not Guide guide || _editor.Document == null) return;
        _editor.Canvas.PendingGuide = null;
        Point p = e.GetPosition(_editor.Canvas);
        if (p.X >= 0 && p.Y >= 0 && p.X <= _editor.Canvas.Bounds.Width && p.Y <= _editor.Canvas.Bounds.Height)
        {
            _editor.Document.Guides.Add(guide);
            _editor.Document.History.Push("New Guide");
            _editor.Settings.ShowGuides = true;
            _editor.RefreshPanels();
        }
        _editor.InvalidateOverlay();
    }
}
