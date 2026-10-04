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
using ShareX.Tools.Localization;
using Xunit;

namespace ShareX.Tools.Tests;

public sealed class FileMediaSupportTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FileCapabilityUsesSelectedPathWithoutQueryingDesktopRecording(bool supported)
    {
        using Fixture fixture = new();
        RecordingProbe recording = new(supported);
        FeatureSupport result = FileMediaFeatureSupport.Get(recording, fixture.EnginePath);
        Assert.Equal(fixture.EnginePath, recording.LastPath);
        Assert.Equal(supported, result.IsSupported);
        Assert.Equal(supported ? null : Strings.VideoConverterWindow_FFmpeg_unavailable, result.Reason);
    }

    [Fact]
    public async Task UnavailableConverterPreservesSettingsAndDoesNotCallPickersOrHandler()
    {
        using Fixture fixture = new();
        int pickers = 0, conversions = 0;
        using VideoConverterViewModel model = new(fixture.ConverterOptions, (_, _, _) =>
        {
            conversions++;
            return Task.FromResult(new VideoConversionResult(true, false));
        }, getSupport: () => Unavailable);
        model.SelectInputFileRequested = _ => { pickers++; return Task.FromResult<string?>(fixture.InputPath); };
        model.SelectOutputFolderRequested = _ => { pickers++; return Task.FromResult<string?>(fixture.DirectoryPath); };
        model.InputFilePath = "changed.mp4";
        model.OutputFileName = "changed";
        model.AutoOpenFolder = true;
        model.LoadInput("changed.mp4");
        await model.BrowseInputCommand.ExecuteAsync(null);
        await model.BrowseOutputFolderCommand.ExecuteAsync(null);
        await model.StartEncodingCommand.ExecuteAsync(null);
        Assert.False(model.CanEdit);
        Assert.False(model.CanSelect);
        Assert.Equal(fixture.InputPath, fixture.ConverterOptions.InputFilePath);
        Assert.Equal("output", fixture.ConverterOptions.OutputFileName);
        Assert.False(fixture.ConverterOptions.AutoOpenFolder);
        Assert.Equal(Strings.VideoConverterWindow_FFmpeg_unavailable, model.StatusText);
        Assert.Equal(0, pickers);
        Assert.Equal(0, conversions);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ConverterIgnoresPickerResultAfterSupportLoss(bool outputPicker)
    {
        using Fixture fixture = new();
        bool supported = true;
        TaskCompletionSource<string?> picker = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using VideoConverterViewModel model = new(fixture.ConverterOptions,
            (_, _, _) => throw new InvalidOperationException("No conversion was requested."),
            getSupport: () => supported ? FeatureSupport.Supported : Unavailable);
        model.SelectInputFileRequested = _ => picker.Task;
        model.SelectOutputFolderRequested = _ => picker.Task;
        Task selection = outputPicker ? model.BrowseOutputFolderCommand.ExecuteAsync(null) : model.BrowseInputCommand.ExecuteAsync(null);
        supported = false;
        picker.SetResult(Path.Combine(fixture.DirectoryPath, "late"));
        await selection.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(fixture.InputPath, model.InputFilePath);
        Assert.Equal(fixture.DirectoryPath, model.OutputFolderPath);
        Assert.Equal(fixture.InputPath, fixture.ConverterOptions.InputFilePath);
        Assert.False(model.IsSelecting);
        Assert.Equal(Strings.VideoConverterWindow_FFmpeg_unavailable, model.StatusText);
    }

    [Fact]
    public async Task ConverterRechecksSupportAfterReentrantEncodingStateChange()
    {
        using Fixture fixture = new();
        bool supported = true;
        int conversions = 0;
        using VideoConverterViewModel model = new(fixture.ConverterOptions, (_, _, _) =>
        {
            conversions++;
            return Task.FromResult(new VideoConversionResult(true, false));
        }, getSupport: () => supported ? FeatureSupport.Supported : Unavailable);
        model.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(model.IsEncoding) && model.IsEncoding) supported = false; };
        await model.StartEncodingCommand.ExecuteAsync(null);
        Assert.Equal(0, conversions);
        Assert.False(model.IsEncoding);
        Assert.Equal(Strings.VideoConverterWindow_FFmpeg_unavailable, model.StatusText);
    }

    [Fact]
    public async Task RunningConverterKeepsStopAfterSupportLoss()
    {
        using Fixture fixture = new();
        bool supported = true;
        CancellationToken token = default;
        TaskCompletionSource<VideoConversionResult> result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using VideoConverterViewModel model = new(fixture.ConverterOptions, (_, _, cancellation) =>
        {
            token = cancellation;
            return result.Task;
        }, getSupport: () => supported ? FeatureSupport.Supported : Unavailable);
        Task conversion = model.StartEncodingCommand.ExecuteAsync(null);
        supported = false;
        Assert.True(model.CanStop);
        model.StopEncodingCommand.Execute(null);
        Assert.True(token.IsCancellationRequested);
        result.SetResult(new VideoConversionResult(true, false));
        await conversion.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(Strings.VideoConverterViewModel_Conversion_stopped, model.StatusText);
    }

    [Fact]
    public async Task UnavailableTrimmerDoesNotSelectOrLoadInput()
    {
        using Fixture fixture = new();
        int pickers = 0;
        using VideoTrimmerViewModel model = new(fixture.MissingEnginePath, getSupport: () => Unavailable);
        model.SelectInputRequested = () => { pickers++; return Task.FromResult<string?>(fixture.InputPath); };
        await model.BrowseCommand.ExecuteAsync(null);
        await model.LoadInputAsync(fixture.InputPath);
        Assert.Equal(0, pickers);
        Assert.Empty(model.InputFilePath);
        Assert.False(model.IsLoading);
        Assert.False(model.CanBrowse);
        Assert.Equal(Strings.VideoConverterWindow_FFmpeg_unavailable, model.StatusText);
    }

    [Fact]
    public async Task TrimmerRejectsExportAfterPickerSupportLossAndKeepsCancelAvailable()
    {
        using Fixture fixture = new();
        bool supported = false; // Prevent preview work while constructing synthetic metadata.
        TaskCompletionSource<string?> picker = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using VideoTrimmerViewModel model = new(fixture.MissingEnginePath,
            getSupport: () => supported ? FeatureSupport.Supported : Unavailable);
        model.InputFilePath = fixture.InputPath;
        model.Duration = 10;
        model.End = 9;
        model.SelectOutputRequested = _ => picker.Task;
        supported = true;
        Task export = model.ExportCommand.ExecuteAsync(null);
        Assert.True(model.IsExporting);
        supported = false;
        Assert.True(model.CanUsePrimaryAction);
        Assert.Same(model.CancelCommand, model.PrimaryActionCommand);
        model.SetEndTime("00:00:05.000");
        Assert.Equal(9, model.End);
        picker.SetResult(fixture.OutputPath);
        await export.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.False(model.IsExporting);
        Assert.Empty(model.OutputFilePath);
        Assert.False(File.Exists(fixture.OutputPath));
        Assert.Equal(Strings.VideoConverterWindow_FFmpeg_unavailable, model.StatusText);
    }

    [Fact]
    public async Task SupportedTrimmerCanSelectOutputWithoutDesktopRecording()
    {
        using Fixture fixture = new();
        bool supported = false;
        int pickers = 0;
        using VideoTrimmerViewModel model = new(fixture.MissingEnginePath,
            getSupport: () => supported ? FeatureSupport.Supported : Unavailable);
        model.Duration = 10;
        model.End = 9;
        model.SelectOutputRequested = _ => { pickers++; return Task.FromResult<string?>(null); };
        supported = true;
        Assert.True(model.CanTrim);
        await model.ExportCommand.ExecuteAsync(null);
        Assert.Equal(1, pickers);
        Assert.True(model.CanTrim);
        Assert.False(model.IsExporting);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task ThumbnailPickerCannotChangeSavedOptionsAfterLossOrClose(bool outputPicker, bool close)
    {
        using Fixture fixture = new();
        bool supported = true;
        int workers = 0;
        TaskCompletionSource<string?> picker = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using VideoThumbnailerViewModel model = new(fixture.EnginePath, fixture.ThumbnailOptions, null,
            () => supported ? FeatureSupport.Supported : Unavailable,
            () => { workers++; return Task.FromResult<IReadOnlyList<VideoThumbnailInfo>>([]); });
        model.SelectVideoRequested = () => picker.Task;
        model.SelectOutputFolderRequested = _ => picker.Task;
        Task selection = outputPicker ? model.SelectOutputFolderCommand.ExecuteAsync(null) : model.SelectVideoCommand.ExecuteAsync(null);
        if (close) model.Dispose();
        else supported = false;
        picker.SetResult(Path.Combine(fixture.DirectoryPath, "late"));
        await selection.WaitAsync(TimeSpan.FromSeconds(5));
        model.ThumbnailCount = 99;
        model.UploadThumbnails = true;
        model.LoadVideo("changed.mp4");
        await model.StartCommand.ExecuteAsync(null);
        Assert.Equal(fixture.InputPath, fixture.ThumbnailOptions.LastVideoPath);
        Assert.Equal(fixture.DirectoryPath, fixture.ThumbnailOptions.CustomOutputDirectory);
        Assert.Equal(3, fixture.ThumbnailOptions.ThumbnailCount);
        Assert.False(fixture.ThumbnailOptions.UploadThumbnails);
        Assert.False(model.CanStart);
        Assert.False(model.IsIdle);
        Assert.Equal(0, workers);
    }

    [Fact]
    public async Task ThumbnailWorkerIsNotDispatchedAfterReentrantSupportLoss()
    {
        using Fixture fixture = new();
        bool supported = true;
        int workers = 0;
        using VideoThumbnailerViewModel model = new(fixture.EnginePath, fixture.ThumbnailOptions, null,
            () => supported ? FeatureSupport.Supported : Unavailable,
            () => { workers++; return Task.FromResult<IReadOnlyList<VideoThumbnailInfo>>([]); });
        model.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(model.IsBusy) && model.IsBusy) supported = false; };
        await model.StartCommand.ExecuteAsync(null);
        Assert.Equal(0, workers);
        Assert.False(model.IsBusy);
        Assert.Equal(Strings.VideoConverterWindow_FFmpeg_unavailable, model.ErrorMessage);
    }

    [Fact]
    public async Task AcceptedThumbnailJobRetainsOutputCallbackAfterSupportLossAndClose()
    {
        using Fixture fixture = new();
        bool supported = true;
        int callbacks = 0;
        TaskCompletionSource<IReadOnlyList<VideoThumbnailInfo>> result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using VideoThumbnailerViewModel model = new(fixture.EnginePath, fixture.ThumbnailOptions,
            thumbnails => { Assert.Single(thumbnails); callbacks++; },
            () => supported ? FeatureSupport.Supported : Unavailable, () => result.Task);
        Task job = model.StartCommand.ExecuteAsync(null);
        Assert.True(model.IsBusy);
        supported = false;
        model.Dispose();
        model.Dispose();
        model.UploadThumbnails = true;
        result.SetResult([new(fixture.OutputPath)]);
        await job.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(1, callbacks);
        Assert.False(model.IsBusy);
        Assert.False(model.IsIdle);
        Assert.False(fixture.ThumbnailOptions.UploadThumbnails);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task DisposedFileToolIgnoresActionsWithoutQueryingReleasedProvider(int tool)
    {
        using Fixture fixture = new();
        bool providerDisposed = false;
        FeatureSupport GetSupport() => providerDisposed
            ? throw new ObjectDisposedException("fixture capability provider")
            : FeatureSupport.Supported;
        if (tool == 0)
        {
            using VideoConverterViewModel model = new(fixture.ConverterOptions,
                (_, _, _) => throw new InvalidOperationException("No job after close."), getSupport: GetSupport);
            model.Dispose();
            providerDisposed = true;
            Assert.Null(model.SupportReason);
            model.LoadInput(fixture.InputPath);
            await model.BrowseInputCommand.ExecuteAsync(null);
            await model.StartEncodingCommand.ExecuteAsync(null);
        }
        else if (tool == 1)
        {
            using VideoTrimmerViewModel model = new(fixture.MissingEnginePath, getSupport: GetSupport);
            model.Dispose();
            providerDisposed = true;
            Assert.Null(model.SupportReason);
            await model.LoadInputAsync(fixture.InputPath);
            await model.BrowseCommand.ExecuteAsync(null);
            await model.ExportCommand.ExecuteAsync(null);
        }
        else
        {
            using VideoThumbnailerViewModel model = new(fixture.EnginePath, fixture.ThumbnailOptions, getSupport: GetSupport);
            model.Dispose();
            providerDisposed = true;
            Assert.Null(model.SupportReason);
            model.LoadVideo(fixture.InputPath);
            await model.SelectVideoCommand.ExecuteAsync(null);
            await model.StartCommand.ExecuteAsync(null);
        }
    }

    private static FeatureSupport Unavailable => FeatureSupport.NotSupported("Install a legacy-helper to enable this feature.");

    private sealed class RecordingProbe(bool supported) : IScreenRecordingService
    {
        public string? LastPath { get; private set; }
        public FeatureSupport Support => throw new InvalidOperationException("File tools must not query desktop recording support.");
        public FeatureSupport GetFileMediaSupport(string ffmpegPath)
        {
            LastPath = ffmpegPath;
            return supported ? FeatureSupport.Supported : Unavailable;
        }
        public FeatureSupport GetDeviceActionSupport(RecordingDeviceAction action) => throw new NotSupportedException();
        public IReadOnlyList<string> GetSupportedDevices() => throw new NotSupportedException();
        public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request) => throw new NotSupportedException();
        public void PrepareDevice(string device, ScreenRecordingRequest request) => throw new NotSupportedException();
        public string GetDefaultFFmpegPath(string applicationDirectory) => throw new NotSupportedException();
    }

    private sealed class Fixture : IDisposable
    {
        public string DirectoryPath { get; } = Path.Combine(Path.GetTempPath(), "ShareX-file-media-" + Guid.NewGuid().ToString("N"));
        public string InputPath => Path.Combine(DirectoryPath, "input.mp4");
        public string EnginePath => Path.Combine(DirectoryPath, "existence-only-engine.fixture");
        public string MissingEnginePath => Path.Combine(DirectoryPath, "never-installed-engine");
        public string OutputPath => Path.Combine(DirectoryPath, "output.mp4");
        public VideoConverterOptions ConverterOptions { get; }
        public VideoThumbnailOptions ThumbnailOptions { get; }
        public Fixture()
        {
            Directory.CreateDirectory(DirectoryPath);
            File.WriteAllBytes(InputPath, [1]);
            File.WriteAllBytes(EnginePath, [1]); // Existence only. Injected handlers ensure this is never executed.
            ConverterOptions = new() { InputFilePath = InputPath, OutputFolderPath = DirectoryPath, OutputFileName = "output", AutoOpenFolder = false };
            ThumbnailOptions = new() { LastVideoPath = InputPath, CustomOutputDirectory = DirectoryPath, ThumbnailCount = 3, UploadThumbnails = false };
        }
        public void Dispose()
        {
            File.Delete(InputPath);
            File.Delete(EnginePath);
            File.Delete(OutputPath);
            Directory.Delete(DirectoryPath);
        }
    }
}
