#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ShareX;

public partial class DebugLogWindow : Window
{
    private readonly Logger _logger;
    private readonly Action<string>? _uploadRequested;
    private readonly string _startupPath;

    public DebugLogWindow()
    {
        _logger = new Logger();
        _startupPath = AppDomain.CurrentDomain.BaseDirectory;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
    }

    public DebugLogWindow(Logger logger, Action<string>? uploadRequested, string uploadWarning) : this()
    {
        _logger = logger;
        _uploadRequested = uploadRequested;

        LogTextBox.Text = _logger.ToString() ?? string.Empty;
        LogTextBox.CaretIndex = LogTextBox.Text.Length;
        OpenLogFileButton.IsEnabled = !string.IsNullOrEmpty(_logger.LogFilePath);
        UploadLogButton.IsVisible = _uploadRequested != null;
        RunningFromButton.Content = _startupPath;
        UploadWarningText.Text = uploadWarning;

        _logger.MessageAdded += OnLoggerMessageAdded;
        Closed += OnClosed;
    }

    private void OnLoggerMessageAdded(string message)
    {
        Dispatcher.UIThread.Post(() => AppendMessage(message));
    }

    private void AppendMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        string text = LogTextBox.Text ?? string.Empty;
        int selectionStart = LogTextBox.SelectionStart;
        int selectionEnd = LogTextBox.SelectionEnd;
        bool hasSelection = selectionStart != selectionEnd;
        bool wasAtEnd = LogTextBox.CaretIndex >= text.Length;

        LogTextBox.Text = text + message;

        if (hasSelection)
        {
            LogTextBox.SelectionStart = selectionStart;
            LogTextBox.SelectionEnd = selectionEnd;
        }
        else if (wasAtEnd)
        {
            LogTextBox.CaretIndex = LogTextBox.Text.Length;
        }
    }

    private async void OnCopyAllClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string text = (LogTextBox.Text ?? string.Empty).Trim();

        if (!string.IsNullOrEmpty(text) && Clipboard != null)
        {
            await Clipboard.SetTextAsync(text);
        }
    }

    private void OnOpenLogFileClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_logger.LogFilePath))
        {
            FileHelpers.OpenFile(_logger.LogFilePath);
        }
    }

    private void OnLoadedAssembliesClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        StringBuilder builder = new();
        string? directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!assembly.IsDynamic
                && directoryPath != null
                && assembly.Location.StartsWith(directoryPath, StringComparison.OrdinalIgnoreCase))
            {
                builder.AppendLine(assembly.ManifestModule.Name);
            }
        }

        DebugHelper.WriteLine($"Loaded assemblies:\r\n{builder.ToString().Trim()}");
    }

    private void OnUploadLogClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_uploadRequested != null && !string.IsNullOrEmpty(LogTextBox.Text))
        {
            UploadConfirmationOverlay.IsVisible = true;
        }
    }

    private void OnCancelUploadClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        UploadConfirmationOverlay.IsVisible = false;
    }

    private void OnConfirmUploadClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string text = LogTextBox.Text ?? string.Empty;
        UploadConfirmationOverlay.IsVisible = false;

        if (!string.IsNullOrEmpty(text))
        {
            _uploadRequested?.Invoke(text);
        }
    }

    private void OnRunningFromClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        FileHelpers.OpenFolder(_startupPath);
    }

    private void OnLogDoubleTapped(object? sender, TappedEventArgs e)
    {
        string? url = GetUrlAtCaret(LogTextBox.Text ?? string.Empty, LogTextBox.CaretIndex);

        if (url != null)
        {
            URLHelpers.OpenURL(url);
            e.Handled = true;
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _logger.MessageAdded -= OnLoggerMessageAdded;
    }

    private static string? GetUrlAtCaret(string text, int caretIndex)
    {
        if (text.Length == 0)
        {
            return null;
        }

        int index = Math.Clamp(caretIndex, 0, text.Length - 1);
        int start = index;
        int end = index;

        while (start > 0 && !char.IsWhiteSpace(text[start - 1]))
        {
            start--;
        }

        while (end < text.Length && !char.IsWhiteSpace(text[end]))
        {
            end++;
        }

        string candidate = text[start..end].Trim('"', '\'', '(', ')', '[', ']', '{', '}', ',', ';');
        return Uri.TryCreate(candidate, UriKind.Absolute, out Uri? uri)
            && uri.Scheme is "http" or "https"
            ? candidate
            : null;
    }
}
