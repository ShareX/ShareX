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

using ShareX.AvaloniaUI.Controls;
using ShareX.Icons;
using ShareX.Localization;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ShareX;

public sealed class TaskSettingsViewModel : INotifyPropertyChanged
{
    private readonly bool _isDefault;
    private SettingsNavigationItem? _selectedNavigationItem;

    public ObservableCollection<SettingsNavigationItem> NavigationItems { get; } = [];

    public SettingsNavigationItem? SelectedNavigationItem
    {
        get => _selectedNavigationItem;
        set
        {
            if (_isDefault && value is { Children.Count: > 0 } && value.Id is "general" or "upload")
            {
                value = value.Children[0];
            }

            if (ReferenceEquals(_selectedNavigationItem, value))
            {
                return;
            }

            _selectedNavigationItem = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNavigationItem)));
            SelectedPageChanged?.Invoke(value?.Id);
        }
    }

    public event Action<string?>? SelectedPageChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public TaskSettingsViewModel(bool isDefault)
    {
        _isDefault = isDefault;

        if (!isDefault)
        {
            NavigationItems.Add(new SettingsNavigationItem("task", Strings.TaskSettingsWindow_Task, AppIcons.Keyboard));
        }

        NavigationItems.Add(Parent("general", Strings.TaskSettingsWindow_General, AppIcons.Settings,
            Child("general-notifications", Strings.TaskSettingsWindow_Notifications, AppIcons.Bell)));
        NavigationItems.Add(Parent("image", Strings.TaskSettingsWindow_Image, AppIcons.Image,
            Child("image-effects", Strings.TaskSettingsWindow_Effects, AppIcons.MagicWand),
            Child("image-thumbnail", Strings.TaskSettingsWindow_Thumbnail, AppIcons.Images)));
        NavigationItems.Add(Parent("capture", Strings.TaskSettingsWindow_Capture, AppIcons.Camera,
            Child("capture-region", Strings.TaskSettingsWindow_RegionCapture, AppIcons.Crop),
            Child("capture-screen-recorder", Strings.TaskSettingsWindow_ScreenRecorder, AppIcons.Video),
            Child("capture-ocr", Strings.TaskSettingsWindow_OCR, AppIcons.TextRecognition)));
        NavigationItems.Add(Parent("upload", Strings.TaskSettingsWindow_Upload, AppIcons.Upload,
            Child("upload-file-naming", Strings.TaskSettingsWindow_FileNaming, AppIcons.FileEdit),
            Child("upload-clipboard", Strings.TaskSettingsWindow_ClipboardUpload, AppIcons.Clipboard),
            Child("upload-filters", Strings.TaskSettingsWindow_UploaderFilters, AppIcons.Filter)));
        NavigationItems.Add(Parent("tools", Strings.TaskSettingsWindow_Tools, AppIcons.Wrench,
            Child("tools-image-editor", Strings.TaskSettingsWindow_ImageEditor, AppIcons.Image),
            Child("tools-mouse-highlighter", Strings.MouseHighlighter, AppIcons.PointerClick)));
        NavigationItems.Add(Parent("actions", Strings.TaskSettingsWindow_Actions, AppIcons.Zap));
        NavigationItems.Add(Parent("watch-folders", Strings.TaskSettingsWindow_WatchFolders, AppIcons.FolderSearch));
        NavigationItems.Add(Parent("advanced", Strings.TaskSettingsWindow_Advanced, AppIcons.Sliders));

        SelectedNavigationItem = isDefault
            ? NavigationItems.SelectMany(x => x.Children).First(x => x.Id == "general-notifications")
            : NavigationItems[0];
    }

    private static SettingsNavigationItem Parent(string id, string title, string icon, params SettingsNavigationItem[] children) =>
        new(id, title, icon, children: children);

    public void SelectPage(string id)
    {
        SelectedNavigationItem = NavigationItems.SelectMany(item => item.Children.Prepend(item)).First(item => item.Id == id);
    }

    private static SettingsNavigationItem Child(string id, string title, string icon) => new(id, title, icon);
}
