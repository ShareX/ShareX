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

using ShareX.Platform.Diagnostics;
using ShareX.Platform.Linux.DBus;
using ShareX.Platform.Linux.Desktop;
using System;
using System.Runtime.Versioning;

namespace ShareX.Platform.Linux;

/// <summary>Linux platform services. Picks X11 or Wayland backends from the running session and tailors hints to the distribution.</summary>
[SupportedOSPlatform("linux")]
public sealed class LinuxPlatformServices : IPlatformServices
{
    private readonly Lazy<IHotkeyService> hotkeys;
    private readonly Lazy<IScreenCaptureService> screenCapture;

    public LinuxPlatformServices()
        : this(PlatformDetector.Detect(), CommandRunner.Default)
    {
    }

    public LinuxPlatformServices(PlatformInfo info, ICommandRunner runner, Func<int, string>? describeHotkey = null)
    {
        Info = info;
        XdgPathService paths = new XdgPathService();
        Paths = paths;
        Startup = new XdgAutostartService(paths);
        Clipboard = new LinuxClipboardService(info, runner);
        LinuxWindowService windows = new LinuxWindowService(info, runner);
        Windows = windows;
        Input = new LinuxInputService(info, runner);
        WindowManagement = new LinuxWindowManagementService(windows, runner);
        screenCapture = new Lazy<IScreenCaptureService>(() => new LinuxScreenCaptureService(info, runner, windows));
        ScreenRecording = new LinuxScreenRecordingService(info, runner, () => screenCapture.Value.GetScreens());
        Desktop = new LinuxDesktop(info);
        hotkeys = new Lazy<IHotkeyService>(() => Desktop.CreateHotkeyService(runner, describeHotkey));
        Notifications = new FreedesktopNotificationService(info, runner);
        Shell = new LinuxShellService(runner);
        ShellIntegration = new LinuxShellIntegrationService(paths, runner);
        Credentials = new SecretServiceCredentialService();
        Secrets = new KeyFileSecretProtectionService(System.IO.Path.Combine(paths.GetConfigDirectory("ShareX"), "secret.key"));
        Thumbnails = new FreedesktopThumbnailService(System.IO.Path.Combine(paths.CacheHome, "thumbnails"));
        Preferences = new DefaultSystemPreferencesService("/etc/sharex/policy.json", System.IO.Path.Combine(paths.GetConfigDirectory("ShareX"), "policy.json"));
        SystemInfo = new UnixSystemInfoService(info, runner);
        Ocr = new TesseractOcrService(info, runner);
        Printing = new CupsPrintService(info, runner);
        Wallpaper = new LinuxDesktopWallpaperService(runner);
        Sounds = new CommandLineSoundService(runner, ["pw-play", "paplay", "aplay"], LinuxPackages.Missing(info.Distribution ?? LinuxDistribution.Unknown, LinuxTool.PipeWire));
    }

    public PlatformInfo Info { get; }

    /// <summary>The desktop kind and the per-desktop strategies the services use.</summary>
    public LinuxDesktop Desktop { get; }

    public IPathService Paths { get; }

    public IStartupService Startup { get; }

    public IClipboardService Clipboard { get; }

    public IScreenCaptureService ScreenCapture => screenCapture.Value;

    public IScreenRecordingService ScreenRecording { get; }

    /// <summary>Created on first use because both backends start a connection to the display server or session bus.</summary>
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

    public ICodeSignatureService CodeSignature { get; } = new UnsupportedCodeSignatureService("Updates on Linux come from your package manager, which checks signatures itself.");

    public IOcrService Ocr { get; }

    public ITaskbarService Taskbar { get; } = new UnityLauncherTaskbarService();

    public ISystemGraphicsService Graphics { get; } = new XcursorGraphicsService();

    public IPrintService Printing { get; }

    public ISoundService Sounds { get; }

    public IDesktopWallpaperService Wallpaper { get; }

    public IApplicationSessionService Session { get; } = new UnsupportedApplicationSessionService("Linux desktops restore applications through their own session settings.");

    public ITrayService Tray { get; } = new UnsupportedTrayService();

    public IApplicationLaunchService ApplicationLaunch { get; } = new LinuxApplicationLaunchService();

    public void Dispose()
    {
        if (hotkeys.IsValueCreated)
        {
            hotkeys.Value.Dispose();
        }
    }
}
