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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareX;

internal static class ApplicationSettingsRuntime
{
    private static MainForm? _hotkeyHost;

    internal static async Task InitializeAsync(MainForm hotkeyHost)
    {
        ArgumentNullException.ThrowIfNull(hotkeyHost);
        _hotkeyHost = hotkeyHost;
        await ReloadCoreAsync();
    }

    internal static Task ReloadAsync()
    {
        if (_hotkeyHost == null)
        {
            throw new InvalidOperationException("The application runtime has not been initialized.");
        }

        return ReloadCoreAsync();
    }

    internal static void Apply() => ApplyCore(refreshMenus: true);

    internal static void UpdateTrayIcon()
    {
        _hotkeyHost?.UpdateTrayIcon();
    }

    private static void ApplyCore(bool refreshMenus)
    {
        ApplicationConfig settings = ApplicationState.Settings;

        HelpersOptions.CurrentProxy = settings.ProxySettings;
        HelpersOptions.URLEncodeIgnoreEmoji = settings.URLEncodeIgnoreEmoji;
        HelpersOptions.DefaultCopyImageFillBackground = settings.DefaultClipboardCopyImageFillBackground;
        HelpersOptions.UseAlternativeClipboardCopyImage = settings.UseAlternativeClipboardCopyImage;
        HelpersOptions.UseAlternativeClipboardGetImage = settings.UseAlternativeClipboardGetImage;
        HelpersOptions.RotateImageByExifOrientationData = settings.RotateImageByExifOrientationData;
        HelpersOptions.BrowserPath = settings.BrowserPath;
        HelpersOptions.RecentColors = settings.RecentColors;
        HelpersOptions.DevMode = settings.DevMode;
        HelpersOptions.ShareXSpecialFolders = new Dictionary<string, string>
        {
            ["ShareXImageEffects"] = AppPaths.ImageEffectsFolder
        };

        TaskManager.RecentManager.MaxCount = settings.RecentTasksMaxCount;
        _hotkeyHost?.ApplyHotkeySettings();
        _hotkeyHost?.UpdateTrayIcon();

        ApplicationState.UpdateManager.AllowAutoUpdate = !SystemOptions.DisableUpdateCheck && settings.AutoCheckUpdate;
        ApplicationState.UpdateManager.UpdateChannel = settings.UpdateChannel;
        ApplicationState.UpdateManager.ConfigureAutoUpdate();

        MainWindowIntegration.SetTitle(ApplicationInfo.Title);
        MainWindowIntegration.SetTrayVisible(settings.ShowTray);
        if (refreshMenus)
        {
            MainWindowIntegration.RefreshMenus();
        }
    }

    private static async Task ReloadCoreAsync()
    {
        MainForm hotkeyHost = _hotkeyHost ??
            throw new InvalidOperationException("The application runtime has not been initialized.");
        bool resumeCommandRouting = SingleInstanceCommandRouter.Pause();

        try
        {
            TaskManager.RecentManager.InitItems();
            TaskbarManager.Enabled = ApplicationState.Settings.TaskbarProgressEnabled;

            ApplyCore(refreshMenus: false);
            await hotkeyHost.UpdateHotkeysAsync();

            ApplicationState.WatchFolderManager ??= new WatchFolderManager();
            ApplicationState.WatchFolderManager.UpdateWatchFolders();
            DebugHelper.WriteLine("WatchFolderManager started.");

            MainWindowIntegration.RefreshMenus();
        }
        finally
        {
            if (resumeCommandRouting)
            {
                SingleInstanceCommandRouter.Resume();
            }
        }
    }
}
