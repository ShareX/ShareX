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

using ShareX.HelpersLib;
using ShareX.Platform;
using ShareX.Platform.Linux;
using ShareX.Platform.MacOS;
using ShareX.Platform.Windows;
using System;

namespace ShareX;

/// <summary>Chooses the platform services for the operating system ShareX is running on.</summary>
internal static class PlatformBootstrap
{
    /// <remarks>
    /// The only operating system check allowed outside the platform projects (AGENTS.md). Everything else asks
    /// <see cref="PlatformServices.Current"/>.
    /// </remarks>
    public static IPlatformServices CreateForCurrentOS()
    {
        if (OperatingSystem.IsWindows())
        {
            return new WindowsPlatformServices();
        }

        if (OperatingSystem.IsMacOS())
        {
            return new MacPlatformServices();
        }

        if (OperatingSystem.IsLinux())
        {
            return new LinuxPlatformServices(PlatformDetector.Detect(), ShareX.Platform.Diagnostics.CommandRunner.Default, DescribeHotkey);
        }

        throw new PlatformNotSupportedException("ShareX runs on Windows, macOS and Linux.");
    }

    /// <summary>What the desktop shows for a global hotkey in its shortcut settings, for example "Capture region".</summary>
    private static string DescribeHotkey(int id)
    {
        HotkeySettings hotkey = ApplicationState.HotkeyManager?.Hotkeys?.Find(x => x.HotkeyInfo.ID == id);
        return hotkey?.TaskSettings?.ToString() ?? $"ShareX hotkey {id}";
    }

    /// <summary>
    /// The application's message host. Windows keeps its hidden window for the notification area icon and session end messages;
    /// elsewhere Avalonia's TrayIcon needs no host window.
    /// </summary>
    /// <remarks>Migration debt (J3, R15): goes away when the Windows tray and session messages move into ShareX.Platform.Windows.</remarks>
    public static IHotkeyHost CreateApplicationHost()
    {
        return OperatingSystem.IsWindows() ? new WindowsHotkeyHost() : new WindowlessHotkeyHost();
    }
}
