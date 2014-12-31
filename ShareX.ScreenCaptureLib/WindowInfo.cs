#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
using System.Diagnostics;
using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    public class WindowInfo
    {
        public IntPtr Handle { get; private set; }

        public string Text
        {
            get
            {
                return NativeMethods.GetWindowText(Handle);
            }
        }

        public string ClassName
        {
            get
            {
                return NativeMethods.GetClassName(Handle);
            }
        }

        public Process Process
        {
            get
            {
                return NativeMethods.GetProcessByWindowHandle(Handle);
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return CaptureHelpers.GetWindowRectangle(Handle);
            }
        }

        public Rectangle Rectangle0Based
        {
            get
            {
                return CaptureHelpers.ScreenToClient(Rectangle);
            }
        }

        public Rectangle ClientRectangle
        {
            get
            {
                return NativeMethods.GetClientRect(Handle);
            }
        }

        public WindowStyles Styles
        {
            get
            {
                return (WindowStyles)NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_STYLE);
            }
        }

        public bool IsMaximized
        {
            get
            {
                return NativeMethods.IsZoomed(Handle);
            }
        }

        public bool IsMinimized
        {
            get
            {
                return NativeMethods.IsIconic(Handle);
            }
        }

        public bool IsVisible
        {
            get
            {
                return NativeMethods.IsWindowVisible(Handle);
            }
        }

        public Icon Icon
        {
            get
            {
                return NativeMethods.GetApplicationIcon(Handle);
            }
        }

        public WindowInfo(IntPtr handle)
        {
            Handle = handle;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}