#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LocalizedResources = ShareX.HelpersLib.Properties.Resources;

namespace ShareX.HelpersLib;

public readonly record struct DownloaderFormResult(DialogResult DialogResult, DownloaderFormStatus Status);

public partial class DownloaderForm : Window
{
    public delegate void DownloaderInstallEventHandler(string filePath);
    public event DownloaderInstallEventHandler? InstallRequested;

    public string URL { get; set; }
    public string FileName { get; set; }
    public string DownloadLocation { get; private set; } = string.Empty;
    public string AcceptHeader { get; set; } = string.Empty;
    public bool AutoStartDownload { get; set; } = true;
    public InstallType InstallType { get; set; } = InstallType.Silent;
    public bool AutoStartInstall { get; set; } = true;
    public DownloaderFormStatus Status { get; private set; } = DownloaderFormStatus.Waiting;
    public bool RunInstallerInBackground { get; set; } = true;

    private FileDownloader? _fileDownloader;
    private DialogResult _result;

    public DownloaderForm() : this(string.Empty, string.Empty)
    {
    }

    private DownloaderForm(string url, string fileName)
    {
        URL = url;
        FileName = fileName;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        FileNameText.Text = Helpers.SafeStringFormat(LocalizedResources.DownloaderForm_DownloaderForm_Filename___0_, FileName);
        ActionButton.Content = "Download";
        ChangeStatus(LocalizedResources.DownloaderForm_DownloaderForm_Waiting_);

        Opened += OnOpened;
        Closing += OnClosing;
    }

    public static Task<DownloaderFormResult> ShowAsync(
        string url,
        string fileName,
        Action<DownloaderForm>? configure = null)
    {
        return ShowAsyncCore(() => new DownloaderForm(url, fileName), configure);
    }

    public static Task<DownloaderFormResult> ShowAsync(UpdateChecker updateChecker)
    {
        return ShowAsyncCore(() => new DownloaderForm(updateChecker.DownloadURL, updateChecker.FileName), form =>
        {
            if (updateChecker is GitHubUpdateChecker)
            {
                form.AcceptHeader = "application/octet-stream";
            }
        });
    }

    private static Task<DownloaderFormResult> ShowAsyncCore(
        Func<DownloaderForm> createWindow,
        Action<DownloaderForm>? configure)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        TaskCompletionSource<DownloaderFormResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                DownloaderForm window = createWindow();
                configure?.Invoke(window);
                window.Closed += (_, _) => completion.TrySetResult(new DownloaderFormResult(window._result, window.Status));
                window.Show();
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        });

        return completion.Task;
    }

    public void Install()
    {
        if (Status != DownloaderFormStatus.DownloadCompleted) return;

        Status = DownloaderFormStatus.InstallStarted;
        _result = DialogResult.OK;
        ActionButton.IsEnabled = false;
        RunInstallerWithDelay();
        Close();
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        if (AutoStartDownload)
        {
            await StartDownloadAsync();
        }
    }

    private async void OnActionClick(object? sender, RoutedEventArgs e)
    {
        if (Status == DownloaderFormStatus.Waiting)
        {
            await StartDownloadAsync();
        }
        else if (Status == DownloaderFormStatus.DownloadCompleted)
        {
            Install();
        }
        else
        {
            _result = DialogResult.Cancel;
            Close();
        }
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (Status == DownloaderFormStatus.DownloadStarted)
        {
            _fileDownloader?.StopDownload();
        }
    }

    private async Task StartDownloadAsync()
    {
        if (string.IsNullOrEmpty(URL) || Status != DownloaderFormStatus.Waiting) return;

        Status = DownloaderFormStatus.DownloadStarted;
        ActionButton.Content = LocalizedResources.DownloaderForm_StartDownload_Cancel;
        DownloadProgress.IsIndeterminate = true;

        string folderPath = Path.Combine(Path.GetTempPath(), "ShareX");
        FileHelpers.CreateDirectory(folderPath);
        DownloadLocation = Path.Combine(folderPath, FileName);

        DebugHelper.WriteLine($"Downloading: \"{URL}\" -> \"{DownloadLocation}\"");

        _fileDownloader = new FileDownloader(URL, DownloadLocation)
        {
            AcceptHeader = AcceptHeader
        };
        _fileDownloader.FileSizeReceived += OnFileSizeReceived;
        _fileDownloader.ProgressChanged += OnProgressChanged;

        ChangeStatus(LocalizedResources.DownloaderForm_StartDownload_Getting_file_size_);

        try
        {
            bool completed = await _fileDownloader.StartDownload();
            if (!completed) return;

            ChangeStatus(LocalizedResources.DownloaderForm_fileDownloader_DownloadCompleted_Download_completed_);
            Status = DownloaderFormStatus.DownloadCompleted;
            ActionButton.Content = LocalizedResources.DownloaderForm_fileDownloader_DownloadCompleted_Install;

            if (AutoStartInstall)
            {
                Install();
            }
        }
        catch (Exception exception)
        {
            DownloadProgress.IsIndeterminate = false;
            ChangeStatus(exception.Message);
        }
    }

    private void OnFileSizeReceived()
    {
        Dispatcher.UIThread.Post(() =>
        {
            DownloadProgress.IsIndeterminate = false;
            ChangeStatus(LocalizedResources.DownloaderForm_StartDownload_Downloading_);
            UpdateProgress();
        });
    }

    private void OnProgressChanged() => Dispatcher.UIThread.Post(UpdateProgress);

    private void UpdateProgress()
    {
        if (_fileDownloader == null) return;

        DownloadProgress.Value = _fileDownloader.DownloadPercentage;
        ProgressText.Text = $@"{LocalizedResources.DownloaderForm_FileDownloader_ProgressChanged_Progress}: {_fileDownloader.DownloadPercentage:0.0}%
{LocalizedResources.DownloaderForm_FileDownloader_ProgressChanged_DownloadSpeed}: {((long)_fileDownloader.DownloadSpeed).ToSizeString()}/s
{LocalizedResources.DownloaderForm_FileDownloader_ProgressChanged_FileSize}: {_fileDownloader.DownloadedSize.ToSizeString()} / {_fileDownloader.FileSize.ToSizeString()}";
    }

    private void ChangeStatus(string status)
    {
        void Update() => StatusText.Text = Helpers.SafeStringFormat(LocalizedResources.DownloaderForm_ChangeStatus_Status___0_, status);
        if (Dispatcher.UIThread.CheckAccess()) Update();
        else Dispatcher.UIThread.Post(Update);
    }

    private void RunInstallerWithDelay(int delay = 1000)
    {
        if (RunInstallerInBackground)
        {
            Thread thread = new(() =>
            {
                Thread.Sleep(delay);
                RunInstaller();
            })
            {
                // Keep the process alive during ShareX shutdown so the delayed
                // installer launch is not abandoned before Process.Start().
                IsBackground = false
            };
            thread.Start();
        }
        else
        {
            Hide();
            RunInstaller();
        }
    }

    private void RunInstaller()
    {
        if (InstallType == InstallType.Event)
        {
            InstallRequested?.Invoke(DownloadLocation);
            return;
        }

        try
        {
            using Process process = new();
            ProcessStartInfo startInfo = new()
            {
                FileName = DownloadLocation,
                Arguments = "/UPDATE",
                UseShellExecute = true
            };

            if (InstallType == InstallType.Silent) startInfo.Arguments += " /SILENT";
            else if (InstallType == InstallType.VerySilent) startInfo.Arguments += " /VERYSILENT";

            if (Helpers.IsDefaultInstallDir() && !Helpers.IsMemberOfAdministratorsGroup())
            {
                startInfo.Verb = "runas";
            }

            process.StartInfo = startInfo;
            process.Start();
        }
        catch
        {
        }
    }
}
