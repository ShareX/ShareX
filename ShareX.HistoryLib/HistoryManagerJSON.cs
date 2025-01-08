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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShareX.HelpersLib;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareX.HistoryLib
{
    public class HistoryManagerJSON : HistoryManager
    {
        private static readonly object thisLock = new object();

        public HistoryManagerJSON(string filePath) : base(filePath)
        {
        }

        protected override List<HistoryItem> Load(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                lock (thisLock)
                {
                    string json = File.ReadAllText(filePath, Encoding.UTF8);

                    if (!string.IsNullOrEmpty(json))
                    {
                        json = "[" + json + "]";

                        return JsonConvert.DeserializeObject<List<HistoryItem>>(json);
                    }
                }
            }

            return new List<HistoryItem>();
        }

        protected override bool Append(string filePath, IEnumerable<HistoryItem> historyItems)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                lock (thisLock)
                {
                    FileHelpers.CreateDirectoryFromFilePath(filePath);

                    using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, FileOptions.WriteThrough))
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.DefaultValueHandling = DefaultValueHandling.Ignore;

                        bool firstObject = fileStream.Length == 0;

                        foreach (HistoryItem historyItem in historyItems)
                        {
                            string json = "";

                            if (!firstObject)
                            {
                                json += ",\r\n";
                            }

                            json += JObject.FromObject(historyItem, serializer).ToString();

                            streamWriter.Write(json);

                            firstObject = false;
                        }
                    }

                    Backup(FilePath);
                }

                return true;
            }

            return false;
        }
    }
}