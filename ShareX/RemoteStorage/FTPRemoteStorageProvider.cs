#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

#nullable enable

using FluentFTP;
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

internal sealed class FTPRemoteStorageProvider : IRemoteStorageProvider, IDisposable
{
    private readonly FTPAccount _account;
    private readonly FTP _client;
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

    public FTPRemoteStorageProvider(FTPAccount account, bool allowUpload = true)
    {
        ArgumentNullException.ThrowIfNull(account);
        if (account.Protocol is not (FTPProtocol.FTP or FTPProtocol.FTPS))
        {
            throw new ArgumentException("The account must use FTP or FTPS.", nameof(account));
        }

        _account = account;
        _client = new FTP(account);
        _allowUpload = allowUpload;

        string protocolName = account.Protocol == FTPProtocol.FTPS ? "FTPS" : "FTP";
        string accountName = string.IsNullOrWhiteSpace(account.Name)
            ? $"{account.Host}:{account.Port}"
            : $"{account.Name} ({account.Host}:{account.Port})";
        DisplayName = $"{protocolName} - {accountName}";
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

    public Task<IReadOnlyList<RemoteStorageItem>> GetItemsAsync(string path,
        CancellationToken cancellationToken = default)
    {
        string directoryPath = NormalizePath(path);
        return RunAsync<IReadOnlyList<RemoteStorageItem>>(() =>
        {
            EnsureConnected();
            return _client.GetListing(ToRemotePath(directoryPath))
                .Where(IsBrowsableItem)
                .Select(file => new RemoteStorageItem(
                    file.Name,
                    CombinePath(directoryPath, file.Name),
                    file.Type == FtpObjectType.Directory,
                    file.Type == FtpObjectType.File && file.Size >= 0 ? file.Size : null,
                    ToDateTimeOffset(file.Modified)))
                .OrderByDescending(item => item.IsFolder)
                .ThenBy(item => item.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
        }, cancellationToken);
    }

    public Task DownloadAsync(RemoteStorageItem item, Stream destination,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(item);
        ArgumentNullException.ThrowIfNull(destination);

        return RunAsync(() =>
        {
            EnsureConnected();
            _client.DownloadFile(ToRemotePath(item.Path), destination);
        }, cancellationToken);
    }

    public Task UploadAsync(string directoryPath, string fileName, Stream source,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!_allowUpload)
        {
            throw new InvalidOperationException("Uploading is disabled.");
        }

        ValidateName(fileName);
        string remotePath = CombinePath(NormalizePath(directoryPath), fileName);
        return RunAsync(() =>
        {
            EnsureConnected();
            if (!_client.UploadData(source, ToRemotePath(remotePath)))
            {
                throw CreateOperationException("Uploading the file");
            }
        }, cancellationToken);
    }

    public Task CreateFolderAsync(string directoryPath, string folderName,
        CancellationToken cancellationToken = default)
    {
        ValidateName(folderName);
        string remotePath = CombinePath(NormalizePath(directoryPath), folderName);
        return RunAsync(() =>
        {
            EnsureConnected();
            if (!_client.CreateDirectory(ToRemotePath(remotePath)))
            {
                throw CreateOperationException("Creating the folder");
            }
        }, cancellationToken);
    }

    public Task RenameAsync(RemoteStorageItem item, string newName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        ValidateName(newName);

        string parentPath = GetParentPath(item.Path) ?? RootPath;
        string destinationPath = CombinePath(parentPath, newName);
        return RunAsync(() =>
        {
            EnsureConnected();
            _client.Rename(ToRemotePath(item.Path), ToRemotePath(destinationPath));
        }, cancellationToken);
    }

    public Task<bool> HasChildrenAsync(RemoteStorageItem folder,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(folder);
        if (!folder.IsFolder)
        {
            throw new ArgumentException("The operation requires a folder.", nameof(folder));
        }

        return RunAsync(() =>
        {
            EnsureConnected();
            return _client.GetListing(ToRemotePath(folder.Path)).Any(IsBrowsableItem);
        }, cancellationToken);
    }

    public Task DeleteAsync(RemoteStorageItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);

        return RunAsync(() =>
        {
            EnsureConnected();
            string remotePath = ToRemotePath(item.Path);
            if (item.IsFolder)
            {
                _client.DeleteDirectory(remotePath);
            }
            else
            {
                _client.DeleteFile(remotePath);
            }
        }, cancellationToken);
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

    private void EnsureConnected()
    {
        if (!_client.Connect())
        {
            throw CreateOperationException("Connecting to the server");
        }
    }

    private static bool IsBrowsableItem(FtpListItem item)
    {
        return item.Name is not "." and not ".." &&
            item.Type is FtpObjectType.File or FtpObjectType.Directory;
    }

    private static DateTimeOffset? ToDateTimeOffset(DateTime value)
    {
        return value == default ? null : new DateTimeOffset(value);
    }

    private static string ToRemotePath(string path)
    {
        path = NormalizePath(path);
        return path.Length > 0 ? $"/{path}" : "/";
    }

    private static string CombinePath(string directoryPath, string name)
    {
        directoryPath = NormalizePath(directoryPath);
        return directoryPath.Length > 0 ? $"{directoryPath}/{name}" : name;
    }

    private static string NormalizePath(string? path) =>
        path?.Replace('\\', '/').Trim('/') ?? string.Empty;

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

    private static InvalidOperationException CreateOperationException(string action) =>
        new($"{action} failed.");

    private static Task RunAsync(Action action, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            action();
            cancellationToken.ThrowIfCancellationRequested();
        }, cancellationToken);
    }

    private static Task<T> RunAsync<T>(Func<T> action, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            T result = action();
            cancellationToken.ThrowIfCancellationRequested();
            return result;
        }, cancellationToken);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
