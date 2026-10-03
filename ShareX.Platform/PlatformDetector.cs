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
using System.IO;

namespace ShareX.Platform;

/// <summary>Detects the current operating system, display server and desktop environment.</summary>
public static class PlatformDetector
{
    public static PlatformInfo Detect()
    {
        OperatingSystemKind os = OperatingSystem.IsWindows() ? OperatingSystemKind.Windows :
            OperatingSystem.IsMacOS() ? OperatingSystemKind.MacOS :
            OperatingSystem.IsLinux() ? OperatingSystemKind.Linux :
            OperatingSystemKind.Unknown;

        return Detect(os, Environment.GetEnvironmentVariable, File.Exists, os == OperatingSystemKind.Linux ? LinuxDistribution.Detect() : null);
    }

    /// <summary>Detects the platform from the supplied environment, which keeps the logic testable on any OS.</summary>
    public static PlatformInfo Detect(OperatingSystemKind os, Func<string, string?> getEnvironmentVariable, Func<string, bool> fileExists,
        LinuxDistribution? distribution = null)
    {
        switch (os)
        {
            case OperatingSystemKind.Windows:
                return new PlatformInfo(os, DisplayServer.Win32, DesktopEnvironment.Windows, "Windows", false);
            case OperatingSystemKind.MacOS:
                return new PlatformInfo(os, DisplayServer.Quartz, DesktopEnvironment.MacOS, "macOS",
                    !string.IsNullOrEmpty(getEnvironmentVariable("APP_SANDBOX_CONTAINER_ID")));
            case OperatingSystemKind.Linux:
                DisplayServer displayServer = DetectDisplayServer(getEnvironmentVariable);
                (DesktopEnvironment desktop, string desktopName) = DetectDesktopEnvironment(getEnvironmentVariable);
                bool sandboxed = !string.IsNullOrEmpty(getEnvironmentVariable("FLATPAK_ID")) || fileExists("/.flatpak-info") ||
                    !string.IsNullOrEmpty(getEnvironmentVariable("SNAP"));
                return new PlatformInfo(os, displayServer, desktop, desktopName, sandboxed) { Distribution = distribution ?? LinuxDistribution.Unknown };
            default:
                return new PlatformInfo(os, DisplayServer.Unknown, DesktopEnvironment.Unknown, "Unknown", false);
        }
    }

    internal static DisplayServer DetectDisplayServer(Func<string, string?> getEnvironmentVariable)
    {
        string? sessionType = getEnvironmentVariable("XDG_SESSION_TYPE");

        if (string.Equals(sessionType, "wayland", StringComparison.OrdinalIgnoreCase) ||
            !string.IsNullOrEmpty(getEnvironmentVariable("WAYLAND_DISPLAY")))
        {
            return DisplayServer.Wayland;
        }

        if (string.Equals(sessionType, "x11", StringComparison.OrdinalIgnoreCase) ||
            !string.IsNullOrEmpty(getEnvironmentVariable("DISPLAY")))
        {
            return DisplayServer.X11;
        }

        return DisplayServer.None;
    }

    internal static (DesktopEnvironment Desktop, string Name) DetectDesktopEnvironment(Func<string, string?> getEnvironmentVariable)
    {
        if (!string.IsNullOrEmpty(getEnvironmentVariable("HYPRLAND_INSTANCE_SIGNATURE")))
        {
            return (DesktopEnvironment.Hyprland, "Hyprland");
        }

        if (!string.IsNullOrEmpty(getEnvironmentVariable("SWAYSOCK")))
        {
            return (DesktopEnvironment.Sway, "sway");
        }

        // XDG_CURRENT_DESKTOP is a colon separated list, for example "ubuntu:GNOME" or "KDE".
        string? currentDesktop = getEnvironmentVariable("XDG_CURRENT_DESKTOP");

        if (string.IsNullOrEmpty(currentDesktop))
        {
            currentDesktop = getEnvironmentVariable("DESKTOP_SESSION");
        }

        if (string.IsNullOrEmpty(currentDesktop))
        {
            return (DesktopEnvironment.Unknown, "Unknown");
        }

        foreach (string part in currentDesktop.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            DesktopEnvironment desktop = part.ToUpperInvariant() switch
            {
                "GNOME" or "GNOME-CLASSIC" or "GNOME-FLASHBACK" or "UNITY" or "POP" => DesktopEnvironment.Gnome,
                "KDE" or "PLASMA" => DesktopEnvironment.Kde,
                "XFCE" => DesktopEnvironment.Xfce,
                "X-CINNAMON" or "CINNAMON" => DesktopEnvironment.Cinnamon,
                "MATE" => DesktopEnvironment.Mate,
                "LXQT" => DesktopEnvironment.Lxqt,
                "BUDGIE" or "BUDGIE-DESKTOP" => DesktopEnvironment.Budgie,
                "HYPRLAND" => DesktopEnvironment.Hyprland,
                "SWAY" => DesktopEnvironment.Sway,
                _ => DesktopEnvironment.Unknown
            };

            if (desktop != DesktopEnvironment.Unknown)
            {
                return (desktop, part);
            }
        }

        return (DesktopEnvironment.Other, currentDesktop);
    }
}
