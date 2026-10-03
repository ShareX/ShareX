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

#nullable enable

using ShareX.AvaloniaUI.Integration;
using ShareX.HelpersLib;
using System;
using System.IO;

namespace ShareX;

internal interface ITrayIconService : IDisposable
{
    event Action? RightButtonDown;
    event Action? RightButtonUp;

    bool Visible { get; set; }
    string ToolTipText { get; set; }

    void SetIcon(byte[] iconBytes);
}

/// <summary>Desktop tray adapter; Windows retains its additional mouse actions.</summary>
internal sealed class DesktopTrayIconService : ITrayIconService
{
    private readonly WindowsTrayIcon? _windowsIcon;
    private readonly Avalonia.Controls.TrayIcon? _avaloniaIcon;
    private readonly IDisposable? _avaloniaIconRegistration;
    private readonly Avalonia.Threading.DispatcherTimer _singleClickTimer;
    private int _leftClickCount;
    private bool _disposed;
    public event Action? RightButtonDown;
    public event Action? RightButtonUp;

    public bool Visible
    {
        get => _windowsIcon?.Visible ?? _avaloniaIcon!.IsVisible;
        set { if (_windowsIcon != null) _windowsIcon.Visible = value; else _avaloniaIcon!.IsVisible = value; }
    }
    public string ToolTipText
    {
        get => _windowsIcon?.ToolTipText ?? _avaloniaIcon!.ToolTipText ?? string.Empty;
        set { if (_windowsIcon != null) _windowsIcon.ToolTipText = value; else _avaloniaIcon!.ToolTipText = value; }
    }

    public DesktopTrayIconService(IHotkeyHost host, byte[] icon, string text, bool visible)
    {
        _singleClickTimer = new Avalonia.Threading.DispatcherTimer
        {
            Interval = OperatingSystem.IsWindows() ? WindowsTrayIcon.DoubleClickTime :
                Avalonia.Application.Current?.PlatformSettings?.GetDoubleTapTime(Avalonia.Input.PointerType.Mouse) ?? TimeSpan.FromMilliseconds(500)
        };
        _singleClickTimer.Tick += OnSingleClickTimerTick;
        if (OperatingSystem.IsWindows())
        {
            _windowsIcon = new WindowsTrayIcon(host);
            _windowsIcon.MouseDown += OnMouseDown;
            _windowsIcon.MouseUp += OnMouseUp;
        }
        else
        {
            _avaloniaIcon = new Avalonia.Controls.TrayIcon { Menu = new Avalonia.Controls.NativeMenu(), IsVisible = false };
            _avaloniaIcon.Clicked += OnAvaloniaClick;
            _avaloniaIcon.Menu.Opening += (_, _) => BuildNativeMenu(_avaloniaIcon.Menu, new MainMenuBuilder(true).BuildTrayMenu());
        }
        ToolTipText = text;
        SetIcon(icon);
        if (_avaloniaIcon != null) _avaloniaIconRegistration = DesktopServices.RegisterTrayIcon(_avaloniaIcon);
        Visible = visible;
    }

    public void SetIcon(byte[] bytes)
    {
        if (_windowsIcon != null) _windowsIcon.SetIcon(bytes);
        else
        {
            using MemoryStream stream = new(bytes, writable: false);
            _avaloniaIcon!.Icon = new Avalonia.Controls.WindowIcon(stream);
        }
    }

    private void OnMouseDown(InputMouseButton button)
    {
        if (button == InputMouseButton.Right) RightButtonDown?.Invoke();
    }
    private void OnAvaloniaClick(object? sender, EventArgs e) => OnMouseUp(InputMouseButton.Left);

    private async void OnMouseUp(InputMouseButton button)
    {
        switch (button)
        {
            case InputMouseButton.Left:
                if (ApplicationState.Settings.TrayLeftDoubleClickAction == HotkeyType.None)
                {
                    await TaskHelpers.ExecuteJob(ApplicationState.Settings.TrayLeftClickAction);
                }
                else
                {
                    _leftClickCount++;

                    if (_leftClickCount == 1)
                    {
                        _singleClickTimer.Start();
                    }
                    else
                    {
                        _leftClickCount = 0;
                        _singleClickTimer.Stop();
                        await TaskHelpers.ExecuteJob(ApplicationState.Settings.TrayLeftDoubleClickAction);
                    }
                }
                break;
            case InputMouseButton.Middle:
                await TaskHelpers.ExecuteJob(ApplicationState.Settings.TrayMiddleClickAction);
                break;
            case InputMouseButton.Right:
                RightButtonUp?.Invoke();
                break;
        }
    }

    private async void OnSingleClickTimerTick(object? sender, EventArgs e)
    {
        _singleClickTimer.Stop();

        if (_leftClickCount == 1)
        {
            _leftClickCount = 0;
            await TaskHelpers.ExecuteJob(ApplicationState.Settings.TrayLeftClickAction);
        }
    }

    private static void BuildNativeMenu(Avalonia.Controls.NativeMenu menu, System.Collections.Generic.IReadOnlyList<MainMenuEntry> entries)
    {
        menu.Items.Clear();
        foreach (MainMenuEntry entry in entries)
        {
            if (!entry.IsVisible) continue;
            if (entry.IsSeparator) { menu.Items.Add(new Avalonia.Controls.NativeMenuItemSeparator()); continue; }
            // Native tray menus have no tooltip surface, so include the unavailable reason in their label.
            string header = !entry.IsEnabled && !string.IsNullOrEmpty(entry.ToolTip) ? $"{entry.Header} ({entry.ToolTip})" : entry.Header;
            Avalonia.Controls.NativeMenuItem item = new(header) { IsEnabled = entry.IsEnabled, IsChecked = entry.IsChecked };
            if (entry.CreateChildren != null)
            {
                item.Menu = new Avalonia.Controls.NativeMenu();
                item.Menu.Opening += (_, _) => BuildNativeMenu(item.Menu, entry.CreateChildren());
            }
            else if (entry.CreateCategories != null)
            {
                item.Menu = new Avalonia.Controls.NativeMenu();
                item.Menu.Opening += (_, _) =>
                {
                    item.Menu.Items.Clear();
                    foreach (MainMenuCategory category in entry.CreateCategories())
                    {
                        Avalonia.Controls.NativeMenuItem categoryItem = new(category.Header) { Menu = new Avalonia.Controls.NativeMenu() };
                        BuildNativeMenu(categoryItem.Menu, category.Entries);
                        item.Menu.Items.Add(categoryItem);
                    }
                };
            }
            else item.Click += async (_, _) => { if (entry.ExecuteAsync != null) await entry.ExecuteAsync(); };
            menu.Items.Add(item);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _singleClickTimer.Stop();
        _singleClickTimer.Tick -= OnSingleClickTimerTick;
        if (_windowsIcon != null)
        {
            _windowsIcon.MouseDown -= OnMouseDown;
            _windowsIcon.MouseUp -= OnMouseUp;
            _windowsIcon.Dispose();
        }
        if (_avaloniaIcon != null)
        {
            _avaloniaIcon.Clicked -= OnAvaloniaClick;
            _avaloniaIconRegistration?.Dispose();
        }
    }
}
