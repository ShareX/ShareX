using ShareX.Platform.MacOS.Native;
using System;
using System.Collections.Generic;

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
