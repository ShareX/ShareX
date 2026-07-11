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

public partial class ImageThumbnailerWindow : Window
{
    private readonly ImageThumbnailerViewModel _viewModel;

    public ImageThumbnailerWindow()
    {
        _viewModel = new ImageThumbnailerViewModel();
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _viewModel.SelectFilesRequested = SelectFilesAsync;
        _viewModel.SelectOutputFolderRequested = SelectOutputFolderAsync;

        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async Task<IReadOnlyList<string>?> SelectFilesAsync()
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Add images",
            AllowMultiple = true,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });
        return files.Select(file => file.Path.LocalPath).ToArray();
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

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItems != null)
        {
            _viewModel.SetSelectedImages(listBox.SelectedItems.OfType<string>());
        }
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Formats.Contains(DataFormat.File)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        string[] files = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>()
            .Select(file => file.Path.LocalPath).ToArray() ?? [];
        if (files.Length > 0)
        {
            _viewModel.AddFiles(files);
            e.Handled = true;
        }
    }
}
