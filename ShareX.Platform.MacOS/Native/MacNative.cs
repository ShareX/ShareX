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

internal static unsafe partial class CoreGraphics
{
    private const string Library = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";

    public const uint kCGWindowListOptionOnScreenOnly = 1 << 0;
    public const uint kCGWindowListExcludeDesktopElements = 1 << 4;

    [StructLayout(LayoutKind.Sequential)]
    public struct CGRect
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;

        public PlatformRectangle ToRectangle() =>
            new PlatformRectangle((int)Math.Round(X), (int)Math.Round(Y), (int)Math.Round(Width), (int)Math.Round(Height));
    }

    [LibraryImport(Library)]
    public static partial int CGGetActiveDisplayList(uint maxDisplays, uint* displays, out uint displayCount);

    [StructLayout(LayoutKind.Sequential)]
    public struct CGPoint
    {
        public double X;
        public double Y;
    }

    [LibraryImport(Library)]
    public static partial IntPtr CGEventCreate(IntPtr source);

    [LibraryImport(Library)]
    public static partial CGPoint CGEventGetLocation(IntPtr evt);

    [LibraryImport(Library)]
    public static partial uint CGMainDisplayID();

    [LibraryImport(Library)]
    public static partial CGRect CGDisplayBounds(uint display);

    [LibraryImport(Library)]
    public static partial nuint CGDisplayPixelsWide(uint display);

    [LibraryImport(Library)]
    public static partial IntPtr CGWindowListCopyWindowInfo(uint option, uint relativeToWindow);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool CGRectMakeWithDictionaryRepresentation(IntPtr dictionary, out CGRect rect);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool CGPreflightScreenCaptureAccess();

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool CGRequestScreenCaptureAccess();
}

/// <summary>Carbon hot keys. Still the supported way to register global shortcuts without the Accessibility permission.</summary>
internal static unsafe partial class Carbon
{
    private const string Library = "/System/Library/Frameworks/Carbon.framework/Carbon";

    public const uint kEventClassKeyboard = 0x6B657962; // 'keyb'
    public const uint kEventHotKeyPressed = 5;
    public const uint kEventParamDirectObject = 0x2D2D2D2D; // '----'
    public const uint typeEventHotKeyID = 0x686B6964; // 'hkid'
    public const int eventHotKeyExistsErr = -9878;
    public const int eventHotKeyInvalidErr = -9879;

    public const uint cmdKey = 1 << 8;
    public const uint shiftKey = 1 << 9;
    public const uint optionKey = 1 << 11;
    public const uint controlKey = 1 << 12;

    [StructLayout(LayoutKind.Sequential)]
    public struct EventHotKeyID
    {
        public uint signature;
        public uint id;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventTypeSpec
    {
        public uint eventClass;
        public uint eventKind;
    }

    [LibraryImport(Library)]
    public static partial IntPtr GetApplicationEventTarget();

    [LibraryImport(Library)]
    public static partial int InstallEventHandler(IntPtr target, IntPtr handler, nuint numTypes, EventTypeSpec* list, IntPtr userData, out IntPtr handlerRef);

    [LibraryImport(Library)]
    public static partial int RemoveEventHandler(IntPtr handlerRef);

    [LibraryImport(Library)]
    public static partial int RegisterEventHotKey(uint keyCode, uint modifiers, EventHotKeyID hotKeyId, IntPtr target, uint options, out IntPtr hotKeyRef);

    [LibraryImport(Library)]
    public static partial int UnregisterEventHotKey(IntPtr hotKeyRef);

    [LibraryImport(Library)]
    public static partial int GetEventParameter(IntPtr eventRef, uint name, uint desiredType, IntPtr actualType, nuint bufferSize, IntPtr actualSize, void* data);
}

/// <summary>Legacy keychain API. Deprecated but still supported and, unlike SecItem, usable without building CFDictionaries.</summary>
internal static unsafe partial class Security
{
    private const string Library = "/System/Library/Frameworks/Security.framework/Security";

    public const int errSecSuccess = 0;
    public const int errSecDuplicateItem = -25299;
    public const int errSecItemNotFound = -25300;

    [LibraryImport(Library)]
    public static partial int SecKeychainAddGenericPassword(IntPtr keychain, uint serviceNameLength, byte* serviceName, uint accountNameLength,
        byte* accountName, uint passwordLength, byte* passwordData, out IntPtr itemRef);

    [LibraryImport(Library)]
    public static partial int SecKeychainFindGenericPassword(IntPtr keychainOrArray, uint serviceNameLength, byte* serviceName,
        uint accountNameLength, byte* accountName, out uint passwordLength, out IntPtr passwordData, out IntPtr itemRef);

    [LibraryImport(Library)]
    public static partial int SecKeychainItemModifyAttributesAndData(IntPtr itemRef, IntPtr attrList, uint length, byte* data);

    [LibraryImport(Library)]
    public static partial int SecKeychainItemFreeContent(IntPtr attrList, IntPtr data);

    [LibraryImport(Library)]
    public static partial int SecKeychainItemDelete(IntPtr itemRef);
}
