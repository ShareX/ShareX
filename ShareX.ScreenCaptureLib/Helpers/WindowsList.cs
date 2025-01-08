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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.ScreenCaptureLib
{
    public class WindowsList
    {
        public List<IntPtr> IgnoreWindows { get; set; }

        private string[] ignoreList = new string[] { "Progman", "Button" };
        private List<WindowInfo> windows;

        public WindowsList()
        {
            IgnoreWindows = new List<IntPtr>();
        }

        public WindowsList(IntPtr ignoreWindow) : this()
        {
            IgnoreWindows.Add(ignoreWindow);
        }

        public List<WindowInfo> GetWindowsList()
        {
            windows = new List<WindowInfo>();
            EnumWindowsProc ewp = EvalWindows;
            NativeMethods.EnumWindows(ewp, IntPtr.Zero);
            return windows;
        }

        public List<WindowInfo> GetVisibleWindowsList()
        {
            List<WindowInfo> windows = GetWindowsList();

            return windows.Where(IsValidWindow).ToList();
        }

        private bool IsValidWindow(WindowInfo window)
        {
            return window != null && window.IsVisible && !string.IsNullOrEmpty(window.Text) && IsClassNameAllowed(window) && window.Rectangle.IsValid();
        }

        private bool IsClassNameAllowed(WindowInfo window)
        {
            string className = window.ClassName;

            if (!string.IsNullOrEmpty(className))
            {
                return ignoreList.All(ignore => !className.Equals(ignore, StringComparison.OrdinalIgnoreCase));
            }

            return true;
        }

        private bool EvalWindows(IntPtr hWnd, IntPtr lParam)
        {
            if (IgnoreWindows.Any(window => hWnd == window))
            {
                return true;
            }

            windows.Add(new WindowInfo(hWnd));

            return true;
        }
    }
}