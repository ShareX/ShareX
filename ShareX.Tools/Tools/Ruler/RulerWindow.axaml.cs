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
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using ShareX.AvaloniaUI.Theming;
using ShareX.Tools.Controls;
using ShareX.Tools.Ruler;

namespace ShareX.Tools;

public partial class RulerWindow : Window
{
    private RulerOverlayControl _overlay = null!;

    public RulerWindow()
    {
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _overlay = this.FindControl<RulerOverlayControl>("RulerOverlay")!;

        ConfigureOverlayAndCaptureScreen();

        KeyDown += OnKeyDown;
        AddHandler(PointerReleasedEvent, OnWindowPointerReleased);
        Opened += (_, _) =>
        {
            Activate();
            _overlay.Focus();
        };
    }

    private void OnWindowPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            Close();
            e.Handled = true;
        }
    }

    private void ConfigureOverlayAndCaptureScreen()
    {
        IReadOnlyList<Screen> screens = Screens.All;
        if (screens.Count == 0)
        {
            return;
        }

        int left = screens.Min(screen => screen.Bounds.X);
        int top = screens.Min(screen => screen.Bounds.Y);
        int right = screens.Max(screen => screen.Bounds.Right);
        int bottom = screens.Max(screen => screen.Bounds.Bottom);
        PixelRect bounds = new(left, top, right - left, bottom - top);
        double scaling = Screens.ScreenFromPoint(bounds.Position)?.Scaling ?? screens[0].Scaling;

        ScreenPixelBuffer screenPixelBuffer = ScreenPixelBuffer.Capture(bounds);
        _overlay.SetScreenPixelBuffer(screenPixelBuffer);

        Position = bounds.Position;
        Width = bounds.Width / scaling;
        Height = bounds.Height / scaling;
    }

    private async Task CopyMeasurementAsync()
    {
        string? text = _overlay.MeasurementText;
        if (!string.IsNullOrEmpty(text) && Clipboard != null)
        {
            await Clipboard.SetTextAsync(text);
        }

        _overlay.Focus();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        int amount = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1;

        switch (e.Key)
        {
            case Key.Left:
                _overlay.Nudge(-amount, 0);
                e.Handled = true;
                break;
            case Key.Right:
                _overlay.Nudge(amount, 0);
                e.Handled = true;
                break;
            case Key.Up:
                _overlay.Nudge(0, -amount);
                e.Handled = true;
                break;
            case Key.Down:
                _overlay.Nudge(0, amount);
                e.Handled = true;
                break;
            case Key.H:
                _overlay.SetHorizontal();
                e.Handled = true;
                break;
            case Key.V:
                _overlay.SetVertical();
                e.Handled = true;
                break;
            case Key.Space:
                _overlay.ToggleAxis();
                e.Handled = true;
                break;
            case Key.Delete:
                _overlay.Clear();
                e.Handled = true;
                break;
            case Key.C when e.KeyModifiers.HasFlag(KeyModifiers.Control):
                _ = CopyMeasurementAsync();
                e.Handled = true;
                break;
            case Key.Escape:
                Close();
                e.Handled = true;
                break;
        }
    }
}
