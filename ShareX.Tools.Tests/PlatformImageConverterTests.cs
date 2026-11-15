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

using ShareX.HelpersLib;
using ShareX.Platform.Imaging;
using SkiaSharp;
using Xunit;

namespace ShareX.Tools.Tests;

public sealed class PlatformImageConverterTests
{
    [Fact]
    public void OpaqueCapturesAreCopiedWithTheSamePixelsAsTheConversion()
    {
        // 13 pixels wide so the rows are not a whole number of vectors.
        PixelBuffer buffer = new(13, 5);
        new Random(9131).NextBytes(buffer.Pixels);
        PixelBuffer converted = new(13, 5);
        buffer.Pixels.CopyTo(converted.Pixels, 0);
        PixelBuffer.MakeOpaque(converted.Pixels);
        buffer.MakeOpaque();

        using SKBitmap fast = PlatformImageConverter.ToSKBitmap(buffer);
        using SKBitmap slow = PlatformImageConverter.ToSKBitmap(converted);

        Assert.True(buffer.IsOpaque);
        Assert.False(converted.IsOpaque);
        Assert.Equal(slow.Bytes, fast.Bytes);
    }

    [Fact]
    public void DecodeToSrgb_ConvertsACaptureInTheDisplaysColourProfile()
    {
        // What macOS screencapture writes on a Display P3 screen: P3 pixel values and the profile that says so.
        using SKColorSpace displayP3 = SKColorSpace.CreateRgb(SKColorSpaceTransferFn.Srgb, SKColorSpaceXyz.DisplayP3);
        using SKBitmap capture = new SKBitmap(new SKImageInfo(2, 2, SKColorType.Bgra8888, SKAlphaType.Opaque, displayP3));
        // Written as bytes (blue, green, red, alpha): drawing calls would convert the colour into the profile.
        System.Runtime.InteropServices.Marshal.Copy(Enumerable.Repeat(new byte[] { 50, 100, 200, 255 }, 4).SelectMany(x => x).ToArray(), 0, capture.GetPixels(), 16);
        using SKData png = capture.Encode(SKEncodedImageFormat.Png, 100);

        using SKBitmap decoded = PlatformImageConverter.DecodeToSrgb(png.ToArray());

        Assert.True(decoded.ColorSpace == null || decoded.ColorSpace.IsSrgb);
        SKColor pixel = decoded.GetPixel(1, 1);
        // The same colour needs more red and less green and blue in sRGB, whose primaries are less saturated.
        Assert.InRange(pixel.Red, 208, 222);
        Assert.InRange(pixel.Green, 88, 99);
        Assert.InRange(pixel.Blue, 25, 45);
    }

    [Fact]
    public void DecodeToSrgb_LeavesAnSrgbCaptureUnchanged()
    {
        using SKBitmap capture = new SKBitmap(new SKImageInfo(2, 2, SKColorType.Bgra8888, SKAlphaType.Opaque));
        capture.Erase(new SKColor(200, 100, 50));
        using SKData png = capture.Encode(SKEncodedImageFormat.Png, 100);

        using SKBitmap decoded = PlatformImageConverter.DecodeToSrgb(png.ToArray());

        Assert.Equal(new SKColor(200, 100, 50), decoded.GetPixel(0, 0));
    }
}
