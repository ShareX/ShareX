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
using System;
using System.Runtime.Versioning;

namespace ShareX.Platform.MacOS;

/// <summary>macOS platform services for Apple Silicon and Intel Macs.</summary>
[SupportedOSPlatform("macos")]
public sealed class MacPlatformServices : IPlatformServices
{
    private readonly Lazy<IHotkeyService> hotkeys;

    public MacPlatformServices()
        : this(PlatformDetector.Detect(), CommandRunner.Default)
    {
    }

    public MacPlatformServices(PlatformInfo info, ICommandRunner runner)
    {
        Info = info;
        MacPathService paths = new MacPathService();
        Paths = paths;
        Startup = new LaunchAgentStartupService(paths, runner);
        Printing = new CupsPrintService(info, runner);
        Sounds = new CommandLineSoundService(runner, ["afplay"], FeatureSupport.NotSupported("afplay is missing."));
        Clipboard = new MacClipboardService();
        MacWindowService windows = new MacWindowService();
        MacScreenCaptureService screenCapture = new MacScreenCaptureService(runner, windows);
        ScreenCapture = screenCapture;
        ScreenRecording = new MacScreenRecordingService(runner, screenCapture.GetScreens);
        hotkeys = new Lazy<IHotkeyService>(() => new CarbonHotkeyService());
        Windows = windows;
        Input = new MacInputService();
        Notifications = new MacNotificationService(runner);
        Shell = new MacShellService(runner);
        ShellIntegration = new MacShellIntegrationService();
        Credentials = new KeychainCredentialService();
        Secrets = new KeyFileSecretProtectionService(System.IO.Path.Combine(paths.GetConfigDirectory("ShareX"), "secret.key"));
        Thumbnails = new UnsupportedThumbnailService("macOS file thumbnails need QuickLook, which the macOS build does not wrap yet.");
        Preferences = new DefaultSystemPreferencesService("/Library/Application Support/ShareX/policy.json", System.IO.Path.Combine(paths.GetConfigDirectory("ShareX"), "policy.json"));
        SystemInfo = new MacSystemInfoService(runner);
    }

    public PlatformInfo Info { get; }

    public IPathService Paths { get; }

    public IStartupService Startup { get; }

    public IClipboardService Clipboard { get; }

    public IScreenCaptureService ScreenCapture { get; }

    public IScreenRecordingService ScreenRecording { get; }

    /// <summary>Created on first use, from the main thread, because Carbon installs its handler on the application event target.</summary>
    public IHotkeyService Hotkeys => hotkeys.Value;

    public IWindowService Windows { get; }

    public IInputService Input { get; }

    public IWindowManagementService WindowManagement { get; } =
        new UnsupportedWindowManagementService("Inspecting and changing other applications' windows is not available on macOS yet.");

    public INotificationService Notifications { get; }

    public IShellService Shell { get; }

    public IShellIntegrationService ShellIntegration { get; }

    public ICredentialService Credentials { get; }

    public ISecretProtectionService Secrets { get; }

    public IThumbnailService Thumbnails { get; }

    public ISystemPreferencesService Preferences { get; }

    public ISystemInfoService SystemInfo { get; }

    /// <summary>Apple's Vision framework is not wired up yet.</summary>
    public ICodeSignatureService CodeSignature { get; } = new UnsupportedCodeSignatureService("Installer signatures are only checked on Windows.");

    public IOcrService Ocr { get; } = new UnsupportedOcrService("Text recognition is not available on macOS yet.");

    public ITaskbarService Taskbar { get; } = new UnsupportedTaskbarService("Dock icon progress is not available on macOS yet.");

    public ISystemGraphicsService Graphics { get; } = new UnsupportedSystemGraphicsService("Emoji and cursor images are drawn by ShareX on macOS.");

    public IPrintService Printing { get; }

    public ISoundService Sounds { get; }

    public IDesktopWallpaperService Wallpaper { get; } = new UnsupportedDesktopWallpaperService("The desktop wallpaper is not available on macOS yet.");

    public void Dispose()
    {
        if (hotkeys.IsValueCreated)
        {
            hotkeys.Value.Dispose();
        }
    }
}
