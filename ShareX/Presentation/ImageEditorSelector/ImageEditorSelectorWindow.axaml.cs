#region License Information (GPL v3)

/*
    ShareX - A program developed by ShareX Team
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Theming;

namespace ShareX;

public partial class ImageEditorSelectorWindow : Window
{
    public bool? UseLegacyImageEditor { get; private set; }

    public ImageEditorSelectorWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Opened += (_, _) => Activate();
    }

    private void OnModernImageEditorClick(object? sender, RoutedEventArgs e)
    {
        UseLegacyImageEditor = false;
        Close();
    }

    private void OnLegacyImageEditorClick(object? sender, RoutedEventArgs e)
    {
        UseLegacyImageEditor = true;
        Close();
    }
}
