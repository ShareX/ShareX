#region License Information (GPL v3)

/* ShareX - Copyright (c) 2007-2026 ShareX Team - GPL v3 */

#endregion License Information (GPL v3)

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.ImageEffectsLib;

public partial class ImageEffectsConfirmationWindow : Window
{
    public ImageEffectsConfirmationWindow() : this("Continue?")
    {
    }

    public ImageEffectsConfirmationWindow(string message)
    {
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        this.FindControl<TextBlock>("MessageText")!.Text = message;
    }

    private void OnYesClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close(true);
    private void OnNoClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close(false);
}
