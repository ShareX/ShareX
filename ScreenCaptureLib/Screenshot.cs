#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ScreenCaptureLib
{
    public static partial class Screenshot
    {
        public static bool RemoveOutsideScreenArea = true;
        public static bool CaptureCursor = false;
        public static bool CaptureClientArea = false;
        public static bool CaptureShadow = true;
        public static int ShadowOffset = 20;
        public static bool AutoHideTaskbar = false;

        public static Image CaptureRectangle(Rectangle rect)
        {
            if (RemoveOutsideScreenArea)
            {
                Rectangle bounds = CaptureHelpers.GetScreenBounds();
                rect = Rectangle.Intersect(bounds, rect);
            }

            return CaptureRectangleNative(rect, CaptureCursor);
        }

        public static Image CaptureFullscreen()
        {
            Rectangle bounds = CaptureHelpers.GetScreenBounds();

            return CaptureRectangle(bounds);
        }

        public static Image CaptureWindow(IntPtr handle)
        {
            if (handle.ToInt32() > 0)
            {
                Rectangle rect;

                if (CaptureClientArea)
                {
                    rect = NativeMethods.GetClientRect(handle);
                }
                else
                {
                    rect = CaptureHelpers.GetWindowRectangle(handle);
                }

                bool isTaskbarHide = false;

                try
                {
                    if (AutoHideTaskbar)
                    {
                        isTaskbarHide = NativeMethods.SetTaskbarVisibilityIfIntersect(false, rect);
                    }

                    return CaptureRectangle(rect);
                }
                finally
                {
                    if (isTaskbarHide)
                    {
                        NativeMethods.SetTaskbarVisibility(true);
                    }
                }
            }

            return null;
        }

        public static Image CaptureActiveWindow()
        {
            IntPtr handle = NativeMethods.GetForegroundWindow();

            return CaptureWindow(handle);
        }

        public static Image CaptureActiveMonitor()
        {
            Rectangle bounds = CaptureHelpers.GetActiveScreenBounds();

            return CaptureRectangle(bounds);
        }

        public static Image CaptureRectangleNative(Rectangle rect, bool captureCursor = false)
        {
            return CaptureRectangleNative(NativeMethods.GetDesktopWindow(), rect, captureCursor);
        }

        public static Image CaptureRectangleNative(IntPtr handle, Rectangle rect, bool captureCursor = false)
        {
            if (rect.Width == 0 || rect.Height == 0)
            {
                return null;
            }

            IntPtr hdcSrc = NativeMethods.GetWindowDC(handle);
            IntPtr hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, rect.Width, rect.Height);
            IntPtr hOld = NativeMethods.SelectObject(hdcDest, hBitmap);
            NativeMethods.BitBlt(hdcDest, 0, 0, rect.Width, rect.Height, hdcSrc, rect.X, rect.Y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);

            if (captureCursor)
            {
                Point cursorOffset = CaptureHelpers.ScreenToClient(rect.Location);

                try
                {
                    using (CursorData cursorData = new CursorData())
                    {
                        cursorData.DrawCursorToHandle(hdcDest, cursorOffset);
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, "Cursor capture failed.");
                }
            }

            NativeMethods.SelectObject(hdcDest, hOld);
            NativeMethods.DeleteDC(hdcDest);
            NativeMethods.ReleaseDC(handle, hdcSrc);
            Image img = Image.FromHbitmap(hBitmap);
            NativeMethods.DeleteObject(hBitmap);

            return img;
        }

        public static Image CaptureRectangleManaged(Rectangle rect)
        {
            if (rect.Width == 0 || rect.Height == 0)
            {
                return null;
            }

            Image img = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(img))
            {
                // Managed can't use SourceCopy | CaptureBlt because of .NET bug
                g.CopyFromScreen(rect.Location, Point.Empty, rect.Size, CopyPixelOperation.SourceCopy);
            }

            return img;
        }
    }
}