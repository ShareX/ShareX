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

using ShareX.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShareX;

public enum HotkeyRegistrationState
{
    NotConfigured,
    Registered,
    Failed
}

public sealed record HotkeyGesture(
    string KeyName,
    bool Control,
    bool Shift,
    bool Alt,
    bool Win);

public sealed record HotkeyTaskOption(int Value, string Name, string Category, string Icon);

public sealed class HotkeySettingsItem : INotifyPropertyChanged
{
    private HotkeyTaskOption? _selectedTask;
    private string _hotkeyText = string.Empty;
    private HotkeyRegistrationState _registrationState;
    private bool _isCustomized;
    private bool _isSelected;
    private bool _isCapturing;

    public object Source { get; }
    public IReadOnlyList<HotkeyTaskOption> TaskOptions { get; }

    public HotkeyTaskOption? SelectedTask
    {
        get => _selectedTask;
        set => SetField(ref _selectedTask, value);
    }

    public string HotkeyText
    {
        get => _hotkeyText;
        set
        {
            if (SetField(ref _hotkeyText, value))
            {
                OnPropertyChanged(nameof(DisplayHotkey));
            }
        }
    }

    public string DisplayHotkey => IsCapturing ? Strings.HotkeySettingsWindow_PressAHotkey : HotkeyText;

    public HotkeyRegistrationState RegistrationState
    {
        get => _registrationState;
        set
        {
            if (SetField(ref _registrationState, value))
            {
                OnPropertyChanged(nameof(IsRegistered));
                OnPropertyChanged(nameof(IsFailed));
                OnPropertyChanged(nameof(IsNotConfigured));
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }

    public bool IsRegistered => RegistrationState == HotkeyRegistrationState.Registered;
    public bool IsFailed => RegistrationState == HotkeyRegistrationState.Failed;
    public bool IsNotConfigured => RegistrationState == HotkeyRegistrationState.NotConfigured;

    public string StatusText => RegistrationState switch
    {
        HotkeyRegistrationState.Registered => Strings.HotkeySettingsWindow_Registered,
        HotkeyRegistrationState.Failed => Strings.HotkeySettingsWindow_RegistrationFailed,
        _ => Strings.HotkeySettingsWindow_NotConfigured
    };

    public bool IsCustomized
    {
        get => _isCustomized;
        set => SetField(ref _isCustomized, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }

    public bool IsCapturing
    {
        get => _isCapturing;
        set
        {
            if (SetField(ref _isCapturing, value))
            {
                OnPropertyChanged(nameof(DisplayHotkey));
            }
        }
    }

    public HotkeySettingsItem(object source, IReadOnlyList<HotkeyTaskOption> taskOptions)
    {
        Source = source;
        TaskOptions = taskOptions;
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

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public interface IHotkeySettingsService : IDisposable
{
    event EventHandler? StateChanged;

    bool AreHotkeysDisabled { get; }
    IReadOnlyList<HotkeySettingsItem> GetItems();
    HotkeySettingsItem Add();
    HotkeySettingsItem Duplicate(HotkeySettingsItem item);
    void Remove(HotkeySettingsItem item);
    void Move(HotkeySettingsItem item, int offset);
    void SetTask(HotkeySettingsItem item, int taskValue);
    void EditTask(HotkeySettingsItem item);
    bool SetHotkey(HotkeySettingsItem item, HotkeyGesture gesture, bool finalize);
    void FinalizeHotkey(HotkeySettingsItem item);
    void SetCaptureMode(bool isCapturing);
    void Refresh(HotkeySettingsItem item);
    void Reset();
    void EnableHotkeys();
}
