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

using ShareX.HelpersLib;
using System.Collections.Concurrent;
using Xunit;

namespace ShareX.ImageEditor.Tests;

public sealed class StartupCommandRouterTests
{
    private static readonly TimeSpan Limit = TimeSpan.FromSeconds(5);
    private static TaskCompletionSource Signal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    [Fact]
    public async Task AcceptedCommandsWaitForHostAndAllFinishBeforeWarnings()
    {
        ConcurrentQueue<bool> finished = new();
        StartupCommandRouter router = new(action => action(), finished.Enqueue);
        TaskCompletionSource firstStarted = Signal(), secondStarted = Signal(), firstDone = Signal(), secondDone = Signal();
        Task first = router.RunAsync(async () => { firstStarted.SetResult(); await firstDone.Task; });
        Task second = router.RunAsync(async () => { secondStarted.SetResult(); await secondDone.Task; });

        Assert.False(firstStarted.Task.IsCompleted);
        router.MarkReady();
        await Task.WhenAll(firstStarted.Task, secondStarted.Task).WaitAsync(Limit);
        router.CompleteStartup();
        Assert.Empty(finished);
        firstDone.SetResult();
        await first.WaitAsync(Limit);
        Assert.Empty(finished);
        secondDone.SetResult();
        await second.WaitAsync(Limit);
        Assert.Equal([false], finished.ToArray());
    }

    [Fact]
    public async Task ClosingBeforeReadyReleasesAcceptedWaitersWithoutDispatch()
    {
        int dispatches = 0;
        List<bool> finished = new();
        StartupCommandRouter router = new(action => { dispatches++; return action(); }, finished.Add);
        Task command = router.RunAsync(() => throw new InvalidOperationException("Must not run."));
        router.Close();
        router.MarkReady();
        router.CompleteStartup();
        router.Close();
        await command.WaitAsync(Limit);
        await router.RunAsync(() => throw new InvalidOperationException("Must not be accepted."));
        Assert.Equal(0, dispatches);
        Assert.Equal([true], finished);
    }

    [Fact]
    public async Task ClosingDuringReloadReleasesWaitingCommand()
    {
        StartupCommandRouter router = new(action => action(), _ => { });
        router.MarkReady();
        Assert.True(router.Pause());
        int runs = 0;
        Task command = router.RunAsync(() => { runs++; return Task.CompletedTask; });
        router.Close();
        router.Resume();
        await command.WaitAsync(Limit);
        Assert.Equal(0, runs);
        Assert.False(router.Pause());
    }

    [Fact]
    public async Task ShutdownReleasesQueuedDispatcherWorkEvenIfTheUiQueueStops()
    {
        TaskCompletionSource<Func<Task>> queued = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource dispatchDone = Signal();
        StartupCommandRouter router = new(action => { queued.SetResult(action); return dispatchDone.Task; }, _ => { });
        router.MarkReady();
        int runs = 0;
        Task command = router.RunAsync(() => { runs++; return Task.CompletedTask; });
        Func<Task> callback = await queued.Task.WaitAsync(Limit);
        router.Close();
        // The queue has not run or acknowledged its callback. Shutdown must still complete this accepted waiter.
        await command.WaitAsync(Limit);
        Assert.False(dispatchDone.Task.IsCompleted);
        await callback().WaitAsync(Limit);
        dispatchDone.SetResult();
        Assert.Equal(0, runs);
    }

    [Fact]
    public async Task DispatcherRechecksReloadAfterReadiness()
    {
        TaskCompletionSource<Func<Task>> queued = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource dispatchDone = Signal();
        StartupCommandRouter router = new(action => { queued.SetResult(action); return dispatchDone.Task; }, _ => { });
        router.MarkReady();
        int runs = 0;
        Task command = router.RunAsync(() => { runs++; return Task.CompletedTask; });
        Func<Task> callback = await queued.Task.WaitAsync(Limit);
        Assert.True(router.Pause());
        Task callbackTask = callback();
        Assert.Equal(0, runs);
        Assert.False(callbackTask.IsCompleted);
        router.Resume();
        await callbackTask.WaitAsync(Limit);
        dispatchDone.SetResult();
        await command.WaitAsync(Limit);
        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task NestedReloadRequiresLastResume()
    {
        StartupCommandRouter router = new(action => action(), _ => { });
        Assert.False(router.Pause());
        router.MarkReady();
        Assert.True(router.Pause());
        Assert.True(router.Pause());
        int runs = 0;
        Task command = router.RunAsync(() => { runs++; return Task.CompletedTask; });
        router.Resume();
        Assert.False(command.IsCompleted);
        router.MarkReady();
        Assert.Equal(0, runs);
        router.Resume();
        await command.WaitAsync(Limit);
        router.Resume();
        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task ReloadTimeoutDoesNotRetainStartupWork()
    {
        ConcurrentQueue<bool> finished = new();
        StartupCommandRouter router = new(action => action(), finished.Enqueue, TimeSpan.FromMilliseconds(50));
        router.MarkReady();
        router.Pause();
        Task command = router.RunAsync(() => throw new InvalidOperationException("Must not run."));
        router.CompleteStartup();
        await Assert.ThrowsAsync<TimeoutException>(() => command.WaitAsync(Limit));
        Assert.Equal([false], finished.ToArray());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ActionFailureOrCancellationCompletesAcceptedStartupWork(bool cancel)
    {
        ConcurrentQueue<bool> finished = new();
        StartupCommandRouter router = new(action => action(), finished.Enqueue);
        Task command = router.RunAsync(() => cancel
            ? Task.FromCanceled(new CancellationToken(true))
            : Task.FromException(new InvalidOperationException("Controlled failure.")));
        router.CompleteStartup();
        router.MarkReady();
        if (cancel) await Assert.ThrowsAnyAsync<OperationCanceledException>(() => command.WaitAsync(Limit));
        else await Assert.ThrowsAsync<InvalidOperationException>(() => command.WaitAsync(Limit));
        Assert.Equal([false], finished.ToArray());
    }

    [Fact]
    public async Task DispatcherFailureCompletesAcceptedStartupWork()
    {
        ConcurrentQueue<bool> finished = new();
        StartupCommandRouter router = new(_ => throw new InvalidOperationException("Dispatcher failure."), finished.Enqueue);
        Task command = router.RunAsync(() => Task.CompletedTask);
        router.CompleteStartup();
        router.MarkReady();
        await Assert.ThrowsAsync<InvalidOperationException>(() => command.WaitAsync(Limit));
        Assert.Equal([false], finished.ToArray());
    }

    [Fact]
    public async Task LaterCommandsDoNotRepeatStartupCompletion()
    {
        List<bool> finished = new();
        StartupCommandRouter router = new(action => action(), finished.Add);
        router.MarkReady();
        router.CompleteStartup();
        router.CompleteStartup();
        int runs = 0;
        await router.RunAsync(() => { runs++; return Task.CompletedTask; });
        router.Close();
        Assert.Equal(1, runs);
        Assert.Equal([false], finished);
    }

    [Fact]
    public async Task ClosingRetainsActiveCompletionAndDiscardsLateWarnings()
    {
        DeferredActionGate warnings = new();
        warnings.Hold();
        int shown = 0;
        warnings.Run(() => shown++);
        StartupCommandRouter router = new(action => action(), closing =>
        {
            if (closing) warnings.Discard(); else warnings.Release();
        });
        router.MarkReady();
        TaskCompletionSource done = Signal();
        Task command = router.RunAsync(() => done.Task);
        router.Close();
        Assert.False(command.IsCompleted);
        warnings.Run(() => shown++);
        router.CompleteStartup();
        done.SetResult();
        await command.WaitAsync(Limit);
        warnings.Release();
        Assert.Equal(0, shown);
        Assert.False(warnings.IsHeld);
    }
}
