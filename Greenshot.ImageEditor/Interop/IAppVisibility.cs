/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;

namespace Greenshot.Interop
{
    // This is used for Windows 8 to see if the App Launcher is active
    // See http://msdn.microsoft.com/en-us/library/windows/desktop/jj554119%28v=vs.85%29.aspx
    [ComProgId("clsid:7E5FE3D9-985F-4908-91F9-EE19F9FD1514")]
    [ComImport, Guid("2246EA2D-CAEA-4444-A3C4-6DE827E44313"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAppVisibility
    {
        MONITOR_APP_VISIBILITY GetAppVisibilityOnMonitor(IntPtr hMonitor);

        bool IsLauncherVisible
        {
            get;
        }
    }

    public enum MONITOR_APP_VISIBILITY
    {
        MAV_UNKNOWN = 0,		// The mode for the monitor is unknown
        MAV_NO_APP_VISIBLE = 1,
        MAV_APP_VISIBLE = 2
    }
}