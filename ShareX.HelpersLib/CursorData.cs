#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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

namespace ShareX.HelpersLib
{
    public class CursorData : IDisposable
    {
        public IntPtr Handle { get; private set; }
        public bool IsVisible { get; private set; }
        public Point Position { get; private set; }

        public bool IsValid => Handle != IntPtr.Zero && IsVisible;

        public CursorData()
        {
            UpdateCursorData();
        }

        public void UpdateCursorData()
        {
            CursorInfo cursorInfo = new CursorInfo();
            cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);

            if (NativeMethods.GetCursorInfo(out cursorInfo))
            {
                IsVisible = cursorInfo.flags == NativeConstants.CURSOR_SHOWING;

                if (IsVisible)
                {
                    Handle = NativeMethods.CopyIcon(cursorInfo.hCursor);
                    IconInfo iconInfo;

                    if (NativeMethods.GetIconInfo(Handle, out iconInfo))
                    {
                        Point cursorPosition = CaptureHelpers.GetZeroBasedMousePosition();
                        Position = new Point(cursorPosition.X - iconInfo.xHotspot, cursorPosition.Y - iconInfo.yHotspot);

                        if (iconInfo.hbmMask != IntPtr.Zero)
                        {
                            NativeMethods.DeleteObject(iconInfo.hbmMask);
                        }

                        if (iconInfo.hbmColor != IntPtr.Zero)
                        {
                            NativeMethods.DeleteObject(iconInfo.hbmColor);
                        }
                    }
                }
            }
        }

        public void DrawCursor(Image img)
        {
            DrawCursor(img, Point.Empty);
        }

        public void DrawCursor(Image img, Point cursorOffset)
        {
            if (IsValid)
            {
                Point drawPosition = new Point(Position.X - cursorOffset.X, Position.Y - cursorOffset.Y);

                using (Graphics g = Graphics.FromImage(img))
                using (Icon icon = Icon.FromHandle(Handle))
                {
                    g.DrawIcon(icon, drawPosition.X, drawPosition.Y);
                }
            }
        }

        public void DrawCursor(IntPtr hdcDest)
        {
            DrawCursor(hdcDest, Point.Empty);
        }

        public void DrawCursor(IntPtr hdcDest, Point cursorOffset)
        {
            if (IsValid)
            {
                Point drawPosition = new Point(Position.X - cursorOffset.X, Position.Y - cursorOffset.Y);

                NativeMethods.DrawIconEx(hdcDest, drawPosition.X, drawPosition.Y, Handle, 0, 0, 0, IntPtr.Zero, NativeConstants.DI_NORMAL);
            }
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                NativeMethods.DestroyIcon(Handle);
                Handle = IntPtr.Zero;
            }
        }
    }
}