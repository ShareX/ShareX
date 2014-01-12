#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.Collections.Generic;
using System.Linq;

namespace ShareX
{
    public class WatchFolderManager : IDisposable
    {
        public List<WatchFolder> WatchFolders { get; private set; }

        public WatchFolderManager()
        {
            WatchFolders = new List<WatchFolder>();

            foreach (WatchFolderSettings defaultWatchFolderSetting in Program.DefaultTaskSettings.WatchFolderList)
            {
                AddWatchFolder(defaultWatchFolderSetting, Program.DefaultTaskSettings);
            }

            foreach (HotkeySettings hotkeySetting in Program.HotkeysConfig.Hotkeys)
            {
                foreach (WatchFolderSettings watchFolderSetting in hotkeySetting.TaskSettings.WatchFolderList)
                {
                    AddWatchFolder(watchFolderSetting, hotkeySetting.TaskSettings);
                }
            }
        }

        private WatchFolder FindWatchFolder(WatchFolderSettings watchFolderSetting)
        {
            return WatchFolders.FirstOrDefault(watchFolder => watchFolder.Settings == watchFolderSetting);
        }

        private bool IsExist(WatchFolderSettings watchFolderSetting)
        {
            return FindWatchFolder(watchFolderSetting) != null;
        }

        public void AddWatchFolder(WatchFolderSettings watchFolderSetting, TaskSettings taskSettings)
        {
            if (!IsExist(watchFolderSetting))
            {
                if (!taskSettings.WatchFolderList.Contains(watchFolderSetting))
                {
                    taskSettings.WatchFolderList.Add(watchFolderSetting);
                }

                WatchFolder watchFolder = new WatchFolder { Settings = watchFolderSetting, TaskSettings = taskSettings };
                watchFolder.FileWatcherTrigger += path =>
                {
                    TaskSettings taskSettingsCopy = TaskSettings.GetSafeTaskSettings(taskSettings);
                    UploadManager.UploadFile(path, taskSettingsCopy);
                };
                WatchFolders.Add(watchFolder);

                if (taskSettings.WatchFolderEnabled)
                {
                    watchFolder.Enable();
                }
            }
        }

        public void RemoveWatchFolder(WatchFolderSettings watchFolderSetting)
        {
            using (WatchFolder watchFolder = FindWatchFolder(watchFolderSetting))
            {
                if (watchFolder != null)
                {
                    watchFolder.TaskSettings.WatchFolderList.Remove(watchFolderSetting);
                    WatchFolders.Remove(watchFolder);
                }
            }
        }

        public void UpdateWatchFolderState(WatchFolderSettings watchFolderSetting)
        {
            WatchFolder watchFolder = FindWatchFolder(watchFolderSetting);
            if (watchFolder != null)
            {
                if (watchFolder.TaskSettings.WatchFolderEnabled)
                {
                    watchFolder.Enable();
                }
                else
                {
                    watchFolder.Dispose();
                }
            }
        }

        public void Dispose()
        {
            if (WatchFolders != null)
            {
                foreach (WatchFolder watchFolder in WatchFolders)
                {
                    if (watchFolder != null)
                    {
                        watchFolder.Dispose();
                    }
                }
            }
        }
    }
}