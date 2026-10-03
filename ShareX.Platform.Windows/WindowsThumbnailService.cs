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

using ShareX.Platform.Imaging;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ShareX.Platform.Windows;

/// <summary>Explorer's own thumbnails through IShellItemImageFactory, including video frames and document previews.</summary>
[SupportedOSPlatform("windows")]
public sealed unsafe class WindowsThumbnailService : IThumbnailService
{
    private static readonly Guid ShellItemImageFactory = new Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b");

    public FeatureSupport Support => FeatureSupport.Supported;

    public byte[]? GetThumbnail(string path, int maxWidth, int maxHeight)
    {
        IShellItemImageFactory? factory = null;
        IntPtr bitmap = IntPtr.Zero;

        try
        {
            SHCreateItemFromParsingName(path, IntPtr.Zero, ShellItemImageFactory, out factory);
            factory.GetImage(new SIZE { cx = maxWidth, cy = maxHeight }, 0 /* SIIGBF_RESIZETOFIT */, out bitmap);

            if (bitmap == IntPtr.Zero)
            {
                return null;
            }

            PixelBuffer? pixels = ReadPixels(bitmap);
            return pixels == null ? null : PngCodec.Encode(pixels);
        }
        catch (Exception ex) when (ex is COMException or ArgumentException or InvalidCastException)
        {
            return null;
        }
        finally
        {
            if (bitmap != IntPtr.Zero)
            {
                Native.Win32.DeleteObject(bitmap);
            }

            if (factory != null && Marshal.IsComObject(factory))
            {
                Marshal.ReleaseComObject(factory);
            }
        }
    }

    /// <summary>Copies the HBITMAP into a top down BGRA buffer. Shell thumbnails use premultiplied alpha, which PixelBuffer does not.</summary>
    private static PixelBuffer? ReadPixels(IntPtr bitmap)
    {
        Native.Win32.BITMAP info;

        if (Native.Win32.GetObject(bitmap, sizeof(Native.Win32.BITMAP), &info) == 0 || info.bmWidth <= 0 || info.bmHeight <= 0)
        {
            return null;
        }

        PixelBuffer buffer = new PixelBuffer(info.bmWidth, info.bmHeight);
        Native.Win32.BITMAPINFOHEADER header = new Native.Win32.BITMAPINFOHEADER
        {
            biSize = (uint)sizeof(Native.Win32.BITMAPINFOHEADER),
            biWidth = info.bmWidth,
            biHeight = -info.bmHeight, // Top down
            biPlanes = 1,
            biBitCount = 32,
            biCompression = Native.Win32.BI_RGB
        };

        IntPtr screenDc = Native.Win32.GetDC(IntPtr.Zero);

        try
        {
            fixed (byte* pixels = buffer.Pixels)
            {
                if (Native.Win32.GetDIBits(screenDc, bitmap, 0, (uint)info.bmHeight, pixels, &header, Native.Win32.DIB_RGB_COLORS) == 0)
                {
                    return null;
                }
            }
        }
        finally
        {
            Native.Win32.ReleaseDC(IntPtr.Zero, screenDc);
        }

        byte[] data = buffer.Pixels;

        for (int i = 0; i < info.bmWidth * info.bmHeight * 4; i += 4)
        {
            byte alpha = data[i + 3];

            if (alpha != 0 && alpha != 255)
            {
                data[i] = (byte)Math.Min(255, data[i] * 255 / alpha);
                data[i + 1] = (byte)Math.Min(255, data[i + 1] * 255 / alpha);
                data[i + 2] = (byte)Math.Min(255, data[i + 2] * 255 / alpha);
            }
        }

        return buffer;
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    private static extern void SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string path, IntPtr bindContext, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IShellItemImageFactory result);

    [StructLayout(LayoutKind.Sequential)]
    private struct SIZE
    {
        public int cx;
        public int cy;
    }

    [ComImport]
    [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItemImageFactory
    {
        void GetImage([In, MarshalAs(UnmanagedType.Struct)] SIZE size, [In] int flags, out IntPtr bitmap);
    }
}
