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
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.ViewModels;

namespace ShareX.ImageEditor.Presentation.Views
{
    /// <summary>
    /// Background settings shown in the editor's left sidebar. <see cref="EditorView"/> creates it the first time
    /// the sidebar opens, because building it is costly and most editor sessions, such as region capture, never open it.
    /// </summary>
    public partial class BackgroundSettingsPanel : UserControl
    {
        public BackgroundSettingsPanel()
        {
            InitializeComponent();
        }

        private void OnSidebarScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            // TODO: Restore sidebar scrollbar overlay logic
        }

        private async void OnBrowseBackgroundImageClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not MainViewModel vm)
            {
                return;
            }

            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.StorageProvider == null)
            {
                return;
            }

            IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = Strings.EditorView_SelectBackgroundImage,
                AllowMultiple = false,
                FileTypeFilter = [FilePickerFileTypes.ImageAll]
            });

            if (files.Count > 0)
            {
                vm.SetBackgroundImagePath(files[0].Path.LocalPath);
            }
        }

        private void OnGradientColor1PreviewPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            OpenBackgroundColorPicker(BackgroundGradientColor1Popup, BackgroundGradientColor2Popup, BackgroundColorPopup);
            e.Handled = true;
        }

        private void OnGradientColor2PreviewPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            OpenBackgroundColorPicker(BackgroundGradientColor2Popup, BackgroundGradientColor1Popup, BackgroundColorPopup);
            e.Handled = true;
        }

        private void OnBackgroundColorPreviewPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            OpenBackgroundColorPicker(BackgroundColorPopup, BackgroundGradientColor1Popup, BackgroundGradientColor2Popup);
            e.Handled = true;
        }

        private static void OpenBackgroundColorPicker(Popup popupToOpen, params Popup[] popupsToClose)
        {
            foreach (Popup popupToClose in popupsToClose)
            {
                popupToClose.IsOpen = false;
            }

            popupToOpen.IsOpen = true;
        }
    }
}
