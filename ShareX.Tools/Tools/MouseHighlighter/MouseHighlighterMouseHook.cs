#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShareX.Tools;

internal sealed class MouseHighlighterMouseHook : IDisposable
{
    private readonly HookProc _callback;
    private readonly MouseHighlighterInputBuffer _input;
    private readonly Thread _thread;
    private IntPtr _handle;
    private uint _threadId;
    private int _stopRequested;
    private int _disposed;
    private Exception? _failure;

    public Exception? Failure => Volatile.Read(ref _failure);

    public MouseHighlighterMouseHook(MouseHighlighterInputBuffer input)
    {
        _input = input;
        _callback = OnMouseInput;
        TaskCompletionSource initialized = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _thread = new Thread(() => Run(initialized))
        {
            IsBackground = true,
            Name = "ShareX mouse highlighter input",
            Priority = ThreadPriority.AboveNormal
        };
        _thread.Start();
        try { initialized.Task.GetAwaiter().GetResult(); }
        catch
        {
            _thread.Join();
            throw;
        }
    }

    private void Run(TaskCompletionSource initialized)
    {
        try
        {
            _threadId = GetCurrentThreadId();
            // Create the queue before reporting readiness, so Dispose can post
            // WM_QUIT even immediately after construction.
            PeekMessage(out _, IntPtr.Zero, 0, 0, 0);
            _handle = SetWindowsHookEx(NativeConstants.WH_MOUSE_LL, _callback, NativeMethods.GetModuleHandle(null), 0);
            if (_handle == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
            initialized.SetResult();

            while (Volatile.Read(ref _stopRequested) == 0)
            {
                int result = GetMessage(out NativeMessage message, IntPtr.Zero, 0, 0);
                if (result == -1) throw new Win32Exception(Marshal.GetLastWin32Error());
                if (result == 0) break;
                TranslateMessage(ref message);
                DispatchMessage(ref message);
            }
        }
        catch (Exception ex)
        {
            Volatile.Write(ref _failure, ex);
            initialized.TrySetException(ex);
        }
        finally
        {
            if (_handle != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_handle);
                _handle = IntPtr.Zero;
            }
            GC.KeepAlive(_callback);
        }
    }

    private IntPtr OnMouseInput(int code, IntPtr message, IntPtr data)
    {
        if (code >= 0)
        {
            // The POINT is the first field of MSLLHOOKSTRUCT. This callback
            // only publishes values; it never invokes or waits for the UI.
            System.Drawing.Point point = new(Marshal.ReadInt32(data), Marshal.ReadInt32(data, 4));
            _input.SetPosition(point);
            int mouseMessage = message.ToInt32();
            if (mouseMessage is 0x0201 or 0x0202 or 0x0204 or 0x0205)
            {
                bool swapped = NativeMethods.GetSystemMetrics(SystemMetric.SM_SWAPBUTTON) != 0;
                bool right = mouseMessage is 0x0204 or 0x0205;
                bool pressed = mouseMessage is 0x0201 or 0x0204;
                _input.PublishButton(new MouseHighlighterButtonEvent(right != swapped, pressed, point, Stopwatch.GetTimestamp()));
            }
        }

        return NativeMethods.CallNextHookEx(_handle, code, message, data);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        Volatile.Write(ref _stopRequested, 1);
        // The hook is removed by its owning thread. No UI dispatch is involved.
        PostThreadMessage(_threadId, 0x0012, IntPtr.Zero, IntPtr.Zero); // WM_QUIT
        if (Thread.CurrentThread != _thread)
        {
            _thread.Join();
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, HookProc callback, IntPtr module, uint threadId);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeMessage
    {
        public IntPtr Window;
        public uint Message;
        public IntPtr WParam;
        public IntPtr LParam;
        public uint Time;
        public POINT Position;
        public uint Private;
    }

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetMessage(out NativeMessage message, IntPtr window, uint min, uint max);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PeekMessage(out NativeMessage message, IntPtr window, uint min, uint max, uint remove);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PostThreadMessage(uint threadId, uint message, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TranslateMessage(ref NativeMessage message);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr DispatchMessage(ref NativeMessage message);
}