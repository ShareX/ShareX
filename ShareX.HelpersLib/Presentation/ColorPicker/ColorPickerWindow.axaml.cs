#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
    This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License.
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using ShareX.AvaloniaUI.Windows;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrawingColor = System.Drawing.Color;

namespace ShareX.HelpersLib;

public partial class ColorPickerWindow : Window
{
    private readonly ColorPickerOptions _options;
    private readonly ScreenColorPickerOptions _screenColorPickerOptions;
    private readonly Func<PointInfo>? _openScreenColorPicker;
    private readonly bool _selectionMode;
    private DrawingColor _currentColor = DrawingColor.Red;
    private DrawingColor _previousColor = DrawingColor.Red;
    private bool _hasPreviousColor;
    private bool _updatingControls;
    private bool _initialized;

    public DrawingColor? SelectedColor { get; private set; }

    public ColorPickerWindow() : this(new ColorPickerOptions(), new ScreenColorPickerOptions())
    {
    }

    public ColorPickerWindow(ColorPickerOptions? options, ScreenColorPickerOptions? screenColorPickerOptions)
        : this(DrawingColor.Red, options, screenColorPickerOptions, null, false)
    {
    }

    internal ColorPickerWindow(
        DrawingColor currentColor,
        ColorPickerOptions? options,
        ScreenColorPickerOptions? screenColorPickerOptions,
        Func<PointInfo>? openScreenColorPicker)
        : this(currentColor, options, screenColorPickerOptions, openScreenColorPicker, true)
    {
    }

    private ColorPickerWindow(
        DrawingColor currentColor,
        ColorPickerOptions? options,
        ScreenColorPickerOptions? screenColorPickerOptions,
        Func<PointInfo>? openScreenColorPicker,
        bool selectionMode)
    {
        _options = options ?? new ColorPickerOptions();
        _screenColorPickerOptions = screenColorPickerOptions ?? new ScreenColorPickerOptions();
        _openScreenColorPicker = openScreenColorPicker;
        _selectionMode = selectionMode;
        _currentColor = currentColor;
        _previousColor = currentColor;
        _hasPreviousColor = selectionMode;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Topmost = selectionMode;
        ShowInTaskbar = !selectionMode;
        AcceptButton.IsVisible = selectionMode;
        CancelButton.IsVisible = selectionMode;
        Picker.ColorChanged += OnPickerColorChanged;
        KeyDown += OnKeyDown;
        Opened += OnOpened;
        Closed += (_, _) =>
        {
            if (!_selectionMode)
            {
                AddRecentColor(_currentColor);
            }
        };

        if (_options.RecentColorsSelected) RecentPalette.IsChecked = true;
        else StandardPalette.IsChecked = true;

        SetColor(_currentColor);
        _initialized = true;
        RebuildPalette();
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        Activate();
        if (Clipboard == null) return;
        try
        {
            string? text = await Clipboard.TryGetTextAsync();
            if (ColorHelpers.ParseColor(text ?? string.Empty, out DrawingColor color))
            {
                _previousColor = _currentColor;
                _hasPreviousColor = true;
                SetColor(color);
            }
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex, "Failed to read a color from the clipboard.");
        }
    }

    private void OnPickerColorChanged(object? sender, DrawingColor color) => SetColor(color, false);

    private void SetColor(DrawingColor color, bool updatePicker = true)
    {
        _currentColor = color;
        _updatingControls = true;
        if (updatePicker) Picker.SelectedColor = color;

        HSB hsb = color;
        CMYK cmyk = color;
        RedValue.Value = color.R;
        GreenValue.Value = color.G;
        BlueValue.Value = color.B;
        AlphaValue.Value = color.A;
        HueValue.Value = (decimal)Math.Round(hsb.Hue360);
        SaturationValue.Value = (decimal)Math.Round(hsb.Saturation100);
        BrightnessValue.Value = (decimal)Math.Round(hsb.Brightness100);
        CyanValue.Value = (decimal)Math.Round(cmyk.Cyan100);
        MagentaValue.Value = (decimal)Math.Round(cmyk.Magenta100);
        YellowValue.Value = (decimal)Math.Round(cmyk.Yellow100);
        KeyValue.Value = (decimal)Math.Round(cmyk.Key100);
        HexValue.Text = "#" + ColorHelpers.ColorToHex(color);
        DecimalValue.Value = ColorHelpers.ColorToDecimal(color);
        ColorNameText.Text = ColorHelpers.GetColorName(color);
        NewColorPreview.Background = new SolidColorBrush(ToAvalonia(color));
        OldColorPreview.Background = new SolidColorBrush(ToAvalonia(_previousColor));
        OldColorPreview.IsVisible = _hasPreviousColor;
        _updatingControls = false;
    }

    private void OnPickerModeChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (Picker == null || sender is not ComboBox comboBox) return;
        Picker.DrawStyle = comboBox.SelectedIndex switch
        {
            1 => DrawStyle.Saturation,
            2 => DrawStyle.Brightness,
            3 => DrawStyle.Red,
            4 => DrawStyle.Green,
            5 => DrawStyle.Blue,
            _ => DrawStyle.Hue
        };
    }

    private void OnRgbValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_updatingControls || !_initialized) return;
        SetColor(DrawingColor.FromArgb(Value(AlphaValue), Value(RedValue), Value(GreenValue), Value(BlueValue)));
    }

    private void OnHsbValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_updatingControls || !_initialized) return;
        SetColor(new HSB(Value(HueValue), Value(SaturationValue), Value(BrightnessValue), Value(AlphaValue)).ToColor());
    }

    private void OnCmykValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_updatingControls || !_initialized) return;
        SetColor(new CMYK(Value(CyanValue), Value(MagentaValue), Value(YellowValue), Value(KeyValue), Value(AlphaValue)).ToColor());
    }

    private void OnHexTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_updatingControls || !_initialized) return;
        try
        {
            string text = HexValue.Text?.Trim() ?? string.Empty;
            DrawingColor parsed = text.TrimStart('#').Length == 8
                ? ColorHelpers.HexToColor(text, ColorFormat.RGBA)
                : ColorHelpers.HexToColor(text);
            if (!parsed.IsEmpty) SetColor(parsed);
        }
        catch
        {
        }
    }

    private void OnDecimalValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_updatingControls || !_initialized) return;
        SetColor(ColorHelpers.DecimalToColor(Value(DecimalValue)));
    }

    private async void OnPickScreenClick(object? sender, RoutedEventArgs e)
    {
        DrawingColor? selectedColor = null;

        if (_openScreenColorPicker != null)
        {
            try
            {
                Hide();
                await Task.Delay(250);
                PointInfo? pointInfo = _openScreenColorPicker();
                selectedColor = pointInfo?.Color;
            }
            catch (Exception exception)
            {
                DebugHelper.WriteException(exception, "Failed to pick a color from the screen.");
            }
            finally
            {
                Show();
                Activate();
            }
        }
        else
        {
            ScreenColorPickerWindow picker = new(_screenColorPickerOptions);
            ScreenColorPickerResult? result = await picker.PickAsync(this);

            if (result != null)
            {
                Avalonia.Media.Color selected = result.Color;
                selectedColor = DrawingColor.FromArgb(selected.A, selected.R, selected.G, selected.B);
            }
        }

        if (selectedColor.HasValue)
        {
            _previousColor = _currentColor;
            _hasPreviousColor = true;
            SetColor(selectedColor.Value);
            AddRecentColor(_currentColor);
            RebuildPalette();
        }
    }

    private async void OnClipboardClick(object? sender, RoutedEventArgs e)
    {
        if (Clipboard == null) return;
        try
        {
            string? text = await Clipboard.TryGetTextAsync();
            if (ColorHelpers.ParseColor(text ?? string.Empty, out DrawingColor color))
            {
                _previousColor = _currentColor;
                _hasPreviousColor = true;
                SetColor(color);
            }
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex, "Failed to read a color from the clipboard.");
        }
    }

    private void OnPreviousColorPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!_hasPreviousColor || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        DrawingColor old = _currentColor;
        SetColor(_previousColor);
        _previousColor = old;
        OldColorPreview.Background = new SolidColorBrush(ToAvalonia(_previousColor));
        e.Handled = true;
    }

    private void OnPaletteChanged(object? sender, RoutedEventArgs e)
    {
        if (!_initialized || sender is not RadioButton { IsChecked: true }) return;
        _options.RecentColorsSelected = RecentPalette.IsChecked == true;
        RebuildPalette();
    }

    private void RebuildPalette()
    {
        if (PalettePanel == null) return;
        PalettePanel.Children.Clear();
        IReadOnlyList<DrawingColor> colors = _options.RecentColorsSelected
            ? HelpersOptions.RecentColors
            : ColorHelpers.StandardColors;

        foreach (DrawingColor color in colors.Take(HelpersOptions.RecentColorsMax))
        {
            Border swatch = new()
            {
                Width = 19,
                Height = 19,
                Background = new SolidColorBrush(ToAvalonia(color)),
                CornerRadius = new CornerRadius(3)
            };
            Button button = new()
            {
                Content = swatch,
                Width = 21,
                Height = 21,
                Padding = new Thickness(1),
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Tag = color
            };
            button.Click += OnPaletteColorClick;
            PalettePanel.Children.Add(button);
        }
    }

    private void OnPaletteColorClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: DrawingColor color })
        {
            _previousColor = _currentColor;
            _hasPreviousColor = true;
            SetColor(color);
        }
    }

    private void AddRecentColor(DrawingColor color)
    {
        HelpersOptions.RecentColors.Remove(color);
        HelpersOptions.RecentColors.Insert(0, color);
        if (HelpersOptions.RecentColors.Count > HelpersOptions.RecentColorsMax)
        {
            HelpersOptions.RecentColors.RemoveRange(HelpersOptions.RecentColorsMax,
                HelpersOptions.RecentColors.Count - HelpersOptions.RecentColorsMax);
        }
    }

    private async Task CopyAsync(string text)
    {
        AddRecentColor(_currentColor);
        if (_options.RecentColorsSelected) RebuildPalette();
        if (Clipboard != null) await Clipboard.SetTextAsync(text);
    }

    private async void OnCopyAllClick(object? sender, RoutedEventArgs e)
    {
        MyColor color = _currentColor;
        string text = color.ToString();
        await CopyAsync(text);
    }

    private async void OnCopyRgbClick(object? sender, RoutedEventArgs e) => await CopyAsync($"{_currentColor.R}, {_currentColor.G}, {_currentColor.B}");
    private async void OnCopyHexClick(object? sender, RoutedEventArgs e) => await CopyAsync("#" + ColorHelpers.ColorToHex(_currentColor));
    private async void OnCopyDecimalClick(object? sender, RoutedEventArgs e) => await CopyAsync(ColorHelpers.ColorToDecimal(_currentColor).ToString());

    private async void OnCopyHsbClick(object? sender, RoutedEventArgs e)
    {
        HSB hsb = _currentColor;
        await CopyAsync($"{hsb.Hue360:0.0}°, {hsb.Saturation100:0.0}%, {hsb.Brightness100:0.0}%");
    }

    private async void OnCopyCmykClick(object? sender, RoutedEventArgs e)
    {
        CMYK cmyk = _currentColor;
        await CopyAsync($"{cmyk.Cyan100:0.0}%, {cmyk.Magenta100:0.0}%, {cmyk.Yellow100:0.0}%, {cmyk.Key100:0.0}%");
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape) return;
        Close();
        e.Handled = true;
    }

    private void OnAcceptClick(object? sender, RoutedEventArgs e)
    {
        SelectedColor = _currentColor;
        AddRecentColor(_currentColor);
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();

    private static int Value(NumericUpDown input) => (int)(input.Value ?? 0);
    private static Avalonia.Media.Color ToAvalonia(DrawingColor color) => Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
}
