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

namespace ShareX.HelpersLib
{
    public static class DesktopIconManager
    {
        public static bool AreDesktopIconsVisible()
        {
            IntPtr hIcons = GetDesktopListViewHandle();

            return hIcons != IntPtr.Zero && NativeMethods.IsWindowVisible(hIcons);
        }

        public static bool SetDesktopIconsVisibility(bool show)
        {
            IntPtr hIcons = GetDesktopListViewHandle();

            if (hIcons != IntPtr.Zero)
            {
                NativeMethods.ShowWindow(hIcons, show ? (int)WindowShowStyle.Show : (int)WindowShowStyle.Hide);

                return true;
            }

            return false;
        }

        private static IntPtr GetDesktopListViewHandle()
        {
            IntPtr progman = NativeMethods.FindWindow("Progman", null);
            IntPtr desktopWnd = IntPtr.Zero;

            IntPtr defView = NativeMethods.FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);

            if (defView == IntPtr.Zero)
            {
                IntPtr desktopHandle = IntPtr.Zero;

                while ((desktopHandle = NativeMethods.FindWindowEx(IntPtr.Zero, desktopHandle, "WorkerW", null)) != IntPtr.Zero)
                {
                    defView = NativeMethods.FindWindowEx(desktopHandle, IntPtr.Zero, "SHELLDLL_DefView", null);

                    if (defView != IntPtr.Zero)
                    {
                        break;
                    }
                }
            }

            if (defView != IntPtr.Zero)
            {
                IntPtr sysListView = NativeMethods.FindWindowEx(defView, IntPtr.Zero, "SysListView32", "FolderView");
                return sysListView;
            }

            return IntPtr.Zero;
        }
    }
}