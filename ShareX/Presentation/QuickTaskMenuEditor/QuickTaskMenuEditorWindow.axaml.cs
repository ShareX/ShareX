#region License Information (GPL v3)

/*
    ShareX - A program developed by ShareX Team
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ShareX;

public partial class QuickTaskMenuEditorWindow : Window
{
    private readonly ObservableCollection<QuickTaskPresetItem> _items = [];
    private ObservableCollection<QuickTaskFlagItem> _afterCaptureOptions = [];
    private ObservableCollection<QuickTaskFlagItem> _afterUploadOptions = [];
    private QuickTaskPresetItem? _editedItem;

    private QuickTaskPresetItem? SelectedItem => TaskList.SelectedItem as QuickTaskPresetItem;

    public QuickTaskMenuEditorWindow()
    {
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        Program.Settings.QuickTaskPresets ??= [];
        ResetConfirmationText.Text = Properties.Resources.QuickTaskMenuEditorForm_Reset_all_quick_tasks_to_defaults_Confirmation;
        TaskList.ItemsSource = _items;
        ReloadItems();
        UpdateSelectionState();

        KeyDown += OnWindowKeyDown;
        Opened += (_, _) => Activate();
    }

    private void ReloadItems()
    {
        _items.Clear();
        foreach (QuickTaskInfo task in Program.Settings.QuickTaskPresets)
        {
            _items.Add(new QuickTaskPresetItem(task));
        }

        TaskList.SelectedItem = _items.FirstOrDefault();
        UpdateSelectionState();
    }

    private void OnAddClick(object? sender, RoutedEventArgs e) => ShowItemEditor(null);

    private void OnEditClick(object? sender, RoutedEventArgs e)
    {
        if (SelectedItem is { } item)
        {
            ShowItemEditor(item);
        }
    }

    private void OnRemoveClick(object? sender, RoutedEventArgs e)
    {
        if (SelectedItem is not { } item)
        {
            return;
        }

        int index = _items.IndexOf(item);
        Program.Settings.QuickTaskPresets.Remove(item.Model);
        _items.Remove(item);
        TaskList.SelectedItem = _items.Count == 0 ? null : _items[Math.Min(index, _items.Count - 1)];
        SaveSettings();
        UpdateSelectionState();
    }

    private void OnMoveUpClick(object? sender, RoutedEventArgs e) => MoveSelected(-1);

    private void OnMoveDownClick(object? sender, RoutedEventArgs e) => MoveSelected(1);

    private void MoveSelected(int offset)
    {
        if (SelectedItem is not { } item)
        {
            return;
        }

        int oldIndex = _items.IndexOf(item);
        int newIndex = oldIndex + offset;
        if (newIndex < 0 || newIndex >= _items.Count)
        {
            return;
        }

        _items.Move(oldIndex, newIndex);
        QuickTaskInfo model = Program.Settings.QuickTaskPresets[oldIndex];
        Program.Settings.QuickTaskPresets.RemoveAt(oldIndex);
        Program.Settings.QuickTaskPresets.Insert(newIndex, model);
        TaskList.SelectedItem = item;
        SaveSettings();
        UpdateSelectionState();
    }

    private void OnResetClick(object? sender, RoutedEventArgs e) => ResetConfirmationOverlay.IsVisible = true;

    private void OnConfirmResetClick(object? sender, RoutedEventArgs e)
    {
        Program.Settings.QuickTaskPresets = QuickTaskInfo.DefaultPresets;
        ResetConfirmationOverlay.IsVisible = false;
        ReloadItems();
        SaveSettings();
    }

    private void OnCancelResetClick(object? sender, RoutedEventArgs e) => ResetConfirmationOverlay.IsVisible = false;

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();

    private void OnTaskSelectionChanged(object? sender, SelectionChangedEventArgs e) => UpdateSelectionState();

    private void OnTaskListDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (SelectedItem != null)
        {
            ShowItemEditor(SelectedItem);
        }
    }

    private void UpdateSelectionState()
    {
        int index = SelectedItem == null ? -1 : _items.IndexOf(SelectedItem);
        bool selected = index >= 0;
        EditButton.IsEnabled = selected;
        RemoveButton.IsEnabled = selected;
        MoveUpButton.IsEnabled = index > 0;
        MoveDownButton.IsEnabled = selected && index < _items.Count - 1;
    }

    private void ShowItemEditor(QuickTaskPresetItem? item)
    {
        _editedItem = item;
        QuickTaskInfo task = item?.Model ?? new QuickTaskInfo();

        ItemEditorTitle.Text = item == null ? "Add quick task" : "Edit quick task";
        TaskNameBox.Text = task.Name ?? string.Empty;
        _afterCaptureOptions = CreateFlagOptions(task.AfterCaptureTasks);
        _afterUploadOptions = CreateFlagOptions(task.AfterUploadTasks);
        AfterCaptureTaskOptions.ItemsSource = _afterCaptureOptions;
        AfterUploadTaskOptions.ItemsSource = _afterUploadOptions;
        UpdateEditorPreview();
        ItemEditorOverlay.IsVisible = true;

        Dispatcher.UIThread.Post(() =>
        {
            TaskNameBox.Focus();
            TaskNameBox.SelectAll();
        }, DispatcherPriority.Input);
    }

    private ObservableCollection<QuickTaskFlagItem> CreateFlagOptions<T>(T selected) where T : struct, Enum
    {
        long selectedValue = Convert.ToInt64(selected);
        ObservableCollection<QuickTaskFlagItem> items = [];

        foreach (T value in Enum.GetValues<T>().Where(value => Convert.ToInt64(value) != 0))
        {
            long flag = Convert.ToInt64(value);
            QuickTaskFlagItem item = new(flag, value.GetLocalizedDescription(), (selectedValue & flag) == flag);
            item.Changed += UpdateEditorPreview;
            items.Add(item);
        }

        return items;
    }

    private void UpdateEditorPreview()
    {
        AfterCaptureTasks afterCapture = ReadFlags<AfterCaptureTasks>(_afterCaptureOptions);
        AfterUploadTasks afterUpload = ReadFlags<AfterUploadTasks>(_afterUploadOptions);
        string generatedName = new QuickTaskInfo(afterCapture, afterUpload).ToString();
        TaskNameBox.PlaceholderText = string.IsNullOrEmpty(generatedName) ? "Separator" : generatedName;
    }

    private static T ReadFlags<T>(IEnumerable<QuickTaskFlagItem> items) where T : struct, Enum
    {
        long value = items.Where(item => item.IsChecked).Aggregate(0L, (current, item) => current | item.FlagValue);
        return (T)Enum.ToObject(typeof(T), value);
    }

    private void OnSaveItemClick(object? sender, RoutedEventArgs e)
    {
        QuickTaskInfo task;
        QuickTaskPresetItem item;

        if (_editedItem == null)
        {
            task = new QuickTaskInfo();
            item = new QuickTaskPresetItem(task);
            Program.Settings.QuickTaskPresets.Add(task);
            _items.Add(item);
        }
        else
        {
            item = _editedItem;
            task = item.Model;
        }

        task.Name = TaskNameBox.Text ?? string.Empty;
        task.AfterCaptureTasks = ReadFlags<AfterCaptureTasks>(_afterCaptureOptions);
        task.AfterUploadTasks = ReadFlags<AfterUploadTasks>(_afterUploadOptions);
        item.Refresh();
        TaskList.SelectedItem = item;

        HideItemEditor();
        SaveSettings();
        UpdateSelectionState();
    }

    private void OnCancelItemClick(object? sender, RoutedEventArgs e) => HideItemEditor();

    private void HideItemEditor()
    {
        ItemEditorOverlay.IsVisible = false;
        _editedItem = null;
        _afterCaptureOptions = [];
        _afterUploadOptions = [];
        AfterCaptureTaskOptions.ItemsSource = null;
        AfterUploadTaskOptions.ItemsSource = null;
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            if (ResetConfirmationOverlay.IsVisible)
            {
                ResetConfirmationOverlay.IsVisible = false;
            }
            else if (ItemEditorOverlay.IsVisible)
            {
                HideItemEditor();
            }
            else
            {
                Close();
            }

            e.Handled = true;
        }
        else if (e.Key == Key.Delete && !ItemEditorOverlay.IsVisible && !ResetConfirmationOverlay.IsVisible)
        {
            OnRemoveClick(sender, e);
            e.Handled = true;
        }
    }

    private static void SaveSettings() => SettingManager.SaveApplicationConfigAsync();
}

public sealed class QuickTaskPresetItem : INotifyPropertyChanged
{
    public QuickTaskInfo Model { get; }
    public bool IsSeparator => !Model.IsValid;
    public string Title => IsSeparator ? "Separator" : Model.ToString();

    public string Summary
    {
        get
        {
            if (IsSeparator)
            {
                return string.Empty;
            }

            string capture = string.Join(", ", Model.AfterCaptureTasks.GetFlags().Select(value => value.GetLocalizedDescription()));
            string upload = string.Join(", ", Model.AfterUploadTasks.GetFlags().Select(value => value.GetLocalizedDescription()));
            return string.IsNullOrEmpty(upload) ? capture : $"{capture}  •  After upload: {upload}";
        }
    }

    public QuickTaskPresetItem(QuickTaskInfo model)
    {
        Model = model;
    }

    public void Refresh()
    {
        OnPropertyChanged(nameof(IsSeparator));
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Summary));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class QuickTaskFlagItem : INotifyPropertyChanged
{
    private bool _isChecked;

    public long FlagValue { get; }
    public string Name { get; }

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked == value)
            {
                return;
            }

            _isChecked = value;
            OnPropertyChanged();
            Changed?.Invoke();
        }
    }

    public QuickTaskFlagItem(long flagValue, string name, bool isChecked)
    {
        FlagValue = flagValue;
        Name = name;
        _isChecked = isChecked;
    }

    public event Action? Changed;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
