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
/// Screen capture on Linux. X11 reads the root window directly. Wayland uses grim on wlroots compositors (sway, Hyprland)
/// and the xdg-desktop-portal Screenshot interface on GNOME, KDE Plasma and in Flatpak.
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
    private readonly ICommandRunner runner;
    private readonly IWindowService? windows;

    /// <param name="windows">Locates windows for <see cref="ScreenCaptureMode.Window"/>; window capture is unavailable without it.</param>
    public LinuxScreenCaptureService(PlatformInfo info, ICommandRunner runner, IWindowService? windows = null)
    {
        this.info = info;
        this.runner = runner;
        this.windows = windows;
        ActiveBackend = SelectBackend(info, runner.Exists("grim"), DBusSession.IsAvailable);
    }

    internal Backend ActiveBackend { get; }

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

    public IReadOnlyList<ScreenInfo> GetScreens()
    {
        if (info.IsX11 || ActiveBackend == Backend.X11)
        {
            using X11Display? display = X11Display.TryOpen();

            if (display != null)
            {
                return display.GetMonitors();
            }
        }

        if (info.DesktopEnvironment == DesktopEnvironment.Hyprland)
        {
            return RunJson("hyprctl", ["monitors", "-j"], ParseHyprlandMonitors);
        }

        if (info.DesktopEnvironment == DesktopEnvironment.Sway)
        {
            return RunJson("swaymsg", ["-t", "get_outputs", "-r"], ParseSwayOutputs);
        }

        // GNOME and KDE do not expose output layout to plain Wayland clients. Callers fall back to the UI toolkit's screen list.
        return Array.Empty<ScreenInfo>();
    }

    public async Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Mode == ScreenCaptureMode.Window)
        {
            request = ToRegionRequest(request);
        }

        switch (ActiveBackend)
        {
            case Backend.X11:
                return CaptureX11(request);
            case Backend.Grim:
                return await CaptureGrimAsync(request, cancellationToken).ConfigureAwait(false);
            case Backend.Portal:
                return await CapturePortalAsync(request, cancellationToken).ConfigureAwait(false);
            default:
                throw new PlatformNotSupportedException(Support.Reason);
        }
    }

    /// <summary>XFixes on X11. Wayland compositors draw the cursor themselves and do not give it to clients.</summary>
    public CursorCapture? CaptureCursor()
    {
        if (ActiveBackend != Backend.X11)
        {
            return null;
        }

        using X11Display? display = X11Display.TryOpen();
        return display?.GetCursorImage() is (PixelBuffer image, PlatformPoint position) ? new CursorCapture(image, position) : null;
    }

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

    private ScreenCaptureResult CaptureX11(ScreenCaptureRequest request)
    {
        using X11Display display = X11Display.TryOpen() ?? throw new InvalidOperationException("Cannot open the X11 display.");
        PlatformRectangle area = ResolveArea(request, display.GetMonitors(), display.GetRootBounds());
        PixelBuffer pixels = display.CaptureRoot(area, request.IncludeCursor);
        return new ScreenCaptureResult(pixels, area.Intersect(display.GetRootBounds()), "X11");
    }

    private async Task<ScreenCaptureResult> CaptureGrimAsync(ScreenCaptureRequest request, CancellationToken cancellationToken)
    {
        List<string> arguments = new List<string>();

        if (request.IncludeCursor)
        {
            arguments.Add("-c");
        }

        PlatformRectangle bounds = PlatformRectangle.Empty;

        switch (request.Mode)
        {
            case ScreenCaptureMode.Region:
                bounds = ClipToScreens(request.Region, GetScreens());
                arguments.Add("-g");
                arguments.Add(FormatGeometry(bounds));
                break;
            case ScreenCaptureMode.Screen when request.ScreenId != null:
                arguments.Add("-o");
                arguments.Add(request.ScreenId);
                bounds = GetScreens().FirstOrDefault(s => s.Id == request.ScreenId)?.Bounds ?? PlatformRectangle.Empty;
                break;
            case ScreenCaptureMode.Interactive when runner.Exists("slurp"):
                CommandResult selection = await runner.RunAsync("slurp", [], timeout: TimeSpan.FromMinutes(5), cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!selection.Success)
                {
                    throw new OperationCanceledException("The selection was cancelled.");
                }

                string geometry = selection.StandardOutputText.Trim();
                bounds = ParseGeometry(geometry);
                arguments.Add("-g");
                arguments.Add(geometry);
                break;
        }

        // "-" writes the PNG to standard output.
        arguments.Add("-t");
        arguments.Add("png");
        arguments.Add("-");

        CommandResult result = await runner.RunAsync("grim", arguments, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!result.Success || !PngCodec.IsPng(result.StandardOutput))
        {
            throw new InvalidOperationException($"grim failed: {result.StandardError.Trim()}");
        }

        if (bounds.IsEmpty)
        {
            PlatformSize size = PngCodec.ReadSize(result.StandardOutput);
            bounds = new PlatformRectangle(0, 0, size.Width, size.Height);
        }

        return new ScreenCaptureResult(result.StandardOutput, bounds, "grim");
    }

    private async Task<ScreenCaptureResult> CapturePortalAsync(ScreenCaptureRequest request, CancellationToken cancellationToken)
    {
        bool interactive = request.Mode == ScreenCaptureMode.Interactive;
        byte[] png = await PortalScreenshot.CaptureAsync(interactive, cancellationToken).ConfigureAwait(false);
        PlatformSize size = PngCodec.ReadSize(png);
        PlatformRectangle full = new PlatformRectangle(0, 0, size.Width, size.Height);

        if (interactive || request.Mode == ScreenCaptureMode.FullScreen)
        {
            return new ScreenCaptureResult(png, full, "xdg-desktop-portal");
        }

        // The portal always returns the whole desktop, so crop for region and single screen requests.
        PlatformRectangle area = ResolveArea(request, GetScreens(), full);
        PlatformRectangle crop = area.Intersect(full);

        if (crop.IsEmpty || crop == full)
        {
            return new ScreenCaptureResult(png, full, "xdg-desktop-portal");
        }

        return new ScreenCaptureResult(PngCodec.Crop(png, crop), crop, "xdg-desktop-portal");
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

    private IReadOnlyList<ScreenInfo> RunJson(string command, IReadOnlyList<string> arguments, Func<JsonElement, IReadOnlyList<ScreenInfo>> parse)
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
        catch (Exception e) when (e is JsonException or TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException or KeyNotFoundException)
        {
        }

        return Array.Empty<ScreenInfo>();
    }
}
