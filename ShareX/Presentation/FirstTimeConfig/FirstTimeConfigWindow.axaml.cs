#region License Information (GPL v3)

/*
    ShareX - A program developed by ShareX Team
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Interactivity;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShareX;

public partial class FirstTimeConfigWindow : Window
{
    public FirstTimeConfigWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        DataContext = new FirstTimeConfigViewModel();
        Opened += (_, _) => Activate();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e) => Close();
}

public sealed class FirstTimeConfigViewModel : INotifyPropertyChanged
{
    private bool _startWithWindows;
    private bool _startWithWindowsEnabled;
    private string _startWithWindowsText = string.Empty;
    private bool _shellContextMenu;
    private bool _sendToMenu;
    private bool _steamShowInApp;
    private string _statusMessage = string.Empty;

    public bool StartWithWindows
    {
        get => _startWithWindows;
        set
        {
            if (!_startWithWindowsEnabled || _startWithWindows == value)
            {
                return;
            }

            TryApply(() => StartupManager.State = value ? StartupState.Enabled : StartupState.Disabled);
            RefreshStartupState();
        }
    }

    public bool StartWithWindowsEnabled
    {
        get => _startWithWindowsEnabled;
        private set => SetField(ref _startWithWindowsEnabled, value);
    }

    public string StartWithWindowsText
    {
        get => _startWithWindowsText;
        private set => SetField(ref _startWithWindowsText, value);
    }

    public bool ShellContextMenu
    {
        get => _shellContextMenu;
        set
        {
            if (_shellContextMenu == value)
            {
                return;
            }

            TryApply(() => IntegrationHelpers.CreateShellContextMenuButton(value));
            SetField(ref _shellContextMenu, ReadOnMainThread(IntegrationHelpers.CheckShellContextMenuButton));
        }
    }

    public bool SendToMenu
    {
        get => _sendToMenu;
        set
        {
            if (_sendToMenu == value)
            {
                return;
            }

            TryApply(() => IntegrationHelpers.CreateSendToMenuButton(value));
            SetField(ref _sendToMenu, ReadOnMainThread(IntegrationHelpers.CheckSendToMenuButton));
        }
    }

    public bool SteamShowInApp
    {
        get => _steamShowInApp;
        set
        {
            if (_steamShowInApp == value)
            {
                return;
            }

            TryApply(() => IntegrationHelpers.SteamShowInApp(value));
            SetField(ref _steamShowInApp, ReadOnMainThread(IntegrationHelpers.CheckSteamShowInApp));
        }
    }

    public bool SteamOptionVisible { get; }

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (SetField(ref _statusMessage, value))
            {
                OnPropertyChanged(nameof(HasStatusMessage));
            }
        }
    }

    public bool HasStatusMessage => !string.IsNullOrWhiteSpace(StatusMessage);

    public FirstTimeConfigViewModel()
    {
        RefreshStartupState();
        _shellContextMenu = ReadOnMainThread(IntegrationHelpers.CheckShellContextMenuButton);
        _sendToMenu = ReadOnMainThread(IntegrationHelpers.CheckSendToMenuButton);

#if STEAM
        SteamOptionVisible = true;
        _steamShowInApp = ReadOnMainThread(IntegrationHelpers.CheckSteamShowInApp);
#else
        SteamOptionVisible = false;
#endif
    }

    private void RefreshStartupState()
    {
        StartWithWindowsText = Resources.ApplicationSettingsForm_cbStartWithWindows_Text;
        StartWithWindowsEnabled = false;

        try
        {
            StartupState state = ReadOnMainThread(() => StartupManager.State);
            SetField(ref _startWithWindows, state == StartupState.Enabled || state == StartupState.EnabledByPolicy, nameof(StartWithWindows));

            if (state == StartupState.DisabledByUser)
            {
                StartWithWindowsText = Resources.ApplicationSettingsForm_cbStartWithWindows_DisabledByUser_Text;
            }
            else if (state == StartupState.DisabledByPolicy)
            {
                StartWithWindowsText = Resources.ApplicationSettingsForm_cbStartWithWindows_DisabledByPolicy_Text;
            }
            else if (state == StartupState.EnabledByPolicy)
            {
                StartWithWindowsText = Resources.ApplicationSettingsForm_cbStartWithWindows_EnabledByPolicy_Text;
            }
            else
            {
                StartWithWindowsEnabled = true;
            }
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            StatusMessage = exception.Message;
        }
    }

    private void TryApply(Action action)
    {
        try
        {
            RunOnMainThread(action);
            StatusMessage = string.Empty;
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            StatusMessage = exception.Message;
        }
    }

    private static void RunOnMainThread(Action action)
    {
        if (Program.MainForm?.InvokeRequired == true)
        {
            Program.MainForm.Invoke(action);
        }
        else
        {
            action();
        }
    }

    private static T ReadOnMainThread<T>(Func<T> action)
    {
        if (Program.MainForm?.InvokeRequired == true)
        {
            return (T)Program.MainForm.Invoke(action);
        }

        return action();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
