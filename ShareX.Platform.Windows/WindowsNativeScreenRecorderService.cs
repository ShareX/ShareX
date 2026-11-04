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

#if WINDOWS10_0_17763_0_OR_GREATER
using ShareX.ScreenRecordingLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Windows;

/// <summary>ShareX.ScreenRecordingLib behind the platform contract: Windows Graphics Capture, Media Foundation and WASAPI.</summary>
[SupportedOSPlatform("windows10.0.22621.0")]
public sealed class WindowsNativeScreenRecorderService : INativeScreenRecorderService
{
    public FeatureSupport Support => FeatureSupport.Supported;

    public IReadOnlyList<CaptureDevice> GetSystemAudioDevices() => AudioCaptureDevices.GetSystemAudioDevices().Select(ToDevice).ToList();

    public IReadOnlyList<CaptureDevice> GetMicrophones() => AudioCaptureDevices.GetMicrophones().Select(ToDevice).ToList();

    public IReadOnlyList<CaptureDevice> GetCameras() =>
        CameraCaptureDevices.GetCameras().Select(device => new CaptureDevice(device.Id, device.Name)).ToList();

    public CaptureDevice? GetDefaultSystemAudioDevice() => AudioCaptureDevices.GetDefaultSystemAudioDevice() is { } device ? ToDevice(device) : null;

    public CaptureDevice? GetDefaultMicrophone() => AudioCaptureDevices.GetDefaultMicrophone() is { } device ? ToDevice(device) : null;

    public INativeScreenRecording Create(NativeRecordingRequest request) => new Recording(new ScreenRecorder(ToOptions(request)));

    private static CaptureDevice ToDevice(AudioCaptureDevice device) => new CaptureDevice(device.Id, device.Name);

    internal static RecordingOptions ToOptions(NativeRecordingRequest request) => new RecordingOptions
    {
        OutputPath = request.OutputPath,
        Region = ToRectangle(request.Region),
        WindowHandle = (nint)request.WindowHandle,
        FramesPerSecond = request.FramesPerSecond,
        VideoBitrate = request.VideoBitrate,
        IncludeCursor = request.IncludeCursor,
        CaptureSystemAudio = request.CaptureSystemAudio,
        SystemAudioDeviceId = request.SystemAudioDeviceId,
        SystemAudioGain = request.SystemAudioGain,
        CaptureMicrophone = request.CaptureMicrophone,
        MicrophoneDeviceId = request.MicrophoneDeviceId,
        MicrophoneGain = request.MicrophoneGain,
        CaptureCamera = request.CaptureCamera,
        CameraDeviceId = request.CameraDeviceId,
        CameraResolution = request.CameraResolution,
        CameraFramesPerSecond = request.CameraFramesPerSecond,
        CameraPosition = request.CameraPosition,
        CameraShape = request.CameraShape,
        CameraWidthPercent = request.CameraWidthPercent,
        CameraMargin = request.CameraMargin,
        RequireHardwareEncoder = request.RequireHardwareEncoder,
        Duration = request.Duration
    };

    private static Rectangle ToRectangle(PlatformRectangle r) => new Rectangle(r.X, r.Y, r.Width, r.Height);

    private sealed class Recording(ScreenRecorder recorder) : INativeScreenRecording
    {
        public Task<NativeRecordingResult> Completion { get; } = recorder.Completion.ContinueWith(task =>
        {
            RecordingResult result = task.GetAwaiter().GetResult();
            return new NativeRecordingResult(result.Encoder.Name, result.Encoder.IsHardwareAccelerated, result.VideoFrames,
                result.DroppedVideoFrames, result.AudioDiscontinuities);
        }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        public event Action<string>? Diagnostic
        {
            add => recorder.Diagnostic += value;
            remove => recorder.Diagnostic -= value;
        }

        public event Action<int>? EncodingProgressChanged
        {
            add => recorder.EncodingProgressChanged += value;
            remove => recorder.EncodingProgressChanged -= value;
        }

        public Task PrepareAsync(CancellationToken cancellationToken = default) => recorder.PrepareAsync(cancellationToken);

        public Task StartAsync(CancellationToken cancellationToken = default) => recorder.StartAsync(cancellationToken);

        public void Pause() => recorder.Pause();

        public void Resume() => recorder.Resume();

        public void Resume(PlatformRectangle region) => recorder.Resume(ToRectangle(region));

        public void RequestStop() => recorder.RequestStop();

        public void RequestAbort() => recorder.RequestAbort();

        public ValueTask DisposeAsync() => recorder.DisposeAsync();
    }
}
#endif
