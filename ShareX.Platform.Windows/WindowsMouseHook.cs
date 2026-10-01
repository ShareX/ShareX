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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Windows;

/// <summary>
/// A WH_MOUSE_LL hook on its own thread and message loop. The callback only hands values to the listener; it never waits for the
/// UI, so a busy UI thread cannot make the system drop the hook.
/// </summary>
internal sealed unsafe partial class WindowsMouseHook : IDisposable
{
    private const int WH_MOUSE_LL = 14;
    private const int SM_SWAPBUTTON = 23;

    // One hook at a time is enough for ShareX; the static lets the unmanaged callback find its listener.
    private static WindowsMouseHook? current;

    private readonly IGlobalMouseListener listener;
    private readonly Thread thread;
    private IntPtr hook;
    private uint threadId;
    private int disposed;

    public WindowsMouseHook(IGlobalMouseListener listener)
    {
        this.listener = listener;

        if (Interlocked.CompareExchange(ref current, this, null) != null)
        {
            throw new InvalidOperationException("A mouse hook is already installed.");
        }

        TaskCompletionSource initialized = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        thread = new Thread(() => Run(initialized))
        {
            IsBackground = true,
            Name = "ShareX mouse hook",
            Priority = ThreadPriority.AboveNormal
        };
        thread.Start();

        try
        {
            initialized.Task.GetAwaiter().GetResult();
        }
        catch
        {
            thread.Join();
            Interlocked.CompareExchange(ref current, null, this);
            throw;
        }
    }

    private void Run(TaskCompletionSource initialized)
    {
        try
        {
            threadId = Win32.GetCurrentThreadId();
            // Create the message queue before reporting readiness, so Dispose can post WM_QUIT straight away.
            Win32.PeekMessage(out _, IntPtr.Zero, 0, 0, 0);
            hook = SetWindowsHookEx(WH_MOUSE_LL, &OnMouseInput, GetModuleHandle(IntPtr.Zero), 0);

            if (hook == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastPInvokeError());
            }

            initialized.SetResult();

            while (Win32.GetMessage(out Win32.MSG message, IntPtr.Zero, 0, 0) > 0)
            {
            }
        }
        catch (Exception e)
        {
            initialized.TrySetException(e);
        }
        finally
        {
            if (hook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hook);
                hook = IntPtr.Zero;
            }
        }
    }

    [UnmanagedCallersOnly]
    private static nint OnMouseInput(int code, nuint message, nint data)
    {
        WindowsMouseHook? self = current;

        if (code >= 0 && self != null)
        {
            try
            {
                // The POINT is the first field of MSLLHOOKSTRUCT.
                Win32.POINT* point = (Win32.POINT*)data;
                PlatformPoint position = new PlatformPoint(point->X, point->Y);
                self.listener.OnMove(position);

                uint mouseMessage = (uint)message;

                // WM_LBUTTONDOWN/UP, WM_RBUTTONDOWN/UP, WM_MBUTTONDOWN/UP
                if (mouseMessage is 0x0201 or 0x0202 or 0x0204 or 0x0205 or 0x0207 or 0x0208)
                {
                    bool swapped = Win32.GetSystemMetrics(SM_SWAPBUTTON) != 0;
                    bool right = mouseMessage is 0x0204 or 0x0205;
                    GlobalMouseButton button = mouseMessage is 0x0207 or 0x0208
                        ? GlobalMouseButton.Middle
                        : right != swapped ? GlobalMouseButton.Secondary : GlobalMouseButton.Primary;
                    bool pressed = mouseMessage is 0x0201 or 0x0204 or 0x0207;
                    self.listener.OnButton(new GlobalMouseButtonEvent(button, pressed, position, Stopwatch.GetTimestamp()));
                }
            }
            catch (Exception e)
            {
                // An exception must not cross the unmanaged boundary.
                Trace.WriteLine($"Mouse hook listener failed: {e}");
            }
        }

        return CallNextHookEx(IntPtr.Zero, code, message, data);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed, 1) != 0)
        {
            return;
        }

        // The hook is removed by its own thread.
        Win32.PostThreadMessage(threadId, Win32.WM_QUIT, 0, 0);

        if (Thread.CurrentThread != thread)
        {
            thread.Join();
        }

        Interlocked.CompareExchange(ref current, null, this);
    }

    [LibraryImport("user32.dll", EntryPoint = "SetWindowsHookExW", SetLastError = true)]
    private static partial IntPtr SetWindowsHookEx(int idHook, delegate* unmanaged<int, nuint, nint, nint> callback, IntPtr module, uint threadId);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UnhookWindowsHookEx(IntPtr hook);

    [LibraryImport("user32.dll")]
    private static partial nint CallNextHookEx(IntPtr hook, int code, nuint wParam, nint lParam);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW")]
    private static partial IntPtr GetModuleHandle(IntPtr moduleName);
}
