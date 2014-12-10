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

using System;

namespace GreenshotPlugin.UnmanagedHelpers
{
    /// <summary>
    /// Window Style Flags
    /// </summary>
    [Flags]
    public enum WindowStyleFlags : long
    {
        //WS_OVERLAPPED       = 0x00000000,
        WS_POPUP = 0x80000000,
        WS_CHILD = 0x40000000,
        WS_MINIMIZE = 0x20000000,
        WS_VISIBLE = 0x10000000,
        WS_DISABLED = 0x08000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_MAXIMIZE = 0x01000000,
        WS_BORDER = 0x00800000,
        WS_DLGFRAME = 0x00400000,
        WS_VSCROLL = 0x00200000,
        WS_HSCROLL = 0x00100000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000,
        WS_GROUP = 0x00020000,
        WS_TABSTOP = 0x00010000,

        WS_UNK8000 = 0x00008000,
        WS_UNK4000 = 0x00004000,
        WS_UNK2000 = 0x00002000,
        WS_UNK1000 = 0x00001000,
        WS_UNK800 = 0x00000800,
        WS_UNK400 = 0x00000400,
        WS_UNK200 = 0x00000200,
        WS_UNK100 = 0x00000100,
        WS_UNK80 = 0x00000080,
        WS_UNK40 = 0x00000040,
        WS_UNK20 = 0x00000020,
        WS_UNK10 = 0x00000010,
        WS_UNK8 = 0x00000008,
        WS_UNK4 = 0x00000004,
        WS_UNK2 = 0x00000002,
        WS_UNK1 = 0x00000001,

        //WS_MINIMIZEBOX      = 0x00020000,
        //WS_MAXIMIZEBOX      = 0x00010000,

        //WS_CAPTION          = WS_BORDER | WS_DLGFRAME,
        //WS_TILED            = WS_OVERLAPPED,
        //WS_ICONIC           = WS_MINIMIZE,
        //WS_SIZEBOX          = WS_THICKFRAME,
        //WS_TILEDWINDOW      = WS_OVERLAPPEDWINDOW,

        //WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        //WS_POPUPWINDOW	    = WS_POPUP | WS_BORDER | WS_SYSMENU
        //WS_CHILDWINDOW      = WS_CHILD
    }

    [Flags]
    public enum ExtendedWindowStyleFlags : uint
    {
        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_ACCEPTFILES = 0x00000010,
        WS_EX_TRANSPARENT = 0x00000020,

        //#if(WINVER >= 0x0400)
        WS_EX_MDICHILD = 0x00000040,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_WINDOWEDGE = 0x00000100,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_CONTEXTHELP = 0x00000400,

        WS_EX_RIGHT = 0x00001000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_LTRREADING = 0x00000000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_RIGHTSCROLLBAR = 0x00000000,

        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_APPWINDOW = 0x00040000,

        //WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
        //WS_EX_PALETTEWINDOW    = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),

        WS_EX_LAYERED = 0x00080000,
        WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
        WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring
        WS_EX_COMPOSITED = 0x02000000,
        WS_EX_NOACTIVATE = 0x08000000
    }

    [Flags]
    public enum WindowPlacementFlags : uint
    {
        // The coordinates of the minimized window may be specified.
        // This flag must be specified if the coordinates are set in the ptMinPosition member.
        WPF_SETMINPOSITION = 0x0001,
        // If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
        WPF_ASYNCWINDOWPLACEMENT = 0x0004,
        // The restored window will be maximized, regardless of whether it was maximized before it was minimized. This setting is only valid the next time the window is restored. It does not change the default restoration behavior.
        // This flag is only valid when the SW_SHOWMINIMIZED value is specified for the showCmd member.
        WPF_RESTORETOMAXIMIZED = 0x0002
    }

    public enum ShowWindowCommand : uint
    {
        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        Hide = 0,
        /// <summary>
        /// Activates and displays a window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when displaying the window
        /// for the first time.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        ShowMinimized = 2,
        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        Maximize = 3, // is this the right value?
        /// <summary>
        /// Activates the window and displays it as a maximized window.
        /// </summary>
        ShowMaximized = 3,
        /// <summary>
        /// Displays a window in its most recent size and position. This value
        /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except
        /// the window is not actived.
        /// </summary>
        ShowNoActivate = 4,
        /// <summary>
        /// Activates the window and displays it in its current size and position.
        /// </summary>
        Show = 5,
        /// <summary>
        /// Minimizes the specified window and activates the next top-level
        /// window in the Z order.
        /// </summary>
        Minimize = 6,
        /// <summary>
        /// Displays the window as a minimized window. This value is similar to
        /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the
        /// window is not activated.
        /// </summary>
        ShowMinNoActive = 7,
        /// <summary>
        /// Displays the window in its current size and position. This value is
        /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the
        /// window is not activated.
        /// </summary>
        ShowNA = 8,
        /// <summary>
        /// Activates and displays the window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when restoring a minimized window.
        /// </summary>
        Restore = 9,
        /// <summary>
        /// Sets the show state based on the SW_* value specified in the
        /// STARTUPINFO structure passed to the CreateProcess function by the
        /// program that started the application.
        /// </summary>
        ShowDefault = 10,
        /// <summary>
        ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
        /// that owns the window is not responding. This flag should only be
        /// used when minimizing windows from a different thread.
        /// </summary>
        ForceMinimize = 11
    }

    public enum SYSCOLOR : int
    {
        SCROLLBAR = 0,
        BACKGROUND = 1,
        DESKTOP = 1,
        ACTIVECAPTION = 2,
        INACTIVECAPTION = 3,
        MENU = 4,
        WINDOW = 5,
        WINDOWFRAME = 6,
        MENUTEXT = 7,
        WINDOWTEXT = 8,
        CAPTIONTEXT = 9,
        ACTIVEBORDER = 10,
        INACTIVEBORDER = 11,
        APPWORKSPACE = 12,
        HIGHLIGHT = 13,
        HIGHLIGHTTEXT = 14,
        BTNFACE = 15,
        THREEDFACE = 15,
        BTNSHADOW = 16,
        THREEDSHADOW = 16,
        GRAYTEXT = 17,
        BTNTEXT = 18,
        INACTIVECAPTIONTEXT = 19,
        BTNHIGHLIGHT = 20,
        TREEDHIGHLIGHT = 20,
        THREEDHILIGHT = 20,
        BTNHILIGHT = 20,
        THREEDDKSHADOW = 21,
        THREEDLIGHT = 22,
        INFOTEXT = 23,
        INFOBK = 24
    }
    /// <summary>
    /// Flags used with the Windows API (User32.dll):GetSystemMetrics(SystemMetric smIndex)
    ///
    /// This Enum and declaration signature was written by Gabriel T. Sharp
    /// ai_productions@verizon.net or osirisgothra@hotmail.com
    /// Obtained on pinvoke.net, please contribute your code to support the wiki!
    /// </summary>
    public enum SystemMetric : int
    {
        /// <summary>
        ///  Width of the screen of the primary display monitor, in pixels. This is the same values obtained by calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, HORZRES).
        /// </summary>
        SM_CXSCREEN = 0,
        /// <summary>
        /// Height of the screen of the primary display monitor, in pixels. This is the same values obtained by calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, VERTRES).
        /// </summary>
        SM_CYSCREEN = 1,
        /// <summary>
        /// Width of a horizontal scroll bar, in pixels.
        /// </summary>
        SM_CYVSCROLL = 2,
        /// <summary>
        /// Height of a horizontal scroll bar, in pixels.
        /// </summary>
        SM_CXVSCROLL = 3,
        /// <summary>
        /// Height of a caption area, in pixels.
        /// </summary>
        SM_CYCAPTION = 4,
        /// <summary>
        /// Width of a window border, in pixels. This is equivalent to the SM_CXEDGE value for windows with the 3-D look.
        /// </summary>
        SM_CXBORDER = 5,
        /// <summary>
        /// Height of a window border, in pixels. This is equivalent to the SM_CYEDGE value for windows with the 3-D look.
        /// </summary>
        SM_CYBORDER = 6,
        /// <summary>
        /// Thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels. SM_CXFIXEDFRAME is the height of the horizontal border and SM_CYFIXEDFRAME is the width of the vertical border.
        /// </summary>
        SM_CXDLGFRAME = 7,
        /// <summary>
        /// Thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels. SM_CXFIXEDFRAME is the height of the horizontal border and SM_CYFIXEDFRAME is the width of the vertical border.
        /// </summary>
        SM_CYDLGFRAME = 8,
        /// <summary>
        /// Height of the thumb box in a vertical scroll bar, in pixels
        /// </summary>
        SM_CYVTHUMB = 9,
        /// <summary>
        /// Width of the thumb box in a horizontal scroll bar, in pixels.
        /// </summary>
        SM_CXHTHUMB = 10,
        /// <summary>
        /// Default width of an icon, in pixels. The LoadIcon function can load only icons with the dimensions specified by SM_CXICON and SM_CYICON
        /// </summary>
        SM_CXICON = 11,
        /// <summary>
        /// Default height of an icon, in pixels. The LoadIcon function can load only icons with the dimensions SM_CXICON and SM_CYICON.
        /// </summary>
        SM_CYICON = 12,
        /// <summary>
        /// Width of a cursor, in pixels. The system cannot create cursors of other sizes.
        /// </summary>
        SM_CXCURSOR = 13,
        /// <summary>
        /// Height of a cursor, in pixels. The system cannot create cursors of other sizes.
        /// </summary>
        SM_CYCURSOR = 14,
        /// <summary>
        /// Height of a single-line menu bar, in pixels.
        /// </summary>
        SM_CYMENU = 15,
        /// <summary>
        /// Width of the client area for a full-screen window on the primary display monitor, in pixels. To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
        /// </summary>
        SM_CXFULLSCREEN = 16,
        /// <summary>
        /// Height of the client area for a full-screen window on the primary display monitor, in pixels. To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
        /// </summary>
        SM_CYFULLSCREEN = 17,
        /// <summary>
        /// For double byte character set versions of the system, this is the height of the Kanji window at the bottom of the screen, in pixels
        /// </summary>
        SM_CYKANJIWINDOW = 18,
        /// <summary>
        /// Nonzero if a mouse with a wheel is installed; zero otherwise
        /// </summary>
        SM_MOUSEWHEELPRESENT = 75,
        /// <summary>
        /// Height of the arrow bitmap on a vertical scroll bar, in pixels.
        /// </summary>
        SM_CYHSCROLL = 20,
        /// <summary>
        /// Width of the arrow bitmap on a horizontal scroll bar, in pixels.
        /// </summary>
        SM_CXHSCROLL = 21,
        /// <summary>
        /// Nonzero if the debug version of User.exe is installed; zero otherwise.
        /// </summary>
        SM_DEBUG = 22,
        /// <summary>
        /// Nonzero if the left and right mouse buttons are reversed; zero otherwise.
        /// </summary>
        SM_SWAPBUTTON = 23,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        SM_RESERVED1 = 24,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        SM_RESERVED2 = 25,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        SM_RESERVED3 = 26,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        SM_RESERVED4 = 27,
        /// <summary>
        /// Minimum width of a window, in pixels.
        /// </summary>
        SM_CXMIN = 28,
        /// <summary>
        /// Minimum height of a window, in pixels.
        /// </summary>
        SM_CYMIN = 29,
        /// <summary>
        /// Width of a button in a window's caption or title bar, in pixels.
        /// </summary>
        SM_CXSIZE = 30,
        /// <summary>
        /// Height of a button in a window's caption or title bar, in pixels.
        /// </summary>
        SM_CYSIZE = 31,
        /// <summary>
        /// Thickness of the sizing border around the perimeter of a window that can be resized, in pixels. SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border.
        /// </summary>
        SM_CXFRAME = 32,
        /// <summary>
        /// Thickness of the sizing border around the perimeter of a window that can be resized, in pixels. SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border.
        /// </summary>
        SM_CYFRAME = 33,
        /// <summary>
        /// Minimum tracking width of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
        /// </summary>
        SM_CXMINTRACK = 34,
        /// <summary>
        /// Minimum tracking height of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message
        /// </summary>
        SM_CYMINTRACK = 35,
        /// <summary>
        /// Width of the rectangle around the location of a first click in a double-click sequence, in pixels. The second click must occur within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system to consider the two clicks a double-click
        /// </summary>
        SM_CXDOUBLECLK = 36,
        /// <summary>
        /// Height of the rectangle around the location of a first click in a double-click sequence, in pixels. The second click must occur within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system to consider the two clicks a double-click. (The two clicks must also occur within a specified time.)
        /// </summary>
        SM_CYDOUBLECLK = 37,
        /// <summary>
        /// Width of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CXICON
        /// </summary>
        SM_CXICONSPACING = 38,
        /// <summary>
        /// Height of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CYICON.
        /// </summary>
        SM_CYICONSPACING = 39,
        /// <summary>
        /// Nonzero if drop-down menus are right-aligned with the corresponding menu-bar item; zero if the menus are left-aligned.
        /// </summary>
        SM_MENUDROPALIGNMENT = 40,
        /// <summary>
        /// Nonzero if the Microsoft Windows for Pen computing extensions are installed; zero otherwise.
        /// </summary>
        SM_PENWINDOWS = 41,
        /// <summary>
        /// Nonzero if User32.dll supports DBCS; zero otherwise. (WinMe/95/98): Unicode
        /// </summary>
        SM_DBCSENABLED = 42,
        /// <summary>
        /// Number of buttons on mouse, or zero if no mouse is installed.
        /// </summary>
        SM_CMOUSEBUTTONS = 43,
        /// <summary>
        /// Identical Values Changed After Windows NT 4.0
        /// </summary>
        SM_CXFIXEDFRAME = SM_CXDLGFRAME,
        /// <summary>
        /// Identical Values Changed After Windows NT 4.0
        /// </summary>
        SM_CYFIXEDFRAME = SM_CYDLGFRAME,
        /// <summary>
        /// Identical Values Changed After Windows NT 4.0
        /// </summary>
        SM_CXSIZEFRAME = SM_CXFRAME,
        /// <summary>
        /// Identical Values Changed After Windows NT 4.0
        /// </summary>
        SM_CYSIZEFRAME = SM_CYFRAME,
        /// <summary>
        /// Nonzero if security is present; zero otherwise.
        /// </summary>
        SM_SECURE = 44,
        /// <summary>
        /// Width of a 3-D border, in pixels. This is the 3-D counterpart of SM_CXBORDER
        /// </summary>
        SM_CXEDGE = 45,
        /// <summary>
        /// Height of a 3-D border, in pixels. This is the 3-D counterpart of SM_CYBORDER
        /// </summary>
        SM_CYEDGE = 46,
        /// <summary>
        /// Width of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged. This value is always greater than or equal to SM_CXMINIMIZED.
        /// </summary>
        SM_CXMINSPACING = 47,
        /// <summary>
        /// Height of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged. This value is always greater than or equal to SM_CYMINIMIZED.
        /// </summary>
        SM_CYMINSPACING = 48,
        /// <summary>
        /// Recommended width of a small icon, in pixels. Small icons typically appear in window captions and in small icon view
        /// </summary>
        SM_CXSMICON = 49,
        /// <summary>
        /// Recommended height of a small icon, in pixels. Small icons typically appear in window captions and in small icon view.
        /// </summary>
        SM_CYSMICON = 50,
        /// <summary>
        /// Height of a small caption, in pixels
        /// </summary>
        SM_CYSMCAPTION = 51,
        /// <summary>
        /// Width of small caption buttons, in pixels.
        /// </summary>
        SM_CXSMSIZE = 52,
        /// <summary>
        /// Height of small caption buttons, in pixels.
        /// </summary>
        SM_CYSMSIZE = 53,
        /// <summary>
        /// Width of menu bar buttons, such as the child window close button used in the multiple document interface, in pixels.
        /// </summary>
        SM_CXMENUSIZE = 54,
        /// <summary>
        /// Height of menu bar buttons, such as the child window close button used in the multiple document interface, in pixels.
        /// </summary>
        SM_CYMENUSIZE = 55,
        /// <summary>
        /// Flags specifying how the system arranged minimized windows
        /// </summary>
        SM_ARRANGE = 56,
        /// <summary>
        /// Width of a minimized window, in pixels.
        /// </summary>
        SM_CXMINIMIZED = 57,
        /// <summary>
        /// Height of a minimized window, in pixels.
        /// </summary>
        SM_CYMINIMIZED = 58,
        /// <summary>
        /// Default maximum width of a window that has a caption and sizing borders, in pixels. This metric refers to the entire desktop. The user cannot drag the window frame to a size larger than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
        /// </summary>
        SM_CXMAXTRACK = 59,
        /// <summary>
        /// Default maximum height of a window that has a caption and sizing borders, in pixels. This metric refers to the entire desktop. The user cannot drag the window frame to a size larger than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
        /// </summary>
        SM_CYMAXTRACK = 60,
        /// <summary>
        /// Default width, in pixels, of a maximized top-level window on the primary display monitor.
        /// </summary>
        SM_CXMAXIMIZED = 61,
        /// <summary>
        /// Default height, in pixels, of a maximized top-level window on the primary display monitor.
        /// </summary>
        SM_CYMAXIMIZED = 62,
        /// <summary>
        /// Least significant bit is set if a network is present; otherwise, it is cleared. The other bits are reserved for future use
        /// </summary>
        SM_NETWORK = 63,
        /// <summary>
        /// Value that specifies how the system was started: 0-normal, 1-failsafe, 2-failsafe /w net
        /// </summary>
        SM_CLEANBOOT = 67,
        /// <summary>
        /// Width of a rectangle centered on a drag point to allow for limited movement of the mouse pointer before a drag operation begins, in pixels.
        /// </summary>
        SM_CXDRAG = 68,
        /// <summary>
        /// Height of a rectangle centered on a drag point to allow for limited movement of the mouse pointer before a drag operation begins. This value is in pixels. It allows the user to click and release the mouse button easily without unintentionally starting a drag operation.
        /// </summary>
        SM_CYDRAG = 69,
        /// <summary>
        /// Nonzero if the user requires an application to present information visually in situations where it would otherwise present the information only in audible form; zero otherwise.
        /// </summary>
        SM_SHOWSOUNDS = 70,
        /// <summary>
        /// Width of the default menu check-mark bitmap, in pixels.
        /// </summary>
        SM_CXMENUCHECK = 71,
        /// <summary>
        /// Height of the default menu check-mark bitmap, in pixels.
        /// </summary>
        SM_CYMENUCHECK = 72,
        /// <summary>
        /// Nonzero if the computer has a low-end (slow) processor; zero otherwise
        /// </summary>
        SM_SLOWMACHINE = 73,
        /// <summary>
        /// Nonzero if the system is enabled for Hebrew and Arabic languages, zero if not.
        /// </summary>
        SM_MIDEASTENABLED = 74,
        /// <summary>
        /// Nonzero if a mouse is installed; zero otherwise. This value is rarely zero, because of support for virtual mice and because some systems detect the presence of the port instead of the presence of a mouse.
        /// </summary>
        SM_MOUSEPRESENT = 19,
        /// <summary>
        /// Windows 2000 (v5.0+) Coordinate of the top of the virtual screen
        /// </summary>
        SM_XVIRTUALSCREEN = 76,
        /// <summary>
        /// Windows 2000 (v5.0+) Coordinate of the left of the virtual screen
        /// </summary>
        SM_YVIRTUALSCREEN = 77,
        /// <summary>
        /// Windows 2000 (v5.0+) Width of the virtual screen
        /// </summary>
        SM_CXVIRTUALSCREEN = 78,
        /// <summary>
        /// Windows 2000 (v5.0+) Height of the virtual screen
        /// </summary>
        SM_CYVIRTUALSCREEN = 79,
        /// <summary>
        /// Number of display monitors on the desktop
        /// </summary>
        SM_CMONITORS = 80,
        /// <summary>
        /// Windows XP (v5.1+) Nonzero if all the display monitors have the same color format, zero otherwise. Note that two displays can have the same bit depth, but different color formats. For example, the red, green, and blue pixels can be encoded with different numbers of bits, or those bits can be located in different places in a pixel's color value.
        /// </summary>
        SM_SAMEDISPLAYFORMAT = 81,
        /// <summary>
        /// Windows XP (v5.1+) Nonzero if Input Method Manager/Input Method Editor features are enabled; zero otherwise
        /// </summary>
        SM_IMMENABLED = 82,
        /// <summary>
        /// Windows XP (v5.1+) Width of the left and right edges of the focus rectangle drawn by DrawFocusRect. This value is in pixels.
        /// </summary>
        SM_CXFOCUSBORDER = 83,
        /// <summary>
        /// Windows XP (v5.1+) Height of the top and bottom edges of the focus rectangle drawn by DrawFocusRect. This value is in pixels.
        /// </summary>
        SM_CYFOCUSBORDER = 84,
        /// <summary>
        /// Nonzero if the current operating system is the Windows XP Tablet PC edition, zero if not.
        /// </summary>
        SM_TABLETPC = 86,
        /// <summary>
        /// Nonzero if the current operating system is the Windows XP, Media Center Edition, zero if not.
        /// </summary>
        SM_MEDIACENTER = 87,
        /// <summary>
        /// Metrics Other
        /// </summary>
        SM_CMETRICS_OTHER = 76,
        /// <summary>
        /// Metrics Windows 2000
        /// </summary>
        SM_CMETRICS_2000 = 83,
        /// <summary>
        /// Metrics Windows NT
        /// </summary>
        SM_CMETRICS_NT = 88,
        /// <summary>
        /// Windows XP (v5.1+) This system metric is used in a Terminal Services environment. If the calling process is associated with a Terminal Services client session, the return value is nonzero. If the calling process is associated with the Terminal Server console session, the return value is zero. The console session is not necessarily the physical console - see WTSGetActiveConsoleSessionId for more information.
        /// </summary>
        SM_REMOTESESSION = 0x1000,
        /// <summary>
        /// Windows XP (v5.1+) Nonzero if the current session is shutting down; zero otherwise
        /// </summary>
        SM_SHUTTINGDOWN = 0x2000,
        /// <summary>
        /// Windows XP (v5.1+) This system metric is used in a Terminal Services environment. Its value is nonzero if the current session is remotely controlled; zero otherwise
        /// </summary>
        SM_REMOTECONTROL = 0x2001
    }

    public enum RegionResult
    {
        REGION_ERROR = 0,
        REGION_NULLREGION = 1,
        REGION_SIMPLEREGION = 2,
        REGION_COMPLEXREGION = 3
    }

    // See http://msdn.microsoft.com/en-us/library/aa969530(v=vs.85).aspx
    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY,
        DWMWA_TRANSITIONS_FORCEDISABLED,
        DWMWA_ALLOW_NCPAINT,
        DWMWA_CAPTION_BUTTON_BOUNDS,
        DWMWA_NONCLIENT_RTL_LAYOUT,
        DWMWA_FORCE_ICONIC_REPRESENTATION,
        DWMWA_FLIP3D_POLICY,
        DWMWA_EXTENDED_FRAME_BOUNDS, // This is the one we need for retrieving the Window size since Windows Vista
        DWMWA_HAS_ICONIC_BITMAP,        // Since Windows 7
        DWMWA_DISALLOW_PEEK,            // Since Windows 7
        DWMWA_EXCLUDED_FROM_PEEK,       // Since Windows 7
        DWMWA_CLOAK,                    // Since Windows 8
        DWMWA_CLOAKED,                  // Since Windows 8
        DWMWA_FREEZE_REPRESENTATION,    // Since Windows 8
        DWMWA_LAST
    }

    public enum GetWindowCommand : uint
    {
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6
    }

    [Flags]
    public enum DWM_BB
    {
        Enable = 1,
        BlurRegion = 2,
        TransitionMaximized = 4
    }

    public enum ClassLongIndex : int
    {
        GCL_CBCLSEXTRA = -20, // the size, in bytes, of the extra memory associated with the class. Setting this value does not change the number of extra bytes already allocated.
        GCL_CBWNDEXTRA = -18, // the size, in bytes, of the extra window memory associated with each window in the class. Setting this value does not change the number of extra bytes already allocated. For information on how to access this memory, see SetWindowLong.
        GCL_HBRBACKGROUND = -10, // a handle to the background brush associated with the class.
        GCL_HCURSOR = -12, // a handle to the cursor associated with the class.
        GCL_HICON = -14, // a handle to the icon associated with the class.
        GCL_HICONSM = -34, // a handle to the small icon associated with the class.
        GCL_HMODULE = -16, // a handle to the module that registered the class.
        GCL_MENUNAME = -8, // the address of the menu name string. The string identifies the menu resource associated with the class.
        GCL_STYLE = -26, // the window-class style bits.
        GCL_WNDPROC = -24, // the address of the window procedure, or a handle representing the address of the window procedure. You must use the CallWindowProc function to call the window procedure.
    }

    public enum WindowsMessages : int
    {
        WM_NULL = 0x0000,
        WM_CREATE = 0x0001,
        WM_DESTROY = 0x0002,
        WM_MOVE = 0x0003,
        WM_SIZE = 0x0005,
        WM_ACTIVATE = 0x0006,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_ENABLE = 0x000A,
        WM_SETREDRAW = 0x000B,
        WM_SETTEXT = 0x000C,
        WM_GETTEXT = 0x000D,
        WM_GETTEXTLENGTH = 0x000E,
        WM_PAINT = 0x000F,
        WM_CLOSE = 0x0010,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUIT = 0x0012,
        WM_QUERYOPEN = 0x0013,
        WM_ERASEBKGND = 0x0014,
        WM_SYSCOLORCHANGE = 0x0015,
        WM_ENDSESSION = 0x0016,
        WM_SHOWWINDOW = 0x0018,
        WM_WININICHANGE = 0x001A,
        WM_SETTINGCHANGE = 0x001A,
        WM_DEVMODECHANGE = 0x001B,
        WM_ACTIVATEAPP = 0x001C,
        WM_FONTCHANGE = 0x001D,
        WM_TIMECHANGE = 0x001E,
        WM_CANCELMODE = 0x001F,
        WM_SETCURSOR = 0x0020,
        WM_MOUSEACTIVATE = 0x0021,
        WM_CHILDACTIVATE = 0x0022,
        WM_QUEUESYNC = 0x0023,
        WM_GETMINMAXINFO = 0x0024,
        WM_PAINTICON = 0x0026,
        WM_ICONERASEBKGND = 0x0027,
        WM_NEXTDLGCTL = 0x0028,
        WM_SPOOLERSTATUS = 0x002A,
        WM_DRAWITEM = 0x002B,
        WM_MEASUREITEM = 0x002C,
        WM_DELETEITEM = 0x002D,
        WM_VKEYTOITEM = 0x002E,
        WM_CHARTOITEM = 0x002F,
        WM_SETFONT = 0x0030,
        WM_GETFONT = 0x0031,
        WM_SETHOTKEY = 0x0032,
        WM_GETHOTKEY = 0x0033,
        WM_QUERYDRAGICON = 0x0037,
        WM_COMPAREITEM = 0x0039,
        WM_GETOBJECT = 0x003D,
        WM_COMPACTING = 0x0041,
        WM_COMMNOTIFY = 0x0044,
        WM_WINDOWPOSCHANGING = 0x0046,
        WM_WINDOWPOSCHANGED = 0x0047,
        WM_POWER = 0x0048,
        WM_COPYDATA = 0x004A,
        WM_CANCELJOURNAL = 0x004B,
        WM_NOTIFY = 0x004E,
        WM_INPUTLANGCHANGEREQUEST = 0x0050,
        WM_INPUTLANGCHANGE = 0x0051,
        WM_TCARD = 0x0052,
        WM_HELP = 0x0053,
        WM_USERCHANGED = 0x0054,
        WM_NOTIFYFORMAT = 0x0055,
        WM_CONTEXTMENU = 0x007B,
        WM_STYLECHANGING = 0x007C,
        WM_STYLECHANGED = 0x007D,
        WM_DISPLAYCHANGE = 0x007E,
        WM_GETICON = 0x007F,
        WM_SETICON = 0x0080,
        WM_NCCREATE = 0x0081,
        WM_NCDESTROY = 0x0082,
        WM_NCCALCSIZE = 0x0083,
        WM_NCHITTEST = 0x0084,
        WM_NCPAINT = 0x0085,
        WM_NCACTIVATE = 0x0086,
        WM_GETDLGCODE = 0x0087,
        WM_SYNCPAINT = 0x0088,
        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMBUTTONDBLCLK = 0x00A9,
        WM_KEYFIRST = 0x0100,
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_CHAR = 0x0102,
        WM_DEADCHAR = 0x0103,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,
        WM_SYSCHAR = 0x0106,
        WM_SYSDEADCHAR = 0x0107,
        WM_KEYLAST = 0x0108,
        WM_IME_STARTCOMPOSITION = 0x010D,
        WM_IME_ENDCOMPOSITION = 0x010E,
        WM_IME_COMPOSITION = 0x010F,
        WM_IME_KEYLAST = 0x010F,
        WM_INITDIALOG = 0x0110,
        WM_COMMAND = 0x0111,
        WM_SYSCOMMAND = 0x0112,
        WM_TIMER = 0x0113,
        WM_HSCROLL = 0x0114,
        WM_VSCROLL = 0x0115,
        WM_INITMENU = 0x0116,
        WM_INITMENUPOPUP = 0x0117,
        WM_MENUSELECT = 0x011F,
        WM_MENUCHAR = 0x0120,
        WM_ENTERIDLE = 0x0121,
        WM_MENURBUTTONUP = 0x0122,
        WM_MENUDRAG = 0x0123,
        WM_MENUGETOBJECT = 0x0124,
        WM_UNINITMENUPOPUP = 0x0125,
        WM_MENUCOMMAND = 0x0126,
        WM_CTLCOLORMSGBOX = 0x0132,
        WM_CTLCOLOREDIT = 0x0133,
        WM_CTLCOLORLISTBOX = 0x0134,
        WM_CTLCOLORBTN = 0x0135,
        WM_CTLCOLORDLG = 0x0136,
        WM_CTLCOLORSCROLLBAR = 0x0137,
        WM_CTLCOLORSTATIC = 0x0138,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEFIRST = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_MOUSEWHEEL = 0x020A,
        WM_MOUSELAST = 0x020A,
        WM_PARENTNOTIFY = 0x0210,
        WM_ENTERMENULOOP = 0x0211,
        WM_EXITMENULOOP = 0x0212,
        WM_SIZING = 0x0214,
        WM_CAPTURECHANGED = 0x0215,
        WM_MOVING = 0x0216,
        WM_POWERBROADCAST = 0x0218,
        WM_DEVICECHANGE = 0x0219,
        WM_MDICREATE = 0x0220,
        WM_MDIDESTROY = 0x0221,
        WM_MDIACTIVATE = 0x0222,
        WM_MDIRESTORE = 0x0223,
        WM_MDINEXT = 0x0224,
        WM_MDIMAXIMIZE = 0x0225,
        WM_MDITILE = 0x0226,
        WM_MDICASCADE = 0x0227,
        WM_MDIICONARRANGE = 0x0228,
        WM_MDIGETACTIVE = 0x0229,
        WM_MDISETMENU = 0x0230,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_EXITSIZEMOVE = 0x0232,
        WM_DROPFILES = 0x0233,
        WM_MDIREFRESHMENU = 0x0234,
        WM_IME_SETCONTEXT = 0x0281,
        WM_IME_NOTIFY = 0x0282,
        WM_IME_CONTROL = 0x0283,
        WM_IME_COMPOSITIONFULL = 0x0284,
        WM_IME_SELECT = 0x0285,
        WM_IME_CHAR = 0x0286,
        WM_IME_REQUEST = 0x0288,
        WM_IME_KEYDOWN = 0x0290,
        WM_IME_KEYUP = 0x0291,
        WM_NCMOUSEHOVER = 0x02A0,
        WM_MOUSEHOVER = 0x02A1,
        WM_NCMOUSELEAVE = 0x02A2,
        WM_MOUSELEAVE = 0x02A3,
        WM_CUT = 0x0300,
        WM_COPY = 0x0301,
        WM_PASTE = 0x0302,
        WM_CLEAR = 0x0303,
        WM_UNDO = 0x0304,
        WM_RENDERFORMAT = 0x0305,
        WM_RENDERALLFORMATS = 0x0306,
        WM_DESTROYCLIPBOARD = 0x0307,
        WM_DRAWCLIPBOARD = 0x0308,
        WM_PAINTCLIPBOARD = 0x0309,
        WM_VSCROLLCLIPBOARD = 0x030A,
        WM_SIZECLIPBOARD = 0x030B,
        WM_ASKCBFORMATNAME = 0x030C,
        WM_CHANGECBCHAIN = 0x030D,
        WM_HSCROLLCLIPBOARD = 0x030E,
        WM_QUERYNEWPALETTE = 0x030F,
        WM_PALETTEISCHANGING = 0x0310,
        WM_PALETTECHANGED = 0x0311,
        WM_HOTKEY = 0x0312,
        WM_PRINT = 0x0317,
        WM_PRINTCLIENT = 0x0318,
        WM_HANDHELDFIRST = 0x0358,
        WM_HANDHELDLAST = 0x035F,
        WM_AFXFIRST = 0x0360,
        WM_AFXLAST = 0x037F,
        WM_PENWINFIRST = 0x0380,
        WM_PENWINLAST = 0x038F,
        WM_APP = 0x8000,
        WM_USER = 0x0400
    }

    // Get/Set WindowLong Enum See: http://msdn.microsoft.com/en-us/library/ms633591.aspx
    public enum WindowLongIndex : int
    {
        GWL_EXSTYLE = -20,	// Sets a new extended window style.
        GWL_HINSTANCE = -6,	// Sets a new application instance handle.
        GWL_ID = -12,	// Sets a new identifier of the child window. The window cannot be a top-level window.
        GWL_STYLE = -16,	// Sets a new window style.
        GWL_USERDATA = -21,	// Sets the user data associated with the window. This data is intended for use by the application that created the window. Its value is initially zero.
        GWL_WNDPROC = -4 // Sets a new address for the window procedure. You cannot change this attribute if the window does not belong to the same process as the calling thread.
    }

    // See: http://msdn.microsoft.com/en-us/library/ms633545.aspx
    [Flags]
    public enum WindowPos : int
    {
        SWP_ASYNCWINDOWPOS = 0x4000,	// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
        SWP_DEFERERASE = 0x2000,	// Prevents generation of the WM_SYNCPAINT message.
        SWP_DRAWFRAME = 0x0020,	 // Draws a frame (defined in the window's class description) around the window.
        SWP_FRAMECHANGED = 0x0020, //Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
        SWP_HIDEWINDOW = 0x0080,	// Hides the window.
        SWP_NOACTIVATE = 0x0010,	// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
        SWP_NOCOPYBITS = 0x0100,	// Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
        SWP_NOMOVE = 0x0002,	//Retains the current position (ignores X and Y parameters).
        SWP_NOOWNERZORDER = 0x0200,	//Does not change the owner window's position in the Z order.
        SWP_NOREDRAW = 0x0008,	//Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
        SWP_NOREPOSITION = 0x0200,	// Same as the SWP_NOOWNERZORDER flag.
        SWP_NOSENDCHANGING = 0x0400,	//Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
        SWP_NOSIZE = 0x0001,	// Retains the current size (ignores the cx and cy parameters).
        SWP_NOZORDER = 0x0004,	// Retains the current Z order (ignores the hWndInsertAfter parameter).
        SWP_SHOWWINDOW = 0x0040	//Displays the window.
    }

    public enum ScrollBarDirection : int
    {
        SB_HORZ = 0,
        SB_VERT = 1,
        SB_CTL = 2,
        SB_BOTH = 3
    }
    public enum ScrollbarCommand : int
    {
        SB_LINEUP = 0, // Scrolls one line up.
        SB_LINEDOWN = 1, // Scrolls one line down.
        SB_PAGEUP = 2, // Scrolls one page up.
        SB_PAGEDOWN = 3, // Scrolls one page down.
        SB_THUMBPOSITION = 4, // The user has dragged the scroll box (thumb) and released the mouse button. The high-order word indicates the position of the scroll box at the end of the drag operation.
        SB_THUMBTRACK = 5, // The user is dragging the scroll box. This message is sent repeatedly until the user releases the mouse button. The high-order word indicates the position that the scroll box has been dragged to.
        SB_TOP = 6, // Scrolls to the upper left.
        SB_BOTTOM = 7, // Scrolls to the lower right.
        SB_ENDSCROLL = 8 // Ends scroll.
    }

    public enum ScrollInfoMask
    {
        SIF_RANGE = 0x1,
        SIF_PAGE = 0x2,
        SIF_POS = 0x4,
        SIF_DISABLENOSCROLL = 0x8,
        SIF_TRACKPOS = 0x10,
        SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
    }

    /// <summary>
    /// See: http://www.pinvoke.net/default.aspx/Enums/SendMessageTimeoutFlags.html
    /// </summary>
    [Flags]
    public enum SendMessageTimeoutFlags : uint
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8
    }

    [Flags]
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VMOperation = 0x00000008,
        VMRead = 0x00000010,
        VMWrite = 0x00000020,
        DupHandle = 0x00000040,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        Synchronize = 0x00100000
    }

    /// <summary>
    /// See: http://msdn.microsoft.com/en-us/library/aa909766.aspx
    /// </summary>
    [Flags]
    public enum SoundFlags : int
    {
        SND_SYNC = 0x0000,			// play synchronously (default)
        SND_ASYNC = 0x0001,			// play asynchronously
        SND_NODEFAULT = 0x0002,		// silence (!default) if sound not found
        SND_MEMORY = 0x0004,		// pszSound points to a memory file
        SND_LOOP = 0x0008,			// loop the sound until next sndPlaySound
        SND_NOSTOP = 0x0010,		// don't stop any currently playing sound
        SND_NOWAIT = 0x00002000,	// don't wait if the driver is busy
        SND_ALIAS = 0x00010000,		// name is a registry alias
        SND_ALIAS_ID = 0x00110000,	// alias is a predefined id
        SND_FILENAME = 0x00020000,	// name is file name
    }

    /// <summary>
    /// Used by GDI32.GetDeviceCaps
    /// See: http://msdn.microsoft.com/en-us/library/windows/desktop/dd144877%28v=vs.85%29.aspx
    /// </summary>
    public enum DeviceCaps
    {
        /// <summary>
        /// Device driver version
        /// </summary>
        DRIVERVERSION = 0,
        /// <summary>
        /// Device classification
        /// </summary>
        TECHNOLOGY = 2,
        /// <summary>
        /// Horizontal size in millimeters
        /// </summary>
        HORZSIZE = 4,
        /// <summary>
        /// Vertical size in millimeters
        /// </summary>
        VERTSIZE = 6,
        /// <summary>
        /// Horizontal width in pixels
        /// </summary>
        HORZRES = 8,
        /// <summary>
        /// Vertical height in pixels
        /// </summary>
        VERTRES = 10,
        /// <summary>
        /// Number of bits per pixel
        /// </summary>
        BITSPIXEL = 12,
        /// <summary>
        /// Number of planes
        /// </summary>
        PLANES = 14,
        /// <summary>
        /// Number of brushes the device has
        /// </summary>
        NUMBRUSHES = 16,
        /// <summary>
        /// Number of pens the device has
        /// </summary>
        NUMPENS = 18,
        /// <summary>
        /// Number of markers the device has
        /// </summary>
        NUMMARKERS = 20,
        /// <summary>
        /// Number of fonts the device has
        /// </summary>
        NUMFONTS = 22,
        /// <summary>
        /// Number of colors the device supports
        /// </summary>
        NUMCOLORS = 24,
        /// <summary>
        /// Size required for device descriptor
        /// </summary>
        PDEVICESIZE = 26,
        /// <summary>
        /// Curve capabilities
        /// </summary>
        CURVECAPS = 28,
        /// <summary>
        /// Line capabilities
        /// </summary>
        LINECAPS = 30,
        /// <summary>
        /// Polygonal capabilities
        /// </summary>
        POLYGONALCAPS = 32,
        /// <summary>
        /// Text capabilities
        /// </summary>
        TEXTCAPS = 34,
        /// <summary>
        /// Clipping capabilities
        /// </summary>
        CLIPCAPS = 36,
        /// <summary>
        /// Bitblt capabilities
        /// </summary>
        RASTERCAPS = 38,
        /// <summary>
        /// Length of the X leg
        /// </summary>
        ASPECTX = 40,
        /// <summary>
        /// Length of the Y leg
        /// </summary>
        ASPECTY = 42,
        /// <summary>
        /// Length of the hypotenuse
        /// </summary>
        ASPECTXY = 44,
        /// <summary>
        /// Shading and Blending caps
        /// </summary>
        SHADEBLENDCAPS = 45,

        /// <summary>
        /// Logical pixels inch in X
        /// </summary>
        LOGPIXELSX = 88,
        /// <summary>
        /// Logical pixels inch in Y
        /// </summary>
        LOGPIXELSY = 90,

        /// <summary>
        /// Number of entries in physical palette
        /// </summary>
        SIZEPALETTE = 104,
        /// <summary>
        /// Number of reserved entries in palette
        /// </summary>
        NUMRESERVED = 106,
        /// <summary>
        /// Actual color resolution
        /// </summary>
        COLORRES = 108,

        // Printing related DeviceCaps. These replace the appropriate Escapes
        /// <summary>
        /// Physical Width in device units
        /// </summary>
        PHYSICALWIDTH = 110,
        /// <summary>
        /// Physical Height in device units
        /// </summary>
        PHYSICALHEIGHT = 111,
        /// <summary>
        /// Physical Printable Area x margin
        /// </summary>
        PHYSICALOFFSETX = 112,
        /// <summary>
        /// Physical Printable Area y margin
        /// </summary>
        PHYSICALOFFSETY = 113,
        /// <summary>
        /// Scaling factor x
        /// </summary>
        SCALINGFACTORX = 114,
        /// <summary>
        /// Scaling factor y
        /// </summary>
        SCALINGFACTORY = 115,

        /// <summary>
        /// Current vertical refresh rate of the display device (for displays only) in Hz
        /// </summary>
        VREFRESH = 116,
        /// <summary>
        /// Horizontal width of entire desktop in pixels
        /// </summary>
        DESKTOPVERTRES = 117,
        /// <summary>
        /// Vertical height of entire desktop in pixels
        /// </summary>
        DESKTOPHORZRES = 118,
        /// <summary>
        /// Preferred blt alignment
        /// </summary>
        BLTALIGNMENT = 119
    }

    /// <summary>
    /// Used for User32.SetWinEventHook
    /// See: http://msdn.microsoft.com/en-us/library/windows/desktop/dd373640%28v=vs.85%29.aspx
    /// </summary>
    public enum WinEventHookFlags : int
    {
        WINEVENT_SKIPOWNTHREAD = 1,
        WINEVENT_SKIPOWNPROCESS = 2,
        WINEVENT_OUTOFCONTEXT = 0,
        WINEVENT_INCONTEXT = 4
    }

    /// <summary>
    /// Used for User32.SetWinEventHook
    /// See MSDN: http://msdn.microsoft.com/en-us/library/windows/desktop/dd318066%28v=vs.85%29.aspx
    /// </summary>
    public enum WinEvent : uint
    {
        EVENT_OBJECT_ACCELERATORCHANGE = 32786,
        EVENT_OBJECT_CREATE = 32768,
        EVENT_OBJECT_DESTROY = 32769,
        EVENT_OBJECT_DEFACTIONCHANGE = 32785,
        EVENT_OBJECT_DESCRIPTIONCHANGE = 32781,
        EVENT_OBJECT_FOCUS = 32773,
        EVENT_OBJECT_HELPCHANGE = 32784,
        EVENT_OBJECT_SHOW = 32770,
        EVENT_OBJECT_HIDE = 32771,
        EVENT_OBJECT_LOCATIONCHANGE = 32779,
        EVENT_OBJECT_NAMECHANGE = 32780,
        EVENT_OBJECT_PARENTCHANGE = 32783,
        EVENT_OBJECT_REORDER = 32772,
        EVENT_OBJECT_SELECTION = 32774,
        EVENT_OBJECT_SELECTIONADD = 32775,
        EVENT_OBJECT_SELECTIONREMOVE = 32776,
        EVENT_OBJECT_SELECTIONWITHIN = 32777,
        EVENT_OBJECT_STATECHANGE = 32778,
        EVENT_OBJECT_VALUECHANGE = 32782,
        EVENT_SYSTEM_ALERT = 2,
        EVENT_SYSTEM_CAPTUREEND = 9,
        EVENT_SYSTEM_CAPTURESTART = 8,
        EVENT_SYSTEM_CONTEXTHELPEND = 13,
        EVENT_SYSTEM_CONTEXTHELPSTART = 12,
        EVENT_SYSTEM_DIALOGEND = 17,
        EVENT_SYSTEM_DIALOGSTART = 16,
        EVENT_SYSTEM_DRAGDROPEND = 15,
        EVENT_SYSTEM_DRAGDROPSTART = 14,
        EVENT_SYSTEM_FOREGROUND = 3,
        EVENT_SYSTEM_MENUEND = 5,
        EVENT_SYSTEM_MENUPOPUPEND = 7,
        EVENT_SYSTEM_MENUPOPUPSTART = 6,
        EVENT_SYSTEM_MENUSTART = 4,
        EVENT_SYSTEM_MINIMIZEEND = 23,
        EVENT_SYSTEM_MINIMIZESTART = 22,
        EVENT_SYSTEM_MOVESIZEEND = 11,
        EVENT_SYSTEM_MOVESIZESTART = 10,
        EVENT_SYSTEM_SCROLLINGEND = 19,
        EVENT_SYSTEM_SCROLLINGSTART = 18,
        EVENT_SYSTEM_SOUND = 1,
        EVENT_SYSTEM_SWITCHEND = 21,
        EVENT_SYSTEM_SWITCHSTART = 20
    }

    /// <summary>
    /// Used for User32.SetWinEventHook
    /// See: http://msdn.microsoft.com/en-us/library/windows/desktop/dd373606%28v=vs.85%29.aspx#OBJID_WINDOW
    /// </summary>
    public enum EventObjects : int
    {
        OBJID_ALERT = -10,
        OBJID_CARET = -8,
        OBJID_CLIENT = -4,
        OBJID_CURSOR = -9,
        OBJID_HSCROLL = -6,
        OBJID_MENU = -3,
        OBJID_SIZEGRIP = -7,
        OBJID_SOUND = -11,
        OBJID_SYSMENU = -1,
        OBJID_TITLEBAR = -2,
        OBJID_VSCROLL = -5,
        OBJID_WINDOW = 0
    }
}