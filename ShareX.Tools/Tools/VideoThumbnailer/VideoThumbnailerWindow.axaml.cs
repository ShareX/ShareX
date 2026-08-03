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

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class VideoThumbnailerWindow : Window
{
    private readonly VideoThumbnailerViewModel _viewModel;

    public VideoThumbnailerWindow()
        : this(string.Empty, new VideoThumbnailOptions())
    {
    }

    public VideoThumbnailerWindow(string ffmpegPath, VideoThumbnailOptions options,
        Action<IReadOnlyList<VideoThumbnailInfo>>? thumbnailsTaken = null)
    {
        _viewModel = new VideoThumbnailerViewModel(ffmpegPath, options, thumbnailsTaken);
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _viewModel.SelectVideoRequested = SelectVideoAsync;
        _viewModel.SelectOutputFolderRequested = SelectOutputFolderAsync;

        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async Task<string?> SelectVideoAsync()
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Localization.Strings.VideoThumbnailerWindow_Select_video_dialog,
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType(Localization.Strings.VideoThumbnailerWindow_Video_files)
                {
                    Patterns = ["*.mp4", "*.mkv", "*.webm", "*.avi", "*.mov", "*.wmv", "*.m4v", "*.mpg", "*.mpeg", "*.flv"]
                },
                FilePickerFileTypes.All
            ]
        });

        return files.FirstOrDefault()?.Path.LocalPath;
    }

    private async Task<string?> SelectOutputFolderAsync(string? currentFolder)
    {
        IStorageFolder? startFolder = null;
        if (!string.IsNullOrWhiteSpace(currentFolder) && Directory.Exists(currentFolder))
        {
            startFolder = await StorageProvider.TryGetFolderFromPathAsync(currentFolder);
        }

        IReadOnlyList<IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = Localization.Strings.VideoThumbnailerWindow_Select_output_folder_dialog,
            AllowMultiple = false,
            SuggestedStartLocation = startFolder
        });

        return folders.FirstOrDefault()?.Path.LocalPath;
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = _viewModel.IsBusy || !e.DataTransfer.Formats.Contains(DataFormat.File)
            ? DragDropEffects.None
            : DragDropEffects.Copy;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        IStorageFile? file = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().FirstOrDefault();
        if (!_viewModel.IsBusy && file != null)
        {
            _viewModel.LoadVideo(file.Path.LocalPath);
            e.Handled = true;
        }
    }
}
