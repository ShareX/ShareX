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

using Avalonia.Controls;
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Controls;
using ShareX.AvaloniaUI.Theming;
using ShareX.Icons;
using System;

namespace ShareX;

/// <summary>
/// Asks once, after updating to a version with the legacy icon pack, which icon pack to use, showing a few icons
/// from each. Picking a card applies the pack at once, so the windows behind it change too.
/// </summary>
public partial class IconPackPromptWindow : Window
{
    // Main menu commands that look clearly different in the two packs.
    private static readonly string[] PreviewIcons =
    [
        AppIcons.Region,
        AppIcons.ScreenRecording,
        AppIcons.ImageUpload,
        AppIcons.ImageEditor,
        AppIcons.Clipboard,
        AppIcons.History,
        AppIcons.ScreenColorPicker,
        AppIcons.Settings
    ];

    private ApplicationThemeOptions ThemeOptions => ApplicationState.Settings.ThemeOptions;

    public IconPackPromptWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        FillPreview(ModernPreview);
        FillPreview(LegacyPreview);
        UpdateSelection();

        Opened += (_, _) => Activate();
        ThemeManager.ThemeChanged += OnThemeChanged;
        Closed += OnClosed;
    }

    /// <summary>Shows the prompt over the main window once, the first time it is shown after the update.</summary>
    internal static void ShowOnceWithMainWindow()
    {
        if (!ApplicationState.Settings.ShowIconPackPrompt || StartupOptions.Sandbox || MainWindowIntegration.Instance is not MainWindow mainWindow)
        {
            return;
        }

        if (mainWindow.IsVisible)
        {
            Show(mainWindow);
        }
        else
        {
            // Started silently: wait until the user first opens the main window.
            void OnMainWindowOpened(object? sender, EventArgs e)
            {
                mainWindow.Opened -= OnMainWindowOpened;
                Show(mainWindow);
            }

            mainWindow.Opened += OnMainWindowOpened;
        }

        static void Show(MainWindow owner)
        {
            if (ApplicationState.Settings.ShowIconPackPrompt)
            {
                new IconPackPromptWindow().Show(owner);
            }
        }
    }

    private static void FillPreview(Panel panel)
    {
        foreach (string icon in PreviewIcons)
        {
            panel.Children.Add(new IconPresenter { Icon = icon });
        }
    }

    private void SelectPack(IconPack pack)
    {
        ThemeOptions.IconPack = pack;
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        ModernCard.Classes.Set("selected", ThemeOptions.IconPack == IconPack.Modern);
        LegacyCard.Classes.Set("selected", ThemeOptions.IconPack == IconPack.Legacy);
    }

    private void OnModernClick(object? sender, RoutedEventArgs e) => SelectPack(IconPack.Modern);

    private void OnLegacyClick(object? sender, RoutedEventArgs e) => SelectPack(IconPack.Legacy);

    private void OnContinueClick(object? sender, RoutedEventArgs e) => Close();

    private void OnThemeChanged(object? sender, Avalonia.Styling.ThemeVariant theme) =>
        Avalonia.Threading.Dispatcher.UIThread.Post(() => RequestedThemeVariant = theme);

    private void OnClosed(object? sender, EventArgs e)
    {
        ThemeManager.ThemeChanged -= OnThemeChanged;
        ApplicationState.Settings.ShowIconPackPrompt = false;
        SettingManager.SaveApplicationConfigAsync();
    }
}
