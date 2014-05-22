#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace HelpersLib
{
    [Serializable]
    public abstract class SettingsBase<T> where T : SettingsBase<T>, new()
    {
        public static readonly SerializationType SerializationType = SerializationType.Json;

        [Browsable(false), XmlIgnore, JsonIgnore]
        public string FilePath { get; private set; }

        public static T Load(string filePath)
        {
            T setting = SettingsHelper.Load<T>(filePath, SerializationType);
            if (setting != null) setting.FilePath = filePath;
            return setting;
        }

        public static T Load(Stream stream)
        {
            T setting = SettingsHelper.Load<T>(stream, SerializationType);
            return setting;
        }

        public virtual bool Save(string filePath)
        {
            FilePath = filePath;
            return SettingsHelper.Save(this, filePath, SerializationType);
        }

        private void Save()
        {
            Save(FilePath);
        }

        public void SaveAsync(string filePath)
        {
            Task.Run(() => Save(filePath));
        }

        private void SaveAsync()
        {
            SaveAsync(FilePath);
        }

        public virtual void Save(Stream stream)
        {
            SettingsHelper.Save(this, stream, SerializationType);
        }
    }
}