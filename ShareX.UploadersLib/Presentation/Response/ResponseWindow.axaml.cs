#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Newtonsoft.Json;
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.Text;
using LocalizedResources = ShareX.UploadersLib.Properties.Resources;

namespace ShareX.UploadersLib;

public partial class ResponseWindow : Window
{
    private static readonly object InstanceLock = new();
    private static ResponseWindow? _instance;

    public UploadResult Result { get; private set; }

    public ResponseWindow() : this(new UploadResult())
    {
    }

    public ResponseWindow(UploadResult result)
    {
        Result = result;

        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        SectionList.SelectedIndex = 0;
        UpdateResult(result);

        Opened += OnOpened;
        Closed += OnClosed;
    }

    public static void ShowInstance(UploadResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        AvaloniaBootstrapper.EnsureInitialized();
        Dispatcher.UIThread.Post(() =>
        {
            lock (InstanceLock)
            {
                if (_instance == null)
                {
                    _instance = new ResponseWindow(result);
                    _instance.Show();
                }
                else
                {
                    _instance.UpdateResult(result);
                }

                if (_instance.WindowState == Avalonia.Controls.WindowState.Minimized)
                {
                    _instance.WindowState = Avalonia.Controls.WindowState.Normal;
                }

                _instance.Activate();
            }
        });
    }

    private void OnOpened(object? sender, EventArgs e) => Activate();

    private void OnClosed(object? sender, EventArgs e)
    {
        lock (InstanceLock)
        {
            if (ReferenceEquals(_instance, this))
            {
                _instance = null;
            }
        }
    }

    private void UpdateResult(UploadResult result)
    {
        Result = result;

        ResultTextBox.Text = BuildResultText(result);
        ResultTextBox.CaretIndex = 0;

        ResponseInfo? responseInfo = result.ResponseInfo;
        ResponseInfoTextBox.Text = responseInfo != null
            ? BuildResponseInfoText(responseInfo)
            : "No response information is available.";
        ResponseInfoTextBox.CaretIndex = 0;

        ResponseTextBox.Text = responseInfo?.ResponseText ?? result.Response ?? string.Empty;
        ResponseTextBox.CaretIndex = 0;

        bool hasShortenedUrl = !string.IsNullOrEmpty(result.ShortenedURL);
        bool hasUrl = !string.IsNullOrEmpty(result.URL);
        bool hasThumbnailUrl = !string.IsNullOrEmpty(result.ThumbnailURL);
        bool hasDeletionUrl = !string.IsNullOrEmpty(result.DeletionURL);

        CopyShortenedUrlButton.IsVisible = hasShortenedUrl;
        CopyUrlButton.IsVisible = hasUrl;
        OpenUrlButton.IsVisible = hasUrl;
        CopyThumbnailUrlButton.IsVisible = hasThumbnailUrl;
        CopyDeletionUrlButton.IsVisible = hasDeletionUrl;
        OpenResponseUrlButton.IsVisible = !string.IsNullOrEmpty(responseInfo?.ResponseURL);

        ResponseSummaryText.Text = result.IsError
            ? "The upload completed with an error."
            : "Inspect the URLs and response returned by the destination.";

        FormatStatusText.IsVisible = false;
    }

    private static string BuildResultText(UploadResult result)
    {
        StringBuilder text = new();
        AppendInfo(text, LocalizedResources.ShortenedURL, result.ShortenedURL);
        AppendInfo(text, LocalizedResources.URL, result.URL);
        AppendInfo(text, LocalizedResources.ThumbnailURL, result.ThumbnailURL);
        AppendInfo(text, LocalizedResources.DeletionURL, result.DeletionURL);

        if (result.IsError)
        {
            AppendInfo(text, LocalizedResources.Error, result.ErrorsToString());
        }

        return text.Length > 0 ? text.ToString() : "No result details are available.";
    }

    private static string BuildResponseInfoText(ResponseInfo responseInfo)
    {
        StringBuilder text = new();
        AppendInfo(text, LocalizedResources.StatusCode,
            $"({(int)responseInfo.StatusCode}) {responseInfo.StatusDescription}");
        AppendInfo(text, LocalizedResources.ResponseURL, responseInfo.ResponseURL);

        if (responseInfo.Headers is { Count: > 0 })
        {
            AppendInfo(text, LocalizedResources.Headers,
                responseInfo.Headers.ToString().TrimEnd('\r', '\n'));
        }

        AppendInfo(text, LocalizedResources.ResponseText, responseInfo.ResponseText);
        return text.ToString();
    }

    private static void AppendInfo(StringBuilder text, string name, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        if (text.Length > 0)
        {
            text.AppendLine();
            text.AppendLine();
        }

        text.Append(name);
        text.AppendLine(":");
        text.Append(value);
    }

    private void OnSectionSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        int selectedIndex = SectionList.SelectedIndex;
        ResultPanel.IsVisible = selectedIndex == 0;
        ResponseInfoPanel.IsVisible = selectedIndex == 1;
        ResponseTextPanel.IsVisible = selectedIndex == 2;
    }

    private async void OnCopyShortenedUrlClick(object? sender, RoutedEventArgs e) =>
        await CopyTextAsync(Result.ShortenedURL);

    private async void OnCopyUrlClick(object? sender, RoutedEventArgs e) =>
        await CopyTextAsync(Result.URL);

    private async void OnCopyThumbnailUrlClick(object? sender, RoutedEventArgs e) =>
        await CopyTextAsync(Result.ThumbnailURL);

    private async void OnCopyDeletionUrlClick(object? sender, RoutedEventArgs e) =>
        await CopyTextAsync(Result.DeletionURL);

    private async void OnCopyResponseInfoClick(object? sender, RoutedEventArgs e) =>
        await CopyTextAsync(ResponseInfoTextBox.Text);

    private async void OnCopyResponseTextClick(object? sender, RoutedEventArgs e) =>
        await CopyTextAsync(ResponseTextBox.Text);

    private void OnOpenUrlClick(object? sender, RoutedEventArgs e) => OpenUrl(Result.URL);

    private void OnOpenResponseUrlClick(object? sender, RoutedEventArgs e) =>
        OpenUrl(Result.ResponseInfo?.ResponseURL);

    private static void OpenUrl(string? url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            URLHelpers.OpenURL(url);
        }
    }

    private async System.Threading.Tasks.Task CopyTextAsync(string? text)
    {
        if (!string.IsNullOrEmpty(text) && Clipboard != null)
        {
            await Clipboard.SetTextAsync(text);
        }
    }

    private void OnJsonFormatClick(object? sender, RoutedEventArgs e)
    {
        FormatResponse(
            response => Helpers.JSONFormat(response, Formatting.Indented),
            LocalizedResources.FormattingFailed_JSON);
    }

    private void OnXmlFormatClick(object? sender, RoutedEventArgs e)
    {
        FormatResponse(Helpers.XMLFormat, LocalizedResources.FormattingFailed_XML);
    }

    private void FormatResponse(Func<string, string> formatter, string failureMessage)
    {
        string response = ResponseTextBox.Text ?? string.Empty;
        if (string.IsNullOrEmpty(response))
        {
            return;
        }

        try
        {
            ResponseTextBox.Text = formatter(response);
            ResponseTextBox.CaretIndex = 0;
            FormatStatusText.IsVisible = false;
        }
        catch
        {
            FormatStatusText.Text = failureMessage;
            FormatStatusText.IsVisible = true;
        }
    }
}
