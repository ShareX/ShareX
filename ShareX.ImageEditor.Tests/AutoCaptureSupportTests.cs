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
using System.Drawing;
using Xunit;
using Strings = ShareX.ScreenCaptureLib.Localization.Strings;

namespace ShareX.ImageEditor.Tests;

public sealed class AutoCaptureSupportTests
{
    private static readonly Rectangle Region = new(10, 20, 30, 40);

    [Fact]
    public async Task UnsupportedActionsPreserveSettingsAndNeverStartReaders()
    {
        AutoCaptureWindowViewModel model = new(() => FeatureSupport.NotSupported("Install a legacy screenshot tool."));
        Rectangle savedRegion = Region;
        decimal repeat = decimal.MaxValue;
        bool minimize = true, wait = true;
        int actions = 0;
        Assert.Equal(Strings.AutoCaptureWindow_CaptureUnavailable, model.Support.Reason);
        Assert.False(model.CanEdit);
        Assert.False(model.TryStart(Region));
        Assert.False(model.TryChange(() => { repeat = 1; minimize = false; wait = false; }));
        Assert.False(await model.TrySelectRegionAsync(() => { actions++; return Task.FromResult<Rectangle?>(new Rectangle(1, 2, 3, 4)); }, r => savedRegion = r));
        Assert.False(await model.TryCaptureAsync(Region, _ => { actions++; return Task.FromResult<SKBitmap?>(null); }, _ => actions++));
        model.Stop();
        Assert.False(model.IsRunning);
        Assert.Equal(Region, savedRegion);
        Assert.Equal(decimal.MaxValue, repeat);
        Assert.True(minimize);
        Assert.True(wait);
        Assert.Equal(0, actions);
    }

    [Fact]
    public async Task SupportedCaptureKeepsCoordinatesAndTransfersTheFrameOnce()
    {
        AutoCaptureWindowViewModel model = new(() => FeatureSupport.Supported);
        using SKBitmap bitmap = new(1, 1);
        int captures = 0, publications = 0;
        Assert.False(model.TryStart(new Rectangle(1, 2, 0, 4)));
        Assert.False(model.TryStart(new Rectangle(1, 2, 3, -1)));
        Assert.True(model.TryStart(Region));
        Assert.False(model.TryStart(Region));
        Assert.True(await model.TryCaptureAsync(Region, region =>
        {
            Assert.Equal(Region, region);
            captures++;
            return Task.FromResult<SKBitmap?>(bitmap);
        }, frame => { Assert.Same(bitmap, frame); publications++; }));
        Assert.NotEqual(IntPtr.Zero, bitmap.Handle);
        Assert.Equal(1, captures);
        Assert.Equal(1, publications);
        Assert.False(model.IsBusy);
        Assert.True(model.IsRunning);
        model.Stop();
        Assert.False(model.IsRunning);
        Assert.True(model.CanStart(Region));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task StopCloseOrSupportLossDisposesPendingFramesAndPreventsRestartOverlap(int interruption)
    {
        FeatureSupport support = FeatureSupport.Supported;
        AutoCaptureWindowViewModel model = new(() => support);
        TaskCompletionSource<SKBitmap?> result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int readers = 0, publications = 0;
        Assert.True(model.TryStart(Region));
        Task<bool> capture = model.TryCaptureAsync(Region, _ => { readers++; return result.Task; }, _ => publications++);
        Assert.True(model.IsCapturing);
        Assert.False(model.TryChange(() => publications++));
        Assert.False(await model.TrySelectRegionAsync(() => { readers++; return Task.FromResult<Rectangle?>(Region); }, _ => publications++));
        Assert.False(await model.TryCaptureAsync(Region, _ => { readers++; return Task.FromResult<SKBitmap?>(null); }, _ => publications++));
        if (interruption == 0) model.Stop();
        else if (interruption == 1) model.Close();
        else support = FeatureSupport.NotSupported("Fixture desktop ended.");
        Assert.False(model.TryStart(Region));
        SKBitmap abandoned = new(1, 1);
        result.SetResult(abandoned);
        Assert.False(await capture);
        Assert.Equal(IntPtr.Zero, abandoned.Handle);
        Assert.False(model.IsBusy);
        Assert.False(model.IsRunning);
        Assert.Equal(1, readers);
        Assert.Equal(0, publications);
        Assert.Equal(interruption == 0, model.CanStart(Region));
        support = FeatureSupport.Supported;
        Assert.Equal(interruption != 1, model.TryStart(Region));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LateSelectionsAfterCloseOrCapabilityLossKeepSavedRegion(bool close)
    {
        FeatureSupport support = FeatureSupport.Supported;
        AutoCaptureWindowViewModel model = new(() => support);
        TaskCompletionSource<Rectangle?> result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Rectangle saved = Region;
        int readers = 0;
        Task<bool> selection = model.TrySelectRegionAsync(() => { readers++; return result.Task; }, rectangle => saved = rectangle);
        Assert.True(model.IsSelectingRegion);
        Assert.False(model.TryStart(Region));
        Assert.False(model.TryChange(() => saved = Rectangle.Empty));
        Assert.False(await model.TrySelectRegionAsync(() => { readers++; return Task.FromResult<Rectangle?>(null); }, r => saved = r));
        if (close) model.Close();
        else support = FeatureSupport.NotSupported("Fixture capability ended.");
        result.SetResult(new Rectangle(1, 2, 3, 4));
        Assert.False(await selection);
        Assert.False(model.IsBusy);
        Assert.Equal(Region, saved);
        Assert.Equal(1, readers);
    }

    [Fact]
    public async Task SelectionCancellationInvalidRegionsAndErrorsClearBusyState()
    {
        AutoCaptureWindowViewModel model = new(() => FeatureSupport.Supported);
        Rectangle saved = Region;
        Assert.False(await model.TrySelectRegionAsync(() => Task.FromResult<Rectangle?>(null), r => saved = r));
        Assert.False(await model.TrySelectRegionAsync(() => Task.FromResult<Rectangle?>(new Rectangle(2, 3, 0, 4)), r => saved = r));
        InvalidOperationException error = new("Synthetic selector failure.");
        Assert.Same(error, await Assert.ThrowsAsync<InvalidOperationException>(() =>
            model.TrySelectRegionAsync(() => Task.FromException<Rectangle?>(error), r => saved = r)));
        Assert.False(model.IsBusy);
        Assert.Equal(Region, saved);
        Assert.True(await model.TrySelectRegionAsync(() => Task.FromResult<Rectangle?>(new Rectangle(1, 2, 3, 4)), r => saved = r));
        Assert.Equal(new Rectangle(1, 2, 3, 4), saved);
    }

    [Fact]
    public async Task ReaderAndPublicationFailuresLeaveNoBusyStateOrOwnedFrame()
    {
        AutoCaptureWindowViewModel model = new(() => FeatureSupport.Supported);
        Assert.True(model.TryStart(Region));
        InvalidOperationException error = new("Synthetic frame failure.");
        Assert.Same(error, await Assert.ThrowsAsync<InvalidOperationException>(() =>
            model.TryCaptureAsync(Region, _ => Task.FromException<SKBitmap?>(error), _ => throw new Exception("Must not publish."))));
        Assert.False(model.IsBusy);
        SKBitmap bitmap = new(1, 1);
        Assert.Same(error, await Assert.ThrowsAsync<InvalidOperationException>(() =>
            model.TryCaptureAsync(Region, _ => Task.FromResult<SKBitmap?>(bitmap), _ => throw error)));
        Assert.Equal(IntPtr.Zero, bitmap.Handle);
        Assert.False(model.IsBusy);
        model.Stop();
        Assert.True(model.TryStart(Region));
    }

    [Fact]
    public async Task SupportLossBeforeTheTickStopsWithoutInvokingCapture()
    {
        FeatureSupport support = FeatureSupport.Supported;
        AutoCaptureWindowViewModel model = new(() => support);
        Assert.True(model.TryStart(Region));
        support = FeatureSupport.NotSupported("Fixture capability ended.");
        Assert.False(await model.TryCaptureAsync(Region, _ => throw new Exception("Must not capture."), _ => throw new Exception("Must not publish.")));
        Assert.False(model.IsRunning);
        model.Stop();
    }

    [Fact]
    public void InvalidSavedRepeatValuesAreClampedWithoutMutationOrOverflow()
    {
        decimal saved = decimal.MaxValue;
        Assert.Equal(86400m, AutoCaptureWindowViewModel.ClampRepeatSeconds(saved));
        Assert.Equal(86400000, AutoCaptureWindowViewModel.GetRepeatDelayMilliseconds(saved));
        Assert.Equal(decimal.MaxValue, saved);
        Assert.Equal(1000, AutoCaptureWindowViewModel.GetRepeatDelayMilliseconds(decimal.MinValue));
        Assert.Equal(1000, AutoCaptureWindowViewModel.GetRepeatDelayMilliseconds(0));
        Assert.Equal(1500, AutoCaptureWindowViewModel.GetRepeatDelayMilliseconds(1.5m));
    }
}
