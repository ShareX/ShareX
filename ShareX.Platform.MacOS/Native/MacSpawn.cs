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
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ShareX.Platform.MacOS.Native;

/// <summary>posix_spawn with Darwin's flag values, for starting ShareX detached from the browser's native messaging host.</summary>
internal static unsafe partial class MacSpawn
{
    private const string Library = "/usr/lib/libSystem.B.dylib";
    private const int O_RDWR = 2;
    // Darwin values (spawn.h).
    private const short POSIX_SPAWN_SETSIGDEF = 0x04;
    private const short POSIX_SPAWN_SETSIGMASK = 0x08;
    private const short POSIX_SPAWN_SETSID = 0x0400;
    // posix_spawnattr_t and posix_spawn_file_actions_t are pointers on Darwin and sigset_t is 32 bits; this is ample.
    private const int OpaqueSize = 256;

    /// <summary>Starts <paramref name="path"/> in a new session with default signals, an empty mask and stdio on /dev/null.</summary>
    public static int SpawnDetached(string path, string[] argv, string[] envp)
    {
        byte* attributes = stackalloc byte[OpaqueSize];
        byte* actions = stackalloc byte[OpaqueSize];
        byte* signals = stackalloc byte[OpaqueSize];
        new Span<byte>(attributes, OpaqueSize).Clear();
        new Span<byte>(actions, OpaqueSize).Clear();
        new Span<byte>(signals, OpaqueSize).Clear();
        Check(posix_spawnattr_init(attributes));
        Check(posix_spawn_file_actions_init(actions));
        nint argvBlock = 0, envBlock = 0;

        try
        {
            Check(posix_spawnattr_setsigmask(attributes, signals));
            sigfillset(signals);
            Check(posix_spawnattr_setsigdefault(attributes, signals));
            Check(posix_spawnattr_setflags(attributes, (short)(POSIX_SPAWN_SETSID | POSIX_SPAWN_SETSIGMASK | POSIX_SPAWN_SETSIGDEF)));

            for (int fd = 0; fd <= 2; fd++)
            {
                Check(posix_spawn_file_actions_addopen(actions, fd, "/dev/null", O_RDWR, 0));
            }

            argvBlock = AllocStringArray(argv);
            envBlock = AllocStringArray(envp);
            Check(posix_spawn(out int pid, path, actions, attributes, argvBlock, envBlock));
            return pid;
        }
        finally
        {
            FreeStringArray(argvBlock, argv.Length);
            FreeStringArray(envBlock, envp.Length);
            posix_spawn_file_actions_destroy(actions);
            posix_spawnattr_destroy(attributes);
        }
    }

    private static void Check(int error)
    {
        if (error != 0)
        {
            throw new Win32Exception(error);
        }
    }

    private static nint AllocStringArray(string[] values)
    {
        nint block = Marshal.AllocHGlobal((values.Length + 1) * IntPtr.Size);
        for (int i = 0; i < values.Length; i++)
        {
            Marshal.WriteIntPtr(block, i * IntPtr.Size, Marshal.StringToCoTaskMemUTF8(values[i]));
        }
        Marshal.WriteIntPtr(block, values.Length * IntPtr.Size, 0);
        return block;
    }

    private static void FreeStringArray(nint block, int count)
    {
        if (block == 0)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            Marshal.FreeCoTaskMem(Marshal.ReadIntPtr(block, i * IntPtr.Size));
        }
        Marshal.FreeHGlobal(block);
    }

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    private static partial int posix_spawn(out int pid, string path, byte* fileActions, byte* attributes, nint argv, nint envp);

    [LibraryImport(Library)]
    private static partial int posix_spawnattr_init(byte* attributes);

    [LibraryImport(Library)]
    private static partial int posix_spawnattr_destroy(byte* attributes);

    [LibraryImport(Library)]
    private static partial int posix_spawnattr_setflags(byte* attributes, short flags);

    [LibraryImport(Library)]
    private static partial int posix_spawnattr_setsigmask(byte* attributes, byte* signals);

    [LibraryImport(Library)]
    private static partial int posix_spawnattr_setsigdefault(byte* attributes, byte* signals);

    [LibraryImport(Library)]
    private static partial int sigfillset(byte* signals);

    [LibraryImport(Library)]
    private static partial int posix_spawn_file_actions_init(byte* actions);

    [LibraryImport(Library)]
    private static partial int posix_spawn_file_actions_destroy(byte* actions);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    private static partial int posix_spawn_file_actions_addopen(byte* actions, int fd, string path, int flags, ushort mode);
}
