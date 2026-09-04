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

#nullable enable

using Avalonia.Controls;
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.IO;
using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;
using DrawingBitmap = System.Drawing.Bitmap;

namespace ShareX;

public partial class StartScreenWindow : Window
{
    private readonly AvaloniaBitmap _logoBitmap;
    private readonly StartScreenViewModel _viewModel;

    public StartScreenWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        _viewModel = new StartScreenViewModel();
        DataContext = _viewModel;

        using DrawingBitmap logo = ShareXResources.Logo;
        using Stream logoStream = logo.GetStream();
        logoStream.Position = 0;
        _logoBitmap = new AvaloniaBitmap(logoStream);
        LogoImage.Source = _logoBitmap;

        Opened += (_, _) => Activate();
        ThemeManager.ThemeChanged += OnThemeChanged;
        Closed += OnClosed;
    }

    public Task ShowAsync()
    {
        TaskCompletionSource<bool> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Closed += (_, _) => completion.TrySetResult(true);
        Show();
        return completion.Task;
    }

    private void OnThemeChanged(object? sender, Avalonia.Styling.ThemeVariant theme) =>
        Avalonia.Threading.Dispatcher.UIThread.Post(() => RequestedThemeVariant = theme);

    private void OnGetStartedClick(object? sender, RoutedEventArgs e) => Close();

    private void OnClosed(object? sender, EventArgs e)
    {
        ThemeManager.ThemeChanged -= OnThemeChanged;
        SettingManager.SaveApplicationConfigAsync();
        _viewModel.Dispose();
        _logoBitmap.Dispose();
    }
}
