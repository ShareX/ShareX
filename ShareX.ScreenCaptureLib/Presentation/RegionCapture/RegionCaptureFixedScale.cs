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

#nullable enable

using ShareX.HelpersLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

/// <summary>
/// Keeps a region capture overlay at the scale it was created with while it is open. On Windows an overlay that spans
/// monitors with different scaling is otherwise switched between their DPIs whenever it is moved or resized. The overlay is
/// sized in device pixels, so following each switch only resizes it again: it flickers between scales and pointer positions
/// map to the wrong screenshot pixels (selecting the empty area of the ShareX main window then captures its sidebar).
/// <para>
/// Windows still gives the window the DPI of the monitor it overlaps most, so the overlay must be created on that monitor
/// (<see cref="GetDpiAnchor"/>). Otherwise Avalonia keeps one scale while Windows uses another and the toolbar, magnifier
/// and drawing land in the wrong place.
/// </para>
/// </summary>
internal static unsafe partial class RegionCaptureFixedScale
{
    private const uint WM_NCDESTROY = 0x0082;
    private const uint WM_DPICHANGED = 0x02E0;
    private const nuint SubclassId = 0x53585343; // "SXSC"

    /// <summary>
    /// A point on the monitor Windows takes a window's DPI from once it covers <paramref name="bounds"/>: the monitor it
    /// overlaps most. Positioning the still small overlay there first gives Avalonia the same scale Windows ends up with.
    /// </summary>
    public static System.Drawing.Point GetDpiAnchor(System.Drawing.Rectangle bounds)
    {
        if (OperatingSystem.IsWindows())
        {
            Rect rect = new() { Left = bounds.Left, Top = bounds.Top, Right = bounds.Right, Bottom = bounds.Bottom };
            IntPtr monitor = MonitorFromRect(ref rect, MonitorDefaultToNearest);
            MonitorInfo info = new() { Size = Marshal.SizeOf<MonitorInfo>() };

            if (monitor != IntPtr.Zero && GetMonitorInfo(monitor, ref info))
            {
                return new System.Drawing.Point(info.Monitor.Left, info.Monitor.Top);
            }
        }

        return bounds.Location;
    }

    /// <summary>Drops WM_DPICHANGED for the window. Returns false where nothing was done.</summary>
    public static bool Apply(IntPtr windowHandle) =>
        OperatingSystem.IsWindows() && windowHandle != IntPtr.Zero &&
        SetWindowSubclass(windowHandle, &WindowProcedure, SubclassId, 0);

    /// <summary>
    /// Drops WM_DPICHANGED before Avalonia sees it, so the window keeps its scale and its size in device pixels. Windows does
    /// not resize a window that leaves WM_DPICHANGED unhandled.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static nint WindowProcedure(IntPtr hwnd, uint message, nuint wParam, nint lParam, nuint id, nuint data)
    {
        if (message == WM_DPICHANGED)
        {
            // Recorded for display diagnostics: Windows has already switched the window's DPI, only Avalonia misses it.
            WindowGeometryLog.NoteDpiChange(hwnd, (uint)(wParam & 0xFFFF), suppressed: true);
            return 0;
        }

        if (message == WM_NCDESTROY)
        {
            WindowGeometryLog.NoteWindowDestroyed(hwnd);
            RemoveWindowSubclass(hwnd, &WindowProcedure, id);
        }

        return DefSubclassProc(hwnd, message, wParam, lParam);
    }

    [LibraryImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowSubclass(IntPtr hwnd,
        delegate* unmanaged[Stdcall]<IntPtr, uint, nuint, nint, nuint, nuint, nint> procedure, nuint id, nuint data);

    [LibraryImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool RemoveWindowSubclass(IntPtr hwnd,
        delegate* unmanaged[Stdcall]<IntPtr, uint, nuint, nint, nuint, nuint, nint> procedure, nuint id);

    private const uint MonitorDefaultToNearest = 2;

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int Size;
        public Rect Monitor;
        public Rect WorkArea;
        public uint Flags;
    }

    [LibraryImport("user32.dll")]
    private static partial IntPtr MonitorFromRect(ref Rect rect, uint flags);

    [LibraryImport("user32.dll", EntryPoint = "GetMonitorInfoW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo info);

    [LibraryImport("comctl32.dll")]
    private static partial nint DefSubclassProc(IntPtr hwnd, uint message, nuint wParam, nint lParam);
}
