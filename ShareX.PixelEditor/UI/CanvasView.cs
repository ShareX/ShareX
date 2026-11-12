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
using Avalonia.Threading;
using ShareX.PhotoEditor.UI.Theme;
using ShareX.PixelEditor.Tools;

namespace ShareX.PixelEditor.UI;

/// <summary>Drawing helpers for tool overlays; positions are in sprite pixels.</summary>
public sealed class OverlayContext
{
    private static readonly Pen Dark = new(Brushes.Black, 1);
    private static readonly Pen Light = new(Brushes.White, 1, new DashStyle([4, 4], 0));

    public OverlayContext(DrawingContext drawing, SpriteView view)
    {
        Drawing = drawing;
        View = view;
    }

    public DrawingContext Drawing { get; }

    public SpriteView View { get; }

    public double Zoom => View.Zoom;

    private Point Snap(double x, double y)
    {
        Point p = View.ToScreen(x, y);
        return new Point(Math.Round(p.X) + 0.5, Math.Round(p.Y) + 0.5);
    }

    public void Line(double x0, double y0, double x1, double y1)
    {
        Point a = Snap(x0, y0), b = Snap(x1, y1);
        Drawing.DrawLine(Dark, a, b);
        Drawing.DrawLine(Light, a, b);
    }

    public void RectOutline(SKRectI rect)
    {
        Point a = Snap(rect.Left, rect.Top), b = Snap(rect.Right, rect.Bottom);
        Rect r = new(a, b);
        Drawing.DrawRectangle(null, Dark, r);
        Drawing.DrawRectangle(null, Light, r);
    }

    public void EllipseOutline(SKRectI rect)
    {
        Point a = Snap(rect.Left, rect.Top), b = Snap(rect.Right, rect.Bottom);
        Rect r = new(a, b);
        Drawing.DrawEllipse(null, Dark, r.Center, r.Width / 2, r.Height / 2);
        Drawing.DrawEllipse(null, Light, r.Center, r.Width / 2, r.Height / 2);
    }

    /// <summary>A path through pixel centers.</summary>
    public void PixelPath(IReadOnlyList<SKPointI> points, bool closed)
    {
        if (points.Count == 0) return;
        StreamGeometry geometry = new();
        using (StreamGeometryContext context = geometry.Open())
        {
            context.BeginFigure(Snap(points[0].X + 0.5, points[0].Y + 0.5), false);
            for (int i = 1; i < points.Count; i++) context.LineTo(Snap(points[i].X + 0.5, points[i].Y + 0.5));
            context.EndFigure(closed);
        }
        Drawing.DrawGeometry(null, Dark, geometry);
        Drawing.DrawGeometry(null, Light, geometry);
    }

    public void Handle(double x, double y, bool round)
    {
        Point p = Snap(x, y);
        Rect r = new(p.X - 4, p.Y - 4, 8, 8);
        if (round) Drawing.DrawEllipse(Brushes.White, Dark, p, 4.5, 4.5);
        else Drawing.DrawRectangle(Brushes.White, Dark, r);
    }

    public void Slice(Slice slice, bool selected)
    {
        Color color = Argb.ToAvalonia(slice.Color);
        Point a = Snap(slice.Bounds.Left, slice.Bounds.Top), b = Snap(slice.Bounds.Right, slice.Bounds.Bottom);
        Rect r = new(a, b);
        Drawing.DrawRectangle(new SolidColorBrush(color, selected ? 0.22 : 0.08), new Pen(new SolidColorBrush(color), selected ? 2 : 1), r);
        if (Zoom < 2 && !selected) return;
        FormattedText text = new(slice.Name, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 10, Brushes.White);
        Rect label = new(a.X, a.Y - text.Height - 2, text.Width + 6, text.Height + 2);
        Drawing.DrawRectangle(new SolidColorBrush(color), null, label);
        Drawing.DrawText(text, new Point(label.X + 3, label.Y + 1));
    }
}

/// <summary>Shows the active sprite with zoom and pan, draws grids, selection and tool overlays, and routes input to tools.</summary>
public sealed class CanvasView : Control
{
    private static readonly IBrush Pasteboard = new SolidColorBrush(Color.FromRgb(0x28, 0x28, 0x28));

    private readonly PixelEditorWindow _editor;
    private readonly DispatcherTimer _antsTimer;
    private SelectionMask? _outlineSource;
    private StreamGeometry? _outline;
    private IBrush? _checker;
    private (string, string, int, double) _checkerKey;
    private Point? _panLast;
    private bool _spaceHeld, _toolDown, _toolRight, _hovering;
    private SKPointI _hoverPixel;
    private SKPointI[]? _tipSource;
    private StreamGeometry? _tipOutline;

    public CanvasView(PixelEditorWindow editor)
    {
        _editor = editor;
        Focusable = true;
        ClipToBounds = true;
        _antsTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(160) };
        _antsTimer.Tick += (_, _) =>
        {
            SpriteView? view = _editor.View;
            if (view?.Sprite.Selection == null) return;
            view.AntsPhase = (view.AntsPhase + 1) % 8;
            InvalidateVisual();
        };
        _antsTimer.Start();
    }

    public void StopTimers() => _antsTimer.Stop();

    public bool IsToolPointerDown => _toolDown;

    public SKPointI HoverPixel => _hoverPixel;

    public bool SpaceHeld
    {
        get => _spaceHeld;
        set
        {
            _spaceHeld = value;
            UpdateCursor();
        }
    }

    public void UpdateCursor()
    {
        StandardCursorType type = _spaceHeld || _panLast != null ? StandardCursorType.Hand : _editor.Tools.Active.Cursor;
        Cursor = new Cursor(type);
    }

    private void UpdateViewport(SpriteView view, Size size)
    {
        view.Viewport = size;
        if (view.NeedsFit && size.Width > 0 && size.Height > 0)
        {
            view.Fit();
            Dispatcher.UIThread.Post(_editor.RefreshStatus);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_editor.View is SpriteView view) UpdateViewport(view, finalSize);
        return base.ArrangeOverride(finalSize);
    }

    public override void Render(DrawingContext context)
    {
        context.FillRectangle(Pasteboard, new Rect(Bounds.Size));
        SpriteView? view = _editor.View;
        if (view == null) return;
        EditorSettings settings = _editor.Settings;
        UpdateViewport(view, Bounds.Size);
        view.Update(settings);
        Sprite sprite = view.Sprite;
        Rect docRect = new(0, 0, sprite.Width, sprite.Height);
        bool tileX = settings.Tiled is TiledMode.Horizontal or TiledMode.Both, tileY = settings.Tiled is TiledMode.Vertical or TiledMode.Both;

        using (context.PushTransform(view.Matrix))
        {
            // The checker keeps its size on screen at every zoom level.
            var key = (settings.CheckerColor1, settings.CheckerColor2, settings.CheckerSize, view.Zoom);
            if (_checker == null || key != _checkerKey)
            {
                _checker = Ui.Checker(EditorSettings.ParseColor(settings.CheckerColor1, 0xFF8C8C8C), EditorSettings.ParseColor(settings.CheckerColor2, 0xFFA6A6A6), settings.CheckerSize / view.Zoom);
                _checkerKey = key;
            }
            Rect area = new(tileX ? -sprite.Width : 0, tileY ? -sprite.Height : 0, sprite.Width * (tileX ? 3 : 1), sprite.Height * (tileY ? 3 : 1));
            context.DrawRectangle(new SolidColorBrush(Color.FromArgb(110, 0, 0, 0)), null, area.Inflate(2 / view.Zoom));
            context.FillRectangle(_checker, area);
            if (view.Display != null)
            {
                BitmapInterpolationMode mode = view.Zoom >= 1 ? BitmapInterpolationMode.None : BitmapInterpolationMode.MediumQuality;
                using (context.PushRenderOptions(new RenderOptions { BitmapInterpolationMode = mode, EdgeMode = EdgeMode.Aliased }))
                {
                    for (int ty = tileY ? -1 : 0; ty <= (tileY ? 1 : 0); ty++)
                        for (int tx = tileX ? -1 : 0; tx <= (tileX ? 1 : 0); tx++)
                            context.DrawImage(view.Display, docRect, docRect.Translate(new Vector(tx * sprite.Width, ty * sprite.Height)));
                }
            }
        }

        Rect clip = new(Bounds.Size);
        if (settings.ShowPixelGrid && view.Zoom >= settings.PixelGridZoom) DrawGrid(context, view, 1, 1, new Pen(new SolidColorBrush(Color.FromArgb(46, 0, 0, 0)), 1), clip);
        if (settings.ShowGrid && settings.GridWidth * view.Zoom >= 4 && settings.GridHeight * view.Zoom >= 4)
        {
            Color grid = Argb.ToAvalonia(EditorSettings.ParseColor(settings.GridColor, 0xFF00A0FF));
            DrawGrid(context, view, settings.GridWidth, settings.GridHeight, new Pen(new SolidColorBrush(grid, 0.55), 1), clip);
        }
        if (tileX || tileY)
        {
            // Mark the real sprite among its repeats.
            Point a = view.ToScreen(0, 0), b = view.ToScreen(sprite.Width, sprite.Height);
            context.DrawRectangle(null, new Pen(new SolidColorBrush(Color.FromArgb(160, 255, 255, 255)), 1), new Rect(Math.Round(a.X) - 0.5, Math.Round(a.Y) - 0.5, Math.Round(b.X - a.X) + 1, Math.Round(b.Y - a.Y) + 1));
        }
        DrawSymmetry(context, view, settings);

        OverlayContext overlay = new(context, view);
        if (settings.ShowSlices)
            foreach (Slice slice in sprite.Slices) overlay.Slice(slice, false);
        if (sprite.Selection != null) DrawSelection(context, view, sprite.Selection);
        _editor.Tools.Active.DrawOverlay(overlay);
        if (_hovering && !_spaceHeld && _panLast == null && settings.ShowBrushPreview && _editor.Tools.Active.PreviewTip is SKPointI[] tip) DrawBrushPreview(context, view, tip, settings);
    }

    private static void DrawGrid(DrawingContext context, SpriteView view, int cellW, int cellH, Pen pen, Rect clip)
    {
        Sprite sprite = view.Sprite;
        (double left, double top) = view.ToDocument(clip.TopLeft);
        (double right, double bottom) = view.ToDocument(clip.BottomRight);
        int x0 = Math.Max(0, (int)Math.Floor(left / cellW) * cellW), x1 = Math.Min(sprite.Width, (int)Math.Ceiling(right));
        int y0 = Math.Max(0, (int)Math.Floor(top / cellH) * cellH), y1 = Math.Min(sprite.Height, (int)Math.Ceiling(bottom));
        double sy0 = view.ToScreen(0, Math.Max(0, top)).Y, sy1 = view.ToScreen(0, Math.Min(sprite.Height, bottom)).Y;
        double sx0 = view.ToScreen(Math.Max(0, left), 0).X, sx1 = view.ToScreen(Math.Min(sprite.Width, right), 0).X;
        for (int x = x0; x <= x1; x += cellW)
        {
            double sx = Math.Round(view.ToScreen(x, 0).X) + 0.5;
            context.DrawLine(pen, new Point(sx, sy0), new Point(sx, sy1));
        }
        for (int y = y0; y <= y1; y += cellH)
        {
            double sy = Math.Round(view.ToScreen(0, y).Y) + 0.5;
            context.DrawLine(pen, new Point(sx0, sy), new Point(sx1, sy));
        }
    }

    private void DrawSymmetry(DrawingContext context, SpriteView view, EditorSettings settings)
    {
        if (settings.Symmetry == SymmetryMode.None) return;
        Pen pen = new(new SolidColorBrush(PhotoEditorTheme.AccentBorderBrush.Color, 0.9), 1, new DashStyle([6, 3], 0));
        if (settings.Symmetry is SymmetryMode.Horizontal or SymmetryMode.Both)
        {
            double x = Math.Round(view.ToScreen(view.AxisX2 / 2.0, 0).X) + 0.5;
            context.DrawLine(pen, new Point(x, 0), new Point(x, Bounds.Height));
        }
        if (settings.Symmetry is SymmetryMode.Vertical or SymmetryMode.Both)
        {
            double y = Math.Round(view.ToScreen(0, view.AxisY2 / 2.0).Y) + 0.5;
            context.DrawLine(pen, new Point(0, y), new Point(Bounds.Width, y));
        }
    }

    private void DrawSelection(DrawingContext context, SpriteView view, SelectionMask selection)
    {
        if (!ReferenceEquals(_outlineSource, selection))
        {
            _outlineSource = selection;
            StreamGeometry geometry = new();
            using (StreamGeometryContext g = geometry.Open())
            {
                foreach ((int x0, int y0, int x1, int y1) in selection.Outline())
                {
                    g.BeginFigure(new Point(x0, y0), false);
                    g.LineTo(new Point(x1, y1));
                    g.EndFigure(false);
                }
            }
            _outline = geometry;
        }
        if (_outline == null) return;
        double thickness = 1 / view.Zoom;
        using (context.PushTransform(view.Matrix))
        {
            context.DrawGeometry(null, new Pen(Brushes.Black, thickness), _outline);
            context.DrawGeometry(null, new Pen(Brushes.White, thickness, new DashStyle([4, 4], view.AntsPhase)), _outline);
        }
    }

    private void DrawBrushPreview(DrawingContext context, SpriteView view, SKPointI[] tip, EditorSettings settings)
    {
        if (!ReferenceEquals(tip, _tipSource) && (_tipSource == null || !_tipSource.AsSpan().SequenceEqual(tip)))
        {
            HashSet<SKPointI> set = [.. tip];
            StreamGeometry geometry = new();
            using (StreamGeometryContext g = geometry.Open())
            {
                void Edge(int x0, int y0, int x1, int y1)
                {
                    g.BeginFigure(new Point(x0, y0), false);
                    g.LineTo(new Point(x1, y1));
                    g.EndFigure(false);
                }
                foreach (SKPointI p in tip)
                {
                    if (!set.Contains(new SKPointI(p.X, p.Y - 1))) Edge(p.X, p.Y, p.X + 1, p.Y);
                    if (!set.Contains(new SKPointI(p.X, p.Y + 1))) Edge(p.X, p.Y + 1, p.X + 1, p.Y + 1);
                    if (!set.Contains(new SKPointI(p.X - 1, p.Y))) Edge(p.X, p.Y, p.X, p.Y + 1);
                    if (!set.Contains(new SKPointI(p.X + 1, p.Y))) Edge(p.X + 1, p.Y, p.X + 1, p.Y + 1);
                }
            }
            _tipOutline = geometry;
        }
        _tipSource = tip;
        if (_tipOutline == null) return;
        double thickness = 1 / view.Zoom;
        Pen dark = new(new SolidColorBrush(Colors.Black, 0.8), thickness), light = new(new SolidColorBrush(Colors.White, 0.9), thickness, new DashStyle([2, 2], 0));

        void At(int x, int y)
        {
            using (context.PushTransform(Matrix.CreateTranslation(x, y) * view.Matrix))
            {
                context.DrawGeometry(null, dark, _tipOutline);
                context.DrawGeometry(null, light, _tipOutline);
            }
        }
        At(_hoverPixel.X, _hoverPixel.Y);
        if (_editor.Tools.Active is not PaintToolBase || settings.Symmetry == SymmetryMode.None) return;
        int mx = view.AxisX2 - 1 - _hoverPixel.X, my = view.AxisY2 - 1 - _hoverPixel.Y;
        if (settings.Symmetry is SymmetryMode.Horizontal or SymmetryMode.Both) At(mx, _hoverPixel.Y);
        if (settings.Symmetry is SymmetryMode.Vertical or SymmetryMode.Both) At(_hoverPixel.X, my);
        if (settings.Symmetry == SymmetryMode.Both) At(mx, my);
    }

    // ------------------------------------------------------------------ input

    private ToolPointer MakePointer(Point position, bool right, KeyModifiers modifiers)
    {
        SpriteView view = _editor.View!;
        (double x, double y) = view.ToDocument(position);
        return new ToolPointer((int)Math.Floor(x), (int)Math.Floor(y), x, y, right, modifiers);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Focus();
        SpriteView? view = _editor.View;
        if (view == null || _editor.IsBusy) return;
        PointerPoint point = e.GetCurrentPoint(this);
        if (point.Properties.IsMiddleButtonPressed || _spaceHeld && point.Properties.IsLeftButtonPressed)
        {
            _panLast = point.Position;
            e.Pointer.Capture(this);
            UpdateCursor();
            return;
        }
        if (_toolDown || !point.Properties.IsLeftButtonPressed && !point.Properties.IsRightButtonPressed) return;
        bool right = point.Properties.IsRightButtonPressed;
        ToolPointer p = MakePointer(point.Position, right, e.KeyModifiers);
        e.Pointer.Capture(this);
        if (e.ClickCount == 2 && _editor.Tools.Active.DoubleClick(p)) return;
        _toolDown = true;
        _toolRight = right;
        _editor.Tools.Active.Down(p);
        InvalidateVisual();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        SpriteView? view = _editor.View;
        if (view == null) return;
        Point position = e.GetPosition(this);
        if (_panLast is Point last)
        {
            view.PanBy(position.X - last.X, position.Y - last.Y);
            _panLast = position;
            InvalidateVisual();
            return;
        }
        ToolPointer p = MakePointer(position, _toolRight, e.KeyModifiers);
        _hovering = true;
        _hoverPixel = p.Pixel;
        _editor.UpdateCursorStatus(p.X, p.Y);
        if (_toolDown) _editor.Tools.Active.Move(p);
        else _editor.Tools.Active.Hover(p);
        InvalidateVisual();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_panLast != null)
        {
            _panLast = null;
            e.Pointer.Capture(null);
            UpdateCursor();
            return;
        }
        if (!_toolDown || _editor.View == null)
        {
            e.Pointer.Capture(null);
            return;
        }
        // Only the button that started the stroke ends it.
        if (_toolRight ? e.InitialPressMouseButton != MouseButton.Right : e.InitialPressMouseButton != MouseButton.Left) return;
        _toolDown = false;
        e.Pointer.Capture(null);
        _editor.Tools.Active.Up(MakePointer(e.GetPosition(this), _toolRight, e.KeyModifiers));
        InvalidateVisual();
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _panLast = null;
        if (_toolDown)
        {
            _toolDown = false;
            _editor.Tools.Active.Cancel();
            _editor.InvalidateCanvas();
        }
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        _hovering = false;
        _editor.UpdateCursorStatus(null, null);
        InvalidateVisual();
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        SpriteView? view = _editor.View;
        if (view == null) return;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Alt) && e.Delta.Y != 0)
        {
            // Alt + wheel changes the brush size.
            _editor.Execute(e.Delta.Y > 0 ? "tool.brushLarger" : "tool.brushSmaller");
            e.Handled = true;
            return;
        }
        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift)) view.PanBy(e.Delta.Y * 48, 0);
        else if (e.Delta.Y != 0) view.ZoomStep(e.Delta.Y > 0, e.GetPosition(this));
        else view.PanBy(e.Delta.X * 48, 0);
        _editor.InvalidateOverlay();
        e.Handled = true;
    }

    /// <summary>Ends a stroke in progress without committing it, for when the sprite changes under the tool.</summary>
    public void AbortTool()
    {
        if (!_toolDown) return;
        _toolDown = false;
        _editor.Tools.Active.Cancel();
    }
}
