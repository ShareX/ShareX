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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShareX.AvaloniaUI.Controls;

public sealed class SettingsNavigationItem : INotifyPropertyChanged
{
    private bool _isVisible = true;
    private bool _isExpanded = true;
    private string _searchText;

    public string Id { get; }
    public string Title { get; }
    public string Icon { get; }
    public bool HasIcon => !string.IsNullOrEmpty(Icon);
    public string SearchText
    {
        get => _searchText;
        private set => SetField(ref _searchText, value);
    }
    public ObservableCollection<SettingsNavigationItem> Children { get; } = [];

    public bool IsVisible
    {
        get => _isVisible;
        internal set => SetField(ref _isVisible, value);
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetField(ref _isExpanded, value);
    }

    public SettingsNavigationItem(
        string id,
        string title,
        string icon = "",
        string? searchText = null,
        IEnumerable<SettingsNavigationItem>? children = null)
    {
        Id = id;
        Title = title;
        Icon = icon;
        _searchText = string.Join(' ', title, searchText ?? string.Empty);

        if (children != null)
        {
            foreach (SettingsNavigationItem child in children)
            {
                Children.Add(child);
            }
        }
    }

    public void UpdateSearchText(string? searchText)
    {
        SearchText = string.Join(' ', Title, searchText ?? string.Empty);
    }

    internal bool ApplyFilter(string query)
    {
        query ??= string.Empty;

        bool childMatches = false;

        foreach (SettingsNavigationItem child in Children)
        {
            childMatches |= child.ApplyFilter(query);
        }

        string[] terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        bool selfMatches = terms.Length == 0 ||
            terms.All(term => SearchText.Contains(term, StringComparison.CurrentCultureIgnoreCase));

        IsVisible = selfMatches || childMatches;

        if (!string.IsNullOrWhiteSpace(query) && childMatches)
        {
            IsExpanded = true;
        }

        return IsVisible;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
