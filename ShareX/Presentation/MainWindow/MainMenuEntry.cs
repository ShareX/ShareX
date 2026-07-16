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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareX;

internal enum MainMenuToggleType
{
    None,
    CheckBox,
    Radio
}

internal sealed class MainMenuEntry
{
    public string Header { get; }
    public string Icon { get; }
    public Func<Task>? ExecuteAsync { get; }
    public Func<IReadOnlyList<MainMenuEntry>>? CreateChildren { get; }
    public bool IsSeparator { get; }
    public bool IsEnabled { get; }
    public bool IsVisible { get; }
    public bool IsChecked { get; }
    public MainMenuToggleType ToggleType { get; }

    public MainMenuEntry(
        string header,
        string icon,
        Action? execute = null,
        Func<IReadOnlyList<MainMenuEntry>>? createChildren = null,
        bool isEnabled = true,
        bool isVisible = true,
        bool isChecked = false,
        MainMenuToggleType toggleType = MainMenuToggleType.None)
    {
        Header = header;
        Icon = icon;
        ExecuteAsync = execute == null ? null : () =>
        {
            execute();
            return Task.CompletedTask;
        };
        CreateChildren = createChildren;
        IsEnabled = isEnabled;
        IsVisible = isVisible;
        IsChecked = isChecked;
        ToggleType = toggleType;
    }

    public MainMenuEntry(
        string header,
        string icon,
        Func<Task> executeAsync,
        Func<IReadOnlyList<MainMenuEntry>>? createChildren = null,
        bool isEnabled = true,
        bool isVisible = true,
        bool isChecked = false,
        MainMenuToggleType toggleType = MainMenuToggleType.None)
    {
        Header = header;
        Icon = icon;
        ExecuteAsync = executeAsync;
        CreateChildren = createChildren;
        IsEnabled = isEnabled;
        IsVisible = isVisible;
        IsChecked = isChecked;
        ToggleType = toggleType;
    }

    private MainMenuEntry()
    {
        Header = string.Empty;
        Icon = string.Empty;
        IsSeparator = true;
        IsEnabled = false;
        IsVisible = true;
    }

    public static MainMenuEntry Separator() => new();
}

internal sealed class MainNavigationSection
{
    public string Header { get; }
    public string Icon { get; }
    public Func<IReadOnlyList<MainMenuEntry>>? CreateChildren { get; }
    public Action? Execute { get; }
    public bool IsVisible { get; }

    public MainNavigationSection(string header, string icon, Func<IReadOnlyList<MainMenuEntry>> createChildren, bool isVisible = true)
    {
        Header = header;
        Icon = icon;
        CreateChildren = createChildren;
        IsVisible = isVisible;
    }

    public MainNavigationSection(string header, string icon, Action execute, bool isVisible = true)
    {
        Header = header;
        Icon = icon;
        Execute = execute;
        IsVisible = isVisible;
    }
}
