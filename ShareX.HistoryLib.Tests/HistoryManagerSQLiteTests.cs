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
using System.Threading;
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
    public async Task WritesAfterCloseAreRefusedWithoutThrowing()
    {
        HistoryManagerSQLite manager = new HistoryManagerSQLite(DatabasePath);
        manager.Dispose();

        Assert.False(manager.AppendHistoryItem(CreateItem(1)));
        Assert.False(await manager.AppendHistoryItemInBackground(CreateItem(2)));
        manager.Dispose();
    }

    /// <summary>Holds every write until the test releases it, like a slow disk.</summary>
    private sealed class SlowHistoryManager(string path) : HistoryManagerSQLite(path)
    {
        public ManualResetEventSlim Gate { get; } = new ManualResetEventSlim(false);
        public ManualResetEventSlim Writing { get; } = new ManualResetEventSlim(false);

        protected override bool Append(string dbPath, IEnumerable<HistoryItem> historyItems)
        {
            Writing.Set();
            Gate.Wait(TimeSpan.FromSeconds(30));
            return base.Append(dbPath, historyItems);
        }
    }

    [Fact]
    public async Task SlowWriteStillFinishesWhenCloseTimesOut()
    {
        SlowHistoryManager manager = new SlowHistoryManager(DatabasePath) { CloseTimeout = TimeSpan.FromMilliseconds(100) };
        Task<bool> write = manager.AppendHistoryItemInBackground(CreateItem(1));
        Assert.True(manager.Writing.Wait(TimeSpan.FromSeconds(10)));

        // Returns after the timeout without closing the database under the running write.
        manager.Dispose();
        Assert.False(write.IsCompleted);

        manager.Gate.Set();
        Assert.True(await write.WaitAsync(TimeSpan.FromSeconds(10)));

        HistoryManagerSQLite reopened = await ReopenWhenClosedAsync(manager);
        Assert.Single(reopened.GetHistoryItems());
        reopened.Dispose();
    }

    [Fact]
    public async Task WritesQueuedWhileClosingAreRefusedAndAcceptedOnesKept()
    {
        SlowHistoryManager manager = new SlowHistoryManager(DatabasePath) { CloseTimeout = TimeSpan.FromSeconds(10) };
        Task<bool> accepted = manager.AppendHistoryItemInBackground(CreateItem(1));
        Assert.True(manager.Writing.Wait(TimeSpan.FromSeconds(10)));

        Task closing = Task.Run(manager.Dispose);
        // Let Dispose stop accepting work, then try to queue behind it.
        await Task.Delay(200);
        Task<bool> late = manager.AppendHistoryItemInBackground(CreateItem(2));

        manager.Gate.Set();
        await closing.WaitAsync(TimeSpan.FromSeconds(15));

        Assert.True(await accepted);
        Assert.False(await late);

        HistoryManagerSQLite reopened = await ReopenWhenClosedAsync(manager);
        Assert.Equal(["capture-1.png"], reopened.GetHistoryItems().Select(x => x.FileName));
        reopened.Dispose();
    }

    /// <summary>Waits until a delayed close has released the database.</summary>
    private async Task<HistoryManagerSQLite> ReopenWhenClosedAsync(HistoryManagerSQLite closing)
    {
        for (int i = 0; i < 100 && !closing.IsClosed; i++)
        {
            await Task.Delay(50);
        }

        Assert.True(closing.IsClosed);
        return new HistoryManagerSQLite(DatabasePath);
    }
}
