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

using ShareX.Desktop.Commands;
using ShareX.Desktop.Settings;
using ShareX.Desktop.Workflows;
using ShareX.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.Desktop.Tests;

public sealed class CaptureWorkflowTests : IDisposable
{
    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now.ToUniversalTime();

        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }

    private sealed class FakeUploader : IUploadService
    {
        public string? NotConfiguredReason { get; set; }

        public UploadOutcome Outcome { get; set; } = new UploadOutcome(true, "https://example.test/a.png", null);

        public int Uploads { get; private set; }

        public string GetDestinationName(bool isImage) => "Example host";

        public string? LastFileName { get; private set; }

        public bool IsConfigured(bool isImage, out string? reason)
        {
            reason = NotConfiguredReason;
            return NotConfiguredReason == null;
        }

        public Task<UploadOutcome> UploadAsync(string fileName, byte[] data, bool isImage, CancellationToken cancellationToken)
        {
            Uploads++;
            LastFileName = fileName;
            return Task.FromResult(Outcome);
        }
    }

    private sealed class FakeHistory : IHistoryRecorder
    {
        public List<HistoryEntry> Entries { get; } = new List<HistoryEntry>();

        public void Record(HistoryEntry entry) => Entries.Add(entry);
    }

    private sealed class FakeEditor : IEditorLauncher
    {
        public string? OpenedPath { get; private set; }

        public byte[]? OpenedPng { get; private set; }

        public int Opens { get; private set; }

        public Task OpenAsync(string? filePath, byte[]? png)
        {
            Opens++;
            OpenedPath = filePath;
            OpenedPng = png;
            return Task.CompletedTask;
        }
    }

    private readonly string root = Path.Combine(Path.GetTempPath(), "sharex-workflow-" + Guid.NewGuid().ToString("N"));
    private readonly FakePlatform platform;
    private readonly FakeUploader uploader = new FakeUploader();
    private readonly FakeEditor editor = new FakeEditor();
    private readonly FakeHistory history = new FakeHistory();
    private readonly DesktopSettings settings = new DesktopSettings();
    private readonly CaptureWorkflow workflow;

    public CaptureWorkflowTests()
    {
        platform = new FakePlatform(root);
        workflow = new CaptureWorkflow(platform, settings, uploader, editor, new FixedClock(new DateTimeOffset(2026, 10, 1, 5, 52, 39, TimeSpan.Zero)), history);
    }

    public void Dispose()
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, true);
        }
    }

    private static AfterCapture Actions(bool save = true, bool copy = true, bool upload = false, bool edit = false, bool notify = true) => new AfterCapture(save, copy, upload, edit, notify);

    [Fact]
    public async Task Capture_SavesIntoAMonthFolderUnderPicturesAndCopiesTheImage()
    {
        WorkflowResult result = await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions());

        string expected = Path.Combine(root, "Pictures", "ShareX", "2026-10", "ShareX_20261001_055239.png");
        Assert.True(result.Success);
        Assert.Equal(expected, result.FilePath);
        Assert.Equal(platform.Capture.Png, await File.ReadAllBytesAsync(expected));
        Assert.Equal(platform.Capture.Png, platform.ClipboardFake.Image);
        Assert.Single(platform.NotificationsFake.Shown);
    }

    [Fact]
    public async Task Capture_NeverOverwritesAnExistingFile()
    {
        WorkflowResult first = await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions());
        WorkflowResult second = await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions());

        Assert.NotEqual(first.FilePath, second.FilePath);
        Assert.EndsWith("ShareX_20261001_055239_2.png", second.FilePath);
        Assert.True(File.Exists(first.FilePath));
    }

    [Fact]
    public async Task Capture_CustomFolderAndNoMonthGrouping()
    {
        settings.ScreenshotsFolder = Path.Combine(root, "shots");
        settings.GroupByMonth = false;

        WorkflowResult result = await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions());

        Assert.Equal(Path.Combine(root, "shots", "ShareX_20261001_055239.png"), result.FilePath);
    }

    [Theory]
    [InlineData(CaptureTarget.Region, ScreenCaptureMode.Interactive, null)]
    [InlineData(CaptureTarget.FullScreen, ScreenCaptureMode.FullScreen, null)]
    [InlineData(CaptureTarget.Screen, ScreenCaptureMode.Screen, "b")]
    public async Task Capture_AsksThePlatformForTheRightKindOfCapture(CaptureTarget target, ScreenCaptureMode mode, string? screenId)
    {
        await workflow.CaptureAsync(target, Actions(save: false, copy: false, notify: false));

        Assert.Equal(mode, platform.Capture.LastRequest!.Mode);
        Assert.Equal(screenId, platform.Capture.LastRequest.ScreenId);
    }

    [Fact]
    public async Task Capture_CanSkipSaveCopyAndNotification()
    {
        WorkflowResult result = await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions(save: false, copy: false, notify: false));

        Assert.True(result.Success);
        Assert.Null(result.FilePath);
        Assert.Null(platform.ClipboardFake.Image);
        Assert.Empty(platform.NotificationsFake.Shown);
        Assert.False(Directory.Exists(Path.Combine(root, "Pictures")));
    }

    [Fact]
    public async Task Upload_CopiesTheUrlAfterTheImage()
    {
        WorkflowResult result = await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions(upload: true));

        Assert.True(result.Success);
        Assert.Equal("https://example.test/a.png", result.Url);
        Assert.Equal("https://example.test/a.png", platform.ClipboardFake.Text);
        Assert.Equal("ShareX_20261001_055239.png", uploader.LastFileName);
        Assert.Equal("Uploaded", platform.NotificationsFake.Shown[0].Title);
    }

    [Fact]
    public async Task History_RecordsTheSavedFile_AndTheUploadHost()
    {
        await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions());
        await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions(upload: true));

        Assert.Equal(2, history.Entries.Count);
        Assert.Equal("", history.Entries[0].Host);
        Assert.Null(history.Entries[0].Url);
        Assert.NotNull(history.Entries[0].FilePath);
        Assert.Equal("Example host", history.Entries[1].Host);
        Assert.Equal("https://example.test/a.png", history.Entries[1].Url);
        Assert.Equal("Image", history.Entries[1].Type);
        Assert.Equal(new DateTime(2026, 10, 1, 5, 52, 39), history.Entries[1].When);
    }

    [Fact]
    public async Task History_SkipsACaptureThatWasNeitherSavedNorUploaded()
    {
        await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions(save: false));

        Assert.Empty(history.Entries);
    }

    [Fact]
    public async Task Upload_NotConfigured_ExplainsWhatToDoAndStillKeepsTheScreenshot()
    {
        uploader.NotConfiguredReason = "No upload destination is set.";

        WorkflowResult result = await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions(upload: true));

        Assert.False(result.Success);
        Assert.Equal(0, uploader.Uploads);
        Assert.NotNull(result.FilePath);
        Assert.True(File.Exists(result.FilePath));
        Assert.Contains("No upload destination is set.", result.Message);
        Assert.Equal("Upload failed", platform.NotificationsFake.Shown[0].Title);
    }

    [Fact]
    public async Task Upload_Failure_IsReportedAndDoesNotReplaceTheClipboardImageWithAnUrl()
    {
        uploader.Outcome = UploadOutcome.Failed("HTTP 500");

        WorkflowResult result = await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions(upload: true));

        Assert.False(result.Success);
        Assert.Contains("HTTP 500", result.Message);
        Assert.Null(platform.ClipboardFake.Text);
        Assert.NotNull(platform.ClipboardFake.Image);
    }

    [Fact]
    public async Task Edit_OpensTheSavedFile_OrTheBytesWhenNothingWasSaved()
    {
        WorkflowResult saved = await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions(edit: true));
        Assert.Equal(saved.FilePath, editor.OpenedPath);
        Assert.Null(editor.OpenedPng);

        await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions(save: false, edit: true));
        Assert.Null(editor.OpenedPath);
        Assert.Equal(platform.Capture.Png, editor.OpenedPng);
    }

    [Fact]
    public async Task Capture_Unsupported_ReportsWhyAndTellsTheUser()
    {
        platform.Capture.Support = FeatureSupport.NotSupported("Install grim.");

        WorkflowResult result = await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions());

        Assert.False(result.Success);
        Assert.Contains("Install grim.", result.Message);
        Assert.Single(platform.NotificationsFake.Shown);
    }

    [Fact]
    public async Task Region_AFailureThatMentionsTheSelection_IsStillReportedAsAFailure()
    {
        // Only OperationCanceledException means cancel. Guessing from the message hid real errors before.
        platform.Capture.Throw = new InvalidOperationException("grim failed: invalid selection geometry");

        WorkflowResult result = await workflow.CaptureAsync(CaptureTarget.Region, Actions());

        Assert.False(result.Success);
        Assert.Contains("invalid selection geometry", result.Message);
        Assert.Single(platform.NotificationsFake.Shown);
    }

    [Fact]
    public async Task Region_SelectorKilled_IsACancelNotACrash()
    {
        platform.Capture.Throw = new OperationCanceledException();

        WorkflowResult result = await workflow.CaptureAsync(CaptureTarget.Region, Actions());

        Assert.False(result.Success);
        Assert.Equal("Capture cancelled.", result.Message);
        Assert.Empty(platform.NotificationsFake.Shown);
    }

    [Fact]
    public async Task Capture_CancelledByTheCaller_StillPropagates()
    {
        platform.Capture.Throw = new OperationCanceledException();
        using CancellationTokenSource cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => workflow.CaptureAsync(CaptureTarget.Region, Actions(), cts.Token));
    }

    [Fact]
    public async Task FullScreen_RealFailure_IsNotMistakenForACancel()
    {
        platform.Capture.Throw = new InvalidOperationException("grim failed: compositor does not support screencopy");

        WorkflowResult result = await workflow.CaptureAsync(CaptureTarget.FullScreen, Actions());

        Assert.Contains("grim failed", result.Message);
        Assert.Single(platform.NotificationsFake.Shown);
    }
}
