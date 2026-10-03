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

using SkiaSharp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib;

/// <summary>Windows capture and shell handles enter the image pipeline as Skia bitmaps.</summary>
public static class WindowsImageInterop
{
    [DllImport("gdi32.dll", EntryPoint = "GetObjectW", SetLastError = true)]
    private static extern int GetBitmapObject(IntPtr handle, int size, out NativeBitmap bitmap);
    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern int GetDIBits(IntPtr dc, IntPtr bitmap, uint start, uint lines, IntPtr pixels,
        ref BITMAPINFOHEADER header, uint usage);
    [DllImport("gdi32.dll")]
    private static extern bool GdiFlush();
    [StructLayout(LayoutKind.Sequential)]
    private struct NativeBitmap
    {
        public int Type, Width, Height, WidthBytes;
        public ushort Planes, BitsPixel;
        public IntPtr Bits;
    }

    public static Size GetBitmapSize(IntPtr bitmap)
    {
        if (GetBitmapObject(bitmap, Marshal.SizeOf<NativeBitmap>(), out NativeBitmap info) == 0)
            throw new Win32Exception();
        return new Size(info.Width, info.Height);
    }

    public static SKBitmap FromHBitmap(IntPtr handle, bool preserveAlpha = false)
    {
        Size size = GetBitmapSize(handle);
        SKBitmap result = new(new SKImageInfo(size.Width, size.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul));
        IntPtr dc = NativeMethods.GetDC(IntPtr.Zero);
        try
        {
            BITMAPINFOHEADER header = new(size.Width, -size.Height, 32) { biSize = 40, biSizeImage = checked((uint)size.Width * (uint)size.Height * 4) };
            if (GetDIBits(dc, handle, 0, (uint)size.Height, result.GetPixels(), ref header, 0) != size.Height)
                throw new Win32Exception();
            if (!preserveAlpha) SetOpaque(result);
            return result;
        }
        catch { result.Dispose(); throw; }
        finally { NativeMethods.ReleaseDC(IntPtr.Zero, dc); }
    }

    public static SKBitmap Capture(Rectangle rectangle, Action<IntPtr> drawCursor = null, IntPtr window = default)
    {
        if (rectangle.Width <= 0 || rectangle.Height <= 0) return null;
        if (window == IntPtr.Zero) window = NativeMethods.GetDesktopWindow();
        IntPtr source = NativeMethods.GetWindowDC(window);
        if (source == IntPtr.Zero) throw new Win32Exception();
        try
        {
            return DrawNative(rectangle.Size, SKColors.Black, dc =>
            {
                if (!NativeMethods.BitBlt(dc, 0, 0, rectangle.Width, rectangle.Height, source,
                    rectangle.X, rectangle.Y, NativeRasterOperation.SourceCopy | NativeRasterOperation.CaptureBlt))
                    throw new Win32Exception();
                drawCursor?.Invoke(dc);
            });
        }
        finally { NativeMethods.ReleaseDC(window, source); }
    }

    private static unsafe SKBitmap DrawNative(Size size, SKColor background, Action<IntPtr> draw)
    {
        IntPtr dc = NativeMethods.CreateCompatibleDC(IntPtr.Zero);
        if (dc == IntPtr.Zero) throw new Win32Exception();
        IntPtr handle = IntPtr.Zero, previous = IntPtr.Zero;
        try
        {
            BITMAPINFOHEADER header = new(size.Width, -size.Height, 32) { biSize = 40, biSizeImage = checked((uint)size.Width * (uint)size.Height * 4) };
            handle = NativeMethods.CreateDIBSection(dc, ref header, 0, out IntPtr pixels, IntPtr.Zero, 0);
            if (handle == IntPtr.Zero) throw new Win32Exception();
            previous = NativeMethods.SelectObject(dc, handle);
            using SKBitmap surface = new();
            if (!surface.InstallPixels(new SKImageInfo(size.Width, size.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul), pixels, size.Width * 4))
                throw new InvalidOperationException("Unable to access Windows image pixels.");
            surface.Erase(background);
            draw(dc);
            GdiFlush();
            surface.NotifyPixelsChanged();
            SKBitmap result = surface.Copy();
            SetOpaque(result);
            return result;
        }
        finally
        {
            if (previous != IntPtr.Zero) NativeMethods.SelectObject(dc, previous);
            if (handle != IntPtr.Zero) NativeMethods.DeleteObject(handle);
            NativeMethods.DeleteDC(dc);
        }
    }

    public static SKBitmap FromIcon(IntPtr icon, Size size = default)
    {
        if (icon == IntPtr.Zero) return null;
        if (size.IsEmpty && NativeMethods.GetIconInfo(icon, out IconInfo info))
        {
            try
            {
                Size maskSize = GetBitmapSize(info.hbmColor != IntPtr.Zero ? info.hbmColor : info.hbmMask);
                size = new Size(maskSize.Width, info.hbmColor != IntPtr.Zero ? maskSize.Height : maskSize.Height / 2);
            }
            finally
            {
                if (info.hbmColor != IntPtr.Zero) NativeMethods.DeleteObject(info.hbmColor);
                if (info.hbmMask != IntPtr.Zero) NativeMethods.DeleteObject(info.hbmMask);
            }
        }
        if (size.IsEmpty) return null;
        Action<IntPtr> draw = dc => NativeMethods.DrawIconEx(dc, 0, 0, icon, size.Width, size.Height, 0, IntPtr.Zero, NativeConstants.DI_NORMAL);
        using SKBitmap black = DrawNative(size, SKColors.Black, draw);
        using SKBitmap white = DrawNative(size, SKColors.White, draw);
        SKBitmap result = SkiaImageHelpers.CreateBitmap(size.Width, size.Height);
        using SkiaPixelBuffer dark = new(black, true, PixelAccess.ReadOnly);
        using SkiaPixelBuffer light = new(white, true, PixelAccess.ReadOnly);
        using SkiaPixelBuffer output = new(result, true, PixelAccess.WriteOnly);
        for (int index = 0; index < output.PixelCount; index++)
        {
            ColorBgra d = dark.GetPixel(index), l = light.GetPixel(index);
            int alpha = 255 - Math.Clamp(Math.Max(l.Red - d.Red, Math.Max(l.Green - d.Green, l.Blue - d.Blue)), 0, 255);
            if (alpha == 0) continue;
            output.SetPixel(index, new ColorBgra((byte)Math.Min(255, d.Blue * 255 / alpha),
                (byte)Math.Min(255, d.Green * 255 / alpha), (byte)Math.Min(255, d.Red * 255 / alpha), (byte)alpha));
        }
        return result;
    }

    private static unsafe void SetOpaque(SKBitmap bitmap)
    {
        for (int y = 0; y < bitmap.Height; y++)
        {
            byte* row = (byte*)bitmap.GetPixels() + y * bitmap.RowBytes;
            for (int x = 0; x < bitmap.Width; x++) row[x * 4 + 3] = 255;
        }
        bitmap.NotifyPixelsChanged();
    }
}
