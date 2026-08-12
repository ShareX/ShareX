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

namespace ShareX.HelpersLib;

public readonly record struct DownloaderWindowResult(DialogResult DialogResult, DownloaderWindowStatus Status);

public partial class DownloaderWindow : Window
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
    public DownloaderWindowStatus Status { get; private set; } = DownloaderWindowStatus.Waiting;
    public bool RunInstallerInBackground { get; set; } = true;

    private FileDownloader? _fileDownloader;
    private DialogResult _result;

    public DownloaderWindow() : this(string.Empty, string.Empty)
    {
    }

    private DownloaderWindow(string url, string fileName)
    {
        URL = url;
        FileName = fileName;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        FileNameText.Text = Helpers.SafeStringFormat(Localization.Strings.DownloaderForm_DownloaderForm_Filename___0_, FileName);
        ActionButton.Content = Localization.Strings.DownloaderWindow_Download;
        ChangeStatus(Localization.Strings.DownloaderForm_DownloaderForm_Waiting_);

        Opened += OnOpened;
        Closing += OnClosing;
    }

    public static Task<DownloaderWindowResult> ShowAsync(
        string url,
        string fileName,
        Action<DownloaderWindow>? configure = null)
    {
        return ShowAsyncCore(() => new DownloaderWindow(url, fileName), configure);
    }

    public static Task<DownloaderWindowResult> ShowAsync(UpdateChecker updateChecker)
    {
        return ShowAsyncCore(() => new DownloaderWindow(updateChecker.DownloadURL, updateChecker.FileName), window =>
        {
            if (updateChecker is GitHubUpdateChecker)
            {
                window.AcceptHeader = "application/octet-stream";
            }
        });
    }

    private static Task<DownloaderWindowResult> ShowAsyncCore(
        Func<DownloaderWindow> createWindow,
        Action<DownloaderWindow>? configure)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        TaskCompletionSource<DownloaderWindowResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                DownloaderWindow window = createWindow();
                configure?.Invoke(window);
                window.Closed += (_, _) => completion.TrySetResult(new DownloaderWindowResult(window._result, window.Status));
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
        if (Status != DownloaderWindowStatus.DownloadCompleted) return;

        Status = DownloaderWindowStatus.InstallStarted;
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
        if (Status == DownloaderWindowStatus.Waiting)
        {
            await StartDownloadAsync();
        }
        else if (Status == DownloaderWindowStatus.DownloadCompleted)
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
        if (Status == DownloaderWindowStatus.DownloadStarted)
        {
            _fileDownloader?.StopDownload();
        }
    }

    private async Task StartDownloadAsync()
    {
        if (string.IsNullOrEmpty(URL) || Status != DownloaderWindowStatus.Waiting) return;

        Status = DownloaderWindowStatus.DownloadStarted;
        ActionButton.Content = Localization.Strings.DownloaderForm_StartDownload_Cancel;
        DownloadProgress.Value = 0;

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

        ChangeStatus(Localization.Strings.DownloaderForm_StartDownload_Getting_file_size_);

        try
        {
            bool completed = await _fileDownloader.StartDownload();
            if (!completed) return;

            ChangeStatus(Localization.Strings.DownloaderForm_fileDownloader_DownloadCompleted_Download_completed_);
            Status = DownloaderWindowStatus.DownloadCompleted;
            ActionButton.Content = Localization.Strings.DownloaderForm_fileDownloader_DownloadCompleted_Install;

            if (AutoStartInstall)
            {
                Install();
            }
        }
        catch (Exception exception)
        {
            ChangeStatus(exception.Message);
        }
    }

    private void OnFileSizeReceived()
    {
        Dispatcher.UIThread.Post(() =>
        {
            ChangeStatus(Localization.Strings.DownloaderForm_StartDownload_Downloading_);
            UpdateProgress();
        });
    }

    private void OnProgressChanged() => Dispatcher.UIThread.Post(UpdateProgress);

    private void UpdateProgress()
    {
        if (_fileDownloader == null) return;

        DownloadProgress.Value = _fileDownloader.DownloadPercentage;
        ProgressText.Text = $@"{Localization.Strings.DownloaderForm_FileDownloader_ProgressChanged_Progress}: {_fileDownloader.DownloadPercentage:0.0}%
{Localization.Strings.DownloaderForm_FileDownloader_ProgressChanged_DownloadSpeed}: {((long)_fileDownloader.DownloadSpeed).ToSizeString()}/s
{Localization.Strings.DownloaderForm_FileDownloader_ProgressChanged_FileSize}: {_fileDownloader.DownloadedSize.ToSizeString()} / {_fileDownloader.FileSize.ToSizeString()}";
    }

    private void ChangeStatus(string status)
    {
        void Update() => StatusText.Text = Helpers.SafeStringFormat(Localization.Strings.DownloaderForm_ChangeStatus_Status___0_, status);
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
