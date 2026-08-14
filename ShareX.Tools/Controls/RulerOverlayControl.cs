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
        PixelPoint SamplePoint,
        DrawingRectangle SourceBounds);

    public static readonly StyledProperty<Color> AccentColorProperty =
        AvaloniaProperty.Register<RulerOverlayControl, Color>(nameof(AccentColor), Color.Parse("#3E83F2"));

    private static readonly IBrush HitTestBrush = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
    private static readonly IPen ShadowPen = new Pen(new SolidColorBrush(Color.FromArgb(125, 0, 0, 0)), 4);
    private static readonly Typeface LabelTypeface = new(FontFamily.Default, FontStyle.Normal, FontWeight.SemiBold);
    private static readonly DashStyle DragDashStyle = new([5, 4], 0);

    private const int DragThreshold = 4;
    private const int MeasurementHitTestPadding = 6;
    private const int MinimumLineLength = 2;
    private const int MaximumColorTolerance = 255;
    private const double EndCapSize = 10;
    private const double RectangleCrossSize = 20;
    private const double LabelFontSize = 13;
    private const double LabelHorizontalPadding = 10;
    private const double LabelVerticalPadding = 5;
    private const double LightAccentLuminanceThreshold = 0.45;

    private readonly DispatcherTimer _statusTimer;
    private readonly List<Measurement> _measurements = [];
    private ScreenPixelBuffer? _screen;
    private Axis _axis = Axis.Horizontal;
    private int _tolerancePercentage;
    private PixelPoint? _pointerScreenPoint;
    private PixelPoint _pressScreenPoint;
    private DrawingRectangle _dragSelection;
    private Measurement? _hoverMeasurement;
    private bool _leftButtonPressed;
    private bool _isDragging;
    private string? _statusText;

    public Color AccentColor
    {
        get => GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public string? MeasurementText => _measurements.Count > 0
        ? GetMeasurementText(_measurements[^1])
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

        foreach (Measurement measurement in _measurements)
        {
            DrawMeasurement(context, measurement, accentBrush, accentPen);
        }

        if (_isDragging && !_dragSelection.IsEmpty)
        {
            DrawDragSelection(context, accentColor, accentBrush);
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

        if (pointer.Properties.IsMiddleButtonPressed)
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
        bool pointerMoved = _pointerScreenPoint is not PixelPoint previousPoint || previousPoint != screenPoint;
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

        if (pointerMoved)
        {
            _hoverMeasurement = CreateLineMeasurement(screenPoint);
            InvalidateVisual();
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            int measurementIndex = HitTestMeasurement(e.GetPosition(this));

            if (measurementIndex >= 0)
            {
                _measurements.RemoveAt(measurementIndex);
                InvalidateVisual();
                e.Handled = true;
            }

            return;
        }

        if (!_leftButtonPressed || e.InitialPressMouseButton != MouseButton.Left || _screen == null)
        {
            return;
        }

        PixelPoint screenPoint = ToScreen(e.GetPosition(this));
        _pointerScreenPoint = screenPoint;

        if (_isDragging)
        {
            DrawingRectangle sourceSelection = _screen.Clamp(CreateRectangle(_pressScreenPoint, screenPoint));
            DrawingRectangle snapped = _screen.FindContentBounds(sourceSelection, ColorTolerance);
            if (!snapped.IsEmpty)
            {
                _measurements.Add(new Measurement(MeasurementKind.Rectangle, snapped, _axis, screenPoint, sourceSelection));
            }
        }
        else
        {
            Measurement? line = CreateLineMeasurement(screenPoint);
            if (line is Measurement measurement)
            {
                _measurements.Add(measurement);
            }
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

        PixelPoint screenPoint = ToScreen(e.GetPosition(this));
        _pointerScreenPoint = screenPoint;
        _hoverMeasurement = CreateLineMeasurement(screenPoint);

        _tolerancePercentage = Math.Clamp(_tolerancePercentage + Math.Sign(e.Delta.Y), 0, 100);
        RecalculateHoverMeasurement();
        ShowStatus();
        e.Handled = true;
    }

    public void Clear()
    {
        _measurements.Clear();
        if (_pointerScreenPoint is PixelPoint point)
        {
            _hoverMeasurement = CreateLineMeasurement(point);
        }
        InvalidateVisual();
    }

    public void Nudge(int x, int y)
    {
        if (_screen == null || _measurements.Count == 0)
        {
            return;
        }

        int index = _measurements.Count - 1;
        Measurement measurement = _measurements[index];

        if (measurement.Kind == MeasurementKind.Line)
        {
            PixelPoint point = measurement.SamplePoint;
            point = _screen.Clamp(new PixelPoint(point.X + x, point.Y + y));
            Measurement? movedLine = CreateLineMeasurement(point, measurement.Axis);
            if (movedLine is Measurement line)
            {
                _measurements[index] = line;
            }
        }
        else
        {
            DrawingRectangle sourceSelection = measurement.SourceBounds;
            sourceSelection.Offset(x, y);
            sourceSelection = _screen.ClampPreservingSize(sourceSelection);
            DrawingRectangle snapped = _screen.FindContentBounds(sourceSelection, ColorTolerance);
            _measurements[index] = measurement with { Bounds = snapped, SourceBounds = sourceSelection };
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

        if (samplePoint is PixelPoint hoverPoint)
        {
            _hoverMeasurement = CreateLineMeasurement(hoverPoint);
        }

        InvalidateVisual();
    }

    private Measurement? CreateLineMeasurement(PixelPoint point, Axis? axis = null)
    {
        if (_screen == null || !_screen.Bounds.Contains(point))
        {
            return null;
        }

        Axis measurementAxis = axis ?? _axis;
        DrawingRectangle bounds = _screen.FindColorRun(point, measurementAxis == Axis.Horizontal, ColorTolerance);
        int length = measurementAxis == Axis.Horizontal ? bounds.Width : bounds.Height;
        return length < MinimumLineLength
            ? null
            : new Measurement(MeasurementKind.Line, bounds, measurementAxis, point, default);
    }

    private void RecalculateHoverMeasurement()
    {
        if (_screen == null)
        {
            return;
        }

        if (_hoverMeasurement is Measurement hover)
        {
            _hoverMeasurement = CreateLineMeasurement(hover.SamplePoint);
        }
        else if (_pointerScreenPoint is PixelPoint point)
        {
            _hoverMeasurement = CreateLineMeasurement(point);
        }

        InvalidateVisual();
    }

    private void ShowStatus()
    {
        _statusText = string.Format(Localization.Strings.RulerOverlayControl_Tolerance, _tolerancePercentage);
        _statusTimer.Stop();
        _statusTimer.Start();
        InvalidateVisual();
    }

    private int ColorTolerance =>
        (int)Math.Round(_tolerancePercentage * MaximumColorTolerance / 100d);

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

    private int HitTestMeasurement(Point point)
    {
        for (int i = _measurements.Count - 1; i >= 0; i--)
        {
            Measurement measurement = _measurements[i];
            Rect shapeBounds;

            if (measurement.Kind == MeasurementKind.Rectangle)
            {
                shapeBounds = ToClient(measurement.Bounds);
            }
            else
            {
                GetLineEndpoints(measurement, out Point start, out Point end);
                shapeBounds = new Rect(
                    Math.Min(start.X, end.X),
                    Math.Min(start.Y, end.Y),
                    Math.Abs(end.X - start.X),
                    Math.Abs(end.Y - start.Y));
            }

            Rect hitBounds = Union(shapeBounds, GetMeasurementLabelBounds(measurement))
                .Inflate(MeasurementHitTestPadding);

            if (hitBounds.Contains(point))
            {
                return i;
            }
        }

        return -1;
    }

    private static Rect Union(Rect first, Rect second)
    {
        double left = Math.Min(first.Left, second.Left);
        double top = Math.Min(first.Top, second.Top);
        double right = Math.Max(first.Right, second.Right);
        double bottom = Math.Max(first.Bottom, second.Bottom);
        return new Rect(left, top, right - left, bottom - top);
    }

    private void DrawLineMeasurement(DrawingContext context, Measurement measurement, IBrush accentBrush, IPen accentPen)
    {
        GetLineEndpoints(measurement, out Point start, out Point end);

        context.DrawLine(ShadowPen, start, end);
        context.DrawLine(accentPen, start, end);
        DrawEndCap(context, start, measurement.Axis, accentPen);
        DrawEndCap(context, end, measurement.Axis, accentPen);

        string label = GetMeasurementText(measurement);
        Point labelCenter = GetLineLabelCenter(label, start, end, measurement.Axis);
        DrawLabel(context, label, labelCenter, accentBrush);
    }

    private void DrawRectangleMeasurement(DrawingContext context, Measurement measurement, IBrush accentBrush, IPen accentPen)
    {
        Rect rect = ToClient(measurement.Bounds);
        IBrush fill = new SolidColorBrush(Color.FromArgb(28, AccentColor.R, AccentColor.G, AccentColor.B));
        context.DrawRectangle(fill, ShadowPen, rect);
        context.DrawRectangle(null, accentPen, rect);

        double halfCross = RectangleCrossSize / 2;
        context.DrawLine(accentPen,
            new Point(rect.Center.X - halfCross, rect.Center.Y),
            new Point(rect.Center.X + halfCross, rect.Center.Y));
        context.DrawLine(accentPen,
            new Point(rect.Center.X, rect.Center.Y - halfCross),
            new Point(rect.Center.X, rect.Center.Y + halfCross));

        string label = GetMeasurementText(measurement);
        DrawLabel(context, label, GetRectangleLabelCenter(measurement, label), accentBrush);
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
        if (_pointerScreenPoint is not PixelPoint screenPoint)
        {
            return;
        }

        const double offset = 16;
        const double margin = 6;
        Point pointer = ToClient(screenPoint.X, screenPoint.Y);
        Size labelSize = MeasureLabel(text);
        double x = pointer.X + offset + labelSize.Width / 2;
        double y = pointer.Y + offset + labelSize.Height / 2;

        if (x + labelSize.Width / 2 > Bounds.Width - margin)
        {
            x = pointer.X - offset - labelSize.Width / 2;
        }

        if (y + labelSize.Height / 2 > Bounds.Height - margin)
        {
            y = pointer.Y - offset - labelSize.Height / 2;
        }

        DrawLabel(context, text, new Point(x, y), accentBrush);
    }

    private void DrawLabel(DrawingContext context, string text, Point center, IBrush accentBrush)
    {
        IBrush textBrush = GetContrastingTextBrush();
        FormattedText formattedText = CreateText(text, textBrush);
        Rect capsule = GetLabelBounds(text, center);

        context.DrawRectangle(accentBrush, null, capsule, capsule.Height / 2, capsule.Height / 2);
        context.DrawText(formattedText, new Point(
            capsule.X + (capsule.Width - formattedText.Width) / 2,
            capsule.Y + (capsule.Height - formattedText.Height) / 2));
    }

    private Rect GetMeasurementLabelBounds(Measurement measurement)
    {
        string text = GetMeasurementText(measurement);
        Point center;

        if (measurement.Kind == MeasurementKind.Line)
        {
            GetLineEndpoints(measurement, out Point start, out Point end);
            center = GetLineLabelCenter(text, start, end, measurement.Axis);
        }
        else
        {
            center = GetRectangleLabelCenter(measurement, text);
        }

        return GetLabelBounds(text, center);
    }

    private Rect GetLabelBounds(string text, Point center)
    {
        Size size = MeasureLabel(text);
        Rect capsule = new(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
        return KeepInside(capsule, new Rect(Bounds.Size), 6);
    }

    private Point GetRectangleLabelCenter(Measurement measurement, string text)
    {
        Rect rect = ToClient(measurement.Bounds);
        double labelHeight = MeasureLabel(text).Height;
        double y = rect.Bottom + 10 + labelHeight <= Bounds.Height
            ? rect.Bottom + 10 + labelHeight / 2
            : rect.Top - 10 - labelHeight / 2;
        return new Point(rect.Center.X, y);
    }

    private void GetLineEndpoints(Measurement measurement, out Point start, out Point end)
    {
        DrawingRectangle bounds = measurement.Bounds;

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
    }

    private Point GetLineLabelCenter(string text, Point start, Point end, Axis axis)
    {
        Point center = Midpoint(start, end);
        Size labelSize = MeasureLabel(text);
        double lineLength = axis == Axis.Horizontal
            ? Math.Abs(end.X - start.X)
            : Math.Abs(end.Y - start.Y);
        double coveredLength = axis == Axis.Horizontal ? labelSize.Width : labelSize.Height;

        if (lineLength >= coveredLength)
        {
            return center;
        }

        const double gap = 8;

        if (axis == Axis.Horizontal)
        {
            double offset = EndCapSize / 2 + gap + labelSize.Height / 2;
            double below = center.Y + offset;
            double above = center.Y - offset;
            center = center.WithY(below + labelSize.Height / 2 <= Bounds.Height - 6 ? below : above);
        }
        else
        {
            double offset = gap + labelSize.Height / 2;
            double bottom = Math.Max(start.Y, end.Y);
            double top = Math.Min(start.Y, end.Y);
            double below = bottom + offset;
            double above = top - offset;
            center = center.WithY(below + labelSize.Height / 2 <= Bounds.Height - 6 ? below : above);
        }

        return center;
    }

    private static Size MeasureLabel(string text)
    {
        FormattedText formattedText = CreateText(text, Brushes.White);
        return new Size(formattedText.Width + LabelHorizontalPadding * 2,
            formattedText.Height + LabelVerticalPadding * 2);
    }

    private IBrush GetContrastingTextBrush()
    {
        double luminance = GetRelativeLuminance(AccentColor);
        return luminance >= LightAccentLuminanceThreshold ? Brushes.Black : Brushes.White;
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
