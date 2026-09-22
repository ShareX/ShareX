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

#nullable enable

using ShareX.Tools;
using ShareX.UploadersLib;
using ShareX.UploadersLib.FileUploaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX;

internal sealed class SFTPRemoteStorageProvider : IRemoteStorageProvider, IDisposable
{
    private readonly FTPAccount _account;
    private readonly SFTP _client;
    private readonly bool _allowUpload;

    public string DisplayName { get; }
    public string RootDisplayName { get; }
    public string RootPath => string.Empty;
    public RemoteStorageProviderCapabilities Capabilities =>
        RemoteStorageProviderCapabilities.Download |
        (_allowUpload ? RemoteStorageProviderCapabilities.Upload : RemoteStorageProviderCapabilities.None) |
        RemoteStorageProviderCapabilities.CreateFolder |
        RemoteStorageProviderCapabilities.Rename |
        RemoteStorageProviderCapabilities.Delete |
        RemoteStorageProviderCapabilities.Url;

    public SFTPRemoteStorageProvider(FTPAccount account, bool allowUpload = true)
    {
        ArgumentNullException.ThrowIfNull(account);

        _account = account;
        _client = new SFTP(account);
        _allowUpload = allowUpload;

        string accountName = string.IsNullOrWhiteSpace(account.Name)
            ? $"{account.Host}:{account.Port}"
            : $"{account.Name} ({account.Host}:{account.Port})";
        DisplayName = $"SFTP - {accountName}";
        RootDisplayName = accountName;
    }

    public string GetDisplayPath(string path)
    {
        string address = _account.FTPAddress.TrimEnd('/');
        path = NormalizePath(path);
        return path.Length > 0 ? $"{address}/{path}" : $"{address}/";
    }

    public string? GetParentPath(string path)
    {
        path = NormalizePath(path);
        if (path.Length == 0)
        {
            return null;
        }

        int separatorIndex = path.LastIndexOf('/');
        return separatorIndex >= 0 ? path[..separatorIndex] : RootPath;
    }

    public async Task<IReadOnlyList<RemoteStorageItem>> GetItemsAsync(string path,
        CancellationToken cancellationToken = default)
    {
        string directoryPath = NormalizePath(path);
        IReadOnlyList<SFTPFileInfo> files = await _client.ListDirectoryAsync(ToRemotePath(directoryPath),
            cancellationToken);

        return files
            .Where(IsBrowsableItem)
            .Select(file => new RemoteStorageItem(
                file.Name,
                CombinePath(directoryPath, file.Name),
                file.IsDirectory && !file.IsSymbolicLink,
                file.IsDirectory ? null : file.Length,
                ToDateTimeOffset(file.LastWriteTimeUtc)))
            .OrderByDescending(item => item.IsFolder)
            .ThenBy(item => item.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
    }

    public async Task DownloadAsync(RemoteStorageItem item, Stream destination,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(item);
        ArgumentNullException.ThrowIfNull(destination);
        await _client.DownloadFileAsync(ToRemotePath(item.Path), destination, cancellationToken);
    }

    public async Task UploadAsync(string directoryPath, string fileName, Stream source,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!_allowUpload)
        {
            throw new InvalidOperationException("Uploading is disabled.");
        }

        ValidateName(fileName);
        string remotePath = CombinePath(NormalizePath(directoryPath), fileName);
        await _client.UploadFileAsync(source, ToRemotePath(remotePath), cancellationToken);
    }

    public async Task CreateFolderAsync(string directoryPath, string folderName,
        CancellationToken cancellationToken = default)
    {
        ValidateName(folderName);
        string remotePath = CombinePath(NormalizePath(directoryPath), folderName);
        await _client.CreateDirectoryAsync(ToRemotePath(remotePath), cancellationToken);
    }

    public async Task RenameAsync(RemoteStorageItem item, string newName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        ValidateName(newName);

        string parentPath = GetParentPath(item.Path) ?? RootPath;
        string destinationPath = CombinePath(parentPath, newName);
        await _client.RenameAsync(ToRemotePath(item.Path), ToRemotePath(destinationPath), cancellationToken);
    }

    public async Task DeleteAsync(RemoteStorageItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.IsFolder)
        {
            await DeleteDirectoryAsync(NormalizePath(item.Path), cancellationToken);
        }
        else
        {
            await _client.DeleteFileAsync(ToRemotePath(item.Path), cancellationToken);
        }
    }

    public Task<bool> HasChildrenAsync(RemoteStorageItem folder,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(folder);
        if (!folder.IsFolder)
        {
            throw new ArgumentException("The operation requires a folder.", nameof(folder));
        }

        return _client.DirectoryHasItemsAsync(ToRemotePath(folder.Path), cancellationToken);
    }

    public string? GetUrl(RemoteStorageItem item)
    {
        if (item == null || item.IsFolder)
        {
            return null;
        }

        try
        {
            string parentPath = GetParentPath(item.Path) ?? RootPath;
            return _account.GetUriPath(item.Name, parentPath);
        }
        catch
        {
            return null;
        }
    }

    private async Task DeleteDirectoryAsync(string directoryPath, CancellationToken cancellationToken)
    {
        IReadOnlyList<SFTPFileInfo> files = await _client.ListDirectoryAsync(ToRemotePath(directoryPath),
            cancellationToken);

        foreach (SFTPFileInfo file in files.Where(IsBrowsableItem))
        {
            cancellationToken.ThrowIfCancellationRequested();
            string childPath = CombinePath(directoryPath, file.Name);

            if (file.IsDirectory && !file.IsSymbolicLink)
            {
                await DeleteDirectoryAsync(childPath, cancellationToken);
            }
            else
            {
                await _client.DeleteFileAsync(ToRemotePath(childPath), cancellationToken);
            }
        }

        await _client.DeleteDirectoryAsync(ToRemotePath(directoryPath), cancellationToken);
    }

    private static bool IsBrowsableItem(SFTPFileInfo file) => file.Name is not "." and not "..";

    private static DateTimeOffset? ToDateTimeOffset(DateTime value)
    {
        return value == default ? null : new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }

    private static string ToRemotePath(string path) => path.Length > 0 ? path : ".";

    private static string CombinePath(string directoryPath, string name)
    {
        directoryPath = NormalizePath(directoryPath);
        return directoryPath.Length > 0 ? $"{directoryPath}/{name}" : name;
    }

    private static string NormalizePath(string? path) => path?.Trim('/') ?? string.Empty;

    private static void ValidateFile(RemoteStorageItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (item.IsFolder)
        {
            throw new ArgumentException("The operation requires a file.", nameof(item));
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name is "." or ".." || name.Contains('/') || name.Contains('\\'))
        {
            throw new ArgumentException("The name is not valid.", nameof(name));
        }
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
