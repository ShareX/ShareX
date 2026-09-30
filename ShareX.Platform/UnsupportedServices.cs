using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform;

/// <summary>Placeholder implementations used when a platform or session cannot provide a feature.</summary>
public sealed class UnsupportedNotificationService(string reason) : INotificationService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public Task<bool> ShowAsync(PlatformNotification notification, CancellationToken cancellationToken = default) => Task.FromResult(false);
}

public sealed class UnsupportedHotkeyService(string reason) : IHotkeyService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed { add { } remove { } }

    public HotkeyRegistrationStatus Register(int id, PlatformHotkey hotkey) => HotkeyRegistrationStatus.NotSupported;

    public bool Unregister(int id) => false;

    public void UnregisterAll()
    {
    }

    public void Dispose()
    {
    }
}

public sealed class UnsupportedWindowService(string reason) : IWindowService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public IReadOnlyList<PlatformWindow> GetWindows() => Array.Empty<PlatformWindow>();

    public PlatformWindow? GetActiveWindow() => null;
}

public sealed class UnsupportedShellIntegrationService(string reason) : IShellIntegrationService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public bool IsRegistered(ShellMenuEntry entry) => false;

    public void Register(ShellMenuEntry entry) => throw new PlatformNotSupportedException(Support.Reason);

    public void Unregister(ShellMenuEntry entry)
    {
    }
}

public sealed class UnsupportedScreenRecordingService(string reason) : IScreenRecordingService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public IReadOnlyList<string> GetSupportedDevices() => Array.Empty<string>();

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request) => throw new PlatformNotSupportedException(Support.Reason);
}
