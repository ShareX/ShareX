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

using ShareX.Platform.Windows.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ShareX.Platform.Windows;

/// <summary>Top level windows from EnumWindows, skipping hidden, cloaked (other virtual desktops, suspended UWP) and tool windows.</summary>
public sealed unsafe class WindowsWindowService : IWindowService
{
    private static readonly string[] IgnoredClassNames = ["Progman", "Button"];

    public FeatureSupport Support => FeatureSupport.Supported;

    public IReadOnlyList<PlatformWindow> GetWindows()
    {
        List<IntPtr> handles = new List<IntPtr>();
        GCHandle handle = GCHandle.Alloc(handles);

        try
        {
            Win32.EnumWindows(&OnWindow, GCHandle.ToIntPtr(handle));
        }
        finally
        {
            handle.Free();
        }

        List<PlatformWindow> windows = new List<PlatformWindow>(handles.Count);

        // EnumWindows walks the Z order from the top.
        foreach (IntPtr hwnd in handles)
        {
            PlatformWindow? window = ReadWindow(hwnd);

            if (window != null)
            {
                windows.Add(window);
            }
        }

        return windows;
    }

    public PlatformPoint? GetCursorPosition() => Win32.GetCursorPos(out Win32.POINT point) ? new PlatformPoint(point.X, point.Y) : null;

    public void ReleaseMouseCapture() => Win32.ReleaseCapture();

    public bool ConfineCursor(long windowHandle)
    {
        if (!Win32.GetWindowRect((IntPtr)windowHandle, out Win32.RECT rect) || rect.Right <= rect.Left || rect.Bottom <= rect.Top)
        {
            return false;
        }

        return Win32.ClipCursor(&rect);
    }

    public void ReleaseCursorConfinement() => Win32.ClipCursor(null);

    public IReadOnlyList<SnapTarget> GetSnapTargets(bool includeControls, long ignoredHandle, CancellationToken cancellationToken = default) =>
        new SnapTargetCollector(includeControls, (IntPtr)ignoredHandle, cancellationToken).Collect();

    public bool SetCursorPosition(PlatformPoint position) => Win32.SetCursorPos(position.X, position.Y);

    public bool RestoreWindow(long windowHandle)
    {
        IntPtr hwnd = (IntPtr)windowHandle;

        if (Win32.IsIconic(hwnd))
        {
            Win32.ShowWindow(hwnd, Win32.SW_RESTORE);
        }

        return true;
    }

    public bool ActivateWindow(long windowHandle) => windowHandle != 0 && Win32.SetForegroundWindow((IntPtr)windowHandle);

    public bool SetOverlayStyle(long windowHandle, bool clickThrough)
    {
        IntPtr hwnd = (IntPtr)windowHandle;
        nint style = Win32.GetWindowLongPtr(hwnd, Win32.GWL_EXSTYLE) | (nint)Win32.WS_EX_TOOLWINDOW;

        if (clickThrough)
        {
            style |= (nint)Win32.WS_EX_TRANSPARENT;
        }

        Win32.SetWindowLongPtr(hwnd, Win32.GWL_EXSTYLE, style);
        return true;
    }

    public bool SetWindowShape(long windowHandle, IReadOnlyList<PlatformRectangle> visibleAreas)
    {
        IntPtr region = Win32.CreateRectRgn(0, 0, 0, 0);

        if (region == IntPtr.Zero)
        {
            return false;
        }

        foreach (PlatformRectangle area in visibleAreas)
        {
            IntPtr part = Win32.CreateRectRgn(area.X, area.Y, area.Right, area.Bottom);

            if (part != IntPtr.Zero)
            {
                Win32.CombineRgn(region, region, part, Win32.RGN_OR);
                Win32.DeleteObject(part);
            }
        }

        // The system owns the region once SetWindowRgn succeeds.
        if (Win32.SetWindowRgn((IntPtr)windowHandle, region, true) == 0)
        {
            Win32.DeleteObject(region);
            return false;
        }

        return true;
    }

    public FeatureSupport OverlaySupport => FeatureSupport.Supported;

    public IScreenOverlay CreateOverlay(PlatformRectangle screenBounds) => new WindowsScreenOverlay(screenBounds);

    public long GetActiveWindowHandle() => Win32.GetForegroundWindow().ToInt64();

    public PlatformRectangle? GetWindowBounds(long windowHandle)
    {
        PlatformRectangle bounds = GetBounds((IntPtr)windowHandle);
        return bounds.IsEmpty ? null : bounds;
    }

    public PlatformRectangle? GetClientBounds(long windowHandle)
    {
        PlatformRectangle bounds = GetClientBounds((IntPtr)windowHandle);
        return bounds.IsEmpty ? null : bounds;
    }

    public PlatformWindow? GetActiveWindow()
    {
        IntPtr hwnd = Win32.GetForegroundWindow();
        return hwnd != IntPtr.Zero ? ReadWindow(hwnd) : null;
    }

    [UnmanagedCallersOnly]
    private static int OnWindow(IntPtr hwnd, nint data)
    {
        ((List<IntPtr>)GCHandle.FromIntPtr(data).Target!).Add(hwnd);
        return 1;
    }

    private static PlatformWindow? ReadWindow(IntPtr hwnd)
    {
        if (!Win32.IsWindowVisible(hwnd) || IsCloaked(hwnd) || (Win32.GetWindowLongPtr(hwnd, Win32.GWL_EXSTYLE) & Win32.WS_EX_TOOLWINDOW) != 0)
        {
            return null;
        }

        string title = GetTitle(hwnd);

        // The desktop (Progman) has a title but is not a window anyone means to pick.
        if (title.Length == 0 || IgnoredClassNames.Contains(GetClassName(hwnd), StringComparer.OrdinalIgnoreCase))
        {
            return null;
        }

        PlatformRectangle bounds = GetBounds(hwnd);

        if (bounds.IsEmpty)
        {
            return null;
        }

        Win32.GetWindowThreadProcessId(hwnd, out uint processId);
        return new PlatformWindow(hwnd.ToInt64(), title, GetProcessName(processId), (int)processId, bounds, Win32.IsIconic(hwnd));
    }

    internal static string GetTitle(IntPtr hwnd)
    {
        int length = Win32.GetWindowTextLength(hwnd);

        if (length <= 0)
        {
            return "";
        }

        char[] buffer = new char[length + 1];

        fixed (char* chars = buffer)
        {
            int copied = Win32.GetWindowText(hwnd, chars, buffer.Length);
            return new string(buffer, 0, copied);
        }
    }

    /// <summary>The visible frame. GetWindowRect includes the invisible resize borders of Windows 10 and later.</summary>
    internal static PlatformRectangle GetBounds(IntPtr hwnd)
    {
        Win32.RECT rect;

        if (Win32.DwmGetWindowAttribute(hwnd, Win32.DWMWA_EXTENDED_FRAME_BOUNDS, &rect, sizeof(Win32.RECT)) == 0)
        {
            return rect.ToRectangle();
        }

        return Win32.GetWindowRect(hwnd, out rect) ? rect.ToRectangle() : PlatformRectangle.Empty;
    }

    internal static PlatformRectangle GetClientBounds(IntPtr hwnd)
    {
        if (!Win32.GetClientRect(hwnd, out Win32.RECT rect))
        {
            return PlatformRectangle.Empty;
        }

        Win32.POINT origin = new Win32.POINT { X = rect.Left, Y = rect.Top };
        Win32.ClientToScreen(hwnd, ref origin);
        return new PlatformRectangle(origin.X, origin.Y, rect.Right - rect.Left, rect.Bottom - rect.Top);
    }

    internal static string GetClassName(IntPtr hwnd)
    {
        char* buffer = stackalloc char[256];
        int length = Win32.GetClassName(hwnd, buffer, 256);
        return length > 0 ? new string(buffer, 0, length) : "";
    }

    internal static PlatformWindow? ToPlatformWindow(IntPtr hwnd, PlatformRectangle bounds)
    {
        Win32.GetWindowThreadProcessId(hwnd, out uint processId);
        return new PlatformWindow(hwnd.ToInt64(), GetTitle(hwnd), GetProcessName(processId), (int)processId, bounds, Win32.IsIconic(hwnd));
    }

    internal static bool IsCloaked(IntPtr hwnd)
    {
        int cloaked = 0;
        return Win32.DwmGetWindowAttribute(hwnd, Win32.DWMWA_CLOAKED, &cloaked, sizeof(int)) == 0 && cloaked != 0;
    }

    internal static string? GetProcessName(uint processId)
    {
        try
        {
            using Process process = Process.GetProcessById((int)processId);
            return process.ProcessName;
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            return null;
        }
    }
}

/// <summary>
/// Windows, their client areas and optionally their child controls, as region capture's snap targets. Ported from ShareX's
/// WindowsRectangleList: same filters and the same rule that a control hidden inside an earlier rectangle is dropped.
/// </summary>
internal sealed unsafe class SnapTargetCollector
{
    // NVIDIA GeForce Overlay DT
    private static readonly string[] IgnoredClassNames = ["CEF-OSC-WIDGET"];

    private readonly bool includeControls;
    private readonly IntPtr ignoredHandle;
    private readonly CancellationToken cancellationToken;
    private readonly List<SnapTarget> found = new List<SnapTarget>();
    private readonly HashSet<IntPtr> visitedParents = new HashSet<IntPtr>();

    public SnapTargetCollector(bool includeControls, IntPtr ignoredHandle, CancellationToken cancellationToken)
    {
        this.includeControls = includeControls;
        this.ignoredHandle = ignoredHandle;
        this.cancellationToken = cancellationToken;
    }

    public IReadOnlyList<SnapTarget> Collect()
    {
        foreach (IntPtr hwnd in Enumerate(IntPtr.Zero))
        {
            if (!Visit(hwnd, null))
            {
                break;
            }
        }

        List<SnapTarget> result = new List<SnapTarget>(found.Count);

        foreach (SnapTarget target in found)
        {
            if (target.IsWindow || !result.Any(other => other.Bounds.Contains(target.Bounds)))
            {
                result.Add(target);
            }
        }

        return result;
    }

    /// <returns>False once cancelled.</returns>
    private bool Visit(IntPtr hwnd, PlatformRectangle? clip)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        if (hwnd == ignoredHandle || !Win32.IsWindowVisible(hwnd))
        {
            return true;
        }

        bool isWindow = clip == null;
        PlatformRectangle bounds;

        if (isWindow)
        {
            if (WindowsWindowService.IsCloaked(hwnd))
            {
                return true;
            }

            string className = WindowsWindowService.GetClassName(hwnd);

            if (IgnoredClassNames.Any(ignored => className.Equals(ignored, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // Non-activatable tool windows (tiling manager overlays, system auxiliaries) are never what the user means to
            // capture, and snapping to them would put the overlay's title in the file name instead of the application's.
            long exStyle = Win32.GetWindowLongPtr(hwnd, Win32.GWL_EXSTYLE);

            if ((exStyle & Win32.WS_EX_TOOLWINDOW) != 0 && (exStyle & Win32.WS_EX_NOACTIVATE) != 0)
            {
                return true;
            }

            bounds = WindowsWindowService.GetBounds(hwnd);
        }
        else
        {
            bounds = Win32.GetWindowRect(hwnd, out Win32.RECT rect) ? rect.ToRectangle().Intersect(clip!.Value) : PlatformRectangle.Empty;
        }

        if (bounds.IsEmpty)
        {
            return true;
        }

        if (includeControls && visitedParents.Add(hwnd))
        {
            foreach (IntPtr child in Enumerate(hwnd))
            {
                if (!Visit(child, bounds))
                {
                    return false;
                }
            }
        }

        if (isWindow)
        {
            PlatformRectangle client = WindowsWindowService.GetClientBounds(hwnd);

            if (!client.IsEmpty && client != bounds)
            {
                found.Add(new SnapTarget(hwnd.ToInt64(), client, null));
            }

            found.Add(new SnapTarget(hwnd.ToInt64(), bounds, WindowsWindowService.ToPlatformWindow(hwnd, bounds)));
        }
        else
        {
            found.Add(new SnapTarget(hwnd.ToInt64(), bounds, null));
        }

        return true;
    }

    private static List<IntPtr> Enumerate(IntPtr parent)
    {
        List<IntPtr> handles = new List<IntPtr>();
        GCHandle handle = GCHandle.Alloc(handles);

        try
        {
            if (parent == IntPtr.Zero)
            {
                Win32.EnumWindows(&OnWindow, GCHandle.ToIntPtr(handle));
            }
            else
            {
                Win32.EnumChildWindows(parent, &OnWindow, GCHandle.ToIntPtr(handle));
            }
        }
        finally
        {
            handle.Free();
        }

        return handles;
    }

    [UnmanagedCallersOnly]
    private static int OnWindow(IntPtr hwnd, nint data)
    {
        ((List<IntPtr>)GCHandle.FromIntPtr(data).Target!).Add(hwnd);
        return 1;
    }
}
