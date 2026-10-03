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

namespace ShareX.Platform.Linux;

/// <summary>FFmpeg x11grab input for X11 sessions.</summary>
/// <remarks>
/// Wayland compositors do not allow x11grab to see native Wayland windows. Recording there needs the xdg-desktop-portal
/// ScreenCast interface feeding a PipeWire stream, which is tracked as follow up work.
/// </remarks>
public sealed class LinuxScreenRecordingService : IScreenRecordingService
{
    private readonly PlatformInfo info;
    private readonly ICommandRunner runner;

    public LinuxScreenRecordingService(PlatformInfo info, ICommandRunner runner)
    {
        this.info = info;
        this.runner = runner;
    }

    public FeatureSupport Support
    {
        get
        {
            if (!runner.Exists("ffmpeg"))
            {
                return LinuxPackages.Missing(info.Distribution ?? LinuxDistribution.Unknown, LinuxTool.FFmpeg);
            }

            if (info.IsWayland)
            {
                return FeatureSupport.NotSupported("Screen recording on Wayland requires the xdg-desktop-portal ScreenCast interface, which is not implemented yet. Log in to an X11 session to record.");
            }

            return info.IsX11 ? FeatureSupport.Supported : FeatureSupport.NotSupported("No graphical session was found.");
        }
    }

    public IReadOnlyList<string> GetSupportedDevices() => info.IsX11 ? ["x11grab"] : Array.Empty<string>();

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request)
    {
        PlatformRectangle fallback = PlatformRectangle.Empty;

        if (request.Region.IsEmpty && request.Screen == null)
        {
            using X11Display? display = X11Display.TryOpen();
            fallback = display?.GetRootBounds() ?? throw new InvalidOperationException("Cannot open the X11 display.");
        }

        return CreateX11GrabInput(request, Environment.GetEnvironmentVariable("DISPLAY") ?? ":0", fallback);
    }

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
