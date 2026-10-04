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

using ShareX.ScreenCaptureLib;
using Xunit;

namespace ShareX.ImageEditor.Tests;

public sealed class RecordingWindowLifetimeTests
{
    [Fact]
    public void QueuedTrayAndFrameCallbacksNeverTouchResourcesAfterClose()
    {
        ScreenRecordWindowViewModel model = new();
        Queue<Action> queue = new();
        using ManualResetEvent signal = new(false);
        int frameApplications = 0, notifications = 0;
        Assert.True(model.Dispatch(queue.Enqueue, () => signal.Set()));
        Assert.True(model.Dispatch(queue.Enqueue, () => frameApplications++));
        Assert.True(model.Dispatch(queue.Enqueue, () => notifications++));
        Assert.Equal(3, queue.Count);
        Assert.True(model.TryClose());
        signal.Dispose();
        foreach (Action callback in queue) callback();
        Assert.True(model.IsClosed);
        Assert.Equal(0, frameApplications);
        Assert.Equal(0, notifications);
        // A new callback must not even enter the dispatcher after closure.
        Assert.False(model.Dispatch(_ => throw new Exception("Must not dispatch."), () => signal.Set()));
        Assert.False(model.TryRun(() => signal.Reset()));
        Assert.False(model.TryClose());
    }

    [Fact]
    public async Task CloseWhileSynchronousInvokeIsWaitingRechecksBeforeExecution()
    {
        ScreenRecordWindowViewModel model = new();
        TaskCompletionSource<Action> queued = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource invoked = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int executions = 0;
        Task<bool> worker = Task.Run(() => model.Dispatch(callback =>
        {
            queued.SetResult(callback);
            // Models the synchronous UI invoke used by the recording worker.
            invoked.Task.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        }, () => executions++));
        try
        {
            Action callback = await queued.Task.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.True(model.TryClose());
            callback();
        }
        finally
        {
            invoked.TrySetResult();
        }
        Assert.True(await worker.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.Equal(0, executions);
    }

    [Fact]
    public void OpenWindowCommandsAndBothDispatchModesKeepTheirOrderAndFailures()
    {
        ScreenRecordWindowViewModel model = new();
        using ManualResetEvent signal = new(false);
        List<string> order = [];
        Queue<Action> queue = new();
        Assert.True(model.TryRun(() => { signal.Set(); order.Add("start"); }));
        Assert.True(signal.WaitOne(0));
        Assert.True(model.Dispatch(callback => callback(), () => { signal.Reset(); order.Add("pause"); }));
        Assert.False(signal.WaitOne(0));
        Assert.True(model.Dispatch(queue.Enqueue, () => { signal.Set(); order.Add("resume"); }));
        Assert.Equal(new[] { "start", "pause" }, order);
        queue.Dequeue()();
        Assert.True(signal.WaitOne(0));
        Assert.Equal(new[] { "start", "pause", "resume" }, order);

        InvalidOperationException error = new("Synthetic recorder callback failure.");
        Assert.Same(error, Assert.Throws<InvalidOperationException>(() => model.TryRun(() => throw error)));
        Assert.Same(error, Assert.Throws<InvalidOperationException>(() => model.Dispatch(callback => callback(), () => throw error)));
        Assert.Same(error, Assert.Throws<InvalidOperationException>(() => model.Dispatch(_ => throw error, () => order.Add("must not run"))));
        Assert.False(model.IsClosed);
        Assert.Equal(3, order.Count);
    }

    [Fact]
    public void ReentrantClosureRejectsLaterSignalsAndCallbacks()
    {
        ScreenRecordWindowViewModel model = new();
        using ManualResetEvent signal = new(false);
        bool signaled = true;
        Assert.True(model.TryRun(() =>
        {
            // A stop callback can close the window before the command signals its event.
            Assert.True(model.TryClose());
            signal.Dispose();
            signaled = model.TryRun(() => signal.Set());
        }));
        Assert.False(signaled);
        Assert.False(model.Dispatch(callback => callback(), () => signal.Reset()));
    }

    [Fact]
    public async Task RepeatedConcurrentCloseHasOnlyOneCleanupOwner()
    {
        ScreenRecordWindowViewModel model = new();
        int cleanupOwners = 0;
        await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Task.Run(() =>
        {
            if (model.TryClose()) Interlocked.Increment(ref cleanupOwners);
        })));
        Assert.Equal(1, cleanupOwners);
        Assert.True(model.IsClosed);
    }
}
