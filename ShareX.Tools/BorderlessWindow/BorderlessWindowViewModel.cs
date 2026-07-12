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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ShareX.Tools;

public sealed class BorderlessWindowOptions
{
    public bool RememberWindowTitle { get; set; } = true;
    public string WindowTitle { get; set; } = string.Empty;
    public bool AutoCloseWindow { get; set; }
    public bool ExcludeTaskbarArea { get; set; }
}

public sealed partial class BorderlessWindowViewModel : ViewModelBase, IDisposable
{
    private readonly BorderlessWindowOptions _options;
    private readonly Func<string, bool, bool> _toggleWindow;
    private readonly Action<BorderlessWindowOptions>? _settingsChanged;
    private readonly Action? _playNotificationSound;
    private bool _updatingWindowList;
    private IntPtr _ignoredWindowHandle;

    public ObservableCollection<InspectWindowListItem> Windows { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanToggle))]
    private string _windowTitle = string.Empty;

    [ObservableProperty]
    private InspectWindowListItem? _selectedWindow;

    [ObservableProperty]
    private bool _rememberWindowTitle;

    [ObservableProperty]
    private bool _autoCloseWindow;

    [ObservableProperty]
    private bool _excludeTaskbarArea;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public Action? CloseRequested { get; set; }
    public bool CanToggle => !string.IsNullOrWhiteSpace(WindowTitle);
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public BorderlessWindowViewModel(
        BorderlessWindowOptions options,
        Func<string, bool, bool> toggleWindow,
        Action<BorderlessWindowOptions>? settingsChanged = null,
        Action? playNotificationSound = null)
    {
        _options = options;
        _toggleWindow = toggleWindow;
        _settingsChanged = settingsChanged;
        _playNotificationSound = playNotificationSound;

        _rememberWindowTitle = options.RememberWindowTitle;
        _autoCloseWindow = options.AutoCloseWindow;
        _excludeTaskbarArea = options.ExcludeTaskbarArea;
        _windowTitle = options.RememberWindowTitle ? options.WindowTitle : string.Empty;
    }

    public void SetIgnoredWindowHandle(IntPtr handle)
    {
        _ignoredWindowHandle = handle;
        ReloadWindowList();
    }

    public void ReloadWindowList()
    {
        _updatingWindowList = true;
        try
        {
            DisposeWindowList();
            Windows.Clear();
            foreach (InspectWindowListItem window in InspectWindowService.GetVisibleWindows(_ignoredWindowHandle))
            {
                Windows.Add(window);
            }
            SelectedWindow = null;
        }
        finally
        {
            _updatingWindowList = false;
        }
    }

    partial void OnSelectedWindowChanged(InspectWindowListItem? value)
    {
        if (!_updatingWindowList && value != null)
        {
            WindowTitle = value.Title;
            ErrorMessage = string.Empty;
        }
    }

    partial void OnWindowTitleChanged(string value)
    {
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(HasError));
    }

    partial void OnRememberWindowTitleChanged(bool value) => SaveOptions();
    partial void OnAutoCloseWindowChanged(bool value) => SaveOptions();
    partial void OnExcludeTaskbarAreaChanged(bool value) => SaveOptions();

    [RelayCommand]
    private void Toggle()
    {
        string title = WindowTitle.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            return;
        }

        try
        {
            if (!_toggleWindow(title, ExcludeTaskbarArea))
            {
                ErrorMessage = "Unable to find a window with the specified title.";
                OnPropertyChanged(nameof(HasError));
                return;
            }

            _options.WindowTitle = RememberWindowTitle ? title : string.Empty;
            SaveOptions();
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(HasError));
            _playNotificationSound?.Invoke();

            if (AutoCloseWindow)
            {
                CloseRequested?.Invoke();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unable to toggle the selected window. {ex.Message}";
            OnPropertyChanged(nameof(HasError));
            ToolsDiagnostics.ReportWarning(nameof(BorderlessWindowViewModel), "Failed to toggle borderless window state.", ex);
        }
    }

    private void SaveOptions()
    {
        _options.RememberWindowTitle = RememberWindowTitle;
        _options.AutoCloseWindow = AutoCloseWindow;
        _options.ExcludeTaskbarArea = ExcludeTaskbarArea;
        _settingsChanged?.Invoke(_options);
    }

    private void DisposeWindowList()
    {
        foreach (InspectWindowListItem window in Windows)
        {
            window.Dispose();
        }
    }

    public void Dispose()
    {
        DisposeWindowList();
        Windows.Clear();
    }
}
