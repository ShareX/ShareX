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
using Strings = ShareX.ScreenCaptureLib.Localization.Strings;

namespace ShareX.ImageEditor.Tests;

public sealed class FFmpegOptionsSupportTests
{
    [Fact]
    public async Task UnsupportedRecordingDoesNotChangeSavedOptionsOrStartDiscovery()
    {
        FeatureSupport support = FeatureSupport.NotSupported("Install a legacy recorder.");
        FFmpegOptionsWindowViewModel viewModel = new(true, () => support);
        FFmpegOptions options = new() { CLIPath = "fixture-ffmpeg", VideoSource = "fixture-screen" };
        int mutations = 0, discoveries = 0, publications = 0;

        Assert.False(viewModel.Support.IsSupported);
        Assert.Equal(Strings.FFmpegOptionsWindow_RecordingUnavailable, viewModel.Support.Reason);
        Assert.NotEqual(support.Reason, viewModel.Support.Reason);
        Assert.False(viewModel.TryChange(() => { options.CLIPath = "changed"; mutations++; }));
        Assert.False(await viewModel.TryReadAsync(() => { discoveries++; return Task.FromResult("changed-device"); },
            result => { options.VideoSource = result; publications++; }));
        Assert.Equal("fixture-ffmpeg", options.CLIPath);
        Assert.Equal("fixture-screen", options.VideoSource);
        Assert.Equal(0, mutations);
        Assert.Equal(0, discoveries);
        Assert.Equal(0, publications);
    }

    [Fact]
    public async Task CapabilityLossWhileReadingDiscardsResultsAndRejectsStaleSetters()
    {
        FeatureSupport support = FeatureSupport.Supported;
        FFmpegOptionsWindowViewModel viewModel = new(true, () => support);
        FFmpegOptions options = new() { CLIPath = "original", VideoSource = "original-device" };
        TaskCompletionSource<string> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int reads = 0, publications = 0;
        Task<bool> operation = viewModel.TryReadAsync(() => { reads++; return completion.Task; }, result =>
        {
            options.CLIPath = result;
            options.VideoSource = "new-device";
            publications++;
        });
        Assert.Equal(1, reads);
        Assert.False(operation.IsCompleted);

        support = FeatureSupport.NotSupported("Fixture desktop ended.");
        completion.SetResult("selected-path");
        Assert.False(await operation);
        Assert.False(viewModel.TryChange(() => options.CLIPath = "stale-setter"));
        Assert.Equal("original", options.CLIPath);
        Assert.Equal("original-device", options.VideoSource);
        Assert.Equal(0, publications);
    }

    [Fact]
    public async Task ConversionModeDoesNotReadDesktopRecordingSupport()
    {
        FFmpegOptionsWindowViewModel viewModel = new(false,
            () => throw new InvalidOperationException("File conversion must not query capture availability."));
        FFmpegOptions options = new() { CLIPath = "original" };
        Assert.True(viewModel.Support.IsSupported);
        Assert.Null(viewModel.Support.Reason);
        Assert.True(viewModel.TryChange(() => options.UseCustomCommands = true));
        Assert.True(await viewModel.TryReadAsync(() => Task.FromResult("conversion-path"), path => options.CLIPath = path));
        Assert.True(options.UseCustomCommands);
        Assert.Equal("conversion-path", options.CLIPath);
    }

    [Fact]
    public async Task SupportedActionsApplyOnceAndPreserveReaderAndSetterFailures()
    {
        FFmpegOptionsWindowViewModel viewModel = new(true, () => FeatureSupport.Supported);
        int mutations = 0, publications = 0;
        Assert.True(viewModel.TryChange(() => mutations++));
        Assert.True(await viewModel.TryReadAsync(() => Task.FromResult("fixture-result"), result =>
        {
            Assert.Equal("fixture-result", result);
            publications++;
        }));
        Assert.Equal(1, mutations);
        Assert.Equal(1, publications);

        InvalidOperationException error = new("Fixture discovery failed.");
        Assert.Same(error, await Assert.ThrowsAsync<InvalidOperationException>(() =>
            viewModel.TryReadAsync(() => Task.FromException<string>(error), _ => publications++)));
        Assert.Equal(1, publications);
        Assert.Same(error, Assert.Throws<InvalidOperationException>(() => viewModel.TryChange(() => throw error)));
    }
}
