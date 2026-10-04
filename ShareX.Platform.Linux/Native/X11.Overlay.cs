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

namespace ShareX.Platform.Linux.Native;

/// <summary>Xlib calls for the click-through overlay window and the pointer poller.</summary>
internal static unsafe partial class X11
{
    public const int TrueColor = 4;
    public const int InputOutput = 1;
    public const int AllocNone = 0;
    public const nuint CWBackPixel = 1 << 1;
    public const nuint CWBorderPixel = 1 << 3;
    public const nuint CWOverrideRedirect = 1 << 9;
    public const nuint CWColormap = 1 << 13;
    public const uint Button1Mask = 1 << 8;
    public const uint Button2Mask = 1 << 9;
    public const uint Button3Mask = 1 << 10;

    [StructLayout(LayoutKind.Sequential)]
    public struct XVisualInfo
    {
        public IntPtr visual;
        public nuint visualid;
        public int screen;
        public int depth;
        public int c_class;
        public nuint red_mask;
        public nuint green_mask;
        public nuint blue_mask;
        public int colormap_size;
        public int bits_per_rgb;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XSetWindowAttributes
    {
        public nuint background_pixmap;
        public nuint background_pixel;
        public nuint border_pixmap;
        public nuint border_pixel;
        public int bit_gravity;
        public int win_gravity;
        public int backing_store;
        public nuint backing_planes;
        public nuint backing_pixel;
        public int save_under;
        public nint event_mask;
        public nint do_not_propagate_mask;
        public int override_redirect;
        public nuint colormap;
        public nuint cursor;
    }

    [LibraryImport(LibX11)]
    public static partial int XMatchVisualInfo(IntPtr display, int screen, int depth, int visualClass, out XVisualInfo visualInfo);

    [LibraryImport(LibX11)]
    public static partial nuint XCreateColormap(IntPtr display, nuint window, IntPtr visual, int alloc);

    [LibraryImport(LibX11)]
    public static partial int XFreeColormap(IntPtr display, nuint colormap);

    [LibraryImport(LibX11)]
    public static partial nuint XCreateWindow(IntPtr display, nuint parent, int x, int y, uint width, uint height, uint borderWidth,
        int depth, uint windowClass, IntPtr visual, nuint valueMask, XSetWindowAttributes* attributes);

    [LibraryImport(LibX11)]
    public static partial int XDestroyWindow(IntPtr display, nuint window);

    [LibraryImport(LibX11)]
    public static partial int XMapRaised(IntPtr display, nuint window);

    [LibraryImport(LibX11)]
    public static partial int XUnmapWindow(IntPtr display, nuint window);

    [LibraryImport(LibX11)]
    public static partial int XMoveResizeWindow(IntPtr display, nuint window, int x, int y, uint width, uint height);

    [LibraryImport(LibX11)]
    public static partial int XRaiseWindow(IntPtr display, nuint window);

    [LibraryImport(LibX11)]
    public static partial IntPtr XCreateGC(IntPtr display, nuint drawable, nuint valueMask, IntPtr values);

    [LibraryImport(LibX11)]
    public static partial int XFreeGC(IntPtr display, IntPtr gc);

    [LibraryImport(LibX11)]
    public static partial int XInitImage(XImage* image);

    [LibraryImport(LibX11)]
    public static partial int XPutImage(IntPtr display, nuint drawable, IntPtr gc, XImage* image, int sourceX, int sourceY,
        int destinationX, int destinationY, uint width, uint height);

    [LibraryImport(LibX11)]
    public static partial nuint XGetSelectionOwner(IntPtr display, nuint selection);
}
