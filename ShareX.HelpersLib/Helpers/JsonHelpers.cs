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
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Text;

namespace ShareX.HelpersLib
{
    public static class JsonHelpers
    {
        public static void Serialize<T>(T obj, TextWriter textWriter, DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling nullValueHandling = NullValueHandling.Include, ISerializationBinder serializationBinder = null)
        {
            if (textWriter != null)
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(textWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;

                    JsonSerializer serializer = new JsonSerializer();
                    serializer.ContractResolver = new WritablePropertiesOnlyResolver();
                    serializer.Converters.Add(new StringEnumConverter());
                    serializer.DefaultValueHandling = defaultValueHandling;
                    serializer.NullValueHandling = nullValueHandling;
                    if (serializationBinder != null) serializer.SerializationBinder = serializationBinder;
                    serializer.Serialize(jsonTextWriter, obj);
                }
            }
        }

        public static string SerializeToString<T>(T obj, DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling nullValueHandling = NullValueHandling.Include, ISerializationBinder serializationBinder = null)
        {
            StringBuilder sb = new StringBuilder();

            using (StringWriter stringWriter = new StringWriter(sb))
            {
                Serialize(obj, stringWriter, defaultValueHandling, nullValueHandling, serializationBinder);
            }

            return sb.ToString();
        }

        public static void SerializeToStream<T>(T obj, Stream stream, DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling nullValueHandling = NullValueHandling.Include, ISerializationBinder serializationBinder = null)
        {
            if (stream != null)
            {
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    Serialize(obj, streamWriter, defaultValueHandling, nullValueHandling, serializationBinder);
                }
            }
        }

        public static MemoryStream SerializeToMemoryStream<T>(T obj, DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling nullValueHandling = NullValueHandling.Include, ISerializationBinder serializationBinder = null)
        {
            MemoryStream memoryStream = new MemoryStream();
            SerializeToStream(obj, memoryStream, defaultValueHandling, nullValueHandling, serializationBinder);
            return memoryStream;
        }

        public static void SerializeToFile<T>(T obj, string filePath, DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling nullValueHandling = NullValueHandling.Include, ISerializationBinder serializationBinder = null)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                FileHelpers.CreateDirectoryFromFilePath(filePath);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.WriteThrough))
                {
                    SerializeToStream(obj, fileStream, defaultValueHandling, nullValueHandling, serializationBinder);
                }
            }
        }

        public static T Deserialize<T>(TextReader textReader, ISerializationBinder serializationBinder = null)
        {
            if (textReader != null)
            {
                using (JsonTextReader jsonTextReader = new JsonTextReader(textReader))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new StringEnumConverter());
                    serializer.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    if (serializationBinder != null) serializer.SerializationBinder = serializationBinder;
                    serializer.Error += (sender, e) => e.ErrorContext.Handled = true;
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }

            return default;
        }

        public static T DeserializeFromString<T>(string json, ISerializationBinder serializationBinder = null)
        {
            if (!string.IsNullOrEmpty(json))
            {
                using (StringReader stringReader = new StringReader(json))
                {
                    return Deserialize<T>(stringReader, serializationBinder);
                }
            }

            return default;
        }

        public static T DeserializeFromStream<T>(Stream stream, ISerializationBinder serializationBinder = null)
        {
            if (stream != null)
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    return Deserialize<T>(streamReader, serializationBinder);
                }
            }

            return default;
        }

        public static T DeserializeFromFile<T>(string filePath, ISerializationBinder serializationBinder = null)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (fileStream.Length > 0)
                    {
                        return DeserializeFromStream<T>(fileStream, serializationBinder);
                    }
                }
            }

            return default;
        }

        public static bool QuickVerifyJsonFile(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        if (fileStream.Length > 1 && fileStream.ReadByte() == (byte)'{')
                        {
                            fileStream.Seek(-1, SeekOrigin.End);
                            return fileStream.ReadByte() == (byte)'}';
                        }
                    }
                }
            }
            catch
            {
            }

            return false;
        }
    }
}