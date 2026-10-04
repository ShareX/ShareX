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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using System.Diagnostics;
using Xunit;

namespace ShareX.ImageEditor.Tests;

public sealed class AvaloniaStartupLifetimeTests
{
    [DesktopFixtureTheory]
    [InlineData("cold")]
    [InlineData("early-dialog")]
    [InlineData("foreign-lifetime")]
    public async Task DesktopLifetimeStartsAndExitsOnce(string mode)
    {
        ProcessStartInfo info = new("dotnet")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        info.ArgumentList.Add(typeof(NativeProcessFixture).Assembly.Location);
        info.ArgumentList.Add("--avalonia-bootstrapper");
        info.ArgumentList.Add(mode);
        using Process process = Assert.IsType<Process>(Process.Start(info));
        Task<string> output = process.StandardOutput.ReadToEndAsync();
        Task<string> error = process.StandardError.ReadToEndAsync();
        try
        {
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(15));
            string diagnostics = await output.WaitAsync(TimeSpan.FromSeconds(2)) +
                await error.WaitAsync(TimeSpan.FromSeconds(2));
            Assert.True(process.ExitCode == 0, diagnostics);
            Assert.Contains("fixture passed: " + mode, diagnostics);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
            }
        }
    }

    internal static int RunFixture(string[] args)
    {
        try
        {
            string mode = args.Single();
            Application? early = null;
            if (mode == "early-dialog")
            {
                AvaloniaBootstrapper.EnsureInitialized();
                early = Application.Current;
                AvaloniaBootstrapper.EnsureInitialized();
                if (!ReferenceEquals(early, Application.Current)) throw new InvalidOperationException("Early application replaced.");
                // Exercise an actual early modal message loop, before the host attaches startup callbacks.
                DesktopServices.Run(() =>
                {
                    TaskCompletionSource<bool> closed = new(TaskCreationOptions.RunContinuationsAsynchronously);
                    Window dialog = new() { Title = "ShareX synthetic early error", Width = 180, Height = 80 };
                    dialog.Closed += (_, _) => closed.TrySetResult(true);
                    dialog.Show();
                    Dispatcher.UIThread.Post(dialog.Close);
                    return closed.Task;
                });
                if (((IClassicDesktopStyleApplicationLifetime)early!.ApplicationLifetime!).Windows.Count != 0)
                    throw new InvalidOperationException("Early dialog retained in the lifetime.");
            }
            if (mode == "foreign-lifetime")
            {
                AppBuilder.Configure<ShareXAvaloniaApplication>().UsePlatformDetect()
                    .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime());
                try
                {
                    AvaloniaBootstrapper.Initialize([], () => Task.CompletedTask, () => { });
                    throw new InvalidOperationException("An existing host lifetime was replaced.");
                }
                catch (InvalidOperationException exception) when (exception.Message == "Avalonia is already initialized.") { }
            }
            else
            {
                int started = 0, exited = 0;
                AvaloniaBootstrapper.Initialize(["synthetic"], () =>
                {
                    started++;
                    IClassicDesktopStyleApplicationLifetime desktop =
                        (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
                    Window window = new() { Title = "ShareX synthetic startup fixture", Width = 180, Height = 80 };
                    desktop.MainWindow = window;
                    window.Show();
                    Dispatcher.UIThread.Post(() =>
                    {
                        window.Close();
                        AvaloniaBootstrapper.Shutdown();
                        AvaloniaBootstrapper.Shutdown();
                    });
                    return Task.CompletedTask;
                }, () => exited++);
                if (early != null && !ReferenceEquals(early, Application.Current))
                    throw new InvalidOperationException("Dialog application was reinitialized.");
                try
                {
                    AvaloniaBootstrapper.Initialize([], () => Task.CompletedTask, () => { });
                    throw new InvalidOperationException("Duplicate initialization accepted.");
                }
                catch (InvalidOperationException exception) when (exception.Message == "Avalonia is already initialized.") { }
                if (AvaloniaBootstrapper.Run() != 0 || started != 1 || exited != 1)
                    throw new InvalidOperationException($"Invalid lifetime: startup={started}, exit={exited}.");
            }
            Console.WriteLine("fixture passed: " + mode);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
    }
}

public sealed class DesktopFixtureTheoryAttribute : TheoryAttribute
{
    public DesktopFixtureTheoryAttribute()
    {
        if (Environment.GetEnvironmentVariable("SHAREX_TEST_DESKTOP") != "1")
            Skip = "Set SHAREX_TEST_DESKTOP=1 on a real desktop to run the isolated Avalonia lifetime fixture.";
    }
}
