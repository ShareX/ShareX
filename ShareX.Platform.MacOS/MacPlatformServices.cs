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
        Clipboard = new MacClipboardService();
        MacScreenCaptureService screenCapture = new MacScreenCaptureService(runner);
        ScreenCapture = screenCapture;
        ScreenRecording = new MacScreenRecordingService(runner, screenCapture.GetScreens);
        hotkeys = new Lazy<IHotkeyService>(() => new CarbonHotkeyService());
        Windows = new MacWindowService();
        Notifications = new MacNotificationService(runner);
        Shell = new MacShellService(runner);
        ShellIntegration = new UnsupportedShellIntegrationService(
            "Finder integration needs an NSServices entry in the app bundle's Info.plist, which the macOS packaging adds.");
        Credentials = new KeychainCredentialService();
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

    public INotificationService Notifications { get; }

    public IShellService Shell { get; }

    public IShellIntegrationService ShellIntegration { get; }

    public ICredentialService Credentials { get; }

    public void Dispose()
    {
        if (hotkeys.IsValueCreated)
        {
            hotkeys.Value.Dispose();
        }
    }
}
