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
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Theming;
using Strings = ShareX.Tools.Localization.Strings;

namespace ShareX.Tools;

public partial class VideoTrimmerWindow : Window
{
    private readonly VideoTrimmerViewModel _viewModel;

    public VideoTrimmerWindow() : this("ffmpeg.exe") { }

    public VideoTrimmerWindow(string ffmpegPath, string? inputFilePath = null)
    {
        _viewModel = new(ffmpegPath);
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _viewModel.SelectInputRequested = SelectInputAsync;
        _viewModel.SelectOutputRequested = SelectOutputAsync;
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        Opened += async (_, _) =>
        {
            if (!string.IsNullOrEmpty(inputFilePath)) await _viewModel.LoadInputAsync(inputFilePath);
        };
        Closed += (_, _) => _viewModel.Dispose();
    }

    private async Task<string?> SelectInputAsync()
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Strings.VideoTrimmer_Open,
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType(Strings.VideoTrimmer_VideoFiles)
                {
                    Patterns = ["*.mp4", "*.mkv", "*.webm", "*.mov", "*.avi", "*.m4v", "*.wmv", "*.ts", "*.mts", "*.m2ts"]
                },
                FilePickerFileTypes.All
            ]
        });
        return files.FirstOrDefault()?.TryGetLocalPath();
    }

    private async Task<string?> SelectOutputAsync(string suggestedName)
    {
        string extension = Path.GetExtension(suggestedName);
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Strings.VideoTrimmer_Export,
            SuggestedFileName = suggestedName,
            SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(Path.GetDirectoryName(_viewModel.InputFilePath)!),
            DefaultExtension = extension.TrimStart('.'),
            ShowOverwritePrompt = true,
            FileTypeChoices = [new FilePickerFileType(Strings.VideoTrimmer_VideoFiles) { Patterns = ["*" + extension] }]
        });
        return file?.TryGetLocalPath();
    }

    private void OnStartTimeLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox) _viewModel.SetStartTime(textBox.Text);
    }

    private void OnEndTimeLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox) _viewModel.SetEndTime(textBox.Text);
    }

    private void OnStartTimeKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && sender is TextBox textBox)
        {
            _viewModel.SetStartTime(textBox.Text);
            e.Handled = true;
        }
    }

    private void OnEndTimeKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && sender is TextBox textBox)
        {
            _viewModel.SetEndTime(textBox.Text);
            e.Handled = true;
        }
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = _viewModel.CanBrowse && e.DataTransfer.Formats.Contains(DataFormat.File)
            ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        if (!_viewModel.CanBrowse) return;
        string? path = e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().FirstOrDefault()?.TryGetLocalPath();
        if (path != null)
        {
            e.Handled = true;
            await _viewModel.LoadInputAsync(path);
        }
    }
}
