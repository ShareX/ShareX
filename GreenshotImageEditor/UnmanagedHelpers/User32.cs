/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

namespace GreenshotPlugin.UnmanagedHelpers
{
    /// <summary>
    /// Used with EnumWindows or EnumChildWindows
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    public delegate int EnumWindowsProc(IntPtr hwnd, int lParam);

    /// <summary>
    /// User32 Wrappers
    /// </summary>
    public static class User32
    {
        public const int SC_RESTORE = 0xF120;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_MINIMIZE = 0xF020;

        public const int PW_DEFAULT = 0x00;
        public const int PW_CLIENTONLY = 0x01;

        // For MonitorFromWindow
        public const int MONITOR_DEFAULTTONULL = 0;
        public const int MONITOR_DEFAULTTOPRIMARY = 1;
        public const int MONITOR_DEFAULTTONEAREST = 2;
        public const Int32 CURSOR_SHOWING = 0x00000001;

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr processId);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCommand uCmd);

        [DllImport("user32", SetLastError = true)]
        public static extern int ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);

        [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
        public extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int cch);

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        public static extern uint GetSysColor(int nIndex);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool IsIconic(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool IsZoomed(IntPtr hwnd);

        [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
        public extern static int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetClassLong(IntPtr hWnd, int nIndex);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32", SetLastError = true)]
        public extern static int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", SetLastError = true, EntryPoint = "SendMessageA")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32", SetLastError = true)]
        public extern static uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32", SetLastError = true)]
        public extern static uint GetWindowLongPtr(IntPtr hwnd, int nIndex);

        [DllImport("user32", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int index, uint styleFlags);

        [DllImport("user32", SetLastError = true)]
        public static extern int SetWindowLongPtr(IntPtr hWnd, int index, uint styleFlags);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr MonitorFromRect([In] ref RECT lprc, uint dwFlags);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WindowInfo pwi);

        [DllImport("user32", SetLastError = true)]
        public extern static int EnumWindows(EnumWindowsProc lpEnumFunc, int lParam);

        [DllImport("user32", SetLastError = true)]
        public extern static int EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, int lParam);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(IntPtr hwnd, ScrollBarDirection scrollBar, bool show);

        [DllImport("user32", SetLastError = true)]
        public static extern int SetScrollPos(IntPtr hWnd, Orientation nBar, int nPos, bool bRedraw);

        [DllImport("user32", SetLastError = true, EntryPoint = "PostMessageA")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32", SetLastError = true)]
        public static extern RegionResult GetWindowRgn(IntPtr hWnd, SafeHandle hRgn);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, WindowPos uFlags);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32", SetLastError = true)]
        public static extern void ReleaseDC(IntPtr dc);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetClipboardOwner();

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        // Added for WinEventHook logic, Greenshot 1.2
        [DllImport("user32", SetLastError = true)]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr SetWinEventHook(WinEvent eventMin, WinEvent eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, int idProcess, int idThread, WinEventHookFlags dwFlags);

        // Added for finding Metro apps, Greenshot 1.1
        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        /// uiFlags: 0 - Count of GDI objects
        /// uiFlags: 1 - Count of USER objects
        /// - Win32 GDI objects (pens, brushes, fonts, palettes, regions, device contexts, bitmap headers)
        /// - Win32 USER objects:
        ///	- 	WIN32 resources (accelerator tables, bitmap resources, dialog box templates, font resources, menu resources, raw data resources, string table entries, message table entries, cursors/icons)
        /// - Other USER objects (windows, menus)
        ///
        [DllImport("user32", SetLastError = true)]
        public static extern uint GetGuiResources(IntPtr hProcess, uint uiFlags);

        [DllImport("user32", EntryPoint = "RegisterWindowMessageA", SetLastError = true)]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out UIntPtr lpdwResult);

        [DllImport("user32", SetLastError = true)]
        public static extern bool GetPhysicalCursorPos(out POINT cursorLocation);

        [DllImport("user32", SetLastError = true)]
        public static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, [MarshalAs(UnmanagedType.U4)] int cPoints);

        [DllImport("user32", SetLastError = true)]
        public static extern int GetSystemMetrics(SystemMetric index);

        /// <summary>
        /// The following is used for Icon handling
        /// </summary>
        /// <param name="hIcon"></param>
        /// <returns></returns>
        [DllImport("user32", SetLastError = true)]
        public static extern SafeIconHandle CopyIcon(IntPtr hIcon);

        [DllImport("user32", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32", SetLastError = true)]
        public static extern bool GetCursorInfo(out CursorInfo cursorInfo);

        [DllImport("user32", SetLastError = true)]
        public static extern bool GetIconInfo(SafeIconHandle iconHandle, out IconInfo iconInfo);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReleaseCapture();

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        /// <summary>
        /// Wrapper for the GetClassLong which decides if the system is 64-bit or not and calls the right one.
        /// </summary>
        /// <param name="hWnd">IntPtr</param>
        /// <param name="nIndex">int</param>
        /// <returns>IntPtr</returns>
        public static IntPtr GetClassLongWrapper(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
            {
                return GetClassLongPtr(hWnd, nIndex);
            }
            else
            {
                return GetClassLong(hWnd, nIndex);
            }
        }

        /// <summary>
        /// Wrapper for the GetWindowLong which decides if the system is 64-bit or not and calls the right one.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public static uint GetWindowLongWrapper(IntPtr hwnd, int nIndex)
        {
            if (IntPtr.Size == 8)
            {
                return GetWindowLongPtr(hwnd, nIndex);
            }
            else
            {
                return GetWindowLong(hwnd, nIndex);
            }
        }

        /// <summary>
        /// Wrapper for the SetWindowLong which decides if the system is 64-bit or not and calls the right one.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nIndex"></param>
        /// <param name="styleFlags"></param>
        public static void SetWindowLongWrapper(IntPtr hwnd, int nIndex, uint styleFlags)
        {
            if (IntPtr.Size == 8)
            {
                SetWindowLongPtr(hwnd, nIndex, styleFlags);
            }
            else
            {
                SetWindowLong(hwnd, nIndex, styleFlags);
            }
        }

        public static uint GetGuiResourcesGDICount()
        {
            return GetGuiResources(Process.GetCurrentProcess().Handle, 0);
        }

        public static uint GetGuiResourcesUserCount()
        {
            return GetGuiResources(Process.GetCurrentProcess().Handle, 1);
        }

        /// <summary>
        /// Helper method to create a Win32 exception with the windows message in it
        /// </summary>
        /// <param name="method">string with current method</param>
        /// <returns>Exception</returns>
        public static Exception CreateWin32Exception(string method)
        {
            Win32Exception exceptionToThrow = new Win32Exception();
            exceptionToThrow.Data.Add("Method", method);
            return exceptionToThrow;
        }
    }

    /// <summary>
    /// Used with SetWinEventHook
    /// </summary>
    /// <param name="hWinEventHook"></param>
    /// <param name="eventType"></param>
    /// <param name="hwnd"></param>
    /// <param name="idObject"></param>
    /// <param name="idChild"></param>
    /// <param name="dwEventThread"></param>
    /// <param name="dwmsEventTime"></param>
    public delegate void WinEventDelegate(IntPtr hWinEventHook, WinEvent eventType, IntPtr hwnd, EventObjects idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    /// <summary>
    /// A SafeHandle class implementation for the hIcon
    /// </summary>
    public class SafeIconHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeIconHandle()
            : base(true)
        {
        }

        public SafeIconHandle(IntPtr hIcon)
            : base(true)
        {
            SetHandle(hIcon);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        protected override bool ReleaseHandle()
        {
            return User32.DestroyIcon(handle);
        }
    }

    /// <summary>
    /// A WindowDC SafeHandle implementation
    /// </summary>
    public class SafeWindowDCHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32", SetLastError = true)]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        private IntPtr hWnd;

        [SecurityCritical]
        private SafeWindowDCHandle()
            : base(true)
        {
        }

        [SecurityCritical]
        public SafeWindowDCHandle(IntPtr hWnd, IntPtr preexistingHandle)
            : base(true)
        {
            this.hWnd = hWnd;
            SetHandle(preexistingHandle);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        protected override bool ReleaseHandle()
        {
            bool returnValue = ReleaseDC(hWnd, handle);
            return returnValue;
        }

        public static SafeWindowDCHandle fromDesktop()
        {
            IntPtr hWndDesktop = User32.GetDesktopWindow();
            IntPtr hDCDesktop = GetWindowDC(hWndDesktop);
            return new SafeWindowDCHandle(hWndDesktop, hDCDesktop);
        }
    }
}