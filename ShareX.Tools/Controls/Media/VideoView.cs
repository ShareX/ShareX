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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using ShareX.Platform;

namespace ShareX.Tools;

/// <summary>
/// Hosts the system's video player (<see cref="IVideoPlaybackService"/>) as a native child view: Media Foundation on Windows.
/// Where the system has none, <see cref="IsSupported"/> is false and the control stays empty.
/// All members are used on the UI thread.
/// </summary>
public sealed class VideoView : NativeControlHost, IDisposable
{
    private TaskCompletionSource<IVideoPlayback> _playbackReady = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private IVideoPlayback? _playback;
    private double _volume = 1;
    private bool _isMuted;
    private bool _disposed;

    public event Action<double>? PositionChanged;
    public event Action<bool>? IsPlayingChanged;
    public event Action<Exception>? PlaybackFailed;
    public event Action? VideoClicked;

    public VideoView()
    {
        Focusable = false;
    }

    public static FeatureSupport Support => PlatformServices.IsInitialized
        ? PlatformServices.Current.VideoPlayback.Support
        : FeatureSupport.NotSupported("No platform services.");

    public static bool IsSupported => Support.IsSupported;

    public bool IsPlaybackRequested => _playback?.IsPlaybackRequested ?? false;

    public double Volume
    {
        get => _volume;
        set
        {
            if (!double.IsFinite(value)) return;
            _volume = Math.Clamp(value, 0, 1);
            if (_playback != null) _playback.Volume = _volume;
        }
    }

    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            _isMuted = value;
            if (_playback != null) _playback.IsMuted = value;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double ratio = _playback?.AspectRatio ?? 0;
        if (ratio <= 0) return base.MeasureOverride(availableSize);
        double width = Math.Min(availableSize.Width, availableSize.Height * ratio);
        return double.IsFinite(width) ? new Size(width, width / ratio) : base.MeasureOverride(availableSize);
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (_disposed || !IsSupported) return base.CreateNativeControlCore(parent);

        IVideoPlayback playback = PlatformServices.Current.VideoPlayback.CreatePlayback(parent.Handle);
        playback.Volume = _volume;
        playback.IsMuted = _isMuted;
        playback.PositionChanged += position => PositionChanged?.Invoke(position);
        playback.IsPlayingChanged += playing => IsPlayingChanged?.Invoke(playing);
        playback.PlaybackFailed += error => PlaybackFailed?.Invoke(error);
        playback.VideoClicked += () => VideoClicked?.Invoke();
        playback.AspectRatioChanged += InvalidateMeasure;
        _playback = playback;
        _playbackReady.TrySetResult(playback);
        return new PlatformHandle((IntPtr)playback.Handle, playback.HandleDescriptor);
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        if (_playback == null || control.Handle != (IntPtr)_playback.Handle)
        {
            base.DestroyNativeControlCore(control);
            return;
        }

        _playback.Dispose();
        _playback = null;
        _playbackReady = new(TaskCreationOptions.RunContinuationsAsynchronously);
        InvalidateMeasure();
    }

    /// <summary>Opens a video once the native view exists. Throws <see cref="PlatformNotSupportedException"/> without playback.</summary>
    public async Task<VideoPlaybackInfo> LoadAsync(string path, CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!IsSupported) throw new PlatformNotSupportedException(Support.Reason);
        IVideoPlayback playback = await _playbackReady.Task.WaitAsync(token);
        return await playback.LoadAsync(path, token);
    }

    public void Unload() => _playback?.Unload();

    public void Seek(double seconds) => _playback?.Seek(seconds);

    public void Play() => _playback?.Play();

    public void Pause() => _playback?.Pause();

    public void StepFrame(bool forward) => _playback?.StepFrame(forward);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _playback?.Unload();
        _playbackReady.TrySetCanceled();
    }
}
