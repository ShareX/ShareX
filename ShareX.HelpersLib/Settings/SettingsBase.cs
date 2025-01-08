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
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Browsable(false), JsonIgnore]
        public bool SupportDPAPIEncryption { get; set; }

        public bool IsUpgradeFrom(string version)
        {
            return IsUpgrade && Helpers.CompareVersion(ApplicationVersion, version) <= 0;
        }

        protected virtual void OnSettingsSaved(string filePath, bool result)
        {
            SettingsSaved?.Invoke((T)this, filePath, result);
        }

        protected virtual void OnSettingsSaveFailed(Exception e)
        {
            SettingsSaveFailed?.Invoke(e);
        }

        public bool Save(string filePath)
        {
            FilePath = filePath;
            ApplicationVersion = Helpers.GetApplicationVersion();

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

        public MemoryStream SaveToMemoryStream(bool supportDPAPIEncryption = false)
        {
            ApplicationVersion = Helpers.GetApplicationVersion();

            MemoryStream ms = new MemoryStream();
            SaveToStream(ms, supportDPAPIEncryption, true);
            return ms;
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
                        FileHelpers.CreateDirectoryFromFilePath(filePath);

                        string tempFilePath = filePath + ".temp";

                        using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.WriteThrough))
                        {
                            SaveToStream(fileStream, SupportDPAPIEncryption);
                        }

                        if (!JsonHelpers.QuickVerifyJsonFile(tempFilePath))
                        {
                            throw new Exception($"{typeName} file is corrupt: {tempFilePath}");
                        }

                        if (File.Exists(filePath))
                        {
                            string backupFilePath = null;

                            if (CreateBackup)
                            {
                                string fileName = Path.GetFileName(filePath);
                                backupFilePath = Path.Combine(BackupFolder, fileName);
                                FileHelpers.CreateDirectory(BackupFolder);
                            }

                            File.Replace(tempFilePath, filePath, backupFilePath, true);
                        }
                        else
                        {
                            File.Move(tempFilePath, filePath);
                        }

                        if (CreateWeeklyBackup && !string.IsNullOrEmpty(BackupFolder))
                        {
                            FileHelpers.BackupFileWeekly(filePath, BackupFolder);
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

        private void SaveToStream(Stream stream, bool supportDPAPIEncryption = false, bool leaveOpen = false)
        {
            using (StreamWriter streamWriter = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, leaveOpen))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                JsonSerializer serializer = new JsonSerializer();

                if (supportDPAPIEncryption)
                {
                    serializer.ContractResolver = new DPAPIEncryptedStringPropertyResolver();
                }
                else
                {
                    serializer.ContractResolver = new WritablePropertiesOnlyResolver();
                }

                serializer.Converters.Add(new StringEnumConverter());
                serializer.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(jsonWriter, this);
                jsonWriter.Flush();
            }
        }

        public static T Load(string filePath, string backupFolder = null, bool fallbackSupport = true)
        {
            List<string> fallbackFilePaths = new List<string>();

            if (fallbackSupport && !string.IsNullOrEmpty(filePath))
            {
                string tempFilePath = filePath + ".temp";
                fallbackFilePaths.Add(tempFilePath);

                if (!string.IsNullOrEmpty(backupFolder) && Directory.Exists(backupFolder))
                {
                    string fileName = Path.GetFileName(filePath);
                    string backupFilePath = Path.Combine(backupFolder, fileName);
                    fallbackFilePaths.Add(backupFilePath);

                    string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
                    string lastWeeklyBackupFilePath = Directory.GetFiles(backupFolder, fileNameNoExt + "-*").OrderBy(x => x).LastOrDefault();
                    if (!string.IsNullOrEmpty(lastWeeklyBackupFilePath))
                    {
                        fallbackFilePaths.Add(lastWeeklyBackupFilePath);
                    }
                }
            }

            T setting = LoadInternal(filePath, fallbackFilePaths);

            if (setting != null)
            {
                setting.FilePath = filePath;
                setting.IsFirstTimeRun = string.IsNullOrEmpty(setting.ApplicationVersion);
                setting.IsUpgrade = !setting.IsFirstTimeRun && Helpers.CompareApplicationVersion(setting.ApplicationVersion) < 0;
                setting.BackupFolder = backupFolder;
            }

            return setting;
        }

        private static T LoadInternal(string filePath, List<string> fallbackFilePaths = null)
        {
            string typeName = typeof(T).Name;

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                DebugHelper.WriteLine($"{typeName} load started: {filePath}");

                try
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
                                serializer.ContractResolver = new DPAPIEncryptedStringPropertyResolver();
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
                        else
                        {
                            throw new Exception($"{typeName} file stream length is 0.");
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, $"{typeName} load failed: {filePath}");
                }
            }
            else
            {
                DebugHelper.WriteLine($"{typeName} file does not exist: {filePath}");
            }

            if (fallbackFilePaths != null && fallbackFilePaths.Count > 0)
            {
                filePath = fallbackFilePaths[0];
                fallbackFilePaths.RemoveAt(0);
                return LoadInternal(filePath, fallbackFilePaths);
            }

            DebugHelper.WriteLine($"Loading new {typeName} instance.");

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