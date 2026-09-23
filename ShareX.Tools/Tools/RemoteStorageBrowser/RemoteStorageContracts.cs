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

namespace ShareX.Tools;

[Flags]
public enum RemoteStorageProviderCapabilities
{
    None = 0,
    Download = 1,
    Upload = 2,
    Rename = 4,
    Delete = 8,
    Url = 16,
    CreateFolder = 32
}

public sealed class RemoteStorageItem
{
    public string Name { get; }
    public string Path { get; }
    public bool IsFolder { get; }
    public long? Size { get; }
    public DateTimeOffset? Modified { get; }

    public RemoteStorageItem(string name, string path, bool isFolder, long? size = null,
        DateTimeOffset? modified = null)
    {
        Name = name;
        Path = path;
        IsFolder = isFolder;
        Size = size;
        Modified = modified;
    }
}

public interface IRemoteStorageProvider
{
    string DisplayName { get; }
    string RootDisplayName { get; }
    string RootPath { get; }
    RemoteStorageProviderCapabilities Capabilities { get; }

    string GetDisplayPath(string path);
    string? GetParentPath(string path);
    Task<IReadOnlyList<RemoteStorageItem>> GetItemsAsync(string path, CancellationToken cancellationToken = default);
    Task DownloadAsync(RemoteStorageItem item, Stream destination, CancellationToken cancellationToken = default);
    Task UploadAsync(string directoryPath, string fileName, Stream source, CancellationToken cancellationToken = default);
    Task CreateFolderAsync(string directoryPath, string folderName, CancellationToken cancellationToken = default);
    Task RenameAsync(RemoteStorageItem item, string newName, CancellationToken cancellationToken = default);
    Task<bool> HasChildrenAsync(RemoteStorageItem folder, CancellationToken cancellationToken = default);
    Task DeleteAsync(RemoteStorageItem item, CancellationToken cancellationToken = default);
    string? GetUrl(RemoteStorageItem item);
}

public sealed class RemoteStorageBrowserServices
{
    public Action<string>? OpenUrl { get; init; }
}
