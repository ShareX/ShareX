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
using System.Linq;
using System.Runtime.Versioning;

namespace ShareX.Platform.MacOS;

/// <summary>
/// Window inspection on macOS from the window list (CGWindowList) and the owning application (NSRunningApplication): the window
/// under the pointer, its title, owner, executable, bounds and the application icon. macOS does not let one application change
/// another's window level, opacity or frame, so those operations report a reason.
/// </summary>
[SupportedOSPlatform("macos")]
public sealed class MacWindowManagementService(IWindowService windows) : IWindowManagementService
{
    private const string OtherWindowsReason = "macOS does not let applications change other applications' windows.";

    public FeatureSupport Support => windows.Support;

    public FeatureSupport BorderlessSupport { get; } = FeatureSupport.NotSupported(OtherWindowsReason);

    public FeatureSupport GetSupport(WindowManagementFeature feature) => feature switch
    {
        WindowManagementFeature.Inspect => Support,
        WindowManagementFeature.ChildControls => FeatureSupport.NotSupported("macOS reports whole windows only, not the controls inside them."),
        _ => FeatureSupport.NotSupported(OtherWindowsReason)
    };

    /// <summary>The frontmost window containing the point (the window list is in front-to-back order); controls are not available.</summary>
    public long GetWindowAt(PlatformPoint point, bool topLevel) =>
        windows.GetWindows().FirstOrDefault(window => !window.IsMinimized && window.Bounds.Contains(point))?.Handle ?? 0;

    public WindowDetails? GetDetails(long windowHandle)
    {
        PlatformWindow? window = windows.GetWindows().FirstOrDefault(w => w.Handle == windowHandle);

        if (window == null)
        {
            return null;
        }

        string? path = window.ProcessId is int pid ? GetExecutablePath(pid) : null;
        // Normal application windows sit on layer 0; the list ShareX uses holds only those.
        return new WindowDetails(window.Handle, window.Title, null, window.ProcessName, path, window.ProcessId, window.Bounds, null,
            [], [], false, null);
    }

    public byte[]? GetIcon(long windowHandle)
    {
        PlatformWindow? window = windows.GetWindows().FirstOrDefault(w => w.Handle == windowHandle);
        return window?.ProcessId is int pid ? GetApplicationIconPng(pid) : null;
    }

    public bool SetTopMost(long windowHandle, bool topMost) => false;

    public bool SetOpacity(long windowHandle, byte opacity) => false;

    public bool ToggleBorderless(long windowHandle, bool useWorkingArea) => false;

    private static string? GetExecutablePath(int pid) => ObjC.WithAutoreleasePool(() =>
    {
        IntPtr application = ObjC.Send(ObjC.GetClass("NSRunningApplication"), "runningApplicationWithProcessIdentifier:", pid);
        IntPtr url = application != IntPtr.Zero ? ObjC.Send(application, "executableURL") : IntPtr.Zero;
        return url != IntPtr.Zero ? CoreFoundation.ToManagedString(ObjC.Send(url, "path")) : null;
    });

    /// <summary>The application's icon as PNG: NSImage, through its TIFF form, re-encoded by NSBitmapImageRep.</summary>
    private static byte[]? GetApplicationIconPng(int pid) => ObjC.WithAutoreleasePool<byte[]?>(() =>
    {
        IntPtr application = ObjC.Send(ObjC.GetClass("NSRunningApplication"), "runningApplicationWithProcessIdentifier:", pid);
        IntPtr icon = application != IntPtr.Zero ? ObjC.Send(application, "icon") : IntPtr.Zero;
        IntPtr tiff = icon != IntPtr.Zero ? ObjC.Send(icon, "TIFFRepresentation") : IntPtr.Zero;
        IntPtr representation = tiff != IntPtr.Zero ? ObjC.Send(ObjC.GetClass("NSBitmapImageRep"), "imageRepWithData:", tiff) : IntPtr.Zero;

        if (representation == IntPtr.Zero)
        {
            return null;
        }

        IntPtr properties = ObjC.Send(ObjC.GetClass("NSDictionary"), "dictionary");
        // NSBitmapImageFileTypePNG
        IntPtr png = ObjC.Send(representation, "representationUsingType:properties:", (IntPtr)4, properties);
        return png != IntPtr.Zero ? CoreFoundation.ToArray(png) : null;
    });
}
