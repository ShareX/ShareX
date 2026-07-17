#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

#nullable enable

using ShareX.HelpersLib;
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX;

/// <summary>
/// Hidden WinForms host for the process message loop, global hotkeys, and tray icon lifetime.
/// The visible main window is owned by Avalonia.
/// </summary>
public sealed class MainForm : HotkeyForm
{
    private bool _forceClose;

    public bool IsReady { get; private set; }
    internal ITrayIconService TrayIconService { get; }

    public MainForm()
    {
        ShowInTaskbar = false;
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Text = Program.Title;
        ShareXResources.UseWhiteIcon = Program.Settings.UseWhiteShareXIcon;
        Icon = ShareXResources.Icon;
        TrayIconService = new WinFormsTrayIconService(Icon, Program.TitleShort, Program.Settings.ShowTray);

        HandleCreated += MainForm_HandleCreated;
        FormClosed += MainForm_FormClosed;
    }

    private async void MainForm_HandleCreated(object? sender, EventArgs e)
    {
        RunPuushTasks();
        NativeMethods.UseImmersiveDarkMode(Handle, ShareXResources.IsDarkTheme);

        await UpdateControls();

        bool showMainWindow = !(Program.SilentRun || Program.Settings.SilentRun) || !Program.Settings.ShowTray;
        MainWindowIntegration.Initialize(this, showMainWindow);

        if (showMainWindow)
        {
            AfterShownJobs();
        }

        DebugHelper.WriteLine("Startup time: {0} ms", Program.StartTimer.ElapsedMilliseconds);

        await Program.CLI.UseCommandLineArgs();

        if (Program.Settings.ActionsToolbarRunAtStartup)
        {
            TaskHelpers.OpenActionsToolbar();
        }
    }

    public async Task UpdateControls()
    {
        IsReady = false;

        TaskManager.UpdateMainFormTip();
        TaskManager.RecentManager.InitItems();
        TaskbarManager.Enabled = Program.Settings.TaskbarProgressEnabled;

        ApplyApplicationSettings();
        await InitHotkeys();

        IsReady = true;
        MainWindowIntegration.RefreshMenus();
    }

    private void ApplyApplicationSettings()
    {
        HotkeyRepeatLimit = Program.Settings.HotkeyRepeatLimit;

        HelpersOptions.CurrentProxy = Program.Settings.ProxySettings;
        HelpersOptions.URLEncodeIgnoreEmoji = Program.Settings.URLEncodeIgnoreEmoji;
        HelpersOptions.DefaultCopyImageFillBackground = Program.Settings.DefaultClipboardCopyImageFillBackground;
        HelpersOptions.UseAlternativeClipboardCopyImage = Program.Settings.UseAlternativeClipboardCopyImage;
        HelpersOptions.UseAlternativeClipboardGetImage = Program.Settings.UseAlternativeClipboardGetImage;
        HelpersOptions.RotateImageByExifOrientationData = Program.Settings.RotateImageByExifOrientationData;
        HelpersOptions.BrowserPath = Program.Settings.BrowserPath;
        HelpersOptions.RecentColors = Program.Settings.RecentColors;
        HelpersOptions.DevMode = Program.Settings.DevMode;
        Program.UpdateHelpersSpecialFolders();

        TaskManager.RecentManager.MaxCount = Program.Settings.RecentTasksMaxCount;

        ShareXResources.UseWhiteIcon = Program.Settings.UseWhiteShareXIcon;
        Icon = ShareXResources.Icon;
        Text = Program.Title;
        TrayIconService.ToolTipText = Program.TitleShort;
        TrayIconService.Visible = Program.Settings.ShowTray;

        UpdateTheme();
        ConfigureAutoUpdate();

        MainWindowIntegration.SetTitle(Program.Title);
        MainWindowIntegration.SetTrayVisible(Program.Settings.ShowTray);
        MainWindowIntegration.RefreshMenus();
    }

    public void UpdateTheme()
    {
        if (Program.Settings.Themes == null || Program.Settings.Themes.Count == 0)
        {
            Program.Settings.Themes = ShareXTheme.GetDefaultThemes();
            Program.Settings.SelectedTheme = 0;
        }

        if (!Program.Settings.Themes.IsValidIndex(Program.Settings.SelectedTheme))
        {
            Program.Settings.SelectedTheme = 0;
        }

        ShareXResources.Theme = Program.Settings.Themes[Program.Settings.SelectedTheme];

        if (IsHandleCreated)
        {
            NativeMethods.UseImmersiveDarkMode(Handle, ShareXResources.IsDarkTheme);
        }

#pragma warning disable WFO5001
        Application.SetColorMode(ShareXResources.IsDarkTheme ? SystemColorMode.Dark : SystemColorMode.Classic);
#pragma warning restore WFO5001

        using Icon trayIcon = ShareXResources.Icon;
        TrayIconService.SetIcon(trayIcon);
        MainWindowIntegration.RefreshMenus();
    }

    private void ConfigureAutoUpdate()
    {
        Program.UpdateManager.AllowAutoUpdate = !SystemOptions.DisableUpdateCheck && Program.Settings.AutoCheckUpdate;
        Program.UpdateManager.UpdateChannel = Program.Settings.UpdateChannel;
        Program.UpdateManager.ConfigureAutoUpdate();
    }

    private async Task InitHotkeys()
    {
        await Task.Run(SettingManager.WaitHotkeysConfig);

        if (Program.HotkeyManager == null)
        {
            Program.HotkeyManager = new HotkeyManager(this);
            Program.HotkeyManager.HotkeyTrigger += HandleHotkeys;
        }

        Program.HotkeyManager.UpdateHotkeys(Program.HotkeysConfig.Hotkeys, !Program.IgnoreHotkeyWarning);
        DebugHelper.WriteLine("HotkeyManager started.");

        Program.WatchFolderManager ??= new WatchFolderManager();
        Program.WatchFolderManager.UpdateWatchFolders();
        DebugHelper.WriteLine("WatchFolderManager started.");

        MainWindowIntegration.RefreshMenus();
    }

    private async void HandleHotkeys(HotkeySettings hotkeySetting)
    {
        DebugHelper.WriteLine("Hotkey triggered. " + hotkeySetting);
        await TaskHelpers.ExecuteJob(hotkeySetting.TaskSettings);
    }

    private static void RunPuushTasks()
    {
        if (!Program.PuushMode || !Program.Settings.IsFirstTimeRun)
        {
            return;
        }

        using PuushLoginForm puushLoginForm = new();
        if (puushLoginForm.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        Program.DefaultTaskSettings.ImageDestination = ImageDestination.FileUploader;
        Program.DefaultTaskSettings.ImageFileDestination = FileDestination.Puush;
        Program.DefaultTaskSettings.TextDestination = TextDestination.FileUploader;
        Program.DefaultTaskSettings.TextFileDestination = FileDestination.Puush;
        Program.DefaultTaskSettings.FileDestination = FileDestination.Puush;

        SettingManager.WaitUploadersConfig();
        if (Program.UploadersConfig != null)
        {
            Program.UploadersConfig.PuushAPIKey = puushLoginForm.APIKey;
        }
    }

    private static void AfterShownJobs()
    {
        if (Program.SteamFirstTimeConfig)
        {
            using FirstTimeConfigForm firstTimeConfigForm = new();
            firstTimeConfigForm.ShowDialog();
        }
        else
        {
            MainWindowIntegration.Activate();
        }
    }

    public void ForceClose()
    {
        if (ScreenRecordManager.IsRecording)
        {
            if (MessageBox.Show(Resources.ShareXCannotBeClosedWhileScreenRecordingIsActive, "ShareX",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                ScreenRecordManager.AbortRecording();
            }

            return;
        }

        _forceClose = true;
        MainWindowIntegration.Close();
        Close();
    }

    public new bool Visible => MainWindowIntegration.IsVisible;

    public new void Hide()
    {
        if (MainWindowIntegration.IsInitialized)
        {
            MainWindowIntegration.Hide();
        }
        else
        {
            base.Hide();
        }
    }

    public void ForceActivate()
    {
        if (MainWindowIntegration.IsInitialized)
        {
            MainWindowIntegration.Activate();
        }
        else
        {
            FormExtensions.ForceActivate(this);
        }
    }

    internal void SetAvaloniaScreenshotDelay(decimal delay)
    {
        Program.DefaultTaskSettings.CaptureSettings.ScreenshotDelay = delay;
        MainWindowIntegration.RefreshMenus();
    }

    internal void ExecuteAvaloniaMainFormCommand(MainFormCommand command)
    {
        switch (command)
        {
            case MainFormCommand.ApplicationSettings:
                OpenApplicationSettings();
                break;
            case MainFormCommand.TaskSettings:
                OpenTaskSettings();
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
                UploadManager.UploadText(Resources.MainForm_tsmiTestTextUpload_Click_Text_upload_test);
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
    }

    private void OpenApplicationSettings()
    {
        using ApplicationSettingsForm settingsForm = new();
        settingsForm.ShowDialog();

        if (!IsDisposed)
        {
            ApplyApplicationSettings();
            SettingManager.SaveApplicationConfigAsync();
        }
    }

    private void OpenTaskSettings()
    {
        using TaskSettingsForm taskSettingsForm = new(Program.DefaultTaskSettings, true);
        taskSettingsForm.ShowDialog();

        if (!IsDisposed)
        {
            MainWindowIntegration.RefreshMenus();
            SettingManager.SaveApplicationConfigAsync();
        }
    }

    private void OpenHotkeySettings()
    {
        if (Program.HotkeyManager == null)
        {
            return;
        }

        HotkeySettingsIntegration.Show(new HotkeySettingsAvaloniaService(
            Program.HotkeyManager,
            this,
            () =>
            {
                if (!IsDisposed)
                {
                    MainWindowIntegration.RefreshMenus();
                    SettingManager.SaveHotkeysConfigAsync();
                }
            }));
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == (int)WindowsMessages.QUERYENDSESSION)
        {
            EndSessionReasons reason = (EndSessionReasons)m.LParam.ToInt64();
            if (reason.HasFlag(EndSessionReasons.ENDSESSION_CLOSEAPP))
            {
                NativeMethods.RegisterApplicationRestart("-silent", 0);
            }

            m.Result = new IntPtr(1);
        }
        else if (m.Msg == (int)WindowsMessages.ENDSESSION)
        {
            if (m.WParam != IntPtr.Zero)
            {
                Program.CloseSequence();
            }

            m.Result = IntPtr.Zero;
        }
        else
        {
            base.WndProc(ref m);
        }
    }

    protected override void SetVisibleCore(bool value)
    {
        if (value && !IsHandleCreated)
        {
            CreateHandle();
        }

        base.SetVisibleCore(false);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing && Program.Settings.ShowTray && !_forceClose)
        {
            e.Cancel = true;
            MainWindowIntegration.Hide();
            SettingManager.SaveAllSettingsAsync();
        }

        base.OnFormClosing(e);
    }

    private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        MainWindowIntegration.Close();
        TrayIconService.Dispose();
        TaskManager.StopAllTasks();
    }
}
