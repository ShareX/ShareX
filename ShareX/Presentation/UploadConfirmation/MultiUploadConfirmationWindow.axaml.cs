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
using LocalizedResources = ShareX.Properties.Resources;

namespace ShareX;

public partial class MultiUploadConfirmationWindow : Window
{
    public bool IsConfirmed { get; private set; }
    public bool DontShowAgain => DontShowAgainCheckBox.IsChecked == true;

    public MultiUploadConfirmationWindow() : this(1)
    {
    }

    public MultiUploadConfirmationWindow(int fileCount)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        Title = $"ShareX - {LocalizedResources.UploadManager_IsUploadConfirmed_Upload_files}";
        MessageText.Text = string.Format(
            LocalizedResources.UploadManager_IsUploadConfirmed_Are_you_sure_you_want_to_upload__0__files_,
            fileCount);
        DontShowAgainCheckBox.Content =
            LocalizedResources.UploadManager_IsUploadConfirmed_Don_t_show_this_message_again_;

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e) => Activate();

    private void OnUploadClick(object? sender, RoutedEventArgs e)
    {
        IsConfirmed = true;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
