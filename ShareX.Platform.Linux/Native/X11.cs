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
using System.Runtime.InteropServices;
using System.Threading;

namespace ShareX.Platform.Linux.Native;

/// <summary>Xlib, XRandR and XFixes declarations. XIDs and Atoms are C unsigned long, which is nuint on 64 bit Linux.</summary>
internal static unsafe partial class X11
{
    private const string LibX11 = "libX11.so.6";
    private const string LibXrandr = "libXrandr.so.2";
    private const string LibXfixes = "libXfixes.so.3";

    public const int ZPixmap = 2;
    public const int KeyPress = 2;
    public const int GrabModeAsync = 1;
    public const int Success = 0;
    public const int BadAccess = 10;
    public const int IsViewable = 2;

    public const uint ShiftMask = 1 << 0;
    public const uint LockMask = 1 << 1;
    public const uint ControlMask = 1 << 2;
    public const uint Mod1Mask = 1 << 3; // Alt
    public const uint Mod2Mask = 1 << 4; // Num Lock
    public const uint Mod4Mask = 1 << 6; // Super

    public static readonly nuint AnyPropertyType = 0;
    public static readonly nuint AllPlanes = nuint.MaxValue;

    [StructLayout(LayoutKind.Sequential)]
    public struct XImage
    {
        public int width;
        public int height;
        public int xoffset;
        public int format;
        public IntPtr data;
        public int byte_order;
        public int bitmap_unit;
        public int bitmap_bit_order;
        public int bitmap_pad;
        public int depth;
        public int bytes_per_line;
        public int bits_per_pixel;
        public nuint red_mask;
        public nuint green_mask;
        public nuint blue_mask;
        public IntPtr obdata;
        public IntPtr create_image;
        public IntPtr destroy_image;
        public IntPtr get_pixel;
        public IntPtr put_pixel;
        public IntPtr sub_image;
        public IntPtr add_pixel;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XKeyEvent
    {
        public int type;
        public nuint serial;
        public int send_event;
        public IntPtr display;
        public nuint window;
        public nuint root;
        public nuint subwindow;
        public nuint time;
        public int x;
        public int y;
        public int x_root;
        public int y_root;
        public uint state;
        public uint keycode;
        public int same_screen;
    }

    /// <summary>XEvent is a union padded to 24 longs.</summary>
    [StructLayout(LayoutKind.Explicit, Size = 24 * 8)]
    public struct XEvent
    {
        [FieldOffset(0)] public int type;
        [FieldOffset(0)] public XKeyEvent xkey;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XErrorEvent
    {
        public int type;
        public IntPtr display;
        public nuint resourceid;
        public nuint serial;
        public byte error_code;
        public byte request_code;
        public byte minor_code;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XRRMonitorInfo
    {
        public nuint name;
        public int primary;
        public int automatic;
        public int noutput;
        public int x;
        public int y;
        public int width;
        public int height;
        public int mwidth;
        public int mheight;
        public IntPtr outputs;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XFixesCursorImage
    {
        public short x;
        public short y;
        public ushort width;
        public ushort height;
        public ushort xhot;
        public ushort yhot;
        public nuint cursor_serial;
        public nuint* pixels;
        public nuint atom;
        public IntPtr name;
    }

    [LibraryImport(LibX11)]
    public static partial IntPtr XOpenDisplay(IntPtr displayName);

    [LibraryImport(LibX11)]
    public static partial int XCloseDisplay(IntPtr display);

    [LibraryImport(LibX11)]
    public static partial int XDefaultScreen(IntPtr display);

    [LibraryImport(LibX11)]
    public static partial int XQueryPointer(IntPtr display, nuint window, out nuint rootReturn, out nuint childReturn,
        out int rootX, out int rootY, out int windowX, out int windowY, out uint mask);

    [LibraryImport(LibX11)]
    public static partial nuint XDefaultRootWindow(IntPtr display);

    [LibraryImport(LibX11)]
    public static partial int XDisplayWidth(IntPtr display, int screen);

    [LibraryImport(LibX11)]
    public static partial int XDisplayHeight(IntPtr display, int screen);

    [LibraryImport(LibX11)]
    public static partial int XConnectionNumber(IntPtr display);

    [LibraryImport(LibX11)]
    public static partial XImage* XGetImage(IntPtr display, nuint drawable, int x, int y, uint width, uint height, nuint planeMask, int format);

    [LibraryImport(LibX11)]
    public static partial int XFree(IntPtr data);

    [LibraryImport(LibX11, StringMarshalling = StringMarshalling.Utf8)]
    public static partial nuint XInternAtom(IntPtr display, string atomName, [MarshalAs(UnmanagedType.Bool)] bool onlyIfExists);

    [LibraryImport(LibX11)]
    public static partial IntPtr XGetAtomName(IntPtr display, nuint atom);

    [LibraryImport(LibX11)]
    public static partial int XGetWindowProperty(IntPtr display, nuint window, nuint property, nint longOffset, nint longLength,
        [MarshalAs(UnmanagedType.Bool)] bool delete, nuint reqType, out nuint actualType, out int actualFormat, out nuint itemCount,
        out nuint bytesAfter, out IntPtr prop);

    [LibraryImport(LibX11)]
    public static partial int XGetGeometry(IntPtr display, nuint drawable, out nuint root, out int x, out int y, out uint width,
        out uint height, out uint borderWidth, out uint depth);

    [LibraryImport(LibX11)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool XTranslateCoordinates(IntPtr display, nuint sourceWindow, nuint destinationWindow, int sourceX, int sourceY,
        out int destinationX, out int destinationY, out nuint child);

    [LibraryImport(LibX11)]
    public static partial int XFetchName(IntPtr display, nuint window, out IntPtr windowName);

    [LibraryImport(LibX11)]
    public static partial byte XKeysymToKeycode(IntPtr display, nuint keysym);

    [LibraryImport(LibX11)]
    public static partial int XGrabKey(IntPtr display, int keycode, uint modifiers, nuint grabWindow,
        [MarshalAs(UnmanagedType.Bool)] bool ownerEvents, int pointerMode, int keyboardMode);

    [LibraryImport(LibX11)]
    public static partial int XUngrabKey(IntPtr display, int keycode, uint modifiers, nuint grabWindow);

    [LibraryImport(LibX11)]
    public static partial int XSync(IntPtr display, [MarshalAs(UnmanagedType.Bool)] bool discard);

    [LibraryImport(LibX11)]
    public static partial int XPending(IntPtr display);

    [LibraryImport(LibX11)]
    public static partial int XNextEvent(IntPtr display, out XEvent xEvent);

    [LibraryImport(LibX11)]
    public static partial IntPtr XSetErrorHandler(IntPtr handler);

    [LibraryImport(LibXrandr)]
    public static partial XRRMonitorInfo* XRRGetMonitors(IntPtr display, nuint window, [MarshalAs(UnmanagedType.Bool)] bool getActive, out int monitorCount);

    [LibraryImport(LibXrandr)]
    public static partial void XRRFreeMonitors(XRRMonitorInfo* monitors);

    [LibraryImport(LibXfixes)]
    public static partial XFixesCursorImage* XFixesGetCursorImage(IntPtr display);

    public static void DestroyImage(XImage* image)
    {
        // XDestroyImage is a macro that calls through the image's function table.
        ((delegate* unmanaged<XImage*, int>)image->destroy_image)(image);
    }

    [ThreadStatic]
    private static int lastErrorCode;

    private static int errorHandlerInstalled;

    /// <summary>
    /// Xlib's default error handler terminates the process. Install one that records the error instead, so
    /// a window closing mid enumeration or a key already grabbed by another program is survivable.
    /// </summary>
    public static void EnsureErrorHandler()
    {
        if (Interlocked.Exchange(ref errorHandlerInstalled, 1) == 0)
        {
            XSetErrorHandler((IntPtr)(delegate* unmanaged<IntPtr, XErrorEvent*, int>)&OnError);
        }
    }

    /// <summary>Returns and clears the last X error seen on this thread.</summary>
    public static int TakeLastError()
    {
        int error = lastErrorCode;
        lastErrorCode = Success;
        return error;
    }

    [UnmanagedCallersOnly]
    private static int OnError(IntPtr display, XErrorEvent* error)
    {
        lastErrorCode = error->error_code;
        return 0;
    }

    /// <summary>Opens the default display. Returns an invalid handle when no X server is reachable.</summary>
    public static DisplayHandle OpenDisplay()
    {
        EnsureErrorHandler();
        return new DisplayHandle(XOpenDisplay(IntPtr.Zero));
    }

    public static string? PtrToString(IntPtr value) => value == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(value);
}

internal sealed class DisplayHandle : SafeHandle
{
    public DisplayHandle(IntPtr handle)
        : base(IntPtr.Zero, true)
    {
        SetHandle(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    public IntPtr Display => handle;

    protected override bool ReleaseHandle()
    {
        X11.XCloseDisplay(handle);
        return true;
    }
}
