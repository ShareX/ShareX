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
using ShareX.Platform;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace ShareX;

internal interface ITrayIconService : IDisposable
{
    event Action? RightButtonDown;
    event Action? RightButtonUp;
    event Action? CloseRequested;

    bool Visible { get; set; }
    string ToolTipText { get; set; }

    void SetIcon(byte[] iconBytes);
}

/// <summary>Desktop tray adapter; Windows retains its additional mouse actions.</summary>
internal sealed class DesktopTrayIconService : ITrayIconService
{
    private readonly ITraySession? _nativeIcon;
    private readonly Avalonia.Controls.TrayIcon? _avaloniaIcon;
    private readonly IDisposable? _avaloniaIconRegistration;
    private readonly Avalonia.Threading.DispatcherTimer _singleClickTimer;
    private Avalonia.Threading.DispatcherTimer? _menuRefreshTimer;
    private string? _menuSignature;
    private int _leftClickCount;
    private bool _disposed;
    public event Action? RightButtonDown;
    public event Action? RightButtonUp;
    public event Action? CloseRequested;

    public bool Visible
    {
        get => !_disposed && (_nativeIcon?.Visible ?? _avaloniaIcon!.IsVisible);
        set
        {
            // MainWindow clears visibility during its later closing callback, after host disposal.
            if (_disposed) return;
            if (_nativeIcon != null) _nativeIcon.Visible = value; else _avaloniaIcon!.IsVisible = value;
        }
    }
    public string ToolTipText
    {
        get => _nativeIcon?.ToolTipText ?? _avaloniaIcon!.ToolTipText ?? string.Empty;
        set
        {
            if (_disposed) return;
            if (_nativeIcon != null) _nativeIcon.ToolTipText = value; else _avaloniaIcon!.ToolTipText = value;
        }
    }

    public DesktopTrayIconService(byte[] icon, string text, bool visible)
    {
        ITrayService tray = PlatformServices.Current.Tray;
        _nativeIcon = tray.Support.IsSupported
            ? tray.CreateSession(exception => Avalonia.Threading.Dispatcher.UIThread.Post(ExceptionDispatchInfo.Capture(exception).Throw))
            : null;
        _singleClickTimer = new Avalonia.Threading.DispatcherTimer
        {
            Interval = _nativeIcon?.DoubleClickTime ??
                Avalonia.Application.Current?.PlatformSettings?.GetDoubleTapTime(Avalonia.Input.PointerType.Mouse) ?? TimeSpan.FromMilliseconds(500)
        };
        _singleClickTimer.Tick += OnSingleClickTimerTick;
        if (_nativeIcon != null)
        {
            _nativeIcon.MouseDown += OnMouseDown;
            _nativeIcon.MouseUp += OnMouseUp;
            _nativeIcon.CloseRequested += OnCloseRequested;
        }
        else
        {
            _avaloniaIcon = new Avalonia.Controls.TrayIcon { Menu = new Avalonia.Controls.NativeMenu(), IsVisible = false };
            _avaloniaIcon.Clicked += OnAvaloniaClick;
            // Linux panels read the menu over D-Bus (StatusNotifierItem and dbusmenu) without raising NativeMenu.Opening, so the
            // menu is built ahead of time and refreshed while ShareX runs; it was empty and right click showed nothing.
            RefreshNativeMenu();
            _menuRefreshTimer = new Avalonia.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _menuRefreshTimer.Tick += (_, _) => RefreshNativeMenu();
            _menuRefreshTimer.Start();
        }
        try
        {
            ToolTipText = text;
            SetIcon(icon);
            if (_avaloniaIcon != null) _avaloniaIconRegistration = DesktopServices.RegisterTrayIcon(_avaloniaIcon);
            Visible = visible;
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public void SetIcon(byte[] bytes)
    {
        if (_disposed) return;
        if (_nativeIcon != null)
        {
            // Keep the existing Skia decode/PNG normalization, with native resource ownership in the platform.
            using SKBitmap bitmap = SkiaImageHelpers.ByteArrayToBitmap(bytes);
            using MemoryStream stream = new();
            bitmap.Save(stream, SKEncodedImageFormat.Png);
            _nativeIcon.SetIcon(stream.ToArray());
        }
        else
        {
            using MemoryStream stream = new(bytes, writable: false);
            _avaloniaIcon!.Icon = new Avalonia.Controls.WindowIcon(stream);
        }
    }

    private void OnCloseRequested() => CloseRequested?.Invoke();

    private void OnMouseDown(TrayMouseButton button)
    {
        if (button == TrayMouseButton.Right) RightButtonDown?.Invoke();
    }
    private void OnAvaloniaClick(object? sender, EventArgs e) => OnMouseUp(TrayMouseButton.Left);

    private async void OnMouseUp(TrayMouseButton button)
    {
        switch (button)
        {
            case TrayMouseButton.Left:
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
            case TrayMouseButton.Middle:
                if (PlatformServices.Current.Tray.MiddleClickSupport.IsSupported)
                {
                    await TaskHelpers.ExecuteJob(ApplicationState.Settings.TrayMiddleClickAction);
                }
                break;
            case TrayMouseButton.Right:
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

    /// <summary>A tray menu item with its submenu already expanded.</summary>
    private sealed record TrayMenuNode(string Header, bool IsSeparator, bool IsEnabled, bool IsChecked, MainMenuToggleType ToggleType,
        Func<System.Threading.Tasks.Task>? ExecuteAsync, System.Collections.Generic.IReadOnlyList<TrayMenuNode>? Children);

    /// <summary>Keeps the main window reachable and rebuilds changed menus without replacing an unchanged open menu.</summary>
    private void RefreshNativeMenu()
    {
        if (_disposed || _avaloniaIcon?.Menu == null)
        {
            return;
        }

        try
        {
            if (!ApplicationLifecycle.IsClosing && MainWindowIntegration.IsInitialized &&
                !MainWindowIntegration.IsVisible && !ApplicationState.IsTrayUsable)
            {
                MainWindowIntegration.Activate();
            }

            System.Collections.Generic.IReadOnlyList<TrayMenuNode> nodes = Expand(new MainMenuBuilder(true).BuildTrayMenu());
            string signature = Describe(nodes);

            if (signature == _menuSignature)
            {
                return;
            }

            _menuSignature = signature;
            Fill(_avaloniaIcon.Menu, nodes);
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e, "Tray menu refresh failed.");
        }
    }

    private static System.Collections.Generic.IReadOnlyList<TrayMenuNode> Expand(System.Collections.Generic.IReadOnlyList<MainMenuEntry> entries)
    {
        System.Collections.Generic.List<TrayMenuNode> nodes = new();

        foreach (MainMenuEntry entry in entries)
        {
            if (!entry.IsVisible) continue;
            if (entry.IsSeparator)
            {
                nodes.Add(new TrayMenuNode("", true, true, false, MainMenuToggleType.None, null, null));
                continue;
            }

            // Native tray menus have no tooltip surface, so include the unavailable reason in their label.
            string header = !entry.IsEnabled && !string.IsNullOrEmpty(entry.ToolTip) ? $"{entry.Header} ({entry.ToolTip})" : entry.Header;
            System.Collections.Generic.IReadOnlyList<TrayMenuNode>? children = null;

            if (entry.CreateChildren != null)
            {
                children = Expand(entry.CreateChildren());
            }
            else if (entry.CreateCategories != null)
            {
                children = entry.CreateCategories()
                    .Select(category => new TrayMenuNode(category.Header, false, true, false, MainMenuToggleType.None, null, Expand(category.Entries)))
                    .ToList();
            }

            nodes.Add(new TrayMenuNode(header, false, entry.IsEnabled, entry.IsChecked, entry.ToggleType, entry.ExecuteAsync, children));
        }

        return nodes;
    }

    private static string Describe(System.Collections.Generic.IReadOnlyList<TrayMenuNode> nodes) => string.Join("|", nodes.Select(node =>
        node.IsSeparator ? "-" : $"{node.Header}:{node.IsEnabled}:{node.IsChecked}:{node.ToggleType}" + (node.Children != null ? "[" + Describe(node.Children) + "]" : "")));

    private static void Fill(Avalonia.Controls.NativeMenu menu, System.Collections.Generic.IReadOnlyList<TrayMenuNode> nodes)
    {
        menu.Items.Clear();

        foreach (TrayMenuNode node in nodes)
        {
            if (node.IsSeparator)
            {
                menu.Items.Add(new Avalonia.Controls.NativeMenuItemSeparator());
                continue;
            }

            Avalonia.Controls.NativeMenuItem item = new(node.Header)
            {
                IsEnabled = node.IsEnabled,
                // A check mark only shows on items with a toggle type.
                ToggleType = node.ToggleType switch
                {
                    MainMenuToggleType.CheckBox => Avalonia.Controls.MenuItemToggleType.CheckBox,
                    MainMenuToggleType.Radio => Avalonia.Controls.MenuItemToggleType.Radio,
                    _ => node.IsChecked ? Avalonia.Controls.MenuItemToggleType.CheckBox : Avalonia.Controls.MenuItemToggleType.None
                },
                IsChecked = node.IsChecked
            };

            if (node.Children != null)
            {
                item.Menu = new Avalonia.Controls.NativeMenu();
                Fill(item.Menu, node.Children);
            }
            else if (node.ExecuteAsync != null)
            {
                Func<System.Threading.Tasks.Task> execute = node.ExecuteAsync;
                item.Click += async (_, _) => await execute();
            }

            menu.Items.Add(item);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _singleClickTimer.Stop();
        _singleClickTimer.Tick -= OnSingleClickTimerTick;
        _menuRefreshTimer?.Stop();
        if (_nativeIcon != null)
        {
            _nativeIcon.MouseDown -= OnMouseDown;
            _nativeIcon.MouseUp -= OnMouseUp;
            _nativeIcon.CloseRequested -= OnCloseRequested;
            _nativeIcon.Dispose();
        }
        if (_avaloniaIcon != null)
        {
            _avaloniaIcon.Clicked -= OnAvaloniaClick;
            _avaloniaIconRegistration?.Dispose();
        }
        CloseRequested = null;
    }
}
