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
    public partial class FontSizePickerDropdown : UserControl
    {
        public const float MinimumFontSize = 1;
        public const float MaximumFontSize = 1000;

        public static readonly StyledProperty<float> SelectedFontSizeProperty =
            AvaloniaProperty.Register<FontSizePickerDropdown, float>(
                nameof(SelectedFontSize),
                defaultValue: 30);

        public static readonly StyledProperty<IEnumerable<float>> FontSizeOptionsProperty =
            AvaloniaProperty.Register<FontSizePickerDropdown, IEnumerable<float>>(
                nameof(FontSizeOptions),
                defaultValue: GetDefaultFontSizes());

        public float SelectedFontSize
        {
            get => GetValue(SelectedFontSizeProperty);
            set => SetValue(SelectedFontSizeProperty, value);
        }

        public IEnumerable<float> FontSizeOptions
        {
            get => GetValue(FontSizeOptionsProperty);
            set => SetValue(FontSizeOptionsProperty, value);
        }

        public event EventHandler<float>? FontSizeChanged;

        private bool _escapeKeyDownHandled;

        public FontSizePickerDropdown()
        {
            AvaloniaXamlLoader.Load(this);

            var popup = this.FindControl<Popup>("FontSizePopup");
            if (popup != null)
            {
                popup.Opened += OnPopupOpened;
                popup.Closed += OnPopupClosed;
            }
        }

        private void OnPopupOpened(object? sender, EventArgs e)
        {
            UpdateActiveStates();

            var customSizeBox = this.FindControl<TextBox>("CustomSizeBox");
            if (customSizeBox != null)
            {
                customSizeBox.Text = FormatFontSize(SelectedFontSize);

                Dispatcher.UIThread.Post(() =>
                {
                    customSizeBox.Focus();
                    customSizeBox.SelectAll();
                }, DispatcherPriority.Loaded);
            }
        }

        private void OnPopupClosed(object? sender, EventArgs e)
        {
            _escapeKeyDownHandled = false;
        }

        private void UpdateActiveStates()
        {
            var itemsControl = this.FindControl<ItemsControl>("FontSizeItems");
            if (itemsControl == null)
            {
                return;
            }

            foreach (var item in itemsControl.GetRealizedContainers())
            {
                if (item is ContentPresenter presenter && presenter.Child is Button button)
                {
                    if (button.CommandParameter is float size && size == SelectedFontSize)
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

        private void OnDropdownButtonClick(object? sender, RoutedEventArgs e)
        {
            var popup = this.FindControl<Popup>("FontSizePopup");
            if (popup != null)
            {
                popup.IsOpen = !popup.IsOpen;
            }
        }

        private void OnFontSizeSelected(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is float selectedSize)
            {
                SelectFontSize(selectedSize);
            }
        }

        private void SelectFontSize(float selectedSize)
        {
            SelectedFontSize = selectedSize;

            // Defer popup close and event firing to after the current input cycle.
            // Closing the popup synchronously during a Click handler disposes the
            // PopupRoot's native window while PointerReleased is still being dispatched
            // through it, causing "PlatformImpl is null" warnings.
            Dispatcher.UIThread.Post(() =>
            {
                var popup = this.FindControl<Popup>("FontSizePopup");
                if (popup != null)
                {
                    popup.IsOpen = false;
                }

                FontSizeChanged?.Invoke(this, selectedSize);
            }, DispatcherPriority.Input);
        }

        // Handled on the popup content rather than the text box, so keys from a focused size button are covered too.
        private void OnPopupKeyDown(object? sender, KeyEventArgs e)
        {
            // A key down that reaches the popup content, such as one an IME processed, breaks the pairing with the next Escape key up.
            _escapeKeyDownHandled = false;

            if (e.KeyModifiers != KeyModifiers.None)
            {
                return;
            }

            if (e.Key == Key.Enter)
            {
                var customSizeBox = this.FindControl<TextBox>("CustomSizeBox");
                if (customSizeBox != null && customSizeBox.IsFocused)
                {
                    if (TryParseFontSize(customSizeBox.Text, out float fontSize))
                    {
                        SelectFontSize(fontSize);
                    }
                    else
                    {
                        customSizeBox.Text = FormatFontSize(SelectedFontSize);
                        customSizeBox.SelectAll();
                    }

                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Escape)
            {
                // Escape is acted on in OnPopupKeyUp, because the editor and region capture close on its key up.
                _escapeKeyDownHandled = true;
                e.Handled = true;
            }
        }

        private void OnPopupKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape)
            {
                return;
            }

            bool escapeKeyDownHandled = _escapeKeyDownHandled;
            _escapeKeyDownHandled = false;

            if (e.KeyModifiers != KeyModifiers.None)
            {
                return;
            }

            e.Handled = true;

            // An IME consumes the Escape key down that cancels its composition but still sends the key up.
            if (escapeKeyDownHandled)
            {
                var popup = this.FindControl<Popup>("FontSizePopup");
                if (popup != null)
                {
                    popup.IsOpen = false;
                }
            }
        }

        private static string FormatFontSize(float fontSize)
        {
            return fontSize.ToString(CultureInfo.CurrentCulture);
        }

        // Accepts "36", "36.5" (or the current culture's decimal separator) and "36 pt"; the result is clamped to 1-1000.
        internal static bool TryParseFontSize(string? text, out float fontSize)
        {
            fontSize = 0;

            string value = text?.Trim() ?? string.Empty;
            if (value.EndsWith("pt", StringComparison.OrdinalIgnoreCase))
            {
                value = value[..^2].TrimEnd();
            }

            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out float parsed) &&
                !float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
            {
                return false;
            }

            if (!float.IsFinite(parsed) || parsed <= 0)
            {
                return false;
            }

            fontSize = MathF.Round(Math.Clamp(parsed, MinimumFontSize, MaximumFontSize), 1);
            return true;
        }

        private static IEnumerable<float> GetDefaultFontSizes()
        {
            return new List<float> { 10, 13, 16, 20, 24, 30, 48, 72, 96, 144, 216, 288 };
        }
    }
}
