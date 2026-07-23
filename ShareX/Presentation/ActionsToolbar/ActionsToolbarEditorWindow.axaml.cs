#region License Information (GPL v3)

/*
    ShareX - A program developed by ShareX Team
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ShareX;

public partial class ActionsToolbarEditorWindow : Window
{
    private readonly ObservableCollection<ActionsToolbarItem> _items = [];
    private readonly Action _toolbarChanged;

    private ActionsToolbarItem? SelectedItem => ActionList.SelectedItem as ActionsToolbarItem;

    public ActionsToolbarEditorWindow() : this(() => { })
    {
    }

    public ActionsToolbarEditorWindow(Action toolbarChanged)
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _toolbarChanged = toolbarChanged;

        Program.Settings.ActionsToolbarList ??= [];
        foreach (HotkeyType action in Program.Settings.ActionsToolbarList)
        {
            _items.Add(new ActionsToolbarItem(action));
        }

        ActionList.ItemsSource = _items;
        ActionList.SelectedItem = _items.FirstOrDefault();
        UpdateSelectionState();
        KeyDown += OnWindowKeyDown;
        Opened += (_, _) => Activate();
    }

    private void OnAddClick(object? sender, RoutedEventArgs e)
    {
        ContextMenu menu = BuildActionMenu();
        menu.Placement = PlacementMode.BottomEdgeAlignedLeft;
        menu.Open(AddButton);
    }

    private ContextMenu BuildActionMenu()
    {
        List<MenuItem> rootItems = [];
        Dictionary<string, List<MenuItem>> categories = [];

        foreach (HotkeyType action in Helpers.GetEnums<HotkeyType>())
        {
            EnumInfo info = new(action);
            string title = action == HotkeyType.None
                ? Properties.Resources.ActionsToolbarEditForm_Separator
                : action.GetLocalizedDescription();

            MenuItem item = new()
            {
                Header = title,
                Icon = CreateIcon(TaskHelpers.FindMenuLucideIcon(action))
            };
            item.Click += (_, _) => AddAction(action);

            if (string.IsNullOrWhiteSpace(info.Category))
            {
                rootItems.Add(item);
            }
            else
            {
                if (!categories.TryGetValue(info.Category, out List<MenuItem>? items))
                {
                    items = [];
                    categories.Add(info.Category, items);
                }

                items.Add(item);
            }
        }

        foreach ((string category, List<MenuItem> items) in categories)
        {
            rootItems.Add(new MenuItem { Header = category, ItemsSource = items });
        }

        return new ContextMenu { ItemsSource = rootItems };
    }

    private static TextBlock CreateIcon(string icon) => new()
    {
        Classes = { "icon" },
        Text = icon,
        FontSize = 15,
        Width = 18,
        TextAlignment = Avalonia.Media.TextAlignment.Center
    };

    private void AddAction(HotkeyType action)
    {
        ActionsToolbarItem item = new(action);
        Program.Settings.ActionsToolbarList.Add(action);
        _items.Add(item);
        ActionList.SelectedItem = item;
        NotifyChanged();
    }

    private void OnRemoveClick(object? sender, RoutedEventArgs e)
    {
        if (SelectedItem is not { } item)
        {
            return;
        }

        int index = _items.IndexOf(item);
        Program.Settings.ActionsToolbarList.RemoveAt(index);
        _items.RemoveAt(index);
        ActionList.SelectedItem = _items.Count == 0 ? null : _items[Math.Min(index, _items.Count - 1)];
        NotifyChanged();
    }

    private void OnMoveUpClick(object? sender, RoutedEventArgs e) => MoveSelected(-1);

    private void OnMoveDownClick(object? sender, RoutedEventArgs e) => MoveSelected(1);

    private void MoveSelected(int offset)
    {
        if (SelectedItem is not { } item)
        {
            return;
        }

        int oldIndex = _items.IndexOf(item);
        int newIndex = oldIndex + offset;
        if (newIndex < 0 || newIndex >= _items.Count)
        {
            return;
        }

        _items.Move(oldIndex, newIndex);
        HotkeyType model = Program.Settings.ActionsToolbarList[oldIndex];
        Program.Settings.ActionsToolbarList.RemoveAt(oldIndex);
        Program.Settings.ActionsToolbarList.Insert(newIndex, model);
        ActionList.SelectedItem = item;
        NotifyChanged();
    }

    private void NotifyChanged()
    {
        _toolbarChanged();
        SettingManager.SaveApplicationConfigAsync();
        UpdateSelectionState();
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => UpdateSelectionState();

    private void UpdateSelectionState()
    {
        int index = SelectedItem == null ? -1 : _items.IndexOf(SelectedItem);
        RemoveButton.IsEnabled = index >= 0;
        MoveUpButton.IsEnabled = index > 0;
        MoveDownButton.IsEnabled = index >= 0 && index < _items.Count - 1;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
            e.Handled = true;
        }
        else if (e.Key == Key.Delete)
        {
            OnRemoveClick(sender, e);
            e.Handled = true;
        }
    }
}

public sealed class ActionsToolbarItem
{
    public HotkeyType Action { get; }
    public bool IsSeparator => Action == HotkeyType.None;
    public string Title => IsSeparator ? Properties.Resources.ActionsToolbarEditForm_Separator : Action.GetLocalizedDescription();
    public string Icon => TaskHelpers.FindMenuLucideIcon(Action);

    public ActionsToolbarItem(HotkeyType action)
    {
        Action = action;
    }
}
