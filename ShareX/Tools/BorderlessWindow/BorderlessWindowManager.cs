#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
using ShareX.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public class BorderlessWindowManager
    {
        public static bool MakeWindowBorderless(string windowTitle, bool useWorkingArea = false)
        {
            if (!string.IsNullOrEmpty(windowTitle))
            {
                IntPtr hWnd = SearchWindow(windowTitle);

                if (hWnd == IntPtr.Zero)
                {
                    MessageBox.Show(Resources.UnableToFindAWindowWithSpecifiedWindowTitle, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MakeWindowBorderless(hWnd, useWorkingArea);

                    return true;
                }
            }

            return false;
        }

        public static void MakeWindowBorderless(IntPtr hWnd, bool useWorkingArea = false)
        {
            WindowInfo windowInfo = new WindowInfo(hWnd);

            if (windowInfo.IsMinimized)
            {
                windowInfo.Restore();
            }

            WindowStyles windowStyle = windowInfo.Style;
            // https://docs.microsoft.com/en-us/windows/win32/winmsg/window-styles
            windowStyle &= ~(
                WindowStyles.WS_CAPTION // The window has a title bar (includes the WS_BORDER style).
                | WindowStyles.WS_MAXIMIZEBOX // The window has a maximize button.
                /*| WindowStyles.WS_MINIMIZEBOX // The window has a minimize button.*/
                | WindowStyles.WS_SYSMENU // The window has a window menu on its title bar.
                | WindowStyles.WS_THICKFRAME // The window has a sizing border. Same as the WS_SIZEBOX style.
                );
            windowInfo.Style = windowStyle;

            WindowStyles windowExStyle = windowInfo.ExStyle;
            // https://docs.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles
            windowExStyle &= ~(
                WindowStyles.WS_EX_CLIENTEDGE // The window has a border with a sunken edge.
                | WindowStyles.WS_EX_DLGMODALFRAME // The window has a double border.
                | WindowStyles.WS_EX_STATICEDGE // The window has a three-dimensional border style intended to be used for items that do not accept user input.
                );
            windowInfo.ExStyle = windowExStyle;

            Screen screen = Screen.FromHandle(hWnd);
            Rectangle rect;

            if (useWorkingArea)
            {
                rect = screen.WorkingArea;
            }
            else
            {
                rect = screen.Bounds;
            }

            // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
            SetWindowPosFlags setWindowPosFlag =
                SetWindowPosFlags.SWP_FRAMECHANGED // Applies new frame styles set using the SetWindowLong function.
                | SetWindowPosFlags.SWP_NOOWNERZORDER // Does not change the owner window's position in the Z order.
                | SetWindowPosFlags.SWP_NOZORDER // Retains the current Z order (ignores the hWndInsertAfter parameter).
                ;

            if (rect.IsEmpty)
            {
                setWindowPosFlag |=
                    SetWindowPosFlags.SWP_NOMOVE // Retains the current position (ignores X and Y parameters).
                    | SetWindowPosFlags.SWP_NOSIZE // Retains the current size (ignores the cx and cy parameters).
                    ;
            }

            windowInfo.SetWindowPos(rect, setWindowPosFlag);
        }

        private static IntPtr SearchWindow(string windowTitle)
        {
            IntPtr hWnd = NativeMethods.FindWindow(null, windowTitle);

            if (hWnd == IntPtr.Zero)
            {
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.MainWindowTitle.Contains(windowTitle, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return process.MainWindowHandle;
                    }
                }
            }

            return hWnd;
        }
    }
}