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

namespace ShareX.Platform;

public enum OperatingSystemKind
{
    Unknown,
    Windows,
    MacOS,
    Linux
}

public enum DisplayServer
{
    Unknown,
    /// <summary>No graphical session is available (for example SSH or a CI runner).</summary>
    None,
    Win32,
    Quartz,
    X11,
    Wayland
}

public enum DesktopEnvironment
{
    Unknown,
    Windows,
    MacOS,
    Gnome,
    Kde,
    Xfce,
    Cinnamon,
    Mate,
    Lxqt,
    Budgie,
    Hyprland,
    Sway,
    Other
}

/// <summary>Describes the operating system and graphical session ShareX is running in.</summary>
public sealed record PlatformInfo(
    OperatingSystemKind OperatingSystem,
    DisplayServer DisplayServer,
    DesktopEnvironment DesktopEnvironment,
    string DesktopEnvironmentName,
    bool IsSandboxed)
{
    /// <summary>The Linux distribution from /etc/os-release. Null on Windows and macOS.</summary>
    public LinuxDistribution? Distribution { get; init; }

    public bool IsWindows => OperatingSystem == OperatingSystemKind.Windows;

    public bool IsMacOS => OperatingSystem == OperatingSystemKind.MacOS;

    public bool IsLinux => OperatingSystem == OperatingSystemKind.Linux;

    public bool IsWayland => DisplayServer == DisplayServer.Wayland;

    public bool IsX11 => DisplayServer == DisplayServer.X11;

    /// <summary>True for compositors built on wlroots or similar which expose wlr protocols (grim, wf-recorder).</summary>
    public bool IsWlrootsCompositor => IsWayland && DesktopEnvironment is DesktopEnvironment.Sway or DesktopEnvironment.Hyprland;

    public override string ToString()
    {
        string sandbox = IsSandboxed ? ", sandboxed" : "";
        string distribution = Distribution != null ? $"{Distribution.PrettyName}, " : "";
        return $"{OperatingSystem} ({distribution}{DisplayServer}, {DesktopEnvironmentName}{sandbox})";
    }
}
