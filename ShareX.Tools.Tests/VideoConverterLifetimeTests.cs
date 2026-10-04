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

using System.Collections.Concurrent;
using ShareX.Tools.Localization;
using Xunit;

namespace ShareX.Tools.Tests;

public sealed class VideoConverterLifetimeTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PendingPickerBlocksOtherActionsAndIgnoresItsResultAfterClose(bool outputPicker)
    {
        using Fixture fixture = new();
        TaskCompletionSource<string?> picker = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int pickerCalls = 0, conversions = 0;
        using VideoConverterViewModel model = new(fixture.Options, (_, _, _) =>
        {
            conversions++;
            return Task.FromResult(new VideoConversionResult(true, false));
        });
        model.SelectInputFileRequested = _ => { pickerCalls++; return picker.Task; };
        model.SelectOutputFolderRequested = _ => { pickerCalls++; return picker.Task; };

        Task selection = outputPicker
            ? model.BrowseOutputFolderCommand.ExecuteAsync(null)
            : model.BrowseInputCommand.ExecuteAsync(null);
        Assert.True(model.IsSelecting);
        Assert.False(model.CanEdit);
        Assert.False(model.StartEncodingCommand.CanExecute(null));
        Assert.False(model.BrowseInputCommand.CanExecute(null));
        Assert.False(model.BrowseOutputFolderCommand.CanExecute(null));
        TryChangeSettings(model);
        model.LoadInput(Path.Combine(fixture.DirectoryPath, "late.mp4"));
        await model.StartEncodingCommand.ExecuteAsync(null);
        if (outputPicker) await model.BrowseInputCommand.ExecuteAsync(null);
        else await model.BrowseOutputFolderCommand.ExecuteAsync(null);
        fixture.AssertOriginalSettings(model);
        Assert.Equal(1, pickerCalls);
        Assert.Equal(0, conversions);

        model.Dispose();
        model.Dispose();
        picker.SetResult(outputPicker ? Path.GetTempPath() : Path.Combine(fixture.DirectoryPath, "late.mp4"));
        await selection.WaitAsync(TimeSpan.FromSeconds(5));
        TryChangeSettings(model);
        await model.StartEncodingCommand.ExecuteAsync(null);
        fixture.AssertOriginalSettings(model);
        Assert.False(model.IsSelecting);
        Assert.False(model.IsIdle);
        Assert.False(model.CanEdit);
        Assert.Equal(0, conversions);
    }

    [Fact]
    public async Task LivePickersAndSettingsRetainCodecAndConversionBehavior()
    {
        using Fixture fixture = new();
        VideoConverterOptions options = new();
        VideoConversionRequest? request = null;
        using VideoConverterViewModel model = new(options, (value, _, _) =>
        {
            request = value;
            return Task.FromResult(new VideoConversionResult(true, false));
        });
        model.SelectInputFileRequested = _ => Task.FromResult<string?>(fixture.InputPath);
        model.SelectOutputFolderRequested = _ => Task.FromResult<string?>(fixture.DirectoryPath);
        await model.BrowseInputCommand.ExecuteAsync(null);
        await model.BrowseOutputFolderCommand.ExecuteAsync(null);
        Assert.Equal(fixture.InputPath, options.InputFilePath);
        Assert.Equal("input-output", options.OutputFileName);
        model.SelectedCodec = VideoConverterViewModel.Codecs.Single(x => x.Codec == VideoConverterCodec.Vp9);
        model.VideoQuality = 40;
        model.UseBitrate = true;
        model.VideoBitrate = 2700;
        model.AutoOpenFolder = false;
        model.OutputFileName = "converted";
        string expectedOutput = Path.Combine(fixture.DirectoryPath, "converted.webm");

        await model.StartEncodingCommand.ExecuteAsync(null);

        Assert.NotNull(request);
        Assert.Equal(expectedOutput, request.OutputFilePath);
        Assert.False(request.AutoOpenFolder);
        Assert.Contains($"-i \"{fixture.InputPath}\"", request.Arguments);
        Assert.Contains("-c:v libvpx-vp9 -b:v 2700k", request.Arguments);
        Assert.Contains("-c:a libvorbis -q:a 3", request.Arguments);
        Assert.EndsWith($"-y \"{expectedOutput}\"", request.Arguments);
        Assert.Equal(ConverterVideoCodecs.vp9, options.VideoCodec);
        Assert.Equal(40, options.VideoQuality);
        Assert.Equal(2700, options.VideoQualityBitrate);
        Assert.True(options.VideoQualityUseBitrate);
        Assert.True(model.CanEdit);
        Assert.Equal(100, model.Progress);
        Assert.Equal(string.Format(Strings.VideoConverterViewModel_Conversion_complete, expectedOutput), model.StatusText);
    }

    [Fact]
    public async Task CancelledOrFailedPickerReleasesBusyState()
    {
        using Fixture fixture = new();
        using VideoConverterViewModel model = new(fixture.Options, (_, _, _) => throw new InvalidOperationException());
        model.SelectInputFileRequested = _ => Task.FromCanceled<string?>(new CancellationToken(true));
        await model.BrowseInputCommand.ExecuteAsync(null);
        Assert.True(model.CanEdit);
        fixture.AssertOriginalSettings(model);

        InvalidOperationException failure = new("Fixture picker failed.");
        model.SelectOutputFolderRequested = _ => Task.FromException<string?>(failure);
        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => model.BrowseOutputFolderCommand.ExecuteAsync(null)));
        Assert.True(model.CanEdit);
        Assert.False(model.IsSelecting);
        fixture.AssertOriginalSettings(model);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CloseCancelsOnceButKeepsJobTokenAliveUntilActualCompletion(bool lateFailure)
    {
        using Fixture fixture = new();
        TaskCompletionSource<VideoConversionResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken token = default;
        int cancellations = 0;
        using VideoConverterViewModel model = new(fixture.Options, (_, _, value) =>
        {
            token = value;
            return completion.Task;
        });
        Task conversion = model.StartEncodingCommand.ExecuteAsync(null);
        using CancellationTokenRegistration registration = token.Register(() => cancellations++);
        Assert.False(token.WaitHandle.WaitOne(0));
        Assert.True(model.CanStop);
        TryChangeSettings(model);
        fixture.AssertOriginalSettings(model);
        string status = model.StatusText;

        model.Dispose();
        model.Dispose();
        model.StopEncodingCommand.Execute(null);
        Assert.Equal(1, cancellations);
        Assert.True(token.IsCancellationRequested);
        Assert.True(token.WaitHandle.WaitOne(0)); // The worker can still use its cancellation resources.
        Assert.False(model.CanStop);
        Assert.False(model.CanEdit);
        Assert.False(conversion.IsCompleted);
        if (lateFailure) completion.SetException(new InvalidOperationException("Late fixture error."));
        else completion.SetResult(new VideoConversionResult(true, false));
        await conversion.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(status, model.StatusText);
        Assert.Equal(0, model.Progress);
        Assert.False(model.IsEncoding);
        Assert.Throws<ObjectDisposedException>(() => token.WaitHandle);
        fixture.AssertOriginalSettings(model);
    }

    [Fact]
    public async Task QueuedProgressCannotOverwriteAResultOrAStartedNextJob()
    {
        using Fixture fixture = new();
        using QueuedContext context = new();
        TaskCompletionSource<VideoConversionResult> second = new(TaskCreationOptions.RunContinuationsAsynchronously);
        IProgress<double>? firstProgress = null, secondProgress = null;
        int calls = 0;
        using VideoConverterViewModel model = new(fixture.Options, (_, progress, _) =>
        {
            if (++calls == 1)
            {
                firstProgress = progress;
                progress.Report(20);
                return Task.FromResult(new VideoConversionResult(true, false));
            }
            secondProgress = progress;
            return second.Task;
        });
        await StartWithContext(model, context);
        Assert.Equal(100, model.Progress);
        Task next = StartWithContext(model, context);
        Assert.True(model.IsEncoding);
        Assert.NotNull(firstProgress);
        Assert.NotNull(secondProgress);
        secondProgress.Report(65);
        firstProgress.Report(5);
        context.RunAll();
        Assert.Equal(65, model.Progress);

        second.SetResult(new VideoConversionResult(true, false));
        await context.CompleteAsync(next);
        secondProgress.Report(15);
        context.RunAll();
        Assert.Equal(100, model.Progress);
        Assert.True(model.CanEdit);
    }

    [Fact]
    public async Task StopKeepsJobBusyAndRejectsQueuedProgressAndLateSuccess()
    {
        using Fixture fixture = new();
        using QueuedContext context = new();
        TaskCompletionSource<VideoConversionResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        IProgress<double>? progress = null;
        int cancellations = 0;
        using VideoConverterViewModel model = new(fixture.Options, (_, value, token) =>
        {
            progress = value;
            token.Register(() => cancellations++);
            return completion.Task;
        });
        Task conversion = StartWithContext(model, context);
        Assert.NotNull(progress);
        progress.Report(35);
        model.StopEncodingCommand.Execute(null);
        model.StopEncodingCommand.Execute(null);
        context.RunAll();
        Assert.Equal(1, cancellations);
        Assert.Equal(0, model.Progress);
        Assert.Equal(Strings.VideoConverterViewModel_Stopping, model.StatusText);
        Assert.True(model.IsEncoding);
        Assert.False(model.CanEdit);
        Assert.False(model.CanStop);
        await model.StartEncodingCommand.ExecuteAsync(null);
        completion.SetResult(new VideoConversionResult(true, false));
        await context.CompleteAsync(conversion);
        Assert.Equal(Strings.VideoConverterViewModel_Conversion_stopped, model.StatusText);
        Assert.Equal(0, model.Progress);
        Assert.True(model.CanEdit);
    }

    [Fact]
    public async Task QueuedProgressAndPickerErrorsAfterCloseAreIgnored()
    {
        using Fixture fixture = new();
        using QueuedContext context = new();
        TaskCompletionSource<VideoConversionResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        IProgress<double>? progress = null;
        using VideoConverterViewModel model = new(fixture.Options, (_, value, _) =>
        {
            progress = value;
            return completion.Task;
        });
        Task conversion = StartWithContext(model, context);
        Assert.NotNull(progress);
        progress.Report(45);
        string status = model.StatusText;
        model.Dispose();
        progress.Report(90);
        context.RunAll();
        Assert.True(model.IsClosed);
        Assert.Equal(0, model.Progress);
        Assert.Equal(status, model.StatusText);
        completion.SetResult(new VideoConversionResult(true, false));
        await context.CompleteAsync(conversion);

        TaskCompletionSource<string?> picker = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using VideoConverterViewModel pickerModel = new(fixture.Options, (_, _, _) => throw new InvalidOperationException());
        pickerModel.SelectInputFileRequested = _ => picker.Task;
        Task selection = pickerModel.BrowseInputCommand.ExecuteAsync(null);
        pickerModel.Dispose();
        picker.SetException(new InvalidOperationException("Late storage-provider error."));
        await selection.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.False(pickerModel.IsSelecting);
        fixture.AssertOriginalSettings(pickerModel);
    }

    [Fact]
    public async Task CloseBeforeJobDispatchDoesNotCallTheHandler()
    {
        using Fixture fixture = new();
        int calls = 0;
        using VideoConverterViewModel model = new(fixture.Options, (_, _, _) =>
        {
            calls++;
            return Task.FromResult(new VideoConversionResult(true, false));
        });
        model.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(model.IsEncoding) && model.IsEncoding) model.Dispose();
        };
        await model.StartEncodingCommand.ExecuteAsync(null);
        Assert.Equal(0, calls);
        Assert.False(model.IsEncoding);
        Assert.False(model.CanEdit);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FailedJobReturnsToIdleAndAllowsRetry(bool throws)
    {
        using Fixture fixture = new();
        int calls = 0;
        using VideoConverterViewModel model = new(fixture.Options, (_, _, _) =>
        {
            if (++calls > 1) return Task.FromResult(new VideoConversionResult(true, false));
            return throws ? Task.FromException<VideoConversionResult>(new InvalidOperationException("Fixture failure."))
                : Task.FromResult(new VideoConversionResult(false, false, "Fixture failure."));
        });
        await model.StartEncodingCommand.ExecuteAsync(null);
        Assert.Equal(string.Format(Strings.VideoConverterViewModel_Conversion_failed_message, "Fixture failure."), model.StatusText);
        Assert.Equal(0, model.Progress);
        Assert.True(model.CanEdit);
        Assert.False(model.CanStop);
        await model.StartEncodingCommand.ExecuteAsync(null);
        Assert.Equal(2, calls);
        Assert.Equal(100, model.Progress);
    }

    private static void TryChangeSettings(VideoConverterViewModel model)
    {
        model.InputFilePath = "rejected.mp4";
        model.OutputFolderPath = "rejected-folder";
        model.OutputFileName = "rejected-output";
        model.SelectedCodec = VideoConverterViewModel.Codecs.Single(x => x.Codec == VideoConverterCodec.Gif);
        model.UseBitrate = true;
        model.VideoQuality = 5;
        model.VideoBitrate = 100;
        model.AutoOpenFolder = false;
    }

    private static Task StartWithContext(VideoConverterViewModel model, SynchronizationContext context)
    {
        SynchronizationContext? previous = SynchronizationContext.Current;
        try
        {
            SynchronizationContext.SetSynchronizationContext(context);
            return model.StartEncodingCommand.ExecuteAsync(null);
        }
        finally { SynchronizationContext.SetSynchronizationContext(previous); }
    }

    private sealed class QueuedContext : SynchronizationContext, IDisposable
    {
        private readonly ConcurrentQueue<(SendOrPostCallback Callback, object? State)> _queue = new();
        private readonly SemaphoreSlim _posted = new(0);
        public override void Post(SendOrPostCallback callback, object? state)
        {
            _queue.Enqueue((callback, state));
            _posted.Release();
        }
        public void RunAll()
        {
            while (_queue.TryDequeue(out var item)) item.Callback(item.State);
        }
        public async Task CompleteAsync(Task operation)
        {
            using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(5));
            while (!operation.IsCompleted)
            {
                RunAll();
                if (!operation.IsCompleted) await _posted.WaitAsync(timeout.Token);
            }
            RunAll();
            await operation;
        }
        public void Dispose() => _posted.Dispose();
    }

    private sealed class Fixture : IDisposable
    {
        public string DirectoryPath { get; } = Path.Combine(Path.GetTempPath(), "ShareX-converter-" + Guid.NewGuid().ToString("N"));
        public string InputPath => Path.Combine(DirectoryPath, "input.mp4");
        public VideoConverterOptions Options { get; }
        public Fixture()
        {
            Directory.CreateDirectory(DirectoryPath);
            File.WriteAllBytes(InputPath, [1]); // Existence fixture only; never passed to a media engine.
            Options = new() { InputFilePath = InputPath, OutputFolderPath = DirectoryPath, OutputFileName = "output" };
        }
        public void AssertOriginalSettings(VideoConverterViewModel model)
        {
            Assert.Equal(InputPath, model.InputFilePath);
            Assert.Equal(InputPath, Options.InputFilePath);
            Assert.Equal(DirectoryPath, model.OutputFolderPath);
            Assert.Equal(DirectoryPath, Options.OutputFolderPath);
            Assert.Equal("output", model.OutputFileName);
            Assert.Equal("output", Options.OutputFileName);
            Assert.Equal(VideoConverterCodec.X264, model.SelectedCodec.Codec);
            Assert.Equal(ConverterVideoCodecs.x264, Options.VideoCodec);
            Assert.False(model.UseBitrate);
            Assert.False(Options.VideoQualityUseBitrate);
            Assert.Equal(23, model.VideoQuality);
            Assert.Equal(23, Options.VideoQuality);
            Assert.Equal(3000, model.VideoBitrate);
            Assert.Equal(3000, Options.VideoQualityBitrate);
            Assert.True(model.AutoOpenFolder);
            Assert.True(Options.AutoOpenFolder);
        }
        public void Dispose()
        {
            File.Delete(InputPath);
            Directory.Delete(DirectoryPath); // Exact, fixture-owned directory; never recursive.
        }
    }
}
