#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.Controls;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX;

public partial class TaskSettingsWindow : Window
{
    private TaskSettingsViewModel? _viewModel;
    private IReadOnlyDictionary<string, Control> _pages = new Dictionary<string, Control>();
    private ExternalProgram? _editedAction;
    private Action<ExternalProgram>? _actionSaved;
    private WatchFolderSettings? _editedWatchFolder;
    private Action<WatchFolderSettings>? _watchFolderSaved;

    public TaskSettingsWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        AttachActionArgumentsMenu();
        KeyDown += OnWindowKeyDown;
    }

    public TaskSettingsWindow(TaskSettings settings, bool isDefault) : this()
    {
        Title = isDefault
            ? "ShareX - " + Properties.Resources.TaskSettingsForm_UpdateWindowTitle_Task_settings
            : "ShareX - " + string.Format(Properties.Resources.TaskSettingsForm_UpdateWindowTitle_Task_settings_for__0_, settings);

        _viewModel = new TaskSettingsViewModel(isDefault);
        DataContext = _viewModel;

        _pages = new TaskSettingsPageBuilder(this, settings, isDefault).BuildPages();
        foreach (Control page in _pages.Values)
        {
            SettingsPages.Children.Add(page);
        }

        _viewModel.SelectedPageChanged += SelectPage;
        SelectPage(_viewModel.SelectedNavigationItem?.Id);

        Opened += (_, _) =>
        {
            Activate();
            Navigation.RefreshFilter();
        };
    }

    private void SelectPage(string? pageId)
    {
        foreach ((string id, Control page) in _pages)
        {
            page.IsVisible = id == pageId;
        }
    }

    internal void ShowActionEditor(ExternalProgram? action, Action<ExternalProgram> saved)
    {
        _editedAction = action;
        _actionSaved = saved;

        ExternalProgram values = action ?? new ExternalProgram();
        ActionEditorTitle.Text = action == null ? "Add action" : "Edit action";
        ActionNameBox.Text = values.Name ?? string.Empty;
        ActionPathBox.Text = values.Path ?? string.Empty;
        ActionArgumentsBox.Text = values.Args ?? string.Empty;
        ActionOutputExtensionBox.Text = values.OutputExtension ?? string.Empty;
        ActionExtensionsBox.Text = values.Extensions ?? string.Empty;
        ActionHiddenWindowCheckBox.IsChecked = values.HiddenWindow;
        ActionDeleteInputFileCheckBox.IsChecked = values.DeleteInputFile;
        ActionEditorErrorText.IsVisible = false;
        UpdateDeleteInputFileState();

        ActionEditorOverlay.IsVisible = true;
        Dispatcher.UIThread.Post(() =>
        {
            ActionNameBox.Focus();
            ActionNameBox.SelectAll();
        }, DispatcherPriority.Input);
    }

    private void HideActionEditor()
    {
        ActionEditorOverlay.IsVisible = false;
        _editedAction = null;
        _actionSaved = null;
    }

    private void OnCancelActionEditorClick(object? sender, RoutedEventArgs e) => HideActionEditor();

    private void OnSaveActionEditorClick(object? sender, RoutedEventArgs e)
    {
        string name = ActionNameBox.Text ?? string.Empty;
        string path = ActionPathBox.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowActionEditorError(Properties.Resources.ActionsForm_btnOK_Click_Name_can_t_be_empty_, ActionNameBox);
            return;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            ShowActionEditorError(Properties.Resources.ActionsForm_btnOK_Click_File_path_can_t_be_empty_, ActionPathBox);
            return;
        }

        ExternalProgram action = _editedAction ?? new ExternalProgram { IsActive = true };
        action.Name = name;
        action.Path = path;
        action.Args = ActionArgumentsBox.Text ?? string.Empty;
        action.OutputExtension = ActionOutputExtensionBox.Text ?? string.Empty;
        action.Extensions = ActionExtensionsBox.Text ?? string.Empty;
        action.HiddenWindow = ActionHiddenWindowCheckBox.IsChecked == true;
        action.DeleteInputFile = ActionDeleteInputFileCheckBox.IsChecked == true;

        Action<ExternalProgram>? saved = _actionSaved;
        HideActionEditor();
        saved?.Invoke(action);
    }

    private void ShowActionEditorError(string message, TextBox field)
    {
        ActionEditorErrorText.Text = message;
        ActionEditorErrorText.IsVisible = true;
        field.Focus();
        field.SelectAll();
    }

    private async void OnBrowseActionPathClick(object? sender, RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose program",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.All]
        });

        string? path = files.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrEmpty(path))
        {
            ActionPathBox.Text = path;
            ActionEditorErrorText.IsVisible = false;
        }
    }

    private void OnActionOutputExtensionTextChanged(object? sender, TextChangedEventArgs e) => UpdateDeleteInputFileState();

    private void UpdateDeleteInputFileState()
    {
        ActionDeleteInputFileCheckBox.IsEnabled = !string.IsNullOrEmpty(ActionOutputExtensionBox.Text);
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape)
        {
            return;
        }

        if (WatchFolderEditorOverlay.IsVisible)
        {
            HideWatchFolderEditor();
            e.Handled = true;
        }
        else if (ActionEditorOverlay.IsVisible)
        {
            HideActionEditor();
            e.Handled = true;
        }
    }

    private void AttachActionArgumentsMenu()
    {
        ContextMenu menu = new();
        AddArgumentToken(menu, CodeMenuEntryActions.input);
        AddArgumentToken(menu, CodeMenuEntryActions.output);
        ActionArgumentsBox.ContextMenu = menu;
    }

    private void AddArgumentToken(ContextMenu menu, CodeMenuEntryActions entry)
    {
        string token = entry.ToPrefixString();
        MenuItem item = new() { Header = $"{token} — {entry.Description}" };
        item.Click += (_, _) => InsertArgumentToken(token);
        menu.Items.Add(item);
    }

    private void InsertArgumentToken(string token)
    {
        string text = ActionArgumentsBox.Text ?? string.Empty;
        int start = Math.Clamp(Math.Min(ActionArgumentsBox.SelectionStart, ActionArgumentsBox.SelectionEnd), 0, text.Length);
        int end = Math.Clamp(Math.Max(ActionArgumentsBox.SelectionStart, ActionArgumentsBox.SelectionEnd), start, text.Length);
        ActionArgumentsBox.Text = text.Remove(start, end - start).Insert(start, token);
        ActionArgumentsBox.CaretIndex = start + token.Length;
        ActionArgumentsBox.Focus();
    }

    internal void ShowWatchFolderEditor(WatchFolderSettings? folder, Action<WatchFolderSettings> saved)
    {
        _editedWatchFolder = folder;
        _watchFolderSaved = saved;

        WatchFolderEditorTitle.Text = folder == null ? "Add watch folder" : "Edit watch folder";
        WatchFolderPathBox.Text = folder?.FolderPath ?? string.Empty;
        WatchFolderFilterBox.Text = folder?.Filter ?? string.Empty;
        WatchFolderIncludeSubdirectoriesCheckBox.IsChecked = folder?.IncludeSubdirectories ?? false;
        WatchFolderMoveToScreenshotsCheckBox.IsChecked = folder?.MoveFilesToScreenshotsFolder ?? false;
        WatchFolderEditorOverlay.IsVisible = true;

        Dispatcher.UIThread.Post(() =>
        {
            WatchFolderPathBox.Focus();
            WatchFolderPathBox.SelectAll();
        }, DispatcherPriority.Input);
    }

    private void HideWatchFolderEditor()
    {
        WatchFolderEditorOverlay.IsVisible = false;
        _editedWatchFolder = null;
        _watchFolderSaved = null;
    }

    private void OnSaveWatchFolderEditorClick(object? sender, RoutedEventArgs e)
    {
        WatchFolderSettings folder = _editedWatchFolder ?? new WatchFolderSettings();
        folder.FolderPath = WatchFolderPathBox.Text ?? string.Empty;
        folder.Filter = WatchFolderFilterBox.Text ?? string.Empty;
        folder.IncludeSubdirectories = WatchFolderIncludeSubdirectoriesCheckBox.IsChecked == true;
        folder.MoveFilesToScreenshotsFolder = WatchFolderMoveToScreenshotsCheckBox.IsChecked == true;

        Action<WatchFolderSettings>? saved = _watchFolderSaved;
        HideWatchFolderEditor();
        saved?.Invoke(folder);
    }

    private void OnCancelWatchFolderEditorClick(object? sender, RoutedEventArgs e) => HideWatchFolderEditor();

    private async void OnBrowseWatchFolderClick(object? sender, RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Choose watch folder",
            AllowMultiple = false
        });

        string? path = folders.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrEmpty(path))
        {
            WatchFolderPathBox.Text = path;
        }
    }
}
