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
    /// <summary>
    /// A helper that produces the input FFmpeg reads, such as wf-recorder writing into a pipe on Wayland. Null when FFmpeg captures
    /// the screen itself. The recorder starts it right before FFmpeg and disposes it when FFmpeg has finished.
    /// </summary>
    public IScreenRecordingSource? Source { get; init; }

    public override string ToString() => InputArguments;
}

/// <summary>Device actions the recording settings offer besides recording itself.</summary>
public enum RecordingDeviceAction
{
    /// <summary>Listing DirectShow video and audio sources through FFmpeg (-list_devices, dshow).</summary>
    ListDirectShowDevices,
    /// <summary>Downloading and setting up the screen-capture-recorder and virtual-audio-capturer DirectShow devices.</summary>
    InstallRecorderDevices
}

/// <summary>A helper process that feeds the screen to FFmpeg.</summary>
public interface IScreenRecordingSource : System.IDisposable
{
    /// <summary>Starts the helper. Called once, right before FFmpeg starts reading.</summary>
    void Start();
}

/// <summary>Screen recording through FFmpeg: gdigrab/ddagrab on Windows, avfoundation on macOS, x11grab on Linux.</summary>
public interface IScreenRecordingService
{
    FeatureSupport Support { get; }

    /// <summary>
    /// Whether a device action of the recording settings works here. Independent of <see cref="Support"/>: Linux can record
    /// through its own devices while DirectShow, which only exists on Windows, is unsupported.
    /// </summary>
    FeatureSupport GetDeviceActionSupport(RecordingDeviceAction action);

    /// <summary>FFmpeg input devices usable on this platform, most preferred first.</summary>
    IReadOnlyList<string> GetSupportedDevices();

    FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request);

    /// <summary>
    /// Configures an FFmpeg input device that reads the capture area from system settings instead of its arguments
    /// (the screen-capture-recorder DirectShow filter on Windows). Does nothing for every other device.
    /// </summary>
    void PrepareDevice(string device, ScreenRecordingRequest request);

    /// <summary>
    /// The FFmpeg program to use when the user has not chosen one: ffmpeg.exe next to ShareX on Windows (which ShareX downloads),
    /// an ffmpeg next to ShareX or the one installed with the system on Linux and macOS.
    /// </summary>
    string GetDefaultFFmpegPath(string applicationDirectory);
}
