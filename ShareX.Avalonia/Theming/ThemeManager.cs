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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using System.ComponentModel;

namespace ShareX.AvaloniaUI.Theming
{
    public static class ThemeManager
    {
        public static readonly ThemeVariant ShareXDark = new ThemeVariant("ShareXDark", ThemeVariant.Dark);
        public static readonly ThemeVariant ShareXLight = new ThemeVariant("ShareXLight", ThemeVariant.Light);
        private static ThemeVariant _currentTheme = ShareXDark;
        private static ApplicationThemeOptions? _options;
        private static IPlatformSettings? _platformSettings;

        public static event EventHandler<ThemeVariant>? ThemeChanged;
        public static event EventHandler<Color>? AccentColorChanged;

        public static void Configure(ApplicationThemeOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            if (!ReferenceEquals(_options, options))
            {
                if (_options != null)
                {
                    _options.PropertyChanged -= OnOptionsPropertyChanged;
                }

                _options = options;
                _options.PropertyChanged += OnOptionsPropertyChanged;
            }

            RunOnUIThread(ApplyConfiguredSettings);
        }

        public static void SetTheme(ThemeVariant theme, object? target = null)
        {
            _currentTheme = theme;

            if (target is Application app)
            {
                app.RequestedThemeVariant = theme;

                if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    foreach (Window window in desktop.Windows)
                    {
                        window.RequestedThemeVariant = theme;
                    }
                }
            }
            else if (target is Window window)
            {
                window.RequestedThemeVariant = theme;
            }
            else if (target is ThemeVariantScope scope)
            {
                scope.RequestedThemeVariant = theme;
            }

            ThemeChanged?.Invoke(null, theme);
        }

        public static ThemeVariant GetCurrentTheme()
        {
            return _currentTheme;
        }

        public static void Refresh()
        {
            RunOnUIThread(ApplyConfiguredSettings);
        }

        private static void OnOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RunOnUIThread(ApplyConfiguredSettings);
        }

        private static void ApplyConfiguredSettings()
        {
            if (_options == null)
            {
                return;
            }

            Application? application = Application.Current;
            SetPlatformSettings((_options.UseSystemTheme || _options.UseSystemAccentColor)
                ? application?.PlatformSettings
                : null);

            PlatformColorValues? colorValues = _platformSettings?.GetColorValues();
            ThemeVariant theme = _options.UseSystemTheme
                ? MapSystemTheme(colorValues, _options.Theme)
                : MapConfiguredTheme(_options.Theme);

            SetTheme(theme, application);

            Color accentColor = _options.AccentColor;
            if (_options.UseSystemAccentColor && colorValues is { AccentColor1.A: > 0 })
            {
                accentColor = colorValues.AccentColor1;
            }

            ApplyAccentColor(application, accentColor);
        }

        private static void SetPlatformSettings(IPlatformSettings? platformSettings)
        {
            if (ReferenceEquals(_platformSettings, platformSettings))
            {
                return;
            }

            if (_platformSettings != null)
            {
                _platformSettings.ColorValuesChanged -= OnPlatformColorValuesChanged;
            }

            _platformSettings = platformSettings;

            if (_platformSettings != null)
            {
                _platformSettings.ColorValuesChanged += OnPlatformColorValuesChanged;
            }
        }

        private static void OnPlatformColorValuesChanged(object? sender, PlatformColorValues e)
        {
            RunOnUIThread(ApplyConfiguredSettings);
        }

        private static ThemeVariant MapSystemTheme(PlatformColorValues? colorValues, string configuredTheme)
        {
            if (colorValues != null)
            {
                return IsLightTheme(colorValues.ThemeVariant.ToString()) ? ShareXLight : ShareXDark;
            }

            return MapConfiguredTheme(configuredTheme);
        }

        private static ThemeVariant MapConfiguredTheme(string? theme)
        {
            return IsLightTheme(theme) ? ShareXLight : ShareXDark;
        }

        private static bool IsLightTheme(string? theme)
        {
            return !string.IsNullOrWhiteSpace(theme) &&
                theme.Contains("Light", StringComparison.OrdinalIgnoreCase);
        }

        private static void ApplyAccentColor(Application? application, Color startColor)
        {
            if (application == null)
            {
                return;
            }

            Color endColor = DarkenColor(startColor, 0.10);
            Color foregroundColor = GetAccentForegroundColor(startColor, endColor);

            application.Resources["SystemAccentColor"] = startColor;
            application.Resources["ShareX.Color.Accent.Start"] = startColor;
            application.Resources["ShareX.Color.Accent.End"] = endColor;
            application.Resources["ShareX.Color.Accent.Foreground"] = foregroundColor;
            application.Resources["ShareX.Brush.Accent.Start"] = new SolidColorBrush(startColor);
            application.Resources["ShareX.Brush.Accent.Foreground"] = new SolidColorBrush(foregroundColor);
            application.Resources["ShareX.Brush.Accent"] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(startColor, 0),
                    new GradientStop(endColor, 1)
                }
            };

            AccentColorChanged?.Invoke(null, startColor);
        }

        private static Color DarkenColor(Color color, double amount)
        {
            double factor = Math.Clamp(1 - amount, 0, 1);
            return Color.FromArgb(color.A,
                (byte)Math.Round(color.R * factor),
                (byte)Math.Round(color.G * factor),
                (byte)Math.Round(color.B * factor));
        }

        private static Color GetAccentForegroundColor(Color startColor, Color endColor)
        {
            Color light = Color.Parse("#D8DADB");
            Color dark = Color.Parse("#4E4E4E");
            double lightContrast = Math.Min(GetContrastRatio(light, startColor), GetContrastRatio(light, endColor));
            double darkContrast = Math.Min(GetContrastRatio(dark, startColor), GetContrastRatio(dark, endColor));
            return darkContrast >= lightContrast * 1.75 ? dark : light;
        }

        private static double GetContrastRatio(Color first, Color second)
        {
            double firstLuminance = GetRelativeLuminance(first);
            double secondLuminance = GetRelativeLuminance(second);
            return (Math.Max(firstLuminance, secondLuminance) + 0.05) /
                (Math.Min(firstLuminance, secondLuminance) + 0.05);
        }

        private static double GetRelativeLuminance(Color color)
        {
            static double Linearize(byte channel)
            {
                double value = channel / 255d;
                return value <= 0.03928 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
            }

            return (0.2126 * Linearize(color.R)) +
                (0.7152 * Linearize(color.G)) +
                (0.0722 * Linearize(color.B));
        }

        private static void RunOnUIThread(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                action();
            }
            else
            {
                Dispatcher.UIThread.Post(action);
            }
        }
    }
}
