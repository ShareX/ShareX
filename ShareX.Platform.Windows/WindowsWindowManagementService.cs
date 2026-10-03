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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShareX.Platform.Windows;

/// <summary>Window inspection, top most, opacity and borderless mode through Win32, as ShareX's WindowInfo and borderless tool did.</summary>
public sealed class WindowsWindowManagementService : IWindowManagementService
{
    private const uint WM_GETICON = 0x007F;
    private const uint WM_QUERYDRAGICON = 0x0037;
    private const uint SMTO_ABORTIFHUNG = 0x0002;
    private const nuint ICON_SMALL = 0;
    private const nuint ICON_BIG = 1;
    private const nuint ICON_SMALL2 = 2;
    private const int GCL_HICON = -14;
    private const int GCL_HICONSM = -34;

    private readonly ConcurrentDictionary<long, BorderlessState> borderlessWindows = new ConcurrentDictionary<long, BorderlessState>();
    private readonly WindowsScreenCaptureService screens;

    public WindowsWindowManagementService(WindowsScreenCaptureService screens)
    {
        this.screens = screens;
    }

    public FeatureSupport Support => FeatureSupport.Supported;

    public FeatureSupport BorderlessSupport => FeatureSupport.Supported;

    public long GetWindowAt(PlatformPoint point, bool topLevel)
    {
        IntPtr hwnd = Win32.WindowFromPoint(new Win32.POINT { X = point.X, Y = point.Y });
        return (topLevel && hwnd != IntPtr.Zero ? Win32.GetAncestor(hwnd, Win32.GA_ROOT) : hwnd).ToInt64();
    }

    public WindowDetails? GetDetails(long windowHandle)
    {
        IntPtr hwnd = (IntPtr)windowHandle;

        if (!Win32.GetWindowRect(hwnd, out Win32.RECT rect))
        {
            return null;
        }

        Win32.GetWindowThreadProcessId(hwnd, out uint processId);
        (string? name, string? path) = GetProcess(processId);
        WindowStyles style = (WindowStyles)Win32.GetWindowLongPtr(hwnd, Win32.GWL_STYLE);
        WindowStyles exStyle = (WindowStyles)Win32.GetWindowLongPtr(hwnd, Win32.GWL_EXSTYLE);

        return new WindowDetails(
            windowHandle,
            WindowsWindowService.GetTitle(hwnd),
            WindowsWindowService.GetClassName(hwnd),
            name,
            path,
            processId != 0 ? (int)processId : null,
            WindowsWindowService.GetBounds(hwnd),
            // Inspect Window has always shown GetClientRect coordinates relative to the client area.
            Win32.GetClientRect(hwnd, out Win32.RECT clientRect) ? clientRect.ToRectangle() : null,
            SplitFlags(style),
            SplitFlags(exStyle),
            (exStyle & WindowStyles.WS_EX_TOPMOST) != 0,
            GetOpacity(hwnd, exStyle));
    }

    public byte[]? GetIcon(long windowHandle)
    {
        IntPtr icon = FindIcon((IntPtr)windowHandle);

        if (icon == IntPtr.Zero)
        {
            return null;
        }

        PixelBuffer pixels = WindowsScreenCaptureService.RenderIcon(icon, 32);
        return PngCodec.Encode(pixels);
    }

    public bool SetTopMost(long windowHandle, bool topMost) =>
        Win32.SetWindowPos((IntPtr)windowHandle, topMost ? Win32.HWND_TOPMOST : Win32.HWND_NOTOPMOST, 0, 0, 0, 0,
            Win32.SWP_NOMOVE | Win32.SWP_NOSIZE | Win32.SWP_NOACTIVATE);

    public bool SetOpacity(long windowHandle, byte opacity)
    {
        IntPtr hwnd = (IntPtr)windowHandle;
        nint exStyle = Win32.GetWindowLongPtr(hwnd, Win32.GWL_EXSTYLE);

        if ((exStyle & (nint)Win32.WS_EX_LAYERED) == 0)
        {
            Win32.SetWindowLongPtr(hwnd, Win32.GWL_EXSTYLE, exStyle | (nint)Win32.WS_EX_LAYERED);
        }

        return Win32.SetLayeredWindowAttributes(hwnd, 0, opacity, Win32.LWA_ALPHA);
    }

    public bool ToggleBorderless(long windowHandle, bool useWorkingArea)
    {
        IntPtr hwnd = (IntPtr)windowHandle;

        if (Win32.IsIconic(hwnd))
        {
            Win32.ShowWindow(hwnd, Win32.SW_RESTORE);
        }

        const uint flags = Win32.SWP_FRAMECHANGED | Win32.SWP_NOOWNERZORDER | Win32.SWP_NOZORDER;

        if (borderlessWindows.TryRemove(windowHandle, out BorderlessState? state))
        {
            Win32.SetWindowLongPtr(hwnd, Win32.GWL_STYLE, state.Style);
            Win32.SetWindowLongPtr(hwnd, Win32.GWL_EXSTYLE, state.ExStyle);
            PlatformRectangle r = state.Bounds;
            return Win32.SetWindowPos(hwnd, IntPtr.Zero, r.X, r.Y, r.Width, r.Height, flags);
        }

        if (!Win32.GetWindowRect(hwnd, out Win32.RECT rect))
        {
            return false;
        }

        nint style = Win32.GetWindowLongPtr(hwnd, Win32.GWL_STYLE);
        nint exStyle = Win32.GetWindowLongPtr(hwnd, Win32.GWL_EXSTYLE);
        borderlessWindows[windowHandle] = new BorderlessState(rect.ToRectangle(), style, exStyle);

        Win32.SetWindowLongPtr(hwnd, Win32.GWL_STYLE, style & ~(nint)(WindowStyles.WS_CAPTION | WindowStyles.WS_MAXIMIZEBOX |
            WindowStyles.WS_SYSMENU | WindowStyles.WS_THICKFRAME));
        Win32.SetWindowLongPtr(hwnd, Win32.GWL_EXSTYLE, exStyle & ~(nint)(WindowStyles.WS_EX_CLIENTEDGE | WindowStyles.WS_EX_DLGMODALFRAME |
            WindowStyles.WS_EX_STATICEDGE));

        // v22's DesktopScreen.FromHandle used the entire window rectangle, then the primary screen.
        ScreenInfo? screen = SelectBorderlessScreen(rect.ToRectangle(), screens.GetScreens());

        if (screen == null)
        {
            return Win32.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, flags | Win32.SWP_NOMOVE | Win32.SWP_NOSIZE);
        }

        PlatformRectangle target = useWorkingArea ? screen.WorkingArea : screen.Bounds;
        return Win32.SetWindowPos(hwnd, IntPtr.Zero, target.X, target.Y, target.Width, target.Height, flags);
    }

    internal static ScreenInfo? SelectBorderlessScreen(PlatformRectangle windowBounds, IReadOnlyList<ScreenInfo> screens)
    {
        ScreenInfo? best = null;
        long bestArea = 0;

        foreach (ScreenInfo screen in screens)
        {
            PlatformRectangle overlap = screen.Bounds.Intersect(windowBounds);
            long area = (long)overlap.Width * overlap.Height;

            if (area > bestArea)
            {
                best = screen;
                bestArea = area;
            }
        }

        return best ?? screens.FirstOrDefault(screen => screen.IsPrimary) ?? screens.FirstOrDefault();
    }

    /// <summary>The flag names, one per set bit group, as WindowStyles.ToString() printed them in ShareX.</summary>
    private static IReadOnlyList<string> SplitFlags(WindowStyles styles) =>
        styles.ToString().Split(", ", StringSplitOptions.RemoveEmptyEntries);

    private static byte? GetOpacity(IntPtr hwnd, WindowStyles exStyle)
    {
        if ((exStyle & WindowStyles.WS_EX_LAYERED) == 0)
        {
            return 255;
        }

        return Win32.GetLayeredWindowAttributes(hwnd, out _, out byte alpha, out uint flags) && (flags & Win32.LWA_ALPHA) != 0 ? alpha : (byte)255;
    }

    private static (string? Name, string? Path) GetProcess(uint processId)
    {
        try
        {
            using Process process = Process.GetProcessById((int)processId);
            string? path = null;

            try
            {
                path = process.MainModule?.FileName;
            }
            catch (Exception e) when (e is System.ComponentModel.Win32Exception or InvalidOperationException)
            {
                // Elevated and protected processes hide their modules.
            }

            return (process.ProcessName, path);
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            return (null, null);
        }
    }

    /// <summary>The small icon first, as ShareX's window lists always showed, then the large one.</summary>
    private static IntPtr FindIcon(IntPtr hwnd)
    {
        IntPtr icon = SendIconMessage(hwnd, WM_GETICON, ICON_SMALL2);
        if (icon == IntPtr.Zero) icon = SendIconMessage(hwnd, WM_GETICON, ICON_SMALL);
        if (icon == IntPtr.Zero) icon = Win32.GetClassLongPtr(hwnd, GCL_HICONSM);
        if (icon == IntPtr.Zero) icon = SendIconMessage(hwnd, WM_QUERYDRAGICON, 0);
        if (icon == IntPtr.Zero) icon = SendIconMessage(hwnd, WM_GETICON, ICON_BIG);
        if (icon == IntPtr.Zero) icon = Win32.GetClassLongPtr(hwnd, GCL_HICON);
        return icon;
    }

    private static IntPtr SendIconMessage(IntPtr hwnd, uint message, nuint wParam)
    {
        Win32.SendMessageTimeout(hwnd, message, wParam, 0, SMTO_ABORTIFHUNG, 1000, out nint result);
        return result;
    }

    private sealed record BorderlessState(PlatformRectangle Bounds, nint Style, nint ExStyle);
}
