using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ShareX.Platform.Windows;

/// <summary>FFmpeg gdigrab and ddagrab inputs, with the same arguments ShareX.ScreenCaptureLib builds.</summary>
public sealed class WindowsScreenRecordingService : IScreenRecordingService
{
    public const string GdiGrab = "gdigrab";
    public const string DdaGrab = "ddagrab";

    private readonly Func<IReadOnlyList<ScreenInfo>> getScreens;

    public WindowsScreenRecordingService(Func<IReadOnlyList<ScreenInfo>> getScreens)
    {
        this.getScreens = getScreens;
    }

    /// <summary>The device used by <see cref="CreateVideoInput"/>. Defaults to gdigrab, ShareX's default.</summary>
    public string Device { get; set; } = GdiGrab;

    public FeatureSupport Support => FeatureSupport.Supported;

    public IReadOnlyList<string> GetSupportedDevices() => [GdiGrab, DdaGrab];

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request)
    {
        IReadOnlyList<ScreenInfo> screens = getScreens();
        return Device == DdaGrab ? CreateDdaGrabInput(request, screens) : CreateGdiGrabInput(request, WindowsScreenCaptureService.GetVirtualScreen());
    }

    internal static FFmpegVideoInput CreateGdiGrabInput(ScreenRecordingRequest request, PlatformRectangle virtualScreen)
    {
        PlatformRectangle region = request.GetEffectiveRegion(virtualScreen);

        // https://ffmpeg.org/ffmpeg-devices.html#gdigrab
        string arguments = string.Create(CultureInfo.InvariantCulture,
            $"-f gdigrab -thread_queue_size 1024 -rtbufsize 256M -framerate {request.FrameRate} -offset_x {region.X} -offset_y {region.Y} " +
            $"-video_size {region.Width}x{region.Height} -draw_mouse {(request.DrawCursor ? 1 : 0)} -i desktop");

        return new FFmpegVideoInput(GdiGrab, arguments, Array.Empty<string>());
    }

    internal static FFmpegVideoInput CreateDdaGrabInput(ScreenRecordingRequest request, IReadOnlyList<ScreenInfo> screens)
    {
        // Desktop Duplication captures one output. Pick the one that overlaps the region most, primary first.
        ScreenInfo[] ordered = screens.OrderBy(s => !s.IsPrimary).ToArray();

        if (ordered.Length == 0)
        {
            throw new InvalidOperationException("No displays were found.");
        }

        int outputIndex = 0;
        PlatformRectangle target = request.Region.IsEmpty ? request.Screen?.Bounds ?? ordered[0].Bounds : request.Region;
        PlatformRectangle captureArea = ordered[0].Bounds.Offset(-ordered[0].Bounds.X, -ordered[0].Bounds.Y);
        long best = 0;

        for (int i = 0; i < ordered.Length; i++)
        {
            PlatformRectangle intersection = ordered[i].Bounds.Intersect(target);
            long area = intersection.IsEmpty ? 0 : (long)intersection.Width * intersection.Height;

            if (area > best)
            {
                best = area;
                outputIndex = i;
                captureArea = intersection.Offset(-ordered[i].Bounds.X, -ordered[i].Bounds.Y);
            }
        }

        if (request.RequireEvenSize)
        {
            captureArea = captureArea with { Width = captureArea.Width & ~1, Height = captureArea.Height & ~1 };
        }

        // https://ffmpeg.org/ffmpeg-filters.html#ddagrab
        string arguments = string.Create(CultureInfo.InvariantCulture,
            $"-f lavfi -i ddagrab=output_idx={outputIndex}:draw_mouse={(request.DrawCursor ? "true" : "false")}:framerate={request.FrameRate}:" +
            $"offset_x={captureArea.X}:offset_y={captureArea.Y}:video_size={captureArea.Width}x{captureArea.Height}:output_fmt=bgra");

        // Frames stay on the GPU. Software encoders need them downloaded.
        return new FFmpegVideoInput(DdaGrab, arguments, ["hwdownload", "format=bgra"]);
    }
}
