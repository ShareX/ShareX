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

/// <summary>
/// Uniform type identifiers and default applications (CoreServices). The UTType and LS role-handler functions are deprecated in
/// favour of Swift-only APIs but remain the C interface macOS provides.
/// </summary>
internal static partial class LaunchServices
{
    private const string CoreServices = "/System/Library/Frameworks/CoreServices.framework/CoreServices";

    public const uint kLSRolesAll = 0xFFFFFFFF;

    [LibraryImport(CoreServices)]
    private static partial IntPtr UTTypeCreatePreferredIdentifierForTag(IntPtr tagClass, IntPtr tag, IntPtr conformingToUti);

    [LibraryImport(CoreServices)]
    private static partial IntPtr UTTypeCopyPreferredTagWithClass(IntPtr uti, IntPtr tagClass);

    [LibraryImport(CoreServices)]
    private static partial IntPtr LSCopyDefaultRoleHandlerForContentType(IntPtr contentType, uint role);

    [LibraryImport(CoreServices)]
    private static partial int LSSetDefaultRoleHandlerForContentType(IntPtr contentType, uint role, IntPtr handlerBundleId);

    [LibraryImport(CoreServices)]
    private static partial int LSRegisterURL(IntPtr url, [MarshalAs(UnmanagedType.U1)] bool update);

    [LibraryImport(CoreFoundation.Library)]
    private static partial IntPtr CFBundleGetMainBundle();

    [LibraryImport(CoreFoundation.Library)]
    private static partial IntPtr CFBundleGetIdentifier(IntPtr bundle);

    [LibraryImport(CoreFoundation.Library)]
    private static partial IntPtr CFBundleCopyBundleURL(IntPtr bundle);

    /// <summary>The uniform type identifier for a file extension without its dot, such as "public.png" for "png".</summary>
    public static string? GetTypeForExtension(string extension) =>
        WithStrings("public.filename-extension", extension, (tagClass, tag) => TakeString(UTTypeCreatePreferredIdentifierForTag(tagClass, tag, IntPtr.Zero)));

    /// <summary>The MIME type macOS knows for a file extension without its dot, or null.</summary>
    public static string? GetMimeTypeForExtension(string extension)
    {
        string? uti = GetTypeForExtension(extension);

        // Unknown extensions get a dynamic identifier ("dyn.…") with no MIME type.
        if (uti == null || uti.StartsWith("dyn.", StringComparison.Ordinal))
        {
            return null;
        }

        return WithStrings(uti, "public.mime-type", (type, tagClass) => TakeString(UTTypeCopyPreferredTagWithClass(type, tagClass)));
    }

    /// <summary>The bundle identifier of the application that opens <paramref name="uti"/> by default, or null.</summary>
    public static string? GetDefaultHandler(string uti) =>
        WithStrings(uti, null, (type, _) => TakeString(LSCopyDefaultRoleHandlerForContentType(type, kLSRolesAll)));

    public static bool SetDefaultHandler(string uti, string bundleId) =>
        WithStrings(uti, bundleId, (type, handler) => LSSetDefaultRoleHandlerForContentType(type, kLSRolesAll, handler) == 0);

    /// <summary>The running application's bundle identifier; null when it is not started from an app bundle.</summary>
    public static string? MainBundleIdentifier
    {
        get
        {
            IntPtr bundle = CFBundleGetMainBundle();
            return bundle != IntPtr.Zero ? CoreFoundation.ToManagedString(CFBundleGetIdentifier(bundle)) : null;
        }
    }

    /// <summary>Registers the running app bundle with Launch Services, so the types its Info.plist declares are known wherever it was copied.</summary>
    public static void RegisterMainBundle()
    {
        IntPtr bundle = CFBundleGetMainBundle();
        IntPtr url = bundle != IntPtr.Zero ? CFBundleCopyBundleURL(bundle) : IntPtr.Zero;

        if (url != IntPtr.Zero)
        {
            LSRegisterURL(url, true);
            CoreFoundation.CFRelease(url);
        }
    }

    private static T WithStrings<T>(string first, string? second, Func<IntPtr, IntPtr, T> action)
    {
        IntPtr a = CoreFoundation.CreateString(first);
        IntPtr b = second != null ? CoreFoundation.CreateString(second) : IntPtr.Zero;

        try
        {
            return action(a, b);
        }
        finally
        {
            CoreFoundation.CFRelease(a);
            if (b != IntPtr.Zero) CoreFoundation.CFRelease(b);
        }
    }

    /// <summary>Reads and releases a CFString the caller owns (a Create or Copy result).</summary>
    private static string? TakeString(IntPtr value)
    {
        if (value == IntPtr.Zero)
        {
            return null;
        }

        try
        {
            return CoreFoundation.ToManagedString(value);
        }
        finally
        {
            CoreFoundation.CFRelease(value);
        }
    }
}
