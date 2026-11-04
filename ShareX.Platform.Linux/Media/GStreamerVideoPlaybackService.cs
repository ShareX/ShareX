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

using ShareX.Platform.Imaging;
using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux.Media;

/// <summary>
/// Video playback and filmstrips through the distribution's GStreamer (gst-plugins-base and good), in an X11 child window.
/// Unsupported without GStreamer, a video sink that draws into a foreign window, or an X server (ShareX uses XWayland on Wayland).
/// </summary>
public sealed class GStreamerVideoPlaybackService : IVideoPlaybackService
{
    // Sinks that scale into a foreign window, best first. ximagesink draws at the video's size but works everywhere.
    private static readonly string[] Sinks = ["glimagesink", "xvimagesink", "ximagesink"];

    private readonly Lazy<(FeatureSupport Support, string? Sink)> state;

    public GStreamerVideoPlaybackService(Func<bool> hasX11)
    {
        state = new Lazy<(FeatureSupport, string?)>(() =>
        {
            if (!hasX11()) return (FeatureSupport.NotSupported("Video playback draws into an X11 window, and no X server is available."), null);
            if (!GStreamer.IsAvailable) return (FeatureSupport.RequiresTool("GStreamer (gstreamer and gst-plugins-base)"), null);
            if (!GStreamer.HasElement("playbin")) return (FeatureSupport.RequiresTool("GStreamer's playbin (gst-plugins-base)"), null);
            string? sink = Sinks.FirstOrDefault(GStreamer.HasElement);
            return sink == null
                ? (FeatureSupport.RequiresTool("a GStreamer video sink (gst-plugins-base or gst-plugins-good)"), null)
                : (FeatureSupport.Supported, sink);
        });
    }

    public FeatureSupport Support => state.Value.Support;

    public IVideoPlayback CreatePlayback(long parentWindowHandle)
    {
        if (!Support.IsSupported) throw new PlatformNotSupportedException(Support.Reason);
        return new GStreamerVideoPlayback((nuint)parentWindowHandle,
            SynchronizationContext.Current ?? throw new InvalidOperationException("Create the video player on the UI thread."), state.Value.Sink!);
    }

    public Task<IReadOnlyList<VideoThumbnail>> CreateThumbnailsAsync(string path, double duration, int count, CancellationToken cancellationToken = default)
    {
        if (!GStreamer.IsAvailable) return Task.FromException<IReadOnlyList<VideoThumbnail>>(new PlatformNotSupportedException(Support.Reason));
        return Task.Run(() => CreateThumbnails(path, duration, count, cancellationToken), cancellationToken);
    }

    private static unsafe IReadOnlyList<VideoThumbnail> CreateThumbnails(string path, double duration, int count, CancellationToken cancellationToken)
    {
        string uri = GStreamer.TakeString(GStreamer.gst_filename_to_uri(Path.GetFullPath(path), IntPtr.Zero)) ?? throw new FileNotFoundException(path);
        // videoscale keeps the display aspect ratio when only the width is fixed.
        string description = $"uridecodebin uri=\"{uri.Replace("\"", "%22")}\" ! videoconvert ! videoscale ! " +
            "video/x-raw,format=BGRA,width=160,pixel-aspect-ratio=1/1 ! appsink name=sink sync=false max-buffers=1";
        IntPtr pipeline = GStreamer.gst_parse_launch(description, out IntPtr error);
        if (error != IntPtr.Zero) GStreamer.g_error_free(error);
        if (pipeline == IntPtr.Zero) throw new InvalidDataException("GStreamer cannot read this video.");
        GStreamer.gst_object_ref_sink(pipeline);
        IntPtr sink = GStreamer.gst_bin_get_by_name(pipeline, "sink");
        List<VideoThumbnail> thumbnails = new List<VideoThumbnail>();
        try
        {
            if (sink == IntPtr.Zero || GStreamer.gst_element_set_state(pipeline, GStreamer.StatePaused) == GStreamer.StateChangeFailure ||
                GStreamer.gst_element_get_state(pipeline, out _, out _, 10 * 1_000_000_000UL) == GStreamer.StateChangeFailure)
            {
                throw new InvalidDataException("GStreamer cannot read this video.");
            }

            for (int i = 0; i < count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                double position = duration * i / count;
                GStreamer.gst_element_seek_simple(pipeline, GStreamer.FormatTime, GStreamer.SeekFlagFlush | GStreamer.SeekFlagAccurate,
                    (long)(position * 1_000_000_000));
                IntPtr sample = GStreamer.gst_app_sink_try_pull_preroll(sink, 5 * 1_000_000_000UL);
                if (sample == IntPtr.Zero) continue;
                try
                {
                    if (ReadFrame(sample) is PixelBuffer image) thumbnails.Add(new VideoThumbnail(position, image));
                }
                finally
                {
                    GStreamer.gst_mini_object_unref(sample);
                }
            }

            return thumbnails;
        }
        finally
        {
            GStreamer.gst_element_set_state(pipeline, GStreamer.StateNull);
            if (sink != IntPtr.Zero) GStreamer.gst_object_unref(sink);
            GStreamer.gst_object_unref(pipeline);
        }
    }

    private static unsafe PixelBuffer? ReadFrame(IntPtr sample)
    {
        if (!GStreamer.TryGetVideoSize(GStreamer.gst_sample_get_caps(sample), out int width, out int height, out _)) return null;
        IntPtr buffer = GStreamer.gst_sample_get_buffer(sample);
        GStreamer.GstMapInfo map = default;
        if (buffer == IntPtr.Zero || !GStreamer.gst_buffer_map(buffer, &map, GStreamer.MapRead)) return null;
        try
        {
            int stride = width * 4;
            if ((long)map.Size < (long)stride * height) return null;
            PixelBuffer image = PixelBuffer.FromBgra((IntPtr)map.Data, width, height, stride, forceOpaque: true);
            return image;
        }
        finally
        {
            GStreamer.gst_buffer_unmap(buffer, &map);
        }
    }
}
