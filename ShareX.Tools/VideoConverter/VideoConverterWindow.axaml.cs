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
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ShareX.Tools;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class VideoConverterWindow : Window
{
    private readonly VideoConverterViewModel _viewModel;

    public VideoConverterWindow()
        : this(
            new VideoConverterOptions(),
            (_, _, _) => Task.FromResult(new VideoConversionResult(false, false, "FFmpeg is unavailable.")))
    {
    }

    public VideoConverterWindow(
        VideoConverterOptions options,
        VideoConversionHandler conversionHandler,
        string? inputFilePath = null)
    {
        _viewModel = new VideoConverterViewModel(options, conversionHandler, inputFilePath);
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _viewModel.SelectInputFileRequested = SelectInputFileAsync;
        _viewModel.SelectOutputFolderRequested = SelectOutputFolderAsync;
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        Closed += (_, _) => _viewModel.Dispose();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async Task<string?> SelectInputFileAsync(string title)
    {
        IStorageFolder? suggestedStartLocation = await GetSuggestedFolderAsync(
            Path.GetDirectoryName(_viewModel.InputFilePath));

        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            SuggestedStartLocation = suggestedStartLocation,
            FileTypeFilter =
            [
                new FilePickerFileType("Video and animation files")
                {
                    Patterns = ["*.mp4", "*.mkv", "*.webm", "*.avi", "*.mov", "*.m4v", "*.wmv", "*.gif", "*.webp", "*.png", "*.apng"]
                },
                FilePickerFileTypes.All
            ]
        });

        return files.Count > 0 ? files[0].Path.LocalPath : null;
    }

    private async Task<string?> SelectOutputFolderAsync(string title)
    {
        IStorageFolder? suggestedStartLocation = await GetSuggestedFolderAsync(_viewModel.OutputFolderPath);

        IReadOnlyList<IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            SuggestedStartLocation = suggestedStartLocation
        });

        return folders.Count > 0 ? folders[0].Path.LocalPath : null;
    }

    private async Task<IStorageFolder?> GetSuggestedFolderAsync(string? folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
        {
            return null;
        }

        return await StorageProvider.TryGetFolderFromPathAsync(new Uri(folderPath));
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = _viewModel.IsIdle && e.DataTransfer.Formats.Contains(DataFormat.File)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (!_viewModel.IsIdle)
        {
            return;
        }

        List<IStorageItem> droppedItems = e.DataTransfer.TryGetFiles()?.ToList() ?? [];
        if (droppedItems.Count == 0)
        {
            foreach (IDataTransferItem item in e.DataTransfer.Items)
            {
                if (item.TryGetRaw(DataFormat.File) is IStorageItem storageItem)
                {
                    droppedItems.Add(storageItem);
                }
            }
        }

        IStorageFile? file = droppedItems.OfType<IStorageFile>().FirstOrDefault();
        if (file != null)
        {
            _viewModel.LoadInput(file.Path.LocalPath);
            e.Handled = true;
        }
    }
}
