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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System.Globalization;

namespace ShareX.ImageEditor.Presentation.Controls
{
    public partial class FontFamilyPickerDropdown : UserControl
    {
        public static readonly StyledProperty<string> SelectedFontFamilyProperty =
            AvaloniaProperty.Register<FontFamilyPickerDropdown, string>(
                nameof(SelectedFontFamily),
                defaultValue: "Segoe UI");

        public static readonly StyledProperty<IEnumerable<string>> FontFamilyOptionsProperty =
            AvaloniaProperty.Register<FontFamilyPickerDropdown, IEnumerable<string>>(
                nameof(FontFamilyOptions),
                defaultValue: Array.Empty<string>());

        public string SelectedFontFamily
        {
            get => GetValue(SelectedFontFamilyProperty);
            set => SetValue(SelectedFontFamilyProperty, value);
        }

        public IEnumerable<string> FontFamilyOptions
        {
            get => GetValue(FontFamilyOptionsProperty);
            set => SetValue(FontFamilyOptionsProperty, value);
        }

        public event EventHandler<string>? FontFamilyChanged;

        private string _searchQuery = string.Empty;
        private bool _isEscapePressed;

        public FontFamilyPickerDropdown()
        {
            AvaloniaXamlLoader.Load(this);

            var popup = this.FindControl<Popup>("FontFamilyPopup");
            if (popup != null)
            {
                popup.Opened += OnPopupOpened;
                popup.Closed += OnPopupClosed;
            }

            var itemsControl = this.FindControl<ItemsControl>("FontFamilyItems");
            if (itemsControl != null)
            {
                itemsControl.ContainerPrepared += OnFontFamilyContainerPrepared;
            }
        }

        private void OnPopupOpened(object? sender, EventArgs e)
        {
            UpdateActiveStates();

            Dispatcher.UIThread.Post(() => this.FindControl<TextBox>("SearchBox")?.Focus(), DispatcherPriority.Loaded);
        }

        private void OnPopupClosed(object? sender, EventArgs e)
        {
            _isEscapePressed = false;

            var searchBox = this.FindControl<TextBox>("SearchBox");
            if (searchBox != null)
            {
                searchBox.Text = string.Empty;
            }
        }

        private IEnumerable<(Button Button, string FontFamily)> GetFontFamilyItems()
        {
            var itemsControl = this.FindControl<ItemsControl>("FontFamilyItems");
            if (itemsControl == null)
            {
                yield break;
            }

            foreach (var item in itemsControl.GetRealizedContainers())
            {
                if (item is ContentPresenter { Child: Button { CommandParameter: string fontFamily } button })
                {
                    yield return (button, fontFamily);
                }
            }
        }

        private void UpdateActiveStates()
        {
            foreach (var (button, fontFamily) in GetFontFamilyItems())
            {
                if (string.Equals(fontFamily, SelectedFontFamily, StringComparison.Ordinal))
                {
                    button.Classes.Add("active");
                }
                else
                {
                    button.Classes.Remove("active");
                }
            }
        }

        private void OnFontFamilyContainerPrepared(object? sender, ContainerPreparedEventArgs e)
        {
            // Containers created after a search, e.g. for a new FontFamilyOptions list, need the filter too.
            if (sender is ItemsControl itemsControl && itemsControl.ItemFromContainer(e.Container) is string fontFamily)
            {
                e.Container.IsVisible = MatchesSearch(fontFamily, _searchQuery);
            }
        }

        private void ApplySearchFilter()
        {
            _searchQuery = this.FindControl<TextBox>("SearchBox")?.Text?.Trim() ?? string.Empty;

            var itemsControl = this.FindControl<ItemsControl>("FontFamilyItems");
            if (itemsControl != null)
            {
                foreach (var container in itemsControl.GetRealizedContainers())
                {
                    if (itemsControl.ItemFromContainer(container) is string fontFamily)
                    {
                        container.IsVisible = MatchesSearch(fontFamily, _searchQuery);
                    }
                }
            }

            var noResultsText = this.FindControl<TextBlock>("NoResultsText");
            if (noResultsText != null)
            {
                noResultsText.IsVisible = GetFirstSearchMatch() == null;
            }

            var scrollViewer = this.FindControl<ScrollViewer>("FontFamilyScrollViewer");
            if (scrollViewer != null)
            {
                scrollViewer.Offset = default;
            }
        }

        private string? GetFirstSearchMatch()
        {
            return FontFamilyOptions.FirstOrDefault(fontFamily => MatchesSearch(fontFamily, _searchQuery));
        }

        private static bool MatchesSearch(string fontFamily, string query)
        {
            // Ignoring symbols also ignores spaces, so "segoeui" matches "Segoe UI".
            return query.Length == 0 || CultureInfo.CurrentCulture.CompareInfo.IndexOf(fontFamily, query,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols) >= 0;
        }

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            ApplySearchFilter();
        }

        // Handled on the popup content rather than the search box, so keys from a focused font button are covered too.
        private void OnPopupKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyModifiers != KeyModifiers.None)
            {
                return;
            }

            if (e.Key == Key.Enter)
            {
                string? fontFamily = GetFirstSearchMatch();
                if (fontFamily != null)
                {
                    SelectFontFamily(fontFamily);
                }

                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                // Escape is acted on in OnPopupKeyUp, because the editor and region capture close on its key up.
                _isEscapePressed = true;
                e.Handled = true;
            }
        }

        private void OnPopupKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape || e.KeyModifiers != KeyModifiers.None)
            {
                return;
            }

            e.Handled = true;

            // An IME consumes the Escape key down that cancels its composition but still sends the key up.
            if (!_isEscapePressed)
            {
                return;
            }

            _isEscapePressed = false;

            var searchBox = this.FindControl<TextBox>("SearchBox");
            if (searchBox != null && !string.IsNullOrEmpty(searchBox.Text))
            {
                searchBox.Text = string.Empty;
            }
            else
            {
                var popup = this.FindControl<Popup>("FontFamilyPopup");
                if (popup != null)
                {
                    popup.IsOpen = false;
                }
            }
        }

        private void OnDropdownButtonClick(object? sender, RoutedEventArgs e)
        {
            var popup = this.FindControl<Popup>("FontFamilyPopup");
            if (popup != null)
            {
                popup.IsOpen = !popup.IsOpen;
            }
        }

        private void OnFontFamilySelected(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string selectedFontFamily)
            {
                SelectFontFamily(selectedFontFamily);
            }
        }

        private void SelectFontFamily(string selectedFontFamily)
        {
            SelectedFontFamily = selectedFontFamily;
            UpdateActiveStates();

            Dispatcher.UIThread.Post(() =>
            {
                var popup = this.FindControl<Popup>("FontFamilyPopup");
                if (popup != null)
                {
                    popup.IsOpen = false;
                }

                FontFamilyChanged?.Invoke(this, selectedFontFamily);
            }, DispatcherPriority.Input);
        }
    }
}