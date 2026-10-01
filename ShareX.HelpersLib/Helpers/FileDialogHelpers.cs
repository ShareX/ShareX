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

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ShareX.AvaloniaUI.Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShareX.HelpersLib;

public static class FileDialogHelpers
{
    public static string[] OpenFiles(string title = null, string filter = null, bool multiselect = false,
        string initialDirectory = null, Window owner = null) => DesktopServices.Run(async () =>
    {
        IStorageProvider storage = DesktopServices.GetWindow(owner).StorageProvider;
        IReadOnlyList<IStorageFile> files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = multiselect,
            FileTypeFilter = ParseFilter(filter),
            SuggestedStartLocation = await GetFolder(storage, initialDirectory)
        });
        return files.Select(x => x.TryGetLocalPath()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
    });

    public static string SaveFile(string title = null, string filter = null, string fileName = null,
        string initialDirectory = null, string defaultExtension = null, int filterIndex = 1,
        Window owner = null) => DesktopServices.Run(async () =>
    {
        IStorageProvider storage = DesktopServices.GetWindow(owner).StorageProvider;
        IReadOnlyList<FilePickerFileType> types = ParseFilter(filter);
        IStorageFile file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = fileName,
            DefaultExtension = defaultExtension,
            FileTypeChoices = types,
            SuggestedFileType = types.Count > 0 ? types[Math.Clamp(filterIndex - 1, 0, types.Count - 1)] : null,
            SuggestedStartLocation = await GetFolder(storage, initialDirectory),
            ShowOverwritePrompt = true
        });
        return file?.TryGetLocalPath();
    });

    public static string OpenFolder(string title = null, string initialDirectory = null,
        Window owner = null) => DesktopServices.Run(async () =>
    {
        IStorageProvider storage = DesktopServices.GetWindow(owner).StorageProvider;
        IReadOnlyList<IStorageFolder> folders = await storage.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            SuggestedStartLocation = await GetFolder(storage, initialDirectory)
        });
        return folders.FirstOrDefault()?.TryGetLocalPath();
    });

    private static async System.Threading.Tasks.Task<IStorageFolder> GetFolder(IStorageProvider storage, string path) =>
        !string.IsNullOrEmpty(path) && Directory.Exists(path) ? await storage.TryGetFolderFromPathAsync(path) : null;

    internal static IReadOnlyList<FilePickerFileType> ParseFilter(string filter)
    {
        if (string.IsNullOrEmpty(filter)) return [FilePickerFileTypes.All];
        string[] parts = filter.Split('|');
        List<FilePickerFileType> types = new();
        for (int i = 0; i + 1 < parts.Length; i += 2)
            types.Add(new FilePickerFileType(parts[i])
            {
                Patterns = parts[i + 1].Split(';').Select(x => x == "*.*" ? "*" : x).ToArray()
            });
        return types;
    }
}
