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

using ShareX.HistoryLib;
using ShareX.UploadersLib;
using System;

namespace ShareX;

internal static class ApplicationState
{
    private static ApplicationConfig? _settings;
    private static UploadersConfig? _uploadersConfig;
    private static HotkeysConfig? _hotkeysConfig;

    internal static ApplicationConfig Settings
    {
        get => _settings ?? throw new InvalidOperationException("Application settings have not been loaded.");
        set => _settings = value;
    }

    internal static ApplicationConfig? SettingsOrNull => _settings;
    internal static TaskSettings DefaultTaskSettings { get; set; } = null!;

    internal static UploadersConfig UploadersConfig
    {
        get => _uploadersConfig ?? throw new InvalidOperationException("Uploader settings have not been loaded.");
        set => _uploadersConfig = value;
    }

    internal static UploadersConfig? UploadersConfigOrNull => _uploadersConfig;

    internal static HotkeysConfig HotkeysConfig
    {
        get => _hotkeysConfig ?? throw new InvalidOperationException("Hotkey settings have not been loaded.");
        set => _hotkeysConfig = value;
    }

    internal static HotkeysConfig? HotkeysConfigOrNull => _hotkeysConfig;
    internal static HistoryManagerSQLite? HistoryManager { get; set; }
    internal static HotkeyManager? HotkeyManager { get; set; }
    internal static WatchFolderManager? WatchFolderManager { get; set; }
    internal static ShareXUpdateManager UpdateManager { get; set; } = null!;
}
