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
}
