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
using System.IO;

namespace ShareX.Desktop.Workflows;

/// <summary>Writes to History.db, the same SQLite file and schema the Windows application uses.</summary>
public sealed class SqliteHistoryRecorder : IHistoryRecorder, IDisposable
{
    private readonly object sync = new object();

    public SqliteHistoryRecorder(string filePath)
    {
        string? folder = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(folder))
        {
            Directory.CreateDirectory(folder);
        }

        Manager = new HistoryManagerSQLite(filePath);
    }

    public HistoryManagerSQLite Manager { get; }

    public void Record(HistoryEntry entry)
    {
        lock (sync)
        {
            Manager.AppendHistoryItem(new HistoryItem
            {
                FileName = entry.FileName,
                FilePath = entry.FilePath ?? "",
                DateTime = entry.When,
                Type = entry.Type,
                Host = entry.Host,
                URL = entry.Url ?? ""
            });
        }
    }

    public void Dispose() => Manager.Dispose();
}
