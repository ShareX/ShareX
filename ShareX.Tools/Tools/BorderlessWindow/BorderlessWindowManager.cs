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

using ShareX.HelpersLib;
using System.Collections.Concurrent;
using System.Drawing;

namespace ShareX.Tools;

public static class BorderlessWindowManager
{
    private static readonly ConcurrentDictionary<IntPtr, BorderlessWindowInfo> BorderlessWindows = new();

    public static bool ToggleBorderlessWindow(string windowTitle, bool useWorkingArea = false)
    {
        if (string.IsNullOrWhiteSpace(windowTitle))
        {
            return false;
        }

        IntPtr handle = NativeMethods.SearchWindow(windowTitle);
        if (handle == IntPtr.Zero)
        {
            return false;
        }

        ToggleBorderlessWindow(handle, useWorkingArea);
        return true;
    }

    public static void ToggleBorderlessWindow(IntPtr handle, bool useWorkingArea = false)
    {
        if (BorderlessWindows.TryGetValue(handle, out BorderlessWindowInfo? info))
        {
            RestoreWindow(handle, info);
        }
        else
        {
            MakeWindowBorderless(handle, useWorkingArea);
        }
    }

    private static void RestoreWindow(IntPtr handle, BorderlessWindowInfo info)
    {
        WindowInfo window = new(handle);
        if (window.IsMinimized)
        {
            window.Restore();
        }

        window.Style = info.Style;
        window.ExStyle = info.ExStyle;
        window.SetWindowPos(info.Rectangle, SetWindowPosFlags.SWP_FRAMECHANGED |
            SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOZORDER);
        BorderlessWindows.TryRemove(handle, out _);
    }

    private static void MakeWindowBorderless(IntPtr handle, bool useWorkingArea)
    {
        WindowInfo window = new(handle);
        if (window.IsMinimized)
        {
            window.Restore();
        }

        BorderlessWindowInfo info = new(window);
        window.Style &= ~(WindowStyles.WS_CAPTION | WindowStyles.WS_MAXIMIZEBOX |
            WindowStyles.WS_SYSMENU | WindowStyles.WS_THICKFRAME);
        window.ExStyle &= ~(WindowStyles.WS_EX_CLIENTEDGE | WindowStyles.WS_EX_DLGMODALFRAME |
            WindowStyles.WS_EX_STATICEDGE);

        Rectangle bounds = CaptureHelpers.GetScreenBounds(handle, useWorkingArea);
        SetWindowPosFlags flags = SetWindowPosFlags.SWP_FRAMECHANGED |
            SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOZORDER;

        if (bounds.IsEmpty)
        {
            flags |= SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE;
        }

        window.SetWindowPos(bounds, flags);
        BorderlessWindows.TryAdd(handle, info);
    }
}
