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

using Avalonia.Platform;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
using AvaloniaColor = Avalonia.Media.Color;

namespace ShareX;

public sealed class StartScreenViewModel : INotifyPropertyChanged, IDisposable
{
    private bool _startWithWindows;
    private bool _startWithWindowsEnabled;
    private string _startWithWindowsText = Strings.ApplicationSettingsForm_cbStartWithWindows_Text;

    private ApplicationConfig Settings => Program.Settings;

    public string WindowTitle => Strings.StartScreen_Welcome;
    public string WelcomeTitle => Strings.StartScreen_Welcome;
    public string WelcomeSubtitle => Strings.StartScreen_WelcomeSubtitle;
    public string CaptureFeature => Strings.MainMenuBuilder_Capture;
    public string AutomateFeature => Strings.MainMenuBuilder_Workflows;
    public string ShareFeature => Strings.MainMenuBuilder_Tools;
    public string PlatformNote => Strings.StartScreen_PlatformNote;
    public string PersonalizeTitle => Strings.StartScreen_PersonalizeTitle;
    public string PersonalizeSubtitle => Strings.StartScreen_PersonalizeSubtitle;
    public string SettingsNote => Strings.StartScreen_SettingsNote;
    public string GetStartedText => Strings.StartScreen_GetStarted;
    public string LanguageLabel => RemoveTrailingColon(Strings.ApplicationSettingsWindow_LanguageLabel);
    public string FollowSystemThemeLabel => Strings.ApplicationSettingsWindow_FollowSystemTheme;
    public string ThemeLabel => RemoveTrailingColon(Strings.ApplicationSettingsWindow_ThemeLabel);
    public string FollowSystemAccentColorLabel => Strings.ApplicationSettingsWindow_FollowSystemAccentColor;
    public string AccentColorLabel => RemoveTrailingColon(Strings.ApplicationSettingsWindow_AccentColorLabel);

    public IReadOnlyList<StartScreenLanguageOption> LanguageOptions { get; } = CreateLanguageOptions();
    public IReadOnlyList<StartScreenThemeOption> ThemeOptions { get; } =
    [
        new("Dark", Strings.ApplicationSettingsWindow_Dark),
        new("Light", Strings.ApplicationSettingsWindow_Light)
    ];

    public StartScreenViewModel()
    {
        RefreshLocalizedText();
    }

    public StartScreenLanguageOption? SelectedLanguage
    {
        get => LanguageOptions.FirstOrDefault(x => x.Value == Settings.Language);
        set
        {
            if (value == null || value.Value == Settings.Language)
            {
                return;
            }

            Settings.Language = value.Value;
            LanguageHelper.ChangeLanguage(value.Value);
            CultureInfo culture = value.Value == SupportedLanguage.Automatic
                ? CultureInfo.InstalledUICulture
                : CultureInfo.GetCultureInfo(LanguageHelper.GetCultureName(value.Value));
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            RefreshLocalizedText();
        }
    }

    public bool UseSystemTheme
    {
        get => Settings.ThemeOptions.UseSystemTheme;
        set
        {
            if (Settings.ThemeOptions.UseSystemTheme != value)
            {
                Settings.ThemeOptions.UseSystemTheme = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEditTheme));
            }
        }
    }

    public bool CanEditTheme => !UseSystemTheme;

    public StartScreenThemeOption? SelectedTheme
    {
        get => ThemeOptions.FirstOrDefault(x => string.Equals(x.Value, NormalizeTheme(Settings.ThemeOptions.Theme), StringComparison.Ordinal));
        set
        {
            if (value != null && !string.Equals(Settings.ThemeOptions.Theme, value.Value, StringComparison.Ordinal))
            {
                Settings.ThemeOptions.Theme = value.Value;
                OnPropertyChanged();
            }
        }
    }

    public bool UseSystemAccentColor
    {
        get => Settings.ThemeOptions.UseSystemAccentColor;
        set
        {
            if (Settings.ThemeOptions.UseSystemAccentColor != value)
            {
                Settings.ThemeOptions.UseSystemAccentColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEditAccentColor));
            }
        }
    }

    public bool CanEditAccentColor => !UseSystemAccentColor;

    public AvaloniaColor AccentColor
    {
        get => Settings.ThemeOptions.AccentColor;
        set
        {
            if (Settings.ThemeOptions.AccentColor != value)
            {
                Settings.ThemeOptions.AccentColor = value;
                OnPropertyChanged();
            }
        }
    }

    public bool StartWithWindows
    {
        get => _startWithWindows;
        set
        {
            if (!_startWithWindowsEnabled || _startWithWindows == value)
            {
                return;
            }

            try
            {
                StartupManager.State = value ? StartupState.Enabled : StartupState.Disabled;
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            RefreshStartWithWindows();
        }
    }

    public bool StartWithWindowsEnabled => _startWithWindowsEnabled;
    public string StartWithWindowsText => _startWithWindowsText;

    private void RefreshStartWithWindows()
    {
        _startWithWindowsText = Strings.ApplicationSettingsForm_cbStartWithWindows_Text;
        _startWithWindowsEnabled = false;

        try
        {
            StartupState state = StartupManager.State;
            _startWithWindows = state == StartupState.Enabled || state == StartupState.EnabledByPolicy;

            if (state == StartupState.DisabledByUser)
            {
                _startWithWindowsText = Strings.ApplicationSettingsForm_cbStartWithWindows_DisabledByUser_Text;
            }
            else if (state == StartupState.DisabledByPolicy)
            {
                _startWithWindowsText = Strings.ApplicationSettingsForm_cbStartWithWindows_DisabledByPolicy_Text;
            }
            else if (state == StartupState.EnabledByPolicy)
            {
                _startWithWindowsText = Strings.ApplicationSettingsForm_cbStartWithWindows_EnabledByPolicy_Text;
            }
            else
            {
                _startWithWindowsEnabled = true;
            }
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
        }

        OnPropertyChanged(nameof(StartWithWindows));
        OnPropertyChanged(nameof(StartWithWindowsEnabled));
        OnPropertyChanged(nameof(StartWithWindowsText));
    }

    private static IReadOnlyList<StartScreenLanguageOption> CreateLanguageOptions() =>
        Helpers.GetEnums<SupportedLanguage>()
            .Select(x => new StartScreenLanguageOption(
                x,
                x.GetLocalizedDescription(),
                LoadLanguageFlag(x),
                x == SupportedLanguage.Automatic ? LucideIcons.languages : null))
            .ToArray();

    private static AvaloniaBitmap? LoadLanguageFlag(SupportedLanguage language)
    {
        if (language == SupportedLanguage.Automatic)
        {
            return null;
        }

        string countryCode = LanguageHelper.GetCultureName(language).Split('-')[1].ToLowerInvariant();
        using Stream stream = AssetLoader.Open(new Uri($"avares://ShareX/Resources/Flags/{countryCode}.png"));
        return new AvaloniaBitmap(stream);
    }

    private static string NormalizeTheme(string? theme) =>
        string.Equals(theme, "Light", StringComparison.OrdinalIgnoreCase) ? "Light" : "Dark";

    private static string RemoveTrailingColon(string text) => text.TrimEnd().TrimEnd(':', '：').TrimEnd();

    private void RefreshLocalizedText()
    {
        foreach (StartScreenLanguageOption language in LanguageOptions)
        {
            language.UpdateDisplayName(language.Value.GetLocalizedDescription());
        }

        ThemeOptions[0].UpdateDisplayName(Strings.ApplicationSettingsWindow_Dark);
        ThemeOptions[1].UpdateDisplayName(Strings.ApplicationSettingsWindow_Light);

        RefreshStartWithWindows();
        OnPropertyChanged(string.Empty);
    }

    private void DisposeLanguageOptions()
    {
        foreach (StartScreenLanguageOption language in LanguageOptions)
        {
            language.Flag?.Dispose();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Dispose()
    {
        DisposeLanguageOptions();
    }
}

public sealed class StartScreenLanguageOption : INotifyPropertyChanged
{
    private string _displayName;

    public SupportedLanguage Value { get; }
    public string DisplayName => _displayName;
    public AvaloniaBitmap? Flag { get; }
    public string? IconGlyph { get; }

    public StartScreenLanguageOption(SupportedLanguage value, string displayName, AvaloniaBitmap? flag, string? iconGlyph)
    {
        Value = value;
        _displayName = displayName;
        Flag = flag;
        IconGlyph = iconGlyph;
    }

    public void UpdateDisplayName(string displayName)
    {
        if (_displayName != displayName)
        {
            _displayName = displayName;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public sealed class StartScreenThemeOption : INotifyPropertyChanged
{
    private string _displayName;

    public string Value { get; }
    public string DisplayName => _displayName;

    public StartScreenThemeOption(string value, string displayName)
    {
        Value = value;
        _displayName = displayName;
    }

    public void UpdateDisplayName(string displayName)
    {
        if (_displayName != displayName)
        {
            _displayName = displayName;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
