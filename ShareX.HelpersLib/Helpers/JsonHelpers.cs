#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.IO;

namespace ShareX.HelpersLib
{
    public static class JsonHelpers
    {
        public static T Deserialize<T>(TextReader textReader)
        {
            using (JsonTextReader jsonTextReader = new JsonTextReader(textReader))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new StringEnumConverter());
                serializer.ObjectCreationHandling = ObjectCreationHandling.Replace;
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                serializer.Error += (sender, e) => e.ErrorContext.Handled = true;
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        public static T DeserializeFromString<T>(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                using (StringReader stringReader = new StringReader(json))
                {
                    return Deserialize<T>(stringReader);
                }
            }

            return default(T);
        }

        public static T DeserializeFromFilePath<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (fileStream.Length > 0)
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            return Deserialize<T>(streamReader);
                        }
                    }
                }
            }

            return default(T);
        }
    }
}