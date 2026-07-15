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
using System;
using System.Collections.Generic;
using AboutResources = ShareX.Properties.Resources;

namespace ShareX;

public partial class AboutWindow : Window
{
    private UpdateChecker? _updateChecker;
    private bool _checkUpdate;
    private bool _updateChecked;

    public AboutWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        ProductNameText.Text = Program.Title;
        CopyrightText.Text = "Copyright (c) 2007-2026 ShareX Team";
        SectionsControl.ItemsSource = CreateSections();

#if STEAM
        BuildText.Text = "Steam build";
        BuildText.IsVisible = true;
#elif MicrosoftStore
        BuildText.Text = "Microsoft Store build";
        BuildText.IsVisible = true;
#else
        if (!SystemOptions.DisableUpdateCheck)
        {
            _checkUpdate = true;
            UpdatePanel.IsVisible = true;
            UpdateStatusText.Text = "Checking for updates...";
        }
#endif

        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Activate();

        if (!_checkUpdate || _updateChecked)
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
                UpdateStatusText.Text = "Update check failed.";
                break;
            case UpdateStatus.UpdateAvailable:
                UpdateStatusText.IsVisible = false;
                UpdateAvailableButton.Content = "A newer version of ShareX is available";
                UpdateAvailableButton.IsVisible = true;
                break;
            case UpdateStatus.UpToDate:
                UpdateStatusText.Text = "ShareX is up to date.";
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

    private void OnUpdateAvailableClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_updateChecker == null)
        {
            return;
        }

        Program.MainForm.BeginInvoke((Action)(() => UpdateMessageBox.Start(_updateChecker)));
    }

    private static IReadOnlyList<AboutSection> CreateSections()
    {
        return
        [
            new AboutSection(AboutResources.AboutForm_AboutForm_Links,
            [
                Link(AboutResources.AboutForm_AboutForm_Website, Links.Website),
                Link(AboutResources.AboutForm_AboutForm_Project_page, Links.GitHub),
                Link(AboutResources.AboutForm_AboutForm_Changelog, Links.Changelog),
                Link(AboutResources.AboutForm_AboutForm_Privacy_policy, Links.PrivacyPolicy),
                Link(AboutResources.AboutForm_AboutForm_Donate, Links.Donate),
                Link("X", Links.X),
                Link("Discord", Links.Discord),
                Link("Reddit", Links.Reddit),
                Link("Steam", Links.Steam),
                Link("Microsoft Store", Links.MicrosoftStore)
            ]),
            new AboutSection(AboutResources.AboutForm_AboutForm_Team,
            [
                Link("Jaex", Links.Jaex),
                Link("McoreD", Links.McoreD)
            ]),
            new AboutSection(AboutResources.AboutForm_AboutForm_Translators,
            [
                Link(AboutResources.AboutForm_AboutForm_Language_tr, "https://github.com/Jaex"),
                Link(AboutResources.AboutForm_AboutForm_Language_de, "https://github.com/Starbug2"),
                Link(AboutResources.AboutForm_AboutForm_Language_de, "https://github.com/Kaeltis"),
                Link(AboutResources.AboutForm_AboutForm_Language_fr, "https://github.com/nwies"),
                Link(AboutResources.AboutForm_AboutForm_Language_fr, "https://github.com/Shadorc"),
                Link(AboutResources.AboutForm_AboutForm_Language_zh_CH, "https://github.com/jiajiechan"),
                Link(AboutResources.AboutForm_AboutForm_Language_hu, "https://github.com/devBluestar"),
                Link(AboutResources.AboutForm_AboutForm_Language_ko_KR, "https://github.com/123jimin"),
                Link(AboutResources.AboutForm_AboutForm_Language_es, "https://github.com/ovnisoftware"),
                Link(AboutResources.AboutForm_AboutForm_Language_nl_NL, "https://github.com/canihavesomecoffee"),
                Link(AboutResources.AboutForm_AboutForm_Language_pt_BR, "https://github.com/RockyTV"),
                Link(AboutResources.AboutForm_AboutForm_Language_pt_BR, "https://github.com/athosbr99"),
                Link(AboutResources.AboutForm_AboutForm_Language_vi_VN, "https://github.com/thanhpd"),
                Link(AboutResources.AboutForm_AboutForm_Language_ru, "https://github.com/L1Q"),
                Link(AboutResources.AboutForm_AboutForm_Language_zh_TW, "https://github.com/alantsai"),
                Link(AboutResources.AboutForm_AboutForm_Language_it_IT, "https://github.com/pjammo"),
                Link(AboutResources.AboutForm_AboutForm_Language_uk, "https://github.com/6c6c6"),
                Link(AboutResources.AboutForm_AboutForm_Language_id_ID, "https://github.com/Nicedward"),
                Link(AboutResources.AboutForm_AboutForm_Language_es_MX, "https://github.com/absay"),
                Link(AboutResources.AboutForm_AboutForm_Language_fa_IR, "https://github.com/pourmand1376"),
                Link(AboutResources.AboutForm_AboutForm_Language_pt_PT, "https://github.com/FarewellAngelina"),
                Link(AboutResources.AboutForm_AboutForm_Language_ja_JP, "https://github.com/kanaxx"),
                Link(AboutResources.AboutForm_AboutForm_Language_ro, "https://github.com/Edward205"),
                Link(AboutResources.AboutForm_AboutForm_Language_pl, "https://github.com/RikoDEV"),
                Link(AboutResources.AboutForm_AboutForm_Language_he_IL, "https://github.com/erelado"),
                Link(AboutResources.AboutForm_AboutForm_Language_ar_YE, "https://github.com/OthmanAliModaes")
            ]),
            new AboutSection(AboutResources.AboutForm_AboutForm_Credits,
            [
                Link("Avalonia UI", "https://avaloniaui.net"),
                Link("Json.NET", "https://github.com/JamesNK/Newtonsoft.Json"),
                Link("SSH.NET", "https://github.com/sshnet/SSH.NET"),
                Link("Lucide Icons", "https://lucide.dev"),
                Link("Fugue Icons", "http://p.yusukekamiyamane.com"),
                Link("ImageListView", "https://github.com/oozcitak/imagelistview"),
                Link("FFmpeg", "https://www.ffmpeg.org"),
                Link("Recorder devices", "https://github.com/rdp/screen-capture-recorder-to-video-windows-free"),
                Link("FluentFTP", "https://github.com/robinrodricks/FluentFTP"),
                Link("ZXing.Net", "https://github.com/micjahn/ZXing.Net"),
                Link("ExifTool", "https://exiftool.org")
            ])
        ];
    }

    private static AboutLinkItem Link(string label, string url) => new(label, url, new Uri(url));
}

public sealed record AboutSection(string Title, IReadOnlyList<AboutLinkItem> Items);

public sealed record AboutLinkItem(string Label, string DisplayText, Uri Uri);
