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

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task ClosedChooserWaitsForTheActualSelectorAndRejectsAllLateSources(int sourceKind)
    {
        TaskCompletionSource<PinToScreenSource?> result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int selected = 0, restored = 0, finished = 0, calls = 0;
        Task<PinToScreenSource?> Select() { calls++; return result.Task; }
        using PinToScreenStartupViewModel viewModel = new(CreateServices(Select, Select, Select),
            () => sourceKind == 0 ? FeatureSupport.Supported : FeatureSupport.NotSupported("No desktop capture."),
            () => Task.CompletedTask);
        viewModel.SourceSelected = _ => selected++;
        viewModel.RegionCaptureFinished = () => restored++;
        viewModel.SelectionFinished = () =>
        {
            finished++;
            Assert.False(viewModel.IsBusy);
            Assert.True(viewModel.RequestClose());
        };
        Task selection = sourceKind switch
        {
            0 => viewModel.CaptureRegionCommand.ExecuteAsync(null),
            1 => viewModel.FromClipboardCommand.ExecuteAsync(null),
            _ => viewModel.FromFileCommand.ExecuteAsync(null)
        };
        Assert.True(viewModel.IsBusy);
        Assert.False(viewModel.RequestClose());
        Assert.False(viewModel.RequestClose());
        Assert.True(viewModel.IsClosed);
        Assert.False(viewModel.IsIdle);
        Assert.False(viewModel.CanCaptureRegion);
        Assert.False(viewModel.CaptureRegionCommand.CanExecute(null));
        Assert.False(viewModel.FromClipboardCommand.CanExecute(null));
        Assert.False(viewModel.FromFileCommand.CanExecute(null));
        await viewModel.CaptureRegionCommand.ExecuteAsync(null);
        await viewModel.FromClipboardCommand.ExecuteAsync(null);
        await viewModel.FromFileCommand.ExecuteAsync(null);
        Assert.Equal(1, calls);
        Assert.Equal(0, finished);
        Assert.False(selection.IsCompleted);

        result.SetResult(CreateSource());
        await selection.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(0, selected);
        Assert.Equal(0, restored);
        Assert.Equal(1, finished);
        Assert.False(viewModel.IsBusy);
        Assert.False(viewModel.IsIdle);
        Assert.True(viewModel.RequestClose());
        Assert.False(viewModel.HasError);
    }

    [Fact]
    public async Task CloseDuringHideWaitSkipsTheSelectorAndRestoration()
    {
        TaskCompletionSource hidden = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int calls = 0, restored = 0, finished = 0;
        using PinToScreenStartupViewModel viewModel = new(CreateServices(() =>
        {
            calls++;
            return Task.FromResult<PinToScreenSource?>(CreateSource());
        }), () => FeatureSupport.Supported, () => hidden.Task);
        viewModel.SourceSelected = _ => Assert.Fail("A closed chooser cannot pin.");
        viewModel.RegionCaptureFinished = () => restored++;
        viewModel.SelectionFinished = () => { finished++; Assert.True(viewModel.RequestClose()); };
        Task selection = viewModel.CaptureRegionCommand.ExecuteAsync(null);
        Assert.False(viewModel.RequestClose());
        Assert.True(viewModel.IsBusy);
        Assert.False(selection.IsCompleted);
        hidden.SetResult();
        await selection.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(0, calls);
        Assert.Equal(0, restored);
        Assert.Equal(1, finished);
        Assert.False(viewModel.HasError);
    }

    [Fact]
    public async Task SupportLossDuringSelectionRejectsTheSourceButKeepsFileAndClipboardUsable()
    {
        TaskCompletionSource<PinToScreenSource?> result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        FeatureSupport support = FeatureSupport.Supported;
        int restored = 0, finished = 0;
        List<PinToScreenSource> selected = [];
        PinToScreenSource source = CreateSource();
        using PinToScreenStartupViewModel viewModel = new(CreateServices(() => result.Task,
            () => Task.FromResult<PinToScreenSource?>(source), () => Task.FromResult<PinToScreenSource?>(source)),
            () => support, () => Task.CompletedTask);
        viewModel.SourceSelected = selected.Add;
        viewModel.RegionCaptureFinished = () => restored++;
        viewModel.SelectionFinished = () => { Assert.False(viewModel.IsBusy); finished++; };
        Task capture = viewModel.CaptureRegionCommand.ExecuteAsync(null);
        support = FeatureSupport.NotSupported("Fixture permission withdrawn.");
        result.SetResult(source);
        await capture.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Empty(selected);
        Assert.Equal(Strings.PinToScreenStartupViewModel_CaptureUnavailable, viewModel.ErrorMessage);
        Assert.Equal(1, restored);
        Assert.Equal(1, finished);
        Assert.True(viewModel.IsIdle);
        Assert.False(viewModel.CanCaptureRegion);

        await viewModel.FromClipboardCommand.ExecuteAsync(null);
        await viewModel.FromFileCommand.ExecuteAsync(null);
        Assert.Equal(new[] { source, source }, selected);
        Assert.Equal(3, finished);
        Assert.False(viewModel.HasError);
    }

    [Fact]
    public async Task SuccessfulPinMayCloseReentrantlyAndCompletesBeforeTheWindowIsReleased()
    {
        PinToScreenSource source = CreateSource();
        List<string> events = [];
        using PinToScreenStartupViewModel viewModel = new(CreateServices(() => Task.FromResult<PinToScreenSource?>(source)),
            () => FeatureSupport.Supported, () => Task.CompletedTask);
        viewModel.RegionCaptureStarted = () => events.Add("hidden");
        viewModel.SourceSelected = selected =>
        {
            Assert.Same(source, selected);
            Assert.Equal(new System.Drawing.Point(-50, 75), selected.Location);
            events.Add("pinned");
            Assert.False(viewModel.RequestClose());
        };
        viewModel.RegionCaptureFinished = () => Assert.Fail("Successful close must not restore the chooser.");
        viewModel.SelectionFinished = () =>
        {
            Assert.True(viewModel.RequestClose());
            events.Add("closed");
        };
        await viewModel.CaptureRegionCommand.ExecuteAsync(null);
        Assert.Equal(new[] { "hidden", "pinned", "closed" }, events);
        Assert.True(viewModel.IsClosed);
        Assert.False(viewModel.IsBusy);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ClosedChooserIgnoresLateEmptyResultsAndErrors(bool failure)
    {
        TaskCompletionSource<PinToScreenSource?> result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int restored = 0, finished = 0;
        using PinToScreenStartupViewModel viewModel = new(CreateServices(() => result.Task),
            () => FeatureSupport.Supported, () => Task.CompletedTask);
        viewModel.RegionCaptureFinished = () => restored++;
        viewModel.SelectionFinished = () => finished++;
        Task selection = viewModel.CaptureRegionCommand.ExecuteAsync(null);
        Assert.False(viewModel.RequestClose());
        if (failure) result.SetException(new InvalidOperationException("Late selector failure."));
        else result.SetResult(null);
        await selection.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.False(viewModel.HasError);
        Assert.Equal(0, restored);
        Assert.Equal(1, finished);
        Assert.True(viewModel.RequestClose());
    }

    [Fact]
    public async Task IdleCloseRejectsCommandsWithoutReadingDisposedCapabilities()
    {
        int calls = 0;
        bool disposed = false;
        Task<PinToScreenSource?> Select() { calls++; return Task.FromResult<PinToScreenSource?>(CreateSource()); }
        using PinToScreenStartupViewModel viewModel = new(CreateServices(Select, Select, Select),
            () => disposed ? throw new ObjectDisposedException("Fixture platform") : FeatureSupport.Supported);
        Assert.True(viewModel.RequestClose());
        disposed = true;
        viewModel.Dispose();
        Assert.False(viewModel.CanCaptureRegion);
        Assert.Null(viewModel.CaptureUnavailableReason);
        await viewModel.CaptureRegionCommand.ExecuteAsync(null);
        await viewModel.FromClipboardCommand.ExecuteAsync(null);
        await viewModel.FromFileCommand.ExecuteAsync(null);
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task RestorationFailureStillReleasesTheSelectionOwner()
    {
        int finished = 0;
        InvalidOperationException error = new("Fixture restoration failed.");
        using PinToScreenStartupViewModel viewModel = new(CreateServices(() => Task.FromResult<PinToScreenSource?>(null)),
            () => FeatureSupport.Supported, () => Task.CompletedTask);
        viewModel.RegionCaptureFinished = () => throw error;
        viewModel.SelectionFinished = () => { finished++; Assert.False(viewModel.IsBusy); };
        Assert.Same(error, await Assert.ThrowsAsync<InvalidOperationException>(() => viewModel.CaptureRegionCommand.ExecuteAsync(null)));
        Assert.Equal(1, finished);
        Assert.True(viewModel.IsIdle);
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
