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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib;

/// <summary>Hidden Windows message transport for the application host.</summary>
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
}
