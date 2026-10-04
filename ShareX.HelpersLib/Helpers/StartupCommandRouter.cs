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

using System;
using System.Threading.Tasks;

namespace ShareX.HelpersLib;

/// <summary>
/// Routes accepted commands after host initialization, pauses them during settings reload and rejects them during shutdown.
/// Startup finishes only when its presentation and every command accepted during startup have finished.
/// </summary>
public sealed class StartupCommandRouter
{
    private readonly object sync = new();
    private readonly Func<Func<Task>, Task> dispatch;
    private readonly Action<bool> finishStartup;
    private readonly TimeSpan reloadTimeout;
    private TaskCompletionSource ready = CreateSignal();
    private readonly TaskCompletionSource closed = CreateSignal();
    private bool hasBeenReady;
    private bool closing;
    private bool startupCompleted;
    private bool startupFinished;
    private int pauseCount;
    private int startupCommands;

    public StartupCommandRouter(Func<Func<Task>, Task> dispatch, Action<bool> finishStartup, TimeSpan? reloadTimeout = null)
    {
        this.dispatch = dispatch ?? throw new ArgumentNullException(nameof(dispatch));
        this.finishStartup = finishStartup ?? throw new ArgumentNullException(nameof(finishStartup));
        this.reloadTimeout = reloadTimeout ?? TimeSpan.FromSeconds(5);
    }

    public bool IsClosing
    {
        get { lock (sync) return closing; }
    }

    public void MarkReady()
    {
        lock (sync)
        {
            if (closing) return;
            hasBeenReady = true;
            if (pauseCount == 0) ready.TrySetResult();
        }
    }

    public bool Pause()
    {
        lock (sync)
        {
            if (!hasBeenReady || closing) return false;
            if (pauseCount++ == 0) ready = CreateSignal();
            return true;
        }
    }

    public void Resume()
    {
        lock (sync)
        {
            if (!closing && pauseCount > 0 && --pauseCount == 0) ready.TrySetResult();
        }
    }

    public Task RunAsync(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        bool acceptedDuringStartup;
        lock (sync)
        {
            if (closing) return Task.CompletedTask;
            acceptedDuringStartup = !startupFinished;
            if (acceptedDuringStartup) startupCommands++;
        }
        return RunAcceptedAsync(action, acceptedDuringStartup);
    }

    public void CompleteStartup()
    {
        bool notify;
        lock (sync)
        {
            startupCompleted = true;
            notify = TryFinishStartup();
        }
        if (notify) finishStartup(false);
    }

    public void Close()
    {
        bool notify;
        lock (sync)
        {
            if (closing) return;
            closing = true;
            ready.TrySetResult();
            closed.TrySetResult();
            notify = !startupFinished;
            startupFinished = true;
        }
        if (notify) finishStartup(true);
    }

    private async Task RunAcceptedAsync(Func<Task> action, bool acceptedDuringStartup)
    {
        try
        {
            if (!await WaitUntilReadyAsync().ConfigureAwait(false)) return;
            TaskCompletionSource actionStarted = CreateSignal();
            Task dispatched = dispatch(async () =>
            {
                // A reload or shutdown may start after readiness was signalled but before the dispatcher runs this callback.
                if (!await WaitUntilReadyAsync()) return;
                lock (sync)
                {
                    if (closing) return;
                    actionStarted.TrySetResult();
                }
                await action();
            });
            Task first = await Task.WhenAny(dispatched, actionStarted.Task, closed.Task).ConfigureAwait(false);
            if (first == closed.Task && !actionStarted.Task.IsCompleted)
            {
                // Shutdown may abandon the UI queue. Do not retain a command that never started on that queue.
                _ = ObserveDiscardedDispatchAsync(dispatched);
                return;
            }
            await dispatched.ConfigureAwait(false);
        }
        finally
        {
            if (acceptedDuringStartup)
            {
                bool notify;
                lock (sync)
                {
                    startupCommands--;
                    notify = TryFinishStartup();
                }
                if (notify) finishStartup(false);
            }
        }
    }

    private static async Task ObserveDiscardedDispatchAsync(Task dispatched)
    {
        try
        {
            await dispatched.ConfigureAwait(false);
        }
        catch
        {
            // Its action was rejected at shutdown; observe any dispatcher cancellation/failure after the caller has left.
        }
    }

    private async Task<bool> WaitUntilReadyAsync()
    {
        while (true)
        {
            Task pending;
            bool reload;
            lock (sync)
            {
                if (closing) return false;
                pending = ready.Task;
                reload = hasBeenReady;
            }
            if (reload) await pending.WaitAsync(reloadTimeout).ConfigureAwait(false);
            else await pending.ConfigureAwait(false);
            lock (sync)
            {
                if (closing) return false;
                if (hasBeenReady && pauseCount == 0) return true;
            }
        }
    }

    // Called with sync held. Commands accepted after this boundary are normal running-application commands.
    private bool TryFinishStartup()
    {
        if (closing || startupFinished || !startupCompleted || startupCommands != 0) return false;
        startupFinished = true;
        return true;
    }

    private static TaskCompletionSource CreateSignal() => new(TaskCreationOptions.RunContinuationsAsynchronously);
}
