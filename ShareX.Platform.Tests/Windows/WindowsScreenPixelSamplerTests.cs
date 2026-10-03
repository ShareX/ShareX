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
using ShareX.Platform.Windows;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.Platform.Tests;

public sealed class WindowsScreenPixelSamplerTests
{
    [Fact]
    public void ClipsNegativeOriginsAndLeavesOutsidePixelsTransparent()
    {
        int calls = 0;
        using WindowsScreenPixelSampler sampler = new(new PlatformRectangle(-4, -2, 4, 3), area =>
        {
            calls++;
            Assert.Equal(new PlatformRectangle(-4, -2, 4, 3), area);
            PixelBuffer pixels = new(area.Width, area.Height);
            for (int i = 0; i < pixels.Pixels.Length; i += 4)
            {
                pixels.Pixels[i] = 17;
                pixels.Pixels[i + 1] = 34;
                pixels.Pixels[i + 2] = 51;
                pixels.Pixels[i + 3] = 255;
            }
            return pixels;
        });

        PixelBuffer result = sampler.Read(new PlatformRectangle(-5, -3, 6, 5))!;
        Assert.Equal(1, calls);
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                int offset = (y * 6 + x) * 4;
                bool inside = x >= 1 && x <= 4 && y >= 1 && y <= 3;
                Assert.Equal(inside ? (byte)17 : (byte)0, result.Pixels[offset]);
                Assert.Equal(inside ? (byte)255 : (byte)0, result.Pixels[offset + 3]);
            }
        }
    }

    [Fact]
    public void ReadsLivePixelsAgainAndNeverCreatesASnapshot()
    {
        byte value = 0;
        using WindowsScreenPixelSampler sampler = new(new PlatformRectangle(0, 0, 1, 1), _ =>
            new PixelBuffer(1, 1, [++value, 0, 0, 255]));
        Assert.True(sampler.IsLive);
        Assert.Null(sampler.Snapshot);
        Assert.Equal(1, sampler.Read(new PlatformRectangle(0, 0, 1, 1))!.Pixels[0]);
        Assert.Equal(2, sampler.Read(new PlatformRectangle(0, 0, 1, 1))!.Pixels[0]);
    }

    [Fact]
    public void InvalidOrOutsideAreasDoNotCallGdi()
    {
        using WindowsScreenPixelSampler sampler = new(new PlatformRectangle(-10, -10, 5, 5),
            _ => throw new InvalidOperationException("Must not capture outside the desktop."));
        Assert.Null(sampler.Read(PlatformRectangle.Empty));
        Assert.All(sampler.Read(new PlatformRectangle(0, 0, 2, 2))!.Pixels, value => Assert.Equal(0, value));
    }

    [Fact]
    public void UnreadablePixelsReturnNullAndDisposedSessionsRejectReads()
    {
        WindowsScreenPixelSampler sampler = new(new PlatformRectangle(0, 0, 1, 1),
            _ => throw new InvalidOperationException("BitBlt failed."));
        Assert.Null(sampler.Read(new PlatformRectangle(0, 0, 1, 1)));
        sampler.Dispose();
        sampler.Dispose();
        Assert.Throws<ObjectDisposedException>(() => sampler.Read(new PlatformRectangle(0, 0, 1, 1)));
    }

    [WindowsFact]
    public async Task WindowsServiceCreatesLiveSessionWithoutCapturingTheDesktop()
    {
        WindowsScreenCaptureService service = new();
        using IScreenPixelSampler sampler = await service.CreatePixelSamplerAsync();
        Assert.True(sampler.IsLive);
        Assert.Null(sampler.Snapshot);
        Assert.Equal(WindowsScreenCaptureService.GetVirtualScreen(), sampler.Bounds);
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        await Assert.ThrowsAsync<OperationCanceledException>(() => service.CreatePixelSamplerAsync(cancellation.Token));
    }
}
