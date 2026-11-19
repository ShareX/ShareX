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

using ShareX.HelpersLib;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace ShareX.Tools;

/// <summary>A low-level keyboard hook on its own thread. It only reports key presses; it never blocks or changes them.</summary>
internal sealed class KeyHighlighterKeyboardHook : IDisposable
{
    private delegate IntPtr KeyboardProc(int code, IntPtr message, IntPtr data);

    private readonly KeyboardProc _callback;
    private readonly Action<int, bool, IntPtr> _onKey;
    private readonly Thread _thread;
    private IntPtr _handle;
    private uint _threadId;
    private int _stopRequested;
    private int _disposed;

    /// <param name="onKey">Called with the virtual key, whether it is a key press, and the KBDLLHOOKSTRUCT pointer.</param>
    public KeyHighlighterKeyboardHook(Action<int, bool, IntPtr> onKey)
    {
        _onKey = onKey;
        _callback = OnKeyboardInput;
        TaskCompletionSource initialized = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _thread = new Thread(() => Run(initialized)) { IsBackground = true, Name = "ShareX key highlighter input" };
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
            PeekMessage(out _, IntPtr.Zero, 0, 0, 0);
            _handle = SetWindowsHookEx(NativeConstants.WH_KEYBOARD_LL, _callback, NativeMethods.GetModuleHandle(null), 0);
            if (_handle == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
            initialized.SetResult();

            while (Volatile.Read(ref _stopRequested) == 0)
            {
                int result = GetMessage(out NativeMessage message, IntPtr.Zero, 0, 0);
                if (result <= 0) break;
            }
        }
        catch (Exception ex)
        {
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

    private IntPtr OnKeyboardInput(int code, IntPtr message, IntPtr data)
    {
        if (code >= 0)
        {
            int keyMessage = message.ToInt32();
            int virtualKey = Marshal.ReadInt32(data);
            try { _onKey(virtualKey, keyMessage is 0x0100 or 0x0104, data); }
            catch { }
        }
        return NativeMethods.CallNextHookEx(_handle, code, message, data);
    }

    /// <summary>The character a text key types with the foreground window's layout and the current Shift state.</summary>
    public static string? GetTypedText(int virtualKey, bool shift, bool capsLock)
    {
        IntPtr foreground = NativeMethods.GetForegroundWindow();
        uint threadId = GetWindowThreadProcessId(foreground, IntPtr.Zero);
        IntPtr layout = GetKeyboardLayout(threadId);
        if (virtualKey == 0x20) return " ";
        uint value = MapVirtualKeyEx((uint)virtualKey, 2, layout);
        if (value == 0 || (value & 0x80000000) != 0) return null;
        string text = ((char)(value & 0xFFFF)).ToString();
        // MAPVK_VK_TO_CHAR gives the unshifted character; letters follow Shift and Caps Lock.
        if (text.Length == 1 && char.IsLetter(text[0])) return shift != capsLock ? text.ToUpperInvariant() : text.ToLowerInvariant();
        return shift ? null : text;
    }

    /// <summary>True when the focused control is a Windows edit box that hides its text.</summary>
    public static bool IsPasswordFieldFocused()
    {
        IntPtr foreground = NativeMethods.GetForegroundWindow();
        uint threadId = GetWindowThreadProcessId(foreground, IntPtr.Zero);
        GuiThreadInfo info = new() { Size = Marshal.SizeOf<GuiThreadInfo>() };
        if (!GetGUIThreadInfo(threadId, ref info) || info.Focus == IntPtr.Zero) return false;
        StringBuilder className = new(64);
        GetClassName(info.Focus, className, className.Capacity);
        const int ES_PASSWORD = 0x20;
        return className.ToString().Contains("Edit", StringComparison.OrdinalIgnoreCase) &&
            (GetWindowLong(info.Focus, -16) & ES_PASSWORD) != 0;
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        Volatile.Write(ref _stopRequested, 1);
        PostThreadMessage(_threadId, 0x0012, IntPtr.Zero, IntPtr.Zero); // WM_QUIT
        if (Thread.CurrentThread != _thread) _thread.Join();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeMessage
    {
        public IntPtr Window;
        public uint Message;
        public IntPtr WParam;
        public IntPtr LParam;
        public uint Time;
        public int X, Y;
        public uint Private;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct GuiThreadInfo
    {
        public int Size;
        public int Flags;
        public IntPtr Active;
        public IntPtr Focus;
        public IntPtr Capture;
        public IntPtr MenuOwner;
        public IntPtr MoveSize;
        public IntPtr Caret;
        public int CaretLeft, CaretTop, CaretRight, CaretBottom;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardProc callback, IntPtr module, uint threadId);

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
    private static extern uint GetWindowThreadProcessId(IntPtr window, IntPtr processId);

    [DllImport("user32.dll")]
    private static extern IntPtr GetKeyboardLayout(uint threadId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern uint MapVirtualKeyEx(uint code, uint mapType, IntPtr layout);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetGUIThreadInfo(uint threadId, ref GuiThreadInfo info);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(IntPtr window, StringBuilder className, int maxCount);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr window, int index);
}
