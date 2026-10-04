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
using ShareX.HistoryLib;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ShareX.HistoryLib.Tests.ExitProbe
{
    /// <summary>
    /// Usage: ExitProbe DATABASE WRITE_MILLISECONDS. Queues one history item whose write takes WRITE_MILLISECONDS, closes the
    /// history with a 200 ms budget, as ShareX's close sequence does, and returns from Main while the write is still running.
    /// Optional EXIT_MILLISECONDS LOG_FILE exercise the bounded failure diagnostic; WRITE_MILLISECONDS=-1 never completes.
    /// </summary>
    internal static class Program
    {
        private sealed class SlowHistoryManager(string path, int writeMilliseconds) : HistoryManagerSQLite(path)
        {
            public ManualResetEventSlim WriteStarted { get; } = new ManualResetEventSlim();

            protected override bool Append(string dbPath, IEnumerable<HistoryItem> historyItems)
            {
                WriteStarted.Set();
                Thread.Sleep(writeMilliseconds);
                return base.Append(dbPath, historyItems);
            }
        }

        private static int Main(string[] args)
        {
            if (args.Length > 2)
            {
                HistoryManager.ProcessExitBudget = TimeSpan.FromMilliseconds(int.Parse(args[2]));
                DebugHelper.Init(args[3]);

                // Hold the asynchronous logger at real process exit, after Main's flush. Without the history callback's
                // own flush, the timeout message stays queued and the process ends before this worker can persist it.
                ManualResetEventSlim loggerHeld = new ManualResetEventSlim();
                DebugHelper.Logger.MessageAdded += message =>
                {
                    if (message.Contains("exit-probe: logger barrier", StringComparison.Ordinal))
                    {
                        loggerHeld.Set();
                        Thread.Sleep(1000);
                    }
                };
                AppDomain.CurrentDomain.ProcessExit += (_, _) =>
                {
                    DebugHelper.WriteLine("exit-probe: logger barrier");
                    if (!loggerHeld.Wait(TimeSpan.FromSeconds(5)))
                    {
                        Console.Error.WriteLine("The asynchronous logger barrier did not start.");
                    }
                };
            }

            SlowHistoryManager manager = new SlowHistoryManager(args[0], int.Parse(args[1])) { CloseTimeout = TimeSpan.FromMilliseconds(200) };
            manager.AppendHistoryItemInBackground(new HistoryItem
            {
                FileName = "exit-probe.png",
                FilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(args[0])!, "exit-probe.png"),
                DateTime = new DateTime(2026, 10, 4, 12, 0, 0),
                Type = "Image",
                URL = "http://127.0.0.1:8765/f/exit-probe.png"
            });
            if (!manager.WriteStarted.Wait(TimeSpan.FromSeconds(5)))
            {
                Console.Error.WriteLine("The history write did not start.");
                return 2;
            }
            manager.Dispose();
            Console.WriteLine("closed");
            // Mirror Program's final flush, which precedes the process-exit wait and any timeout diagnostic.
            DebugHelper.Flush();
            return 0;
        }
    }
}
