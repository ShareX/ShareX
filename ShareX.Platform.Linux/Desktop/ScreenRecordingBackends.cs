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
using System.IO;
using System.Linq;

namespace ShareX.Platform.Linux.Desktop;

/// <summary>How one kind of Linux desktop feeds the screen to FFmpeg.</summary>
internal interface IScreenRecordingBackend
{
    /// <summary>The FFmpeg capture device name ShareX's recording options select.</summary>
    string Device { get; }

    FeatureSupport Support { get; }

    FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request);
}

/// <summary>X11: FFmpeg's x11grab reads the display directly.</summary>
internal sealed class X11GrabRecordingBackend : IScreenRecordingBackend
{
    public string Device => "x11grab";

    public FeatureSupport Support => FeatureSupport.Supported;

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request)
    {
        PlatformRectangle fallback = PlatformRectangle.Empty;

        if (request.Region.IsEmpty && request.Screen == null)
        {
            using X11Display? display = X11Display.TryOpen();
            fallback = display?.GetRootBounds() ?? throw new InvalidOperationException("Cannot open the X11 display.");
        }

        return LinuxScreenRecordingService.CreateX11GrabInput(request, Environment.GetEnvironmentVariable("DISPLAY") ?? ":0", fallback);
    }
}

/// <summary>Hyprland, sway and other wlroots compositors: wf-recorder writes the region into a pipe FFmpeg reads.</summary>
internal sealed class WfRecorderRecordingBackend(ICommandRunner runner, LinuxDistribution distribution, Func<IReadOnlyList<ScreenInfo>> getScreens) : IScreenRecordingBackend
{
    public string Device => LinuxScreenRecordingService.WfRecorderDevice;

    public FeatureSupport Support => runner.Exists(Device) ? FeatureSupport.Supported : LinuxPackages.Missing(distribution, LinuxTool.WfRecorder);

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request)
    {
        PlatformRectangle desktop = getScreens().Select(s => s.Bounds).Aggregate(PlatformRectangle.Empty, (a, b) => a.Union(b));
        string directory = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR") is { Length: > 0 } runtime ? runtime : Path.GetTempPath();
        return LinuxScreenRecordingService.CreateWfRecorderInput(request, Path.Combine(directory, $"sharex-recording-{Guid.NewGuid():N}.mkv"), desktop);
    }
}

/// <summary>Desktops ShareX cannot record yet.</summary>
internal sealed class UnsupportedRecordingBackend(string reason) : IScreenRecordingBackend
{
    public string Device => "";

    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request) => throw new PlatformNotSupportedException(Support.Reason);
}
