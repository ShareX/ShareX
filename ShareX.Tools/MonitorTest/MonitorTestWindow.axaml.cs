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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;

namespace ShareX.Tools;

public partial class MonitorTestWindow : Window
{
    private readonly MonitorTestViewModel _viewModel = new();

    public MonitorTestWindow()
    {
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Opened += OnOpened;
        KeyDown += OnKeyDown;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Screen? screen = null;
        if (NativeMethods.GetCursorPos(out POINT cursorPosition))
        {
            screen = Screens.ScreenFromPoint(new PixelPoint(cursorPosition.X, cursorPosition.Y));
        }
        screen ??= Screens.Primary;

        if (screen != null)
        {
            Position = screen.Bounds.Position;
            Width = screen.Bounds.Width / screen.Scaling;
            Height = screen.Bounds.Height / screen.Scaling;
        }

        Activate();
        Focus();
    }

    private void OnGrayscaleClick(object? sender, RoutedEventArgs e) => _viewModel.SelectMode(MonitorTestMode.Grayscale);
    private void OnRgbClick(object? sender, RoutedEventArgs e) => _viewModel.SelectMode(MonitorTestMode.Rgb);
    private void OnGradientClick(object? sender, RoutedEventArgs e) => _viewModel.SelectMode(MonitorTestMode.Gradient);
    private void OnPatternClick(object? sender, RoutedEventArgs e) => _viewModel.SelectMode(MonitorTestMode.Pattern);
    private void OnMotionClick(object? sender, RoutedEventArgs e) => _viewModel.SelectMode(MonitorTestMode.Motion);
    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();

    private void OnSurfacePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            Close();
            e.Handled = true;
        }
        else if (e.InitialPressMouseButton == MouseButton.Left)
        {
            _viewModel.ControlsVisible = !_viewModel.ControlsVisible;
            e.Handled = true;
        }
    }

    private void OnSurfacePointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_viewModel.IsMotion)
        {
            double direction = Math.Sign(e.Delta.Y);
            _viewModel.MotionSpeed = Math.Clamp(_viewModel.MotionSpeed + direction * 50, 100, 2000);
            e.Handled = true;
        }
    }

    private static void OnControlsPointerPressed(object? sender, PointerPressedEventArgs e) => e.Handled = true;
    private static void OnControlsPointerReleased(object? sender, PointerReleasedEventArgs e) => e.Handled = true;
    private static void OnControlsPointerWheelChanged(object? sender, PointerWheelEventArgs e) => e.Handled = true;

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                Close();
                e.Handled = true;
                break;
            case Key.Space:
                _viewModel.ControlsVisible = !_viewModel.ControlsVisible;
                e.Handled = true;
                break;
            case Key.Left:
                _viewModel.SelectRelativeMode(-1);
                e.Handled = true;
                break;
            case Key.Right:
                _viewModel.SelectRelativeMode(1);
                e.Handled = true;
                break;
        }
    }
}
