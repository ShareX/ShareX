#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using ShareX.AvaloniaUI.Controls;
using ShareX.AvaloniaUI.Theming;
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
            NavigationItems.Add(new SettingsNavigationItem("task", "Task", LucideIcons.keyboard));
        }

        NavigationItems.Add(Parent("general", "General", LucideIcons.settings,
            Child("general-notifications", "Notifications", LucideIcons.bell)));
        NavigationItems.Add(Parent("image", "Image", LucideIcons.image,
            Child("image-effects", "Effects", LucideIcons.wand_sparkles),
            Child("image-thumbnail", "Thumbnail", LucideIcons.images)));
        NavigationItems.Add(Parent("capture", "Capture", LucideIcons.camera,
            Child("capture-region", "Region capture", LucideIcons.crop),
            Child("capture-screen-recorder", "Screen recorder", LucideIcons.video),
            Child("capture-ocr", "OCR", LucideIcons.scan_text)));
        NavigationItems.Add(Parent("upload", "Upload", LucideIcons.upload,
            Child("upload-file-naming", "File naming", LucideIcons.file_pen),
            Child("upload-clipboard", "Clipboard upload", LucideIcons.clipboard),
            Child("upload-filters", "Uploader filters", LucideIcons.filter)));
        NavigationItems.Add(Parent("tools", "Tools", LucideIcons.wrench));
        NavigationItems.Add(Parent("actions", "Actions", LucideIcons.zap));
        NavigationItems.Add(Parent("watch-folders", "Watch folders", LucideIcons.folder_search));
        NavigationItems.Add(Parent("advanced", "Advanced", LucideIcons.sliders_horizontal));

        SelectedNavigationItem = isDefault
            ? NavigationItems.SelectMany(x => x.Children).First(x => x.Id == "general-notifications")
            : NavigationItems[0];
    }

    private static SettingsNavigationItem Parent(string id, string title, string icon, params SettingsNavigationItem[] children) =>
        new(id, title, icon, children: children);

    private static SettingsNavigationItem Child(string id, string title, string icon) => new(id, title, icon);
}
