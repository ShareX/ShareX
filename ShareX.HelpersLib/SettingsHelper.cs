#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace ShareX.HelpersLib
{
    public enum SerializationType
    {
        Binary,
        Xml,
        Json
    }

    public static class SettingsHelper
    {
        public static bool Save(object obj, string filePath, SerializationType type, bool createBackup = true)
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
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Save(obj, ms, type);

                            if (createBackup && File.Exists(filePath))
                            {
                                File.Copy(filePath, filePath + ".bak", true);
                            }

                            isSuccess = ms.WriteToFile(filePath);
                        }
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

        public static void Save(object obj, Stream stream, SerializationType type)
        {
            switch (type)
            {
                case SerializationType.Binary:
                    new BinaryFormatter().Serialize(stream, obj);
                    break;
                case SerializationType.Xml:
                    Type t = obj.GetType();
                    new XmlSerializer(t).Serialize(stream, obj);
                    break;
                case SerializationType.Json:
                    StreamWriter streamWriter = new StreamWriter(stream);
                    JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter);
                    jsonWriter.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.ContractResolver = new WritablePropertiesOnlyResolver();
                    serializer.Converters.Add(new StringEnumConverter());
                    serializer.Serialize(jsonWriter, obj);
                    jsonWriter.Flush();
                    break;
            }
        }

        public static T Load<T>(string path, SerializationType type, bool checkBackup = true) where T : new()
        {
            string typeName = typeof(T).Name;

            if (!string.IsNullOrEmpty(path))
            {
                DebugHelper.WriteLine("{0} load started: {1}", typeName, path);

                try
                {
                    if (File.Exists(path))
                    {
                        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            if (fs.Length > 0)
                            {
                                T settings = Load<T>(fs, type);
                                if (settings == null) throw new Exception(typeName + " object is null.");

                                DebugHelper.WriteLine("{0} load finished: {1}", typeName, path);

                                return settings;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, typeName + " load failed");
                }

                if (checkBackup)
                {
                    return Load<T>(path + ".bak", type, false);
                }
            }

            DebugHelper.WriteLine("{0} not found. Loading new instance.", typeName);

            return new T();
        }

        public static T Load<T>(Stream stream, SerializationType type)
        {
            T settings;

            switch (type)
            {
                case SerializationType.Binary:
                    settings = (T)new BinaryFormatter().Deserialize(stream);
                    break;
                default:
                case SerializationType.Xml:
                    settings = (T)new XmlSerializer(typeof(T)).Deserialize(stream);
                    break;
                case SerializationType.Json:
                    using (StreamReader streamReader = new StreamReader(stream))
                    using (JsonReader jsonReader = new JsonTextReader(streamReader))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Converters.Add(new StringEnumConverter());
                        serializer.Error += (sender, e) => e.ErrorContext.Handled = true;
                        serializer.ObjectCreationHandling = ObjectCreationHandling.Replace;
                        settings = serializer.Deserialize<T>(jsonReader);
                    }
                    break;
            }

            return settings;
        }
    }

    internal class WritablePropertiesOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => p.Writable).ToList();
        }
    }
}