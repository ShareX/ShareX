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
using System.IO;

namespace ShareX.HelpersLib;

public partial class ErrorWindow : Window
{
    public bool IsUnhandledException { get; }
    public string? LogFilePath { get; }
    public string? BugReportPath { get; }

    public ErrorWindow() : this("Error", "An unexpected error occurred.", null, null, false)
    {
    }

    public ErrorWindow(Exception error, string? logFilePath, string? bugReportPath)
        : this(error.Message, error.ToString(), logFilePath, bugReportPath)
    {
    }

    public ErrorWindow(string errorTitle, string errorMessage, string? logFilePath, string? bugReportPath,
        bool unhandledException = true)
    {
        IsUnhandledException = unhandledException;
        LogFilePath = logFilePath;
        BugReportPath = bugReportPath;

        if (IsUnhandledException)
        {
            DebugHelper.WriteException(errorMessage, "Unhandled exception");
        }

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        ErrorTitleText.Text = errorTitle;
        ExceptionTextBox.Text = errorMessage;
        BugReportButton.IsVisible = !string.IsNullOrEmpty(BugReportPath);
        OpenLogButton.IsVisible = !string.IsNullOrEmpty(LogFilePath) && File.Exists(LogFilePath);
        ContinueButton.IsVisible = IsUnhandledException;
        QuitButton.IsVisible = IsUnhandledException;
        OKButton.IsVisible = !IsUnhandledException;

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        ExceptionTextBox.Focus();
        ExceptionTextBox.CaretIndex = ExceptionTextBox.Text?.Length ?? 0;
    }

    private void OnSendBugReportClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(BugReportPath))
        {
            URLHelpers.OpenURL(BugReportPath);
        }
    }

    private void OnOpenLogClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(LogFilePath))
        {
            FileHelpers.OpenFile(LogFilePath);
        }
    }

    private void OnContinueClick(object? sender, RoutedEventArgs e)
    {
        DebugHelper.WriteLine("ShareX continue.");
        Close();
    }

    private void OnQuitClick(object? sender, RoutedEventArgs e)
    {
        DebugHelper.WriteLine("ShareX closing. Reason: Unhandled exception.");
        Close();
        System.Windows.Forms.Application.Exit();
    }

    private void OnOKClick(object? sender, RoutedEventArgs e) => Close();
}
