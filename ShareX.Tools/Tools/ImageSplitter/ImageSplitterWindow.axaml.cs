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

public partial class ImageSplitterWindow : Window
{
    private readonly ImageSplitterViewModel _viewModel;

    public ImageSplitterWindow()
    {
        _viewModel = new ImageSplitterViewModel();
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _viewModel.SelectImageRequested = SelectImageAsync;
        _viewModel.SelectOutputFolderRequested = SelectOutputFolderAsync;

        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async Task<string?> SelectImageAsync()
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose image",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });
        return files.FirstOrDefault()?.Path.LocalPath;
    }

    private async Task<string?> SelectOutputFolderAsync()
    {
        IReadOnlyList<IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Choose output folder",
            AllowMultiple = false,
            SuggestedStartLocation = Directory.Exists(_viewModel.OutputFolderPath)
                ? await StorageProvider.TryGetFolderFromPathAsync(_viewModel.OutputFolderPath)
                : null
        });
        return folders.FirstOrDefault()?.Path.LocalPath;
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Formats.Contains(DataFormat.File)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        string? file = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().FirstOrDefault()?.Path.LocalPath;
        if (!string.IsNullOrWhiteSpace(file))
        {
            _viewModel.SetImageFile(file);
            e.Handled = true;
        }
    }
}
