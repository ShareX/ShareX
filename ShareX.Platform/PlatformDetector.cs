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

        return Detect(os, Environment.GetEnvironmentVariable, File.Exists);
    }

    /// <summary>Detects the platform from the supplied environment, which keeps the logic testable on any OS.</summary>
    public static PlatformInfo Detect(OperatingSystemKind os, Func<string, string?> getEnvironmentVariable, Func<string, bool> fileExists)
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
                return new PlatformInfo(os, displayServer, desktop, desktopName, sandboxed);
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
