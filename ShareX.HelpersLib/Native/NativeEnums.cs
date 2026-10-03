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

namespace ShareX.HelpersLib
{
    public enum SystemMetric
    {
        SM_CXSMICON = 49,
        SM_CYSMICON = 50
    }

    public enum WindowsMessages : uint
    {
        CLOSE = 0x0010,
        QUERYENDSESSION = 0x0011,
        ENDSESSION = 0x0016
    }

    [Flags]
    public enum WindowStyles : uint
    {
        WS_POPUP = 0x80000000,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_NOACTIVATE = 0x08000000
    }

    [Flags]
    public enum Modifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }

    [Flags]
    public enum CreateProcessFlags : uint
    {
        DEBUG_PROCESS = 0x00000001,
        DEBUG_ONLY_THIS_PROCESS = 0x00000002,
        CREATE_SUSPENDED = 0x00000004,
        DETACHED_PROCESS = 0x00000008,
        CREATE_NEW_CONSOLE = 0x00000010,
        NORMAL_PRIORITY_CLASS = 0x00000020,
        IDLE_PRIORITY_CLASS = 0x00000040,
        HIGH_PRIORITY_CLASS = 0x00000080,
        REALTIME_PRIORITY_CLASS = 0x00000100,
        CREATE_NEW_PROCESS_GROUP = 0x00000200,
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,
        CREATE_SEPARATE_WOW_VDM = 0x00000800,
        CREATE_SHARED_WOW_VDM = 0x00001000,
        CREATE_FORCEDOS = 0x00002000,
        BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
        ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
        INHERIT_PARENT_AFFINITY = 0x00010000,
        INHERIT_CALLER_PRIORITY = 0x00020000,
        CREATE_PROTECTED_PROCESS = 0x00040000,
        EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
        PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000,
        PROCESS_MODE_BACKGROUND_END = 0x00200000,
        CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
        CREATE_DEFAULT_ERROR_MODE = 0x04000000,
        CREATE_NO_WINDOW = 0x08000000,
        PROFILE_USER = 0x10000000,
        PROFILE_KERNEL = 0x20000000,
        PROFILE_SERVER = 0x40000000,
        CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000,
    }

    [Flags]
    public enum RegisterApplicationRestartFlags : uint
    {
        /// <summary>
        /// Do not restart the process if it terminates due to an unhandled exception.
        /// </summary>
        RESTART_NO_CRASH = 1,
        /// <summary>
        /// Do not restart the process if it terminates due to the application not responding.
        /// </summary>
        RESTART_NO_HANG = 2,
        /// <summary>
        /// Do not restart the process if it terminates due to the installation of an update.
        /// </summary>
        RESTART_NO_PATCH = 4,
        /// <summary>
        /// Do not restart the process if the computer is restarted as the result of an update.
        /// </summary>
        RESTART_NO_REBOOT = 8
    }

    [Flags]
    public enum EndSessionReasons : uint
    {
        /// <summary>
        /// The application is using a file that must be replaced, the system is being serviced, or system resources are exhausted.
        /// </summary>
        ENDSESSION_CLOSEAPP = 0x1,
        /// <summary>
        /// The application is forced to shut down.
        /// </summary>
        ENDSESSION_CRITICAL = 0x40000000,
        /// <summary>
        /// The user is logging off.
        /// </summary>
        ENDSESSION_LOGOFF = 0x80000000
    }
}
