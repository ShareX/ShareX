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
using Avalonia.Threading;
using ShareX.Tools.Ruler;
using System.Globalization;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.Tools.Controls;

public sealed class RulerOverlayControl : Control
{
    private enum Axis
    {
        Horizontal,
        Vertical
    }

    private enum MeasurementKind
    {
        Line,
        Rectangle
    }

    private readonly record struct Measurement(
        MeasurementKind Kind,
        DrawingRectangle Bounds,
        Axis Axis,
        PixelPoint SamplePoint);

    public static readonly StyledProperty<Color> AccentColorProperty =
        AvaloniaProperty.Register<RulerOverlayControl, Color>(nameof(AccentColor), Color.Parse("#3E83F2"));

    private static readonly IBrush HitTestBrush = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
    private static readonly IPen ShadowPen = new Pen(new SolidColorBrush(Color.FromArgb(125, 0, 0, 0)), 4);
    private static readonly Typeface LabelTypeface = new(FontFamily.Default, FontStyle.Normal, FontWeight.SemiBold);
    private static readonly DashStyle DragDashStyle = new([5, 4], 0);

    private const int DragThreshold = 4;
    private const int ToleranceStep = 3;
    private const int MaximumTolerance = 255;
    private const double EndCapSize = 10;
    private const double LabelFontSize = 13;
    private const double LabelHorizontalPadding = 10;
    private const double LabelVerticalPadding = 5;

    private readonly DispatcherTimer _statusTimer;
    private ScreenPixelBuffer? _screen;
    private Axis _axis = Axis.Horizontal;
    private int _tolerance;
    private PixelPoint? _pointerScreenPoint;
    private PixelPoint _pressScreenPoint;
    private DrawingRectangle _sourceSelection;
    private DrawingRectangle _dragSelection;
    private Measurement? _hoverMeasurement;
    private Measurement? _measurement;
    private bool _leftButtonPressed;
    private bool _isDragging;
    private string? _statusText;

    public Color AccentColor
    {
        get => GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public string? MeasurementText => _measurement is Measurement measurement
        ? GetMeasurementText(measurement)
        : null;

    public RulerOverlayControl()
    {
        Focusable = true;
        Cursor = new Cursor(StandardCursorType.Cross);

        _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1400) };
        _statusTimer.Tick += (_, _) =>
        {
            _statusTimer.Stop();
            _statusText = null;
            InvalidateVisual();
        };
    }

    internal void SetScreenPixelBuffer(ScreenPixelBuffer screen)
    {
        _screen = screen;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.FillRectangle(HitTestBrush, new Rect(Bounds.Size));

        Color accentColor = AccentColor;
        IBrush accentBrush = new SolidColorBrush(accentColor);
        IPen accentPen = new Pen(accentBrush, 2);

        if (_isDragging && !_dragSelection.IsEmpty)
        {
            DrawDragSelection(context, accentColor, accentBrush);
        }
        else if (_measurement is Measurement measurement)
        {
            DrawMeasurement(context, measurement, accentBrush, accentPen);
        }
        else if (_hoverMeasurement is Measurement hover)
        {
            DrawMeasurement(context, hover, accentBrush, accentPen);
        }

        if (!string.IsNullOrEmpty(_statusText))
        {
            DrawStatus(context, _statusText, accentBrush);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == AccentColorProperty)
        {
            InvalidateVisual();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        PointerPoint pointer = e.GetCurrentPoint(this);
        PixelPoint screenPoint = ToScreen(pointer.Position);
        _pointerScreenPoint = screenPoint;

        if (pointer.Properties.IsRightButtonPressed)
        {
            ToggleAxis(screenPoint);
            e.Handled = true;
            return;
        }

        if (!pointer.Properties.IsLeftButtonPressed || _screen == null)
        {
            return;
        }

        Focus();
        _leftButtonPressed = true;
        _isDragging = false;
        _pressScreenPoint = screenPoint;
        _dragSelection = default;
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        PixelPoint screenPoint = ToScreen(e.GetPosition(this));
        _pointerScreenPoint = screenPoint;

        if (_screen == null)
        {
            return;
        }

        if (_leftButtonPressed)
        {
            int deltaX = Math.Abs(screenPoint.X - _pressScreenPoint.X);
            int deltaY = Math.Abs(screenPoint.Y - _pressScreenPoint.Y);
            _isDragging |= deltaX >= DragThreshold || deltaY >= DragThreshold;

            if (_isDragging)
            {
                _dragSelection = _screen.Clamp(CreateRectangle(_pressScreenPoint, screenPoint));
                InvalidateVisual();
            }

            e.Handled = true;
            return;
        }

        if (_measurement == null)
        {
            _hoverMeasurement = CreateLineMeasurement(screenPoint);
            InvalidateVisual();
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!_leftButtonPressed || e.InitialPressMouseButton != MouseButton.Left || _screen == null)
        {
            return;
        }

        PixelPoint screenPoint = ToScreen(e.GetPosition(this));
        _pointerScreenPoint = screenPoint;

        if (_isDragging)
        {
            _sourceSelection = _screen.Clamp(CreateRectangle(_pressScreenPoint, screenPoint));
            DrawingRectangle snapped = _screen.FindContentBounds(_sourceSelection, _tolerance);
            _measurement = new Measurement(MeasurementKind.Rectangle, snapped, _axis, screenPoint);
        }
        else
        {
            _sourceSelection = default;
            _measurement = CreateLineMeasurement(screenPoint);
        }

        _hoverMeasurement = null;
        _dragSelection = default;
        _leftButtonPressed = false;
        _isDragging = false;
        e.Pointer.Capture(null);
        InvalidateVisual();
        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _leftButtonPressed = false;
        _isDragging = false;
        _dragSelection = default;
        InvalidateVisual();
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (_screen == null || e.Delta.Y == 0)
        {
            return;
        }

        int step = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? ToleranceStep * 5 : ToleranceStep;
        _tolerance = Math.Clamp(_tolerance + Math.Sign(e.Delta.Y) * step, 0, MaximumTolerance);
        RecalculateMeasurement();
        ShowStatus();
        e.Handled = true;
    }

    public void Clear()
    {
        _measurement = null;
        _sourceSelection = default;
        if (_pointerScreenPoint is PixelPoint point)
        {
            _hoverMeasurement = CreateLineMeasurement(point);
        }
        InvalidateVisual();
    }

    public void Nudge(int x, int y)
    {
        if (_screen == null || _measurement == null)
        {
            return;
        }

        if (_measurement.Value.Kind == MeasurementKind.Line)
        {
            PixelPoint point = _measurement.Value.SamplePoint;
            point = _screen.Clamp(new PixelPoint(point.X + x, point.Y + y));
            _measurement = CreateLineMeasurement(point);
        }
        else
        {
            _sourceSelection.Offset(x, y);
            _sourceSelection = _screen.ClampPreservingSize(_sourceSelection);
            DrawingRectangle snapped = _screen.FindContentBounds(_sourceSelection, _tolerance);
            _measurement = _measurement.Value with { Bounds = snapped };
        }

        InvalidateVisual();
    }

    public void SetHorizontal() => SetAxis(Axis.Horizontal);

    public void SetVertical() => SetAxis(Axis.Vertical);

    public void ToggleAxis() => ToggleAxis(_pointerScreenPoint);

    private void ToggleAxis(PixelPoint? point)
    {
        SetAxis(_axis == Axis.Horizontal ? Axis.Vertical : Axis.Horizontal, point);
    }

    private void SetAxis(Axis axis, PixelPoint? point = null)
    {
        _axis = axis;
        PixelPoint? samplePoint = point ?? _pointerScreenPoint;

        if (_measurement is { Kind: MeasurementKind.Line } measurement)
        {
            _measurement = CreateLineMeasurement(samplePoint ?? measurement.SamplePoint);
        }
        else if (_measurement == null && samplePoint is PixelPoint hoverPoint)
        {
            _hoverMeasurement = CreateLineMeasurement(hoverPoint);
        }

        ShowStatus();
    }

    private Measurement? CreateLineMeasurement(PixelPoint point)
    {
        if (_screen == null || !_screen.Bounds.Contains(point))
        {
            return null;
        }

        DrawingRectangle bounds = _screen.FindColorRun(point, _axis == Axis.Horizontal, _tolerance);
        return bounds.IsEmpty ? null : new Measurement(MeasurementKind.Line, bounds, _axis, point);
    }

    private void RecalculateMeasurement()
    {
        if (_screen == null)
        {
            return;
        }

        if (_measurement is { Kind: MeasurementKind.Line } line)
        {
            _measurement = CreateLineMeasurement(line.SamplePoint);
        }
        else if (_measurement is { Kind: MeasurementKind.Rectangle } rectangle && !_sourceSelection.IsEmpty)
        {
            DrawingRectangle snapped = _screen.FindContentBounds(_sourceSelection, _tolerance);
            _measurement = rectangle with { Bounds = snapped };
        }
        else if (_hoverMeasurement is Measurement hover)
        {
            _hoverMeasurement = CreateLineMeasurement(hover.SamplePoint);
        }

        InvalidateVisual();
    }

    private void ShowStatus()
    {
        _statusText = $"{(_axis == Axis.Horizontal ? "H" : "V")}  ·  T {_tolerance}";
        _statusTimer.Stop();
        _statusTimer.Start();
        InvalidateVisual();
    }

    private void DrawMeasurement(DrawingContext context, Measurement measurement, IBrush accentBrush, IPen accentPen)
    {
        if (measurement.Kind == MeasurementKind.Line)
        {
            DrawLineMeasurement(context, measurement, accentBrush, accentPen);
        }
        else
        {
            DrawRectangleMeasurement(context, measurement, accentBrush, accentPen);
        }
    }

    private void DrawLineMeasurement(DrawingContext context, Measurement measurement, IBrush accentBrush, IPen accentPen)
    {
        DrawingRectangle bounds = measurement.Bounds;
        Point start;
        Point end;

        if (measurement.Axis == Axis.Horizontal)
        {
            start = ToClient(bounds.Left, measurement.SamplePoint.Y);
            end = ToClient(bounds.Right, measurement.SamplePoint.Y);
        }
        else
        {
            start = ToClient(measurement.SamplePoint.X, bounds.Top);
            end = ToClient(measurement.SamplePoint.X, bounds.Bottom);
        }

        context.DrawLine(ShadowPen, start, end);
        context.DrawLine(accentPen, start, end);
        DrawEndCap(context, start, measurement.Axis, accentPen);
        DrawEndCap(context, end, measurement.Axis, accentPen);
        DrawLabel(context, GetMeasurementText(measurement), Midpoint(start, end), accentBrush);
    }

    private void DrawRectangleMeasurement(DrawingContext context, Measurement measurement, IBrush accentBrush, IPen accentPen)
    {
        Rect rect = ToClient(measurement.Bounds);
        IBrush fill = new SolidColorBrush(Color.FromArgb(28, AccentColor.R, AccentColor.G, AccentColor.B));
        context.DrawRectangle(fill, ShadowPen, rect);
        context.DrawRectangle(null, accentPen, rect);

        double halfCap = EndCapSize / 2;
        context.DrawLine(accentPen, new Point(rect.Center.X, rect.Top - halfCap), new Point(rect.Center.X, rect.Top + halfCap));
        context.DrawLine(accentPen, new Point(rect.Center.X, rect.Bottom - halfCap), new Point(rect.Center.X, rect.Bottom + halfCap));
        context.DrawLine(accentPen, new Point(rect.Left - halfCap, rect.Center.Y), new Point(rect.Left + halfCap, rect.Center.Y));
        context.DrawLine(accentPen, new Point(rect.Right - halfCap, rect.Center.Y), new Point(rect.Right + halfCap, rect.Center.Y));

        FormattedText text = CreateText(GetMeasurementText(measurement), GetContrastingTextBrush());
        double labelHeight = text.Height + LabelVerticalPadding * 2;
        double y = rect.Bottom + 10 + labelHeight <= Bounds.Height
            ? rect.Bottom + 10 + labelHeight / 2
            : rect.Top - 10 - labelHeight / 2;
        DrawLabel(context, GetMeasurementText(measurement), new Point(rect.Center.X, y), accentBrush);
    }

    private void DrawDragSelection(DrawingContext context, Color accentColor, IBrush accentBrush)
    {
        Rect rect = ToClient(_dragSelection);
        IBrush fill = new SolidColorBrush(Color.FromArgb(22, accentColor.R, accentColor.G, accentColor.B));
        IPen pen = new Pen(accentBrush, 1, DragDashStyle);
        context.DrawRectangle(fill, pen, rect);
    }

    private void DrawStatus(DrawingContext context, string text, IBrush accentBrush)
    {
        DrawLabel(context, text, new Point(Bounds.Width / 2, 28), accentBrush);
    }

    private void DrawLabel(DrawingContext context, string text, Point center, IBrush accentBrush)
    {
        IBrush textBrush = GetContrastingTextBrush();
        FormattedText formattedText = CreateText(text, textBrush);
        Size size = new(formattedText.Width + LabelHorizontalPadding * 2,
            formattedText.Height + LabelVerticalPadding * 2);
        Rect capsule = new(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
        capsule = KeepInside(capsule, new Rect(Bounds.Size), 6);

        context.DrawRectangle(accentBrush, null, capsule, capsule.Height / 2, capsule.Height / 2);
        context.DrawText(formattedText, new Point(
            capsule.X + (capsule.Width - formattedText.Width) / 2,
            capsule.Y + (capsule.Height - formattedText.Height) / 2));
    }

    private IBrush GetContrastingTextBrush()
    {
        double luminance = GetRelativeLuminance(AccentColor);
        double whiteContrast = 1.05 / (luminance + 0.05);
        double blackContrast = (luminance + 0.05) / 0.05;
        return whiteContrast >= blackContrast ? Brushes.White : Brushes.Black;
    }

    private static FormattedText CreateText(string text, IBrush brush) => new(
        text,
        CultureInfo.CurrentUICulture,
        FlowDirection.LeftToRight,
        LabelTypeface,
        LabelFontSize,
        brush);

    private PixelPoint ToScreen(Point point)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        return topLevel?.PointToScreen(point) ?? default;
    }

    private Point ToClient(int x, int y)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        return topLevel?.PointToClient(new PixelPoint(x, y)) ?? default;
    }

    private Rect ToClient(DrawingRectangle rectangle)
    {
        Point topLeft = ToClient(rectangle.Left, rectangle.Top);
        Point bottomRight = ToClient(rectangle.Right, rectangle.Bottom);
        return new Rect(topLeft, bottomRight);
    }

    private static DrawingRectangle CreateRectangle(PixelPoint start, PixelPoint end)
    {
        int left = Math.Min(start.X, end.X);
        int top = Math.Min(start.Y, end.Y);
        return new DrawingRectangle(left, top,
            Math.Abs(end.X - start.X) + 1,
            Math.Abs(end.Y - start.Y) + 1);
    }

    private static string GetMeasurementText(Measurement measurement) => measurement.Kind switch
    {
        MeasurementKind.Rectangle => $"{measurement.Bounds.Width} × {measurement.Bounds.Height} px",
        _ when measurement.Axis == Axis.Horizontal => $"{measurement.Bounds.Width} px",
        _ => $"{measurement.Bounds.Height} px"
    };

    private static void DrawEndCap(DrawingContext context, Point point, Axis axis, IPen pen)
    {
        double half = EndCapSize / 2;
        if (axis == Axis.Horizontal)
        {
            context.DrawLine(pen, new Point(point.X, point.Y - half), new Point(point.X, point.Y + half));
        }
        else
        {
            context.DrawLine(pen, new Point(point.X - half, point.Y), new Point(point.X + half, point.Y));
        }
    }

    private static Point Midpoint(Point first, Point second) =>
        new((first.X + second.X) / 2, (first.Y + second.Y) / 2);

    private static Rect KeepInside(Rect rect, Rect container, double margin)
    {
        double x = Math.Clamp(rect.X, container.Left + margin,
            Math.Max(container.Left + margin, container.Right - margin - rect.Width));
        double y = Math.Clamp(rect.Y, container.Top + margin,
            Math.Max(container.Top + margin, container.Bottom - margin - rect.Height));
        return rect.WithX(x).WithY(y);
    }

    private static double GetRelativeLuminance(Color color)
    {
        static double Linearize(byte channel)
        {
            double value = channel / 255d;
            return value <= 0.03928 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
        }

        return (0.2126 * Linearize(color.R)) +
            (0.7152 * Linearize(color.G)) +
            (0.0722 * Linearize(color.B));
    }
}
