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

using ShareX.HelpersLib;
using ShareX.HistoryLib.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ShareX.HistoryLib
{
    public class HistoryManager
    {
        private XMLManager manager;

        public HistoryManager(string historyPath)
        {
            manager = new XMLManager(historyPath);
        }

        private bool IsValidHistoryItem(HistoryItem historyItem)
        {
            return historyItem != null && !string.IsNullOrEmpty(historyItem.Filename) && historyItem.DateTime != DateTime.MinValue &&
                (!string.IsNullOrEmpty(historyItem.URL) || !string.IsNullOrEmpty(historyItem.Filepath));
        }

        public List<HistoryItem> GetHistoryItems()
        {
            try
            {
                return manager.Load();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);

                MessageBox.Show(string.Format(Resources.HistoryManager_GetHistoryItems_Error_occured_while_reading_XML_file___0_, manager.FilePath) + "\r\n\r\n" + e,
                    "ShareX - " + Resources.HistoryManager_GetHistoryItems_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return new List<HistoryItem>();
        }

        public bool AppendHistoryItem(HistoryItem historyItem)
        {
            try
            {
                if (IsValidHistoryItem(historyItem))
                {
                    return manager.Append(historyItem);
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static void AddHistoryItemAsync(string historyPath, HistoryItem historyItem)
        {
            TaskEx.Run(() =>
            {
                HistoryManager history = new HistoryManager(historyPath);
                history.AppendHistoryItem(historyItem);
            });
        }
    }
}