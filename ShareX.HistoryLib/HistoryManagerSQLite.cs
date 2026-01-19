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

namespace ShareX.HistoryLib
{
    public class HistoryManagerSQLite : HistoryManager, IDisposable
    {
        private SqliteConnection connection;

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

        public void Dispose()
        {
            connection?.Dispose();
        }
    }
}