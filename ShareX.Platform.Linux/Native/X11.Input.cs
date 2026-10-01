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

/// <summary>Pointer warping, EWMH client messages, the SHAPE extension (libXext) and XTEST input (libXtst).</summary>
internal static unsafe partial class X11
{
    private const string LibXext = "libXext.so.6";
    private const string LibXtst = "libXtst.so.6";

    public const int ClientMessage = 33;
    public const nint SubstructureRedirectMask = 1 << 20;
    public const nint SubstructureNotifyMask = 1 << 19;
    public const int ShapeInput = 2;
    public const int ShapeSet = 0;
    public const int Unsorted = 0;

    [StructLayout(LayoutKind.Sequential)]
    public struct XClientMessageEvent
    {
        public int type;
        public nuint serial;
        public int send_event;
        public IntPtr display;
        public nuint window;
        public nuint message_type;
        public int format;
        public fixed long data[5];
    }

    /// <summary>XEvent is a union padded to 24 longs.</summary>
    [StructLayout(LayoutKind.Explicit, Size = 24 * 8)]
    public struct XClientMessageEventPadded
    {
        [FieldOffset(0)] public XClientMessageEvent xclient;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XRectangle
    {
        public short x;
        public short y;
        public ushort width;
        public ushort height;
    }

    [LibraryImport(LibX11)]
    public static partial int XWarpPointer(IntPtr display, nuint sourceWindow, nuint destinationWindow, int sourceX, int sourceY,
        uint sourceWidth, uint sourceHeight, int destinationX, int destinationY);

    [LibraryImport(LibX11)]
    public static partial int XFlush(IntPtr display);

    [LibraryImport(LibX11)]
    public static partial int XSendEvent(IntPtr display, nuint window, [MarshalAs(UnmanagedType.Bool)] bool propagate, nint eventMask,
        XClientMessageEventPadded* xEvent);

    [LibraryImport(LibXext)]
    public static partial void XShapeCombineRectangles(IntPtr display, nuint window, int destinationKind, int xOffset, int yOffset,
        XRectangle* rectangles, int count, int operation, int ordering);

    [LibraryImport(LibXtst)]
    public static partial int XTestFakeKeyEvent(IntPtr display, uint keycode, [MarshalAs(UnmanagedType.Bool)] bool isPress, nuint delay);

    [LibraryImport(LibXtst)]
    public static partial int XTestFakeButtonEvent(IntPtr display, uint button, [MarshalAs(UnmanagedType.Bool)] bool isPress, nuint delay);
}
