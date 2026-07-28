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

namespace ShareX.UploadersLib;

public sealed record EmailWindowResult(string ToEmail, string Subject, string Body);

public partial class EmailWindow : Window
{
    public EmailWindowResult? SubmittedResult { get; private set; }

    public EmailWindow() : this(string.Empty, string.Empty, string.Empty)
    {
    }

    public EmailWindow(string? toEmail, string? subject, string? body)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        ToEmailTextBox.Text = toEmail ?? string.Empty;
        SubjectTextBox.Text = subject ?? string.Empty;
        MessageTextBox.Text = body ?? string.Empty;

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        MessageTextBox.Focus();
        MessageTextBox.CaretIndex = MessageTextBox.Text?.Length ?? 0;
    }

    private void OnSendClick(object? sender, RoutedEventArgs e)
    {
        SubmittedResult = new EmailWindowResult(
            ToEmailTextBox.Text ?? string.Empty,
            SubjectTextBox.Text ?? string.Empty,
            MessageTextBox.Text ?? string.Empty);
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
