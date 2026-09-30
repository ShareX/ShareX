using ShareX.Platform.Windows.Native;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Windows;

/// <summary>RegisterHotKey on a dedicated message loop thread, so it works without a WinForms or Avalonia window.</summary>
public sealed class WindowsHotkeyService : IHotkeyService
{
    private readonly ConcurrentQueue<Action> commands = new ConcurrentQueue<Action>();
    private readonly Dictionary<int, PlatformHotkey> registrations = new Dictionary<int, PlatformHotkey>();
    private readonly Thread thread;
    private readonly TaskCompletionSource<uint> threadId = new TaskCompletionSource<uint>(TaskCreationOptions.RunContinuationsAsynchronously);
    private volatile bool running = true;

    public WindowsHotkeyService()
    {
        thread = new Thread(Run) { IsBackground = true, Name = "ShareX hotkeys" };
        thread.Start();
        threadId.Task.Wait();
    }

    public FeatureSupport Support => FeatureSupport.Supported;

    public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    public HotkeyRegistrationStatus Register(int id, PlatformHotkey hotkey)
    {
        if (!hotkey.IsValid)
        {
            return HotkeyRegistrationStatus.UnsupportedKey;
        }

        return Invoke(() =>
        {
            if (registrations.Remove(id))
            {
                Win32.UnregisterHotKey(IntPtr.Zero, id);
            }

            if (Win32.RegisterHotKey(IntPtr.Zero, id, (uint)hotkey.Modifiers | Win32.MOD_NOREPEAT, (uint)hotkey.KeyCode))
            {
                registrations[id] = hotkey;
                return HotkeyRegistrationStatus.Registered;
            }

            return System.Runtime.InteropServices.Marshal.GetLastPInvokeError() == Win32.ERROR_HOTKEY_ALREADY_REGISTERED
                ? HotkeyRegistrationStatus.InUse
                : HotkeyRegistrationStatus.Failed;
        }, HotkeyRegistrationStatus.Failed);
    }

    public bool Unregister(int id) => Invoke(() => registrations.Remove(id) && Win32.UnregisterHotKey(IntPtr.Zero, id), false);

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

        TaskCompletionSource<T> completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        commands.Enqueue(() =>
        {
            try
            {
                completion.SetResult(action());
            }
            catch (Exception e)
            {
                completion.SetException(e);
            }
        });

        Win32.PostThreadMessage(threadId.Task.Result, Win32.WM_APP, 0, 0);
        return completion.Task.Wait(TimeSpan.FromSeconds(5)) ? completion.Task.Result : fallback;
    }

    private void Run()
    {
        // Create the message queue before anyone posts to it.
        Win32.PeekMessage(out _, IntPtr.Zero, 0, 0, 0);
        threadId.SetResult(Win32.GetCurrentThreadId());

        while (running && Win32.GetMessage(out Win32.MSG message, IntPtr.Zero, 0, 0) > 0)
        {
            if (message.message == Win32.WM_HOTKEY)
            {
                int id = (int)message.wParam;

                if (registrations.TryGetValue(id, out PlatformHotkey hotkey))
                {
                    HotkeyPressed?.Invoke(this, new HotkeyPressedEventArgs(id, hotkey));
                }
            }
            else if (message.message == Win32.WM_APP)
            {
                while (commands.TryDequeue(out Action? command))
                {
                    command();
                }
            }
        }

        foreach (int id in registrations.Keys)
        {
            Win32.UnregisterHotKey(IntPtr.Zero, id);
        }
    }

    public void Dispose()
    {
        if (!running)
        {
            return;
        }

        running = false;
        Win32.PostThreadMessage(threadId.Task.Result, Win32.WM_QUIT, 0, 0);
        thread.Join(TimeSpan.FromSeconds(2));
    }
}
