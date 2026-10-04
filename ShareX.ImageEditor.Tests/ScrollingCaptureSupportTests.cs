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
using Xunit;
using StartResult = ShareX.ScreenCaptureLib.ScrollingCaptureWindowViewModel.StartResult;
using Strings = ShareX.ScreenCaptureLib.Localization.Strings;

namespace ShareX.ImageEditor.Tests;

public sealed class ScrollingCaptureSupportTests
{
    [Fact]
    public async Task UnavailableAndInvalidMethodsDoNotHideSelectCaptureOrChangeSavedOptions()
    {
        int supportReads = 0, actions = 0;
        ScrollingCaptureWindowViewModel viewModel = new(_ =>
        {
            supportReads++;
            return FeatureSupport.NotSupported("Install a legacy scroll helper.");
        });
        ScrollingCaptureOptions options = new() { StartDelay = 777, ScrollMethod = ScrollMethod.ScrollMessage, AutoUpload = true };

        Assert.Equal(Strings.ScrollingCaptureWindow_Unavailable, viewModel.GetSupport(options.ScrollMethod).Reason);
        Assert.False(viewModel.TryChange(options.ScrollMethod, false, () => { options.StartDelay = 1; actions++; }));
        Assert.Equal(StartResult.Unavailable, await viewModel.TryCaptureAsync(options,
            () => actions++, () => { actions++; return Task.CompletedTask; },
            () => { actions++; return Task.FromResult(true); }, () => { actions++; return Task.CompletedTask; }));
        Assert.False(viewModel.IsBusy);
        Assert.Equal(777, options.StartDelay);
        Assert.Equal(ScrollMethod.ScrollMessage, options.ScrollMethod);
        Assert.True(options.AutoUpload);
        Assert.Equal(0, actions);

        int previousReads = supportReads;
        Assert.False(viewModel.TryChange((ScrollMethod)(-1), false, () => actions++));
        Assert.False(viewModel.GetSupport((ScrollMethod)987).IsSupported);
        Assert.Equal(previousReads, supportReads);
    }

    [Fact]
    public async Task BusyStateCoversHideDelayAndCapabilityLossPreservesPreviousImage()
    {
        FeatureSupport support = FeatureSupport.Supported;
        ScrollingCaptureWindowViewModel viewModel = new(_ => support);
        ScrollingCaptureOptions options = new();
        TaskCompletionSource delay = new(TaskCreationOptions.RunContinuationsAsynchronously);
        string previousImage = "generated-preview";
        int hides = 0, selections = 0, captures = 0;

        Task<StartResult> operation = viewModel.TryCaptureAsync(options, () => hides++, () => delay.Task,
            () => { selections++; return Task.FromResult(true); }, () =>
            {
                captures++;
                previousImage = "new-preview";
                return Task.CompletedTask;
            });
        Assert.True(viewModel.IsBusy);
        Assert.False(operation.IsCompleted);
        Assert.False(viewModel.TryChange(ScrollMethod.MouseWheel, false, () => options.ScrollAmount = 6));
        Assert.Equal(StartResult.Busy, await viewModel.TryCaptureAsync(options,
            () => throw new InvalidOperationException("A competing capture must not hide."),
            () => Task.CompletedTask, () => Task.FromResult(true), () => Task.CompletedTask));

        support = FeatureSupport.NotSupported("Fixture desktop ended.");
        delay.SetResult();
        Assert.Equal(StartResult.Unavailable, await operation);
        Assert.False(viewModel.IsBusy);
        Assert.Equal(1, hides);
        Assert.Equal(0, selections);
        Assert.Equal(0, captures);
        Assert.Equal("generated-preview", previousImage);
        Assert.Equal(2, options.ScrollAmount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SelectionCompletionCannotCaptureAfterCapabilityLossOrWindowClosure(bool close)
    {
        FeatureSupport support = FeatureSupport.Supported;
        ScrollingCaptureWindowViewModel viewModel = new(_ => support);
        TaskCompletionSource<bool> selection = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int captures = 0;
        Task<StartResult> operation = viewModel.TryCaptureAsync(new(), () => { }, () => Task.CompletedTask,
            () => selection.Task, () => { captures++; return Task.CompletedTask; });
        Assert.True(viewModel.IsBusy);
        Assert.False(operation.IsCompleted);

        if (close) viewModel.Close();
        else support = FeatureSupport.NotSupported("Fixture support lost.");
        // Busy remains set until the outstanding selector releases its service.
        Assert.True(viewModel.IsBusy);
        selection.SetResult(true);
        Assert.Equal(close ? StartResult.Closed : StartResult.Unavailable, await operation);
        Assert.False(viewModel.IsBusy);
        Assert.Equal(0, captures);
        Assert.False(viewModel.TryChange(ScrollMethod.MouseWheel, false, () => captures++));
    }

    [Fact]
    public async Task ClosureDuringHideDelayDoesNotOpenSelectorOrAcceptAnotherAction()
    {
        ScrollingCaptureWindowViewModel viewModel = new(_ => FeatureSupport.Supported);
        TaskCompletionSource delay = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int actions = 0;
        Task<StartResult> operation = viewModel.TryCaptureAsync(new(), () => { }, () => delay.Task,
            () => { actions++; return Task.FromResult(true); }, () => { actions++; return Task.CompletedTask; });
        viewModel.Close();
        delay.SetResult();
        Assert.Equal(StartResult.Closed, await operation);
        Assert.False(viewModel.IsBusy);
        Assert.False(viewModel.TryChange(ScrollMethod.MouseWheel, false, () => actions++));
        Assert.Equal(StartResult.Closed, await viewModel.TryCaptureAsync(new(), () => actions++,
            () => Task.CompletedTask, () => Task.FromResult(true), () => Task.CompletedTask));
        Assert.Equal(0, actions);
    }

    [Fact]
    public async Task AutoTopRequiresItsOwnInputsAndCanBeExplicitlyTurnedOff()
    {
        FeatureSupport autoTopSupport = FeatureSupport.NotSupported("Fixture keyboard/window scrolling unavailable.");
        ScrollingCaptureWindowViewModel viewModel = new(_ => FeatureSupport.Supported, () => autoTopSupport);
        ScrollingCaptureOptions options = new() { AutoScrollTop = true, ScrollDelay = 800 };
        int actions = 0;
        Assert.True(viewModel.GetSupport(ScrollMethod.MouseWheel).IsSupported);
        Assert.False(viewModel.GetSupport(ScrollMethod.MouseWheel, true).IsSupported);
        Assert.False(viewModel.TryChange(ScrollMethod.MouseWheel, true, () => options.ScrollDelay = 1));
        Assert.Equal(StartResult.Unavailable, await viewModel.TryCaptureAsync(options, () => actions++,
            () => Task.CompletedTask, () => Task.FromResult(true), () => Task.CompletedTask));
        Assert.True(options.AutoScrollTop);
        Assert.Equal(800, options.ScrollDelay);
        Assert.Equal(0, actions);

        Assert.True(viewModel.TryChange(ScrollMethod.MouseWheel, false, () => options.AutoScrollTop = false));
        autoTopSupport = FeatureSupport.Supported;
        Assert.True(viewModel.TryChange(ScrollMethod.PageDown, true, () =>
        {
            options.ScrollMethod = ScrollMethod.PageDown;
            options.AutoScrollTop = true;
        }));
        Assert.Equal(ScrollMethod.PageDown, options.ScrollMethod);
        Assert.True(options.AutoScrollTop);
    }

    [Fact]
    public async Task SupportedOrderCancellationAndSelectionFailureReleaseBusyState()
    {
        ScrollingCaptureWindowViewModel viewModel = new(_ => FeatureSupport.Supported);
        List<string> events = [];
        Assert.Equal(StartResult.Completed, await viewModel.TryCaptureAsync(new(),
            () => { Assert.True(viewModel.IsBusy); events.Add("hide"); },
            () => { events.Add("delay"); return Task.CompletedTask; },
            () => { events.Add("select"); return Task.FromResult(true); },
            () => { Assert.True(viewModel.IsBusy); events.Add("capture"); return Task.CompletedTask; }));
        Assert.Equal(new[] { "hide", "delay", "select", "capture" }, events);
        Assert.False(viewModel.IsBusy);

        Assert.Equal(StartResult.Cancelled, await viewModel.TryCaptureAsync(new(), () => { }, () => Task.CompletedTask,
            () => Task.FromResult(false), () => throw new InvalidOperationException("Cancellation must retain the preview.")));
        Assert.False(viewModel.IsBusy);
        InvalidOperationException error = new("Fixture selector failed.");
        Assert.Same(error, await Assert.ThrowsAsync<InvalidOperationException>(() =>
            viewModel.TryCaptureAsync(new(), () => { }, () => Task.CompletedTask,
                () => Task.FromException<bool>(error), () => Task.CompletedTask)));
        Assert.False(viewModel.IsBusy);
        Assert.True(viewModel.TryChange(ScrollMethod.DownArrow, false, () => events.Add("saved")));
        Assert.Equal("saved", events[^1]);
    }
}
