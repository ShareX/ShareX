using ShareX.Platform.Imaging;
using ShareX.Platform.Windows.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Windows;

/// <summary>GDI screen capture (BitBlt with CAPTUREBLT), matching ShareX's default capture method.</summary>
/// <remarks>Coordinates are physical pixels when the process is per monitor DPI aware, as the ShareX manifest declares.</remarks>
public sealed unsafe class WindowsScreenCaptureService : IScreenCaptureService
{
    public FeatureSupport Support => FeatureSupport.Supported;

    public PermissionState GetPermissionState() => PermissionState.NotRequired;

    public bool RequestPermission() => true;

    public static PlatformRectangle GetVirtualScreen() => new PlatformRectangle(
        Win32.GetSystemMetrics(Win32.SM_XVIRTUALSCREEN), Win32.GetSystemMetrics(Win32.SM_YVIRTUALSCREEN),
        Win32.GetSystemMetrics(Win32.SM_CXVIRTUALSCREEN), Win32.GetSystemMetrics(Win32.SM_CYVIRTUALSCREEN));

    public IReadOnlyList<ScreenInfo> GetScreens()
    {
        List<ScreenInfo> screens = new List<ScreenInfo>();
        GCHandle handle = GCHandle.Alloc(screens);

        try
        {
            Win32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, &OnMonitor, GCHandle.ToIntPtr(handle));
        }
        finally
        {
            handle.Free();
        }

        return screens;
    }

    [UnmanagedCallersOnly]
    private static int OnMonitor(IntPtr monitor, IntPtr hdc, Win32.RECT* rect, nint data)
    {
        List<ScreenInfo> screens = (List<ScreenInfo>)GCHandle.FromIntPtr(data).Target!;
        Win32.MONITORINFOEX info = new Win32.MONITORINFOEX { cbSize = sizeof(Win32.MONITORINFOEX) };

        if (Win32.GetMonitorInfo(monitor, &info))
        {
            string device = new string(info.szDevice);
            double scale = 1;

            try
            {
                // MDT_EFFECTIVE_DPI
                if (Win32.GetDpiForMonitor(monitor, 0, out uint dpiX, out _) == 0)
                {
                    scale = dpiX / 96.0;
                }
            }
            catch (EntryPointNotFoundException)
            {
            }

            screens.Add(new ScreenInfo(device, device, info.rcMonitor.ToRectangle(), info.rcWork.ToRectangle(),
                (info.dwFlags & Win32.MONITORINFOF_PRIMARY) != 0, scale));
        }

        return 1;
    }

    public Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, CancellationToken cancellationToken = default)
    {
        PlatformRectangle area = request.Mode switch
        {
            ScreenCaptureMode.Region => request.Region,
            ScreenCaptureMode.Screen => GetScreens().FirstOrDefault(s => s.Id == request.ScreenId)?.Bounds
                ?? throw new ArgumentException($"Unknown screen '{request.ScreenId}'.", nameof(request)),
            ScreenCaptureMode.Interactive => throw new NotSupportedException("Windows has no system region picker. Use ShareX's region capture."),
            _ => GetVirtualScreen()
        };

        area = area.Intersect(GetVirtualScreen());

        if (area.IsEmpty)
        {
            throw new ArgumentException("The capture area is outside the screen.", nameof(request));
        }

        PixelBuffer pixels = Capture(area, request.IncludeCursor);
        return Task.FromResult(new ScreenCaptureResult(PngCodec.Encode(pixels), area, "GDI"));
    }

    public static PixelBuffer Capture(PlatformRectangle area, bool includeCursor)
    {
        IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
        IntPtr memoryDc = Win32.CreateCompatibleDC(screenDc);
        Win32.BITMAPINFOHEADER header = new Win32.BITMAPINFOHEADER
        {
            biSize = (uint)sizeof(Win32.BITMAPINFOHEADER),
            biWidth = area.Width,
            biHeight = -area.Height, // Top down
            biPlanes = 1,
            biBitCount = 32,
            biCompression = Win32.BI_RGB
        };

        IntPtr bitmap = Win32.CreateDIBSection(screenDc, &header, Win32.DIB_RGB_COLORS, out IntPtr bits, IntPtr.Zero, 0);

        if (bitmap == IntPtr.Zero)
        {
            Win32.DeleteDC(memoryDc);
            Win32.ReleaseDC(IntPtr.Zero, screenDc);
            throw new InvalidOperationException("CreateDIBSection failed.");
        }

        IntPtr previous = Win32.SelectObject(memoryDc, bitmap);

        try
        {
            if (!Win32.BitBlt(memoryDc, 0, 0, area.Width, area.Height, screenDc, area.X, area.Y, Win32.SRCCOPY | Win32.CAPTUREBLT))
            {
                throw new InvalidOperationException("BitBlt failed.");
            }

            if (includeCursor)
            {
                DrawCursor(memoryDc, area);
            }

            Win32.GdiFlush();
            return PixelBuffer.FromBgra(bits, area.Width, area.Height, area.Width * 4, forceOpaque: true);
        }
        finally
        {
            Win32.SelectObject(memoryDc, previous);
            Win32.DeleteObject(bitmap);
            Win32.DeleteDC(memoryDc);
            Win32.ReleaseDC(IntPtr.Zero, screenDc);
        }
    }

    private static void DrawCursor(IntPtr hdc, PlatformRectangle area)
    {
        Win32.CURSORINFO cursor = new Win32.CURSORINFO { cbSize = sizeof(Win32.CURSORINFO) };

        if (!Win32.GetCursorInfo(&cursor) || cursor.flags != Win32.CURSOR_SHOWING || !Win32.GetIconInfo(cursor.hCursor, out Win32.ICONINFO icon))
        {
            return;
        }

        try
        {
            Win32.DrawIconEx(hdc, cursor.ptScreenPos.X - icon.xHotspot - area.X, cursor.ptScreenPos.Y - icon.yHotspot - area.Y,
                cursor.hCursor, 0, 0, 0, IntPtr.Zero, Win32.DI_NORMAL);
        }
        finally
        {
            if (icon.hbmMask != IntPtr.Zero) Win32.DeleteObject(icon.hbmMask);
            if (icon.hbmColor != IntPtr.Zero) Win32.DeleteObject(icon.hbmColor);
        }
    }
}
