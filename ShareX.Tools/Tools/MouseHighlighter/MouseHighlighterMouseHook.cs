#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ShareX.Tools;

internal sealed class MouseHighlighterMouseHook : IDisposable
{
    private readonly HookProc _callback;
    private readonly Action<int, System.Drawing.Point> _input;
    private IntPtr _handle;

    public MouseHighlighterMouseHook(Action<int, System.Drawing.Point> input)
    {
        _input = input;
        _callback = OnMouseInput;
        _handle = SetWindowsHookEx(NativeConstants.WH_MOUSE_LL, _callback, NativeMethods.GetModuleHandle(null), 0);
        if (_handle == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    private IntPtr OnMouseInput(int code, IntPtr message, IntPtr data)
    {
        if (code >= 0)
        {
            // The POINT is the first field of MSLLHOOKSTRUCT. Keep this callback
            // short; rendering happens on the animation timer, outside the hook.
            System.Drawing.Point point = new(Marshal.ReadInt32(data), Marshal.ReadInt32(data, 4));
            _input(message.ToInt32(), point);
        }

        return NativeMethods.CallNextHookEx(_handle, code, message, data);
    }

    public void Dispose()
    {
        if (_handle != IntPtr.Zero)
        {
            NativeMethods.UnhookWindowsHookEx(_handle);
            _handle = IntPtr.Zero;
        }
        GC.KeepAlive(_callback);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, HookProc callback, IntPtr module, uint threadId);
}
