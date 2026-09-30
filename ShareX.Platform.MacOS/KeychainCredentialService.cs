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
