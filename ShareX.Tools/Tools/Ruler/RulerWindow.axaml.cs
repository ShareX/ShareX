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
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using ShareX.AvaloniaUI.Theming;
using ShareX.Tools.Controls;

namespace ShareX.Tools;

public partial class RulerWindow : Window
{
    private readonly RulerViewModel _viewModel = new();
    private RulerOverlayControl? _overlay;
    private Border? _infoPanel;
    private Point? _pointerPosition;
    private Rect _selection;

    public RulerWindow()
    {
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _overlay = this.FindControl<RulerOverlayControl>("RulerOverlay");
        _infoPanel = this.FindControl<Border>("InfoPanel");
        if (_overlay != null)
        {
            _overlay.MeasurementChanged += OnMeasurementChanged;
        }

        KeyDown += OnKeyDown;
        AddHandler(PointerReleasedEvent, OnWindowPointerReleased, RoutingStrategies.Tunnel);
        AddHandler(PointerMovedEvent, OnWindowPointerMoved, RoutingStrategies.Tunnel);
        Opened += (_, _) => _overlay?.Focus();
    }

    private void OnMeasurementChanged(object? sender, Rect selection)
    {
        _selection = selection;
        if (selection.Width <= 0 && selection.Height <= 0)
        {
            _viewModel.Clear();
            UpdateInfoPanelPlacement();
            return;
        }

        _viewModel.Update(selection, RenderScaling, Position);
        UpdateInfoPanelPlacement();
    }

    private async Task CopyMeasurementsAsync()
    {
        if (_viewModel.HasMeasurement && Clipboard != null)
        {
            await Clipboard.SetTextAsync(_viewModel.ClipboardText);
        }
        _overlay?.Focus();
    }

    private void OnWindowPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            Close();
            e.Handled = true;
        }
    }

    private void OnWindowPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_overlay != null)
        {
            _pointerPosition = e.GetPosition(_overlay);
            UpdateInfoPanelPlacement();
        }
    }

    private void UpdateInfoPanelPlacement()
    {
        if (_overlay == null || _infoPanel == null || _infoPanel.Bounds.Width <= 0)
        {
            return;
        }

        const double margin = 16;
        Rect topPanelBounds = new(
            Math.Max(0, (_overlay.Bounds.Width - _infoPanel.Bounds.Width) / 2),
            margin,
            _infoPanel.Bounds.Width,
            _infoPanel.Bounds.Height);
        Rect collisionBounds = topPanelBounds.Inflate(10);
        bool pointerIntersects = _pointerPosition is Point pointer && collisionBounds.Contains(pointer);
        bool rulerIntersects = (_selection.Width > 0 || _selection.Height > 0) && collisionBounds.Intersects(_selection);

        _infoPanel.VerticalAlignment = pointerIntersects || rulerIntersects
            ? VerticalAlignment.Bottom
            : VerticalAlignment.Top;
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
                _ = CopyMeasurementsAsync();
                e.Handled = true;
                break;
            case Key.Escape:
                Close();
                e.Handled = true;
                break;
        }
    }
}
