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

using System;
using System.Collections.Generic;
using System.Threading;

namespace ShareX.Platform;

/// <summary>A top level window as seen by the window manager.</summary>
/// <param name="Handle">Native handle: HWND on Windows, CGWindowID on macOS, X11 window id, or compositor address on Wayland.</param>
/// <param name="Bounds">Visible bounds on the virtual desktop in physical pixels, excluding invisible resize borders where the OS reports them.</param>
public sealed record PlatformWindow(long Handle, string Title, string? ProcessName, int? ProcessId, PlatformRectangle Bounds, bool IsMinimized);

/// <summary>A rectangle region capture can snap to.</summary>
/// <param name="Handle">The window or control the rectangle belongs to.</param>
/// <param name="Bounds">Visible bounds on the virtual desktop, in the same coordinates as <see cref="PlatformWindow.Bounds"/>.</param>
/// <param name="Window">The top level window when the rectangle is a whole window; null for client areas and controls.</param>
public sealed record SnapTarget(long Handle, PlatformRectangle Bounds, PlatformWindow? Window)
{
    public bool IsWindow => Window != null;

    /// <summary>Snap targets for platforms that only know top level windows: one per visible window, topmost first.</summary>
    public static IReadOnlyList<SnapTarget> FromWindows(IEnumerable<PlatformWindow> windows, long ignoredHandle)
    {
        List<SnapTarget> targets = new List<SnapTarget>();

        foreach (PlatformWindow window in windows)
        {
            if (window.Handle != ignoredHandle && !window.IsMinimized && !window.Bounds.IsEmpty)
            {
                targets.Add(new SnapTarget(window.Handle, window.Bounds, window));
            }
        }

        return targets;
    }
}

/// <summary>Memory to draw the next overlay frame into: <see cref="Height"/> premultiplied BGRA rows of <see cref="Stride"/> bytes.</summary>
public readonly record struct OverlayBuffer(IntPtr Pixels, int Stride, int Height);

/// <summary>
/// An always on top, click through surface over one screen, for effects drawn over other applications such as the mouse
/// highlighter. Use from the UI thread.
/// </summary>
public interface IScreenOverlay : IDisposable
{
    /// <summary>A buffer of at least <paramref name="width"/> by <paramref name="height"/> pixels, valid until the next call.</summary>
    OverlayBuffer GetBuffer(int width, int height);

    /// <summary>Shows the top left area of the buffer at <paramref name="area"/> on the desktop.</summary>
    void Present(PlatformRectangle area);

    void Hide();
}

/// <summary>Window enumeration for window and region capture.</summary>
public interface IWindowService
{
    FeatureSupport Support { get; }

    /// <summary>Visible top level windows in Z order, topmost first where the platform reports it.</summary>
    IReadOnlyList<PlatformWindow> GetWindows();

    PlatformWindow? GetActiveWindow();

    /// <summary>
    /// The window with keyboard focus, unfiltered (unlike <see cref="GetActiveWindow"/>, it can be a tool window or untitled), or 0
    /// where the platform does not say.
    /// </summary>
    long GetActiveWindowHandle();

    /// <summary>
    /// A window's visible bounds on the virtual desktop (without invisible resize borders), or null when the window is gone or
    /// the platform cannot locate it.
    /// </summary>
    PlatformRectangle? GetWindowBounds(long windowHandle);

    /// <summary>The bounds of a window's content area, without title bar and borders. Null where the platform does not know it.</summary>
    PlatformRectangle? GetClientBounds(long windowHandle);

    /// <summary>
    /// The mouse pointer on the virtual desktop, in the same coordinates as IScreenCaptureService.GetScreens, or null where the session does not reveal it
    /// (for example Wayland compositors other than Hyprland).
    /// </summary>
    PlatformPoint? GetCursorPosition();

    /// <summary>
    /// Pixels of ShareX's own windows per screen coordinate at <paramref name="point"/>. 1 on Windows, macOS, X11 and sway, where
    /// window positions use screen coordinates. On Hyprland with xwayland:force_zero_scaling, ShareX's XWayland windows are placed
    /// in device pixels while screen coordinates are layout coordinates, so it is the monitor's scale.
    /// </summary>
    double GetOwnWindowPixelScale(PlatformPoint point);

    /// <summary>
    /// Releases the mouse capture another window of this process holds, so a newly opened window receives clicks straight away.
    /// Only Windows has application-held mouse capture; elsewhere this does nothing.
    /// </summary>
    void ReleaseMouseCapture();

    /// <summary>
    /// Draws the user's attention to one of ShareX's own windows without activating it: flashes its task bar button
    /// <paramref name="count"/> times on Windows, marks it urgent on Linux. Returns false where the platform cannot.
    /// </summary>
    bool RequestAttention(long windowHandle, int count) => false;

    /// <summary>
    /// Keeps the pointer inside the window (for single monitor region capture). Returns false where the platform does not let
    /// applications confine the pointer, which includes every Wayland compositor.
    /// </summary>
    bool ConfineCursor(long windowHandle);

    void ReleaseCursorConfinement();

    /// <summary>
    /// Rectangles region capture snaps to, topmost first: windows and, where the platform can see inside other applications'
    /// windows (Windows), their client areas and, with <paramref name="includeControls"/>, child controls.
    /// </summary>
    /// <param name="ignoredHandle">A window to leave out, normally the region capture window itself.</param>
    IReadOnlyList<SnapTarget> GetSnapTargets(bool includeControls, long ignoredHandle, CancellationToken cancellationToken = default);

    /// <summary>Moves the mouse pointer. Returns false where applications may not move it (most Wayland compositors).</summary>
    bool SetCursorPosition(PlatformPoint position);

    /// <summary>Brings a window to the front and gives it keyboard focus. Returns false when the platform refused.</summary>
    bool ActivateWindow(long windowHandle);

    /// <summary>Restores a minimised window to its normal size. Returns false when the platform refused or cannot.</summary>
    bool RestoreWindow(long windowHandle);

    /// <summary>
    /// Makes one of ShareX's own windows an overlay: hidden from the task bar and window switcher and, with
    /// <paramref name="clickThrough"/>, transparent to mouse input. Returns false where the platform has no such control.
    /// </summary>
    bool SetOverlayStyle(long windowHandle, bool clickThrough);

    /// <summary>
    /// Limits one of ShareX's own windows to the union of <paramref name="visibleAreas"/> (window relative, physical pixels), so
    /// clicks outside them reach the windows behind. Returns false where the platform cannot shape windows; the window then
    /// stays rectangular and relies on a transparent background.
    /// </summary>
    bool SetWindowShape(long windowHandle, IReadOnlyList<PlatformRectangle> visibleAreas);

    /// <summary>
    /// Makes ShareX the active application, so its newest window receives the keyboard. Needed on macOS, where a window activated
    /// while another application is in front (after a global hotkey) is not given key events. Elsewhere activating the window is
    /// enough and this does nothing. Call from the UI thread.
    /// </summary>
    void ActivateOwnApplication()
    {
    }

    /// <summary>Whether <see cref="CreateOverlay"/> works.</summary>
    FeatureSupport OverlaySupport { get; }

    /// <summary>Creates a hidden overlay for the screen at <paramref name="screenBounds"/>.</summary>
    IScreenOverlay CreateOverlay(PlatformRectangle screenBounds);
}
