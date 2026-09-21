#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
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
