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
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.Rendering;

namespace ShareX.ImageEditor.Presentation.Controls
{
    public partial class CursorTypePickerDropdown : UserControl
    {
        public static readonly StyledProperty<CursorType> SelectedCursorTypeProperty =
            AvaloniaProperty.Register<CursorTypePickerDropdown, CursorType>(
                nameof(SelectedCursorType),
                defaultValue: CursorType.Default);

        public static readonly StyledProperty<IEnumerable<CursorType>> CursorTypeOptionsProperty =
            AvaloniaProperty.Register<CursorTypePickerDropdown, IEnumerable<CursorType>>(
                nameof(CursorTypeOptions),
                defaultValue: Array.Empty<CursorType>());

        public CursorType SelectedCursorType
        {
            get => GetValue(SelectedCursorTypeProperty);
            set => SetValue(SelectedCursorTypeProperty, value);
        }

        public IEnumerable<CursorType> CursorTypeOptions
        {
            get => GetValue(CursorTypeOptionsProperty);
            set => SetValue(CursorTypeOptionsProperty, value);
        }

        public event EventHandler<CursorType>? CursorTypeChanged;

        public CursorTypePickerDropdown()
        {
            AvaloniaXamlLoader.Load(this);

            var popup = this.FindControl<Popup>("CursorTypePopup");
            if (popup != null)
            {
                popup.Opened += OnPopupOpened;
            }
        }

        private void OnPopupOpened(object? sender, EventArgs e)
        {
            UpdateActiveStates();
        }

        private void UpdateActiveStates()
        {
            var popup = this.FindControl<Popup>("CursorTypePopup");
            if (popup?.Child is Border border && border.Child is ItemsControl itemsControl)
            {
                foreach (var item in itemsControl.GetRealizedContainers())
                {
                    if (item is ContentPresenter presenter && presenter.Child is Border { Child: Button button })
                    {
                        if (button.CommandParameter is CursorType cursorType && cursorType == SelectedCursorType)
                        {
                            button.Classes.Add("active");
                        }
                        else
                        {
                            button.Classes.Remove("active");
                        }
                    }
                }
            }
        }

        private void OnDropdownButtonClick(object? sender, RoutedEventArgs e)
        {
            var popup = this.FindControl<Popup>("CursorTypePopup");
            if (popup != null)
            {
                popup.IsOpen = !popup.IsOpen;
            }
        }

        private void OnCursorTypeSelected(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is CursorType selectedCursorType)
            {
                if (!CursorBitmapRenderer.GetSupport(selectedCursorType).IsSupported) return;
                SelectedCursorType = selectedCursorType;
                UpdateActiveStates();

                Dispatcher.UIThread.Post(() =>
                {
                    var popup = this.FindControl<Popup>("CursorTypePopup");
                    if (popup != null)
                    {
                        popup.IsOpen = false;
                    }

                    CursorTypeChanged?.Invoke(this, selectedCursorType);
                }, DispatcherPriority.Input);
            }
        }
    }
}
