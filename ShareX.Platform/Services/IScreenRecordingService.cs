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

namespace ShareX.Platform;

public sealed record ScreenRecordingRequest
{
    /// <summary>Area to record on the virtual desktop. Empty records the whole primary screen.</summary>
    public PlatformRectangle Region { get; init; }

    public int FrameRate { get; init; } = 30;

    public bool DrawCursor { get; init; } = true;

    /// <summary>Round the size down to even numbers, which yuv420p encoders such as libx264 require.</summary>
    public bool RequireEvenSize { get; init; } = true;

    /// <summary>Optional screen to record, as returned by <see cref="IScreenCaptureService.GetScreens"/>.</summary>
    public ScreenInfo? Screen { get; init; }
}

public static class ScreenRecordingRequestExtensions
{
    /// <summary>The region to record with <see cref="ScreenRecordingRequest.RequireEvenSize"/> applied.</summary>
    public static PlatformRectangle GetEffectiveRegion(this ScreenRecordingRequest request, PlatformRectangle fallback)
    {
        PlatformRectangle region = request.Region.IsEmpty ? request.Screen?.Bounds ?? fallback : request.Region;

        if (request.RequireEvenSize)
        {
            region = region with { Width = region.Width & ~1, Height = region.Height & ~1 };
        }

        return region;
    }
}

/// <summary>FFmpeg arguments describing the video input for the current platform.</summary>
/// <param name="Device">The FFmpeg input device, for example gdigrab, avfoundation or x11grab.</param>
/// <param name="InputArguments">Arguments placed before and including -i.</param>
/// <param name="VideoFilters">Filters that must be applied to the input, for example a crop on macOS. Empty when none are needed.</param>
public sealed record FFmpegVideoInput(string Device, string InputArguments, IReadOnlyList<string> VideoFilters)
{
    public override string ToString() => InputArguments;
}

/// <summary>Screen recording through FFmpeg: gdigrab/ddagrab on Windows, avfoundation on macOS, x11grab on Linux.</summary>
public interface IScreenRecordingService
{
    FeatureSupport Support { get; }

    /// <summary>FFmpeg input devices usable on this platform, most preferred first.</summary>
    IReadOnlyList<string> GetSupportedDevices();

    FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request);
}
