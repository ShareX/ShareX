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
using SkiaSharp;
using Xunit;
using Strings = ShareX.Tools.Localization.Strings;

namespace ShareX.Tools.Tests;

public sealed class PinToScreenSourceSupportTests
{
    [Fact]
    public async Task UnsupportedCaptureDoesNotHideOrReadAndOtherSourcesRemainAvailable()
    {
        PinToScreenSource source = CreateSource();
        int captureCalls = 0, clipboardCalls = 0, fileCalls = 0, started = 0, finished = 0;
        List<PinToScreenSource> selected = [];
        PinToScreenStartupViewModel viewModel = new(CreateServices(
            () => { captureCalls++; return Task.FromResult<PinToScreenSource?>(source); },
            () => { clipboardCalls++; return Task.FromResult<PinToScreenSource?>(source); },
            () => { fileCalls++; return Task.FromResult<PinToScreenSource?>(source); }),
            () => FeatureSupport.NotSupported("Install a legacy capture helper."),
            () => throw new InvalidOperationException("Unavailable capture must not wait for hiding."));
        viewModel.SourceSelected = selected.Add;
        viewModel.RegionCaptureStarted = () => started++;
        viewModel.RegionCaptureFinished = () => finished++;

        Assert.False(viewModel.CanCaptureRegion);
        Assert.False(viewModel.CaptureRegionCommand.CanExecute(null));
        Assert.Equal(Strings.PinToScreenStartupViewModel_CaptureUnavailable, viewModel.CaptureUnavailableReason);
        // Direct command execution must still guard a stale or forced callback.
        await viewModel.CaptureRegionCommand.ExecuteAsync(null);
        Assert.Equal(Strings.PinToScreenStartupViewModel_CaptureUnavailable, viewModel.ErrorMessage);
        Assert.True(viewModel.IsIdle);
        Assert.Equal(0, captureCalls);
        Assert.Equal(0, started);
        Assert.Equal(0, finished);

        await viewModel.FromClipboardCommand.ExecuteAsync(null);
        await viewModel.FromFileCommand.ExecuteAsync(null);
        Assert.Equal(1, clipboardCalls);
        Assert.Equal(1, fileCalls);
        Assert.Equal(new[] { source, source }, selected);
        Assert.False(viewModel.HasError);
        Assert.False(viewModel.CanCaptureRegion);
    }

    [Fact]
    public async Task CaptureCapabilityLossWhileHiddenRestoresTheChooserWithoutReading()
    {
        FeatureSupport support = FeatureSupport.Supported;
        TaskCompletionSource hidden = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int captureCalls = 0;
        List<string> events = [];
        PinToScreenStartupViewModel viewModel = new(CreateServices(() =>
        {
            captureCalls++;
            return Task.FromResult<PinToScreenSource?>(CreateSource());
        }), () => support, () => hidden.Task);
        viewModel.RegionCaptureStarted = () => { Assert.True(viewModel.IsBusy); events.Add("hidden"); };
        viewModel.RegionCaptureFinished = () => { Assert.True(viewModel.HasError); events.Add("restored"); };
        viewModel.SourceSelected = _ => Assert.Fail("Capture must not select an image after capability loss.");

        Assert.True(viewModel.CaptureRegionCommand.CanExecute(null));
        Task capture = viewModel.CaptureRegionCommand.ExecuteAsync(null);
        Assert.True(viewModel.IsBusy);
        support = FeatureSupport.NotSupported("Fixture capture service was removed.");
        hidden.SetResult();
        await capture.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(0, captureCalls);
        Assert.Equal(new[] { "hidden", "restored" }, events);
        Assert.True(viewModel.IsIdle);
        Assert.False(viewModel.CanCaptureRegion);
        Assert.False(viewModel.CaptureRegionCommand.CanExecute(null));
        Assert.Equal(Strings.PinToScreenStartupViewModel_CaptureUnavailable, viewModel.ErrorMessage);
        Assert.Equal(Strings.PinToScreenStartupViewModel_CaptureUnavailable, viewModel.CaptureUnavailableReason);
    }

    [Fact]
    public async Task CaptureKeepsTheSelectionBusyAcrossHideAndBackendCompletion()
    {
        TaskCompletionSource hidden = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource captureStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<PinToScreenSource?> captureResult = new(TaskCreationOptions.RunContinuationsAsynchronously);
        PinToScreenSource source = CreateSource();
        int clipboardCalls = 0, fileCalls = 0;
        List<string> events = [];
        PinToScreenStartupViewModel viewModel = new(CreateServices(
            () => { events.Add("capture"); captureStarted.SetResult(); return captureResult.Task; },
            () => { clipboardCalls++; return Task.FromResult<PinToScreenSource?>(source); },
            () => { fileCalls++; return Task.FromResult<PinToScreenSource?>(source); }),
            () => FeatureSupport.Supported, () => hidden.Task);
        viewModel.RegionCaptureStarted = () => { Assert.True(viewModel.IsBusy); events.Add("hidden"); };
        viewModel.SourceSelected = selected => { Assert.Same(source, selected); events.Add("selected"); };
        viewModel.RegionCaptureFinished = () => events.Add("finished");

        Task capture = viewModel.CaptureRegionCommand.ExecuteAsync(null);
        Assert.True(viewModel.IsBusy);
        Assert.False(viewModel.CanCaptureRegion);
        Assert.Null(viewModel.CaptureUnavailableReason);
        await viewModel.FromClipboardCommand.ExecuteAsync(null);
        await viewModel.FromFileCommand.ExecuteAsync(null);
        hidden.SetResult();
        await captureStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.True(viewModel.IsBusy);
        await viewModel.FromClipboardCommand.ExecuteAsync(null);
        await viewModel.FromFileCommand.ExecuteAsync(null);
        Assert.Equal(0, clipboardCalls);
        Assert.Equal(0, fileCalls);
        captureResult.SetResult(source);
        await capture.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(new[] { "hidden", "capture", "selected", "finished" }, events);
        Assert.True(viewModel.IsIdle);
        Assert.True(viewModel.CanCaptureRegion);
        Assert.True(viewModel.CaptureRegionCommand.CanExecute(null));
        Assert.False(viewModel.HasError);
        Assert.Equal(new System.Drawing.Point(-50, 75), source.Location);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CancelledOrFailedCaptureReleasesBusyAndFinishesHiding(bool fail)
    {
        int started = 0, finished = 0;
        PinToScreenStartupViewModel viewModel = new(CreateServices(() => fail
            ? Task.FromException<PinToScreenSource?>(new InvalidOperationException("Synthetic capture failure."))
            : Task.FromResult<PinToScreenSource?>(null)),
            () => FeatureSupport.Supported, () => Task.CompletedTask);
        viewModel.RegionCaptureStarted = () => started++;
        viewModel.RegionCaptureFinished = () => finished++;
        viewModel.SourceSelected = _ => Assert.Fail("Cancelled or failed capture must not select a source.");

        await viewModel.CaptureRegionCommand.ExecuteAsync(null);

        Assert.Equal(1, started);
        Assert.Equal(1, finished);
        Assert.True(viewModel.IsIdle);
        Assert.True(viewModel.CanCaptureRegion);
        Assert.True(viewModel.HasError);
        Assert.Equal(fail ? string.Format(Strings.PinToScreenStartupViewModel_Unable_load_image, "Synthetic capture failure.")
            : Strings.PinToScreenStartupViewModel_No_region_selected, viewModel.ErrorMessage);
    }

    private static PinToScreenServices CreateServices(Func<Task<PinToScreenSource?>> capture,
        Func<Task<PinToScreenSource?>>? clipboard = null, Func<Task<PinToScreenSource?>>? file = null) => new()
    {
        CaptureRegionAsync = capture,
        GetClipboardImageAsync = clipboard ?? (() => Task.FromResult<PinToScreenSource?>(null)),
        SelectImageFileAsync = file ?? (() => Task.FromResult<PinToScreenSource?>(null)),
        CopyImage = _ => Assert.Fail("Synthetic source tests must not access the clipboard.")
    };

    private static PinToScreenSource CreateSource()
    {
        using SKBitmap bitmap = new(1, 1);
        bitmap.Erase(SKColors.Blue);
        using SKData png = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        return new PinToScreenSource(png.ToArray(), new System.Drawing.Point(-50, 75));
    }
}
