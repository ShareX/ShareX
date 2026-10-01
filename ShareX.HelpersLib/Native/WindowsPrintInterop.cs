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
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib;

/// <summary>Transfers a rendered Skia page to a Windows printer or preview device context.</summary>
internal static class WindowsPrintInterop
{
    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern int StretchDIBits(IntPtr dc, int x, int y, int width, int height,
        int sourceX, int sourceY, int sourceWidth, int sourceHeight, IntPtr pixels,
        ref BITMAPINFOHEADER info, uint usage, uint operation);

    public static void DrawImage(PrintPageEventArgs args, SKBitmap image, Rectangle rectangle)
    {
        using MemoryStream stream = new();
        image.Save(stream, SKEncodedImageFormat.Bmp);
        byte[] bytes = stream.ToArray();
        BITMAPINFOHEADER header = new(image.Width, image.Height, 24) { biSize = 40 };
        header.biSizeImage = (uint)(bytes.Length - 54);
        float scaleX = args.Graphics.DpiX / 100f, scaleY = args.Graphics.DpiY / 100f;
        rectangle = new Rectangle((int)Math.Round(rectangle.X * scaleX), (int)Math.Round(rectangle.Y * scaleY),
            (int)Math.Round(rectangle.Width * scaleX), (int)Math.Round(rectangle.Height * scaleY));
        GCHandle pinned = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        IntPtr dc = IntPtr.Zero;
        try
        {
            dc = args.Graphics.GetHdc();
            int result = StretchDIBits(dc, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height,
                0, 0, image.Width, image.Height, IntPtr.Add(pinned.AddrOfPinnedObject(), 54), ref header, 0, 0x00CC0020);
            if (result == -1) throw new Win32Exception();
        }
        finally
        {
            if (dc != IntPtr.Zero) args.Graphics.ReleaseHdc(dc);
            pinned.Free();
        }
    }
}
