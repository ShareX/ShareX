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
using ShareX.Platform.Windows.Native;
using System;
using Vortice.WIC;

namespace ShareX.Platform.Windows;

public sealed partial class WindowsSystemGraphicsService
{
    public unsafe SystemCursorImage? GetSystemCursor(SystemCursor cursor, int? size = null)
    {
        if (size is <= 0) return null;
        int resource = cursor switch
        {
            SystemCursor.Arrow => 32512,
            SystemCursor.IBeam => 32513,
            SystemCursor.Wait => 32514,
            SystemCursor.Cross => 32515,
            SystemCursor.UpArrow => 32516,
            SystemCursor.SizeNWSE => 32642,
            SystemCursor.SizeNESW => 32643,
            SystemCursor.SizeWE => 32644,
            SystemCursor.SizeNS => 32645,
            SystemCursor.SizeAll => 32646,
            SystemCursor.No => 32648,
            SystemCursor.Hand => 32649,
            SystemCursor.AppStarting => 32650,
            SystemCursor.Help => 32651,
            _ => 0
        };
        if (resource == 0) return null;

        lock (syncRoot)
        {
            if (disposed) return null;
            IntPtr handle = Win32.LoadCursor(IntPtr.Zero, (IntPtr)resource);
            if (handle == IntPtr.Zero || !Win32.GetIconInfo(handle, out Win32.ICONINFO info)) return null;
            try
            {
                int originalWidth = 32, originalHeight = 32;
                IntPtr image = info.hbmColor != IntPtr.Zero ? info.hbmColor : info.hbmMask;
                Win32.BITMAP bitmap;
                if (image != IntPtr.Zero && Win32.GetObject(image, sizeof(Win32.BITMAP), &bitmap) != 0)
                {
                    originalWidth = Math.Abs(bitmap.bmWidth);
                    originalHeight = Math.Abs(bitmap.bmHeight);
                    if (info.hbmColor == IntPtr.Zero && originalHeight > 1) originalHeight /= 2;
                }
                if (originalWidth <= 0 || originalHeight <= 0) return null;
                int width = size ?? originalWidth;
                int height = size.HasValue ? Math.Max(1, (int)Math.Round(originalHeight * width / (double)originalWidth)) : originalHeight;
                // GDI can erase thin monochrome strokes when stretching the cursor masks.
                // Recover native pixels first, then resize the completed alpha image.
                PixelBuffer? pixels = DrawCursor(handle, originalWidth, originalHeight);
                if (pixels == null) return null;
                if (width != originalWidth || height != originalHeight) pixels = ResizeCursor(pixels, width, height);
                PlatformPoint hotspot = new(
                    Math.Clamp((int)Math.Round(info.xHotspot * (double)width / originalWidth), 0, width - 1),
                    Math.Clamp((int)Math.Round(info.yHotspot * (double)height / originalHeight), 0, height - 1));
                return new SystemCursorImage(pixels, hotspot);
            }
            finally
            {
                if (info.hbmColor != IntPtr.Zero) Win32.DeleteObject(info.hbmColor);
                if (info.hbmMask != IntPtr.Zero) Win32.DeleteObject(info.hbmMask);
            }
        }
    }

    internal static unsafe PixelBuffer ResizeCursor(PixelBuffer source, int width, int height)
    {
        byte[] premultiplied = (byte[])source.Pixels.Clone();
        for (int i = 0; i < premultiplied.Length; i += 4)
            for (int channel = 0; channel < 3; channel++)
                premultiplied[i + channel] = (byte)((premultiplied[i + channel] * premultiplied[i + 3] + 127) / 255);

        using IWICImagingFactory factory = new();
        fixed (byte* data = premultiplied)
        {
            using IWICBitmap bitmap = factory.CreateBitmapFromMemory((uint)source.Width, (uint)source.Height,
                PixelFormat.Format32bppPBGRA, (uint)source.Stride, (uint)premultiplied.Length, data);
            using IWICBitmapScaler scaler = factory.CreateBitmapScaler();
            scaler.Initialize(bitmap, (uint)width, (uint)height, BitmapInterpolationMode.Fant);
            byte[] output = new byte[checked(width * height * 4)];
            scaler.CopyPixels((uint)(width * 4), output);
            return FromPremultipliedBgra(width, height, output);
        }
    }

    private static unsafe PixelBuffer? DrawCursor(IntPtr handle, int width, int height)
    {
        IntPtr dc = Win32.CreateCompatibleDC(IntPtr.Zero);
        if (dc == IntPtr.Zero) return null;
        Win32.BITMAPINFOHEADER info = new()
        {
            biSize = (uint)sizeof(Win32.BITMAPINFOHEADER), biWidth = width, biHeight = -height,
            biPlanes = 1, biBitCount = 32, biCompression = Win32.BI_RGB, biSizeImage = (uint)checked(width * height * 4)
        };
        IntPtr bitmap = Win32.CreateDIBSection(dc, &info, Win32.DIB_RGB_COLORS, out IntPtr bits, IntPtr.Zero, 0);
        if (bitmap == IntPtr.Zero || bits == IntPtr.Zero)
        {
            if (bitmap != IntPtr.Zero) Win32.DeleteObject(bitmap);
            Win32.DeleteDC(dc);
            return null;
        }
        IntPtr previous = Win32.SelectObject(dc, bitmap);
        try
        {
            int length = checked(width * height * 4);
            new Span<byte>((void*)bits, length).Clear();
            if (!Win32.DrawIconEx(dc, 0, 0, handle, width, height, 0, IntPtr.Zero, Win32.DI_NORMAL)) return null;
            Win32.GdiFlush();
            byte[] dark = new ReadOnlySpan<byte>((void*)bits, length).ToArray();
            bool hasAlpha = false;
            for (int i = 3; i < length; i += 4) hasAlpha |= dark[i] != 0;
            if (hasAlpha) return FromPremultipliedBgra(width, height, dark);

            // Monochrome cursors have no alpha. Preserve the editor's black/white mask recovery.
            new Span<byte>((void*)bits, length).Fill(255);
            if (!Win32.DrawIconEx(dc, 0, 0, handle, width, height, 0, IntPtr.Zero, Win32.DI_NORMAL)) return null;
            Win32.GdiFlush();
            byte* light = (byte*)bits;
            for (int i = 0; i < length; i += 4)
            {
                int alpha = 255 - Math.Clamp(Math.Max(light[i] - dark[i],
                    Math.Max(light[i + 1] - dark[i + 1], light[i + 2] - dark[i + 2])), 0, 255);
                for (int channel = 0; channel < 3; channel++)
                    dark[i + channel] = alpha == 0 ? (byte)0 : (byte)Math.Min(255, dark[i + channel] * 255 / alpha);
                dark[i + 3] = (byte)alpha;
            }
            return new PixelBuffer(width, height, dark);
        }
        finally
        {
            if (previous != IntPtr.Zero) Win32.SelectObject(dc, previous);
            Win32.DeleteObject(bitmap);
            Win32.DeleteDC(dc);
        }
    }
}
