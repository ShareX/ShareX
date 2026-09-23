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

namespace ShareX;

public static class AutoCaptureWindowIntegration
{
    private static AutoCaptureWindow? _window;

    public static bool IsRunning => _window?.IsRunning == true;

    public static void Show(TaskSettings taskSettings)
    {
        Dispatch(window =>
        {
            window.TaskSettings = taskSettings;
            window.ShowAndActivate();
        });
    }

    public static void Start(TaskSettings taskSettings)
    {
        Dispatch(window =>
        {
            if (!window.IsRunning)
            {
                window.TaskSettings = taskSettings;
                window.ShowAndActivate();
                window.Execute();
            }
        });
    }

    public static void Stop()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_window?.IsRunning == true)
            {
                _window.Execute();
            }
        });
    }

    private static void Dispatch(System.Action<AutoCaptureWindow> action)
    {
        Dispatcher.UIThread.Post(() => action(GetOrCreateWindow()));
    }

    private static AutoCaptureWindow GetOrCreateWindow()
    {
        if (_window == null)
        {
            _window = new AutoCaptureWindow();
            _window.Closed += (_, _) => _window = null;
        }

        return _window;
    }
}
