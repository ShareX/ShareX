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
using Xunit;

namespace ShareX.Platform.Tests;

public class SnapshotPixelSamplerTests
{
    /// <summary>A 5x5 capture whose pixel at (x, y) has blue = x, green = y.</summary>
    private static PixelBuffer CreateGrid(int width, int height)
    {
        PixelBuffer buffer = new PixelBuffer(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = (y * width + x) * 4;
                buffer.Pixels[i] = (byte)x;
                buffer.Pixels[i + 1] = (byte)y;
                buffer.Pixels[i + 3] = 255;
            }
        }

        return buffer;
    }

    [Fact]
    public void ReadsOneToOne()
    {
        SnapshotPixelSampler sampler = new SnapshotPixelSampler(CreateGrid(5, 5), new PlatformRectangle(10, 20, 5, 5));

        PixelBuffer? area = sampler.Read(new PlatformRectangle(11, 22, 2, 1));

        Assert.NotNull(area);
        Assert.Equal(new byte[] { 1, 2, 0, 255, 2, 2, 0, 255 }, area!.Pixels);
        Assert.False(sampler.IsLive);
    }

    [Fact]
    public void ScaledCaptureReadsNearestPixel()
    {
        // A 125% Wayland output: 4 screen coordinates captured as 5 pixels.
        SnapshotPixelSampler sampler = new SnapshotPixelSampler(CreateGrid(5, 5), new PlatformRectangle(0, 0, 4, 4));

        PixelBuffer area = sampler.Read(new PlatformRectangle(0, 0, 4, 1))!;

        Assert.Equal(new byte[] { 0, 1, 3, 4 }, new[] { area.Pixels[0], area.Pixels[4], area.Pixels[8], area.Pixels[12] });
    }

    [Fact]
    public void OutsideTheScreenIsTransparent()
    {
        SnapshotPixelSampler sampler = new SnapshotPixelSampler(CreateGrid(5, 5), new PlatformRectangle(0, 0, 5, 5));

        PixelBuffer area = sampler.Read(new PlatformRectangle(-1, 0, 2, 1))!;

        Assert.Equal(0, area.Pixels[3]);
        Assert.Equal(255, area.Pixels[7]);
    }
}
