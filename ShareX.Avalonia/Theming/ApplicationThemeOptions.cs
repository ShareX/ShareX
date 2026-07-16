#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media;

namespace ShareX.AvaloniaUI.Theming;

public sealed class ApplicationThemeOptions : INotifyPropertyChanged
{
    public const string DefaultTheme = "Dark";
    public const string DefaultAccentColorHex = "#3E83F2";

    private string _theme = DefaultTheme;
    private bool _useSystemTheme = true;
    private string _accentColorHex = DefaultAccentColorHex;
    private bool _useSystemAccentColor = true;

    public string Theme
    {
        get => _theme;
        set => SetField(ref _theme, value);
    }

    public bool UseSystemTheme
    {
        get => _useSystemTheme;
        set => SetField(ref _useSystemTheme, value);
    }

    public string AccentColorHex
    {
        get => _accentColorHex;
        set => SetField(ref _accentColorHex, value);
    }

    public Color AccentColor
    {
        get => Color.TryParse(AccentColorHex, out Color color) && color.A > 0
            ? color
            : Color.Parse(DefaultAccentColorHex);
        set => AccentColorHex = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
    }

    // Newtonsoft.Json convention used by ApplicationConfig without coupling this project to Newtonsoft.Json.
    public bool ShouldSerializeAccentColor() => false;

    public bool UseSystemAccentColor
    {
        get => _useSystemAccentColor;
        set => SetField(ref _useSystemAccentColor, value);
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == nameof(AccentColorHex))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AccentColor)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
