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

using ShareX.Platform.Diagnostics;
using ShareX.Platform.Linux.Native;
using System;

namespace ShareX.Platform.Linux;

/// <summary>
/// Synthetic input through XTEST on X11 and hyprctl sendshortcut on Hyprland. Other Wayland compositors give applications no
/// way to send input to other windows.
/// </summary>
public sealed class LinuxInputService : IInputService
{
    private const string WaylandReason = "Wayland does not let applications send keys or mouse wheel turns to other windows.";

    // X11 mouse buttons 4 and 5 are the wheel turning up and down.
    private const uint WheelUp = 4;
    private const uint WheelDown = 5;

    private readonly PlatformInfo info;
    private readonly ICommandRunner runner;

    public LinuxInputService(PlatformInfo info, ICommandRunner runner)
    {
        this.info = info;
        this.runner = runner;
        bool x11 = info.IsX11 && !info.IsSandboxed;
        bool hyprland = info.IsWayland && info.DesktopEnvironment == DesktopEnvironment.Hyprland;

        KeyboardSupport = x11 || hyprland ? FeatureSupport.Supported : FeatureSupport.NotSupported(WaylandReason);
        MouseWheelSupport = x11 ? FeatureSupport.Supported : FeatureSupport.NotSupported(WaylandReason);
    }

    public FeatureSupport KeyboardSupport { get; }

    public FeatureSupport MouseWheelSupport { get; }

    public FeatureSupport WindowScrollSupport { get; } =
        FeatureSupport.NotSupported("Only Windows lets one application drive another application's scroll bars. Use the mouse wheel or a key instead.");

    public bool SendKeyPress(int virtualKey)
    {
        if (!KeyboardSupport.IsSupported || !XKeyMap.TryGetKeysym(virtualKey, out uint keysym, out string name))
        {
            return false;
        }

        if (info.IsX11)
        {
            return WithXTest(display =>
            {
                byte keycode = X11.XKeysymToKeycode(display.Display, keysym);
                return keycode != 0 &&
                    X11.XTestFakeKeyEvent(display.Display, keycode, true, 0) != 0 &&
                    X11.XTestFakeKeyEvent(display.Display, keycode, false, 0) != 0;
            });
        }

        // "MODS, KEY, WINDOW": no modifiers, to the focused window.
        return Run("hyprctl", ["dispatch", "sendshortcut", $", {name}, activewindow"]);
    }

    public bool SendMouseWheel(int detents)
    {
        if (!MouseWheelSupport.IsSupported || detents == 0)
        {
            return false;
        }

        uint button = detents > 0 ? WheelUp : WheelDown;

        return WithXTest(display =>
        {
            for (int i = 0; i < Math.Abs(detents); i++)
            {
                if (X11.XTestFakeButtonEvent(display.Display, button, true, 0) == 0 || X11.XTestFakeButtonEvent(display.Display, button, false, 0) == 0)
                {
                    return false;
                }
            }

            return true;
        });
    }

    public bool ScrollWindow(long windowHandle, WindowScrollCommand command) => false;

    private static bool WithXTest(Func<X11Display, bool> action)
    {
        using X11Display? display = X11Display.TryOpen();

        if (display == null)
        {
            return false;
        }

        try
        {
            bool result = action(display);
            X11.XFlush(display.Display);
            return result;
        }
        catch (DllNotFoundException)
        {
            // libXtst is not installed.
            return false;
        }
    }

    private bool Run(string command, string[] arguments)
    {
        try
        {
            return runner.RunAsync(command, arguments, timeout: TimeSpan.FromSeconds(2)).GetAwaiter().GetResult().Success;
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return false;
        }
    }
}
