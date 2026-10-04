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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.HistoryLib.Tests;

/// <summary>
/// B24/R29: uploads finishing in the background wrote history on a detached task while shutdown closed the same connection,
/// failing with "cannot rollback - no transaction is active" and losing the item.
/// </summary>
public class HistoryManagerSQLiteTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), "sharex-history-test-" + Guid.NewGuid().ToString("N"));

    private string DatabasePath => Path.Combine(directory, "History.db");

    public void Dispose()
    {
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }

    private static HistoryItem CreateItem(int index) => new HistoryItem
    {
        FileName = $"capture-{index}.png",
        FilePath = $"/tmp/capture-{index}.png",
        DateTime = new DateTime(2026, 10, 4, 12, 0, 0).AddSeconds(index),
        Type = "Image",
        URL = $"http://127.0.0.1:8765/f/{index}"
    };

    [Fact]
    public void ClosingRightAfterQueuedWritesKeepsEveryItemOnce()
    {
        HistoryManagerSQLite manager = new HistoryManagerSQLite(DatabasePath);

        for (int i = 0; i < 200; i++)
        {
            manager.AppendHistoryItemInBackground(CreateItem(i));
        }

        // What automatic exit does: close straight away.
        manager.Dispose();

        HistoryManagerSQLite reopened = new HistoryManagerSQLite(DatabasePath);
        List<HistoryItem> items = reopened.GetHistoryItems();
        reopened.Dispose();

        Assert.Equal(200, items.Count);
        Assert.Equal(200, items.Select(x => x.FileName).Distinct().Count());
    }

    [Fact]
    public void QueuedWritesKeepTheirOrder()
    {
        HistoryManagerSQLite manager = new HistoryManagerSQLite(DatabasePath);
        List<Task<bool>> writes = Enumerable.Range(0, 50).Select(i => manager.AppendHistoryItemInBackground(CreateItem(i))).ToList();

        Assert.True(manager.FlushBackgroundWrites(TimeSpan.FromSeconds(10)));
        Assert.All(writes, write => Assert.True(write.Result));
        List<HistoryItem> items = manager.GetHistoryItems().OrderBy(x => x.Id).ToList();
        manager.Dispose();

        Assert.Equal(Enumerable.Range(0, 50).Select(i => $"capture-{i}.png"), items.Select(x => x.FileName));
    }

    [Fact]
    public void ConcurrentReadsEditsAndWritesDoNotCollide()
    {
        HistoryManagerSQLite manager = new HistoryManagerSQLite(DatabasePath);
        manager.AppendHistoryItem(CreateItem(-1));
        HistoryItem first = manager.GetHistoryItems().Single();

        Parallel.For(0, 100, i =>
        {
            switch (i % 3)
            {
                case 0:
                    manager.AppendHistoryItem(CreateItem(i));
                    break;
                case 1:
                    first.Tags = new Dictionary<string, string> { ["n"] = i.ToString() };
                    manager.Edit(first);
                    break;
                default:
                    manager.GetHistoryItems();
                    break;
            }
        });

        Assert.Equal(1 + 34, manager.GetHistoryItems().Count);
        manager.Dispose();
    }

    [Fact]
    public void WritesAfterCloseAreRefusedWithoutThrowing()
    {
        HistoryManagerSQLite manager = new HistoryManagerSQLite(DatabasePath);
        manager.Dispose();

        Assert.False(manager.AppendHistoryItem(CreateItem(1)));
        Assert.False(manager.AppendHistoryItemInBackground(CreateItem(2)).Result);
        manager.Dispose();
    }
}
