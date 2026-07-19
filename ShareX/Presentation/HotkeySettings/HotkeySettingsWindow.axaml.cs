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
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ShareX.AvaloniaUI.Theming;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX;

public partial class HotkeySettingsWindow : Window
{
    private HotkeySettingsViewModel? _viewModel;

    public HotkeySettingsWindow()
    {
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
    }

    public HotkeySettingsWindow(IHotkeySettingsService service) : this()
    {
        _viewModel = new HotkeySettingsViewModel(service);
        DataContext = _viewModel;
        Closed += (_, _) => _viewModel?.Dispose();
    }

    private void OnAddClick(object? sender, RoutedEventArgs e) => _viewModel?.Add();
    private void OnRemoveClick(object? sender, RoutedEventArgs e) => _viewModel?.RemoveSelected();
    private void OnEditClick(object? sender, RoutedEventArgs e) => _viewModel?.EditSelectedTask();
    private void OnDuplicateClick(object? sender, RoutedEventArgs e) => _viewModel?.DuplicateSelected();
    private void OnMoveUpClick(object? sender, RoutedEventArgs e) => _viewModel?.MoveSelected(-1);
    private void OnMoveDownClick(object? sender, RoutedEventArgs e) => _viewModel?.MoveSelected(1);
    private void OnEnableHotkeysClick(object? sender, RoutedEventArgs e) => _viewModel?.EnableHotkeys();
    private void OnResetClick(object? sender, RoutedEventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.IsResetConfirmationVisible = true;
        }
    }
    private void OnCancelResetClick(object? sender, RoutedEventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.IsResetConfirmationVisible = false;
        }
    }
    private void OnConfirmResetClick(object? sender, RoutedEventArgs e) => _viewModel?.ConfirmReset();

    private void OnEditRowClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: HotkeySettingsItem item })
        {
            _viewModel?.EditTask(item);
        }
    }

    private void OnTaskClick(object? sender, RoutedEventArgs e)
    {
        if (_viewModel == null || sender is not Button { DataContext: HotkeySettingsItem item } button)
        {
            return;
        }

        _viewModel.SelectedItem = item;

        List<MenuItem> rootItems = [];

        foreach (IGrouping<string, HotkeyTaskOption> group in item.TaskOptions.GroupBy(x => x.Category))
        {
            List<MenuItem> taskItems = group.Select(option => CreateTaskMenuItem(item, option)).ToList();

            if (string.IsNullOrWhiteSpace(group.Key))
            {
                rootItems.AddRange(taskItems);
            }
            else
            {
                rootItems.Add(new MenuItem
                {
                    Header = group.Key,
                    ItemsSource = taskItems
                });
            }
        }

        ContextMenu menu = new()
        {
            Placement = PlacementMode.BottomEdgeAlignedLeft,
            PlacementTarget = button,
            ItemsSource = rootItems
        };
        menu.Open(button);
    }

    private MenuItem CreateTaskMenuItem(HotkeySettingsItem item, HotkeyTaskOption option)
    {
        MenuItem menuItem = new()
        {
            Header = option.Name,
            Icon = CreateTaskMenuIcon(option.Icon),
            ToggleType = MenuItemToggleType.Radio,
            IsChecked = item.SelectedTask?.Value == option.Value
        };
        menuItem.Click += (_, _) => _viewModel?.ChangeTask(item, option);
        return menuItem;
    }

    private static Control? CreateTaskMenuIcon(string icon)
    {
        if (string.IsNullOrEmpty(icon))
        {
            return null;
        }

        TextBlock textBlock = new()
        {
            Text = icon,
            FontSize = 16,
            Width = 18,
            TextAlignment = Avalonia.Media.TextAlignment.Center
        };
        textBlock.Classes.Add("icon");
        textBlock.Classes.Add("accent-menu-icon");
        return textBlock;
    }

    private void OnHotkeyClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: HotkeySettingsItem item } button)
        {
            _viewModel?.ToggleCapture(item);
            button.Focus();
        }
    }

    private void OnHotkeyKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not Control { DataContext: HotkeySettingsItem item } || !item.IsCapturing)
        {
            return;
        }

        e.Handled = true;

        if (e.Key == Key.Escape)
        {
            _viewModel?.ClearAndStopCapture();
            return;
        }

        bool isWindowsKey = e.Key is Key.LWin or Key.RWin;
        bool isModifierKey = e.Key is Key.LeftCtrl or Key.RightCtrl
            or Key.LeftShift or Key.RightShift
            or Key.LeftAlt or Key.RightAlt
            or Key.LWin or Key.RWin;

        HotkeyGesture gesture = new(
            isWindowsKey ? string.Empty : e.Key.ToString(),
            e.KeyModifiers.HasFlag(KeyModifiers.Control),
            e.KeyModifiers.HasFlag(KeyModifiers.Shift),
            e.KeyModifiers.HasFlag(KeyModifiers.Alt),
            isWindowsKey || e.KeyModifiers.HasFlag(KeyModifiers.Meta));

        _viewModel?.ApplyGesture(item, gesture);

        if (isModifierKey)
        {
            item.IsCapturing = true;
        }
    }

    private void OnHotkeyKeyUp(object? sender, KeyEventArgs e)
    {
        if (sender is Control { DataContext: HotkeySettingsItem item }
            && item.IsCapturing
            && e.Key == Key.PrintScreen)
        {
            e.Handled = true;
            OnHotkeyKeyDown(sender, e);
        }
    }

    private void OnHotkeyLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: HotkeySettingsItem item } && item.IsCapturing)
        {
            _viewModel?.StopCapture();
        }
    }
}
