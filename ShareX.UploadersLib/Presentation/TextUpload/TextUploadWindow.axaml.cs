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
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.Threading.Tasks;

namespace ShareX.UploadersLib;

public partial class TextUploadWindow : Window
{
    private string? _submittedContent;
    private readonly bool _selectInitialContent;

    public TextUploadWindow() : this(null)
    {
    }

    public TextUploadWindow(string? content)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        if (string.IsNullOrEmpty(content) && ClipboardHelpers.ContainsText())
        {
            content = ClipboardHelpers.GetText();
        }

        if (!string.IsNullOrEmpty(content))
        {
            ContentTextBox.Text = content;
            _selectInitialContent = true;
        }

        UpdateCharacterCount();
        Opened += OnOpened;
    }

    public static Task<string?> ShowAsync(string? content = null)
    {
        AvaloniaBootstrapper.EnsureInitialized();
        TaskCompletionSource<string?> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                TextUploadWindow window = new(content);
                window.Closed += (_, _) => completion.TrySetResult(window._submittedContent);
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
        ContentTextBox.Focus();

        if (_selectInitialContent)
        {
            ContentTextBox.SelectAll();
        }
    }

    private void OnContentChanged(object? sender, TextChangedEventArgs e) => UpdateCharacterCount();

    private void UpdateCharacterCount()
    {
        int length = ContentTextBox.Text?.Length ?? 0;
        CharacterCountText.Text = $"{length:N0} {(length == 1 ? "character" : "characters")}";
    }

    private void OnUploadClick(object? sender, RoutedEventArgs e)
    {
        _submittedContent = ContentTextBox.Text ?? string.Empty;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
