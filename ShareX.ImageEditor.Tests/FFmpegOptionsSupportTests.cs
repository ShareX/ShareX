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
using ShareX.Platform.Windows;
using ShareX.ScreenCaptureLib;
using System.Runtime.Versioning;
using Xunit;
using Strings = ShareX.ScreenCaptureLib.Localization.Strings;

namespace ShareX.ImageEditor.Tests;

public sealed class FFmpegOptionsSupportTests
{
    [Fact]
    public async Task UnavailableDeviceActionsKeepRecordingAndPlatformSourcesUsable()
    {
        FeatureSupport unavailable = FeatureSupport.NotSupported("This desktop does not expose DirectShow devices.");
        FFmpegOptionsWindowViewModel viewModel = new(true, () => FeatureSupport.Supported, _ => unavailable);
        FFmpegOptions options = new() { VideoSource = "saved-camera", AudioSource = "saved-microphone" };
        int actions = 0;
        Assert.True(viewModel.Support.IsSupported);
        Assert.Same(unavailable, viewModel.GetDeviceActionSupport(RecordingDeviceAction.ListDirectShowDevices));
        Assert.False(await viewModel.TryReadDevicesAsync(() => { actions++; return Task.FromResult("new-camera"); }, _ => actions++));
        Assert.False(viewModel.TryDeviceAction(RecordingDeviceAction.InstallRecorderDevices, () => actions++));
        List<FFmpegCaptureDevice> video = [FFmpegCaptureDevice.None, new("fixture-screen", "fixture-screen")];
        List<FFmpegCaptureDevice> audio = [FFmpegCaptureDevice.None];
        options.VideoSource = viewModel.ResolveSelectedSource(video, options.VideoSource, "fixture-screen").Value;
        options.AudioSource = viewModel.ResolveSelectedSource(audio, options.AudioSource, "").Value;
        Assert.Equal("saved-camera", options.VideoSource);
        Assert.Equal("saved-microphone", options.AudioSource);
        Assert.Contains(video, x => x.Value == "fixture-screen");
        Assert.Contains(video, x => x.Value == options.VideoSource);
        Assert.Contains(audio, x => x.Value == options.AudioSource);
        Assert.True(viewModel.TryChange(() => options.VideoSource = "fixture-screen"));
        Assert.Equal("fixture-screen", options.VideoSource);
        Assert.Equal(0, actions);
    }

    [Theory]
    [InlineData(RecordingDeviceAction.ListDirectShowDevices)]
    [InlineData(RecordingDeviceAction.InstallRecorderDevices)]
    public async Task DeviceActionsAreIndependentOfEachOther(RecordingDeviceAction unavailable)
    {
        FFmpegOptionsWindowViewModel viewModel = new(true, () => FeatureSupport.Supported,
            action => action == unavailable ? FeatureSupport.NotSupported("Fixture action unavailable.") : FeatureSupport.Supported);
        int discoveries = 0, publications = 0, downloads = 0;
        bool read = await viewModel.TryReadDevicesAsync(() => { discoveries++; return Task.FromResult("fixture"); }, _ => publications++);
        bool download = viewModel.TryDeviceAction(RecordingDeviceAction.InstallRecorderDevices, () => downloads++);
        Assert.Equal(unavailable != RecordingDeviceAction.ListDirectShowDevices, read);
        Assert.Equal(read ? 1 : 0, discoveries);
        Assert.Equal(read ? 1 : 0, publications);
        Assert.Equal(unavailable != RecordingDeviceAction.InstallRecorderDevices, download);
        Assert.Equal(download ? 1 : 0, downloads);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task DeviceReadRejectsOverlapAndLateResultsAfterLossOrClosure(int interruption)
    {
        FeatureSupport devices = FeatureSupport.Supported;
        FeatureSupport recording = FeatureSupport.Supported;
        FFmpegOptionsWindowViewModel viewModel = new(true, () => recording, _ => devices);
        TaskCompletionSource<string> result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int reads = 0, published = 0;
        Task<bool> operation = viewModel.TryReadDevicesAsync(() => { reads++; return result.Task; }, _ => published++);
        Assert.True(viewModel.IsReadingDevices);
        Assert.False(await viewModel.TryReadDevicesAsync(() => { reads++; return Task.FromResult("competing"); }, _ => published++));
        if (interruption == 1) viewModel.Close();
        else if (interruption == 2) recording = FeatureSupport.NotSupported("Fixture recording ended.");
        else devices = FeatureSupport.NotSupported("Fixture capability ended.");
        result.SetResult("late-result");
        Assert.False(await operation);
        Assert.False(viewModel.IsReadingDevices);
        Assert.Equal(1, reads);
        Assert.Equal(0, published);
        Assert.False(viewModel.TryDeviceAction(RecordingDeviceAction.InstallRecorderDevices, () => published++));
        if (interruption == 1) Assert.False(viewModel.TryChange(() => published++));
        else
        {
            devices = FeatureSupport.Supported;
            recording = FeatureSupport.Supported;
            Assert.True(await viewModel.TryReadDevicesAsync(() => Task.FromResult("recovered"), _ => published++));
            Assert.Equal(1, published);
        }
    }

    [Fact]
    public async Task SupportedDiscoveryRetainsWindowsFallbackAndClearsBusyAfterErrors()
    {
        FFmpegOptionsWindowViewModel viewModel = new(true, () => FeatureSupport.Supported, _ => FeatureSupport.Supported);
        List<FFmpegCaptureDevice> sources = [FFmpegCaptureDevice.None, FFmpegCaptureDevice.GDIGrab];
        Assert.Equal(FFmpegCaptureDevice.GDIGrab, viewModel.ResolveSelectedSource(sources, "missing-device", "gdigrab"));
        Assert.Equal(2, sources.Count);
        InvalidOperationException error = new("Synthetic discovery failure.");
        Assert.Same(error, await Assert.ThrowsAsync<InvalidOperationException>(() =>
            viewModel.TryReadDevicesAsync(() => Task.FromException<string>(error), _ => throw new Exception("Must not publish."))));
        Assert.False(viewModel.IsReadingDevices);
        int published = 0;
        Assert.True(await viewModel.TryReadDevicesAsync(() => Task.FromResult("fixture"), _ => published++));
        Assert.Equal(1, published);
    }

    [WindowsRecordingDeviceFact]
    [SupportedOSPlatform("windows")]
    public void WindowsKeepsItsNamedDeviceActionsWithoutQueryingDisplays()
    {
        WindowsScreenRecordingService service = new(() => throw new InvalidOperationException("Device availability must not enumerate displays."));
        Assert.True(service.GetDeviceActionSupport(RecordingDeviceAction.ListDirectShowDevices).IsSupported);
        Assert.True(service.GetDeviceActionSupport(RecordingDeviceAction.InstallRecorderDevices).IsSupported);
        Assert.False(service.GetDeviceActionSupport((RecordingDeviceAction)123).IsSupported);
    }

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

public sealed class WindowsRecordingDeviceFactAttribute : FactAttribute
{
    public WindowsRecordingDeviceFactAttribute()
    {
        if (!OperatingSystem.IsWindows()) Skip = "The Windows recording device service is tested on Windows.";
    }
}
