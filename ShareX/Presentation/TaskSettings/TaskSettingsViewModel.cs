#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using ShareX.AvaloniaUI.Controls;
using ShareX.AvaloniaUI.Theming;
using ShareX.Localization;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ShareX;

public sealed class TaskSettingsViewModel : INotifyPropertyChanged
{
    private readonly bool _isDefault;
    private SettingsNavigationItem? _selectedNavigationItem;

    public ObservableCollection<SettingsNavigationItem> NavigationItems { get; } = [];

    public SettingsNavigationItem? SelectedNavigationItem
    {
        get => _selectedNavigationItem;
        set
        {
            if (_isDefault && value is { Children.Count: > 0 } && value.Id is "general" or "upload")
            {
                value = value.Children[0];
            }

            if (ReferenceEquals(_selectedNavigationItem, value))
            {
                return;
            }

            _selectedNavigationItem = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNavigationItem)));
            SelectedPageChanged?.Invoke(value?.Id);
        }
    }

    public event Action<string?>? SelectedPageChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public TaskSettingsViewModel(bool isDefault)
    {
        _isDefault = isDefault;

        if (!isDefault)
        {
            NavigationItems.Add(new SettingsNavigationItem("task", Strings.TaskSettingsWindow_Task, LucideIcons.keyboard));
        }

        NavigationItems.Add(Parent("general", Strings.TaskSettingsWindow_General, LucideIcons.settings,
            Child("general-notifications", Strings.TaskSettingsWindow_Notifications, LucideIcons.bell)));
        NavigationItems.Add(Parent("image", Strings.TaskSettingsWindow_Image, LucideIcons.image,
            Child("image-effects", Strings.TaskSettingsWindow_Effects, LucideIcons.wand_sparkles),
            Child("image-thumbnail", Strings.TaskSettingsWindow_Thumbnail, LucideIcons.images)));
        NavigationItems.Add(Parent("capture", Strings.TaskSettingsWindow_Capture, LucideIcons.camera,
            Child("capture-region", Strings.TaskSettingsWindow_RegionCapture, LucideIcons.crop),
            Child("capture-screen-recorder", Strings.TaskSettingsWindow_ScreenRecorder, LucideIcons.video),
            Child("capture-ocr", Strings.TaskSettingsWindow_OCR, LucideIcons.scan_text)));
        NavigationItems.Add(Parent("upload", Strings.TaskSettingsWindow_Upload, LucideIcons.upload,
            Child("upload-file-naming", Strings.TaskSettingsWindow_FileNaming, LucideIcons.file_pen),
            Child("upload-clipboard", Strings.TaskSettingsWindow_ClipboardUpload, LucideIcons.clipboard),
            Child("upload-filters", Strings.TaskSettingsWindow_UploaderFilters, LucideIcons.filter)));
        NavigationItems.Add(Parent("tools", Strings.TaskSettingsWindow_Tools, LucideIcons.wrench,
            Child("tools-image-editor", Strings.TaskSettingsWindow_ImageEditor, LucideIcons.image)));
        NavigationItems.Add(Parent("actions", Strings.TaskSettingsWindow_Actions, LucideIcons.zap));
        NavigationItems.Add(Parent("watch-folders", Strings.TaskSettingsWindow_WatchFolders, LucideIcons.folder_search));
        NavigationItems.Add(Parent("advanced", Strings.TaskSettingsWindow_Advanced, LucideIcons.sliders_horizontal));

        SelectedNavigationItem = isDefault
            ? NavigationItems.SelectMany(x => x.Children).First(x => x.Id == "general-notifications")
            : NavigationItems[0];
    }

    private static SettingsNavigationItem Parent(string id, string title, string icon, params SettingsNavigationItem[] children) =>
        new(id, title, icon, children: children);

    private static SettingsNavigationItem Child(string id, string title, string icon) => new(id, title, icon);
}
