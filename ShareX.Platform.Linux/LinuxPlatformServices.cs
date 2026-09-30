using ShareX.Platform.Diagnostics;
using ShareX.Platform.Linux.DBus;
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
        screenCapture = new Lazy<IScreenCaptureService>(() => new LinuxScreenCaptureService(info, runner));
        ScreenRecording = new LinuxScreenRecordingService(info, runner);
        hotkeys = new Lazy<IHotkeyService>(() => CreateHotkeyService(info, describeHotkey));
        Windows = new LinuxWindowService(info, runner);
        Notifications = new FreedesktopNotificationService(info, runner);
        Shell = new LinuxShellService(runner);
        ShellIntegration = new LinuxShellIntegrationService(paths);
        Credentials = new SecretServiceCredentialService(info, runner);
    }

    public PlatformInfo Info { get; }

    public IPathService Paths { get; }

    public IStartupService Startup { get; }

    public IClipboardService Clipboard { get; }

    public IScreenCaptureService ScreenCapture => screenCapture.Value;

    public IScreenRecordingService ScreenRecording { get; }

    /// <summary>Created on first use because both backends start a connection to the display server or session bus.</summary>
    public IHotkeyService Hotkeys => hotkeys.Value;

    public IWindowService Windows { get; }

    public INotificationService Notifications { get; }

    public IShellService Shell { get; }

    public IShellIntegrationService ShellIntegration { get; }

    public ICredentialService Credentials { get; }

    private static IHotkeyService CreateHotkeyService(PlatformInfo info, Func<int, string>? describeHotkey)
    {
        if (info.IsX11 && !info.IsSandboxed)
        {
            return new X11HotkeyService();
        }

        if (DBusSession.IsAvailable)
        {
            return new PortalGlobalShortcutsService(describeHotkey);
        }

        return new UnsupportedHotkeyService("Global hotkeys need an X11 session or the xdg-desktop-portal GlobalShortcuts interface.");
    }

    public void Dispose()
    {
        if (hotkeys.IsValueCreated)
        {
            hotkeys.Value.Dispose();
        }
    }
}
