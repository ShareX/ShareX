using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Windows;

/// <summary>Generic credentials in Windows Credential Manager, protected with the user's DPAPI key.</summary>
public sealed unsafe partial class WindowsCredentialService : ICredentialService
{
    private const uint CRED_TYPE_GENERIC = 1;
    private const uint CRED_PERSIST_LOCAL_MACHINE = 2;
    private const int ERROR_NOT_FOUND = 1168;

    [StructLayout(LayoutKind.Sequential)]
    private struct CREDENTIAL
    {
        public uint Flags;
        public uint Type;
        public char* TargetName;
        public char* Comment;
        public long LastWritten;
        public uint CredentialBlobSize;
        public byte* CredentialBlob;
        public uint Persist;
        public uint AttributeCount;
        public IntPtr Attributes;
        public char* TargetAlias;
        public char* UserName;
    }

    [LibraryImport("advapi32.dll", EntryPoint = "CredWriteW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CredWrite(CREDENTIAL* credential, uint flags);

    [LibraryImport("advapi32.dll", EntryPoint = "CredReadW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CredRead(string target, uint type, uint flags, out CREDENTIAL* credential);

    [LibraryImport("advapi32.dll", EntryPoint = "CredDeleteW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CredDelete(string target, uint type, uint flags);

    [LibraryImport("advapi32.dll")]
    private static partial void CredFree(void* buffer);

    public FeatureSupport Support => FeatureSupport.Supported;

    public static string GetTargetName(string service, string account) => $"{service}:{account}";

    public Task<bool> StoreAsync(string service, string account, string secret, CancellationToken cancellationToken = default)
    {
        byte[] blob = Encoding.UTF8.GetBytes(secret);

        try
        {
            fixed (char* target = GetTargetName(service, account), user = account)
            fixed (byte* data = blob)
            {
                CREDENTIAL credential = new CREDENTIAL
                {
                    Type = CRED_TYPE_GENERIC,
                    TargetName = target,
                    UserName = user,
                    CredentialBlob = data,
                    CredentialBlobSize = (uint)blob.Length,
                    Persist = CRED_PERSIST_LOCAL_MACHINE
                };

                return Task.FromResult(CredWrite(&credential, 0));
            }
        }
        finally
        {
            Array.Clear(blob);
        }
    }

    public Task<string?> GetAsync(string service, string account, CancellationToken cancellationToken = default)
    {
        if (!CredRead(GetTargetName(service, account), CRED_TYPE_GENERIC, 0, out CREDENTIAL* credential))
        {
            return Task.FromResult<string?>(null);
        }

        try
        {
            return Task.FromResult<string?>(Encoding.UTF8.GetString(credential->CredentialBlob, (int)credential->CredentialBlobSize));
        }
        finally
        {
            CredFree(credential);
        }
    }

    public Task<bool> DeleteAsync(string service, string account, CancellationToken cancellationToken = default)
    {
        bool deleted = CredDelete(GetTargetName(service, account), CRED_TYPE_GENERIC, 0);
        return Task.FromResult(deleted || Marshal.GetLastPInvokeError() == ERROR_NOT_FOUND);
    }
}
