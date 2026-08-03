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
using SkiaSharp;

namespace ShareX.Tools;

public partial class BackgroundRemoverWindow : Window
{
    private static readonly Cursor WaitCursor = new(StandardCursorType.Wait);
    private readonly BackgroundRemoverViewModel _viewModel;

    public BackgroundRemoverWindow()
        : this(null)
    {
    }

    public BackgroundRemoverWindow(string? modelsFolder)
        : this(modelsFolder, new BackgroundRemoverOptions())
    {
    }

    public BackgroundRemoverWindow(string? modelsFolder, BackgroundRemoverOptions options)
    {
        _viewModel = new BackgroundRemoverViewModel(modelsFolder, options);
        DataContext = _viewModel;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _viewModel.SelectImageFileRequested = SelectImageFileAsync;
        _viewModel.SaveImageRequested = SaveImageAsync;
        _viewModel.SaveImageAsRequested = SaveImageAsAsync;
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        _viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(BackgroundRemoverViewModel.IsProcessing))
            {
                Cursor = _viewModel.IsProcessing ? WaitCursor : null;
            }
        };
        Closed += (_, _) => _viewModel.Dispose();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async Task<string?> SelectImageFileAsync(string title)
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });

        return files.Count > 0 ? files[0].Path.LocalPath : null;
    }

    private Task<string?> SaveImageAsync(SKBitmap image, string? sourcePath)
    {
        string extension = Path.GetExtension(sourcePath)?.ToLowerInvariant() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(sourcePath) && extension is ".png" or ".webp")
        {
            SaveImageToFile(image, sourcePath);
            return Task.FromResult<string?>(sourcePath);
        }

        return SaveImageAsAsync(image, sourcePath);
    }

    private async Task<string?> SaveImageAsAsync(SKBitmap image, string? sourcePath)
    {
        string suggestedFileName = string.IsNullOrWhiteSpace(sourcePath)
            ? "image-output.png"
            : $"{Path.GetFileNameWithoutExtension(sourcePath)}-output.png";

        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Localization.Strings.BackgroundRemoverWindow_Save_image_as_dialog,
            SuggestedFileName = suggestedFileName,
            DefaultExtension = "png",
            FileTypeChoices =
            [
                new FilePickerFileType("PNG") { Patterns = ["*.png"] },
                new FilePickerFileType("WebP") { Patterns = ["*.webp"] }
            ]
        });

        if (file == null)
        {
            return null;
        }

        string filePath = file.Path.LocalPath;
        SaveImageToFile(image, filePath);
        return filePath;
    }

    private static void SaveImageToFile(SKBitmap image, string filePath)
    {
        SKEncodedImageFormat format = Path.GetExtension(filePath).Equals(".webp", StringComparison.OrdinalIgnoreCase)
            ? SKEncodedImageFormat.Webp
            : SKEncodedImageFormat.Png;

        using SKImage skImage = SKImage.FromBitmap(image);
        using SKData data = skImage.Encode(format, 100);
        using FileStream stream = File.Create(filePath);
        data.SaveTo(stream);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = !_viewModel.IsProcessing && e.DataTransfer.Formats.Contains(DataFormat.File)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (_viewModel.IsProcessing)
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
            _viewModel.LoadImage(file.Path.LocalPath);
            e.Handled = true;
        }
    }

    private void OnNotificationPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _viewModel.DismissNotification();
        e.Handled = true;
    }

    private void OnNotificationPointerEntered(object? sender, PointerEventArgs e)
    {
        _viewModel.SetNotificationHoverState(true);
    }

    private void OnNotificationPointerExited(object? sender, PointerEventArgs e)
    {
        _viewModel.SetNotificationHoverState(false);
    }
}
