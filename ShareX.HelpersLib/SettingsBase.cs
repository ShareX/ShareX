#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public abstract class SettingsBase<T> where T : SettingsBase<T>, new()
    {
        public delegate void SettingsSavedEventHandler(T settings, string filePath, bool result);
        public event SettingsSavedEventHandler SettingsSaved;

        public delegate void SettingsSaveFailedEventHandler(Exception e);
        public event SettingsSaveFailedEventHandler SettingsSaveFailed;

        [Browsable(false), JsonIgnore]
        public string FilePath { get; private set; }

        [Browsable(false)]
        public string ApplicationVersion { get; set; }

        [Browsable(false), JsonIgnore]
        public bool IsFirstTimeRun { get; private set; }

        [Browsable(false), JsonIgnore]
        public bool IsUpgrade { get; private set; }

        [Browsable(false), JsonIgnore]
        public string BackupFolder { get; set; }

        [Browsable(false), JsonIgnore]
        public bool CreateBackup { get; set; }

        [Browsable(false), JsonIgnore]
        public bool CreateWeeklyBackup { get; set; }

        public bool IsUpgradeFrom(string version)
        {
            return IsUpgrade && Helpers.CompareVersion(ApplicationVersion, version) <= 0;
        }

        protected virtual void OnSettingsSaved(string filePath, bool result)
        {
            if (SettingsSaved != null)
            {
                SettingsSaved((T)this, filePath, result);
            }
        }

        protected virtual void OnSettingsSaveFailed(Exception e)
        {
            if (SettingsSaveFailed != null)
            {
                SettingsSaveFailed(e);
            }
        }

        public bool Save(string filePath)
        {
            FilePath = filePath;
            ApplicationVersion = Application.ProductVersion;

            bool result = SaveInternal(FilePath);

            OnSettingsSaved(FilePath, result);

            return result;
        }

        public bool Save()
        {
            return Save(FilePath);
        }

        public void SaveAsync(string filePath)
        {
            Task.Run(() => Save(filePath));
        }

        public void SaveAsync()
        {
            SaveAsync(FilePath);
        }

        public static T Load(string filePath, string backupFolder = null, bool createBackup = false, bool createWeeklyBackup = false)
        {
            T setting = LoadInternal(filePath, backupFolder);

            if (setting != null)
            {
                setting.FilePath = filePath;
                setting.IsFirstTimeRun = string.IsNullOrEmpty(setting.ApplicationVersion);
                setting.IsUpgrade = !setting.IsFirstTimeRun && Helpers.CompareApplicationVersion(setting.ApplicationVersion) < 0;
                setting.BackupFolder = backupFolder;
                setting.CreateBackup = createBackup;
                setting.CreateWeeklyBackup = createWeeklyBackup;
            }

            return setting;
        }

        private bool SaveInternal(string filePath)
        {
            string typeName = GetType().Name;
            DebugHelper.WriteLine($"{typeName} save started: {filePath}");

            bool isSuccess = false;

            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    lock (this)
                    {
                        Helpers.CreateDirectoryFromFilePath(filePath);

                        string tempFilePath = filePath + ".temp";

                        using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                        using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.ContractResolver = new WritablePropertiesOnlyResolver();
                            serializer.Converters.Add(new StringEnumConverter());
                            serializer.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                            serializer.Formatting = Formatting.Indented;
                            serializer.Serialize(jsonWriter, this);
                            jsonWriter.Flush();
                        }

                        if (File.Exists(filePath))
                        {
                            if (CreateBackup)
                            {
                                Helpers.CopyFile(filePath, BackupFolder);
                            }

                            File.Delete(filePath);
                        }

                        File.Move(tempFilePath, filePath);

                        if (CreateWeeklyBackup && !string.IsNullOrEmpty(BackupFolder))
                        {
                            Helpers.BackupFileWeekly(filePath, BackupFolder);
                        }

                        isSuccess = true;
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);

                OnSettingsSaveFailed(e);
            }
            finally
            {
                string status = isSuccess ? "successful" : "failed";
                DebugHelper.WriteLine($"{typeName} save {status}: {filePath}");
            }

            return isSuccess;
        }

        private static T LoadInternal(string filePath, string backupFolder = null)
        {
            string typeName = typeof(T).Name;

            if (!string.IsNullOrEmpty(filePath))
            {
                DebugHelper.WriteLine($"{typeName} load started: {filePath}");

                try
                {
                    if (File.Exists(filePath))
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            if (fileStream.Length > 0)
                            {
                                T settings;

                                using (StreamReader streamReader = new StreamReader(fileStream))
                                using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                                {
                                    JsonSerializer serializer = new JsonSerializer();
                                    serializer.Converters.Add(new StringEnumConverter());
                                    serializer.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                                    serializer.ObjectCreationHandling = ObjectCreationHandling.Replace;
                                    serializer.Error += Serializer_Error;
                                    settings = serializer.Deserialize<T>(jsonReader);
                                }

                                if (settings == null)
                                {
                                    throw new Exception($"{typeName} object is null.");
                                }

                                DebugHelper.WriteLine($"{typeName} load finished: {filePath}");

                                return settings;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, $"{typeName} load failed: {filePath}");
                }

                if (!string.IsNullOrEmpty(backupFolder))
                {
                    string fileName = Path.GetFileName(filePath);
                    string backupFilePath = Path.Combine(backupFolder, fileName);
                    return LoadInternal(backupFilePath);
                }
            }

            DebugHelper.WriteLine($"{typeName} not found. Loading new instance.");

            return new T();
        }

        private static void Serializer_Error(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            // Handle missing enum values
            if (e.ErrorContext.Error.Message.StartsWith("Error converting value"))
            {
                e.ErrorContext.Handled = true;
            }
        }
    }
}