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
        Dictionary<uint, (CoreGraphics.CGRect Frame, PlatformRectangle Edges, int ObscuredTop)> insets;

        try
        {
            insets = ReadNSScreens();
        }
        catch (Exception e) when (e is PlatformNotSupportedException or EntryPointNotFoundException)
        {
            // Without AppKit the screens are still listed, with their whole bounds as the working area.
            insets = new Dictionary<uint, (CoreGraphics.CGRect, PlatformRectangle, int)>();
        }

        for (int i = 0; i < count; i++)
        {
            CoreGraphics.CGRect bounds = CoreGraphics.CGDisplayBounds(displays[i]);
            double scale = GetBackingScale(displays[i]);
            PlatformRectangle rectangle = bounds.ToRectangle();
            // screencapture -D numbers displays from 1 in CGGetActiveDisplayList order.
            string id = (i + 1).ToString(CultureInfo.InvariantCulture);
            PlatformRectangle workingArea = rectangle;
            int obscuredTop = 0;

            if (insets.TryGetValue(displays[i], out (CoreGraphics.CGRect Frame, PlatformRectangle Edges, int ObscuredTop) inset))
            {
                workingArea = ApplyInsets(rectangle, inset.Edges);
                obscuredTop = Math.Clamp(inset.ObscuredTop, 0, rectangle.Height / 2);
            }

            screens.Add(new ScreenInfo(id, $"Display {id}", rectangle, workingArea, displays[i] == main, Math.Max(1, scale)) { ObscuredTop = obscuredTop });
        }

        return screens;
    }

    /// <summary>
    /// For each display: its frame in AppKit's coordinates, how far the menu bar and the Dock reach in from its left, top,
    /// right and bottom edges (held in a rectangle as X, Y, Width and Height), and the height of its camera housing. Core
    /// Graphics knows neither of the last two, so they come from AppKit's NSScreen.
    /// </summary>
    internal static Dictionary<uint, (CoreGraphics.CGRect Frame, PlatformRectangle Edges, int ObscuredTop)> ReadNSScreens() => ObjC.WithAutoreleasePool(() =>
    {
        Dictionary<uint, (CoreGraphics.CGRect, PlatformRectangle, int)> result = new Dictionary<uint, (CoreGraphics.CGRect, PlatformRectangle, int)>();
        IntPtr screens = ObjC.Send(ObjC.GetClass("NSScreen"), "screens");

        if (screens == IntPtr.Zero)
        {
            return result;
        }

        IntPtr key = CoreFoundation.CreateString("NSScreenNumber");

        try
        {
            nint count = ObjC.SendNInt(screens, "count");

            for (nint i = 0; i < count; i++)
            {
                IntPtr screen = ObjC.Send(screens, "objectAtIndex:", (IntPtr)i);
                IntPtr number = ObjC.Send(ObjC.Send(screen, "deviceDescription"), "objectForKey:", key);

                if (number == IntPtr.Zero)
                {
                    continue;
                }

                CoreGraphics.CGRect frame = ObjC.SendRect(screen, "frame");
                PlatformRectangle edges = GetEdgeInsets(frame, ObjC.SendRect(screen, "visibleFrame"));
                // safeAreaInsets is macOS 12 and later.
                double top = ObjC.RespondsTo(screen, "safeAreaInsets") ? ObjC.SendEdgeInsets(screen, "safeAreaInsets").Top : 0;
                result[ObjC.SendUInt32(number, "unsignedIntValue")] = (frame, edges, double.IsFinite(top) ? (int)Math.Ceiling(top) : 0);
            }
        }
        finally
        {
            CoreFoundation.CFRelease(key);
        }

        return result;
    });

    /// <summary>
    /// The distance of <paramref name="visible"/> from each edge of <paramref name="frame"/> as X (left), Y (top), Width (right)
    /// and Height (bottom). AppKit's rectangles have their origin at the bottom left, so the top inset is measured from the
    /// frame's upper edge.
    /// </summary>
    internal static PlatformRectangle GetEdgeInsets(CoreGraphics.CGRect frame, CoreGraphics.CGRect visible)
    {
        static int Inset(double value) => double.IsFinite(value) ? Math.Max(0, (int)Math.Round(value)) : 0;

        return new PlatformRectangle(
            Inset(visible.X - frame.X),
            Inset(frame.Y + frame.Height - (visible.Y + visible.Height)),
            Inset(frame.X + frame.Width - (visible.X + visible.Width)),
            Inset(visible.Y - frame.Y));
    }

    /// <summary><paramref name="bounds"/> without the edges <see cref="GetEdgeInsets"/> measured, or the bounds when nothing would be left.</summary>
    internal static PlatformRectangle ApplyInsets(PlatformRectangle bounds, PlatformRectangle edges)
    {
        int width = bounds.Width - edges.X - edges.Width;
        int height = bounds.Height - edges.Y - edges.Height;
        return width > 0 && height > 0 ? new PlatformRectangle(bounds.X + edges.X, bounds.Y + edges.Y, width, height) : bounds;
    }

    /// <summary>
    /// Pixels per point of a display: the current mode's backing width over its point width. CGDisplayPixelsWide reports points on
    /// Retina displays, which made the factor 1 and cropped recordings in points from a pixel stream.
    /// </summary>
    internal static double GetBackingScale(uint display)
    {
        IntPtr mode = CoreGraphics.CGDisplayCopyDisplayMode(display);

        if (mode == IntPtr.Zero)
        {
            return 1;
        }

        try
        {
            return ComputeScale(CoreGraphics.CGDisplayModeGetPixelWidth(mode), CoreGraphics.CGDisplayModeGetWidth(mode));
        }
        finally
        {
            CoreGraphics.CGDisplayModeRelease(mode);
        }
    }

    internal static double ComputeScale(nuint pixelWidth, nuint pointWidth) =>
        pixelWidth > 0 && pointWidth > 0 ? Math.Max(1, (double)pixelWidth / pointWidth) : 1;

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
