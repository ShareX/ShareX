/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
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
        public static SafeDeviceContextHandle GetSafeDeviceContext(this Graphics graphics)
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

        protected SafeObjectHandle(bool ownsHandle) : base(ownsHandle)
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
        private SafeHBitmapHandle() : base(true)
        {
        }

        [SecurityCritical]
        public SafeHBitmapHandle(IntPtr preexistingHandle) : base(true)
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
        private SafeRegionHandle() : base(true)
        {
        }

        [SecurityCritical]
        public SafeRegionHandle(IntPtr preexistingHandle) : base(true)
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
        private SafeDibSectionHandle() : base(true)
        {
        }

        [SecurityCritical]
        public SafeDibSectionHandle(IntPtr preexistingHandle) : base(true)
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
        private SafeSelectObjectHandle() : base(true)
        {
        }

        [SecurityCritical]
        public SafeSelectObjectHandle(SafeDCHandle hdc, SafeHandle newHandle) : base(true)
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
        protected SafeDCHandle(bool ownsHandle) : base(ownsHandle)
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
        private SafeCompatibleDCHandle() : base(true)
        {
        }

        [SecurityCritical]
        public SafeCompatibleDCHandle(IntPtr preexistingHandle) : base(true)
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
        private SafeDeviceContextHandle() : base(true)
        {
        }

        [SecurityCritical]
        public SafeDeviceContextHandle(Graphics graphics, IntPtr preexistingHandle) : base(true)
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
        public static extern SafeDibSectionHandle CreateDIBSection(SafeHandle hdc, ref BITMAPINFOHEADER bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);

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
            using (SafeDeviceContextHandle targetDC = target.GetSafeDeviceContext())
            {
                using (SafeCompatibleDCHandle safeCompatibleDCHandle = CreateCompatibleDC(targetDC))
                {
                    using (SafeHBitmapHandle hBitmapHandle = new SafeHBitmapHandle(sourceBitmap.GetHbitmap()))
                    {
                        using (safeCompatibleDCHandle.SelectObject(hBitmapHandle))
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
            using (SafeDeviceContextHandle targetDC = target.GetSafeDeviceContext())
            {
                using (SafeCompatibleDCHandle safeCompatibleDCHandle = CreateCompatibleDC(targetDC))
                {
                    using (SafeHBitmapHandle hBitmapHandle = new SafeHBitmapHandle(sourceBitmap.GetHbitmap()))
                    {
                        using (safeCompatibleDCHandle.SelectObject(hBitmapHandle))
                        {
                            BitBlt(targetDC, destination.X, destination.Y, source.Width, source.Height, safeCompatibleDCHandle, source.Left, source.Top, rop);
                        }
                    }
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct BITMAPFILEHEADER
    {
        public static readonly short BM = 0x4d42; // BM
        public short bfType;
        public int bfSize;
        public short bfReserved1;
        public short bfReserved2;
        public int bfOffBits;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BitfieldColorMask
    {
        public uint blue;
        public uint green;
        public uint red;

        public void InitValues()
        {
            red = (uint)255 << 8;
            green = (uint)255 << 16;
            blue = (uint)255 << 24;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CIEXYZ
    {
        public uint ciexyzX; //FXPT2DOT30
        public uint ciexyzY; //FXPT2DOT30
        public uint ciexyzZ; //FXPT2DOT30

        public CIEXYZ(uint FXPT2DOT30)
        {
            ciexyzX = FXPT2DOT30;
            ciexyzY = FXPT2DOT30;
            ciexyzZ = FXPT2DOT30;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CIEXYZTRIPLE
    {
        public CIEXYZ ciexyzRed;
        public CIEXYZ ciexyzGreen;
        public CIEXYZ ciexyzBlue;
    }

    public enum BI_COMPRESSION : uint
    {
        BI_RGB = 0,         // Uncompressed
        BI_RLE8 = 1,        // RLE 8BPP
        BI_RLE4 = 2,        // RLE 4BPP
        BI_BITFIELDS = 3,   // Specifies that the bitmap is not compressed and that the color table consists of three DWORD color masks that specify the red, green, and blue components, respectively, of each pixel. This is valid when used with 16- and 32-bpp bitmaps.
        BI_JPEG = 4,        // Indicates that the image is a JPEG image.
        BI_PNG = 5          // Indicates that the image is a PNG image.
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct BITMAPINFOHEADER
    {
        [FieldOffset(0)]
        public uint biSize;
        [FieldOffset(4)]
        public int biWidth;
        [FieldOffset(8)]
        public int biHeight;
        [FieldOffset(12)]
        public ushort biPlanes;
        [FieldOffset(14)]
        public ushort biBitCount;
        [FieldOffset(16)]
        public BI_COMPRESSION biCompression;
        [FieldOffset(20)]
        public uint biSizeImage;
        [FieldOffset(24)]
        public int biXPelsPerMeter;
        [FieldOffset(28)]
        public int biYPelsPerMeter;
        [FieldOffset(32)]
        public uint biClrUsed;
        [FieldOffset(36)]
        public uint biClrImportant;
        [FieldOffset(40)]
        public uint bV5RedMask;
        [FieldOffset(44)]
        public uint bV5GreenMask;
        [FieldOffset(48)]
        public uint bV5BlueMask;
        [FieldOffset(52)]
        public uint bV5AlphaMask;
        [FieldOffset(56)]
        public uint bV5CSType;
        [FieldOffset(60)]
        public CIEXYZTRIPLE bV5Endpoints;
        [FieldOffset(96)]
        public uint bV5GammaRed;
        [FieldOffset(100)]
        public uint bV5GammaGreen;
        [FieldOffset(104)]
        public uint bV5GammaBlue;
        [FieldOffset(108)]
        public uint bV5Intent;      // Rendering intent for bitmap
        [FieldOffset(112)]
        public uint bV5ProfileData;
        [FieldOffset(116)]
        public uint bV5ProfileSize;
        [FieldOffset(120)]
        public uint bV5Reserved;

        public const int DIB_RGB_COLORS = 0;

        public BITMAPINFOHEADER(int width, int height, ushort bpp)
        {
            biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));    // BITMAPINFOHEADER < DIBV5 is 40 bytes
            biPlanes = 1;   // Should allways be 1
            biCompression = BI_COMPRESSION.BI_RGB;
            biWidth = width;
            biHeight = height;
            biBitCount = bpp;
            biSizeImage = (uint)(width * height * (bpp >> 3));
            biXPelsPerMeter = 0;
            biYPelsPerMeter = 0;
            biClrUsed = 0;
            biClrImportant = 0;

            // V5
            bV5RedMask = (uint)255 << 16;
            bV5GreenMask = (uint)255 << 8;
            bV5BlueMask = (uint)255;
            bV5AlphaMask = (uint)255 << 24;
            bV5CSType = 1934772034; // sRGB
            bV5Endpoints = new CIEXYZTRIPLE();
            bV5Endpoints.ciexyzBlue = new CIEXYZ(0);
            bV5Endpoints.ciexyzGreen = new CIEXYZ(0);
            bV5Endpoints.ciexyzRed = new CIEXYZ(0);
            bV5GammaRed = 0;
            bV5GammaGreen = 0;
            bV5GammaBlue = 0;
            bV5Intent = 4;
            bV5ProfileData = 0;
            bV5ProfileSize = 0;
            bV5Reserved = 0;
        }

        public bool IsDibV5
        {
            get
            {
                uint sizeOfBMI = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                return biSize >= sizeOfBMI;
            }
        }
        public uint OffsetToPixels
        {
            get
            {
                if (biCompression == BI_COMPRESSION.BI_BITFIELDS)
                {
                    // Add 3x4 bytes for the bitfield color mask
                    return biSize + 3 * 4;
                }
                return biSize;
            }
        }
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct BITMAPINFO
    {
        /// <summary>
        /// A BITMAPINFOHEADER structure that contains information about the dimensions of color format.
        /// </summary>
        public BITMAPINFOHEADER bmiHeader;

        /// <summary>
        /// An array of RGBQUAD. The elements of the array that make up the color table.
        /// </summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct)]
        public RGBQUAD[] bmiColors;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RGBQUAD
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;
    }
}