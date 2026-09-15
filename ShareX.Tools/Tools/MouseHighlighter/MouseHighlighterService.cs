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
    public bool Secondary { get; init; }
    public double Started { get; init; }
    public double? Released { get; set; }
    public bool Crosshairs { get; init; }
}

// Created and disposed on the Avalonia UI thread, which pumps the mouse hook.
internal sealed class MouseHighlighterService : IDisposable
{
    private readonly List<MouseHighlighterOverlayWindow> _windows = [];
    private readonly List<MouseHighlight> _highlights = [];
    private readonly DispatcherTimer _timer;
    private readonly MouseHighlighterMouseHook? _hook;
    private readonly Window _screenProbe = new();
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private MouseHighlight? _primaryHeld;
    private MouseHighlight? _secondaryHeld;

    public MouseHighlighterOptions Options { get; private set; }
    public DrawingPoint CursorPosition { get; private set; }
    public IReadOnlyList<MouseHighlight> Highlights => _highlights;
    public double Time => _clock.Elapsed.TotalMilliseconds;

    public MouseHighlighterService(MouseHighlighterOptions options)
    {
        Options = options;
        options.Validate();
        CursorPosition = CaptureHelpers.GetCursorPosition();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000d / 60) };
        _timer.Tick += OnTick;
        try
        {
            _hook = new MouseHighlighterMouseHook(OnMouseInput);
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
        _primaryHeld = _secondaryHeld = null;
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

    private void OnMouseInput(int message, DrawingPoint point)
    {
        CursorPosition = point;
        bool follow = Options.Mode != MouseHighlightMode.Ripple || Options.FollowCursorWhileHeld;
        if (follow)
        {
            if (_primaryHeld != null) _primaryHeld.Position = point;
            if (_secondaryHeld != null) _secondaryHeld.Position = point;
        }

        // Respect the Windows primary/secondary button assignment.
        bool swapped = NativeMethods.GetSystemMetrics(SystemMetric.SM_SWAPBUTTON) != 0;
        switch (message)
        {
            case 0x0201: Press(swapped, point); break; // WM_LBUTTONDOWN
            case 0x0204: Press(!swapped, point); break; // WM_RBUTTONDOWN
            case 0x0202: Release(swapped, point); break;
            case 0x0205: Release(!swapped, point); break;
        }
    }

    private void Press(bool secondary, DrawingPoint point)
    {
        MouseHighlight? previous = secondary ? _secondaryHeld : _primaryHeld;
        if (previous != null) previous.Released = Time;
        MouseHighlight highlight = new() { Position = point, Secondary = secondary, Started = Time };
        _highlights.Add(highlight);
        if (secondary) _secondaryHeld = highlight;
        else _primaryHeld = highlight;
    }

    private void Release(bool secondary, DrawingPoint point)
    {
        MouseHighlight? highlight = secondary ? _secondaryHeld : _primaryHeld;
        if (highlight == null) return;
        highlight.Released = Time;
        if (secondary) _secondaryHeld = null;
        else _primaryHeld = null;

        if (secondary && Options.Mode == MouseHighlightMode.Ripple && Options.ShowSecondaryReleaseCrosshairs)
        {
            _highlights.Add(new MouseHighlight
            {
                Position = point, Secondary = true, Started = Time, Released = Time, Crosshairs = true
            });
        }
    }

    private void OnTick(object? sender, EventArgs e)
    {
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
