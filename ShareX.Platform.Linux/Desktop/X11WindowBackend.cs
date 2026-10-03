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

using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>X11: EWMH properties for the window list and active window, Xlib for the pointer and geometry.</summary>
internal sealed class X11WindowBackend : IDesktopWindowBackend
{
    public LinuxWindowService.Backend Kind => LinuxWindowService.Backend.X11;

    public FeatureSupport Support => FeatureSupport.Supported;

    public bool IdentifiesOwnWindowsByProcess => false;

    public IReadOnlyList<PlatformWindow> GetWindows()
    {
        using X11Display? display = X11Display.TryOpen();

        if (display == null)
        {
            return Array.Empty<PlatformWindow>();
        }

        // Stacking order is bottom to top. Reverse it so the topmost window comes first, as on Windows.
        nuint[] clients = display.GetLongProperty(display.Root, "_NET_CLIENT_LIST_STACKING");

        if (clients.Length == 0)
        {
            clients = display.GetLongProperty(display.Root, "_NET_CLIENT_LIST");
        }

        List<PlatformWindow> windows = new List<PlatformWindow>(clients.Length);

        for (int i = clients.Length - 1; i >= 0; i--)
        {
            PlatformWindow? window = ReadWindow(display, clients[i]);

            if (window != null && !window.Bounds.IsEmpty)
            {
                windows.Add(window);
            }
        }

        return windows;
    }

    public IReadOnlyList<PlatformWindow> GetSnapWindows() => GetWindows();

    public IReadOnlyList<ScreenInfo> GetScreens()
    {
        using X11Display? display = X11Display.TryOpen();
        return display?.GetMonitors() ?? (IReadOnlyList<ScreenInfo>)Array.Empty<ScreenInfo>();
    }

    public PlatformWindow? GetActiveWindow()
    {
        using X11Display? display = X11Display.TryOpen();

        if (display == null)
        {
            return null;
        }

        nuint[] active = display.GetLongProperty(display.Root, "_NET_ACTIVE_WINDOW", 1);
        return active.Length == 1 && active[0] != 0 ? ReadWindow(display, active[0]) : null;
    }

    public PlatformPoint? GetCursorPosition()
    {
        using X11Display? display = X11Display.TryOpen();
        return display?.GetPointerPosition();
    }

    public bool SetCursorPosition(PlatformPoint position)
    {
        using X11Display? display = X11Display.TryOpen();

        if (display == null)
        {
            return false;
        }

        X11.XWarpPointer(display.Display, 0, display.Root, 0, 0, 0, 0, position.X, position.Y);
        X11.XFlush(display.Display);
        return true;
    }

    public bool ActivateWindow(long windowHandle)
    {
        using X11Display? display = X11Display.TryOpen();
        return display != null && display.RequestActivation((nuint)windowHandle);
    }

    public PlatformRectangle? GetWindowBounds(long windowHandle)
    {
        using X11Display? display = X11Display.TryOpen();
        return display?.GetWindowBounds((nuint)windowHandle, includeFrame: true);
    }

    /// <summary>X11 knows the client window without the frame the window manager adds.</summary>
    public PlatformRectangle? GetClientBounds(long windowHandle)
    {
        using X11Display? display = X11Display.TryOpen();
        return display?.GetWindowBounds((nuint)windowHandle, includeFrame: false);
    }

    public double GetOwnWindowPixelScale(PlatformPoint point) => 1;

    private static PlatformWindow? ReadWindow(X11Display display, nuint window)
    {
        PlatformRectangle? bounds = display.GetWindowBounds(window, includeFrame: true);

        if (bounds == null)
        {
            return null;
        }

        string title = display.GetStringProperty(window, "_NET_WM_NAME") ?? display.GetStringProperty(window, "WM_NAME") ?? "";
        nuint[] pid = display.GetLongProperty(window, "_NET_WM_PID", 1);
        int? processId = pid.Length == 1 ? (int)pid[0] : null;
        nuint hidden = display.GetAtom("_NET_WM_STATE_HIDDEN");
        bool minimized = display.GetLongProperty(window, "_NET_WM_STATE").Contains(hidden);

        return new PlatformWindow((long)window, title, processId != null ? LinuxWindowService.GetProcessName(processId.Value) : null, processId, bounds.Value, minimized);
    }
}
