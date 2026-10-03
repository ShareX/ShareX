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

using ShareX.Platform.Imaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform;

/// <summary>A physical display.</summary>
/// <param name="Bounds">Bounds on the virtual desktop in physical pixels.</param>
/// <param name="WorkingArea">Bounds minus task bars, docks and panels. Equal to <paramref name="Bounds"/> when unknown.</param>
/// <param name="ScaleFactor">Ratio of physical pixels to logical units, for example 2 on a Retina display.</param>
public sealed record ScreenInfo(string Id, string Name, PlatformRectangle Bounds, PlatformRectangle WorkingArea, bool IsPrimary, double ScaleFactor);

public enum ScreenCaptureMode
{
    /// <summary>All displays combined into one image.</summary>
    FullScreen,
    /// <summary>A rectangle on the virtual desktop.</summary>
    Region,
    /// <summary>A single display identified by <see cref="ScreenCaptureRequest.ScreenId"/>.</summary>
    Screen,
    /// <summary>Let the OS or compositor ask the user to choose (xdg-desktop-portal, screencapture -i).</summary>
    Interactive,
    /// <summary>The window identified by <see cref="ScreenCaptureRequest.WindowHandle"/>, as it appears on screen.</summary>
    Window
}

/// <summary>Capture features beyond plain screen and region capture. Read them from <see cref="IScreenCaptureService.Features"/> before offering the option.</summary>
[Flags]
public enum ScreenCaptureFeatures
{
    None = 0,
    /// <summary><see cref="ScreenCaptureMode.Window"/> works.</summary>
    Window = 1 << 0,
    /// <summary><see cref="WindowCaptureOptions.ClientAreaOnly"/> is honoured.</summary>
    WindowClientArea = 1 << 1,
    /// <summary><see cref="WindowCaptureOptions.Transparent"/> and <see cref="WindowCaptureOptions.IncludeShadow"/> are honoured.</summary>
    TransparentWindow = 1 << 2,
    /// <summary><see cref="WindowCaptureOptions.HideTaskbar"/> is honoured.</summary>
    HideTaskbar = 1 << 3,
    /// <summary><see cref="ScreenCaptureRequest.HdrToneMapping"/> is honoured.</summary>
    HdrToneMapping = 1 << 4
}

public sealed record WindowCaptureOptions
{
    /// <summary>Only the client area, without the title bar and borders.</summary>
    public bool ClientAreaOnly { get; init; }

    /// <summary>Keep the window's transparency (rounded corners, translucent areas) and crop to the window.</summary>
    public bool Transparent { get; init; }

    /// <summary>With <see cref="Transparent"/>, keep the drop shadow the window manager draws.</summary>
    public bool IncludeShadow { get; init; }

    /// <summary>Pixels of shadow to keep around the window with <see cref="IncludeShadow"/>.</summary>
    public int ShadowOffset { get; init; } = 20;

    /// <summary>Hide the task bar while capturing when it overlaps the window.</summary>
    public bool HideTaskbar { get; init; }
}

public sealed record ScreenCaptureRequest
{
    public ScreenCaptureMode Mode { get; init; } = ScreenCaptureMode.FullScreen;

    public PlatformRectangle Region { get; init; }

    public string? ScreenId { get; init; }

    public bool IncludeCursor { get; init; }

    /// <summary>The window to capture in <see cref="ScreenCaptureMode.Window"/>.</summary>
    public long WindowHandle { get; init; }

    public WindowCaptureOptions Window { get; init; } = new WindowCaptureOptions();

    /// <summary>Tone map HDR displays to SDR so the image matches what the user sees.</summary>
    public bool HdrToneMapping { get; init; }

    /// <summary>Clip the region to the screens. When false, areas outside every screen come back black.</summary>
    public bool ClipToScreens { get; init; } = true;

    public static ScreenCaptureRequest ForWindow(long windowHandle, bool includeCursor = false) => new ScreenCaptureRequest { Mode = ScreenCaptureMode.Window, WindowHandle = windowHandle, IncludeCursor = includeCursor };

    public static ScreenCaptureRequest FullScreen(bool includeCursor = false) => new ScreenCaptureRequest { Mode = ScreenCaptureMode.FullScreen, IncludeCursor = includeCursor };

    public static ScreenCaptureRequest ForRegion(PlatformRectangle region, bool includeCursor = false) => new ScreenCaptureRequest { Mode = ScreenCaptureMode.Region, Region = region, IncludeCursor = includeCursor };

    public static ScreenCaptureRequest ForScreen(string screenId, bool includeCursor = false) => new ScreenCaptureRequest { Mode = ScreenCaptureMode.Screen, ScreenId = screenId, IncludeCursor = includeCursor };
}

/// <summary>A captured image, kept as pixels when the backend produced pixels and as PNG when it produced a file.</summary>
public sealed class ScreenCaptureResult
{
    private byte[]? png;

    /// <param name="png">The captured image encoded as PNG.</param>
    /// <param name="bounds">The area of the virtual desktop the image covers.</param>
    /// <param name="backend">The mechanism used, for example "GDI", "screencapture", "xdg-desktop-portal" or "grim". Useful for diagnostics.</param>
    public ScreenCaptureResult(byte[] png, PlatformRectangle bounds, string backend)
    {
        this.png = png ?? throw new ArgumentNullException(nameof(png));
        Bounds = bounds;
        Backend = backend;
    }

    public ScreenCaptureResult(PixelBuffer pixels, PlatformRectangle bounds, string backend)
    {
        Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
        Bounds = bounds;
        Backend = backend;
    }

    /// <summary>Straight alpha BGRA pixels, when the backend captured pixels directly. Saves a PNG round trip.</summary>
    public PixelBuffer? Pixels { get; }

    /// <summary>The image encoded as PNG, encoded on first use when the backend captured pixels.</summary>
    public byte[] Png => png ??= PngCodec.Encode(Pixels!);

    public PlatformRectangle Bounds { get; }

    public string Backend { get; }
}

/// <summary>Still image capture: GDI on Windows, CoreGraphics on macOS, X11 or Wayland portals on Linux.</summary>
public interface IScreenCaptureService
{
    FeatureSupport Support { get; }

    /// <summary>Optional capture features this platform implements.</summary>
    ScreenCaptureFeatures Features { get; }

    /// <summary>macOS Screen Recording permission, or <see cref="PermissionState.NotRequired"/> elsewhere.</summary>
    PermissionState GetPermissionState();

    /// <summary>Asks the OS to show its permission prompt. Returns true when access is granted.</summary>
    bool RequestPermission();

    IReadOnlyList<ScreenInfo> GetScreens();

    Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// The mouse cursor as an image (straight alpha) and its top left corner on the virtual desktop, for drawing it onto a capture
    /// later. Null when it is hidden or the platform does not reveal it (Wayland).
    /// </summary>
    CursorCapture? CaptureCursor();
}

/// <param name="Position">Top left corner of <paramref name="Image"/> on the virtual desktop (the pointer position minus the hot spot).</param>
public sealed record CursorCapture(PixelBuffer Image, PlatformPoint Position);
