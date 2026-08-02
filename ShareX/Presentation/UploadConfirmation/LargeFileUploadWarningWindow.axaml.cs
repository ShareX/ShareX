#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Theming;
using ShareX.Localization;
using System;

namespace ShareX;

public partial class LargeFileUploadWarningWindow : Window
{
    public bool ShouldContinue { get; private set; }
    public bool DontShowAgain => DontShowAgainCheckBox.IsChecked == true;

    public LargeFileUploadWarningWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        MessageText.Text = Strings.LargeFileUploadWarningWindow_Message;

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e) => Activate();

    private void OnContinueClick(object? sender, RoutedEventArgs e)
    {
        ShouldContinue = true;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
