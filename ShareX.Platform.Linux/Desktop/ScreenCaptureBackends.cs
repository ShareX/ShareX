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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>One way of taking a screenshot on Linux. Window requests arrive already turned into regions.</summary>
internal interface IScreenCaptureBackend
{
    LinuxScreenCaptureService.Backend Kind { get; }

    Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, IReadOnlyList<ScreenInfo> screens, CancellationToken cancellationToken);

    /// <summary>The pointer image and position, where the session reveals it.</summary>
    CursorCapture? CaptureCursor();
}

/// <summary>X11: reads the root window, cursor through XFixes.</summary>
internal sealed class X11CaptureBackend : IScreenCaptureBackend
{
    public LinuxScreenCaptureService.Backend Kind => LinuxScreenCaptureService.Backend.X11;

    public Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, IReadOnlyList<ScreenInfo> screens, CancellationToken cancellationToken)
    {
        using X11Display display = X11Display.TryOpen() ?? throw new InvalidOperationException("Cannot open the X11 display.");
        PlatformRectangle area = LinuxScreenCaptureService.ResolveArea(request, display.GetMonitors(), display.GetRootBounds());
        PixelBuffer pixels = display.CaptureRoot(area, request.IncludeCursor);
        return Task.FromResult(new ScreenCaptureResult(pixels, area.Intersect(display.GetRootBounds()), "X11"));
    }

    public CursorCapture? CaptureCursor()
    {
        using X11Display? display = X11Display.TryOpen();
        return display?.GetCursorImage() is (PixelBuffer image, PlatformPoint position) ? new CursorCapture(image, position) : null;
    }
}

/// <summary>Hyprland, sway and other wlroots compositors: grim (wlr-screencopy) with regions in layout coordinates.</summary>
internal sealed class GrimCaptureBackend(ICommandRunner runner) : IScreenCaptureBackend
{
    public LinuxScreenCaptureService.Backend Kind => LinuxScreenCaptureService.Backend.Grim;

    public async Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, IReadOnlyList<ScreenInfo> screens, CancellationToken cancellationToken)
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
                bounds = LinuxScreenCaptureService.ClipToScreens(request.Region, screens);
                arguments.Add("-g");
                arguments.Add(LinuxScreenCaptureService.FormatGeometry(bounds));
                break;
            case ScreenCaptureMode.Screen when request.ScreenId != null:
                arguments.Add("-o");
                arguments.Add(request.ScreenId);
                bounds = screens.FirstOrDefault(s => s.Id == request.ScreenId)?.Bounds ?? PlatformRectangle.Empty;
                break;
            case ScreenCaptureMode.Interactive when runner.Exists("slurp"):
                CommandResult selection = await runner.RunAsync("slurp", [], timeout: TimeSpan.FromMinutes(5), cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!selection.Success)
                {
                    throw new OperationCanceledException("The selection was cancelled.");
                }

                string geometry = selection.StandardOutputText.Trim();
                bounds = LinuxScreenCaptureService.ParseGeometry(geometry);
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

    /// <summary>Wayland compositors draw the cursor themselves and do not give it to clients.</summary>
    public CursorCapture? CaptureCursor() => null;
}

/// <summary>GNOME, KDE Plasma and sandboxes: the xdg-desktop-portal Screenshot interface, which returns the whole desktop.</summary>
internal sealed class PortalCaptureBackend : IScreenCaptureBackend
{
    public LinuxScreenCaptureService.Backend Kind => LinuxScreenCaptureService.Backend.Portal;

    public async Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, IReadOnlyList<ScreenInfo> screens, CancellationToken cancellationToken)
    {
        bool interactive = request.Mode == ScreenCaptureMode.Interactive;
        byte[] png = await PortalScreenshot.CaptureAsync(interactive, cancellationToken).ConfigureAwait(false);
        PlatformSize size = PngCodec.ReadSize(png);
        PlatformRectangle full = new PlatformRectangle(0, 0, size.Width, size.Height);

        if (interactive || request.Mode == ScreenCaptureMode.FullScreen)
        {
            return new ScreenCaptureResult(png, full, "xdg-desktop-portal");
        }

        // The portal always returns the whole desktop in pixels, so crop for region, window and single screen requests.
        PlatformRectangle layout = screens.Count > 0 ? screens.Select(s => s.Bounds).Aggregate((a, b) => a.Union(b)) : full;
        PlatformRectangle area = LinuxScreenCaptureService.ResolveArea(request, screens, layout).Intersect(layout);
        PlatformRectangle crop = MapToPixels(area, layout, size);

        if (area.IsEmpty || crop.IsEmpty || crop == full)
        {
            return new ScreenCaptureResult(png, full, "xdg-desktop-portal");
        }

        // Like grim, report the area in layout coordinates.
        return new ScreenCaptureResult(PngCodec.Crop(png, crop), area, "xdg-desktop-portal");
    }

    /// <summary>
    /// Maps an area in layout (logical) coordinates to pixels of a screenshot of the whole layout. A scaled desktop, for example
    /// 3840x2160 pixels shown as 3072x1728 at 125%, gives a larger image than its layout. Assumes one scale for every monitor.
    /// </summary>
    internal static PlatformRectangle MapToPixels(PlatformRectangle area, PlatformRectangle layout, PlatformSize pixels)
    {
        if (layout.Width <= 0 || layout.Height <= 0)
        {
            return area;
        }

        double scaleX = pixels.Width / (double)layout.Width;
        double scaleY = pixels.Height / (double)layout.Height;
        int left = (int)Math.Round((area.X - layout.X) * scaleX);
        int top = (int)Math.Round((area.Y - layout.Y) * scaleY);
        int right = (int)Math.Round((area.X + area.Width - layout.X) * scaleX);
        int bottom = (int)Math.Round((area.Y + area.Height - layout.Y) * scaleY);
        return new PlatformRectangle(left, top, right - left, bottom - top).Intersect(new PlatformRectangle(0, 0, pixels.Width, pixels.Height));
    }

    public CursorCapture? CaptureCursor() => null;
}
