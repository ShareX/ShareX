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
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ShareX.Platform.MacOS;

/// <summary>Window enumeration with CGWindowListCopyWindowInfo. Window titles need the Screen Recording permission.</summary>
public sealed class MacWindowService : IWindowService
{
    public FeatureSupport Support => FeatureSupport.Supported;

    public IReadOnlyList<PlatformWindow> GetWindows()
    {
        IntPtr list = CoreGraphics.CGWindowListCopyWindowInfo(
            CoreGraphics.kCGWindowListOptionOnScreenOnly | CoreGraphics.kCGWindowListExcludeDesktopElements, 0);

        if (list == IntPtr.Zero)
        {
            return Array.Empty<PlatformWindow>();
        }

        try
        {
            nint count = CoreFoundation.CFArrayGetCount(list);
            List<PlatformWindow> windows = new List<PlatformWindow>((int)count);

            // The list is ordered front to back.
            for (nint i = 0; i < count; i++)
            {
                IntPtr info = CoreFoundation.CFArrayGetValueAtIndex(list, i);

                // Layer 0 holds normal application windows. Menu bar, Dock and overlays use other layers.
                if (CoreFoundation.GetInt64(info, "kCGWindowLayer") != 0)
                {
                    continue;
                }

                IntPtr boundsDictionary = CoreFoundation.GetValue(info, "kCGWindowBounds");

                if (boundsDictionary == IntPtr.Zero || !CoreGraphics.CGRectMakeWithDictionaryRepresentation(boundsDictionary, out CoreGraphics.CGRect rect))
                {
                    continue;
                }

                PlatformRectangle bounds = rect.ToRectangle();

                if (bounds.IsEmpty)
                {
                    continue;
                }

                long number = CoreFoundation.GetInt64(info, "kCGWindowNumber") ?? 0;
                long? pid = CoreFoundation.GetInt64(info, "kCGWindowOwnerPID");
                string? owner = CoreFoundation.GetString(info, "kCGWindowOwnerName");
                string title = CoreFoundation.GetString(info, "kCGWindowName") ?? "";

                windows.Add(new PlatformWindow(number, title, owner, pid != null ? (int)pid.Value : null, bounds, false));
            }

            return windows;
        }
        finally
        {
            CoreFoundation.CFRelease(list);
        }
    }

    // macOS has no application-held mouse capture, and only the frontmost app's own windows can restrict the pointer.
    public void ReleaseMouseCapture()
    {
    }

    public bool ConfineCursor(long windowHandle) => false;

    public void ReleaseCursorConfinement()
    {
    }

    /// <summary>A null event's location is the current pointer position, in global display coordinates with the origin top left.</summary>
    public PlatformPoint? GetCursorPosition()
    {
        IntPtr evt = CoreGraphics.CGEventCreate(IntPtr.Zero);

        if (evt == IntPtr.Zero)
        {
            return null;
        }

        try
        {
            CoreGraphics.CGPoint location = CoreGraphics.CGEventGetLocation(evt);
            return new PlatformPoint((int)Math.Round(location.X), (int)Math.Round(location.Y));
        }
        finally
        {
            CoreFoundation.CFRelease(evt);
        }
    }

    public IReadOnlyList<SnapTarget> GetSnapTargets(bool includeControls, long ignoredHandle, CancellationToken cancellationToken = default)
    {
        // The handle Avalonia reports is an NSWindow pointer, not a CGWindowID, so leave out every window of this process.
        int ownProcess = Environment.ProcessId;
        return SnapTarget.FromWindows(GetWindows().Where(window => window.ProcessId != ownProcess), ignoredHandle);
    }

    public bool SetCursorPosition(PlatformPoint position) =>
        CoreGraphics.CGWarpMouseCursorPosition(new CoreGraphics.CGPoint { X = position.X, Y = position.Y }) == 0;

    public bool RestoreWindow(long windowHandle) => ActivateWindow(windowHandle);

    /// <summary>macOS activates applications, not windows: this brings the window's application to the front.</summary>
    public bool ActivateWindow(long windowHandle)
    {
        int? processId = GetWindows().FirstOrDefault(window => window.Handle == windowHandle)?.ProcessId;

        if (processId == null)
        {
            return false;
        }

        return ObjC.WithAutoreleasePool(() =>
        {
            IntPtr application = ObjC.Send(ObjC.GetClass("NSRunningApplication"), "runningApplicationWithProcessIdentifier:", processId.Value);
            // NSApplicationActivateIgnoringOtherApps
            return application != IntPtr.Zero && ObjC.SendBool(application, "activateWithOptions:", 2);
        });
    }

    // Avalonia's ShowInTaskbar and transparent windows cover this on macOS; AppKit has no input shape for a window.
    public bool SetOverlayStyle(long windowHandle, bool clickThrough) => false;

    public bool SetWindowShape(long windowHandle, IReadOnlyList<PlatformRectangle> visibleAreas) => false;

    // Overlays need an always on top, click through window with per pixel alpha placed at exact desktop coordinates, which
    // Wayland compositors do not allow and which is not implemented for X11 and macOS yet.
    public FeatureSupport OverlaySupport { get; } = FeatureSupport.NotSupported("Drawing over other applications is not available on this platform yet.");

    public IScreenOverlay CreateOverlay(PlatformRectangle screenBounds) => throw new PlatformNotSupportedException(OverlaySupport.Reason);

    public long GetActiveWindowHandle() => GetActiveWindow()?.Handle ?? 0;

    public PlatformRectangle? GetWindowBounds(long windowHandle) =>
        GetWindows().FirstOrDefault(window => window.Handle == windowHandle)?.Bounds;

    // CGWindowList reports whole windows only; macOS draws the title bar inside them.
    public PlatformRectangle? GetClientBounds(long windowHandle) => GetWindowBounds(windowHandle);

    public PlatformWindow? GetActiveWindow()
    {
        int frontmostPid = ObjC.WithAutoreleasePool(() =>
        {
            IntPtr workspace = ObjC.Send(ObjC.GetClass("NSWorkspace"), "sharedWorkspace");
            IntPtr application = ObjC.Send(workspace, "frontmostApplication");
            return application != IntPtr.Zero ? ObjC.SendInt32(application, "processIdentifier") : -1;
        });

        foreach (PlatformWindow window in GetWindows())
        {
            if (window.ProcessId == frontmostPid)
            {
                return window;
            }
        }

        return null;
    }
}
