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
using System;
using LocalizedResources = ShareX.HelpersLib.Properties.Resources;

namespace ShareX.HelpersLib;

public partial class InputBoxWindow : Window
{
    public string? SubmittedText { get; private set; }

    public InputBoxWindow() : this("Input")
    {
    }

    public InputBoxWindow(
        string title,
        string? inputText = null,
        string? okText = null,
        string? cancelText = null)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        Title = $"ShareX - {title}";
        InputTextBox.Text = inputText ?? string.Empty;
        OKButton.Content = string.IsNullOrEmpty(okText)
            ? LocalizedResources.MyMessageBox_MyMessageBox_OK
            : okText;
        CancelButton.Content = string.IsNullOrEmpty(cancelText)
            ? LocalizedResources.MyMessageBox_MyMessageBox_Cancel
            : cancelText;

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        InputTextBox.Focus();
        InputTextBox.SelectAll();
    }

    private void OnOKClick(object? sender, RoutedEventArgs e)
    {
        SubmittedText = InputTextBox.Text ?? string.Empty;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
