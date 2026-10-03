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

/// <summary>Cursor themes through libXcursor, which reads the theme files directly and needs no display connection.</summary>
internal static unsafe partial class Xcursor
{
    private const string LibXcursor = "libXcursor.so.1";

    [StructLayout(LayoutKind.Sequential)]
    public struct XcursorImage
    {
        public uint Version;
        public uint Size;
        public uint Width;
        public uint Height;
        public uint XHot;
        public uint YHot;
        public uint Delay;
        /// <summary>Width * Height premultiplied ARGB values.</summary>
        public uint* Pixels;
    }

    [LibraryImport(LibXcursor, StringMarshalling = StringMarshalling.Utf8)]
    public static partial XcursorImage* XcursorLibraryLoadImage(string name, string? theme, int size);

    [LibraryImport(LibXcursor)]
    public static partial void XcursorImageDestroy(XcursorImage* image);

    public static bool IsAvailable { get; } = NativeLibrary.TryLoad(LibXcursor, typeof(Xcursor).Assembly, null, out _);
}
