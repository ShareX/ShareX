using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using SharpGen.Runtime;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Vortice;
using Vortice.MediaFoundation;

namespace ShareX.Tools;

public sealed record VideoPlaybackInfo(double Duration, int Width, int Height);

/// <summary>
/// A Windows Media Foundation player. Windows decodes, presents and synchronizes video and audio
/// in a child HWND; playback never copies frames through managed memory or starts another process.
/// All public operations and events run on the Avalonia UI thread.
/// </summary>
public sealed class WindowsMediaPlayer : NativeControlHost, IDisposable
{
    private readonly DispatcherTimer _positionTimer;
    private readonly WindowSubclass _windowProcedure;
    private TaskCompletionSource<nint> _windowReady = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private TaskCompletionSource<VideoPlaybackInfo>? _loadCompletion;
    private IMFMediaEngine? _mediaEngine;
    private IMFMediaEngineEx? _engine;
    private nint _window;
    private int _generation;
    private double _duration;
    private double _videoAspectRatio;
    private double? _pendingSeek;
    private int _pendingSteps;
    private bool _seeking, _stepping, _playWhenReady, _mediaFoundationStarted, _disposed;

    public event Action<double>? PositionChanged;
    public event Action<bool>? IsPlayingChanged;
    public event Action<Exception>? PlaybackFailed;

    public WindowsMediaPlayer()
    {
        Focusable = false;
        _windowProcedure = WindowProcedure;
        _positionTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _positionTimer.Tick += (_, _) => PublishPosition();
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (!OperatingSystem.IsWindows() || parent.HandleDescriptor != "HWND")
            return base.CreateNativeControlCore(parent);

        // SS_BLACKRECT keeps the surface black before the first frame and after unloading.
        _window = CreateWindowEx(0, "STATIC", string.Empty, 0x56000004,
            0, 0, 1, 1, parent.Handle, 0, 0, 0); // WS_CHILD | WS_VISIBLE | WS_CLIPCHILDREN | WS_CLIPSIBLINGS
        if (_window == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
        if (!SetWindowSubclass(_window, _windowProcedure, 1, 0))
        {
            DestroyWindow(_window);
            _window = 0;
            throw new Win32Exception();
        }

        _windowReady.TrySetResult(_window);
        return new PlatformHandle(_window, "HWND");
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        if (control.HandleDescriptor != "HWND")
        {
            base.DestroyNativeControlCore(control);
            return;
        }

        CloseMedia();
        RemoveWindowSubclass(control.Handle, _windowProcedure, 1);
        DestroyWindow(control.Handle);
        _window = 0;
        _windowReady = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public async Task<VideoPlaybackInfo> LoadAsync(string path, CancellationToken token = default)
    {
        Dispatcher.UIThread.VerifyAccess();
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();
        CloseMedia();
        int generation = _generation;
        try
        {
            nint window = await _windowReady.Task.WaitAsync(token);
            token.ThrowIfCancellationRequested();
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (generation != _generation) throw new OperationCanceledException();

            MediaFactory.MFStartup(true).CheckError();
            _mediaFoundationStarted = true;
            _loadCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);
            using IMFAttributes attributes = MediaFactory.MFCreateAttributes(1);
            attributes.Set(MediaEngineAttributeKeys.PlaybackHwnd.Guid, (ulong)window).CheckError();
            using IMFMediaEngineClassFactory factory = new();
            _mediaEngine = factory.CreateInstance(MediaEngineCreateFlags.None, attributes,
                (mediaEvent, parameter1, parameter2) => OnMediaEvent(generation, mediaEvent, parameter1, parameter2));
            // The base wrapper owns the managed COM notification callback; retain it through shutdown.
            _engine = _mediaEngine.QueryInterface<IMFMediaEngineEx>();
            _engine.AutoPlay = false;
            _engine.Preload = MediaEnginePreload.Automatic;
            UpdateVideoRectangle();
            // The Windows URL resolver also accepts local paths, without URI escaping of '#' or Unicode names.
            _engine.SetSource(Path.GetFullPath(path));
            _engine.Load();
            return await _loadCompletion.Task.WaitAsync(token);
        }
        catch
        {
            if (generation == _generation) CloseMedia();
            throw;
        }
    }

    public void Unload()
    {
        Dispatcher.UIThread.VerifyAccess();
        CloseMedia();
    }

    public void Seek(double seconds)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (_engine == null || _duration <= 0 || !double.IsFinite(seconds)) return;
        Pause();
        _pendingSteps = 0;
        // Seeking exactly to EOF has no video frame. Keep the final frame available when paused.
        _pendingSeek = Math.Clamp(seconds, 0, Math.Max(0, _duration - 0.000001));
        RunPendingOperation();
    }

    public void Play()
    {
        Dispatcher.UIThread.VerifyAccess();
        if (_engine == null || _duration <= 0) return;
        if (_engine.IsEnded && !_seeking && _pendingSeek == null) Seek(0);
        _playWhenReady = true;
        RunPendingOperation();
    }

    public void Pause()
    {
        Dispatcher.UIThread.VerifyAccess();
        _playWhenReady = false;
        _positionTimer.Stop();
        if (_engine != null) TryOperation(() => _engine.Pause());
        IsPlayingChanged?.Invoke(false);
    }

    public void StepFrame(bool forward)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (_engine == null || _duration <= 0) return;
        Pause();
        _pendingSteps = Math.Clamp(_pendingSteps + (forward ? 1 : -1), -60, 60);
        RunPendingOperation();
    }

    private void RunPendingOperation()
    {
        if (_engine == null || _seeking || _stepping) return;
        TryOperation(() =>
        {
            if (_pendingSeek is double target)
            {
                _pendingSeek = null;
                _seeking = true;
                // Normal seeks decode to the requested time; Approximate seeks only choose a keyframe.
                _engine.SetCurrentTimeEx(target, MediaEngineSeekMode.Normal);
            }
            else if (_pendingSteps != 0)
            {
                bool forward = _pendingSteps > 0;
                _pendingSteps += forward ? -1 : 1;
                _stepping = true;
                _engine.FrameStep(forward);
            }
            else if (_playWhenReady)
            {
                _engine.Play();
            }
        });
    }

    private void OnMediaEvent(int generation, MediaEngineEvent mediaEvent, nuint parameter1, int parameter2)
    {
        // Media Foundation holds its own lock in this callback. Never call the engine here.
        // NOTIFYSTABLESTATE is the sole event requiring a synchronous response.
        if (mediaEvent == MediaEngineEvent.NotifyStableState)
        {
            SetEvent((nint)parameter1);
            return;
        }

        if (mediaEvent is MediaEngineEvent.CanPlay or MediaEngineEvent.LoadedMetadata or
            MediaEngineEvent.Error or MediaEngineEvent.StreamRenderingError or MediaEngineEvent.ResourceLost or
            MediaEngineEvent.Seeked or MediaEngineEvent.FrameStepCompleted or
            MediaEngineEvent.Playing or MediaEngineEvent.Pause or MediaEngineEvent.Ended)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (generation != _generation || _engine == null || _disposed) return;
                TryOperation(() => HandleMediaEvent(mediaEvent, parameter2));
            });
        }
    }

    private void HandleMediaEvent(MediaEngineEvent mediaEvent, int errorCode)
    {
        switch (mediaEvent)
        {
            case MediaEngineEvent.LoadedMetadata:
                if (!_engine!.HasVideo()) throw new InvalidOperationException(Localization.Strings.VideoTrimmer_InvalidVideo);
                _engine.GetVideoAspectRatio(out int numerator, out int denominator);
                _videoAspectRatio = numerator > 0 && denominator > 0 ? (double)numerator / denominator : 0;
                UpdateVideoRectangle();
                break;
            case MediaEngineEvent.CanPlay:
                double duration = _engine!.Duration;
                if (!_engine.HasVideo() || !double.IsFinite(duration) || duration <= 0)
                    throw new InvalidOperationException(Localization.Strings.VideoTrimmer_InvalidVideo);
                _duration = duration;
                _engine.GetNativeVideoSize(out int width, out int height);
                UpdateVideoRectangle();
                _loadCompletion?.TrySetResult(new(duration, width, height));
                PublishPosition();
                break;
            case MediaEngineEvent.Seeked:
                _seeking = false;
                RunPendingOperation();
                PublishPosition();
                break;
            case MediaEngineEvent.FrameStepCompleted:
                _stepping = false;
                RunPendingOperation();
                PublishPosition();
                break;
            case MediaEngineEvent.Playing:
                // Events already queued before a pause or a new seek can arrive afterward.
                if (_playWhenReady && !_engine!.IsPaused)
                {
                    _positionTimer.Start();
                    IsPlayingChanged?.Invoke(true);
                }
                break;
            case MediaEngineEvent.Pause:
            case MediaEngineEvent.Ended:
                if (_engine!.IsPaused || _engine.IsEnded)
                {
                    _positionTimer.Stop();
                    IsPlayingChanged?.Invoke(false);
                    PublishPosition();
                }
                break;
            case MediaEngineEvent.Error:
            case MediaEngineEvent.StreamRenderingError:
            case MediaEngineEvent.ResourceLost:
                throw Marshal.GetExceptionForHR(errorCode) ?? new InvalidOperationException(Localization.Strings.VideoTrimmer_InvalidVideo);
        }
    }

    private void PublishPosition()
    {
        if (_engine == null || _duration <= 0 || _seeking || _stepping || _pendingSeek != null) return;
        TryOperation(() =>
        {
            double position = _engine.CurrentTime;
            if (double.IsFinite(position)) PositionChanged?.Invoke(Math.Clamp(position, 0, _duration));
        });
    }

    private void TryOperation(Action operation)
    {
        try { operation(); }
        catch (Exception ex)
        {
            TaskCompletionSource<VideoPlaybackInfo>? load = _loadCompletion;
            bool loading = load is { Task.IsCompleted: false };
            if (loading) load!.TrySetException(ex);
            CloseMedia();
            if (!loading) PlaybackFailed?.Invoke(ex);
        }
    }

    private void UpdateVideoRectangle()
    {
        if (_engine == null || _window == 0 || !GetClientRect(_window, out RawRect rectangle)) return;
        int width = rectangle.Right, height = rectangle.Bottom;
        if (width <= 0 || height <= 0) return;
        if (_videoAspectRatio > 0)
        {
            int videoWidth = Math.Min(width, Math.Max(1, (int)Math.Round(height * _videoAspectRatio)));
            int videoHeight = Math.Min(height, Math.Max(1, (int)Math.Round(width / _videoAspectRatio)));
            int left = (width - videoWidth) / 2, top = (height - videoHeight) / 2;
            rectangle = new RawRect(left, top, left + videoWidth, top + videoHeight);
        }
        _engine.UpdateVideoStream(null, rectangle, new Vortice.Mathematics.ColorBgra(0, 0, 0, 255));
    }

    private nint WindowProcedure(nint window, uint message, nuint wParam, nint lParam, nuint id, nuint data)
    {
        nint result = DefSubclassProc(window, message, wParam, lParam);
        // Resize coordinates are physical pixels, including when the window moves between DPI settings.
        if (message is 0x0005 or 0x000F) TryOperation(UpdateVideoRectangle); // WM_SIZE / WM_PAINT
        return result;
    }

    private void CloseMedia()
    {
        _generation++;
        _positionTimer.Stop();
        _loadCompletion?.TrySetCanceled();
        _loadCompletion = null;
        _pendingSeek = null;
        _pendingSteps = 0;
        _duration = 0;
        _videoAspectRatio = 0;
        _seeking = _stepping = _playWhenReady = false;
        if (_engine != null)
        {
            try { _engine.Shutdown(); }
            catch (SharpGenException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            finally { _engine.Dispose(); _engine = null; }
        }
        _mediaEngine?.Dispose();
        _mediaEngine = null;
        if (_mediaFoundationStarted)
        {
            MediaFactory.MFShutdown();
            _mediaFoundationStarted = false;
        }
        IsPlayingChanged?.Invoke(false);
        if (_window != 0) InvalidateRect(_window, 0, true);
    }

    public void Dispose()
    {
        Dispatcher.UIThread.VerifyAccess();
        if (_disposed) return;
        _disposed = true;
        CloseMedia();
        _windowReady.TrySetCanceled();
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate nint WindowSubclass(nint window, uint message, nuint wParam, nint lParam, nuint id, nuint data);

    [DllImport("user32.dll", EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint CreateWindowEx(uint extendedStyle, string className, string title, uint style,
        int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(nint window);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetClientRect(nint window, out RawRect rectangle);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool InvalidateRect(nint window, nint rectangle, [MarshalAs(UnmanagedType.Bool)] bool erase);
    [DllImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowSubclass(nint window, WindowSubclass procedure, nuint id, nuint data);
    [DllImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RemoveWindowSubclass(nint window, WindowSubclass procedure, nuint id);
    [DllImport("comctl32.dll")]
    private static extern nint DefSubclassProc(nint window, uint message, nuint wParam, nint lParam);
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetEvent(nint handle);
}
