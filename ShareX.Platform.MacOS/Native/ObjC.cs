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

/// <summary>Minimal Objective-C runtime bridge for the few AppKit calls without a C API (NSPasteboard, NSWorkspace).</summary>
internal static unsafe partial class ObjC
{
    private const string LibObjC = "/usr/lib/libobjc.A.dylib";
    private const string AppKit = "/System/Library/Frameworks/AppKit.framework/AppKit";

    private static readonly Lazy<bool> appKitLoaded = new Lazy<bool>(() => NativeLibrary.TryLoad(AppKit, out _));
    private static readonly IntPtr msgSend = NativeLibrary.GetExport(NativeLibrary.Load(LibObjC), "objc_msgSend");

    [LibraryImport(LibObjC, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr objc_getClass(string name);

    [LibraryImport(LibObjC, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr sel_registerName(string name);

    [LibraryImport(LibObjC)]
    public static partial IntPtr objc_autoreleasePoolPush();

    [LibraryImport(LibObjC)]
    public static partial void objc_autoreleasePoolPop(IntPtr pool);

    public static IntPtr GetClass(string name)
    {
        if (!appKitLoaded.Value)
        {
            throw new PlatformNotSupportedException("AppKit could not be loaded.");
        }

        IntPtr cls = objc_getClass(name);
        return cls != IntPtr.Zero ? cls : throw new EntryPointNotFoundException($"Objective-C class {name} was not found.");
    }

    public static IntPtr Selector(string name) => sel_registerName(name);

    // objc_msgSend must be called through a pointer cast to the exact signature of each method.
    public static IntPtr Send(IntPtr receiver, string selector) =>
        ((delegate* unmanaged<IntPtr, IntPtr, IntPtr>)msgSend)(receiver, Selector(selector));

    public static IntPtr Send(IntPtr receiver, string selector, IntPtr arg1) =>
        ((delegate* unmanaged<IntPtr, IntPtr, IntPtr, IntPtr>)msgSend)(receiver, Selector(selector), arg1);

    public static IntPtr Send(IntPtr receiver, string selector, IntPtr arg1, IntPtr arg2) =>
        ((delegate* unmanaged<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr>)msgSend)(receiver, Selector(selector), arg1, arg2);

    public static IntPtr Send(IntPtr receiver, string selector, IntPtr arg1, IntPtr arg2, IntPtr arg3) =>
        ((delegate* unmanaged<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr>)msgSend)(receiver, Selector(selector), arg1, arg2, arg3);

    /// <summary>-[NSWindow initWithContentRect:styleMask:backing:defer:].</summary>
    public static IntPtr SendInitWithRect(IntPtr receiver, string selector, CoreGraphics.CGRect rect, nuint styleMask, nuint backing, bool defer) =>
        ((delegate* unmanaged<IntPtr, IntPtr, CoreGraphics.CGRect, nuint, nuint, byte, IntPtr>)msgSend)(receiver, Selector(selector), rect, styleMask, backing, defer ? (byte)1 : (byte)0);

    /// <summary>-[NSWindow setFrame:display:].</summary>
    public static void SendRectBool(IntPtr receiver, string selector, CoreGraphics.CGRect rect, bool flag) =>
        ((delegate* unmanaged<IntPtr, IntPtr, CoreGraphics.CGRect, byte, void>)msgSend)(receiver, Selector(selector), rect, flag ? (byte)1 : (byte)0);

    /// <summary>Methods taking one BOOL, such as setOpaque: or setIgnoresMouseEvents:.</summary>
    public static void SendBoolArg(IntPtr receiver, string selector, bool value) =>
        ((delegate* unmanaged<IntPtr, IntPtr, byte, void>)msgSend)(receiver, Selector(selector), value ? (byte)1 : (byte)0);

    public static bool SendBool(IntPtr receiver, string selector, IntPtr arg1, IntPtr arg2) =>
        ((delegate* unmanaged<IntPtr, IntPtr, IntPtr, IntPtr, byte>)msgSend)(receiver, Selector(selector), arg1, arg2) != 0;

    public static bool SendBool(IntPtr receiver, string selector, IntPtr arg1) =>
        ((delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte>)msgSend)(receiver, Selector(selector), arg1) != 0;

    public static bool SendBool(IntPtr receiver, string selector) =>
        ((delegate* unmanaged<IntPtr, IntPtr, byte>)msgSend)(receiver, Selector(selector)) != 0;

    public static nint SendNInt(IntPtr receiver, string selector) =>
        ((delegate* unmanaged<IntPtr, IntPtr, nint>)msgSend)(receiver, Selector(selector));

    /// <summary>Methods returning NSPoint or NSSize. Two doubles come back in registers on both arm64 and x86-64, so no _stret variant.</summary>
    public static CoreGraphics.CGPoint SendPoint(IntPtr receiver, string selector) =>
        ((delegate* unmanaged<IntPtr, IntPtr, CoreGraphics.CGPoint>)msgSend)(receiver, Selector(selector));

    /// <summary>Whether a class or object implements <paramref name="selector"/>, checked before calling a method that may be missing.</summary>
    public static bool RespondsTo(IntPtr receiver, string selector) => SendBool(receiver, "respondsToSelector:", Selector(selector));

    public static int SendInt32(IntPtr receiver, string selector) =>
        ((delegate* unmanaged<IntPtr, IntPtr, int>)msgSend)(receiver, Selector(selector));

    /// <summary>Runs <paramref name="action"/> inside an autorelease pool, which background threads do not have by default.</summary>
    public static T WithAutoreleasePool<T>(Func<T> action)
    {
        IntPtr pool = objc_autoreleasePoolPush();

        try
        {
            return action();
        }
        finally
        {
            objc_autoreleasePoolPop(pool);
        }
    }
}
