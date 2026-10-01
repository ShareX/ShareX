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

    public LinuxWindowService(PlatformInfo info, ICommandRunner runner)
    {
        this.runner = runner;
        ActiveBackend = SelectBackend(info);
        Support = ActiveBackend != Backend.None
            ? FeatureSupport.Supported
            : FeatureSupport.NotSupported($"{info.DesktopEnvironmentName} on Wayland does not let applications list other windows. Use region capture or the portal's window picker instead.");
    }

    internal Backend ActiveBackend { get; }

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

    internal static IReadOnlyList<PlatformWindow> ParseHyprlandClients(JsonElement root)
    {
        List<(int FocusHistory, PlatformWindow Window)> windows = new List<(int, PlatformWindow)>();
        IEnumerable<JsonElement> clients = root.ValueKind == JsonValueKind.Array ? root.EnumerateArray() : new[] { root };

        foreach (JsonElement client in clients)
        {
            if (client.TryGetProperty("mapped", out JsonElement mapped) && mapped.ValueKind == JsonValueKind.False)
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
