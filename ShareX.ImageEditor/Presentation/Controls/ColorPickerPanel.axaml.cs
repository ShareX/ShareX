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
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ShareX.AvaloniaUI.Windows;
using ShareX.ImageEditor.Integration;
using DrawingColor = System.Drawing.Color;

namespace ShareX.ImageEditor.Presentation.Controls
{
    public partial class ColorPickerPanel : UserControl
    {
        public static readonly StyledProperty<Color> SelectedColorValueProperty =
            AvaloniaProperty.Register<ColorPickerPanel, Color>(
                nameof(SelectedColorValue),
                defaultValue: Colors.Red);

        public static readonly StyledProperty<HsvColor> SelectedHsvColorProperty =
            AvaloniaProperty.Register<ColorPickerPanel, HsvColor>(
                nameof(SelectedHsvColor));

        public static readonly StyledProperty<Popup?> HostPopupProperty =
            AvaloniaProperty.Register<ColorPickerPanel, Popup?>(nameof(HostPopup));

        private const int RecentColorCount = 5;

        private bool _isScreenColorPickerOpen;
        private Color _colorWhenOpened;

        static ColorPickerPanel()
        {
            HostPopupProperty.Changed.AddClassHandler<ColorPickerPanel>((panel, e) =>
                panel.OnHostPopupChanged(e.OldValue as Popup, e.NewValue as Popup));
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

        public Popup? HostPopup
        {
            get => GetValue(HostPopupProperty);
            set => SetValue(HostPopupProperty, value);
        }

        public ColorPickerPanel()
        {
            AvaloniaXamlLoader.Load(this);
            UpdateScreenPickerAvailability();
            Loaded += OnLoaded;
            RefreshRecentColors();
        }

        private void OnHostPopupChanged(Popup? oldPopup, Popup? newPopup)
        {
            if (oldPopup != null)
            {
                oldPopup.Opened -= OnHostPopupOpened;
                oldPopup.Closed -= OnHostPopupClosed;
            }

            if (newPopup != null)
            {
                newPopup.Opened += OnHostPopupOpened;
                newPopup.Closed += OnHostPopupClosed;
            }
        }

        private void OnHostPopupOpened(object? sender, EventArgs e)
        {
            _colorWhenOpened = SelectedColorValue;
            RefreshRecentColors();
        }

        // Only the color the popup is closed with counts as used, so dragging across the spectrum does not fill the list.
        private void OnHostPopupClosed(object? sender, EventArgs e)
        {
            if (SelectedColorValue != _colorWhenOpened)
            {
                AddRecentColor(SelectedColorValue);
            }
        }

        private static void AddRecentColor(Color color)
        {
            EditorRecentColors.Add(DrawingColor.FromArgb(color.A, color.R, color.G, color.B));
        }

        private void RefreshRecentColors()
        {
            var panel = this.FindControl<StackPanel>("RecentColorsPanel");
            if (panel == null)
            {
                return;
            }

            panel.Children.Clear();
            List<DrawingColor> recentColors = EditorRecentColors.Colors;

            for (int i = 0; i < RecentColorCount; i++)
            {
                if (i < recentColors.Count)
                {
                    DrawingColor color = recentColors[i];
                    panel.Children.Add(CreateRecentColorButton(Color.FromArgb(color.A, color.R, color.G, color.B)));
                }
                else
                {
                    panel.Children.Add(new Border { Classes = { "recent-color-empty" } });
                }
            }
        }

        private Button CreateRecentColorButton(Color color)
        {
            string hex = color.A < 255
                ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
                : $"#{color.R:X2}{color.G:X2}{color.B:X2}";

            var button = new Button
            {
                Classes = { "transparent-button", "recent-color" },
                Content = new Grid
                {
                    Children =
                    {
                        new Border { Classes = { "checkered-surface" }, CornerRadius = new CornerRadius(4), ClipToBounds = true },
                        new Border { Classes = { "recent-color-fill" }, Background = new SolidColorBrush(color) }
                    }
                }
            };

            ToolTip.SetTip(button, hex);
            AutomationProperties.SetName(button, hex);
            button.Click += (_, _) => SelectedColorValue = color;
            return button;
        }

        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            UpdateScreenPickerAvailability();
            var colorView = this.FindControl<ColorView>("ColorViewControl");
            if (colorView?.PaletteColors == null)
            {
                return;
            }

            var paletteColors = colorView.PaletteColors.ToList();
            int count = paletteColors.Count;
            if (count < 13)
            {
                return;
            }

            paletteColors[count - 13] = Color.FromArgb(255, 235, 235, 235);
            paletteColors[count - 7] = Color.FromArgb(255, 20, 20, 20);
            paletteColors[count - 1] = Colors.Transparent;
            colorView.PaletteColors = paletteColors;
        }

        private async void OnScreenColorPickerClick(object? sender, RoutedEventArgs e)
        {
            if (_isScreenColorPickerOpen || !UpdateScreenPickerAvailability())
            {
                return;
            }

            Window? owner = HostPopup?.PlacementTarget is Visual placementTarget
                ? TopLevel.GetTopLevel(placementTarget) as Window
                : TopLevel.GetTopLevel(this) as Window;

            if (HostPopup != null)
            {
                HostPopup.IsOpen = false;
            }

            _isScreenColorPickerOpen = true;

            try
            {
                var picker = new ScreenColorPickerWindow();
                ScreenColorPickerResult? result = await picker.PickAsync(owner);

                if (result != null)
                {
                    SelectedColorValue = result.Color;
                    AddRecentColor(result.Color);
                }
            }
            finally
            {
                _isScreenColorPickerOpen = false;
            }
        }

        private bool UpdateScreenPickerAvailability()
        {
            var support = ScreenColorPickerAvailability.Support;
            this.FindControl<Button>("PickScreenButton")!.IsEnabled = support.IsSupported;
            ToolTip.SetTip(this.FindControl<Border>("ScreenPickerAvailability")!, support.IsSupported ? null : support.Reason);
            return support.IsSupported;
        }
    }
}
