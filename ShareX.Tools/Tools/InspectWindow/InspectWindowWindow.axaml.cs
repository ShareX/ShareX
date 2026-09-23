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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class InspectWindowWindow : Window
{
    private readonly InspectWindowViewModel _viewModel = new();
    private readonly List<InspectWindowPickerOverlay> _pickerOverlays = [];

    public InspectWindowWindow()
    {
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        Opened += OnOpened;
        Closed += (_, _) =>
        {
            ClosePickerOverlays();
            _viewModel.Dispose();
        };
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        IntPtr handle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        _viewModel.SetIgnoredWindowHandle(handle);
    }

    private void OnPickWindowClick(object? sender, RoutedEventArgs e) => StartPicking(true);
    private void OnPickControlClick(object? sender, RoutedEventArgs e) => StartPicking(false);

    private void OnWindowListDropDownOpened(object? sender, EventArgs e)
    {
        _viewModel.ReloadWindowList();
    }

    private void StartPicking(bool selectTopLevelWindow)
    {
        ClosePickerOverlays();
        Hide();

        foreach (Screen screen in Screens.All)
        {
            InspectWindowPickerOverlay overlay = new(screen, selectTopLevelWindow);
            overlay.TargetPicked += (_, point) => CompletePicking(point, selectTopLevelWindow);
            overlay.PickingCanceled += (_, _) => CancelPicking();
            _pickerOverlays.Add(overlay);
            overlay.Show();
        }
    }

    private void CompletePicking(PixelPoint point, bool selectTopLevelWindow)
    {
        foreach (InspectWindowPickerOverlay overlay in _pickerOverlays)
        {
            overlay.Hide();
        }

        IntPtr handle = InspectWindowService.GetWindowAtPoint(point.X, point.Y, selectTopLevelWindow);
        ClosePickerOverlays();
        Show();
        Activate();
        _viewModel.SelectWindow(handle, selectTopLevelWindow);
    }

    private void CancelPicking()
    {
        ClosePickerOverlays();
        Show();
        Activate();
    }

    private void ClosePickerOverlays()
    {
        foreach (InspectWindowPickerOverlay overlay in _pickerOverlays)
        {
            overlay.Close();
        }
        _pickerOverlays.Clear();
    }

    private async void OnCopyClick(object? sender, RoutedEventArgs e)
    {
        IClipboard? clipboard = Clipboard;
        if (clipboard != null && _viewModel.HasSelection)
        {
            await clipboard.SetTextAsync(_viewModel.ClipboardText);
        }
    }
}
