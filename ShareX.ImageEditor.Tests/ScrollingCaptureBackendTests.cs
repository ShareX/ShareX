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

using ShareX.Platform;
using ShareX.ScreenCaptureLib;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.ImageEditor.Tests;

/// <summary>R33: the scrolling capture backend rechecks the desktop before each native step and stops cleanly.</summary>
public sealed class ScrollingCaptureBackendTests
{
    private sealed class FakeInput : IInputService
    {
        public FeatureSupport KeyboardSupport { get; set; } = FeatureSupport.Supported;
        public FeatureSupport MouseWheelSupport { get; set; } = FeatureSupport.Supported;
        public FeatureSupport WindowScrollSupport { get; set; } = FeatureSupport.Supported;
        public FeatureSupport MouseHookSupport => FeatureSupport.NotSupported("Not used.");
        public List<string> Sent { get; } = new List<string>();

        public bool SendKeyPress(int virtualKey) { Sent.Add("key:" + virtualKey); return true; }
        public bool SendMouseWheel(int detents) { Sent.Add("wheel:" + detents); return true; }
        public bool ScrollWindow(long windowHandle, WindowScrollCommand command) { Sent.Add("scroll:" + command); return true; }
        public IDisposable HookMouse(IGlobalMouseListener listener) => throw new NotSupportedException();
    }

    private sealed class FakeHost : ScrollingCaptureHost
    {
        public FakeInput FakeInput { get; } = new FakeInput();
        public FeatureSupport Capture { get; set; } = FeatureSupport.Supported;
        public Func<int, CancellationToken, Task> OnDelay { get; set; } = (_, _) => Task.CompletedTask;
        public Func<Task<(Rectangle Rectangle, PlatformWindow? Window)?>> OnSelect { get; set; } =
            () => Task.FromResult<(Rectangle, PlatformWindow?)?>((new Rectangle(0, 0, 40, 40), new PlatformWindow(7, "Page", null, null, new PlatformRectangle(0, 0, 40, 40), false)));
        public int Captures { get; private set; }
        public Action<int>? AfterCapture { get; set; }

        public override FeatureSupport CaptureSupport => Capture;
        public override IInputService Input => FakeInput;
        public override void ActivateWindow(long handle) { }
        public override IDisposable? ShowRegion(Rectangle rectangle) => null;
        public override Task Delay(int milliseconds, CancellationToken cancellationToken) => OnDelay(milliseconds, cancellationToken);
        public override Task<(Rectangle Rectangle, PlatformWindow? Window)?> SelectAsync() => OnSelect();

        public override Task<SKBitmap?> CaptureAsync(Rectangle rectangle)
        {
            Captures++;
            // Each capture shows different content so the loop does not end on "no change".
            SKBitmap bitmap = new SKBitmap(rectangle.Width, rectangle.Height);
            bitmap.Erase(new SKColor((byte)(Captures * 40), 0, 0));
            AfterCapture?.Invoke(Captures);
            return Task.FromResult<SKBitmap?>(bitmap);
        }
    }

    private static async Task<ScrollingCaptureManager> SelectedAsync(FakeHost host, ScrollingCaptureOptions options)
    {
        ScrollingCaptureManager manager = new ScrollingCaptureManager(options, host);
        Assert.True(await manager.SelectWindowAsync());
        return manager;
    }

    [Fact]
    public async Task AutoTopSendsOnlyTheInputThisDesktopAllows()
    {
        FakeHost host = new FakeHost();
        host.FakeInput.WindowScrollSupport = FeatureSupport.NotSupported("Wayland");
        host.AfterCapture = n => { };
        ScrollingCaptureOptions options = new ScrollingCaptureOptions { AutoScrollTop = true, ScrollMethod = ScrollMethod.PageDown, ShowRegion = false };
        using ScrollingCaptureManager manager = await SelectedAsync(host, options);
        host.AfterCapture = n => { if (n == 1) manager.StopCapture(); };

        await manager.StartCapture();

        Assert.Equal("key:" + VirtualKeys.Home, host.FakeInput.Sent[0]);
        Assert.DoesNotContain("scroll:" + WindowScrollCommand.Top, host.FakeInput.Sent);
    }

    [Fact]
    public async Task WindowsKeepsBothAutoTopActions()
    {
        FakeHost host = new FakeHost();
        ScrollingCaptureOptions options = new ScrollingCaptureOptions { AutoScrollTop = true, ScrollMethod = ScrollMethod.MouseWheel, ShowRegion = false };
        using ScrollingCaptureManager manager = await SelectedAsync(host, options);
        host.AfterCapture = n => { if (n == 1) manager.StopCapture(); };

        await manager.StartCapture();

        Assert.Equal(["key:" + VirtualKeys.Home, "scroll:" + WindowScrollCommand.Top], host.FakeInput.Sent[..2]);
    }

    [Fact]
    public async Task SupportLostDuringStartDelaySendsNothing()
    {
        FakeHost host = new FakeHost();
        host.OnDelay = (_, _) =>
        {
            host.FakeInput.MouseWheelSupport = FeatureSupport.NotSupported("The desktop stopped allowing input.");
            return Task.CompletedTask;
        };
        ScrollingCaptureOptions options = new ScrollingCaptureOptions { AutoScrollTop = true, ScrollMethod = ScrollMethod.MouseWheel, ShowRegion = false };
        using ScrollingCaptureManager manager = await SelectedAsync(host, options);

        ScrollingCaptureStatus status = await manager.StartCapture();

        Assert.Equal(ScrollingCaptureStatus.Failed, status);
        Assert.Equal("The desktop stopped allowing input.", manager.FailureReason);
        Assert.Empty(host.FakeInput.Sent);
        Assert.Equal(0, host.Captures);
    }

    [Fact]
    public async Task SupportLostBetweenStepsStopsBeforeTheNextInput()
    {
        FakeHost host = new FakeHost();
        ScrollingCaptureOptions options = new ScrollingCaptureOptions { ScrollMethod = ScrollMethod.DownArrow, ScrollAmount = 2, ShowRegion = false };
        using ScrollingCaptureManager manager = await SelectedAsync(host, options);
        host.AfterCapture = n => { if (n == 2) host.Capture = FeatureSupport.NotSupported("Capture is no longer available."); };

        await manager.StartCapture();

        Assert.Equal(2, host.FakeInput.Sent.Count);
        Assert.Equal(2, host.Captures);
        Assert.Equal("Capture is no longer available.", manager.FailureReason);
        Assert.NotNull(manager.Result);
    }

    [Fact]
    public async Task DisposeEndsAWaitingCaptureAndDropsTheResult()
    {
        FakeHost host = new FakeHost();
        TaskCompletionSource waiting = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        host.OnDelay = (ms, token) =>
        {
            waiting.TrySetResult();
            return Task.Delay(Timeout.Infinite, token);
        };
        ScrollingCaptureManager manager = await SelectedAsync(host, new ScrollingCaptureOptions { ShowRegion = false });

        Task<ScrollingCaptureStatus> capture = manager.StartCapture();
        await waiting.Task.WaitAsync(TimeSpan.FromSeconds(5));
        manager.Dispose();

        Assert.Equal(ScrollingCaptureStatus.Failed, await capture.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.Empty(host.FakeInput.Sent);
        Assert.Null(manager.Result);
        Assert.False(manager.IsCapturing);
    }

    [Fact]
    public async Task SelectionFinishingAfterDisposeIsIgnored()
    {
        FakeHost host = new FakeHost();
        TaskCompletionSource<(Rectangle Rectangle, PlatformWindow? Window)?> selector = new(TaskCreationOptions.RunContinuationsAsynchronously);
        host.OnSelect = () => selector.Task;
        ScrollingCaptureManager manager = new ScrollingCaptureManager(new ScrollingCaptureOptions(), host);

        Task<bool> selecting = manager.SelectWindowAsync();
        manager.Dispose();
        selector.SetResult((new Rectangle(0, 0, 10, 10), new PlatformWindow(1, "x", null, null, new PlatformRectangle(0, 0, 10, 10), false)));

        Assert.False(await selecting);
        Assert.Equal(ScrollingCaptureStatus.Failed, await manager.StartCapture());
        Assert.Equal(0, host.Captures);
    }
}
