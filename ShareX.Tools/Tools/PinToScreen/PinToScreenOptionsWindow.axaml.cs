#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class PinToScreenOptionsWindow : Window
{
    public PinToScreenOptionsWindow() : this(new PinToScreenOptions())
    {
    }

    public PinToScreenOptionsWindow(PinToScreenOptions options)
    {
        PinToScreenOptionsViewModel viewModel = new(options);
        DataContext = viewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        viewModel.CloseRequested = result => Close(result);
    }
}
