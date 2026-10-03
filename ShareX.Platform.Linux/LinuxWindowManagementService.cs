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
using ShareX.Platform.Imaging;
using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ShareX.Platform.Linux;

/// <summary>
/// Inspecting and changing other windows: EWMH properties and client messages on X11, hyprctl on Hyprland, swaymsg on sway.
/// GNOME and KDE on Wayland do not let applications do this.
/// </summary>
public sealed class LinuxWindowManagementService : IWindowManagementService
{
    private readonly LinuxWindowService windows;
    private readonly ICommandRunner runner;

    public LinuxWindowManagementService(LinuxWindowService windows, ICommandRunner runner)
    {
        this.windows = windows;
        this.runner = runner;
    }

    public FeatureSupport Support => windows.Support;

    public FeatureSupport BorderlessSupport => windows.Support;

    public FeatureSupport GetSupport(WindowManagementFeature feature)
    {
        if (!Support.IsSupported)
        {
            return Support;
        }

        return (feature, Backend) switch
        {
            (WindowManagementFeature.Inspect, _) => FeatureSupport.Supported,
            (WindowManagementFeature.ChildControls, _) =>
                FeatureSupport.NotSupported("Linux applications draw their controls themselves, so only whole windows can be selected."),
            // Hyprland keeps only floating windows above others; WindowDetails.IsTopMost is null for tiled ones.
            (WindowManagementFeature.TopMost, LinuxWindowService.Backend.X11 or LinuxWindowService.Backend.Hyprland) => FeatureSupport.Supported,
            (WindowManagementFeature.TopMost, _) => FeatureSupport.NotSupported("This desktop does not let applications keep another window on top."),
            (WindowManagementFeature.Opacity, LinuxWindowService.Backend.X11) => FeatureSupport.Supported,
            (WindowManagementFeature.Opacity, _) => FeatureSupport.NotSupported("Wayland desktops do not let applications change another window's opacity."),
            _ => BorderlessSupport
        };
    }

    private LinuxWindowService.Backend Backend => windows.ActiveBackend;

    public long GetWindowAt(PlatformPoint point, bool topLevel)
    {
        // Only top level windows are visible to other applications here, so a control request also gets the window.
        SnapTarget? target = windows.GetSnapTargets(false, 0).FirstOrDefault(t => t.Bounds.Contains(point));
        return target?.Handle ?? 0;
    }

    public WindowDetails? GetDetails(long windowHandle)
    {
        switch (Backend)
        {
            case LinuxWindowService.Backend.X11:
                return GetX11Details(windowHandle);
            case LinuxWindowService.Backend.Hyprland:
                JsonElement? client = FindHyprlandClient(windowHandle);
                return client != null ? ParseHyprlandDetails(client.Value) : null;
            default:
                PlatformWindow? window = windows.GetWindows().FirstOrDefault(w => w.Handle == windowHandle);
                return window == null ? null : new WindowDetails(window.Handle, window.Title, null, window.ProcessName,
                    GetProcessPath(window.ProcessId), window.ProcessId, window.Bounds, null, [], [], null, null);
        }
    }

    public byte[]? GetIcon(long windowHandle)
    {
        if (Backend != LinuxWindowService.Backend.X11)
        {
            return null;
        }

        using X11Display? display = X11Display.TryOpen();
        PixelBuffer? icon = display != null ? ParseNetWmIcon(display.GetLongProperty((nuint)windowHandle, "_NET_WM_ICON", 1 << 20), 32) : null;
        return icon != null ? PngCodec.Encode(icon) : null;
    }

    public bool SetTopMost(long windowHandle, bool topMost)
    {
        switch (Backend)
        {
            case LinuxWindowService.Backend.X11:
                using (X11Display? display = X11Display.TryOpen())
                {
                    return display != null && display.ChangeWmState((nuint)windowHandle, topMost ? 1 : 0, "_NET_WM_STATE_ABOVE");
                }
            case LinuxWindowService.Backend.Hyprland:
                // Hyprland pins floating windows (above everything, on every workspace); "pin" toggles.
                WindowDetails? details = GetDetails(windowHandle);
                return details?.IsTopMost == null || details.IsTopMost == topMost ||
                    Run("hyprctl", ["dispatch", "pin", LinuxWindowService.FormatHyprlandAddress(windowHandle)]);
            default:
                return false;
        }
    }

    public bool SetOpacity(long windowHandle, byte opacity)
    {
        if (Backend != LinuxWindowService.Backend.X11)
        {
            return false;
        }

        // Compositing managers (picom, KWin, Mutter) read _NET_WM_WINDOW_OPACITY; fully opaque removes it.
        using X11Display? display = X11Display.TryOpen();
        return display != null && display.SetCardinalProperty((nuint)windowHandle, "_NET_WM_WINDOW_OPACITY", (uint)(opacity * 0x01010101u));
    }

    public bool ToggleBorderless(long windowHandle, bool useWorkingArea)
    {
        switch (Backend)
        {
            case LinuxWindowService.Backend.X11:
                using (X11Display? display = X11Display.TryOpen())
                {
                    return display != null && (useWorkingArea
                        ? display.ChangeWmState((nuint)windowHandle, 2, "_NET_WM_STATE_MAXIMIZED_VERT", "_NET_WM_STATE_MAXIMIZED_HORZ")
                        : display.ChangeWmState((nuint)windowHandle, 2, "_NET_WM_STATE_FULLSCREEN"));
                }
            case LinuxWindowService.Backend.Hyprland:
                // Fullscreen mode 1 keeps the bar and gaps (the working area); mode 0 covers the screen.
                return Run("hyprctl", ["--batch",
                    $"dispatch focuswindow {LinuxWindowService.FormatHyprlandAddress(windowHandle)}; dispatch fullscreen {(useWorkingArea ? 1 : 0)}"]);
            case LinuxWindowService.Backend.Sway:
                return Run("swaymsg", [string.Create(CultureInfo.InvariantCulture, $"[con_id={windowHandle}]"), "fullscreen", "toggle"]);
            default:
                return false;
        }
    }

    private WindowDetails? GetX11Details(long windowHandle)
    {
        PlatformWindow? window = windows.GetWindows().FirstOrDefault(w => w.Handle == windowHandle);
        using X11Display? display = X11Display.TryOpen();

        if (window == null || display == null)
        {
            return null;
        }

        nuint handle = (nuint)windowHandle;
        // WM_CLASS holds "instance\0class\0"; the class is the familiar name.
        string? className = display.GetStringProperty(handle, "WM_CLASS")?.Split('\0', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        List<string> states = display.GetLongProperty(handle, "_NET_WM_STATE").Select(display.GetAtomName).OfType<string>().ToList();
        states.AddRange(display.GetLongProperty(handle, "_NET_WM_WINDOW_TYPE").Select(display.GetAtomName).OfType<string>());
        nuint[] opacity = display.GetLongProperty(handle, "_NET_WM_WINDOW_OPACITY", 1);

        return new WindowDetails(windowHandle, window.Title, className, window.ProcessName, GetProcessPath(window.ProcessId), window.ProcessId,
            window.Bounds, ToClientCoordinates(display.GetWindowBounds(handle, includeFrame: false)), states, [],
            states.Contains("_NET_WM_STATE_ABOVE"), opacity.Length == 1 ? (byte)((uint)opacity[0] >> 24) : (byte)255);
    }

    private JsonElement? FindHyprlandClient(long windowHandle)
    {
        try
        {
            CommandResult result = runner.RunAsync("hyprctl", ["clients", "-j"], timeout: TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

            if (!result.Success)
            {
                return null;
            }

            using JsonDocument document = JsonDocument.Parse(result.StandardOutput);
            string address = "0x" + windowHandle.ToString("x", CultureInfo.InvariantCulture);

            foreach (JsonElement client in document.RootElement.EnumerateArray())
            {
                if (client.TryGetProperty("address", out JsonElement value) && value.GetString() == address)
                {
                    return client.Clone();
                }
            }
        }
        catch (Exception e) when (e is JsonException or TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException)
        {
        }

        return null;
    }

    internal static WindowDetails ParseHyprlandDetails(JsonElement client)
    {
        IReadOnlyList<PlatformWindow> parsed = LinuxWindowService.ParseHyprlandClients(client);
        PlatformWindow window = parsed[0];
        bool floating = client.TryGetProperty("floating", out JsonElement f) && f.ValueKind == JsonValueKind.True;
        bool pinned = client.TryGetProperty("pinned", out JsonElement p) && p.ValueKind == JsonValueKind.True;
        List<string> states = [floating ? "floating" : "tiled"];

        if (pinned) states.Add("pinned");
        if (client.TryGetProperty("fullscreen", out JsonElement fullscreen) && fullscreen.ValueKind == JsonValueKind.Number && fullscreen.GetInt32() != 0) states.Add("fullscreen");
        if (client.TryGetProperty("xwayland", out JsonElement x) && x.ValueKind == JsonValueKind.True) states.Add("xwayland");
        if (client.TryGetProperty("workspace", out JsonElement workspace) && workspace.TryGetProperty("name", out JsonElement name)) states.Add("workspace " + name.GetString());

        string? className = client.TryGetProperty("class", out JsonElement c) ? c.GetString() : null;

        // Only floating windows can be pinned above the others; opacity is not readable through hyprctl.
        return new WindowDetails(window.Handle, window.Title, className, window.ProcessName, GetProcessPath(window.ProcessId), window.ProcessId,
            window.Bounds, null, states, [], floating ? pinned : null, null);
    }

    /// <summary>WindowDetails.ClientBounds is in the client area's own coordinates, like GetClientRect.</summary>
    internal static PlatformRectangle? ToClientCoordinates(PlatformRectangle? client) =>
        client is PlatformRectangle rectangle ? new PlatformRectangle(0, 0, rectangle.Width, rectangle.Height) : null;

    /// <summary>
    /// _NET_WM_ICON holds one or more images, each a width, a height and width * height ARGB values (one per C long). Picks the
    /// smallest image at least <paramref name="preferredSize"/> wide, or the largest.
    /// </summary>
    internal static PixelBuffer? ParseNetWmIcon(nuint[] data, int preferredSize)
    {
        (int Offset, int Width, int Height)? best = null;

        for (int offset = 0; offset + 2 <= data.Length;)
        {
            int width = (int)data[offset], height = (int)data[offset + 1];
            long length = (long)width * height;

            if (width <= 0 || height <= 0 || offset + 2 + length > data.Length)
            {
                break;
            }

            bool better = best == null ||
                (width >= preferredSize ? best.Value.Width < preferredSize || width < best.Value.Width : width > best.Value.Width);

            if (better)
            {
                best = (offset + 2, width, height);
            }

            offset += 2 + (int)length;
        }

        if (best == null)
        {
            return null;
        }

        PixelBuffer image = new PixelBuffer(best.Value.Width, best.Value.Height);

        for (int i = 0; i < best.Value.Width * best.Value.Height; i++)
        {
            // 0xAARRGGBB stored little endian is B, G, R, A: the PixelBuffer layout.
            BitConverter.TryWriteBytes(image.Pixels.AsSpan(i * 4), (uint)data[best.Value.Offset + i]);
        }

        return image;
    }

    private static string? GetProcessPath(int? processId)
    {
        try
        {
            return processId is int pid ? new FileInfo($"/proc/{pid.ToString(CultureInfo.InvariantCulture)}/exe").ResolveLinkTarget(false)?.FullName : null;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return null;
        }
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
}
