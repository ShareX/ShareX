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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace ShareX.Platform.Linux;

/// <summary>
/// Window enumeration through EWMH properties on X11, hyprctl on Hyprland and swaymsg on sway.
/// GNOME and KDE Wayland do not expose other applications' windows to clients.
/// </summary>
public sealed class LinuxWindowService : IWindowService
{
    internal enum Backend
    {
        None,
        X11,
        Hyprland,
        Sway
    }

    private readonly ICommandRunner runner;
    private readonly HashSet<long> clickThroughWindows = new HashSet<long>();

    public LinuxWindowService(PlatformInfo info, ICommandRunner runner)
    {
        this.runner = runner;
        ActiveBackend = SelectBackend(info);
        Support = ActiveBackend != Backend.None
            ? FeatureSupport.Supported
            : FeatureSupport.NotSupported($"{info.DesktopEnvironmentName} on Wayland does not let applications list other windows. Use region capture or the portal's window picker instead.");
    }

    internal Backend ActiveBackend { get; }

    /// <summary>
    /// ShareX's windows are X11 windows (natively or under XWayland), so the EWMH urgency state works on every Linux desktop that
    /// runs them; Hyprland, GNOME and KDE show urgent windows in their bars and borders.
    /// </summary>
    public bool RequestAttention(long windowHandle, int count)
    {
        if (windowHandle == 0)
        {
            return false;
        }

        using X11Display? display = X11Display.TryOpen();
        return display != null && display.ChangeWmState((nuint)windowHandle, 1, "_NET_WM_STATE_DEMANDS_ATTENTION");
    }

    public FeatureSupport Support { get; }

    internal static Backend SelectBackend(PlatformInfo info)
    {
        if (info.IsX11)
        {
            return Backend.X11;
        }

        return info.DesktopEnvironment switch
        {
            DesktopEnvironment.Hyprland when info.IsWayland => Backend.Hyprland,
            DesktopEnvironment.Sway when info.IsWayland => Backend.Sway,
            _ => Backend.None
        };
    }

    public IReadOnlyList<PlatformWindow> GetWindows() => ActiveBackend switch
    {
        Backend.X11 => GetX11Windows(),
        Backend.Hyprland => RunJson("hyprctl", ["clients", "-j"], ParseHyprlandClients),
        Backend.Sway => RunJson("swaymsg", ["-t", "get_tree", "-r"], ParseSwayTree),
        _ => Array.Empty<PlatformWindow>()
    };

    public PlatformPoint? GetCursorPosition()
    {
        switch (ActiveBackend)
        {
            case Backend.X11:
                using (X11Display? display = X11Display.TryOpen())
                {
                    return display?.GetPointerPosition();
                }
            case Backend.Hyprland:
                try
                {
                    CommandResult result = runner.RunAsync("hyprctl", ["cursorpos"], timeout: TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
                    return result.Success ? ParseHyprlandCursorPosition(result.StandardOutputText) : null;
                }
                catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException)
                {
                    return null;
                }
            default:
                // sway and the other Wayland compositors do not tell clients where the pointer is.
                return null;
        }
    }

    /// <summary>hyprctl cursorpos prints "x, y" in layout coordinates.</summary>
    internal static PlatformPoint? ParseHyprlandCursorPosition(string output)
    {
        string[] parts = output.Trim().Split(',', StringSplitOptions.TrimEntries);

        return parts.Length == 2 &&
            double.TryParse(parts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double x) &&
            double.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double y)
            ? new PlatformPoint((int)Math.Round(x), (int)Math.Round(y))
            : null;
    }

    // X11 could grab the pointer, but a grab also blocks input to every other client, which is worse than no confinement.
    // Wayland does not let clients confine the pointer outside their own surface at all.
    public void ReleaseMouseCapture()
    {
    }

    public bool ConfineCursor(long windowHandle) => false;

    public void ReleaseCursorConfinement()
    {
    }

    public IReadOnlyList<SnapTarget> GetSnapTargets(bool includeControls, long ignoredHandle, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<PlatformWindow> windows = ActiveBackend switch
        {
            // Clients on hidden workspaces still report their last position, so keep only the workspaces on screen.
            Backend.Hyprland => GetHyprlandVisibleWindows(),
            Backend.Sway => GetWindows().Where(window => !window.IsMinimized).ToList(),
            _ => GetWindows()
        };

        // Compositor window ids never match the X11 id Avalonia gives ShareX's own window under XWayland, so leave out
        // every window of this process; otherwise the full screen region capture window would cover everything.
        int ownProcess = Environment.ProcessId;
        return SnapTarget.FromWindows(ActiveBackend == Backend.X11 ? windows : windows.Where(window => window.ProcessId != ownProcess), ignoredHandle);
    }

    private bool? hyprlandZeroScaling;

    public double GetOwnWindowPixelScale(PlatformPoint point)
    {
        if (ActiveBackend != Backend.Hyprland)
        {
            return 1;
        }

        hyprlandZeroScaling ??= ReadJson("hyprctl", ["getoption", "xwayland:force_zero_scaling", "-j"], ParseHyprlandBoolOption) ?? false;

        if (hyprlandZeroScaling != true)
        {
            return 1;
        }

        IReadOnlyList<ScreenInfo> screens = ReadJson("hyprctl", ["monitors", "-j"], LinuxScreenCaptureService.ParseHyprlandMonitors) ?? [];
        ScreenInfo? screen = screens.FirstOrDefault(s => s.Bounds.Contains(point)) ?? screens.FirstOrDefault(s => s.IsPrimary) ?? screens.FirstOrDefault();
        return screen?.ScaleFactor ?? 1;
    }

    /// <summary>hyprctl getoption -j reports a boolean option as "bool": true, or "int": 1 on older versions.</summary>
    internal static bool? ParseHyprlandBoolOption(JsonElement option)
    {
        if (option.TryGetProperty("bool", out JsonElement value) && value.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            return value.GetBoolean();
        }

        return option.TryGetProperty("int", out value) && value.ValueKind == JsonValueKind.Number ? value.GetInt32() != 0 : null;
    }

    private T? ReadJson<T>(string command, IReadOnlyList<string> arguments, Func<JsonElement, T> parse)
    {
        try
        {
            CommandResult result = runner.RunAsync(command, arguments, timeout: TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

            if (result.Success)
            {
                using JsonDocument document = JsonDocument.Parse(result.StandardOutput);
                return parse(document.RootElement);
            }
        }
        catch (Exception e) when (e is JsonException or TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException or KeyNotFoundException or FormatException)
        {
        }

        return default;
    }

    private IReadOnlyList<PlatformWindow> GetHyprlandVisibleWindows()
    {
        HashSet<int> workspaces = new HashSet<int>();

        RunJson("hyprctl", ["monitors", "-j"], root =>
        {
            workspaces.UnionWith(ParseHyprlandVisibleWorkspaces(root));
            return Array.Empty<PlatformWindow>();
        });

        return RunJson("hyprctl", ["clients", "-j"], root => ParseHyprlandClients(root, workspaces));
    }

    /// <summary>The active workspace of every monitor, plus a special workspace shown over it.</summary>
    internal static IReadOnlyCollection<int> ParseHyprlandVisibleWorkspaces(JsonElement monitors)
    {
        HashSet<int> workspaces = new HashSet<int>();

        foreach (JsonElement monitor in monitors.EnumerateArray())
        {
            foreach (string property in new[] { "activeWorkspace", "specialWorkspace" })
            {
                if (monitor.TryGetProperty(property, out JsonElement workspace) && workspace.TryGetProperty("id", out JsonElement id) && id.GetInt32() != 0)
                {
                    workspaces.Add(id.GetInt32());
                }
            }
        }

        return workspaces;
    }

    public bool SetCursorPosition(PlatformPoint position)
    {
        string x = position.X.ToString(CultureInfo.InvariantCulture);
        string y = position.Y.ToString(CultureInfo.InvariantCulture);

        switch (ActiveBackend)
        {
            case Backend.X11:
                using (X11Display? display = X11Display.TryOpen())
                {
                    if (display == null) return false;
                    X11.XWarpPointer(display.Display, 0, display.Root, 0, 0, 0, 0, position.X, position.Y);
                    X11.XFlush(display.Display);
                    return true;
                }
            case Backend.Hyprland:
                return Run("hyprctl", ["dispatch", "movecursor", x, y]);
            case Backend.Sway:
                return Run("swaymsg", ["seat", "-", "cursor", "set", x, y]);
            default:
                return false;
        }
    }

    /// <summary>Activating a window also brings it back from minimised on X11, Hyprland and sway.</summary>
    public bool RestoreWindow(long windowHandle) => ActivateWindow(windowHandle);

    public bool ActivateWindow(long windowHandle)
    {
        switch (ActiveBackend)
        {
            case Backend.X11:
                using (X11Display? display = X11Display.TryOpen())
                {
                    return display != null && display.RequestActivation((nuint)windowHandle);
                }
            case Backend.Hyprland:
                return Run("hyprctl", ["dispatch", "focuswindow", FormatHyprlandAddress(windowHandle)]);
            case Backend.Sway:
                return Run("swaymsg", [string.Create(CultureInfo.InvariantCulture, $"[con_id={windowHandle}]"), "focus"]);
            default:
                return false;
        }
    }

    internal static string FormatHyprlandAddress(long handle) => "address:0x" + handle.ToString("x", CultureInfo.InvariantCulture);

    // ShareX's own windows are X11 windows (Avalonia uses X11, through XWayland on Wayland sessions), so the SHAPE extension works
    // in both kinds of session. Hiding from the task bar is left to Avalonia's ShowInTaskbar.
    public bool SetOverlayStyle(long windowHandle, bool clickThrough)
    {
        if (!clickThrough)
        {
            return false;
        }

        lock (clickThroughWindows)
        {
            clickThroughWindows.Add(windowHandle);
        }

        using X11Display? display = X11Display.TryOpen();
        return display != null && display.SetInputShape((nuint)windowHandle, Array.Empty<PlatformRectangle>());
    }

    public bool SetWindowShape(long windowHandle, IReadOnlyList<PlatformRectangle> visibleAreas)
    {
        // A click through window stays click through, as WS_EX_TRANSPARENT wins over the window region on Windows.
        lock (clickThroughWindows)
        {
            if (clickThroughWindows.Contains(windowHandle))
            {
                visibleAreas = Array.Empty<PlatformRectangle>();
            }
        }

        // Only the input region is shaped: the window's transparent background already hides the rest.
        using X11Display? display = X11Display.TryOpen();
        return display != null && display.SetInputShape((nuint)windowHandle, visibleAreas);
    }

    private bool Run(string command, IReadOnlyList<string> arguments)
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

    // Overlays need an always on top, click through window with per pixel alpha placed at exact desktop coordinates, which
    // Wayland compositors do not allow and which is not implemented for X11 and macOS yet.
    public FeatureSupport OverlaySupport { get; } = FeatureSupport.NotSupported("Drawing over other applications is not available on this platform yet.");

    public IScreenOverlay CreateOverlay(PlatformRectangle screenBounds) => throw new PlatformNotSupportedException(OverlaySupport.Reason);

    public long GetActiveWindowHandle() => GetActiveWindow()?.Handle ?? 0;

    public PlatformRectangle? GetWindowBounds(long windowHandle)
    {
        if (ActiveBackend == Backend.X11)
        {
            using X11Display? display = X11Display.TryOpen();
            return display?.GetWindowBounds((nuint)windowHandle, includeFrame: true);
        }

        return GetWindows().FirstOrDefault(window => window.Handle == windowHandle)?.Bounds;
    }

    /// <summary>X11 knows the client window without the frame the window manager adds; compositors only report the whole window.</summary>
    public PlatformRectangle? GetClientBounds(long windowHandle)
    {
        if (ActiveBackend == Backend.X11)
        {
            using X11Display? display = X11Display.TryOpen();
            return display?.GetWindowBounds((nuint)windowHandle, includeFrame: false);
        }

        return GetWindowBounds(windowHandle);
    }

    public PlatformWindow? GetActiveWindow()
    {
        switch (ActiveBackend)
        {
            case Backend.X11:
                using (X11Display? display = X11Display.TryOpen())
                {
                    if (display == null) return null;
                    nuint[] active = display.GetLongProperty(display.Root, "_NET_ACTIVE_WINDOW", 1);
                    return active.Length == 1 && active[0] != 0 ? ReadX11Window(display, active[0]) : null;
                }
            case Backend.Hyprland:
                IReadOnlyList<PlatformWindow> windows = RunJson("hyprctl", ["activewindow", "-j"],
                    root => root.ValueKind == JsonValueKind.Object ? ParseHyprlandClients(root) : Array.Empty<PlatformWindow>());
                return windows.FirstOrDefault();
            case Backend.Sway:
                return RunJson("swaymsg", ["-t", "get_tree", "-r"], root => ParseSwayTree(root, focusedOnly: true)).FirstOrDefault();
            default:
                return null;
        }
    }

    private static IReadOnlyList<PlatformWindow> GetX11Windows()
    {
        using X11Display? display = X11Display.TryOpen();

        if (display == null)
        {
            return Array.Empty<PlatformWindow>();
        }

        // Stacking order is bottom to top. Reverse it so the topmost window comes first, as on Windows.
        nuint[] clients = display.GetLongProperty(display.Root, "_NET_CLIENT_LIST_STACKING");

        if (clients.Length == 0)
        {
            clients = display.GetLongProperty(display.Root, "_NET_CLIENT_LIST");
        }

        List<PlatformWindow> windows = new List<PlatformWindow>(clients.Length);

        for (int i = clients.Length - 1; i >= 0; i--)
        {
            PlatformWindow? window = ReadX11Window(display, clients[i]);

            if (window != null && !window.Bounds.IsEmpty)
            {
                windows.Add(window);
            }
        }

        return windows;
    }

    private static PlatformWindow? ReadX11Window(X11Display display, nuint window)
    {
        PlatformRectangle? bounds = display.GetWindowBounds(window, includeFrame: true);

        if (bounds == null)
        {
            return null;
        }

        string title = display.GetStringProperty(window, "_NET_WM_NAME") ?? display.GetStringProperty(window, "WM_NAME") ?? "";
        nuint[] pid = display.GetLongProperty(window, "_NET_WM_PID", 1);
        int? processId = pid.Length == 1 ? (int)pid[0] : null;
        nuint hidden = display.GetAtom("_NET_WM_STATE_HIDDEN");
        bool minimized = display.GetLongProperty(window, "_NET_WM_STATE").Contains(hidden);

        return new PlatformWindow((long)window, title, processId != null ? GetProcessName(processId.Value) : null, processId, bounds.Value, minimized);
    }

    internal static string? GetProcessName(int processId)
    {
        try
        {
            return File.ReadAllText($"/proc/{processId.ToString(CultureInfo.InvariantCulture)}/comm").Trim();
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    internal static IReadOnlyList<PlatformWindow> ParseHyprlandClients(JsonElement root) => ParseHyprlandClients(root, null);

    /// <param name="visibleWorkspaces">When given, only clients on these workspaces.</param>
    internal static IReadOnlyList<PlatformWindow> ParseHyprlandClients(JsonElement root, IReadOnlyCollection<int>? visibleWorkspaces)
    {
        List<(int FocusHistory, PlatformWindow Window)> windows = new List<(int, PlatformWindow)>();
        IEnumerable<JsonElement> clients = root.ValueKind == JsonValueKind.Array ? root.EnumerateArray() : new[] { root };

        foreach (JsonElement client in clients)
        {
            if (client.TryGetProperty("mapped", out JsonElement mapped) && mapped.ValueKind == JsonValueKind.False)
            {
                continue;
            }

            if (visibleWorkspaces != null &&
                (!client.TryGetProperty("workspace", out JsonElement workspace) || !workspace.TryGetProperty("id", out JsonElement workspaceId) ||
                !visibleWorkspaces.Contains(workspaceId.GetInt32())))
            {
                continue;
            }

            JsonElement at = client.GetProperty("at");
            JsonElement size = client.GetProperty("size");
            PlatformRectangle bounds = new PlatformRectangle(at[0].GetInt32(), at[1].GetInt32(), size[0].GetInt32(), size[1].GetInt32());
            // Hyprland addresses look like "0x55d0c2a6e3c0".
            string address = client.GetProperty("address").GetString() ?? "0x0";
            long handle = long.Parse(address.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            int? pid = client.TryGetProperty("pid", out JsonElement p) && p.GetInt32() > 0 ? p.GetInt32() : null;
            string title = client.TryGetProperty("title", out JsonElement t) ? t.GetString() ?? "" : "";
            string? className = client.TryGetProperty("class", out JsonElement c) ? c.GetString() : null;
            bool hidden = client.TryGetProperty("hidden", out JsonElement h) && h.ValueKind == JsonValueKind.True;
            int focusHistory = client.TryGetProperty("focusHistoryID", out JsonElement f) ? f.GetInt32() : int.MaxValue;
            string? processName = pid != null ? GetProcessName(pid.Value) ?? className : className;
            windows.Add((focusHistory, new PlatformWindow(handle, title, processName, pid, bounds, hidden)));
        }

        // focusHistoryID 0 is the most recently focused window, the closest Hyprland has to Z order.
        return windows.OrderBy(w => w.FocusHistory).Select(w => w.Window).ToList();
    }

    internal static IReadOnlyList<PlatformWindow> ParseSwayTree(JsonElement root) => ParseSwayTree(root, focusedOnly: false);

    private static IReadOnlyList<PlatformWindow> ParseSwayTree(JsonElement root, bool focusedOnly)
    {
        List<PlatformWindow> windows = new List<PlatformWindow>();
        Visit(root);
        return windows;

        void Visit(JsonElement node)
        {
            bool isView = node.TryGetProperty("pid", out JsonElement pidElement) && pidElement.ValueKind == JsonValueKind.Number;

            if (isView && (!focusedOnly || (node.TryGetProperty("focused", out JsonElement focused) && focused.ValueKind == JsonValueKind.True)))
            {
                JsonElement rect = node.GetProperty("rect");
                PlatformRectangle bounds = new PlatformRectangle(rect.GetProperty("x").GetInt32(), rect.GetProperty("y").GetInt32(),
                    rect.GetProperty("width").GetInt32(), rect.GetProperty("height").GetInt32());
                int pid = pidElement.GetInt32();
                string title = node.TryGetProperty("name", out JsonElement n) ? n.GetString() ?? "" : "";
                string? appId = node.TryGetProperty("app_id", out JsonElement a) && a.ValueKind == JsonValueKind.String ? a.GetString() : null;
                bool visible = !node.TryGetProperty("visible", out JsonElement v) || v.ValueKind != JsonValueKind.False;
                windows.Add(new PlatformWindow(node.GetProperty("id").GetInt64(), title, GetProcessName(pid) ?? appId, pid, bounds, !visible));
            }

            foreach (string children in new[] { "nodes", "floating_nodes" })
            {
                if (node.TryGetProperty(children, out JsonElement list))
                {
                    foreach (JsonElement child in list.EnumerateArray())
                    {
                        Visit(child);
                    }
                }
            }
        }
    }

    private IReadOnlyList<PlatformWindow> RunJson(string command, IReadOnlyList<string> arguments, Func<JsonElement, IReadOnlyList<PlatformWindow>> parse)
    {
        try
        {
            CommandResult result = runner.RunAsync(command, arguments, timeout: TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

            if (result.Success)
            {
                using JsonDocument document = JsonDocument.Parse(result.StandardOutput);
                return parse(document.RootElement);
            }
        }
        catch (Exception e) when (e is JsonException or TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException or KeyNotFoundException or FormatException)
        {
        }

        return Array.Empty<PlatformWindow>();
    }
}
