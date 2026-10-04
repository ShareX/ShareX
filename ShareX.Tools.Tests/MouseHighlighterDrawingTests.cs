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

using System.Runtime.Versioning;
using System.Drawing;
using System.Runtime.InteropServices;
using ShareX.Platform;
using ShareX.Platform.Windows;
using Xunit;

namespace ShareX.Tools.Tests;

public sealed class MouseHighlighterDrawingTests
{
    [Fact]
    public void CircleUsesPremultipliedBgraAndClipsAtANegativeMonitorOrigin()
    {
        MouseHighlighterOptions options = CircleOptions();
        MouseHighlighterFrame frame = new(options, new Point(-99, -60), [], 0);
        using BufferOverlay buffer = new();
        using MouseHighlighterOverlayWindow drawing = new(new Rectangle(-100, -60, 20, 20), buffer, () => frame);
        drawing.Refresh();

        Assert.Equal(new PlatformRectangle(-100, -60, 9, 8), Assert.Single(buffer.Presentations));
        Assert.Equal(new byte[] { 32, 64, 128, 128 }, buffer.Pixel(1, 0));
        Assert.Equal(new byte[] { 0, 0, 0, 0 }, buffer.Pixel(0, 7));
        AssertUnusedAreaIsTransparent(buffer);
        frame = frame with { CursorPosition = new Point(-500, -500) };
        drawing.Refresh();
        Assert.Equal(1, buffer.Hides);
        Assert.Equal(1, buffer.Reads);
    }

    [Fact]
    public void ReleasedCirclePreservesDelayFadeAndZeroDurationBehavior()
    {
        MouseHighlighterOptions options = CircleOptions();
        options.AlwaysColor = Color.Transparent;
        options.PrimaryColor = Color.FromArgb(128, 255, 128, 64);
        options.FadeDelay = 100;
        options.FadeDuration = 100;
        MouseHighlight highlight = new() { Position = new Point(30, 40), Button = MouseHighlightButton.Primary, Started = 0, Released = 0 };
        MouseHighlighterFrame frame = new(options, default, [highlight], 50);
        using BufferOverlay buffer = new();
        using MouseHighlighterOverlayWindow drawing = new(new Rectangle(0, 0, 100, 100), buffer, () => frame);
        drawing.Refresh();
        Assert.Equal(new byte[] { 32, 64, 128, 128 }, buffer.Pixel(7, 7));
        frame = frame with { Time = 150 };
        drawing.Refresh();
        Assert.Equal(new byte[] { 16, 32, 64, 64 }, buffer.Pixel(7, 7));
        frame = frame with { Time = 200 };
        drawing.Refresh();
        Assert.All(buffer.Bytes(), value => Assert.Equal(0, value));
        options.FadeDuration = 0;
        frame = frame with { Time = 100 };
        drawing.Refresh();
        Assert.All(buffer.Bytes(), value => Assert.Equal(0, value));
    }

    [Fact]
    public void ReusedAndReallocatedBuffersClearOldEffectsAndDisposeOnce()
    {
        MouseHighlighterOptions options = CircleOptions();
        options.AlwaysColor = Color.Transparent;
        options.PrimaryColor = Color.FromArgb(128, 255, 128, 64);
        MouseHighlight first = new() { Position = new Point(30, 30), Button = MouseHighlightButton.Primary };
        MouseHighlight second = new() { Position = new Point(50, 30), Button = MouseHighlightButton.Primary };
        MouseHighlighterFrame frame = new(options, default, [first, second], 0);
        int stateReads = 0;
        using BufferOverlay buffer = new();
        using MouseHighlighterOverlayWindow drawing = new(new Rectangle(0, 0, 200, 200), buffer, () => { stateReads++; return frame; });
        drawing.Refresh();
        IntPtr initial = buffer.Current.Pixels;
        Assert.Equal((byte)128, buffer.Pixel(27, 7)[3]);
        frame = frame with { Highlights = new[] { first } };
        drawing.Refresh();
        Assert.Equal(initial, buffer.Current.Pixels);
        Assert.Equal(new byte[] { 0, 0, 0, 0 }, buffer.Pixel(27, 7));
        AssertUnusedAreaIsTransparent(buffer);

        options.Radius = 50;
        drawing.Refresh();
        Assert.NotEqual(initial, buffer.Current.Pixels);
        Assert.Equal(2, buffer.Allocations);
        Assert.Equal(new byte[] { 32, 64, 128, 128 }, buffer.Pixel(30, 30));
        Assert.Equal(new PlatformRectangle(0, 0, 83, 83), buffer.Presentations[^1]);
        drawing.Dispose();
        drawing.Dispose();
        drawing.Refresh();
        Assert.Equal(1, buffer.Disposals);
        Assert.Equal(3, buffer.Reads);
        Assert.Equal(3, stateReads);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RippleAndReleaseCrosshairsDrawWithinTheirNativeArea(bool crosshairs)
    {
        MouseHighlighterOptions options = new() { Mode = MouseHighlightMode.Ripple, RippleSize = 40, RippleDuration = 200 };
        MouseHighlight highlight = new()
        {
            Position = new Point(50, 60), Button = MouseHighlightButton.Secondary,
            Started = 0, Released = 100, Crosshairs = crosshairs
        };
        MouseHighlighterFrame frame = new(options, default, [highlight], 200);
        using BufferOverlay buffer = new();
        using MouseHighlighterOverlayWindow drawing = new(new Rectangle(0, 0, 200, 200), buffer, () => frame);
        drawing.Refresh();
        Assert.Equal(new PlatformRectangle(18, 28, 65, 65), Assert.Single(buffer.Presentations));
        Assert.Equal((byte)0, buffer.Pixel(0, 0)[3]);
        if (crosshairs)
        {
            Assert.Equal((byte)0, buffer.Pixel(32, 32)[3]);
            Assert.True(buffer.Pixel(49, 32)[3] > 0);
        }
        else Assert.True(buffer.Pixel(32, 32)[3] > 0);
        AssertUnusedAreaIsTransparent(buffer);
        frame = frame with { Time = 400 };
        drawing.Refresh();
        Assert.All(buffer.Bytes(), value => Assert.Equal(0, value));
    }

    [WindowsMouseOverlayFact]
    [SupportedOSPlatform("windows")]
    public void HiddenWindowsDibUsesTheSamePixelsAndRejectsUseAfterDisposal()
    {
        IScreenOverlay native = new WindowsWindowService().CreateOverlay(new PlatformRectangle(-100, -60, 20, 20));
        using BufferOverlay buffer = new(native);
        MouseHighlighterFrame frame = new(CircleOptions(), new Point(-99, -60), [], 0);
        using MouseHighlighterOverlayWindow drawing = new(new Rectangle(-100, -60, 20, 20), buffer, () => frame);
        // Present is intercepted by the fixture, so the real native window stays hidden.
        drawing.Refresh();
        Assert.Equal(new byte[] { 32, 64, 128, 128 }, buffer.Pixel(1, 0));
        Assert.Equal(new byte[] { 0, 0, 0, 0 }, buffer.Pixel(0, 7));
        Assert.Equal(new PlatformRectangle(-100, -60, 9, 8), Assert.Single(buffer.Presentations));
        Assert.True(buffer.Current.Stride >= 9 * 4);
        Assert.True(buffer.Current.Height >= 8);
        AssertUnusedAreaIsTransparent(buffer);
        drawing.Dispose();
        native.Dispose();
        native.Hide();
        Assert.Throws<ObjectDisposedException>(() => native.GetBuffer(10, 10));
        Assert.Throws<ObjectDisposedException>(() => native.Present(new PlatformRectangle(0, 0, 10, 10)));
        drawing.Refresh();
        Assert.Equal(1, buffer.Disposals);
        Assert.Equal(1, buffer.Reads);
    }

    private static MouseHighlighterOptions CircleOptions() => new()
    {
        Mode = MouseHighlightMode.Circle, Radius = 5, AlwaysColor = Color.FromArgb(128, 255, 128, 64)
    };

    private static void AssertUnusedAreaIsTransparent(BufferOverlay buffer)
    {
        PlatformRectangle area = buffer.Presentations[^1];
        byte[] pixels = buffer.Bytes();
        for (int y = 0; y < buffer.Current.Height; y++)
        {
            for (int x = 0; x < buffer.Current.Stride / 4; x++)
            {
                if (x >= area.Width || y >= area.Height)
                {
                    int offset = y * buffer.Current.Stride + x * 4;
                    Assert.Equal(new byte[] { 0, 0, 0, 0 }, pixels.AsSpan(offset, 4).ToArray());
                }
            }
        }
    }

    private sealed class BufferOverlay(IScreenOverlay? native = null) : IScreenOverlay
    {
        private bool _disposed;
        public OverlayBuffer Current { get; private set; }
        public List<PlatformRectangle> Presentations { get; } = [];
        public int Reads { get; private set; }
        public int Hides { get; private set; }
        public int Allocations { get; private set; }
        public int Disposals { get; private set; }

        public OverlayBuffer GetBuffer(int width, int height)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            Reads++;
            if (native != null) return Current = native.GetBuffer(width, height);
            if (Current.Pixels == IntPtr.Zero || Current.Stride < width * 4 || Current.Height < height)
            {
                int stride = (width + 3) / 4 * 16, rows = (height + 3) / 4 * 4;
                // Allocate first so a resize cannot accidentally return the old pointer.
                IntPtr allocation = Marshal.AllocHGlobal(checked(stride * rows));
                if (Current.Pixels != IntPtr.Zero) Marshal.FreeHGlobal(Current.Pixels);
                Current = new OverlayBuffer(allocation, stride, rows);
                Allocations++;
            }
            return Current;
        }

        public void Present(PlatformRectangle area) => Presentations.Add(area);
        public void Hide() => Hides++;
        public byte[] Bytes()
        {
            byte[] result = new byte[checked(Current.Stride * Current.Height)];
            Marshal.Copy(Current.Pixels, result, 0, result.Length);
            return result;
        }
        public byte[] Pixel(int x, int y)
        {
            byte[] result = new byte[4];
            Marshal.Copy(IntPtr.Add(Current.Pixels, y * Current.Stride + x * 4), result, 0, 4);
            return result;
        }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Disposals++;
            if (native != null) native.Dispose();
            else if (Current.Pixels != IntPtr.Zero) Marshal.FreeHGlobal(Current.Pixels);
            Current = default;
        }
    }
}

public sealed class WindowsMouseOverlayFactAttribute : FactAttribute
{
    public WindowsMouseOverlayFactAttribute()
    {
        if (!OperatingSystem.IsWindows()) Skip = "Requires a fixture-owned hidden Windows overlay buffer.";
    }
}
