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
using Avalonia.Layout;
using Avalonia.Media;
using DrawingColor = System.Drawing.Color;
using ShareX.Tools.Localization;

namespace ShareX.Tools;

public sealed class MouseHighlighterSettingsControl : UserControl
{
    public MouseHighlighterSettingsControl(MouseHighlighterOptions options, Action? settingsChanged = null)
    {
        options.Validate();
        void Changed()
        {
            options.Validate();
            MouseHighlighterManager.RefreshOptions(options);
            settingsChanged?.Invoke();
        }

        StackPanel panel = new() { Spacing = 12 };
        StackPanel circle = new() { Spacing = 10 };
        StackPanel ripple = new() { Spacing = 10 };
        CheckBox primaryCrosshairs = Check(Strings.MouseHighlighter_PrimaryReleaseCrosshairs, options.ShowPrimaryReleaseCrosshairs,
            value => { options.ShowPrimaryReleaseCrosshairs = value; Changed(); });
        CheckBox secondaryCrosshairs = Check(Strings.MouseHighlighter_ReleaseCrosshairs, options.ShowSecondaryReleaseCrosshairs,
            value => { options.ShowSecondaryReleaseCrosshairs = value; Changed(); });
        CheckBox middleCrosshairs = Check(Strings.MouseHighlighter_MiddleReleaseCrosshairs, options.ShowMiddleReleaseCrosshairs,
            value => { options.ShowMiddleReleaseCrosshairs = value; Changed(); });
        primaryCrosshairs.IsEnabled = options.HighlightPrimaryClicks;
        secondaryCrosshairs.IsEnabled = options.HighlightSecondaryClicks;
        middleCrosshairs.IsEnabled = options.HighlightMiddleClicks;
        Control primaryColor = EnabledColorRow(Strings.MouseHighlighter_PrimaryColor, options.HighlightPrimaryClicks,
            value => { options.HighlightPrimaryClicks = value; primaryCrosshairs.IsEnabled = value; Changed(); }, options.PrimaryColor,
            color => { options.PrimaryColor = color; Changed(); });
        Control secondaryColor = EnabledColorRow(Strings.MouseHighlighter_SecondaryColor, options.HighlightSecondaryClicks,
            value => { options.HighlightSecondaryClicks = value; secondaryCrosshairs.IsEnabled = value; Changed(); }, options.SecondaryColor,
            color => { options.SecondaryColor = color; Changed(); });
        Control middleColor = EnabledColorRow(Strings.MouseHighlighter_MiddleColor, options.HighlightMiddleClicks,
            value => { options.HighlightMiddleClicks = value; middleCrosshairs.IsEnabled = value; Changed(); }, options.MiddleColor,
            color => { options.MiddleColor = color; Changed(); });

        ComboBox mode = new()
        {
            ItemsSource = new[] { Strings.MouseHighlighter_Circle, Strings.MouseHighlighter_Ripple },
            SelectedIndex = options.Mode == MouseHighlightMode.Circle ? 0 : 1,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        void UpdateMode()
        {
            bool isRipple = options.Mode == MouseHighlightMode.Ripple;
            circle.IsVisible = !isRipple;
            ripple.IsVisible = isRipple;
            primaryCrosshairs.IsVisible = secondaryCrosshairs.IsVisible = middleCrosshairs.IsVisible = isRipple;
        }
        mode.SelectionChanged += (_, _) =>
        {
            if (mode.SelectedIndex < 0) return;
            options.Mode = mode.SelectedIndex == 0 ? MouseHighlightMode.Circle : MouseHighlightMode.Ripple;
            UpdateMode();
            Changed();
        };
        panel.Children.Add(Row(Strings.MouseHighlighter_Mode, mode));
        panel.Children.Add(primaryColor);
        panel.Children.Add(primaryCrosshairs);
        panel.Children.Add(secondaryColor);
        panel.Children.Add(secondaryCrosshairs);
        panel.Children.Add(middleColor);
        panel.Children.Add(middleCrosshairs);
        circle.Children.Add(ColorRow(Strings.MouseHighlighter_AlwaysColor, options.AlwaysColor,
            color => { options.AlwaysColor = color; Changed(); }));
        circle.Children.Add(new TextBlock
        {
            Text = Strings.MouseHighlighter_AlwaysColorHelp,
            TextWrapping = TextWrapping.Wrap,
            Foreground = Brushes.Gray,
            FontWeight = FontWeight.Normal
        });
        circle.Children.Add(NumberRow(Strings.MouseHighlighter_Radius, options.Radius, 5, 500, 1,
            value => { options.Radius = (int)value; Changed(); }));
        circle.Children.Add(NumberRow(Strings.MouseHighlighter_FadeDelay, options.FadeDelay, 0, 10000, 10,
            value => { options.FadeDelay = (int)value; Changed(); }));
        circle.Children.Add(NumberRow(Strings.MouseHighlighter_FadeDuration, options.FadeDuration, 0, 10000, 10,
            value => { options.FadeDuration = (int)value; Changed(); }));
        ripple.Children.Add(NumberRow(Strings.MouseHighlighter_RippleSize, options.RippleSize, 10, 300, 1,
            value => { options.RippleSize = (int)value; Changed(); }));
        ripple.Children.Add(NumberRow(Strings.MouseHighlighter_RippleIntensity, (decimal)options.RippleIntensity, 0.15m, 1.35m, 0.05m,
            value => { options.RippleIntensity = (double)value; Changed(); }, "0.00"));
        ripple.Children.Add(NumberRow(Strings.MouseHighlighter_RippleDuration, options.RippleDuration, 60, 2000, 10,
            value => { options.RippleDuration = (int)value; Changed(); }));
        ripple.Children.Add(Check(Strings.MouseHighlighter_FollowCursor, options.FollowCursorWhileHeld,
            value => { options.FollowCursorWhileHeld = value; Changed(); }));
        panel.Children.Add(circle);
        panel.Children.Add(ripple);
        panel.Children.Add(Check(Strings.MouseHighlighter_AutoActivate, options.AutoActivate,
            value => { options.AutoActivate = value; Changed(); }));
        UpdateMode();
        Content = panel;
    }

    private static Control NumberRow(string label, decimal value, decimal min, decimal max, decimal increment,
        Action<decimal> changed, string format = "0")
    {
        NumericUpDown number = new() { Minimum = min, Maximum = max, Increment = increment, Value = value, FormatString = format };
        number.ValueChanged += (_, _) => { if (number.Value.HasValue) changed(number.Value.Value); };
        return Row(label, number);
    }

    private static Control ColorRow(string label, DrawingColor color, Action<DrawingColor> changed)
    {
        ColorView picker = new() { Color = Color.FromArgb(color.A, color.R, color.G, color.B), IsAlphaEnabled = true, IsAlphaVisible = true };
        Border swatch = new() { Width = 28, Height = 20, Background = new SolidColorBrush(picker.Color), BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
        Button button = new() { Content = swatch, HorizontalAlignment = HorizontalAlignment.Left, Flyout = new Flyout { Content = picker } };
        picker.ColorChanged += (_, _) =>
        {
            Color selected = picker.Color;
            swatch.Background = new SolidColorBrush(selected);
            changed(DrawingColor.FromArgb(selected.A, selected.R, selected.G, selected.B));
        };
        return Row(label, button);
    }

    private static Control EnabledColorRow(string label, bool enabled, Action<bool> enabledChanged,
        DrawingColor color, Action<DrawingColor> colorChanged)
    {
        ColorView picker = new() { Color = Color.FromArgb(color.A, color.R, color.G, color.B), IsAlphaEnabled = true, IsAlphaVisible = true };
        Border swatch = new() { Width = 28, Height = 20, Background = new SolidColorBrush(picker.Color), BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
        Button button = new() { Content = swatch, HorizontalAlignment = HorizontalAlignment.Left, Flyout = new Flyout { Content = picker }, IsEnabled = enabled };
        picker.ColorChanged += (_, _) =>
        {
            Color selected = picker.Color;
            swatch.Background = new SolidColorBrush(selected);
            colorChanged(DrawingColor.FromArgb(selected.A, selected.R, selected.G, selected.B));
        };

        CheckBox check = Check(label, enabled, value =>
        {
            button.IsEnabled = value;
            enabledChanged(value);
        });
        Grid row = new() { ColumnDefinitions = new ColumnDefinitions("*,180"), ColumnSpacing = 12 };
        row.Children.Add(check);
        Grid.SetColumn(button, 1);
        row.Children.Add(button);
        return row;
    }

    private static CheckBox Check(string label, bool value, Action<bool> changed)
    {
        CheckBox check = new() { Content = new TextBlock { Text = label, TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Normal }, IsChecked = value };
        check.IsCheckedChanged += (_, _) => changed(check.IsChecked == true);
        return check;
    }

    private static Control Row(string label, Control editor)
    {
        Grid row = new() { ColumnDefinitions = new ColumnDefinitions("*,180"), ColumnSpacing = 12 };
        row.Children.Add(new TextBlock { Text = label, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeight.Normal });
        Grid.SetColumn(editor, 1);
        row.Children.Add(editor);
        return row;
    }
}
