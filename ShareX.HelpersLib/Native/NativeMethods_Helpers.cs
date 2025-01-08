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
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static partial class NativeMethods
    {
        public static string GetForegroundWindowText()
        {
            IntPtr handle = GetForegroundWindow();
            return GetWindowText(handle);
        }

        public static string GetWindowText(IntPtr handle)
        {
            if (handle.ToInt32() > 0)
            {
                try
                {
                    int length = GetWindowTextLength(handle);

                    if (length > 0)
                    {
                        StringBuilder sb = new StringBuilder(length + 1);

                        if (GetWindowText(handle, sb, sb.Capacity) > 0)
                        {
                            return sb.ToString();
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return null;
        }

        public static Process GetForegroundWindowProcess()
        {
            IntPtr handle = GetForegroundWindow();
            return GetProcessByWindowHandle(handle);
        }

        public static string GetForegroundWindowProcessName()
        {
            using (Process process = GetForegroundWindowProcess())
            {
                return process?.ProcessName;
            }
        }

        public static Process GetProcessByWindowHandle(IntPtr hwnd)
        {
            if (hwnd.ToInt32() > 0)
            {
                try
                {
                    GetWindowThreadProcessId(hwnd, out uint processID);

                    if (processID != 0)
                    {
                        return Process.GetProcessById((int)processID);
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return null;
        }

        public static string GetClassName(IntPtr handle)
        {
            if (handle.ToInt32() > 0)
            {
                StringBuilder sb = new StringBuilder(256);

                if (GetClassName(handle, sb, sb.Capacity) > 0)
                {
                    return sb.ToString();
                }
            }

            return null;
        }

        public static IntPtr GetClassLongPtrSafe(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
            {
                return GetClassLongPtr(hWnd, nIndex);
            }

            return new IntPtr(GetClassLong(hWnd, nIndex));
        }

        public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32(hWnd, nIndex);
            }

            return GetWindowLongPtr64(hWnd, nIndex);
        }

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            }

            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        private static Icon GetSmallApplicationIcon(IntPtr handle)
        {
            IntPtr iconHandle;

            SendMessageTimeout(handle, (int)WindowsMessages.GETICON, NativeConstants.ICON_SMALL2, 0, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out iconHandle);

            if (iconHandle == IntPtr.Zero)
            {
                SendMessageTimeout(handle, (int)WindowsMessages.GETICON, NativeConstants.ICON_SMALL, 0, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out iconHandle);

                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = GetClassLongPtrSafe(handle, NativeConstants.GCL_HICONSM);

                    if (iconHandle == IntPtr.Zero)
                    {
                        SendMessageTimeout(handle, (int)WindowsMessages.QUERYDRAGICON, 0, 0, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out iconHandle);
                    }
                }
            }

            if (iconHandle != IntPtr.Zero)
            {
                return Icon.FromHandle(iconHandle);
            }

            return null;
        }

        private static Icon GetBigApplicationIcon(IntPtr handle)
        {
            SendMessageTimeout(handle, (int)WindowsMessages.GETICON, NativeConstants.ICON_BIG, 0, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out IntPtr iconHandle);

            if (iconHandle == IntPtr.Zero)
            {
                iconHandle = GetClassLongPtrSafe(handle, NativeConstants.GCL_HICON);
            }

            if (iconHandle != IntPtr.Zero)
            {
                return Icon.FromHandle(iconHandle);
            }

            return null;
        }

        public static Icon GetApplicationIcon(IntPtr handle)
        {
            return GetSmallApplicationIcon(handle) ?? GetBigApplicationIcon(handle);
        }

        public static bool GetBorderSize(IntPtr handle, out Size size)
        {
            WINDOWINFO wi = WINDOWINFO.Create();
            bool result = GetWindowInfo(handle, ref wi);

            if (result)
            {
                size = new Size((int)wi.cxWindowBorders, (int)wi.cyWindowBorders);
            }
            else
            {
                size = Size.Empty;
            }

            return result;
        }

        public static bool GetWindowRegion(IntPtr hWnd, out Region region)
        {
            IntPtr hRgn = CreateRectRgn(0, 0, 0, 0);
            RegionType regionType = (RegionType)GetWindowRgn(hWnd, hRgn);
            region = Region.FromHrgn(hRgn);
            return regionType != RegionType.ERROR && regionType != RegionType.NULLREGION;
        }

        public static bool IsDWMEnabled()
        {
            return Helpers.IsWindowsVistaOrGreater() && DwmIsCompositionEnabled();
        }

        public static bool GetExtendedFrameBounds(IntPtr handle, out Rectangle rectangle)
        {
            int result = DwmGetWindowAttribute(handle, (int)DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out RECT rect, Marshal.SizeOf(typeof(RECT)));
            rectangle = rect;
            return result == 0;
        }

        public static bool GetNCRenderingEnabled(IntPtr handle)
        {
            int result = DwmGetWindowAttribute(handle, (int)DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_ENABLED, out bool enabled, sizeof(bool));
            return result == 0 && enabled;
        }

        public static void SetNCRenderingPolicy(IntPtr handle, DWMNCRENDERINGPOLICY renderingPolicy)
        {
            int attrValue = (int)renderingPolicy;
            DwmSetWindowAttribute(handle, (int)DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_POLICY, ref attrValue, sizeof(int));
        }

        public static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (Helpers.IsWindows10OrGreater(18985))
            {
                int useImmersiveDarkMode = enabled ? 1 : 0;
                return DwmSetWindowAttribute(handle, (int)DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useImmersiveDarkMode, sizeof(int)) == 0;
            }

            return false;
        }

        public static void SetWindowCornerPreference(IntPtr handle, DWM_WINDOW_CORNER_PREFERENCE cornerPreference)
        {
            int attrValue = (int)cornerPreference;
            DwmSetWindowAttribute(handle, (int)DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref attrValue, sizeof(int));
        }

        public static Rectangle GetWindowRect(IntPtr handle)
        {
            GetWindowRect(handle, out RECT rect);
            return rect;
        }

        public static Rectangle GetClientRect(IntPtr handle)
        {
            GetClientRect(handle, out RECT rect);
            Point position = rect.Location;
            ClientToScreen(handle, ref position);
            return new Rectangle(position, rect.Size);
        }

        public static Rectangle MaximizedWindowFix(IntPtr handle, Rectangle windowRect)
        {
            if (GetBorderSize(handle, out Size size))
            {
                windowRect = new Rectangle(windowRect.X + size.Width, windowRect.Y + size.Height, windowRect.Width - (size.Width * 2), windowRect.Height - (size.Height * 2));
            }

            return windowRect;
        }

        public static Rectangle GetTaskbarRectangle()
        {
            APPBARDATA abd = APPBARDATA.NewAPPBARDATA();
            SHAppBarMessage((uint)ABMsg.ABM_GETTASKBARPOS, ref abd);
            return abd.rc;
        }

        public static bool SetTaskbarVisibilityIfIntersect(bool visible, Rectangle rect)
        {
            bool result = false;

            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);

            if (taskbarHandle != IntPtr.Zero)
            {
                Rectangle taskbarRect = GetWindowRect(taskbarHandle);

                if (rect.IntersectsWith(taskbarRect))
                {
                    ShowWindow(taskbarHandle, visible ? (int)WindowShowStyle.Show : (int)WindowShowStyle.Hide);
                    result = true;
                }

                if (Helpers.IsWindowsVista() || Helpers.IsWindows7())
                {
                    IntPtr startHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, null);

                    if (startHandle != IntPtr.Zero)
                    {
                        Rectangle startRect = GetWindowRect(startHandle);

                        if (rect.IntersectsWith(startRect))
                        {
                            ShowWindow(startHandle, visible ? (int)WindowShowStyle.Show : (int)WindowShowStyle.Hide);
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public static bool SetTaskbarVisibility(bool visible)
        {
            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);

            if (taskbarHandle != IntPtr.Zero)
            {
                ShowWindow(taskbarHandle, visible ? (int)WindowShowStyle.Show : (int)WindowShowStyle.Hide);

                if (Helpers.IsWindowsVista() || Helpers.IsWindows7())
                {
                    IntPtr startHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, null);

                    if (startHandle != IntPtr.Zero)
                    {
                        ShowWindow(startHandle, visible ? (int)WindowShowStyle.Show : (int)WindowShowStyle.Hide);
                    }
                }

                return true;
            }

            return false;
        }

        public static void TrimMemoryUse()
        {
            GC.Collect();
            GC.WaitForFullGCComplete();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (IntPtr)(-1), (IntPtr)(-1));
        }

        public static bool IsWindowCloaked(IntPtr handle)
        {
            if (IsDWMEnabled())
            {
                int result = DwmGetWindowAttribute(handle, (int)DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out int cloaked, sizeof(int));
                return result == 0 && cloaked != 0;
            }

            return false;
        }

        public static bool IsActive(IntPtr handle)
        {
            return GetForegroundWindow() == handle;
        }

        public static void RestoreWindow(IntPtr handle)
        {
            WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
            wp.length = Marshal.SizeOf(wp);

            if (GetWindowPlacement(handle, ref wp))
            {
                if (wp.flags == (int)WindowPlacementFlags.WPF_RESTORETOMAXIMIZED)
                {
                    wp.showCmd = WindowShowStyle.ShowMaximized;
                }
                else
                {
                    wp.showCmd = WindowShowStyle.Restore;
                }

                SetWindowPlacement(handle, ref wp);
            }
        }

        /// <summary>
        /// Version of <see cref="AVISaveOptions(IntPtr, int, int, IntPtr[], IntPtr[])"/> for one stream only.
        /// </summary>
        ///
        /// <param name="stream">Stream to configure.</param>
        /// <param name="options">Stream options.</param>
        ///
        /// <returns>Returns TRUE if the user pressed OK, FALSE for CANCEL, or an error otherwise.</returns>
        public static int AVISaveOptions(IntPtr stream, ref AVICOMPRESSOPTIONS options, IntPtr parentWindow)
        {
            IntPtr[] streams = new IntPtr[1];
            IntPtr[] infPtrs = new IntPtr[1];

            // alloc unmanaged memory
            IntPtr mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(AVICOMPRESSOPTIONS)));

            // copy from managed structure to unmanaged memory
            Marshal.StructureToPtr(options, mem, false);

            streams[0] = stream;
            infPtrs[0] = mem;

            // show dialog with a list of available compresors and configuration
            int ret = AVISaveOptions(parentWindow, 0, 1, streams, infPtrs);

            // copy from unmanaged memory to managed structure
            options = (AVICOMPRESSOPTIONS)Marshal.PtrToStructure(mem, typeof(AVICOMPRESSOPTIONS));

            // free AVI compression options
            AVISaveOptionsFree(1, infPtrs);

            // clear it, because the information already freed by AVISaveOptionsFree
            options.format = 0;
            options.parameters = 0;

            // free unmanaged memory
            Marshal.FreeHGlobal(mem);

            return ret;
        }

        /// <summary>
        /// .NET replacement of mmioFOURCC macros. Converts four characters to code.
        /// </summary>
        ///
        /// <param name="str">Four characters string.</param>
        ///
        /// <returns>Returns the code created from provided characters.</returns>
        public static int mmioFOURCC(string str)
        {
            return (
                (byte)(str[0]) |
                ((byte)(str[1]) << 8) |
                ((byte)(str[2]) << 16) |
                ((byte)(str[3]) << 24));
        }

        /// <summary>
        /// Inverse to <see cref="mmioFOURCC"/>. Converts code to fout characters string.
        /// </summary>
        ///
        /// <param name="code">Code to convert.</param>
        ///
        /// <returns>Returns four characters string.</returns>
        public static string decode_mmioFOURCC(int code)
        {
            char[] chs = new char[4];

            for (int i = 0; i < 4; i++)
            {
                chs[i] = (char)(byte)((code >> (i << 3)) & 0xFF);
                if (!char.IsLetterOrDigit(chs[i]))
                    chs[i] = ' ';
            }
            return new string(chs);
        }

        public static bool FlashWindowEx(Form frm, uint flashCount = uint.MaxValue)
        {
            FLASHWINFO fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = frm.Handle;
            fInfo.dwFlags = (uint)FlashWindow.FLASHW_ALL | (uint)FlashWindow.FLASHW_TIMERNOFG;
            fInfo.uCount = flashCount;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }

        public static void OpenFolderAndSelectFile(string filePath)
        {
            IntPtr pidl = ILCreateFromPathW(filePath);

            try
            {
                SHOpenFolderAndSelectItems(pidl, 0, IntPtr.Zero, 0);
            }
            finally
            {
                ILFree(pidl);
            }
        }

        public static bool CreateProcess(string path, string arguments, CreateProcessFlags flags = CreateProcessFlags.NORMAL_PRIORITY_CLASS)
        {
            //PROCESS_INFORMATION pInfo = new PROCESS_INFORMATION();
            STARTUPINFO sInfo = new STARTUPINFO();
            SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();
            SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();
            pSec.nLength = Marshal.SizeOf(pSec);
            tSec.nLength = Marshal.SizeOf(tSec);

            return CreateProcess(path, $"\"{path}\" {arguments}", ref pSec, ref tSec, false, (uint)flags, IntPtr.Zero, null, ref sInfo, out _);
        }

        public static Icon GetFileIcon(string filePath, bool isSmallIcon)
        {
            SHFILEINFO shfi = new SHFILEINFO();

            SHGFI flags = SHGFI.Icon;

            if (isSmallIcon)
            {
                flags |= SHGFI.SmallIcon;
            }
            else
            {
                flags |= SHGFI.LargeIcon;
            }

            SHGetFileInfo(filePath, 0, ref shfi, (uint)Marshal.SizeOf(shfi), (uint)flags);

            Icon icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            DestroyIcon(shfi.hIcon);
            return icon;
        }

        public static Icon GetJumboFileIcon(string filePath, bool jumboSize = true)
        {
            SHFILEINFO shfi = new SHFILEINFO();

            SHGFI flags = SHGFI.SysIconIndex | SHGFI.UseFileAttributes;
            SHGetFileInfo(filePath, 0, ref shfi, (uint)Marshal.SizeOf(shfi), (uint)flags);

            IImageList spiml = null;
            Guid guil = new Guid(NativeConstants.IID_IImageList2);

            SHGetImageList(jumboSize ? NativeConstants.SHIL_JUMBO : NativeConstants.SHIL_EXTRALARGE, ref guil, ref spiml);
            IntPtr hIcon = IntPtr.Zero;
            spiml.GetIcon(shfi.iIcon, NativeConstants.ILD_TRANSPARENT | NativeConstants.ILD_IMAGE, ref hIcon);

            Icon icon = (Icon)Icon.FromHandle(hIcon).Clone();
            DestroyIcon(hIcon);
            return icon;
        }

        public static float GetScreenScalingFactor()
        {
            float scalingFactor;

            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr desktop = g.GetHdc();
                int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
                int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
                int logpixelsy = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);
                float screenScalingFactor = (float)PhysicalScreenHeight / LogicalScreenHeight;
                float dpiScalingFactor = logpixelsy / 96f;
                scalingFactor = Math.Max(screenScalingFactor, dpiScalingFactor);
                g.ReleaseHdc(desktop);
            }

            return scalingFactor;
        }

        public static IntPtr SearchWindow(string windowTitle)
        {
            IntPtr hWnd = FindWindow(null, windowTitle);

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