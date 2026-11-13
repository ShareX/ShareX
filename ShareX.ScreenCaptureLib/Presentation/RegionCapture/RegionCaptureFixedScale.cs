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
/// Keeps a region capture overlay at its size in device pixels while it is open. On Windows an overlay that spans monitors
/// with different scaling is switched between their DPIs when it is moved or resized, and WM_DPICHANGED suggests a rectangle
/// scaled for the new DPI. Applying that rectangle resized the overlay, which switched the DPI back: it flickered between
/// scales and pointer positions mapped to the wrong screenshot pixels. Dropping the message instead left Windows and Avalonia
/// on different scales, which misplaced the toolbar and magnifier. The message now reaches Avalonia with the window's current
/// rectangle, so the scale follows Windows while the size and position stay put.
/// </summary>
internal static unsafe partial class RegionCaptureFixedScale
{
    private const uint WM_NCDESTROY = 0x0082;
    private const uint WM_DPICHANGED = 0x02E0;
    private const nuint SubclassId = 0x53585343; // "SXSC"

    /// <summary>Pins the window's rectangle across DPI changes. Returns false where nothing was done.</summary>
    public static bool Apply(IntPtr windowHandle) =>
        OperatingSystem.IsWindows() && windowHandle != IntPtr.Zero &&
        SetWindowSubclass(windowHandle, &WindowProcedure, SubclassId, 0);

    /// <summary>
    /// Replaces the rectangle WM_DPICHANGED suggests with the window's current one before Avalonia handles the message, so
    /// Avalonia takes the new scale without moving or resizing the window.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static nint WindowProcedure(IntPtr hwnd, uint message, nuint wParam, nint lParam, nuint id, nuint data)
    {
        if (message == WM_DPICHANGED)
        {
            WindowGeometryLog.NoteDpiChange(hwnd, (uint)(wParam & 0xFFFF), suppressed: false);

            if (lParam != 0 && GetWindowRect(hwnd, out Rect current))
            {
                *(Rect*)lParam = current;
            }
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

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetWindowRect(IntPtr hwnd, out Rect rect);

    [LibraryImport("comctl32.dll")]
    private static partial nint DefSubclassProc(IntPtr hwnd, uint message, nuint wParam, nint lParam);
}
