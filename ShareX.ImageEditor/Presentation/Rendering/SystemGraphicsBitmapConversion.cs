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
using SkiaSharp;
using System.Runtime.InteropServices;

namespace ShareX.ImageEditor.Presentation.Rendering;

internal static class SystemGraphicsBitmapConversion
{
    internal static SKBitmap ToSkBitmap(PixelBuffer pixels)
    {
        using SKBitmap source = new(new SKImageInfo(pixels.Width, pixels.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul));
        Marshal.Copy(pixels.Pixels, 0, source.GetPixels(), checked(pixels.Width * pixels.Height * 4));
        SKBitmap output = new(new SKImageInfo(pixels.Width, pixels.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
        using SKPixmap pixmap = source.PeekPixels();
        if (!pixmap.ReadPixels(output.Info, output.GetPixels(), output.RowBytes))
        {
            output.Dispose();
            throw new InvalidOperationException("Unable to convert platform graphics pixels.");
        }
        return output;
    }
}
