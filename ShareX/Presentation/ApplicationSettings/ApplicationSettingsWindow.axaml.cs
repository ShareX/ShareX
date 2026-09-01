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
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ShareX;

public partial class ApplicationSettingsWindow : Window
{
    private ApplicationSettingsViewModel ViewModel => (ApplicationSettingsViewModel)DataContext!;
    private ClipboardFormatItem? _editedClipboardFormat;
    private ContextMenu? _activeCodeMenu;

    private static readonly string[] ClipboardResultTokens =
    [
        "$result", "$url", "$shorturl", "$thumbnailurl", "$deletionurl", "$filepath", "$filename",
        "$filenamenoext", "$thumbnailfilename", "$thumbnailfilenamenoext", "$folderpath", "$foldername", "$uploadtime"
    ];

    public ApplicationSettingsWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        DataContext = new ApplicationSettingsViewModel();
        AttachClipboardFormatMenu();
        AttachFilenamePatternMenu(SaveImageSubFolderPatternTextBox,
            CodeMenuEntryFilename.t, CodeMenuEntryFilename.pn, CodeMenuEntryFilename.i,
            CodeMenuEntryFilename.width, CodeMenuEntryFilename.height, CodeMenuEntryFilename.n);
        AttachFilenamePatternMenu(SaveImageSubFolderPatternWindowTextBox,
            CodeMenuEntryFilename.i, CodeMenuEntryFilename.n);
        ClipboardFormatSupportedVariablesText.Text = string.Format(
            Strings.ClipboardFormatForm_ClipboardFormatForm_Supported_variables___0__and_other_variables_such_as__1__etc_,
            string.Join(", ", ClipboardResultTokens),
            "%y, %mo, %d");
        KeyDown += OnWindowKeyDown;
        Opened += (_, _) => Activate();
        ThemeManager.ThemeChanged += OnThemeChanged;
        Closed += OnClosed;
    }

    private void OnThemeChanged(object? sender, Avalonia.Styling.ThemeVariant theme) =>
        Dispatcher.UIThread.Post(() => RequestedThemeVariant = theme);

    private void OnClosed(object? sender, EventArgs e)
    {
        ThemeManager.ThemeChanged -= OnThemeChanged;
        ViewModel.Dispose();
    }

    private void OnRestartClick(object? sender, RoutedEventArgs e) => ViewModel.Restart();
    private void OnEditQuickTaskMenuClick(object? sender, RoutedEventArgs e) => ViewModel.EditQuickTaskMenu();
    private async void OnCheckDevBuildClick(object? sender, RoutedEventArgs e) => await ViewModel.CheckDevBuildAsync();
    private void OnOpenChromeExtensionClick(object? sender, RoutedEventArgs e) => ViewModel.OpenChromeExtensionPage();
    private void OnOpenFirefoxAddonClick(object? sender, RoutedEventArgs e) => ViewModel.OpenFirefoxAddonPage();
    private void OnOpenPersonalFolderClick(object? sender, RoutedEventArgs e) => ViewModel.OpenPersonalFolder();
    private void OnOpenScreenshotsFolderClick(object? sender, RoutedEventArgs e) => ViewModel.OpenScreenshotsFolder();
    private void OnResetThumbnailSizeClick(object? sender, RoutedEventArgs e) => ViewModel.ResetThumbnailSize();
    private void OnAddClipboardFormatClick(object? sender, RoutedEventArgs e) => ShowClipboardFormatEditor(null);
    private void OnEditClipboardFormatClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedClipboardFormat is { } format)
        {
            ShowClipboardFormatEditor(format);
        }
    }
    private void OnRemoveClipboardFormatClick(object? sender, RoutedEventArgs e) => ViewModel.RemoveSelectedClipboardFormat();
    private void OnImagePrintSettingsClick(object? sender, RoutedEventArgs e) => ViewModel.ShowImagePrintSettings(this);
    private async void OnResetSettingsClick(object? sender, RoutedEventArgs e) => await ViewModel.ResetAsync();

    private async void OnBrowsePersonalFolderClick(object? sender, RoutedEventArgs e)
    {
        string? path = await PickFolderAsync(Strings.ApplicationSettingsWindow_ChooseShareXPersonalFolderPath);
        if (!string.IsNullOrEmpty(path))
        {
            ViewModel.PersonalFolderPath = path;
        }
    }

    private async void OnBrowseScreenshotsFolderClick(object? sender, RoutedEventArgs e)
    {
        string? path = await PickFolderAsync(Strings.ApplicationSettingsWindow_ChooseScreenshotsFolderPath);
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
            Title = Strings.ApplicationSettingsWindow_ExportShareXBackup,
            SuggestedFileName = $"ShareX-{Helpers.GetApplicationVersion()}-{machineName}-backup.sxb",
            DefaultExtension = "sxb",
            FileTypeChoices =
            [
                new FilePickerFileType(Strings.ApplicationSettingsWindow_ShareXBackup) { Patterns = ["*.sxb"] },
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
            Title = Strings.ApplicationSettingsWindow_ImportShareXBackup,
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType(Strings.ApplicationSettingsWindow_ShareXBackup) { Patterns = ["*.sxb"] }]
        });

        string? path = files.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            await ViewModel.ImportAsync(path);
        }
    }

    private void ShowClipboardFormatEditor(ClipboardFormatItem? format)
    {
        _editedClipboardFormat = format;
        ClipboardFormatEditorTitle.Text = format == null
            ? Strings.ApplicationSettingsWindow_AddClipboardFormat
            : Strings.ApplicationSettingsWindow_EditClipboardFormat;
        ClipboardFormatDescriptionBox.Text = format?.Description ?? string.Empty;
        ClipboardFormatTextBox.Text = format?.Format ?? string.Empty;
        ClipboardFormatEditorOverlay.IsVisible = true;

        Dispatcher.UIThread.Post(() =>
        {
            ClipboardFormatDescriptionBox.Focus();
            ClipboardFormatDescriptionBox.SelectAll();
        }, DispatcherPriority.Input);
    }

    private void HideClipboardFormatEditor()
    {
        ClipboardFormatEditorOverlay.IsVisible = false;
        _editedClipboardFormat = null;
    }

    private void OnSaveClipboardFormatClick(object? sender, RoutedEventArgs e)
    {
        string description = ClipboardFormatDescriptionBox.Text ?? string.Empty;
        string format = ClipboardFormatTextBox.Text ?? string.Empty;

        if (_editedClipboardFormat == null)
        {
            ViewModel.AddClipboardFormat(description, format);
        }
        else
        {
            _editedClipboardFormat.Description = description;
            _editedClipboardFormat.Format = format;
        }

        HideClipboardFormatEditor();
    }

    private void OnCancelClipboardFormatClick(object? sender, RoutedEventArgs e) => HideClipboardFormatEditor();

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && ClipboardFormatEditorOverlay.IsVisible)
        {
            HideClipboardFormatEditor();
            e.Handled = true;
        }
    }

    private void AttachClipboardFormatMenu()
    {
        List<MenuItem> menuItems = [];
        menuItems.Add(new MenuItem
        {
            Header = Strings.ApplicationSettingsWindow_UploadResult,
            ItemsSource = ClipboardResultTokens.Select(CreateClipboardTokenItem).ToList()
        });

        IEnumerable<CodeMenuEntryFilename> filenameEntries = typeof(CodeMenuEntryFilename)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(CodeMenuEntryFilename))
            .Select(field => field.GetValue(null))
            .OfType<CodeMenuEntryFilename>();

        foreach (IGrouping<string?, CodeMenuEntryFilename> group in filenameEntries.GroupBy(entry => entry.Category))
        {
            List<MenuItem> items = group.Select(entry => CreateClipboardTokenItem(entry.ToPrefixString(), entry.Description)).ToList();
            if (string.IsNullOrWhiteSpace(group.Key))
            {
                menuItems.AddRange(items);
            }
            else
            {
                menuItems.Add(new MenuItem { Header = group.Key, ItemsSource = items });
            }
        }

        ClipboardFormatTextBox.ContextMenu = new ContextMenu { ItemsSource = menuItems };
    }

    private void AttachFilenamePatternMenu(TextBox textBox, params CodeMenuEntryFilename[] ignoredEntries)
    {
        HashSet<CodeMenuEntryFilename> ignored = ignoredEntries.ToHashSet();
        IEnumerable<CodeMenuEntryFilename> entries = typeof(CodeMenuEntryFilename)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(CodeMenuEntryFilename))
            .Select(field => field.GetValue(null))
            .OfType<CodeMenuEntryFilename>()
            .Where(entry => !ignored.Contains(entry));

        MenuItem CreateItem(CodeMenuEntryFilename entry)
        {
            string pattern = entry.ToPrefixString();
            MenuItem item = new() { Header = $"{pattern} - {entry.Description}", Focusable = false };
            item.Click += (_, _) => InsertText(textBox, pattern);
            return item;
        }

        List<MenuItem> rootItems = [];
        foreach (IGrouping<string?, CodeMenuEntryFilename> group in entries.GroupBy(entry => entry.Category))
        {
            List<MenuItem> items = group.Select(CreateItem).ToList();
            if (string.IsNullOrWhiteSpace(group.Key))
            {
                rootItems.AddRange(items);
            }
            else
            {
                rootItems.Add(new MenuItem { Header = group.Key, ItemsSource = items, Focusable = false });
            }
        }

        ContextMenu menu = new()
        {
            Focusable = false,
            Placement = PlacementMode.RightEdgeAlignedTop,
            PlacementTarget = textBox,
            ItemsSource = rootItems
        };
        textBox.ContextMenu = menu;

        void OpenMenu()
        {
            if (_activeCodeMenu != null && !ReferenceEquals(_activeCodeMenu, menu))
            {
                _activeCodeMenu.Close();
            }

            if (!menu.IsOpen)
            {
                _activeCodeMenu = menu;
                menu.Open(textBox);
            }
        }

        menu.Closed += (_, _) =>
        {
            if (ReferenceEquals(_activeCodeMenu, menu))
            {
                _activeCodeMenu = null;
            }
        };
        textBox.GotFocus += (_, _) => OpenMenu();
        textBox.PointerReleased += (_, _) => OpenMenu();
    }

    private static void InsertText(TextBox textBox, string textToInsert)
    {
        string text = textBox.Text ?? string.Empty;
        int start = Math.Clamp(Math.Min(textBox.SelectionStart, textBox.SelectionEnd), 0, text.Length);
        textBox.Text = text.Insert(start, textToInsert);
        textBox.CaretIndex = start + textToInsert.Length;
        textBox.Focus();
    }

    private MenuItem CreateClipboardTokenItem(string token) => CreateClipboardTokenItem(token, null);

    private MenuItem CreateClipboardTokenItem(string token, string? description)
    {
        MenuItem item = new() { Header = token };
        if (!string.IsNullOrEmpty(description))
        {
            ToolTip.SetTip(item, description);
        }
        item.Click += (_, _) => InsertClipboardToken(token);
        return item;
    }

    private void InsertClipboardToken(string token)
    {
        string text = ClipboardFormatTextBox.Text ?? string.Empty;
        int start = Math.Clamp(Math.Min(ClipboardFormatTextBox.SelectionStart, ClipboardFormatTextBox.SelectionEnd), 0, text.Length);
        int end = Math.Clamp(Math.Max(ClipboardFormatTextBox.SelectionStart, ClipboardFormatTextBox.SelectionEnd), start, text.Length);
        ClipboardFormatTextBox.Text = text.Remove(start, end - start).Insert(start, token);
        ClipboardFormatTextBox.CaretIndex = start + token.Length;
        ClipboardFormatTextBox.Focus();
    }
}
