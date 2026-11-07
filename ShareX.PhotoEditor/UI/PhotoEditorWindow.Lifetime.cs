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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.PhotoEditor.Integration;
using ShareX.PhotoEditor.Tools;
using ShareX.PhotoEditor.UI.Commands;
using ShareX.PhotoEditor.UI.Dialogs;
using ShareX.PhotoEditor.UI.Panels;

namespace ShareX.PhotoEditor.UI;

public sealed partial class PhotoEditorWindow
{
    public async Task<IReadOnlyList<string>> PickOpenFilesAsync(string title, bool multiple = true)
    {
        IStorageFolder? start = Services.DefaultFolder != null ? await StorageProvider.TryGetFolderFromPathAsync(Services.DefaultFolder) : null;
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = multiple,
            SuggestedStartLocation = start,
            FileTypeFilter =
            [
                new FilePickerFileType(Strings.ImageIO_AllSupportedImages) { Patterns = ImageIO.Formats.SelectMany(f => f.Extensions).Select(x => "*" + x).ToArray() },
                .. ImageIO.Formats.Select(f => new FilePickerFileType(f.Name) { Patterns = f.Extensions.Select(x => "*" + x).ToArray() }),
                FilePickerFileTypes.All
            ]
        });
        return files.Select(f => f.TryGetLocalPath()).Where(p => p != null).Select(p => p!).ToList();
    }

    public async Task<string?> PickSaveFileAsync(string title, string suggestedName, bool projectFirst)
    {
        IStorageFolder? start = Document?.FilePath != null ? await StorageProvider.TryGetFolderFromPathAsync(Path.GetDirectoryName(Document.FilePath)!) :
            Services.DefaultFolder != null ? await StorageProvider.TryGetFolderFromPathAsync(Services.DefaultFolder) : null;
        IEnumerable<FileFormatInfo> formats = ImageIO.Formats.Where(f => f.CanSave);
        if (!projectFirst) formats = formats.OrderBy(f => f.Extensions[0] == ImageIO.ProjectExtension);
        List<(FileFormatInfo Format, FilePickerFileType Type)> choices = formats.Select(f => (f, new FilePickerFileType(f.Name) { Patterns = f.Extensions.Select(x => "*" + x).ToArray() })).ToList();
        // Preselect the file type matching the suggested name, otherwise the dialog falls back to the first type and replaces the extension.
        string extension = Path.GetExtension(suggestedName);
        FilePickerFileType? suggestedType = choices.FirstOrDefault(c => c.Format.Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase)).Type;
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = suggestedName,
            SuggestedStartLocation = start,
            DefaultExtension = suggestedType != null ? extension : projectFirst ? ImageIO.ProjectExtension : ".png",
            SuggestedFileType = suggestedType,
            ShowOverwritePrompt = true,
            FileTypeChoices = choices.Select(c => c.Type).ToList()
        });
        return file?.TryGetLocalPath();
    }

    private bool _windowPlacementApplied;
    private bool _windowPlacementSavePending;
    private WindowState _restoreWindowState = WindowState.Normal;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == WindowStateProperty && change.GetNewValue<WindowState>() != WindowState.Minimized)
        {
            _restoreWindowState = change.GetNewValue<WindowState>();
        }
    }

    /// <summary>Shows the window, restores it when minimized to the state it had before, and brings it to the front.</summary>
    public void BringToFront()
    {
        if (!IsVisible) Show();
        if (WindowState == WindowState.Minimized) WindowState = _restoreWindowState;
        Activate();
    }

    // ------------------------------------------------------------------ closing

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (IsBusy)
        {
            e.Cancel = true;
            return;
        }
        if (_closingConfirmed)
        {
            SaveSettings();
            return;
        }
        if (_views.Any(v => v.Document.IsDirty && v.Document != TaskDocument))
        {
            e.Cancel = true;
            foreach (DocumentView view in _views.ToList())
            {
                if (!await CloseViewAsync(view)) return;
            }
            _closingConfirmed = true;
            Close();
            return;
        }
        SaveSettings();
        // Timers and queued callbacks can still run after closing, so the active view must not point at disposed documents.
        _view = null;
        foreach (DocumentView view in _views.ToList())
        {
            CaptureTaskResult(view);
            view.Dispose();
            view.Document.Dispose();
        }
        _views.Clear();
    }

    private void ApplySavedWindowState()
    {
        if (!Settings.RememberWindowState) return;
        ShareX.HelpersLib.WindowState saved = Settings.WindowState;
        if (!saved.Size.IsEmpty)
        {
            Width = saved.Size.Width;
            Height = saved.Size.Height;
        }
        if (!saved.Location.IsEmpty)
        {
            PixelPoint position = new(saved.Location.X, saved.Location.Y);
            if (Screens.ScreenFromPoint(position) != null)
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Position = position;
            }
        }
        if (saved.IsMaximized) WindowState = WindowState.Maximized;
    }

    private void QueueWindowPlacementSave()
    {
        if (!_windowPlacementApplied || _windowPlacementSavePending) return;
        _windowPlacementSavePending = true;
        // Maximizing moves the window before its state changes, so wait for the state to settle.
        Dispatcher.UIThread.Post(() =>
        {
            _windowPlacementSavePending = false;
            SaveNormalWindowPlacement();
        }, DispatcherPriority.Background);
    }

    /// <summary>Remembers the size and position to restore to; a maximized window keeps the ones from before it was maximized.</summary>
    private void SaveNormalWindowPlacement()
    {
        if (!Settings.RememberWindowState || WindowState != WindowState.Normal || !IsVisible || Bounds.Width <= 0 || Bounds.Height <= 0) return;
        Settings.WindowState.Size = new System.Drawing.Size((int)Math.Round(Bounds.Width), (int)Math.Round(Bounds.Height));
        Settings.WindowState.Location = new System.Drawing.Point(Position.X, Position.Y);
    }

    private void SaveWindowState()
    {
        if (!Settings.RememberWindowState) return;
        SaveNormalWindowPlacement();
        // Full screen and minimized keep the last remembered state.
        if (WindowState is WindowState.Normal or WindowState.Maximized) Settings.WindowState.IsMaximized = WindowState == WindowState.Maximized;
    }

    public void SaveSettings()
    {
        SaveWindowState();
        _dock.SaveLayout();
        if (_dockColumn.ActualWidth > 0) Settings.RightDockWidth = _dockColumn.ActualWidth;
        Settings.Save(Services.SettingsFilePath);
    }
}
