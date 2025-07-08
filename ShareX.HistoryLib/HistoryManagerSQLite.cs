#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ShareX.HistoryLib
{
    public class HistoryManagerSQLite : HistoryManager, IDisposable
    {
        private readonly object dbLock = new object();
        private readonly SQLiteConnection connection;

        public HistoryManagerSQLite(string dbFilePath) : base(dbFilePath)
        {
            FileHelpers.CreateDirectoryFromFilePath(dbFilePath);

            string connectionString = $"Data Source={dbFilePath};Version=3;";
            connection = new SQLiteConnection(connectionString);
            connection.Open();

            EnsureDatabase();
        }

        private void EnsureDatabase()
        {
            lock (dbLock)
            {
                using (SQLiteCommand cmd = connection.CreateCommand())
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
    ShortenedURL TEXT
);

CREATE TABLE IF NOT EXISTS Tags (
    HistoryId INTEGER,
    Key TEXT,
    Value TEXT,
    FOREIGN KEY(HistoryId) REFERENCES History(Id)
);
";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        internal override List<HistoryItem> Load(string dbPath)
        {
            List<HistoryItem> items = new List<HistoryItem>();

            lock (dbLock)
            {
                string sql = @"SELECT * FROM History;";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
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
                            Tags = new Dictionary<string, string>()
                        };

                        items.Add(item);
                    }
                }

                string tagSql = @"SELECT HistoryId, Key, Value FROM Tags;";
                using (SQLiteCommand cmd = new SQLiteCommand(tagSql, connection))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        long historyId = (long)reader["HistoryId"];
                        string key = reader["Key"].ToString();
                        string value = reader["Value"].ToString();

                        HistoryItem item = items.Find(i => i.Id == historyId);

                        if (item != null)
                        {
                            item.Tags[key] = value;
                        }
                    }
                }
            }

            return items;
        }

        protected override bool Append(string dbPath, IEnumerable<HistoryItem> historyItems)
        {
            lock (dbLock)
            {
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    foreach (HistoryItem item in historyItems)
                    {
                        long newId;

                        using (SQLiteCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = @"
INSERT INTO History
(FileName, FilePath, DateTime, Type, Host, URL, ThumbnailURL, DeletionURL, ShortenedURL)
VALUES (@FileName, @FilePath, @DateTime, @Type, @Host, @URL, @ThumbnailURL, @DeletionURL, @ShortenedURL);
SELECT last_insert_rowid();";
                            cmd.Parameters.AddWithValue("@FileName", item.FileName);
                            cmd.Parameters.AddWithValue("@FilePath", item.FilePath);
                            cmd.Parameters.AddWithValue("@DateTime", item.DateTime.ToString("o"));
                            cmd.Parameters.AddWithValue("@Type", item.Type);
                            cmd.Parameters.AddWithValue("@Host", item.Host);
                            cmd.Parameters.AddWithValue("@URL", item.URL);
                            cmd.Parameters.AddWithValue("@ThumbnailURL", item.ThumbnailURL);
                            cmd.Parameters.AddWithValue("@DeletionURL", item.DeletionURL);
                            cmd.Parameters.AddWithValue("@ShortenedURL", item.ShortenedURL);
                            newId = (long)cmd.ExecuteScalar();
                        }

                        if (item.Tags != null)
                        {
                            foreach (KeyValuePair<string, string> kvp in item.Tags)
                            {
                                using (SQLiteCommand tagCmd = connection.CreateCommand())
                                {
                                    tagCmd.CommandText = @"
INSERT INTO Tags (HistoryId, Key, Value) VALUES (@HistoryId, @Key, @Value);";
                                    tagCmd.Parameters.AddWithValue("@HistoryId", newId);
                                    tagCmd.Parameters.AddWithValue("@Key", kvp.Key);
                                    tagCmd.Parameters.AddWithValue("@Value", kvp.Value);
                                    tagCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    transaction.Commit();
                }
            }

            return true;
        }

        public void Edit(HistoryItem item)
        {
            lock (dbLock)
            {
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand cmd = connection.CreateCommand())
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
ShortenedURL = @ShortenedURL
WHERE Id = @Id;";
                        cmd.Parameters.AddWithValue("@FileName", item.FileName);
                        cmd.Parameters.AddWithValue("@FilePath", item.FilePath);
                        cmd.Parameters.AddWithValue("@DateTime", item.DateTime.ToString("o"));
                        cmd.Parameters.AddWithValue("@Type", item.Type);
                        cmd.Parameters.AddWithValue("@Host", item.Host);
                        cmd.Parameters.AddWithValue("@URL", item.URL);
                        cmd.Parameters.AddWithValue("@ThumbnailURL", item.ThumbnailURL);
                        cmd.Parameters.AddWithValue("@DeletionURL", item.DeletionURL);
                        cmd.Parameters.AddWithValue("@ShortenedURL", item.ShortenedURL);
                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.ExecuteNonQuery();
                    }

                    using (SQLiteCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Tags WHERE HistoryId = @HistoryId;";
                        cmd.Parameters.AddWithValue("@HistoryId", item.Id);
                        cmd.ExecuteNonQuery();
                    }

                    if (item.Tags != null)
                    {
                        foreach (KeyValuePair<string, string> kvp in item.Tags)
                        {
                            using (SQLiteCommand cmd = connection.CreateCommand())
                            {
                                cmd.CommandText = "INSERT INTO Tags (HistoryId, Key, Value) VALUES (@HistoryId, @Key, @Value);";
                                cmd.Parameters.AddWithValue("@HistoryId", item.Id);
                                cmd.Parameters.AddWithValue("@Key", kvp.Key);
                                cmd.Parameters.AddWithValue("@Value", kvp.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        public void Delete(HistoryItem item)
        {
            lock (dbLock)
            {
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Tags WHERE HistoryId = @HistoryId;";
                        cmd.Parameters.AddWithValue("@HistoryId", item.Id);
                        cmd.ExecuteNonQuery();
                    }

                    using (SQLiteCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM History WHERE Id = @Id;";
                        cmd.Parameters.AddWithValue("@Id", item.Id);
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