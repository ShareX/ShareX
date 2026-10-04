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
using Xunit;

namespace ShareX.HistoryLib.Tests;

/// <summary>R38: an accepted history write that outlives closing the history still reaches the database when the process exits.</summary>
public class HistoryExitTests
{
    [Fact]
    public void SlowWriteSurvivesProcessExitAfterCloseTimeout()
    {
        string directory = Path.Combine(Path.GetTempPath(), "sharex-history-exit-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string database = Path.Combine(directory, "History.db");
        string probe = Path.Combine(AppContext.BaseDirectory, "ShareX.HistoryLib.Tests.ExitProbe.dll");

        try
        {
            ProcessStartInfo start = new ProcessStartInfo(Environment.ProcessPath!.EndsWith("dotnet", StringComparison.OrdinalIgnoreCase) ||
                Environment.ProcessPath.EndsWith("dotnet.exe", StringComparison.OrdinalIgnoreCase) ? Environment.ProcessPath : "dotnet")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            start.ArgumentList.Add(probe);
            start.ArgumentList.Add(database);
            start.ArgumentList.Add("2000");

            Stopwatch timer = Stopwatch.StartNew();
            using Process child = Process.Start(start)!;
            string output = child.StandardOutput.ReadToEnd();
            Assert.True(child.WaitForExit(TimeSpan.FromSeconds(60)));
            timer.Stop();

            Assert.Equal(0, child.ExitCode);
            Assert.Contains("closed", output);
            // Main returned after the 200 ms close budget; the process stayed alive for the 2 s write.
            Assert.True(timer.ElapsedMilliseconds >= 2000, $"exited after {timer.ElapsedMilliseconds} ms");

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
}
