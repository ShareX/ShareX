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

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

/// <summary>
/// Keeps a region capture overlay at the scale it was created with while it is open. On Windows an overlay that spans
/// monitors with different scaling is otherwise switched between their DPIs whenever it is moved or resized. The overlay is
/// sized in device pixels, so following each switch only resizes it again: it flickers between scales and pointer positions
/// map to the wrong screenshot pixels (selecting the empty area of the ShareX main window then captures its sidebar).
/// </summary>
internal static unsafe partial class RegionCaptureFixedScale
{
    private const uint WM_NCDESTROY = 0x0082;
    private const uint WM_DPICHANGED = 0x02E0;
    private const nuint SubclassId = 0x53585343; // "SXSC"

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
            return 0;
        }

        if (message == WM_NCDESTROY)
        {
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

    [LibraryImport("comctl32.dll")]
    private static partial nint DefSubclassProc(IntPtr hwnd, uint message, nuint wParam, nint lParam);
}
