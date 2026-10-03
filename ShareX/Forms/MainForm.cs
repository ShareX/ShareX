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

using Avalonia.Threading;
using ShareX.HelpersLib;
using ShareX.Platform;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace ShareX;

/// <summary>
/// Coordinates the application host and desktop notification-area icon.
/// Avalonia owns the application lifetime and all visible windows.
/// </summary>
internal sealed class MainForm
{
    private readonly IHotkeyHost _hotkeyHost;
    private readonly IApplicationSessionService _session;
    internal bool IsDisposed => _hotkeyHost.IsDisposed;
    internal ITrayIconService TrayIconService { get; }

    public MainForm()
    {
        _hotkeyHost = PlatformBootstrap.CreateApplicationHost();
        _hotkeyHost.Closed += OnHostClosed;
        _session = PlatformServices.Current.Session;
        _session.RestartRequested += OnRestartRequested;
        _session.SessionEnding += OnSessionEnding;

        ShareXResources.UseWhiteIcon = ApplicationState.Settings.UseWhiteShareXIcon;
        TrayIconService = new DesktopTrayIconService(_hotkeyHost, ShareXResources.IconBytes, ApplicationInfo.TitleShort, ApplicationState.Settings.ShowTray);
    }

    internal void Initialize()
    {
        _hotkeyHost.Initialize();
        _session.Initialize(exception => Dispatcher.UIThread.Post(ExceptionDispatchInfo.Capture(exception).Throw));
    }

    internal void UpdateTrayIcon()
    {
        ShareXResources.UseWhiteIcon = ApplicationState.Settings.UseWhiteShareXIcon;
        TrayIconService.SetIcon(ShareXResources.IconBytes);
    }

    internal async Task UpdateHotkeysAsync()
    {
        await Task.Run(SettingManager.WaitHotkeysConfig);

        HotkeyManager? hotkeyManager = ApplicationState.HotkeyManager;

        if (hotkeyManager == null)
        {
            hotkeyManager = new HotkeyManager();
            hotkeyManager.HotkeyTrigger += HandleHotkeys;
            ApplicationState.HotkeyManager = hotkeyManager;
        }

        hotkeyManager.UpdateHotkeys(ApplicationState.HotkeysConfig.Hotkeys, !StartupOptions.IgnoreHotkeyWarning);
        DebugHelper.WriteLine("HotkeyManager started.");
    }

    private async void HandleHotkeys(HotkeySettings hotkeySetting)
    {
        DebugHelper.WriteLine("Hotkey triggered. " + hotkeySetting);
        await TaskHelpers.ExecuteJob(hotkeySetting.TaskSettings);
    }

    internal void ExitApplication() => _hotkeyHost.Close();

    private void OnRestartRequested(object? sender, EventArgs e)
    {
        if (_session.RestartSupport.IsSupported)
        {
            _session.RegisterRestart("-silent");
        }
    }

    private void OnSessionEnding(object? sender, SessionEndingEventArgs e) => ApplicationLifecycle.CloseSequence();

    private void OnHostClosed(object? sender, EventArgs e)
    {
        ApplicationState.HotkeyManager?.UnregisterAllHotkeys(false);
        TrayIconService.Dispose();
        _session.RestartRequested -= OnRestartRequested;
        _session.SessionEnding -= OnSessionEnding;
        _session.Dispose();
        ApplicationLifecycle.OnHotkeyHostClosed();
    }
}
