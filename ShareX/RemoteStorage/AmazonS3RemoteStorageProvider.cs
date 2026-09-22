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
using ShareX.UploadersLib.FileUploaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX;

internal sealed class AmazonS3RemoteStorageProvider : IRemoteStorageProvider
{
    private readonly AmazonS3 _client;
    private readonly string _bucket;
    private readonly bool _allowUpload;

    public string DisplayName => "Amazon S3";
    public string RootDisplayName => _bucket;
    public string RootPath => string.Empty;
    public RemoteStorageProviderCapabilities Capabilities =>
        RemoteStorageProviderCapabilities.Download |
        (_allowUpload ? RemoteStorageProviderCapabilities.Upload : RemoteStorageProviderCapabilities.None) |
        RemoteStorageProviderCapabilities.CreateFolder |
        RemoteStorageProviderCapabilities.Rename |
        RemoteStorageProviderCapabilities.Delete |
        RemoteStorageProviderCapabilities.Url;

    public AmazonS3RemoteStorageProvider(AmazonS3Settings settings, bool allowUpload = true)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _client = new AmazonS3(settings);
        _bucket = settings.Bucket;
        _allowUpload = allowUpload;
    }

    public string GetDisplayPath(string path)
    {
        path = NormalizeDirectoryPath(path);
        return $"s3://{_bucket}/{path}";
    }

    public string? GetParentPath(string path)
    {
        path = NormalizeDirectoryPath(path);
        if (path.Length == 0)
        {
            return null;
        }

        string withoutTrailingSlash = path.TrimEnd('/');
        int separatorIndex = withoutTrailingSlash.LastIndexOf('/');
        return separatorIndex >= 0 ? withoutTrailingSlash[..(separatorIndex + 1)] : RootPath;
    }

    public async Task<IReadOnlyList<RemoteStorageItem>> GetItemsAsync(string path,
        CancellationToken cancellationToken = default)
    {
        string prefix = NormalizeDirectoryPath(path);
        IReadOnlyList<AmazonS3ObjectInfo>? objects = await _client.ListObjectsAsync(prefix, "/", cancellationToken);
        if (objects == null)
        {
            throw CreateOperationException("Listing objects");
        }

        Dictionary<string, RemoteStorageItem> items = new(StringComparer.Ordinal);
        foreach (AmazonS3ObjectInfo item in objects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!item.Key.StartsWith(prefix, StringComparison.Ordinal) || item.Key.Equals(prefix, StringComparison.Ordinal))
            {
                continue;
            }

            string relativePath = item.Key[prefix.Length..];
            string name = relativePath.TrimEnd('/');
            if (name.Length == 0 || name.Contains('/'))
            {
                continue;
            }

            bool isFolder = item.IsFolder || item.Key.EndsWith("/", StringComparison.Ordinal);
            string objectPath = isFolder ? NormalizeDirectoryPath(item.Key) : item.Key;
            RemoteStorageItem storageItem = new(name, objectPath, isFolder,
                isFolder ? null : item.Size, isFolder ? null : item.LastModified);

            if (!items.TryGetValue(objectPath, out RemoteStorageItem? existing) ||
                existing.Modified == null && storageItem.Modified != null)
            {
                items[objectPath] = storageItem;
            }
        }

        return items.Values
            .OrderByDescending(x => x.IsFolder)
            .ThenBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
    }

    public async Task DownloadAsync(RemoteStorageItem item, Stream destination,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(item);
        ArgumentNullException.ThrowIfNull(destination);

        if (!await _client.DownloadObjectAsync(item.Path, destination, cancellationToken))
        {
            throw CreateOperationException("Downloading the object");
        }
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
        string objectKey = NormalizeDirectoryPath(directoryPath) + fileName;

        if (!await _client.UploadObjectAsync(source, objectKey, cancellationToken))
        {
            throw CreateOperationException("Uploading the object");
        }
    }

    public async Task CreateFolderAsync(string directoryPath, string folderName,
        CancellationToken cancellationToken = default)
    {
        ValidateName(folderName);
        string objectKey = NormalizeDirectoryPath(directoryPath) + folderName + "/";
        using MemoryStream stream = new();

        if (!await _client.UploadObjectAsync(stream, objectKey, cancellationToken))
        {
            throw CreateOperationException("Creating the folder");
        }
    }

    public async Task RenameAsync(RemoteStorageItem item, string newName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        ValidateName(newName);

        string parentPath = item.IsFolder
            ? GetParentPath(item.Path) ?? RootPath
            : GetFileParentPath(item.Path);

        if (item.IsFolder)
        {
            string sourcePrefix = NormalizeDirectoryPath(item.Path);
            string destinationPrefix = NormalizeDirectoryPath(parentPath + newName);
            await RenameFolderAsync(sourcePrefix, destinationPrefix, cancellationToken);
        }
        else
        {
            string destinationPath = parentPath + newName;
            if (!await _client.CopyObjectAsync(item.Path, destinationPath, cancellationToken))
            {
                throw CreateOperationException("Renaming the object");
            }

            if (!await _client.DeleteObjectAsync(item.Path, cancellationToken))
            {
                await _client.DeleteObjectAsync(destinationPath, CancellationToken.None);
                throw CreateOperationException("Removing the original object after rename");
            }
        }
    }

    public async Task DeleteAsync(RemoteStorageItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (!item.IsFolder)
        {
            if (!await _client.DeleteObjectAsync(item.Path, cancellationToken))
            {
                throw CreateOperationException("Deleting the object");
            }
            return;
        }

        IReadOnlyList<AmazonS3ObjectInfo>? objects = await _client.ListObjectsAsync(
            NormalizeDirectoryPath(item.Path), null!, cancellationToken);
        if (objects == null)
        {
            throw CreateOperationException("Listing the folder contents");
        }

        foreach (AmazonS3ObjectInfo child in objects.Where(x => !x.IsFolder))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!await _client.DeleteObjectAsync(child.Key, cancellationToken))
            {
                throw CreateOperationException($"Deleting '{child.Key}'");
            }
        }
    }

    public async Task<bool> HasChildrenAsync(RemoteStorageItem folder,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(folder);
        if (!folder.IsFolder)
        {
            throw new ArgumentException("The operation requires a folder.", nameof(folder));
        }

        string prefix = NormalizeDirectoryPath(folder.Path);
        IReadOnlyList<AmazonS3ObjectInfo>? objects = await _client.ListObjectsAsync(
            prefix, null!, cancellationToken, maxKeys: 2);
        if (objects == null)
        {
            throw CreateOperationException("Checking the folder contents");
        }

        return objects.Any(x => !x.Key.Equals(prefix, StringComparison.Ordinal));
    }

    public string? GetUrl(RemoteStorageItem item)
    {
        return item == null || item.IsFolder ? null : _client.GenerateURL(item.Path);
    }

    private async Task RenameFolderAsync(string sourcePrefix, string destinationPrefix,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AmazonS3ObjectInfo>? objects = await _client.ListObjectsAsync(sourcePrefix, null!, cancellationToken);
        if (objects == null)
        {
            throw CreateOperationException("Listing the folder contents");
        }

        AmazonS3ObjectInfo[] sourceObjects = objects.Where(x => !x.IsFolder).ToArray();
        List<string> copiedObjects = new();

        try
        {
            foreach (AmazonS3ObjectInfo source in sourceObjects)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string destination = destinationPrefix + source.Key[sourcePrefix.Length..];
                if (!await _client.CopyObjectAsync(source.Key, destination, cancellationToken))
                {
                    throw CreateOperationException($"Renaming '{source.Key}'");
                }
                copiedObjects.Add(destination);
            }
        }
        catch
        {
            foreach (string copiedObject in copiedObjects)
            {
                await _client.DeleteObjectAsync(copiedObject, CancellationToken.None);
            }
            throw;
        }

        foreach (AmazonS3ObjectInfo source in sourceObjects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!await _client.DeleteObjectAsync(source.Key, cancellationToken))
            {
                throw CreateOperationException($"Removing the original object '{source.Key}' after rename");
            }
        }
    }

    private InvalidOperationException CreateOperationException(string action)
    {
        string details = _client.ToErrorString();
        return new InvalidOperationException(string.IsNullOrWhiteSpace(details) ? $"{action} failed." : details);
    }

    private static string GetFileParentPath(string path)
    {
        int separatorIndex = path.LastIndexOf('/');
        return separatorIndex >= 0 ? path[..(separatorIndex + 1)] : string.Empty;
    }

    private static string NormalizeDirectoryPath(string path)
    {
        path = path?.Replace('\\', '/').Trim('/') ?? string.Empty;
        return path.Length > 0 ? path + "/" : string.Empty;
    }

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
}
