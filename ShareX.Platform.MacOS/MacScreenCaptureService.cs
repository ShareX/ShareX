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

    public MacScreenCaptureService(ICommandRunner runner)
    {
        this.runner = runner;
    }

    public FeatureSupport Support => GetPermissionState() == PermissionState.Denied
        ? FeatureSupport.NotSupported("Allow ShareX in System Settings > Privacy & Security > Screen & System Audio Recording.")
        : FeatureSupport.Supported;

    public PermissionState GetPermissionState() => CoreGraphics.CGPreflightScreenCaptureAccess() ? PermissionState.Granted : PermissionState.Denied;

    public bool RequestPermission() => CoreGraphics.CGRequestScreenCaptureAccess();

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
