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
using System.Drawing;

namespace ShareX.HelpersLib
{
    public class WindowInfo
    {
        public IntPtr Handle { get; }

        private string _linuxText;
        private Rectangle _linuxRectangle;

        public bool IsHandleCreated => Handle != IntPtr.Zero;

        public string Text => !OperatingSystem.IsWindows() ? (_linuxText ?? "") : NativeMethods.GetWindowText(Handle);

        public string ClassName => !OperatingSystem.IsWindows() ? "LinuxWindow" : NativeMethods.GetClassName(Handle);

        public Process Process => !OperatingSystem.IsWindows() ? null : NativeMethods.GetProcessByWindowHandle(Handle);

        public string ProcessName
        {
            get
            {
                if (!OperatingSystem.IsWindows()) return "LinuxApp";
                using (Process process = Process)
                {
                    return process?.ProcessName;
                }
            }
        }

        public string ProcessFilePath
        {
            get
            {
                if (!OperatingSystem.IsWindows()) return "";
                using (Process process = Process)
                {
                    return process?.MainModule?.FileName;
                }
            }
        }

        public string ProcessFileName => FileHelpers.GetFileNameSafe(ProcessFilePath);

        public int ProcessId
        {
            get
            {
                if (!OperatingSystem.IsWindows()) return 0;
                using (Process process = Process)
                {
                    return process != null ? process.Id : 0;
                }
            }
        }

        public IntPtr Parent => !OperatingSystem.IsWindows() ? IntPtr.Zero : NativeMethods.GetParent(Handle);

        public Rectangle Rectangle => !OperatingSystem.IsWindows() ? _linuxRectangle : CaptureHelpers.GetWindowRectangle(Handle);

        public Rectangle ClientRectangle => !OperatingSystem.IsWindows() ? _linuxRectangle : NativeMethods.GetClientRect(Handle);

        public WindowStyles Style
        {
            get
            {
                return (WindowStyles)(ulong)NativeMethods.GetWindowLong(Handle, NativeConstants.GWL_STYLE);
            }
            set
            {
                NativeMethods.SetWindowLong(Handle, NativeConstants.GWL_STYLE, (IntPtr)value);
            }
        }

        public WindowStyles ExStyle
        {
            get
            {
                if (!OperatingSystem.IsWindows()) return (WindowStyles)0;
                return (WindowStyles)(ulong)NativeMethods.GetWindowLong(Handle, NativeConstants.GWL_EXSTYLE);
            }
            set
            {
                if (!OperatingSystem.IsWindows()) return;
                NativeMethods.SetWindowLong(Handle, NativeConstants.GWL_EXSTYLE, (IntPtr)value);
            }
        }

        public bool Layered
        {
            get
            {
                return ExStyle.HasFlag(WindowStyles.WS_EX_LAYERED);
            }
            set
            {
                if (value)
                {
                    ExStyle |= WindowStyles.WS_EX_LAYERED;
                }
                else
                {
                    ExStyle &= ~WindowStyles.WS_EX_LAYERED;
                }
            }
        }

        public bool TopMost
        {
            get
            {
                return ExStyle.HasFlag(WindowStyles.WS_EX_TOPMOST);
            }
            set
            {
                SetWindowPos(value ? (IntPtr)NativeConstants.HWND_TOPMOST : (IntPtr)NativeConstants.HWND_NOTOPMOST,
                    SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
            }
        }

        public byte Opacity
        {
            get
            {
                if (Layered)
                {
                    NativeMethods.GetLayeredWindowAttributes(Handle, out _, out byte alpha, out _);
                    return alpha;
                }

                return 255;
            }
            set
            {
                if (value < 255)
                {
                    Layered = true;
                    NativeMethods.SetLayeredWindowAttributes(Handle, 0, value, NativeConstants.LWA_ALPHA);
                }
                else
                {
                    Layered = false;
                }
            }
        }

        public Icon Icon => !OperatingSystem.IsWindows() ? null : NativeMethods.GetApplicationIcon(Handle);

        public bool IsMaximized => !OperatingSystem.IsWindows() ? false : NativeMethods.IsZoomed(Handle);

        public bool IsMinimized => !OperatingSystem.IsWindows() ? false : NativeMethods.IsIconic(Handle);

        public bool IsVisible => !OperatingSystem.IsWindows() ? true : NativeMethods.IsWindowVisible(Handle);

        public bool IsCloaked => !OperatingSystem.IsWindows() ? false : NativeMethods.IsWindowCloaked(Handle);

        public bool IsActive => !OperatingSystem.IsWindows() ? false : NativeMethods.IsActive(Handle);

        public WindowInfo(IntPtr handle)
        {
            Handle = handle;
        }

        public WindowInfo(IntPtr handle, string text, Rectangle rectangle)
        {
            Handle = handle;
            _linuxText = text;
            _linuxRectangle = rectangle;
        }

        public void Activate()
        {
            if (IsHandleCreated && OperatingSystem.IsWindows())
            {
                NativeMethods.SetForegroundWindow(Handle);
            }
        }

        public void BringToFront()
        {
            if (IsHandleCreated && OperatingSystem.IsWindows())
            {
                SetWindowPos(SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
            }
        }

        public void Restore()
        {
            if (IsHandleCreated && OperatingSystem.IsWindows())
            {
                NativeMethods.ShowWindow(Handle, (int)WindowShowStyle.Restore);
            }
        }

        public void SetWindowPos(SetWindowPosFlags flags)
        {
            SetWindowPos((IntPtr)NativeConstants.HWND_TOP, 0, 0, 0, 0, flags);
        }

        public void SetWindowPos(Rectangle rect, SetWindowPosFlags flags)
        {
            SetWindowPos((IntPtr)NativeConstants.HWND_TOP, rect.X, rect.Y, rect.Width, rect.Height, flags);
        }

        public void SetWindowPos(IntPtr insertAfter, SetWindowPosFlags flags)
        {
            SetWindowPos(insertAfter, 0, 0, 0, 0, flags);
        }

        public void SetWindowPos(IntPtr insertAfter, int x, int y, int width, int height, SetWindowPosFlags flags)
        {
            if (OperatingSystem.IsWindows())
            {
                NativeMethods.SetWindowPos(Handle, insertAfter, x, y, width, height, flags);
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }
}