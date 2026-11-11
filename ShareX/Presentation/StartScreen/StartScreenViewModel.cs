#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Media;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Icons;
using ShareX.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using AvaloniaColor = Avalonia.Media.Color;

namespace ShareX;

public sealed class StartScreenViewModel : INotifyPropertyChanged
{
    private bool _startWithWindows;
    private bool _startWithWindowsEnabled;
    private string _startWithWindowsText = Strings.ApplicationSettingsForm_cbStartWithWindows_Text;

    private ApplicationConfig Settings => ApplicationState.Settings;

    public string WindowTitle => Strings.StartScreen_Welcome;
    public string ThankYouTitle => Strings.StartScreen_ThankYou;
    public string SettingsNote => Strings.StartScreen_SettingsNote;
    public string GetStartedText => Strings.StartScreen_GetStarted;
    public string LanguageLabel => RemoveTrailingColon(Strings.ApplicationSettingsWindow_LanguageLabel);
    public string ThemeLabel => RemoveTrailingColon(Strings.ApplicationSettingsWindow_ThemeLabel);
    public string FollowSystemAccentColorLabel => Strings.ApplicationSettingsWindow_FollowSystemAccentColor;
    public string AccentColorLabel => RemoveTrailingColon(Strings.ApplicationSettingsWindow_AccentColorLabel);

    public IReadOnlyList<StartScreenLanguageOption> LanguageOptions { get; } = CreateLanguageOptions();

    // "System" stands for UseSystemTheme rather than a theme name, so one choice covers both settings.
    internal const string SystemTheme = "System";

    public IReadOnlyList<StartScreenThemeOption> ThemeOptions { get; } =
    [
        new(SystemTheme, Strings.StartScreen_UseSystemSetting),
        new("Light", Strings.ApplicationSettingsWindow_Light),
        new("Dark", Strings.ApplicationSettingsWindow_Dark)
    ];

    // The first preset is the default accent color, so a fresh install starts with it selected.
    public IReadOnlyList<StartScreenAccentOption> AccentOptions { get; } =
        new[] { ApplicationThemeOptions.DefaultAccentColorHex, "#6366F1", "#8B5CF6", "#EC4899", "#EF4444", "#F97316", "#10B981", "#64748B" }
            .Select(x => new StartScreenAccentOption(AvaloniaColor.Parse(x)))
            .ToArray();

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

    public StartScreenThemeOption? SelectedTheme
    {
        get
        {
            string value = Settings.ThemeOptions.UseSystemTheme ? SystemTheme : NormalizeTheme(Settings.ThemeOptions.Theme);
            return ThemeOptions.FirstOrDefault(x => string.Equals(x.Value, value, StringComparison.Ordinal));
        }
        set
        {
            if (value == null || value == SelectedTheme)
            {
                return;
            }

            if (value.Value == SystemTheme)
            {
                Settings.ThemeOptions.UseSystemTheme = true;
            }
            else
            {
                Settings.ThemeOptions.Theme = value.Value;
                Settings.ThemeOptions.UseSystemTheme = false;
            }

            OnPropertyChanged();
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

    public string IconPackLabel => RemoveTrailingColon(Strings.ApplicationSettingsWindow_IconPackLabel);

    public IReadOnlyList<StartScreenIconPackOption> IconPackOptions { get; } =
    [
        new(IconPack.Modern, Strings.ApplicationSettingsWindow_IconPackModern),
        new(IconPack.Legacy, Strings.ApplicationSettingsWindow_IconPackLegacy)
    ];

    public StartScreenIconPackOption? SelectedIconPack
    {
        get => IconPackOptions.FirstOrDefault(x => x.Value == Settings.ThemeOptions.IconPack);
        set
        {
            if (value != null && Settings.ThemeOptions.IconPack != value.Value)
            {
                Settings.ThemeOptions.IconPack = value.Value;
                OnPropertyChanged();
            }
        }
    }

    public AvaloniaColor AccentColor
    {
        get => Settings.ThemeOptions.AccentColor;
        set
        {
            if (Settings.ThemeOptions.AccentColor != value)
            {
                Settings.ThemeOptions.AccentColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedAccent));
            }
        }
    }

    // Null when the accent is a custom color from the picker.
    public StartScreenAccentOption? SelectedAccent
    {
        get => AccentOptions.FirstOrDefault(x => x.Color == AccentColor);
        set
        {
            if (value != null)
            {
                AccentColor = value.Color;
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
                LanguageFlags.Get(x),
                x == SupportedLanguage.Automatic ? AppIcons.Language : null))
            .ToArray();

    private static string NormalizeTheme(string? theme) =>
        string.Equals(theme, "Light", StringComparison.OrdinalIgnoreCase) ? "Light" : "Dark";

    private static string RemoveTrailingColon(string text) => text.TrimEnd().TrimEnd(':', '：').TrimEnd();

    private void RefreshLocalizedText()
    {
        foreach (StartScreenLanguageOption language in LanguageOptions)
        {
            language.UpdateDisplayName(language.Value.GetLocalizedDescription());
        }

        ThemeOptions[0].UpdateDisplayName(Strings.StartScreen_UseSystemSetting);
        ThemeOptions[1].UpdateDisplayName(Strings.ApplicationSettingsWindow_Light);
        ThemeOptions[2].UpdateDisplayName(Strings.ApplicationSettingsWindow_Dark);
        IconPackOptions[0].UpdateDisplayName(Strings.ApplicationSettingsWindow_IconPackModern);
        IconPackOptions[1].UpdateDisplayName(Strings.ApplicationSettingsWindow_IconPackLegacy);

        RefreshStartWithWindows();
        OnPropertyChanged(string.Empty);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler? PropertyChanged;
}

public sealed class StartScreenLanguageOption : INotifyPropertyChanged
{
    private string _displayName;

    public SupportedLanguage Value { get; }
    public string DisplayName => _displayName;
    public IImage? Flag { get; }
    public string? IconGlyph { get; }

    public StartScreenLanguageOption(SupportedLanguage value, string displayName, IImage? flag, string? iconGlyph)
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

public sealed class StartScreenIconPackOption : INotifyPropertyChanged
{
    private string _displayName;

    public IconPack Value { get; }
    public string DisplayName => _displayName;

    public StartScreenIconPackOption(IconPack value, string displayName)
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

public sealed class StartScreenThemeOption : INotifyPropertyChanged
{
    private string _displayName;

    public string Value { get; }
    public string DisplayName => _displayName;
    public bool IsSystem => Value == StartScreenViewModel.SystemTheme;
    public bool IsLight => Value == "Light";
    public bool IsDark => Value == "Dark";

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

public sealed class StartScreenAccentOption
{
    public AvaloniaColor Color { get; }
    public IBrush Brush { get; }

    public StartScreenAccentOption(AvaloniaColor color)
    {
        Color = color;
        Brush = new SolidColorBrush(color);
    }
}
