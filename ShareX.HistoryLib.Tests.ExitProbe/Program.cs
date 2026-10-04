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

using ShareX.HistoryLib;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ShareX.HistoryLib.Tests.ExitProbe
{
    /// <summary>
    /// Usage: ExitProbe DATABASE WRITE_MILLISECONDS. Queues one history item whose write takes WRITE_MILLISECONDS, closes the
    /// history with a 200 ms budget, as ShareX's close sequence does, and returns from Main while the write is still running.
    /// </summary>
    internal static class Program
    {
        private sealed class SlowHistoryManager(string path, int writeMilliseconds) : HistoryManagerSQLite(path)
        {
            protected override bool Append(string dbPath, IEnumerable<HistoryItem> historyItems)
            {
                Thread.Sleep(writeMilliseconds);
                return base.Append(dbPath, historyItems);
            }
        }

        private static int Main(string[] args)
        {
            SlowHistoryManager manager = new SlowHistoryManager(args[0], int.Parse(args[1])) { CloseTimeout = TimeSpan.FromMilliseconds(200) };
            manager.AppendHistoryItemInBackground(new HistoryItem
            {
                FileName = "exit-probe.png",
                FilePath = "/tmp/exit-probe.png",
                DateTime = new DateTime(2026, 10, 4, 12, 0, 0),
                Type = "Image",
                URL = "http://127.0.0.1:8765/f/exit-probe.png"
            });
            manager.Dispose();
            Console.WriteLine("closed");
            return 0;
        }
    }
}
