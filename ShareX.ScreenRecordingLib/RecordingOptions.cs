// SPDX-License-Identifier: GPL-3.0-or-later
using System.Drawing;

namespace ShareX.ScreenRecordingLib;

/// <summary>Native H.264/AAC MP4 recording. Coordinates are physical desktop pixels.</summary>
public sealed record RecordingOptions
{
    public required string OutputPath { get; init; }
    public Rectangle Region { get; init; }
    /// <summary>When nonzero, capture this window (including when occluded) instead of Region.</summary>
    public nint WindowHandle { get; init; }
    public int FramesPerSecond { get; init; } = 30;
    public int VideoBitrate { get; init; } = 8_000_000;
    public int AudioBitrate { get; init; } = 192_000;
    public bool IncludeCursor { get; init; } = true;
    public bool CaptureSystemAudio { get; init; } = true;
    public bool CaptureMicrophone { get; init; }
    /// <summary>WASAPI endpoint ID; null selects the default multimedia render endpoint.</summary>
    public string? SystemAudioDeviceId { get; init; }
    /// <summary>WASAPI endpoint ID; null selects the default communications capture endpoint.</summary>
    public string? MicrophoneDeviceId { get; init; }
    public float SystemAudioGain { get; init; } = 1;
    public float MicrophoneGain { get; init; } = 1;
    /// <summary>Reject a software video encoder rather than silently increasing CPU usage.</summary>
    public bool RequireHardwareEncoder { get; init; } = true;
    public TimeSpan Duration { get; init; }

    internal bool HasAudio => CaptureSystemAudio || CaptureMicrophone;

    internal void Validate()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(OutputPath);
        if (!Path.GetExtension(OutputPath).Equals(".mp4", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Native recording writes H.264/AAC MP4 files.", nameof(OutputPath));
        if (WindowHandle == 0 && (Region.Width < 2 || Region.Height < 2))
            throw new ArgumentException("Select a window or a region of at least 2 × 2 pixels.", nameof(Region));
        if (FramesPerSecond is < 1 or > 240) throw new ArgumentOutOfRangeException(nameof(FramesPerSecond));
        if (VideoBitrate is < 100_000 or > 200_000_000) throw new ArgumentOutOfRangeException(nameof(VideoBitrate));
        if (AudioBitrate is not (96_000 or 128_000 or 160_000 or 192_000)) throw new ArgumentOutOfRangeException(nameof(AudioBitrate));
        if (Duration < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(Duration));
        if (!float.IsFinite(SystemAudioGain) || SystemAudioGain is < 0 or > 4) throw new ArgumentOutOfRangeException(nameof(SystemAudioGain));
        if (!float.IsFinite(MicrophoneGain) || MicrophoneGain is < 0 or > 4) throw new ArgumentOutOfRangeException(nameof(MicrophoneGain));
    }
}

public sealed record VideoEncoderInfo(string Name, bool IsHardwareAccelerated, bool IsD3D11Aware);

public sealed record RecordingResult(string OutputPath, TimeSpan Duration, long VideoFrames,
    long DroppedVideoFrames, long AudioDiscontinuities, VideoEncoderInfo Encoder);