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
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;

namespace ShareX.Tools;

public partial class RemoteStorageBrowserWindow : Window
{
    private readonly RemoteStorageBrowserViewModel _viewModel;
    private readonly RemoteStorageBrowserServices _services;
    private bool _opened;

    public RemoteStorageBrowserWindow()
        : this([new EmptyRemoteStorageProvider()])
    {
    }

    public RemoteStorageBrowserWindow(IEnumerable<IRemoteStorageProvider> providers,
        RemoteStorageBrowserServices? services = null)
    {
        _services = services ?? new RemoteStorageBrowserServices();
        _viewModel = new RemoteStorageBrowserViewModel(providers);
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _viewModel.ShowErrorRequested = ShowError;
        Opened += OnOpened;
        Closed += OnClosed;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        _opened = true;
        await _viewModel.InitializeAsync();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _viewModel.Dispose();
    }

    private async void OnProviderSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_opened && sender is ComboBox { SelectedItem: IRemoteStorageProvider provider })
        {
            await _viewModel.ChangeProviderAsync(provider);
        }
    }

    private async void OnUpClick(object? sender, RoutedEventArgs e) => await _viewModel.GoUpAsync();

    private async void OnRefreshClick(object? sender, RoutedEventArgs e) => await _viewModel.RefreshAsync();

    private async void OnUploadClick(object? sender, RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Localization.Strings.RemoteStorageBrowser_SelectFilesToUpload,
            AllowMultiple = true,
            FileTypeFilter = [FilePickerFileTypes.All]
        });

        RemoteStorageUploadSource[] sources = files.Select(file =>
            new RemoteStorageUploadSource(file.Name, file.OpenReadAsync)).ToArray();
        await _viewModel.UploadAsync(sources);
    }

    private async void OnCreateFolderClick(object? sender, RoutedEventArgs e)
    {
        string dialogTitle = Localization.Strings.RemoteStorageBrowser_CreateFolder.TrimEnd('.', '…');
        string? folderName = InputBoxWindowIntegration.Show(
            dialogTitle,
            Localization.Strings.RemoteStorageBrowser_NewFolder,
            dialogTitle,
            Localization.Strings.RemoteStorageBrowser_Cancel);
        if (folderName != null)
        {
            await _viewModel.CreateFolderAsync(folderName.Trim());
        }
    }

    private async void OnDownloadClick(object? sender, RoutedEventArgs e) => await DownloadSelectedAsync();

    private async Task DownloadSelectedAsync()
    {
        RemoteStorageBrowserItemViewModel? item = _viewModel.SelectedItem;
        if (item == null || item.IsFolder)
        {
            return;
        }

        string extension = Path.GetExtension(item.Name).TrimStart('.');
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Localization.Strings.RemoteStorageBrowser_SaveDownloadedFile,
            SuggestedFileName = item.Name,
            DefaultExtension = string.IsNullOrEmpty(extension) ? null : extension,
            FileTypeChoices = [FilePickerFileTypes.All]
        });
        if (file == null)
        {
            return;
        }

        bool success;
        await using (Stream stream = await file.OpenWriteAsync())
        {
            stream.SetLength(0);
            success = await _viewModel.DownloadAsync(item, stream);
        }

        if (!success)
        {
            try
            {
                await file.DeleteAsync();
            }
            catch
            {
            }
        }
    }

    private async void OnRenameClick(object? sender, RoutedEventArgs e)
    {
        RemoteStorageBrowserItemViewModel? item = _viewModel.SelectedItem;
        if (item == null)
        {
            return;
        }

        string dialogTitle = Localization.Strings.RemoteStorageBrowser_Rename.TrimEnd('.', '…');
        string? newName = InputBoxWindowIntegration.Show(
            dialogTitle,
            item.Name,
            dialogTitle,
            Localization.Strings.RemoteStorageBrowser_Cancel);
        if (newName != null)
        {
            await _viewModel.RenameAsync(item, newName.Trim());
        }
    }

    private async void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        RemoteStorageBrowserItemViewModel? item = _viewModel.SelectedItem;
        if (item == null)
        {
            return;
        }

        string prompt = item.IsFolder
            ? string.Format(Localization.Strings.RemoteStorageBrowser_DeleteFolderConfirmation, item.Name)
            : string.Format(Localization.Strings.RemoteStorageBrowser_DeleteFileConfirmation, item.Name);
        string dialogTitle = Localization.Strings.RemoteStorageBrowser_Delete.TrimEnd('.', '…');
        if (MessageBox.Show(this, prompt, dialogTitle,
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            await _viewModel.DeleteAsync(item);
        }
    }

    private async void OnViewClick(object? sender, RoutedEventArgs e) => await ViewSelectedAsync();

    private async Task ViewSelectedAsync()
    {
        RemoteStorageBrowserItemViewModel? item = _viewModel.SelectedItem;
        if (item?.IsImage != true)
        {
            return;
        }

        byte[]? data = await _viewModel.ReadFileAsync(item);
        if (data is { Length: > 0 })
        {
            ImageViewerWindowIntegration.ShowImage(data, item.Name, this);
        }
    }

    private void OnOpenUrlClick(object? sender, RoutedEventArgs e)
    {
        string? url = GetSelectedUrl();
        if (!string.IsNullOrWhiteSpace(url))
        {
            _services.OpenUrl?.Invoke(url);
        }
    }

    private async void OnCopyUrlClick(object? sender, RoutedEventArgs e)
    {
        string? url = GetSelectedUrl();
        if (!string.IsNullOrWhiteSpace(url) && Clipboard != null)
        {
            await Clipboard.SetTextAsync(url);
        }
    }

    private string? GetSelectedUrl()
    {
        return _viewModel.SelectedItem is { IsFolder: false } item
            ? _viewModel.SelectedProvider.GetUrl(item.Item)
            : null;
    }

    private async void OnItemDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_viewModel.SelectedItem is { IsFolder: true } folder)
        {
            await _viewModel.OpenFolderAsync(folder);
        }
        else if (_viewModel.CanView)
        {
            await ViewSelectedAsync();
        }
    }

    private void OnItemPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control { DataContext: RemoteStorageBrowserItemViewModel item } control &&
            e.GetCurrentPoint(control).Properties.IsRightButtonPressed)
        {
            _viewModel.SelectedItem = item;
        }
    }

    private async void OnFileListKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && _viewModel.SelectedItem is { IsFolder: true } folder)
        {
            await _viewModel.OpenFolderAsync(folder);
            e.Handled = true;
        }
        else if (e.Key == Key.Delete && _viewModel.CanDelete)
        {
            OnDeleteClick(sender, e);
            e.Handled = true;
        }
    }

    private void OnContextMenuOpened(object? sender, RoutedEventArgs e)
    {
        if (sender is not ContextMenu contextMenu)
        {
            return;
        }

        MenuItem[] menuItems = contextMenu.Items.OfType<MenuItem>().ToArray();
        Separator[] separators = contextMenu.Items.OfType<Separator>().ToArray();
        if (menuItems.Length < 8 || separators.Length < 2)
        {
            return;
        }

        MenuItem downloadMenuItem = menuItems[0];
        MenuItem uploadMenuItem = menuItems[1];
        MenuItem createFolderMenuItem = menuItems[2];
        MenuItem renameMenuItem = menuItems[3];
        MenuItem deleteMenuItem = menuItems[4];
        MenuItem viewMenuItem = menuItems[5];
        MenuItem openUrlMenuItem = menuItems[6];
        MenuItem copyUrlMenuItem = menuItems[7];

        downloadMenuItem.IsVisible = _viewModel.CanDownload;
        uploadMenuItem.IsVisible = _viewModel.CanUpload;
        createFolderMenuItem.IsVisible = _viewModel.CanCreateFolder;
        renameMenuItem.IsVisible = _viewModel.CanRename;
        deleteMenuItem.IsVisible = _viewModel.CanDelete;
        separators[0].IsVisible = renameMenuItem.IsVisible || deleteMenuItem.IsVisible;
        viewMenuItem.IsVisible = _viewModel.CanView;
        openUrlMenuItem.IsVisible = _viewModel.CanUseUrl && _services.OpenUrl != null;
        copyUrlMenuItem.IsVisible = _viewModel.CanUseUrl;
        separators[1].IsVisible = viewMenuItem.IsVisible || openUrlMenuItem.IsVisible || copyUrlMenuItem.IsVisible;
    }

    private void ShowError(string message)
    {
        MessageBox.Show(this, message, Localization.Strings.RemoteStorageBrowser_Title,
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private sealed class EmptyRemoteStorageProvider : IRemoteStorageProvider
    {
        public string DisplayName => "Remote storage";
        public string RootDisplayName => DisplayName;
        public string RootPath => string.Empty;
        public RemoteStorageProviderCapabilities Capabilities => RemoteStorageProviderCapabilities.None;

        public string GetDisplayPath(string path) => DisplayName;
        public string? GetParentPath(string path) => null;
        public Task<IReadOnlyList<RemoteStorageItem>> GetItemsAsync(string path,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<RemoteStorageItem>>([]);

        public Task DownloadAsync(RemoteStorageItem item, Stream destination,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UploadAsync(string directoryPath, string fileName, Stream source,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task CreateFolderAsync(string directoryPath, string folderName,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RenameAsync(RemoteStorageItem item, string newName,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DeleteAsync(RemoteStorageItem item,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
        public string? GetUrl(RemoteStorageItem item) => null;
    }
}
