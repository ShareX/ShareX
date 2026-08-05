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
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using System;
using System.Collections.Generic;

namespace ShareX;

public partial class AboutWindow : Window
{
    private UpdateChecker? _updateChecker;
    private bool _updateChecked;

    public AboutWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        ProductNameText.Text = Program.Title;
        CopyrightText.Text = Strings.AboutWindow_Copyright;
        SectionsControl.ItemsSource = CreateSections();

#if STEAM
        BuildText.Text = Strings.AboutWindow_SteamBuild;
        BuildText.IsVisible = true;
#elif MicrosoftStore
        BuildText.Text = Strings.AboutWindow_MicrosoftStoreBuild;
        BuildText.IsVisible = true;
#else
        if (!SystemOptions.DisableUpdateCheck)
        {
            UpdatePanel.IsVisible = true;
            UpdateStatusText.Text = Strings.AboutWindow_CheckingForUpdates;
        }
#endif

        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Activate();

        if (!UpdatePanel.IsVisible || _updateChecked)
        {
            return;
        }

        _updateChecked = true;
        _updateChecker = Program.UpdateManager.CreateUpdateChecker();
        await _updateChecker.CheckUpdateAsync();

        UpdateProgress.IsVisible = false;

        switch (_updateChecker.Status)
        {
            case UpdateStatus.UpdateCheckFailed:
                UpdateStatusText.Text = Strings.AboutWindow_UpdateCheckFailed;
                break;
            case UpdateStatus.UpdateAvailable:
                UpdateStatusText.IsVisible = false;
                UpdateAvailableButton.Content = Strings.AboutWindow_NewVersionAvailable;
                UpdateAvailableButton.IsVisible = true;
                break;
            case UpdateStatus.UpToDate:
                UpdateStatusText.Text = Strings.AboutWindow_UpToDate;
                break;
        }
    }

    private void OnLogoPressed(object? sender, PointerPressedEventArgs e)
    {
        LogoImage.IsVisible = false;
        AnimationContainer.IsVisible = true;
        LogoAnimation.Start();
        TaskHelpers.PlayNotificationSoundAsync(NotificationSound.ActionCompleted);
        e.Handled = true;
    }

    private void OnAnimationPressed(object? sender, PointerPressedEventArgs e)
    {
        LogoAnimation.TogglePaused();
        e.Handled = true;
    }

    private async void OnUpdateAvailableClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_updateChecker == null)
        {
            return;
        }

        await UpdateMessageWindow.StartAsync(_updateChecker);
    }

    private static IReadOnlyList<AboutSection> CreateSections()
    {
        return
        [
            new AboutSection(Strings.AboutWindow_Team,
            [
                Link("Jaex", Links.Jaex),
                Link("McoreD", Links.McoreD)
            ]),
            new AboutSection(Strings.AboutWindow_Links,
            [
                Link(Strings.AboutWindow_Website, Links.Website),
                Link(Strings.AboutWindow_ProjectPage, Links.GitHub),
                Link(Strings.AboutWindow_Changelog, Links.Changelog),
                Link(Strings.AboutWindow_PrivacyPolicy, Links.PrivacyPolicy),
                Link(Strings.AboutWindow_Donate, Links.Donate),
                Link("X", Links.X),
                Link("Discord", Links.Discord),
                Link("Reddit", Links.Reddit),
                Link("Steam", Links.Steam),
                Link("Microsoft Store", Links.MicrosoftStore)
            ])
        ];
    }

    private static AboutLinkItem Link(string label, string url) => new(label, url, new Uri(url));
}

public sealed record AboutSection(string Title, IReadOnlyList<AboutLinkItem> Items);

public sealed record AboutLinkItem(string Label, string DisplayText, Uri Uri);
