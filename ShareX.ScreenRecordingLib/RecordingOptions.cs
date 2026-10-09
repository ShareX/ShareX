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
    public bool CaptureCamera { get; init; }
    /// <summary>Camera symbolic link; null selects the first available camera.</summary>
    public string? CameraDeviceId { get; init; }
    /// <summary>Preferred camera mode; the closest supported resolution/rate is negotiated.</summary>
    public CameraCaptureResolution CameraResolution { get; init; } = CameraCaptureResolution.Size1280x720;
    public int CameraFramesPerSecond { get; init; } = 30;
    public CameraOverlayPosition CameraPosition { get; init; } = CameraOverlayPosition.BottomRight;
    /// <summary>Circles use a centered square crop; rectangles preserve the full camera image.</summary>
    public CameraOverlayShape CameraShape { get; init; } = CameraOverlayShape.Rectangle;
    /// <summary>Overlay width as a percentage of the recording, fitted to the selected shape.</summary>
    public int CameraWidthPercent { get; init; } = 20;
    /// <summary>Distance from the recording's corner in physical pixels, clamped to fit.</summary>
    public int CameraMargin { get; init; } = 16;
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
        if (!Enum.IsDefined(CameraResolution)) throw new ArgumentOutOfRangeException(nameof(CameraResolution));
        if (!Enum.IsDefined(CameraPosition)) throw new ArgumentOutOfRangeException(nameof(CameraPosition));
        if (!Enum.IsDefined(CameraShape)) throw new ArgumentOutOfRangeException(nameof(CameraShape));
        if (CameraFramesPerSecond is < 1 or > 60) throw new ArgumentOutOfRangeException(nameof(CameraFramesPerSecond));
        if (CameraWidthPercent is < 5 or > 50) throw new ArgumentOutOfRangeException(nameof(CameraWidthPercent));
        if (CameraMargin is < 0 or > 1000) throw new ArgumentOutOfRangeException(nameof(CameraMargin));
        if (!float.IsFinite(SystemAudioGain) || SystemAudioGain is < 0 or > 4) throw new ArgumentOutOfRangeException(nameof(SystemAudioGain));
        if (!float.IsFinite(MicrophoneGain) || MicrophoneGain is < 0 or > 4) throw new ArgumentOutOfRangeException(nameof(MicrophoneGain));
    }
}

public sealed record VideoEncoderInfo(string Name, bool IsHardwareAccelerated, bool IsD3D11Aware);

public sealed record RecordingResult(string OutputPath, TimeSpan Duration, long VideoFrames,
    long DroppedVideoFrames, long AudioDiscontinuities, VideoEncoderInfo Encoder);
