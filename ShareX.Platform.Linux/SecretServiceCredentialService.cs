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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Linux;

/// <summary>
/// Secrets through the freedesktop Secret Service (GNOME Keyring, KWallet 5.97+, KeePassXC), spoken directly over D-Bus so
/// no extra program is needed. Secrets stored earlier through secret-tool use the same attributes and are still found.
/// </summary>
public sealed class SecretServiceCredentialService : ICredentialService
{
    private readonly Lazy<bool> available = new Lazy<bool>(SecretService.IsAvailable);

    public FeatureSupport Support => available.Value
        ? FeatureSupport.Supported
        : FeatureSupport.NotSupported("No keyring service (such as GNOME Keyring or KWallet) is running in this session, so secrets cannot be stored securely.");

    public Task<bool> StoreAsync(string service, string account, string secret, CancellationToken cancellationToken = default)
    {
        EnsureSupported();
        return SecretService.StoreAsync($"{service}: {account}", Attributes(service, account), secret, cancellationToken);
    }

    public Task<string?> GetAsync(string service, string account, CancellationToken cancellationToken = default)
    {
        EnsureSupported();
        return SecretService.GetAsync(Attributes(service, account), cancellationToken);
    }

    public Task<bool> DeleteAsync(string service, string account, CancellationToken cancellationToken = default)
    {
        EnsureSupported();
        return SecretService.DeleteAsync(Attributes(service, account), cancellationToken);
    }

    private static Dictionary<string, string> Attributes(string service, string account) =>
        new Dictionary<string, string> { ["service"] = service, ["account"] = account };

    private void EnsureSupported()
    {
        if (!Support.IsSupported)
        {
            throw new PlatformNotSupportedException(Support.Reason);
        }
    }
}
