#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using ShareX.HistoryLib;
using ShareX.UploadersLib;
using System;

namespace ShareX;

internal static class ApplicationState
{
    private static ApplicationConfig? _settings;
    private static UploadersConfig? _uploadersConfig;
    private static HotkeysConfig? _hotkeysConfig;

    internal static ApplicationConfig Settings
    {
        get => _settings ?? throw new InvalidOperationException("Application settings have not been loaded.");
        set => _settings = value;
    }

    internal static ApplicationConfig? SettingsOrNull => _settings;
    internal static TaskSettings DefaultTaskSettings { get; set; } = null!;

    internal static UploadersConfig UploadersConfig
    {
        get => _uploadersConfig ?? throw new InvalidOperationException("Uploader settings have not been loaded.");
        set => _uploadersConfig = value;
    }

    internal static UploadersConfig? UploadersConfigOrNull => _uploadersConfig;

    internal static HotkeysConfig HotkeysConfig
    {
        get => _hotkeysConfig ?? throw new InvalidOperationException("Hotkey settings have not been loaded.");
        set => _hotkeysConfig = value;
    }

    internal static HotkeysConfig? HotkeysConfigOrNull => _hotkeysConfig;
    internal static HistoryManagerSQLite? HistoryManager { get; set; }
    internal static HotkeyManager? HotkeyManager { get; set; }
    internal static WatchFolderManager? WatchFolderManager { get; set; }
    internal static ShareXUpdateManager UpdateManager { get; set; } = null!;
}
