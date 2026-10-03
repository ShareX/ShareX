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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShareX.Platform.Windows.Native;

/// <summary>A hidden top-level window, owned and destroyed by its creating UI thread.</summary>
internal sealed class WindowsMessageWindow : IDisposable
{
    private delegate IntPtr WindowProcedure(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);
    private static readonly WindowProcedure Procedure = Dispatch;
    private static readonly ConcurrentDictionary<IntPtr, WindowsMessageWindow> Windows = new();
    private static readonly string ClassName = "ShareX.Platform.MessageWindow." + Guid.NewGuid().ToString("N");
    private static readonly Lazy<ushort> WindowClass = new(RegisterWindowClass);
    private const uint Popup = 0x80000000;
    private const uint ToolWindow = 0x00000080;
    private const uint NoActivate = 0x08000000;
    private readonly int threadId = Environment.CurrentManagedThreadId;
    private readonly Func<uint, IntPtr, IntPtr, IntPtr?> onMessage;
    private readonly Action<Exception> onUnhandledException;

    internal IntPtr Handle { get; private set; }

    internal WindowsMessageWindow(string title, Func<uint, IntPtr, IntPtr, IntPtr?> onMessage, Action<Exception> onUnhandledException)
    {
        this.onMessage = onMessage;
        this.onUnhandledException = onUnhandledException;
        _ = WindowClass.Value;
        Handle = CreateWindowEx(ToolWindow | NoActivate, ClassName, title, Popup,
            0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, GetModuleHandle(null), IntPtr.Zero);
        if (Handle == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
        Windows.TryAdd(Handle, this);
    }

    internal void VerifyAccess()
    {
        if (Environment.CurrentManagedThreadId != threadId)
        {
            throw new InvalidOperationException("The native message window must be accessed on its creating UI thread.");
        }
    }

    private static ushort RegisterWindowClass()
    {
        WindowClassInfo info = new()
        {
            Size = (uint)Marshal.SizeOf<WindowClassInfo>(),
            Procedure = Marshal.GetFunctionPointerForDelegate(Procedure),
            Instance = GetModuleHandle(null),
            ClassName = ClassName
        };
        ushort result = RegisterClassEx(ref info);
        if (result == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
        return result;
    }

    private static IntPtr Dispatch(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam)
    {
        if (Windows.TryGetValue(handle, out WindowsMessageWindow? window))
        {
            if (message == 0x0082) // WM_NCDESTROY, including destruction by DefWindowProc.
            {
                Windows.TryRemove(handle, out _);
                window.Handle = IntPtr.Zero;
            }

            try
            {
                if (window.onMessage(message, wParam, lParam) is IntPtr result) return result;
            }
            catch (Exception exception)
            {
                try { window.onUnhandledException(exception); }
                catch (Exception reportingException) { Trace.TraceError("Native window exception reporting failed: {0}", reportingException); }
                return IntPtr.Zero;
            }
        }

        return DefWindowProc(handle, message, wParam, lParam);
    }

    public void Dispose()
    {
        if (Handle == IntPtr.Zero) return;
        VerifyAccess();
        if (!DestroyWindow(Handle)) throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WindowClassInfo
    {
        public uint Size, Style;
        public IntPtr Procedure;
        public int ClassExtra, WindowExtra;
        public IntPtr Instance, Icon, Cursor, Background;
        public string? MenuName, ClassName;
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
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr GetModuleHandle(string? moduleName);
}
