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
using System.Drawing;

namespace ShareX.HelpersLib
{
    public class WindowInfo
    {
        public IntPtr Handle { get; }

        public bool IsHandleCreated => Handle != IntPtr.Zero;

        public string Text => NativeMethods.GetWindowText(Handle);

        public string ClassName => NativeMethods.GetClassName(Handle);

        public Process Process => NativeMethods.GetProcessByWindowHandle(Handle);

        public string ProcessName
        {
            get
            {
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
                using (Process process = Process)
                {
                    return process.Id;
                }
            }
        }

        public IntPtr Parent => NativeMethods.GetParent(Handle);

        public Rectangle Rectangle => CaptureHelpers.GetWindowRectangle(Handle);

        public Rectangle ClientRectangle => NativeMethods.GetClientRect(Handle);

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
                return (WindowStyles)(ulong)NativeMethods.GetWindowLong(Handle, NativeConstants.GWL_EXSTYLE);
            }
            set
            {
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

        public Icon Icon => NativeMethods.GetApplicationIcon(Handle);

        public bool IsMaximized => NativeMethods.IsZoomed(Handle);

        public bool IsMinimized => NativeMethods.IsIconic(Handle);

        public bool IsVisible => NativeMethods.IsWindowVisible(Handle) && !IsCloaked;

        public bool IsCloaked => NativeMethods.IsWindowCloaked(Handle);

        public bool IsActive => NativeMethods.IsActive(Handle);

        public WindowInfo(IntPtr handle)
        {
            Handle = handle;
        }

        public void Activate()
        {
            if (IsHandleCreated)
            {
                NativeMethods.SetForegroundWindow(Handle);
            }
        }

        public void BringToFront()
        {
            if (IsHandleCreated)
            {
                SetWindowPos(SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
            }
        }

        public void Restore()
        {
            if (IsHandleCreated)
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
            NativeMethods.SetWindowPos(Handle, insertAfter, x, y, width, height, flags);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}