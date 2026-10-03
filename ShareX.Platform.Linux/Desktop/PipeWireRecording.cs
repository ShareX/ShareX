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

using ShareX.Platform.Diagnostics;
using ShareX.Platform.Linux.DBus;
using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>
/// Records through the ScreenCast portal (GNOME, KDE Plasma and other Wayland desktops): the desktop shares a monitor as a
/// PipeWire stream, GStreamer's pipewiresrc turns it into raw frames, and ShareX crops the region and hands the frames to FFmpeg
/// as YUV4MPEG through a named pipe. Everything starts in <see cref="Start"/>, so building FFmpeg's arguments (the FFmpeg
/// options window shows them) never asks the desktop to share the screen.
/// </summary>
internal sealed partial class PipeWireRecordingSource : IScreenRecordingSource
{
    private readonly ICommandRunner runner;
    private readonly ScreenRecordingRequest request;
    private PortalScreenCast? screenCast;
    private Process? gstreamer;
    private Thread? copier;
    private volatile bool disposed;

    public PipeWireRecordingSource(ICommandRunner runner, ScreenRecordingRequest request, string pipePath)
    {
        this.runner = runner;
        this.request = request;
        PipePath = pipePath;
    }

    public string PipePath { get; }

    public void Start()
    {
        screenCast = Task.Run(() => PortalScreenCast.StartAsync(request.DrawCursor, CancellationToken.None)).GetAwaiter().GetResult();
        uint node = screenCast.Stream.NodeId;

        PlatformSize frame = ProbeFrameSize(node) ?? throw new InvalidOperationException("The shared screen sent no video frames.");
        PlatformRectangle crop = GetCrop(request, screenCast.Stream, frame);

        if (File.Exists(PipePath))
        {
            File.Delete(PipePath);
        }

        if (LibC.MakeFifo(PipePath, Convert.ToUInt32("600", 8)) != 0)
        {
            throw new Win32Exception(System.Runtime.InteropServices.Marshal.GetLastPInvokeError(), "Cannot create the recording pipe.");
        }

        ProcessStartInfo startInfo = new ProcessStartInfo("gst-launch-1.0")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (string argument in CreatePipelineArguments(node, frame, request.FrameRate))
        {
            startInfo.ArgumentList.Add(argument);
        }

        gstreamer = Process.Start(startInfo) ?? throw new InvalidOperationException("GStreamer could not be started.");
        gstreamer.ErrorDataReceived += (_, _) => { };
        gstreamer.BeginErrorReadLine();

        Stream frames = gstreamer.StandardOutput.BaseStream;
        int fps = Math.Max(1, request.FrameRate);
        copier = new Thread(() => CopyFrames(frames, frame, crop, fps)) { IsBackground = true, Name = "ShareX PipeWire frames" };
        copier.Start();
    }

    /// <summary>Raw I420 at a constant frame rate; pipewiresrc only sends frames when the screen changes.</summary>
    internal static IReadOnlyList<string> CreatePipelineArguments(uint node, PlatformSize frame, int frameRate) =>
    [
        "-q",
        "pipewiresrc", $"path={node.ToString(CultureInfo.InvariantCulture)}", "do-timestamp=true", "keepalive-time=1000", "always-copy=true", "!",
        "videorate", "!",
        "videoconvert", "!",
        string.Create(CultureInfo.InvariantCulture,
            $"video/x-raw,format=I420,width={frame.Width},height={frame.Height},framerate={Math.Max(1, frameRate)}/1"), "!",
        "fdsink", "fd=1"
    ];

    /// <summary>
    /// The region in stream pixels, rounded to even numbers as I420 requires. Region and stream position are in compositor
    /// coordinates; on scaled outputs the stream has more pixels than that.
    /// </summary>
    internal static PlatformRectangle GetCrop(ScreenRecordingRequest request, ScreenCastStream stream, PlatformSize frame)
    {
        PlatformRectangle full = new PlatformRectangle(0, 0, frame.Width & ~1, frame.Height & ~1);

        if (request.Region.IsEmpty || stream.Position is not PlatformPoint origin || stream.Size is not PlatformSize size || size.Width <= 0 || size.Height <= 0)
        {
            return full;
        }

        double scaleX = frame.Width / (double)size.Width;
        double scaleY = frame.Height / (double)size.Height;
        int left = (int)Math.Round((request.Region.X - origin.X) * scaleX) & ~1;
        int top = (int)Math.Round((request.Region.Y - origin.Y) * scaleY) & ~1;
        int right = (int)Math.Round((request.Region.Right - origin.X) * scaleX);
        int bottom = (int)Math.Round((request.Region.Bottom - origin.Y) * scaleY);
        PlatformRectangle crop = PlatformRectangle.FromLTRB(left, top, right, bottom).Intersect(full);

        return crop.IsEmpty ? full : crop with { Width = crop.Width & ~1, Height = crop.Height & ~1 };
    }

    private PlatformSize? ProbeFrameSize(uint node)
    {
        try
        {
            CommandResult result = Task.Run(() => runner.RunAsync("gst-launch-1.0",
                ["-v", "pipewiresrc", $"path={node.ToString(CultureInfo.InvariantCulture)}", "num-buffers=1", "!", "videoconvert", "!", "fakesink"],
                timeout: TimeSpan.FromSeconds(15))).GetAwaiter().GetResult();
            return ParseFrameSize(result.StandardOutputText + "\n" + result.StandardError);
        }
        catch (Exception e) when (e is TimeoutException or Win32Exception or InvalidOperationException)
        {
            return null;
        }
    }

    /// <summary>The first negotiated "width=(int)W, height=(int)H" in gst-launch -v output.</summary>
    internal static PlatformSize? ParseFrameSize(string output)
    {
        Match match = CapsSizeRegex().Match(output);
        return match.Success
            ? new PlatformSize(int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture))
            : null;
    }

    [GeneratedRegex(@"video/x-raw[^\n]*?width=\(int\)(\d+), height=\(int\)(\d+)")]
    private static partial Regex CapsSizeRegex();

    /// <summary>Reads I420 frames from GStreamer and writes the cropped frames to the pipe as YUV4MPEG until FFmpeg stops reading.</summary>
    private void CopyFrames(Stream frames, PlatformSize frame, PlatformRectangle crop, int fps)
    {
        try
        {
            using FileStream pipe = new FileStream(PipePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite, 1 << 20);
            byte[] header = Encoding.ASCII.GetBytes(string.Create(CultureInfo.InvariantCulture,
                $"YUV4MPEG2 W{crop.Width} H{crop.Height} F{fps}:1 Ip A1:1 C420jpeg\n"));
            pipe.Write(header);

            byte[] input = new byte[frame.Width * frame.Height * 3 / 2];
            byte[] output = new byte[crop.Width * crop.Height * 3 / 2];
            byte[] marker = "FRAME\n"u8.ToArray();

            while (!disposed)
            {
                frames.ReadExactly(input);
                CropI420(input, frame, crop, output);
                pipe.Write(marker);
                pipe.Write(output);
            }
        }
        catch (Exception e) when (e is IOException or EndOfStreamException or ObjectDisposedException or UnauthorizedAccessException)
        {
            // FFmpeg finished (stop or duration) or GStreamer ended.
        }
    }

    internal static void CropI420(byte[] input, PlatformSize frame, PlatformRectangle crop, byte[] output)
    {
        int offset = 0;

        // Y plane, then the half resolution U and V planes.
        for (int y = 0; y < crop.Height; y++)
        {
            Buffer.BlockCopy(input, (crop.Y + y) * frame.Width + crop.X, output, offset, crop.Width);
            offset += crop.Width;
        }

        int chromaWidth = frame.Width / 2;
        int chromaHeight = frame.Height / 2;

        for (int plane = 0; plane < 2; plane++)
        {
            int planeStart = frame.Width * frame.Height + plane * chromaWidth * chromaHeight;

            for (int y = 0; y < crop.Height / 2; y++)
            {
                Buffer.BlockCopy(input, planeStart + (crop.Y / 2 + y) * chromaWidth + crop.X / 2, output, offset, crop.Width / 2);
                offset += crop.Width / 2;
            }
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        if (gstreamer != null)
        {
            try
            {
                if (!gstreamer.HasExited)
                {
                    LibC.Kill(gstreamer.Id, LibC.SIGINT);

                    if (!gstreamer.WaitForExit(2000))
                    {
                        gstreamer.Kill();
                    }
                }
            }
            catch (InvalidOperationException)
            {
            }

            gstreamer.Dispose();
        }

        copier?.Join(TimeSpan.FromSeconds(2));

        if (screenCast != null)
        {
            Task.Run(() => screenCast.DisposeAsync().AsTask()).Wait(TimeSpan.FromSeconds(2));
        }

        try
        {
            File.Delete(PipePath);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
        }
    }
}

/// <summary>GNOME, KDE Plasma and other Wayland desktops: the ScreenCast portal and GStreamer's PipeWire plugin feed FFmpeg.</summary>
internal sealed class PortalRecordingBackend(ICommandRunner runner, LinuxDistribution distribution) : IScreenRecordingBackend
{
    public const string DeviceName = "pipewire";

    private FeatureSupport? support;

    public string Device => DeviceName;

    public FeatureSupport Support => support ??= DetectSupport();

    private FeatureSupport DetectSupport()
    {
        uint? version = DBusSession.IsAvailable ? DBusSession.RunSync(() => PortalScreenCast.GetVersionAsync(), TimeSpan.FromSeconds(3)) : null;

        if (version == null)
        {
            return FeatureSupport.NotSupported("This desktop's xdg-desktop-portal does not offer screen sharing (ScreenCast).");
        }

        bool pipewire = runner.Exists("gst-launch-1.0") && runner.Exists("gst-inspect-1.0") &&
            Task.Run(() => runner.RunAsync("gst-inspect-1.0", ["pipewiresrc"], timeout: TimeSpan.FromSeconds(5))).GetAwaiter().GetResult().Success;

        return pipewire ? FeatureSupport.Supported : LinuxPackages.Missing(distribution, LinuxTool.GStreamerPipeWire);
    }

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request)
    {
        string directory = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR") is { Length: > 0 } runtime ? runtime : Path.GetTempPath();
        string pipe = Path.Combine(directory, $"sharex-recording-{Guid.NewGuid():N}.y4m");
        return new FFmpegVideoInput(DeviceName, $"-thread_queue_size 1024 -f yuv4mpegpipe -i \"{pipe}\"", Array.Empty<string>())
        {
            Source = new PipeWireRecordingSource(runner, request, pipe)
        };
    }
}
