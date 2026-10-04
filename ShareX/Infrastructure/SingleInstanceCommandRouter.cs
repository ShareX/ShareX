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
    private static readonly StartupCommandRouter Router = new(RunOnUiThreadAsync, OnStartupFinished);

    internal static bool IsClosing => Router.IsClosing;
    internal static void MarkReady() => Router.MarkReady();
    internal static bool Pause() => Router.Pause();
    internal static void Resume() => Router.Resume();
    internal static void CompleteStartup() => Router.CompleteStartup();
    internal static void Close() => Router.Close();

    private static void OnStartupFinished(bool closing) => _ = FinishStartupAsync(closing);

    private static async Task FinishStartupAsync(bool closing)
    {
        try
        {
            await RunOnUiThreadAsync(() =>
            {
                if (closing || ApplicationLifecycle.IsClosing) HotkeyManager.StartupWarnings.Discard();
                else HotkeyManager.StartupWarnings.Release();
                return Task.CompletedTask;
            });
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
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
            await Router.RunAsync(() => ApplicationCommandLine.ExecuteReceivedAsync(arguments));
        }
        catch (TimeoutException)
        {
            DebugHelper.WriteLine("Arguments were not processed because settings reload did not complete within 5 seconds.");
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
}
