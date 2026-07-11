#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace ShareX.Tools.Controls;

public sealed class RulerOverlayControl : Control
{
    private enum DragMode { None, Create, Move, Resize }
    private enum ResizeHandle { None, TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left }

    private static readonly IBrush FillBrush = new SolidColorBrush(Color.FromArgb(42, 0, 120, 212));
    private static readonly IBrush HitTestBrush = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
    private static readonly IBrush EdgeBrush = new SolidColorBrush(Color.FromArgb(245, 255, 255, 255));
    private static readonly IBrush AccentBrush = new SolidColorBrush(Color.FromRgb(0, 120, 212));
    private static readonly IPen OuterPen = new Pen(new SolidColorBrush(Color.FromArgb(190, 0, 0, 0)), 3);
    private static readonly IPen EdgePen = new Pen(EdgeBrush, 1);
    private static readonly IPen DiagonalPen = new Pen(new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)), 1,
        new DashStyle([5, 4], 0));

    private const double HandleSize = 9;
    private Rect _selection;
    private Rect _originalSelection;
    private Point _anchor;
    private Point _lastPoint;
    private DragMode _dragMode;
    private ResizeHandle _resizeHandle;

    public event EventHandler<Rect>? MeasurementChanged;

    public Rect Selection => _selection;

    public RulerOverlayControl()
    {
        Focusable = true;
        Cursor = new Cursor(StandardCursorType.Cross);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.FillRectangle(HitTestBrush, new Rect(Bounds.Size));
        if (_selection.Width <= 0 && _selection.Height <= 0)
        {
            return;
        }

        context.FillRectangle(FillBrush, _selection);
        context.DrawRectangle(null, OuterPen, _selection);
        context.DrawRectangle(null, EdgePen, _selection);
        context.DrawLine(DiagonalPen, _selection.TopLeft, _selection.BottomRight);
        DrawTicks(context);
        DrawHandles(context);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        Focus();
        Point point = e.GetPosition(this);
        _originalSelection = _selection;
        _anchor = point;
        _lastPoint = point;
        _resizeHandle = HitTestHandle(point);

        if (_resizeHandle != ResizeHandle.None)
        {
            _dragMode = DragMode.Resize;
        }
        else if (_selection.Contains(point))
        {
            _dragMode = DragMode.Move;
        }
        else
        {
            _dragMode = DragMode.Create;
            _selection = new Rect(point, point);
        }

        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        Point point = e.GetPosition(this);
        if (_dragMode == DragMode.None)
        {
            UpdateCursor(point);
            return;
        }

        switch (_dragMode)
        {
            case DragMode.Create:
                _selection = CreateRect(_anchor, point, e.KeyModifiers.HasFlag(KeyModifiers.Shift));
                break;
            case DragMode.Move:
                Vector delta = point - _lastPoint;
                _selection = Clamp(new Rect(_selection.Position + delta, _selection.Size));
                _lastPoint = point;
                break;
            case DragMode.Resize:
                _selection = Resize(_originalSelection, point, _resizeHandle);
                break;
        }

        RaiseChanged();
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_dragMode != DragMode.None)
        {
            _dragMode = DragMode.None;
            e.Pointer.Capture(null);
            RaiseChanged();
            e.Handled = true;
        }
    }

    public void Clear()
    {
        _selection = default;
        RaiseChanged();
    }

    public void Nudge(double x, double y)
    {
        if (_selection.Width <= 0 && _selection.Height <= 0)
        {
            return;
        }

        _selection = Clamp(new Rect(_selection.X + x, _selection.Y + y, _selection.Width, _selection.Height));
        RaiseChanged();
    }

    private void RaiseChanged()
    {
        InvalidateVisual();
        MeasurementChanged?.Invoke(this, _selection);
    }

    private Rect Clamp(Rect rect)
    {
        double x = Math.Clamp(rect.X, 0, Math.Max(0, Bounds.Width - rect.Width));
        double y = Math.Clamp(rect.Y, 0, Math.Max(0, Bounds.Height - rect.Height));
        return new Rect(x, y, Math.Min(rect.Width, Bounds.Width), Math.Min(rect.Height, Bounds.Height));
    }

    private Rect Resize(Rect original, Point point, ResizeHandle handle)
    {
        double left = original.Left;
        double top = original.Top;
        double right = original.Right;
        double bottom = original.Bottom;

        if (handle is ResizeHandle.TopLeft or ResizeHandle.Left or ResizeHandle.BottomLeft) left = point.X;
        if (handle is ResizeHandle.TopLeft or ResizeHandle.Top or ResizeHandle.TopRight) top = point.Y;
        if (handle is ResizeHandle.TopRight or ResizeHandle.Right or ResizeHandle.BottomRight) right = point.X;
        if (handle is ResizeHandle.BottomLeft or ResizeHandle.Bottom or ResizeHandle.BottomRight) bottom = point.Y;

        return Clamp(CreateRect(new Point(left, top), new Point(right, bottom), false));
    }

    private static Rect CreateRect(Point start, Point end, bool constrainSquare)
    {
        Vector delta = end - start;
        if (constrainSquare)
        {
            double size = Math.Max(Math.Abs(delta.X), Math.Abs(delta.Y));
            end = new Point(start.X + Math.CopySign(size, delta.X == 0 ? 1 : delta.X),
                start.Y + Math.CopySign(size, delta.Y == 0 ? 1 : delta.Y));
        }

        return new Rect(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y),
            Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y));
    }

    private ResizeHandle HitTestHandle(Point point)
    {
        if (_selection.Width <= 0 && _selection.Height <= 0) return ResizeHandle.None;
        Point[] points = GetHandlePoints();
        for (int i = 0; i < points.Length; i++)
        {
            if (new Rect(points[i].X - HandleSize, points[i].Y - HandleSize, HandleSize * 2, HandleSize * 2).Contains(point))
            {
                return (ResizeHandle)(i + 1);
            }
        }
        return ResizeHandle.None;
    }

    private Point[] GetHandlePoints() =>
    [
        _selection.TopLeft,
        new Point(_selection.Center.X, _selection.Top),
        _selection.TopRight,
        new Point(_selection.Right, _selection.Center.Y),
        _selection.BottomRight,
        new Point(_selection.Center.X, _selection.Bottom),
        _selection.BottomLeft,
        new Point(_selection.Left, _selection.Center.Y)
    ];

    private void DrawHandles(DrawingContext context)
    {
        foreach (Point point in GetHandlePoints())
        {
            Rect handle = new(point.X - HandleSize / 2, point.Y - HandleSize / 2, HandleSize, HandleSize);
            context.FillRectangle(AccentBrush, handle);
            context.DrawRectangle(null, EdgePen, handle);
        }
    }

    private void DrawTicks(DrawingContext context)
    {
        for (double x = 10; x < _selection.Width; x += 10)
        {
            double length = x % 100 == 0 ? 15 : x % 50 == 0 ? 10 : 5;
            context.DrawLine(EdgePen, new Point(_selection.Left + x, _selection.Top), new Point(_selection.Left + x, _selection.Top + length));
            context.DrawLine(EdgePen, new Point(_selection.Left + x, _selection.Bottom), new Point(_selection.Left + x, _selection.Bottom - length));
        }

        for (double y = 10; y < _selection.Height; y += 10)
        {
            double length = y % 100 == 0 ? 15 : y % 50 == 0 ? 10 : 5;
            context.DrawLine(EdgePen, new Point(_selection.Left, _selection.Top + y), new Point(_selection.Left + length, _selection.Top + y));
            context.DrawLine(EdgePen, new Point(_selection.Right, _selection.Top + y), new Point(_selection.Right - length, _selection.Top + y));
        }
    }

    private void UpdateCursor(Point point)
    {
        ResizeHandle handle = HitTestHandle(point);
        Cursor = handle switch
        {
            ResizeHandle.TopLeft or ResizeHandle.BottomRight => new Cursor(StandardCursorType.TopLeftCorner),
            ResizeHandle.TopRight or ResizeHandle.BottomLeft => new Cursor(StandardCursorType.TopRightCorner),
            ResizeHandle.Top or ResizeHandle.Bottom => new Cursor(StandardCursorType.SizeNorthSouth),
            ResizeHandle.Left or ResizeHandle.Right => new Cursor(StandardCursorType.SizeWestEast),
            _ when _selection.Contains(point) => new Cursor(StandardCursorType.SizeAll),
            _ => new Cursor(StandardCursorType.Cross)
        };
    }
}
