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

using Avalonia.Threading;
using ShareX.AvaloniaUI.Controls;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Properties;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX;

public sealed class ApplicationSettingsViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly DispatcherTimer _saveTimer;
    private SettingsNavigationItem? _selectedNavigationItem;
    private ClipboardFormatItem? _selectedClipboardFormat;
    private UploaderOption<ImageDestination>? _selectedImageUploader;
    private UploaderOption<TextDestination>? _selectedTextUploader;
    private UploaderOption<FileDestination>? _selectedFileUploader;
    private string _personalFolderPath = string.Empty;
    private string _personalFolderPreview = string.Empty;
    private string _screenshotsFolderPreview = string.Empty;
    private bool _startWithWindows;
    private bool _startWithWindowsEnabled;
    private string _startWithWindowsText = string.Empty;
    private bool _shellContextMenu;
    private bool _editWithShareX;
    private bool _sendToMenu;
    private bool _chromeExtensionSupport;
    private bool _firefoxAddonSupport;
    private bool _steamShowInApp;
    private bool _exportSettings = true;
    private bool _exportHistory = true;
    private bool _personalPathDirty;
    private bool _isBusy;
    private bool _restartRequired;
    private string _statusMessage = string.Empty;
    private bool _disposed;

    private ApplicationConfig Settings => Program.Settings;

    public ObservableCollection<SettingsNavigationItem> NavigationItems { get; private set; } = [];
    public ObservableCollection<ClipboardFormatItem> ClipboardFormats { get; private set; } = [];
    public ObservableCollection<UploaderOption<ImageDestination>> SecondaryImageUploaders { get; private set; } = [];
    public ObservableCollection<UploaderOption<TextDestination>> SecondaryTextUploaders { get; private set; } = [];
    public ObservableCollection<UploaderOption<FileDestination>> SecondaryFileUploaders { get; private set; } = [];
    public ObservableCollection<AdvancedSettingItem> AdvancedSettings { get; private set; } = [];
    public ObservableCollection<AdvancedSettingCategory> AdvancedSettingCategories { get; private set; } = [];

    public IReadOnlyList<EnumOption<SupportedLanguage>> LanguageOptions { get; } = CreateEnumOptions<SupportedLanguage>();
    public IReadOnlyList<EnumOption<HotkeyType>> HotkeyTypeOptions { get; } = CreateEnumOptions<HotkeyType>();
    public IReadOnlyList<EnumOption<UpdateChannel>> UpdateChannelOptions { get; } = CreateEnumOptions<UpdateChannel>();
    public IReadOnlyList<EnumOption<ThumbnailTitleLocation>> ThumbnailTitleLocationOptions { get; } = CreateEnumOptions<ThumbnailTitleLocation>();
    public IReadOnlyList<EnumOption<ThumbnailViewClickAction>> ThumbnailClickActionOptions { get; } = CreateEnumOptions<ThumbnailViewClickAction>();
    public IReadOnlyList<EnumOption<ProxyMethod>> ProxyMethodOptions { get; } = CreateEnumOptions<ProxyMethod>();
    public IReadOnlyList<EnumOption<int>> BufferSizeOptions { get; private set; } = [];

    public SettingsNavigationItem? SelectedNavigationItem
    {
        get => _selectedNavigationItem;
        set
        {
            if (!SetField(ref _selectedNavigationItem, value))
            {
                return;
            }

            OnPageChanged();

            if (value?.Id == "integration")
            {
                RefreshStartWithWindows();
            }
        }
    }

    public bool IsGeneralPage => IsPage("general");
    public bool IsIntegrationPage => IsPage("integration");
    public bool IsPathsPage => IsPage("paths");
    public bool IsSettingsPage => IsPage("settings");
    public bool IsMainWindowPage => IsPage("main-window");
    public bool IsClipboardFormatsPage => IsPage("clipboard-formats");
    public bool IsUploadPage => IsPage("upload");
    public bool IsHistoryPage => IsPage("history");
    public bool IsPrintPage => IsPage("print");
    public bool IsProxyPage => IsPage("proxy");
    public bool IsAdvancedPage => IsPage("advanced");

    public bool UpdatesVisible
    {
        get
        {
#if STEAM || MicrosoftStore
            return false;
#else
            return !SystemOptions.DisableUpdateCheck;
#endif
        }
    }

    public bool WindowsIntegrationVisible
    {
        get
        {
#if MicrosoftStore
            return false;
#else
            return true;
#endif
        }
    }

    public bool SteamIntegrationVisible
    {
        get
        {
#if STEAM
            return true;
#else
            return false;
#endif
        }
    }

    public EnumOption<SupportedLanguage>? SelectedLanguage
    {
        get => Find(LanguageOptions, Settings.Language);
        set
        {
            if (value == null || Settings.Language == value.Value)
            {
                return;
            }

            Settings.Language = value.Value;
            MarkChanged();

            if (LanguageHelper.ChangeLanguage(value.Value))
            {
                RestartRequired = true;
            }
        }
    }

    public bool ShowTray
    {
        get => Settings.ShowTray;
        set
        {
            if (SetSetting(Settings.ShowTray, value, x => Settings.ShowTray = x))
            {
                MainWindowIntegration.SetTrayVisible(value);
                OnPropertyChanged(nameof(SilentRunEnabled));
            }
        }
    }

    public bool SilentRunEnabled => ShowTray;
    public bool SilentRun { get => Settings.SilentRun; set => SetSetting(Settings.SilentRun, value, x => Settings.SilentRun = x); }
    public bool TrayIconProgressEnabled { get => Settings.TrayIconProgressEnabled; set => SetSetting(Settings.TrayIconProgressEnabled, value, x => Settings.TrayIconProgressEnabled = x); }

    public bool TaskbarProgressEnabled
    {
        get => Settings.TaskbarProgressEnabled;
        set
        {
            if (SetSetting(Settings.TaskbarProgressEnabled, value, x => Settings.TaskbarProgressEnabled = x))
            {
                TaskbarManager.Enabled = value;
            }
        }
    }

    public bool TaskbarProgressSupported => TaskbarManager.IsPlatformSupported;

    public bool UseWhiteShareXIcon
    {
        get => Settings.UseWhiteShareXIcon;
        set
        {
            if (SetSetting(Settings.UseWhiteShareXIcon, value, x => Settings.UseWhiteShareXIcon = x))
            {
                InvokeOnMainThread(Program.MainForm.UpdateTheme);
            }
        }
    }

    public bool RememberMainFormPosition { get => Settings.RememberMainFormPosition; set => SetSetting(Settings.RememberMainFormPosition, value, x => Settings.RememberMainFormPosition = x); }
    public bool RememberMainFormSize { get => Settings.RememberMainFormSize; set => SetSetting(Settings.RememberMainFormSize, value, x => Settings.RememberMainFormSize = x); }

    public EnumOption<HotkeyType>? SelectedTrayLeftDoubleClickAction
    {
        get => Find(HotkeyTypeOptions, Settings.TrayLeftDoubleClickAction);
        set { if (value != null) SetSetting(Settings.TrayLeftDoubleClickAction, value.Value, x => Settings.TrayLeftDoubleClickAction = x); }
    }

    public EnumOption<HotkeyType>? SelectedTrayLeftClickAction
    {
        get => Find(HotkeyTypeOptions, Settings.TrayLeftClickAction);
        set { if (value != null) SetSetting(Settings.TrayLeftClickAction, value.Value, x => Settings.TrayLeftClickAction = x); }
    }

    public EnumOption<HotkeyType>? SelectedTrayMiddleClickAction
    {
        get => Find(HotkeyTypeOptions, Settings.TrayMiddleClickAction);
        set { if (value != null) SetSetting(Settings.TrayMiddleClickAction, value.Value, x => Settings.TrayMiddleClickAction = x); }
    }

    public bool AutoCheckUpdate
    {
        get => Settings.AutoCheckUpdate;
        set
        {
            if (SetSetting(Settings.AutoCheckUpdate, value, x => Settings.AutoCheckUpdate = x))
            {
                OnPropertyChanged(nameof(UpdateChannelEnabled));
            }
        }
    }

    public bool UpdateChannelEnabled => AutoCheckUpdate;

    public EnumOption<UpdateChannel>? SelectedUpdateChannel
    {
        get => Find(UpdateChannelOptions, Settings.UpdateChannel);
        set { if (value != null) SetSetting(Settings.UpdateChannel, value.Value, x => Settings.UpdateChannel = x); }
    }

    public bool StartWithWindows
    {
        get => _startWithWindows;
        set
        {
            if (!_startWithWindowsEnabled || _startWithWindows == value)
            {
                return;
            }

            try
            {
                InvokeOnMainThread(() => StartupManager.State = value ? StartupState.Enabled : StartupState.Disabled);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                StatusMessage = e.Message;
            }

            RefreshStartWithWindows();
        }
    }

    public bool StartWithWindowsEnabled { get => _startWithWindowsEnabled; private set => SetField(ref _startWithWindowsEnabled, value); }
    public string StartWithWindowsText { get => _startWithWindowsText; private set => SetField(ref _startWithWindowsText, value); }

    public bool ShellContextMenu
    {
        get => _shellContextMenu;
        set
        {
            if (SetField(ref _shellContextMenu, value))
            {
                InvokeOnMainThread(() => IntegrationHelpers.CreateShellContextMenuButton(value));
            }
        }
    }

    public bool EditWithShareX
    {
        get => _editWithShareX;
        set
        {
            if (SetField(ref _editWithShareX, value))
            {
                InvokeOnMainThread(() => IntegrationHelpers.CreateEditShellContextMenuButton(value));
            }
        }
    }

    public bool SendToMenu
    {
        get => _sendToMenu;
        set
        {
            if (SetField(ref _sendToMenu, value))
            {
                InvokeOnMainThread(() => IntegrationHelpers.CreateSendToMenuButton(value));
            }
        }
    }

    public bool ChromeExtensionSupport
    {
        get => _chromeExtensionSupport;
        set
        {
            if (SetField(ref _chromeExtensionSupport, value))
            {
                InvokeOnMainThread(() => IntegrationHelpers.CreateChromeExtensionSupport(value));
            }
        }
    }

    public bool FirefoxAddonSupport
    {
        get => _firefoxAddonSupport;
        set
        {
            if (SetField(ref _firefoxAddonSupport, value))
            {
                InvokeOnMainThread(() => IntegrationHelpers.CreateFirefoxAddonSupport(value));
            }
        }
    }

    public bool SteamShowInApp
    {
        get => _steamShowInApp;
        set
        {
            if (SetField(ref _steamShowInApp, value))
            {
                InvokeOnMainThread(() => IntegrationHelpers.SteamShowInApp(value));
            }
        }
    }

    public string PersonalFolderPath
    {
        get => _personalFolderPath;
        set
        {
            if (!SetField(ref _personalFolderPath, value))
            {
                return;
            }

            UpdatePersonalFolderPreview();
            _personalPathDirty = true;
            MarkChanged();
        }
    }

    public string PersonalFolderPreview { get => _personalFolderPreview; private set => SetField(ref _personalFolderPreview, value); }

    public bool UseCustomScreenshotsPath
    {
        get => Settings.UseCustomScreenshotsPath;
        set
        {
            if (SetSetting(Settings.UseCustomScreenshotsPath, value, x => Settings.UseCustomScreenshotsPath = x))
            {
                UpdateScreenshotsFolderPreview();
            }
        }
    }

    public string CustomScreenshotsPath
    {
        get => Settings.CustomScreenshotsPath;
        set
        {
            string sanitized = FileHelpers.SanitizePath(value);
            if (SetSetting(Settings.CustomScreenshotsPath, sanitized, x => Settings.CustomScreenshotsPath = x))
            {
                UpdateScreenshotsFolderPreview();
            }
        }
    }

    public string SaveImageSubFolderPattern
    {
        get => Settings.SaveImageSubFolderPattern;
        set
        {
            string sanitized = FileHelpers.SanitizePath(value);
            if (SetSetting(Settings.SaveImageSubFolderPattern, sanitized, x => Settings.SaveImageSubFolderPattern = x))
            {
                UpdateScreenshotsFolderPreview();
            }
        }
    }

    public string SaveImageSubFolderPatternWindow
    {
        get => Settings.SaveImageSubFolderPatternWindow;
        set => SetSetting(Settings.SaveImageSubFolderPatternWindow, FileHelpers.SanitizePath(value), x => Settings.SaveImageSubFolderPatternWindow = x);
    }

    public string ScreenshotsFolderPreview { get => _screenshotsFolderPreview; private set => SetField(ref _screenshotsFolderPreview, value); }

    public bool ExportSettings { get => _exportSettings; set { if (SetField(ref _exportSettings, value)) OnPropertyChanged(nameof(CanExport)); } }
    public bool ExportHistory { get => _exportHistory; set { if (SetField(ref _exportHistory, value)) OnPropertyChanged(nameof(CanExport)); } }
    public bool CanExport => !IsBusy && (ExportSettings || ExportHistory);

    public bool AutoCleanupBackupFiles { get => Settings.AutoCleanupBackupFiles; set => SetSetting(Settings.AutoCleanupBackupFiles, value, x => Settings.AutoCleanupBackupFiles = x); }
    public bool AutoCleanupLogFiles { get => Settings.AutoCleanupLogFiles; set => SetSetting(Settings.AutoCleanupLogFiles, value, x => Settings.AutoCleanupLogFiles = x); }
    public decimal CleanupKeepFileCount { get => Settings.CleanupKeepFileCount; set => SetSetting(Settings.CleanupKeepFileCount, decimal.ToInt32(value), x => Settings.CleanupKeepFileCount = x); }

    public bool ShowThumbnailTitle { get => Settings.ShowThumbnailTitle; set => SetSetting(Settings.ShowThumbnailTitle, value, x => Settings.ShowThumbnailTitle = x); }

    public EnumOption<ThumbnailTitleLocation>? SelectedThumbnailTitleLocation
    {
        get => Find(ThumbnailTitleLocationOptions, Settings.ThumbnailTitleLocation);
        set { if (value != null) SetSetting(Settings.ThumbnailTitleLocation, value.Value, x => Settings.ThumbnailTitleLocation = x); }
    }

    public decimal ThumbnailWidth
    {
        get => Settings.ThumbnailSize.Width;
        set => SetSetting(Settings.ThumbnailSize.Width, decimal.ToInt32(value), x => Settings.ThumbnailSize = new Size(x, Settings.ThumbnailSize.Height));
    }

    public decimal ThumbnailHeight
    {
        get => Settings.ThumbnailSize.Height;
        set => SetSetting(Settings.ThumbnailSize.Height, decimal.ToInt32(value), x => Settings.ThumbnailSize = new Size(Settings.ThumbnailSize.Width, x));
    }

    public EnumOption<ThumbnailViewClickAction>? SelectedThumbnailClickAction
    {
        get => Find(ThumbnailClickActionOptions, Settings.ThumbnailClickAction);
        set { if (value != null) SetSetting(Settings.ThumbnailClickAction, value.Value, x => Settings.ThumbnailClickAction = x); }
    }

    public ClipboardFormatItem? SelectedClipboardFormat
    {
        get => _selectedClipboardFormat;
        set
        {
            if (SetField(ref _selectedClipboardFormat, value))
            {
                OnPropertyChanged(nameof(HasSelectedClipboardFormat));
            }
        }
    }

    public bool HasSelectedClipboardFormat => SelectedClipboardFormat != null;

    public decimal UploadLimit { get => Settings.UploadLimit; set => SetSetting(Settings.UploadLimit, decimal.ToInt32(value), x => Settings.UploadLimit = x); }

    public EnumOption<int>? SelectedBufferSize
    {
        get => Find(BufferSizeOptions, Settings.BufferSizePower);
        set { if (value != null) SetSetting(Settings.BufferSizePower, value.Value, x => Settings.BufferSizePower = x); }
    }

    public decimal MaxUploadFailRetry { get => Settings.MaxUploadFailRetry; set => SetSetting(Settings.MaxUploadFailRetry, decimal.ToInt32(value), x => Settings.MaxUploadFailRetry = x); }
    public bool UseSecondaryUploaders { get => Settings.UseSecondaryUploaders; set => SetSetting(Settings.UseSecondaryUploaders, value, x => Settings.UseSecondaryUploaders = x); }

    public UploaderOption<ImageDestination>? SelectedImageUploader { get => _selectedImageUploader; set => SetField(ref _selectedImageUploader, value); }
    public UploaderOption<TextDestination>? SelectedTextUploader { get => _selectedTextUploader; set => SetField(ref _selectedTextUploader, value); }
    public UploaderOption<FileDestination>? SelectedFileUploader { get => _selectedFileUploader; set => SetField(ref _selectedFileUploader, value); }

    public bool HistorySaveTasks { get => Settings.HistorySaveTasks; set => SetSetting(Settings.HistorySaveTasks, value, x => Settings.HistorySaveTasks = x); }
    public bool HistoryCheckURL { get => Settings.HistoryCheckURL; set => SetSetting(Settings.HistoryCheckURL, value, x => Settings.HistoryCheckURL = x); }
    public bool RecentTasksSave { get => Settings.RecentTasksSave; set => SetSetting(Settings.RecentTasksSave, value, x => Settings.RecentTasksSave = x); }
    public decimal RecentTasksMaxCount { get => Settings.RecentTasksMaxCount; set => SetSetting(Settings.RecentTasksMaxCount, decimal.ToInt32(value), x => Settings.RecentTasksMaxCount = x); }
    public bool RecentTasksShowInMainWindow { get => Settings.RecentTasksShowInMainWindow; set => SetSetting(Settings.RecentTasksShowInMainWindow, value, x => Settings.RecentTasksShowInMainWindow = x); }
    public bool RecentTasksShowInTrayMenu { get => Settings.RecentTasksShowInTrayMenu; set => SetSetting(Settings.RecentTasksShowInTrayMenu, value, x => Settings.RecentTasksShowInTrayMenu = x); }
    public bool RecentTasksTrayMenuMostRecentFirst { get => Settings.RecentTasksTrayMenuMostRecentFirst; set => SetSetting(Settings.RecentTasksTrayMenuMostRecentFirst, value, x => Settings.RecentTasksTrayMenuMostRecentFirst = x); }

    public bool DontShowPrintSettingsDialog { get => Settings.DontShowPrintSettingsDialog; set => SetSetting(Settings.DontShowPrintSettingsDialog, value, x => Settings.DontShowPrintSettingsDialog = x); }

    public bool DontShowWindowsPrintDialog
    {
        get => !Settings.PrintSettings.ShowPrintDialog;
        set
        {
            if (SetSetting(Settings.PrintSettings.ShowPrintDialog, !value, x => Settings.PrintSettings.ShowPrintDialog = x))
            {
                OnPropertyChanged(nameof(DefaultPrinterOverrideVisible));
            }
        }
    }

    public bool DefaultPrinterOverrideVisible => !Settings.PrintSettings.ShowPrintDialog;
    public string DefaultPrinterOverride { get => Settings.PrintSettings.DefaultPrinterOverride; set => SetSetting(Settings.PrintSettings.DefaultPrinterOverride, value, x => Settings.PrintSettings.DefaultPrinterOverride = x); }

    public EnumOption<ProxyMethod>? SelectedProxyMethod
    {
        get => Find(ProxyMethodOptions, Settings.ProxySettings.ProxyMethod);
        set
        {
            if (value == null || !SetSetting(Settings.ProxySettings.ProxyMethod, value.Value, x => Settings.ProxySettings.ProxyMethod = x))
            {
                return;
            }

            if (value.Value == ProxyMethod.Automatic)
            {
                Settings.ProxySettings.IsValidProxy();
                OnPropertyChanged(nameof(ProxyHost));
                OnPropertyChanged(nameof(ProxyPort));
            }

            OnPropertyChanged(nameof(ProxyCredentialsEnabled));
            OnPropertyChanged(nameof(ManualProxyEnabled));
        }
    }

    public bool ProxyCredentialsEnabled => Settings.ProxySettings.ProxyMethod != ProxyMethod.None;
    public bool ManualProxyEnabled => Settings.ProxySettings.ProxyMethod == ProxyMethod.Manual;
    public string ProxyUsername { get => Settings.ProxySettings.Username ?? string.Empty; set => SetSetting(Settings.ProxySettings.Username, value, x => Settings.ProxySettings.Username = x); }
    public string ProxyPassword { get => Settings.ProxySettings.Password ?? string.Empty; set => SetSetting(Settings.ProxySettings.Password, value, x => Settings.ProxySettings.Password = x); }
    public string ProxyHost { get => Settings.ProxySettings.Host ?? string.Empty; set => SetSetting(Settings.ProxySettings.Host, value, x => Settings.ProxySettings.Host = x); }
    public decimal ProxyPort { get => Settings.ProxySettings.Port; set => SetSetting(Settings.ProxySettings.Port, decimal.ToInt32(value), x => Settings.ProxySettings.Port = x); }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetField(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(CanExport));
            }
        }
    }

    public bool RestartRequired { get => _restartRequired; private set => SetField(ref _restartRequired, value); }
    public string StatusMessage { get => _statusMessage; private set { if (SetField(ref _statusMessage, value)) OnPropertyChanged(nameof(HasStatusMessage)); } }
    public bool HasStatusMessage => !string.IsNullOrWhiteSpace(StatusMessage);

    public ApplicationSettingsViewModel()
    {
        _saveTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(650) };
        _saveTimer.Tick += OnSaveTimerTick;

        Reload();
    }

    public void AddClipboardFormat()
    {
        ClipboardFormat format = new("New format", "$url");
        Settings.ClipboardContentFormats.Add(format);
        ClipboardFormatItem item = new(format, MarkChanged);
        ClipboardFormats.Add(item);
        SelectedClipboardFormat = item;
        MarkChanged();
    }

    public void RemoveSelectedClipboardFormat()
    {
        if (SelectedClipboardFormat == null)
        {
            return;
        }

        int index = ClipboardFormats.IndexOf(SelectedClipboardFormat);
        Settings.ClipboardContentFormats.Remove(SelectedClipboardFormat.Model);
        ClipboardFormats.Remove(SelectedClipboardFormat);
        SelectedClipboardFormat = ClipboardFormats.Count == 0 ? null : ClipboardFormats[Math.Min(index, ClipboardFormats.Count - 1)];
        MarkChanged();
    }

    public void ResetThumbnailSize()
    {
        Settings.ThumbnailSize = new Size(200, 150);
        OnPropertyChanged(nameof(ThumbnailWidth));
        OnPropertyChanged(nameof(ThumbnailHeight));
        MarkChanged();
    }

    public void MoveSelectedImageUploader(int offset) => MoveUploader(SecondaryImageUploaders, SelectedImageUploader, offset, list => Settings.SecondaryImageUploaders = list);
    public void MoveSelectedTextUploader(int offset) => MoveUploader(SecondaryTextUploaders, SelectedTextUploader, offset, list => Settings.SecondaryTextUploaders = list);
    public void MoveSelectedFileUploader(int offset) => MoveUploader(SecondaryFileUploaders, SelectedFileUploader, offset, list => Settings.SecondaryFileUploaders = list);

    public void EditQuickTaskMenu() => InvokeOnMainThread(() =>
    {
        using QuickTaskMenuEditorForm form = new();
        form.ShowDialog(Program.MainForm);
    });

    public async Task CheckDevBuildAsync()
    {
        IsBusy = true;
        try
        {
            await TaskHelpers.DownloadDevBuild();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void OpenChromeExtensionPage() => URLHelpers.OpenURL("https://chrome.google.com/webstore/detail/sharex/nlkoigbdolhchiicbonbihbphgamnaoc");
    public void OpenFirefoxAddonPage() => URLHelpers.OpenURL("https://addons.mozilla.org/en-US/firefox/addon/sharex/");
    public void OpenPersonalFolder() => FileHelpers.OpenFolder(PersonalFolderPreview);
    public void OpenScreenshotsFolder() => FileHelpers.OpenFolder(ScreenshotsFolderPreview);

    public void ShowImagePrintSettings()
    {
        InvokeOnMainThread(() =>
        {
            using Image image = TaskHelpers.GetScreenshot().CaptureActiveMonitor();
            using PrintForm form = new(image, Settings.PrintSettings, true);
            form.ShowDialog(Program.MainForm);
        });
        MarkChanged();
    }

    public async Task ExportAsync(string path)
    {
        IsBusy = true;
        StatusMessage = "Exporting backup...";

        try
        {
            bool exportSettings = ExportSettings;
            bool exportHistory = ExportHistory;
            bool result = await Task.Run(() =>
            {
                SettingManager.SaveAllSettings();
                return SettingManager.Export(path, exportSettings, exportHistory);
            });
            StatusMessage = result ? $"Backup exported to {path}" : "Backup export failed.";
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
            StatusMessage = e.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task ImportAsync(string path)
    {
        IsBusy = true;
        StatusMessage = "Importing backup...";
        _saveTimer.Stop();

        try
        {
            bool result = await Task.Run(() =>
            {
                if (!SettingManager.Import(path))
                {
                    return false;
                }

                SettingManager.LoadAllSettings();
                return true;
            });

            if (result)
            {
                LanguageHelper.ChangeLanguage(Settings.Language);
                Reload();
                await UpdateMainFormAsync();
                StatusMessage = $"Backup imported from {path}";
            }
            else
            {
                StatusMessage = "Backup import failed.";
            }
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
            StatusMessage = e.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task ResetAsync()
    {
        bool confirmed = InvokeOnMainThread(() => MessageBox.Show(
            Resources.ApplicationSettingsForm_btnResetSettings_Click_WouldYouLikeToResetShareXSettings,
            "ShareX - " + Resources.Confirmation,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Exclamation) == DialogResult.Yes);

        if (!confirmed)
        {
            return;
        }

        IsBusy = true;
        _saveTimer.Stop();

        try
        {
            InvokeOnMainThread(() =>
            {
                SettingManager.ResetSettings();
                SettingManager.SaveAllSettings();
            });
            LanguageHelper.ChangeLanguage(Settings.Language);
            Reload();
            await UpdateMainFormAsync();
            StatusMessage = "Settings reset.";
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
            StatusMessage = e.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void Restart() => InvokeOnMainThread(() => Program.Restart());

    public void Reload()
    {
        _personalFolderPath = Program.ReadPersonalPathConfig();
        UpdatePersonalFolderPreview();
        UpdateScreenshotsFolderPreview();
        RefreshIntegrations();
        RefreshStartWithWindows();

        ClipboardFormats = new ObservableCollection<ClipboardFormatItem>(Settings.ClipboardContentFormats
            .Select(x => new ClipboardFormatItem(x, MarkChanged)));
        SelectedClipboardFormat = ClipboardFormats.FirstOrDefault();

        SecondaryImageUploaders = CreateUploaderOptions(Settings.SecondaryImageUploaders);
        SecondaryTextUploaders = CreateUploaderOptions(Settings.SecondaryTextUploaders);
        SecondaryFileUploaders = CreateUploaderOptions(Settings.SecondaryFileUploaders);
        SyncUploaderSettings();

        AdvancedSettings = new ObservableCollection<AdvancedSettingItem>(
            TypeDescriptor.GetProperties(Settings).Cast<PropertyDescriptor>()
                .Where(x => x.IsBrowsable && (x.PropertyType == typeof(bool) || x.PropertyType == typeof(int) || x.PropertyType == typeof(string) || x.PropertyType.IsEnum))
                .OrderBy(x => x.Category)
                .ThenBy(x => x.DisplayName)
                .Select(x => new AdvancedSettingItem(Settings, x, OnAdvancedSettingChanged)));

        AdvancedSettingCategories = new ObservableCollection<AdvancedSettingCategory>(AdvancedSettings
            .GroupBy(x => x.Category)
            .Select(group => new AdvancedSettingCategory(group.Key, new ObservableCollection<AdvancedSettingItem>(group))));

        RefreshBufferSizeOptions();

        NavigationItems = CreateNavigationItems();
        SelectedNavigationItem = NavigationItems.FirstOrDefault();

        OnPropertyChanged(string.Empty);
    }

    private ObservableCollection<SettingsNavigationItem> CreateNavigationItems()
    {
        return
        [
            Nav("general", "General", LucideIcons.settings),
            Nav("integration", "Integration", LucideIcons.plug),
            Nav("paths", "Paths", LucideIcons.folder),
            Nav("settings", "Settings", LucideIcons.database_backup),
            Nav("main-window", "Main window", LucideIcons.monitor),
            Nav("clipboard-formats", "Clipboard formats", LucideIcons.clipboard_list),
            Nav("upload", "Upload", LucideIcons.upload),
            Nav("history", "History", LucideIcons.history),
            Nav("print", "Print", LucideIcons.printer),
            Nav("proxy", "Proxy", LucideIcons.network),
            Nav("advanced", "Advanced", LucideIcons.sliders_horizontal)
        ];
    }

    private static SettingsNavigationItem Nav(string id, string title, string icon) => new(id, title, icon);

    private void RefreshIntegrations()
    {
#if !MicrosoftStore
        _shellContextMenu = IntegrationHelpers.CheckShellContextMenuButton();
        _editWithShareX = IntegrationHelpers.CheckEditShellContextMenuButton();
        _sendToMenu = IntegrationHelpers.CheckSendToMenuButton();
        _chromeExtensionSupport = IntegrationHelpers.CheckChromeExtensionSupport();
        _firefoxAddonSupport = IntegrationHelpers.CheckFirefoxAddonSupport();
#endif
#if STEAM
        _steamShowInApp = IntegrationHelpers.CheckSteamShowInApp();
#endif
    }

    private void RefreshStartWithWindows()
    {
        StartWithWindowsText = Resources.ApplicationSettingsForm_cbStartWithWindows_Text;
        StartWithWindowsEnabled = false;

        try
        {
            StartupState state = InvokeOnMainThread(() => StartupManager.State);
            _startWithWindows = state == StartupState.Enabled || state == StartupState.EnabledByPolicy;
            OnPropertyChanged(nameof(StartWithWindows));

            if (state == StartupState.DisabledByUser)
            {
                StartWithWindowsText = Resources.ApplicationSettingsForm_cbStartWithWindows_DisabledByUser_Text;
            }
            else if (state == StartupState.DisabledByPolicy)
            {
                StartWithWindowsText = Resources.ApplicationSettingsForm_cbStartWithWindows_DisabledByPolicy_Text;
            }
            else if (state == StartupState.EnabledByPolicy)
            {
                StartWithWindowsText = Resources.ApplicationSettingsForm_cbStartWithWindows_EnabledByPolicy_Text;
            }
            else
            {
                StartWithWindowsEnabled = true;
            }
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
            StatusMessage = e.Message;
        }
    }

    private void UpdatePersonalFolderPreview()
    {
        try
        {
            string path = FileHelpers.SanitizePath(_personalFolderPath);
            if (string.IsNullOrEmpty(path))
            {
                path = Program.Portable ? Program.PortablePersonalFolder : Program.DefaultPersonalFolder;
            }
            else
            {
                path = FileHelpers.GetAbsolutePath(path);
            }

            PersonalFolderPreview = path;
        }
        catch (Exception e)
        {
            PersonalFolderPreview = "Error: " + e.Message;
        }
    }

    private void UpdateScreenshotsFolderPreview()
    {
        try
        {
            ScreenshotsFolderPreview = TaskHelpers.GetScreenshotsFolder();
        }
        catch (Exception e)
        {
            ScreenshotsFolderPreview = "Error: " + e.Message;
        }
    }

    private void SyncUploaderSettings()
    {
        Settings.SecondaryImageUploaders = SecondaryImageUploaders.Select(x => x.Value).ToList();
        Settings.SecondaryTextUploaders = SecondaryTextUploaders.Select(x => x.Value).ToList();
        Settings.SecondaryFileUploaders = SecondaryFileUploaders.Select(x => x.Value).ToList();
    }

    private void MoveUploader<T>(ObservableCollection<UploaderOption<T>> items, UploaderOption<T>? selected, int offset, Action<List<T>> update)
        where T : struct, Enum
    {
        if (selected == null || items.Count < 2)
        {
            return;
        }

        int oldIndex = items.IndexOf(selected);
        int newIndex = Math.Clamp(oldIndex + offset, 0, items.Count - 1);
        if (oldIndex == newIndex)
        {
            return;
        }

        items.Move(oldIndex, newIndex);
        update(items.Select(x => x.Value).ToList());
        MarkChanged();
    }

    private static ObservableCollection<UploaderOption<T>> CreateUploaderOptions<T>(List<T> configured) where T : struct, Enum
    {
        IReadOnlyList<T> values = Helpers.GetEnums<T>();
        List<T> normalized = configured.Where(values.Contains).Distinct().ToList();
        normalized.AddRange(values.Where(x => !normalized.Contains(x)));
        return new ObservableCollection<UploaderOption<T>>(normalized.Select(x => new UploaderOption<T>(x, x.GetLocalizedDescription())));
    }

    private bool IsPage(string id) => SelectedNavigationItem?.Id == id;

    private void OnPageChanged()
    {
        OnPropertyChanged(nameof(IsGeneralPage));
        OnPropertyChanged(nameof(IsIntegrationPage));
        OnPropertyChanged(nameof(IsPathsPage));
        OnPropertyChanged(nameof(IsSettingsPage));
        OnPropertyChanged(nameof(IsMainWindowPage));
        OnPropertyChanged(nameof(IsClipboardFormatsPage));
        OnPropertyChanged(nameof(IsUploadPage));
        OnPropertyChanged(nameof(IsHistoryPage));
        OnPropertyChanged(nameof(IsPrintPage));
        OnPropertyChanged(nameof(IsProxyPage));
        OnPropertyChanged(nameof(IsAdvancedPage));
    }

    private bool SetSetting<T>(T current, T value, Action<T> setter, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(current, value))
        {
            return false;
        }

        setter(value);
        OnPropertyChanged(propertyName);
        MarkChanged();
        return true;
    }

    private void MarkChanged()
    {
        _saveTimer.Stop();
        _saveTimer.Start();
    }

    private void OnAdvancedSettingChanged()
    {
        RefreshBufferSizeOptions();
        MarkChanged();
    }

    private void RefreshBufferSizeOptions()
    {
        BufferSizeOptions = Enumerable.Range(0, 14)
            .Select(power => new EnumOption<int>(power, ((long)(Math.Pow(2, power) * 1024)).ToSizeString(Settings.BinaryUnits, 0)))
            .ToArray();
        OnPropertyChanged(nameof(BufferSizeOptions));
        OnPropertyChanged(nameof(SelectedBufferSize));
    }

    private void OnSaveTimerTick(object? sender, EventArgs e)
    {
        _saveTimer.Stop();
        FlushPersonalPath();
        InvokeOnMainThread(Program.MainForm.ApplyApplicationSettings);
        SettingManager.SaveApplicationConfigAsync();
    }

    private void FlushPersonalPath()
    {
        if (!_personalPathDirty)
        {
            return;
        }

        _personalPathDirty = false;

        try
        {
            bool changed = InvokeOnMainThread(() => Program.WritePersonalPathConfig(FileHelpers.SanitizePath(_personalFolderPath)));
            if (changed)
            {
                RestartRequired = true;
            }
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
            StatusMessage = e.Message;
        }
    }

    private async Task UpdateMainFormAsync()
    {
        Task updateTask = InvokeOnMainThread(() => Program.MainForm.UpdateControls());
        await updateTask;
    }

    private static IReadOnlyList<EnumOption<T>> CreateEnumOptions<T>() where T : struct, Enum =>
        Helpers.GetEnums<T>().Select(x => new EnumOption<T>(x, x.GetLocalizedDescription())).ToArray();

    private static EnumOption<T>? Find<T>(IReadOnlyList<EnumOption<T>> options, T value) =>
        options.FirstOrDefault(x => EqualityComparer<T>.Default.Equals(x.Value, value));

    private static void InvokeOnMainThread(Action action)
    {
        if (Program.MainForm == null || Program.MainForm.IsDisposed)
        {
            return;
        }

        if (Program.MainForm.InvokeRequired)
        {
            Program.MainForm.Invoke(action);
        }
        else
        {
            action();
        }
    }

    private static T InvokeOnMainThread<T>(Func<T> action)
    {
        if (Program.MainForm == null || Program.MainForm.IsDisposed)
        {
            return action();
        }

        if (Program.MainForm.InvokeRequired)
        {
            return (T)Program.MainForm.Invoke(action);
        }

        return action();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _saveTimer.Stop();
        FlushPersonalPath();
        InvokeOnMainThread(Program.MainForm.ApplyApplicationSettings);
        SettingManager.SaveApplicationConfigAsync();
    }
}
