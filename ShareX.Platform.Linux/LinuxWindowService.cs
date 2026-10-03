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
using ShareX.Platform.Linux.Desktop;
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
/// Windows and the pointer. The per-desktop work is done by the backend <see cref="LinuxDesktop"/> chooses: EWMH on X11,
/// hyprctl on Hyprland, swaymsg on sway; GNOME and KDE on Wayland do not expose other applications' windows. Operations on
/// ShareX's own windows use X11 in every session, since Avalonia runs ShareX as an X11 client (through XWayland on Wayland).
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

    private readonly IDesktopWindowBackend backend;
    private readonly HashSet<long> clickThroughWindows = new HashSet<long>();

    public LinuxWindowService(PlatformInfo info, ICommandRunner runner)
        : this(new LinuxDesktop(info).CreateWindowBackend(runner))
    {
    }

    internal LinuxWindowService(IDesktopWindowBackend backend)
    {
        this.backend = backend;
    }

    internal Backend ActiveBackend => backend.Kind;

    internal IDesktopWindowBackend DesktopBackend => backend;

    public FeatureSupport Support => backend.Support;

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

    internal static Backend SelectBackend(PlatformInfo info) => LinuxDesktop.GetKind(info) switch
    {
        LinuxDesktopKind.X11 => Backend.X11,
        LinuxDesktopKind.Hyprland => Backend.Hyprland,
        LinuxDesktopKind.Sway => Backend.Sway,
        _ => Backend.None
    };

    public IReadOnlyList<PlatformWindow> GetWindows() => backend.GetWindows();

    public PlatformPoint? GetCursorPosition() => backend.GetCursorPosition();

    /// <summary>hyprctl cursorpos prints "x, y" in layout coordinates.</summary>
    internal static PlatformPoint? ParseHyprlandCursorPosition(string output)
    {
        string[] parts = output.Trim().Split(',', StringSplitOptions.TrimEntries);

        return parts.Length == 2 &&
            double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x) &&
            double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y)
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
        IReadOnlyList<PlatformWindow> windows = backend.GetSnapWindows();

        // Compositor window ids never match the X11 id Avalonia gives ShareX's own window under XWayland, so leave out
        // every window of this process; otherwise the full screen region capture window would cover everything.
        int ownProcess = Environment.ProcessId;
        return SnapTarget.FromWindows(backend.IdentifiesOwnWindowsByProcess ? windows.Where(window => window.ProcessId != ownProcess) : windows, ignoredHandle);
    }

    public double GetOwnWindowPixelScale(PlatformPoint point) => backend.GetOwnWindowPixelScale(point);

    /// <summary>hyprctl getoption -j reports a boolean option as "bool": true, or "int": 1 on older versions.</summary>
    internal static bool? ParseHyprlandBoolOption(JsonElement option)
    {
        if (option.TryGetProperty("bool", out JsonElement value) && value.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            return value.GetBoolean();
        }

        return option.TryGetProperty("int", out value) && value.ValueKind == JsonValueKind.Number ? value.GetInt32() != 0 : null;
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

    public bool SetCursorPosition(PlatformPoint position) => backend.SetCursorPosition(position);

    /// <summary>Activating a window also brings it back from minimised on X11, Hyprland and sway.</summary>
    public bool RestoreWindow(long windowHandle) => ActivateWindow(windowHandle);

    public bool ActivateWindow(long windowHandle) => backend.ActivateWindow(windowHandle);

    internal static string FormatHyprlandAddress(long handle) => HyprlandDispatcher.Address(handle);

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

    // Overlays need an always on top, click through window with per pixel alpha placed at exact desktop coordinates, which
    // Wayland compositors do not allow and which is not implemented for X11 and macOS yet.
    public FeatureSupport OverlaySupport { get; } = FeatureSupport.NotSupported("Drawing over other applications is not available on this platform yet.");

    public IScreenOverlay CreateOverlay(PlatformRectangle screenBounds) => throw new PlatformNotSupportedException(OverlaySupport.Reason);

    public long GetActiveWindowHandle() => GetActiveWindow()?.Handle ?? 0;

    public PlatformRectangle? GetWindowBounds(long windowHandle) => backend.GetWindowBounds(windowHandle);

    public PlatformRectangle? GetClientBounds(long windowHandle) => backend.GetClientBounds(windowHandle);

    public PlatformWindow? GetActiveWindow() => backend.GetActiveWindow();

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

    internal static IReadOnlyList<PlatformWindow> ParseSwayTree(JsonElement root, bool focusedOnly)
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
}
