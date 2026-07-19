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
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ShareX;

public partial class ApplicationSettingsWindow : Window
{
    private static readonly string[] SearchablePropertyNames = ["Text", "Content", "Header", "Watermark"];
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> SearchableProperties = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> SearchableItemProperties = new();

    private bool _searchUpdateQueued;

    private ApplicationSettingsViewModel ViewModel => (ApplicationSettingsViewModel)DataContext!;

    public ApplicationSettingsWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        DataContext = new ApplicationSettingsViewModel();
        Opened += OnOpened;
        Closed += OnClosed;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Activate();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        QueueSettingsSearch();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        ViewModel.Dispose();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ApplicationSettingsViewModel.SettingsSearchText) ||
            e.PropertyName == nameof(ApplicationSettingsViewModel.SelectedNavigationItem))
        {
            QueueSettingsSearch();
        }
    }

    private void QueueSettingsSearch()
    {
        if (_searchUpdateQueued)
        {
            return;
        }

        _searchUpdateQueued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _searchUpdateQueued = false;
            ApplySettingsSearch();
        }, DispatcherPriority.Loaded);
    }

    private void ApplySettingsSearch()
    {
        string query = ViewModel.SettingsSearchText;

        foreach (ScrollViewer page in this.GetLogicalDescendants().OfType<ScrollViewer>().Where(x => x.Tag is string))
        {
            string pageId = (string)page.Tag!;
            var navigationItem = ViewModel.NavigationItems.FirstOrDefault(x => x.Id == pageId);
            if (navigationItem == null)
            {
                continue;
            }

            string pageTitle = page.GetLogicalDescendants()
                .OfType<TextBlock>()
                .FirstOrDefault(x => x.Classes.Contains("page-title"))?.Text ?? navigationItem.Title;

            Control[] panels = page.GetLogicalDescendants()
                .OfType<Control>()
                .Where(IsSearchPanel)
                .Where(x => !HasSearchPanelAncestor(x, page))
                .ToArray();

            List<string> pageSearchText = [pageTitle];
            pageSearchText.Add(GetItemsSourceSearchText(page));

            foreach (Control panel in panels)
            {
                bool isAvailable = IsPanelAvailable(panel, page);
                string panelSearchText = GetDisplayedSearchText(panel);

                if (isAvailable)
                {
                    pageSearchText.Add(panelSearchText);
                }

                panel.IsVisible = isAvailable && MatchesSearch(string.Join(' ', pageTitle, panelSearchText), query);
            }

            navigationItem.UpdateSearchText(string.Join(' ', pageSearchText));
        }

        SettingsNav.RefreshFilter();
    }

    private static bool IsSearchPanel(Control control) =>
        control.Classes.Contains("section-card") || control.Classes.Contains("search-panel");

    private static bool HasSearchPanelAncestor(Control panel, Control page)
    {
        ILogical? ancestor = panel.GetLogicalParent();

        while (ancestor != null && ancestor != page)
        {
            if (ancestor is Control control && IsSearchPanel(control))
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
            if (ancestor is Control control && control.Classes.Contains("search-availability") && !control.IsVisible)
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
            // Editable values are setting data, not labels, and can contain private information.
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

    private static bool MatchesSearch(string searchText, string query)
    {
        string[] terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return terms.Length == 0 || terms.All(term => searchText.Contains(term, StringComparison.CurrentCultureIgnoreCase));
    }

    private void OnRestartClick(object? sender, RoutedEventArgs e) => ViewModel.Restart();
    private void OnEditQuickTaskMenuClick(object? sender, RoutedEventArgs e) => ViewModel.EditQuickTaskMenu();
    private async void OnCheckDevBuildClick(object? sender, RoutedEventArgs e) => await ViewModel.CheckDevBuildAsync();
    private void OnOpenChromeExtensionClick(object? sender, RoutedEventArgs e) => ViewModel.OpenChromeExtensionPage();
    private void OnOpenFirefoxAddonClick(object? sender, RoutedEventArgs e) => ViewModel.OpenFirefoxAddonPage();
    private void OnOpenPersonalFolderClick(object? sender, RoutedEventArgs e) => ViewModel.OpenPersonalFolder();
    private void OnOpenScreenshotsFolderClick(object? sender, RoutedEventArgs e) => ViewModel.OpenScreenshotsFolder();
    private void OnResetThumbnailSizeClick(object? sender, RoutedEventArgs e) => ViewModel.ResetThumbnailSize();
    private void OnAddClipboardFormatClick(object? sender, RoutedEventArgs e) => ViewModel.AddClipboardFormat();
    private void OnRemoveClipboardFormatClick(object? sender, RoutedEventArgs e) => ViewModel.RemoveSelectedClipboardFormat();
    private void OnMoveImageUploaderUpClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedImageUploader(-1);
    private void OnMoveImageUploaderDownClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedImageUploader(1);
    private void OnMoveTextUploaderUpClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedTextUploader(-1);
    private void OnMoveTextUploaderDownClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedTextUploader(1);
    private void OnMoveFileUploaderUpClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedFileUploader(-1);
    private void OnMoveFileUploaderDownClick(object? sender, RoutedEventArgs e) => ViewModel.MoveSelectedFileUploader(1);
    private void OnImagePrintSettingsClick(object? sender, RoutedEventArgs e) => ViewModel.ShowImagePrintSettings();
    private async void OnResetSettingsClick(object? sender, RoutedEventArgs e) => await ViewModel.ResetAsync();

    private async void OnBrowsePersonalFolderClick(object? sender, RoutedEventArgs e)
    {
        string? path = await PickFolderAsync("Choose ShareX personal folder path");
        if (!string.IsNullOrEmpty(path))
        {
            ViewModel.PersonalFolderPath = path;
        }
    }

    private async void OnBrowseScreenshotsFolderClick(object? sender, RoutedEventArgs e)
    {
        string? path = await PickFolderAsync("Choose screenshots folder path");
        if (!string.IsNullOrEmpty(path))
        {
            ViewModel.CustomScreenshotsPath = path;
        }
    }

    private async System.Threading.Tasks.Task<string?> PickFolderAsync(string title)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });
        return folders.FirstOrDefault()?.TryGetLocalPath();
    }

    private async void OnExportClick(object? sender, RoutedEventArgs e)
    {
        string machineName = FileHelpers.SanitizeFileName(Environment.MachineName.ToLowerInvariant());
        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export ShareX backup",
            SuggestedFileName = $"ShareX-{Helpers.GetApplicationVersion()}-{machineName}-backup.sxb",
            DefaultExtension = "sxb",
            FileTypeChoices =
            [
                new FilePickerFileType("ShareX backup") { Patterns = ["*.sxb"] },
                FilePickerFileTypes.All
            ]
        });

        string? path = file?.TryGetLocalPath();
        if (!string.IsNullOrEmpty(path))
        {
            await ViewModel.ExportAsync(path);
        }
    }

    private async void OnImportClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import ShareX backup",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("ShareX backup") { Patterns = ["*.sxb"] }]
        });

        string? path = files.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            await ViewModel.ImportAsync(path);
        }
    }
}
