#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Threading.Tasks;

namespace ShareX;

public partial class ShortenURLWindow : Window
{
    private string? _submittedURL;
    private readonly bool _selectInitialURL;

    public ShortenURLWindow() : this(null)
    {
    }

    public ShortenURLWindow(string? initialURL)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        if (!string.IsNullOrEmpty(initialURL))
        {
            URLTextBox.Text = initialURL;
            _selectInitialURL = true;
        }

        UpdateValidation();
        Opened += OnOpened;
    }

    public static Task<string?> ShowAsync(string? initialURL = null)
    {
        TaskCompletionSource<string?> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                ShortenURLWindow window = new(initialURL);
                window.Closed += (_, _) => completion.TrySetResult(window._submittedURL);
                window.Show();
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        });

        return completion.Task;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        URLTextBox.Focus();

        if (_selectInitialURL)
        {
            URLTextBox.SelectAll();
        }
    }

    private void OnURLChanged(object? sender, TextChangedEventArgs e) => UpdateValidation();

    private void UpdateValidation()
    {
        string url = URLTextBox.Text?.Trim() ?? string.Empty;
        bool isValid = URLInputValidation.IsSupported(url);
        ShortenButton.IsEnabled = isValid;
        ValidationText.IsVisible = url.Length > 0 && !isValid;
    }

    private void OnShortenClick(object? sender, RoutedEventArgs e)
    {
        string url = URLTextBox.Text?.Trim() ?? string.Empty;
        if (!URLInputValidation.IsSupported(url))
        {
            UpdateValidation();
            return;
        }

        _submittedURL = url;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
