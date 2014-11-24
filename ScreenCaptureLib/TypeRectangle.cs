using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using HelpersLib;

namespace ScreenCaptureLib
{
    public class TypeRectangle : iCaptureType
    {
        bool RemoveOutsideScreenArea = Screenshot.RemoveOutsideScreenArea;

        public bool getRemoveOutsideScreenArea() { return RemoveOutsideScreenArea; }
        public void setRemoveOutsideScreenArea(bool x) { RemoveOutsideScreenArea = x; }

        public Image Capture(Rectangle rect, IntPtr handle, bool captureCursor)
        {
            if (RemoveOutsideScreenArea)
            {
                Rectangle bounds = CaptureHelpers.GetScreenBounds();
                rect = Rectangle.Intersect(bounds, rect);
            }

            return CaptureRectangleNative(rect, captureCursor);
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

        internal static Image Capture(Rectangle bounds)
        {
            TypeRectangle tr = new TypeRectangle();
            return tr.Capture(bounds, new IntPtr(0), false);
        }

        public TypeRectangle()
        {
        }
    }
}
