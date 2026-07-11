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
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShareX.AvaloniaUI.Theming;
using ShareX.Tools.Controls;

namespace ShareX.Tools;

public partial class RulerWindow : Window
{
    private readonly RulerViewModel _viewModel = new();
    private RulerOverlayControl? _overlay;

    public RulerWindow()
    {
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _overlay = this.FindControl<RulerOverlayControl>("RulerOverlay");
        if (_overlay != null)
        {
            _overlay.MeasurementChanged += OnMeasurementChanged;
        }

        KeyDown += OnKeyDown;
        AddHandler(PointerPressedEvent, OnWindowPointerPressed, RoutingStrategies.Tunnel);
        Opened += (_, _) => _overlay?.Focus();
    }

    private void OnMeasurementChanged(object? sender, Rect selection)
    {
        if (selection.Width <= 0 && selection.Height <= 0)
        {
            _viewModel.Clear();
            return;
        }

        _viewModel.Update(selection, RenderScaling, Position);
    }

    private void OnResetClick(object? sender, RoutedEventArgs e) => _overlay?.Clear();
    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();

    private async void OnCopyClick(object? sender, RoutedEventArgs e)
    {
        if (_viewModel.HasMeasurement && Clipboard != null)
        {
            await Clipboard.SetTextAsync(_viewModel.ClipboardText);
        }
        _overlay?.Focus();
    }

    private void OnWindowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            Close();
            e.Handled = true;
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        double amount = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1;
        switch (e.Key)
        {
            case Key.Left:
                _overlay?.Nudge(-amount, 0);
                e.Handled = true;
                break;
            case Key.Right:
                _overlay?.Nudge(amount, 0);
                e.Handled = true;
                break;
            case Key.Up:
                _overlay?.Nudge(0, -amount);
                e.Handled = true;
                break;
            case Key.Down:
                _overlay?.Nudge(0, amount);
                e.Handled = true;
                break;
            case Key.Delete:
                _overlay?.Clear();
                e.Handled = true;
                break;
            case Key.C when e.KeyModifiers.HasFlag(KeyModifiers.Control):
                OnCopyClick(this, new RoutedEventArgs());
                e.Handled = true;
                break;
            case Key.Escape:
                Close();
                e.Handled = true;
                break;
        }
    }
}
