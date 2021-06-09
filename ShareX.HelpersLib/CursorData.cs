#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
    public class CursorData
    {
        public IntPtr Handle { get; private set; }
        public Point Position { get; private set; }
        public bool IsVisible { get; private set; }

        public CursorData()
        {
            UpdateCursorData();
        }

        public void UpdateCursorData()
        {
            Handle = IntPtr.Zero;
            Position = Point.Empty;
            IsVisible = false;

            CursorInfo cursorInfo = new CursorInfo();
            cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);

            if (NativeMethods.GetCursorInfo(out cursorInfo))
            {
                Handle = cursorInfo.hCursor;
                Position = cursorInfo.ptScreenPos;
                IsVisible = cursorInfo.flags == NativeConstants.CURSOR_SHOWING;

                if (IsVisible)
                {
                    IntPtr iconHandle = NativeMethods.CopyIcon(Handle);

                    if (iconHandle != IntPtr.Zero)
                    {
                        if (NativeMethods.GetIconInfo(iconHandle, out IconInfo iconInfo))
                        {
                            Position = new Point(Position.X - iconInfo.xHotspot, Position.Y - iconInfo.yHotspot);

                            if (iconInfo.hbmMask != IntPtr.Zero)
                            {
                                NativeMethods.DeleteObject(iconInfo.hbmMask);
                            }

                            if (iconInfo.hbmColor != IntPtr.Zero)
                            {
                                NativeMethods.DeleteObject(iconInfo.hbmColor);
                            }
                        }

                        NativeMethods.DestroyIcon(iconHandle);
                    }
                }
            }
        }

        public void DrawCursor(Image img)
        {
            DrawCursor(img, Point.Empty);
        }

        public void DrawCursor(Image img, Point offset)
        {
            if (IsVisible)
            {
                Point drawPosition = new Point(Position.X - offset.X, Position.Y - offset.Y);
                drawPosition = CaptureHelpers.ScreenToClient(drawPosition);

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

        public void DrawCursor(IntPtr hdcDest, Point offset)
        {
            if (IsVisible)
            {
                Point drawPosition = new Point(Position.X - offset.X, Position.Y - offset.Y);
                drawPosition = CaptureHelpers.ScreenToClient(drawPosition);

                NativeMethods.DrawIconEx(hdcDest, drawPosition.X, drawPosition.Y, Handle, 0, 0, 0, IntPtr.Zero, NativeConstants.DI_NORMAL);
            }
        }
    }
}