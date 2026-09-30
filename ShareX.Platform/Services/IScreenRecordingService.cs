using System.Collections.Generic;

namespace ShareX.Platform;

public sealed record ScreenRecordingRequest
{
    /// <summary>Area to record on the virtual desktop. Empty records the whole primary screen.</summary>
    public PlatformRectangle Region { get; init; }

    public int FrameRate { get; init; } = 30;

    public bool DrawCursor { get; init; } = true;

    /// <summary>Optional screen to record, as returned by <see cref="IScreenCaptureService.GetScreens"/>.</summary>
    public ScreenInfo? Screen { get; init; }
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
