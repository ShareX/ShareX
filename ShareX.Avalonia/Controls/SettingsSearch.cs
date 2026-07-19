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
using Avalonia.LogicalTree;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace ShareX.AvaloniaUI.Controls;

public sealed class SettingsSearch : AvaloniaObject
{
    public static readonly AttachedProperty<string?> PageIdProperty =
        AvaloniaProperty.RegisterAttached<SettingsSearch, Control, string?>("PageId");

    public static readonly AttachedProperty<bool> IsPageTitleProperty =
        AvaloniaProperty.RegisterAttached<SettingsSearch, Control, bool>("IsPageTitle");

    public static readonly AttachedProperty<bool> IsPanelProperty =
        AvaloniaProperty.RegisterAttached<SettingsSearch, Control, bool>("IsPanel");

    public static readonly AttachedProperty<bool> IsAvailabilityContainerProperty =
        AvaloniaProperty.RegisterAttached<SettingsSearch, Control, bool>("IsAvailabilityContainer");

    private static readonly string[] SearchablePropertyNames = ["Text", "Content", "Header", "Watermark"];
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> SearchableProperties = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> SearchableItemProperties = new();

    private SettingsSearch()
    {
    }

    public static string? GetPageId(Control control) => control.GetValue(PageIdProperty);
    public static void SetPageId(Control control, string? value) => control.SetValue(PageIdProperty, value);

    public static bool GetIsPageTitle(Control control) => control.GetValue(IsPageTitleProperty);
    public static void SetIsPageTitle(Control control, bool value) => control.SetValue(IsPageTitleProperty, value);

    public static bool GetIsPanel(Control control) => control.GetValue(IsPanelProperty);
    public static void SetIsPanel(Control control, bool value) => control.SetValue(IsPanelProperty, value);

    public static bool GetIsAvailabilityContainer(Control control) => control.GetValue(IsAvailabilityContainerProperty);
    public static void SetIsAvailabilityContainer(Control control, bool value) => control.SetValue(IsAvailabilityContainerProperty, value);

    internal static void Apply(Control root, IEnumerable<SettingsNavigationItem> navigationItems, string? query)
    {
        query ??= string.Empty;
        SettingsNavigationItem[] flattenedItems = Flatten(navigationItems).ToArray();

        foreach (Control page in root.GetLogicalDescendants().OfType<Control>().Where(x => !string.IsNullOrEmpty(GetPageId(x))))
        {
            string pageId = GetPageId(page)!;
            SettingsNavigationItem? navigationItem = flattenedItems.FirstOrDefault(x => x.Id == pageId);
            if (navigationItem == null)
            {
                continue;
            }

            string pageTitle = page.GetLogicalDescendants()
                .OfType<Control>()
                .FirstOrDefault(GetIsPageTitle) is { } titleControl
                ? GetDisplayedSearchText(titleControl)
                : navigationItem.Title;

            Control[] panels = page.GetLogicalDescendants()
                .OfType<Control>()
                .Where(GetIsPanel)
                .Where(x => !HasPanelAncestor(x, page))
                .ToArray();

            List<string> pageSearchText = [pageTitle, GetItemsSourceSearchText(page)];

            foreach (Control panel in panels)
            {
                bool isAvailable = IsPanelAvailable(panel, page);
                string panelSearchText = GetDisplayedSearchText(panel);

                if (isAvailable)
                {
                    pageSearchText.Add(panelSearchText);
                }

                panel.IsVisible = isAvailable && Matches(string.Join(' ', pageTitle, panelSearchText), query);
            }

            navigationItem.UpdateSearchText(string.Join(' ', pageSearchText));
        }
    }

    private static bool HasPanelAncestor(Control panel, Control page)
    {
        ILogical? ancestor = panel.GetLogicalParent();

        while (ancestor != null && ancestor != page)
        {
            if (ancestor is Control control && GetIsPanel(control))
            {
                return true;
            }

            ancestor = ancestor.GetLogicalParent();
        }

        return false;
    }

    private static bool IsPanelAvailable(Control panel, Control page)
    {
        ILogical? ancestor = panel.GetLogicalParent();

        while (ancestor != null && ancestor != page)
        {
            if (ancestor is Control control && GetIsAvailabilityContainer(control) && !control.IsVisible)
            {
                return false;
            }

            ancestor = ancestor.GetLogicalParent();
        }

        return true;
    }

    private static string GetDisplayedSearchText(Control root)
    {
        List<string> values = [];
        AddDisplayedText(root, values);
        AddItemsSourceText(root, values);

        foreach (Control control in root.GetLogicalDescendants().OfType<Control>().Where(x => x.IsVisible))
        {
            AddDisplayedText(control, values);
            AddItemsSourceText(control, values);
        }

        return string.Join(' ', values);
    }

    private static void AddDisplayedText(Control control, List<string> values)
    {
        PropertyInfo[] properties = SearchableProperties.GetOrAdd(control.GetType(), static type => SearchablePropertyNames
            .Select(name => type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public))
            .Where(property => property is { CanRead: true } && property.GetIndexParameters().Length == 0)
            .Cast<PropertyInfo>()
            .ToArray());

        foreach (PropertyInfo property in properties)
        {
            if (control is TextBox && property.Name == "Text")
            {
                continue;
            }

            try
            {
                if (property.GetValue(control) is string text && !string.IsNullOrWhiteSpace(text))
                {
                    values.Add(text);
                }
            }
            catch (TargetInvocationException)
            {
                // A custom control property should not be able to break settings search.
            }
        }
    }

    private static string GetItemsSourceSearchText(Control root)
    {
        List<string> values = [];
        AddItemsSourceText(root, values);

        foreach (ItemsControl itemsControl in root.GetLogicalDescendants().OfType<ItemsControl>())
        {
            AddItemsSourceText(itemsControl, values);
        }

        return string.Join(' ', values);
    }

    private static void AddItemsSourceText(Control control, List<string> values)
    {
        if (control is ItemsControl { ItemsSource: IEnumerable items })
        {
            AddSearchableItemText(items, values, 0);
        }
    }

    private static void AddSearchableItemText(object? value, List<string> values, int depth)
    {
        if (value == null || depth > 4)
        {
            return;
        }

        if (value is string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                values.Add(text);
            }

            return;
        }

        if (value is IEnumerable items)
        {
            foreach (object? item in items)
            {
                AddSearchableItemText(item, values, depth + 1);
            }

            return;
        }

        PropertyInfo[] properties = SearchableItemProperties.GetOrAdd(value.GetType(), static type => type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanRead && property.GetIndexParameters().Length == 0 &&
                (property.PropertyType == typeof(string) || typeof(IEnumerable).IsAssignableFrom(property.PropertyType)))
            .ToArray());

        foreach (PropertyInfo property in properties)
        {
            try
            {
                AddSearchableItemText(property.GetValue(value), values, depth + 1);
            }
            catch (TargetInvocationException)
            {
                // A custom item property should not be able to break settings search.
            }
        }
    }

    private static bool Matches(string searchText, string query)
    {
        string[] terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return terms.Length == 0 || terms.All(term => searchText.Contains(term, StringComparison.CurrentCultureIgnoreCase));
    }

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
