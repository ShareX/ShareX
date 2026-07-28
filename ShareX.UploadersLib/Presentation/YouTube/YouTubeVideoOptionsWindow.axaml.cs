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
using ShareX.HelpersLib;
using System;

namespace ShareX.UploadersLib;

public sealed record YouTubeVideoOptionsWindowResult(
    string Title,
    string Description,
    YouTubeVideoPrivacy Visibility);

public partial class YouTubeVideoOptionsWindow : Window
{
    public YouTubeVideoOptionsWindowResult? SubmittedResult { get; private set; }

    public YouTubeVideoOptionsWindow()
        : this(string.Empty, string.Empty, YouTubeVideoPrivacy.Private)
    {
    }

    public YouTubeVideoOptionsWindow(
        string? title,
        string? description,
        YouTubeVideoPrivacy visibility)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        TitleTextBox.Text = title ?? string.Empty;
        DescriptionTextBox.Text = description ?? string.Empty;
        VisibilityComboBox.ItemsSource = Helpers.GetLocalizedEnumDescriptions<YouTubeVideoPrivacy>();
        VisibilityComboBox.SelectedIndex = (int)visibility;

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        TitleTextBox.Focus();
        TitleTextBox.SelectAll();
    }

    private void OnConfirmClick(object? sender, RoutedEventArgs e)
    {
        int selectedIndex = VisibilityComboBox.SelectedIndex;
        if (!Enum.IsDefined(typeof(YouTubeVideoPrivacy), selectedIndex))
        {
            selectedIndex = (int)YouTubeVideoPrivacy.Private;
        }

        SubmittedResult = new YouTubeVideoOptionsWindowResult(
            TitleTextBox.Text ?? string.Empty,
            DescriptionTextBox.Text ?? string.Empty,
            (YouTubeVideoPrivacy)selectedIndex);
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();
}
