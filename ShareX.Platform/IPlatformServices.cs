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

namespace ShareX.Platform;

/// <summary>
/// Entry point to everything that differs between Windows, macOS and Linux. Implemented by
/// ShareX.Platform.Windows, ShareX.Platform.MacOS and ShareX.Platform.Linux.
/// </summary>
/// <remarks>
/// The system tray is not part of this interface because Avalonia's TrayIcon already covers NotifyIcon,
/// NSStatusItem and StatusNotifierItem.
/// </remarks>
public interface IPlatformServices : IDisposable
{
    PlatformInfo Info { get; }

    IPathService Paths { get; }

    IStartupService Startup { get; }

    IClipboardService Clipboard { get; }

    IScreenCaptureService ScreenCapture { get; }

    IScreenRecordingService ScreenRecording { get; }

    IHotkeyService Hotkeys { get; }

    IWindowService Windows { get; }

    INotificationService Notifications { get; }

    IShellService Shell { get; }

    IShellIntegrationService ShellIntegration { get; }

    ICredentialService Credentials { get; }

    /// <summary>Encrypts values stored in settings files.</summary>
    ISecretProtectionService Secrets { get; }

    /// <summary>File thumbnails from the OS.</summary>
    IThumbnailService Thumbnails { get; }

    /// <summary>OS preferences such as the mouse wheel scroll amount.</summary>
    ISystemPreferencesService Preferences { get; }

    /// <summary>Operating system name, elevation and similar facts.</summary>
    ISystemInfoService SystemInfo { get; }
}
