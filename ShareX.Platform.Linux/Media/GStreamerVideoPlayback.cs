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

using ShareX.Platform.Linux.Native;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux.Media;

/// <summary>
/// GStreamer playbin drawing into an X11 child window that the UI toolkit embeds (ShareX's windows are X11 or XWayland).
/// GStreamer decodes and synchronizes video and audio. The UI thread polls the bus, position and size on a timer, so every
/// event is raised there; nothing calls back into managed code from GStreamer's threads.
/// </summary>
internal sealed unsafe class GStreamerVideoPlayback : IVideoPlayback
{
    // CWBackPixel | CWEventMask; ButtonPressMask | ButtonReleaseMask.
    private const nuint WindowAttributes = (1 << 1) | (1 << 11);
    private const nint ButtonEvents = (1 << 2) | (1 << 3);
    private const int ButtonPress = 4, ButtonRelease = 5;
    private const long NanosecondsPerSecond = 1_000_000_000;

    private readonly SynchronizationContext _context;
    private readonly int _threadId = Environment.CurrentManagedThreadId;
    private readonly IntPtr _display;
    private readonly string _sinkFactory;
    private Timer? _timer;
    private nuint _window;
    private IntPtr _pipeline, _sink, _bus;
    private TaskCompletionSource<VideoPlaybackInfo>? _loadCompletion;
    private double _duration, _aspectRatio, _frameDuration = 1d / 30;
    private uint _width, _height;
    private double _volume = 1;
    private bool _isMuted, _playWhenReady, _playing, _buttonPressed, _disposed, _ticking;

    public GStreamerVideoPlayback(nuint parent, SynchronizationContext context, string sinkFactory)
    {
        _context = context;
        _sinkFactory = sinkFactory;
        _display = X11.XOpenDisplay(IntPtr.Zero);
        if (_display == IntPtr.Zero) throw new InvalidOperationException("Cannot connect to the X server.");

        X11.XSetWindowAttributes attributes = new X11.XSetWindowAttributes { background_pixel = 0, event_mask = ButtonEvents };
        // depth 0 and a null visual: CopyFromParent.
        _window = X11.XCreateWindow(_display, parent, 0, 0, 1, 1, 0, 0, X11.InputOutput, IntPtr.Zero, WindowAttributes, &attributes);
        if (_window == 0)
        {
            X11.XCloseDisplay(_display);
            throw new InvalidOperationException("Cannot create the video window.");
        }

        X11.XMapRaised(_display, _window);
        X11.XFlush(_display);
        _timer = new Timer(_ => _context.Post(_ => Tick(), null), null, 33, 33);
    }

    public long Handle => (long)_window;
    public string HandleDescriptor => "XID";
    public double AspectRatio => _aspectRatio;
    public bool IsPlaybackRequested => _playWhenReady;

    public double Volume
    {
        get => _volume;
        set
        {
            if (!double.IsFinite(value)) return;
            _volume = Math.Clamp(value, 0, 1);
            if (_pipeline != IntPtr.Zero) GStreamer.SetProperty(_pipeline, "volume", _volume);
        }
    }

    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            _isMuted = value;
            if (_pipeline != IntPtr.Zero) GStreamer.SetProperty(_pipeline, "mute", value);
        }
    }

    public event Action<double>? PositionChanged;
    public event Action<bool>? IsPlayingChanged;
    public event Action<Exception>? PlaybackFailed;
    public event Action? VideoClicked;
    public event Action? AspectRatioChanged;

    public Task<VideoPlaybackInfo> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        VerifyAccess();
        ObjectDisposedException.ThrowIf(_disposed, this);
        Unload();

        IntPtr uri = GStreamer.gst_filename_to_uri(Path.GetFullPath(path), IntPtr.Zero);
        string? uriText = GStreamer.TakeString(uri) ?? throw new FileNotFoundException(path);
        _pipeline = GStreamer.gst_element_factory_make("playbin", null);
        if (_pipeline == IntPtr.Zero) throw new InvalidOperationException("GStreamer has no playbin element (gst-plugins-base).");
        GStreamer.gst_object_ref_sink(_pipeline);

        _sink = GStreamer.gst_element_factory_make(_sinkFactory, null);
        if (_sink != IntPtr.Zero)
        {
            GStreamer.gst_object_ref_sink(_sink);
            GStreamer.SetProperty(_sink, "force-aspect-ratio", true);
            GStreamer.gst_video_overlay_set_window_handle(_sink, _window);
            GStreamer.SetObjectProperty(_pipeline, "video-sink", _sink);
        }

        GStreamer.SetProperty(_pipeline, "uri", uriText);
        GStreamer.SetProperty(_pipeline, "volume", _volume);
        GStreamer.SetProperty(_pipeline, "mute", _isMuted);
        _bus = GStreamer.gst_element_get_bus(_pipeline);
        _loadCompletion = new TaskCompletionSource<VideoPlaybackInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<VideoPlaybackInfo> load = _loadCompletion;

        // Prerolls in the background; ASYNC_DONE or an error arrives on the bus, read by Tick.
        if (GStreamer.gst_element_set_state(_pipeline, GStreamer.StatePaused) == GStreamer.StateChangeFailure)
        {
            Unload();
            throw new InvalidDataException("The file has no playable video.");
        }

        CancellationTokenRegistration registration = cancellationToken.Register(() =>
            _context.Post(_ => { if (_loadCompletion == load) Unload(); load.TrySetCanceled(); }, null));
        load.Task.ContinueWith(_ => registration.Dispose(), TaskScheduler.Default);
        return load.Task;
    }

    public void Unload()
    {
        VerifyAccess();
        _loadCompletion?.TrySetCanceled();
        _loadCompletion = null;
        _playWhenReady = false;
        SetPlaying(false);
        if (_pipeline != IntPtr.Zero)
        {
            GStreamer.gst_element_set_state(_pipeline, GStreamer.StateNull);
            if (_bus != IntPtr.Zero) GStreamer.gst_object_unref(_bus);
            GStreamer.gst_object_unref(_pipeline);
            if (_sink != IntPtr.Zero) GStreamer.gst_object_unref(_sink);
        }

        _pipeline = _sink = _bus = IntPtr.Zero;
        _duration = 0;
        if (_aspectRatio != 0)
        {
            _aspectRatio = 0;
            AspectRatioChanged?.Invoke();
        }
    }

    public void Seek(double seconds)
    {
        VerifyAccess();
        if (_pipeline == IntPtr.Zero || _duration <= 0 || !double.IsFinite(seconds)) return;
        Pause();
        // Seeking exactly to the end has no video frame; keep the last one.
        double target = Math.Clamp(seconds, 0, Math.Max(0, _duration - _frameDuration / 2));
        GStreamer.gst_element_seek_simple(_pipeline, GStreamer.FormatTime, GStreamer.SeekFlagFlush | GStreamer.SeekFlagAccurate,
            (long)(target * NanosecondsPerSecond));
    }

    public void Play()
    {
        VerifyAccess();
        if (_pipeline == IntPtr.Zero || _duration <= 0) return;
        if (QueryPosition() is double position && position >= _duration - _frameDuration) Seek(0);
        _playWhenReady = true;
        GStreamer.gst_element_set_state(_pipeline, GStreamer.StatePlaying);
        SetPlaying(true);
    }

    public void Pause()
    {
        VerifyAccess();
        _playWhenReady = false;
        if (_pipeline != IntPtr.Zero) GStreamer.gst_element_set_state(_pipeline, GStreamer.StatePaused);
        SetPlaying(false);
        PublishPosition();
    }

    public void StepFrame(bool forward)
    {
        VerifyAccess();
        if (_pipeline == IntPtr.Zero || _duration <= 0) return;
        Pause();
        if (forward)
        {
            GStreamer.gst_element_send_event(_pipeline, GStreamer.gst_event_new_step(GStreamer.FormatBuffers, 1, 1, true, false));
        }
        else if (QueryPosition() is double position)
        {
            Seek(position - _frameDuration);
        }

        PublishPosition();
    }

    private void Tick()
    {
        if (_disposed || _ticking) return;
        _ticking = true;
        try
        {
            ReadClicks();
            ReadBus();
            UpdateRenderRectangle();
            if (_playing) PublishPosition();
        }
        catch (Exception e)
        {
            Fail(e);
        }
        finally
        {
            _ticking = false;
        }
    }

    private void ReadBus()
    {
        IntPtr message;
        while (_bus != IntPtr.Zero && (message = GStreamer.gst_bus_pop(_bus)) != IntPtr.Zero)
        {
            int type = GStreamer.MessageType(message);
            try
            {
                switch (type)
                {
                    case GStreamer.MessageError:
                        Fail(new InvalidDataException(GStreamer.ParseError(message)));
                        return;
                    case GStreamer.MessageEos:
                        _playWhenReady = false;
                        GStreamer.gst_element_set_state(_pipeline, GStreamer.StatePaused);
                        SetPlaying(false);
                        PositionChanged?.Invoke(_duration);
                        break;
                    case GStreamer.MessageAsyncDone:
                        // Preroll, a seek or a frame step finished.
                        if (_loadCompletion != null) CompleteLoad();
                        else PublishPosition();
                        break;
                }
            }
            finally
            {
                GStreamer.gst_mini_object_unref(message);
            }
        }
    }

    private void CompleteLoad()
    {
        TaskCompletionSource<VideoPlaybackInfo> load = _loadCompletion!;
        _loadCompletion = null;
        if (!GStreamer.gst_element_query_duration(_pipeline, GStreamer.FormatTime, out long duration) || duration <= 0 || !ReadVideoSize(out int width, out int height))
        {
            Unload();
            load.TrySetException(new InvalidDataException("The file has no playable video."));
            return;
        }

        _duration = duration / (double)NanosecondsPerSecond;
        AspectRatioChanged?.Invoke();
        load.TrySetResult(new VideoPlaybackInfo(_duration, width, height));
        PublishPosition();
    }

    private bool ReadVideoSize(out int width, out int height)
    {
        width = height = 0;
        if (_sink == IntPtr.Zero) return false;
        IntPtr pad = GStreamer.gst_element_get_static_pad(_sink, "sink");
        if (pad == IntPtr.Zero) return false;
        IntPtr caps = GStreamer.gst_pad_get_current_caps(pad);
        GStreamer.gst_object_unref(pad);
        if (caps == IntPtr.Zero) return false;
        try
        {
            if (!GStreamer.TryGetVideoSize(caps, out width, out height, out double pixelAspect)) return false;
            _aspectRatio = width * pixelAspect / height;
            IntPtr structure = GStreamer.gst_caps_get_structure(caps, 0);
            if (GStreamer.gst_structure_get_fraction(structure, "framerate", out int frames, out int seconds) && frames > 0 && seconds > 0)
            {
                _frameDuration = (double)seconds / frames;
            }

            return true;
        }
        finally
        {
            GStreamer.gst_mini_object_unref(caps);
        }
    }

    private void UpdateRenderRectangle()
    {
        if (X11.XGetGeometry(_display, _window, out _, out _, out _, out uint width, out uint height, out _, out _) == 0) return;
        if (width == _width && height == _height) return;
        _width = width;
        _height = height;
        if (_sink != IntPtr.Zero)
        {
            GStreamer.gst_video_overlay_set_render_rectangle(_sink, 0, 0, (int)width, (int)height);
            GStreamer.gst_video_overlay_expose(_sink);
        }
    }

    private void ReadClicks()
    {
        while (X11.XPending(_display) > 0)
        {
            X11.XNextEvent(_display, out X11.XEvent xEvent);
            if (xEvent.type == ButtonPress && ButtonOf(&xEvent) == 1)
            {
                _buttonPressed = true;
            }
            else if (xEvent.type == ButtonRelease && ButtonOf(&xEvent) == 1 && _buttonPressed)
            {
                _buttonPressed = false;
                if (_duration > 0) VideoClicked?.Invoke();
            }
        }
    }

    // XButtonEvent.button follows type, serial, send_event, display, window, root, subwindow, time, x, y, x_root, y_root, state.
    private static uint ButtonOf(X11.XEvent* xEvent) => *(uint*)((byte*)xEvent + 84);

    private double? QueryPosition() =>
        _pipeline != IntPtr.Zero && GStreamer.gst_element_query_position(_pipeline, GStreamer.FormatTime, out long position) && position >= 0
            ? Math.Clamp(position / (double)NanosecondsPerSecond, 0, _duration)
            : null;

    private void PublishPosition()
    {
        if (_duration > 0 && QueryPosition() is double position) PositionChanged?.Invoke(position);
    }

    private void SetPlaying(bool playing)
    {
        if (_playing == playing) return;
        _playing = playing;
        IsPlayingChanged?.Invoke(playing);
    }

    private void Fail(Exception error)
    {
        TaskCompletionSource<VideoPlaybackInfo>? load = _loadCompletion;
        Unload();
        if (load != null) load.TrySetException(error);
        else PlaybackFailed?.Invoke(error);
    }

    private void VerifyAccess()
    {
        if (Environment.CurrentManagedThreadId != _threadId) throw new InvalidOperationException("The video player is used from another thread.");
    }

    public void Dispose()
    {
        if (_disposed) return;
        Unload();
        _disposed = true;
        _timer?.Dispose();
        _timer = null;
        if (_window != 0) X11.XDestroyWindow(_display, _window);
        _window = 0;
        X11.XCloseDisplay(_display);
    }
}
