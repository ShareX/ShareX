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

using ShareX.Platform;
using ShareX.Platform.Imaging;
using SkiaSharp;
using System;

namespace ShareX.HelpersLib
{
    /// <summary>Converts images from ShareX.Platform into SkiaSharp bitmaps.</summary>
    public static class PlatformImageConverter
    {
        /// <summary>A premultiplied BGRA bitmap holding the capture, without a PNG round trip when the platform captured pixels.</summary>
        public static SKBitmap ToSKBitmap(ScreenCaptureResult result)
        {
            ArgumentNullException.ThrowIfNull(result);

            if (result.Pixels != null)
            {
                return ToSKBitmap(result.Pixels);
            }

            return SKBitmap.Decode(result.Png) ?? throw new InvalidOperationException("The captured image could not be decoded.");
        }

        public static unsafe SKBitmap ToSKBitmap(PixelBuffer pixels)
        {
            SKBitmap bitmap = new SKBitmap(new SKImageInfo(pixels.Width, pixels.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
            SKImageInfo source = new SKImageInfo(pixels.Width, pixels.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);

            fixed (byte* data = pixels.Pixels)
            {
                using SKPixmap pixmap = new SKPixmap(source, (IntPtr)data, pixels.Stride);
                pixmap.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.RowBytes, 0, 0);
            }

            return bitmap;
        }

        /// <summary>A straight alpha BGRA copy of <paramref name="bitmap"/> for the platform services.</summary>
        public static unsafe PixelBuffer ToPixelBuffer(SKBitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap);

            PixelBuffer pixels = new PixelBuffer(bitmap.Width, bitmap.Height);
            SKImageInfo target = new SKImageInfo(bitmap.Width, bitmap.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);

            fixed (byte* data = pixels.Pixels)
            {
                if (!bitmap.PeekPixels().ReadPixels(target, (IntPtr)data, pixels.Stride, 0, 0))
                {
                    throw new InvalidOperationException("The image could not be converted.");
                }
            }

            return pixels;
        }
    }
}
