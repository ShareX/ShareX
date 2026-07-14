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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ShareX;

internal sealed class HotkeySettingsAvaloniaService : IHotkeySettingsService
{
    private readonly HotkeyManager _manager;
    private readonly MainForm _mainForm;
    private readonly Action _closed;
    private readonly IReadOnlyList<HotkeyTaskOption> _taskOptions;
    private bool _disposed;

    public event EventHandler StateChanged;

    public bool AreHotkeysDisabled => InvokeOnMainThread(() => Program.Settings.DisableHotkeys);

    public HotkeySettingsAvaloniaService(HotkeyManager manager, MainForm mainForm, Action closed)
    {
        _manager = manager;
        _mainForm = mainForm;
        _closed = closed;
        _taskOptions = Helpers.GetEnums<HotkeyType>()
            .Select(value => new EnumInfo(value))
            .Select(info => new HotkeyTaskOption(
                Convert.ToInt32(info.Value),
                info.Description,
                info.Category ?? string.Empty,
                TaskHelpers.FindMenuLucideIcon((HotkeyType)info.Value)))
            .ToArray();

        InvokeOnMainThread(() =>
        {
            _manager.HotkeysToggledTrigger += OnHotkeysToggled;
            _manager.RegisterFailedHotkeys();
        });
    }

    public IReadOnlyList<HotkeySettingsItem> GetItems()
    {
        HotkeySettings[] settings = InvokeOnMainThread(() => _manager.Hotkeys.ToArray());
        return settings.Select(CreateItem).ToArray();
    }

    public HotkeySettingsItem Add()
    {
        HotkeySettings settings = InvokeOnMainThread(() =>
        {
            HotkeySettings newSettings = new()
            {
                TaskSettings = TaskSettings.GetDefaultTaskSettings()
            };
            _manager.Hotkeys.Add(newSettings);
            return newSettings;
        });

        return CreateItem(settings);
    }

    public HotkeySettingsItem Duplicate(HotkeySettingsItem item)
    {
        HotkeySettings source = GetSettings(item);
        HotkeySettings duplicate = InvokeOnMainThread(() =>
        {
            HotkeySettings newSettings = new()
            {
                TaskSettings = source.TaskSettings.Copy()
            };
            newSettings.TaskSettings.WatchFolderEnabled = false;
            newSettings.TaskSettings.WatchFolderList = [];
            _manager.Hotkeys.Add(newSettings);
            return newSettings;
        });

        return CreateItem(duplicate);
    }

    public void Remove(HotkeySettingsItem item)
    {
        HotkeySettings settings = GetSettings(item);
        InvokeOnMainThread(() => _manager.UnregisterHotkey(settings));
    }

    public void Move(HotkeySettingsItem item, int offset)
    {
        HotkeySettings settings = GetSettings(item);
        InvokeOnMainThread(() =>
        {
            int oldIndex = _manager.Hotkeys.IndexOf(settings);
            if (oldIndex < 0 || _manager.Hotkeys.Count < 2)
            {
                return;
            }

            int newIndex = (oldIndex + offset + _manager.Hotkeys.Count) % _manager.Hotkeys.Count;
            _manager.Hotkeys.RemoveAt(oldIndex);
            _manager.Hotkeys.Insert(newIndex, settings);
        });
    }

    public void SetTask(HotkeySettingsItem item, int taskValue)
    {
        HotkeySettings settings = GetSettings(item);
        InvokeOnMainThread(() => settings.TaskSettings.Job = (HotkeyType)taskValue);
    }

    public void EditTask(HotkeySettingsItem item)
    {
        HotkeySettings settings = GetSettings(item);
        InvokeOnMainThread(() =>
        {
            settings.TaskSettings.SetDefaultSettings();
            using TaskSettingsForm form = new(settings.TaskSettings);
            form.ShowDialog(_mainForm);
        });
    }

    public bool SetHotkey(HotkeySettingsItem item, HotkeyGesture gesture, bool finalize)
    {
        HotkeySettings settings = GetSettings(item);
        return InvokeOnMainThread(() =>
        {
            settings.HotkeyInfo.Hotkey = ConvertGesture(gesture);
            settings.HotkeyInfo.Win = gesture.Win;

            if (finalize && settings.HotkeyInfo.IsOnlyModifiers)
            {
                settings.HotkeyInfo.Hotkey = Keys.None;
                settings.HotkeyInfo.Win = false;
            }

            RegisterAndRetry(settings);
            return settings.HotkeyInfo.IsValidHotkey;
        });
    }

    public void FinalizeHotkey(HotkeySettingsItem item)
    {
        HotkeySettings settings = GetSettings(item);
        InvokeOnMainThread(() =>
        {
            if (settings.HotkeyInfo.IsOnlyModifiers)
            {
                settings.HotkeyInfo.Hotkey = Keys.None;
                settings.HotkeyInfo.Win = false;
            }

            RegisterAndRetry(settings);
        });
    }

    public void SetCaptureMode(bool isCapturing)
    {
        InvokeOnMainThread(() => _manager.IgnoreHotkeys = isCapturing);
    }

    public void Refresh(HotkeySettingsItem item)
    {
        HotkeySettings settings = GetSettings(item);
        HotkeyItemSnapshot snapshot = InvokeOnMainThread(() => new HotkeyItemSnapshot(
            Convert.ToInt32(settings.TaskSettings.Job),
            settings.HotkeyInfo.ToString(),
            settings.HotkeyInfo.Status switch
            {
                HotkeyStatus.Registered => HotkeyRegistrationState.Registered,
                HotkeyStatus.Failed => HotkeyRegistrationState.Failed,
                _ => HotkeyRegistrationState.NotConfigured
            },
            !settings.TaskSettings.IsUsingDefaultSettings));

        item.SelectedTask = _taskOptions.FirstOrDefault(x => x.Value == snapshot.TaskValue);
        item.HotkeyText = snapshot.HotkeyText;
        item.RegistrationState = snapshot.State;
        item.IsCustomized = snapshot.IsCustomized;
    }

    public void Reset()
    {
        InvokeOnMainThread(_manager.ResetHotkeys);
    }

    public void EnableHotkeys()
    {
        InvokeOnMainThread(() => TaskHelpers.ToggleHotkeys(false));
    }

    private HotkeySettingsItem CreateItem(HotkeySettings settings)
    {
        HotkeySettingsItem item = new(settings, _taskOptions);
        Refresh(item);
        return item;
    }

    private void RegisterAndRetry(HotkeySettings settings)
    {
        _manager.RegisterHotkey(settings);
        _manager.RegisterFailedHotkeys();
    }

    private static HotkeySettings GetSettings(HotkeySettingsItem item)
    {
        return (HotkeySettings)item.Source;
    }

    private static Keys ConvertGesture(HotkeyGesture gesture)
    {
        Keys key = gesture.KeyName switch
        {
            nameof(Avalonia.Input.Key.LeftCtrl) => Keys.LControlKey,
            nameof(Avalonia.Input.Key.RightCtrl) => Keys.RControlKey,
            nameof(Avalonia.Input.Key.LeftShift) => Keys.LShiftKey,
            nameof(Avalonia.Input.Key.RightShift) => Keys.RShiftKey,
            nameof(Avalonia.Input.Key.LeftAlt) => Keys.LMenu,
            nameof(Avalonia.Input.Key.RightAlt) => Keys.RMenu,
            nameof(Avalonia.Input.Key.Enter) => Keys.Enter,
            nameof(Avalonia.Input.Key.CapsLock) => Keys.CapsLock,
            nameof(Avalonia.Input.Key.PageUp) => Keys.PageUp,
            nameof(Avalonia.Input.Key.PageDown) => Keys.PageDown,
            nameof(Avalonia.Input.Key.PrintScreen) => Keys.PrintScreen,
            _ when Enum.TryParse(gesture.KeyName, true, out Keys parsed) => parsed,
            _ => Keys.None
        };

        if (gesture.Control)
        {
            key |= Keys.Control;
        }
        if (gesture.Shift)
        {
            key |= Keys.Shift;
        }
        if (gesture.Alt)
        {
            key |= Keys.Alt;
        }

        return key;
    }

    private void OnHotkeysToggled(bool hotkeysDisabled)
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void InvokeOnMainThread(Action action)
    {
        if (_mainForm.IsDisposed)
        {
            return;
        }

        if (_mainForm.InvokeRequired)
        {
            _mainForm.Invoke(action);
        }
        else
        {
            action();
        }
    }

    private T InvokeOnMainThread<T>(Func<T> action)
    {
        if (_mainForm.InvokeRequired)
        {
            return (T)_mainForm.Invoke(action);
        }

        return action();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        InvokeOnMainThread(() =>
        {
            _manager.IgnoreHotkeys = false;
            _manager.HotkeysToggledTrigger -= OnHotkeysToggled;
            _closed();
        });
    }

    private sealed record HotkeyItemSnapshot(
        int TaskValue,
        string HotkeyText,
        HotkeyRegistrationState State,
        bool IsCustomized);
}
