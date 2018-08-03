#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
            DebugHelper.WriteLine("{0} save started: {1}", typeName, filePath);

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
                            jsonWriter.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                            jsonWriter.Formatting = Formatting.Indented;

                            JsonSerializer serializer = new JsonSerializer();
                            serializer.ContractResolver = new WritablePropertiesOnlyResolver();
                            serializer.Converters.Add(new StringEnumConverter());
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
            }
            finally
            {
                DebugHelper.WriteLine("{0} save {1}: {2}", typeName, isSuccess ? "successful" : "failed", filePath);
            }

            return isSuccess;
        }

        private static T LoadInternal(string filePath, string backupFolder = null)
        {
            string typeName = typeof(T).Name;

            if (!string.IsNullOrEmpty(filePath))
            {
                DebugHelper.WriteLine("{0} load started: {1}", typeName, filePath);

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
                                    jsonReader.DateTimeZoneHandling = DateTimeZoneHandling.Local;

                                    JsonSerializer serializer = new JsonSerializer();
                                    serializer.Converters.Add(new StringEnumConverter());
                                    serializer.ObjectCreationHandling = ObjectCreationHandling.Replace;
                                    serializer.Error += (sender, e) => e.ErrorContext.Handled = true;
                                    settings = serializer.Deserialize<T>(jsonReader);
                                }

                                if (settings == null)
                                {
                                    throw new Exception(typeName + " object is null.");
                                }

                                DebugHelper.WriteLine("{0} load finished: {1}", typeName, filePath);

                                return settings;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, typeName + " load failed: " + filePath);
                }

                if (!string.IsNullOrEmpty(backupFolder))
                {
                    string fileName = Path.GetFileName(filePath);
                    string backupFilePath = Path.Combine(backupFolder, fileName);
                    return LoadInternal(backupFilePath);
                }
            }

            DebugHelper.WriteLine("{0} not found. Loading new instance.", typeName);

            return new T();
        }
    }
}