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

using Avalonia.Controls;
using Avalonia.Platform;
using ShareX.HelpersLib;
using ShareX.Platform;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ShareX;

internal sealed class DisplayDiagnosticsMonitor
{
    public int Number { get; init; }
    public string DeviceName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool Primary { get; init; }
    public Rectangle Bounds { get; init; }
    public Rectangle WorkingArea { get; init; }
    public int ScalePercent { get; init; }
}

internal sealed class DisplayDiagnosticsResult
{
    public IReadOnlyList<DisplayDiagnosticsMonitor> Monitors { get; init; } = [];
    public IReadOnlyList<string> Findings { get; init; } = [];
    public Point? CursorPosition { get; init; }
    public string Text { get; init; } = string.Empty;
}

/// <summary>
/// Collects everything needed to diagnose multi-monitor and scaling problems into one plain text report. The report stays in
/// English on purpose: it is written for the people who read bug reports, whatever language ShareX is shown in.
/// </summary>
internal static class DisplayDiagnosticsCollector
{
    private const int LabelWidth = 30;

    private static IDisplayDiagnosticsService Native =>
        PlatformServices.IsInitialized ? PlatformServices.Current.DisplayDiagnostics : DisplayDiagnosticsUnsupported.Instance;

    private static Rectangle Drawing(PlatformRectangle rectangle) =>
        Rectangle.FromLTRB(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);

    private static Point Drawing(PlatformPoint point) => new(point.X, point.Y);

    private static Point? Drawing(PlatformPoint? point) => point is PlatformPoint value ? Drawing(value) : null;

    private sealed class MonitorData
    {
        public int Number;
        public DisplayMonitorInfo Native = null!;
        public Screen? AvaloniaScreen;
        public Point? EffectiveDpi;
        public Point? AngularDpi;
        public Point? RawDpi;
        public int? WindowsScale;
        public DisplayModeInfo? Mode;
        public DisplayDeviceInfo? Adapter;
        public DisplayDeviceInfo? MonitorDevice;
        public DisplayTargetInfo? Target;

        public int ScalePercent => EffectiveDpi is Point dpi ? (int)Math.Round(dpi.X * 100.0 / 96) :
            AvaloniaScreen != null ? (int)Math.Round(AvaloniaScreen.Scaling * 100) : 100;

        public string Name
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Target?.FriendlyName)) return Target!.FriendlyName;
                if (!string.IsNullOrWhiteSpace(AvaloniaScreen?.DisplayName)) return AvaloniaScreen!.DisplayName!;
                if (!string.IsNullOrWhiteSpace(MonitorDevice?.DeviceString)) return MonitorDevice!.DeviceString;
                return "Unknown monitor";
            }
        }
    }

    public static DisplayDiagnosticsResult Collect(Window owner)
    {
        StringBuilder report = new();
        List<string> findings = [];

        if (!Native.IsSupported)
        {
            return CollectAvaloniaOnly(owner);
        }

        List<MonitorData> monitors = CollectMonitors(owner);
        Point? cursor = Drawing(Native.GetCursorPosition());

        AppendHeader(report);
        int findingsInsertIndex = report.Length;

        AppendApplication(report);
        AppendWindows(report);
        AppendDpi(report, findings);
        AppendVirtualScreen(report, findings, owner, monitors);
        AppendMonitors(report, findings, monitors);
        AppendArrangement(report, findings, monitors);
        AppendTaskbars(report, findings, monitors);
        AppendGraphics(report);
        AppendCursor(report, findings, monitors, cursor);
        AppendThisWindow(report, owner, monitors);
        AppendOverlayWindows(report, findings, monitors);
        AppendCaptureSettings(report);

        StringBuilder findingsText = new();
        findingsText.AppendLine("Findings");
        if (findings.Count == 0)
        {
            findingsText.AppendLine("  - Nothing unusual detected.");
        }
        else
        {
            foreach (string finding in findings)
            {
                findingsText.Append("  - ").AppendLine(finding);
            }
        }
        findingsText.AppendLine();
        report.Insert(findingsInsertIndex, findingsText.ToString());

        return new DisplayDiagnosticsResult
        {
            Monitors = monitors.Select(x => new DisplayDiagnosticsMonitor
            {
                Number = x.Number,
                DeviceName = x.Native.DeviceName,
                Name = x.Name,
                Primary = x.Native.Primary,
                Bounds = Drawing(x.Native.Bounds),
                WorkingArea = Drawing(x.Native.WorkingArea),
                ScalePercent = x.ScalePercent
            }).ToArray(),
            Findings = findings,
            CursorPosition = cursor,
            Text = report.ToString().TrimEnd() + Environment.NewLine
        };
    }

    private static List<MonitorData> CollectMonitors(Window owner)
    {
        IReadOnlyList<DisplayTargetInfo> targets = Native.GetDisplayTargets();
        IReadOnlyList<Screen> screens = owner.Screens.All;
        List<MonitorData> monitors = [];
        int fallbackNumber = 100;

        foreach (DisplayMonitorInfo native in Native.EnumerateMonitors())
        {
            (DisplayDeviceInfo? adapter, DisplayDeviceInfo? monitorDevice) =
                Native.GetDisplayDevices(native.DeviceName);

            monitors.Add(new MonitorData
            {
                Number = GetDisplayNumber(native.DeviceName) ?? fallbackNumber++,
                Native = native,
                AvaloniaScreen = screens.FirstOrDefault(x =>
                    string.Equals(DesktopScreen.GetDeviceName(x), native.DeviceName, StringComparison.OrdinalIgnoreCase)),
                EffectiveDpi = Drawing(Native.GetDpiForMonitor(native.Handle, DisplayMetric.EffectiveDpi)),
                AngularDpi = Drawing(Native.GetDpiForMonitor(native.Handle, DisplayMetric.AngularDpi)),
                RawDpi = Drawing(Native.GetDpiForMonitor(native.Handle, DisplayMetric.RawDpi)),
                WindowsScale = Native.GetScaleFactorForMonitor(native.Handle),
                Mode = Native.GetCurrentDisplayMode(native.DeviceName),
                Adapter = adapter,
                MonitorDevice = monitorDevice,
                Target = targets.FirstOrDefault(x =>
                    string.Equals(x.SourceDeviceName, native.DeviceName, StringComparison.OrdinalIgnoreCase))
            });
        }

        return monitors.OrderBy(x => x.Number).ToList();
    }

    private static int? GetDisplayNumber(string deviceName)
    {
        const string prefix = @"\\.\DISPLAY";
        return deviceName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(deviceName.AsSpan(prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, out int number)
            ? number
            : null;
    }

    private static void AppendHeader(StringBuilder report)
    {
        report.AppendLine("ShareX display diagnostics");
        report.AppendLine(new string('=', 26));
        Line(report, "Generated", DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture));
        report.AppendLine();
    }

    private static void AppendApplication(StringBuilder report)
    {
        Section(report, "ShareX");
        Line(report, "Version", ApplicationInfo.VersionText);
        Line(report, "Build", ApplicationInfo.Build.ToString());
        Line(report, "Elevated", YesNo(StartupOptions.IsAdmin));
        Line(report, ".NET", RuntimeInformation.FrameworkDescription);
        Line(report, "Avalonia", typeof(Window).Assembly.GetName().Version?.ToString() ?? "Unknown");
        Line(report, "Process architecture", RuntimeInformation.ProcessArchitecture.ToString());
        Line(report, "Language", CultureInfo.CurrentUICulture.Name);
        report.AppendLine();
    }

    private static void AppendWindows(StringBuilder report)
    {
        Section(report, "Windows");

        WindowsProductInfo? product = Native.ReadWindowsProduct();
        string productName = Environment.OSVersion.VersionString;
        string build = Environment.OSVersion.Version.Build.ToString(CultureInfo.InvariantCulture);
        string? version = null;
        string? ubr = null;
        if (product is WindowsProductInfo info)
        {
            if (!string.IsNullOrEmpty(info.ProductName))
            {
                productName = info.ProductName;
            }

            if (!string.IsNullOrEmpty(info.Build))
            {
                build = info.Build;
            }

            version = info.DisplayVersion ?? info.ReleaseId;
            ubr = string.IsNullOrEmpty(info.UpdateBuildRevision) ? null : info.UpdateBuildRevision;
        }

        // Windows 11 still reports "Windows 10" as its product name.
        if (int.TryParse(build, NumberStyles.None, CultureInfo.InvariantCulture, out int buildNumber) && buildNumber >= 22000)
        {
            productName = productName.Replace("Windows 10", "Windows 11", StringComparison.Ordinal);
        }

        Line(report, "Edition", productName);
        Line(report, "Version", version ?? "Unknown");
        Line(report, "Build", ubr != null ? $"{build}.{ubr}" : build);
        Line(report, "OS architecture", RuntimeInformation.OSArchitecture.ToString());
        report.AppendLine();
    }

    private static void AppendDpi(StringBuilder report, List<string> findings)
    {
        Section(report, "DPI awareness");

        string awareness = Native.GetProcessDpiAwareness();
        Line(report, "Thread DPI awareness", awareness);
        if (!awareness.StartsWith("Per-monitor v2", StringComparison.Ordinal))
        {
            findings.Add($"ShareX is not running per-monitor v2 DPI aware ({awareness}). Capture coordinates can be scaled by Windows.");
        }

        uint? systemDpi = Native.GetDpiForSystem();
        Line(report, "System DPI", systemDpi is uint dpi ? $"{dpi} ({Percent(dpi)})" : "Unknown");

        string? userLayers = Native.ReadCompatibilityLayers(machineWide: false);
        string? machineLayers = Native.ReadCompatibilityLayers(machineWide: true);
        Line(report, "Compatibility (user)", string.IsNullOrWhiteSpace(userLayers) ? "None" : userLayers);
        Line(report, "Compatibility (all users)", string.IsNullOrWhiteSpace(machineLayers) ? "None" : machineLayers);

        string layers = $"{userLayers} {machineLayers}";
        if (layers.Contains("DPI", StringComparison.OrdinalIgnoreCase))
        {
            findings.Add("A Windows compatibility setting overrides high DPI scaling for ShareX.exe (Properties > Compatibility > " +
                "Change high DPI settings). Turn it off and test again.");
        }

        report.AppendLine();
    }

    private static void AppendVirtualScreen(StringBuilder report, List<string> findings, Window owner, List<MonitorData> monitors)
    {
        Section(report, "Virtual screen");

        Rectangle gdi = new(
            Native.GetSystemMetric(DisplayMetric.VirtualScreenX),
            Native.GetSystemMetric(DisplayMetric.VirtualScreenY),
            Native.GetSystemMetric(DisplayMetric.VirtualScreenWidth),
            Native.GetSystemMetric(DisplayMetric.VirtualScreenHeight));
        Rectangle nativeUnion = Combine(monitors.Select(x => Drawing(x.Native.Bounds)));
        IReadOnlyList<Screen> screens = owner.Screens.All;
        Rectangle avaloniaUnion = Combine(screens.Select(x => new Rectangle(x.Bounds.X, x.Bounds.Y, x.Bounds.Width, x.Bounds.Height)));
        Rectangle shareXBounds = CaptureHelpers.GetScreenBounds();

        Line(report, "GDI virtual screen", Format(gdi));
        Line(report, "Monitor bounds combined", Format(nativeUnion));
        Line(report, "Avalonia screens combined", Format(avaloniaUnion));
        Line(report, "ShareX capture bounds", Format(shareXBounds));
        Line(report, "Monitor count", $"GDI {Native.GetSystemMetric(DisplayMetric.MonitorCount)}, " +
            $"enumerated {monitors.Count}, Avalonia {screens.Count}");
        Line(report, "Same display format", YesNo(Native.GetSystemMetric(DisplayMetric.SameDisplayFormat) != 0));

        if (gdi != nativeUnion || avaloniaUnion != nativeUnion || shareXBounds != nativeUnion)
        {
            findings.Add($"The desktop bounds disagree: GDI {Format(gdi)}, monitors {Format(nativeUnion)}, " +
                $"Avalonia {Format(avaloniaUnion)}, ShareX {Format(shareXBounds)}.");
        }

        if (screens.Count != monitors.Count)
        {
            findings.Add($"Avalonia sees {screens.Count} screens but Windows lists {monitors.Count} monitors.");
        }

        if (nativeUnion.X < 0 || nativeUnion.Y < 0)
        {
            findings.Add($"The desktop extends left of or above the primary monitor (origin {nativeUnion.X}, {nativeUnion.Y}).");
        }

        report.AppendLine();
    }

    private static void AppendMonitors(StringBuilder report, List<string> findings, List<MonitorData> monitors)
    {
        foreach (MonitorData monitor in monitors)
        {
            DisplayMonitorInfo native = monitor.Native;
            Section(report, $"Monitor {monitor.Number}{(native.Primary ? " (primary)" : "")}");

            Line(report, "Name", monitor.Name);
            Line(report, "Device", native.DeviceName);
            if (monitor.Target != null)
            {
                Line(report, "Connection", monitor.Target.OutputTechnology);
            }
            Line(report, "Graphics adapter", monitor.Adapter?.DeviceString ?? "Unknown");
            Line(report, "Bounds", Format(native.Bounds));
            Line(report, "Working area", $"{Format(native.WorkingArea)}  insets {FormatInsets(native.Bounds, native.WorkingArea)}");

            if (monitor.Mode is DisplayModeInfo mode)
            {
                Line(report, "Display mode", $"{mode.Width} x {mode.Height} at {mode.Position.X}, {mode.Position.Y}, " +
                    $"{mode.Frequency} Hz, {mode.BitsPerPixel}-bit, {GetOrientationName(mode.Orientation)}");

                if (mode.Width != native.Bounds.Width || mode.Height != native.Bounds.Height)
                {
                    findings.Add($"Monitor {monitor.Number} reports {native.Bounds.Width} x {native.Bounds.Height} to ShareX but runs at " +
                        $"{mode.Width} x {mode.Height}, so ShareX is being given scaled (virtualised) coordinates.");
                }

                if (mode.Orientation != 0)
                {
                    findings.Add($"Monitor {monitor.Number} is rotated ({GetOrientationName(mode.Orientation)}).");
                }
            }

            int scale = monitor.ScalePercent;
            Line(report, "Scale", $"{scale}%");
            Line(report, "Effective DPI", FormatDpi(monitor.EffectiveDpi));
            Line(report, "Angular DPI", FormatDpi(monitor.AngularDpi));
            Line(report, "Raw DPI", FormatDpi(monitor.RawDpi));
            Line(report, "Windows scale factor", monitor.WindowsScale is int windowsScale ? $"{windowsScale}%" : "Unknown");
            Line(report, "Size in logical pixels", $"{Math.Round(native.Bounds.Width * 100.0 / scale)} x " +
                $"{Math.Round(native.Bounds.Height * 100.0 / scale)}");

            if (monitor.AvaloniaScreen is Screen screen)
            {
                Line(report, "Avalonia", $"scaling {screen.Scaling.ToString("0.###", CultureInfo.InvariantCulture)}, " +
                    $"bounds {Format(screen.Bounds)}, working area {Format(screen.WorkingArea)}");

                if (Math.Abs(screen.Scaling * 100 - scale) >= 1)
                {
                    findings.Add($"Avalonia scales monitor {monitor.Number} by {screen.Scaling * 100:0}% but Windows uses {scale}%.");
                }

                if (Format(screen.Bounds) != Format(native.Bounds))
                {
                    findings.Add($"Avalonia places monitor {monitor.Number} at {Format(screen.Bounds)} but Windows has it at {Format(native.Bounds)}.");
                }
            }
            else
            {
                Line(report, "Avalonia", "Not matched to a screen");
            }

            if (monitor.Target is DisplayTargetInfo target)
            {
                if (target.RefreshRate > 0)
                {
                    Line(report, "Refresh rate", $"{target.RefreshRate.ToString("0.##", CultureInfo.InvariantCulture)} Hz");
                }

                string hdr = !target.AdvancedColorSupported ? "Not supported" :
                    target.AdvancedColorEnabled ? "On" : "Off";
                if (target.AdvancedColorEnabled && target.SdrWhiteLevelNits > 0)
                {
                    hdr += $", SDR content brightness {target.SdrWhiteLevelNits} nits";
                }
                if (target.BitsPerColorChannel > 0)
                {
                    hdr += $", {target.BitsPerColorChannel} bits per channel";
                }
                Line(report, "HDR", hdr);
            }

            report.AppendLine();
        }

        List<int> scales = monitors.Select(x => x.ScalePercent).Distinct().OrderBy(x => x).ToList();
        if (scales.Count > 1)
        {
            findings.Insert(0, $"Monitors use different scaling ({string.Join(", ", scales.Select(x => x + "%"))}). " +
                "Most region capture offset reports come from mixed scaling.");
        }
    }

    private static void AppendArrangement(StringBuilder report, List<string> findings, List<MonitorData> monitors)
    {
        Section(report, "Arrangement");

        if (monitors.Count < 2)
        {
            report.AppendLine("  Single monitor.");
            report.AppendLine();
            return;
        }

        foreach (MonitorData monitor in monitors)
        {
            Rectangle a = Drawing(monitor.Native.Bounds);
            bool touchesAny = false;

            foreach (MonitorData other in monitors.Where(x => x.Number > monitor.Number))
            {
                Rectangle b = Drawing(other.Native.Bounds);
                string? relation = null;
                int startOffset = 0, endOffset = 0;

                if (a.Right == b.Left || b.Right == a.Left)
                {
                    if (Math.Min(a.Bottom, b.Bottom) > Math.Max(a.Top, b.Top))
                    {
                        relation = a.Right == b.Left ? "left of" : "right of";
                        startOffset = b.Top - a.Top;
                        endOffset = b.Bottom - a.Bottom;
                    }
                }
                else if (a.Bottom == b.Top || b.Bottom == a.Top)
                {
                    if (Math.Min(a.Right, b.Right) > Math.Max(a.Left, b.Left))
                    {
                        relation = a.Bottom == b.Top ? "above" : "below";
                        startOffset = b.Left - a.Left;
                        endOffset = b.Right - a.Right;
                    }
                }

                if (relation == null)
                {
                    if (a.IntersectsWith(b))
                    {
                        report.AppendLine($"  Monitor {monitor.Number} overlaps monitor {other.Number}.");
                        findings.Add($"Monitors {monitor.Number} and {other.Number} overlap ({Format(Rectangle.Intersect(a, b))}).");
                        touchesAny = true;
                    }

                    continue;
                }

                touchesAny = true;
                bool vertical = relation is "left of" or "right of";
                string edges = startOffset == 0 && endOffset == 0
                    ? "edges aligned"
                    : vertical
                        ? $"top edges differ by {startOffset} px, bottom edges by {endOffset} px"
                        : $"left edges differ by {startOffset} px, right edges by {endOffset} px";
                report.AppendLine($"  Monitor {monitor.Number} is {relation} monitor {other.Number}: {edges}.");

                if ((startOffset != 0 || endOffset != 0) && monitor.ScalePercent != other.ScalePercent)
                {
                    findings.Add($"Monitors {monitor.Number} ({monitor.ScalePercent}%) and {other.Number} ({other.ScalePercent}%) " +
                        "have different scaling and are not aligned, so one overhangs the other. This arrangement is linked to region capture offsets.");
                }
            }

            if (!touchesAny && monitors.Where(x => x != monitor).All(x => !Touches(a, Drawing(x.Native.Bounds))))
            {
                report.AppendLine($"  Monitor {monitor.Number} does not touch any other monitor.");
                findings.Add($"Monitor {monitor.Number} does not touch any other monitor, leaving a gap in the desktop.");
            }
        }

        report.AppendLine();
    }

    private static bool Touches(Rectangle a, Rectangle b)
    {
        Rectangle inflated = Rectangle.Inflate(a, 1, 1);
        return inflated.IntersectsWith(b);
    }

    private static void AppendTaskbars(StringBuilder report, List<string> findings, List<MonitorData> monitors)
    {
        Section(report, "Taskbars and app bars");

        List<Rectangle> taskbars = [];

        if (Native.GetTaskbar() is var (edge, platformBounds, autoHide))
        {
            Rectangle bounds = Drawing(platformBounds);
            taskbars.Add(bounds);
            Line(report, "Main taskbar", $"{GetEdgeName(edge)}, {Format(bounds)}{(autoHide ? ", auto-hide" : "")}" +
                $", on monitor {FindMonitorNumber(monitors, bounds)}");
            if (edge != 3)
            {
                findings.Add($"The taskbar is on the {GetEdgeName(edge).ToLowerInvariant()} edge. Side and top taskbars have caused overlay offsets.");
            }
        }
        else
        {
            Line(report, "Main taskbar", "Not found");
        }

        foreach (PlatformRectangle secondaryBounds in Native.GetSecondaryTaskbars())
        {
            Rectangle secondary = Drawing(secondaryBounds);
            taskbars.Add(secondary);
            Line(report, "Secondary taskbar", $"{Format(secondary)}, on monitor {FindMonitorNumber(monitors, secondary)}");
        }

        // Work-area insets that no taskbar explains belong to app bars such as yasb or docked toolbars.
        foreach (MonitorData monitor in monitors)
        {
            Rectangle area = Drawing(monitor.Native.Bounds);
            Rectangle work = Drawing(monitor.Native.WorkingArea);
            (string Edge, int Size)[] insets =
            [
                ("left", work.Left - area.Left),
                ("top", work.Top - area.Top),
                ("right", area.Right - work.Right),
                ("bottom", area.Bottom - work.Bottom)
            ];

            foreach ((string insetEdge, int size) in insets.Where(x => x.Size > 0))
            {
                Rectangle strip = insetEdge switch
                {
                    "left" => new Rectangle(area.Left, area.Top, size, area.Height),
                    "top" => new Rectangle(area.Left, area.Top, area.Width, size),
                    "right" => new Rectangle(area.Right - size, area.Top, size, area.Height),
                    _ => new Rectangle(area.Left, area.Bottom - size, area.Width, size)
                };

                if (!taskbars.Any(x => x.IntersectsWith(strip)))
                {
                    Line(report, "Other app bar", $"monitor {monitor.Number}, {insetEdge} edge, {size} px");
                    findings.Add($"Something other than the taskbar reserves {size} px at the {insetEdge} of monitor {monitor.Number} " +
                        "(an app bar such as yasb or a docked toolbar). App bars have shifted capture overlays.");
                }
            }
        }

        report.AppendLine();
    }

    private static void AppendGraphics(StringBuilder report)
    {
        Section(report, "Graphics adapters");

        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);
        foreach (DisplayDeviceInfo adapter in Native.GetAttachedAdapters())
        {
            string key = adapter.DeviceString + "|" + adapter.DeviceKey;
            if (!seen.Add(key))
            {
                continue;
            }

            string driver = GetDriverVersion(adapter.DeviceKey);
            Line(report, adapter.DeviceName, string.IsNullOrEmpty(driver) ? adapter.DeviceString : $"{adapter.DeviceString}, driver {driver}");
        }

        report.AppendLine();
    }

    private static string GetDriverVersion(string deviceKey) => Native.ReadDriverVersion(deviceKey);

    private static void AppendCursor(StringBuilder report, List<string> findings, List<MonitorData> monitors, Point? cursor)
    {
        Section(report, "Cursor");

        Point? physical = Drawing(Native.GetPhysicalCursorPosition());
        Line(report, "Position", cursor is Point position ? $"{position.X}, {position.Y}" : "Unknown");
        Line(report, "Physical position", physical is Point physicalPosition ? $"{physicalPosition.X}, {physicalPosition.Y}" : "Unknown");

        if (cursor is Point point)
        {
            IntPtr handle = Native.MonitorFromPoint(new PlatformPoint(point.X, point.Y));
            MonitorData? monitor = monitors.FirstOrDefault(x => x.Native.Handle == handle);
            Line(report, "On monitor", monitor != null ? monitor.Number.ToString(CultureInfo.InvariantCulture) : "None");

            if (physical is Point physicalPoint && physicalPoint != point)
            {
                findings.Add($"The cursor position differs between logical ({point.X}, {point.Y}) and physical ({physicalPoint.X}, {physicalPoint.Y}) coordinates.");
            }
        }

        report.AppendLine();
    }

    private static void AppendThisWindow(StringBuilder report, Window owner, List<MonitorData> monitors)
    {
        Section(report, "This window");

        IntPtr handle = owner.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        Rectangle? bounds = handle != IntPtr.Zero && Native.GetWindowBounds(handle) is PlatformRectangle windowRect
            ? Drawing(windowRect)
            : null;
        Line(report, "Window bounds", bounds is Rectangle rect ? Format(rect) : "Unknown");
        Line(report, "Window DPI", Native.GetDpiForWindow(handle) is uint dpi ? $"{dpi} ({Percent(dpi)})" : "Unknown");
        Line(report, "Avalonia render scaling", owner.RenderScaling.ToString("0.###", CultureInfo.InvariantCulture));
        if (bounds is Rectangle windowBounds)
        {
            Line(report, "On monitor", FindMonitorNumber(monitors, windowBounds));
        }

        report.AppendLine();
    }

    private static void AppendOverlayWindows(StringBuilder report, List<string> findings, List<MonitorData> monitors)
    {
        Section(report, "Recent capture windows");

        IReadOnlyList<WindowGeometryEntry> entries = WindowGeometryLog.Entries;
        if (entries.Count == 0)
        {
            report.AppendLine("  None yet. Start a region capture, close it, then press Refresh to add its measurements.");
            report.AppendLine();
            return;
        }

        foreach (WindowGeometryEntry entry in entries.Reverse())
        {
            report.AppendLine($"  {entry.Time.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} {entry.Source}");
            bool positionOnly = entry.ExpectedBounds.Width == 0 && entry.ExpectedBounds.Height == 0;
            Line(report, "  Expected", positionOnly
                ? $"position {entry.ExpectedBounds.X}, {entry.ExpectedBounds.Y}"
                : Format(entry.ExpectedBounds));
            Line(report, "  Actual window", Format(entry.ActualBounds));
            Line(report, "  Client origin", $"{entry.ClientOrigin.X}, {entry.ClientOrigin.Y}");
            Line(report, "  Window DPI", entry.WindowDpi > 0 ? $"{entry.WindowDpi} ({Percent(entry.WindowDpi)})" : "Unknown");
            Line(report, "  Avalonia render scaling", entry.RenderScaling.ToString("0.###", CultureInfo.InvariantCulture));
            Line(report, "  Client size x scaling", $"{Math.Round(entry.ClientSize.Width * entry.RenderScaling)} x " +
                $"{Math.Round(entry.ClientSize.Height * entry.RenderScaling)}");
            if (!entry.ImageSize.IsEmpty)
            {
                Line(report, "  Screenshot size", $"{entry.ImageSize.Width} x {entry.ImageSize.Height}");
            }
            Line(report, "  Monitor", entry.MonitorDeviceName);

            if (entry.DpiChanges is WindowDpiChanges dpiChanges)
            {
                Line(report, "  DPI changes", dpiChanges.Received == 0
                    ? "None"
                    : $"{dpiChanges.Received} received, {dpiChanges.Suppressed} dropped: {dpiChanges.History}");

                if (dpiChanges.Suppressed > 0)
                {
                    findings.Add($"Windows changed the DPI of the {entry.Source.ToLowerInvariant()} window {dpiChanges.Suppressed} time(s) " +
                        $"while it was open ({dpiChanges.History}) and ShareX dropped the change, so Avalonia kept its original scale.");
                }
            }

            if (entry.WindowDpi > 0 && Math.Abs(entry.WindowDpi / 96.0 - entry.RenderScaling) > 0.01)
            {
                findings.Add($"The {entry.Source.ToLowerInvariant()} window runs at {Percent(entry.WindowDpi)} in Windows but Avalonia renders it " +
                    $"at {entry.RenderScaling * 100:0}%. Toolbar, magnifier and drawing positions can be scaled twice or not at all.");
            }

            if (entry.CursorScreen is Point cursorScreen && entry.CursorAvaloniaClientPixels is PointF avaloniaCursor)
            {
                Point expectedCursor = new(cursorScreen.X - entry.ClientOrigin.X, cursorScreen.Y - entry.ClientOrigin.Y);
                Line(report, "  Cursor in window", $"Windows {expectedCursor.X}, {expectedCursor.Y}; " +
                    $"Avalonia {Math.Round(avaloniaCursor.X)}, {Math.Round(avaloniaCursor.Y)}");

                if (Math.Abs(avaloniaCursor.X - expectedCursor.X) > 2 || Math.Abs(avaloniaCursor.Y - expectedCursor.Y) > 2)
                {
                    findings.Add($"In the {entry.Source.ToLowerInvariant()} window Avalonia places the cursor at " +
                        $"{Math.Round(avaloniaCursor.X)}, {Math.Round(avaloniaCursor.Y)} but Windows has it at {expectedCursor.X}, {expectedCursor.Y}, " +
                        "so the magnifier and selections use the wrong pixels.");
                }
            }

            foreach (WindowGeometryCheck check in entry.Checks)
            {
                bool compared = check.CompareX || check.CompareY;
                Line(report, "  " + check.Name, $"expected {check.Expected.X}, {check.Expected.Y}; actual {check.Actual.X}, {check.Actual.Y}" +
                    (compared ? check.Matches() ? "" : "  MISMATCH" : ""));

                if (compared && !check.Matches())
                {
                    findings.Add($"{entry.Source}: {check.Name.ToLowerInvariant()} is at {check.Actual.X}, {check.Actual.Y} instead of " +
                        $"{check.Expected.X}, {check.Expected.Y}.");
                }
            }

            bool positionWrong = entry.ActualBounds.X != entry.ExpectedBounds.X || entry.ActualBounds.Y != entry.ExpectedBounds.Y;
            bool sizeWrong = !positionOnly &&
                (entry.ActualBounds.Width != entry.ExpectedBounds.Width || entry.ActualBounds.Height != entry.ExpectedBounds.Height);
            if (positionWrong || sizeWrong)
            {
                findings.Add($"The {entry.Source.ToLowerInvariant()} window opened at {Format(entry.ActualBounds)} instead of " +
                    $"{(positionOnly ? $"{entry.ExpectedBounds.X}, {entry.ExpectedBounds.Y}" : Format(entry.ExpectedBounds))}.");
            }

            if (!entry.ImageSize.IsEmpty &&
                (Math.Abs(entry.ClientSize.Width * entry.RenderScaling - entry.ImageSize.Width) > 1 ||
                 Math.Abs(entry.ClientSize.Height * entry.RenderScaling - entry.ImageSize.Height) > 1))
            {
                findings.Add($"The {entry.Source.ToLowerInvariant()} window covers " +
                    $"{Math.Round(entry.ClientSize.Width * entry.RenderScaling)} x {Math.Round(entry.ClientSize.Height * entry.RenderScaling)} " +
                    $"pixels but its screenshot is {entry.ImageSize.Width} x {entry.ImageSize.Height}, so selections will not line up.");
            }
        }

        report.AppendLine();
    }

    private static void AppendCaptureSettings(StringBuilder report)
    {
        Section(report, "Capture settings");

        TaskSettings? taskSettings = ApplicationState.DefaultTaskSettings;
        if (taskSettings == null)
        {
            report.AppendLine("  Not loaded.");
            return;
        }

        TaskSettingsCapture capture = taskSettings.CaptureSettings;
        Line(report, "Show cursor", YesNo(capture.ShowCursor));
        Line(report, "Capture transparent", YesNo(capture.CaptureTransparent));
        Line(report, "HDR colour correction", YesNo(capture.HDRScreenshotColorCorrection));
        Line(report, "Restrict to active monitor", YesNo(capture.RegionCaptureOptions.ActiveMonitorMode));
        Line(report, "Detect windows", YesNo(capture.RegionCaptureOptions.DetectWindows));
        Line(report, "Detect controls", YesNo(capture.RegionCaptureOptions.DetectControls));
        Line(report, "Show magnifier", YesNo(capture.RegionCaptureOptions.ShowMagnifier));
        Line(report, "Screen recorder", capture.ScreenRecordUseNative ? "Native" : $"FFmpeg ({capture.FFmpegOptions.VideoSource})");
        Line(report, "Toast monitor", taskSettings.GeneralSettings.ToastWindowMonitor +
            (string.IsNullOrEmpty(taskSettings.GeneralSettings.ToastWindowMonitorDeviceName)
                ? ""
                : $" ({taskSettings.GeneralSettings.ToastWindowMonitorDeviceName})"));
    }

    /// <summary>Other platforms: report what Avalonia knows, which is all the scaling ShareX uses there.</summary>
    private static DisplayDiagnosticsResult CollectAvaloniaOnly(Window owner)
    {
        StringBuilder report = new();
        AppendHeader(report);
        AppendApplication(report);

        Section(report, "Operating system");
        Line(report, "Description", RuntimeInformation.OSDescription);
        Line(report, "Session", Environment.GetEnvironmentVariable("XDG_SESSION_TYPE") ?? "Unknown");
        Line(report, "Desktop", Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP") ?? "Unknown");
        report.AppendLine();

        List<DisplayDiagnosticsMonitor> monitors = [];
        int number = 1;
        foreach (Screen screen in owner.Screens.All)
        {
            Rectangle bounds = new(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);
            Rectangle workingArea = new(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width, screen.WorkingArea.Height);
            int scale = (int)Math.Round(screen.Scaling * 100);
            monitors.Add(new DisplayDiagnosticsMonitor
            {
                Number = number,
                Name = screen.DisplayName ?? string.Empty,
                Primary = screen.IsPrimary,
                Bounds = bounds,
                WorkingArea = workingArea,
                ScalePercent = scale
            });

            Section(report, $"Monitor {number}{(screen.IsPrimary ? " (primary)" : "")}");
            Line(report, "Name", screen.DisplayName ?? "Unknown");
            Line(report, "Bounds", Format(bounds));
            Line(report, "Working area", Format(workingArea));
            Line(report, "Scale", $"{scale}%");
            report.AppendLine();
            number++;
        }

        return new DisplayDiagnosticsResult { Monitors = monitors, Text = report.ToString().TrimEnd() + Environment.NewLine };
    }

    private static string FindMonitorNumber(List<MonitorData> monitors, Rectangle bounds)
    {
        MonitorData? best = monitors
            .Select(x => (Monitor: x, Area: Area(Rectangle.Intersect(Drawing(x.Native.Bounds), bounds))))
            .Where(x => x.Area > 0)
            .OrderByDescending(x => x.Area)
            .Select(x => x.Monitor)
            .FirstOrDefault();
        return best?.Number.ToString(CultureInfo.InvariantCulture) ?? "none";
    }

    private static long Area(Rectangle rectangle) => (long)rectangle.Width * rectangle.Height;

    private static Rectangle Combine(IEnumerable<Rectangle> rectangles)
    {
        Rectangle? result = null;
        foreach (Rectangle rectangle in rectangles)
        {
            result = result is Rectangle current ? Rectangle.Union(current, rectangle) : rectangle;
        }

        return result ?? Rectangle.Empty;
    }

    private static void Section(StringBuilder report, string title)
    {
        report.AppendLine(title);
    }

    private static void Line(StringBuilder report, string label, string value)
    {
        report.Append("  ").Append((label + ":").PadRight(LabelWidth)).AppendLine(value);
    }

    private static string Format(PlatformRectangle rectangle) => Format(Drawing(rectangle));

    private static string Format(Rectangle rectangle) =>
        $"X={rectangle.X} Y={rectangle.Y} W={rectangle.Width} H={rectangle.Height}";

    private static string Format(Avalonia.PixelRect rectangle) =>
        $"X={rectangle.X} Y={rectangle.Y} W={rectangle.Width} H={rectangle.Height}";

    private static string FormatInsets(PlatformRectangle bounds, PlatformRectangle workingArea) =>
        FormatInsets(Drawing(bounds), Drawing(workingArea));

    private static string FormatInsets(Rectangle bounds, Rectangle workingArea) =>
        $"L={workingArea.Left - bounds.Left} T={workingArea.Top - bounds.Top} " +
        $"R={bounds.Right - workingArea.Right} B={bounds.Bottom - workingArea.Bottom}";

    private static string FormatDpi(Point? dpi) =>
        dpi is Point value ? value.X == value.Y ? $"{value.X}" : $"{value.X} x {value.Y}" : "Unknown";

    private static string Percent(uint dpi) => $"{Math.Round(dpi * 100.0 / 96)}%";

    private static string YesNo(bool value) => value ? "Yes" : "No";

    private static string GetOrientationName(int orientation) => orientation switch
    {
        1 => "portrait (90°)",
        2 => "landscape flipped (180°)",
        3 => "portrait flipped (270°)",
        _ => "landscape"
    };

    private static string GetEdgeName(int edge) => edge switch
    {
        0 => "Left",
        1 => "Top",
        2 => "Right",
        _ => "Bottom"
    };
}
