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
using ShareX.HelpersLib;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ShareX.Tools;

public sealed record InspectWindowProperty(string Name, string Value, double Height = 40, bool IsMultiline = false);

public sealed partial class InspectWindowViewModel : ViewModelBase, IDisposable
{
    private WindowInfo? _selectedWindow;
    private bool _updating;
    private IntPtr _ignoredWindowHandle;

    public ObservableCollection<InspectWindowListItem> Windows { get; } = [];

    [ObservableProperty]
    private InspectWindowListItem? _selectedListItem;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRefresh))]
    private bool _hasSelection;

    [ObservableProperty]
    private bool _isTopLevelWindow;

    [ObservableProperty]
    private string _selectedTitle = "No target selected";

    [ObservableProperty]
    private string _selectedSubtitle = "Pick a window or control to inspect its properties";

    [ObservableProperty]
    private string _selectedType = string.Empty;

    [ObservableProperty]
    private IReadOnlyList<InspectWindowProperty> _details = [];

    [ObservableProperty]
    private bool _isTopMost;

    [ObservableProperty]
    private double _opacity = 100;

    public bool CanRefresh => HasSelection;
    public string ClipboardText => string.Join(Environment.NewLine + Environment.NewLine,
        Details.Select(x => $"{x.Name}{Environment.NewLine}{x.Value}"));

    public void SetIgnoredWindowHandle(IntPtr handle)
    {
        _ignoredWindowHandle = handle;
        RefreshWindowList();
    }

    public void SelectWindow(IntPtr handle, bool isTopLevelWindow)
    {
        if (handle == IntPtr.Zero || handle == _ignoredWindowHandle)
        {
            return;
        }

        _selectedWindow = new WindowInfo(handle);
        IsTopLevelWindow = isTopLevelWindow;
        SelectedListItem = null;
        UpdateWindowInfo();
    }

    partial void OnSelectedListItemChanged(InspectWindowListItem? value)
    {
        if (!_updating && value != null)
        {
            SelectWindow(value.Handle, true);
        }
    }

    [RelayCommand]
    private void RefreshWindowList()
    {
        _updating = true;
        try
        {
            DisposeWindowList();
            Windows.Clear();
            foreach (InspectWindowListItem window in InspectWindowService.GetVisibleWindows(_ignoredWindowHandle))
            {
                Windows.Add(window);
            }
            SelectedListItem = null;
        }
        finally
        {
            _updating = false;
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        if (_selectedWindow != null)
        {
            _selectedWindow = new WindowInfo(_selectedWindow.Handle);
            UpdateWindowInfo();
        }
    }

    partial void OnIsTopMostChanged(bool value)
    {
        if (_updating || !IsTopLevelWindow || _selectedWindow == null)
        {
            return;
        }

        try
        {
            new WindowInfo(_selectedWindow.Handle).TopMost = value;
            Refresh();
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(InspectWindowViewModel), "Failed to change window topmost state.", ex);
        }
    }

    partial void OnOpacityChanged(double value)
    {
        if (_updating || !IsTopLevelWindow || _selectedWindow == null)
        {
            return;
        }

        try
        {
            double percentage = Math.Clamp(value, 10, 100);
            new WindowInfo(_selectedWindow.Handle).Opacity = (byte)Math.Round(percentage / 100d * 255d);
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(InspectWindowViewModel), "Failed to change window opacity.", ex);
        }
    }

    private void UpdateWindowInfo()
    {
        if (_selectedWindow == null)
        {
            ClearSelection();
            return;
        }

        _updating = true;
        try
        {
            IntPtr handle = _selectedWindow.Handle;
            string title = TryGet(() => _selectedWindow.Text);
            string className = TryGet(() => _selectedWindow.ClassName);
            string processName = TryGet(() => _selectedWindow.ProcessName);
            string processFileName = TryGet(() => _selectedWindow.ProcessFileName);
            string processId = TryGet(() => _selectedWindow.ProcessId.ToString());
            Rectangle windowRectangle = TryGet(() => _selectedWindow.Rectangle, Rectangle.Empty);
            Rectangle clientRectangle = TryGet(() => _selectedWindow.ClientRectangle, Rectangle.Empty);
            string styles = TryGet(() => _selectedWindow.Style.ToString().Replace(", ", Environment.NewLine));
            string extendedStyles = TryGet(() => _selectedWindow.ExStyle.ToString().Replace(", ", Environment.NewLine));

            SelectedTitle = string.IsNullOrWhiteSpace(title) ? "Untitled window" : title;
            SelectedSubtitle = string.IsNullOrWhiteSpace(processName)
                ? className
                : string.IsNullOrWhiteSpace(className) ? processName : $"{processName}  |  {className}";
            SelectedType = IsTopLevelWindow ? "Window" : "Control";
            Details =
            [
                new("Window handle", $"0x{handle.ToInt64():X8}"),
                new("Window title", title),
                new("Class name", className),
                new("Process name", processName),
                new("Process file name", processFileName),
                new("Process identifier", processId),
                new("Window rectangle", FormatRectangle(windowRectangle)),
                new("Client rectangle", FormatRectangle(clientRectangle)),
                new("Window styles", styles, 96, true),
                new("Extended window styles", extendedStyles, 96, true)
            ];

            if (IsTopLevelWindow)
            {
                IsTopMost = TryGet(() => _selectedWindow.TopMost, false);
                byte opacity = TryGet(() => _selectedWindow.Opacity, (byte)255);
                Opacity = Math.Round(opacity / 255d * 100d);
            }

            HasSelection = true;
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(InspectWindowViewModel), "Failed to update inspected window information.", ex);
            ClearSelection();
        }
        finally
        {
            _updating = false;
        }
    }

    private void ClearSelection()
    {
        _selectedWindow = null;
        HasSelection = false;
        IsTopLevelWindow = false;
        SelectedTitle = "No target selected";
        SelectedSubtitle = "Pick a window or control to inspect its properties";
        SelectedType = string.Empty;
        Details = [];
        IsTopMost = false;
        Opacity = 100;
    }

    private static string FormatRectangle(Rectangle rectangle)
    {
        return rectangle.IsEmpty
            ? string.Empty
            : $"X: {rectangle.X}  Y: {rectangle.Y}  |  Width: {rectangle.Width}  Height: {rectangle.Height}";
    }

    private static string TryGet(Func<string> valueFactory)
    {
        try
        {
            return valueFactory() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static T TryGet<T>(Func<T> valueFactory, T fallback)
    {
        try
        {
            return valueFactory();
        }
        catch
        {
            return fallback;
        }
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
