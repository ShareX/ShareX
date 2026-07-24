#region License Information (GPL v3)

/*
    ShareX - A program developed by ShareX Team
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Theming;
using System;
using DrawingImage = System.Drawing.Image;
using LocalizedResources = ShareX.HelpersLib.Properties.Resources;

namespace ShareX.HelpersLib;

public partial class PrintWindow : Window
{
    private PrintHelper? _printHelper;
    private PrintSettings _printSettings = new();

    public PrintWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        PrintButton.IsEnabled = false;
        PrintButton.Content = LocalizedResources.PrintForm_LoadSettings_Print;
        CancelButton.Content = LocalizedResources.MyMessageBox_MyMessageBox_Cancel;

        Opened += (_, _) => Activate();
        Closed += (_, _) => _printHelper?.Dispose();
    }

    public PrintWindow(DrawingImage image, PrintSettings settings, bool previewOnly = false)
        : this()
    {
        _printSettings = settings;
        _printHelper = new PrintHelper(image)
        {
            Settings = settings
        };

        MarginInput.Value = settings.Margin;
        AutoRotateCheckBox.IsChecked = settings.AutoRotateImage;
        AutoScaleCheckBox.IsChecked = settings.AutoScaleImage;
        AllowEnlargeCheckBox.IsChecked = settings.AllowEnlargeImage;
        CenterImageCheckBox.IsChecked = settings.CenterImage;

        PrintButton.IsEnabled = !previewOnly;
        PrintButton.Content = LocalizedResources.PrintForm_LoadSettings_Print +
            (settings.ShowPrintDialog ? "..." : string.Empty);
        CancelButton.Content = LocalizedResources.MyMessageBox_MyMessageBox_Cancel;
        UpdateScaleDependentOptions();
    }

    private void OnMarginChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (e.NewValue.HasValue)
        {
            _printSettings.Margin = (int)e.NewValue.Value;
        }
    }

    private void OnAutoRotateChanged(object? sender, RoutedEventArgs e) =>
        _printSettings.AutoRotateImage = AutoRotateCheckBox.IsChecked == true;

    private void OnAutoScaleChanged(object? sender, RoutedEventArgs e)
    {
        _printSettings.AutoScaleImage = AutoScaleCheckBox.IsChecked == true;
        UpdateScaleDependentOptions();
    }

    private void OnAllowEnlargeChanged(object? sender, RoutedEventArgs e) =>
        _printSettings.AllowEnlargeImage = AllowEnlargeCheckBox.IsChecked == true;

    private void OnCenterImageChanged(object? sender, RoutedEventArgs e) =>
        _printSettings.CenterImage = CenterImageCheckBox.IsChecked == true;

    private void UpdateScaleDependentOptions()
    {
        bool enabled = _printSettings.AutoScaleImage;
        AllowEnlargeCheckBox.IsEnabled = enabled;
        CenterImageCheckBox.IsEnabled = enabled;
    }

    private void OnPreviewClick(object? sender, RoutedEventArgs e)
    {
        if (_printHelper != null)
        {
            RunNativeDialog(_printHelper.ShowPreview);
        }
    }

    private void OnPrintClick(object? sender, RoutedEventArgs e)
    {
        if (_printHelper != null)
        {
            RunNativeDialog(() => _printHelper.Print());
            Close();
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();

    private void RunNativeDialog(Action action)
    {
        IsEnabled = false;

        try
        {
            action();
        }
        finally
        {
            IsEnabled = true;
            Activate();
        }
    }
}
