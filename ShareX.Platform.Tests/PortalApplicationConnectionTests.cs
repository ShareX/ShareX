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

using ShareX.Platform.Linux.DBus;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Xunit;

namespace ShareX.Platform.Tests;

public sealed class LinuxPortalFactAttribute : FactAttribute
{
    public LinuxPortalFactAttribute()
    {
        if (!OperatingSystem.IsLinux() || !File.Exists("/usr/bin/dbus-daemon"))
        {
            Skip = "Requires Linux with its existing D-Bus daemon for an isolated portal fixture.";
        }
    }
}

public class PortalApplicationConnectionTests
{
    [LinuxPortalFact]
    public async Task HostRegistersShareXBeforeItsFirstPortalCall()
    {
        await WithPortal(null, hasSandboxIdentity: false, async (portal, connection, error) =>
        {
            Assert.Null(error);
            Assert.Equal("sharex", portal.RegisteredId);
            Assert.Equal(connection.UniqueName, portal.RegisteredPeer);
            Assert.Equal(1, portal.RegistrationCalls);
            Assert.Equal(1u, await ReadVersion(connection));
        });
    }

    [LinuxPortalFact]
    public async Task KnownSandboxCallsThePortalWithoutHostRegistration()
    {
        await WithPortal("Can't manually register a io.snapcraft application", hasSandboxIdentity: true,
            async (portal, connection, error) =>
        {
            Assert.Null(error);
            Assert.Equal(0, portal.RegistrationCalls);
            Assert.Equal(1u, await ReadVersion(connection));
        });
    }

    [LinuxPortalFact]
    public async Task UndetectedSnapReplacesTheRejectedPeerBeforeCallingThePortal()
    {
        await WithPortal("Could not register app ID: Can't manually register a io.snapcraft application",
            hasSandboxIdentity: false, async (portal, connection, error) =>
        {
            Assert.Null(error);
            Assert.Equal(1, portal.RegistrationCalls);
            Assert.NotEqual(portal.RegisteredPeer, connection.UniqueName);
            Assert.Equal(1u, await ReadVersion(connection));
            Assert.False(await NameHasOwner(portal.Connection, portal.RegisteredPeer!));
        });
    }

    [LinuxPortalFact]
    public async Task MissingHostDesktopEntryRemainsARegistrationFailure()
    {
        const string reason = "Could not register app ID: App info not found for 'sharex'";
        await WithPortal(reason, hasSandboxIdentity: false, (portal, connection, error) =>
        {
            Assert.Equal(reason, error);
            Assert.Equal(1, portal.RegistrationCalls);
            Assert.Equal(portal.RegisteredPeer, connection.UniqueName);
            return Task.CompletedTask;
        });
    }

    private static async Task WithPortal(string? rejection, bool hasSandboxIdentity,
        Func<Portal, DBusConnection, string?, Task> verify)
    {
        ProcessStartInfo start = new ProcessStartInfo("/usr/bin/dbus-daemon")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        start.ArgumentList.Add("--session");
        start.ArgumentList.Add("--nofork");
        start.ArgumentList.Add("--print-address=1");
        using Process daemon = Process.Start(start)!;

        try
        {
            string address = (await daemon.StandardOutput.ReadLineAsync().WaitAsync(TimeSpan.FromSeconds(5)))!;
            Assert.False(string.IsNullOrEmpty(address));
            using DBusConnection server = new DBusConnection(address);
            await server.ConnectAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5));
            Portal portal = new Portal(server, rejection);
            server.AddMethodHandler(portal);
            await server.RequestNameAsync(DBusSession.PortalBusName).WaitAsync(TimeSpan.FromSeconds(5));
            (DBusConnection connection, string? error) = await DBusSession.CreateConnectionAsync(address, hasSandboxIdentity)
                .WaitAsync(TimeSpan.FromSeconds(5));

            using (connection)
            {
                await verify(portal, connection, error).WaitAsync(TimeSpan.FromSeconds(5));
            }
        }
        finally
        {
            if (!daemon.HasExited)
            {
                daemon.Kill();
            }

            await daemon.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
        }
    }

    private static Task<uint> ReadVersion(DBusConnection connection) =>
        connection.CallMethodAsync(DBusSession.CreateMethodCall(connection, DBusSession.PortalBusName,
            DBusSession.PortalObjectPath, "org.freedesktop.DBus.Properties", "Get", "ss", (ref MessageWriter writer) =>
            {
                writer.WriteString("org.freedesktop.portal.GlobalShortcuts");
                writer.WriteString("version");
            }), static (Message message, object? _) => message.GetBodyReader().ReadVariantValue().GetUInt32(), null);

    private static Task<bool> NameHasOwner(DBusConnection connection, string peer) =>
        connection.CallMethodAsync(DBusSession.CreateMethodCall(connection, "org.freedesktop.DBus",
            "/org/freedesktop/DBus", "org.freedesktop.DBus", "NameHasOwner", "s",
            (ref MessageWriter writer) => writer.WriteString(peer)),
            static (Message message, object? _) => message.GetBodyReader().ReadBool(), null);

    private sealed class Portal(DBusConnection connection, string? rejection) : IPathMethodHandler
    {
        public string Path => DBusSession.PortalObjectPath;
        public bool HandlesChildPaths => false;
        public DBusConnection Connection => connection;
        public string? RegisteredPeer { get; private set; }
        public string? RegisteredId { get; private set; }
        public int RegistrationCalls { get; private set; }

        public ValueTask HandleMethodAsync(MethodContext context)
        {
            Message request = context.Request;

            if (request.InterfaceAsString == "org.freedesktop.host.portal.Registry" && request.MemberAsString == "Register")
            {
                RegisteredPeer = request.SenderAsString;
                Reader reader = request.GetBodyReader();
                RegisteredId = reader.ReadString();
                Assert.Empty(reader.ReadDictionaryOfStringToVariantValue());
                RegistrationCalls++;

                if (rejection != null)
                {
                    context.ReplyError("org.freedesktop.portal.Error.Failed", rejection);
                }
                else
                {
                    using MessageWriter reply = context.CreateReplyWriter(null);
                    context.Reply(reply.CreateMessage());
                }
            }
            else if (request.InterfaceAsString == "org.freedesktop.DBus.Properties" && request.MemberAsString == "Get")
            {
                if (rejection != null && request.SenderAsString == RegisteredPeer)
                {
                    context.ReplyError("org.freedesktop.portal.Error.NotAllowed", "Cached host registration failure");
                }
                else
                {
                    using MessageWriter reply = context.CreateReplyWriter("v");
                    reply.WriteVariantUInt32(1);
                    context.Reply(reply.CreateMessage());
                }
            }
            else
            {
                context.ReplyUnknownMethodError();
            }

            return ValueTask.CompletedTask;
        }
    }
}
