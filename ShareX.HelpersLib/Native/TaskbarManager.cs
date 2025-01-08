#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public enum TaskbarProgressBarStatus
    {
        NoProgress = 0,
        Indeterminate = 0x1,
        Normal = 0x2,
        Error = 0x4,
        Paused = 0x8
    }

    public static class TaskbarManager
    {
        [ComImport, Guid("c43dc798-95d1-4bea-9030-bb99e2983a1a"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskbarList4
        {
            // ITaskbarList
            [PreserveSig]
            void HrInit();

            [PreserveSig]
            void AddTab(IntPtr hwnd);

            [PreserveSig]
            void DeleteTab(IntPtr hwnd);

            [PreserveSig]
            void ActivateTab(IntPtr hwnd);

            [PreserveSig]
            void SetActiveAlt(IntPtr hwnd);

            // ITaskbarList2
            [PreserveSig]
            void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            // ITaskbarList3
            [PreserveSig]
            void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);

            [PreserveSig]
            void SetProgressState(IntPtr hwnd, TaskbarProgressBarStatus tbpFlags);
        }

        [ComImport, Guid("56FDF344-FD6D-11d0-958A-006097C9A090"), ClassInterface(ClassInterfaceType.None)]
        private class CTaskbarList
        {
        }

        private static readonly object syncLock = new object();

        private static ITaskbarList4 taskbarList;

        private static ITaskbarList4 TaskbarList
        {
            get
            {
                if (taskbarList == null)
                {
                    lock (syncLock)
                    {
                        if (taskbarList == null)
                        {
                            taskbarList = (ITaskbarList4)new CTaskbarList();
                            taskbarList.HrInit();
                        }
                    }
                }

                return taskbarList;
            }
        }

        private static IntPtr mainWindowHandle;

        private static IntPtr MainWindowHandle
        {
            get
            {
                if (mainWindowHandle == IntPtr.Zero)
                {
                    Process currentProcess = Process.GetCurrentProcess();

                    if (currentProcess == null || currentProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        mainWindowHandle = IntPtr.Zero;
                    }
                    else
                    {
                        mainWindowHandle = currentProcess.MainWindowHandle;
                    }
                }

                return mainWindowHandle;
            }
        }

        public static bool Enabled { get; set; }

        public static bool IsPlatformSupported
        {
            get
            {
                return Helpers.IsWindows7OrGreater();
            }
        }

        private static void SetProgressValue(IntPtr hwnd, int currentValue, int maximumValue = 100)
        {
            if (Enabled && IsPlatformSupported && hwnd != IntPtr.Zero)
            {
                currentValue = currentValue.Clamp(0, maximumValue);

                try
                {
                    TaskbarList.SetProgressValue(hwnd, Convert.ToUInt32(currentValue), Convert.ToUInt32(maximumValue));
                }
                catch (FileNotFoundException)
                {
                    Enabled = false;
                }
            }
        }

        public static void SetProgressValue(int currentValue, int maximumValue = 100)
        {
            SetProgressValue(MainWindowHandle, currentValue, maximumValue);
        }

        public static void SetProgressValue(Form form, int currentValue, int maximumValue = 100)
        {
            form.InvokeSafe(() => SetProgressValue(form.Handle, currentValue, maximumValue));
        }

        private static void SetProgressState(IntPtr hwnd, TaskbarProgressBarStatus state)
        {
            if (Enabled && IsPlatformSupported && hwnd != IntPtr.Zero)
            {
                try
                {
                    TaskbarList.SetProgressState(hwnd, state);
                }
                catch (FileNotFoundException)
                {
                    Enabled = false;
                }
            }
        }

        public static void SetProgressState(TaskbarProgressBarStatus state)
        {
            SetProgressState(MainWindowHandle, state);
        }

        public static void SetProgressState(Form form, TaskbarProgressBarStatus state)
        {
            form.InvokeSafe(() => SetProgressState(form.Handle, state));
        }
    }
}