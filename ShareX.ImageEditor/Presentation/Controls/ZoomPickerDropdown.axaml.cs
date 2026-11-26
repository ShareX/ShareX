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
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ShareX.ImageEditor.Presentation.Controls
{
    public partial class ZoomPickerDropdown : UserControl
    {
        public static readonly StyledProperty<double> SelectedZoomProperty =
            AvaloniaProperty.Register<ZoomPickerDropdown, double>(
                nameof(SelectedZoom),
                defaultValue: 1.0);

        public static readonly StyledProperty<IEnumerable<double>> ZoomLevelsProperty =
            AvaloniaProperty.Register<ZoomPickerDropdown, IEnumerable<double>>(
                nameof(ZoomLevels),
                defaultValue: GetDefaultZoomLevels());

        public static readonly StyledProperty<bool> IsCompactProperty =
            AvaloniaProperty.Register<ZoomPickerDropdown, bool>(nameof(IsCompact));

        /// <summary>Uses a smaller icon and minimum width, for the compact editor toolbar.</summary>
        public bool IsCompact
        {
            get => GetValue(IsCompactProperty);
            set => SetValue(IsCompactProperty, value);
        }

        public double SelectedZoom
        {
            get => GetValue(SelectedZoomProperty);
            set => SetValue(SelectedZoomProperty, value);
        }

        public IEnumerable<double> ZoomLevels
        {
            get => GetValue(ZoomLevelsProperty);
            set => SetValue(ZoomLevelsProperty, value);
        }

        public event EventHandler<double>? ZoomChanged;
        public event EventHandler? ZoomToFitRequested;

        public ZoomPickerDropdown()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsCompactProperty)
            {
                PseudoClasses.Set(":compact", change.GetNewValue<bool>());
            }
        }

        private void OnDropdownButtonClick(object? sender, RoutedEventArgs e)
        {
            var popup = this.FindControl<Popup>("ZoomPopup");
            if (popup != null)
            {
                popup.IsOpen = !popup.IsOpen;
            }
        }

        private void OnZoomSelected(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is double selectedZoom)
            {
                SelectedZoom = selectedZoom;
                ZoomChanged?.Invoke(this, selectedZoom);

                // Close the popup
                var popup = this.FindControl<Popup>("ZoomPopup");
                if (popup != null)
                {
                    popup.IsOpen = false;
                }
            }
        }

        private void OnZoomToFitSelected(object? sender, RoutedEventArgs e)
        {
            var popup = this.FindControl<Popup>("ZoomPopup");
            if (popup != null)
            {
                popup.IsOpen = false;
            }

            ZoomToFitRequested?.Invoke(this, EventArgs.Empty);
        }

        private static IEnumerable<double> GetDefaultZoomLevels()
        {
            return new List<double> { 0.25, 0.5, 0.75, 1.0, 1.25, 1.5, 2.0, 3.0, 4.0 };
        }
    }
}