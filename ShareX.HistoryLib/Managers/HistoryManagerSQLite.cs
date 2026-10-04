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
using Newtonsoft.Json;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.HistoryLib
{
    public class HistoryManagerSQLite : HistoryManager, IDisposable
    {
        private SqliteConnection connection;
        // One connection serves the history window, uploads finishing in the background and shutdown; SQLite connections are not
        // thread safe, and closing one during another thread's transaction failed with "cannot rollback - no transaction is active".
        private readonly object connectionLock = new object();
        private bool disposed;

        public HistoryManagerSQLite(string filePath) : base(filePath)
        {
            Connect(filePath);
            EnsureDatabase();
        }

        private void Connect(string filePath)
        {
            FileHelpers.CreateDirectoryFromFilePath(filePath);

            string connectionString = $"Data Source={filePath}";
            connection = new SqliteConnection(connectionString);
            connection.Open();

            SetBusyTimeout(5000);
        }

        private void SetBusyTimeout(int milliseconds)
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"PRAGMA busy_timeout = {milliseconds};";
                cmd.ExecuteNonQuery();
            }
        }

        private void EnsureDatabase()
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS History (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FileName TEXT,
    FilePath TEXT,
    DateTime TEXT,
    Type TEXT,
    Host TEXT,
    URL TEXT,
    ThumbnailURL TEXT,
    DeletionURL TEXT,
    ShortenedURL TEXT,
    Tags TEXT
);
";
                cmd.ExecuteNonQuery();
            }
        }

        internal override List<HistoryItem> Load(string dbPath)
        {
            lock (connectionLock)
            {
                return disposed ? new List<HistoryItem>() : LoadCore(dbPath);
            }
        }

        private List<HistoryItem> LoadCore(string dbPath)
        {
            List<HistoryItem> items = new List<HistoryItem>();

            using (SqliteCommand cmd = new SqliteCommand("SELECT * FROM History;", connection))
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    HistoryItem item = new HistoryItem()
                    {
                        Id = (long)reader["Id"],
                        FileName = reader["FileName"].ToString(),
                        FilePath = reader["FilePath"].ToString(),
                        DateTime = DateTime.Parse(reader["DateTime"].ToString()),
                        Type = reader["Type"].ToString(),
                        Host = reader["Host"].ToString(),
                        URL = reader["URL"].ToString(),
                        ThumbnailURL = reader["ThumbnailURL"].ToString(),
                        DeletionURL = reader["DeletionURL"].ToString(),
                        ShortenedURL = reader["ShortenedURL"].ToString(),
                        Tags = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader["Tags"]?.ToString() ?? "{}")
                    };

                    items.Add(item);
                }
            }

            return items;
        }

        protected override bool Append(string dbPath, IEnumerable<HistoryItem> historyItems)
        {
            lock (connectionLock)
            {
                // A write that arrives after shutdown is reported as not saved instead of failing on a closed connection.
                return !disposed && AppendCore(historyItems);
            }
        }

        private bool AppendCore(IEnumerable<HistoryItem> historyItems)
        {
            using (SqliteTransaction transaction = connection.BeginTransaction())
            {
                foreach (HistoryItem item in historyItems)
                {
                    using (SqliteCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = @"
INSERT INTO History
(FileName, FilePath, DateTime, Type, Host, URL, ThumbnailURL, DeletionURL, ShortenedURL, Tags)
VALUES (@FileName, @FilePath, @DateTime, @Type, @Host, @URL, @ThumbnailURL, @DeletionURL, @ShortenedURL, @Tags);
SELECT last_insert_rowid();";
                        cmd.Parameters.AddWithValue("@FileName", item.FileName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FilePath", item.FilePath ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateTime", item.DateTime.ToString("o") ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Type", item.Type ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Host", item.Host ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@URL", item.URL ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ThumbnailURL", item.ThumbnailURL ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DeletionURL", item.DeletionURL ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ShortenedURL", item.ShortenedURL ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Tags", item.Tags != null ? JsonConvert.SerializeObject(item.Tags) : (object)DBNull.Value);
                        item.Id = (long)cmd.ExecuteScalar();
                    }
                }

                transaction.Commit();
            }

            return true;
        }

        public void Edit(HistoryItem item)
        {
            lock (connectionLock)
            {
                if (!disposed)
                {
                    EditCore(item);
                }
            }
        }

        private void EditCore(HistoryItem item)
        {
            using (SqliteTransaction transaction = connection.BeginTransaction())
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE History SET
FileName = @FileName,
FilePath = @FilePath,
DateTime = @DateTime,
Type = @Type,
Host = @Host,
URL = @URL,
ThumbnailURL = @ThumbnailURL,
DeletionURL = @DeletionURL,
ShortenedURL = @ShortenedURL,
Tags = @Tags
WHERE Id = @Id;";
                cmd.Parameters.AddWithValue("@FileName", item.FileName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FilePath", item.FilePath ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DateTime", item.DateTime.ToString("o") ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Type", item.Type ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Host", item.Host ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@URL", item.URL ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ThumbnailURL", item.ThumbnailURL ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DeletionURL", item.DeletionURL ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ShortenedURL", item.ShortenedURL ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Tags", item.Tags != null ? JsonConvert.SerializeObject(item.Tags) : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        public void Delete(params HistoryItem[] items)
        {
            lock (connectionLock)
            {
                if (!disposed)
                {
                    DeleteCore(items);
                }
            }
        }

        private void DeleteCore(HistoryItem[] items)
        {
            if (items != null && items.Length > 0)
            {
                using (SqliteTransaction transaction = connection.BeginTransaction())
                using (SqliteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM History WHERE Id = @Id;";
                    SqliteParameter idParam = cmd.CreateParameter();
                    idParam.ParameterName = "@Id";
                    cmd.Parameters.Add(idParam);

                    foreach (HistoryItem item in items)
                    {
                        idParam.Value = item.Id;
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        public void MigrateFromJSON(string jsonFilePath)
        {
            HistoryManagerJSON jsonManager = new HistoryManagerJSON(jsonFilePath);
            List<HistoryItem> items = jsonManager.Load(jsonFilePath);

            if (items.Count > 0)
            {
                Append(items);
            }
        }

        /// <summary>How long closing waits for queued writes before it leaves the database to close after the last one.</summary>
        internal TimeSpan CloseTimeout { get; set; } = TimeSpan.FromSeconds(10);

        internal bool IsClosed
        {
            get
            {
                lock (connectionLock)
                {
                    return disposed;
                }
            }
        }

        /// <summary>
        /// Refuses new background writes, then waits for the accepted ones. If they are still running after <see cref="CloseTimeout"/>,
        /// the database is not closed under them: it closes when the last one finishes, the delay is logged, and process exit
        /// waits for it (see <see cref="HistoryManager.FinishBeforeProcessExit"/>).
        /// </summary>
        public void Dispose()
        {
            Task pending = CloseBackgroundWrites();

            if (!WaitForWrites(pending, CloseTimeout))
            {
                DebugHelper.WriteLine($"History writes are still running after {CloseTimeout.TotalSeconds:0} seconds; the history database closes when they finish.");
                Task closing = pending.ContinueWith(_ => CloseConnection(), CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
                // The application may be exiting now; the process waits for these writes instead of abandoning them.
                FinishBeforeProcessExit(closing);
                return;
            }

            CloseConnection();
        }

        private void CloseConnection()
        {
            lock (connectionLock)
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                    SqliteConnection.ClearPool(connection);
                }
            }
        }
    }
}
