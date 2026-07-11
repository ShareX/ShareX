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
using ShareX.Tools.HashChecker;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools.HashChecker;

public partial class HashCheckerWindow : Window
{
    private readonly HashCheckerViewModel _viewModel;

    public HashCheckerWindow()
        : this((_, _, _, _) => Task.FromResult<string?>(null))
    {
    }

    public HashCheckerWindow(
        HashCalculationHandler hashCalculationHandler,
        Action? playNotificationSound = null,
        string? filePath = null)
    {
        _viewModel = new HashCheckerViewModel(hashCalculationHandler, playNotificationSound, filePath);
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _viewModel.SelectFileRequested = SelectFileAsync;

        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnFirstFileDrop);

        Control? secondFileDropZone = this.FindControl<Control>("SecondFileDropZone");
        if (secondFileDropZone != null)
        {
            DragDrop.SetAllowDrop(secondFileDropZone, true);
            secondFileDropZone.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            secondFileDropZone.AddHandler(DragDrop.DropEvent, OnSecondFileDrop);
        }

        Opened += OnOpened;
        Closed += (_, _) => _viewModel.Dispose();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        await _viewModel.StartAutomaticallyAsync();
    }

    private async Task<string?> SelectFileAsync(string title, string? currentFilePath)
    {
        IStorageFolder? suggestedStartLocation = null;
        string? folderPath = Path.GetDirectoryName(currentFilePath);
        if (!string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath))
        {
            suggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(new Uri(folderPath));
        }

        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            SuggestedStartLocation = suggestedStartLocation,
            FileTypeFilter = [FilePickerFileTypes.All]
        });

        return files.Count > 0 ? files[0].Path.LocalPath : null;
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = _viewModel.IsIdle && e.DataTransfer.Formats.Contains(DataFormat.File)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void OnFirstFileDrop(object? sender, DragEventArgs e)
    {
        SetDroppedFile(e, false);
    }

    private void OnSecondFileDrop(object? sender, DragEventArgs e)
    {
        SetDroppedFile(e, true);
    }

    private void SetDroppedFile(DragEventArgs e, bool secondFile)
    {
        if (!_viewModel.IsIdle)
        {
            return;
        }

        IStorageFile? file = GetDroppedFiles(e).OfType<IStorageFile>().FirstOrDefault();
        if (file != null)
        {
            _viewModel.SetDroppedFile(file.Path.LocalPath, secondFile);
            e.Handled = true;
        }
    }

    private static List<IStorageItem> GetDroppedFiles(DragEventArgs e)
    {
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

        return droppedItems;
    }
}
