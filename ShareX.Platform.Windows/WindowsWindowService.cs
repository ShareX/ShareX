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
using System.Runtime.InteropServices;

namespace ShareX.Platform.Windows;

/// <summary>Top level windows from EnumWindows, skipping hidden, cloaked (other virtual desktops, suspended UWP) and tool windows.</summary>
public sealed unsafe class WindowsWindowService : IWindowService
{
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

        if (title.Length == 0)
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

    private static string GetTitle(IntPtr hwnd)
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
    private static PlatformRectangle GetBounds(IntPtr hwnd)
    {
        Win32.RECT rect;

        if (Win32.DwmGetWindowAttribute(hwnd, Win32.DWMWA_EXTENDED_FRAME_BOUNDS, &rect, sizeof(Win32.RECT)) == 0)
        {
            return rect.ToRectangle();
        }

        return Win32.GetWindowRect(hwnd, out rect) ? rect.ToRectangle() : PlatformRectangle.Empty;
    }

    private static bool IsCloaked(IntPtr hwnd)
    {
        int cloaked = 0;
        return Win32.DwmGetWindowAttribute(hwnd, Win32.DWMWA_CLOAKED, &cloaked, sizeof(int)) == 0 && cloaked != 0;
    }

    private static string? GetProcessName(uint processId)
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
