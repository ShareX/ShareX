using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ShareX.Platform.Linux.DBus;

/// <summary>
/// Global hotkeys on Wayland through org.freedesktop.portal.GlobalShortcuts (KDE Plasma 5.27+, GNOME 48+, Hyprland).
/// </summary>
/// <remarks>
/// Wayland does not let applications grab keys. The portal asks the desktop to bind them, and the desktop may show a
/// confirmation dialog or let the user choose different keys, so registration completes asynchronously. The trigger
/// ShareX asks for is only a preference.
/// </remarks>
public sealed class PortalGlobalShortcutsService : IHotkeyService
{
    private const string Interface = "org.freedesktop.portal.GlobalShortcuts";
    private const string IdPrefix = "sharex-";

    private readonly object syncRoot = new object();
    private readonly Dictionary<int, PlatformHotkey> hotkeys = new Dictionary<int, PlatformHotkey>();
    private readonly Func<int, string> describe;
    private readonly SemaphoreSlim bindLock = new SemaphoreSlim(1, 1);
    private string? sessionHandle;
    private IDisposable? activatedSubscription;
    private CancellationTokenSource? pendingBind;
    private bool disposed;

    /// <param name="describe">Returns the description shown in the desktop's shortcut settings for a hotkey id, for example "Capture region".</param>
    public PortalGlobalShortcutsService(Func<int, string>? describe = null)
    {
        this.describe = describe ?? (id => $"ShareX hotkey {id}");
        uint? version = null;

        try
        {
            version = DBusSession.RunSync(() => DBusSession.GetPortalVersionAsync(Interface), TimeSpan.FromSeconds(3));
        }
        catch (TimeoutException)
        {
        }

        Support = version != null
            ? FeatureSupport.Supported
            : FeatureSupport.NotSupported("The desktop's xdg-desktop-portal does not provide GlobalShortcuts. Update the portal backend or bind ShareX commands in the desktop's keyboard settings.");
    }

    public FeatureSupport Support { get; }

    public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    /// <summary>Raised when binding fails, for example when the user declines the desktop's dialog.</summary>
    public event EventHandler<Exception>? BindFailed;

    public HotkeyRegistrationStatus Register(int id, PlatformHotkey hotkey)
    {
        if (!Support.IsSupported)
        {
            return HotkeyRegistrationStatus.NotSupported;
        }

        if (XKeyMap.ToShortcutTrigger(hotkey) == null)
        {
            return HotkeyRegistrationStatus.UnsupportedKey;
        }

        lock (syncRoot)
        {
            hotkeys[id] = hotkey;
        }

        ScheduleBind();
        return HotkeyRegistrationStatus.Registered;
    }

    public bool Unregister(int id)
    {
        bool removed;

        lock (syncRoot)
        {
            removed = hotkeys.Remove(id);
        }

        if (removed)
        {
            ScheduleBind();
        }

        return removed;
    }

    public void UnregisterAll()
    {
        lock (syncRoot)
        {
            hotkeys.Clear();
        }

        ScheduleBind();
    }

    /// <summary>ShareX registers many hotkeys at start up, so bind them in one request after the calls settle.</summary>
    private void ScheduleBind()
    {
        CancellationTokenSource cancellation = new CancellationTokenSource();
        Interlocked.Exchange(ref pendingBind, cancellation)?.Cancel();

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(300, cancellation.Token).ConfigureAwait(false);
                await BindAsync(cancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e) when (DBusSession.IsExpectedFailure(e))
            {
                BindFailed?.Invoke(this, e);
            }
        });
    }

    private async Task BindAsync(CancellationToken cancellationToken)
    {
        await bindLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            List<(string Id, string Description, string Trigger)> shortcuts;

            lock (syncRoot)
            {
                shortcuts = hotkeys.Select(pair => (IdPrefix + pair.Key.ToString(CultureInfo.InvariantCulture), describe(pair.Key),
                    XKeyMap.ToShortcutTrigger(pair.Value)!)).ToList();
            }

            DBusConnection bus = await DBusSession.GetConnectionAsync(cancellationToken).ConfigureAwait(false);

            // A session binds its shortcuts once, so changes need a fresh session.
            await CloseSessionAsync(bus).ConfigureAwait(false);

            if (shortcuts.Count == 0 || disposed)
            {
                return;
            }

            await EnsureActivatedSubscriptionAsync(bus).ConfigureAwait(false);

            string sessionToken = DBusSession.CreateToken();
            PortalResponse created = await DBusSession.CallPortalRequestAsync((connection, token) =>
                DBusSession.CreateMethodCall(connection, DBusSession.PortalBusName, DBusSession.PortalObjectPath, Interface, "CreateSession", "a{sv}",
                    (ref MessageWriter writer) =>
                    {
                        writer.WriteDictionary(new Dictionary<string, VariantValue>
                        {
                            ["handle_token"] = token,
                            ["session_handle_token"] = sessionToken
                        });
                    }), cancellationToken).ConfigureAwait(false);

            if (!created.Success || !created.Results.TryGetValue("session_handle", out VariantValue handleValue))
            {
                throw new InvalidOperationException($"GlobalShortcuts.CreateSession failed with response {created.Code}.");
            }

            string session = handleValue.Type == VariantValueType.ObjectPath ? handleValue.GetObjectPathAsString() : handleValue.GetString();
            sessionHandle = session;

            PortalResponse bound = await DBusSession.CallPortalRequestAsync((connection, token) =>
                DBusSession.CreateMethodCall(connection, DBusSession.PortalBusName, DBusSession.PortalObjectPath, Interface, "BindShortcuts", "oa(sa{sv})sa{sv}",
                    (ref MessageWriter writer) =>
                    {
                        writer.WriteObjectPath(session);
                        ArrayStart array = writer.WriteArrayStart(DBusType.Struct);

                        foreach ((string id, string description, string trigger) in shortcuts)
                        {
                            writer.WriteStructureStart();
                            writer.WriteString(id);
                            writer.WriteDictionary(new Dictionary<string, VariantValue>
                            {
                                ["description"] = description,
                                ["preferred_trigger"] = trigger
                            });
                        }

                        writer.WriteArrayEnd(array);
                        writer.WriteString("");
                        writer.WriteDictionary(new Dictionary<string, VariantValue> { ["handle_token"] = token });
                    }), cancellationToken).ConfigureAwait(false);

            if (!bound.Success)
            {
                throw new InvalidOperationException(bound.Cancelled
                    ? "The desktop's global shortcut dialog was dismissed."
                    : $"GlobalShortcuts.BindShortcuts failed with response {bound.Code}.");
            }
        }
        finally
        {
            bindLock.Release();
        }
    }

    private async Task EnsureActivatedSubscriptionAsync(DBusConnection bus)
    {
        if (activatedSubscription != null)
        {
            return;
        }

        activatedSubscription = await bus.WatchSignalAsync(null, DBusSession.PortalObjectPath, Interface, "Activated",
            static (Message message, object? state) =>
            {
                Reader reader = message.GetBodyReader();
                string session = reader.ReadObjectPathAsString();
                string shortcutId = reader.ReadString();
                return (session, shortcutId);
            },
            (Notification<(string Session, string ShortcutId)> notification) =>
            {
                if (notification.HasValue)
                {
                    OnActivated(notification.Value.Session, notification.Value.ShortcutId);
                }
            }, ObserverFlags.None, false, null).ConfigureAwait(false);
    }

    private void OnActivated(string session, string shortcutId)
    {
        if (session != sessionHandle || !shortcutId.StartsWith(IdPrefix, StringComparison.Ordinal) ||
            !int.TryParse(shortcutId.AsSpan(IdPrefix.Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
        {
            return;
        }

        PlatformHotkey hotkey;

        lock (syncRoot)
        {
            if (!hotkeys.TryGetValue(id, out hotkey))
            {
                return;
            }
        }

        HotkeyPressed?.Invoke(this, new HotkeyPressedEventArgs(id, hotkey));
    }

    private async Task CloseSessionAsync(DBusConnection bus)
    {
        string? session = Interlocked.Exchange(ref sessionHandle, null);

        if (session == null)
        {
            return;
        }

        try
        {
            await bus.CallMethodAsync(DBusSession.CreateMethodCall(bus, DBusSession.PortalBusName, session, "org.freedesktop.portal.Session", "Close", null, null))
                .ConfigureAwait(false);
        }
        catch (Exception e) when (DBusSession.IsExpectedFailure(e))
        {
            // The session may already be gone.
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        pendingBind?.Cancel();
        activatedSubscription?.Dispose();

        try
        {
            DBusSession.RunSync(async () =>
            {
                await CloseSessionAsync(await DBusSession.GetConnectionAsync().ConfigureAwait(false)).ConfigureAwait(false);
                return true;
            }, TimeSpan.FromSeconds(2));
        }
        catch (Exception e) when (DBusSession.IsExpectedFailure(e))
        {
        }
    }
}
