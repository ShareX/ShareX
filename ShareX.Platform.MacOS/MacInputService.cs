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

using ShareX.Platform.MacOS.Native;
using System;
using System.Runtime.InteropServices;

namespace ShareX.Platform.MacOS;

/// <summary>Synthetic input through Quartz events, which macOS only delivers once ShareX has the Accessibility permission.</summary>
public sealed partial class MacInputService : IInputService
{
    private const string PermissionReason = "Allow ShareX in System Settings > Privacy & Security > Accessibility.";

    private static volatile bool permissionRequested;

    /// <summary>
    /// Supported until input was refused in this run: the first attempt shows macOS's Accessibility prompt, which a disabled option
    /// could never reach.
    /// </summary>
    public FeatureSupport KeyboardSupport => IsTrusted() || !permissionRequested ? FeatureSupport.Supported : FeatureSupport.NotSupported(PermissionReason);

    public FeatureSupport MouseWheelSupport => KeyboardSupport;

    public FeatureSupport WindowScrollSupport { get; } =
        FeatureSupport.NotSupported("Only Windows lets one application drive another application's scroll bars. Use the mouse wheel or a key instead.");

    public bool SendKeyPress(int virtualKey)
    {
        if (!MacKeyMap.TryGetKeyCode(virtualKey, out uint keyCode))
        {
            return false;
        }

        return Post(CoreGraphics.CGEventCreateKeyboardEvent(IntPtr.Zero, (ushort)keyCode, true)) &&
            Post(CoreGraphics.CGEventCreateKeyboardEvent(IntPtr.Zero, (ushort)keyCode, false));
    }

    public bool SendMouseWheel(int detents) =>
        detents != 0 && Post(CoreGraphics.CGEventCreateScrollWheelEvent2(IntPtr.Zero, CoreGraphics.kCGScrollEventUnitLine, 1, detents, 0, 0));

    public bool ScrollWindow(long windowHandle, WindowScrollCommand command) => false;

    // Reads pointer and button state, which macOS allows without a permission.
    public FeatureSupport MouseHookSupport => FeatureSupport.Supported;

    public IDisposable HookMouse(IGlobalMouseListener listener) => new MacMouseHook(listener);

    private static bool Post(IntPtr evt)
    {
        if (evt == IntPtr.Zero)
        {
            return false;
        }

        // Quartz drops synthetic events silently without the permission, so ask for it instead of pretending to succeed.
        if (!IsTrusted())
        {
            CoreFoundation.CFRelease(evt);
            RequestTrust();
            return false;
        }

        try
        {
            CoreGraphics.CGEventPost(CoreGraphics.kCGHIDEventTap, evt);
            return true;
        }
        finally
        {
            CoreFoundation.CFRelease(evt);
        }
    }

    private static bool IsTrusted()
    {
        try
        {
            return AXIsProcessTrusted();
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }
    }

    /// <summary>Shows macOS's Accessibility prompt, once per run.</summary>
    private static void RequestTrust()
    {
        if (permissionRequested)
        {
            return;
        }

        permissionRequested = true;

        try
        {
            ObjC.WithAutoreleasePool(() =>
            {
                // kAXTrustedCheckOptionPrompt: true
                IntPtr key = CoreFoundation.CreateString("AXTrustedCheckOptionPrompt");
                IntPtr value = ObjC.Send(ObjC.GetClass("NSNumber"), "numberWithBool:", 1);
                IntPtr options = ObjC.Send(ObjC.GetClass("NSDictionary"), "dictionaryWithObject:forKey:", value, key);
                CoreFoundation.CFRelease(key);
                return AXIsProcessTrustedWithOptions(options);
            });
        }
        catch (EntryPointNotFoundException)
        {
        }
    }

    [LibraryImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool AXIsProcessTrusted();

    [LibraryImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool AXIsProcessTrustedWithOptions(IntPtr options);
}
