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

#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShareX;

/// <summary>Win32 queries used only by display diagnostics. Every call tolerates missing entry points on older Windows.</summary>
internal static class DisplayDiagnosticsNative
{
    internal const int MDT_EFFECTIVE_DPI = 0;
    internal const int MDT_ANGULAR_DPI = 1;
    internal const int MDT_RAW_DPI = 2;

    internal const int SM_XVIRTUALSCREEN = 76;
    internal const int SM_YVIRTUALSCREEN = 77;
    internal const int SM_CXVIRTUALSCREEN = 78;
    internal const int SM_CYVIRTUALSCREEN = 79;
    internal const int SM_CMONITORS = 80;
    internal const int SM_SAMEDISPLAYFORMAT = 81;

    private const int ENUM_CURRENT_SETTINGS = -1;
    private const uint MONITORINFOF_PRIMARY = 1;
    private const uint MONITOR_DEFAULTTONULL = 0;
    private const uint ABM_GETSTATE = 4;
    private const uint ABM_GETTASKBARPOS = 5;
    private const int ABS_AUTOHIDE = 1;
    private const int DISPLAY_DEVICE_ATTACHED_TO_DESKTOP = 0x1;

    internal sealed class NativeMonitor
    {
        public IntPtr Handle { get; init; }
        public string DeviceName { get; init; } = string.Empty;
        public Rectangle Bounds { get; init; }
        public Rectangle WorkingArea { get; init; }
        public bool Primary { get; init; }
    }

    internal sealed class DisplayMode
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public Point Position { get; init; }
        public int BitsPerPixel { get; init; }
        public int Frequency { get; init; }
        /// <summary>DMDO_DEFAULT, DMDO_90, DMDO_180 or DMDO_270 as 0 to 3.</summary>
        public int Orientation { get; init; }
    }

    internal sealed class DisplayDevice
    {
        public string DeviceName { get; init; } = string.Empty;
        public string DeviceString { get; init; } = string.Empty;
        public string DeviceKey { get; init; } = string.Empty;
        public string DeviceId { get; init; } = string.Empty;
    }

    internal static List<NativeMonitor> EnumerateMonitors()
    {
        List<NativeMonitor> monitors = new();

        bool Callback(IntPtr monitor, IntPtr hdc, ref NativeRect rect, IntPtr data)
        {
            MonitorInfoEx info = new() { Size = Marshal.SizeOf<MonitorInfoEx>(), DeviceName = string.Empty };
            if (GetMonitorInfo(monitor, ref info))
            {
                monitors.Add(new NativeMonitor
                {
                    Handle = monitor,
                    DeviceName = info.DeviceName,
                    Bounds = info.Monitor.ToRectangle(),
                    WorkingArea = info.WorkArea.ToRectangle(),
                    Primary = (info.Flags & MONITORINFOF_PRIMARY) != 0
                });
            }

            return true;
        }

        MonitorEnumProc callback = Callback;
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero);
        GC.KeepAlive(callback);
        return monitors;
    }

    internal static Point? GetDpiForMonitor(IntPtr monitor, int dpiType)
    {
        try
        {
            return GetDpiForMonitorNative(monitor, dpiType, out uint x, out uint y) == 0 ? new Point((int)x, (int)y) : null;
        }
        catch (Exception e) when (e is EntryPointNotFoundException or DllNotFoundException)
        {
            return null;
        }
    }

    /// <summary>The scale Windows display settings shows for the monitor, such as 150.</summary>
    internal static int? GetScaleFactorForMonitor(IntPtr monitor)
    {
        try
        {
            return GetScaleFactorForMonitorNative(monitor, out int scale) == 0 ? scale : null;
        }
        catch (Exception e) when (e is EntryPointNotFoundException or DllNotFoundException)
        {
            return null;
        }
    }

    internal static DisplayMode? GetCurrentDisplayMode(string deviceName)
    {
        DevMode mode = new() { Size = (ushort)Marshal.SizeOf<DevMode>(), DeviceName = string.Empty, FormName = string.Empty };
        if (!EnumDisplaySettingsEx(deviceName, ENUM_CURRENT_SETTINGS, ref mode, 0))
        {
            return null;
        }

        return new DisplayMode
        {
            Width = (int)mode.PelsWidth,
            Height = (int)mode.PelsHeight,
            Position = new Point(mode.PositionX, mode.PositionY),
            BitsPerPixel = (int)mode.BitsPerPel,
            Frequency = (int)mode.DisplayFrequency,
            Orientation = (int)mode.DisplayOrientation
        };
    }

    /// <summary>The graphics adapter driving a GDI source such as \\.\DISPLAY1, and the monitor attached to it.</summary>
    internal static (DisplayDevice? Adapter, DisplayDevice? Monitor) GetDisplayDevices(string deviceName)
    {
        DisplayDevice? adapter = null;

        for (uint i = 0; ; i++)
        {
            DisplayDeviceNative device = CreateDisplayDevice();
            if (!EnumDisplayDevices(null, i, ref device, 0))
            {
                break;
            }

            if (string.Equals(device.DeviceName, deviceName, StringComparison.OrdinalIgnoreCase))
            {
                adapter = ToDisplayDevice(device);
                break;
            }
        }

        DisplayDevice? monitor = null;
        DisplayDeviceNative monitorDevice = CreateDisplayDevice();
        if (EnumDisplayDevices(deviceName, 0, ref monitorDevice, 0))
        {
            monitor = ToDisplayDevice(monitorDevice);
        }

        return (adapter, monitor);
    }

    /// <summary>Every adapter attached to the desktop, once each, by name.</summary>
    internal static List<DisplayDevice> GetAttachedAdapters()
    {
        List<DisplayDevice> adapters = new();

        for (uint i = 0; ; i++)
        {
            DisplayDeviceNative device = CreateDisplayDevice();
            if (!EnumDisplayDevices(null, i, ref device, 0))
            {
                break;
            }

            if ((device.StateFlags & DISPLAY_DEVICE_ATTACHED_TO_DESKTOP) != 0)
            {
                adapters.Add(ToDisplayDevice(device));
            }
        }

        return adapters;
    }

    internal static string GetProcessDpiAwareness()
    {
        try
        {
            IntPtr context = GetThreadDpiAwarenessContext();
            string name;

            if (AreDpiAwarenessContextsEqual(context, new IntPtr(-4)))
            {
                name = "Per-monitor v2";
            }
            else if (AreDpiAwarenessContextsEqual(context, new IntPtr(-5)))
            {
                name = "Unaware (GDI scaled)";
            }
            else
            {
                name = GetAwarenessFromDpiAwarenessContext(context) switch
                {
                    0 => "Unaware",
                    1 => "System aware",
                    2 => "Per-monitor v1",
                    _ => "Unknown"
                };
            }

            if (GetProcessDpiAwarenessNative(IntPtr.Zero, out int processAwareness) == 0)
            {
                string process = processAwareness switch
                {
                    0 => "unaware",
                    1 => "system aware",
                    2 => "per-monitor aware",
                    _ => processAwareness.ToString()
                };
                name += $" (process: {process})";
            }

            return name;
        }
        catch (Exception e) when (e is EntryPointNotFoundException or DllNotFoundException)
        {
            return "Unknown (API not available)";
        }
    }

    internal static uint? GetDpiForSystem()
    {
        try
        {
            return GetDpiForSystemNative();
        }
        catch (EntryPointNotFoundException)
        {
            return null;
        }
    }

    internal static uint? GetDpiForWindow(IntPtr window)
    {
        try
        {
            uint dpi = GetDpiForWindowNative(window);
            return dpi > 0 ? dpi : null;
        }
        catch (EntryPointNotFoundException)
        {
            return null;
        }
    }

    internal static Point? GetCursorPosition()
    {
        return GetCursorPos(out NativePoint point) ? new Point(point.X, point.Y) : null;
    }

    internal static Point? GetPhysicalCursorPosition()
    {
        try
        {
            return GetPhysicalCursorPos(out NativePoint point) ? new Point(point.X, point.Y) : null;
        }
        catch (EntryPointNotFoundException)
        {
            return null;
        }
    }

    internal static IntPtr MonitorFromPoint(Point point) =>
        MonitorFromPointNative(new NativePoint { X = point.X, Y = point.Y }, MONITOR_DEFAULTTONULL);

    internal static Rectangle? GetWindowBounds(IntPtr window) =>
        GetWindowRect(window, out NativeRect rect) ? rect.ToRectangle() : null;

    /// <summary>The main taskbar: its edge (0 left, 1 top, 2 right, 3 bottom), bounds and auto-hide state.</summary>
    internal static (int Edge, Rectangle Bounds, bool AutoHide)? GetTaskbar()
    {
        AppBarData data = new() { Size = Marshal.SizeOf<AppBarData>() };
        if (SHAppBarMessage(ABM_GETTASKBARPOS, ref data) == IntPtr.Zero)
        {
            return null;
        }

        AppBarData stateData = new() { Size = Marshal.SizeOf<AppBarData>() };
        bool autoHide = ((int)SHAppBarMessage(ABM_GETSTATE, ref stateData) & ABS_AUTOHIDE) != 0;
        return (data.Edge, data.Rect.ToRectangle(), autoHide);
    }

    /// <summary>Bounds of the taskbars Windows shows on secondary monitors.</summary>
    internal static List<Rectangle> GetSecondaryTaskbars()
    {
        List<Rectangle> taskbars = new();
        IntPtr window = IntPtr.Zero;

        while ((window = FindWindowEx(IntPtr.Zero, window, "Shell_SecondaryTrayWnd", null)) != IntPtr.Zero)
        {
            if (IsWindowVisible(window) && GetWindowRect(window, out NativeRect rect))
            {
                taskbars.Add(rect.ToRectangle());
            }
        }

        return taskbars;
    }

    internal static int GetSystemMetric(int index) => GetSystemMetrics(index);

    private static DisplayDeviceNative CreateDisplayDevice() => new()
    {
        Size = Marshal.SizeOf<DisplayDeviceNative>(),
        DeviceName = string.Empty,
        DeviceString = string.Empty,
        DeviceId = string.Empty,
        DeviceKey = string.Empty
    };

    private static DisplayDevice ToDisplayDevice(DisplayDeviceNative device) => new()
    {
        DeviceName = device.DeviceName,
        DeviceString = device.DeviceString,
        DeviceId = device.DeviceId,
        DeviceKey = device.DeviceKey
    };

    private delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, ref NativeRect rect, IntPtr data);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr clip, MonitorEnumProc callback, IntPtr data);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfoEx info);

    [DllImport("shcore.dll", EntryPoint = "GetDpiForMonitor")]
    private static extern int GetDpiForMonitorNative(IntPtr monitor, int dpiType, out uint dpiX, out uint dpiY);

    [DllImport("shcore.dll", EntryPoint = "GetScaleFactorForMonitor")]
    private static extern int GetScaleFactorForMonitorNative(IntPtr monitor, out int scale);

    [DllImport("shcore.dll", EntryPoint = "GetProcessDpiAwareness")]
    private static extern int GetProcessDpiAwarenessNative(IntPtr process, out int awareness);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumDisplaySettingsEx(string deviceName, int modeNumber, ref DevMode mode, uint flags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumDisplayDevices(string? device, uint deviceIndex, ref DisplayDeviceNative displayDevice, uint flags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetThreadDpiAwarenessContext();

    [DllImport("user32.dll")]
    private static extern int GetAwarenessFromDpiAwarenessContext(IntPtr context);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AreDpiAwarenessContextsEqual(IntPtr contextA, IntPtr contextB);

    [DllImport("user32.dll", EntryPoint = "GetDpiForSystem")]
    private static extern uint GetDpiForSystemNative();

    [DllImport("user32.dll", EntryPoint = "GetDpiForWindow")]
    private static extern uint GetDpiForWindowNative(IntPtr window);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int index);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out NativePoint point);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetPhysicalCursorPos(out NativePoint point);

    [DllImport("user32.dll", EntryPoint = "MonitorFromPoint")]
    private static extern IntPtr MonitorFromPointNative(NativePoint point, uint flags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr window, out NativeRect rect);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string className, string? windowName);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr window);

    [DllImport("shell32.dll")]
    private static extern IntPtr SHAppBarMessage(uint message, ref AppBarData data);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public readonly Rectangle ToRectangle() => Rectangle.FromLTRB(Left, Top, Right, Bottom);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MonitorInfoEx
    {
        public int Size;
        public NativeRect Monitor;
        public NativeRect WorkArea;
        public uint Flags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DisplayDeviceNative
    {
        public int Size;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;

        public int StateFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DevMode
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        public ushort SpecVersion;
        public ushort DriverVersion;
        public ushort Size;
        public ushort DriverExtra;
        public uint Fields;
        public int PositionX;
        public int PositionY;
        public uint DisplayOrientation;
        public uint DisplayFixedOutput;
        public short Color;
        public short Duplex;
        public short YResolution;
        public short TTOption;
        public short Collate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FormName;
        public ushort LogPixels;
        public uint BitsPerPel;
        public uint PelsWidth;
        public uint PelsHeight;
        public uint DisplayFlags;
        public uint DisplayFrequency;
        public uint ICMMethod;
        public uint ICMIntent;
        public uint MediaType;
        public uint DitherType;
        public uint Reserved1;
        public uint Reserved2;
        public uint PanningWidth;
        public uint PanningHeight;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AppBarData
    {
        public int Size;
        public IntPtr Window;
        public uint CallbackMessage;
        public int Edge;
        public NativeRect Rect;
        public IntPtr LParam;
    }
}
