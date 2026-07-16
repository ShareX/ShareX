using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.ImageEditor.Presentation.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private static readonly IReadOnlyList<string> SupportedEditorThemes = new[]
        {
            "Dark",
            "Light"
        };

        public event EventHandler? OpenOptionsPanelRequested;

        public IReadOnlyList<string> EditorThemeOptions => SupportedEditorThemes;

        public bool EditorUseSystemTheme
        {
            get => ThemeOptions.UseSystemTheme;
            set
            {
                if (ThemeOptions.UseSystemTheme == value)
                {
                    return;
                }

                ThemeOptions.UseSystemTheme = value;
                OnPropertyChanged(nameof(EditorUseSystemTheme));
                OnPropertyChanged(nameof(CanEditEditorTheme));
            }
        }

        public bool CanEditEditorTheme => !EditorUseSystemTheme;

        public string EditorTheme
        {
            get => NormalizeEditorTheme(ThemeOptions.Theme);
            set
            {
                string normalizedTheme = NormalizeEditorTheme(value);
                if (string.Equals(ThemeOptions.Theme, normalizedTheme, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                ThemeOptions.Theme = normalizedTheme;
                OnPropertyChanged(nameof(EditorTheme));
            }
        }

        public bool EditorUseSystemAccentColor
        {
            get => ThemeOptions.UseSystemAccentColor;
            set
            {
                if (ThemeOptions.UseSystemAccentColor == value)
                {
                    return;
                }

                ThemeOptions.UseSystemAccentColor = value;
                OnPropertyChanged(nameof(EditorUseSystemAccentColor));
                OnPropertyChanged(nameof(CanEditEditorAccentColor));
            }
        }

        public bool CanEditEditorAccentColor => !EditorUseSystemAccentColor;

        public Color EditorAccentColor
        {
            get => ThemeOptions.AccentColor;
            set
            {
                if (EditorAccentColor == value)
                {
                    return;
                }

                ThemeOptions.AccentColor = value;
                OnPropertyChanged(nameof(EditorAccentColor));
                OnPropertyChanged(nameof(EditorAccentColorHex));
            }
        }

        public string EditorAccentColorHex => ThemeOptions.AccentColorHex;

        public bool EditorRememberWindowState
        {
            get => Options.RememberWindowState;
            set
            {
                if (Options.RememberWindowState == value)
                {
                    return;
                }

                Options.RememberWindowState = value;
                OnPropertyChanged(nameof(EditorRememberWindowState));
            }
        }

        public bool EditorShowExitConfirmation
        {
            get => Options.ShowExitConfirmation;
            set
            {
                if (Options.ShowExitConfirmation == value)
                {
                    return;
                }

                Options.ShowExitConfirmation = value;
                OnPropertyChanged(nameof(EditorShowExitConfirmation));
            }
        }

        public bool EditorZoomToFitOnOpen
        {
            get => Options.ZoomToFitOnOpen;
            set
            {
                if (Options.ZoomToFitOnOpen == value)
                {
                    return;
                }

                Options.ZoomToFitOnOpen = value;
                OnPropertyChanged(nameof(EditorZoomToFitOnOpen));
            }
        }

        public bool EditorQuickCrop
        {
            get => Options.QuickCrop;
            set
            {
                if (Options.QuickCrop == value)
                {
                    return;
                }

                Options.QuickCrop = value;
                OnPropertyChanged(nameof(EditorQuickCrop));
            }
        }

        public bool EditorAutoCloseEditorOnTask
        {
            get => Options.AutoCloseEditorOnTask;
            set
            {
                if (Options.AutoCloseEditorOnTask == value)
                {
                    return;
                }

                Options.AutoCloseEditorOnTask = value;
                OnPropertyChanged(nameof(EditorAutoCloseEditorOnTask));
            }
        }

        public bool EditorAutoCopyImageToClipboard
        {
            get => Options.AutoCopyImageToClipboard;
            set
            {
                if (Options.AutoCopyImageToClipboard == value)
                {
                    return;
                }

                Options.AutoCopyImageToClipboard = value;
                OnPropertyChanged(nameof(EditorAutoCopyImageToClipboard));
            }
        }

        public bool EditorShowInsertImageDialog
        {
            get => Options.ShowInsertImageDialog;
            set
            {
                if (Options.ShowInsertImageDialog == value)
                {
                    return;
                }

                Options.ShowInsertImageDialog = value;
                OnPropertyChanged(nameof(EditorShowInsertImageDialog));
            }
        }

        public bool EditorShowNotifications
        {
            get => Options.ShowNotifications;
            set
            {
                if (Options.ShowNotifications == value)
                {
                    return;
                }

                Options.ShowNotifications = value;
                OnPropertyChanged(nameof(EditorShowNotifications));

                if (!value)
                {
                    HideNotification();
                }
            }
        }

        [RelayCommand]
        private void OpenOptionsPanel()
        {
            OpenOptionsPanelRequested?.Invoke(this, EventArgs.Empty);
        }

        private static string NormalizeEditorTheme(string? theme)
        {
            return !string.IsNullOrWhiteSpace(theme) &&
                theme.Contains("Light", StringComparison.OrdinalIgnoreCase)
                ? "Light"
                : "Dark";
        }
    }
}
