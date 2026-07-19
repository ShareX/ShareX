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
using Avalonia.Data;
using Avalonia.Threading;

namespace ShareX.AvaloniaUI.Controls;

public partial class SettingsNavigation : UserControl
{
    public static readonly StyledProperty<IEnumerable<SettingsNavigationItem>?> ItemsProperty =
        AvaloniaProperty.Register<SettingsNavigation, IEnumerable<SettingsNavigationItem>?>(nameof(Items));

    public static readonly StyledProperty<SettingsNavigationItem?> SelectedItemProperty =
        AvaloniaProperty.Register<SettingsNavigation, SettingsNavigationItem?>(
            nameof(SelectedItem),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> SearchTextProperty =
        AvaloniaProperty.Register<SettingsNavigation, string>(
            nameof(SearchText),
            string.Empty,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<Control?> SearchRootProperty =
        AvaloniaProperty.Register<SettingsNavigation, Control?>(nameof(SearchRoot));

    private bool _filterUpdateQueued;

    public IEnumerable<SettingsNavigationItem>? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public SettingsNavigationItem? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public string SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public Control? SearchRoot
    {
        get => GetValue(SearchRootProperty);
        set => SetValue(SearchRootProperty, value);
    }

    public SettingsNavigation()
    {
        InitializeComponent();
        SearchTextProperty.Changed.AddClassHandler<SettingsNavigation>((control, _) => control.QueueFilterUpdate());
        ItemsProperty.Changed.AddClassHandler<SettingsNavigation>((control, _) => control.QueueFilterUpdate());
        SelectedItemProperty.Changed.AddClassHandler<SettingsNavigation>((control, _) => control.QueueFilterUpdate());
        SearchRootProperty.Changed.AddClassHandler<SettingsNavigation>((control, _) => control.QueueFilterUpdate());
        AttachedToLogicalTree += (_, _) => QueueFilterUpdate();
    }

    private void QueueFilterUpdate()
    {
        if (_filterUpdateQueued)
        {
            return;
        }

        _filterUpdateQueued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _filterUpdateQueued = false;
            ApplyFilter();
        }, DispatcherPriority.Loaded);
    }

    private void ApplyFilter()
    {
        if (Items == null)
        {
            return;
        }

        if (SearchRoot != null)
        {
            SettingsSearch.Apply(SearchRoot, Items, SearchText);
        }

        foreach (SettingsNavigationItem item in Items)
        {
            item.ApplyFilter(SearchText);
        }

        if (SelectedItem?.IsVisible != true)
        {
            SelectedItem = Flatten(Items).FirstOrDefault(x => x.IsVisible && x.Children.Count == 0) ??
                Flatten(Items).FirstOrDefault(x => x.IsVisible);
        }
    }

    public void RefreshFilter() => QueueFilterUpdate();

    private static IEnumerable<SettingsNavigationItem> Flatten(IEnumerable<SettingsNavigationItem> items)
    {
        foreach (SettingsNavigationItem item in items)
        {
            yield return item;

            foreach (SettingsNavigationItem child in Flatten(item.Children))
            {
                yield return child;
            }
        }
    }
}
