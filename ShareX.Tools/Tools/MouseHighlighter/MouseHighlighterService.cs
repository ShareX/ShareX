#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using Avalonia.Controls;
using Avalonia.Threading;
using ShareX.HelpersLib;
using System.Diagnostics;
using DrawingPoint = System.Drawing.Point;

namespace ShareX.Tools;

internal sealed class MouseHighlight
{
    public DrawingPoint Position { get; set; }
    public MouseHighlightButton Button { get; init; }
    public double Started { get; init; }
    public double? Released { get; set; }
    public bool Crosshairs { get; init; }
}

// Animation state stays on the Avalonia UI thread. The mouse hook has its own
// message loop and only communicates through the nonblocking input buffer.
internal sealed class MouseHighlighterService : IDisposable
{
    private readonly List<MouseHighlighterOverlayWindow> _windows = [];
    private readonly List<MouseHighlight> _highlights = [];
    private readonly DispatcherTimer _timer;
    private readonly MouseHighlighterMouseHook? _hook;
    private readonly MouseHighlighterInputBuffer _input;
    private readonly Window _screenProbe = new();
    private readonly long _startTimestamp = Stopwatch.GetTimestamp();
    private readonly MouseHighlight?[] _heldHighlights = new MouseHighlight?[3];

    public MouseHighlighterOptions Options { get; private set; }
    public DrawingPoint CursorPosition { get; private set; }
    public IReadOnlyList<MouseHighlight> Highlights => _highlights;
    public double Time => Stopwatch.GetElapsedTime(_startTimestamp).TotalMilliseconds;

    public MouseHighlighterService(MouseHighlighterOptions options)
    {
        Options = options;
        options.Validate();
        CursorPosition = CaptureHelpers.GetCursorPosition();
        _input = new MouseHighlighterInputBuffer(CursorPosition);
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000d / 60) };
        _timer.Tick += OnTick;
        try
        {
            _hook = new MouseHighlighterMouseHook(_input);
            CreateOverlays();
            _screenProbe.Screens.Changed += OnScreensChanged;
            _timer.Start();
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public void UpdateOptions(MouseHighlighterOptions options)
    {
        options.Validate();
        Options = options;
        _highlights.Clear();
        Array.Clear(_heldHighlights);
        _input.DiscardPendingEvents();
    }

    private void CreateOverlays()
    {
        // Each monitor has its own window and scale. Coordinates and option
        // sizes stay in physical pixels, including monitors left of the origin.
        foreach (var screen in _screenProbe.Screens.All)
        {
            MouseHighlighterOverlayWindow window = new(this, screen.Bounds);
            _windows.Add(window);
            window.Refresh();
        }
    }

    private void OnScreensChanged(object? sender, EventArgs e)
    {
        try
        {
            foreach (MouseHighlighterOverlayWindow window in _windows) window.Dispose();
            _windows.Clear();
            CreateOverlays();
        }
        catch (Exception ex) { MouseHighlighterManager.StopOnError(this, ex); }
    }

    private void MoveCursor(DrawingPoint point)
    {
        CursorPosition = point;
        bool follow = Options.Mode != MouseHighlightMode.Ripple || Options.FollowCursorWhileHeld;
        if (follow)
        {
            foreach (MouseHighlight? highlight in _heldHighlights)
            {
                if (highlight != null) highlight.Position = point;
            }
        }
    }

    private void ProcessPendingInput()
    {
        if (_input.ConsumeOverflow())
        {
            _input.DiscardPendingEvents();
            _highlights.Clear();
            Array.Clear(_heldHighlights);
            MoveCursor(_input.Position);
            int pressed = _input.PressedButtons;
            for (int i = 0; i < _heldHighlights.Length; i++)
            {
                if ((pressed & (1 << i)) != 0) Press((MouseHighlightButton)i, CursorPosition, Time);
            }
        }
        // Limit work per frame even if input arrives continuously.
        for (int i = 0; i < MouseHighlighterInputBuffer.Capacity && _input.TryRead(out MouseHighlighterButtonEvent input); i++)
        {
            MoveCursor(input.Position);
            double time = Stopwatch.GetElapsedTime(_startTimestamp, input.Timestamp).TotalMilliseconds;
            if (input.Pressed) Press(input.Button, input.Position, time);
            else Release(input.Button, input.Position, time);
        }
        MoveCursor(_input.Position);
    }

    private void Press(MouseHighlightButton button, DrawingPoint point, double time)
    {
        MouseHighlight? previous = _heldHighlights[(int)button];
        if (previous != null) previous.Released = time;
        MouseHighlight highlight = new() { Position = point, Button = button, Started = time };
        _highlights.Add(highlight);
        _heldHighlights[(int)button] = highlight;
    }

    private void Release(MouseHighlightButton button, DrawingPoint point, double time)
    {
        MouseHighlight? highlight = _heldHighlights[(int)button];
        if (highlight == null) return;
        highlight.Released = time;
        _heldHighlights[(int)button] = null;

        if (Options.Mode == MouseHighlightMode.Ripple && Options.ShowReleaseCrosshairs(button))
        {
            _highlights.Add(new MouseHighlight
            {
                Position = point, Button = button, Started = time, Released = time, Crosshairs = true
            });
        }
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (_hook?.Failure is Exception failure)
        {
            MouseHighlighterManager.StopOnError(this, failure);
            return;
        }
        ProcessPendingInput();
        double time = Time;
        _highlights.RemoveAll(highlight => highlight.Released.HasValue &&
            time - highlight.Released.Value >= (Options.Mode == MouseHighlightMode.Ripple
                ? Options.RippleDuration : Options.FadeDelay + Options.FadeDuration));
        // Bound retained effects even under very high click rates.
        while (_highlights.Count > 64)
        {
            int index = _highlights.FindIndex(highlight => highlight.Released.HasValue);
            if (index < 0) break;
            _highlights.RemoveAt(index);
        }
        try
        {
            foreach (MouseHighlighterOverlayWindow window in _windows) window.Refresh();
        }
        catch (Exception ex) { MouseHighlighterManager.StopOnError(this, ex); }
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTick;
        _hook?.Dispose();
        _screenProbe.Screens.Changed -= OnScreensChanged;
        foreach (MouseHighlighterOverlayWindow window in _windows) window.Dispose();
        _screenProbe.Close();
        _windows.Clear();
        _highlights.Clear();
    }
}