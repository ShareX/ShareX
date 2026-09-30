using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform;

public sealed record PlatformNotification(string Title, string Message)
{
    /// <summary>Optional path to an image shown with the notification, for example the screenshot thumbnail.</summary>
    public string? ImagePath { get; init; }

    /// <summary>How long the notification should stay visible. Null uses the OS default.</summary>
    public int? TimeoutMilliseconds { get; init; }
}

/// <summary>OS notifications: libnotify/D-Bus on Linux, Notification Center on macOS.</summary>
public interface INotificationService
{
    FeatureSupport Support { get; }

    Task<bool> ShowAsync(PlatformNotification notification, CancellationToken cancellationToken = default);
}
