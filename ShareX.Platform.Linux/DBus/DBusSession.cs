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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ShareX.Platform.Linux.DBus;

/// <summary>Shared connection to the session bus plus helpers for the xdg-desktop-portal request pattern.</summary>
internal static class DBusSession
{
    public const string PortalBusName = "org.freedesktop.portal.Desktop";
    public const string PortalObjectPath = "/org/freedesktop/portal/desktop";

    /// <summary>ShareX's application id, the name of its desktop entry (sharex.desktop), which the portal looks up.</summary>
    public const string ApplicationId = "sharex";

    private static readonly SemaphoreSlim connectLock = new SemaphoreSlim(1, 1);
    private static DBusConnection? connection;

    /// <summary>Null when registration succeeded or was not needed; otherwise why the portal did not accept ShareX's application id.</summary>
    public static string? RegistrationError { get; private set; }

    public static bool IsAvailable => !string.IsNullOrEmpty(DBusAddress.Session);

    public static async Task<DBusConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        DBusConnection? existing = Volatile.Read(ref connection);

        if (existing != null)
        {
            return existing;
        }

        await connectLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (connection == null)
            {
                string? address = DBusAddress.Session;

                if (string.IsNullOrEmpty(address))
                {
                    throw new PlatformNotSupportedException("No D-Bus session bus is available.");
                }

                DBusConnection created = new DBusConnection(address);
                await created.ConnectAsync().ConfigureAwait(false);
                await RegisterApplicationAsync(created).ConfigureAwait(false);
                Volatile.Write(ref connection, created);
            }

            return connection;
        }
        finally
        {
            connectLock.Release();
        }
    }

    /// <summary>
    /// Applications that are not sandboxed tell xdg-desktop-portal 1.19+ who they are before their first portal call; GlobalShortcuts
    /// refuses to work without it ("An app id is required"). Older portals do not have the registry, which is fine.
    /// </summary>
    private static async Task RegisterApplicationAsync(DBusConnection bus)
    {
        try
        {
            MessageBuffer call = CreateMethodCall(bus, PortalBusName, PortalObjectPath, "org.freedesktop.host.portal.Registry", "Register", "sa{sv}",
                (ref MessageWriter writer) =>
                {
                    writer.WriteString(ApplicationId);
                    writer.WriteDictionary(new Dictionary<string, VariantValue>());
                });

            await bus.CallMethodAsync(call).ConfigureAwait(false);
            RegistrationError = null;
        }
        catch (DBusErrorReplyException e) when (e.ErrorName is "org.freedesktop.DBus.Error.UnknownMethod" or "org.freedesktop.DBus.Error.UnknownInterface" or "org.freedesktop.DBus.Error.ServiceUnknown")
        {
            RegistrationError = null;
        }
        catch (DBusErrorReplyException e)
        {
            // Usually "App info not found": the desktop entry is not installed.
            RegistrationError = e.ErrorMessage;
        }
    }

    public delegate void BodyWriter(ref MessageWriter writer);

    public static MessageBuffer CreateMethodCall(DBusConnection bus, string destination, string path, string @interface, string member,
        string? signature, BodyWriter? body)
    {
        MessageWriter writer = bus.GetMessageWriter();

        try
        {
            writer.WriteMethodCallHeader(destination: destination, path: path, @interface: @interface, member: member, signature: signature);
            body?.Invoke(ref writer);
            return writer.CreateMessage();
        }
        finally
        {
            writer.Dispose();
        }
    }

    /// <summary>Runs a blocking wait for code paths that must stay synchronous, without capturing the caller's context.</summary>
    public static T RunSync<T>(Func<Task<T>> action, TimeSpan timeout)
    {
        Task<T> task = Task.Run(action);
        return task.Wait(timeout) ? task.Result : throw new TimeoutException("D-Bus call timed out.");
    }

    public static async Task<uint?> GetPortalVersionAsync(string portalInterface, CancellationToken cancellationToken = default)
    {
        try
        {
            DBusConnection bus = await GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            MessageBuffer call = CreateMethodCall(bus, PortalBusName, PortalObjectPath, "org.freedesktop.DBus.Properties", "Get", "ss",
                (ref MessageWriter writer) =>
                {
                    writer.WriteString(portalInterface);
                    writer.WriteString("version");
                });

            return await bus.CallMethodAsync(call, static (Message message, object? state) =>
            {
                Reader reader = message.GetBodyReader();
                return (uint?)reader.ReadVariantValue().GetUInt32();
            }, null).ConfigureAwait(false);
        }
        catch (Exception e) when (IsExpectedFailure(e))
        {
            return null;
        }
    }

    /// <summary>
    /// Calls a portal method that returns an org.freedesktop.portal.Request handle and waits for its Response signal.
    /// </summary>
    /// <param name="writeCall">Writes the method call. Receives the handle token which must be put in the options as "handle_token".</param>
    /// <returns>The response code (0 success, 1 cancelled by the user, 2 other failure) and the results dictionary.</returns>
    public static async Task<PortalResponse> CallPortalRequestAsync(Func<DBusConnection, string, MessageBuffer> writeCall, CancellationToken cancellationToken = default)
    {
        DBusConnection bus = await GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        string token = CreateToken();
        string sender = (bus.UniqueName ?? throw new InvalidOperationException("D-Bus connection has no unique name.")).TrimStart(':').Replace('.', '_');
        string requestPath = $"{PortalObjectPath}/request/{sender}/{token}";

        TaskCompletionSource<PortalResponse> completion = new TaskCompletionSource<PortalResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Subscribe before calling so a fast portal cannot answer before we listen.
        using IDisposable subscription = await bus.WatchSignalAsync(null, requestPath, "org.freedesktop.portal.Request", "Response",
            static (Message message, object? state) =>
            {
                Reader reader = message.GetBodyReader();
                uint code = reader.ReadUInt32();
                Dictionary<string, VariantValue> results = reader.ReadDictionaryOfStringToVariantValue();
                return new PortalResponse(code, results);
            },
            (Notification<PortalResponse> notification) =>
            {
                if (notification.HasValue)
                {
                    completion.TrySetResult(notification.Value);
                }
                else if (notification.Exception != null)
                {
                    completion.TrySetException(notification.Exception);
                }
            }, ObserverFlags.None, false, null).ConfigureAwait(false);

        await bus.CallMethodAsync(writeCall(bus, token)).ConfigureAwait(false);

        using (cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken)))
        {
            return await completion.Task.ConfigureAwait(false);
        }
    }

    /// <summary>Failures that mean "the service is not there" rather than a bug: no bus, no portal, or an error reply.</summary>
    public static bool IsExpectedFailure(Exception e) =>
        e.GetType().Namespace == typeof(DBusConnection).Namespace || e is PlatformNotSupportedException or TimeoutException or InvalidOperationException or System.IO.IOException or System.Net.Sockets.SocketException;

    public static string CreateToken() => "sharex" + Guid.NewGuid().ToString("N");
}

internal readonly record struct PortalResponse(uint Code, Dictionary<string, VariantValue> Results)
{
    public bool Success => Code == 0;

    public bool Cancelled => Code == 1;
}
