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
using System;
using System.Threading.Tasks;

namespace ShareX;

internal static class SingleInstanceCommandRouter
{
    private static readonly object SyncRoot = new();
    private static TaskCompletionSource _ready = CreateSignal();
    private static readonly TimeSpan StartupTimeout = TimeSpan.FromSeconds(5);
    private static bool _hasBeenReady;
    private static int _pauseCount;

    internal static void MarkReady()
    {
        lock (SyncRoot)
        {
            _hasBeenReady = true;
            if (_pauseCount == 0)
            {
                _ready.TrySetResult();
            }
        }
    }

    internal static bool Pause()
    {
        lock (SyncRoot)
        {
            if (!_hasBeenReady)
            {
                return false;
            }

            if (_pauseCount++ == 0)
            {
                _ready = CreateSignal();
            }

            return true;
        }
    }

    internal static void Resume()
    {
        lock (SyncRoot)
        {
            if (_pauseCount > 0 && --_pauseCount == 0)
            {
                _ready.TrySetResult();
            }
        }
    }

    internal static void ArgumentsReceived(string[] arguments)
    {
        string formattedArguments = arguments == null ? "null" : $"\"{string.Join(" ", arguments)}\"";
        DebugHelper.WriteLine("Arguments received: " + formattedArguments);
        _ = DispatchWhenReadyAsync(arguments);
    }

    private static async Task DispatchWhenReadyAsync(string[]? arguments)
    {
        try
        {
            Task readyTask;
            lock (SyncRoot)
            {
                readyTask = _ready.Task;
            }

            await readyTask.WaitAsync(StartupTimeout);
            await RunOnUiThreadAsync(() => ApplicationCommandLine.ExecuteReceivedAsync(arguments));
        }
        catch (TimeoutException)
        {
            DebugHelper.WriteLine("Arguments were not processed because application startup did not complete within 5 seconds.");
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
        }
    }

    private static Task RunOnUiThreadAsync(Func<Task> action)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            return action();
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(async () =>
        {
            try
            {
                await action();
                completion.TrySetResult();
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        });
        return completion.Task;
    }

    private static TaskCompletionSource CreateSignal() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);
}
