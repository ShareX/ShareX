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
using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ShareX.Platform.Linux;

/// <summary>FFmpeg x11grab on X11; wf-recorder feeding FFmpeg on Hyprland, sway and other wlroots compositors.</summary>
/// <remarks>
/// GNOME and KDE on Wayland need the xdg-desktop-portal ScreenCast interface feeding a PipeWire stream, which is follow up work.
/// </remarks>
public sealed class LinuxScreenRecordingService : IScreenRecordingService
{
    public const string WfRecorderDevice = "wf-recorder";

    private readonly PlatformInfo info;
    private readonly ICommandRunner runner;
    private readonly Func<IReadOnlyList<ScreenInfo>> getScreens;

    public LinuxScreenRecordingService(PlatformInfo info, ICommandRunner runner, Func<IReadOnlyList<ScreenInfo>> getScreens)
    {
        this.info = info;
        this.runner = runner;
        this.getScreens = getScreens;
    }

    private LinuxDistribution Distribution => info.Distribution ?? LinuxDistribution.Unknown;

    public FeatureSupport Support
    {
        get
        {
            if (!runner.Exists("ffmpeg"))
            {
                return LinuxPackages.Missing(Distribution, LinuxTool.FFmpeg);
            }

            if (info.IsWayland)
            {
                if (!info.IsWlrootsCompositor)
                {
                    return FeatureSupport.NotSupported("Screen recording on GNOME and KDE Wayland needs the xdg-desktop-portal ScreenCast interface, which ShareX does not use yet. Log in to an X11 session to record.");
                }

                return runner.Exists(WfRecorderDevice) ? FeatureSupport.Supported : LinuxPackages.Missing(Distribution, LinuxTool.WfRecorder);
            }

            return info.IsX11 ? FeatureSupport.Supported : FeatureSupport.NotSupported("No graphical session was found.");
        }
    }

    public IReadOnlyList<string> GetSupportedDevices()
    {
        if (info.IsWayland)
        {
            return info.IsWlrootsCompositor && runner.Exists(WfRecorderDevice) ? [WfRecorderDevice] : Array.Empty<string>();
        }

        return info.IsX11 ? ["x11grab"] : Array.Empty<string>();
    }

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request)
    {
        if (info.IsWayland)
        {
            PlatformRectangle desktop = getScreens().Select(s => s.Bounds).Aggregate(PlatformRectangle.Empty, (a, b) => a.Union(b));
            string directory = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR") is { Length: > 0 } runtime ? runtime : Path.GetTempPath();
            return CreateWfRecorderInput(request, Path.Combine(directory, $"sharex-recording-{Guid.NewGuid():N}.mkv"), desktop);
        }

        PlatformRectangle fallback = PlatformRectangle.Empty;

        if (request.Region.IsEmpty && request.Screen == null)
        {
            using X11Display? display = X11Display.TryOpen();
            fallback = display?.GetRootBounds() ?? throw new InvalidOperationException("Cannot open the X11 display.");
        }

        return CreateX11GrabInput(request, Environment.GetEnvironmentVariable("DISPLAY") ?? ":0", fallback);
    }

    /// <summary>The region is in layout coordinates, as grim and wf-recorder take it; the video has the monitor's pixels.</summary>
    internal static FFmpegVideoInput CreateWfRecorderInput(ScreenRecordingRequest request, string pipePath, PlatformRectangle desktop)
    {
        PlatformRectangle region = request.GetEffectiveRegion(desktop);
        WfRecorderSource source = new WfRecorderSource(pipePath, region, request.FrameRate);
        string arguments = $"-thread_queue_size 1024 -f matroska -i \"{pipePath}\"";
        // Encoders that need even sizes get them whatever the monitor scale makes of the region.
        IReadOnlyList<string> filters = request.RequireEvenSize ? ["crop=trunc(iw/2)*2:trunc(ih/2)*2"] : Array.Empty<string>();
        return new FFmpegVideoInput(WfRecorderDevice, arguments, filters) { Source = source };
    }

    public string GetDefaultFFmpegPath(string applicationDirectory) => UnixFFmpegLocator.Find(applicationDirectory, ["/usr/bin", "/usr/local/bin"]);

    internal static FFmpegVideoInput CreateX11GrabInput(ScreenRecordingRequest request, string display, PlatformRectangle fallback)
    {
        PlatformRectangle region = request.GetEffectiveRegion(fallback);

        // https://ffmpeg.org/ffmpeg-devices.html#x11grab
        string arguments = string.Create(CultureInfo.InvariantCulture,
            $"-f x11grab -thread_queue_size 1024 -framerate {request.FrameRate} -draw_mouse {(request.DrawCursor ? 1 : 0)} " +
            $"-video_size {region.Width}x{region.Height} -i {display}+{region.X},{region.Y}");

        return new FFmpegVideoInput("x11grab", arguments, Array.Empty<string>());
    }

    public void PrepareDevice(string device, ScreenRecordingRequest request)
    {
    }
}
