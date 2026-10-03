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

using Avalonia;
using Avalonia.Platform;
using ShareX.AvaloniaUI.Input;
using SkiaSharp;
using System.Buffers.Binary;
using Xunit;

namespace ShareX.ImageEditor.Tests;

public sealed class CursorBitmapTests
{
    [Theory]
    [InlineData("openhand", 7, 5, 179)]
    [InlineData("closedhand", 7, 5, 113)]
    [InlineData("Crosshair", 15, 15, 132)]
    public void InteractiveCursorAssetsKeepTheirOriginalPixelsAndHotspots(string asset, int hotX, int hotY, int visiblePixels)
    {
        byte[] data = ReadAsset(asset);
        using MemoryStream stream = new(data);
        using SKBitmap bitmap = CursorBitmapDecoder.Decode(stream, 1, out PixelPoint hotSpot);
        Assert.Equal(new PixelPoint(hotX, hotY), hotSpot);
        Assert.Equal(32, bitmap.Width);
        Assert.Equal(32, bitmap.Height);
        Assert.Equal(visiblePixels, bitmap.Pixels.Count(color => color.Alpha != 0));

        // CUR and ICO share the raster payload. Compare with Skia's independent ICO decoder.
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(2), 1);
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(10), 1);
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(12), 1);
        using SKBitmap reference = SKBitmap.Decode(data);
        Assert.Equal(reference.Pixels, bitmap.Pixels);
    }

    [Theory]
    [InlineData(1.25, 40, 9, 6, 19)]
    [InlineData(1.5, 48, 10, 8, 22)]
    [InlineData(1.75, 56, 12, 9, 26)]
    [InlineData(2, 64, 14, 10, 30)]
    [InlineData(2.5, 80, 18, 12, 38)]
    public void FractionalDpiScalesImageAndHotspotTogether(double scale, int size, int handX, int handY, int crosshair)
    {
        foreach (string asset in new[] { "openhand", "closedhand", "Crosshair" })
        {
            using MemoryStream stream = new(ReadAsset(asset));
            using SKBitmap bitmap = CursorBitmapDecoder.Decode(stream, scale, out PixelPoint hotSpot);
            Assert.Equal(size, bitmap.Width);
            Assert.Equal(size, bitmap.Height);
            Assert.Equal(asset == "Crosshair" ? new PixelPoint(crosshair, crosshair) : new PixelPoint(handX, handY), hotSpot);
            Assert.Contains(bitmap.Pixels, color => color.Alpha != 0);
            Assert.Contains(bitmap.Pixels, color => color.Alpha == 0);
        }
    }

    [Fact]
    public void ArgbChannelsArePremultipliedAndExplicitAlphaNeedsNoAndMask()
    {
        byte[] dib = Dib(2, 1, 32, 8);
        new byte[] { 16, 64, 240, 128, 255, 64, 16, 0 }.CopyTo(dib, 40);
        byte[] data = CursorFile(dib, 2, 1);
        using MemoryStream stream = new(data);
        using SKBitmap bitmap = CursorBitmapDecoder.Decode(stream, 1, out _);
        Assert.Equal(SKAlphaType.Premul, bitmap.AlphaType);
        Assert.Equal(new byte[] { 8, 32, 120, 128, 0, 0, 0, 0 }, bitmap.GetPixelSpan().ToArray());
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(2), 1);
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(10), 1);
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(12), 32);
        using SKBitmap reference = SKBitmap.Decode(data);
        Assert.Equal(reference.Pixels, bitmap.Pixels);
    }

    [Fact]
    public void LegacyAndMaskAndScreenInversionFallbackRemainVisible()
    {
        byte[] dib = Dib(4, 1, 1, 16);
        new byte[] { 0, 0, 0, 0, 255, 255, 255, 0 }.CopyTo(dib, 40);
        dib[48] = 0b01010000; // black, white, transparent, inverted
        dib[52] = 0b00110000;
        using MemoryStream stream = new(CursorFile(dib, 4, 1));
        using SKBitmap bitmap = CursorBitmapDecoder.Decode(stream, 1, out _);
        Assert.Equal(new[] { SKColors.Black, SKColors.White, new SKColor(0, 0, 0, 0), SKColors.White }, bitmap.Pixels);
    }

    [Fact]
    public void ArgbWithoutAlphaUsesBottomUpRowsAndLegacyMask()
    {
        byte[] dib = Dib(2, 2, 32, 24);
        // Bottom row blue/green, top row red/white; all legacy alpha bytes are zero.
        new byte[] { 255, 0, 0, 0, 0, 255, 0, 0, 0, 0, 255, 0, 255, 255, 255, 0 }.CopyTo(dib, 40);
        dib[56] = 0b01000000; // bottom-right transparent
        using MemoryStream stream = new(CursorFile(dib, 2, 2));
        using SKBitmap bitmap = CursorBitmapDecoder.Decode(stream, 1, out _);
        Assert.Equal(new[] { SKColors.Red, SKColors.White, SKColors.Blue, new SKColor(0, 0, 0, 0) }, bitmap.Pixels);
    }

    [Fact]
    public void PngPayloadKeepsTransparencyAndUsesClampedScaledHotspot()
    {
        using SKBitmap original = new(3, 2);
        original.Erase(SKColors.Transparent);
        original.SetPixel(1, 0, SKColors.Red);
        using SKImage image = SKImage.FromBitmap(original);
        using SKData png = image.Encode(SKEncodedImageFormat.Png, 100);
        using MemoryStream stream = new(CursorFile(png.ToArray(), 3, 2, 2, 1));
        using SKBitmap bitmap = CursorBitmapDecoder.Decode(stream, 2, out PixelPoint hotSpot);
        Assert.Equal(6, bitmap.Width);
        Assert.Equal(4, bitmap.Height);
        Assert.Equal(new PixelPoint(4, 2), hotSpot);
        Assert.Contains(bitmap.Pixels, pixel => pixel.Red > 0 && pixel.Alpha > 0);
        Assert.Contains(bitmap.Pixels, pixel => pixel.Alpha == 0);
    }

    [Fact]
    public void TruncatedPayloadCannotReadAnotherDirectoryEntryOrTrailingBytes()
    {
        byte[] dib = Dib(4, 1, 1, 16);
        byte[] data = CursorFile(dib, 4, 1);
        BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(14), 42); // file still contains the mask, but payload does not
        using MemoryStream stream = new(data);
        Assert.Throws<InvalidDataException>(() => CursorBitmapDecoder.Decode(stream, 1, out _));
    }

    [GraphicsArtifactFact]
    public void RenderInteractiveCursorDpiContactSheet()
    {
        string directory = Environment.GetEnvironmentVariable("SHAREX_TEST_GRAPHICS_OUTPUT")!;
        Directory.CreateDirectory(directory);
        double[] scales = [1, 1.25, 1.5, 2, 2.5];
        string[] assets = ["openhand", "closedhand", "Crosshair"];
        using SKBitmap sheet = new(650, 360);
        using SKCanvas canvas = new(sheet);
        using SKFont font = new(SKTypeface.Default, 13);
        using SKPaint text = new() { Color = SKColors.Black, IsAntialias = true };
        using SKPaint marker = new() { Color = SKColors.Magenta, IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1 };
        canvas.Clear(new SKColor(235, 235, 235));
        for (int row = 0; row < assets.Length; row++)
        {
            for (int column = 0; column < scales.Length; column++)
            {
                using MemoryStream stream = new(ReadAsset(assets[row]));
                using SKBitmap cursor = CursorBitmapDecoder.Decode(stream, scales[column], out PixelPoint hotSpot);
                float x = column * 130 + 16;
                float y = row * 120 + 29;
                using SKPaint background = new() { Color = row % 2 == 0 ? SKColors.White : new SKColor(75, 75, 75) };
                canvas.DrawRect(x - 3, y - 3, 96, 87, background);
                canvas.DrawBitmap(cursor, x, y);
                canvas.DrawCircle(x + hotSpot.X, y + hotSpot.Y, 3, marker);
                canvas.DrawText($"{assets[row]} {scales[column]:0.##}x", column * 130 + 8, row * 120 + 17, font, text);
            }
        }
        using SKImage image = SKImage.FromBitmap(sheet);
        using SKData png = image.Encode(SKEncodedImageFormat.Png, 100);
        using FileStream output = File.Create(Path.Combine(directory, "interactive-cursors-dpi.png"));
        png.SaveTo(output);
    }

    private static byte[] ReadAsset(string asset)
    {
        using Stream stream = new StandardAssetLoader().Open(new Uri($"avares://ShareX.Avalonia/Assets/{asset}.cur"));
        using MemoryStream memory = new();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    private static byte[] CursorFile(byte[] payload, byte width, byte height, ushort hotX = 0, ushort hotY = 0)
    {
        byte[] data = new byte[22 + payload.Length];
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(2), 2);
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(4), 1);
        data[6] = width;
        data[7] = height;
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(10), hotX);
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(12), hotY);
        BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(14), (uint)payload.Length);
        BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(18), 22);
        payload.CopyTo(data, 22);
        return data;
    }

    private static byte[] Dib(int width, int height, ushort bits, int imageBytes)
    {
        byte[] data = new byte[40 + imageBytes];
        BinaryPrimitives.WriteInt32LittleEndian(data, 40);
        BinaryPrimitives.WriteInt32LittleEndian(data.AsSpan(4), width);
        BinaryPrimitives.WriteInt32LittleEndian(data.AsSpan(8), height * 2);
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(12), 1);
        BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(14), bits);
        return data;
    }
}
