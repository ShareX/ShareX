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

using ShareX.Desktop.Workflows;
using ShareX.HistoryLib;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace ShareX.Desktop.Tests;

public sealed class HistoryRecorderTests : IDisposable
{
    private readonly string folder = Path.Combine(Path.GetTempPath(), "sharex-history-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true);
        }
    }

    [Fact]
    public void Record_WritesRowsTheHistoryWindowCanReadBack()
    {
        string path = Path.Combine(folder, "nested", "History.db");
        DateTime when = new DateTime(2026, 10, 1, 5, 52, 39);

        using (SqliteHistoryRecorder recorder = new SqliteHistoryRecorder(path))
        {
            recorder.Record(new HistoryEntry("a.png", "/pics/a.png", null, "Image", "", when));
            recorder.Record(new HistoryEntry("b.png", "/pics/b.png", "https://example.test/b.png", "Image", "Example host", when.AddMinutes(1)));

            HistoryItem[] items = recorder.Manager.GetHistoryItems().OrderBy(i => i.DateTime).ToArray();

            Assert.Equal(2, items.Length);
            Assert.Equal("a.png", items[0].FileName);
            Assert.Equal("/pics/a.png", items[0].FilePath);
            Assert.Equal("Example host", items[1].Host);
            Assert.Equal("https://example.test/b.png", items[1].URL);
            Assert.Equal(when.AddMinutes(1), items[1].DateTime);
        }

        Assert.True(File.Exists(path));
    }

    [Fact]
    public void Record_IgnoresEntriesWithNothingToShow()
    {
        using SqliteHistoryRecorder recorder = new SqliteHistoryRecorder(Path.Combine(folder, "History.db"));

        recorder.Record(new HistoryEntry("x.png", null, null, "Image", "", DateTime.Now));

        Assert.Empty(recorder.Manager.GetHistoryItems());
    }
}
