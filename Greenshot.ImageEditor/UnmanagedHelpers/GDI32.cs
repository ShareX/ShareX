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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;

namespace GreenshotPlugin.UnmanagedHelpers
{
    public static class GDIExtensions
    {
        /// <summary>
        /// Check if all the corners of the rectangle are visible in the specified region.
        /// Not a perfect check, but this currently a workaround for checking if a window is completely visible
        /// </summary>
        /// <param name="region"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static bool AreRectangleCornersVisisble(this Region region, Rectangle rectangle)
        {
            Point topLeft = new Point(rectangle.X, rectangle.Y);
            Point topRight = new Point(rectangle.X + rectangle.Width, rectangle.Y);
            Point bottomLeft = new Point(rectangle.X, rectangle.Y + rectangle.Height);
            Point bottomRight = new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
            bool topLeftVisible = region.IsVisible(topLeft);
            bool topRightVisible = region.IsVisible(topRight);
            bool bottomLeftVisible = region.IsVisible(bottomLeft);
            bool bottomRightVisible = region.IsVisible(bottomRight);

            return topLeftVisible && topRightVisible && bottomLeftVisible && bottomRightVisible;
        }

        /// <summary>
        /// Get a SafeHandle for the GetHdc, so one can use using to automatically cleanup the devicecontext
        /// </summary>
        /// <param name="graphics"></param>
        /// <returns>SafeDeviceContextHandle</returns>
        public static SafeDeviceContextHandle getSafeDeviceContext(this Graphics graphics)
        {
            return SafeDeviceContextHandle.fromGraphics(graphics);
        }
    }

    /// <summary>
    /// Abstract class SafeObjectHandle which contains all handles that are cleaned with DeleteObject
    /// </summary>
    public abstract class SafeObjectHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [DllImport("gdi32", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        protected SafeObjectHandle(bool ownsHandle)
            : base(ownsHandle)
        {
        }

        protected override bool ReleaseHandle()
        {
            return DeleteObject(handle);
        }
    }

    /// <summary>
    /// A hbitmap SafeHandle implementation
    /// </summary>
    public class SafeHBitmapHandle : SafeObjectHandle
    {
        [SecurityCritical]
        private SafeHBitmapHandle()
            : base(true)
        {
        }

        [SecurityCritical]
        public SafeHBitmapHandle(IntPtr preexistingHandle)
            : base(true)
        {
            SetHandle(preexistingHandle);
        }
    }

    /// <summary>
    /// A hRegion SafeHandle implementation
    /// </summary>
    public class SafeRegionHandle : SafeObjectHandle
    {
        [SecurityCritical]
        private SafeRegionHandle()
            : base(true)
        {
        }

        [SecurityCritical]
        public SafeRegionHandle(IntPtr preexistingHandle)
            : base(true)
        {
            SetHandle(preexistingHandle);
        }
    }

    /// <summary>
    /// A dibsection SafeHandle implementation
    /// </summary>
    public class SafeDibSectionHandle : SafeObjectHandle
    {
        [SecurityCritical]
        private SafeDibSectionHandle()
            : base(true)
        {
        }

        [SecurityCritical]
        public SafeDibSectionHandle(IntPtr preexistingHandle)
            : base(true)
        {
            SetHandle(preexistingHandle);
        }
    }

    /// <summary>
    /// A select object safehandle implementation
    /// This impl will select the passed SafeHandle to the HDC and replace the returned value when disposing
    /// </summary>
    public class SafeSelectObjectHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [DllImport("gdi32", SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        private SafeHandle hdc;

        [SecurityCritical]
        private SafeSelectObjectHandle()
            : base(true)
        {
        }

        [SecurityCritical]
        public SafeSelectObjectHandle(SafeDCHandle hdc, SafeHandle newHandle)
            : base(true)
        {
            this.hdc = hdc;
            SetHandle(SelectObject(hdc.DangerousGetHandle(), newHandle.DangerousGetHandle()));
        }

        protected override bool ReleaseHandle()
        {
            SelectObject(hdc.DangerousGetHandle(), handle);
            return true;
        }
    }

    public abstract class SafeDCHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        protected SafeDCHandle(bool ownsHandle)
            : base(ownsHandle)
        {
        }
    }

    /// <summary>
    /// A CompatibleDC SafeHandle implementation
    /// </summary>
    public class SafeCompatibleDCHandle : SafeDCHandle
    {
        [DllImport("gdi32", SetLastError = true)]
        private static extern bool DeleteDC(IntPtr hDC);

        [SecurityCritical]
        private SafeCompatibleDCHandle()
            : base(true)
        {
        }

        [SecurityCritical]
        public SafeCompatibleDCHandle(IntPtr preexistingHandle)
            : base(true)
        {
            SetHandle(preexistingHandle);
        }

        public SafeSelectObjectHandle SelectObject(SafeHandle newHandle)
        {
            return new SafeSelectObjectHandle(this, newHandle);
        }

        protected override bool ReleaseHandle()
        {
            return DeleteDC(handle);
        }
    }

    /// <summary>
    /// A DeviceContext SafeHandle implementation
    /// </summary>
    public class SafeDeviceContextHandle : SafeDCHandle
    {
        private Graphics graphics = null;

        [SecurityCritical]
        private SafeDeviceContextHandle()
            : base(true)
        {
        }

        [SecurityCritical]
        public SafeDeviceContextHandle(Graphics graphics, IntPtr preexistingHandle)
            : base(true)
        {
            this.graphics = graphics;
            SetHandle(preexistingHandle);
        }

        protected override bool ReleaseHandle()
        {
            graphics.ReleaseHdc(handle);
            return true;
        }

        public SafeSelectObjectHandle SelectObject(SafeHandle newHandle)
        {
            return new SafeSelectObjectHandle(this, newHandle);
        }

        public static SafeDeviceContextHandle fromGraphics(Graphics graphics)
        {
            return new SafeDeviceContextHandle(graphics, graphics.GetHdc());
        }
    }

    /// <summary>
    /// GDI32 Helpers
    /// </summary>
    public static class GDI32
    {
        [DllImport("gdi32", SetLastError = true)]
        public static extern bool BitBlt(SafeHandle hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, SafeHandle hdcSrc, int nXSrc, int nYSrc, CopyPixelOperation dwRop);

        [DllImport("gdi32", SetLastError = true)]
        private static extern bool StretchBlt(SafeHandle hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, SafeHandle hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, CopyPixelOperation dwRop);

        [DllImport("gdi32", SetLastError = true)]
        public static extern SafeCompatibleDCHandle CreateCompatibleDC(SafeHandle hDC);

        [DllImport("gdi32", SetLastError = true)]
        public static extern IntPtr SelectObject(SafeHandle hDC, SafeHandle hObject);

        [DllImport("gdi32", SetLastError = true)]
        public static extern SafeDibSectionHandle CreateDIBSection(SafeHandle hdc, ref BitmapInfoHeader bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32", SetLastError = true)]
        public static extern SafeRegionHandle CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32", SetLastError = true)]
        public static extern uint GetPixel(SafeHandle hdc, int nXPos, int nYPos);

        [DllImport("gdi32", SetLastError = true)]
        public static extern int GetDeviceCaps(SafeHandle hdc, DeviceCaps nIndex);

        /// <summary>
        /// StretchBlt extension for the graphics object
        /// Doesn't work?
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void StretchBlt(this Graphics target, Bitmap sourceBitmap, Rectangle source, Rectangle destination)
        {
            using (SafeDeviceContextHandle targetDC = target.getSafeDeviceContext())
            {
                using (SafeCompatibleDCHandle safeCompatibleDCHandle = CreateCompatibleDC(targetDC))
                {
                    using (SafeHBitmapHandle hBitmapHandle = new SafeHBitmapHandle(sourceBitmap.GetHbitmap()))
                    {
                        using (SafeSelectObjectHandle selectObject = safeCompatibleDCHandle.SelectObject(hBitmapHandle))
                        {
                            StretchBlt(targetDC, destination.X, destination.Y, destination.Width, destination.Height, safeCompatibleDCHandle, source.Left, source.Top, source.Width, source.Height, CopyPixelOperation.SourceCopy);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Bitblt extension for the graphics object
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void BitBlt(this Graphics target, Bitmap sourceBitmap, Rectangle source, Point destination, CopyPixelOperation rop)
        {
            using (SafeDeviceContextHandle targetDC = target.getSafeDeviceContext())
            {
                using (SafeCompatibleDCHandle safeCompatibleDCHandle = CreateCompatibleDC(targetDC))
                {
                    using (SafeHBitmapHandle hBitmapHandle = new SafeHBitmapHandle(sourceBitmap.GetHbitmap()))
                    {
                        using (SafeSelectObjectHandle selectObject = safeCompatibleDCHandle.SelectObject(hBitmapHandle))
                        {
                            BitBlt(targetDC, destination.X, destination.Y, source.Width, source.Height, safeCompatibleDCHandle, source.Left, source.Top, rop);
                        }
                    }
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct BitmapFileHeader
    {
        public static readonly short BM = 0x4d42; // BM
        public short bfType;
        public int bfSize;
        public short bfReserved1;
        public short bfReserved2;
        public int bfOffBits;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfoHeader
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public int biClrImportant;

        private const int BI_RGB = 0;	//Das Bitmap ist nicht komprimiert
        private const int BI_RLE8 = 1; //Das Bitmap ist komprimiert (Für 8-Bit Bitmaps)
        private const int BI_RLE4 = 2; //Das Bitmap ist komprimiert (Für 4-Bit Bitmaps)
        private const int BI_BITFIELDS = 3; //Das Bitmap ist nicht komprimiert. Die Farbtabelle enthält
        public const int DIB_RGB_COLORS = 0;

        public BitmapInfoHeader(int width, int height, short bpp)
        {
            biSize = (uint)Marshal.SizeOf(typeof(BitmapInfoHeader));	// BITMAPINFOHEADER is 40 bytes
            biPlanes = 1;	// Should allways be 1
            biCompression = BI_RGB;
            biWidth = width;
            biHeight = height;
            biBitCount = bpp;
            biSizeImage = 0;
            biXPelsPerMeter = 0;
            biYPelsPerMeter = 0;
            biClrUsed = 0;
            biClrImportant = 0;
        }
    }
}