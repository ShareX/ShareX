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

using ShareX.Platform.Windows;
using System;
using System.Runtime.InteropServices;
using Xunit;

namespace ShareX.Platform.Tests.Windows;

/// <summary>
/// The region capture overlay spans monitors with different scaling. Windows sends it WM_DPICHANGED with a rectangle for the
/// other monitor's scale whenever it moves or resizes; a window that applies it (as Avalonia does) is resized, which switches the
/// DPI back, and the overlay flickers between scales. KeepScaleFixed must keep the overlay's size and position.
/// </summary>
[Collection("Windows platform services")]
public sealed class WindowsKeepScaleFixedTests
{
    private const uint WM_DPICHANGED = 0x02E0;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;

    [WindowsFact]
    public void KeepScaleFixed_IgnoresDpiChangeSuggestedRectangle()
    {
        IntPtr window = CreateWindowExW(0, "STATIC", "ShareX scale fixture", 0, 10, 20, 400, 300, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        Assert.NotEqual(IntPtr.Zero, window);

        try
        {
            // Stands in for Avalonia: moves and resizes the window to the rectangle Windows suggests for the new DPI.
            Assert.True(SetWindowSubclass(window, ApplySuggestedRectangleProcedure, 1, 0));

            Assert.True(new WindowsWindowService().KeepScaleFixed(window.ToInt64()));

            RECT suggested = new RECT { Left = 10, Top = 20, Right = 10 + 320, Bottom = 20 + 240 };
            SendMessageW(window, WM_DPICHANGED, (nuint)(120 | (120 << 16)), ref suggested);

            Assert.True(GetWindowRect(window, out RECT rect));
            Assert.Equal((10, 20, 400, 300), (rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top));
        }
        finally
        {
            DestroyWindow(window);
        }
    }

    private delegate nint SubclassProcedure(IntPtr hwnd, uint message, nuint wParam, nint lParam, nuint id, nuint data);

    // Kept in a static field so the delegate outlives the native subclass.
    private static readonly SubclassProcedure ApplySuggestedRectangleProcedure = ApplySuggestedRectangle;

    private static nint ApplySuggestedRectangle(IntPtr hwnd, uint message, nuint wParam, nint lParam, nuint id, nuint data)
    {
        if (message == WM_DPICHANGED)
        {
            RECT rect = Marshal.PtrToStructure<RECT>(lParam);
            SetWindowPos(hwnd, IntPtr.Zero, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top,
                SWP_NOZORDER | SWP_NOACTIVATE);
            return 0;
        }

        return DefSubclassProc(hwnd, message, wParam, lParam);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowExW(uint extendedStyle, string className, string title, uint style,
        int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr parameter);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(IntPtr window);

    [DllImport("user32.dll")]
    private static extern nint SendMessageW(IntPtr window, uint message, nuint wParam, ref RECT lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr window, out RECT rect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(IntPtr window, IntPtr insertAfter, int x, int y, int width, int height, uint flags);

    [DllImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowSubclass(IntPtr window, SubclassProcedure procedure, nuint id, nuint data);

    [DllImport("comctl32.dll")]
    private static extern nint DefSubclassProc(IntPtr window, uint message, nuint wParam, nint lParam);
}
