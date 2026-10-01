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

using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib;

/// <summary>Windows transport for native overlays; rendering is supplied by Skia.</summary>
public sealed class WindowsNativeWindow : IDisposable
{
    private delegate IntPtr WindowProcedure(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);
    private static readonly WindowProcedure Procedure = Dispatch;
    private static readonly ConcurrentDictionary<IntPtr, WindowsNativeWindow> Windows = new();
    private static readonly Lazy<ushort> WindowClass = new(RegisterWindowClass);
    private const string ClassName = "ShareX.NativeOverlay";
    public IntPtr Handle { get; private set; }
    public event EventHandler<NativeWindowMessageEventArgs> MessageReceived;

    public WindowsNativeWindow(string title, Rectangle bounds, WindowStyles extendedStyle)
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("Native overlays require Windows.");
        _ = WindowClass.Value;
        Handle = CreateWindowEx((uint)extendedStyle, ClassName, title, (uint)WindowStyles.WS_POPUP,
            bounds.X, bounds.Y, bounds.Width, bounds.Height, IntPtr.Zero, IntPtr.Zero,
            NativeMethods.GetModuleHandle(null), IntPtr.Zero);
        if (Handle == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
        Windows.TryAdd(Handle, this);
    }

    private static ushort RegisterWindowClass()
    {
        WindowClassInfo info = new()
        {
            Size = (uint)Marshal.SizeOf<WindowClassInfo>(),
            Procedure = Marshal.GetFunctionPointerForDelegate(Procedure),
            Instance = NativeMethods.GetModuleHandle(null),
            ClassName = ClassName
        };
        ushort result = RegisterClassEx(ref info);
        if (result == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
        return result;
    }

    private static IntPtr Dispatch(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam)
    {
        if (Windows.TryGetValue(handle, out WindowsNativeWindow window))
        {
            NativeWindowMessageEventArgs args = new((int)message, wParam, lParam);
            try { window.MessageReceived?.Invoke(window, args); }
            catch (Exception exception) { DebugHelper.WriteException(exception); }
            if (args.Handled) return args.Result;
        }
        return DefWindowProc(handle, message, wParam, lParam);
    }

    public void SetBackground(Color color, Rectangle bounds)
    {
        using SKBitmap bitmap = SkiaImageHelpers.CreateBitmap(bounds.Width, bounds.Height);
        bitmap.Erase(color.ToSKColor());
        IntPtr dc = NativeMethods.CreateCompatibleDC(IntPtr.Zero);
        IntPtr dib = IntPtr.Zero, previous = IntPtr.Zero;
        try
        {
            BITMAPINFOHEADER info = new(bounds.Width, bounds.Height, 32) { biHeight = -bounds.Height };
            dib = NativeMethods.CreateDIBSection(dc, ref info, 0, out IntPtr pixels, IntPtr.Zero, 0);
            if (dc == IntPtr.Zero || dib == IntPtr.Zero || pixels == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            previous = NativeMethods.SelectObject(dc, dib);
            using SKPixmap source = bitmap.PeekPixels();
            if (!source.ReadPixels(bitmap.Info, pixels, bounds.Width * 4))
                throw new InvalidOperationException("Unable to transfer the capture background.");
            POINT destination = new(bounds.X, bounds.Y), origin = new(0, 0);
            SIZE size = new(bounds.Width, bounds.Height);
            BLENDFUNCTION blend = new() { SourceConstantAlpha = 255, AlphaFormat = NativeConstants.AC_SRC_ALPHA };
            if (!UpdateLayeredWindow(Handle, IntPtr.Zero, ref destination, ref size, dc, ref origin, 0, ref blend, NativeConstants.ULW_ALPHA))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        finally
        {
            if (previous != IntPtr.Zero) NativeMethods.SelectObject(dc, previous);
            if (dib != IntPtr.Zero) NativeMethods.DeleteObject(dib);
            if (dc != IntPtr.Zero) NativeMethods.DeleteDC(dc);
        }
    }

    public void Dispose()
    {
        IntPtr handle = Handle;
        if (handle == IntPtr.Zero) return;
        if (!DestroyWindow(handle)) throw new Win32Exception(Marshal.GetLastWin32Error());
        Windows.TryRemove(handle, out _);
        Handle = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WindowClassInfo
    {
        public uint Size, Style;
        public IntPtr Procedure;
        public int ClassExtra, WindowExtra;
        public IntPtr Instance, Icon, Cursor, Background;
        public string MenuName, ClassName;
        public IntPtr SmallIcon;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern ushort RegisterClassEx(ref WindowClassInfo info);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowEx(uint extendedStyle, string className, string title, uint style,
        int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr parameter);
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(IntPtr window);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr DefWindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UpdateLayeredWindow(IntPtr window, IntPtr destinationDc, ref POINT destination,
        ref SIZE size, IntPtr sourceDc, ref POINT source, uint colorKey, ref BLENDFUNCTION blend, uint flags);
}
