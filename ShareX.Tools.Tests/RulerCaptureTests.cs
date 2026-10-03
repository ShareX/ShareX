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
using ShareX.Platform;
using ShareX.Platform.Imaging;
using ShareX.Tools.Ruler;
using System.Buffers.Binary;
using Xunit;
using Rectangle = System.Drawing.Rectangle;

namespace ShareX.Tools.Tests;

public sealed class RulerCaptureTests
{
    [Fact]
    public async Task CaptureAwaitsOneRegionRequestAndUsesReturnedBounds()
    {
        CaptureService service = new();
        using CancellationTokenSource cancellation = new();
        Task<ScreenPixelBuffer> pending = ScreenPixelBuffer.CaptureAsync(service, new PixelRect(-8, -3, 10, 8), cancellation.Token);

        Assert.False(pending.IsCompleted);
        Assert.Equal(1, service.Calls);
        Assert.Equal(ScreenCaptureMode.Region, service.Request!.Mode);
        Assert.Equal(new PlatformRectangle(-8, -3, 10, 8), service.Request.Region);
        Assert.False(service.Request.IncludeCursor);
        Assert.Equal(cancellation.Token, service.CancellationToken);

        service.Completion.SetResult(new ScreenCaptureResult(Image(6, 4, new Rectangle(1, 1, 3, 2)),
            new PlatformRectangle(-6, -2, 6, 4), "fixture"));
        ScreenPixelBuffer screen = await pending;

        Assert.Equal(new PixelRect(-6, -2, 6, 4), screen.Bounds);
        Assert.Equal(new Rectangle(-5, -1, 3, 1), screen.FindColorRun(new PixelPoint(-4, -1), true, 0));
        Assert.Equal(new Rectangle(-4, -1, 1, 2), screen.FindColorRun(new PixelPoint(-4, -1), false, 0));
        Assert.Equal(new Rectangle(-5, -1, 3, 2), screen.FindContentBounds(new Rectangle(-8, -3, 10, 8), 0));
        Assert.Equal(Rectangle.Empty, screen.FindColorRun(new PixelPoint(-7, -1), true, 0));
        Assert.Equal(1, service.Calls);
    }

    [Fact]
    public void PngAndPixelCapturesPreserveColorChannelsAndIgnoreAlphaWhenMeasuring()
    {
        PixelBuffer image = Image(6, 4, new Rectangle(1, 1, 3, 2));
        PlatformRectangle bounds = new(-10, 5, 6, 4);
        ScreenPixelBuffer direct = ScreenPixelBuffer.FromCapture(new ScreenCaptureResult(image, bounds, "pixels"));
        ScreenPixelBuffer png = ScreenPixelBuffer.FromCapture(new ScreenCaptureResult(PngCodec.Encode(image), bounds, "png"));

        Rectangle expected = new(-9, 6, 3, 2);
        Rectangle selection = new(-10, 5, 6, 4);
        Assert.Equal(expected, direct.FindContentBounds(selection, 0));
        Assert.Equal(expected, png.FindContentBounds(selection, 0));
        Assert.Equal(new Rectangle(-9, 6, 3, 1), png.FindColorRun(new PixelPoint(-8, 6), true, 0));
        // The source buffer is owned by the platform, so measuring must not make it opaque or otherwise mutate it.
        Assert.Equal((byte)128, image.Pixels[(6 + 1) * 4 + 3]);
    }

    [Theory]
    [InlineData(8, 6, 4, 3, 2, 2, 4, 2, 1, 1, 2, 1)]
    [InlineData(2, 2, 4, 4, 1, 1, 1, 1, 2, 2, 2, 2)]
    [InlineData(6, 6, 4, 4, 2, 2, 3, 3, 2, 2, 2, 2)]
    public void ScaledImagesSampleInDesktopCoordinatesWithoutBlurringEdges(
        int imageWidth, int imageHeight, int boundsWidth, int boundsHeight,
        int contentX, int contentY, int contentWidth, int contentHeight,
        int expectedX, int expectedY, int expectedWidth, int expectedHeight)
    {
        ScreenPixelBuffer screen = ScreenPixelBuffer.FromCapture(new ScreenCaptureResult(
            Image(imageWidth, imageHeight, new Rectangle(contentX, contentY, contentWidth, contentHeight)),
            new PlatformRectangle(-2, 6, boundsWidth, boundsHeight), "scaled fixture"));
        Rectangle expected = new(expectedX - 2, expectedY + 6, expectedWidth, expectedHeight);
        Assert.Equal(expected, screen.FindContentBounds(new Rectangle(-2, 6, boundsWidth, boundsHeight), 0));
        Assert.Equal(new Rectangle(expected.X, expected.Y, expected.Width, 1),
            screen.FindColorRun(new PixelPoint(expected.X, expected.Y), true, 0));
    }

    [Fact]
    public async Task UnsupportedCaptureReportsServiceReasonWithoutStartingCapture()
    {
        CaptureService service = new() { Support = FeatureSupport.NotSupported("Capture is unavailable in this session.") };
        PlatformNotSupportedException exception = await Assert.ThrowsAsync<PlatformNotSupportedException>(
            () => ScreenPixelBuffer.CaptureAsync(service, new PixelRect(0, 0, 4, 4)));
        Assert.Equal(service.Support.Reason, exception.Message);
        Assert.Equal(0, service.Calls);
    }

    [Fact]
    public async Task CancelledCaptureDoesNotReturnPixelsWhenBackendCompletesLate()
    {
        CaptureService service = new();
        using CancellationTokenSource cancellation = new();
        Task<ScreenPixelBuffer> pending = ScreenPixelBuffer.CaptureAsync(service, new PixelRect(0, 0, 4, 4), cancellation.Token);
        cancellation.Cancel();
        service.Completion.SetResult(new ScreenCaptureResult(Image(4, 4, new Rectangle(1, 1, 2, 2)),
            new PlatformRectangle(0, 0, 4, 4), "late fixture"));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending);

        CaptureService alreadyCancelled = new();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => ScreenPixelBuffer.CaptureAsync(alreadyCancelled, new PixelRect(0, 0, 4, 4), cancellation.Token));
        Assert.Equal(0, alreadyCancelled.Calls);
    }

    private static PixelBuffer Image(int width, int height, Rectangle content)
    {
        PixelBuffer image = new(width, height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Background blue=40, green=80, red=120; foreground swaps red and blue (same green).
                int color = content.Contains(x, y) ? unchecked((int)0x80285078) : unchecked((int)0xFF785028);
                BinaryPrimitives.WriteInt32LittleEndian(image.Pixels.AsSpan((x + y * width) * 4, 4), color);
            }
        }
        return image;
    }

    private sealed class CaptureService : IScreenCaptureService
    {
        public FeatureSupport Support { get; set; } = FeatureSupport.Supported;
        public ScreenCaptureFeatures Features => ScreenCaptureFeatures.None;
        public int Calls { get; private set; }
        public ScreenCaptureRequest? Request { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
        public TaskCompletionSource<ScreenCaptureResult> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public PermissionState GetPermissionState() => PermissionState.NotRequired;
        public bool RequestPermission() => true;
        public IReadOnlyList<ScreenInfo> GetScreens() => [];
        public CursorCapture? CaptureCursor() => null;
        public Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, CancellationToken cancellationToken = default)
        {
            Calls++;
            Request = request;
            CancellationToken = cancellationToken;
            return Completion.Task;
        }
    }
}
