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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ShareX.Platform.MacOS;

/// <summary>FFmpeg avfoundation input. The device records a whole display, so regions become a crop filter.</summary>
public sealed class MacScreenRecordingService : IScreenRecordingService
{
    private readonly ICommandRunner runner;
    private readonly Func<IReadOnlyList<ScreenInfo>> getScreens;

    public MacScreenRecordingService(ICommandRunner runner, Func<IReadOnlyList<ScreenInfo>> getScreens)
    {
        this.runner = runner;
        this.getScreens = getScreens;
    }

    public FeatureSupport Support => runner.Exists("ffmpeg")
        ? FeatureSupport.Supported
        : FeatureSupport.RequiresTool("FFmpeg (brew install ffmpeg, or let ShareX download it)");

    public IReadOnlyList<string> GetSupportedDevices() => ["avfoundation"];

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request)
    {
        IReadOnlyList<ScreenInfo> screens = getScreens();
        return CreateAVFoundationInput(request, screens);
    }

    internal static FFmpegVideoInput CreateAVFoundationInput(ScreenRecordingRequest request, IReadOnlyList<ScreenInfo> screens)
    {
        if (screens.Count == 0)
        {
            throw new InvalidOperationException("No displays were found.");
        }

        // Record the display that contains most of the region.
        ScreenInfo screen = request.Screen ?? (request.Region.IsEmpty
            ? screens.FirstOrDefault(s => s.IsPrimary) ?? screens[0]
            : screens.OrderByDescending(s => Area(s.Bounds.Intersect(request.Region))).First());

        int index = 0;

        for (int i = 0; i < screens.Count; i++)
        {
            if (screens[i].Id == screen.Id) index = i;
        }

        // https://ffmpeg.org/ffmpeg-devices.html#avfoundation - screens are listed as "Capture screen N".
        string arguments = string.Create(CultureInfo.InvariantCulture,
            $"-f avfoundation -thread_queue_size 1024 -capture_cursor {(request.DrawCursor ? 1 : 0)} -framerate {request.FrameRate} " +
            $"-pixel_format bgr0 -i \"Capture screen {index}:none\"");

        List<string> filters = new List<string>();
        PlatformRectangle region = request.Region.IsEmpty ? screen.Bounds : request.Region.Intersect(screen.Bounds);

        if (region != screen.Bounds)
        {
            // Region is in points relative to the global space. avfoundation delivers pixels of this display.
            double scale = screen.ScaleFactor;
            PlatformRectangle pixels = new PlatformRectangle(
                (int)Math.Round((region.X - screen.Bounds.X) * scale), (int)Math.Round((region.Y - screen.Bounds.Y) * scale),
                (int)Math.Round(region.Width * scale), (int)Math.Round(region.Height * scale));

            if (request.RequireEvenSize)
            {
                pixels = pixels with { Width = pixels.Width & ~1, Height = pixels.Height & ~1 };
            }

            filters.Add(string.Create(CultureInfo.InvariantCulture, $"crop={pixels.Width}:{pixels.Height}:{pixels.X}:{pixels.Y}"));
        }

        return new FFmpegVideoInput("avfoundation", arguments, filters);
    }

    public void PrepareDevice(string device, ScreenRecordingRequest request)
    {
    }

    private static long Area(PlatformRectangle rectangle) => rectangle.IsEmpty ? 0 : (long)rectangle.Width * rectangle.Height;

    public FeatureSupport GetDeviceActionSupport(RecordingDeviceAction action) => FeatureSupport.NotSupported(action == RecordingDeviceAction.ListDirectShowDevices
        ? "DirectShow devices exist only on Windows."
        : "The screen-capture-recorder devices are DirectShow filters for Windows.");

    public string GetDefaultFFmpegPath(string applicationDirectory) =>
        UnixFFmpegLocator.Find(applicationDirectory, ["/opt/homebrew/bin", "/usr/local/bin", "/opt/local/bin"]);
}
