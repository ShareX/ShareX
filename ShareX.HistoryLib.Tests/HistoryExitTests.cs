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

using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.HistoryLib.Tests;

/// <summary>R38: an accepted history write that outlives closing the history still reaches the database when the process exits.</summary>
public class HistoryExitTests
{
    [Fact]
    public async Task SlowWriteSurvivesProcessExitAfterCloseTimeout()
    {
        string directory = Path.Combine(Path.GetTempPath(), "sharex-history-exit-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string database = Path.Combine(directory, "History.db");
        try
        {
            (string output, TimeSpan elapsed) = await RunProbeAsync(database, "2000");
            Assert.Contains("closed", output);
            // Main returned after the 200 ms close budget; the process stayed alive for the 2 s write.
            Assert.True(elapsed.TotalMilliseconds >= 2000, $"exited after {elapsed.TotalMilliseconds} ms");

            HistoryManagerSQLite reopened = new HistoryManagerSQLite(database);
            Assert.Equal(["exit-probe.png"], reopened.GetHistoryItems().Select(x => x.FileName));
            reopened.Dispose();
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task ProcessExitTimeoutPersistsItsDiagnosticAfterTheFinalFlush()
    {
        string directory = Path.Combine(Path.GetTempPath(), "sharex-history-exit-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string database = Path.Combine(directory, "History.db");
        string log = Path.Combine(directory, "exit.log");

        try
        {
            // The accepted write never finishes, and the child flushes before returning from Main. Only ProcessExit can
            // report its short injected budget; reading the file after real exit verifies that the diagnostic survived.
            (string output, TimeSpan elapsed) = await RunProbeAsync(database, "-1", "100", log);
            Assert.Contains("closed", output);
            Assert.True(elapsed < TimeSpan.FromSeconds(10), $"exited after {elapsed.TotalMilliseconds} ms");
            string diagnostic = File.ReadAllText(log);
            Assert.Contains("exit-probe: logger barrier", diagnostic);
            Assert.Contains("History writes did not finish within ", diagnostic);
            Assert.Contains("seconds of exit; the last items may be missing from the history.", diagnostic);

            using HistoryManagerSQLite reopened = new HistoryManagerSQLite(database);
            Assert.Empty(reopened.GetHistoryItems());
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            Directory.Delete(directory, true);
        }
    }

    private static async Task<(string Output, TimeSpan Elapsed)> RunProbeAsync(params string[] arguments)
    {
        string host = Environment.ProcessPath!;
        ProcessStartInfo start = new ProcessStartInfo(host.EndsWith("dotnet", StringComparison.OrdinalIgnoreCase) ||
            host.EndsWith("dotnet.exe", StringComparison.OrdinalIgnoreCase) ? host : "dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };
        start.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "ShareX.HistoryLib.Tests.ExitProbe.dll"));
        foreach (string argument in arguments) start.ArgumentList.Add(argument);

        Stopwatch timer = Stopwatch.StartNew();
        using Process child = Process.Start(start)!;
        Task<string> output = child.StandardOutput.ReadToEndAsync();
        Task<string> error = child.StandardError.ReadToEndAsync();
        bool exited = false;
        try
        {
            await child.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(10));
            exited = true;
        }
        catch (TimeoutException)
        {
            // Terminate only this fixture's child, then report the original timeout with its bounded diagnostics.
        }
        finally
        {
            if (!child.HasExited)
            {
                child.Kill(entireProcessTree: true);
                await child.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
            }
        }
        timer.Stop();
        string[] diagnostics = await Task.WhenAll(output, error).WaitAsync(TimeSpan.FromSeconds(5));
        string combined = string.Join(Environment.NewLine, diagnostics);
        Assert.True(exited, "History exit child did not finish within 10 seconds." + Environment.NewLine + combined);
        Assert.True(child.ExitCode == 0, $"Exit code: {child.ExitCode}" + Environment.NewLine + combined);
        return (combined, timer.Elapsed);
    }
}
