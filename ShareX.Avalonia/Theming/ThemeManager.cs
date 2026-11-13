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
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using ShareX.Icons;
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
        private static ResourceInclude? _compactResources;
        private static readonly ResourceDictionary SpacingResources = new();

        public static event EventHandler<ThemeVariant>? ThemeChanged;
        public static event EventHandler<Color>? AccentColorChanged;
        public static event EventHandler? SpacingChanged;

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
            bool themeChanged = !Equals(_currentTheme, theme);
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

            if (themeChanged)
            {
                ThemeChanged?.Invoke(null, theme);
            }
        }

        public static ThemeVariant GetCurrentTheme()
        {
            return _currentTheme;
        }

        public static bool IsDarkTheme => !IsLightTheme(_currentTheme.ToString());

        public static void Refresh()
        {
            RunOnUIThread(ApplyConfiguredSettings);
        }

        private static void OnOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Spacing changes arrive continuously while a slider is dragged, so they skip the theme and
            // accent refresh, which replaces brushes across the whole application.
            if (ApplicationThemeOptions.IsSpacingProperty(e.PropertyName))
            {
                RunOnUIThread(() => ApplySpacing(Application.Current));
                return;
            }

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

            ApplyAccentColor(application, accentColor, theme);
            IconCatalog.Pack = _options.IconPack;
            ApplySpacing(application);

            if (application != null)
            {
                SetResources(application.Resources, new Dictionary<object, object?>
                {
                    ["ShareX.ScrollBar.AllowAutoHide"] = !_options.AlwaysExpandScrollBars
                });
            }
        }

        private static void ApplySpacing(Application? application)
        {
            if (application == null || _options == null)
            {
                return;
            }

            // Application resources are looked up before application styles, so these values
            // override the default spacing declared in ControlStyles.axaml.
            IList<IResourceProvider> dictionaries = application.Resources.MergedDictionaries;
            if (!dictionaries.Contains(SpacingResources))
            {
                dictionaries.Add(SpacingResources);
            }

            _compactResources ??= new ResourceInclude(new Uri("avares://ShareX.Avalonia/"))
            {
                Source = new Uri("avares://ShareX.Avalonia/Theming/CompactMode.axaml")
            };

            bool changed = false;

            // CompactMode.axaml lists every spacing value with its compact size; the default size of the same
            // key comes from the application styles, and the configured spacing picks a point between the two.
            foreach (object key in _compactResources.Loaded.Keys.ToArray())
            {
                if (key is not string name || !TryGetSpacingArea(name, out SpacingArea area) ||
                    !_compactResources.TryGetResource(key, _currentTheme, out object? compactValue) ||
                    !application.Styles.TryGetResource(key, _currentTheme, out object? defaultValue))
                {
                    continue;
                }

                double amount = _options.GetSpacing(area) / (double)ApplicationThemeOptions.DefaultSpacing;

                // Wider spacing doesn't need a wider settings window, so the window width only shrinks.
                if (name == "ShareX.Density.Settings.WindowWidth")
                {
                    amount = Math.Min(amount, 1);
                }

                object? value = InterpolateSpacing(name, compactValue, defaultValue, amount);

                if (value != null && (!SpacingResources.TryGetValue(key, out object? current) || !Equals(current, value)))
                {
                    SpacingResources[key] = value;
                    changed = true;
                }
            }

            if (changed)
            {
                SpacingChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        private static bool TryGetSpacingArea(string key, out SpacingArea area)
        {
            if (key.StartsWith("ShareX.Density.Nav.", StringComparison.Ordinal))
            {
                area = SpacingArea.Sidebar;
            }
            else if (key.StartsWith("ShareX.Density.Menu.", StringComparison.Ordinal) ||
                key is "MenuFlyoutPresenterThemePadding" or "MenuIconPresenterMargin")
            {
                area = SpacingArea.Menus;
            }
            else if (key.StartsWith("ShareX.Density.ToolGrid.", StringComparison.Ordinal))
            {
                area = SpacingArea.ToolMenus;
            }
            else if (key.StartsWith("ShareX.Density.Settings.", StringComparison.Ordinal))
            {
                area = SpacingArea.SettingsWindows;
            }
            else
            {
                area = default;
                return false;
            }

            return true;
        }

        private static object? InterpolateSpacing(string key, object? compactValue, object? defaultValue, double amount)
        {
            // Text sizes move in half-pixel steps so they stay readable; everything else snaps to whole pixels.
            bool isTextSize = key.EndsWith("FontSize", StringComparison.Ordinal) || key.EndsWith("IconSize", StringComparison.Ordinal);

            double Lerp(double compact, double comfortable)
            {
                double value = Math.Max(0, compact + ((comfortable - compact) * amount));
                return isTextSize ? Math.Round(value * 2) / 2 : Math.Round(value);
            }

            return (compactValue, defaultValue) switch
            {
                (double compact, double comfortable) => Lerp(compact, comfortable),
                (Thickness compact, Thickness comfortable) => new Thickness(
                    Lerp(compact.Left, comfortable.Left),
                    Lerp(compact.Top, comfortable.Top),
                    Lerp(compact.Right, comfortable.Right),
                    Lerp(compact.Bottom, comfortable.Bottom)),
                _ => null
            };
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

        // Theme backgrounds that accent colored text and indicators are drawn on.
        private static readonly string[] AccentSurfaceKeys =
        [
            "ShareX.Color.Background.Main",
            "ShareX.Color.Background.Panel",
            "ShareX.Color.Background.Popup",
            "ShareX.Color.Background.Menu",
            "ShareX.Color.Background.Toolbar",
            "ShareX.Color.Control.Background",
            "ShareX.Color.Control.Background.Hover"
        ];

        // Accent colored text and indicators depend on the background they are drawn on, so they are computed for
        // each theme and placed in theme dictionaries. Windows that force a variant (the photo editor uses plain Dark)
        // still resolve them through the base Dark/Light entries.
        private static AccentPalette? _darkPalette;
        private static AccentPalette? _lightPalette;

        /// <summary>The accent palette for the given theme, or null before the accent has been applied.</summary>
        public static AccentPalette? GetAccentPalette(ThemeVariant? theme = null)
        {
            return IsLightTheme((theme ?? _currentTheme).ToString()) ? _lightPalette : _darkPalette;
        }

        private static void ApplyAccentColor(Application? application, Color accentColor, ThemeVariant theme)
        {
            if (application == null)
            {
                return;
            }

            _darkPalette = AccentPalette.Create(accentColor, GetAccentSurfaces(application, ShareXDark));
            _lightPalette = AccentPalette.Create(accentColor, GetAccentSurfaces(application, ShareXLight));
            AccentPalette palette = GetAccentPalette(theme)!;
            IResourceDictionary resources = application.Resources;

            // Every resource write makes Avalonia re-resolve resources across all open windows, so the values are
            // collected and written in one batch per dictionary rather than one key at a time.
            Dictionary<object, object?> values = new()
            {
                // Accent fills (buttons, selections, checked states) and the text drawn on them. These do not depend
                // on the theme.
                ["ShareX.Color.Accent.Start"] = palette.FillStart,
                ["ShareX.Color.Accent.End"] = palette.FillEnd,
                ["ShareX.Color.Accent.Foreground"] = palette.OnFill,
                ["ShareX.Brush.Accent.Start"] = new SolidColorBrush(palette.FillStart),
                ["ShareX.Brush.Accent.Foreground"] = new SolidColorBrush(palette.OnFill),
                // Icons on the theme backgrounds use the accent as picked, so they do not depend on the theme either.
                ["ShareX.Color.Accent.Icon"] = palette.Icon,
                ["ShareX.Brush.Accent.Icon"] = new SolidColorBrush(palette.Icon),
                ["ShareX.Brush.Accent"] = new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                    GradientStops =
                    {
                        new GradientStop(palette.FillStart, 0),
                        new GradientStop(palette.FillEnd, 1)
                    }
                }
            };

            AddFluentAccentResources(values, palette);
            bool changed = SetResources(resources, values);

            // Accent colored text and indicators drawn on the theme backgrounds rather than on the accent.
            // ShareXDark and ShareXLight inherit Dark and Light, so they resolve these through the base entries.
            changed |= SetResources(GetThemeDictionary(resources, ThemeVariant.Dark), GetSurfaceAccentResources(_darkPalette));
            changed |= SetResources(GetThemeDictionary(resources, ThemeVariant.Light), GetSurfaceAccentResources(_lightPalette));

            if (changed)
            {
                AccentColorChanged?.Invoke(null, palette.FillStart);
            }
        }

        /// <summary>Writes the values that differ from what the dictionary already holds, raising at most one change notification.</summary>
        private static bool SetResources(IResourceDictionary resources, IReadOnlyDictionary<object, object?> values)
        {
            KeyValuePair<object, object?>[] changed = values
                .Where(pair => !resources.TryGetValue(pair.Key, out object? current) || !IsSameResource(current, pair.Value))
                .ToArray();

            if (changed.Length == 0)
            {
                return false;
            }

            if (resources is ResourceDictionary dictionary)
            {
                dictionary.SetItems(changed);
            }
            else
            {
                foreach (KeyValuePair<object, object?> pair in changed)
                {
                    resources[pair.Key] = pair.Value;
                }
            }

            return true;
        }

        // Brushes are recreated on every refresh, so they are compared by what they paint.
        private static bool IsSameResource(object? current, object? value)
        {
            return (current, value) switch
            {
                (ISolidColorBrush a, ISolidColorBrush b) => a.Color == b.Color && a.Opacity == b.Opacity,
                (IGradientBrush a, IGradientBrush b) => a.Opacity == b.Opacity &&
                    a.GradientStops.Select(stop => (stop.Color, stop.Offset)).SequenceEqual(b.GradientStops.Select(stop => (stop.Color, stop.Offset))),
                _ => Equals(current, value)
            };
        }

        // A dictionary can only belong to one theme key, so each variant gets its own, created once and updated in place.
        private static IResourceDictionary GetThemeDictionary(IResourceDictionary resources, ThemeVariant theme)
        {
            if (resources.ThemeDictionaries.TryGetValue(theme, out IThemeVariantProvider? provider) && provider is IResourceDictionary dictionary)
            {
                return dictionary;
            }

            ResourceDictionary created = new();
            resources.ThemeDictionaries[theme] = created;
            return created;
        }

        private static Dictionary<object, object?> GetSurfaceAccentResources(AccentPalette palette)
        {
            return new Dictionary<object, object?>
            {
                ["ShareX.Color.Accent.Text"] = palette.Text,
                ["ShareX.Brush.Accent.Text"] = new SolidColorBrush(palette.Text),
                ["ShareX.Color.Accent.Border"] = palette.Border,
                ["ShareX.Brush.Accent.Border"] = new SolidColorBrush(palette.Border),
                // Drawn inside selected items, so it outlines an accent fill that is too close to the background
                // without changing layout. It matches the fill when the fill already stands out.
                ["ShareX.BoxShadow.Accent.Outline"] = new BoxShadows(new BoxShadow
                {
                    IsInset = true,
                    Spread = 1,
                    Color = palette.Border
                }),
                ["ShareX.Color.Link"] = palette.Text,
                ["ShareX.Brush.Link"] = new SolidColorBrush(palette.Text),
                ["HyperlinkButtonForeground"] = new SolidColorBrush(palette.Text),
                ["HyperlinkButtonForegroundPointerOver"] = new SolidColorBrush(palette.Text),
                ["HyperlinkButtonForegroundPressed"] = new SolidColorBrush(palette.Text)
            };
        }

        // Fluent's built-in controls derive their checked/selected fills from SystemAccentColor and its Light/Dark
        // variants, and draw white glyphs and text on them. Point those at the palette so they stay readable, and
        // make the list selection fills opaque so text contrast does not depend on what is behind them.
        private static void AddFluentAccentResources(IDictionary<object, object?> resources, AccentPalette palette)
        {
            resources["SystemAccentColor"] = palette.FillStart;
            resources["SystemAccentColorLight1"] = palette.FillLight1;
            resources["SystemAccentColorLight2"] = palette.FillLight2;
            resources["SystemAccentColorLight3"] = palette.FillLight3;
            resources["SystemAccentColorDark1"] = palette.FillDark1;
            resources["SystemAccentColorDark2"] = palette.FillDark2;
            resources["SystemAccentColorDark3"] = palette.FillDark3;

            SolidColorBrush fill = new(palette.FillStart);
            SolidColorBrush fillHover = new(palette.FillLight1);
            SolidColorBrush fillPressed = new(palette.FillDark1);
            SolidColorBrush onFill = new(palette.OnFill);

            resources["SystemControlHighlightListAccentLowBrush"] = fill;
            resources["SystemControlHighlightListAccentMediumBrush"] = fillHover;
            resources["SystemControlHighlightListAccentHighBrush"] = fillPressed;
            resources["TreeViewItemBackgroundSelected"] = fill;
            resources["TreeViewItemBackgroundSelectedPointerOver"] = fillHover;
            resources["TreeViewItemBackgroundSelectedPressed"] = fillPressed;
            resources["TextControlSelectionHighlightColor"] = fill;

            foreach (string key in FluentOnAccentKeys)
            {
                resources[key] = onFill;
            }
        }

        private static readonly string[] FluentOnAccentKeys =
        [
            "AccentButtonForeground",
            "AccentButtonForegroundPointerOver",
            "AccentButtonForegroundPressed",
            "ToggleButtonForegroundChecked",
            "ToggleButtonForegroundCheckedPointerOver",
            "ToggleButtonForegroundCheckedPressed",
            "SplitButtonForegroundChecked",
            "SplitButtonForegroundCheckedPointerOver",
            "SplitButtonForegroundCheckedPressed",
            "CheckBoxCheckGlyphForegroundChecked",
            "CheckBoxCheckGlyphForegroundCheckedPointerOver",
            "CheckBoxCheckGlyphForegroundCheckedPressed",
            "CheckBoxCheckGlyphForegroundIndeterminate",
            "CheckBoxCheckGlyphForegroundIndeterminatePointerOver",
            "CheckBoxCheckGlyphForegroundIndeterminatePressed",
            "RadioButtonCheckGlyphFill",
            "RadioButtonCheckGlyphFillPointerOver",
            "RadioButtonCheckGlyphFillPressed",
            "ToggleSwitchKnobFillOn",
            "ToggleSwitchKnobFillOnPointerOver",
            "ToggleSwitchKnobFillOnPressed",
            "TreeViewItemForegroundSelected",
            "TreeViewItemForegroundSelectedPointerOver",
            "TreeViewItemForegroundSelectedPressed",
            "ComboBoxItemForegroundSelected",
            "ComboBoxItemForegroundSelectedPointerOver",
            "ComboBoxItemForegroundSelectedPressed"
        ];

        private static Color[] GetAccentSurfaces(Application application, ThemeVariant theme)
        {
            Color[] surfaces = AccentSurfaceKeys
                .Select(key => application.TryGetResource(key, theme, out object? value) ? value : null)
                .OfType<Color>()
                .ToArray();

            return surfaces.Length > 0 ? surfaces : [IsLightTheme(theme.ToString()) ? Colors.White : Colors.Black];
        }

        private static void RunOnUIThread(Action action)
        {
            if (Application.Current == null)
            {
                throw new InvalidOperationException("Avalonia must be initialized before applying theme changes.");
            }

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
