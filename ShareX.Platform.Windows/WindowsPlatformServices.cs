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
        Notifications = new UnsupportedNotificationService("ShareX shows its own notification window on Windows.");
        Shell = new WindowsShellService();
        ShellIntegration = new WindowsShellIntegrationService();
        Credentials = new WindowsCredentialService();
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
