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
using System.Collections.Specialized;
using System.Globalization;

namespace ShareX.Tools.Controls;

public sealed class NetworkMonitorChart : Control
{
    public static readonly StyledProperty<IEnumerable<NetworkMonitorSample>?> SamplesProperty =
        AvaloniaProperty.Register<NetworkMonitorChart, IEnumerable<NetworkMonitorSample>?>(nameof(Samples));
    public static readonly StyledProperty<TimeSpan?> TimeRangeProperty =
        AvaloniaProperty.Register<NetworkMonitorChart, TimeSpan?>(nameof(TimeRange));
    public static readonly StyledProperty<string> EmptyTextProperty =
        AvaloniaProperty.Register<NetworkMonitorChart, string>(nameof(EmptyText), string.Empty);

    private INotifyCollectionChanged? _observableSamples;

    static NetworkMonitorChart()
    {
        AffectsRender<NetworkMonitorChart>(SamplesProperty, TimeRangeProperty, EmptyTextProperty);
    }

    public IEnumerable<NetworkMonitorSample>? Samples
    {
        get => GetValue(SamplesProperty);
        set => SetValue(SamplesProperty, value);
    }

    public TimeSpan? TimeRange
    {
        get => GetValue(TimeRangeProperty);
        set => SetValue(TimeRangeProperty, value);
    }

    public string EmptyText
    {
        get => GetValue(EmptyTextProperty);
        set => SetValue(EmptyTextProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SamplesProperty)
        {
            if (_observableSamples != null)
            {
                _observableSamples.CollectionChanged -= OnSamplesChanged;
            }
            _observableSamples = change.NewValue as INotifyCollectionChanged;
            if (_observableSamples != null)
            {
                _observableSamples.CollectionChanged += OnSamplesChanged;
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Bounds.Width < 120 || Bounds.Height < 80)
        {
            return;
        }

        SolidColorBrush textBrush = new(Color.FromRgb(142, 148, 158));
        SolidColorBrush gridBrush = new(Color.FromArgb(55, 142, 148, 158));
        SolidColorBrush successBrush = new(Color.FromRgb(48, 187, 108));
        SolidColorBrush failureBrush = new(Color.FromRgb(235, 78, 78));
        Pen gridPen = new(gridBrush, 1);
        Pen linePen = new(successBrush, 2);
        Rect plot = new(52, 12, Math.Max(1, Bounds.Width - 64), Math.Max(1, Bounds.Height - 42));

        DateTimeOffset now = DateTimeOffset.Now;
        List<NetworkMonitorSample> allSamples = Samples?.OrderBy(x => x.Timestamp).ToList() ?? [];
        DateTimeOffset start = GetStartTime(allSamples, now);
        List<NetworkMonitorSample> visible = allSamples.Where(x => x.Timestamp >= start && x.Timestamp <= now).ToList();

        if (visible.Count == 0)
        {
            DrawCenteredText(context, string.IsNullOrWhiteSpace(EmptyText) ? "—" : EmptyText, textBrush, plot);
            return;
        }

        DateTimeOffset end = now;
        if (end - start < TimeSpan.FromSeconds(1))
        {
            start = end - TimeSpan.FromMinutes(1);
        }

        double observedMaximum = visible
            .Where(x => x.Success && x.LatencyMilliseconds != null)
            .Select(x => x.LatencyMilliseconds!.Value)
            .DefaultIfEmpty(0)
            .Max();
        double maximumLatency = Math.Max(50, Math.Ceiling(observedMaximum / 25) * 25);

        const int horizontalGridLines = 4;
        for (int index = 0; index <= horizontalGridLines; index++)
        {
            double ratio = index / (double)horizontalGridLines;
            double y = plot.Bottom - plot.Height * ratio;
            context.DrawLine(gridPen, new Point(plot.Left, y), new Point(plot.Right, y));
            string label = string.Format(CultureInfo.CurrentCulture,
                Localization.Strings.NetworkMonitorViewModel_Milliseconds, maximumLatency * ratio);
            DrawText(context, label, textBrush, new Point(2, y - 7), 11);
        }

        DrawTimeLabel(context, start, textBrush, plot.Left, plot.Bottom + 8, TextAlignment.Left);
        DrawTimeLabel(context, start + (end - start) / 2, textBrush, plot.Center.X, plot.Bottom + 8, TextAlignment.Center);
        DrawTimeLabel(context, end, textBrush, plot.Right, plot.Bottom + 8, TextAlignment.Right);

        Point? previousPoint = null;
        foreach (NetworkMonitorSample sample in visible)
        {
            double xRatio = Math.Clamp((sample.Timestamp - start).TotalMilliseconds / (end - start).TotalMilliseconds, 0, 1);
            double x = plot.Left + plot.Width * xRatio;
            if (!sample.Success || sample.LatencyMilliseconds == null)
            {
                context.DrawLine(new Pen(failureBrush, 1.5), new Point(x, plot.Bottom - 12), new Point(x, plot.Bottom));
                previousPoint = null;
                continue;
            }

            double yRatio = Math.Clamp(sample.LatencyMilliseconds.Value / maximumLatency, 0, 1);
            Point point = new(x, plot.Bottom - plot.Height * yRatio);
            if (previousPoint != null)
            {
                context.DrawLine(linePen, previousPoint.Value, point);
            }
            previousPoint = point;
        }
    }

    private DateTimeOffset GetStartTime(IReadOnlyList<NetworkMonitorSample> samples, DateTimeOffset now)
    {
        if (TimeRange != null)
        {
            return now - TimeRange.Value;
        }
        return samples.Count > 0 ? samples[0].Timestamp : now - TimeSpan.FromMinutes(1);
    }

    private static void DrawTimeLabel(
        DrawingContext context,
        DateTimeOffset timestamp,
        IBrush brush,
        double x,
        double y,
        TextAlignment alignment)
    {
        FormattedText text = CreateText(timestamp.LocalDateTime.ToString("t"), brush, 11);
        double drawX = alignment switch
        {
            TextAlignment.Center => x - text.Width / 2,
            TextAlignment.Right => x - text.Width,
            _ => x
        };
        context.DrawText(text, new Point(drawX, y));
    }

    private static void DrawCenteredText(DrawingContext context, string value, IBrush brush, Rect bounds)
    {
        FormattedText text = CreateText(value, brush, 12);
        context.DrawText(text, new Point(bounds.Center.X - text.Width / 2, bounds.Center.Y - text.Height / 2));
    }

    private static void DrawText(DrawingContext context, string value, IBrush brush, Point point, double size)
    {
        context.DrawText(CreateText(value, brush, size), point);
    }

    private static FormattedText CreateText(string value, IBrush brush, double size) => new(
        value,
        CultureInfo.CurrentCulture,
        FlowDirection.LeftToRight,
        new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.Normal),
        size,
        brush);

    private void OnSamplesChanged(object? sender, NotifyCollectionChangedEventArgs e) => InvalidateVisual();
}
