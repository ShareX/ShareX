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
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ShareX.Platform.Windows;

/// <summary>
/// A layered popup window: per pixel alpha through UpdateLayeredWindow, click through (WS_EX_TRANSPARENT and HTTRANSPARENT), never
/// activated, hidden from the task bar. Its messages are pumped by whatever loop runs on the creating thread (Avalonia's on the UI
/// thread).
/// </summary>
internal sealed unsafe partial class WindowsScreenOverlay : IScreenOverlay
{
    private const string ClassName = "ShareXScreenOverlay";
    private const uint WS_POPUP = 0x80000000;
    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint WS_EX_TRANSPARENT = 0x00000020;
    private const uint WS_EX_TOOLWINDOW = 0x00000080;
    private const uint WS_EX_NOACTIVATE = 0x08000000;
    private const uint WS_EX_TOPMOST = 0x00000008;
    private const uint ULW_ALPHA = 0x00000002;
    private const byte AC_SRC_ALPHA = 0x01;
    private const uint SWP_SHOWWINDOW = 0x0040;
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    private static readonly object RegistrationLock = new object();
    private static bool registered;

    private readonly IntPtr handle;
    private IntPtr dc;
    private IntPtr bitmap;
    private IntPtr previousBitmap;
    private IntPtr pixels;
    private int bufferWidth;
    private int bufferHeight;
    private bool visible;

    public WindowsScreenOverlay(PlatformRectangle screenBounds)
    {
        EnsureClassRegistered();
        handle = CreateWindowExW(WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE | WS_EX_TOPMOST, ClassName,
            "ShareX overlay", WS_POPUP, screenBounds.X, screenBounds.Y, 1, 1, IntPtr.Zero, IntPtr.Zero, GetModuleHandleW(null), IntPtr.Zero);

        if (handle == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError());
        }
    }

    public OverlayBuffer GetBuffer(int width, int height)
    {
        // Keep the buffer while it is large enough and not far too large, so a growing ripple does not allocate every frame.
        if (bitmap == IntPtr.Zero || width > bufferWidth || height > bufferHeight || width < bufferWidth / 4 || height < bufferHeight / 4)
        {
            ReleaseBuffer();
            bufferWidth = (width + 63) / 64 * 64;
            bufferHeight = (height + 63) / 64 * 64;
            dc = Win32.CreateCompatibleDC(IntPtr.Zero);
            Win32.BITMAPINFOHEADER header = new Win32.BITMAPINFOHEADER
            {
                biSize = (uint)sizeof(Win32.BITMAPINFOHEADER),
                biWidth = bufferWidth,
                biHeight = -bufferHeight,
                biPlanes = 1,
                biBitCount = 32,
                biCompression = Win32.BI_RGB
            };

            bitmap = Win32.CreateDIBSection(dc, &header, Win32.DIB_RGB_COLORS, out pixels, IntPtr.Zero, 0);

            if (dc == IntPtr.Zero || bitmap == IntPtr.Zero || pixels == IntPtr.Zero)
            {
                ReleaseBuffer();
                throw new InvalidOperationException("Could not allocate the overlay bitmap.");
            }

            previousBitmap = Win32.SelectObject(dc, bitmap);
        }

        return new OverlayBuffer(pixels, bufferWidth * 4, bufferHeight);
    }

    public void Present(PlatformRectangle area)
    {
        Win32.GdiFlush();
        Win32.POINT destination = new Win32.POINT { X = area.X, Y = area.Y };
        Win32.POINT source = default;
        SIZE size = new SIZE { cx = area.Width, cy = area.Height };
        BLENDFUNCTION blend = new BLENDFUNCTION { SourceConstantAlpha = 255, AlphaFormat = AC_SRC_ALPHA };

        if (!UpdateLayeredWindow(handle, IntPtr.Zero, &destination, &size, dc, &source, 0, &blend, ULW_ALPHA))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError());
        }

        if (!visible)
        {
            Win32.SetWindowPos(handle, HWND_TOPMOST, area.X, area.Y, area.Width, area.Height, Win32.SWP_NOACTIVATE | SWP_SHOWWINDOW);
            visible = true;
        }
    }

    public void Hide()
    {
        if (visible)
        {
            Win32.ShowWindow(handle, Win32.SW_HIDE);
            visible = false;
        }
    }

    public void Dispose()
    {
        ReleaseBuffer();
        Win32.DestroyWindow(handle);
    }

    private void ReleaseBuffer()
    {
        if (previousBitmap != IntPtr.Zero) Win32.SelectObject(dc, previousBitmap);
        if (bitmap != IntPtr.Zero) Win32.DeleteObject(bitmap);
        if (dc != IntPtr.Zero) Win32.DeleteDC(dc);
        previousBitmap = bitmap = dc = pixels = IntPtr.Zero;
    }

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
                    throw new Win32Exception(Marshal.GetLastPInvokeError());
                }
            }

            registered = true;
        }
    }

    [UnmanagedCallersOnly]
    private static nint WindowProc(IntPtr hwnd, uint message, nuint wParam, nint lParam) => message switch
    {
        0x0084 => -1, // WM_NCHITTEST: HTTRANSPARENT
        0x0021 => 3, // WM_MOUSEACTIVATE: MA_NOACTIVATE
        _ => DefWindowProcW(hwnd, message, wParam, lParam)
    };

    [StructLayout(LayoutKind.Sequential)]
    private struct SIZE
    {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

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

    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial ushort RegisterClassExW(WNDCLASSEXW* windowClass);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr CreateWindowExW(uint exStyle, string className, string? windowName, uint style, int x, int y, int width, int height,
        IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);

    [LibraryImport("user32.dll")]
    private static partial nint DefWindowProcW(IntPtr hwnd, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UpdateLayeredWindow(IntPtr hwnd, IntPtr destinationDc, Win32.POINT* destination, SIZE* size, IntPtr sourceDc,
        Win32.POINT* source, uint colorKey, BLENDFUNCTION* blend, uint flags);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr GetModuleHandleW(string? moduleName);
}
