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
using System.IO;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib;

internal static class AuthenticodeSignatureVerifier
{
    private static readonly Guid WinTrustActionGenericVerifyV2 = new("00AAC56B-CD44-11D0-8CC2-00C04FC295EE");

    public static bool IsTrusted(string filePath)
    {
        if (!File.Exists(filePath)) return false;

        IntPtr fileInfoPointer = IntPtr.Zero;

        try
        {
            WinTrustFileInfo fileInfo = new()
            {
                StructSize = (uint)Marshal.SizeOf<WinTrustFileInfo>(),
                FilePath = filePath
            };

            fileInfoPointer = Marshal.AllocHGlobal(Marshal.SizeOf<WinTrustFileInfo>());
            Marshal.StructureToPtr(fileInfo, fileInfoPointer, false);

            WinTrustData trustData = new()
            {
                StructSize = (uint)Marshal.SizeOf<WinTrustData>(),
                UIChoice = WinTrustDataUIChoice.None,
                RevocationChecks = WinTrustDataRevocationChecks.WholeChain,
                UnionChoice = WinTrustDataChoice.File,
                FileInfoPointer = fileInfoPointer,
                StateAction = WinTrustDataStateAction.Ignore,
                UIContext = WinTrustDataUIContext.Execute
            };

            Guid action = WinTrustActionGenericVerifyV2;
            int result = WinVerifyTrust(new IntPtr(-1), ref action, ref trustData);

            if (result != 0)
            {
                DebugHelper.WriteLine($"Authenticode verification failed for \"{filePath}\" with error 0x{result:X8}.");
            }

            return result == 0;
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception, $"Authenticode verification failed for \"{filePath}\".");
            return false;
        }
        finally
        {
            if (fileInfoPointer != IntPtr.Zero)
            {
                Marshal.DestroyStructure<WinTrustFileInfo>(fileInfoPointer);
                Marshal.FreeHGlobal(fileInfoPointer);
            }
        }
    }

    [DllImport("wintrust.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern int WinVerifyTrust(IntPtr windowHandle, ref Guid actionId, ref WinTrustData trustData);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WinTrustFileInfo
    {
        public uint StructSize;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string FilePath;

        public IntPtr FileHandle;
        public IntPtr KnownSubject;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WinTrustData
    {
        public uint StructSize;
        public IntPtr PolicyCallbackData;
        public IntPtr SIPClientData;
        public WinTrustDataUIChoice UIChoice;
        public WinTrustDataRevocationChecks RevocationChecks;
        public WinTrustDataChoice UnionChoice;
        public IntPtr FileInfoPointer;
        public WinTrustDataStateAction StateAction;
        public IntPtr StateData;
        public IntPtr URLReference;
        public WinTrustDataProviderFlags ProviderFlags;
        public WinTrustDataUIContext UIContext;
        public IntPtr SignatureSettings;
    }

    private enum WinTrustDataUIChoice : uint
    {
        None = 2
    }

    private enum WinTrustDataRevocationChecks : uint
    {
        WholeChain = 1
    }

    private enum WinTrustDataChoice : uint
    {
        File = 1
    }

    private enum WinTrustDataStateAction : uint
    {
        Ignore = 0
    }

    [Flags]
    private enum WinTrustDataProviderFlags : uint
    {
        None = 0
    }

    private enum WinTrustDataUIContext : uint
    {
        Execute = 0
    }
}
