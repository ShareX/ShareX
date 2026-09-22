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

using CommunityToolkit.Mvvm.ComponentModel;
using ShareX.HelpersLib;
using System.Collections.ObjectModel;

namespace ShareX.Tools;

public sealed class RemoteStorageUploadSource
{
    public string FileName { get; }
    public Func<Task<Stream>> OpenReadAsync { get; }

    public RemoteStorageUploadSource(string fileName, Func<Task<Stream>> openReadAsync)
    {
        FileName = fileName;
        OpenReadAsync = openReadAsync;
    }
}

public sealed class RemoteStorageBrowserItemViewModel
{
    public RemoteStorageItem Item { get; }
    public string Name => Item.Name;
    public bool IsFolder => Item.IsFolder;
    public bool IsImage => !IsFolder && FileHelpers.IsImageFile(Name);
    public string Icon => IsFolder ? AvaloniaUI.Theming.LucideIcons.folder :
        IsImage ? AvaloniaUI.Theming.LucideIcons.file_image : AvaloniaUI.Theming.LucideIcons.file;
    public string SizeText => Item.Size.HasValue ? Item.Size.Value.ToSizeString() : string.Empty;
    public string ModifiedText => Item.Modified?.LocalDateTime.ToString("g") ?? string.Empty;

    public RemoteStorageBrowserItemViewModel(RemoteStorageItem item)
    {
        Item = item;
    }
}

public sealed partial class RemoteStorageBrowserViewModel : ViewModelBase, IDisposable
{
    private CancellationTokenSource? _operationCancellation;
    private bool _disposed;

    public IReadOnlyList<IRemoteStorageProvider> Providers { get; }
    public ObservableCollection<RemoteStorageBrowserItemViewModel> Items { get; } = [];
    public Action<string>? ShowErrorRequested { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoUp))]
    [NotifyPropertyChangedFor(nameof(CanUpload))]
    [NotifyPropertyChangedFor(nameof(CanDownload))]
    [NotifyPropertyChangedFor(nameof(CanRename))]
    [NotifyPropertyChangedFor(nameof(CanDelete))]
    [NotifyPropertyChangedFor(nameof(CanView))]
    [NotifyPropertyChangedFor(nameof(CanUseUrl))]
    private IRemoteStorageProvider _selectedProvider;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    [NotifyPropertyChangedFor(nameof(CanDownload))]
    [NotifyPropertyChangedFor(nameof(CanRename))]
    [NotifyPropertyChangedFor(nameof(CanDelete))]
    [NotifyPropertyChangedFor(nameof(CanView))]
    [NotifyPropertyChangedFor(nameof(CanUseUrl))]
    private RemoteStorageBrowserItemViewModel? _selectedItem;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoUp))]
    private string _currentPath = string.Empty;

    [ObservableProperty]
    private string _locationText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyPropertyChangedFor(nameof(CanGoUp))]
    [NotifyPropertyChangedFor(nameof(CanUpload))]
    [NotifyPropertyChangedFor(nameof(CanDownload))]
    [NotifyPropertyChangedFor(nameof(CanRename))]
    [NotifyPropertyChangedFor(nameof(CanDelete))]
    [NotifyPropertyChangedFor(nameof(CanView))]
    [NotifyPropertyChangedFor(nameof(CanUseUrl))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _hasItems;

    [ObservableProperty]
    private string _statusText = string.Empty;

    public bool IsEmpty => !IsBusy && !HasItems;
    public bool HasSelection => SelectedItem != null;
    public bool CanGoUp => !IsBusy && SelectedProvider.GetParentPath(CurrentPath) != null;
    public bool CanUpload => !IsBusy && HasCapability(RemoteStorageProviderCapabilities.Upload);
    public bool CanDownload => !IsBusy && SelectedItem is { IsFolder: false } &&
        HasCapability(RemoteStorageProviderCapabilities.Download);
    public bool CanRename => !IsBusy && HasSelection && HasCapability(RemoteStorageProviderCapabilities.Rename);
    public bool CanDelete => !IsBusy && HasSelection && HasCapability(RemoteStorageProviderCapabilities.Delete);
    public bool CanView => CanDownload && SelectedItem?.IsImage == true;
    public bool CanUseUrl => !IsBusy && SelectedItem is { IsFolder: false } &&
        HasCapability(RemoteStorageProviderCapabilities.Url) &&
        !string.IsNullOrWhiteSpace(SelectedProvider.GetUrl(SelectedItem.Item));

    public RemoteStorageBrowserViewModel(IEnumerable<IRemoteStorageProvider> providers)
    {
        Providers = providers?.ToArray() ?? throw new ArgumentNullException(nameof(providers));
        if (Providers.Count == 0)
        {
            throw new ArgumentException("At least one remote storage provider is required.", nameof(providers));
        }

        _selectedProvider = Providers[0];
        UpdateLocation();
    }

    public Task InitializeAsync() => BrowseAsync(SelectedProvider.RootPath);

    public async Task ChangeProviderAsync(IRemoteStorageProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        if (!Providers.Contains(provider) || ReferenceEquals(provider, SelectedProvider))
        {
            return;
        }

        SelectedProvider = provider;
        Items.Clear();
        HasItems = false;
        SelectedItem = null;
        CurrentPath = provider.RootPath;
        UpdateLocation();
        await BrowseAsync(provider.RootPath);
    }

    public Task RefreshAsync() => BrowseAsync(CurrentPath);

    public Task OpenFolderAsync(RemoteStorageBrowserItemViewModel item)
    {
        return item?.IsFolder == true ? BrowseAsync(item.Item.Path) : Task.CompletedTask;
    }

    public Task GoUpAsync()
    {
        string? parentPath = SelectedProvider.GetParentPath(CurrentPath);
        return parentPath != null ? BrowseAsync(parentPath) : Task.CompletedTask;
    }

    public async Task<bool> DownloadAsync(RemoteStorageBrowserItemViewModel item, Stream destination)
    {
        if (item == null || item.IsFolder)
        {
            return false;
        }

        return await RunOperationAsync(
            cancellationToken => SelectedProvider.DownloadAsync(item.Item, destination, cancellationToken),
            string.Format(Localization.Strings.RemoteStorageBrowser_Downloaded, item.Name));
    }

    public async Task<byte[]?> ReadFileAsync(RemoteStorageBrowserItemViewModel item)
    {
        if (item == null || item.IsFolder)
        {
            return null;
        }

        using MemoryStream stream = new();
        bool success = await RunOperationAsync(
            cancellationToken => SelectedProvider.DownloadAsync(item.Item, stream, cancellationToken),
            string.Format(Localization.Strings.RemoteStorageBrowser_Downloaded, item.Name));
        return success ? stream.ToArray() : null;
    }

    public async Task UploadAsync(IReadOnlyList<RemoteStorageUploadSource> sources)
    {
        if (sources == null || sources.Count == 0)
        {
            return;
        }

        string targetPath = CurrentPath;
        bool success = await RunOperationAsync(async cancellationToken =>
        {
            foreach (RemoteStorageUploadSource source in sources)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await using Stream stream = await source.OpenReadAsync();
                await SelectedProvider.UploadAsync(targetPath, source.FileName, stream, cancellationToken);
            }
        }, string.Format(Localization.Strings.RemoteStorageBrowser_UploadedFiles, sources.Count));

        if (success)
        {
            await BrowseAsync(targetPath);
        }
    }

    public async Task RenameAsync(RemoteStorageBrowserItemViewModel item, string newName)
    {
        if (item == null || string.IsNullOrWhiteSpace(newName) || newName == item.Name)
        {
            return;
        }

        if (Items.Any(x => !ReferenceEquals(x, item) && x.Name.Equals(newName, StringComparison.Ordinal)))
        {
            ShowErrorRequested?.Invoke(string.Format(Localization.Strings.RemoteStorageBrowser_NameAlreadyExists, newName));
            return;
        }

        string targetPath = CurrentPath;
        if (await RunOperationAsync(
            cancellationToken => SelectedProvider.RenameAsync(item.Item, newName, cancellationToken),
            string.Format(Localization.Strings.RemoteStorageBrowser_Renamed, item.Name, newName)))
        {
            await BrowseAsync(targetPath);
        }
    }

    public async Task DeleteAsync(RemoteStorageBrowserItemViewModel item)
    {
        if (item == null)
        {
            return;
        }

        string targetPath = CurrentPath;
        if (await RunOperationAsync(
            cancellationToken => SelectedProvider.DeleteAsync(item.Item, cancellationToken),
            string.Format(Localization.Strings.RemoteStorageBrowser_Deleted, item.Name)))
        {
            await BrowseAsync(targetPath);
        }
    }

    private async Task BrowseAsync(string path)
    {
        if (IsBusy || _disposed)
        {
            return;
        }

        IRemoteStorageProvider provider = SelectedProvider;
        await RunOperationAsync(async cancellationToken =>
        {
            StatusText = Localization.Strings.RemoteStorageBrowser_Loading;
            IReadOnlyList<RemoteStorageItem> items = await provider.GetItemsAsync(path, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (!ReferenceEquals(provider, SelectedProvider))
            {
                return;
            }

            Items.Clear();
            foreach (RemoteStorageItem item in items)
            {
                Items.Add(new RemoteStorageBrowserItemViewModel(item));
            }

            SelectedItem = null;
            CurrentPath = path;
            HasItems = Items.Count > 0;
            UpdateLocation();
            StatusText = string.Format(Localization.Strings.RemoteStorageBrowser_ItemCount, Items.Count);
        });
    }

    private async Task<bool> RunOperationAsync(Func<CancellationToken, Task> operation, string? successStatus = null)
    {
        if (IsBusy || _disposed)
        {
            return false;
        }

        _operationCancellation?.Dispose();
        _operationCancellation = new CancellationTokenSource();
        IsBusy = true;

        try
        {
            await operation(_operationCancellation.Token);
            if (!string.IsNullOrWhiteSpace(successStatus))
            {
                StatusText = successStatus;
            }
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(RemoteStorageBrowserViewModel), "Remote storage operation failed.", ex);
            StatusText = Localization.Strings.RemoteStorageBrowser_OperationFailed;
            ShowErrorRequested?.Invoke(ex.Message);
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool HasCapability(RemoteStorageProviderCapabilities capability) =>
        SelectedProvider.Capabilities.HasFlag(capability);

    private void UpdateLocation()
    {
        LocationText = SelectedProvider.GetDisplayPath(CurrentPath);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _operationCancellation?.Cancel();
        _operationCancellation?.Dispose();
        _operationCancellation = null;
    }
}
