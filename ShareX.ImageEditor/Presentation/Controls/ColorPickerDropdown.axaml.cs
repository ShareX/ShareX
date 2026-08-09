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
using Avalonia.Media;

namespace ShareX.ImageEditor.Presentation.Controls
{
    public partial class ColorPickerDropdown : UserControl
    {
        public static readonly StyledProperty<IBrush> SelectedColorProperty =
            AvaloniaProperty.Register<ColorPickerDropdown, IBrush>(
                nameof(SelectedColor),
                defaultValue: Brushes.Red);

        public static readonly StyledProperty<Color> SelectedColorValueProperty =
            AvaloniaProperty.Register<ColorPickerDropdown, Color>(
                nameof(SelectedColorValue),
                defaultValue: Colors.Red);

        public static readonly StyledProperty<HsvColor> SelectedHsvColorProperty =
            AvaloniaProperty.Register<ColorPickerDropdown, HsvColor>(
                nameof(SelectedHsvColor));

        public static readonly StyledProperty<bool> ShowBorderHoleProperty =
            AvaloniaProperty.Register<ColorPickerDropdown, bool>(
                nameof(ShowBorderHole));

        public static readonly StyledProperty<bool> ShowTextLabelProperty =
            AvaloniaProperty.Register<ColorPickerDropdown, bool>(
                nameof(ShowTextLabel));

        public static readonly StyledProperty<IBrush> IconForegroundProperty =
            AvaloniaProperty.Register<ColorPickerDropdown, IBrush>(
                nameof(IconForeground),
                defaultValue: new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)));

        public IBrush SelectedColor
        {
            get => GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public Color SelectedColorValue
        {
            get => GetValue(SelectedColorValueProperty);
            set => SetValue(SelectedColorValueProperty, value);
        }

        public HsvColor SelectedHsvColor
        {
            get => GetValue(SelectedHsvColorProperty);
            set => SetValue(SelectedHsvColorProperty, value);
        }

        public bool ShowBorderHole
        {
            get => GetValue(ShowBorderHoleProperty);
            set => SetValue(ShowBorderHoleProperty, value);
        }

        public bool ShowTextLabel
        {
            get => GetValue(ShowTextLabelProperty);
            set => SetValue(ShowTextLabelProperty, value);
        }

        public IBrush IconForeground
        {
            get => GetValue(IconForegroundProperty);
            set => SetValue(IconForegroundProperty, value);
        }

        public event EventHandler<IBrush>? ColorChanged;

        static ColorPickerDropdown()
        {
            // Sync SelectedColor -> SelectedColorValue
            SelectedColorProperty.Changed.AddClassHandler<ColorPickerDropdown>((s, e) =>
            {
                if (e.NewValue is SolidColorBrush brush)
                {
                    s.SelectedColorValue = brush.Color;
                }
            });

            // Sync SelectedColorValue -> SelectedColor
            SelectedColorValueProperty.Changed.AddClassHandler<ColorPickerDropdown>((s, e) =>
            {
                if (e.NewValue is Color color)
                {
                    var newBrush = new SolidColorBrush(color);
                    s.SelectedColor = newBrush;
                    s.UpdateIconForeground(color);
                    s.ColorChanged?.Invoke(s, newBrush);
                }
            });
        }

        public ColorPickerDropdown()
        {
            AvaloniaXamlLoader.Load(this);

            Loaded += (s, e) =>
            {
                UpdateIconForeground(SelectedColorValue);
            };
        }

        private void UpdateIconForeground(Color color)
        {
            IconForeground = IsLightColor(color)
                ? new SolidColorBrush(Color.FromArgb(255, 78, 78, 78))
                : new SolidColorBrush(Color.FromArgb(255, 222, 224, 225));
        }

        public static bool IsLightColor(Color color)
        {
            // Low-alpha colors show the checkered background through, treat as light
            if (color.A < 128)
            {
                return true;
            }

            double luminance = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;
            return luminance > 128;
        }

        private void OnDropdownButtonClick(object? sender, RoutedEventArgs e)
        {
            var popup = this.FindControl<Popup>("ColorPopup");
            if (popup != null)
            {
                popup.IsOpen = !popup.IsOpen;
            }
        }
    }
}