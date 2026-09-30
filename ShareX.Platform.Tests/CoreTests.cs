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
using Xunit;

namespace ShareX.Platform.Tests;

public class GeometryTests
{
    [Fact]
    public void Intersect_ReturnsEmptyWhenApart()
    {
        PlatformRectangle a = new PlatformRectangle(0, 0, 10, 10);

        Assert.Equal(new PlatformRectangle(5, 5, 5, 5), a.Intersect(new PlatformRectangle(5, 5, 10, 10)));
        Assert.True(a.Intersect(new PlatformRectangle(10, 0, 5, 5)).IsEmpty);
    }

    [Fact]
    public void Union_IgnoresEmpty()
    {
        PlatformRectangle a = new PlatformRectangle(-100, 0, 100, 50);

        Assert.Equal(a, a.Union(PlatformRectangle.Empty));
        Assert.Equal(new PlatformRectangle(-100, 0, 200, 80), a.Union(new PlatformRectangle(0, 20, 100, 60)));
    }

    [Fact]
    public void Contains_ExcludesRightAndBottomEdges()
    {
        PlatformRectangle a = new PlatformRectangle(0, 0, 10, 10);

        Assert.True(a.Contains(new PlatformPoint(9, 9)));
        Assert.False(a.Contains(new PlatformPoint(10, 5)));
    }

    [Fact]
    public void RecordingRegion_RoundsDownToEvenSize()
    {
        ScreenRecordingRequest request = new ScreenRecordingRequest { Region = new PlatformRectangle(1, 1, 101, 51) };

        Assert.Equal(new PlatformRectangle(1, 1, 100, 50), request.GetEffectiveRegion(PlatformRectangle.Empty));
        Assert.Equal(new PlatformRectangle(1, 1, 101, 51), (request with { RequireEvenSize = false }).GetEffectiveRegion(PlatformRectangle.Empty));
        Assert.Equal(new PlatformRectangle(0, 0, 1920, 1080), new ScreenRecordingRequest().GetEffectiveRegion(new PlatformRectangle(0, 0, 1921, 1080)));
    }
}

public class HotkeyTests
{
    [Fact]
    public void ToString_ListsModifiersInOrder()
    {
        Assert.Equal("Ctrl+Alt+Shift+Super+A", new PlatformHotkey('A', HotkeyModifiers.Super | HotkeyModifiers.Shift | HotkeyModifiers.Alt | HotkeyModifiers.Control).ToString());
        Assert.Equal("PrintScreen", new PlatformHotkey(VirtualKeys.PrintScreen, HotkeyModifiers.None).ToString());
        Assert.Equal("Shift+F11", new PlatformHotkey(VirtualKeys.F1 + 10, HotkeyModifiers.Shift).ToString());
    }

    [Fact]
    public void ModifierValues_MatchWin32()
    {
        Assert.Equal(1, (int)HotkeyModifiers.Alt);
        Assert.Equal(2, (int)HotkeyModifiers.Control);
        Assert.Equal(4, (int)HotkeyModifiers.Shift);
        Assert.Equal(8, (int)HotkeyModifiers.Super);
    }

    [Fact]
    public void IsValid_RequiresKey()
    {
        Assert.False(new PlatformHotkey(0, HotkeyModifiers.Control).IsValid);
    }
}

public class PngCodecTests
{
    private static PixelBuffer CreateGradient(int width, int height)
    {
        PixelBuffer image = new PixelBuffer(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = (y * width + x) * 4;
                image.Pixels[i] = (byte)(x * 7);
                image.Pixels[i + 1] = (byte)(y * 13);
                image.Pixels[i + 2] = (byte)(x + y);
                image.Pixels[i + 3] = (byte)(255 - x);
            }
        }

        return image;
    }

    [Fact]
    public void EncodeDecode_RoundTrips()
    {
        PixelBuffer image = CreateGradient(37, 19);

        byte[] png = PngCodec.Encode(image);
        PixelBuffer decoded = PngCodec.Decode(png);

        Assert.True(PngCodec.IsPng(png));
        Assert.Equal(new PlatformSize(37, 19), PngCodec.ReadSize(png));
        Assert.Equal(image.Pixels, decoded.Pixels);
    }

    [Fact]
    public void Crop_KeepsSelectedPixels()
    {
        PixelBuffer image = CreateGradient(20, 20);

        PixelBuffer cropped = PngCodec.Decode(PngCodec.Crop(PngCodec.Encode(image), new PlatformRectangle(5, 6, 4, 3)));

        Assert.Equal(4, cropped.Width);
        Assert.Equal(3, cropped.Height);
        Assert.Equal(image.Pixels.AsSpan((6 * 20 + 5) * 4, 16).ToArray(), cropped.Pixels.AsSpan(0, 16).ToArray());
    }

    [Fact]
    public void IsPng_RejectsOtherData()
    {
        Assert.False(PngCodec.IsPng(new byte[] { 0xFF, 0xD8, 0xFF }));
    }

    [Fact]
    public void BlendFrom_ComposesOverlay()
    {
        PixelBuffer background = new PixelBuffer(1, 1, [0, 0, 0, 255]);
        PixelBuffer overlay = new PixelBuffer(1, 1, [255, 255, 255, 128]);

        background.BlendFrom(overlay, 0, 0);

        Assert.Equal(new byte[] { 128, 128, 128, 255 }, background.Pixels);
    }
}

public class PlatformServicesTests
{
    [Fact]
    public void Current_ThrowsBeforeInitialize()
    {
        if (!PlatformServices.IsInitialized)
        {
            Assert.Throws<InvalidOperationException>(() => PlatformServices.Current);
        }
    }
}
