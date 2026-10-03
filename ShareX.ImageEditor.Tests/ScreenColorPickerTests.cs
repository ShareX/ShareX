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
using Avalonia.Media;
using ShareX.AvaloniaUI.Windows;
using ShareX.Platform;
using ShareX.Platform.Imaging;
using Xunit;

namespace ShareX.ImageEditor.Tests;

public sealed class ScreenColorPickerTests
{
    [Theory]
    [InlineData(-1920, -1080, 3840, 2160, 0, 0, 3840, 2160, -1920, -1080, 0, 0)]
    [InlineData(-1920, -1080, 3840, 2160, 0, 0, 3840, 2160, -1, -1, 1919, 1079)]
    [InlineData(100, -400, 1600, 900, 0, 0, 2000, 1125, 100, -400, 0, 0)]
    [InlineData(100, -400, 1600, 900, 0, 0, 2000, 1125, 101, -399, 1, 1)]
    [InlineData(100, -400, 1600, 900, 0, 0, 2000, 1125, 899, 49, 999, 561)]
    [InlineData(0, 0, 1000, 800, -10, 20, 1500, 1200, 666, 533, 989, 820)]
    public void FrozenDesktopMapsPointerToTheDisplayedCapture(int x, int y, int width, int height,
        int sampleX, int sampleY, int sampleWidth, int sampleHeight, int pointerX, int pointerY, int expectedX, int expectedY)
    {
        PlatformPoint mapped = ScreenColorPickerPixels.MapPosition(new PixelPoint(pointerX, pointerY),
            new PlatformRectangle(x, y, width, height), new PlatformRectangle(sampleX, sampleY, sampleWidth, sampleHeight), false);
        Assert.Equal(new PlatformPoint(expectedX, expectedY), mapped);
    }

    [Fact]
    public void LiveReadsKeepPhysicalDesktopCoordinates()
    {
        Assert.Equal(new PlatformPoint(-100, 25), ScreenColorPickerPixels.MapPosition(new PixelPoint(-100, 25),
            new PlatformRectangle(-1920, 0, 3840, 2160), new PlatformRectangle(0, 0, 1920, 1080), true));
    }

    [Fact]
    public void MagnifierUsesTheCenterPixelAndRejectsTransparentPadding()
    {
        PixelBuffer pixels = new(15, 15);
        int center = (7 * 15 + 7) * 4;
        pixels.Pixels[center] = 100;
        pixels.Pixels[center + 1] = 200;
        pixels.Pixels[center + 2] = 255;
        Assert.False(ScreenColorPickerPixels.TryGetCenterColor(pixels, out _));
        pixels.Pixels[center + 3] = 255;
        Assert.True(ScreenColorPickerPixels.TryGetCenterColor(pixels, out Color color));
        Assert.Equal(Color.FromRgb(255, 200, 100), color);
        Assert.False(ScreenColorPickerPixels.TryGetCenterColor(null, out _));
        Assert.True(ScreenColorPickerPixels.TryGetCenterColor(new PixelBuffer(1, 1, [0, 0, 0, 255]), out color));
        Assert.Equal(Colors.Black, color);
    }

    [Fact]
    public void SnapshotAndMagnifierPixelsArePremultipliedWithoutChangingTheSource()
    {
        byte[] original = [255, 128, 64, 128, 255, 128, 64, 0, 10, 20, 30, 255];
        PixelBuffer pixels = new(3, 1, (byte[])original.Clone());
        Assert.Equal(new byte[] { 128, 64, 32, 128, 0, 0, 0, 0, 10, 20, 30, 255 }, ScreenColorPickerPixels.Premultiply(pixels));
        Assert.Equal(original, pixels.Pixels);
    }
}
