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
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Xunit;

namespace ShareX.Platform.Tests;

public class PortalApplicationRegistrationTests
{
    [Theory]
    [InlineData("0::/user.slice/user-1000.slice/user@1000.service/app.slice/snap.code.code-123.scope\n", true)]
    [InlineData("0::/user.slice/snap.sharex.sharex.scope/child", true)]
    [InlineData("4:freezer:/snap.sharex", true)]
    [InlineData("2:name=systemd:/user.slice/snap.code.scope", true)]
    [InlineData("2:cpu:/snap.sharex\n0::/user.slice/snap.code.scope\n", true)]
    [InlineData("0::/user.slice/app-sharex.scope", false)]
    [InlineData("0::/user.slice/app-org.chromium.Chromium.scope", false)]
    [InlineData("2:cpu:/snap.sharex", false)]
    [InlineData("2:memory:/snap.sharex", false)]
    [InlineData("0::/user.slice/app-snap.sharex.scope", false)]
    [InlineData("0::/user.slice/snapshot.scope", false)]
    [InlineData("malformed\n0:missing-path\n", false)]
    [InlineData("", false)]
    public void SnapIdentityMatchesThePortalsCgroupControllers(string cgroups, bool expected)
    {
        Assert.Equal(expected, PortalApplicationRegistration.IsSnapCgroup(cgroups));
    }

    [Fact]
    public async Task SandboxIdentityNeverCallsTheHostRegistry()
    {
        int calls = 0;
        PortalRegistrationResult result = await PortalApplicationRegistration.RegisterAsync(() =>
        {
            calls++;
            throw new DBusErrorReplyException("org.freedesktop.portal.Error.Failed",
                "Could not register app ID: Can't manually register a io.snapcraft application");
        }, hasSandboxIdentity: true);

        Assert.Equal(0, calls);
        Assert.Null(result.Error);
        Assert.False(result.UseNewConnection);
    }

    [Fact]
    public async Task HostRegistrationIsAwaitedBeforePublishingSuccess()
    {
        TaskCompletionSource completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        int calls = 0;
        Task<PortalRegistrationResult> registration = PortalApplicationRegistration.RegisterAsync(() =>
        {
            calls++;
            return completion.Task;
        }, hasSandboxIdentity: false);

        Assert.Equal(1, calls);
        Assert.False(registration.IsCompleted);
        completion.SetResult();
        PortalRegistrationResult result = await registration;
        Assert.Null(result.Error);
        Assert.False(result.UseNewConnection);
    }

    [Theory]
    [InlineData("org.freedesktop.portal.Error.Failed", "Could not register app ID: Can't manually register a io.snapcraft application")]
    [InlineData("org.freedesktop.portal.Error.NotAllowed", "Can't manually register a io.snapcraft application")]
    [InlineData("org.freedesktop.portal.Error.Failed", "Could not register app ID: Can't manually register a Snap application")]
    [InlineData("org.freedesktop.portal.Error.Failed", "Could not register app ID: Can't manually register a Flatpak application")]
    [InlineData("org.freedesktop.portal.Error.NotAllowed", "Can't manually register a org.flatpak application")]
    public async Task UndetectedSandboxUsesAFreshPeerForAutomaticPortalIdentity(string name, string message)
    {
        PortalRegistrationResult result = await RejectRegistration(name, message);
        Assert.Null(result.Error);
        Assert.True(result.UseNewConnection);
    }

    [Theory]
    [InlineData("org.freedesktop.DBus.Error.UnknownMethod")]
    [InlineData("org.freedesktop.DBus.Error.UnknownInterface")]
    [InlineData("org.freedesktop.DBus.Error.ServiceUnknown")]
    public async Task OlderPortalsCanIdentifyTheApplicationWithoutTheRegistry(string name)
    {
        PortalRegistrationResult result = await RejectRegistration(name, "Unavailable");
        Assert.Null(result.Error);
        Assert.False(result.UseNewConnection);
    }

    [Theory]
    [InlineData("org.freedesktop.portal.Error.Failed", "Could not register app ID: App info not found for 'sharex'")]
    [InlineData("org.freedesktop.portal.Error.Failed", "Could not register app ID: Connection already associated with an application ID")]
    [InlineData("org.freedesktop.portal.Error.NotAllowed", "Permission denied")]
    [InlineData("org.freedesktop.portal.Error.Failed", "Can't manually register unknown application")]
    [InlineData("org.freedesktop.portal.Error.InvalidArgument", "Can't manually register a io.snapcraft application")]
    [InlineData("org.freedesktop.portal.Error.Failed", "Other failure: Can't manually register a io.snapcraft application")]
    public async Task MissingDesktopEntriesAndUnrelatedErrorsRemainFailures(string name, string message)
    {
        PortalRegistrationResult result = await RejectRegistration(name, message);
        Assert.Equal(message, result.Error);
        Assert.False(result.UseNewConnection);
    }

    [Fact]
    public async Task TransportFailuresAreNotReportedAsSuccessfulIdentity()
    {
        await Assert.ThrowsAsync<TimeoutException>(() => PortalApplicationRegistration.RegisterAsync(
            () => Task.FromException(new TimeoutException()), hasSandboxIdentity: false));
    }

    private static Task<PortalRegistrationResult> RejectRegistration(string name, string message) =>
        PortalApplicationRegistration.RegisterAsync(
            () => Task.FromException(new DBusErrorReplyException(name, message)), hasSandboxIdentity: false);
}
