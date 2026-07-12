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
using Avalonia.Media;
using Avalonia.Threading;
using System.Diagnostics;

namespace ShareX.Tools.Controls;

public sealed class MonitorTestPatternControl : Control
{
    public static readonly StyledProperty<MonitorTestMode> ModeProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, MonitorTestMode>(nameof(Mode));
    public static readonly StyledProperty<double> GrayValueProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, double>(nameof(GrayValue), 255);
    public static readonly StyledProperty<double> RedProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, double>(nameof(Red), 255);
    public static readonly StyledProperty<double> GreenProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, double>(nameof(Green));
    public static readonly StyledProperty<double> BlueProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, double>(nameof(Blue));
    public static readonly StyledProperty<Color> GradientStartProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, Color>(nameof(GradientStart), Colors.White);
    public static readonly StyledProperty<Color> GradientEndProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, Color>(nameof(GradientEnd), Colors.Black);
    public static readonly StyledProperty<MonitorGradientDirection> GradientDirectionProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, MonitorGradientDirection>(nameof(GradientDirection), MonitorGradientDirection.Vertical);
    public static readonly StyledProperty<MonitorPattern> PatternProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, MonitorPattern>(nameof(Pattern));
    public static readonly StyledProperty<double> PatternSizeProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, double>(nameof(PatternSize), 5);
    public static readonly StyledProperty<MonitorMotionDirection> MotionDirectionProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, MonitorMotionDirection>(nameof(MotionDirection));
    public static readonly StyledProperty<double> MotionSpeedProperty =
        AvaloniaProperty.Register<MonitorTestPatternControl, double>(nameof(MotionSpeed), 500);

    private readonly DispatcherTimer _animationTimer;
    private readonly Stopwatch _animationTime = new();
    private TimeSpan _lastElapsed;
    private double _motionOffset;

    static MonitorTestPatternControl()
    {
        AffectsRender<MonitorTestPatternControl>(
            ModeProperty, GrayValueProperty, RedProperty, GreenProperty, BlueProperty,
            GradientStartProperty, GradientEndProperty, GradientDirectionProperty,
            PatternProperty, PatternSizeProperty, MotionDirectionProperty, MotionSpeedProperty);
    }

    public MonitorTestPatternControl()
    {
        _animationTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Render, OnAnimationTick);
    }

    public MonitorTestMode Mode { get => GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    public double GrayValue { get => GetValue(GrayValueProperty); set => SetValue(GrayValueProperty, value); }
    public double Red { get => GetValue(RedProperty); set => SetValue(RedProperty, value); }
    public double Green { get => GetValue(GreenProperty); set => SetValue(GreenProperty, value); }
    public double Blue { get => GetValue(BlueProperty); set => SetValue(BlueProperty, value); }
    public Color GradientStart { get => GetValue(GradientStartProperty); set => SetValue(GradientStartProperty, value); }
    public Color GradientEnd { get => GetValue(GradientEndProperty); set => SetValue(GradientEndProperty, value); }
    public MonitorGradientDirection GradientDirection { get => GetValue(GradientDirectionProperty); set => SetValue(GradientDirectionProperty, value); }
    public MonitorPattern Pattern { get => GetValue(PatternProperty); set => SetValue(PatternProperty, value); }
    public double PatternSize { get => GetValue(PatternSizeProperty); set => SetValue(PatternSizeProperty, value); }
    public MonitorMotionDirection MotionDirection { get => GetValue(MotionDirectionProperty); set => SetValue(MotionDirectionProperty, value); }
    public double MotionSpeed { get => GetValue(MotionSpeedProperty); set => SetValue(MotionSpeedProperty, value); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ModeProperty)
        {
            UpdateAnimationState();
        }
    }

    public override void Render(DrawingContext context)
    {
        Rect bounds = new(Bounds.Size);
        switch (Mode)
        {
            case MonitorTestMode.Grayscale:
                byte gray = ToByte(GrayValue);
                context.FillRectangle(new SolidColorBrush(Color.FromRgb(gray, gray, gray)), bounds);
                break;
            case MonitorTestMode.Rgb:
                context.FillRectangle(new SolidColorBrush(Color.FromRgb(ToByte(Red), ToByte(Green), ToByte(Blue))), bounds);
                break;
            case MonitorTestMode.Gradient:
                context.FillRectangle(CreateGradientBrush(), bounds);
                break;
            case MonitorTestMode.Pattern:
                DrawPattern(context, bounds);
                break;
            case MonitorTestMode.Motion:
                DrawMotion(context, bounds);
                break;
        }
    }

    private IBrush CreateGradientBrush()
    {
        (RelativePoint start, RelativePoint end) = GradientDirection switch
        {
            MonitorGradientDirection.Horizontal => (new(0, 0.5, RelativeUnit.Relative), new(1, 0.5, RelativeUnit.Relative)),
            MonitorGradientDirection.ForwardDiagonal => (RelativePoint.TopLeft, RelativePoint.BottomRight),
            MonitorGradientDirection.BackwardDiagonal =>
                (new RelativePoint(1, 0, RelativeUnit.Relative), new RelativePoint(0, 1, RelativeUnit.Relative)),
            _ => (new(0.5, 0, RelativeUnit.Relative), new(0.5, 1, RelativeUnit.Relative))
        };

        return new LinearGradientBrush
        {
            StartPoint = start,
            EndPoint = end,
            GradientStops = new GradientStops
            {
                new(GradientStart, 0),
                new(GradientEnd, 1)
            }
        };
    }

    private void DrawPattern(DrawingContext context, Rect bounds)
    {
        context.FillRectangle(Brushes.White, bounds);
        double size = Math.Max(1, Math.Round(PatternSize));
        double period = size * 2;
        DrawingGroup drawing = new();

        if (Pattern == MonitorPattern.HorizontalLines)
        {
            drawing.Children.Add(CreateBlackRectangle(new Rect(0, 0, period, size)));
        }
        else if (Pattern == MonitorPattern.VerticalLines)
        {
            drawing.Children.Add(CreateBlackRectangle(new Rect(0, 0, size, period)));
        }
        else
        {
            drawing.Children.Add(CreateBlackRectangle(new Rect(0, 0, size, size)));
            drawing.Children.Add(CreateBlackRectangle(new Rect(size, size, size, size)));
        }

        DrawingBrush patternBrush = new()
        {
            Drawing = drawing,
            SourceRect = new RelativeRect(0, 0, period, period, RelativeUnit.Absolute),
            DestinationRect = new RelativeRect(0, 0, period, period, RelativeUnit.Absolute),
            Stretch = Stretch.None,
            TileMode = TileMode.Tile
        };
        context.FillRectangle(patternBrush, bounds);
    }

    private static GeometryDrawing CreateBlackRectangle(Rect rectangle)
    {
        return new GeometryDrawing
        {
            Brush = Brushes.Black,
            Geometry = new RectangleGeometry(rectangle)
        };
    }

    private void DrawMotion(DrawingContext context, Rect bounds)
    {
        const double barSize = 50;
        const double period = barSize * 2;
        context.FillRectangle(Brushes.White, bounds);
        double start = _motionOffset % period - barSize;

        if (MotionDirection == MonitorMotionDirection.VerticalBars)
        {
            for (double x = start; x < bounds.Width; x += period)
            {
                context.FillRectangle(Brushes.Black, new Rect(x, 0, barSize, bounds.Height));
            }
        }
        else
        {
            for (double y = start; y < bounds.Height; y += period)
            {
                context.FillRectangle(Brushes.Black, new Rect(0, y, bounds.Width, barSize));
            }
        }
    }

    private void UpdateAnimationState()
    {
        if (Mode == MonitorTestMode.Motion)
        {
            _motionOffset = 0;
            _lastElapsed = TimeSpan.Zero;
            _animationTime.Restart();
            _animationTimer.Start();
        }
        else
        {
            _animationTimer.Stop();
            _animationTime.Stop();
        }
    }

    private void OnAnimationTick(object? sender, EventArgs e)
    {
        TimeSpan elapsed = _animationTime.Elapsed;
        _motionOffset += (elapsed - _lastElapsed).TotalSeconds * Math.Clamp(MotionSpeed, 100, 2000);
        _lastElapsed = elapsed;
        InvalidateVisual();
    }

    private static byte ToByte(double value) => (byte)Math.Round(Math.Clamp(value, 0, 255));
}
