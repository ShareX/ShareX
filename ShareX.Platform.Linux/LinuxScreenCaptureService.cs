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
using ShareX.Platform.Linux.DBus;
using ShareX.Platform.Linux.Desktop;
using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux;

/// <summary>
/// Screen capture on Linux through the backend <see cref="LinuxDesktop"/> chooses: the X11 root window on X11, grim on wlroots
/// compositors (Hyprland, sway), and the xdg-desktop-portal Screenshot interface on GNOME, KDE Plasma and in sandboxes. The
/// monitor layout comes from the desktop's window backend.
/// </summary>
public sealed class LinuxScreenCaptureService : IScreenCaptureService
{
    internal enum Backend
    {
        None,
        X11,
        Grim,
        Portal
    }

    private readonly PlatformInfo info;
    private readonly IWindowService? windows;
    private readonly IScreenCaptureBackend? backend;
    private readonly IDesktopWindowBackend layout;

    /// <param name="windows">Locates windows for <see cref="ScreenCaptureMode.Window"/>; window capture is unavailable without it.</param>
    public LinuxScreenCaptureService(PlatformInfo info, ICommandRunner runner, IWindowService? windows = null)
    {
        this.info = info;
        this.windows = windows;
        LinuxDesktop desktop = new LinuxDesktop(info);
        backend = desktop.CreateCaptureBackend(runner);
        layout = (windows as LinuxWindowService)?.DesktopBackend ?? desktop.CreateWindowBackend(runner);
    }

    internal Backend ActiveBackend => backend?.Kind ?? Backend.None;

    public FeatureSupport Support => ActiveBackend switch
    {
        Backend.None when info.IsWlrootsCompositor => LinuxPackages.Missing(info.Distribution ?? LinuxDistribution.Unknown, LinuxTool.Grim),
        Backend.None when info.DisplayServer == DisplayServer.None => FeatureSupport.NotSupported("No graphical session was found."),
        Backend.None => LinuxPackages.Missing(info.Distribution ?? LinuxDistribution.Unknown, LinuxTool.XdgDesktopPortal),
        _ => FeatureSupport.Supported
    };

    /// <summary>
    /// Window capture works wherever ShareX can list windows and capture a region (X11, Hyprland, sway): it captures the window's
    /// rectangle as it is on screen. Transparency, client area only and the other Windows extras are not available.
    /// </summary>
    public ScreenCaptureFeatures Features =>
        windows != null && windows.Support.IsSupported && ActiveBackend is Backend.X11 or Backend.Grim ? ScreenCaptureFeatures.Window : ScreenCaptureFeatures.None;

    public FeatureSupport GetFeatureSupport(ScreenCaptureFeatures feature)
    {
        if (!Support.IsSupported || (Features & feature) == feature)
        {
            return Support;
        }

        return feature switch
        {
            ScreenCaptureFeatures.Window => FeatureSupport.NotSupported(info.IsWayland
                ? "This Wayland desktop does not let applications find or capture a single window. Capture a region instead."
                : "Window capture is not available in this session."),
            ScreenCaptureFeatures.WindowClientArea => FeatureSupport.NotSupported("On Linux windows are captured with their frame."),
            ScreenCaptureFeatures.TransparentWindow => FeatureSupport.NotSupported("Transparent corners and window shadows can only be captured on Windows."),
            ScreenCaptureFeatures.HideTaskbar => FeatureSupport.NotSupported("Hiding the task bar during capture is only available on Windows."),
            ScreenCaptureFeatures.HdrToneMapping => FeatureSupport.NotSupported("HDR capture is only available on Windows."),
            _ => FeatureSupport.NotSupported("This capture option is not available on this system.")
        };
    }

    internal static Backend SelectBackend(PlatformInfo info, bool grimAvailable, bool sessionBusAvailable)
    {
        if (info.IsX11 && !info.IsSandboxed)
        {
            return Backend.X11;
        }

        if (info.IsWayland)
        {
            if (info.IsWlrootsCompositor && grimAvailable && !info.IsSandboxed)
            {
                return Backend.Grim;
            }

            return sessionBusAvailable ? Backend.Portal : Backend.None;
        }

        // Sandboxed X11 sessions still work through the portal.
        return info.IsX11 && sessionBusAvailable ? Backend.Portal : Backend.None;
    }

    // Linux has no screen recording permission gate outside the portal, which asks on each request.
    public PermissionState GetPermissionState() => PermissionState.NotRequired;

    public bool RequestPermission() => true;

    public IReadOnlyList<ScreenInfo> GetScreens() => layout.GetScreens();

    public async Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, CancellationToken cancellationToken = default)
    {
        if (backend == null)
        {
            throw new PlatformNotSupportedException(Support.Reason);
        }

        if (request.Mode == ScreenCaptureMode.Window)
        {
            request = ToRegionRequest(request);
        }

        return await backend.CaptureAsync(request, GetScreens(), cancellationToken).ConfigureAwait(false);
    }

    public CursorCapture? CaptureCursor() => backend?.CaptureCursor();

    private ScreenCaptureRequest ToRegionRequest(ScreenCaptureRequest request)
    {
        if ((Features & ScreenCaptureFeatures.Window) == 0)
        {
            throw new PlatformNotSupportedException(windows?.Support.Reason ?? "Window capture is not available in this session.");
        }

        PlatformWindow window = windows!.GetWindows().FirstOrDefault(w => w.Handle == request.WindowHandle)
            ?? throw new ArgumentException("The window was not found. It may have closed.", nameof(request));

        return request with { Mode = ScreenCaptureMode.Region, Region = window.Bounds };
    }

    internal static PlatformRectangle ResolveArea(ScreenCaptureRequest request, IReadOnlyList<ScreenInfo> screens, PlatformRectangle desktop)
    {
        switch (request.Mode)
        {
            case ScreenCaptureMode.Region:
                return request.Region;
            case ScreenCaptureMode.Screen:
                ScreenInfo? screen = screens.FirstOrDefault(s => s.Id == request.ScreenId);
                return screen?.Bounds ?? throw new ArgumentException($"Unknown screen '{request.ScreenId}'.", nameof(request));
            default:
                return screens.Count > 0 ? screens.Select(s => s.Bounds).Aggregate((a, b) => a.Union(b)) : desktop;
        }
    }

    internal static string FormatGeometry(PlatformRectangle area) =>
        string.Create(CultureInfo.InvariantCulture, $"{area.X},{area.Y} {area.Width}x{area.Height}");

    internal static PlatformRectangle ParseGeometry(string geometry)
    {
        // slurp and grim use "x,y wxh".
        string[] parts = geometry.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string[] position = parts[0].Split(',');
        string[] size = parts[1].Split('x');
        return new PlatformRectangle(int.Parse(position[0], CultureInfo.InvariantCulture), int.Parse(position[1], CultureInfo.InvariantCulture),
            int.Parse(size[0], CultureInfo.InvariantCulture), int.Parse(size[1], CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// The part of <paramref name="region"/> on the screens, so a window partly off screen captures its visible part. Throws
    /// ArgumentException, which callers treat as "nothing to capture", when it is on no screen. Unknown layouts pass through.
    /// </summary>
    internal static PlatformRectangle ClipToScreens(PlatformRectangle region, IReadOnlyList<ScreenInfo> screens)
    {
        if (screens.Count == 0)
        {
            return region;
        }

        PlatformRectangle desktop = screens.Select(s => s.Bounds).Aggregate((a, b) => a.Union(b));
        PlatformRectangle visible = region.Intersect(desktop);

        if (visible.IsEmpty)
        {
            throw new ArgumentException("The area is not on any screen.", nameof(region));
        }

        return visible;
    }

    internal static IReadOnlyList<ScreenInfo> ParseHyprlandMonitors(JsonElement root)
    {
        List<ScreenInfo> screens = new List<ScreenInfo>();

        foreach (JsonElement monitor in root.EnumerateArray())
        {
            if (monitor.TryGetProperty("disabled", out JsonElement disabled) && disabled.ValueKind == JsonValueKind.True)
            {
                continue;
            }

            string name = monitor.GetProperty("name").GetString() ?? "";
            double scale = monitor.TryGetProperty("scale", out JsonElement scaleElement) ? scaleElement.GetDouble() : 1;
            int x = monitor.GetProperty("x").GetInt32();
            int y = monitor.GetProperty("y").GetInt32();
            // Hyprland reports the mode in pixels and the position in layout (logical) coordinates.
            int width = (int)Math.Round(monitor.GetProperty("width").GetInt32() / scale);
            int height = (int)Math.Round(monitor.GetProperty("height").GetInt32() / scale);
            PlatformRectangle bounds = new PlatformRectangle(x, y, width, height);
            bool focused = monitor.TryGetProperty("focused", out JsonElement focusedElement) && focusedElement.ValueKind == JsonValueKind.True;
            string description = monitor.TryGetProperty("description", out JsonElement d) ? d.GetString() ?? name : name;
            screens.Add(new ScreenInfo(name, description, bounds, bounds, focused, scale));
        }

        return screens;
    }

    internal static IReadOnlyList<ScreenInfo> ParseSwayOutputs(JsonElement root)
    {
        List<ScreenInfo> screens = new List<ScreenInfo>();

        foreach (JsonElement output in root.EnumerateArray())
        {
            if (output.TryGetProperty("active", out JsonElement active) && active.ValueKind == JsonValueKind.False)
            {
                continue;
            }

            string name = output.GetProperty("name").GetString() ?? "";
            JsonElement rect = output.GetProperty("rect");
            PlatformRectangle bounds = new PlatformRectangle(rect.GetProperty("x").GetInt32(), rect.GetProperty("y").GetInt32(),
                rect.GetProperty("width").GetInt32(), rect.GetProperty("height").GetInt32());
            double scale = output.TryGetProperty("scale", out JsonElement s) && s.ValueKind == JsonValueKind.Number ? s.GetDouble() : 1;
            bool focused = output.TryGetProperty("focused", out JsonElement f) && f.ValueKind == JsonValueKind.True;
            string make = output.TryGetProperty("make", out JsonElement m) ? m.GetString() ?? "" : "";
            string model = output.TryGetProperty("model", out JsonElement mo) ? mo.GetString() ?? "" : "";
            string description = (make + " " + model).Trim();
            screens.Add(new ScreenInfo(name, description.Length > 0 ? description : name, bounds, bounds, focused, scale));
        }

        return screens;
    }
}
