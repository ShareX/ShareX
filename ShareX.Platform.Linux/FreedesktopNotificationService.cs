using ShareX.Platform.Diagnostics;
using ShareX.Platform.Linux.DBus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ShareX.Platform.Linux;

/// <summary>Notifications through org.freedesktop.Notifications, falling back to notify-send.</summary>
/// <remarks>Implemented by GNOME Shell, KDE Plasma, mako, dunst, swaync and xfce4-notifyd.</remarks>
public sealed class FreedesktopNotificationService : INotificationService
{
    private readonly ICommandRunner runner;
    private readonly string applicationName;
    private readonly LinuxDistribution distribution;

    public FreedesktopNotificationService(PlatformInfo info, ICommandRunner runner, string applicationName = "ShareX")
    {
        this.runner = runner;
        this.applicationName = applicationName;
        distribution = info.Distribution ?? LinuxDistribution.Unknown;
    }

    public FeatureSupport Support => DBusSession.IsAvailable || runner.Exists("notify-send")
        ? FeatureSupport.Supported
        : LinuxPackages.Missing(distribution, LinuxTool.Libnotify);

    public async Task<bool> ShowAsync(PlatformNotification notification, CancellationToken cancellationToken = default)
    {
        if (DBusSession.IsAvailable)
        {
            try
            {
                await NotifyAsync(notification, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (Exception e) when (DBusSession.IsExpectedFailure(e))
            {
                // No notification daemon is running on the bus. Try notify-send, which may use a portal.
            }
        }

        if (!runner.Exists("notify-send"))
        {
            return false;
        }

        List<string> arguments = new List<string> { "--app-name", applicationName };

        if (!string.IsNullOrEmpty(notification.ImagePath))
        {
            arguments.Add("--icon");
            arguments.Add(notification.ImagePath);
        }

        if (notification.TimeoutMilliseconds != null)
        {
            arguments.Add("--expire-time");
            arguments.Add(notification.TimeoutMilliseconds.Value.ToString(CultureInfo.InvariantCulture));
        }

        arguments.Add("--");
        arguments.Add(notification.Title);
        arguments.Add(notification.Message);

        try
        {
            return (await runner.RunAsync("notify-send", arguments, cancellationToken: cancellationToken).ConfigureAwait(false)).Success;
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception)
        {
            return false;
        }
    }

    private async Task NotifyAsync(PlatformNotification notification, CancellationToken cancellationToken)
    {
        DBusConnection bus = await DBusSession.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        Dictionary<string, VariantValue> hints = new Dictionary<string, VariantValue>();

        if (!string.IsNullOrEmpty(notification.ImagePath))
        {
            hints["image-path"] = new Uri(notification.ImagePath).AbsoluteUri;
        }

        MessageBuffer call = DBusSession.CreateMethodCall(bus, "org.freedesktop.Notifications", "/org/freedesktop/Notifications",
            "org.freedesktop.Notifications", "Notify", "susssasa{sv}i",
            (ref MessageWriter writer) =>
            {
                writer.WriteString(applicationName);
                writer.WriteUInt32(0);
                writer.WriteString(applicationName.ToLowerInvariant());
                writer.WriteString(notification.Title);
                writer.WriteString(EscapeBody(notification.Message));
                writer.WriteArray(Array.Empty<string>());
                writer.WriteDictionary(hints);
                writer.WriteInt32(notification.TimeoutMilliseconds ?? -1);
            });

        await bus.CallMethodAsync(call).ConfigureAwait(false);
    }

    /// <summary>The body may contain a subset of HTML markup, so escape it.</summary>
    internal static string EscapeBody(string text) => text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}
