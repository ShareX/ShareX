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

using System;
using System.Collections.Generic;

namespace ShareX.Platform;

/// <summary>Win32 indexes the display diagnostics report passes through. They are not calls.</summary>
public static class DisplayMetric
{
    public const int EffectiveDpi = 0;
    public const int AngularDpi = 1;
    public const int RawDpi = 2;

    public const int VirtualScreenX = 76;
    public const int VirtualScreenY = 77;
    public const int VirtualScreenWidth = 78;
    public const int VirtualScreenHeight = 79;
    public const int MonitorCount = 80;
    public const int SameDisplayFormat = 81;
}

public sealed class DisplayMonitorInfo
{
    public IntPtr Handle { get; init; }
    public string DeviceName { get; init; } = string.Empty;
    public PlatformRectangle Bounds { get; init; }
    public PlatformRectangle WorkingArea { get; init; }
    public bool Primary { get; init; }
}

public sealed class DisplayModeInfo
{
    public int Width { get; init; }
    public int Height { get; init; }
    public PlatformPoint Position { get; init; }
    public int BitsPerPixel { get; init; }
    public int Frequency { get; init; }

    /// <summary>DMDO_DEFAULT, DMDO_90, DMDO_180 or DMDO_270 as 0 to 3.</summary>
    public int Orientation { get; init; }
}

public sealed class DisplayDeviceInfo
{
    public string DeviceName { get; init; } = string.Empty;
    public string DeviceString { get; init; } = string.Empty;
    public string DeviceKey { get; init; } = string.Empty;
    public string DeviceId { get; init; } = string.Empty;
}

/// <summary>One active display path: the monitor behind a GDI source, its connection and its colour state.</summary>
public sealed class DisplayTargetInfo
{
    /// <summary>GDI device name of the source, such as \\.\DISPLAY1.</summary>
    public string SourceDeviceName { get; set; } = string.Empty;
    public string FriendlyName { get; set; } = string.Empty;
    public string OutputTechnology { get; set; } = string.Empty;

    /// <summary>Clockwise rotation in degrees.</summary>
    public int Rotation { get; set; }
    public double RefreshRate { get; set; }
    public bool AdvancedColorSupported { get; set; }
    public bool AdvancedColorEnabled { get; set; }
    public int BitsPerColorChannel { get; set; }
    public uint SdrWhiteLevelNits { get; set; }
}

/// <summary>Windows NT current-version values used in the display diagnostics report. Null fields were missing.</summary>
public readonly record struct WindowsProductInfo(
    string ProductName,
    string Build,
    string? UpdateBuildRevision,
    string? DisplayVersion,
    string? ReleaseId);

/// <summary>
/// Native display facts for the diagnostics report. Unsupported desktops answer <see cref="IsSupported"/> false and the
/// report is built from the toolkit's screen list instead.
/// </summary>
public interface IDisplayDiagnosticsService
{
    bool IsSupported => false;

    IReadOnlyList<DisplayMonitorInfo> EnumerateMonitors() => [];

    PlatformPoint? GetDpiForMonitor(IntPtr monitor, int dpiType) => null;

    /// <summary>The scale Windows display settings shows for the monitor, such as 150.</summary>
    int? GetScaleFactorForMonitor(IntPtr monitor) => null;

    DisplayModeInfo? GetCurrentDisplayMode(string deviceName) => null;

    (DisplayDeviceInfo? Adapter, DisplayDeviceInfo? Monitor) GetDisplayDevices(string deviceName) => (null, null);

    IReadOnlyList<DisplayDeviceInfo> GetAttachedAdapters() => [];

    string GetProcessDpiAwareness() => "Unknown";

    uint? GetDpiForSystem() => null;

    uint? GetDpiForWindow(IntPtr window) => null;

    PlatformPoint? GetCursorPosition() => null;

    PlatformPoint? GetPhysicalCursorPosition() => null;

    IntPtr MonitorFromPoint(PlatformPoint point) => IntPtr.Zero;

    PlatformRectangle? GetWindowBounds(IntPtr window) => null;

    /// <summary>The main taskbar: its edge (0 left, 1 top, 2 right, 3 bottom), bounds and auto-hide state.</summary>
    (int Edge, PlatformRectangle Bounds, bool AutoHide)? GetTaskbar() => null;

    IReadOnlyList<PlatformRectangle> GetSecondaryTaskbars() => [];

    int GetSystemMetric(int index) => 0;

    IReadOnlyList<DisplayTargetInfo> GetDisplayTargets() => [];

    WindowsProductInfo? ReadWindowsProduct() => null;

    string? ReadCompatibilityLayers(bool machineWide) => null;

    string ReadDriverVersion(string deviceKey) => string.Empty;
}

/// <summary>Answers that this system has no native display report.</summary>
public sealed class DisplayDiagnosticsUnsupported : IDisplayDiagnosticsService
{
    public static readonly DisplayDiagnosticsUnsupported Instance = new();

    private DisplayDiagnosticsUnsupported()
    {
    }
}
