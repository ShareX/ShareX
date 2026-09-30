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

using ShareX.Platform.MacOS.Native;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.MacOS;

/// <summary>Generic passwords in the user's login keychain.</summary>
public sealed unsafe class KeychainCredentialService : ICredentialService
{
    public FeatureSupport Support => FeatureSupport.Supported;

    public Task<bool> StoreAsync(string service, string account, string secret, CancellationToken cancellationToken = default)
    {
        byte[] serviceBytes = Encoding.UTF8.GetBytes(service);
        byte[] accountBytes = Encoding.UTF8.GetBytes(account);
        byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

        fixed (byte* s = serviceBytes, a = accountBytes, p = secretBytes)
        {
            int status = Security.SecKeychainAddGenericPassword(IntPtr.Zero, (uint)serviceBytes.Length, s, (uint)accountBytes.Length, a,
                (uint)secretBytes.Length, p, out IntPtr item);

            if (status == Security.errSecDuplicateItem)
            {
                status = Security.SecKeychainFindGenericPassword(IntPtr.Zero, (uint)serviceBytes.Length, s, (uint)accountBytes.Length, a,
                    out _, out IntPtr existing, out item);

                if (status == Security.errSecSuccess)
                {
                    Security.SecKeychainItemFreeContent(IntPtr.Zero, existing);
                    status = Security.SecKeychainItemModifyAttributesAndData(item, IntPtr.Zero, (uint)secretBytes.Length, p);
                }
            }

            if (item != IntPtr.Zero)
            {
                CoreFoundation.CFRelease(item);
            }

            CryptographicClear(secretBytes);
            return Task.FromResult(status == Security.errSecSuccess);
        }
    }

    public Task<string?> GetAsync(string service, string account, CancellationToken cancellationToken = default)
    {
        byte[] serviceBytes = Encoding.UTF8.GetBytes(service);
        byte[] accountBytes = Encoding.UTF8.GetBytes(account);

        fixed (byte* s = serviceBytes, a = accountBytes)
        {
            int status = Security.SecKeychainFindGenericPassword(IntPtr.Zero, (uint)serviceBytes.Length, s, (uint)accountBytes.Length, a,
                out uint length, out IntPtr data, out IntPtr item);

            if (status != Security.errSecSuccess)
            {
                return Task.FromResult<string?>(null);
            }

            try
            {
                return Task.FromResult<string?>(Marshal.PtrToStringUTF8(data, (int)length));
            }
            finally
            {
                Security.SecKeychainItemFreeContent(IntPtr.Zero, data);
                CoreFoundation.CFRelease(item);
            }
        }
    }

    public Task<bool> DeleteAsync(string service, string account, CancellationToken cancellationToken = default)
    {
        byte[] serviceBytes = Encoding.UTF8.GetBytes(service);
        byte[] accountBytes = Encoding.UTF8.GetBytes(account);

        fixed (byte* s = serviceBytes, a = accountBytes)
        {
            int status = Security.SecKeychainFindGenericPassword(IntPtr.Zero, (uint)serviceBytes.Length, s, (uint)accountBytes.Length, a,
                out _, out IntPtr data, out IntPtr item);

            if (status != Security.errSecSuccess)
            {
                return Task.FromResult(status == Security.errSecItemNotFound);
            }

            Security.SecKeychainItemFreeContent(IntPtr.Zero, data);
            status = Security.SecKeychainItemDelete(item);
            CoreFoundation.CFRelease(item);
            return Task.FromResult(status == Security.errSecSuccess);
        }
    }

    private static void CryptographicClear(byte[] buffer) => Array.Clear(buffer);
}
