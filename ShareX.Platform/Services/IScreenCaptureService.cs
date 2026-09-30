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
    Interactive
}

public sealed record ScreenCaptureRequest
{
    public ScreenCaptureMode Mode { get; init; } = ScreenCaptureMode.FullScreen;

    public PlatformRectangle Region { get; init; }

    public string? ScreenId { get; init; }

    public bool IncludeCursor { get; init; }

    public static ScreenCaptureRequest FullScreen(bool includeCursor = false) => new ScreenCaptureRequest { Mode = ScreenCaptureMode.FullScreen, IncludeCursor = includeCursor };

    public static ScreenCaptureRequest ForRegion(PlatformRectangle region, bool includeCursor = false) => new ScreenCaptureRequest { Mode = ScreenCaptureMode.Region, Region = region, IncludeCursor = includeCursor };

    public static ScreenCaptureRequest ForScreen(string screenId, bool includeCursor = false) => new ScreenCaptureRequest { Mode = ScreenCaptureMode.Screen, ScreenId = screenId, IncludeCursor = includeCursor };
}

/// <param name="Png">The captured image encoded as PNG.</param>
/// <param name="Bounds">The area of the virtual desktop the image covers.</param>
/// <param name="Backend">The mechanism used, for example "GDI", "screencapture", "xdg-desktop-portal" or "grim". Useful for diagnostics.</param>
public sealed record ScreenCaptureResult(byte[] Png, PlatformRectangle Bounds, string Backend);

/// <summary>Still image capture: GDI on Windows, CoreGraphics on macOS, X11 or Wayland portals on Linux.</summary>
public interface IScreenCaptureService
{
    FeatureSupport Support { get; }

    /// <summary>macOS Screen Recording permission, or <see cref="PermissionState.NotRequired"/> elsewhere.</summary>
    PermissionState GetPermissionState();

    /// <summary>Asks the OS to show its permission prompt. Returns true when access is granted.</summary>
    bool RequestPermission();

    IReadOnlyList<ScreenInfo> GetScreens();

    Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, CancellationToken cancellationToken = default);
}
