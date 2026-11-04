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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform;

public enum CameraOverlayPosition { TopLeft, TopRight, BottomLeft, BottomRight }

public enum CameraOverlayShape { Rectangle, Circle }

public enum CameraCaptureResolution { Size640x480, Size1280x720, Size1920x1080 }

/// <summary>An audio endpoint or camera the native recorder can capture from.</summary>
public sealed record CaptureDevice(string Id, string Name);

/// <summary>What a native recording captures and how it encodes it. Device ids are those from <see cref="INativeScreenRecorderService"/>.</summary>
public sealed record NativeRecordingRequest
{
    public required string OutputPath { get; init; }
    /// <summary>The screen area, in screen coordinates; ignored when <see cref="WindowHandle"/> is set.</summary>
    public PlatformRectangle Region { get; init; }
    /// <summary>A window to record on its own, or 0 to record <see cref="Region"/>.</summary>
    public long WindowHandle { get; init; }
    public int FramesPerSecond { get; init; } = 30;
    public int VideoBitrate { get; init; } = 8_000_000;
    public bool IncludeCursor { get; init; } = true;
    public bool CaptureSystemAudio { get; init; } = true;
    public string? SystemAudioDeviceId { get; init; }
    public float SystemAudioGain { get; init; } = 1;
    public bool CaptureMicrophone { get; init; }
    public string? MicrophoneDeviceId { get; init; }
    public float MicrophoneGain { get; init; } = 1;
    public bool CaptureCamera { get; init; }
    public string? CameraDeviceId { get; init; }
    public CameraCaptureResolution CameraResolution { get; init; } = CameraCaptureResolution.Size1280x720;
    public int CameraFramesPerSecond { get; init; } = 30;
    public CameraOverlayPosition CameraPosition { get; init; } = CameraOverlayPosition.BottomRight;
    public CameraOverlayShape CameraShape { get; init; } = CameraOverlayShape.Rectangle;
    public int CameraWidthPercent { get; init; } = 20;
    public int CameraMargin { get; init; } = 16;
    public bool RequireHardwareEncoder { get; init; } = true;
    /// <summary>Stop after this long, or zero to record until stopped.</summary>
    public TimeSpan Duration { get; init; }
}

public sealed record NativeRecordingResult(string EncoderName, bool IsHardwareAccelerated, long VideoFrames, long DroppedVideoFrames,
    long AudioDiscontinuities);

/// <summary>One native recording, prepared before a countdown and started after it.</summary>
public interface INativeScreenRecording : IAsyncDisposable
{
    /// <summary>Completes when the file is written; faults when recording fails.</summary>
    Task<NativeRecordingResult> Completion { get; }

    /// <summary>Messages for the log.</summary>
    event Action<string>? Diagnostic;

    /// <summary>Encoding progress in percent once capture has stopped (GIF), starting at 0.</summary>
    event Action<int>? EncodingProgressChanged;

    /// <summary>Sets up the graphics device, encoders and audio endpoints without capturing anything.</summary>
    Task PrepareAsync(CancellationToken cancellationToken = default);

    Task StartAsync(CancellationToken cancellationToken = default);

    void Pause();

    void Resume();

    /// <summary>Resumes recording a region that was moved while paused.</summary>
    void Resume(PlatformRectangle region);

    void RequestStop();

    void RequestAbort();
}

/// <summary>
/// ShareX's own screen recorder, without FFmpeg: Windows Graphics Capture, Media Foundation (H.264/AAC MP4 and GIF) and WASAPI
/// audio on Windows. Elsewhere screen recording goes through FFmpeg (<see cref="IScreenRecordingService"/>).
/// </summary>
public interface INativeScreenRecorderService
{
    FeatureSupport Support { get; }

    IReadOnlyList<CaptureDevice> GetSystemAudioDevices();

    IReadOnlyList<CaptureDevice> GetMicrophones();

    IReadOnlyList<CaptureDevice> GetCameras();

    CaptureDevice? GetDefaultSystemAudioDevice();

    CaptureDevice? GetDefaultMicrophone();

    INativeScreenRecording Create(NativeRecordingRequest request);
}

public sealed class UnsupportedNativeScreenRecorderService(string reason) : INativeScreenRecorderService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public IReadOnlyList<CaptureDevice> GetSystemAudioDevices() => [];

    public IReadOnlyList<CaptureDevice> GetMicrophones() => [];

    public IReadOnlyList<CaptureDevice> GetCameras() => [];

    public CaptureDevice? GetDefaultSystemAudioDevice() => null;

    public CaptureDevice? GetDefaultMicrophone() => null;

    public INativeScreenRecording Create(NativeRecordingRequest request) => throw new PlatformNotSupportedException(Support.Reason);
}
