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

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    [Serializable]
    public abstract class SettingsBase<T> where T : SettingsBase<T>, new()
    {
        private static readonly SerializationType SerializationType = SerializationType.Json;

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

        public bool Save(string filePath)
        {
            FilePath = filePath;
            ApplicationVersion = Application.ProductVersion;
            return SettingsHelper.Save(this, FilePath, SerializationType);
        }

        public void Save()
        {
            Save(FilePath);
        }

        public void SaveAsync(string filePath)
        {
            TaskEx.Run(() => Save(filePath));
        }

        public void SaveAsync()
        {
            SaveAsync(FilePath);
        }

        public void Save(Stream stream)
        {
            SettingsHelper.Save(this, stream, SerializationType);
        }
    }
}