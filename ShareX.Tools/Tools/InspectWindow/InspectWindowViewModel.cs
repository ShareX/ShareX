#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShareX.HelpersLib;
using ShareX.Platform;
using System.Collections.ObjectModel;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace ShareX.Tools;

public sealed record InspectWindowProperty(string Name, string Value, bool IsMultiline = false);

public sealed partial class InspectWindowViewModel : ViewModelBase, IDisposable
{
    private WindowDetails? _selectedWindow;
    private long _selectedWindowHandle;
    private bool _updating;
    private IntPtr _ignoredWindowHandle;

    public ObservableCollection<InspectWindowListItem> Windows { get; } = [];

    [ObservableProperty]
    private InspectWindowListItem? _selectedListItem;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRefresh))]
    private bool _hasSelection;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanChangeTopMost))]
    [NotifyPropertyChangedFor(nameof(CanChangeOpacity))]
    [NotifyPropertyChangedFor(nameof(TopMostSupportReason))]
    [NotifyPropertyChangedFor(nameof(OpacitySupportReason))]
    private bool _isTopLevelWindow;

    [ObservableProperty]
    private string _selectedTitle = Localization.Strings.InspectWindowViewModel_No_target_selected;

    [ObservableProperty]
    private string _selectedSubtitle = Localization.Strings.InspectWindowViewModel_Pick_target;

    [ObservableProperty]
    private string _selectedType = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedIcon))]
    private AvaloniaBitmap? _selectedIcon;

    [ObservableProperty]
    private IReadOnlyList<InspectWindowProperty> _details = [];

    [ObservableProperty]
    private bool _isTopMost;

    [ObservableProperty]
    private double _opacity = 100;

    public bool CanRefresh => HasSelection && CanPickWindow;
    public bool HasSelectedIcon => SelectedIcon != null;
    public bool CanPickWindow => GetSupport(WindowManagementFeature.Inspect).IsSupported;
    public bool CanPickControl => CanPickWindow && GetSupport(WindowManagementFeature.ChildControls).IsSupported;
    public string? PickWindowSupportReason => GetReason(GetSupport(WindowManagementFeature.Inspect));
    public string? PickControlSupportReason => CanPickWindow ? GetReason(GetSupport(WindowManagementFeature.ChildControls)) : PickWindowSupportReason;
    public bool CanChangeTopMost => IsTopLevelWindow && _selectedWindow?.IsTopMost != null && GetSupport(WindowManagementFeature.TopMost).IsSupported;
    public bool CanChangeOpacity => IsTopLevelWindow && _selectedWindow?.Opacity != null && GetSupport(WindowManagementFeature.Opacity).IsSupported;
    public string? TopMostSupportReason => GetSettingReason(WindowManagementFeature.TopMost, _selectedWindow?.IsTopMost != null);
    public string? OpacitySupportReason => GetSettingReason(WindowManagementFeature.Opacity, _selectedWindow?.Opacity != null);
    public string ClipboardText => string.Join(Environment.NewLine + Environment.NewLine,
        Details.Select(x => $"{x.Name}{Environment.NewLine}{x.Value}"));

    public void SetIgnoredWindowHandle(IntPtr handle)
    {
        _ignoredWindowHandle = handle;
        ReloadWindowList();
    }

    public void SelectWindow(IntPtr handle, bool isTopLevelWindow)
    {
        if (handle == IntPtr.Zero || handle == _ignoredWindowHandle || !(isTopLevelWindow ? CanPickWindow : CanPickControl))
        {
            return;
        }

        _selectedWindowHandle = handle.ToInt64();
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

    public void ReloadWindowList()
    {
        _updating = true;
        try
        {
            DisposeWindowList();
            Windows.Clear();
            RefreshCapabilities();
            if (!CanPickWindow) return;
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
        if (_selectedWindowHandle != 0)
        {
            UpdateWindowInfo();
        }
    }

    partial void OnIsTopMostChanged(bool value)
    {
        if (_updating || !CanChangeTopMost || _selectedWindow == null)
        {
            return;
        }

        try
        {
            PlatformServices.Current.WindowManagement.SetTopMost(_selectedWindow.Handle, value);
            Refresh();
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(InspectWindowViewModel), "Failed to change window topmost state.", ex);
        }
    }

    partial void OnOpacityChanged(double value)
    {
        if (_updating || !CanChangeOpacity || _selectedWindow == null)
        {
            return;
        }

        try
        {
            double percentage = Math.Clamp(value, 10, 100);
            if (!PlatformServices.Current.WindowManagement.SetOpacity(_selectedWindow.Handle, (byte)Math.Round(percentage / 100d * 255d)))
            {
                Refresh();
            }
        }
        catch (Exception ex)
        {
            ToolsDiagnostics.ReportWarning(nameof(InspectWindowViewModel), "Failed to change window opacity.", ex);
        }
    }

    private void UpdateWindowInfo()
    {
        if (_selectedWindowHandle == 0)
        {
            ClearSelection();
            return;
        }

        _updating = true;
        try
        {
            IWindowManagementService service = PlatformServices.Current.WindowManagement;
            _selectedWindow = service.GetSupport(WindowManagementFeature.Inspect).IsSupported ? service.GetDetails(_selectedWindowHandle) : null;
            if (_selectedWindow == null)
            {
                ClearSelection();
                return;
            }

            long handle = _selectedWindow.Handle;
            string title = _selectedWindow.Title;
            string className = _selectedWindow.ClassName ?? string.Empty;
            string processName = _selectedWindow.ProcessName ?? string.Empty;
            string processFileName = _selectedWindow.ProcessPath ?? string.Empty;
            string processId = _selectedWindow.ProcessId?.ToString() ?? string.Empty;
            PlatformRectangle windowRectangle = _selectedWindow.Bounds;
            PlatformRectangle clientRectangle = _selectedWindow.ClientBounds ?? default;
            string styles = string.Join(Environment.NewLine, _selectedWindow.Styles);
            string extendedStyles = string.Join(Environment.NewLine, _selectedWindow.ExtendedStyles);

            SelectedTitle = string.IsNullOrWhiteSpace(title) ? Localization.Strings.InspectWindowViewModel_Untitled_window : title;
            SelectedSubtitle = string.IsNullOrWhiteSpace(processName)
                ? className
                : string.IsNullOrWhiteSpace(className) ? processName : $"{processName}  |  {className}";
            SelectedType = IsTopLevelWindow ? Localization.Strings.InspectWindowViewModel_Window : Localization.Strings.InspectWindowViewModel_Control;
            ReplaceSelectedIcon(InspectWindowService.GetWindowIcon(new IntPtr(handle)));
            Details =
            [
                new(Localization.Strings.InspectWindowViewModel_Window_handle, $"0x{handle:X8}"),
                new(Localization.Strings.InspectWindowViewModel_Window_title, title),
                new(Localization.Strings.InspectWindowViewModel_Class_name, className),
                new(Localization.Strings.InspectWindowViewModel_Process_name, processName),
                new(Localization.Strings.InspectWindowViewModel_Process_file_name, processFileName),
                new(Localization.Strings.InspectWindowViewModel_Process_identifier, processId),
                new(Localization.Strings.InspectWindowViewModel_Window_rectangle, FormatRectangle(windowRectangle)),
                new(Localization.Strings.InspectWindowViewModel_Client_rectangle, FormatRectangle(clientRectangle)),
                new(Localization.Strings.InspectWindowViewModel_Window_styles, styles, true),
                new(Localization.Strings.InspectWindowViewModel_Extended_window_styles, extendedStyles, true)
            ];

            if (IsTopLevelWindow)
            {
                IsTopMost = _selectedWindow.IsTopMost ?? false;
                byte opacity = _selectedWindow.Opacity ?? 255;
                Opacity = Math.Round(opacity / 255d * 100d);
            }

            HasSelection = true;
            RefreshCapabilities();
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
        _selectedWindowHandle = 0;
        HasSelection = false;
        IsTopLevelWindow = false;
        SelectedTitle = Localization.Strings.InspectWindowViewModel_No_target_selected;
        SelectedSubtitle = Localization.Strings.InspectWindowViewModel_Pick_target;
        SelectedType = string.Empty;
        ReplaceSelectedIcon(null);
        Details = [];
        IsTopMost = false;
        Opacity = 100;
        RefreshCapabilities();
    }

    private static FeatureSupport GetSupport(WindowManagementFeature feature) => PlatformServices.Current.WindowManagement.GetSupport(feature);

    private static string? GetReason(FeatureSupport support) => support.IsSupported ? null : support.Reason;

    private string? GetSettingReason(WindowManagementFeature feature, bool hasValue)
    {
        FeatureSupport support = GetSupport(feature);
        if (!support.IsSupported) return support.Reason;
        return IsTopLevelWindow && _selectedWindow != null && !hasValue
            ? Localization.Strings.InspectWindowViewModel_Selected_window_setting_unavailable : null;
    }

    private void RefreshCapabilities()
    {
        OnPropertyChanged(nameof(CanRefresh));
        OnPropertyChanged(nameof(CanPickWindow));
        OnPropertyChanged(nameof(CanPickControl));
        OnPropertyChanged(nameof(PickWindowSupportReason));
        OnPropertyChanged(nameof(PickControlSupportReason));
        OnPropertyChanged(nameof(CanChangeTopMost));
        OnPropertyChanged(nameof(CanChangeOpacity));
        OnPropertyChanged(nameof(TopMostSupportReason));
        OnPropertyChanged(nameof(OpacitySupportReason));
    }

    private static string FormatRectangle(PlatformRectangle rectangle)
    {
        return rectangle.IsEmpty
            ? string.Empty
            : string.Format(Localization.Strings.InspectWindowViewModel_Rectangle_format, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    }

    private void DisposeWindowList()
    {
        foreach (InspectWindowListItem window in Windows)
        {
            window.Dispose();
        }
    }

    private void ReplaceSelectedIcon(AvaloniaBitmap? icon)
    {
        SelectedIcon?.Dispose();
        SelectedIcon = icon;
    }

    public void Dispose()
    {
        ReplaceSelectedIcon(null);
        DisposeWindowList();
        Windows.Clear();
    }
}
