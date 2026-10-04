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
using ShareX.Platform.Linux.Desktop;
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

    private readonly ICommandRunner runner;
    private readonly LinuxDistribution distribution;
    private readonly IScreenRecordingBackend backend;

    public LinuxScreenRecordingService(PlatformInfo info, ICommandRunner runner, Func<IReadOnlyList<ScreenInfo>> getScreens)
    {
        this.runner = runner;
        distribution = info.Distribution ?? LinuxDistribution.Unknown;
        backend = new LinuxDesktop(info).CreateRecordingBackend(runner, getScreens);
    }

    public FeatureSupport Support => runner.Exists("ffmpeg") ? backend.Support : LinuxPackages.Missing(distribution, LinuxTool.FFmpeg);

    public IReadOnlyList<string> GetSupportedDevices() => backend.Support.IsSupported ? [backend.Device] : Array.Empty<string>();

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request) => backend.CreateVideoInput(request);

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

    // The FFmpeg the user chose, or the system's when none was chosen; ShareX does not download one on Linux.
    public FeatureSupport GetFileMediaSupport(string ffmpegPath) =>
        (!string.IsNullOrEmpty(ffmpegPath) && System.IO.File.Exists(ffmpegPath)) || runner.Exists("ffmpeg")
            ? FeatureSupport.Supported
            : LinuxPackages.Missing(distribution, LinuxTool.FFmpeg);

    public FeatureSupport GetDeviceActionSupport(RecordingDeviceAction action) => FeatureSupport.NotSupported(action == RecordingDeviceAction.ListDirectShowDevices
        ? "DirectShow devices exist only on Windows."
        : "The screen-capture-recorder devices are DirectShow filters for Windows.");

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
