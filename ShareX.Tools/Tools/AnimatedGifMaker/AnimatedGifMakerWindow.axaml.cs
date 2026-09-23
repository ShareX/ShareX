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
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class AnimatedGifMakerWindow : Window
{
    private readonly AnimatedGifMakerViewModel _viewModel;

    public AnimatedGifMakerWindow() : this(null)
    {
    }

    public AnimatedGifMakerWindow(IEnumerable<string>? imageFiles)
    {
        _viewModel = new AnimatedGifMakerViewModel();
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _viewModel.SelectFilesRequested = SelectFilesAsync;
        _viewModel.SelectOutputFileRequested = SelectOutputFileAsync;

        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        Closed += (_, _) => _viewModel.Dispose();

        _viewModel.AddFiles(imageFiles);
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async Task<IReadOnlyList<string>?> SelectFilesAsync()
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Localization.Strings.AnimatedGifMakerWindow_Add_images_dialog,
            AllowMultiple = true,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });
        return files.Select(file => file.Path.LocalPath).ToArray();
    }

    private async Task<string?> SelectOutputFileAsync(string suggestedFileName)
    {
        string? sourceFolder = Path.GetDirectoryName(_viewModel.Images.FirstOrDefault());
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Localization.Strings.AnimatedGifMakerWindow_Save_GIF_dialog,
            SuggestedFileName = suggestedFileName,
            SuggestedStartLocation = !string.IsNullOrWhiteSpace(sourceFolder) && Directory.Exists(sourceFolder)
                ? await StorageProvider.TryGetFolderFromPathAsync(sourceFolder)
                : null,
            DefaultExtension = "gif",
            ShowOverwritePrompt = true,
            FileTypeChoices =
            [
                new FilePickerFileType(Localization.Strings.AnimatedGifMakerWindow_GIF_images)
                {
                    Patterns = ["*.gif"]
                }
            ]
        });
        return file?.TryGetLocalPath();
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItems != null)
        {
            _viewModel.SetSelectedImages(listBox.SelectedItems.OfType<string>());
        }
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Formats.Contains(DataFormat.File)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
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

        string[] files = droppedItems.OfType<IStorageFile>()
            .Select(file => file.Path.LocalPath).ToArray();
        if (files.Length > 0)
        {
            _viewModel.AddFiles(files);
            e.Handled = true;
        }
    }
}
