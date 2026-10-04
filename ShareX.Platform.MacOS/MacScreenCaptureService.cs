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
using ShareX.Platform.Imaging;
using ShareX.Platform.MacOS.Native;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.MacOS;

/// <summary>Screen capture with the system screencapture tool, which uses ScreenCaptureKit on current macOS releases.</summary>
/// <remarks>
/// Coordinates are in points (the global display coordinate space). Images are captured at the display's native resolution,
/// so a Retina capture is twice the size of its bounds. macOS asks for the Screen Recording permission on first use.
/// </remarks>
public sealed class MacScreenCaptureService : IScreenCaptureService
{
    private readonly ICommandRunner runner;
    private readonly IWindowService? windows;

    public MacScreenCaptureService(ICommandRunner runner, IWindowService? windows = null)
    {
        this.runner = runner;
        this.windows = windows;
    }

    /// <summary>screencapture -l captures a window by its CGWindowID, with its transparency and, unless -o is given, its shadow.</summary>
    public ScreenCaptureFeatures Features => ScreenCaptureFeatures.Window | ScreenCaptureFeatures.TransparentWindow;

    private const string PermissionReason =
        "Allow ShareX in System Settings > Privacy & Security > Screen & System Audio Recording, then restart ShareX.";

    private volatile bool permissionRequested;

    /// <summary>
    /// Supported until ShareX has asked for the permission in this run and been refused: macOS cannot tell "not asked yet" from
    /// "denied" without prompting, and the first capture is what shows the prompt.
    /// </summary>
    public FeatureSupport Support => GetPermissionState() == PermissionState.Denied
        ? FeatureSupport.NotSupported(PermissionReason)
        : FeatureSupport.Supported;

    public PermissionState GetPermissionState() => CoreGraphics.CGPreflightScreenCaptureAccess() ? PermissionState.Granted :
        permissionRequested ? PermissionState.Denied : PermissionState.Unknown;

    /// <summary>Shows macOS's prompt the first time; afterwards it only reports the answer, which takes effect after a restart.</summary>
    public bool RequestPermission()
    {
        permissionRequested = true;
        return CoreGraphics.CGRequestScreenCaptureAccess();
    }

    public unsafe IReadOnlyList<ScreenInfo> GetScreens()
    {
        uint* displays = stackalloc uint[32];

        if (CoreGraphics.CGGetActiveDisplayList(32, displays, out uint count) != 0)
        {
            return Array.Empty<ScreenInfo>();
        }

        uint main = CoreGraphics.CGMainDisplayID();
        List<ScreenInfo> screens = new List<ScreenInfo>((int)count);

        for (int i = 0; i < count; i++)
        {
            CoreGraphics.CGRect bounds = CoreGraphics.CGDisplayBounds(displays[i]);
            double scale = bounds.Width > 0 ? CoreGraphics.CGDisplayPixelsWide(displays[i]) / bounds.Width : 1;
            PlatformRectangle rectangle = bounds.ToRectangle();
            // screencapture -D numbers displays from 1 in CGGetActiveDisplayList order.
            string id = (i + 1).ToString(CultureInfo.InvariantCulture);
            screens.Add(new ScreenInfo(id, $"Display {id}", rectangle, rectangle, displays[i] == main, Math.Max(1, scale)));
        }

        return screens;
    }

    public async Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, CancellationToken cancellationToken = default)
    {
        // Without the permission screencapture returns only the wallpaper and ShareX's own windows.
        if (GetPermissionState() != PermissionState.Granted && !RequestPermission())
        {
            throw new PlatformNotSupportedException(PermissionReason);
        }

        IReadOnlyList<ScreenInfo> screens = GetScreens();

        // screencapture only writes one display per file, so span several displays with a region.
        if (request.Mode == ScreenCaptureMode.FullScreen && screens.Count > 1)
        {
            request = request with { Mode = ScreenCaptureMode.Region, Region = screens.Select(s => s.Bounds).Aggregate((a, b) => a.Union(b)) };
        }

        string file = Path.Combine(Path.GetTempPath(), $"sharex-{Guid.NewGuid():N}.png");
        List<string> arguments = CreateArguments(request, file);
        PlatformRectangle bounds = request.Mode switch
        {
            ScreenCaptureMode.Region => request.Region,
            ScreenCaptureMode.Screen => screens.FirstOrDefault(s => s.Id == request.ScreenId)?.Bounds ?? PlatformRectangle.Empty,
            ScreenCaptureMode.FullScreen => screens.FirstOrDefault(s => s.IsPrimary)?.Bounds ?? PlatformRectangle.Empty,
            ScreenCaptureMode.Window => windows?.GetWindows().FirstOrDefault(w => w.Handle == request.WindowHandle)?.Bounds ?? PlatformRectangle.Empty,
            _ => PlatformRectangle.Empty
        };

        try
        {
            TimeSpan timeout = request.Mode == ScreenCaptureMode.Interactive ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(30);
            CommandResult result = await runner.RunAsync("screencapture", arguments, timeout: timeout, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!File.Exists(file))
            {
                if (request.Mode == ScreenCaptureMode.Interactive)
                {
                    throw new OperationCanceledException("The selection was cancelled.");
                }

                throw new InvalidOperationException($"screencapture failed: {result.StandardError.Trim()}");
            }

            byte[] png = await File.ReadAllBytesAsync(file, cancellationToken).ConfigureAwait(false);

            if (bounds.IsEmpty)
            {
                PlatformSize size = PngCodec.ReadSize(png);
                bounds = new PlatformRectangle(0, 0, size.Width, size.Height);
            }

            return new ScreenCaptureResult(png, bounds, "screencapture");
        }
        finally
        {
            TryDelete(file);
        }
    }

    /// <summary>
    /// The cursor shown on screen (NSCursor.currentSystemCursor, whichever application set it) at the resolution of the display under
    /// the pointer. Its position is in points, like the screen bounds, and its image in that display's pixels.
    /// </summary>
    public CursorCapture? CaptureCursor()
    {
        PlatformPoint? pointer = windows?.GetCursorPosition();

        if (pointer is not PlatformPoint location)
        {
            return null;
        }

        double scale = GetScreens().FirstOrDefault(screen => screen.Bounds.Contains(location))?.ScaleFactor ?? 1;

        return ObjC.WithAutoreleasePool(() =>
        {
            IntPtr cursor = ObjC.Send(ObjC.GetClass("NSCursor"), "currentSystemCursor");

            if (AppKitGraphicsService.ReadCursor(cursor, null, scale) is not { } image)
            {
                return null;
            }

            PlatformPoint hotspot = new PlatformPoint((int)Math.Round(image.Hotspot.X / scale), (int)Math.Round(image.Hotspot.Y / scale));
            return new CursorCapture(image.Image, new PlatformPoint(location.X - hotspot.X, location.Y - hotspot.Y));
        });
    }

    internal static List<string> CreateArguments(ScreenCaptureRequest request, string file)
    {
        // -x: no shutter sound. -t png: output format.
        List<string> arguments = new List<string> { "-x", "-t", "png" };

        if (request.IncludeCursor)
        {
            arguments.Add("-C");
        }

        switch (request.Mode)
        {
            case ScreenCaptureMode.Region:
                PlatformRectangle r = request.Region;
                arguments.Add(string.Create(CultureInfo.InvariantCulture, $"-R{r.X},{r.Y},{r.Width},{r.Height}"));
                break;
            case ScreenCaptureMode.Screen:
                arguments.Add("-D");
                arguments.Add(request.ScreenId ?? "1");
                break;
            case ScreenCaptureMode.Interactive:
                arguments.Add("-i");
                break;
            case ScreenCaptureMode.Window:
                arguments.Add(string.Create(CultureInfo.InvariantCulture, $"-l{request.WindowHandle}"));

                // A window keeps its rounded corners transparent either way; the shadow only when asked for.
                if (!request.Window.Transparent || !request.Window.IncludeShadow)
                {
                    arguments.Add("-o");
                }

                break;
            default:
                // The main display. Several displays are captured as a region, see CaptureAsync.
                arguments.Add("-m");
                break;
        }

        arguments.Add(file);
        return arguments;
    }

    private static void TryDelete(string file)
    {
        try
        {
            if (File.Exists(file)) File.Delete(file);
        }
        catch (IOException)
        {
        }
    }
}
