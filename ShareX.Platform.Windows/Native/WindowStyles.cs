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

namespace ShareX.Platform.Windows.Native;

/// <summary>
/// Window Styles
/// The following styles can be specified wherever a window style is required. After the control has been created, these styles cannot be modified, except as noted.
/// </summary>
[Flags]
internal enum WindowStyles : uint
{
    /// <summary>
    /// Creates an overlapped window. An overlapped window usually has a caption and a border.
    /// </summary>
    WS_OVERLAPPED = 0x00000000,

    /// <summary>
    /// Creates a pop-up window. Cannot be used with the <see cref="WS_CHILD"/> style.
    /// </summary>
    WS_POPUP = 0x80000000,

    /// <summary>
    /// Creates a child window. Cannot be used with the <see cref="WS_POPUP"/> style.
    /// </summary>
    WS_CHILD = 0x40000000,

    /// <summary>
    /// Creates a window that is initially minimized. For use with the <see cref="WS_OVERLAPPED"/> style only.
    /// </summary>
    WS_MINIMIZE = 0x20000000,

    /// <summary>
    /// Creates a window that is initially visible.
    /// </summary>
    WS_VISIBLE = 0x10000000,

    /// <summary>
    /// Creates a window that is initially disabled.
    /// </summary>
    WS_DISABLED = 0x08000000,

    /// <summary>
    /// Clips child windows relative to each other; that is, when a particular child window receives a paint message, the WS_CLIPSIBLINGS style clips all other overlapped child windows out of the region of the child window to be updated. (If WS_CLIPSIBLINGS is not given and child windows overlap, when you draw within the client area of a child window, it is possible to draw within the client area of a neighboring child window.) For use with the <see cref="WS_CHILD"/> style only.
    /// </summary>
    WS_CLIPSIBLINGS = 0x04000000,

    /// <summary>
    /// Excludes the area occupied by child windows when you draw within the parent window.
    /// Used when you create the parent window.
    /// </summary>
    WS_CLIPCHILDREN = 0x02000000,

    /// <summary>
    /// Creates a window of maximum size.
    /// </summary>
    WS_MAXIMIZE = 0x01000000,

    /// <summary>
    /// Creates a window that has a border.
    /// </summary>
    WS_BORDER = 0x00800000,

    /// <summary>
    /// Creates a window with a double border but no title.
    /// </summary>
    WS_DLGFRAME = 0x00400000,

    /// <summary>
    /// Creates a window that has a vertical scroll bar.
    /// </summary>
    WS_VSCROLL = 0x00200000,

    /// <summary>
    /// Creates a window that has a horizontal scroll bar.
    /// </summary>
    WS_HSCROLL = 0x00100000,

    /// <summary>
    /// Creates a window that has a Control-menu box in its title bar. Used only for windows with title bars.
    /// </summary>
    WS_SYSMENU = 0x00080000,

    /// <summary>
    /// Creates a window with a thick frame that can be used to size the window.
    /// </summary>
    WS_THICKFRAME = 0x00040000,

    /// <summary>
    /// Specifies the first control of a group of controls in which the user can move from one control to the next with the arrow keys. All controls defined with the WS_GROUP style FALSE after the first control belong to the same group. The next control with the WS_GROUP style starts the next group (that is, one group ends where the next begins).
    /// </summary>
    WS_GROUP = 0x00020000,

    /// <summary>
    /// Specifies one of any number of controls through which the user can move by using the TAB key. The TAB key moves the user to the next control specified by the WS_TABSTOP style.
    /// </summary>
    WS_TABSTOP = 0x00010000,

    /// <summary>
    /// Creates a window that has a Minimize button.
    /// </summary>
    WS_MINIMIZEBOX = 0x00020000,

    /// <summary>
    /// Creates a window that has a Maximize button.
    /// </summary>
    WS_MAXIMIZEBOX = 0x00010000,

    /// <summary>
    /// Creates a window that has a title bar (implies the <see cref="WS_BORDER"/> style).
    /// Cannot be used with the <see cref="WS_DLGFRAME"/> style.
    /// </summary>
    WS_CAPTION = WS_BORDER | WS_DLGFRAME,

    /// <summary>
    /// Creates an overlapped window. An overlapped window has a title bar and a border. Same as the <see cref="WS_OVERLAPPED"/> style.
    /// </summary>
    WS_TILED = WS_OVERLAPPED,

    /// <summary>
    /// Creates a window that is initially minimized. Same as the <see cref="WS_MINIMIZE"/> style.
    /// </summary>
    WS_ICONIC = WS_MINIMIZE,

    /// <summary>
    /// Creates a window that has a sizing border. Same as the <see cref="WS_THICKFRAME"/> style.
    /// </summary>
    WS_SIZEBOX = WS_THICKFRAME,

    /// <summary>
    /// Creates an overlapped window with the <see cref="WS_OVERLAPPED"/>, <see cref="WS_CAPTION"/>, <see cref="WS_SYSMENU"/>, <see cref="WS_THICKFRAME"/>, <see cref="WS_MINIMIZEBOX"/>, and <see cref="WS_MAXIMIZEBOX"/> styles. Same as the <see cref="WS_OVERLAPPEDWINDOW"/> style.
    /// </summary>
    WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

    /// <summary>
    /// Creates an overlapped window with the <see cref="WS_OVERLAPPED"/>, <see cref="WS_CAPTION"/>, <see cref="WS_SYSMENU"/>, <see cref="WS_THICKFRAME"/>, <see cref="WS_MINIMIZEBOX"/>, and <see cref="WS_MAXIMIZEBOX"/> styles.
    /// </summary>
    WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

    /// <summary>
    /// Creates a pop-up window with the <see cref="WS_BORDER"/>, <see cref="WS_POPUP"/>, and <see cref="WS_SYSMENU"/> styles. The WS_CAPTION style must be combined with the <see cref="WS_POPUPWINDOW"/> style to make the Control menu visible.
    /// </summary>
    WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

    /// <summary>
    /// Same as the <see cref="WS_CHILD"/> style.
    /// </summary>
    WS_CHILDWINDOW = WS_CHILD,

    //Extended Window Styles

    /// <summary>
    /// Designates a window with a double border that may (optionally) be created with a title bar when you specify the <see cref="WS_CAPTION"/> style flag in the dwStyle parameter.
    /// </summary>
    WS_EX_DLGMODALFRAME = 0x00000001,

    /// <summary>
    /// Specifies that a child window created with this style will not send the <see cref="WM_PARENTNOTIFY"/> message to its parent window when the child window is created or destroyed.
    /// </summary>
    WS_EX_NOPARENTNOTIFY = 0x00000004,

    /// <summary>
    /// Specifies that a window created with this style should be placed above all nontopmost windows and stay above them even when the window is deactivated. An application can use the <see cref="SetWindowPos"/> member function to add or remove this attribute.
    /// </summary>
    WS_EX_TOPMOST = 0x00000008,

    /// <summary>
    /// Specifies that a window created with this style accepts drag-and-drop files.
    /// </summary>
    WS_EX_ACCEPTFILES = 0x00000010,

    /// <summary>
    /// Specifies that a window created with this style is to be transparent. That is, any windows that are beneath the window are not obscured by the window. A window created with this style receives <see cref="WM_PAINT"/> messages only after all sibling windows beneath it have been updated.
    /// </summary>
    WS_EX_TRANSPARENT = 0x00000020,

    //#if(WINVER >= 0x0400)

    /// <summary>
    /// Creates an MDI child window.
    /// </summary>
    WS_EX_MDICHILD = 0x00000040,

    /// <summary>
    /// Creates a tool window, which is a window intended to be used as a floating toolbar. A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font. A tool window does not appear in the task bar or in the window that appears when the user presses ALT+TAB.
    /// </summary>
    WS_EX_TOOLWINDOW = 0x00000080,

    /// <summary>
    /// Specifies that a window has a border with a raised edge.
    /// </summary>
    WS_EX_WINDOWEDGE = 0x00000100,

    /// <summary>
    /// Specifies that a window has a 3D look — that is, a border with a sunken edge.
    /// </summary>
    WS_EX_CLIENTEDGE = 0x00000200,

    /// <summary>
    /// Includes a question mark in the title bar of the window. When the user clicks the question mark, the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a <see cref="WM_HELP"/> message.
    /// </summary>
    WS_EX_CONTEXTHELP = 0x00000400,

    /// <summary>
    /// Gives a window generic right-aligned properties. This depends on the window class.
    /// </summary>
    WS_EX_RIGHT = 0x00001000,

    /// <summary>
    /// Gives window generic left-aligned properties. This is the default.
    /// </summary>
    WS_EX_LEFT = 0x00000000,

    /// <summary>
    /// Displays the window text using right-to-left reading order properties.
    /// </summary>
    WS_EX_RTLREADING = 0x00002000,

    /// <summary>
    /// Displays the window text using left-to-right reading order properties. This is the default.
    /// </summary>
    WS_EX_LTRREADING = 0x00000000,

    /// <summary>
    /// Places a vertical scroll bar to the left of the client area.
    /// </summary>
    WS_EX_LEFTSCROLLBAR = 0x00004000,

    /// <summary>
    /// Places a vertical scroll bar (if present) to the right of the client area. This is the default.
    /// </summary>
    WS_EX_RIGHTSCROLLBAR = 0x00000000,

    /// <summary>
    /// Allows the user to navigate among the child windows of the window by using the TAB key.
    /// </summary>
    WS_EX_CONTROLPARENT = 0x00010000,

    /// <summary>
    /// Creates a window with a three-dimensional border style intended to be used for items that do not accept user input.
    /// </summary>
    WS_EX_STATICEDGE = 0x00020000,

    /// <summary>
    /// Forces a top-level window onto the taskbar when the window is visible.
    /// </summary>
    WS_EX_APPWINDOW = 0x00040000,

    /// <summary>
    /// Combines the <see cref="WS_EX_CLIENTEDGE"/> and <see cref="WS_EX_WINDOWEDGE"/> styles.
    /// </summary>
    WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,

    /// <summary>
    /// Combines the <see cref="WS_EX_WINDOWEDGE"/> and <see cref="WS_EX_TOPMOST"/> styles.
    /// </summary>
    WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
    //#endif /* WINVER >= 0x0400 */

    //#if(_WIN32_WINNT >= 0x0500)
    /// <summary>
    /// Windows 2000/XP: Creates a layered window. Note that this cannot be used for child windows. Also, this cannot be used if the window has a class style of either <see cref="CS_OWNDC"/> or <see cref="CS_CLASSDC"/>.
    /// </summary>
    WS_EX_LAYERED = 0x00080000,
    //#endif /* _WIN32_WINNT >= 0x0500 */

    //#if(WINVER >= 0x0500)
    /// <summary>
    /// Windows 2000/XP: A window created with this style does not pass its window layout to its child windows.
    /// </summary>
    WS_EX_NOINHERITLAYOUT = 0x00100000,

    /// <summary>
    /// Arabic and Hebrew versions of Windows 98/Me, Windows 2000/XP: Creates a window whose horizontal origin is on the right edge. Increasing horizontal values advance to the left.
    /// </summary>
    WS_EX_LAYOUTRTL = 0x00400000,
    //#endif /* WINVER >= 0x0500 */

    //#if(_WIN32_WINNT >= 0x0500)
    /// <summary>
    /// Windows XP: Paints all descendants of a window in bottom-to-top painting order using double-buffering. For more information, see Remarks. This cannot be used if the window has a class style of either <see cref="CS_OWNDC"/> or <see cref="CS_CLASSDC"/>.
    /// </summary>
    WS_EX_COMPOSITED = 0x02000000,

    /// <summary>
    /// Windows 2000/XP: A top-level window created with this style does not become the foreground window when the user clicks it. The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
    /// To activate the window, use the <see cref="SetActiveWindow"/> or <see cref="SetForegroundWindow"/> function.
    /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the <see cref="WS_EX_APPWINDOW"/> style.
    /// </summary>
    WS_EX_NOACTIVATE = 0x08000000
    //#endif /* _WIN32_WINNT >= 0x0500 */
}
