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
using System.Runtime.InteropServices;
using System.Threading;

namespace ShareX.Platform.Windows;

/// <summary>
/// A borderless, solid colour Win32 window placed directly behind another window. Transparent window capture photographs the
/// window over white and over black to recover its alpha channel. This does what a WinForms form did in ShareX before the
/// capture code moved here.
/// </summary>
internal sealed unsafe partial class Win32BackdropWindow : IDisposable
{
    private const string ClassName = "ShareXBackdropWindow";
    private const uint WS_POPUP = 0x80000000;
    private const uint WS_EX_TOOLWINDOW = 0x00000080;
    private const uint WS_EX_NOACTIVATE = 0x08000000;
    private const int SW_SHOWNOACTIVATE = 4;
    private const int GCLP_HBRBACKGROUND = -10;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint PM_REMOVE = 0x0001;
    private const uint RDW_INVALIDATE = 0x0001;
    private const uint RDW_ERASE = 0x0004;
    private const uint RDW_UPDATENOW = 0x0100;

    private static readonly object RegistrationLock = new object();
    private static bool registered;

    private readonly IntPtr handle;
    private IntPtr brush;

    public Win32BackdropWindow(PlatformRectangle bounds, uint rgb)
    {
        EnsureClassRegistered();
        brush = CreateSolidBrush(ToColorRef(rgb));
        handle = CreateWindowExW(WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE, ClassName, null, WS_POPUP,
            bounds.X, bounds.Y, bounds.Width, bounds.Height, IntPtr.Zero, IntPtr.Zero, GetModuleHandleW(null), IntPtr.Zero);

        if (handle == IntPtr.Zero)
        {
            DeleteObject(brush);
            throw new InvalidOperationException("CreateWindowEx failed: " + Marshal.GetLastPInvokeError());
        }

        SetClassLongPtrW(handle, GCLP_HBRBACKGROUND, brush);
        ShowWindow(handle, SW_SHOWNOACTIVATE);
    }

    /// <summary>Puts the backdrop directly under the target window in the Z order.</summary>
    public bool PlaceBehind(IntPtr window) => SetWindowPos(handle, window, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);

    public void SetColor(uint rgb)
    {
        IntPtr previous = brush;
        brush = CreateSolidBrush(ToColorRef(rgb));
        SetClassLongPtrW(handle, GCLP_HBRBACKGROUND, brush);
        DeleteObject(previous);
        RedrawWindow(handle, IntPtr.Zero, IntPtr.Zero, RDW_INVALIDATE | RDW_ERASE | RDW_UPDATENOW);
        PumpMessages();
    }

    /// <summary>Lets Windows paint the backdrop and the desktop compositor present it before the next capture.</summary>
    public void PumpMessages(int settleMilliseconds = 10)
    {
        while (PeekMessageW(out MSG message, IntPtr.Zero, 0, 0, PM_REMOVE))
        {
            TranslateMessage(ref message);
            DispatchMessageW(ref message);
        }

        Thread.Sleep(settleMilliseconds);
    }

    public void Dispose()
    {
        DestroyWindow(handle);
        DeleteObject(brush);
    }

    /// <summary>COLORREF is 0x00BBGGRR; callers pass 0xRRGGBB.</summary>
    private static uint ToColorRef(uint rgb) => ((rgb >> 16) & 0xFF) | (rgb & 0xFF00) | ((rgb & 0xFF) << 16);

    private static void EnsureClassRegistered()
    {
        lock (RegistrationLock)
        {
            if (registered)
            {
                return;
            }

            fixed (char* name = ClassName)
            {
                WNDCLASSEXW windowClass = new WNDCLASSEXW
                {
                    cbSize = (uint)sizeof(WNDCLASSEXW),
                    lpfnWndProc = &WindowProc,
                    hInstance = GetModuleHandleW(null),
                    lpszClassName = name
                };

                if (RegisterClassExW(&windowClass) == 0 && Marshal.GetLastPInvokeError() != 1410) // ERROR_CLASS_ALREADY_EXISTS
                {
                    throw new InvalidOperationException("RegisterClassEx failed: " + Marshal.GetLastPInvokeError());
                }
            }

            registered = true;
        }
    }

    [UnmanagedCallersOnly]
    private static nint WindowProc(IntPtr hwnd, uint message, nuint wParam, nint lParam) => DefWindowProcW(hwnd, message, wParam, lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct WNDCLASSEXW
    {
        public uint cbSize;
        public uint style;
        public delegate* unmanaged<IntPtr, uint, nuint, nint, nint> lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public char* lpszMenuName;
        public char* lpszClassName;
        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public nuint wParam;
        public nint lParam;
        public uint time;
        public int ptX;
        public int ptY;
        public uint lPrivate;
    }

    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial ushort RegisterClassExW(WNDCLASSEXW* windowClass);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr CreateWindowExW(uint exStyle, string className, string? windowName, uint style, int x, int y, int width, int height,
        IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);

    [LibraryImport("user32.dll")]
    private static partial nint DefWindowProcW(IntPtr hwnd, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DestroyWindow(IntPtr hwnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(IntPtr hwnd, int command);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowPos(IntPtr hwnd, IntPtr insertAfter, int x, int y, int width, int height, uint flags);

    [LibraryImport("user32.dll")]
    private static partial nint SetClassLongPtrW(IntPtr hwnd, int index, nint value);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool RedrawWindow(IntPtr hwnd, IntPtr updateRect, IntPtr updateRegion, uint flags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PeekMessageW(out MSG message, IntPtr hwnd, uint filterMin, uint filterMax, uint remove);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool TranslateMessage(ref MSG message);

    [LibraryImport("user32.dll")]
    private static partial nint DispatchMessageW(ref MSG message);

    [LibraryImport("gdi32.dll")]
    private static partial IntPtr CreateSolidBrush(uint color);

    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DeleteObject(IntPtr handle);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr GetModuleHandleW(string? moduleName);
}
