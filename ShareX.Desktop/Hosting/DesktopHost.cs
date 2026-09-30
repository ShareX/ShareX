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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Theming;
using ShareX.Desktop.Commands;
using ShareX.Desktop.Ipc;
using ShareX.Desktop.Settings;
using ShareX.Desktop.Workflows;
using ShareX.HistoryLib;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Presentation.ViewModels;
using ShareX.ImageEditor.Presentation.Views;
using ShareX.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Desktop.Hosting;

/// <summary>The running application: tray icon, command handling and the Avalonia lifetime.</summary>
public sealed class DesktopHost : IEditorLauncher
{
    private readonly IPlatformServices platform;
    private readonly DesktopSettings settings;
    private readonly IUploadService uploader;
    private readonly CaptureWorkflow workflow;
    private readonly SqliteHistoryRecorder? history;
    private readonly ImageHistorySettings historySettings = new ImageHistorySettings();
    private TrayIcon? trayIcon;

    public DesktopHost(IPlatformServices platform, DesktopSettings settings, IUploadService uploader, SqliteHistoryRecorder? history = null)
    {
        this.platform = platform;
        this.settings = settings;
        this.uploader = uploader;
        this.history = history;
        workflow = new CaptureWorkflow(platform, settings, uploader, this, history: history);
    }

    /// <summary>Runs the Avalonia application until Quit. Must be called on the main thread.</summary>
    public int Run(string[] args, DesktopCommand initialCommand, Action onShutdown)
    {
        AvaloniaBootstrapper.Initialize(args, () =>
        {
            // Avalonia's Linux tray watcher throws TaskCanceledException on the UI thread when the icon is disposed at shutdown.
            // That is expected, and must not turn a clean quit into a crash.
            Dispatcher.UIThread.UnhandledException += (_, e) =>
            {
                if (e.Exception is OperationCanceledException)
                {
                    e.Handled = true;
                }
            };

            // The theme manager needs a running Avalonia application, so it is configured here and not before Initialize.
            ThemeManager.Configure(new ApplicationThemeOptions());
            CreateTrayIcon();

            if (initialCommand.Kind != CommandKind.Run)
            {
                _ = Task.Run(async () =>
                {
                    CommandResponse response = await HandleAsync(initialCommand, CancellationToken.None).ConfigureAwait(false);
                    await Console.Out.WriteLineAsync(response.Message).ConfigureAwait(false);
                });
            }

            return Task.CompletedTask;
        }, () =>
        {
            trayIcon?.Dispose();
            history?.Dispose();
            onShutdown();
        });

        return AvaloniaBootstrapper.Run();
    }

    public async Task<CommandResponse> HandleAsync(DesktopCommand command, CancellationToken cancellationToken)
    {
        switch (command.Kind)
        {
            case CommandKind.Run:
                return new CommandResponse(true, "ShareX is running.");
            case CommandKind.Status:
                return new CommandResponse(true, $"ShareX is running (process {Environment.ProcessId}) on {platform.Info}.");
            case CommandKind.History:
                ShowHistory();
                return new CommandResponse(history != null, history != null ? "Opened the history." : "History is not available.");
            case CommandKind.Quit:
                AvaloniaBootstrapper.Shutdown();
                return new CommandResponse(true, "ShareX is shutting down.");
            case CommandKind.Capture:
                WorkflowResult result = await workflow.CaptureAsync(command.Target, settings.ToAfterCapture(command.AfterCapture), cancellationToken).ConfigureAwait(false);
                return new CommandResponse(result.Success, result.Message);
            case CommandKind.Upload:
                return await UploadFilesAsync(command.Files, settings.ToAfterCapture(command.AfterCapture).Notify, cancellationToken).ConfigureAwait(false);
            case CommandKind.Editor:
                foreach (string file in command.Files.Count == 0 ? [""] : command.Files)
                {
                    await OpenAsync(string.IsNullOrEmpty(file) ? null : Path.GetFullPath(file), null).ConfigureAwait(false);
                }

                return new CommandResponse(true, "Opened the editor.");
            default:
                return new CommandResponse(false, $"'{command.Kind}' is not handled by the running application.");
        }
    }

    private async Task<CommandResponse> UploadFilesAsync(IReadOnlyList<string> files, bool notify, CancellationToken cancellationToken)
    {
        StringBuilder message = new StringBuilder();
        bool allOk = true;
        List<string> urls = new List<string>();

        foreach (string file in files)
        {
            string path = Path.GetFullPath(file);

            if (!File.Exists(path))
            {
                message.AppendLine($"{file}: file not found.");
                allOk = false;
                continue;
            }

            bool isImage = IsImage(path);

            if (!uploader.IsConfigured(isImage, out string? reason))
            {
                message.AppendLine($"{file}: {reason}");
                allOk = false;
                continue;
            }

            UploadOutcome outcome = await uploader.UploadAsync(Path.GetFileName(path), await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false), isImage, cancellationToken).ConfigureAwait(false);

            if (outcome.Success && outcome.Url != null)
            {
                urls.Add(outcome.Url);
                message.AppendLine($"{file}: {outcome.Url}");
            }
            else
            {
                allOk = false;
                message.AppendLine($"{file}: {outcome.Error}");
            }
        }

        if (urls.Count > 0 && settings.CopyUrlAfterUpload)
        {
            await platform.Clipboard.SetTextAsync(string.Join(Environment.NewLine, urls), cancellationToken).ConfigureAwait(false);
        }

        if (notify && platform.Notifications.Support.IsSupported)
        {
            await platform.Notifications.ShowAsync(new PlatformNotification(allOk ? "Uploaded" : "Upload failed", message.ToString().Trim()), cancellationToken).ConfigureAwait(false);
        }

        return new CommandResponse(allOk, message.ToString().TrimEnd());
    }

    internal static bool IsImage(string path) => Path.GetExtension(path).ToLowerInvariant() is ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp" or ".webp" or ".tif" or ".tiff";

    public Task OpenAsync(string? filePath, byte[]? png)
    {
        return Dispatcher.UIThread.InvokeAsync(() =>
        {
            EditorWindow window = new EditorWindow(new ImageEditorOptions());

            if (window.DataContext is MainViewModel vm)
            {
                vm.ShowFileMenu = true;
                vm.ShowTaskButtons = false;
                vm.UseContinueWorkflow = false;
                vm.ShowBottomToolbar = true;
                vm.ShowStartScreen = filePath == null && png == null;
            }

            if (filePath != null)
            {
                window.LoadImage(filePath);
            }
            else if (png != null)
            {
                using MemoryStream stream = new MemoryStream(png);
                window.LoadImage(stream);
            }

            window.Show();
            window.Activate();
        }).GetTask();
    }

    private void CreateTrayIcon()
    {
        if (Application.Current == null)
        {
            return;
        }

        NativeMenu menu = new NativeMenu();
        menu.Items.Add(Item("Capture region", () => Fire(new DesktopCommand(CommandKind.Capture) { Target = CaptureTarget.Region })));
        menu.Items.Add(Item("Capture full screen", () => Fire(new DesktopCommand(CommandKind.Capture) { Target = CaptureTarget.FullScreen })));
        menu.Items.Add(new NativeMenuItemSeparator());
        menu.Items.Add(Item("Image editor", () => Fire(new DesktopCommand(CommandKind.Editor))));
        menu.Items.Add(Item("History", ShowHistory));
        menu.Items.Add(Item("Open screenshots folder", OpenScreenshotsFolder));
        menu.Items.Add(new NativeMenuItemSeparator());
        menu.Items.Add(Item("Quit", () => AvaloniaBootstrapper.Shutdown()));

        using Stream icon = AssetLoader.Open(new Uri("avares://sharex/Assets/ShareX.png"));
        trayIcon = new TrayIcon
        {
            Icon = new WindowIcon(icon),
            ToolTipText = "ShareX",
            Menu = menu,
            IsVisible = true
        };

        TrayIcon.SetIcons(Application.Current, new TrayIcons { trayIcon });
    }

    private static NativeMenuItem Item(string header, Action click)
    {
        NativeMenuItem item = new NativeMenuItem(header);
        item.Click += (_, _) => click();
        return item;
    }

    private void ShowHistory()
    {
        if (history == null)
        {
            return;
        }

        HistoryIntegration.ShowImageHistoryWindow(history.Manager, historySettings, new HistoryWindowServices
        {
            EditImage = path => Fire(new DesktopCommand(CommandKind.Editor) { Files = [path] }),
            UploadFile = path => Fire(new DesktopCommand(CommandKind.Upload) { Files = [path] })
        });
    }

    private void Fire(DesktopCommand command) => _ = Task.Run(() => HandleAsync(command, CancellationToken.None));

    private void OpenScreenshotsFolder()
    {
        string folder = string.IsNullOrWhiteSpace(settings.ScreenshotsFolder) ? Path.Combine(platform.Paths.GetPicturesDirectory(), "ShareX") : settings.ScreenshotsFolder;
        Directory.CreateDirectory(folder);
        platform.Shell.OpenPath(folder);
    }
}
