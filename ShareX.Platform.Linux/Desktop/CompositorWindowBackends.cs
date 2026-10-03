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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>Hyprland: hyprctl IPC in layout coordinates.</summary>
internal sealed class HyprlandWindowBackend(CompositorCommands commands) : IDesktopWindowBackend
{
    private readonly HyprlandDispatcher dispatcher = new HyprlandDispatcher(commands);
    private bool? zeroScaling;

    public LinuxWindowService.Backend Kind => LinuxWindowService.Backend.Hyprland;

    public FeatureSupport Support => FeatureSupport.Supported;

    public bool IdentifiesOwnWindowsByProcess => true;

    internal HyprlandDispatcher Dispatcher => dispatcher;

    public IReadOnlyList<PlatformWindow> GetWindows() => commands.ReadList("hyprctl", ["clients", "-j"], LinuxWindowService.ParseHyprlandClients);

    public IReadOnlyList<ScreenInfo> GetScreens() => commands.ReadList("hyprctl", ["monitors", "-j"], LinuxScreenCaptureService.ParseHyprlandMonitors);

    /// <summary>Clients on hidden workspaces still report their last position, so keep only the workspaces on screen.</summary>
    public IReadOnlyList<PlatformWindow> GetSnapWindows()
    {
        IReadOnlyCollection<int> workspaces = commands.ReadJson("hyprctl", ["monitors", "-j"], LinuxWindowService.ParseHyprlandVisibleWorkspaces) ?? Array.Empty<int>();
        return commands.ReadList("hyprctl", ["clients", "-j"], root => LinuxWindowService.ParseHyprlandClients(root, workspaces));
    }

    public PlatformWindow? GetActiveWindow() => commands.ReadList("hyprctl", ["activewindow", "-j"],
        root => root.ValueKind == JsonValueKind.Object ? LinuxWindowService.ParseHyprlandClients(root) : Array.Empty<PlatformWindow>()).FirstOrDefault();

    public PlatformPoint? GetCursorPosition() =>
        commands.ReadText("hyprctl", ["cursorpos"]) is string output ? LinuxWindowService.ParseHyprlandCursorPosition(output) : null;

    public bool SetCursorPosition(PlatformPoint position) => dispatcher.MoveCursor(position);

    public bool ActivateWindow(long windowHandle) => dispatcher.Focus(windowHandle);

    public PlatformRectangle? GetWindowBounds(long windowHandle) => GetWindows().FirstOrDefault(window => window.Handle == windowHandle)?.Bounds;

    /// <summary>Hyprland only reports the whole window.</summary>
    public PlatformRectangle? GetClientBounds(long windowHandle) => GetWindowBounds(windowHandle);

    /// <summary>With xwayland:force_zero_scaling, ShareX's XWayland windows are placed in device pixels.</summary>
    public double GetOwnWindowPixelScale(PlatformPoint point)
    {
        zeroScaling ??= commands.ReadJson("hyprctl", ["getoption", "xwayland:force_zero_scaling", "-j"], LinuxWindowService.ParseHyprlandBoolOption) ?? false;

        if (zeroScaling != true)
        {
            return 1;
        }

        IReadOnlyList<ScreenInfo> screens = commands.ReadList("hyprctl", ["monitors", "-j"], LinuxScreenCaptureService.ParseHyprlandMonitors);
        ScreenInfo? screen = screens.FirstOrDefault(s => s.Bounds.Contains(point)) ?? screens.FirstOrDefault(s => s.IsPrimary) ?? screens.FirstOrDefault();
        return screen?.ScaleFactor ?? 1;
    }
}

/// <summary>sway: swaymsg IPC in layout coordinates. sway does not tell clients where the pointer is.</summary>
internal sealed class SwayWindowBackend(CompositorCommands commands) : IDesktopWindowBackend
{
    public LinuxWindowService.Backend Kind => LinuxWindowService.Backend.Sway;

    public FeatureSupport Support => FeatureSupport.Supported;

    public bool IdentifiesOwnWindowsByProcess => true;

    public IReadOnlyList<PlatformWindow> GetWindows() => commands.ReadList("swaymsg", ["-t", "get_tree", "-r"], LinuxWindowService.ParseSwayTree);

    public IReadOnlyList<ScreenInfo> GetScreens() => commands.ReadList("swaymsg", ["-t", "get_outputs", "-r"], LinuxScreenCaptureService.ParseSwayOutputs);

    public IReadOnlyList<PlatformWindow> GetSnapWindows() => GetWindows().Where(window => !window.IsMinimized).ToList();

    public PlatformWindow? GetActiveWindow() =>
        commands.ReadList("swaymsg", ["-t", "get_tree", "-r"], root => LinuxWindowService.ParseSwayTree(root, focusedOnly: true)).FirstOrDefault();

    public PlatformPoint? GetCursorPosition() => null;

    public bool SetCursorPosition(PlatformPoint position) =>
        commands.Run("swaymsg", ["seat", "-", "cursor", "set", position.X.ToString(System.Globalization.CultureInfo.InvariantCulture), position.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)]);

    public bool ActivateWindow(long windowHandle) =>
        commands.Run("swaymsg", [string.Create(System.Globalization.CultureInfo.InvariantCulture, $"[con_id={windowHandle}]"), "focus"]);

    public PlatformRectangle? GetWindowBounds(long windowHandle) => GetWindows().FirstOrDefault(window => window.Handle == windowHandle)?.Bounds;

    public PlatformRectangle? GetClientBounds(long windowHandle) => GetWindowBounds(windowHandle);

    public double GetOwnWindowPixelScale(PlatformPoint point) => 1;
}

/// <summary>GNOME, KDE and other Wayland desktops that do not let applications see other windows or the pointer.</summary>
internal sealed class UnsupportedWindowBackend(string reason) : IDesktopWindowBackend
{
    public LinuxWindowService.Backend Kind => LinuxWindowService.Backend.None;

    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public bool IdentifiesOwnWindowsByProcess => true;

    public IReadOnlyList<PlatformWindow> GetWindows() => Array.Empty<PlatformWindow>();

    public IReadOnlyList<PlatformWindow> GetSnapWindows() => Array.Empty<PlatformWindow>();

    /// <summary>GNOME and KDE do not expose the output layout to plain Wayland clients; callers fall back to the toolkit's list.</summary>
    public IReadOnlyList<ScreenInfo> GetScreens() => Array.Empty<ScreenInfo>();

    public PlatformWindow? GetActiveWindow() => null;

    public PlatformPoint? GetCursorPosition() => null;

    public bool SetCursorPosition(PlatformPoint position) => false;

    public bool ActivateWindow(long windowHandle) => false;

    public PlatformRectangle? GetWindowBounds(long windowHandle) => null;

    public PlatformRectangle? GetClientBounds(long windowHandle) => null;

    public double GetOwnWindowPixelScale(PlatformPoint point) => 1;
}
