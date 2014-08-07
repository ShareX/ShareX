#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using HelpersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ScreenCaptureLib
{
    public class WindowsListAdvanced
    {
        public IntPtr IgnoreHandle { get; set; }
        public bool IncludeChildWindows { get; set; }

        private List<WindowInfo> windows;

        public List<WindowInfo> GetWindowsList()
        {
            windows = new List<WindowInfo>();
            NativeMethods.EnumWindowsProc ewp = EvalWindow;
            NativeMethods.EnumWindows(ewp, IntPtr.Zero);
            return windows;
        }

        public List<Rectangle> GetWindowsRectangleList()
        {
            List<Rectangle> result = new List<Rectangle>();

            foreach (Rectangle rect in GetWindowsList().Select(x => x.Rectangle))
            {
                bool rectVisible = true;

                foreach (Rectangle rect2 in result)
                {
                    if (rect2.Contains(rect))
                    {
                        rectVisible = false;
                        break;
                    }
                }

                if (rectVisible)
                {
                    result.Add(rect);
                }
            }

            return result;
        }

        private bool IsValidWindow(WindowInfo window)
        {
            return window.Handle != IgnoreHandle && window.Rectangle.IsValid() && window.IsVisible;
        }

        private bool EvalWindow(IntPtr hWnd, IntPtr lParam)
        {
            WindowInfo window = new WindowInfo(hWnd);

            if (!IsValidWindow(window))
            {
                return true;
            }

            if (IncludeChildWindows)
            {
                NativeMethods.EnumWindowsProc ewp = EvalWindow;
                NativeMethods.EnumChildWindows(hWnd, ewp, IntPtr.Zero);
            }

            windows.Add(window);

            return true;
        }
    }
}