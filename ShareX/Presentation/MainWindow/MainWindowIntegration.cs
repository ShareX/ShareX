#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using ShareX.HelpersLib;
using ShareX.Localization;
using ShareX.UploadersLib;
using System;
using System.IO;

namespace ShareX;

public static class MainWindowIntegration
{
    private static MainWindow? _window;
    private static ITrayIconService? _trayIconService;
    private static bool _isVisible;

    public static bool IsInitialized => _window != null;
    public static bool IsVisible => _isVisible;
    internal static IntPtr WindowHandle => _window?.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
    internal static MainWindow? Instance => _window;
    internal static ITrayIconService TrayIconService => _trayIconService ??
        throw new InvalidOperationException("The main window integration is not initialized.");

    internal static void Initialize(ITrayIconService trayIconService, bool show)
    {
        RunOnUiThread(() =>
        {
            if (_window == null)
            {
                _trayIconService = trayIconService;
                MainWindow window = new MainWindow(trayIconService);
                _window = window;

                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = window;
                }

                window.Closed += (_, _) =>
                {
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime &&
                        ReferenceEquals(lifetime.MainWindow, window))
                    {
                        lifetime.MainWindow = null;
                    }

                    _window = null;
                    _isVisible = false;
                };
            }

            if (show)
            {
                _window.ShowAndActivate();
                _isVisible = true;
            }
        });
    }

    public static void Activate()
    {
        RunOnUiThread(() =>
        {
            _window?.ShowAndActivate();
            _isVisible = _window?.IsVisible == true;
        });
    }

    public static void Hide()
    {
        RunOnUiThread(() =>
        {
            _window?.HideToTray();
            _isVisible = false;
        });
    }

    public static void Close()
    {
        RunOnUiThread(() =>
        {
            _window?.CloseFromHost();
            _isVisible = false;
        });
    }

    public static void SetTitle(string title) => RunOnUiThread(() =>
    {
        _window?.SetTitle(title);
        if (_trayIconService != null) _trayIconService.ToolTipText = title;
    });

    public static void SetTrayVisible(bool visible) => RunOnUiThread(() =>
    {
        if (_trayIconService != null) _trayIconService.Visible = visible;
    });

    public static void SetTrayIcon(System.Drawing.Icon icon)
    {
        using MemoryStream stream = new();
        icon.Save(stream);
        byte[] iconBytes = stream.ToArray();
        RunOnUiThread(() => _trayIconService?.SetIcon(iconBytes));
    }

    public static void ShowTrayMenu() => RunOnUiThread(() => _window?.ShowTrayMenu());

    public static void RefreshMenus() => RunOnUiThread(() => _window?.RefreshMenus());

    internal static void SetScreenshotDelay(decimal delay)
    {
        Program.DefaultTaskSettings.CaptureSettings.ScreenshotDelay = delay;
        RefreshMenus();
    }

    internal static void ExecuteCommand(MainFormCommand command) => RunOnUiThread(() =>
    {
        switch (command)
        {
            case MainFormCommand.ApplicationSettings:
                ApplicationSettingsIntegration.Show();
                break;
            case MainFormCommand.TaskSettings:
                TaskSettingsIntegration.Show(Program.DefaultTaskSettings, true, () =>
                {
                    if (!Program.IsClosing)
                    {
                        RefreshMenus();
                        SettingManager.SaveApplicationConfigAsync();
                    }
                });
                break;
            case MainFormCommand.HotkeySettings:
                OpenHotkeySettings();
                break;
            case MainFormCommand.DestinationSettings:
                TaskHelpers.OpenUploadersConfigWindow();
                break;
            case MainFormCommand.CustomUploaderSettings:
                TaskHelpers.OpenCustomUploaderSettingsWindow();
                break;
            case MainFormCommand.ScreenshotsFolder:
                TaskHelpers.OpenScreenshotsFolder();
                break;
            case MainFormCommand.History:
                TaskHelpers.OpenHistory();
                break;
            case MainFormCommand.ImageHistory:
                TaskHelpers.OpenImageHistory();
                break;
            case MainFormCommand.DebugLog:
                TaskHelpers.OpenDebugLog();
                break;
            case MainFormCommand.TestImageUpload:
                UploadManager.UploadImage(ShareXResources.Logo);
                break;
            case MainFormCommand.TestTextUpload:
                UploadManager.UploadText(Strings.MainForm_tsmiTestTextUpload_Click_Text_upload_test);
                break;
            case MainFormCommand.TestFileUpload:
                UploadManager.UploadImage(ShareXResources.Logo, ImageDestination.FileUploader, Program.DefaultTaskSettings.FileDestination);
                break;
            case MainFormCommand.TestUrlShortener:
                UploadManager.ShortenURL(Links.Website);
                break;
            case MainFormCommand.TestUrlSharing:
                UploadManager.ShareURL(Links.Website);
                break;
            case MainFormCommand.Donate:
#if STEAM
                URLHelpers.OpenURL(Links.Website);
#else
                URLHelpers.OpenURL(Links.Donate);
#endif
                break;
            case MainFormCommand.X:
                URLHelpers.OpenURL(Links.XFollow);
                break;
            case MainFormCommand.Discord:
                URLHelpers.OpenURL(Links.Discord);
                break;
            case MainFormCommand.About:
                AboutWindowIntegration.Show();
                break;
        }
    });

    private static void OpenHotkeySettings()
    {
        if (Program.HotkeyManager == null)
        {
            return;
        }

        HotkeySettingsIntegration.Show(new HotkeySettingsAvaloniaService(
            Program.HotkeyManager,
            () =>
            {
                if (!Program.IsClosing)
                {
                    RefreshMenus();
                    SettingManager.SaveHotkeysConfigAsync();
                }
            }));
    }

    internal static void ReportVisibility(bool visible) => _isVisible = visible;

    private static void RunOnUiThread(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
        }
        else
        {
            Dispatcher.UIThread.Post(action);
        }
    }
}
