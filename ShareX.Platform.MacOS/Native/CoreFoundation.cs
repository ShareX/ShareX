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

namespace ShareX.Platform.MacOS.Native;

/// <summary>CoreFoundation helpers. CF types are toll free bridged to their Foundation counterparts (CFStringRef is NSString*).</summary>
internal static unsafe partial class CoreFoundation
{
    public const string Library = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    public const int kCFNumberSInt32Type = 3;
    public const int kCFNumberSInt64Type = 4;
    public const int kCFNumberDoubleType = 13;
    public const int kCFURLPOSIXPathStyle = 0;

    [LibraryImport(Library)]
    public static partial void CFRelease(IntPtr cf);

    [LibraryImport(Library)]
    public static partial IntPtr CFRetain(IntPtr cf);

    [LibraryImport(Library)]
    public static partial IntPtr CFStringCreateWithCharacters(IntPtr allocator, char* chars, nint length);

    [LibraryImport(Library)]
    public static partial nint CFStringGetLength(IntPtr str);

    [LibraryImport(Library)]
    public static partial void CFStringGetCharacters(IntPtr str, CFRange range, char* buffer);

    [LibraryImport(Library)]
    public static partial nint CFArrayGetCount(IntPtr array);

    [LibraryImport(Library)]
    public static partial IntPtr CFArrayGetValueAtIndex(IntPtr array, nint index);

    [LibraryImport(Library)]
    public static partial IntPtr CFArrayCreate(IntPtr allocator, IntPtr* values, nint count, IntPtr callBacks);

    [LibraryImport(Library)]
    public static partial IntPtr CFDictionaryGetValue(IntPtr dictionary, IntPtr key);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool CFNumberGetValue(IntPtr number, int type, void* value);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool CFBooleanGetValue(IntPtr boolean);

    [LibraryImport(Library)]
    public static partial IntPtr CFDataCreate(IntPtr allocator, byte* bytes, nint length);

    [LibraryImport(Library)]
    public static partial nint CFDataGetLength(IntPtr data);

    [LibraryImport(Library)]
    public static partial byte* CFDataGetBytePtr(IntPtr data);

    [LibraryImport(Library)]
    public static partial IntPtr CFURLCreateWithFileSystemPath(IntPtr allocator, IntPtr filePath, int pathStyle, [MarshalAs(UnmanagedType.U1)] bool isDirectory);

    [LibraryImport(Library)]
    public static partial IntPtr CFURLCopyFileSystemPath(IntPtr url, int pathStyle);

    [StructLayout(LayoutKind.Sequential)]
    public struct CFRange
    {
        public nint location;
        public nint length;
    }

    private static readonly Lazy<IntPtr> typeArrayCallBacks = new Lazy<IntPtr>(() =>
        NativeLibrary.GetExport(NativeLibrary.Load(Library), "kCFTypeArrayCallBacks"));

    /// <summary>Creates a CFString the caller must release.</summary>
    public static IntPtr CreateString(string value)
    {
        fixed (char* chars = value)
        {
            return CFStringCreateWithCharacters(IntPtr.Zero, chars, value.Length);
        }
    }

    public static string? ToManagedString(IntPtr str)
    {
        if (str == IntPtr.Zero)
        {
            return null;
        }

        nint length = CFStringGetLength(str);

        if (length == 0)
        {
            return "";
        }

        string result = new string('\0', (int)length);

        fixed (char* buffer = result)
        {
            CFStringGetCharacters(str, new CFRange { location = 0, length = length }, buffer);
        }

        return result;
    }

    /// <summary>Creates a CFArray that retains its items. The caller releases the array.</summary>
    public static IntPtr CreateArray(ReadOnlySpan<IntPtr> items)
    {
        fixed (IntPtr* values = items)
        {
            return CFArrayCreate(IntPtr.Zero, values, items.Length, typeArrayCallBacks.Value);
        }
    }

    public static IntPtr CreateData(ReadOnlySpan<byte> bytes)
    {
        fixed (byte* pointer = bytes)
        {
            return CFDataCreate(IntPtr.Zero, pointer, bytes.Length);
        }
    }

    public static byte[] ToArray(IntPtr data)
    {
        nint length = CFDataGetLength(data);
        return new ReadOnlySpan<byte>(CFDataGetBytePtr(data), (int)length).ToArray();
    }

    /// <summary>Looks up a value by a string key. The returned value is not retained.</summary>
    public static IntPtr GetValue(IntPtr dictionary, string key)
    {
        IntPtr cfKey = CreateString(key);

        try
        {
            return CFDictionaryGetValue(dictionary, cfKey);
        }
        finally
        {
            CFRelease(cfKey);
        }
    }

    public static long? GetInt64(IntPtr dictionary, string key)
    {
        IntPtr number = GetValue(dictionary, key);
        long value = 0;
        return number != IntPtr.Zero && CFNumberGetValue(number, kCFNumberSInt64Type, &value) ? value : null;
    }

    public static string? GetString(IntPtr dictionary, string key) => ToManagedString(GetValue(dictionary, key));
}
