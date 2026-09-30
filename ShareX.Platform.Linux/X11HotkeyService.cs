using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux;

/// <summary>Global hotkeys on X11 with XGrabKey on the root window.</summary>
/// <remarks>A dedicated thread owns its own X connection, so registration calls are marshalled to it.</remarks>
public sealed partial class X11HotkeyService : IHotkeyService
{
    private static readonly uint[] LockCombinations = [0, X11.LockMask, X11.Mod2Mask, X11.LockMask | X11.Mod2Mask];
    private const uint RelevantModifiers = X11.ShiftMask | X11.ControlMask | X11.Mod1Mask | X11.Mod4Mask;

    private readonly BlockingCollection<Action<X11Display>> commands = new BlockingCollection<Action<X11Display>>();
    private readonly Dictionary<int, Registration> registrations = new Dictionary<int, Registration>();
    private readonly Thread thread;
    private readonly TaskCompletionSource<bool> started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    private volatile bool running = true;
    private X11Display? threadDisplay;

    private readonly record struct Registration(PlatformHotkey Hotkey, int KeyCode, uint Modifiers);

    public X11HotkeyService()
    {
        thread = new Thread(Run) { IsBackground = true, Name = "ShareX X11 hotkeys" };
        thread.Start();
        Support = started.Task.Wait(TimeSpan.FromSeconds(5)) && started.Task.Result
            ? FeatureSupport.Supported
            : FeatureSupport.NotSupported("Cannot connect to the X11 display.");
    }

    public FeatureSupport Support { get; }

    public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    public HotkeyRegistrationStatus Register(int id, PlatformHotkey hotkey)
    {
        if (!Support.IsSupported)
        {
            return HotkeyRegistrationStatus.NotSupported;
        }

        if (!XKeyMap.TryGetKeysym(hotkey.KeyCode, out uint keysym, out _))
        {
            return HotkeyRegistrationStatus.UnsupportedKey;
        }

        return Invoke(display =>
        {
            Unregister(display, id);

            int keyCode = X11.XKeysymToKeycode(display.Display, keysym);

            if (keyCode == 0)
            {
                return HotkeyRegistrationStatus.UnsupportedKey;
            }

            uint modifiers = XKeyMap.ToX11Modifiers(hotkey.Modifiers);
            X11.TakeLastError();

            foreach (uint lockMask in LockCombinations)
            {
                X11.XGrabKey(display.Display, keyCode, modifiers | lockMask, display.Root, false, X11.GrabModeAsync, X11.GrabModeAsync);
            }

            X11.XSync(display.Display, false);
            int error = X11.TakeLastError();

            if (error != X11.Success)
            {
                foreach (uint lockMask in LockCombinations)
                {
                    X11.XUngrabKey(display.Display, keyCode, modifiers | lockMask, display.Root);
                }

                X11.XSync(display.Display, false);
                X11.TakeLastError();
                return error == X11.BadAccess ? HotkeyRegistrationStatus.InUse : HotkeyRegistrationStatus.Failed;
            }

            registrations[id] = new Registration(hotkey, keyCode, modifiers);
            return HotkeyRegistrationStatus.Registered;
        }, HotkeyRegistrationStatus.Failed);
    }

    public bool Unregister(int id) => Invoke(display => Unregister(display, id), false);

    public void UnregisterAll()
    {
        Invoke(display =>
        {
            foreach (int id in new List<int>(registrations.Keys))
            {
                Unregister(display, id);
            }

            return true;
        }, false);
    }

    private bool Unregister(X11Display display, int id)
    {
        if (!registrations.Remove(id, out Registration registration))
        {
            return false;
        }

        foreach (uint lockMask in LockCombinations)
        {
            X11.XUngrabKey(display.Display, registration.KeyCode, registration.Modifiers | lockMask, display.Root);
        }

        X11.XSync(display.Display, false);
        X11.TakeLastError();
        return true;
    }

    private T Invoke<T>(Func<X11Display, T> action, T fallback)
    {
        if (!running)
        {
            return fallback;
        }

        // Called from a HotkeyPressed handler, which already runs on the X thread.
        if (Thread.CurrentThread == thread && threadDisplay != null)
        {
            return action(threadDisplay);
        }

        TaskCompletionSource<T> completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        commands.Add(display =>
        {
            try
            {
                completion.SetResult(action(display));
            }
            catch (Exception e)
            {
                completion.SetException(e);
            }
        });

        return completion.Task.Wait(TimeSpan.FromSeconds(5)) ? completion.Task.Result : fallback;
    }

    private void Run()
    {
        X11Display? display = X11Display.TryOpen();

        if (display == null)
        {
            running = false;
            started.SetResult(false);
            return;
        }

        threadDisplay = display;
        started.SetResult(true);

        using (display)
        {
            PollFd pollFd = new PollFd { fd = X11.XConnectionNumber(display.Display), events = PollIn };

            while (running)
            {
                while (commands.TryTake(out Action<X11Display>? command))
                {
                    command(display);
                }

                while (X11.XPending(display.Display) > 0)
                {
                    X11.XNextEvent(display.Display, out X11.XEvent xEvent);

                    if (xEvent.type == X11.KeyPress)
                    {
                        OnKeyPress(xEvent.xkey);
                    }
                }

                // Wake up for X events, or every 50 ms to run queued registration commands.
                poll(ref pollFd, 1, 50);
            }

            foreach (Registration registration in registrations.Values)
            {
                foreach (uint lockMask in LockCombinations)
                {
                    X11.XUngrabKey(display.Display, registration.KeyCode, registration.Modifiers | lockMask, display.Root);
                }
            }

            X11.XSync(display.Display, false);
        }
    }

    private void OnKeyPress(X11.XKeyEvent keyEvent)
    {
        uint modifiers = keyEvent.state & RelevantModifiers;

        foreach (KeyValuePair<int, Registration> pair in registrations)
        {
            if (pair.Value.KeyCode == keyEvent.keycode && pair.Value.Modifiers == modifiers)
            {
                HotkeyPressed?.Invoke(this, new HotkeyPressedEventArgs(pair.Key, pair.Value.Hotkey));
                break;
            }
        }
    }

    public void Dispose()
    {
        if (!running)
        {
            return;
        }

        running = false;
        thread.Join(TimeSpan.FromSeconds(2));
        commands.Dispose();
    }

    private const short PollIn = 0x1;

    [StructLayout(LayoutKind.Sequential)]
    private struct PollFd
    {
        public int fd;
        public short events;
        public short revents;
    }

    [LibraryImport("libc", SetLastError = true)]
    private static partial int poll(ref PollFd fds, nuint count, int timeout);
}
