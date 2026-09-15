#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Strings = ShareX.Tools.Localization.Strings;
using DrawingColor = System.Drawing.Color;

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
        Control primaryColor = ColorRow(Strings.MouseHighlighter_PrimaryColor, options.PrimaryColor,
            color => { options.PrimaryColor = color; Changed(); });
        Control secondaryColor = ColorRow(Strings.MouseHighlighter_SecondaryColor, options.SecondaryColor,
            color => { options.SecondaryColor = color; Changed(); });
        Control middleColor = ColorRow(Strings.MouseHighlighter_MiddleColor, options.MiddleColor,
            color => { options.MiddleColor = color; Changed(); });
        CheckBox primaryCrosshairs = Check(Strings.MouseHighlighter_PrimaryReleaseCrosshairs, options.ShowPrimaryReleaseCrosshairs,
            value => { options.ShowPrimaryReleaseCrosshairs = value; Changed(); });
        CheckBox secondaryCrosshairs = Check(Strings.MouseHighlighter_ReleaseCrosshairs, options.ShowSecondaryReleaseCrosshairs,
            value => { options.ShowSecondaryReleaseCrosshairs = value; Changed(); });
        CheckBox middleCrosshairs = Check(Strings.MouseHighlighter_MiddleReleaseCrosshairs, options.ShowMiddleReleaseCrosshairs,
            value => { options.ShowMiddleReleaseCrosshairs = value; Changed(); });

        ComboBox mode = new()
        {
            ItemsSource = new[] { Strings.MouseHighlighter_Circle, Strings.MouseHighlighter_Spotlight, Strings.MouseHighlighter_Ripple },
            SelectedIndex = (int)options.Mode,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        void UpdateMode()
        {
            bool isRipple = options.Mode == MouseHighlightMode.Ripple;
            circle.IsVisible = !isRipple;
            ripple.IsVisible = isRipple;
            primaryColor.IsVisible = secondaryColor.IsVisible = middleColor.IsVisible = options.Mode != MouseHighlightMode.Spotlight;
            primaryCrosshairs.IsVisible = secondaryCrosshairs.IsVisible = middleCrosshairs.IsVisible = isRipple;
        }
        mode.SelectionChanged += (_, _) =>
        {
            if (mode.SelectedIndex < 0) return;
            options.Mode = (MouseHighlightMode)mode.SelectedIndex;
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
            Text = Strings.MouseHighlighter_AlwaysColorHelp, TextWrapping = TextWrapping.Wrap,
            Foreground = Brushes.Gray, FontWeight = FontWeight.Normal
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
