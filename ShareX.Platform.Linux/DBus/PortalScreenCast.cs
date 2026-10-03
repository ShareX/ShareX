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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ShareX.Platform.Linux.DBus;

/// <summary>One PipeWire stream the ScreenCast portal shares: its node and where it sits on the desktop.</summary>
/// <param name="Position">Top left corner in compositor coordinates, when the portal reports it (monitors).</param>
/// <param name="Size">Size in compositor coordinates, when reported. The frames may have more pixels on scaled outputs.</param>
internal sealed record ScreenCastStream(uint NodeId, PlatformPoint? Position, PlatformSize? Size);

/// <summary>
/// org.freedesktop.portal.ScreenCast: how GNOME, KDE Plasma and other Wayland desktops let an application record the screen.
/// The desktop asks the user which monitor to share the first time; a restore token kept in ShareX's state folder skips the
/// question afterwards where the desktop allows it.
/// </summary>
internal sealed class PortalScreenCast : IAsyncDisposable
{
    public const string Interface = "org.freedesktop.portal.ScreenCast";

    private const uint SourceTypeMonitor = 1;
    private const uint CursorHidden = 1;
    private const uint CursorEmbedded = 2;
    private const uint PersistUntilRevoked = 2;

    private readonly string sessionHandle;

    private PortalScreenCast(string sessionHandle, ScreenCastStream stream)
    {
        this.sessionHandle = sessionHandle;
        Stream = stream;
    }

    public ScreenCastStream Stream { get; }

    public static Task<uint?> GetVersionAsync() => DBusSession.GetPortalVersionAsync(Interface);

    /// <summary>Asks the desktop to share one monitor and returns its stream. Throws OperationCanceledException when the user declines.</summary>
    public static async Task<PortalScreenCast> StartAsync(bool includeCursor, CancellationToken cancellationToken)
    {
        string sessionToken = DBusSession.CreateToken();
        PortalResponse created = await DBusSession.CallPortalRequestAsync((bus, token) =>
            DBusSession.CreateMethodCall(bus, DBusSession.PortalBusName, DBusSession.PortalObjectPath, Interface, "CreateSession", "a{sv}",
                (ref MessageWriter writer) => writer.WriteDictionary(new Dictionary<string, VariantValue>
                {
                    ["handle_token"] = token,
                    ["session_handle_token"] = sessionToken
                })), cancellationToken).ConfigureAwait(false);

        if (!created.Success || !created.Results.TryGetValue("session_handle", out VariantValue handleValue))
        {
            throw new InvalidOperationException($"ScreenCast.CreateSession failed with response {created.Code}.");
        }

        string session = handleValue.Type == VariantValueType.ObjectPath ? handleValue.GetObjectPathAsString() : handleValue.GetString();

        try
        {
            Dictionary<string, VariantValue> options = new Dictionary<string, VariantValue>
            {
                ["types"] = SourceTypeMonitor,
                ["multiple"] = false,
                ["cursor_mode"] = includeCursor ? CursorEmbedded : CursorHidden,
                ["persist_mode"] = PersistUntilRevoked
            };

            if (ReadRestoreToken() is string restoreToken)
            {
                options["restore_token"] = restoreToken;
            }

            PortalResponse selected = await DBusSession.CallPortalRequestAsync((bus, token) =>
                DBusSession.CreateMethodCall(bus, DBusSession.PortalBusName, DBusSession.PortalObjectPath, Interface, "SelectSources", "oa{sv}",
                    (ref MessageWriter writer) =>
                    {
                        writer.WriteObjectPath(session);
                        options["handle_token"] = token;
                        writer.WriteDictionary(options);
                    }), cancellationToken).ConfigureAwait(false);

            ThrowIfFailed(selected, "SelectSources");

            PortalResponse started = await DBusSession.CallPortalRequestAsync((bus, token) =>
                DBusSession.CreateMethodCall(bus, DBusSession.PortalBusName, DBusSession.PortalObjectPath, Interface, "Start", "osa{sv}",
                    (ref MessageWriter writer) =>
                    {
                        writer.WriteObjectPath(session);
                        writer.WriteString("");
                        writer.WriteDictionary(new Dictionary<string, VariantValue> { ["handle_token"] = token });
                    }), cancellationToken).ConfigureAwait(false);

            ThrowIfFailed(started, "Start");

            if (started.Results.TryGetValue("restore_token", out VariantValue newToken))
            {
                WriteRestoreToken(newToken.GetString());
            }

            if (!started.Results.TryGetValue("streams", out VariantValue streams) || ParseStreams(streams) is not { Count: > 0 } list)
            {
                throw new InvalidOperationException("The ScreenCast portal shared no stream.");
            }

            return new PortalScreenCast(session, list[0]);
        }
        catch
        {
            await CloseAsync(session).ConfigureAwait(false);
            throw;
        }
    }

    /// <summary>Closing the session ends the stream and the desktop's "screen is being shared" indicator.</summary>
    public ValueTask DisposeAsync() => new ValueTask(CloseAsync(sessionHandle));

    private static async Task CloseAsync(string session)
    {
        try
        {
            DBusConnection bus = await DBusSession.GetConnectionAsync().ConfigureAwait(false);
            await bus.CallMethodAsync(DBusSession.CreateMethodCall(bus, DBusSession.PortalBusName, session, "org.freedesktop.portal.Session", "Close", null, null))
                .ConfigureAwait(false);
        }
        catch (Exception e) when (DBusSession.IsExpectedFailure(e))
        {
        }
    }

    private static void ThrowIfFailed(PortalResponse response, string method)
    {
        if (response.Cancelled)
        {
            throw new OperationCanceledException("Screen sharing was cancelled.");
        }

        if (!response.Success)
        {
            throw new InvalidOperationException($"ScreenCast.{method} failed with response {response.Code}.");
        }
    }

    /// <summary>streams: a(ua{sv}) with "position" (ii) and "size" (ii) for monitors.</summary>
    internal static IReadOnlyList<ScreenCastStream> ParseStreams(VariantValue streams)
    {
        if (streams.Type == VariantValueType.Variant)
        {
            streams = streams.GetVariantValue();
        }

        List<ScreenCastStream> result = new List<ScreenCastStream>();

        for (int i = 0; i < streams.Count; i++)
        {
            VariantValue stream = streams.GetItem(i);
            uint node = stream.GetItem(0).GetUInt32();
            VariantValue properties = stream.GetItem(1);
            PlatformPoint? position = null;
            PlatformSize? size = null;

            for (int j = 0; j < properties.Count; j++)
            {
                KeyValuePair<VariantValue, VariantValue> entry = properties.GetDictionaryEntry(j);
                VariantValue value = entry.Value.Type == VariantValueType.Variant ? entry.Value.GetVariantValue() : entry.Value;

                switch (entry.Key.GetString())
                {
                    case "position":
                        position = new PlatformPoint(value.GetItem(0).GetInt32(), value.GetItem(1).GetInt32());
                        break;
                    case "size":
                        size = new PlatformSize(value.GetItem(0).GetInt32(), value.GetItem(1).GetInt32());
                        break;
                }
            }

            result.Add(new ScreenCastStream(node, position, size));
        }

        return result;
    }

    private static string TokenPath
    {
        get
        {
            string state = Environment.GetEnvironmentVariable("XDG_STATE_HOME") is { Length: > 0 } dir
                ? dir
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "state");
            return Path.Combine(state, "ShareX", "screencast-restore-token");
        }
    }

    private static string? ReadRestoreToken()
    {
        try
        {
            string token = File.ReadAllText(TokenPath).Trim();
            return token.Length > 0 ? token : null;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static void WriteRestoreToken(string token)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(TokenPath)!);
            File.WriteAllText(TokenPath, token);
            File.SetUnixFileMode(TokenPath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
        }
    }
}
