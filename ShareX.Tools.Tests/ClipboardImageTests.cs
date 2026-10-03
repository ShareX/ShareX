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
using SkiaSharp;
using System.Buffers.Binary;
using Xunit;

namespace ShareX.Tools.Tests;

public sealed class ClipboardImageTests
{
    // Explicit packed DIB fixtures, independent of ShareX's encoder and without clipboard access.
    private static readonly SKColor[] ExpectedColors =
        [SKColors.Red, SKColors.Lime, SKColors.Blue, SKColors.Yellow, SKColors.Cyan, SKColors.Magenta];

    [Theory]
    [InlineData(16, 0, 40, false)]
    [InlineData(16, 0, 40, true)]
    [InlineData(16, 3, 40, false)]
    [InlineData(16, 3, 40, true)]
    [InlineData(24, 0, 40, false)]
    [InlineData(24, 0, 40, true)]
    [InlineData(32, 0, 40, false)]
    [InlineData(32, 0, 40, true)]
    [InlineData(32, 3, 40, false)]
    [InlineData(32, 3, 40, true)]
    [InlineData(32, 3, 124, false)]
    [InlineData(32, 3, 124, true)]
    public void TrueColorDibSkipsOptionalColorTableAndKeepsRows(int bitCount, int compression, int headerSize, bool topDown)
    {
        foreach (int colorCount in new[] { 0, 2 })
        {
            byte[] dib = CreateTrueColorDib(bitCount, compression, headerSize, topDown, colorCount);
            byte[] original = (byte[])dib.Clone();
            using SKBitmap? image = ClipboardHelpers.ConvertClipboardDibToBitmap(dib);
            Assert.NotNull(image);
            Assert.Equal(3, image.Width);
            Assert.Equal(2, image.Height);
            for (int y = 0; y < 2; y++)
                for (int x = 0; x < 3; x++)
                    Assert.Equal(ExpectedColors[y * 3 + x], image.GetPixel(x, y));
            Assert.Equal(original, dib);
        }
    }

    [Fact]
    public void IndexedDibStillUsesItsColorTable()
    {
        byte[] dib = new byte[52];
        WriteHeader(dib, 40, 3, -1, 8, 0, 2);
        // Two RGBQUAD entries (red and blue), followed by one padded row of indices.
        dib[42] = 255;
        dib[44] = 255;
        dib[48] = 0;
        dib[49] = 1;
        dib[50] = 0;
        using SKBitmap? image = ClipboardHelpers.ConvertClipboardDibToBitmap(dib);
        Assert.NotNull(image);
        Assert.Equal(SKColors.Red, image.GetPixel(0, 0));
        Assert.Equal(SKColors.Blue, image.GetPixel(1, 0));
        Assert.Equal(SKColors.Red, image.GetPixel(2, 0));
    }

    [Fact]
    public void V5DibKeepsExplicitPremultipliedAlpha()
    {
        byte[] dib = new byte[132];
        WriteHeader(dib, 124, 2, -1, 32, 3, 0);
        WriteMasks(dib, 32);
        BinaryPrimitives.WriteUInt32LittleEndian(dib.AsSpan(52), 0xFF000000);
        // Half-transparent premultiplied red followed by a fully transparent pixel.
        dib[126] = 128;
        dib[127] = 128;
        using SKBitmap? image = ClipboardHelpers.ConvertClipboardDibV5ToBitmap(dib);
        Assert.NotNull(image);
        Assert.Equal(SKAlphaType.Premul, image.AlphaType);
        Assert.Equal(new SKColor(255, 0, 0, 128), image.GetPixel(0, 0));
        Assert.Equal(new SKColor(0, 0, 0, 0), image.GetPixel(1, 0));
    }

    [Fact]
    public void RejectsColorTableThatLeavesTooFewPixelBytes()
    {
        byte[] dib = CreateTrueColorDib(32, 0, 40, false, 2);
        Array.Resize(ref dib, dib.Length - 1);
        using SKBitmap? image = ClipboardHelpers.ConvertClipboardDibToBitmap(dib);
        Assert.Null(image);
    }

    private static byte[] CreateTrueColorDib(int bitCount, int compression, int headerSize, bool topDown, int colorCount)
    {
        const int width = 3, height = 2;
        int stride = (width * bitCount + 31) / 32 * 4;
        int masksSize = headerSize == 40 && compression == 3 ? 12 : 0;
        int pixelOffset = headerSize + masksSize + colorCount * 4;
        byte[] dib = new byte[pixelOffset + stride * height];
        WriteHeader(dib, headerSize, width, topDown ? -height : height, bitCount, compression, colorCount);
        if (compression == 3) WriteMasks(dib, bitCount);
        // Deliberately unrelated colors make an incorrect pixel offset visible.
        for (int i = 0; i < colorCount; i++)
        {
            int entry = headerSize + masksSize + i * 4;
            dib[entry] = 17;
            dib[entry + 1] = 29;
            dib[entry + 2] = 43;
        }
        for (int y = 0; y < height; y++)
        {
            int row = pixelOffset + (topDown ? y : height - 1 - y) * stride;
            for (int x = 0; x < width; x++)
            {
                SKColor color = ExpectedColors[y * width + x];
                int pixel = row + x * (bitCount / 8);
                if (bitCount == 16)
                {
                    int value = compression == 3
                        ? (color.Red >> 3) << 11 | (color.Green >> 2) << 5 | color.Blue >> 3
                        : (color.Red >> 3) << 10 | (color.Green >> 3) << 5 | color.Blue >> 3;
                    BinaryPrimitives.WriteUInt16LittleEndian(dib.AsSpan(pixel), (ushort)value);
                }
                else
                {
                    dib[pixel] = color.Blue;
                    dib[pixel + 1] = color.Green;
                    dib[pixel + 2] = color.Red;
                    if (bitCount == 32) dib[pixel + 3] = 255;
                }
            }
        }
        return dib;
    }

    private static void WriteHeader(byte[] dib, int size, int width, int height, int bitCount, int compression, int colorCount)
    {
        BinaryPrimitives.WriteInt32LittleEndian(dib, size);
        BinaryPrimitives.WriteInt32LittleEndian(dib.AsSpan(4), width);
        BinaryPrimitives.WriteInt32LittleEndian(dib.AsSpan(8), height);
        BinaryPrimitives.WriteUInt16LittleEndian(dib.AsSpan(12), 1);
        BinaryPrimitives.WriteUInt16LittleEndian(dib.AsSpan(14), (ushort)bitCount);
        BinaryPrimitives.WriteInt32LittleEndian(dib.AsSpan(16), compression);
        BinaryPrimitives.WriteInt32LittleEndian(dib.AsSpan(32), colorCount);
    }

    private static void WriteMasks(byte[] dib, int bitCount)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(dib.AsSpan(40), bitCount == 16 ? 0xF800u : 0x00FF0000u);
        BinaryPrimitives.WriteUInt32LittleEndian(dib.AsSpan(44), bitCount == 16 ? 0x07E0u : 0x0000FF00u);
        BinaryPrimitives.WriteUInt32LittleEndian(dib.AsSpan(48), bitCount == 16 ? 0x001Fu : 0x000000FFu);
    }
}
