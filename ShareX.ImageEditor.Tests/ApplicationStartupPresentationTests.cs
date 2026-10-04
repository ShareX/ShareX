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

using ShareX.AvaloniaUI.Integration;
using ShareX.HelpersLib;
using System.Collections.Concurrent;
using Xunit;

namespace ShareX.ImageEditor.Tests;

public sealed class ApplicationStartupPresentationTests
{
    private static readonly TimeSpan Limit = TimeSpan.FromSeconds(5);
    private static TaskCompletionSource Signal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    [Fact]
    public async Task FirstRunReceivesWorkDuringWelcomeAndBuildsMainWindowInChosenLanguage()
    {
        Scenario scenario = new();
        TaskCompletionSource workStarted = Signal(), workDone = Signal();
        Task received = scenario.Router.RunAsync(async () => { workStarted.SetResult(); await workDone.Task; });
        Task startup = scenario.RunAsync(welcome: true, initialActions: false);
        await workStarted.Task.WaitAsync(Limit);

        Assert.False(scenario.WelcomeClosed.Task.IsCompleted);
        Assert.False(startup.IsCompleted);
        Assert.DoesNotContain(scenario.Log, entry => entry.StartsWith("main:"));
        Assert.Equal(0, scenario.WarningsShown);
        scenario.Language = "fr";
        scenario.WelcomeClosed.SetResult();
        await startup.WaitAsync(Limit);
        Assert.Contains("main:fr", scenario.Log);
        Assert.Contains("toolbar", scenario.Log);
        Assert.Equal(0, scenario.WarningsShown);

        workDone.SetResult();
        await received.WaitAsync(Limit);
        await scenario.StartupFinished.Task.WaitAsync(Limit);
        Assert.Equal(1, scenario.WarningsShown);
        Assert.Equal(["runtime", "welcome", "ready", "main:fr", "ready", "initial", "toolbar", "warnings"], scenario.Log.ToArray());
    }

    [Fact]
    public async Task InitialActionRunsBeforePendingWelcomeAndWarningsWaitForPresentation()
    {
        Scenario scenario = new();
        TaskCompletionSource initialStarted = Signal(), initialDone = Signal();
        scenario.ExecuteInitial = async () => { initialStarted.SetResult(); await initialDone.Task; };
        Task startup = scenario.RunAsync(welcome: true, initialActions: true);
        await initialStarted.Task.WaitAsync(Limit);
        Assert.False(scenario.WelcomeShown.Task.IsCompleted);
        initialDone.SetResult();
        await scenario.WelcomeShown.Task.WaitAsync(Limit);
        Assert.Equal(0, scenario.WarningsShown);
        scenario.WelcomeClosed.SetResult();
        await startup.WaitAsync(Limit);
        Assert.Equal(["runtime", "main:en", "ready", "initial", "welcome", "toolbar", "warnings"], scenario.Log.ToArray());
    }

    [Fact]
    public async Task InitialExitDoesNotOpenWelcomeToolbarOrWarnings()
    {
        Scenario scenario = new();
        scenario.ExecuteInitial = () => { scenario.Close(); return Task.CompletedTask; };
        await scenario.RunAsync(welcome: true, initialActions: true).WaitAsync(Limit);
        Assert.False(scenario.WelcomeShown.Task.IsCompleted);
        Assert.DoesNotContain("toolbar", scenario.Log);
        Assert.Equal(0, scenario.WarningsShown);
        Assert.Equal(["runtime", "main:en", "ready", "initial", "discard"], scenario.Log.ToArray());
    }

    [Fact]
    public async Task ReceivedExitDuringWelcomeDoesNotBuildMainWindowOrRunRemainingStartup()
    {
        Scenario scenario = new();
        Task received = scenario.Router.RunAsync(() => { scenario.Close(); return Task.CompletedTask; });
        Task startup = scenario.RunAsync(welcome: true, initialActions: false);
        await Task.WhenAll(startup, received).WaitAsync(Limit);
        Assert.Equal(["runtime", "welcome", "ready", "discard"], scenario.Log.ToArray());
        Assert.Equal(0, scenario.WarningsShown);
    }

    [Fact]
    public async Task ClosingDuringHostInitializationRejectsWaitingCommandsAndPresentation()
    {
        Scenario scenario = new();
        TaskCompletionSource runtimeDone = Signal();
        scenario.InitializeRuntime = () => runtimeDone.Task;
        Task received = scenario.Router.RunAsync(() => throw new InvalidOperationException("Must not launch."));
        Task startup = scenario.RunAsync(welcome: true, initialActions: false);
        scenario.Close();
        await received.WaitAsync(Limit);
        Assert.False(startup.IsCompleted);
        runtimeDone.SetResult();
        await startup.WaitAsync(Limit);
        Assert.Equal(["runtime", "discard"], scenario.Log.ToArray());
    }

    [Fact]
    public async Task ClosingDuringInitialWorkDoesNotPresentPendingWelcome()
    {
        Scenario scenario = new();
        TaskCompletionSource initialDone = Signal();
        scenario.ExecuteInitial = () => initialDone.Task;
        Task startup = scenario.RunAsync(welcome: true, initialActions: true);
        scenario.Close();
        initialDone.SetResult();
        await startup.WaitAsync(Limit);
        Assert.False(scenario.WelcomeShown.Task.IsCompleted);
        Assert.DoesNotContain("toolbar", scenario.Log);
        Assert.Equal(0, scenario.WarningsShown);
    }

    [Fact]
    public async Task NormalStartupWithoutWelcomeKeepsMainInitialAndToolbarOrder()
    {
        Scenario scenario = new();
        await scenario.RunAsync(welcome: false, initialActions: false).WaitAsync(Limit);
        Assert.Equal(["runtime", "main:en", "ready", "initial", "toolbar", "warnings"], scenario.Log.ToArray());
        Assert.Equal(1, scenario.WarningsShown);
    }

    [Fact]
    public async Task InitialFailureCompletesStartupAccountingAndPropagates()
    {
        Scenario scenario = new();
        scenario.ExecuteInitial = () => Task.FromException(new InvalidOperationException("Controlled initial failure."));
        await Assert.ThrowsAsync<InvalidOperationException>(() => scenario.RunAsync(welcome: true, initialActions: true).WaitAsync(Limit));
        Assert.False(scenario.WelcomeShown.Task.IsCompleted);
        Assert.DoesNotContain("toolbar", scenario.Log);
        Assert.True(scenario.StartupFinished.Task.IsCompleted);
    }

    [Fact]
    public void WarningDiscardInterruptsReentrantReleaseAndCanStartANewHold()
    {
        DeferredActionGate gate = new();
        List<int> shown = new();
        gate.Hold();
        gate.Run(() => { shown.Add(1); gate.Discard(); });
        gate.Run(() => shown.Add(2));
        gate.Release();
        gate.Run(() => shown.Add(3));
        Assert.Equal([1], shown);
        gate.Hold();
        gate.Run(() => shown.Add(4));
        gate.Release();
        Assert.Equal([1, 4], shown);
    }

    private sealed class Scenario
    {
        internal ConcurrentQueue<string> Log { get; } = new();
        internal StartupCommandRouter Router { get; }
        internal TaskCompletionSource WelcomeShown { get; } = Signal();
        internal TaskCompletionSource WelcomeClosed { get; } = Signal();
        internal TaskCompletionSource StartupFinished { get; } = Signal();
        internal Func<Task> InitializeRuntime { get; set; } = () => Task.CompletedTask;
        internal Func<Task> ExecuteInitial { get; set; } = () => Task.CompletedTask;
        internal string Language { get; set; } = "en";
        private readonly DeferredActionGate warnings = new();
        private int warningsShown;
        internal int WarningsShown => Volatile.Read(ref warningsShown);

        internal Scenario()
        {
            warnings.Hold();
            warnings.Run(() => { Interlocked.Increment(ref warningsShown); Log.Enqueue("warnings"); });
            Router = new(action => action(), closing =>
            {
                if (closing) { warnings.Discard(); Log.Enqueue("discard"); }
                else warnings.Release();
                StartupFinished.TrySetResult();
            });
        }

        internal void Close()
        {
            Router.Close();
            // Models MainWindowIntegration.Close closing the tracked welcome window during host shutdown.
            WelcomeClosed.TrySetResult();
        }

        internal Task RunAsync(bool welcome, bool initialActions) => ApplicationStartupPresentation.RunAsync(
            welcomePending: welcome,
            hasInitialActions: initialActions,
            initializeRuntimeAsync: () => { Log.Enqueue("runtime"); return InitializeRuntime(); },
            showWelcomeAsync: () => { Log.Enqueue("welcome"); WelcomeShown.TrySetResult(); return WelcomeClosed.Task; },
            initializeMainWindow: () => Log.Enqueue("main:" + Language),
            markReady: () => { Log.Enqueue("ready"); Router.MarkReady(); },
            executeInitialAsync: () => { Log.Enqueue("initial"); return ExecuteInitial(); },
            finishPresentation: () => Log.Enqueue("toolbar"),
            isClosing: () => Router.IsClosing,
            completeStartup: Router.CompleteStartup);
    }
}
