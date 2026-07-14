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

using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShareX;

public sealed class HotkeySettingsViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly IHotkeySettingsService _service;
    private HotkeySettingsItem? _selectedItem;
    private HotkeySettingsItem? _capturingItem;
    private bool _areHotkeysDisabled;
    private bool _isResetConfirmationVisible;
    private bool _disposed;

    public ObservableCollection<HotkeySettingsItem> Items { get; } = [];

    public HotkeySettingsItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (_selectedItem == value)
            {
                return;
            }

            if (_selectedItem != null)
            {
                _selectedItem.IsSelected = false;
            }

            _selectedItem = value;

            if (_selectedItem != null)
            {
                _selectedItem.IsSelected = true;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(HasSelection));
            OnPropertyChanged(nameof(CanMove));
        }
    }

    public bool HasSelection => SelectedItem != null;
    public bool CanMove => SelectedItem != null && Items.Count > 1;
    public bool IsEmpty => Items.Count == 0;

    public bool AreHotkeysDisabled
    {
        get => _areHotkeysDisabled;
        private set => SetField(ref _areHotkeysDisabled, value);
    }

    public bool IsResetConfirmationVisible
    {
        get => _isResetConfirmationVisible;
        set => SetField(ref _isResetConfirmationVisible, value);
    }

    public HotkeySettingsViewModel(IHotkeySettingsService service)
    {
        _service = service;
        _service.StateChanged += OnServiceStateChanged;
        Reload();
    }

    public void Add()
    {
        HotkeySettingsItem item = _service.Add();
        Items.Add(item);
        SelectedItem = item;
        NotifyCollectionStateChanged();
    }

    public void RemoveSelected()
    {
        if (SelectedItem == null)
        {
            return;
        }

        int index = Items.IndexOf(SelectedItem);
        StopCapture();
        _service.Remove(SelectedItem);
        Items.Remove(SelectedItem);
        SelectedItem = Items.Count == 0 ? null : Items[Math.Min(index, Items.Count - 1)];
        NotifyCollectionStateChanged();
    }

    public void DuplicateSelected()
    {
        if (SelectedItem == null)
        {
            return;
        }

        HotkeySettingsItem item = _service.Duplicate(SelectedItem);
        Items.Add(item);
        SelectedItem = item;
        NotifyCollectionStateChanged();
    }

    public void MoveSelected(int offset)
    {
        if (SelectedItem == null || Items.Count < 2)
        {
            return;
        }

        int oldIndex = Items.IndexOf(SelectedItem);
        int newIndex = (oldIndex + offset + Items.Count) % Items.Count;
        _service.Move(SelectedItem, offset);
        Items.Move(oldIndex, newIndex);
    }

    public void EditSelectedTask()
    {
        if (SelectedItem != null)
        {
            EditTask(SelectedItem);
        }
    }

    public void EditTask(HotkeySettingsItem item)
    {
        SelectedItem = item;
        _service.EditTask(item);
        _service.Refresh(item);
    }

    public void ChangeTask(HotkeySettingsItem item, HotkeyTaskOption option)
    {
        if (item.SelectedTask?.Value == option.Value)
        {
            return;
        }

        SelectedItem = item;
        _service.SetTask(item, option.Value);
        _service.Refresh(item);
    }

    public void ToggleCapture(HotkeySettingsItem item)
    {
        SelectedItem = item;

        if (_capturingItem == item)
        {
            StopCapture();
            return;
        }

        StopCapture();
        _capturingItem = item;
        item.IsCapturing = true;
        _service.SetCaptureMode(true);
        _service.SetHotkey(item, new HotkeyGesture(string.Empty, false, false, false, false), false);
        RefreshAll();
    }

    public void ApplyGesture(HotkeySettingsItem item, HotkeyGesture gesture)
    {
        if (_capturingItem != item)
        {
            return;
        }

        bool complete = _service.SetHotkey(item, gesture, false);
        RefreshAll();

        if (complete)
        {
            StopCapture();
        }
    }

    public void ClearAndStopCapture()
    {
        if (_capturingItem == null)
        {
            return;
        }

        _service.SetHotkey(_capturingItem, new HotkeyGesture(string.Empty, false, false, false, false), true);
        StopCapture();
    }

    public void StopCapture()
    {
        if (_capturingItem == null)
        {
            return;
        }

        HotkeySettingsItem item = _capturingItem;
        _capturingItem = null;
        _service.FinalizeHotkey(item);
        item.IsCapturing = false;
        _service.SetCaptureMode(false);
        RefreshAll();
    }

    public void EnableHotkeys()
    {
        _service.EnableHotkeys();
        RefreshAll();
    }

    public void ConfirmReset()
    {
        StopCapture();
        _service.Reset();
        IsResetConfirmationVisible = false;
        Reload();
    }

    private void Reload()
    {
        Items.Clear();

        foreach (HotkeySettingsItem item in _service.GetItems())
        {
            Items.Add(item);
        }

        SelectedItem = null;
        AreHotkeysDisabled = _service.AreHotkeysDisabled;
        NotifyCollectionStateChanged();
    }

    private void RefreshAll()
    {
        foreach (HotkeySettingsItem item in Items)
        {
            _service.Refresh(item);
        }

        AreHotkeysDisabled = _service.AreHotkeysDisabled;
    }

    private void OnServiceStateChanged(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(RefreshAll);
    }

    private void NotifyCollectionStateChanged()
    {
        OnPropertyChanged(nameof(CanMove));
        OnPropertyChanged(nameof(IsEmpty));
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        StopCapture();
        _service.StateChanged -= OnServiceStateChanged;
        _service.Dispose();
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
