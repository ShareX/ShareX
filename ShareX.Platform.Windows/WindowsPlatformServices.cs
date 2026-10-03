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
using System.Runtime.Versioning;

namespace ShareX.Platform.Windows;

/// <summary>Windows platform services built on Win32 only, so they can back both the WinForms and the Avalonia front ends.</summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsPlatformServices : IPlatformServices
{
    private readonly Lazy<IHotkeyService> hotkeys = new Lazy<IHotkeyService>(() => new WindowsHotkeyService());

    public WindowsPlatformServices()
        : this(PlatformDetector.Detect())
    {
    }

    public WindowsPlatformServices(PlatformInfo info)
    {
        Info = info;
        Paths = new WindowsPathService();
        Startup = new WindowsStartupService();
        Clipboard = new WindowsClipboardService();
        WindowsScreenCaptureService screenCapture = new WindowsScreenCaptureService();
        ScreenCapture = screenCapture;
        ScreenRecording = new WindowsScreenRecordingService(screenCapture.GetScreens);
        Windows = new WindowsWindowService();
        Input = new WindowsInputService();
        WindowManagement = new WindowsWindowManagementService(screenCapture);
        Notifications = new UnsupportedNotificationService("ShareX shows its own notification window on Windows.");
        Shell = new WindowsShellService();
        ShellIntegration = new WindowsShellIntegrationService();
        Credentials = new WindowsCredentialService();
        Secrets = new WindowsSecretProtectionService();
        Thumbnails = new WindowsThumbnailService();
        Preferences = new WindowsSystemPreferencesService();
        SystemInfo = new WindowsSystemInfoService();
#if WINDOWS10_0_17763_0_OR_GREATER
        Ocr = OperatingSystem.IsWindowsVersionAtLeast(10, 0, 18362)
            ? new WindowsOcrService()
            : new UnsupportedOcrService("Text recognition needs Windows 10 version 1903 or later.");
#else
        Ocr = new UnsupportedOcrService("Text recognition uses the Windows Runtime, which this build of ShareX was compiled without.");
#endif
    }

    public PlatformInfo Info { get; }

    public IPathService Paths { get; }

    public IStartupService Startup { get; }

    public IClipboardService Clipboard { get; }

    public IScreenCaptureService ScreenCapture { get; }

    public IScreenRecordingService ScreenRecording { get; }

    /// <summary>Created on first use because it starts a message loop thread.</summary>
    public IHotkeyService Hotkeys => hotkeys.Value;

    public IWindowService Windows { get; }

    public IInputService Input { get; }

    public IWindowManagementService WindowManagement { get; }

    public INotificationService Notifications { get; }

    public IShellService Shell { get; }

    public IShellIntegrationService ShellIntegration { get; }

    public ICredentialService Credentials { get; }

    public ISecretProtectionService Secrets { get; }

    public IThumbnailService Thumbnails { get; }

    public ISystemPreferencesService Preferences { get; }

    public ISystemInfoService SystemInfo { get; }

    public ICodeSignatureService CodeSignature { get; } = new WindowsCodeSignatureService();

    public IOcrService Ocr { get; }

    public ITaskbarService Taskbar { get; } = new WindowsTaskbarService();

    public ISystemGraphicsService Graphics { get; } = new WindowsSystemGraphicsService();

    public IPrintService Printing { get; } = new WindowsPrintService();

    public ISoundService Sounds { get; } = new WindowsSoundService();

    public IDesktopWallpaperService Wallpaper { get; } = new WindowsDesktopWallpaperService();

    public IApplicationSessionService Session { get; } = new WindowsApplicationSessionService();

    public ITrayService Tray { get; } = new WindowsTrayService();

    public IApplicationLaunchService ApplicationLaunch { get; } = new WindowsApplicationLaunchService();

    public void Dispose()
    {
        Session.Dispose();
        ((WindowsSystemGraphicsService)Graphics).Dispose();
        if (hotkeys.IsValueCreated)
        {
            hotkeys.Value.Dispose();
        }
    }
}
