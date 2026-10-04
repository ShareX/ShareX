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
using ShareX.Platform.Linux.DBus;
using System;
using System.Collections.Generic;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>The kinds of Linux desktop ShareX adapts to; each gets its own strategies behind the shared contracts.</summary>
public enum LinuxDesktopKind
{
    /// <summary>An X11 session: Xlib, EWMH and XGrabKey work for every desktop.</summary>
    X11,
    /// <summary>Hyprland: hyprctl IPC, grim, the Hyprland portal.</summary>
    Hyprland,
    /// <summary>sway and its i3-compatible IPC, grim, the wlr portal.</summary>
    Sway,
    /// <summary>GNOME on Wayland: only portals.</summary>
    Gnome,
    /// <summary>KDE Plasma on Wayland: only portals.</summary>
    Kde,
    /// <summary>Any other Wayland compositor: portals where they exist.</summary>
    OtherWayland
}

/// <summary>
/// The desktop ShareX runs on, and the place that decides which implementation each service uses there. Services that differ
/// between compositors ask this class instead of checking the desktop themselves.
/// </summary>
public sealed class LinuxDesktop
{
    private readonly PlatformInfo info;

    public LinuxDesktop(PlatformInfo info)
    {
        this.info = info;
        Kind = GetKind(info);
    }

    public LinuxDesktopKind Kind { get; }

    internal static LinuxDesktopKind GetKind(PlatformInfo info)
    {
        if (info.IsX11)
        {
            return LinuxDesktopKind.X11;
        }

        return info.DesktopEnvironment switch
        {
            DesktopEnvironment.Hyprland => LinuxDesktopKind.Hyprland,
            DesktopEnvironment.Sway => LinuxDesktopKind.Sway,
            DesktopEnvironment.Gnome => LinuxDesktopKind.Gnome,
            DesktopEnvironment.Kde => LinuxDesktopKind.Kde,
            _ => LinuxDesktopKind.OtherWayland
        };
    }

    /// <summary>x11grab on X11, wf-recorder on wlroots compositors, the ScreenCast portal with GStreamer on GNOME, KDE and other Wayland desktops.</summary>
    internal IScreenRecordingBackend CreateRecordingBackend(ICommandRunner runner, Func<IReadOnlyList<ScreenInfo>> getScreens) =>
        // SHAREX_RECORDING_BACKEND=portal tries the portal path on any Wayland desktop, to test it where another backend is the default.
        info.IsWayland && Environment.GetEnvironmentVariable("SHAREX_RECORDING_BACKEND") == "portal"
            ? new PortalRecordingBackend(runner, info.Distribution ?? LinuxDistribution.Unknown)
            : CreateDefaultRecordingBackend(runner, getScreens);

    private IScreenRecordingBackend CreateDefaultRecordingBackend(ICommandRunner runner, Func<IReadOnlyList<ScreenInfo>> getScreens) => Kind switch
    {
        LinuxDesktopKind.X11 => new X11GrabRecordingBackend(),
        LinuxDesktopKind.Hyprland or LinuxDesktopKind.Sway => new WfRecorderRecordingBackend(runner, info.Distribution ?? LinuxDistribution.Unknown, getScreens),
        _ when info.DisplayServer == DisplayServer.None => new UnsupportedRecordingBackend("No graphical session was found."),
        _ => new PortalRecordingBackend(runner, info.Distribution ?? LinuxDistribution.Unknown)
    };

    /// <summary>The X11 root window on X11, grim on wlroots compositors, the Screenshot portal elsewhere; null when none works.</summary>
    internal IScreenCaptureBackend? CreateCaptureBackend(ICommandRunner runner) =>
        // SHAREX_CAPTURE_BACKEND=portal ignores grim, to test the portal path that wlroots desktops fall back to without it.
        LinuxScreenCaptureService.SelectBackend(info, runner.Exists("grim") && Environment.GetEnvironmentVariable("SHAREX_CAPTURE_BACKEND") != "portal", DBusSession.IsAvailable) switch
        {
            LinuxScreenCaptureService.Backend.X11 => new X11CaptureBackend(),
            LinuxScreenCaptureService.Backend.Grim => new GrimCaptureBackend(runner),
            LinuxScreenCaptureService.Backend.Portal => new PortalCaptureBackend(),
            _ => null
        };

    /// <summary>EWMH on X11, hyprctl on Hyprland, swaymsg on sway; GNOME, KDE and other Wayland desktops do not expose windows.</summary>
    internal IDesktopWindowBackend CreateWindowBackend(ICommandRunner runner) => Kind switch
    {
        LinuxDesktopKind.X11 => new X11WindowBackend(),
        LinuxDesktopKind.Hyprland => new HyprlandWindowBackend(new CompositorCommands(runner)),
        LinuxDesktopKind.Sway => new SwayWindowBackend(new CompositorCommands(runner)),
        _ => new UnsupportedWindowBackend($"{info.DesktopEnvironmentName} on Wayland does not let applications list other windows. Use region capture or the portal's window picker instead.")
    };

    /// <summary>
    /// X11: XGrabKey. Hyprland: the GlobalShortcuts portal, with ShareX binding the keys. sway: IPC binds (its portal has no
    /// GlobalShortcuts). GNOME, KDE and others: the GlobalShortcuts portal, whose dialog lets the user confirm the keys.
    /// </summary>
    public IHotkeyService CreateHotkeyService(ICommandRunner runner, Func<int, string>? describeHotkey)
    {
        if (Kind == LinuxDesktopKind.X11 && !info.IsSandboxed)
        {
            return new X11HotkeyService();
        }

        if (Kind == LinuxDesktopKind.Sway)
        {
            SwayHotkeyService sway = new SwayHotkeyService(runner);

            if (sway.Support.IsSupported)
            {
                return sway;
            }
        }

        if (DBusSession.IsAvailable)
        {
            IShortcutKeyBinder? binder = Kind == LinuxDesktopKind.Hyprland ? new HyprlandShortcutKeyBinder(runner, DBusSession.ApplicationId) : null;
            return new PortalGlobalShortcutsService(describeHotkey, binder);
        }

        return new UnsupportedHotkeyService("Global hotkeys need an X11 session, sway, or the xdg-desktop-portal GlobalShortcuts interface.");
    }
}
