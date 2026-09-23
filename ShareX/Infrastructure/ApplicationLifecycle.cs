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
using ShareX.AvaloniaUI;
using ShareX.AvaloniaUI.Integration;
using ShareX.HelpersLib;
using ShareX.Localization;
using System;
using System.Diagnostics;
using System.Threading;

namespace ShareX;

internal static class ApplicationLifecycle
{
    private static MainForm? _hotkeyHost;
    private static int _exitStarted;
    private static int _hostClosed;
    private static int _closeSequenceStarted;
    private static bool _restartRequested;
    private static bool _restartAsAdmin;

    internal static bool IsClosing => Volatile.Read(ref _exitStarted) != 0 || Volatile.Read(ref _closeSequenceStarted) != 0;

    internal static void AttachHost(MainForm hotkeyHost)
    {
        ArgumentNullException.ThrowIfNull(hotkeyHost);
        _hotkeyHost = hotkeyHost;
    }

    internal static void OnAvaloniaStopped()
    {
        MainForm? hotkeyHost = _hotkeyHost;
        if (Volatile.Read(ref _hostClosed) == 0 && hotkeyHost is { IsDisposed: false })
        {
            hotkeyHost.ExitApplication();
        }
    }

    internal static void OnHotkeyHostClosed()
    {
        if (Interlocked.Exchange(ref _hostClosed, 1) != 0)
        {
            return;
        }

        Interlocked.Exchange(ref _exitStarted, 1);
        ShareX.Tools.MouseHighlighterManager.Shutdown();
        MainWindowIntegration.Close();
        TaskManager.StopAllTasks();
        AvaloniaBootstrapper.Shutdown();
    }

    internal static void CloseSequence()
    {
        if (Interlocked.Exchange(ref _closeSequenceStarted, 1) != 0)
        {
            return;
        }

        DebugHelper.WriteLine("ShareX closing.");
        ApplicationState.WatchFolderManager?.Dispose();
        ApplicationState.WatchFolderManager = null;
        SettingManager.HistoryClose();
        SettingManager.SaveAllSettings();
        DebugHelper.WriteLine("ShareX closed.");
    }

    internal static void Restart(bool asAdmin = false)
    {
        _restartRequested = true;
        _restartAsAdmin = asAdmin;
        Exit();
    }

    internal static void RestartIfRequested()
    {
        if (!_restartRequested)
        {
            return;
        }

        DebugHelper.WriteLine("ShareX restarting.");
        string executablePath = Environment.ProcessPath ??
            throw new InvalidOperationException("Unable to determine the ShareX executable path.");

        ProcessStartInfo startInfo = new()
        {
            FileName = executablePath,
            UseShellExecute = true
        };

        if (_restartAsAdmin)
        {
            startInfo.Arguments = "-silent";
            startInfo.Verb = "runas";
        }

        Process.Start(startInfo);
    }

    internal static void Exit()
    {
        void ExitCore()
        {
            if (Interlocked.Exchange(ref _exitStarted, 1) != 0)
            {
                return;
            }

            if (_hotkeyHost is { IsDisposed: false } hotkeyHost)
            {
                hotkeyHost.ExitApplication();
            }
            else
            {
                AvaloniaBootstrapper.Shutdown();
            }
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            ExitCore();
        }
        else
        {
            Dispatcher.UIThread.Post(ExitCore);
        }
    }

    internal static void ForceClose()
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Post(ForceClose);
            return;
        }

        if (ScreenRecordManager.IsRecording)
        {
            if (MessageBox.Show(Strings.ShareXCannotBeClosedWhileScreenRecordingIsActive, ApplicationInfo.Name,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                ScreenRecordManager.AbortRecording();
            }

            return;
        }

        Exit();
    }
}
