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

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShareX.AvaloniaUI.Theming;

namespace ShareX.Tools;

public partial class BorderlessWindowWindow : Window
{
    private readonly BorderlessWindowViewModel _viewModel;

    public BorderlessWindowWindow() : this(new BorderlessWindowSettings(), (_, _) => false)
    {
    }

    public BorderlessWindowWindow(
        BorderlessWindowSettings options,
        Func<string, bool, bool> toggleWindow,
        Action<BorderlessWindowSettings>? settingsChanged = null,
        Action? playNotificationSound = null)
    {
        _viewModel = new BorderlessWindowViewModel(options, toggleWindow, settingsChanged, playNotificationSound);
        DataContext = _viewModel;
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _viewModel.CloseRequested = Close;
        Opened += OnOpened;
        Closed += (_, _) => _viewModel.Dispose();
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        _viewModel.SetIgnoredWindowHandle(TryGetPlatformHandle()?.Handle ?? IntPtr.Zero);
    }

    private void OnWindowListDropDownOpened(object? sender, EventArgs e)
    {
        _viewModel.ReloadWindowList();
    }
}
