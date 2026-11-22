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

#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ShareX.Platform.Linux.Wayland;

internal static unsafe partial class WaylandNative
{
    private const string WaylandClientLib = "libwayland-client.so.0";
    private const string WaylandCursorLib = "libwayland-cursor.so.0";
    private const string XkbCommonLib = "libxkbcommon.so.0";
    private const string LibC = "libc";

    public const uint WL_MARSHAL_FLAG_DESTROY = 1;
    public const int WL_SHM_FORMAT_ARGB8888 = 0;

    public const int EAGAIN = 11;
    public const int EINTR = 4;

    public const uint MFD_CLOEXEC = 1;
    public const int EFD_NONBLOCK = 0x800;
    public const int EFD_CLOEXEC = 0x80000;

    public const int PROT_READ = 1;
    public const int PROT_WRITE = 2;
    public const int MAP_SHARED = 1;
    public const int MAP_PRIVATE = 2;
    public static readonly IntPtr MAP_FAILED = new(-1);

    public const int O_RDWR = 0x2;
    public const int O_CREAT = 0x40;
    public const int O_EXCL = 0x80;

    public const short POLLIN = 0x1;
    public const short POLLERR = 0x8;
    public const short POLLHUP = 0x10;

    public const int XKB_CONTEXT_NO_FLAGS = 0;
    public const int WL_KEYBOARD_KEYMAP_FORMAT_NO_KEYMAP = 0;
    public const int XKB_KEYMAP_FORMAT_TEXT_V1 = 1;
    public const int XKB_KEYMAP_COMPILE_NO_FLAGS = 0;
    public const int XKB_STATE_MODS_EFFECTIVE = 1 << 3;

    private static IntPtr waylandClientHandle;
    private static bool waylandProbed;
    private static bool waylandCursorAvailable;
    private static bool waylandCursorProbed;
    private static bool xkbAvailable;
    private static bool xkbProbed;
    private static readonly object LoadLock = new();

    public static bool IsWaylandClientAvailable
    {
        get
        {
            if (!waylandProbed)
            {
                lock (LoadLock)
                {
                    if (!waylandProbed)
                    {
                        NativeLibrary.TryLoad(WaylandClientLib, out waylandClientHandle);
                        waylandProbed = true;
                    }
                }
            }

            return waylandClientHandle != IntPtr.Zero;
        }
    }

    public static bool IsXkbAvailable
    {
        get
        {
            if (!xkbProbed)
            {
                lock (LoadLock)
                {
                    if (!xkbProbed)
                    {
                        xkbAvailable = NativeLibrary.TryLoad(XkbCommonLib, out _);
                        xkbProbed = true;
                    }
                }
            }

            return xkbAvailable;
        }
    }

    public static bool IsWaylandCursorAvailable
    {
        get
        {
            if (!waylandCursorProbed)
            {
                lock (LoadLock)
                {
                    if (!waylandCursorProbed)
                    {
                        waylandCursorAvailable = NativeLibrary.TryLoad(WaylandCursorLib, out _);
                        waylandCursorProbed = true;
                    }
                }
            }

            return waylandCursorAvailable;
        }
    }

    public static IntPtr GetWaylandExport(string symbolName)
    {
        if (!IsWaylandClientAvailable)
        {
            throw new InvalidOperationException("libwayland-client.so.0 is not available.");
        }

        return NativeLibrary.GetExport(waylandClientHandle, symbolName);
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    internal struct WlArgument
    {
        [FieldOffset(0)] public int Int32;
        [FieldOffset(0)] public uint UInt32;
        [FieldOffset(0)] public int Fixed;
        [FieldOffset(0)] public IntPtr Pointer;
        [FieldOffset(0)] public uint NewId;
        [FieldOffset(0)] public int Fd;

        public static int FixedFromDouble(double value) => (int)Math.Round(value * 256.0);

        public double FixedToDouble() => Fixed / 256.0;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WlMessage
    {
        public IntPtr Name;
        public IntPtr Signature;
        public WlInterface** Types;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WlInterface
    {
        public IntPtr Name;
        public int Version;
        public int MethodCount;
        public WlMessage* Methods;
        public int EventCount;
        public WlMessage* Events;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WlArray
    {
        public nuint Size;
        public nuint Alloc;
        public IntPtr Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PollFd
    {
        public int Fd;
        public short Events;
        public short Revents;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WlCursorImage
    {
        public uint Width;
        public uint Height;
        public uint HotspotX;
        public uint HotspotY;
        public uint Delay;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WlCursor
    {
        public uint ImageCount;
        public WlCursorImage** Images;
        public IntPtr Name;
    }

    // --- libwayland-client ---

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_display_connect", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr WlDisplayConnect(string? name);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_display_disconnect")]
    public static partial void WlDisplayDisconnect(IntPtr display);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_display_get_fd")]
    public static partial int WlDisplayGetFd(IntPtr display);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_display_roundtrip")]
    public static partial int WlDisplayRoundtrip(IntPtr display);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_display_flush", SetLastError = true)]
    public static partial int WlDisplayFlush(IntPtr display);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_display_prepare_read")]
    public static partial int WlDisplayPrepareRead(IntPtr display);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_display_cancel_read")]
    public static partial void WlDisplayCancelRead(IntPtr display);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_display_read_events")]
    public static partial int WlDisplayReadEvents(IntPtr display);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_display_dispatch_pending")]
    public static partial int WlDisplayDispatchPending(IntPtr display);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_display_get_error")]
    public static partial int WlDisplayGetError(IntPtr display);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_proxy_marshal_array_flags")]
    public static partial IntPtr WlProxyMarshalArrayFlags(
        IntPtr proxy,
        uint opcode,
        WlInterface* iface,
        uint version,
        uint flags,
        WlArgument* args);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_proxy_add_dispatcher")]
    public static partial int WlProxyAddDispatcher(
        IntPtr proxy,
        delegate* unmanaged[Cdecl]<IntPtr, IntPtr, uint, IntPtr, WlArgument*, int> dispatcher,
        IntPtr dispatcherData,
        IntPtr userData);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_proxy_get_version")]
    public static partial uint WlProxyGetVersion(IntPtr proxy);

    [LibraryImport(WaylandClientLib, EntryPoint = "wl_proxy_destroy")]
    public static partial void WlProxyDestroy(IntPtr proxy);

    // --- libwayland-cursor ---

    [LibraryImport(WaylandCursorLib, EntryPoint = "wl_cursor_theme_load", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr WlCursorThemeLoad(string? name, int size, IntPtr shm);

    [LibraryImport(WaylandCursorLib, EntryPoint = "wl_cursor_theme_destroy")]
    public static partial void WlCursorThemeDestroy(IntPtr theme);

    [LibraryImport(WaylandCursorLib, EntryPoint = "wl_cursor_theme_get_cursor", StringMarshalling = StringMarshalling.Utf8)]
    public static partial WlCursor* WlCursorThemeGetCursor(IntPtr theme, string name);

    [LibraryImport(WaylandCursorLib, EntryPoint = "wl_cursor_image_get_buffer")]
    public static partial IntPtr WlCursorImageGetBuffer(IntPtr image);

    // --- libxkbcommon ---

    [LibraryImport(XkbCommonLib, EntryPoint = "xkb_context_new")]
    public static partial IntPtr XkbContextNew(int flags);

    [LibraryImport(XkbCommonLib, EntryPoint = "xkb_context_unref")]
    public static partial void XkbContextUnref(IntPtr context);

    [LibraryImport(XkbCommonLib, EntryPoint = "xkb_keymap_new_from_buffer")]
    public static partial IntPtr XkbKeymapNewFromBuffer(IntPtr context, byte* buffer, nuint size, int format, int flags);

    [LibraryImport(XkbCommonLib, EntryPoint = "xkb_keymap_new_from_names")]
    public static partial IntPtr XkbKeymapNewFromNames(IntPtr context, IntPtr names, int flags);

    [LibraryImport(XkbCommonLib, EntryPoint = "xkb_keymap_unref")]
    public static partial void XkbKeymapUnref(IntPtr keymap);

    [LibraryImport(XkbCommonLib, EntryPoint = "xkb_state_new")]
    public static partial IntPtr XkbStateNew(IntPtr keymap);

    [LibraryImport(XkbCommonLib, EntryPoint = "xkb_state_unref")]
    public static partial void XkbStateUnref(IntPtr state);

    [LibraryImport(XkbCommonLib, EntryPoint = "xkb_state_update_mask")]
    public static partial int XkbStateUpdateMask(
        IntPtr state,
        uint depressedMods,
        uint latchedMods,
        uint lockedMods,
        uint depressedLayout,
        uint latchedLayout,
        uint lockedLayout);

    [LibraryImport(XkbCommonLib, EntryPoint = "xkb_state_key_get_one_sym")]
    public static partial uint XkbStateKeyGetOneSym(IntPtr state, uint key);

    [LibraryImport(XkbCommonLib, EntryPoint = "xkb_state_key_get_utf8")]
    public static partial int XkbStateKeyGetUtf8(IntPtr state, uint key, byte* buffer, nuint size);

    [LibraryImport(XkbCommonLib, EntryPoint = "xkb_state_mod_name_is_active", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int XkbStateModNameIsActive(IntPtr state, string name, int type);

    // --- libc ---

    [LibraryImport(LibC, EntryPoint = "memfd_create", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
    public static partial int MemfdCreate(string name, uint flags);

    [LibraryImport(LibC, EntryPoint = "shm_open", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
    public static partial int ShmOpen(string name, int oflag, uint mode);

    [LibraryImport(LibC, EntryPoint = "shm_unlink", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
    public static partial int ShmUnlink(string name);

    [LibraryImport(LibC, EntryPoint = "ftruncate", SetLastError = true)]
    public static partial int Ftruncate(int fd, long length);

    [LibraryImport(LibC, EntryPoint = "mmap", SetLastError = true)]
    public static partial IntPtr Mmap(IntPtr addr, nuint length, int prot, int flags, int fd, long offset);

    [LibraryImport(LibC, EntryPoint = "munmap", SetLastError = true)]
    public static partial int Munmap(IntPtr addr, nuint length);

    [LibraryImport(LibC, EntryPoint = "close", SetLastError = true)]
    public static partial int Close(int fd);

    [LibraryImport(LibC, EntryPoint = "eventfd", SetLastError = true)]
    public static partial int EventFd(uint initval, int flags);

    [LibraryImport(LibC, EntryPoint = "read", SetLastError = true)]
    public static partial nint Read(int fd, void* buf, nuint count);

    [LibraryImport(LibC, EntryPoint = "write", SetLastError = true)]
    public static partial nint Write(int fd, void* buf, nuint count);

    [LibraryImport(LibC, EntryPoint = "poll", SetLastError = true)]
    public static partial int Poll(PollFd* fds, nuint nfds, int timeoutMs);
}
