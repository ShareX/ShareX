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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ShareX.Platform.Linux.DBus;

/// <summary>
/// A minimal client for the freedesktop Secret Service API (GNOME Keyring, KWallet 5.97+, KeePassXC) on the session bus.
/// Secrets travel in a "plain" session: the bus is private to the user, and libsecret's secret-tool does the same unless it
/// negotiates encryption. Items carry "service" and "account" attributes, so secrets stored earlier by secret-tool are found.
/// </summary>
internal static class SecretService
{
    private const string BusName = "org.freedesktop.secrets";
    private const string ServicePath = "/org/freedesktop/secrets";
    private const string ServiceInterface = "org.freedesktop.Secret.Service";
    private const string NoPrompt = "/";

    /// <summary>Whether a Secret Service is running or can be started on the session bus. Waits at most a few seconds.</summary>
    public static bool IsAvailable()
    {
        if (!DBusSession.IsAvailable)
        {
            return false;
        }

        try
        {
            return DBusSession.RunSync(async () =>
            {
                DBusConnection bus = await DBusSession.GetConnectionAsync().ConfigureAwait(false);
                return await HasNameAsync(bus, "NameHasOwner").ConfigureAwait(false) || await HasNameAsync(bus, "ListActivatableNames").ConfigureAwait(false);
            }, TimeSpan.FromSeconds(5));
        }
        catch (Exception e) when (DBusSession.IsExpectedFailure(e is AggregateException { InnerException: Exception inner } ? inner : e))
        {
            return false;
        }
    }

    private static Task<bool> HasNameAsync(DBusConnection bus, string member)
    {
        bool owner = member == "NameHasOwner";
        MessageBuffer call = DBusSession.CreateMethodCall(bus, "org.freedesktop.DBus", "/org/freedesktop/DBus", "org.freedesktop.DBus", member,
            owner ? "s" : null, owner ? (ref MessageWriter writer) => writer.WriteString(BusName) : null);

        return bus.CallMethodAsync(call, static (Message message, object? state) =>
        {
            Reader reader = message.GetBodyReader();
            return (bool)state! ? reader.ReadBool() : Array.IndexOf(reader.ReadArrayOfString(), BusName) >= 0;
        }, owner);
    }

    public static async Task<string?> GetAsync(Dictionary<string, string> attributes, CancellationToken cancellationToken)
    {
        DBusConnection bus = await DBusSession.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        string? item = await FindUnlockedItemAsync(bus, attributes, cancellationToken).ConfigureAwait(false);

        if (item == null)
        {
            return null;
        }

        string session = await OpenSessionAsync(bus).ConfigureAwait(false);

        try
        {
            MessageBuffer call = DBusSession.CreateMethodCall(bus, BusName, item, "org.freedesktop.Secret.Item", "GetSecret", "o",
                (ref MessageWriter writer) => writer.WriteObjectPath(session));

            byte[] value = await bus.CallMethodAsync(call, static (Message message, object? state) =>
            {
                // (oayays): session, parameters, value, content type.
                Reader reader = message.GetBodyReader();
                reader.AlignStruct();
                reader.ReadObjectPath();
                reader.ReadArrayOfByte();
                return reader.ReadArrayOfByte();
            }, null).ConfigureAwait(false);

            return Encoding.UTF8.GetString(value);
        }
        finally
        {
            await CloseSessionAsync(bus, session).ConfigureAwait(false);
        }
    }

    public static async Task<bool> StoreAsync(string label, Dictionary<string, string> attributes, string secret, CancellationToken cancellationToken)
    {
        DBusConnection bus = await DBusSession.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        string? collection = await GetDefaultCollectionAsync(bus, cancellationToken).ConfigureAwait(false);

        if (collection == null)
        {
            return false;
        }

        string session = await OpenSessionAsync(bus).ConfigureAwait(false);

        try
        {
            byte[] value = Encoding.UTF8.GetBytes(secret);
            MessageBuffer call = DBusSession.CreateMethodCall(bus, BusName, collection, "org.freedesktop.Secret.Collection", "CreateItem", "a{sv}(oayays)b",
                (ref MessageWriter writer) =>
                {
                    writer.WriteDictionary(new Dictionary<string, VariantValue>
                    {
                        ["org.freedesktop.Secret.Item.Label"] = label,
                        ["org.freedesktop.Secret.Item.Attributes"] = new Dict<string, string>(attributes)
                    });
                    writer.WriteStructureStart();
                    writer.WriteObjectPath(session);
                    writer.WriteArray(Array.Empty<byte>());
                    writer.WriteArray(value);
                    writer.WriteString("text/plain; charset=utf8");
                    writer.WriteBool(true);
                });

            (string item, string prompt) = await bus.CallMethodAsync(call, static (Message message, object? state) =>
            {
                Reader reader = message.GetBodyReader();
                return (reader.ReadObjectPathAsString(), reader.ReadObjectPathAsString());
            }, null).ConfigureAwait(false);

            return item != NoPrompt || await PromptAsync(bus, prompt, cancellationToken).ConfigureAwait(false) is { Dismissed: false };
        }
        finally
        {
            await CloseSessionAsync(bus, session).ConfigureAwait(false);
        }
    }

    public static async Task<bool> DeleteAsync(Dictionary<string, string> attributes, CancellationToken cancellationToken)
    {
        DBusConnection bus = await DBusSession.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        (string[] unlocked, string[] locked) = await SearchAsync(bus, attributes).ConfigureAwait(false);
        bool deleted = false;

        foreach (string item in (string[])[.. unlocked, .. locked])
        {
            MessageBuffer call = DBusSession.CreateMethodCall(bus, BusName, item, "org.freedesktop.Secret.Item", "Delete", null, null);
            string prompt = await bus.CallMethodAsync(call, static (Message message, object? state) => message.GetBodyReader().ReadObjectPathAsString(), null)
                .ConfigureAwait(false);
            deleted |= prompt == NoPrompt || await PromptAsync(bus, prompt, cancellationToken).ConfigureAwait(false) is { Dismissed: false };
        }

        return deleted;
    }

    private static async Task<string?> FindUnlockedItemAsync(DBusConnection bus, Dictionary<string, string> attributes, CancellationToken cancellationToken)
    {
        (string[] unlocked, string[] locked) = await SearchAsync(bus, attributes).ConfigureAwait(false);

        if (unlocked.Length > 0)
        {
            return unlocked[0];
        }

        if (locked.Length == 0)
        {
            return null;
        }

        string[] opened = await UnlockAsync(bus, locked[..1], cancellationToken).ConfigureAwait(false);
        return opened.Length > 0 ? opened[0] : null;
    }

    private static Task<(string[] Unlocked, string[] Locked)> SearchAsync(DBusConnection bus, Dictionary<string, string> attributes)
    {
        MessageBuffer call = DBusSession.CreateMethodCall(bus, BusName, ServicePath, ServiceInterface, "SearchItems", "a{ss}",
            (ref MessageWriter writer) => WriteStringDictionary(ref writer, attributes));

        return bus.CallMethodAsync(call, static (Message message, object? state) =>
        {
            Reader reader = message.GetBodyReader();
            return (ReadPaths(ref reader), ReadPaths(ref reader));
        }, null);
    }

    /// <summary>Unlocks objects, which may ask the user for the keyring password. Returns the objects that are now unlocked.</summary>
    private static async Task<string[]> UnlockAsync(DBusConnection bus, string[] objects, CancellationToken cancellationToken)
    {
        MessageBuffer call = DBusSession.CreateMethodCall(bus, BusName, ServicePath, ServiceInterface, "Unlock", "ao",
            (ref MessageWriter writer) => writer.WriteArray(Array.ConvertAll(objects, path => new ObjectPath(path))));

        (string[] unlocked, string prompt) = await bus.CallMethodAsync(call, static (Message message, object? state) =>
        {
            Reader reader = message.GetBodyReader();
            return (ReadPaths(ref reader), reader.ReadObjectPathAsString());
        }, null).ConfigureAwait(false);

        if (prompt == NoPrompt)
        {
            return unlocked;
        }

        PromptResult? result = await PromptAsync(bus, prompt, cancellationToken).ConfigureAwait(false);
        return result is { Dismissed: false } ? ToPaths(result.Value.Result) : [];
    }

    /// <summary>The default collection ("login" in GNOME Keyring, "kdewallet" in KWallet), unlocked, or null.</summary>
    private static async Task<string?> GetDefaultCollectionAsync(DBusConnection bus, CancellationToken cancellationToken)
    {
        MessageBuffer call = DBusSession.CreateMethodCall(bus, BusName, ServicePath, ServiceInterface, "ReadAlias", "s",
            (ref MessageWriter writer) => writer.WriteString("default"));
        string collection = await bus.CallMethodAsync(call, static (Message message, object? state) => message.GetBodyReader().ReadObjectPathAsString(), null)
            .ConfigureAwait(false);

        if (collection == NoPrompt)
        {
            // A new user without a keyring: the service asks for a password for the new one.
            MessageBuffer create = DBusSession.CreateMethodCall(bus, BusName, ServicePath, ServiceInterface, "CreateCollection", "a{sv}s",
                (ref MessageWriter writer) =>
                {
                    writer.WriteDictionary(new Dictionary<string, VariantValue> { ["org.freedesktop.Secret.Collection.Label"] = "Login" });
                    writer.WriteString("default");
                });
            (string created, string prompt) = await bus.CallMethodAsync(create, static (Message message, object? state) =>
            {
                Reader reader = message.GetBodyReader();
                return (reader.ReadObjectPathAsString(), reader.ReadObjectPathAsString());
            }, null).ConfigureAwait(false);

            if (created != NoPrompt)
            {
                return created;
            }

            PromptResult? result = await PromptAsync(bus, prompt, cancellationToken).ConfigureAwait(false);
            return result is { Dismissed: false } && result.Value.Result.Type == VariantValueType.ObjectPath ? result.Value.Result.GetObjectPathAsString() : null;
        }

        string[] unlocked = await UnlockAsync(bus, [collection], cancellationToken).ConfigureAwait(false);
        return unlocked.Length > 0 ? collection : null;
    }

    private static Task<string> OpenSessionAsync(DBusConnection bus)
    {
        MessageBuffer call = DBusSession.CreateMethodCall(bus, BusName, ServicePath, ServiceInterface, "OpenSession", "sv",
            (ref MessageWriter writer) =>
            {
                writer.WriteString("plain");
                writer.WriteVariantString("");
            });

        return bus.CallMethodAsync(call, static (Message message, object? state) =>
        {
            Reader reader = message.GetBodyReader();
            reader.ReadVariantValue();
            return reader.ReadObjectPathAsString();
        }, null);
    }

    private static async Task CloseSessionAsync(DBusConnection bus, string session)
    {
        try
        {
            await bus.CallMethodAsync(DBusSession.CreateMethodCall(bus, BusName, session, "org.freedesktop.Secret.Session", "Close", null, null)).ConfigureAwait(false);
        }
        catch (Exception e) when (DBusSession.IsExpectedFailure(e))
        {
            // The service closes sessions of departed clients itself.
        }
    }

    private readonly record struct PromptResult(bool Dismissed, VariantValue Result);

    /// <summary>Shows a Secret Service prompt (a password dialog from the keyring) and waits for the user.</summary>
    private static async Task<PromptResult?> PromptAsync(DBusConnection bus, string prompt, CancellationToken cancellationToken)
    {
        TaskCompletionSource<PromptResult> completion = new TaskCompletionSource<PromptResult>(TaskCreationOptions.RunContinuationsAsynchronously);

        using IDisposable subscription = await bus.WatchSignalAsync(BusName, prompt, "org.freedesktop.Secret.Prompt", "Completed",
            static (Message message, object? state) =>
            {
                Reader reader = message.GetBodyReader();
                return new PromptResult(reader.ReadBool(), reader.ReadVariantValue());
            },
            (Notification<PromptResult> notification) =>
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

        await bus.CallMethodAsync(DBusSession.CreateMethodCall(bus, BusName, prompt, "org.freedesktop.Secret.Prompt", "Prompt", "s",
            (ref MessageWriter writer) => writer.WriteString(""))).ConfigureAwait(false);

        // The user may take a while to type a keyring password.
        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromMinutes(2));

        try
        {
            return await completion.Task.WaitAsync(timeout.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return null;
        }
    }

    private static void WriteStringDictionary(ref MessageWriter writer, Dictionary<string, string> values)
    {
        ArrayStart start = writer.WriteDictionaryStart();

        foreach (KeyValuePair<string, string> pair in values)
        {
            writer.WriteDictionaryEntryStart();
            writer.WriteString(pair.Key);
            writer.WriteString(pair.Value);
        }

        writer.WriteDictionaryEnd(start);
    }

    private static string[] ReadPaths(ref Reader reader) => Array.ConvertAll(reader.ReadArrayOfObjectPath(), path => path.ToString());

    private static string[] ToPaths(VariantValue value) =>
        value.Type == VariantValueType.Array ? Array.ConvertAll(value.GetArray<ObjectPath>(), path => path.ToString()) : [];
}
