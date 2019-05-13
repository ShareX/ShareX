#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace ShareX.HelpersLib
{
    #region Delegates

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    #endregion Delegates

    public static partial class NativeMethods
    {
        #region user32.dll

        [DllImport("user32.dll")]
        public static extern bool AnimateWindow(IntPtr hwnd, int time, AnimateWindowFlags flags);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpfn, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyHeight, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, int diFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentHwnd, IntPtr childAfterHwnd, IntPtr className, string windowText);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern uint GetClassLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool GetCursorInfo(out CursorInfo pci);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo piconinfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect([In] ref IconInfo piconinfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        /// <summary>
        /// The GetNextWindow function retrieves a handle to the next or previous window in the Z-Order.
        /// The next window is below the specified window; the previous window is above.
        /// If the specified window is a topmost window, the function retrieves a handle to the next (or previous) topmost window.
        /// If the specified window is a top-level window, the function retrieves a handle to the next (or previous) top-level window.
        /// If the specified window is a child window, the function searches for a handle to the next (or previous) child window.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowConstants wCmd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int smIndex);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern ulong GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>Determines the visibility state of the specified window.</summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>Determines whether the specified window is minimized (iconic).</summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        /// <summary>Determines whether a window is maximized.</summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int iconId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out UIntPtr lpdwResult);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, int wParam, int lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pptSrc, uint crKey, [In] ref BLENDFUNCTION pblend, uint dwFlags);

        /// <summary> The RegisterHotKey function defines a system-wide hot key </summary>
        /// <param name="hWnd">Handle to the window that will receive WM_HOTKEY messages generated by the hot key.</param>
        /// <param name="id">Specifies the identifier of the hot key.</param>
        /// <param name="fsModifiers">Specifies keys that must be pressed in combination with the key
        /// specified by the 'vk' parameter in order to generate the WM_HOTKEY message.</param>
        /// <param name="vk">Specifies the virtual-key code of the hot key</param>
        /// <returns><c>true</c> if the function succeeds, otherwise <c>false</c></returns>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/ms646309(VS.85).aspx"/>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        #endregion user32.dll

        #region kernel32.dll

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes,
            bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll")]
        public static extern int ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        public static extern int SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetProcessWorkingSetSize(IntPtr handle, IntPtr min, IntPtr max);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern ushort GlobalAddAtom(string lpString);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern ushort GlobalDeleteAtom(ushort nAtom);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        [DllImport("kernel32.dll", PreserveSig = false)]
        public static extern void RegisterApplicationRestart(string pwzCommandline, RegisterApplicationRestartFlags dwFlags);

        #endregion kernel32.dll

        #region gdi32.dll

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, CopyPixelOperation dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nReghtRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nReghtRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFOHEADER pbmi, uint pila, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        #endregion gdi32.dll

        #region gdiplus.dll

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GdipGetImageType(HandleRef image, out int type);

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GdipImageForceValidation(HandleRef image);

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GdipLoadImageFromFile(string filename, out IntPtr image);

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GdipDisposeImage(HandleRef image);

        [DllImport("gdiplus.dll")]
        public static extern int GdipWindingModeOutline(HandleRef path, IntPtr matrix, float flatness);

        #endregion gdiplus.dll

        #region shell32.dll

        [DllImport("shell32.dll")]
        public static extern IntPtr SHAppBarMessage(uint dwMessage, [In] ref APPBARDATA pData);

        [DllImport("shell32.dll", EntryPoint = "#727")]
        public extern static int SHGetImageList(int iImageList, ref Guid riid, ref IImageList ppv);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("shell32.dll")]
        public static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, int cild, IntPtr apidl, int dwFlags);

        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(HChangeNotifyEventID wEventId, HChangeNotifyFlags uFlags, IntPtr dwItem1, IntPtr dwItem2);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr ILCreateFromPathW(string pszPath);

        [DllImport("shell32.dll")]
        public static extern void ILFree(IntPtr pidl);

        #endregion shell32.dll

        #region dwmapi.dll

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out bool pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableComposition(CompositionAction uCompositionAction);

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out SIZE size);

        [DllImport("dwmapi.dll")]
        public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetDxFrameDuration(IntPtr hwnd, uint cRefreshes);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUnregisterThumbnail(IntPtr thumb);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);

        #endregion dwmapi.dll

        #region Other dll

        /// <summary>
        /// Copy a block of memory.
        /// </summary>
        ///
        /// <param name="dst">Destination pointer.</param>
        /// <param name="src">Source pointer.</param>
        /// <param name="count">Memory block's length to copy.</param>
        ///
        /// <returns>Return's the value of <b>dst</b> - pointer to destination.</returns>
        [DllImport("ntdll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int memcpy(int dst, int src, int count);

        /// <summary>
        /// Initialize the AVIFile library.
        /// </summary>
        [DllImport("avifil32.dll")]
        public static extern void AVIFileInit();

        /// <summary>
        /// Exit the AVIFile library.
        /// </summary>
        [DllImport("avifil32.dll")]
        public static extern void AVIFileExit();

        /// <summary>
        /// Open an AVI file.
        /// </summary>
        ///
        /// <param name="aviHandler">Opened AVI file interface.</param>
        /// <param name="fileName">AVI file name.</param>
        /// <param name="mode">Opening mode (see <see cref="OpenFileMode"/>).</param>
        /// <param name="handler">Handler to use (<b>null</b> to use default).</param>
        ///
        /// <returns>Returns zero on success or error code otherwise.</returns>
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)]
        public static extern int AVIFileOpen(out IntPtr aviHandler, string fileName, OpenFileMode mode, IntPtr handler);

        /// <summary>
        /// Release an open AVI stream.
        /// </summary>
        ///
        /// <param name="aviHandler">Open AVI file interface.</param>
        ///
        /// <returns>Returns the reference count of the file.</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIFileRelease(IntPtr aviHandler);

        /// <summary>
        /// Get stream interface that is associated with a specified AVI file
        /// </summary>
        ///
        /// <param name="aviHandler">Handler to an open AVI file.</param>
        /// <param name="streamHandler">Stream interface.</param>
        /// <param name="streamType">Stream type to open.</param>
        /// <param name="streamNumner">Count of the stream type. Identifies which occurrence of the specified stream type to access. </param>
        ///
        /// <returns></returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIFileGetStream(IntPtr aviHandler, out IntPtr streamHandler, int streamType, int streamNumner);

        /// <summary>
        /// Create a new stream in an existing file and creates an interface to the new stream.
        /// </summary>
        ///
        /// <param name="aviHandler">Handler to an open AVI file.</param>
        /// <param name="streamHandler">Stream interface.</param>
        /// <param name="streamInfo">Pointer to a structure containing information about the new stream.</param>
        ///
        /// <returns>Returns zero if successful or an error otherwise.</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIFileCreateStream(IntPtr aviHandler, out IntPtr streamHandler, ref AVISTREAMINFO streamInfo);

        /// <summary>
        /// Release an open AVI stream.
        /// </summary>
        ///
        /// <param name="streamHandler">Handle to an open stream.</param>
        ///
        /// <returns>Returns the current reference count of the stream.</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIStreamRelease(IntPtr streamHandler);

        /// <summary>
        /// Set the format of a stream at the specified position.
        /// </summary>
        ///
        /// <param name="streamHandler">Handle to an open stream.</param>
        /// <param name="position">Position in the stream to receive the format.</param>
        /// <param name="format">Pointer to a structure containing the new format.</param>
        /// <param name="formatSize">Size, in bytes, of the block of memory referenced by <b>format</b>.</param>
        ///
        /// <returns>Returns zero if successful or an error otherwise.</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIStreamSetFormat(IntPtr streamHandler, int position, ref BITMAPINFOHEADER format, int formatSize);

        /// <summary>
        /// Get the starting sample number for the stream.
        /// </summary>
        ///
        /// <param name="streamHandler">Handle to an open stream.</param>
        ///
        /// <returns>Returns the number if successful or  1 otherwise.</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIStreamStart(IntPtr streamHandler);

        /// <summary>
        /// Get the length of the stream.
        /// </summary>
        ///
        /// <param name="streamHandler">Handle to an open stream.</param>
        ///
        /// <returns>Returns the stream's length, in samples, if successful or -1 otherwise.</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIStreamLength(IntPtr streamHandler);

        /// <summary>
        /// Obtain stream header information.
        /// </summary>
        ///
        /// <param name="streamHandler">Handle to an open stream.</param>
        /// <param name="streamInfo">Pointer to a structure to contain the stream information.</param>
        /// <param name="infoSize">Size, in bytes, of the structure used for <b>streamInfo</b>.</param>
        ///
        /// <returns>Returns zero if successful or an error otherwise.</returns>
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)]
        public static extern int AVIStreamInfo(IntPtr streamHandler, ref AVISTREAMINFO streamInfo, int infoSize);

        /// <summary>
        /// Prepare to decompress video frames from the specified video stream
        /// </summary>
        ///
        /// <param name="streamHandler">Pointer to the video stream used as the video source.</param>
        /// <param name="wantedFormat">Pointer to a structure that defines the desired video format. Specify NULL to use a default format.</param>
        ///
        /// <returns>Returns an object that can be used with the <see cref="AVIStreamGetFrame"/> function.</returns>
        [DllImport("avifil32.dll")]
        public static extern IntPtr AVIStreamGetFrameOpen(IntPtr streamHandler, ref BITMAPINFOHEADER wantedFormat);

        /// <summary>
        /// Prepare to decompress video frames from the specified video stream.
        /// </summary>
        ///
        /// <param name="streamHandler">Pointer to the video stream used as the video source.</param>
        /// <param name="wantedFormat">Pointer to a structure that defines the desired video format. Specify NULL to use a default format.</param>
        ///
        /// <returns>Returns a <b>GetFrame</b> object that can be used with the <see cref="AVIStreamGetFrame"/> function.</returns>
        [DllImport("avifil32.dll")]
        public static extern IntPtr AVIStreamGetFrameOpen(IntPtr streamHandler, int wantedFormat);

        /// <summary>
        /// Releases resources used to decompress video frames.
        /// </summary>
        ///
        /// <param name="getFrameObject">Handle returned from the <see cref="AVIStreamGetFrameOpen(IntPtr,int)"/> function.</param>
        ///
        /// <returns>Returns zero if successful or an error otherwise.</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIStreamGetFrameClose(IntPtr getFrameObject);

        /// <summary>
        /// Return the address of a decompressed video frame.
        /// </summary>
        ///
        /// <param name="getFrameObject">Pointer to a GetFrame object.</param>
        /// <param name="position">Position, in samples, within the stream of the desired frame.</param>
        ///
        /// <returns>Returns a pointer to the frame data if successful or NULL otherwise.</returns>
        [DllImport("avifil32.dll")]
        public static extern IntPtr AVIStreamGetFrame(IntPtr getFrameObject, int position);

        /// <summary>
        /// Write data to a stream.
        /// </summary>
        ///
        /// <param name="streamHandler">Handle to an open stream.</param>
        /// <param name="start">First sample to write.</param>
        /// <param name="samples">Number of samples to write.</param>
        /// <param name="buffer">Pointer to a buffer containing the data to write. </param>
        /// <param name="bufferSize">Size of the buffer referenced by <b>buffer</b>.</param>
        /// <param name="flags">Flag associated with this data.</param>
        /// <param name="samplesWritten">Pointer to a buffer that receives the number of samples written. This can be set to NULL.</param>
        /// <param name="bytesWritten">Pointer to a buffer that receives the number of bytes written. This can be set to NULL.</param>
        ///
        /// <returns>Returns zero if successful or an error otherwise.</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIStreamWrite(IntPtr streamHandler, int start, int samples, IntPtr buffer, int bufferSize, int flags, IntPtr samplesWritten,
            IntPtr bytesWritten);

        /// <summary>
        /// Retrieve the save options for a file and returns them in a buffer.
        /// </summary>
        ///
        /// <param name="window">Handle to the parent window for the Compression Options dialog box.</param>
        /// <param name="flags">Flags for displaying the Compression Options dialog box.</param>
        /// <param name="streams">Number of streams that have their options set by the dialog box.</param>
        /// <param name="streamInterfaces">Pointer to an array of stream interface pointers.</param>
        /// <param name="options">Pointer to an array of pointers to AVICOMPRESSOPTIONS structures.</param>
        ///
        /// <returns>Returns TRUE if the user pressed OK, FALSE for CANCEL, or an error otherwise.</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVISaveOptions(IntPtr window, int flags, int streams, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] streamInterfaces,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] options);

        /// <summary>
        /// Free the resources allocated by the AVISaveOptions function.
        /// </summary>
        ///
        /// <param name="streams">Count of the AVICOMPRESSOPTIONS structures referenced in <b>options</b>.</param>
        /// <param name="options">Pointer to an array of pointers to AVICOMPRESSOPTIONS structures.</param>
        ///
        /// <returns>Returns 0.</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVISaveOptionsFree(int streams, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] options);

        /// <summary>
        /// Create a compressed stream from an uncompressed stream and a
        /// compression filter, and returns the address of a pointer to
        /// the compressed stream.
        /// </summary>
        ///
        /// <param name="compressedStream">Pointer to a buffer that receives the compressed stream pointer.</param>
        /// <param name="sourceStream">Pointer to the stream to be compressed.</param>
        /// <param name="options">Pointer to a structure that identifies the type of compression to use and the options to apply.</param>
        /// <param name="clsidHandler">Pointer to a class identifier used to create the stream.</param>
        ///
        /// <returns>Returns 0 if successful or an error otherwise.</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIMakeCompressedStream(out IntPtr compressedStream, IntPtr sourceStream, ref AVICOMPRESSOPTIONS options, IntPtr clsidHandler);

        [DllImport("dnsapi.dll")]
        public static extern uint DnsFlushResolverCache();

        #endregion Other dll
    }
}