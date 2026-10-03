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

using ShareX.Platform.Imaging;
using ShareX.Platform.Windows.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Windows;

/// <summary>
/// GDI screen capture (BitBlt with CAPTUREBLT), matching ShareX's default capture method, plus the Windows-only extras: window
/// capture with optional transparency and shadow, hiding the task bar, and HDR tone mapping through Desktop Duplication.
/// </summary>
/// <remarks>Coordinates are physical pixels when the process is per monitor DPI aware, as the ShareX manifest declares.</remarks>
public sealed unsafe class WindowsScreenCaptureService : IScreenCaptureService
{
    public FeatureSupport Support => FeatureSupport.Supported;

    public ScreenCaptureFeatures Features => ScreenCaptureFeatures.Window | ScreenCaptureFeatures.WindowClientArea |
        ScreenCaptureFeatures.TransparentWindow | ScreenCaptureFeatures.HideTaskbar | ScreenCaptureFeatures.HdrToneMapping;

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
        if (request.Mode == ScreenCaptureMode.Window)
        {
            return Task.FromResult(CaptureWindow(request));
        }

        PlatformRectangle area = request.Mode switch
        {
            ScreenCaptureMode.Region => request.Region,
            ScreenCaptureMode.Screen => GetScreens().FirstOrDefault(s => s.Id == request.ScreenId)?.Bounds
                ?? throw new ArgumentException($"Unknown screen '{request.ScreenId}'.", nameof(request)),
            ScreenCaptureMode.Interactive => throw new NotSupportedException("Windows has no system region picker. Use ShareX's region capture."),
            _ => GetVirtualScreen()
        };

        return Task.FromResult(CaptureArea(area, request));
    }

    private static ScreenCaptureResult CaptureArea(PlatformRectangle area, ScreenCaptureRequest request)
    {
        if (request.ClipToScreens)
        {
            area = area.Intersect(GetVirtualScreen());
        }

        if (area.IsEmpty)
        {
            throw new ArgumentException("The capture area is outside the screen.", nameof(request));
        }

        PixelBuffer pixels = Capture(area, request.IncludeCursor, request.HdrToneMapping);
        return new ScreenCaptureResult(pixels, area, request.HdrToneMapping ? "GDI+HDR" : "GDI");
    }

    private static ScreenCaptureResult CaptureWindow(ScreenCaptureRequest request)
    {
        IntPtr hwnd = (IntPtr)request.WindowHandle;

        if (hwnd == IntPtr.Zero)
        {
            throw new ArgumentException("No window was given.", nameof(request));
        }

        if (request.Window.Transparent)
        {
            ScreenCaptureResult? transparent = TransparentWindowCapture.Capture(hwnd, request);

            if (transparent != null)
            {
                return transparent;
            }
        }

        PlatformRectangle area = request.Window.ClientAreaOnly ? WindowsWindowService.GetClientBounds(hwnd) : WindowsWindowService.GetBounds(hwnd);

        using (request.Window.HideTaskbar ? TaskbarHider.HideIfIntersecting(area) : null)
        {
            return CaptureArea(area, request);
        }
    }

    public static PixelBuffer Capture(PlatformRectangle area, bool includeCursor, bool hdrToneMapping = false)
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

            if (hdrToneMapping)
            {
                Win32.GdiFlush();

                try
                {
                    HdrScreenCapture.ApplyColorCorrection((byte*)bits, area.Width * 4, area);
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"HDR screenshot color correction failed: {e}");
                }
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

    /// <summary>
    /// The cursor as an image with alpha and its top left corner on the desktop, or null when it is hidden. Drawn over white and
    /// over black so monochrome and inverting cursors come out the way GDI would draw them over a light background.
    /// </summary>
    internal static (PixelBuffer Image, PlatformPoint Position)? CaptureCursorImage()
    {
        Win32.CURSORINFO cursor = new Win32.CURSORINFO { cbSize = sizeof(Win32.CURSORINFO) };

        if (!Win32.GetCursorInfo(&cursor) || cursor.flags != Win32.CURSOR_SHOWING || !Win32.GetIconInfo(cursor.hCursor, out Win32.ICONINFO icon))
        {
            return null;
        }

        try
        {
            const int size = 64;
            PlatformPoint position = new PlatformPoint(cursor.ptScreenPos.X - icon.xHotspot, cursor.ptScreenPos.Y - icon.yHotspot);
            PixelBuffer white = DrawIcon(cursor.hCursor, size, 0xFFFFFFFF);
            PixelBuffer black = DrawIcon(cursor.hCursor, size, 0xFF000000);
            return (TransparentWindowCapture.CombineBackgrounds(white, black), position);
        }
        finally
        {
            if (icon.hbmMask != IntPtr.Zero) Win32.DeleteObject(icon.hbmMask);
            if (icon.hbmColor != IntPtr.Zero) Win32.DeleteObject(icon.hbmColor);
        }
    }

    /// <summary>An icon or cursor drawn at <paramref name="size"/> pixels with its transparency.</summary>
    internal static PixelBuffer RenderIcon(IntPtr icon, int size) =>
        TransparentWindowCapture.CombineBackgrounds(DrawIcon(icon, size, 0xFFFFFFFF, true), DrawIcon(icon, size, 0xFF000000, true));

    private static PixelBuffer DrawIcon(IntPtr icon, int size, uint background, bool fitToSize = false)
    {
        IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
        IntPtr memoryDc = Win32.CreateCompatibleDC(screenDc);
        Win32.BITMAPINFOHEADER header = new Win32.BITMAPINFOHEADER
        {
            biSize = (uint)sizeof(Win32.BITMAPINFOHEADER),
            biWidth = size,
            biHeight = -size,
            biPlanes = 1,
            biBitCount = 32,
            biCompression = Win32.BI_RGB
        };

        IntPtr bitmap = Win32.CreateDIBSection(screenDc, &header, Win32.DIB_RGB_COLORS, out IntPtr bits, IntPtr.Zero, 0);
        IntPtr previous = Win32.SelectObject(memoryDc, bitmap);

        try
        {
            new Span<uint>((void*)bits, size * size).Fill(background);
            // Width and height 0 draw a cursor at its own size; window icons are scaled to fill the buffer.
            int drawSize = fitToSize ? size : 0;
            Win32.DrawIconEx(memoryDc, 0, 0, icon, drawSize, drawSize, 0, IntPtr.Zero, Win32.DI_NORMAL);
            Win32.GdiFlush();
            return PixelBuffer.FromBgra(bits, size, size, size * 4, forceOpaque: true);
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
