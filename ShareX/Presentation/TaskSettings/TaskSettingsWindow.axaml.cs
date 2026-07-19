#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using ShareX.AvaloniaUI.Theming;
using System.Collections.Generic;

namespace ShareX;

public partial class TaskSettingsWindow : Window
{
    private TaskSettingsViewModel? _viewModel;
    private IReadOnlyDictionary<string, Control> _pages = new Dictionary<string, Control>();

    public TaskSettingsWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
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
}
