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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Windows;

/// <summary>RegisterHotKey on a dedicated message loop thread, so it works without a WinForms or Avalonia window.</summary>
public sealed class WindowsHotkeyService : IHotkeyService
{
    private readonly object syncLock = new object();
    private readonly Queue<Command> commands = new Queue<Command>();
    private readonly Dictionary<int, PlatformHotkey> registrations = new Dictionary<int, PlatformHotkey>();
    private readonly Thread thread;
    private readonly TaskCompletionSource<uint> threadId = new TaskCompletionSource<uint>(TaskCreationOptions.RunContinuationsAsynchronously);
    private volatile bool running = true;

    public WindowsHotkeyService()
    {
        thread = new Thread(Run) { IsBackground = true, Name = "ShareX hotkeys" };
        thread.Start();
        threadId.Task.GetAwaiter().GetResult();
    }

    public FeatureSupport Support => running ? FeatureSupport.Supported : FeatureSupport.NotSupported("Global hotkeys are no longer available in this session.");

    public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    public HotkeyRegistrationStatus Register(int id, PlatformHotkey hotkey)
    {
        if (!hotkey.IsValid || hotkey.KeyCode > 0xFF || hotkey.KeyCode is 0x10 or 0x11 or 0x12 or 0x5B or 0x5C or >= 0xA0 and <= 0xA5 ||
            (hotkey.Modifiers & ~(HotkeyModifiers.Alt | HotkeyModifiers.Control | HotkeyModifiers.Shift | HotkeyModifiers.Super)) != 0)
        {
            return HotkeyRegistrationStatus.UnsupportedKey;
        }

        return Invoke(() =>
        {
            if (registrations.ContainsKey(id))
            {
                if (!Win32.UnregisterHotKey(IntPtr.Zero, id))
                {
                    return HotkeyRegistrationStatus.Failed;
                }

                registrations.Remove(id);
            }

            // ShareX's configurable repeat limit belongs to the shared caller. Keep native repeat events,
            // including when the user sets that limit to zero, just as WindowsHotkeyHost did in v22.
            if (Win32.RegisterHotKey(IntPtr.Zero, id, (uint)hotkey.Modifiers, (uint)hotkey.KeyCode))
            {
                registrations[id] = hotkey;
                return HotkeyRegistrationStatus.Registered;
            }

            return Marshal.GetLastPInvokeError() == Win32.ERROR_HOTKEY_ALREADY_REGISTERED
                ? HotkeyRegistrationStatus.InUse
                : HotkeyRegistrationStatus.Failed;
        }, HotkeyRegistrationStatus.Failed);
    }

    public bool Unregister(int id) => Invoke(() =>
    {
        if (registrations.ContainsKey(id) && Win32.UnregisterHotKey(IntPtr.Zero, id))
        {
            registrations.Remove(id);
            return true;
        }

        return false;
    }, false);

    public void UnregisterAll()
    {
        Invoke(() =>
        {
            foreach (int id in registrations.Keys)
            {
                Win32.UnregisterHotKey(IntPtr.Zero, id);
            }

            registrations.Clear();
            return true;
        }, false);
    }

    /// <summary>Hotkeys belong to the thread that registers them, so every call runs on the message loop thread.</summary>
    private T Invoke<T>(Func<T> action, T fallback)
    {
        if (!running)
        {
            return fallback;
        }

        if (Thread.CurrentThread == thread)
        {
            return action();
        }

        Command<T> command = new Command<T>(action, fallback);

        lock (syncLock)
        {
            if (!running)
            {
                return fallback;
            }

            commands.Enqueue(command);

            if (!Win32.PostThreadMessage(threadId.Task.Result, Win32.WM_APP, 0, 0))
            {
                command.Cancel();
            }
        }

        if (!command.Completion.Task.Wait(TimeSpan.FromSeconds(5)))
        {
            // Cancel only work that has not started. Never report failure and register the key later.
            command.Cancel();
        }

        return command.Completion.Task.GetAwaiter().GetResult();
    }

    private void Run()
    {
        try
        {
            // Create the message queue before anyone posts to it.
            Win32.PeekMessage(out _, IntPtr.Zero, 0, 0, 0);
            threadId.TrySetResult(Win32.GetCurrentThreadId());

            while (running && Win32.GetMessage(out Win32.MSG message, IntPtr.Zero, 0, 0) > 0)
            {
                if (message.message == Win32.WM_HOTKEY)
                {
                    int id = (int)message.wParam;
                    uint data = unchecked((uint)message.lParam);

                    if (registrations.TryGetValue(id, out PlatformHotkey hotkey) && hotkey.KeyCode == (int)(data >> 16) &&
                        (uint)hotkey.Modifiers == (data & 0xFFFF))
                    {
                        RaiseHotkeyPressed(new HotkeyPressedEventArgs(id, hotkey));
                    }
                }
                else if (message.message == Win32.WM_APP)
                {
                    while (running)
                    {
                        Command command;

                        lock (syncLock)
                        {
                            if (!running || commands.Count == 0) break;
                            command = commands.Dequeue();
                        }

                        command.Execute();
                    }
                }
            }
        }
        catch (Exception exception)
        {
            threadId.TrySetException(exception);
            Trace.TraceError("Windows hotkey message loop failed: {0}", exception);
        }
        finally
        {
            StopAcceptingCommands();

            foreach (int id in registrations.Keys)
            {
                Win32.UnregisterHotKey(IntPtr.Zero, id);
            }

            registrations.Clear();
        }
    }

    private void RaiseHotkeyPressed(HotkeyPressedEventArgs args)
    {
        if (HotkeyPressed is not EventHandler<HotkeyPressedEventArgs> handlers) return;

        foreach (EventHandler<HotkeyPressedEventArgs> handler in handlers.GetInvocationList())
        {
            try
            {
                handler(this, args);
            }
            catch (Exception exception)
            {
                // A subscriber must not kill the native thread and leave its callers waiting.
                Trace.TraceError("Windows hotkey handler failed: {0}", exception);
            }
        }
    }

    private void StopAcceptingCommands()
    {
        lock (syncLock)
        {
            running = false;

            while (commands.TryDequeue(out Command? command))
            {
                command.Cancel();
            }
        }
    }

    public void Dispose()
    {
        StopAcceptingCommands();
        if (threadId.Task.IsCompletedSuccessfully)
        {
            Win32.PostThreadMessage(threadId.Task.Result, Win32.WM_QUIT, 0, 0);
        }

        if (Thread.CurrentThread != thread)
        {
            thread.Join(TimeSpan.FromSeconds(2));
        }
    }

    private abstract class Command
    {
        public abstract void Execute();
        public abstract void Cancel();
    }

    private sealed class Command<T>(Func<T> action, T fallback) : Command
    {
        private int started;
        public TaskCompletionSource<T> Completion { get; } = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

        public override void Execute()
        {
            if (Interlocked.CompareExchange(ref started, 1, 0) != 0) return;

            try
            {
                Completion.TrySetResult(action());
            }
            catch (Exception exception)
            {
                Completion.TrySetException(exception);
            }
        }

        public override void Cancel()
        {
            if (Interlocked.CompareExchange(ref started, 1, 0) == 0)
            {
                Completion.TrySetResult(fallback);
            }
        }
    }
}
