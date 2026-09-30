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
using ShareX.Platform;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Desktop.Workflows;

public sealed record WorkflowResult(bool Success, string Message, string? FilePath = null, string? Url = null);

/// <summary>Capture, then the after capture tasks: save, copy, upload, edit, notify.</summary>
public sealed class CaptureWorkflow
{
    private readonly IPlatformServices platform;
    private readonly DesktopSettings settings;
    private readonly IUploadService uploader;
    private readonly IEditorLauncher editor;
    private readonly IHistoryRecorder? history;
    private readonly TimeProvider clock;

    public CaptureWorkflow(IPlatformServices platform, DesktopSettings settings, IUploadService uploader, IEditorLauncher editor, TimeProvider? clock = null, IHistoryRecorder? history = null)
    {
        this.history = history;
        this.platform = platform;
        this.settings = settings;
        this.uploader = uploader;
        this.editor = editor;
        this.clock = clock ?? TimeProvider.System;
    }

    public async Task<WorkflowResult> CaptureAsync(CaptureTarget target, AfterCapture actions, CancellationToken cancellationToken = default)
    {
        IScreenCaptureService capture = platform.ScreenCapture;

        if (!capture.Support.IsSupported)
        {
            return await FailAsync(actions, "Screen capture is not available: " + capture.Support.Reason).ConfigureAwait(false);
        }

        ScreenCaptureResult result;

        try
        {
            result = await capture.CaptureAsync(CreateRequest(target, capture), cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // The selector was closed or killed, which is a cancel and not a failure of the workflow itself.
            return new WorkflowResult(false, "Capture cancelled.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Closing the region selector without choosing an area is the normal way to cancel, not an error worth a notification.
            bool cancelled = target == CaptureTarget.Region && IsSelectionCancelled(ex);
            return cancelled ? new WorkflowResult(false, "Capture cancelled.") : await FailAsync(actions, "Capture failed: " + ex.Message).ConfigureAwait(false);
        }

        return await RunAfterCaptureAsync(result.Png, actions, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>The steps after a capture, also used for images that arrive another way (an editor, a file).</summary>
    public async Task<WorkflowResult> RunAfterCaptureAsync(byte[] png, AfterCapture actions, CancellationToken cancellationToken = default)
    {
        DateTimeOffset now = clock.GetLocalNow();
        string fileName = string.Format(System.Globalization.CultureInfo.InvariantCulture, settings.FileNameFormat, now.DateTime) + ".png";
        string? filePath = null;
        string? url = null;
        string summary = "Captured.";

        if (actions.Save)
        {
            try
            {
                filePath = await SaveAsync(png, fileName, now.DateTime, cancellationToken).ConfigureAwait(false);
                summary = "Saved " + filePath;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                return await FailAsync(actions, "Could not save the screenshot: " + ex.Message).ConfigureAwait(false);
            }
        }

        if (actions.CopyImage)
        {
            bool copied = await platform.Clipboard.SetImageAsync(png, cancellationToken).ConfigureAwait(false);
            summary += copied ? " Copied to the clipboard." : " The clipboard is unavailable: " + platform.Clipboard.Support.Reason;
        }

        string? uploadError = null;

        if (actions.Upload)
        {
            if (!uploader.IsConfigured(isImage: true, out string? reason))
            {
                uploadError = reason ?? "No upload destination is configured.";
            }
            else
            {
                UploadOutcome outcome = await uploader.UploadAsync(fileName, png, isImage: true, cancellationToken).ConfigureAwait(false);

                if (outcome.Success && !string.IsNullOrEmpty(outcome.Url))
                {
                    url = outcome.Url;
                    summary += " Uploaded: " + url;

                    if (settings.CopyUrlAfterUpload)
                    {
                        await platform.Clipboard.SetTextAsync(url, cancellationToken).ConfigureAwait(false);
                    }
                }
                else
                {
                    uploadError = outcome.Error ?? "The upload failed.";
                }
            }

            if (uploadError != null)
            {
                summary += " Upload failed: " + uploadError;
            }
        }

        if (filePath != null || url != null)
        {
            history?.Record(new HistoryEntry(fileName, filePath, url, "Image", url != null ? uploader.GetDestinationName(isImage: true) : "", now.DateTime));
        }

        if (actions.Edit)
        {
            await editor.OpenAsync(filePath, filePath == null ? png : null).ConfigureAwait(false);
        }

        if (actions.Notify)
        {
            await NotifyAsync(uploadError != null ? "Upload failed" : url != null ? "Uploaded" : "Screenshot", uploadError ?? url ?? summary, filePath).ConfigureAwait(false);
        }

        return new WorkflowResult(uploadError == null, summary, filePath, url);
    }

    private ScreenCaptureRequest CreateRequest(CaptureTarget target, IScreenCaptureService capture)
    {
        switch (target)
        {
            case CaptureTarget.Region:
                return new ScreenCaptureRequest { Mode = ScreenCaptureMode.Interactive, IncludeCursor = settings.IncludeCursor };
            case CaptureTarget.Screen:
                // There is no portable way to ask for the cursor position, so this is the primary screen.
                foreach (ScreenInfo screen in capture.GetScreens())
                {
                    if (screen.IsPrimary)
                    {
                        return ScreenCaptureRequest.ForScreen(screen.Id, settings.IncludeCursor);
                    }
                }

                goto default;
            default:
                return ScreenCaptureRequest.FullScreen(settings.IncludeCursor);
        }
    }

    private static bool IsSelectionCancelled(Exception ex) =>
        ex is OperationCanceledException || ex.Message.Contains("cancel", StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("selection", StringComparison.OrdinalIgnoreCase);

    private async Task<string> SaveAsync(byte[] png, string fileName, DateTime now, CancellationToken cancellationToken)
    {
        string root = string.IsNullOrWhiteSpace(settings.ScreenshotsFolder)
            ? Path.Combine(platform.Paths.GetPicturesDirectory(), "ShareX")
            : settings.ScreenshotsFolder;
        string folder = settings.GroupByMonth ? Path.Combine(root, now.ToString("yyyy-MM", System.Globalization.CultureInfo.InvariantCulture)) : root;
        Directory.CreateDirectory(folder);
        string path = GetUniquePath(Path.Combine(folder, fileName));
        await File.WriteAllBytesAsync(path, png, cancellationToken).ConfigureAwait(false);
        return path;
    }

    internal static string GetUniquePath(string path)
    {
        if (!File.Exists(path))
        {
            return path;
        }

        string folder = Path.GetDirectoryName(path) ?? "";
        string name = Path.GetFileNameWithoutExtension(path);
        string extension = Path.GetExtension(path);

        for (int i = 2; ; i++)
        {
            string candidate = Path.Combine(folder, $"{name}_{i}{extension}");

            if (!File.Exists(candidate))
            {
                return candidate;
            }
        }
    }

    private async Task<WorkflowResult> FailAsync(AfterCapture actions, string message)
    {
        if (actions.Notify)
        {
            await NotifyAsync("ShareX", message, null).ConfigureAwait(false);
        }

        return new WorkflowResult(false, message);
    }

    private async Task NotifyAsync(string title, string message, string? imagePath)
    {
        if (platform.Notifications.Support.IsSupported)
        {
            await platform.Notifications.ShowAsync(new PlatformNotification(title, message) { ImagePath = imagePath }).ConfigureAwait(false);
        }
    }
}
