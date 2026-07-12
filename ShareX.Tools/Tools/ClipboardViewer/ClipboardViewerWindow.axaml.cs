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

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class ClipboardViewerWindow : Window
{
    private readonly ClipboardViewerViewModel _viewModel = new();

    public ClipboardViewerWindow()
    {
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Closed += (_, _) => _viewModel.Dispose();
    }

    private void OnImagePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && _viewModel.PreviewImageData is { Length: > 0 } imageData)
        {
            new ImageViewerWindow(imageData, _viewModel.SelectedFormat).Show();
            e.Handled = true;
        }
    }
}
