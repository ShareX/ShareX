#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public abstract class SettingsBase<T> where T : SettingsBase<T>, new()
    {
        public delegate void SettingsChangedEventHandler(object sender, EventArgs e);
        public event SettingsChangedEventHandler SettingsChanged;

        public delegate void SettingsSavedEventHandler(object sender, EventArgs e);
        public event SettingsSavedEventHandler SettingsSaved;

        [Browsable(false)]
        public string FilePath { get; private set; }

        [Browsable(false)]
        public string ApplicationVersion { get; set; }

        [Browsable(false)]
        public bool IsFirstTimeRun
        {
            get
            {
                return string.IsNullOrEmpty(ApplicationVersion);
            }
        }

        [Browsable(false)]
        public bool IsUpgrade
        {
            get
            {
                return !IsFirstTimeRun && Helpers.CompareApplicationVersion(ApplicationVersion) < 0;
            }
        }

        public void TriggerSettingsChange()
        {
            OnSettingsChanged(EventArgs.Empty);
        }

        protected virtual void OnSettingsChanged(EventArgs e)
        {
            if (SettingsChanged != null)
                SettingsChanged(this, e);
        }

        protected virtual void OnSettingsSaved(EventArgs e)
        {
            if (SettingsSaved != null)
                SettingsSaved(this, e);
        }

        public bool Save(string filePath)
        {
            FilePath = filePath;
            ApplicationVersion = Application.ProductVersion;

            bool result = SaveInternal(this, FilePath, true);
            if (result) OnSettingsSaved(EventArgs.Empty);

            return result;
        }

        public bool Save()
        {
            return Save(FilePath);
        }

        public void SaveAsync(string filePath)
        {
            TaskEx.Run(() => Save(filePath));
        }

        public void SaveAsync()
        {
            SaveAsync(FilePath);
        }

        public static T Load(string filePath)
        {
            T setting = LoadInternal(filePath, true);

            if (setting != null)
            {
                setting.FilePath = filePath;
            }

            return setting;
        }

        private static bool SaveInternal(object obj, string filePath, bool createBackup)
        {
            string typeName = obj.GetType().Name;
            DebugHelper.WriteLine("{0} save started: {1}", typeName, filePath);

            bool isSuccess = false;

            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    lock (obj)
                    {
                        Helpers.CreateDirectoryIfNotExist(filePath);

                        string tempFilePath = filePath + ".temp";

                        using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                        using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            jsonWriter.Formatting = Formatting.Indented;
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.ContractResolver = new WritablePropertiesOnlyResolver();
                            serializer.Converters.Add(new StringEnumConverter());
                            serializer.Serialize(jsonWriter, obj);
                            jsonWriter.Flush();
                        }

                        if (File.Exists(filePath))
                        {
                            if (createBackup)
                            {
                                File.Copy(filePath, filePath + ".bak", true);
                            }

                            File.Delete(filePath);
                        }

                        File.Move(tempFilePath, filePath);

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

        private static T LoadInternal(string filePath, bool checkBackup)
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

                if (checkBackup)
                {
                    return LoadInternal(filePath + ".bak", false);
                }
            }

            DebugHelper.WriteLine("{0} not found. Loading new instance.", typeName);

            return new T();
        }
    }
}