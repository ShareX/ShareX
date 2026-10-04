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
using System.Runtime.Versioning;

namespace ShareX.Platform.MacOS;

/// <summary>
/// Upload progress as a percentage badge on ShareX's Dock icon. AppKit must be used on the main thread, so the badge is set with
/// performSelectorOnMainThread, which is safe from any thread.
/// </summary>
[SupportedOSPlatform("macos")]
public sealed class DockProgressService : ITaskbarService
{
    private TaskbarProgressState state;

    public FeatureSupport Support => FeatureSupport.Supported;

    public void SetProgressValue(int value, int maximum)
    {
        if (state is TaskbarProgressState.None or TaskbarProgressState.Indeterminate || maximum <= 0)
        {
            return;
        }

        int percent = (int)Math.Round(100.0 * Math.Clamp(value, 0, maximum) / maximum);
        SetBadge(percent + "%");
    }

    public void SetProgressState(TaskbarProgressState state)
    {
        this.state = state;

        switch (state)
        {
            case TaskbarProgressState.None:
                SetBadge(null);
                break;
            case TaskbarProgressState.Indeterminate:
                SetBadge("…");
                break;
            case TaskbarProgressState.Error:
                SetBadge("!");
                break;
        }
    }

    private static void SetBadge(string? text)
    {
        ObjC.WithAutoreleasePool(() =>
        {
            IntPtr application = ObjC.Send(ObjC.GetClass("NSApplication"), "sharedApplication");
            IntPtr dockTile = ObjC.Send(application, "dockTile");
            IntPtr label = text != null ? CoreFoundation.CreateString(text) : IntPtr.Zero;

            try
            {
                ObjC.Send(dockTile, "performSelectorOnMainThread:withObject:waitUntilDone:", ObjC.Selector("setBadgeLabel:"), label, IntPtr.Zero);
            }
            finally
            {
                // performSelectorOnMainThread retains its argument until the call has run.
                if (label != IntPtr.Zero) CoreFoundation.CFRelease(label);
            }

            return true;
        });
    }
}
