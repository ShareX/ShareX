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
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class MetadataWindow : Window
{
    private readonly MetadataViewModel _viewModel;

    public MetadataWindow() : this(null, null)
    {
    }

    public MetadataWindow(string? filePath, Action? playNotificationSound)
    {
        _viewModel = new MetadataViewModel(filePath, playNotificationSound);
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _viewModel.SelectFileRequested = SelectFileAsync;
        _viewModel.CopyTextRequested = CopyTextAsync;

        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        await _viewModel.StartAsync();
    }

    private async Task<string?> SelectFileAsync()
    {
        IStorageFolder? startFolder = null;
        string? folder = Path.GetDirectoryName(_viewModel.FilePath);
        if (!string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder))
        {
            startFolder = await StorageProvider.TryGetFolderFromPathAsync(folder);
        }

        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open file",
            AllowMultiple = false,
            SuggestedStartLocation = startFolder
        });
        return files.FirstOrDefault()?.Path.LocalPath;
    }

    private async Task CopyTextAsync(string text)
    {
        if (Clipboard != null)
        {
            await Clipboard.SetTextAsync(text);
        }
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = !_viewModel.IsBusy && e.DataTransfer.Formats.Contains(DataFormat.File)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        if (_viewModel.IsBusy)
        {
            return;
        }

        IStorageFile? file = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().FirstOrDefault();
        if (file != null)
        {
            await _viewModel.OpenFileAsync(file.Path.LocalPath);
            e.Handled = true;
        }
    }

    private void OnUrlPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
            sender is SelectableTextBlock { DataContext: MetadataEntry entry } && entry.HasUrl)
        {
            _viewModel.OpenUrlCommand.Execute(entry.Url);
            e.Handled = true;
        }
    }
}
