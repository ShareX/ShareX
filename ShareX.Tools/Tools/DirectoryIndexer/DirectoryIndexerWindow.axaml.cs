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
using System.Text;

namespace ShareX.Tools;

public partial class DirectoryIndexerWindow : Window
{
    private readonly DirectoryIndexerViewModel _viewModel;

    public DirectoryIndexerWindow()
        : this(new IndexerSettings())
    {
    }

    public DirectoryIndexerWindow(IndexerSettings settings, Func<string, IndexerOutput, Task>? uploadRequested = null)
    {
        _viewModel = new DirectoryIndexerViewModel(settings);
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _viewModel.SelectFolderRequested = SelectFolderAsync;
        _viewModel.SelectCSSFileRequested = SelectCSSFileAsync;
        _viewModel.SaveRequested = SaveAsync;
        _viewModel.UploadRequested = uploadRequested;
        _viewModel.CloseRequested = Close;

        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        Opened += OnOpened;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        await _viewModel.InitializeAsync();
    }

    private async Task<string?> SelectFolderAsync()
    {
        IReadOnlyList<IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = Localization.Strings.DirectoryIndexerWindow_Select_folder_to_index_dialog,
            AllowMultiple = false
        });

        return folders.FirstOrDefault()?.Path.LocalPath;
    }

    private async Task<string?> SelectCSSFileAsync()
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Localization.Strings.DirectoryIndexerWindow_Select_CSS_file_dialog,
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType(Localization.Strings.DirectoryIndexerWindow_CSS_files) { Patterns = ["*.css"] },
                FilePickerFileTypes.All
            ]
        });

        return files.FirstOrDefault()?.Path.LocalPath;
    }

    private async Task<bool> SaveAsync(string source, string suggestedName, string extension)
    {
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Localization.Strings.DirectoryIndexerWindow_Save_directory_index_dialog,
            SuggestedFileName = suggestedName,
            DefaultExtension = extension,
            FileTypeChoices =
            [
                new FilePickerFileType(string.Format(Localization.Strings.DirectoryIndexerWindow_File_type, extension.ToUpperInvariant())) { Patterns = [$"*.{extension}"] },
                FilePickerFileTypes.All
            ]
        });

        if (file == null)
        {
            return false;
        }

        await using Stream stream = await file.OpenWriteAsync();
        stream.SetLength(0);
        await using StreamWriter writer = new(stream, new UTF8Encoding(true));
        await writer.WriteAsync(source);
        return true;
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = _viewModel.IsBusy || e.DataTransfer.TryGetFiles()?.OfType<IStorageFolder>().Any() != true
            ? DragDropEffects.None
            : DragDropEffects.Copy;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        IStorageFolder? folder = e.DataTransfer.TryGetFiles()?.OfType<IStorageFolder>().FirstOrDefault();
        if (!_viewModel.IsBusy && folder != null)
        {
            _viewModel.SetFolder(folder.Path.LocalPath);
            e.Handled = true;
        }
    }
}
