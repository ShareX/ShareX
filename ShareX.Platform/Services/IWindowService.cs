using System.Collections.Generic;

namespace ShareX.Platform;

/// <summary>A top level window as seen by the window manager.</summary>
/// <param name="Handle">Native handle: HWND on Windows, CGWindowID on macOS, X11 window id, or compositor address on Wayland.</param>
/// <param name="Bounds">Visible bounds on the virtual desktop in physical pixels, excluding invisible resize borders where the OS reports them.</param>
public sealed record PlatformWindow(long Handle, string Title, string? ProcessName, int? ProcessId, PlatformRectangle Bounds, bool IsMinimized);

/// <summary>Window enumeration for window and region capture.</summary>
public interface IWindowService
{
    FeatureSupport Support { get; }

    /// <summary>Visible top level windows in Z order, topmost first where the platform reports it.</summary>
    IReadOnlyList<PlatformWindow> GetWindows();

    PlatformWindow? GetActiveWindow();
}
