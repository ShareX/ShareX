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

using ShareX.Platform.Linux;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.Platform.Tests;

/// <summary>Writes to the user's real keyring, so it runs only when SHAREX_KEYRING_TEST=1 is set.</summary>
public sealed class KeyringFactAttribute : FactAttribute
{
    public KeyringFactAttribute()
    {
        if (!OperatingSystem.IsLinux() || Environment.GetEnvironmentVariable("SHAREX_KEYRING_TEST") != "1")
        {
            Skip = "Set SHAREX_KEYRING_TEST=1 on a Linux desktop to test against the real keyring.";
        }
    }
}

public class SecretServiceCredentialTests
{
    [KeyringFact]
    public async Task StoreGetReplaceDelete_RoundTripsThroughTheKeyring()
    {
        SecretServiceCredentialService service = new SecretServiceCredentialService();
        string account = "test-" + Guid.NewGuid().ToString("N");

        Assert.True(service.Support.IsSupported, service.Support.Reason);

        try
        {
            Assert.True(await service.StoreAsync("ShareX-test", account, "first ✓"));
            Assert.Equal("first ✓", await service.GetAsync("ShareX-test", account));
            Assert.True(await service.StoreAsync("ShareX-test", account, "second"));
            Assert.Equal("second", await service.GetAsync("ShareX-test", account));
        }
        finally
        {
            Assert.True(await service.DeleteAsync("ShareX-test", account));
        }

        Assert.Null(await service.GetAsync("ShareX-test", account));
        Assert.False(await service.DeleteAsync("ShareX-test", account));
    }
}
