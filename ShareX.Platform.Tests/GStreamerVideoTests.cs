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

using ShareX.Platform.Linux.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.Platform.Tests;

public class GStreamerVideoTests
{
    [Fact]
    public void Support_NeedsAnXServer()
    {
        GStreamerVideoPlaybackService service = new GStreamerVideoPlaybackService(() => false);

        Assert.False(service.Support.IsSupported);
        Assert.Contains("X server", service.Support.Reason);
        Assert.Throws<PlatformNotSupportedException>(() => service.CreatePlayback(1));
    }

    [LinuxFact]
    public async Task Thumbnails_AreFramesOfTheVideoAtItsAspectRatio()
    {
        string? video = CreateTestVideo();
        if (video == null) return; // GStreamer or its Theora encoder is not installed here.

        try
        {
            GStreamerVideoPlaybackService service = new GStreamerVideoPlaybackService(() => true);
            IReadOnlyList<VideoThumbnail> thumbnails = await service.CreateThumbnailsAsync(video, 2, 4);

            Assert.Equal(4, thumbnails.Count);
            Assert.Equal([0, 0.5, 1, 1.5], thumbnails.Select(thumbnail => thumbnail.Position));
            Assert.All(thumbnails, thumbnail =>
            {
                // 320x180 scaled to a width of 160 keeps 16:9.
                Assert.Equal(160, thumbnail.Image.Width);
                Assert.Equal(90, thumbnail.Image.Height);
                // The SMPTE colour bars are not black.
                Assert.Contains(thumbnail.Image.Pixels, value => value > 64);
            });
        }
        finally
        {
            File.Delete(video);
        }
    }

    /// <summary>
    /// Opens, plays, seeks and steps a video in a 1x1 muted child of the root window. It needs a running X server, so it runs
    /// only when SHAREX_GSTREAMER_WINDOW_TEST=1 is set; CI and normal runs never open a window on a desktop.
    /// </summary>
    [LinuxFact]
    public void Playback_LoadsPlaysSeeksAndSteps()
    {
        if (Environment.GetEnvironmentVariable("SHAREX_GSTREAMER_WINDOW_TEST") != "1" || string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY"))) return;
        string? video = CreateTestVideo();
        if (video == null) return;

        // GStreamer's sinks use Xlib from their own threads; Avalonia calls XInitThreads first in the app.
        XInitThreads();
        using SingleThreadContext context = new SingleThreadContext();
        try
        {
            context.Run(() =>
            {
                IntPtr display = ShareX.Platform.Linux.Native.X11.XOpenDisplay(IntPtr.Zero);
                nuint root = ShareX.Platform.Linux.Native.X11.XDefaultRootWindow(display);
                GStreamerVideoPlaybackService service = new GStreamerVideoPlaybackService(() => true);
                Assert.True(service.Support.IsSupported, service.Support.Reason);
                using IVideoPlayback playback = service.CreatePlayback((long)root);
                playback.IsMuted = true;
                List<double> positions = new List<double>();
                List<bool> playing = new List<bool>();
                playback.PositionChanged += positions.Add;
                playback.IsPlayingChanged += playing.Add;

                VideoPlaybackInfo info = context.Wait(playback.LoadAsync(video));
                Assert.Equal(320, info.Width);
                Assert.Equal(180, info.Height);
                Assert.InRange(info.Duration, 1.9, 2.1);
                Assert.Equal(16d / 9, playback.AspectRatio, 3);

                playback.Play();
                context.Pump(TimeSpan.FromMilliseconds(700));
                Assert.Contains(true, playing);
                Assert.Contains(positions, position => position > 0.2);

                playback.Seek(1.5);
                context.Pump(TimeSpan.FromMilliseconds(300));
                Assert.False(playing[^1]);
                positions.Clear();
                playback.StepFrame(true);
                context.Pump(TimeSpan.FromMilliseconds(300));
                Assert.Contains(positions, position => position >= 1.5);
                ShareX.Platform.Linux.Native.X11.XCloseDisplay(display);
            });
        }
        finally
        {
            File.Delete(video);
        }
    }

    [System.Runtime.InteropServices.DllImport("libX11.so.6")]
    private static extern int XInitThreads();

    /// <summary>A synchronization context on one thread, pumped by the test, like a UI thread.</summary>
    private sealed class SingleThreadContext : System.Threading.SynchronizationContext, IDisposable
    {
        private readonly System.Collections.Concurrent.BlockingCollection<(System.Threading.SendOrPostCallback, object?)> queue = new();

        public override void Post(System.Threading.SendOrPostCallback d, object? state) => queue.Add((d, state));

        public void Run(Action action)
        {
            System.Threading.SynchronizationContext? previous = Current;
            SetSynchronizationContext(this);
            try { action(); }
            finally { SetSynchronizationContext(previous); }
        }

        public void Pump(TimeSpan duration)
        {
            Stopwatch watch = Stopwatch.StartNew();
            while (watch.Elapsed < duration)
            {
                if (queue.TryTake(out var item, 10)) item.Item1(item.Item2);
            }
        }

        public T Wait<T>(Task<T> task)
        {
            Stopwatch watch = Stopwatch.StartNew();
            while (!task.IsCompleted && watch.Elapsed < TimeSpan.FromSeconds(10))
            {
                if (queue.TryTake(out var item, 10)) item.Item1(item.Item2);
            }

            return task.GetAwaiter().GetResult();
        }

        public void Dispose() => queue.Dispose();
    }

    /// <summary>Two seconds of colour bars as Ogg Theora (gst-plugins-base), or null without gst-launch-1.0.</summary>
    private static string? CreateTestVideo()
    {
        string path = Path.Combine(Path.GetTempPath(), $"sharex-gst-{Guid.NewGuid():N}.ogv");
        try
        {
            using Process process = Process.Start(new ProcessStartInfo("gst-launch-1.0",
                ["-q", "videotestsrc", "num-buffers=60", "pattern=smpte", "!", "video/x-raw,width=320,height=180,framerate=30/1", "!",
                    "videoconvert", "!", "theoraenc", "!", "oggmux", "!", "filesink", $"location={path}"])
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            })!;
            process.WaitForExit(30_000);
            return process.ExitCode == 0 && File.Exists(path) && new FileInfo(path).Length > 0 ? path : null;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return null;
        }
    }
}
