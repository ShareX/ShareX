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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ShareX.Platform.Windows;

/// <summary>Taskbar button progress with ITaskbarList3, as ShareX's TaskbarManager has always done.</summary>
public sealed class WindowsTaskbarService : ITaskbarService
{
    // Values match TBPFLAG.
    private enum TaskbarProgressFlags
    {
        NoProgress = 0,
        Indeterminate = 0x1,
        Normal = 0x2,
        Error = 0x4,
        Paused = 0x8
    }

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
        void SetProgressState(IntPtr hwnd, TaskbarProgressFlags tbpFlags);
    }

    [ComImport, Guid("56FDF344-FD6D-11d0-958A-006097C9A090"), ClassInterface(ClassInterfaceType.None)]
    private class CTaskbarList
    {
    }

    private readonly object syncLock = new object();
    private ITaskbarList4? taskbarList;
    private IntPtr mainWindowHandle;
    private bool available = true;

    public FeatureSupport Support => available ? FeatureSupport.Supported : FeatureSupport.NotSupported("The taskbar is not available.");

    public void SetProgressValue(int value, int maximum)
    {
        IntPtr hwnd = MainWindowHandle;

        if (hwnd != IntPtr.Zero && maximum > 0)
        {
            ulong completed = (ulong)Math.Clamp(value, 0, maximum);
            Invoke(taskbar => taskbar.SetProgressValue(hwnd, completed, (ulong)maximum));
        }
    }

    public void SetProgressState(TaskbarProgressState state)
    {
        IntPtr hwnd = MainWindowHandle;

        if (hwnd != IntPtr.Zero)
        {
            TaskbarProgressFlags flags = state switch
            {
                TaskbarProgressState.Indeterminate => TaskbarProgressFlags.Indeterminate,
                TaskbarProgressState.Normal => TaskbarProgressFlags.Normal,
                TaskbarProgressState.Error => TaskbarProgressFlags.Error,
                TaskbarProgressState.Paused => TaskbarProgressFlags.Paused,
                _ => TaskbarProgressFlags.NoProgress
            };

            Invoke(taskbar => taskbar.SetProgressState(hwnd, flags));
        }
    }

    private IntPtr MainWindowHandle
    {
        get
        {
            if (mainWindowHandle == IntPtr.Zero)
            {
                using Process currentProcess = Process.GetCurrentProcess();
                mainWindowHandle = currentProcess.MainWindowHandle;
            }

            return mainWindowHandle;
        }
    }

    private ITaskbarList4 TaskbarList
    {
        get
        {
            if (taskbarList == null)
            {
                taskbarList = (ITaskbarList4)new CTaskbarList();
                taskbarList.HrInit();
            }

            return taskbarList;
        }
    }

    private void Invoke(Action<ITaskbarList4> action)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        lock (syncLock)
        {
            if (!available)
            {
                return;
            }

            try
            {
                action(TaskbarList);
            }
            catch (InvalidComObjectException)
            {
                // Upload progress can be reported by different COM apartments.
                // Recreate a cached RCW whose creating apartment was torn down.
                taskbarList = null;

                try
                {
                    action(TaskbarList);
                }
                catch (InvalidComObjectException)
                {
                    taskbarList = null;
                }
                catch (FileNotFoundException)
                {
                    available = false;
                }
            }
            catch (FileNotFoundException)
            {
                available = false;
            }
        }
    }
}
